import copy
import json
import uuid
import httpcore
import yaml
import base64
import threading
from enum import Enum, auto
from typing import Any, List, Union, Dict, Tuple


from anthropic import AsyncAnthropic, APITimeoutError, Anthropic, RateLimitError, AsyncAnthropicBedrock


from agent_c.agents.base import BaseAgent
from agent_c.chat.session_manager import ChatSessionManager
from agent_c.models.input import FileInput
from agent_c.models.input.audio_input import AudioInput
from agent_c.models.input.image_input import ImageInput
from agent_c.prompting import PromptBuilder
from agent_c.util.token_counter import TokenCounter


class ThinkToolState(Enum):
    """Enum representing the state of the think tool processing."""
    INACTIVE = auto()   # Not currently processing a think tool
    WAITING = auto()    # Waiting for the JSON to start
    EMITTING = auto()   # Processing and emitting think tool content


class ClaudeChatAgent(BaseAgent):
    CLAUDE_MAX_TOKENS: int = 64000
    class ClaudeTokenCounter(TokenCounter):

        def __init__(self):
            self.anthropic: Anthropic = Anthropic()

        def count_tokens(self, text: str) -> int:
            response = self.anthropic.messages.count_tokens(
                model="claude-3-5-sonnet-latest",
                system="",
                messages=[{
                    "role": "user",
                    "content": text
                }],
            )

            return response.input_tokens


    def __init__(self, **kwargs) -> None:
        """
        Initialize ChatAgent object.

        Non-Base Parameters:
        client: AsyncAnthropic, default is AsyncAnthropic()
            The client to use for making requests to the Anthropic API.
        max_tokens: int, optional
            The maximum number of tokens to generate in the response.
        """
        kwargs['token_counter'] = kwargs.get('token_counter', ClaudeChatAgent.ClaudeTokenCounter())
        super().__init__(**kwargs)
        self.client: Union[AsyncAnthropic,AsyncAnthropicBedrock] = kwargs.get("client", self.__class__.client())
        self.supports_multimodal = True
        self.can_use_tools = True
        self.allow_betas = kwargs.get("allow_betas", True)

        # JO: I need these as class level variables to adjust outside a chat call.
        self.max_tokens = kwargs.get("max_tokens", self.CLAUDE_MAX_TOKENS)
        self.budget_tokens = kwargs.get("budget_tokens", 0)

    @classmethod
    def client(cls, **opts):
        return AsyncAnthropic(**opts)

    @property
    def tool_format(self) -> str:
        return "claude"

    async def __interaction_setup(self, **kwargs) -> dict[str, Any]:
        model_name: str = kwargs.get("model_name", self.model_name)
        if model_name is None:
            raise ValueError('Claude agent is missing a model_name')

        temperature: float = kwargs.get("temperature", self.temperature)
        max_tokens: int = kwargs.get("max_tokens", self.max_tokens)
        allow_server_tools: bool = kwargs.get("allow_server_tools", False)
        callback_opts = self._callback_opts(**kwargs)
        tool_chest = kwargs.get("tool_chest", self.tool_chest)
        toolsets: List[str] = kwargs.get("toolsets", [])
        if len(toolsets) == 0:
            functions: List[Dict[str, Any]] = tool_chest.active_claude_schemas
        else:
            inference_data = tool_chest.get_inference_data(toolsets, "claude")
            functions: List[Dict[str, Any]] = inference_data['schemas']
            kwargs['tool_sections'] = inference_data['sections']

        messages = await self._construct_message_array(**kwargs)
        kwargs['prompt_metadata']['model_id'] = model_name
        (tool_context, prompt_context) = await self._render_contexts(**kwargs)
        sys_prompt: str = prompt_context["system_prompt"]
        allow_betas: bool = kwargs.get("allow_betas", self.allow_betas)
        completion_opts = {"model": model_name.removeprefix("bedrock_"), "messages": messages,
                           "system": sys_prompt,  "max_tokens": max_tokens,
                           'temperature': temperature}

        if '3-7-sonnet' in model_name or '-4-' in model_name:
            if allow_server_tools:
                max_searches: int = kwargs.get("max_searches", 0)
                if max_searches > 0:
                    functions.append({"type": "web_search_20250305", "name": "web_search", "max_uses": max_searches})

            if allow_betas:
                if allow_server_tools:
                    functions.append({"type": "code_execution_20250522","name": "code_execution"})

                if '-4-' in model_name:
                    if max_tokens == self.CLAUDE_MAX_TOKENS:
                        if 'sonnet' in model_name:
                            completion_opts['max_tokens'] = 64000
                        else:
                            completion_opts['max_tokens'] = 32000

                    completion_opts['betas'] = ['interleaved-thinking-2025-05-14', "files-api-2025-04-14"] #, "code-execution-2025-05-22"]
                else:
                    completion_opts['betas'] = ["token-efficient-tools-2025-02-19", "output-128k-2025-02-19", "files-api-2025-04-14"] # , "code-execution-2025-05-22"]
                    if max_tokens == self.CLAUDE_MAX_TOKENS:
                        completion_opts['max_tokens'] = 128000


        budget_tokens: int = kwargs.get("budget_tokens", self.budget_tokens)
        if budget_tokens > 0:
            completion_opts['thinking'] = {"budget_tokens": budget_tokens, "type": "enabled"}
            completion_opts['temperature'] = 1


        if len(functions):
            completion_opts['tools'] = functions

        completion_opts["metadata"] = {'user_id': kwargs.get('user_id', 'Agent C user')}

        opts = {"callback_opts": callback_opts, "completion_opts": completion_opts, 'tool_chest': tool_chest, 'tool_context': tool_context}
        return opts


    @staticmethod
    def process_escapes(text):
        return text.replace("\\n", "\n").replace('\\"', '"').replace("\\\\", "\\")


    async def chat(self, **kwargs) -> List[dict[str, Any]]:
        """Main method for interacting with Claude API. Split into smaller helper methods for clarity."""
        opts = await self.__interaction_setup(**kwargs)
        prompt_builder: PromptBuilder = kwargs.get("prompt_builder")
        client_wants_cancel: threading.Event = kwargs.get("client_wants_cancel")
        callback_opts = opts["callback_opts"]
        tool_chest = opts['tool_chest']
        session_manager: Union[ChatSessionManager, None] = kwargs.get("session_manager", None)
        messages = opts["completion_opts"]["messages"]
        await self._raise_system_prompt(opts["completion_opts"]["system"], **callback_opts)
        await self._raise_user_request(kwargs.get('user_message', ''), **callback_opts)
        delay = 1  # Initial delay between retries
        async with (self.semaphore):
            interaction_id = await self._raise_interaction_start(**callback_opts)

            while delay <= self.max_delay:
                try:
                    # Stream handling encapsulated in a helper method
                    result, state = await self._handle_claude_stream(
                        opts["completion_opts"],
                        tool_chest,
                        session_manager,
                        messages,
                        callback_opts,
                        interaction_id,
                        client_wants_cancel,
                        opts["tool_context"]
                    )

                    if state['complete'] and state['stop_reason'] != 'tool_use':
                        self.logger.info(f"Interaction {interaction_id} stopped with reason: {state['stop_reason']}")
                        return result

                    new_system_prompt  = await prompt_builder.render(opts['tool_context'], tool_sections=kwargs.get("tool_sections", None))
                    if new_system_prompt != opts["completion_opts"]["system"]:
                        self.logger.debug(f"Updating system prompt for interaction {interaction_id}")
                        opts["completion_opts"]["system"] = new_system_prompt
                        await self._raise_system_prompt(new_system_prompt, **callback_opts)

                    delay = 1
                    messages = result
                except RateLimitError:
                    self.logger.warning(f"Ratelimit. Retrying...Delay is {delay} seconds")
                    await self._raise_system_event(f"Rate limit reach, slowing down... Delay is {delay} seconds \n", severity="warning", **callback_opts)
                    delay = await self._handle_retryable_error(delay)
                except APITimeoutError:
                    self.logger.warning(f"API Timeout. Retrying...Delay is {delay} seconds")
                    await self._raise_system_event(f"Claude API is overloaded, retrying... Delay is {delay} seconds \n", severity="warning", **callback_opts)
                    delay = await self._handle_retryable_error(delay)
                except httpcore.RemoteProtocolError:
                    self.logger.warning(f"Remote protocol error encountered, retrying...Delay is {delay} seconds")
                    await self._raise_system_event(f"Claude API is overloaded, retrying... Delay is {delay} seconds \n", severity="warning", **callback_opts)
                    delay = await self._handle_retryable_error(delay)
                except Exception as e:
                    if "overloaded" in str(e).lower():
                        self.logger.warning(f"Claude API is overloaded, retrying... Delay is {delay} seconds")
                        await self._raise_system_event(f"Claude API is overloaded, retrying... Delay is {delay} seconds \n", severity="warning", **callback_opts)
                        delay = await self._handle_retryable_error(delay)
                    else:
                        self.logger.exception(f"Uncoverable error during Claude chat: {e}", exc_info=True)
                        await self._raise_system_event(f"Exception calling `client.messages.stream`.\n\n{e}\n",  **callback_opts)
                        await self._raise_completion_end(opts["completion_opts"], stop_reason="exception", **callback_opts)
                        return []

        self.logger.warning("ABNORMAL TERMINATION OF CLAUDE CHAT")
        await self._raise_system_event(f"ABNORMAL TERMINATION OF CLAUDE CHAT", **callback_opts)
        await self._raise_completion_end(opts["completion_opts"], stop_reason="overload", **callback_opts)
        return messages


    async def _handle_retryable_error(self, delay):
        """Handle retryable errors with exponential backoff."""
        await self._exponential_backoff(delay)
        return delay * 2  # Return the new delay for the next attempt


    async def _handle_claude_stream(self, completion_opts, tool_chest, session_manager,
                                    messages, callback_opts, interaction_id,
                                    client_wants_cancel: threading.Event,
                                    tool_context: Dict[str, Any]) -> Tuple[List[dict[str, Any]], dict[str, Any]]:
        """Handle the Claude API streaming response."""
        await self._raise_completion_start(completion_opts, **callback_opts)

        # Initialize state trackers
        state = self._init_stream_state()
        state['interaction_id'] = interaction_id

        if "betas" in  completion_opts:
            stream_source = self.client.beta
        else:
            stream_source = self.client


        async with stream_source.messages.stream(**completion_opts) as stream:
            async for event in stream:
                await self._process_stream_event(event, state, tool_chest, session_manager,
                                                 messages, callback_opts)

                if client_wants_cancel.is_set():
                    state['complete'] = True
                    state['stop_reason'] = "client_cancel"

                # If we've reached the end of a non-tool response, return
                if state['complete'] and state['stop_reason'] != 'tool_use':
                    messages.extend(state['server_tool_calls'])
                    messages.extend(state['server_tool_responses'])
                    await self._raise_history_event(messages, **callback_opts)
                    # Add interaction_id to callback_opts to avoid parameter conflicts
                    callback_opts_with_interaction = callback_opts.copy()
                    callback_opts_with_interaction['interaction_id'] = state['interaction_id']
                    await self._raise_interaction_end(**callback_opts_with_interaction)
                    return messages, state

                # If we've reached the end of a tool call response, continue after processing tool calls
                elif state['complete'] and state['stop_reason'] == 'tool_use':
                    await self._finalize_tool_calls(state, tool_chest, session_manager,
                                                  messages, callback_opts, tool_context)

                    messages.extend(state['server_tool_calls'])
                    messages.extend(state['server_tool_responses'])

                    await self._raise_history_event(messages, **callback_opts)
                    return  messages, state

        return messages, state


    def _init_stream_state(self) -> Dict[str, Any]:
        """Initialize the state object for stream processing."""
        return {
            "collected_messages": [],
            "collected_tool_calls": [],
            "server_tool_calls": [],
            "server_tool_responses": [],
            "input_tokens": 0,
            "output_tokens": 0,
            "model_outputs": [],
            "current_block_type": None,
            "current_thought": None,
            "current_agent_msg": None,
            "think_tool_state": ThinkToolState.INACTIVE,
            "think_partial": "",
            "think_escape_buffer": "",  # Added to track escape buffer
            "stop_reason": None,
            "complete": False,
            "interaction_id": None
        }


    async def _process_stream_event(self, event, state, tool_chest, session_manager,
                                   messages, callback_opts):
        """Process a single event from the Claude stream."""
        event_type = event.type

        if event_type == "message_start":
            await self._handle_message_start(event, state, callback_opts)
        elif event_type == "content_block_delta":
            await self._handle_content_block_delta(event, state, callback_opts)
        elif event_type == "message_stop":
            await self._handle_message_stop(event, state, tool_chest, session_manager,
                                          messages, callback_opts)
        elif event_type == "content_block_start":
            await self._handle_content_block_start(event, state, callback_opts)
        elif event_type == "content_block_stop":
            await self._handle_content_block_end(event, state, callback_opts)
        elif event_type == "input_json":
            self._handle_input_json(event, state)
        elif event_type == "text":
            await self._handle_text_event(event, state, callback_opts)
        elif event_type == "message_delta":
            self._handle_message_delta(event, state)


    async def _handle_message_start(self, event, state, callback_opts):
        """Handle the message_start event."""
        state["input_tokens"] = event.message.usage.input_tokens


    async def _handle_content_block_delta(self, event, state, callback_opts):
        """Handle content_block_delta events."""
        delta = event.delta

        if delta.type == "signature_delta":
            state['current_thought']['signature'] = delta.signature
        elif delta.type == "thinking_delta":
            await self._handle_thinking_delta(delta, state, callback_opts)
        elif delta.type == "input_json_delta":
            await self._handle_think_tool_json(delta, state, callback_opts)


    async def _handle_thinking_delta(self, delta, state, callback_opts):
        """Handle thinking delta events."""
        if state['current_block_type'] == "redacted_thinking":
            state['current_thought']['data'] = state['current_thought']['data'] + delta.data
        else:
            state['current_thought']['thinking'] = state['current_thought']['thinking'] + delta.thinking
            await self._raise_thought_delta(delta.thinking, **callback_opts)


    async def _handle_think_tool_json(self, delta, state, callback_opts):
        """Handle the special case of processing the think tool JSON."""
        j = delta.partial_json
        think_tool_state = state['think_tool_state']

        if think_tool_state == ThinkToolState.WAITING:
            state['think_partial'] = state['think_partial'] + j

            # Check if we've received the opening part of the thought
            prefix = '{"thought": "'
            if prefix in state['think_partial']:
                await self._start_emitting_thought(state, callback_opts)

        elif think_tool_state == ThinkToolState.EMITTING:
            # Add new content to our escape sequence buffer
            state['think_escape_buffer'] = state['think_escape_buffer'] + j

            # If we don't end with backslash, we can process
            if not state['think_escape_buffer'].endswith('\\'):
                await self._process_thought_buffer(state, callback_opts)


    async def _start_emitting_thought(self, state, callback_opts):
        """Start emitting thought content after finding the opening JSON."""
        prefix = '{"thought": "'
        start_pos = state['think_partial'].find(prefix) + len(prefix)
        content = state['think_partial'][start_pos:]

        # Start buffering content for escape sequence handling
        state['think_partial'] = ""
        state['think_escape_buffer'] = content
        state['think_tool_state'] = ThinkToolState.EMITTING

        # Process and emit if we don't have a partial escape sequence
        if not state['think_escape_buffer'].endswith('\\'):
            await self._process_thought_buffer(state, callback_opts)


    async def _process_thought_buffer(self, state, callback_opts):
        """Process the thought buffer, handling escape sequences."""
        processed = self.process_escapes(state['think_escape_buffer'])
        state['think_escape_buffer'] = ""  # Clear the buffer after processing
        complete: bool = False

        # Check if we've hit the end of the JSON
        if processed.endswith('"}'):
            state['think_tool_state'] = ThinkToolState.INACTIVE
            # Remove closing quote and brace
            processed = processed[:-2]
            complete = True

        await self._raise_thought_delta(processed, **callback_opts)
        if complete:
            await self._raise_complete_thought(processed, **callback_opts)


    async def _handle_message_stop(self, event, state, tool_chest, session_manager, messages, callback_opts):
        """Handle the message_stop event."""
        state['output_tokens'] = event.message.usage.output_tokens
        state['complete'] = True

        # Completion end event
        await self._raise_completion_end(
            callback_opts.get('completion_opts', {}),
            stop_reason=state['stop_reason'],
            input_tokens=state['input_tokens'],
            output_tokens=state['output_tokens'],
            **callback_opts
        )

        # Save interaction to session
        #assistant_content = self._format_model_outputs_to_text(state['model_outputs'])
        #await self._save_interaction_to_session(session_manager, assistant_content)

        # Update messages
        msg = {'role': 'assistant', 'content': state['model_outputs']}
        messages.append(msg)
        await self._raise_history_delta([msg], **callback_opts)
        if session_manager is not None:
            session_manager.active_memory.messages = messages

    async def _handle_content_block_end(self, event, state, callback_opts):
        if state['current_block_type'] == "thinking":
            await self._raise_complete_thought(state['current_thought']['thinking'], **callback_opts)

    async def _handle_content_block_start(self, event, state, callback_opts):
        """Handle the content_block_start event."""
        state['current_block_type'] = event.content_block.type
        state['current_agent_msg'] = None
        state['current_thought'] = None
        state['think_tool_state'] = ThinkToolState.INACTIVE
        state['think_partial'] = ""

        if state['current_block_type'] == "text":
            await self._handle_text_block_start(event, state, callback_opts)
        elif state['current_block_type'] == "tool_use":
            await self._handle_tool_use_block(event, state, callback_opts)
        elif state['current_block_type'] == "server_tool_use":
            await self._handle_server_tool_use_block(event, state, callback_opts)
        elif state['current_block_type'] in ['web_search_tool_result', 'code_execution_tool_result']:
            state['server_tool_responses'].append(event.content_block.model_dump())
        elif state['current_block_type'] in ["thinking", "redacted_thinking"]:
            await self._handle_thinking_block(event, state, callback_opts)
        else:
            self.logger.warning(f"content_block_start Unknown content type: {state['current_block_type']}")


    async def _handle_text_block_start(self, event, state, callback_opts):
        """Handle text block start event."""
        content = event.content_block.text
        state['current_agent_msg'] = copy.deepcopy(event.content_block.model_dump())
        state['model_outputs'].append(state['current_agent_msg'])
        if len(content) > 0:
            await self._raise_text_delta(content, **callback_opts)


    async def _handle_tool_use_block(self, event, state, callback_opts):
        """Handle tool use block event."""
        tool_call = event.content_block.model_dump()
        if event.content_block.name == "think":
            state['think_tool_state'] = ThinkToolState.WAITING

        state['collected_tool_calls'].append(tool_call)
        await self._raise_tool_call_delta(state['collected_tool_calls'] + state['server_tool_calls'], **callback_opts)



    async def _handle_server_tool_use_block(self, event, state, callback_opts):
        """Handle tool use block event."""
        tool_call = event.content_block.model_dump()
        state['server_tool_calls'].append(tool_call)
        await self._raise_tool_call_delta(state['collected_tool_calls'] + state['server_tool_calls'], **callback_opts)

    async def _handle_thinking_block(self, event, state, callback_opts):
        """Handle thinking block event."""
        state['current_thought'] = copy.deepcopy(event.content_block.model_dump())
        state['model_outputs'].append(state['current_thought'])

        if state['current_block_type'] == "redacted_thinking":
            content = "*redacted*"
        else:
            content = state['current_thought']['thinking']

        await self._raise_thought_delta(content, **callback_opts)


    def _handle_input_json(self, event, state):
        """Handle input_json event."""
        if state['collected_tool_calls']:
            state['collected_tool_calls'][-1]['input'] = event.snapshot


    async def _handle_text_event(self, event, state, callback_opts):
        """Handle text event."""
        if state['current_block_type'] == "text":
            state['current_agent_msg']['text'] = state['current_agent_msg']['text'] + event.text
            await self._raise_text_delta(event.text, **callback_opts)
        elif state['current_block_type'] in ["thinking", "redacted_thinking"]:
            if state['current_block_type'] == "redacted_thinking":
                state['current_thought']['data'] = state['current_thought']['data'] + event.data
            else:
                state['current_thought']['thinking'] = state['current_thought']['thinking'] + event.text
                await self._raise_thought_delta(event.text, **callback_opts)


    def _handle_message_delta(self, event, state):
        """Handle message_delta event."""
        state['stop_reason'] = event.delta.stop_reason


    # async def _finalize_tool_calls(self, state, tool_chest, session_manager, messages, callback_opts, tool_context):
    #     """Finalize tool calls after receiving a complete message."""
    #     await self._raise_tool_call_start(state['collected_tool_calls'], vendor="anthropic", **callback_opts)
    #
    #     # Process tool calls and get response messages
    #     tool_response_messages = await self.__tool_calls_to_messages(
    #         state,
    #         tool_chest,
    #         tool_context
    #     )
    #
    #
    #     # Add tool response messages to the conversation history
    #     messages.extend(tool_response_messages)
    #
    #
    #
    #     await self._raise_tool_call_end(
    #         state['collected_tool_calls'],
    #         messages[-1]['content'],
    #         vendor="anthropic",
    #         **callback_opts
    #     )


    async def _finalize_tool_calls(
            self,
            state,
            tool_chest,
            session_manager,
            *args,
            **kwargs
    ):
        tool_results = []
        tool_calls = state.get("tool_calls", [])

        # Run tools
        for tool_call in tool_calls:
            tool_name = tool_call.get("name")
            tool_input = tool_call.get("input")

            if tool_name and tool_chest.has_tool(tool_name):
                try:
                    result = await tool_chest.run_tool(
                        tool_name, tool_input, session_manager=session_manager
                    )
                    tool_results.append({"id": tool_call.get("id"), "output": result})
                except Exception as e:
                    tool_results.append({"id": tool_call.get("id"), "error": str(e)})
            else:
                tool_results.append({"id": tool_call.get("id"), "error": f"Unknown tool: {tool_name}"})

        # ✅ session_id handling
        session_id = state.get("session_id") or getattr(session_manager, "session_id", None)
        if not session_id:
            session_id = str(uuid.uuid4())

        # ✅ role fallback
        role = state.get("role") or "assistant"

        await self._raise_tool_call_end(
            {
                "tool_calls": tool_calls,
                "tool_results": tool_results,
                "vendor": "anthropic",
                "session_id": session_id,
                "role": role,
            }
        )

        return state

    async def _generate_multi_modal_user_message(self, user_input: str, images: List[ImageInput], audio: List[AudioInput],
                                           files: List[FileInput]) -> Union[List[dict[str, Any]], None]:
        """
        Generates a multimodal message containing text, images, and file content.

        This method formats various input types into a structure that can be sent to
        the Claude API, adhering to the Anthropic message format.

        Args:
            user_input (str): The user's text message
            images (List[ImageInput]): List of image inputs to include
            audio (List[AudioInput]): List of audio inputs (not directly supported by Claude)
            files (List[FileInput]): List of file inputs to include

        Returns:
            Union[List[dict[str, Any]], None]: Formatted message content for Claude
        """
        contents = []

        # Add images first
        for image in images:
            if image.content is None and image.url is not None:
                self.logger.warning(
                    f"ImageInput has no content and Claude doesn't support image URLs. Skipping image {image.url}")
                continue

            img_source = {"type": "base64", "media_type": image.content_type, "data": image.content}
            contents.append({"type": "image", "source": img_source})

        # Process file content
        file_content_blocks = []
        if files:
            self.logger.info(f"Processing {len(files)} file inputs in Claude _generate_multi_modal_user_message")

            for idx, file in enumerate(files):
                extracted_text = None
                if self.allow_betas:
                    try:
                        file_upload = await self.client.beta.files.upload(file=(file.file_name, base64.b64decode(file.content), file.content_type))
                        contents.append({"type": "document", "source": {"type": "file","file_id": file_upload.id}})
                    except Exception as e:
                        self.logger.exception(f"Error uploading file {file.file_name}: {e}", exc_info=True)
                        continue
                elif "pdf" in file.content_type.lower() or ".pdf" in str(file.file_name).lower():
                    pdf_source = {"type": "base64", "media_type": file.content_type, "data": file.content}
                    contents.append({"type": "document", "source": pdf_source,"cache_control": {"type": "ephemeral"}})
                else:
                    # Check if get_text_content method exists and call it
                    if hasattr(file, 'get_text_content') and callable(file.get_text_content):
                        extracted_text = file.get_text_content()
                        self.logger.info(
                            f"Claude: File {idx} ({file.file_name}): get_text_content() returned {len(extracted_text) if extracted_text else 0} chars")

                    if extracted_text:
                        file_name = file.file_name or "unknown file"
                        content_block = f"Content from file {file_name}:\n\n{extracted_text}"

                        file_content_blocks.append(content_block)
                        self.logger.info(f"Claude: File {idx} ({file.file_name}): Added extracted text to message")
                    else:
                        # Fall back to mentioning the file without content
                        file_name = file.file_name or "unknown file"
                        file_content_blocks.append(f"[File attached: {file_name} (content could not be extracted)]")
                        self.logger.warning(
                            f"Claude: File {idx} ({file.file_name}): No text content available, adding file name only")

        # Prepare the main text content with file content
        main_text = user_input or ""

        # If we have file content blocks, add them before the user message
        if file_content_blocks:
            all_file_content = "\n\n".join(file_content_blocks)
            main_text = f"{all_file_content}\n\n{main_text}"

        # Add the combined text as the final content block
        contents.append({"type": "text", "text": main_text})

        # For audio clips, since Claude doesn't support audio directly, just log a warning
        if audio and len(audio) > 0:
            self.logger.warning(
                f"Claude does not directly support audio input. Mentioned {len(audio)} audio clips in text.")

        return [{"role": "user", "content": contents}]

    async def __tool_calls_to_messages(self, state, tool_chest, tool_context):
        # Use the new centralized tool call handling in ToolChest
        tools_calls = await tool_chest.call_tools(state['collected_tool_calls'], tool_context, format_type="claude")

        return tools_calls

    def _format_model_outputs_to_text(self, model_outputs: List[Dict[str, Any]]) -> str:
        """
        Convert Claude's model outputs into a single text string for session storage.
        CREATED BY JOE: May not be the best way
        Args:
            model_outputs: List of output blocks from Claude

        Returns:
            str: Combined text content from all text blocks
        """
        text_parts = []
        for output in model_outputs:
            if 'text' in output:
                text_parts.append(output['text'])
            elif 'thinking' in output:
                # use one or the other below lines for adding to message history, for now we're going to exclude
                # text_parts.append(f"[Thinking] {output['thinking']}") # Add thinking blocks to the output
                pass  # Skip thinking blocks by default

        return "".join(text_parts)

    async def process_tool_response_messages(self, session_manager, tool_response_messages):
        """
        Process the tool response messages from the assistant.
        CREATED BY JOE: May not be the best way
        Args:
            session_manager: to save messages to for chat history
            tool_response_messages: incoming tool response messages

        Returns:

        """
        # Process the assistant's tool call (first message) - has role of Assistant
        assistant_message = tool_response_messages[0]
        if isinstance(assistant_message.get('content', ''), list):
            content = json.dumps(assistant_message['content'])
        else:
            content = assistant_message.get('content', '')
        prefixed_content = "[Tool Call] " + content


        # Process the tool result message (second message, if it exists)
        if len(tool_response_messages) > 1:
            tool_result = tool_response_messages[1]
            if isinstance(tool_result.get('content', ''), list):
                content = json.dumps(tool_result['content'])
            else:
                content = tool_result.get('content', '')

            # Save the tool result as 'assistant' with a prefix to indicate it was a tool response. This is because Claude
            # does not like you passing in prior messages with a role of tool
            tool_response_content = "[Tool Response] " + content
            await self._save_message_to_session(session_manager, tool_response_content, 'assistant')

    async def one_shot_sync(self, **kwargs) -> str:
        """For text in, text out processing. without chat"""
        messages = await self.chat_sync(**kwargs)
        if len(messages) > 0:
            return yaml.dump(messages[-1])

        return "Unknown issue no messages returned please try again later"

    async def chat_sync(self, **kwargs) -> List[dict[str, Any]]:
        """
        Non-streaming chat method that returns the complete response directly.
        
        This method provides the same functionality as chat() but without emitting
        intermediate streaming events. It's ideal for tool usage, batch processing,
        or when you don't need real-time response streaming.
        
        Args:
            **kwargs: Same parameters as chat() method:
                - user_message (str): The user's message
                - messages (List[dict], optional): Existing conversation history
                - tool_chest (ToolChest, optional): Tools available to the agent
                - session_manager (ChatSessionManager, optional): Session management
                - model_name (str, optional): Override default model
                - temperature (float, optional): Override default temperature
                - max_tokens (int, optional): Override default max tokens
                - budget_tokens (int, optional): Thinking budget tokens
                - toolsets (List[str], optional): Specific toolsets to use
                - client_wants_cancel (threading.Event, optional): Cancellation signal
                - emit_tool_events (bool, optional): Whether to emit tool call events
                - All other parameters from base chat() method
        
        Returns:
            List[dict[str, Any]]: Complete conversation messages including:
                - Original messages
                - Assistant responses
                - Tool calls and responses
                
        Raises:
            Same exceptions as chat() method:
                - ValueError: For invalid parameters
                - APITimeoutError: For API timeouts (after retries)
                - RateLimitError: For rate limits (after retries)
                - Exception: For other API errors
                
        Note:
            - Does not emit intermediate streaming events
            - Emits minimal events: interaction_start, interaction_end, errors
            - Tool calls are processed synchronously in sequence
            - Compatible with all existing session managers and tool chests
        """
        opts = await self.__interaction_setup(**kwargs)
        client_wants_cancel: threading.Event = kwargs.get("client_wants_cancel")
        emit_tool_events: bool = kwargs.get("emit_tool_events", False)
        callback_opts = opts["callback_opts"]
        tool_chest = opts['tool_chest']
        session_manager: Union[ChatSessionManager, None] = kwargs.get("session_manager", None)
        messages = opts["completion_opts"]["messages"].copy()  # Work with a copy
        tool_context = opts["tool_context"]
        
        delay = 1  # Initial delay between retries
        async with (self.semaphore):
            interaction_id = await self._raise_interaction_start(**callback_opts)
            
            while delay <= self.max_delay:
                try:
                    # Check for cancellation
                    if client_wants_cancel.is_set():
                        self.logger.info("Chat cancelled by client")
                        # Add interaction_id to callback_opts to avoid parameter conflicts
                        callback_opts_with_interaction = callback_opts.copy()
                        callback_opts_with_interaction['interaction_id'] = interaction_id
                        callback_opts_with_interaction['stop_reason'] = "client_cancel"
                        await self._raise_interaction_end(**callback_opts_with_interaction)
                        return messages
                    
                    # Process conversation with tool calls until completion
                    final_messages = await self._handle_claude_sync(
                        opts["completion_opts"],
                        tool_chest,
                        session_manager,
                        messages,
                        callback_opts,
                        interaction_id,
                        client_wants_cancel,
                        tool_context,
                        emit_tool_events
                    )
                    
                    # Add interaction_id to callback_opts to avoid parameter conflicts
                    callback_opts_with_interaction = callback_opts.copy()
                    callback_opts_with_interaction['interaction_id'] = interaction_id
                    await self._raise_interaction_end(**callback_opts_with_interaction)
                    return final_messages
                    
                except (APITimeoutError, RateLimitError) as e:
                    # Exponential backoff handled in a helper method
                    delay = await self._handle_retryable_error(delay)
                    self.logger.warning(f"Timeout / Ratelimit. Retrying...Delay is {delay} seconds")
                except Exception as e:
                    if "overloaded" in str(e).lower():
                        self.logger.warning(f"Claude API is overloaded. Retrying... Delay is {delay} seconds")
                        delay = await self._handle_retryable_error(delay)
                    else:
                        self.logger.exception(f"Unrecoverable error during Claude chat_sync: {e}", exc_info=True)
                        await self._raise_system_event(f"Exception calling `client.messages.create`.\n\n{e}\n", **callback_opts)
                        # Add interaction_id to callback_opts to avoid parameter conflicts
                        callback_opts_with_interaction = callback_opts.copy()
                        callback_opts_with_interaction['interaction_id'] = interaction_id
                        callback_opts_with_interaction['stop_reason'] = "exception"
                        await self._raise_interaction_end(**callback_opts_with_interaction)
                        raise
            
            self.logger.warning("Claude API is overloaded. GIVING UP")
            await self._raise_system_event(f"Claude API is overloaded. GIVING UP.\n", **callback_opts)
            # Add interaction_id to callback_opts to avoid parameter conflicts
            callback_opts_with_interaction = callback_opts.copy()
            callback_opts_with_interaction['interaction_id'] = interaction_id
            callback_opts_with_interaction['stop_reason'] = "overload"
            await self._raise_interaction_end(**callback_opts_with_interaction)
            return messages
    
    async def _handle_claude_sync(self, completion_opts, tool_chest, session_manager,
                                  messages, callback_opts, interaction_id,
                                  client_wants_cancel: threading.Event,
                                  tool_context: Dict[str, Any],
                                  emit_tool_events: bool = False) -> List[dict[str, Any]]:
        """
        Handle the Claude API non-streaming response with tool call processing.
        """
        # Work with a copy of completion_opts to avoid modifying the original
        current_completion_opts = completion_opts.copy()
        current_messages = messages.copy()
        
        # Process conversation until no more tool calls
        max_iterations = 10  # Prevent infinite loops
        iteration = 0
        
        while iteration < max_iterations:
            iteration += 1
            
            # Check for cancellation
            if client_wants_cancel.is_set():
                self.logger.info("Chat cancelled by client during processing")
                return current_messages
            
            # Update messages in completion options
            current_completion_opts["messages"] = current_messages
            
            # Make the API call
            self.logger.debug(f"Making non-streaming API call (iteration {iteration})")
            response = await self.client.beta.messages.create(**current_completion_opts)
            
            # Add assistant response to messages
            assistant_message = {"role": "assistant", "content": response.content}
            current_messages.append(assistant_message)
            
            # Update session manager if present
            if session_manager is not None:
                session_manager.active_memory.messages = current_messages
            
            # Check if we're done (no tool calls)
            if response.stop_reason != "tool_use":
                self.logger.info(f"Conversation completed with stop_reason: {response.stop_reason}")
                break
            
            # Extract tool calls from response content
            tool_calls = [block for block in response.content if hasattr(block, 'type') and block.type == "tool_use"]
            
            if not tool_calls:
                self.logger.info("No tool calls found despite tool_use stop reason")
                break
            
            self.logger.info(f"Processing {len(tool_calls)} tool call(s)")
            
            # Emit tool call start event if requested
            if emit_tool_events:
                await self._raise_tool_call_start(tool_calls, vendor="anthropic", **callback_opts)
            
            # Convert tool calls to the format expected by tool_chest
            formatted_tool_calls = []
            for tool_call in tool_calls:
                formatted_tool_calls.append({
                    "id": tool_call.id,
                    "name": tool_call.name,
                    "input": tool_call.input
                })
            
            # Execute tool calls
            try:
                tool_response_messages = await tool_chest.call_tools(
                    formatted_tool_calls, 
                    tool_context, 
                    format_type="claude"
                )
                
                # Add tool responses to conversation
                current_messages.extend(tool_response_messages)
                
                # Emit tool call end event if requested
                if emit_tool_events:
                    await self._raise_tool_call_end(
                        formatted_tool_calls,
                        tool_response_messages,
                        vendor="anthropic",
                        **callback_opts
                    )
                
                # Update session manager with tool responses
                if session_manager is not None:
                    session_manager.active_memory.messages = current_messages
                    
            except Exception as e:
                self.logger.exception(f"Error executing tool calls: {e}", exc_info=True)
                # Add error message and continue
                error_message = {
                    "role": "user",
                    "content": [{
                        "type": "tool_result",
                        "tool_use_id": tool_calls[0].id if tool_calls else "unknown",
                        "content": f"Error executing tool: {str(e)}",
                        "is_error": True
                    }]
                }
                current_messages.append(error_message)
                
                if emit_tool_events:
                    await self._raise_tool_call_end(
                        formatted_tool_calls,
                        [error_message],
                        vendor="anthropic",
                        **callback_opts
                    )
        
        if iteration >= max_iterations:
            self.logger.warning(f"Reached maximum iterations ({max_iterations}) in tool call loop")
        
        return current_messages

class ClaudeBedrockChatAgent(ClaudeChatAgent):
    def __init__(self, **kwargs) -> None:
        kwargs['allow_betas'] = False
        super().__init__(**kwargs)

    @classmethod
    def client(cls, **opts):
        return AsyncAnthropicBedrock(**opts)

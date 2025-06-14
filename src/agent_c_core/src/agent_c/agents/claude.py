import copy
import json

import httpcore
import yaml
import base64
import threading
from enum import Enum, auto
from typing import Any, List, Union, Dict, Tuple


from anthropic import AsyncAnthropic, APITimeoutError, Anthropic, RateLimitError, AsyncAnthropicBedrock


from agent_c.agents.base import BaseAgent
from agent_c.chat.session_manager import ChatSessionManager
from agent_c.models.context.interaction_context import InteractionContext
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


    async def chat(self, context: InteractionContext,  **kwargs) -> List[dict[str, Any]]:
        """Main method for interacting with Claude API. Split into smaller helper methods for clarity."""
        opts = await self.__interaction_setup(**kwargs)
        prompt_builder: PromptBuilder = kwargs.get("prompt_builder")
        callback_opts = opts["callback_opts"]
        tool_chest = opts['tool_chest']
        session_manager: Union[ChatSessionManager, None] = kwargs.get("session_manager", None)
        messages = opts["completion_opts"]["messages"]

        await self._raise_system_prompt(context, opts["completion_opts"]["system"])
        await self._raise_user_request(context)

        delay = 1  # Initial delay between retries
        async with (self.semaphore):
            await self._raise_interaction_start(context)

            while delay <= self.max_delay:
                try:
                    # Stream handling encapsulated in a helper method
                    result, state = await self._handle_claude_stream(
                        opts["completion_opts"],
                        messages,
                        callback_opts,
                        opts["tool_context"]
                    )

                    if state['complete'] and state['stop_reason'] != 'tool_use':
                        self.logger.info(f"Interaction {context.interaction_id} stopped with reason: {state['stop_reason']}")
                        return result

                    new_system_prompt  = await prompt_builder.render(opts['tool_context'], tool_sections=kwargs.get("tool_sections", None))
                    if new_system_prompt != opts["completion_opts"]["system"]:
                        opts["completion_opts"]["system"] = new_system_prompt
                        await self._raise_system_prompt(context, new_system_prompt)

                    delay = 1
                    messages = result
                except RateLimitError:
                    self.logger.warning(f"Ratelimit. Retrying...Delay is {delay} seconds")
                    await self._raise_system_event(context, f"Rate limit reach, slowing down... Delay is {delay} seconds \n", severity="warning")
                    await self._raise_completion_end(context, opts["completion_opts"], stop_reason="rate_limit")
                    delay = await self._handle_retryable_error(delay)
                except APITimeoutError:
                    self.logger.warning(f"API Timeout. Retrying...Delay is {delay} seconds")
                    await self._raise_system_event(context, "Claude API is overloaded (timeout), retrying... Delay is {delay} seconds \n", severity="warning")
                    await self._raise_completion_end(context, opts["completion_opts"], stop_reason="overload")
                    delay = await self._handle_retryable_error(delay)
                except httpcore.RemoteProtocolError:
                    self.logger.warning(f"Remote protocol error encountered, retrying...Delay is {delay} seconds")
                    await self._raise_system_event(context,f"Claude API is overloaded (remote protocol error), retrying... Delay is {delay} seconds \n", severity="warning")
                    await self._raise_completion_end(context, opts["completion_opts"], stop_reason="overload")
                    delay = await self._handle_retryable_error(delay)
                except Exception as e:
                    if "overloaded" in str(e).lower():
                        self.logger.warning(f"Claude API is overloaded, retrying... Delay is {delay} seconds")
                        await self._raise_system_event(context, f"Claude API is overloaded, retrying... Delay is {delay} seconds \n", severity="warning")
                        await self._raise_completion_end(context, opts["completion_opts"], stop_reason="overload")
                        delay = await self._handle_retryable_error(delay)
                    else:
                        self.logger.exception(f"Uncoverable error during Claude chat: {e}", exc_info=True)
                        await self._raise_system_event(context, f"Exception calling `client.messages.stream`.\n\n{e}\n")
                        await self._raise_completion_end(context, opts["completion_opts"], stop_reason="exception")
                        await self._raise_interaction_end(context)
                        return messages

        self.logger.warning("ABNORMAL TERMINATION OF CLAUDE CHAT")
        await self._raise_system_event(context,f"ABNORMAL TERMINATION OF CLAUDE CHAT")
        await self._raise_completion_end(context, opts["completion_opts"], stop_reason="overload")
        await self._raise_interaction_end(context)
        return messages


    async def _handle_retryable_error(self, delay):
        """Handle retryable errors with exponential backoff."""
        await self._exponential_backoff(delay)
        return delay * 2  # Return the new delay for the next attempt


    async def _handle_claude_stream(self, context: InteractionContext, completion_opts,
                                    messages, tool_context: Dict[str, Any]) -> Tuple[List[dict[str, Any]], dict[str, Any]]:
        """Handle the Claude API streaming response."""
        await self._raise_completion_start(context, completion_opts)

        # Initialize state trackers
        state = self._init_stream_state()
        state['interaction_id'] = context.interaction_id

        if "betas" in  completion_opts:
            stream_source = self.client.beta
        else:
            stream_source = self.client


        async with stream_source.messages.stream(**completion_opts) as stream:
            async for event in stream:
                await self._process_stream_event(event, state, context.tool_chest, messages)

                if context.client_wants_cancel.is_set():
                    state['complete'] = True
                    state['stop_reason'] = "client_cancel"

                # If we've reached the end of a non-tool response, return
                if state['complete'] and state['stop_reason'] != 'tool_use':
                    messages.extend(state['server_tool_calls'])
                    messages.extend(state['server_tool_responses'])
                    await self._raise_history_event(context, messages)
                    await self._raise_interaction_end(context)
                    return messages, state

                # If we've reached the end of a tool call response, continue after processing tool calls
                elif state['complete'] and state['stop_reason'] == 'tool_use':
                    await self._finalize_tool_calls(state, context, messages)

                    messages.extend(state['server_tool_calls'])
                    messages.extend(state['server_tool_responses'])

                    await self._raise_history_event(context, messages)
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


    async def _process_stream_event(self, context: InteractionContext, event, state, messages):
        """Process a single event from the Claude stream."""
        event_type = event.type

        if event_type == "message_start":
            await self._handle_message_start(event, state)
        elif event_type == "content_block_delta":
            await self._handle_content_block_delta(event, state, context)
        elif event_type == "message_stop":
            await self._handle_message_stop(event, state, messages, context)
        elif event_type == "content_block_start":
            await self._handle_content_block_start(event, state, context)
        elif event_type == "content_block_stop":
            await self._handle_content_block_end(event, state, context)
        elif event_type == "input_json":
            self._handle_input_json(event, state)
        elif event_type == "text":
            await self._handle_text_event(event, state, context)
        elif event_type == "message_delta":
            self._handle_message_delta(event, state)


    async def _handle_message_start(self, event, state):
        """Handle the message_start event."""
        state["input_tokens"] = event.message.usage.input_tokens


    async def _handle_content_block_delta(self, event, state, context: InteractionContext):
        """Handle content_block_delta events."""
        delta = event.delta

        if delta.type == "signature_delta":
            state['current_thought']['signature'] = delta.signature
        elif delta.type == "thinking_delta":
            await self._handle_thinking_delta(delta, state, context)
        elif delta.type == "input_json_delta":
            await self._handle_think_tool_json(delta, state, context)


    async def _handle_thinking_delta(self, delta, state, context: InteractionContext):
        """Handle thinking delta events."""
        if state['current_block_type'] == "redacted_thinking":
            state['current_thought']['data'] = state['current_thought']['data'] + delta.data
        else:
            state['current_thought']['thinking'] = state['current_thought']['thinking'] + delta.thinking
            await self._raise_thought_delta(context, delta.thinking)


    async def _handle_think_tool_json(self, delta, state, context):
        """Handle the special case of processing the think tool JSON."""
        j = delta.partial_json
        think_tool_state = state['think_tool_state']

        if think_tool_state == ThinkToolState.WAITING:
            state['think_partial'] = state['think_partial'] + j

            # Check if we've received the opening part of the thought
            prefix = '{"thought": "'
            if prefix in state['think_partial']:
                await self._start_emitting_thought(state, context)

        elif think_tool_state == ThinkToolState.EMITTING:
            # Add new content to our escape sequence buffer
            state['think_escape_buffer'] = state['think_escape_buffer'] + j

            # If we don't end with backslash, we can process
            if not state['think_escape_buffer'].endswith('\\'):
                await self._process_thought_buffer(state, context)


    async def _start_emitting_thought(self, state, context):
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
            await self._process_thought_buffer(state, context)


    async def _process_thought_buffer(self, state, context):
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

        await self._raise_thought_delta(context, processed)
        if complete:
            await self._raise_complete_thought(context, processed)


    async def _handle_message_stop(self, event, state, messages, context):
        """Handle the message_stop event."""
        state['output_tokens'] = event.message.usage.output_tokens
        state['complete'] = True

        # Completion end event
        await self._raise_completion_end(context, state['stop_reason'], state['input_tokens'], state['output_tokens'])

        # Update messages
        msg = {'role': 'assistant', 'content': state['model_outputs']}
        messages.append(msg)
        await self._raise_history_delta(context, [msg])

    async def _handle_content_block_end(self, _, state, context):
        if state['current_block_type'] == "thinking":
            await self._raise_complete_thought(context, state['current_thought']['thinking'])

    async def _handle_content_block_start(self, event, state, context: InteractionContext):
        """Handle the content_block_start event."""
        state['current_block_type'] = event.content_block.type
        state['current_agent_msg'] = None
        state['current_thought'] = None
        state['think_tool_state'] = ThinkToolState.INACTIVE
        state['think_partial'] = ""

        if state['current_block_type'] == "text":
            await self._handle_text_block_start(event, state, context)
        elif state['current_block_type'] == "tool_use":
            await self._handle_tool_use_block(event, state, context)
        elif state['current_block_type'] == "server_tool_use":
            await self._handle_server_tool_use_block(event, state, context)
        elif state['current_block_type'] in ['web_search_tool_result', 'code_execution_tool_result']:
            state['server_tool_responses'].append(event.content_block.model_dump())
        elif state['current_block_type'] in ["thinking", "redacted_thinking"]:
            await self._handle_thinking_block(event, state, context)
        else:
            self.logger.warning(f"content_block_start Unknown content type: {state['current_block_type']}")


    async def _handle_text_block_start(self, event, state, context):
        """Handle text block start event."""
        content = event.content_block.text
        state['current_agent_msg'] = copy.deepcopy(event.content_block.model_dump())
        state['model_outputs'].append(state['current_agent_msg'])
        if len(content) > 0:
            await self._raise_text_delta(context, content)


    async def _handle_tool_use_block(self, event, state, context):
        """Handle tool use block event."""
        tool_call = event.content_block.model_dump()
        if event.content_block.name == "think":
            state['think_tool_state'] = ThinkToolState.WAITING
        else:
            state['think_tool_state'] = ThinkToolState.INACTIVE

        state['collected_tool_calls'].append(tool_call)
        await self._raise_tool_call_delta(context, state['collected_tool_calls'] + state['server_tool_calls'])



    async def _handle_server_tool_use_block(self, event, state, context):
        """Handle tool use block event."""
        tool_call = event.content_block.model_dump()
        state['server_tool_calls'].append(tool_call)
        await self._raise_tool_call_delta(context, state['collected_tool_calls'] + state['server_tool_calls'])

    async def _handle_thinking_block(self, event, state, context):
        """Handle thinking block event."""
        state['current_thought'] = copy.deepcopy(event.content_block.model_dump())
        state['model_outputs'].append(state['current_thought'])

        if state['current_block_type'] == "redacted_thinking":
            content = "*redacted*"
        else:
            content = state['current_thought']['thinking']

        await self._raise_thought_delta(context, content)


    def _handle_input_json(self, event, state):
        """Handle input_json event."""
        if state['collected_tool_calls']:
            state['collected_tool_calls'][-1]['input'] = event.snapshot


    async def _handle_text_event(self, event, state, context: InteractionContext):
        """Handle text event."""
        if state['current_block_type'] == "text":
            state['current_agent_msg']['text'] = state['current_agent_msg']['text'] + event.text
            await self._raise_text_delta(context, event.text)
        elif state['current_block_type'] in ["thinking", "redacted_thinking"]:
            if state['current_block_type'] == "redacted_thinking":
                state['current_thought']['data'] = state['current_thought']['data'] + event.data
            else:
                state['current_thought']['thinking'] = state['current_thought']['thinking'] + event.text
                await self._raise_thought_delta(context, event.text)


    def _handle_message_delta(self, event, state):
        """Handle message_delta event."""
        state['stop_reason'] = event.delta.stop_reason


    async def _finalize_tool_calls(self, state, context: InteractionContext, messages):
        """Finalize tool calls after receiving a complete message."""
        await self._raise_tool_call_start(context, state['collected_tool_calls'])
        tool_response_messages = await self.__tool_calls_to_messages(state, context)
        messages.extend(tool_response_messages)

        await self._raise_tool_call_end(context, state['collected_tool_calls'], messages[-1]['content'])


    async def _generate_multi_modal_user_message(self, context: InteractionContext) -> Union[List[dict[str, Any]], None]:
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
        for image in context.inputs.images:
            if image.content is None and image.url is not None:
                self.logger.warning(
                    f"ImageInput has no content and Claude doesn't support image URLs. Skipping image {image.url}")
                continue

            img_source = {"type": "base64", "media_type": image.content_type, "data": image.content}
            contents.append({"type": "image", "source": img_source})

        # Process file content
        file_content_blocks = []
        if context.inputs.files:
            self.logger.info(f"Processing {len( context.inputs.files)} file inputs in Claude _generate_multi_modal_user_message")

            for idx, file in enumerate( context.inputs.files):
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
        main_text =  context.inputs.text

        # If we have file content blocks, add them before the user message
        if file_content_blocks:
            all_file_content = "\n\n".join(file_content_blocks)
            main_text = f"{all_file_content}\n\n{main_text}"

        # Add the combined text as the final content block
        contents.append({"type": "text", "text": main_text})

        return [{"role": "user", "content": contents}]

    async def __tool_calls_to_messages(self, state, context: InteractionContext) -> List[dict[str, Any]]:
        # Use the new centralized tool call handling in ToolChest
        tools_calls = await context.tool_chest.call_tools(state['collected_tool_calls'], context, format_type="claude")

        return tools_calls


class ClaudeBedrockChatAgent(ClaudeChatAgent):
    def __init__(self, **kwargs) -> None:
        kwargs['allow_betas'] = False
        super().__init__(**kwargs)

    @classmethod
    def client(cls, **opts):
        return AsyncAnthropicBedrock(**opts)

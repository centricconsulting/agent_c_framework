import asyncio
import copy
import json
import logging
import threading
from enum import Enum, auto

from typing import Any, List, Union, Dict, Tuple
from anthropic import AsyncAnthropic, APITimeoutError, Anthropic, RateLimitError
from anthropic.types import OverloadedError

from agent_c.agents.base import BaseAgent
from agent_c.chat.session_manager import ChatSessionManager
from agent_c.models.input import FileInput
from agent_c.models.input.audio_input import AudioInput
from agent_c.models.input.image_input import ImageInput
from agent_c.util.logging_utils import LoggingManager
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
        self.client: AsyncAnthropic = kwargs.get("client", AsyncAnthropic())
        self.supports_multimodal = True
        self.can_use_tools = True

        # Initialize logger
        logging_manager = LoggingManager(__name__)
        self.logger = logging_manager.get_logger()


        # JO: I need these as class level variables to adjust outside a chat call.
        self.max_tokens = kwargs.get("max_tokens", self.CLAUDE_MAX_TOKENS)
        self.budget_tokens = kwargs.get("budget_tokens", 0)


    async def __interaction_setup(self, **kwargs) -> dict[str, Any]:
        model_name: str = kwargs.get("model_name", self.model_name)
        if model_name is None:
            raise ValueError('Claude agent is missing a model_name')
        sys_prompt: str = await self._render_system_prompt(**kwargs)
        temperature: float = kwargs.get("temperature", self.temperature)
        max_tokens: int = kwargs.get("max_tokens", self.max_tokens)

        messages = await self._construct_message_array(**kwargs)
        callback_opts = self._callback_opts(**kwargs)

        tool_chest = kwargs.get("tool_chest", self.tool_chest)

        functions: List[Dict[str, Any]] = tool_chest.active_claude_schemas

        completion_opts = {"model": model_name, "messages": messages,
                           "system": sys_prompt,  "max_tokens": max_tokens,
                           'temperature': temperature}

        if '3-7-sonnet' in model_name:
            completion_opts['betas'] = ["token-efficient-tools-2025-02-19", "output-128k-2025-02-19"]

        budget_tokens: int = kwargs.get("budget_tokens", self.budget_tokens)
        if budget_tokens > 0:
            completion_opts['thinking'] = {"budget_tokens": budget_tokens, "type": "enabled"}
            completion_opts['temperature'] = 1


        if len(functions):
            completion_opts['tools'] = functions

        session_manager: Union[ChatSessionManager, None] = kwargs.get("session_manager", None)
        if session_manager is not None:
            completion_opts["metadata"] = {'user_id': session_manager.user.user_id}

        opts = {"callback_opts": callback_opts, "completion_opts": completion_opts, 'tool_chest': tool_chest}
        return opts


    @staticmethod
    def process_escapes(text):
        return text.replace("\\n", "\n").replace('\\"', '"').replace("\\\\", "\\")


    async def chat(self, **kwargs) -> List[dict[str, Any]]:
        """Main method for interacting with Claude API. Split into smaller helper methods for clarity."""
        opts = await self.__interaction_setup(**kwargs)
        client_wants_cancel: threading.Event = kwargs.get("client_wants_cancel", threading.Event())
        callback_opts = opts["callback_opts"]
        tool_chest = opts['tool_chest']
        session_manager: Union[ChatSessionManager, None] = kwargs.get("session_manager", None)
        messages = opts["completion_opts"]["messages"]

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
                        client_wants_cancel
                    )
                    if state['complete'] and state['stop_reason'] != 'tool_use':
                        return result

                    messages = result
                except (APITimeoutError, RateLimitError) as e:
                    # Exponential backoff handled in a helper method
                    delay = await self._handle_retryable_error(e, delay, callback_opts)
                except Exception as e:
                    if "overloaded" in str(e).lower():
                        delay = await self._handle_retryable_error(e, delay, callback_opts)
                    else:
                        await self._raise_system_event(f"Exception calling `client.messages.stream`.\n\n{e}\n", **callback_opts)
                        await self._raise_completion_end(opts["completion_opts"], stop_reason="exception", **callback_opts)
                        return []

        return messages


    async def _handle_retryable_error(self, error, delay, callback_opts):
        """Handle retryable errors with exponential backoff."""
        error_type = type(error).__name__
        await self._raise_system_event(
            f"Warning: The Claude streaming API may be under heavy load or you have hit your rate Limit.\n\nDelaying for {delay} seconds.\n",
            **callback_opts
        )
        await self._exponential_backoff(delay)
        return delay * 2  # Return the new delay for the next attempt


    async def _handle_claude_stream(self, completion_opts, tool_chest, session_manager,
                                    messages, callback_opts, interaction_id,
                                    client_wants_cancel: threading.Event) -> Tuple[List[dict[str, Any]], dict[str, Any]]:
        """Handle the Claude API streaming response."""
        await self._raise_completion_start(completion_opts, **callback_opts)

        # Initialize state trackers
        state = self._init_stream_state()
        state['interaction_id'] = interaction_id

        async with self.client.beta.messages.stream(**completion_opts) as stream:
            async for event in stream:
                await self._process_stream_event(event, state, tool_chest, session_manager,
                                               messages, callback_opts)

                if client_wants_cancel.is_set():
                    state['complete'] = True
                    state['stop_reason'] = "client_cancel"

                # If we've reached the end of a non-tool response, return
                if state['complete'] and state['stop_reason'] != 'tool_use':
                    await self._raise_history_event(messages, **callback_opts)
                    await self._raise_interaction_end(id=state['interaction_id'], **callback_opts)
                    return messages, state

                # If we've reached the end of a tool call response, continue after processing tool calls
                elif state['complete'] and state['stop_reason'] == 'tool_use':
                    await self._finalize_tool_calls(state, tool_chest, session_manager,
                                                  messages, callback_opts)
                    await self._raise_history_event(messages, **callback_opts)
                    return  messages, state

        return messages, state


    def _init_stream_state(self) -> Dict[str, Any]:
        """Initialize the state object for stream processing."""
        return {
            "collected_messages": [],
            "collected_tool_calls": [],
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

        # Check if we've hit the end of the JSON
        if processed.endswith('"}'):
            state['think_tool_state'] = ThinkToolState.INACTIVE
            # Remove closing quote and brace
            processed = processed[:-2]

        await self._raise_thought_delta(processed, **callback_opts)


    async def _handle_message_stop(self, event, state, tool_chest, session_manager,
                                  messages, callback_opts):
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
        assistant_content = self._format_model_outputs_to_text(state['model_outputs'])
        await self._save_interaction_to_session(session_manager, assistant_content)

        # Update messages
        messages.append({'role': 'assistant', 'content': state['model_outputs']})


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
        elif state['current_block_type'] in ["thinking", "redacted_thinking"]:
            await self._handle_thinking_block(event, state, callback_opts)
        else:
            await self._raise_system_event(
                f"content_block_start Unknown content type: {state['current_block_type']}",
                **callback_opts
            )


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
        await self._raise_tool_call_delta(state['collected_tool_calls'], **callback_opts)


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


    async def _finalize_tool_calls(self, state, tool_chest, session_manager, messages, callback_opts):
        """Finalize tool calls after receiving a complete message."""
        await self._raise_tool_call_start(state['collected_tool_calls'], vendor="anthropic", **callback_opts)

        # Process tool calls and get response messages
        tool_response_messages = await self.__tool_calls_to_messages(
            state['collected_tool_calls'],
            tool_chest
        )

        # Add tool response messages to the conversation history
        messages.extend(tool_response_messages)

        await self._raise_tool_call_end(
            state['collected_tool_calls'],
            messages[-1]['content'],
            vendor="anthropic",
            **callback_opts
        )


    def _generate_multi_modal_user_message(self, user_input: str, images: List[ImageInput], audio: List[AudioInput],
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
                logging.warning(
                    f"ImageInput has no content and Claude doesn't support image URLs. Skipping image {image.url}")
                continue

            img_source = {"type": "base64", "media_type": image.content_type, "data": image.content}
            contents.append({"type": "image", "source": img_source})

        # Process file content
        file_content_blocks = []
        if files:
            logging.info(f"Processing {len(files)} file inputs in Claude _generate_multi_modal_user_message")

            for idx, file in enumerate(files):
                # Always try to use get_text_content() to get extracted text
                extracted_text = None

                # Check if get_text_content method exists and call it
                if hasattr(file, 'get_text_content') and callable(file.get_text_content):
                    extracted_text = file.get_text_content()
                    logging.info(
                        f"Claude: File {idx} ({file.file_name}): get_text_content() returned {len(extracted_text) if extracted_text else 0} chars")
                else:
                    logging.warning(f"Claude: File {idx} ({file.file_name}): get_text_content() method not available")

                if extracted_text:
                    file_name = file.file_name or "unknown file"
                    content_block = f"Content from file {file_name}:\n\n{extracted_text}"

                    file_content_blocks.append(content_block)
                    logging.info(f"Claude: File {idx} ({file.file_name}): Added extracted text to message")
                else:
                    # Fall back to mentioning the file without content
                    file_name = file.file_name or "unknown file"
                    file_content_blocks.append(f"[File attached: {file_name} (content could not be extracted)]")
                    logging.warning(
                        f"Claude: File {idx} ({file.file_name}): No text content available, adding file name only")

        # Prepare the main text content with file content
        main_text = user_input or ""

        # If we have file content blocks, add them before the user message
        if file_content_blocks:
            all_file_content = "\n\n".join(file_content_blocks)
            main_text = f"{all_file_content}\n\n{main_text}"

        # Add PI mitigation if needed
        if self.mitigate_image_prompt_injection and images:
            main_text = f"{main_text}{BaseAgent.IMAGE_PI_MITIGATION}"

        # Add the combined text as the final content block
        contents.append({"type": "text", "text": main_text})

        # For audio clips, since Claude doesn't support audio directly, just log a warning
        if audio and len(audio) > 0:
            logging.warning(
                f"Claude does not directly support audio input. Mentioned {len(audio)} audio clips in text.")

        return [{"role": "user", "content": contents}]

    async def __tool_calls_to_messages(self, tool_calls, tool_chest):
        # Use the new centralized tool call handling in ToolChest
        return await tool_chest.call_tools(tool_calls, format_type="claude")

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
        await self._save_message_to_session(session_manager, prefixed_content, 'assistant')

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
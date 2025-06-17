import copy
import base64
import os

import httpcore

from enum import Enum, auto
from typing import Any, List, Union, Dict, Tuple, Optional
from anthropic import AsyncAnthropic, APITimeoutError, Anthropic, RateLimitError, AsyncAnthropicBedrock
from anthropic.types import MessageParam

from agent_c.agent_runtimes.base import AgentRuntime
from agent_c.util.logging_utils import LoggingManager
from agent_c.util.token_counter import TokenCounter
from agent_c.agent_runtimes.runtime_registry import RuntimeRegistry
from agent_c.models.completion.claude import ClaudeCompletionParams
from agent_c.models.context.interaction_context import InteractionContext

logger = LoggingManager(__name__).get_logger()

class ThinkToolState(Enum):
    """Enum representing the state of the think tool processing."""
    INACTIVE = auto()   # Not currently processing a think tool
    WAITING = auto()    # Waiting for the JSON to start
    EMITTING = auto()   # Processing and emitting think tool content

class ClaudeTokenCounter(TokenCounter):
    def __init__(self):
        self.anthropic: Anthropic = Anthropic()

    def count_tokens(self, text: str) -> int:
        response = self.anthropic.messages.count_tokens(
            model="claude-3-7-sonnet-latest",
            system="",
            messages=[MessageParam(role="user", content=text)]
        )

        return response.input_tokens

class ClaudeChatAgentRuntime(AgentRuntime):
    CLAUDE_MAX_TOKENS: int = 64000

    def __init__(self, max_retry_delay_secs: int = 300, concurrency_limit: int = 3,
                 context = None, client: Optional[Union[AsyncAnthropic, AsyncAnthropicBedrock]] = None) -> None:
        """
        Initialize ChatAgent object.

        Non-Base Parameters:
        client: Optional[Union[AsyncAnthropic, AsyncAnthropicBedrock]]
            The client to use for making requests to the Anthropic API.
        """
        super().__init__(ClaudeTokenCounter(), max_retry_delay_secs=max_retry_delay_secs, concurrency_limit=concurrency_limit)
        self.client: Union[AsyncAnthropic, AsyncAnthropicBedrock] = client or self.__class__.client(context)

    @classmethod
    def client(cls, context = None):
        if context and hasattr(context, 'chat_session'):
            auth = context.chat_session.agent_config.agent_params.auth
            api_key = auth.api_key if hasattr(auth, 'api_key') else os.environ.get("ANTHROPIC_API_KEY")
            auth_token = auth.token if hasattr(auth, 'token') else os.environ.get('ANTHROPIC_AUTH_TOKEN')
        else:
            api_key = os.environ.get("ANTHROPIC_API_KEY")
            auth_token = os.environ.get("ANTHROPIC_AUTH_TOKEN")

        if not api_key and not auth_token:
            logger.warning("Waring an attempt was made to create an Anthropic API client without an API key or auth token.  Set ANTHROPIC_API_KEY or ANTHROPIC_AUTH_TOKEN to use this runtime ")
            return None

        if auth_token:
            return AsyncAnthropic(auth_token=auth_token)
        else:
            return AsyncAnthropic(api_key=api_key)

    @classmethod
    def can_create(cls, context = None) -> bool:
        server_key = os.environ.get("ANTHROPIC_API_KEY")
        if context and hasattr(context, 'chat_session'):
            context_key = context.chat_session.agent_config.agent_params.auth.api_key if context else None
        else:
            context_key = None

        if not server_key and not context_key:
            return False

        return True


    @property
    def tool_format(self) -> str:
        return "claude"

    @staticmethod
    def process_escapes(text: str):
        return text.replace("\\n", "\n").replace('\\"', '"').replace("\\\\", "\\")

    def _correct_max_tokens_in_context(self, context: InteractionContext):
        agent_params: ClaudeCompletionParams = context.chat_session.agent_config.agent_params

        if self.__is_claude_4(agent_params):
            if agent_params == self.CLAUDE_MAX_TOKENS:
                if self.__is_sonnet(agent_params):
                    agent_params.max_tokens = 64000
                else:
                    agent_params.max_tokens = 32000
        elif agent_params.allow_betas and agent_params.max_tokens == self.CLAUDE_MAX_TOKENS:
            agent_params.max_tokens = 128000

    @staticmethod
    def __is_claude_4(agent_params: ClaudeCompletionParams) -> bool:
        """
        Check if the agent parameters indicate a Claude 4 model.
        """
        return '-4-' in agent_params.model_id

    @staticmethod
    def __is_claude_3_7(agent_params: ClaudeCompletionParams) -> bool:
        """
        Check if the agent parameters indicate a Claude 3 model.
        """
        return '-3-7' in agent_params.model_id

    @staticmethod
    def __is_sonnet(agent_params: ClaudeCompletionParams) -> bool:
        """
        Check if the agent parameters indicate a Sonnet model.
        """
        return 'sonnet' in agent_params.model_id

    @staticmethod
    def __is_opus(agent_params: ClaudeCompletionParams) -> bool:
        """
        Check if the agent parameters indicate an Opus model.
        """
        return 'opus' in agent_params.model_id

    async def __build_completion_options(self, context: InteractionContext) -> Dict[str, Any]:
        # Make sure we have the correct max tokens set in the context,
        # the UI might have set it to and old default value.
        self._correct_max_tokens_in_context(context)

        agent_params: ClaudeCompletionParams = context.chat_session.agent_config.agent_params
        allow_server_tools: bool = agent_params.allow_server_tools
        allow_betas: bool = agent_params.allow_betas

        try:
            completion_opts = agent_params.as_completion_params()
        except Exception as e:
            self.logger.exception(f"Error building completion options: {e}", exc_info=True)
            raise e

        if allow_server_tools:
            if agent_params.max_searches > 0:
                context.external_tool_schemas.append({"type": "web_search_20250305", "name": "web_search", "max_uses": agent_params.max_searches})

        if allow_betas:
            if allow_server_tools:
                context.external_tool_schemas.append({"type": "code_execution_20250522","name": "code_execution"})

            if self.__is_claude_4(agent_params):
                completion_opts['betas'] = ['interleaved-thinking-2025-05-14', "files-api-2025-04-14"] # , "code-execution-2025-05-22"]
            else:
                completion_opts['betas'] = ["token-efficient-tools-2025-02-19", "output-128k-2025-02-19", "files-api-2025-04-14"] # , "code-execution-2025-05-22"]

        if agent_params.budget_tokens > 0:
            completion_opts['thinking'] = {"budget_tokens": agent_params.budget_tokens, "type": "enabled"}

        completion_opts["metadata"] = {'user_id': context.chat_session.user_id}

        return completion_opts

    async def __add_system_prompt_and_tools(self, context: InteractionContext,  completion_opts: Dict[str, Any]) -> Dict[str, Any]:
        """
        Add system prompt and tools to the completion options.
        """
        tool_schemas = context.tool_schemas

        completion_opts["system"] = await self.prompt_builder.render(context)

        await self._raise_system_prompt(context, completion_opts["system"])

        if tool_schemas:
            completion_opts["tools"] = tool_schemas

        return completion_opts

    async def chat(self, context: InteractionContext) -> List[Dict[str, Any]]:
        # This first time through, we will build the completion options.
        completion_opts = await self.__build_completion_options(context)
        completion_opts = await self.__add_system_prompt_and_tools(context, completion_opts)

        messages = await self._construct_message_array(context)
        completion_opts["messages"] = messages

        await self._raise_user_request(context)

        delay = 1  # Initial delay between retries
        async with (self.semaphore):
            await self._raise_interaction_start(context)

            while delay <= self.max_delay:
                try:
                    # Stream handling encapsulated in a helper method
                    result, state = await self._handle_claude_stream(
                        context,
                        completion_opts,
                        messages
                    )

                    if state['complete'] and state['stop_reason'] != 'tool_use':
                        self.logger.info(f"Interaction {context.interaction_id} stopped with reason: {state['stop_reason']}")
                        return result

                    completion_opts = await self.__add_system_prompt_and_tools(context, completion_opts)

                    delay = 1
                    messages = result
                except RateLimitError:
                    delay = await self._handle_retryable_error(delay, context, "Claude API is overloaded (rate limit)", "rate_limit", completion_opts)
                except APITimeoutError:
                    delay = await self._handle_retryable_error(delay, context, "Claude API is overloaded (API timeout)", "timeout", completion_opts)
                except httpcore.RemoteProtocolError:
                    delay = await self._handle_retryable_error(delay, context, "Claude API is overloaded (remote protocol error)", "overload", completion_opts)
                except Exception as e:
                    if "overloaded" in str(e).lower():
                        delay = await self._handle_retryable_error(delay, context, "Claude API is overloaded", "overload", completion_opts)
                    else:
                        self.logger.exception(f"Uncoverable error during Claude chat: {e}", exc_info=True)
                        await self._raise_system_event(context, f"Uncoverable error during Claude chat: {e}")
                        await self._raise_completion_end(context, completion_opts, stop_reason="exception")
                        await self._raise_interaction_end(context)
                        return messages

        self.logger.warning("ABNORMAL TERMINATION OF CLAUDE CHAT")
        await self._raise_system_event(context,f"ABNORMAL TERMINATION OF CLAUDE CHAT")
        await self._raise_completion_end(context, completion_opts, stop_reason="overload")
        await self._raise_interaction_end(context)
        return messages


    async def _handle_retryable_error(self, delay: int, context: InteractionContext, message: str,
                                      stop_reason: str, completion_opts: Dict[str, Any]) -> int:
        """Handle retryable errors with exponential backoff."""
        user_message = f"{message}, retrying... Delay is {delay} seconds."
        self.logger.warning(f"{user_message} Session {context.chat_session.session_id}, User {context.chat_session.user_id}")
        await self._raise_system_event(context, user_message, severity="warning")
        await self._raise_completion_end(context, completion_opts, stop_reason=stop_reason)
        await self._exponential_backoff(delay)
        return delay * 2  # Return the new delay for the next attempt


    async def _handle_claude_stream(self, context: InteractionContext, completion_opts,
                                    messages: List[Dict[str, Any]]) -> Tuple[List[Dict[str, Any]], Dict[str, Any]]:
        """Handle the Claude API streaming response."""
        await self._raise_completion_start(context, completion_opts)

        # Initialize state trackers
        state = self._init_stream_state()
        state['interaction_id'] = context.interaction_id
        state['completion_options'] = completion_opts

        if "betas" in  completion_opts:
            stream_source = self.client.beta
        else:
            stream_source = self.client


        async with stream_source.messages.stream(**completion_opts) as stream:
            async for event in stream:
                await self._process_stream_event(context, event, state, messages)

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


    @staticmethod
    def _init_stream_state() -> Dict[str, Any]:
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
        elif event_type == "input_json":
            self._handle_input_json(event, state)
        elif event_type == "text":
            await self._handle_text_event(event, state, context)
        elif event_type == "message_delta":
            self._handle_message_delta(event, state)


    @staticmethod
    async def _handle_message_start(event, state):
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

        # Check if we've hit the end of the JSON
        if processed.endswith('"}'):
            state['think_tool_state'] = ThinkToolState.INACTIVE
            # Remove closing quote and brace
            processed = processed[:-2]

        await self._raise_thought_delta(context, processed)

    async def _handle_message_stop(self, event, state, messages, context):
        """Handle the message_stop event."""
        state['output_tokens'] = event.message.usage.output_tokens
        state['complete'] = True

        # Completion end event
        await self._raise_completion_end(context, state['completion_options'], state['stop_reason'], state['input_tokens'], state['output_tokens'])

        # Update messages
        msg = {'role': 'assistant', 'content': state['model_outputs']}
        messages.append(msg)
        await self._raise_history_delta(context, [msg])

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


    @staticmethod
    def _handle_input_json(event, state):
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


    @staticmethod
    def _handle_message_delta(event, state):
        """Handle message_delta event."""
        state['stop_reason'] = event.delta.stop_reason


    async def _finalize_tool_calls(self, state, context: InteractionContext, messages):
        """Finalize tool calls after receiving a complete message."""
        await self._raise_tool_call_start(context, state['collected_tool_calls'])

        tool_response_messages = await context.tool_chest.call_tools(state['collected_tool_calls'], context, format_type="claude")
        messages.extend(tool_response_messages)

        await self._raise_tool_call_end(context, state['collected_tool_calls'], messages[-1]['content'])


    async def _generate_multi_modal_user_message(self, context: InteractionContext) -> Optional[List[dict[str, Any]]]:
        """
        Generates a multimodal message containing text, images, and file content.

        This method formats various input types into a structure that can be sent to
        the Claude API, adhering to the Anthropic message format.

        Args:
            context (InteractionContext): The interaction context containing user inputs.

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
                if context.chat_session.agent_config.agent_params.allow_betas:
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


class ClaudeBedrockChatAgent(ClaudeChatAgentRuntime):
    @classmethod
    def client(cls, **opts):
        return AsyncAnthropicBedrock(**opts)

    @classmethod
    def vendor(cls) -> str:
        return "bedrock"

    @classmethod
    def can_create(cls, context: Optional[InteractionContext]) -> bool:
        server_key = os.environ.get("ANTHROPIC_API_KEY")
        context_key = context.chat_session.agent_config.agent_params.auth.api_key if context else None

        if not server_key and not context_key:
            return False

        return True


# Register the chat agents with the runtime registry
RuntimeRegistry.register(ClaudeChatAgentRuntime, "claude")
RuntimeRegistry.register(ClaudeBedrockChatAgent, "bedrock")

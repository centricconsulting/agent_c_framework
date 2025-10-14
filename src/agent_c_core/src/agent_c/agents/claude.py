import asyncio
import copy
import json
import time

import httpcore
import yaml
import base64
import threading
from enum import Enum, auto
from typing import Any, List, Union, Dict, Tuple


from anthropic import AsyncAnthropic, APITimeoutError, Anthropic, RateLimitError, AsyncAnthropicBedrock


from agent_c.agents.base import BaseAgent
from agent_c.chat.session_manager import ChatSessionManager
from agent_c.models.events.chat import AnthropicUserMessageEvent
from agent_c.models.input import FileInput
from agent_c.models.input.audio_input import AudioInput
from agent_c.models.input.image_input import ImageInput
from agent_c.prompting import PromptBuilder
from agent_c.util.logging_utils import LoggingManager
from agent_c.util.token_counter import TokenCounter

SERVER_TOOL_RESULT_TYPES = ['web_search_tool_result', 'code_execution_tool_result', 'mcp_tool_result', 'web_fetch_tool_result']


class ThinkToolState(Enum):
    """Enum representing the state of the think tool processing."""
    INACTIVE = auto()   # Not currently processing a think tool
    WAITING = auto()    # Waiting for the JSON to start
    EMITTING = auto()   # Processing and emitting think tool content


class ClaudeChatAgent(BaseAgent):
    """
    Claude Chat Agent with optimized lazy JSON processing.
    
    Key optimization: Tool arguments are kept as accumulated strings during streaming
    and only parsed to JSON at tool execution time. This reduces memory overhead
    during streaming while maintaining backward compatibility.
    
    CRITICAL: The "think" tool has special handling that streams arguments directly
    to the client as thought deltas. This functionality is preserved exactly.
    """
    CLAUDE_MAX_TOKENS: int = 64000
    class ClaudeTokenCounter(TokenCounter):

        def __init__(self):
            self.anthropic: Anthropic = Anthropic()

        def count_tokens(self, text: str) -> int:
            try:
                response = self.anthropic.messages.count_tokens(
                    model="claude-sonnet-4-20250514",
                    system="",
                    messages=[{
                        "role": "user",
                        "content": text
                    }],
                )
                return response.input_tokens
            except Exception as e:
                LoggingManager(__name__).get_logger().exception("Failed to count tokens")

            return 0


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
        super().__init__(**kwargs, vendor="anthropic")
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

    def _context_opts(self) -> dict[str, Any]:
        opts = {
                "edits": [
                    {
                        "type": "clear_tool_uses_20250919",
                        "trigger": {
                            "type": "input_tokens",
                            "value": 80000
                        },
                        "keep": {
                            "type": "tool_uses",
                            "value": 4
                        },
                        "clear_at_least": {
                            "type": "input_tokens",
                            "value": 5000
                        },
                        "exclude_tools": ["think", "wsp_get_task", "wsp_update_task", "wsp_add_lesson_learned", 'act_chat', 'ateam_chat']
                    }
                ]
            }
        return opts

    async def __interaction_setup(self, **kwargs) -> dict[str, Any]:
        model_name: str = kwargs.get("model_name", self.model_name)
        if model_name is None:
            raise ValueError('Claude agent is missing a model_name')

        callback_opts = self._callback_opts(**kwargs)
        messages = await self._construct_message_array(**kwargs)
        await self._raise_user_message(messages[-1], **callback_opts)

        temperature: float = kwargs.get("temperature", self.temperature)
        max_tokens: int = self.CLAUDE_MAX_TOKENS
        allow_server_tools: bool = kwargs.get("allow_server_tools", False)


        tool_chest = kwargs.get("tool_chest", self.tool_chest)
        functions: List[Dict[str, Any]] = kwargs['schemas']

        kwargs['prompt_metadata']['model_id'] = model_name
        (tool_context, prompt_context) = await self._render_contexts(**kwargs)
        sys_prompt: str = prompt_context["system_prompt"]
        allow_betas: bool = kwargs.get("allow_betas", self.allow_betas)
        completion_opts: Dict[str, Any] = {"model": model_name.removeprefix("bedrock_"), "messages": messages,
                                           "system": sys_prompt,  "max_tokens": max_tokens,
                                           'temperature': temperature}

        if allow_server_tools:

            max_searches: int = kwargs.get("max_searches", 0)
            if max_searches > 0:
                functions.append({"type": "web_search_20250305", "name": "web_search", "max_uses": max_searches})



        if allow_betas:
            tool_betas = ["code-execution-2025-08-25", "computer-use-2025-01-24",
                          "web-fetch-2025-09-10", "mcp-client-2025-04-04"]

            if "sonnet" in model_name:
                if '-4' in model_name:
                    completion_opts['betas'] = ["context-1m-2025-08-07", "context-management-2025-06-27",  "files-api-2025-04-14", 'interleaved-thinking-2025-05-14']
                else:
                    completion_opts['betas'] = ["token-efficient-tools-2025-02-19", "output-128k-2025-02-19", "files-api-2025-04-14"]
                    max_tokens = 128000
            elif '-4' in model_name:
                completion_opts['betas'] =["token-efficient-tools-2025-02-19", "files-api-2025-04-14", "context-management-2025-06-27"]

            if "context-management-2025-06-27" in completion_opts.get('betas', []):
                completion_opts['context_management'] = self._context_opts()

            completion_opts['betas'].extend(tool_betas)

        budget_tokens: int = kwargs.get("budget_tokens", self.budget_tokens)
        if budget_tokens > 0:
            completion_opts['thinking'] = {"budget_tokens": budget_tokens, "type": "enabled"}
            completion_opts['temperature'] = 1


        if len(functions):
            completion_opts['tools'] = functions

        completion_opts['max_tokens'] = max_tokens
        completion_opts["metadata"] = {'user_id': kwargs.get('user_id', 'admin')}

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
        interaction_id = await self._raise_interaction_start(**callback_opts)
        await self._raise_system_prompt(opts["completion_opts"]["system"], **callback_opts)
        chat_session = kwargs.get("chat_session")

        delay = 3  # Initial delay between retries
        async with (self.semaphore):
            while delay <= self.max_delay:
                # Check for cancellation before starting new completion
                if client_wants_cancel and client_wants_cancel.is_set():
                    self.logger.info(f"Client requested cancellation before completion start for interaction {interaction_id}")
                    await self._raise_completion_end(opts["completion_opts"], stop_reason="client_cancel", **callback_opts)
                    await self._raise_interaction_end(id=interaction_id, **callback_opts)
                    return messages
                    
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
                        
                    # Check for cancellation after tool processing
                    if state['complete'] and state['stop_reason'] == 'client_cancel':
                        self.logger.info(f"Interaction {interaction_id} cancelled during tool processing")
                        # Fire necessary events for proper cleanup
                        await self._raise_history_event(result, **callback_opts)
                        await self._raise_interaction_end(id=interaction_id, **callback_opts)
                        return result

                    new_system_prompt  = await prompt_builder.render(opts['tool_context'], tool_sections=kwargs.get("tool_sections", None))
                    if new_system_prompt != opts["completion_opts"]["system"]:
                        self.logger.debug(f"Updating system prompt for interaction {interaction_id}")
                        opts["completion_opts"]["system"] = new_system_prompt
                        await self._raise_system_prompt(new_system_prompt, **callback_opts)

                    # Get agent config from chat_session or tool_context (for act one-shot scenarios that don't provide it) - old code commented out.
                    # tool_params = tool_chest.get_inference_data(chat_session.agent_config.tools, self.tool_format)
                    # opts["completion_opts"]['tools'] = chat_session.agent_config.filter_allowed_tools(tool_params['schemas'])
                    agent_config = chat_session.agent_config if chat_session else opts['tool_context'].get('active_agent') or opts['tool_context'].get('agent_config')
                    if agent_config:
                        tool_params = tool_chest.get_inference_data(agent_config.tools, self.tool_format)
                        opts["completion_opts"]['tools'] = agent_config.filter_allowed_tools(tool_params['schemas'])


                    delay = 3
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

    async def _raise_user_message(self, message, **opts):
        streaming_callback = opts.pop('streaming_callback', None)

        try:
            event = AnthropicUserMessageEvent(message=message, **opts)
        except Exception as e:
            self.logger.exception(f"Error creating AnthropicUserMessageEvent: {e}", exc_info=True)
            return

        await self._raise_user_message_event(event, streaming_callback=streaming_callback)



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
                    self.logger.info("Client requested cancellation.")
                    state['complete'] = True
                    state['stop_reason'] = "client_cancel"

                # If we've reached the end of a non-tool response, return
                if state['complete'] and state['stop_reason'] != 'tool_use':
                    if state['stop_reason'] == 'refusal':
                        self.logger.warning("Call resulted in refusal, ending interaction.")
                    await self._raise_history_event(messages, **callback_opts)
                    await self._raise_interaction_end(id=state['interaction_id'], **callback_opts)
                    return messages, state

                # If we've reached the end of a tool call response, continue after processing tool calls
                elif state['complete'] and state['stop_reason'] == 'tool_use':
                    if state['collected_tool_calls']:
                        await self._finalize_tool_calls(state, tool_chest, session_manager,
                                                        messages, callback_opts, tool_context, client_wants_cancel)
                    
                    # Check if finalize_tool_calls set cancellation state
                    if state['stop_reason'] == 'client_cancel':
                        # Tool call processing was cancelled, fire events and return
                        await self._raise_history_event(messages, **callback_opts)
                        await self._raise_interaction_end(id=state['interaction_id'], **callback_opts)
                        return messages, state

                    await self._raise_history_event(messages, **callback_opts)
                    return  messages, state

                await asyncio.sleep(0)

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
            "tool_json_buffers": [],  # JSON buffers for each tool call (lazy processing)
            "stop_reason": None,
            "complete": False,
            "interaction_id": None,
            # Performance optimization: cache concatenated tool calls
            "cached_all_tool_calls": [],
            "tool_calls_cache_dirty": False,
            # Text delta batching for performance (NOT for think tool - preserve streaming)
            "text_delta_buffer": "",
            "text_delta_batch_size": 50,  # Batch size for text deltas
            "last_text_delta_time": 0
        }


    async def _process_stream_event(self, event, state, tool_chest, session_manager,
                                   messages, callback_opts):
        """Process a single event from the Claude stream."""
        event_type = event.type
        await asyncio.sleep(0)
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
            await self._handle_input_json(event, state, callback_opts)
        elif event_type == "text":
            await self._handle_text_event(event, state, callback_opts)
        elif event_type == "message_delta":
            self._handle_message_delta(event, state)

        await asyncio.sleep(0)


    async def _handle_message_start(self, event, state, callback_opts):
        """Handle the message_start event."""
        state["input_tokens"] = event.message.usage.input_tokens

    async def _handle_server_tool_use_block(self, event, state, callback_opts):
        """Handle server tool use block event."""
        tool_call = event.content_block.model_dump()
        state['server_tool_calls'].append(tool_call)
        state['model_outputs'].append(tool_call)  # ADD THIS - part of assistant message
        state['tool_calls_cache_dirty'] = True

        await self._raise_tool_call_delta(state['server_tool_calls'], **callback_opts)
        await self._raise_tool_call_start([tool_call], vendor="anthropic", **callback_opts)

    async def _handle_content_block_delta(self, event, state, callback_opts):
        """Handle content_block_delta events."""
        delta = event.delta

        if delta.type == "signature_delta":
            state['current_thought']['signature'] = delta.signature
        elif delta.type == "thinking_delta":
            await self._handle_thinking_delta(delta, state, callback_opts)
        elif delta.type == "input_json_delta":
            # Check if this is a think tool (preserve existing functionality)
            if (state['collected_tool_calls'] and 
                state['collected_tool_calls'][-1].get('name') == 'think'):
                await self._handle_think_tool_json(delta, state, callback_opts)
            else:
                # Handle regular tool JSON delta accumulation
                await self._handle_input_json_delta(delta, state, callback_opts)


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


    async def _handle_input_json_delta(self, delta, state, callback_opts):
        """Handle input_json_delta for regular tool calls (not think tool)."""
        if state['collected_tool_calls']:
            tool_call_index = len(state['collected_tool_calls']) - 1
            # Ensure we have a buffer for this tool call
            while len(state['tool_json_buffers']) <= tool_call_index:
                state['tool_json_buffers'].append("")
            
            # Accumulate the JSON delta to the buffer
            state['tool_json_buffers'][tool_call_index] += delta.partial_json


    async def _handle_message_stop(self, event, state, tool_chest, session_manager, messages, callback_opts):
        """Handle the message_stop event."""
        state['output_tokens'] = event.message.usage.output_tokens
        state['complete'] = True

        # Flush any remaining text deltas before completing
        await self._flush_text_delta_buffer(state, callback_opts)

        # Fire ToolCallEvent with active=true for completed non-think tools
        # This happens BEFORE completion event to ensure proper UI state transitions
        if state['collected_tool_calls']:
            non_think_tool_calls = [tool_call for tool_call in state['collected_tool_calls'] 
                                  if tool_call.get('name', '') != 'think' 
                                  and tool_call.get('input') is not None]
            if non_think_tool_calls:
                await self._raise_tool_call_start(non_think_tool_calls, vendor="anthropic", **callback_opts)

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
        #
        # Update messages

        if len(state['model_outputs']) > 0:
            msg = {'role': 'assistant', 'content': state['model_outputs']}
            messages.append(msg)
            await self._raise_history_delta([msg], **callback_opts)
            if session_manager is not None:
                session_manager.active_memory.messages = messages

    async def _handle_content_block_end(self, event, state, callback_opts):
        if state['current_block_type'] == "thinking":
            await self._raise_complete_thought(state['current_thought']['thinking'], **callback_opts)
        elif state['current_block_type'] == "text":
            # Flush any remaining text deltas at block end
            await self._flush_text_delta_buffer(state, callback_opts)

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
        elif state['current_block_type'] in ["server_tool_use", "mcp_server_use"]:
            await self._handle_server_tool_use_block(event, state, callback_opts)
        elif state['current_block_type'] in ["thinking", "redacted_thinking"]:
            await self._handle_thinking_block(event, state, callback_opts)
        elif state['current_block_type'] in SERVER_TOOL_RESULT_TYPES:
            result = event.content_block.model_dump()
            tool_use_id = result.get('tool_use_id')
            matching_request = None
            if tool_use_id:
                for server_tool_call in state['server_tool_calls']:
                    if server_tool_call.get('id') == tool_use_id:
                        matching_request = server_tool_call
                        break

            state['server_tool_responses'].append(result)
            state['model_outputs'].append(result)

            if matching_request:
                await self._raise_tool_call_end([matching_request], [result], vendor="anthropic",  **callback_opts)
            else:
                self.logger.warning(f"Could not find matching server tool call for result with tool_use_id: {tool_use_id}")
                await self._raise_tool_call_end([], [result], **callback_opts)

        else:
            self.logger.warning(f"content_block_start Unknown content type: {state['current_block_type']}")


    async def _handle_text_block_start(self, event, state, callback_opts):
        """Handle text block start event."""
        content = event.content_block.text
        state['current_agent_msg'] = copy.deepcopy(event.content_block.model_dump())
        state['model_outputs'].append(state['current_agent_msg'])
        if len(content) > 0:
            # Use batching for regular text deltas for performance
            await self._batch_text_delta(content, state, callback_opts)


    async def _handle_tool_use_block(self, event, state, callback_opts):
        """Handle tool use block event."""
        tool_call = event.content_block.model_dump()
        if event.content_block.name == "think":
            state['think_tool_state'] = ThinkToolState.WAITING

        state['collected_tool_calls'].append(tool_call)
        # Initialize JSON buffer for this tool call (empty string for delta accumulation)
        state['tool_json_buffers'].append("")
        # Mark cache as dirty and update
        state['tool_calls_cache_dirty'] = True
        await self._raise_tool_call_delta(self._get_all_tool_calls(state), **callback_opts)


    async def _handle_thinking_block(self, event, state, callback_opts):
        """Handle thinking block event."""
        state['current_thought'] = copy.deepcopy(event.content_block.model_dump())
        state['model_outputs'].append(state['current_thought'])

        if state['current_block_type'] == "redacted_thinking":
            content = "*redacted*"
        else:
            content = state['current_thought']['thinking']

        await self._raise_thought_delta(content, **callback_opts)


    async def _handle_input_json(self, event, state, callback_opts):
        """Handle input_json event with lazy JSON processing.
        
        For think tools, maintain existing behavior (immediate parsing for streaming).
        For other tools, store raw JSON string for lazy parsing at execution time.
        """
        if state['collected_tool_calls']:
            tool_call = state['collected_tool_calls'][-1]
            tool_name = tool_call.get('name', '')
            
            # CRITICAL: Preserve think tool streaming behavior exactly
            if tool_name == 'think':
                # Think tool needs immediate parsing to support streaming
                tool_call_index = len(state['collected_tool_calls']) - 1
                if tool_call_index < len(state['tool_json_buffers']):
                    json_str = state['tool_json_buffers'][tool_call_index]
                    if json_str.strip():
                        try:
                            tool_call['input'] = json.loads(json_str)
                        except json.JSONDecodeError:
                            tool_call['input'] = event.snapshot
                    else:
                        tool_call['input'] = event.snapshot
                else:
                    tool_call['input'] = event.snapshot
            else:
                # For regular tools: store raw JSON string for lazy parsing
                tool_call_index = len(state['collected_tool_calls']) - 1
                if tool_call_index < len(state['tool_json_buffers']):
                    json_str = state['tool_json_buffers'][tool_call_index]
                    if json_str.strip():
                        # Store raw JSON string instead of parsing
                        tool_call['input'] = json_str
                    else:
                        # Fallback to snapshot as string if buffer is empty
                        tool_call['input'] = json.dumps(event.snapshot) if event.snapshot else '{}'
                else:
                    # Fallback to snapshot as string if buffer doesn't exist
                    tool_call['input'] = json.dumps(event.snapshot) if event.snapshot else '{}'


    async def _handle_text_event(self, event, state, callback_opts):
        """Handle text event."""
        if state['current_block_type'] == "text":
            state['current_agent_msg']['text'] = state['current_agent_msg']['text'] + event.text
            # Use batching for regular text deltas for performance
            await self._batch_text_delta(event.text, state, callback_opts)
        elif state['current_block_type'] in ["thinking", "redacted_thinking"]:
            if state['current_block_type'] == "redacted_thinking":
                state['current_thought']['data'] = state['current_thought']['data'] + event.data
            else:
                state['current_thought']['thinking'] = state['current_thought']['thinking'] + event.text
                # CRITICAL: Think tool deltas must stream immediately - do NOT batch
                await self._raise_thought_delta(event.text, **callback_opts)


    def _handle_message_delta(self, event, state):
        """Handle message_delta event."""
        state['stop_reason'] = event.delta.stop_reason

    def _get_all_tool_calls(self, state) -> List[dict[str, Any]]:
        """Get concatenated tool calls list, using cache for performance.
        
        This optimization avoids repeatedly concatenating the same lists on every
        tool call delta event. The cache is invalidated whenever tool calls are modified.
        """
        if state['tool_calls_cache_dirty'] or not state['cached_all_tool_calls']:
            # Recalculate cached list only when needed
            state['cached_all_tool_calls'] = state['collected_tool_calls'] + state['server_tool_calls']
            state['tool_calls_cache_dirty'] = False
        return state['cached_all_tool_calls']

    async def _batch_text_delta(self, text: str, state, callback_opts, force_flush: bool = False):
        """Batch text deltas for performance, but preserve immediate streaming for think tool.
        
        CRITICAL: This is only used for regular text content. Think tool deltas are 
        streamed immediately via _raise_thought_delta() to preserve exact streaming behavior.
        """
        current_time = time.time()
        state['text_delta_buffer'] += text
        
        # Force immediate flush if requested, or if buffer is large enough, or enough time has passed
        should_flush = (
            force_flush or 
            len(state['text_delta_buffer']) >= state['text_delta_batch_size'] or
            (current_time - state['last_text_delta_time']) > 0.05  # 50ms timeout
        )
        
        if should_flush and state['text_delta_buffer']:
            await self._raise_text_delta(state['text_delta_buffer'], **callback_opts)
            state['text_delta_buffer'] = ""
            state['last_text_delta_time'] = current_time

    async def _flush_text_delta_buffer(self, state, callback_opts):
        """Flush any remaining text in the buffer."""
        if state['text_delta_buffer']:
            await self._raise_text_delta(state['text_delta_buffer'], **callback_opts)
            state['text_delta_buffer'] = ""
            state['last_text_delta_time'] = time.time()


    async def _finalize_tool_calls(self, state, tool_chest, session_manager, messages, callback_opts, tool_context, client_wants_cancel: threading.Event = None):
        """Finalize tool calls after receiving a complete message."""
        # Check for cancellation before processing tool calls
        if client_wants_cancel and client_wants_cancel.is_set():
            self.logger.info("Client requested cancellation during tool call processing.")
            
            # Clean up any unfulfilled tool calls from the assistant message
            await self._cleanup_unfulfilled_tool_calls(state, messages, callback_opts)
            
            # Set cancellation state
            state['complete'] = True
            state['stop_reason'] = "client_cancel"
            return
            
        # ToolCallEvent with active=true is now handled in _handle_message_stop
        # before the completion event, so no additional activation is needed here

        # Process tool calls and get response messages
        tool_response_messages = await self.__tool_calls_to_messages(
            state,
            tool_chest,
            tool_context
        )

        # Add tool response messages to the conversation history
        messages.extend(tool_response_messages)

        await self._raise_tool_call_end(
            state['collected_tool_calls'],
            messages[-1]['content'],
            vendor="anthropic",
            **callback_opts
        )

    async def _cleanup_unfulfilled_tool_calls(self, state, messages, callback_opts):
        """Clean up unfulfilled tool calls from message history when cancelled."""
        # If we have model outputs with tool calls, we need to remove the tool call blocks
        # but keep any text content that was generated before the tool calls
        if state['model_outputs']:
            # Filter out tool_use blocks from model_outputs, keeping only text blocks
            cleaned_outputs = []
            for output in state['model_outputs']:
                if output.get('type') != 'tool_use':
                    cleaned_outputs.append(output)
            
            # Update the state with cleaned outputs
            state['model_outputs'] = cleaned_outputs
            
            # If there's any content left, add it to messages
            if cleaned_outputs:
                msg = {'role': 'assistant', 'content': cleaned_outputs}
                messages.append(msg)
                await self._raise_history_delta([msg], **callback_opts)


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

    def _validate_and_parse_json(self, json_str: str, tool_name: str = "unknown") -> dict:
        """Validate JSON completeness and parse safely.
        
        Args:
            json_str: Raw JSON string to parse
            tool_name: Name of tool for logging purposes
            
        Returns:
            Parsed JSON dict, or empty dict if parsing fails
        """
        if not json_str or not json_str.strip():
            self.logger.warning(f"Empty JSON string for tool {tool_name}")
            return {}
            
        json_str = json_str.strip()
        
        # Basic JSON completeness validation
        if not (json_str.startswith('{') and json_str.endswith('}')):
            self.logger.warning(f"JSON appears incomplete for tool {tool_name}: {json_str[:50]}...")
            return {}
            
        try:
            return json.loads(json_str)
        except json.JSONDecodeError as e:
            self.logger.warning(f"Failed to parse JSON for tool {tool_name}: {e}. JSON: {json_str[:100]}...")
            return {}

    async def __tool_calls_to_messages(self, state, tool_chest, tool_context):
        """Process tool calls with lazy JSON parsing.
        
        This method now handles JSON parsing for non-think tools at execution time
        rather than during streaming, improving memory efficiency.
        """
        # Create a copy of tool calls to avoid modifying the original state
        processed_tool_calls = []
        
        for tool_call in state['collected_tool_calls']:
            processed_call = tool_call.copy()
            tool_name = tool_call.get('name', 'unknown')
            tool_input = tool_call.get('input')
            
            # CRITICAL: Think tools are already parsed and ready to use
            if tool_name == 'think':
                # Think tool input is already parsed - use as-is
                processed_tool_calls.append(processed_call)
            else:
                # For regular tools: parse JSON string now (lazy parsing)
                if isinstance(tool_input, str):
                    # Parse the JSON string that was stored during streaming
                    parsed_input = self._validate_and_parse_json(tool_input, tool_name)
                    processed_call['input'] = parsed_input
                elif isinstance(tool_input, dict):
                    # Already parsed (fallback case) - use as-is
                    processed_call['input'] = tool_input
                else:
                    # Unexpected type - provide empty dict
                    self.logger.warning(f"Unexpected input type for tool {tool_name}: {type(tool_input)}")
                    processed_call['input'] = {}
                    
                processed_tool_calls.append(processed_call)
        
        # Call tools with processed (lazily parsed) tool calls
        tools_calls = await tool_chest.call_tools(processed_tool_calls, tool_context, format_type="claude")
        return tools_calls




class ClaudeBedrockChatAgent(ClaudeChatAgent):
    def __init__(self, **kwargs) -> None:
        kwargs['allow_betas'] = False
        super().__init__(**kwargs)

    @classmethod
    def client(cls, **opts):
        return AsyncAnthropicBedrock(**opts)

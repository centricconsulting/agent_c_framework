import copy
import json
import os

import yaml
import base64
import threading
from enum import Enum, auto
from typing import Any, List, Union, Dict, Tuple, Optional

from anthropic import AsyncAnthropic, APITimeoutError, Anthropic, RateLimitError, AsyncAnthropicBedrock, AsyncMessageStreamManager
from httpcore import RemoteProtocolError

from agent_c.agents.base import BaseAgent
from agent_c.chat.session_manager import ChatSessionManager
from agent_c.models.context.interaction_context import InteractionContext
from agent_c.models.events import ToolCallDeltaEvent, TextDeltaEvent
from agent_c.models.input import FileInput
from agent_c.models.input.audio_input import AudioInput
from agent_c.models.input.image_input import ImageInput
from agent_c.agent_runtimes.claude_token_counter import ClaudeTokenCounter
from agent_c_api.tests.v2.history.test_events import session_id


class ClaudeChatAgent(BaseAgent):
    CLAUDE_MAX_TOKENS: int = 64000

    def __init__(self, **kwargs) -> None:
        """
        Initialize ChatAgent object.

        Non-Base Parameters:
        client: AsyncAnthropic, default is AsyncAnthropic()
            The client to use for making requests to the Anthropic API.
        max_tokens: int, optional
            The maximum number of tokens to generate in the response.
        """
        kwargs['token_counter'] = kwargs.get('token_counter', ClaudeTokenCounter())
        super().__init__(**kwargs)
        self.client: Union[AsyncAnthropic, AsyncAnthropicBedrock] = kwargs.get("client", self.__class__.client())
        self.supports_multimodal = True
        self.can_use_tools = True
        self.allow_betas = kwargs.get("allow_betas", True)

    @classmethod
    def client(cls, **opts):
        return AsyncAnthropic(**opts)

    @property
    def tool_format(self) -> str:
        return "claude"


    def _context_to_completion_opts(self, context: InteractionContext) -> dict[str, Any]:
        """
        Convert InteractionContext to completion options for Claude API.
        """
        opts = context.agent.agent_params.model_dump(exclude_none=True)
        opts['messages'] = context.messages

        if self.allow_betas:
            if '-4-' in context.agent.agent_params.model_name:
                opts['betas'] = os.environ.get('CLAUDE_4_BETAS', "interleaved-thinking-2025-05-14,files-api-2025-04-14").split(',')
            elif '3-7-sonnet' in context.agent.agent_params.model_name:
                opts['betas'] = os.environ.get('CLAUDE_3_7_BETAS', "token-efficient-tools-2025-02-19,output-128k-2025-02-19,files-api-2025-04-14").split(',')

        opts['tools'] = context.tool_schemas

        return opts

    async def agent_interaction(self, context: InteractionContext):

        stream = self.client.messages.stream(**self._context_to_completion_opts(context))

        current_tool = None
        tool_calls = []

        stop_reason = None

        # Process the stream
        async for chunk in stream:
            if hasattr(chunk, 'stop_reason'):
                stop_reason = chunk.stop_reason

            if chunk.type == "message_start":
                continue

            elif chunk.type == "content_block_start":
                if chunk.content_block.type == "tool_use":
                    current_tool = {"id": chunk.content_block.id, "name": chunk.content_block.name, "input": ""}
                    await context.streaming_callback(ToolCallDeltaEvent(session_id=context.session_id, tool_calls= [current_tool]))

            elif chunk.type == "content_block_delta":
                if chunk.delta.type == "text_delta":
                    await context.streaming_callback(TextDeltaEvent(session_id=context.session_id, text=chunk.delta.text))

                elif chunk.delta.type == "input_json_delta":
                    if current_tool:
                        current_tool["input"] += chunk.delta.partial_json

            elif chunk.type == "content_block_stop":
                if current_tool:
                    try:
                        current_tool["input"] = json.loads(str(current_tool["input"]))
                    except json.JSONDecodeError:
                        self.logger.exception("Failed to decode JSON input for tool call", exc_info=True)

                    tool_calls.append(current_tool)
                    current_tool = None

            elif chunk.type == "message_stop":
                final_message = await stream.get_final_message()
                # Execute tools if needed
                if stop_reason == "tool_use" and tool_calls:
                    tool_results = []


                    for tool in tool_calls:
                        result = await execute_tool(tool["name"], tool["input"])
                        tool_results.append({
                            "type": "tool_result",
                            "tool_use_id": tool["id"],
                            "content": str(result)
                        })

                    # Continue conversation
                    context.messages.append({"role": "assistant", "content": final_message.content})
                    context.messages.append({"role": "user", "content": tool_results})

                    return await self.agent_interaction(context)


                return context





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

        completion_opts = {"model": model_name, "messages": messages,
                           "system": sys_prompt, "max_tokens": max_tokens,
                           'temperature': temperature}

        if '3-7-sonnet' in model_name or '-4-' in model_name:
            if allow_server_tools:
                max_searches: int = kwargs.get("max_searches", 0)
                if max_searches > 0:
                    functions.append({"type": "web_search_20250305", "name": "web_search", "max_uses": max_searches})

            if self.allow_betas:
                if allow_server_tools:
                    functions.append({"type": "code_execution_20250522", "name": "code_execution"})

                if '-4-' in model_name:
                    if max_tokens == self.CLAUDE_MAX_TOKENS:
                        if 'sonnet' in model_name:
                            completion_opts['max_tokens'] = 64000
                        else:
                            completion_opts['max_tokens'] = 32000

                    completion_opts['betas'] = ['interleaved-thinking-2025-05-14', "files-api-2025-04-14"]  # , "code-execution-2025-05-22"]
                else:
                    completion_opts['betas'] = ["token-efficient-tools-2025-02-19", "output-128k-2025-02-19", "files-api-2025-04-14"]  # , "code-execution-2025-05-22"]
                    if max_tokens == self.CLAUDE_MAX_TOKENS:
                        completion_opts['max_tokens'] = 128000

        budget_tokens: int = kwargs.get("budget_tokens", self.budget_tokens)
        if budget_tokens > 0:
            completion_opts['thinking'] = {"budget_tokens": budget_tokens, "type": "enabled"}
            completion_opts['temperature'] = 1

        if len(functions):
            completion_opts['tools'] = functions

        session_manager: Union[ChatSessionManager, None] = kwargs.get("session_manager", None)
        if session_manager is not None:
            completion_opts["metadata"] = {'user_id': session_manager.user.user_id}

        opts = {"callback_opts": callback_opts, "completion_opts": completion_opts, 'tool_chest': tool_chest, 'tool_context': tool_context}
        return opts

    @staticmethod
    def process_escapes(text):
        return text.replace("\\n", "\n").replace('\\"', '"').replace("\\\\", "\\")

    async def chat(self, **kwargs) -> List[dict[str, Any]]:
        """Main method for interacting with Claude API. Split into smaller helper methods for clarity."""
        opts = await self.__interaction_setup(**kwargs)
        client_wants_cancel: threading.Event = kwargs.get("client_wants_cancel")
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
                        client_wants_cancel,
                        opts["tool_context"]
                    )

                    if state['complete'] and state['stop_reason'] != 'tool_use':
                        self.logger.info(f"Interaction {interaction_id} stopped with reason: {state['stop_reason']}")
                        return result
                    delay = 1
                    messages = result
                except RateLimitError:
                    self.logger.warning(f"Ratelimit. Retrying...Delay is {delay} seconds")
                    await self._raise_system_event(f"Claude API is overloaded, retrying... Delay is {delay} seconds \n", severity="warning", **callback_opts)
                    delay = await self._handle_retryable_error(delay)
                except APITimeoutError:
                    self.logger.warning(f"API Timeout. Retrying...Delay is {delay} seconds")
                    await self._raise_system_event(f"Claude API is overloaded, retrying... Delay is {delay} seconds \n", severity="warning", **callback_opts)
                    delay = await self._handle_retryable_error(delay)
                except RemoteProtocolError:
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
                        await self._raise_system_event(f"Exception calling `client.messages.stream`.\n\n{e}\n", **callback_opts)
                        await self._raise_completion_end(opts["completion_opts"], stop_reason="exception", **callback_opts)
                        return []

        self.logger.warning("Claude API is overloaded. GIVING UP")
        await self._raise_system_event(f"Claude API is overloaded. GIVING UP.\n", **callback_opts)
        await self._raise_completion_end(opts["completion_opts"], stop_reason="overload", **callback_opts)
        return messages

    async def _handle_retryable_error(self, delay):
        """Handle retryable errors with exponential backoff."""
        await self._exponential_backoff(delay)
        return delay * 2  # Return the new delay for the next attempt



    async def _finalize_tool_calls(self, state, tool_chest, session_manager, messages, callback_opts, tool_context):
        """Finalize tool calls after receiving a complete message."""
        await self._raise_tool_call_start(state['collected_tool_calls'], vendor="anthropic", **callback_opts)

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
                        contents.append({"type": "document", "source": {"type": "file", "file_id": file_upload.id}})
                    except Exception as e:
                        self.logger.exception(f"Error uploading file {file.file_name}: {e}", exc_info=True)
                        continue
                elif "pdf" in file.content_type.lower() or ".pdf" in str(file.file_name).lower():
                    pdf_source = {"type": "base64", "media_type": file.content_type, "data": file.content}
                    contents.append({"type": "document", "source": pdf_source, "cache_control": {"type": "ephemeral"}})
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



class ClaudeBedrockChatAgent(ClaudeChatAgent):
    def __init__(self, **kwargs) -> None:
        kwargs['allow_betas'] = False
        super().__init__(**kwargs)

    @classmethod
    def client(cls, **opts):
        return AsyncAnthropicBedrock(**opts)

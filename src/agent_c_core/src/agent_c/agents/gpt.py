import os
import json
import asyncio
import threading

import openai
import tiktoken
import logging
from collections import defaultdict
from openai import AsyncOpenAI, AsyncStream, AsyncAzureOpenAI
from tiktoken import Encoding, encoding_for_model
from openai.types.chat import ChatCompletionChunk
from typing import Any, Dict, List, Union, Optional, Tuple

from agent_c.chat.session_manager import ChatSessionManager
from agent_c.models.input import FileInput
from agent_c.models.input.audio_input import AudioInput
from agent_c.models.events.chat import ReceivedAudioDeltaEvent
from agent_c.models.input.image_input import ImageInput
from agent_c.util.token_counter import TokenCounter
from agent_c.agents.base import BaseAgent
from agent_c.util.logging_utils import LoggingManager


class TikTokenTokenCounter(TokenCounter):
    """
    This is a token counter that uses the TikToken Encoding model to count tokens.
    """

    def __init__(self, model_name: str = "gpt-3.5-turbo"):
        self.encoder: Encoding = encoding_for_model(model_name)

    def count_tokens(self, text: str) -> int:
        return len(self.encoder.encode(text))


class GPTChatAgent(BaseAgent):
    REASONING_MODELS: List[str] = ["o1", 'o1-mini', 'o3', 'o3-mini']

    def __init__(self, **kwargs) -> None:
        """
        Initialize ChatAgent object.

        Non-Base Parameters:
        client: AsyncOpenAI, default is AsyncOpenAI()
            The client to use for making requests to the Open AI API.
        """
        kwargs['model_name']: str = kwargs.get('model_name')
        kwargs['token_counter'] = kwargs.get('token_counter', TikTokenTokenCounter())
        super().__init__(**kwargs)
        self.schemas: Union[None, List[Dict[str, Any]]] = None

        # Initialize logger
        logging_manager = LoggingManager(__name__)
        self.logger = logging_manager.get_logger()

        # Initialize the client based on environment or provided client
        if kwargs.get("client", None) is None:
            azure_endpoint = os.environ.get("AZURE_OPENAI_ENDPOINT")
            azure_api_key = os.environ.get("AZURE_OPENAI_API_KEY")
            if azure_endpoint is not None and azure_api_key is not None:
                self.logger.debug(
                    "Initializing ****AsyncAzureOpenAI**** with endpoint: %s and API version: %s",
                    azure_endpoint,
                    os.environ.get("AZURE_OPENAI_API_VERSION")
                )
                self.client = AsyncAzureOpenAI(
                    azure_endpoint=os.environ["AZURE_OPENAI_ENDPOINT"],
                    api_key=os.environ["AZURE_OPENAI_API_KEY"],
                    api_version=os.environ.get("AZURE_OPENAI_API_VERSION", "2024-03-01-preview")
                )
                self.model_name = os.environ.get("AZURE_OPENAI_MODEL", self.model_name)
            else:
                self.logger.debug("Initializing default ****AsyncOpenAI**** client")
                self.client = kwargs.get("client", AsyncOpenAI())
        else:
            self.logger.debug("Initializing with provided client.")
            self.client = kwargs.get("client")

        self.encoding = tiktoken.encoding_for_model('gpt-3.5-turbo')
        self.can_use_tools = True
        self.supports_multimodal = True

        # Temporary until all the models support this
        if self.model_name in self.__class__.REASONING_MODELS:
            self.root_message_role = "developer"

    @classmethod
    def client(cls, **opts):
        return AsyncOpenAI(**opts)

    @property
    def tool_format(self) -> str:
        """
        Returns the tool format for the agent.
        """
        return "openai"

    def _generate_multi_modal_user_message(self, user_input: str, images: List[ImageInput],
                                           audio_clips: List[AudioInput], files: List[FileInput] = None) -> Union[
        List[dict[str, Any]], None]:
        """
        Generates a multimodal message containing text, images, audio, and file content.
        """
        self.logger.debug("Starting _generate_multi_modal_user_message")
        contents = []

        # Add text content if available
        if user_input is not None and len(user_input) > 0:
            contents.append({"type": "text", "text": user_input})

        # Add images
        for image in images:
            url: Union[str, None] = image.url

            if url is None and image.content is not None:
                url = f"data:{image.content_type};base64,{image.content}"

            if url is not None:
                contents.append({"type": "image_url", "image_url": {"url": url}})

        # Add audio clips
        for clip in audio_clips:
            contents.append({"type": "input_audio", "input_audio": {"data": clip.content, 'format': clip.format}})

        # Add file content as additional text blocks
        if files:
            for file in files:
                text_content = file.get_text_content()

                if text_content:
                    file_name = file.file_name or "unknown file"
                    contents.append({
                        "type": "text",
                        "text": f"Content from file {file_name}:\n{text_content}"
                    })
                else:
                    # If no text content available, at least mention the file
                    file_name = file.file_name or "unknown file"
                    contents.append({
                        "type": "text",
                        "text": f"[File uploaded by user: {file_name}. But preprocessing failed to extract any text]"
                    })

        return [{"role": "user", "content": contents}]

    async def __interaction_setup(self, **kwargs) -> dict[str, Any]:
        """
        Set up the interaction parameters for the OpenAI API request.
        """
        json_mode: bool = kwargs.get("json_mode", False)
        model_name: str = kwargs.get("model_name", self.model_name)
        if model_name is None:
            raise ValueError('GPT agent is missing a model_name')

        (tool_context, prompt_context) = await self._render_contexts(**kwargs)
        sys_prompt: str = prompt_context["system_prompt"]
        temperature: float = kwargs.get("temperature", self.temperature)
        max_tokens: Optional[int] = kwargs.get("max_tokens", None)
        tool_choice: str = kwargs.get("tool_choice", "auto")
        voice: Optional[str] = kwargs.get("voice", None)
        tool_chest = kwargs.get("tool_chest", self.tool_chest)

        messages = await self._construct_message_array(system_prompt=sys_prompt, **kwargs)
        functions: List[Dict[str, Any]] = tool_chest.active_open_ai_schemas

        # Configure completion options based on model type
        if model_name in self.__class__.REASONING_MODELS:
            reasoning_effort = kwargs.get("reasoning_effort", "medium")
            completion_opts = {
                "model": model_name,
                "messages": messages,
                "stream": True,
                "reasoning_effort": reasoning_effort
            }
        else:
            completion_opts = {
                "model": model_name,
                "temperature": temperature,
                "messages": messages,
                "stream": True
            }
            if voice is not None:
                completion_opts['modalities'] = ["text", "audio"]
                completion_opts['audio'] = {"voice": voice, "format": "pcm16"}

        completion_opts['stream_options'] = {"include_usage": True}

        # Add tools if available
        if len(functions):
            completion_opts['tools'] = functions
            completion_opts['tool_choice'] = tool_choice

        # Add max tokens if specified
        if max_tokens is not None:
            completion_opts['max_completion_tokens'] = max_tokens

        # Set JSON response format if required
        if json_mode:
            completion_opts['response_format'] = {"type": "json_object"}

        # Add user identifier if available
        user = kwargs.get("user_name", None)
        if user is not None:
            completion_opts["user"] = user

        return {
            'completion_opts': completion_opts,
            'callback_opts': self._callback_opts(**kwargs),
            'tool_chest': tool_chest,
            'tool_context': tool_context
        }

    async def _save_audio_interaction_to_session(self, mgr: ChatSessionManager, audio_id, transcript: str):
        """
        Save audio interaction to the session.
        """
        self.logger.debug("Starting _save_audio_interaction_to_session")
        await self._save_message_to_session(mgr, transcript, "assistant")
        return {"role": "assistant", "audio": {"id": audio_id}}

    def _init_stream_state(self) -> Dict[str, Any]:
        """
        Initialize the state object for stream processing.
        """
        return {
            "collected_messages": [],  # Collects text fragments during streaming
            "collected_tool_calls": [],  # Collects tool calls during streaming
            "input_tokens": -1,  # Tracks input token usage
            "output_tokens": -1,  # Tracks output token usage
            "current_audio_id": None,  # For audio responses
            "stop_reason": None,  # Reason for stream stopping
            "complete": False,  # Whether streaming is complete
            "tool_calls_processed": False,  # Whether tool calls were processed
        }

    async def _handle_retryable_error(self, error, delay, callback_opts):
        """
        Handle retryable errors with exponential backoff.
        """
        error_type = type(error).__name__
        await self._raise_system_event(
            f"Warning: The Claude streaming API may be under heavy load or you have hit your rate Limit.\n\n Delaying for {delay} seconds.\n",
            **callback_opts
        )
        await self._exponential_backoff(delay)
        return delay * 2  # Return the new delay for the next attempt

    async def chat(self, **kwargs) -> List[dict[str, Any]]:
        """
        Main method for performing chat operations with GPT models.
        Controls the overall interaction flow and handles retries.
        """
        self.logger.debug("Starting chat")
        opts = await self.__interaction_setup(**kwargs)
        messages = opts['completion_opts']['messages'].copy()
        session_manager = kwargs.get("session_manager", None)
        tool_chest = opts['tool_chest']
        callback_opts = opts['callback_opts']
        client_wants_cancel: threading.Event = kwargs.get("client_wants_cancel")
        delay = 1  # Initial delay between retries

        async with self.semaphore:
            self.logger.debug("Starting Interaction")
            interaction_id = await self._raise_interaction_start(**callback_opts)

            # Main loop - continues until we get a complete response or fail
            while delay <= self.max_delay:
                try:
                    # Process the stream with a separate handler method
                    result, state = await self._handle_gpt_stream(
                        opts['completion_opts'],
                        tool_chest,
                        session_manager,
                        messages,
                        callback_opts,
                        interaction_id,
                        client_wants_cancel,
                        opts['tool_context']
                    )

                    # If we completed without tool calls, we're done
                    if state['complete'] and not state['tool_calls_processed']:
                        return result

                    # Otherwise update messages and continue the loop for tool call follow-up
                    messages = result
                    # Update completion options with updated messages for next API call
                    opts['completion_opts']['messages'] = messages.copy()

                except openai.BadRequestError as e:
                    self.logger.error(f"Invalid request occurred: {e}")
                    await self._raise_system_event(f"Invalid request error: {e}", **callback_opts)
                    await self._raise_completion_end(opts["completion_opts"], stop_reason="exception", **callback_opts)
                    await self._raise_interaction_end(id=interaction_id, **callback_opts)
                    raise

                except (openai.APITimeoutError, openai.InternalServerError) as e:
                    # Handle retryable errors with exponential backoff
                    delay = await self._handle_retryable_error(e, delay, callback_opts)
                    if delay > self.max_delay:
                        await self._raise_interaction_end(id=interaction_id, **callback_opts)
                        raise

                except Exception as e:
                    self.logger.error(f"Error occurred during chat completion: {e}")
                    await self._raise_system_event(f"Exception in chat completion: {e}", **callback_opts)
                    await self._raise_completion_end(opts["completion_opts"], stop_reason="exception", **callback_opts)
                    await self._raise_interaction_end(id=interaction_id, **callback_opts)
                    raise

        return messages

    async def _handle_gpt_stream(self, completion_opts, tool_chest, session_manager,
                                 messages, callback_opts, interaction_id,
                                 client_wants_cancel: threading.Event,
                                 tool_context: Dict[str, Any]) -> Tuple[
        List[Dict[str, Any]], Dict[str, Any]]:
        """
        Handle the OpenAI stream processing.
        Similar to Claude's _handle_claude_stream but adapted for OpenAI's streaming format.
        """
        # Signal start of completion
        await self._raise_completion_start(completion_opts, **callback_opts)

        # Initialize state
        state = self._init_stream_state()

        # Start API call
        async with await self.client.chat.completions.create(**completion_opts) as stream:

            try:
                async for chunk in stream:
                    # Process each chunk through appropriate handler
                    await self._process_stream_chunk(chunk, state, tool_chest, session_manager,
                                                     messages, callback_opts)

                    if client_wants_cancel.is_set():
                        state['complete'] = True
                        state['stop_reason'] = "client_cancel"

                    # If we've completed processing and it's not a tool call, we're done
                    if state['complete'] and not state['tool_calls_processed']:
                        # Add the collected content to messages and finalize
                        output_text = "".join(state["collected_messages"])
                        if output_text.strip():  # Only add if there's actual content
                            if state["current_audio_id"] is not None:
                                messages.append(await self._save_audio_interaction_to_session(
                                    session_manager, state["current_audio_id"], output_text
                                ))
                            else:
                                messages.append(await self._save_interaction_to_session(session_manager, output_text))

                        # Finalize events
                        await self._raise_history_event(messages, **callback_opts)
                        await self._raise_interaction_end(id=interaction_id, **callback_opts)
                        return messages, state

                    # If we've completed and there are tool calls, process them
                    elif state['complete'] and state['tool_calls_processed']:
                        await self._process_tool_calls(state, tool_chest, session_manager, messages, callback_opts, tool_context)
                        return messages, state

            except Exception as e:
                self.logger.error(f"Error during stream processing: {e}")
                raise

        # Ensure we handle any pending updates before returning
        if state['collected_messages'] and not state['complete']:
            output_text = "".join(state["collected_messages"])
            if output_text.strip():
                if state["current_audio_id"] is not None:
                    messages.append(await self._save_audio_interaction_to_session(
                        session_manager, state["current_audio_id"], output_text
                    ))
                else:
                    messages.append(await self._save_interaction_to_session(session_manager, output_text))

            # Raise events
            await self._raise_history_event(messages, **callback_opts)

        # End completion event
        await self._raise_completion_end(
            completion_opts,
            stop_reason=state['stop_reason'],
            input_tokens=state['input_tokens'],
            output_tokens=state['output_tokens'],
            **callback_opts
        )

        return messages, state

    async def _process_stream_chunk(self, chunk, state, tool_chest, session_manager,
                                    messages, callback_opts):
        """
        Process a single chunk from the OpenAI stream.
        Delegates to specific handlers based on content type.
        """
        if chunk.choices is None:
            return

        if len(chunk.choices) == 0:
            # This is likely usage information at the end of the stream
            await self._handle_usage_info(chunk, state, callback_opts)
            return

        # Process the first choice
        first_choice = chunk.choices[0]

        # Check if we have a finish reason
        if first_choice.finish_reason is not None:
            state['stop_reason'] = first_choice.finish_reason
            state['complete'] = True
            return

        # Process different types of delta content
        if first_choice.delta.tool_calls is not None:
            await self._handle_tool_call_delta(first_choice.delta.tool_calls[0], state, callback_opts)
        elif first_choice.delta.content is not None:
            await self._handle_content_delta(first_choice.delta.content, state, callback_opts)
        elif first_choice.delta.model_extra and first_choice.delta.model_extra.get('audio', None) is not None:
            await self._handle_audio_delta(first_choice.delta.model_extra['audio'], state, callback_opts)

    async def _handle_usage_info(self, chunk, state, callback_opts):
        """
        Handle usage information chunks that come at the end of the stream.
        """
        if chunk.usage is not None:
            state['input_tokens'] = chunk.usage.prompt_tokens
            state['output_tokens'] = chunk.usage.completion_tokens

    async def _handle_tool_call_delta(self, tool_call, state, callback_opts):
        """
        Handle tool call delta events.
        """
        index = tool_call.index
        if index == len(state["collected_tool_calls"]):
            state["collected_tool_calls"].append(defaultdict(str))

        # Update tool call with new information
        tc = state["collected_tool_calls"][index]
        if tool_call.id:
            tc['id'] = tool_call.id
        if tool_call.function:
            if tool_call.function.name:
                tc['name'] = tool_call.function.name
            if tool_call.function.arguments:
                tc['arguments'] += tool_call.function.arguments

        # Mark that we've seen tool calls
        state["tool_calls_processed"] = True

    async def _handle_content_delta(self, content, state, callback_opts):
        """
        Handle text content delta events.
        """
        state["collected_messages"].append(content)
        try:
            await self._raise_text_delta(content, **callback_opts)
        except Exception as e:
            self.logger.error(f"Error raising text_delta event: {e}")

    async def _handle_audio_delta(self, audio_delta, state, callback_opts):
        """
        Handle audio content delta events.
        """
        if state["current_audio_id"] is None:
            state["current_audio_id"] = audio_delta.get('id', None)

        transcript = audio_delta.get('transcript', None)
        if transcript is not None:
            state["collected_messages"].append(transcript)
            await self._raise_text_delta(transcript, **callback_opts)

        b64_audio = audio_delta.get('data', None)
        if b64_audio is not None:
            await self._raise_event(ReceivedAudioDeltaEvent(
                content_type="audio/L16",
                id=state["current_audio_id"],
                content=b64_audio,
                **callback_opts
            ))

    async def _process_tool_calls(self, state, tool_chest, session_manager, messages, callback_opts, tool_context):
        """
        Process tool calls after stream completion.
        """
        tool_calls = state["collected_tool_calls"]
        if not tool_calls:
            return

        self.logger.debug(f"Processing {len(tool_calls)} tool call(s)")

        # Start tool call event
        await self._raise_tool_call_start(tool_calls, vendor="open_ai", **callback_opts)

        try:
            # Execute tool calls
            result_messages = await self.__tool_calls_to_messages(tool_calls, tool_chest, tool_context)

            if result_messages:
                # End tool call event with results
                await self._raise_tool_call_end(tool_calls, result_messages[1:], vendor="open_ai", **callback_opts)
                messages.extend(result_messages)
                await self._raise_history_event(messages, **callback_opts)
        except Exception as e:
            self.logger.error(f"Failed calling tool sets: {e}")
            await self._raise_tool_call_end(tool_calls, [], vendor="open_ai", **callback_opts)
            await self._raise_system_event(f"An error occurred while processing tool calls: {e}", **callback_opts)

    async def __tool_calls_to_messages(self, tool_calls, tool_chest, tool_context):
        return await tool_chest.call_tools(tool_calls, tool_context, format_type="openai")

class AzureGPTChatAgent(GPTChatAgent):
    """
    Azure-specific implementation of the GPTChatAgent.
    Inherits from GPTChatAgent and overrides the client initialization.
    """
    @classmethod
    def client(cls, **opts):
        return AsyncAzureOpenAI(**opts)

import os
from pydantic import Field

import openai


from collections import defaultdict
from openai import AsyncOpenAI,  AsyncAzureOpenAI
from tiktoken import Encoding, encoding_for_model
from typing import Any, Dict, List, Union, Optional, Tuple

from agent_c.agent_runtimes.runtime_registry import RuntimeRegistry
from agent_c.models.completion.azure_auth_info import AzureAuthInfo
from agent_c.models.completion.gpt import GPTCompletionParams
from agent_c.models.completion.open_ai_auth_info import OpenAiAuthInfo
from agent_c.models.config import BaseRuntimeConfig
from agent_c.models.context.interaction_context import InteractionContext
from agent_c.models.events.chat import ReceivedAudioDeltaEvent
from agent_c.util.token_counter import TokenCounter
from agent_c.agent_runtimes.base import AgentRuntime
from agent_c.util.logging_utils import LoggingManager

logger = LoggingManager(__name__).get_logger()

class TikTokenTokenCounter(TokenCounter):
    """
    This is a token counter that uses the TikToken Encoding model to count tokens.
    """

    def __init__(self, model_name: str = "gpt-3.5-turbo"):
        self.encoder: Encoding = encoding_for_model(model_name)

    def count_tokens(self, text: str) -> int:
        return len(self.encoder.encode(text))

class OpenAIConfig(BaseRuntimeConfig):
    auth: Optional[OpenAiAuthInfo] = Field(default_factory= lambda: OpenAiAuthInfo(api_key=os.environ.get("OPENAI_API_KEY")),
                                           description="OpenAI authentication information, including API) key and organization ID.")

class AzureOpenAIConfig(BaseRuntimeConfig):
    auth: Optional[AzureAuthInfo] = Field(default_factory=lambda: AzureAuthInfo(),
                                           description="Azure OpenAI authentication information, including API key, endpoint, and deployment name.")

class GPTChatAgentRuntime(AgentRuntime):
    config_type: str = "open_ai"
    def __init__(self, max_retry_delay_secs: int = 300, concurrency_limit: int = 3,  context = None, client = None) -> None:

        super().__init__(TikTokenTokenCounter(), max_retry_delay_secs=max_retry_delay_secs,
                         concurrency_limit=concurrency_limit, context=context)

        self.client = client or self.client(context)

    @classmethod
    def client(cls, context = None) -> Optional[AsyncOpenAI]:
        if context and hasattr(context, 'chat_session'):
            auth = context.chat_session.agent_config.agent_params.auth
            api_key = auth.api_key if hasattr(auth, 'api_key') else os.environ.get("OPENAI_API_KEY")
        else:
            api_key = os.environ.get("OPENAI_API_KEY")

        if not api_key:
            logger.warning("Waring an attempt was made to create an OpenAI API client without an API key.  Set OPENAI_API_KEY to use this runtime ")
            return None

        return AsyncOpenAI(api_key=api_key)

    @property
    def tool_format(self) -> str:
        """
        Returns the tool format for the agent.
        """
        return "openai"

    def _generate_multi_modal_user_message(self, context: InteractionContext) -> Union[
        List[dict[str, Any]], None]:
        """
        Generates a multimodal message containing text, images, audio, and file content.
        """
        self.logger.debug("Starting _generate_multi_modal_user_message")
        contents = []

        if context.inputs.text.content is not None and len(context.inputs.text.conten) > 0:
            contents.append({"type": "text", "text": context.inputs.text.conten})

        for image in context.input.images:
            url: Optional[str] = image.url

            if url is None and image.content is not None:
                url = f"data:{image.content_type};base64,{image.content}"

            if url is not None:
                contents.append({"type": "image_url", "image_url": {"url": url}})

        for clip in context.input.audio_clips:
            contents.append({"type": "input_audio", "input_audio": {"data": clip.content, 'format': clip.format}})


        for file in context.input.files:
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

    @staticmethod
    async def __build_completion_options(context: InteractionContext) -> Dict[str, Any]:
        """
        Set up the interaction parameters for the OpenAI API request.
       """
        params: GPTCompletionParams = context.chat_session.agent_config.agent_params
        completion_opts = params.as_completion_params()
        completion_opts['stream'] = True  # Enable streaming responses
        completion_opts['stream_options'] = {"include_usage": True}

        if params.voice:
            completion_opts['modalities'] = ["text", "audio"]
            completion_opts['audio'] = {"voice": params.voice, "format": "pcm16"}

        completion_opts["user"] = context.chat_session.user_id

        return completion_opts

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

    async def _handle_retryable_error(self, context, error, delay):
        """
        Handle retryable errors with exponential backoff.
        """
        error_type = type(error).__name__
        await self._raise_system_event( context,
            f"Warning: The Open AI API may be under heavy load or you have hit your rate Limit.\n\n Delaying for {delay} seconds.\n" )

        await self._exponential_backoff(delay)
        return delay * 2  # Return the new delay for the next attempt

    async def __add_system_prompt_and_tools(self, context: InteractionContext,  completion_opts: Dict[str, Any]) -> Dict[str, Any]:
        """
        Add system prompt and tools to the completion options.
        """
        tool_schemas = context.tool_schemas

        system_prompt = await self.prompt_builder.render(context)
        if 'messages' not in completion_opts:
            completion_opts['messages'] = await self._construct_message_array(context, system_prompt)
        else:
            completion_opts['messages'][0]["content"] = system_prompt

        await self._raise_system_prompt(context, system_prompt)

        if tool_schemas:
            completion_opts["tools"] = tool_schemas

        return completion_opts

    async def chat(self, context: InteractionContext) -> List[dict[str, Any]]:
        """
        Main method for performing chat operations with GPT models.
        Controls the overall interaction flow and handles retries.
        """
        completion_opts = await self.__build_completion_options(context)
        completion_opts = await self.__add_system_prompt_and_tools(context, completion_opts)
        messages = completion_opts['messages']
        delay = 1

        async with self.semaphore:
            await self._raise_interaction_start(context)

            # Main loop - continues until we get a complete response or fail
            while delay <= self.max_delay:
                try:
                    result, state = await self._handle_gpt_stream(completion_opts, context,  messages)

                    if state['complete'] and not state['tool_calls_processed']:
                        return result

                    messages = result
                    completion_opts['messages'] = messages
                    completion_opts = await self.__add_system_prompt_and_tools(context, completion_opts)

                except openai.BadRequestError as e:
                    self.logger.exception(f"Invalid request occurred: {e}", exc_info=True)
                    await self._raise_system_event(context, f"Invalid request error: {e}")
                    await self._raise_completion_end(context, completion_opts, stop_reason="exception")
                    await self._raise_interaction_end(context)
                    return messages

                except (openai.APITimeoutError, openai.InternalServerError) as e:
                    delay = await self._handle_retryable_error(context, e, delay)
                    if delay > self.max_delay:
                        await self._raise_interaction_end(context)
                        return messages

                except Exception as e:
                    self.logger.error(f"Error occurred during chat completion: {e}")
                    await self._raise_system_event(context,"Exception in chat completion: {e}")
                    await self._raise_completion_end(context, stop_reason="exception")
                    await self._raise_interaction_end(context)
                    return messages

        return messages

    async def _handle_gpt_stream(self, completion_opts, context: InteractionContext, messages) -> Tuple[List[Dict[str, Any]], Dict[str, Any]]:
        """
        Handle the OpenAI stream processing.
        Similar to Claude's _handle_claude_stream but adapted for OpenAI's streaming format.
        """
        await self._raise_completion_start(context, completion_opts)
        state = self._init_stream_state()
        async with await self.client.chat.completions.create(**completion_opts) as stream:

            try:
                async for chunk in stream:
                    await self._process_stream_chunk(chunk, state, context, messages)

                    if context.client_wants_cancel.is_set():
                        state['complete'] = True
                        state['stop_reason'] = "client_cancel"

                    # If we've completed processing and it's not a tool call, we're done
                    if state['complete'] and not state['tool_calls_processed']:
                        output_text = "".join(state["collected_messages"]).strip()
                        if len(output_text):
                            messages.append({"role": "assistant", "content": output_text})

                        if state["current_audio_id"] is not None:
                            messages.append({"role": "assistant", "audio": {"id": state["current_audio_id"]}})

                        await self._raise_history_event(context, messages)
                        await self._raise_interaction_end(context)
                        return messages, state

                    # If we've completed and there are tool calls, process them
                    elif state['complete'] and state['tool_calls_processed']:
                        await self._process_tool_calls(state, context, messages)
                        return messages, state

            except Exception as e:
                self.logger.exception(f"Error during stream processing: {e}", exc_info=True)
                raise

        # Ensure we handle any pending updates before returning
        if state['collected_messages'] and not state['complete']:
            output_text = "".join(state["collected_messages"]).strip()
            if len(output_text):
                messages.append({"role": "assistant", "content": output_text})

            if state["current_audio_id"] is not None:
                messages.append({"role": "assistant", "audio": {"id": state["current_audio_id"]}})

            await self._raise_history_event(context, messages)


        await self._raise_completion_end( context, completion_opts, state['stop_reason'], state['input_tokens'], state['output_tokens'])
        return messages, state

    async def _process_stream_chunk(self, chunk, state, context: InteractionContext, messages):
        """
        Process a single chunk from the OpenAI stream.
        Delegates to specific handlers based on content type.
        """
        if chunk.choices is None:
            return

        if len(chunk.choices) == 0:
            await self._handle_usage_info(chunk, state)
            return

        first_choice = chunk.choices[0]
        if first_choice.finish_reason is not None:
            state['stop_reason'] = first_choice.finish_reason
            state['complete'] = True
            return

        if first_choice.delta.tool_calls is not None:
            await self._handle_tool_call_delta(first_choice.delta.tool_calls[0], state)
        elif first_choice.delta.content is not None:
            await self._handle_content_delta(first_choice.delta.content, state, context)
        elif first_choice.delta.model_extra and first_choice.delta.model_extra.get('audio', None) is not None:
            await self._handle_audio_delta(first_choice.delta.model_extra['audio'], state, context)

    @staticmethod
    async def _handle_usage_info(chunk, state):
        """
        Handle usage information chunks that come at the end of the stream.
        """
        if chunk.usage is not None:
            state['input_tokens'] = chunk.usage.prompt_tokens
            state['output_tokens'] = chunk.usage.completion_tokens

    @staticmethod
    async def _handle_tool_call_delta(tool_call, state):
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

    async def _handle_content_delta(self, content, state, context):
        """
        Handle text content delta events.
        """
        state["collected_messages"].append(content)
        await self._raise_text_delta(context, content)

    async def _handle_audio_delta(self, audio_delta, state, context):
        """
        Handle audio content delta events.
        """
        if state["current_audio_id"] is None:
            state["current_audio_id"] = audio_delta.get('id', None)

        transcript = audio_delta.get('transcript', None)
        if transcript is not None:
            state["collected_messages"].append(transcript)
            await self._raise_text_delta(context, transcript)

        b64_audio = audio_delta.get('data', None)
        if b64_audio is not None:
            await self._raise_event(context, ReceivedAudioDeltaEvent(content_type="audio/L16",
                                                                     id=state["current_audio_id"],
                                                                     content=b64_audio))

    async def _process_tool_calls(self, state, context, messages):
        """
        Process tool calls after stream completion.
        """
        tool_calls = state["collected_tool_calls"]
        if not tool_calls:
            return

        await self._raise_tool_call_start(context, tool_calls)

        try:
            result_messages = await self.__tool_calls_to_messages(tool_calls, context)

            if result_messages:
                await self._raise_tool_call_end(context, tool_calls, result_messages[1:])
                messages.extend(result_messages)
                await self._raise_history_event(context, messages)

        except Exception as e:
            self.logger.exception(f"Failed calling tool sets: {e}", exc_info=True)
            await self._raise_tool_call_end(context, tool_calls, [])
            await self._raise_system_event(context,f"An error occurred while processing tool calls: {e}")

    async def __tool_calls_to_messages(self, tool_calls, context: InteractionContext) -> List[Dict[str, Any]]:
        return await context.tool_chest.call_tools(tool_calls, context, self.tool_format)

class AzureGPTChatAgent(GPTChatAgentRuntime):
    """
    Azure-specific implementation of the GPTChatAgentRuntime.
    Inherits from GPTChatAgentRuntime and overrides the client initialization.
    """
    config_type: str = "azure_open_ai"
    @classmethod
    def client(cls, **opts):
        return AsyncAzureOpenAI(**opts)

# Register the chat agents with the runtime registry
RuntimeRegistry.register(GPTChatAgentRuntime, "openai")
RuntimeRegistry.register(AzureGPTChatAgent, "azure_openai")
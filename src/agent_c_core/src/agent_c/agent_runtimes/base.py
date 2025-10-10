import copy
import asyncio

from asyncio import Semaphore
from typing import Any, List, Union, Optional

from agent_c.util.token_counter import TokenCounter
from agent_c.util.logging_utils import LoggingManager
from agent_c.models.events.chat import ThoughtDeltaEvent, HistoryDeltaEvent, SystemPromptEvent, UserRequestEvent, InteractionStartEvent, InteractionEndEvent
from agent_c.models.events import ToolCallEvent, TextDeltaEvent, HistoryEvent, CompletionEvent, ToolCallDeltaEvent, SystemMessageEvent, SessionEvent


class AgentRuntime:
    def __init__(self, token_counter: TokenCounter, max_retry_delay_secs: int = 300,
                 concurrency_limit: int = 3, can_use_tools: bool = True, supports_multimodal: bool = True,
                 context=None):
        """
        Initialize ChatAgent object.

        Parameters:
        token_counter: TokenCounter, required
            Token counter to count tokens in messages.
        max_retry_delay_secs: int, default is 300
            Maximum delay for exponential backoff.
        concurrency_limit: int, default is 3
            Maximum number of current operations allowing for concurrent operations.
        can_use_tools: bool, default is True
            Whether the agent can use tools.
        supports_multimodal: bool, default is True
            Whether the agent supports multimodal inputs (images, audio, etc.).
        """
        self.max_delay: int = max_retry_delay_secs
        self.concurrency_limit: int = concurrency_limit
        self.can_use_tools: bool = can_use_tools
        self.supports_multimodal: bool = supports_multimodal
        self.token_counter: TokenCounter = token_counter

        self.semaphore: Semaphore = asyncio.Semaphore(self.concurrency_limit)
        self.logger = LoggingManager(self.__class__.__name__).get_logger()
        self.background_tasks = set()

    @classmethod
    def client(cls, **opts):
        raise NotImplementedError

    @classmethod
    def can_create(cls, context=None) -> bool:
        raise NotImplementedError

    @classmethod
    def tool_format(cls) -> str:
        raise NotImplementedError

    @classmethod
    def vendor(cls) -> str:
        return cls.tool_format()

    def count_tokens(self, text: str) -> int:
        return self.token_counter.count_tokens(text)

    async def one_shot(self, context) -> Optional[List[dict[str, Any]]]:
        """For text in, text out processing. without chat"""
        messages = await self.chat(context)
        if len(messages) > 0:
            return messages

        return None

    async def parallel_one_shots(self, contexts: List, **kwargs):
        """Run multiple one-shot tasks in parallel"""
        tasks = [self.one_shot(context=oneshot_input) for oneshot_input in contexts]
        return await asyncio.gather(*tasks)

    async def chat(self, context) -> List[dict[str, Any]]:
        """For chat interactions"""
        raise NotImplementedError


    async def _raise_event(self, context, event: SessionEvent):
        """
        Raise a chat event to the event stream.

        Events are sent to the streaming_callback to be sent to the application layer / UI
        """

        try:
            event.session_id = context.chat_session.user_session_id
            event.model_name = context.model_id
            task = asyncio.create_task(context.streaming_callback(event))
            self.background_tasks.add(task)
            task.add_done_callback(lambda t: self.background_tasks.discard(t))
            #await context.streaming_callback(event)
        except Exception as e:
            self.logger.exception(
                f"Streaming callback error for event: {e}. Event Type: {getattr(event, 'type', 'unknown')}")

    async def _raise_system_event(self, context, content: str, severity: str = "error"):
        """
        Raise a system event to the event stream.
        """
        await self._raise_event(context, SystemMessageEvent(role="system",
                                                            severity=severity,
                                                            content=content,
                                                            session_id=context.chat_session.user_session_id))

    async def _raise_history_delta(self, context, messages):
        """
        Raise a HistoryDelta event to the event stream.
        """
        await self._raise_event(context, HistoryDeltaEvent(messages=messages,
                                                           role=context.runtime_role,
                                                           session_id=context.chat_session.user_session_id,
                                                           vendor=self.tool_format()))

    async def _raise_completion_start(self, context, comp_options):
        """
        Raise a completion start event to the event stream.
        """
        completion_options: dict = copy.deepcopy(comp_options)
        completion_options.pop("messages", None)
        context.completion_started()
        await self._raise_event(context, CompletionEvent(running=True,
                                                         completion_options=completion_options,
                                                         session_id=context.chat_session.user_session_id))

    async def _raise_completion_end(self, context, comp_options, stop_reason: str, intput_tokens: int = 0, output_tokens: int = 0):
        """
        Raise a completion start event to the event stream.
        """
        completion_options: dict = copy.deepcopy(comp_options)
        completion_options.pop("messages", None)
        context.completion_ended()
        await self._raise_event(context, CompletionEvent(running=False,
                                                         completion_options=completion_options,
                                                         session_id=context.chat_session.user_session_id,
                                                         input_tokens=intput_tokens,
                                                         output_tokens=output_tokens,
                                                         stop_reason=stop_reason))

    async def _raise_tool_call_start(self, context, tool_calls):
        await self._raise_event(context, ToolCallEvent(active=True,
                                                       tool_calls=tool_calls,
                                                       session_id=context.chat_session.user_session_id,
                                                       vendor=self.tool_format()))

    async def _raise_system_prompt(self, context, prompt: str):
        await self._raise_event(context, SystemPromptEvent(content=prompt))

    async def _raise_user_request(self, context):
        await self._raise_event(context, UserRequestEvent(data={"message": context.inputs.text}))

    async def _raise_tool_call_delta(self, context, tool_calls):
        await self._raise_event(context, ToolCallDeltaEvent(tool_calls=tool_calls))

    async def _raise_tool_call_end(self, context, tool_calls, tool_results):
        await self._raise_event(context, ToolCallEvent(active=False,
                                                       tool_calls=tool_calls,
                                                       tool_results=tool_results,
                                                       vendor=self.vendor))

    async def _raise_interaction_start(self, context):
        await self._raise_event(context, InteractionStartEvent(id=context.interaction_id))
        context.interaction_started()
        return context.interaction_id

    async def _raise_interaction_end(self, context):
        context.interaction_ended()
        await self._raise_event(context, InteractionEndEvent(id=context.interaction_id))

    async def _raise_text_delta(self, context, content: str):
        await self._raise_event(context, TextDeltaEvent(content=content,
                                                        vendor=self.tool_format(),
                                                        role=context.runtime_role))

    async def _raise_thought_delta(self, context, content: str):
            await self._raise_event(context, ThoughtDeltaEvent(content=content,
                                                               vendor=self.tool_format(),
                                                               role=context.runtime_role))

    async def _raise_history_event(self, context, messages: List[dict[str, Any]]):
        await self._raise_event(context, HistoryEvent(messages=messages,
                                                      vendor=self.tool_format()))

    async def _exponential_backoff(self, delay: int) -> None:
        """
        Delays the execution for backoff strategy.

        Parameters
        ----------
        delay : int
            The delay in seconds.
        """
        await asyncio.sleep(min(2 * delay, self.max_delay))


    async def _generate_multi_modal_user_message(self, context) -> Union[List[dict[str, Any]], None]:
        """
        Subclasses will implement this method to generate a multimodal user message.
        """
        return None

    async def _construct_message_array(self, context, sys_prompt: Optional[str] = None) -> List[dict[str, Any]]:
        """
       Construct a message using an array of messages.

       Parameters:
       user_message: str
           User message.
       sys_prompt: str, optional
           System prompt to be used for messages[0]

       Returns: Message array as a list.
       """
        messages: List[dict[str, Any]] = context.chat_session.messages
        message_array: List[dict[str, Any]] = []

        if messages is not None:
            message_array.extend(messages)

        if len(context.inputs.images) > 0 or len(context.inputs.audio) > 0 or len(context.inputs.files) > 0:
            multimodal_user_message = await self._generate_multi_modal_user_message(context)
            message_array += multimodal_user_message
        else:
            message_array.append({"role": "user", "content": context.inputs.text.content})

        return message_array
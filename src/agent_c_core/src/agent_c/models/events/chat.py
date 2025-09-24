from pydantic import Field
from typing import Optional, List, Dict, Any, Literal

from agent_c.models.events import BaseEvent
from agent_c.models.events.session_event import SessionEvent

class SystemPromptEvent(SessionEvent):
    """
    Sent to notify the UI that the system prompt has been updated.
        - This is used for quality purposes and is captured in the session logs.
        - Most clients can ignore this event.
    """
    def __init__(self, **data):
        super().__init__(type = "system_prompt", **data)

    content: str = Field(..., description="The content of the system prompt")
    format: str = Field("markdown", description="The format of the content, default is markdown")

class UserMessageEvent(SessionEvent):
    """
    Sent to notify the UI that a user message has been added to the chat session.
    This base event is never sent directly, but is used to derive vendor specific events.
    """
    vendor: str = Field(..., description="The vendor of the model being used for the user request, e.g., 'openai', 'anthropic', etc.")

class OpenAIUserMessageEvent(UserMessageEvent):
    """
    Sent to notify the UI that a user message has been added to the chat session. When the agent runtime is openai
    """
    vendor: str = Field( "openai", description="The vendor of the model being used for the user request, e.g., 'openai', 'anthropic', etc.")
    message: Dict[str, Any] = Field(..., description="The user message that was added to the chat session")

class AnthropicUserMessageEvent(UserMessageEvent):
    """
    Sent to notify the UI that a user message has been added to the chat session. When the agent runtime is anthropic
    """
    vendor: str = Field( "anthropic", description="The vendor of the model being used for the user request, e.g., 'openai', 'anthropic', etc.")
    message: Dict[str, Any] = Field(..., description="The user message that was added to the chat session")

class InteractionEvent(SessionEvent):
    """
    Sent to notify the UI that an interaction has been initiated or completed.
    - When `started` is True, the interaction has been initiated
    - When `started` is False, the interaction has been completed.

    """
    def __init__(self, **data):
        super().__init__(type = "interaction", **data)

    started: bool = Field(..., description="If False, the interaction has been completed.")
    id: str = Field(..., description="The ID of the interaction in slug format")

class CompletionEvent(SessionEvent):
    """
    Sent to notify the UI that the completion call.
    - When `running` is True, the completion is starting.
    - When `running` is False, the completion has completed.
        - The `stop_reason` indicates why the completion stopped, if available.
        - The `input_tokens` and `output_tokens` indicate the number of tokens used in the input and output, if available.
    - The `completion_options` contains the options used for the completion call, in vendor format.  These can be ignored by most clients
    """
    def __init__(self, **data):
        super().__init__(type = "completion", **data)

    running: bool = Field(..., description="If True, the completion will run immediately after the event goes out.")
    completion_options: dict = Field(..., description="The completion options used, in vendor format")
    stop_reason: Optional[str] = Field(None, description="The reason the completion was stopped")
    input_tokens: Optional[int] = Field(0, description="The number of tokens in the input")
    output_tokens: Optional[int] = Field(0, description="The number of tokens in the output")

class MessageEvent(SessionEvent):
    """
    Sent to notify the UI to display an entire text based message, in the chat
    """
    def __init__(self, **data):
        super().__init__(type = "message", **data)

    content: str = Field(..., description="The content of the message")
    format: str = Field("markdown", description="The format of the content, default is markdown")

class SystemMessageEvent(SessionEvent):
    """
    Contains a message from the system to be displayed to the user.
    - This is NOT a part of the chat history, but a one-time message to be displayed to the user.
        - It is typically used for error messages, warnings, or informational messages the user needs to be aware of.
    """
    def __init__(self, **data):
        super().__init__(type = "system_message", **data)

    role: str = Field("system", description="The role that triggered this event event")
    content: str = Field(..., description="The content of the message")
    format: str = Field("markdown", description="The format of the content, default is markdown")
    severity: str = Field("error", description="The severity of the message, default is error, can be 'info', 'warning', or 'error'")

class TextDeltaEvent(SessionEvent):
    """
    Sent to notify the UI that a chunk of content text has been received.
    - Clients should handle this event by appending the content to the current message,
      for the role, within the current interaction.
    - If there isn't a message for the role in the current interaction,
      a new message should be created.
    """
    def __init__(self, **data):
        if data.get('type', None) == None:
            data['type'] = "text_delta"

        super().__init__( **data)

    content: str = Field(..., description="A chunk of content text.")
    format: str = Field("markdown", description="The format of the content, default is markdown")

class ThoughtDeltaEvent(TextDeltaEvent):
    """
    Sent when a token of a thought is received.
        - Clients should handle this event by appending the content to the current thought message,
    """
    def __init__(self, **data):
        super().__init__(type = "thought_delta", **data)
        self.role = self.role + " (thought)"

class CompleteThoughtEvent(TextDeltaEvent):
    """
    Sends the final thought content when a thought is completed ffor clients that don't track deltas
    """
    def __init__(self, **data):
        super().__init__(type = "complete_thought", **data)
        self.role = self.role + " (thought)"

class AudioInputDeltaEvent(BaseEvent):
    """
    Clients that cannot use binary audio input can send audio input in chunks using this event.
    """
    content: str = Field(..., description="A base64s encoded chunk of audio data")
    content_type: str = Field("audio/L16", description="The type of audio data")

class ReceivedAudioDeltaEvent(SessionEvent):
    """
    For clients that cannot handle binary audio, this event is used to send audio in base64 chunks.
    Sent to notify the UI that a chunk of audio has been received from the completion API.
    - Clients should handle this event by appending the audio to the current message,
      for the role, within the current interaction.
    - If there isn't a message for the role in the current interaction,
      a new message should be created.
    """
    def __init__(self, **data):
        super().__init__(type = "audio_delta", **data)
    id: Optional[str] = Field(None, description="The audio ID the audio delta is part of")
    content: str = Field(..., description="A base64s encoded chunk of audio data")
    content_type: str = Field("audio/L16", description="The type of audio data")

class HistoryEvent(SessionEvent):
    """
    Sent to notify the UI that the message history has been updated.
    It will contain the ENTIRE history of messages in vendor format
    Most clients should ignore this event, as they will maintain their own history by assembling deltas.
    """
    def __init__(self, **data):
        super().__init__(type = "history", **data)

    vendor: str = Field(..., description="The vendor of the model being used for the user request, e.g., 'openai', 'anthropic', etc.")
    messages: List[dict] = Field(..., description="The list of messages in the current chat history")

class HistoryDeltaEvent(SessionEvent):
    """
    Sent to notify the UI that messages have been added to the history.
    It will contain only the messages that have been added to the history in vendor format
    """
    def __init__(self, **data):
        super().__init__(type = "history_delta", **data)

    vendor: str = Field(..., description="The vendor of the model being used for the user request, e.g., 'openai', 'anthropic', etc.")
    messages: List[dict] = Field(..., description="The list of messages that have been added to the history")

class SubsessionStartedEvent(SessionEvent):
    """
    Set to notify the UI that a subsession has started.

    Events that follow this, until the SubsessionEndedEvent, are part of the subsession, not the user session
    - The client should display these events, as typical chat events, with  an indicator that they're from a subsession.
    - The server will not add these events to the user session, but will maintain them as part of the subsession.

    Note: Subsessions are often nested. There will be a matching end for each start.
    """
    sub_session_type: Literal["chat", "oneshot"] = Field(..., description="The type of the subsession, either 'chat' or 'oneshot'")
    sub_agent_type: Literal["clone", "team", "assist", "tool"] = Field(..., description="The type of the sub-agent that started the subsession, either 'clone', 'team', 'assist', or 'tool'")
    prime_agent_key: str = Field(..., description="The key of the prime agent that started the subsession, if applicable")
    sub_agent_key: str = Field(..., description="The key of the sub-agent that the prime is talking to in  the subsession")

class SubsessionEndedEvent(SessionEvent):
    """
    Set to notify the UI that a subsession has ended.

    Note: Subsessions are often nested. There will be a matching end for each start.
    """
    pass
from pydantic import Field
from typing import Optional, List

from agent_c_core.agent_c.models.events.session_event import SessionEvent

class InteractionEvent(SessionEvent):
    """
    Sent to notify the UI that an interaction has been initiated or completed.
    While an interaction is running, the UI should not allow input from the user.
    """
    def __init__(self, **data):
        super().__init__(type = "interaction", **data)

    started: bool = Field(..., description="If False, the interaction has been completed. And the UI should allow input from the use")
    id: str = Field(..., description="The ID of the interaction")


class CompletionEvent(SessionEvent):
    """
    Sent to notify the UI that the completion call
    """
    def __init__(self, **data):
        super().__init__(type = "completion", **data)

    running: bool = Field(..., description="If True, the completion will run immediately after the event goes out.")
    completion_options: dict = Field(..., description="The completion options used, in vendor format")
    stop_reason: Optional[str] = Field(None, description="The reason the completion was stopped")
    input_tokens: Optional[int] = Field(None, description="The number of tokens in the input")
    output_tokens: Optional[int] = Field(None, description="The number of tokens in the output")

class MessageEvent(SessionEvent):
    """
    Sent to notify the UI to display an entire message.
    """
    def __init__(self, **data):
        super().__init__(type = "message", **data)

    content: str = Field(..., description="The content of the message")
    format: str = Field("markdown", description="The format of the content, default is markdown")

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
    def __init__(self, **data):
        super().__init__(type = "thought_delta", **data)
        self.role = self.role + " (thought)"

class ReceivedAudioDeltaEvent(SessionEvent):
    """
    Sent to notify the UI that a chunk of audio has been received from the competion API.
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
    """
    def __init__(self, **data):
        super().__init__(type = "history", **data)

    messages: List[dict] = Field(..., description="The list of messages in the current chat history")
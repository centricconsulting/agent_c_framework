from pydantic import Field
from agent_c_core.agent_c.models.events.session_event import SemiSessionEvent


class AudioInputBeginEvent(SemiSessionEvent):
    """
    Sent to notify that audio input has begun and to start expecting deltas.
    """
    id: str = Field(..., description="The ID of the audio input, used to collate the deltas")
    sample_rate: int = Field(..., description="The sample rate of the audio input")
    channels: int = Field(..., description="The number of channels in the audio input")
    format: str = Field(..., description="The format of the audio input")
    chunk_length_s: float = Field(..., description="The length of each chunk of audio input in seconds")


class AudioInputDeltaEvent(SemiSessionEvent):
    """
    Sent to notify that audio input has been received.
    """
    id: str = Field(..., description="The ID of the audio input")
    audio: str = Field(..., description="A base64 encoded chunk of audio data")

class AudioInputEndEvent(SemiSessionEvent):
    """
    Sent to notify that audio input has ended and to stop expecting deltas.
    """
    id: str = Field(..., description="The ID of the audio input")

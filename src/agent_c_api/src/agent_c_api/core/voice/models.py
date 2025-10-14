from agents.voice import TTSVoice
from pydantic import Field

from agent_c.models.base import BaseModel

class AvailableVoiceModel(BaseModel):
    """
    Represents an available voice model for text-to-speech synthesis.
    """
    voice_id: str = Field(..., description="The vendor identifier for the voice model")
    vendor: str = Field(..., description="The vendor providing the voice model")
    description: str = Field("", description="A brief description of the voice model")
    output_format: str = Field(..., description="The audio output format produced by the voice model")

class OpenAIVoiceModel(AvailableVoiceModel):
    """
    Represents an OpenAI voice model for text-to-speech synthesis.
    """
    voice_id: TTSVoice = Field(..., description="The OpenAI voice name")
    vendor: str = Field("openai", description="The vendor providing the voice model, always 'openai'")
    output_format: str = Field("pcm16", description="The audio output format produced by the voice model, always 'pcm16'")

"""List of available OpenAI voice models for TTS"""
open_ai_voice_models = [OpenAIVoiceModel(voice_id="alloy"), OpenAIVoiceModel(voice_id="ash"),
                        OpenAIVoiceModel(voice_id="coral"),
                        OpenAIVoiceModel(voice_id="echo"), OpenAIVoiceModel(voice_id="fable"),
                        OpenAIVoiceModel(voice_id="nova"), OpenAIVoiceModel(voice_id="onyx"),
                        OpenAIVoiceModel(voice_id="sage"), OpenAIVoiceModel(voice_id="shimmer")]

"""Special "voice model" to indicate that the HeyGen streaming avatar SDK is handling the audio output."""
heygen_avatar_voice_model = AvailableVoiceModel(voice_id="avatar", vendor="heygen", description="HeyGen Avatar Voice Model", output_format="special")

"""Special "voice model" to indicate that no TTS should be used, text only."""
no_voice_model = AvailableVoiceModel(voice_id="none", vendor="system", description="No Voice (text only)", output_format="none")
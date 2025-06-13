from typing import Optional, List

from pydantic import Field

from agent_c.models.base import BaseModel
from agent_c.models.input import TextInput, FileInput, ImageInput, AudioInput, VideoInput

class InteractionInputs(BaseModel):
    """
    A model class for handling interaction inputs with various formats.
    This class can contain text, images, audio, files, and videos.
    """
    text: TextInput = Field(..., description="Text input for the interaction")
    files: Optional[List[FileInput]] = Field(default_factory=None, description="List of file inputs for the interaction")
    images: Optional[List[ImageInput]] = Field(default_factory=None, description="List of image inputs for the interaction")
    audio: Optional[List[AudioInput]] = Field(default_factory=None, description="List of audio inputs for the interaction")
    videos: Optional[List[VideoInput]] = Field(default_factory=None, description="List of video inputs for the interaction")

import io
import wave
import base64
import mimetypes
import numpy as np
import soundfile as sf


from typing import Union, Optional
from pathlib import Path
from pydantic import Field

from agent_c.models.input.file_input import FileInput
from agent_c.util.logging_utils import LoggingManager


class AudioInput(FileInput):
    """
    A model to handle audio input for completion calls

    Attributes:
        format (str): The type of audio content (e.g., 'wav').
        content (str): A base64-encoded string representing the audio data.
    """
    format: str = Field("wav")
    sample_rate: Optional[int] = Field(None, description="The sample rate of the audio content.")
    channels: Optional[int] = Field(None, description="The number of channels in the audio content.")
    transcript: Optional[str] = Field(None, description="The transcript of the audio content.")

    def __init__(self, **data):
        if 'content_type' not in data:
            data['content_type'] = 'audio/' + data.get('format', 'wav')

        super().__init__(**data)

        logging_manager = LoggingManager(__name__)
        self._logger = logging_manager.get_logger()


    def as_nd_array(self) -> np.ndarray:
        """
        Convert the audio content to a NumPy array

        Returns:
            bytes: The audio content as a NumPy array
        """
        wav_bytes = base64.b64decode(self.content)
        with io.BytesIO(wav_bytes) as wav_buffer:
            audio_array, sample_rate = sf.read(wav_buffer)
            # Whisper expects float32 mono audio
            if audio_array.ndim > 1:
                audio_array = audio_array.mean(axis=1)

            # Convert to float32 and normalize if needed
            audio_array = audio_array.astype(np.float32)

            # Normalize if not already in [-1, 1]
            if audio_array.max() > 1.0 or audio_array.min() < -1.0:
                audio_array = audio_array / 32768.0  # for 16-bit audio

            return audio_array


    @classmethod
    def from_bytes_as_wav(cls, raw_bytes: bytes, sample_width: int = 2, **data) -> "AudioInput":
        """Convert raw audio bytes to WAV format"""
        buffer = io.BytesIO()

        try:
            with wave.open(buffer, 'wb') as wav_file:
                wav_file.setnchannels(data['channels'])
                wav_file.setsampwidth(sample_width)  # 2 bytes for int16
                wav_file.setframerate(data['sample_rate'])
                wav_file.writeframes(raw_bytes)

            return cls.from_bytes(buffer.getvalue(), 'wav', **data)
        except Exception as e:
            logging_manager = LoggingManager(__name__)
            logger = logging_manager.get_logger()
            logger.error(f"Error converting audio to WAV: {str(e)}")
            raise
        finally:
            buffer.close()



    @classmethod
    def from_bytes(cls, audio_bytes: bytes, format_str: str = 'wav', **data) -> "AudioInput":
        """
        Create an AudioInput instance from audio bytes.

        Args:
            audio_bytes: The audio content as bytes
            format_str: The format of the audio content (e.g., 'wav')

        Returns:
            AudioInput: An instance with the audio content base64 encoded
        """
        content = base64.b64encode(audio_bytes).decode('utf-8')
        return cls(format=format_str, content=content, **data)

    @classmethod
    def from_file(cls, file_path: Union[str, Path]) -> "AudioInput":
        """
        Create an AudioInput instance from a WAV or MP3 file.

        Args:
            file_path: Path to the audio file (WAV or MP3)

        Returns:
            AudioInput: An instance with the file's content base64 encoded

        Raises:
            ValueError: If the file format is not supported or file doesn't exist
            IOError: If there are issues reading the file
        """
        path = Path(file_path)
        if not path.exists():
            raise ValueError(f"File not found: {file_path}")

        # Get the file format
        mime_type = mimetypes.guess_type(path)[0]
        if mime_type not in ('audio/wav', 'audio/x-wav', 'audio/mpeg', 'audio/mp3'):
            raise ValueError(f"Unsupported audio format: {mime_type}. Only WAV and MP3 are supported.")

        try:
            with path.open('rb') as f:
                content = base64.b64encode(f.read()).decode('utf-8')
        except IOError as e:
            logging_manager = LoggingManager(__name__)
            logger = logging_manager.get_logger()
            logger.error(f"Error reading audio file {file_path}: {str(e)}")
            raise


        format_str = 'wav' if 'wav' in mime_type.lower() else 'mp3'
        return cls(format=format_str, content=content, content_type=mime_type, file_name=path.name)


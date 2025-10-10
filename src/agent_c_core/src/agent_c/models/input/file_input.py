import base64
import mimetypes

from pathlib import Path
from pydantic import Field
from typing import Optional, Union

from agent_c.models.input.base import BaseInput
from agent_c.util.logging_utils import LoggingManager


class FileInput(BaseInput):
    """
    A class representing file input with support for various file formats.

    Attributes:
        content_type (str): The MIME type of the file content.
        url (Optional[str]): Optional URL where the file is hosted.
        content (Optional[str]): Optional base64 encoded content of the file.
        file_name (Optional[str]): Optional name of the file.
    """

    content_type: str = Field(..., alias="content-type")
    url: Optional[str] = None
    content: Optional[str] = Field(None, description="An optional that will contain raw base64 encoded content.")
    file_name: Optional[str] = None
    activity_hint: Optional[str] = Field(None, description="An optional hint to the system/assistant about the activity intended for the file.")
    id: Optional[str] = Field(None, description="An optional identifier for the file.")

    @classmethod
    def from_bytes(cls, content_type: str, content: bytes, **kwargs) -> 'FileInput':
        """
        Create a FileInput instance from bytes.

        Args:
            content_type: The MIME type of the file content.
            content: The file content as bytes.
            **kwargs: Additional keyword arguments to pass to the constructor.

        Returns:
            FileInput: An instance with the file content base64 encoded.
        """
        content = base64.b64encode(content).decode('utf-8')
        return cls(content_type=content_type, content=content, **kwargs)

    @classmethod
    def from_file(cls, file_path: Union[str, Path]) -> 'FileInput':
        """
        Create a FileInput instance from a file path.

        Args:
            file_path: Path to the file, either as string or Path object.

        Returns:
            FileInput: An instance with the file content loaded and encoded.

        Raises:
            ValueError: If the file does not exist.
            IOError: If there are issues reading the file.
        """
        path: Path = Path(file_path)
        if not path.exists():
            raise ValueError(f"File not found: {file_path}")

        mime_type: Optional[str] = mimetypes.guess_type(path)[0]
        try:
            with path.open('rb') as f:
                content: str = base64.b64encode(f.read()).decode('utf-8')
        except IOError as e:
            logging_manager = LoggingManager(__name__)
            logger = logging_manager.get_logger()
            logger.exception(f"Error reading input file {file_path}: {str(e)}", exc_info=True)
            raise

        return cls(content_type=mime_type, content=content, file_name=path.name)

    @property
    def data_url(self) -> str:
        """
        Converts the base64-encoded content into a data URL.

        Returns:
            str: The data URL of the file, which can be directly used in HTML.
        """
        return f"data:{self.content_type};base64,{self.content}"

    def get_text_content(self) -> Optional[str]:
        """
        Get text content extracted from the file, if possible.

        Returns:
            Optional[str]: Extracted text content, or None if not available
        """
        return None


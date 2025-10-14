import mimetypes
import shutil

from datetime import datetime, timedelta
from pathlib import Path
from typing import Dict, List, Optional, Union, Literal, TYPE_CHECKING

from fastapi import UploadFile, HTTPException
from pydantic import BaseModel, Field

from markitdown import MarkItDown

# Import existing models from agent_c
from agent_c.models.input.file_input import FileInput
from agent_c.models.input.image_input import ImageInput
from agent_c.models.input.audio_input import AudioInput
from agent_c_api.core.util.logging_utils import LoggingManager


if TYPE_CHECKING:
    from .realtime_bridge import RealtimeBridge


# Optional document processing libraries
try:
    import docx2txt

    DOCX_AVAILABLE = True
except ImportError:
    DOCX_AVAILABLE = False

try:
    import pandas as pd

    PANDAS_AVAILABLE = True
except ImportError:
    PANDAS_AVAILABLE = False

try:
    from pptx import Presentation

    PPTX_AVAILABLE = True
except ImportError:
    PPTX_AVAILABLE = False


class FileMetadata(BaseModel):
    """Metadata about an uploaded file"""
    id: str
    filename: str
    original_filename: str
    mime_type: str
    size: int
    upload_time: datetime
    session_id: str
    extracted_text: Optional[str] = None
    processed: bool = False
    processing_error: Optional[str] = None
    processing_status: Optional[Literal["pending", "failed", "completed"]] = "pending"


class FileHandler:
    """
    Utility class for handling file operations in the chat application.

    This class manages file uploads, storage, and retrieval, as well as
    text extraction from supported document types.
    """

    def __init__(self,
                 base_dir: Union[str, Path] = "uploads",
                 retention_days: int = 7):
        """
        Initialize the FileHandler.

        Args:
            base_dir: Base directory for file storage
            retention_days: Number of days to retain files
            logger: Optional logger instance
        """
        self.base_dir = Path(base_dir)
        self.base_dir.mkdir(parents=True, exist_ok=True)
        self.retention_days = retention_days
        logging_manager = LoggingManager(__name__)
        self.logger = logging_manager.get_logger()

        # Cache of session files for quick lookup
        self.session_files: Dict[str, List[FileMetadata]] = {}

    async def save_file(self, file: UploadFile, session_id: str) -> FileMetadata:
        """
        Save an uploaded file and associate it with a session.

        Args:
            file: The uploaded file
            session_id: ID of the session to associate with

        Returns:
            FileMetadata: Metadata about the saved file
        """
        # Create session directory
        session_dir = self.base_dir / session_id
        session_dir.mkdir(parents=True, exist_ok=True)

        # Generate a unique filename
        original_filename = file.filename or "upload.bin"
        file_id = f"{datetime.now().strftime('%Y%m%d%H%M%S')}_{original_filename}"
        file_path = session_dir / file_id

        # Save the file
        try:
            content = await file.read()
            with open(file_path, "wb") as f:
                f.write(content)

            # Determine MIME type
            mime_type, _ = mimetypes.guess_type(original_filename)
            mime_type = mime_type or "application/octet-stream"

            # Create metadata
            metadata = FileMetadata(
                id=file_id,
                filename=str(file_path),
                original_filename=original_filename,
                mime_type=mime_type,
                size=file_path.stat().st_size,
                upload_time=datetime.now(),
                session_id=session_id,
                processed=False
            )

            # Cache the metadata
            if session_id not in self.session_files:
                self.session_files[session_id] = []
            self.session_files[session_id].append(metadata)

            self.logger.info(f"Saved file {original_filename} for session {session_id}")
            return metadata

        except Exception as e:
            self.logger.error(f"Error saving file: {str(e)}")
            if file_path.exists():
                file_path.unlink()
            raise HTTPException(status_code=500, detail=f"Error saving file: {str(e)}")

    async def process_file(self, file_id: str, session_id: str) -> Optional[FileMetadata]:
        """
        Process a file to extract text if possible.

        Args:
            file_id: ID of the file to process
            session_id: Session ID

        Returns:
            Optional[FileMetadata]: Updated metadata or None if file not found
        """
        # Find the file metadata
        metadata = self.get_file_metadata(file_id, session_id)
        if not metadata:
            return None

        if isinstance(metadata, dict):
            metadata = FileMetadata.model_validate(metadata)

        if metadata.processed:
            return metadata

        try:
            # Check for supported document types to use MarkItDown
            office_doc_types = [
                "application/vnd.openxmlformats-officedocument.wordprocessingml.document",  # docx
                "application/msword",  # doc
                "application/vnd.openxmlformats-officedocument.presentationml.presentation",  # pptx
                "application/vnd.ms-powerpoint",  # ppt
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",  # xlsx
                "application/vnd.ms-excel",  # xls
                "text/html",  # html
                "application/pdf"  # pdf
            ]

            # Use MarkItDown for office documents
            if metadata.mime_type in office_doc_types:
                try:
                    md = MarkItDown()
                    result = md.convert(metadata.filename)
                    metadata.extracted_text = result.text_content
                    self.logger.info(f"Successfully processed {metadata.original_filename} using MarkItDown")
                except Exception as e:
                    self.logger.error(f"Error using MarkItDown to process {metadata.original_filename}: {str(e)}")
                    metadata.processing_error = f"[Error processing {metadata.original_filename}: {str(e)}]"
                    metadata.processed = False
                    metadata.processing_status = "failed"
                    metadata.extracted_text = None

            # Handle text files (keeping the existing logic)
            elif metadata.mime_type.startswith("text/"):
                try:
                    with open(metadata.filename, 'r', encoding='utf-8') as f:
                        metadata.extracted_text = f.read()
                except UnicodeDecodeError:
                    with open(metadata.filename, 'r', encoding='latin-1') as f:
                        metadata.extracted_text = f.read()

            metadata.processing_status = "completed"
            metadata.processed = True
            self.logger.info(f"Processed file {metadata.original_filename}")
            return metadata

        except Exception as e:
            self.logger.error(f"Error processing file: {str(e)}")
            metadata.processing_error = f"[Error processing {metadata.original_filename}: {str(e)}]"
            metadata.processed = False
            metadata.processing_status = "failed"
            metadata.extracted_text = None

            return metadata

    def get_file_metadata(self, file_id: str, session_id: str) -> Optional[FileMetadata]:
        """
        Get metadata for a specific file.

        Args:
            file_id: ID of the file
            session_id: Session ID

        Returns:
            Optional[FileMetadata]: File metadata or None if not found
        """
        if session_id not in self.session_files:
            return None

        for metadata in self.session_files[session_id]:
            if metadata.id == file_id:
                return metadata

        return None

    def get_session_files(self, session_id: str) -> List[FileMetadata]:
        """
        Get all files for a session.

        Args:
            session_id: Session ID

        Returns:
            List[FileMetadata]: List of file metadata
        """
        return self.session_files.get(session_id, [])

    def get_file_as_input(self, file_id: str, session_id: str) -> Optional[Union[FileInput, ImageInput, AudioInput]]:
        """
        Convert a file to the appropriate agent input type based on its MIME type.

        This method leverages the specialized input classes from agent_c:
        - ImageInput for images
        - AudioInput for audio files
        - FileInput for all other file types

        Extracted text (if available) is attached to FileInput objects.

        Args:
            file_id: ID of the file
            session_id: Session ID

        Returns:
            Optional[Union[FileInput, ImageInput, AudioInput]]:
                The appropriate input object for the file type, or None if conversion fails
        """
        metadata = self.get_file_metadata(file_id, session_id)
        if not metadata:
            self.logger.warning(f"No metadata found for file {file_id}")
            return None

        if isinstance(metadata, dict):
            metadata = FileMetadata.model_validate(metadata)

        file_path = metadata.filename

        try:
            if metadata.mime_type.startswith("image/"):
                # For images, create an ImageInput object
                self.logger.info(f"Creating ImageInput for {metadata.original_filename}")
                return ImageInput.from_file(file_path)

            elif metadata.mime_type.startswith("audio/"):
                # For audio files, create an AudioInput object
                self.logger.info(f"Creating AudioInput for {metadata.original_filename}")
                return AudioInput.from_file(file_path)

            else:
                # For all other files, create a FileInput object
                self.logger.info(f"Creating FileInput for {metadata.original_filename}")
                file_input = FileInput.from_file(file_path)

                # Attach the extracted text if available
                if metadata.extracted_text:
                    # Create a subclass of FileInput that overrides get_text_content
                    class DocumentFileInput(FileInput):
                        def get_text_content(self) -> Optional[str]:
                            return metadata.extracted_text

                    # Copy attributes from file_input to the new instance
                    doc_input = DocumentFileInput(
                        content_type=file_input.content_type,
                        content=file_input.content,
                        file_name=metadata.original_filename,
                        url=getattr(file_input, 'url', None)
                    )

                    self.logger.info(f"Attached extracted text ({len(metadata.extracted_text)} chars) to FileInput")
                    return doc_input

                return file_input

        except Exception as e:
            self.logger.error(f"Error creating input object for {metadata.original_filename}: {str(e)}", exc_info=True)
            return None

    def cleanup_session(self, session_id: str) -> int:
        """
        Delete all files for a session.

        Args:
            session_id: Session ID

        Returns:
            int: Number of files deleted
        """
        if session_id not in self.session_files:
            return 0

        count = 0
        session_dir = self.base_dir / session_id

        try:
            if session_dir.exists():
                shutil.rmtree(session_dir)
                count = len(self.session_files[session_id])
                del self.session_files[session_id]
                self.logger.info(f"Deleted {count} files for session {session_id}")
        except Exception as e:
            self.logger.error(f"Error cleaning up session: {str(e)}")

        return count

    def cleanup_expired_files(self) -> int:
        """
        Delete files older than the retention period.

        Returns:
            int: Number of files deleted
        """
        cutoff = datetime.now() - timedelta(days=self.retention_days)
        count = 0

        for session_id in list(self.session_files.keys()):
            # Files to keep and delete
            keep_files = []
            delete_files = []

            for metadata in self.session_files[session_id]:
                if metadata.upload_time < cutoff:
                    delete_files.append(metadata)
                else:
                    keep_files.append(metadata)

            # Delete individual files
            for metadata in delete_files:
                try:
                    path = Path(metadata.filename)
                    if path.exists():
                        path.unlink()
                        count += 1
                except Exception as e:
                    self.logger.error(f"Error deleting expired file: {str(e)}")

            # Update metadata cache
            if not keep_files:
                # If no files left, clean up the directory
                session_dir = self.base_dir / session_id
                try:
                    if session_dir.exists():
                        shutil.rmtree(session_dir)
                except Exception as e:
                    self.logger.error(f"Error removing session directory: {str(e)}")

                del self.session_files[session_id]
            else:
                self.session_files[session_id] = keep_files

        if count > 0:
            self.logger.info(f"Cleaned up {count} expired files")

        return count


class RTFileHandler(FileHandler):
    def __init__(self,
                 bridge: 'RealtimeBridge',
                 user_id: str,
                 retention_days: int = 7):
        base_dir = Path(f"uploads/{user_id}")
        super().__init__(base_dir=base_dir, retention_days=retention_days)

        self.bridge = bridge

    async def save_file(self, file: UploadFile, _: str) -> FileMetadata:
        chat_session = self.bridge.chat_session
        file_data =  await super().save_file(file, chat_session.session_id)
        session_files = chat_session.metadata.get("uploaded_files", {})
        session_files[file_data.id] = file_data.model_dump(mode='json')
        chat_session.metadata["uploaded_files"] = session_files
        await self.bridge.send_chat_session_meta()
        return file_data

    async def process_file(self, file_id: str, _: str) -> Optional[FileMetadata]:
        chat_session = self.bridge.chat_session
        file_data = await super().process_file(file_id, chat_session.session_id)
        session_files = chat_session.metadata.get("uploaded_files", {})
        session_files[file_data.id] = file_data.model_dump(mode='json')
        chat_session.metadata["uploaded_files"] = session_files
        await self.bridge.send_chat_session_meta()
        return file_data

    def get_file_metadata(self, file_id: str, _: str) -> Optional[FileMetadata]:
        metadata = self.bridge.chat_session.metadata.get("uploaded_files", {}).get(file_id)
        if isinstance(metadata, dict):
            metadata = FileMetadata.model_validate(metadata)

        return metadata

    def get_session_files(self, _: str) -> List[FileMetadata]:
        files = self.bridge.chat_session.metadata.get("uploaded_files", {})
        return [FileMetadata.model_validate(f) for f in files.values()]

    def get_file_as_input(self, file_id: str, _: str) -> Optional[Union[FileInput, ImageInput, AudioInput]]:
        return super().get_file_as_input(file_id, self.bridge.chat_session.session_id)

    def cleanup_session(self, _: str) -> int:
        chat_session = self.bridge.chat_session
        count = super().cleanup_session(chat_session.session_id)
        chat_session.metadata["uploaded_files"] = {}
        return count
# tests/unit/api/v2/sessions/test_files.py
"""
Tests for the file endpoints in the sessions module.

These tests cover the API endpoints for file operations within a session, including:
- Uploading files
- Listing files in a session
- Getting file metadata
- Downloading file content
- Deleting files

All tests use mocks to avoid actual file operations and focus on the API behavior.

IMPORTANT: This test file includes special handling for async operations and background tasks
to avoid 'coroutine was never awaited' warnings. The AsyncBackgroundTasks class provides
a way to properly execute and await background tasks during testing.
"""

import pytest
import asyncio
from unittest.mock import AsyncMock, MagicMock, patch
from fastapi import UploadFile, HTTPException, BackgroundTasks
from fastapi.testclient import TestClient
from datetime import datetime
from pathlib import Path


# Helper class for async testing of background tasks
class AsyncBackgroundTasks:
    """A test implementation of BackgroundTasks that allows testing async background tasks.
    
    IMPORTANT: This class helps prevent 'coroutine was never awaited' warnings by providing
    a way to execute and await all background tasks during testing. Without this, AsyncMock
    objects used in background tasks would generate warnings because their coroutines are
    never awaited.
    
    This class mimics FastAPI's BackgroundTasks but adds the ability to execute
    all scheduled tasks during tests, ensuring that async tasks are properly awaited.
    """
    def __init__(self):
        self.tasks = []
    
    def add_task(self, func, *args, **kwargs):
        """Add a task to be executed in the background.
        
        Args:
            func: The function to be executed
            *args: Arguments to pass to the function
            **kwargs: Keyword arguments to pass to the function
        """
        self.tasks.append((func, args, kwargs))
    
    async def execute_all(self):
        """Execute all scheduled tasks.
        
        This method should be called in tests to ensure that all async background
        tasks are properly awaited.
        """
        for func, args, kwargs in self.tasks:
            if asyncio.iscoroutinefunction(func):
                await func(*args, **kwargs)
            else:
                func(*args, **kwargs)

from agent_c_api.main import app
from agent_c_api.api.v2.models.file_models import FileMeta, FileUploadResponse
from agent_c_api.api.v2.sessions.services import SessionService
from agent_c_api.api.v2.sessions.files import (
    upload_file, list_files, get_file_metadata, download_file, delete_file, get_session_service
)


# Test client
client = TestClient(app)


@pytest.fixture
def mock_file_metadata():
    """Create mock file metadata for testing.
    
    Returns:
        MagicMock: A mock object representing file metadata with all required fields.
    """
    return MagicMock(
        id="test-file-id",
        filename=str(Path("uploads/test-session/test-file.txt")),
        original_filename="test-file.txt",
        mime_type="text/plain",
        size=100,
        upload_time=datetime.now(),
        processed=True,
        processing_status="complete",
        processing_error=None
    )


@pytest.fixture
def mock_session_service():
    """Create a mock session service with a valid session.
    
    Returns:
        tuple: A tuple containing (mock_service, session_id) where session_id is a valid
              session ID string in MnemonicSlug format.
    """
    service = MagicMock(spec=SessionService)
    # Use a MnemonicSlug format instead of UUID
    session_id = "tiger-castle"
    
    # Configure get_session to return a session for our test ID
    def mock_get_session(session_id_param):
        if session_id_param == session_id:
            return {"id": session_id_param, "name": "Test Session"}
        return None
    
    service.get_session.side_effect = mock_get_session
    service.agent_manager = MagicMock()
    service.agent_manager.get_session_data.return_value = {"agent": MagicMock()}
    
    return service, session_id


@pytest.fixture
def mock_file_handler(mock_file_metadata):
    """Create a mock file handler for file operations.
    
    Args:
        mock_file_metadata: The mock file metadata fixture
        
    Returns:
        MagicMock: A mock object for the file handler with all methods configured.
    """
    with patch("agent_c_api.api.v2.sessions.files.file_handler") as mock_handler:
        # For async methods, we need to create proper async side_effect functions
        # that will return values (or raise exceptions) when awaited
        
        # Define async functions for methods that need to be awaited
        async def async_save_file(*args, **kwargs):
            return mock_file_metadata
        
        async def async_process_file(*args, **kwargs):
            return mock_file_metadata
            
        # Configure the mock with our async functions
        mock_handler.save_file = AsyncMock(side_effect=async_save_file)
        mock_handler.process_file = AsyncMock(side_effect=async_process_file)
        
        # Non-async methods can use regular return values
        mock_handler.get_session_files.return_value = [mock_file_metadata]
        mock_handler.get_file_metadata.return_value = mock_file_metadata
        mock_handler.session_files = {"tiger-castle": [mock_file_metadata]}
        yield mock_handler


@pytest.mark.unit
@pytest.mark.session
@pytest.mark.files
class TestUploadFile:
    """Tests for the upload_file endpoint.
    
    This class contains tests for uploading files to a session, including successful uploads
    and various error cases.
    """
    
    @pytest.mark.asyncio
    async def test_upload_file_success(self, mock_session_service, mock_file_handler, mock_file_metadata):
        """Test successful file upload to a session.
        
        Verifies that:
        1. The upload_file endpoint returns a FileUploadResponse
        2. The response contains the correct file information
        3. Background tasks are added for processing
        4. The file handler save_file method is called correctly
        """
        service, session_id = mock_session_service
        
        # Create a mock upload file
        mock_upload_file = MagicMock(spec=UploadFile)
        mock_upload_file.filename = "test-file.txt"
        
        # Create mock background tasks for testing async operations
        mock_background_tasks = AsyncBackgroundTasks()
        
        # Call the endpoint
        response = await upload_file(
            session_id=session_id,
            file=mock_upload_file,
            background_tasks=mock_background_tasks,
            session_service=service
        )
        
        # Execute background tasks to ensure any async operations complete
        await mock_background_tasks.execute_all()
        
        # Ensure background tasks are properly awaited
        # This is important because background_tasks.add_task is called with mock_file_handler.process_file
        # which is an AsyncMock
        
        # Verify response
        assert isinstance(response, FileUploadResponse)
        assert response.file_id == mock_file_metadata.id
        assert response.filename == mock_file_metadata.original_filename
        assert response.content_type == mock_file_metadata.mime_type
        assert response.size == mock_file_metadata.size
        
        # Verify background tasks were added
        assert len(mock_background_tasks.tasks) == 2
        
        # Verify file handler methods were called
        mock_file_handler.save_file.assert_awaited_once_with(
            mock_upload_file, session_id
        )
        
        # Execute background tasks to ensure any async operations complete
        await mock_background_tasks.execute_all()

    @pytest.mark.problematic
    @pytest.mark.asyncio
    async def test_upload_file_session_not_found(self, mock_session_service, mock_file_handler):
        """Test file upload with a non-existent session ID.
        
        Verifies that:
        1. The endpoint raises a 404 HTTPException when the session doesn't exist
        2. The exception detail contains the appropriate error message
        """
        service, _ = mock_session_service
        invalid_session_id = "nonexistent-session"
        
        # Ensure get_session returns None for the invalid session ID
        # This is critical for the 404 error to be triggered
        service.get_session.side_effect = lambda sid: None if sid == invalid_session_id else {"id": sid, "name": "Test Session"}
        
        # Create a mock upload file
        mock_upload_file = MagicMock(spec=UploadFile)
        mock_upload_file.filename = "test-file.txt"
        
        # Create mock background tasks for testing async operations
        mock_background_tasks = AsyncBackgroundTasks()
        
        # Test with invalid session ID
        with pytest.raises(HTTPException) as exc_info:
            await upload_file(
                session_id=invalid_session_id,
                file=mock_upload_file,
                background_tasks=mock_background_tasks,
                session_service=service
            )
            
            # Note: We don't need to await background_tasks.execute_all() here
            # because the exception is raised before any background tasks are added
        
        assert exc_info.value.status_code == 404
        assert "Session not found" in exc_info.value.detail

    @pytest.mark.asyncio
    async def test_upload_file_server_error(self, mock_session_service, mock_file_handler):
        """Test file upload with a server error during processing.
        
        Verifies that:
        1. Server errors during file processing are caught and re-raised as HTTPExceptions
        2. The exception contains the original error message
        """
        service, session_id = mock_session_service
        
        # Create a mock upload file
        mock_upload_file = MagicMock(spec=UploadFile)
        mock_upload_file.filename = "test-file.txt"
        
        # Create mock background tasks for testing async operations
        mock_background_tasks = AsyncBackgroundTasks()
        
        # Configure file handler to raise an exception asynchronously
        async def async_error(*args, **kwargs):
            raise Exception("Storage error")
            
        mock_file_handler.save_file = AsyncMock(side_effect=async_error)
        
        # Test with server error
        with pytest.raises(HTTPException) as exc_info:
            await upload_file(
                session_id=session_id,
                file=mock_upload_file,
                background_tasks=mock_background_tasks,
                session_service=service
            )
        
        assert exc_info.value.status_code == 500
        assert "Error uploading file: Storage error" in exc_info.value.detail

    @pytest.mark.asyncio
    async def test_upload_file_model_validation(self, mock_session_service, mock_file_handler, mock_file_metadata):
        """Test that the upload response model is correctly structured.
        
        Verifies that the FileUploadResponse model:
        1. Contains all required fields
        2. Has fields with the correct types
        3. Has fields with the correct values
        """
        service, session_id = mock_session_service
        
        # Create a mock upload file
        mock_upload_file = MagicMock(spec=UploadFile)
        mock_upload_file.filename = "test-file.txt"
        
        # Create mock background tasks for testing async operations
        mock_background_tasks = AsyncBackgroundTasks()
        
        # Call the endpoint
        response = await upload_file(
            session_id=session_id,
            file=mock_upload_file,
            background_tasks=mock_background_tasks,
            session_service=service
        )
        
        # Execute background tasks to ensure any async operations complete
        await mock_background_tasks.execute_all()
        
        # Ensure background tasks are properly awaited
        # This is important because background_tasks.add_task is called with mock_file_handler.process_file
        # which is an AsyncMock
        
        # Verify response structure
        assert isinstance(response, FileUploadResponse)
        assert hasattr(response, "file_id")
        assert hasattr(response, "filename")
        assert hasattr(response, "content_type")
        assert hasattr(response, "size")
        
        # Verify field types
        assert isinstance(response.file_id, str)
        assert isinstance(response.filename, str)
        assert isinstance(response.content_type, str)
        assert isinstance(response.size, int)
        
        # Execute background tasks to ensure any async operations complete
        await mock_background_tasks.execute_all()


@pytest.mark.unit
@pytest.mark.session
@pytest.mark.files
class TestListFiles:
    """Tests for the list_files endpoint.
    
    This class contains tests for listing files in a session, including successful listing
    and various error cases.
    """
    
    @pytest.mark.asyncio
    async def test_list_files_success(self, mock_session_service, mock_file_handler, mock_file_metadata):
        """Test successful listing of files in a session.
        
        Verifies that:
        1. The list_files endpoint returns a list of FileMeta objects
        2. The response contains the correct file information
        3. The file handler get_session_files method is called correctly
        """
        service, session_id = mock_session_service
        
        # Call the endpoint
        response = await list_files(
            session_id=session_id,
            session_service=service
        )
        
        # Verify response
        assert isinstance(response, list)
        assert len(response) == 1
        assert isinstance(response[0], FileMeta)
        assert response[0].id == mock_file_metadata.id
        assert response[0].filename == mock_file_metadata.original_filename
        assert response[0].content_type == mock_file_metadata.mime_type
        assert response[0].size == mock_file_metadata.size
        assert "processed" in response[0].metadata
        
        # Verify file handler methods were called
        mock_file_handler.get_session_files.assert_called_once_with(session_id)

    @pytest.mark.problematic
    @pytest.mark.asyncio
    async def test_list_files_session_not_found(self, mock_session_service, mock_file_handler):
        """Test file listing with a non-existent session ID.
        
        Verifies that:
        1. The endpoint raises a 404 HTTPException when the session doesn't exist
        2. The exception detail contains the appropriate error message
        """
        service, _ = mock_session_service
        invalid_session_id = "nonexistent-session"
        
        # Ensure get_session returns None for the invalid session ID
        # This is critical for the 404 error to be triggered
        service.get_session.side_effect = lambda sid: None if sid == invalid_session_id else {"id": sid, "name": "Test Session"}
        
        # Test with invalid session ID
        with pytest.raises(HTTPException) as exc_info:
            await list_files(
                session_id=invalid_session_id,
                session_service=service
            )
        
        assert exc_info.value.status_code == 404
        assert "Session not found" in exc_info.value.detail

    @pytest.mark.asyncio
    async def test_list_files_empty(self, mock_session_service, mock_file_handler):
        """Test listing files when the session has no files.
        
        Verifies that:
        1. An empty list is returned when there are no files
        2. The response is properly structured
        """
        service, session_id = mock_session_service
        
        # Configure file handler to return empty list
        mock_file_handler.get_session_files.return_value = []
        
        # Call the endpoint
        response = await list_files(
            session_id=session_id,
            session_service=service
        )
        
        # Verify empty list is returned
        assert isinstance(response, list)
        assert len(response) == 0
        
        # Verify file handler methods were called
        mock_file_handler.get_session_files.assert_called_once_with(session_id)

    @pytest.mark.asyncio
    async def test_list_files_server_error(self, mock_session_service, mock_file_handler):
        """Test file listing with a server error.
        
        Verifies that:
        1. Server errors are caught and re-raised as HTTPExceptions
        2. The exception contains the original error message
        """
        service, session_id = mock_session_service
        
        # Configure file handler to raise an exception
        mock_file_handler.get_session_files.side_effect = Exception("Database error")
        
        # Test with server error
        with pytest.raises(HTTPException) as exc_info:
            await list_files(
                session_id=session_id,
                session_service=service
            )
        
        assert exc_info.value.status_code == 500
        assert "Error listing files: Database error" in exc_info.value.detail


@pytest.mark.unit
@pytest.mark.session
@pytest.mark.files
class TestFileMetadata:
    """Tests for the get_file_metadata endpoint.
    
    This class contains tests for retrieving metadata for a specific file,
    including successful retrieval and various error cases.
    """
    
    @pytest.mark.asyncio
    async def test_get_file_metadata_success(self, mock_session_service, mock_file_handler, mock_file_metadata):
        """Test successful retrieval of file metadata.
        
        Verifies that:
        1. The get_file_metadata endpoint returns a FileMeta object
        2. The response contains the correct file information
        3. The file handler get_file_metadata method is called correctly
        """
        service, session_id = mock_session_service
        file_id = "test-file-id"
        
        # Call the endpoint
        response = await get_file_metadata(
            session_id=session_id,
            file_id=file_id,
            session_service=service
        )
        
        # Verify response
        assert isinstance(response, FileMeta)
        assert response.id == mock_file_metadata.id
        assert response.filename == mock_file_metadata.original_filename
        assert response.content_type == mock_file_metadata.mime_type
        assert response.size == mock_file_metadata.size
        assert "processed" in response.metadata
        
        # Verify file handler methods were called
        mock_file_handler.get_file_metadata.assert_called_once_with(file_id, session_id)

    @pytest.mark.problematic
    @pytest.mark.asyncio
    async def test_get_file_metadata_session_not_found(self, mock_session_service, mock_file_handler):
        """Test file metadata retrieval with a non-existent session ID.
        
        Verifies that:
        1. The endpoint raises a 404 HTTPException when the session doesn't exist
        2. The exception detail contains the appropriate error message
        """
        service, _ = mock_session_service
        invalid_session_id = "nonexistent-session"
        file_id = "test-file-id"
        
        # Ensure get_session returns None for the invalid session ID
        # This is critical for the 404 error to be triggered
        service.get_session.side_effect = lambda sid: None if sid == invalid_session_id else {"id": sid, "name": "Test Session"}
        
        # Test with invalid session ID
        with pytest.raises(HTTPException) as exc_info:
            await get_file_metadata(
                session_id=invalid_session_id,
                file_id=file_id,
                session_service=service
            )
        
        assert exc_info.value.status_code == 404
        assert "Session not found" in exc_info.value.detail

    @pytest.mark.asyncio
    async def test_get_file_metadata_file_not_found(self, mock_session_service, mock_file_handler):
        """Test file metadata retrieval with a non-existent file ID.
        
        Verifies that:
        1. The endpoint raises a 404 HTTPException when the file doesn't exist
        2. The exception detail contains the appropriate error message with the file ID
        """
        service, session_id = mock_session_service
        file_id = "nonexistent-file-id"
        
        # Configure file handler to return None for this file ID
        mock_file_handler.get_file_metadata.return_value = None
        
        # Test with invalid file ID
        with pytest.raises(HTTPException) as exc_info:
            await get_file_metadata(
                session_id=session_id,
                file_id=file_id,
                session_service=service
            )
        
        assert exc_info.value.status_code == 404
        assert f"File {file_id} not found" in exc_info.value.detail

    @pytest.mark.asyncio
    async def test_get_file_metadata_model_validation(self, mock_session_service, mock_file_handler, mock_file_metadata):
        """Test that the file metadata response model is correctly structured.
        
        Verifies that the FileMeta model:
        1. Contains all required fields
        2. Has fields with the correct types
        3. Has a well-formed metadata dictionary
        """
        service, session_id = mock_session_service
        file_id = "test-file-id"
        
        # Call the endpoint
        response = await get_file_metadata(
            session_id=session_id,
            file_id=file_id,
            session_service=service
        )
        
        # Verify model structure
        assert isinstance(response, FileMeta)
        assert hasattr(response, "id")
        assert hasattr(response, "filename")
        assert hasattr(response, "content_type")
        assert hasattr(response, "size")
        assert hasattr(response, "uploaded_at")
        assert hasattr(response, "session_id")
        assert hasattr(response, "metadata")
        
        # Verify field types
        assert isinstance(response.id, str)
        assert isinstance(response.filename, str)
        assert isinstance(response.content_type, str)
        assert isinstance(response.size, int)
        assert isinstance(response.uploaded_at, datetime)
        assert isinstance(response.session_id, str)  # We're expecting string ID, not UUID
        assert isinstance(response.metadata, dict)
        
        # Verify metadata structure
        assert "processed" in response.metadata
        assert "processing_status" in response.metadata
        assert response.metadata["processed"] is True
        assert response.metadata["processing_status"] == "complete"


@pytest.mark.unit
@pytest.mark.session
@pytest.mark.files
class TestDownloadFile:
    """Tests for the download_file endpoint.
    
    This class contains tests for downloading file content, including successful downloads
    and various error cases.
    """
    
    @pytest.mark.asyncio
    async def test_download_file_success(self, mock_session_service, mock_file_handler, mock_file_metadata):
        """Test successful file download.
        
        Verifies that:
        1. The download_file endpoint returns a FileResponse
        2. The FileResponse is created with the correct parameters
        3. The file handler get_file_metadata method is called correctly
        """
        service, session_id = mock_session_service
        file_id = "test-file-id"
        
        # Mock FileResponse
        with patch("agent_c_api.api.v2.sessions.files.FileResponse") as mock_file_response:
            mock_file_response.return_value = "file_response_object"
            
            # Call the endpoint
            response = await download_file(
                session_id=session_id,
                file_id=file_id,
                session_service=service
            )
            
            # Verify response
            assert response == "file_response_object"
            
            # Ensure any async operations would be completed
            # In a real application, FastAPI would handle awaiting any async operations
            
            # Verify FileResponse was called correctly
            mock_file_response.assert_called_once_with(
                path=mock_file_metadata.filename,
                filename=mock_file_metadata.original_filename,
                media_type=mock_file_metadata.mime_type
            )
            
            # Verify file handler methods were called
            mock_file_handler.get_file_metadata.assert_called_once_with(file_id, session_id)

    @pytest.mark.asyncio
    async def test_download_file_file_not_found(self, mock_session_service, mock_file_handler):
        """Test file download with a non-existent file ID.
        
        Verifies that:
        1. The endpoint raises a 404 HTTPException when the file doesn't exist
        2. The exception detail contains the appropriate error message with the file ID
        """
        service, session_id = mock_session_service
        file_id = "nonexistent-file-id"
        
        # Configure file handler to return None for this file ID
        mock_file_handler.get_file_metadata.return_value = None
        
        # Test with invalid file ID
        with pytest.raises(HTTPException) as exc_info:
            await download_file(
                session_id=session_id,
                file_id=file_id,
                session_service=service
            )
        
        assert exc_info.value.status_code == 404
        assert f"File {file_id} not found" in exc_info.value.detail

    @pytest.mark.asyncio
    async def test_download_file_server_error(self, mock_session_service, mock_file_handler, mock_file_metadata):
        """Test file download with a server error.
        
        Verifies that:
        1. Server errors are caught and re-raised as HTTPExceptions
        2. The exception contains the original error message
        """
        service, session_id = mock_session_service
        file_id = "test-file-id"
        
        # Mock FileResponse to raise an exception
        with patch("agent_c_api.api.v2.sessions.files.FileResponse") as mock_file_response:
            mock_file_response.side_effect = Exception("File system error")
            
            # Test with server error
            with pytest.raises(HTTPException) as exc_info:
                await download_file(
                    session_id=session_id,
                    file_id=file_id,
                    session_service=service
                )
            
            assert exc_info.value.status_code == 500
            assert "Error downloading file: File system error" in exc_info.value.detail


@pytest.mark.unit
@pytest.mark.session
@pytest.mark.files
class TestDeleteFile:
    """Tests for the delete_file endpoint.
    
    This class contains tests for deleting files, including successful deletions
    and various error cases.
    """
    
    @pytest.mark.asyncio
    async def test_delete_file_success(self, mock_session_service, mock_file_handler, mock_file_metadata):
        """Test successful file deletion.
        
        Verifies that:
        1. The delete_file endpoint returns None (204 No Content)
        2. os.remove is called with the correct file path
        3. The file handler get_file_metadata method is called correctly
        """
        service, session_id = mock_session_service
        file_id = "test-file-id"
        
        # Mock os.remove
        with patch("os.remove") as mock_remove:
            # Call the endpoint
            response = await delete_file(
                session_id=session_id,
                file_id=file_id,
                session_service=service
            )
            
            # Verify response is None (204 No Content)
            assert response is None
            
            # Verify os.remove was called
            mock_remove.assert_called_once_with(mock_file_metadata.filename)
            
            # Verify file handler methods were called
            mock_file_handler.get_file_metadata.assert_called_once_with(file_id, session_id)

    @pytest.mark.asyncio
    async def test_delete_file_file_not_found(self, mock_session_service, mock_file_handler):
        """Test file deletion with a non-existent file ID.
        
        Verifies that:
        1. The endpoint raises a 404 HTTPException when the file doesn't exist
        2. The exception detail contains the appropriate error message with the file ID
        """
        service, session_id = mock_session_service
        file_id = "nonexistent-file-id"
        
        # Configure file handler to return None for this file ID
        mock_file_handler.get_file_metadata.return_value = None
        
        # Test with invalid file ID
        with pytest.raises(HTTPException) as exc_info:
            await delete_file(
                session_id=session_id,
                file_id=file_id,
                session_service=service
            )
        
        assert exc_info.value.status_code == 404
        assert f"File {file_id} not found" in exc_info.value.detail

    @pytest.mark.asyncio
    async def test_delete_file_server_error(self, mock_session_service, mock_file_handler, mock_file_metadata):
        """Test file deletion with a server error during file removal.
        
        Verifies that:
        1. Server errors during file removal are caught and re-raised as HTTPExceptions
        2. The exception contains the original error message
        """
        service, session_id = mock_session_service
        file_id = "test-file-id"
        
        # Mock os.remove to raise an exception
        with patch("os.remove") as mock_remove:
            mock_remove.side_effect = Exception("Permission denied")
            
            # Test with server error
            with pytest.raises(HTTPException) as exc_info:
                await delete_file(
                    session_id=session_id,
                    file_id=file_id,
                    session_service=service
                )
            
            assert exc_info.value.status_code == 500
            assert "Error deleting file: Permission denied" in exc_info.value.detail
# src/agent_c_api/api/v2/sessions/files.py
import logging
from typing import List


from fastapi import APIRouter, Depends, HTTPException, File, UploadFile, BackgroundTasks, status, Path
from fastapi.responses import FileResponse
from fastapi_versioning import version

from agent_c_api.api.v2.models.file_models import FileMeta, FileUploadResponse
from .services import SessionService, get_session_service
from agent_c_api.core.file_handler import FileHandler


# Setup router
router = APIRouter(prefix="/sessions", tags=["sessions"])

# Initialize logger
logger = logging.getLogger(__name__)


# Create a shared file handler
file_handler = FileHandler(base_dir="uploads", retention_days=7)

# Background tasks
def cleanup_expired_files():
    """Background task to clean up expired files"""
    try:
        count = file_handler.cleanup_expired_files()
        if count > 0:
            logger.info(f"Cleaned up {count} expired files")
    except Exception as e:
        logger.error(f"Error cleaning up expired files: {str(e)}")

@router.post(
    "/{session_id}/files",
    response_model=FileUploadResponse,
    status_code=status.HTTP_201_CREATED,
    summary="Upload a file to a session",
    description="Upload a file to be used in chat messages within a specific session",
    responses={
        201: {
            "description": "File successfully uploaded",
            "content": {
                "application/json": {
                    "example": {
                        "file_id": "file_abc123",
                        "filename": "report.pdf",
                        "content_type": "application/pdf",
                        "size": 2097152
                    }
                }
            }
        },
        404: {
            "description": "Session not found",
            "content": {
                "application/json": {
                    "example": {"detail": "Session not found"}
                }
            }
        },
        500: {
            "description": "Server error processing the file",
            "content": {
                "application/json": {
                    "example": {"detail": "Error uploading file: Failed to process file"}
                }
            }
        }
    }
)
@version(2)
async def upload_file(
    session_id: str = Path(..., description="MnemonicSlug ID of the session to upload the file to (e.g., 'tiger-castle')"),
    file: UploadFile = File(..., description="The file to upload (multipart/form-data)"),
    background_tasks: BackgroundTasks = BackgroundTasks(),
    session_service: SessionService = Depends(get_session_service)
) -> FileUploadResponse:
    """Upload a file to a session
    
    This endpoint allows uploading a file to be used in a chat session. The file can
    later be referenced in chat messages by using its file_id. This enables multimodal
    interactions where users can share documents, images, and other files with the agent.
    
    Files are automatically processed in the background for text extraction (when applicable),
    which helps the agent understand and reference the file content.
    
    Args:
        session_id: The MnemonicSlug ID of the session (e.g., 'tiger-castle') to upload the file to
        file: The file to upload as multipart/form-data
        background_tasks: FastAPI background tasks for async processing
        session_service: Session service dependency injection
        
    Returns:
        FileUploadResponse: Information about the uploaded file including its ID
        
    Raises:
        HTTPException(404): If the specified session doesn't exist
        HTTPException(500): If there's an error uploading or processing the file
        
    Example:
        ```python
        import requests
        
        # Upload a file to a session
        with open('document.pdf', 'rb') as f:
            files = {'file': ('document.pdf', f, 'application/pdf')}
            response = requests.post(
                'https://api.example.com/api/v2/sessions/tiger-castle/files',
                files=files
            )
            
        # Get the file ID for future reference
        file_info = response.json()
        file_id = file_info['file_id']
        print(f"Uploaded {file_info['filename']} ({file_id})")
        ```
    """
    # Schedule expired files cleanup
    background_tasks.add_task(cleanup_expired_files)
    
    # Get session and verify it exists
    session = session_service.get_session(str(session_id))
    if not session:
        logger.error(f"No session found for session_id: {session_id}")
        raise HTTPException(status_code=404, detail="Session not found")
    
    try:
        # Save the file
        metadata = await file_handler.save_file(file, str(session_id))
        
        # Set the file handler on the agent if needed
        session_data = await session_service.agent_manager.get_user_session(str(session_id))
        agent_bridge = session_data.get("agent_bridge")
        if agent_bridge and not hasattr(agent_bridge, "file_handler"):
            agent_bridge.file_handler = file_handler
        elif agent_bridge and agent_bridge.file_handler is None:
            agent_bridge.file_handler = file_handler
        
        # Process the file in the background for text extraction
        background_tasks.add_task(file_handler.process_file, metadata.id, str(session_id))
        
        # Return response
        return FileUploadResponse(
            file_id=metadata.id,
            filename=metadata.original_filename,
            content_type=metadata.mime_type,
            size=metadata.size
        )
    except Exception as e:
        logger.error(f"Error uploading file: {str(e)}")
        raise HTTPException(status_code=500, detail=f"Error uploading file: {str(e)}")

@router.get(
    "/{session_id}/files",
    response_model=List[FileMeta],
    status_code=status.HTTP_200_OK,
    summary="List files in a session",
    description="Retrieve metadata for all files associated with a specific session",
    responses={
        200: {
            "description": "List of file metadata",
            "content": {
                "application/json": {
                    "example": [
                        {
                            "id": "file_abc123",
                            "filename": "document.pdf",
                            "content_type": "application/pdf",
                            "size": 1048576,
                            "uploaded_at": "2025-04-04T12:00:00Z",
                            "session_id": "tiger-castle",
                            "metadata": {
                                "processed": True,
                                "processing_status": "complete",
                                "page_count": 5
                            }
                        },
                        {
                            "id": "file_def456",
                            "filename": "screenshot.png",
                            "content_type": "image/png",
                            "size": 256000,
                            "uploaded_at": "2025-04-04T12:30:00Z",
                            "session_id": "tiger-castle",
                            "metadata": {
                                "processed": True,
                                "processing_status": "complete"
                            }
                        }
                    ]
                }
            }
        },
        404: {
            "description": "Session not found",
            "content": {
                "application/json": {
                    "example": {"detail": "Session not found"}
                }
            }
        },
        500: {
            "description": "Server error listing files",
            "content": {
                "application/json": {
                    "example": {"detail": "Error listing files: Internal server error"}
                }
            }
        }
    }
)
@version(2)
async def list_files(
    session_id: str = Path(..., description="MnemonicSlug ID of the session to list files for (e.g., 'tiger-castle')"),
    session_service: SessionService = Depends(get_session_service)
) -> List[FileMeta]:
    """List all files for a session
    
    This endpoint retrieves metadata for all files associated with a specific session.
    It provides information about each file, including its ID, name, size, upload time,
    and processing status.
    
    Args:
        session_id: The MnemonicSlug ID of the session (e.g., 'tiger-castle') to list files for
        session_service: Session service dependency injection
        
    Returns:
        List[FileMeta]: List of file metadata objects for all files in the session
        
    Raises:
        HTTPException(404): If the specified session doesn't exist
        HTTPException(500): If there's an error retrieving the file list
        
    Example:
        ```python
        import requests
        
        # List all files in a session
        response = requests.get(
            'https://api.example.com/api/v2/sessions/tiger-castle/files'
        )
        
        files = response.json()
        for file in files:
            print(f"{file['filename']} ({file['id']}) - {file['size']} bytes")
            print(f"  Uploaded: {file['uploaded_at']}")
            print(f"  Status: {file['metadata'].get('processing_status', 'unknown')}")
        ```
    """
    # Get session and verify it exists
    session = session_service.get_session(str(session_id))
    if not session:
        logger.error(f"No session found for session_id: {session_id}")
        raise HTTPException(status_code=404, detail="Session not found")
    
    try:
        # Get files for the session
        files = file_handler.get_session_files(str(session_id))
        
        # Convert to FileMeta models
        return [
            FileMeta(
                id=f.id,
                filename=f.original_filename,
                content_type=f.mime_type,
                size=f.size,
                uploaded_at=f.upload_time,
                session_id=session_id,
                metadata={
                    "processed": f.processed,
                    "processing_status": f.processing_status,
                    "processing_error": f.processing_error
                }
            )
            for f in files
        ]
    except Exception as e:
        logger.error(f"Error listing files: {str(e)}")
        raise HTTPException(status_code=500, detail=f"Error listing files: {str(e)}")

@router.get(
    "/{session_id}/files/{file_id}",
    response_model=FileMeta,
    status_code=status.HTTP_200_OK,
    summary="Get file metadata",
    description="Retrieve metadata for a specific file in a session",
    responses={
        200: {
            "description": "File metadata",
            "content": {
                "application/json": {
                    "example": {
                        "id": "file_abc123",
                        "filename": "document.pdf",
                        "content_type": "application/pdf",
                        "size": 1048576,
                        "uploaded_at": "2025-04-04T12:00:00Z",
                        "session_id": "550e8400-e29b-41d4-a716-446655440000",
                        "metadata": {
                            "processed": True,
                            "processing_status": "complete",
                            "page_count": 5
                        }
                    }
                }
            }
        },
        404: {
            "description": "Session or file not found",
            "content": {
                "application/json": {
                    "example": {"detail": "File file_abc123 not found"}
                }
            }
        },
        500: {
            "description": "Server error retrieving file metadata",
            "content": {
                "application/json": {
                    "example": {"detail": "Error retrieving file metadata: Internal server error"}
                }
            }
        }
    }
)
@version(2)
async def get_file_metadata(
    session_id: str = Path(..., description="MnemonicSlug ID of the session the file belongs to (e.g., 'tiger-castle')"),
    file_id: str = Path(..., description="ID of the file to retrieve metadata for"),
    session_service: SessionService = Depends(get_session_service)
) -> FileMeta:
    """Get metadata for a specific file
    
    This endpoint retrieves detailed metadata for a specific file in a session.
    The metadata includes basic file information (name, size, type) as well as
    processing status and any additional metadata generated during file analysis.
    
    Args:
        session_id: The MnemonicSlug ID of the session (e.g., 'tiger-castle') the file belongs to
        file_id: The unique identifier of the file to retrieve metadata for
        session_service: Session service dependency injection
        
    Returns:
        FileMeta: Comprehensive metadata for the specified file
        
    Raises:
        HTTPException(404): If the session or file doesn't exist
        HTTPException(500): If there's an error retrieving the file metadata
        
    Example:
        ```python
        import requests
        
        # Get metadata for a specific file
        response = requests.get(
            'https://api.example.com/api/v2/sessions/tiger-castle/files/file_abc123'
        )
        
        file_meta = response.json()
        print(f"File: {file_meta['filename']}")
        print(f"Type: {file_meta['content_type']}")
        print(f"Size: {file_meta['size']} bytes")
        print(f"Uploaded: {file_meta['uploaded_at']}")
        print(f"Processing status: {file_meta['metadata'].get('processing_status', 'unknown')}")
        ```
    """
    # Get session and verify it exists
    session = session_service.get_session(str(session_id))
    if not session:
        logger.error(f"No session found for session_id: {session_id}")
        raise HTTPException(status_code=404, detail="Session not found")
    
    try:
        # Get file metadata
        metadata = file_handler.get_file_metadata(file_id, str(session_id))
        if not metadata:
            raise HTTPException(status_code=404, detail=f"File {file_id} not found")
        
        # Return as FileMeta model
        return FileMeta(
            id=metadata.id,
            filename=metadata.original_filename,
            content_type=metadata.mime_type,
            size=metadata.size,
            uploaded_at=metadata.upload_time,
            session_id=session_id,
            metadata={
                "processed": metadata.processed,
                "processing_status": metadata.processing_status,
                "processing_error": metadata.processing_error
            }
        )
    except HTTPException:
        raise
    except Exception as e:
        logger.error(f"Error retrieving file metadata: {str(e)}")
        raise HTTPException(status_code=500, detail=f"Error retrieving file metadata: {str(e)}")

@router.get(
    "/{session_id}/files/{file_id}/content",
    summary="Download file content",
    description="Download the content of a specific file",
    responses={
        200: {
            "description": "File content with appropriate Content-Type header",
            "content": {
                "application/octet-stream": {},
                "application/pdf": {},
                "image/jpeg": {},
                "image/png": {},
                "text/plain": {}
            }
        },
        404: {
            "description": "Session or file not found",
            "content": {
                "application/json": {
                    "example": {"detail": "File file_abc123 not found"}
                }
            }
        },
        500: {
            "description": "Server error downloading file",
            "content": {
                "application/json": {
                    "example": {"detail": "Error downloading file: Internal server error"}
                }
            }
        }
    }
)
@version(2)
async def download_file(
    session_id: str = Path(..., description="MnemonicSlug ID of the session the file belongs to (e.g., 'tiger-castle')"),
    file_id: str = Path(..., description="ID of the file to download"),
    session_service: SessionService = Depends(get_session_service)
):
    """Download a file's content
    
    This endpoint retrieves the actual content of a file previously uploaded to a session.
    It returns the file with the appropriate Content-Type header and attachment disposition,
    making it suitable for direct downloading in browsers.
    
    Args:
        session_id: The MnemonicSlug ID of the session (e.g., 'tiger-castle') the file belongs to
        file_id: The unique identifier of the file to download
        session_service: Session service dependency injection
        
    Returns:
        FileResponse: The file content with appropriate headers for downloading
        
    Raises:
        HTTPException(404): If the session or file doesn't exist
        HTTPException(500): If there's an error retrieving the file content
        
    Example:
        ```python
        import requests
        
        # Download a file (binary content)
        response = requests.get(
            'https://api.example.com/api/v2/sessions/tiger-castle/files/file_abc123/content',
            stream=True  # Important for large files
        )
        
        # Save the file locally
        filename = response.headers.get('content-disposition', '').split('filename=')[1].strip('"')
        with open(filename, 'wb') as f:
            for chunk in response.iter_content(chunk_size=8192):
                f.write(chunk)
                
        print(f"Downloaded {filename}")
        ```
    """
    # Get session and verify it exists
    session = session_service.get_session(str(session_id))
    if not session:
        logger.error(f"No session found for session_id: {session_id}")
        raise HTTPException(status_code=404, detail="Session not found")
    
    try:
        # Get file metadata
        metadata = file_handler.get_file_metadata(file_id, str(session_id))
        if not metadata:
            raise HTTPException(status_code=404, detail=f"File {file_id} not found")
        
        # Return file content
        return FileResponse(
            path=metadata.filename,
            filename=metadata.original_filename,
            media_type=metadata.mime_type
        )
    except HTTPException:
        raise
    except Exception as e:
        logger.error(f"Error downloading file: {str(e)}")
        raise HTTPException(status_code=500, detail=f"Error downloading file: {str(e)}")

@router.delete(
    "/{session_id}/files/{file_id}",
    status_code=status.HTTP_204_NO_CONTENT,
    summary="Delete a file",
    description="Remove a file from a session",
    responses={
        204: {
            "description": "File successfully deleted (no content returned)"
        },
        404: {
            "description": "Session or file not found",
            "content": {
                "application/json": {
                    "example": {"detail": "File file_abc123 not found"}
                }
            }
        },
        500: {
            "description": "Server error deleting file",
            "content": {
                "application/json": {
                    "example": {"detail": "Error deleting file: Could not remove file from storage"}
                }
            }
        }
    }
)
@version(2)
async def delete_file(
    session_id: str = Path(..., description="MnemonicSlug ID of the session the file belongs to (e.g., 'tiger-castle')"),
    file_id: str = Path(..., description="ID of the file to delete"),
    session_service: SessionService = Depends(get_session_service)
) -> None:
    """Delete a file from a session
    
    This endpoint permanently removes a file from a session. Once deleted, the file
    can no longer be referenced in chat messages or downloaded. This operation cannot
    be undone.
    
    Args:
        session_id: The MnemonicSlug ID of the session (e.g., 'tiger-castle') the file belongs to
        file_id: The unique identifier of the file to delete
        session_service: Session service dependency injection
        
    Returns:
        None: Returns no content on successful deletion (204 status code)
        
    Raises:
        HTTPException(404): If the session or file doesn't exist
        HTTPException(500): If there's an error deleting the file
        
    Example:
        ```python
        import requests
        
        # Delete a file
        response = requests.delete(
            'https://api.example.com/api/v2/sessions/tiger-castle/files/file_abc123'
        )
        
        # Check if deletion was successful (204 No Content)
        if response.status_code == 204:
            print("File successfully deleted")
        else:
            print(f"Error deleting file: {response.text}")
        ```
    """
    # Get session and verify it exists
    session = session_service.get_session(str(session_id))
    if not session:
        logger.error(f"No session found for session_id: {session_id}")
        raise HTTPException(status_code=404, detail="Session not found")
    
    try:
        # Get file metadata
        metadata = file_handler.get_file_metadata(file_id, str(session_id))
        if not metadata:
            raise HTTPException(status_code=404, detail=f"File {file_id} not found")
        
        # Delete the file
        try:
            import os
            os.remove(metadata.filename)
            
            # Update the session files
            session_files = file_handler.session_files.get(str(session_id), [])
            file_handler.session_files[str(session_id)] = [
                f for f in session_files if f.id != file_id
            ]
            
        except Exception as e:
            logger.error(f"Error deleting file {file_id}: {str(e)}")
            raise HTTPException(status_code=500, detail=f"Error deleting file: {str(e)}")
        
        return None
    except HTTPException:
        raise
    except Exception as e:
        logger.error(f"Error deleting file: {str(e)}")
        raise HTTPException(status_code=500, detail=f"Error deleting file: {str(e)}")
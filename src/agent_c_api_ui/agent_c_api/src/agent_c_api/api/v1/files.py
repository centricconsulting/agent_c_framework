import json
import logging
import traceback
from typing import List, Optional

from fastapi import APIRouter, UploadFile, File, Form, HTTPException, Depends, BackgroundTasks
from fastapi.responses import JSONResponse, FileResponse
from pydantic import BaseModel

from agent_c_api.api.dependencies import get_bridge_manager
from agent_c_api.core.file_handler import FileHandler, FileMetadata


router = APIRouter()
logger = logging.getLogger(__name__)


# Create a shared file handler
file_handler = FileHandler(base_dir="uploads", retention_days=7)


class UserFileResponse(BaseModel):
    """Response model for file operations"""
    id: str
    filename: str
    mime_type: str
    size: int


# Background task to clean up expired files
def cleanup_expired_files():
    """Background task to clean up expired files"""
    try:
        count = file_handler.cleanup_expired_files()
        if count > 0:
            logger.info(f"Cleaned up {count} expired files")
    except Exception as e:
        logger.error(f"Error cleaning up expired files: {str(e)}")


@router.post("/upload_file", response_model=UserFileResponse)
async def upload_file(
        ui_session_id: str = Form(...),
        file: UploadFile = File(...),
        background_tasks: BackgroundTasks = BackgroundTasks(),
        agent_manager=Depends(get_bridge_manager)
):
    """
    Upload a file for use in chat.

    Args:
        ui_session_id: Session ID
        file: The file to upload
        background_tasks: FastAPI background tasks
        agent_manager: Agent manager dependency

    Returns:
        FileResponse: Information about the uploaded file
    """
    # Schedule a cleanup task
    background_tasks.add_task(cleanup_expired_files)

    try:
        # Verify session exists
        logger.debug(f"Agent keys are {agent_manager.ui_sessions.keys()}")
        session_data = await agent_manager.get_user_session(ui_session_id)
        if not session_data:
            logger.error(f"No session found for session_id: {ui_session_id}")
            raise HTTPException(status_code=404, detail="Session not found")

        # Save the file
        metadata = await file_handler.save_file(file, ui_session_id)

        # Set the file handler on the agent if it doesn't have one
        agent_bridge = session_data.get("agent_bridge")
        if agent_bridge and not hasattr(agent_bridge, "file_handler"):
            agent_bridge.file_handler = file_handler
        elif agent_bridge and agent_bridge.file_handler is None:
            agent_bridge.file_handler = file_handler

        # Process the file in the background for text extraction
        background_tasks.add_task(file_handler.process_file, metadata.id, ui_session_id)

        return UserFileResponse(
            id=metadata.id,
            filename=metadata.original_filename,
            mime_type=metadata.mime_type,
            size=metadata.size,
        )
    except HTTPException:
        raise
    except Exception as e:
        error_traceback = traceback.format_exc()
        logger.error(f"Error uploading file: {str(e)}\n{error_traceback}")
        raise HTTPException(status_code=500, detail=f"Error uploading file: {str(e)}")


@router.get("/files/{ui_session_id}")
async def get_session_files(
        ui_session_id: str,
        agent_manager=Depends(get_bridge_manager)
):
    """
    List all files for a session.

    Args:
        ui_session_id: Session ID
        agent_manager: Agent manager dependency

    Returns:
        dict: List of files for the session
    """
    try:
        # Verify session exists
        session_data = await agent_manager.get_user_session(ui_session_id)
        if not session_data:
            logger.error(f"No session found for session_id: {ui_session_id}")
            raise HTTPException(status_code=404, detail="Session not found")

        # Get files for the session
        files = file_handler.get_session_files(ui_session_id)

        return {
            "files": [
                {
                    "id": f.id,
                    "filename": f.original_filename,
                    "mime_type": f.mime_type,
                    "size": f.size,
                    "upload_time": f.upload_time.isoformat(),
                    "processed": f.processed,
                    "processing_status": f.processing_status,
                    "processing_error": f.processing_error
                }
                for f in files
            ]
        }
    except HTTPException:
        raise
    except Exception as e:
        error_traceback = traceback.format_exc()
        logger.error(f"Error listing files: {str(e)}\n{error_traceback}")
        raise HTTPException(status_code=500, detail=f"Error listing files: {str(e)}")


@router.get("/files/{ui_session_id}/{file_id}")
async def get_file(
        ui_session_id: str,
        file_id: str,
        agent_manager=Depends(get_bridge_manager)
):
    """
    Get a specific file.

    Args:
        ui_session_id: Session ID
        file_id: File ID
        agent_manager: Agent manager dependency

    Returns:
        FileResponse: The file content
    """
    try:
        # Verify session exists
        session_data = await agent_manager.get_user_session(ui_session_id)
        if not session_data:
            logger.error(f"No session found for session_id: {ui_session_id}")
            raise HTTPException(status_code=404, detail="Session not found")

        # Get file metadata
        metadata = file_handler.get_file_metadata(file_id, ui_session_id)
        if not metadata:
            raise HTTPException(status_code=404, detail=f"File {file_id} not found")

        return UserFileResponse(
            path=metadata.filename,
            filename=metadata.original_filename,
            media_type=metadata.mime_type
        )
    except HTTPException:
        raise
    except Exception as e:
        error_traceback = traceback.format_exc()
        logger.error(f"Error retrieving file: {str(e)}\n{error_traceback}")
        raise HTTPException(status_code=500, detail=f"Error retrieving file: {str(e)}")


@router.delete("/files/{ui_session_id}/{file_id}")
async def delete_file(
        ui_session_id: str,
        file_id: str,
        agent_manager=Depends(get_bridge_manager)
):
    """
    Delete a specific file.

    Args:
        ui_session_id: Session ID
        file_id: File ID
        agent_manager: Agent manager dependency

    Returns:
        dict: Success message
    """
    try:
        # Verify session exists
        session_data = await agent_manager.get_user_session(ui_session_id)
        if not session_data:
            logger.error(f"No session found for session_id: {ui_session_id}")
            raise HTTPException(status_code=404, detail="Session not found")

        # Get file metadata
        metadata = file_handler.get_file_metadata(file_id, ui_session_id)
        if not metadata:
            raise HTTPException(status_code=404, detail=f"File {file_id} not found")

        # Delete the file
        try:
            import os
            os.remove(metadata.filename)
        except Exception as e:
            logger.error(f"Error deleting file {file_id}: {str(e)}")
            raise HTTPException(status_code=500, detail=f"Error deleting file: {str(e)}")

        # Update the session files
        session_files = file_handler.session_files.get(ui_session_id, [])
        file_handler.session_files[ui_session_id] = [
            f for f in session_files if f.id != file_id
        ]

        return {"message": f"File {metadata.original_filename} deleted successfully"}
    except HTTPException:
        raise
    except Exception as e:
        error_traceback = traceback.format_exc()
        logger.error(f"Error deleting file: {str(e)}\n{error_traceback}")
        raise HTTPException(status_code=500, detail=f"Error deleting file: {str(e)}")

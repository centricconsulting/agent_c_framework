import json
import os
import mimetypes
import traceback

from pathlib import Path
from typing import Optional, Dict, TYPE_CHECKING

from agent_c.util.logging_utils import LoggingManager
from agent_c.util.uncish_path import UNCishPath

from fastapi import APIRouter, UploadFile, File, Form, HTTPException, Depends, BackgroundTasks, Query, Request
from fastapi.responses import FileResponse
from agent_c.models.base import BaseModel

from agent_c_api.core.util.jwt import validate_request_jwt
from agent_c_tools.tools.workspace.local_project import LocalProjectWorkspace

if TYPE_CHECKING:
    from agent_c_api.core.file_handler import RTFileHandler
    from agent_c_api.models.realtime_session import RealtimeSession


router = APIRouter()
logger = LoggingManager(__name__).get_logger()

LOCAL_WORKSPACES_FILE = '.local_workspaces.json'

class WSResolver:
    workspaces: Optional[Dict[str, Path]] = None

    @classmethod
    def _init_workspaces(cls) -> None:
        local_project = LocalProjectWorkspace()
        cls.workspaces['project'] = local_project.workspace_root
        try:
            with open(LOCAL_WORKSPACES_FILE, 'r', encoding='utf-8') as json_file:
                local_workspaces = json.load(json_file)

            for ws in local_workspaces['local_workspaces']:
                cls.workspaces[ws['name']] =  Path(ws['workspace_path']).resolve()
        except FileNotFoundError:
            # Local workspaces file is optional
            pass

    @classmethod
    def resolve_workspace_path(cls, workspace_path: str) -> Path:
        if cls.workspaces is None:
            cls._init_workspaces()

        unc_path: UNCishPath = UNCishPath(workspace_path)
        if not unc_path.source not in cls.workspaces:
            raise ValueError(f"Workspace '{unc_path.source}' is not configured.")

        return Path.joinpath(cls.workspaces[unc_path.source], unc_path.path).resolve()



class UserFileResponse(BaseModel):
    """Response model for file operations"""
    id: str
    filename: str
    mime_type: str
    size: int


# Background task to clean up expired files
def cleanup_expired_files(file_handler: 'RTFileHandler'):
    """Background task to clean up expired files"""
    try:
        count = file_handler.cleanup_expired_files()
        if count > 0:
            logger.info(f"Cleaned up {count} expired files")
    except Exception as e:
        logger.error(f"Error cleaning up expired files: {str(e)}")

@router.get("/file/{workspace_path:path}")
async def get_workspace_file(
        request: Request,
        workspace_path: str,
        download: bool = Query(False, description="Force download vs inline viewing"),
):
    user_info = await validate_request_jwt(request)
    if not user_info:
        raise HTTPException(status_code=401, detail="Invalid token")

    actual_path = WSResolver.resolve_workspace_path(f"//{workspace_path}")

    if not os.path.exists(actual_path) or not os.path.isfile(actual_path):
        raise HTTPException(status_code=404, detail="File not found")

    filename = os.path.basename(actual_path)

    media_type, _ = mimetypes.guess_type(actual_path)

    if media_type is None:
        media_type = 'application/octet-stream'

    headers = {}

    if download:
        headers["Content-Disposition"] = f"attachment; filename={filename}"
    else:
        headers["Content-Disposition"] = f"inline; filename={filename}"

    return FileResponse(actual_path, filename=filename, media_type=media_type, headers=headers)

@router.post("/upload_file", response_model=UserFileResponse)
async def upload_file(
        request: Request,
        ui_session_id: str = Form(...),
        file: UploadFile = File(...),
        background_tasks: BackgroundTasks = BackgroundTasks()
):
    """
    Upload a file for use in chat.

    Args:
        ui_session_id: Session ID
        file: The file to upload
        background_tasks: FastAPI background tasks
    Returns:
        FileResponse: Information about the uploaded file
    """
    user_info = await validate_request_jwt(request)
    if not user_info:
        raise HTTPException(status_code=401, detail="Invalid token")

    manager = request.app.state.realtime_manager

    try:
        session_data: 'RealtimeSession' = manager.get_session_data(ui_session_id)
        if not session_data:
            logger.error(f"No session found for session_id: {ui_session_id}")
            raise HTTPException(status_code=404, detail="Session not found")

        file_handler: RTFileHandler = session_data.bridge.file_handler
        # Save the file
        metadata = await file_handler.save_file(file, ui_session_id)

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
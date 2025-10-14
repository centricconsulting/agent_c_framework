from typing import List, Optional

from fastapi import APIRouter, HTTPException, Form, Depends, Request, Query

from agent_c_api.api.v1.interactions.interaction_models.interaction_model import InteractionSummary, InteractionDetail
from agent_c_api.api.v1.interactions.services.interaction_service import InteractionService
from agent_c_api.core.util.logging_utils import LoggingManager

logging_manager = LoggingManager(__name__)
logger = logging_manager.get_logger()

router = APIRouter(
    prefix="/interactions"
)

interaction_service = InteractionService()

@router.get("/", response_model=List[InteractionSummary])
async def list_sessions(
    limit: Optional[int] = Query(50, description="Maximum number of sessions to return"),
    offset: Optional[int] = Query(0, description="Number of sessions to skip"),
    sort_by: Optional[str] = Query("timestamp", description="Field to sort by"),
    sort_order: Optional[str] = Query("desc", description="Sort order (asc or desc)")
):
    """
    List all available sessions with pagination and sorting.
    """
    return await interaction_service.list_sessions(limit, offset, sort_by, sort_order)

@router.get("/{session_id}", response_model=InteractionDetail)
async def get_session(session_id: str):
    """
    Get detailed information about a specific session.
    """
    session = await interaction_service.get_session(session_id)
    if not session:
        raise HTTPException(status_code=404, detail=f"Session {session_id} not found")
    return session

@router.get("/{session_id}/files", response_model=List[str])
async def get_session_files(session_id: str):
    """
    Get a list of all JSONL files associated with a specific session.
    """
    files = await interaction_service.get_session_files(session_id)
    if not files:
        raise HTTPException(status_code=404, detail=f"No files found for session {session_id}")
    return files

@router.delete("/{session_id}")
async def delete_session(session_id: str):
    """
    Delete a session directory and all its files.
    """
    success = await interaction_service.delete_session(session_id)
    if not success:
        raise HTTPException(status_code=404, detail=f"Session {session_id} not found or could not be deleted")
    return {"status": "success", "message": f"Session {session_id} deleted successfully"}
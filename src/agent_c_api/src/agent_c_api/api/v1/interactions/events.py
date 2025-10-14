from typing import List, Optional

from fastapi import APIRouter, HTTPException, Form, Depends, Request, Query, BackgroundTasks
from fastapi.responses import StreamingResponse
from agent_c_api.api.v1.interactions.interaction_models.event_model import Event, EventType, ReplayControlRequest
from agent_c_api.api.v1.interactions.services.event_service import EventService
from agent_c_api.core.util.logging_utils import LoggingManager

logging_manager = LoggingManager(__name__)
logger = logging_manager.get_logger()

router = APIRouter(prefix="/events")

event_service = EventService()


@router.get("/{session_id}", response_model=List[Event])
async def get_events(
    session_id: str,
    event_types: Optional[List[EventType]] = Query(None, description="Filter by event types"),
    start_time: Optional[str] = Query(None, description="Filter events after this timestamp"),
    end_time: Optional[str] = Query(None, description="Filter events before this timestamp"),
    limit: Optional[int] = Query(1000, description="Maximum number of events to return"),
):
    """
    Get events for a specific session with filtering options.
    """
    events = await event_service.get_events(session_id, event_types, start_time, end_time, limit)
    if not events:
        raise HTTPException(status_code=404, detail=f"No events found for session {session_id}")
    return events

@router.get("/{session_id}/stream")
async def stream_events(
    session_id: str,
    event_types: Optional[List[EventType]] = Query(None, description="Filter by event types"),
    real_time: bool = Query(False, description="Replay events with original timing"),
    speed_factor: float = Query(1.0, description="Speed multiplier for real-time replay")
):
    """
    Stream events for a specific session, optionally in real-time.
    """
    return StreamingResponse(
        event_service.stream_events(session_id, event_types, real_time, speed_factor),
        media_type="text/event-stream"
    )

@router.get("/{session_id}/replay-status")
async def get_replay_status(session_id: str):
    """
    Get the current status of a session replay.
    """
    status = event_service.get_replay_status(session_id)
    if not status:
        raise HTTPException(status_code=404, detail=f"No active replay for session {session_id}")
    return status

@router.post("/{session_id}/replay/control")
async def control_replay(
    session_id: str,
    control_request: ReplayControlRequest,
    background_tasks: BackgroundTasks = None
):
    """
    Control a session replay (play, pause, stop, seek).
    Accepts a JSON body with 'action' and optional 'position' fields.
    """
    result = await event_service.control_replay(
        session_id, 
        control_request.action, 
        control_request.position, 
        background_tasks
    )
    if not result:
        raise HTTPException(status_code=400, detail=f"Invalid replay control action")
    return {"status": "success", "action": control_request.action}
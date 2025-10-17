import pytest
from datetime import datetime, timedelta
from unittest.mock import  patch
from uuid import  uuid4

from agent_c_api.api.v2.history.services import EventService
from agent_c_api.api.v2.models.history_models import StoredEvent,  ReplayStatus
from agent_c.models.events.chat import MessageEvent
from agent_c_api.api.v2.models.response_models import APIResponse, PaginatedResponse, PaginationMeta, APIStatus

# Test data
session_id = uuid4()

# Create core event instances
user_event = MessageEvent(
    session_id=str(session_id),
    role="user",
    content="Hello, agent",
    format="markdown"
)

assistant_event = MessageEvent(
    session_id=str(session_id),
    role="assistant",
    content="Hello, user",
    format="markdown"
)

# Wrap in StoredEvent for API response
MOCK_EVENTS = [
    StoredEvent(
        id=f"{session_id}-1",
        event=user_event,
        timestamp=datetime.now() - timedelta(minutes=5)
    ),
    StoredEvent(
        id=f"{session_id}-2",
        event=assistant_event,
        timestamp=datetime.now() - timedelta(minutes=4)
    )
]

MOCK_REPLAY_STATUS = ReplayStatus(
    session_id=session_id,
    is_playing=True,
    current_position=datetime.now() - timedelta(minutes=4),
    start_time=datetime.now() - timedelta(minutes=10),
    end_time=datetime.now()
)

@pytest.mark.asyncio
@patch.object(EventService, "get_events")
async def test_get_events(mock_get_events, client):
    # Setup mock
    mock_response = PaginatedResponse(
        status=APIStatus(success=True),
        data=MOCK_EVENTS,
        pagination=PaginationMeta(
            page=1,
            page_size=100,
            total_items=len(MOCK_EVENTS),
            total_pages=1
        )
    )
    mock_get_events.return_value = mock_response
    
    # Test endpoint
    response = client.get(f"/api/v2/history/{session_id}/events")
    
    # Verify response
    assert response.status_code == 200
    data = response.json()
    assert data["status"]["success"] == True
    assert len(data["data"]) == len(MOCK_EVENTS)
    assert data["data"][0]["event"]["type"] == "message"

@pytest.mark.asyncio
@patch.object(EventService, "get_events")
async def test_get_events_with_filters(mock_get_events, client):
    # Setup mock with filtered data
    filtered_events = [MOCK_EVENTS[0]]  # Just the user_request
    mock_response = PaginatedResponse(
        status=APIStatus(success=True),
        data=filtered_events,
        pagination=PaginationMeta(
            page=1,
            page_size=10,
            total_items=len(filtered_events),
            total_pages=1
        )
    )
    mock_get_events.return_value = mock_response
    
    # Test endpoint with filters
    response = client.get(f"/api/v2/history/{session_id}/events?event_types=user_request&limit=10")
    
    # Verify response
    assert response.status_code == 200
    data = response.json()
    assert len(data["data"]) == 1
    assert data["data"][0]["event"]["type"] == "message"

@pytest.mark.asyncio
@patch.object(EventService, "get_events")
async def test_get_events_not_found(mock_get_events, client):
    # Setup mock for empty results
    mock_response = PaginatedResponse(
        status=APIStatus(success=True),
        data=[],
        pagination=PaginationMeta(
            page=1,
            page_size=100,
            total_items=0,
            total_pages=0
        )
    )
    mock_get_events.return_value = mock_response
    
    # Test endpoint
    response = client.get(f"/api/v2/history/{uuid4()}/events")
    
    # Verify response
    assert response.status_code == 404
    detail = response.json()["detail"]
    assert "No events found" in detail["error"]
    assert detail["error_code"] == "EVENTS_NOT_FOUND"

@pytest.mark.asyncio
@patch.object(EventService, "stream_events")
async def test_stream_events(mock_stream_events, client):
    # Setup mock
    async def mock_event_generator():
        for i in range(3):
            yield f"data: {i}\n\n"
    
    mock_stream_events.return_value = mock_event_generator()
    
    # Test endpoint
    response = client.get(f"/api/v2/history/{session_id}/stream")
    
    # Verify it's a streaming response
    assert response.status_code == 200
    assert response.headers["content-type"] == "text/event-stream"

@pytest.mark.asyncio
@patch.object(EventService, "get_replay_status")
async def test_get_replay_status(mock_get_replay_status, client):
    # Setup mock
    mock_get_replay_status.return_value = MOCK_REPLAY_STATUS
    
    # Test endpoint
    response = client.get(f"/api/v2/history/{session_id}/replay")
    
    # Verify response
    assert response.status_code == 200
    data = response.json()
    assert data["status"]["success"] == True
    assert data["data"]["session_id"] == str(session_id)
    assert data["data"]["is_playing"] == True

@pytest.mark.asyncio
@patch.object(EventService, "get_replay_status")
async def test_get_replay_status_not_found(mock_get_replay_status, client):
    # Setup mock
    mock_get_replay_status.return_value = None
    
    # Test endpoint
    response = client.get(f"/api/v2/history/{uuid4()}/replay")
    
    # Verify response
    assert response.status_code == 404
    detail = response.json()["detail"]
    assert "No active replay" in detail["error"]
    assert detail["error_code"] == "REPLAY_NOT_FOUND"

@pytest.mark.asyncio
@patch.object(EventService, "control_replay")
async def test_control_replay(mock_control_replay, client):
    # Setup mock
    mock_response = APIResponse(
        status=APIStatus(success=True, message="Replay control successful"),
        data=True
    )
    mock_control_replay.return_value = mock_response
    
    # Test endpoint
    response = client.post(
        f"/api/v2/history/{session_id}/replay",
        json={"action": "play"}
    )
    
    # Verify response
    assert response.status_code == 200
    data = response.json()
    assert data["status"]["success"] == True
    assert data["data"] == True

@pytest.mark.asyncio
@patch.object(EventService, "control_replay")
async def test_control_replay_invalid(mock_control_replay, client):
    # Setup mock
    mock_response = APIResponse(
        status=APIStatus(success=False, message="Replay control failed", error_code="REPLAY_CONTROL_FAILED"),
        data=False
    )
    mock_control_replay.return_value = mock_response
    
    # Test endpoint
    response = client.post(
        f"/api/v2/history/{session_id}/replay",
        json={"action": "pause"}
    )
    
    # Verify response
    assert response.status_code == 200  # Still returns 200 but with success=False
    data = response.json()
    assert data["status"]["success"] == False
    assert data["status"]["error_code"] == "REPLAY_CONTROL_FAILED"
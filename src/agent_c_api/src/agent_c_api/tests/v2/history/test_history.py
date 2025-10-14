import pytest
from httpx import AsyncClient
from datetime import datetime, timedelta
from unittest.mock import AsyncMock, patch
from fastapi import FastAPI
from agent_c_api.api.v2.history.services import HistoryService
from agent_c_api.api.v2.models import HistorySummary, HistoryDetail

# Test data
MOCK_HISTORY_SUMMARIES = [
    {
        "id": "session1",
        "start_time": datetime.now() - timedelta(hours=2),
        "end_time": datetime.now() - timedelta(hours=1),
        "duration_seconds": 3600,
        "event_count": 120,
        "file_count": 2
    },
    {
        "id": "session2",
        "start_time": datetime.now() - timedelta(hours=4),
        "end_time": datetime.now() - timedelta(hours=3),
        "duration_seconds": 3600,
        "event_count": 80,
        "file_count": 1
    }
]

MOCK_HISTORY_DETAIL = {
    "id": "session1",
    "start_time": datetime.now() - timedelta(hours=2),
    "end_time": datetime.now() - timedelta(hours=1),
    "duration_seconds": 3600,
    "event_count": 120,
    "file_count": 2,
    "files": ["events_1.jsonl", "events_2.jsonl"],
    "event_types": {"text_delta": 80, "tool_call": 10, "user_request": 5},
    "metadata": {"model": "gpt-4"},
    "user_id": "user123",
    "has_thinking": True,
    "tool_calls": ["tool1", "tool2"]
}

@pytest.mark.asyncio
@patch.object(HistoryService, "list_histories")
async def test_list_histories(mock_list_histories, client):
    # Setup mock
    mock_response = {
        "items": MOCK_HISTORY_SUMMARIES,
        "total": len(MOCK_HISTORY_SUMMARIES),
        "limit": 50,
        "offset": 0
    }
    mock_list_histories.return_value = mock_response
    
    # Test endpoint
    response = client.get("/api/v2/history")
    
    # Verify response
    assert response.status_code == 200
    data = response.json()
    assert data["total"] == len(MOCK_HISTORY_SUMMARIES)
    assert len(data["items"]) == len(MOCK_HISTORY_SUMMARIES)
    assert data["items"][0]["id"] == "session1"

@pytest.mark.asyncio
@patch.object(HistoryService, "get_history")
async def test_get_history(mock_get_history, client):
    # Setup mock
    mock_get_history.return_value = MOCK_HISTORY_DETAIL
    
    # Test endpoint
    response = client.get("/api/v2/history/session1")
    
    # Verify response
    assert response.status_code == 200
    data = response.json()
    assert data["id"] == "session1"
    assert data["event_count"] == 120
    assert len(data["files"]) == 2
    assert "tool1" in data["tool_calls"]

@pytest.mark.asyncio
@patch.object(HistoryService, "get_history")
async def test_get_history_not_found(mock_get_history, client):
    # Setup mock
    mock_get_history.return_value = None
    
    # Test endpoint
    response = client.get("/api/v2/history/nonexistent")
    
    # Verify response
    assert response.status_code == 404
    assert "not found" in response.json()["detail"]

@pytest.mark.asyncio
@patch.object(HistoryService, "delete_history")
async def test_delete_history(mock_delete_history, client):
    # Setup mock
    mock_delete_history.return_value = True
    
    # Test endpoint
    response = client.delete("/api/v2/history/session1")
    
    # Verify response
    assert response.status_code == 200
    assert response.json()["status"] == "success"

@pytest.mark.asyncio
@patch.object(HistoryService, "delete_history")
async def test_delete_history_not_found(mock_delete_history, client):
    # Setup mock
    mock_delete_history.return_value = False
    
    # Test endpoint
    response = client.delete("/api/v2/history/nonexistent")
    
    # Verify response
    assert response.status_code == 404
    assert "not found" in response.json()["detail"]
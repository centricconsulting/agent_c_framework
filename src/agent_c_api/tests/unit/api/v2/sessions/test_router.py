"""Unit tests for the Session Router in v2 API.

These tests verify functionality of the session router endpoints, including:
- Session creation
- Session listing with pagination
- Session retrieval by ID
- Session updates
- Session deletion

The tests use mocked dependencies to isolate the router functionality.
"""

import pytest
from datetime import datetime
from fastapi import HTTPException
from fastapi.testclient import TestClient
from unittest.mock import AsyncMock, MagicMock, patch

from agent_c_api.api.v2.models import SessionCreate, SessionDetail, SessionUpdate, SessionListResponse, SessionSummary
from agent_c_api.api.v2.sessions.sessions import router, get_session_service
from agent_c_api.main import app


@pytest.fixture
def mock_session_service():
    """Fixture for a mocked SessionService.
    
    Returns a MagicMock with AsyncMock methods that can be configured to return
    specific responses or raise exceptions as needed by individual tests.
    """
    service = MagicMock()
    service.create_session = AsyncMock()
    service.get_sessions = AsyncMock()
    service.get_session = AsyncMock()
    service.update_session = AsyncMock()
    service.delete_session = AsyncMock()
    return service


@pytest.fixture
def client(mock_session_service):
    """Fixture for a FastAPI test client with mocked dependencies.
    
    Args:
        mock_session_service: The mocked SessionService to inject
        
    Returns:
        TestClient: FastAPI test client with dependency overrides configured
    """
    app.dependency_overrides[get_session_service] = lambda: mock_session_service
    yield TestClient(app)
    app.dependency_overrides = {}


@pytest.fixture
def sample_session_detail():
    """Fixture for a sample SessionDetail object.
    
    Returns:
        SessionDetail: A fully populated session detail object using MnemonicSlug ID format
    """
    return SessionDetail(
        id="tiger-castle",  # Using MnemonicSlug format
        model_id="gpt-4",
        persona_id="programmer",
        name="Test Session",  # Required field
        is_active=True,  # Required field
        created_at=datetime.now(),
        last_activity=datetime.now(),
        agent_internal_id="agent_123",
        tools=["search", "calculator"],
        tool_ids=["search", "calculator"],  # Required field
        temperature=0.7,
        reasoning_effort=8,  # Must be <= 10
        budget_tokens=None,
        max_tokens=2000,
        custom_prompt=None
    )


@pytest.fixture
def sample_session_list():
    """Fixture for a sample SessionListResponse object.
    
    Returns:
        SessionListResponse: A response with a list of session summaries and pagination metadata
    """
    return SessionListResponse(
        items=[
            SessionSummary(
                id="tiger-castle",  # Using MnemonicSlug format
                model_id="gpt-4",
                persona_id="programmer",
                name="Test Session 1",  # Required field
                is_active=True,  # Required field
                created_at=datetime.now(),
                last_activity=datetime.now()
            ),
            SessionSummary(
                id="banana-phone",  # Using MnemonicSlug format
                model_id="claude-3",
                persona_id="default",
                name="Test Session 2",  # Required field
                is_active=True,  # Required field
                created_at=datetime.now(),
                last_activity=None
            )
        ],
        total=2,
        limit=10,
        offset=0
    )


@pytest.mark.unit
@pytest.mark.session
class TestSessionRouter:
    """Tests for the session router endpoints.
    
    This class tests the HTTP endpoints for CRUD operations on sessions,
    verifying both successful and error cases.
    """
    
    @pytest.mark.asyncio
    async def test_create_session(self, client, mock_session_service, sample_session_detail):
        """Test creating a session with valid parameters.
        
        Should return a 201 Created response with the created session details.
        """
        # Arrange
        mock_session_service.create_session.return_value = sample_session_detail
        
        # Act
        response = client.post(
            "/api/v2/sessions",
            json={
                "model_id": "gpt-4",
                "persona_id": "programmer",
                "temperature": 0.7,
                "tools": ["search", "calculator"]
            }
        )
        
        # Assert
        assert response.status_code == 201
        assert response.json()["id"] == "tiger-castle"  # Using MnemonicSlug ID
        assert response.json()["model_id"] == "gpt-4"
        assert response.json()["persona_id"] == "programmer"
        assert response.json()["temperature"] == 0.7
        assert "created_at" in response.json()
        assert response.json()["tools"] == ["search", "calculator"]
        mock_session_service.create_session.assert_called_once()
    
    @pytest.mark.asyncio
    async def test_create_session_validation_error(self, client, mock_session_service):
        """Test creating a session with invalid parameters.
        
        Should return a 422 Unprocessable Entity response with validation error details.
        """
        # Act - Send temperature outside valid range (0-1)
        response = client.post(
            "/api/v2/sessions",
            json={
                "model_id": "gpt-4",
                "persona_id": "programmer",
                "temperature": 1.5,  # Invalid: outside 0-1 range
                "tools": ["search", "calculator"]
            }
        )
        
        # Assert
        assert response.status_code == 422
        assert "detail" in response.json()
        assert "temperature" in response.json()["detail"][0]["loc"]
        mock_session_service.create_session.assert_not_called()
    
    @pytest.mark.asyncio
    async def test_create_session_server_error(self, client, mock_session_service):
        """Test server error handling during session creation.
        
        Should return a 500 Internal Server Error response with error details.
        """
        # Arrange
        mock_session_service.create_session.side_effect = Exception("Test server error")
        
        # Act
        response = client.post(
            "/api/v2/sessions",
            json={
                "model_id": "gpt-4",
                "persona_id": "programmer",
                "temperature": 0.7,
                "tools": ["search", "calculator"]
            }
        )
        
        # Assert
        assert response.status_code == 500
        assert "detail" in response.json()
        assert "server error" in response.json()["detail"].lower()
        mock_session_service.create_session.assert_called_once()
    
    @pytest.mark.asyncio
    async def test_list_sessions(self, client, mock_session_service, sample_session_list):
        """Test listing sessions with pagination.
        
        Should return a 200 OK response with a list of sessions and pagination metadata.
        Tests both default pagination and custom pagination parameters.
        """
        # Arrange
        mock_session_service.get_sessions.return_value = sample_session_list
        
        # Act - Test with default pagination
        response = client.get("/api/v2/sessions")
        
        # Assert
        assert response.status_code == 200
        assert response.json()["total"] == 2
        assert len(response.json()["items"]) == 2
        assert response.json()["items"][0]["id"] == "tiger-castle"
        assert response.json()["items"][1]["id"] == "banana-phone"
        assert response.json()["limit"] == 10
        assert response.json()["offset"] == 0
        mock_session_service.get_sessions.assert_called_once_with(10, 0)
        
        # Act - Test with custom pagination
        response = client.get("/api/v2/sessions?limit=5&offset=1")
        
        # Assert
        assert response.status_code == 200
        mock_session_service.get_sessions.assert_called_with(5, 1)
    
    @pytest.mark.asyncio
    async def test_response_model_validation(self, client, mock_session_service, sample_session_detail, sample_session_list):
        """Test that responses conform to expected models.
        
        Verifies that the response structures match the expected Pydantic models.
        """
        # Arrange for session detail
        mock_session_service.get_session.return_value = sample_session_detail
        mock_session_service.get_sessions.return_value = sample_session_list
        
        # Act - Get session detail
        detail_response = client.get("/api/v2/sessions/tiger-castle")
        list_response = client.get("/api/v2/sessions")
        
        # Assert session detail structure
        detail_json = detail_response.json()
        assert "id" in detail_json
        assert "model_id" in detail_json
        assert "persona_id" in detail_json
        assert "created_at" in detail_json
        assert "last_activity" in detail_json
        assert "agent_internal_id" in detail_json
        assert "tools" in detail_json
        assert "temperature" in detail_json
        assert "reasoning_effort" in detail_json
        
        # Assert session list structure
        list_json = list_response.json()
        assert "items" in list_json
        assert "total" in list_json
        assert "limit" in list_json
        assert "offset" in list_json
        assert len(list_json["items"]) > 0
        assert "id" in list_json["items"][0]
        assert "model_id" in list_json["items"][0]
    
    @pytest.mark.asyncio
    async def test_get_session_existing(self, client, mock_session_service, sample_session_detail):
        """Test getting an existing session by ID.
        
        Should return a 200 OK response with the session details.
        """
        # Arrange
        mock_session_service.get_session.return_value = sample_session_detail
        
        # Act
        response = client.get("/api/v2/sessions/tiger-castle")
        
        # Assert
        assert response.status_code == 200
        assert response.json()["id"] == "tiger-castle"
        assert response.json()["model_id"] == "gpt-4"
        assert response.json()["persona_id"] == "programmer"
        assert response.json()["temperature"] == 0.7
        mock_session_service.get_session.assert_called_once_with("tiger-castle")
    
    @pytest.mark.asyncio
    async def test_get_session_nonexistent(self, client, mock_session_service):
        """Test getting a non-existent session by ID.
        
        Should return a 404 Not Found response with error details.
        """
        # Arrange
        mock_session_service.get_session.return_value = None
        
        # Act
        response = client.get("/api/v2/sessions/nonexistent-session")
        
        # Assert
        assert response.status_code == 404
        assert "not found" in response.json()["detail"].lower()
        mock_session_service.get_session.assert_called_once_with("nonexistent-session")
    
    @pytest.mark.asyncio
    async def test_update_session(self, client, mock_session_service, sample_session_detail):
        """Test updating an existing session.
        
        Should return a 200 OK response with the updated session details.
        """
        # Arrange
        updated_session = sample_session_detail.model_copy()
        updated_session.persona_id = "researcher"
        updated_session.temperature = 0.8
        mock_session_service.get_session.return_value = sample_session_detail
        mock_session_service.update_session.return_value = updated_session
        
        # Act
        response = client.patch(
            "/api/v2/sessions/tiger-castle",
            json={
                "persona_id": "researcher",
                "temperature": 0.8
            }
        )
        
        # Assert
        assert response.status_code == 200
        assert response.json()["id"] == "tiger-castle"
        assert response.json()["persona_id"] == "researcher"
        assert response.json()["temperature"] == 0.8
        assert response.json()["model_id"] == "gpt-4"  # Unchanged field
        mock_session_service.update_session.assert_called_once()
    
    @pytest.mark.asyncio
    async def test_update_session_error(self, client, mock_session_service, sample_session_detail):
        """Test handling errors during session update.
        
        Should return a 500 Internal Server Error response with error details.
        """
        # Arrange
        mock_session_service.get_session.return_value = sample_session_detail
        mock_session_service.update_session.side_effect = Exception("Test error")
        
        # Act
        response = client.patch(
            "/api/v2/sessions/tiger-castle",
            json={"temperature": 0.8}
        )
        
        # Assert
        assert response.status_code == 500
        assert "Failed to update session" in response.json()["detail"]
        mock_session_service.update_session.assert_called_once()
    
    @pytest.mark.asyncio
    async def test_delete_session_existing(self, client, mock_session_service, sample_session_detail):
        """Test deleting an existing session.
        
        Should return a 204 No Content response on successful deletion.
        """
        # Arrange
        mock_session_service.get_session.return_value = sample_session_detail
        mock_session_service.delete_session.return_value = True
        
        # Act
        response = client.delete("/api/v2/sessions/tiger-castle")
        
        # Assert
        assert response.status_code == 204
        assert response.content == b''  # No content in response body
        mock_session_service.delete_session.assert_called_once_with("tiger-castle")
    
    @pytest.mark.asyncio
    async def test_delete_session_nonexistent(self, client, mock_session_service):
        """Test deleting a non-existent session.
        
        Should return a 404 Not Found response with error details.
        """
        # Arrange
        mock_session_service.get_session.return_value = None
        
        # Act
        response = client.delete("/api/v2/sessions/nonexistent-session")
        
        # Assert
        assert response.status_code == 404
        assert "not found" in response.json()["detail"].lower()
        mock_session_service.delete_session.assert_not_called()
    
    @pytest.mark.asyncio
    async def test_delete_session_failure(self, client, mock_session_service, sample_session_detail):
        """Test failure when deleting a session.
        
        Should return a 500 Internal Server Error response with error details when
        the session exists but deletion fails.
        """
        # Arrange
        mock_session_service.get_session.return_value = sample_session_detail
        mock_session_service.delete_session.return_value = False
        
        # Act
        response = client.delete("/api/v2/sessions/tiger-castle")
        
        # Assert
        assert response.status_code == 500
        assert "Failed to delete session" in response.json()["detail"]
        mock_session_service.delete_session.assert_called_once_with("tiger-castle")
# tests/unit/api/v2/debug/test_debug.py
import pytest
from unittest.mock import AsyncMock, MagicMock
from fastapi import HTTPException

from agent_c_api.api.v2.debug.debug import get_session_debug_info, get_agent_debug_info
from agent_c_api.core.user_session_manager import UserSessionManager
from agent_c_api.api.v2.models.debug_models import SessionDebugInfo, AgentDebugInfo
from agent_c_api.api.v2.models.response_models import APIResponse, APIStatus


@pytest.fixture
def mock_agent_manager():
    """
    Create a mock agent manager for testing.
    
    Returns:
        MagicMock: A mocked agent manager with necessary methods configured
    """
    manager = MagicMock(spec=UserSessionManager)
    manager.debug_session = AsyncMock()
    manager.get_session_data = MagicMock()
    return manager


@pytest.mark.unit
@pytest.mark.debug
class TestSessionDebugEndpoint:
    """
    Tests for the debug session endpoint.
    
    This class tests the functionality of the get_session_debug_info endpoint, including
    success cases, error handling, and response model validation.
    """

    @pytest.mark.asyncio
    async def test_get_session_debug_info_success(self, mock_agent_manager):
        """
        Test successful retrieval of session debug info.
        
        Verifies that the endpoint correctly returns debug info and formats the response
        when a valid session ID is provided.
        """
        # Setup mock response
        session_id = "tiger-castle"  # Use MnemonicSlug format
        mock_debug_info = {
            "session_id": session_id,
            "agent_c_session_id": "internal-session-id",
            "agent_name": "Test Agent",
            "created_at": "2025-05-04T15:30:00",
            "backend": "openai",
            "model_name": "gpt-4",
            "session_manager": {"exists": True, "user_id": "test-user", "has_chat_session": True},
            "chat_session": {"session_id": "chat-session-id", "has_active_memory": True},
            "messages": {"count": 10, "user_messages": 5, "assistant_messages": 5, "latest_message": "Hello..."},
            "recent_messages": [
                {"role": "user", "content_preview": "Hello", "timestamp": "2025-05-04T15:35:00"},
                {"role": "assistant", "content_preview": "Hi there", "timestamp": "2025-05-04T15:35:10"}
            ],
            "current_chat_Log": {"exists": True, "count": 10},
            "tool_chest": {"exists": True, "active_tools": ["tool1", "tool2"]}
        }
        mock_agent_manager.debug_session.return_value = mock_debug_info

        # Call the endpoint function
        response = await get_session_debug_info(session_id, mock_agent_manager)

        # Verify the response
        assert response.status.success == True
        assert response.status.message == "Session debug information retrieved successfully"
        assert response.data == mock_debug_info
        mock_agent_manager.debug_session.assert_called_once_with(session_id)

    @pytest.mark.asyncio
    async def test_get_session_debug_info_session_not_found(self, mock_agent_manager):
        """
        Test session debug info when session is not found.
        
        Verifies that the endpoint correctly raises a 404 error when an invalid or 
        non-existent session ID is provided.
        """
        # Setup mock to raise ValueError
        session_id = "nonexistent-session"
        mock_agent_manager.debug_session.side_effect = ValueError(f"Invalid session ID: {session_id}")

        # Call the endpoint function and expect an exception
        with pytest.raises(HTTPException) as exc_info:
            await get_session_debug_info(session_id, mock_agent_manager)

        # Verify the exception
        assert exc_info.value.status_code == 404
        assert f"Invalid session ID: {session_id}" in str(exc_info.value.detail)
        mock_agent_manager.debug_session.assert_called_once_with(session_id)

    @pytest.mark.asyncio
    async def test_get_session_debug_info_error(self, mock_agent_manager):
        """
        Test session debug info when an unexpected error occurs.
        
        Verifies that the endpoint correctly raises a 500 error with an appropriate
        message when an unexpected exception occurs during request processing.
        """
        # Setup mock to raise an unexpected exception
        session_id = "error-session"
        mock_agent_manager.debug_session.side_effect = Exception("Unexpected error")

        # Call the endpoint function and expect an exception
        with pytest.raises(HTTPException) as exc_info:
            await get_session_debug_info(session_id, mock_agent_manager)

        # Verify the exception
        assert exc_info.value.status_code == 500
        assert "Error retrieving debug information" in str(exc_info.value.detail)
        mock_agent_manager.debug_session.assert_called_once_with(session_id)

    @pytest.mark.asyncio
    async def test_session_debug_info_response_model(self, mock_agent_manager):
        """
        Test that the session debug info response conforms to the expected model.
        
        Verifies that the response structure follows the APIResponse[SessionDebugInfo] model
        and contains all required fields.
        """
        # Setup mock response
        session_id = "tiger-castle"
        mock_debug_info = {
            "session_id": session_id,
            "agent_c_session_id": "internal-session-id",
            "agent_name": "Test Agent",
            "created_at": "2025-05-04T15:30:00",
            "backend": "openai",
            "model_name": "gpt-4",
            "session_manager": {"exists": True, "user_id": "test-user", "has_chat_session": True},
            "current_chat_Log": {"exists": True, "count": 10},
            "tool_chest": {"exists": True, "active_tools": ["tool1", "tool2"]}
        }
        mock_agent_manager.debug_session.return_value = mock_debug_info

        # Call the endpoint function
        response = await get_session_debug_info(session_id, mock_agent_manager)

        # Verify response model structure
        assert isinstance(response, APIResponse)
        assert isinstance(response.status, APIStatus)
        assert isinstance(response.data, dict)  # Should match SessionDebugInfo structure
        
        # Verify required fields
        assert all(k in response.data for k in [
            "session_id", "agent_c_session_id", "agent_name", "created_at",
            "backend", "model_name", "session_manager", "current_chat_Log", "tool_chest"
        ])

    @pytest.mark.asyncio
    async def test_session_debug_info_minimal(self, mock_agent_manager):
        """
        Test session debug info when minimal data is available.
        
        Verifies that the endpoint correctly handles minimal debug information
        and optional fields are properly handled when missing.
        """
        # Setup mock with minimal data
        session_id = "minimal-session"
        minimal_debug_info = {
            "session_id": session_id,
            "agent_c_session_id": "internal-id",
            "agent_name": "Minimal Agent",
            "created_at": "2025-05-04T12:00:00",
            "backend": "anthropic",
            "model_name": "claude-3",
            "session_manager": {"exists": True},
            "current_chat_Log": {"exists": False, "count": 0},
            "tool_chest": {"exists": True, "active_tools": []}
            # Missing optional fields: chat_session, messages, recent_messages
        }
        mock_agent_manager.debug_session.return_value = minimal_debug_info

        # Call the endpoint function
        response = await get_session_debug_info(session_id, mock_agent_manager)

        # Verify response handles minimal data appropriately
        assert response.status.success == True
        assert response.data["session_id"] == session_id
        assert "chat_session" not in response.data
        assert "messages" not in response.data
        assert "recent_messages" not in response.data
        assert response.data["tool_chest"]["active_tools"] == []


@pytest.mark.unit
@pytest.mark.debug
class TestAgentDebugEndpoint:
    """
    Tests for the agent debug endpoint.
    
    This class tests the functionality of the get_agent_debug_info endpoint, including
    success cases, error handling, and response model validation.
    """

    @pytest.mark.asyncio
    async def test_get_agent_debug_info_success(self, mock_agent_manager):
        """
        Test successful retrieval of agent debug info.
        
        Verifies that the endpoint correctly returns debug info about an agent and
        formats the response properly when a valid session ID is provided.
        """
        # Setup mock response
        session_id = "tiger-castle"  # Use MnemonicSlug format
        mock_agent = MagicMock()
        mock_agent.temperature = 0.7
        mock_agent.reasoning_effort = "high"
        mock_agent.extended_thinking = True
        mock_agent.budget_tokens = 4000
        mock_agent.max_tokens = 2000

        # Setup mock internal agent
        mock_internal_agent = MagicMock()
        mock_internal_agent.__class__.__name__ = "TestAgent"
        mock_internal_agent.temperature = 0.8
        mock_internal_agent.reasoning_effort = "medium"
        mock_internal_agent.budget_tokens = 3000
        mock_internal_agent.max_tokens = 1500
        mock_agent.agent = mock_internal_agent

        # Setup mock session data
        mock_agent_manager.get_session_data.return_value = {"agent": mock_agent}

        # Call the endpoint function
        response = await get_agent_debug_info(session_id, mock_agent_manager)

        # Verify the response
        assert response.status.success == True
        assert response.status.message == "Agent debug information retrieved successfully"
        assert response.data["status"] == "success"
        assert response.data["agent_bridge_params"]["temperature"] == 0.7
        assert response.data["agent_bridge_params"]["reasoning_effort"] == "high"
        assert response.data["agent_bridge_params"]["extended_thinking"] == True
        assert response.data["agent_bridge_params"]["budget_tokens"] == 4000
        assert response.data["agent_bridge_params"]["max_tokens"] == 2000
        assert response.data["internal_agent_params"]["type"] == "TestAgent"
        assert response.data["internal_agent_params"]["temperature"] == 0.8
        assert response.data["internal_agent_params"]["reasoning_effort"] == "medium"
        assert response.data["internal_agent_params"]["budget_tokens"] == 3000
        assert response.data["internal_agent_params"]["max_tokens"] == 1500
        mock_agent_manager.get_session_data.assert_called_once_with(session_id)

    @pytest.mark.asyncio
    async def test_get_agent_debug_info_session_not_found(self, mock_agent_manager):
        """
        Test agent debug info when session is not found.
        
        Verifies that the endpoint correctly raises a 404 error when an invalid or
        non-existent session ID is provided.
        """
        # Setup mock to return None for session data
        session_id = "nonexistent-session"
        mock_agent_manager.get_session_data.return_value = None

        # Call the endpoint function and expect an exception
        with pytest.raises(HTTPException) as exc_info:
            await get_agent_debug_info(session_id, mock_agent_manager)

        # Verify the exception
        assert exc_info.value.status_code == 404
        assert "Session not found" in str(exc_info.value.detail)
        mock_agent_manager.get_session_data.assert_called_once_with(session_id)

    @pytest.mark.asyncio
    async def test_get_agent_debug_info_no_internal_agent(self, mock_agent_manager):
        """
        Test agent debug info when there's no internal agent.
        
        Verifies that the endpoint correctly handles the case where an agent exists
        but has no internal agent configured.
        """
        # Setup mock response without internal agent
        session_id = "tiger-castle"
        mock_agent = MagicMock()
        mock_agent.temperature = 0.7
        mock_agent.agent = None  # No internal agent

        # Setup mock session data
        mock_agent_manager.get_session_data.return_value = {"agent": mock_agent}

        # Call the endpoint function
        response = await get_agent_debug_info(session_id, mock_agent_manager)

        # Verify the response contains agent_bridge_params but internal_agent_params is empty
        assert response.status.success == True
        assert response.data["agent_bridge_params"]["temperature"] == 0.7
        assert response.data["internal_agent_params"] == {}

    @pytest.mark.asyncio
    async def test_get_agent_debug_info_error(self, mock_agent_manager):
        """
        Test agent debug info when an unexpected error occurs.
        
        Verifies that the endpoint correctly raises a 500 error with an appropriate
        message when an unexpected exception occurs during request processing.
        """
        # Setup mock to raise an unexpected exception
        session_id = "error-session"
        mock_agent_manager.get_session_data.side_effect = Exception("Unexpected error")

        # Call the endpoint function and expect an exception
        with pytest.raises(HTTPException) as exc_info:
            await get_agent_debug_info(session_id, mock_agent_manager)

        # Verify the exception
        assert exc_info.value.status_code == 500
        assert "Error retrieving agent debug information" in str(exc_info.value.detail)
        mock_agent_manager.get_session_data.assert_called_once_with(session_id)

    @pytest.mark.asyncio
    async def test_agent_debug_info_response_model(self, mock_agent_manager):
        """
        Test that the agent debug info response conforms to the expected model.
        
        Verifies that the response structure follows the APIResponse[AgentDebugInfo] model
        and contains all required fields with the correct data types.
        """
        # Setup mock response
        session_id = "tiger-castle"
        mock_agent = MagicMock()
        mock_agent.temperature = 0.7
        mock_agent.reasoning_effort = "high"
        mock_agent.extended_thinking = True
        mock_agent.budget_tokens = 4000
        mock_agent.max_tokens = 2000
        mock_agent.agent = None  # No internal agent for simpler test

        # Setup mock session data
        mock_agent_manager.get_session_data.return_value = {"agent": mock_agent}

        # Call the endpoint function
        response = await get_agent_debug_info(session_id, mock_agent_manager)

        # Verify response model structure
        assert isinstance(response, APIResponse)
        assert isinstance(response.status, APIStatus)
        assert isinstance(response.data, dict)  # Should match AgentDebugInfo structure
        
        # Verify required fields
        assert all(k in response.data for k in [
            "status", "agent_bridge_params", "internal_agent_params"
        ])
        
        # Verify field types
        assert isinstance(response.data["status"], str)
        assert isinstance(response.data["agent_bridge_params"], dict)
        assert isinstance(response.data["internal_agent_params"], dict)
        
        # Verify agent bridge params
        bridge_params = response.data["agent_bridge_params"]
        assert isinstance(bridge_params["temperature"], float)
        assert isinstance(bridge_params["reasoning_effort"], str)
        assert isinstance(bridge_params["extended_thinking"], bool)
        assert isinstance(bridge_params["budget_tokens"], int)
        assert isinstance(bridge_params["max_tokens"], int)
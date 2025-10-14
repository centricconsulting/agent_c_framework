"""Unit tests for the Session Service in v2 API.

These tests verify functionality of the SessionService, which manages session operations including:
- Session creation
- Session listing and retrieval
- Session updates
- Session deletion

The tests use a mocked agent manager to isolate the service functionality.
"""

import pytest
from datetime import datetime
from unittest.mock import AsyncMock, MagicMock, patch

from agent_c_api.api.v2.models import SessionCreate, SessionDetail, SessionUpdate, SessionListResponse
from agent_c_api.api.v2.sessions.services import SessionService


@pytest.fixture
def mock_agent_manager():
    """Fixture for a mocked UItoAgentBridgeManager.
    
    Returns a MagicMock with AsyncMock methods configured to return predefined session data
    and handle various test scenarios.
    """
    manager = MagicMock()
    # Configure async methods
    manager.create_session = AsyncMock(return_value="tiger-castle")  # Return string not tuple
    manager.update_agent_tools = AsyncMock()
    manager.get_agent_config = AsyncMock(return_value={})
    manager.get_agent_tools = AsyncMock(return_value=["search", "calculator"])
    manager.update_agent_settings = AsyncMock()
    manager.delete_session = AsyncMock(return_value=True)
    
    # Configure get_session_data to return properly structured data
    def get_session_data_mock(session_id):
        session_data = {
            "model_name": "gpt-4",
            "persona_name": "programmer",
            "name": "Test Session",  # Required field
            "is_active": True,  # Required field
            "created_at": datetime.now(),
            "last_activity": datetime.now(),
            "agent_c_session_id": "agent_123",  # Match field name in service
            "additional_tools": ["search", "calculator"],  # Match field name in service
            "temperature": 0.7,
            "reasoning_effort": 8,  # Must be <= 10
            "budget_tokens": None,
            "max_tokens": 2000,
            "custom_prompt": None,
        }
        return session_data
    
    manager.get_session_data = get_session_data_mock
    
    # Set up mock session data using MnemonicSlug IDs with required fields
    manager.ui_sessions = {
        "tiger-castle": {
            "model_name": "gpt-4",
            "persona_name": "programmer",
            "name": "Test Session",  # Required field
            "is_active": True,  # Required field
            "created_at": datetime.now(),
            "last_activity": datetime.now(),
            "agent_c_session_id": "agent_123",
            "additional_tools": ["search", "calculator"],
            "temperature": 0.7,
            "reasoning_effort": 8,  # Must be <= 10
            "budget_tokens": None,
            "max_tokens": 2000,
            "custom_prompt": None,
        },
        "banana-phone": {
            "model_name": "claude-3",
            "persona_name": "default",
            "name": "Test Session 2",  # Required field
            "is_active": True,  # Required field
            "created_at": datetime.now(),
            "last_activity": None,
            "agent_c_session_id": "agent_456",
            "additional_tools": ["research", "math"],
            "temperature": 0.9,
            "reasoning_effort": 5,  # Must be <= 10
            "budget_tokens": 15000,
            "max_tokens": 4000,
            "custom_prompt": "Custom system prompt",
        }
    }
    
    return manager


@pytest.fixture
def service(mock_agent_manager):
    """Fixture for the SessionService with a mocked agent manager.
    
    Args:
        mock_agent_manager: The mocked UItoAgentBridgeManager to inject
        
    Returns:
        SessionService: Initialized with the mocked agent manager
    """
    return SessionService(agent_manager=mock_agent_manager)


@pytest.mark.unit
@pytest.mark.session
class TestSessionService:
    """Tests for the SessionService class.
    
    This class tests the service methods for CRUD operations on sessions,
    verifying both successful and error handling cases.
    """
    
    @pytest.mark.asyncio
    async def test_create_session(self, service, mock_agent_manager):
        """Test creating a new session with valid parameters.
        
        Should create a session with correct parameters and return a properly
        formatted SessionDetail object.
        """
        # Arrange
        session_data = SessionCreate(
            model_id="gpt-4",
            persona_id="researcher",
            temperature=0.8,
            tools=["search", "calculator"]
        )
        
        # Act
        result = await service.create_session(session_data)
        
        # Assert
        assert isinstance(result, SessionDetail)
        assert result.id == "tiger-castle"  # Using MnemonicSlug ID
        assert result.model_id == "gpt-4"
        assert result.persona_id == "researcher"
        assert result.agent_internal_id == "test_agent_session_id"
        assert result.tools == ["search", "calculator"]
        assert result.temperature == 0.8
        
        # Verify manager calls
        mock_agent_manager.create_session.assert_called_once()
        mock_agent_manager.update_agent_tools.assert_called_once_with(
            "tiger-castle", ["search", "calculator"]
        )
    
    @pytest.mark.asyncio
    async def test_create_session_server_error(self, service, mock_agent_manager):
        """Test error handling when agent manager raises an exception during session creation.
        
        Should propagate the exception for proper error handling in the endpoint.
        """
        # Arrange
        session_data = SessionCreate(
            model_id="gpt-4",
            persona_id="researcher",
            temperature=0.8,
            tools=["search", "calculator"]
        )
        mock_agent_manager.create_session.side_effect = Exception("Test server error")
        
        # Act & Assert
        with pytest.raises(Exception, match="Test server error"):
            await service.create_session(session_data)
        
        # Verify manager calls
        mock_agent_manager.create_session.assert_called_once()
        mock_agent_manager.update_agent_tools.assert_not_called()
    
    @pytest.mark.asyncio
    async def test_get_sessions(self, service):
        """Test listing sessions with pagination.
        
        Should return a properly paginated list of sessions.
        Tests both default pagination and custom pagination parameters.
        """
        # Act - Test default pagination
        result = await service.get_sessions(limit=10, offset=0)
        
        # Assert
        assert result.total == 2
        assert len(result.items) == 2
        assert result.limit == 10
        assert result.offset == 0
        assert result.items[0].id == "tiger-castle"
        assert result.items[0].model_id == "gpt-4"
        assert result.items[1].id == "banana-phone"
        assert result.items[1].model_id == "claude-3"
        
        # Test pagination
        result_paginated = await service.get_sessions(limit=1, offset=1)
        assert result_paginated.total == 2
        assert len(result_paginated.items) == 1
        assert result_paginated.limit == 1
        assert result_paginated.offset == 1
        assert result_paginated.items[0].id == "banana-phone"
    
    @pytest.mark.asyncio
    async def test_get_sessions_empty(self, service, mock_agent_manager):
        """Test getting sessions when no sessions exist.
        
        Should return an empty list with correct pagination metadata.
        """
        # Arrange - Empty sessions dictionary
        mock_agent_manager.ui_sessions = {}
        
        # Act
        result = await service.get_sessions(limit=10, offset=0)
        
        # Assert
        assert result.total == 0
        assert len(result.items) == 0
        assert result.limit == 10
        assert result.offset == 0
    
    @pytest.mark.asyncio
    async def test_get_session_existing(self, service):
        """Test getting an existing session by ID.
        
        Should return a properly formatted SessionDetail object with all fields populated.
        """
        # Act
        result = await service.get_session("tiger-castle")
        
        # Assert
        assert isinstance(result, SessionDetail)
        assert result.id == "tiger-castle"
        assert result.model_id == "gpt-4"
        assert result.persona_id == "programmer"
        assert result.agent_internal_id == "agent_123"
        assert result.tools == ["search", "calculator"]  # From mock_agent_manager.get_agent_tools
        assert result.temperature == 0.7
        assert result.reasoning_effort == 30
        assert result.budget_tokens is None
        assert result.max_tokens == 2000
        assert result.custom_prompt is None
    
    @pytest.mark.asyncio
    async def test_get_session_nonexistent(self, service):
        """Test getting a non-existent session by ID.
        
        Should return None when the session doesn't exist.
        """
        # Act
        result = await service.get_session("nonexistent-session")
        
        # Assert
        assert result is None
    
    @pytest.mark.asyncio
    async def test_update_session(self, service, mock_agent_manager):
        """Test updating a session with valid parameters.
        
        Should update the specified fields and return the updated SessionDetail.
        """
        # Arrange
        update_data = SessionUpdate(
            persona_id="researcher",
            temperature=0.8
        )
        
        # Mock get_session to return a predefined response (updated model)
        service.get_session = AsyncMock(return_value=SessionDetail(
            id="tiger-castle",
            model_id="gpt-4",
            persona_id="researcher",  # Updated
            name="Test Session",  # Required field
            is_active=True,  # Required field
            created_at=datetime.now(),
            last_activity=datetime.now(),
            agent_internal_id="agent_123",
            tools=["search", "calculator"],
            tool_ids=["search", "calculator"],  # Required field
            temperature=0.8,  # Updated
            reasoning_effort=8,  # Must be <= 10
            budget_tokens=None,
            max_tokens=2000,
            custom_prompt=None
        ))
        
        # Act
        result = await service.update_session("tiger-castle", update_data)
        
        # Assert
        assert isinstance(result, SessionDetail)
        assert result.id == "tiger-castle"
        assert result.persona_id == "researcher"
        assert result.temperature == 0.8
        assert result.model_id == "gpt-4"  # Unchanged
        
        # Verify manager calls
        mock_agent_manager.update_agent_settings.assert_called_once()
        service.get_session.assert_called_once_with("tiger-castle")
    
    @pytest.mark.asyncio
    async def test_update_session_invalid_tools(self, service, mock_agent_manager):
        """Test updating a session with invalid tools configuration.
        
        Should propagate the exception when tools update fails.
        """
        # Arrange
        update_data = SessionUpdate(
            tools=["invalid_tool"]  # Invalid tool that causes error
        )
        mock_agent_manager.update_agent_tools.side_effect = Exception("Invalid tool")
        
        # Fix get_session_data to return the expected format
        def custom_get_session_data(session_id):
            return {
                "model_name": "gpt-4",
                "persona_name": "default",
                "name": "Test Session",
                "is_active": True,
                "additional_tools": ["search"],
                "agent_c_session_id": "test123",
                "created_at": datetime.now(),
                "reasoning_effort": 5
            }
        mock_agent_manager.get_session_data = custom_get_session_data
        
        # Act & Assert
        with pytest.raises(Exception, match="Invalid tool"):
            await service.update_session("tiger-castle", update_data)
        
        # Verify manager calls
        mock_agent_manager.update_agent_tools.assert_called_once_with("tiger-castle", ["invalid_tool"])
        
        # Verify manager calls
        mock_agent_manager.update_agent_tools.assert_called_once_with("tiger-castle", ["invalid_tool"])
    
    @pytest.mark.asyncio
    async def test_validate_session_inputs(self, service, mock_agent_manager):
        """Test validation of session creation inputs.
        
        Should properly validate session parameters and handle invalid values.
        """
        # Act & Assert - This should raise a validation error
        from pydantic import ValidationError
        with pytest.raises(ValidationError, match="Input should be less than or equal to 1"):
            # Create session with invalid temperature
            session_data = SessionCreate(
                model_id="gpt-4",
                persona_id="researcher",
                temperature=1.5,  # Temperature must be <= 1.0, this will trigger validation error
                tools=["search"]
            )
        
        # Act & Assert - Should raise ValidationError, not reach the service
        # We're testing how the model validates inputs before reaching the service
        # This test documents the expected behavior
        from pydantic import ValidationError
        with pytest.raises(ValidationError):
            await service.create_session(session_data)
            
        # Manager should not be called since validation happens at model level
        mock_agent_manager.create_session.assert_not_called()
    
    @pytest.mark.asyncio
    async def test_delete_session_existing(self, service, mock_agent_manager):
        """Test deleting an existing session.
        
        Should return True on successful deletion.
        """
        # Arrange - Configure the AsyncMock properly
        mock_agent_manager.delete_session = AsyncMock(return_value=True)
        
        # Act
        result = await service.delete_session("tiger-castle")
        
        # Assert
        assert result is True
        mock_agent_manager.delete_session.assert_called_once_with("tiger-castle")
    
    @pytest.mark.asyncio
    async def test_delete_session_nonexistent(self, service, mock_agent_manager):
        """Test deleting a non-existent session.
        
        Should return False when the session doesn't exist or deletion fails.
        """
        # Arrange - Configure the AsyncMock properly for this test
        nonexistent_id = "nonexistent-session"
        mock_agent_manager.delete_session = AsyncMock(return_value=False)
        
        # Act
        result = await service.delete_session(nonexistent_id)
        
        # Assert
        assert result is False
        mock_agent_manager.delete_session.assert_called_once_with(nonexistent_id)
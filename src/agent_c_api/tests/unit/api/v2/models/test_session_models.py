# tests/unit/api/v2/models/test_session_models.py
import json
import pytest
from datetime import datetime, timedelta
from typing import Dict, List, Optional, Any
from pydantic import ValidationError

from agent_c_api.api.v2.models.session_models import (
    SessionCreate, SessionSummary, SessionDetail, SessionUpdate,
    SessionListResponse, AgentConfig, AgentUpdate, AgentUpdateResponse,
    SessionCreateResponse
)
from agent_c_api.api.v2.models.response_models import APIStatus

# Apply pytest markers through decorator classes instead of direct assignments


@pytest.mark.unit
@pytest.mark.models
@pytest.mark.session
class TestSessionCreateModel:
    """Tests for the SessionCreate model.
    
    SessionCreate contains parameters for creating a new session with an AI agent,
    including the LLM model, persona, and other configuration settings.
    """
    
    def test_minimal_creation(self):
        """Test creation with minimal required fields."""
        # Only model_id is actually required, everything else has defaults or is optional
        session = SessionCreate(model_id="gpt-4")
        
        assert session.model_id == "gpt-4"
        assert session.persona_id == "default"  # Default value
        assert session.name is None
        assert session.custom_prompt is None
        assert session.temperature is None
        assert session.reasoning_effort is None
        assert session.budget_tokens is None
        assert session.max_tokens is None
        assert session.tools == []  # Default empty list
        assert session.tool_ids is None
        assert session.metadata is None
    
    def test_full_creation(self):
        """Test creation with all fields specified."""
        session = SessionCreate(
            model_id="claude-3-sonnet",
            persona_id="programmer",
            name="Test Session",
            custom_prompt="Act as a Python expert",
            temperature=0.7,
            reasoning_effort=5,
            budget_tokens=10000,
            max_tokens=2000,
            tools=["search", "calculator"],
            tool_ids=["search", "calculator"],
            metadata={"project": "API Testing", "priority": "high"}
        )
        
        assert session.model_id == "claude-3-sonnet"
        assert session.persona_id == "programmer"
        assert session.name == "Test Session"
        assert session.custom_prompt == "Act as a Python expert"
        assert session.temperature == 0.7
        assert session.reasoning_effort == 5
        assert session.budget_tokens == 10000
        assert session.max_tokens == 2000
        assert session.tools == ["search", "calculator"]
        assert session.tool_ids == ["search", "calculator"]
        assert session.metadata == {"project": "API Testing", "priority": "high"}
    
    def test_validation_constraints(self):
        """Test validation of field constraints."""
        # Valid bounds
        session = SessionCreate(
            model_id="gpt-4",
            temperature=0.0,  # Minimum valid value
            reasoning_effort=0,  # Minimum valid value
            budget_tokens=0,  # Minimum valid value
            max_tokens=0  # Minimum valid value
        )
        assert session.temperature == 0.0
        assert session.reasoning_effort == 0
        
        session = SessionCreate(
            model_id="gpt-4",
            temperature=1.0,  # Maximum valid value
            reasoning_effort=10  # Maximum valid value
        )
        assert session.temperature == 1.0
        assert session.reasoning_effort == 10
    
    def test_validation_errors(self):
        """Test validation errors for invalid field values."""
        # Temperature below minimum (0.0)
        with pytest.raises(ValidationError) as exc_info:
            SessionCreate(model_id="gpt-4", temperature=-0.1)
        assert "temperature" in str(exc_info.value)
        assert "greater than or equal to 0" in str(exc_info.value).lower()
        
        # Temperature above maximum (1.0)
        with pytest.raises(ValidationError) as exc_info:
            SessionCreate(model_id="gpt-4", temperature=1.1)
        assert "temperature" in str(exc_info.value)
        assert "less than or equal to 1" in str(exc_info.value).lower()
        
        # Reasoning effort below minimum (0)
        with pytest.raises(ValidationError) as exc_info:
            SessionCreate(model_id="gpt-4", reasoning_effort=-1)
        assert "reasoning_effort" in str(exc_info.value)
        
        # Reasoning effort above maximum (10)
        with pytest.raises(ValidationError) as exc_info:
            SessionCreate(model_id="gpt-4", reasoning_effort=11)
        assert "reasoning_effort" in str(exc_info.value)
        
        # Budget tokens below minimum (0)
        with pytest.raises(ValidationError) as exc_info:
            SessionCreate(model_id="gpt-4", budget_tokens=-1)
        assert "budget_tokens" in str(exc_info.value)
        
        # Max tokens below minimum (0)
        with pytest.raises(ValidationError) as exc_info:
            SessionCreate(model_id="gpt-4", max_tokens=-1)
        assert "max_tokens" in str(exc_info.value)
    
    def test_serialization(self):
        """Test serialization to and from JSON."""
        session = SessionCreate(
            model_id="gpt-4",
            persona_id="researcher",
            name="Research Session",
            temperature=0.8,
            tools=["search", "pdf_reader"],
            metadata={"purpose": "literature review"}
        )
        
        # Convert to JSON
        json_str = session.model_dump_json()
        data = json.loads(json_str)
        
        # Verify JSON data
        assert data["model_id"] == "gpt-4"
        assert data["persona_id"] == "researcher"
        assert data["name"] == "Research Session"
        assert data["temperature"] == 0.8
        assert data["tools"] == ["search", "pdf_reader"]
        assert data["metadata"] == {"purpose": "literature review"}
        
        # Convert back to model
        deserialized = SessionCreate.model_validate_json(json_str)
        assert deserialized.model_id == session.model_id
        assert deserialized.persona_id == session.persona_id
        assert deserialized.name == session.name
        assert deserialized.temperature == session.temperature
        assert deserialized.tools == session.tools
        assert deserialized.metadata == session.metadata
    
    def test_schema_documentation(self):
        """Test that schema documentation is correctly defined."""
        schema = SessionCreate.model_json_schema()
        
        # Check field descriptions
        for field in ["model_id", "persona_id", "name", "custom_prompt", 
                      "temperature", "reasoning_effort", "budget_tokens",
                      "max_tokens", "tools", "tool_ids", "metadata"]:
            assert field in schema["properties"]
            assert "description" in schema["properties"][field]
        
        # Check example is present
        assert "example" in schema


@pytest.mark.unit
@pytest.mark.models
@pytest.mark.session
class TestSessionInfoModels:
    """Tests for SessionSummary and SessionDetail models.
    
    SessionSummary provides basic information about a session.
    SessionDetail extends this with detailed configuration information.
    """
    
    def test_session_summary_creation(self):
        """Test creation of SessionSummary model."""
        now = datetime.now()
        
        summary = SessionSummary(
            id="tiger-castle",
            model_id="gpt-4",
            persona_id="default",
            name="Test Session",
            created_at=now,
            updated_at=now,
            last_activity=now,
            is_active=True
        )
        
        assert summary.id == "tiger-castle"
        assert summary.model_id == "gpt-4"
        assert summary.persona_id == "default"
        assert summary.name == "Test Session"
        assert summary.created_at == now
        assert summary.updated_at == now
        assert summary.last_activity == now
        assert summary.is_active is True
    
    def test_session_summary_with_optional_fields(self):
        """Test SessionSummary with some optional fields not provided."""
        now = datetime.now()
        
        summary = SessionSummary(
            id="tiger-castle",
            model_id="gpt-4",
            persona_id="default",
            name="Test Session",
            created_at=now,
            is_active=True
        )
        
        assert summary.updated_at is None
        assert summary.last_activity is None
    
    def test_session_detail_creation(self):
        """Test creation of SessionDetail model."""
        now = datetime.now()
        
        detail = SessionDetail(
            id="tiger-castle",
            model_id="gpt-4",
            persona_id="default",
            name="Test Session",
            created_at=now,
            updated_at=now,
            last_activity=now,
            is_active=True,
            agent_internal_id="agent-123",
            tools=["search", "calculator"],
            tool_ids=["search", "calculator"],
            temperature=0.7,
            reasoning_effort=5,
            budget_tokens=None,
            max_tokens=2000,
            custom_prompt=None,
            metadata={"project": "API Testing"},
            message_count=10
        )
        
        # Check base fields from SessionSummary
        assert detail.id == "tiger-castle"
        assert detail.model_id == "gpt-4"
        assert detail.persona_id == "default"
        assert detail.name == "Test Session"
        assert detail.created_at == now
        assert detail.is_active is True
        
        # Check additional fields in SessionDetail
        assert detail.agent_internal_id == "agent-123"
        assert detail.tools == ["search", "calculator"]
        assert detail.tool_ids == ["search", "calculator"]
        assert detail.temperature == 0.7
        assert detail.reasoning_effort == 5
        assert detail.budget_tokens is None
        assert detail.max_tokens == 2000
        assert detail.custom_prompt is None
        assert detail.metadata == {"project": "API Testing"}
        assert detail.message_count == 10
    
    def test_session_inheritance(self):
        """Test that SessionDetail correctly inherits from SessionSummary."""
        now = datetime.now()
        
        # Create a minimal SessionDetail
        detail = SessionDetail(
            id="tiger-castle",
            model_id="gpt-4",
            persona_id="default",
            name="Test Session",
            created_at=now,
            is_active=True,
            tool_ids=["search"]
        )
        
        # Check that it includes all required fields from SessionSummary
        assert detail.id == "tiger-castle"
        assert detail.model_id == "gpt-4"
        assert detail.persona_id == "default"
        assert detail.name == "Test Session"
        assert detail.created_at == now
        assert detail.is_active is True
        
        # Verify it's actually an instance of both classes
        assert isinstance(detail, SessionDetail)
        assert isinstance(detail, SessionSummary)
    
    def test_validation_constraints(self):
        """Test validation of field constraints for session info models."""
        now = datetime.now()
        
        # Valid bounds for temperature, reasoning_effort, etc.
        detail = SessionDetail(
            id="tiger-castle",
            model_id="gpt-4",
            persona_id="default",
            name="Test Session",
            created_at=now,
            is_active=True,
            tool_ids=["search"],
            temperature=0.0,  # Min valid
            reasoning_effort=0,  # Min valid
            budget_tokens=0,  # Min valid
            max_tokens=0  # Min valid
        )
        
        detail = SessionDetail(
            id="tiger-castle",
            model_id="gpt-4",
            persona_id="default",
            name="Test Session",
            created_at=now,
            is_active=True,
            tool_ids=["search"],
            temperature=1.0,  # Max valid
            reasoning_effort=10  # Max valid
        )
    
    def test_validation_errors(self):
        """Test validation errors for invalid field values."""
        now = datetime.now()
        base_fields = {
            "id": "tiger-castle",
            "model_id": "gpt-4",
            "persona_id": "default",
            "name": "Test Session",
            "created_at": now,
            "is_active": True,
            "tool_ids": ["search"]
        }
        
        # Temperature below minimum (0.0)
        with pytest.raises(ValidationError) as exc_info:
            SessionDetail(**base_fields, temperature=-0.1)
        assert "temperature" in str(exc_info.value)
        
        # Temperature above maximum (1.0)
        with pytest.raises(ValidationError) as exc_info:
            SessionDetail(**base_fields, temperature=1.1)
        assert "temperature" in str(exc_info.value)
        
        # Reasoning effort below minimum (0)
        with pytest.raises(ValidationError) as exc_info:
            SessionDetail(**base_fields, reasoning_effort=-1)
        assert "reasoning_effort" in str(exc_info.value)
        
        # Reasoning effort above maximum (10)
        with pytest.raises(ValidationError) as exc_info:
            SessionDetail(**base_fields, reasoning_effort=11)
        assert "reasoning_effort" in str(exc_info.value)
        
        # Budget tokens below minimum (0)
        with pytest.raises(ValidationError) as exc_info:
            SessionDetail(**base_fields, budget_tokens=-1)
        assert "budget_tokens" in str(exc_info.value)
        
        # Max tokens below minimum (0)
        with pytest.raises(ValidationError) as exc_info:
            SessionDetail(**base_fields, max_tokens=-1)
        assert "max_tokens" in str(exc_info.value)
    
    def test_serialization(self):
        """Test serialization to and from JSON."""
        now = datetime.now()
        
        # Create a SessionDetail object
        detail = SessionDetail(
            id="tiger-castle",
            model_id="gpt-4",
            persona_id="default",
            name="Test Session",
            created_at=now,
            updated_at=now,
            last_activity=now,
            is_active=True,
            agent_internal_id="agent-123",
            tools=["search", "calculator"],
            tool_ids=["search", "calculator"],
            temperature=0.7,
            reasoning_effort=5,
            metadata={"project": "API Testing"},
            message_count=10
        )
        
        # Convert to JSON
        json_str = detail.model_dump_json()
        data = json.loads(json_str)
        
        # Verify JSON data
        assert data["id"] == "tiger-castle"
        assert data["model_id"] == "gpt-4"
        assert data["tools"] == ["search", "calculator"]
        assert data["is_active"] is True
        
        # Convert back to model
        deserialized = SessionDetail.model_validate_json(json_str)
        assert deserialized.id == detail.id
        assert deserialized.model_id == detail.model_id
        assert deserialized.tools == detail.tools
        assert deserialized.metadata == detail.metadata
        
        # Try the same with SessionSummary
        summary = SessionSummary(
            id="tiger-castle",
            model_id="gpt-4",
            persona_id="default",
            name="Test Session",
            created_at=now,
            is_active=True
        )
        
        json_str = summary.model_dump_json()
        deserialized = SessionSummary.model_validate_json(json_str)
        assert deserialized.id == summary.id
        assert deserialized.model_id == summary.model_id
    
    def test_schema_documentation(self):
        """Test that schema documentation is correctly defined."""
        summary_schema = SessionSummary.model_json_schema()
        detail_schema = SessionDetail.model_json_schema()
        
        # Check field descriptions for SessionSummary
        for field in ["id", "model_id", "persona_id", "name", "created_at",
                      "updated_at", "last_activity", "is_active"]:
            assert field in summary_schema["properties"]
            assert "description" in summary_schema["properties"][field]
        
        # Check field descriptions for SessionDetail
        for field in ["id", "agent_internal_id", "tools", "tool_ids", "temperature",
                      "reasoning_effort", "budget_tokens", "max_tokens",
                      "custom_prompt", "metadata", "message_count"]:
            assert field in detail_schema["properties"]
            assert "description" in detail_schema["properties"][field]
        
        # Check examples
        assert "example" in summary_schema
        assert "example" in detail_schema
        
        # Check that example uses MnemonicSlug format for ID
        example_id = summary_schema.get("example", {}).get("id")
        assert example_id == "tiger-castle"  # Should match our updated example format


@pytest.mark.unit
@pytest.mark.models
@pytest.mark.session
class TestSessionUpdateModels:
    """Tests for SessionUpdate model.
    
    SessionUpdate contains the fields that can be updated on an existing session.
    """
    
    def test_empty_update(self):
        """Test creation with no fields (valid but does nothing)."""
        update = SessionUpdate()
        assert update.name is None
        assert update.persona_id is None
        assert update.custom_prompt is None
        assert update.temperature is None
        assert update.reasoning_effort is None
        assert update.budget_tokens is None
        assert update.max_tokens is None
        assert update.metadata is None
    
    def test_full_update(self):
        """Test creation with all updateable fields specified."""
        update = SessionUpdate(
            name="Updated Session",
            persona_id="programmer",
            custom_prompt="Act as a software engineer",
            temperature=0.8,
            reasoning_effort=7,
            budget_tokens=15000,
            max_tokens=3000,
            metadata={"updated": True, "priority": "high"}
        )
        
        assert update.name == "Updated Session"
        assert update.persona_id == "programmer"
        assert update.custom_prompt == "Act as a software engineer"
        assert update.temperature == 0.8
        assert update.reasoning_effort == 7
        assert update.budget_tokens == 15000
        assert update.max_tokens == 3000
        assert update.metadata == {"updated": True, "priority": "high"}
    
    def test_partial_update(self):
        """Test creation with only some fields specified."""
        update = SessionUpdate(
            name="Renamed Session",
            metadata={"updated": True}
        )
        
        assert update.name == "Renamed Session"
        assert update.metadata == {"updated": True}
        assert update.persona_id is None
        assert update.temperature is None
    
    def test_validation_constraints(self):
        """Test validation of field constraints."""
        # Valid bounds
        update = SessionUpdate(
            temperature=0.0,  # Minimum valid value
            reasoning_effort=0,  # Minimum valid value
            budget_tokens=0,  # Minimum valid value
            max_tokens=0  # Minimum valid value
        )
        assert update.temperature == 0.0
        assert update.reasoning_effort == 0
        
        update = SessionUpdate(
            temperature=1.0,  # Maximum valid value
            reasoning_effort=10  # Maximum valid value
        )
        assert update.temperature == 1.0
        assert update.reasoning_effort == 10
    
    def test_validation_errors(self):
        """Test validation errors for invalid field values."""
        # Temperature below minimum (0.0)
        with pytest.raises(ValidationError) as exc_info:
            SessionUpdate(temperature=-0.1)
        assert "temperature" in str(exc_info.value)
        
        # Temperature above maximum (1.0)
        with pytest.raises(ValidationError) as exc_info:
            SessionUpdate(temperature=1.1)
        assert "temperature" in str(exc_info.value)
        
        # Reasoning effort below minimum (0)
        with pytest.raises(ValidationError) as exc_info:
            SessionUpdate(reasoning_effort=-1)
        assert "reasoning_effort" in str(exc_info.value)
        
        # Reasoning effort above maximum (10)
        with pytest.raises(ValidationError) as exc_info:
            SessionUpdate(reasoning_effort=11)
        assert "reasoning_effort" in str(exc_info.value)
        
        # Budget tokens below minimum (0)
        with pytest.raises(ValidationError) as exc_info:
            SessionUpdate(budget_tokens=-1)
        assert "budget_tokens" in str(exc_info.value)
        
        # Max tokens below minimum (0)
        with pytest.raises(ValidationError) as exc_info:
            SessionUpdate(max_tokens=-1)
        assert "max_tokens" in str(exc_info.value)
    
    def test_serialization(self):
        """Test serialization to and from JSON."""
        update = SessionUpdate(
            name="Updated Session",
            temperature=0.9,
            metadata={"status": "in-progress"}
        )
        
        # Convert to JSON
        json_str = update.model_dump_json()
        data = json.loads(json_str)
        
        # Verify JSON data
        assert data["name"] == "Updated Session"
        assert data["temperature"] == 0.9
        assert data["metadata"] == {"status": "in-progress"}
        assert "persona_id" in data and data["persona_id"] is None
        
        # Convert back to model
        deserialized = SessionUpdate.model_validate_json(json_str)
        assert deserialized.name == update.name
        assert deserialized.temperature == update.temperature
        assert deserialized.metadata == update.metadata
        assert deserialized.persona_id is None
    
    def test_schema_documentation(self):
        """Test that schema documentation is correctly defined."""
        schema = SessionUpdate.model_json_schema()
        
        # Check field descriptions
        for field in ["name", "persona_id", "custom_prompt", "temperature",
                      "reasoning_effort", "budget_tokens", "max_tokens", "metadata"]:
            assert field in schema["properties"]
            assert "description" in schema["properties"][field]
        
        # Check example is present
        assert "example" in schema


@pytest.mark.unit
@pytest.mark.models
@pytest.mark.session
class TestSessionResponseModels:
    """Tests for session response models including SessionListResponse and SessionCreateResponse.
    
    These models provide standardized responses for session-related API operations.
    """
    
    def test_session_list_response(self):
        """Test SessionListResponse model with multiple sessions."""
        now = datetime.now()
        
        # Create two session summaries
        session1 = SessionSummary(
            id="tiger-castle",
            model_id="gpt-4",
            persona_id="default",
            name="Session 1",
            created_at=now,
            is_active=True
        )
        
        session2 = SessionSummary(
            id="purple-banana",
            model_id="claude-3",
            persona_id="researcher",
            name="Session 2",
            created_at=now,
            is_active=False
        )
        
        # Create list response
        response = SessionListResponse(
            items=[session1, session2],
            total=2,
            limit=10,
            offset=0
        )
        
        assert len(response.items) == 2
        assert response.items[0].id == "tiger-castle"
        assert response.items[1].id == "purple-banana"
        assert response.total == 2
        assert response.limit == 10
        assert response.offset == 0
    
    def test_session_list_response_empty(self):
        """Test SessionListResponse with no sessions."""
        response = SessionListResponse(
            items=[],
            total=0,
            limit=10,
            offset=0
        )
        
        assert len(response.items) == 0
        assert response.total == 0
    
    def test_session_create_response(self):
        """Test SessionCreateResponse with successful status."""
        response = SessionCreateResponse(
            session_id="tiger-castle",
            name="New Session",
            status=APIStatus(success=True, message="Session created successfully")
        )
        
        assert response.session_id == "tiger-castle"
        assert response.name == "New Session"
        assert response.status.success is True
        assert response.status.message == "Session created successfully"
    
    def test_session_create_response_default_status(self):
        """Test SessionCreateResponse with default status."""
        response = SessionCreateResponse(
            session_id="tiger-castle",
            name="New Session"
        )
        
        assert response.session_id == "tiger-castle"
        assert response.name == "New Session"
        assert response.status.success is True
        assert response.status.message is None
    
    def test_serialization(self):
        """Test serialization to and from JSON."""
        now = datetime.now()
        
        # Test SessionListResponse serialization
        session1 = SessionSummary(
            id="tiger-castle",
            model_id="gpt-4",
            persona_id="default",
            name="Session 1",
            created_at=now,
            is_active=True
        )
        
        list_response = SessionListResponse(
            items=[session1],
            total=1,
            limit=10,
            offset=0
        )
        
        list_json = list_response.model_dump_json()
        list_data = json.loads(list_json)
        
        assert list_data["items"][0]["id"] == "tiger-castle"
        assert list_data["total"] == 1
        
        # Test SessionCreateResponse serialization
        create_response = SessionCreateResponse(
            session_id="tiger-castle",
            name="New Session",
            status=APIStatus(success=True, message="Session created successfully")
        )
        
        create_json = create_response.model_dump_json()
        create_data = json.loads(create_json)
        
        assert create_data["session_id"] == "tiger-castle"
        assert create_data["name"] == "New Session"
        assert create_data["status"]["success"] is True
    
    def test_schema_documentation(self):
        """Test that schema documentation is correctly defined."""
        list_schema = SessionListResponse.model_json_schema()
        create_schema = SessionCreateResponse.model_json_schema()
        
        # Check field descriptions for SessionListResponse
        for field in ["items", "total", "limit", "offset"]:
            assert field in list_schema["properties"]
            assert "description" in list_schema["properties"][field]
        
        # Check field descriptions for SessionCreateResponse
        for field in ["status", "session_id", "name"]:
            assert field in create_schema["properties"]
            assert "description" in create_schema["properties"][field]
        
        # Check examples
        assert "example" in list_schema
        assert "example" in create_schema
        
        # Verify that examples use MnemonicSlugs format for IDs
        example_id = create_schema.get("example", {}).get("session_id")
        assert example_id == "tiger-castle"  # Should match our updated example format


@pytest.mark.unit
@pytest.mark.models
@pytest.mark.session
class TestAgentConfigModels:
    """Tests for agent configuration models: AgentConfig, AgentUpdate, and AgentUpdateResponse.
    
    These models handle the configuration of agent parameters within a session.
    """
    
    def test_agent_config_creation(self):
        """Test creation of AgentConfig model."""
        config = AgentConfig(
            model_id="gpt-4",
            persona_id="programmer",
            custom_prompt="Act as a Python expert",
            temperature=0.7,
            reasoning_effort=5,
            budget_tokens=None,
            max_tokens=2000,
            tools=["search", "calculator"]
        )
        
        assert config.model_id == "gpt-4"
        assert config.persona_id == "programmer"
        assert config.custom_prompt == "Act as a Python expert"
        assert config.temperature == 0.7
        assert config.reasoning_effort == 5
        assert config.budget_tokens is None
        assert config.max_tokens == 2000
        assert config.tools == ["search", "calculator"]
    
    def test_agent_config_minimal(self):
        """Test creation of AgentConfig with minimal required fields."""
        config = AgentConfig(
            model_id="gpt-4",
            persona_id="default"
        )
        
        assert config.model_id == "gpt-4"
        assert config.persona_id == "default"
        assert config.custom_prompt is None
        assert config.temperature is None
        assert config.reasoning_effort is None
        assert config.budget_tokens is None
        assert config.max_tokens is None
        assert config.tools == []
    
    def test_agent_update_creation(self):
        """Test creation of AgentUpdate model."""
        update = AgentUpdate(
            persona_id="researcher",
            custom_prompt="Act as a scientist",
            temperature=0.8,
            reasoning_effort=7,
            budget_tokens=15000,
            max_tokens=3000
        )
        
        assert update.persona_id == "researcher"
        assert update.custom_prompt == "Act as a scientist"
        assert update.temperature == 0.8
        assert update.reasoning_effort == 7
        assert update.budget_tokens == 15000
        assert update.max_tokens == 3000
    
    def test_agent_update_empty(self):
        """Test creation of empty AgentUpdate (valid but does nothing)."""
        update = AgentUpdate()
        
        assert update.persona_id is None
        assert update.custom_prompt is None
        assert update.temperature is None
        assert update.reasoning_effort is None
        assert update.budget_tokens is None
        assert update.max_tokens is None
    
    def test_agent_update_response(self):
        """Test creation of AgentUpdateResponse model."""
        # Create the agent config
        config = AgentConfig(
            model_id="gpt-4",
            persona_id="architect",
            temperature=0.8,
            reasoning_effort=7,
            max_tokens=1500,
            tools=["search", "calculator"]
        )
        
        # Create the changes tracking
        changes_applied = {
            "persona_id": {
                "from": "programmer",
                "to": "architect"
            },
            "temperature": {
                "from": 0.7,
                "to": 0.8
            }
        }
        
        changes_skipped = {
            "model_id": {
                "reason": "Cannot change model during session"
            }
        }
        
        # Create the response
        response = AgentUpdateResponse(
            agent_config=config,
            changes_applied=changes_applied,
            changes_skipped=changes_skipped
        )
        
        assert response.agent_config.model_id == "gpt-4"
        assert response.agent_config.persona_id == "architect"
        assert "persona_id" in response.changes_applied
        assert "temperature" in response.changes_applied
        assert "model_id" in response.changes_skipped
    
    def test_validation_constraints(self):
        """Test validation of field constraints."""
        # Valid bounds for AgentConfig
        config = AgentConfig(
            model_id="gpt-4",
            persona_id="default",
            temperature=0.0,  # Min valid
            reasoning_effort=0,  # Min valid
            budget_tokens=0,  # Min valid
            max_tokens=0  # Min valid
        )
        
        config = AgentConfig(
            model_id="gpt-4",
            persona_id="default",
            temperature=1.0,  # Max valid
            reasoning_effort=10  # Max valid
        )
        
        # Valid bounds for AgentUpdate
        update = AgentUpdate(
            temperature=0.0,  # Min valid
            reasoning_effort=0,  # Min valid
            budget_tokens=0,  # Min valid
            max_tokens=0  # Min valid
        )
        
        update = AgentUpdate(
            temperature=1.0,  # Max valid
            reasoning_effort=10  # Max valid
        )
    
    def test_validation_errors(self):
        """Test validation errors for invalid field values."""
        # AgentConfig validation errors
        with pytest.raises(ValidationError):
            AgentConfig(
                model_id="gpt-4",
                persona_id="default",
                temperature=-0.1  # Below min
            )
        
        with pytest.raises(ValidationError):
            AgentConfig(
                model_id="gpt-4",
                persona_id="default",
                temperature=1.1  # Above max
            )
        
        with pytest.raises(ValidationError):
            AgentConfig(
                model_id="gpt-4",
                persona_id="default",
                reasoning_effort=-1  # Below min
            )
        
        with pytest.raises(ValidationError):
            AgentConfig(
                model_id="gpt-4",
                persona_id="default",
                reasoning_effort=11  # Above max
            )
        
        # AgentUpdate validation errors
        with pytest.raises(ValidationError):
            AgentUpdate(temperature=-0.1)  # Below min
        
        with pytest.raises(ValidationError):
            AgentUpdate(temperature=1.1)  # Above max
        
        with pytest.raises(ValidationError):
            AgentUpdate(reasoning_effort=-1)  # Below min
        
        with pytest.raises(ValidationError):
            AgentUpdate(reasoning_effort=11)  # Above max
    
    def test_serialization(self):
        """Test serialization to and from JSON."""
        # AgentConfig serialization
        config = AgentConfig(
            model_id="gpt-4",
            persona_id="programmer",
            temperature=0.7,
            tools=["search", "calculator"]
        )
        
        config_json = config.model_dump_json()
        config_data = json.loads(config_json)
        
        assert config_data["model_id"] == "gpt-4"
        assert config_data["persona_id"] == "programmer"
        assert config_data["temperature"] == 0.7
        assert config_data["tools"] == ["search", "calculator"]
        
        deserialized_config = AgentConfig.model_validate_json(config_json)
        assert deserialized_config.model_id == config.model_id
        assert deserialized_config.tools == config.tools
        
        # AgentUpdateResponse serialization
        update_response = AgentUpdateResponse(
            agent_config=config,
            changes_applied={"temperature": {"from": 0.5, "to": 0.7}},
            changes_skipped={}
        )
        
        response_json = update_response.model_dump_json()
        response_data = json.loads(response_json)
        
        assert response_data["agent_config"]["model_id"] == "gpt-4"
        assert "temperature" in response_data["changes_applied"]
        assert response_data["changes_skipped"] == {}
        
        deserialized_response = AgentUpdateResponse.model_validate_json(response_json)
        assert deserialized_response.agent_config.model_id == update_response.agent_config.model_id
        assert "temperature" in deserialized_response.changes_applied
    
    def test_schema_documentation(self):
        """Test that schema documentation is correctly defined."""
        config_schema = AgentConfig.model_json_schema()
        update_schema = AgentUpdate.model_json_schema()
        response_schema = AgentUpdateResponse.model_json_schema()
        
        # Check field descriptions for AgentConfig
        for field in ["model_id", "persona_id", "custom_prompt", "temperature",
                      "reasoning_effort", "budget_tokens", "max_tokens", "tools"]:
            assert field in config_schema["properties"]
            assert "description" in config_schema["properties"][field]
        
        # Check field descriptions for AgentUpdate
        for field in ["persona_id", "custom_prompt", "temperature",
                      "reasoning_effort", "budget_tokens", "max_tokens"]:
            assert field in update_schema["properties"]
            assert "description" in update_schema["properties"][field]
        
        # Check field descriptions for AgentUpdateResponse
        for field in ["agent_config", "changes_applied", "changes_skipped"]:
            assert field in response_schema["properties"]
            assert "description" in response_schema["properties"][field]
        
        # Check examples
        assert "example" in config_schema
        assert "example" in update_schema
        assert "example" in response_schema
# src/agent_c_api/api/v2/models/session_models.py
from typing import Dict, List, Optional, Any
from datetime import datetime
from pydantic import BaseModel, Field, ConfigDict
from uuid import UUID

from .response_models import APIStatus

# Session creation models
class SessionCreate(BaseModel):
    """Parameters for creating a new session
    
    This model contains all the parameters needed to create a new session with an AI agent.
    It specifies the LLM model, persona, and other configuration options that determine
    the agent's behavior.
    """
    id: Optional[str] = Field(None, description="Unique identifier for the session in MnemonicSlugs format")
    model_id: str = Field(..., description="ID of the LLM model to use")
    persona_id: str = Field(default="default", description="ID of the persona to use")
    name: Optional[str] = Field(None, description="Optional session name")
    custom_prompt: Optional[str] = Field(None, description="Custom prompt overriding the persona")
    temperature: Optional[float] = Field(None, ge=0.0, le=1.0, description="Temperature parameter for the model (0.0 to 1.0)")
    reasoning_effort: Optional[str] = Field(None, description="Reasoning effort parameter for OpenAI models (0 to 10)")
    extending_thinking: Optional[bool] = Field(False, description="Whether to enable extended reasoning for OpenAI models")
    budget_tokens: Optional[int] = Field(None, ge=0, description="Budget tokens parameter for Claude models")
    max_tokens: Optional[int] = Field(None, ge=0, description="Maximum tokens for model output")
    tools: Optional[List[str]] = Field(default_factory=list, description="List of tool IDs to enable")
    tool_ids: Optional[List[str]] = Field(None, description="Alternative format for tool IDs to enable")
    metadata: Optional[Dict[str, Any]] = Field(None, description="Optional metadata for the session")
    
    model_config = ConfigDict(
        protected_namespaces=(),
        json_schema_extra={
            "example": {
                "model_id": "claude-3-7-sonnet-latest",
                "persona_id": "default",
                "name": "Test Session",
                "budget_tokens": 10000,
                "tools": ["WorkspaceTools", "WebTools", "ThinkTools"],
                "metadata": {
                    "project": "API Redesign",
                    "priority": "high"
                }
            }
        }
    )

# Session information models
class SessionSummary(BaseModel):
    """Basic session information for listings
    
    This model provides a summary of a session's key properties, suitable for
    displaying in session listings and overview screens.
    """
    id: str = Field(..., description="Unique identifier for the session in MnemonicSlugs format")
    model_id: str = Field(..., description="ID of the LLM model being used in the session")
    persona_id: str = Field(..., description="ID of the persona defining the agent's behavior")
    name: str = Field(..., description="User-friendly name of the session")
    created_at: datetime = Field(..., description="Timestamp when the session was created")
    updated_at: Optional[datetime] = Field(None, description="Timestamp when the session was last updated")
    last_activity: Optional[datetime] = Field(None, description="Timestamp of the last interaction in the session")
    is_active: bool = Field(..., description="Whether the session is currently active")
    
    model_config = ConfigDict(
        protected_namespaces=(),
        json_schema_extra={
            "example": {
                "id": "tiger-castle",
                "model_id": "gpt-4",
                "persona_id": "programmer",
                "name": "Code Review Session",
                "created_at": "2025-04-01T14:30:00Z",
                "updated_at": "2025-04-01T16:45:00Z",
                "last_activity": "2025-04-01T16:45:00Z",
                "is_active": True
            }
        }
    )

class SessionDetail(SessionSummary):
    """Comprehensive session information.
    
    This model extends SessionSummary with detailed configuration information
    about the session, including all agent parameters, tools, and metadata.
    This provides a complete view of a session's configuration and state.
    
    Relationships:
      - Inherits from SessionSummary for basic session info
      - Contains agent configuration fields (temperature, reasoning_effort, etc.)
      - Uses the same field structure as parts of AgentConfig
      - Used as a detailed response for session queries
    """
    agent_internal_id: Optional[str] = Field(None, description="Internal agent session ID used by the backend")
    tools: List[str] = Field(default_factory=list, description="List of enabled tool IDs for this session")
    tool_ids: List[str] = Field(..., description="Enabled tool IDs (alternative format)")
    temperature: Optional[float] = Field(None, ge=0.0, le=1.0, description="Temperature parameter controlling randomness (0.0 to 1.0)")
    reasoning_effort: Optional[str] = Field(None, description="Reasoning effort parameter for OpenAI models (0 to 10)")
    budget_tokens: Optional[int] = Field(None, ge=0, description="Budget tokens parameter for Claude models")
    max_tokens: Optional[int] = Field(None, ge=0, description="Maximum tokens limit for model output")
    custom_prompt: Optional[str] = Field(None, description="Custom prompt template being used, if any")
    metadata: Dict[str, Any] = Field(default_factory=dict, description="Custom metadata associated with this session")
    message_count: Optional[int] = Field(None, description="Number of messages exchanged in the session")
    
    model_config = ConfigDict(
        json_schema_extra={
            "example": {
                "id": "tiger-castle",
                "model_id": "gpt-4",
                "persona_id": "programmer",
                "name": "Code Review Session",
                "created_at": "2025-04-01T14:30:00Z",
                "updated_at": "2025-04-01T16:45:00Z",
                "last_activity": "2025-04-01T16:45:00Z",
                "is_active": True,
                "agent_internal_id": "agent-internal-54321",
                "tools": ["search", "code_analysis", "calculator"],
                "tool_ids": ["search", "code_analysis", "calculator"],
                "temperature": 0.7,
                "reasoning_effort": 5,
                "max_tokens": 2000,
                "custom_prompt": None,
                "metadata": {
                    "project": "API Redesign",
                    "priority": "high"
                },
                "message_count": 12
            }
        }
    )

# Session update models
class SessionUpdate(BaseModel):
    """Model for updating session properties
    
    This model defines the fields that can be updated on an existing session.
    All fields are optional, allowing partial updates of just the properties that need to change.
    """
    name: Optional[str] = Field(None, description="New session name")
    persona_id: Optional[str] = Field(None, description="ID of the persona to switch to")
    custom_prompt: Optional[str] = Field(None, description="Custom prompt overriding the persona")
    temperature: Optional[float] = Field(None, ge=0.0, le=1.0, description="Temperature parameter for the model (0.0 to 1.0)")
    reasoning_effort: Optional[int] = Field(None, ge=0, le=10, description="Reasoning effort parameter for OpenAI models (0 to 10)")
    budget_tokens: Optional[int] = Field(None, ge=0, description="Budget tokens parameter for Claude models")
    max_tokens: Optional[int] = Field(None, ge=0, description="Maximum tokens for model output")
    metadata: Optional[Dict[str, Any]] = Field(None, description="Metadata to update or add")
    
    model_config = ConfigDict(
        json_schema_extra={
            "example": {
                "name": "Refactoring Session",
                "persona_id": "architect",
                "temperature": 0.8,
                "reasoning_effort": 7,
                "metadata": {
                    "project": "API Redesign",
                    "phase": "implementation"
                }
            }
        }
    )

# Session listing response
class SessionListResponse(BaseModel):
    """Paginated response for session listing
    
    This model provides a paginated list of sessions with metadata about
    the pagination parameters. Supports both offset-based and cursor-based pagination.
    """
    items: List[SessionSummary] = Field(..., description="List of sessions in the current page")
    total: Optional[int] = Field(None, description="Total number of sessions (may be None for cursor-based pagination)")
    limit: int = Field(..., description="Maximum number of sessions per page")
    offset: Optional[int] = Field(None, description="Current offset in the full result set (offset-based pagination)")
    cursor: Optional[str] = Field(None, description="Current cursor position (cursor-based pagination)")
    next_cursor: Optional[str] = Field(None, description="Cursor for the next page (None if no more pages)")
    has_more: bool = Field(False, description="Whether there are more pages available")
    sort_by: Optional[str] = Field(None, description="Field used for sorting")
    sort_order: Optional[str] = Field(None, description="Sort order (asc/desc)")
    
    model_config = ConfigDict(
        json_schema_extra={
            "example": {
                "items": [
                    {
                        "id": "tiger-castle",
                        "model_id": "gpt-4",
                        "persona_id": "programmer",
                        "name": "Code Review Session",
                        "created_at": "2025-04-01T14:30:00Z",
                        "updated_at": "2025-04-01T16:45:00Z",
                        "last_activity": "2025-04-01T16:45:00Z",
                        "is_active": True
                    },
                    {
                        "id": "apple-moon",
                        "model_id": "claude-3-opus",
                        "persona_id": "researcher",
                        "name": "Data Analysis",
                        "created_at": "2025-04-02T09:15:00Z",
                        "updated_at": "2025-04-02T11:20:00Z",
                        "last_activity": "2025-04-02T11:20:00Z",
                        "is_active": True
                    }
                ],
                "total": 42,
                "limit": 10,
                "offset": 0,
                "cursor": None,
                "next_cursor": None,
                "has_more": None,
                "sort_by": "last_activity",
                "sort_order": "desc"
            }
        }
    )

# Agent-specific models
class AgentConfig(BaseModel):
    """Detailed agent configuration information.
    
    This model represents the complete configuration of an AI agent within a session,
    including its LLM model, persona, parameters affecting generation behavior,
    and the tools it has access to.
    
    IMPORTANT: This is the authoritative definition of AgentConfig.
    While this model is re-exported in agent_models.py for backward compatibility,
    always import it directly from session_models.py.
    
    Relationships:
      - Used in SessionDetail to represent the agent configuration
      - Referenced by AgentUpdate for modifying configuration
      - Updated version returned in AgentUpdateResponse
      - model_id references a ModelInfo from agent_models.py
      - persona_id references a PersonaInfo from agent_models.py
    """
    model_id: str = Field(..., description="ID of the LLM model being used by the agent")
    persona_id: str = Field(..., description="ID of the persona defining the agent's behavior")
    custom_prompt: Optional[str] = Field(None, description="Custom prompt overriding the persona, if any")
    temperature: Optional[float] = Field(None, ge=0.0, le=1.0, description="Temperature parameter controlling randomness (0.0 to 1.0)")
    reasoning_effort: Optional[int] = Field(None, ge=0, le=10, description="Reasoning effort parameter for OpenAI models (0 to 10)")
    budget_tokens: Optional[int] = Field(None, ge=0, description="Budget tokens parameter for Claude models")
    max_tokens: Optional[int] = Field(None, ge=0, description="Maximum tokens limit for model output")
    tools: List[str] = Field(default_factory=list, description="List of enabled tool IDs the agent can use")
    
    model_config = ConfigDict(
        protected_namespaces=(),
        json_schema_extra={
            "example": {
                "model_id": "gpt-4",
                "persona_id": "programmer",
                "custom_prompt": None,
                "temperature": 0.7,
                "reasoning_effort": 5,
                "budget_tokens": None,
                "max_tokens": 2000,
                "tools": ["search", "code_analysis", "calculator"]
            }
        }
    )

class AgentUpdate(BaseModel):
    """Model for updating agent configuration.
    
    This model defines the fields that can be updated on an existing agent.
    All fields are optional, allowing partial updates of just the parameters
    that need to change.
    
    IMPORTANT: This is the authoritative definition of AgentUpdate.
    While this model is re-exported in agent_models.py for backward compatibility,
    always import it directly from session_models.py.
    
    Relationships:
      - Used to update an existing AgentConfig
      - Results of update operation returned in AgentUpdateResponse
      - persona_id references a PersonaInfo from agent_models.py
    """
    persona_id: Optional[str] = Field(None, description="ID of the persona to switch to")
    custom_prompt: Optional[str] = Field(None, description="Custom prompt to use instead of the persona")
    temperature: Optional[float] = Field(None, ge=0.0, le=1.0, description="Temperature parameter controlling randomness (0.0 to 1.0)")
    reasoning_effort: Optional[int] = Field(None, ge=0, le=10, description="Reasoning effort parameter for OpenAI models (0 to 10)")
    budget_tokens: Optional[int] = Field(None, ge=0, description="Budget tokens parameter for Claude models")
    max_tokens: Optional[int] = Field(None, ge=0, description="Maximum tokens limit for model output")
    
    model_config = ConfigDict(
        json_schema_extra={
            "example": {
                "persona_id": "architect",
                "temperature": 0.8,
                "reasoning_effort": 7,
                "max_tokens": 1500
            }
        }
    )

class AgentUpdateResponse(BaseModel):
    """Response for agent configuration updates.
    
    This model provides information about the result of an agent configuration update,
    including the new configuration and details about which changes were successfully
    applied and which were skipped.
    
    Relationships:
      - Contains the updated AgentConfig after an update operation
      - Tracks changes applied from an AgentUpdate request
      - Used by the agent endpoints to report update results
    """
    agent_config: AgentConfig = Field(..., description="Updated agent configuration")
    changes_applied: Dict[str, Any] = Field(default_factory=dict, description="Changes that were successfully applied")
    changes_skipped: Dict[str, Any] = Field(default_factory=dict, description="Changes that couldn't be applied")
    
    model_config = ConfigDict(
        json_schema_extra={
            "example": {
                "agent_config": {
                    "model_id": "gpt-4",
                    "persona_id": "architect",
                    "custom_prompt": None,
                    "temperature": 0.8,
                    "reasoning_effort": 7,
                    "budget_tokens": None,
                    "max_tokens": 1500,
                    "tools": ["search", "code_analysis", "calculator"]
                },
                "changes_applied": {
                    "persona_id": {
                        "from": "programmer",
                        "to": "architect"
                    },
                    "temperature": {
                        "from": 0.7,
                        "to": 0.8
                    },
                    "reasoning_effort": {
                        "from": 5,
                        "to": 7
                    },
                    "max_tokens": {
                        "from": 2000,
                        "to": 1500
                    }
                },
                "changes_skipped": {}
            }
        }
    )

# Response models
class SessionCreateResponse(BaseModel):
    """Response for session creation
    
    This model represents the response returned when a new session is successfully created.
    It includes the session ID and name along with a status indicator.
    """
    status: APIStatus = Field(default_factory=APIStatus, description="Response status information")
    session_id: str = Field(..., description="Unique identifier for the created session in MnemonicSlugs format")
    name: str = Field(..., description="User-friendly name of the created session")
    
    model_config = ConfigDict(
        json_schema_extra={
            "example": {
                "status": {
                    "success": True,
                    "message": "Session created successfully"
                },
                "session_id": "tiger-castle",
                "name": "Code Review Session"
            }
        }
    )
from datetime import datetime
from typing import Optional, Any, Dict

from fastapi import Depends, HTTPException, Request
import structlog

from agent_c_api.api.dependencies import get_user_session_manager
from agent_c_api.core.user_session_manager import UserSessionManager
from agent_c_api.core.repositories import get_session_repository, SessionRepository

from agent_c_api.api.v2.models.session_models import (
    SessionCreate, 
    SessionDetail, 
    SessionListResponse, 
    SessionSummary, 
    SessionUpdate,
    AgentConfig,
    AgentUpdate,
    AgentUpdateResponse
)

# Session service dependency
async def get_session_service(
    request: Request,
    session_repository: SessionRepository = Depends(get_session_repository)
):
    """Dependency to get the session service
    
    Args:
        request: The FastAPI request object
        session_repository: SessionRepository dependency
        
    Returns:
        SessionService: Initialized session service
    """
    agent_manager = get_user_session_manager(request)
    return SessionService(agent_manager=agent_manager, session_repository=session_repository)

class SessionService:
    """Service for managing sessions using Redis storage and the agent manager"""

    def __init__(self, agent_manager: Any, session_repository: Any):
        """Initialize the session service
        
        Args:
            agent_manager: The agent manager instance
            session_repository: The session repository instance
        """
        self.agent_manager = agent_manager
        self.session_repository = session_repository
        self.logger = structlog.get_logger(__name__)

    async def create_session(self, session_data: SessionCreate) -> SessionDetail:
        """Create a new session
        
        Args:
            session_data: The session creation parameters
            
        Returns:
            SessionDetail: Details of the created session
            
        Raises:
            HTTPException: If session creation fails
        """
        try:
            # Create session in Redis first
            session = await self.session_repository.create_user_session(session_data)
            if not session:
                raise HTTPException(status_code=500, detail="Failed to create session in Redis")
            
            # Convert SessionCreate to kwargs for user_session_manager.create_session
            model_params = {
                "temperature": session_data.temperature,
                "reasoning_effort": session_data.reasoning_effort,
                "budget_tokens": session_data.budget_tokens,
                "max_tokens": session_data.max_tokens,
            }
            
            additional_params: Dict[str, Any] = {
                "agent_key": session_data.persona_id,
                "custom_prompt": session_data.custom_prompt,
            }
            
            # Handle tools parameter
            if session_data.tools is not None and len(session_data.tools) > 0:
                additional_params["additional_tools"] = session_data.tools
            
            # Filter out None values
            model_params = {k: v for k, v in model_params.items() if v is not None}
            additional_params = {k: v for k, v in additional_params.items() if v is not None}
            
            # Create the session in the agent manager with existing session ID
            session_params = {"existing_ui_session_id": str(session.id)}
            
            # Create session in agent manager
            ui_session_id = await self.agent_manager.create_user_session(
                llm_model=session_data.model_id,
                **model_params,
                **additional_params,
                **session_params
            )
            
            # Get the created session data
            session_data = await self.agent_manager.get_user_session(ui_session_id)
            if not session_data:
                raise HTTPException(status_code=500, detail="Session created but data not found")
            
            # Extract agent_c_session_id
            agent_c_session_id = session_data.get("agent_c_session_id", "")
            

            # Update Redis session with agent_internal_id
            # await self.session_repository.update_session(
            #     str(session.id),
            #     SessionUpdate(agent_internal_id=agent_c_session_id)

            # Transform into our response model
            return SessionDetail(
                id=ui_session_id,
                model_id=session_data.get("model_name", ""),
                persona_id=session_data.get("agent_key", "default"),
                name=session_data.get("name", f"Session {ui_session_id}"),  # Required field with default
                is_active=True,  # Set default active status
                created_at=session_data.get("created_at", datetime.now()),
                last_activity=session_data.get("last_activity"),
                agent_internal_id=agent_c_session_id,
                tools=session_data.get("additional_tools", []),
                tool_ids=session_data.get("additional_tools", []),  # Duplicate tools for tool_ids field
                temperature=session_data.get("temperature"),
                reasoning_effort=session_data.get("reasoning_effort"),
                budget_tokens=session_data.get("budget_tokens"),
                max_tokens=session_data.get("max_tokens"),
                custom_prompt=session_data.get("custom_prompt"),

            )
            
            # Return the session details
            #return await self.session_repository.get_session(str(session.id))
            
        except Exception as e:
            self.logger.error("create_session_failed", error=str(e))
            raise HTTPException(status_code=500, detail=f"Failed to create session: {str(e)}")

    async def get_sessions(self, limit: int = 10, offset: int = 0) -> SessionListResponse:
        """Get list of sessions with pagination
        
        Args:
            limit: Maximum number of sessions to return
            offset: Number of sessions to skip
            
        Returns:
            SessionListResponse: Paginated list of sessions
        """

        #return await self.session_repository.list_sessions(limit, offset)

        # Get all sessions from the agent manager
        sessions = self.agent_manager.ui_sessions
        
        # Convert to list for pagination
        session_list = [
            SessionSummary(
                id=session_id,
                model_id=session_data.get("model_name", ""),
                persona_id=session_data.get("agent_key", "default"),
                name=session_data.get("name", f"Session {session_id}"),  # Required field with default
                is_active=session_data.get("is_active", True),  # Required field with default
                created_at=session_data.get("created_at", datetime.now()),
                last_activity=session_data.get("last_activity"),
            )
            for session_id, session_data in sessions.items()
        ]
        
        # Apply pagination
        total = len(session_list)
        paginated_sessions = session_list[offset : offset + limit]
        
        # Return paginated response
        return SessionListResponse(
            items=paginated_sessions,
            total=total,
            limit=limit,
            offset=offset,
        )


    async def get_session(self, session_id: str) -> Optional[SessionDetail]:
        """Get detailed information about a specific session
        
        Args:
            session_id: ID of the session to retrieve
            
        Returns:
            SessionDetail: Session details if found, None otherwise
        """

        #return await self.session_repository.get_session(session_id)

        # Get session data using the manager's method
        session_data = await self.agent_manager.get_user_session(session_id)
        if not session_data:
            return None
            
        # Extract agent_c_session_id
        agent_c_session_id = session_data.get("agent_c_session_id", "")
        
        # Get tools - use the tools that were provided at creation time
        tools = session_data.get("additional_tools", [])
        
        # Construct and return the SessionDetail response
        return SessionDetail(
            id=session_id,
            model_id=session_data.get("model_name", ""),
            persona_id=session_data.get("agent_key", "default"),
            name=session_data.get("name", f"Session {session_id}"),  # Required field with default
            is_active=session_data.get("is_active", True),  # Required field with default
            created_at=session_data.get("created_at", datetime.now()),
            last_activity=session_data.get("last_activity"),
            agent_internal_id=agent_c_session_id,
            tools=tools,
            tool_ids=tools,  # Duplicate tools for tool_ids field
            temperature=session_data.get("temperature"),
            reasoning_effort=session_data.get("reasoning_effort"),
            budget_tokens=session_data.get("budget_tokens"),
            max_tokens=session_data.get("max_tokens"),
            custom_prompt=session_data.get("custom_prompt"),
        )


    async def update_session(self, session_id: str, update_data: SessionUpdate) -> SessionDetail:
        """Update session properties
        
        Args:
            session_id: ID of the session to update
            update_data: New session properties
            
        Returns:
            SessionDetail: Updated session details
            
        Raises:
            HTTPException: If session not found or update fails
        """
        try:
            # Get current session
            current_session = await self.session_repository.get_session(session_id)
            if not current_session:
                raise HTTPException(status_code=404, detail=f"Session {session_id} not found")
            
            # Update session in Redis
            updated_session = await self.session_repository.update_session(session_id, update_data)
            if not updated_session:
                raise HTTPException(status_code=500, detail="Failed to update session in Redis")
            
            # Get current session data from agent manager
            current_agent_data = await self.agent_manager.get_user_session(session_id)
            if not current_agent_data:
                raise HTTPException(status_code=404, detail=f"Agent session {session_id} not found")
            
            # Prepare update parameters
            updates = update_data.model_dump(exclude_unset=True, exclude_none=True)
            
            # Create new parameters based on current session
            model_params = {
                "temperature": current_session.temperature,
                "reasoning_effort": current_session.reasoning_effort,
                "budget_tokens": current_session.budget_tokens,
                "max_tokens": current_session.max_tokens,
            }
            
            additional_params = {
                "agent_key": current_session.persona_id,
                "custom_prompt": current_session.custom_prompt,
                "additional_tools": current_session.tools,
            }
            
            # Apply updates
            for key, value in updates.items():
                if key == "persona_id":
                    additional_params["agent_key"] = value
                elif key in model_params:
                    model_params[key] = value
                elif key in additional_params:
                    additional_params[key] = value
            
            # Remove None values
            model_params = {k: v for k, v in model_params.items() if v is not None}
            additional_params = {k: v for k, v in additional_params.items() if v is not None}
            
            # Create a new session with existing session ID
            session_params = {"existing_ui_session_id": session_id}
            
            # Update agent manager session
            await self.agent_manager.create_user_session(
                llm_model=current_session.model_id,
                **model_params,
                **additional_params,
                **session_params
            )
            
            # Return updated session details
            return updated_session
            
        except HTTPException:
            raise
        except Exception as e:
            self.logger.error("update_session_failed", session_id=session_id, error=str(e))
            raise HTTPException(status_code=500, detail=f"Failed to update session: {str(e)}")

    async def delete_session(self, session_id: str) -> bool:
        """Delete a specific session
        
        Args:
            session_id: ID of the session to delete
            
        Returns:
            bool: True if successful, False if session not found
        """
        # Delete from Redis first
        if not await self.session_repository.delete_session(session_id):
            return False
            
        # Then clean up in agent manager
        try:
            await self.agent_manager.cleanup_session(session_id)
            return True
        except Exception as e:
            self.logger.error("delete_session_failed", session_id=session_id, error=str(e))
            return False

    async def get_agent_config(self, session_id: str) -> Optional[AgentConfig]:
        """Get agent configuration for a session
        
        Args:
            session_id: ID of the session
            
        Returns:
            AgentConfig: Agent configuration if found, None otherwise
        """
        # Get session from Redis
        session = await self.session_repository.get_session(session_id)
        if not session:
            return None
            
        # Get agent data from manager
        session_data = await self.agent_manager.get_user_session(session_id)
        if not session_data:
            return None
            
        # Get the agent bridge directly from session data
        agent_bridge = session_data.get("agent_bridge")
        if not agent_bridge:
            return None
            
        # Get agent configuration using the agent's method
        agent_config = agent_bridge.get_agent_runtime_config()
        if not agent_config:
            return None
            
        # Extract model parameters from agent_parameters
        agent_params = agent_config.get("agent_parameters", {})
            
        # Convert to AgentConfig model
        return AgentConfig(
            model_id=agent_config.get("model_name", ""),
            persona_id=agent_config.get("agent_key", "default"),
            custom_prompt=agent_config.get("custom_prompt"),
            temperature=agent_params.get("temperature"),
            reasoning_effort=agent_params.get("reasoning_effort"),
            budget_tokens=agent_params.get("budget_tokens"),
            max_tokens=agent_params.get("max_tokens"),
            tools=agent_config.get("initialized_tools", [])
        )
    
    async def update_agent_config(self, session_id: str, update_data: AgentUpdate) -> Optional[AgentUpdateResponse]:
        """Update agent configuration for a session
        
        Args:
            session_id: ID of the session
            update_data: New agent configuration parameters
            
        Returns:
            AgentUpdateResponse: Response with updated configuration and change details,
                              or None if session not found
        """
        # Get session from Redis
        session = await self.session_repository.get_session(session_id)
        if not session:
            return None
            
        # Get agent data from manager
        session_data = await self.agent_manager.get_user_session(session_id)
        if not session_data:
            return None
            
        # Get the agent directly from session data
        agent_bridge = session_data.get("agent_bridge")
        if not agent_bridge:
            return None
            
        # Create a dictionary with only the fields that were provided in the update_data
        updates = update_data.model_dump(exclude_unset=True, exclude_none=True)
        
        # Map persona_id to agent_key if it exists
        if "persona_id" in updates:
            updates["agent_key"] = updates.pop("persona_id")
        
        # Helper function for safe string conversion and truncation
        def safe_truncate(val, length=10):
            if val is None:
                return "None"
            val_str = str(val)
            return val_str[:length] + "..." if len(val_str) > length else val_str
        
        changes_applied = {}
        changes_skipped = {}
        needs_agent_reinitialization = False
        
        # Update each parameter that exists in the update payload
        for key, value in updates.items():
            if key in ["temperature", "reasoning_effort", "budget_tokens", "max_tokens", "custom_prompt", "agent_key"]:
                # Only update if value is not None
                if value is not None:
                    # Record the change
                    old_value = getattr(agent_bridge, key, None)
                    
                    # Update only attributes that changed
                    if old_value != value:
                        setattr(agent_bridge, key, value)
                        changes_applied[key] = {
                            "from": safe_truncate(old_value),
                            "to": safe_truncate(value)
                        }
                        needs_agent_reinitialization = True
                else:
                    changes_skipped[key] = "Value is None"
            else:
                changes_skipped[key] = "Invalid parameter"
        

        # Get updated agent configuration
        updated_config = await self.get_agent_config(session_id)
        if not updated_config:
            return None
        
        # Return update response
        return AgentUpdateResponse(
            agent_config=updated_config,
            changes_applied=changes_applied,
            changes_skipped=changes_skipped
        )
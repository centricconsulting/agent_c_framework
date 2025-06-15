# src/agent_c_api/api/v2/debug/debug.py
import structlog
from uuid import UUID

from fastapi import APIRouter, Depends, HTTPException, status
from fastapi_versioning import version

from ....core.agent_bridge import AgentBridge
from ....core.agent_manager import UItoAgentBridgeManager
from typing import Any
from ...dependencies import get_bridge_manager
from ..models.debug_models import SessionDebugInfo, AgentDebugInfo
from ..models.response_models import APIResponse, APIStatus

# Configure structured logging
logger = structlog.get_logger()

# Create a router with a debug prefix
router = APIRouter(
    tags=["Debug"],
    prefix="/debug",
    responses={
        status.HTTP_404_NOT_FOUND: {
            "description": "Session not found",
            "content": {
                "application/json": {
                    "example": {
                        "detail": "Session not found"
                    }
                }
            }
        },
        status.HTTP_500_INTERNAL_SERVER_ERROR: {
            "description": "Internal server error",
            "content": {
                "application/json": {
                    "example": {
                        "detail": "Error retrieving debug information: Internal error occurred"
                    }
                }
            }
        }
    }
)


@router.get(
    "/sessions/{session_id}", 
    response_model=APIResponse[SessionDebugInfo],
    summary="Get Session Debug Information",
    description="""
    Get comprehensive debug information about a session.
    
    This endpoint provides detailed diagnostic information about a session's state,
    including chat history statistics, memory status, and tool configuration.
    Intended for development, troubleshooting, and administrative purposes.
    """,
    responses={
        status.HTTP_200_OK: {
            "description": "Session debug information retrieved successfully",
            "content": {
                "application/json": {
                    "example": {
                        "status": {
                            "success": True,
                            "message": "Session debug information retrieved successfully"
                        },
                        "data": {
                            "session_id": "ui-sess-def456",
                            "agent_c_session_id": "internal-sess-xyz789",
                            "agent_name": "Tech Support Agent",
                            "created_at": "2025-05-04T13:45:22Z",
                            "backend": "openai",
                            "model_name": "gpt-4",
                            "session_manager": {
                                "exists": True,
                                "user_id": "user-12345",
                                "has_chat_session": True
                            },
                            "chat_session": {
                                "session_id": "chat-sess-abc123",
                                "has_active_memory": True
                            },
                            "messages": {
                                "count": 7,
                                "user_messages": 3,
                                "assistant_messages": 4,
                                "latest_message": "I'll analyze that code snippet now..."
                            },
                            "recent_messages": [
                                {
                                    "role": "user",
                                    "content_preview": "Can you help me debug my Python code...",
                                    "timestamp": "2025-05-04T14:22:15Z"
                                }
                            ],
                            "current_chat_Log": {
                                "exists": True,
                                "count": 12
                            },
                            "tool_chest": {
                                "exists": True,
                                "active_tools": ["web_search", "code_interpreter"]
                            }
                        }
                    }
                }
            }
        }
    }
)
@version(2)
async def get_session_debug_info(
    session_id: UUID, 
    agent_manager: Any = Depends(get_bridge_manager)  # Use Any instead of UItoAgentBridgeManager
):
    """
    Get comprehensive debug information about a session.
    
    This endpoint provides detailed information about the session state,
    including chat history statistics, memory status, and tool configuration.
    Intended for development and troubleshooting purposes.
    
    Args:
        session_id: The unique identifier for the session
        
    Returns:
        Detailed debug information about the session
    
    Raises:
        404: If the session does not exist
        500: If there's an error retrieving debug information
    """
    try:
        debug_info = await agent_manager.debug_session(str(session_id))
        return APIResponse(
            status=APIStatus(
                success=True,
                message="Session debug information retrieved successfully"
            ),
            data=debug_info
        )
    except ValueError as e:
        raise HTTPException(status_code=status.HTTP_404_NOT_FOUND, detail=str(e))
    except Exception as e:
        logger.exception("Error retrieving session debug info", session_id=str(session_id), error=str(e))
        raise HTTPException(status_code=status.HTTP_500_INTERNAL_SERVER_ERROR, detail=f"Error retrieving debug information: {str(e)}")


@router.get(
    "/agent/{session_id}", 
    response_model=APIResponse[AgentDebugInfo],
    summary="Get Agent Debug Information",
    description="""
    Get detailed debug information about an agent's state and configuration.
    
    This endpoint provides diagnostic information about the agent's internal state,
    including configuration parameters, temperature settings, reasoning effort,
    and other runtime properties. This information is valuable for understanding
    how the agent is currently configured and for troubleshooting issues with
    agent behavior.
    
    The response includes both the agent bridge parameters (the API-facing configuration)
    and the internal agent parameters (the underlying implementation configuration).
    """,
    responses={
        status.HTTP_200_OK: {
            "description": "Agent debug information retrieved successfully",
            "content": {
                "application/json": {
                    "example": {
                        "status": {
                            "success": True,
                            "message": "Agent debug information retrieved successfully"
                        },
                        "data": {
                            "status": "success",
                            "agent_bridge_params": {
                                "temperature": 0.7,
                                "reasoning_effort": "thorough",
                                "extended_thinking": True,
                                "budget_tokens": 8000,
                                "max_tokens": 4000
                            },
                            "internal_agent_params": {
                                "type": "ReactJSAgent",
                                "temperature": 0.5,
                                "reasoning_effort": "thorough",
                                "budget_tokens": 8000,
                                "max_tokens": 4000
                            }
                        }
                    }
                }
            }
        }
    }
)
@version(2)
async def get_agent_debug_info(
    session_id: UUID, 
    agent_manager: Any = Depends(get_bridge_manager)  # Use Any instead of UItoAgentBridgeManager
):
    """
    Get detailed debug information about an agent's state and configuration.
    
    This endpoint provides detailed information about the agent's internal state,
    including configuration parameters, temperature settings, and other runtime properties.
    Intended for development and troubleshooting purposes.
    
    Args:
        session_id: The unique identifier for the session containing the agent
        
    Returns:
        Detailed debug information about the agent's state
    
    Raises:
        404: If the session or agent does not exist
        500: If there's an error retrieving debug information
    """
    try:
        session_data = await agent_manager.get_user_session(str(session_id))
        if not session_data:
            raise HTTPException(status_code=status.HTTP_404_NOT_FOUND, detail="Session not found")

        agent_bridge: AgentBridge = session_data["agent_bridge"]

        # Get ReactJSAgent parameters
        agent_bridge_params = {
            "temperature": getattr(agent_bridge, "temperature", None),
            "reasoning_effort": getattr(agent_bridge, "reasoning_effort", None),
            "extended_thinking": getattr(agent_bridge, "extended_thinking", None),
            "budget_tokens": getattr(agent_bridge, "budget_tokens", None),
            "max_tokens": getattr(agent_bridge, "max_tokens", None),
        }

        # Get internal agent parameters
        internal_agent_params = {}
        if agent_bridge.agent_runtime:
            internal_agent_runtime = agent_bridge.agent_runtime
            internal_agent_params = {
                "type": type(internal_agent_runtime).__name__,
                "temperature": getattr(internal_agent_runtime, "temperature", None),
                "reasoning_effort": getattr(internal_agent_runtime, "reasoning_effort", None),
                "budget_tokens": getattr(internal_agent_runtime, "budget_tokens", None),
                "max_tokens": getattr(internal_agent_runtime, "max_tokens", None),
            }

        debug_info = {
            "status": "success",
            "agent_bridge_params": agent_bridge_params,
            "internal_agent_params": internal_agent_params,
        }

        return APIResponse(
            status=APIStatus(
                success=True,
                message="Agent debug information retrieved successfully"
            ),
            data=debug_info
        )
    except HTTPException:
        raise
    except Exception as e:
        logger.exception("Error retrieving agent debug info", session_id=str(session_id), error=str(e))
        raise HTTPException(status_code=status.HTTP_500_INTERNAL_SERVER_ERROR, detail=f"Error retrieving agent debug information: {str(e)}")
from fastapi import APIRouter, HTTPException, Depends
import logging

from agent_c.util.logging_utils import LoggingManager
from agent_c_api.core.agent_manager import UItoAgentBridgeManager
from agent_c_api.api.dependencies import get_bridge_manager
from agent_c_api.api.v1.llm_models.agent_params import AgentInitializationParams

router = APIRouter()

logger = LoggingManager(__name__).get_logger()

@router.post("/initialize")
async def initialize_user_session(params: AgentInitializationParams,
                                  agent_manager:UItoAgentBridgeManager=Depends(get_bridge_manager)
                                  ):
    """
    Creates an agent session with the provided parameters.
    """
    try:
        agent_key: str = params.persona_name
        session_id = params.ui_session_id

        # Create a new session with both model and backend parameters
        logging.info(f"Creating/resuming session with agent-key: {agent_key}. Existing session ID (if passed): {session_id}")
        session_data = await agent_manager.create_user_session(agent_key, session_id)
        logger.debug(f"Current sessions in memory: {list(agent_manager.ui_sessions.keys())}")
        logger.debug(f"Start user Session {session_data.session_id} for user {session_data.chat_session.user_id}")

        return {"ui_session_id": session_data.session_id,
                "agent_c_session_id": session_data.session_id}

    except Exception as e:
        logger.exception(f"Error during session initialization: {str(e)}", exc_info=True)
        raise HTTPException(status_code=500, detail=str(e))


@router.get("/verify_session/{ui_session_id}")
async def verify_session(ui_session_id: str, agent_manager=Depends(get_bridge_manager)):
    """
    Verifies if a session exists and is valid
    """
    session_data = agent_manager.get_user_session(ui_session_id)
    return {"valid": session_data is not None}

@router.get("/sessions")
async def get_sessions(agent_manager=Depends(get_bridge_manager)):
    """
    Retrieves all available sessions.

    Returns:
        dict: A dictionary containing session IDs and their details

    Raises:
        HTTPException: If there's an error retrieving sessions
    """
    try:
        mgr: UItoAgentBridgeManager = agent_manager
        sessions = mgr.chat_session_manager.session_id_list
        return {"session_ids": sessions}

    except Exception as e:
        logger.exception(f"Error retrieving sessions: {str(e)}", exc_info=True)
        raise HTTPException(
            status_code=500,
            detail=f"Failed to retrieve sessions: {str(e)}"
        )

@router.delete("/sessions")
async def delete_all_sessions(agent_manager=Depends(get_bridge_manager)):
    """
    Delete all active sessions and cleanup their resources.

    Returns:
        dict: Status message with number of sessions deleted

    Raises:
        HTTPException: If there's an error during session cleanup
    """
    try:
        # Get count of sessions before deletion
        session_count = len(agent_manager.ui_sessions)

        # Create list of session IDs to avoid modifying dict during iteration
        ui_session_ids = list(agent_manager.ui_sessions.keys())

        # Clean up each session
        for ui_session_id in ui_session_ids:
            await agent_manager.cleanup_session(ui_session_id)

        logger.debug(
            f"Deleted {session_count} sessions. Hanging sessions from deletion: {list(agent_manager.ui_sessions.keys())}")

        return {
            "status": "success",
            "message": f"Successfully deleted {session_count} sessions",
            "deleted_count": session_count
        }

    except Exception as e:
        logger.exception(f"Error deleting sessions: {str(e)}", exc_info=True)
        raise HTTPException(
            status_code=500,
            detail=f"Failed to delete sessions: {str(e)}"
        )

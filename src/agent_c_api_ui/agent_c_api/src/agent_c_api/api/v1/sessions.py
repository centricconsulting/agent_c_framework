from fastapi import APIRouter, HTTPException, Depends, Request

from agent_c.util.logging_utils import LoggingManager
from agent_c_api.core.user_session_manager import UserSessionManager
from agent_c_api.api.v1.llm_models.agent_params import AgentInitializationParams

router = APIRouter()

logger = LoggingManager(__name__).get_logger()

@router.post("/initialize")
async def initialize_user_session(request: Request, params: AgentInitializationParams):
    """
    Creates an agent session with the provided parameters.
    """
    try:
        logger.info(f"Raw request body: {await request.body()}")
        session_manager = request.app.state.user_session_manager
        agent_key: str = params.agent_key
        session_id = params.ui_session_id

        # Create a new session with both model and backend parameters
        logger.info(f"Creating/resuming session with agent-key: {agent_key}. Existing session ID (if passed): {session_id}")
        session_data = await session_manager.create_user_session(agent_key, session_id)
        logger.debug(f"Current sessions in memory: {list(session_manager.ui_sessions.keys())}")
        logger.debug(f"Start user Session {session_data.session_id} for user {session_data.chat_session.user_id}")

        return {"ui_session_id": session_data.session_id,
                "agent_c_session_id": session_data.session_id}

    except Exception as e:
        logger.exception(f"Error during session initialization: {str(e)}", exc_info=True)
        raise HTTPException(status_code=500, detail=str(e))


@router.get("/verify_session/{ui_session_id}")
async def verify_session(request: Request, ui_session_id: str):
    """
    Verifies if a session exists and is valid
    """
    session_manager: UserSessionManager = request.app.state.user_session_manager
    session_data = session_manager.get_user_session(ui_session_id)
    return {"valid": session_data is not None}

@router.get("/sessions")
async def get_sessions(request: Request):
    """
    Retrieves all available sessions.

    Returns:
        dict: A dictionary containing session IDs and their details

    Raises:
        HTTPException: If there's an error retrieving sessions
    """
    try:
        mgr: UserSessionManager = request.app.state.user_session_manager
        sessions = mgr.chat_session_manager.session_id_list
        return {"session_ids": sessions}

    except Exception as e:
        logger.exception(f"Error retrieving sessions: {str(e)}", exc_info=True)
        raise HTTPException(
            status_code=500,
            detail=f"Failed to retrieve sessions: {str(e)}"
        )

@router.delete("/sessions")
async def delete_all_sessions(request: Request):
    """
    Delete all active sessions and cleanup their resources.

    Returns:
        dict: Status message with number of sessions deleted

    Raises:
        HTTPException: If there's an error during session cleanup
    """
    try:
        session_manager: UserSessionManager = request.app.state.user_session_manager
        # Get count of sessions before deletion
        session_count = len(session_manager.ui_sessions)

        # Create list of session IDs to avoid modifying dict during iteration
        ui_session_ids = list(session_manager.ui_sessions.keys())

        # Clean up each session
        for ui_session_id in ui_session_ids:
            await session_manager.cleanup_session(ui_session_id)

        logger.debug(
            f"Deleted {session_count} sessions. Hanging sessions from deletion: {list(session_manager.ui_sessions.keys())}")

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

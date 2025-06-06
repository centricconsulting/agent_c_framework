from fastapi import APIRouter, HTTPException, Depends
import logging
from agent_c_api.core.agent_manager import UItoAgentBridgeManager
from agent_c_api.api.dependencies import get_agent_manager
from agent_c_api.api.v1.llm_models.agent_params import AgentInitializationParams

router = APIRouter()
logger = logging.getLogger(__name__)


@router.post("/initialize")
async def initialize_agent(params: AgentInitializationParams,
        agent_manager=Depends(get_agent_manager)
):
    """
    Creates an agent session with the provided parameters.
    """
    try:
        # Create a new session with both model and backend parameters
        logging.debug(f"Creating new session with model: {params.model_name}, backend: {params.backend}")

        # Get only the relevant model parameters using your filtering method.
        model_params = params.to_agent_kwargs()
        logging.debug(f"--->Model parameters: {model_params} identified")

        # Get the additional parameters (including persona_name and custom prompt).
        additional_params = params.to_additional_params()
        logging.debug(f"--->Additional parameters: {additional_params} identified")

        # Use ui_session_id if provided (for model changes) - this allows us to transfer chat history
        session_params = {}
        existing_ui_session_id = getattr(params, 'ui_session_id', None)
        if existing_ui_session_id:
            session_params['existing_ui_session_id'] = existing_ui_session_id
        logger.debug(f"Existing session ID (if passed): {existing_ui_session_id}")


        new_session_id = await agent_manager.create_session(
            llm_model=params.model_name,
            backend=params.backend,
            **model_params,
            **additional_params,
            **session_params
        )

        logger.debug(f"Current sessions in memory: {list(agent_manager.ui_sessions.keys())}")
        logger.debug(
            f"User Session {new_session_id} with session details: {agent_manager.get_session_data(new_session_id)}")
        return {"ui_session_id": new_session_id,
                "agent_c_session_id": agent_manager.get_session_data(new_session_id).get('agent_c_session_id',
                                                                                         "Unknown")}

    except Exception as e:
        logger.error(f"Error during session initialization: {str(e)}")
        raise HTTPException(status_code=500, detail=str(e))


@router.get("/verify_session/{ui_session_id}")
async def verify_session(ui_session_id: str, agent_manager=Depends(get_agent_manager)):
    """
    Verifies if a session exists and is valid
    """
    session_data = agent_manager.get_session_data(ui_session_id)
    return {"valid": session_data is not None}

@router.get("/sessions")
async def get_sessions(agent_manager=Depends(get_agent_manager)):
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
        logger.error(f"Error retrieving sessions: {str(e)}")
        raise HTTPException(
            status_code=500,
            detail=f"Failed to retrieve sessions: {str(e)}"
        )

@router.delete("/sessions")
async def delete_all_sessions(agent_manager=Depends(get_agent_manager)):
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
        logger.error(f"Error deleting sessions: {str(e)}")
        raise HTTPException(
            status_code=500,
            detail=f"Failed to delete sessions: {str(e)}"
        )

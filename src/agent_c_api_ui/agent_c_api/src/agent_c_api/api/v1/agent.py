from typing import Dict, Any

from fastapi import APIRouter, HTTPException, Depends

from agent_c.models.agent_config import AgentConfiguration
from agent_c_api.api.dependencies import get_agent_manager
from agent_c_api.api.v1.llm_models.agent_params import AgentUpdateParams
from agent_c_api.api.v1.llm_models.tool_model import ToolUpdateRequest
from agent_c_api.core.agent_bridge import AgentBridge
from agent_c_api.core.util.logging_utils import LoggingManager

logging_manager = LoggingManager(__name__)
logger = logging_manager.get_logger()

router = APIRouter()


@router.post("/update_settings")
async def update_agent_settings(
        update_params: AgentUpdateParams,
        agent_manager=Depends(get_agent_manager)
):
    """
    Update agent settings for a given session.
    """
    # Get current session and agent
    ui_session_data = await agent_manager.get_session_data(update_params.ui_session_id)
    if not ui_session_data:
        return {"error": "Invalid session_id"}

    agent_bridge: AgentBridge = ui_session_data["agent_bridge"]

    if not agent_bridge:
        logger.warning(f"No agent bridge found for session {update_params.ui_session_id}")
        return {"error": "No agent bridge found to update"}

    logger.debug(f"Pydantic model received: {update_params}")
    logger.debug(f"Model dump: {update_params.model_dump(exclude_unset=False)}")

    # Helper function for safe string conversion and truncation
    def safe_truncate(val, length=10):
        if val is None:
            return "None"
        val_str = str(val)
        return val_str[:length] + "..." if len(val_str) > length else val_str

    try:
        # Only process parameters that were actually provided in the form
        updates = update_params.model_dump(exclude_unset=True)
        logger.debug(f"Updates received: {updates}")

        # Update each parameter that exists in the update payload
        changes_made = {}
        failed_updates = []

        if "persona_name" in updates:
            agent_key = updates["persona_name"]
            agent_config: AgentConfiguration = agent_manager.agent_config_loader.duplicate(agent_key)
            logger.info(f"Updating agent config for session {update_params.ui_session_id} with key: {agent_key} from{agent_bridge.chat_session.agent_config.key}")
            agent_bridge.chat_session.agent_config = agent_config
            await agent_bridge.update_tools(agent_config.tools)
            ui_session_data["active_tools"] = agent_config.tools
        else:
            agent_config = agent_bridge.chat_session.agent_config



        for key, value in updates.items():
            if key in ["temperature", "reasoning_effort", "extended_thinking", "budget_tokens"]:
                # Only update if value is not None
                if value is not None:
                    if key in agent_config.agent_params.__fields__:
                        # Record the change
                        old_value = getattr(agent_config.agent_params, key, None)

                        # Update only attributes that changed
                        if old_value != value:
                            setattr(agent_config.agent_params, key, value)
                            changes_made[key] = {
                                "from": safe_truncate(old_value),
                                "to": safe_truncate(value)
                            }

                        logger.debug(f"Updated {key}: {safe_truncate(old_value)} -> {safe_truncate(value)}")



        logger.info(f"Settings updated for session {update_params.ui_session_id}: {changes_made}")
        logger.info(f"Failed updates: {failed_updates}")

        return {
            "status": "success",
            "message": f"Settings updated successfully for Agent {update_params.ui_session_id}",
            "changes_made": changes_made,
            "skipped_null_values": [k for k, v in updates.items() if v is None],
            "failed_updates": failed_updates
        }
    except Exception as e:
        logger.exception(f"Error updating settings for session {update_params.ui_session_id}: {str(e)}", exc_info=True)
        return {"error": f"Failed to update settings: {str(e)}"}


@router.get("/get_agent_config/{ui_session_id}")
async def get_agent_config(ui_session_id: str, agent_manager=Depends(get_agent_manager)):
    try:
        # logger.info(f"get_agent_config called for session: {ui_session_id}")
        ui_session_data = await agent_manager.get_session_data(ui_session_id)
        if not ui_session_data:
            raise HTTPException(status_code=404, detail="Session not found")

        agent_bridge = ui_session_data["agent_bridge"]
        config = agent_bridge.get_agent_runtime_config()

        runtime_params: Dict[str, Any] = config['agent_parameters']
        runtime_params["name"] =  runtime_params.pop("model_name")


        # Add additional configuration info
        config.update({
            "ui_session_id": ui_session_id,
            "agent_c_session_id": ui_session_id,
            "model_info": runtime_params,
            "initialized_tools": config["initialized_tools"]
        })

        # logger.info(f"Session {ui_session_id} requested agent config: {config[:100]}")
        return {
            "config": config,
            "status": "success"
        }
    except Exception as e:
        logger.exception(f"Session {ui_session_id} - Error getting agent config: {e}", exc_info=True)
        raise HTTPException(status_code=500, detail=str(e))


@router.post("/update_tools")
async def update_agent_tools(
        data: ToolUpdateRequest,
        agent_manager=Depends(get_agent_manager)
):
    try:
        ui_session_id = data.ui_session_id
        ui_session_data = await agent_manager.get_session_data(ui_session_id)
        if not ui_session_data:
            raise HTTPException(status_code=404, detail="Invalid session ID")

        tool_list = data.tools
        # Validate that tools is a list of strings
        if not isinstance(data.tools, list):
            raise HTTPException(status_code=400, detail="Tools must be an array")

        agent_bridge = ui_session_data["agent_bridge"]
        await agent_bridge.update_tools(tool_list)
        ui_session_data["active_tools"] = tool_list

        return {
            "status": "success",
            "message": "Tools updated successfully",
            "active_tools": tool_list,
            "ui_session_id": ui_session_id,
            "agent_c_session_id": ui_session_data.get('agent_c_session_id', "Unknown")
        }
    except Exception as e:
        logger.error(f"Error updating tools: {str(e)}")
        raise HTTPException(status_code=500, detail=str(e))


@router.get("/get_agent_tools/{ui_session_id}")
async def get_agent_tools(ui_session_id: str, agent_manager=Depends(get_agent_manager)):
    try:
        session_data = await agent_manager.get_session_data(ui_session_id)
        if not session_data:
            raise HTTPException(status_code=404, detail="Session not found")

        agent_bridge: AgentBridge = session_data["agent_bridge"]
        config = agent_bridge.get_agent_runtime_config()
        logger.info(f"Session {ui_session_id} requested tools config: {config['initialized_tools']}")

        return {
            "initialized_tools": config["initialized_tools"],
            "status": "success"
        }
    except Exception as e:
        logger.error(f"Error getting agent tools: {e}")
        raise HTTPException(status_code=500, detail=str(e))


# http://localhost:8000/api/v1/debug_agent_state/2971f215-a631-4177-852e-c3595b6d256a
@router.get("/debug_agent_state/{ui_session_id}")
async def debug_agent_state(ui_session_id: str, agent_manager=Depends(get_agent_manager)):
    """
    Debug endpoint to check the state of an agent and its internal components.
    """
    try:
        session_data = await agent_manager.get_session_data(ui_session_id)
        if not session_data:
            raise HTTPException(status_code=404, detail="Session not found")

        agent_bridge = session_data["agent_bridge"]

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
            internal_agent = agent_bridge.agent_runtime
            internal_agent_params = {
                "type": type(internal_agent).__name__,
                "temperature": getattr(internal_agent, "temperature", None),
                "reasoning_effort": getattr(internal_agent, "reasoning_effort", None),
                "budget_tokens": getattr(internal_agent, "budget_tokens", None),
                "max_tokens": getattr(internal_agent, "max_tokens", None),
            }

        return {
            "status": "success",
            "agent_bridge_params": agent_bridge_params,
            "internal_agent_params": internal_agent_params,
        }
    except Exception as e:
        logger.error(f"Error debugging agent state: {e}")
        raise HTTPException(status_code=500, detail=str(e))

# http://localhost:8000/api/v1/chat_session_debug/2971f215-a631-4177-852e-c3595b6d256a
@router.get("/chat_session_debug/{ui_session_id}")
async def debug_chat_session(ui_session_id: str, agent_manager=Depends(get_agent_manager)):
    try:
        debug_info = await agent_manager.debug_session(ui_session_id)
        return debug_info
    except ValueError as e:
        raise HTTPException(status_code=404, detail=str(e))
    except Exception as e:
        raise HTTPException(status_code=500, detail=f"Error debugging session: {str(e)}")
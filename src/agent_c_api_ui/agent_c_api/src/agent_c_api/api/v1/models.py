from fastapi import APIRouter, HTTPException
from agent_c_api.core.util.logging_utils import LoggingManager
from agent_c.config.model_config_loader import ModelConfigurationLoader

router = APIRouter()
logger = LoggingManager(__name__).get_logger()


@router.get("/models")
async def list_models():
    """Get list of available models from model_configs.json"""
    try:
        loader: ModelConfigurationLoader = ModelConfigurationLoader.instance()
        loader_list = loader.model_list
        model_list = [model.model_dump(exclude_none=True) for model in loader_list]
        return {"models": model_list}
    except Exception as e:
        logger.exception(f"Error reading model config: {e}", exc_info=True)
        raise HTTPException(status_code=500, detail=str(e))
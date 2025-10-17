import os
from fastapi import APIRouter, HTTPException, Depends
import json
import logging
from agent_c_api.core.agent_manager import UItoAgentBridgeManager
from agent_c_api.config.config_loader import MODELS_CONFIG


router = APIRouter()
logger = logging.getLogger(__name__)


@router.get("/models")
async def list_models():
    """Get list of available models from model_configs.json"""
    try:
        # Ensure file exists
        if not MODELS_CONFIG:
            logger.warning("Model config is empty or not loaded.")
            return {"models": []}

        # Transform the data for frontend consumption
        models = []
        # May consider returning unaltered structure in future
        # if not MODELS_CONFIG:
        #     logger.warning("Model config is empty or not loaded.")
        #     return {"models": []}

        # For now, will flatten it abit for frontend consumption and UI control visiblity (on/off)
        for vendor in MODELS_CONFIG.get("vendors", []):
            vendor_name = vendor.get("vendor")
            for model in vendor.get("models", []):
                models.append({
                    "id": model["id"],
                    "label": model["ui_name"],
                    "description": model["description"],
                    "model_type": model["model_type"],
                    "backend": vendor_name,
                    "parameters": model["parameters"],
                    "capabilities": model["capabilities"],
                    "allowed_inputs": model["allowed_inputs"]
                })

        logger.info(f"Found {len(models)} models in configuration")
        return {"models": models}

    except Exception as e:
        logger.error(f"Error reading model config: {e}")
        raise HTTPException(status_code=500, detail=str(e))
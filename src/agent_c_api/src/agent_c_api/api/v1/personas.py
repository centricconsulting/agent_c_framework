import glob
import os
from typing import List, Dict
from fastapi import APIRouter, HTTPException, Depends

import logging

from agent_c.config.agent_config_loader import AgentConfigLoader
from agent_c_api.config.env_config import settings


router = APIRouter()
logger = logging.getLogger(__name__)

@router.get("/personas")
async def list_personas() -> List[Dict[str, str]]:
    """Get list of available personas from personas directory"""
    loader: AgentConfigLoader = AgentConfigLoader()
    personas = []

    # load the agent configuration catalog into this old persona format for now
    for key, config in loader.catalog.items():
        if "domo" in config.category:
            # this is a domo agent, so we can use the config as a persona
            personas.append({
                "name": config.key,
                "content": config.agent_description,
                "file": f"{config.key}.md"
            })

    logger.info(f"Found {len(personas)} personas")
    return personas
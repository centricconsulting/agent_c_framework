from fastapi import APIRouter, HTTPException
import logging

from agent_c.toolsets import Toolset
from agent_c_api.core.agent_manager import UItoAgentBridgeManager

# Always import the core tools - These must be available!
from agent_c_tools import *  # noqa
from agent_c_tools.tools.in_process import * # noqa


# Conditionally import other tool modules
logger = logging.getLogger(__name__)


router = APIRouter()

@router.get("/tools")
async def tools_list():
    try:
        essential_tools = []
        tool_groups = {
            'Core Tools': [],
            # 'Demo Tools': [],
            # 'Voice Tools': [],
            # 'RAG Tools': []
        }
        tool_name_mapping = {}

        categories = {
            'agent_c_tools': 'Core Tools',
            # 'agent_c_demo': 'Demo Tools',
            # 'agent_c_voice': 'Voice Tools',
            # 'agent_c_rag': 'RAG Tools'
        }

        # Get all tools from the toolsets
        for tool_class in Toolset.tool_registry:
            tool_info = {
                'name': tool_class.__name__,
                'module': tool_class.__module__,
                'doc': tool_class.__doc__,
                'essential': False
            }

            # Categorize non-essential tools
            category = None
            for module_prefix, category_name in categories.items():
                if tool_class.__module__.startswith(module_prefix):
                    category = category_name
                    break

            if category:
                tool_groups[category].append(tool_info)

        for category in tool_groups:
            tool_groups[category].sort(key=lambda x: x['name'].lower())

        return {
            "essential_toolsets": [],
            "groups": tool_groups,
            "categories": list(categories.values()),
            "tool_name_mapping": tool_name_mapping
        }
    except Exception as e:
        logger.error(f"Error retrieving tools: {e}")
        raise HTTPException(status_code=500, detail=str(e))
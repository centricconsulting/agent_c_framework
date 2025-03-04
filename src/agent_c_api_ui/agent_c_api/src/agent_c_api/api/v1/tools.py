from fastapi import APIRouter, HTTPException, Form, UploadFile, File, Depends, Request

from agent_c_api.core.agent_manager import UItoAgentBridgeManager
from agent_c import Toolset


# Ensure all our toolsets get registered
from agent_c_tools.tools import *  # noqa


router = APIRouter()
logger = logging.getLogger(__name__)

@router.get("/tools")
async def tools_list():
    try:
        essential_tools = []
        tool_groups = {
            'Core Tools': [],
            'Demo Tools': [],
            'Voice Tools': [],
            'RAG Tools': []
        }
        tool_name_mapping = {}

        categories = {
            'agent_c_tools': 'Core Tools',
            'agent_c_demo': 'Demo Tools',
            'agent_c_voice': 'Voice Tools',
            'agent_c_rag': 'RAG Tools'
        }

        # Get all tools from the toolsets
        for tool_class in Toolset.tool_registry:
            tool_info = {
                'name': tool_class.__name__,
                'module': tool_class.__module__,
                'doc': tool_class.__doc__,
                'essential': tool_class.__name__ in UItoAgentBridgeManager.ESSENTIAL_TOOLS
            }

            if tool_info['essential']:
                essential_tools.append(tool_info)
                continue

            # Categorize non-essential tools
            category = None
            for module_prefix, category_name in categories.items():
                if tool_class.__module__.startswith(module_prefix):
                    category = category_name
                    break

            if category:
                tool_groups[category].append(tool_info)

        # Sort tools
        essential_tools.sort(key=lambda x: x['name'].lower())
        for category in tool_groups:
            tool_groups[category].sort(key=lambda x: x['name'].lower())

        return {
            "essential_tools": essential_tools,
            "groups": tool_groups,
            "categories": list(categories.values()),
            "tool_name_mapping": tool_name_mapping
        }
    except Exception as e:
        logger.error(f"Error retrieving tools: {e}")
        raise HTTPException(status_code=500, detail=str(e))



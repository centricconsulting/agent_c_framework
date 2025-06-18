from typing import Optional

from fastapi.params import Depends
from fastapi_cache.decorator import cache

from agent_c.config import ModelConfigurationLoader
from agent_c.toolsets.tool_set import Toolset
from agent_c.models.agent_config import AgentConfiguration
from agent_c.config.agent_config_loader import AgentConfigLoader
from agent_c_api.api.dependencies import get_agent_config_loader, get_model_config_loader

from agent_c_api.api.v2.models.config_models import (
    ModelInfo, ToolInfo, ModelParameter,
    ModelsResponse, AgentConfigsResponse, ToolsResponse, SystemConfigResponse
)
from agent_c_api.core.util.logging_utils import LoggingManager

logger = LoggingManager(__name__).get_logger()

class ConfigService:
    """Service for retrieving configuration data from existing sources"""
    def __init__(self):
        self.agent_config_loader: AgentConfigLoader = get_agent_config_loader()

    @cache(expire=300)  # Cache for 5 minutes
    async def get_models(self, loader: ModelConfigurationLoader = Depends(get_model_config_loader)) -> ModelsResponse:
        """
        Get available models using the existing configuration mechanism
        """
        try:
            model_list = loader.model_list
        except Exception as e:
            logger.exception(f"Error reading model config: {e}", exc_info=True)
            raise e
        
        return ModelsResponse(models=model_list)
    
    @cache(expire=300)  # Cache for 5 minutes
    async def get_model(self, model_id: str) -> Optional[ModelInfo]:
        """
        Get a specific model by ID
        """
        models_response = await self.get_models()
        for model in models_response.models:
            if model.id == model_id:
                return model
        return None
    
    @cache(expire=300)  # Cache for 5 minutes
    async def get_agent_configs(self) -> AgentConfigsResponse:
        """
        Get available personas using the existing file-based mechanism
        """
        return AgentConfigsResponse(agents=list(self.agent_config_loader.catalog.values()))

    
    @cache(expire=300)  # Cache for 5 minutes
    async def get_agent_config(self, agent_key: str) -> Optional[AgentConfiguration]:
        """
        Get a specific agent by ID
        """
        if agent_key not in self.agent_config_loader.catalog:
            return None

        return self.agent_config_loader.catalog[agent_key]
    
    @cache(expire=300)  # Cache for 5 minutes
    async def get_tools(self) -> ToolsResponse:
        """
        Get available tools using the existing tool discovery mechanism
        """
        # Using the same approach as in v1/tools.py
        tool_list = []
        categories = set()

        # Categories mapping similar to v1 implementation
        category_mapping = {
            'agent_c_tools': 'Core Tools'
        }
        
        # Get all tools from the Toolset registry
        for tool_class in Toolset.tool_registry:
            # Determine category based on module name
            category = "General"
            for module_prefix, category_name in category_mapping.items():
                if tool_class.__module__.startswith(module_prefix):
                    category = category_name
                    break
                    
            categories.add(category)

            # Get parameters from tool class (simplified approach)
            parameters = []
            # For now, we don't have easy access to parameter info
            # This would require inspecting the tool class more thoroughly
            
            # Create tool info
            tool_info = ToolInfo(
                id=tool_class.__name__,
                name=tool_class.__name__,
                description=tool_class.__doc__ or "",
                category=category,
                parameters=parameters,
                is_essential=False
            )
            tool_list.append(tool_info)
        
        return ToolsResponse(
            tools=tool_list,
            categories=sorted(list(categories)),
            essential_tools=[]
        )
    
    @cache(expire=300)  # Cache for 5 minutes
    async def get_tool(self, tool_id: str) -> Optional[ToolInfo]:
        """
        Get a specific tool by ID
        """
        tools_response = await self.get_tools()
        for tool in tools_response.tools:
            if tool.id == tool_id:
                return tool
        return None
    
    @cache(expire=300)  # Cache for 5 minutes
    async def get_system_config(self) -> SystemConfigResponse:
        """
        Get combined system configuration
        """
        models_response = await self.get_models()
        agent_configs_response = await self.get_agent_configs()
        tools_response = await self.get_tools()
        
        return SystemConfigResponse(
            models=models_response.models,
            agent_configs=agent_configs_response.agents,
            tools=tools_response.tools,
            tool_categories=tools_response.categories,
            essential_tools=[]
        )
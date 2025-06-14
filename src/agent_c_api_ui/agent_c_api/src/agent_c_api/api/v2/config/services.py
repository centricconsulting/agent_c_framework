from typing import Optional
from fastapi_cache.decorator import cache

from agent_c.toolsets.tool_set import Toolset
from agent_c.models.agent_config import AgentConfiguration
from agent_c_api.config.config_loader import MODELS_CONFIG
from agent_c.config.agent_config_loader import AgentConfigLoader

from agent_c_api.api.v2.models.config_models import (
    ModelInfo, ToolInfo, ModelParameter,
    ModelsResponse, AgentConfigsResponse, ToolsResponse, SystemConfigResponse
)

class ConfigService:
    """Service for retrieving configuration data from existing sources"""
    def __init__(self):
        self.agent_config_loader: AgentConfigLoader = AgentConfigLoader()

    @cache(expire=300)  # Cache for 5 minutes
    async def get_models(self) -> ModelsResponse:
        """
        Get available models using the existing configuration mechanism
        """
        # Using the same approach as in v1/models.py
        model_list = []
        
        # Process models in the same format as v1 API
        if not MODELS_CONFIG:
            return ModelsResponse(models=[])
            
        for vendor in MODELS_CONFIG.get("vendors", []):
            vendor_name = vendor.get("vendor")
            for model in vendor.get("models", []):
                # Transform to our v2 model format
                parameters = []
                for param_name, param_data in model.get("parameters", {}).items():
                    parameters.append(ModelParameter(
                        name=param_name,
                        type=param_data.get("type", "string"),
                        description=param_data.get("description", ""),
                        default=param_data.get("default")
                    ))
                
                # Transform capabilities from dict to list if needed
                capabilities = []
                if isinstance(model.get("capabilities", {}), dict):
                    capabilities = [key for key, value in model.get("capabilities", {}).items() if value]
                elif isinstance(model.get("capabilities", []), list):
                    capabilities = model.get("capabilities", [])
                
                # Transform allowed_inputs from dict to list if needed
                allowed_inputs = []
                if isinstance(model.get("allowed_inputs", {}), dict):
                    allowed_inputs = [key for key, value in model.get("allowed_inputs", {}).items() if value]
                elif isinstance(model.get("allowed_inputs", []), list):
                    allowed_inputs = model.get("allowed_inputs", [])
                
                model_info = ModelInfo(
                    id=model["id"],
                    name=model.get("ui_name", model["id"]),
                    provider=vendor_name,
                    description=model.get("description", ""),
                    capabilities=capabilities,
                    parameters=parameters,
                    allowed_inputs=allowed_inputs
                )
                model_list.append(model_info)
        
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
        essential_tools = []
        
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
            essential_tools=essential_tools
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
            essential_tools=tools_response.essential_tools
        )
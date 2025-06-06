import os
import glob
from typing import Optional
from fastapi_cache.decorator import cache

from agent_c.toolsets.tool_set import Toolset
from agent_c_api.config.config_loader import MODELS_CONFIG
from agent_c_api.config.env_config import settings
from agent_c_api.core.agent_manager import UItoAgentBridgeManager

from agent_c_api.api.v2.models.config_models import (
    ModelInfo, PersonaInfo, ToolInfo, ModelParameter, ToolParameter,
    ModelsResponse, PersonasResponse, ToolsResponse, SystemConfigResponse
)

class ConfigService:
    """Service for retrieving configuration data from existing sources"""
    
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
    async def get_personas(self) -> PersonasResponse:
        """
        Get available personas using the existing file-based mechanism
        """
        # Using the same approach as in v1/personas.py
        persona_list = []
        persona_dir = settings.PERSONA_DIR
        
        # Ensure directory exists
        if not os.path.isdir(persona_dir):
            return PersonasResponse(personas=[])
            
        # Get all .md files in personas directory
        for file_path in glob.glob(os.path.join(persona_dir, "**/*.md"), recursive=True):
            rel_path = os.path.relpath(file_path, persona_dir)
            name_with_path: str  = rel_path[:-3]
            # Replace directory separators with desired character (e.g., '_' or '/')
            name = name_with_path.replace(os.sep, ' - ')
            
            # Read persona content
            try:
                with open(file_path, 'r', encoding='utf-8') as f:
                    content = f.read()
            except Exception as e:
                continue
                
            persona_info = PersonaInfo(
                id=name,  # Use the name as the ID
                name=name,
                description="",  # No description available in v1 implementation
                file_path=file_path,
                content=content
            )
            persona_list.append(persona_info)
        
        return PersonasResponse(personas=persona_list)
    
    @cache(expire=300)  # Cache for 5 minutes
    async def get_persona(self, persona_id: str) -> Optional[PersonaInfo]:
        """
        Get a specific persona by ID
        """
        personas_response = await self.get_personas()
        for persona in personas_response.personas:
            if persona.id == persona_id:
                return persona
        return None
    
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
            'agent_c_tools': 'Core Tools',
            'agent_c_demo': 'Demo Tools',
            'agent_c_voice': 'Voice Tools',
            'agent_c_rag': 'RAG Tools'
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
            
            # Check if tool is essential
            is_essential = tool_class.__name__ in UItoAgentBridgeManager.ESSENTIAL_TOOLS
            if is_essential:
                essential_tools.append(tool_class.__name__)
            
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
                is_essential=is_essential
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
        personas_response = await self.get_personas()
        tools_response = await self.get_tools()
        
        return SystemConfigResponse(
            models=models_response.models,
            personas=personas_response.personas,
            tools=tools_response.tools,
            tool_categories=tools_response.categories,
            essential_tools=tools_response.essential_tools
        )
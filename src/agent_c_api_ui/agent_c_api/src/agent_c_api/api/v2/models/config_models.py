from typing import Dict, List, Optional, Any, Union, ClassVar
from pydantic import BaseModel, Field, ConfigDict

from agent_c.models.agent_config import AgentConfigurationV2, AgentConfiguration


class ModelParameter(BaseModel):
    """Parameter for a model configuration"""
    name: str
    type: str
    description: Optional[str] = None
    default: Optional[Any] = None
    
    model_config = ConfigDict(
        json_schema_extra= {
            "example": {
                "name": "temperature",
                "type": "float",
                "description": "Controls randomness in the output. Higher values produce more creative results.",
                "default": 0.7
            }
        })


class ModelInfo(BaseModel):
    """Information about an available LLM model"""
    id: str = Field(description="Unique identifier for the model")
    name: str = Field(description="Display name of the model")
    provider: str = Field(default="unknown", description="Provider of the model (e.g., OpenAI, Anthropic)")
    description: Optional[str] = Field(None, description="Detailed description of the model capabilities")
    capabilities: List[str] = Field(default_factory=list, description="List of capabilities (e.g., text, images, audio)")
    parameters: List[ModelParameter] = Field(default_factory=list, description="Configuration parameters supported by the model")
    allowed_inputs: List[str] = Field(default_factory=list, description="Types of inputs the model can process")

    model_config = ConfigDict(
        json_schema_extra={
            "example": {
                "id": "gpt-4",
                "name": "GPT-4",
                "provider": "openai",
                "description": "OpenAI's most advanced model, with broader general knowledge and advanced reasoning capabilities.",
                "capabilities": ["text", "images", "reasoning"],
                "parameters": [
                    {
                        "name": "temperature",
                        "type": "float",
                        "description": "Controls randomness",
                        "default": 0.7
                    },
                    {
                        "name": "max_tokens",
                        "type": "integer",
                        "description": "Maximum length of generated content",
                        "default": 2048
                    }
                ],
                "allowed_inputs": ["text", "image"]
            }
        })


class PersonaInfo(BaseModel):
    """Information about an available persona"""
    id: str = Field(description="Unique identifier for the persona")
    name: str = Field(description="Display name of the persona")
    description: Optional[str] = Field(None, description="Brief description of the persona's capabilities and purpose")
    file_path: Optional[str] = Field(None, description="Path to the persona definition file (internal use)")
    content: Optional[str] = Field(None, description="The actual content of the persona definition (may be truncated for large personas)")

    model_config = ConfigDict(
        json_schema_extra={
            "example": {
                "id": "coder",
                "name": "Coding Assistant",
                "description": "Specialized in writing and reviewing code across multiple languages.",
                "file_path": "/personas/coder.md",
                "content": "You are a coding assistant specialized in helping developers write, review, and debug code..."
            }
        })


class ToolParameter(BaseModel):
    """Parameter for a tool"""
    name: str = Field(description="Name of the parameter")
    type: str = Field(description="Data type of the parameter (string, integer, boolean, etc.)")
    description: Optional[str] = Field(None, description="Description of the parameter's purpose and usage")
    required: bool = Field(False, description="Whether this parameter is required for the tool to function")

    model_config = ConfigDict(
        json_schema_extra={
            "example": {
                "name": "query",
                "type": "string",
                "description": "Search query to perform",
                "required": True
            }
        })


class ToolInfo(BaseModel):
    """Information about an available tool"""
    id: str = Field(description="Unique identifier for the tool")
    name: str = Field(description="Display name of the tool")
    description: Optional[str] = Field(None, description="Detailed description of the tool's functionality")
    category: str = Field("general", description="Category the tool belongs to (web, utility, data, etc.)")
    parameters: List[ToolParameter] = Field(default_factory=list, description="Parameters accepted by the tool")
    is_essential: bool = Field(False, description="Whether this tool is considered essential for basic agent functionality")

    model_config = ConfigDict(
        json_schema_extra={
            "example": {
                "id": "web_search",
                "name": "Web Search",
                "description": "Search the web for up-to-date information on a given topic.",
                "category": "web",
                "parameters": [
                    {
                        "name": "query",
                        "type": "string",
                        "description": "Search query",
                        "required": True
                    },
                    {
                        "name": "max_results",
                        "type": "integer",
                        "description": "Maximum number of results to return",
                        "required": False
                    }
                ],
                "is_essential": True
            }
        })

class ModelsResponse(BaseModel):
    """Response containing available models"""
    models: List[ModelInfo] = Field(description="List of available LLM models")

    model_config = ConfigDict(
        json_schema_extra={
            "example": {
                "models": [
                    {
                        "id": "gpt-4",
                        "name": "GPT-4",
                        "provider": "openai",
                        "description": "OpenAI's most advanced model",
                        "capabilities": ["text", "images"],
                        "parameters": [
                            {
                                "name": "temperature",
                                "type": "float",
                                "description": "Controls randomness",
                                "default": 0.7
                            }
                        ],
                        "allowed_inputs": ["text"]
                    },
                    {
                        "id": "claude-3",
                        "name": "Claude 3",
                        "provider": "anthropic",
                        "description": "Anthropic's most capable multimodal model",
                        "capabilities": ["text", "images"],
                        "parameters": [
                            {
                                "name": "temperature",
                                "type": "float",
                                "description": "Controls randomness",
                                "default": 0.5
                            }
                        ],
                        "allowed_inputs": ["text", "image"]
                    }
                ]
            }
        })


class AgentConfigsResponse(BaseModel):
    """Response containing available personas"""
    agents: List[AgentConfiguration] = Field(description="List of available agent configurations")



class ToolsResponse(BaseModel):
    """Response containing available tools"""
    tools: List[ToolInfo] = Field(description="List of available tools")
    categories: List[str] = Field(description="Available tool categories for organization")
    essential_tools: List[str] = Field(description="List of tool IDs considered essential for basic functionality")

    model_config = ConfigDict(
        json_schema_extra={
            "example": {
                "tools": [
                    {
                        "id": "web_search",
                        "name": "Web Search",
                        "description": "Search the web for information",
                        "category": "web",
                        "parameters": [
                            {
                                "name": "query",
                                "type": "string",
                                "description": "Search query",
                                "required": True
                            }
                        ],
                        "is_essential": True
                    },
                    {
                        "id": "file_search",
                        "name": "File Search",
                        "description": "Search through files in a workspace",
                        "category": "file",
                        "parameters": [
                            {
                                "name": "query",
                                "type": "string",
                                "description": "Search query",
                                "required": True
                            },
                            {
                                "name": "workspace",
                                "type": "string",
                                "description": "Workspace to search in",
                                "required": True
                            }
                        ],
                        "is_essential": False
                    }
                ],
                "categories": ["web", "file", "utility"],
                "essential_tools": ["web_search"]
            }
        })


class SystemConfigResponse(BaseModel):
    """Combined system configuration response"""
    models: List[ModelInfo] = Field(description="List of available LLM models")
    agent_configs: List[AgentConfiguration] = Field(description="List of available agent configurations")
    tools: List[ToolInfo] = Field(description="List of available tools")
    tool_categories: List[str] = Field(description="Available tool categories")
    essential_tools: List[str] = Field(description="Tools considered essential for basic functionality")

    model_config = ConfigDict(
        json_schema_extra={
            "example": {

                "models": [
                    {
                        "id": "gpt-4",
                        "name": "GPT-4",
                        "provider": "openai",
                        "description": "OpenAI's most advanced model"
                    },
                    {
                        "id": "claude-3",
                        "name": "Claude 3",
                        "provider": "anthropic",
                        "description": "Anthropic's most capable model"
                    }
                ],
                "personas": [
                    {
                        "id": "default",
                        "name": "Default Persona",
                        "description": "General purpose assistant"
                    },
                    {
                        "id": "coder",
                        "name": "Coding Assistant",
                        "description": "Specialized in code tasks"
                    }
                ],
                "tools": [
                    {
                        "id": "web_search",
                        "name": "Web Search",
                        "category": "web",
                        "description": "Search the web"
                    },
                    {
                        "id": "file_search",
                        "name": "File Search",
                        "category": "file",
                        "description": "Search files"
                    }
                ],
                "tool_categories": ["web", "file", "utility"],
                "essential_tools": ["web_search"]
            }
        })
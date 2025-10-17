# tests/unit/api/v2/config/test_models.py

import pytest
from typing import Dict, List, Any, Optional
from pydantic import ValidationError

from agent_c_api.api.v2.models.config_models import (
    ModelParameter, ModelInfo, PersonaInfo, ToolParameter, ToolInfo,
    ModelsResponse, AgentConfigsResponse, ToolsResponse, SystemConfigResponse
)


@pytest.mark.unit
@pytest.mark.config
@pytest.mark.models
class TestConfigModels:
    """Test suite for the configuration models in the v2 API.
    
    These tests verify that the Pydantic models used in the config module
    correctly handle initialization, field validation, and data access.
    
    Models tested:
    - ModelParameter - Parameters for LLM models
    - ModelInfo - Information about available LLM models
    - PersonaInfo - Information about available personas
    - ToolParameter - Parameters for tools
    - ToolInfo - Information about available tools
    - Response wrappers: ModelsResponse, AgentConfigsResponse, ToolsResponse, SystemConfigResponse
    """

    def test_config_models_imports(self):
        """Verify that config models can be imported correctly.
        
        This test simply validates that the imports are successful, which helps
        detect issues with circular imports or missing dependencies.
        """
        # This test just verifies imports work, so we don't need to do anything else
        pass

    def test_model_parameter(self):
        """Test ModelParameter creation and field validation.
        
        Verifies that a ModelParameter can be created with the expected fields
        and that the fields are correctly accessible.
        """
        param = ModelParameter(
            name="temperature",
            type="float",
            description="Controls randomness",
            default=0.7
        )
        
        assert param.name == "temperature"
        assert param.type == "float"
        assert param.description == "Controls randomness"
        assert param.default == 0.7
    
    def test_model_parameter_optional_fields(self):
        """Test ModelParameter with optional fields.
        
        Verifies that a ModelParameter can be created with only required fields
        and that optional fields have the expected default values.
        """
        # Only required fields
        param = ModelParameter(
            name="top_p",
            type="float"
        )
        
        assert param.name == "top_p"
        assert param.type == "float"
        assert param.description is None
        assert param.default is None

    def test_model_info(self):
        """Test ModelInfo creation and field validation.
        
        Verifies that a ModelInfo can be created with expected fields
        and that complex nested fields like parameters are handled correctly.
        """
        model_param = ModelParameter(
            name="temperature",
            type="float",
            description="Controls randomness",
            default=0.7
        )
        
        model = ModelInfo(
            id="gpt-4",
            name="GPT-4",
            provider="OpenAI",
            description="Advanced language model",
            capabilities=["chat", "completion"],
            parameters=[model_param],
            allowed_inputs=["text"]
        )
        
        assert model.id == "gpt-4"
        assert model.name == "GPT-4"
        assert model.provider == "OpenAI"
        assert model.description == "Advanced language model"
        assert "chat" in model.capabilities
        assert "completion" in model.capabilities
        assert len(model.parameters) == 1
        assert model.parameters[0].name == "temperature"
        assert model.allowed_inputs == ["text"]
    
    def test_model_info_optional_fields(self):
        """Test ModelInfo with optional fields.
        
        Verifies that ModelInfo handles optional fields correctly
        and assigns appropriate default values.
        """
        # Only required fields
        model = ModelInfo(
            id="gpt-4",
            name="GPT-4"
        )
        
        assert model.id == "gpt-4"
        assert model.name == "GPT-4"
        assert model.provider == "unknown"  # Default value
        assert model.description is None
        assert model.capabilities == []
        assert model.parameters == []
        assert model.allowed_inputs == []

    def test_persona_info(self):
        """Test PersonaInfo creation and field validation.
        
        Verifies that a PersonaInfo can be created with expected fields.
        """
        persona = PersonaInfo(
            id="assistant",
            name="Assistant",
            description="A helpful assistant",
            file_path="/path/to/file.md",
            content="You are a helpful assistant"
        )
        
        assert persona.id == "assistant"
        assert persona.name == "Assistant"
        assert persona.description == "A helpful assistant"
        assert persona.file_path == "/path/to/file.md"
        assert persona.content == "You are a helpful assistant"
    
    def test_persona_info_optional_fields(self):
        """Test PersonaInfo with optional fields.
        
        Verifies that PersonaInfo handles optional fields correctly.
        """
        # Only required fields
        persona = PersonaInfo(
            id="minimal",
            name="Minimal Persona"
        )
        
        assert persona.id == "minimal"
        assert persona.name == "Minimal Persona"
        assert persona.description is None
        assert persona.file_path is None
        assert persona.content is None

    def test_tool_parameter(self):
        """Test ToolParameter creation and field validation.
        
        Verifies that a ToolParameter can be created with expected fields
        and that fields like 'required' are correctly applied.
        """
        param = ToolParameter(
            name="query",
            type="string",
            description="Search query",
            required=True
        )
        
        assert param.name == "query"
        assert param.type == "string"
        assert param.description == "Search query"
        assert param.required == True
    
    def test_tool_parameter_optional_fields(self):
        """Test ToolParameter with optional fields.
        
        Verifies that ToolParameter handles optional fields correctly.
        """
        # Only required fields with defaults for optional fields
        param = ToolParameter(
            name="filter",
            type="string"
        )
        
        assert param.name == "filter"
        assert param.type == "string"
        assert param.description is None
        assert param.required == False  # Default value

    def test_tool_info(self):
        """Test ToolInfo creation and field validation.
        
        Verifies that a ToolInfo can be created with expected fields
        including complex nested fields like parameters.
        """
        param = ToolParameter(
            name="query",
            type="string",
            description="Search query",
            required=True
        )
        
        tool = ToolInfo(
            id="search",
            name="Search Tool",
            description="Search the web",
            category="web",
            parameters=[param],
            is_essential=True
        )
        
        assert tool.id == "search"
        assert tool.name == "Search Tool"
        assert tool.description == "Search the web"
        assert tool.category == "web"
        assert len(tool.parameters) == 1
        assert tool.parameters[0].name == "query"
        assert tool.is_essential == True
    
    def test_tool_info_optional_fields(self):
        """Test ToolInfo with optional fields.
        
        Verifies that ToolInfo handles optional fields with appropriate defaults.
        """
        # Only required fields
        tool = ToolInfo(
            id="basic_tool",
            name="Basic Tool"
        )
        
        assert tool.id == "basic_tool"
        assert tool.name == "Basic Tool"
        assert tool.description is None
        assert tool.category == "general"  # Default value
        assert tool.parameters == []
        assert tool.is_essential == False  # Default value

    def test_models_response(self):
        """Test ModelsResponse creation and validation.
        
        Verifies that a ModelsResponse correctly wraps a list of ModelInfo objects.
        """
        model = ModelInfo(
            id="gpt-4",
            name="GPT-4",
            provider="OpenAI"
        )
        
        response = ModelsResponse(models=[model])
        
        assert len(response.models) == 1
        assert response.models[0].id == "gpt-4"
    
    def test_empty_models_response(self):
        """Test ModelsResponse with empty models list.
        
        Verifies that a ModelsResponse can be created with an empty list.
        """
        response = ModelsResponse(models=[])
        assert len(response.models) == 0

    def test_personas_response(self):
        """Test AgentConfigsResponse creation and validation.
        
        Verifies that a AgentConfigsResponse correctly wraps a list of PersonaInfo objects.
        """
        persona = PersonaInfo(
            id="assistant",
            name="Assistant"
        )
        
        response = AgentConfigsResponse(personas=[persona])
        
        assert len(response.personas) == 1
        assert response.personas[0].id == "assistant"
    
    def test_empty_personas_response(self):
        """Test AgentConfigsResponse with empty personas list.
        
        Verifies that a AgentConfigsResponse can be created with an empty list.
        """
        response = AgentConfigsResponse(personas=[])
        assert len(response.personas) == 0

    def test_tools_response(self):
        """Test ToolsResponse creation and validation.
        
        Verifies that a ToolsResponse correctly wraps a list of ToolInfo objects
        along with categories and essential tools.
        """
        tool = ToolInfo(
            id="search",
            name="Search Tool",
            category="web",
            is_essential=True
        )
        
        response = ToolsResponse(
            tools=[tool],
            categories=["web", "utility"],
            essential_tools=["search"]
        )
        
        assert len(response.tools) == 1
        assert response.tools[0].id == "search"
        assert "web" in response.categories
        assert "search" in response.essential_tools
    
    def test_empty_tools_response(self):
        """Test ToolsResponse with empty collections.
        
        Verifies that a ToolsResponse can be created with empty lists.
        """
        response = ToolsResponse(
            tools=[],
            categories=[],
            essential_tools=[]
        )
        
        assert len(response.tools) == 0
        assert len(response.categories) == 0
        assert len(response.essential_tools) == 0

    def test_system_config_response(self):
        """Test SystemConfigResponse creation and validation.
        
        Verifies that a SystemConfigResponse correctly wraps all configuration objects.
        """
        model = ModelInfo(id="gpt-4", name="GPT-4", provider="openai")
        persona = PersonaInfo(id="assistant", name="Assistant")
        tool = ToolInfo(id="search", name="Search Tool", category="web")
        
        response = SystemConfigResponse(
            models=[model],
            personas=[persona],
            tools=[tool],
            tool_categories=["web", "utility"],
            essential_tools=["search"]
        )
        
        assert len(response.models) == 1
        assert response.models[0].id == "gpt-4"
        assert len(response.personas) == 1
        assert response.personas[0].id == "assistant"
        assert len(response.tools) == 1
        assert response.tools[0].id == "search"
        assert "web" in response.tool_categories
        assert "search" in response.essential_tools
    
    def test_empty_system_config_response(self):
        """Test SystemConfigResponse with empty collections.
        
        Verifies that a SystemConfigResponse can be created with empty lists.
        """
        response = SystemConfigResponse(
            models=[],
            personas=[],
            tools=[],
            tool_categories=[],
            essential_tools=[]
        )
        
        assert len(response.models) == 0
        assert len(response.personas) == 0
        assert len(response.tools) == 0
        assert len(response.tool_categories) == 0
        assert len(response.essential_tools) == 0
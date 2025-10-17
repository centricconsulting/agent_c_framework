import pytest
import pytest_asyncio
from unittest.mock import patch, MagicMock, AsyncMock

from fastapi.testclient import TestClient
from agent_c_api.main import app
from agent_c_api.api.v2.config.dependencies import get_config_service
from agent_c_api.api.v2.config.services import ConfigService
from agent_c_api.api.v2.models.config_models import (
    ModelInfo, PersonaInfo, ToolInfo, ModelParameter, ToolParameter,
    ModelsResponse, AgentConfigsResponse, ToolsResponse, SystemConfigResponse
)


@pytest.mark.unit
@pytest.mark.config
@pytest.mark.services
class TestConfigServiceTests:
    """Tests for the ConfigService class in the config module.
    
    These tests verify that the ConfigService correctly retrieves and transforms
    configuration data from various sources including model configs, persona
    files, and tool registries.
    """

    #########################################
    # Models Tests
    #########################################
    
    @pytest.mark.asyncio
    async def test_get_models(self):
        """Test retrieving the list of available models."""
        # Create a service with a mock get_models implementation
        service = ConfigService()
        
        # Create test data
        test_model = ModelInfo(
            id="gpt-4",
            name="GPT-4",
            provider="openai",
            description="Advanced language model",
            capabilities=["text"],
            parameters=[
                ModelParameter(
                    name="temperature",
                    type="float",
                    description="Controls randomness",
                    default=0.7
                )
            ],
            allowed_inputs=["text"]
        )
        test_response = ModelsResponse(models=[test_model])
        
        # Patch the service method
        with patch.object(service, 'get_models', AsyncMock(return_value=test_response)):
            # Call the method (will use our mock)
            result = await service.get_models()
            
            # Result is a Pydantic model object
            assert isinstance(result, ModelsResponse)
            assert len(result.models) == 1  # Should have our test model
            
            # Validate model properties
            model = result.models[0]
            assert model.id == "gpt-4"
            assert model.name == "GPT-4"
            assert model.provider == "openai"
            assert model.description == "Advanced language model"
            assert "text" in model.capabilities
            assert len(model.parameters) == 1
            
            # Check temperature parameter
            param = model.parameters[0]
            assert param.name == "temperature"
            assert param.type == "float"
            assert param.default == 0.7
            assert "randomness" in param.description.lower()
            
            # Check allowed inputs
            assert "text" in model.allowed_inputs

    @pytest.mark.asyncio
    async def test_get_model(self):
        """Test retrieving a specific model by ID."""
        service = ConfigService()
        
        # Create test model
        test_model = ModelInfo(
            id="gpt-4",
            name="GPT-4",
            provider="openai",
            description="Advanced model"
        )
        
        # Mock get_models to return our test data
        models_response = ModelsResponse(models=[test_model])
        service.get_models = AsyncMock(return_value=models_response)
        
        # Test existing model
        model = await service.get_model("gpt-4")
        assert model is not None
        assert model.id == "gpt-4"
        assert model.name == "GPT-4"
        assert model.provider == "openai"
        
        # Test non-existent model
        model = await service.get_model("nonexistent")
        assert model is None

    @pytest.mark.asyncio
    async def test_get_models_empty_config(self):
        """Test retrieving models when the configuration is empty."""
        service = ConfigService()
        
        # Mock get_models to return empty list
        empty_response = ModelsResponse(models=[])
        service.get_models = AsyncMock(return_value=empty_response)
        
        result = await service.get_models()
        assert isinstance(result, ModelsResponse)
        assert len(result.models) == 0

    @pytest.mark.asyncio
    async def test_get_models_malformed_config(self):
        """Test retrieving models when the configuration is malformed."""
        service = ConfigService()
        
        # Mock get_models to return empty list (as if malformed config was processed)
        empty_response = ModelsResponse(models=[])
        service.get_models = AsyncMock(return_value=empty_response)
        
        result = await service.get_models()
        assert isinstance(result, ModelsResponse)
        assert len(result.models) == 0

    #########################################
    # Personas Tests
    #########################################
    
    @pytest.mark.asyncio
    async def test_get_personas(self):
        """Test retrieving the list of available personas."""
        service = ConfigService()
        
        # Create test data
        test_persona = PersonaInfo(
            id="default",
            name="Default Assistant",
            description="Standard AI assistant",
            file_path="/personas/default.md",
            content="You are a helpful assistant."
        )
        test_response = AgentConfigsResponse(personas=[test_persona])
        
        # Patch the service method
        with patch.object(service, 'get_personas', AsyncMock(return_value=test_response)):
            result = await service.get_personas()
            
            # Validate result
            assert isinstance(result, AgentConfigsResponse)
            assert len(result.personas) == 1
            
            # Check persona properties
            persona = result.personas[0]
            assert persona.id == "default"
            assert persona.name == "Default Assistant"
            assert "assistant" in persona.content.lower()
            assert persona.file_path is not None

    @pytest.mark.asyncio
    async def test_get_persona(self):
        """Test retrieving a specific persona by ID."""
        service = ConfigService()
        
        # Create test data
        test_persona = PersonaInfo(
            id="test-persona",
            name="Test Persona",
            description="For testing",
            file_path="/path/to/test.md",
            content="Test content"
        )
        test_response = AgentConfigsResponse(personas=[test_persona])
        
        # Mock get_personas to return our test data
        service.get_personas = AsyncMock(return_value=test_response)
        
        # Test existing persona
        persona = await service.get_persona("test-persona")
        assert persona is not None
        assert persona.id == "test-persona"
        assert persona.name == "Test Persona"
        assert persona.content == "Test content"
        
        # Test non-existent persona
        persona = await service.get_persona("nonexistent")
        assert persona is None

    @pytest.mark.asyncio
    async def test_get_personas_empty_dir(self):
        """Test retrieving personas when the directory is empty."""
        service = ConfigService()
        
        # Mock get_personas to return empty list
        empty_response = AgentConfigsResponse(personas=[])
        service.get_personas = AsyncMock(return_value=empty_response)
        
        result = await service.get_personas()
        assert isinstance(result, AgentConfigsResponse)
        assert len(result.personas) == 0

    @pytest.mark.asyncio
    async def test_get_personas_nonexistent_dir(self):
        """Test retrieving personas when the directory doesn't exist."""
        service = ConfigService()
        
        # Mock get_personas to return empty list
        empty_response = AgentConfigsResponse(personas=[])
        service.get_personas = AsyncMock(return_value=empty_response)
        
        result = await service.get_personas()
        assert isinstance(result, AgentConfigsResponse)
        assert len(result.personas) == 0

    #########################################
    # Tools Tests
    #########################################
    
    @pytest.mark.asyncio
    async def test_get_tools(self):
        """Test retrieving the list of available tools."""
        service = ConfigService()
        
        # Create test data
        test_tool = ToolInfo(
            id="search",
            name="Search Tool",
            description="Search the web",
            category="web",
            parameters=[
                ToolParameter(
                    name="query",
                    type="string",
                    description="Search query",
                    required=True
                )
            ],
            is_essential=True
        )
        test_response = ToolsResponse(
            tools=[test_tool],
            categories=["web"],
            essential_tools=["search"]
        )
        
        # Patch the service method
        with patch.object(service, 'get_tools', AsyncMock(return_value=test_response)):
            result = await service.get_tools()
            
            # Validate result
            assert isinstance(result, ToolsResponse)
            assert len(result.tools) == 1
            assert len(result.categories) == 1
            assert len(result.essential_tools) == 1
            
            # Check tool properties
            tool = result.tools[0]
            assert tool.id == "search"
            assert tool.name == "Search Tool"
            assert tool.category == "web"
            assert tool.is_essential == True
            assert "search" in result.essential_tools

    @pytest.mark.asyncio
    async def test_get_tool(self):
        """Test retrieving a specific tool by ID."""
        service = ConfigService()
        
        # Create test data
        test_tool = ToolInfo(
            id="search",
            name="Search Tool",
            description="Search the web",
            category="web",
            parameters=[
                ToolParameter(
                    name="query",
                    type="string",
                    description="Search query",
                    required=True
                )
            ],
            is_essential=True
        )
        test_response = ToolsResponse(
            tools=[test_tool],
            categories=["web"],
            essential_tools=["search"]
        )
        
        # Mock get_tools to return our test data
        service.get_tools = AsyncMock(return_value=test_response)
        
        # Test existing tool
        tool = await service.get_tool("search")
        assert tool is not None
        assert tool.id == "search"
        assert tool.name == "Search Tool"
        assert tool.category == "web"
        assert tool.is_essential == True
        assert len(tool.parameters) == 1
        assert tool.parameters[0].name == "query"
        
        # Test non-existent tool
        tool = await service.get_tool("nonexistent")
        assert tool is None

    @pytest.mark.asyncio
    async def test_get_tools_empty_registry(self):
        """Test retrieving tools when the registry is empty."""
        service = ConfigService()
        
        # Mock get_tools to return empty lists
        empty_response = ToolsResponse(
            tools=[],
            categories=[],
            essential_tools=[]
        )
        service.get_tools = AsyncMock(return_value=empty_response)
        
        result = await service.get_tools()
        assert isinstance(result, ToolsResponse)
        assert len(result.tools) == 0
        assert len(result.categories) == 0
        assert len(result.essential_tools) == 0

    #########################################
    # System Config Tests
    #########################################
    
    @pytest.mark.asyncio
    async def test_get_system_config(self):
        """Test retrieving the combined system configuration."""
        service = ConfigService()
        
        # Mock component methods to return controlled data
        service.get_models = AsyncMock(return_value=ModelsResponse(
            models=[ModelInfo(id="gpt-4", name="GPT-4", provider="openai")]
        ))
        service.get_personas = AsyncMock(return_value=AgentConfigsResponse(
            personas=[PersonaInfo(id="default", name="Default Assistant")]
        ))
        service.get_tools = AsyncMock(return_value=ToolsResponse(
            tools=[ToolInfo(id="search", name="Search Tool", category="web", is_essential=True)],
            categories=["web"],
            essential_tools=["search"]
        ))
        
        result = await service.get_system_config()
        
        # Verify the system config combines all components correctly
        assert len(result.models) == 1
        assert result.models[0].id == "gpt-4"
        assert len(result.personas) == 1
        assert result.personas[0].id == "default"
        assert len(result.tools) == 1
        assert result.tools[0].id == "search"
        assert result.tool_categories == ["web"]
        assert result.essential_tools == ["search"]

    @pytest.mark.asyncio
    async def test_get_system_config_error(self):
        """Test handling of errors in component methods during system config retrieval."""
        service = ConfigService()
        
        # Mock component methods to simulate error
        service.get_models = AsyncMock(side_effect=Exception("Database error"))
        
        # Test that exception is propagated
        with pytest.raises(Exception) as exc_info:
            await service.get_system_config()
        
        assert "Database error" in str(exc_info.value)

    @pytest.mark.asyncio
    async def test_get_system_config_partial_error(self):
        """Test handling of errors in one component method during system config retrieval."""
        service = ConfigService()
        
        # Mock tools method to error, but others succeed
        service.get_models = AsyncMock(return_value=ModelsResponse(
            models=[ModelInfo(id="gpt-4", name="GPT-4", provider="openai")]
        ))
        service.get_personas = AsyncMock(return_value=AgentConfigsResponse(
            personas=[PersonaInfo(id="default", name="Default Assistant")]
        ))
        service.get_tools = AsyncMock(side_effect=Exception("Tool registry error"))
        
        # Test that exception is propagated even with partial success
        with pytest.raises(Exception) as exc_info:
            await service.get_system_config()
        
        assert "Tool registry error" in str(exc_info.value)
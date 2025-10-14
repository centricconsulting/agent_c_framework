# tests/unit/api/v2/config/test_endpoints.py

import pytest
from unittest.mock import MagicMock, AsyncMock
from fastapi.testclient import TestClient

from agent_c_api.main import app
from agent_c_api.api.v2.config.router import get_config_service
from agent_c_api.api.v2.config.services import ConfigService
from agent_c_api.api.v2.models.config_models import (
    ModelInfo, PersonaInfo, ToolInfo, ToolParameter, ModelParameter,
    ModelsResponse, AgentConfigsResponse, ToolsResponse, SystemConfigResponse
)


# Create test client and service fixtures
@pytest.fixture
def mock_config_service():
    """Create a mock ConfigService for testing.
    
    Returns a preconfigured mock service with AsyncMock methods 
    that can be used to control test behavior.
    
    Returns:
        MagicMock: Configured mock service
    """
    service = MagicMock(spec=ConfigService)
    
    # Setup async methods with AsyncMock
    service.get_models = AsyncMock()
    service.get_model = AsyncMock()
    service.get_personas = AsyncMock()
    service.get_persona = AsyncMock()
    service.get_tools = AsyncMock()
    service.get_tool = AsyncMock()
    service.get_system_config = AsyncMock()
    
    return service


@pytest.fixture
def client(mock_config_service):
    """Test client fixture for the FastAPI application.
    
    Creates a test client with dependency overrides configured to use our
    mock services instead of the real ones.
    
    Args:
        mock_config_service: The mock service fixture
        
    Returns:
        TestClient: Configured test client
    """
    # Set up dependency override to use our mock
    app.dependency_overrides[get_config_service] = lambda: mock_config_service
    
    # Create client with overridden dependencies
    test_client = TestClient(app)
    
    yield test_client
    
    # Clean up after test
    app.dependency_overrides = {}


@pytest.mark.unit
@pytest.mark.config
@pytest.mark.endpoints
class TestConfigEndpoints:
    """Base class for config endpoint tests.
    
    Contains common test logic and helper methods used by child test classes.
    """
    pass


@pytest.mark.unit
@pytest.mark.config
@pytest.mark.endpoints
class TestModelEndpoints(TestConfigEndpoints):
    """Tests for model configuration endpoints.
    
    These tests verify that the /api/v2/config/models endpoints correctly return
    information about available language models and handle error conditions.
    
    Endpoints tested:
    - GET /api/v2/config/models - List all available models
    - GET /api/v2/config/models/{model_id} - Get details of a specific model
    """
    
    def test_list_models_success(self, client, mock_config_service):
        """Test successful listing of available models.
        
        Verifies that the models list endpoint returns the expected response
        structure and data when models are available.
        """
        # Set up mock response
        mock_config_service.get_models.return_value = ModelsResponse(
            models=[
                ModelInfo(
                    id="gpt-4",
                    name="GPT-4",
                    provider="openai",
                    description="Advanced language model",
                    capabilities=["text"],
                    parameters=[],
                    allowed_inputs=["text"]
                )
            ]
        )
        
        # Test endpoint
        response = client.get("/api/v2/config/models")
        
        # Verify response
        assert response.status_code == 200
        data = response.json()
        assert "models" in data
        assert len(data["models"]) == 1
        assert data["models"][0]["id"] == "gpt-4"
        assert data["models"][0]["provider"] == "openai"
        assert data["models"][0]["capabilities"] == ["text"]
        assert "allowed_inputs" in data["models"][0]
        
        # Verify service was called
        mock_config_service.get_models.assert_called_once()
    
    def test_list_models_error(self, client, mock_config_service):
        """Test error handling when listing models fails.
        
        Verifies that the models list endpoint properly handles and reports
        errors that occur during service execution.
        """
        # Set up mock to raise an exception
        mock_config_service.get_models.side_effect = Exception("Failed to retrieve models")
        
        # Test endpoint
        response = client.get("/api/v2/config/models")
        
        # Verify response
        assert response.status_code == 500
        error_detail = response.json()["detail"]
        assert isinstance(error_detail, dict)
        assert error_detail["error_code"] == "MODELS_RETRIEVAL_ERROR"
        assert "Failed to retrieve models" in error_detail["message"]
    
    def test_get_model_success(self, client, mock_config_service):
        """Test successful retrieval of a specific model.
        
        Verifies that the get-model-by-id endpoint returns the expected model
        information when the model exists.
        """
        # Set up mock response
        mock_config_service.get_model.return_value = ModelInfo(
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
        
        # Test endpoint
        response = client.get("/api/v2/config/models/gpt-4")
        
        # Verify response
        assert response.status_code == 200
        data = response.json()
        assert data["id"] == "gpt-4"
        assert data["provider"] == "openai"
        assert data["capabilities"] == ["text"]
        assert len(data["parameters"]) == 1
        assert data["parameters"][0]["name"] == "temperature"
        assert data["allowed_inputs"] == ["text"]
        
        # Verify service was called
        mock_config_service.get_model.assert_called_once_with("gpt-4")
    
    def test_get_model_not_found(self, client, mock_config_service):
        """Test handling of non-existent model requests.
        
        Verifies that the get-model-by-id endpoint returns a proper 404 error
        when the requested model does not exist.
        """
        # Set up mock response
        mock_config_service.get_model.return_value = None
        
        # Test endpoint
        response = client.get("/api/v2/config/models/nonexistent")
        
        # Verify response
        assert response.status_code == 404
        error_detail = response.json()["detail"]
        assert isinstance(error_detail, dict)
        assert error_detail["error_code"] == "MODEL_NOT_FOUND"
        assert "nonexistent" in error_detail["message"]
        assert error_detail["params"]["model_id"] == "nonexistent"


@pytest.mark.unit
@pytest.mark.config
@pytest.mark.endpoints
class TestPersonaEndpoints(TestConfigEndpoints):
    """Tests for persona configuration endpoints.
    
    These tests verify that the /api/v2/config/personas endpoints correctly return
    information about available personas and handle error conditions.
    
    Endpoints tested:
    - GET /api/v2/config/personas - List all available personas
    - GET /api/v2/config/personas/{persona_id} - Get details of a specific persona
    """
    
    def test_list_personas_success(self, client, mock_config_service):
        """Test successful listing of available personas.
        
        Verifies that the personas list endpoint returns the expected response
        structure and data when personas are available.
        """
        # Set up mock response
        mock_config_service.get_personas.return_value = AgentConfigsResponse(
            personas=[
                PersonaInfo(
                    id="default",
                    name="Default Assistant",
                    description="Standard assistant persona",
                    file_path="/path/to/default.md",
                    content="You are a helpful assistant..."
                )
            ]
        )
        
        # Test endpoint
        response = client.get("/api/v2/config/personas")
        
        # Verify response
        assert response.status_code == 200
        data = response.json()
        assert "personas" in data
        assert len(data["personas"]) == 1
        assert data["personas"][0]["id"] == "default"
        assert data["personas"][0]["name"] == "Default Assistant"
        assert "file_path" in data["personas"][0]
        assert "content" in data["personas"][0]
        
        # Verify service was called
        mock_config_service.get_personas.assert_called_once()
    
    def test_list_personas_error(self, client, mock_config_service):
        """Test error handling when listing personas fails.
        
        Verifies that the personas list endpoint properly handles and reports
        errors that occur during service execution.
        """
        # Set up mock to raise an exception
        mock_config_service.get_personas.side_effect = Exception("Failed to retrieve personas")
        
        # Test endpoint
        response = client.get("/api/v2/config/personas")
        
        # Verify response
        assert response.status_code == 500
        error_detail = response.json()["detail"]
        assert isinstance(error_detail, dict)
        assert error_detail["error_code"] == "PERSONAS_RETRIEVAL_ERROR"
        assert "Failed to retrieve personas" in error_detail["message"]
    
    def test_get_persona_success(self, client, mock_config_service):
        """Test successful retrieval of a specific persona.
        
        Verifies that the get-persona-by-id endpoint returns the expected persona
        information when the persona exists.
        """
        # Set up mock response
        mock_config_service.get_persona.return_value = PersonaInfo(
            id="default",
            name="Default Assistant",
            description="Standard assistant persona",
            file_path="/path/to/default.md",
            content="You are a helpful assistant..."
        )
        
        # Test endpoint
        response = client.get("/api/v2/config/personas/default")
        
        # Verify response
        assert response.status_code == 200
        data = response.json()
        assert data["id"] == "default"
        assert data["name"] == "Default Assistant"
        assert "file_path" in data
        assert "content" in data

        # Verify service was called
        mock_config_service.get_persona.assert_called_once_with("default")
    
    def test_get_persona_not_found(self, client, mock_config_service):
        """Test handling of non-existent persona requests.
        
        Verifies that the get-persona-by-id endpoint returns a proper 404 error
        when the requested persona does not exist.
        """
        # Set up mock response
        mock_config_service.get_persona.return_value = None
        
        # Test endpoint
        response = client.get("/api/v2/config/personas/nonexistent")
        
        # Verify response
        assert response.status_code == 404
        error_detail = response.json()["detail"]
        assert isinstance(error_detail, dict)
        assert error_detail["error_code"] == "PERSONA_NOT_FOUND"
        assert "nonexistent" in error_detail["message"]
        assert error_detail["params"]["persona_id"] == "nonexistent"


@pytest.mark.unit
@pytest.mark.config
@pytest.mark.endpoints
class TestToolEndpoints(TestConfigEndpoints):
    """Tests for tool configuration endpoints.
    
    These tests verify that the /api/v2/config/tools endpoints correctly return
    information about available tools and handle error conditions.
    
    Endpoints tested:
    - GET /api/v2/config/tools - List all available tools
    - GET /api/v2/config/tools/{tool_id} - Get details of a specific tool
    """
    
    def test_list_tools_success(self, client, mock_config_service):
        """Test successful listing of available tools.
        
        Verifies that the tools list endpoint returns the expected response
        structure and data when tools are available.
        """
        # Set up mock response
        mock_config_service.get_tools.return_value = ToolsResponse(
            tools=[
                ToolInfo(
                    id="search",
                    name="Search Tool",
                    category="web",
                    description="Search the web",
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
            ],
            categories=["web"],
            essential_tools=["search"]
        )
        
        # Test endpoint
        response = client.get("/api/v2/config/tools")
        
        # Verify response
        assert response.status_code == 200
        data = response.json()
        assert "tools" in data
        assert "categories" in data
        assert "essential_tools" in data
        assert len(data["tools"]) == 1
        assert data["tools"][0]["id"] == "search"
        assert data["tools"][0]["category"] == "web"
        assert data["tools"][0]["is_essential"] == True
        assert len(data["tools"][0]["parameters"]) == 1
        
        # Verify service was called
        mock_config_service.get_tools.assert_called_once()

    def test_list_tools_with_category_filter(self, client, mock_config_service):
        """Test listing tools with category filter.
        
        Verifies that the tools list endpoint correctly handles the category
        filter parameter.
        """
        # Set up mock response - same as above, service filtering is simulated
        mock_config_service.get_tools.return_value = ToolsResponse(
            tools=[
                ToolInfo(
                    id="search",
                    name="Search Tool",
                    category="web",
                    is_essential=True
                )
            ],
            categories=["web"],
            essential_tools=["search"]
        )
        
        # Test endpoint with category parameter
        response = client.get("/api/v2/config/tools?category=web")
        
        # Verify response
        assert response.status_code == 200
        data = response.json()
        assert "tools" in data
        assert len(data["tools"]) == 1  # We should get our mocked data
        
        # Verify service was called
        mock_config_service.get_tools.assert_called_once()
    
    def test_list_tools_error(self, client, mock_config_service):
        """Test error handling when listing tools fails.
        
        Verifies that the tools list endpoint properly handles and reports
        errors that occur during service execution.
        """
        # Set up mock to raise an exception
        mock_config_service.get_tools.side_effect = Exception("Failed to retrieve tools")
        
        # Test endpoint
        response = client.get("/api/v2/config/tools")
        
        # Verify response
        assert response.status_code == 500
        error_detail = response.json()["detail"]
        assert isinstance(error_detail, dict)
        assert error_detail["error_code"] == "TOOLS_RETRIEVAL_ERROR"
        assert "Failed to retrieve tools" in error_detail["message"]
    
    def test_get_tool_success(self, client, mock_config_service):
        """Test successful retrieval of a specific tool.
        
        Verifies that the get-tool-by-id endpoint returns the expected tool
        information when the tool exists.
        """
        # Set up mock response
        mock_config_service.get_tool.return_value = ToolInfo(
            id="search",
            name="Search Tool",
            category="web",
            description="Search the web",
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
        
        # Test endpoint
        response = client.get("/api/v2/config/tools/search")
        
        # Verify response
        assert response.status_code == 200
        data = response.json()
        assert data["id"] == "search"
        assert data["name"] == "Search Tool"
        assert data["category"] == "web"
        assert data["is_essential"] == True
        assert len(data["parameters"]) == 1
        assert data["parameters"][0]["name"] == "query"
        assert data["parameters"][0]["required"] == True
        
        # Verify service was called
        mock_config_service.get_tool.assert_called_once_with("search")
    
    def test_get_tool_not_found(self, client, mock_config_service):
        """Test handling of non-existent tool requests.
        
        Verifies that the get-tool-by-id endpoint returns a proper 404 error
        when the requested tool does not exist.
        """
        # Set up mock response
        mock_config_service.get_tool.return_value = None
        
        # Test endpoint
        response = client.get("/api/v2/config/tools/nonexistent")
        
        # Verify response
        assert response.status_code == 404
        error_detail = response.json()["detail"]
        assert isinstance(error_detail, dict)
        assert error_detail["error_code"] == "TOOL_NOT_FOUND"
        assert "nonexistent" in error_detail["message"]
        assert error_detail["params"]["tool_id"] == "nonexistent"


@pytest.mark.unit
@pytest.mark.config
@pytest.mark.endpoints
class TestSystemConfigEndpoint(TestConfigEndpoints):
    """Tests for the system configuration endpoint.
    
    These tests verify that the /api/v2/config/system endpoint correctly returns
    combined configuration data and handles error conditions.
    
    Endpoints tested:
    - GET /api/v2/config/system - Get combined system configuration
    """
    
    def test_get_system_config_success(self, client, mock_config_service):
        """Test successful retrieval of the system configuration.
        
        Verifies that the system config endpoint returns the expected combined
        configuration data when the request is successful.
        """
        # Set up mock response
        mock_config_service.get_system_config.return_value = SystemConfigResponse(
            models=[
                ModelInfo(id="gpt-4", name="GPT-4", provider="openai")
            ],
            personas=[
                PersonaInfo(id="default", name="Default Persona")
            ],
            tools=[
                ToolInfo(id="search", name="Search Tool", category="web")
            ],
            tool_categories=["web", "utility"],
            essential_tools=["search"]
        )
        
        # Test endpoint
        response = client.get("/api/v2/config/system")
        
        # Verify response
        assert response.status_code == 200
        data = response.json()
        assert "models" in data
        assert "personas" in data
        assert "tools" in data
        assert "tool_categories" in data
        assert "essential_tools" in data
        assert len(data["models"]) == 1
        assert len(data["personas"]) == 1
        assert len(data["tools"]) == 1
        assert len(data["tool_categories"]) == 2
        assert len(data["essential_tools"]) == 1
        
        # Verify service was called
        mock_config_service.get_system_config.assert_called_once()
    
    def test_system_config_error(self, client, mock_config_service):
        """Test error handling for system configuration retrieval.
        
        Verifies that the system config endpoint properly handles and reports
        errors that occur during service execution.
        """
        # Set up mock to raise an exception
        mock_config_service.get_system_config.side_effect = Exception("Config error")
        
        # Test endpoint
        response = client.get("/api/v2/config/system")
        
        # Verify response
        assert response.status_code == 500
        error_detail = response.json()["detail"]
        assert isinstance(error_detail, dict)
        assert error_detail["error_code"] == "CONFIG_RETRIEVAL_ERROR"
        assert "Config error" in error_detail["message"]
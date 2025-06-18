# tests/unit/api/v2/config/conftest.py
import pytest
from unittest.mock import patch, MagicMock, AsyncMock

from fastapi.testclient import TestClient
from fastapi_cache import FastAPICache
from fastapi_cache.decorator import cache as cache_decorator
from fastapi_cache.backends.inmemory import InMemoryBackend

from agent_c_api.main import app
from agent_c_api.api.v2.config.dependencies import get_config_service
from agent_c_api.api.v2.config.services import ConfigService
from agent_c_api.api.v2.models.config_models import (
    ModelInfo, PersonaInfo, ToolInfo, ToolParameter, ModelParameter,
    ModelsResponse, AgentConfigsResponse, ToolsResponse, SystemConfigResponse
)



@pytest.fixture(scope="session", autouse=True)
def init_cache():
    """Initialize the cache once for all tests."""
    FastAPICache.init(InMemoryBackend(), prefix="agent_c_api_cache_test")
    yield


@pytest.fixture(autouse=True, scope="function")
def disable_caching():
    """
    Fixture to monkey patch the cache decorator function to be a no-op for tests.
    """
    # Store the original implementation
    original_implementation = cache_decorator.__wrapped__ if hasattr(cache_decorator, "__wrapped__") else cache_decorator

    # Define a no-op replacement decorator
    def no_op_cache(*args, **kwargs):
        def inner(func):
            return func

        return inner

    # Replace the cache decorator with our no-op version
    import fastapi_cache.decorator
    fastapi_cache.decorator.cache = no_op_cache

    yield

    # Restore original implementation
    fastapi_cache.decorator.cache = original_implementation

@pytest.fixture
def mock_models_config():
    """
    Mock the models configuration data source.
    
    Provides a sample configuration structure similar to the real implementation.
    """
    with patch('agent_c_api.config.config_loader.MODELS_CONFIG', new={
        "vendors": [
            {
                "vendor": "openai",
                "models": [
                    {
                        "id": "gpt-4",
                        "ui_name": "GPT-4",
                        "description": "Advanced language model",
                        "capabilities": ["text"],
                        "parameters": {
                            "temperature": {
                                "type": "float",
                                "default": 0.7,
                                "description": "Controls randomness"
                            }
                        },
                        "allowed_inputs": ["text"]
                    }
                ]
            }
        ]
    }):
        yield

@pytest.fixture
def mock_persona_dir():
    """
    Mock the persona directory and file operations.
    
    Simulates finding and reading persona files from the filesystem.
    """
    with patch('os.path.isdir') as mock_isdir, \
         patch('glob.glob') as mock_glob, \
         patch('builtins.open', create=True) as mock_open:
        # Set up persona directory mock
        mock_isdir.return_value = True
        
        # Mock persona files
        mock_glob.return_value = [
            "/personas/default.md"
        ]
        
        # Mock file read
        mock_file = MagicMock()
        mock_file.__enter__.return_value.read.return_value = "You are a helpful assistant."
        mock_open.return_value = mock_file
        
        yield

@pytest.fixture
def mock_toolset():
    """
    Mock the Agent C toolset.
    
    Simulates tool registry and essential tools configuration.
    """
    with patch('agent_c.Toolset.tool_registry') as mock_registry, \
         patch('agent_c_api.core.user_session_manager.UserSessionManager.ESSENTIAL_TOOLS', new=["search"]):
        # Create mock tool classes
        search_tool = MagicMock()
        search_tool.__name__ = "search"
        search_tool.__doc__ = "Search the web"
        search_tool.__module__ = "agent_c_tools.search"
        
        # Add tools to registry
        mock_registry.__iter__.return_value = [search_tool]
        
        yield

@pytest.fixture
def mock_config_service():
    """
    Mock the ConfigService for endpoint testing.
    
    Returns a ConfigService mock with preset async method returns.
    """
    service = MagicMock(spec=ConfigService)
    
    # Setup async methods with AsyncMock - return Pydantic model objects
    service.get_models = AsyncMock(return_value=ModelsResponse(
        models=[
            ModelInfo(
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
        ]
    ))
    
    service.get_model = AsyncMock(return_value=ModelInfo(
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
    ))
    
    service.get_personas = AsyncMock(return_value=AgentConfigsResponse(
        personas=[
            PersonaInfo(
                id="default",
                name="Default Assistant",
                description="Standard assistant persona",
                file_path="/path/to/default.md",
                content="You are a helpful assistant..."
            )
        ]
    ))
    
    service.get_persona = AsyncMock(return_value=PersonaInfo(
        id="default",
        name="Default Assistant",
        description="Standard assistant persona",
        file_path="/path/to/default.md",
        content="You are a helpful assistant..."
    ))
    
    service.get_tools = AsyncMock(return_value=ToolsResponse(
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
    ))
    
    service.get_tool = AsyncMock(return_value=ToolInfo(
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
    ))
    
    service.get_system_config = AsyncMock(return_value=SystemConfigResponse(
        models=[
            ModelInfo(id="gpt-4", name="GPT-4", provider="openai")
        ],
        personas=[
            PersonaInfo(id="default", name="Default Assistant")
        ],
        tools=[
            ToolInfo(id="search", name="Search Tool", category="web")
        ],
        tool_categories=["web"],
        essential_tools=["search"]
    ))
    
    return service

@pytest.fixture
def client(mock_config_service):
    """
    Create a test client with dependency overrides for ConfigService.
    
    Args:
        mock_config_service: The mocked ConfigService fixture
        
    Returns:
        TestClient: A FastAPI test client with dependencies properly mocked
    """
    # Reset the mock service before use
    mock_config_service.reset_mock()
    
    # Clear any existing overrides
    app.dependency_overrides = {}
    
    # Set up our dependency override to use the mock service
    app.dependency_overrides[get_config_service] = lambda: mock_config_service
    
    # Create a test client using the app with overridden dependencies
    test_client = TestClient(app)
    
    # Yield the client for test use
    yield test_client
    
    # Clear overrides after tests are complete
    app.dependency_overrides = {}
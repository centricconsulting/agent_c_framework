# test_client.py - A simple test to verify our fixture understanding

import pytest
from fastapi.testclient import TestClient

from agent_c_api.main import app
from agent_c_api.api.v2.config.router import get_config_service

@pytest.fixture
def mock_config_service_override(mock_config_service):
    """Ensure our mock is properly attached to the app dependency system"""
    # Clear any existing overrides
    app.dependency_overrides = {}
    
    # Set up our dependency override to use the mock service
    app.dependency_overrides[get_config_service] = lambda: mock_config_service
    
    # Yield the mock for test use
    yield mock_config_service
    
    # Clear overrides after tests are complete
    app.dependency_overrides = {}

@pytest.fixture
def client_with_mocks(mock_config_service_override):
    """Create a test client with our dependency overrides"""
    with TestClient(app) as client:
        yield client

def test_mock_setup(client_with_mocks, mock_config_service_override):
    """Verify our test fixture setup works"""
    # Set the mock return value
    from agent_c_api.api.v2.models.config_models import ModelInfo, ModelsResponse
    mock_config_service_override.get_models.return_value = ModelsResponse(
        models=[
            ModelInfo(
                id="test-model",
                name="Test Model",
                provider="test"
            )
        ]
    )
    
    # Test the endpoint
    response = client_with_mocks.get("/api/v2/config/models")
    
    # Verify we got our mocked data
    assert response.status_code == 200
    data = response.json()
    assert data["models"][0]["id"] == "test-model"
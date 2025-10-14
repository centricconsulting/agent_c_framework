import pytest
import json
from fastapi import status
from fastapi.testclient import TestClient

from agent_c_api.main import app
from agent_c_api.api.v2.config.router import get_config_service
from agent_c_api.api.v2.models.config_models import (
    ModelInfo, PersonaInfo, ToolInfo, ModelParameter, ToolParameter,
    ModelsResponse, AgentConfigsResponse, ToolsResponse, SystemConfigResponse
)


# Fixtures for dependency injection
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
def test_client(mock_config_service_override):
    """Create a test client with our dependency overrides"""
    with TestClient(app) as client:
        yield client


@pytest.mark.unit
@pytest.mark.config
@pytest.mark.router
class TestConfigRouter:
    """Tests for the config module router endpoints.

    These tests verify that the router endpoints correctly interact with the
    ConfigService and return proper responses. They use mock dependencies to
    isolate testing of the API layer.
    """

    #########################################
    # Models Endpoints Tests
    #########################################

    def test_list_models(self, test_client, mock_config_service_override):
        """Test the GET /config/models endpoint."""
        # Set up the mock return value explicitly
        mock_config_service_override.get_models.return_value = ModelsResponse(
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
        )

        response = test_client.get("/api/v2/config/models")

        # Verify response status code
        assert response.status_code == status.HTTP_200_OK

        # Verify response content
        data = response.json()
        assert "models" in data
        assert len(data["models"]) > 0
        assert data["models"][0]["id"] == "gpt-4"
        assert data["models"][0]["name"] == "GPT-4"
        assert data["models"][0]["provider"] == "openai"

        # Verify service method was called once
        mock_config_service_override.get_models.assert_called_once_with()
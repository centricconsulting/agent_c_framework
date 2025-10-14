# src/agent_c_api/tests/v2/conftest.py
import pytest
from unittest.mock import patch, MagicMock
from fastapi.testclient import TestClient
from agent_c_api.main import app
# Cache handling moved to root conftest.py

@pytest.fixture
def client():
    """Test client fixture for the FastAPI application"""
    return TestClient(app)
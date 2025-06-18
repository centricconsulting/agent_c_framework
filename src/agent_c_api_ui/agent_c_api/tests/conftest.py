# tests/conftest.py

import pytest
import pytest_asyncio
import asyncio
from datetime import datetime
from unittest.mock import AsyncMock, MagicMock, patch
from fastapi.testclient import TestClient
from fastapi_cache import FastAPICache
from fastapi_cache.backends.inmemory import InMemoryBackend
from fastapi_cache.decorator import cache as cache_decorator
from agent_c_api.main import app


#################################################
# Global Setup and Teardown Fixtures
#################################################

@pytest_asyncio.fixture(autouse=True, scope="function")
async def setup_test_cache():
    """
    Fixture to properly initialize FastAPICache for testing.
    
    This fixture:
    1. Initializes FastAPICache with an in-memory backend
    2. Ensures each test starts with a clean cache state
    """
    # Initialize cache if not already initialized
    # The try/except handles cases where it might already be initialized
    try:
        FastAPICache.init(InMemoryBackend(), prefix="test_cache")
    except ValueError:
        # Already initialized, just clear it
        await FastAPICache.clear()
    
    # Let the test run
    yield
    
    # Clear the cache after the test
    await FastAPICache.clear()


#################################################
# API Testing Fixtures
#################################################

@pytest.fixture
def client():
    """
    Test client fixture for the FastAPI application.
    
    Returns:
        TestClient: A configured FastAPI test client.
    """
    return TestClient(app)


#################################################
# Mock Core Services
#################################################

@pytest.fixture
def mock_agent_manager():
    """
    Fixture for a mocked UserSessionManager.
    
    Returns:
        MagicMock: A configured mock of the agent manager.
    """
    manager = MagicMock()
    manager.create_session = AsyncMock(return_value=("test_ui_session_id", "test_agent_session_id"))
    manager.update_agent_tools = AsyncMock()
    manager.get_agent_config = AsyncMock(return_value={})
    manager.get_agent_tools = AsyncMock(return_value=["search", "calculator"])
    manager.update_agent_settings = AsyncMock()
    manager.delete_session = AsyncMock(return_value=True)
    
    # Set up mock session data
    manager.ui_sessions = {
        "test_session_1": {
            "model_name": "gpt-4",
            "agent_key": "programmer",
            "created_at": datetime.now(),
            "last_activity": datetime.now(),
            "agent_session_id": "agent_123",
            "temperature": 0.7,
            "reasoning_effort": 30,
            "budget_tokens": None,
            "max_tokens": 2000,
            "custom_prompt": None,
        },
        "test_session_2": {
            "model_name": "claude-3",
            "agent_key": "default",
            "created_at": datetime.now(),
            "last_activity": None,
            "agent_session_id": "agent_456",
            "temperature": 0.9,
            "reasoning_effort": None,
            "budget_tokens": 15000,
            "max_tokens": 4000,
            "custom_prompt": "Custom system prompt",
        }
    }
    
    return manager


#################################################
# Common Test Data Fixtures
#################################################

@pytest.fixture
def sample_session_detail():
    """
    Fixture providing a sample session detail object for testing.
    
    Returns:
        SessionDetail: A populated session detail object.
    """
    from agent_c_api.api.v2.models import SessionDetail
    
    return SessionDetail(
        id="test_session_1",
        model_id="gpt-4",
        persona_id="programmer",
        created_at=datetime.now(),
        last_activity=datetime.now(),
        agent_internal_id="agent_123",
        tools=["search", "calculator"],
        temperature=0.7,
        reasoning_effort=30,
        budget_tokens=None,
        max_tokens=2000,
        custom_prompt=None
    )


@pytest.fixture
def sample_session_list():
    """
    Fixture providing a sample session list response for testing.
    
    Returns:
        SessionListResponse: A populated session list response.
    """
    from agent_c_api.api.v2.models import SessionListResponse, SessionSummary
    
    return SessionListResponse(
        items=[
            SessionSummary(
                id="test_session_1",
                model_id="gpt-4",
                persona_id="programmer",
                created_at=datetime.now(),
                last_activity=datetime.now()
            ),
            SessionSummary(
                id="test_session_2",
                model_id="claude-3",
                persona_id="default",
                created_at=datetime.now(),
                last_activity=None
            )
        ],
        total=2,
        limit=10,
        offset=0
    )
import pytest
import asyncio
from unittest.mock import patch, MagicMock
from fastapi_cache import FastAPICache
from fastapi_cache.backends.inmemory import InMemoryBackend

@pytest.fixture(autouse=True, scope="function")
async def setup_test_cache():
    """
    Fixture to properly initialize FastAPICache for testing and make the cache decorator a no-op.
    
    This fixture:
    1. Initializes the FastAPICache system (required by the framework)
    2. Makes the @cache decorator function as a pass-through (no actual caching)
    3. Cleans up after each test
    """
    # Save the original cache implementation
    original_cache = FastAPICache.cache
    
    # Initialize the cache system with a test backend
    # This is necessary to avoid "You must call init first!" errors
    FastAPICache.init(InMemoryBackend(), prefix="agent_c_api_cache")
    
    # Replace the cache decorator with a pass-through version
    def pass_through_decorator(*args, **kwargs):
        def inner(func):
            # Just return the original function unchanged
            return func
        return inner
    
    # Apply our no-op version
    FastAPICache.cache = pass_through_decorator
    
    # Let the test run
    yield
    
    # Clean up: restore the original cache implementation and clear the cache
    FastAPICache.cache = original_cache
    await FastAPICache.clear()
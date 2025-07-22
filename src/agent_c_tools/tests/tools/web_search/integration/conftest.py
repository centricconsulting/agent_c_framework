"""
Pytest configuration and fixtures for integration tests.
"""
import pytest
import os
import sys
from typing import Dict, Any

# Add the web search tools to Python path
sys.path.insert(0, "../../../src/agent_c_tools/tools/web_search")


@pytest.fixture(scope="session")
def api_keys():
    """Fixture to provide API keys for testing."""
    return {
        "serpapi": os.getenv("SERPAPI_API_KEY"),
        "tavily": os.getenv("TAVILI_API_KEY"),
        "newsapi": os.getenv("NEWSAPI_API_KEY")
    }


@pytest.fixture(scope="session")
def available_engines(api_keys):
    """Fixture to determine which engines are available for testing."""
    from agent_c_tools.tools.web_search.engines.duckduckgo_engine import create_duckduckgo_engine
    from agent_c_tools.tools.web_search.engines.google_serp_engine import create_google_serp_engine
    from agent_c_tools.tools.web_search.engines.tavily_engine import create_tavily_engine
    from agent_c_tools.tools.web_search.engines.wikipedia_engine import create_wikipedia_engine
    from agent_c_tools.tools.web_search.engines.news_api_engine import create_news_api_engine
    from agent_c_tools.tools.web_search.engines.hacker_news_engine import create_hacker_news_engine
    
    engines = {
        "duckduckgo": create_duckduckgo_engine(),
        "google_serp": create_google_serp_engine(),
        "tavily": create_tavily_engine(),
        "wikipedia": create_wikipedia_engine(),
        "news_api": create_news_api_engine(),
        "hacker_news": create_hacker_news_engine()
    }
    
    available = {}
    for name, engine in engines.items():
        try:
            available[name] = engine.is_available()
        except Exception:
            available[name] = False
    
    return available


@pytest.fixture(scope="function")
def web_search_tools():
    """Fixture to provide WebSearchTools instance."""
    from agent_c_tools.tools.web_search.web_search_tools import WebSearchTools
    return WebSearchTools()


@pytest.fixture(scope="function")
def sample_queries():
    """Fixture to provide sample queries for testing."""
    return {
        "web": "Python programming tutorial",
        "news": "technology news today",
        "educational": "quantum physics",
        "research": "climate change research 2024",
        "tech": "programming discussion",
        "flights": "flights from NYC to LAX",
        "events": "concerts in New York"
    }


@pytest.fixture(scope="function")
def test_parameters():
    """Fixture to provide common test parameters."""
    return {
        "max_results": 3,
        "safe_search": "moderate",
        "language": "en",
        "region": "us"
    }


def pytest_configure(config):
    """Configure pytest with custom markers."""
    config.addinivalue_line(
        "markers", "integration: marks tests as integration tests"
    )
    config.addinivalue_line(
        "markers", "network: marks tests that require network access"
    )
    config.addinivalue_line(
        "markers", "requires_api_key: marks tests that require API keys"
    )
    config.addinivalue_line(
        "markers", "slow: marks tests as slow running"
    )


def pytest_collection_modifyitems(config, items):
    """Modify test collection to add markers based on test names."""
    for item in items:
        # Add integration marker to all tests in integration directory
        if "integration" in str(item.fspath):
            item.add_marker(pytest.mark.integration)
        
        # Add network marker to tests that likely need network
        if any(keyword in item.name.lower() for keyword in ["real", "api", "network", "rate"]):
            item.add_marker(pytest.mark.network)
        
        # Add slow marker to tests that are likely slow
        if any(keyword in item.name.lower() for keyword in ["concurrent", "load", "sustained", "sequential"]):
            item.add_marker(pytest.mark.slow)


def pytest_runtest_setup(item):
    """Setup for each test run."""
    # Skip API key tests if keys are not available
    if item.get_closest_marker("requires_api_key"):
        required_keys = []
        
        # Determine required keys based on test name
        if "google_serp" in item.name.lower() or "serp" in item.name.lower():
            required_keys.append("SERPAPI_API_KEY")
        if "tavily" in item.name.lower():
            required_keys.append("TAVILI_API_KEY")
        if "news_api" in item.name.lower() or "newsapi" in item.name.lower():
            required_keys.append("NEWSAPI_API_KEY")
        
        # Check if required keys are available
        missing_keys = [key for key in required_keys if not os.getenv(key)]
        if missing_keys:
            pytest.skip(f"Required API keys not available: {missing_keys}")


@pytest.fixture(scope="function")
def cleanup_environment():
    """Fixture to cleanup environment variables after tests."""
    original_env = dict(os.environ)
    yield
    
    # Restore original environment
    os.environ.clear()
    os.environ.update(original_env)


@pytest.fixture(scope="function")
def mock_api_responses():
    """Fixture to provide mock API responses for testing."""
    return {
        "duckduckgo": {
            "results": [
                {
                    "title": "Test Result 1",
                    "url": "https://example.com/1",
                    "snippet": "Test snippet 1",
                    "published_date": "2024-01-01"
                }
            ]
        },
        "google_serp": {
            "organic_results": [
                {
                    "title": "Google Result 1",
                    "link": "https://google.com/1",
                    "snippet": "Google snippet 1",
                    "date": "2024-01-01"
                }
            ]
        },
        "tavily": {
            "results": [
                {
                    "title": "Research Result 1",
                    "url": "https://research.com/1",
                    "content": "Research content 1",
                    "published_date": "2024-01-01"
                }
            ]
        },
        "wikipedia": {
            "results": [
                {
                    "title": "Wikipedia Article 1",
                    "url": "https://en.wikipedia.org/wiki/Test",
                    "snippet": "Wikipedia content 1",
                    "published_date": "2024-01-01"
                }
            ]
        },
        "news_api": {
            "articles": [
                {
                    "title": "News Article 1",
                    "url": "https://news.com/1",
                    "description": "News description 1",
                    "publishedAt": "2024-01-01T12:00:00Z"
                }
            ]
        },
        "hacker_news": {
            "stories": [
                {
                    "title": "HN Story 1",
                    "url": "https://news.ycombinator.com/item?id=1",
                    "score": 100,
                    "time": "2024-01-01T12:00:00Z"
                }
            ]
        }
    }


@pytest.fixture(scope="function")
def performance_monitor():
    """Fixture to monitor performance during tests."""
    import time
    import psutil
    import os
    
    process = psutil.Process(os.getpid())
    
    start_time = time.time()
    start_memory = process.memory_info().rss
    
    yield
    
    end_time = time.time()
    end_memory = process.memory_info().rss
    
    execution_time = end_time - start_time
    memory_delta = end_memory - start_memory
    
    # Log performance metrics
    print(f"\nPerformance Metrics:")
    print(f"Execution time: {execution_time:.2f}s")
    print(f"Memory delta: {memory_delta / 1024 / 1024:.2f} MB")
    
    # Assert reasonable performance
    assert execution_time < 60.0, f"Test took too long: {execution_time}s"
    assert memory_delta < 50 * 1024 * 1024, f"Memory usage too high: {memory_delta / 1024 / 1024:.2f} MB"
"""
Pytest configuration and fixtures for web search tests.
"""
import pytest
import asyncio
from unittest.mock import Mock, MagicMock, patch
from datetime import datetime, timedelta
from typing import Dict, Any, List, Optional

# Import the web search system components
import sys
import os
sys.path.insert(0, os.path.join(os.path.dirname(__file__), '..', '..', '..', 'src', 'agent_c_tools', 'tools', 'web_search'))

from base.models import (
    SearchParameters, SearchResponse, SearchResult, WebSearchConfig,
    EngineCapabilities, EngineHealthStatus, SearchType, SafeSearchLevel, SearchDepth
)
from base.engine import BaseWebSearchEngine, EngineException
from base.registry import EngineRegistry
from base.router import EngineRouter, QueryAnalyzer
from base.validator import ParameterValidator
from base.standardizer import ResponseStandardizer
from base.error_handler import ErrorHandler


@pytest.fixture
def mock_search_parameters():
    """Create mock SearchParameters for testing."""
    return SearchParameters(
        query="test query",
        engine="duckduckgo",
        search_type=SearchType.WEB,
        max_results=10,
        safe_search=SafeSearchLevel.MODERATE,
        language="en",
        region="US"
    )


@pytest.fixture
def mock_search_result():
    """Create mock SearchResult for testing."""
    return SearchResult(
        title="Test Result",
        url="https://example.com",
        snippet="Test snippet",
        published_date=datetime.now(),
        source="example.com",
        relevance_score=0.95,
        metadata={"test": "data"}
    )


@pytest.fixture
def mock_search_response(mock_search_result):
    """Create mock SearchResponse for testing."""
    return SearchResponse(
        results=[mock_search_result],
        total_results=1,
        search_time=0.5,
        engine_used="duckduckgo",
        query="test query",
        search_type=SearchType.WEB,
        metadata={"test": "response"}
    )


@pytest.fixture
def mock_web_search_config():
    """Create mock WebSearchConfig for testing."""
    return WebSearchConfig(
        engine_name="test_engine",
        api_key="test_key",
        timeout=30,
        max_retries=3,
        rate_limit=100,
        custom_settings={"test": "setting"}
    )


@pytest.fixture
def mock_engine_capabilities():
    """Create mock EngineCapabilities for testing."""
    return EngineCapabilities(
        supported_search_types=[SearchType.WEB, SearchType.NEWS],
        max_results=100,
        supports_safe_search=True,
        supports_date_filtering=True,
        supports_domain_filtering=True,
        supports_language_filtering=True,
        supports_region_filtering=True,
        rate_limit=1000,
        requires_api_key=False
    )


@pytest.fixture
def mock_engine_health_status():
    """Create mock EngineHealthStatus for testing."""
    return EngineHealthStatus(
        is_available=True,
        last_check=datetime.now(),
        response_time=0.5,
        error_rate=0.01,
        status_message="Engine is healthy"
    )


@pytest.fixture
def mock_base_engine(mock_web_search_config, mock_engine_capabilities):
    """Create mock BaseWebSearchEngine for testing."""
    engine = Mock(spec=BaseWebSearchEngine)
    engine.config = mock_web_search_config
    engine.capabilities = mock_engine_capabilities
    engine.name = "test_engine"
    engine.is_available.return_value = True
    engine.supports_search_type.return_value = True
    engine.get_health_status.return_value = mock_engine_health_status
    return engine


@pytest.fixture
def mock_engine_registry():
    """Create mock EngineRegistry for testing."""
    registry = Mock(spec=EngineRegistry)
    registry.get_available_engines.return_value = ["duckduckgo", "google_serp", "tavily"]
    registry.get_engine.return_value = Mock(spec=BaseWebSearchEngine)
    registry.is_engine_available.return_value = True
    return registry


@pytest.fixture
def mock_query_analyzer():
    """Create mock QueryAnalyzer for testing."""
    analyzer = Mock(spec=QueryAnalyzer)
    analyzer.analyze_query.return_value = {
        "detected_categories": ["general"],
        "preferred_engines": ["duckduckgo"],
        "confidence_scores": {"general": 0.8},
        "suggested_search_type": SearchType.WEB
    }
    return analyzer


@pytest.fixture
def mock_engine_router(mock_engine_registry):
    """Create mock EngineRouter for testing."""
    router = Mock(spec=EngineRouter)
    router.route_search_request.return_value = "duckduckgo"
    router.get_routing_recommendations.return_value = {
        "recommended_engine": "duckduckgo",
        "confidence": 0.9,
        "reasoning": "Best for general queries"
    }
    return router


@pytest.fixture
def mock_parameter_validator():
    """Create mock ParameterValidator for testing."""
    validator = Mock(spec=ParameterValidator)
    validator.validate_parameters.return_value = Mock(spec=SearchParameters)
    return validator


@pytest.fixture
def mock_response_standardizer():
    """Create mock ResponseStandardizer for testing."""
    standardizer = Mock(spec=ResponseStandardizer)
    standardizer.standardize_response.return_value = Mock(spec=SearchResponse)
    return standardizer


@pytest.fixture
def mock_error_handler():
    """Create mock ErrorHandler for testing."""
    handler = Mock(spec=ErrorHandler)
    handler.handle_engine_error.return_value = None
    handler.should_retry.return_value = True
    handler.get_fallback_engine.return_value = "duckduckgo"
    return handler


@pytest.fixture
def sample_raw_search_results():
    """Create sample raw search results for testing."""
    return {
        "results": [
            {
                "title": "Test Result 1",
                "url": "https://example1.com",
                "snippet": "Test snippet 1",
                "published_date": "2024-01-01",
                "source": "example1.com"
            },
            {
                "title": "Test Result 2", 
                "url": "https://example2.com",
                "snippet": "Test snippet 2",
                "published_date": "2024-01-02",
                "source": "example2.com"
            }
        ],
        "total_results": 2,
        "search_time": 0.5,
        "metadata": {"engine": "test"}
    }


@pytest.fixture
def sample_engine_configs():
    """Create sample engine configurations for testing."""
    return {
        "duckduckgo": WebSearchConfig(
            engine_name="duckduckgo",
            timeout=30,
            max_retries=3,
            rate_limit=100
        ),
        "google_serp": WebSearchConfig(
            engine_name="google_serp",
            api_key="test_key",
            timeout=30,
            max_retries=3,
            rate_limit=100
        ),
        "tavily": WebSearchConfig(
            engine_name="tavily",
            api_key="test_key",
            timeout=30,
            max_retries=3,
            rate_limit=100
        )
    }


@pytest.fixture
def mock_legacy_tools():
    """Create mock legacy tools for testing engine adapters."""
    mock_duckduckgo = Mock()
    mock_duckduckgo.web_search.return_value = '{"results": [{"title": "Test", "url": "https://test.com"}]}'
    
    mock_google_serp = Mock()
    mock_google_serp.web_search.return_value = '{"results": [{"title": "Test", "url": "https://test.com"}]}'
    
    mock_tavily = Mock()
    mock_tavily.research_search.return_value = '{"results": [{"title": "Test", "url": "https://test.com"}]}'
    
    return {
        "duckduckgo": mock_duckduckgo,
        "google_serp": mock_google_serp,
        "tavily": mock_tavily
    }


@pytest.fixture
def mock_api_responses():
    """Create mock API responses for different engines."""
    return {
        "duckduckgo": {
            "results": [
                {
                    "title": "DuckDuckGo Result",
                    "url": "https://duckduckgo.com/test",
                    "snippet": "DuckDuckGo test result",
                    "published_date": "2024-01-01"
                }
            ]
        },
        "google_serp": {
            "organic_results": [
                {
                    "title": "Google Result",
                    "link": "https://google.com/test",
                    "snippet": "Google test result",
                    "date": "2024-01-01"
                }
            ]
        },
        "tavily": {
            "results": [
                {
                    "title": "Tavily Result",
                    "url": "https://tavily.com/test",
                    "content": "Tavily test result",
                    "published_date": "2024-01-01"
                }
            ]
        }
    }


@pytest.fixture
def event_loop():
    """Create event loop for async tests."""
    loop = asyncio.new_event_loop()
    yield loop
    loop.close()


# Utility functions for tests
def create_mock_engine_with_capabilities(name: str, capabilities: EngineCapabilities) -> Mock:
    """Create a mock engine with specific capabilities."""
    engine = Mock(spec=BaseWebSearchEngine)
    engine.name = name
    engine.capabilities = capabilities
    engine.is_available.return_value = True
    engine.supports_search_type.return_value = True
    return engine


def create_validation_error_response(parameter: str, message: str) -> Dict[str, Any]:
    """Create a validation error response for testing."""
    return {
        "error": "ValidationError",
        "parameter": parameter,
        "message": message
    }


def create_engine_error_response(engine: str, message: str) -> Dict[str, Any]:
    """Create an engine error response for testing."""
    return {
        "error": "EngineError",
        "engine": engine,
        "message": message
    }
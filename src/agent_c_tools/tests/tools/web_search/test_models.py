"""
Unit tests for data models in the web search system.
"""
import pytest
from datetime import datetime, timedelta
from typing import Dict, Any, List

from base.models import (
    SearchParameters, SearchResponse, SearchResult, WebSearchConfig,
    EngineCapabilities, EngineHealthStatus, SearchType, SafeSearchLevel, SearchDepth
)


class TestSearchParameters:
    """Test suite for SearchParameters model."""
    
    def test_search_parameters_creation(self):
        """Test SearchParameters creation with required fields."""
        params = SearchParameters(
            query="test query",
            engine="duckduckgo",
            search_type=SearchType.WEB
        )
        
        assert params.query == "test query"
        assert params.engine == "duckduckgo"
        assert params.search_type == SearchType.WEB
        assert params.max_results == 10  # Default value
        assert params.safe_search == SafeSearchLevel.MODERATE  # Default value
    
    def test_search_parameters_with_all_fields(self):
        """Test SearchParameters creation with all fields."""
        params = SearchParameters(
            query="comprehensive test query",
            engine="google_serp",
            search_type=SearchType.NEWS,
            max_results=50,
            safe_search=SafeSearchLevel.STRICT,
            language="en",
            region="US",
            include_domains=["example.com"],
            exclude_domains=["spam.com"],
            date_from=datetime(2024, 1, 1),
            date_to=datetime(2024, 12, 31),
            search_depth=SearchDepth.DEEP,
            include_images=True,
            include_videos=False,
            category="technology",
            sort="publishedAt",
            departure_date=datetime(2024, 6, 1),
            return_date=datetime(2024, 6, 15)
        )
        
        assert params.query == "comprehensive test query"
        assert params.engine == "google_serp"
        assert params.search_type == SearchType.NEWS
        assert params.max_results == 50
        assert params.safe_search == SafeSearchLevel.STRICT
        assert params.language == "en"
        assert params.region == "US"
        assert params.include_domains == ["example.com"]
        assert params.exclude_domains == ["spam.com"]
        assert params.date_from == datetime(2024, 1, 1)
        assert params.date_to == datetime(2024, 12, 31)
        assert params.search_depth == SearchDepth.DEEP
        assert params.include_images is True
        assert params.include_videos is False
        assert params.category == "technology"
        assert params.sort == "publishedAt"
        assert params.departure_date == datetime(2024, 6, 1)
        assert params.return_date == datetime(2024, 6, 15)
    
    def test_search_parameters_validation(self):
        """Test SearchParameters validation."""
        # Test empty query
        with pytest.raises(ValueError):
            SearchParameters(query="", engine="duckduckgo", search_type=SearchType.WEB)
        
        # Test invalid max_results
        with pytest.raises(ValueError):
            SearchParameters(
                query="test", engine="duckduckgo", 
                search_type=SearchType.WEB, max_results=0
            )
        
        # Test invalid date range
        with pytest.raises(ValueError):
            SearchParameters(
                query="test", engine="duckduckgo", search_type=SearchType.WEB,
                date_from=datetime(2024, 12, 31), date_to=datetime(2024, 1, 1)
            )
    
    def test_search_parameters_serialization(self):
        """Test SearchParameters serialization."""
        params = SearchParameters(
            query="test query",
            engine="duckduckgo",
            search_type=SearchType.WEB,
            max_results=20
        )
        
        # Should be serializable to dict
        params_dict = params.model_dump()
        assert isinstance(params_dict, dict)
        assert params_dict["query"] == "test query"
        assert params_dict["engine"] == "duckduckgo"
        assert params_dict["search_type"] == "web"
        assert params_dict["max_results"] == 20


class TestSearchResult:
    """Test suite for SearchResult model."""
    
    def test_search_result_creation(self):
        """Test SearchResult creation with required fields."""
        result = SearchResult(
            title="Test Result",
            url="https://example.com",
            snippet="Test snippet"
        )
        
        assert result.title == "Test Result"
        assert result.url == "https://example.com"
        assert result.snippet == "Test snippet"
        assert result.published_date is None
        assert result.source == ""
        assert result.relevance_score == 0.0
        assert result.metadata == {}
    
    def test_search_result_with_all_fields(self):
        """Test SearchResult creation with all fields."""
        published_date = datetime(2024, 1, 1, 12, 0, 0)
        metadata = {"custom_field": "custom_value"}
        
        result = SearchResult(
            title="Complete Test Result",
            url="https://example.com/article",
            snippet="Complete test snippet with more details",
            published_date=published_date,
            source="example.com",
            relevance_score=0.95,
            metadata=metadata
        )
        
        assert result.title == "Complete Test Result"
        assert result.url == "https://example.com/article"
        assert result.snippet == "Complete test snippet with more details"
        assert result.published_date == published_date
        assert result.source == "example.com"
        assert result.relevance_score == 0.95
        assert result.metadata == metadata
    
    def test_search_result_validation(self):
        """Test SearchResult validation."""
        # Test empty title
        with pytest.raises(ValueError):
            SearchResult(title="", url="https://example.com", snippet="test")
        
        # Test empty URL
        with pytest.raises(ValueError):
            SearchResult(title="test", url="", snippet="test")
        
        # Test invalid relevance score
        with pytest.raises(ValueError):
            SearchResult(
                title="test", url="https://example.com", 
                snippet="test", relevance_score=1.5
            )
        
        with pytest.raises(ValueError):
            SearchResult(
                title="test", url="https://example.com", 
                snippet="test", relevance_score=-0.1
            )
    
    def test_search_result_serialization(self):
        """Test SearchResult serialization."""
        result = SearchResult(
            title="Test Result",
            url="https://example.com",
            snippet="Test snippet",
            relevance_score=0.8
        )
        
        result_dict = result.model_dump()
        assert isinstance(result_dict, dict)
        assert result_dict["title"] == "Test Result"
        assert result_dict["url"] == "https://example.com"
        assert result_dict["snippet"] == "Test snippet"
        assert result_dict["relevance_score"] == 0.8


class TestSearchResponse:
    """Test suite for SearchResponse model."""
    
    def test_search_response_creation(self):
        """Test SearchResponse creation."""
        results = [
            SearchResult(
                title="Result 1",
                url="https://example1.com",
                snippet="Snippet 1"
            ),
            SearchResult(
                title="Result 2",
                url="https://example2.com",
                snippet="Snippet 2"
            )
        ]
        
        response = SearchResponse(
            results=results,
            total_results=2,
            search_time=0.5,
            engine_used="duckduckgo",
            query="test query",
            search_type=SearchType.WEB
        )
        
        assert len(response.results) == 2
        assert response.total_results == 2
        assert response.search_time == 0.5
        assert response.engine_used == "duckduckgo"
        assert response.query == "test query"
        assert response.search_type == SearchType.WEB
        assert response.metadata == {}
    
    def test_search_response_with_metadata(self):
        """Test SearchResponse with metadata."""
        metadata = {
            "api_version": "v1",
            "rate_limit": "100/hour",
            "custom_data": {"key": "value"}
        }
        
        response = SearchResponse(
            results=[],
            total_results=0,
            search_time=0.3,
            engine_used="google_serp",
            query="empty query",
            search_type=SearchType.NEWS,
            metadata=metadata
        )
        
        assert response.metadata == metadata
        assert response.metadata["api_version"] == "v1"
        assert response.metadata["custom_data"]["key"] == "value"
    
    def test_search_response_validation(self):
        """Test SearchResponse validation."""
        # Test negative search time
        with pytest.raises(ValueError):
            SearchResponse(
                results=[], total_results=0, search_time=-0.1,
                engine_used="duckduckgo", query="test", search_type=SearchType.WEB
            )
        
        # Test negative total results
        with pytest.raises(ValueError):
            SearchResponse(
                results=[], total_results=-1, search_time=0.5,
                engine_used="duckduckgo", query="test", search_type=SearchType.WEB
            )
        
        # Test empty query
        with pytest.raises(ValueError):
            SearchResponse(
                results=[], total_results=0, search_time=0.5,
                engine_used="duckduckgo", query="", search_type=SearchType.WEB
            )
    
    def test_search_response_serialization(self):
        """Test SearchResponse serialization."""
        results = [
            SearchResult(title="Test", url="https://example.com", snippet="Test")
        ]
        
        response = SearchResponse(
            results=results,
            total_results=1,
            search_time=0.5,
            engine_used="duckduckgo",
            query="test query",
            search_type=SearchType.WEB
        )
        
        response_dict = response.model_dump()
        assert isinstance(response_dict, dict)
        assert len(response_dict["results"]) == 1
        assert response_dict["total_results"] == 1
        assert response_dict["search_time"] == 0.5
        assert response_dict["engine_used"] == "duckduckgo"
        assert response_dict["query"] == "test query"
        assert response_dict["search_type"] == "web"


class TestWebSearchConfig:
    """Test suite for WebSearchConfig model."""
    
    def test_web_search_config_creation(self):
        """Test WebSearchConfig creation."""
        config = WebSearchConfig(
            engine_name="test_engine",
            api_key="test_key",
            timeout=30,
            max_retries=3,
            rate_limit=100
        )
        
        assert config.engine_name == "test_engine"
        assert config.api_key == "test_key"
        assert config.timeout == 30
        assert config.max_retries == 3
        assert config.rate_limit == 100
        assert config.custom_settings == {}
    
    def test_web_search_config_with_custom_settings(self):
        """Test WebSearchConfig with custom settings."""
        custom_settings = {
            "custom_param": "custom_value",
            "advanced_option": True,
            "numeric_setting": 42
        }
        
        config = WebSearchConfig(
            engine_name="advanced_engine",
            custom_settings=custom_settings
        )
        
        assert config.custom_settings == custom_settings
        assert config.custom_settings["custom_param"] == "custom_value"
        assert config.custom_settings["advanced_option"] is True
        assert config.custom_settings["numeric_setting"] == 42
    
    def test_web_search_config_validation(self):
        """Test WebSearchConfig validation."""
        # Test empty engine name
        with pytest.raises(ValueError):
            WebSearchConfig(engine_name="")
        
        # Test negative timeout
        with pytest.raises(ValueError):
            WebSearchConfig(engine_name="test", timeout=-1)
        
        # Test negative max_retries
        with pytest.raises(ValueError):
            WebSearchConfig(engine_name="test", max_retries=-1)
        
        # Test negative rate_limit
        with pytest.raises(ValueError):
            WebSearchConfig(engine_name="test", rate_limit=-1)


class TestEngineCapabilities:
    """Test suite for EngineCapabilities model."""
    
    def test_engine_capabilities_creation(self):
        """Test EngineCapabilities creation."""
        capabilities = EngineCapabilities(
            supported_search_types=[SearchType.WEB, SearchType.NEWS],
            max_results=100,
            supports_safe_search=True,
            supports_date_filtering=True,
            supports_domain_filtering=False,
            supports_language_filtering=True,
            supports_region_filtering=True,
            rate_limit=1000,
            requires_api_key=True
        )
        
        assert SearchType.WEB in capabilities.supported_search_types
        assert SearchType.NEWS in capabilities.supported_search_types
        assert capabilities.max_results == 100
        assert capabilities.supports_safe_search is True
        assert capabilities.supports_date_filtering is True
        assert capabilities.supports_domain_filtering is False
        assert capabilities.supports_language_filtering is True
        assert capabilities.supports_region_filtering is True
        assert capabilities.rate_limit == 1000
        assert capabilities.requires_api_key is True
    
    def test_engine_capabilities_validation(self):
        """Test EngineCapabilities validation."""
        # Test empty supported search types
        with pytest.raises(ValueError):
            EngineCapabilities(
                supported_search_types=[],
                max_results=100,
                rate_limit=1000,
                requires_api_key=False
            )
        
        # Test negative max_results
        with pytest.raises(ValueError):
            EngineCapabilities(
                supported_search_types=[SearchType.WEB],
                max_results=-1,
                rate_limit=1000,
                requires_api_key=False
            )
        
        # Test negative rate_limit
        with pytest.raises(ValueError):
            EngineCapabilities(
                supported_search_types=[SearchType.WEB],
                max_results=100,
                rate_limit=-1,
                requires_api_key=False
            )


class TestEngineHealthStatus:
    """Test suite for EngineHealthStatus model."""
    
    def test_engine_health_status_creation(self):
        """Test EngineHealthStatus creation."""
        last_check = datetime.now()
        
        status = EngineHealthStatus(
            is_available=True,
            last_check=last_check,
            response_time=0.5,
            error_rate=0.01,
            status_message="Engine is healthy"
        )
        
        assert status.is_available is True
        assert status.last_check == last_check
        assert status.response_time == 0.5
        assert status.error_rate == 0.01
        assert status.status_message == "Engine is healthy"
    
    def test_engine_health_status_validation(self):
        """Test EngineHealthStatus validation."""
        # Test negative response_time
        with pytest.raises(ValueError):
            EngineHealthStatus(
                is_available=True,
                last_check=datetime.now(),
                response_time=-0.1,
                error_rate=0.01
            )
        
        # Test invalid error_rate (> 1.0)
        with pytest.raises(ValueError):
            EngineHealthStatus(
                is_available=True,
                last_check=datetime.now(),
                response_time=0.5,
                error_rate=1.5
            )
        
        # Test invalid error_rate (< 0.0)
        with pytest.raises(ValueError):
            EngineHealthStatus(
                is_available=True,
                last_check=datetime.now(),
                response_time=0.5,
                error_rate=-0.1
            )


class TestEnums:
    """Test suite for enum models."""
    
    def test_search_type_enum(self):
        """Test SearchType enum."""
        assert SearchType.WEB == "web"
        assert SearchType.NEWS == "news"
        assert SearchType.EDUCATIONAL == "educational"
        assert SearchType.RESEARCH == "research"
        assert SearchType.TECH == "tech"
        assert SearchType.FLIGHTS == "flights"
        assert SearchType.EVENTS == "events"
        
        # Test enum values
        all_types = list(SearchType)
        assert len(all_types) == 7
        assert SearchType.WEB in all_types
        assert SearchType.NEWS in all_types
    
    def test_safe_search_level_enum(self):
        """Test SafeSearchLevel enum."""
        assert SafeSearchLevel.OFF == "off"
        assert SafeSearchLevel.MODERATE == "moderate"
        assert SafeSearchLevel.STRICT == "strict"
        
        # Test enum values
        all_levels = list(SafeSearchLevel)
        assert len(all_levels) == 3
        assert SafeSearchLevel.OFF in all_levels
        assert SafeSearchLevel.MODERATE in all_levels
        assert SafeSearchLevel.STRICT in all_levels
    
    def test_search_depth_enum(self):
        """Test SearchDepth enum."""
        assert SearchDepth.BASIC == "basic"
        assert SearchDepth.STANDARD == "standard"
        assert SearchDepth.DEEP == "deep"
        
        # Test enum values
        all_depths = list(SearchDepth)
        assert len(all_depths) == 3
        assert SearchDepth.BASIC in all_depths
        assert SearchDepth.STANDARD in all_depths
        assert SearchDepth.DEEP in all_depths


class TestModelIntegration:
    """Test suite for model integration scenarios."""
    
    def test_complete_search_workflow(self):
        """Test complete search workflow with all models."""
        # Create search parameters
        params = SearchParameters(
            query="integration test query",
            engine="duckduckgo",
            search_type=SearchType.WEB,
            max_results=20,
            safe_search=SafeSearchLevel.MODERATE
        )
        
        # Create search results
        results = [
            SearchResult(
                title="Integration Test Result 1",
                url="https://example1.com",
                snippet="First integration test result",
                relevance_score=0.9
            ),
            SearchResult(
                title="Integration Test Result 2",
                url="https://example2.com",
                snippet="Second integration test result",
                relevance_score=0.8
            )
        ]
        
        # Create search response
        response = SearchResponse(
            results=results,
            total_results=2,
            search_time=0.75,
            engine_used="duckduckgo",
            query="integration test query",
            search_type=SearchType.WEB,
            metadata={"integration_test": True}
        )
        
        # Verify complete workflow
        assert response.query == params.query
        assert response.engine_used == params.engine
        assert response.search_type == params.search_type
        assert len(response.results) == 2
        assert response.total_results == 2
        assert response.metadata["integration_test"] is True
    
    def test_model_serialization_roundtrip(self):
        """Test model serialization and deserialization."""
        # Create original models
        original_params = SearchParameters(
            query="serialization test",
            engine="test_engine",
            search_type=SearchType.NEWS,
            max_results=15
        )
        
        original_result = SearchResult(
            title="Serialization Test",
            url="https://serialization.test",
            snippet="Testing serialization",
            relevance_score=0.85
        )
        
        # Serialize to dict
        params_dict = original_params.model_dump()
        result_dict = original_result.model_dump()
        
        # Deserialize back to models
        restored_params = SearchParameters(**params_dict)
        restored_result = SearchResult(**result_dict)
        
        # Verify roundtrip integrity
        assert restored_params.query == original_params.query
        assert restored_params.engine == original_params.engine
        assert restored_params.search_type == original_params.search_type
        assert restored_params.max_results == original_params.max_results
        
        assert restored_result.title == original_result.title
        assert restored_result.url == original_result.url
        assert restored_result.snippet == original_result.snippet
        assert restored_result.relevance_score == original_result.relevance_score
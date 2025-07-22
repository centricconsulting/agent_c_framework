"""
Unit tests for parameter validation in the web search system.
"""
import pytest
from datetime import datetime, timedelta
from typing import Dict, Any

from base.validator import ParameterValidator, ValidationError
from base.models import SearchParameters, SearchType, SafeSearchLevel, SearchDepth


class TestParameterValidator:
    """Test suite for ParameterValidator class."""
    
    def setup_method(self):
        """Set up test fixtures before each test method."""
        self.validator = ParameterValidator(
            available_engines=["duckduckgo", "google_serp", "tavily", "wikipedia", "news_api", "hacker_news"]
        )
    
    def test_validator_initialization(self):
        """Test ParameterValidator initialization."""
        validator = ParameterValidator()
        assert validator is not None
        
        validator_with_engines = ParameterValidator(available_engines=["duckduckgo", "google_serp"])
        assert validator_with_engines is not None
    
    def test_validate_basic_parameters(self):
        """Test validation of basic search parameters."""
        raw_params = {
            "query": "test search",
            "engine": "duckduckgo",
            "max_results": 10
        }
        
        params = self.validator.validate_parameters(raw_params)
        
        assert isinstance(params, SearchParameters)
        assert params.query == "test search"
        assert params.engine == "duckduckgo"
        assert params.max_results == 10
    
    def test_validate_query_parameter(self):
        """Test query parameter validation."""
        # Valid query
        raw_params = {"query": "valid search query"}
        params = self.validator.validate_parameters(raw_params)
        assert params.query == "valid search query"
        
        # Empty query should raise error
        with pytest.raises(ValidationError) as exc_info:
            self.validator.validate_parameters({"query": ""})
        assert "query" in str(exc_info.value)
        
        # Missing query should raise error
        with pytest.raises(ValidationError) as exc_info:
            self.validator.validate_parameters({})
        assert "query" in str(exc_info.value)
        
        # Query too long should raise error
        long_query = "a" * 1001
        with pytest.raises(ValidationError) as exc_info:
            self.validator.validate_parameters({"query": long_query})
        assert "query" in str(exc_info.value)
    
    def test_validate_engine_parameter(self):
        """Test engine parameter validation."""
        # Valid engine
        raw_params = {"query": "test", "engine": "duckduckgo"}
        params = self.validator.validate_parameters(raw_params)
        assert params.engine == "duckduckgo"
        
        # Invalid engine should raise error
        with pytest.raises(ValidationError) as exc_info:
            self.validator.validate_parameters({"query": "test", "engine": "invalid_engine"})
        assert "engine" in str(exc_info.value)
        
        # Auto engine should work
        raw_params = {"query": "test", "engine": "auto"}
        params = self.validator.validate_parameters(raw_params)
        assert params.engine == "auto"
    
    def test_validate_search_type_parameter(self):
        """Test search_type parameter validation."""
        # Valid search types
        for search_type in ["web", "news", "educational", "research", "tech", "flights", "events"]:
            raw_params = {"query": "test", "search_type": search_type}
            params = self.validator.validate_parameters(raw_params)
            assert params.search_type == SearchType(search_type)
        
        # Invalid search type should raise error
        with pytest.raises(ValidationError) as exc_info:
            self.validator.validate_parameters({"query": "test", "search_type": "invalid"})
        assert "search_type" in str(exc_info.value)
    
    def test_validate_max_results_parameter(self):
        """Test max_results parameter validation."""
        # Valid max_results
        raw_params = {"query": "test", "max_results": 20}
        params = self.validator.validate_parameters(raw_params)
        assert params.max_results == 20
        
        # Max results too high should be capped
        raw_params = {"query": "test", "max_results": 200}
        params = self.validator.validate_parameters(raw_params)
        assert params.max_results == 100  # Should be capped to maximum
        
        # Max results too low should raise error
        with pytest.raises(ValidationError) as exc_info:
            self.validator.validate_parameters({"query": "test", "max_results": 0})
        assert "max_results" in str(exc_info.value)
        
        # Invalid type should raise error
        with pytest.raises(ValidationError) as exc_info:
            self.validator.validate_parameters({"query": "test", "max_results": "invalid"})
        assert "max_results" in str(exc_info.value)
    
    def test_validate_safe_search_parameter(self):
        """Test safe_search parameter validation."""
        # Valid safe search levels
        for level in ["off", "moderate", "strict"]:
            raw_params = {"query": "test", "safe_search": level}
            params = self.validator.validate_parameters(raw_params)
            assert params.safe_search == SafeSearchLevel(level)
        
        # Invalid safe search level should raise error
        with pytest.raises(ValidationError) as exc_info:
            self.validator.validate_parameters({"query": "test", "safe_search": "invalid"})
        assert "safe_search" in str(exc_info.value)
    
    def test_validate_language_parameter(self):
        """Test language parameter validation."""
        # Valid language codes
        for lang in ["en", "es", "fr", "de", "zh"]:
            raw_params = {"query": "test", "language": lang}
            params = self.validator.validate_parameters(raw_params)
            assert params.language == lang
        
        # Invalid language code should raise error
        with pytest.raises(ValidationError) as exc_info:
            self.validator.validate_parameters({"query": "test", "language": "invalid"})
        assert "language" in str(exc_info.value)
    
    def test_validate_region_parameter(self):
        """Test region parameter validation."""
        # Valid region codes
        for region in ["US", "GB", "CA", "AU", "DE"]:
            raw_params = {"query": "test", "region": region}
            params = self.validator.validate_parameters(raw_params)
            assert params.region == region
        
        # Invalid region code should raise error
        with pytest.raises(ValidationError) as exc_info:
            self.validator.validate_parameters({"query": "test", "region": "invalid"})
        assert "region" in str(exc_info.value)
    
    def test_validate_date_parameters(self):
        """Test date parameter validation."""
        # Valid date strings
        raw_params = {
            "query": "test",
            "date_from": "2024-01-01",
            "date_to": "2024-12-31"
        }
        params = self.validator.validate_parameters(raw_params)
        assert params.date_from is not None
        assert params.date_to is not None
        assert params.date_from < params.date_to
        
        # Invalid date format should raise error
        with pytest.raises(ValidationError) as exc_info:
            self.validator.validate_parameters({"query": "test", "date_from": "invalid-date"})
        assert "date_from" in str(exc_info.value)
        
        # Date range validation (date_from > date_to)
        with pytest.raises(ValidationError) as exc_info:
            self.validator.validate_parameters({
                "query": "test",
                "date_from": "2024-12-31",
                "date_to": "2024-01-01"
            })
        assert "date range" in str(exc_info.value).lower()
    
    def test_validate_domain_parameters(self):
        """Test domain parameter validation."""
        # Valid include domains
        raw_params = {
            "query": "test",
            "include_domains": ["example.com", "test.org"]
        }
        params = self.validator.validate_parameters(raw_params)
        assert params.include_domains == ["example.com", "test.org"]
        
        # Valid exclude domains
        raw_params = {
            "query": "test",
            "exclude_domains": ["spam.com", "ads.net"]
        }
        params = self.validator.validate_parameters(raw_params)
        assert params.exclude_domains == ["spam.com", "ads.net"]
        
        # Invalid domain format should raise error
        with pytest.raises(ValidationError) as exc_info:
            self.validator.validate_parameters({
                "query": "test",
                "include_domains": ["invalid domain"]
            })
        assert "include_domains" in str(exc_info.value)
        
        # Conflicting domains should raise error
        with pytest.raises(ValidationError) as exc_info:
            self.validator.validate_parameters({
                "query": "test",
                "include_domains": ["example.com"],
                "exclude_domains": ["example.com"]
            })
        assert "conflict" in str(exc_info.value).lower()
    
    def test_validate_search_depth_parameter(self):
        """Test search_depth parameter validation."""
        # Valid search depths
        for depth in ["basic", "standard", "deep"]:
            raw_params = {"query": "test", "search_depth": depth}
            params = self.validator.validate_parameters(raw_params)
            assert params.search_depth == SearchDepth(depth)
        
        # Invalid search depth should raise error
        with pytest.raises(ValidationError) as exc_info:
            self.validator.validate_parameters({"query": "test", "search_depth": "invalid"})
        assert "search_depth" in str(exc_info.value)
    
    def test_validate_news_specific_parameters(self):
        """Test news-specific parameter validation."""
        # Valid news category
        raw_params = {
            "query": "test",
            "search_type": "news",
            "category": "technology"
        }
        params = self.validator.validate_parameters(raw_params)
        assert params.category == "technology"
        
        # Valid sort parameter
        raw_params = {
            "query": "test",
            "search_type": "news",
            "sort": "publishedAt"
        }
        params = self.validator.validate_parameters(raw_params)
        assert params.sort == "publishedAt"
        
        # Invalid news category should raise error
        with pytest.raises(ValidationError) as exc_info:
            self.validator.validate_parameters({
                "query": "test",
                "search_type": "news",
                "category": "invalid_category"
            })
        assert "category" in str(exc_info.value)
    
    def test_validate_flight_specific_parameters(self):
        """Test flight-specific parameter validation."""
        raw_params = {
            "query": "flights from NYC to LAX",
            "search_type": "flights",
            "departure_date": "2024-06-01",
            "return_date": "2024-06-15"
        }
        params = self.validator.validate_parameters(raw_params)
        assert params.departure_date is not None
        assert params.return_date is not None
        
        # Return date before departure date should raise error
        with pytest.raises(ValidationError) as exc_info:
            self.validator.validate_parameters({
                "query": "flights",
                "search_type": "flights",
                "departure_date": "2024-06-15",
                "return_date": "2024-06-01"
            })
        assert "return_date" in str(exc_info.value)
    
    def test_validate_boolean_parameters(self):
        """Test boolean parameter validation."""
        # Valid boolean values
        raw_params = {
            "query": "test",
            "include_images": True,
            "include_videos": False
        }
        params = self.validator.validate_parameters(raw_params)
        assert params.include_images is True
        assert params.include_videos is False
        
        # String boolean values should be converted
        raw_params = {
            "query": "test",
            "include_images": "true",
            "include_videos": "false"
        }
        params = self.validator.validate_parameters(raw_params)
        assert params.include_images is True
        assert params.include_videos is False
    
    def test_validate_parameter_combinations(self):
        """Test validation of parameter combinations."""
        # Valid combination
        raw_params = {
            "query": "test",
            "engine": "google_serp",
            "search_type": "flights",
            "departure_date": "2024-06-01"
        }
        params = self.validator.validate_parameters(raw_params)
        assert params.engine == "google_serp"
        assert params.search_type == SearchType.FLIGHTS
        
        # Invalid combination (flights with non-Google engine)
        with pytest.raises(ValidationError) as exc_info:
            self.validator.validate_parameters({
                "query": "test",
                "engine": "duckduckgo",
                "search_type": "flights"
            })
        assert "flights" in str(exc_info.value).lower()
    
    def test_update_available_engines(self):
        """Test updating available engines."""
        new_engines = ["duckduckgo", "tavily"]
        self.validator.update_available_engines(new_engines)
        
        # Should accept valid engine
        raw_params = {"query": "test", "engine": "duckduckgo"}
        params = self.validator.validate_parameters(raw_params)
        assert params.engine == "duckduckgo"
        
        # Should reject engine not in updated list
        with pytest.raises(ValidationError) as exc_info:
            self.validator.validate_parameters({"query": "test", "engine": "google_serp"})
        assert "engine" in str(exc_info.value)
    
    def test_get_validation_schema(self):
        """Test getting validation schema."""
        schema = self.validator.get_validation_schema()
        
        assert isinstance(schema, dict)
        assert "query" in schema
        assert "engine" in schema
        assert "search_type" in schema
        assert "max_results" in schema
        
        # Check that schema contains validation rules
        query_rules = schema["query"]
        assert "required" in query_rules
        assert "type" in query_rules
        assert "max_length" in query_rules
    
    def test_validation_error_details(self):
        """Test that validation errors contain proper details."""
        try:
            self.validator.validate_parameters({"query": ""})
        except ValidationError as e:
            assert e.parameter == "query"
            assert e.value == ""
            assert "empty" in str(e).lower()
        
        try:
            self.validator.validate_parameters({"query": "test", "max_results": -1})
        except ValidationError as e:
            assert e.parameter == "max_results"
            assert e.value == -1
    
    def test_normalize_parameters(self):
        """Test parameter normalization."""
        raw_params = {
            "query": "  Test Query  ",  # Should be stripped
            "engine": "DuckDuckGo",     # Should be lowercased
            "max_results": "10",        # Should be converted to int
            "include_images": "true"    # Should be converted to bool
        }
        
        params = self.validator.validate_parameters(raw_params)
        
        assert params.query == "Test Query"
        assert params.engine == "duckduckgo"
        assert params.max_results == 10
        assert params.include_images is True
    
    def test_default_values(self):
        """Test that default values are applied correctly."""
        raw_params = {"query": "test"}
        params = self.validator.validate_parameters(raw_params)
        
        # Check default values are set
        assert params.engine == "auto"
        assert params.search_type == SearchType.WEB
        assert params.max_results == 10
        assert params.safe_search == SafeSearchLevel.MODERATE
        assert params.language == "en"
        assert params.region == "US"
        assert params.search_depth == SearchDepth.STANDARD
        assert params.include_images is False
        assert params.include_videos is False
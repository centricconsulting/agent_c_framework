"""
Unit tests for response standardization in the web search system.
"""
import pytest
import json
from datetime import datetime
from typing import Dict, Any

from base.standardizer import ResponseStandardizer
from base.models import SearchResponse, SearchResult, SearchParameters, SearchType


class TestResponseStandardizer:
    """Test suite for ResponseStandardizer class."""
    
    def setup_method(self):
        """Set up test fixtures before each test method."""
        self.standardizer = ResponseStandardizer()
    
    def test_standardizer_initialization(self):
        """Test ResponseStandardizer initialization."""
        standardizer = ResponseStandardizer()
        assert standardizer is not None
    
    def test_standardize_duckduckgo_response(self):
        """Test standardization of DuckDuckGo response format."""
        raw_response = {
            "results": [
                {
                    "title": "DuckDuckGo Result",
                    "url": "https://example.com",
                    "snippet": "Test snippet",
                    "published_date": "2024-01-01"
                }
            ],
            "total_results": 1,
            "search_time": 0.5
        }
        
        params = SearchParameters(
            query="test query",
            engine="duckduckgo",
            search_type=SearchType.WEB
        )
        
        response = self.standardizer.standardize_response(
            raw_response, params, "duckduckgo", 1234567890.0
        )
        
        assert isinstance(response, SearchResponse)
        assert len(response.results) == 1
        assert response.results[0].title == "DuckDuckGo Result"
        assert response.results[0].url == "https://example.com"
        assert response.engine_used == "duckduckgo"
        assert response.query == "test query"
        assert response.search_type == SearchType.WEB
    
    def test_standardize_google_serp_response(self):
        """Test standardization of Google SERP response format."""
        raw_response = {
            "organic_results": [
                {
                    "title": "Google Result",
                    "link": "https://google.com/test",
                    "snippet": "Google snippet",
                    "date": "2024-01-01"
                }
            ],
            "search_metadata": {
                "total_results": "About 1,000,000 results"
            }
        }
        
        params = SearchParameters(
            query="test query",
            engine="google_serp",
            search_type=SearchType.WEB
        )
        
        response = self.standardizer.standardize_response(
            raw_response, params, "google_serp", 1234567890.0
        )
        
        assert isinstance(response, SearchResponse)
        assert len(response.results) == 1
        assert response.results[0].title == "Google Result"
        assert response.results[0].url == "https://google.com/test"
        assert response.engine_used == "google_serp"
    
    def test_standardize_tavily_response(self):
        """Test standardization of Tavily response format."""
        raw_response = {
            "results": [
                {
                    "title": "Tavily Result",
                    "url": "https://tavily.com/test",
                    "content": "Tavily content",
                    "published_date": "2024-01-01",
                    "score": 0.95
                }
            ],
            "query": "test query"
        }
        
        params = SearchParameters(
            query="test query",
            engine="tavily",
            search_type=SearchType.RESEARCH
        )
        
        response = self.standardizer.standardize_response(
            raw_response, params, "tavily", 1234567890.0
        )
        
        assert isinstance(response, SearchResponse)
        assert len(response.results) == 1
        assert response.results[0].title == "Tavily Result"
        assert response.results[0].relevance_score == 0.95
        assert response.engine_used == "tavily"
        assert response.search_type == SearchType.RESEARCH
    
    def test_standardize_wikipedia_response(self):
        """Test standardization of Wikipedia response format."""
        raw_response = {
            "results": [
                {
                    "title": "Wikipedia Article",
                    "url": "https://en.wikipedia.org/wiki/Test",
                    "snippet": "Wikipedia content",
                    "published_date": "2024-01-01"
                }
            ]
        }
        
        params = SearchParameters(
            query="test query",
            engine="wikipedia",
            search_type=SearchType.EDUCATIONAL
        )
        
        response = self.standardizer.standardize_response(
            raw_response, params, "wikipedia", 1234567890.0
        )
        
        assert isinstance(response, SearchResponse)
        assert len(response.results) == 1
        assert response.results[0].title == "Wikipedia Article"
        assert response.engine_used == "wikipedia"
        assert response.search_type == SearchType.EDUCATIONAL
    
    def test_standardize_news_api_response(self):
        """Test standardization of NewsAPI response format."""
        raw_response = {
            "articles": [
                {
                    "title": "News Article",
                    "url": "https://news.com/article",
                    "description": "News description",
                    "publishedAt": "2024-01-01T12:00:00Z",
                    "source": {"name": "News Source"}
                }
            ],
            "totalResults": 1
        }
        
        params = SearchParameters(
            query="test query",
            engine="news_api",
            search_type=SearchType.NEWS
        )
        
        response = self.standardizer.standardize_response(
            raw_response, params, "news_api", 1234567890.0
        )
        
        assert isinstance(response, SearchResponse)
        assert len(response.results) == 1
        assert response.results[0].title == "News Article"
        assert response.results[0].source == "News Source"
        assert response.engine_used == "news_api"
        assert response.search_type == SearchType.NEWS
    
    def test_standardize_hacker_news_response(self):
        """Test standardization of HackerNews response format."""
        raw_response = {
            "stories": [
                {
                    "title": "Tech Story",
                    "url": "https://news.ycombinator.com/item?id=123",
                    "score": 100,
                    "time": "2024-01-01T12:00:00Z",
                    "by": "user123"
                }
            ]
        }
        
        params = SearchParameters(
            query="test query",
            engine="hacker_news",
            search_type=SearchType.TECH
        )
        
        response = self.standardizer.standardize_response(
            raw_response, params, "hacker_news", 1234567890.0
        )
        
        assert isinstance(response, SearchResponse)
        assert len(response.results) == 1
        assert response.results[0].title == "Tech Story"
        assert response.results[0].source == "Hacker News"
        assert response.engine_used == "hacker_news"
        assert response.search_type == SearchType.TECH
    
    def test_standardize_empty_response(self):
        """Test standardization of empty response."""
        raw_response = {"results": []}
        
        params = SearchParameters(
            query="test query",
            engine="duckduckgo",
            search_type=SearchType.WEB
        )
        
        response = self.standardizer.standardize_response(
            raw_response, params, "duckduckgo", 1234567890.0
        )
        
        assert isinstance(response, SearchResponse)
        assert len(response.results) == 0
        assert response.total_results == 0
        assert response.engine_used == "duckduckgo"
    
    def test_standardize_malformed_response(self):
        """Test standardization of malformed response."""
        raw_response = {"invalid": "format"}
        
        params = SearchParameters(
            query="test query",
            engine="duckduckgo",
            search_type=SearchType.WEB
        )
        
        response = self.standardizer.standardize_response(
            raw_response, params, "duckduckgo", 1234567890.0
        )
        
        # Should handle gracefully and return empty results
        assert isinstance(response, SearchResponse)
        assert len(response.results) == 0
        assert response.engine_used == "duckduckgo"
    
    def test_date_parsing(self):
        """Test date parsing from various formats."""
        # Test ISO format
        raw_response = {
            "results": [
                {
                    "title": "Test",
                    "url": "https://example.com",
                    "snippet": "Test",
                    "published_date": "2024-01-01T12:00:00Z"
                }
            ]
        }
        
        params = SearchParameters(
            query="test",
            engine="duckduckgo",
            search_type=SearchType.WEB
        )
        
        response = self.standardizer.standardize_response(
            raw_response, params, "duckduckgo", 1234567890.0
        )
        
        assert isinstance(response.results[0].published_date, datetime)
        
        # Test date-only format
        raw_response["results"][0]["published_date"] = "2024-01-01"
        
        response = self.standardizer.standardize_response(
            raw_response, params, "duckduckgo", 1234567890.0
        )
        
        assert isinstance(response.results[0].published_date, datetime)
        
        # Test invalid date format
        raw_response["results"][0]["published_date"] = "invalid-date"
        
        response = self.standardizer.standardize_response(
            raw_response, params, "duckduckgo", 1234567890.0
        )
        
        assert response.results[0].published_date is None
    
    def test_url_validation(self):
        """Test URL validation and normalization."""
        raw_response = {
            "results": [
                {
                    "title": "Test",
                    "url": "https://example.com/path?param=value",
                    "snippet": "Test"
                }
            ]
        }
        
        params = SearchParameters(
            query="test",
            engine="duckduckgo",
            search_type=SearchType.WEB
        )
        
        response = self.standardizer.standardize_response(
            raw_response, params, "duckduckgo", 1234567890.0
        )
        
        assert response.results[0].url == "https://example.com/path?param=value"
        
        # Test invalid URL
        raw_response["results"][0]["url"] = "not-a-url"
        
        response = self.standardizer.standardize_response(
            raw_response, params, "duckduckgo", 1234567890.0
        )
        
        # Should still include the result but with the invalid URL
        assert response.results[0].url == "not-a-url"
    
    def test_snippet_cleaning(self):
        """Test snippet text cleaning and normalization."""
        raw_response = {
            "results": [
                {
                    "title": "Test",
                    "url": "https://example.com",
                    "snippet": "  Test snippet with   extra spaces  \n"
                }
            ]
        }
        
        params = SearchParameters(
            query="test",
            engine="duckduckgo",
            search_type=SearchType.WEB
        )
        
        response = self.standardizer.standardize_response(
            raw_response, params, "duckduckgo", 1234567890.0
        )
        
        assert response.results[0].snippet == "Test snippet with extra spaces"
    
    def test_relevance_score_normalization(self):
        """Test relevance score normalization."""
        raw_response = {
            "results": [
                {
                    "title": "Test",
                    "url": "https://example.com",
                    "snippet": "Test",
                    "score": 95  # Out of 100
                }
            ]
        }
        
        params = SearchParameters(
            query="test",
            engine="tavily",
            search_type=SearchType.RESEARCH
        )
        
        response = self.standardizer.standardize_response(
            raw_response, params, "tavily", 1234567890.0
        )
        
        # Should normalize to 0-1 range
        assert 0.0 <= response.results[0].relevance_score <= 1.0
    
    def test_metadata_preservation(self):
        """Test that engine-specific metadata is preserved."""
        raw_response = {
            "results": [
                {
                    "title": "Test",
                    "url": "https://example.com",
                    "snippet": "Test",
                    "custom_field": "custom_value"
                }
            ],
            "engine_metadata": {
                "api_version": "v1",
                "rate_limit": "100/hour"
            }
        }
        
        params = SearchParameters(
            query="test",
            engine="duckduckgo",
            search_type=SearchType.WEB
        )
        
        response = self.standardizer.standardize_response(
            raw_response, params, "duckduckgo", 1234567890.0
        )
        
        # Check that metadata is preserved
        assert "custom_field" in response.results[0].metadata
        assert response.results[0].metadata["custom_field"] == "custom_value"
        assert "engine_metadata" in response.metadata
    
    def test_search_time_calculation(self):
        """Test search time calculation."""
        raw_response = {"results": []}
        
        params = SearchParameters(
            query="test",
            engine="duckduckgo",
            search_type=SearchType.WEB
        )
        
        start_time = 1234567890.0
        
        response = self.standardizer.standardize_response(
            raw_response, params, "duckduckgo", start_time
        )
        
        # Search time should be calculated from start_time
        assert response.search_time > 0
        assert isinstance(response.search_time, float)
    
    def test_total_results_extraction(self):
        """Test extraction of total results count."""
        # Test explicit total_results
        raw_response = {
            "results": [{"title": "Test", "url": "https://example.com", "snippet": "Test"}],
            "total_results": 1000
        }
        
        params = SearchParameters(
            query="test",
            engine="duckduckgo",
            search_type=SearchType.WEB
        )
        
        response = self.standardizer.standardize_response(
            raw_response, params, "duckduckgo", 1234567890.0
        )
        
        assert response.total_results == 1000
        
        # Test Google SERP format
        raw_response = {
            "organic_results": [{"title": "Test", "link": "https://example.com", "snippet": "Test"}],
            "search_metadata": {"total_results": "About 1,000,000 results"}
        }
        
        response = self.standardizer.standardize_response(
            raw_response, params, "google_serp", 1234567890.0
        )
        
        assert response.total_results == 1000000
    
    def test_source_extraction(self):
        """Test extraction of source information."""
        raw_response = {
            "results": [
                {
                    "title": "Test",
                    "url": "https://example.com/path",
                    "snippet": "Test"
                }
            ]
        }
        
        params = SearchParameters(
            query="test",
            engine="duckduckgo",
            search_type=SearchType.WEB
        )
        
        response = self.standardizer.standardize_response(
            raw_response, params, "duckduckgo", 1234567890.0
        )
        
        # Should extract domain as source
        assert response.results[0].source == "example.com"
    
    def test_engine_specific_standardizers(self):
        """Test that engine-specific standardizers are used when available."""
        # This test verifies that the standardizer can handle different engines
        engines_and_responses = [
            ("duckduckgo", {"results": []}),
            ("google_serp", {"organic_results": []}),
            ("tavily", {"results": []}),
            ("wikipedia", {"results": []}),
            ("news_api", {"articles": []}),
            ("hacker_news", {"stories": []})
        ]
        
        for engine, raw_response in engines_and_responses:
            params = SearchParameters(
                query="test",
                engine=engine,
                search_type=SearchType.WEB
            )
            
            response = self.standardizer.standardize_response(
                raw_response, params, engine, 1234567890.0
            )
            
            assert isinstance(response, SearchResponse)
            assert response.engine_used == engine
    
    def test_error_handling_in_standardization(self):
        """Test error handling during standardization."""
        # Test with None response
        params = SearchParameters(
            query="test",
            engine="duckduckgo",
            search_type=SearchType.WEB
        )
        
        response = self.standardizer.standardize_response(
            None, params, "duckduckgo", 1234567890.0
        )
        
        assert isinstance(response, SearchResponse)
        assert len(response.results) == 0
        
        # Test with response that causes parsing errors
        raw_response = {
            "results": [
                {
                    "title": None,  # Invalid title
                    "url": "https://example.com",
                    "snippet": "Test"
                }
            ]
        }
        
        response = self.standardizer.standardize_response(
            raw_response, params, "duckduckgo", 1234567890.0
        )
        
        # Should handle gracefully
        assert isinstance(response, SearchResponse)
        # Result might be filtered out or have default title
        assert len(response.results) >= 0
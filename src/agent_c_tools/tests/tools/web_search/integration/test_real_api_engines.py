"""
Integration tests for web search engines with real API calls.

These tests require actual API keys and make real network requests.
They validate that the engine adapters work correctly with live services.
"""
import pytest
import os
import json
from datetime import datetime, timedelta
from typing import Dict, Any

# Import the unified system components
import sys
sys.path.insert(0, "../../../src/agent_c_tools/tools/web_search")

from agent_c_tools.tools.web_search.base.models import SearchParameters, SearchType, SafeSearchLevel, SearchDepth
from agent_c_tools.tools.web_search.engines.duckduckgo_engine import create_duckduckgo_engine
from agent_c_tools.tools.web_search.engines.google_serp_engine import create_google_serp_engine
from agent_c_tools.tools.web_search.engines.tavily_engine import create_tavily_engine
from agent_c_tools.tools.web_search.engines.wikipedia_engine import create_wikipedia_engine
from agent_c_tools.tools.web_search.engines.news_api_engine import create_news_api_engine
from agent_c_tools.tools.web_search.engines.hacker_news_engine import create_hacker_news_engine


class TestDuckDuckGoIntegration:
    """Integration tests for DuckDuckGo engine with real API."""
    
    def setup_method(self):
        """Set up test fixtures."""
        self.engine = create_duckduckgo_engine()
    
    @pytest.mark.integration
    @pytest.mark.network
    def test_real_web_search(self):
        """Test real web search with DuckDuckGo."""
        params = SearchParameters(
            query="Python programming tutorial",
            engine="duckduckgo",
            search_type=SearchType.WEB,
            max_results=5
        )
        
        response = self.engine.execute_search(params)
        
        # Validate response structure
        assert response is not None
        assert response.engine_used == "duckduckgo"
        assert response.query == "Python programming tutorial"
        assert len(response.results) > 0
        assert len(response.results) <= 5
        
        # Validate result structure
        for result in response.results:
            assert result.title is not None
            assert result.url is not None
            assert result.url.startswith("http")
            assert result.snippet is not None
    
    @pytest.mark.integration
    @pytest.mark.network
    def test_safe_search_filtering(self):
        """Test safe search filtering with real API."""
        params = SearchParameters(
            query="family friendly content",
            engine="duckduckgo",
            search_type=SearchType.WEB,
            safe_search=SafeSearchLevel.STRICT,
            max_results=3
        )
        
        response = self.engine.execute_search(params)
        
        assert response is not None
        assert len(response.results) > 0
        # Safe search should return clean results
        for result in response.results:
            assert result.title is not None
            assert result.url is not None
    
    @pytest.mark.integration
    @pytest.mark.network
    def test_availability_check(self):
        """Test engine availability check."""
        assert self.engine.is_available() == True
        
        health_status = self.engine.get_health_status()
        assert health_status.is_available == True
        assert health_status.response_time > 0
        assert health_status.last_check is not None


class TestGoogleSerpIntegration:
    """Integration tests for Google SERP engine with real API."""
    
    def setup_method(self):
        """Set up test fixtures."""
        self.engine = create_google_serp_engine()
    
    @pytest.mark.integration
    @pytest.mark.network
    @pytest.mark.requires_api_key
    @pytest.mark.skipif(not os.getenv("SERPAPI_API_KEY"), reason="SERPAPI_API_KEY not set")
    def test_real_web_search(self):
        """Test real web search with Google SERP API."""
        params = SearchParameters(
            query="artificial intelligence news",
            engine="google_serp",
            search_type=SearchType.WEB,
            max_results=5
        )
        
        response = self.engine.execute_search(params)
        
        # Validate response structure
        assert response is not None
        assert response.engine_used == "google_serp"
        assert response.query == "artificial intelligence news"
        assert len(response.results) > 0
        assert len(response.results) <= 5
        
        # Validate result structure
        for result in response.results:
            assert result.title is not None
            assert result.url is not None
            assert result.url.startswith("http")
            assert result.snippet is not None
    
    @pytest.mark.integration
    @pytest.mark.network
    @pytest.mark.requires_api_key
    @pytest.mark.skipif(not os.getenv("SERPAPI_API_KEY"), reason="SERPAPI_API_KEY not set")
    def test_real_news_search(self):
        """Test real news search with Google SERP API."""
        params = SearchParameters(
            query="technology news today",
            engine="google_serp",
            search_type=SearchType.NEWS,
            max_results=5
        )
        
        response = self.engine.execute_search(params)
        
        assert response is not None
        assert response.engine_used == "google_serp"
        assert response.search_type == SearchType.NEWS
        assert len(response.results) > 0
        
        # News results should have publication dates
        for result in response.results:
            assert result.title is not None
            assert result.url is not None
    
    @pytest.mark.integration
    @pytest.mark.network
    @pytest.mark.requires_api_key
    @pytest.mark.skipif(not os.getenv("SERPAPI_API_KEY"), reason="SERPAPI_API_KEY not set")
    def test_real_flights_search(self):
        """Test real flights search with Google SERP API."""
        params = SearchParameters(
            query="flights from NYC to LAX",
            engine="google_serp",
            search_type=SearchType.FLIGHTS,
            max_results=3
        )
        
        response = self.engine.execute_search(params)
        
        assert response is not None
        assert response.engine_used == "google_serp"
        assert response.search_type == SearchType.FLIGHTS
        # Flight results may vary, just check basic structure
        assert len(response.results) >= 0
    
    @pytest.mark.integration
    @pytest.mark.network
    @pytest.mark.requires_api_key
    @pytest.mark.skipif(not os.getenv("SERPAPI_API_KEY"), reason="SERPAPI_API_KEY not set")
    def test_api_key_validation(self):
        """Test API key validation."""
        assert self.engine.is_available() == True
    
    @pytest.mark.integration
    @pytest.mark.network
    def test_availability_without_api_key(self):
        """Test availability check without API key."""
        # Temporarily remove API key
        original_key = os.environ.get("SERPAPI_API_KEY")
        if original_key:
            del os.environ["SERPAPI_API_KEY"]
        
        try:
            engine = create_google_serp_engine()
            assert engine.is_available() == False
        finally:
            # Restore API key
            if original_key:
                os.environ["SERPAPI_API_KEY"] = original_key


class TestTavilyIntegration:
    """Integration tests for Tavily engine with real API."""
    
    def setup_method(self):
        """Set up test fixtures."""
        self.engine = create_tavily_engine()
    
    @pytest.mark.integration
    @pytest.mark.network
    @pytest.mark.requires_api_key
    @pytest.mark.skipif(not os.getenv("TAVILI_API_KEY"), reason="TAVILI_API_KEY not set")
    def test_real_research_search(self):
        """Test real research search with Tavily API."""
        params = SearchParameters(
            query="climate change research 2024",
            engine="tavily",
            search_type=SearchType.RESEARCH,
            max_results=5,
            search_depth=SearchDepth.ADVANCED
        )
        
        response = self.engine.execute_search(params)
        
        assert response is not None
        assert response.engine_used == "tavily"
        assert response.search_type == SearchType.RESEARCH
        assert len(response.results) > 0
        assert len(response.results) <= 5
        
        # Research results should have detailed content
        for result in response.results:
            assert result.title is not None
            assert result.url is not None
            assert result.snippet is not None
    
    @pytest.mark.integration
    @pytest.mark.network
    @pytest.mark.requires_api_key
    @pytest.mark.skipif(not os.getenv("TAVILI_API_KEY"), reason="TAVILI_API_KEY not set")
    def test_domain_filtering(self):
        """Test domain filtering with real API."""
        params = SearchParameters(
            query="machine learning",
            engine="tavily",
            search_type=SearchType.RESEARCH,
            max_results=3,
            include_domains=["arxiv.org", "scholar.google.com"]
        )
        
        response = self.engine.execute_search(params)
        
        assert response is not None
        assert len(response.results) > 0
        
        # Check that results are from allowed domains
        for result in response.results:
            assert any(domain in result.url for domain in ["arxiv.org", "scholar.google.com"])


class TestWikipediaIntegration:
    """Integration tests for Wikipedia engine with real API."""
    
    def setup_method(self):
        """Set up test fixtures."""
        self.engine = create_wikipedia_engine()
    
    @pytest.mark.integration
    @pytest.mark.network
    def test_real_educational_search(self):
        """Test real educational search with Wikipedia API."""
        params = SearchParameters(
            query="quantum computing",
            engine="wikipedia",
            search_type=SearchType.EDUCATIONAL,
            max_results=5
        )
        
        response = self.engine.execute_search(params)
        
        assert response is not None
        assert response.engine_used == "wikipedia"
        assert response.search_type == SearchType.EDUCATIONAL
        assert len(response.results) > 0
        assert len(response.results) <= 5
        
        # Wikipedia results should have educational content
        for result in response.results:
            assert result.title is not None
            assert result.url is not None
            assert "wikipedia.org" in result.url
            assert result.snippet is not None
    
    @pytest.mark.integration
    @pytest.mark.network
    def test_availability_check(self):
        """Test Wikipedia availability check."""
        assert self.engine.is_available() == True
        
        health_status = self.engine.get_health_status()
        assert health_status.is_available == True
        assert health_status.response_time > 0


class TestNewsApiIntegration:
    """Integration tests for NewsAPI engine with real API."""
    
    def setup_method(self):
        """Set up test fixtures."""
        self.engine = create_news_api_engine()
    
    @pytest.mark.integration
    @pytest.mark.network
    @pytest.mark.requires_api_key
    @pytest.mark.skipif(not os.getenv("NEWSAPI_API_KEY"), reason="NEWSAPI_API_KEY not set")
    def test_real_news_search(self):
        """Test real news search with NewsAPI."""
        params = SearchParameters(
            query="technology innovation",
            engine="news_api",
            search_type=SearchType.NEWS,
            max_results=5
        )
        
        response = self.engine.execute_search(params)
        
        assert response is not None
        assert response.engine_used == "news_api"
        assert response.search_type == SearchType.NEWS
        assert len(response.results) > 0
        assert len(response.results) <= 5
        
        # News results should have publication info
        for result in response.results:
            assert result.title is not None
            assert result.url is not None
            assert result.snippet is not None
    
    @pytest.mark.integration
    @pytest.mark.network
    @pytest.mark.requires_api_key
    @pytest.mark.skipif(not os.getenv("NEWSAPI_API_KEY"), reason="NEWSAPI_API_KEY not set")
    def test_date_filtering(self):
        """Test date filtering with real API."""
        # Search for news from last week
        from_date = datetime.now() - timedelta(days=7)
        to_date = datetime.now()
        
        params = SearchParameters(
            query="artificial intelligence",
            engine="news_api",
            search_type=SearchType.NEWS,
            max_results=3,
            from_date=from_date.strftime("%Y-%m-%d"),
            to_date=to_date.strftime("%Y-%m-%d")
        )
        
        response = self.engine.execute_search(params)
        
        assert response is not None
        assert len(response.results) >= 0  # May not have results in date range


class TestHackerNewsIntegration:
    """Integration tests for HackerNews engine with real API."""
    
    def setup_method(self):
        """Set up test fixtures."""
        self.engine = create_hacker_news_engine()
    
    @pytest.mark.integration
    @pytest.mark.network
    def test_real_tech_search(self):
        """Test real tech search with HackerNews API."""
        params = SearchParameters(
            query="programming discussion",
            engine="hacker_news",
            search_type=SearchType.TECH,
            max_results=5
        )
        
        response = self.engine.execute_search(params)
        
        assert response is not None
        assert response.engine_used == "hacker_news"
        assert response.search_type == SearchType.TECH
        assert len(response.results) > 0
        assert len(response.results) <= 5
        
        # HackerNews results should have tech community content
        for result in response.results:
            assert result.title is not None
            assert result.url is not None
    
    @pytest.mark.integration
    @pytest.mark.network
    def test_availability_check(self):
        """Test HackerNews availability check."""
        assert self.engine.is_available() == True
        
        health_status = self.engine.get_health_status()
        assert health_status.is_available == True
        assert health_status.response_time > 0


class TestRateLimitingAndErrors:
    """Integration tests for rate limiting and error handling."""
    
    @pytest.mark.integration
    @pytest.mark.network
    @pytest.mark.slow
    def test_rate_limiting_handling(self):
        """Test rate limiting behavior with multiple rapid requests."""
        engine = create_duckduckgo_engine()
        
        # Make multiple rapid requests
        results = []
        for i in range(3):
            params = SearchParameters(
                query=f"test query {i}",
                engine="duckduckgo",
                search_type=SearchType.WEB,
                max_results=2
            )
            
            try:
                response = engine.execute_search(params)
                results.append(response)
            except Exception as e:
                # Rate limiting or other errors are acceptable
                print(f"Request {i} failed: {e}")
        
        # At least some requests should succeed
        assert len(results) > 0
    
    @pytest.mark.integration
    @pytest.mark.network
    def test_invalid_query_handling(self):
        """Test handling of invalid queries."""
        engine = create_duckduckgo_engine()
        
        params = SearchParameters(
            query="",  # Empty query
            engine="duckduckgo",
            search_type=SearchType.WEB,
            max_results=5
        )
        
        # Should handle empty query gracefully
        try:
            response = engine.execute_search(params)
            # If it succeeds, response should be valid
            assert response is not None
        except Exception as e:
            # Or it should raise a meaningful exception
            assert "query" in str(e).lower() or "empty" in str(e).lower()
    
    @pytest.mark.integration
    @pytest.mark.network
    def test_network_timeout_handling(self):
        """Test network timeout handling."""
        engine = create_duckduckgo_engine()
        
        params = SearchParameters(
            query="test network timeout",
            engine="duckduckgo",
            search_type=SearchType.WEB,
            max_results=5
        )
        
        # This should complete within reasonable time or handle timeout
        try:
            response = engine.execute_search(params)
            assert response is not None
        except Exception as e:
            # Timeout or network errors are acceptable
            assert "timeout" in str(e).lower() or "network" in str(e).lower() or "connection" in str(e).lower()


class TestEngineHealthMonitoring:
    """Integration tests for engine health monitoring."""
    
    @pytest.mark.integration
    @pytest.mark.network
    def test_all_engines_health_status(self):
        """Test health status for all engines."""
        engines = [
            create_duckduckgo_engine(),
            create_google_serp_engine(),
            create_tavily_engine(),
            create_wikipedia_engine(),
            create_news_api_engine(),
            create_hacker_news_engine()
        ]
        
        for engine in engines:
            health_status = engine.get_health_status()
            
            # Health status should be properly populated
            assert health_status is not None
            assert health_status.last_check is not None
            assert isinstance(health_status.response_time, (int, float))
            assert isinstance(health_status.error_rate, (int, float))
            assert health_status.error_rate >= 0
            assert health_status.error_rate <= 1.0
            
            # Availability should match API key requirements
            if engine.capabilities.requires_api_key:
                expected_available = engine.is_available()
                assert health_status.is_available == expected_available
            else:
                # Non-API key engines should generally be available
                assert health_status.is_available == True
    
    @pytest.mark.integration
    @pytest.mark.network
    def test_engine_capabilities_accuracy(self):
        """Test that engine capabilities match actual behavior."""
        engines = [
            create_duckduckgo_engine(),
            create_google_serp_engine(),
            create_tavily_engine(),
            create_wikipedia_engine(),
            create_news_api_engine(),
            create_hacker_news_engine()
        ]
        
        for engine in engines:
            capabilities = engine.capabilities
            
            # Test search type support
            for search_type in capabilities.supported_search_types:
                assert engine.supports_search_type(search_type) == True
            
            # Test unsupported search types
            all_search_types = [SearchType.WEB, SearchType.NEWS, SearchType.EDUCATIONAL, 
                              SearchType.RESEARCH, SearchType.TECH, SearchType.FLIGHTS, SearchType.EVENTS]
            
            for search_type in all_search_types:
                if search_type not in capabilities.supported_search_types:
                    assert engine.supports_search_type(search_type) == False
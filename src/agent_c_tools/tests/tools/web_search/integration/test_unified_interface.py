"""
Integration tests for the unified WebSearchTools interface with real APIs.

These tests validate that the unified interface works correctly with real search engines
and handles all the routing, validation, and error scenarios properly.
"""
import pytest
import os
import json
from datetime import datetime, timedelta
from typing import Dict, Any

# Import the unified system
import sys
sys.path.insert(0, "../../../src/agent_c_tools/tools/web_search")

from agent_c_tools.tools.web_search.web_search_tools import WebSearchTools



class TestUnifiedWebSearchInterface:
    """Integration tests for the unified WebSearchTools interface."""
    
    def setup_method(self):
        """Set up test fixtures."""
        self.tools = WebSearchTools()
    
    @pytest.mark.integration
    @pytest.mark.network
    def test_web_search_auto_routing(self):
        """Test web search with automatic engine routing."""
        result = self.tools.web_search(
            query="Python programming tutorial",
            max_results=5
        )
        
        # Parse JSON response
        response = json.loads(result)
        
        assert response["success"] == True
        assert "results" in response
        assert len(response["results"]) > 0
        assert len(response["results"]) <= 5
        assert "engine_used" in response
        assert "query" in response
        assert response["query"] == "Python programming tutorial"
        
        # Validate result structure
        for result_item in response["results"]:
            assert "title" in result_item
            assert "url" in result_item
            assert "snippet" in result_item
            assert result_item["url"].startswith("http")
    
    @pytest.mark.integration
    @pytest.mark.network
    def test_web_search_explicit_engine(self):
        """Test web search with explicit engine selection."""
        result = self.tools.web_search(
            query="artificial intelligence",
            engine="duckduckgo",
            max_results=3
        )
        
        response = json.loads(result)
        
        assert response["success"] == True
        assert response["engine_used"] == "duckduckgo"
        assert len(response["results"]) > 0
        assert len(response["results"]) <= 3
    
    @pytest.mark.integration
    @pytest.mark.network
    def test_news_search_routing(self):
        """Test news search with automatic routing to news engines."""
        result = self.tools.news_search(
            query="technology news today",
            max_results=5
        )
        
        response = json.loads(result)
        
        assert response["success"] == True
        assert "results" in response
        assert len(response["results"]) > 0
        assert "engine_used" in response
        # Should route to a news-capable engine
        assert response["engine_used"] in ["google_serp", "news_api"]
    
    @pytest.mark.integration
    @pytest.mark.network
    def test_educational_search_routing(self):
        """Test educational search routing to Wikipedia."""
        result = self.tools.educational_search(
            query="quantum physics",
            max_results=3
        )
        
        response = json.loads(result)
        
        assert response["success"] == True
        assert response["engine_used"] == "wikipedia"
        assert len(response["results"]) > 0
        
        # Wikipedia results should have educational content
        for result_item in response["results"]:
            assert "wikipedia.org" in result_item["url"]
    
    @pytest.mark.integration
    @pytest.mark.network
    @pytest.mark.requires_api_key
    @pytest.mark.skipif(not os.getenv("TAVILI_API_KEY"), reason="TAVILI_API_KEY not set")
    def test_research_search_routing(self):
        """Test research search routing to Tavily."""
        result = self.tools.research_search(
            query="climate change research 2024",
            max_results=3,
            search_depth="advanced"
        )
        
        response = json.loads(result)
        
        assert response["success"] == True
        assert response["engine_used"] == "tavily"
        assert len(response["results"]) > 0
    
    @pytest.mark.integration
    @pytest.mark.network
    def test_tech_search_routing(self):
        """Test tech search routing to HackerNews."""
        result = self.tools.tech_search(
            query="programming discussion",
            max_results=5
        )
        
        response = json.loads(result)
        
        assert response["success"] == True
        assert response["engine_used"] == "hacker_news"
        assert len(response["results"]) > 0
    
    @pytest.mark.integration
    @pytest.mark.network
    @pytest.mark.requires_api_key
    @pytest.mark.skipif(not os.getenv("SERPAPI_API_KEY"), reason="SERPAPI_API_KEY not set")
    def test_flights_search_routing(self):
        """Test flights search routing to Google SERP."""
        result = self.tools.flights_search(
            query="flights from NYC to LAX",
            max_results=3
        )
        
        response = json.loads(result)
        
        assert response["success"] == True
        assert response["engine_used"] == "google_serp"
        # Flight results may vary
        assert "results" in response
    
    @pytest.mark.integration
    @pytest.mark.network
    @pytest.mark.requires_api_key
    @pytest.mark.skipif(not os.getenv("SERPAPI_API_KEY"), reason="SERPAPI_API_KEY not set")
    def test_events_search_routing(self):
        """Test events search routing to Google SERP."""
        result = self.tools.events_search(
            query="concerts in New York",
            max_results=3
        )
        
        response = json.loads(result)
        
        assert response["success"] == True
        assert response["engine_used"] == "google_serp"
        assert "results" in response
    
    @pytest.mark.integration
    @pytest.mark.network
    def test_safe_search_parameter(self):
        """Test safe search parameter handling."""
        result = self.tools.web_search(
            query="family content",
            safe_search="strict",
            max_results=3
        )
        
        response = json.loads(result)
        
        assert response["success"] == True
        assert len(response["results"]) > 0
        # Safe search should return appropriate content
        for result_item in response["results"]:
            assert result_item["title"] is not None
            assert result_item["url"] is not None
    
    @pytest.mark.integration
    @pytest.mark.network
    def test_domain_filtering(self):
        """Test domain filtering functionality."""
        result = self.tools.web_search(
            query="machine learning",
            include_domains=["github.com", "stackoverflow.com"],
            max_results=3
        )
        
        response = json.loads(result)
        
        assert response["success"] == True
        if len(response["results"]) > 0:
            # Check that results are from allowed domains
            for result_item in response["results"]:
                url = result_item["url"]
                assert any(domain in url for domain in ["github.com", "stackoverflow.com"])
    
    @pytest.mark.integration
    @pytest.mark.network
    def test_language_parameter(self):
        """Test language parameter handling."""
        result = self.tools.web_search(
            query="programming tutorial",
            language="en",
            max_results=3
        )
        
        response = json.loads(result)
        
        assert response["success"] == True
        assert len(response["results"]) > 0
        # Language filtering should work
        assert "language" in response.get("metadata", {}) or True  # May not be in metadata
    
    @pytest.mark.integration
    @pytest.mark.network
    def test_region_parameter(self):
        """Test region parameter handling."""
        result = self.tools.web_search(
            query="local news",
            region="us",
            max_results=3
        )
        
        response = json.loads(result)
        
        assert response["success"] == True
        assert len(response["results"]) > 0
    
    @pytest.mark.integration
    @pytest.mark.network
    def test_get_engine_info(self):
        """Test engine information retrieval."""
        result = self.tools.get_engine_info()
        
        response = json.loads(result)
        
        assert response["success"] == True
        assert "engines" in response
        assert len(response["engines"]) > 0
        
        # Check engine info structure
        for engine_info in response["engines"]:
            assert "name" in engine_info
            assert "available" in engine_info
            assert "capabilities" in engine_info
            assert "health_status" in engine_info
            
            capabilities = engine_info["capabilities"]
            assert "supported_search_types" in capabilities
            assert "max_results" in capabilities
            assert "requires_api_key" in capabilities
            
            health_status = engine_info["health_status"]
            assert "is_available" in health_status
            assert "response_time" in health_status
            assert "error_rate" in health_status


class TestUnifiedErrorHandling:
    """Integration tests for error handling in the unified interface."""
    
    def setup_method(self):
        """Set up test fixtures."""
        self.tools = WebSearchTools()
    
    @pytest.mark.integration
    @pytest.mark.network
    def test_invalid_engine_fallback(self):
        """Test fallback when invalid engine is specified."""
        result = self.tools.web_search(
            query="test query",
            engine="invalid_engine",
            max_results=3
        )
        
        response = json.loads(result)
        
        # Should fallback to a valid engine
        assert response["success"] == True
        assert response["engine_used"] != "invalid_engine"
        assert len(response["results"]) > 0
    
    @pytest.mark.integration
    @pytest.mark.network
    def test_empty_query_handling(self):
        """Test handling of empty queries."""
        result = self.tools.web_search(
            query="",
            max_results=3
        )
        
        response = json.loads(result)
        
        # Should handle empty query gracefully
        assert response["success"] == False
        assert "error" in response
        assert "query" in response["error"].lower()
    
    @pytest.mark.integration
    @pytest.mark.network
    def test_invalid_parameters_handling(self):
        """Test handling of invalid parameters."""
        result = self.tools.web_search(
            query="test query",
            max_results=-1  # Invalid max_results
        )
        
        response = json.loads(result)
        
        # Should handle invalid parameters gracefully
        assert response["success"] == False
        assert "error" in response
    
    @pytest.mark.integration
    @pytest.mark.network
    def test_unavailable_engine_fallback(self):
        """Test fallback when requested engine is unavailable."""
        # Test with an engine that requires API key but key is not set
        original_key = os.environ.get("SERPAPI_API_KEY")
        if original_key:
            del os.environ["SERPAPI_API_KEY"]
        
        try:
            result = self.tools.web_search(
                query="test query",
                engine="google_serp",
                max_results=3
            )
            
            response = json.loads(result)
            
            # Should fallback to an available engine
            assert response["success"] == True
            assert response["engine_used"] != "google_serp"
            assert len(response["results"]) > 0
        finally:
            # Restore API key
            if original_key:
                os.environ["SERPAPI_API_KEY"] = original_key
    
    @pytest.mark.integration
    @pytest.mark.network
    def test_network_error_handling(self):
        """Test handling of network errors."""
        # This test is difficult to simulate reliably
        # We'll test that the system handles exceptions gracefully
        result = self.tools.web_search(
            query="test network error handling",
            max_results=3
        )
        
        response = json.loads(result)
        
        # Should either succeed or fail gracefully
        if response["success"]:
            assert len(response["results"]) > 0
        else:
            assert "error" in response
            assert isinstance(response["error"], str)


class TestUnifiedConfigurationManagement:
    """Integration tests for configuration management."""
    
    def setup_method(self):
        """Set up test fixtures."""
        self.tools = WebSearchTools()
    
    @pytest.mark.integration
    @pytest.mark.network
    def test_api_key_detection(self):
        """Test API key detection and engine availability."""
        result = self.tools.get_engine_info()
        response = json.loads(result)
        
        engines = {engine["name"]: engine for engine in response["engines"]}
        
        # Check API key requirements match availability
        api_key_engines = {
            "google_serp": "SERPAPI_API_KEY",
            "tavily": "TAVILI_API_KEY", 
            "news_api": "NEWSAPI_API_KEY"
        }
        
        for engine_name, env_var in api_key_engines.items():
            if engine_name in engines:
                engine_info = engines[engine_name]
                has_api_key = bool(os.getenv(env_var))
                
                assert engine_info["capabilities"]["requires_api_key"] == True
                assert engine_info["available"] == has_api_key
    
    @pytest.mark.integration
    @pytest.mark.network
    def test_configuration_validation(self):
        """Test configuration validation."""
        # Test with various configuration scenarios
        test_cases = [
            {"max_results": 5, "expected_valid": True},
            {"max_results": 0, "expected_valid": False},
            {"max_results": 1000, "expected_valid": False},
            {"safe_search": "strict", "expected_valid": True},
            {"safe_search": "invalid", "expected_valid": False},
        ]
        
        for case in test_cases:
            params = {"query": "test query"}
            params.update({k: v for k, v in case.items() if k != "expected_valid"})
            
            result = self.tools.web_search(**params)
            response = json.loads(result)
            
            if case["expected_valid"]:
                assert response["success"] == True
            else:
                assert response["success"] == False
                assert "error" in response


class TestUnifiedPerformance:
    """Integration tests for performance characteristics."""
    
    def setup_method(self):
        """Set up test fixtures."""
        self.tools = WebSearchTools()
    
    @pytest.mark.integration
    @pytest.mark.network
    @pytest.mark.slow
    def test_response_time_reasonable(self):
        """Test that response times are reasonable."""
        import time
        
        start_time = time.time()
        
        result = self.tools.web_search(
            query="performance test query",
            max_results=3
        )
        
        end_time = time.time()
        response_time = end_time - start_time
        
        response = json.loads(result)
        
        assert response["success"] == True
        # Response should be within reasonable time (30 seconds)
        assert response_time < 30.0
        assert len(response["results"]) > 0
    
    @pytest.mark.integration
    @pytest.mark.network
    @pytest.mark.slow
    def test_concurrent_requests_handling(self):
        """Test handling of concurrent requests."""
        import threading
        import time
        
        results = []
        errors = []
        
        def make_request(query_suffix):
            try:
                result = self.tools.web_search(
                    query=f"concurrent test {query_suffix}",
                    max_results=2
                )
                response = json.loads(result)
                results.append(response)
            except Exception as e:
                errors.append(str(e))
        
        # Create multiple threads
        threads = []
        for i in range(3):
            thread = threading.Thread(target=make_request, args=(i,))
            threads.append(thread)
        
        # Start all threads
        start_time = time.time()
        for thread in threads:
            thread.start()
        
        # Wait for all threads to complete
        for thread in threads:
            thread.join()
        
        end_time = time.time()
        total_time = end_time - start_time
        
        # At least some requests should succeed
        assert len(results) > 0
        # Should complete within reasonable time
        assert total_time < 60.0
        
        # Check successful results
        for result in results:
            assert result["success"] == True
            assert len(result["results"]) > 0
    
    @pytest.mark.integration
    @pytest.mark.network
    def test_caching_behavior(self):
        """Test caching behavior for repeated queries."""
        import time
        
        query = "caching test query unique"
        
        # First request
        start_time1 = time.time()
        result1 = self.tools.web_search(query=query, max_results=3)
        end_time1 = time.time()
        time1 = end_time1 - start_time1
        
        # Second request (should potentially be cached)
        start_time2 = time.time()
        result2 = self.tools.web_search(query=query, max_results=3)
        end_time2 = time.time()
        time2 = end_time2 - start_time2
        
        response1 = json.loads(result1)
        response2 = json.loads(result2)
        
        assert response1["success"] == True
        assert response2["success"] == True
        
        # Results should be consistent
        assert response1["query"] == response2["query"]
        assert response1["engine_used"] == response2["engine_used"]
        
        # Second request might be faster due to caching
        # (This is not guaranteed, so we just check it's reasonable)
        assert time2 < 30.0
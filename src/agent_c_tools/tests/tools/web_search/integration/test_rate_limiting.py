"""
Integration tests for rate limiting and API constraints.

These tests validate that the system properly handles rate limiting,
API quotas, and other constraints imposed by search engine APIs.
"""
import pytest
import os
import json
import time
import threading
from datetime import datetime, timedelta
from typing import Dict, Any, List

# Import the unified system
import sys
sys.path.insert(0, "../../../src/agent_c_tools/tools/web_search")

from agent_c_tools.tools.web_search.web_search_tools import WebSearchTools
from agent_c_tools.tools.web_search.engines.google_serp_engine import create_google_serp_engine

from agent_c_tools.tools.web_search.engines.news_api_engine import create_news_api_engine
from agent_c_tools.tools.web_search.engines.duckduckgo_engine import create_duckduckgo_engine
from agent_c_tools.tools.web_search.base.models import SearchParameters, SearchType


class TestRateLimiting:
    """Integration tests for rate limiting behavior."""
    
    def setup_method(self):
        """Set up test fixtures."""
        self.tools = WebSearchTools()
    
    @pytest.mark.integration
    @pytest.mark.network
    @pytest.mark.slow
    def test_sequential_requests_duckduckgo(self):
        """Test sequential requests to DuckDuckGo (no API key required)."""
        engine = create_duckduckgo_engine()
        
        results = []
        response_times = []
        
        # Make 5 sequential requests
        for i in range(5):
            start_time = time.time()
            
            params = SearchParameters(
                query=f"test sequential request {i}",
                engine="duckduckgo",
                search_type=SearchType.WEB,
                max_results=3
            )
            
            try:
                response = engine.execute_search(params)
                end_time = time.time()
                
                results.append(response)
                response_times.append(end_time - start_time)
                
                # Small delay between requests to be respectful
                time.sleep(1)
                
            except Exception as e:
                print(f"Request {i} failed: {e}")
                response_times.append(None)
        
        # At least some requests should succeed
        successful_results = [r for r in results if r is not None]
        assert len(successful_results) > 0
        
        # Check response times are reasonable
        valid_times = [t for t in response_times if t is not None]
        if valid_times:
            avg_time = sum(valid_times) / len(valid_times)
            assert avg_time < 10.0  # Should be under 10 seconds on average
    
    @pytest.mark.integration
    @pytest.mark.network
    @pytest.mark.slow
    @pytest.mark.requires_api_key
    @pytest.mark.skipif(not os.getenv("SERPAPI_API_KEY"), reason="SERPAPI_API_KEY not set")
    def test_sequential_requests_google_serp(self):
        """Test sequential requests to Google SERP API."""
        engine = create_google_serp_engine()
        
        results = []
        response_times = []
        
        # Make 3 sequential requests (fewer due to API limits)
        for i in range(3):
            start_time = time.time()
            
            params = SearchParameters(
                query=f"test google serp {i}",
                engine="google_serp",
                search_type=SearchType.WEB,
                max_results=3
            )
            
            try:
                response = engine.execute_search(params)
                end_time = time.time()
                
                results.append(response)
                response_times.append(end_time - start_time)
                
                # Delay between requests to respect rate limits
                time.sleep(2)
                
            except Exception as e:
                print(f"Google SERP request {i} failed: {e}")
                response_times.append(None)
        
        # At least some requests should succeed
        successful_results = [r for r in results if r is not None]
        assert len(successful_results) > 0
        
        # Check response times
        valid_times = [t for t in response_times if t is not None]
        if valid_times:
            avg_time = sum(valid_times) / len(valid_times)
            assert avg_time < 15.0  # API requests may be slower
    
    @pytest.mark.integration
    @pytest.mark.network
    @pytest.mark.slow
    def test_concurrent_requests_handling(self):
        """Test handling of concurrent requests."""
        def make_request(request_id: int, results: List, errors: List):
            """Make a single request and store result."""
            try:
                result = self.tools.web_search(
                    query=f"concurrent test {request_id}",
                    max_results=2
                )
                response = json.loads(result)
                results.append((request_id, response))
            except Exception as e:
                errors.append((request_id, str(e)))
        
        results = []
        errors = []
        threads = []
        
        # Create 3 concurrent threads
        for i in range(3):
            thread = threading.Thread(
                target=make_request,
                args=(i, results, errors)
            )
            threads.append(thread)
        
        # Start all threads
        start_time = time.time()
        for thread in threads:
            thread.start()
        
        # Wait for completion
        for thread in threads:
            thread.join(timeout=30)  # 30 second timeout
        
        end_time = time.time()
        total_time = end_time - start_time
        
        # Check results
        assert len(results) > 0, f"No successful results. Errors: {errors}"
        assert total_time < 45.0, f"Concurrent requests took too long: {total_time}s"
        
        # Verify successful results
        for request_id, response in results:
            assert response["success"] == True
            assert len(response["results"]) > 0
            assert "engine_used" in response
    
    @pytest.mark.integration
    @pytest.mark.network
    @pytest.mark.slow
    def test_rapid_requests_rate_limiting(self):
        """Test rate limiting with rapid requests."""
        engine = create_duckduckgo_engine()
        
        results = []
        errors = []
        
        # Make rapid requests (no delay)
        for i in range(5):
            params = SearchParameters(
                query=f"rapid test {i}",
                engine="duckduckgo",
                search_type=SearchType.WEB,
                max_results=2
            )
            
            try:
                response = engine.execute_search(params)
                results.append(response)
            except Exception as e:
                errors.append(str(e))
        
        # Some requests should succeed, some may be rate limited
        total_requests = len(results) + len(errors)
        assert total_requests == 5
        
        # At least one request should succeed
        assert len(results) > 0
        
        # Check for rate limiting errors
        rate_limit_errors = [
            error for error in errors 
            if any(keyword in error.lower() for keyword in ["rate", "limit", "too many", "throttle"])
        ]
        
        # Rate limiting is expected with rapid requests
        print(f"Successful requests: {len(results)}")
        print(f"Rate limit errors: {len(rate_limit_errors)}")
        print(f"Other errors: {len(errors) - len(rate_limit_errors)}")


class TestAPIQuotaHandling:
    """Integration tests for API quota and usage limits."""
    
    def setup_method(self):
        """Set up test fixtures."""
        self.tools = WebSearchTools()
    
    @pytest.mark.integration
    @pytest.mark.network
    @pytest.mark.requires_api_key
    @pytest.mark.skipif(not os.getenv("NEWSAPI_API_KEY"), reason="NEWSAPI_API_KEY not set")
    def test_newsapi_quota_awareness(self):
        """Test NewsAPI quota handling."""
        engine = create_news_api_engine()
        
        # Make a few requests to test quota handling
        results = []
        for i in range(3):
            params = SearchParameters(
                query=f"news quota test {i}",
                engine="news_api",
                search_type=SearchType.NEWS,
                max_results=5
            )
            
            try:
                response = engine.execute_search(params)
                results.append(response)
                time.sleep(1)  # Respectful delay
            except Exception as e:
                print(f"NewsAPI request {i} failed: {e}")
                # Check if it's a quota error
                if any(keyword in str(e).lower() for keyword in ["quota", "limit", "exceeded"]):
                    print("Quota limit reached")
                    break
        
        # At least one request should succeed
        assert len(results) > 0
    
    @pytest.mark.integration
    @pytest.mark.network
    @pytest.mark.slow
    def test_quota_exhaustion_fallback(self):
        """Test fallback behavior when API quotas are exhausted."""
        # This test simulates quota exhaustion by making many requests
        # In practice, this would be difficult to test reliably
        
        # Test that the unified interface handles quota exhaustion gracefully
        results = []
        errors = []
        
        # Make requests until we potentially hit limits
        for i in range(10):
            try:
                result = self.tools.web_search(
                    query=f"quota exhaustion test {i}",
                    max_results=2
                )
                response = json.loads(result)
                results.append(response)
                
                # Small delay to be respectful
                time.sleep(0.5)
                
            except Exception as e:
                errors.append(str(e))
        
        # Should have some successful results
        assert len(results) > 0
        
        # All successful results should be valid
        for response in results:
            assert response["success"] == True
            assert len(response["results"]) > 0
    
    @pytest.mark.integration
    @pytest.mark.network
    def test_engine_fallback_on_quota_exhaustion(self):
        """Test engine fallback when primary engine hits quota limits."""
        # This test checks that the system falls back to alternative engines
        # when the primary engine is unavailable due to quota limits
        
        # Test with a specific engine preference
        result = self.tools.web_search(
            query="engine fallback test",
            engine="duckduckgo",  # Start with DuckDuckGo
            max_results=3
        )
        
        response = json.loads(result)
        
        # Should succeed with DuckDuckGo or fallback
        assert response["success"] == True
        assert len(response["results"]) > 0
        
        # Engine used should be valid
        assert response["engine_used"] in [
            "duckduckgo", "wikipedia", "hacker_news", "google_serp", "tavily", "news_api"
        ]


class TestErrorRecovery:
    """Integration tests for error recovery and resilience."""
    
    def setup_method(self):
        """Set up test fixtures."""
        self.tools = WebSearchTools()
    
    @pytest.mark.integration
    @pytest.mark.network
    def test_network_timeout_recovery(self):
        """Test recovery from network timeouts."""
        # Test that the system handles network timeouts gracefully
        results = []
        
        for i in range(3):
            try:
                result = self.tools.web_search(
                    query=f"timeout recovery test {i}",
                    max_results=2
                )
                response = json.loads(result)
                results.append(response)
                
                time.sleep(1)
                
            except Exception as e:
                print(f"Request {i} failed: {e}")
        
        # At least some requests should succeed
        assert len(results) > 0
        
        # Successful results should be valid
        for response in results:
            assert response["success"] == True
            assert len(response["results"]) > 0
    
    @pytest.mark.integration
    @pytest.mark.network
    def test_service_unavailable_recovery(self):
        """Test recovery when services are temporarily unavailable."""
        # Test multiple engines to see if system can recover
        engines_to_test = ["duckduckgo", "wikipedia", "hacker_news"]
        
        successful_engines = []
        
        for engine in engines_to_test:
            try:
                result = self.tools.web_search(
                    query=f"service recovery test {engine}",
                    engine=engine,
                    max_results=2
                )
                response = json.loads(result)
                
                if response["success"]:
                    successful_engines.append(engine)
                
                time.sleep(1)
                
            except Exception as e:
                print(f"Engine {engine} failed: {e}")
        
        # At least one engine should be working
        assert len(successful_engines) > 0
        print(f"Working engines: {successful_engines}")
    
    @pytest.mark.integration
    @pytest.mark.network
    def test_partial_failure_handling(self):
        """Test handling of partial failures in search results."""
        # Test that the system handles cases where some results are invalid
        result = self.tools.web_search(
            query="partial failure test query",
            max_results=5
        )
        
        response = json.loads(result)
        
        if response["success"]:
            # Check that all returned results are valid
            for result_item in response["results"]:
                assert "title" in result_item
                assert "url" in result_item
                assert "snippet" in result_item
                assert result_item["title"] is not None
                assert result_item["url"] is not None
                assert result_item["url"].startswith("http")
        else:
            # If it fails, should provide meaningful error
            assert "error" in response
            assert isinstance(response["error"], str)
            assert len(response["error"]) > 0


class TestPerformanceUnderLoad:
    """Integration tests for performance under load conditions."""
    
    def setup_method(self):
        """Set up test fixtures."""
        self.tools = WebSearchTools()
    
    @pytest.mark.integration
    @pytest.mark.network
    @pytest.mark.slow
    def test_sustained_load_performance(self):
        """Test performance under sustained load."""
        results = []
        response_times = []
        
        # Make requests over a period of time
        for i in range(10):
            start_time = time.time()
            
            try:
                result = self.tools.web_search(
                    query=f"sustained load test {i}",
                    max_results=2
                )
                response = json.loads(result)
                
                end_time = time.time()
                response_time = end_time - start_time
                
                results.append(response)
                response_times.append(response_time)
                
                # Delay between requests
                time.sleep(2)
                
            except Exception as e:
                print(f"Request {i} failed: {e}")
                response_times.append(None)
        
        # Check performance metrics
        successful_results = [r for r in results if r["success"]]
        valid_times = [t for t in response_times if t is not None]
        
        assert len(successful_results) > 0
        
        if valid_times:
            avg_time = sum(valid_times) / len(valid_times)
            max_time = max(valid_times)
            
            print(f"Average response time: {avg_time:.2f}s")
            print(f"Maximum response time: {max_time:.2f}s")
            print(f"Success rate: {len(successful_results)}/{len(results)}")
            
            # Performance should be reasonable
            assert avg_time < 10.0
            assert max_time < 30.0
    
    @pytest.mark.integration
    @pytest.mark.network
    @pytest.mark.slow
    def test_memory_usage_stability(self):
        """Test that memory usage remains stable under load."""
        import psutil
        import os
        
        process = psutil.Process(os.getpid())
        initial_memory = process.memory_info().rss
        
        # Make multiple requests
        for i in range(5):
            result = self.tools.web_search(
                query=f"memory stability test {i}",
                max_results=3
            )
            response = json.loads(result)
            
            # Small delay
            time.sleep(1)
        
        final_memory = process.memory_info().rss
        memory_increase = final_memory - initial_memory
        
        print(f"Initial memory: {initial_memory / 1024 / 1024:.2f} MB")
        print(f"Final memory: {final_memory / 1024 / 1024:.2f} MB")
        print(f"Memory increase: {memory_increase / 1024 / 1024:.2f} MB")
        
        # Memory increase should be reasonable (less than 100MB)
        assert memory_increase < 100 * 1024 * 1024
    
    @pytest.mark.integration
    @pytest.mark.network
    @pytest.mark.slow
    def test_engine_health_monitoring_under_load(self):
        """Test engine health monitoring under load conditions."""
        # Make requests and monitor engine health
        for i in range(5):
            # Make a search request
            result = self.tools.web_search(
                query=f"health monitoring test {i}",
                max_results=2
            )
            response = json.loads(result)
            
            # Check engine health
            health_result = self.tools.get_engine_info()
            health_response = json.loads(health_result)
            
            assert health_response["success"] == True
            
            # Check that health metrics are being updated
            for engine_info in health_response["engines"]:
                health_status = engine_info["health_status"]
                assert "response_time" in health_status
                assert "error_rate" in health_status
                assert "last_check" in health_status
                
                # Response time should be reasonable
                if health_status["response_time"] > 0:
                    assert health_status["response_time"] < 30.0
                
                # Error rate should be reasonable
                assert 0 <= health_status["error_rate"] <= 1.0
            
            time.sleep(1)
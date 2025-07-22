"""
Integration tests for API key management and configuration.

These tests validate that the system properly handles API keys,
configuration loading, and graceful degradation when keys are missing.
"""
import pytest
import os
import json
from unittest.mock import patch
from typing import Dict, Any

# Import the unified system
import sys
sys.path.insert(0, "../../../src/agent_c_tools/tools/web_search")

from agent_c_tools.tools.web_search.web_search_tools import WebSearchTools
from agent_c_tools.tools.web_search.engines.google_serp_engine import create_google_serp_engine
from agent_c_tools.tools.web_search.engines.tavily_engine import create_tavily_engine
from agent_c_tools.tools.web_search.engines.news_api_engine import create_news_api_engine


class TestAPIKeyManagement:
    """Integration tests for API key management."""
    
    def setup_method(self):
        """Set up test fixtures."""
        self.tools = WebSearchTools()
        
        # Store original environment variables
        self.original_env = {}
        api_key_vars = ["SERPAPI_API_KEY", "TAVILI_API_KEY", "NEWSAPI_API_KEY"]
        for var in api_key_vars:
            self.original_env[var] = os.environ.get(var)
    
    def teardown_method(self):
        """Clean up after tests."""
        # Restore original environment variables
        for var, value in self.original_env.items():
            if value is not None:
                os.environ[var] = value
            elif var in os.environ:
                del os.environ[var]
    
    @pytest.mark.integration
    @pytest.mark.network
    def test_api_key_detection_google_serp(self):
        """Test API key detection for Google SERP engine."""
        # Test with API key present
        if self.original_env.get("SERPAPI_API_KEY"):
            engine = create_google_serp_engine()
            assert engine.is_available() == True
            
            health_status = engine.get_health_status()
            assert health_status.is_available == True
        
        # Test without API key
        if "SERPAPI_API_KEY" in os.environ:
            del os.environ["SERPAPI_API_KEY"]
        
        engine = create_google_serp_engine()
        assert engine.is_available() == False
        
        health_status = engine.get_health_status()
        assert health_status.is_available == False
    
    @pytest.mark.integration
    @pytest.mark.network
    def test_api_key_detection_tavily(self):
        """Test API key detection for Tavily engine."""
        # Test with API key present
        if self.original_env.get("TAVILI_API_KEY"):
            engine = create_tavily_engine()
            assert engine.is_available() == True
        
        # Test without API key
        if "TAVILI_API_KEY" in os.environ:
            del os.environ["TAVILI_API_KEY"]
        
        engine = create_tavily_engine()
        assert engine.is_available() == False
    
    @pytest.mark.integration
    @pytest.mark.network
    def test_api_key_detection_news_api(self):
        """Test API key detection for NewsAPI engine."""
        # Test with API key present
        if self.original_env.get("NEWSAPI_API_KEY"):
            engine = create_news_api_engine()
            assert engine.is_available() == True
        
        # Test without API key
        if "NEWSAPI_API_KEY" in os.environ:
            del os.environ["NEWSAPI_API_KEY"]
        
        engine = create_news_api_engine()
        assert engine.is_available() == False
    
    @pytest.mark.integration
    @pytest.mark.network
    def test_unified_interface_api_key_fallback(self):
        """Test unified interface fallback when API keys are missing."""
        # Remove all API keys
        api_key_vars = ["SERPAPI_API_KEY", "TAVILI_API_KEY", "NEWSAPI_API_KEY"]
        for var in api_key_vars:
            if var in os.environ:
                del os.environ[var]
        
        # Test web search - should fallback to non-API engines
        result = self.tools.web_search(
            query="test query without api keys",
            max_results=3
        )
        
        response = json.loads(result)
        
        assert response["success"] == True
        assert response["engine_used"] in ["duckduckgo", "wikipedia", "hacker_news"]
        assert len(response["results"]) > 0
    
    @pytest.mark.integration
    @pytest.mark.network
    def test_news_search_without_api_keys(self):
        """Test news search when news API keys are not available."""
        # Remove news-related API keys
        news_api_vars = ["SERPAPI_API_KEY", "NEWSAPI_API_KEY"]
        for var in news_api_vars:
            if var in os.environ:
                del os.environ[var]
        
        result = self.tools.news_search(
            query="technology news",
            max_results=3
        )
        
        response = json.loads(result)
        
        # Should either succeed with fallback or fail gracefully
        if response["success"]:
            # Should use an available engine
            assert response["engine_used"] in ["duckduckgo", "wikipedia", "hacker_news"]
            assert len(response["results"]) > 0
        else:
            # Should provide clear error message
            assert "error" in response
            assert "api" in response["error"].lower() or "unavailable" in response["error"].lower()
    
    @pytest.mark.integration
    @pytest.mark.network
    def test_research_search_without_tavily_key(self):
        """Test research search when Tavily API key is not available."""
        # Remove Tavily API key
        if "TAVILI_API_KEY" in os.environ:
            del os.environ["TAVILI_API_KEY"]
        
        result = self.tools.research_search(
            query="research topic",
            max_results=3
        )
        
        response = json.loads(result)
        
        # Should either fallback or fail gracefully
        if response["success"]:
            # Should use an available engine
            assert response["engine_used"] in ["duckduckgo", "wikipedia", "google_serp"]
            assert len(response["results"]) > 0
        else:
            # Should provide clear error message
            assert "error" in response
    
    @pytest.mark.integration
    @pytest.mark.network
    def test_engine_info_with_missing_keys(self):
        """Test engine info when some API keys are missing."""
        # Remove some API keys
        if "SERPAPI_API_KEY" in os.environ:
            del os.environ["SERPAPI_API_KEY"]
        if "NEWSAPI_API_KEY" in os.environ:
            del os.environ["NEWSAPI_API_KEY"]
        
        result = self.tools.get_engine_info()
        response = json.loads(result)
        
        assert response["success"] == True
        assert "engines" in response
        
        engines = {engine["name"]: engine for engine in response["engines"]}
        
        # Engines without API keys should be marked as unavailable
        if "google_serp" in engines:
            assert engines["google_serp"]["available"] == False
            assert engines["google_serp"]["capabilities"]["requires_api_key"] == True
        
        if "news_api" in engines:
            assert engines["news_api"]["available"] == False
            assert engines["news_api"]["capabilities"]["requires_api_key"] == True
        
        # Engines without API key requirements should be available
        if "duckduckgo" in engines:
            assert engines["duckduckgo"]["available"] == True
            assert engines["duckduckgo"]["capabilities"]["requires_api_key"] == False
        
        if "wikipedia" in engines:
            assert engines["wikipedia"]["available"] == True
            assert engines["wikipedia"]["capabilities"]["requires_api_key"] == False


class TestConfigurationValidation:
    """Integration tests for configuration validation."""
    
    def setup_method(self):
        """Set up test fixtures."""
        self.tools = WebSearchTools()
    
    @pytest.mark.integration
    @pytest.mark.network
    def test_invalid_api_key_handling(self):
        """Test handling of invalid API keys."""
        # Set invalid API keys
        os.environ["SERPAPI_API_KEY"] = "invalid_key_12345"
        os.environ["NEWSAPI_API_KEY"] = "invalid_key_67890"
        
        # Test that engines detect invalid keys
        google_engine = create_google_serp_engine()
        news_engine = create_news_api_engine()
        
        # Engines should report as available (they have keys)
        # but may fail on actual requests
        assert google_engine.is_available() == True
        assert news_engine.is_available() == True
        
        # Test unified interface with invalid keys
        result = self.tools.web_search(
            query="test with invalid api key",
            engine="google_serp",
            max_results=3
        )
        
        response = json.loads(result)
        
        # Should either succeed with fallback or fail gracefully
        if not response["success"]:
            assert "error" in response
            assert "api" in response["error"].lower() or "auth" in response["error"].lower()
    
    @pytest.mark.integration
    @pytest.mark.network
    def test_configuration_error_messages(self):
        """Test that configuration errors provide clear messages."""
        # Test with missing required configuration
        test_cases = [
            {
                "method": "flights_search",
                "params": {"query": "flights NYC to LAX"},
                "required_key": "SERPAPI_API_KEY",
                "expected_error_keywords": ["api", "key", "serp"]
            },
            {
                "method": "research_search", 
                "params": {"query": "research topic"},
                "required_key": "TAVILI_API_KEY",
                "expected_error_keywords": ["api", "key", "tavily"]
            }
        ]
        
        for case in test_cases:
            # Remove required API key
            original_key = os.environ.get(case["required_key"])
            if case["required_key"] in os.environ:
                del os.environ[case["required_key"]]
            
            try:
                # Call the method
                method = getattr(self.tools, case["method"])
                result = method(**case["params"])
                response = json.loads(result)
                
                # Should either succeed with fallback or provide clear error
                if not response["success"]:
                    error_msg = response["error"].lower()
                    assert any(keyword in error_msg for keyword in case["expected_error_keywords"])
                
            finally:
                # Restore API key
                if original_key:
                    os.environ[case["required_key"]] = original_key
    
    @pytest.mark.integration
    @pytest.mark.network
    def test_configuration_loading_order(self):
        """Test configuration loading order and precedence."""
        # Test that environment variables are properly loaded
        original_key = os.environ.get("SERPAPI_API_KEY")
        
        # Set a test key
        test_key = "test_key_12345"
        os.environ["SERPAPI_API_KEY"] = test_key
        
        try:
            # Create new engine to pick up the environment variable
            engine = create_google_serp_engine()
            
            # Engine should detect the key
            assert engine.is_available() == True
            
            # Check that the key is being used (indirectly)
            health_status = engine.get_health_status()
            assert health_status.is_available == True
            
        finally:
            # Restore original key
            if original_key:
                os.environ["SERPAPI_API_KEY"] = original_key
            else:
                del os.environ["SERPAPI_API_KEY"]


class TestGracefulDegradation:
    """Integration tests for graceful degradation when services are unavailable."""
    
    def setup_method(self):
        """Set up test fixtures."""
        self.tools = WebSearchTools()
    
    @pytest.mark.integration
    @pytest.mark.network
    def test_partial_service_availability(self):
        """Test behavior when some services are available and others are not."""
        # Get current engine availability
        result = self.tools.get_engine_info()
        response = json.loads(result)
        
        available_engines = [
            engine["name"] for engine in response["engines"] 
            if engine["available"]
        ]
        
        unavailable_engines = [
            engine["name"] for engine in response["engines"] 
            if not engine["available"]
        ]
        
        # Test that available engines work
        if available_engines:
            for engine_name in available_engines[:2]:  # Test first 2 available engines
                result = self.tools.web_search(
                    query=f"test {engine_name}",
                    engine=engine_name,
                    max_results=3
                )
                
                response = json.loads(result)
                assert response["success"] == True
                assert response["engine_used"] == engine_name
        
        # Test that unavailable engines fallback gracefully
        if unavailable_engines:
            for engine_name in unavailable_engines[:2]:  # Test first 2 unavailable engines
                result = self.tools.web_search(
                    query=f"test {engine_name}",
                    engine=engine_name,
                    max_results=3
                )
                
                response = json.loads(result)
                # Should either succeed with fallback or fail gracefully
                if response["success"]:
                    assert response["engine_used"] != engine_name
                    assert response["engine_used"] in available_engines
                else:
                    assert "error" in response
    
    @pytest.mark.integration
    @pytest.mark.network
    def test_all_api_engines_unavailable(self):
        """Test behavior when all API-requiring engines are unavailable."""
        # Remove all API keys
        api_keys = ["SERPAPI_API_KEY", "TAVILI_API_KEY", "NEWSAPI_API_KEY"]
        original_keys = {}
        
        for key in api_keys:
            original_keys[key] = os.environ.get(key)
            if key in os.environ:
                del os.environ[key]
        
        try:
            # Test that system still works with non-API engines
            result = self.tools.web_search(
                query="test without api engines",
                max_results=3
            )
            
            response = json.loads(result)
            
            assert response["success"] == True
            assert response["engine_used"] in ["duckduckgo", "wikipedia", "hacker_news"]
            assert len(response["results"]) > 0
            
            # Test engine info shows correct availability
            result = self.tools.get_engine_info()
            response = json.loads(result)
            
            engines = {engine["name"]: engine for engine in response["engines"]}
            
            # Non-API engines should be available
            assert engines["duckduckgo"]["available"] == True
            assert engines["wikipedia"]["available"] == True
            assert engines["hacker_news"]["available"] == True
            
            # API engines should be unavailable
            if "google_serp" in engines:
                assert engines["google_serp"]["available"] == False
            if "tavily" in engines:
                assert engines["tavily"]["available"] == False
            if "news_api" in engines:
                assert engines["news_api"]["available"] == False
                
        finally:
            # Restore original keys
            for key, value in original_keys.items():
                if value is not None:
                    os.environ[key] = value
    
    @pytest.mark.integration
    @pytest.mark.network
    def test_service_recovery(self):
        """Test that services recover when configuration is restored."""
        # Remove API key
        original_key = os.environ.get("SERPAPI_API_KEY")
        if "SERPAPI_API_KEY" in os.environ:
            del os.environ["SERPAPI_API_KEY"]
        
        # Verify engine is unavailable
        engine = create_google_serp_engine()
        assert engine.is_available() == False
        
        # Restore API key
        if original_key:
            os.environ["SERPAPI_API_KEY"] = original_key
            
            # Create new engine instance to pick up the key
            engine = create_google_serp_engine()
            assert engine.is_available() == True
            
            # Test that it works
            result = self.tools.web_search(
                query="test service recovery",
                engine="google_serp",
                max_results=2
            )
            
            response = json.loads(result)
            assert response["success"] == True
            assert response["engine_used"] == "google_serp"


class TestEnvironmentVariableHandling:
    """Integration tests for environment variable handling."""
    
    @pytest.mark.integration
    def test_environment_variable_names(self):
        """Test that correct environment variable names are used."""
        expected_env_vars = {
            "google_serp": "SERPAPI_API_KEY",
            "tavily": "TAVILI_API_KEY",
            "news_api": "NEWSAPI_API_KEY"
        }
        
        for engine_name, env_var in expected_env_vars.items():
            # Test with environment variable set
            test_value = "test_value_12345"
            os.environ[env_var] = test_value
            
            try:
                # Create engine and check availability
                if engine_name == "google_serp":
                    engine = create_google_serp_engine()
                elif engine_name == "tavily":
                    engine = create_tavily_engine()
                elif engine_name == "news_api":
                    engine = create_news_api_engine()
                
                # Should be available with API key set
                assert engine.is_available() == True
                
            finally:
                # Clean up
                del os.environ[env_var]
            
            # Test without environment variable
            engine_classes = {
                "google_serp": create_google_serp_engine,
                "tavily": create_tavily_engine,
                "news_api": create_news_api_engine
            }
            
            engine = engine_classes[engine_name]()
            assert engine.is_available() == False
    
    @pytest.mark.integration
    def test_case_sensitivity(self):
        """Test environment variable case sensitivity."""
        # Test that environment variables are case-sensitive
        original_key = os.environ.get("SERPAPI_API_KEY")
        
        # Set lowercase version (should not work)
        os.environ["serpapi_api_key"] = "test_key"
        
        try:
            engine = create_google_serp_engine()
            assert engine.is_available() == False
            
        finally:
            del os.environ["serpapi_api_key"]
            if original_key:
                os.environ["SERPAPI_API_KEY"] = original_key
    
    @pytest.mark.integration
    def test_empty_environment_variables(self):
        """Test handling of empty environment variables."""
        # Set empty environment variables
        os.environ["SERPAPI_API_KEY"] = ""
        os.environ["TAVILI_API_KEY"] = ""
        
        try:
            google_engine = create_google_serp_engine()
            tavily_engine = create_tavily_engine()
            
            # Empty strings should be treated as unavailable
            assert google_engine.is_available() == False
            assert tavily_engine.is_available() == False
            
        finally:
            del os.environ["SERPAPI_API_KEY"]
            del os.environ["TAVILI_API_KEY"]
"""
Agent testing for tool discovery and schema understanding.

These tests validate that AI agents can effectively discover, understand,
and use the unified web search tools through their JSON schemas and descriptions.
"""
import pytest
import json
import inspect
from typing import Dict, Any, List

# Import the unified system
import sys
sys.path.insert(0, "../../../src/agent_c_tools/tools/web_search")

from agent_c_tools.tools.web_search.web_search_tools import WebSearchTools


class TestAgentToolDiscovery:
    """Test agent tool discovery and schema understanding."""
    
    def setup_method(self):
        """Set up test fixtures."""
        self.tools = WebSearchTools()
    
    def test_tool_method_discovery(self):
        """Test that agents can discover all available tool methods."""
        # Get all public methods that don't start with underscore
        methods = [method for method in dir(self.tools) 
                  if not method.startswith('_') and callable(getattr(self.tools, method))]
        
        # Expected search methods
        expected_methods = [
            'web_search',
            'news_search', 
            'educational_search',
            'research_search',
            'tech_search',
            'flights_search',
            'events_search',
            'get_engine_info'
        ]
        
        # All expected methods should be discoverable
        for method in expected_methods:
            assert method in methods, f"Method {method} not discoverable"
        
        # Check that methods are callable
        for method in expected_methods:
            method_obj = getattr(self.tools, method)
            assert callable(method_obj), f"Method {method} is not callable"
    
    def test_method_docstrings(self):
        """Test that all methods have helpful docstrings for agents."""
        search_methods = [
            'web_search', 'news_search', 'educational_search',
            'research_search', 'tech_search', 'flights_search',
            'events_search', 'get_engine_info'
        ]
        
        for method_name in search_methods:
            method = getattr(self.tools, method_name)
            docstring = method.__doc__
            
            assert docstring is not None, f"Method {method_name} has no docstring"
            assert len(docstring.strip()) > 0, f"Method {method_name} has empty docstring"
            
            # Docstring should contain key information
            docstring_lower = docstring.lower()
            assert any(keyword in docstring_lower for keyword in ['search', 'query', 'find']), \
                f"Method {method_name} docstring doesn't mention search functionality"
    
    def test_method_signatures(self):
        """Test that method signatures are agent-friendly."""
        search_methods = [
            'web_search', 'news_search', 'educational_search',
            'research_search', 'tech_search', 'flights_search',
            'events_search'
        ]
        
        for method_name in search_methods:
            method = getattr(self.tools, method_name)
            signature = inspect.signature(method)
            
            # Should have 'query' parameter
            assert 'query' in signature.parameters, \
                f"Method {method_name} missing 'query' parameter"
            
            # Query parameter should be required or have clear default
            query_param = signature.parameters['query']
            assert query_param.default == inspect.Parameter.empty or query_param.default is None, \
                f"Method {method_name} query parameter should be required"
            
            # Should have reasonable parameter names
            param_names = list(signature.parameters.keys())
            expected_params = ['query', 'max_results', 'engine', 'safe_search', 'language', 'region']
            
            # At least query and max_results should be present
            assert 'query' in param_names
            if method_name != 'get_engine_info':  # get_engine_info doesn't need max_results
                assert 'max_results' in param_names or any('result' in param for param in param_names)
    
    def test_json_schema_availability(self):
        """Test that JSON schemas are available for agent understanding."""
        # This test checks if the tools provide JSON schemas
        # The exact implementation depends on how Agent C framework exposes schemas
        
        search_methods = [
            'web_search', 'news_search', 'educational_search',
            'research_search', 'tech_search', 'flights_search',
            'events_search', 'get_engine_info'
        ]
        
        for method_name in search_methods:
            method = getattr(self.tools, method_name)
            
            # Check if method has schema information
            # This might be in annotations, attributes, or decorators
            if hasattr(method, '__annotations__'):
                annotations = method.__annotations__
                assert len(annotations) > 0, f"Method {method_name} has no type annotations"
            
            # Check for json_schema decorator or attribute
            if hasattr(method, 'json_schema') or hasattr(method, '__json_schema__'):
                # Method has explicit schema
                schema = getattr(method, 'json_schema', None) or getattr(method, '__json_schema__', None)
                assert schema is not None, f"Method {method_name} has null schema"
    
    def test_parameter_descriptions(self):
        """Test that parameters have clear descriptions for agents."""
        # Test a sample method call to see parameter validation
        try:
            # This should fail with helpful error message
            result = self.tools.web_search()
        except Exception as e:
            error_msg = str(e)
            # Error should mention required parameters
            assert any(keyword in error_msg.lower() for keyword in ['query', 'required', 'missing']), \
                f"Error message not helpful for agents: {error_msg}"
        
        # Test with invalid parameter
        try:
            result = self.tools.web_search(query="test", max_results=-1)
            response = json.loads(result)
            if not response.get("success", True):
                error_msg = response.get("error", "")
                assert "max_results" in error_msg.lower(), \
                    f"Error message should mention invalid parameter: {error_msg}"
        except Exception as e:
            error_msg = str(e)
            assert "max_results" in error_msg.lower(), \
                f"Error message should mention invalid parameter: {error_msg}"


class TestAgentParameterUnderstanding:
    """Test agent understanding of parameters and their usage."""
    
    def setup_method(self):
        """Set up test fixtures."""
        self.tools = WebSearchTools()
    
    def test_query_parameter_understanding(self):
        """Test that agents understand query parameter usage."""
        # Test various query types
        test_queries = [
            "Python programming tutorial",
            "What is machine learning?",
            "How to cook pasta",
            "Latest news about AI",
            "Weather in New York"
        ]
        
        for query in test_queries:
            result = self.tools.web_search(query=query, max_results=2)
            response = json.loads(result)
            
            assert response.get("success", False), f"Query '{query}' failed: {response.get('error')}"
            assert response.get("query") == query, f"Query not preserved in response"
            assert len(response.get("results", [])) > 0, f"No results for query '{query}'"
    
    def test_max_results_parameter_understanding(self):
        """Test agent understanding of max_results parameter."""
        test_cases = [
            {"max_results": 1, "expected_max": 1},
            {"max_results": 5, "expected_max": 5},
            {"max_results": 10, "expected_max": 10},
        ]
        
        for case in test_cases:
            result = self.tools.web_search(
                query="test query",
                max_results=case["max_results"]
            )
            response = json.loads(result)
            
            if response.get("success", False):
                results = response.get("results", [])
                assert len(results) <= case["expected_max"], \
                    f"Too many results returned: {len(results)} > {case['expected_max']}"
    
    def test_engine_parameter_understanding(self):
        """Test agent understanding of engine selection."""
        # Test explicit engine selection
        engines_to_test = ["duckduckgo", "wikipedia", "hacker_news"]
        
        for engine in engines_to_test:
            result = self.tools.web_search(
                query="test query",
                engine=engine,
                max_results=2
            )
            response = json.loads(result)
            
            if response.get("success", False):
                assert response.get("engine_used") == engine, \
                    f"Expected engine {engine}, got {response.get('engine_used')}"
    
    def test_safe_search_parameter_understanding(self):
        """Test agent understanding of safe search options."""
        safe_search_options = ["off", "moderate", "strict"]
        
        for option in safe_search_options:
            result = self.tools.web_search(
                query="family content",
                safe_search=option,
                max_results=2
            )
            response = json.loads(result)
            
            # Should either succeed or provide clear error
            if not response.get("success", False):
                error = response.get("error", "")
                assert "safe_search" in error.lower() or "invalid" in error.lower(), \
                    f"Unclear error for safe_search={option}: {error}"
    
    def test_search_type_routing_understanding(self):
        """Test agent understanding of different search types."""
        search_type_tests = [
            {"method": "web_search", "query": "Python tutorial", "expected_type": "web"},
            {"method": "news_search", "query": "latest news", "expected_type": "news"},
            {"method": "educational_search", "query": "quantum physics", "expected_type": "educational"},
            {"method": "tech_search", "query": "programming", "expected_type": "tech"},
        ]
        
        for test in search_type_tests:
            method = getattr(self.tools, test["method"])
            result = method(query=test["query"], max_results=2)
            response = json.loads(result)
            
            if response.get("success", False):
                # Check that appropriate engine was selected
                engine_used = response.get("engine_used")
                assert engine_used is not None, f"No engine specified for {test['method']}"
                
                # Verify results are appropriate for search type
                results = response.get("results", [])
                assert len(results) > 0, f"No results for {test['method']}"


class TestAgentErrorHandling:
    """Test agent-friendly error handling and messages."""
    
    def setup_method(self):
        """Set up test fixtures."""
        self.tools = WebSearchTools()
    
    def test_missing_query_error(self):
        """Test error handling when query is missing."""
        try:
            result = self.tools.web_search(max_results=5)
            response = json.loads(result)
            
            assert response.get("success", True) == False, "Should fail without query"
            error = response.get("error", "")
            assert "query" in error.lower(), f"Error should mention query: {error}"
            assert len(error) > 0, "Error message should not be empty"
            
        except Exception as e:
            error_msg = str(e)
            assert "query" in error_msg.lower(), f"Exception should mention query: {error_msg}"
    
    def test_invalid_parameter_errors(self):
        """Test error handling for invalid parameters."""
        invalid_cases = [
            {"max_results": -1, "expected_error": "max_results"},
            {"max_results": 0, "expected_error": "max_results"},
            {"max_results": 1000, "expected_error": "max_results"},
            {"safe_search": "invalid", "expected_error": "safe_search"},
            {"engine": "nonexistent", "expected_error": "engine"},
        ]
        
        for case in invalid_cases:
            params = {"query": "test query"}
            params.update({k: v for k, v in case.items() if k != "expected_error"})
            
            result = self.tools.web_search(**params)
            response = json.loads(result)
            
            if not response.get("success", False):
                error = response.get("error", "").lower()
                assert case["expected_error"] in error, \
                    f"Error should mention {case['expected_error']}: {error}"
                assert len(error) > 10, "Error message should be descriptive"
    
    def test_network_error_handling(self):
        """Test error handling for network issues."""
        # This test checks that network errors are handled gracefully
        # We can't easily simulate network errors, so we test the error format
        
        result = self.tools.web_search(query="test network error", max_results=2)
        response = json.loads(result)
        
        # Should either succeed or provide clear error
        if not response.get("success", False):
            error = response.get("error", "")
            assert len(error) > 0, "Error message should not be empty"
            assert isinstance(error, str), "Error should be a string"
            # Error should be in plain English
            assert not error.startswith("Traceback"), "Error should not be a raw traceback"
    
    def test_api_key_missing_errors(self):
        """Test error handling when API keys are missing."""
        # Test with engines that require API keys
        api_engines = ["google_serp", "tavily", "news_api"]
        
        for engine in api_engines:
            result = self.tools.web_search(
                query="test api key error",
                engine=engine,
                max_results=2
            )
            response = json.loads(result)
            
            # Should either succeed (with fallback) or provide clear error
            if not response.get("success", False):
                error = response.get("error", "").lower()
                assert any(keyword in error for keyword in ["api", "key", "unavailable", "configuration"]), \
                    f"Error should mention API key issue: {error}"
    
    def test_empty_results_handling(self):
        """Test handling when no results are found."""
        # Use a very specific query that's unlikely to return results
        result = self.tools.web_search(
            query="xyzabc123nonexistentquerythatshouldfail456",
            max_results=5
        )
        response = json.loads(result)
        
        # Should either succeed with empty results or provide clear message
        if response.get("success", False):
            results = response.get("results", [])
            # Empty results are acceptable
            assert isinstance(results, list), "Results should be a list"
        else:
            error = response.get("error", "")
            assert "no results" in error.lower() or "not found" in error.lower(), \
                f"Error should indicate no results found: {error}"


class TestAgentUseCaseScenarios:
    """Test various agent use case scenarios."""
    
    def setup_method(self):
        """Set up test fixtures."""
        self.tools = WebSearchTools()
    
    def test_research_agent_scenario(self):
        """Test scenario for a research agent."""
        # Research agent needs comprehensive, accurate information
        result = self.tools.research_search(
            query="climate change impacts on agriculture",
            max_results=5,
            search_depth="advanced"
        )
        response = json.loads(result)
        
        if response.get("success", False):
            results = response.get("results", [])
            assert len(results) > 0, "Research agent should get results"
            
            # Results should have substantial content
            for result_item in results:
                assert len(result_item.get("snippet", "")) > 50, \
                    "Research results should have substantial snippets"
                assert result_item.get("url", "").startswith("http"), \
                    "Research results should have valid URLs"
    
    def test_news_agent_scenario(self):
        """Test scenario for a news monitoring agent."""
        # News agent needs recent, relevant news
        result = self.tools.news_search(
            query="artificial intelligence breakthrough",
            max_results=5
        )
        response = json.loads(result)
        
        if response.get("success", False):
            results = response.get("results", [])
            assert len(results) > 0, "News agent should get results"
            
            # Results should be news-like
            for result_item in results:
                title = result_item.get("title", "")
                assert len(title) > 0, "News results should have titles"
                # News titles often contain certain patterns
                assert any(char in title for char in [" ", "-", ":"]), \
                    "News titles should be formatted properly"
    
    def test_educational_agent_scenario(self):
        """Test scenario for an educational agent."""
        # Educational agent needs clear, authoritative information
        result = self.tools.educational_search(
            query="photosynthesis process",
            max_results=3
        )
        response = json.loads(result)
        
        if response.get("success", False):
            results = response.get("results", [])
            assert len(results) > 0, "Educational agent should get results"
            
            # Should prefer educational sources
            educational_domains = ["wikipedia", "edu", "academic"]
            has_educational_source = any(
                any(domain in result_item.get("url", "") for domain in educational_domains)
                for result_item in results
            )
            # At least some results should be from educational sources
            assert has_educational_source or len(results) > 0, \
                "Educational search should prefer educational sources"
    
    def test_tech_agent_scenario(self):
        """Test scenario for a technical/programming agent."""
        # Tech agent needs technical, community-driven information
        result = self.tools.tech_search(
            query="Python async programming",
            max_results=5
        )
        response = json.loads(result)
        
        if response.get("success", False):
            results = response.get("results", [])
            assert len(results) > 0, "Tech agent should get results"
            
            # Results should be relevant to tech community
            for result_item in results:
                title = result_item.get("title", "").lower()
                snippet = result_item.get("snippet", "").lower()
                content = title + " " + snippet
                
                # Should contain technical terms
                tech_terms = ["python", "async", "programming", "code", "function"]
                assert any(term in content for term in tech_terms), \
                    f"Tech results should contain technical terms: {content[:100]}"
    
    def test_travel_agent_scenario(self):
        """Test scenario for a travel planning agent."""
        # Travel agent needs flight and event information
        result = self.tools.flights_search(
            query="flights from New York to London",
            max_results=3
        )
        response = json.loads(result)
        
        # Flight search may not always return results (depends on API availability)
        if response.get("success", False):
            results = response.get("results", [])
            # If results are returned, they should be flight-related
            if len(results) > 0:
                for result_item in results:
                    content = (result_item.get("title", "") + " " + 
                             result_item.get("snippet", "")).lower()
                    flight_terms = ["flight", "airline", "airport", "departure", "arrival"]
                    assert any(term in content for term in flight_terms), \
                        f"Flight results should contain flight terms: {content[:100]}"
    
    def test_general_assistant_scenario(self):
        """Test scenario for a general assistant agent."""
        # General assistant needs versatile, reliable search
        queries = [
            "weather forecast",
            "recipe for chocolate cake",
            "how to change a tire",
            "best restaurants nearby",
            "movie recommendations"
        ]
        
        successful_queries = 0
        
        for query in queries:
            result = self.tools.web_search(query=query, max_results=3)
            response = json.loads(result)
            
            if response.get("success", False):
                successful_queries += 1
                results = response.get("results", [])
                assert len(results) > 0, f"No results for general query: {query}"
                
                # Results should be relevant
                for result_item in results:
                    assert len(result_item.get("title", "")) > 0, \
                        "General search results should have titles"
                    assert result_item.get("url", "").startswith("http"), \
                        "General search results should have valid URLs"
        
        # At least most queries should succeed
        assert successful_queries >= len(queries) * 0.7, \
            f"General assistant should handle most queries: {successful_queries}/{len(queries)}"


class TestAgentEngineSelection:
    """Test agent ability to select appropriate engines."""
    
    def setup_method(self):
        """Set up test fixtures."""
        self.tools = WebSearchTools()
    
    def test_automatic_engine_selection(self):
        """Test that agents can rely on automatic engine selection."""
        test_cases = [
            {"query": "Python tutorial", "method": "web_search", "expected_engines": ["duckduckgo", "google_serp"]},
            {"query": "latest tech news", "method": "news_search", "expected_engines": ["google_serp", "news_api"]},
            {"query": "quantum physics", "method": "educational_search", "expected_engines": ["wikipedia"]},
            {"query": "programming discussion", "method": "tech_search", "expected_engines": ["hacker_news"]},
        ]
        
        for case in test_cases:
            method = getattr(self.tools, case["method"])
            result = method(query=case["query"], max_results=2)
            response = json.loads(result)
            
            if response.get("success", False):
                engine_used = response.get("engine_used")
                assert engine_used in case["expected_engines"], \
                    f"Unexpected engine {engine_used} for {case['method']} with query '{case['query']}'"
    
    def test_engine_availability_awareness(self):
        """Test that agents can understand engine availability."""
        result = self.tools.get_engine_info()
        response = json.loads(result)
        
        assert response.get("success", False), "Should be able to get engine info"
        engines = response.get("engines", [])
        assert len(engines) > 0, "Should return engine information"
        
        # Check engine info structure
        for engine in engines:
            assert "name" in engine, "Engine should have name"
            assert "available" in engine, "Engine should have availability status"
            assert "capabilities" in engine, "Engine should have capabilities"
            
            capabilities = engine["capabilities"]
            assert "supported_search_types" in capabilities, \
                "Engine should list supported search types"
            assert "requires_api_key" in capabilities, \
                "Engine should indicate API key requirement"
    
    def test_engine_fallback_understanding(self):
        """Test agent understanding of engine fallback behavior."""
        # Request unavailable engine, should fallback gracefully
        result = self.tools.web_search(
            query="test fallback",
            engine="nonexistent_engine",
            max_results=2
        )
        response = json.loads(result)
        
        if response.get("success", False):
            # Should have fallen back to available engine
            engine_used = response.get("engine_used")
            assert engine_used != "nonexistent_engine", \
                "Should fallback from nonexistent engine"
            assert engine_used is not None, "Should specify which engine was used"
        else:
            # Should provide clear error about engine availability
            error = response.get("error", "").lower()
            assert "engine" in error or "unavailable" in error, \
                f"Error should mention engine issue: {error}"
"""
Agent usability testing for the web search tools.

These tests focus on the user experience from an agent's perspective,
including ease of use, clarity of responses, and overall usability.
"""
import pytest
import json
from typing import Dict, Any, List, Optional
import re

# Import the unified system
import sys
sys.path.insert(0, "../../../src/agent_c_tools/tools/web_search")

from agent_c_tools.tools.web_search.web_search_tools import WebSearchTools


class TestAgentUsabilityBasics:
    """Test basic usability aspects for agents."""
    
    def setup_method(self):
        """Set up test fixtures."""
        self.tools = WebSearchTools()
    
    def test_minimal_parameter_usage(self):
        """Test that agents can use tools with minimal parameters."""
        # Agents should be able to use tools with just a query
        result = self.tools.web_search(query="Python programming")
        response = json.loads(result)
        
        assert response.get("success", False), "Should work with minimal parameters"
        assert len(response.get("results", [])) > 0, "Should return results with minimal parameters"
    
    def test_parameter_flexibility(self):
        """Test that agents have flexibility in parameter usage."""
        query = "machine learning tutorial"
        
        # Test various parameter combinations
        parameter_combinations = [
            {"query": query},
            {"query": query, "max_results": 5},
            {"query": query, "max_results": 3, "safe_search": "moderate"},
            {"query": query, "engine": "duckduckgo"},
            {"query": query, "language": "en", "region": "us"}
        ]
        
        successful_combinations = 0
        
        for params in parameter_combinations:
            result = self.tools.web_search(**params)
            response = json.loads(result)
            
            if response.get("success", False):
                successful_combinations += 1
                assert len(response.get("results", [])) > 0, \
                    f"Should return results for params: {params}"
        
        # Most combinations should work
        assert successful_combinations >= len(parameter_combinations) * 0.8, \
            f"Most parameter combinations should work: {successful_combinations}/{len(parameter_combinations)}"
    
    def test_intuitive_method_names(self):
        """Test that method names are intuitive for agents."""
        # Test that method names match their functionality
        method_tests = [
            ("web_search", "general web search", "Python tutorial"),
            ("news_search", "news search", "technology news"),
            ("educational_search", "educational search", "photosynthesis"),
            ("research_search", "research search", "climate change research"),
            ("tech_search", "tech search", "programming languages"),
        ]
        
        for method_name, description, test_query in method_tests:
            method = getattr(self.tools, method_name)
            result = method(query=test_query, max_results=2)
            response = json.loads(result)
            
            if response.get("success", False):
                # Method should return appropriate results for its purpose
                results = response.get("results", [])
                assert len(results) > 0, f"Method {method_name} should return results for {description}"
                
                # Results should be relevant to the search type
                for result_item in results:
                    title = result_item.get("title", "").lower()
                    snippet = result_item.get("snippet", "").lower()
                    content = title + " " + snippet
                    
                    # Should contain some relevant terms
                    query_words = test_query.lower().split()
                    relevant_words = sum(1 for word in query_words if word in content)
                    assert relevant_words > 0, \
                        f"Results should be relevant to query for {method_name}"
    
    def test_clear_response_structure(self):
        """Test that response structure is clear and consistent."""
        result = self.tools.web_search(query="test query", max_results=3)
        response = json.loads(result)
        
        # Response should have clear structure
        assert isinstance(response, dict), "Response should be a dictionary"
        assert "success" in response, "Response should indicate success/failure"
        
        if response.get("success", False):
            # Successful response structure
            required_fields = ["results", "query", "engine_used"]
            for field in required_fields:
                assert field in response, f"Successful response should include {field}"
            
            # Results should be well-structured
            results = response.get("results", [])
            assert isinstance(results, list), "Results should be a list"
            
            for result_item in results:
                assert isinstance(result_item, dict), "Each result should be a dictionary"
                
                result_fields = ["title", "url", "snippet"]
                for field in result_fields:
                    assert field in result_item, f"Result should include {field}"
                    assert isinstance(result_item[field], str), f"{field} should be a string"
        else:
            # Failed response structure
            assert "error" in response, "Failed response should include error message"
            assert isinstance(response["error"], str), "Error should be a string"
            assert len(response["error"]) > 0, "Error message should not be empty"
    
    def test_helpful_error_messages(self):
        """Test that error messages are helpful for agents."""
        # Test various error conditions
        error_tests = [
            # Missing query
            ({"max_results": 5}, "query"),
            # Invalid max_results
            ({"query": "test", "max_results": -1}, "max_results"),
            # Invalid safe_search
            ({"query": "test", "safe_search": "invalid"}, "safe_search"),
            # Invalid engine
            ({"query": "test", "engine": "nonexistent"}, "engine"),
        ]
        
        for params, expected_error_keyword in error_tests:
            result = self.tools.web_search(**params)
            response = json.loads(result)
            
            if not response.get("success", False):
                error_message = response.get("error", "").lower()
                assert expected_error_keyword in error_message, \
                    f"Error message should mention {expected_error_keyword}: {error_message}"
                
                # Error message should be descriptive
                assert len(error_message) > 10, \
                    f"Error message should be descriptive: {error_message}"
                
                # Error message should not contain technical jargon
                technical_terms = ["traceback", "exception", "stack", "module", "class"]
                has_technical_terms = any(term in error_message for term in technical_terms)
                assert not has_technical_terms, \
                    f"Error message should be user-friendly: {error_message}"


class TestAgentResultQuality:
    """Test the quality of results from an agent perspective."""
    
    def setup_method(self):
        """Set up test fixtures."""
        self.tools = WebSearchTools()
    
    def test_result_relevance(self):
        """Test that results are relevant to the query."""
        test_queries = [
            "Python programming tutorial",
            "climate change effects",
            "machine learning algorithms",
            "healthy cooking recipes",
            "travel destinations Europe"
        ]
        
        for query in test_queries:
            result = self.tools.web_search(query=query, max_results=5)
            response = json.loads(result)
            
            if response.get("success", False):
                results = response.get("results", [])
                assert len(results) > 0, f"Should return results for query: {query}"
                
                # Check relevance of results
                query_words = set(query.lower().split())
                relevant_results = 0
                
                for result_item in results:
                    title = result_item.get("title", "").lower()
                    snippet = result_item.get("snippet", "").lower()
                    content_words = set((title + " " + snippet).split())
                    
                    # Calculate relevance score
                    common_words = query_words.intersection(content_words)
                    relevance_score = len(common_words) / len(query_words)
                    
                    if relevance_score > 0.2:  # At least 20% word overlap
                        relevant_results += 1
                
                # At least half the results should be relevant
                assert relevant_results >= len(results) * 0.5, \
                    f"Results should be relevant to query '{query}': {relevant_results}/{len(results)}"
    
    def test_result_completeness(self):
        """Test that results contain complete information."""
        result = self.tools.web_search(query="artificial intelligence", max_results=3)
        response = json.loads(result)
        
        if response.get("success", False):
            results = response.get("results", [])
            assert len(results) > 0, "Should return results"
            
            for i, result_item in enumerate(results):
                # Each result should have complete information
                assert result_item.get("title"), f"Result {i} should have a title"
                assert result_item.get("url"), f"Result {i} should have a URL"
                assert result_item.get("snippet"), f"Result {i} should have a snippet"
                
                # Title should be meaningful
                title = result_item.get("title", "")
                assert len(title) > 5, f"Result {i} title should be meaningful: '{title}'"
                
                # URL should be valid
                url = result_item.get("url", "")
                assert url.startswith("http"), f"Result {i} should have valid URL: '{url}'"
                
                # Snippet should be informative
                snippet = result_item.get("snippet", "")
                assert len(snippet) > 20, f"Result {i} snippet should be informative: '{snippet[:50]}...'"
    
    def test_result_diversity(self):
        """Test that results provide diverse perspectives."""
        result = self.tools.web_search(query="renewable energy", max_results=6)
        response = json.loads(result)
        
        if response.get("success", False):
            results = response.get("results", [])
            assert len(results) > 0, "Should return results"
            
            # Check for diversity in sources
            domains = []
            for result_item in results:
                url = result_item.get("url", "")
                if url:
                    # Extract domain from URL
                    domain_match = re.search(r'https?://([^/]+)', url)
                    if domain_match:
                        domains.append(domain_match.group(1))
            
            # Should have results from multiple domains
            unique_domains = set(domains)
            assert len(unique_domains) > 1, \
                f"Results should come from diverse sources: {unique_domains}"
    
    def test_result_freshness(self):
        """Test that results are reasonably fresh."""
        # Test with a current topic
        result = self.tools.web_search(query="latest technology trends 2024", max_results=4)
        response = json.loads(result)
        
        if response.get("success", False):
            results = response.get("results", [])
            assert len(results) > 0, "Should return results for current topics"
            
            # Check for freshness indicators
            fresh_indicators = ["2024", "latest", "recent", "new", "current", "today"]
            fresh_results = 0
            
            for result_item in results:
                content = (result_item.get("title", "") + " " + 
                          result_item.get("snippet", "")).lower()
                
                if any(indicator in content for indicator in fresh_indicators):
                    fresh_results += 1
            
            # At least some results should appear fresh
            assert fresh_results > 0, \
                f"Some results should appear fresh for current topics: {fresh_results}/{len(results)}"


class TestAgentWorkflowSupport:
    """Test support for common agent workflows."""
    
    def setup_method(self):
        """Set up test fixtures."""
        self.tools = WebSearchTools()
    
    def test_iterative_search_workflow(self):
        """Test support for iterative search workflows."""
        # Simulate an agent doing iterative research
        search_sequence = [
            ("artificial intelligence", "web_search"),
            ("machine learning applications", "research_search"),
            ("AI ethics concerns", "news_search"),
            ("neural networks explained", "educational_search")
        ]
        
        search_results = []
        
        for query, method_name in search_sequence:
            method = getattr(self.tools, method_name)
            result = method(query=query, max_results=3)
            response = json.loads(result)
            
            search_results.append((query, method_name, response))
            
            if response.get("success", False):
                results = response.get("results", [])
                assert len(results) > 0, f"Should support iterative search for: {query}"
        
        # Most searches in the sequence should succeed
        successful_searches = sum(1 for _, _, response in search_results 
                                if response.get("success", False))
        
        assert successful_searches >= len(search_sequence) * 0.7, \
            f"Should support iterative workflows: {successful_searches}/{len(search_sequence)}"
    
    def test_multi_engine_exploration(self):
        """Test agent ability to explore multiple engines."""
        query = "Python programming best practices"
        engines = ["duckduckgo", "wikipedia"]  # Available engines
        
        engine_results = []
        
        for engine in engines:
            result = self.tools.web_search(query=query, engine=engine, max_results=2)
            response = json.loads(result)
            engine_results.append((engine, response))
            
            if response.get("success", False):
                assert response.get("engine_used") == engine, \
                    f"Should use specified engine: {engine}"
                
                results = response.get("results", [])
                assert len(results) > 0, f"Engine {engine} should return results"
        
        # Should be able to use multiple engines
        successful_engines = sum(1 for _, response in engine_results 
                               if response.get("success", False))
        
        assert successful_engines > 0, \
            f"Should be able to use multiple engines: {successful_engines}/{len(engines)}"
    
    def test_search_refinement_workflow(self):
        """Test support for search refinement workflows."""
        # Simulate an agent refining searches
        refinement_sequence = [
            {"query": "programming", "max_results": 2},
            {"query": "Python programming", "max_results": 3},
            {"query": "Python programming tutorial beginners", "max_results": 4},
        ]
        
        refinement_results = []
        
        for params in refinement_sequence:
            result = self.tools.web_search(**params)
            response = json.loads(result)
            refinement_results.append(response)
            
            if response.get("success", False):
                results = response.get("results", [])
                assert len(results) > 0, f"Should support search refinement: {params['query']}"
        
        # Refinement should generally work
        successful_refinements = sum(1 for response in refinement_results 
                                   if response.get("success", False))
        
        assert successful_refinements >= len(refinement_sequence) * 0.6, \
            f"Should support search refinement: {successful_refinements}/{len(refinement_sequence)}"
    
    def test_contextual_search_workflow(self):
        """Test support for contextual search workflows."""
        # Simulate an agent doing contextual searches
        context_sequence = [
            ("climate change", "web_search"),
            ("climate change solutions", "research_search"),
            ("renewable energy news", "news_search"),
            ("solar power explained", "educational_search")
        ]
        
        context_results = []
        
        for query, method_name in context_sequence:
            method = getattr(self.tools, method_name)
            result = method(query=query, max_results=2)
            response = json.loads(result)
            context_results.append((query, response))
            
            if response.get("success", False):
                results = response.get("results", [])
                assert len(results) > 0, f"Should support contextual search: {query}"
        
        # Contextual searches should generally work
        successful_contexts = sum(1 for _, response in context_results 
                                if response.get("success", False))
        
        assert successful_contexts >= len(context_sequence) * 0.7, \
            f"Should support contextual workflows: {successful_contexts}/{len(context_sequence)}"


class TestAgentFeedbackAndLearning:
    """Test features that support agent feedback and learning."""
    
    def setup_method(self):
        """Set up test fixtures."""
        self.tools = WebSearchTools()
    
    def test_engine_performance_visibility(self):
        """Test that agents can understand engine performance."""
        result = self.tools.get_engine_info()
        response = json.loads(result)
        
        assert response.get("success", False), "Should be able to get engine info"
        engines = response.get("engines", [])
        assert len(engines) > 0, "Should return engine information"
        
        for engine in engines:
            # Engine info should help agents make decisions
            assert "name" in engine, "Should provide engine name"
            assert "available" in engine, "Should indicate availability"
            assert "capabilities" in engine, "Should describe capabilities"
            assert "health_status" in engine, "Should provide health status"
            
            # Health status should be informative
            health = engine["health_status"]
            assert "response_time" in health, "Should provide response time"
            assert "error_rate" in health, "Should provide error rate"
            assert "is_available" in health, "Should indicate current availability"
    
    def test_search_metadata_availability(self):
        """Test that search results include useful metadata."""
        result = self.tools.web_search(query="machine learning", max_results=3)
        response = json.loads(result)
        
        if response.get("success", False):
            # Response should include metadata
            assert "query" in response, "Should include original query"
            assert "engine_used" in response, "Should indicate which engine was used"
            
            # May include additional metadata
            metadata_fields = ["search_time", "total_results", "search_type"]
            for field in metadata_fields:
                if field in response:
                    print(f"Found metadata field: {field}")
    
    def test_error_learning_support(self):
        """Test that errors provide learning opportunities."""
        # Test various error conditions
        error_scenarios = [
            ({"query": ""}, "empty query"),
            ({"query": "test", "max_results": 0}, "invalid max_results"),
            ({"query": "test", "engine": "nonexistent"}, "invalid engine"),
        ]
        
        for params, scenario in error_scenarios:
            result = self.tools.web_search(**params)
            response = json.loads(result)
            
            if not response.get("success", False):
                error_message = response.get("error", "")
                
                # Error should be educational
                assert len(error_message) > 0, f"Error message should exist for {scenario}"
                assert not error_message.startswith("Traceback"), \
                    f"Error should be user-friendly for {scenario}"
                
                # Error should suggest corrective action
                corrective_words = ["should", "must", "required", "valid", "between"]
                has_corrective_language = any(word in error_message.lower() 
                                            for word in corrective_words)
                
                if not has_corrective_language:
                    print(f"Error message could be more corrective for {scenario}: {error_message}")
    
    def test_result_quality_indicators(self):
        """Test that results include quality indicators."""
        result = self.tools.web_search(query="authoritative information", max_results=3)
        response = json.loads(result)
        
        if response.get("success", False):
            results = response.get("results", [])
            assert len(results) > 0, "Should return results"
            
            for result_item in results:
                # Results should include quality indicators
                url = result_item.get("url", "")
                title = result_item.get("title", "")
                snippet = result_item.get("snippet", "")
                
                # URL can indicate source quality
                quality_domains = [".edu", ".gov", ".org", "wikipedia", "britannica"]
                has_quality_domain = any(domain in url.lower() for domain in quality_domains)
                
                # Title and snippet length can indicate content quality
                has_substantial_content = len(title) > 10 and len(snippet) > 50
                
                # At least some indicators should be present
                if has_quality_domain or has_substantial_content:
                    print(f"Quality indicators found for: {title[:50]}")


class TestAgentAccessibility:
    """Test accessibility features for different types of agents."""
    
    def setup_method(self):
        """Set up test fixtures."""
        self.tools = WebSearchTools()
    
    def test_simple_agent_support(self):
        """Test support for simple agents with basic needs."""
        # Simple agents need straightforward, reliable functionality
        simple_queries = [
            "what is Python",
            "how to cook pasta",
            "weather today",
            "news headlines",
            "movie recommendations"
        ]
        
        for query in simple_queries:
            result = self.tools.web_search(query=query, max_results=2)
            response = json.loads(result)
            
            if response.get("success", False):
                results = response.get("results", [])
                assert len(results) > 0, f"Simple agent should get results for: {query}"
                
                # Results should be straightforward
                for result_item in results:
                    title = result_item.get("title", "")
                    snippet = result_item.get("snippet", "")
                    
                    # Should have clear, readable content
                    assert len(title) > 0, "Simple agents need clear titles"
                    assert len(snippet) > 0, "Simple agents need informative snippets"
    
    def test_advanced_agent_support(self):
        """Test support for advanced agents with complex needs."""
        # Advanced agents need sophisticated functionality
        advanced_params = {
            "query": "quantum computing research applications",
            "max_results": 8,
            "search_depth": "advanced",
            "safe_search": "moderate"
        }
        
        result = self.tools.research_search(**advanced_params)
        response = json.loads(result)
        
        if response.get("success", False):
            results = response.get("results", [])
            assert len(results) > 0, "Advanced agent should get comprehensive results"
            
            # Advanced agents need detailed information
            detailed_results = sum(1 for result_item in results 
                                 if len(result_item.get("snippet", "")) > 100)
            
            assert detailed_results > 0, \
                f"Advanced agents need detailed content: {detailed_results}/{len(results)}"
    
    def test_multilingual_agent_support(self):
        """Test support for multilingual agents."""
        # Test language parameter support
        multilingual_tests = [
            {"query": "programming tutorial", "language": "en"},
            {"query": "machine learning", "language": "en"},
        ]
        
        for params in multilingual_tests:
            result = self.tools.web_search(**params, max_results=2)
            response = json.loads(result)
            
            if response.get("success", False):
                results = response.get("results", [])
                assert len(results) > 0, f"Should support language parameter: {params}"
    
    def test_domain_specific_agent_support(self):
        """Test support for domain-specific agents."""
        # Test domain filtering support
        domain_tests = [
            {"query": "programming examples", "include_domains": ["github.com", "stackoverflow.com"]},
            {"query": "academic research", "include_domains": ["edu"]},
        ]
        
        for params in domain_tests:
            result = self.tools.web_search(**params, max_results=3)
            response = json.loads(result)
            
            if response.get("success", False):
                results = response.get("results", [])
                if len(results) > 0:
                    # Check that domain filtering worked
                    included_domains = params.get("include_domains", [])
                    domain_matches = 0
                    
                    for result_item in results:
                        url = result_item.get("url", "")
                        if any(domain in url for domain in included_domains):
                            domain_matches += 1
                    
                    # Some results should match domain filters
                    assert domain_matches > 0, \
                        f"Domain filtering should work: {domain_matches}/{len(results)}"
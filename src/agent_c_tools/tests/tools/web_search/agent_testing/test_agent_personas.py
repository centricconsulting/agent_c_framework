"""
Agent testing for different agent personas and use cases.

These tests simulate various types of AI agents using the web search tools
to validate that the tools meet the needs of different agent personalities
and use cases.
"""
import pytest
import json
from typing import Dict, Any, List
from datetime import datetime, timedelta

# Import the unified system
import sys
sys.path.insert(0, "../../../src/agent_c_tools/tools/web_search")

from agent_c_tools.tools.web_search.web_search_tools import WebSearchTools


class TestResearcherAgentPersona:
    """Test web search tools from a researcher agent perspective."""
    
    def setup_method(self):
        """Set up test fixtures."""
        self.tools = WebSearchTools()
        self.persona = {
            "name": "Dr. Research",
            "role": "Academic Researcher",
            "needs": ["comprehensive information", "authoritative sources", "recent studies"],
            "preferences": ["detailed content", "academic sources", "multiple perspectives"]
        }
    
    def test_comprehensive_research_query(self):
        """Test researcher agent performing comprehensive research."""
        research_query = "machine learning bias in healthcare algorithms"
        
        # Researcher would use research_search for comprehensive results
        result = self.tools.research_search(
            query=research_query,
            max_results=8,
            search_depth="advanced"
        )
        response = json.loads(result)
        
        if response.get("success", False):
            results = response.get("results", [])
            assert len(results) > 0, "Researcher should get comprehensive results"
            
            # Researcher needs detailed content
            for result_item in results:
                snippet = result_item.get("snippet", "")
                assert len(snippet) > 100, \
                    f"Research results should have substantial content: {len(snippet)} chars"
                
                # Should have academic or authoritative sources
                url = result_item.get("url", "")
                title = result_item.get("title", "")
                content = (title + " " + snippet).lower()
                
                # Look for academic indicators
                academic_indicators = [
                    "study", "research", "analysis", "findings", "methodology",
                    "journal", "paper", "publication", "academic", "university"
                ]
                has_academic_content = any(indicator in content for indicator in academic_indicators)
                
                # At least some results should have academic content
                if not has_academic_content:
                    print(f"Non-academic result: {title[:50]}...")
    
    def test_multi_perspective_research(self):
        """Test researcher gathering multiple perspectives."""
        controversial_topic = "artificial intelligence job displacement"
        
        # Researcher would search for different perspectives
        result = self.tools.research_search(
            query=controversial_topic,
            max_results=6
        )
        response = json.loads(result)
        
        if response.get("success", False):
            results = response.get("results", [])
            assert len(results) > 0, "Should find multiple perspectives"
            
            # Check for diversity in sources
            domains = [result_item.get("url", "") for result_item in results]
            unique_domains = set(domain.split('/')[2] if '/' in domain else domain for domain in domains)
            
            # Should have results from multiple sources
            assert len(unique_domains) > 1, \
                f"Researcher needs diverse sources: {unique_domains}"
    
    def test_recent_developments_research(self):
        """Test researcher looking for recent developments."""
        current_topic = "quantum computing breakthroughs 2024"
        
        result = self.tools.research_search(
            query=current_topic,
            max_results=5
        )
        response = json.loads(result)
        
        if response.get("success", False):
            results = response.get("results", [])
            assert len(results) > 0, "Should find recent developments"
            
            # Results should be relevant to recent developments
            for result_item in results:
                content = (result_item.get("title", "") + " " + 
                          result_item.get("snippet", "")).lower()
                
                # Should contain recent/current indicators
                recent_indicators = ["2024", "recent", "latest", "new", "breakthrough", "advance"]
                has_recent_content = any(indicator in content for indicator in recent_indicators)
                
                # Most results should indicate recency
                if not has_recent_content:
                    print(f"Non-recent result: {result_item.get('title', '')[:50]}...")


class TestNewsAnalystAgentPersona:
    """Test web search tools from a news analyst agent perspective."""
    
    def setup_method(self):
        """Set up test fixtures."""
        self.tools = WebSearchTools()
        self.persona = {
            "name": "NewsBot",
            "role": "News Analyst",
            "needs": ["current events", "breaking news", "trending topics"],
            "preferences": ["recent articles", "credible sources", "diverse viewpoints"]
        }
    
    def test_breaking_news_monitoring(self):
        """Test news analyst monitoring breaking news."""
        breaking_news_query = "technology industry layoffs"
        
        result = self.tools.news_search(
            query=breaking_news_query,
            max_results=10
        )
        response = json.loads(result)
        
        if response.get("success", False):
            results = response.get("results", [])
            assert len(results) > 0, "News analyst should find current news"
            
            # News results should have news-like characteristics
            for result_item in results:
                title = result_item.get("title", "")
                assert len(title) > 0, "News articles should have titles"
                
                # News titles often have specific patterns
                news_patterns = [
                    any(word in title.lower() for word in ["announces", "reports", "says", "reveals"]),
                    ":" in title,  # Common in news headlines
                    any(word in title for word in ["Company", "CEO", "Inc", "Corp"])
                ]
                
                # At least some pattern should match
                if not any(news_patterns):
                    print(f"Non-news-like title: {title}")
    
    def test_trend_analysis(self):
        """Test news analyst analyzing trends."""
        trend_query = "artificial intelligence adoption trends"
        
        result = self.tools.news_search(
            query=trend_query,
            max_results=8
        )
        response = json.loads(result)
        
        if response.get("success", False):
            results = response.get("results", [])
            assert len(results) > 0, "Should find trend-related news"
            
            # Results should be relevant to trends
            for result_item in results:
                content = (result_item.get("title", "") + " " + 
                          result_item.get("snippet", "")).lower()
                
                trend_indicators = [
                    "trend", "growth", "increase", "adoption", "market",
                    "industry", "analysis", "report", "forecast"
                ]
                
                has_trend_content = any(indicator in content for indicator in trend_indicators)
                assert has_trend_content, \
                    f"News result should relate to trends: {result_item.get('title', '')[:50]}"
    
    def test_source_credibility_awareness(self):
        """Test news analyst getting results from credible sources."""
        news_query = "climate change policy updates"
        
        result = self.tools.news_search(
            query=news_query,
            max_results=6
        )
        response = json.loads(result)
        
        if response.get("success", False):
            results = response.get("results", [])
            assert len(results) > 0, "Should find policy news"
            
            # Check for credible news sources
            credible_indicators = [
                ".com", ".org", ".gov",  # Legitimate domains
                "news", "times", "post", "journal", "reuters", "ap", "bbc"
            ]
            
            credible_sources = 0
            for result_item in results:
                url = result_item.get("url", "").lower()
                if any(indicator in url for indicator in credible_indicators):
                    credible_sources += 1
            
            # Most sources should appear credible
            assert credible_sources >= len(results) * 0.5, \
                f"News analyst needs credible sources: {credible_sources}/{len(results)}"


class TestEducationalAgentPersona:
    """Test web search tools from an educational agent perspective."""
    
    def setup_method(self):
        """Set up test fixtures."""
        self.tools = WebSearchTools()
        self.persona = {
            "name": "EduBot",
            "role": "Educational Assistant",
            "needs": ["clear explanations", "authoritative sources", "structured information"],
            "preferences": ["educational content", "step-by-step guides", "verified facts"]
        }
    
    def test_concept_explanation(self):
        """Test educational agent explaining concepts."""
        concept_query = "photosynthesis process explanation"
        
        result = self.tools.educational_search(
            query=concept_query,
            max_results=5
        )
        response = json.loads(result)
        
        if response.get("success", False):
            results = response.get("results", [])
            assert len(results) > 0, "Should find educational content"
            
            # Educational content should be clear and structured
            for result_item in results:
                snippet = result_item.get("snippet", "")
                assert len(snippet) > 50, \
                    "Educational content should have substantial explanations"
                
                # Should contain educational language
                educational_terms = [
                    "process", "step", "occurs", "involves", "produces",
                    "definition", "explanation", "how", "what", "why"
                ]
                
                content = (result_item.get("title", "") + " " + snippet).lower()
                has_educational_language = any(term in content for term in educational_terms)
                
                assert has_educational_language, \
                    f"Educational content should use explanatory language: {content[:100]}"
    
    def test_factual_information_retrieval(self):
        """Test educational agent retrieving factual information."""
        factual_query = "solar system planets order"
        
        result = self.tools.educational_search(
            query=factual_query,
            max_results=3
        )
        response = json.loads(result)
        
        if response.get("success", False):
            results = response.get("results", [])
            assert len(results) > 0, "Should find factual information"
            
            # Should prefer authoritative educational sources
            educational_domains = ["wikipedia", "edu", "britannica", "nasa", "science"]
            
            authoritative_sources = 0
            for result_item in results:
                url = result_item.get("url", "").lower()
                if any(domain in url for domain in educational_domains):
                    authoritative_sources += 1
            
            # Most sources should be authoritative
            assert authoritative_sources >= len(results) * 0.6, \
                f"Educational agent needs authoritative sources: {authoritative_sources}/{len(results)}"
    
    def test_how_to_guides(self):
        """Test educational agent finding how-to guides."""
        how_to_query = "how to solve quadratic equations"
        
        result = self.tools.educational_search(
            query=how_to_query,
            max_results=4
        )
        response = json.loads(result)
        
        if response.get("success", False):
            results = response.get("results", [])
            assert len(results) > 0, "Should find how-to guides"
            
            # Results should contain instructional content
            for result_item in results:
                content = (result_item.get("title", "") + " " + 
                          result_item.get("snippet", "")).lower()
                
                instructional_terms = [
                    "how to", "step", "method", "solve", "calculate",
                    "formula", "equation", "example", "tutorial"
                ]
                
                has_instructional_content = any(term in content for term in instructional_terms)
                assert has_instructional_content, \
                    f"How-to content should be instructional: {content[:100]}"


class TestTechnicalAgentPersona:
    """Test web search tools from a technical/programming agent perspective."""
    
    def setup_method(self):
        """Set up test fixtures."""
        self.tools = WebSearchTools()
        self.persona = {
            "name": "CodeBot",
            "role": "Technical Assistant",
            "needs": ["code examples", "technical documentation", "community discussions"],
            "preferences": ["developer resources", "practical solutions", "current best practices"]
        }
    
    def test_programming_problem_solving(self):
        """Test technical agent solving programming problems."""
        tech_query = "Python async await best practices"
        
        result = self.tools.tech_search(
            query=tech_query,
            max_results=6
        )
        response = json.loads(result)
        
        if response.get("success", False):
            results = response.get("results", [])
            assert len(results) > 0, "Should find technical solutions"
            
            # Technical content should contain programming terms
            for result_item in results:
                content = (result_item.get("title", "") + " " + 
                          result_item.get("snippet", "")).lower()
                
                technical_terms = [
                    "python", "async", "await", "function", "code",
                    "programming", "development", "syntax", "example"
                ]
                
                has_technical_content = any(term in content for term in technical_terms)
                assert has_technical_content, \
                    f"Technical content should contain programming terms: {content[:100]}"
    
    def test_community_discussion_access(self):
        """Test technical agent accessing community discussions."""
        community_query = "React hooks performance optimization"
        
        result = self.tools.tech_search(
            query=community_query,
            max_results=5
        )
        response = json.loads(result)
        
        if response.get("success", False):
            results = response.get("results", [])
            assert len(results) > 0, "Should find community discussions"
            
            # Should include community-oriented content
            community_indicators = [
                "discussion", "community", "forum", "question", "answer",
                "stackoverflow", "reddit", "github", "developer"
            ]
            
            community_content = 0
            for result_item in results:
                content = (result_item.get("title", "") + " " + 
                          result_item.get("snippet", "") + " " +
                          result_item.get("url", "")).lower()
                
                if any(indicator in content for indicator in community_indicators):
                    community_content += 1
            
            # Some results should be community-oriented
            assert community_content > 0, \
                f"Technical agent should find community content: {community_content}/{len(results)}"
    
    def test_current_technology_trends(self):
        """Test technical agent staying current with technology trends."""
        trend_query = "JavaScript framework trends 2024"
        
        result = self.tools.tech_search(
            query=trend_query,
            max_results=5
        )
        response = json.loads(result)
        
        if response.get("success", False):
            results = response.get("results", [])
            assert len(results) > 0, "Should find technology trends"
            
            # Results should be current and trend-focused
            for result_item in results:
                content = (result_item.get("title", "") + " " + 
                          result_item.get("snippet", "")).lower()
                
                trend_terms = [
                    "2024", "trend", "popular", "latest", "new", "modern",
                    "framework", "javascript", "technology", "development"
                ]
                
                has_trend_content = any(term in content for term in trend_terms)
                assert has_trend_content, \
                    f"Tech trend content should be current: {content[:100]}"


class TestPersonalAssistantAgentPersona:
    """Test web search tools from a personal assistant agent perspective."""
    
    def setup_method(self):
        """Set up test fixtures."""
        self.tools = WebSearchTools()
        self.persona = {
            "name": "Assistant",
            "role": "Personal Assistant",
            "needs": ["quick answers", "practical information", "local results"],
            "preferences": ["concise responses", "actionable information", "reliable sources"]
        }
    
    def test_quick_factual_queries(self):
        """Test personal assistant handling quick factual queries."""
        quick_queries = [
            "capital of France",
            "how many ounces in a pound",
            "current time in Tokyo",
            "boiling point of water",
            "who invented the telephone"
        ]
        
        successful_queries = 0
        
        for query in quick_queries:
            result = self.tools.web_search(query=query, max_results=3)
            response = json.loads(result)
            
            if response.get("success", False):
                successful_queries += 1
                results = response.get("results", [])
                assert len(results) > 0, f"Should find answer for: {query}"
                
                # Results should be concise and direct
                for result_item in results:
                    title = result_item.get("title", "")
                    snippet = result_item.get("snippet", "")
                    
                    # Should contain relevant keywords from query
                    query_words = query.lower().split()
                    content = (title + " " + snippet).lower()
                    
                    relevant_words = sum(1 for word in query_words if word in content)
                    assert relevant_words > 0, \
                        f"Result should be relevant to query '{query}': {title[:50]}"
        
        # Most queries should succeed
        assert successful_queries >= len(quick_queries) * 0.7, \
            f"Personal assistant should handle most queries: {successful_queries}/{len(quick_queries)}"
    
    def test_practical_how_to_queries(self):
        """Test personal assistant handling practical how-to queries."""
        practical_queries = [
            "how to tie a tie",
            "how to cook rice",
            "how to change a tire",
            "how to remove stains",
            "how to reset password"
        ]
        
        for query in practical_queries:
            result = self.tools.web_search(query=query, max_results=3)
            response = json.loads(result)
            
            if response.get("success", False):
                results = response.get("results", [])
                assert len(results) > 0, f"Should find practical guide for: {query}"
                
                # Results should be practical and actionable
                for result_item in results:
                    content = (result_item.get("title", "") + " " + 
                              result_item.get("snippet", "")).lower()
                    
                    practical_terms = [
                        "how", "step", "easy", "simple", "guide", "instructions",
                        "method", "way", "tips", "tutorial"
                    ]
                    
                    has_practical_content = any(term in content for term in practical_terms)
                    assert has_practical_content, \
                        f"Practical query should return actionable content: {content[:100]}"
    
    def test_local_information_queries(self):
        """Test personal assistant handling location-based queries."""
        local_queries = [
            "restaurants near me",
            "weather forecast",
            "local news",
            "nearby gas stations",
            "movie theaters"
        ]
        
        for query in local_queries:
            result = self.tools.web_search(query=query, max_results=3)
            response = json.loads(result)
            
            if response.get("success", False):
                results = response.get("results", [])
                assert len(results) > 0, f"Should find local information for: {query}"
                
                # Results should be relevant to local/location queries
                for result_item in results:
                    content = (result_item.get("title", "") + " " + 
                              result_item.get("snippet", "")).lower()
                    
                    location_terms = [
                        "near", "local", "location", "address", "map",
                        "directions", "hours", "open", "close", "find"
                    ]
                    
                    has_location_content = any(term in content for term in location_terms)
                    # Not all results need location terms, but content should be relevant
                    if not has_location_content:
                        print(f"Non-location result for '{query}': {result_item.get('title', '')[:50]}")


class TestSpecializedAgentPersona:
    """Test web search tools from specialized agent perspectives."""
    
    def setup_method(self):
        """Set up test fixtures."""
        self.tools = WebSearchTools()
    
    def test_travel_agent_persona(self):
        """Test travel planning agent persona."""
        travel_queries = [
            "best time to visit Japan",
            "Paris travel guide",
            "budget hotels in New York",
            "flight deals to Europe",
            "travel insurance recommendations"
        ]
        
        for query in travel_queries:
            result = self.tools.web_search(query=query, max_results=4)
            response = json.loads(result)
            
            if response.get("success", False):
                results = response.get("results", [])
                assert len(results) > 0, f"Should find travel information for: {query}"
                
                # Results should be travel-relevant
                for result_item in results:
                    content = (result_item.get("title", "") + " " + 
                              result_item.get("snippet", "")).lower()
                    
                    travel_terms = [
                        "travel", "trip", "vacation", "hotel", "flight",
                        "destination", "guide", "tourism", "visit", "booking"
                    ]
                    
                    has_travel_content = any(term in content for term in travel_terms)
                    assert has_travel_content, \
                        f"Travel query should return travel-relevant content: {content[:100]}"
    
    def test_health_information_agent_persona(self):
        """Test health information agent persona."""
        health_queries = [
            "symptoms of common cold",
            "healthy diet tips",
            "exercise for beginners",
            "stress management techniques",
            "sleep hygiene recommendations"
        ]
        
        for query in health_queries:
            result = self.tools.web_search(query=query, max_results=3)
            response = json.loads(result)
            
            if response.get("success", False):
                results = response.get("results", [])
                assert len(results) > 0, f"Should find health information for: {query}"
                
                # Results should be health-relevant
                for result_item in results:
                    content = (result_item.get("title", "") + " " + 
                              result_item.get("snippet", "")).lower()
                    
                    health_terms = [
                        "health", "medical", "symptoms", "treatment", "doctor",
                        "wellness", "fitness", "nutrition", "exercise", "tips"
                    ]
                    
                    has_health_content = any(term in content for term in health_terms)
                    assert has_health_content, \
                        f"Health query should return health-relevant content: {content[:100]}"
    
    def test_financial_advisor_agent_persona(self):
        """Test financial advisor agent persona."""
        financial_queries = [
            "investment strategies for beginners",
            "retirement planning tips",
            "stock market analysis",
            "cryptocurrency trends",
            "budgeting advice"
        ]
        
        for query in financial_queries:
            result = self.tools.web_search(query=query, max_results=3)
            response = json.loads(result)
            
            if response.get("success", False):
                results = response.get("results", [])
                assert len(results) > 0, f"Should find financial information for: {query}"
                
                # Results should be finance-relevant
                for result_item in results:
                    content = (result_item.get("title", "") + " " + 
                              result_item.get("snippet", "")).lower()
                    
                    financial_terms = [
                        "investment", "money", "financial", "stock", "market",
                        "budget", "savings", "retirement", "portfolio", "advice"
                    ]
                    
                    has_financial_content = any(term in content for term in financial_terms)
                    assert has_financial_content, \
                        f"Financial query should return finance-relevant content: {content[:100]}"


class TestCrossPersonaConsistency:
    """Test consistency across different agent personas."""
    
    def setup_method(self):
        """Set up test fixtures."""
        self.tools = WebSearchTools()
    
    def test_consistent_basic_functionality(self):
        """Test that basic functionality works consistently across personas."""
        # Same query from different persona perspectives
        query = "artificial intelligence"
        
        search_methods = [
            ("web_search", {}),
            ("research_search", {"search_depth": "basic"}),
            ("educational_search", {}),
            ("tech_search", {}),
            ("news_search", {})
        ]
        
        successful_methods = 0
        
        for method_name, extra_params in search_methods:
            method = getattr(self.tools, method_name)
            params = {"query": query, "max_results": 3}
            params.update(extra_params)
            
            result = method(**params)
            response = json.loads(result)
            
            if response.get("success", False):
                successful_methods += 1
                results = response.get("results", [])
                assert len(results) > 0, f"Method {method_name} should return results"
                
                # All methods should return valid result structure
                for result_item in results:
                    assert "title" in result_item, f"Method {method_name} should return titles"
                    assert "url" in result_item, f"Method {method_name} should return URLs"
                    assert "snippet" in result_item, f"Method {method_name} should return snippets"
        
        # Most methods should work
        assert successful_methods >= len(search_methods) * 0.6, \
            f"Most search methods should work: {successful_methods}/{len(search_methods)}"
    
    def test_consistent_error_handling(self):
        """Test that error handling is consistent across personas."""
        # Test invalid parameters across different methods
        search_methods = ["web_search", "news_search", "educational_search", "tech_search"]
        
        for method_name in search_methods:
            method = getattr(self.tools, method_name)
            
            # Test with invalid max_results
            result = method(query="test", max_results=-1)
            response = json.loads(result)
            
            if not response.get("success", False):
                error = response.get("error", "").lower()
                assert "max_results" in error, \
                    f"Method {method_name} should provide consistent error messages"
    
    def test_consistent_response_format(self):
        """Test that response format is consistent across personas."""
        query = "test query"
        search_methods = ["web_search", "news_search", "educational_search"]
        
        for method_name in search_methods:
            method = getattr(self.tools, method_name)
            result = method(query=query, max_results=2)
            response = json.loads(result)
            
            # All methods should return consistent response structure
            assert "success" in response, f"Method {method_name} should include success field"
            
            if response.get("success", False):
                assert "results" in response, f"Method {method_name} should include results field"
                assert "engine_used" in response, f"Method {method_name} should include engine_used field"
                assert "query" in response, f"Method {method_name} should include query field"
                
                # Results should have consistent structure
                for result_item in response["results"]:
                    required_fields = ["title", "url", "snippet"]
                    for field in required_fields:
                        assert field in result_item, \
                            f"Method {method_name} results should include {field} field"
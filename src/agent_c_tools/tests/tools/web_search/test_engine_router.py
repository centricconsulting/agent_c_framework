"""
Unit tests for engine routing logic in the web search system.
"""
import pytest
from unittest.mock import Mock, patch
from typing import List, Dict, Any

from base.router import EngineRouter, QueryAnalyzer
from base.registry import EngineRegistry
from base.models import SearchParameters, SearchType, SafeSearchLevel, SearchDepth
from base.engine import EngineUnavailableException


class TestQueryAnalyzer:
    """Test suite for QueryAnalyzer class."""
    
    def setup_method(self):
        """Set up test fixtures before each test method."""
        self.analyzer = QueryAnalyzer()
    
    def test_analyzer_initialization(self):
        """Test QueryAnalyzer initialization."""
        analyzer = QueryAnalyzer()
        assert analyzer is not None
        assert hasattr(analyzer, '_compile_patterns')
    
    def test_analyze_general_query(self):
        """Test analysis of general web queries."""
        query = "python programming tutorial"
        result = self.analyzer.analyze_query(query)
        
        assert isinstance(result, dict)
        assert "detected_categories" in result
        assert "preferred_engines" in result
        assert "confidence_scores" in result
        assert "suggested_search_type" in result
        
        assert "general" in result["detected_categories"]
        assert result["suggested_search_type"] == SearchType.WEB
    
    def test_analyze_news_query(self):
        """Test analysis of news-related queries."""
        query = "latest news about climate change"
        result = self.analyzer.analyze_query(query)
        
        assert "news" in result["detected_categories"]
        assert result["suggested_search_type"] == SearchType.NEWS
        assert "news_api" in result["preferred_engines"]
    
    def test_analyze_tech_query(self):
        """Test analysis of technology queries."""
        query = "hacker news python frameworks"
        result = self.analyzer.analyze_query(query)
        
        assert "tech" in result["detected_categories"]
        assert result["suggested_search_type"] == SearchType.TECH
        assert "hacker_news" in result["preferred_engines"]
    
    def test_analyze_educational_query(self):
        """Test analysis of educational queries."""
        query = "wikipedia history of artificial intelligence"
        result = self.analyzer.analyze_query(query)
        
        assert "educational" in result["detected_categories"]
        assert result["suggested_search_type"] == SearchType.EDUCATIONAL
        assert "wikipedia" in result["preferred_engines"]
    
    def test_analyze_research_query(self):
        """Test analysis of research queries."""
        query = "comprehensive analysis of machine learning algorithms"
        result = self.analyzer.analyze_query(query)
        
        assert "research" in result["detected_categories"]
        assert result["suggested_search_type"] == SearchType.RESEARCH
        assert "tavily" in result["preferred_engines"]
    
    def test_analyze_flight_query(self):
        """Test analysis of flight queries."""
        query = "flights from New York to Los Angeles"
        result = self.analyzer.analyze_query(query)
        
        assert "flights" in result["detected_categories"]
        assert result["suggested_search_type"] == SearchType.FLIGHTS
        assert "google_serp" in result["preferred_engines"]
    
    def test_analyze_events_query(self):
        """Test analysis of events queries."""
        query = "events in San Francisco this weekend"
        result = self.analyzer.analyze_query(query)
        
        assert "events" in result["detected_categories"]
        assert result["suggested_search_type"] == SearchType.EVENTS
        assert "google_serp" in result["preferred_engines"]
    
    def test_analyze_empty_query(self):
        """Test analysis of empty or invalid queries."""
        result = self.analyzer.analyze_query("")
        
        assert result["detected_categories"] == ["general"]
        assert result["suggested_search_type"] == SearchType.WEB
        assert "duckduckgo" in result["preferred_engines"]
    
    def test_analyze_mixed_query(self):
        """Test analysis of queries with mixed categories."""
        query = "latest tech news about AI research"
        result = self.analyzer.analyze_query(query)
        
        # Should detect multiple categories
        categories = result["detected_categories"]
        assert len(categories) > 1
        assert any(cat in ["news", "tech", "research"] for cat in categories)
    
    def test_confidence_scores(self):
        """Test confidence scoring for query analysis."""
        query = "breaking news technology"
        result = self.analyzer.analyze_query(query)
        
        scores = result["confidence_scores"]
        assert isinstance(scores, dict)
        assert all(0.0 <= score <= 1.0 for score in scores.values())
        
        # News should have high confidence for this query
        assert scores.get("news", 0) > 0.5


class TestEngineRouter:
    """Test suite for EngineRouter class."""
    
    def setup_method(self):
        """Set up test fixtures before each test method."""
        self.mock_registry = Mock(spec=EngineRegistry)
        self.mock_registry.get_available_engines.return_value = [
            "duckduckgo", "google_serp", "tavily", "wikipedia", "news_api", "hacker_news"
        ]
        self.mock_registry.is_engine_available.return_value = True
        
        self.router = EngineRouter(self.mock_registry)
    
    def test_router_initialization(self):
        """Test EngineRouter initialization."""
        router = EngineRouter(self.mock_registry)
        assert router is not None
        assert router.registry == self.mock_registry
    
    def test_route_explicit_engine(self):
        """Test routing with explicit engine specification."""
        params = SearchParameters(
            query="test query",
            engine="duckduckgo",
            search_type=SearchType.WEB
        )
        
        result = self.router.route_search_request(params)
        assert result == "duckduckgo"
    
    def test_route_auto_engine_web_search(self):
        """Test automatic routing for web searches."""
        params = SearchParameters(
            query="python programming",
            engine="auto",
            search_type=SearchType.WEB
        )
        
        result = self.router.route_search_request(params)
        assert result in ["duckduckgo", "google_serp", "tavily"]
    
    def test_route_auto_engine_news_search(self):
        """Test automatic routing for news searches."""
        params = SearchParameters(
            query="latest news",
            engine="auto",
            search_type=SearchType.NEWS
        )
        
        result = self.router.route_search_request(params)
        assert result in ["news_api", "google_serp"]
    
    def test_route_auto_engine_educational_search(self):
        """Test automatic routing for educational searches."""
        params = SearchParameters(
            query="history of computers",
            engine="auto",
            search_type=SearchType.EDUCATIONAL
        )
        
        result = self.router.route_search_request(params)
        assert result == "wikipedia"
    
    def test_route_auto_engine_research_search(self):
        """Test automatic routing for research searches."""
        params = SearchParameters(
            query="comprehensive analysis",
            engine="auto",
            search_type=SearchType.RESEARCH
        )
        
        result = self.router.route_search_request(params)
        assert result == "tavily"
    
    def test_route_auto_engine_tech_search(self):
        """Test automatic routing for tech searches."""
        params = SearchParameters(
            query="programming discussion",
            engine="auto",
            search_type=SearchType.TECH
        )
        
        result = self.router.route_search_request(params)
        assert result == "hacker_news"
    
    def test_route_auto_engine_flights_search(self):
        """Test automatic routing for flight searches."""
        params = SearchParameters(
            query="flights to paris",
            engine="auto",
            search_type=SearchType.FLIGHTS
        )
        
        result = self.router.route_search_request(params)
        assert result == "google_serp"
    
    def test_route_auto_engine_events_search(self):
        """Test automatic routing for events searches."""
        params = SearchParameters(
            query="events near me",
            engine="auto",
            search_type=SearchType.EVENTS
        )
        
        result = self.router.route_search_request(params)
        assert result == "google_serp"
    
    def test_route_with_limited_engines(self):
        """Test routing with limited available engines."""
        available_engines = ["duckduckgo", "wikipedia"]
        
        params = SearchParameters(
            query="test query",
            engine="auto",
            search_type=SearchType.WEB
        )
        
        result = self.router.route_search_request(params, available_engines)
        assert result in available_engines
    
    def test_route_unavailable_engine(self):
        """Test routing when specified engine is unavailable."""
        self.mock_registry.is_engine_available.return_value = False
        
        params = SearchParameters(
            query="test query",
            engine="duckduckgo",
            search_type=SearchType.WEB
        )
        
        with pytest.raises(EngineUnavailableException):
            self.router.route_search_request(params)
    
    def test_route_no_available_engines(self):
        """Test routing when no engines are available."""
        self.mock_registry.get_available_engines.return_value = []
        
        params = SearchParameters(
            query="test query",
            engine="auto",
            search_type=SearchType.WEB
        )
        
        with pytest.raises(EngineUnavailableException):
            self.router.route_search_request(params)
    
    @patch('base.router.QueryAnalyzer')
    def test_query_analysis_routing(self, mock_analyzer_class):
        """Test routing based on query analysis."""
        mock_analyzer = Mock()
        mock_analyzer.analyze_query.return_value = {
            "detected_categories": ["news"],
            "preferred_engines": ["news_api"],
            "confidence_scores": {"news": 0.9},
            "suggested_search_type": SearchType.NEWS
        }
        mock_analyzer_class.return_value = mock_analyzer
        
        router = EngineRouter(self.mock_registry)
        
        params = SearchParameters(
            query="breaking news",
            engine="auto",
            search_type=SearchType.WEB
        )
        
        result = router.route_search_request(params)
        assert result == "news_api"
    
    def test_routing_cache(self):
        """Test routing decision caching."""
        params = SearchParameters(
            query="test query",
            engine="auto",
            search_type=SearchType.WEB
        )
        
        # First call should route and cache
        result1 = self.router.route_search_request(params)
        
        # Second call should use cache
        result2 = self.router.route_search_request(params)
        
        assert result1 == result2
    
    def test_get_routing_recommendations(self):
        """Test getting routing recommendations."""
        params = SearchParameters(
            query="test query",
            engine="auto",
            search_type=SearchType.WEB
        )
        
        recommendations = self.router.get_routing_recommendations(params)
        
        assert isinstance(recommendations, dict)
        assert "recommended_engine" in recommendations
        assert "confidence" in recommendations
        assert "reasoning" in recommendations
        assert "alternatives" in recommendations
    
    def test_clear_cache(self):
        """Test clearing routing cache."""
        params = SearchParameters(
            query="test query",
            engine="auto",
            search_type=SearchType.WEB
        )
        
        # Route to populate cache
        self.router.route_search_request(params)
        
        # Clear cache
        self.router.clear_cache()
        
        # Cache should be empty
        stats = self.router.get_cache_stats()
        assert stats["cache_size"] == 0
    
    def test_cache_stats(self):
        """Test getting cache statistics."""
        params = SearchParameters(
            query="test query",
            engine="auto",
            search_type=SearchType.WEB
        )
        
        # Route to populate cache
        self.router.route_search_request(params)
        
        stats = self.router.get_cache_stats()
        
        assert isinstance(stats, dict)
        assert "cache_size" in stats
        assert "cache_hits" in stats
        assert "cache_misses" in stats
        assert stats["cache_size"] > 0
    
    def test_fallback_engine_selection(self):
        """Test fallback engine selection when preferred engines are unavailable."""
        # Make preferred engines unavailable
        def mock_is_available(engine):
            return engine not in ["news_api", "tavily"]
        
        self.mock_registry.is_engine_available.side_effect = mock_is_available
        
        params = SearchParameters(
            query="news about technology",
            engine="auto",
            search_type=SearchType.NEWS
        )
        
        result = self.router.route_search_request(params)
        
        # Should fall back to available engine
        assert result in ["duckduckgo", "google_serp", "wikipedia", "hacker_news"]
    
    def test_engine_capabilities_consideration(self):
        """Test that routing considers engine capabilities."""
        # Mock engine capabilities
        def mock_supports_search_type(engine, search_type):
            if search_type == SearchType.FLIGHTS:
                return engine == "google_serp"
            return True
        
        self.mock_registry.supports_search_type = mock_supports_search_type
        
        params = SearchParameters(
            query="flights to paris",
            engine="auto",
            search_type=SearchType.FLIGHTS
        )
        
        result = self.router.route_search_request(params)
        assert result == "google_serp"
    
    def test_routing_with_domain_preferences(self):
        """Test routing with domain include/exclude preferences."""
        params = SearchParameters(
            query="test query",
            engine="auto",
            search_type=SearchType.WEB,
            include_domains=["wikipedia.org"]
        )
        
        result = self.router.route_search_request(params)
        
        # Should prefer Wikipedia for Wikipedia domain searches
        assert result == "wikipedia"
    
    def test_routing_performance_optimization(self):
        """Test that routing considers performance characteristics."""
        params = SearchParameters(
            query="quick search",
            engine="auto",
            search_type=SearchType.WEB,
            max_results=5
        )
        
        result = self.router.route_search_request(params)
        
        # Should prefer fast engines for small result sets
        assert result in ["duckduckgo", "google_serp"]
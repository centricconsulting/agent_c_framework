"""
Unit tests for engine adapters in the web search system.
"""
import pytest
import json
from unittest.mock import Mock, patch, MagicMock
from agent_c_tools.tools.web_search.base.models import SearchParameters, SearchResponse, SearchType, SafeSearchLevel
from agent_c_tools.tools.web_search.base.engine import BaseWebSearchEngine, EngineException
from agent_c_tools.tools.web_search.engines.duckduckgo_engine import DuckDuckGoEngine, create_duckduckgo_engine
from agent_c_tools.tools.web_search.engines.google_serp_engine import GoogleSerpEngine, create_google_serp_engine
from agent_c_tools.tools.web_search.engines.tavily_engine import TavilyEngine, create_tavily_engine
from agent_c_tools.tools.web_search.engines.wikipedia_engine import WikipediaEngine, create_wikipedia_engine
from agent_c_tools.tools.web_search.engines.news_api_engine import NewsApiEngine, create_news_api_engine
from agent_c_tools.tools.web_search.engines.hacker_news_engine import HackerNewsEngine, create_hacker_news_engine


class TestDuckDuckGoEngine:
    """Test suite for DuckDuckGoEngine adapter."""
    
    def setup_method(self):
        """Set up test fixtures before each test method."""
        self.engine = create_duckduckgo_engine()
    
    def test_engine_initialization(self):
        """Test DuckDuckGoEngine initialization."""
        assert self.engine is not None
        assert isinstance(self.engine, DuckDuckGoEngine)
        assert self.engine.name == "duckduckgo"
        assert not self.engine.capabilities.requires_api_key
    
    def test_engine_capabilities(self):
        """Test DuckDuckGoEngine capabilities."""
        capabilities = self.engine.capabilities
        
        assert SearchType.WEB in capabilities.supported_search_types
        assert capabilities.max_results == 50
        assert capabilities.supports_safe_search
        assert not capabilities.requires_api_key
    
    @patch('engines.duckduckgo_engine.DuckDuckGoTools')
    def test_execute_search_success(self, mock_tools_class):
        """Test successful search execution."""
        # Mock the legacy tool
        mock_tool = Mock()
        mock_tool.web_search.return_value = json.dumps({
            "results": [
                {
                    "title": "Test Result",
                    "url": "https://example.com",
                    "snippet": "Test snippet",
                    "published_date": "2024-01-01"
                }
            ]
        })
        mock_tools_class.return_value = mock_tool
        
        params = SearchParameters(
            query="test query",
            engine="duckduckgo",
            search_type=SearchType.WEB,
            max_results=10
        )
        
        response = self.engine.execute_search(params)
        
        assert isinstance(response, SearchResponse)
        assert len(response.results) == 1
        assert response.results[0].title == "Test Result"
        assert response.engine_used == "duckduckgo"
        assert response.query == "test query"
    
    @patch('engines.duckduckgo_engine.DuckDuckGoTools')
    def test_execute_search_with_safe_search(self, mock_tools_class):
        """Test search execution with safe search enabled."""
        mock_tool = Mock()
        mock_tool.web_search.return_value = json.dumps({"results": []})
        mock_tools_class.return_value = mock_tool
        
        params = SearchParameters(
            query="test query",
            engine="duckduckgo",
            search_type=SearchType.WEB,
            safe_search=SafeSearchLevel.STRICT
        )
        
        self.engine.execute_search(params)
        
        # Verify safe search was passed to legacy tool
        mock_tool.web_search.assert_called_once()
        call_args = mock_tool.web_search.call_args[1]
        assert call_args["safe_search"] == "strict"
    
    @patch('engines.duckduckgo_engine.DuckDuckGoTools')
    def test_execute_search_error_handling(self, mock_tools_class):
        """Test error handling in search execution."""
        mock_tool = Mock()
        mock_tool.web_search.side_effect = Exception("Search failed")
        mock_tools_class.return_value = mock_tool
        
        params = SearchParameters(
            query="test query",
            engine="duckduckgo",
            search_type=SearchType.WEB
        )
        
        with pytest.raises(EngineException) as exc_info:
            self.engine.execute_search(params)
        
        assert "Search failed" in str(exc_info.value)
    
    def test_availability_check(self):
        """Test engine availability check."""
        # DuckDuckGo should generally be available (no API key required)
        assert self.engine.is_available()
    
    def test_supports_search_type(self):
        """Test search type support checking."""
        assert self.engine.supports_search_type(SearchType.WEB)
        assert not self.engine.supports_search_type(SearchType.FLIGHTS)
        assert not self.engine.supports_search_type(SearchType.EVENTS)
    
    def test_get_health_status(self):
        """Test getting engine health status."""
        status = self.engine.get_health_status()
        
        assert status.is_available
        assert status.last_check is not None
        assert isinstance(status.response_time, float)
        assert isinstance(status.error_rate, float)


class TestGoogleSerpEngine:
    """Test suite for GoogleSerpEngine adapter."""
    
    def setup_method(self):
        """Set up test fixtures before each test method."""
        self.engine = create_google_serp_engine()
    
    def test_engine_initialization(self):
        """Test GoogleSerpEngine initialization."""
        assert self.engine is not None
        assert isinstance(self.engine, GoogleSerpEngine)
        assert self.engine.name == "google_serp"
        assert self.engine.capabilities.requires_api_key
    
    def test_engine_capabilities(self):
        """Test GoogleSerpEngine capabilities."""
        capabilities = self.engine.capabilities
        
        assert SearchType.WEB in capabilities.supported_search_types
        assert SearchType.NEWS in capabilities.supported_search_types
        assert SearchType.FLIGHTS in capabilities.supported_search_types
        assert SearchType.EVENTS in capabilities.supported_search_types
        assert capabilities.max_results == 100
        assert capabilities.requires_api_key
    
    @patch('engines.google_serp_engine.GoogleSerpTools')
    def test_execute_web_search(self, mock_tools_class):
        """Test web search execution."""
        mock_tool = Mock()
        mock_tool.web_search.return_value = json.dumps({
            "organic_results": [
                {
                    "title": "Google Result",
                    "link": "https://google.com/test",
                    "snippet": "Google test result",
                    "date": "2024-01-01"
                }
            ]
        })
        mock_tools_class.return_value = mock_tool
        
        params = SearchParameters(
            query="test query",
            engine="google_serp",
            search_type=SearchType.WEB
        )
        
        response = self.engine.execute_search(params)
        
        assert isinstance(response, SearchResponse)
        assert len(response.results) == 1
        assert response.results[0].title == "Google Result"
        assert response.engine_used == "google_serp"
    
    @patch('engines.google_serp_engine.GoogleSerpTools')
    def test_execute_news_search(self, mock_tools_class):
        """Test news search execution."""
        mock_tool = Mock()
        mock_tool.news_search.return_value = json.dumps({
            "news_results": [
                {
                    "title": "News Article",
                    "link": "https://news.com/article",
                    "snippet": "News content",
                    "date": "2024-01-01"
                }
            ]
        })
        mock_tools_class.return_value = mock_tool
        
        params = SearchParameters(
            query="latest news",
            engine="google_serp",
            search_type=SearchType.NEWS
        )
        
        response = self.engine.execute_search(params)
        
        assert isinstance(response, SearchResponse)
        assert response.search_type == SearchType.NEWS
        mock_tool.news_search.assert_called_once()
    
    @patch('engines.google_serp_engine.GoogleSerpTools')
    def test_execute_flights_search(self, mock_tools_class):
        """Test flights search execution."""
        mock_tool = Mock()
        mock_tool.flights_search.return_value = json.dumps({
            "flights": [
                {
                    "departure_airport": "JFK",
                    "arrival_airport": "LAX",
                    "price": "$299",
                    "duration": "5h 30m"
                }
            ]
        })
        mock_tools_class.return_value = mock_tool
        
        params = SearchParameters(
            query="flights JFK to LAX",
            engine="google_serp",
            search_type=SearchType.FLIGHTS
        )
        
        response = self.engine.execute_search(params)
        
        assert isinstance(response, SearchResponse)
        assert response.search_type == SearchType.FLIGHTS
        mock_tool.flights_search.assert_called_once()
    
    @patch('engines.google_serp_engine.GoogleSerpTools')
    def test_execute_events_search(self, mock_tools_class):
        """Test events search execution."""
        mock_tool = Mock()
        mock_tool.events_search.return_value = json.dumps({
            "events_results": [
                {
                    "title": "Concert Event",
                    "date": "2024-06-15",
                    "venue": "Madison Square Garden",
                    "link": "https://events.com/concert"
                }
            ]
        })
        mock_tools_class.return_value = mock_tool
        
        params = SearchParameters(
            query="concerts in NYC",
            engine="google_serp",
            search_type=SearchType.EVENTS
        )
        
        response = self.engine.execute_search(params)
        
        assert isinstance(response, SearchResponse)
        assert response.search_type == SearchType.EVENTS
        mock_tool.events_search.assert_called_once()
    
    @patch.dict('os.environ', {'SERPAPI_API_KEY': 'test_key'})
    def test_availability_with_api_key(self):
        """Test availability check with API key present."""
        assert self.engine.is_available()
    
    @patch.dict('os.environ', {}, clear=True)
    def test_availability_without_api_key(self):
        """Test availability check without API key."""
        assert not self.engine.is_available()


class TestTavilyEngine:
    """Test suite for TavilyEngine adapter."""
    
    def setup_method(self):
        """Set up test fixtures before each test method."""
        self.engine = create_tavily_engine()
    
    def test_engine_initialization(self):
        """Test TavilyEngine initialization."""
        assert self.engine is not None
        assert isinstance(self.engine, TavilyEngine)
        assert self.engine.name == "tavily"
        assert self.engine.capabilities.requires_api_key
    
    def test_engine_capabilities(self):
        """Test TavilyEngine capabilities."""
        capabilities = self.engine.capabilities
        
        assert SearchType.WEB in capabilities.supported_search_types
        assert SearchType.RESEARCH in capabilities.supported_search_types
        assert capabilities.max_results == 20
        assert capabilities.supports_domain_filtering
        assert capabilities.requires_api_key
    
    @patch('engines.tavily_engine.TavilyResearchTools')
    def test_execute_research_search(self, mock_tools_class):
        """Test research search execution."""
        mock_tool = Mock()
        mock_tool.research_search.return_value = json.dumps({
            "results": [
                {
                    "title": "Research Result",
                    "url": "https://research.com/article",
                    "content": "Detailed research content",
                    "published_date": "2024-01-01"
                }
            ]
        })
        mock_tools_class.return_value = mock_tool
        
        params = SearchParameters(
            query="AI research",
            engine="tavily",
            search_type=SearchType.RESEARCH
        )
        
        response = self.engine.execute_search(params)
        
        assert isinstance(response, SearchResponse)
        assert response.search_type == SearchType.RESEARCH
        assert response.engine_used == "tavily"
        mock_tool.research_search.assert_called_once()
    
    @patch('engines.tavily_engine.TavilyResearchTools')
    def test_execute_search_with_domain_filtering(self, mock_tools_class):
        """Test search with domain filtering."""
        mock_tool = Mock()
        mock_tool.research_search.return_value = json.dumps({"results": []})
        mock_tools_class.return_value = mock_tool
        
        params = SearchParameters(
            query="test query",
            engine="tavily",
            search_type=SearchType.RESEARCH,
            include_domains=["example.com"],
            exclude_domains=["spam.com"]
        )
        
        self.engine.execute_search(params)
        
        # Verify domain filtering was passed to legacy tool
        mock_tool.research_search.assert_called_once()
        call_args = mock_tool.research_search.call_args[1]
        assert call_args["include_domains"] == ["example.com"]
        assert call_args["exclude_domains"] == ["spam.com"]


class TestWikipediaEngine:
    """Test suite for WikipediaEngine adapter."""
    
    def setup_method(self):
        """Set up test fixtures before each test method."""
        self.engine = create_wikipedia_engine()
    
    def test_engine_initialization(self):
        """Test WikipediaEngine initialization."""
        assert self.engine is not None
        assert isinstance(self.engine, WikipediaEngine)
        assert self.engine.name == "wikipedia"
        assert not self.engine.capabilities.requires_api_key
    
    def test_engine_capabilities(self):
        """Test WikipediaEngine capabilities."""
        capabilities = self.engine.capabilities
        
        assert SearchType.EDUCATIONAL in capabilities.supported_search_types
        assert SearchType.WEB in capabilities.supported_search_types
        assert capabilities.max_results == 50
        assert not capabilities.requires_api_key
    
    @patch('engines.wikipedia_engine.WikipediaTools')
    def test_execute_educational_search(self, mock_tools_class):
        """Test educational search execution."""
        mock_tool = Mock()
        mock_tool.search.return_value = json.dumps({
            "results": [
                {
                    "title": "Wikipedia Article",
                    "url": "https://en.wikipedia.org/wiki/Test",
                    "snippet": "Wikipedia content",
                    "published_date": "2024-01-01"
                }
            ]
        })
        mock_tools_class.return_value = mock_tool
        
        params = SearchParameters(
            query="history of computers",
            engine="wikipedia",
            search_type=SearchType.EDUCATIONAL
        )
        
        response = self.engine.execute_search(params)
        
        assert isinstance(response, SearchResponse)
        assert response.search_type == SearchType.EDUCATIONAL
        assert response.engine_used == "wikipedia"
        mock_tool.search.assert_called_once()
    
    def test_availability_check(self):
        """Test Wikipedia availability check."""
        # Wikipedia should generally be available (no API key required)
        assert self.engine.is_available()


class TestNewsApiEngine:
    """Test suite for NewsApiEngine adapter."""
    
    def setup_method(self):
        """Set up test fixtures before each test method."""
        self.engine = create_news_api_engine()
    
    def test_engine_initialization(self):
        """Test NewsApiEngine initialization."""
        assert self.engine is not None
        assert isinstance(self.engine, NewsApiEngine)
        assert self.engine.name == "news_api"
        assert self.engine.capabilities.requires_api_key
    
    def test_engine_capabilities(self):
        """Test NewsApiEngine capabilities."""
        capabilities = self.engine.capabilities
        
        assert SearchType.NEWS in capabilities.supported_search_types
        assert capabilities.max_results == 100
        assert capabilities.supports_date_filtering
        assert capabilities.requires_api_key
    
    @patch('engines.news_api_engine.NewsApiTools')
    def test_execute_news_search(self, mock_tools_class):
        """Test news search execution."""
        mock_tool = Mock()
        mock_tool.search_everything.return_value = json.dumps({
            "articles": [
                {
                    "title": "News Article",
                    "url": "https://news.com/article",
                    "description": "News description",
                    "publishedAt": "2024-01-01T12:00:00Z"
                }
            ]
        })
        mock_tools_class.return_value = mock_tool
        
        params = SearchParameters(
            query="breaking news",
            engine="news_api",
            search_type=SearchType.NEWS
        )
        
        response = self.engine.execute_search(params)
        
        assert isinstance(response, SearchResponse)
        assert response.search_type == SearchType.NEWS
        assert response.engine_used == "news_api"
        mock_tool.search_everything.assert_called_once()
    
    @patch.dict('os.environ', {'NEWSAPI_API_KEY': 'test_key'})
    def test_availability_with_api_key(self):
        """Test availability check with API key present."""
        assert self.engine.is_available()
    
    @patch.dict('os.environ', {}, clear=True)
    def test_availability_without_api_key(self):
        """Test availability check without API key."""
        assert not self.engine.is_available()


class TestHackerNewsEngine:
    """Test suite for HackerNewsEngine adapter."""
    
    def setup_method(self):
        """Set up test fixtures before each test method."""
        self.engine = create_hacker_news_engine()
    
    def test_engine_initialization(self):
        """Test HackerNewsEngine initialization."""
        assert self.engine is not None
        assert isinstance(self.engine, HackerNewsEngine)
        assert self.engine.name == "hacker_news"
        assert not self.engine.capabilities.requires_api_key
    
    def test_engine_capabilities(self):
        """Test HackerNewsEngine capabilities."""
        capabilities = self.engine.capabilities
        
        assert SearchType.TECH in capabilities.supported_search_types
        assert capabilities.max_results == 50
        assert not capabilities.requires_api_key
    
    @patch('engines.hacker_news_engine.HackerNewsTools')
    def test_execute_tech_search(self, mock_tools_class):
        """Test tech search execution."""
        mock_tool = Mock()
        mock_tool.get_top_stories.return_value = json.dumps({
            "stories": [
                {
                    "title": "Tech Story",
                    "url": "https://news.ycombinator.com/item?id=123",
                    "score": 100,
                    "time": "2024-01-01T12:00:00Z"
                }
            ]
        })
        mock_tools_class.return_value = mock_tool
        
        params = SearchParameters(
            query="programming discussion",
            engine="hacker_news",
            search_type=SearchType.TECH
        )
        
        response = self.engine.execute_search(params)
        
        assert isinstance(response, SearchResponse)
        assert response.search_type == SearchType.TECH
        assert response.engine_used == "hacker_news"
        mock_tool.get_top_stories.assert_called_once()
    
    def test_availability_check(self):
        """Test HackerNews availability check."""
        # HackerNews should generally be available (no API key required)
        assert self.engine.is_available()


class TestEngineAdapterCommon:
    """Test common functionality across all engine adapters."""
    
    def test_all_engines_implement_base_interface(self):
        """Test that all engines implement the BaseWebSearchEngine interface."""
        engines = [
            create_duckduckgo_engine(),
            create_google_serp_engine(),
            create_tavily_engine(),
            create_wikipedia_engine(),
            create_news_api_engine(),
            create_hacker_news_engine()
        ]
        
        for engine in engines:
            assert isinstance(engine, BaseWebSearchEngine)
            assert hasattr(engine, 'execute_search')
            assert hasattr(engine, 'is_available')
            assert hasattr(engine, 'supports_search_type')
            assert hasattr(engine, 'get_health_status')
    
    def test_all_engines_have_valid_capabilities(self):
        """Test that all engines have valid capability definitions."""
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
            assert len(capabilities.supported_search_types) > 0
            assert capabilities.max_results > 0
            assert isinstance(capabilities.requires_api_key, bool)
    
    def test_engine_factory_functions(self):
        """Test that all engine factory functions work correctly."""
        factory_functions = [
            create_duckduckgo_engine,
            create_google_serp_engine,
            create_tavily_engine,
            create_wikipedia_engine,
            create_news_api_engine,
            create_hacker_news_engine
        ]
        
        for factory in factory_functions:
            engine = factory()
            assert isinstance(engine, BaseWebSearchEngine)
            assert engine.name is not None
            assert len(engine.name) > 0
    
    def test_engine_error_handling(self):
        """Test that engines handle errors consistently."""
        engines = [
            create_duckduckgo_engine(),
            create_google_serp_engine(),
            create_tavily_engine(),
            create_wikipedia_engine(),
            create_news_api_engine(),
            create_hacker_news_engine()
        ]
        
        for engine in engines:
            # Test that engines raise EngineException for invalid parameters
            with patch.object(engine, '_execute_search', side_effect=Exception("Test error")):
                params = SearchParameters(
                    query="test",
                    engine=engine.name,
                    search_type=SearchType.WEB
                )
                
                with pytest.raises(EngineException):
                    engine.execute_search(params)
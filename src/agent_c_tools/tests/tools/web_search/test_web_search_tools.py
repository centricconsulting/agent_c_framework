"""
Unit tests for the main WebSearchTools class.
"""
import pytest
import json
from unittest.mock import Mock, patch, MagicMock
from typing import Dict, Any

from agent_c_tools.tools.web_search.web_search_tools import WebSearchTools
from agent_c_tools.tools.web_search.base.models import SearchParameters, SearchResponse, SearchType, SafeSearchLevel
from agent_c_tools.tools.web_search.base.engine import EngineException, EngineUnavailableException


class TestWebSearchTools:
    """Test suite for WebSearchTools class."""
    
    def setup_method(self):
        """Set up test fixtures before each test method."""
        self.tools = WebSearchTools()
    
    def test_tools_initialization(self):
        """Test WebSearchTools initialization."""
        tools = WebSearchTools()
        assert tools is not None
        assert hasattr(tools, 'registry')
        assert hasattr(tools, 'router')
        assert hasattr(tools, 'validator')
        assert hasattr(tools, 'standardizer')
        assert hasattr(tools, 'error_handler')
    
    def test_engine_registration(self):
        """Test that engines are properly registered."""
        # Check that engines are registered
        available_engines = self.tools.registry.get_available_engines()
        
        expected_engines = [
            "duckduckgo", "google_serp", "tavily", 
            "wikipedia", "news_api", "hacker_news"
        ]
        
        for engine in expected_engines:
            assert engine in available_engines
    
    @patch('web_search_tools.WebSearchTools._execute_search')
    def test_web_search_method(self, mock_execute):
        """Test the web_search method."""
        # Mock successful search response
        mock_response = SearchResponse(
            results=[],
            total_results=0,
            search_time=0.5,
            engine_used="duckduckgo",
            query="test query",
            search_type=SearchType.WEB
        )
        mock_execute.return_value = mock_response
        
        # Test web search
        result = self.tools.web_search(
            query="test query",
            engine="duckduckgo",
            max_results=10
        )
        
        # Verify result is JSON string
        assert isinstance(result, str)
        parsed_result = json.loads(result)
        assert "results" in parsed_result
        assert "metadata" in parsed_result
        assert parsed_result["metadata"]["engine_used"] == "duckduckgo"
        
        # Verify mock was called with correct parameters
        mock_execute.assert_called_once()
        call_args = mock_execute.call_args[0][0]
        assert call_args.query == "test query"
        assert call_args.engine == "duckduckgo"
        assert call_args.max_results == 10
    
    @patch('web_search_tools.WebSearchTools._execute_search')
    def test_news_search_method(self, mock_execute):
        """Test the news_search method."""
        mock_response = SearchResponse(
            results=[],
            total_results=0,
            search_time=0.5,
            engine_used="news_api",
            query="latest news",
            search_type=SearchType.NEWS
        )
        mock_execute.return_value = mock_response
        
        result = self.tools.news_search(
            query="latest news",
            category="technology",
            max_results=20
        )
        
        assert isinstance(result, str)
        parsed_result = json.loads(result)
        assert parsed_result["metadata"]["search_type"] == "news"
        
        # Verify search type was set correctly
        call_args = mock_execute.call_args[0][0]
        assert call_args.search_type == SearchType.NEWS
        assert call_args.category == "technology"
    
    @patch('web_search_tools.WebSearchTools._execute_search')
    def test_educational_search_method(self, mock_execute):
        """Test the educational_search method."""
        mock_response = SearchResponse(
            results=[],
            total_results=0,
            search_time=0.5,
            engine_used="wikipedia",
            query="history of computers",
            search_type=SearchType.EDUCATIONAL
        )
        mock_execute.return_value = mock_response
        
        result = self.tools.educational_search(
            query="history of computers",
            max_results=15
        )
        
        assert isinstance(result, str)
        parsed_result = json.loads(result)
        assert parsed_result["metadata"]["search_type"] == "educational"
        
        call_args = mock_execute.call_args[0][0]
        assert call_args.search_type == SearchType.EDUCATIONAL
    
    @patch('web_search_tools.WebSearchTools._execute_search')
    def test_research_search_method(self, mock_execute):
        """Test the research_search method."""
        mock_response = SearchResponse(
            results=[],
            total_results=0,
            search_time=0.5,
            engine_used="tavily",
            query="AI research",
            search_type=SearchType.RESEARCH
        )
        mock_execute.return_value = mock_response
        
        result = self.tools.research_search(
            query="AI research",
            search_depth="deep",
            include_domains=["arxiv.org"]
        )
        
        assert isinstance(result, str)
        parsed_result = json.loads(result)
        assert parsed_result["metadata"]["search_type"] == "research"
        
        call_args = mock_execute.call_args[0][0]
        assert call_args.search_type == SearchType.RESEARCH
        assert call_args.include_domains == ["arxiv.org"]
    
    @patch('web_search_tools.WebSearchTools._execute_search')
    def test_tech_search_method(self, mock_execute):
        """Test the tech_search method."""
        mock_response = SearchResponse(
            results=[],
            total_results=0,
            search_time=0.5,
            engine_used="hacker_news",
            query="programming discussion",
            search_type=SearchType.TECH
        )
        mock_execute.return_value = mock_response
        
        result = self.tools.tech_search(
            query="programming discussion",
            max_results=25
        )
        
        assert isinstance(result, str)
        parsed_result = json.loads(result)
        assert parsed_result["metadata"]["search_type"] == "tech"
        
        call_args = mock_execute.call_args[0][0]
        assert call_args.search_type == SearchType.TECH
    
    @patch('web_search_tools.WebSearchTools._execute_search')
    def test_flights_search_method(self, mock_execute):
        """Test the flights_search method."""
        mock_response = SearchResponse(
            results=[],
            total_results=0,
            search_time=0.5,
            engine_used="google_serp",
            query="flights NYC to LAX",
            search_type=SearchType.FLIGHTS
        )
        mock_execute.return_value = mock_response
        
        result = self.tools.flights_search(
            query="flights NYC to LAX",
            departure_date="2024-06-01",
            return_date="2024-06-15"
        )
        
        assert isinstance(result, str)
        parsed_result = json.loads(result)
        assert parsed_result["metadata"]["search_type"] == "flights"
        
        call_args = mock_execute.call_args[0][0]
        assert call_args.search_type == SearchType.FLIGHTS
        assert call_args.departure_date is not None
        assert call_args.return_date is not None
    
    @patch('web_search_tools.WebSearchTools._execute_search')
    def test_events_search_method(self, mock_execute):
        """Test the events_search method."""
        mock_response = SearchResponse(
            results=[],
            total_results=0,
            search_time=0.5,
            engine_used="google_serp",
            query="events in San Francisco",
            search_type=SearchType.EVENTS
        )
        mock_execute.return_value = mock_response
        
        result = self.tools.events_search(
            query="events in San Francisco",
            date_from="2024-06-01",
            date_to="2024-06-30"
        )
        
        assert isinstance(result, str)
        parsed_result = json.loads(result)
        assert parsed_result["metadata"]["search_type"] == "events"
        
        call_args = mock_execute.call_args[0][0]
        assert call_args.search_type == SearchType.EVENTS
    
    def test_get_engine_info_method(self):
        """Test the get_engine_info method."""
        result = self.tools.get_engine_info()
        
        assert isinstance(result, str)
        parsed_result = json.loads(result)
        assert "engines" in parsed_result
        assert "summary" in parsed_result
        
        # Check that all expected engines are listed
        engines = parsed_result["engines"]
        expected_engines = [
            "duckduckgo", "google_serp", "tavily", 
            "wikipedia", "news_api", "hacker_news"
        ]
        
        for engine in expected_engines:
            assert engine in engines
            assert "capabilities" in engines[engine]
            assert "health_status" in engines[engine]
    
    def test_parameter_validation(self):
        """Test parameter validation in search methods."""
        # Test invalid query (empty)
        with pytest.raises(Exception):  # Should raise validation error
            self.tools.web_search(query="")
        
        # Test invalid engine
        with pytest.raises(Exception):  # Should raise validation error
            self.tools.web_search(query="test", engine="invalid_engine")
        
        # Test invalid max_results
        with pytest.raises(Exception):  # Should raise validation error
            self.tools.web_search(query="test", max_results=0)
    
    @patch('web_search_tools.WebSearchTools._execute_search')
    def test_auto_engine_routing(self, mock_execute):
        """Test automatic engine routing."""
        mock_response = SearchResponse(
            results=[],
            total_results=0,
            search_time=0.5,
            engine_used="duckduckgo",  # Router chose this engine
            query="test query",
            search_type=SearchType.WEB
        )
        mock_execute.return_value = mock_response
        
        # Test with engine="auto"
        result = self.tools.web_search(
            query="test query",
            engine="auto"
        )
        
        assert isinstance(result, str)
        parsed_result = json.loads(result)
        
        # Verify that an engine was selected
        assert parsed_result["metadata"]["engine_used"] in [
            "duckduckgo", "google_serp", "tavily", 
            "wikipedia", "news_api", "hacker_news"
        ]
    
    @patch('web_search_tools.WebSearchTools._execute_search')
    def test_error_handling(self, mock_execute):
        """Test error handling in search methods."""
        # Test engine exception
        mock_execute.side_effect = EngineException("Search failed", "duckduckgo")
        
        result = self.tools.web_search(
            query="test query",
            engine="duckduckgo"
        )
        
        assert isinstance(result, str)
        parsed_result = json.loads(result)
        assert "error" in parsed_result
        assert parsed_result["error"]["type"] == "EngineException"
        assert "Search failed" in parsed_result["error"]["message"]
    
    @patch('web_search_tools.WebSearchTools._execute_search')
    def test_fallback_engine_on_failure(self, mock_execute):
        """Test fallback engine selection on primary engine failure."""
        # First call fails, second succeeds with fallback
        mock_execute.side_effect = [
            EngineUnavailableException("Primary engine down"),
            SearchResponse(
                results=[],
                total_results=0,
                search_time=0.5,
                engine_used="google_serp",  # Fallback engine
                query="test query",
                search_type=SearchType.WEB
            )
        ]
        
        result = self.tools.web_search(
            query="test query",
            engine="duckduckgo"
        )
        
        assert isinstance(result, str)
        parsed_result = json.loads(result)
        
        # Should have used fallback engine
        assert parsed_result["metadata"]["engine_used"] == "google_serp"
        assert "fallback_used" in parsed_result["metadata"]
    
    def test_search_parameter_building(self):
        """Test internal search parameter building."""
        # Test with various parameters
        kwargs = {
            "query": "test query",
            "engine": "duckduckgo",
            "search_type": "web",
            "max_results": 20,
            "safe_search": "strict",
            "language": "en",
            "region": "US",
            "include_domains": ["example.com"],
            "exclude_domains": ["spam.com"],
            "date_from": "2024-01-01",
            "date_to": "2024-12-31"
        }
        
        params = self.tools._build_search_parameters(kwargs)
        
        assert isinstance(params, SearchParameters)
        assert params.query == "test query"
        assert params.engine == "duckduckgo"
        assert params.search_type == SearchType.WEB
        assert params.max_results == 20
        assert params.safe_search == SafeSearchLevel.STRICT
        assert params.language == "en"
        assert params.region == "US"
        assert params.include_domains == ["example.com"]
        assert params.exclude_domains == ["spam.com"]
        assert params.date_from is not None
        assert params.date_to is not None
    
    @patch('web_search_tools.WebSearchTools._execute_search')
    def test_json_response_format(self, mock_execute):
        """Test that responses are properly formatted as JSON."""
        mock_response = SearchResponse(
            results=[
                Mock(
                    title="Test Result",
                    url="https://example.com",
                    snippet="Test snippet",
                    published_date=None,
                    source="example.com",
                    relevance_score=0.95,
                    metadata={"test": "data"}
                )
            ],
            total_results=1,
            search_time=0.5,
            engine_used="duckduckgo",
            query="test query",
            search_type=SearchType.WEB,
            metadata={"engine_metadata": "test"}
        )
        mock_execute.return_value = mock_response
        
        result = self.tools.web_search(query="test query")
        
        # Verify it's valid JSON
        parsed_result = json.loads(result)
        
        # Check structure
        assert "results" in parsed_result
        assert "metadata" in parsed_result
        
        # Check results structure
        assert len(parsed_result["results"]) == 1
        result_item = parsed_result["results"][0]
        assert "title" in result_item
        assert "url" in result_item
        assert "snippet" in result_item
        assert "source" in result_item
        assert "relevance_score" in result_item
        
        # Check metadata structure
        metadata = parsed_result["metadata"]
        assert "engine_used" in metadata
        assert "query" in metadata
        assert "search_type" in metadata
        assert "total_results" in metadata
        assert "search_time" in metadata
    
    def test_toolset_interface_compliance(self):
        """Test that WebSearchTools complies with toolset interface."""
        # Should have required methods for toolset
        assert hasattr(self.tools, 'web_search')
        assert hasattr(self.tools, 'news_search')
        assert hasattr(self.tools, 'educational_search')
        assert hasattr(self.tools, 'research_search')
        assert hasattr(self.tools, 'tech_search')
        assert hasattr(self.tools, 'flights_search')
        assert hasattr(self.tools, 'events_search')
        assert hasattr(self.tools, 'get_engine_info')
        
        # Methods should be callable
        assert callable(self.tools.web_search)
        assert callable(self.tools.news_search)
        assert callable(self.tools.educational_search)
        assert callable(self.tools.research_search)
        assert callable(self.tools.tech_search)
        assert callable(self.tools.flights_search)
        assert callable(self.tools.events_search)
        assert callable(self.tools.get_engine_info)
    
    @patch('web_search_tools.WebSearchTools._execute_search')
    def test_concurrent_search_handling(self, mock_execute):
        """Test handling of concurrent search requests."""
        import threading
        import time
        
        # Mock response with delay to simulate concurrent requests
        def delayed_response(*args, **kwargs):
            time.sleep(0.1)  # Small delay
            return SearchResponse(
                results=[],
                total_results=0,
                search_time=0.5,
                engine_used="duckduckgo",
                query="test query",
                search_type=SearchType.WEB
            )
        
        mock_execute.side_effect = delayed_response
        
        # Execute multiple searches concurrently
        results = []
        threads = []
        
        def search_worker():
            result = self.tools.web_search(query="test query")
            results.append(result)
        
        # Start multiple threads
        for _ in range(3):
            thread = threading.Thread(target=search_worker)
            threads.append(thread)
            thread.start()
        
        # Wait for all threads to complete
        for thread in threads:
            thread.join()
        
        # Verify all searches completed successfully
        assert len(results) == 3
        for result in results:
            assert isinstance(result, str)
            parsed_result = json.loads(result)
            assert "results" in parsed_result
            assert "metadata" in parsed_result
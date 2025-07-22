"""
Google SERP search engine adapter for the unified web search system.

This module provides a Google SERP engine implementation that conforms to the
BaseWebSearchEngine interface while wrapping the existing GoogleSerpTools.
"""

import json
import asyncio
from typing import Dict, Any, Optional
from datetime import datetime
from agent_c.util.structured_logging import get_logger

from ..base.engine import BaseWebSearchEngine, EngineException
from ..base.models import SearchParameters, WebSearchConfig, EngineCapabilities, SearchType
from ..google_serp.tool import GoogleSerpTools

logger = get_logger(__name__)


class GoogleSerpEngine(BaseWebSearchEngine):
    """
    Google SERP search engine adapter.
    
    This engine provides comprehensive search capabilities using Google's search API
    through SerpAPI. It supports web search, news, flights, and events.
    """
    
    def _initialize_engine(self) -> None:
        """Initialize the Google SERP engine with the legacy tool."""
        try:
            self.legacy_tool = GoogleSerpTools()
            logger.info("Google SERP engine initialized successfully")
        except Exception as e:
            logger.error(f"Failed to initialize Google SERP engine: {e}")
            raise EngineException(f"Google SERP initialization failed: {str(e)}", self.engine_name)
    
    def _execute_search(self, params: SearchParameters) -> Dict[str, Any]:
        """
        Execute search using Google SERP API.
        
        Args:
            params: Validated search parameters
            
        Returns:
            Raw search results from Google SERP
            
        Raises:
            EngineException: If search execution fails
        """
        try:
            # Route to appropriate search method based on search type
            if params.search_type == SearchType.WEB:
                return self._execute_web_search(params)
            elif params.search_type == SearchType.NEWS:
                return self._execute_news_search(params)
            elif params.search_type == SearchType.FLIGHTS:
                return self._execute_flights_search(params)
            elif params.search_type == SearchType.EVENTS:
                return self._execute_events_search(params)
            else:
                # Default to web search
                return self._execute_web_search(params)
                
        except Exception as e:
            logger.error(f"Google SERP search execution error: {e}")
            raise EngineException(f"Google SERP search failed: {str(e)}", self.engine_name)
    
    def _execute_web_search(self, params: SearchParameters) -> Dict[str, Any]:
        """Execute web search using Google SERP."""
        search_kwargs = {
            'query': params.query,
            'max_return': params.max_results
        }
        
        # Execute search using legacy tool (it's async)
        loop = asyncio.new_event_loop()
        asyncio.set_event_loop(loop)
        try:
            raw_result = loop.run_until_complete(
                self.legacy_tool.get_search_results(**search_kwargs)
            )
        finally:
            loop.close()
        
        # Parse JSON response
        if isinstance(raw_result, str):
            results = json.loads(raw_result)
        else:
            results = raw_result
        
        # Convert to standardized format
        standardized_results = []
        for item in results:
            standardized_item = {
                'title': item.get('title', ''),
                'url': item.get('link', ''),
                'snippet': item.get('snippet', ''),
                'source': 'Google',
                'metadata': {
                    'position': item.get('position'),
                    'displayed_link': item.get('displayed_link'),
                    'original_data': item
                }
            }
            standardized_results.append(standardized_item)
        
        return {
            'results': standardized_results,
            'total_results': len(standardized_results),
            'metadata': {
                'engine': 'google_serp',
                'search_type': params.search_type.value
            }
        }
    
    def _execute_news_search(self, params: SearchParameters) -> Dict[str, Any]:
        """Execute news search using Google SERP."""
        search_kwargs = {
            'query': params.query,
            'max_return': params.max_results
        }
        
        # Execute search using legacy tool (it's async)
        loop = asyncio.new_event_loop()
        asyncio.set_event_loop(loop)
        try:
            raw_result = loop.run_until_complete(
                self.legacy_tool.get_news(**search_kwargs)
            )
        finally:
            loop.close()
        
        # Parse JSON response
        if isinstance(raw_result, str):
            results = json.loads(raw_result)
        else:
            results = raw_result
        
        # Convert to standardized format
        standardized_results = []
        for item in results:
            # Parse date if available
            published_date = None
            if 'date' in item:
                published_date = self._parse_date(item['date'])
            
            standardized_item = {
                'title': item.get('title', ''),
                'url': item.get('link', ''),
                'snippet': item.get('snippet', ''),
                'published_date': published_date,
                'source': item.get('source', 'Google News'),
                'metadata': {
                    'thumbnail': item.get('thumbnail'),
                    'original_data': item
                }
            }
            standardized_results.append(standardized_item)
        
        return {
            'results': standardized_results,
            'total_results': len(standardized_results),
            'metadata': {
                'engine': 'google_serp',
                'search_type': params.search_type.value
            }
        }
    
    def _execute_flights_search(self, params: SearchParameters) -> Dict[str, Any]:
        """Execute flights search using Google SERP."""
        # For flights, we need specific parameters from additional_params
        additional_params = params.additional_params or {}
        
        search_kwargs = {
            'departure_id': additional_params.get('departure_id', ''),
            'arrival_id': additional_params.get('arrival_id', ''),
            'outbound_date': additional_params.get('outbound_date', ''),
            'return_date': additional_params.get('return_date', ''),
            'currency': additional_params.get('currency', 'USD'),
            'flight_type': additional_params.get('flight_type', '2'),
            'search_locale': additional_params.get('search_locale', 'us')
        }
        
        # Execute search using legacy tool (it's async)
        loop = asyncio.new_event_loop()
        asyncio.set_event_loop(loop)
        try:
            raw_result = loop.run_until_complete(
                self.legacy_tool.get_flights(**search_kwargs)
            )
        finally:
            loop.close()
        
        # Parse JSON response
        if isinstance(raw_result, str):
            results = json.loads(raw_result)
        else:
            results = raw_result
        
        # Convert flights to standardized format
        standardized_results = []
        
        # Process best flights
        for flight in results.get('best_flights', []):
            standardized_item = {
                'title': f"Flight {flight.get('departure_airport', {}).get('id', '')} to {flight.get('arrival_airport', {}).get('id', '')}",
                'url': flight.get('booking_token', ''),
                'snippet': f"Price: {flight.get('price', 'N/A')}, Duration: {flight.get('total_duration', 'N/A')}",
                'source': 'Google Flights',
                'metadata': {
                    'type': 'best_flight',
                    'price': flight.get('price'),
                    'duration': flight.get('total_duration'),
                    'original_data': flight
                }
            }
            standardized_results.append(standardized_item)
        
        # Process other flights
        for flight in results.get('other_flights', []):
            standardized_item = {
                'title': f"Flight {flight.get('departure_airport', {}).get('id', '')} to {flight.get('arrival_airport', {}).get('id', '')}",
                'url': flight.get('booking_token', ''),
                'snippet': f"Price: {flight.get('price', 'N/A')}, Duration: {flight.get('total_duration', 'N/A')}",
                'source': 'Google Flights',
                'metadata': {
                    'type': 'other_flight',
                    'price': flight.get('price'),
                    'duration': flight.get('total_duration'),
                    'original_data': flight
                }
            }
            standardized_results.append(standardized_item)
        
        return {
            'results': standardized_results,
            'total_results': len(standardized_results),
            'metadata': {
                'engine': 'google_serp',
                'search_type': params.search_type.value,
                'price_insights': results.get('price_insights')
            }
        }
    
    def _execute_events_search(self, params: SearchParameters) -> Dict[str, Any]:
        """Execute events search using Google SERP."""
        additional_params = params.additional_params or {}
        
        search_kwargs = {
            'query': params.query,
            'location': additional_params.get('location', ''),
            'max_return': params.max_results
        }
        
        # Execute search using legacy tool (it's async)
        loop = asyncio.new_event_loop()
        asyncio.set_event_loop(loop)
        try:
            raw_result = loop.run_until_complete(
                self.legacy_tool.get_events(**search_kwargs)
            )
        finally:
            loop.close()
        
        # Parse JSON response
        if isinstance(raw_result, str):
            results = json.loads(raw_result)
        else:
            results = raw_result
        
        # Convert to standardized format
        standardized_results = []
        for item in results:
            # Parse date if available
            published_date = None
            if 'date' in item:
                date_info = item['date']
                if isinstance(date_info, dict) and 'start_date' in date_info:
                    published_date = self._parse_date(date_info['start_date'])
            
            standardized_item = {
                'title': item.get('title', ''),
                'url': item.get('link', ''),
                'snippet': item.get('description', ''),
                'published_date': published_date,
                'source': 'Google Events',
                'metadata': {
                    'venue': item.get('venue'),
                    'address': item.get('address'),
                    'date': item.get('date'),
                    'original_data': item
                }
            }
            standardized_results.append(standardized_item)
        
        return {
            'results': standardized_results,
            'total_results': len(standardized_results),
            'metadata': {
                'engine': 'google_serp',
                'search_type': params.search_type.value
            }
        }
    
    def _check_availability(self) -> bool:
        """
        Check if Google SERP API is available.
        
        Returns:
            True if Google SERP API is available and configured
        """
        try:
            import os
            api_key = os.getenv('SERPAPI_API_KEY')
            
            if not api_key:
                logger.warning("SERPAPI_API_KEY not configured")
                return False
            
            # Try to import required library
            from serpapi import GoogleSearch
            
            return True
            
        except ImportError:
            logger.error("SerpAPI library not available")
            return False
        except Exception as e:
            logger.warning(f"Google SERP availability check failed: {e}")
            return False


def create_google_serp_engine() -> GoogleSerpEngine:
    """
    Factory function to create a configured Google SERP engine.
    
    Returns:
        Configured GoogleSerpEngine instance
    """
    config = WebSearchConfig(
        engine_name="google_serp",
        requires_api_key=True,
        api_key_name="SERPAPI_API_KEY",
        timeout=30,
        max_retries=3,
        capabilities=EngineCapabilities(
            search_types=[SearchType.WEB, SearchType.NEWS, SearchType.FLIGHTS, SearchType.EVENTS],
            supports_pagination=True,
            supports_date_filtering=False,
            supports_domain_filtering=False,
            supports_safe_search=False,
            supports_language_filtering=True,
            supports_region_filtering=True,
            supports_image_search=False,
            max_results_per_request=100
        ),
        default_parameters={
            'max_results': 10,
            'language': 'en',
            'region': 'us'
        }
    )
    
    return GoogleSerpEngine(config)
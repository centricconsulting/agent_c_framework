"""
Google SERP search engine adapter for the unified web search system.

This module provides a Google SERP engine implementation that conforms to the
BaseWebSearchEngine interface while wrapping the existing GoogleSerpTools.
"""

import json
import logging
import asyncio
from typing import Dict, Any, Optional
from datetime import datetime

from ..base.engine import BaseWebSearchEngine, EngineException
from ..base.models import SearchParameters, WebSearchConfig, EngineCapabilities, SearchType
from .tool import GoogleSerpTools

logger = logging.getLogger(__name__)


class GoogleSerpEngine(BaseWebSearchEngine):
    """
    Google SERP search engine adapter.
    
    This engine provides comprehensive search capabilities using Google's search API
    through SerpAPI. It supports web search, news search, flights, and events.
    """
    
    def _initialize_engine(self) -> None:
        """Initialize the Google SERP engine with the legacy tool."""
        try:
            self.legacy_tool = GoogleSerpTools()
            if not self.legacy_tool.valid:
                raise EngineException("Google SERP API key not configured", self.engine_name)
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
                return self._execute_flight_search(params)
            elif params.search_type == SearchType.EVENTS:
                return self._execute_event_search(params)
            else:
                # Default to web search for unsupported types
                logger.warning(f"Search type {params.search_type.value} not directly supported, using web search")
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
                'engine': 'google',
                'search_type': 'web'
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
                try:
                    published_date = datetime.fromisoformat(item['date'].replace('Z', '+00:00'))
                except:
                    pass
            
            standardized_item = {
                'title': item.get('title', ''),
                'url': item.get('link', ''),
                'snippet': item.get('snippet', ''),
                'published_date': published_date,
                'source': item.get('source', 'Google News'),
                'metadata': {
                    'position': item.get('position'),
                    'thumbnail': item.get('thumbnail'),
                    'original_data': item
                }
            }
            standardized_results.append(standardized_item)
        
        return {
            'results': standardized_results,
            'total_results': len(standardized_results),
            'metadata': {
                'engine': 'google',
                'search_type': 'news'
            }
        }
    
    def _execute_flight_search(self, params: SearchParameters) -> Dict[str, Any]:
        """Execute flight search using Google SERP."""
        # Flight search requires specific parameters from additional_params
        additional = params.additional_params or {}
        
        if not all(key in additional for key in ['departure_id', 'arrival_id', 'outbound_date']):
            raise EngineException("Flight search requires departure_id, arrival_id, and outbound_date parameters", self.engine_name)
        
        search_kwargs = {
            'departure_id': additional['departure_id'],
            'arrival_id': additional['arrival_id'],
            'outbound_date': additional['outbound_date'],
            'return_date': additional.get('return_date'),
            'currency': additional.get('currency', 'USD'),
            'flight_type': additional.get('flight_type', '2'),
            'search_locale': additional.get('search_locale', 'us')
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
        
        # Convert flight results to standardized format
        standardized_results = []
        
        # Process best flights
        for flight in results.get('best_flights', []):
            standardized_item = {
                'title': f"Flight from {additional['departure_id']} to {additional['arrival_id']}",
                'url': flight.get('booking_link', ''),
                'snippet': f"Price: {flight.get('price', 'N/A')}, Duration: {flight.get('total_duration', 'N/A')}",
                'source': 'Google Flights',
                'metadata': {
                    'flight_type': 'best',
                    'price': flight.get('price'),
                    'duration': flight.get('total_duration'),
                    'original_data': flight
                }
            }
            standardized_results.append(standardized_item)
        
        # Process other flights
        for flight in results.get('other_flights', []):
            standardized_item = {
                'title': f"Flight from {additional['departure_id']} to {additional['arrival_id']}",
                'url': flight.get('booking_link', ''),
                'snippet': f"Price: {flight.get('price', 'N/A')}, Duration: {flight.get('total_duration', 'N/A')}",
                'source': 'Google Flights',
                'metadata': {
                    'flight_type': 'other',
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
                'engine': 'google',
                'search_type': 'flights',
                'price_insights': results.get('price_insights')
            }
        }
    
    def _execute_event_search(self, params: SearchParameters) -> Dict[str, Any]:
        """Execute event search using Google SERP."""
        additional = params.additional_params or {}
        
        search_kwargs = {
            'query': params.query,
            'location': additional.get('location', ''),
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
            event_date = None
            if 'date' in item:
                try:
                    event_date = datetime.fromisoformat(item['date'].replace('Z', '+00:00'))
                except:
                    pass
            
            standardized_item = {
                'title': item.get('title', ''),
                'url': item.get('link', ''),
                'snippet': f"Date: {item.get('date', 'TBD')}, Venue: {item.get('venue', 'TBD')}",
                'published_date': event_date,
                'source': 'Google Events',
                'metadata': {
                    'venue': item.get('venue'),
                    'address': item.get('address'),
                    'event_date': item.get('date'),
                    'original_data': item
                }
            }
            standardized_results.append(standardized_item)
        
        return {
            'results': standardized_results,
            'total_results': len(standardized_results),
            'metadata': {
                'engine': 'google',
                'search_type': 'events'
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
        engine_name="google",
        requires_api_key=True,
        api_key_name="SERPAPI_API_KEY",
        timeout=30,
        max_retries=3,
        capabilities=EngineCapabilities(
            search_types=[SearchType.WEB, SearchType.NEWS, SearchType.FLIGHTS, SearchType.EVENTS],
            supports_pagination=True,
            supports_date_filtering=True,
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
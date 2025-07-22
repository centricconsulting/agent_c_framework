"""
Hacker News search engine adapter for the unified web search system.

This module provides a Hacker News engine implementation that conforms to the
BaseWebSearchEngine interface while wrapping the existing HackerNewsTools.
"""

import json
import logging
import asyncio
import requests
from typing import Dict, Any, Optional
from datetime import datetime

from ..base.engine import BaseWebSearchEngine, EngineException
from ..base.models import SearchParameters, WebSearchConfig, EngineCapabilities, SearchType
from .tool import HackerNewsTools

logger = logging.getLogger(__name__)


class HackerNewsEngine(BaseWebSearchEngine):
    """
    Hacker News search engine adapter.
    
    This engine provides tech community search capabilities using Hacker News API,
    optimized for technology discussions, startup news, and programming content.
    """
    
    def _initialize_engine(self) -> None:
        """Initialize the Hacker News engine with the legacy tool."""
        try:
            self.legacy_tool = HackerNewsTools()
            self.base_url = 'https://hacker-news.firebaseio.com/v0/'
            self.search_url = 'https://hn.algolia.com/api/v1/search'
            logger.info("Hacker News engine initialized successfully")
        except Exception as e:
            logger.error(f"Failed to initialize Hacker News engine: {e}")
            raise EngineException(f"Hacker News initialization failed: {str(e)}", self.engine_name)
    
    def _execute_search(self, params: SearchParameters) -> Dict[str, Any]:
        """
        Execute search using Hacker News API.
        
        Args:
            params: Validated search parameters
            
        Returns:
            Raw search results from Hacker News
            
        Raises:
            EngineException: If search execution fails
        """
        try:
            additional = params.additional_params or {}
            
            # Determine search type
            if params.search_type == SearchType.TRENDS or not params.query.strip():
                # Get trending/top stories
                story_type = additional.get('story_type', 'top')
                return self._execute_trending_search(params, story_type)
            else:
                # Search for specific content
                return self._execute_content_search(params)
                
        except Exception as e:
            logger.error(f"Hacker News search execution error: {e}")
            raise EngineException(f"Hacker News search failed: {str(e)}", self.engine_name)
    
    def _execute_content_search(self, params: SearchParameters) -> Dict[str, Any]:
        """Execute content search using Hacker News Algolia search API."""
        try:
            # Use Algolia search API for content search
            search_params = {
                'query': params.query,
                'hitsPerPage': min(params.max_results, 50),
                'page': getattr(params, 'page', 1) - 1,  # Algolia uses 0-based indexing
                'tags': 'story'  # Focus on stories rather than comments
            }
            
            # Add time range if specified
            additional = params.additional_params or {}
            time_range = additional.get('time_range', 'all')
            if time_range != 'all':
                search_params['numericFilters'] = self._get_time_filter(time_range)
            
            response = requests.get(self.search_url, params=search_params, timeout=30)
            response.raise_for_status()
            
            search_results = response.json()
            
            # Convert to standardized format
            standardized_results = []
            for hit in search_results.get('hits', []):
                # Parse creation time
                created_at = None
                if hit.get('created_at'):
                    try:
                        created_at = datetime.fromisoformat(hit['created_at'].replace('Z', '+00:00'))
                    except:
                        pass
                
                # Determine URL - prefer story URL, fall back to HN discussion
                url = hit.get('url') or f"https://news.ycombinator.com/item?id={hit.get('objectID')}"
                
                # Create snippet from story text or title
                snippet = hit.get('story_text', '')
                if not snippet:
                    snippet = f"Hacker News discussion about: {hit.get('title', '')}"
                
                standardized_item = {
                    'title': hit.get('title', ''),
                    'url': url,
                    'snippet': snippet[:500] + "..." if len(snippet) > 500 else snippet,
                    'published_date': created_at,
                    'source': 'Hacker News',
                    'metadata': {
                        'author': hit.get('author'),
                        'points': hit.get('points'),
                        'num_comments': hit.get('num_comments'),
                        'story_id': hit.get('story_id'),
                        'object_id': hit.get('objectID'),
                        'tags': hit.get('_tags', []),
                        'created_at_i': hit.get('created_at_i'),
                        'original_data': hit
                    }
                }
                standardized_results.append(standardized_item)
            
            return {
                'results': standardized_results,
                'total_results': search_results.get('nbHits', len(standardized_results)),
                'metadata': {
                    'engine': 'hackernews',
                    'search_type': 'content',
                    'query': params.query,
                    'page': search_results.get('page', 0) + 1,
                    'pages_available': search_results.get('nbPages', 1),
                    'processing_time': search_results.get('processingTimeMS')
                }
            }
            
        except requests.RequestException as e:
            logger.error(f"Hacker News search API error: {e}")
            raise EngineException(f"Hacker News search API failed: {str(e)}", self.engine_name)
        except Exception as e:
            logger.error(f"Hacker News content search error: {e}")
            raise EngineException(f"Hacker News content search failed: {str(e)}", self.engine_name)
    
    def _execute_trending_search(self, params: SearchParameters, story_type: str = 'top') -> Dict[str, Any]:
        """Execute trending/top stories search using legacy tool."""
        try:
            # Map story types to legacy tool methods
            if story_type == 'job':
                search_method = self.legacy_tool.get_job_stories
            else:
                search_method = self.legacy_tool.get_top_stories
            
            search_kwargs = {
                'limit': params.max_results
            }
            
            # Execute search using legacy tool (it's async)
            loop = asyncio.new_event_loop()
            asyncio.set_event_loop(loop)
            try:
                raw_result = loop.run_until_complete(
                    search_method(**search_kwargs)
                )
            finally:
                loop.close()
            
            # Parse JSON response
            if isinstance(raw_result, str):
                results = json.loads(raw_result)
            else:
                results = raw_result
            
            # Handle error responses
            if isinstance(results, dict) and 'error' in results:
                raise EngineException(f"Hacker News API error: {results['error']}", self.engine_name)
            
            # Convert to standardized format
            standardized_results = []
            for i, item in enumerate(results):
                title = item.get('title', '')
                
                # Generate HN discussion URL (we don't have story IDs from legacy tool)
                # This is a limitation of the current legacy tool implementation
                url = f"https://news.ycombinator.com/newest"
                
                standardized_item = {
                    'title': title,
                    'url': url,
                    'snippet': f"Trending {story_type} story on Hacker News",
                    'source': 'Hacker News',
                    'metadata': {
                        'story_type': story_type,
                        'rank': i + 1,
                        'original_data': item
                    }
                }
                standardized_results.append(standardized_item)
            
            return {
                'results': standardized_results,
                'total_results': len(standardized_results),
                'metadata': {
                    'engine': 'hackernews',
                    'search_type': 'trending',
                    'story_type': story_type
                }
            }
            
        except Exception as e:
            logger.error(f"Hacker News trending search error: {e}")
            raise EngineException(f"Hacker News trending search failed: {str(e)}", self.engine_name)
    
    def _get_time_filter(self, time_range: str) -> str:
        """Get numeric filter for time range."""
        now = datetime.now()
        
        if time_range == 'day':
            timestamp = int((now.timestamp() - 86400))  # 24 hours ago
        elif time_range == 'week':
            timestamp = int((now.timestamp() - 604800))  # 7 days ago
        elif time_range == 'month':
            timestamp = int((now.timestamp() - 2592000))  # 30 days ago
        elif time_range == 'year':
            timestamp = int((now.timestamp() - 31536000))  # 365 days ago
        else:
            return ''  # No filter for 'all' or unknown ranges
        
        return f'created_at_i>{timestamp}'
    
    def _check_availability(self) -> bool:
        """
        Check if Hacker News is available.
        
        Hacker News doesn't require API keys and is generally available.
        
        Returns:
            True if Hacker News is available
        """
        try:
            # Test both the Firebase API and Algolia search API
            firebase_response = requests.get(f"{self.base_url}/topstories.json", timeout=10)
            algolia_response = requests.get(f"{self.search_url}?query=test&hitsPerPage=1", timeout=10)
            
            return firebase_response.status_code == 200 and algolia_response.status_code == 200
            
        except Exception as e:
            logger.warning(f"Hacker News availability check failed: {e}")
            return False


def create_hackernews_engine() -> HackerNewsEngine:
    """
    Factory function to create a configured Hacker News engine.
    
    Returns:
        Configured HackerNewsEngine instance
    """
    config = WebSearchConfig(
        engine_name="hackernews",
        requires_api_key=False,
        timeout=30,
        max_retries=3,
        capabilities=EngineCapabilities(
            search_types=[SearchType.TECH_COMMUNITY, SearchType.TRENDS, SearchType.NEWS],
            supports_pagination=True,
            supports_date_filtering=True,
            supports_domain_filtering=False,
            supports_safe_search=False,
            supports_language_filtering=False,
            supports_region_filtering=False,
            supports_image_search=False,
            max_results_per_request=50
        ),
        default_parameters={
            'max_results': 20,
            'story_type': 'top',
            'time_range': 'all'
        }
    )
    
    return HackerNewsEngine(config)
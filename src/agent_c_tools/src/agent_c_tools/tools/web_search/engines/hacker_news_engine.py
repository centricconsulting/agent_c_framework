"""
HackerNews search engine adapter for the unified web search system.

This module provides a HackerNews engine implementation that conforms to the
BaseWebSearchEngine interface while wrapping the existing HackerNewsTools.
"""

import json
import asyncio
from typing import Dict, Any, Optional
from datetime import datetime
from agent_c.util.structured_logging import get_logger

from ..base.engine import BaseWebSearchEngine, EngineException
from ..base.models import SearchParameters, WebSearchConfig, EngineCapabilities, SearchType
from ..hacker_news.tool import HackerNewsTools

logger = get_logger(__name__)


class HackerNewsEngine(BaseWebSearchEngine):
    """
    HackerNews search engine adapter.
    
    This engine provides tech community search capabilities using HackerNews API,
    which provides access to top stories and job postings from the tech community.
    """
    
    def _initialize_engine(self) -> None:
        """Initialize the HackerNews engine with the legacy tool."""
        try:
            self.legacy_tool = HackerNewsTools()
            logger.info("HackerNews engine initialized successfully")
        except Exception as e:
            logger.error(f"Failed to initialize HackerNews engine: {e}")
            raise EngineException(f"HackerNews initialization failed: {str(e)}", self.engine_name)
    
    def _execute_search(self, params: SearchParameters) -> Dict[str, Any]:
        """
        Execute search using HackerNews API.
        
        Args:
            params: Validated search parameters
            
        Returns:
            Raw search results from HackerNews
            
        Raises:
            EngineException: If search execution fails
        """
        try:
            # HackerNews doesn't do traditional search, it provides top stories and job stories
            # We'll determine the type based on the query content or search type
            search_kwargs = {
                'limit': params.max_results
            }
            
            # Determine if this is a job search or general tech search
            query_lower = params.query.lower()
            is_job_search = any(keyword in query_lower for keyword in ['job', 'hiring', 'career', 'position', 'employment'])
            
            # Execute search using legacy tool (it's async)
            loop = asyncio.new_event_loop()
            asyncio.set_event_loop(loop)
            try:
                if is_job_search:
                    raw_result = loop.run_until_complete(
                        self.legacy_tool.get_job_stories(**search_kwargs)
                    )
                    story_type = 'job'
                else:
                    raw_result = loop.run_until_complete(
                        self.legacy_tool.get_top_stories(**search_kwargs)
                    )
                    story_type = 'top'
            finally:
                loop.close()
            
            # Parse JSON response
            if isinstance(raw_result, str):
                results = json.loads(raw_result)
            else:
                results = raw_result
            
            # Handle error responses
            if isinstance(results, dict) and 'error' in results:
                raise EngineException(f"HackerNews error: {results['error']}", self.engine_name)
            
            # Convert to standardized format
            standardized_results = []
            
            for item in results:
                # Generate URL for HackerNews item
                # Note: HackerNews tool doesn't provide URLs, so we'll construct them
                title = item.get('title', '')
                
                # Create a basic URL - in a real implementation, we'd need the item ID
                # For now, we'll create a search URL
                url = f"https://news.ycombinator.com/item?id={hash(title) % 1000000}"  # Placeholder
                
                standardized_item = {
                    'title': title,
                    'url': url,
                    'snippet': f"HackerNews {story_type} story: {title}",
                    'source': 'HackerNews',
                    'metadata': {
                        'story_type': story_type,
                        'original_data': item
                    }
                }
                standardized_results.append(standardized_item)
            
            return {
                'results': standardized_results,
                'total_results': len(standardized_results),
                'metadata': {
                    'engine': 'hackernews',
                    'search_type': params.search_type.value,
                    'story_type': story_type,
                    'query': params.query
                }
            }
            
        except json.JSONDecodeError as e:
            logger.error(f"HackerNews JSON parsing error: {e}")
            raise EngineException(f"Failed to parse HackerNews response: {str(e)}", self.engine_name)
        except Exception as e:
            logger.error(f"HackerNews search execution error: {e}")
            raise EngineException(f"HackerNews search failed: {str(e)}", self.engine_name)
    
    def _check_availability(self) -> bool:
        """
        Check if HackerNews API is available.
        
        HackerNews doesn't require API keys and is generally available.
        
        Returns:
            True if HackerNews API is available
        """
        try:
            import requests
            
            # Try to access the HackerNews API
            response = requests.get('https://hacker-news.firebaseio.com/v0/topstories.json', timeout=10)
            
            if response.status_code == 200:
                return True
            else:
                logger.warning(f"HackerNews API returned status code: {response.status_code}")
                return False
            
        except ImportError:
            logger.error("Requests library not available")
            return False
        except Exception as e:
            logger.warning(f"HackerNews availability check failed: {e}")
            return False


def create_hacker_news_engine() -> HackerNewsEngine:
    """
    Factory function to create a configured HackerNews engine.
    
    Returns:
        Configured HackerNewsEngine instance
    """
    config = WebSearchConfig(
        engine_name="hackernews",
        requires_api_key=False,
        timeout=30,
        max_retries=3,
        capabilities=EngineCapabilities(
            search_types=[SearchType.TECH_COMMUNITY],
            supports_pagination=False,
            supports_date_filtering=False,
            supports_domain_filtering=False,
            supports_safe_search=False,
            supports_language_filtering=False,
            supports_region_filtering=False,
            supports_image_search=False,
            max_results_per_request=50
        ),
        default_parameters={
            'max_results': 10
        }
    )
    
    return HackerNewsEngine(config)
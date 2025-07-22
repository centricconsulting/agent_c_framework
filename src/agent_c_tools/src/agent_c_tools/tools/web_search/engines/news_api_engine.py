"""
NewsAPI search engine adapter for the unified web search system.

This module provides a NewsAPI engine implementation that conforms to the
BaseWebSearchEngine interface while wrapping the existing NewsApiTools.
"""

import json
import asyncio
from typing import Dict, Any, Optional
from datetime import datetime
from agent_c.util.structured_logging import get_logger

from ..base.engine import BaseWebSearchEngine, EngineException
from ..base.models import SearchParameters, WebSearchConfig, EngineCapabilities, SearchType
from ..news_api.tool import NewsApiTools

logger = get_logger(__name__)


class NewsApiEngine(BaseWebSearchEngine):
    """
    NewsAPI search engine adapter.
    
    This engine provides news search capabilities using NewsAPI.org's API,
    which provides access to news articles from various sources worldwide.
    """
    
    def _initialize_engine(self) -> None:
        """Initialize the NewsAPI engine with the legacy tool."""
        try:
            self.legacy_tool = NewsApiTools()
            logger.info("NewsAPI engine initialized successfully")
        except Exception as e:
            logger.error(f"Failed to initialize NewsAPI engine: {e}")
            raise EngineException(f"NewsAPI initialization failed: {str(e)}", self.engine_name)
    
    def _execute_search(self, params: SearchParameters) -> Dict[str, Any]:
        """
        Execute search using NewsAPI.
        
        Args:
            params: Validated search parameters
            
        Returns:
            Raw search results from NewsAPI
            
        Raises:
            EngineException: If search execution fails
        """
        try:
            # Convert parameters to legacy tool format
            search_kwargs = {
                'q': params.query,
                'max_articles': params.max_results,
                'pageSize': min(params.max_results, 100),  # NewsAPI max is 100
                'page': params.page,
                'sort': 'relevancy'  # Default sort
            }
            
            # Add date filtering if specified
            if params.start_date:
                search_kwargs['start_date'] = params.start_date.strftime('%Y-%m-%d')
            if params.end_date:
                search_kwargs['end_date'] = params.end_date.strftime('%Y-%m-%d')
            
            # Execute search using legacy tool (it's async)
            loop = asyncio.new_event_loop()
            asyncio.set_event_loop(loop)
            try:
                raw_result = loop.run_until_complete(
                    self.legacy_tool.get_all_articles(**search_kwargs)
                )
            finally:
                loop.close()
            
            # Parse JSON response
            if isinstance(raw_result, str):
                results = json.loads(raw_result)
            else:
                results = raw_result
            
            # Handle error responses
            if 'error' in results:
                raise EngineException(f"NewsAPI error: {results['error']}", self.engine_name)
            
            # Convert to standardized format
            standardized_results = []
            articles = results.get('articles', [])
            
            for article in articles:
                # Parse publication date
                published_date = None
                if article.get('publishedAt'):
                    published_date = self._parse_date(article['publishedAt'])
                
                standardized_item = {
                    'title': article.get('title', ''),
                    'url': article.get('url', ''),
                    'snippet': article.get('description', ''),
                    'published_date': published_date,
                    'source': article.get('source', {}).get('name', 'NewsAPI'),
                    'metadata': {
                        'author': article.get('author'),
                        'source_id': article.get('source', {}).get('id'),
                        'url_to_image': article.get('urlToImage'),
                        'content': article.get('content'),
                        'original_data': article
                    }
                }
                standardized_results.append(standardized_item)
            
            return {
                'results': standardized_results,
                'total_results': results.get('total_articles', len(standardized_results)),
                'metadata': {
                    'engine': 'newsapi',
                    'search_type': params.search_type.value,
                    'query': results.get('query', params.query)
                }
            }
            
        except json.JSONDecodeError as e:
            logger.error(f"NewsAPI JSON parsing error: {e}")
            raise EngineException(f"Failed to parse NewsAPI response: {str(e)}", self.engine_name)
        except Exception as e:
            logger.error(f"NewsAPI search execution error: {e}")
            raise EngineException(f"NewsAPI search failed: {str(e)}", self.engine_name)
    
    def _check_availability(self) -> bool:
        """
        Check if NewsAPI is available.
        
        Returns:
            True if NewsAPI is available and configured
        """
        try:
            import os
            api_key = os.getenv('NEWSAPI_API_KEY')
            
            if not api_key:
                logger.warning("NEWSAPI_API_KEY not configured")
                return False
            
            # Try to import required library
            from newsapi.newsapi_client import NewsApiClient
            
            # Try to create client to verify API key format
            client = NewsApiClient(api_key=api_key)
            
            return True
            
        except ImportError:
            logger.error("NewsAPI library not available")
            return False
        except Exception as e:
            logger.warning(f"NewsAPI availability check failed: {e}")
            return False


def create_news_api_engine() -> NewsApiEngine:
    """
    Factory function to create a configured NewsAPI engine.
    
    Returns:
        Configured NewsApiEngine instance
    """
    config = WebSearchConfig(
        engine_name="newsapi",
        requires_api_key=True,
        api_key_name="NEWSAPI_API_KEY",
        timeout=30,
        max_retries=3,
        capabilities=EngineCapabilities(
            search_types=[SearchType.NEWS],
            supports_pagination=True,
            supports_date_filtering=True,
            supports_domain_filtering=False,
            supports_safe_search=False,
            supports_language_filtering=True,
            supports_region_filtering=False,
            supports_image_search=False,
            max_results_per_request=100
        ),
        default_parameters={
            'max_results': 10,
            'language': 'en',
            'sort': 'relevancy'
        }
    )
    
    return NewsApiEngine(config)
"""
NewsAPI search engine adapter for the unified web search system.

This module provides a NewsAPI engine implementation that conforms to the
BaseWebSearchEngine interface while wrapping the existing NewsApiTools.
"""

import json
import logging
import asyncio
from typing import Dict, Any, Optional
from datetime import datetime

from ..base.engine import BaseWebSearchEngine, EngineException
from ..base.models import SearchParameters, WebSearchConfig, EngineCapabilities, SearchType
from .tool import NewsApiTools

logger = logging.getLogger(__name__)


class NewsApiEngine(BaseWebSearchEngine):
    """
    NewsAPI search engine adapter.
    
    This engine provides comprehensive news search capabilities using NewsAPI,
    supporting both headline retrieval and full article search with date filtering.
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
            # Determine search method based on parameters
            additional = params.additional_params or {}
            
            # If no specific query but category is specified, use headlines
            if not params.query.strip() and additional.get('category'):
                return self._execute_headlines_search(params)
            else:
                return self._execute_article_search(params)
                
        except Exception as e:
            logger.error(f"NewsAPI search execution error: {e}")
            raise EngineException(f"NewsAPI search failed: {str(e)}", self.engine_name)
    
    def _execute_article_search(self, params: SearchParameters) -> Dict[str, Any]:
        """Execute article search using NewsAPI."""
        additional = params.additional_params or {}
        
        search_kwargs = {
            'q': params.query,
            'max_articles': params.max_results,
            'pageSize': min(params.max_results, 100),  # NewsAPI limit
            'page': params.page if hasattr(params, 'page') else 1,
            'sort': additional.get('sort', 'relevancy')
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
            result_data = json.loads(raw_result)
        else:
            result_data = raw_result
        
        # Handle error responses
        if 'error' in result_data:
            raise EngineException(f"NewsAPI error: {result_data['error']}", self.engine_name)
        
        # Convert to standardized format
        standardized_results = []
        articles = result_data.get('articles', [])
        
        for article in articles:
            # Parse publication date
            published_date = None
            if article.get('publishedAt'):
                try:
                    published_date = datetime.fromisoformat(
                        article['publishedAt'].replace('Z', '+00:00')
                    )
                except:
                    pass
            
            standardized_item = {
                'title': article.get('title', ''),
                'url': article.get('url', ''),
                'snippet': article.get('description', ''),
                'published_date': published_date,
                'source': article.get('source', {}).get('name', 'NewsAPI'),
                'metadata': {
                    'author': article.get('author'),
                    'url_to_image': article.get('urlToImage'),
                    'content': article.get('content'),
                    'source_id': article.get('source', {}).get('id'),
                    'original_data': article
                }
            }
            standardized_results.append(standardized_item)
        
        return {
            'results': standardized_results,
            'total_results': result_data.get('total_articles', len(standardized_results)),
            'metadata': {
                'engine': 'newsapi',
                'search_type': 'news',
                'query': result_data.get('query', params.query),
                'sort': search_kwargs.get('sort', 'relevancy')
            }
        }
    
    def _execute_headlines_search(self, params: SearchParameters) -> Dict[str, Any]:
        """Execute headlines search using NewsAPI."""
        additional = params.additional_params or {}
        
        search_kwargs = {
            'category': additional.get('category'),
            'pageSize': min(params.max_results, 100),  # NewsAPI limit
            'page': params.page if hasattr(params, 'page') else 1
        }
        
        # Execute search using legacy tool (it's async)
        loop = asyncio.new_event_loop()
        asyncio.set_event_loop(loop)
        try:
            raw_result = loop.run_until_complete(
                self.legacy_tool.get_top_headlines(**search_kwargs)
            )
        finally:
            loop.close()
        
        # Parse JSON response
        if isinstance(raw_result, str):
            result_data = json.loads(raw_result)
        else:
            result_data = raw_result
        
        # Handle error responses
        if isinstance(result_data, str) and result_data.startswith('error:'):
            raise EngineException(f"NewsAPI error: {result_data}", self.engine_name)
        
        # Convert to standardized format
        standardized_results = []
        articles = result_data.get('articles', [])
        
        for article in articles:
            # Parse publication date
            published_date = None
            if article.get('publishedAt'):
                try:
                    published_date = datetime.fromisoformat(
                        article['publishedAt'].replace('Z', '+00:00')
                    )
                except:
                    pass
            
            standardized_item = {
                'title': article.get('title', ''),
                'url': article.get('url', ''),
                'snippet': article.get('description', ''),
                'published_date': published_date,
                'source': article.get('source', {}).get('name', 'NewsAPI'),
                'metadata': {
                    'author': article.get('author'),
                    'url_to_image': article.get('urlToImage'),
                    'content': article.get('content'),
                    'source_id': article.get('source', {}).get('id'),
                    'category': additional.get('category'),
                    'original_data': article
                }
            }
            standardized_results.append(standardized_item)
        
        return {
            'results': standardized_results,
            'total_results': result_data.get('totalResults', len(standardized_results)),
            'metadata': {
                'engine': 'newsapi',
                'search_type': 'news',
                'category': additional.get('category'),
                'status': result_data.get('status')
            }
        }
    
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


def create_newsapi_engine() -> NewsApiEngine:
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
            'max_results': 20,
            'sort': 'relevancy',
            'language': 'en'
        }
    )
    
    return NewsApiEngine(config)
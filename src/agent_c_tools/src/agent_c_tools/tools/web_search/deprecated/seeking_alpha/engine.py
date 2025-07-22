"""
Seeking Alpha search engine adapter for the unified web search system.

This module provides a Seeking Alpha engine implementation that conforms to the
BaseWebSearchEngine interface while wrapping the existing SeekingAlphaTools.
"""

import json
import logging
import asyncio
from typing import Dict, Any, Optional
from datetime import datetime

from ..base.engine import BaseWebSearchEngine, EngineException
from ..base.models import SearchParameters, WebSearchConfig, EngineCapabilities, SearchType
from .tool import SeekingAlphaTools

logger = logging.getLogger(__name__)


class SeekingAlphaEngine(BaseWebSearchEngine):
    """
    Seeking Alpha search engine adapter.
    
    This engine provides financial news and analysis search capabilities using
    Seeking Alpha's trending news API, optimized for financial market content.
    """
    
    def _initialize_engine(self) -> None:
        """Initialize the Seeking Alpha engine with the legacy tool."""
        try:
            self.legacy_tool = SeekingAlphaTools()
            logger.info("Seeking Alpha engine initialized successfully")
        except Exception as e:
            logger.error(f"Failed to initialize Seeking Alpha engine: {e}")
            raise EngineException(f"Seeking Alpha initialization failed: {str(e)}", self.engine_name)
    
    def _execute_search(self, params: SearchParameters) -> Dict[str, Any]:
        """
        Execute search using Seeking Alpha API.
        
        Note: The current Seeking Alpha tool only supports trending news,
        not query-based search. This is a limitation of the existing implementation.
        
        Args:
            params: Validated search parameters
            
        Returns:
            Raw search results from Seeking Alpha
            
        Raises:
            EngineException: If search execution fails
        """
        try:
            additional = params.additional_params or {}
            
            # Convert parameters to legacy tool format
            search_kwargs = {
                'limit': params.max_results,
                'extract_content': additional.get('extract_content', False)
            }
            
            # Execute search using legacy tool (it's async)
            loop = asyncio.new_event_loop()
            asyncio.set_event_loop(loop)
            try:
                raw_result = loop.run_until_complete(
                    self.legacy_tool.get_topk_trending_news(**search_kwargs)
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
                raise EngineException(f"Seeking Alpha API error: {results['error']}", self.engine_name)
            
            # Filter results by query if provided (simple text matching)
            if params.query.strip():
                filtered_results = self._filter_results_by_query(results, params.query)
            else:
                filtered_results = results
            
            # Convert to standardized format
            standardized_results = []
            for article in filtered_results:
                # Parse publication date
                published_date = None
                if article.get('publishedAt'):
                    try:
                        # Handle the specific format from Seeking Alpha
                        date_str = article['publishedAt']
                        if 'T' in date_str:
                            published_date = datetime.fromisoformat(date_str)
                        else:
                            published_date = datetime.strptime(date_str, '%Y-%m-%d %H:%M:%S')
                    except:
                        pass
                
                # Create snippet from content or title
                content = article.get('content', '')
                if content:
                    snippet = content[:500] + "..." if len(content) > 500 else content
                else:
                    snippet = f"Financial news from Seeking Alpha: {article.get('title', '')}"
                
                standardized_item = {
                    'title': article.get('title', ''),
                    'url': article.get('url', ''),
                    'snippet': snippet,
                    'published_date': published_date,
                    'source': 'Seeking Alpha',
                    'metadata': {
                        'article_id': article.get('id'),
                        'content': content,
                        'extract_content': search_kwargs.get('extract_content', False),
                        'original_data': article
                    }
                }
                standardized_results.append(standardized_item)
            
            return {
                'results': standardized_results,
                'total_results': len(standardized_results),
                'metadata': {
                    'engine': 'seeking_alpha',
                    'search_type': params.search_type.value,
                    'is_trending': True,
                    'query_filtered': bool(params.query.strip()),
                    'extract_content': search_kwargs.get('extract_content', False)
                }
            }
            
        except json.JSONDecodeError as e:
            logger.error(f"Seeking Alpha JSON parsing error: {e}")
            raise EngineException(f"Failed to parse Seeking Alpha response: {str(e)}", self.engine_name)
        except Exception as e:
            logger.error(f"Seeking Alpha search execution error: {e}")
            raise EngineException(f"Seeking Alpha search failed: {str(e)}", self.engine_name)
    
    def _filter_results_by_query(self, results: list, query: str) -> list:
        """
        Filter results by query since Seeking Alpha tool doesn't support search.
        
        This is a simple text matching approach until a proper search API is available.
        
        Args:
            results: List of articles from Seeking Alpha
            query: Search query to filter by
            
        Returns:
            Filtered list of articles
        """
        if not query.strip():
            return results
        
        query_lower = query.lower()
        filtered = []
        
        for article in results:
            # Check title
            title = article.get('title', '').lower()
            content = article.get('content', '').lower()
            
            # Simple keyword matching
            if (query_lower in title or 
                query_lower in content or
                any(word in title for word in query_lower.split()) or
                any(word in content for word in query_lower.split())):
                filtered.append(article)
        
        return filtered
    
    def _check_availability(self) -> bool:
        """
        Check if Seeking Alpha is available.
        
        Seeking Alpha doesn't require API keys but we should check connectivity.
        
        Returns:
            True if Seeking Alpha is available
        """
        try:
            import aiohttp
            import asyncio
            
            async def check_connectivity():
                try:
                    async with aiohttp.ClientSession() as session:
                        async with session.get(
                            "https://seekingalpha.com/news/trending_news",
                            timeout=10
                        ) as response:
                            return response.status == 200
                except:
                    return False
            
            # Run async check
            loop = asyncio.new_event_loop()
            asyncio.set_event_loop(loop)
            try:
                is_available = loop.run_until_complete(check_connectivity())
                return is_available
            finally:
                loop.close()
            
        except ImportError:
            logger.error("aiohttp library not available for Seeking Alpha")
            return False
        except Exception as e:
            logger.warning(f"Seeking Alpha availability check failed: {e}")
            return False


def create_seeking_alpha_engine() -> SeekingAlphaEngine:
    """
    Factory function to create a configured Seeking Alpha engine.
    
    Returns:
        Configured SeekingAlphaEngine instance
    """
    config = WebSearchConfig(
        engine_name="seeking_alpha",
        requires_api_key=False,
        timeout=30,
        max_retries=3,
        capabilities=EngineCapabilities(
            search_types=[SearchType.FINANCIAL, SearchType.NEWS],
            supports_pagination=False,
            supports_date_filtering=False,
            supports_domain_filtering=False,
            supports_safe_search=False,
            supports_language_filtering=False,
            supports_region_filtering=False,
            supports_image_search=False,
            supports_content_extraction=True,
            max_results_per_request=50
        ),
        default_parameters={
            'max_results': 10,
            'extract_content': False
        }
    )
    
    return SeekingAlphaEngine(config)
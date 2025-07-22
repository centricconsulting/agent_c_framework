"""
Tavily research search engine adapter for the unified web search system.

This module provides a Tavily engine implementation that conforms to the
BaseWebSearchEngine interface while wrapping the existing TavilyResearchTools.
"""

import json
import asyncio
from typing import Dict, Any, Optional
from datetime import datetime
from agent_c.util.structured_logging import get_logger

from ..base.engine import BaseWebSearchEngine, EngineException
from ..base.models import SearchParameters, WebSearchConfig, EngineCapabilities, SearchType
from ..tavily_research.tool import TavilyResearchTools

logger = get_logger(__name__)


class TavilyEngine(BaseWebSearchEngine):
    """
    Tavily research search engine adapter.
    
    This engine provides advanced research capabilities using Tavily's API,
    which is optimized for comprehensive research and content extraction.
    """
    
    def _initialize_engine(self) -> None:
        """Initialize the Tavily engine with the legacy tool."""
        try:
            self.legacy_tool = TavilyResearchTools()
            logger.info("Tavily engine initialized successfully")
        except Exception as e:
            logger.error(f"Failed to initialize Tavily engine: {e}")
            raise EngineException(f"Tavily initialization failed: {str(e)}", self.engine_name)
    
    def _execute_search(self, params: SearchParameters) -> Dict[str, Any]:
        """
        Execute search using Tavily Research API.
        
        Args:
            params: Validated search parameters
            
        Returns:
            Raw search results from Tavily
            
        Raises:
            EngineException: If search execution fails
        """
        try:
            # Convert parameters to legacy tool format
            search_kwargs = {
                'query': params.query,
                'search_depth': params.search_depth.value,
                'max_results': params.max_results,
                'include_images': params.include_images,
                'include_answer': True,  # Tavily's strength is providing answers
                'include_raw_content': True,  # Get full content for research
            }
            
            # Add domain filtering if specified
            if params.include_domains:
                search_kwargs['include_domains'] = params.include_domains
            if params.exclude_domains:
                search_kwargs['exclude_domains'] = params.exclude_domains
            
            # Execute search using legacy tool (it's async)
            loop = asyncio.new_event_loop()
            asyncio.set_event_loop(loop)
            try:
                raw_result = loop.run_until_complete(
                    self.legacy_tool.search_tavily(**search_kwargs)
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
                # Extract title from URL if not provided
                title = item.get('title', self._extract_title_from_url(item.get('url', '')))
                
                # Truncate body content for snippet
                body = item.get('content', item.get('body', ''))
                snippet = body[:500] + "..." if len(body) > 500 else body
                
                standardized_item = {
                    'title': title,
                    'url': item.get('url', ''),
                    'snippet': snippet,
                    'score': item.get('score'),
                    'source': 'Tavily',
                    'metadata': {
                        'raw_content': body,
                        'search_depth': params.search_depth.value,
                        'original_data': item
                    }
                }
                standardized_results.append(standardized_item)
            
            return {
                'results': standardized_results,
                'total_results': len(standardized_results),
                'metadata': {
                    'engine': 'tavily',
                    'search_type': params.search_type.value,
                    'search_depth': params.search_depth.value,
                    'include_images': params.include_images
                }
            }
            
        except json.JSONDecodeError as e:
            logger.error(f"Tavily JSON parsing error: {e}")
            raise EngineException(f"Failed to parse Tavily response: {str(e)}", self.engine_name)
        except Exception as e:
            logger.error(f"Tavily search execution error: {e}")
            raise EngineException(f"Tavily search failed: {str(e)}", self.engine_name)
    
    def _extract_title_from_url(self, url: str) -> str:
        """Extract a readable title from a URL."""
        try:
            from urllib.parse import urlparse
            parsed = urlparse(url)
            
            # Get the path and clean it up
            path = parsed.path.strip('/')
            if path:
                # Remove file extensions and replace separators
                title = path.split('/')[-1]
                title = title.replace('-', ' ').replace('_', ' ')
                title = title.replace('%20', ' ')  # URL decode spaces
                # Remove file extensions
                if '.' in title:
                    title = title.rsplit('.', 1)[0]
                return title.title()
            
            # Fall back to domain name
            domain = parsed.netloc.replace('www.', '')
            return domain.title()
            
        except Exception:
            return url[:50] + "..." if len(url) > 50 else url
    
    def _check_availability(self) -> bool:
        """
        Check if Tavily API is available.
        
        Returns:
            True if Tavily API is available and configured
        """
        try:
            import os
            api_key = os.getenv('TAVILI_API_KEY')
            
            if not api_key:
                logger.warning("TAVILI_API_KEY not configured")
                return False
            
            # Try to import required library
            from tavily import TavilyClient
            
            # Try to create client to verify API key format
            client = TavilyClient(api_key=api_key)
            
            return True
            
        except ImportError:
            logger.error("Tavily library not available")
            return False
        except Exception as e:
            logger.warning(f"Tavily availability check failed: {e}")
            return False


def create_tavily_engine() -> TavilyEngine:
    """
    Factory function to create a configured Tavily engine.
    
    Returns:
        Configured TavilyEngine instance
    """
    config = WebSearchConfig(
        engine_name="tavily",
        requires_api_key=True,
        api_key_name="TAVILI_API_KEY",
        timeout=60,  # Tavily can take longer for deep research
        max_retries=3,
        capabilities=EngineCapabilities(
            search_types=[SearchType.WEB, SearchType.RESEARCH],
            supports_pagination=False,
            supports_date_filtering=False,
            supports_domain_filtering=True,
            supports_safe_search=False,
            supports_language_filtering=False,
            supports_region_filtering=False,
            supports_image_search=True,
            supports_content_extraction=True,
            max_results_per_request=20
        ),
        default_parameters={
            'search_depth': 'advanced',
            'max_results': 5,
            'include_answer': True,
            'include_raw_content': True
        }
    )
    
    return TavilyEngine(config)
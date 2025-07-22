"""
DuckDuckGo search engine adapter for the unified web search system.

This module provides a DuckDuckGo engine implementation that conforms to the
BaseWebSearchEngine interface while wrapping the existing DuckDuckGoTools.
"""

import json
import asyncio
from typing import Dict, Any, Optional
from datetime import datetime
from agent_c.util.structured_logging import get_logger

from ..base.engine import BaseWebSearchEngine, EngineException
from ..base.models import SearchParameters, WebSearchConfig, EngineCapabilities, SearchType
from ..duck_duck_go.tool import DuckDuckGoTools

logger = get_logger(__name__)


class DuckDuckGoEngine(BaseWebSearchEngine):
    """
    DuckDuckGo search engine adapter.
    
    This engine provides web search capabilities using DuckDuckGo's search API
    through the duckduckgo-search library. It supports basic web search with
    safe search filtering.
    """
    
    def _initialize_engine(self) -> None:
        """Initialize the DuckDuckGo engine with the legacy tool."""
        try:
            self.legacy_tool = DuckDuckGoTools()
            logger.info("DuckDuckGo engine initialized successfully")
        except Exception as e:
            logger.error(f"Failed to initialize DuckDuckGo engine: {e}")
            raise EngineException(f"DuckDuckGo initialization failed: {str(e)}", self.engine_name)
    
    def _execute_search(self, params: SearchParameters) -> Dict[str, Any]:
        """
        Execute search using DuckDuckGo.
        
        Args:
            params: Validated search parameters
            
        Returns:
            Raw search results from DuckDuckGo
            
        Raises:
            EngineException: If search execution fails
        """
        try:
            # Convert parameters to legacy tool format
            search_kwargs = {
                'query': params.query,
                'max_results': min(params.max_results, 50),  # DDG has limits
                'safesearch': params.safesearch.value
            }
            
            # Execute search using legacy tool (it's async)
            loop = asyncio.new_event_loop()
            asyncio.set_event_loop(loop)
            try:
                raw_result = loop.run_until_complete(
                    self.legacy_tool.web_search(**search_kwargs)
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
                    'url': item.get('href', ''),
                    'snippet': item.get('body', ''),
                    'source': 'DuckDuckGo',
                    'metadata': {
                        'original_data': item
                    }
                }
                standardized_results.append(standardized_item)
            
            return {
                'results': standardized_results,
                'total_results': len(standardized_results),
                'metadata': {
                    'engine': 'duckduckgo',
                    'search_type': params.search_type.value,
                    'safesearch': params.safesearch.value
                }
            }
            
        except json.JSONDecodeError as e:
            logger.error(f"DuckDuckGo JSON parsing error: {e}")
            raise EngineException(f"Failed to parse DuckDuckGo response: {str(e)}", self.engine_name)
        except Exception as e:
            logger.error(f"DuckDuckGo search execution error: {e}")
            raise EngineException(f"DuckDuckGo search failed: {str(e)}", self.engine_name)
    
    def _check_availability(self) -> bool:
        """
        Check if DuckDuckGo is available.
        
        DuckDuckGo doesn't require API keys and is generally available,
        but we can do a basic connectivity check.
        
        Returns:
            True if DuckDuckGo is available
        """
        try:
            # DuckDuckGo doesn't require API keys, so just check if we can import
            from duckduckgo_search import DDGS
            
            # Try a simple test search to verify connectivity
            ddgs = DDGS()
            test_results = list(ddgs.text("test", max_results=1))
            
            # If we get here without exception, it's available
            return True
            
        except ImportError:
            logger.error("DuckDuckGo search library not available")
            return False
        except Exception as e:
            logger.warning(f"DuckDuckGo availability check failed: {e}")
            # Even if test search fails, the engine might still work
            # so we'll return True unless it's a clear import error
            return True


def create_duckduckgo_engine() -> DuckDuckGoEngine:
    """
    Factory function to create a configured DuckDuckGo engine.
    
    Returns:
        Configured DuckDuckGoEngine instance
    """
    config = WebSearchConfig(
        engine_name="duckduckgo",
        requires_api_key=False,
        timeout=30,
        max_retries=3,
        capabilities=EngineCapabilities(
            search_types=[SearchType.WEB],
            supports_pagination=False,
            supports_date_filtering=False,
            supports_domain_filtering=False,
            supports_safe_search=True,
            supports_language_filtering=False,
            supports_region_filtering=False,
            supports_image_search=False,
            max_results_per_request=50
        ),
        default_parameters={
            'safesearch': 'moderate',
            'max_results': 20
        }
    )
    
    return DuckDuckGoEngine(config)
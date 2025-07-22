"""
Wikipedia search engine adapter for the unified web search system.

This module provides a Wikipedia engine implementation that conforms to the
BaseWebSearchEngine interface while wrapping the existing WikipediaTools.
"""

import json
import asyncio
from typing import Dict, Any, Optional
from datetime import datetime
from agent_c.util.structured_logging import get_logger

from ..base.engine import BaseWebSearchEngine, EngineException
from ..base.models import SearchParameters, WebSearchConfig, EngineCapabilities, SearchType
from ..wikipedia.tool import WikipediaTools

logger = get_logger(__name__)


class WikipediaEngine(BaseWebSearchEngine):
    """
    Wikipedia search engine adapter.
    
    This engine provides educational and reference search capabilities using
    Wikipedia's API through the wikipedia-api library.
    """
    
    def _initialize_engine(self) -> None:
        """Initialize the Wikipedia engine with the legacy tool."""
        try:
            self.legacy_tool = WikipediaTools()
            logger.info("Wikipedia engine initialized successfully")
        except Exception as e:
            logger.error(f"Failed to initialize Wikipedia engine: {e}")
            raise EngineException(f"Wikipedia initialization failed: {str(e)}", self.engine_name)
    
    def _execute_search(self, params: SearchParameters) -> Dict[str, Any]:
        """
        Execute search using Wikipedia API.
        
        Args:
            params: Validated search parameters
            
        Returns:
            Raw search results from Wikipedia
            
        Raises:
            EngineException: If search execution fails
        """
        try:
            # Convert parameters to legacy tool format
            search_kwargs = {
                'search': params.query
            }
            
            # Execute search using legacy tool (it's async)
            loop = asyncio.new_event_loop()
            asyncio.set_event_loop(loop)
            try:
                raw_result = loop.run_until_complete(
                    self.legacy_tool.search_wiki(**search_kwargs)
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
            
            # Handle different response formats from Wikipedia
            if isinstance(results, list):
                # Simple list of search terms
                for i, item in enumerate(results[:params.max_results]):
                    if isinstance(item, str):
                        # Simple string result
                        standardized_item = {
                            'title': item,
                            'url': f"https://en.wikipedia.org/wiki/{item.replace(' ', '_')}",
                            'snippet': f"Wikipedia article about {item}",
                            'source': 'Wikipedia',
                            'metadata': {
                                'search_result_type': 'title_only',
                                'original_data': item
                            }
                        }
                    else:
                        # Dictionary result with more details
                        standardized_item = {
                            'title': item.get('title', item.get('key', str(item))),
                            'url': f"https://en.wikipedia.org/wiki/{item.get('title', item.get('key', str(item))).replace(' ', '_')}",
                            'snippet': item.get('description', f"Wikipedia article about {item.get('title', item.get('key', str(item)))}"),
                            'source': 'Wikipedia',
                            'metadata': {
                                'search_result_type': 'detailed',
                                'id': item.get('id'),
                                'key': item.get('key'),
                                'original_data': item
                            }
                        }
                    standardized_results.append(standardized_item)
            else:
                # Single result or unexpected format
                if isinstance(results, dict):
                    standardized_item = {
                        'title': results.get('title', results.get('key', str(results))),
                        'url': f"https://en.wikipedia.org/wiki/{results.get('title', results.get('key', str(results))).replace(' ', '_')}",
                        'snippet': results.get('description', f"Wikipedia article about {results.get('title', results.get('key', str(results)))}"),
                        'source': 'Wikipedia',
                        'metadata': {
                            'search_result_type': 'single_result',
                            'original_data': results
                        }
                    }
                    standardized_results.append(standardized_item)
            
            return {
                'results': standardized_results,
                'total_results': len(standardized_results),
                'metadata': {
                    'engine': 'wikipedia',
                    'search_type': params.search_type.value,
                    'query': params.query
                }
            }
            
        except json.JSONDecodeError as e:
            logger.error(f"Wikipedia JSON parsing error: {e}")
            raise EngineException(f"Failed to parse Wikipedia response: {str(e)}", self.engine_name)
        except Exception as e:
            logger.error(f"Wikipedia search execution error: {e}")
            raise EngineException(f"Wikipedia search failed: {str(e)}", self.engine_name)
    
    def _check_availability(self) -> bool:
        """
        Check if Wikipedia is available.
        
        Wikipedia doesn't require API keys and is generally available.
        
        Returns:
            True if Wikipedia is available
        """
        try:
            # Try to import required library
            import wikipedia
            
            # Try a simple test search to verify connectivity
            test_results = wikipedia.search("test", results=1)
            
            # If we get here without exception, it's available
            return True
            
        except ImportError:
            logger.error("Wikipedia library not available")
            return False
        except Exception as e:
            logger.warning(f"Wikipedia availability check failed: {e}")
            # Even if test search fails, the engine might still work
            return True


def create_wikipedia_engine() -> WikipediaEngine:
    """
    Factory function to create a configured Wikipedia engine.
    
    Returns:
        Configured WikipediaEngine instance
    """
    config = WebSearchConfig(
        engine_name="wikipedia",
        requires_api_key=False,
        timeout=30,
        max_retries=3,
        capabilities=EngineCapabilities(
            search_types=[SearchType.WEB, SearchType.EDUCATIONAL],
            supports_pagination=False,
            supports_date_filtering=False,
            supports_domain_filtering=False,
            supports_safe_search=False,
            supports_language_filtering=True,
            supports_region_filtering=False,
            supports_image_search=False,
            max_results_per_request=50
        ),
        default_parameters={
            'max_results': 10,
            'language': 'en'
        }
    )
    
    return WikipediaEngine(config)
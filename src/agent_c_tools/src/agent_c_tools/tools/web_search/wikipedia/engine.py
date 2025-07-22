"""
Wikipedia search engine adapter for the unified web search system.

This module provides a Wikipedia engine implementation that conforms to the
BaseWebSearchEngine interface while wrapping the existing WikipediaTools.
"""

import json
import logging
import asyncio
from typing import Dict, Any, Optional
from datetime import datetime

from ..base.engine import BaseWebSearchEngine, EngineException
from ..base.models import SearchParameters, WebSearchConfig, EngineCapabilities, SearchType
from .tool import WikipediaTools

logger = logging.getLogger(__name__)


class WikipediaEngine(BaseWebSearchEngine):
    """
    Wikipedia search engine adapter.
    
    This engine provides educational and reference search capabilities using
    Wikipedia's search API, optimized for factual and encyclopedic content.
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
            
            # Handle different response formats from Wikipedia tool
            if isinstance(results, list):
                # Simple list of search terms/titles
                for i, item in enumerate(results[:params.max_results]):
                    if isinstance(item, str):
                        # Simple title string
                        standardized_item = {
                            'title': item,
                            'url': f"https://en.wikipedia.org/wiki/{item.replace(' ', '_')}",
                            'snippet': f"Wikipedia article about {item}",
                            'source': 'Wikipedia',
                            'metadata': {
                                'article_title': item,
                                'search_rank': i + 1,
                                'original_data': item
                            }
                        }
                    else:
                        # Dictionary with more details
                        title = item.get('title', str(item))
                        standardized_item = {
                            'title': title,
                            'url': f"https://en.wikipedia.org/wiki/{title.replace(' ', '_')}",
                            'snippet': item.get('description', f"Wikipedia article about {title}"),
                            'source': 'Wikipedia',
                            'metadata': {
                                'article_id': item.get('id'),
                                'key': item.get('key'),
                                'description': item.get('description'),
                                'search_rank': i + 1,
                                'original_data': item
                            }
                        }
                    standardized_results.append(standardized_item)
            
            elif isinstance(results, dict):
                # Single result or structured response
                title = results.get('title', params.query)
                standardized_item = {
                    'title': title,
                    'url': f"https://en.wikipedia.org/wiki/{title.replace(' ', '_')}",
                    'snippet': results.get('description', f"Wikipedia article about {title}"),
                    'source': 'Wikipedia',
                    'metadata': {
                        'article_id': results.get('id'),
                        'key': results.get('key'),
                        'description': results.get('description'),
                        'original_data': results
                    }
                }
                standardized_results.append(standardized_item)
            
            # If we have results, try to get more detailed content for top results
            if standardized_results and params.max_results <= 5:
                standardized_results = self._enhance_results_with_content(standardized_results)
            
            return {
                'results': standardized_results,
                'total_results': len(standardized_results),
                'metadata': {
                    'engine': 'wikipedia',
                    'search_type': params.search_type.value,
                    'language': 'en'  # Wikipedia tool currently only supports English
                }
            }
            
        except json.JSONDecodeError as e:
            logger.error(f"Wikipedia JSON parsing error: {e}")
            raise EngineException(f"Failed to parse Wikipedia response: {str(e)}", self.engine_name)
        except Exception as e:
            logger.error(f"Wikipedia search execution error: {e}")
            raise EngineException(f"Wikipedia search failed: {str(e)}", self.engine_name)
    
    def _enhance_results_with_content(self, results: list) -> list:
        """
        Enhance search results with actual Wikipedia content.
        
        Args:
            results: List of standardized results to enhance
            
        Returns:
            Enhanced results with content snippets
        """
        try:
            import wikipedia
            
            enhanced_results = []
            for result in results:
                try:
                    # Get the page title from metadata
                    title = result['metadata'].get('article_title', result['title'])
                    
                    # Try to get page summary
                    try:
                        summary = wikipedia.summary(title, sentences=2)
                        result['snippet'] = summary
                        result['metadata']['has_content'] = True
                    except wikipedia.exceptions.DisambiguationError as e:
                        # If disambiguation, use the first option
                        if e.options:
                            try:
                                summary = wikipedia.summary(e.options[0], sentences=2)
                                result['snippet'] = summary
                                result['title'] = e.options[0]
                                result['url'] = f"https://en.wikipedia.org/wiki/{e.options[0].replace(' ', '_')}"
                                result['metadata']['disambiguated_to'] = e.options[0]
                                result['metadata']['has_content'] = True
                            except:
                                result['metadata']['has_content'] = False
                        else:
                            result['metadata']['has_content'] = False
                    except wikipedia.exceptions.PageError:
                        # Page doesn't exist, keep original snippet
                        result['metadata']['has_content'] = False
                    except:
                        # Other errors, keep original snippet
                        result['metadata']['has_content'] = False
                    
                    enhanced_results.append(result)
                    
                except Exception as e:
                    logger.warning(f"Failed to enhance Wikipedia result for '{result['title']}': {e}")
                    result['metadata']['enhancement_error'] = str(e)
                    enhanced_results.append(result)
            
            return enhanced_results
            
        except ImportError:
            logger.warning("Wikipedia library not available for content enhancement")
            return results
        except Exception as e:
            logger.warning(f"Failed to enhance Wikipedia results: {e}")
            return results
    
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
            search_types=[SearchType.EDUCATIONAL, SearchType.WEB],
            supports_pagination=False,
            supports_date_filtering=False,
            supports_domain_filtering=False,
            supports_safe_search=False,
            supports_language_filtering=False,  # Currently only English
            supports_region_filtering=False,
            supports_image_search=False,
            supports_content_extraction=True,
            max_results_per_request=20
        ),
        default_parameters={
            'max_results': 10,
            'language': 'en'
        }
    )
    
    return WikipediaEngine(config)
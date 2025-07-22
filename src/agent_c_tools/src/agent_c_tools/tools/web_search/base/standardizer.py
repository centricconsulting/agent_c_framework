"""
Response standardization for web search results.

This module provides standardization of search results from different engines
into a consistent format for the unified web search system.
"""

from typing import Dict, Any, List, Optional
from datetime import datetime, timedelta
import json
import re
from agent_c.util.structured_logging import get_logger

from .models import SearchResult, SearchResponse, SearchType

logger = get_logger(__name__)


class ResponseStandardizer:
    """Standardizes search responses from different engines into a unified format."""
    
    def __init__(self):
        """Initialize the response standardizer."""
        self.engine_standardizers = {
            'duckduckgo': self._standardize_duckduckgo_response,
            'google': self._standardize_google_response,
            'tavily': self._standardize_tavily_response,
            'newsapi': self._standardize_newsapi_response,
            'seeking_alpha': self._standardize_seeking_alpha_response,
            'hackernews': self._standardize_hackernews_response,
            'wikipedia': self._standardize_wikipedia_response,
            'google_trends': self._standardize_google_trends_response
        }
    
    def standardize_response(
        self,
        raw_response: Any,
        engine_name: str,
        search_type: SearchType,
        query: str,
        execution_time: float
    ) -> SearchResponse:
        """
        Standardize a raw engine response into a unified SearchResponse.
        
        Args:
            raw_response: Raw response from the search engine
            engine_name: Name of the engine that produced the response
            search_type: Type of search that was performed
            query: Original search query
            execution_time: Time taken to execute the search
            
        Returns:
            Standardized SearchResponse object
        """
        try:
            # Get engine-specific standardizer
            standardizer = self.engine_standardizers.get(
                engine_name, 
                self._standardize_generic_response
            )
            
            # Standardize the response
            results, metadata = standardizer(raw_response, search_type)
            
            # Create standardized response
            response = SearchResponse(
                success=True,
                engine_used=engine_name,
                search_type=search_type.value,
                query=query,
                execution_time=execution_time,
                results=results,
                total_results=metadata.get('total_results'),
                page=metadata.get('page'),
                pages_available=metadata.get('pages_available'),
                metadata=metadata
            )
            
            logger.debug(f"Standardized response: {len(results)} results from {engine_name}")
            return response
            
        except Exception as e:
            logger.error(f"Response standardization failed for {engine_name}: {e}")
            
            # Return error response
            return SearchResponse(
                success=False,
                engine_used=engine_name,
                search_type=search_type.value,
                query=query,
                execution_time=execution_time,
                results=[],
                error={
                    'type': 'StandardizationError',
                    'message': f"Failed to standardize response: {str(e)}",
                    'engine': engine_name
                }
            )
    
    def _standardize_duckduckgo_response(
        self, 
        raw_response: Any, 
        search_type: SearchType
    ) -> tuple[List[SearchResult], Dict[str, Any]]:
        """Standardize DuckDuckGo search response."""
        results = []
        metadata = {}
        
        try:
            # Parse JSON if it's a string
            if isinstance(raw_response, str):
                data = json.loads(raw_response)
            else:
                data = raw_response
            
            # Handle list of results
            if isinstance(data, list):
                for item in data:
                    result = SearchResult(
                        title=item.get('title', ''),
                        url=item.get('href', ''),
                        snippet=item.get('body', ''),
                        source='DuckDuckGo',
                        metadata={
                            'original_data': item
                        }
                    )
                    results.append(result)
            
            metadata['total_results'] = len(results)
            
        except Exception as e:
            logger.error(f"DuckDuckGo response standardization error: {e}")
            raise
        
        return results, metadata
    
    def _standardize_google_response(
        self, 
        raw_response: Any, 
        search_type: SearchType
    ) -> tuple[List[SearchResult], Dict[str, Any]]:
        """Standardize Google/SerpAPI search response."""
        results = []
        metadata = {}
        
        try:
            data = raw_response if isinstance(raw_response, dict) else json.loads(raw_response)
            
            # Handle different Google search types
            if search_type == SearchType.WEB:
                organic_results = data.get('organic_results', [])
                for item in organic_results:
                    result = SearchResult(
                        title=item.get('title', ''),
                        url=item.get('link', ''),
                        snippet=item.get('snippet', ''),
                        published_date=self._parse_date(item.get('date')),
                        source='Google',
                        metadata={
                            'position': item.get('position'),
                            'displayed_link': item.get('displayed_link'),
                            'original_data': item
                        }
                    )
                    results.append(result)
            
            elif search_type == SearchType.NEWS:
                news_results = data.get('news_results', [])
                for item in news_results:
                    result = SearchResult(
                        title=item.get('title', ''),
                        url=item.get('link', ''),
                        snippet=item.get('snippet', ''),
                        published_date=self._parse_date(item.get('date')),
                        source=item.get('source', 'Google News'),
                        metadata={
                            'position': item.get('position'),
                            'thumbnail': item.get('thumbnail'),
                            'original_data': item
                        }
                    )
                    results.append(result)
            
            # Extract pagination info
            pagination = data.get('serpapi_pagination', {})
            metadata.update({
                'total_results': data.get('search_information', {}).get('total_results'),
                'page': pagination.get('current'),
                'pages_available': len(pagination.get('other_pages', [])) + 1,
                'search_parameters': data.get('search_parameters', {})
            })
            
        except Exception as e:
            logger.error(f"Google response standardization error: {e}")
            raise
        
        return results, metadata
    
    def _standardize_tavily_response(
        self, 
        raw_response: Any, 
        search_type: SearchType
    ) -> tuple[List[SearchResult], Dict[str, Any]]:
        """Standardize Tavily search response."""
        results = []
        metadata = {}
        
        try:
            # Parse JSON if it's a string
            if isinstance(raw_response, str):
                data = json.loads(raw_response)
            else:
                data = raw_response
            
            # Handle Tavily response format
            if isinstance(data, list):
                # Handle list format from existing tool
                for item in data:
                    result = SearchResult(
                        title=self._extract_title_from_url(item.get('href', '')),
                        url=item.get('href', ''),
                        snippet=self._truncate_content(item.get('body', '')),
                        score=item.get('score'),
                        source='Tavily',
                        metadata={
                            'raw_content': item.get('body'),
                            'original_data': item
                        }
                    )
                    results.append(result)
            
            elif isinstance(data, dict):
                # Handle standard Tavily API response
                search_results = data.get('results', [])
                for item in search_results:
                    result = SearchResult(
                        title=item.get('title', ''),
                        url=item.get('url', ''),
                        snippet=item.get('content', ''),
                        score=item.get('score'),
                        published_date=self._parse_date(item.get('published_date')),
                        source='Tavily',
                        metadata={
                            'raw_content': item.get('raw_content'),
                            'original_data': item
                        }
                    )
                    results.append(result)
                
                metadata.update({
                    'query': data.get('query'),
                    'answer': data.get('answer'),
                    'follow_up_questions': data.get('follow_up_questions'),
                    'images': data.get('images', [])
                })
            
            metadata['total_results'] = len(results)
            
        except Exception as e:
            logger.error(f"Tavily response standardization error: {e}")
            raise
        
        return results, metadata
    
    def _standardize_newsapi_response(
        self, 
        raw_response: Any, 
        search_type: SearchType
    ) -> tuple[List[SearchResult], Dict[str, Any]]:
        """Standardize NewsAPI search response."""
        results = []
        metadata = {}
        
        try:
            data = raw_response if isinstance(raw_response, dict) else json.loads(raw_response)
            
            articles = data.get('articles', [])
            for article in articles:
                result = SearchResult(
                    title=article.get('title', ''),
                    url=article.get('url', ''),
                    snippet=article.get('description', ''),
                    published_date=self._parse_date(article.get('publishedAt')),
                    source=article.get('source', {}).get('name', 'NewsAPI'),
                    metadata={
                        'author': article.get('author'),
                        'url_to_image': article.get('urlToImage'),
                        'content': article.get('content'),
                        'source_id': article.get('source', {}).get('id'),
                        'original_data': article
                    }
                )
                results.append(result)
            
            metadata.update({
                'status': data.get('status'),
                'total_results': data.get('totalResults'),
                'articles_returned': len(articles)
            })
            
        except Exception as e:
            logger.error(f"NewsAPI response standardization error: {e}")
            raise
        
        return results, metadata
    
    def _standardize_seeking_alpha_response(
        self, 
        raw_response: Any, 
        search_type: SearchType
    ) -> tuple[List[SearchResult], Dict[str, Any]]:
        """Standardize Seeking Alpha search response."""
        results = []
        metadata = {}
        
        try:
            # This would be implemented based on Seeking Alpha's actual response format
            # For now, provide a generic implementation
            data = raw_response if isinstance(raw_response, dict) else json.loads(raw_response)
            
            # Handle articles or search results
            articles = data.get('articles', data.get('results', []))
            for article in articles:
                result = SearchResult(
                    title=article.get('title', ''),
                    url=article.get('url', article.get('link', '')),
                    snippet=article.get('summary', article.get('description', '')),
                    published_date=self._parse_date(article.get('publish_on', article.get('date'))),
                    source='Seeking Alpha',
                    metadata={
                        'author': article.get('author'),
                        'symbols': article.get('symbols', []),
                        'themes': article.get('themes', []),
                        'original_data': article
                    }
                )
                results.append(result)
            
            metadata['total_results'] = len(results)
            
        except Exception as e:
            logger.error(f"Seeking Alpha response standardization error: {e}")
            raise
        
        return results, metadata
    
    def _standardize_hackernews_response(
        self, 
        raw_response: Any, 
        search_type: SearchType
    ) -> tuple[List[SearchResult], Dict[str, Any]]:
        """Standardize Hacker News search response."""
        results = []
        metadata = {}
        
        try:
            data = raw_response if isinstance(raw_response, dict) else json.loads(raw_response)
            
            # Handle HN API response format
            hits = data.get('hits', [])
            for hit in hits:
                # Determine URL - prefer story URL, fall back to HN discussion
                url = hit.get('url') or f"https://news.ycombinator.com/item?id={hit.get('objectID')}"
                
                result = SearchResult(
                    title=hit.get('title', ''),
                    url=url,
                    snippet=hit.get('comment_text', hit.get('story_text', '')),
                    published_date=self._parse_date(hit.get('created_at')),
                    source='Hacker News',
                    metadata={
                        'author': hit.get('author'),
                        'points': hit.get('points'),
                        'num_comments': hit.get('num_comments'),
                        'story_id': hit.get('story_id'),
                        'object_id': hit.get('objectID'),
                        'tags': hit.get('_tags', []),
                        'original_data': hit
                    }
                )
                results.append(result)
            
            metadata.update({
                'total_results': data.get('nbHits'),
                'page': data.get('page'),
                'pages_available': data.get('nbPages'),
                'processing_time': data.get('processingTimeMS'),
                'query': data.get('query')
            })
            
        except Exception as e:
            logger.error(f"Hacker News response standardization error: {e}")
            raise
        
        return results, metadata
    
    def _standardize_wikipedia_response(
        self, 
        raw_response: Any, 
        search_type: SearchType
    ) -> tuple[List[SearchResult], Dict[str, Any]]:
        """Standardize Wikipedia search response."""
        results = []
        metadata = {}
        
        try:
            data = raw_response if isinstance(raw_response, dict) else json.loads(raw_response)
            
            # Handle Wikipedia API response format
            pages = data.get('query', {}).get('pages', {})
            for page_id, page in pages.items():
                if page_id != '-1':  # Skip missing pages
                    result = SearchResult(
                        title=page.get('title', ''),
                        url=f"https://en.wikipedia.org/wiki/{page.get('title', '').replace(' ', '_')}",
                        snippet=page.get('extract', ''),
                        source='Wikipedia',
                        metadata={
                            'page_id': page.get('pageid'),
                            'namespace': page.get('ns'),
                            'thumbnail': page.get('thumbnail'),
                            'original_data': page
                        }
                    )
                    results.append(result)
            
            metadata['total_results'] = len(results)
            
        except Exception as e:
            logger.error(f"Wikipedia response standardization error: {e}")
            raise
        
        return results, metadata
    
    def _standardize_google_trends_response(
        self, 
        raw_response: Any, 
        search_type: SearchType
    ) -> tuple[List[SearchResult], Dict[str, Any]]:
        """Standardize Google Trends search response."""
        results = []
        metadata = {}
        
        try:
            data = raw_response if isinstance(raw_response, dict) else json.loads(raw_response)
            
            # Handle trending topics
            trending_searches = data.get('trending_searches', data.get('default', {}).get('trending_searches', []))
            for trend in trending_searches:
                result = SearchResult(
                    title=trend.get('title', trend.get('query', '')),
                    url=trend.get('explore_link', ''),
                    snippet=f"Traffic: {trend.get('formattedTraffic', 'N/A')}",
                    source='Google Trends',
                    metadata={
                        'traffic': trend.get('formattedTraffic'),
                        'related_queries': trend.get('relatedQueries', []),
                        'articles': trend.get('articles', []),
                        'original_data': trend
                    }
                )
                results.append(result)
            
            metadata.update({
                'total_results': len(results),
                'country': data.get('country'),
                'date': data.get('date')
            })
            
        except Exception as e:
            logger.error(f"Google Trends response standardization error: {e}")
            raise
        
        return results, metadata
    
    def _standardize_generic_response(
        self, 
        raw_response: Any, 
        search_type: SearchType
    ) -> tuple[List[SearchResult], Dict[str, Any]]:
        """Generic standardization for unknown engine responses."""
        results = []
        metadata = {}
        
        try:
            # Try to parse as JSON if string
            if isinstance(raw_response, str):
                try:
                    data = json.loads(raw_response)
                except json.JSONDecodeError:
                    # If not JSON, treat as plain text
                    result = SearchResult(
                        title="Text Response",
                        url="",
                        snippet=raw_response[:500],
                        source="Unknown Engine"
                    )
                    return [result], {'total_results': 1}
            else:
                data = raw_response
            
            # Try to extract results from common response formats
            if isinstance(data, list):
                # Direct list of results
                for item in data:
                    if isinstance(item, dict):
                        result = self._extract_generic_result(item)
                        if result:
                            results.append(result)
            
            elif isinstance(data, dict):
                # Look for common result containers
                for key in ['results', 'items', 'data', 'hits', 'articles']:
                    if key in data and isinstance(data[key], list):
                        for item in data[key]:
                            if isinstance(item, dict):
                                result = self._extract_generic_result(item)
                                if result:
                                    results.append(result)
                        break
                
                # Extract metadata
                metadata.update({
                    'total_results': data.get('total', data.get('count', len(results))),
                    'page': data.get('page', data.get('current_page')),
                    'original_response_keys': list(data.keys())
                })
            
        except Exception as e:
            logger.error(f"Generic response standardization error: {e}")
            # Return empty results rather than failing
            pass
        
        return results, metadata
    
    def _extract_generic_result(self, item: Dict[str, Any]) -> Optional[SearchResult]:
        """Extract a SearchResult from a generic item dictionary."""
        try:
            # Try to find title
            title = (item.get('title') or item.get('name') or 
                    item.get('headline') or item.get('summary', ''))[:200]
            
            # Try to find URL
            url = (item.get('url') or item.get('link') or 
                  item.get('href') or item.get('uri', ''))
            
            # Try to find snippet/description
            snippet = (item.get('snippet') or item.get('description') or 
                      item.get('content') or item.get('body') or 
                      item.get('summary', ''))[:500]
            
            if not title and not url and not snippet:
                return None
            
            return SearchResult(
                title=title or "Untitled",
                url=url,
                snippet=snippet,
                published_date=self._parse_date(item.get('date', item.get('published_date'))),
                score=item.get('score', item.get('relevance')),
                source="Generic Engine",
                metadata={'original_data': item}
            )
            
        except Exception as e:
            logger.warning(f"Failed to extract generic result: {e}")
            return None
    
    def _parse_date(self, date_str: Optional[str]) -> Optional[datetime]:
        """Parse various date formats into datetime objects."""
        if not date_str:
            return None
        
        try:
            # Common date formats
            formats = [
                '%Y-%m-%dT%H:%M:%SZ',
                '%Y-%m-%dT%H:%M:%S',
                '%Y-%m-%d %H:%M:%S',
                '%Y-%m-%d',
                '%m/%d/%Y',
                '%d/%m/%Y',
                '%Y-%m-%dT%H:%M:%S.%fZ'
            ]
            
            for fmt in formats:
                try:
                    return datetime.strptime(date_str, fmt)
                except ValueError:
                    continue
            
            # Try to handle relative dates like "2 hours ago"
            relative_match = re.search(r'(\d+)\s*(hour|day|week|month)s?\s*ago', date_str.lower())
            if relative_match:
                amount = int(relative_match.group(1))
                unit = relative_match.group(2)
                
                now = datetime.now()
                if unit == 'hour':
                    return now - timedelta(hours=amount)
                elif unit == 'day':
                    return now - timedelta(days=amount)
                elif unit == 'week':
                    return now - timedelta(weeks=amount)
                elif unit == 'month':
                    return now - timedelta(days=amount * 30)
            
            logger.warning(f"Could not parse date: {date_str}")
            return None
            
        except Exception as e:
            logger.warning(f"Date parsing error: {e}")
            return None
    
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
                title = re.sub(r'\.[^.]+$', '', title)  # Remove extension
                title = re.sub(r'[-_]', ' ', title)     # Replace separators
                title = title.replace('%20', ' ')       # URL decode spaces
                return title.title()
            
            # Fall back to domain name
            domain = parsed.netloc.replace('www.', '')
            return domain.title()
            
        except Exception:
            return url[:50] + "..." if len(url) > 50 else url
    
    def _truncate_content(self, content: str, max_length: int = 500) -> str:
        """Truncate content to a reasonable snippet length."""
        if not content:
            return ""
        
        if len(content) <= max_length:
            return content
        
        # Try to truncate at a sentence boundary
        truncated = content[:max_length]
        last_sentence = truncated.rfind('.')
        if last_sentence > max_length * 0.7:  # If we can keep most of the content
            return truncated[:last_sentence + 1]
        
        # Fall back to word boundary
        last_space = truncated.rfind(' ')
        if last_space > max_length * 0.8:
            return truncated[:last_space] + "..."
        
        return truncated + "..."
import json
import logging
import os
import aiohttp
from typing import Dict, Any, List
from agent_c.toolsets import Toolset, json_schema


class PubMedTools(Toolset):
    """
    Toolset for searching PubMed articles via the NCBI E-utilities API.
    """
    CACHE_EXPIRY = 86400 # 1 day
    def __init__(self, **kwargs):
        super().__init__(**kwargs, name='pubmed', needed_keys=['PUBMED_API_KEY'])
        self.logger: logging.Logger = logging.getLogger(__name__)
        self.base_url = 'https://eutils.ncbi.nlm.nih.gov/entrez/eutils'
        self.api_key = os.environ.get('PUBMED_API_KEY')

    @json_schema(
        'Search PubMed for medical literature and research articles',
        {
            'query': {
                'type': 'string',
                'description': 'Search query to find medical literature in PubMed',
                'required': True
            },
            'max_results': {
                'type': 'integer',
                'description': 'Maximum number of results to return (default: 10)',
                'required': False,
                'default': 10
            }
        }
    )
    async def get_articles(self, **kwargs) -> str:
        """
        Search for articles in PubMed based on a query.

        Args:
            query: The search term to find relevant articles
            max_results: Maximum number of results to return (default: 10)

        Returns:
            A JSON string containing the list of articles found
        """
        query = kwargs.get('query')
        max_results = kwargs.get('max_results', 10)

        if not query:
            return json.dumps({"error": "Query parameter is required for PubMed search"})

        # Create cache key for this query
        cache_key = f"pubmed:search:{query}:{max_results}"

        # Check if we have cached results
        cached_result = None
        if self.tool_cache:
            cached_result = self.tool_cache.get(cache_key)
            if cached_result:
                self.logger.info(f"Using cached PubMed results for query: {query}")
                return cached_result

        try:
            # Search and fetch articles
            articles = await self.search_articles(query, max_results)

            # Format the result
            result = json.dumps({"articles": articles})

            # Cache the result
            if self.tool_cache:
                self.tool_cache.set(cache_key, result, self.CACHE_EXPIRY)  # Cache for 1 hour

            return result

        except Exception as e:
            self.logger.error(f"Error searching PubMed: {str(e)}")
            return json.dumps({"error": f"Failed to search PubMed: {str(e)}"})

    async def search_articles(self, query: str, max_results: int = 10) -> List[Dict[str, Any]]:
        """
        Search for articles in PubMed and fetch their details.

        Args:
            query: The search term
            max_results: Maximum number of results to return

        Returns:
            List of article details
        """
        # Step 1: Search for article IDs
        pmids = await self._search_pubmed_ids(query, max_results)

        if not pmids:
            return []

        # Step 2: Get article details
        return await self._fetch_article_details(pmids)

    async def _search_pubmed_ids(self, query: str, max_results: int) -> List[str]:
        """
        Search PubMed for article IDs matching the query.

        Args:
            query: The search term
            max_results: Maximum number of results

        Returns:
            List of PubMed IDs (PMIDs)
        """
        search_url = f"{self.base_url}/esearch.fcgi"
        params = {
            'db': 'pubmed',
            'term': query,
            'retmax': str(max_results),
            'retmode': 'json',
            'api_key': self.api_key
        }

        async with aiohttp.ClientSession() as session:
            async with session.get(search_url, params=params) as response:
                if response.status != 200:
                    error_text = await response.text()
                    self.logger.error(f"PubMed search error: {error_text}")
                    raise RuntimeError(f"PubMed API error: {response.status}")

                data = await response.json()
                return data.get('esearchresult', {}).get('idlist', [])

    async def _fetch_article_details(self, pmids: List[str]) -> List[Dict[str, Any]]:
        """
        Fetch details for a list of PubMed article IDs.

        Args:
            pmids: List of PubMed IDs to fetch details for

        Returns:
            List of article details
        """
        if not pmids:
            return []

        summary_url = f"{self.base_url}/esummary.fcgi"
        params = {
            'db': 'pubmed',
            'id': ','.join(pmids),
            'retmode': 'json',
            'api_key': self.api_key
        }

        async with aiohttp.ClientSession() as session:
            async with session.get(summary_url, params=params) as response:
                if response.status != 200:
                    error_text = await response.text()
                    self.logger.error(f"PubMed summary error: {error_text}.  PubMed API error: {response.status}")
                    return []

                data = await response.json()
                result = data.get('result', {})

                articles = []
                for pmid in pmids:
                    article_data = result.get(pmid, {})
                    if article_data:
                        authors = []
                        for author in article_data.get('authors', []):
                            if isinstance(author, dict) and 'name' in author:
                                authors.append(author['name'])

                        articles.append({
                            'title': article_data.get('title', ''),
                            'authors': authors,
                            'journal': article_data.get('fulljournalname', ''),
                            'pubDate': article_data.get('pubdate', ''),
                            'doi': article_data.get('elocationid', ''),
                            'abstract': article_data.get('abstract', ''),
                            'pmid': pmid
                        })

                return articles


# Register the toolset
Toolset.register(PubMedTools)
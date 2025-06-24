import json
import aiohttp
import logging
import os
from typing import Dict, Any
from urllib.parse import urlencode

from agent_c.toolsets import Toolset, json_schema


class FDANDCTools(Toolset):
    """
    Toolset for accessing the FDA API to retrieve drug information from the FDA's NDC directory (national drug code).
    Other tools could be made for other fda directories - see https://open.fda.gov/apis/drug/ndc/ for details.
    Forms a queries like https://api.fda.gov/drug/ndc.json?api_key=api_key&search=generic_name:ibuprofen&limit=3
    """

    def __init__(self, **kwargs):
        super().__init__(**kwargs, name='fda_ndc_registry', needed_keys=['FDA_API_KEY'])
        self.logger = logging.getLogger(__name__)
        self.base_url = 'https://api.fda.gov/drug/ndc.json'
        self.fda_api_key = os.environ.get('FDA_API_KEY')

    @json_schema(
        'Get drug information by generic name from the FDA database.',
        {
            'generic_name': {
                'type': 'string',
                'description': 'The generic name of the drug to search for',
                'required': True
            },
            'limit': {
                'type': 'integer',
                'description': 'The maximum number of results to return. If set to 0, will return all results.',
                'default': 0,
                'required': False
            }
        }
    )
    async def get_drug_info(self, **kwargs) -> str:
        """
        Search for drug information by generic name in the FDA database.

        Args:
            kwargs:
                generic_name: The generic name of the drug to search for
                limit: the number of results to return. 0 returns all results

        Returns:
            A JSON string containing information about the drug
        """
        self.logger.info(f"Getting drug information with parameters: {kwargs}")
        generic_name = kwargs.get('generic_name')
        limit = kwargs.get('limit', 0)

        if not generic_name:
            return json.dumps({"error": "Generic name is required"})

        # Create cache key
        cache_key = f"fda:drug:{generic_name}_{limit}"

        # Try to get from cache first
        cached_result = self.tool_cache.get(cache_key) if self.tool_cache else None
        if cached_result:
            self.logger.info(f"Retrieved drug info from cache for {generic_name}")
            return cached_result

        # If not in cache, fetch from FDA API
        try:
            results = await self._search_generic_name(generic_name, limit)
            result_json = json.dumps(results)

            # Cache the result
            if self.tool_cache:
                self.tool_cache.set(cache_key, result_json)

            return result_json

        except Exception as e:
            self.logger.error(f"Error fetching drug information: {str(e)}")
            return json.dumps({"error": f"Failed to fetch drug information: {str(e)}"})

    async def _search_generic_name(self, generic_name: str, limit: int = 0) -> Dict[str, Any]:
        """
        Search the FDA API for a drug by generic name.

        Args:
            generic_name: The generic name of the drug to search for
            limit: The maximum number of results to return. If set to 0, will return all results.

        Returns:
            Dictionary containing the drug information
        """
        query_params = {'search': f'generic_name:"{generic_name}"', 'api_key': self.fda_api_key}

        if limit > 0:
            query_params['limit'] = limit

        url = f"{self.base_url}?{urlencode(query_params)}"

        async with aiohttp.ClientSession() as session:
            async with session.get(url) as response:
                if response.status != 200:
                    error_text = await response.text()
                    self.logger.error(f"FDA API error: {response.status} - {error_text}")
                    return {"error": f"FDA API returned status {response.status}"}

                data = await response.json()
                return data.get('results', [])



Toolset.register(FDANDCTools)

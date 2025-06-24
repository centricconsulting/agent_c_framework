import json
import logging
import aiohttp
from typing import Dict, Any, List, Optional
from agent_c.toolsets import Toolset, json_schema


class ClinicalTrialsTools(Toolset):
    """
    Toolset for searching clinical trials via the ClinicalTrials.gov API.
    """

    def __init__(self, **kwargs):
        super().__init__(**kwargs, name='clinical_trials')
        self.logger: logging.Logger = logging.getLogger(__name__)
        self.base_url = 'https://clinicaltrials.gov/api/v2/studies'

    @json_schema(
        'Search for clinical trials by condition and optional location',
        {
            'condition': {
                'type': 'string',
                'description': 'Medical condition to search for clinical trials',
                'required': True
            },
            'location': {
                'type': 'string',
                'description': 'Geographic location to filter trials (e.g., "New York", "Boston")',
                'required': False
            }
        }
    )
    async def get_trials(self, **kwargs) -> str:
        """
        Search for clinical trials based on condition and optional location.

        Args:
            condition: The medical condition to search for
            location: Optional geographic location to filter trials

        Returns:
            A JSON string containing the list of clinical trials found
        """
        condition = kwargs.get('condition')
        location = kwargs.get('location')

        if not condition:
            return json.dumps({"error": "Condition parameter is required"})

        # Create cache key for this query
        cache_key = f"trials:search:{condition}:{location or 'any'}"

        # Check if we have cached results
        cached_result = None
        if self.tool_cache:
            cached_result = self.tool_cache.get(cache_key)
            if cached_result:
                self.logger.info(
                    f"Using cached Clinical Trials results for condition: {condition}, location: {location}")
                return cached_result

        try:
            # Search for trials
            trials = await self.search_trials(condition, location)

            # Format the result
            result = json.dumps({"trials": trials})

            # Cache the result
            if self.tool_cache:
                self.tool_cache.set(cache_key, result, 3600)  # Cache for 1 hour

            return result

        except Exception as e:
            self.logger.error(f"Error searching Clinical Trials: {str(e)}")
            return json.dumps({"error": f"Failed to search Clinical Trials: {str(e)}"})

    async def search_trials(self, condition: str, location: Optional[str] = None) -> List[Dict[str, Any]]:
        """
        Search for clinical trials based on condition and optional location.

        Args:
            condition: The medical condition
            location: Optional geographic location

        Returns:
            List of clinical trial details
        """
        url = self.base_url
        params = {'query.cond': condition}

        if location:
            params['query.locn'] = location

        async with aiohttp.ClientSession() as session:
            async with session.get(url, params=params) as response:
                if response.status != 200:
                    error_text = await response.text()
                    self.logger.error(f"Clinical Trials search error: {error_text}")
                    raise RuntimeError(f"Clinical Trials API error: {response.status}")

                data = await response.json()
                # TODO: data contains a nextPageToken. We're not using it.  But we could in the future.
                studies = data.get('studies', [])

                formatted_trials = []
                for study in studies:
                    formatted_trial = {
                        'nctId': study.get('protocolSection', {}).get('identificationModule', {}).get('nctId', ''),
                        'title': study.get('protocolSection', {}).get('identificationModule', {}).get('briefTitle', ''),
                        'status': study.get('protocolSection', {}).get('statusModule', {}).get('overallStatus', ''),
                        'phase': study.get('protocolSection', {}).get('designModule', {}).get('phases', []),
                        'conditions': study.get('protocolSection', {}).get('conditionsModule', {}).get('conditions',
                                                                                                       []),
                        'locations': self._extract_locations(study),
                        'lastUpdated': study.get('protocolSection', {}).get('statusModule', {}).get(
                            'lastUpdateSubmitDate', '')
                    }
                    formatted_trials.append(formatted_trial)

                return formatted_trials

    def _extract_locations(self, study: Dict[str, Any]) -> List[str]:
        """Extract location information from study data"""
        locations = []
        try:
            location_data = study.get('protocolSection', {}).get('contactsLocationsModule', {}).get('locations', [])
            for loc in location_data:
                facility = loc.get('facility', {}).get('name', '')
                city = loc.get('city', '')
                state = loc.get('state', '')
                country = loc.get('country', '')

                location_str = ', '.join(filter(None, [facility, city, state, country]))
                if location_str:
                    locations.append(location_str)
        except Exception as e:
            self.logger.error(f"Error extracting locations: {str(e)}")

        return locations


# Register the toolset
Toolset.register(ClinicalTrialsTools)
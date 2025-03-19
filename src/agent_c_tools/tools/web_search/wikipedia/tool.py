import json

import wikipedia

from agent_c_core.agent_c import Toolset, json_schema
from agent_c_core.agent_c.util import filter_dict_by_keys

class WikipediaSearch(Toolset):

    def __init__(self, **kwargs):
        super().__init__(name='wikipedia', **kwargs)

    @json_schema(
        'Perform a wikipedia search using this tool',
        {
            'search': {
                'type': 'string',
                'description': 'Query input to search Wikipedia',
                'required': True
            },
        }
    )
    async def search_wiki(self, **kwargs):
        search_query = kwargs.get('search')
        wanted_keys = [
            'id',
            'key',
            'title',
            'description'
        ]
        results = wikipedia.search(search_query)
        filtered_results = filter_dict_by_keys(results, wanted_keys)
        # TODO: This tool needs refactored to better search wikipedia - see https://medium.com/@shashankvats/building-a-wikipedia-search-engine-with-langchain-and-streamlit-d63cb11181d0 for ideas
        if filtered_results is None or len(filtered_results) == 0:
            # if you search a generic term, you'll get a list of search results keywords returned
            # Causing filtered_results to be empty or none
            return json.dumps(results)
        else:
            return json.dumps(filtered_results)


Toolset.register(WikipediaSearch)

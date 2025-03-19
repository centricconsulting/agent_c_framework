import json
from duckduckgo_search import AsyncDDGS
from agent_c.toolsets import json_schema, Toolset

# A tool belt for asynchronous search functionality using the DuckDuckGo search API.
class DuckDuckGoTools(Toolset):
    def __init__(self, **kwargs):
        super().__init__(**kwargs, name='duckduckgo')

    @json_schema(
        'Perform a web search using DuckDuckGo.',
        {
            'query': {
                'type': 'string',
                'description': 'The search query to perform.',
                'required': True
            },
            'max_results': {
                'type': 'integer',
                'description': 'The maximum number of search results to return. Defaults to 20.',
                'required': False
            },
            'safesearch': {
                'enum': ['on', 'moderate', 'off'],
                'description': 'Set the safe search level for adult content, defaults to moderate.',
                'required': False
            }
        }
    )
    async def web_search(self, **kwargs):
        query = kwargs.get('query')
        max_results = kwargs.get('max_results', 20)
        safesearch = kwargs.get('safesearch', 'moderate')

        results = await AsyncDDGS().text(query, max_results=max_results, safesearch=safesearch)

        return json.dumps(results)


Toolset.register(DuckDuckGoTools)

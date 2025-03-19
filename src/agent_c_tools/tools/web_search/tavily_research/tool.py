import os
import json

from tavily import TavilyClient

from agent_c_core.agent_c import json_schema, Toolset
from agent_c_tools.tools.web_search.tavily_research.prompt import TavilyResearchPrompt


class TavilyTools(Toolset):

    def __init__(self, **kwargs):
        super().__init__(**kwargs, name='tavily', required_tools=['workspace'], needed_keys=['TAVILI_API_KEY'])
        self.section = TavilyResearchPrompt()

        self.api_key = kwargs.get('api_key', os.environ['TAVILI_API_KEY'])
        self.client = TavilyClient(api_key=self.api_key)

    @json_schema(
        description="Perform Tavily based research using Tavily's API and return the combined body of the results.  "
                    "Only use this tool when web research is requested. Do NOT use unless the user requests this tool. "
                    "The tool response will be a json string with the url, body, and score",
        params={
            'query': {
                'type': 'string',
                'description': 'The search query from the user',
                'required': True
            },
            'search_depth': {
                'type': 'string',
                'description': 'The depth of the search (e.g., "advanced")',
                'required': True,
                'default': 'advanced'
            },
            'max_results': {
                'type': 'integer',
                'description': 'The maximum number of results to return',
                'required': True,
                'default': 5,
            },
            'include_images': {
                'type': 'boolean',
                'description': 'Whether to include images in the results',
                'required': False,
                'default': False
            },
            'include_answer': {
                'type': 'boolean',
                'description': 'Whether to include answers in the results',
                'required': False,
                'default': False
            },
            'include_raw_content': {
                'type': 'boolean',
                'description': 'Whether to include raw content in the results',
                'required': False,
                'default': True
            },
            'include_domains': {
                'type': 'array',
                'items': {
                    'type': 'string'
                },
                'description': 'List of domains to include in the search',
                'required': False,
                'default': None
            },
            'exclude_domains': {
                'type': 'array',
                'items': {
                    'type': 'string'
                },
                'description': 'List of domains to exclude from the search',
                'required': False,
                'default': None
            }
        }
    )
    async def search_tavily(self, **kwargs):
        query = kwargs.get('query')
        search_depth = kwargs.get('search_depth')
        max_results = kwargs.get('max_results', 5)
        include_images = kwargs.get('include_images', False)
        include_answer = kwargs.get('include_answer', False)
        include_raw_content = kwargs.get('include_raw_content', True)
        include_domains = kwargs.get('include_domains', None)
        exclude_domains = kwargs.get('exclude_domains', None)

        response = self.client.search(
            query=query,
            search_depth=search_depth,
            max_results=max_results,
            include_images=include_images,
            include_answer=include_answer,
            include_raw_content=include_raw_content,
            include_domains=include_domains,
            exclude_domains=exclude_domains
        )

        sources = response.get("results", [])
        search_response = [{"href": obj["url"], "body": obj["raw_content"], "score": obj["score"]} for obj in sources]

        return json.dumps(search_response)


Toolset.register(TavilyTools)

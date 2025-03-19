import json
import requests
from agent_c_core.agent_c.toolsets import json_schema, Toolset


# A tool belt for asynchronous search functionality using the HackerNews search API.
class HackerNewsSearch(Toolset):

    def __init__(self, **kwargs):
        super().__init__(**kwargs, name='hacker_news')
        self.base_url = 'https://hacker-news.firebaseio.com/v0/'

    @json_schema(
        'Call this tool to get top stories from hacker news',
        {
            'limit': {
                'type': 'integer',
                'description': 'Number of Top Stories to display, Default to 10',
                'required': True
            }
        }
    )
    async def get_top_stories(self, **kwargs):
        result_count = kwargs.get('limit', 10)
        top_story_url = f"{self.base_url}/topstories.json?print=pretty"
        stories_ids = self.fetch_stories_ids(top_story_url, result_count)
        results = self.fetch_stories(stories_ids)
        return json.dumps(results)

    @json_schema(
        'Call this tool to get job stories from hacker news',
        {
            'limit': {
                'type': 'integer',
                'description': 'Number of Job Stories to display, Default to 10',
                'required': True
            }
        }
    )
    async def get_job_stories(self, **kwargs):
        result_count = kwargs.get('limit', 10)
        job_story_url = f"{self.base_url}/jobstories.json?print=pretty"
        stories_ids = self.fetch_stories_ids(job_story_url, result_count)
        results = self.fetch_stories(stories_ids)
        return json.dumps(results)

    def fetch_stories_ids(self, url, limit):
        response = requests.get(url)
        stories = response.content.decode('utf-8')
        return json.loads(stories)[:limit]

    def fetch_stories(self, stories_ids):
        stories_data = []
        for story_id in stories_ids:
            story_url = f"{self.base_url}/item/{story_id}.json?print=pretty"
            response = requests.get(story_url)
            try:
                story_data = response.json()
                title = story_data['title']
                if title:
                    stories_data.append({'title': title})
            except Exception as e:
                return json.dumps({'error': str(e)})
        return stories_data


Toolset.register(HackerNewsSearch)

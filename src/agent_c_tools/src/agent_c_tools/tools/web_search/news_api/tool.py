import os
import re
import json
import logging

from newsapi.newsapi_client import NewsApiClient

from agent_c.toolsets import json_schema, Toolset

class NewsApiTools(Toolset):
    def __init__(self, **kwargs):
        super().__init__(**kwargs, name='newsapiorg', needed_keys=['NEWSAPI_API_KEY'])
        api_key = kwargs.get('api_key', os.environ['NEWSAPI_API_KEY'])
        self.newsapi = NewsApiClient(api_key=api_key)
        self.logger: logging.Logger = logging.getLogger(__name__)

    @json_schema(
        description="Get the top headlines from NewsAPI.  This does not have a date range option.",
        params={
            'category': {
                'type': 'string',
                'description': 'This is a comma separated list to get headlines for. Valid categories are business, entertainment, general, health, science, sports, technology.',
                'required': False,
                'validation': '^(business|entertainment|general|health|science|sports|technology)(,(business|entertainment|general|health|science|sports|technology))*$'
            },
            'pageSize': {
                'type': 'integer',
                'description': 'The number of results to return per page. Default is 20, maximum is 100.',
                'required': False,
                'default': 20,
            },
            'page': {
                'type': 'integer',
                'description': 'The page number to return for the headlines. Default is 1.',
                'required': False,
                'default': 1
            }
        }
    )
    async def get_top_headlines(self, **kwargs)->str:
        category = kwargs.get('category', None)
        page_size = kwargs.get('pageSize', 20)
        page = kwargs.get('page', 1)

        pattern = r'^(business|entertainment|general|health|science|sports|technology)(,(business|entertainment|general|health|science|sports|technology))*$'
        if category is not None and not re.match(pattern, category):
            return "error: Invalid category. Valid categories are business, entertainment, general, health, science, sports, technology."

        try:
            headlines = self.newsapi.get_top_headlines(
                category=category,
                page_size=page_size,
                page=page,
                language="en",
            )
            return json.dumps(headlines)
        except Exception as e:
            self.logger.error(f"Error fetching top headlines: {e}")
            return f"Error fetching top headlines: {str(e)}"

    @json_schema(
        description="Get a list of news sources from NewsAPI",
        params={
            'category': {
                'type': 'string',
                'description': 'This is a comma separated list to get headlines for. Valid categories are business, entertainment, general, health, science, sports, technology.',
                'required': False,
                'validation': '^(business|entertainment|general|health|science|sports|technology)(,(business|entertainment|general|health|science|sports|technology))*$'
            }
        }
    )
    async def get_sources(self, **kwargs)->str:
        category = kwargs.get('category', None)
        pattern = r'^(business|entertainment|general|health|science|sports|technology)(,(business|entertainment|general|health|science|sports|technology))*$'
        if category is not None and not (re.match(pattern, category)):
            return "error: Invalid category. Valid categories are business, entertainment, general, health, science, sports, technology."

        try:
            sources = self.newsapi.get_sources(category=category, language="en")
            return json.dumps(sources)
        except Exception as e:
            self.logger.error(f"Error fetching sources: {e}")
            return f"Error fetching sources: {str(e)}"

    @json_schema(
        description="Get all articles from NewsAPI",
        params={
            'q': {
                'type': 'string',
                'description': 'The query string to search for articles. Must be less than 500 characters',
                'required': True
            },
            'start_date': {
                'type': 'string',
                'description': 'A date and optional time for the oldest article allowed. format must conform to ISO-8601 specifically as one of either %Y-%m-%d (e.g. 2019-09-07) or %Y-%m-%dT%H:%M:%S (e.g. 2019-09-07T13:04:15)',
                'required': False
            },
            'end_date': {
                'type': 'string',
                'description': 'A date and optional time for the oldest article allowed. format must conform to ISO-8601 specifically as one of either %Y-%m-%d (e.g. 2019-09-07) or %Y-%m-%dT%H:%M:%S (e.g. 2019-09-07T13:04:15)',
                'required': False
            },
            'max_articles': {
                'type': 'integer',
                'description': 'The maximum number of articles to return back to the user. Default is 10.',
                'required': False,
                'default': 10
            },
            'pageSize': {
                'type': 'integer',
                'description': 'The number of results to return per API page call. Default is 20, maximum is 100.',
                'required': False
            },
            'page': {
                'type': 'integer',
                'description': 'The page number to return for the paginated list of articles. Default is 1.',
                'required': False
            },
            'sort': {
                'type': 'string',
                'description': 'The order to sort the articles in. Valid options are relevancy, popularity, publishedAt. Default is relevancy.',
                'required': False,
                'validation': '^(relevancy|popularity|publishedAt)$',
                'default': 'relevancy'
            }
        }
    )
    async def get_all_articles(self, **kwargs)->str:
        query = kwargs.get('q')
        start_date = kwargs.get('start_date', None)
        end_date = kwargs.get('end_date', None)
        page_size = kwargs.get('pageSize', 20)
        page = kwargs.get('page', 1)
        sort = kwargs.get('sort', 'relevancy')
        max_articles = kwargs.get('max_articles', 10)

        if len(query) > 500:
            return "error: Query parameter 'q' must be less than 500 characters."

        try:
            articles = self.newsapi.get_everything(
                q=query,
                from_param=start_date,
                to=end_date,
                page_size=page_size,
                page=page,
                sort_by=sort,
                language="en",
            )
            self.logger.info(f"Found {articles['totalResults']} articles for query: {query}")
            return json.dumps({'query': query, 'total_articles': articles['totalResults'], 'articles': articles['articles'][:max_articles]})  # Return only the max_articles
        except Exception as e:
            self.logger.error(f"Error fetching articles: {e}")
            # if 'You are trying to request results too far in the past' in (str(e)):
            #     response = json.dumps({'error': "The date range is too far in the past. The date can be no more than 29 days in the past. Unless you have the paid plan", 'status': e.args[0]['code']})
            # else:
            #     response = json.dumps({'error': str(e)})
            return json.dumps({'error': str(e)})


Toolset.register(NewsApiTools)

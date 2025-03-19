import os
import json
import logging

###### THIS USES Google-Search-Results library, NOT SERPAPI library

from serpapi import GoogleSearch

from agent_c_core.agent_c.toolsets import json_schema, Toolset

class SerpAPITools(Toolset):
    """
    GoogleSearchResults tool lets you use GoogleAPIs using SERPAPI.
    """

    def __init__(self, **kwargs):
        """
        Initializes GoogleSearchResultsTool by setting up a GOOGLE SERPAPI instance with the
        environment credentials.
        SERPAPI_API_KEY environment variable is required and needs to be mentioned in
        .env file, in order to this to work.
        """
        super().__init__(**kwargs, name='google_search', needed_keys=['SERPAPI_API_KEY'])
        self.api_key = os.environ.get("SERPAPI_API_KEY", None)
        self.logger: logging.Logger = logging.getLogger(__name__)
        if not self.api_key:
            self.valid = False


    def search_google_(self, params):
        self.logger.info(f"Searching Google with params: {params}")
        params["api_key"] = self.api_key
        search = GoogleSearch(params)
        results = search.get_dict()
        return results

    @json_schema(
        'Call this tool to fetch flights from GoogleFlight results API. Based on the users query providing '
        'details like departure and arrival airports, or outbound and inbound dates, user can find flights.',
        {
            'departure_id': {
                'type': 'string',
                'description': 'Departure Airport ID',
                'required': True
            },
            'arrival_id': {
                'type': 'string',
                'description': 'Arrival Airport ID',
                'required': True
            },
            'outbound_date': {
                'type': 'string',
                'description': 'Departure Date',
                'required': True
            },
            'return_date': {
                'type': 'string',
                'description': 'Return Date - Default is None',
                'required': False
            },
            'currency': {
                'type': 'string',
                'description': 'Currency - Default is USD($)',
                'required': False
            },
            'flight_type': {
                'type': 'string',
                'description': 'Flight type - RoundTrip(1) or OneWay(2) - Default is OneWay(2)',
                'required': False
            },
            'search_locale': {
                'type': 'string',
                'description': 'Google SerpAPI search locale - Default is us',
                'required': False
            },
        }
    )
    async def get_flights(self, **kwargs):
        departure_airport = kwargs.get('departure_id')
        arrival_airport = kwargs.get('arrival_id')
        departure_date = kwargs.get('outbound_date')
        return_date = kwargs.get('return_date', "")
        currency = kwargs.get('currency', 'USD')
        flight_type = kwargs.get('flight_type', "2")
        gl = kwargs.get('search_locale', 'us')

        if return_date != "":
            flight_type = "1"

        params = {
            "engine": "google_flights",
            "departure_id": departure_airport,
            "arrival_id": arrival_airport,
            "outbound_date": departure_date,
            "return_date": return_date,
            "type": flight_type,
            "currency": currency,
            "hl": "en",
            "gl": gl
        }

        results = self.search_google_(params)
        flights = self.filter_flight_(results)
        return json.dumps(flights)


    def filter_flight_(self, results):
        best_flights = results.get('best_flights', [])
        other_flights = results.get('other_flights', [])
        price_insights = results.get('price_insights')

        filtered_flight_results = {
            'best_flights': best_flights,
            'other_flights': other_flights,
            'price_insights': price_insights
        }
        return filtered_flight_results

    @json_schema(
        'Call this tool to search for events using Google Events API. Based on the users query '
        'provide top 5 events, scheduled to happen near the place user mentioned',
        {
            'query': {
                'type': 'string',
                'description': 'Query string to search events for',
                'required': True
            },
            'location': {
                'type': 'string',
                'description': 'Location for the events',
                'required': False
            },
            'max_return': {
                'type': 'integer',
                'description': 'Maximum number of events to return',
                'required': False,
                'default': 10
            }
        }
    )
    async def get_events(self, **kwargs):
        query = kwargs.get('query')
        location = kwargs.get('location', '')
        max_return = kwargs.get('max_return', 10)

        params = {
            "engine": "google_events",
            "q": query,
            "location": location,
            "hl": "en",
            "gl": "us",
            "htichips": "date:week"  # Default to events in the next week
        }

        results = self.search_google_(params)
        events = results.get('events', [])
        return json.dumps(events[:max_return])

    @json_schema(
        'Call this tool to search for News using Google News API. Based on the users query '
        'Display title of the news, and source of the news. Do not exceed more than 30 words count.',
        {
            'query': {
                'type': 'string',
                'description': 'Query string to search News for',
                'required': True
            },
            'max_return': {
                'type': 'integer',
                'description': 'Maximum number of events to return',
                'required': False,
                'default': 10
            }
        }
    )
    async def get_news(self, **kwargs):
        query = kwargs.get('query')
        max_return = kwargs.get('max_return', 10)

        params = {
            "engine": "google_news",
            "q": f"{query}"
        }
        results = self.search_google_(params)
        news = results.get('news_results', [])
        return json.dumps(news[:max_return])

    @json_schema(
        'Call this tool to scrape the results from Google search engine. Display organic results',
        {
            'query': {
                'type': 'string',
                'description': 'Query string to search Google for',
                'required': True
            },
            'max_return': {
                'type': 'integer',
                'description': 'Maximum number of events to return',
                'required': False,
                'default': 10
            }
        }
    )
    async def get_search_results(self, **kwargs):
        query = kwargs.get('query')
        max_return = kwargs.get('max_return', 10)

        params = {
            "engine": "google",
            "hl": "en",
            "gl": "us",
            "q": f"{query}",
            "num": max_return
        }

        results = self.search_google_(params)
        organic_results = results.get('organic_results', [])
        return json.dumps(organic_results)


Toolset.register(SerpAPITools)

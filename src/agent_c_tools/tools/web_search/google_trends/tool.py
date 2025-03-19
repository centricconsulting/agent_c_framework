import io
import json
import logging
from datetime import datetime, timedelta
import pandas as pd
from pytrends.request import TrendReq
from agent_c_core.agent_c.toolsets import json_schema, Toolset


class GoogleTrendsTools(Toolset):
    def __init__(self, **kwargs):
        super().__init__(**kwargs, name='googletrends')
        self.logger: logging.Logger = logging.getLogger(__name__)
        # Common timeframes for easy reference
        self.timeframe_options = {
            'past_hour': 'now 1-H',
            'past_day': 'now 1-d',
            'past_week': 'now 7-d',
            'past_month': 'today 1-m',
            'past_3months': 'today 3-m',
            'past_year': 'today 12-m',
            'past_5years': 'today 5-y',
            'all': 'all'
        }

    def _cache_dataframe(self, df: pd.DataFrame, query: str, timeframe: str, region: str) -> str:
        """
        Cache the DataFrame and return a cache key
        """
        if self.tool_cache is None:
            return "There is no tool cache to cache data. Please check the configuration."

        # Create a unique cache key
        cache_key = f"trends_{query}_{timeframe}_{region}_{datetime.now().strftime('%Y%m%d_%H%M%S')}"
        if df is None or df.empty:
            return 'No DataFrame is in memory. Unable to save to cache. Please load data first'

        # self.logger.debug(f"dataframe shape: {dataframe.shape}")

        # Convert DataFrame to parquet format
        parquet_buffer = io.BytesIO()
        df.to_parquet(parquet_buffer, engine='auto', compression='snappy')
        parquet_data = parquet_buffer.getvalue()

        # Cache the DataFrame
        self.tool_cache.set(cache_key, parquet_data, expire=3600)

        return cache_key

    @json_schema(
        description="Returns overall trending searches from Google. Limited to the United States unless a region is provided.",
        params={
            'region': {
                'type': 'string',
                'description': 'The region to get trending searches for. must pass in snake case country name. For example, united_states, japan.',
                'required': False,
                'default': 'united_states'
            }
        }
    )
    async def get_google_trending_searches(self, **kwargs):
        region = kwargs.get('region', 'united_states')
        try:
            pytrend = TrendReq()
            df = pytrend.trending_searches(pn=region)
            cache_key = self._cache_dataframe(df, "trending_searches", "current", region)

            return json.dumps({
                "data": df.to_json(),
                "cache_key": cache_key
            })
        except Exception as e:
            self.logger.error(f"Exception occurred while fetching trending searches: {str(e)}")
            return json.dumps({"error": str(e)})

    @json_schema(
        description="Find google search trends for a given query filtered by region and time period.",
        params={
            'query': {
                'type': 'string',
                'description': 'The query to search trends for',
                'required': True
            },
            'timeframe': {
                'type': 'string',
                'description': 'Time period for trends. Options: past_hour, past_day, past_week, past_month, past_3months, past_year, past_5years, all. Or custom format like "2022-01-01 2023-01-01"',
                'required': False,
                'default': 'past_month'
            },
            # 'find_related': {
            #     'type': 'boolean',
            #     'description': 'Flag to find related trends.',
            #     'required': False,
            #     'default': False
            # },
            'region': {
                'type': 'string',
                'description': 'The region to filter trends by (e.g., "US" for United States)',
                'required': False,
                'default': ''
            }
        }
    )
    # Removed the find_related parameter  from the schema and setting to False as there is a google error
    # https://github.com/GeneralMills/pytrends/issues/628
    async def get_google_trends_for_query(self, **kwargs):
        query = kwargs.get('query')
        find_related = kwargs.get('find_related', False)
        region = kwargs.get('region', '')
        timeframe = kwargs.get('timeframe', 'past_month')
        self.logger.info(f"Fetching Google trends for query: {query}, timeframe: {timeframe}, region: {region}")

        if not query:
            return json.dumps({"error": "The 'query' parameter is required."})

        try:
            # Convert friendly timeframe to pytrends format
            tf = self.timeframe_options.get(timeframe, timeframe)

            pytrend = TrendReq()
            pytrend.build_payload(
                kw_list=[query],
                timeframe=tf,
                geo=region
            )

            # Get interest over time data
            try:
                interest_df = pytrend.interest_over_time()
                if interest_df is None or interest_df.empty:
                    return json.dumps({"error": "No trend data found for the given query and parameters."})

                # Cache the DataFrame
                cache_key = self._cache_dataframe(interest_df, query, timeframe, region)
                interest_data = {
                    "data": interest_df.to_json(),
                    "cache_key": cache_key
                }

            except Exception as e:
                self.logger.error(f"Error fetching interest over time: {str(e)}")
                return json.dumps({"error": f"Failed to fetch trend data: {str(e)}"})

            # If related queries are requested
            if find_related:
                # TODO: Caching this data is not implemented yet
                self.logger.info(f"Fetching related queries for: {query}")
                try:
                    # Build a new payload specifically for related queries
                    pytrend.build_payload(
                        kw_list=[query],
                        timeframe=tf,
                        geo=region
                    )
                    related_data = pytrend.related_queries()

                    # Process related queries data
                    processed_related = {}
                    if related_data and query in related_data:
                        query_data = related_data[query]
                        for key, df in query_data.items():
                            if df is not None and not df.empty:
                                processed_related[key] = df.to_dict('records')
                            else:
                                processed_related[key] = []

                    return json.dumps({
                        "interest_over_time": interest_data,
                        "related_queries": processed_related
                    })
                except Exception as e:
                    self.logger.error(f"Error fetching related queries: {str(e)}")
                    return json.dumps({
                        "interest_over_time": interest_data,
                        "related_queries_error": str(e)
                    })

            return json.dumps({"interest_over_time": interest_data})

        except Exception as e:
            self.logger.error(f"Exception occurred while fetching trends for query: {str(e)}")
            return json.dumps({"error": str(e)})


Toolset.register(GoogleTrendsTools)
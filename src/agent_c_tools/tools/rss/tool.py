import json
import logging
import feedparser

from typing import List

from agent_c_core.agent_c.toolsets import json_schema, Toolset
from agent_c_tools.tools.rss.feeds import RSSToolFeed, default_feeds
from agent_c_tools.tools.rss.prompt import RSSSection


class RSSTools(Toolset):

    def __init__(self, **kwargs):
        super().__init__(**kwargs, name='rss')
        self.feeds: List[RSSToolFeed] = kwargs.get('feeds', default_feeds)
        feed_list: str = "\n".join([str(feed) for feed in self.feeds])
        self.section = RSSSection(feeds=feed_list)
        self.logger = kwargs.get('logger', logging.getLogger(__name__))

    async def _fetch_rss_feed(self, feed_url, schema):
        feed = feedparser.parse(feed_url)
        extracted_info = [
            {key: entry.get(key, '') for key in schema}
            for entry in feed.entries
        ]
        return json.dumps(extracted_info)

    def __find_feed_by_id(self, id):
        try:
            return next(feed for feed in self.feeds if feed.id == id)
        except StopIteration:
            self.logger.warning(f"No feed found with the ID: {id}")
            return None

    @json_schema(
            'Fetch an RSS feed for the supplied source ID. ',
            {
                'feed_id': {
                    'type': 'string',
                    'required': True
                }
            }
    )
    async def fetch_rss_feed(self, **kwargs):
        feed_id: str = kwargs['feed_id']
        feed = self.__find_feed_by_id(feed_id)

        if feed is None:
            return f"No feed with the ID `{feed_id}`"

        extracted_info = self.tool_cache.get(feed.url)
        if extracted_info is None:
            extracted_info = await self._fetch_rss_feed(feed.url, feed.fields_wanted)
            self.tool_cache.set(feed.url, extracted_info)

        return extracted_info

Toolset.register(RSSTools)

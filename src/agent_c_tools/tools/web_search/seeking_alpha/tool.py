import json
import logging

from bs4 import BeautifulSoup
from aiohttp import ClientSession

from agent_c_core.agent_c.toolsets import json_schema, Toolset

class SeekingAlphaTools(Toolset):
    def __init__(self, **kwargs):
        super().__init__(**kwargs, name='seekingalpha')
        self.logger: logging.Logger = logging.getLogger(__name__)

    @json_schema(
        description="Returns top K trending news from seekingalpha.",
        params={
            'limit': {
                'type': 'integer',
                'description': 'The number of news articles to return',
                'required': False,
                'default': 10
            },
            'extract_content': {
                'type': 'boolean',
                'description': 'Flag to extract content from news articles',
                'required': False,
                'default': False
            }
        }
    )
    async def get_topk_trending_news(self, **kwargs):
        limit = kwargs.get('limit', 10)
        extract_content = kwargs.get('extract_content', False)

        try:
            articles = []
            URL = "https://seekingalpha.com/news/trending_news"
            async with ClientSession() as session:
                async with session.get(URL) as response:
                    if response.status != 200:
                        return json.dumps({"error": f"Failed to fetch trending news with status code {response.status}"})

                    trending_news = await response.json()

                    for item in trending_news:
                        article_url = item.get("uri", "")
                        if not article_url.startswith("/news/"):
                            continue

                        article_id = article_url.split("/")[2].split("-")[0]

                        content = ""
                        if extract_content:
                            article_detail_url = f"https://seekingalpha.com/api/v3/news/{article_id}"
                            async with session.get(article_detail_url) as article_response:
                                if article_response.status == 200:
                                    jdata = await article_response.json()
                                    try:
                                        content_html = jdata["data"]["attributes"]["content"].replace("</li>", "</li>\n")
                                        content = BeautifulSoup(content_html, features="html.parser").get_text()
                                    except Exception as e:
                                        self.logger.error(f"Unable to extract content for: {article_detail_url}, error: {str(e)}")
                                else:
                                    self.logger.warning(f"Failed to fetch article details for {article_detail_url} with status {article_response.status}")

                        articles.append(
                            {
                                "title": item["title"],
                                "publishedAt": item["publish_on"].rsplit(".", 1)[0],
                                "url": "https://seekingalpha.com" + article_url,
                                "id": article_id,
                                "content": content,
                            }
                        )

                        if len(articles) >= limit:
                            break

            return json.dumps(articles[:limit])
        except Exception as e:
            self.logger.error(f"Exception occurred: {str(e)}")
            return json.dumps({"error": str(e)})


Toolset.register(SeekingAlphaTools)

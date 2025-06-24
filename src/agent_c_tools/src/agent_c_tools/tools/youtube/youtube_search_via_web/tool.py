from enum import Enum
from typing import Dict, Any, List, Optional
import json
import logging

from agent_c.toolsets import json_schema, Toolset
from ....helpers.media_file_html_helper import get_file_html
from ....helpers.path_helper import ensure_file_extension, create_unc_path, os_file_system_path
from ....tools.youtube import YouTubeBase, YouTubeError
from .web_scrape_util.youtube_search import YoutubeSearch


class SearchOrderType(str, Enum):
    RELEVANCE = "relevance"  # Most relevant first (default)
    DATE = "date"  # Most recent first
    # RATING = "rating"  # Highest rated first (not supported by youtube_search)
    # TITLE = "title"  # Alphabetical (not supported by youtube_search)
    # VIEW_COUNT = "viewCount"  # Most viewed first (not supported by youtube_search)


class SearchTimeFrame(str, Enum):
    LAST_HOUR = "last_hour"
    TODAY = "today"
    THIS_WEEK = "this_week"
    THIS_MONTH = "this_month"
    THIS_YEAR = "this_year"
    ALL_TIME = "all_time"


class YouTubeSearchScraperError(YouTubeError):
    """Exception for search-related errors using youtube_search library."""
    pass


class YoutubeSearchViaWebTools(YouTubeBase):
    def __init__(self, **kwargs):
        super().__init__(**kwargs, name='youtube_search_web')
        self.logger: logging.Logger = logging.getLogger(__name__)

    @json_schema(
        'Search for YouTube videos based on hashtag or search term using youtube_search library',
        {
            'query': {
                'type': 'string',
                'description': 'Search using combinations of:\n'
                               '- Hashtags: Include # symbol (e.g. #AI)\n'
                               '- Tags: Use tag: prefix (e.g. tag:news or tag:\'AI News\')\n'
                               '- Regular terms: Add directly\n'
                               'Example: "#AI tag:\'AI News\' latest developments"',
                'required': True
            },
            'max_results': {
                'type': 'integer',
                'description': 'Maximum number of videos to return. Default is 10, max is 50.',
                'required': False,
                'default': 10,
                'maximum': 50
            },
            'order': {
                'type': 'string',
                'description': 'Order of results before limiting. Supported: "relevance" (default), "viewcount".',
                'enum': ['relevance', 'viewcount'],
                'required': False,
                'default': 'relevance'
            },
            'workspace_name': {
                'type': 'string',
                'description': 'The name of the workspace to save the transcript.',
                'required': False,
                'default': 'project'
            },
            'file_path': {
                'type': 'string',
                'description': "The relative file path and name in the workspace for saving the results file.",
                'required': True
            },
            'return_type' : {
                'description': 'Return the output of the tool call directly instead of back to the agent',
                'type': 'string',
                'enum': ['both', 'agent', 'ui_only'],
                'required': False,
                'default': 'agent'
            }
        }
    )
    async def search_for_videos(self, **kwargs) -> str:
        try:
            query = kwargs.get('query')
            max_results = min(kwargs.get('max_results', 10), 50)  # Cap at 50
            workspace_name = kwargs.get('workspace_name', 'project')
            file_path = kwargs.get('file_path')
            order = kwargs.get('order', 'relevance')
            return_type = kwargs.get('return_type', 'agent')
            tool_context = kwargs.get('tool_context', {})

            self.logger.info(f'Searching for youtube videos via web tool with query: {query}')

            # Create cache key
            cache_key = f"search_{query.replace(' ', '_').replace('#', 'hash_').replace(':', '_').replace('"', '').replace("'", '')}_{max_results}"
            cached_results = self.tool_cache.get(cache_key)

            if cached_results:
                self.logger.debug(f'Found cached search results for query: {query}')
                videos = json.loads(cached_results)
            else:
                self.logger.debug(f'Performing search using youtube_search for query: {query}')
                videos = await self._search_youtube_videos(
                    query=query,
                    max_results=max_results,
                    order=order
                )
                self.tool_cache.set(cache_key, json.dumps(videos), expire=self.cache_expire)

            # Save results to file
            file_path = await self._save_search_results(results=videos, file_path=file_path, workspace_name=workspace_name, tool_context=tool_context)
            output = json.dumps({
                'videos': videos,
                'total_results': len(videos),
                'query': query,
                'file_path': file_path
            })

            # Create a user-friendly message
            ui_message = f"Found {len(videos)} videos for query '{query}':<br>"
            for video in videos:
                tags = f" | Tags: {', '.join(video['tags'])}" if video['tags'] else ""
                ui_message += (
                    f"• <a href='{video['url']}' target='_blank'>{video['title']}</a> "
                    f"({video['duration']}) - {video['channel_title']}<br>"
                    f"  Views: {video['view_count']:,} | Likes: {video['like_count']:,}{tags}<br>")

            await self._raise_render_media(
                sent_by_class=self.__class__.__name__,
                sent_by_function='web_search_for_videos',
                content_type="text/html",
                content=ui_message,
                tool_context=tool_context
            )

            if return_type == 'ui_only':
                return "Results sent to user."
            else:  # 'agent'
                return output

        except Exception as e:
            self.logger.error(f'Error in search_videos: {str(e)}')
            return self._error_response(str(e))

    async def _search_youtube_videos(
            self,
            query: str,
            max_results: int = 10,
            order: str = 'relevance',
    ) -> List[Dict[str, Any]]:
        """
        Search YouTube videos using the youtube_search library.

        Args:
            query: Search term
            max_results: Maximum number of results to return
            order: 'relevance, data, or viewCount'
        """
        results = YoutubeSearch(query).to_dict()

        # Convert to a format similar to the original code.
        # Some fields not available, so we'll default them.
        videos = []
        try:
            for item in results:
                video_id = item.get('id', '')
                url = f"https://www.youtube.com/watch?v={video_id}" if video_id else f"https://www.youtube.com{item['url_suffix']}"
                thumbnail = item['thumbnails'][0] if item.get('thumbnails') else ''

                # Convert views from string like "1,234 views" to int
                view_count = self._parse_view_count(item.get('views', '0 views'))

                # Duration may already be in a human-readable format like "4:32"
                # We'll keep it as is.
                duration_str = item.get('duration', '')

                # We don't have like_count, comment_count, tags, definition, or captions info
                video = {
                    "title": item.get('title', ''),
                    "description": item.get('long_desc') if item.get('long_desc') is not None else "",
                    "url": url,
                    "thumbnail": thumbnail,
                    "published_at": item.get('publish_time', ''),
                    "channel_title": item.get('channel', ''),
                    "channel_id": '',  # Not provided by youtube_search
                    "channel_url": '', # Not provided by youtube_search
                    "duration": duration_str,
                    "view_count": view_count,
                    "like_count": 0, # Not available
                    "comment_count": 0, # Not available
                    "definition": "",  # Not available
                    "has_caption": False, # Not available
                    "tags": [] # Not available
                }
                videos.append(video)
        except Exception as e:
            raise YouTubeSearchScraperError(f"Error parsing web youtube search results: {str(e)}")

        # Debug help - looking for titles with "Google" in them
        # results = [item for item in results if "Google".lower() in item['title'].lower()]
        if order.lower() == 'viewcount':
            videos.sort(key=lambda v: v['view_count'], reverse=True)
        elif order.lower() == 'date':
            # We have only relative times (e.g., "3 hours ago", "1 day ago"), which are difficult to parse reliably.
            # Unless you implement logic to convert relative times to a comparable value,
            # sorting by date won't be reliable. We'll skip implementing it for now.
            pass

        return videos[:max_results]

    @staticmethod
    def _parse_view_count(views_str: str) -> int:
        # views_str typically looks like "12,345 views"
        # We'll remove non-digits and convert to int.
        import re
        digits = re.sub(r'[^\d]', '', views_str)
        return int(digits) if digits else 0

    async def _save_search_results(self, results: List[Dict[str, Any]], workspace_name: str, file_path: str, tool_context: Optional[Dict] = None) -> str:
        """Save search results to a file in the workspace.

        Args:
            results: List of search results to save.
            workspace_name: Name of the workspace to save the file in.
            file_path: Relative path and name of the file in the workspace.
        """
        # Prepare file path and ensure .json extension
        file_path = ensure_file_extension(file_path, 'json')
        unc_path = create_unc_path(workspace_name, file_path)
        error, workspace, relative_path = self.workspace_tool.validate_and_get_workspace_path(unc_path)
        if error:
            return f"Invalid path: {error}"
        result = await self.workspace_tool.write(path=unc_path, mode='write', data=json.dumps(results, indent=2))
        result_data = json.loads(result)
        if 'error' in result_data:
            return f"Error writing file: {result_data['error']}"

        # Get the full path of the file
        os_path = os_file_system_path(self.workspace_tool, unc_path)

        await self._raise_render_media(
            sent_by_class=self.__class__.__name__,
            sent_by_function='save_youtube_web_search_results',
            content_type="text/html",
            content=get_file_html(os_path=os_path, unc_path=unc_path, additional_html=f"Youtube web search results saved."),
            tool_context=tool_context
        )
        self.logger.debug(f'Saved search results to workspace: {unc_path}')
        return unc_path


Toolset.register(YoutubeSearchViaWebTools, required_tools=['WorkspaceTools'])
from enum import Enum
from typing import Dict, Any, List, Optional
import json
import logging
from datetime import datetime, timedelta
import requests

from agent_c.toolsets import json_schema, Toolset
from ....helpers.media_file_html_helper import get_file_html
from ....helpers.path_helper import ensure_file_extension, create_unc_path, os_file_system_path
from ....tools.youtube import YouTubeBase, YouTubeError
from datetime import timezone

class SearchOrderType(str, Enum):
    DATE = "date"  # Most recent first
    RATING = "rating"  # Highest rated first
    RELEVANCE = "relevance"  # Most relevant first
    TITLE = "title"  # Alphabetical
    VIEW_COUNT = "viewCount"  # Most viewed first


class SearchTimeFrame(str, Enum):
    LAST_HOUR = "last_hour"
    TODAY = "today"
    THIS_WEEK = "this_week"
    THIS_MONTH = "this_month"
    THIS_YEAR = "this_year"
    ALL_TIME = "all_time"


class YouTubeSearchError(YouTubeError):
    """Exception for search-related errors."""
    pass


class YoutubeSearchViaApiTools(YouTubeBase):
    def __init__(self, **kwargs):
        super().__init__(**kwargs, name='youtube_search_api')
        self.logger: logging.Logger = logging.getLogger(__name__)

    @json_schema(
        'Search for YouTube videos based on hashtag or search term',
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
                'description': 'Order of results.',
                'enum': ['date', 'rating', 'relevance', 'title', 'viewCount'],
                'required': False,
                'default': 'relevance'
            },
            'time_frame': {
                'type': 'string',
                'description': 'Time frame to search within.',
                'enum': ['last_hour', 'today', 'this_week', 'this_month', 'this_year', 'all_time'],
                'required': False,
                'default': 'this_month'
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
            order = kwargs.get('order', 'relevance')
            time_frame = kwargs.get('time_frame', 'this_month')
            workspace_name = kwargs.get('workspace_name', 'project')
            file_path = kwargs.get('file_path')
            return_type = kwargs.get('return_type', 'agent')
            tool_context = kwargs.get('tool_context', {})

            self.logger.info(f'Searching for youtube videos via api tool with query: {query}')

            # Create cache key
            cache_key = f"search_{query.replace(' ', '_').replace('#', 'hash_').replace(':', '_').replace('"', '').replace("'", '')}_{max_results}_{order}_{time_frame}"
            cached_results = self.tool_cache.get(cache_key)

            if cached_results:
                self.logger.debug(f'Found cached search results for query: {query}')
                videos = json.loads(cached_results)
            else:
                self.logger.debug(f'Performing search for query: {query}')
                videos = await self._search_youtube_videos(
                    query=query,
                    max_results=max_results,
                    order=order,
                    time_frame=time_frame
                )
                self.tool_cache.set(cache_key, json.dumps(videos), expire=self.cache_expire)

            # Save results to file
            file_path = await self._save_search_results(results=videos, file_path=file_path,workspace_name=workspace_name, tool_context=tool_context)
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
                sent_by_function='api_search_for_videos',
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
            max_results: int,
            order: str = 'relevance',
            time_frame: str = 'all_time'
    ) -> List[Dict[str, Any]]:
        """
        Search YouTube videos using the Data API with enhanced metadata.

        Args:
            query: Search term or hashtag
            max_results: Maximum number of results to return
            order: Order of results
            time_frame: Time frame to search within
        """
        videos = []
        page_token = None
        base_url = "https://www.googleapis.com/youtube/v3/search"

        # Convert time_frame to publishedAfter parameter
        published_after = self._get_published_after(time_frame)

        while len(videos) < max_results:
            params = {
                "key": self.api_key,
                "part": "snippet,id",  # Include ID part
                "q": query,
                "type": "video",
                "maxResults": min(50, max_results - len(videos)),
                "order": order,
                "pageToken": page_token,
                "videoEmbeddable": True,
                "safeSearch": "none",  # Include all results
                "videoType": "any",  # Include all video types
                "relevanceLanguage": "en"  # Prioritize English results
            }

            if published_after:
                params["publishedAfter"] = published_after

            response = requests.get(base_url, params=params)
            if response.status_code == 200:
                data = response.json()

                # Get video statistics and content details in batch
                video_ids = [item['id']['videoId'] for item in data['items']]
                details = await self._get_video_details(video_ids)

                for item in data['items']:
                    video_id = item['id']['videoId']
                    video_details = details.get(video_id, {})
                    snippet = item['snippet']
                    statistics = video_details.get('statistics', {})
                    content_details = video_details.get('contentDetails', {})

                    # Parse duration into minutes and seconds
                    duration = content_details.get('duration', 'PT0M0S')
                    duration_str = self._format_duration(duration)

                    video = {
                        "title": snippet['title'],
                        "description": snippet['description'],
                        "url": f"https://www.youtube.com/watch?v={video_id}",
                        "thumbnail": self._get_best_thumbnail(snippet['thumbnails']),
                        "published_at": snippet['publishedAt'],
                        "channel_title": snippet['channelTitle'],
                        "channel_id": snippet['channelId'],
                        "channel_url": f"https://www.youtube.com/channel/{snippet['channelId']}",
                        "duration": duration_str,
                        "view_count": int(statistics.get('viewCount', 0)),
                        "like_count": int(statistics.get('likeCount', 0)),
                        "comment_count": int(statistics.get('commentCount', 0)),
                        "definition": content_details.get('definition', '').upper(),  # HD or SD
                        "has_caption": content_details.get('caption', '').lower() == 'true',
                        "tags": video_details.get('snippet', {}).get('tags', [])
                    }
                    videos.append(video)

                if len(videos) >= max_results:
                    break

                page_token = data.get('nextPageToken')
                if not page_token:
                    break
            else:
                self._handle_api_error(response)

        # Sort by view count if that was requested
        if order == 'viewCount':
            videos.sort(key=lambda x: x['view_count'], reverse=True)

        return videos[:max_results]

    async def _get_video_details(self, video_ids: List[str]) -> Dict[str, Dict[str, Any]]:
        """Get detailed information for a batch of videos."""
        details = {}
        base_url = "https://www.googleapis.com/youtube/v3/videos"

        # Process in batches of 50 (API limit)
        for i in range(0, len(video_ids), 50):
            batch = video_ids[i:i + 50]
            params = {
                "key": self.api_key,
                "part": "snippet,statistics,contentDetails",  # Get more details
                "id": ",".join(batch)
            }

            response = requests.get(base_url, params=params)
            if response.status_code == 200:
                data = response.json()
                for item in data['items']:
                    details[item['id']] = item
            else:
                self._handle_api_error(response)

        return details

    @staticmethod
    def _format_duration(duration: str) -> str:
        """Convert YouTube duration format (PT1H2M10S) to readable format (1:02:10)."""
        import re

        # Extract hours, minutes, and seconds
        hours = re.search(r'(\d+)H', duration)
        minutes = re.search(r'(\d+)M', duration)
        seconds = re.search(r'(\d+)S', duration)

        # Convert to integers, default to 0 if not found
        h = int(hours.group(1)) if hours else 0
        m = int(minutes.group(1)) if minutes else 0
        s = int(seconds.group(1)) if seconds else 0

        # Format the duration string
        if h > 0:
            return f"{h}:{m:02d}:{s:02d}"
        else:
            return f"{m}:{s:02d}"

    @staticmethod
    def _get_best_thumbnail(thumbnails: Dict[str, Any]) -> str:
        """Get the highest quality thumbnail available."""
        # Priority order from highest to lowest quality
        priorities = ['maxres', 'high', 'medium', 'standard', 'default']

        for quality in priorities:
            if quality in thumbnails:
                return thumbnails[quality]['url']

        # Fallback to default if nothing else is available
        return thumbnails.get('default', {}).get('url', '')

    async def _get_video_statistics(self, video_ids: List[str]) -> Dict[str, Dict[str, Any]]:
        """Get statistics for a batch of videos."""
        stats = {}
        base_url = "https://www.googleapis.com/youtube/v3/videos"

        # Process in batches of 50 (API limit)
        for i in range(0, len(video_ids), 50):
            batch = video_ids[i:i + 50]
            params = {
                "key": self.api_key,
                "part": "statistics",
                "id": ",".join(batch)
            }

            response = requests.get(base_url, params=params)
            if response.status_code == 200:
                data = response.json()
                for item in data['items']:
                    stats[item['id']] = item['statistics']
            else:
                self._handle_api_error(response)

        return stats

    async def _save_search_results(self, results: List[Dict[str, Any]], workspace_name: str, file_path: str, tool_context: Optional[Dict] = None) -> str:
        """Save search results to a file in the workspace.
            Args:
                results: List of search results to save.
                workspace_name: Name of the workspace to save the file in.
                file_path: Relative path and name of the file in the workspace.
        """
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
            sent_by_function='save_youtube_api_search_results',
            content_type="text/html",
            content=get_file_html(os_path=os_path, unc_path=unc_path, additional_html=f"Youtube API Search results saved"),
            tool_context=tool_context
        )
        self.logger.debug(f'Saved api search results to workspace: {unc_path}')
        return unc_path

    @staticmethod
    def _get_published_after(time_frame: str) -> Optional[str]:
        """Convert time frame to ISO 8601 timestamp for publishedAfter parameter."""

        if time_frame == 'all_time':
            return None

        # Use timezone-aware datetime
        now = datetime.now(timezone.utc)

        time_deltas = {
            'last_hour': timedelta(hours=1),
            'today': timedelta(days=1),
            'this_week': timedelta(weeks=1),
            'this_month': timedelta(days=30),
            'this_year': timedelta(days=365)
        }

        delta = time_deltas.get(time_frame)
        if delta:
            published_after = (now - delta).isoformat()
            return published_after

        return None


Toolset.register(YoutubeSearchViaApiTools, required_tools=['WorkspaceTools'])
from enum import Enum
from typing import Dict, Any, List, Literal, Optional
import json
from datetime import datetime
from collections import Counter
import requests

from agent_c.toolsets import json_schema, Toolset
from ....helpers.media_file_html_helper import get_file_html
from ....helpers.path_helper import ensure_file_extension, create_unc_path, os_file_system_path
from ..base import YouTubeBase, YouTubeError


class CommentOrderType(str, Enum):
    RELEVANCE = "relevance"  # Top comments first
    TIME = "time"  # Most recent first


class YouTubeCommentsError(YouTubeError):
    """Exception for comment-related errors."""
    pass


class YoutubeCommentsTools(YouTubeBase):
    def __init__(self, **kwargs):
        super().__init__(**kwargs, name='youtube_comments')

    @json_schema(
        'Fetch comments from a YouTube video with sorting options',
        {
            'video_url': {
                'type': 'string',
                'description': 'The URL of the YouTube video.',
                'required': True
            },
            'max_comments': {
                'type': 'integer',
                'description': 'Maximum number of comments to fetch. Default is 100.',
                'required': False,
                'default': 100
            },
            'order': {
                'type': 'string',
                'description': 'Order of comments: "relevance" (top comments) or "time" (most recent). Default is relevance.',
                'enum': ['relevance', 'time'],
                'required': False,
                'default': 'relevance'
            },
            'include_replies': {
                'type': 'boolean',
                'description': 'Whether to include reply comments. Default is False.',
                'required': False,
                'default': False
            },
            'workspace_name': {
                'type': 'string',
                'description': 'The name of the workspace to save the transcript.',
                'required': False,
                'default': 'project'
            },
            'file_path': {
                'type': 'string',
                'description': 'The path for saving the transcript file. Defaults to the workspace root.',
                'required': False
            },
            'file_name': {
                'type': 'string',
                'description': 'The name for saving the comments file. Defaults to video_id_comments.',
                'required': False
            }
        }
    )
    async def fetch_comments(self, **kwargs) -> str:
        try:
            video_url = kwargs.get('video_url')
            max_comments = kwargs.get('max_comments', 100)
            order = kwargs.get('order', 'relevance')
            include_replies = kwargs.get('include_replies', False)
            file_name = kwargs.get('file_name')
            workspace_name = kwargs.get('workspace_name', 'project')
            file_path = kwargs.get('file_path')
            tool_context = kwargs.get('tool_context', {})

            video_id = self._extract_video_id(video_url)
            if not video_id:
                self.logger.error(f'Invalid YouTube URL: {video_url}')
                return self._error_response('Invalid YouTube URL or Video ID could not be extracted.')

            # Create a unique cache key based on the fetch parameters
            cache_key = f"comments_{video_id}_{order}_{include_replies}_{max_comments}"
            cached_comments = self.tool_cache.get(cache_key)

            if cached_comments:
                self.logger.debug(f'Found cached comments for video: {video_id}')
                comments = json.loads(cached_comments)
            else:
                self.logger.debug(f'Fetching comments for video: {video_id}')
                comments = await self._fetch_youtube_comments(
                    video_id=video_id,
                    max_comments=max_comments,
                    order=order,
                    include_replies=include_replies
                )
                # Cache the full results
                self.tool_cache.set(cache_key, json.dumps(comments), expire=self.cache_expire)

            # Save full results to file
            file_name = file_name if file_name else f"{video_id}_comments_{order}"
            await self._save_data(data=comments, data_type='comments',
                                  file_name=file_name, file_path=file_path, workspace_name=workspace_name,
                                  tool_context=tool_context)

            self.logger.info(f'Fetched {len(comments)} comments for video: {video_id}')

            # Return limited preview in response
            preview_comments = comments[:10]  # Always return only first 10 for preview

            return json.dumps({
                'instruction': 'Only a snippet of comments is returned for preview. Full comments are saved in workspace and cache.',
                'comments': preview_comments,
                'total_comments': len(comments),
                'order_type': order,
                'file_path': file_path,
                'cache_key': cache_key  # Return cache key for analysis to use
            })

        except Exception as e:
            self.logger.error(f'Error in fetch_comments: {str(e)}')
            return self._error_response(str(e))

    async def _fetch_youtube_comments(
            self,
            video_id: str,
            max_comments: int,
            order: Literal['relevance', 'time'] = 'relevance',
            include_replies: bool = False
    ) -> List[Dict[str, Any]]:
        """
        Fetch comments from YouTube API with enhanced ordering and reply support.

        Args:
            video_id: The YouTube video ID
            max_comments: Maximum number of comments to fetch
            order: 'relevance' for top comments or 'time' for most recent
            include_replies: Whether to fetch reply comments
        """
        comments = []
        page_token = ""
        base_url = "https://www.googleapis.com/youtube/v3/commentThreads"

        while len(comments) < max_comments:
            params = {
                "key": self.api_key,
                "part": "snippet,replies",
                "videoId": video_id,
                "maxResults": min(100, max_comments - len(comments)),
                "pageToken": page_token,
                "order": order,
                "textFormat": "plainText",
            }

            response = requests.get(base_url, params=params)
            if response.status_code == 200:
                data = response.json()

                for item in data.get("items", []):
                    # Get main comment
                    comment_data = item["snippet"]["topLevelComment"]["snippet"]
                    comment = {
                        "text": comment_data["textDisplay"],
                        "author": comment_data["authorDisplayName"],
                        "likes": comment_data["likeCount"],
                        "published_at": comment_data["publishedAt"],
                        "updated_at": comment_data["updatedAt"],
                        "reply_count": item["snippet"]["totalReplyCount"],
                        "is_reply": False
                    }
                    comments.append(comment)

                    # Get replies if requested and available
                    if include_replies and item["snippet"]["totalReplyCount"] > 0 and "replies" in item:
                        for reply in item["replies"]["comments"]:
                            reply_data = reply["snippet"]
                            reply_comment = {
                                "text": reply_data["textDisplay"],
                                "author": reply_data["authorDisplayName"],
                                "likes": reply_data["likeCount"],
                                "published_at": reply_data["publishedAt"],
                                "updated_at": reply_data["updatedAt"],
                                "is_reply": True,
                                "parent_author": comment["author"]
                            }
                            comments.append(reply_comment)

                    if len(comments) >= max_comments:
                        break

                page_token = data.get("nextPageToken")
                if not page_token:
                    break
            else:
                self._handle_api_error(response)

        return comments[:max_comments]

    @json_schema(
        'Analyze YouTube comments for insights with ordering support',
        {
            'video_url': {
                'type': 'string',
                'description': 'The URL of the YouTube video.',
                'required': True
            },
            'max_comments': {
                'type': 'integer',
                'description': 'Maximum number of comments to analyze. Default is 100.',
                'required': False,
                'default': 100
            },
            'order': {
                'type': 'string',
                'description': 'Order of comments: "relevance" (top comments) or "time" (most recent). Default is relevance.',
                'enum': ['relevance', 'time'],
                'required': False,
                'default': 'relevance'
            },
            'include_replies': {
                'type': 'boolean',
                'description': 'Whether to include reply comments in analysis. Default is False.',
                'required': False
            },
            'workspace_name': {
                'type': 'string',
                'description': 'The name of the workspace to save the transcript.',
                'required': False,
                'default': 'project'
            },
            'file_path': {
                'type': 'string',
                'description': 'The path for saving the transcript file. Defaults to the workspace root.',
                'required': False
            },
            'file_name': {
                'type': 'string',
                'description': 'The name for saving the analysis file. Defaults to video_id_analysis.',
                'required': False
            }
        }
    )
    async def analyze_comments(self, **kwargs) -> str:
        try:
            video_url = kwargs.get('video_url')
            max_comments = kwargs.get('max_comments', 100)
            order = kwargs.get('order', 'relevance')
            include_replies = kwargs.get('include_replies', False)
            file_name = kwargs.get('file_name')
            workspace_name = kwargs.get('workspace_name', 'project')
            file_path = kwargs.get('file_path')
            tool_context = kwargs.get('tool_context', {})

            video_id = self._extract_video_id(video_url)
            if not video_id:
                return self._error_response('Invalid YouTube URL or Video ID could not be extracted.')

            # Try to get from cache first
            cache_key = f"comments_{video_id}_{order}_{include_replies}_{max_comments}"
            cached_comments = self.tool_cache.get(cache_key)

            if cached_comments:
                self.logger.debug(f'Using cached comments for analysis: {video_id}')
                comments = json.loads(cached_comments)
            else:
                # If not in cache, fetch them (this will also cache them)
                comments_result = json.loads(await self.fetch_comments(**kwargs))

                if 'error' in comments_result:
                    return self._error_response(f"Failed to fetch comments: {comments_result['error']}")

                # Get from cache using the key returned by fetch_comments
                cached_comments = self.tool_cache.get(comments_result['cache_key'])
                comments = json.loads(cached_comments)

            analysis = await self._analyze_comments(comments, include_replies)

            file_name = file_name if file_name else f"{video_id}_analysis_{order}"
            await self._save_data(data=analysis, data_type='analysis', file_name=file_name,
                                  file_path=file_path, workspace_name=workspace_name, tool_context=tool_context)

            self.logger.debug(f'Analyzed {len(comments)} comments for video: {video_id}')

            return json.dumps({
                'analysis': analysis,
                'file_path': file_path,
                'order_type': order
            })

        except Exception as e:
            self.logger.error(f'Error in analyze_comments: {str(e)}')
            return self._error_response(str(e))

    @staticmethod
    async def _analyze_comments(comments: List[Dict[str, Any]], include_replies: bool) -> Dict[str, Any]:
        """Analyze comments for insights with enhanced metrics."""
        main_comments = [c for c in comments if not c.get("is_reply", False)]
        reply_comments = [c for c in comments if c.get("is_reply", False)] if include_replies else []

        analysis = {
            "total_comments": len(main_comments),
            "total_replies": len(reply_comments),
            "total_likes": sum(comment["likes"] for comment in comments),
            "top_commenters": Counter(comment["author"] for comment in main_comments).most_common(5),
            "engagement_over_time": {},
            "avg_likes_per_comment": 0,
            "most_liked_comments": sorted(main_comments, key=lambda x: x["likes"], reverse=True)[:5],
            "most_discussed_comments": sorted(main_comments, key=lambda x: x["reply_count"], reverse=True)[
                                       :5] if include_replies else []
        }

        # Calculate average likes
        if comments:
            analysis["avg_likes_per_comment"] = analysis["total_likes"] / len(comments)

        # Analyze engagement over time
        for comment in comments:
            date = datetime.fromisoformat(comment["published_at"].replace('Z', '+00:00')).strftime('%Y-%m-%d')
            if date not in analysis["engagement_over_time"]:
                analysis["engagement_over_time"][date] = {
                    "comments": 0,
                    "replies": 0,
                    "likes": 0
                }

            if comment.get("is_reply", False):
                analysis["engagement_over_time"][date]["replies"] += 1
            else:
                analysis["engagement_over_time"][date]["comments"] += 1

            analysis["engagement_over_time"][date]["likes"] += comment["likes"]

        return analysis

    async def _save_data(self, data: Any, data_type: Literal['comments','analysis'],
                         workspace_name: str,
                         file_path: str = None, tool_context: Optional[Dict] = None, **kwargs) -> str:
        """Save data to a file in the workspace.
         Args:
                data: List of search results to save.
                data_type: Type of data being saved (comments or analysis).
                workspace_name: Name of the workspace to save the file in.
                file_path: Relative path and name of the file in the workspace.
        """
        file_path = ensure_file_extension(file_path, 'json')
        unc_path = create_unc_path(workspace_name, file_path)
        error, workspace, relative_path = self.workspace_tool.validate_and_get_workspace_path(unc_path)
        if error:
            return f"Invalid path: {error}"
        result = await self.workspace_tool.write(path=unc_path, mode='write', data=json.dumps(data, indent=2))
        result_data = json.loads(result)
        if 'error' in result_data:
            return f"Error writing file: {result_data['error']}"

        # Get the full path of the file
        os_path = os_file_system_path(self.workspace_tool, unc_path)


        await self._raise_render_media(
            sent_by_class=self.__class__.__name__,
            sent_by_function=f'save_youtube_video_{data_type}',
            content_type="text/html",
            content=get_file_html(os_path=os_path, unc_path=unc_path, additional_html=f"Youtube {data_type.capitalize()} saved."),
            tool_context=tool_context
        )
        self.logger.debug(f'Saved data to workspace: {unc_path}')
        return unc_path


Toolset.register(YoutubeCommentsTools, required_tools=['WorkspaceTools'])
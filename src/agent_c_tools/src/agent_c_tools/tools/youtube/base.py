import os
import json
import logging
import traceback
from typing import Optional, Dict, Any

import requests
from agent_c.toolsets import Toolset


class YouTubeError(Exception):
    """Base exception for YouTube-related errors."""

    def __init__(self, message, original_error=None):
        super().__init__(message)
        self.original_error = original_error
        self.stack_trace = traceback.format_exc()
        self.log_error()
        self.logger: logging.Logger = logging.getLogger(__name__)

    def log_error(self):
        self.logger.error(f"{self.__class__.__name__}: {self}")
        if self.original_error:
            self.logger.error(f"Original error: {self.original_error}")
        self.logger.error(f"Stack trace:\n{self.stack_trace}")


class YouTubeBase(Toolset):
    """Base class for YouTube-related tools."""

    def __init__(self, **kwargs):
        super().__init__(
            **kwargs,
            needed_keys=['GOOGLE_YOUTUBE_API_KEY']
        )
        self.logger: logging.Logger = logging.getLogger(__name__)
        # Get workspace tools for file operations
        self.workspace_tool = self.tool_chest.available_tools.get('WorkspaceTools')
        if not self.workspace_tool:
            self.logger.warning("Workspace toolset not available. This tool requires workspace tools.")

        self.api_key = os.getenv('GOOGLE_YOUTUBE_API_KEY')
        self.MAX_TOKEN_SIZE = 35000
        self.cache_expire = 3600  # expires in 1 hour for cache

    def _count_tokens(self, data: str, tool_context) -> int:
        """
        Centralized token counting method for YouTube tools.

        Args:
            data: The text content to count tokens for
            tool_context: Tool context containing agent_runtime

        Returns:
            Number of tokens in the data

        Raises:
            ValueError: If tool_context or agent_runtime is missing
        """
        if tool_context is None or 'agent_runtime' not in tool_context:
            raise ValueError("tool_context with agent_runtime is required for token counting")
        return tool_context['agent_runtime'].count_tokens(data)

    def _handle_api_error(self, response: requests.Response) -> None:
        """Handle YouTube API errors."""
        if response.status_code == 403:
            error_details = response.json()
            if "error" in error_details:
                if "errors" in error_details["error"]:
                    for error in error_details["error"]["errors"]:
                        if error.get("reason") == "commentsDisabled":
                            self.logger.error("Comments are disabled for this video.")
                            raise YouTubeError("Comments are disabled for this video.")
                        elif error.get("reason") == "quotaExceeded":
                            self.logger.error("YouTube API quota has been exceeded.")
                            raise YouTubeError("YouTube API quota has been exceeded.")
                        elif error.get("reason") == "forbidden":
                            self.logger.error("Insufficient permissions to access the comments.")
                            raise YouTubeError("Insufficient permissions to access the comments.")
        elif response.status_code == 404:
            self.logger.error(f"The specified video was not found. {response}")
            raise YouTubeError("The specified video was not found.")
        else:
            self.logger.error(f"YouTube API error: {response}")
            response.raise_for_status()

    @staticmethod
    def _extract_video_id(url: str) -> Optional[str]:
        import re
        youtube_regex = r'''
            (?:https?://)?
            (?:www\.)?
            (?:youtube|youtu|youtube-nocookie)\.
            (?:com|be)/
            (?:watch\?v=|embed/|v/|.+/)?
            ([\w-]{10,12})
            (?:&.+)?
        '''
        match = re.match(youtube_regex, url, re.VERBOSE)
        return match.group(1) if match else None

    @staticmethod
    def _error_response(message: str) -> str:
        return json.dumps({'success': False, 'error': message})

    def _is_response_to_big(self, response: str, tool_context) -> bool:
        response_size = self._count_tokens(response, tool_context)
        if response_size > self.MAX_TOKEN_SIZE:
            self.logger.debug(f"Response is too big: {response_size}")
            return True
        else:
            return False
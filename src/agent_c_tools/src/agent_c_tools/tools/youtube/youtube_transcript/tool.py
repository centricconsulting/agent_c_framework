import json
import logging
from pickle import FALSE
from typing import Dict, Any, Optional

from youtube_transcript_api import (
    YouTubeTranscriptApi,
    TranscriptsDisabled,
    NoTranscriptFound,
    VideoUnavailable
)

from agent_c.toolsets import json_schema, Toolset
from ....helpers.media_file_html_helper import get_file_html
from ....helpers.path_helper import create_unc_path, ensure_file_extension, os_file_system_path
from ..base import YouTubeBase, YouTubeError


class YouTubeTranscriptError(YouTubeError):
    """Exception for transcript-related errors."""
    pass


class YoutubeTranscriptTools(YouTubeBase):
    """Class for handling YouTube video transcripts."""

    def __init__(self, **kwargs):
        super().__init__(**kwargs, name='youtube_transcript')
        self.transcript_api = YouTubeTranscriptApi()
        self.logger: logging.Logger = logging.getLogger(__name__)

    @json_schema(
        'Retrieve and save a YouTube transcript.',
        {
            'video_url': {
                'type': 'string',
                'description': 'The URL of the YouTube video.',
                'required': True
            },
            'workspace_name': {
                'type': 'string',
                'description': 'The name of the workspace to save the transcript.',
                'required': False,
                'default': 'project'
            },
            'file_path': {
                'type': 'string',
                'description': "The relative file path and name in the workspace for saving the transcript file. File extension will be '.txt'",
                'required': True
            },
            'include_timestamps': {
                'type': 'boolean',
                'description': 'Whether to include timestamps in the transcript. Default is False.',
                'required': False,
                'default': False
            }
        }
    )
    async def retrieve_and_save_transcript(self, **kwargs) -> str:
        """
        Retrieve and save a YouTube video transcript.

        Args:
            kwargs:
                video_url: URL of the YouTube video
                file_name: Optional name for the saved file
                include_timestamps: Whether to include timestamps in output
                workspace_name: Name of the workspace to save the transcript
                file_path: Optional path for saving the file

        Returns:
            JSON string containing transcript and file path or error
        """
        try:
            video_url = kwargs.get('video_url')
            include_timestamps = kwargs.get('include_timestamps', False)
            file_path = kwargs.get('file_path')
            workspace_name = kwargs.get('workspace_name', 'project')
            tool_context = kwargs.get('tool_context', None)

            video_id = self._extract_video_id(video_url)
            if not video_id:
                self.logger.error(f"Invalid Youtube URL or Video ID for url: {video_url} - returned id: {video_id}")
                return self._error_response('Invalid YouTube URL or Video ID could not be extracted.')

            transcript = await self._get_transcript(video_url, video_id, include_timestamps)
            file_path = await self._save_transcript(transcript=transcript, file_path=file_path, include_timestamps=include_timestamps,
                                                    workspace_name=workspace_name, tool_context=tool_context)
            return json.dumps({
                'transcript': transcript,
                'file_path': file_path,
                'video_id': video_id
            })

        except TranscriptsDisabled:
            error_msg = "Transcripts are disabled for this video"
            self.logger.error(error_msg)
            return self._error_response(error_msg)
        except NoTranscriptFound:
            error_msg = "No transcript found for this video"
            self.logger.error(error_msg)
            return self._error_response(error_msg)
        except VideoUnavailable:
            error_msg = "The video is unavailable"
            self.logger.error(error_msg)
            return self._error_response(error_msg)
        except Exception as e:
            self.logger.error(f'Error in retrieve_and_save_transcript: {e}')
            return self._error_response(str(e))

    @json_schema(
        'Save a YouTube transcript without returning its content.',
        {
            'video_url': {
                'type': 'string',
                'description': 'The URL of the YouTube video.',
                'required': True
            },
            'workspace_name': {
                'type': 'string',
                'description': 'The name of the workspace to save the transcript.',
                'required': False,
                'default': 'project'
            },
            'file_path': {
                'type': 'string',
                'description': "The relative file path and name in the workspace for saving the transcript file. File extension will be '.txt'",
                'required': True
            },
            'include_timestamps': {
                'type': 'boolean',
                'description': 'Whether to include timestamps in the transcript. Default is False.',
                'required': False,
                'default': False
            }
        }
    )
    async def only_save_transcript(self, **kwargs) -> str:
        """
        Save a transcript without returning its content.

        Args:
            kwargs:
                video_url: URL of the YouTube video
                file_name: Optional name for the saved file
                include_timestamps: Whether to include timestamps
                workspace_name: Name of the workspace to save the transcript
                file_path: Optional path for saving the file

        Returns:
            JSON string containing file path or error
        """
        result = json.loads(await self.retrieve_and_save_transcript(**kwargs))
        if 'file_path' in result:
            return json.dumps({
                'file_path': result['file_path'],
                'video_id': result.get('video_id')
            })
        return self._error_response('Failed to retrieve or save transcript.')

    @json_schema(
        'List available transcript languages for a video.',
        {
            'video_url': {
                'type': 'string',
                'description': 'The URL of the YouTube video.',
                'required': True
            }
        }
    )
    async def list_transcript_languages(self, **kwargs) -> str:
        """
        List available transcript languages for a video.

        Args:
            kwargs:
                video_url: URL of the YouTube video

        Returns:
            JSON string containing list of available languages or error
        """
        try:
            video_url = kwargs.get('video_url')
            video_id = self._extract_video_id(video_url)
            if not video_id:
                return self._error_response('Invalid YouTube URL or Video ID could not be extracted.')

            transcript_list = self.transcript_api.list_transcripts(video_id)
            languages = [{
                'language_code': t.language_code,
                'language': t.language,
                'is_generated': t.is_generated
            } for t in transcript_list]

            return json.dumps({
                'languages': languages,
                'video_id': video_id
            })

        except Exception as e:
            self.logger.error(f'Error in list_transcript_languages: {e}')
            return self._error_response(str(e))

    async def _get_transcript(
            self,
            video_url: str,
            video_id: str,
            include_timestamps: bool = False
    ) -> Dict[str, Any]:
        """
        Get transcript for a video with optional caching.

        Args:
            video_url: The video URL
            video_id: The video ID
            include_timestamps: Whether to include timestamps

        Returns:
            Dictionary containing transcript data
        """
        cache_key = f"{video_url}_{include_timestamps}"
        cached_transcript = self.tool_cache.get(cache_key)
        if cached_transcript is not None:
            self.logger.debug(f'Found cached transcript for video: {video_id}')
            return json.loads(cached_transcript)

        self.logger.debug(f'Fetching transcript for video: {video_id}')
        try:
            transcript = self.transcript_api.fetch(video_id)
        except Exception as e:
            self.logger.error(f'Error fetching transcript for video {video_id}: {e}')
            raise YouTubeTranscriptError(f"Unable to fetch transcript: {str(e)}")

        if include_timestamps:
            stripped_transcript = {
                'transcript': [{
                    'text': item['text'],
                    'start': item['start'],
                    'duration': item['duration']
                } for item in transcript]
            }
        else:
            stripped_transcript = {
                'transcript': [item['text'] for item in transcript]
            }

        self.tool_cache.set(cache_key, json.dumps(stripped_transcript))
        return stripped_transcript

    async def _save_transcript(
            self,
            transcript: Dict[str, Any],
            workspace_name: str,
            include_timestamps: bool = False,
            file_path: str = None,
            tool_context: Optional[Dict] = None
    ) -> str:
        """
        Save transcript to a file.

        Args:
            transcript: Transcript data
            file_path: Relative path and file name for the saved file
            include_timestamps: Whether to include timestamps
            workspace_name: Name of the workspace to save the transcript

        Returns:
            Full path to the saved file
        """
        transcript_data = transcript['transcript']
        if not isinstance(transcript_data, list):
            raise YouTubeTranscriptError(f"Transcript data is not a list: {type(transcript_data)}")

        if include_timestamps:
            if not isinstance(transcript_data[0], dict):
                raise YouTubeTranscriptError(
                    f"Unexpected transcript data format with timestamps: {type(transcript_data[0])}"
                )
            # Format with timestamps: [00:00:00] Text
            save_data = '\n'.join(
                f"[{self._format_timestamp(item['start'])}] {item['text']}"
                for item in transcript_data
            )
        else:
            if isinstance(transcript_data[0], str):
                save_data = ' '.join(transcript_data)
            elif isinstance(transcript_data[0], dict):
                save_data = ' '.join(item['text'] for item in transcript_data)
            else:
                raise YouTubeTranscriptError(
                    f"Unexpected transcript data format: {type(transcript_data[0])}"
                )

        # Prepare proper file path, name, etc... and write
        file_path = ensure_file_extension(file_path, 'txt')
        unc_path = create_unc_path(workspace_name, file_path)
        error, workspace, relative_path = self.workspace_tool.validate_and_get_workspace_path(unc_path)
        if error:
            return f"Invalid path: {error}"
        result = await self.workspace_tool.write(path=unc_path, mode='write', data=save_data)
        result_data = json.loads(result)
        if 'error' in result_data:
            return f"Error writing file: {result_data['error']}"

        # Get the full path of the file
        os_path = os_file_system_path(self.workspace_tool, unc_path)

        # Create media event for this.
        await self._raise_render_media(
            sent_by_class=self.__class__.__name__,
            sent_by_function='save_transcript',
            content_type="text/html",
            content=get_file_html(os_path=os_path, unc_path=unc_path, additional_html=f"Transcript for video saved."),
            tool_context=tool_context
        )
        self.logger.debug(f'Saved transcript to workspace: {os_path}')

        return unc_path

    @staticmethod
    def _format_timestamp(seconds: float) -> str:
        """Convert seconds to HH:MM:SS format."""
        hours = int(seconds // 3600)
        minutes = int((seconds % 3600) // 60)
        seconds = int(seconds % 60)
        return f"{hours:02d}:{minutes:02d}:{seconds:02d}"


Toolset.register(YoutubeTranscriptTools, required_tools=['WorkspaceTools'])
import json
import logging
import datetime
from pathlib import Path
from typing import Any, Dict, Optional, Union, List

from agent_c.models.events.render_media import RenderMediaEvent


class SessionLogger:
    """
    A simple logger for recording agent session events to a file.
    
    This class captures events emitted by the agent during a session and writes them
    to a log file for later replay or analysis.
    """
    
    def __init__(self, log_file_path: Union[str, Path], include_system_prompt: bool = True) -> None:
        """
        Initialize a SessionLogger.
        
        Args:
            log_file_path: Path to the log file
            include_system_prompt: Whether to include the system prompt in the log
        """
        self.log_file_path = Path(log_file_path)
        self.include_system_prompt = include_system_prompt
        self.directory_created = False
        
        # Log initialization
        logging.info(f"Initialized SessionLogger writing to {log_file_path}")

    def _ensure_directory_exists(self):
        """
        Ensures the log directory exists, creating it if necessary.
        Returns True if directory exists or was created, False otherwise.
        """
        if self.directory_created:
            # Double-check directory still exists
            if self.log_file_path.parent.exists():
                return True
            else:
                # Directory was deleted after creation
                self.directory_created = False

        try:
            # Create the directory
            self.log_file_path.parent.mkdir(parents=True, exist_ok=True)

            # Verify the directory was created
            if self.log_file_path.parent.exists():
                self.directory_created = True
                logging.info(f"Created log directory for {self.log_file_path}")
                return True
            else:
                logging.error(f"Failed to create log directory for {self.log_file_path}")
                return False
        except Exception as e:
            logging.exception(f"Error creating log directory: {e}")
            return False

    @staticmethod
    def stringify_unserializable(obj):
        return str(obj)

    async def log_event(self, event: Any) -> bool:
        """
        Log a single event to the file with timestamp.

        Args:
            event: The event to log. Can be any object that can be serialized to JSON.
        """
        try:
            if not self._ensure_directory_exists():
                logging.error("Could not ensure log directory exists, event not logged")
                return False

            # Convert datetime to ISO format string
            timestamp = datetime.datetime.now().isoformat()

            # Process the event based on its type
            if hasattr(event, 'model_dump'):
                # It's a Pydantic model
                event_data = event.model_dump()
            elif isinstance(event, str):
                # It's a string, try to parse as JSON first
                try:
                    event_data = json.loads(event)
                except json.JSONDecodeError:
                    # Not JSON, use as-is
                    event_data = event
            else:
                # Use as-is (dict, list, primitive, etc.)
                event_data = event

            # Create the log entry with timestamp
            log_entry = {
                'timestamp': timestamp,
                'event': event_data
            }

            # Write the entry to the log file
            with open(self.log_file_path, 'a', encoding='utf-8') as f:
                f.write(json.dumps(log_entry, default=self.stringify_unserializable) + '\n')
            return True
        except Exception as e:
            logging.exception(f"Error logging event: {e}", exc_info=True)
            return False
    
    async def log_system_prompt(self, prompt: str) -> None:
        """
        Log the system prompt if enabled.
        
        Args:
            prompt: The system prompt text
        """
        if not self.include_system_prompt:
            return

        await self.log_event({
            'type': 'system_prompt',
            'content': prompt
        })

    async def log_user_request(self, user_message: Optional[str] = None,
                               audio_clips: Optional[List] = None,
                               images: Optional[List] = None,
                               files: Optional[List] = None) -> None:
        """
        Log the user's request including text and any multimedia content. We are not logging the actual content at this time, only text, and metadata about multimedia content.

        Args:
            user_message: The text message from the user
            audio_clips: List of audio inputs
            images: List of image inputs
            files: List of file inputs
        """
        try:
            # Create a structured representation of the request
            request_data = {
                'message': user_message
            }

            # Add metadata about multimedia content (not the full content)
            if audio_clips and len(audio_clips) > 0:
                request_data['audio'] = [
                    {
                        'transcript': clip.transcript,
                        'format': clip.format,
                        'duration': getattr(clip, 'duration', None)
                    } for clip in audio_clips
                ]

            if images and len(images) > 0:
                request_data['images'] = [
                    {
                        'content_type': img.content_type,
                        'url': img.url,
                        # Don't include the full base64 content
                        'has_content': img.content is not None
                    } for img in images
                ]

            if files and len(files) > 0:
                request_data['files'] = [
                    {
                        'file_name': file.file_name,
                        'content_type': getattr(file, 'content_type', None),
                        'size': getattr(file, 'size', None)
                    } for file in files
                ]

            await self.log_event({
                'type': 'user_request',
                'data': request_data
            })
        except Exception as e:
            logging.exception(f"Error logging user request: {e}")
            
    async def log_render_media(self, render_media_event: RenderMediaEvent) -> None:
        """
        Log a render_media event, capturing only the essential metadata and not the full content.
        
        Args:
            render_media_event: The RenderMediaEvent object to log
        """
        try:
            # Create a structured representation of the render media event
            # We avoid logging the full content for privacy and size considerations

            media_data = {
                'content_type': render_media_event.content_type,
                'url': render_media_event.url,
                'name': render_media_event.name,
                'content': render_media_event.content,
                'content_bytes': len(render_media_event.content_bytes) if render_media_event.content_bytes else None,
                'sent_by_class': render_media_event.sent_by_class,
                'sent_by_function': render_media_event.sent_by_function,
                'role': render_media_event.role,
                'session_id': render_media_event.session_id
            }
            
            await self.log_event({
                'type': 'render_media',
                'data': media_data
            })
        except Exception as e:
            logging.exception(f"Error logging render media event: {e}")
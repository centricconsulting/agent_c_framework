"""
Loader class for model configuration data.

This module provides a loader class to handle loading, parsing, and saving
of model configurations from JSON files.
"""
import datetime
import json
from pathlib import Path
from typing import Optional, List

from agent_c.models.chat_history import ChatSession
from agent_c.config.config_loader import ConfigLoader

class SavedChatLoader(ConfigLoader):
    """
    Loader for model configuration files.

    Handles loading, parsing, validation, and saving of model configuration
    data from JSON files.
    """

    def __init__(self, config_path: Optional[str] = None):
        super().__init__(config_path)
        self.save_file_folder = Path(self.config_path).joinpath("saved_sessions")

    @property
    def session_id_list(self) -> List[str]:
        """
        Get a list of all saved session IDs.

        Returns:
            List of session IDs as strings
        """
        if not self.save_file_folder.exists():
            return []

        return [f.stem for f in self.save_file_folder.glob("*.json")]

    def load_session_id(self, session_id: str) -> ChatSession:
        """
        Load a chat session by its ID.

        Args:
            session_id: The ID of the session to load

        Returns:
            ChatSession instance with the loaded data

        Raises:
            FileNotFoundError: If the session file doesn't exist
            json.JSONDecodeError: If the JSON is malformed
            ValidationError: If the JSON doesn't match the expected schema
        """
        session_file = self.save_file_folder.joinpath(f"{session_id}.json")

        if not session_file.exists():
            self.logger.warning(f"Session file not found: {session_file}")
            raise FileNotFoundError(f"Session file not found: {session_file}")

        with open(session_file, 'r', encoding='utf-8') as f:
            session_data = json.load(f)

        return ChatSession.model_validate(session_data)

    def save_session(self, session: ChatSession) -> None:
        """
        Save a chat session to a file.

        Args:
            session: ChatSession instance to save

        Raises:
            ValueError: If the session ID is not set
        """
        self.save_file_folder.mkdir(parents=True, exist_ok=True)
        session_file = self.save_file_folder.joinpath(f"{session.session_id}.json")

        with open(session_file, 'w', encoding='utf-8') as f:
            json.dump(session.model_dump(), f, indent=4)

    def delete_session(self, session_id: str) -> None:
        """
        Delete a chat session file by its ID.

        Args:
            session_id: The ID of the session to delete

        Raises:
            FileNotFoundError: If the session file doesn't exist
        """
        session_file = self.save_file_folder.joinpath(f"{session_id}.json")

        if not session_file.exists():
            raise FileNotFoundError(f"Session file not found: {session_file}")

        session: ChatSession = self.load_session_id(session_id)
        session.deleted_at = datetime.datetime.now().isoformat()
        self.save_session(session)
        # move the file to a deleted folder
        deleted_folder = self.save_file_folder.joinpath("deleted")
        deleted_folder.mkdir(parents=True, exist_ok=True)
        session_file.rename(deleted_folder.joinpath(session_file.name))
        self.logger.info(f"Deleted session file: {session_file}")
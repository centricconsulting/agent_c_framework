"""
Loader class for model configuration data.

This module provides a loader class to handle loading, parsing, and saving
of model configurations from JSON files.
"""
import datetime
import json
from pathlib import Path
from typing import Optional, List, Dict

import yaml

from agent_c.models.chat.user import ChatUser
from agent_c.config.config_loader import ConfigLoader
from agent_c.util import MnemonicSlugs

_singleton_instance = None

class UserLoader(ConfigLoader):
    """
    Loader for model configuration files.

    Handles loading, parsing, validation, and saving of model configuration
    data from JSON files.
    """

    def __init__(self, config_path: Optional[str] = None):
        super().__init__(config_path)
        self.save_file_folder = Path(self.config_path).joinpath("users")
        global _singleton_instance
        if _singleton_instance is None:
            _singleton_instance = self

        self._user_cache: Dict[str, ChatUser] = {}
        self._ensure_default_user_exists()

    def _ensure_default_user_exists(self) -> None:
        """
        Ensure that a default user exists in the user loader.
        If no user exists, create a default user with the ID "agent_c_user".
        """
        if not self.user_id_list:
            default_user = ChatUser(user_id="agent_c_user", user_name="Agent C User")
            self.save_user(default_user)
            self.logger.info("Default user created: agent_c_user")

    @classmethod
    def instance(cls) -> 'UserLoader':
        """
        Returns the singleton instance of UserLoader.

        This method ensures that only one instance of UserLoader is created
        and reused throughout the application.

        Returns:
            UserLoader: The singleton instance of UserLoader.
        """
        global _singleton_instance
        if _singleton_instance is None:
            _singleton_instance = cls()
        return _singleton_instance

    @property
    def user_id_list(self) -> List[str]:
        """
        Get a list of all saved user IDs.

        Returns:
            List of user IDs as strings
        """
        if not self.save_file_folder.exists():
            return []

        return [f.stem for f in self.save_file_folder.glob("*.yaml")]

    def load_user_name(self, user_name: str) -> Optional['ChatUser']:
        """
        Load a user by their name from a JSON file.

        Args:
            user_name: The name of the user to load

        Returns:
            ChatUser with the loaded data, or None if not found

        Raises:
            FileNotFoundError: If the user file doesn't exist
            json.JSONDecodeError: If the JSON is malformed
            ValidationError: If the JSON doesn't match the expected schema
        """
        return self.load_user_id(MnemonicSlugs.generate_id_slug(2, user_name))

    def load_user_id(self, user_id: str, force_reload: bool = False, restore_deleted: bool = False) -> 'ChatUser':
        """
        Load a user by their ID from a JSON file.

        Args:
            user_id: The ID of the user to load
            force_reload: If True, will reload the user even if it's already cached
            restore_deleted: If True, will attempt to load from the deleted folder if the user file is not found

        Returns:
            ChatUSer with the loaded data

        Raises:
            FileNotFoundError: If the user file doesn't exist
            json.JSONDecodeError: If the JSON is malformed
            ValidationError: If the JSON doesn't match the expected schema
        """
        if user_id in self._user_cache and not force_reload:
            return self._user_cache[user_id]

        user_file = self.save_file_folder.joinpath(f"{user_id}.yaml")

        if not user_file.exists():
            deleted_user_file = self.save_file_folder.joinpath(f"deleted/{user_id}.yaml")
            if deleted_user_file.exists():
                if not restore_deleted:
                    self.logger.warning(f"Attempted to load deleted User {user_id} without setting restore_deleted=True")
                    raise RuntimeError(f"User {user_id} is deleted and cannot be loaded without setting "
                                       f"restore_deleted=True")
                else:
                    deleted_user_file.rename(user_file)
            else:
                self.logger.warning(f"User file not found: {user_file}")
                raise FileNotFoundError(f"User file not found: {user_file}")

        with open(user_file, 'r', encoding='utf-8') as f:
            user_data = yaml.load(f, Loader=yaml.FullLoader)

        user = ChatUser.model_validate(user_data)
        self._user_cache[user_id] = user
        self.logger.info(f"Loaded user: {user_id} from {user_file}")

        return user

    def save_user(self, user: 'ChatUser') -> None:
        """
        Save a chat user to a file.

        Args:
            user: ChatUser instance to save

        Raises:
            ValueError: If the user ID is not set
        """
        self._user_cache[user.user_id] = user
        self.save_file_folder.mkdir(parents=True, exist_ok=True)
        user_file = self.save_file_folder.joinpath(f"{user.user_id}.yaml")

        with open(user_file, 'w', encoding='utf-8') as f:
            f.write(yaml.dump(user.model_dump(), allow_unicode=True, sort_keys=False))

    def delete_user(self, user_id: str) -> None:
        """
        Delete a chat user file by its ID.

        Args:
            user_id: The ID of the user to delete

        Raises:
            FileNotFoundError: If the user file doesn't exist
        """
        user_file = self.save_file_folder.joinpath(f"{user_id}.yaml")

        if not user_file.exists():
            raise FileNotFoundError(f"Session file not found: {user_file}")

        user: ChatUser = self.load_user_id(user_id)
        user.deleted_at = datetime.datetime.now().isoformat()
        self.save_user(user)
        self._user_cache.pop(user_id, None)
        # move the file to a deleted folder
        deleted_folder = self.save_file_folder.joinpath("deleted")
        deleted_folder.mkdir(parents=True, exist_ok=True)
        user_file.rename(deleted_folder.joinpath(user_file.name))
        self.logger.info(f"Deleted user: {user_id} and moved file to {deleted_folder}")
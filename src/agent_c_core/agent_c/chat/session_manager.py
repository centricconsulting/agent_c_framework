import json
import logging

import yaml
from typing import Union, Optional, Dict
from agent_c_core.agent_c.models.chat_history import ChatSession, MemoryMessage, ChatUser


class ChatSessionManager:
    """
    Interface for managing chat sessions.

    This is the base interface for chat session management and requires derived classes
    to implement all methods for functionality. On its own, it's purely a blueprint.
    """

    def __init__(self) -> None:
        """
        Initializes a new instance of ChatSessionManager with default values.
        """
        self.is_new_user: bool = True
        self.is_new_session: bool = True
        self.user: Optional[ChatUser] = None
        self.chat_session: Optional[ChatSession] = None

    @property
    def active_memory(self):
        return self.chat_session.active_memory

    async def add_message(self, msg: MemoryMessage):
        return await self.chat_session.add_message(msg)

    async def delete_session(self, session_id: str = None) -> None:
        """
        Deletes a chat session.

        Args:
            session_id (str): The ID of the session to delete.
        """
        if session_id is None:
            session_id = self.chat_session.session_id
        pass

    async def new_session(self, session_id: Optional[str] = None) -> str:
        """
        Creates a new chat session, optionally with a given session_id.

        Args:
            session_id (Optional[str]): The session ID to initialize a session with.
                                         If not provided, a new session ID will be generated.

        Returns:
            str: The session ID of the newly created session.
        """
        opts = {'user_id': self.user.user_id}
        if session_id:
            opts['session_id'] = session_id

        self.chat_session = ChatSession(**opts)
        return self.chat_session.session_id

    async def init(self, user_id: str, session_id: Optional[str] = None) -> str:
        """
        Initializes the chat session and user for a given user_id and optional session_id.

        Args:
            user_id (str): The ID of the user.
            session_id (Optional[str]): The ID of the session. If not provided, a new session is created.

        Returns:
            str: The initialized session's ID.
        """
        self.user = ChatUser(user_id=user_id)
        return await self.new_session(session_id)

    async def add_message(self, msg: MemoryMessage) -> None:
        """
        Adds a message to the current chat session.

        Args:
            msg (MemoryMessage): The message to be added to the chat session.

        Raises:
            ValueError: If a chat session is not initialized.
        """
        if self.chat_session is None:
            raise ValueError("Session not initialized")

        await self.chat_session.add_message(msg)

    async def update(self) -> None:
        """
        Asynchronously updates the cached session and user. Meant to sync in-memory changes.
        """
        pass

    async def flush(self) -> None:
        """
        Asynchronously flushes the changes for both the session and user back to the datastore.
        """
        pass

    def filtered_session_meta(self, prefix: str) -> Dict:
        """
        Returns session meta information filtered by the specified prefix.

        Args:
            prefix (str): The prefix to filter for.

        Returns:
            Dict: Dictionary containing filtered session metadata.
        """
        return self.get_session_meta_meta(prefix)

    def filtered_user_meta(self, prefix: str) -> Dict:
        """
        Returns user meta information filtered by the specified prefix.

        Args:
            prefix (str): The prefix to filter for.

        Returns:
            Dict: Dictionary containing filtered user metadata.
        """
        return self.get_user_meta_meta(prefix)

    def get_user_meta_meta(self, prefix: str) -> Dict:
        """
        Retrieves 'metameta' user metadata associated with the given prefix.

        Args:
            prefix (str): The desired metadata prefix.

        Returns:
            Dict: A dictionary containing the desired user metadata.

        Raises:
            Exception: If metadata cannot be decoded properly.
        """
        json_str = self.user.metadata.get('metameta', {}).get(prefix, '{}')
        try:
            return json.loads(json_str)
        except Exception as e:
            logging.exception(f"Failed to decode metameta for {prefix}")
            return {}

    def set_user_meta_meta(self, prefix: str, metameta: Dict) -> None:
        """
        Sets 'metameta' user metadata under the specified prefix.

        Args:
            prefix (str): The prefix under which metadata should be stored.
            metameta (Dict): The metadata to set.

        Raises:
            Exception: If metadata cannot be encoded into JSON.
        """
        try:
            json_str = json.dumps(metameta)
        except Exception as e:
            logging.exception(f"Failed to encode metameta for {prefix}")
            raise

        if not self.user.metadata.get('metameta'):
            self.user.metadata['metameta'] = {prefix: json_str}
        else:
            self.user.metadata['metameta'][prefix] = json_str

    def get_session_meta_meta(self, prefix: str) -> Dict:
        """
        Retrieves 'metameta' session metadata associated with the given prefix.

        Args:
            prefix (str): The desired metadata prefix.

        Returns:
            Dict: A dictionary containing the desired session metadata.

        Raises:
            Exception: If metadata cannot be decoded properly.
        """
        json_str = self.chat_session.metadata.get('metameta', {}).get(prefix, '{}')
        try:
            return json.loads(json_str)
        except Exception:
            logging.exception(f"Failed to decode session metameta for {prefix}")
            return {}

    def set_session_meta_meta(self, prefix: str, metameta: Dict) -> None:
        """
        Sets 'metameta' session metadata under the specified prefix.

        Args:
            prefix (str): The prefix under which the metadata should be stored.
            metameta (Dict): The metadata to store.

        Raises:
            Exception: If metadata cannot be encoded into JSON.
        """
        try:
            json_str = json.dumps(metameta)
        except Exception as e:
            logging.exception(f"Failed to encode session metameta for {prefix}")
            raise

        if not self.chat_session.metadata.get('metameta'):
            self.chat_session.metadata['metameta'] = {prefix: json_str}
        else:
            self.chat_session.metadata['metameta'][prefix] = json_str

    @staticmethod
    def dict_to_yaml(data_dict: Dict) -> str:
        """
        Converts a dictionary into YAML format.

        Args:
            data_dict (Dict): The dictionary to convert.

        Returns:
            str: The YAML string representation of the dictionary.

        Raises:
            yaml.YAMLError: If an error occurs during YAML conversion.
        """
        try:
            return yaml.dump(data_dict)
        except yaml.YAMLError as exc:
            logging.error(f"Error serializing to YAML: {exc}")
            return ""

    def filtered_session_meta_string(self, prefix: str) -> str:
        """
        Returns filtered session meta information as a YAML string.

        Args:
            prefix (str): The prefix to filter session meta information for.

        Returns:
            str: YAML string representation of the filtered session meta.
        """
        return self.dict_to_yaml(self.filtered_session_meta(prefix))

    def filtered_user_meta_string(self, prefix: str) -> str:
        """
        Returns filtered user meta information as a YAML string.

        Args:
            prefix (str): The prefix to filter user meta information for.

        Returns:
            str: YAML string representation of the filtered user meta.
        """
        return self.dict_to_yaml(self.filtered_user_meta(prefix))

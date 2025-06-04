import json
import yaml

from typing import Optional, Dict, List

from agent_c.config.saved_chat import SavedChatLoader
from agent_c.models.chat_history import ChatSession
from agent_c.util.logging_utils import LoggingManager



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
        self._loader: SavedChatLoader = SavedChatLoader()
        self.session_id_list: List[str] = self._loader.session_id_list
        self._session_cache: Dict[str, ChatSession] = {}
        self.logger = LoggingManager(__name__).get_logger()

    async def delete_session(self, session_id) -> None:
        """
        Deletes a chat session by its ID.

        Args:
            session_id (str): The ID of the session to delete.
        """
        if session_id in self._session_cache:
            del self._session_cache[session_id]
        if session_id in self.session_id_list:
            self.session_id_list.remove(session_id)

        self._loader.delete_session(session_id)

    async def new_session(self, session: ChatSession):
        self._session_cache[session.session_id] = session
        self.session_id_list.append(session.session_id)
        session.touch()
        self._loader.save_session(session)

    async def get_session(self, session_id: str) -> Optional[ChatSession]:
        """
        Retrieves a chat session by its ID.

        Args:
            session_id (str): The ID of the session to retrieve.

        Returns:
            Optional[ChatSession]: The chat session if found, otherwise None.
        """
        if session_id in self._session_cache:
            return self._session_cache[session_id]

        if session_id in self.session_id_list:
            session = self._loader.load_session_id(session_id)
            self._session_cache[session_id] = session
            return session

        return None

    async def update(self) -> None:
        """
        Asynchronously updates the cached session and user. Meant to sync in-memory changes.
        """
        pass

    async def flush(self, session_id: str) -> None:
        self._loader.save_session(self._session_cache[session_id])

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
            self.logger.exception(f"Failed to decode metameta for {prefix}")
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
            self.logger.exception(f"Failed to encode metameta for {prefix}")
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
            self.logger.exception(f"Failed to decode session metameta for {prefix}")
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
            self.logger.exception(f"Failed to encode session metameta for {prefix}")
            raise

        if not self.chat_session.metadata.get('metameta'):
            self.chat_session.metadata['metameta'] = {prefix: json_str}
        else:
            self.chat_session.metadata['metameta'][prefix] = json_str


    def dict_to_yaml(self, data_dict: Dict) -> str:
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
            self.logger.error(f"Error serializing to YAML: {exc}")
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

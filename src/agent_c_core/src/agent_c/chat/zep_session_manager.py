import os
import json
import os
import yaml
import logging

from logging import Logger
from typing import Optional, Any, List, TYPE_CHECKING

from zep_cloud import Message

if TYPE_CHECKING:
    from agent_c.util.pruning.chat_log_pruner import ChatLogPruner

if os.environ.get("ZEP_CE_KEY"):
    os.environ["ZEP_API_KEY"] = os.environ.get("ZEP_CE_KEY")
    from zep_python.client import AsyncZep
    from zep_python.errors import NotFoundError, InternalServerError, BadRequestError
    class UnauthorizedError(Exception):
        pass
else:
    from zep_cloud.client import AsyncZep
    from zep_cloud.errors import NotFoundError, InternalServerError, BadRequestError, UnauthorizedError

from agent_c import ChatSessionManager, MemoryMessage
from agent_c.util.slugs import MnemonicSlugs

# TODOS:
# - clean up exception handling and add retries


class ZepCESessionManager(ChatSessionManager):
    _DEFAULT_META_DATA = {"session_name": "New Chat"}

    def __init__(self, zep_client: AsyncZep = None, default_meta_data: Optional[dict[str,Any]] = None, allow_auto_user_create: bool = False, logger: Optional[Logger] = None) -> None:
        super().__init__()
        self.zep_client = zep_client or AsyncZep()
        self.default_meta_data = default_meta_data or self._DEFAULT_META_DATA
        self.__allow_auto_user_create = allow_auto_user_create
        self.logger = logger or logging.getLogger(__name__)
        self.session_id: Optional[str] = None
        self.user_id: Optional[str] = None
        self.chat_session  = None
        self.user  = None
        self.is_new_session = True
        self.is_new_user = True
        self._pruner: Optional["ChatLogPruner"] = None
        self._active_memory = None

    @property
    def active_memory(self):
        return self._active_memory

    async def add_message(self, msg: MemoryMessage):
        z_msg = Message(role=msg.role, content=msg.content, uuid=msg.uuid, role_type=msg.role, metadata=msg.metadata,
                        token_count=msg.token_count)
        return await self.zep_client.memory.add(self.session_id, messages=[z_msg])

    async def add_messages(self, msgs: List[MemoryMessage]):
        return await self.zep_client.memory.add(self.session_id,
                                                messages=[Message(role=msg.role, content=msg.content,
                                                                  uuid=msg.uuid, role_type=msg.role,
                                                                  metadata=msg.metadata, token_count=msg.token_count)
                                                          for msg in msgs])

    async def init(self, user_id: str, session_id: Optional[str] = None):
        await self.__ensure_user_exists(user_id)
        self.user_id = user_id

        await self.__ensure_session_exists(session_id)
        self.session_id = self.chat_session.session_id

        await self.__fetch_memory()

    async def __fetch_memory(self):
        try:
            self._active_memory = await self.zep_client.memory.get(self.session_id)
        except NotFoundError:
            self.logger.exception(f"Memory not found for session {self.session_id}")
        except InternalServerError:
            self.logger.exception(f"ZEP API error, when fetching memory for session {self.session_id}")
        except ConnectionError:
            self.logger.exception(f"ZEP connection error, when fetching memory for session {self.session_id}")

    async def __ensure_user_exists(self, user_id: str):
        try:
            self.user = await self.zep_client.user.get(user_id)
            self.is_new_user = False
        except NotFoundError:
            if self.__allow_auto_user_create:
                # First time user.
                await self.__add_user(user_id)
            else:
                raise
        except InternalServerError:
            self.logger.exception(f"ZEP API error, when getting user {user_id}")
            raise
        except UnauthorizedError:
            self.logger.exception(f"Unauthorized to get user {user_id}")
            raise
        except BadRequestError:
            self.logger.exception(f"Bad request when getting user {user_id}")
            raise
        except ConnectionError:
            self.logger.exception(f"ZEP connection error, when getting user {user_id}")
            raise
        except Exception as e:
            self.logger.exception(f"Unknown error when getting user {user_id}")
            raise

    async def __add_user(self, user_id: str):
        try:
            self.user = await self.zep_client.user.add(user_id=user_id, email="demo@demo.com", first_name="Demo", last_name="User")
        except InternalServerError:
            self.logger.exception(f"ZEP API error, when adding user {user_id}")
            raise
        except UnauthorizedError:
            self.logger.exception(f"Unauthorized to add user {user_id}")
            raise
        except BadRequestError:
            self.logger.exception(f"Bad request when adding user {user_id}")
            raise
        except ConnectionError:
            self.logger.exception(f"ZEP connection error, when adding user {user_id}")
            raise

    async def __ensure_session_exists(self, session_id: Optional[str]):
        if session_id is None:
            return await self.__new_zep_session()
        try:
            session = await self.zep_client.memory.get_session(session_id)

            # TODO: Probably wanna allow exceptions to this
            if session.user_id != self.user.user_id:
                self.logger.warning(f"User {self.user.user_id} attempted to access session {session.session_id} which belongs to {session.user_id}.  Starting new session instead")
                return self.__new_zep_session()

            self.chat_session = session
            self.is_new_session = False

        except NotFoundError:
            return await self.__new_zep_session()
        except InternalServerError:
            self.logger.exception(f"ZEP API error, when getting session ID {session_id}")
            raise
        except UnauthorizedError:
            self.logger.exception(f"Unauthorized to get session ID {session_id}")
            raise
        except BadRequestError:
            self.logger.exception(f"Bad request when getting session ID {session_id}")
            raise
        except ConnectionError:
            self.logger.exception(f"ZEP connection error, getting session ID {session_id}")
            raise

    async def __new_zep_session(self):
        session_id = MnemonicSlugs.generate_slug(3)

        try:
            # TODO: This is gonna collide eventually, check for uniqueness
            self.chat_session = await self.zep_client.memory.add_session(session_id=session_id, user_id=self.user.user_id, metadata=self.default_meta_data)
        except InternalServerError:
            self.logger.exception(f"ZEP API error, when adding session {session_id}")
        except ConnectionError:
            self.logger.exception(f"ZEP connection error, adding session {session_id}")

    async def update(self) -> None:
        """
        Asynchronously updates the cached session and user by fetching the latest
        data from the Zep API.
        """
        try:
            self.chat_session = await self.zep_client.memory.get_session(self.session_id)
        except Exception as e:
            self.logger.info(f"Failed to update session cache for session ID {self.session_id}: {e}")

        try:
            if self.user is None:
                self.user = await self.zep_client.user.get(self.user_id)
        except Exception as e:
            self.logger.info(f"Failed to update user cache for user ID {self.user_id}: {e}")

        await self.__fetch_memory()

    async def flush(self) -> None:
        """
        Asynchronously flushes the changes to the cached session and user back to the Zep API.
        """
        if self.chat_session:
            try:
                await self.zep_client.memory.update_session(session_id=self.session_id, metadata=self.chat_session.metadata)
            except Exception as e:
                self.logger.info(f"Failed to flush session changes to the Zep API for session ID {self.session_id}: {e}")

        if self.user:
            try:
                await self.zep_client.user.update(user_id=self.user_id,
                                                  email=self.user.email,
                                                  first_name=self.user.first_name,
                                                  last_name=self.user.last_name,
                                                  metadata=self.user.metadata)
            except Exception as e:
                self.logger.info(f"Failed to flush user changes to the Zep API for user ID {self.user_id}: {e}")

    def filtered_session_meta(self, prefix: str):
        if self.chat_session is None or self.chat_session.metadata is None:
            return {}

        return self.get_session_meta_meta(prefix)

    def filtered_user_meta(self, prefix: str):
        if self.user is None or self.user.metadata is None:
            return {}

        return self.get_user_meta_meta(prefix)

    def get_user_meta_meta(self, prefix) -> dict:
        json_str = self.user.metadata.get('metameta', {}).get(prefix, '{}')
        try:
            return json.loads(json_str)
        except Exception as e:
            self.logger.exception(f"Failed to decode metameta for {prefix}")

        return {}

    def set_user_meta_meta(self, prefix, metameta):
        try:
            json_str = json.dumps(metameta)
        except Exception as e:
            self.logger.exception(f"Failed to encode metameta for {prefix}")
            raise

        if not self.user.metadata.get('metameta'):
            self.user.metadata['metameta'] = {prefix: json_str}
        else:
            self.user.metadata['metameta'][prefix] = json_str

    def get_session_meta_meta(self, prefix) -> dict:
        json_str = self.chat_session.metadata.get('metameta', {}).get(prefix, '{}')
        try:
            return json.loads(json_str)
        except Exception as e:
            self.logger.exception(f"Failed to decode session metameta for {prefix}")

        return {}

    def set_session_meta_meta(self, prefix, metameta):
        try:
            json_str = json.dumps(metameta)
        except Exception as e:
            self.logger.exception(f"Failed to encode session metameta for {prefix}")
            raise

        if not self.chat_session.metadata.get('metameta'):
            self.chat_session.metadata['metameta'] = {prefix: json_str}
        else:
            self.chat_session.metadata['metameta'][prefix] = json_str

    def kvps_as_str(self, kvps: dict) -> str:
        try:
            return yaml.dump(kvps)
        except yaml.YAMLError as exc:
            # Handle YAML serialization errors if they occur
            print(f"Error serializing to YAML: {exc}")
            return ""

    def filtered_session_meta_string(self, prefix: str):
        return self.kvps_as_str(self.filtered_session_meta(prefix))

    def filtered_user_meta_string(self, prefix: str):
        return self.kvps_as_str(self.filtered_user_meta(prefix))
    
    def _get_memory_messages(self) -> List[MemoryMessage]:
        """
        Get messages from Zep active memory as MemoryMessage objects.
        
        Returns:
            List of MemoryMessage objects
        """
        if self._active_memory is None:
            return []
            
        messages = []
        # Zep memory has a messages attribute with Message objects
        if hasattr(self._active_memory, 'messages') and self._active_memory.messages:
            for zep_msg in self._active_memory.messages:
                try:
                    # Convert Zep Message to MemoryMessage
                    message = MemoryMessage(
                        role=zep_msg.role,
                        content=zep_msg.content,
                        uuid=getattr(zep_msg, 'uuid', None),
                        token_count=getattr(zep_msg, 'token_count', None),
                        metadata=getattr(zep_msg, 'metadata', None)
                    )
                    messages.append(message)
                except Exception as e:
                    self.logger.warning(f"Failed to convert Zep message to MemoryMessage: {e}")
                    continue
                    
        return messages
    
    async def _update_session_messages(self, pruned_messages: List[MemoryMessage]) -> None:
        """
        Update the Zep session with pruned messages.
        
        Args:
            pruned_messages: List of messages after pruning
        """
        if self.session_id is None:
            self.logger.warning("No session_id available for updating Zep messages")
            return
            
        try:
            # Convert MemoryMessage objects to Zep Message objects
            zep_messages = []
            for msg in pruned_messages:
                zep_msg = Message(
                    role=msg.role,
                    content=msg.content,
                    uuid=msg.uuid,
                    role_type=msg.role,
                    metadata=msg.metadata,
                    token_count=msg.token_count
                )
                zep_messages.append(zep_msg)
            
            # Note: Zep doesn't have a direct "replace all messages" API
            # This is a limitation - we'd need to delete and recreate the session
            # For now, we'll update the local memory and log a warning
            self.logger.warning(
                f"Zep doesn't support direct message replacement. "
                f"Pruning performed locally but not synced to Zep for session {self.session_id}"
            )
            
            # Update local memory structure if it exists
            if self._active_memory is not None:
                # Update the local memory with pruned messages
                if hasattr(self._active_memory, 'messages'):
                    self._active_memory.messages = zep_messages
                    
                # Update token count if the memory object supports it
                if hasattr(self._active_memory, 'token_count'):
                    from agent_c.util.pruning.message_analyzer import calculate_total_tokens
                    self._active_memory.token_count = calculate_total_tokens(pruned_messages)
                    
        except Exception as e:
            self.logger.error(f"Error updating Zep session messages: {e}", exc_info=True)

    def get_pruner(self) -> Optional["ChatLogPruner"]:
        """
        Get the chat log pruner instance for this session manager.
        
        Returns:
            Optional[ChatLogPruner]: The pruner instance, or None if not configured
        """
        return self._pruner

    def set_pruner(self, pruner: Optional["ChatLogPruner"]) -> None:
        """
        Set the chat log pruner instance for this session manager.
        
        Args:
            pruner: The pruner instance to use, or None to disable pruning
        """
        self._pruner = pruner

    def prune_session_if_needed(self, session_id: str, context_limit: int) -> bool:
        """
        Prune the session messages if needed based on token count and context limit.
        
        Args:
            session_id (str): The ID of the session to potentially prune
            context_limit (int): Maximum context window size for the model
            
        Returns:
            bool: True if pruning was performed, False otherwise
        """
        if self._pruner is None:
            return False
            
        try:
            # Get current session messages
            messages = self.get_session_messages()
            if not messages:
                return False
                
            # Check if pruning is needed
            if not self._pruner.should_prune(messages, context_limit):
                return False
                
            self.logger.info(f"Pruning session {session_id} - {len(messages)} messages, context limit: {context_limit}")
            
            # Perform pruning
            result = self._pruner.prune_messages(messages, context_limit)
            
            if result.pruned_messages != messages:
                # Update the session with pruned messages
                self._update_session_messages(result.pruned_messages)
                
                self.logger.info(
                    f"Pruning completed for session {session_id}: "
                    f"{len(messages)} -> {len(result.pruned_messages)} messages, "
                    f"{result.tokens_before} -> {result.tokens_after} tokens"
                )
                return True
                
        except Exception as e:
            self.logger.error(f"Error during session pruning: {e}", exc_info=True)
            
        return False

    def get_session_messages(self) -> List[MemoryMessage]:
        """
        Get the current session messages.
        
        Returns:
            List[MemoryMessage]: Current session messages
        """
        return self._get_memory_messages()

class ZepCloudSessionManager(ZepCESessionManager):
    pass
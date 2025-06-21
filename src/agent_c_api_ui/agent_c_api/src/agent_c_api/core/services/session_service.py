import time

from typing import Dict, List, Any, Optional

from agent_c.models.chat.chat_session import ChatSession
from agent_c_api.core.util.logging_utils import LoggingManager
from agent_c_api.core.repositories.session_repository import SessionRepository


class SessionService:
    """
    Service for managing chat sessions and session data.
    """
    
    def __init__(self, session_repository: SessionRepository):
        """
        Initialize the session service.
        
        Args:
            session_repository (SessionRepository): Session repository instance
        """
        self.session_repository = session_repository
        self.logger = LoggingManager(self.__class__.__name__).get_logger()
        
        self.logger.info(
            "session_service_initialized",
            repository_type=type(session_repository).__name__
        )
    
    async def create_session(self, session: ChatSession) -> ChatSession:
        """
        Create a new chat session.
        
        Args:
            session (ChatSession): The session to create
            
        Returns:
            ChatSession: The created session with updated fields
        """
        start_time = time.time()
        
        try:
            session_id = getattr(session, 'session_id', 'unknown')
            user_id = getattr(session, 'user_id', 'unknown')
            
            self.logger.info(
                "session_creating",
                session_id=session_id,
                user_id=user_id
            )
            
            created_session = await self.session_repository.add_session(session)
            
            duration = time.time() - start_time
            self.logger.info(
                "session_created",
                session_id=getattr(created_session, 'session_id', session_id),
                user_id=getattr(created_session, 'user_id', user_id),
                duration_ms=round(duration * 1000, 2)
            )
            
            return created_session
            
        except Exception as e:
            duration = time.time() - start_time
            self.logger.error(
                "session_creation_failed",
                session_id=session_id if 'session_id' in locals() else 'unknown',
                user_id=user_id if 'user_id' in locals() else 'unknown',
                error=str(e),
                duration_ms=round(duration * 1000, 2)
            )
            raise
    
    async def get_session(self, session_id: str) -> Optional[ChatSession]:
        """
        Get a chat session by ID.
        
        Args:
            session_id (str): The session ID
            
        Returns:
            Optional[ChatSession]: The session if found, None otherwise
        """
        start_time = time.time()
        
        try:
            self.logger.info(
                "session_retrieving",
                session_id=session_id
            )
            
            session = await self.session_repository.get_session(session_id)
            
            duration = time.time() - start_time
            if session:
                self.logger.info(
                    "session_retrieved",
                    session_id=session_id,
                    found=True,
                    user_id=getattr(session, 'user_id', 'unknown'),
                    duration_ms=round(duration * 1000, 2)
                )
            else:
                self.logger.info(
                    "session_not_found",
                    session_id=session_id,
                    found=False,
                    duration_ms=round(duration * 1000, 2)
                )
            
            return session
            
        except Exception as e:
            duration = time.time() - start_time
            self.logger.error(
                "session_retrieval_failed",
                session_id=session_id,
                error=str(e),
                duration_ms=round(duration * 1000, 2)
            )
            raise
    
    async def update_session(self, session: ChatSession) -> ChatSession:
        """
        Update an existing chat session.
        
        Args:
            session (ChatSession): The session to update
            
        Returns:
            ChatSession: The updated session
        """
        start_time = time.time()
        
        try:
            session_id = getattr(session, 'session_id', 'unknown')
            user_id = getattr(session, 'user_id', 'unknown')
            
            self.logger.info(
                "session_updating",
                session_id=session_id,
                user_id=user_id
            )
            
            updated_session = await self.session_repository.add_session(session)
            
            duration = time.time() - start_time
            self.logger.info(
                "session_updated",
                session_id=getattr(updated_session, 'session_id', session_id),
                user_id=getattr(updated_session, 'user_id', user_id),
                duration_ms=round(duration * 1000, 2)
            )
            
            return updated_session
            
        except Exception as e:
            duration = time.time() - start_time
            self.logger.error(
                "session_update_failed",
                session_id=session_id if 'session_id' in locals() else 'unknown',
                user_id=user_id if 'user_id' in locals() else 'unknown',
                error=str(e),
                duration_ms=round(duration * 1000, 2)
            )
            raise
    
    async def delete_session(self, session_id: str) -> bool:
        """
        Delete a chat session.
        
        Args:
            session_id (str): The session ID
            
        Returns:
            bool: True if deleted, False otherwise
        """
        start_time = time.time()
        
        try:
            self.logger.info(
                "session_deleting",
                session_id=session_id
            )
            
            success = await self.session_repository.delete_session(session_id)
            
            duration = time.time() - start_time
            if success:
                self.logger.info(
                    "session_deleted",
                    session_id=session_id,
                    success=True,
                    duration_ms=round(duration * 1000, 2)
                )
            else:
                self.logger.warning(
                    "session_deletion_failed",
                    session_id=session_id,
                    success=False,
                    reason="session_not_found_or_already_deleted",
                    duration_ms=round(duration * 1000, 2)
                )
            
            return success
            
        except Exception as e:
            duration = time.time() - start_time
            self.logger.error(
                "session_deletion_error",
                session_id=session_id,
                error=str(e),
                duration_ms=round(duration * 1000, 2)
            )
            raise
    
    async def get_user_sessions(self, user_id: str) -> List[str]:
        """
        Get all session IDs for a user.
        
        Args:
            user_id (str): The user ID
            
        Returns:
            List[str]: List of session IDs
        """
        start_time = time.time()
        
        try:
            self.logger.info(
                "user_sessions_retrieving",
                user_id=user_id
            )
            
            sessions = await self.session_repository.get_user_sessions(user_id)
            
            duration = time.time() - start_time
            self.logger.info(
                "user_sessions_retrieved",
                user_id=user_id,
                sessions_count=len(sessions),
                duration_ms=round(duration * 1000, 2)
            )
            
            return sessions
            
        except Exception as e:
            duration = time.time() - start_time
            self.logger.error(
                "user_sessions_retrieval_failed",
                user_id=user_id,
                error=str(e),
                duration_ms=round(duration * 1000, 2)
            )
            raise
    
    async def get_all_sessions(self) -> List[str]:
        """
        Get all session IDs.
        
        Returns:
            List[str]: List of all session IDs
        """
        start_time = time.time()
        
        try:
            self.logger.info(
                "all_sessions_retrieving"
            )
            
            sessions = await self.session_repository.get_all_sessions()
            
            duration = time.time() - start_time
            self.logger.info(
                "all_sessions_retrieved",
                sessions_count=len(sessions),
                duration_ms=round(duration * 1000, 2)
            )
            
            return sessions
            
        except Exception as e:
            duration = time.time() - start_time
            self.logger.error(
                "all_sessions_retrieval_failed",
                error=str(e),
                duration_ms=round(duration * 1000, 2)
            )
            raise
    
    async def add_message(self, session_id: str, message: Dict[str, Any]) -> bool:
        """
        Add a message to a session's active memory.
        
        Args:
            session_id (str): The session ID
            message (Dict[str, Any]): The message to add
            
        Returns:
            bool: True if added, False otherwise
        """
        start_time = time.time()
        
        try:
            message_id = message.get('id', 'unknown')
            
            self.logger.info(
                "session_message_adding",
                session_id=session_id,
                message_id=message_id
            )
            
            success = await self.session_repository.add_message_to_session(session_id, message)
            
            duration = time.time() - start_time
            if success:
                self.logger.info(
                    "session_message_added",
                    session_id=session_id,
                    message_id=message_id,
                    success=True,
                    duration_ms=round(duration * 1000, 2)
                )
            else:
                self.logger.warning(
                    "session_message_add_failed",
                    session_id=session_id,
                    message_id=message_id,
                    success=False,
                    reason="session_not_found_or_memory_issue",
                    duration_ms=round(duration * 1000, 2)
                )
            
            return success
            
        except Exception as e:
            duration = time.time() - start_time
            self.logger.error(
                "session_message_add_error",
                session_id=session_id,
                message_id=message_id if 'message_id' in locals() else 'unknown',
                error=str(e),
                duration_ms=round(duration * 1000, 2)
            )
            raise
    
    async def create_new_memory(self, session_id: str, name: Optional[str] = None,
                             description: Optional[str] = None) -> Optional[int]:
        """
        Create a new memory for a session and make it active.
        
        Args:
            session_id (str): The session ID
            name (Optional[str]): Optional name for the memory
            description (Optional[str]): Optional description for the memory
            
        Returns:
            Optional[int]: Index of the new memory or None if session doesn't exist
        """
        start_time = time.time()
        
        try:
            self.logger.info(
                "session_memory_creating",
                session_id=session_id,
                memory_name=name,
                has_description=description is not None
            )
            
            memory_index = await self.session_repository.create_new_memory(session_id, name, description)
            
            duration = time.time() - start_time
            if memory_index is not None:
                self.logger.info(
                    "session_memory_created",
                    session_id=session_id,
                    memory_index=memory_index,
                    memory_name=name,
                    duration_ms=round(duration * 1000, 2)
                )
            else:
                self.logger.warning(
                    "session_memory_creation_failed",
                    session_id=session_id,
                    memory_name=name,
                    reason="session_not_found",
                    duration_ms=round(duration * 1000, 2)
                )
            
            return memory_index
            
        except Exception as e:
            duration = time.time() - start_time
            self.logger.error(
                "session_memory_creation_error",
                session_id=session_id,
                memory_name=name,
                error=str(e),
                duration_ms=round(duration * 1000, 2)
            )
            raise
    
    async def switch_active_memory(self, session_id: str, memory_index: int) -> bool:
        """
        Switch the active memory of a session.
        
        Args:
            session_id (str): The session ID
            memory_index (int): Index of the memory to make active
            
        Returns:
            bool: True if switched, False if memory doesn't exist
        """
        start_time = time.time()
        
        try:
            self.logger.info(
                "session_memory_switching",
                session_id=session_id,
                target_memory_index=memory_index
            )
            
            success = await self.session_repository.switch_active_memory(session_id, memory_index)
            
            duration = time.time() - start_time
            if success:
                self.logger.info(
                    "session_memory_switched",
                    session_id=session_id,
                    active_memory_index=memory_index,
                    success=True,
                    duration_ms=round(duration * 1000, 2)
                )
            else:
                self.logger.warning(
                    "session_memory_switch_failed",
                    session_id=session_id,
                    target_memory_index=memory_index,
                    success=False,
                    reason="memory_index_not_found",
                    duration_ms=round(duration * 1000, 2)
                )
            
            return success
            
        except Exception as e:
            duration = time.time() - start_time
            self.logger.error(
                "session_memory_switch_error",
                session_id=session_id,
                target_memory_index=memory_index,
                error=str(e),
                duration_ms=round(duration * 1000, 2)
            )
            raise
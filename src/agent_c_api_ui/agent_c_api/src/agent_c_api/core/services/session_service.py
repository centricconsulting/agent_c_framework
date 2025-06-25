import time
from typing import Dict, List, Any, Optional, Union

from selenium.webdriver.common.devtools.v85.overlay import set_show_viewport_size_on_resize

from agent_c.util.event_session_logger_factory import LoggerConfiguration
from agent_c.util.structured_logging import get_logger, LoggingContext
from agent_c.models.chat_history.chat_session import ChatSession
from agent_c.models.chat_history.chat_memory import ChatMemory
from ..repositories.session_repository import SessionRepository


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
        self.logger = get_logger(__name__)
        
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
        session_id = getattr(session, 'session_id', 'unknown')
        user_id = getattr(session, 'user_id', 'unknown')
        duration = time.time() - start_time

        with LoggingContext(session_id=session_id,user_id=user_id,
                            duration_ms=round(duration * 1000, 2)):
            try:
                self.logger.info("session_creating")

                created_session = await self.session_repository.add_session(session)


                self.logger.info("session_created")

                return created_session

            except Exception as e:
                duration = time.time() - start_time
                self.logger.error(
                    "session_creation_failed",error=str(e))
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
        duration = time.time() - start_time

        with LoggingContext(session_id=session_id,
                            duration_ms=round(duration * 1000, 2)):
            try:
                self.logger.info("session_retrieving")

                session = await self.session_repository.get_session(session_id)

                if session:
                    self.logger.info(
                        "session_retrieved",
                        found=True,
                        user_id=getattr(session, 'user_id', 'unknown'),
                    )
                else:
                    self.logger.info(
                        "session_not_found",
                        found=False,
                    )

                return session

            except Exception as e:
                duration = time.time() - start_time
                self.logger.error(
                    "session_retrieval_failed",
                    error=str(e),
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
        session_id = getattr(session, 'session_id', 'unknown')
        user_id = getattr(session, 'user_id', 'unknown')
        duration = time.time() - start_time

        with LoggingContext(session_id=session_id,user_id=user_id,
                            duration_ms=round(duration * 1000, 2)):
            try:
                self.logger.info("session_updating")

                updated_session = await self.session_repository.add_session(session)
                self.logger.info("session_updated")

                return updated_session

            except Exception as e:
                duration = time.time() - start_time
                self.logger.error(
                    "session_update_failed",
                    session_id=session_id if 'session_id' in locals() else 'unknown',
                    user_id=user_id if 'user_id' in locals() else 'unknown',
                    error=str(e),
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
        duration = time.time() - start_time
        with LoggingContext(session_id=session_id,duration_ms=round(duration * 1000, 2)):
            try:
                self.logger.info("session_deleting")

                success = await self.session_repository.delete_session(session_id)


                if success:
                    self.logger.info(
                        "session_deleted",
                         success=True,
                    )
                else:
                    self.logger.warning(
                        "session_deletion_failed",
                        success=False,
                        reason="session_not_found_or_already_deleted",
                    )

                return success

            except Exception as e:
                duration = time.time() - start_time
                self.logger.error(
                    "session_deletion_error",
                    error=str(e),
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
        duration = time.time() - start_time
        with LoggingContext(user_id=user_id,duration_ms=round(duration * 1000, 2)):
            try:
                self.logger.info("user_sessions_retrieving")

                sessions = await self.session_repository.get_user_sessions(user_id)


                self.logger.info(
                    "user_sessions_retrieved",
                    sessions_count=len(sessions),
                )

                return sessions

            except Exception as e:
                duration = time.time() - start_time
                self.logger.error(
                    "user_sessions_retrieval_failed",
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
        message_id = message.get('id', 'unknown')
        duration = time.time() - start_time
        with LoggingContext(session_id=session_id,message_id=message_id,
                            duration_ms=round(duration * 1000, 2)):
            try:


                self.logger.info("session_message_adding")

                success = await self.session_repository.add_message_to_session(session_id, message)


                if success:
                    self.logger.info(
                        "session_message_added",
                        success=True,
                    )
                else:
                    self.logger.warning(
                        "session_message_add_failed",
                        success=False,
                        reason="session_not_found_or_memory_issue",
                    )

                return success

            except Exception as e:
                duration = time.time() - start_time
                self.logger.error(
                    "session_message_add_error",
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
        with LoggingContext(session_id=session_id,memory_name=name):
            try:
                self.logger.info(
                    "session_memory_creating",
                    has_description=description is not None
                )

                memory_index = await self.session_repository.create_new_memory(session_id, name, description)

                duration = time.time() - start_time
                if memory_index is not None:
                    self.logger.info(
                        "session_memory_created",
                        memory_index=memory_index,
                        duration_ms=round(duration * 1000, 2)
                    )
                else:
                    self.logger.warning(
                        "session_memory_creation_failed",
                        reason="session_not_found",
                        duration_ms=round(duration * 1000, 2)
                    )

                return memory_index

            except Exception as e:
                duration = time.time() - start_time
                self.logger.error(
                    "session_memory_creation_error",
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
        with LoggingContext(session_id=session_id, target_memory_index=memory_index):
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
                        active_memory_index=memory_index,
                        success=True,
                        duration_ms=round(duration * 1000, 2)
                    )
                else:
                    self.logger.warning(
                        "session_memory_switch_failed",
                        success=False,
                        reason="memory_index_not_found",
                        duration_ms=round(duration * 1000, 2)
                    )

                return success

            except Exception as e:
                duration = time.time() - start_time
                self.logger.error(
                    "session_memory_switch_error",
                    error=str(e),
                    duration_ms=round(duration * 1000, 2)
                )
                raise
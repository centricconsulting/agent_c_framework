from typing import Dict, List, Any, Optional, Union

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
    
    async def create_session(self, session: ChatSession) -> ChatSession:
        """
        Create a new chat session.
        
        Args:
            session (ChatSession): The session to create
            
        Returns:
            ChatSession: The created session with updated fields
        """
        return await self.session_repository.add_session(session)
    
    async def get_session(self, session_id: str) -> Optional[ChatSession]:
        """
        Get a chat session by ID.
        
        Args:
            session_id (str): The session ID
            
        Returns:
            Optional[ChatSession]: The session if found, None otherwise
        """
        return await self.session_repository.get_session(session_id)
    
    async def update_session(self, session: ChatSession) -> ChatSession:
        """
        Update an existing chat session.
        
        Args:
            session (ChatSession): The session to update
            
        Returns:
            ChatSession: The updated session
        """
        return await self.session_repository.add_session(session)
    
    async def delete_session(self, session_id: str) -> bool:
        """
        Delete a chat session.
        
        Args:
            session_id (str): The session ID
            
        Returns:
            bool: True if deleted, False otherwise
        """
        return await self.session_repository.delete_session(session_id)
    
    async def get_user_sessions(self, user_id: str) -> List[str]:
        """
        Get all session IDs for a user.
        
        Args:
            user_id (str): The user ID
            
        Returns:
            List[str]: List of session IDs
        """
        return await self.session_repository.get_user_sessions(user_id)
    
    async def get_all_sessions(self) -> List[str]:
        """
        Get all session IDs.
        
        Returns:
            List[str]: List of all session IDs
        """
        return await self.session_repository.get_all_sessions()
    
    async def add_message(self, session_id: str, message: Dict[str, Any]) -> bool:
        """
        Add a message to a session's active memory.
        
        Args:
            session_id (str): The session ID
            message (Dict[str, Any]): The message to add
            
        Returns:
            bool: True if added, False otherwise
        """
        return await self.session_repository.add_message_to_session(session_id, message)
    
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
        return await self.session_repository.create_new_memory(session_id, name, description)
    
    async def switch_active_memory(self, session_id: str, memory_index: int) -> bool:
        """
        Switch the active memory of a session.
        
        Args:
            session_id (str): The session ID
            memory_index (int): Index of the memory to make active
            
        Returns:
            bool: True if switched, False if memory doesn't exist
        """
        return await self.session_repository.switch_active_memory(session_id, memory_index)
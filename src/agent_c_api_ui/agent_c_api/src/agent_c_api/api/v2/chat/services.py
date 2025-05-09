from typing import Dict, List, Any, Optional, Union, Sequence
from fastapi_cache.decorator import cache

from agent_c.models.events.chat import MessageEvent, InteractionEvent
from agent_c.models.events.tool_calls import ToolCallEvent
from agent_c_api.config.redis_config import RedisConfig
from agent_c_api.core.repositories.chat_repository import ChatRepository
from agent_c_api.core.services.chat_service import ChatService as CoreChatService

class ChatService:
    """Service for managing chat operations"""
    
    async def _get_core_service(self, session_id: str) -> CoreChatService:
        """Get the core chat service with dependencies
        
        Args:
            session_id: The session ID
            
        Returns:
            Initialized core chat service
        """
        redis_client = await RedisConfig.get_redis_client()
        chat_repository = ChatRepository(redis_client, session_id)
        return CoreChatService(chat_repository)
    
    async def add_message(self, session_id: str, message: Union[MessageEvent, Dict[str, Any]]) -> None:
        """Add a message to a chat session
        
        Args:
            session_id: The session ID
            message: The message to add
        """
        core_service = await self._get_core_service(session_id)
        await core_service.add_message(message)
    
    async def get_messages(self, session_id: str, start: str = "-", end: str = "+", count: int = 100) -> List[Dict[str, Any]]:
        """Get messages from a chat session
        
        Args:
            session_id: The session ID
            start: Start ID for range query (default: "-" = oldest)
            end: End ID for range query (default: "+" = newest)
            count: Maximum number of messages to retrieve
            
        Returns:
            List of messages
        """
        core_service = await self._get_core_service(session_id)
        return await core_service.get_messages(start, end, count)
    
    async def get_session_meta(self, session_id: str) -> Dict[str, Any]:
        """Get session metadata
        
        Args:
            session_id: The session ID
            
        Returns:
            Session metadata
        """
        core_service = await self._get_core_service(session_id)
        return await core_service.get_session_meta()
    
    async def set_session_meta(self, session_id: str, key: str, value: Any) -> None:
        """Set session metadata
        
        Args:
            session_id: The session ID
            key: Metadata key
            value: Metadata value
        """
        core_service = await self._get_core_service(session_id)
        await core_service.set_session_meta(key, value)
    
    async def get_managed_meta(self, session_id: str) -> Dict[str, Any]:
        """Get managed session metadata
        
        Args:
            session_id: The session ID
            
        Returns:
            Managed session metadata
        """
        core_service = await self._get_core_service(session_id)
        return await core_service.get_managed_meta()
    
    async def set_managed_meta(self, session_id: str, key: str, value: Any) -> None:
        """Set managed session metadata
        
        Args:
            session_id: The session ID
            key: Metadata key
            value: Metadata value
        """
        core_service = await self._get_core_service(session_id)
        await core_service.set_managed_meta(key, value)
    
    async def add_tool_call(self, session_id: str, tool_call: Union[ToolCallEvent, Dict[str, Any]]) -> None:
        """Add a tool call to a chat session
        
        Args:
            session_id: The session ID
            tool_call: The tool call to add
        """
        core_service = await self._get_core_service(session_id)
        await core_service.add_tool_call(tool_call)
    
    async def get_tool_calls(self, session_id: str, start: str = "-", end: str = "+", count: int = 100) -> List[Dict[str, Any]]:
        """Get tool calls from a chat session
        
        Args:
            session_id: The session ID
            start: Start ID for range query (default: "-" = oldest)
            end: End ID for range query (default: "+" = newest)
            count: Maximum number of tool calls to retrieve
            
        Returns:
            List of tool calls
        """
        core_service = await self._get_core_service(session_id)
        return await core_service.get_tool_calls(start, end, count)
    
    async def add_interaction(self, session_id: str, 
                            messages: Sequence[Union[MessageEvent, Dict[str, Any]]], 
                            tool_calls: Optional[Sequence[Union[ToolCallEvent, Dict[str, Any]]]] = None,
                            interaction_id: Optional[str] = None) -> str:
        """Add multiple messages as a single interaction to a chat session
        
        Args:
            session_id: The session ID
            messages: Messages to add
            tool_calls: Tool calls to add (optional)
            interaction_id: Custom interaction ID (optional)
            
        Returns:
            The interaction ID
        """
        core_service = await self._get_core_service(session_id)
        return await core_service.add_interaction(messages, tool_calls, interaction_id)
    
    async def get_interactions(self, session_id: str) -> List[str]:
        """Get all interaction IDs for a chat session
        
        Args:
            session_id: The session ID
            
        Returns:
            List of interaction IDs
        """
        core_service = await self._get_core_service(session_id)
        return await core_service.get_interactions()
    
    async def get_interaction(self, session_id: str, interaction_id: str) -> Dict[str, Any]:
        """Get details of a specific interaction
        
        Args:
            session_id: The session ID
            interaction_id: The interaction ID
            
        Returns:
            Interaction details including messages and tool calls
        """
        core_service = await self._get_core_service(session_id)
        return await core_service.get_interaction(interaction_id)
from typing import Dict, List, Any, Optional, Union, Sequence

from agent_c.models.events.chat import MessageEvent, InteractionEvent
from agent_c.models.events.tool_calls import ToolCallEvent
from ..repositories.chat_repository import ChatRepository

class ChatService:
    """
    Service for managing chat messages and session data.
    """
    
    def __init__(self, chat_repository: ChatRepository):
        """
        Initialize the chat service.
        
        Args:
            chat_repository (ChatRepository): Chat repository instance
        """
        self.chat_repository = chat_repository
    
    async def add_message(self, message: Union[MessageEvent, Dict[str, Any]]) -> None:
        """
        Add a message to the chat session.
        
        Args:
            message (Union[MessageEvent, Dict[str, Any]]): The message to add
        """
        await self.chat_repository.add_message(message)
    
    async def get_messages(self, start: str = "-", end: str = "+", count: int = 100) -> List[Dict[str, Any]]:
        """
        Get messages from the chat session.
        
        Args:
            start (str): Start ID for range query
            end (str): End ID for range query
            count (int): Maximum number of messages to retrieve
            
        Returns:
            List[Dict[str, Any]]: List of messages
        """
        return await self.chat_repository.get_messages(start, end, count)
    
    async def get_session_meta(self) -> Dict[str, Any]:
        """
        Get session metadata.
        
        Returns:
            Dict[str, Any]: Session metadata
        """
        return await self.chat_repository.get_meta()
    
    async def set_session_meta(self, key: str, value: Any) -> None:
        """
        Set session metadata.
        
        Args:
            key (str): Metadata key
            value (Any): Metadata value
        """
        await self.chat_repository.set_meta(key, value)
    
    async def get_managed_meta(self) -> Dict[str, Any]:
        """
        Get managed session metadata.
        
        Returns:
            Dict[str, Any]: Managed session metadata
        """
        return await self.chat_repository.get_managed_meta()
    
    async def set_managed_meta(self, key: str, value: Any) -> None:
        """
        Set managed session metadata.
        
        Args:
            key (str): Metadata key
            value (Any): Metadata value
        """
        await self.chat_repository.set_managed_meta(key, value)
    
    async def add_tool_call(self, tool_call: Union[ToolCallEvent, Dict[str, Any]]) -> None:
        """
        Add a tool call to the chat session.
        
        Args:
            tool_call (Union[ToolCallEvent, Dict[str, Any]]): The tool call to add
        """
        await self.chat_repository.add_tool_call(tool_call)
    
    async def get_tool_calls(self, start: str = "-", end: str = "+", count: int = 100) -> List[Dict[str, Any]]:
        """
        Get tool calls from the chat session.
        
        Args:
            start (str): Start ID for range query
            end (str): End ID for range query
            count (int): Maximum number of tool calls to retrieve
            
        Returns:
            List[Dict[str, Any]]: List of tool calls
        """
        return await self.chat_repository.get_tool_calls(start, end, count)
    
    async def add_interaction(self, messages: Sequence[Union[MessageEvent, Dict[str, Any]]], 
                            tool_calls: Optional[Sequence[Union[ToolCallEvent, Dict[str, Any]]]] = None,
                            interaction_id: Optional[str] = None) -> str:
        """
        Add multiple messages as a single interaction to the chat session.
        
        Args:
            messages (Sequence[Union[MessageEvent, Dict[str, Any]]]): Messages to add
            tool_calls (Optional[Sequence[Union[ToolCallEvent, Dict[str, Any]]]]): Tool calls to add
            interaction_id (Optional[str]): Custom interaction ID
            
        Returns:
            str: The interaction ID
        """
        return await self.chat_repository.add_interaction(messages, tool_calls, interaction_id)
    
    async def get_interactions(self) -> List[str]:
        """
        Get all interaction IDs for the chat session.
        
        Returns:
            List[str]: List of interaction IDs
        """
        return await self.chat_repository.get_interactions()
    
    async def get_interaction(self, interaction_id: str) -> Dict[str, Any]:
        """
        Get details of a specific interaction.
        
        Args:
            interaction_id (str): The interaction ID
            
        Returns:
            Dict[str, Any]: Interaction details including messages and tool calls
        """
        return await self.chat_repository.get_interaction(interaction_id)
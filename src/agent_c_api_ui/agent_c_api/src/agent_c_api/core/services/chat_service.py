import time
from typing import Dict, List, Any, Optional, Union, Sequence

import structlog
from agent_c.models.events.chat import MessageEvent, InteractionEvent
from agent_c.models.events.tool_calls import ToolCallEvent
from agent_c.models.common_chat.models import CommonChatMessage
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
        self.logger = structlog.get_logger(__name__)
        self.session_id = chat_repository.session_id
        
        self.logger.info(
            "chat_service_initialized",
            session_id=self.session_id,
            repository_type=type(chat_repository).__name__
        )
    
    async def add_message(self, message: Union[MessageEvent, CommonChatMessage, Dict[str, Any]]) -> None:
        """
        Add a message to the chat session.
        
        Args:
            message (Union[MessageEvent, CommonChatMessage, Dict[str, Any]]): The message to add
        """
        start_time = time.time()
        
        try:
            # Log message context
            message_type = type(message).__name__
            message_id = getattr(message, 'id', None) if hasattr(message, 'id') else message.get('id', 'unknown') if isinstance(message, dict) else 'unknown'
            
            self.logger.info(
                "chat_message_adding",
                session_id=self.session_id,
                message_id=message_id,
                message_type=message_type
            )
            
            await self.chat_repository.add_message(message)
            
            duration = time.time() - start_time
            self.logger.info(
                "chat_message_added",
                session_id=self.session_id,
                message_id=message_id,
                message_type=message_type,
                duration_ms=round(duration * 1000, 2)
            )
            
        except Exception as e:
            duration = time.time() - start_time
            self.logger.error(
                "chat_message_add_failed",
                session_id=self.session_id,
                message_id=message_id if 'message_id' in locals() else 'unknown',
                message_type=message_type if 'message_type' in locals() else 'unknown',
                error=str(e),
                duration_ms=round(duration * 1000, 2)
            )
            raise
        
    async def add_common_chat_message(self, message: CommonChatMessage) -> None:
        """
        Add a CommonChatMessage to the chat session.
        
        This is a convenience method that explicitly accepts CommonChatMessage.
        
        Args:
            message (CommonChatMessage): The message to add
        """
        start_time = time.time()
        
        try:
            self.logger.info(
                "common_chat_message_adding",
                session_id=self.session_id,
                message_id=message.id,
                role=message.role.value if hasattr(message.role, 'value') else str(message.role)
            )
            
            await self.chat_repository.add_message(message)
            
            duration = time.time() - start_time
            self.logger.info(
                "common_chat_message_added",
                session_id=self.session_id,
                message_id=message.id,
                role=message.role.value if hasattr(message.role, 'value') else str(message.role),
                duration_ms=round(duration * 1000, 2)
            )
            
        except Exception as e:
            duration = time.time() - start_time
            self.logger.error(
                "common_chat_message_add_failed",
                session_id=self.session_id,
                message_id=message.id,
                error=str(e),
                duration_ms=round(duration * 1000, 2)
            )
            raise
    
    async def get_messages(self, start: str = "-", end: str = "+", count: int = 100, 
                      format: str = "default") -> List[Union[Dict[str, Any], CommonChatMessage]]:
        """
        Get messages from the chat session.
        
        Args:
            start (str): Start ID for range query
            end (str): End ID for range query
            count (int): Maximum number of messages to retrieve
            format (str): Message format to return: "default" for original format or "common" for CommonChatMessage
            
        Returns:
            List[Union[Dict[str, Any], CommonChatMessage]]: List of messages
        """
        start_time = time.time()
        
        try:
            self.logger.info(
                "chat_messages_retrieving",
                session_id=self.session_id,
                start=start,
                end=end,
                count=count,
                format=format
            )
            
            messages = await self.chat_repository.get_messages(start, end, count, format)
            
            duration = time.time() - start_time
            self.logger.info(
                "chat_messages_retrieved",
                session_id=self.session_id,
                start=start,
                end=end,
                count=count,
                format=format,
                retrieved_count=len(messages),
                duration_ms=round(duration * 1000, 2)
            )
            
            return messages
            
        except Exception as e:
            duration = time.time() - start_time
            self.logger.error(
                "chat_messages_retrieval_failed",
                session_id=self.session_id,
                start=start,
                end=end,
                count=count,
                format=format,
                error=str(e),
                duration_ms=round(duration * 1000, 2)
            )
            raise
        
    async def get_common_chat_messages(self, start: str = "-", end: str = "+", count: int = 100) -> List[CommonChatMessage]:
        """
        Get messages as CommonChatMessage objects from the chat session.
        
        This is a convenience method that explicitly returns CommonChatMessage objects.
        
        Args:
            start (str): Start ID for range query
            end (str): End ID for range query
            count (int): Maximum number of messages to retrieve
            
        Returns:
            List[CommonChatMessage]: List of messages in CommonChatMessage format
        """
        start_time = time.time()
        
        try:
            self.logger.info(
                "common_chat_messages_retrieving",
                session_id=self.session_id,
                start=start,
                end=end,
                count=count
            )
            
            messages = await self.chat_repository.get_messages(start, end, count, format="common")
            
            duration = time.time() - start_time
            self.logger.info(
                "common_chat_messages_retrieved",
                session_id=self.session_id,
                start=start,
                end=end,
                count=count,
                retrieved_count=len(messages),
                duration_ms=round(duration * 1000, 2)
            )
            
            return messages
            
        except Exception as e:
            duration = time.time() - start_time
            self.logger.error(
                "common_chat_messages_retrieval_failed",
                session_id=self.session_id,
                start=start,
                end=end,
                count=count,
                error=str(e),
                duration_ms=round(duration * 1000, 2)
            )
            raise
    
    async def get_session_meta(self) -> Dict[str, Any]:
        """
        Get session metadata.
        
        Returns:
            Dict[str, Any]: Session metadata
        """
        start_time = time.time()
        
        try:
            self.logger.info(
                "session_meta_retrieving",
                session_id=self.session_id
            )
            
            meta = await self.chat_repository.get_meta()
            
            duration = time.time() - start_time
            self.logger.info(
                "session_meta_retrieved",
                session_id=self.session_id,
                meta_keys_count=len(meta) if meta else 0,
                duration_ms=round(duration * 1000, 2)
            )
            
            return meta
            
        except Exception as e:
            duration = time.time() - start_time
            self.logger.error(
                "session_meta_retrieval_failed",
                session_id=self.session_id,
                error=str(e),
                duration_ms=round(duration * 1000, 2)
            )
            raise
    
    async def set_session_meta(self, key: str, value: Any) -> None:
        """
        Set session metadata.
        
        Args:
            key (str): Metadata key
            value (Any): Metadata value
        """
        start_time = time.time()
        
        try:
            self.logger.info(
                "session_meta_setting",
                session_id=self.session_id,
                key=key,
                value_type=type(value).__name__
            )
            
            await self.chat_repository.set_meta(key, value)
            
            duration = time.time() - start_time
            self.logger.info(
                "session_meta_set",
                session_id=self.session_id,
                key=key,
                value_type=type(value).__name__,
                duration_ms=round(duration * 1000, 2)
            )
            
        except Exception as e:
            duration = time.time() - start_time
            self.logger.error(
                "session_meta_set_failed",
                session_id=self.session_id,
                key=key,
                value_type=type(value).__name__,
                error=str(e),
                duration_ms=round(duration * 1000, 2)
            )
            raise
    
    async def get_managed_meta(self) -> Dict[str, Any]:
        """
        Get managed session metadata.
        
        Returns:
            Dict[str, Any]: Managed session metadata
        """
        start_time = time.time()
        
        try:
            self.logger.info(
                "managed_meta_retrieving",
                session_id=self.session_id
            )
            
            meta = await self.chat_repository.get_managed_meta()
            
            duration = time.time() - start_time
            self.logger.info(
                "managed_meta_retrieved",
                session_id=self.session_id,
                meta_keys_count=len(meta) if meta else 0,
                duration_ms=round(duration * 1000, 2)
            )
            
            return meta
            
        except Exception as e:
            duration = time.time() - start_time
            self.logger.error(
                "managed_meta_retrieval_failed",
                session_id=self.session_id,
                error=str(e),
                duration_ms=round(duration * 1000, 2)
            )
            raise
    
    async def set_managed_meta(self, key: str, value: Any) -> None:
        """
        Set managed session metadata.
        
        Args:
            key (str): Metadata key
            value (Any): Metadata value
        """
        start_time = time.time()
        
        try:
            self.logger.info(
                "managed_meta_setting",
                session_id=self.session_id,
                key=key,
                value_type=type(value).__name__
            )
            
            await self.chat_repository.set_managed_meta(key, value)
            
            duration = time.time() - start_time
            self.logger.info(
                "managed_meta_set",
                session_id=self.session_id,
                key=key,
                value_type=type(value).__name__,
                duration_ms=round(duration * 1000, 2)
            )
            
        except Exception as e:
            duration = time.time() - start_time
            self.logger.error(
                "managed_meta_set_failed",
                session_id=self.session_id,
                key=key,
                value_type=type(value).__name__,
                error=str(e),
                duration_ms=round(duration * 1000, 2)
            )
            raise
    
    async def add_tool_call(self, tool_call: Union[ToolCallEvent, CommonChatMessage, Dict[str, Any]]) -> None:
        """
        Add a tool call to the chat session.
        
        Args:
            tool_call (Union[ToolCallEvent, CommonChatMessage, Dict[str, Any]]): The tool call to add
        """
        start_time = time.time()
        
        try:
            # Log tool call context
            tool_call_type = type(tool_call).__name__
            tool_call_id = getattr(tool_call, 'id', None) if hasattr(tool_call, 'id') else tool_call.get('id', 'unknown') if isinstance(tool_call, dict) else 'unknown'
            
            self.logger.info(
                "tool_call_adding",
                session_id=self.session_id,
                tool_call_id=tool_call_id,
                tool_call_type=tool_call_type
            )
            
            await self.chat_repository.add_tool_call(tool_call)
            
            duration = time.time() - start_time
            self.logger.info(
                "tool_call_added",
                session_id=self.session_id,
                tool_call_id=tool_call_id,
                tool_call_type=tool_call_type,
                duration_ms=round(duration * 1000, 2)
            )
            
        except Exception as e:
            duration = time.time() - start_time
            self.logger.error(
                "tool_call_add_failed",
                session_id=self.session_id,
                tool_call_id=tool_call_id if 'tool_call_id' in locals() else 'unknown',
                tool_call_type=tool_call_type if 'tool_call_type' in locals() else 'unknown',
                error=str(e),
                duration_ms=round(duration * 1000, 2)
            )
            raise
        
    async def add_common_chat_tool_call(self, tool_call: CommonChatMessage) -> None:
        """
        Add a CommonChatMessage as a tool call to the chat session.
        
        This is a convenience method that explicitly accepts CommonChatMessage for tool calls.
        
        Args:
            tool_call (CommonChatMessage): The tool call message to add
        """
        start_time = time.time()
        
        try:
            self.logger.info(
                "common_chat_tool_call_adding",
                session_id=self.session_id,
                tool_call_id=tool_call.id,
                role=tool_call.role.value if hasattr(tool_call.role, 'value') else str(tool_call.role)
            )
            
            await self.chat_repository.add_tool_call(tool_call)
            
            duration = time.time() - start_time
            self.logger.info(
                "common_chat_tool_call_added",
                session_id=self.session_id,
                tool_call_id=tool_call.id,
                role=tool_call.role.value if hasattr(tool_call.role, 'value') else str(tool_call.role),
                duration_ms=round(duration * 1000, 2)
            )
            
        except Exception as e:
            duration = time.time() - start_time
            self.logger.error(
                "common_chat_tool_call_add_failed",
                session_id=self.session_id,
                tool_call_id=tool_call.id,
                error=str(e),
                duration_ms=round(duration * 1000, 2)
            )
            raise
    
    async def get_tool_calls(self, start: str = "-", end: str = "+", count: int = 100, 
                       format: str = "default") -> List[Union[Dict[str, Any], CommonChatMessage]]:
        """
        Get tool calls from the chat session.
        
        Args:
            start (str): Start ID for range query
            end (str): End ID for range query
            count (int): Maximum number of tool calls to retrieve
            format (str): Format to return: "default" for original format or "common" for CommonChatMessage
            
        Returns:
            List[Union[Dict[str, Any], CommonChatMessage]]: List of tool calls
        """
        start_time = time.time()
        
        try:
            self.logger.info(
                "tool_calls_retrieving",
                session_id=self.session_id,
                start=start,
                end=end,
                count=count,
                format=format
            )
            
            tool_calls = await self.chat_repository.get_tool_calls(start, end, count, format)
            
            duration = time.time() - start_time
            self.logger.info(
                "tool_calls_retrieved",
                session_id=self.session_id,
                start=start,
                end=end,
                count=count,
                format=format,
                retrieved_count=len(tool_calls),
                duration_ms=round(duration * 1000, 2)
            )
            
            return tool_calls
            
        except Exception as e:
            duration = time.time() - start_time
            self.logger.error(
                "tool_calls_retrieval_failed",
                session_id=self.session_id,
                start=start,
                end=end,
                count=count,
                format=format,
                error=str(e),
                duration_ms=round(duration * 1000, 2)
            )
            raise
        
    async def get_common_chat_tool_calls(self, start: str = "-", end: str = "+", count: int = 100) -> List[CommonChatMessage]:
        """
        Get tool calls as CommonChatMessage objects from the chat session.
        
        This is a convenience method that explicitly returns CommonChatMessage objects.
        
        Args:
            start (str): Start ID for range query
            end (str): End ID for range query
            count (int): Maximum number of tool calls to retrieve
            
        Returns:
            List[CommonChatMessage]: List of tool calls in CommonChatMessage format
        """
        start_time = time.time()
        
        try:
            self.logger.info(
                "common_chat_tool_calls_retrieving",
                session_id=self.session_id,
                start=start,
                end=end,
                count=count
            )
            
            tool_calls = await self.chat_repository.get_tool_calls(start, end, count, format="common")
            
            duration = time.time() - start_time
            self.logger.info(
                "common_chat_tool_calls_retrieved",
                session_id=self.session_id,
                start=start,
                end=end,
                count=count,
                retrieved_count=len(tool_calls),
                duration_ms=round(duration * 1000, 2)
            )
            
            return tool_calls
            
        except Exception as e:
            duration = time.time() - start_time
            self.logger.error(
                "common_chat_tool_calls_retrieval_failed",
                session_id=self.session_id,
                start=start,
                end=end,
                count=count,
                error=str(e),
                duration_ms=round(duration * 1000, 2)
            )
            raise
    
    async def add_interaction(self, messages: Sequence[Union[MessageEvent, CommonChatMessage, Dict[str, Any]]], 
                            tool_calls: Optional[Sequence[Union[ToolCallEvent, CommonChatMessage, Dict[str, Any]]]] = None,
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
        start_time = time.time()
        
        try:
            self.logger.info(
                "interaction_adding",
                session_id=self.session_id,
                interaction_id=interaction_id,
                messages_count=len(messages),
                tool_calls_count=len(tool_calls) if tool_calls else 0
            )
            
            result_interaction_id = await self.chat_repository.add_interaction(messages, tool_calls, interaction_id)
            
            duration = time.time() - start_time
            self.logger.info(
                "interaction_added",
                session_id=self.session_id,
                interaction_id=result_interaction_id,
                messages_count=len(messages),
                tool_calls_count=len(tool_calls) if tool_calls else 0,
                duration_ms=round(duration * 1000, 2)
            )
            
            return result_interaction_id
            
        except Exception as e:
            duration = time.time() - start_time
            self.logger.error(
                "interaction_add_failed",
                session_id=self.session_id,
                interaction_id=interaction_id,
                messages_count=len(messages),
                tool_calls_count=len(tool_calls) if tool_calls else 0,
                error=str(e),
                duration_ms=round(duration * 1000, 2)
            )
            raise
    
    async def get_interactions(self) -> List[str]:
        """
        Get all interaction IDs for the chat session.
        
        Returns:
            List[str]: List of interaction IDs
        """
        start_time = time.time()
        
        try:
            self.logger.info(
                "interactions_retrieving",
                session_id=self.session_id
            )
            
            interactions = await self.chat_repository.get_interactions()
            
            duration = time.time() - start_time
            self.logger.info(
                "interactions_retrieved",
                session_id=self.session_id,
                interactions_count=len(interactions),
                duration_ms=round(duration * 1000, 2)
            )
            
            return interactions
            
        except Exception as e:
            duration = time.time() - start_time
            self.logger.error(
                "interactions_retrieval_failed",
                session_id=self.session_id,
                error=str(e),
                duration_ms=round(duration * 1000, 2)
            )
            raise
    
    async def get_interaction(self, interaction_id: str, format: str = "default") -> Dict[str, Any]:
        """
        Get details of a specific interaction.
        
        Args:
            interaction_id (str): The interaction ID
            format (str): Format to return: "default" for original format or "common" for CommonChatMessage
            
        Returns:
            Dict[str, Any]: Interaction details including messages and tool calls
        """
        start_time = time.time()
        
        try:
            self.logger.info(
                "interaction_retrieving",
                session_id=self.session_id,
                interaction_id=interaction_id,
                format=format
            )
            
            interaction = await self.chat_repository.get_interaction(interaction_id, format)
            
            duration = time.time() - start_time
            self.logger.info(
                "interaction_retrieved",
                session_id=self.session_id,
                interaction_id=interaction_id,
                format=format,
                messages_count=len(interaction.get('messages', [])),
                tool_calls_count=len(interaction.get('tool_calls', [])),
                duration_ms=round(duration * 1000, 2)
            )
            
            return interaction
            
        except Exception as e:
            duration = time.time() - start_time
            self.logger.error(
                "interaction_retrieval_failed",
                session_id=self.session_id,
                interaction_id=interaction_id,
                format=format,
                error=str(e),
                duration_ms=round(duration * 1000, 2)
            )
            raise
        
    async def get_common_chat_interaction(self, interaction_id: str) -> Dict[str, Any]:
        """
        Get details of a specific interaction with messages/tool calls as CommonChatMessage objects.
        
        This is a convenience method that explicitly returns messages in CommonChatMessage format.
        
        Args:
            interaction_id (str): The interaction ID
            
        Returns:
            Dict[str, Any]: Interaction details with messages and tool calls as CommonChatMessage objects
        """
        start_time = time.time()
        
        try:
            self.logger.info(
                "common_chat_interaction_retrieving",
                session_id=self.session_id,
                interaction_id=interaction_id
            )
            
            interaction = await self.chat_repository.get_interaction(interaction_id, format="common")
            
            duration = time.time() - start_time
            self.logger.info(
                "common_chat_interaction_retrieved",
                session_id=self.session_id,
                interaction_id=interaction_id,
                messages_count=len(interaction.get('messages', [])),
                tool_calls_count=len(interaction.get('tool_calls', [])),
                duration_ms=round(duration * 1000, 2)
            )
            
            return interaction
            
        except Exception as e:
            duration = time.time() - start_time
            self.logger.error(
                "common_chat_interaction_retrieval_failed",
                session_id=self.session_id,
                interaction_id=interaction_id,
                error=str(e),
                duration_ms=round(duration * 1000, 2)
            )
            raise
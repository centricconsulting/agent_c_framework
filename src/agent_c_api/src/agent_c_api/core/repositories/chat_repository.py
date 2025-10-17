from datetime import datetime
import json
import time
from typing import Dict, List, Any, Optional, Union, Sequence, cast

import structlog
from agent_c.models.events.chat import MessageEvent, InteractionEvent
from agent_c.models.events.tool_calls import ToolCallEvent
from agent_c.models.common_chat.models import CommonChatMessage
from redis import asyncio as aioredis

from ..util.common_chat_converter import CommonChatConverter

class ChatRepository:
    """Repository for managing chat messages in Redis"""
    
    def __init__(self, redis_client: aioredis.Redis, session_id: str):
        """
        Initialize the chat repository.
        
        Args:
            redis_client (aioredis.Redis): Redis client instance
            session_id (str): The session ID
        """
        self.redis = redis_client
        self.session_id = session_id
        self.logger = structlog.get_logger(__name__)
        
        self.logger.info(
            "chat_repository_initialized",
            session_id=session_id,
            redis_client_type=type(redis_client).__name__
        )
    
    async def add_message(self, message: Union[MessageEvent, CommonChatMessage, Dict[str, Any]]) -> None:
        """
        Add a message to the chat session.
        
        Args:
            message (Union[MessageEvent, CommonChatMessage, Dict[str, Any]]): The message to add
        """
        start_time = time.time()
        
        try:
            # Extract message context for logging
            message_type = type(message).__name__
            message_id = getattr(message, 'id', None) if hasattr(message, 'id') else message.get('id', 'unknown') if isinstance(message, dict) else 'unknown'
            
            self.logger.info(
                "chat_message_storing",
                session_id=self.session_id,
                message_id=message_id,
                message_type=message_type
            )
            
            # Handle CommonChatMessage
            if isinstance(message, CommonChatMessage):
                # Store the CommonChatMessage as JSON
                common_msg_json = message.model_dump_json()
                msg_id = message.id
                await self.redis.set(
                    f"session:{self.session_id}:common_msg:{msg_id}", 
                    common_msg_json
                )
                
                self.logger.debug(
                    "common_chat_message_stored",
                    session_id=self.session_id,
                    message_id=msg_id,
                    role=message.role.value if hasattr(message.role, 'value') else str(message.role)
                )
                
                # Convert to MessageEvent for backward compatibility
                msg_event = CommonChatConverter.common_chat_to_message_event(message)
                if msg_event:
                    # Continue with MessageEvent processing
                    msg_data = msg_event.model_dump()
                else:
                    # This is a tool message, store minimal data
                    msg_data = {
                        "id": message.id,
                        "role": message.role.value,
                        "content": "[Tool Message]",  # Placeholder
                        "timestamp": message.created_at.isoformat(),
                        "is_common_chat": "true"
                    }
            # Handle MessageEvent
            elif isinstance(message, MessageEvent):
                msg_data = message.model_dump()
                
                # Also convert and store as CommonChatMessage for future use
                common_msg = CommonChatConverter.message_event_to_common_chat(message)
                common_msg_json = common_msg.model_dump_json()
                msg_id = common_msg.id
                await self.redis.set(
                    f"session:{self.session_id}:common_msg:{msg_id}", 
                    common_msg_json
                )
                
                self.logger.debug(
                    "message_event_converted_and_stored",
                    session_id=self.session_id,
                    message_id=msg_id,
                    original_type="MessageEvent"
                )
            # Handle dict
            else:
                msg_data = message
            
            # Add timestamp if not present
            if "timestamp" not in msg_data:
                msg_data["timestamp"] = datetime.now().isoformat()
            
            # Convert any non-string values
            msg_data = {k: json.dumps(v) if not isinstance(v, str) else v 
                        for k, v in msg_data.items()}
            
            # Add to Redis stream
            stream_result = await self.redis.xadd(f"session:{self.session_id}:messages", msg_data)
            
            # Update session updated_at time
            await self.redis.hset(f"session:{self.session_id}:meta", "updated_at", 
                                datetime.now().isoformat())
            
            duration = time.time() - start_time
            self.logger.info(
                "chat_message_stored",
                session_id=self.session_id,
                message_id=message_id,
                message_type=message_type,
                stream_id=stream_result.decode('utf-8') if isinstance(stream_result, bytes) else str(stream_result),
                duration_ms=round(duration * 1000, 2)
            )
            
        except Exception as e:
            duration = time.time() - start_time
            self.logger.error(
                "chat_message_store_failed",
                session_id=self.session_id,
                message_id=message_id if 'message_id' in locals() else 'unknown',
                message_type=message_type if 'message_type' in locals() else 'unknown',
                error=str(e),
                duration_ms=round(duration * 1000, 2)
            )
            raise
    
    async def get_messages(self, start: str = "-", end: str = "+", count: int = 100, 
                      msg_format: str = "default") -> List[Union[Dict[str, Any], CommonChatMessage]]:
        """
        Get messages from the chat session.
        
        Args:
            start (str): Start ID for range query
            end (str): End ID for range query
            count (int): Maximum number of messages to retrieve
            msg_format (str): Message format to return: "default" for original format or "common" for CommonChatMessage
            
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
            
            # Get messages from Redis stream
            messages = await self.redis.xrange(f"session:{self.session_id}:messages", start, end, count)
            
            # Process messages
            result = []
            for msg_id, msg_data in messages:
                # Convert message ID to string
                msg_id_str = msg_id.decode("utf-8") if isinstance(msg_id, bytes) else msg_id
                
                # Process message data
                processed_data = {}
                for k, v in msg_data.items():
                    # Convert bytes to string
                    key = k.decode("utf-8") if isinstance(k, bytes) else k
                    value = v.decode("utf-8") if isinstance(v, bytes) else v
                    
                    # Try to parse JSON values
                    try:
                        if key not in ["timestamp", "role", "content", "format"]:
                            processed_value = json.loads(value)
                        else:
                            processed_value = value
                    except json.JSONDecodeError:
                        processed_value = value
                        
                    processed_data[key] = processed_value
                
                # Add message ID
                processed_data["id"] = msg_id_str
                
                # Check if we should return CommonChatMessage format
                if format == "common":
                    # Check if this is a common chat message
                    is_common_chat = processed_data.get("is_common_chat") == "true"
                    
                    # Try to get the stored CommonChatMessage
                    common_msg_json = await self.redis.get(f"session:{self.session_id}:common_msg:{processed_data['id']}")
                    
                    if common_msg_json:
                        # Deserialize from JSON
                        common_msg = CommonChatMessage.model_validate_json(
                            common_msg_json.decode("utf-8") if isinstance(common_msg_json, bytes) else common_msg_json
                        )
                        result.append(common_msg)
                    else:
                        # Convert on the fly if not stored
                        msg_event = MessageEvent(**processed_data)
                        common_msg = CommonChatConverter.message_event_to_common_chat(msg_event)
                        result.append(common_msg)
                else:
                    # Return original format
                    result.append(processed_data)
            
            duration = time.time() - start_time
            self.logger.info(
                "chat_messages_retrieved",
                session_id=self.session_id,
                start=start,
                end=end,
                count=count,
                format=msg_format,
                retrieved_count=len(result),
                duration_ms=round(duration * 1000, 2)
            )
            
            return result
            
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
    
    async def get_meta(self) -> Dict[str, Any]:
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
            
            # Get metadata from Redis hash
            meta_data = await self.redis.hgetall(f"session:{self.session_id}:meta")
            
            # Process metadata
            result = {}
            for k, v in meta_data.items():
                # Convert bytes to string
                key = k.decode("utf-8") if isinstance(k, bytes) else k
                value = v.decode("utf-8") if isinstance(v, bytes) else v
                
                # Try to parse JSON values
                try:
                    if key not in ["created_at", "updated_at", "name", "description"]:
                        processed_value = json.loads(value)
                    else:
                        processed_value = value
                except json.JSONDecodeError:
                    processed_value = value
                    
                result[key] = processed_value
            
            duration = time.time() - start_time
            self.logger.info(
                "session_meta_retrieved",
                session_id=self.session_id,
                meta_keys_count=len(result),
                duration_ms=round(duration * 1000, 2)
            )
            
            return result
            
        except Exception as e:
            duration = time.time() - start_time
            self.logger.error(
                "session_meta_retrieval_failed",
                session_id=self.session_id,
                error=str(e),
                duration_ms=round(duration * 1000, 2)
            )
            raise
    
    async def set_meta(self, key: str, value: Any) -> None:
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
            
            # Convert value to string if needed
            if not isinstance(value, str):
                value = json.dumps(value)
                
            # Set metadata in Redis hash
            await self.redis.hset(f"session:{self.session_id}:meta", key, value)
            
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
        # Get metadata from Redis hash
        meta_data = await self.redis.hgetall(f"session:{self.session_id}:managed_meta")
        
        # Process metadata (same as get_meta)
        result = {}
        for k, v in meta_data.items():
            # Convert bytes to string
            key = k.decode("utf-8") if isinstance(k, bytes) else k
            value = v.decode("utf-8") if isinstance(v, bytes) else v
            
            # Try to parse JSON values
            try:
                processed_value = json.loads(value)
            except json.JSONDecodeError:
                processed_value = value
                
            result[key] = processed_value
        
        return result
    
    async def set_managed_meta(self, key: str, value: Any) -> None:
        """
        Set managed session metadata.
        
        Args:
            key (str): Metadata key
            value (Any): Metadata value
        """
        # Convert value to string if needed
        if not isinstance(value, str):
            value = json.dumps(value)
            
        # Set metadata in Redis hash
        await self.redis.hset(f"session:{self.session_id}:managed_meta", key, value)
    
    async def add_tool_call(self, tool_call: Union[ToolCallEvent, CommonChatMessage, Dict[str, Any]]) -> None:
        """
        Add a tool call to the chat session.
        
        Args:
            tool_call (Union[ToolCallEvent, CommonChatMessage, Dict[str, Any]]): The tool call to add
        """
        # Handle CommonChatMessage
        if isinstance(tool_call, CommonChatMessage):
            # Store the CommonChatMessage as JSON
            common_msg_json = tool_call.model_dump_json()
            tool_id = tool_call.id
            await self.redis.set(
                f"session:{self.session_id}:common_msg:{tool_id}", 
                common_msg_json
            )
            
            # Convert to ToolCallEvent for backward compatibility
            tool_event = CommonChatConverter.common_chat_to_tool_call_event(tool_call)
            if tool_event:
                # Continue with ToolCallEvent processing
                tool_data = tool_event.model_dump()
                # Add flag to indicate this is a common chat message
                tool_data["is_common_chat"] = "true"
            else:
                # This is not a valid tool message, skip
                return
        # Handle ToolCallEvent
        elif isinstance(tool_call, ToolCallEvent):
            tool_data = tool_call.model_dump()
            
            # Also convert and store as CommonChatMessage for future use
            common_msg = CommonChatConverter.tool_call_event_to_common_chat(tool_call)
            common_msg_json = common_msg.model_dump_json()
            tool_id = common_msg.id
            await self.redis.set(
                f"session:{self.session_id}:common_msg:{tool_id}", 
                common_msg_json
            )
        # Handle dict
        else:
            tool_data = tool_call
        
        # Add timestamp if not present
        if "timestamp" not in tool_data:
            tool_data["timestamp"] = datetime.now().isoformat()
        
        # Convert any non-string values
        tool_data = {k: json.dumps(v) if not isinstance(v, str) else v 
                    for k, v in tool_data.items()}
        
        # Add to Redis stream
        await self.redis.xadd(f"session:{self.session_id}:tool_calls", tool_data)
        
        # Update session updated_at time
        await self.redis.hset(f"session:{self.session_id}:meta", "updated_at", 
                            datetime.now().isoformat())
    
    async def get_tool_calls(self, start: str = "-", end: str = "+", count: int = 100,
                       msg_format: str = "default") -> List[Union[Dict[str, Any], CommonChatMessage]]:
        """
        Get tool calls from the chat session.
        
        Args:
            start (str): Start ID for range query
            end (str): End ID for range query
            count (int): Maximum number of tool calls to retrieve
            msg_format (str): Format to return: "default" for original format or "common" for CommonChatMessage
            
        Returns:
            List[Union[Dict[str, Any], CommonChatMessage]]: List of tool calls
        """
        # Get tool calls from Redis stream
        tool_calls = await self.redis.xrange(f"session:{self.session_id}:tool_calls", start, end, count)
        
        # Process tool calls
        result = []
        for call_id, call_data in tool_calls:
            # Convert ID to string
            call_id_str = call_id.decode("utf-8") if isinstance(call_id, bytes) else call_id
            
            # Process tool call data
            processed_data = {}
            for k, v in call_data.items():
                # Convert bytes to string
                key = k.decode("utf-8") if isinstance(k, bytes) else k
                value = v.decode("utf-8") if isinstance(v, bytes) else v
                
                # Try to parse JSON values
                try:
                    if key not in ["timestamp", "name", "description"]:
                        processed_value = json.loads(value)
                    else:
                        processed_value = value
                except json.JSONDecodeError:
                    processed_value = value
                    
                processed_data[key] = processed_value
            
            # Add call ID
            processed_data["id"] = call_id_str
            
            # Check if we should return CommonChatMessage format
            if msg_format == "common":
                # Check if this is a common chat message
                is_common_chat = processed_data.get("is_common_chat") == "true"
                
                # Try to get the stored CommonChatMessage
                common_msg_json = await self.redis.get(f"session:{self.session_id}:common_msg:{processed_data['id']}")
                
                if common_msg_json:
                    # Deserialize from JSON
                    common_msg = CommonChatMessage.model_validate_json(
                        common_msg_json.decode("utf-8") if isinstance(common_msg_json, bytes) else common_msg_json
                    )
                    result.append(common_msg)
                else:
                    # Convert on the fly if not stored
                    tool_event = ToolCallEvent(**processed_data)
                    common_msg = CommonChatConverter.tool_call_event_to_common_chat(tool_event)
                    result.append(common_msg)
            else:
                # Return original format
                result.append(processed_data)
        
        return result
    
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
        # Generate interaction ID if not provided
        if interaction_id is None:
            from uuid import uuid4
            interaction_id = str(uuid4())
        
        # Get current timestamp
        timestamp = datetime.now().isoformat()
        
        # Create a Redis transaction pipeline
        pipe = self.redis.pipeline()
        
        # Add interaction metadata
        interaction_meta = {
            "timestamp": timestamp,
            "message_count": str(len(messages)),
            "tool_call_count": str(len(tool_calls) if tool_calls else 0)
        }
        pipe.hset(f"session:{self.session_id}:interaction:{interaction_id}", mapping=interaction_meta)
        
        # Add all messages with the interaction ID
        for i, message in enumerate(messages):
            # Handle different message types
            if isinstance(message, CommonChatMessage):
                # Store the CommonChatMessage
                common_msg_json = message.model_dump_json()
                msg_id = message.id
                await self.redis.set(
                    f"session:{self.session_id}:common_msg:{msg_id}", 
                    common_msg_json
                )
                
                # Convert to MessageEvent for backward compatibility
                msg_event = CommonChatConverter.common_chat_to_message_event(message)
                if msg_event:
                    msg_data = msg_event.model_dump()
                else:
                    # This is a tool message, store minimal data
                    msg_data = {
                        "id": message.id,
                        "role": message.role.value,
                        "content": "[Tool Message]",  # Placeholder
                        "timestamp": message.created_at.isoformat(),
                        "is_common_chat": "true"
                    }
            elif isinstance(message, MessageEvent):
                msg_data = message.model_dump()
                
                # Also convert and store as CommonChatMessage
                common_msg = CommonChatConverter.message_event_to_common_chat(message)
                common_msg_json = common_msg.model_dump_json()
                msg_id = common_msg.id
                await self.redis.set(
                    f"session:{self.session_id}:common_msg:{msg_id}", 
                    common_msg_json
                )
            else:
                msg_data = message.copy()  # Create a copy to avoid modifying the original
            
            # Add interaction ID and timestamp
            msg_data["interaction_id"] = interaction_id
            if "timestamp" not in msg_data:
                msg_data["timestamp"] = timestamp
            
            # Add order index
            msg_data["interaction_index"] = str(i)
            
            # Convert any non-string values
            msg_data = {k: json.dumps(v) if not isinstance(v, str) else v 
                        for k, v in msg_data.items()}
            
            # Add to Redis stream
            pipe.xadd(f"session:{self.session_id}:messages", msg_data)
        
        # Add tool calls if provided
        if tool_calls:
            for i, tool_call in enumerate(tool_calls):
                # Handle different tool call types
                if isinstance(tool_call, CommonChatMessage):
                    # Store the CommonChatMessage
                    common_msg_json = tool_call.model_dump_json()
                    tool_id = tool_call.id
                    await self.redis.set(
                        f"session:{self.session_id}:common_msg:{tool_id}", 
                        common_msg_json
                    )
                    
                    # Convert to ToolCallEvent for backward compatibility
                    tool_event = CommonChatConverter.common_chat_to_tool_call_event(tool_call)
                    if tool_event:
                        tool_data = tool_event.model_dump()
                        # Add flag to indicate this is a common chat message
                        tool_data["is_common_chat"] = "true"
                    else:
                        # Skip invalid tool calls
                        continue
                elif isinstance(tool_call, ToolCallEvent):
                    tool_data = tool_call.model_dump()
                    
                    # Also convert and store as CommonChatMessage
                    common_msg = CommonChatConverter.tool_call_event_to_common_chat(tool_call)
                    common_msg_json = common_msg.model_dump_json()
                    tool_id = common_msg.id
                    await self.redis.set(
                        f"session:{self.session_id}:common_msg:{tool_id}", 
                        common_msg_json
                    )
                else:
                    tool_data = tool_call.copy()  # Create a copy to avoid modifying the original
                
                # Add interaction ID and timestamp
                tool_data["interaction_id"] = interaction_id
                if "timestamp" not in tool_data:
                    tool_data["timestamp"] = timestamp
                
                # Add order index
                tool_data["interaction_index"] = str(i)
                
                # Convert any non-string values
                tool_data = {k: json.dumps(v) if not isinstance(v, str) else v 
                            for k, v in tool_data.items()}
                
                # Add to Redis stream
                pipe.xadd(f"session:{self.session_id}:tool_calls", tool_data)
        
        # Update session updated_at time
        pipe.hset(f"session:{self.session_id}:meta", "updated_at", timestamp)
        
        # Update interactions index
        pipe.sadd(f"session:{self.session_id}:interactions", interaction_id)
        
        # Execute pipeline
        await pipe.execute()
        
        return interaction_id
    
    async def get_interactions(self) -> List[str]:
        """
        Get all interaction IDs for the chat session.
        
        Returns:
            List[str]: List of interaction IDs
        """
        # Get interaction IDs from Redis set
        interactions = await self.redis.smembers(f"session:{self.session_id}:interactions")
        
        # Convert to strings
        return [interaction.decode("utf-8") if isinstance(interaction, bytes) else interaction
                for interaction in interactions]
    
    async def get_interaction(self, interaction_id: str, format: str = "default") -> Dict[str, Any]:
        """
        Get details of a specific interaction.
        
        Args:
            interaction_id (str): The interaction ID
            
        Returns:
            Dict[str, Any]: Interaction details including messages and tool calls
        """
        # Get interaction metadata
        meta = await self.redis.hgetall(f"session:{self.session_id}:interaction:{interaction_id}")
        
        # Process metadata
        meta_dict = {}
        for k, v in meta.items():
            key = k.decode("utf-8") if isinstance(k, bytes) else k
            value = v.decode("utf-8") if isinstance(v, bytes) else v
            meta_dict[key] = value
        
        # Get messages for this interaction
        all_messages = await self.get_messages(format=format)
        if format == "common":
            # For CommonChatMessage objects, filter differently
            interaction_messages = []
            for msg in all_messages:
                # Check interaction_id in provider_metadata.additional_data
                if hasattr(msg, 'provider_metadata') and msg.provider_metadata:
                    if msg.provider_metadata.additional_data.get("interaction_id") == interaction_id:
                        interaction_messages.append(msg)
        else:
            # For dict objects, filter normally
            interaction_messages = [msg for msg in all_messages 
                                  if msg.get("interaction_id") == interaction_id]
        
        # Get tool calls for this interaction
        all_tool_calls = await self.get_tool_calls(format=format)
        if format == "common":
            # For CommonChatMessage objects, filter differently
            interaction_tool_calls = []
            for call in all_tool_calls:
                # Check interaction_id in provider_metadata.additional_data
                if hasattr(call, 'provider_metadata') and call.provider_metadata:
                    if call.provider_metadata.additional_data.get("interaction_id") == interaction_id:
                        interaction_tool_calls.append(call)
        else:
            # For dict objects, filter normally
            interaction_tool_calls = [call for call in all_tool_calls 
                                    if call.get("interaction_id") == interaction_id]
        
        # Sort by interaction_index if available
        interaction_messages.sort(key=lambda x: int(x.get("interaction_index", 0)))
        interaction_tool_calls.sort(key=lambda x: int(x.get("interaction_index", 0)))
        
        # Build result
        result = {
            "id": interaction_id,
            "timestamp": meta_dict.get("timestamp"),
            "messages": interaction_messages,
            "tool_calls": interaction_tool_calls,
            "metadata": meta_dict
        }
        
        return result
import asyncio
import json
import logging
from typing import Optional, Dict, Any, AsyncGenerator, List, Callable, Awaitable

import redis.asyncio as redis

from agent_c.models.events import SessionEvent

class RedisStreamManager:
    """
    Manages Redis stream operations for event streaming between components.
    
    This class provides a unified interface for publishing events to and consuming
    events from Redis streams. It handles connection management, event serialization,
    and stream consumption with configurable batch size and polling interval.
    """
    _redis_client = None
    _initialized = False
    _logger = logging.getLogger(__name__)
    
    @classmethod
    async def initialize(cls, redis_url: str):
        """
        Initialize the Redis connection for the manager.
        
        Args:
            redis_url: Redis connection URL (e.g., "redis://localhost:6379/0")
        """
        if not cls._initialized:
            try:
                cls._redis_client = redis.from_url(redis_url)
                # Test connection
                await cls._redis_client.ping()
                cls._initialized = True
                cls._logger.info(f"Connected to Redis at {redis_url}")
            except Exception as e:
                cls._logger.error(f"Failed to connect to Redis: {e}")
                raise
    
    @classmethod
    async def shutdown(cls):
        """
        Close the Redis connection when shutting down.
        """
        if cls._redis_client is not None:
            await cls._redis_client.close()
            cls._initialized = False
            cls._logger.info("Redis connection closed")
    
    @classmethod
    async def publish_event(cls, stream_id: str, event: SessionEvent) -> str:
        """
        Publish an event to a Redis stream.
        
        Args:
            stream_id: Identifier of the stream to publish to
            event: The event to publish
            
        Returns:
            The message ID of the published event
        """
        if not cls._initialized:
            raise RuntimeError("Redis stream manager not initialized")
        
        try:
            # Serialize the event for storage in Redis
            event_data = cls._serialize_event(event)
            
            # Publish to Redis stream
            message_id = await cls._redis_client.xadd(
                stream_id,
                {"event_type": event.type, "event_data": json.dumps(event_data)}
            )
            
            return message_id
        except Exception as e:
            cls._logger.error(f"Failed to publish event to stream {stream_id}: {e}")
            raise
    
    @classmethod
    async def consume_events(cls, 
                           stream_id: str, 
                           consumer_group: str = "agent_bridge",
                           consumer_name: str = "default",
                           batch_size: int = 10,
                           poll_interval: float = 0.1) -> AsyncGenerator[SessionEvent, None]:
        """
        Consume events from a Redis stream.
        
        Args:
            stream_id: Identifier of the stream to consume from
            consumer_group: Name of the consumer group 
            consumer_name: Name of this consumer within the group
            batch_size: Maximum number of events to read at once
            poll_interval: Time in seconds to wait between polling attempts
            
        Yields:
            Reconstructed SessionEvent objects
        """
        if not cls._initialized:
            raise RuntimeError("Redis stream manager not initialized")
        
        # Create consumer group if it doesn't exist
        try:
            await cls._redis_client.xgroup_create(stream_id, consumer_group, "0", mkstream=True)
            cls._logger.info(f"Created consumer group {consumer_group} for stream {stream_id}")
        except redis.exceptions.ResponseError as e:
            # Group already exists, which is fine
            if "BUSYGROUP" not in str(e):
                cls._logger.error(f"Error creating consumer group: {e}")
                raise
        
        last_id = '0'  # Start from beginning if no previous state
        
        while True:
            try:
                # Read new messages from the stream
                response = await cls._redis_client.xreadgroup(
                    consumer_group,
                    consumer_name,
                    {stream_id: '>'}, # > means "give me messages that haven't been delivered to any other consumer"
                    count=batch_size
                )
                
                if response:  # We have messages
                    # Process the messages from the stream
                    stream_messages = response[0][1]  # First stream's messages
                    
                    for message_id, message_data in stream_messages:
                        # Convert message data to an event
                        event_type = message_data.get(b"event_type", b"").decode("utf-8")
                        event_data_str = message_data.get(b"event_data", b"{}").decode("utf-8")
                        event_data = json.loads(event_data_str)
                        
                        # Reconstruct and yield the event
                        event = cls._deserialize_event(event_type, event_data)
                        if event:
                            yield event
                            
                        # Acknowledge the message as processed
                        await cls._redis_client.xack(stream_id, consumer_group, message_id)
                        last_id = message_id
                else:
                    # No new messages, wait before polling again
                    await asyncio.sleep(poll_interval)
            except Exception as e:
                cls._logger.error(f"Error consuming from stream {stream_id}: {e}")
                await asyncio.sleep(poll_interval * 2)  # Wait a bit longer before retrying
    
    @classmethod
    def _serialize_event(cls, event: SessionEvent) -> Dict[str, Any]:
        """
        Serialize a SessionEvent to a dictionary for storage in Redis.
        
        Args:
            event: The event to serialize
            
        Returns:
            Dictionary representation of the event
        """
        # Use the model_dump method to convert Pydantic model to dict
        if hasattr(event, "model_dump"):
            # For newer Pydantic v2
            event_dict = event.model_dump(exclude={"content_bytes"})
        elif hasattr(event, "dict"):
            # For older Pydantic v1 
            event_dict = event.dict(exclude={"content_bytes"})
        else:
            # Fallback for non-Pydantic objects
            raise ValueError(f"Cannot serialize event of type {type(event).__name__}")
            
        return event_dict
    
    @classmethod
    def _deserialize_event(cls, event_type: str, event_data: Dict[str, Any]) -> Optional[SessionEvent]:
        """
        Reconstruct a SessionEvent from its serialized form.
        
        Args:
            event_type: Type of the event
            event_data: Serialized event data
            
        Returns:
            Reconstructed SessionEvent or None if reconstruction fails
        """
        from agent_c.models.events import (
            MessageEvent, TextDeltaEvent, RenderMediaEvent, ToolCallEvent,
            HistoryEvent, CompletionEvent, InteractionEvent, ThoughtDeltaEvent,
            ToolCallDeltaEvent, SystemMessageEvent
        )
        
        # Map event types to their classes
        event_classes = {
            "message": MessageEvent,
            "text_delta": TextDeltaEvent,
            "render_media": RenderMediaEvent,
            "tool_call": ToolCallEvent,
            "history": HistoryEvent,
            "completion": CompletionEvent,
            "interaction": InteractionEvent,
            "thought_delta": ThoughtDeltaEvent,
            "tool_call_delta": ToolCallDeltaEvent,
            "tool_select_delta": ToolCallDeltaEvent,  # Reusing same class
            "system_message": SystemMessageEvent,
        }
        
        event_class = event_classes.get(event_type)
        if not event_class:
            cls._logger.warning(f"Unknown event type: {event_type}")
            return None
        
        try:
            # Reconstruct the event
            return event_class(**event_data)
        except Exception as e:
            cls._logger.error(f"Failed to deserialize event of type {event_type}: {e}")
            return None
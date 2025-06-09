"""
Redis Stream Manager for Agent C

Core Redis Streams functionality for distributed event processing, providing
publish/consume capabilities with error handling and fallback mechanisms.
"""

import asyncio
import logging
import os
import time
from typing import Optional, Dict, Any, AsyncGenerator, Tuple, Union
from datetime import datetime, timedelta

try:
    import redis.asyncio as redis
    REDIS_AVAILABLE = True
except ImportError:
    REDIS_AVAILABLE = False

from agent_c.models import ChatEvent
from .stream_key_manager import StreamKeyManager
from .event_serializer import EventSerializer, EventContext


logger = logging.getLogger(__name__)


class RedisConfig:
    """Configuration for Redis Streams."""
    
    def __init__(self):
        self.host = os.getenv('REDIS_HOST', 'localhost')
        self.port = int(os.getenv('REDIS_PORT', 6379))
        self.password = os.getenv('REDIS_PASSWORD', None)
        self.db = int(os.getenv('REDIS_DB', 0))
        self.ssl = os.getenv('REDIS_SSL', 'false').lower() == 'true'
        
        # Stream configuration
        self.stream_max_length = int(os.getenv('REDIS_STREAM_MAX_LEN', 1000))
        self.stream_ttl = int(os.getenv('REDIS_STREAM_TTL', 3600))  # 1 hour
        self.read_timeout = int(os.getenv('REDIS_STREAM_READ_TIMEOUT', 1000))  # 1 second
        
        # Connection pool configuration
        self.max_connections = int(os.getenv('REDIS_MAX_CONNECTIONS', 10))
        self.socket_timeout = int(os.getenv('REDIS_SOCKET_TIMEOUT', 5))
        self.socket_connect_timeout = int(os.getenv('REDIS_SOCKET_CONNECT_TIMEOUT', 5))
        
        # Feature flags
        self.enabled = os.getenv('ENABLE_REDIS_STREAMS', 'false').lower() == 'true'
        self.fallback_enabled = os.getenv('REDIS_STREAMS_FALLBACK_TO_QUEUE', 'true').lower() == 'true'


class FallbackEventQueue:
    """In-memory fallback queue when Redis is unavailable."""
    
    def __init__(self, max_size: int = 10000):
        self._queue = asyncio.Queue(maxsize=max_size)
        self._fallback_active = False
        self._dropped_events = 0
    
    async def put(self, event_data: Dict[str, Any]):
        """Add event to fallback queue."""
        try:
            self._queue.put_nowait(event_data)
        except asyncio.QueueFull:
            self._dropped_events += 1
            logger.warning(f"Fallback queue full, dropped event. Total dropped: {self._dropped_events}")
    
    async def get(self) -> Optional[Dict[str, Any]]:
        """Get event from fallback queue."""
        try:
            return await asyncio.wait_for(self._queue.get(), timeout=1.0)
        except asyncio.TimeoutError:
            return None
    
    def activate(self):
        """Activate fallback mode."""
        self._fallback_active = True
        logger.warning("Redis Streams unavailable, activating in-memory fallback")
    
    def deactivate(self):
        """Deactivate fallback mode."""
        if self._fallback_active:
            logger.info("Redis Streams restored, deactivating fallback")
            self._fallback_active = False
    
    @property
    def is_active(self) -> bool:
        return self._fallback_active
    
    @property
    def size(self) -> int:
        return self._queue.qsize()


class RedisStreamManager:
    """Core Redis Streams manager for Agent C event processing."""
    
    def __init__(self, redis_config: Optional[RedisConfig] = None):
        """
        Initialize Redis Stream Manager.
        
        Args:
            redis_config: Redis configuration. If None, uses default configuration.
        """
        self.config = redis_config or RedisConfig()
        self._redis_client = None
        self._connection_pool = None
        self._fallback_queue = FallbackEventQueue()
        self._is_healthy = False
        self._last_health_check = None
        self._sequence_counter = 0
        
        # Check if Redis is available and enabled
        if not REDIS_AVAILABLE:
            logger.warning("Redis library not available, Redis Streams disabled")
            self.config.enabled = False
        
        if not self.config.enabled:
            logger.info("Redis Streams disabled, will use fallback queue only")
            self._fallback_queue.activate()
    
    async def initialize(self):
        """Initialize Redis connection and health checking."""
        if not self.config.enabled:
            return
        
        try:
            # Create connection pool
            self._connection_pool = redis.ConnectionPool(
                host=self.config.host,
                port=self.config.port,
                password=self.config.password,
                db=self.config.db,
                ssl=self.config.ssl,
                max_connections=self.config.max_connections,
                socket_timeout=self.config.socket_timeout,
                socket_connect_timeout=self.config.socket_connect_timeout,
                health_check_interval=30
            )
            
            # Create Redis client
            self._redis_client = redis.Redis(connection_pool=self._connection_pool)
            
            # Test connection
            await self._health_check()
            
            if self._is_healthy:
                logger.info("Redis Streams initialized successfully")
                self._fallback_queue.deactivate()
            else:
                logger.warning("Redis connection failed, using fallback queue")
                self._fallback_queue.activate()
                
        except Exception as e:
            logger.error(f"Failed to initialize Redis Streams: {e}")
            self._fallback_queue.activate()
    
    async def publish_event(self, 
                          event_type: str, 
                          event_data: Union[Dict[str, Any], ChatEvent], 
                          session_id: str, 
                          interaction_id: str, 
                          source: str = "BaseAgent",
                          user_id: str = "unknown") -> Optional[str]:
        """
        Publish an event to Redis Stream.
        
        Args:
            event_type: Type of the event
            event_data: Event data (dict or ChatEvent)
            session_id: Session identifier
            interaction_id: Interaction identifier
            source: Source component name
            user_id: User identifier
            
        Returns:
            Redis message ID if successful, None if failed
        """
        
        # Create event context
        context = EventContext(
            session_id=session_id,
            interaction_id=interaction_id,
            user_id=user_id,
            sequence=self._get_next_sequence(),
            source=source
        )
        
        # If Redis is not available or healthy, use fallback
        if not self._is_healthy or not self.config.enabled:
            await self._publish_to_fallback(event_type, event_data, context)
            return None
        
        try:
            # Build stream key
            stream_key = StreamKeyManager.build_stream_key(session_id, interaction_id)
            
            # Serialize event
            if isinstance(event_data, ChatEvent):
                serialized_message = EventSerializer.serialize_event(event_data, context)
            else:
                # Create a simple event wrapper for dict data
                class SimpleEvent:
                    def __init__(self, event_type: str, data: Dict[str, Any]):
                        self.type = event_type
                        self.data = data
                        
                    def to_dict(self):
                        return self.data
                
                simple_event = SimpleEvent(event_type, event_data)
                serialized_message = EventSerializer.serialize_event(simple_event, context)
            
            # Publish to Redis Stream
            message_id = await self._redis_client.xadd(
                stream_key,
                serialized_message,
                maxlen=self.config.stream_max_length,
                approximate=True
            )
            
            logger.debug(f"Published event {event_type} to stream {stream_key}: {message_id}")
            return message_id.decode() if isinstance(message_id, bytes) else message_id
            
        except Exception as e:
            logger.error(f"Failed to publish event to Redis Stream: {e}")
            await self._handle_redis_error(e)
            
            # Fallback to queue
            await self._publish_to_fallback(event_type, event_data, context)
            return None
    
    async def consume_events(self, 
                           session_id: str, 
                           interaction_id: str, 
                           last_id: str = '0-0', 
                           block: Optional[int] = None) -> AsyncGenerator[Dict[str, Any], None]:
        """
        Consume events from Redis Stream.
        
        Args:
            session_id: Session identifier
            interaction_id: Interaction identifier
            last_id: Last message ID received
            block: Block time in milliseconds, None for non-blocking
            
        Yields:
            Event data dictionaries
        """
        
        # If Redis is not available, consume from fallback
        if not self._is_healthy or not self.config.enabled:
            async for event_data in self._consume_from_fallback():
                yield event_data
            return
        
        try:
            stream_key = StreamKeyManager.build_stream_key(session_id, interaction_id)
            current_id = last_id
            
            while True:
                try:
                    # Read from stream
                    response = await self._redis_client.xread(
                        {stream_key: current_id},
                        count=10,
                        block=block or self.config.read_timeout
                    )
                    
                    if not response:
                        continue  # No new messages
                    
                    # Process messages
                    for stream, messages in response:
                        for message_id, fields in messages:
                            try:
                                # Convert bytes to strings
                                string_fields = {}
                                for key, value in fields.items():
                                    key_str = key.decode() if isinstance(key, bytes) else key
                                    value_str = value.decode() if isinstance(value, bytes) else value
                                    string_fields[key_str] = value_str
                                
                                # Add message ID
                                string_fields['message_id'] = message_id.decode() if isinstance(message_id, bytes) else message_id
                                
                                # Deserialize event
                                event_data, context = EventSerializer.deserialize_event(string_fields)
                                
                                # Add context info to event data
                                event_data['_context'] = {
                                    'message_id': string_fields['message_id'],
                                    'session_id': context.session_id,
                                    'interaction_id': context.interaction_id,
                                    'source': context.source,
                                    'sequence': context.sequence
                                }
                                
                                yield event_data
                                current_id = message_id
                                
                            except Exception as e:
                                logger.error(f"Error processing message {message_id}: {e}")
                                continue
                    
                except asyncio.TimeoutError:
                    continue  # Timeout is expected with blocking reads
                except Exception as e:
                    logger.error(f"Error reading from Redis Stream {stream_key}: {e}")
                    await self._handle_redis_error(e)
                    break
                    
        except Exception as e:
            logger.error(f"Error in consume_events: {e}")
            # Fallback to queue consumption
            async for event_data in self._consume_from_fallback():
                yield event_data
    
    async def create_stream(self, session_id: str, interaction_id: str) -> str:
        """
        Create a new stream with metadata.
        
        Args:
            session_id: Session identifier
            interaction_id: Interaction identifier
            
        Returns:
            Stream key
        """
        stream_key = StreamKeyManager.build_stream_key(session_id, interaction_id)
        
        if not self._is_healthy or not self.config.enabled:
            logger.debug(f"Redis not available, stream {stream_key} created in memory only")
            return stream_key
        
        try:
            # Create metadata event
            metadata_event = EventSerializer.create_stream_metadata_event(
                "stream_created",
                session_id,
                interaction_id,
                created_at=datetime.now().isoformat(),
                version="1.0"
            )
            
            # Add to stream
            message_id = await self._redis_client.xadd(stream_key, metadata_event)
            
            # Set TTL
            await self._redis_client.expire(stream_key, self.config.stream_ttl)
            
            logger.debug(f"Created stream {stream_key} with message {message_id}")
            return stream_key
            
        except Exception as e:
            logger.error(f"Failed to create stream {stream_key}: {e}")
            await self._handle_redis_error(e)
            return stream_key
    
    async def close_stream(self, session_id: str, interaction_id: str):
        """
        Close a stream with final metadata.
        
        Args:
            session_id: Session identifier
            interaction_id: Interaction identifier
        """
        if not self._is_healthy or not self.config.enabled:
            return
        
        try:
            stream_key = StreamKeyManager.build_stream_key(session_id, interaction_id)
            
            # Add closure metadata
            metadata_event = EventSerializer.create_stream_metadata_event(
                "stream_closed",
                session_id,
                interaction_id,
                closed_at=datetime.now().isoformat(),
                final_event="true"
            )
            
            await self._redis_client.xadd(stream_key, metadata_event)
            
            # Extend TTL for historical retention
            extended_ttl = self.config.stream_ttl * 24  # 24 hours for closed streams
            await self._redis_client.expire(stream_key, extended_ttl)
            
            logger.debug(f"Closed stream {stream_key}")
            
        except Exception as e:
            logger.error(f"Failed to close stream {stream_key}: {e}")
    
    async def cleanup_old_streams(self, max_age_seconds: int = 3600):
        """
        Clean up old streams.
        
        Args:
            max_age_seconds: Maximum age of streams to keep
        """
        if not self._is_healthy or not self.config.enabled:
            return
        
        try:
            pattern = StreamKeyManager.get_all_streams_pattern()
            async for key in self._redis_client.scan_iter(match=pattern):
                try:
                    # Check stream age using TTL
                    ttl = await self._redis_client.ttl(key)
                    if ttl == -1:  # No TTL set
                        await self._redis_client.expire(key, self.config.stream_ttl)
                    elif ttl < 0:  # Stream expired
                        continue
                        
                except Exception as e:
                    logger.error(f"Error checking stream {key}: {e}")
                    
        except Exception as e:
            logger.error(f"Error during cleanup: {e}")
    
    async def close(self):
        """Clean shutdown of Redis connections."""
        if self._redis_client:
            try:
                await self._redis_client.close()
            except Exception as e:
                logger.error(f"Error closing Redis client: {e}")
        
        if self._connection_pool:
            try:
                await self._connection_pool.disconnect()
            except Exception as e:
                logger.error(f"Error closing connection pool: {e}")
    
    async def _health_check(self):
        """Perform Redis health check."""
        try:
            if self._redis_client:
                await self._redis_client.ping()
                self._is_healthy = True
                self._last_health_check = datetime.now()
                if self._fallback_queue.is_active:
                    self._fallback_queue.deactivate()
        except Exception as e:
            self._is_healthy = False
            logger.warning(f"Redis health check failed: {e}")
            if not self._fallback_queue.is_active:
                self._fallback_queue.activate()
    
    async def _handle_redis_error(self, error: Exception):
        """Handle Redis connection errors."""
        self._is_healthy = False
        logger.error(f"Redis error: {error}")
        
        if not self._fallback_queue.is_active:
            self._fallback_queue.activate()
    
    async def _publish_to_fallback(self, event_type: str, event_data: Any, context: EventContext):
        """Publish event to fallback queue."""
        fallback_event = {
            'event_type': event_type,
            'event_data': event_data.to_dict() if hasattr(event_data, 'to_dict') else event_data,
            'context': context.__dict__,
            'timestamp': datetime.now().isoformat()
        }
        await self._fallback_queue.put(fallback_event)
    
    async def _consume_from_fallback(self) -> AsyncGenerator[Dict[str, Any], None]:
        """Consume events from fallback queue."""
        while True:
            event_data = await self._fallback_queue.get()
            if event_data:
                yield event_data
            else:
                await asyncio.sleep(0.1)  # Brief pause when no events
    
    def _get_next_sequence(self) -> int:
        """Get next sequence number."""
        self._sequence_counter += 1
        return self._sequence_counter
    
    @property
    def is_healthy(self) -> bool:
        """Check if Redis connection is healthy."""
        return self._is_healthy
    
    @property
    def is_fallback_active(self) -> bool:
        """Check if fallback queue is active."""
        return self._fallback_queue.is_active
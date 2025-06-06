import json
import logging
import asyncio
import pickle
import time
import uuid
from typing import Dict, List, Optional, Any, Set, Union
from datetime import datetime, timedelta

import redis.asyncio as redis
from agent_c.chat import ChatSession
from agent_c.models.agent_config import AgentConfiguration
from agent_c.models import ChatEvent
from agent_c_api.config.env_config import settings

# Configure logging
logging_manager = logging.getLogger(__name__)
logger = logging_manager


class RedisSessionManager:
    """
    Redis-based implementation of the ChatSessionManager for multi-user environments.
    
    This class provides a Redis-backed session storage mechanism that allows
    sessions to be shared across multiple instances of the application,
    enabling better scalability and reliability in multi-user environments.
    
    It also supports Redis Streams for event tracking, allowing events to be
    properly tracked in a distributed environment.
    
    Attributes:
        redis_client: AsyncIO Redis client instance
        prefix: Prefix for Redis keys to avoid collisions
        stream_prefix: Prefix for Redis stream keys
        session_ttl: Time-to-live for sessions in seconds
        local_cache: Optional local cache of session objects for performance
        stream_max_len: Maximum length of event streams
        stream_trim_interval: How often to trim streams (event count)
    """
    
    def __init__(
        self,
        redis_url: Optional[str] = None,
        prefix: str = "agent_c:session:",
        use_local_cache: bool = True,
    ) -> None:
        """
        Initialize the Redis Session Manager.
        
        Args:
            redis_url: Redis connection URL (redis://host:port/db)
                       If None, uses settings from env_config.py
            prefix: Prefix for Redis keys
            use_local_cache: Whether to use a local cache for performance
        """
        self.prefix = prefix
        self.session_ttl = settings.SESSION_TTL
        self.use_local_cache = use_local_cache
        self.cleanup_interval = settings.SESSION_CLEANUP_INTERVAL
        self.cleanup_batch_size = settings.SESSION_CLEANUP_BATCH_SIZE
        
        # Stream-related attributes
        self.stream_prefix = "agent_c:stream:"
        self.stream_max_len = getattr(settings, 'STREAM_MAX_LENGTH', 10000)
        self.stream_trim_interval = getattr(settings, 'STREAM_TRIM_INTERVAL', 100)
        self.event_count = 0  # Counter for trimming streams periodically
        
        # Build Redis URL from settings if not provided
        if redis_url is None:
            auth = ""
            if hasattr(settings, 'REDIS_USERNAME') and hasattr(settings, 'REDIS_PASSWORD') \
                and settings.REDIS_USERNAME and settings.REDIS_PASSWORD:
                auth = f"{settings.REDIS_USERNAME}:{settings.REDIS_PASSWORD}@"
            elif hasattr(settings, 'REDIS_PASSWORD') and settings.REDIS_PASSWORD:
                auth = f":{settings.REDIS_PASSWORD}@"
            
            redis_url = f"redis://{auth}{settings.REDIS_HOST}:{settings.REDIS_PORT}/{settings.REDIS_DB}"
        
        # Initialize Redis connection pool with settings from env_config
        self.redis_client = redis.from_url(
            redis_url,
            encoding="utf-8",
            decode_responses=False,  # We'll handle decoding ourselves
            socket_timeout=settings.REDIS_SOCKET_TIMEOUT,
            socket_connect_timeout=settings.REDIS_CONNECTION_TIMEOUT,
            max_connections=settings.REDIS_MAX_CONNECTIONS
        )
        
        # Local cache for performance (optional)
        self._local_cache: Dict[str, ChatSession] = {}
        self._session_keys: Set[str] = set()  # Track active session IDs
        
        logger.info(f"RedisSessionManager initialized with prefix: {prefix}")
    
    def _get_session_key(self, session_id: str) -> str:
        """
        Generate a Redis key for a session.
        
        Args:
            session_id: The session ID
            
        Returns:
            The full Redis key including prefix
        """
        return f"{self.prefix}{session_id}"
    
    def _get_index_key(self) -> str:
        """
        Generate the Redis key for the session index.
        
        Returns:
            The full Redis key for the session index
        """
        return f"{self.prefix}index"
    
    def get_stream_key(self, session_id: str) -> str:
        """
        Generate a Redis key for a session's event stream.
        
        Args:
            session_id: The session ID
            
        Returns:
            The full Redis key for the session's event stream
        """
        return f"{self.stream_prefix}{session_id}"
    
    def generate_stream_id(self) -> str:
        """
        Generate a unique, time-ordered stream ID for Redis streams.
        
        Returns:
            A string in the format "timestamp-uuid" suitable for Redis streams
        """
        timestamp = int(time.time() * 1000)  # Millisecond timestamp
        unique_id = uuid.uuid4().hex[:8]    # Short UUID
        return f"{timestamp}-{unique_id}"
    
    async def new_session(self, session: ChatSession) -> None:
        """
        Create a new chat session and store it in Redis.
        
        Args:
            session: The chat session to store
            
        Raises:
            redis.RedisError: If there's an error communicating with Redis
        """
        try:
            # Serialize the session object
            session_data = pickle.dumps(session)
            session_key = self._get_session_key(session.session_id)
            index_key = self._get_index_key()
            
            # Store session in Redis with TTL
            async with self.redis_client.pipeline() as pipe:
                await pipe.set(session_key, session_data, ex=self.session_ttl)
                await pipe.sadd(index_key, session.session_id)  # Add to index
                await pipe.execute()
            
            # Update local cache if enabled
            if self.use_local_cache:
                self._local_cache[session.session_id] = session
                self._session_keys.add(session.session_id)
            
            logger.info(f"Created new session: {session.session_id}")
        except redis.RedisError as e:
            logger.error(f"Error creating session in Redis: {e}")
            raise
    
    async def get_session(self, session_id: str) -> ChatSession:
        """
        Retrieve a session from Redis by ID.
        
        Args:
            session_id: The ID of the session to retrieve
            
        Returns:
            The retrieved chat session
            
        Raises:
            KeyError: If the session doesn't exist
            redis.RedisError: If there's an error communicating with Redis
        """
        # Check local cache first if enabled
        if self.use_local_cache and session_id in self._local_cache:
            logger.debug(f"Session {session_id} found in local cache")
            return self._local_cache[session_id]
        
        try:
            # Get session data from Redis
            session_key = self._get_session_key(session_id)
            session_data = await self.redis_client.get(session_key)
            
            if not session_data:
                logger.warning(f"Session {session_id} not found in Redis")
                raise KeyError(f"Session {session_id} not found")
            
            # Deserialize the session object
            session = pickle.loads(session_data)
            
            # Update local cache if enabled
            if self.use_local_cache:
                self._local_cache[session_id] = session
                self._session_keys.add(session_id)
            
            # Refresh TTL to prevent expiration of active sessions
            await self.redis_client.expire(session_key, self.session_ttl)
            
            logger.debug(f"Retrieved session: {session_id}")
            return session
        except redis.RedisError as e:
            logger.error(f"Error retrieving session {session_id} from Redis: {e}")
            raise
    
    async def update(self) -> None:
        """
        Update all sessions in the local cache to Redis.
        
        This method is used to periodically flush local changes to Redis.
        
        Raises:
            redis.RedisError: If there's an error communicating with Redis
        """
        if not self.use_local_cache or not self._local_cache:
            return
        
        try:
            # Use pipeline for efficient multi-operation
            async with self.redis_client.pipeline() as pipe:
                for session_id, session in self._local_cache.items():
                    session_key = self._get_session_key(session_id)
                    session_data = pickle.dumps(session)
                    pipe.set(session_key, session_data, ex=self.session_ttl)
                
                await pipe.execute()
            
            logger.debug(f"Updated {len(self._local_cache)} sessions to Redis")
        except redis.RedisError as e:
            logger.error(f"Error updating sessions to Redis: {e}")
            raise
    
    async def flush(self, session_id: str) -> None:
        """
        Flush a specific session to Redis.
        
        Args:
            session_id: The ID of the session to flush
            
        Raises:
            KeyError: If the session doesn't exist in the local cache
            redis.RedisError: If there's an error communicating with Redis
        """
        if not self.use_local_cache:
            return
        
        if session_id not in self._local_cache:
            logger.warning(f"Session {session_id} not found in local cache for flushing")
            raise KeyError(f"Session {session_id} not found in local cache")
        
        try:
            session = self._local_cache[session_id]
            session_key = self._get_session_key(session_id)
            session_data = pickle.dumps(session)
            
            # Update in Redis with TTL
            await self.redis_client.set(session_key, session_data, ex=self.session_ttl)
            
            logger.debug(f"Flushed session {session_id} to Redis")
        except redis.RedisError as e:
            logger.error(f"Error flushing session {session_id} to Redis: {e}")
            raise
    
    @property
    async def session_id_list(self) -> List[str]:
        """
        Get a list of all active session IDs.
        
        Returns:
            List of session IDs
            
        Raises:
            redis.RedisError: If there's an error communicating with Redis
        """
        try:
            # Get session IDs from Redis index
            index_key = self._get_index_key()
            session_ids = await self.redis_client.smembers(index_key)
            
            # Convert to list of strings
            session_id_list = [sid.decode('utf-8') if isinstance(sid, bytes) else sid 
                              for sid in session_ids]
            
            # Update local cache of session IDs if enabled
            if self.use_local_cache:
                self._session_keys = set(session_id_list)
            
            return session_id_list
        except redis.RedisError as e:
            logger.error(f"Error retrieving session ID list from Redis: {e}")
            
            # Fall back to local cache if available
            if self.use_local_cache:
                logger.warning("Falling back to local session ID cache")
                return list(self._session_keys)
            
            raise
    
    async def delete_session(self, session_id: str) -> bool:
        """
        Delete a session from Redis.
        
        Args:
            session_id: The ID of the session to delete
            
        Returns:
            True if the session was deleted, False if it didn't exist
            
        Raises:
            redis.RedisError: If there's an error communicating with Redis
        """
        try:
            session_key = self._get_session_key(session_id)
            index_key = self._get_index_key()
            stream_key = self.get_stream_key(session_id)
            
            # Remove from Redis
            async with self.redis_client.pipeline() as pipe:
                pipe.delete(session_key)
                pipe.srem(index_key, session_id)
                pipe.delete(stream_key)  # Also delete the event stream
                results = await pipe.execute()
            
            # Remove from local cache if enabled
            if self.use_local_cache:
                self._local_cache.pop(session_id, None)
                self._session_keys.discard(session_id)
            
            deleted = results[0] > 0
            if deleted:
                logger.info(f"Deleted session: {session_id}")
            else:
                logger.warning(f"Session {session_id} not found for deletion")
            
            return deleted
        except redis.RedisError as e:
            logger.error(f"Error deleting session {session_id} from Redis: {e}")
            raise
    
    async def cleanup_expired_sessions(self) -> int:
        """
        Clean up references to expired sessions from the index.
        
        This method checks all sessions in the index and removes references
        to any that have expired in Redis. It processes sessions in batches
        of size defined by settings.SESSION_CLEANUP_BATCH_SIZE.
        
        Returns:
            Number of expired sessions cleaned up
            
        Raises:
            redis.RedisError: If there's an error communicating with Redis
        """
        try:
            # Get all session IDs from the index
            index_key = self._get_index_key()
            all_session_ids = await self.redis_client.smembers(index_key)
            session_ids = [sid.decode('utf-8') if isinstance(sid, bytes) else sid 
                          for sid in all_session_ids]
            
            total_expired = 0
            expired_sessions = []
            
            # Process in batches to avoid large pipeline operations
            for i in range(0, len(session_ids), self.cleanup_batch_size):
                batch = session_ids[i:i + self.cleanup_batch_size]
                
                # Check which sessions still exist
                async with self.redis_client.pipeline() as pipe:
                    for session_id in batch:
                        session_key = self._get_session_key(session_id)
                        pipe.exists(session_key)
                    
                    results = await pipe.execute()
                
                # Identify expired sessions
                for j, exists in enumerate(results):
                    if not exists:
                        expired_sessions.append(batch[j])
            
            # Remove expired sessions from index
            if expired_sessions:
                # Also clean up streams for expired sessions
                async with self.redis_client.pipeline() as pipe:
                    # Remove from index
                    await pipe.srem(index_key, *expired_sessions)
                    
                    # Remove streams
                    for session_id in expired_sessions:
                        stream_key = self.get_stream_key(session_id)
                        pipe.delete(stream_key)
                    
                    await pipe.execute()
                
                # Update local cache if enabled
                if self.use_local_cache:
                    for session_id in expired_sessions:
                        self._local_cache.pop(session_id, None)
                        self._session_keys.discard(session_id)
                
                total_expired = len(expired_sessions)
                logger.info(f"Cleaned up {total_expired} expired sessions")
            
            return total_expired
        except redis.RedisError as e:
            logger.error(f"Error cleaning up expired sessions: {e}")
            raise
    
    async def add_event_to_stream(self, session_id: str, event_data: Dict[str, Any]) -> str:
        """
        Add an event to a session's Redis stream.
        
        Args:
            session_id: The session ID
            event_data: The event data to add to the stream
           
        Returns:
            The ID of the added event in the stream
            
        Raises:
            redis.RedisError: If there's an error communicating with Redis
        """
        try:
            stream_key = self.get_stream_key(session_id)
            stream_id = self.generate_stream_id()
            
            # Convert event data to Redis compatible format
            # Redis streams require field-value pairs
            fields = {}
            for key, value in event_data.items():
                if isinstance(value, (dict, list, tuple)):
                    fields[key] = json.dumps(value)
                elif not isinstance(value, (str, int, float, bool, type(None))):
                    fields[key] = str(value)
                else:
                    fields[key] = value
            
            # Add event to stream
            await self.redis_client.xadd(
                stream_key,
                fields,
                id=stream_id,
                maxlen=self.stream_max_len
            )
            
            # Increment event count and trim periodically
            self.event_count += 1
            if self.event_count >= self.stream_trim_interval:
                self.event_count = 0
                await self.trim_stream(session_id)
            
            logger.debug(f"Added event to stream {stream_key} with ID {stream_id}")
            return stream_id
        except redis.RedisError as e:
            logger.error(f"Error adding event to stream for session {session_id}: {e}")
            raise
    
    async def get_events_from_stream(self, session_id: str, start: str = "0", end: str = "+", count: Optional[int] = None) -> List[Dict[str, Any]]:
        """
        Get events from a session's Redis stream.
        
        Args:
            session_id: The session ID
            start: Start ID (inclusive)
            end: End ID (inclusive)
            count: Maximum number of events to return
            
        Returns:
            List of events with their IDs and data
            
        Raises:
            redis.RedisError: If there's an error communicating with Redis
        """
        try:
            stream_key = self.get_stream_key(session_id)
            
            # Get events from stream
            events_data = await self.redis_client.xrange(
                stream_key,
                min=start,
                max=end,
                count=count
            )
            
            # Process events
            events = []
            for event_id, event_fields in events_data:
                # Convert Redis response to Python dict
                event_id = event_id.decode('utf-8') if isinstance(event_id, bytes) else event_id
                
                event_data = {}
                for field, value in event_fields.items():
                    field = field.decode('utf-8') if isinstance(field, bytes) else field
                    value = value.decode('utf-8') if isinstance(value, bytes) else value
                    
                    # Try to parse JSON for complex fields
                    try:
                        if isinstance(value, str) and (value.startswith('{') or value.startswith('[')):
                            value = json.loads(value)
                    except json.JSONDecodeError:
                        pass
                    
                    event_data[field] = value
                
                events.append({
                    'id': event_id,
                    'data': event_data
                })
            
            logger.debug(f"Retrieved {len(events)} events from stream {stream_key}")
            return events
        except redis.RedisError as e:
            logger.error(f"Error retrieving events from stream for session {session_id}: {e}")
            raise
    
    async def trim_stream(self, session_id: str, max_len: Optional[int] = None) -> None:
        """
        Trim a session's Redis stream to a maximum length.
        
        Args:
            session_id: The session ID
            max_len: Maximum length to trim to (defaults to self.stream_max_len)
            
        Raises:
            redis.RedisError: If there's an error communicating with Redis
        """
        try:
            if max_len is None:
                max_len = self.stream_max_len
                
            stream_key = self.get_stream_key(session_id)
            await self.redis_client.xtrim(stream_key, maxlen=max_len)
            logger.debug(f"Trimmed stream {stream_key} to max length {max_len}")
        except redis.RedisError as e:
            logger.error(f"Error trimming stream for session {session_id}: {e}")
            # Don't raise, just log - trimming is not critical
    
    async def close(self) -> None:
        """
        Close the Redis connection.
        
        This method should be called when the session manager is no longer needed
        to properly clean up resources.
        """
        try:
            await self.redis_client.close()
            logger.info("Redis connection closed")
        except Exception as e:
            logger.error(f"Error closing Redis connection: {e}")
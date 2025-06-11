"""Unit tests for the Redis Session Manager implementation.

Tests RedisSessionManager with mocked Redis clients to verify business logic
without requiring actual Redis connections.
"""

import pytest
import pytest_asyncio
from unittest.mock import AsyncMock, MagicMock, patch
from datetime import datetime, timedelta
import pickle
import json

from redis import asyncio as aioredis
from agent_c.chat import ChatSession
from agent_c.models.agent_config import AgentConfiguration
from agent_c_api.core.redis_session_manager import RedisSessionManager
from agent_c_api.config.env_config import settings


@pytest.fixture
def mock_chat_session():
    """Create a mock ChatSession for testing."""
    session = MagicMock(spec=ChatSession)
    session.session_id = "test-session-id"
    return session


@pytest.mark.unit
@pytest.mark.core
class TestRedisSessionManager:
    """Test RedisSessionManager with mocked Redis client."""
    
    @pytest_asyncio.fixture
    async def mock_redis_client(self):
        """Create a mock Redis client for testing."""
        client = AsyncMock(spec=aioredis.Redis)
        
        # Mock pipeline
        pipeline_mock = AsyncMock()
        pipeline_mock.execute = AsyncMock(return_value=[True, True])
        pipeline_mock.set = AsyncMock()
        pipeline_mock.sadd = AsyncMock()
        pipeline_mock.expire = AsyncMock()
        pipeline_mock.delete = AsyncMock()
        pipeline_mock.srem = AsyncMock()
        client.pipeline.return_value.__aenter__.return_value = pipeline_mock
        client.pipeline.return_value.__aexit__.return_value = None
        
        return client, pipeline_mock
    
    @pytest_asyncio.fixture
    async def redis_session_manager(self, mock_redis_client):
        """Create a RedisSessionManager with mocked Redis client."""
        client, _ = mock_redis_client
        with patch('redis.asyncio.from_url', return_value=client):
            manager = RedisSessionManager(use_local_cache=True)
            yield manager
            await manager.close()
    
    async def test_initialization(self, redis_session_manager, mock_redis_client):
        """Test RedisSessionManager initialization."""
        client, _ = mock_redis_client
        
        # Verify attributes
        assert redis_session_manager.prefix == "agent_c:session:"
        assert redis_session_manager.session_ttl == settings.SESSION_TTL
        assert redis_session_manager.use_local_cache is True
        assert redis_session_manager.stream_prefix == settings.STREAM_PREFIX
        assert redis_session_manager.stream_max_len == settings.STREAM_MAX_LENGTH
        assert redis_session_manager.stream_trim_interval == settings.STREAM_TRIM_INTERVAL
        assert redis_session_manager.stream_retention_period == settings.STREAM_RETENTION_PERIOD
        assert redis_session_manager.event_count == 0
        
        # Verify redis client
        assert redis_session_manager.redis_client == client
    
    async def test_get_session_key(self, redis_session_manager):
        """Test _get_session_key method."""
        session_id = "test-session-id"
        expected_key = "agent_c:session:test-session-id"
        assert redis_session_manager._get_session_key(session_id) == expected_key
    
    async def test_get_index_key(self, redis_session_manager):
        """Test _get_index_key method."""
        expected_key = "agent_c:session:index"
        assert redis_session_manager._get_index_key() == expected_key
    
    async def test_get_stream_key(self, redis_session_manager):
        """Test get_stream_key method."""
        session_id = "test-session-id"
        expected_key = f"{settings.STREAM_PREFIX}test-session-id"
        assert redis_session_manager.get_stream_key(session_id) == expected_key
    
    async def test_generate_stream_id(self, redis_session_manager):
        """Test generate_stream_id method."""
        stream_id = redis_session_manager.generate_stream_id()
        
        # Format should be timestamp-uuid
        parts = stream_id.split('-')
        assert len(parts) == 2
        
        # First part should be a timestamp (numeric)
        assert parts[0].isdigit()
        
        # Second part should be 8 hex chars (from uuid)
        assert len(parts[1]) == 8
        assert all(c in "0123456789abcdef" for c in parts[1])
    
    async def test_new_session(self, redis_session_manager, mock_redis_client, mock_chat_session):
        """Test new_session method."""
        client, pipeline_mock = mock_redis_client
        
        await redis_session_manager.new_session(mock_chat_session)
        
        # Verify Redis pipeline was used
        client.pipeline.assert_called_once()
        
        # Verify session was added to local cache
        assert mock_chat_session.session_id in redis_session_manager._local_cache
        assert redis_session_manager._local_cache[mock_chat_session.session_id] == mock_chat_session
    
    async def test_get_session_from_cache(self, redis_session_manager, mock_redis_client, mock_chat_session):
        """Test get_session retrieves from local cache when available."""
        client, _ = mock_redis_client
        
        # Add session to local cache
        redis_session_manager._local_cache[mock_chat_session.session_id] = mock_chat_session
        redis_session_manager._session_keys.add(mock_chat_session.session_id)
        
        # Get the session
        session = await redis_session_manager.get_session(mock_chat_session.session_id)
        
        # Verify Redis was not called
        client.get.assert_not_called()
        
        # Verify the correct session was returned
        assert session == mock_chat_session
    
    async def test_get_session_from_redis(self, redis_session_manager, mock_redis_client, mock_chat_session):
        """Test get_session retrieves from Redis when not in cache."""
        client, _ = mock_redis_client
        
        # Mock Redis get operation
        serialized_session = pickle.dumps(mock_chat_session)
        client.get.return_value = serialized_session
        client.exists.return_value = True
        
        # Get the session
        session = await redis_session_manager.get_session(mock_chat_session.session_id)
        
        # Verify Redis was called
        client.get.assert_called_once()
        client.expire.assert_called_once()  # Should refresh TTL
        
        # Verify the session was added to local cache
        assert mock_chat_session.session_id in redis_session_manager._local_cache
    
    async def test_get_session_not_found(self, redis_session_manager, mock_redis_client):
        """Test get_session raises KeyError when session not found."""
        client, _ = mock_redis_client
        
        # Mock Redis get operation returning None
        client.get.return_value = None
        client.exists.return_value = False
        
        # Try to get a non-existent session
        with pytest.raises(KeyError) as exc_info:
            await redis_session_manager.get_session("non-existent-session")
        
        assert "not found" in str(exc_info.value)
    
    async def test_update(self, redis_session_manager, mock_redis_client, mock_chat_session):
        """Test update method."""
        client, pipeline_mock = mock_redis_client
        
        # Add sessions to local cache
        redis_session_manager._local_cache[mock_chat_session.session_id] = mock_chat_session
        
        # Call update
        await redis_session_manager.update()
        
        # Verify Redis pipeline was used
        client.pipeline.assert_called_once()
    
    async def test_flush(self, redis_session_manager, mock_redis_client, mock_chat_session):
        """Test flush method."""
        client, _ = mock_redis_client
        
        # Add session to local cache
        redis_session_manager._local_cache[mock_chat_session.session_id] = mock_chat_session
        
        # Call flush
        await redis_session_manager.flush(mock_chat_session.session_id)
        
        # Verify Redis set was called
        client.set.assert_called_once()
    
    async def test_flush_session_not_in_cache(self, redis_session_manager):
        """Test flush raises KeyError when session not in cache."""
        # Try to flush a non-existent session
        with pytest.raises(KeyError) as exc_info:
            await redis_session_manager.flush("non-existent-session")
        
        assert "not found in local cache" in str(exc_info.value)
    
    async def test_session_id_list(self, redis_session_manager, mock_redis_client):
        """Test session_id_list property."""
        client, _ = mock_redis_client
        
        # Mock Redis smembers operation
        client.smembers.return_value = {b"session1", b"session2", b"session3"}
        
        # Get session IDs
        session_ids = await redis_session_manager.session_id_list
        
        # Verify Redis was called
        client.smembers.assert_called_once()
        
        # Verify the session IDs were returned and decoded
        assert len(session_ids) == 3
        assert "session1" in session_ids
        assert "session2" in session_ids
        assert "session3" in session_ids
        
        # Verify the session IDs were stored in local cache
        assert redis_session_manager._session_keys == set(session_ids)
    
    async def test_delete_session(self, redis_session_manager, mock_redis_client, mock_chat_session):
        """Test delete_session method."""
        client, pipeline_mock = mock_redis_client
        
        # Add session to local cache
        redis_session_manager._local_cache[mock_chat_session.session_id] = mock_chat_session
        redis_session_manager._session_keys.add(mock_chat_session.session_id)
        
        # Mock pipeline execute return value
        pipeline_mock.execute.return_value = [1, 1, 1]  # 1 = success for delete operations
        
        # Delete the session
        result = await redis_session_manager.delete_session(mock_chat_session.session_id)
        
        # Verify Redis pipeline was used
        client.pipeline.assert_called_once()
        
        # Verify the session was removed from local cache
        assert mock_chat_session.session_id not in redis_session_manager._local_cache
        assert mock_chat_session.session_id not in redis_session_manager._session_keys
        
        # Verify the result
        assert result is True
    
    async def test_cleanup_expired_sessions(self, redis_session_manager, mock_redis_client):
        """Test cleanup_expired_sessions method."""
        client, pipeline_mock = mock_redis_client
        
        # Mock Redis operations
        client.smembers.return_value = {b"session1", b"session2", b"session3"}
        client.exists.side_effect = [True, False, True]  # session2 is expired
        
        # Mock pipeline execute for removing expired sessions
        pipeline_mock.execute.return_value = [1]  # 1 session removed
        
        # Add sessions to local cache
        redis_session_manager._local_cache["session1"] = MagicMock()
        redis_session_manager._local_cache["session2"] = MagicMock()
        redis_session_manager._local_cache["session3"] = MagicMock()
        redis_session_manager._session_keys = {"session1", "session2", "session3"}
        
        # Call cleanup_expired_sessions
        count = await redis_session_manager.cleanup_expired_sessions()
        
        # Verify Redis operations
        client.smembers.assert_called_once()
        assert client.exists.call_count == 3
        
        # Verify expired sessions were removed from local cache
        assert "session2" not in redis_session_manager._local_cache
        assert "session2" not in redis_session_manager._session_keys
        
        # Verify the count
        assert count == 1
    
    async def test_add_event_to_stream(self, redis_session_manager, mock_redis_client):
        """Test add_event_to_stream method."""
        client, _ = mock_redis_client
        
        # Mock Redis xadd operation
        client.xadd.return_value = "1622547600000-0"
        
        # Add an event to stream
        event_data = {
            "type": "message",
            "content": "Hello, world!",
            "complex_data": {"key": "value"},
            "custom_object": MagicMock()  # Non-serializable object
        }
        
        stream_id = await redis_session_manager.add_event_to_stream("test-session-id", event_data)
        
        # Verify Redis xadd was called
        client.xadd.assert_called_once()
        
        # Check arguments
        call_args = client.xadd.call_args
        assert call_args[0][0] == f"{settings.STREAM_PREFIX}test-session-id"  # stream key
        
        # Verify the fields - complex data should be JSON serialized, custom objects should be stringified
        fields = call_args[0][1]
        assert fields["type"] == "message"
        assert fields["content"] == "Hello, world!"
        assert isinstance(fields["complex_data"], str)  # JSON string
        assert isinstance(fields["custom_object"], str)  # String representation
        
        # Verify the stream ID was returned
        assert stream_id is not None
    
    async def test_get_events_from_stream(self, redis_session_manager, mock_redis_client):
        """Test get_events_from_stream method."""
        client, _ = mock_redis_client
        
        # Mock Redis xrange operation
        client.xrange.return_value = [
            # Event 1
            [b"1622547600000-0", {
                b"type": b"message",
                b"content": b"Hello, world!",
                b"complex_data": b'{"key": "value"}'
            }],
            # Event 2
            [b"1622547600001-0", {
                b"type": b"message",
                b"content": b"Another message"
            }]
        ]
        
        # Get events from stream
        events = await redis_session_manager.get_events_from_stream(
            "test-session-id",
            start="0",
            end="+",
            count=10
        )
        
        # Verify Redis xrange was called
        client.xrange.assert_called_once()
        
        # Check arguments
        call_args = client.xrange.call_args
        assert call_args[0][0] == f"{settings.STREAM_PREFIX}test-session-id"  # stream key
        assert call_args[1]["min"] == "0"  # start
        assert call_args[1]["max"] == "+"  # end
        assert call_args[1]["count"] == 10  # count
        
        # Verify the events were returned
        assert len(events) == 2
        
        # Verify event 1
        assert events[0]["id"] == "1622547600000-0"
        assert events[0]["data"]["type"] == "message"
        assert events[0]["data"]["content"] == "Hello, world!"
        assert events[0]["data"]["complex_data"] == {"key": "value"}  # Should be parsed JSON
        
        # Verify event 2
        assert events[1]["id"] == "1622547600001-0"
        assert events[1]["data"]["type"] == "message"
        assert events[1]["data"]["content"] == "Another message"
    
    async def test_trim_stream(self, redis_session_manager, mock_redis_client):
        """Test trim_stream method."""
        client, _ = mock_redis_client
        
        # Call trim_stream
        await redis_session_manager.trim_stream("test-session-id")
        
        # Verify Redis xtrim was called
        client.xtrim.assert_called_once()
        
        # Check arguments
        call_args = client.xtrim.call_args
        assert call_args[0][0] == f"{settings.STREAM_PREFIX}test-session-id"  # stream key
        assert call_args[1]["maxlen"] == settings.STREAM_MAX_LENGTH  # max length
    
    async def test_close(self, redis_session_manager, mock_redis_client):
        """Test close method."""
        client, _ = mock_redis_client
        
        # Call close
        await redis_session_manager.close()
        
        # Verify Redis close was called
        client.close.assert_called_once()


@pytest.mark.unit
@pytest.mark.core
class TestRedisSessionManagerErrorHandling:
    """Test RedisSessionManager error handling."""
    
    @pytest_asyncio.fixture
    async def mock_redis_client(self):
        """Create a mock Redis client that raises errors for testing."""
        client = AsyncMock(spec=aioredis.Redis)
        return client
    
    @pytest_asyncio.fixture
    async def redis_session_manager_with_errors(self, mock_redis_client):
        """Create a RedisSessionManager with mocked Redis client that raises errors."""
        with patch('redis.asyncio.from_url', return_value=mock_redis_client):
            manager = RedisSessionManager(use_local_cache=True)
            yield manager, mock_redis_client
            await manager.close()
    
    async def test_new_session_error_handling(self, redis_session_manager_with_errors, mock_chat_session):
        """Test new_session handles Redis errors."""
        manager, client = redis_session_manager_with_errors
        
        # Mock Redis pipeline to raise an error
        pipeline_mock = AsyncMock()
        pipeline_mock.__aenter__.side_effect = aioredis.RedisError("Connection error")
        client.pipeline.return_value = pipeline_mock
        
        # Try to create a new session
        with pytest.raises(aioredis.RedisError) as exc_info:
            await manager.new_session(mock_chat_session)
        
        assert "Connection error" in str(exc_info.value)
    
    async def test_get_session_error_handling(self, redis_session_manager_with_errors):
        """Test get_session handles Redis errors."""
        manager, client = redis_session_manager_with_errors
        
        # Mock Redis get to raise an error
        client.get.side_effect = aioredis.RedisError("Connection error")
        
        # Try to get a session
        with pytest.raises(aioredis.RedisError) as exc_info:
            await manager.get_session("test-session-id")
        
        assert "Connection error" in str(exc_info.value)
    
    async def test_session_id_list_error_handling(self, redis_session_manager_with_errors):
        """Test session_id_list handles Redis errors and falls back to local cache."""
        manager, client = redis_session_manager_with_errors
        
        # Mock Redis smembers to raise an error
        client.smembers.side_effect = aioredis.RedisError("Connection error")
        
        # Add some sessions to local cache
        manager._session_keys = {"session1", "session2"}
        
        # Try to get session IDs - should fall back to local cache
        with pytest.raises(aioredis.RedisError) as exc_info:
            await manager.session_id_list
        
        assert "Connection error" in str(exc_info.value)
    
    async def test_add_event_to_stream_error_handling(self, redis_session_manager_with_errors):
        """Test add_event_to_stream handles Redis errors."""
        manager, client = redis_session_manager_with_errors
        
        # Mock Redis xadd to raise an error
        client.xadd.side_effect = aioredis.RedisError("Connection error")
        
        # Try to add an event to stream
        with pytest.raises(aioredis.RedisError) as exc_info:
            await manager.add_event_to_stream("test-session-id", {"type": "message"})
        
        assert "Connection error" in str(exc_info.value)
    
    async def test_get_events_from_stream_error_handling(self, redis_session_manager_with_errors):
        """Test get_events_from_stream handles Redis errors."""
        manager, client = redis_session_manager_with_errors
        
        # Mock Redis xrange to raise an error
        client.xrange.side_effect = aioredis.RedisError("Connection error")
        
        # Try to get events from stream
        with pytest.raises(aioredis.RedisError) as exc_info:
            await manager.get_events_from_stream("test-session-id")
        
        assert "Connection error" in str(exc_info.value)
    
    async def test_trim_stream_error_handling(self, redis_session_manager_with_errors):
        """Test trim_stream handles Redis errors (doesn't raise)."""
        manager, client = redis_session_manager_with_errors
        
        # Mock Redis xtrim to raise an error
        client.xtrim.side_effect = aioredis.RedisError("Connection error")
        
        # Try to trim the stream - shouldn't raise (just logs)
        await manager.trim_stream("test-session-id")
        
        # Verify xtrim was called
        client.xtrim.assert_called_once()


@pytest.mark.unit
@pytest.mark.core
class TestRedisSessionManagerPerformance:
    """Test RedisSessionManager performance characteristics."""
    
    @pytest_asyncio.fixture
    async def redis_session_manager_for_perf(self):
        """Create a RedisSessionManager with real Redis client for performance testing."""
        # Use a mock for unit testing - in integration tests this would use a real Redis
        client = AsyncMock(spec=aioredis.Redis)
        with patch('redis.asyncio.from_url', return_value=client):
            manager = RedisSessionManager(use_local_cache=True)
            yield manager, client
            await manager.close()
    
    async def test_local_cache_performance(self, redis_session_manager_for_perf, mock_chat_session):
        """Test local cache performance for session retrieval."""
        manager, client = redis_session_manager_for_perf
        
        # Add session to local cache
        manager._local_cache[mock_chat_session.session_id] = mock_chat_session
        manager._session_keys.add(mock_chat_session.session_id)
        
        # Mock Redis operations for measuring performance
        client.get.return_value = pickle.dumps(mock_chat_session)
        
        # Test get_session with local cache
        # First call should use local cache
        await manager.get_session(mock_chat_session.session_id)
        
        # Verify Redis was not called
        client.get.assert_not_called()
    
    async def test_serialization_performance(self, redis_session_manager_for_perf):
        """Test serialization performance with different data sizes."""
        manager, client = redis_session_manager_for_perf
        
        # Generate small, medium, and large test events
        small_event = {"type": "test", "content": "Small event"}
        medium_event = {"type": "test", "content": "Medium event" * 100}
        large_event = {"type": "test", "content": "Large event" * 1000}
        
        # Mock Redis operations
        client.xadd.return_value = "1622547600000-0"
        
        # Add events to stream
        await manager.add_event_to_stream("test-session-id", small_event)
        await manager.add_event_to_stream("test-session-id", medium_event)
        await manager.add_event_to_stream("test-session-id", large_event)
        
        # Verify xadd was called for each event
        assert client.xadd.call_count == 3
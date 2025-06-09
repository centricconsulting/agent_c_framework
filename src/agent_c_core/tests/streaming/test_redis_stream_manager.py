"""
Tests for Redis Stream Manager

This module contains comprehensive tests for the Redis Streams integration,
including unit tests and integration tests with mock Redis.
"""

import pytest
import asyncio
import json
from unittest.mock import Mock, AsyncMock, patch
from datetime import datetime

from agent_c.streaming.redis_stream_manager import RedisStreamManager, RedisConfig, FallbackEventQueue
from agent_c.streaming.stream_key_manager import StreamKeyManager
from agent_c.streaming.event_serializer import EventSerializer, EventContext
from agent_c.models.events import TextDeltaEvent


class TestStreamKeyManager:
    """Test cases for StreamKeyManager utility class."""
    
    def test_build_stream_key(self):
        """Test stream key building with valid inputs."""
        key = StreamKeyManager.build_stream_key("user_123", "chat_001")
        assert key == "agent_c:stream:user_123:chat_001"
    
    def test_build_stream_key_sanitization(self):
        """Test stream key building with inputs that need sanitization."""
        key = StreamKeyManager.build_stream_key("user@123", "chat/001")
        assert key == "agent_c:stream:user_123:chat_001"
    
    def test_parse_stream_key(self):
        """Test parsing valid stream keys."""
        session_id, interaction_id = StreamKeyManager.parse_stream_key("agent_c:stream:user_123:chat_001")
        assert session_id == "user_123"
        assert interaction_id == "chat_001"
    
    def test_parse_stream_key_invalid(self):
        """Test parsing invalid stream keys."""
        with pytest.raises(ValueError):
            StreamKeyManager.parse_stream_key("invalid_key")
        
        with pytest.raises(ValueError):
            StreamKeyManager.parse_stream_key("agent_c:stream:user_123")
    
    def test_sanitize_id(self):
        """Test ID sanitization."""
        assert StreamKeyManager._sanitize_id("user@123") == "user_123"
        assert StreamKeyManager._sanitize_id("user/123") == "user_123"
        assert StreamKeyManager._sanitize_id("user-123_abc") == "user-123_abc"
    
    def test_sanitize_id_empty(self):
        """Test ID sanitization with empty input."""
        with pytest.raises(ValueError):
            StreamKeyManager._sanitize_id("")
        
        with pytest.raises(ValueError):
            StreamKeyManager._sanitize_id("@@@")
    
    def test_is_valid_stream_key(self):
        """Test stream key validation."""
        assert StreamKeyManager.is_valid_stream_key("agent_c:stream:user_123:chat_001")
        assert not StreamKeyManager.is_valid_stream_key("invalid_key")
    
    def test_get_session_pattern(self):
        """Test session pattern generation."""
        pattern = StreamKeyManager.get_session_pattern("user_123")
        assert pattern == "agent_c:stream:user_123:*"
    
    def test_get_all_streams_pattern(self):
        """Test all streams pattern generation."""
        pattern = StreamKeyManager.get_all_streams_pattern()
        assert pattern == "agent_c:stream:*"


class TestEventSerializer:
    """Test cases for EventSerializer."""
    
    def test_serialize_event(self):
        """Test event serialization."""
        event = TextDeltaEvent(content="Hello world")
        context = EventContext(
            session_id="user_123",
            interaction_id="chat_001",
            user_id="user_456"
        )
        
        serialized = EventSerializer.serialize_event(event, context)
        
        assert serialized["event_type"] == "text_delta"
        assert serialized["session_id"] == "user_123"
        assert serialized["interaction_id"] == "chat_001"
        assert serialized["user_id"] == "user_456"
        assert serialized["event_version"] == "1.0"
        
        # Check that event_data is valid JSON
        event_data = json.loads(serialized["event_data"])
        assert event_data["type"] == "text_delta"
    
    def test_deserialize_event(self):
        """Test event deserialization."""
        # Create a serialized message
        stream_message = {
            "event_type": "text_delta",
            "event_id": "test-id",
            "event_data": json.dumps({
                "type": "text_delta",
                "data": {"content": "Hello world"},
                "metadata": {}
            }),
            "event_version": "1.0",
            "session_id": "user_123",
            "interaction_id": "chat_001",
            "user_id": "user_456",
            "sequence": "1",
            "timestamp": "2024-01-01T00:00:00",
            "processing_time": "2024-01-01T00:00:01",
            "source": "BaseAgent",
            "trace_id": "trace-123",
            "correlation_id": "corr-456"
        }
        
        event_data, context = EventSerializer.deserialize_event(stream_message)
        
        assert event_data["type"] == "text_delta"
        assert context.session_id == "user_123"
        assert context.interaction_id == "chat_001"
        assert context.user_id == "user_456"
        assert context.sequence == 1
    
    def test_create_stream_metadata_event(self):
        """Test stream metadata event creation."""
        metadata_event = EventSerializer.create_stream_metadata_event(
            "stream_created",
            "user_123",
            "chat_001",
            custom_field="test_value"
        )
        
        assert metadata_event["event_type"] == "stream_created"
        assert metadata_event["session_id"] == "user_123"
        assert metadata_event["interaction_id"] == "chat_001"
        assert metadata_event["metadata"] == "true"
        assert metadata_event["custom_field"] == "test_value"


class TestFallbackEventQueue:
    """Test cases for FallbackEventQueue."""
    
    @pytest.mark.asyncio
    async def test_fallback_queue_operations(self):
        """Test basic fallback queue operations."""
        queue = FallbackEventQueue(max_size=2)
        
        # Test putting events
        await queue.put({"event": "test1"})
        await queue.put({"event": "test2"})
        
        assert queue.size == 2
        
        # Test getting events
        event1 = await queue.get()
        assert event1["event"] == "test1"
        
        event2 = await queue.get()
        assert event2["event"] == "test2"
    
    @pytest.mark.asyncio
    async def test_fallback_queue_overflow(self):
        """Test fallback queue overflow handling."""
        queue = FallbackEventQueue(max_size=1)
        
        # Fill the queue
        await queue.put({"event": "test1"})
        
        # This should not raise an error but should increment dropped counter
        await queue.put({"event": "test2"})
        
        assert queue._dropped_events == 1
    
    def test_fallback_activation(self):
        """Test fallback activation/deactivation."""
        queue = FallbackEventQueue()
        
        assert not queue.is_active
        
        queue.activate()
        assert queue.is_active
        
        queue.deactivate()
        assert not queue.is_active


class TestRedisStreamManager:
    """Test cases for RedisStreamManager."""
    
    @pytest.fixture
    def redis_config(self):
        """Create a test Redis configuration."""
        config = RedisConfig()
        config.enabled = True
        return config
    
    @pytest.fixture
    def stream_manager(self, redis_config):
        """Create a test Redis stream manager."""
        return RedisStreamManager(redis_config)
    
    def test_redis_stream_manager_init(self, stream_manager):
        """Test Redis stream manager initialization."""
        assert stream_manager.config.enabled
        assert not stream_manager.is_healthy
        assert stream_manager.is_fallback_active
    
    def test_redis_stream_manager_disabled(self):
        """Test Redis stream manager with disabled config."""
        config = RedisConfig()
        config.enabled = False
        
        manager = RedisStreamManager(config)
        assert not manager.config.enabled
        assert manager.is_fallback_active
    
    @pytest.mark.asyncio
    async def test_publish_event_fallback(self, stream_manager):
        """Test event publishing when Redis is unavailable (fallback mode)."""
        # Since Redis is not available in tests, it should use fallback
        result = await stream_manager.publish_event(
            event_type="text_delta",
            event_data={"content": "test"},
            session_id="user_123",
            interaction_id="chat_001"
        )
        
        # Should return None when using fallback
        assert result is None
        assert stream_manager.is_fallback_active
    
    @pytest.mark.asyncio
    async def test_create_stream_fallback(self, stream_manager):
        """Test stream creation when Redis is unavailable."""
        stream_key = await stream_manager.create_stream("user_123", "chat_001")
        
        expected_key = "agent_c:stream:user_123:chat_001"
        assert stream_key == expected_key
    
    @pytest.mark.asyncio
    async def test_cleanup_method_exists(self, stream_manager):
        """Test that cleanup method exists and doesn't crash."""
        # Should not raise an error even when Redis is unavailable
        await stream_manager.cleanup_old_streams()
    
    @pytest.mark.asyncio
    async def test_close_method(self, stream_manager):
        """Test that close method works correctly."""
        await stream_manager.close()
        # Should complete without error


@pytest.mark.integration
class TestRedisStreamManagerIntegration:
    """Integration tests that require a real Redis instance."""
    
    pytest_plugins = ['pytest_asyncio']
    
    @pytest.mark.asyncio
    async def test_redis_integration_if_available(self):
        """
        Test Redis integration if Redis is available.
        This test is marked as integration and will be skipped in unit test runs.
        """
        try:
            import redis.asyncio as redis
            
            # Try to connect to Redis
            client = redis.Redis(host='localhost', port=6379, decode_responses=True)
            await client.ping()
            
            # If we get here, Redis is available
            config = RedisConfig()
            config.enabled = True
            config.host = 'localhost'
            config.port = 6379
            
            manager = RedisStreamManager(config)
            await manager.initialize()
            
            if manager.is_healthy:
                # Test actual Redis operations
                stream_key = await manager.create_stream("test_session", "test_interaction")
                
                result = await manager.publish_event(
                    event_type="test_event",
                    event_data={"test": "data"},
                    session_id="test_session",
                    interaction_id="test_interaction"
                )
                
                assert result is not None  # Should get a message ID
                
                # Clean up
                await manager.close()
                await client.delete(stream_key)
                await client.close()
            
        except (ImportError, redis.ConnectionError, redis.ResponseError):
            pytest.skip("Redis not available for integration testing")


if __name__ == "__main__":
    # Run the tests
    pytest.main([__file__, "-v"])
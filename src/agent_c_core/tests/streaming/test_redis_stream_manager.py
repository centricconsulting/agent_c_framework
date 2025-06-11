"""
Tests for Redis Stream Manager

This module contains comprehensive tests for the Redis Streams integration,
including unit tests and integration tests with mock Redis.
"""

import pytest
import asyncio
import json
import os
import tempfile
from unittest.mock import Mock, AsyncMock, patch
from datetime import datetime

from agent_c.streaming.redis_stream_manager import (
    RedisStreamManager, 
    RedisConfig, 
    FallbackEventQueue,
    OperationMode,
    FailoverStrategy
)
from agent_c.streaming.stream_key_manager import StreamKeyManager
from agent_c.streaming.event_serializer import EventSerializer, EventContext
from agent_c.models.events import TextDeltaEvent


class TestRedisConfig:
    """Test cases for enhanced RedisConfig class."""
    
    def test_default_configuration(self):
        """Test default configuration values."""
        config = RedisConfig()
        
        # Test default values
        assert config.host == 'localhost'
        assert config.port == 6379
        assert config.password is None
        assert config.db == 0
        assert config.ssl is False
        
        # Test operation mode and failover strategy
        assert config.operation_mode == OperationMode.HYBRID
        assert config.failover_strategy == FailoverStrategy.AUTO_FAILOVER
        
        # Test core configuration
        assert config.health_check_interval == 30
        assert config.connection_timeout == 5
        assert config.max_retry_attempts == 3
        assert config.circuit_breaker_threshold == 5
        assert config.recovery_interval == 60
        
        # Test performance configuration
        assert config.batch_size == 100
        assert config.buffer_size == 10000
        assert config.stream_max_len == 1000
        assert config.stream_trim_interval == 300
        
        # Test feature flags
        assert config.enabled is False
        assert config.fallback_enabled is True
    
    def test_constructor_parameter_override(self):
        """Test configuration override via constructor parameters."""
        config = RedisConfig(
            host='redis.example.com',
            port=6380,
            operation_mode='redis_only',
            failover_strategy='no_failover',
            batch_size=200,
            enabled=True
        )
        
        assert config.host == 'redis.example.com'
        assert config.port == 6380
        assert config.operation_mode == OperationMode.REDIS_ONLY
        assert config.failover_strategy == FailoverStrategy.NO_FAILOVER
        assert config.batch_size == 200
        assert config.enabled is True
    
    @patch.dict(os.environ, {
        'REDIS_HOST': 'env.redis.com',
        'REDIS_PORT': '6381',
        'REDIS_PASSWORD': 'secret123',
        'REDIS_OPERATION_MODE': 'async_only',
        'REDIS_FAILOVER_STRATEGY': 'auto_failover',
        'REDIS_BATCH_SIZE': '150',
        'ENABLE_REDIS_STREAMS': 'true'
    })
    def test_environment_variable_loading(self):
        """Test configuration loading from environment variables."""
        config = RedisConfig()
        
        assert config.host == 'env.redis.com'
        assert config.port == 6381
        assert config.password == 'secret123'
        assert config.operation_mode == OperationMode.ASYNC_ONLY
        assert config.failover_strategy == FailoverStrategy.AUTO_FAILOVER
        assert config.batch_size == 150
        assert config.enabled is True
    
    @patch.dict(os.environ, {
        'REDIS_HOST': 'env.redis.com',
        'REDIS_PORT': '6381'
    })
    def test_priority_order(self):
        """Test that constructor parameters override environment variables."""
        config = RedisConfig(
            host='constructor.redis.com',
            port=9999
        )
        
        # Constructor should override environment
        assert config.host == 'constructor.redis.com'
        assert config.port == 9999
    
    def test_config_file_loading_json(self):
        """Test configuration loading from JSON file."""
        config_data = {
            'host': 'file.redis.com',
            'port': 6382,
            'operation_mode': 'redis_only',
            'batch_size': 250
        }
        
        with tempfile.NamedTemporaryFile(mode='w', suffix='.json', delete=False) as f:
            json.dump(config_data, f)
            config_file = f.name
        
        try:
            config = RedisConfig(config_file=config_file)
            
            assert config.host == 'file.redis.com'
            assert config.port == 6382
            assert config.operation_mode == OperationMode.REDIS_ONLY
            assert config.batch_size == 250
        finally:
            os.unlink(config_file)
    
    def test_config_file_loading_yaml(self):
        """Test configuration loading from YAML file."""
        config_data = """
        host: yaml.redis.com
        port: 6383
        operation_mode: async_only
        failover_strategy: no_failover
        """
        
        with tempfile.NamedTemporaryFile(mode='w', suffix='.yaml', delete=False) as f:
            f.write(config_data)
            config_file = f.name
        
        try:
            config = RedisConfig(config_file=config_file)
            
            assert config.host == 'yaml.redis.com'
            assert config.port == 6383
            assert config.operation_mode == OperationMode.ASYNC_ONLY
            assert config.failover_strategy == FailoverStrategy.NO_FAILOVER
        finally:
            os.unlink(config_file)
    
    def test_config_file_with_redis_section(self):
        """Test configuration file with redis section."""
        config_data = {
            'database': {'host': 'db.example.com'},
            'redis': {
                'host': 'section.redis.com',
                'port': 6384,
                'operation_mode': 'hybrid'
            }
        }
        
        with tempfile.NamedTemporaryFile(mode='w', suffix='.json', delete=False) as f:
            json.dump(config_data, f)
            config_file = f.name
        
        try:
            config = RedisConfig(config_file=config_file)
            
            # Should use values from redis section
            assert config.host == 'section.redis.com'
            assert config.port == 6384
            assert config.operation_mode == OperationMode.HYBRID
        finally:
            os.unlink(config_file)
    
    def test_is_redis_primary(self):
        """Test is_redis_primary helper method."""
        # REDIS_ONLY should be primary
        config = RedisConfig(operation_mode='redis_only')
        assert config.is_redis_primary() is True
        
        # HYBRID should be primary
        config = RedisConfig(operation_mode='hybrid')
        assert config.is_redis_primary() is True
        
        # ASYNC_ONLY should not be primary
        config = RedisConfig(operation_mode='async_only')
        assert config.is_redis_primary() is False
    
    def test_is_failover_enabled(self):
        """Test is_failover_enabled helper method."""
        # AUTO_FAILOVER should be enabled
        config = RedisConfig(failover_strategy='auto_failover')
        assert config.is_failover_enabled() is True
        
        # NO_FAILOVER should not be enabled
        config = RedisConfig(failover_strategy='no_failover')
        assert config.is_failover_enabled() is False
    
    def test_get_effective_mode_redis_healthy(self):
        """Test get_effective_mode when Redis is healthy."""
        # When Redis is healthy, should return configured mode
        config = RedisConfig(operation_mode='redis_only', enabled=True)
        assert config.get_effective_mode(redis_healthy=True) == OperationMode.REDIS_ONLY
        
        config = RedisConfig(operation_mode='hybrid', enabled=True)
        assert config.get_effective_mode(redis_healthy=True) == OperationMode.HYBRID
        
        config = RedisConfig(operation_mode='async_only', enabled=True)
        assert config.get_effective_mode(redis_healthy=True) == OperationMode.ASYNC_ONLY
    
    def test_get_effective_mode_redis_unhealthy_with_failover(self):
        """Test get_effective_mode when Redis is unhealthy with failover enabled."""
        config = RedisConfig(
            operation_mode='redis_only',
            failover_strategy='auto_failover',
            enabled=True
        )
        
        # Should failover to ASYNC_ONLY
        assert config.get_effective_mode(redis_healthy=False) == OperationMode.ASYNC_ONLY
    
    def test_get_effective_mode_redis_unhealthy_no_failover(self):
        """Test get_effective_mode when Redis is unhealthy with no failover."""
        config = RedisConfig(
            operation_mode='redis_only',
            failover_strategy='no_failover',
            enabled=True
        )
        
        # Should maintain original mode even if Redis is unhealthy
        assert config.get_effective_mode(redis_healthy=False) == OperationMode.REDIS_ONLY
    
    def test_get_effective_mode_disabled(self):
        """Test get_effective_mode when Redis is disabled."""
        config = RedisConfig(
            operation_mode='redis_only',
            enabled=False
        )
        
        # Should always return ASYNC_ONLY when disabled
        assert config.get_effective_mode(redis_healthy=True) == OperationMode.ASYNC_ONLY
        assert config.get_effective_mode(redis_healthy=False) == OperationMode.ASYNC_ONLY
    
    def test_validation_success(self):
        """Test successful configuration validation."""
        config = RedisConfig(
            host='redis.example.com',
            port=6379,
            health_check_interval=30,
            connection_timeout=5,
            batch_size=100
        )
        
        # Should not raise any exception
        config.validate()
    
    def test_validation_invalid_host(self):
        """Test validation with invalid host."""
        config = RedisConfig(host='')
        
        with pytest.raises(ValueError, match="host must be a non-empty string"):
            config.validate()
    
    def test_validation_invalid_port(self):
        """Test validation with invalid port."""
        config = RedisConfig(port=70000)
        
        with pytest.raises(ValueError, match="port must be an integer between 1 and 65535"):
            config.validate()
    
    def test_validation_negative_values(self):
        """Test validation with negative values."""
        config = RedisConfig(health_check_interval=-1)
        
        with pytest.raises(ValueError, match="health_check_interval must be positive"):
            config.validate()
    
    def test_validation_redis_only_mode_disabled(self):
        """Test validation when REDIS_ONLY mode is used but Redis is disabled."""
        config = RedisConfig(
            operation_mode='redis_only',
            enabled=False
        )
        
        with pytest.raises(ValueError, match="Cannot use REDIS_ONLY mode when Redis Streams is disabled"):
            config.validate()
    
    def test_invalid_operation_mode(self):
        """Test handling of invalid operation mode."""
        with patch('agent_c.streaming.redis_stream_manager.logger') as mock_logger:
            config = RedisConfig(operation_mode='invalid_mode')
            
            # Should default to HYBRID and log warning
            assert config.operation_mode == OperationMode.HYBRID
            mock_logger.warning.assert_called_once()
    
    def test_invalid_failover_strategy(self):
        """Test handling of invalid failover strategy."""
        with patch('agent_c.streaming.redis_stream_manager.logger') as mock_logger:
            config = RedisConfig(failover_strategy='invalid_strategy')
            
            # Should default to AUTO_FAILOVER and log warning
            assert config.failover_strategy == FailoverStrategy.AUTO_FAILOVER
            mock_logger.warning.assert_called_once()
    
    def test_to_dict(self):
        """Test configuration serialization to dictionary."""
        config = RedisConfig(
            host='test.redis.com',
            port=6379,
            operation_mode='redis_only',
            failover_strategy='no_failover',
            batch_size=200
        )
        
        config_dict = config.to_dict()
        
        assert config_dict['host'] == 'test.redis.com'
        assert config_dict['port'] == 6379
        assert config_dict['operation_mode'] == 'redis_only'
        assert config_dict['failover_strategy'] == 'no_failover'
        assert config_dict['batch_size'] == 200
        
        # Ensure all expected keys are present
        expected_keys = {
            'host', 'port', 'password', 'db', 'ssl',
            'operation_mode', 'failover_strategy', 'health_check_interval',
            'connection_timeout', 'max_retry_attempts', 'circuit_breaker_threshold',
            'recovery_interval', 'batch_size', 'buffer_size', 'stream_max_len',
            'stream_trim_interval', 'stream_max_length', 'stream_ttl',
            'read_timeout', 'max_connections', 'socket_timeout',
            'socket_connect_timeout', 'enabled', 'fallback_enabled'
        }
        assert set(config_dict.keys()) == expected_keys
    
    def test_repr(self):
        """Test string representation of configuration."""
        config = RedisConfig(
            host='test.redis.com',
            port=6379,
            operation_mode='redis_only',
            failover_strategy='no_failover',
            enabled=True
        )
        
        repr_str = repr(config)
        
        assert 'RedisConfig(' in repr_str
        assert 'host=test.redis.com' in repr_str
        assert 'port=6379' in repr_str
        assert 'mode=redis_only' in repr_str
        assert 'failover=no_failover' in repr_str
        assert 'enabled=True' in repr_str
    
    def test_legacy_compatibility(self):
        """Test backward compatibility with legacy configuration."""
        # Test that legacy properties still work
        config = RedisConfig()
        
        # These should be available for backward compatibility
        assert hasattr(config, 'stream_max_length')
        assert hasattr(config, 'stream_ttl')
        assert hasattr(config, 'read_timeout')
        assert hasattr(config, 'max_connections')
        assert hasattr(config, 'socket_timeout')
        assert hasattr(config, 'socket_connect_timeout')
        assert hasattr(config, 'enabled')
        assert hasattr(config, 'fallback_enabled')
        
        # stream_max_length should be an alias for stream_max_len
        assert config.stream_max_length == config.stream_max_len
    
    @patch.dict(os.environ, {
        'REDIS_STREAM_MAX_LENGTH': '2000'  # Legacy env var
    })
    def test_legacy_env_var_support(self):
        """Test support for legacy environment variables."""
        config = RedisConfig()
        
        # Legacy env var should still work
        assert config.stream_max_length == 2000


class TestOperationModeEnum:
    """Test cases for OperationMode enum."""
    
    def test_operation_mode_values(self):
        """Test operation mode enum values."""
        assert OperationMode.REDIS_ONLY.value == "redis_only"
        assert OperationMode.HYBRID.value == "hybrid"
        assert OperationMode.ASYNC_ONLY.value == "async_only"
    
    def test_operation_mode_creation(self):
        """Test creating operation modes from string values."""
        assert OperationMode("redis_only") == OperationMode.REDIS_ONLY
        assert OperationMode("hybrid") == OperationMode.HYBRID
        assert OperationMode("async_only") == OperationMode.ASYNC_ONLY
        
        with pytest.raises(ValueError):
            OperationMode("invalid_mode")


class TestFailoverStrategyEnum:
    """Test cases for FailoverStrategy enum."""
    
    def test_failover_strategy_values(self):
        """Test failover strategy enum values."""
        assert FailoverStrategy.AUTO_FAILOVER.value == "auto_failover"
        assert FailoverStrategy.NO_FAILOVER.value == "no_failover"
    
    def test_failover_strategy_creation(self):
        """Test creating failover strategies from string values."""
        assert FailoverStrategy("auto_failover") == FailoverStrategy.AUTO_FAILOVER
        assert FailoverStrategy("no_failover") == FailoverStrategy.NO_FAILOVER
        
        with pytest.raises(ValueError):
            FailoverStrategy("invalid_strategy")


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
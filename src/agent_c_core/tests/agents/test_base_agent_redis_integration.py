"""
Tests for BaseAgent Redis Streams Integration

This module tests the integration of Redis Streams with the BaseAgent class,
ensuring that the _raise_event method and related functionality work correctly.
"""

import pytest
import asyncio
from unittest.mock import Mock, AsyncMock, patch

from agent_c.agents.base import BaseAgent
from agent_c.streaming.redis_stream_manager import RedisStreamManager, RedisConfig
from agent_c.models.events import TextDeltaEvent, SystemMessageEvent


class TestBaseAgent(BaseAgent):
    """Test implementation of BaseAgent for testing purposes."""
    
    def __init__(self, **kwargs):
        super().__init__(**kwargs)
        self.model_name = "test-model"
    
    @classmethod
    def client(cls, **opts):
        return Mock()
    
    @property
    def tool_format(self) -> str:
        return "test"
    
    def count_tokens(self, text: str) -> int:
        return len(text.split())
    
    async def one_shot(self, **kwargs):
        return [{"role": "assistant", "content": "test response"}]
    
    def chat_sync(self, **kwargs):
        return [{"role": "assistant", "content": "test response"}]
    
    async def parallel_one_shots(self, inputs, **kwargs):
        return [await self.one_shot(user_message=input_text, **kwargs) for input_text in inputs]
    
    async def chat(self, **kwargs):
        return [{"role": "assistant", "content": "test response"}]


class TestBaseAgentRedisIntegration:
    """Test cases for BaseAgent Redis Streams integration."""
    
    @pytest.fixture
    def base_agent(self):
        """Create a test BaseAgent instance."""
        return TestBaseAgent()
    
    @pytest.fixture
    def mock_redis_manager(self):
        """Create a mock Redis stream manager."""
        manager = Mock(spec=RedisStreamManager)
        manager.publish_event = AsyncMock(return_value="test-message-id")
        manager.is_healthy = True
        return manager
    
    def test_base_agent_initialization(self, base_agent):
        """Test that BaseAgent initializes with Redis attributes."""
        assert hasattr(base_agent, 'redis_stream_manager')
        assert hasattr(base_agent, '_current_stream_id')
        assert base_agent.redis_stream_manager is None
        assert base_agent._current_stream_id is None
    
    def test_set_redis_stream_manager(self, base_agent, mock_redis_manager):
        """Test setting Redis stream manager."""
        base_agent.set_redis_stream_manager(mock_redis_manager)
        assert base_agent.redis_stream_manager == mock_redis_manager
    
    def test_set_current_stream_id(self, base_agent):
        """Test setting current stream ID."""
        base_agent.set_current_stream_id("user_123", "chat_001")
        assert base_agent._current_stream_id == "user_123:chat_001"
    
    def test_parse_stream_id(self, base_agent):
        """Test stream ID parsing."""
        session_id, interaction_id = base_agent._parse_stream_id("user_123:chat_001")
        assert session_id == "user_123"
        assert interaction_id == "chat_001"
        
        # Test fallback for malformed stream ID
        session_id, interaction_id = base_agent._parse_stream_id("user_123")
        assert session_id == "user_123"
        assert interaction_id == "default"
    
    def test_get_current_stream_id(self, base_agent):
        """Test getting current stream ID."""
        assert base_agent._get_current_stream_id() is None
        
        base_agent.set_current_stream_id("user_123", "chat_001")
        assert base_agent._get_current_stream_id() == "user_123:chat_001"
    
    def test_should_use_redis_streams(self, base_agent, mock_redis_manager):
        """Test Redis Streams usage check."""
        # Initially should not use Redis Streams
        assert not base_agent._should_use_redis_streams()
        
        # Set manager but no stream ID
        base_agent.set_redis_stream_manager(mock_redis_manager)
        assert not base_agent._should_use_redis_streams()
        
        # Set stream ID but manager is not healthy
        base_agent.set_current_stream_id("user_123", "chat_001")
        mock_redis_manager.is_healthy = False
        assert not base_agent._should_use_redis_streams()
        
        # Both manager and stream ID set, and healthy
        mock_redis_manager.is_healthy = True
        assert base_agent._should_use_redis_streams()
    
    @pytest.mark.asyncio
    async def test_raise_event_with_redis_streams(self, base_agent, mock_redis_manager):
        """Test _raise_event with Redis Streams configured."""
        # Setup
        base_agent.set_redis_stream_manager(mock_redis_manager)
        base_agent.set_current_stream_id("user_123", "chat_001")
        
        # Mock streaming callback
        mock_callback = AsyncMock()
        base_agent.streaming_callback = mock_callback
        
        # Create test event
        event = TextDeltaEvent(content="Hello world")
        
        # Call _raise_event
        await base_agent._raise_event(event)
        
        # Verify Redis Stream publish was called
        mock_redis_manager.publish_event.assert_called_once()
        call_args = mock_redis_manager.publish_event.call_args
        assert call_args[1]['event_type'] == 'text_delta'
        assert call_args[1]['session_id'] == 'user_123'
        assert call_args[1]['interaction_id'] == 'chat_001'
        assert call_args[1]['source'] == 'BaseAgent'
        
        # Verify callback was also called (backward compatibility)
        mock_callback.assert_called_once_with(event)
    
    @pytest.mark.asyncio
    async def test_raise_event_redis_error_fallback(self, base_agent, mock_redis_manager):
        """Test _raise_event fallback when Redis fails."""
        # Setup
        base_agent.set_redis_stream_manager(mock_redis_manager)
        base_agent.set_current_stream_id("user_123", "chat_001")
        
        # Make Redis publish fail
        mock_redis_manager.publish_event.side_effect = Exception("Redis connection error")
        
        # Mock streaming callback
        mock_callback = AsyncMock()
        base_agent.streaming_callback = mock_callback
        
        # Create test event
        event = TextDeltaEvent(content="Hello world")
        
        # Call _raise_event - should not raise exception
        await base_agent._raise_event(event)
        
        # Verify callback was still called (fallback behavior)
        mock_callback.assert_called_once_with(event)
    
    @pytest.mark.asyncio
    async def test_raise_event_without_redis(self, base_agent):
        """Test _raise_event without Redis configured (backward compatibility)."""
        # Mock streaming callback
        mock_callback = AsyncMock()
        base_agent.streaming_callback = mock_callback
        
        # Create test event
        event = TextDeltaEvent(content="Hello world")
        
        # Call _raise_event
        await base_agent._raise_event(event)
        
        # Verify only callback was called
        mock_callback.assert_called_once_with(event)
    
    @pytest.mark.asyncio
    async def test_raise_event_with_explicit_stream_id(self, base_agent, mock_redis_manager):
        """Test _raise_event with explicit stream_id parameter."""
        # Setup
        base_agent.set_redis_stream_manager(mock_redis_manager)
        
        # Mock streaming callback
        mock_callback = AsyncMock()
        
        # Create test event
        event = TextDeltaEvent(content="Hello world")
        
        # Call _raise_event with explicit stream_id
        await base_agent._raise_event(
            event, 
            streaming_callback=mock_callback,
            stream_id="explicit_session:explicit_interaction"
        )
        
        # Verify Redis Stream publish was called with explicit IDs
        mock_redis_manager.publish_event.assert_called_once()
        call_args = mock_redis_manager.publish_event.call_args
        assert call_args[1]['session_id'] == 'explicit_session'
        assert call_args[1]['interaction_id'] == 'explicit_interaction'
        
        # Verify callback was called
        mock_callback.assert_called_once_with(event)
    
    @pytest.mark.asyncio
    async def test_raise_text_delta_with_stream_id(self, base_agent, mock_redis_manager):
        """Test _raise_text_delta method with Redis Streams."""
        # Setup
        base_agent.set_redis_stream_manager(mock_redis_manager)
        base_agent.set_current_stream_id("user_123", "chat_001")
        
        # Mock streaming callback
        mock_callback = AsyncMock()
        base_agent.streaming_callback = mock_callback
        
        # Call _raise_text_delta
        await base_agent._raise_text_delta("Hello world")
        
        # Verify Redis Stream publish was called
        mock_redis_manager.publish_event.assert_called_once()
        call_args = mock_redis_manager.publish_event.call_args
        assert call_args[1]['event_type'] == 'text_delta'
        
        # Verify callback was called
        mock_callback.assert_called_once()
    
    @pytest.mark.asyncio
    async def test_raise_system_event_with_stream_id(self, base_agent, mock_redis_manager):
        """Test _raise_system_event method with Redis Streams."""
        # Setup
        base_agent.set_redis_stream_manager(mock_redis_manager)
        base_agent.set_current_stream_id("user_123", "chat_001")
        
        # Mock streaming callback
        mock_callback = AsyncMock()
        base_agent.streaming_callback = mock_callback
        
        # Call _raise_system_event
        await base_agent._raise_system_event("System message", severity="info")
        
        # Verify Redis Stream publish was called
        mock_redis_manager.publish_event.assert_called_once()
        call_args = mock_redis_manager.publish_event.call_args
        assert call_args[1]['event_type'] == 'system_message'
        
        # Verify callback was called
        mock_callback.assert_called_once()
    
    @pytest.mark.asyncio
    async def test_all_raise_methods_have_stream_id_support(self, base_agent):
        """Test that all _raise_* methods support stream_id parameter."""
        import inspect
        
        # Get all methods that start with _raise_
        raise_methods = [
            method for method in dir(base_agent) 
            if method.startswith('_raise_') and callable(getattr(base_agent, method))
        ]
        
        # Exclude the base _raise_event method and private helper methods
        raise_methods = [
            method for method in raise_methods 
            if method not in ['_raise_event', '_log_internal_error']
        ]
        
        # Check that each method has stream_id parameter
        for method_name in raise_methods:
            method = getattr(base_agent, method_name)
            sig = inspect.signature(method)
            
            # Check if stream_id parameter exists
            assert 'stream_id' in sig.parameters, f"Method {method_name} missing stream_id parameter"
            
            # Check that stream_id has Optional[str] annotation and default None
            stream_id_param = sig.parameters['stream_id']
            assert stream_id_param.default is None, f"Method {method_name} stream_id should default to None"


@pytest.mark.asyncio
async def test_integration_example():
    """
    Integration example showing how Redis Streams would be used with BaseAgent.
    """
    # Create agent
    agent = TestBaseAgent()
    
    # Create mock Redis manager
    redis_manager = Mock(spec=RedisStreamManager)
    redis_manager.publish_event = AsyncMock(return_value="msg-123")
    redis_manager.is_healthy = True
    
    # Configure agent for Redis Streams
    agent.set_redis_stream_manager(redis_manager)
    agent.set_current_stream_id("user_session_123", "chat_interaction_456")
    
    # Mock callback for backward compatibility
    callback_mock = AsyncMock()
    agent.streaming_callback = callback_mock
    
    # Raise various events
    await agent._raise_interaction_start()
    await agent._raise_text_delta("Hello")
    await agent._raise_text_delta(" world!")
    await agent._raise_interaction_end()
    
    # Verify Redis calls
    assert redis_manager.publish_event.call_count == 4
    
    # Verify callbacks still work
    assert callback_mock.call_count == 4
    
    # Verify the calls have correct session/interaction IDs
    for call in redis_manager.publish_event.call_args_list:
        kwargs = call[1]
        assert kwargs['session_id'] == 'user_session_123'
        assert kwargs['interaction_id'] == 'chat_interaction_456'
        assert kwargs['source'] == 'BaseAgent'


if __name__ == "__main__":
    # Run the tests
    pytest.main([__file__, "-v"])
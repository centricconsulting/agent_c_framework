"""
Test suite for interaction tracking integration in BaseAgent.

This module tests the interaction ID integration and agent runtime updates
to ensure proper interaction tracking, event context, and work log generation.
"""

import pytest
import asyncio
import uuid
from unittest.mock import Mock, AsyncMock, patch
from typing import List, Dict, Any, Optional

from agent_c.agents.base import BaseAgent
from agent_c.models.chat_event import ChatEvent
from agent_c.models.events import InteractionEvent, TextDeltaEvent, ToolCallEvent
from agent_c.models.chat_history.interaction_container import InteractionContainer
from agent_c.models.common_chat.enhanced_models import EnhancedCommonChatMessage
from agent_c.toolsets.tool_chest import ToolChest


class MockAgent(BaseAgent):
    """Mock agent for testing interaction tracking."""
    
    @classmethod
    def client(cls, **opts):
        return Mock()
    
    @property
    def tool_format(self) -> str:
        return "claude"
    
    async def chat(self, **kwargs) -> List[dict[str, Any]]:
        """Mock chat implementation for testing."""
        # Start interaction
        interaction_id = await self._raise_interaction_start(**kwargs)
        
        # Simulate user message processing
        user_message = kwargs.get('user_message', 'test message')
        await self._raise_user_request(user_message, **kwargs)
        
        # Simulate text generation
        await self._raise_text_delta('Hello', **kwargs)
        await self._raise_text_delta(' world!', **kwargs)
        
        # End interaction
        await self._raise_interaction_end(**kwargs)
        
        return [{'role': 'assistant', 'content': 'Hello world!'}]


class TestInteractionTracking:
    """Test interaction tracking functionality."""
    
    @pytest.fixture
    def mock_streaming_callback(self):
        """Mock streaming callback for event capture."""
        return AsyncMock()
    
    @pytest.fixture
    def agent(self, mock_streaming_callback):
        """Create a mock agent with interaction tracking enabled."""
        return MockAgent(
            model_name="test-model",
            streaming_callback=mock_streaming_callback,
            enable_interaction_tracking=True
        )
    
    @pytest.fixture
    def agent_disabled(self, mock_streaming_callback):
        """Create a mock agent with interaction tracking disabled."""
        return MockAgent(
            model_name="test-model", 
            streaming_callback=mock_streaming_callback,
            enable_interaction_tracking=False
        )
    
    def test_agent_initialization(self, agent):
        """Test that agent initializes with interaction tracking components."""
        assert agent.enable_interaction_tracking is True
        assert agent._current_interaction_id is None
        assert agent._current_interaction_container is None
        assert isinstance(agent._interaction_containers, dict)
        assert len(agent._interaction_containers) == 0
    
    def test_agent_initialization_disabled(self, agent_disabled):
        """Test that agent can be initialized with interaction tracking disabled."""
        assert agent_disabled.enable_interaction_tracking is False
    
    def test_generate_interaction_id(self, agent):
        """Test interaction ID generation."""
        interaction_id = agent._generate_interaction_id()
        assert isinstance(interaction_id, str)
        assert len(interaction_id) > 0
        
        # Should generate unique IDs
        interaction_id2 = agent._generate_interaction_id()
        assert interaction_id != interaction_id2
    
    def test_start_interaction(self, agent):
        """Test starting a new interaction."""
        interaction_id = agent._start_interaction()
        
        assert interaction_id is not None
        assert agent._current_interaction_id == interaction_id
        assert agent._current_interaction_container is not None
        assert isinstance(agent._current_interaction_container, InteractionContainer)
        assert interaction_id in agent._interaction_containers
    
    def test_start_interaction_with_custom_id(self, agent):
        """Test starting interaction with custom ID."""
        custom_id = "custom-interaction-id"
        interaction_id = agent._start_interaction(custom_id)
        
        assert interaction_id == custom_id
        assert agent._current_interaction_id == custom_id
        assert custom_id in agent._interaction_containers
    
    def test_start_interaction_disabled(self, agent_disabled):
        """Test starting interaction when tracking is disabled."""
        interaction_id = agent_disabled._start_interaction()
        assert interaction_id == "disabled"
        assert agent_disabled._current_interaction_id is None
    
    def test_end_interaction(self, agent):
        """Test ending an interaction."""
        interaction_id = agent._start_interaction()
        container = agent._current_interaction_container
        
        agent._end_interaction()
        
        assert agent._current_interaction_id is None
        assert agent._current_interaction_container is None
        assert container.is_completed() is True
    
    def test_end_specific_interaction(self, agent):
        """Test ending a specific interaction."""
        interaction_id1 = agent._start_interaction()
        interaction_id2 = agent._start_interaction()
        
        # End the first interaction specifically
        agent._end_interaction(interaction_id1)
        
        # Current interaction should still be the second one
        assert agent._current_interaction_id == interaction_id2
        
        # First interaction should be completed
        container1 = agent._interaction_containers[interaction_id1]
        assert container1.is_completed() is True
    
    def test_get_current_interaction_id(self, agent):
        """Test getting current interaction ID."""
        assert agent._get_current_interaction_id() is None
        
        interaction_id = agent._start_interaction()
        assert agent._get_current_interaction_id() == interaction_id
    
    def test_get_interaction_container(self, agent):
        """Test getting interaction container."""
        assert agent._get_interaction_container() is None
        
        interaction_id = agent._start_interaction()
        container = agent._get_interaction_container()
        
        assert container is not None
        assert isinstance(container, InteractionContainer)
        assert container.interaction_id == interaction_id
    
    def test_create_enhanced_message(self, agent):
        """Test creating enhanced messages with interaction tracking."""
        interaction_id = agent._start_interaction()
        
        message = agent._create_enhanced_message(
            role="user",
            content="test message"
        )
        
        assert isinstance(message, EnhancedCommonChatMessage)
        assert message.role == "user"
        assert message.interaction_id == interaction_id
        assert len(message.content) == 1
        assert message.content[0].text == "test message"
    
    def test_add_message_to_interaction(self, agent):
        """Test adding messages to interaction container."""
        interaction_id = agent._start_interaction()
        
        message = agent._create_enhanced_message(
            role="user",
            content="test message"
        )
        
        result = agent._add_message_to_interaction(message)
        assert result is True
        
        container = agent._get_interaction_container()
        messages = container.get_active_messages()
        assert len(messages) == 1
        assert messages[0].content[0].text == "test message"
    
    @pytest.mark.asyncio
    async def test_interaction_events(self, agent, mock_streaming_callback):
        """Test that interaction events include interaction context."""
        await agent.chat(user_message="test message")
        
        # Verify events were called
        assert mock_streaming_callback.call_count > 0
        
        # Check that interaction events were raised
        interaction_start_events = [
            call for call in mock_streaming_callback.call_args_list
            if isinstance(call[0][0], InteractionEvent) and call[0][0].started is True
        ]
        assert len(interaction_start_events) == 1
        
        interaction_end_events = [
            call for call in mock_streaming_callback.call_args_list
            if isinstance(call[0][0], InteractionEvent) and call[0][0].started is False
        ]
        assert len(interaction_end_events) == 1
        
        # Verify interaction ID is consistent
        start_event = interaction_start_events[0][0][0]
        end_event = interaction_end_events[0][0][0]
        assert start_event.id == end_event.id
    
    @pytest.mark.asyncio
    async def test_text_delta_events_have_interaction_context(self, agent, mock_streaming_callback):
        """Test that text delta events include interaction context."""
        await agent.chat(user_message="test message")
        
        # Find text delta events
        text_delta_events = [
            call for call in mock_streaming_callback.call_args_list
            if isinstance(call[0][0], TextDeltaEvent)
        ]
        
        assert len(text_delta_events) >= 2  # 'Hello' and ' world!'
        
        # All text delta events should have interaction_id
        for event_call in text_delta_events:
            event = event_call[0][0]
            assert hasattr(event, 'interaction_id')
            assert event.interaction_id is not None
    
    def test_tool_chest_integration(self, agent):
        """Test that tool chest gets registered with interaction containers."""
        mock_tool_chest = Mock(spec=ToolChest)
        agent.tool_chest = mock_tool_chest
        
        interaction_id = agent._start_interaction()
        
        # Tool chest should have been called to register the container
        mock_tool_chest.register_interaction_container.assert_called_once()
        call_args = mock_tool_chest.register_interaction_container.call_args
        assert call_args[0][0] == interaction_id
        assert isinstance(call_args[0][1], InteractionContainer)
    
    def test_tool_chest_unregistration(self, agent):
        """Test that tool chest unregisters interaction containers."""
        mock_tool_chest = Mock(spec=ToolChest)
        agent.tool_chest = mock_tool_chest
        
        interaction_id = agent._start_interaction()
        agent._end_interaction()
        
        # Tool chest should have been called to unregister the container
        mock_tool_chest.unregister_interaction_container.assert_called_once_with(interaction_id)
    
    @pytest.mark.asyncio
    async def test_message_construction_with_interaction_tracking(self, agent):
        """Test that message construction includes interaction tracking."""
        interaction_id = agent._start_interaction()
        
        messages = await agent._construct_message_array(
            user_message="test message",
            system_prompt="test prompt"
        )
        
        # Should have system and user messages
        assert len(messages) >= 2
        
        # Check that interaction context was added
        container = agent._get_interaction_container()
        tracked_messages = container.get_active_messages()
        
        # Should have at least the user message tracked
        assert len(tracked_messages) >= 1
        user_messages = [msg for msg in tracked_messages if msg.role == "user"]
        assert len(user_messages) >= 1
        assert user_messages[0].content[0].text == "test message"
    
    def test_thread_safety(self, agent):
        """Test that interaction management is thread-safe."""
        import threading
        import time
        
        results = []
        errors = []
        
        def start_interaction():
            try:
                interaction_id = agent._start_interaction()
                time.sleep(0.01)  # Small delay to encourage race conditions
                container = agent._get_interaction_container(interaction_id)
                results.append((interaction_id, container is not None))
                agent._end_interaction(interaction_id)
            except Exception as e:
                errors.append(e)
        
        # Start multiple threads
        threads = []
        for _ in range(10):
            thread = threading.Thread(target=start_interaction)
            threads.append(thread)
            thread.start()
        
        # Wait for all threads to complete
        for thread in threads:
            thread.join()
        
        # Check results
        assert len(errors) == 0, f"Errors occurred: {errors}"
        assert len(results) == 10
        
        # All interactions should have been successful
        for interaction_id, container_exists in results:
            assert container_exists is True
            assert interaction_id is not None


class TestInteractionTrackingIntegration:
    """Integration tests for interaction tracking with other components."""
    
    @pytest.fixture
    def mock_streaming_callback(self):
        return AsyncMock()
    
    @pytest.fixture
    def agent_with_tools(self, mock_streaming_callback):
        """Create agent with mock tool chest."""
        tool_chest = Mock(spec=ToolChest)
        tool_chest.register_interaction_container = Mock()
        tool_chest.unregister_interaction_container = Mock()
        
        return MockAgent(
            model_name="test-model",
            streaming_callback=mock_streaming_callback,
            tool_chest=tool_chest,
            enable_interaction_tracking=True
        )
    
    def test_integration_with_tool_chest(self, agent_with_tools):
        """Test integration with ToolChest."""
        interaction_id = agent_with_tools._start_interaction()
        
        # Verify tool chest registration
        agent_with_tools.tool_chest.register_interaction_container.assert_called_once()
        
        agent_with_tools._end_interaction()
        
        # Verify tool chest unregistration
        agent_with_tools.tool_chest.unregister_interaction_container.assert_called_once()
    
    @pytest.mark.asyncio
    async def test_work_log_generation_on_interaction_end(self, agent_with_tools):
        """Test that work log entries are generated when interaction ends."""
        interaction_id = agent_with_tools._start_interaction()
        container = agent_with_tools._get_interaction_container()
        
        # Add a mock message with tool call
        message = agent_with_tools._create_enhanced_message(
            role="assistant",
            content="Using tool"
        )
        agent_with_tools._add_message_to_interaction(message)
        
        # Mock the work log generation
        with patch.object(container, 'generate_work_log_entries') as mock_generate:
            agent_with_tools._end_interaction()
            mock_generate.assert_called_once()
    
    def test_backward_compatibility(self, mock_streaming_callback):
        """Test that existing code works without interaction tracking."""
        # Create agent with tracking disabled
        agent = MockAgent(
            model_name="test-model",
            streaming_callback=mock_streaming_callback,
            enable_interaction_tracking=False
        )
        
        # Should work without errors
        assert agent._get_current_interaction_id() is None
        assert agent._get_interaction_container() is None
        
        # Methods should return safe defaults
        message = agent._create_enhanced_message("user", "test")
        assert message.interaction_id == "unknown"
        
        result = agent._add_message_to_interaction(message)
        assert result is False


if __name__ == "__main__":
    pytest.main([__file__])
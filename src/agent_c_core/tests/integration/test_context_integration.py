"""
Integration tests for ChatSession context integration.

This module tests the integration between BaseAgent and both legacy ChatSession
and EnhancedChatSession to ensure backward compatibility and new functionality.
"""

import pytest
import asyncio
from typing import List, Dict, Any
from unittest.mock import Mock, AsyncMock

from agent_c.agents.base import BaseAgent
from agent_c.models.chat_history.chat_session import ChatSession
from agent_c.models.chat_history.enhanced_chat_session import EnhancedChatSession
from agent_c.models.chat_history.compatibility import SessionManagerCompatibility
from agent_c.models.common_chat.enhanced_models import EnhancedCommonChatMessage, EnhancedTextContentBlock
from agent_c.toolsets.tool_chest import ToolChest


class MockAgent(BaseAgent):
    """Mock agent for testing context integration."""
    
    @classmethod
    def client(cls, **opts):
        return Mock()
    
    @property
    def tool_format(self) -> str:
        return "claude"
    
    async def chat(self, **kwargs) -> List[dict[str, Any]]:
        """Mock chat implementation."""
        # Start interaction
        interaction_id = await self._raise_interaction_start(**kwargs)
        
        # Process user message
        user_message = kwargs.get('user_message', 'test message')
        await self._raise_user_request(user_message, **kwargs)
        
        # Generate response
        await self._raise_text_delta('Hello', **kwargs)
        await self._raise_text_delta(' world!', **kwargs)
        
        # End interaction
        await self._raise_interaction_end(**kwargs)
        
        return [{'role': 'assistant', 'content': 'Hello world!'}]


class TestLegacyChatSessionIntegration:
    """Test integration with legacy ChatSession."""
    
    @pytest.fixture
    def legacy_session(self):
        """Create a legacy ChatSession with sample data."""
        return ChatSession(
            session_id="legacy-test",
            session_name="Legacy Test Session",
            messages=[
                {"role": "user", "content": "Hello"},
                {"role": "assistant", "content": "Hi there!"},
                {"role": "user", "content": "How are you?"}
            ]
        )
    
    @pytest.fixture
    def agent(self):
        """Create a mock agent for testing."""
        return MockAgent(
            model_name="test-model",
            enable_interaction_tracking=True
        )
    
    @pytest.mark.asyncio
    async def test_legacy_session_message_construction(self, agent, legacy_session):
        """Test that legacy sessions work with message construction."""
        messages = await agent._construct_message_array(
            chat_session=legacy_session,
            user_message="New message"
        )
        
        # Should include legacy messages plus new user message
        assert len(messages) >= 4  # 3 legacy + 1 new
        assert messages[-1]["role"] == "user"
        assert messages[-1]["content"] == "New message"
        
        # Legacy messages should be preserved
        assert messages[0]["role"] == "user"
        assert messages[0]["content"] == "Hello"
    
    @pytest.mark.asyncio
    async def test_legacy_session_callback_opts(self, agent, legacy_session):
        """Test callback options with legacy session."""
        opts = agent._callback_opts(chat_session=legacy_session)
        
        assert opts["session_id"] == "legacy-test"
        assert opts["role"] == "assistant"
    
    @pytest.mark.asyncio
    async def test_legacy_session_with_interaction_tracking(self, agent, legacy_session):
        """Test that interaction tracking works with legacy sessions."""
        # Start interaction
        interaction_id = agent._start_interaction()
        
        # Construct messages with legacy session
        messages = await agent._construct_message_array(
            chat_session=legacy_session,
            user_message="Test message",
            interaction_id=interaction_id
        )
        
        # Should have messages
        assert len(messages) >= 4
        
        # Interaction container should have tracked the new user message
        container = agent._get_interaction_container()
        assert container is not None
        
        tracked_messages = container.get_active_messages()
        assert len(tracked_messages) >= 1  # At least the new user message
    
    def test_compatibility_layer_with_legacy_session(self, legacy_session):
        """Test compatibility layer functions with legacy session."""
        # Test get_messages_compatible
        messages = SessionManagerCompatibility.get_messages_compatible(legacy_session)
        assert len(messages) == 3
        assert messages[0]["content"] == "Hello"
        
        # Test add_message_compatible
        new_message = {"role": "assistant", "content": "How can I help?"}
        SessionManagerCompatibility.add_message_compatible(legacy_session, new_message)
        
        updated_messages = SessionManagerCompatibility.get_messages_compatible(legacy_session)
        assert len(updated_messages) == 4
        assert updated_messages[-1]["content"] == "How can I help?"
    
    def test_ensure_enhanced_session_with_legacy(self, legacy_session):
        """Test ensuring enhanced session from legacy session."""
        enhanced = SessionManagerCompatibility.ensure_enhanced_session(legacy_session)
        
        assert isinstance(enhanced, EnhancedChatSession)
        assert enhanced.session_id == legacy_session.session_id
        
        # Messages should be preserved
        enhanced_messages = enhanced.get_messages_for_llm()
        assert len(enhanced_messages) == 3
        assert enhanced_messages[0]["content"] == "Hello"


class TestEnhancedChatSessionIntegration:
    """Test integration with EnhancedChatSession."""
    
    @pytest.fixture
    def enhanced_session(self):
        """Create an enhanced ChatSession with sample data."""
        session = EnhancedChatSession(
            session_id="enhanced-test",
            session_name="Enhanced Test Session"
        )
        
        # Add some sample interactions
        interaction1 = session.start_interaction()
        session.add_message_to_current_interaction(
            EnhancedCommonChatMessage(
                id="msg-1",
                role="user",
                content=[EnhancedTextContentBlock(text="Hello")],
                interaction_id=interaction1
            )
        )
        session.add_message_to_current_interaction(
            EnhancedCommonChatMessage(
                id="msg-2",
                role="assistant",
                content=[EnhancedTextContentBlock(text="Hi there!")],
                interaction_id=interaction1
            )
        )
        session.end_interaction()
        
        interaction2 = session.start_interaction()
        session.add_message_to_current_interaction(
            EnhancedCommonChatMessage(
                id="msg-3",
                role="user",
                content=[EnhancedTextContentBlock(text="How are you?")],
                interaction_id=interaction2
            )
        )
        
        return session
    
    @pytest.fixture
    def agent(self):
        """Create a mock agent for testing."""
        return MockAgent(
            model_name="test-model",
            enable_interaction_tracking=True
        )
    
    @pytest.mark.asyncio
    async def test_enhanced_session_message_construction(self, agent, enhanced_session):
        """Test message construction with enhanced session."""
        messages = await agent._construct_message_array(
            chat_session=enhanced_session,
            user_message="New message"
        )
        
        # Should include enhanced session messages plus new user message
        assert len(messages) >= 4  # 3 from session + 1 new
        assert messages[-1]["role"] == "user"
        assert messages[-1]["content"] == "New message"
        
        # Enhanced session messages should be converted to dict format
        assert messages[0]["role"] == "user"
        assert messages[0]["content"] == "Hello"
    
    @pytest.mark.asyncio
    async def test_enhanced_session_interaction_integration(self, agent, enhanced_session):
        """Test interaction integration with enhanced session."""
        # Agent should work with enhanced session's current interaction
        current_interaction = enhanced_session.get_current_interaction()
        assert current_interaction is not None
        
        # Construct messages
        messages = await agent._construct_message_array(
            chat_session=enhanced_session,
            user_message="Integration test"
        )
        
        # Should have all messages
        assert len(messages) >= 4
        
        # New message should be tracked in the enhanced session's interaction
        updated_messages = enhanced_session.get_all_messages()
        assert len(updated_messages) >= 4  # Original 3 + new 1
    
    def test_compatibility_layer_with_enhanced_session(self, enhanced_session):
        """Test compatibility layer with enhanced session."""
        # Test get_messages_compatible
        messages = SessionManagerCompatibility.get_messages_compatible(enhanced_session)
        assert len(messages) == 3
        assert messages[0]["content"] == "Hello"
        
        # Test add_message_compatible
        new_message = {"role": "assistant", "content": "I'm doing well!"}
        SessionManagerCompatibility.add_message_compatible(enhanced_session, new_message)
        
        updated_messages = SessionManagerCompatibility.get_messages_compatible(enhanced_session)
        assert len(updated_messages) == 4
        assert updated_messages[-1]["content"] == "I'm doing well!"
    
    def test_ensure_enhanced_session_with_enhanced(self, enhanced_session):
        """Test ensuring enhanced session when already enhanced."""
        result = SessionManagerCompatibility.ensure_enhanced_session(enhanced_session)
        
        # Should return the same instance
        assert result is enhanced_session
        assert isinstance(result, EnhancedChatSession)


class TestMixedSessionIntegration:
    """Test integration with mixed session types."""
    
    @pytest.fixture
    def agent_with_tools(self):
        """Create agent with tool chest for advanced testing."""
        tool_chest = Mock(spec=ToolChest)
        tool_chest.register_interaction_container = Mock()
        tool_chest.unregister_interaction_container = Mock()
        
        return MockAgent(
            model_name="test-model",
            tool_chest=tool_chest,
            enable_interaction_tracking=True
        )
    
    def test_mixed_session_types_compatibility(self):
        """Test that both session types work with compatibility functions."""
        # Create both session types
        legacy_session = ChatSession(
            session_id="legacy",
            messages=[{"role": "user", "content": "Legacy message"}]
        )
        
        enhanced_session = EnhancedChatSession(session_id="enhanced")
        enhanced_session.start_interaction()
        enhanced_session.add_message_to_current_interaction(
            EnhancedCommonChatMessage(
                id="enhanced-msg",
                role="user",
                content=[EnhancedTextContentBlock(text="Enhanced message")],
                interaction_id=enhanced_session.current_interaction_id
            )
        )
        
        # Both should work with compatibility functions
        legacy_messages = SessionManagerCompatibility.get_messages_compatible(legacy_session)
        enhanced_messages = SessionManagerCompatibility.get_messages_compatible(enhanced_session)
        
        assert len(legacy_messages) == 1
        assert len(enhanced_messages) == 1
        assert legacy_messages[0]["content"] == "Legacy message"
        assert enhanced_messages[0]["content"] == "Enhanced message"
    
    @pytest.mark.asyncio
    async def test_agent_works_with_both_session_types(self, agent_with_tools):
        """Test that agent works seamlessly with both session types."""
        # Test with legacy session
        legacy_session = ChatSession(
            session_id="legacy-agent-test",
            messages=[{"role": "user", "content": "Legacy test"}]
        )
        
        legacy_messages = await agent_with_tools._construct_message_array(
            chat_session=legacy_session,
            user_message="New legacy message"
        )
        
        assert len(legacy_messages) >= 2
        
        # Test with enhanced session
        enhanced_session = EnhancedChatSession(session_id="enhanced-agent-test")
        enhanced_session.start_interaction()
        enhanced_session.add_message_to_current_interaction(
            EnhancedCommonChatMessage(
                id="test-msg",
                role="user",
                content=[EnhancedTextContentBlock(text="Enhanced test")],
                interaction_id=enhanced_session.current_interaction_id
            )
        )
        
        enhanced_messages = await agent_with_tools._construct_message_array(
            chat_session=enhanced_session,
            user_message="New enhanced message"
        )
        
        assert len(enhanced_messages) >= 2
        
        # Both should produce similar results
        assert legacy_messages[-1]["role"] == "user"
        assert enhanced_messages[-1]["role"] == "user"
    
    @pytest.mark.asyncio
    async def test_interaction_tracking_with_both_session_types(self, agent_with_tools):
        """Test interaction tracking works with both session types."""
        # Test with legacy session
        legacy_session = ChatSession(session_id="legacy-tracking")
        
        # Start interaction and use legacy session
        interaction_id = agent_with_tools._start_interaction()
        await agent_with_tools._construct_message_array(
            chat_session=legacy_session,
            user_message="Track this",
            interaction_id=interaction_id
        )
        
        # Should have tracked the message
        container = agent_with_tools._get_interaction_container(interaction_id)
        assert container is not None
        tracked_messages = container.get_active_messages()
        assert len(tracked_messages) >= 1
        
        agent_with_tools._end_interaction(interaction_id)
        
        # Test with enhanced session
        enhanced_session = EnhancedChatSession(session_id="enhanced-tracking")
        enhanced_session.start_interaction()
        
        interaction_id2 = agent_with_tools._start_interaction()
        await agent_with_tools._construct_message_array(
            chat_session=enhanced_session,
            user_message="Track this too",
            interaction_id=interaction_id2
        )
        
        # Should have tracked this message as well
        container2 = agent_with_tools._get_interaction_container(interaction_id2)
        assert container2 is not None
        tracked_messages2 = container2.get_active_messages()
        assert len(tracked_messages2) >= 1


class TestBackwardCompatibilityGuarantees:
    """Test that backward compatibility is maintained."""
    
    def test_legacy_code_patterns_still_work(self):
        """Test that common legacy code patterns continue to work."""
        # Pattern 1: Direct message access
        session = ChatSession(
            session_id="compat-test",
            messages=[{"role": "user", "content": "Test"}]
        )
        
        # Should still work
        messages = session.messages
        assert len(messages) == 1
        assert messages[0]["content"] == "Test"
        
        # Pattern 2: Message appending
        session.messages.append({"role": "assistant", "content": "Response"})
        assert len(session.messages) == 2
    
    def test_session_manager_compatibility(self):
        """Test session manager compatibility patterns."""
        # Common pattern: getting messages for LLM
        legacy_session = ChatSession(
            session_id="manager-test",
            messages=[{"role": "user", "content": "Hello"}]
        )
        
        # Should work with compatibility layer
        messages = SessionManagerCompatibility.get_messages_compatible(legacy_session)
        assert len(messages) == 1
        assert messages[0]["content"] == "Hello"
        
        # Should work with enhanced session too
        enhanced_session = EnhancedChatSession(session_id="enhanced-manager-test")
        enhanced_session.migrate_from_legacy([{"role": "user", "content": "Hello"}])
        
        enhanced_messages = SessionManagerCompatibility.get_messages_compatible(enhanced_session)
        assert len(enhanced_messages) == 1
        assert enhanced_messages[0]["content"] == "Hello"
    
    @pytest.mark.asyncio
    async def test_agent_backward_compatibility(self):
        """Test that agents maintain backward compatibility."""
        agent = MockAgent(
            model_name="compat-test",
            enable_interaction_tracking=False  # Test with tracking disabled
        )
        
        # Legacy session usage should still work
        legacy_session = ChatSession(
            session_id="agent-compat",
            messages=[{"role": "user", "content": "Legacy"}]
        )
        
        messages = await agent._construct_message_array(
            chat_session=legacy_session,
            user_message="New message"
        )
        
        assert len(messages) >= 2
        assert messages[0]["content"] == "Legacy"
        assert messages[-1]["content"] == "New message"
    
    def test_no_breaking_changes_in_apis(self):
        """Test that no breaking changes were introduced."""
        # ChatSession should still have all original fields and methods
        session = ChatSession(session_id="api-test")
        
        # Original fields should exist
        assert hasattr(session, 'session_id')
        assert hasattr(session, 'messages')
        assert hasattr(session, 'token_count')
        assert hasattr(session, 'metadata')
        assert hasattr(session, 'touch')
        
        # Original methods should work
        session.touch()
        assert session.updated_at is not None
        
        # EnhancedChatSession should be a superset
        enhanced = EnhancedChatSession(session_id="enhanced-api-test")
        
        # Should have all original fields
        assert hasattr(enhanced, 'session_id')
        assert hasattr(enhanced, 'messages')  # Via property
        assert hasattr(enhanced, 'token_count')
        assert hasattr(enhanced, 'metadata')
        assert hasattr(enhanced, 'touch')
        
        # Plus new fields
        assert hasattr(enhanced, 'interaction_containers')
        assert hasattr(enhanced, 'current_interaction_id')
        assert hasattr(enhanced, 'start_interaction')
        assert hasattr(enhanced, 'end_interaction')


if __name__ == "__main__":
    pytest.main([__file__])
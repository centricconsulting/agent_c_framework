"""
Test suite for EnhancedChatSession and ChatSession integration.

This module tests the enhanced chat session functionality, migration capabilities,
and backward compatibility features.
"""

import pytest
import datetime
import uuid
from typing import List, Dict, Any
from unittest.mock import Mock, patch

from agent_c.models.chat_history.chat_session import ChatSession
from agent_c.models.chat_history.enhanced_chat_session import EnhancedChatSession
from agent_c.models.chat_history.interaction_container import InteractionContainer
from agent_c.models.common_chat.enhanced_models import (
    EnhancedCommonChatMessage, 
    EnhancedTextContentBlock,
    EnhancedToolUseContentBlock,
    EnhancedToolResultContentBlock
)


class TestEnhancedChatSession:
    """Test EnhancedChatSession functionality."""
    
    @pytest.fixture
    def enhanced_session(self):
        """Create a basic enhanced session for testing."""
        return EnhancedChatSession(
            session_id="test-session",
            session_name="Test Session",
            user_id="test-user"
        )
    
    @pytest.fixture
    def sample_legacy_messages(self):
        """Sample legacy message array."""
        return [
            {"role": "user", "content": "Hello"},
            {"role": "assistant", "content": "Hi there!"},
            {"role": "user", "content": "How are you?"},
            {"role": "assistant", "content": "I'm doing well, thank you!"}
        ]
    
    def test_enhanced_session_initialization(self, enhanced_session):
        """Test basic initialization of enhanced session."""
        assert enhanced_session.session_id == "test-session"
        assert enhanced_session.session_name == "Test Session"
        assert enhanced_session.user_id == "test-user"
        assert len(enhanced_session.interaction_containers) == 0
        assert enhanced_session.current_interaction_id is None
        assert enhanced_session.enable_work_logs is True
        assert enhanced_session.enable_tool_optimization is True
    
    def test_start_interaction(self, enhanced_session):
        """Test starting a new interaction."""
        interaction_id = enhanced_session.start_interaction()
        
        assert interaction_id is not None
        assert enhanced_session.current_interaction_id == interaction_id
        assert interaction_id in enhanced_session.interaction_containers
        assert isinstance(enhanced_session.interaction_containers[interaction_id], InteractionContainer)
    
    def test_start_interaction_with_custom_id(self, enhanced_session):
        """Test starting interaction with custom ID."""
        custom_id = "custom-interaction-123"
        interaction_id = enhanced_session.start_interaction(custom_id)
        
        assert interaction_id == custom_id
        assert enhanced_session.current_interaction_id == custom_id
        assert custom_id in enhanced_session.interaction_containers
    
    def test_end_interaction(self, enhanced_session):
        """Test ending an interaction."""
        interaction_id = enhanced_session.start_interaction()
        container = enhanced_session.get_current_interaction()
        
        enhanced_session.end_interaction()
        
        assert enhanced_session.current_interaction_id is None
        assert container.is_completed() is True
    
    def test_end_specific_interaction(self, enhanced_session):
        """Test ending a specific interaction."""
        interaction_id1 = enhanced_session.start_interaction()
        interaction_id2 = enhanced_session.start_interaction()
        
        enhanced_session.end_interaction(interaction_id1)
        
        # Current interaction should still be the second one
        assert enhanced_session.current_interaction_id == interaction_id2
        
        # First interaction should be completed
        container1 = enhanced_session.get_interaction(interaction_id1)
        assert container1.is_completed() is True
    
    def test_get_current_interaction(self, enhanced_session):
        """Test getting current interaction container."""
        assert enhanced_session.get_current_interaction() is None
        
        interaction_id = enhanced_session.start_interaction()
        current = enhanced_session.get_current_interaction()
        
        assert current is not None
        assert current.interaction_id == interaction_id
    
    def test_add_message_to_current_interaction(self, enhanced_session):
        """Test adding messages to current interaction."""
        # Should auto-start interaction if none exists
        message = EnhancedCommonChatMessage(
            id=str(uuid.uuid4()),
            role="user",
            content=[EnhancedTextContentBlock(text="Hello")],
            interaction_id="temp"
        )
        
        result = enhanced_session.add_message_to_current_interaction(message)
        assert result is True
        assert enhanced_session.current_interaction_id is not None
        
        # Check message was added
        current = enhanced_session.get_current_interaction()
        messages = current.get_active_messages()
        assert len(messages) == 1
        assert messages[0].content[0].text == "Hello"
    
    def test_get_all_messages(self, enhanced_session):
        """Test getting all messages across interactions."""
        # Add messages to multiple interactions
        interaction1 = enhanced_session.start_interaction()
        enhanced_session.add_message_to_current_interaction(
            EnhancedCommonChatMessage(
                id=str(uuid.uuid4()),
                role="user",
                content=[EnhancedTextContentBlock(text="Message 1")],
                interaction_id=interaction1
            )
        )
        enhanced_session.end_interaction()
        
        interaction2 = enhanced_session.start_interaction()
        enhanced_session.add_message_to_current_interaction(
            EnhancedCommonChatMessage(
                id=str(uuid.uuid4()),
                role="assistant",
                content=[EnhancedTextContentBlock(text="Message 2")],
                interaction_id=interaction2
            )
        )
        
        all_messages = enhanced_session.get_all_messages()
        assert len(all_messages) == 2
        assert all_messages[0].content[0].text == "Message 1"
        assert all_messages[1].content[0].text == "Message 2"
    
    def test_get_messages_for_llm(self, enhanced_session):
        """Test getting messages in LLM-compatible format."""
        interaction_id = enhanced_session.start_interaction()
        
        # Add various message types
        enhanced_session.add_message_to_current_interaction(
            EnhancedCommonChatMessage(
                id=str(uuid.uuid4()),
                role="user",
                content=[EnhancedTextContentBlock(text="Hello")],
                interaction_id=interaction_id
            )
        )
        
        enhanced_session.add_message_to_current_interaction(
            EnhancedCommonChatMessage(
                id=str(uuid.uuid4()),
                role="assistant",
                content=[EnhancedTextContentBlock(text="Hi there!")],
                interaction_id=interaction_id
            )
        )
        
        llm_messages = enhanced_session.get_messages_for_llm()
        
        assert len(llm_messages) == 2
        assert llm_messages[0] == {"role": "user", "content": "Hello"}
        assert llm_messages[1] == {"role": "assistant", "content": "Hi there!"}
    
    def test_backward_compatibility_messages_property(self, enhanced_session):
        """Test backward compatibility messages property."""
        # Add some messages
        interaction_id = enhanced_session.start_interaction()
        enhanced_session.add_message_to_current_interaction(
            EnhancedCommonChatMessage(
                id=str(uuid.uuid4()),
                role="user",
                content=[EnhancedTextContentBlock(text="Test message")],
                interaction_id=interaction_id
            )
        )
        
        # Access via legacy property
        messages = enhanced_session.messages
        assert len(messages) == 1
        assert messages[0]["role"] == "user"
        assert messages[0]["content"] == "Test message"
    
    def test_backward_compatibility_messages_setter(self, enhanced_session, sample_legacy_messages):
        """Test backward compatibility messages setter."""
        # Set messages using legacy format
        enhanced_session.messages = sample_legacy_messages
        
        # Should have created interactions
        assert len(enhanced_session.interaction_containers) > 0
        
        # Messages should be accessible
        messages = enhanced_session.get_messages_for_llm()
        assert len(messages) == 4
        assert messages[0]["content"] == "Hello"
        assert messages[1]["content"] == "Hi there!"
    
    def test_migrate_from_legacy(self, enhanced_session, sample_legacy_messages):
        """Test migration from legacy message format."""
        result = enhanced_session.migrate_from_legacy(sample_legacy_messages)
        
        assert result is True
        assert len(enhanced_session.interaction_containers) >= 1
        
        all_messages = enhanced_session.get_all_messages()
        assert len(all_messages) == 4
        
        # Check message content preservation
        llm_messages = enhanced_session.get_messages_for_llm()
        assert llm_messages[0]["content"] == "Hello"
        assert llm_messages[1]["content"] == "Hi there!"
    
    def test_migrate_from_legacy_with_tool_calls(self, enhanced_session):
        """Test migration with tool call messages."""
        legacy_messages = [
            {"role": "user", "content": "Use a tool"},
            {
                "role": "assistant", 
                "content": [
                    {
                        "type": "tool_use",
                        "id": "tool-123",
                        "name": "test_tool",
                        "input": {"param": "value"}
                    }
                ]
            },
            {
                "role": "user",
                "content": [
                    {
                        "type": "tool_result",
                        "tool_use_id": "tool-123",
                        "content": "Tool result"
                    }
                ]
            }
        ]
        
        result = enhanced_session.migrate_from_legacy(legacy_messages)
        assert result is True
        
        all_messages = enhanced_session.get_all_messages()
        assert len(all_messages) == 3
        
        # Check tool use message
        tool_use_msg = all_messages[1]
        assert tool_use_msg.role == "assistant"
        assert len(tool_use_msg.content) == 1
        assert isinstance(tool_use_msg.content[0], EnhancedToolUseContentBlock)
        assert tool_use_msg.content[0].name == "test_tool"
    
    def test_export_session_data(self, enhanced_session, sample_legacy_messages):
        """Test exporting session data."""
        enhanced_session.migrate_from_legacy(sample_legacy_messages)
        
        data = enhanced_session.export_session_data()
        
        assert data["session_id"] == enhanced_session.session_id
        assert data["session_name"] == enhanced_session.session_name
        assert "interactions" in data
        assert len(data["interactions"]) >= 1
        
        # Check interaction data structure
        interaction_data = list(data["interactions"].values())[0]
        assert "interaction_id" in interaction_data
        assert "messages" in interaction_data
        assert len(interaction_data["messages"]) > 0
    
    def test_import_session_data(self, enhanced_session, sample_legacy_messages):
        """Test importing session data."""
        # First export data
        enhanced_session.migrate_from_legacy(sample_legacy_messages)
        exported_data = enhanced_session.export_session_data()
        
        # Create new session and import
        new_session = EnhancedChatSession(session_id="import-test")
        result = new_session.import_session_data(exported_data)
        
        assert result is True
        assert new_session.session_id == exported_data["session_id"]
        assert len(new_session.interaction_containers) == len(exported_data["interactions"])
        
        # Check message preservation
        imported_messages = new_session.get_messages_for_llm()
        original_messages = enhanced_session.get_messages_for_llm()
        assert len(imported_messages) == len(original_messages)
    
    def test_get_session_statistics(self, enhanced_session, sample_legacy_messages):
        """Test session statistics generation."""
        enhanced_session.migrate_from_legacy(sample_legacy_messages)
        enhanced_session.end_interaction()  # Complete the interaction
        
        stats = enhanced_session.get_session_statistics()
        
        assert stats["session_id"] == enhanced_session.session_id
        assert stats["total_interactions"] >= 1
        assert stats["total_messages"] == 4
        assert stats["interactions_completed"] >= 1
        assert "message_types" in stats
        assert stats["message_types"]["user"] == 2
        assert stats["message_types"]["assistant"] == 2
    
    def test_touch_method(self, enhanced_session):
        """Test the touch method updates timestamp."""
        original_updated = enhanced_session.updated_at
        
        # Small delay to ensure timestamp difference
        import time
        time.sleep(0.001)
        
        enhanced_session.touch()
        
        assert enhanced_session.updated_at != original_updated
    
    def test_empty_session_handling(self, enhanced_session):
        """Test handling of empty sessions."""
        # Empty migration
        result = enhanced_session.migrate_from_legacy([])
        assert result is True
        assert len(enhanced_session.interaction_containers) == 0
        
        # Empty messages access
        messages = enhanced_session.get_messages_for_llm()
        assert len(messages) == 0
        
        # Empty statistics
        stats = enhanced_session.get_session_statistics()
        assert stats["total_messages"] == 0
        assert stats["total_interactions"] == 0


class TestEnhancedChatSessionIntegration:
    """Integration tests for enhanced chat session with other components."""
    
    @pytest.fixture
    def enhanced_session_with_data(self):
        """Enhanced session with sample data."""
        session = EnhancedChatSession(session_id="integration-test")
        
        # Add multiple interactions with various content
        interaction1 = session.start_interaction()
        session.add_message_to_current_interaction(
            EnhancedCommonChatMessage(
                id=str(uuid.uuid4()),
                role="user",
                content=[EnhancedTextContentBlock(text="Hello")],
                interaction_id=interaction1
            )
        )
        session.add_message_to_current_interaction(
            EnhancedCommonChatMessage(
                id=str(uuid.uuid4()),
                role="assistant",
                content=[EnhancedTextContentBlock(text="Hi! How can I help?")],
                interaction_id=interaction1
            )
        )
        session.end_interaction()
        
        interaction2 = session.start_interaction()
        session.add_message_to_current_interaction(
            EnhancedCommonChatMessage(
                id=str(uuid.uuid4()),
                role="user",
                content=[EnhancedTextContentBlock(text="Use a tool please")],
                interaction_id=interaction2
            )
        )
        session.add_message_to_current_interaction(
            EnhancedCommonChatMessage(
                id=str(uuid.uuid4()),
                role="assistant",
                content=[EnhancedToolUseContentBlock(
                    id="tool-123",
                    name="test_tool",
                    input={"param": "value"}
                )],
                interaction_id=interaction2
            )
        )
        session.add_message_to_current_interaction(
            EnhancedCommonChatMessage(
                id=str(uuid.uuid4()),
                role="user",
                content=[EnhancedToolResultContentBlock(
                    tool_use_id="tool-123",
                    content="Tool executed successfully"
                )],
                interaction_id=interaction2
            )
        )
        
        return session
    
    def test_work_log_integration(self, enhanced_session_with_data):
        """Test work log generation integration."""
        # End current interaction to trigger work log generation
        enhanced_session_with_data.end_interaction()
        
        # Check that work log entries were generated
        for container in enhanced_session_with_data.interaction_containers.values():
            if container.is_completed():
                # Should have work log entries for tool usage
                work_log_summary = container.get_tool_context_summary()
                assert work_log_summary is not None
    
    def test_tool_optimization_integration(self, enhanced_session_with_data):
        """Test tool optimization integration."""
        current_interaction = enhanced_session_with_data.get_current_interaction()
        assert current_interaction is not None
        
        # Test message optimization
        initial_message_count = len(current_interaction.get_active_messages())
        
        # Optimize messages (should not fail)
        try:
            current_interaction.optimize_message_array("remove_invalidated")
            # Should complete without error
            assert True
        except Exception as e:
            pytest.fail(f"Message optimization failed: {e}")
    
    def test_observable_pattern_integration(self, enhanced_session_with_data):
        """Test observable pattern integration."""
        current_interaction = enhanced_session_with_data.get_current_interaction()
        
        # Test that interaction container supports observable pattern
        assert hasattr(current_interaction, 'model_changed')
        
        # Add a message and check if observable events would fire
        message = EnhancedCommonChatMessage(
            id=str(uuid.uuid4()),
            role="user",
            content=[EnhancedTextContentBlock(text="Observable test")],
            interaction_id=current_interaction.interaction_id
        )
        
        # Should not raise an error
        current_interaction.add_message(message)
    
    def test_performance_with_large_session(self):
        """Test performance with large number of messages."""
        session = EnhancedChatSession(session_id="performance-test")
        
        # Create a large number of messages
        num_messages = 1000
        interaction_id = session.start_interaction()
        
        import time
        start_time = time.time()
        
        for i in range(num_messages):
            session.add_message_to_current_interaction(
                EnhancedCommonChatMessage(
                    id=str(uuid.uuid4()),
                    role="user" if i % 2 == 0 else "assistant",
                    content=[EnhancedTextContentBlock(text=f"Message {i}")],
                    interaction_id=interaction_id
                )
            )
        
        end_time = time.time()
        duration = end_time - start_time
        
        # Should complete in reasonable time (less than 5 seconds)
        assert duration < 5.0
        
        # Verify all messages were added
        all_messages = session.get_all_messages()
        assert len(all_messages) == num_messages
        
        # Test LLM format conversion performance
        start_time = time.time()
        llm_messages = session.get_messages_for_llm()
        end_time = time.time()
        
        assert len(llm_messages) == num_messages
        assert (end_time - start_time) < 1.0  # Should be fast


if __name__ == "__main__":
    pytest.main([__file__])
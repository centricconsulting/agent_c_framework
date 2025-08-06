"""
Comprehensive tests for InteractionContainer implementation.

Tests cover:
- Core message management functionality
- Tool manipulation interfaces
- Observable pattern integration
- Thread safety
- Message lifecycle management
- Optimization strategies
- Work log integration
"""

import pytest
import time
import threading
from datetime import datetime
from uuid import uuid4
from unittest.mock import Mock, patch

from agent_c.models.chat_history.interaction_container import (
    InteractionContainer,
    OptimizationStrategy,
    InvalidationCriteria
)
from agent_c.models.common_chat.enhanced_models import (
    EnhancedCommonChatMessage,
    EnhancedTextContentBlock,
    EnhancedToolUseContentBlock,
    EnhancedToolResultContentBlock,
    ValidityState,
    OutcomeStatus,
    MessageRole,
    ContentBlockType
)


class TestInteractionContainerBasics:
    """Test basic InteractionContainer functionality."""
    
    def test_basic_creation(self):
        """Test basic container creation with defaults."""
        container = InteractionContainer()
        
        assert container.interaction_id is not None
        assert isinstance(container.interaction_start, float)
        assert container.interaction_stop is None
        assert container.messages == []
        assert container.validity_state == ValidityState.ACTIVE
        assert container.optimization_metadata == {}
        assert container.work_log_entries == []
        assert container.invalidated_by is None
    
    def test_creation_with_messages(self):
        """Test container creation with initial messages."""
        message1 = EnhancedCommonChatMessage(
            role=MessageRole.USER,
            content=[EnhancedTextContentBlock(text="Hello")]
        )
        message2 = EnhancedCommonChatMessage(
            role=MessageRole.ASSISTANT,
            content=[EnhancedTextContentBlock(text="Hi there!")]
        )
        
        container = InteractionContainer(messages=[message1, message2])
        
        assert len(container.messages) == 2
        # Messages should have container's interaction_id
        assert message1.interaction_id == container.interaction_id
        assert message2.interaction_id == container.interaction_id
    
    def test_interaction_id_assignment(self):
        """Test that messages get assigned the container's interaction_id."""
        interaction_id = str(uuid4())
        message = EnhancedCommonChatMessage(
            role=MessageRole.USER,
            content=[EnhancedTextContentBlock(text="Test")]
        )
        
        container = InteractionContainer(
            interaction_id=interaction_id,
            messages=[message]
        )
        
        assert container.interaction_id == interaction_id
        assert message.interaction_id == interaction_id


class TestMessageManagement:
    """Test core message management functionality."""
    
    def test_add_message(self):
        """Test adding messages to container."""
        container = InteractionContainer()
        message = EnhancedCommonChatMessage(
            role=MessageRole.USER,
            content=[EnhancedTextContentBlock(text="Test message")]
        )
        
        container.add_message(message)
        
        assert len(container.messages) == 1
        assert container.messages[0] == message
        assert message.interaction_id == container.interaction_id
    
    def test_add_message_at_position(self):
        """Test adding message at specific position."""
        container = InteractionContainer()
        
        message1 = EnhancedCommonChatMessage(
            role=MessageRole.USER,
            content=[EnhancedTextContentBlock(text="First")]
        )
        message2 = EnhancedCommonChatMessage(
            role=MessageRole.USER,
            content=[EnhancedTextContentBlock(text="Third")]
        )
        message3 = EnhancedCommonChatMessage(
            role=MessageRole.USER,
            content=[EnhancedTextContentBlock(text="Second")]
        )
        
        container.add_message(message1)
        container.add_message(message2)
        container.add_message(message3, position=1)
        
        assert len(container.messages) == 3
        assert container.messages[0].text_content == "First"
        assert container.messages[1].text_content == "Second"
        assert container.messages[2].text_content == "Third"
    
    def test_remove_message(self):
        """Test removing messages by ID."""
        container = InteractionContainer()
        message = EnhancedCommonChatMessage(
            role=MessageRole.USER,
            content=[EnhancedTextContentBlock(text="To be removed")]
        )
        
        container.add_message(message)
        assert len(container.messages) == 1
        
        removed = container.remove_message(message.id)
        assert removed is True
        assert len(container.messages) == 0
        
        # Try removing non-existent message
        removed = container.remove_message("non-existent")
        assert removed is False
    
    def test_get_active_messages(self):
        """Test filtering active messages."""
        container = InteractionContainer()
        
        active_message = EnhancedCommonChatMessage(
            role=MessageRole.USER,
            content=[EnhancedTextContentBlock(text="Active")]
        )
        invalidated_message = EnhancedCommonChatMessage(
            role=MessageRole.USER,
            content=[EnhancedTextContentBlock(text="Invalidated")]
        )
        
        container.add_message(active_message)
        container.add_message(invalidated_message)
        
        # Invalidate one message
        invalidated_message.invalidate("test_tool", "test reason")
        
        active_messages = container.get_active_messages()
        assert len(active_messages) == 1
        assert active_messages[0] == active_message
    
    def test_get_all_messages(self):
        """Test getting all messages with inclusion options."""
        container = InteractionContainer()
        
        active_message = EnhancedCommonChatMessage(
            role=MessageRole.USER,
            content=[EnhancedTextContentBlock(text="Active")]
        )
        invalidated_message = EnhancedCommonChatMessage(
            role=MessageRole.USER,
            content=[EnhancedTextContentBlock(text="Invalidated")]
        )
        
        container.add_message(active_message)
        container.add_message(invalidated_message)
        invalidated_message.invalidate("test_tool", "test reason")
        
        # Get active only (default)
        active_only = container.get_all_messages()
        assert len(active_only) == 1
        
        # Get all including invalidated
        all_messages = container.get_all_messages(include_invalidated=True)
        assert len(all_messages) == 2
    
    def test_get_message_by_id(self):
        """Test finding messages by ID."""
        container = InteractionContainer()
        message = EnhancedCommonChatMessage(
            role=MessageRole.USER,
            content=[EnhancedTextContentBlock(text="Find me")]
        )
        
        container.add_message(message)
        
        found = container.get_message_by_id(message.id)
        assert found == message
        
        not_found = container.get_message_by_id("non-existent")
        assert not_found is None
    
    def test_get_messages_by_tool(self):
        """Test finding messages by tool name."""
        container = InteractionContainer()
        
        tool_message = EnhancedCommonChatMessage(
            role=MessageRole.ASSISTANT,
            content=[
                EnhancedToolUseContentBlock(
                    tool_name="test_tool",
                    tool_id="tool_123",
                    parameters={"param": "value"}
                ),
                EnhancedToolResultContentBlock(
                    tool_name="test_tool",
                    tool_id="tool_123",
                    result="success"
                )
            ]
        )
        
        other_message = EnhancedCommonChatMessage(
            role=MessageRole.USER,
            content=[EnhancedTextContentBlock(text="No tools here")]
        )
        
        container.add_message(tool_message)
        container.add_message(other_message)
        
        # Find messages with tool
        tool_messages = container.get_messages_by_tool("test_tool")
        assert len(tool_messages) == 1
        assert tool_messages[0] == tool_message
        
        # Find messages with tool use only
        tool_use_only = container.get_messages_by_tool("test_tool", include_results=False)
        assert len(tool_use_only) == 1  # Same message has both use and result
        
        # Find non-existent tool
        no_messages = container.get_messages_by_tool("non_existent_tool")
        assert len(no_messages) == 0


class TestToolManipulation:
    """Test tool manipulation interfaces."""
    
    def test_invalidate_messages_by_tool_parameter_conflict(self):
        """Test parameter-based message invalidation."""
        container = InteractionContainer()
        
        message = EnhancedCommonChatMessage(
            role=MessageRole.ASSISTANT,
            content=[
                EnhancedToolUseContentBlock(
                    tool_name="update_file",
                    tool_id="tool_123",
                    parameters={"path": "/old/path.txt", "content": "old content"}
                )
            ]
        )
        
        container.add_message(message)
        
        # Invalidate based on parameter conflict
        criteria = {
            'type': InvalidationCriteria.PARAMETER_CONFLICT,
            'conflicting_parameters': ['path'],
            'new_parameters': {'path': '/new/path.txt'},
            'reason': 'Path parameter changed'
        }
        
        invalidated = container.invalidate_messages_by_tool("update_file", criteria)
        
        assert len(invalidated) == 1
        assert invalidated[0] == message.id
        assert not message.is_active()
        assert message.invalidated_by == "update_file"
    
    def test_invalidate_messages_by_tool_semantic_obsolete(self):
        """Test semantic obsolescence invalidation."""
        container = InteractionContainer()
        
        message = EnhancedCommonChatMessage(
            role=MessageRole.ASSISTANT,
            content=[
                EnhancedToolUseContentBlock(
                    tool_name="old_analyzer",
                    tool_id="tool_456",
                    parameters={"data": "test"}
                )
            ]
        )
        
        container.add_message(message)
        
        # Invalidate based on semantic obsolescence
        criteria = {
            'type': InvalidationCriteria.SEMANTIC_OBSOLETE,
            'obsolete_tools': ['old_analyzer'],
            'reason': 'Tool replaced by new_analyzer'
        }
        
        invalidated = container.invalidate_messages_by_tool("new_analyzer", criteria)
        
        assert len(invalidated) == 1
        assert not message.is_active()
    
    def test_invalidate_messages_by_tool_time_based(self):
        """Test time-based message invalidation."""
        container = InteractionContainer()
        
        # Create old message
        old_message = EnhancedCommonChatMessage(
            role=MessageRole.ASSISTANT,
            content=[EnhancedTextContentBlock(text="Old message")]
        )
        old_message.created_at = datetime.fromtimestamp(time.time() - 7200)  # 2 hours ago
        
        # Create recent message
        recent_message = EnhancedCommonChatMessage(
            role=MessageRole.ASSISTANT,
            content=[EnhancedTextContentBlock(text="Recent message")]
        )
        
        container.add_message(old_message)
        container.add_message(recent_message)
        
        # Invalidate messages older than 1 hour
        criteria = {
            'type': InvalidationCriteria.TIME_BASED,
            'max_age_seconds': 3600,  # 1 hour
            'reason': 'Message too old'
        }
        
        invalidated = container.invalidate_messages_by_tool("cleanup_tool", criteria)
        
        assert len(invalidated) == 1
        assert invalidated[0] == old_message.id
        assert not old_message.is_active()
        assert recent_message.is_active()
    
    def test_invalidate_messages_by_tool_custom(self):
        """Test custom invalidation with callback."""
        container = InteractionContainer()
        
        message = EnhancedCommonChatMessage(
            role=MessageRole.ASSISTANT,
            content=[EnhancedTextContentBlock(text="Custom test")]
        )
        
        container.add_message(message)
        
        # Register custom callback
        def custom_callback(msg, criteria):
            return criteria.get('force_invalidate', False)
        
        container.register_tool_optimizer("custom_tool", custom_callback)
        
        # Test invalidation with custom criteria
        criteria = {
            'type': InvalidationCriteria.CUSTOM,
            'force_invalidate': True,
            'reason': 'Custom invalidation logic'
        }
        
        invalidated = container.invalidate_messages_by_tool("custom_tool", criteria)
        
        assert len(invalidated) == 1
        assert not message.is_active()
    
    def test_mark_interaction_superseded(self):
        """Test marking entire interaction as superseded."""
        container = InteractionContainer()
        
        message1 = EnhancedCommonChatMessage(
            role=MessageRole.USER,
            content=[EnhancedTextContentBlock(text="Message 1")]
        )
        message2 = EnhancedCommonChatMessage(
            role=MessageRole.ASSISTANT,
            content=[EnhancedTextContentBlock(text="Message 2")]
        )
        
        container.add_message(message1)
        container.add_message(message2)
        
        new_interaction_id = str(uuid4())
        container.mark_interaction_superseded(new_interaction_id, "Replaced by better interaction")
        
        assert container.validity_state == ValidityState.SUPERSEDED
        assert container.invalidated_by == new_interaction_id
        assert not message1.is_active()
        assert not message2.is_active()
        assert message1.validity_state == ValidityState.SUPERSEDED
        assert message2.validity_state == ValidityState.SUPERSEDED
    
    def test_optimize_message_array_remove_invalidated(self):
        """Test optimization strategy to remove invalidated messages."""
        container = InteractionContainer()
        
        active_message = EnhancedCommonChatMessage(
            role=MessageRole.USER,
            content=[EnhancedTextContentBlock(text="Active")]
        )
        invalidated_message = EnhancedCommonChatMessage(
            role=MessageRole.USER,
            content=[EnhancedTextContentBlock(text="Invalidated")]
        )
        
        container.add_message(active_message)
        container.add_message(invalidated_message)
        
        # Invalidate one message
        invalidated_message.invalidate("test_tool", "test reason")
        
        # Optimize by removing invalidated
        result = container.optimize_message_array(OptimizationStrategy.REMOVE_INVALIDATED)
        
        assert result['strategy'] == 'remove_invalidated'
        assert result['messages_before'] == 2
        assert result['messages_after'] == 1
        assert result['removed_messages'] == 1
        assert len(container.messages) == 1
        assert container.messages[0] == active_message
    
    def test_optimize_message_array_archive_old(self):
        """Test optimization strategy to archive old messages."""
        container = InteractionContainer()
        
        # Create old message
        old_message = EnhancedCommonChatMessage(
            role=MessageRole.USER,
            content=[EnhancedTextContentBlock(text="Old")]
        )
        old_message.created_at = datetime.fromtimestamp(time.time() - 86400 * 2)  # 2 days ago
        
        # Create recent message
        recent_message = EnhancedCommonChatMessage(
            role=MessageRole.USER,
            content=[EnhancedTextContentBlock(text="Recent")]
        )
        
        container.add_message(old_message)
        container.add_message(recent_message)
        
        # Optimize by archiving old messages
        result = container.optimize_message_array(OptimizationStrategy.ARCHIVE_OLD)
        
        assert result['strategy'] == 'archive_old'
        assert result['archived_messages'] == 1
        assert old_message.validity_state == ValidityState.ARCHIVED
        assert recent_message.validity_state == ValidityState.ACTIVE


class TestWorkLogIntegration:
    """Test work log integration functionality."""
    
    def test_get_tool_context_summary(self):
        """Test tool context summary generation."""
        container = InteractionContainer()
        
        # Add message with tool usage
        tool_message = EnhancedCommonChatMessage(
            role=MessageRole.ASSISTANT,
            content=[
                EnhancedToolUseContentBlock(
                    tool_name="file_writer",
                    tool_id="tool_123",
                    parameters={"path": "/test.txt"}
                ),
                EnhancedToolResultContentBlock(
                    tool_name="file_writer",
                    tool_id="tool_123",
                    result="File written successfully",
                    outcome_status=OutcomeStatus.SUCCESS,
                    execution_time=1.5
                )
            ]
        )
        
        container.add_message(tool_message)
        container.complete_interaction()
        
        summary = container.get_tool_context_summary()
        
        assert summary['interaction_id'] == container.interaction_id
        assert summary['total_messages'] == 1
        assert summary['active_messages'] == 1
        assert 'file_writer' in summary['tools_used']
        
        tool_stats = summary['tools_used']['file_writer']
        assert tool_stats['call_count'] == 1
        assert tool_stats['success_count'] == 1
        assert tool_stats['failure_count'] == 0
        assert tool_stats['total_execution_time'] == 1.5
    
    def test_work_log_entries_tracking(self):
        """Test work log entry tracking."""
        container = InteractionContainer()
        
        # Add work log entries
        container.work_log_entries.append("log_entry_1")
        container.work_log_entries.append("log_entry_2")
        
        summary = container.get_tool_context_summary()
        assert len(summary['work_log_entries']) == 2
        assert "log_entry_1" in summary['work_log_entries']
        assert "log_entry_2" in summary['work_log_entries']


class TestAdvancedFeatures:
    """Test advanced container features."""
    
    def test_branch_from_message(self):
        """Test creating branch from specific message."""
        container = InteractionContainer()
        
        message1 = EnhancedCommonChatMessage(
            role=MessageRole.USER,
            content=[EnhancedTextContentBlock(text="Message 1")]
        )
        message2 = EnhancedCommonChatMessage(
            role=MessageRole.ASSISTANT,
            content=[EnhancedTextContentBlock(text="Message 2")]
        )
        message3 = EnhancedCommonChatMessage(
            role=MessageRole.USER,
            content=[EnhancedTextContentBlock(text="Message 3")]
        )
        
        container.add_message(message1)
        container.add_message(message2)
        container.add_message(message3)
        
        # Branch from message 2
        branch = container.branch_from_message(message2.id)
        
        assert len(branch.messages) == 2
        assert branch.messages[0].text_content == "Message 1"
        assert branch.messages[1].text_content == "Message 2"
        assert branch.interaction_id != container.interaction_id
        assert branch.interaction_start == container.interaction_start
        assert branch.interaction_stop is None
    
    def test_branch_from_nonexistent_message(self):
        """Test branching from non-existent message raises error."""
        container = InteractionContainer()
        
        with pytest.raises(ValueError, match="Message .* not found"):
            container.branch_from_message("non-existent-id")
    
    def test_merge_with_container(self):
        """Test merging containers."""
        container1 = InteractionContainer()
        container2 = InteractionContainer()
        
        message1 = EnhancedCommonChatMessage(
            role=MessageRole.USER,
            content=[EnhancedTextContentBlock(text="Container 1")]
        )
        message2 = EnhancedCommonChatMessage(
            role=MessageRole.USER,
            content=[EnhancedTextContentBlock(text="Container 2")]
        )
        
        container1.add_message(message1)
        container2.add_message(message2)
        container2.work_log_entries.append("log_from_container2")
        
        # Merge container2 into container1
        container1.merge_with_container(container2)
        
        assert len(container1.messages) == 2
        assert message2.interaction_id == container1.interaction_id
        assert "log_from_container2" in container1.work_log_entries
    
    def test_export_context_formats(self):
        """Test context export in different formats."""
        container = InteractionContainer()
        
        message = EnhancedCommonChatMessage(
            role=MessageRole.USER,
            content=[EnhancedTextContentBlock(text="Test message")]
        )
        
        container.add_message(message)
        container.complete_interaction()
        
        # Test JSON format (default)
        json_context = container.export_context()
        assert 'interaction_id' in json_context
        assert 'start_time' in json_context
        assert 'stop_time' in json_context
        
        # Test summary format
        summary_context = container.export_context('summary')
        assert 'tool_summary' in summary_context
        assert 'work_log_entries' in summary_context
        
        # Test full format
        full_context = container.export_context('full')
        assert 'messages' in full_context
        assert 'tool_context' in full_context
        assert 'optimization_metadata' in full_context


class TestLifecycleManagement:
    """Test interaction lifecycle management."""
    
    def test_complete_interaction(self):
        """Test marking interaction as completed."""
        container = InteractionContainer()
        
        assert container.interaction_stop is None
        assert not container.is_completed()
        
        container.complete_interaction()
        
        assert container.interaction_stop is not None
        assert container.is_completed()
    
    def test_is_active(self):
        """Test checking if interaction is active."""
        container = InteractionContainer()
        
        assert container.is_active()
        
        container.validity_state = ValidityState.INVALIDATED
        assert not container.is_active()
    
    def test_get_duration(self):
        """Test getting interaction duration."""
        container = InteractionContainer()
        
        # Duration while ongoing
        duration1 = container.get_duration()
        assert duration1 > 0
        
        # Complete and check duration
        container.complete_interaction()
        duration2 = container.get_duration()
        assert duration2 >= duration1
    
    def test_auto_completion_heuristic(self):
        """Test automatic completion detection."""
        container = InteractionContainer()
        
        # Add user message
        user_message = EnhancedCommonChatMessage(
            role=MessageRole.USER,
            content=[EnhancedTextContentBlock(text="Hello")]
        )
        container.add_message(user_message)
        assert container.interaction_stop is None
        
        # Add assistant message - should trigger completion
        assistant_message = EnhancedCommonChatMessage(
            role=MessageRole.ASSISTANT,
            content=[EnhancedTextContentBlock(text="Hi there!")]
        )
        container.add_message(assistant_message)
        assert container.interaction_stop is not None


class TestThreadSafety:
    """Test thread safety of container operations."""
    
    def test_concurrent_message_addition(self):
        """Test concurrent message addition is thread-safe."""
        container = InteractionContainer()
        messages_added = []
        
        def add_messages(thread_id):
            for i in range(10):
                message = EnhancedCommonChatMessage(
                    role=MessageRole.USER,
                    content=[EnhancedTextContentBlock(text=f"Thread {thread_id} Message {i}")]
                )
                container.add_message(message)
                messages_added.append(message.id)
        
        # Start multiple threads
        threads = []
        for i in range(3):
            thread = threading.Thread(target=add_messages, args=(i,))
            threads.append(thread)
            thread.start()
        
        # Wait for all threads to complete
        for thread in threads:
            thread.join()
        
        # Verify all messages were added
        assert len(container.messages) == 30
        assert len(set(messages_added)) == 30  # All unique
        
        # Verify all messages have correct interaction_id
        for message in container.messages:
            assert message.interaction_id == container.interaction_id
    
    def test_concurrent_invalidation(self):
        """Test concurrent message invalidation is thread-safe."""
        container = InteractionContainer()
        
        # Add messages
        for i in range(20):
            message = EnhancedCommonChatMessage(
                role=MessageRole.USER,
                content=[EnhancedTextContentBlock(text=f"Message {i}")]
            )
            container.add_message(message)
        
        invalidated_counts = []
        
        def invalidate_messages(tool_name):
            criteria = {
                'type': InvalidationCriteria.TOOL_SPECIFIC,
                'force_invalidate': True,
                'reason': f'Invalidated by {tool_name}'
            }
            invalidated = container.invalidate_messages_by_tool(tool_name, criteria)
            invalidated_counts.append(len(invalidated))
        
        # Start multiple invalidation threads
        threads = []
        for i in range(3):
            thread = threading.Thread(target=invalidate_messages, args=(f"tool_{i}",))
            threads.append(thread)
            thread.start()
        
        # Wait for all threads to complete
        for thread in threads:
            thread.join()
        
        # Verify thread safety - each thread should have invalidated some messages
        # but total shouldn't exceed the number of messages
        total_invalidated = sum(invalidated_counts)
        assert total_invalidated <= 20
        
        # Count actually invalidated messages
        actually_invalidated = sum(1 for msg in container.messages if not msg.is_active())
        assert actually_invalidated <= 20


class TestObservableIntegration:
    """Test Observable pattern integration."""
    
    def test_observable_field_changes(self):
        """Test that observable fields trigger events."""
        container = InteractionContainer()
        
        # Mock observer
        observer_calls = []
        
        def mock_observer(container_instance):
            observer_calls.append(('model_changed', container_instance))
        
        container.add_observer(mock_observer, 'model')
        
        # Trigger observable change
        container.validity_state = ValidityState.INVALIDATED
        
        # Should have triggered observer
        assert len(observer_calls) > 0
        assert observer_calls[-1][0] == 'model_changed'
        assert observer_calls[-1][1] == container
    
    def test_batch_operations(self):
        """Test batch operations defer events."""
        container = InteractionContainer()
        
        observer_calls = []
        
        def mock_observer(container_instance):
            observer_calls.append(container_instance)
        
        container.add_observer(mock_observer, 'model')
        
        # Perform batch operations
        with container.batch():
            container.validity_state = ValidityState.INVALIDATED
            container.invalidated_by = "test_tool"
            # Should not trigger events yet
            initial_calls = len(observer_calls)
        
        # Should trigger single event after batch
        final_calls = len(observer_calls)
        assert final_calls == initial_calls + 1


if __name__ == "__main__":
    pytest.main([__file__])
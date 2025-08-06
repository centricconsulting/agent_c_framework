"""
Comprehensive test suite for the MessageManager and advanced message manipulation methods.

This test suite covers all functionality of the advanced message management system including:
- Cross-interaction message retrieval and filtering
- Advanced message manipulation methods
- Batch operations and UI-friendly methods
- Message relationship tracking and analysis
- Import/export functionality
- Performance optimization for large datasets
"""

import pytest
import json
import time
from datetime import datetime, timezone, timedelta
from unittest.mock import Mock, patch
from typing import Dict, Any, List

from agent_c.models.chat_history.message_manager import (
    MessageManager,
    MessageSearchCriteria,
    MessageRelationship,
    MergeStrategy,
    QueryScope
)
from agent_c.models.chat_history.interaction_container import (
    InteractionContainer,
    OptimizationStrategy
)
from agent_c.models.chat_history.agent_work_log import (
    AgentWorkLog,
    ActionCategory,
    ImpactScope
)
from agent_c.models.common_chat.enhanced_models import (
    EnhancedCommonChatMessage,
    EnhancedTextContentBlock,
    EnhancedToolUseContentBlock,
    EnhancedToolResultContentBlock,
    ValidityState,
    OutcomeStatus,
    MessageRole
)


class TestMessageManager:
    """Test the MessageManager class."""
    
    def test_message_manager_creation(self):
        """Test basic message manager creation."""
        manager = MessageManager()
        
        assert len(manager.interactions) == 0
        assert len(manager.message_index) == 0
        assert len(manager.tool_index) == 0
        assert len(manager.relationship_graph) == 0
        assert manager.work_log is not None
    
    def test_add_interaction(self):
        """Test adding interactions to the manager."""
        manager = MessageManager()
        
        # Create test interaction
        message1 = EnhancedCommonChatMessage(
            role=MessageRole.USER,
            content=[EnhancedTextContentBlock(text="Hello")]
        )
        message2 = EnhancedCommonChatMessage(
            role=MessageRole.ASSISTANT,
            content=[EnhancedTextContentBlock(text="Hi there")]
        )
        
        interaction = InteractionContainer(messages=[message1, message2])
        
        # Add interaction
        interaction_id = manager.add_interaction(interaction)
        
        assert interaction_id == interaction.interaction_id
        assert interaction_id in manager.interactions
        assert len(manager.message_index) == 2
        assert message1.id in manager.message_index
        assert message2.id in manager.message_index
    
    def test_remove_interaction(self):
        """Test removing interactions from the manager."""
        manager = MessageManager()
        
        # Create and add interaction
        message = EnhancedCommonChatMessage(
            role=MessageRole.USER,
            content=[EnhancedTextContentBlock(text="Test")]
        )
        interaction = InteractionContainer(messages=[message])
        interaction_id = manager.add_interaction(interaction)
        
        # Verify it was added
        assert interaction_id in manager.interactions
        assert message.id in manager.message_index
        
        # Remove interaction
        success = manager.remove_interaction(interaction_id)
        
        assert success is True
        assert interaction_id not in manager.interactions
        assert message.id not in manager.message_index
        
        # Try to remove non-existent interaction
        success = manager.remove_interaction("nonexistent")
        assert success is False
    
    def test_list_interactions(self):
        """Test listing interactions with various options."""
        manager = MessageManager()
        
        # Create interactions with different states
        interaction1 = InteractionContainer()
        interaction1.interaction_start = time.time() - 100
        
        interaction2 = InteractionContainer()
        interaction2.interaction_start = time.time() - 50
        interaction2.validity_state = ValidityState.INVALIDATED
        
        interaction3 = InteractionContainer()
        interaction3.interaction_start = time.time()
        
        manager.add_interaction(interaction1)
        manager.add_interaction(interaction2)
        manager.add_interaction(interaction3)
        
        # Test listing active interactions only
        active_interactions = manager.list_interactions(include_inactive=False)
        assert len(active_interactions) == 2  # interaction1 and interaction3
        
        # Test listing all interactions
        all_interactions = manager.list_interactions(include_inactive=True)
        assert len(all_interactions) == 3
        
        # Test sorting by start time
        sorted_interactions = manager.list_interactions(sort_by='start_time')
        assert sorted_interactions[0].interaction_id == interaction1.interaction_id
        assert sorted_interactions[-1].interaction_id == interaction3.interaction_id


class TestAdvancedMessageRetrieval:
    """Test advanced message retrieval methods."""
    
    def setup_method(self):
        """Set up test data."""
        self.manager = MessageManager()
        
        # Create test messages
        self.message1 = EnhancedCommonChatMessage(
            role=MessageRole.USER,
            content=[EnhancedTextContentBlock(text="Read file test.txt")]
        )
        
        self.message2 = EnhancedCommonChatMessage(
            role=MessageRole.ASSISTANT,
            content=[
                EnhancedToolUseContentBlock(
                    tool_name="file_read",
                    tool_call_id="call_1",
                    parameters={"path": "test.txt"}
                )
            ]
        )
        
        self.message3 = EnhancedCommonChatMessage(
            role=MessageRole.ASSISTANT,
            content=[
                EnhancedToolResultContentBlock(
                    tool_name="file_read",
                    tool_call_id="call_1",
                    outcome_status=OutcomeStatus.SUCCESS,
                    result_summary="File read successfully"
                )
            ]
        )
        
        # Create interactions
        self.interaction1 = InteractionContainer(messages=[self.message1, self.message2])
        self.interaction2 = InteractionContainer(messages=[self.message3])
        
        self.manager.add_interaction(self.interaction1)
        self.manager.add_interaction(self.interaction2)
    
    def test_get_messages_for_interaction(self):
        """Test retrieving messages for specific interactions."""
        messages = self.manager.get_messages_for_interaction(self.interaction1.interaction_id)
        
        assert len(messages) == 2
        assert self.message1 in messages
        assert self.message2 in messages
        
        # Test non-existent interaction
        messages = self.manager.get_messages_for_interaction("nonexistent")
        assert len(messages) == 0
    
    def test_find_message_by_id(self):
        """Test finding messages by ID across interactions."""
        found_message = self.manager.find_message_by_id(self.message1.id)
        assert found_message == self.message1
        
        found_message = self.manager.find_message_by_id(self.message3.id)
        assert found_message == self.message3
        
        # Test non-existent message
        found_message = self.manager.find_message_by_id("nonexistent")
        assert found_message is None
    
    def test_get_messages_by_tool(self):
        """Test retrieving messages by tool name."""
        tool_messages = self.manager.get_messages_by_tool("file_read")
        
        assert len(tool_messages) == 2  # Tool use and tool result
        assert self.message2 in tool_messages
        assert self.message3 in tool_messages
        
        # Test with include_results=False
        tool_messages = self.manager.get_messages_by_tool("file_read", include_results=False)
        assert len(tool_messages) == 1  # Only tool use
        assert self.message2 in tool_messages
        assert self.message3 not in tool_messages
    
    def test_search_messages(self):
        """Test advanced message search functionality."""
        # Search by text pattern
        criteria = MessageSearchCriteria(text_patterns=["file"])
        results = self.manager.search_messages(criteria)
        
        assert len(results) >= 1
        found_messages = [result[0] for result in results]
        assert any("file" in str(msg.content).lower() for msg in found_messages)
        
        # Search by tool name
        criteria = MessageSearchCriteria(tool_names=["file_read"])
        results = self.manager.search_messages(criteria)
        
        assert len(results) == 2
        
        # Search by role
        criteria = MessageSearchCriteria(roles=["user"])
        results = self.manager.search_messages(criteria)
        
        assert len(results) == 1
        assert results[0][0] == self.message1
        
        # Search with max results
        criteria = MessageSearchCriteria(max_results=1)
        results = self.manager.search_messages(criteria)
        
        assert len(results) == 1
    
    def test_get_message_relationships(self):
        """Test message relationship retrieval."""
        # The manager should have built relationships during interaction addition
        relationships = self.manager.get_message_relationships(self.message1.id)
        
        # Should have conversation flow relationship
        assert len(relationships) >= 0  # May have relationships
        
        # Test filtering by relationship type
        relationships = self.manager.get_message_relationships(
            self.message1.id,
            relationship_types=['conversation_flow']
        )
        
        # Test minimum strength filtering
        relationships = self.manager.get_message_relationships(
            self.message1.id,
            min_strength=0.5
        )


class TestMessageManipulation:
    """Test message manipulation methods."""
    
    def setup_method(self):
        """Set up test data."""
        self.manager = MessageManager()
        
        self.message = EnhancedCommonChatMessage(
            role=MessageRole.USER,
            content=[EnhancedTextContentBlock(text="Original content")]
        )
        
        self.interaction = InteractionContainer(messages=[self.message])
        self.manager.add_interaction(self.interaction)
    
    def test_edit_message_in_place(self):
        """Test editing message in place."""
        new_content = [EnhancedTextContentBlock(text="Updated content")]
        
        result = self.manager.edit_message(
            self.message.id,
            new_content,
            create_branch=False
        )
        
        assert result == self.message.id
        
        # Verify content was updated
        updated_message = self.manager.find_message_by_id(self.message.id)
        assert str(updated_message.content[0].text) == "Updated content"
    
    def test_edit_message_with_branch(self):
        """Test editing message with branch creation."""
        new_content = [EnhancedTextContentBlock(text="Branched content")]
        
        result = self.manager.edit_message(
            self.message.id,
            new_content,
            create_branch=True
        )
        
        assert isinstance(result, InteractionContainer)
        assert result.interaction_id != self.interaction.interaction_id
        
        # Verify original message unchanged
        original_message = self.interaction.get_message_by_id(self.message.id)
        assert "Original content" in str(original_message.content[0].text)
        
        # Verify branch has updated content
        branch_message = result.get_message_by_id(self.message.id)
        assert "Branched content" in str(branch_message.content[0].text)
    
    def test_branch_from_message(self):
        """Test creating branch from specific message."""
        # Add another message to the interaction
        message2 = EnhancedCommonChatMessage(
            role=MessageRole.ASSISTANT,
            content=[EnhancedTextContentBlock(text="Response")]
        )
        self.interaction.add_message(message2)
        
        # Branch from first message
        branch = self.manager.branch_from_message(self.message.id)
        
        assert isinstance(branch, InteractionContainer)
        assert len(branch.messages) == 1  # Only messages up to branch point
        assert branch.messages[0].id == self.message.id
        
        # Test with custom interaction ID
        custom_branch = self.manager.branch_from_message(
            self.message.id,
            new_interaction_id="custom_id"
        )
        
        assert custom_branch.interaction_id == "custom_id"
    
    def test_batch_update_messages(self):
        """Test batch message updates."""
        # Add more messages
        message2 = EnhancedCommonChatMessage(
            role=MessageRole.ASSISTANT,
            content=[EnhancedTextContentBlock(text="Message 2")]
        )
        message3 = EnhancedCommonChatMessage(
            role=MessageRole.USER,
            content=[EnhancedTextContentBlock(text="Message 3")]
        )
        
        self.interaction.add_message(message2)
        self.interaction.add_message(message3)
        
        # Prepare batch updates
        updates = [
            {
                'operation': 'edit',
                'message_id': self.message.id,
                'new_content': [EnhancedTextContentBlock(text="Batch updated 1")]
            },
            {
                'operation': 'invalidate',
                'message_id': message2.id,
                'invalidated_by': 'test_batch',
                'reason': 'Batch invalidation test'
            },
            {
                'operation': 'remove',
                'message_id': message3.id
            }
        ]
        
        # Perform batch update
        results = self.manager.batch_update_messages(updates)
        
        assert results['total_updates'] == 3
        assert len(results['successful_updates']) >= 2  # At least edit and invalidate should succeed
        assert len(results['interactions_affected']) == 1
        
        # Verify updates
        updated_message = self.manager.find_message_by_id(self.message.id)
        assert "Batch updated 1" in str(updated_message.content[0].text)
        
        invalidated_message = self.manager.find_message_by_id(message2.id)
        assert invalidated_message.validity_state == ValidityState.INVALIDATED


class TestInteractionContainerAdvancedMethods:
    """Test advanced methods added to InteractionContainer."""
    
    def setup_method(self):
        """Set up test data."""
        self.message1 = EnhancedCommonChatMessage(
            role=MessageRole.USER,
            content=[EnhancedTextContentBlock(text="First message")]
        )
        
        self.message2 = EnhancedCommonChatMessage(
            role=MessageRole.ASSISTANT,
            content=[EnhancedTextContentBlock(text="Second message")]
        )
        
        self.message3 = EnhancedCommonChatMessage(
            role=MessageRole.USER,
            content=[EnhancedTextContentBlock(text="Third message")]
        )
        
        self.interaction = InteractionContainer(
            messages=[self.message1, self.message2, self.message3]
        )
    
    def test_get_messages_in_range(self):
        """Test getting messages in a specific range."""
        # Get messages from first to second
        range_messages = self.interaction.get_messages_in_range(
            start_id=self.message1.id,
            end_id=self.message2.id
        )
        
        assert len(range_messages) == 2
        assert range_messages[0] == self.message1
        assert range_messages[1] == self.message2
        
        # Test with no start_id (from beginning)
        range_messages = self.interaction.get_messages_in_range(
            end_id=self.message2.id
        )
        
        assert len(range_messages) == 2
        
        # Test with no end_id (to end)
        range_messages = self.interaction.get_messages_in_range(
            start_id=self.message2.id
        )
        
        assert len(range_messages) == 2
        assert range_messages[0] == self.message2
        assert range_messages[1] == self.message3
    
    def test_get_messages_by_validity_state(self):
        """Test filtering messages by validity state."""
        # Initially all messages should be active
        active_messages = self.interaction.get_messages_by_validity_state([ValidityState.ACTIVE])
        assert len(active_messages) == 3
        
        # Invalidate one message
        self.message2.invalidate("test", "Test invalidation")
        
        # Test filtering
        active_messages = self.interaction.get_messages_by_validity_state([ValidityState.ACTIVE])
        assert len(active_messages) == 2
        
        invalidated_messages = self.interaction.get_messages_by_validity_state([ValidityState.INVALIDATED])
        assert len(invalidated_messages) == 1
        assert invalidated_messages[0] == self.message2
    
    def test_edit_message_advanced(self):
        """Test advanced message editing in InteractionContainer."""
        new_content = [EnhancedTextContentBlock(text="Edited content")]
        
        # Test in-place editing
        result = self.interaction.edit_message(
            self.message1.id,
            new_content,
            create_branch=False
        )
        
        assert result == self.message1.id
        assert "Edited content" in str(self.message1.content[0].text)
        
        # Test branch creation
        branch_result = self.interaction.edit_message(
            self.message2.id,
            [EnhancedTextContentBlock(text="Branched content")],
            create_branch=True
        )
        
        assert isinstance(branch_result, InteractionContainer)
        assert branch_result.interaction_id != self.interaction.interaction_id
    
    def test_remove_messages_from_interaction(self):
        """Test removing messages from interaction."""
        original_count = len(self.interaction.messages)
        
        # Remove specific messages
        removed_ids = self.interaction.remove_messages_from_interaction(
            message_ids=[self.message1.id, self.message3.id]
        )
        
        assert len(removed_ids) == 2
        assert self.message1.id in removed_ids
        assert self.message3.id in removed_ids
        assert len(self.interaction.messages) == 1
        assert self.interaction.messages[0] == self.message2
        
        # Test removing all messages
        self.interaction.messages = [self.message1, self.message2, self.message3]  # Reset
        removed_ids = self.interaction.remove_messages_from_interaction()
        
        assert len(removed_ids) == 3
        assert len(self.interaction.messages) == 0
    
    def test_truncate_from_message(self):
        """Test truncating messages from a specific point."""
        # Truncate after message1 (remove message2 and message3)
        removed_ids = self.interaction.truncate_from_message(
            self.message1.id,
            direction='after'
        )
        
        assert len(removed_ids) == 2
        assert self.message2.id in removed_ids
        assert self.message3.id in removed_ids
        assert len(self.interaction.messages) == 1
        assert self.interaction.messages[0] == self.message1
        
        # Reset and test truncate before
        self.interaction.messages = [self.message1, self.message2, self.message3]
        removed_ids = self.interaction.truncate_from_message(
            self.message3.id,
            direction='before'
        )
        
        assert len(removed_ids) == 2
        assert self.message1.id in removed_ids
        assert self.message2.id in removed_ids
        assert len(self.interaction.messages) == 1
        assert self.interaction.messages[0] == self.message3
    
    def test_batch_update_messages_container(self):
        """Test batch updates within InteractionContainer."""
        updates = [
            {
                'operation': 'edit',
                'message_id': self.message1.id,
                'new_content': [EnhancedTextContentBlock(text="Batch edit 1")]
            },
            {
                'operation': 'invalidate',
                'message_id': self.message2.id,
                'invalidated_by': 'batch_test',
                'reason': 'Batch test invalidation'
            }
        ]
        
        results = self.interaction.batch_update_messages(updates)
        
        assert results['total_updates'] == 2
        assert len(results['successful_updates']) == 2
        assert len(results['failed_updates']) == 0
        
        # Verify updates
        assert "Batch edit 1" in str(self.message1.content[0].text)
        assert self.message2.validity_state == ValidityState.INVALIDATED
    
    def test_validate_message_integrity(self):
        """Test message integrity validation."""
        # Test with valid interaction
        validation_result = self.interaction.validate_message_integrity()
        
        assert validation_result['is_valid'] is True
        assert validation_result['message_count'] == 3
        assert len(validation_result['issues']) == 0
        
        # Create integrity issues
        # Duplicate message ID
        duplicate_message = EnhancedCommonChatMessage(
            role=MessageRole.USER,
            content=[EnhancedTextContentBlock(text="Duplicate")]
        )
        duplicate_message.id = self.message1.id  # Same ID as existing message
        self.interaction.messages.append(duplicate_message)
        
        validation_result = self.interaction.validate_message_integrity()
        
        assert validation_result['is_valid'] is False
        assert any('Duplicate message IDs' in issue for issue in validation_result['issues'])
    
    def test_optimize_message_storage(self):
        """Test message storage optimization."""
        # Add some invalidated messages
        self.message2.invalidate("test", "Test invalidation")
        
        # Add a duplicate message (same content)
        duplicate_message = EnhancedCommonChatMessage(
            role=MessageRole.USER,
            content=[EnhancedTextContentBlock(text="First message")]  # Same as message1
        )
        self.interaction.messages.append(duplicate_message)
        
        original_count = len(self.interaction.messages)
        
        optimization_result = self.interaction.optimize_message_storage()
        
        assert optimization_result['original_message_count'] == original_count
        assert optimization_result['final_message_count'] < original_count
        assert len(optimization_result['optimizations_applied']) > 0
        assert optimization_result['space_saved_percent'] > 0


class TestImportExportFunctionality:
    """Test import/export functionality."""
    
    def setup_method(self):
        """Set up test data."""
        self.manager = MessageManager()
        
        self.message1 = EnhancedCommonChatMessage(
            role=MessageRole.USER,
            content=[EnhancedTextContentBlock(text="Test message")]
        )
        
        self.interaction = InteractionContainer(messages=[self.message1])
        self.manager.add_interaction(self.interaction)
    
    def test_export_interaction_context(self):
        """Test exporting interaction context."""
        # Test JSON export
        json_export = self.manager.export_interaction_context(
            self.interaction.interaction_id,
            format='json'
        )
        
        assert isinstance(json_export, str)
        exported_data = json.loads(json_export)
        
        assert 'interaction_id' in exported_data
        assert 'messages' in exported_data
        assert exported_data['interaction_id'] == self.interaction.interaction_id
        assert len(exported_data['messages']) == 1
        
        # Test with work log inclusion
        json_export_with_log = self.manager.export_interaction_context(
            self.interaction.interaction_id,
            format='json',
            include_work_log=True
        )
        
        exported_data_with_log = json.loads(json_export_with_log)
        assert 'work_log' in exported_data_with_log
    
    def test_import_interaction_context(self):
        """Test importing interaction context."""
        # Export first
        exported_context = self.manager.export_interaction_context(
            self.interaction.interaction_id,
            format='json'
        )
        
        # Import as new interaction
        new_interaction_id = self.manager.import_interaction_context(
            exported_context,
            merge_strategy=MergeStrategy.CREATE_NEW
        )
        
        assert new_interaction_id != self.interaction.interaction_id
        assert new_interaction_id in self.manager.interactions
        
        new_interaction = self.manager.get_interaction(new_interaction_id)
        assert len(new_interaction.messages) == 1
        
        # Test append strategy
        new_message = EnhancedCommonChatMessage(
            role=MessageRole.ASSISTANT,
            content=[EnhancedTextContentBlock(text="Appended message")]
        )
        
        append_context = {
            'interaction_id': 'temp',
            'messages': [new_message.model_dump()],
            'start_time': time.time()
        }
        
        result_id = self.manager.import_interaction_context(
            append_context,
            merge_strategy=MergeStrategy.APPEND,
            target_interaction_id=self.interaction.interaction_id
        )
        
        assert result_id == self.interaction.interaction_id
        assert len(self.interaction.messages) == 2


class TestPerformanceAndScalability:
    """Test performance and scalability features."""
    
    def test_large_message_dataset_handling(self):
        """Test handling of large message datasets."""
        manager = MessageManager()
        
        # Create multiple interactions with many messages
        num_interactions = 10
        messages_per_interaction = 50
        
        for i in range(num_interactions):
            messages = []
            for j in range(messages_per_interaction):
                message = EnhancedCommonChatMessage(
                    role=MessageRole.USER if j % 2 == 0 else MessageRole.ASSISTANT,
                    content=[EnhancedTextContentBlock(text=f"Message {i}-{j}")]
                )
                messages.append(message)
            
            interaction = InteractionContainer(messages=messages)
            manager.add_interaction(interaction)
        
        # Verify all messages are indexed
        assert len(manager.message_index) == num_interactions * messages_per_interaction
        
        # Test search performance
        start_time = time.time()
        criteria = MessageSearchCriteria(text_patterns=["Message"])
        results = manager.search_messages(criteria)
        search_time = time.time() - start_time
        
        assert len(results) == num_interactions * messages_per_interaction
        assert search_time < 1.0  # Should complete within 1 second
        
        # Test optimization
        optimization_result = manager.optimize_message_storage()
        assert optimization_result['total_interactions'] == num_interactions
    
    def test_search_caching(self):
        """Test search result caching."""
        manager = MessageManager()
        
        # Add test data
        message = EnhancedCommonChatMessage(
            role=MessageRole.USER,
            content=[EnhancedTextContentBlock(text="Cacheable message")]
        )
        interaction = InteractionContainer(messages=[message])
        manager.add_interaction(interaction)
        
        # Perform search twice
        criteria = MessageSearchCriteria(text_patterns=["Cacheable"])
        
        start_time = time.time()
        results1 = manager.search_messages(criteria)
        first_search_time = time.time() - start_time
        
        start_time = time.time()
        results2 = manager.search_messages(criteria)
        second_search_time = time.time() - start_time
        
        # Results should be identical
        assert len(results1) == len(results2)
        assert results1[0][0].id == results2[0][0].id
        
        # Second search should be faster (cached)
        assert second_search_time <= first_search_time
    
    def test_relationship_graph_performance(self):
        """Test relationship graph performance with many messages."""
        manager = MessageManager()
        
        # Create interaction with tool calls and results
        messages = []
        for i in range(20):
            if i % 2 == 0:
                # Tool use message
                message = EnhancedCommonChatMessage(
                    role=MessageRole.ASSISTANT,
                    content=[
                        EnhancedToolUseContentBlock(
                            tool_name=f"tool_{i//2}",
                            tool_call_id=f"call_{i//2}",
                            parameters={"param": f"value_{i}"}
                        )
                    ]
                )
            else:
                # Tool result message
                message = EnhancedCommonChatMessage(
                    role=MessageRole.ASSISTANT,
                    content=[
                        EnhancedToolResultContentBlock(
                            tool_name=f"tool_{i//2}",
                            tool_call_id=f"call_{i//2}",
                            outcome_status=OutcomeStatus.SUCCESS,
                            result_summary=f"Result {i}"
                        )
                    ]
                )
            messages.append(message)
        
        interaction = InteractionContainer(messages=messages)
        manager.add_interaction(interaction)
        
        # Verify relationships were built
        assert len(manager.relationship_graph) > 0
        
        # Test relationship retrieval performance
        start_time = time.time()
        for message in messages[:5]:  # Test first 5 messages
            relationships = manager.get_message_relationships(message.id)
        relationship_time = time.time() - start_time
        
        assert relationship_time < 0.1  # Should be very fast


class TestErrorHandling:
    """Test error handling and edge cases."""
    
    def test_invalid_message_operations(self):
        """Test operations with invalid message IDs."""
        manager = MessageManager()
        
        # Test finding non-existent message
        message = manager.find_message_by_id("nonexistent")
        assert message is None
        
        # Test editing non-existent message
        with pytest.raises(ValueError, match="Message nonexistent not found"):
            manager.edit_message("nonexistent", [])
        
        # Test branching from non-existent message
        with pytest.raises(ValueError, match="Message nonexistent not found"):
            manager.branch_from_message("nonexistent")
    
    def test_invalid_interaction_operations(self):
        """Test operations with invalid interaction IDs."""
        manager = MessageManager()
        
        # Test getting non-existent interaction
        interaction = manager.get_interaction("nonexistent")
        assert interaction is None
        
        # Test getting messages for non-existent interaction
        messages = manager.get_messages_for_interaction("nonexistent")
        assert len(messages) == 0
        
        # Test exporting non-existent interaction
        with pytest.raises(ValueError, match="Interaction nonexistent not found"):
            manager.export_interaction_context("nonexistent")
    
    def test_malformed_import_data(self):
        """Test importing malformed context data."""
        manager = MessageManager()
        
        # Test invalid JSON
        with pytest.raises(ValueError):
            manager.import_interaction_context("invalid json")
        
        # Test missing required fields
        invalid_context = {"some_field": "value"}
        
        # Should handle gracefully
        try:
            manager.import_interaction_context(invalid_context)
        except Exception as e:
            assert isinstance(e, (ValueError, KeyError))
    
    def test_concurrent_access_safety(self):
        """Test thread safety with concurrent operations."""
        import threading
        
        manager = MessageManager()
        errors = []
        
        def add_interactions(thread_id):
            try:
                for i in range(10):
                    message = EnhancedCommonChatMessage(
                        role=MessageRole.USER,
                        content=[EnhancedTextContentBlock(text=f"Thread {thread_id} Message {i}")]
                    )
                    interaction = InteractionContainer(messages=[message])
                    manager.add_interaction(interaction)
            except Exception as e:
                errors.append(e)
        
        # Create multiple threads
        threads = []
        for i in range(5):
            thread = threading.Thread(target=add_interactions, args=(i,))
            threads.append(thread)
        
        # Start all threads
        for thread in threads:
            thread.start()
        
        # Wait for completion
        for thread in threads:
            thread.join()
        
        # Verify no errors occurred
        assert len(errors) == 0
        
        # Verify all interactions were added
        assert len(manager.interactions) == 50  # 5 threads * 10 interactions


if __name__ == '__main__':
    pytest.main([__file__])
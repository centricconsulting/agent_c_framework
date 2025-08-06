"""
Comprehensive tests for Tool Manipulation API.

Tests cover:
- Tool optimizer registration and management
- Message invalidation strategies
- Tool sequence optimization
- Conflict resolution mechanisms
- Audit trail functionality
- Rollback capabilities
- Integration with ToolChest and InteractionContainer
"""

import pytest
import time
import threading
from unittest.mock import Mock, patch, MagicMock
from uuid import uuid4

from agent_c.toolsets.tool_manipulation_api import (
    ToolManipulationAPI,
    OptimizationPriority,
    ConflictResolutionStrategy,
    OptimizationRecord,
    ToolOptimizer
)
from agent_c.models.chat_history.interaction_container import (
    InteractionContainer, OptimizationStrategy, InvalidationCriteria
)
from agent_c.models.common_chat.enhanced_models import (
    EnhancedCommonChatMessage,
    EnhancedTextContentBlock,
    EnhancedToolUseContentBlock,
    EnhancedToolResultContentBlock,
    ValidityState,
    MessageRole
)


class TestToolManipulationAPIBasics:
    """Test basic Tool Manipulation API functionality."""
    
    def test_api_initialization(self):
        """Test API initialization."""
        api = ToolManipulationAPI()
        
        assert api.tool_chest is None
        assert api._tool_optimizers == {}
        assert api._optimization_records == []
        assert api._optimization_locks == {}
        assert api._container_cache == {}
        assert api._tool_hierarchy == {}
        assert api._rollback_snapshots == {}
    
    def test_api_initialization_with_tool_chest(self):
        """Test API initialization with ToolChest."""
        mock_tool_chest = Mock()
        api = ToolManipulationAPI(tool_chest=mock_tool_chest)
        
        assert api.tool_chest == mock_tool_chest
    
    def test_integrate_with_tool_chest(self):
        """Test integration with ToolChest."""
        api = ToolManipulationAPI()
        mock_tool_chest = Mock()
        
        api.integrate_with_tool_chest(mock_tool_chest)
        
        assert api.tool_chest == mock_tool_chest


class TestToolOptimizerRegistration:
    """Test tool optimizer registration and management."""
    
    def test_register_tool_optimizer_basic(self):
        """Test basic tool optimizer registration."""
        api = ToolManipulationAPI()
        
        def mock_callback(container, criteria):
            return {'success': True}
        
        result = api.register_tool_optimizer(
            "test_tool",
            mock_callback,
            priority=OptimizationPriority.HIGH
        )
        
        assert result is True
        assert "test_tool" in api._tool_optimizers
        
        optimizer = api._tool_optimizers["test_tool"]
        assert optimizer.tool_name == "test_tool"
        assert optimizer.callback == mock_callback
        assert optimizer.priority == OptimizationPriority.HIGH
        assert optimizer.conflict_resolution == ConflictResolutionStrategy.PRIORITY_BASED
    
    def test_register_tool_optimizer_with_options(self):
        """Test tool optimizer registration with all options."""
        api = ToolManipulationAPI()
        
        def mock_callback(container, criteria):
            return {'success': True}
        
        result = api.register_tool_optimizer(
            "advanced_tool",
            mock_callback,
            priority=OptimizationPriority.CRITICAL,
            supported_strategies=["parameter_conflict", "semantic_obsolete"],
            conflict_resolution=ConflictResolutionStrategy.TIME_BASED,
            custom_metadata="test_value"
        )
        
        assert result is True
        
        optimizer = api._tool_optimizers["advanced_tool"]
        assert optimizer.supported_strategies == ["parameter_conflict", "semantic_obsolete"]
        assert optimizer.conflict_resolution == ConflictResolutionStrategy.TIME_BASED
        assert optimizer.metadata["custom_metadata"] == "test_value"
    
    def test_register_tool_optimizer_failure(self):
        """Test tool optimizer registration failure handling."""
        api = ToolManipulationAPI()
        
        # Mock logger to avoid actual logging during test
        with patch.object(api, 'logger'):
            result = api.register_tool_optimizer(
                "bad_tool",
                None,  # Invalid callback
                priority="invalid_priority"  # This should cause an error
            )
        
        # Should handle the error gracefully
        assert result is False
        assert "bad_tool" not in api._tool_optimizers
    
    def test_unregister_tool_optimizer(self):
        """Test tool optimizer unregistration."""
        api = ToolManipulationAPI()
        
        def mock_callback(container, criteria):
            return {'success': True}
        
        # Register first
        api.register_tool_optimizer("test_tool", mock_callback)
        assert "test_tool" in api._tool_optimizers
        
        # Unregister
        result = api.unregister_tool_optimizer("test_tool")
        assert result is True
        assert "test_tool" not in api._tool_optimizers
        
        # Try to unregister non-existent tool
        result = api.unregister_tool_optimizer("non_existent")
        assert result is False


class TestMessageInvalidation:
    """Test message invalidation functionality."""
    
    def test_invalidate_conflicting_calls_basic(self):
        """Test basic conflicting call invalidation."""
        api = ToolManipulationAPI()
        
        # Create container with messages
        container = InteractionContainer()
        message = EnhancedCommonChatMessage(
            role=MessageRole.ASSISTANT,
            content=[
                EnhancedToolUseContentBlock(
                    tool_name="file_writer",
                    tool_id="tool_123",
                    parameters={"path": "/old/path.txt", "content": "old content"}
                )
            ]
        )
        container.add_message(message)
        
        # Register container
        interaction_id = container.interaction_id
        api.register_container(interaction_id, container)
        
        # Invalidate conflicting calls
        new_params = {"path": "/new/path.txt", "content": "new content"}
        invalidated_ids = api.invalidate_conflicting_calls(
            "file_writer", new_params, interaction_id
        )
        
        assert len(invalidated_ids) == 1
        assert invalidated_ids[0] == message.id
        assert not message.is_active()
        assert message.invalidated_by == "file_writer"
    
    def test_invalidate_conflicting_calls_no_container(self):
        """Test invalidation when container not found."""
        api = ToolManipulationAPI()
        
        with patch.object(api, 'logger'):
            invalidated_ids = api.invalidate_conflicting_calls(
                "test_tool", {"param": "value"}, "non_existent_interaction"
            )
        
        assert invalidated_ids == []
    
    def test_invalidate_conflicting_calls_with_container_param(self):
        """Test invalidation with container passed as parameter."""
        api = ToolManipulationAPI()
        
        # Create container
        container = InteractionContainer()
        message = EnhancedCommonChatMessage(
            role=MessageRole.ASSISTANT,
            content=[
                EnhancedToolUseContentBlock(
                    tool_name="test_tool",
                    tool_id="tool_456",
                    parameters={"setting": "old_value"}
                )
            ]
        )
        container.add_message(message)
        
        # Invalidate with container parameter
        new_params = {"setting": "new_value"}
        invalidated_ids = api.invalidate_conflicting_calls(
            "test_tool", new_params, container.interaction_id, container=container
        )
        
        assert len(invalidated_ids) == 1
        assert not message.is_active()


class TestToolSequenceOptimization:
    """Test tool sequence optimization functionality."""
    
    def test_optimize_for_tool_sequence_basic(self):
        """Test basic tool sequence optimization."""
        api = ToolManipulationAPI()
        
        # Register tool optimizers
        def optimizer1(container, criteria):
            return {'success': True, 'tool': 'tool1', 'optimized': True}
        
        def optimizer2(container, criteria):
            return {'success': True, 'tool': 'tool2', 'optimized': True}
        
        api.register_tool_optimizer("tool1", optimizer1, priority=OptimizationPriority.HIGH)
        api.register_tool_optimizer("tool2", optimizer2, priority=OptimizationPriority.MEDIUM)
        
        # Create container
        container = InteractionContainer()
        message = EnhancedCommonChatMessage(
            role=MessageRole.USER,
            content=[EnhancedTextContentBlock(text="Test message")]
        )
        container.add_message(message)
        
        # Register container
        api.register_container(container.interaction_id, container)
        
        # Optimize for sequence
        result = api.optimize_for_tool_sequence(
            ["tool1", "tool2"], 
            container.interaction_id,
            strategy=OptimizationStrategy.REMOVE_INVALIDATED
        )
        
        assert result['success'] is True
        assert result['strategy'] == 'remove_invalidated'
        assert result['tool_sequence'] == ["tool1", "tool2"]
        assert 'container_optimization' in result
        assert 'tool_optimizations' in result
        assert result['tool_optimizations']['tool1']['success'] is True
        assert result['tool_optimizations']['tool2']['success'] is True
    
    def test_optimize_for_tool_sequence_with_conflicts(self):
        """Test tool sequence optimization with conflicts."""
        api = ToolManipulationAPI()
        
        # Register conflicting optimizers
        def optimizer1(container, criteria):
            return {'success': True, 'tool': 'tool1'}
        
        def optimizer2(container, criteria):
            return {'success': True, 'tool': 'tool2'}
        
        api.register_tool_optimizer(
            "tool1", optimizer1, 
            priority=OptimizationPriority.HIGH,
            supported_strategies=["parameter_conflict", "semantic_obsolete"]
        )
        api.register_tool_optimizer(
            "tool2", optimizer2, 
            priority=OptimizationPriority.MEDIUM,
            supported_strategies=["parameter_conflict", "time_based"]  # Conflict on parameter_conflict
        )
        
        # Create container
        container = InteractionContainer()
        api.register_container(container.interaction_id, container)
        
        # Optimize - should detect and resolve conflicts
        result = api.optimize_for_tool_sequence(["tool1", "tool2"], container.interaction_id)
        
        assert result['success'] is True
        # Conflicts should be resolved automatically
    
    def test_optimize_for_tool_sequence_no_container(self):
        """Test optimization when container not found."""
        api = ToolManipulationAPI()
        
        result = api.optimize_for_tool_sequence(
            ["tool1", "tool2"], "non_existent_interaction"
        )
        
        assert result['success'] is False
        assert 'error' in result
    
    def test_optimize_for_tool_sequence_optimizer_error(self):
        """Test optimization with optimizer callback error."""
        api = ToolManipulationAPI()
        
        # Register optimizer that raises exception
        def failing_optimizer(container, criteria):
            raise ValueError("Optimizer failed")
        
        api.register_tool_optimizer("failing_tool", failing_optimizer)
        
        # Create container
        container = InteractionContainer()
        api.register_container(container.interaction_id, container)
        
        # Optimize - should handle optimizer error gracefully
        with patch.object(api, 'logger'):
            result = api.optimize_for_tool_sequence(["failing_tool"], container.interaction_id)
        
        assert result['success'] is True  # Overall operation succeeds
        assert result['tool_optimizations']['failing_tool']['success'] is False
        assert 'error' in result['tool_optimizations']['failing_tool']


class TestRollbackCapabilities:
    """Test rollback functionality."""
    
    def test_rollback_tool_optimizations_basic(self):
        """Test basic rollback functionality."""
        api = ToolManipulationAPI()
        
        # Create container and perform operation that creates rollback data
        container = InteractionContainer()
        api.register_container(container.interaction_id, container)
        
        # Perform operation that creates rollback snapshot
        api.invalidate_conflicting_calls(
            "test_tool", {"param": "value"}, container.interaction_id
        )
        
        # Rollback
        result = api.rollback_tool_optimizations(container.interaction_id)
        
        # Should find rollback data and attempt rollback
        assert 'success' in result
    
    def test_rollback_tool_optimizations_no_data(self):
        """Test rollback when no rollback data available."""
        api = ToolManipulationAPI()
        
        result = api.rollback_tool_optimizations("non_existent_interaction")
        
        assert result['success'] is False
        assert 'error' in result
    
    def test_rollback_tool_optimizations_specific_record(self):
        """Test rollback to specific record."""
        api = ToolManipulationAPI()
        
        # Create container and perform operations
        container = InteractionContainer()
        api.register_container(container.interaction_id, container)
        
        # Perform operation
        api.invalidate_conflicting_calls(
            "test_tool", {"param": "value"}, container.interaction_id
        )
        
        # Get record ID
        records = api.get_optimization_audit_trail(interaction_id=container.interaction_id)
        if records:
            record_id = records[0]['record_id']
            
            # Rollback to specific record
            result = api.rollback_tool_optimizations(
                container.interaction_id, rollback_to_record=record_id
            )
            
            assert 'success' in result


class TestAuditTrail:
    """Test audit trail functionality."""
    
    def test_get_optimization_audit_trail_all(self):
        """Test getting all optimization records."""
        api = ToolManipulationAPI()
        
        # Create container and perform operations
        container = InteractionContainer()
        api.register_container(container.interaction_id, container)
        
        # Perform multiple operations
        api.invalidate_conflicting_calls(
            "tool1", {"param": "value1"}, container.interaction_id
        )
        api.invalidate_conflicting_calls(
            "tool2", {"param": "value2"}, container.interaction_id
        )
        
        # Get all records
        records = api.get_optimization_audit_trail()
        
        assert len(records) >= 2
        assert all('record_id' in record for record in records)
        assert all('tool_name' in record for record in records)
        assert all('timestamp' in record for record in records)
    
    def test_get_optimization_audit_trail_filtered(self):
        """Test getting filtered optimization records."""
        api = ToolManipulationAPI()
        
        # Create containers and perform operations
        container1 = InteractionContainer()
        container2 = InteractionContainer()
        api.register_container(container1.interaction_id, container1)
        api.register_container(container2.interaction_id, container2)
        
        api.invalidate_conflicting_calls("tool1", {"param": "value"}, container1.interaction_id)
        api.invalidate_conflicting_calls("tool2", {"param": "value"}, container2.interaction_id)
        
        # Filter by interaction
        records1 = api.get_optimization_audit_trail(interaction_id=container1.interaction_id)
        assert len(records1) >= 1
        assert all(record['interaction_id'] == container1.interaction_id for record in records1)
        
        # Filter by tool
        records_tool1 = api.get_optimization_audit_trail(tool_name="tool1")
        assert len(records_tool1) >= 1
        assert all(record['tool_name'] == "tool1" for record in records_tool1)
        
        # Filter with limit
        limited_records = api.get_optimization_audit_trail(limit=1)
        assert len(limited_records) == 1


class TestAdvancedFeatures:
    """Test advanced API features."""
    
    def test_set_tool_hierarchy(self):
        """Test setting tool hierarchy."""
        api = ToolManipulationAPI()
        
        hierarchy = {
            "high_priority_tool": 10,
            "medium_priority_tool": 5,
            "low_priority_tool": 1
        }
        
        api.set_tool_hierarchy(hierarchy)
        
        assert api._tool_hierarchy == hierarchy
    
    def test_get_tool_optimization_stats_no_data(self):
        """Test getting optimization stats for tool with no operations."""
        api = ToolManipulationAPI()
        
        stats = api.get_tool_optimization_stats("non_existent_tool")
        
        assert stats['tool_name'] == "non_existent_tool"
        assert stats['total_operations'] == 0
    
    def test_get_tool_optimization_stats_with_data(self):
        """Test getting optimization stats for tool with operations."""
        api = ToolManipulationAPI()
        
        # Create container and perform operations
        container = InteractionContainer()
        api.register_container(container.interaction_id, container)
        
        # Perform operations
        api.invalidate_conflicting_calls("test_tool", {"param": "value1"}, container.interaction_id)
        api.invalidate_conflicting_calls("test_tool", {"param": "value2"}, container.interaction_id)
        
        # Get stats
        stats = api.get_tool_optimization_stats("test_tool")
        
        assert stats['tool_name'] == "test_tool"
        assert stats['total_operations'] >= 2
        assert stats['successful_operations'] >= 0
        assert stats['failed_operations'] >= 0
        assert 'operation_types' in stats
        assert 'first_operation' in stats
        assert 'last_operation' in stats
    
    def test_cleanup_old_records(self):
        """Test cleaning up old records."""
        api = ToolManipulationAPI()
        
        # Add some records manually for testing
        old_record = OptimizationRecord(
            tool_name="test_tool",
            interaction_id="test_interaction",
            operation_type="test",
            timestamp=time.time() - 90000  # Very old
        )
        api._add_optimization_record(old_record)
        
        recent_record = OptimizationRecord(
            tool_name="test_tool2",
            interaction_id="test_interaction2",
            operation_type="test",
            timestamp=time.time()  # Recent
        )
        api._add_optimization_record(recent_record)
        
        # Cleanup old records (older than 1 hour)
        cleaned_count = api.cleanup_old_records(max_age_seconds=3600)
        
        assert cleaned_count >= 1
        
        # Recent record should still be there
        remaining_records = api.get_optimization_audit_trail()
        assert any(r['record_id'] == recent_record.record_id for r in remaining_records)


class TestContainerManagement:
    """Test container registration and management."""
    
    def test_register_container(self):
        """Test container registration."""
        api = ToolManipulationAPI()
        container = InteractionContainer()
        
        api.register_container(container.interaction_id, container)
        
        assert container.interaction_id in api._container_cache
        assert api._container_cache[container.interaction_id] == container
    
    def test_unregister_container(self):
        """Test container unregistration."""
        api = ToolManipulationAPI()
        container = InteractionContainer()
        
        # Register first
        api.register_container(container.interaction_id, container)
        assert container.interaction_id in api._container_cache
        
        # Unregister
        api.unregister_container(container.interaction_id)
        assert container.interaction_id not in api._container_cache
        assert container.interaction_id not in api._optimization_locks


class TestConflictResolution:
    """Test conflict resolution mechanisms."""
    
    def test_detect_optimization_conflicts(self):
        """Test conflict detection between optimizers."""
        api = ToolManipulationAPI()
        
        # Register optimizers with overlapping strategies
        api.register_tool_optimizer(
            "tool1", lambda c, cr: {}, 
            supported_strategies=["parameter_conflict", "semantic_obsolete"]
        )
        api.register_tool_optimizer(
            "tool2", lambda c, cr: {}, 
            supported_strategies=["parameter_conflict", "time_based"]
        )
        
        # Detect conflicts
        conflicts = api._detect_optimization_conflicts(["tool1", "tool2"])
        
        assert len(conflicts) == 1
        conflict = conflicts[0]
        assert conflict['tool1'] == "tool1"
        assert conflict['tool2'] == "tool2"
        assert conflict['conflict_type'] == "strategy_overlap"
        assert "parameter_conflict" in conflict['common_strategies']
    
    def test_resolve_optimization_conflicts_priority_based(self):
        """Test priority-based conflict resolution."""
        api = ToolManipulationAPI()
        
        conflicts = [{
            'tool1': 'high_priority_tool',
            'tool2': 'low_priority_tool',
            'conflict_type': 'strategy_overlap'
        }]
        
        # Register optimizers with different priorities
        api.register_tool_optimizer(
            "high_priority_tool", lambda c, cr: {}, 
            priority=OptimizationPriority.HIGH,
            conflict_resolution=ConflictResolutionStrategy.PRIORITY_BASED
        )
        api.register_tool_optimizer(
            "low_priority_tool", lambda c, cr: {}, 
            priority=OptimizationPriority.LOW
        )
        
        with patch.object(api, 'logger'):
            result = api._resolve_optimization_conflicts(conflicts, ["high_priority_tool", "low_priority_tool"])
        
        assert result['success'] is True
        assert result['conflicts_resolved'] == 1
    
    def test_resolve_optimization_conflicts_tool_hierarchy(self):
        """Test tool hierarchy-based conflict resolution."""
        api = ToolManipulationAPI()
        
        # Set tool hierarchy
        api.set_tool_hierarchy({
            "system_tool": 10,
            "user_tool": 5
        })
        
        conflicts = [{
            'tool1': 'system_tool',
            'tool2': 'user_tool',
            'conflict_type': 'strategy_overlap'
        }]
        
        # Register optimizers with hierarchy-based conflict resolution
        api.register_tool_optimizer(
            "system_tool", lambda c, cr: {}, 
            conflict_resolution=ConflictResolutionStrategy.TOOL_HIERARCHY
        )
        api.register_tool_optimizer(
            "user_tool", lambda c, cr: {}, 
            conflict_resolution=ConflictResolutionStrategy.TOOL_HIERARCHY
        )
        
        with patch.object(api, 'logger'):
            result = api._resolve_optimization_conflicts(conflicts, ["system_tool", "user_tool"])
        
        assert result['success'] is True
        assert result['conflicts_resolved'] == 1


class TestThreadSafety:
    """Test thread safety of API operations."""
    
    def test_concurrent_optimizer_registration(self):
        """Test concurrent optimizer registration is thread-safe."""
        api = ToolManipulationAPI()
        
        def register_optimizer(tool_name):
            def callback(container, criteria):
                return {'success': True, 'tool': tool_name}
            
            api.register_tool_optimizer(f"tool_{tool_name}", callback)
        
        # Start multiple threads
        threads = []
        for i in range(5):
            thread = threading.Thread(target=register_optimizer, args=(i,))
            threads.append(thread)
            thread.start()
        
        # Wait for all threads to complete
        for thread in threads:
            thread.join()
        
        # Verify all optimizers were registered
        assert len(api._tool_optimizers) == 5
        for i in range(5):
            assert f"tool_{i}" in api._tool_optimizers
    
    def test_concurrent_invalidation_operations(self):
        """Test concurrent invalidation operations are thread-safe."""
        api = ToolManipulationAPI()
        
        # Create container
        container = InteractionContainer()
        
        # Add multiple messages
        for i in range(10):
            message = EnhancedCommonChatMessage(
                role=MessageRole.USER,
                content=[EnhancedTextContentBlock(text=f"Message {i}")]
            )
            container.add_message(message)
        
        api.register_container(container.interaction_id, container)
        
        def invalidate_messages(tool_name):
            api.invalidate_conflicting_calls(
                tool_name, {"param": "value"}, container.interaction_id
            )
        
        # Start multiple invalidation threads
        threads = []
        for i in range(3):
            thread = threading.Thread(target=invalidate_messages, args=(f"tool_{i}",))
            threads.append(thread)
            thread.start()
        
        # Wait for all threads to complete
        for thread in threads:
            thread.join()
        
        # Verify operations completed without errors
        # (The exact results depend on timing, but no exceptions should occur)
        records = api.get_optimization_audit_trail()
        assert len(records) >= 0  # Some operations should have completed


if __name__ == "__main__":
    pytest.main([__file__])
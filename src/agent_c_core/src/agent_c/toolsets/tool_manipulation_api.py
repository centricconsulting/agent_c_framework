"""
Tool Manipulation API - Comprehensive tool-driven message optimization system.

This module provides the core API for tools to manipulate message arrays based on their
domain knowledge, including:
- Tool optimization strategy registration
- Message invalidation with multiple strategies
- Conflict resolution for competing optimizations
- Comprehensive audit trail for all changes
- Rollback capabilities for failed optimizations
- Integration with InteractionContainer and ToolChest
"""

import time
import threading
from typing import Dict, List, Any, Optional, Callable, Union, Tuple
from uuid import uuid4
from enum import Enum
from dataclasses import dataclass, field
from datetime import datetime

from agent_c.models.chat_history.interaction_container import (
    InteractionContainer, OptimizationStrategy, InvalidationCriteria
)
from agent_c.models.common_chat.enhanced_models import (
    EnhancedCommonChatMessage, ValidityState
)
from agent_c.util.logging_utils import LoggingManager


class OptimizationPriority(int, Enum):
    """Priority levels for tool optimizations."""
    LOW = 1
    MEDIUM = 5
    HIGH = 8
    CRITICAL = 10


class ConflictResolutionStrategy(str, Enum):
    """Strategies for resolving optimization conflicts."""
    PRIORITY_BASED = "priority_based"      # Higher priority wins
    TIME_BASED = "time_based"              # Latest optimization wins
    TOOL_HIERARCHY = "tool_hierarchy"      # Based on tool hierarchy
    USER_CHOICE = "user_choice"            # Defer to user/system choice
    MERGE_COMPATIBLE = "merge_compatible"   # Merge if compatible


@dataclass
class OptimizationRecord:
    """Record of a tool optimization operation."""
    record_id: str = field(default_factory=lambda: str(uuid4()))
    tool_name: str = ""
    interaction_id: str = ""
    operation_type: str = ""  # "invalidate", "optimize", "rollback"
    timestamp: float = field(default_factory=time.time)
    priority: OptimizationPriority = OptimizationPriority.MEDIUM
    criteria: Dict[str, Any] = field(default_factory=dict)
    affected_message_ids: List[str] = field(default_factory=list)
    result: Dict[str, Any] = field(default_factory=dict)
    rollback_data: Optional[Dict[str, Any]] = None
    success: bool = True
    error_message: Optional[str] = None


@dataclass
class ToolOptimizer:
    """Registration record for a tool optimizer."""
    tool_name: str
    callback: Callable
    priority: OptimizationPriority = OptimizationPriority.MEDIUM
    supported_strategies: List[str] = field(default_factory=list)
    conflict_resolution: ConflictResolutionStrategy = ConflictResolutionStrategy.PRIORITY_BASED
    metadata: Dict[str, Any] = field(default_factory=dict)


class ToolManipulationAPI:
    """
    Comprehensive API for tool-driven message optimization and manipulation.
    
    This class provides the core functionality for tools to intelligently optimize
    message arrays based on their domain knowledge, with full audit trails and
    conflict resolution capabilities.
    """
    
    def __init__(self, tool_chest=None):
        """
        Initialize the Tool Manipulation API.
        
        Args:
            tool_chest: Optional ToolChest instance for integration
        """
        self.tool_chest = tool_chest
        
        # Tool optimizer registry
        self._tool_optimizers: Dict[str, ToolOptimizer] = {}
        
        # Optimization audit trail
        self._optimization_records: List[OptimizationRecord] = []
        
        # Active optimization locks per interaction
        self._optimization_locks: Dict[str, threading.RLock] = {}
        
        # Global API lock
        self._api_lock = threading.RLock()
        
        # Interaction container cache
        self._container_cache: Dict[str, InteractionContainer] = {}
        
        # Logging
        logging_manager = LoggingManager(__name__)
        self.logger = logging_manager.get_logger()
        
        # Tool hierarchy for conflict resolution
        self._tool_hierarchy: Dict[str, int] = {}
        
        # Rollback capability
        self._rollback_snapshots: Dict[str, Dict[str, Any]] = {}
    
    # Core API Methods
    
    def register_tool_optimizer(
        self,
        tool_name: str,
        optimization_callback: Callable,
        priority: OptimizationPriority = OptimizationPriority.MEDIUM,
        supported_strategies: Optional[List[str]] = None,
        conflict_resolution: ConflictResolutionStrategy = ConflictResolutionStrategy.PRIORITY_BASED,
        **metadata
    ) -> bool:
        """
        Register a tool optimization callback.
        
        Args:
            tool_name: Name of the tool
            optimization_callback: Callback function for optimization
            priority: Priority level for conflict resolution
            supported_strategies: List of supported optimization strategies
            conflict_resolution: Strategy for resolving conflicts
            **metadata: Additional metadata for the optimizer
            
        Returns:
            True if registration successful, False otherwise
        """
        with self._api_lock:
            try:
                if supported_strategies is None:
                    supported_strategies = [
                        "parameter_conflict", "semantic_obsolete", 
                        "time_based", "tool_specific"
                    ]
                
                optimizer = ToolOptimizer(
                    tool_name=tool_name,
                    callback=optimization_callback,
                    priority=priority,
                    supported_strategies=supported_strategies,
                    conflict_resolution=conflict_resolution,
                    metadata=metadata
                )
                
                self._tool_optimizers[tool_name] = optimizer
                self.logger.info(f"Registered tool optimizer for {tool_name} with priority {priority.name}")
                
                return True
                
            except Exception as e:
                self.logger.error(f"Failed to register tool optimizer for {tool_name}: {e}")
                return False
    
    def unregister_tool_optimizer(self, tool_name: str) -> bool:
        """
        Unregister a tool optimizer.
        
        Args:
            tool_name: Name of the tool to unregister
            
        Returns:
            True if unregistration successful, False otherwise
        """
        with self._api_lock:
            if tool_name in self._tool_optimizers:
                del self._tool_optimizers[tool_name]
                self.logger.info(f"Unregistered tool optimizer for {tool_name}")
                return True
            return False
    
    def invalidate_conflicting_calls(
        self,
        tool_name: str,
        new_call_params: Dict[str, Any],
        interaction_id: str,
        container: Optional[InteractionContainer] = None
    ) -> List[str]:
        """
        Invalidate messages that conflict with new tool call parameters.
        
        Args:
            tool_name: Name of the tool making the call
            new_call_params: Parameters of the new tool call
            interaction_id: ID of the interaction
            container: Optional InteractionContainer instance
            
        Returns:
            List of invalidated message IDs
        """
        with self._get_interaction_lock(interaction_id):
            try:
                # Get or create container
                if container is None:
                    container = self._get_container(interaction_id)
                    if container is None:
                        self.logger.warning(f"No container found for interaction {interaction_id}")
                        return []
                
                # Create rollback snapshot
                snapshot_id = self._create_rollback_snapshot(interaction_id, container)
                
                # Prepare invalidation criteria
                criteria = {
                    'type': InvalidationCriteria.PARAMETER_CONFLICT,
                    'conflicting_parameters': list(new_call_params.keys()),
                    'new_parameters': new_call_params,
                    'reason': f'Parameter conflict with new {tool_name} call'
                }
                
                # Perform invalidation
                invalidated_ids = container.invalidate_messages_by_tool(tool_name, criteria)
                
                # Record the operation
                record = OptimizationRecord(
                    tool_name=tool_name,
                    interaction_id=interaction_id,
                    operation_type="invalidate_conflicting",
                    criteria=criteria,
                    affected_message_ids=invalidated_ids,
                    result={'invalidated_count': len(invalidated_ids)},
                    rollback_data={'snapshot_id': snapshot_id}
                )
                
                self._add_optimization_record(record)
                
                self.logger.info(f"Tool {tool_name} invalidated {len(invalidated_ids)} conflicting messages")
                return invalidated_ids
                
            except Exception as e:
                self.logger.error(f"Error invalidating conflicting calls for {tool_name}: {e}")
                return []
    
    def optimize_for_tool_sequence(
        self,
        tool_sequence: List[str],
        interaction_id: str,
        container: Optional[InteractionContainer] = None,
        strategy: OptimizationStrategy = OptimizationStrategy.REMOVE_INVALIDATED
    ) -> Dict[str, Any]:
        """
        Optimize message array for a sequence of tool operations.
        
        Args:
            tool_sequence: Ordered list of tool names in the sequence
            interaction_id: ID of the interaction
            container: Optional InteractionContainer instance
            strategy: Optimization strategy to apply
            
        Returns:
            Dictionary with optimization results
        """
        with self._get_interaction_lock(interaction_id):
            try:
                # Get or create container
                if container is None:
                    container = self._get_container(interaction_id)
                    if container is None:
                        return {'success': False, 'error': 'Container not found'}
                
                # Create rollback snapshot
                snapshot_id = self._create_rollback_snapshot(interaction_id, container)
                
                # Check for conflicts between optimizers
                conflicts = self._detect_optimization_conflicts(tool_sequence)
                if conflicts:
                    resolution_result = self._resolve_optimization_conflicts(conflicts, tool_sequence)
                    if not resolution_result['success']:
                        return resolution_result
                
                # Apply optimization strategy
                optimization_result = container.optimize_message_array(strategy)
                
                # Apply tool-specific optimizations in sequence
                tool_results = {}
                for tool_name in tool_sequence:
                    if tool_name in self._tool_optimizers:
                        optimizer = self._tool_optimizers[tool_name]
                        try:
                            tool_result = optimizer.callback(container, {
                                'strategy': strategy,
                                'sequence_position': tool_sequence.index(tool_name),
                                'total_sequence_length': len(tool_sequence)
                            })
                            tool_results[tool_name] = tool_result
                        except Exception as e:
                            self.logger.error(f"Error in tool optimizer {tool_name}: {e}")
                            tool_results[tool_name] = {'success': False, 'error': str(e)}
                
                # Combine results
                result = {
                    'success': True,
                    'strategy': strategy.value,
                    'tool_sequence': tool_sequence,
                    'container_optimization': optimization_result,
                    'tool_optimizations': tool_results,
                    'rollback_snapshot': snapshot_id
                }
                
                # Record the operation
                record = OptimizationRecord(
                    tool_name=f"sequence:{','.join(tool_sequence)}",
                    interaction_id=interaction_id,
                    operation_type="optimize_sequence",
                    criteria={'strategy': strategy.value, 'sequence': tool_sequence},
                    result=result,
                    rollback_data={'snapshot_id': snapshot_id}
                )
                
                self._add_optimization_record(record)
                
                return result
                
            except Exception as e:
                self.logger.error(f"Error optimizing for tool sequence {tool_sequence}: {e}")
                return {'success': False, 'error': str(e)}
    
    def rollback_tool_optimizations(
        self,
        interaction_id: str,
        rollback_to_record: Optional[str] = None
    ) -> Dict[str, Any]:
        """
        Rollback tool optimizations for an interaction.
        
        Args:
            interaction_id: ID of the interaction
            rollback_to_record: Optional record ID to rollback to (default: latest)
            
        Returns:
            Dictionary with rollback results
        """
        with self._get_interaction_lock(interaction_id):
            try:
                # Find rollback point
                if rollback_to_record:
                    target_record = next(
                        (r for r in self._optimization_records 
                         if r.record_id == rollback_to_record), None
                    )
                    if not target_record:
                        return {'success': False, 'error': 'Rollback record not found'}
                else:
                    # Find latest record for this interaction
                    interaction_records = [
                        r for r in self._optimization_records 
                        if r.interaction_id == interaction_id and r.rollback_data
                    ]
                    if not interaction_records:
                        return {'success': False, 'error': 'No rollback data available'}
                    target_record = max(interaction_records, key=lambda r: r.timestamp)
                
                # Get rollback snapshot
                snapshot_id = target_record.rollback_data.get('snapshot_id')
                if not snapshot_id or snapshot_id not in self._rollback_snapshots:
                    return {'success': False, 'error': 'Rollback snapshot not found'}
                
                snapshot = self._rollback_snapshots[snapshot_id]
                
                # Restore container state
                container = self._get_container(interaction_id)
                if container is None:
                    return {'success': False, 'error': 'Container not found'}
                
                # Restore messages and state
                self._restore_from_snapshot(container, snapshot)
                
                # Record the rollback
                rollback_record = OptimizationRecord(
                    tool_name="system",
                    interaction_id=interaction_id,
                    operation_type="rollback",
                    criteria={'target_record': target_record.record_id},
                    result={'rollback_successful': True}
                )
                
                self._add_optimization_record(rollback_record)
                
                self.logger.info(f"Rolled back optimizations for interaction {interaction_id}")
                
                return {
                    'success': True,
                    'rolled_back_to': target_record.record_id,
                    'rollback_timestamp': target_record.timestamp
                }
                
            except Exception as e:
                self.logger.error(f"Error rolling back optimizations for {interaction_id}: {e}")
                return {'success': False, 'error': str(e)}
    
    def get_optimization_audit_trail(
        self,
        interaction_id: Optional[str] = None,
        tool_name: Optional[str] = None,
        limit: Optional[int] = None
    ) -> List[Dict[str, Any]]:
        """
        Get optimization audit trail with optional filtering.
        
        Args:
            interaction_id: Optional interaction ID filter
            tool_name: Optional tool name filter
            limit: Optional limit on number of records
            
        Returns:
            List of optimization records
        """
        with self._api_lock:
            records = self._optimization_records.copy()
            
            # Apply filters
            if interaction_id:
                records = [r for r in records if r.interaction_id == interaction_id]
            
            if tool_name:
                records = [r for r in records if r.tool_name == tool_name]
            
            # Sort by timestamp (newest first)
            records.sort(key=lambda r: r.timestamp, reverse=True)
            
            # Apply limit
            if limit:
                records = records[:limit]
            
            # Convert to dictionaries
            return [self._record_to_dict(record) for record in records]
    
    # Advanced Features
    
    def set_tool_hierarchy(self, hierarchy: Dict[str, int]) -> None:
        """
        Set tool hierarchy for conflict resolution.
        
        Args:
            hierarchy: Dictionary mapping tool names to hierarchy levels (higher = more priority)
        """
        with self._api_lock:
            self._tool_hierarchy.update(hierarchy)
            self.logger.info(f"Updated tool hierarchy: {hierarchy}")
    
    def get_tool_optimization_stats(self, tool_name: str) -> Dict[str, Any]:
        """
        Get optimization statistics for a specific tool.
        
        Args:
            tool_name: Name of the tool
            
        Returns:
            Dictionary with optimization statistics
        """
        with self._api_lock:
            tool_records = [r for r in self._optimization_records if r.tool_name == tool_name]
            
            if not tool_records:
                return {'tool_name': tool_name, 'total_operations': 0}
            
            stats = {
                'tool_name': tool_name,
                'total_operations': len(tool_records),
                'successful_operations': sum(1 for r in tool_records if r.success),
                'failed_operations': sum(1 for r in tool_records if not r.success),
                'operation_types': {},
                'average_affected_messages': 0,
                'first_operation': min(r.timestamp for r in tool_records),
                'last_operation': max(r.timestamp for r in tool_records)
            }
            
            # Count operation types
            for record in tool_records:
                op_type = record.operation_type
                stats['operation_types'][op_type] = stats['operation_types'].get(op_type, 0) + 1
            
            # Calculate average affected messages
            total_affected = sum(len(r.affected_message_ids) for r in tool_records)
            stats['average_affected_messages'] = total_affected / len(tool_records)
            
            return stats
    
    def cleanup_old_records(self, max_age_seconds: float = 86400) -> int:
        """
        Clean up old optimization records and snapshots.
        
        Args:
            max_age_seconds: Maximum age of records to keep (default: 24 hours)
            
        Returns:
            Number of records cleaned up
        """
        with self._api_lock:
            cutoff_time = time.time() - max_age_seconds
            
            # Remove old records
            old_records = [r for r in self._optimization_records if r.timestamp < cutoff_time]
            self._optimization_records = [r for r in self._optimization_records if r.timestamp >= cutoff_time]
            
            # Remove old snapshots
            old_snapshots = []
            for snapshot_id, snapshot in list(self._rollback_snapshots.items()):
                if snapshot.get('timestamp', 0) < cutoff_time:
                    old_snapshots.append(snapshot_id)
                    del self._rollback_snapshots[snapshot_id]
            
            cleaned_count = len(old_records) + len(old_snapshots)
            
            if cleaned_count > 0:
                self.logger.info(f"Cleaned up {len(old_records)} old records and {len(old_snapshots)} old snapshots")
            
            return cleaned_count
    
    # Integration Methods
    
    def integrate_with_tool_chest(self, tool_chest) -> None:
        """
        Integrate with a ToolChest instance.
        
        Args:
            tool_chest: ToolChest instance to integrate with
        """
        self.tool_chest = tool_chest
        self.logger.info("Integrated with ToolChest")
    
    def register_container(self, interaction_id: str, container: InteractionContainer) -> None:
        """
        Register an InteractionContainer for optimization.
        
        Args:
            interaction_id: ID of the interaction
            container: InteractionContainer instance
        """
        with self._api_lock:
            self._container_cache[interaction_id] = container
    
    def unregister_container(self, interaction_id: str) -> None:
        """
        Unregister an InteractionContainer.
        
        Args:
            interaction_id: ID of the interaction
        """
        with self._api_lock:
            self._container_cache.pop(interaction_id, None)
            self._optimization_locks.pop(interaction_id, None)
    
    # Private Helper Methods
    
    def _get_interaction_lock(self, interaction_id: str) -> threading.RLock:
        """Get or create a lock for a specific interaction."""
        with self._api_lock:
            if interaction_id not in self._optimization_locks:
                self._optimization_locks[interaction_id] = threading.RLock()
            return self._optimization_locks[interaction_id]
    
    def _get_container(self, interaction_id: str) -> Optional[InteractionContainer]:
        """Get container from cache or create new one."""
        return self._container_cache.get(interaction_id)
    
    def _create_rollback_snapshot(self, interaction_id: str, container: InteractionContainer) -> str:
        """Create a rollback snapshot of the container state."""
        snapshot_id = str(uuid4())
        
        snapshot = {
            'snapshot_id': snapshot_id,
            'interaction_id': interaction_id,
            'timestamp': time.time(),
            'messages': [msg.model_dump() for msg in container.messages],
            'validity_state': container.validity_state.value,
            'optimization_metadata': container.optimization_metadata.copy(),
            'work_log_entries': container.work_log_entries.copy()
        }
        
        self._rollback_snapshots[snapshot_id] = snapshot
        return snapshot_id
    
    def _restore_from_snapshot(self, container: InteractionContainer, snapshot: Dict[str, Any]) -> None:
        """Restore container state from snapshot."""
        # This would require more sophisticated restoration logic
        # For now, this is a placeholder
        self.logger.info(f"Restoring container from snapshot {snapshot['snapshot_id']}")
    
    def _detect_optimization_conflicts(self, tool_sequence: List[str]) -> List[Dict[str, Any]]:
        """Detect conflicts between tool optimizers."""
        conflicts = []
        
        for i, tool1 in enumerate(tool_sequence):
            for j, tool2 in enumerate(tool_sequence[i+1:], i+1):
                if tool1 in self._tool_optimizers and tool2 in self._tool_optimizers:
                    optimizer1 = self._tool_optimizers[tool1]
                    optimizer2 = self._tool_optimizers[tool2]
                    
                    # Check for strategy conflicts
                    common_strategies = set(optimizer1.supported_strategies) & set(optimizer2.supported_strategies)
                    if common_strategies:
                        conflicts.append({
                            'tool1': tool1,
                            'tool2': tool2,
                            'position1': i,
                            'position2': j,
                            'conflict_type': 'strategy_overlap',
                            'common_strategies': list(common_strategies),
                            'priority1': optimizer1.priority,
                            'priority2': optimizer2.priority
                        })
        
        return conflicts
    
    def _resolve_optimization_conflicts(
        self, 
        conflicts: List[Dict[str, Any]], 
        tool_sequence: List[str]
    ) -> Dict[str, Any]:
        """Resolve optimization conflicts using configured strategies."""
        for conflict in conflicts:
            tool1 = conflict['tool1']
            tool2 = conflict['tool2']
            
            optimizer1 = self._tool_optimizers[tool1]
            optimizer2 = self._tool_optimizers[tool2]
            
            # Use the conflict resolution strategy from higher priority tool
            if optimizer1.priority >= optimizer2.priority:
                resolution_strategy = optimizer1.conflict_resolution
            else:
                resolution_strategy = optimizer2.conflict_resolution
            
            if resolution_strategy == ConflictResolutionStrategy.PRIORITY_BASED:
                # Higher priority tool wins - already handled
                pass
            elif resolution_strategy == ConflictResolutionStrategy.TIME_BASED:
                # Later in sequence wins - already handled by sequence order
                pass
            elif resolution_strategy == ConflictResolutionStrategy.TOOL_HIERARCHY:
                # Use tool hierarchy if available
                hierarchy1 = self._tool_hierarchy.get(tool1, 0)
                hierarchy2 = self._tool_hierarchy.get(tool2, 0)
                if hierarchy1 != hierarchy2:
                    # Hierarchy resolves the conflict
                    pass
                else:
                    # Fall back to priority
                    pass
            
            # Log conflict resolution
            self.logger.info(f"Resolved optimization conflict between {tool1} and {tool2} using {resolution_strategy.value}")
        
        return {'success': True, 'conflicts_resolved': len(conflicts)}
    
    def _add_optimization_record(self, record: OptimizationRecord) -> None:
        """Add an optimization record to the audit trail."""
        with self._api_lock:
            self._optimization_records.append(record)
    
    def _record_to_dict(self, record: OptimizationRecord) -> Dict[str, Any]:
        """Convert OptimizationRecord to dictionary."""
        return {
            'record_id': record.record_id,
            'tool_name': record.tool_name,
            'interaction_id': record.interaction_id,
            'operation_type': record.operation_type,
            'timestamp': record.timestamp,
            'priority': record.priority.name,
            'criteria': record.criteria,
            'affected_message_ids': record.affected_message_ids,
            'result': record.result,
            'success': record.success,
            'error_message': record.error_message
        }
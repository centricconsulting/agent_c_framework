"""
InteractionContainer - Core implementation for intelligent message management.

This module implements the InteractionContainer class that replaces bare message arrays
with structured interaction-based message management, including:
- Observable pattern integration for real-time UI updates
- Tool manipulation interfaces for message optimization
- Message lifecycle management and validity tracking
- Thread-safe operations for concurrent tool access
- Work log integration for comprehensive auditing
"""

import time
import threading
from typing import List, Optional, Dict, Any, Callable, Union
from uuid import uuid4
from datetime import datetime
from enum import Enum

from pydantic import Field, field_validator
from agent_c.models.observable import ObservableModel, ObservableField
from agent_c.models.common_chat.enhanced_models import (
    EnhancedCommonChatMessage, ValidityState, ContentBlockType,
    EnhancedToolUseContentBlock, EnhancedToolResultContentBlock
)
# Note: AgentWorkLog import will be added after the class definition to avoid circular imports


class OptimizationStrategy(str, Enum):
    """Strategies for message array optimization."""
    REMOVE_INVALIDATED = "remove_invalidated"
    COMPRESS_DUPLICATES = "compress_duplicates"
    ARCHIVE_OLD = "archive_old"
    CONSOLIDATE_TOOL_CALLS = "consolidate_tool_calls"
    CUSTOM = "custom"


class InvalidationCriteria(str, Enum):
    """Criteria for message invalidation."""
    PARAMETER_CONFLICT = "parameter_conflict"
    SEMANTIC_OBSOLETE = "semantic_obsolete"
    TIME_BASED = "time_based"
    TOOL_SPECIFIC = "tool_specific"
    CUSTOM = "custom"


class InteractionContainer(ObservableModel):
    """
    Container for managing messages within a single interaction boundary.
    
    An interaction represents a complete user-input → agent-response cycle.
    This container provides intelligent message management with tool optimization,
    lifecycle tracking, and work log integration.
    """
    
    # Core Fields
    interaction_id: str = ObservableField(
        default_factory=lambda: str(uuid4()),
        description="Unique identifier for this interaction"
    )
    interaction_start: float = ObservableField(
        default_factory=time.time,
        description="Timestamp when the interaction started"
    )
    interaction_stop: Optional[float] = ObservableField(
        default=None,
        description="Timestamp when the interaction completed"
    )
    messages: List[EnhancedCommonChatMessage] = ObservableField(
        default_factory=list,
        description="Messages in this interaction"
    )
    
    # Tool Optimization Fields
    optimization_metadata: Dict[str, Any] = ObservableField(
        default_factory=dict,
        description="Metadata for tool optimization strategies"
    )
    invalidated_by: Optional[str] = ObservableField(
        default=None,
        description="Tool name that invalidated this interaction"
    )
    validity_state: ValidityState = ObservableField(
        default=ValidityState.ACTIVE,
        description="Current validity state of this interaction"
    )
    
    # Work Log Integration
    work_log_entries: List[str] = ObservableField(
        default_factory=list,
        description="References to work log entry IDs"
    )
    
    # Thread Safety
    lock: threading.RLock = Field(
        default_factory=threading.RLock,
        exclude=True,
        description="Thread lock for concurrent access"
    )
    
    # Tool Optimization Callbacks
    optimization_callbacks: Dict[str, Callable] = Field(
        default_factory=dict,
        exclude=True,
        description="Registered tool optimization callbacks"
    )
    
    @field_validator('interaction_id', mode='before')
    @classmethod
    def set_interaction_id_if_empty(cls, v):
        """Set a UUID for interaction_id if not provided."""
        if not v:
            return str(uuid4())
        return v
    
    @field_validator('messages')
    @classmethod
    def validate_messages_have_interaction_id(cls, v, info):
        """Ensure all messages have the correct interaction_id."""
        interaction_id = info.data.get('interaction_id')
        if interaction_id:
            for message in v:
                if message.interaction_id != interaction_id:
                    message.interaction_id = interaction_id
        return v
    
    def __init__(self, **data: Any) -> None:
        """Initialize InteractionContainer with thread safety."""
        super().__init__(**data)
        
        # Ensure all messages have the correct interaction_id
        for message in self.messages:
            if message.interaction_id != self.interaction_id:
                message.interaction_id = self.interaction_id
    
    # Core Message Management Methods
    
    def add_message(
        self, 
        message: EnhancedCommonChatMessage, 
        position: Optional[int] = None
    ) -> None:
        """
        Add a message to this interaction.
        
        Args:
            message: The message to add
            position: Optional position to insert at (default: append)
        """
        with self.lock:
            # Ensure message has correct interaction_id
            message.interaction_id = self.interaction_id
            
            if position is None:
                self.messages.append(message)
            else:
                self.messages.insert(position, message)
            
            # Update interaction timing
            if self.interaction_stop is None and len(self.messages) > 1:
                # If we have multiple messages and no stop time, this might be completion
                self._maybe_update_stop_time()
    
    def remove_message(self, message_id: str) -> bool:
        """
        Remove a message by ID.
        
        Args:
            message_id: ID of the message to remove
            
        Returns:
            True if message was found and removed, False otherwise
        """
        with self.lock:
            for i, message in enumerate(self.messages):
                if message.id == message_id:
                    del self.messages[i]
                    return True
            return False
    
    def get_active_messages(self) -> List[EnhancedCommonChatMessage]:
        """
        Get all active (non-invalidated) messages.
        
        Returns:
            List of active messages
        """
        with self.lock:
            return [msg for msg in self.messages if msg.is_active()]
    
    def get_all_messages(self, include_invalidated: bool = False) -> List[EnhancedCommonChatMessage]:
        """
        Get all messages, optionally including invalidated ones.
        
        Args:
            include_invalidated: Whether to include invalidated messages
            
        Returns:
            List of messages
        """
        with self.lock:
            if include_invalidated:
                return self.messages.copy()
            else:
                return self.get_active_messages()
    
    def get_message_by_id(self, message_id: str) -> Optional[EnhancedCommonChatMessage]:
        """
        Get a message by its ID.
        
        Args:
            message_id: ID of the message to find
            
        Returns:
            Message if found, None otherwise
        """
        with self.lock:
            for message in self.messages:
                if message.id == message_id:
                    return message
            return None
    
    def get_messages_by_tool(
        self, 
        tool_name: str, 
        include_results: bool = True
    ) -> List[EnhancedCommonChatMessage]:
        """
        Get all messages related to a specific tool.
        
        Args:
            tool_name: Name of the tool
            include_results: Whether to include tool result messages
            
        Returns:
            List of messages related to the tool
        """
        with self.lock:
            tool_messages = []
            
            for message in self.messages:
                message_tools = message.get_tool_names()
                if tool_name in message_tools:
                    # Check if we should include this message based on content
                    if include_results:
                        tool_messages.append(message)
                    else:
                        # Only include if it has tool use blocks (not just results)
                        has_tool_use = any(
                            isinstance(block, EnhancedToolUseContentBlock) 
                            and block.tool_name == tool_name
                            for block in message.content
                        )
                        if has_tool_use:
                            tool_messages.append(message)
            
            return tool_messages
    
    # Tool Manipulation Interface
    
    def invalidate_messages_by_tool(
        self, 
        tool_name: str, 
        criteria: Dict[str, Any]
    ) -> List[str]:
        """
        Invalidate messages based on tool-specific criteria.
        
        Args:
            tool_name: Name of the tool performing invalidation
            criteria: Criteria for invalidation
            
        Returns:
            List of invalidated message IDs
        """
        with self.lock:
            invalidated_ids = []
            criteria_type = criteria.get('type', InvalidationCriteria.TOOL_SPECIFIC)
            reason = criteria.get('reason', f'Invalidated by {tool_name}')
            
            for message in self.messages:
                if not message.is_active():
                    continue
                
                should_invalidate = False
                
                if criteria_type == InvalidationCriteria.PARAMETER_CONFLICT:
                    should_invalidate = self._check_parameter_conflict(message, tool_name, criteria)
                elif criteria_type == InvalidationCriteria.SEMANTIC_OBSOLETE:
                    should_invalidate = self._check_semantic_obsolete(message, tool_name, criteria)
                elif criteria_type == InvalidationCriteria.TIME_BASED:
                    should_invalidate = self._check_time_based(message, criteria)
                elif criteria_type == InvalidationCriteria.TOOL_SPECIFIC:
                    should_invalidate = self._check_tool_specific(message, tool_name, criteria)
                elif criteria_type == InvalidationCriteria.CUSTOM:
                    callback = self.optimization_callbacks.get(tool_name)
                    if callback:
                        should_invalidate = callback(message, criteria)
                
                if should_invalidate:
                    message.invalidate(tool_name, reason)
                    invalidated_ids.append(message.id)
            
            return invalidated_ids
    
    def mark_interaction_superseded(self, superseded_by: str, reason: str) -> None:
        """
        Mark this entire interaction as superseded.
        
        Args:
            superseded_by: ID of the interaction that supersedes this one
            reason: Reason for superseding
        """
        with self.lock:
            self.validity_state = ValidityState.SUPERSEDED
            self.invalidated_by = superseded_by
            
            # Mark all messages as superseded
            for message in self.messages:
                if message.is_active():
                    message.supersede(superseded_by)
    
    def optimize_message_array(self, optimization_strategy: OptimizationStrategy) -> Dict[str, Any]:
        """
        Optimize the message array based on the specified strategy.
        
        Args:
            optimization_strategy: Strategy to use for optimization
            
        Returns:
            Dictionary with optimization results
        """
        with self.lock:
            result = {
                'strategy': optimization_strategy.value,
                'messages_before': len(self.messages),
                'messages_after': 0,
                'actions_taken': []
            }
            
            if optimization_strategy == OptimizationStrategy.REMOVE_INVALIDATED:
                result.update(self._optimize_remove_invalidated())
            elif optimization_strategy == OptimizationStrategy.COMPRESS_DUPLICATES:
                result.update(self._optimize_compress_duplicates())
            elif optimization_strategy == OptimizationStrategy.ARCHIVE_OLD:
                result.update(self._optimize_archive_old())
            elif optimization_strategy == OptimizationStrategy.CONSOLIDATE_TOOL_CALLS:
                result.update(self._optimize_consolidate_tool_calls())
            
            result['messages_after'] = len(self.messages)
            result['optimization_ratio'] = (
                result['messages_before'] - result['messages_after']
            ) / max(result['messages_before'], 1)
            
            # Update optimization metadata
            self.optimization_metadata[f'last_{optimization_strategy.value}'] = {
                'timestamp': time.time(),
                'result': result
            }
            
            return result
    
    def get_tool_context_summary(self) -> Dict[str, Any]:
        """
        Generate a summary of tool context for work log generation.
        
        Returns:
            Dictionary with tool context summary
        """
        with self.lock:
            summary = {
                'interaction_id': self.interaction_id,
                'start_time': self.interaction_start,
                'stop_time': self.interaction_stop,
                'duration': (
                    (self.interaction_stop or time.time()) - self.interaction_start
                ),
                'total_messages': len(self.messages),
                'active_messages': len(self.get_active_messages()),
                'tools_used': {},
                'tool_outcomes': {},
                'work_log_entries': self.work_log_entries.copy()
            }
            
            # Analyze tool usage
            for message in self.messages:
                for tool_name in message.get_tool_names():
                    if tool_name not in summary['tools_used']:
                        summary['tools_used'][tool_name] = {
                            'call_count': 0,
                            'success_count': 0,
                            'failure_count': 0,
                            'total_execution_time': 0.0
                        }
                    
                    # Count tool usage
                    tool_use_blocks = [
                        block for block in message.content
                        if isinstance(block, EnhancedToolUseContentBlock) 
                        and block.tool_name == tool_name
                    ]
                    summary['tools_used'][tool_name]['call_count'] += len(tool_use_blocks)
                    
                    # Count outcomes
                    tool_result_blocks = [
                        block for block in message.content
                        if isinstance(block, EnhancedToolResultContentBlock) 
                        and block.tool_name == tool_name
                    ]
                    
                    for result_block in tool_result_blocks:
                        if result_block.outcome_status.value == 'success':
                            summary['tools_used'][tool_name]['success_count'] += 1
                        elif result_block.outcome_status.value == 'failure':
                            summary['tools_used'][tool_name]['failure_count'] += 1
                        
                        if result_block.execution_time:
                            summary['tools_used'][tool_name]['total_execution_time'] += result_block.execution_time
            
            return summary
    
    # Advanced Methods
    
    def branch_from_message(self, message_id: str) -> 'InteractionContainer':
        """
        Create a new interaction container branching from a specific message.
        
        Args:
            message_id: ID of the message to branch from
            
        Returns:
            New InteractionContainer with messages up to the branch point
        """
        with self.lock:
            branch_index = None
            for i, message in enumerate(self.messages):
                if message.id == message_id:
                    branch_index = i
                    break
            
            if branch_index is None:
                raise ValueError(f"Message {message_id} not found in interaction")
            
            # Create new container with messages up to branch point
            branch_messages = self.messages[:branch_index + 1].copy()
            
            new_container = InteractionContainer(
                messages=branch_messages,
                interaction_start=self.interaction_start,
                # Don't copy stop time - new branch is ongoing
            )
            
            return new_container
    
    def merge_with_container(self, other: 'InteractionContainer') -> None:
        """
        Merge another container's messages into this one.
        
        Args:
            other: Container to merge from
        """
        with self.lock:
            # Update interaction IDs for merged messages
            for message in other.messages:
                message.interaction_id = self.interaction_id
                self.messages.append(message)
            
            # Update timing
            if other.interaction_start < self.interaction_start:
                self.interaction_start = other.interaction_start
            
            if other.interaction_stop and (
                not self.interaction_stop or other.interaction_stop > self.interaction_stop
            ):
                self.interaction_stop = other.interaction_stop
            
            # Merge work log entries
            for entry in other.work_log_entries:
                if entry not in self.work_log_entries:
                    self.work_log_entries.append(entry)
    
    def export_context(self, format: str = 'json') -> Dict[str, Any]:
        """
        Export interaction context in specified format.
        
        Args:
            format: Export format ('json', 'summary', 'full')
            
        Returns:
            Exported context data
        """
        with self.lock:
            base_context = {
                'interaction_id': self.interaction_id,
                'start_time': self.interaction_start,
                'stop_time': self.interaction_stop,
                'validity_state': self.validity_state.value,
                'message_count': len(self.messages),
                'active_message_count': len(self.get_active_messages())
            }
            
            if format == 'summary':
                base_context.update({
                    'tool_summary': self.get_tool_context_summary()['tools_used'],
                    'work_log_entries': len(self.work_log_entries)
                })
            elif format == 'full':
                base_context.update({
                    'messages': [msg.model_dump() for msg in self.messages],
                    'tool_context': self.get_tool_context_summary(),
                    'optimization_metadata': self.optimization_metadata,
                    'work_log_entries': self.work_log_entries
                })
            
            return base_context
    
    # Tool Optimization Registration
    
    def register_tool_optimizer(self, tool_name: str, callback: Callable) -> None:
        """
        Register a custom optimization callback for a tool.
        
        Args:
            tool_name: Name of the tool
            callback: Callback function for optimization
        """
        with self.lock:
            self.optimization_callbacks[tool_name] = callback
    
    def unregister_tool_optimizer(self, tool_name: str) -> None:
        """
        Unregister a tool optimization callback.
        
        Args:
            tool_name: Name of the tool
        """
        with self.lock:
            self.optimization_callbacks.pop(tool_name, None)
    
    # Lifecycle Management
    
    def complete_interaction(self) -> None:
        """Mark this interaction as completed."""
        with self.lock:
            if self.interaction_stop is None:
                self.interaction_stop = time.time()
    
    def is_active(self) -> bool:
        """Check if this interaction is active."""
        return self.validity_state == ValidityState.ACTIVE
    
    def is_completed(self) -> bool:
        """Check if this interaction is completed."""
        return self.interaction_stop is not None
    
    def get_duration(self) -> float:
        """Get interaction duration in seconds."""
        end_time = self.interaction_stop or time.time()
        return end_time - self.interaction_start
    
    # Private Helper Methods
    
    def _maybe_update_stop_time(self) -> None:
        """Update stop time if interaction appears complete."""
        # Simple heuristic: if we have user message followed by assistant message
        if len(self.messages) >= 2:
            last_message = self.messages[-1]
            if last_message.role.value == 'assistant':
                self.interaction_stop = time.time()
    
    def _check_parameter_conflict(
        self, 
        message: EnhancedCommonChatMessage, 
        tool_name: str, 
        criteria: Dict[str, Any]
    ) -> bool:
        """Check if message has parameter conflicts."""
        conflicting_params = criteria.get('conflicting_parameters', [])
        new_params = criteria.get('new_parameters', {})
        
        for block in message.content:
            if (isinstance(block, EnhancedToolUseContentBlock) 
                and block.tool_name == tool_name):
                
                for param in conflicting_params:
                    if (param in block.parameters 
                        and param in new_params 
                        and block.parameters[param] != new_params[param]):
                        return True
        
        return False
    
    def _check_semantic_obsolete(
        self, 
        message: EnhancedCommonChatMessage, 
        tool_name: str, 
        criteria: Dict[str, Any]
    ) -> bool:
        """Check if message is semantically obsolete."""
        obsolete_tools = criteria.get('obsolete_tools', [])
        message_tools = message.get_tool_names()
        
        return any(tool in obsolete_tools for tool in message_tools)
    
    def _check_time_based(
        self, 
        message: EnhancedCommonChatMessage, 
        criteria: Dict[str, Any]
    ) -> bool:
        """Check if message is too old based on time criteria."""
        max_age = criteria.get('max_age_seconds', 3600)  # Default 1 hour
        message_age = time.time() - message.created_at.timestamp()
        
        return message_age > max_age
    
    def _check_tool_specific(
        self, 
        message: EnhancedCommonChatMessage, 
        tool_name: str, 
        criteria: Dict[str, Any]
    ) -> bool:
        """Check tool-specific invalidation criteria."""
        # This is a placeholder for tool-specific logic
        # Each tool would implement its own criteria
        return criteria.get('force_invalidate', False)
    
    def _optimize_remove_invalidated(self) -> Dict[str, Any]:
        """Remove invalidated messages."""
        original_count = len(self.messages)
        self.messages = [msg for msg in self.messages if msg.is_active()]
        removed_count = original_count - len(self.messages)
        
        return {
            'actions_taken': [f'Removed {removed_count} invalidated messages'],
            'removed_messages': removed_count
        }
    
    def _optimize_compress_duplicates(self) -> Dict[str, Any]:
        """Compress duplicate messages."""
        # Placeholder for duplicate compression logic
        return {
            'actions_taken': ['Checked for duplicates'],
            'compressed_messages': 0
        }
    
    def _optimize_archive_old(self) -> Dict[str, Any]:
        """Archive old messages."""
        archived_count = 0
        cutoff_time = time.time() - 86400  # 24 hours ago
        
        for message in self.messages:
            if (message.created_at.timestamp() < cutoff_time 
                and message.is_active()):
                message.archive()
                archived_count += 1
        
        return {
            'actions_taken': [f'Archived {archived_count} old messages'],
            'archived_messages': archived_count
        }
    
    def _optimize_consolidate_tool_calls(self) -> Dict[str, Any]:
        """Consolidate redundant tool calls."""
        # Placeholder for tool call consolidation logic
        return {
            'actions_taken': ['Checked for tool call consolidation opportunities'],
            'consolidated_calls': 0
        }
    
    # Work Log Integration Methods
    
    def generate_work_log_entries(self, work_log: 'AgentWorkLog') -> List[str]:
        """
        Generate work log entries for all tool calls in this interaction.
        
        Args:
            work_log: AgentWorkLog instance to add entries to
            
        Returns:
            List of generated work log entry IDs
        """
        from .agent_work_log import ActionCategory, ImpactScope, OutcomeStatus
        
        with self.lock:
            entry_ids = []
            
            for message in self.messages:
                # Process tool use blocks
                for content_block in message.content:
                    if isinstance(content_block, EnhancedToolUseContentBlock):
                        # Determine action category
                        category = self._categorize_tool_action(content_block.tool_name)
                        
                        # Determine impact scope
                        impact_scope = self._determine_impact_scope(content_block)
                        
                        # Create work log entry
                        entry_id = work_log.log_tool_call(
                            tool_name=content_block.tool_name,
                            parameters=content_block.parameters,
                            interaction_id=self.interaction_id,
                            action_category=category,
                            impact_scope=impact_scope,
                            agent_context={
                                'message_id': message.id,
                                'message_role': message.role.value,
                                'tool_call_id': content_block.tool_call_id
                            }
                        )
                        
                        entry_ids.append(entry_id)
                        
                        # Find corresponding result block and update outcome
                        result_block = self._find_tool_result_block(
                            message, content_block.tool_call_id
                        )
                        
                        if result_block:
                            work_log.update_entry_outcome(
                                entry_id=entry_id,
                                outcome_status=result_block.outcome_status,
                                outcome_details=result_block.result_summary,
                                execution_time_ms=result_block.execution_time,
                                affected_resources=result_block.impact_scope
                            )
            
            # Update work log entry references
            self.work_log_entries.extend(entry_ids)
            
            return entry_ids
    
    def _categorize_tool_action(self, tool_name: str) -> 'ActionCategory':
        """Categorize a tool action based on tool name patterns."""
        from .agent_work_log import ActionCategory
        
        tool_lower = tool_name.lower()
        
        if any(pattern in tool_lower for pattern in ['read', 'get', 'list', 'search', 'find', 'inspect']):
            return ActionCategory.INFORMATION_RETRIEVAL
        elif any(pattern in tool_lower for pattern in ['write', 'create', 'update', 'delete', 'modify', 'replace']):
            return ActionCategory.DATA_MANIPULATION
        elif any(pattern in tool_lower for pattern in ['plan', 'task', 'wsp']):
            return ActionCategory.PLANNING
        elif any(pattern in tool_lower for pattern in ['agent', 'chat', 'oneshot']):
            return ActionCategory.COMMUNICATION
        elif any(pattern in tool_lower for pattern in ['system', 'process', 'execute']):
            return ActionCategory.SYSTEM_OPERATION
        elif any(pattern in tool_lower for pattern in ['analyze', 'audit', 'review']):
            return ActionCategory.ANALYSIS
        elif any(pattern in tool_lower for pattern in ['monitor', 'watch', 'track']):
            return ActionCategory.MONITORING
        else:
            return ActionCategory.EXECUTION
    
    def _determine_impact_scope(self, tool_block: EnhancedToolUseContentBlock) -> 'ImpactScope':
        """Determine the impact scope of a tool action."""
        from .agent_work_log import ImpactScope
        
        tool_name = tool_block.tool_name.lower()
        parameters = tool_block.parameters
        
        # Check for system-wide operations
        if any(pattern in tool_name for pattern in ['system', 'global', 'config']):
            return ImpactScope.SYSTEM
        
        # Check for external operations
        if any(pattern in tool_name for pattern in ['agent', 'chat', 'api', 'external']):
            return ImpactScope.EXTERNAL
        
        # Check for user data operations
        if any(pattern in tool_name for pattern in ['user', 'profile', 'session']):
            return ImpactScope.USER_DATA
        
        # Check parameters for scope hints
        if 'path' in parameters:
            path = str(parameters['path']).lower()
            if any(pattern in path for pattern in ['session', 'user', 'profile']):
                return ImpactScope.SESSION
            elif any(pattern in path for pattern in ['system', 'config', 'global']):
                return ImpactScope.SYSTEM
        
        # Default to local scope
        return ImpactScope.LOCAL
    
    def _find_tool_result_block(
        self, 
        message: EnhancedCommonChatMessage, 
        tool_call_id: str
    ) -> Optional[EnhancedToolResultContentBlock]:
        """Find the tool result block for a given tool call ID."""
        # Check current message first
        for block in message.content:
            if (isinstance(block, EnhancedToolResultContentBlock) 
                and block.tool_call_id == tool_call_id):
                return block
        
        # Check subsequent messages in the interaction
        message_found = False
        for msg in self.messages:
            if message_found:
                for block in msg.content:
                    if (isinstance(block, EnhancedToolResultContentBlock) 
                        and block.tool_call_id == tool_call_id):
                        return block
            elif msg.id == message.id:
                message_found = True
        
        return None
    
    def add_work_log_entry_reference(self, entry_id: str) -> None:
        """Add a reference to a work log entry."""
        with self.lock:
            if entry_id not in self.work_log_entries:
                self.work_log_entries.append(entry_id)
    
    def remove_work_log_entry_reference(self, entry_id: str) -> None:
        """Remove a reference to a work log entry."""
        with self.lock:
            if entry_id in self.work_log_entries:
                self.work_log_entries.remove(entry_id)
    
    def get_work_log_summary(self, work_log: 'AgentWorkLog') -> Dict[str, Any]:
        """Get a summary of work log entries for this interaction."""
        from .agent_work_log import AgentWorkLog
        
        if not isinstance(work_log, AgentWorkLog):
            return {'error': 'Invalid work log instance'}
        
        return work_log.get_interaction_summary(self.interaction_id)
    
    # Advanced Message Manipulation Methods
    
    def get_messages_in_range(
        self,
        start_id: Optional[str] = None,
        end_id: Optional[str] = None,
        filter_criteria: Optional[Dict[str, Any]] = None
    ) -> List[EnhancedCommonChatMessage]:
        """
        Get messages in a specified range with optional filtering.
        
        Args:
            start_id: ID of the first message in range (inclusive)
            end_id: ID of the last message in range (inclusive)
            filter_criteria: Optional filtering criteria
            
        Returns:
            List of messages in the specified range
        """
        with self.lock:
            # Find start and end indices
            start_idx = 0
            end_idx = len(self.messages) - 1
            
            if start_id:
                for i, msg in enumerate(self.messages):
                    if msg.id == start_id:
                        start_idx = i
                        break
                else:
                    raise ValueError(f"Start message ID {start_id} not found")
            
            if end_id:
                for i, msg in enumerate(self.messages):
                    if msg.id == end_id:
                        end_idx = i
                        break
                else:
                    raise ValueError(f"End message ID {end_id} not found")
            
            # Ensure valid range
            if start_idx > end_idx:
                start_idx, end_idx = end_idx, start_idx
            
            # Get messages in range
            range_messages = self.messages[start_idx:end_idx + 1]
            
            # Apply filters if provided
            if filter_criteria:
                range_messages = self._apply_message_filters(range_messages, filter_criteria)
            
            return range_messages
    
    def get_messages_by_validity_state(
        self,
        validity_states: List[ValidityState]
    ) -> List[EnhancedCommonChatMessage]:
        """
        Get messages filtered by validity state.
        
        Args:
            validity_states: List of validity states to include
            
        Returns:
            List of messages matching the validity states
        """
        with self.lock:
            return [
                msg for msg in self.messages
                if msg.validity_state in validity_states
            ]
    
    def edit_message(
        self,
        message_id: str,
        new_content: List[Any],
        create_branch: bool = False,
        preserve_metadata: bool = True
    ) -> Union[str, 'InteractionContainer']:
        """
        Edit a message's content.
        
        Args:
            message_id: ID of the message to edit
            new_content: New content for the message
            create_branch: Whether to create a branch instead of editing in-place
            preserve_metadata: Whether to preserve original metadata
            
        Returns:
            Message ID if edited in-place, or new InteractionContainer if branched
        """
        with self.lock:
            message = self.get_message_by_id(message_id)
            if not message:
                raise ValueError(f"Message {message_id} not found")
            
            if create_branch:
                # Create a branch from this message
                branch_container = self.branch_from_message(message_id)
                
                # Edit the message in the branch
                branch_message = branch_container.get_message_by_id(message_id)
                if branch_message:
                    if preserve_metadata:
                        # Preserve original metadata
                        original_metadata = {
                            'created_at': branch_message.created_at,
                            'role': branch_message.role,
                            'interaction_id': branch_message.interaction_id,
                            'tool_context': branch_message.tool_context
                        }
                    
                    branch_message.content = new_content
                    
                    if preserve_metadata:
                        # Restore preserved metadata
                        for key, value in original_metadata.items():
                            if hasattr(branch_message, key):
                                setattr(branch_message, key, value)
                
                return branch_container
            else:
                # Edit in-place
                if preserve_metadata:
                    # Preserve original metadata
                    original_metadata = {
                        'created_at': message.created_at,
                        'role': message.role,
                        'interaction_id': message.interaction_id,
                        'tool_context': message.tool_context
                    }
                
                message.content = new_content
                message.updated_at = datetime.now()
                
                if preserve_metadata:
                    # Restore preserved metadata
                    for key, value in original_metadata.items():
                        if hasattr(message, key):
                            setattr(message, key, value)
                
                # Trigger observable update
                self.model_changed.emit(
                    field_name="messages",
                    old_value=None,
                    new_value=message
                )
                
                return message.id
    
    def remove_messages_from_interaction(
        self,
        message_ids: Optional[List[str]] = None,
        keep_work_log: bool = True
    ) -> List[str]:
        """
        Remove messages from this interaction.
        
        Args:
            message_ids: Specific message IDs to remove (None = remove all)
            keep_work_log: Whether to preserve work log entries
            
        Returns:
            List of removed message IDs
        """
        with self.lock:
            removed_ids = []
            
            if message_ids is None:
                # Remove all messages
                removed_ids = [msg.id for msg in self.messages]
                self.messages.clear()
            else:
                # Remove specific messages
                messages_to_keep = []
                for message in self.messages:
                    if message.id in message_ids:
                        removed_ids.append(message.id)
                    else:
                        messages_to_keep.append(message)
                self.messages = messages_to_keep
            
            # Update work log references if not keeping work log
            if not keep_work_log:
                self.work_log_entries.clear()
            
            # Trigger observable update
            if removed_ids:
                self.model_changed.emit(
                    field_name="messages",
                    old_value=None,
                    new_value=self.messages
                )
            
            return removed_ids
    
    def truncate_from_message(
        self,
        message_id: str,
        keep_work_log: bool = True,
        direction: str = 'after'
    ) -> List[str]:
        """
        Truncate messages from a specific message.
        
        Args:
            message_id: ID of the message to truncate from
            keep_work_log: Whether to preserve work log entries
            direction: 'after' to remove messages after, 'before' to remove before
            
        Returns:
            List of removed message IDs
        """
        with self.lock:
            message_index = None
            for i, msg in enumerate(self.messages):
                if msg.id == message_id:
                    message_index = i
                    break
            
            if message_index is None:
                raise ValueError(f"Message {message_id} not found")
            
            removed_ids = []
            
            if direction == 'after':
                # Remove messages after the specified message
                removed_messages = self.messages[message_index + 1:]
                removed_ids = [msg.id for msg in removed_messages]
                self.messages = self.messages[:message_index + 1]
            elif direction == 'before':
                # Remove messages before the specified message
                removed_messages = self.messages[:message_index]
                removed_ids = [msg.id for msg in removed_messages]
                self.messages = self.messages[message_index:]
            else:
                raise ValueError(f"Invalid direction: {direction}. Use 'after' or 'before'")
            
            # Update work log references if not keeping work log
            if not keep_work_log:
                self.work_log_entries.clear()
            
            # Trigger observable update
            if removed_ids:
                self.model_changed.emit(
                    field_name="messages",
                    old_value=None,
                    new_value=self.messages
                )
            
            return removed_ids
    
    def batch_update_messages(
        self,
        updates: List[Dict[str, Any]],
        create_snapshot: bool = True
    ) -> Dict[str, Any]:
        """
        Perform batch updates on multiple messages.
        
        Args:
            updates: List of update operations
            create_snapshot: Whether to create a snapshot before updates
            
        Returns:
            Dictionary with update results
        """
        with self.lock:
            if create_snapshot:
                # Create snapshot for rollback
                snapshot = {
                    'messages': [msg.model_copy() for msg in self.messages],
                    'timestamp': time.time()
                }
                self.optimization_metadata['last_batch_snapshot'] = snapshot
            
            results = {
                'successful_updates': [],
                'failed_updates': [],
                'total_updates': len(updates)
            }
            
            for update in updates:
                try:
                    operation = update.get('operation')
                    message_id = update.get('message_id')
                    
                    if operation == 'edit':
                        result = self.edit_message(
                            message_id,
                            update.get('new_content', []),
                            create_branch=update.get('create_branch', False),
                            preserve_metadata=update.get('preserve_metadata', True)
                        )
                        results['successful_updates'].append({
                            'operation': operation,
                            'message_id': message_id,
                            'result': result
                        })
                    
                    elif operation == 'remove':
                        success = self.remove_message(message_id)
                        if success:
                            results['successful_updates'].append({
                                'operation': operation,
                                'message_id': message_id,
                                'result': 'removed'
                            })
                        else:
                            results['failed_updates'].append({
                                'operation': operation,
                                'message_id': message_id,
                                'error': 'Message not found'
                            })
                    
                    elif operation == 'invalidate':
                        message = self.get_message_by_id(message_id)
                        if message:
                            message.invalidate(
                                update.get('invalidated_by', 'batch_operation'),
                                update.get('reason', 'Batch invalidation')
                            )
                            results['successful_updates'].append({
                                'operation': operation,
                                'message_id': message_id,
                                'result': 'invalidated'
                            })
                        else:
                            results['failed_updates'].append({
                                'operation': operation,
                                'message_id': message_id,
                                'error': 'Message not found'
                            })
                    
                    else:
                        results['failed_updates'].append({
                            'operation': operation,
                            'message_id': message_id,
                            'error': f'Unknown operation: {operation}'
                        })
                
                except Exception as e:
                    results['failed_updates'].append({
                        'operation': update.get('operation'),
                        'message_id': update.get('message_id'),
                        'error': str(e)
                    })
            
            # Trigger observable update for batch changes
            self.model_changed.emit(
                field_name="messages",
                old_value=None,
                new_value=self.messages
            )
            
            return results
    
    def validate_message_integrity(self) -> Dict[str, Any]:
        """
        Validate the integrity of all messages in this interaction.
        
        Returns:
            Dictionary with validation results
        """
        with self.lock:
            validation_results = {
                'is_valid': True,
                'issues': [],
                'message_count': len(self.messages),
                'active_message_count': len(self.get_active_messages()),
                'validation_timestamp': time.time()
            }
            
            # Check for duplicate message IDs
            message_ids = [msg.id for msg in self.messages]
            if len(message_ids) != len(set(message_ids)):
                validation_results['is_valid'] = False
                validation_results['issues'].append('Duplicate message IDs found')
            
            # Check interaction ID consistency
            for i, message in enumerate(self.messages):
                if message.interaction_id != self.interaction_id:
                    validation_results['is_valid'] = False
                    validation_results['issues'].append(
                        f'Message {i} has inconsistent interaction_id: {message.interaction_id}'
                    )
            
            # Check message ordering (timestamps should be non-decreasing)
            for i in range(1, len(self.messages)):
                prev_time = self.messages[i-1].created_at
                curr_time = self.messages[i].created_at
                if curr_time < prev_time:
                    validation_results['is_valid'] = False
                    validation_results['issues'].append(
                        f'Message ordering issue at index {i}: timestamps not in order'
                    )
            
            # Check for orphaned tool results
            tool_call_ids = set()
            tool_result_ids = set()
            
            for message in self.messages:
                for block in message.content:
                    if isinstance(block, EnhancedToolUseContentBlock):
                        tool_call_ids.add(block.tool_call_id)
                    elif isinstance(block, EnhancedToolResultContentBlock):
                        tool_result_ids.add(block.tool_call_id)
            
            orphaned_results = tool_result_ids - tool_call_ids
            if orphaned_results:
                validation_results['is_valid'] = False
                validation_results['issues'].append(
                    f'Orphaned tool results found: {list(orphaned_results)}'
                )
            
            # Check for missing tool results
            missing_results = tool_call_ids - tool_result_ids
            if missing_results:
                validation_results['issues'].append(
                    f'Tool calls without results: {list(missing_results)}'
                )
            
            return validation_results
    
    def optimize_message_storage(self) -> Dict[str, Any]:
        """
        Optimize message storage for large message arrays.
        
        Returns:
            Dictionary with optimization results
        """
        with self.lock:
            original_count = len(self.messages)
            optimization_results = {
                'original_message_count': original_count,
                'optimizations_applied': [],
                'space_saved_percent': 0.0,
                'optimization_timestamp': time.time()
            }
            
            # Remove invalidated messages
            active_messages = [msg for msg in self.messages if msg.is_active()]
            removed_invalidated = original_count - len(active_messages)
            if removed_invalidated > 0:
                self.messages = active_messages
                optimization_results['optimizations_applied'].append(
                    f'Removed {removed_invalidated} invalidated messages'
                )
            
            # Compress duplicate content (basic implementation)
            content_hashes = {}
            compressed_messages = []
            duplicates_removed = 0
            
            for message in self.messages:
                # Create a simple hash of message content
                content_str = str(message.content)
                content_hash = hash(content_str)
                
                if content_hash not in content_hashes:
                    content_hashes[content_hash] = message
                    compressed_messages.append(message)
                else:
                    # Check if it's actually a duplicate (not just hash collision)
                    existing_message = content_hashes[content_hash]
                    if str(existing_message.content) == content_str:
                        duplicates_removed += 1
                    else:
                        compressed_messages.append(message)
            
            if duplicates_removed > 0:
                self.messages = compressed_messages
                optimization_results['optimizations_applied'].append(
                    f'Removed {duplicates_removed} duplicate messages'
                )
            
            # Calculate space saved
            final_count = len(self.messages)
            if original_count > 0:
                optimization_results['space_saved_percent'] = (
                    (original_count - final_count) / original_count * 100
                )
            
            optimization_results['final_message_count'] = final_count
            
            # Update optimization metadata
            self.optimization_metadata['last_storage_optimization'] = optimization_results
            
            return optimization_results
    
    def _apply_message_filters(
        self,
        messages: List[EnhancedCommonChatMessage],
        filter_criteria: Dict[str, Any]
    ) -> List[EnhancedCommonChatMessage]:
        """
        Apply filtering criteria to a list of messages.
        
        Args:
            messages: List of messages to filter
            filter_criteria: Filtering criteria
            
        Returns:
            Filtered list of messages
        """
        filtered_messages = messages
        
        # Filter by validity state
        if 'validity_states' in filter_criteria:
            validity_states = filter_criteria['validity_states']
            filtered_messages = [
                msg for msg in filtered_messages
                if msg.validity_state in validity_states
            ]
        
        # Filter by role
        if 'roles' in filter_criteria:
            roles = filter_criteria['roles']
            filtered_messages = [
                msg for msg in filtered_messages
                if msg.role in roles
            ]
        
        # Filter by tool usage
        if 'tools' in filter_criteria:
            tools = filter_criteria['tools']
            filtered_messages = [
                msg for msg in filtered_messages
                if any(tool in msg.get_tool_names() for tool in tools)
            ]
        
        # Filter by time range
        if 'start_time' in filter_criteria:
            start_time = filter_criteria['start_time']
            filtered_messages = [
                msg for msg in filtered_messages
                if msg.created_at >= start_time
            ]
        
        if 'end_time' in filter_criteria:
            end_time = filter_criteria['end_time']
            filtered_messages = [
                msg for msg in filtered_messages
                if msg.created_at <= end_time
            ]
        
        # Filter by content type
        if 'content_types' in filter_criteria:
            content_types = filter_criteria['content_types']
            filtered_messages = [
                msg for msg in filtered_messages
                if any(
                    any(isinstance(block, content_type) for block in msg.content)
                    for content_type in content_types
                )
            ]
        
        return filtered_messages
"""
MessageManager - Advanced message management system for cross-interaction operations.

This module provides comprehensive message management capabilities that work across
multiple InteractionContainer instances, including:
- Cross-interaction message retrieval and filtering
- Batch operations across multiple interactions
- Message relationship tracking and analysis
- Advanced querying and search capabilities
- Import/export functionality for interaction contexts
- Performance optimization for large message datasets
"""

import json
import time
import threading
from typing import Dict, List, Optional, Any, Union, Callable, Tuple
from datetime import datetime, timezone
from enum import Enum
from dataclasses import dataclass
from uuid import uuid4

from pydantic import BaseModel, Field
from ..observable import ObservableModel
from ..common_chat.enhanced_models import (
    EnhancedCommonChatMessage, ValidityState, OutcomeStatus
)
from .interaction_container import InteractionContainer, OptimizationStrategy
from .agent_work_log import AgentWorkLog, ActionCategory, ImpactScope


class MergeStrategy(str, Enum):
    """Strategies for merging interaction contexts."""
    APPEND = "append"                    # Append messages to existing interaction
    REPLACE = "replace"                  # Replace existing interaction
    MERGE_BY_TIMESTAMP = "merge_by_timestamp"  # Merge by message timestamps
    SMART_MERGE = "smart_merge"          # Intelligent merge based on content
    CREATE_NEW = "create_new"            # Create new interaction


class QueryScope(str, Enum):
    """Scope for message queries."""
    SINGLE_INTERACTION = "single_interaction"
    MULTIPLE_INTERACTIONS = "multiple_interactions"
    ALL_INTERACTIONS = "all_interactions"
    ACTIVE_INTERACTIONS = "active_interactions"


@dataclass
class MessageSearchCriteria:
    """Criteria for advanced message searching."""
    text_patterns: Optional[List[str]] = None          # Text patterns to search for
    tool_names: Optional[List[str]] = None             # Tool names to filter by
    roles: Optional[List[str]] = None                  # Message roles to include
    validity_states: Optional[List[ValidityState]] = None  # Validity states
    interaction_ids: Optional[List[str]] = None        # Specific interactions
    start_time: Optional[datetime] = None              # Time range start
    end_time: Optional[datetime] = None                # Time range end
    outcome_statuses: Optional[List[OutcomeStatus]] = None  # Tool outcome statuses
    content_types: Optional[List[str]] = None          # Content block types
    max_results: Optional[int] = None                  # Maximum results to return
    include_context: bool = True                       # Include surrounding context


@dataclass
class MessageRelationship:
    """Represents a relationship between messages."""
    source_message_id: str
    target_message_id: str
    relationship_type: str  # 'tool_call_result', 'conversation_flow', 'reference', etc.
    strength: float        # Relationship strength (0.0 to 1.0)
    metadata: Dict[str, Any]


class MessageManager(ObservableModel):
    """
    Advanced message management system for cross-interaction operations.
    
    Provides comprehensive message management capabilities including advanced
    querying, batch operations, relationship tracking, and performance optimization.
    """
    
    interactions: Dict[str, InteractionContainer] = Field(
        default_factory=dict,
        description="Managed interaction containers"
    )
    
    message_index: Dict[str, str] = Field(
        default_factory=dict,
        description="Message ID to interaction ID mapping"
    )
    
    tool_index: Dict[str, List[str]] = Field(
        default_factory=dict,
        description="Tool name to message ID mapping"
    )
    
    relationship_graph: Dict[str, List[MessageRelationship]] = Field(
        default_factory=dict,
        description="Message relationship graph"
    )
    
    work_log: Optional[AgentWorkLog] = Field(
        default=None,
        description="Associated work log instance"
    )
    
    def __init__(self, **data):
        super().__init__(**data)
        self._lock = threading.RLock()
        self._search_cache = {}
        self._cache_ttl = 300  # 5 minutes
        
        if self.work_log is None:
            from .agent_work_log import create_work_log
            self.work_log = create_work_log()
    
    # Core Management Methods
    
    def add_interaction(self, interaction: InteractionContainer) -> str:
        """
        Add an interaction container to management.
        
        Args:
            interaction: InteractionContainer to add
            
        Returns:
            Interaction ID
        """
        with self._lock:
            interaction_id = interaction.interaction_id
            self.interactions[interaction_id] = interaction
            
            # Update indexes
            self._update_indexes_for_interaction(interaction)
            
            # Generate work log entries
            if self.work_log:
                interaction.generate_work_log_entries(self.work_log)
            
            # Build relationships
            self._build_relationships_for_interaction(interaction)
            
            return interaction_id
    
    def remove_interaction(self, interaction_id: str) -> bool:
        """
        Remove an interaction from management.
        
        Args:
            interaction_id: ID of interaction to remove
            
        Returns:
            True if removed, False if not found
        """
        with self._lock:
            if interaction_id not in self.interactions:
                return False
            
            interaction = self.interactions[interaction_id]
            
            # Remove from indexes
            self._remove_from_indexes(interaction)
            
            # Remove relationships
            self._remove_relationships_for_interaction(interaction_id)
            
            # Remove interaction
            del self.interactions[interaction_id]
            
            # Clear search cache
            self._clear_search_cache()
            
            return True
    
    def get_interaction(self, interaction_id: str) -> Optional[InteractionContainer]:
        """Get an interaction by ID."""
        with self._lock:
            return self.interactions.get(interaction_id)
    
    def list_interactions(
        self,
        include_inactive: bool = False,
        sort_by: str = 'start_time'
    ) -> List[InteractionContainer]:
        """
        List all managed interactions.
        
        Args:
            include_inactive: Whether to include inactive interactions
            sort_by: Field to sort by ('start_time', 'stop_time', 'message_count')
            
        Returns:
            List of interaction containers
        """
        with self._lock:
            interactions = list(self.interactions.values())
            
            if not include_inactive:
                interactions = [i for i in interactions if i.is_active()]
            
            # Sort interactions
            if sort_by == 'start_time':
                interactions.sort(key=lambda i: i.interaction_start)
            elif sort_by == 'stop_time':
                interactions.sort(key=lambda i: i.interaction_stop or float('inf'))
            elif sort_by == 'message_count':
                interactions.sort(key=lambda i: len(i.messages), reverse=True)
            
            return interactions
    
    # Advanced Message Retrieval Methods
    
    def get_messages_for_interaction(
        self,
        interaction_id: str,
        include_invalidated: bool = False
    ) -> List[EnhancedCommonChatMessage]:
        """
        Get all messages for a specific interaction.
        
        Args:
            interaction_id: ID of the interaction
            include_invalidated: Whether to include invalidated messages
            
        Returns:
            List of messages
        """
        with self._lock:
            interaction = self.interactions.get(interaction_id)
            if not interaction:
                return []
            
            return interaction.get_all_messages(include_invalidated=include_invalidated)
    
    def find_message_by_id(self, message_id: str) -> Optional[EnhancedCommonChatMessage]:
        """
        Find a message by ID across all interactions.
        
        Args:
            message_id: ID of the message to find
            
        Returns:
            Message if found, None otherwise
        """
        with self._lock:
            interaction_id = self.message_index.get(message_id)
            if not interaction_id:
                return None
            
            interaction = self.interactions.get(interaction_id)
            if not interaction:
                return None
            
            return interaction.get_message_by_id(message_id)
    
    def get_messages_by_tool(
        self,
        tool_name: str,
        include_results: bool = True,
        scope: QueryScope = QueryScope.ALL_INTERACTIONS
    ) -> List[EnhancedCommonChatMessage]:
        """
        Get messages related to a specific tool.
        
        Args:
            tool_name: Name of the tool
            include_results: Whether to include tool result messages
            scope: Query scope
            
        Returns:
            List of messages related to the tool
        """
        with self._lock:
            tool_messages = []
            
            interactions_to_search = self._get_interactions_for_scope(scope)
            
            for interaction in interactions_to_search:
                messages = interaction.get_messages_by_tool(tool_name, include_results)
                tool_messages.extend(messages)
            
            return tool_messages
    
    def search_messages(
        self,
        criteria: MessageSearchCriteria,
        scope: QueryScope = QueryScope.ALL_INTERACTIONS
    ) -> List[Tuple[EnhancedCommonChatMessage, Dict[str, Any]]]:
        """
        Advanced message search with multiple criteria.
        
        Args:
            criteria: Search criteria
            scope: Query scope
            
        Returns:
            List of tuples (message, context_info)
        """
        with self._lock:
            # Check cache first
            cache_key = self._generate_search_cache_key(criteria, scope)
            cached_result = self._get_cached_search_result(cache_key)
            if cached_result:
                return cached_result
            
            results = []
            interactions_to_search = self._get_interactions_for_scope(scope)
            
            for interaction in interactions_to_search:
                messages = interaction.get_all_messages(include_invalidated=True)
                
                for message in messages:
                    if self._message_matches_criteria(message, criteria):
                        context_info = {
                            'interaction_id': interaction.interaction_id,
                            'message_index': messages.index(message),
                            'total_messages': len(messages),
                            'interaction_start': interaction.interaction_start,
                            'interaction_stop': interaction.interaction_stop
                        }
                        
                        if criteria.include_context:
                            context_info.update(self._get_message_context(message, interaction))
                        
                        results.append((message, context_info))
            
            # Apply result limit
            if criteria.max_results:
                results = results[:criteria.max_results]
            
            # Cache results
            self._cache_search_result(cache_key, results)
            
            return results
    
    def get_message_relationships(
        self,
        message_id: str,
        relationship_types: Optional[List[str]] = None,
        min_strength: float = 0.0
    ) -> List[MessageRelationship]:
        """
        Get relationships for a specific message.
        
        Args:
            message_id: ID of the message
            relationship_types: Types of relationships to include
            min_strength: Minimum relationship strength
            
        Returns:
            List of message relationships
        """
        with self._lock:
            relationships = self.relationship_graph.get(message_id, [])
            
            # Filter by relationship type
            if relationship_types:
                relationships = [
                    rel for rel in relationships
                    if rel.relationship_type in relationship_types
                ]
            
            # Filter by strength
            relationships = [
                rel for rel in relationships
                if rel.strength >= min_strength
            ]
            
            return relationships
    
    # Private Helper Methods
    
    def _update_indexes_for_interaction(self, interaction: InteractionContainer):
        """Update indexes for a new interaction."""
        for message in interaction.messages:
            self.message_index[message.id] = interaction.interaction_id
            
            # Update tool index
            for tool_name in message.get_tool_names():
                if tool_name not in self.tool_index:
                    self.tool_index[tool_name] = []
                if message.id not in self.tool_index[tool_name]:
                    self.tool_index[tool_name].append(message.id)
    
    def _remove_from_indexes(self, interaction: InteractionContainer):
        """Remove interaction from indexes."""
        for message in interaction.messages:
            self.message_index.pop(message.id, None)
            
            # Remove from tool index
            for tool_name in message.get_tool_names():
                if tool_name in self.tool_index:
                    if message.id in self.tool_index[tool_name]:
                        self.tool_index[tool_name].remove(message.id)
                    if not self.tool_index[tool_name]:
                        del self.tool_index[tool_name]
    
    def _build_relationships_for_interaction(self, interaction: InteractionContainer):
        """Build relationships for messages in an interaction."""
        messages = interaction.messages
        
        for i, message in enumerate(messages):
            # Conversation flow relationships
            if i > 0:
                prev_message = messages[i - 1]
                relationship = MessageRelationship(
                    source_message_id=prev_message.id,
                    target_message_id=message.id,
                    relationship_type='conversation_flow',
                    strength=0.8,
                    metadata={'flow_index': i}
                )
                self._add_relationship(relationship)
    
    def _add_relationship(self, relationship: MessageRelationship):
        """Add a relationship to the graph."""
        source_id = relationship.source_message_id
        if source_id not in self.relationship_graph:
            self.relationship_graph[source_id] = []
        self.relationship_graph[source_id].append(relationship)
    
    def _remove_relationships_for_interaction(self, interaction_id: str):
        """Remove all relationships for an interaction."""
        interaction = self.interactions.get(interaction_id)
        if not interaction:
            return
        
        message_ids = {msg.id for msg in interaction.messages}
        
        # Remove relationships where source or target is in this interaction
        for source_id in list(self.relationship_graph.keys()):
            if source_id in message_ids:
                del self.relationship_graph[source_id]
            else:
                # Remove relationships targeting messages in this interaction
                self.relationship_graph[source_id] = [
                    rel for rel in self.relationship_graph[source_id]
                    if rel.target_message_id not in message_ids
                ]
                if not self.relationship_graph[source_id]:
                    del self.relationship_graph[source_id]
    
    def _get_interactions_for_scope(self, scope: QueryScope) -> List[InteractionContainer]:
        """Get interactions based on query scope."""
        if scope == QueryScope.ALL_INTERACTIONS:
            return list(self.interactions.values())
        elif scope == QueryScope.ACTIVE_INTERACTIONS:
            return [i for i in self.interactions.values() if i.is_active()]
        else:
            # For single/multiple interaction scopes, return all for now
            # This would be refined based on specific scope parameters
            return list(self.interactions.values())
    
    def _message_matches_criteria(self, message: EnhancedCommonChatMessage, criteria: MessageSearchCriteria) -> bool:
        """Check if a message matches search criteria."""
        # Text pattern matching
        if criteria.text_patterns:
            message_text = ' '.join(str(block) for block in message.content)
            if not any(pattern.lower() in message_text.lower() for pattern in criteria.text_patterns):
                return False
        
        # Tool name filtering
        if criteria.tool_names:
            message_tools = message.get_tool_names()
            if not any(tool in message_tools for tool in criteria.tool_names):
                return False
        
        # Role filtering
        if criteria.roles:
            if message.role.value not in criteria.roles:
                return False
        
        # Validity state filtering
        if criteria.validity_states:
            if message.validity_state not in criteria.validity_states:
                return False
        
        # Time range filtering
        if criteria.start_time and message.created_at < criteria.start_time:
            return False
        if criteria.end_time and message.created_at > criteria.end_time:
            return False
        
        return True
    
    def _get_message_context(self, message: EnhancedCommonChatMessage, interaction: InteractionContainer) -> Dict[str, Any]:
        """Get context information for a message."""
        message_index = None
        for i, msg in enumerate(interaction.messages):
            if msg.id == message.id:
                message_index = i
                break
        
        context = {
            'previous_messages': [],
            'next_messages': [],
            'tool_calls': [],
            'tool_results': []
        }
        
        if message_index is not None:
            # Get surrounding messages
            start_idx = max(0, message_index - 2)
            end_idx = min(len(interaction.messages), message_index + 3)
            
            context['previous_messages'] = [
                {'id': msg.id, 'role': msg.role.value, 'summary': str(msg.content)[:100]}
                for msg in interaction.messages[start_idx:message_index]
            ]
            
            context['next_messages'] = [
                {'id': msg.id, 'role': msg.role.value, 'summary': str(msg.content)[:100]}
                for msg in interaction.messages[message_index + 1:end_idx]
            ]
        
        # Extract tool information
        for block in message.content:
            if hasattr(block, 'tool_name'):
                if hasattr(block, 'parameters'):  # Tool use block
                    context['tool_calls'].append({
                        'tool_name': block.tool_name,
                        'tool_call_id': getattr(block, 'tool_call_id', None),
                        'parameters': block.parameters
                    })
                else:  # Tool result block
                    context['tool_results'].append({
                        'tool_name': block.tool_name,
                        'tool_call_id': getattr(block, 'tool_call_id', None),
                        'outcome_status': getattr(block, 'outcome_status', None)
                    })
        
        return context
    
    def _generate_search_cache_key(self, criteria: MessageSearchCriteria, scope: QueryScope) -> str:
        """Generate cache key for search results."""
        import hashlib
        
        cache_data = {
            'criteria': criteria.__dict__,
            'scope': scope.value,
            'timestamp': int(time.time() / self._cache_ttl)  # Round to cache TTL
        }
        
        cache_str = json.dumps(cache_data, sort_keys=True, default=str)
        return hashlib.md5(cache_str.encode()).hexdigest()
    
    def _get_cached_search_result(self, cache_key: str) -> Optional[List[Tuple[EnhancedCommonChatMessage, Dict[str, Any]]]]:
        """Get cached search result if valid."""
        if cache_key in self._search_cache:
            cached_data = self._search_cache[cache_key]
            if time.time() - cached_data['timestamp'] < self._cache_ttl:
                return cached_data['results']
            else:
                del self._search_cache[cache_key]
        return None
    
    def _cache_search_result(self, cache_key: str, results: List[Tuple[EnhancedCommonChatMessage, Dict[str, Any]]]):
        """Cache search results."""
        self._search_cache[cache_key] = {
            'results': results,
            'timestamp': time.time()
        }
        
        # Clean old cache entries
        current_time = time.time()
        expired_keys = [
            key for key, data in self._search_cache.items()
            if current_time - data['timestamp'] > self._cache_ttl
        ]
        for key in expired_keys:
            del self._search_cache[key]
    
    def _clear_search_cache(self):
        """Clear the search cache."""
        self._search_cache.clear()
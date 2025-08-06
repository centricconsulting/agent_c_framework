# Intelligent Message Management System - Architecture Design

## Executive Summary

This document defines the complete architecture for the Intelligent Message Management System, which replaces bare message arrays in ChatSession with a sophisticated InteractionContainer system that enables tool-driven message optimization, comprehensive auditing through work logs, and advanced chat management features.

## Current State Analysis

### Existing Architecture
- **ChatSession**: Simple model with `List[dict[str, Any]]` for messages
- **CommonChatMessage**: Well-designed message format with content blocks
- **ObservableModel**: Solid foundation for reactive UI updates
- **ToolChest**: Robust tool management system
- **BaseAgent**: Event-driven agent architecture with streaming callbacks

### Key Limitations
1. **Bare Message Arrays**: No structure for interaction boundaries or message relationships
2. **No Tool Optimization**: Tools cannot optimize message arrays based on domain knowledge
3. **Limited Auditing**: No high-level view of agent actions
4. **No Message Management**: Cannot edit, branch, or delete messages
5. **No Invalidation Logic**: No way to mark messages as superseded or obsolete

## System Architecture Overview

```
┌─────────────────────────────────────────────────────────────────────────────────┐
│                           Intelligent Message Management System                  │
├─────────────────────────────────────────────────────────────────────────────────┤
│                                                                                 │
│  ┌─────────────────┐    ┌─────────────────┐    ┌─────────────────┐            │
│  │                 │    │                 │    │                 │            │
│  │   ChatSession   │◀──▶│InteractionContainer│◀─▶│  AgentWorkLog   │            │
│  │                 │    │                 │    │                 │            │
│  └─────────────────┘    └─────────┬───────┘    └─────────────────┘            │
│                                   │                                            │
│                                   ▼                                            │
│  ┌─────────────────┐    ┌─────────────────┐    ┌─────────────────┐            │
│  │                 │    │                 │    │                 │            │
│  │ Tool Manipulation│◀──▶│CommonChatMessage│◀──▶│Provider Translation│            │
│  │      API        │    │   (Enhanced)    │    │     Layer       │            │
│  │                 │    │                 │    │                 │            │
│  └─────────────────┘    └─────────────────┘    └─────────────────┘            │
│                                                                                 │
└─────────────────────────────────────────────────────────────────────────────────┘
                                       │
                                       ▼
                    ┌─────────────────────────────────────┐
                    │                                     │
                    │        Observable Event System      │
                    │     (UI Updates & Notifications)    │
                    │                                     │
                    └─────────────────────────────────────┘
```

## Core Components Design

### 1. InteractionContainer Class

**Purpose**: Replace bare message arrays with structured interaction-based message management.

**Interface Specification**:
```python
class InteractionContainer(ObservableModel):
    """
    Container for managing messages within a single interaction boundary.
    An interaction represents a complete user-input → agent-response cycle.
    """
    
    # Core Fields
    interaction_id: str = ObservableField(default_factory=lambda: str(uuid4()))
    interaction_start: float = ObservableField(default_factory=time.time)
    interaction_stop: Optional[float] = ObservableField(default=None)
    messages: List[CommonChatMessage] = ObservableField(default_factory=list)
    
    # Tool Optimization Fields
    optimization_metadata: Dict[str, Any] = ObservableField(default_factory=dict)
    invalidated_by: Optional[str] = ObservableField(default=None)  # tool name that invalidated this
    validity_state: ValidityState = ObservableField(default=ValidityState.ACTIVE)
    
    # Work Log Integration
    work_log_entries: List[str] = ObservableField(default_factory=list)  # References to work log IDs
    
    # Core Methods
    def add_message(self, message: CommonChatMessage, position: Optional[int] = None) -> None
    def remove_message(self, message_id: str) -> bool
    def get_active_messages(self) -> List[CommonChatMessage]
    def get_all_messages(self, include_invalidated: bool = False) -> List[CommonChatMessage]
    
    # Tool Manipulation Interface
    def invalidate_by_tool(self, tool_name: str, reason: str) -> None
    def mark_superseded(self, superseded_by: str) -> None
    def optimize_for_tool_sequence(self, tool_sequence: List[str]) -> None
    def get_tool_context_summary(self) -> Dict[str, Any]
    
    # Advanced Methods
    def branch_from_message(self, message_id: str) -> 'InteractionContainer'
    def merge_with_container(self, other: 'InteractionContainer') -> None
    def export_context(self, format: str = 'json') -> Dict[str, Any]
```

### 2. Enhanced CommonChatMessage

**Enhancements**:
```python
class CommonChatMessage(BaseModel):
    # Existing fields...
    id: str
    role: MessageRole
    content: List[ContentBlock]
    created_at: datetime
    provider_metadata: Optional[ProviderMetadata]
    
    # New fields for intelligent management
    interaction_id: str = Field(description="ID of the interaction this message belongs to")
    invalidated_by: Optional[str] = Field(default=None, description="Tool that invalidated this message")
    validity_state: ValidityState = Field(default=ValidityState.ACTIVE)
    tool_context: Optional[Dict[str, Any]] = Field(default=None, description="Tool-specific metadata for work logs")
    superseded_by: Optional[str] = Field(default=None, description="Message ID that supersedes this one")
    
    # Methods
    def is_active(self) -> bool
    def invalidate(self, invalidated_by: str, reason: str) -> None
    def get_work_log_summary(self) -> Dict[str, Any]
```

**ValidityState Enum**:
```python
class ValidityState(str, Enum):
    ACTIVE = "active"           # Message is current and valid
    INVALIDATED = "invalidated" # Message has been invalidated by a tool
    SUPERSEDED = "superseded"   # Message has been replaced by a newer version
    ARCHIVED = "archived"       # Message is kept for history but not active
```

### 3. Tool Manipulation API

**Interface Design**:
```python
class ToolManipulationAPI:
    """
    API for tools to manipulate message containers based on their domain knowledge.
    """
    
    def register_optimizer(self, tool_name: str, optimizer_callback: Callable) -> None
    def invalidate_conflicting_messages(self, tool_name: str, criteria: Dict[str, Any]) -> List[str]
    def optimize_container_for_tool(self, container: InteractionContainer, tool_name: str) -> None
    def rollback_optimizations(self, interaction_id: str, tool_name: str) -> None
    def get_optimization_audit_trail(self, interaction_id: str) -> List[Dict[str, Any]]
    
    # Invalidation Strategies
    def register_invalidation_strategy(self, tool_name: str, strategy: InvalidationStrategy) -> None
    def apply_parameter_based_invalidation(self, tool_name: str, new_params: Dict) -> None
    def apply_semantic_invalidation(self, invalidating_tool: str, target_tools: List[str]) -> None
    def apply_time_based_invalidation(self, tool_name: str, cutoff_time: float) -> None
```

**InvalidationStrategy Types**:
```python
class InvalidationStrategy(BaseModel):
    strategy_type: str  # "parameter_based", "semantic", "time_based", "custom"
    criteria: Dict[str, Any]
    cascade_rules: Optional[List[str]] = None  # Tools that should also be invalidated
    
class ParameterBasedStrategy(InvalidationStrategy):
    conflicting_parameters: List[str]  # Parameters that conflict with previous calls
    
class SemanticStrategy(InvalidationStrategy):
    invalidated_tools: List[str]  # Tools whose results become obsolete
    
class TimeBasedStrategy(InvalidationStrategy):
    max_age_seconds: float  # Maximum age before invalidation
```

### 4. Agent Work Log System

**AgentWorkLog Model**:
```python
class AgentWorkLog(ObservableModel):
    """
    High-level audit trail of agent actions with concise parameter tracking.
    """
    
    # Core Fields
    log_id: str = ObservableField(default_factory=lambda: str(uuid4()))
    interaction_id: str = ObservableField()
    timestamp: float = ObservableField(default_factory=time.time)
    
    # Action Details
    tool_name: str = ObservableField()
    action_summary: str = ObservableField()  # "Updated plan", "Created file", etc.
    key_parameters: Dict[str, Any] = ObservableField(default_factory=dict)
    
    # Outcome Tracking
    outcome_status: OutcomeStatus = ObservableField(default=OutcomeStatus.SUCCESS)
    impact_scope: List[str] = ObservableField(default_factory=list)  # What was affected
    
    # Links
    related_message_ids: List[str] = ObservableField(default_factory=list)
    tool_call_id: Optional[str] = ObservableField(default=None)
    
    # Methods
    def generate_from_tool_call(cls, tool_call: Dict, result: Any) -> 'AgentWorkLog'
    def extract_key_parameters(self, tool_call: Dict) -> Dict[str, Any]
    def determine_impact_scope(self, tool_name: str, parameters: Dict) -> List[str]
```

**OutcomeStatus Enum**:
```python
class OutcomeStatus(str, Enum):
    SUCCESS = "success"
    FAILURE = "failure"
    PARTIAL = "partial"
    PENDING = "pending"
```

### 5. Updated ChatSession Integration

**Enhanced ChatSession**:
```python
class ChatSession(BaseModel):
    # Existing fields...
    session_id: str
    token_count: int
    context_window_size: int
    session_name: Optional[str]
    created_at: Optional[str]
    updated_at: Optional[str]
    deleted_at: Optional[str]
    user_id: Optional[str]
    metadata: Optional[Dict[str, Any]]
    agent_config: Optional[AgentConfiguration]
    
    # Replaced field
    # OLD: messages: List[dict[str, Any]]
    # NEW: interaction_containers: List[InteractionContainer]
    interaction_containers: List[InteractionContainer] = Field(default_factory=list)
    
    # Work Log Integration
    work_logs: List[AgentWorkLog] = Field(default_factory=list)
    
    # Migration Support
    _legacy_messages: Optional[List[dict[str, Any]]] = Field(default=None, exclude=True)
    
    # Methods
    def get_all_active_messages(self) -> List[CommonChatMessage]
    def get_messages_for_provider(self, provider_type: str) -> List[dict]
    def add_interaction(self, interaction: InteractionContainer) -> None
    def get_interaction_by_id(self, interaction_id: str) -> Optional[InteractionContainer]
    def get_work_log_summary(self) -> List[Dict[str, Any]]
    def migrate_from_legacy_messages(self) -> None
```

## Data Flow Diagrams

### 1. Message Creation Flow
```
User Input → Agent → InteractionContainer.add_message() → Observable Event → UI Update
                ↓
            Tool Call → Tool Manipulation API → Message Optimization → Work Log Entry
```

### 2. Tool Optimization Flow
```
Tool Execution → Check Invalidation Rules → Mark Conflicting Messages → Update Container
                                        ↓
                                   Generate Work Log → Trigger Observable Events
```

### 3. Work Log Generation Flow
```
Tool Call → Extract Key Parameters → Generate Action Summary → Create Work Log Entry
                                                          ↓
                                              Link to Interaction Container → Observable Update
```

## Integration Points

### 1. BaseAgent Integration
- **Interaction ID Generation**: BaseAgent generates unique interaction IDs for each user-agent cycle
- **Container Management**: BaseAgent creates and manages InteractionContainer instances
- **Event Integration**: All existing events enhanced with interaction context
- **Tool Call Tracking**: Tool calls linked to originating interactions

### 2. ToolChest Integration
- **Optimization Registration**: Tools register optimization strategies with ToolChest
- **Manipulation API**: ToolChest provides tool manipulation interface
- **Work Log Generation**: Automatic work log creation from tool calls
- **Invalidation Handling**: ToolChest manages message invalidation logic

### 3. Observable Pattern Integration
- **Real-time Updates**: All container changes trigger observable events
- **UI Reactivity**: UI components can subscribe to specific interaction or message changes
- **Event Filtering**: Observable events can be filtered by interaction ID or tool name
- **Batch Operations**: Support for batched updates to minimize event noise

### 4. Provider Translation Layer
- **Enhanced Translation**: Translation preserves interaction IDs and work log context
- **Provider Optimization**: Translation layer can optimize based on provider capabilities
- **Metadata Preservation**: All tool optimization metadata preserved through translation
- **Round-trip Validation**: Ensures no data loss during provider format conversions

## Backward Compatibility Strategy

### 1. Migration Approach
- **Gradual Migration**: Support both old and new message formats during transition
- **Synthetic Interaction IDs**: Generate interaction IDs for legacy messages
- **Legacy API Support**: Maintain existing ChatSession API with deprecation warnings
- **Data Preservation**: Ensure no message data is lost during migration

### 2. Compatibility Layer
```python
class ChatSessionCompatibility:
    """Compatibility layer for legacy ChatSession usage."""
    
    @property
    def messages(self) -> List[dict[str, Any]]:
        """Legacy messages property - returns flattened message array."""
        return self._flatten_containers_to_legacy_format()
    
    @messages.setter
    def messages(self, value: List[dict[str, Any]]) -> None:
        """Legacy messages setter - converts to interaction containers."""
        self._convert_legacy_messages_to_containers(value)
```

## Performance Considerations

### 1. Memory Optimization
- **Lazy Loading**: Load interaction containers on demand
- **Message Compression**: Compress inactive/archived messages
- **Garbage Collection**: Automatic cleanup of superseded messages
- **Batch Operations**: Minimize individual message operations

### 2. Query Performance
- **Indexing**: Index messages by interaction ID, tool name, and validity state
- **Caching**: Cache frequently accessed message queries
- **Pagination**: Support for paginated message retrieval
- **Filtering**: Efficient filtering of active vs. inactive messages

### 3. Concurrency
- **Thread Safety**: All container operations are thread-safe
- **Lock Granularity**: Fine-grained locking at interaction level
- **Async Support**: Async versions of all major operations
- **Event Batching**: Batch observable events to reduce overhead

## Security and Validation

### 1. Access Control
- **Tool Permissions**: Tools can only manipulate messages they created
- **Interaction Boundaries**: Strict enforcement of interaction boundaries
- **Audit Trail**: Complete audit trail of all message modifications
- **Rollback Capability**: Ability to rollback tool optimizations

### 2. Data Validation
- **Message Integrity**: Validate message consistency across operations
- **Interaction Validation**: Ensure interaction boundaries are maintained
- **Tool Context Validation**: Validate tool-specific metadata
- **Work Log Accuracy**: Ensure work logs accurately reflect tool actions

## Success Criteria

### 1. Functional Requirements
- ✅ InteractionContainer replaces bare message arrays
- ✅ Tools can optimize message arrays based on domain knowledge
- ✅ Work logs provide high-level audit trail with drill-down capability
- ✅ Message editing, branching, and deletion supported
- ✅ Real-time UI updates through observable pattern
- ✅ Lossless provider format translation

### 2. Performance Requirements
- ✅ No performance degradation for existing operations
- ✅ Sub-100ms response time for message operations
- ✅ Support for 10,000+ messages per session
- ✅ Minimal memory overhead for inactive messages

### 3. Quality Requirements
- ✅ 95%+ test coverage across all components
- ✅ Zero data loss during migration
- ✅ Backward compatibility maintained
- ✅ Comprehensive error handling and recovery

## Implementation Phases

### Phase 1: Core Foundation
1. Enhanced CommonChatMessage models
2. InteractionContainer core implementation
3. Basic tool manipulation API

### Phase 2: Intelligence Layer
1. Tool optimization strategies
2. Message invalidation logic
3. Work log system implementation

### Phase 3: Integration
1. ChatSession integration
2. BaseAgent updates
3. Provider translation enhancements

### Phase 4: Migration & Testing
1. Migration utilities
2. Comprehensive testing
3. Performance optimization
4. Documentation and examples

This architecture provides a solid foundation for intelligent message management while maintaining backward compatibility and enabling advanced features for both UI and tool-driven optimization.
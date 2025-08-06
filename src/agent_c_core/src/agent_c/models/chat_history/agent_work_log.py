"""
Agent Work Log System for high-level auditing and review of agent actions.

This module provides comprehensive work log functionality for tracking agent actions,
tool usage, and outcomes in a concise, auditable format. It integrates with the
InteractionContainer system to provide interaction-aware logging and supports
advanced filtering and querying capabilities.
"""

import re
import json
import uuid
import threading
from datetime import datetime, timezone
from typing import Dict, List, Optional, Any, Union, Set, Callable, Tuple
from enum import Enum
from dataclasses import dataclass, field
from pydantic import BaseModel, Field, validator, root_validator

from ..base import ObservableModel
from ..common_chat.enhanced_models import (
    OutcomeStatus, 
    EnhancedCommonChatMessage,
    EnhancedToolUseContentBlock,
    EnhancedToolResultContentBlock
)


class ImpactScope(str, Enum):
    """Defines the scope of impact for a work log entry."""
    LOCAL = "local"           # Affects only current interaction
    SESSION = "session"       # Affects current session
    SYSTEM = "system"         # System-wide effects
    EXTERNAL = "external"     # External system effects
    USER_DATA = "user_data"   # User data modifications
    UNKNOWN = "unknown"       # Impact scope unclear


class ActionCategory(str, Enum):
    """Categories for different types of agent actions."""
    INFORMATION_RETRIEVAL = "information_retrieval"
    DATA_MANIPULATION = "data_manipulation" 
    SYSTEM_OPERATION = "system_operation"
    COMMUNICATION = "communication"
    ANALYSIS = "analysis"
    PLANNING = "planning"
    EXECUTION = "execution"
    MONITORING = "monitoring"
    ERROR_HANDLING = "error_handling"
    OTHER = "other"


class ParameterImportance(str, Enum):
    """Importance levels for parameters in work log entries."""
    CRITICAL = "critical"     # Essential for understanding the action
    HIGH = "high"            # Important for context
    MEDIUM = "medium"        # Useful for detailed review
    LOW = "low"             # Optional detail
    VERBOSE = "verbose"      # Typically filtered out


@dataclass
class ParameterExtractionRule:
    """Rule for extracting parameters from tool calls."""
    tool_pattern: str                           # Regex pattern for tool names
    parameter_rules: Dict[str, ParameterImportance]  # Parameter name -> importance
    extraction_callback: Optional[Callable] = None   # Custom extraction logic
    max_value_length: int = 100                 # Max length for parameter values
    sensitive_patterns: List[str] = field(default_factory=list)  # Patterns to redact


class AgentWorkLogEntry(BaseModel):
    """Individual work log entry representing a single agent action."""
    
    entry_id: str = Field(default_factory=lambda: str(uuid.uuid4()))
    interaction_id: Optional[str] = Field(None, description="Links to InteractionContainer")
    timestamp: datetime = Field(default_factory=lambda: datetime.now(timezone.utc))
    
    # Core action information
    tool_name: str = Field(..., description="Name of the tool used")
    action_summary: str = Field(..., description="Concise description of the action")
    action_category: ActionCategory = Field(ActionCategory.OTHER, description="Category of action")
    
    # Parameters and context
    key_parameters: Dict[str, Any] = Field(default_factory=dict, description="Extracted important parameters")
    parameter_metadata: Dict[str, ParameterImportance] = Field(default_factory=dict, description="Parameter importance levels")
    
    # Outcome tracking
    outcome_status: OutcomeStatus = Field(OutcomeStatus.PENDING, description="Success/failure status")
    outcome_details: Optional[str] = Field(None, description="Additional outcome information")
    execution_time_ms: Optional[float] = Field(None, description="Execution time in milliseconds")
    
    # Impact and scope
    impact_scope: ImpactScope = Field(ImpactScope.UNKNOWN, description="Scope of impact")
    affected_resources: List[str] = Field(default_factory=list, description="Resources affected by the action")
    
    # Relationships
    related_entries: List[str] = Field(default_factory=list, description="Related work log entry IDs")
    parent_entry_id: Optional[str] = Field(None, description="Parent entry for sub-actions")
    
    # Metadata
    agent_context: Optional[Dict[str, Any]] = Field(default_factory=dict, description="Agent-specific context")
    custom_metadata: Optional[Dict[str, Any]] = Field(default_factory=dict, description="Custom metadata")
    
    @validator('key_parameters', pre=True)
    def sanitize_parameters(cls, v):
        """Sanitize parameters to ensure they're JSON serializable."""
        if not isinstance(v, dict):
            return {}
        
        sanitized = {}
        for key, value in v.items():
            try:
                # Test JSON serialization
                json.dumps(value)
                sanitized[key] = value
            except (TypeError, ValueError):
                # Convert non-serializable values to string
                sanitized[key] = str(value)
        
        return sanitized
    
    def is_related_to(self, other_entry: 'AgentWorkLogEntry') -> bool:
        """Check if this entry is related to another entry."""
        return (
            other_entry.entry_id in self.related_entries or
            self.entry_id in other_entry.related_entries or
            self.parent_entry_id == other_entry.entry_id or
            other_entry.parent_entry_id == self.entry_id
        )
    
    def get_concise_summary(self) -> str:
        """Get a concise one-line summary of the action."""
        params_str = ""
        if self.key_parameters:
            # Show only critical and high importance parameters
            important_params = {
                k: v for k, v in self.key_parameters.items()
                if self.parameter_metadata.get(k, ParameterImportance.MEDIUM) in 
                [ParameterImportance.CRITICAL, ParameterImportance.HIGH]
            }
            if important_params:
                param_parts = []
                for k, v in list(important_params.items())[:3]:  # Limit to 3 params
                    value_str = str(v)
                    if len(value_str) > 30:
                        value_str = value_str[:27] + "..."
                    param_parts.append(f"{k}={value_str}")
                params_str = f" ({', '.join(param_parts)})"
        
        status_icon = {
            OutcomeStatus.SUCCESS: "✅",
            OutcomeStatus.FAILURE: "❌", 
            OutcomeStatus.PARTIAL: "⚠️",
            OutcomeStatus.PENDING: "⏳"
        }.get(self.outcome_status, "")
        
        return f"{status_icon} {self.tool_name}: {self.action_summary}{params_str}"


class ParameterExtractor:
    """Handles extraction and summarization of parameters from tool calls."""
    
    def __init__(self):
        self._extraction_rules: List[ParameterExtractionRule] = []
        self._default_rules = self._create_default_rules()
        self._lock = threading.RLock()
    
    def _create_default_rules(self) -> List[ParameterExtractionRule]:
        """Create default parameter extraction rules for common tools."""
        return [
            # File operations
            ParameterExtractionRule(
                tool_pattern=r".*file.*|.*read.*|.*write.*",
                parameter_rules={
                    "path": ParameterImportance.CRITICAL,
                    "filename": ParameterImportance.CRITICAL,
                    "mode": ParameterImportance.HIGH,
                    "encoding": ParameterImportance.MEDIUM,
                    "data": ParameterImportance.VERBOSE,
                    "content": ParameterImportance.VERBOSE
                },
                max_value_length=50,
                sensitive_patterns=[r"password", r"token", r"key", r"secret"]
            ),
            
            # Workspace operations
            ParameterExtractionRule(
                tool_pattern=r"workspace.*",
                parameter_rules={
                    "path": ParameterImportance.CRITICAL,
                    "src_path": ParameterImportance.CRITICAL,
                    "dest_path": ParameterImportance.CRITICAL,
                    "pattern": ParameterImportance.HIGH,
                    "recursive": ParameterImportance.MEDIUM,
                    "max_tokens": ParameterImportance.LOW
                },
                max_value_length=100
            ),
            
            # Planning operations
            ParameterExtractionRule(
                tool_pattern=r".*plan.*|.*task.*",
                parameter_rules={
                    "plan_path": ParameterImportance.CRITICAL,
                    "task_id": ParameterImportance.CRITICAL,
                    "title": ParameterImportance.HIGH,
                    "priority": ParameterImportance.HIGH,
                    "description": ParameterImportance.MEDIUM,
                    "context": ParameterImportance.VERBOSE
                },
                max_value_length=80
            ),
            
            # Agent operations
            ParameterExtractionRule(
                tool_pattern=r".*agent.*|.*chat.*",
                parameter_rules={
                    "agent_key": ParameterImportance.CRITICAL,
                    "session_id": ParameterImportance.HIGH,
                    "message": ParameterImportance.MEDIUM,
                    "request": ParameterImportance.MEDIUM,
                    "process_context": ParameterImportance.VERBOSE
                },
                max_value_length=60
            ),
            
            # Generic fallback
            ParameterExtractionRule(
                tool_pattern=r".*",
                parameter_rules={
                    "id": ParameterImportance.CRITICAL,
                    "name": ParameterImportance.HIGH,
                    "type": ParameterImportance.HIGH,
                    "status": ParameterImportance.HIGH,
                    "result": ParameterImportance.MEDIUM
                },
                max_value_length=50
            )
        ]
    
    def register_extraction_rule(self, rule: ParameterExtractionRule):
        """Register a custom parameter extraction rule."""
        with self._lock:
            self._extraction_rules.append(rule)
    
    def extract_parameters(self, tool_name: str, parameters: Dict[str, Any]) -> Tuple[Dict[str, Any], Dict[str, ParameterImportance]]:
        """Extract and classify parameters based on importance."""
        with self._lock:
            # Find matching rule
            matching_rule = None
            for rule in self._extraction_rules + self._default_rules:
                if re.match(rule.tool_pattern, tool_name, re.IGNORECASE):
                    matching_rule = rule
                    break
            
            if not matching_rule:
                # Use generic fallback
                matching_rule = self._default_rules[-1]
            
            extracted_params = {}
            param_metadata = {}
            
            for param_name, param_value in parameters.items():
                # Determine importance
                importance = matching_rule.parameter_rules.get(
                    param_name, 
                    ParameterImportance.MEDIUM
                )
                
                # Skip verbose parameters unless specifically requested
                if importance == ParameterImportance.VERBOSE:
                    continue
                
                # Process parameter value
                processed_value = self._process_parameter_value(
                    param_value, 
                    matching_rule.max_value_length,
                    matching_rule.sensitive_patterns
                )
                
                extracted_params[param_name] = processed_value
                param_metadata[param_name] = importance
            
            # Apply custom extraction logic if available
            if matching_rule.extraction_callback:
                try:
                    custom_result = matching_rule.extraction_callback(tool_name, parameters)
                    if isinstance(custom_result, tuple) and len(custom_result) == 2:
                        custom_params, custom_metadata = custom_result
                        extracted_params.update(custom_params)
                        param_metadata.update(custom_metadata)
                except Exception:
                    # Ignore custom extraction errors
                    pass
            
            return extracted_params, param_metadata
    
    def _process_parameter_value(self, value: Any, max_length: int, sensitive_patterns: List[str]) -> Any:
        """Process a parameter value for inclusion in work log."""
        # Handle None values
        if value is None:
            return None
        
        # Convert to string for processing
        str_value = str(value)
        
        # Check for sensitive patterns
        for pattern in sensitive_patterns:
            if re.search(pattern, str_value, re.IGNORECASE):
                return "[REDACTED]"
        
        # Truncate if too long
        if len(str_value) > max_length:
            str_value = str_value[:max_length-3] + "..."
        
        # Try to preserve original type for simple types
        if isinstance(value, (int, float, bool)):
            return value
        elif isinstance(value, (list, dict)):
            # For complex types, return truncated string representation
            return str_value
        
        return str_value


class AgentWorkLog(ObservableModel):
    """
    Agent Work Log system for comprehensive auditing and review of agent actions.
    
    Provides high-level auditing capabilities with concise parameter tracking,
    interaction-aware logging, and advanced filtering and querying.
    """
    
    entries: List[AgentWorkLogEntry] = Field(default_factory=list, description="Work log entries")
    interaction_index: Dict[str, List[str]] = Field(default_factory=dict, description="Index by interaction ID")
    tool_index: Dict[str, List[str]] = Field(default_factory=dict, description="Index by tool name")
    
    def __init__(self, **data):
        super().__init__(**data)
        self._parameter_extractor = ParameterExtractor()
        self._lock = threading.RLock()
        self._rebuild_indexes()
    
    def _rebuild_indexes(self):
        """Rebuild internal indexes for efficient querying."""
        with self._lock:
            self.interaction_index.clear()
            self.tool_index.clear()
            
            for entry in self.entries:
                # Index by interaction
                if entry.interaction_id:
                    if entry.interaction_id not in self.interaction_index:
                        self.interaction_index[entry.interaction_id] = []
                    self.interaction_index[entry.interaction_id].append(entry.entry_id)
                
                # Index by tool
                if entry.tool_name not in self.tool_index:
                    self.tool_index[entry.tool_name] = []
                self.tool_index[entry.tool_name].append(entry.entry_id)
    
    def add_entry(self, entry: AgentWorkLogEntry) -> str:
        """Add a work log entry and return its ID."""
        with self._lock:
            self.entries.append(entry)
            
            # Update indexes
            if entry.interaction_id:
                if entry.interaction_id not in self.interaction_index:
                    self.interaction_index[entry.interaction_id] = []
                self.interaction_index[entry.interaction_id].append(entry.entry_id)
            
            if entry.tool_name not in self.tool_index:
                self.tool_index[entry.tool_name] = []
            self.tool_index[entry.tool_name].append(entry.entry_id)
            
            # Trigger observable update
            self.model_changed.emit(field_name="entries", old_value=None, new_value=entry)
            
            return entry.entry_id
    
    def log_tool_call(
        self,
        tool_name: str,
        parameters: Dict[str, Any],
        interaction_id: Optional[str] = None,
        action_summary: Optional[str] = None,
        action_category: ActionCategory = ActionCategory.OTHER,
        impact_scope: ImpactScope = ImpactScope.UNKNOWN,
        agent_context: Optional[Dict[str, Any]] = None
    ) -> str:
        """Log a tool call and return the entry ID."""
        
        # Extract parameters
        key_parameters, param_metadata = self._parameter_extractor.extract_parameters(
            tool_name, parameters
        )
        
        # Generate action summary if not provided
        if not action_summary:
            action_summary = self._generate_action_summary(tool_name, key_parameters)
        
        # Create work log entry
        entry = AgentWorkLogEntry(
            interaction_id=interaction_id,
            tool_name=tool_name,
            action_summary=action_summary,
            action_category=action_category,
            key_parameters=key_parameters,
            parameter_metadata=param_metadata,
            impact_scope=impact_scope,
            agent_context=agent_context or {}
        )
        
        return self.add_entry(entry)
    
    def update_entry_outcome(
        self,
        entry_id: str,
        outcome_status: OutcomeStatus,
        outcome_details: Optional[str] = None,
        execution_time_ms: Optional[float] = None,
        affected_resources: Optional[List[str]] = None
    ):
        """Update the outcome of a work log entry."""
        with self._lock:
            entry = self.get_entry_by_id(entry_id)
            if entry:
                entry.outcome_status = outcome_status
                if outcome_details:
                    entry.outcome_details = outcome_details
                if execution_time_ms is not None:
                    entry.execution_time_ms = execution_time_ms
                if affected_resources:
                    entry.affected_resources.extend(affected_resources)
                
                # Trigger observable update
                self.model_changed.emit(field_name="entries", old_value=None, new_value=entry)
    
    def get_entry_by_id(self, entry_id: str) -> Optional[AgentWorkLogEntry]:
        """Get a work log entry by ID."""
        with self._lock:
            for entry in self.entries:
                if entry.entry_id == entry_id:
                    return entry
            return None
    
    def get_entries_for_interaction(self, interaction_id: str) -> List[AgentWorkLogEntry]:
        """Get all work log entries for a specific interaction."""
        with self._lock:
            entry_ids = self.interaction_index.get(interaction_id, [])
            return [self.get_entry_by_id(eid) for eid in entry_ids if self.get_entry_by_id(eid)]
    
    def get_entries_for_tool(self, tool_name: str) -> List[AgentWorkLogEntry]:
        """Get all work log entries for a specific tool."""
        with self._lock:
            entry_ids = self.tool_index.get(tool_name, [])
            return [self.get_entry_by_id(eid) for eid in entry_ids if self.get_entry_by_id(eid)]
    
    def filter_entries(
        self,
        interaction_ids: Optional[List[str]] = None,
        tool_names: Optional[List[str]] = None,
        outcome_statuses: Optional[List[OutcomeStatus]] = None,
        action_categories: Optional[List[ActionCategory]] = None,
        impact_scopes: Optional[List[ImpactScope]] = None,
        start_time: Optional[datetime] = None,
        end_time: Optional[datetime] = None,
        limit: Optional[int] = None
    ) -> List[AgentWorkLogEntry]:
        """Filter work log entries based on various criteria."""
        with self._lock:
            filtered_entries = []
            
            for entry in self.entries:
                # Filter by interaction IDs
                if interaction_ids and entry.interaction_id not in interaction_ids:
                    continue
                
                # Filter by tool names
                if tool_names and entry.tool_name not in tool_names:
                    continue
                
                # Filter by outcome status
                if outcome_statuses and entry.outcome_status not in outcome_statuses:
                    continue
                
                # Filter by action category
                if action_categories and entry.action_category not in action_categories:
                    continue
                
                # Filter by impact scope
                if impact_scopes and entry.impact_scope not in impact_scopes:
                    continue
                
                # Filter by time range
                if start_time and entry.timestamp < start_time:
                    continue
                if end_time and entry.timestamp > end_time:
                    continue
                
                filtered_entries.append(entry)
            
            # Sort by timestamp (newest first)
            filtered_entries.sort(key=lambda e: e.timestamp, reverse=True)
            
            # Apply limit
            if limit:
                filtered_entries = filtered_entries[:limit]
            
            return filtered_entries
    
    def get_interaction_summary(self, interaction_id: str) -> Dict[str, Any]:
        """Get a comprehensive summary of an interaction's work log entries."""
        entries = self.get_entries_for_interaction(interaction_id)
        
        if not entries:
            return {
                "interaction_id": interaction_id,
                "entry_count": 0,
                "tools_used": [],
                "summary": "No work log entries found"
            }
        
        # Calculate statistics
        tools_used = list(set(entry.tool_name for entry in entries))
        outcome_counts = {}
        category_counts = {}
        total_execution_time = 0
        
        for entry in entries:
            # Count outcomes
            outcome_counts[entry.outcome_status] = outcome_counts.get(entry.outcome_status, 0) + 1
            
            # Count categories
            category_counts[entry.action_category] = category_counts.get(entry.action_category, 0) + 1
            
            # Sum execution times
            if entry.execution_time_ms:
                total_execution_time += entry.execution_time_ms
        
        # Generate summary
        success_rate = outcome_counts.get(OutcomeStatus.SUCCESS, 0) / len(entries) * 100
        
        return {
            "interaction_id": interaction_id,
            "entry_count": len(entries),
            "tools_used": tools_used,
            "outcome_distribution": outcome_counts,
            "category_distribution": category_counts,
            "success_rate_percent": round(success_rate, 1),
            "total_execution_time_ms": total_execution_time,
            "entries": [entry.get_concise_summary() for entry in entries],
            "start_time": min(entry.timestamp for entry in entries),
            "end_time": max(entry.timestamp for entry in entries)
        }
    
    def export_audit_report(
        self,
        format: str = "json",
        include_parameters: bool = True,
        include_metadata: bool = False,
        filter_criteria: Optional[Dict[str, Any]] = None
    ) -> str:
        """Export work log entries as an audit report."""
        # Apply filters if provided
        if filter_criteria:
            entries = self.filter_entries(**filter_criteria)
        else:
            entries = self.entries
        
        # Prepare data for export
        export_data = []
        for entry in entries:
            entry_data = {
                "entry_id": entry.entry_id,
                "interaction_id": entry.interaction_id,
                "timestamp": entry.timestamp.isoformat(),
                "tool_name": entry.tool_name,
                "action_summary": entry.action_summary,
                "action_category": entry.action_category.value,
                "outcome_status": entry.outcome_status.value,
                "impact_scope": entry.impact_scope.value
            }
            
            if include_parameters:
                entry_data["key_parameters"] = entry.key_parameters
                entry_data["parameter_metadata"] = {
                    k: v.value for k, v in entry.parameter_metadata.items()
                }
            
            if include_metadata:
                entry_data.update({
                    "outcome_details": entry.outcome_details,
                    "execution_time_ms": entry.execution_time_ms,
                    "affected_resources": entry.affected_resources,
                    "related_entries": entry.related_entries,
                    "parent_entry_id": entry.parent_entry_id,
                    "agent_context": entry.agent_context,
                    "custom_metadata": entry.custom_metadata
                })
            
            export_data.append(entry_data)
        
        if format.lower() == "json":
            return json.dumps(export_data, indent=2, default=str)
        else:
            raise ValueError(f"Unsupported export format: {format}")
    
    def _generate_action_summary(self, tool_name: str, parameters: Dict[str, Any]) -> str:
        """Generate a concise action summary from tool name and parameters."""
        # Extract key identifiers
        key_identifiers = []
        for key in ["path", "filename", "id", "name", "title"]:
            if key in parameters:
                value = str(parameters[key])
                if len(value) > 30:
                    value = value[:27] + "..."
                key_identifiers.append(value)
                break
        
        # Generate summary based on tool patterns
        tool_lower = tool_name.lower()
        
        if "read" in tool_lower or "get" in tool_lower:
            action = "Retrieved"
        elif "write" in tool_lower or "create" in tool_lower:
            action = "Created/Updated"
        elif "delete" in tool_lower or "remove" in tool_lower:
            action = "Deleted"
        elif "list" in tool_lower or "ls" in tool_lower:
            action = "Listed"
        elif "search" in tool_lower or "find" in tool_lower:
            action = "Searched"
        elif "plan" in tool_lower:
            action = "Planned"
        elif "task" in tool_lower:
            action = "Managed task"
        else:
            action = f"Executed {tool_name}"
        
        if key_identifiers:
            return f"{action} {key_identifiers[0]}"
        else:
            return action
    
    def register_parameter_extraction_rule(self, rule: ParameterExtractionRule):
        """Register a custom parameter extraction rule."""
        self._parameter_extractor.register_extraction_rule(rule)
    
    def get_statistics(self) -> Dict[str, Any]:
        """Get overall work log statistics."""
        with self._lock:
            if not self.entries:
                return {"total_entries": 0}
            
            # Basic counts
            total_entries = len(self.entries)
            unique_interactions = len(self.interaction_index)
            unique_tools = len(self.tool_index)
            
            # Outcome distribution
            outcome_counts = {}
            category_counts = {}
            scope_counts = {}
            
            total_execution_time = 0
            entries_with_timing = 0
            
            for entry in self.entries:
                outcome_counts[entry.outcome_status] = outcome_counts.get(entry.outcome_status, 0) + 1
                category_counts[entry.action_category] = category_counts.get(entry.action_category, 0) + 1
                scope_counts[entry.impact_scope] = scope_counts.get(entry.impact_scope, 0) + 1
                
                if entry.execution_time_ms:
                    total_execution_time += entry.execution_time_ms
                    entries_with_timing += 1
            
            # Calculate averages
            avg_execution_time = total_execution_time / entries_with_timing if entries_with_timing > 0 else 0
            success_rate = outcome_counts.get(OutcomeStatus.SUCCESS, 0) / total_entries * 100
            
            return {
                "total_entries": total_entries,
                "unique_interactions": unique_interactions,
                "unique_tools": unique_tools,
                "outcome_distribution": {k.value: v for k, v in outcome_counts.items()},
                "category_distribution": {k.value: v for k, v in category_counts.items()},
                "scope_distribution": {k.value: v for k, v in scope_counts.items()},
                "success_rate_percent": round(success_rate, 1),
                "average_execution_time_ms": round(avg_execution_time, 2),
                "total_execution_time_ms": total_execution_time,
                "most_used_tools": sorted(
                    [(tool, len(entries)) for tool, entries in self.tool_index.items()],
                    key=lambda x: x[1],
                    reverse=True
                )[:10]
            }


# Convenience functions for creating work log instances
def create_work_log() -> AgentWorkLog:
    """Create a new AgentWorkLog instance."""
    return AgentWorkLog()


def create_work_log_entry(
    tool_name: str,
    action_summary: str,
    interaction_id: Optional[str] = None,
    **kwargs
) -> AgentWorkLogEntry:
    """Create a new AgentWorkLogEntry instance."""
    return AgentWorkLogEntry(
        tool_name=tool_name,
        action_summary=action_summary,
        interaction_id=interaction_id,
        **kwargs
    )
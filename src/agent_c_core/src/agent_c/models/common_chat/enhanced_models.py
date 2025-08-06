"""
Enhanced CommonChatMessage models with interaction tracking, tool invalidation, and work log support.

This module extends the existing CommonChatMessage system with intelligent message management
capabilities including:
- Interaction ID tracking for message grouping
- Tool invalidation and optimization support
- Work log metadata integration
- Message lifecycle state management
- Enhanced content blocks with context tracking
"""

from enum import Enum
from typing import List, Optional, Dict, Any, Union, Literal, Annotated
from datetime import datetime
from uuid import uuid4
from pydantic import BaseModel, Field, ConfigDict, field_validator

# Import existing base classes
from .models import (
    MessageRole, ContentBlockType, BaseContentBlock, 
    StopInfo, TokenUsage, ProviderMetadata
)


class ValidityState(str, Enum):
    """Message lifecycle states for intelligent management."""
    ACTIVE = "active"           # Message is current and valid
    INVALIDATED = "invalidated" # Message has been invalidated by a tool
    SUPERSEDED = "superseded"   # Message has been replaced by a newer version
    ARCHIVED = "archived"       # Message is kept for history but not active


class OutcomeStatus(str, Enum):
    """Tool execution outcome tracking."""
    SUCCESS = "success"
    FAILURE = "failure"
    PARTIAL = "partial"
    PENDING = "pending"


class ReasoningType(str, Enum):
    """Types of reasoning in thinking blocks."""
    ANALYSIS = "analysis"
    PLANNING = "planning"
    REFLECTION = "reflection"
    PROBLEM_SOLVING = "problem_solving"
    DECISION_MAKING = "decision_making"


# Enhanced Content Blocks

class EnhancedTextContentBlock(BaseContentBlock):
    """Enhanced text content block with interaction context."""
    type: Literal[ContentBlockType.TEXT] = ContentBlockType.TEXT
    text: str
    citations: List[Dict[str, Any]] = Field(default_factory=list)
    
    # New fields for work log integration
    interaction_context: Optional[str] = Field(
        default=None, 
        description="Context about the interaction this text relates to"
    )
    content_tags: List[str] = Field(
        default_factory=list,
        description="Tags for categorizing text content"
    )


class EnhancedToolUseContentBlock(BaseContentBlock):
    """Enhanced tool use block with work log metadata and optimization support."""
    type: Literal[ContentBlockType.TOOL_USE] = ContentBlockType.TOOL_USE
    tool_name: str
    tool_id: str
    parameters: Dict[str, Any] = Field(default_factory=dict)
    
    # New fields for work log integration
    work_log_metadata: Optional[Dict[str, Any]] = Field(
        default=None,
        description="Metadata for work log generation"
    )
    execution_context: Optional[Dict[str, Any]] = Field(
        default=None,
        description="Context about the tool execution environment"
    )
    optimization_hints: List[str] = Field(
        default_factory=list,
        description="Hints for tool optimization strategies"
    )
    parameter_importance: Dict[str, int] = Field(
        default_factory=dict,
        description="Importance ranking of parameters (1-10 scale) for work logs"
    )
    expected_outcome: Optional[str] = Field(
        default=None,
        description="Expected outcome description for validation"
    )
    
    def extract_key_parameters(self) -> Dict[str, Any]:
        """Extract key parameters based on importance rankings."""
        if not self.parameter_importance:
            return self.parameters
        
        # Return parameters with importance >= 7 (high importance)
        key_params = {}
        for param, value in self.parameters.items():
            importance = self.parameter_importance.get(param, 5)  # Default to medium
            if importance >= 7:
                key_params[param] = value
        
        return key_params if key_params else self.parameters
    
    def generate_work_log_summary(self) -> str:
        """Generate a concise summary for work log entries."""
        key_params = self.extract_key_parameters()
        
        # Create a concise description
        if key_params:
            param_summary = ", ".join([f"{k}={v}" for k, v in list(key_params.items())[:3]])
            return f"{self.tool_name}({param_summary})"
        else:
            return f"{self.tool_name}()"


class EnhancedToolResultContentBlock(BaseContentBlock):
    """Enhanced tool result block with outcome tracking and impact analysis."""
    type: Literal[ContentBlockType.TOOL_RESULT] = ContentBlockType.TOOL_RESULT
    tool_name: str
    tool_id: str
    result: Any
    
    # New fields for outcome tracking
    outcome_status: OutcomeStatus = Field(
        default=OutcomeStatus.SUCCESS,
        description="Status of the tool execution"
    )
    execution_time: Optional[float] = Field(
        default=None,
        description="Time taken to execute the tool in seconds"
    )
    error_details: Optional[str] = Field(
        default=None,
        description="Error details if execution failed"
    )
    impact_scope: List[str] = Field(
        default_factory=list,
        description="List of resources/entities affected by this tool execution"
    )
    result_summary: Optional[str] = Field(
        default=None,
        description="Concise summary of the result for work logs"
    )
    confidence_score: Optional[float] = Field(
        default=None,
        description="Confidence in the result accuracy (0.0-1.0)"
    )
    
    def generate_outcome_summary(self) -> str:
        """Generate a summary of the tool execution outcome."""
        status_emoji = {
            OutcomeStatus.SUCCESS: "✅",
            OutcomeStatus.FAILURE: "❌", 
            OutcomeStatus.PARTIAL: "⚠️",
            OutcomeStatus.PENDING: "⏳"
        }
        
        emoji = status_emoji.get(self.outcome_status, "")
        base_summary = f"{emoji} {self.tool_name}: {self.outcome_status.value}"
        
        if self.result_summary:
            return f"{base_summary} - {self.result_summary}"
        elif self.error_details and self.outcome_status == OutcomeStatus.FAILURE:
            return f"{base_summary} - {self.error_details[:100]}..."
        else:
            return base_summary


class EnhancedThinkingContentBlock(BaseContentBlock):
    """Enhanced thinking block with interaction context and reasoning classification."""
    type: Literal[ContentBlockType.THINKING] = ContentBlockType.THINKING
    thought: str
    redacted: bool = False
    
    # New fields for interaction context
    interaction_context: Optional[str] = Field(
        default=None,
        description="Context about the interaction this thinking relates to"
    )
    reasoning_type: Optional[ReasoningType] = Field(
        default=None,
        description="Type of reasoning being performed"
    )
    confidence_level: Optional[float] = Field(
        default=None,
        description="Confidence in the reasoning (0.0-1.0)"
    )
    related_tools: List[str] = Field(
        default_factory=list,
        description="Tools that this thinking relates to"
    )
    decision_points: List[str] = Field(
        default_factory=list,
        description="Key decision points in the reasoning"
    )
    
    def generate_thinking_summary(self) -> str:
        """Generate a summary of the thinking content."""
        reasoning_prefix = f"[{self.reasoning_type.value}]" if self.reasoning_type else "[thinking]"
        
        # Truncate thought to reasonable length for summary
        thought_summary = self.thought[:100] + "..." if len(self.thought) > 100 else self.thought
        
        if self.redacted:
            return f"{reasoning_prefix} [REDACTED]"
        else:
            return f"{reasoning_prefix} {thought_summary}"


# Keep existing content blocks that don't need enhancement
class ImageContentBlock(BaseContentBlock):
    """A block containing an image."""
    type: Literal[ContentBlockType.IMAGE] = ContentBlockType.IMAGE
    source: Dict[str, Any]  # Could be URL, base64, etc.


class AudioContentBlock(BaseContentBlock):
    """A block containing audio data."""
    type: Literal[ContentBlockType.AUDIO] = ContentBlockType.AUDIO
    source: Dict[str, Any]  # Could be URL, base64, etc.


# Enhanced discriminated union of content block types
EnhancedContentBlock = Annotated[
    Union[
        EnhancedTextContentBlock,
        EnhancedToolUseContentBlock,
        EnhancedToolResultContentBlock,
        EnhancedThinkingContentBlock,
        ImageContentBlock,
        AudioContentBlock
    ],
    Field(discriminator="type")
]


class EnhancedCommonChatMessage(BaseModel):
    """
    Enhanced common message format with interaction tracking, tool invalidation, and work log support.
    
    This enhanced model provides intelligent message management capabilities including:
    - Interaction boundary tracking
    - Tool-driven message optimization
    - Work log metadata extraction
    - Message lifecycle management
    - Advanced content block features
    """
    
    # Existing core fields
    id: str = Field(description="Unique ID for this message")
    role: MessageRole = Field(description="The role of this message (user, assistant, system, tool)")
    content: List[EnhancedContentBlock] = Field(description="Enhanced content blocks for this message")
    created_at: datetime = Field(description="When this message was created")
    provider_metadata: Optional[ProviderMetadata] = Field(
        default=None, description="Provider-specific metadata"
    )
    
    # New fields for intelligent management
    interaction_id: str = Field(description="ID of the interaction this message belongs to")
    invalidated_by: Optional[str] = Field(
        default=None, 
        description="Tool that invalidated this message"
    )
    validity_state: ValidityState = Field(
        default=ValidityState.ACTIVE,
        description="Current lifecycle state of this message"
    )
    tool_context: Optional[Dict[str, Any]] = Field(
        default=None, 
        description="Tool-specific metadata for work logs"
    )
    superseded_by: Optional[str] = Field(
        default=None, 
        description="Message ID that supersedes this one"
    )
    updated_at: Optional[datetime] = Field(
        default=None, 
        description="When this message was last modified"
    )
    invalidation_reason: Optional[str] = Field(
        default=None,
        description="Reason why this message was invalidated"
    )
    
    model_config = ConfigDict(
        populate_by_name=True,
        json_schema_extra={
            "examples": [
                {
                    "id": "msg_123456",
                    "role": "assistant",
                    "interaction_id": "int_789012",
                    "content": [
                        {
                            "type": "text",
                            "text": "I'll help you with that task.",
                            "interaction_context": "User requested file analysis"
                        }
                    ],
                    "created_at": "2023-08-01T12:00:00Z",
                    "validity_state": "active"
                }
            ]
        }
    )
    
    @field_validator('id', mode='before')
    @classmethod
    def set_id_if_empty(cls, v):
        """Set a UUID if id is not provided."""
        if not v:
            return str(uuid4())
        return v

    @field_validator('created_at', mode='before')
    @classmethod
    def set_created_at_if_empty(cls, v):
        """Set current time if created_at is not provided."""
        if not v:
            return datetime.now()
        return v
    
    @field_validator('interaction_id', mode='before')
    @classmethod
    def set_interaction_id_if_empty(cls, v):
        """Set a UUID for interaction_id if not provided."""
        if not v:
            return str(uuid4())
        return v
    
    @field_validator('validity_state')
    @classmethod
    def validate_validity_state_transitions(cls, v, info):
        """Validate state transitions and set updated_at when state changes."""
        # This would be enhanced with proper state transition validation
        # For now, just ensure it's a valid state
        return v
    
    # Enhanced properties and methods
    
    @property
    def text_content(self) -> str:
        """Get all text content concatenated together."""
        return ''.join([
            block.text for block in self.content 
            if isinstance(block, EnhancedTextContentBlock)
        ])
    
    def get_content_by_type(self, content_type: ContentBlockType) -> List[EnhancedContentBlock]:
        """Get all content blocks of a specific type."""
        return [block for block in self.content if block.type == content_type]
    
    def is_active(self) -> bool:
        """Check if this message is currently active."""
        return self.validity_state == ValidityState.ACTIVE
    
    def invalidate(self, invalidated_by: str, reason: str) -> None:
        """Mark this message as invalidated by a tool."""
        self.validity_state = ValidityState.INVALIDATED
        self.invalidated_by = invalidated_by
        self.invalidation_reason = reason
        self.updated_at = datetime.now()
    
    def supersede(self, superseded_by: str) -> None:
        """Mark this message as superseded by another message."""
        self.validity_state = ValidityState.SUPERSEDED
        self.superseded_by = superseded_by
        self.updated_at = datetime.now()
    
    def archive(self) -> None:
        """Archive this message (keep for history but mark inactive)."""
        self.validity_state = ValidityState.ARCHIVED
        self.updated_at = datetime.now()
    
    def get_work_log_summary(self) -> Dict[str, Any]:
        """Extract work log summary from this message."""
        summary = {
            "message_id": self.id,
            "interaction_id": self.interaction_id,
            "role": self.role.value,
            "timestamp": self.created_at.isoformat(),
            "validity_state": self.validity_state.value,
            "actions": []
        }
        
        # Extract actions from tool use blocks
        for block in self.content:
            if isinstance(block, EnhancedToolUseContentBlock):
                action = {
                    "tool_name": block.tool_name,
                    "action_summary": block.generate_work_log_summary(),
                    "key_parameters": block.extract_key_parameters()
                }
                summary["actions"].append(action)
        
        # Extract outcomes from tool result blocks
        for block in self.content:
            if isinstance(block, EnhancedToolResultContentBlock):
                outcome = {
                    "tool_name": block.tool_name,
                    "outcome_summary": block.generate_outcome_summary(),
                    "status": block.outcome_status.value,
                    "impact_scope": block.impact_scope
                }
                summary["actions"].append(outcome)
        
        return summary
    
    def extract_tool_parameters(self) -> Dict[str, Any]:
        """Extract all tool parameters from this message."""
        tool_params = {}
        
        for block in self.content:
            if isinstance(block, EnhancedToolUseContentBlock):
                tool_params[block.tool_name] = {
                    "tool_id": block.tool_id,
                    "parameters": block.parameters,
                    "key_parameters": block.extract_key_parameters()
                }
        
        return tool_params
    
    def get_interaction_context(self) -> Dict[str, Any]:
        """Get comprehensive interaction context from this message."""
        context = {
            "interaction_id": self.interaction_id,
            "message_id": self.id,
            "role": self.role.value,
            "timestamp": self.created_at.isoformat(),
            "validity_state": self.validity_state.value,
            "content_types": [block.type.value for block in self.content],
            "tool_context": self.tool_context or {}
        }
        
        # Add context from enhanced content blocks
        for block in self.content:
            if hasattr(block, 'interaction_context') and block.interaction_context:
                context["block_contexts"] = context.get("block_contexts", [])
                context["block_contexts"].append({
                    "type": block.type.value,
                    "context": block.interaction_context
                })
        
        return context
    
    def get_tool_names(self) -> List[str]:
        """Get list of all tools used in this message."""
        tool_names = []
        
        for block in self.content:
            if isinstance(block, (EnhancedToolUseContentBlock, EnhancedToolResultContentBlock)):
                if block.tool_name not in tool_names:
                    tool_names.append(block.tool_name)
        
        return tool_names
    
    def has_tool_failures(self) -> bool:
        """Check if this message contains any failed tool executions."""
        for block in self.content:
            if isinstance(block, EnhancedToolResultContentBlock):
                if block.outcome_status == OutcomeStatus.FAILURE:
                    return True
        return False
    
    def get_execution_summary(self) -> Dict[str, Any]:
        """Get summary of all tool executions in this message."""
        summary = {
            "total_tools": 0,
            "successful_tools": 0,
            "failed_tools": 0,
            "pending_tools": 0,
            "total_execution_time": 0.0,
            "tools_used": []
        }
        
        for block in self.content:
            if isinstance(block, EnhancedToolResultContentBlock):
                summary["total_tools"] += 1
                summary["tools_used"].append(block.tool_name)
                
                if block.outcome_status == OutcomeStatus.SUCCESS:
                    summary["successful_tools"] += 1
                elif block.outcome_status == OutcomeStatus.FAILURE:
                    summary["failed_tools"] += 1
                elif block.outcome_status == OutcomeStatus.PENDING:
                    summary["pending_tools"] += 1
                
                if block.execution_time:
                    summary["total_execution_time"] += block.execution_time
        
        return summary


# Backward compatibility alias
CommonChatMessage = EnhancedCommonChatMessage
ContentBlock = EnhancedContentBlock
TextContentBlock = EnhancedTextContentBlock
ToolUseContentBlock = EnhancedToolUseContentBlock
ToolResultContentBlock = EnhancedToolResultContentBlock
ThinkingContentBlock = EnhancedThinkingContentBlock
"""
Comprehensive tests for enhanced CommonChatMessage models.

Tests cover:
- Enhanced content blocks functionality
- Message lifecycle management
- Tool invalidation and optimization
- Work log integration
- Interaction tracking
- Backward compatibility
"""

import pytest
from datetime import datetime
from uuid import uuid4

from agent_c.models.common_chat.enhanced_models import (
    EnhancedCommonChatMessage,
    EnhancedTextContentBlock,
    EnhancedToolUseContentBlock,
    EnhancedToolResultContentBlock,
    EnhancedThinkingContentBlock,
    ValidityState,
    OutcomeStatus,
    ReasoningType,
    MessageRole,
    ContentBlockType
)


class TestValidityState:
    """Test ValidityState enum."""
    
    def test_validity_state_values(self):
        """Test that ValidityState has correct values."""
        assert ValidityState.ACTIVE == "active"
        assert ValidityState.INVALIDATED == "invalidated"
        assert ValidityState.SUPERSEDED == "superseded"
        assert ValidityState.ARCHIVED == "archived"


class TestOutcomeStatus:
    """Test OutcomeStatus enum."""
    
    def test_outcome_status_values(self):
        """Test that OutcomeStatus has correct values."""
        assert OutcomeStatus.SUCCESS == "success"
        assert OutcomeStatus.FAILURE == "failure"
        assert OutcomeStatus.PARTIAL == "partial"
        assert OutcomeStatus.PENDING == "pending"


class TestEnhancedTextContentBlock:
    """Test EnhancedTextContentBlock functionality."""
    
    def test_basic_creation(self):
        """Test basic text block creation."""
        block = EnhancedTextContentBlock(text="Hello world")
        
        assert block.type == ContentBlockType.TEXT
        assert block.text == "Hello world"
        assert block.citations == []
        assert block.interaction_context is None
        assert block.content_tags == []
    
    def test_with_enhancements(self):
        """Test text block with enhanced fields."""
        block = EnhancedTextContentBlock(
            text="Analysis complete",
            interaction_context="User requested file analysis",
            content_tags=["analysis", "completion"]
        )
        
        assert block.interaction_context == "User requested file analysis"
        assert block.content_tags == ["analysis", "completion"]


class TestEnhancedToolUseContentBlock:
    """Test EnhancedToolUseContentBlock functionality."""
    
    def test_basic_creation(self):
        """Test basic tool use block creation."""
        block = EnhancedToolUseContentBlock(
            tool_name="file_reader",
            tool_id="tool_123",
            parameters={"path": "/test/file.txt"}
        )
        
        assert block.type == ContentBlockType.TOOL_USE
        assert block.tool_name == "file_reader"
        assert block.tool_id == "tool_123"
        assert block.parameters == {"path": "/test/file.txt"}
        assert block.work_log_metadata is None
        assert block.optimization_hints == []
        assert block.parameter_importance == {}
    
    def test_extract_key_parameters_with_importance(self):
        """Test key parameter extraction based on importance."""
        block = EnhancedToolUseContentBlock(
            tool_name="update_plan",
            tool_id="tool_456",
            parameters={
                "plan_id": "plan_123",
                "task_id": "task_456", 
                "verbose": True,
                "debug": False
            },
            parameter_importance={
                "plan_id": 10,  # High importance
                "task_id": 8,   # High importance
                "verbose": 3,   # Low importance
                "debug": 2      # Low importance
            }
        )
        
        key_params = block.extract_key_parameters()
        assert key_params == {"plan_id": "plan_123", "task_id": "task_456"}
    
    def test_extract_key_parameters_no_importance(self):
        """Test key parameter extraction without importance rankings."""
        block = EnhancedToolUseContentBlock(
            tool_name="simple_tool",
            tool_id="tool_789",
            parameters={"param1": "value1", "param2": "value2"}
        )
        
        key_params = block.extract_key_parameters()
        assert key_params == {"param1": "value1", "param2": "value2"}
    
    def test_generate_work_log_summary(self):
        """Test work log summary generation."""
        block = EnhancedToolUseContentBlock(
            tool_name="update_plan",
            tool_id="tool_456",
            parameters={
                "plan_id": "plan_123",
                "task_id": "task_456",
                "status": "completed"
            },
            parameter_importance={
                "plan_id": 10,
                "task_id": 8,
                "status": 6  # Below threshold
            }
        )
        
        summary = block.generate_work_log_summary()
        assert "update_plan" in summary
        assert "plan_id=plan_123" in summary
        assert "task_id=task_456" in summary
        assert "status=completed" not in summary  # Below importance threshold


class TestEnhancedToolResultContentBlock:
    """Test EnhancedToolResultContentBlock functionality."""
    
    def test_basic_creation(self):
        """Test basic tool result block creation."""
        block = EnhancedToolResultContentBlock(
            tool_name="file_reader",
            tool_id="tool_123",
            result="File content here"
        )
        
        assert block.type == ContentBlockType.TOOL_RESULT
        assert block.tool_name == "file_reader"
        assert block.tool_id == "tool_123"
        assert block.result == "File content here"
        assert block.outcome_status == OutcomeStatus.SUCCESS
        assert block.execution_time is None
        assert block.error_details is None
        assert block.impact_scope == []
    
    def test_generate_outcome_summary_success(self):
        """Test outcome summary generation for successful execution."""
        block = EnhancedToolResultContentBlock(
            tool_name="file_writer",
            tool_id="tool_456",
            result="File written successfully",
            outcome_status=OutcomeStatus.SUCCESS,
            result_summary="Created new configuration file"
        )
        
        summary = block.generate_outcome_summary()
        assert "✅" in summary
        assert "file_writer" in summary
        assert "success" in summary
        assert "Created new configuration file" in summary
    
    def test_generate_outcome_summary_failure(self):
        """Test outcome summary generation for failed execution."""
        block = EnhancedToolResultContentBlock(
            tool_name="file_reader",
            tool_id="tool_789",
            result=None,
            outcome_status=OutcomeStatus.FAILURE,
            error_details="File not found: /nonexistent/file.txt"
        )
        
        summary = block.generate_outcome_summary()
        assert "❌" in summary
        assert "file_reader" in summary
        assert "failure" in summary
        assert "File not found" in summary


class TestEnhancedThinkingContentBlock:
    """Test EnhancedThinkingContentBlock functionality."""
    
    def test_basic_creation(self):
        """Test basic thinking block creation."""
        block = EnhancedThinkingContentBlock(
            thought="I need to analyze this problem step by step."
        )
        
        assert block.type == ContentBlockType.THINKING
        assert block.thought == "I need to analyze this problem step by step."
        assert block.redacted is False
        assert block.interaction_context is None
        assert block.reasoning_type is None
        assert block.confidence_level is None
        assert block.related_tools == []
        assert block.decision_points == []
    
    def test_with_enhancements(self):
        """Test thinking block with enhanced fields."""
        block = EnhancedThinkingContentBlock(
            thought="Let me break down this complex task into smaller steps.",
            reasoning_type=ReasoningType.PLANNING,
            confidence_level=0.85,
            interaction_context="User requested project planning assistance",
            related_tools=["create_plan", "add_task"],
            decision_points=["Choose task priority", "Determine dependencies"]
        )
        
        assert block.reasoning_type == ReasoningType.PLANNING
        assert block.confidence_level == 0.85
        assert block.interaction_context == "User requested project planning assistance"
        assert block.related_tools == ["create_plan", "add_task"]
        assert block.decision_points == ["Choose task priority", "Determine dependencies"]
    
    def test_generate_thinking_summary(self):
        """Test thinking summary generation."""
        block = EnhancedThinkingContentBlock(
            thought="This is a very long thought that goes on and on about various aspects of the problem and potential solutions that could be implemented.",
            reasoning_type=ReasoningType.ANALYSIS
        )
        
        summary = block.generate_thinking_summary()
        assert "[analysis]" in summary
        assert len(summary) <= 120  # Should be truncated
        assert "..." in summary  # Should have ellipsis for truncation
    
    def test_generate_thinking_summary_redacted(self):
        """Test thinking summary generation for redacted content."""
        block = EnhancedThinkingContentBlock(
            thought="Sensitive information here",
            reasoning_type=ReasoningType.DECISION_MAKING,
            redacted=True
        )
        
        summary = block.generate_thinking_summary()
        assert "[decision_making]" in summary
        assert "[REDACTED]" in summary
        assert "Sensitive information" not in summary


class TestEnhancedCommonChatMessage:
    """Test EnhancedCommonChatMessage functionality."""
    
    def test_basic_creation(self):
        """Test basic message creation with defaults."""
        message = EnhancedCommonChatMessage(
            role=MessageRole.ASSISTANT,
            content=[EnhancedTextContentBlock(text="Hello!")]
        )
        
        assert message.role == MessageRole.ASSISTANT
        assert len(message.content) == 1
        assert message.validity_state == ValidityState.ACTIVE
        assert message.invalidated_by is None
        assert message.superseded_by is None
        assert message.tool_context is None
        
        # Should auto-generate IDs and timestamps
        assert message.id is not None
        assert message.interaction_id is not None
        assert message.created_at is not None
    
    def test_with_interaction_tracking(self):
        """Test message creation with interaction tracking."""
        interaction_id = str(uuid4())
        message = EnhancedCommonChatMessage(
            role=MessageRole.USER,
            interaction_id=interaction_id,
            content=[EnhancedTextContentBlock(text="Please help me")],
            tool_context={"session_id": "sess_123", "user_intent": "assistance"}
        )
        
        assert message.interaction_id == interaction_id
        assert message.tool_context == {"session_id": "sess_123", "user_intent": "assistance"}
    
    def test_is_active(self):
        """Test is_active method."""
        message = EnhancedCommonChatMessage(
            role=MessageRole.ASSISTANT,
            content=[EnhancedTextContentBlock(text="Active message")]
        )
        
        assert message.is_active() is True
        
        message.validity_state = ValidityState.INVALIDATED
        assert message.is_active() is False
    
    def test_invalidate(self):
        """Test message invalidation."""
        message = EnhancedCommonChatMessage(
            role=MessageRole.ASSISTANT,
            content=[EnhancedTextContentBlock(text="To be invalidated")]
        )
        
        original_updated_at = message.updated_at
        message.invalidate("optimization_tool", "Superseded by newer analysis")
        
        assert message.validity_state == ValidityState.INVALIDATED
        assert message.invalidated_by == "optimization_tool"
        assert message.invalidation_reason == "Superseded by newer analysis"
        assert message.updated_at != original_updated_at
        assert message.is_active() is False
    
    def test_supersede(self):
        """Test message superseding."""
        message = EnhancedCommonChatMessage(
            role=MessageRole.ASSISTANT,
            content=[EnhancedTextContentBlock(text="Old version")]
        )
        
        new_message_id = str(uuid4())
        message.supersede(new_message_id)
        
        assert message.validity_state == ValidityState.SUPERSEDED
        assert message.superseded_by == new_message_id
        assert message.updated_at is not None
    
    def test_archive(self):
        """Test message archiving."""
        message = EnhancedCommonChatMessage(
            role=MessageRole.ASSISTANT,
            content=[EnhancedTextContentBlock(text="Archive me")]
        )
        
        message.archive()
        
        assert message.validity_state == ValidityState.ARCHIVED
        assert message.updated_at is not None
    
    def test_get_work_log_summary(self):
        """Test work log summary extraction."""
        message = EnhancedCommonChatMessage(
            role=MessageRole.ASSISTANT,
            content=[
                EnhancedToolUseContentBlock(
                    tool_name="file_writer",
                    tool_id="tool_123",
                    parameters={"path": "/test.txt", "content": "Hello"}
                ),
                EnhancedToolResultContentBlock(
                    tool_name="file_writer",
                    tool_id="tool_123",
                    result="File written successfully",
                    outcome_status=OutcomeStatus.SUCCESS
                )
            ]
        )
        
        summary = message.get_work_log_summary()
        
        assert summary["message_id"] == message.id
        assert summary["interaction_id"] == message.interaction_id
        assert summary["role"] == "assistant"
        assert summary["validity_state"] == "active"
        assert len(summary["actions"]) == 2
        
        # Check tool use action
        tool_use_action = summary["actions"][0]
        assert tool_use_action["tool_name"] == "file_writer"
        assert "file_writer" in tool_use_action["action_summary"]
        
        # Check tool result action
        tool_result_action = summary["actions"][1]
        assert tool_result_action["tool_name"] == "file_writer"
        assert tool_result_action["status"] == "success"
    
    def test_extract_tool_parameters(self):
        """Test tool parameter extraction."""
        message = EnhancedCommonChatMessage(
            role=MessageRole.ASSISTANT,
            content=[
                EnhancedToolUseContentBlock(
                    tool_name="update_plan",
                    tool_id="tool_456",
                    parameters={"plan_id": "plan_123", "status": "completed"},
                    parameter_importance={"plan_id": 10, "status": 8}
                ),
                EnhancedTextContentBlock(text="Plan updated successfully")
            ]
        )
        
        tool_params = message.extract_tool_parameters()
        
        assert "update_plan" in tool_params
        assert tool_params["update_plan"]["tool_id"] == "tool_456"
        assert tool_params["update_plan"]["parameters"] == {"plan_id": "plan_123", "status": "completed"}
        assert tool_params["update_plan"]["key_parameters"] == {"plan_id": "plan_123", "status": "completed"}
    
    def test_get_interaction_context(self):
        """Test interaction context extraction."""
        message = EnhancedCommonChatMessage(
            role=MessageRole.USER,
            content=[
                EnhancedTextContentBlock(
                    text="Please analyze this file",
                    interaction_context="User requesting file analysis"
                )
            ],
            tool_context={"session_type": "analysis", "priority": "high"}
        )
        
        context = message.get_interaction_context()
        
        assert context["interaction_id"] == message.interaction_id
        assert context["message_id"] == message.id
        assert context["role"] == "user"
        assert context["validity_state"] == "active"
        assert context["content_types"] == ["text"]
        assert context["tool_context"] == {"session_type": "analysis", "priority": "high"}
        assert "block_contexts" in context
        assert len(context["block_contexts"]) == 1
    
    def test_get_tool_names(self):
        """Test tool name extraction."""
        message = EnhancedCommonChatMessage(
            role=MessageRole.ASSISTANT,
            content=[
                EnhancedToolUseContentBlock(tool_name="file_reader", tool_id="1", parameters={}),
                EnhancedToolResultContentBlock(tool_name="file_reader", tool_id="1", result="content"),
                EnhancedToolUseContentBlock(tool_name="file_writer", tool_id="2", parameters={}),
                EnhancedTextContentBlock(text="Done")
            ]
        )
        
        tool_names = message.get_tool_names()
        
        assert set(tool_names) == {"file_reader", "file_writer"}
        assert len(tool_names) == 2  # Should not have duplicates
    
    def test_has_tool_failures(self):
        """Test tool failure detection."""
        # Message with successful tools
        success_message = EnhancedCommonChatMessage(
            role=MessageRole.ASSISTANT,
            content=[
                EnhancedToolResultContentBlock(
                    tool_name="tool1", tool_id="1", result="success",
                    outcome_status=OutcomeStatus.SUCCESS
                )
            ]
        )
        
        assert success_message.has_tool_failures() is False
        
        # Message with failed tools
        failure_message = EnhancedCommonChatMessage(
            role=MessageRole.ASSISTANT,
            content=[
                EnhancedToolResultContentBlock(
                    tool_name="tool1", tool_id="1", result=None,
                    outcome_status=OutcomeStatus.FAILURE
                )
            ]
        )
        
        assert failure_message.has_tool_failures() is True
    
    def test_get_execution_summary(self):
        """Test execution summary generation."""
        message = EnhancedCommonChatMessage(
            role=MessageRole.ASSISTANT,
            content=[
                EnhancedToolResultContentBlock(
                    tool_name="tool1", tool_id="1", result="success",
                    outcome_status=OutcomeStatus.SUCCESS, execution_time=1.5
                ),
                EnhancedToolResultContentBlock(
                    tool_name="tool2", tool_id="2", result=None,
                    outcome_status=OutcomeStatus.FAILURE, execution_time=0.8
                ),
                EnhancedTextContentBlock(text="Summary")
            ]
        )
        
        summary = message.get_execution_summary()
        
        assert summary["total_tools"] == 2
        assert summary["successful_tools"] == 1
        assert summary["failed_tools"] == 1
        assert summary["pending_tools"] == 0
        assert summary["total_execution_time"] == 2.3
        assert set(summary["tools_used"]) == {"tool1", "tool2"}


class TestBackwardCompatibility:
    """Test backward compatibility with existing code."""
    
    def test_import_aliases(self):
        """Test that backward compatibility aliases work."""
        from agent_c.models.common_chat import (
            CommonChatMessage,
            ContentBlock,
            TextContentBlock,
            ToolUseContentBlock,
            ToolResultContentBlock,
            ThinkingContentBlock
        )
        
        # These should be the enhanced versions
        assert CommonChatMessage is EnhancedCommonChatMessage
        assert TextContentBlock is EnhancedTextContentBlock
        assert ToolUseContentBlock is EnhancedToolUseContentBlock
        assert ToolResultContentBlock is EnhancedToolResultContentBlock
        assert ThinkingContentBlock is EnhancedThinkingContentBlock
    
    def test_existing_usage_patterns(self):
        """Test that existing usage patterns still work."""
        # This should work exactly as before
        message = CommonChatMessage(
            role=MessageRole.ASSISTANT,
            content=[
                TextContentBlock(text="Hello"),
                ToolUseContentBlock(
                    tool_name="test_tool",
                    tool_id="tool_123",
                    parameters={"param": "value"}
                )
            ]
        )
        
        # Old methods should still work
        assert message.text_content == "Hello"
        assert len(message.get_content_by_type(ContentBlockType.TEXT)) == 1
        assert len(message.get_content_by_type(ContentBlockType.TOOL_USE)) == 1
        
        # New functionality should be available
        assert message.is_active() is True
        assert message.validity_state == ValidityState.ACTIVE


if __name__ == "__main__":
    pytest.main([__file__])
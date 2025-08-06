"""
Comprehensive tests for enhanced provider translation layer.

Tests cover:
- Bidirectional translation for Anthropic and OpenAI
- Work log context preservation
- Interaction ID tracking
- Round-trip translation validation
- Error handling and edge cases
- Performance and audit trail functionality
"""

import pytest
import json
from datetime import datetime
from uuid import uuid4
from typing import Dict, Any, List

from agent_c.models.common_chat.enhanced_converters import (
    EnhancedAnthropicConverter,
    EnhancedOpenAIConverter,
    EnhancedProviderTranslationLayer,
    ProviderType,
    ProviderCapability,
    TranslationError,
    create_translation_layer,
    translate_from_anthropic,
    translate_from_openai,
    translate_to_anthropic,
    translate_to_openai
)
from agent_c.models.common_chat.enhanced_models import (
    EnhancedCommonChatMessage,
    EnhancedTextContentBlock,
    EnhancedToolUseContentBlock,
    EnhancedToolResultContentBlock,
    EnhancedThinkingContentBlock,
    MessageRole,
    ValidityState,
    OutcomeStatus,
    ReasoningType
)


class TestEnhancedAnthropicConverter:
    """Test enhanced Anthropic converter functionality."""
    
    def test_from_native_basic_text_message(self):
        """Test converting basic Anthropic text message."""
        anthropic_msg = {
            "id": "msg_123",
            "type": "message",
            "role": "assistant",
            "content": [
                {
                    "type": "text",
                    "text": "Hello, how can I help you today?"
                }
            ],
            "model": "claude-3-sonnet-20240229",
            "stop_reason": "end_turn",
            "usage": {
                "input_tokens": 10,
                "output_tokens": 20
            }
        }
        
        interaction_id = str(uuid4())
        messages = EnhancedAnthropicConverter.from_native_messages(
            [anthropic_msg], 
            interaction_id=interaction_id
        )
        
        assert len(messages) == 1
        msg = messages[0]
        
        assert msg.id == "msg_123"
        assert msg.role == MessageRole.ASSISTANT
        assert msg.interaction_id == interaction_id
        assert msg.validity_state == ValidityState.ACTIVE
        assert len(msg.content) == 1
        
        text_block = msg.content[0]
        assert isinstance(text_block, EnhancedTextContentBlock)
        assert text_block.text == "Hello, how can I help you today?"
        assert text_block.interaction_context == "Anthropic text response"
        assert "anthropic_response" in text_block.content_tags
        
        # Check provider metadata
        assert msg.provider_metadata.provider_name == "anthropic"
        assert msg.provider_metadata.model == "claude-3-sonnet-20240229"
        assert msg.provider_metadata.stop_info.reason == "end_turn"
        assert msg.provider_metadata.usage.input_tokens == 10
        assert msg.provider_metadata.usage.output_tokens == 20
        
        # Check work log context
        assert msg.tool_context is not None
        assert msg.tool_context["provider"] == "anthropic"
        assert msg.tool_context["model"] == "claude-3-sonnet-20240229"
    
    def test_from_native_tool_use_message(self):
        """Test converting Anthropic message with tool use."""
        anthropic_msg = {
            "id": "msg_456",
            "type": "message",
            "role": "assistant",
            "content": [
                {
                    "type": "text",
                    "text": "I'll help you read that file."
                },
                {
                    "type": "tool_use",
                    "id": "tool_789",
                    "name": "workspace_read",
                    "input": {
                        "path": "//workspace/test.txt",
                        "encoding": "utf-8"
                    }
                }
            ],
            "model": "claude-3-sonnet-20240229",
            "stop_reason": "tool_use"
        }
        
        messages = EnhancedAnthropicConverter.from_native_messages([anthropic_msg])
        msg = messages[0]
        
        assert len(msg.content) == 2
        
        # Check text block
        text_block = msg.content[0]
        assert isinstance(text_block, EnhancedTextContentBlock)
        assert text_block.text == "I'll help you read that file."
        
        # Check tool use block
        tool_block = msg.content[1]
        assert isinstance(tool_block, EnhancedToolUseContentBlock)
        assert tool_block.tool_name == "workspace_read"
        assert tool_block.tool_id == "tool_789"
        assert tool_block.parameters["path"] == "//workspace/test.txt"
        assert tool_block.parameters["encoding"] == "utf-8"
        
        # Check work log metadata
        assert tool_block.work_log_metadata is not None
        assert tool_block.work_log_metadata["provider"] == "anthropic"
        assert tool_block.work_log_metadata["tool_call_id"] == "tool_789"
        
        # Check parameter importance
        assert tool_block.parameter_importance["path"] == 9  # Critical
        assert tool_block.parameter_importance["encoding"] == 7  # Medium-High
        
        # Check optimization hints
        assert "file_operation_batching_candidate" in tool_block.optimization_hints
    
    def test_from_native_thinking_message(self):
        """Test converting Anthropic message with thinking block."""
        anthropic_msg = {
            "id": "msg_thinking",
            "type": "message", 
            "role": "assistant",
            "content": [
                {
                    "type": "thinking",
                    "thinking": "I need to analyze this problem carefully. Let me think about the best approach to solve this issue."
                },
                {
                    "type": "text",
                    "text": "Based on my analysis, here's what I recommend..."
                }
            ],
            "model": "claude-3-sonnet-20240229"
        }
        
        messages = EnhancedAnthropicConverter.from_native_messages([anthropic_msg])
        msg = messages[0]
        
        assert len(msg.content) == 2
        
        # Check thinking block
        thinking_block = msg.content[0]
        assert isinstance(thinking_block, EnhancedThinkingContentBlock)
        assert "analyze this problem carefully" in thinking_block.thought
        assert not thinking_block.redacted
        assert thinking_block.reasoning_type == ReasoningType.ANALYSIS
        assert thinking_block.confidence_level is not None
        assert thinking_block.interaction_context == "Anthropic thinking block"
    
    def test_to_native_basic_conversion(self):
        """Test converting enhanced message back to Anthropic format."""
        enhanced_msg = EnhancedCommonChatMessage(
            id="msg_test",
            role=MessageRole.ASSISTANT,
            interaction_id=str(uuid4()),
            content=[
                EnhancedTextContentBlock(
                    text="Hello there!",
                    interaction_context="Test context"
                )
            ]
        )
        
        native_messages = EnhancedAnthropicConverter.to_native_messages([enhanced_msg])
        
        assert len(native_messages) == 1
        native_msg = native_messages[0]
        
        assert native_msg["type"] == "message"
        assert native_msg["role"] == "assistant"
        assert native_msg["id"] == "msg_test"
        assert len(native_msg["content"]) == 1
        
        content_block = native_msg["content"][0]
        assert content_block["type"] == "text"
        assert content_block["text"] == "Hello there!"
    
    def test_to_native_with_work_log_context(self):
        """Test converting with work log context included."""
        tool_block = EnhancedToolUseContentBlock(
            tool_name="workspace_read",
            tool_id="tool_123",
            parameters={"path": "/test/file.txt"},
            work_log_metadata={"test": "metadata"},
            optimization_hints=["caching_candidate"]
        )
        
        enhanced_msg = EnhancedCommonChatMessage(
            id="msg_test",
            role=MessageRole.ASSISTANT,
            interaction_id=str(uuid4()),
            content=[tool_block],
            tool_context={"context": "test"}
        )
        
        native_messages = EnhancedAnthropicConverter.to_native_messages(
            [enhanced_msg], 
            include_work_log_context=True
        )
        
        native_msg = native_messages[0]
        
        # Check work log context is included
        assert "_work_log_context" in native_msg
        assert native_msg["_work_log_context"]["context"] == "test"
        
        # Check tool block work log metadata
        tool_content = native_msg["content"][0]
        assert "_work_log_metadata" in tool_content
        assert tool_content["_work_log_metadata"]["test"] == "metadata"
        assert "_optimization_hints" in tool_content
        assert "caching_candidate" in tool_content["_optimization_hints"]
    
    def test_parameter_importance_analysis(self):
        """Test parameter importance analysis logic."""
        parameters = {
            "path": "/important/file.txt",
            "query": "search term",
            "recursive": True,
            "verbose_data": {"large": "data structure with lots of content"},
            "mode": "read"
        }
        
        importance = EnhancedAnthropicConverter._analyze_parameter_importance(parameters)
        
        assert importance["path"] == 9  # Critical
        assert importance["query"] == 8  # High
        assert importance["mode"] == 7  # Medium-High
        assert importance["recursive"] == 6  # Medium
        assert importance["verbose_data"] == 3  # Low (verbose)
    
    def test_reasoning_type_analysis(self):
        """Test reasoning type analysis from thinking text."""
        test_cases = [
            ("Let me analyze this problem", ReasoningType.ANALYSIS),
            ("I need to plan my approach", ReasoningType.PLANNING),
            ("Let me reflect on what happened", ReasoningType.REFLECTION),
            ("I need to solve this issue", ReasoningType.PROBLEM_SOLVING),
            ("I need to decide between options", ReasoningType.DECISION_MAKING),
            ("Just some random text", None)
        ]
        
        for text, expected in test_cases:
            result = EnhancedAnthropicConverter._analyze_reasoning_type(text)
            assert result == expected
    
    def test_confidence_level_estimation(self):
        """Test confidence level estimation from thinking text."""
        test_cases = [
            ("I'm certain this is correct", 0.9),
            ("This seems likely to work", 0.7),
            ("I'm unsure about this approach", 0.3),
            ("Neutral statement", None)
        ]
        
        for text, expected in test_cases:
            result = EnhancedAnthropicConverter._estimate_confidence_level(text)
            assert result == expected


class TestEnhancedOpenAIConverter:
    """Test enhanced OpenAI converter functionality."""
    
    def test_from_native_basic_message(self):
        """Test converting basic OpenAI message."""
        openai_msg = {
            "id": "msg_openai_123",
            "role": "assistant",
            "content": "Hello! How can I assist you today?",
            "finish_reason": "stop"
        }
        
        interaction_id = str(uuid4())
        messages = EnhancedOpenAIConverter.from_native_messages(
            [openai_msg], 
            interaction_id=interaction_id
        )
        
        assert len(messages) == 1
        msg = messages[0]
        
        assert msg.id == "msg_openai_123"
        assert msg.role == MessageRole.ASSISTANT
        assert msg.interaction_id == interaction_id
        assert len(msg.content) == 1
        
        text_block = msg.content[0]
        assert isinstance(text_block, EnhancedTextContentBlock)
        assert text_block.text == "Hello! How can I assist you today?"
        assert text_block.interaction_context == "OpenAI assistant message"
        assert "openai_response" in text_block.content_tags
    
    def test_from_native_tool_calls(self):
        """Test converting OpenAI message with tool calls."""
        openai_msg = {
            "id": "msg_tools",
            "role": "assistant",
            "content": None,
            "tool_calls": [
                {
                    "id": "call_123",
                    "type": "function",
                    "function": {
                        "name": "get_weather",
                        "arguments": '{"location": "San Francisco", "unit": "celsius"}'
                    }
                }
            ]
        }
        
        messages = EnhancedOpenAIConverter.from_native_messages([openai_msg])
        msg = messages[0]
        
        assert len(msg.content) == 1
        
        tool_block = msg.content[0]
        assert isinstance(tool_block, EnhancedToolUseContentBlock)
        assert tool_block.tool_name == "get_weather"
        assert tool_block.tool_id == "call_123"
        assert tool_block.parameters["location"] == "San Francisco"
        assert tool_block.parameters["unit"] == "celsius"
        
        # Check work log metadata
        assert tool_block.work_log_metadata is not None
        assert tool_block.work_log_metadata["provider"] == "openai"
        assert tool_block.work_log_metadata["tool_call_id"] == "call_123"
    
    def test_from_native_legacy_function_call(self):
        """Test converting OpenAI legacy function call."""
        openai_msg = {
            "role": "assistant",
            "content": None,
            "function_call": {
                "name": "search_web",
                "arguments": '{"query": "latest news"}'
            }
        }
        
        messages = EnhancedOpenAIConverter.from_native_messages([openai_msg])
        msg = messages[0]
        
        assert len(msg.content) == 1
        
        tool_block = msg.content[0]
        assert isinstance(tool_block, EnhancedToolUseContentBlock)
        assert tool_block.tool_name == "search_web"
        assert tool_block.parameters["query"] == "latest news"
        assert "function-search_web-" in tool_block.tool_id
        
        # Check legacy function call metadata
        assert tool_block.work_log_metadata["call_type"] == "legacy_function_call"
    
    def test_to_native_conversion(self):
        """Test converting enhanced message to OpenAI format."""
        enhanced_msg = EnhancedCommonChatMessage(
            id="msg_test",
            role=MessageRole.USER,
            interaction_id=str(uuid4()),
            content=[
                EnhancedTextContentBlock(text="What's the weather like?"),
                EnhancedToolUseContentBlock(
                    tool_name="get_weather",
                    tool_id="call_456",
                    parameters={"location": "New York"}
                )
            ]
        )
        
        native_messages = EnhancedOpenAIConverter.to_native_messages([enhanced_msg])
        
        assert len(native_messages) == 1
        native_msg = native_messages[0]
        
        assert native_msg["role"] == "user"
        assert native_msg["id"] == "msg_test"
        assert native_msg["content"] == "What's the weather like?"
        
        # Check tool calls
        assert "tool_calls" in native_msg
        assert len(native_msg["tool_calls"]) == 1
        
        tool_call = native_msg["tool_calls"][0]
        assert tool_call["id"] == "call_456"
        assert tool_call["type"] == "function"
        assert tool_call["function"]["name"] == "get_weather"
        
        function_args = json.loads(tool_call["function"]["arguments"])
        assert function_args["location"] == "New York"
    
    def test_parameter_importance_openai(self):
        """Test OpenAI-specific parameter importance analysis."""
        parameters = {
            "messages": [{"role": "user", "content": "Hello"}],
            "model": "gpt-4",
            "temperature": 0.7,
            "stream": True,
            "verbose_config": {"lots": "of configuration data"}
        }
        
        importance = EnhancedOpenAIConverter._analyze_parameter_importance(parameters)
        
        assert importance["messages"] == 9  # Critical
        assert importance["model"] == 8  # High
        assert importance["temperature"] == 8  # High
        assert importance["stream"] == 6  # Medium
        assert importance["verbose_config"] == 3  # Low (verbose)


class TestEnhancedProviderTranslationLayer:
    """Test the unified translation layer."""
    
    def test_initialization(self):
        """Test translation layer initialization."""
        layer = EnhancedProviderTranslationLayer()
        
        supported_providers = layer.get_supported_providers()
        assert ProviderType.ANTHROPIC in supported_providers
        assert ProviderType.OPENAI in supported_providers
        
        # Check capabilities
        anthropic_caps = layer.get_provider_capabilities(ProviderType.ANTHROPIC)
        assert ProviderCapability.THINKING_BLOCKS in anthropic_caps
        assert ProviderCapability.TOOL_CALLS in anthropic_caps
        
        openai_caps = layer.get_provider_capabilities(ProviderType.OPENAI)
        assert ProviderCapability.TOOL_CALLS in openai_caps
        assert ProviderCapability.FUNCTION_CALLING in openai_caps
    
    def test_from_native_messages_anthropic(self):
        """Test unified interface for Anthropic conversion."""
        layer = EnhancedProviderTranslationLayer()
        
        anthropic_msg = {
            "id": "msg_unified",
            "type": "message",
            "role": "assistant",
            "content": [{"type": "text", "text": "Unified test"}],
            "model": "claude-3-sonnet-20240229"
        }
        
        messages = layer.from_native_messages(
            [anthropic_msg], 
            ProviderType.ANTHROPIC
        )
        
        assert len(messages) == 1
        assert messages[0].provider_metadata.provider_name == "anthropic"
        
        # Check audit trail
        audit_trail = layer.get_audit_trail()
        assert len(audit_trail) == 1
        assert audit_trail[0]["operation"] == "from_native"
        assert audit_trail[0]["provider"] == "anthropic"
        assert audit_trail[0]["success"] is True
    
    def test_from_native_messages_openai(self):
        """Test unified interface for OpenAI conversion."""
        layer = EnhancedProviderTranslationLayer()
        
        openai_msg = {
            "role": "user",
            "content": "Test message for OpenAI"
        }
        
        messages = layer.from_native_messages(
            [openai_msg], 
            "openai"  # Test string provider type
        )
        
        assert len(messages) == 1
        assert messages[0].role == MessageRole.USER
        assert messages[0].provider_metadata.provider_name == "openai"
    
    def test_to_native_messages(self):
        """Test unified interface for native conversion."""
        layer = EnhancedProviderTranslationLayer()
        
        enhanced_msg = EnhancedCommonChatMessage(
            id="test_msg",
            role=MessageRole.ASSISTANT,
            interaction_id=str(uuid4()),
            content=[EnhancedTextContentBlock(text="Test response")]
        )
        
        # Test Anthropic conversion
        anthropic_msgs = layer.to_native_messages([enhanced_msg], ProviderType.ANTHROPIC)
        assert len(anthropic_msgs) == 1
        assert anthropic_msgs[0]["type"] == "message"
        
        # Test OpenAI conversion
        openai_msgs = layer.to_native_messages([enhanced_msg], "openai")
        assert len(openai_msgs) == 1
        assert openai_msgs[0]["role"] == "assistant"
    
    def test_unsupported_provider_error(self):
        """Test error handling for unsupported providers."""
        layer = EnhancedProviderTranslationLayer()
        
        with pytest.raises(TranslationError) as exc_info:
            layer.from_native_messages([], "unsupported_provider")
        
        assert "Unsupported provider" in str(exc_info.value)
        assert exc_info.value.provider == "unsupported_provider"
    
    def test_round_trip_validation_anthropic(self):
        """Test round-trip validation for Anthropic."""
        layer = EnhancedProviderTranslationLayer()
        
        original_messages = [
            EnhancedCommonChatMessage(
                id="test_1",
                role=MessageRole.USER,
                interaction_id=str(uuid4()),
                content=[EnhancedTextContentBlock(text="Hello")]
            ),
            EnhancedCommonChatMessage(
                id="test_2",
                role=MessageRole.ASSISTANT,
                interaction_id=str(uuid4()),
                content=[
                    EnhancedTextContentBlock(text="Hi there!"),
                    EnhancedToolUseContentBlock(
                        tool_name="test_tool",
                        tool_id="tool_123",
                        parameters={"param": "value"}
                    )
                ]
            )
        ]
        
        validation_result = layer.validate_round_trip(
            original_messages, 
            ProviderType.ANTHROPIC
        )
        
        assert validation_result["success"] is True
        assert validation_result["original_count"] == 2
        assert validation_result["round_trip_count"] == 2
        assert len(validation_result["differences"]) == 0
    
    def test_round_trip_validation_openai(self):
        """Test round-trip validation for OpenAI."""
        layer = EnhancedProviderTranslationLayer()
        
        original_messages = [
            EnhancedCommonChatMessage(
                id="openai_test",
                role=MessageRole.USER,
                interaction_id=str(uuid4()),
                content=[EnhancedTextContentBlock(text="OpenAI test")]
            )
        ]
        
        validation_result = layer.validate_round_trip(
            original_messages, 
            ProviderType.OPENAI
        )
        
        assert validation_result["success"] is True
        assert validation_result["original_count"] == 1
        assert validation_result["round_trip_count"] == 1
    
    def test_optimization_levels(self):
        """Test different optimization levels."""
        layer = EnhancedProviderTranslationLayer()
        
        # Create messages with different validity states
        active_msg = EnhancedCommonChatMessage(
            id="active",
            role=MessageRole.USER,
            interaction_id=str(uuid4()),
            content=[EnhancedTextContentBlock(text="Active message")],
            validity_state=ValidityState.ACTIVE
        )
        
        invalidated_msg = EnhancedCommonChatMessage(
            id="invalidated",
            role=MessageRole.USER,
            interaction_id=str(uuid4()),
            content=[EnhancedTextContentBlock(text="Invalidated message")],
            validity_state=ValidityState.INVALIDATED
        )
        
        messages = [active_msg, invalidated_msg]
        
        # Minimal optimization (includes all messages)
        minimal_result = layer.to_native_messages(
            messages, 
            ProviderType.ANTHROPIC, 
            optimization_level="minimal"
        )
        assert len(minimal_result) == 2
        
        # Standard optimization (excludes inactive messages)
        standard_result = layer.to_native_messages(
            messages, 
            ProviderType.ANTHROPIC, 
            optimization_level="standard"
        )
        assert len(standard_result) == 1  # Only active message
    
    def test_audit_trail_filtering(self):
        """Test audit trail filtering functionality."""
        layer = EnhancedProviderTranslationLayer()
        
        # Generate some audit entries
        test_msg = EnhancedCommonChatMessage(
            id="audit_test",
            role=MessageRole.USER,
            interaction_id=str(uuid4()),
            content=[EnhancedTextContentBlock(text="Audit test")]
        )
        
        # Perform operations to generate audit entries
        layer.to_native_messages([test_msg], ProviderType.ANTHROPIC)
        layer.to_native_messages([test_msg], ProviderType.OPENAI)
        
        # Test filtering
        all_entries = layer.get_audit_trail()
        assert len(all_entries) >= 2
        
        anthropic_entries = layer.get_audit_trail(provider_filter="anthropic")
        assert len(anthropic_entries) >= 1
        assert all(entry["provider"] == "anthropic" for entry in anthropic_entries)
        
        to_native_entries = layer.get_audit_trail(operation_filter="to_native")
        assert len(to_native_entries) >= 2
        assert all(entry["operation"] == "to_native" for entry in to_native_entries)
    
    def test_performance_stats(self):
        """Test performance statistics tracking."""
        layer = EnhancedProviderTranslationLayer()
        
        initial_stats = layer.get_performance_stats()
        assert initial_stats["total_translations"] == 0
        assert initial_stats["successful_translations"] == 0
        
        # Perform a successful translation
        test_msg = EnhancedCommonChatMessage(
            id="perf_test",
            role=MessageRole.USER,
            interaction_id=str(uuid4()),
            content=[EnhancedTextContentBlock(text="Performance test")]
        )
        
        layer.to_native_messages([test_msg], ProviderType.ANTHROPIC)
        
        updated_stats = layer.get_performance_stats()
        assert updated_stats["total_translations"] == 1
        assert updated_stats["successful_translations"] == 1
        assert updated_stats["failed_translations"] == 0
        assert updated_stats["average_duration"] > 0
    
    def test_error_handling_and_audit(self):
        """Test error handling with audit trail recording."""
        layer = EnhancedProviderTranslationLayer()
        
        # Create an invalid message that should cause an error
        invalid_messages = [{"invalid": "structure"}]
        
        with pytest.raises(TranslationError):
            layer.from_native_messages(invalid_messages, ProviderType.ANTHROPIC)
        
        # Check that failure was recorded in audit trail
        audit_trail = layer.get_audit_trail(success_filter=False)
        assert len(audit_trail) >= 1
        
        failed_entry = audit_trail[0]
        assert failed_entry["success"] is False
        assert failed_entry["error"] is not None
        
        # Check performance stats reflect the failure
        stats = layer.get_performance_stats()
        assert stats["failed_translations"] >= 1


class TestConvenienceFunctions:
    """Test convenience functions for common usage patterns."""
    
    def test_create_translation_layer(self):
        """Test translation layer creation function."""
        layer = create_translation_layer()
        assert isinstance(layer, EnhancedProviderTranslationLayer)
        assert len(layer.get_supported_providers()) >= 2
    
    def test_translate_from_anthropic(self):
        """Test Anthropic convenience function."""
        anthropic_msg = {
            "id": "convenience_test",
            "type": "message",
            "role": "assistant",
            "content": [{"type": "text", "text": "Convenience test"}],
            "model": "claude-3-sonnet-20240229"
        }
        
        messages = translate_from_anthropic([anthropic_msg])
        
        assert len(messages) == 1
        assert messages[0].provider_metadata.provider_name == "anthropic"
        assert messages[0].content[0].text == "Convenience test"
    
    def test_translate_from_openai(self):
        """Test OpenAI convenience function."""
        openai_msg = {
            "role": "user",
            "content": "OpenAI convenience test"
        }
        
        messages = translate_from_openai([openai_msg])
        
        assert len(messages) == 1
        assert messages[0].role == MessageRole.USER
        assert messages[0].content[0].text == "OpenAI convenience test"
    
    def test_translate_to_anthropic(self):
        """Test Anthropic output convenience function."""
        enhanced_msg = EnhancedCommonChatMessage(
            id="to_anthropic_test",
            role=MessageRole.ASSISTANT,
            interaction_id=str(uuid4()),
            content=[EnhancedTextContentBlock(text="To Anthropic test")]
        )
        
        native_messages = translate_to_anthropic([enhanced_msg])
        
        assert len(native_messages) == 1
        assert native_messages[0]["type"] == "message"
        assert native_messages[0]["content"][0]["text"] == "To Anthropic test"
    
    def test_translate_to_openai(self):
        """Test OpenAI output convenience function."""
        enhanced_msg = EnhancedCommonChatMessage(
            id="to_openai_test",
            role=MessageRole.USER,
            interaction_id=str(uuid4()),
            content=[EnhancedTextContentBlock(text="To OpenAI test")]
        )
        
        native_messages = translate_to_openai([enhanced_msg])
        
        assert len(native_messages) == 1
        assert native_messages[0]["role"] == "user"
        assert native_messages[0]["content"] == "To OpenAI test"
    
    def test_translate_with_work_log_context(self):
        """Test convenience functions with work log context."""
        tool_msg = EnhancedCommonChatMessage(
            id="work_log_test",
            role=MessageRole.ASSISTANT,
            interaction_id=str(uuid4()),
            content=[
                EnhancedToolUseContentBlock(
                    tool_name="test_tool",
                    tool_id="tool_123",
                    parameters={"key": "value"},
                    work_log_metadata={"test": "metadata"}
                )
            ]
        )
        
        # Test with work log context
        native_messages = translate_to_anthropic([tool_msg], include_work_log_context=True)
        
        assert len(native_messages) == 1
        tool_content = native_messages[0]["content"][0]
        assert "_work_log_metadata" in tool_content
        assert tool_content["_work_log_metadata"]["test"] == "metadata"


class TestEdgeCasesAndErrorHandling:
    """Test edge cases and error handling scenarios."""
    
    def test_empty_message_list(self):
        """Test handling of empty message lists."""
        layer = EnhancedProviderTranslationLayer()
        
        # Empty list should return empty list
        result = layer.from_native_messages([], ProviderType.ANTHROPIC)
        assert result == []
        
        result = layer.to_native_messages([], ProviderType.OPENAI)
        assert result == []
    
    def test_malformed_json_in_tool_arguments(self):
        """Test handling of malformed JSON in tool arguments."""
        openai_msg = {
            "role": "assistant",
            "tool_calls": [
                {
                    "id": "call_malformed",
                    "type": "function",
                    "function": {
                        "name": "test_tool",
                        "arguments": '{"invalid": json structure'  # Malformed JSON
                    }
                }
            ]
        }
        
        messages = EnhancedOpenAIConverter.from_native_messages([openai_msg])
        
        assert len(messages) == 1
        tool_block = messages[0].content[0]
        assert isinstance(tool_block, EnhancedToolUseContentBlock)
        
        # Should fallback to raw_arguments
        assert "raw_arguments" in tool_block.parameters
        assert tool_block.parameters["raw_arguments"] == '{"invalid": json structure'
    
    def test_missing_required_fields(self):
        """Test handling of messages with missing required fields."""
        # Anthropic message without content
        anthropic_msg = {
            "id": "missing_content",
            "type": "message",
            "role": "assistant"
            # Missing content field
        }
        
        messages = EnhancedAnthropicConverter.from_native_messages([anthropic_msg])
        
        assert len(messages) == 1
        assert len(messages[0].content) == 0  # Empty content list
    
    def test_unknown_content_block_types(self):
        """Test handling of unknown content block types."""
        anthropic_msg = {
            "id": "unknown_block",
            "type": "message",
            "role": "assistant",
            "content": [
                {
                    "type": "unknown_block_type",
                    "data": "some data"
                },
                {
                    "type": "text",
                    "text": "Valid text block"
                }
            ]
        }
        
        messages = EnhancedAnthropicConverter.from_native_messages([anthropic_msg])
        
        assert len(messages) == 1
        # Should skip unknown block type but include valid text block
        assert len(messages[0].content) == 1
        assert isinstance(messages[0].content[0], EnhancedTextContentBlock)
    
    def test_large_parameter_values(self):
        """Test handling of large parameter values."""
        large_data = "x" * 10000  # 10KB of data
        
        anthropic_msg = {
            "id": "large_params",
            "type": "message",
            "role": "assistant",
            "content": [
                {
                    "type": "tool_use",
                    "id": "tool_large",
                    "name": "process_data",
                    "input": {
                        "small_param": "value",
                        "large_param": large_data
                    }
                }
            ]
        }
        
        messages = EnhancedAnthropicConverter.from_native_messages([anthropic_msg])
        
        assert len(messages) == 1
        tool_block = messages[0].content[0]
        
        # Large parameter should get low importance
        assert tool_block.parameter_importance["large_param"] == 3  # Low importance
        assert tool_block.parameter_importance["small_param"] == 5  # Default
    
    def test_interaction_id_generation(self):
        """Test automatic interaction ID generation."""
        anthropic_msg = {
            "id": "auto_interaction",
            "type": "message",
            "role": "assistant",
            "content": [{"type": "text", "text": "Auto interaction test"}]
        }
        
        # Don't provide interaction_id
        messages = EnhancedAnthropicConverter.from_native_messages([anthropic_msg])
        
        assert len(messages) == 1
        assert messages[0].interaction_id is not None
        assert len(messages[0].interaction_id) > 0
        
        # Multiple messages should get the same auto-generated interaction_id
        messages2 = EnhancedAnthropicConverter.from_native_messages([anthropic_msg, anthropic_msg])
        assert messages2[0].interaction_id == messages2[1].interaction_id


if __name__ == "__main__":
    pytest.main([__file__, "-v"])
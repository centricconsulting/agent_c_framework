"""
Tests for message analysis utilities.
"""

import pytest
from datetime import datetime
from agent_c.util.pruning.message_analyzer import (
    is_system_message,
    is_correction_message,
    is_tool_call_message,
    is_tool_result_message,
    find_tool_call_pairs,
    calculate_total_tokens,
    get_recent_message_indices,
    get_tool_call_related_indices
)
from agent_c.models.chat_history.memory_message import MemoryMessage
from agent_c.models.common_chat.models import (
    CommonChatMessage,
    MessageRole,
    TextContentBlock,
    ToolUseContentBlock,
    ToolResultContentBlock,
    ContentBlockType
)


class TestMessageAnalyzer:
    """Test suite for message analysis functions."""

    def test_is_system_message(self):
        """Test system message detection."""
        system_msg = MemoryMessage(role="system", content="You are a helpful assistant")
        user_msg = MemoryMessage(role="user", content="Hello")
        assistant_msg = MemoryMessage(role="assistant", content="Hi there")
        
        assert is_system_message(system_msg)
        assert not is_system_message(user_msg)
        assert not is_system_message(assistant_msg)
        
        # Test case insensitive
        system_msg_upper = MemoryMessage(role="SYSTEM", content="System message")
        assert is_system_message(system_msg_upper)

    def test_is_correction_message(self):
        """Test correction message detection."""
        keywords = ["actually", "wrong", "fix", "correction"]
        
        # Messages that should be detected as corrections
        correction_msg1 = MemoryMessage(role="user", content="Actually, I meant something else")
        correction_msg2 = MemoryMessage(role="user", content="That's wrong, let me clarify")
        correction_msg3 = MemoryMessage(role="user", content="Please fix this issue")
        correction_msg4 = MemoryMessage(role="user", content="ACTUALLY, that's not right")  # Case insensitive
        
        assert is_correction_message(correction_msg1, keywords)
        assert is_correction_message(correction_msg2, keywords)
        assert is_correction_message(correction_msg3, keywords)
        assert is_correction_message(correction_msg4, keywords)
        
        # Messages that should NOT be detected as corrections
        normal_msg = MemoryMessage(role="user", content="This is a normal message")
        partial_match_msg = MemoryMessage(role="user", content="factually speaking")  # Partial match
        empty_msg = MemoryMessage(role="user", content="")
        
        assert not is_correction_message(normal_msg, keywords)
        assert not is_correction_message(partial_match_msg, keywords)
        assert not is_correction_message(empty_msg, keywords)
        
        # Test with empty keywords
        assert not is_correction_message(correction_msg1, [])

    def test_is_tool_call_message(self):
        """Test tool call message detection."""
        # Message with tool call
        tool_call_msg = CommonChatMessage(
            id="msg1",
            role=MessageRole.ASSISTANT,
            content=[
                TextContentBlock(text="I'll help you with that."),
                ToolUseContentBlock(
                    tool_name="search",
                    tool_id="tool_123",
                    parameters={"query": "test"}
                )
            ],
            created_at=datetime.now()
        )
        
        # Message without tool call
        text_only_msg = CommonChatMessage(
            id="msg2",
            role=MessageRole.ASSISTANT,
            content=[TextContentBlock(text="Just a text message")],
            created_at=datetime.now()
        )
        
        assert is_tool_call_message(tool_call_msg)
        assert not is_tool_call_message(text_only_msg)

    def test_is_tool_result_message(self):
        """Test tool result message detection."""
        # Message with tool result
        tool_result_msg = CommonChatMessage(
            id="msg1",
            role=MessageRole.TOOL,
            content=[
                ToolResultContentBlock(
                    tool_name="search",
                    tool_id="tool_123",
                    result={"status": "success", "data": "result"}
                )
            ],
            created_at=datetime.now()
        )
        
        # Message without tool result
        text_only_msg = CommonChatMessage(
            id="msg2",
            role=MessageRole.USER,
            content=[TextContentBlock(text="Just a text message")],
            created_at=datetime.now()
        )
        
        assert is_tool_result_message(tool_result_msg)
        assert not is_tool_result_message(text_only_msg)

    def test_find_tool_call_pairs(self):
        """Test tool call pair detection."""
        messages = [
            # Regular message
            CommonChatMessage(
                id="msg1",
                role=MessageRole.USER,
                content=[TextContentBlock(text="Hello")],
                created_at=datetime.now()
            ),
            # Tool call message
            CommonChatMessage(
                id="msg2",
                role=MessageRole.ASSISTANT,
                content=[
                    ToolUseContentBlock(
                        tool_name="search",
                        tool_id="tool_123",
                        parameters={"query": "test"}
                    )
                ],
                created_at=datetime.now()
            ),
            # Tool result message
            CommonChatMessage(
                id="msg3",
                role=MessageRole.TOOL,
                content=[
                    ToolResultContentBlock(
                        tool_name="search",
                        tool_id="tool_123",
                        result={"data": "result"}
                    )
                ],
                created_at=datetime.now()
            ),
            # Another tool call
            CommonChatMessage(
                id="msg4",
                role=MessageRole.ASSISTANT,
                content=[
                    ToolUseContentBlock(
                        tool_name="calculator",
                        tool_id="tool_456",
                        parameters={"expression": "2+2"}
                    )
                ],
                created_at=datetime.now()
            ),
            # Corresponding result
            CommonChatMessage(
                id="msg5",
                role=MessageRole.TOOL,
                content=[
                    ToolResultContentBlock(
                        tool_name="calculator",
                        tool_id="tool_456",
                        result={"result": 4}
                    )
                ],
                created_at=datetime.now()
            )
        ]
        
        pairs = find_tool_call_pairs(messages)
        
        # Should find two pairs: (1,2) and (3,4)
        assert len(pairs) == 2
        assert (1, 2) in pairs
        assert (3, 4) in pairs

    def test_calculate_total_tokens(self):
        """Test token calculation."""
        messages = [
            MemoryMessage(role="user", content="Hello", token_count=5),
            MemoryMessage(role="assistant", content="Hi there", token_count=10),
            MemoryMessage(role="user", content="How are you?", token_count=8)
        ]
        
        total = calculate_total_tokens(messages)
        assert total == 23

    def test_get_recent_message_indices(self):
        """Test recent message index calculation."""
        messages = [
            MemoryMessage(role="user", content="msg1"),
            MemoryMessage(role="assistant", content="msg2"),
            MemoryMessage(role="user", content="msg3"),
            MemoryMessage(role="assistant", content="msg4"),
            MemoryMessage(role="user", content="msg5")
        ]
        
        # Get last 2 messages
        recent_indices = get_recent_message_indices(messages, 2)
        assert recent_indices == {3, 4}
        
        # Get last 3 messages
        recent_indices = get_recent_message_indices(messages, 3)
        assert recent_indices == {2, 3, 4}
        
        # Get more messages than available
        recent_indices = get_recent_message_indices(messages, 10)
        assert recent_indices == {0, 1, 2, 3, 4}
        
        # Get 0 messages
        recent_indices = get_recent_message_indices(messages, 0)
        assert recent_indices == set()

    def test_get_tool_call_related_indices(self):
        """Test tool call related indices calculation."""
        messages = [
            # Regular message - index 0
            CommonChatMessage(
                id="msg1",
                role=MessageRole.USER,
                content=[TextContentBlock(text="Hello")],
                created_at=datetime.now()
            ),
            # Tool call message - index 1
            CommonChatMessage(
                id="msg2",
                role=MessageRole.ASSISTANT,
                content=[
                    ToolUseContentBlock(
                        tool_name="search",
                        tool_id="tool_123",
                        parameters={"query": "test"}
                    )
                ],
                created_at=datetime.now()
            ),
            # Tool result message - index 2
            CommonChatMessage(
                id="msg3",
                role=MessageRole.TOOL,
                content=[
                    ToolResultContentBlock(
                        tool_name="search",
                        tool_id="tool_123",
                        result={"data": "result"}
                    )
                ],
                created_at=datetime.now()
            ),
            # Regular message - index 3
            CommonChatMessage(
                id="msg4",
                role=MessageRole.USER,
                content=[TextContentBlock(text="Thanks")],
                created_at=datetime.now()
            )
        ]
        
        tool_indices = get_tool_call_related_indices(messages)
        
        # Should include indices 1 and 2 (tool call and result)
        assert tool_indices == {1, 2}
"""
Message analysis utilities for chat log pruning.

This module provides functions to classify and analyze messages for pruning decisions,
including detection of system messages, corrections, tool calls, and token counting.
"""

from typing import List, Tuple, Set
from agent_c.models.chat_history.memory_message import MemoryMessage
from agent_c.models.common_chat.models import (
    CommonChatMessage, 
    MessageRole, 
    ContentBlockType,
    ToolUseContentBlock,
    ToolResultContentBlock
)


def is_system_message(message: MemoryMessage) -> bool:
    """
    Check if a message is a system message.
    
    Args:
        message: The message to analyze
        
    Returns:
        True if the message has a system role
    """
    return message.role.lower() == "system"


def is_correction_message(message: MemoryMessage, keywords: List[str]) -> bool:
    """
    Check if a message contains correction keywords.
    
    Args:
        message: The message to analyze
        keywords: List of correction keywords to search for
        
    Returns:
        True if the message content contains any correction keywords as complete words
    """
    if not keywords:
        return False
        
    # Extract text content from the message
    content = ""
    if isinstance(message.content, str):
        content = message.content
    elif isinstance(message.content, list):
        # Handle list of content blocks - extract text from each
        for item in message.content:
            if hasattr(item, 'text'):
                content += item.text + " "
            elif hasattr(item, 'content') and isinstance(item.content, str):
                content += item.content + " "
    
    if not content:
        return False
    
    import re
    content_lower = content.lower()
    
    # Use word boundaries to match complete words only
    for keyword in keywords:
        pattern = r'\b' + re.escape(keyword.lower()) + r'\b'
        if re.search(pattern, content_lower):
            return True
    return False


def is_tool_call_message(message: CommonChatMessage) -> bool:
    """
    Check if a message contains tool calls.
    
    Args:
        message: The message to analyze
        
    Returns:
        True if the message contains any tool use content blocks
    """
    return any(
        block.type == ContentBlockType.TOOL_USE 
        for block in message.content
    )


def is_tool_result_message(message: CommonChatMessage) -> bool:
    """
    Check if a message contains tool results.
    
    Args:
        message: The message to analyze
        
    Returns:
        True if the message contains any tool result content blocks
    """
    return any(
        block.type == ContentBlockType.TOOL_RESULT 
        for block in message.content
    )


def find_tool_call_pairs(messages: List[CommonChatMessage]) -> List[Tuple[int, int]]:
    """
    Find pairs of tool call and tool result messages.
    
    This function identifies tool calls and their corresponding results to ensure
    they are preserved or removed together during pruning.
    
    Args:
        messages: List of messages to analyze
        
    Returns:
        List of tuples (call_index, result_index) representing paired tool calls
    """
    pairs = []
    tool_calls = {}  # tool_id -> message_index
    
    for i, message in enumerate(messages):
        # Find tool calls and store their positions
        for block in message.content:
            if isinstance(block, ToolUseContentBlock):
                tool_calls[block.tool_id] = i
        
        # Find tool results and match them with calls
        for block in message.content:
            if isinstance(block, ToolResultContentBlock):
                if block.tool_id in tool_calls:
                    call_index = tool_calls[block.tool_id]
                    pairs.append((call_index, i))
                    # Remove from tracking to avoid duplicate pairs
                    del tool_calls[block.tool_id]
    
    return pairs


def calculate_total_tokens(messages: List[MemoryMessage]) -> int:
    """
    Calculate the total token count for a list of messages.
    
    Args:
        messages: List of messages to count tokens for
        
    Returns:
        Total token count across all messages
    """
    total = 0
    for message in messages:
        if message.token_count is not None:
            total += message.token_count
        else:
            # Fallback: if token_count is None, it should have been computed
            # during validation, but we'll handle the edge case
            if isinstance(message.content, str):
                from agent_c.util.token_counter import TokenCounter
                total += TokenCounter.count_tokens(message.content)
            elif isinstance(message.content, list):
                # Sum tokens from content blocks
                for item in message.content:
                    if hasattr(item, 'count_tokens'):
                        total += item.count_tokens()
    
    return total


def get_recent_message_indices(messages: List[MemoryMessage], count: int) -> Set[int]:
    """
    Get indices of the most recent messages.
    
    Args:
        messages: List of messages
        count: Number of recent messages to preserve
        
    Returns:
        Set of indices for the most recent messages
    """
    if count <= 0 or not messages:
        return set()
    
    # Return indices of the last 'count' messages
    start_index = max(0, len(messages) - count)
    return set(range(start_index, len(messages)))


def get_tool_call_related_indices(messages: List[CommonChatMessage]) -> Set[int]:
    """
    Get indices of all messages that are part of tool call pairs.
    
    Args:
        messages: List of messages to analyze
        
    Returns:
        Set of indices for messages involved in tool calls
    """
    indices = set()
    pairs = find_tool_call_pairs(messages)
    
    for call_index, result_index in pairs:
        indices.add(call_index)
        indices.add(result_index)
    
    return indices
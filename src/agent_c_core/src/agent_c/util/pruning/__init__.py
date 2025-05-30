"""
Chat log pruning utilities for Agent C.

This module provides intelligent message removal capabilities while preserving
conversation coherence, system context, and tool call integrity.
"""

from .config import PrunerConfig
from .message_analyzer import (
    is_system_message,
    is_correction_message,
    is_tool_call_message,
    is_tool_result_message,
    find_tool_call_pairs,
    calculate_total_tokens,
    get_recent_message_indices,
    get_tool_call_related_indices
)
from .chat_log_pruner import ChatLogPruner, PruningResult

__all__ = [
    'PrunerConfig',
    'ChatLogPruner',
    'PruningResult',
    'is_system_message',
    'is_correction_message',
    'is_tool_call_message',
    'is_tool_result_message',
    'find_tool_call_pairs',
    'calculate_total_tokens',
    'get_recent_message_indices',
    'get_tool_call_related_indices'
]
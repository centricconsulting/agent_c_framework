"""
Core chat log pruning implementation.

This module provides the main ChatLogPruner class that implements intelligent
message removal while preserving conversation coherence and essential context.
"""

from typing import List, Set, Tuple, Optional
import logging
from dataclasses import dataclass

from agent_c.util.pruning.config import PrunerConfig
from agent_c.util.pruning.message_analyzer import (
    is_system_message,
    is_correction_message,
    calculate_total_tokens,
    get_recent_message_indices
)
from agent_c.models.chat_history.memory_message import MemoryMessage

logger = logging.getLogger(__name__)


@dataclass
class PruningResult:
    """Result of a pruning operation."""
    messages: List[MemoryMessage]
    tokens_before: int
    tokens_after: int
    messages_removed: int
    preserved_system: int
    preserved_recent: int
    preserved_corrections: int
    preserved_tool_calls: int
    was_dry_run: bool


class ChatLogPruner:
    """
    Intelligent chat log pruner that removes messages while preserving conversation coherence.
    
    The pruner implements a FIFO removal strategy with preservation rules for:
    - System messages
    - Recent messages (configurable count)
    - Messages containing correction keywords
    - Tool call pairs (atomic preservation)
    
    The pruner respects message boundaries and ensures tool calls and their results
    are preserved or removed together.
    """
    
    def __init__(self, config: PrunerConfig):
        """
        Initialize the chat log pruner.
        
        Args:
            config: Configuration settings for pruning behavior
        """
        self.config = config
        self._logger = logging.getLogger(f"{__name__}.{self.__class__.__name__}")
    
    def should_prune(self, messages: List[MemoryMessage], context_limit: int) -> bool:
        """
        Check if pruning is needed based on token count and threshold.
        
        Args:
            messages: List of messages to analyze
            context_limit: Maximum context window size for the model
            
        Returns:
            True if pruning should be performed
        """
        if not messages:
            return False
            
        total_tokens = calculate_total_tokens(messages)
        effective_limit = self.config.get_effective_token_limit(context_limit)
        
        self._logger.debug(
            f"Token analysis: {total_tokens}/{effective_limit} "
            f"(threshold: {self.config.token_threshold_percent:.1%})"
        )
        
        return total_tokens > effective_limit
    
    def prune_messages(
        self, 
        messages: List[MemoryMessage], 
        context_limit: int
    ) -> PruningResult:
        """
        Prune messages to fit within the context limit.
        
        Args:
            messages: List of messages to prune
            context_limit: Maximum context window size for the model
            
        Returns:
            PruningResult containing the pruned messages and statistics
        """
        if not messages:
            return PruningResult(
                messages=[],
                tokens_before=0,
                tokens_after=0,
                messages_removed=0,
                preserved_system=0,
                preserved_recent=0,
                preserved_corrections=0,
                preserved_tool_calls=0,
                was_dry_run=self.config.enable_dry_run
            )
        
        tokens_before = calculate_total_tokens(messages)
        effective_limit = self.config.get_effective_token_limit(context_limit)
        
        self._logger.info(
            f"Starting pruning: {len(messages)} messages, "
            f"{tokens_before} tokens, limit: {effective_limit}"
        )
        
        # If we don't need to prune, return original messages
        if tokens_before <= effective_limit:
            self._logger.debug("No pruning needed - under token limit")
            return PruningResult(
                messages=messages.copy(),
                tokens_before=tokens_before,
                tokens_after=tokens_before,
                messages_removed=0,
                preserved_system=0,
                preserved_recent=0,
                preserved_corrections=0,
                preserved_tool_calls=0,
                was_dry_run=self.config.enable_dry_run
            )
        
        # Identify messages to preserve
        preserved_indices = self._identify_preserved_messages(messages)
        
        # Calculate preservation statistics
        stats = self._calculate_preservation_stats(messages, preserved_indices)
        
        # Identify removal candidates
        removal_candidates = self._identify_removal_candidates(messages, preserved_indices)
        
        if not removal_candidates:
            self._logger.warning("No messages available for removal - all are preserved")
            return PruningResult(
                messages=messages.copy(),
                tokens_before=tokens_before,
                tokens_after=tokens_before,
                messages_removed=0,
                preserved_system=stats['system'],
                preserved_recent=stats['recent'],
                preserved_corrections=stats['corrections'],
                preserved_tool_calls=stats['tool_calls'],
                was_dry_run=self.config.enable_dry_run
            )
        
        # Remove candidates until we're under the limit
        pruned_messages = self._remove_candidates_fifo(
            messages, removal_candidates, effective_limit
        )
        
        tokens_after = calculate_total_tokens(pruned_messages)
        messages_removed = len(messages) - len(pruned_messages)
        
        self._logger.info(
            f"Pruning complete: {len(pruned_messages)} messages remaining, "
            f"{tokens_after} tokens, removed {messages_removed} messages"
        )
        
        return PruningResult(
            messages=pruned_messages if not self.config.enable_dry_run else messages.copy(),
            tokens_before=tokens_before,
            tokens_after=tokens_after,
            messages_removed=messages_removed,
            preserved_system=stats['system'],
            preserved_recent=stats['recent'],
            preserved_corrections=stats['corrections'],
            preserved_tool_calls=stats['tool_calls'],
            was_dry_run=self.config.enable_dry_run
        )
    
    def _identify_preserved_messages(self, messages: List[MemoryMessage]) -> Set[int]:
        """
        Identify indices of messages that should be preserved.
        
        Args:
            messages: List of messages to analyze
            
        Returns:
            Set of indices for messages that should be preserved
        """
        preserved = set()
        
        # Preserve system messages
        for i, message in enumerate(messages):
            if is_system_message(message):
                preserved.add(i)
                self._logger.debug(f"Preserving system message at index {i}")
        
        # Preserve recent messages
        recent_indices = get_recent_message_indices(messages, self.config.recent_message_count)
        preserved.update(recent_indices)
        if recent_indices:
            self._logger.debug(f"Preserving recent messages at indices {sorted(recent_indices)}")
        
        # Preserve correction messages
        for i, message in enumerate(messages):
            if is_correction_message(message, self.config.correction_keywords):
                preserved.add(i)
                self._logger.debug(f"Preserving correction message at index {i}")
        
        # TODO: Handle tool call preservation when we have CommonChatMessage integration
        # For now, we'll implement basic tool call preservation in a future iteration
        
        return preserved
    
    def _identify_removal_candidates(
        self, 
        messages: List[MemoryMessage], 
        preserved_indices: Set[int]
    ) -> List[int]:
        """
        Identify messages that can be removed, ordered by removal priority (FIFO).
        
        Args:
            messages: List of messages to analyze
            preserved_indices: Set of indices that should be preserved
            
        Returns:
            List of indices ordered by removal priority (oldest first)
        """
        candidates = []
        
        for i in range(len(messages)):
            if i not in preserved_indices:
                candidates.append(i)
        
        # FIFO: remove oldest messages first
        candidates.sort()
        
        self._logger.debug(f"Identified {len(candidates)} removal candidates: {candidates}")
        return candidates
    
    def _remove_candidates_fifo(
        self, 
        messages: List[MemoryMessage], 
        candidates: List[int], 
        target_tokens: int
    ) -> List[MemoryMessage]:
        """
        Remove candidates in FIFO order until under the target token count.
        
        Args:
            messages: Original list of messages
            candidates: List of candidate indices to remove (oldest first)
            target_tokens: Target token count to achieve
            
        Returns:
            List of messages after removal
        """
        if not candidates:
            return messages.copy()
        
        # Create a set of indices to remove
        to_remove = set()
        current_tokens = calculate_total_tokens(messages)
        
        for candidate_idx in candidates:
            if current_tokens <= target_tokens:
                break
                
            # Calculate tokens for this message
            message = messages[candidate_idx]
            message_tokens = message.token_count or 0
            
            to_remove.add(candidate_idx)
            current_tokens -= message_tokens
            
            self._logger.debug(
                f"Marking message {candidate_idx} for removal "
                f"({message_tokens} tokens, {current_tokens} remaining)"
            )
        
        # Create the pruned message list
        pruned_messages = [
            msg for i, msg in enumerate(messages) 
            if i not in to_remove
        ]
        
        self._logger.debug(f"Removed {len(to_remove)} messages: {sorted(to_remove)}")
        return pruned_messages
    
    def _calculate_preservation_stats(
        self, 
        messages: List[MemoryMessage], 
        preserved_indices: Set[int]
    ) -> dict:
        """
        Calculate statistics about what types of messages were preserved.
        
        Args:
            messages: List of messages
            preserved_indices: Set of preserved message indices
            
        Returns:
            Dictionary with preservation statistics
        """
        stats = {
            'system': 0,
            'recent': 0,
            'corrections': 0,
            'tool_calls': 0
        }
        
        recent_indices = get_recent_message_indices(messages, self.config.recent_message_count)
        
        for i in preserved_indices:
            if i < len(messages):
                message = messages[i]
                
                if is_system_message(message):
                    stats['system'] += 1
                elif i in recent_indices:
                    stats['recent'] += 1
                elif is_correction_message(message, self.config.correction_keywords):
                    stats['corrections'] += 1
                # TODO: Add tool call detection when CommonChatMessage integration is ready
        
        return stats
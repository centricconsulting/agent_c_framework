"""
Comprehensive scenario tests for chat log pruning.

This module tests complex real-world scenarios and edge cases to ensure
the pruning system works correctly in all situations.
"""

import pytest
import time
from typing import List
from agent_c.util.pruning.chat_log_pruner import ChatLogPruner, PruningResult
from agent_c.util.pruning.config import PrunerConfig
from agent_c.models.chat_history.memory_message import MemoryMessage


class TestComprehensiveScenarios:
    """Test suite for comprehensive pruning scenarios."""

    @pytest.fixture
    def default_config(self):
        """Default configuration for testing."""
        return PrunerConfig(
            token_threshold_percent=0.6,
            recent_message_count=15,
            correction_keywords=["actually", "wrong", "fix", "correction", "mistake"]
        )

    @pytest.fixture
    def large_conversation(self):
        """Create a large conversation for testing."""
        messages = []
        
        # System message
        messages.append(MemoryMessage(
            role="system", 
            content="You are a helpful AI assistant.", 
            token_count=50
        ))
        
        # Add varied conversation
        for i in range(50):
            # User messages
            if i % 10 == 5:
                # Add some correction messages
                messages.append(MemoryMessage(
                    role="user",
                    content=f"Actually, I meant to ask about topic {i}",
                    token_count=80
                ))
            else:
                messages.append(MemoryMessage(
                    role="user",
                    content=f"This is user message {i} with some content",
                    token_count=60
                ))
            
            # Assistant responses
            messages.append(MemoryMessage(
                role="assistant",
                content=f"This is assistant response {i} with detailed information",
                token_count=70
            ))
        
        return messages

    def test_empty_conversation(self, default_config):
        """Test pruning empty conversation."""
        pruner = ChatLogPruner(default_config)
        result = pruner.prune_messages([], 1000)
        
        assert result.messages == []
        assert result.tokens_before == 0
        assert result.tokens_after == 0
        assert result.messages_removed == 0

    def test_single_message_conversation(self, default_config):
        """Test pruning conversation with single message."""
        pruner = ChatLogPruner(default_config)
        messages = [MemoryMessage(role="user", content="Hello", token_count=10)]
        
        result = pruner.prune_messages(messages, 1000)
        
        assert len(result.messages) == 1
        assert result.messages[0].content == "Hello"
        assert result.messages_removed == 0

    def test_all_messages_preserved_scenario(self, default_config):
        """Test scenario where all messages should be preserved."""
        # Configure to preserve many recent messages
        config = PrunerConfig(
            token_threshold_percent=0.5,
            recent_message_count=50,  # Preserve all messages
            correction_keywords=["fix"]
        )
        pruner = ChatLogPruner(config)
        
        messages = [
            MemoryMessage(role="system", content="System", token_count=100),
            MemoryMessage(role="user", content="Please fix this", token_count=100),
            MemoryMessage(role="user", content="Message 1", token_count=100),
            MemoryMessage(role="user", content="Message 2", token_count=100),
        ]
        
        result = pruner.prune_messages(messages, 2000)
        
        # All messages should be preserved
        assert len(result.messages) == len(messages)
        assert result.messages_removed == 0

    def test_mixed_preservation_rules(self, default_config):
        """Test complex scenario with multiple preservation rules."""
        pruner = ChatLogPruner(default_config)
        
        messages = [
            # System message (should be preserved)
            MemoryMessage(role="system", content="You are helpful", token_count=100),
            
            # Old messages (candidates for removal)
            MemoryMessage(role="user", content="Old question 1", token_count=100),
            MemoryMessage(role="assistant", content="Old answer 1", token_count=100),
            MemoryMessage(role="user", content="Old question 2", token_count=100),
            MemoryMessage(role="assistant", content="Old answer 2", token_count=100),
            
            # Correction message (should be preserved)
            MemoryMessage(role="user", content="Actually, I meant something different", token_count=100),
            MemoryMessage(role="assistant", content="I understand the correction", token_count=100),
            
            # More old messages
            MemoryMessage(role="user", content="Another old question", token_count=100),
            MemoryMessage(role="assistant", content="Another old answer", token_count=100),
            
            # Recent messages (should be preserved due to recent_message_count=15)
        ]
        
        # Add recent messages
        for i in range(15):
            messages.append(MemoryMessage(
                role="user", 
                content=f"Recent message {i}", 
                token_count=100
            ))
        
        # Total: 2400 tokens, threshold at 60% of 3000 = 1800 tokens
        result = pruner.prune_messages(messages, 3000)
        
        # Verify preservation rules
        remaining_content = [msg.content for msg in result.messages]
        
        # System message should be preserved
        assert "You are helpful" in remaining_content
        
        # Correction message should be preserved
        assert "Actually, I meant something different" in remaining_content
        
        # Recent messages should be preserved
        assert "Recent message 14" in remaining_content
        
        # Some old messages should be removed
        assert result.messages_removed > 0

    def test_conversation_boundaries_respected(self, default_config):
        """Test that user-assistant message pairs are handled properly."""
        pruner = ChatLogPruner(default_config)
        
        messages = []
        # Create conversation pairs
        for i in range(20):
            messages.append(MemoryMessage(
                role="user",
                content=f"User question {i}",
                token_count=100
            ))
            messages.append(MemoryMessage(
                role="assistant", 
                content=f"Assistant answer {i}",
                token_count=100
            ))
        
        # 4000 tokens total, threshold at 60% of 3000 = 1800 tokens
        result = pruner.prune_messages(messages, 3000)
        
        # Verify that we still have coherent conversation structure
        assert result.messages_removed > 0
        assert len(result.messages) < len(messages)
        
        # Check that remaining messages maintain some structure
        remaining_roles = [msg.role for msg in result.messages]
        assert "user" in remaining_roles
        assert "assistant" in remaining_roles

    def test_token_counting_accuracy(self, default_config):
        """Test that token counting is accurate throughout pruning."""
        pruner = ChatLogPruner(default_config)
        
        messages = []
        expected_total = 0
        
        for i in range(20):
            token_count = 50 + (i * 10)  # Varying token counts
            messages.append(MemoryMessage(
                role="user",
                content=f"Message {i}",
                token_count=token_count
            ))
            expected_total += token_count
        
        # Verify initial calculation
        assert pruner.should_prune(messages, 2000) == (expected_total > 1200)  # 60% of 2000
        
        result = pruner.prune_messages(messages, 2000)
        
        # Verify token calculations
        assert result.tokens_before == expected_total
        
        # Manually verify tokens_after
        actual_tokens_after = sum(msg.token_count or 0 for msg in result.messages)
        assert result.tokens_after == actual_tokens_after

    def test_dry_run_comprehensive(self, default_config):
        """Test comprehensive dry run functionality."""
        config = PrunerConfig(
            token_threshold_percent=0.5,
            recent_message_count=10,
            enable_dry_run=True
        )
        pruner = ChatLogPruner(config)
        
        original_messages = []
        for i in range(20):
            original_messages.append(MemoryMessage(
                role="user",
                content=f"Message {i}",
                token_count=100
            ))
        
        # Store original state
        original_count = len(original_messages)
        original_content = [msg.content for msg in original_messages]
        
        result = pruner.prune_messages(original_messages, 1500)
        
        # Verify dry run behavior
        assert result.was_dry_run
        assert len(result.messages) == original_count  # Original messages returned
        assert [msg.content for msg in result.messages] == original_content
        
        # But statistics should show what would have been removed
        assert result.messages_removed > 0
        assert result.tokens_after < result.tokens_before

    def test_performance_large_conversation(self, large_conversation):
        """Test performance with large conversation (<100ms requirement)."""
        config = PrunerConfig(
            token_threshold_percent=0.6,
            recent_message_count=20
        )
        pruner = ChatLogPruner(config)
        
        # Measure performance
        start_time = time.time()
        result = pruner.prune_messages(large_conversation, 5000)
        end_time = time.time()
        
        duration_ms = (end_time - start_time) * 1000
        
        # Verify performance requirement
        assert duration_ms < 100, f"Pruning took {duration_ms:.2f}ms, should be <100ms"
        
        # Verify functionality still works
        assert isinstance(result, PruningResult)
        assert result.tokens_before > 0
        assert result.tokens_after <= result.tokens_before

    def test_edge_case_zero_token_messages(self, default_config):
        """Test handling of messages with zero tokens."""
        pruner = ChatLogPruner(default_config)
        
        messages = [
            MemoryMessage(role="user", content="", token_count=0),
            MemoryMessage(role="assistant", content="", token_count=0),
            MemoryMessage(role="user", content="Real message", token_count=100),
        ]
        
        result = pruner.prune_messages(messages, 1000)
        
        # Should handle zero-token messages gracefully
        assert len(result.messages) <= len(messages)
        assert result.tokens_before == 100
        assert result.tokens_after <= 100

    def test_edge_case_very_large_messages(self, default_config):
        """Test handling of messages that exceed context limit individually."""
        pruner = ChatLogPruner(default_config)
        
        messages = [
            MemoryMessage(role="system", content="System", token_count=100),
            MemoryMessage(role="user", content="Huge message", token_count=2000),  # Exceeds limit alone
            MemoryMessage(role="user", content="Normal message", token_count=100),
        ]
        
        result = pruner.prune_messages(messages, 1000)
        
        # Should handle gracefully - system message should be preserved
        system_preserved = any(msg.role == "system" for msg in result.messages)
        assert system_preserved

    def test_correction_keyword_variations(self):
        """Test various correction keyword scenarios."""
        config = PrunerConfig(
            token_threshold_percent=0.5,
            recent_message_count=10,
            correction_keywords=["actually", "wrong", "fix", "oops", "mistake"]
        )
        pruner = ChatLogPruner(config)
        
        messages = []
        
        # Add messages with various correction patterns
        correction_messages = [
            "Actually, let me clarify that",
            "That's wrong, here's the correct info",
            "Please fix this error",
            "Oops, I made a mistake",
            "There's a mistake in my previous message",
            "Actually speaking, this is correct",  # Should match "actually"
            "I was wrong about this",
            "This needs to be fixed immediately"
        ]
        
        for i, content in enumerate(correction_messages):
            messages.append(MemoryMessage(
                role="user",
                content=content,
                token_count=100
            ))
        
        # Add filler messages to trigger pruning
        for i in range(15):
            messages.append(MemoryMessage(
                role="user",
                content=f"Filler message {i}",
                token_count=100
            ))
        
        result = pruner.prune_messages(messages, 1500)
        
        # Verify correction messages are preserved
        remaining_content = [msg.content for msg in result.messages]
        
        # At least some correction messages should be preserved
        correction_preserved = sum(
            1 for content in remaining_content 
            if any(keyword in content.lower() for keyword in config.correction_keywords)
        )
        assert correction_preserved > 0
        assert result.preserved_corrections > 0

    def test_statistical_accuracy(self, default_config):
        """Test that preservation statistics are accurate."""
        pruner = ChatLogPruner(default_config)
        
        messages = [
            # 2 system messages
            MemoryMessage(role="system", content="System 1", token_count=100),
            MemoryMessage(role="system", content="System 2", token_count=100),
            
            # 3 correction messages
            MemoryMessage(role="user", content="Actually, this is wrong", token_count=100),
            MemoryMessage(role="user", content="Please fix this", token_count=100),
            MemoryMessage(role="user", content="That's a mistake", token_count=100),
            
            # Filler messages
        ]
        
        for i in range(20):
            messages.append(MemoryMessage(
                role="user",
                content=f"Filler {i}",
                token_count=100
            ))
        
        result = pruner.prune_messages(messages, 1500)
        
        # Manually count preserved types
        remaining_messages = result.messages
        actual_system = sum(1 for msg in remaining_messages if msg.role == "system")
        actual_corrections = sum(
            1 for msg in remaining_messages 
            if any(keyword in msg.content.lower() for keyword in default_config.correction_keywords)
        )
        
        # Verify statistics match reality
        assert result.preserved_system == actual_system
        assert result.preserved_corrections == actual_corrections
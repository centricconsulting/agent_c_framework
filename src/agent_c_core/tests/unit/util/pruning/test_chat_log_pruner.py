"""
Tests for the core chat log pruner.
"""

import pytest
from agent_c.util.pruning.chat_log_pruner import ChatLogPruner, PruningResult
from agent_c.util.pruning.config import PrunerConfig
from agent_c.models.chat_history.memory_message import MemoryMessage


class TestChatLogPruner:
    """Test suite for ChatLogPruner class."""

    def test_init(self):
        """Test pruner initialization."""
        config = PrunerConfig()
        pruner = ChatLogPruner(config)
        assert pruner.config == config

    def test_should_prune_empty_messages(self):
        """Test should_prune with empty message list."""
        config = PrunerConfig()
        pruner = ChatLogPruner(config)
        
        assert not pruner.should_prune([], 1000)

    def test_should_prune_under_threshold(self):
        """Test should_prune when under token threshold."""
        config = PrunerConfig(token_threshold_percent=0.75)
        pruner = ChatLogPruner(config)
        
        messages = [
            MemoryMessage(role="user", content="Hello", token_count=5),
            MemoryMessage(role="assistant", content="Hi", token_count=3)
        ]
        
        # 8 tokens total, threshold is 750 tokens (75% of 1000)
        assert not pruner.should_prune(messages, 1000)

    def test_should_prune_over_threshold(self):
        """Test should_prune when over token threshold."""
        config = PrunerConfig(token_threshold_percent=0.75)
        pruner = ChatLogPruner(config)
        
        messages = [
            MemoryMessage(role="user", content="Long message", token_count=400),
            MemoryMessage(role="assistant", content="Another long message", token_count=400)
        ]
        
        # 800 tokens total, threshold is 750 tokens (75% of 1000)
        assert pruner.should_prune(messages, 1000)

    def test_prune_messages_empty_list(self):
        """Test pruning empty message list."""
        config = PrunerConfig()
        pruner = ChatLogPruner(config)
        
        result = pruner.prune_messages([], 1000)
        
        assert isinstance(result, PruningResult)
        assert result.messages == []
        assert result.tokens_before == 0
        assert result.tokens_after == 0
        assert result.messages_removed == 0

    def test_prune_messages_no_pruning_needed(self):
        """Test when no pruning is needed."""
        config = PrunerConfig(token_threshold_percent=0.75)
        pruner = ChatLogPruner(config)
        
        messages = [
            MemoryMessage(role="user", content="Hello", token_count=5),
            MemoryMessage(role="assistant", content="Hi", token_count=3)
        ]
        
        result = pruner.prune_messages(messages, 1000)
        
        assert len(result.messages) == 2
        assert result.tokens_before == 8
        assert result.tokens_after == 8
        assert result.messages_removed == 0

    def test_prune_messages_basic_removal(self):
        """Test basic message removal."""
        config = PrunerConfig(
            token_threshold_percent=0.5,  # 50% threshold
            recent_message_count=10  # Keep only 10 recent messages
        )
        pruner = ChatLogPruner(config)
        
        # Create more messages to test removal with recent_message_count=10
        messages = []
        for i in range(15):
            messages.append(MemoryMessage(role="user", content=f"Message {i}", token_count=100))
        
        # 1500 tokens total, threshold is 1000 tokens (50% of 2000)
        result = pruner.prune_messages(messages, 2000)
        
        # Should remove some messages but keep the recent 10
        assert len(result.messages) < len(messages)
        assert result.tokens_before == 1500
        assert result.tokens_after < 1500
        assert result.messages_removed > 0
        
        # The last 10 messages should be preserved
        assert len(result.messages) >= 10

    def test_preserve_system_messages(self):
        """Test that system messages are always preserved."""
        config = PrunerConfig(
            token_threshold_percent=0.5,  # Low threshold
            recent_message_count=10
        )
        pruner = ChatLogPruner(config)
        
        # Create enough messages to trigger pruning
        messages = [
            MemoryMessage(role="system", content="You are a helpful assistant", token_count=100)
        ]
        # Add many messages to exceed threshold
        for i in range(15):
            messages.append(MemoryMessage(role="user", content=f"Message {i}", token_count=100))
        
        # 1600 tokens total, threshold is 1000 tokens (50% of 2000)
        result = pruner.prune_messages(messages, 2000)
        
        # System message should be preserved
        system_messages = [msg for msg in result.messages if msg.role == "system"]
        assert len(system_messages) == 1
        assert system_messages[0].content == "You are a helpful assistant"
        assert result.preserved_system == 1

    def test_preserve_correction_messages(self):
        """Test that correction messages are preserved."""
        config = PrunerConfig(
            token_threshold_percent=0.5,  # Low threshold
            recent_message_count=10,
            correction_keywords=["actually", "wrong", "fix"]
        )
        pruner = ChatLogPruner(config)
        
        # Create enough messages to trigger pruning
        messages = [
            MemoryMessage(role="user", content="Actually, I meant something else", token_count=100),
        ]
        # Add many messages to exceed threshold
        for i in range(15):
            messages.append(MemoryMessage(role="user", content=f"Message {i}", token_count=100))
        
        # 1600 tokens total, threshold is 1000 tokens (50% of 2000)
        result = pruner.prune_messages(messages, 2000)
        
        # Correction message should be preserved
        correction_found = any(
            "Actually" in msg.content for msg in result.messages
        )
        assert correction_found
        assert result.preserved_corrections >= 1

    def test_dry_run_mode(self):
        """Test dry run mode doesn't modify original messages."""
        config = PrunerConfig(
            token_threshold_percent=0.5,
            recent_message_count=10,
            enable_dry_run=True
        )
        pruner = ChatLogPruner(config)
        
        # Create enough messages to trigger pruning
        messages = []
        for i in range(15):
            messages.append(MemoryMessage(role="user", content=f"Message {i}", token_count=100))
        
        original_count = len(messages)
        # 1500 tokens total, threshold is 1000 tokens (50% of 2000)
        result = pruner.prune_messages(messages, 2000)
        
        # In dry run mode, original messages should be returned
        assert len(result.messages) == original_count
        assert result.was_dry_run
        # But statistics should show what would have been removed
        assert result.messages_removed > 0

    def test_no_removal_candidates(self):
        """Test when all messages are preserved."""
        config = PrunerConfig(
            token_threshold_percent=0.5,  # Low threshold
            recent_message_count=50  # Preserve many recent messages
        )
        pruner = ChatLogPruner(config)
        
        messages = [
            MemoryMessage(role="system", content="System message", token_count=100),
            MemoryMessage(role="user", content="Recent 1", token_count=100),
            MemoryMessage(role="user", content="Recent 2", token_count=100)
        ]
        
        result = pruner.prune_messages(messages, 2000)
        
        # All messages should be preserved
        assert len(result.messages) == len(messages)
        assert result.messages_removed == 0

    def test_fifo_removal_order(self):
        """Test that messages are removed in FIFO order."""
        config = PrunerConfig(
            token_threshold_percent=0.5,
            recent_message_count=10  # Keep last 10 messages
        )
        pruner = ChatLogPruner(config)
        
        # Create enough messages to test FIFO removal
        messages = []
        for i in range(15):
            messages.append(MemoryMessage(role="user", content=f"Message {i}", token_count=100))
        
        # 1500 tokens total, threshold is 1000 tokens (50% of 2000)
        result = pruner.prune_messages(messages, 2000)
        
        # Should keep the recent 10 messages (last 10)
        remaining_content = [msg.content for msg in result.messages]
        
        # The most recent messages should be preserved
        assert "Message 14" in remaining_content  # Last message
        assert "Message 13" in remaining_content  # Second to last
        
        # Oldest messages should be removed first
        if len(result.messages) < len(messages):
            assert "Message 0" not in remaining_content  # First message should be removed

    def test_effective_token_limit_calculation(self):
        """Test that effective token limit is calculated correctly."""
        config = PrunerConfig(
            token_threshold_percent=0.8,
            max_context_tokens=1200  # Override
        )
        pruner = ChatLogPruner(config)
        
        # Should use the override value, not the model limit
        messages = [
            MemoryMessage(role="user", content="Test", token_count=1000)
        ]
        
        # Even though model limit is 2000, should use override of 1200
        # Effective limit = 1200 * 0.8 = 960
        assert pruner.should_prune(messages, 2000)  # 1000 > 960

    def test_preservation_statistics(self):
        """Test that preservation statistics are calculated correctly."""
        config = PrunerConfig(
            token_threshold_percent=0.5,
            recent_message_count=10,
            correction_keywords=["fix"]
        )
        pruner = ChatLogPruner(config)
        
        messages = [
            MemoryMessage(role="system", content="System", token_count=100),
            MemoryMessage(role="user", content="Please fix this", token_count=100),
        ]
        # Add enough messages to trigger pruning
        for i in range(15):
            messages.append(MemoryMessage(role="user", content=f"Message {i}", token_count=100))
        
        # 1700 tokens total, threshold is 1000 tokens (50% of 2000)
        result = pruner.prune_messages(messages, 2000)
        
        assert result.preserved_system >= 1
        assert result.preserved_corrections >= 1
        assert result.preserved_recent >= 1
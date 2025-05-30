"""
Edge case and error handling tests for chat log pruning.

This module tests unusual scenarios, error conditions, and boundary cases
to ensure robust behavior.
"""

import pytest
from agent_c.util.pruning.chat_log_pruner import ChatLogPruner
from agent_c.util.pruning.config import PrunerConfig
from agent_c.models.chat_history.memory_message import MemoryMessage


class TestEdgeCases:
    """Test suite for edge cases and error handling."""

    def test_none_token_counts(self):
        """Test handling of messages with None token counts."""
        config = PrunerConfig()
        pruner = ChatLogPruner(config)
        
        messages = [
            MemoryMessage(role="user", content="Message with None tokens", token_count=None),
            MemoryMessage(role="assistant", content="Another message", token_count=100),
        ]
        
        # Should handle gracefully without crashing
        result = pruner.prune_messages(messages, 1000)
        assert len(result.messages) <= len(messages)

    def test_negative_token_counts(self):
        """Test handling of messages with negative token counts."""
        config = PrunerConfig()
        pruner = ChatLogPruner(config)
        
        messages = [
            MemoryMessage(role="user", content="Negative tokens", token_count=-10),
            MemoryMessage(role="assistant", content="Normal message", token_count=100),
        ]
        
        # Should handle gracefully
        result = pruner.prune_messages(messages, 1000)
        assert len(result.messages) <= len(messages)

    def test_zero_context_limit(self):
        """Test behavior with zero context limit."""
        config = PrunerConfig()
        pruner = ChatLogPruner(config)
        
        messages = [
            MemoryMessage(role="user", content="Test", token_count=50),
        ]
        
        # Should handle gracefully
        result = pruner.prune_messages(messages, 0)
        assert isinstance(result.tokens_before, int)
        assert isinstance(result.tokens_after, int)

    def test_negative_context_limit(self):
        """Test behavior with negative context limit."""
        config = PrunerConfig()
        pruner = ChatLogPruner(config)
        
        messages = [
            MemoryMessage(role="user", content="Test", token_count=50),
        ]
        
        # Should handle gracefully
        result = pruner.prune_messages(messages, -100)
        assert isinstance(result.tokens_before, int)
        assert isinstance(result.tokens_after, int)

    def test_extremely_large_context_limit(self):
        """Test behavior with extremely large context limit."""
        config = PrunerConfig()
        pruner = ChatLogPruner(config)
        
        messages = [
            MemoryMessage(role="user", content="Test", token_count=50),
        ]
        
        # Should handle gracefully
        result = pruner.prune_messages(messages, 999999999)
        assert len(result.messages) == len(messages)  # No pruning needed

    def test_empty_message_content(self):
        """Test handling of messages with empty content."""
        config = PrunerConfig()
        pruner = ChatLogPruner(config)
        
        messages = [
            MemoryMessage(role="user", content="", token_count=0),
            MemoryMessage(role="assistant", content="", token_count=0),
            MemoryMessage(role="user", content="Real content", token_count=50),
        ]
        
        result = pruner.prune_messages(messages, 1000)
        assert len(result.messages) <= len(messages)

    def test_whitespace_only_content(self):
        """Test handling of messages with only whitespace."""
        config = PrunerConfig()
        pruner = ChatLogPruner(config)
        
        messages = [
            MemoryMessage(role="user", content="   \n\t  ", token_count=5),
            MemoryMessage(role="assistant", content="Real content", token_count=50),
        ]
        
        result = pruner.prune_messages(messages, 1000)
        assert len(result.messages) <= len(messages)

    def test_very_long_content(self):
        """Test handling of messages with extremely long content."""
        config = PrunerConfig()
        pruner = ChatLogPruner(config)
        
        # Create a very long message
        long_content = "This is a very long message. " * 1000  # ~30,000 characters
        messages = [
            MemoryMessage(role="user", content=long_content, token_count=5000),
            MemoryMessage(role="assistant", content="Short response", token_count=10),
        ]
        
        result = pruner.prune_messages(messages, 1000)
        assert len(result.messages) <= len(messages)

    def test_unicode_content(self):
        """Test handling of messages with Unicode content."""
        config = PrunerConfig(correction_keywords=["修正", "错误", "actually"])
        pruner = ChatLogPruner(config)
        
        messages = [
            MemoryMessage(role="user", content="Hello 世界! 🌍", token_count=50),
            MemoryMessage(role="assistant", content="Bonjour! 🇫🇷", token_count=30),
            MemoryMessage(role="user", content="Actually, 修正 this", token_count=40),
        ]
        
        result = pruner.prune_messages(messages, 1000)
        assert len(result.messages) <= len(messages)
        
        # Unicode correction keyword should work
        remaining_content = [msg.content for msg in result.messages]
        assert any("修正" in content for content in remaining_content)

    def test_special_characters_in_keywords(self):
        """Test correction keywords with special characters."""
        config = PrunerConfig(correction_keywords=["fix-it", "re:do", "oops!", "actually..."])
        pruner = ChatLogPruner(config)
        
        messages = [
            MemoryMessage(role="user", content="Please fix-it now", token_count=50),
            MemoryMessage(role="user", content="re:do this task", token_count=50),
            MemoryMessage(role="user", content="oops! my mistake", token_count=50),
            MemoryMessage(role="user", content="actually... let me think", token_count=50),
        ]
        
        # Add filler to trigger pruning
        for i in range(20):
            messages.append(MemoryMessage(role="user", content=f"Filler {i}", token_count=100))
        
        result = pruner.prune_messages(messages, 1000)
        
        # Special character keywords should work
        assert result.preserved_corrections > 0

    def test_case_insensitive_keywords(self):
        """Test that correction keywords are case insensitive."""
        config = PrunerConfig(correction_keywords=["ACTUALLY", "Wrong", "FIX"])
        pruner = ChatLogPruner(config)
        
        messages = [
            MemoryMessage(role="user", content="actually, this is wrong", token_count=50),
            MemoryMessage(role="user", content="WRONG answer", token_count=50),
            MemoryMessage(role="user", content="please fix this", token_count=50),
        ]
        
        # Add filler to trigger pruning
        for i in range(20):
            messages.append(MemoryMessage(role="user", content=f"Filler {i}", token_count=100))
        
        result = pruner.prune_messages(messages, 1000)
        
        # Case insensitive matching should work
        assert result.preserved_corrections > 0

    def test_overlapping_keywords(self):
        """Test handling of overlapping correction keywords."""
        config = PrunerConfig(correction_keywords=["fix", "fixing", "fixed"])
        pruner = ChatLogPruner(config)
        
        messages = [
            MemoryMessage(role="user", content="This needs fixing", token_count=50),
            MemoryMessage(role="user", content="I fixed it", token_count=50),
            MemoryMessage(role="user", content="Please fix this", token_count=50),
        ]
        
        # Add filler to trigger pruning
        for i in range(20):
            messages.append(MemoryMessage(role="user", content=f"Filler {i}", token_count=100))
        
        result = pruner.prune_messages(messages, 1000)
        
        # All variations should be detected
        assert result.preserved_corrections > 0

    def test_keyword_at_boundaries(self):
        """Test correction keywords at word boundaries."""
        config = PrunerConfig(correction_keywords=["fix"])
        pruner = ChatLogPruner(config)
        
        messages = [
            MemoryMessage(role="user", content="fix this", token_count=50),  # Should match
            MemoryMessage(role="user", content="prefix this", token_count=50),  # Should NOT match
            MemoryMessage(role="user", content="this is fixed", token_count=50),  # Should NOT match
            MemoryMessage(role="user", content="fix-it now", token_count=50),  # Should match (word boundary)
        ]
        
        # Add filler to trigger pruning
        for i in range(20):
            messages.append(MemoryMessage(role="user", content=f"Filler {i}", token_count=100))
        
        result = pruner.prune_messages(messages, 1000)
        
        remaining_content = [msg.content for msg in result.messages]
        
        # Only true word matches should be preserved
        assert "fix this" in remaining_content
        assert "fix-it now" in remaining_content
        # These should potentially be removed (not preserved as corrections)
        # Note: They might still be preserved as recent messages

    def test_malformed_message_objects(self):
        """Test handling of malformed message objects."""
        config = PrunerConfig()
        pruner = ChatLogPruner(config)
        
        # Create messages with unusual but valid combinations
        messages = [
            MemoryMessage(role="", content="Empty role", token_count=50),
            MemoryMessage(role="unknown_role", content="Unknown role", token_count=50),
            MemoryMessage(role="user", content="Normal message", token_count=50),
        ]
        
        result = pruner.prune_messages(messages, 1000)
        assert len(result.messages) <= len(messages)

    def test_extreme_recent_message_count(self):
        """Test with recent_message_count larger than total messages."""
        config = PrunerConfig(recent_message_count=50)  # Max allowed
        pruner = ChatLogPruner(config)
        
        # Only create 10 messages
        messages = []
        for i in range(10):
            messages.append(MemoryMessage(
                role="user",
                content=f"Message {i}",
                token_count=100
            ))
        
        result = pruner.prune_messages(messages, 500)
        
        # All messages should be preserved (recent_message_count > total messages)
        assert len(result.messages) == len(messages)

    def test_threshold_edge_cases(self):
        """Test behavior at threshold boundaries."""
        config = PrunerConfig(token_threshold_percent=0.5)  # 50%
        pruner = ChatLogPruner(config)
        
        # Test exactly at threshold
        messages = [
            MemoryMessage(role="user", content="Test", token_count=500),  # Exactly 50% of 1000
        ]
        
        # At threshold, should not prune
        assert not pruner.should_prune(messages, 1000)
        
        # Just over threshold, should prune
        messages[0].token_count = 501
        assert pruner.should_prune(messages, 1000)

    def test_concurrent_access_simulation(self):
        """Test behavior with rapid successive calls (simulating concurrent access)."""
        config = PrunerConfig()
        pruner = ChatLogPruner(config)
        
        messages = []
        for i in range(20):
            messages.append(MemoryMessage(
                role="user",
                content=f"Message {i}",
                token_count=100
            ))
        
        # Perform multiple rapid calls
        results = []
        for _ in range(10):
            result = pruner.prune_messages(messages, 1000)
            results.append(result)
        
        # All results should be consistent
        for i in range(1, len(results)):
            assert len(results[i].messages) == len(results[0].messages)
            assert results[i].tokens_after == results[0].tokens_after
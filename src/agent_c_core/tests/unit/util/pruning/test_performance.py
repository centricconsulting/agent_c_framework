"""
Performance tests for chat log pruning.

This module tests the performance requirements and scalability
of the pruning system.
"""

import pytest
import time
import statistics
from typing import List
from agent_c.util.pruning.chat_log_pruner import ChatLogPruner
from agent_c.util.pruning.config import PrunerConfig
from agent_c.models.chat_history.memory_message import MemoryMessage


class TestPerformance:
    """Performance test suite for chat log pruning."""

    @pytest.fixture
    def performance_config(self):
        """Configuration optimized for performance testing."""
        return PrunerConfig(
            token_threshold_percent=0.6,
            recent_message_count=20,
            correction_keywords=["actually", "wrong", "fix"]
        )

    def create_conversation(self, message_count: int, avg_tokens: int = 75) -> List[MemoryMessage]:
        """Create a conversation with specified number of messages."""
        messages = []
        
        # Add system message
        messages.append(MemoryMessage(
            role="system",
            content="You are a helpful assistant",
            token_count=50
        ))
        
        # Add conversation messages
        for i in range(message_count - 1):
            role = "user" if i % 2 == 0 else "assistant"
            content = f"This is {role} message {i} with some content to make it realistic"
            token_count = avg_tokens + (i % 20) - 10  # Slight variation
            
            messages.append(MemoryMessage(
                role=role,
                content=content,
                token_count=token_count
            ))
        
        return messages

    def test_performance_100_messages(self, performance_config):
        """Test performance with 100 messages (target: <100ms)."""
        pruner = ChatLogPruner(performance_config)
        messages = self.create_conversation(100)
        
        start_time = time.time()
        result = pruner.prune_messages(messages, 5000)
        end_time = time.time()
        
        duration_ms = (end_time - start_time) * 1000
        
        assert duration_ms < 100, f"100 messages took {duration_ms:.2f}ms, should be <100ms"
        assert len(result.messages) <= len(messages)

    def test_performance_500_messages(self, performance_config):
        """Test performance with 500 messages."""
        pruner = ChatLogPruner(performance_config)
        messages = self.create_conversation(500)
        
        start_time = time.time()
        result = pruner.prune_messages(messages, 10000)
        end_time = time.time()
        
        duration_ms = (end_time - start_time) * 1000
        
        # Should still be reasonable for larger conversations
        assert duration_ms < 500, f"500 messages took {duration_ms:.2f}ms, should be <500ms"
        assert len(result.messages) <= len(messages)

    def test_performance_1000_messages(self, performance_config):
        """Test performance with 1000 messages."""
        pruner = ChatLogPruner(performance_config)
        messages = self.create_conversation(1000)
        
        start_time = time.time()
        result = pruner.prune_messages(messages, 15000)
        end_time = time.time()
        
        duration_ms = (end_time - start_time) * 1000
        
        # Should scale reasonably
        assert duration_ms < 1000, f"1000 messages took {duration_ms:.2f}ms, should be <1000ms"
        assert len(result.messages) <= len(messages)

    def test_performance_consistency(self, performance_config):
        """Test that performance is consistent across multiple runs."""
        pruner = ChatLogPruner(performance_config)
        messages = self.create_conversation(150)
        
        durations = []
        
        # Run multiple times to check consistency
        for _ in range(10):
            start_time = time.time()
            result = pruner.prune_messages(messages, 5000)
            end_time = time.time()
            
            duration_ms = (end_time - start_time) * 1000
            durations.append(duration_ms)
            
            # Verify correctness each time
            assert len(result.messages) <= len(messages)
        
        # Check statistical consistency
        avg_duration = statistics.mean(durations)
        std_duration = statistics.stdev(durations)
        
        assert avg_duration < 100, f"Average duration {avg_duration:.2f}ms should be <100ms"
        assert std_duration < 20, f"Standard deviation {std_duration:.2f}ms should be <20ms (consistent)"

    def test_performance_should_prune_check(self, performance_config):
        """Test performance of should_prune check."""
        pruner = ChatLogPruner(performance_config)
        messages = self.create_conversation(1000)
        
        # should_prune should be very fast
        start_time = time.time()
        for _ in range(100):  # Run many times
            should_prune = pruner.should_prune(messages, 5000)
        end_time = time.time()
        
        total_duration_ms = (end_time - start_time) * 1000
        avg_duration_ms = total_duration_ms / 100
        
        assert avg_duration_ms < 1, f"should_prune took {avg_duration_ms:.2f}ms on average, should be <1ms"

    def test_performance_scaling(self, performance_config):
        """Test that performance scales reasonably with message count."""
        pruner = ChatLogPruner(performance_config)
        
        message_counts = [50, 100, 200, 400]
        durations = []
        
        for count in message_counts:
            messages = self.create_conversation(count)
            
            start_time = time.time()
            result = pruner.prune_messages(messages, count * 50)  # Proportional context
            end_time = time.time()
            
            duration_ms = (end_time - start_time) * 1000
            durations.append(duration_ms)
            
            assert len(result.messages) <= len(messages)
        
        # Performance should scale sub-linearly (not worse than O(n))
        # Check that doubling messages doesn't more than triple time
        for i in range(1, len(durations)):
            # Handle very fast operations (avoid division by zero)
            if durations[i-1] == 0:
                # If previous duration was 0, current should also be very fast
                assert durations[i] < 10, f"Performance degraded: {durations[i]:.2f}ms for {message_counts[i]} messages"
                continue
                
            ratio = durations[i] / durations[i-1]
            message_ratio = message_counts[i] / message_counts[i-1]
            
            assert ratio <= message_ratio * 1.5, f"Performance scaling too poor: {ratio:.2f}x time for {message_ratio:.2f}x messages"

    def test_memory_efficiency(self, performance_config):
        """Test memory efficiency during pruning."""
        import psutil
        import os
        
        pruner = ChatLogPruner(performance_config)
        messages = self.create_conversation(1000)
        
        # Get initial memory usage
        process = psutil.Process(os.getpid())
        initial_memory = process.memory_info().rss
        
        # Perform pruning
        result = pruner.prune_messages(messages, 10000)
        
        # Get final memory usage
        final_memory = process.memory_info().rss
        memory_increase = final_memory - initial_memory
        
        # Memory increase should be reasonable (less than 50MB for 1000 messages)
        assert memory_increase < 50 * 1024 * 1024, f"Memory increased by {memory_increase / 1024 / 1024:.2f}MB, should be <50MB"
        
        assert len(result.messages) <= len(messages)

    def test_performance_with_many_corrections(self, performance_config):
        """Test performance when many messages match correction keywords."""
        pruner = ChatLogPruner(performance_config)
        
        messages = []
        # Create messages where many match correction keywords
        for i in range(200):
            if i % 3 == 0:
                content = f"Actually, let me correct message {i}"
            elif i % 3 == 1:
                content = f"That's wrong, here's the fix for {i}"
            else:
                content = f"Regular message {i}"
            
            messages.append(MemoryMessage(
                role="user",
                content=content,
                token_count=75
            ))
        
        start_time = time.time()
        result = pruner.prune_messages(messages, 8000)
        end_time = time.time()
        
        duration_ms = (end_time - start_time) * 1000
        
        # Should still be fast even with many corrections
        assert duration_ms < 200, f"Many corrections took {duration_ms:.2f}ms, should be <200ms"
        assert result.preserved_corrections > 0

    def test_performance_dry_run_vs_normal(self, performance_config):
        """Test that dry run mode doesn't significantly impact performance."""
        # Use larger conversation to get more reliable timing measurements
        messages = self.create_conversation(500)
        
        # Test normal mode multiple times for average
        normal_config = PrunerConfig(
            token_threshold_percent=0.6,
            recent_message_count=20,
            enable_dry_run=False
        )
        normal_pruner = ChatLogPruner(normal_config)
        
        normal_durations = []
        for _ in range(5):  # Run multiple times
            start_time = time.time()
            normal_result = normal_pruner.prune_messages(messages, 15000)
            normal_duration = time.time() - start_time
            normal_durations.append(normal_duration)
        
        avg_normal_duration = sum(normal_durations) / len(normal_durations)
        
        # Test dry run mode multiple times for average
        dry_run_config = PrunerConfig(
            token_threshold_percent=0.6,
            recent_message_count=20,
            enable_dry_run=True
        )
        dry_run_pruner = ChatLogPruner(dry_run_config)
        
        dry_run_durations = []
        for _ in range(5):  # Run multiple times
            start_time = time.time()
            dry_run_result = dry_run_pruner.prune_messages(messages, 15000)
            dry_run_duration = time.time() - start_time
            dry_run_durations.append(dry_run_duration)
        
        avg_dry_run_duration = sum(dry_run_durations) / len(dry_run_durations)
        
        # Dry run should not be significantly slower (allow more tolerance for small operations)
        if avg_normal_duration > 0:
            duration_ratio = avg_dry_run_duration / avg_normal_duration
            assert duration_ratio < 2.5, f"Dry run {duration_ratio:.2f}x slower than normal, should be <2.5x"
        else:
            # If normal duration is effectively 0, dry run should also be very fast
            assert avg_dry_run_duration < 0.01, f"Dry run took {avg_dry_run_duration:.4f}s, should be <0.01s"
        
        # Verify dry run behavior
        assert dry_run_result.was_dry_run
        assert len(dry_run_result.messages) == len(messages)  # Original messages returned
"""
Pytest configuration and fixtures for pruning tests.

This module provides shared fixtures and configuration for all pruning tests.
"""

import pytest
from typing import List
from agent_c.util.pruning.config import PrunerConfig
from agent_c.util.pruning.chat_log_pruner import ChatLogPruner
from agent_c.models.chat_history.memory_message import MemoryMessage


@pytest.fixture
def basic_config():
    """Basic pruner configuration for testing."""
    return PrunerConfig(
        token_threshold_percent=0.6,
        recent_message_count=15,
        correction_keywords=["actually", "wrong", "fix", "correction", "mistake"]
    )


@pytest.fixture
def strict_config():
    """Strict pruner configuration for aggressive pruning."""
    return PrunerConfig(
        token_threshold_percent=0.5,
        recent_message_count=10,
        correction_keywords=["actually", "wrong", "fix"]
    )


@pytest.fixture
def lenient_config():
    """Lenient pruner configuration for minimal pruning."""
    return PrunerConfig(
        token_threshold_percent=0.9,
        recent_message_count=30,
        correction_keywords=["actually", "wrong", "fix", "correction", "mistake", "oops"]
    )


@pytest.fixture
def dry_run_config():
    """Configuration with dry run enabled."""
    return PrunerConfig(
        token_threshold_percent=0.6,
        recent_message_count=15,
        enable_dry_run=True
    )


@pytest.fixture
def basic_pruner(basic_config):
    """Basic pruner instance for testing."""
    return ChatLogPruner(basic_config)


@pytest.fixture
def simple_conversation():
    """Simple conversation for basic testing."""
    return [
        MemoryMessage(role="system", content="You are helpful", token_count=50),
        MemoryMessage(role="user", content="Hello", token_count=25),
        MemoryMessage(role="assistant", content="Hi there!", token_count=30),
        MemoryMessage(role="user", content="How are you?", token_count=35),
        MemoryMessage(role="assistant", content="I'm doing well, thank you!", token_count=40),
    ]


@pytest.fixture
def conversation_with_corrections():
    """Conversation containing correction messages."""
    return [
        MemoryMessage(role="system", content="You are helpful", token_count=50),
        MemoryMessage(role="user", content="What's 2+2?", token_count=25),
        MemoryMessage(role="assistant", content="It's 5", token_count=20),
        MemoryMessage(role="user", content="Actually, that's wrong. It's 4.", token_count=35),
        MemoryMessage(role="assistant", content="You're right, my mistake!", token_count=30),
        MemoryMessage(role="user", content="Please fix your math", token_count=25),
        MemoryMessage(role="assistant", content="I'll be more careful", token_count=30),
    ]


@pytest.fixture
def large_conversation():
    """Large conversation for performance testing."""
    messages = [
        MemoryMessage(role="system", content="You are helpful", token_count=50)
    ]
    
    for i in range(100):
        messages.append(MemoryMessage(
            role="user",
            content=f"User message {i}",
            token_count=60
        ))
        messages.append(MemoryMessage(
            role="assistant", 
            content=f"Assistant response {i}",
            token_count=70
        ))
    
    return messages


@pytest.fixture
def mixed_conversation():
    """Conversation with mixed message types and token counts."""
    messages = []
    
    # System message
    messages.append(MemoryMessage(
        role="system",
        content="You are a helpful AI assistant",
        token_count=60
    ))
    
    # Regular conversation
    for i in range(10):
        messages.append(MemoryMessage(
            role="user",
            content=f"User question {i}",
            token_count=40 + (i * 5)
        ))
        messages.append(MemoryMessage(
            role="assistant",
            content=f"Assistant answer {i}",
            token_count=50 + (i * 3)
        ))
    
    # Add some corrections
    messages.append(MemoryMessage(
        role="user",
        content="Actually, I need to correct my earlier question",
        token_count=80
    ))
    
    messages.append(MemoryMessage(
        role="user",
        content="That answer was wrong, please fix it",
        token_count=70
    ))
    
    return messages


def create_conversation_with_tokens(message_count: int, tokens_per_message: int) -> List[MemoryMessage]:
    """Helper function to create conversations with specific token counts."""
    messages = []
    
    for i in range(message_count):
        role = "user" if i % 2 == 0 else "assistant"
        messages.append(MemoryMessage(
            role=role,
            content=f"Message {i}",
            token_count=tokens_per_message
        ))
    
    return messages


# Performance test markers
pytest.mark.performance = pytest.mark.mark("performance")
pytest.mark.slow = pytest.mark.mark("slow")


# Test categories
def pytest_configure(config):
    """Configure pytest markers."""
    config.addinivalue_line(
        "markers", "performance: marks tests as performance tests"
    )
    config.addinivalue_line(
        "markers", "slow: marks tests as slow running tests"
    )
    config.addinivalue_line(
        "markers", "edge_case: marks tests as edge case tests"
    )
    config.addinivalue_line(
        "markers", "integration: marks tests as integration tests"
    )
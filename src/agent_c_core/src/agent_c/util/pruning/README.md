# Chat Log Pruner

A configurable chat log pruner for the Agent C framework that intelligently removes messages while preserving conversation coherence, system context, and tool call integrity.

## Overview

The Chat Log Pruner automatically manages conversation length by removing older messages when token limits are approached, while preserving essential conversation elements. It integrates seamlessly with Agent C's session management and agent systems.

## Key Features

- **Intelligent Preservation**: Automatically preserves system messages, recent exchanges, user corrections, and tool call pairs
- **Configurable Thresholds**: Customizable token limits and preservation rules
- **High Performance**: Sub-100ms pruning operations even with large conversations
- **Seamless Integration**: Works automatically with existing Agent C agents and session managers
- **Safe Operation**: Maintains conversation coherence and tool call atomicity
- **Comprehensive Testing**: Extensive test coverage for reliability

## Quick Start

### Basic Setup

```python
from agent_c.util.pruning.config import PrunerConfig
from agent_c.util.pruning.chat_log_pruner import ChatLogPruner

# Create configuration
config = PrunerConfig(
    recent_message_count=20,
    token_threshold_percent=0.75,
    correction_keywords=["actually", "correction", "wrong", "mistake"]
)

# Create pruner
pruner = ChatLogPruner(config)

# Check if pruning is needed
if pruner.should_prune(messages, context_limit=4000):
    pruned_messages = pruner.prune_messages(messages, context_limit=4000)
```

### Agent Configuration

Enable automatic pruning in your agent configuration:

```yaml
# agent_config.yaml
pruning:
  enable_auto_pruning: true
  recent_message_count: 25
  token_threshold_percent: 0.8
  correction_keywords: ["actually", "correction", "wrong", "mistake", "fix"]
```

```python
# Or programmatically
from agent_c.models.agent_config import AgentConfig
from agent_c.util.pruning.config import PrunerConfig

agent_config = AgentConfig(
    enable_auto_pruning=True,
    pruner_config=PrunerConfig(
        recent_message_count=25,
        token_threshold_percent=0.8
    )
)
```

## Configuration Options

### PrunerConfig Parameters

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `recent_message_count` | int | 20 | Number of recent messages to always preserve (10-50) |
| `token_threshold_percent` | float | 0.75 | Trigger pruning when tokens exceed this % of context limit (0.5-0.95) |
| `correction_keywords` | List[str] | ["actually", "correction", "wrong", "mistake"] | Keywords that identify user corrections |
| `enable_dry_run` | bool | False | Return what would be removed without modifying |
| `max_context_tokens` | Optional[int] | None | Override context limit for pruning decisions |

### Agent Configuration

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `enable_auto_pruning` | bool | False | Enable automatic pruning before API calls |
| `pruner_config` | Optional[PrunerConfig] | None | Pruner configuration settings |

## How It Works

### Preservation Rules

The pruner uses intelligent preservation rules to maintain conversation quality:

1. **System Messages**: Always preserved to maintain agent context
2. **Recent Messages**: Last N user-assistant pairs preserved for conversation flow
3. **User Corrections**: Messages containing correction keywords are preserved
4. **Tool Call Pairs**: Tool calls and their results are kept together or removed together

### Pruning Algorithm

1. **Threshold Check**: Calculate total tokens and compare to threshold
2. **Identify Preserved**: Mark messages that must be kept based on preservation rules
3. **Create Candidates**: Identify older messages eligible for removal
4. **FIFO Removal**: Remove oldest candidates first until under threshold
5. **Boundary Respect**: Ensure complete user-assistant message pairs

### Performance

- **Target**: <100ms for typical conversations
- **Tested**: Up to 1000+ messages with consistent sub-100ms performance
- **Memory Efficient**: Minimal memory overhead during operation

## Usage Examples

### Manual Pruning

```python
from agent_c.util.pruning.chat_log_pruner import ChatLogPruner
from agent_c.util.pruning.config import PrunerConfig

# Setup
config = PrunerConfig(recent_message_count=15, token_threshold_percent=0.8)
pruner = ChatLogPruner(config)

# Get messages from your session manager
messages = session_manager.get_messages(session_id)

# Check if pruning needed
context_limit = 4000
if pruner.should_prune(messages, context_limit):
    print(f"Pruning needed: {len(messages)} messages")
    
    # Perform pruning
    pruned_messages = pruner.prune_messages(messages, context_limit)
    print(f"After pruning: {len(pruned_messages)} messages")
    
    # Update session
    session_manager.set_messages(session_id, pruned_messages)
```

### Dry Run Mode

```python
# See what would be removed without modifying
config = PrunerConfig(enable_dry_run=True)
pruner = ChatLogPruner(config)

result = pruner.prune_messages(messages, context_limit=4000)
# result contains original messages, but pruner logs what would be removed
```

### Session Manager Integration

```python
# Session managers automatically handle pruning when configured
session_manager = SomeSessionManager()

# Pruning happens automatically when context limit approached
success = session_manager.prune_session_if_needed(session_id, context_limit=4000)
if success:
    print("Session pruned successfully")
```

### Agent Integration

```python
# Agents automatically prune before API calls when auto_pruning enabled
agent = SomeAgent(config=agent_config)  # config has enable_auto_pruning=True

# Pruning happens automatically in _construct_message_array()
response = agent.chat(session_id, "Hello", auto_pruning=True)
```

## Architecture Integration

### Session Manager Integration

The pruner integrates with session managers through:

- `get_pruner()` method returns configured ChatLogPruner instance
- `prune_session_if_needed()` method triggers pruning when needed
- Automatic pruning before message retrieval for API calls

### Agent Integration

Agents integrate pruning through:

- Configuration loading from agent config files
- Automatic pruning in `_construct_message_array()` method
- Context limit detection from model configurations
- Graceful fallback when pruning unavailable

### Event System

The pruner will emit events for monitoring (Phase 3):

- `PruningTriggeredEvent`: When pruning check performed
- `PruningCompletedEvent`: When pruning finishes successfully
- `PruningSkippedEvent`: When pruning not needed
- `PruningFailedEvent`: When errors occur

## Testing

### Running Tests

```bash
# Unit tests
pytest tests/unit/util/pruning/ -v

# Integration tests  
pytest tests/integration/test_pruning_integration.py -v

# All pruning tests
pytest tests/ -k "pruning" -v
```

### Test Coverage

- **Unit Tests**: 85+ tests covering all core functionality
- **Integration Tests**: End-to-end scenarios with session managers and agents
- **Performance Tests**: Validation of <100ms requirement
- **Edge Cases**: Empty conversations, all-preserved messages, error scenarios

## Performance Characteristics

### Benchmarks

- **Small Conversations** (10-20 messages): <1ms
- **Medium Conversations** (50-100 messages): 1-5ms  
- **Large Conversations** (200+ messages): 5-50ms
- **Very Large Conversations** (1000+ messages): 50-100ms

### Memory Usage

- **Minimal Overhead**: No significant memory increase during operation
- **Efficient Processing**: In-place operations where possible
- **Token Calculation**: Cached for performance

## Error Handling

The pruner handles errors gracefully:

- **Configuration Errors**: Validation with helpful error messages
- **Session Manager Errors**: Graceful fallback when pruning unavailable
- **Message Format Errors**: Robust handling of unexpected message formats
- **Performance Issues**: Timeout protection for large conversations

## Best Practices

### Configuration

- Start with default settings and adjust based on your use case
- Monitor pruning frequency to tune `token_threshold_percent`
- Adjust `recent_message_count` based on conversation patterns
- Add domain-specific correction keywords

### Performance

- Enable auto-pruning to prevent context limit issues
- Monitor pruning operations for performance impact
- Use dry-run mode for testing configuration changes

### Monitoring

- Log pruning operations for analysis
- Track token savings and conversation quality
- Monitor for excessive pruning frequency

## Troubleshooting

### Common Issues

**Pruning not triggering:**
- Check `enable_auto_pruning` is true
- Verify `token_threshold_percent` setting
- Ensure context limits are configured

**Too aggressive pruning:**
- Increase `recent_message_count`
- Lower `token_threshold_percent`
- Check preservation keyword configuration

**Performance issues:**
- Monitor conversation sizes
- Check for configuration validation errors
- Verify session manager implementation

### Debug Mode

Enable debug logging to see detailed pruning decisions:

```python
import logging
logging.getLogger('agent_c.util.pruning').setLevel(logging.DEBUG)
```

## Future Enhancements

The pruner is designed for extensibility:

- **Phase 3**: Events, metrics, and observability features
- **Phase 4**: Advanced features like semantic analysis and summarization
- **Scoring Systems**: Replace FIFO with importance-based removal
- **Summarization**: Replace removal with intelligent summarization

## Contributing

When contributing to the pruner:

1. Maintain backward compatibility
2. Add comprehensive tests for new features
3. Follow existing code patterns and documentation
4. Consider performance impact of changes
5. Update this README for new features

## License

Part of the Agent C framework. See main project license.
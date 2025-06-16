# SessionLogger Migration Guide

## Overview

The SessionLogger has been refactored from an embedded pattern to an event-driven gateway pattern. This migration guide provides step-by-step instructions for updating your code to use the new EventSessionLogger system.

## Why Migrate?

The new EventSessionLogger provides:

- **Better Separation of Concerns**: Agents focus on LLM interactions, logging is handled separately
- **Future-Proof Architecture**: Ready for transport layer integration (queues, message buses)
- **Error Isolation**: Logging failures don't affect agent operations
- **Enhanced Performance**: Gateway pattern with optimized event handling
- **Flexible Configuration**: Environment-based configuration and factory patterns

## Migration Timeline

- **Deprecated in**: Version 2.0.0
- **Will be removed in**: Version 3.0.0
- **Recommended Action**: Migrate now to avoid breaking changes

## Quick Migration Examples

### Basic File Logging

**OLD PATTERN (Deprecated):**

```python
from agent_c.util.session_logger import SessionLogger
from agent_c.agent_runtimes.claude import ClaudeChatAgentRuntime

# Old embedded pattern
session_logger = SessionLogger(log_file_path="./logs/session.log")
agent = ClaudeChatAgentRuntime(session_logger=session_logger)
```

**NEW PATTERN (Recommended):**

```python
from agent_c.util.event_session_logger_factory import create_session_logger
from agent_c.agent_runtimes.claude import ClaudeChatAgentRuntime

# New gateway pattern
logger = create_session_logger(log_base_dir="./logs")
agent = ClaudeChatAgentRuntime(streaming_callback=logger)
```

### With Callback Function

**OLD PATTERN:**
```python
def my_callback(event):
    print(f"Event: {event}")

session_logger = SessionLogger(log_file_path="./logs/session.log")
# Callback handling was mixed with agent logic
```

**NEW PATTERN:**
```python
from agent_c.util.event_session_logger_factory import create_with_callback

async def my_callback(event):
    print(f"Event: {event}")

# Clean separation: logger handles both file logging and callback
logger = create_with_callback(
    callback=my_callback,
    log_base_dir="./logs"
)
agent = ClaudeChatAgent(streaming_callback=logger)
```

### Environment-Based Configuration

**NEW PATTERN:**
```python
import os
from agent_c.util.event_session_logger_factory import create_from_environment

# Set environment variables
os.environ['AGENT_LOG_DIR'] = './logs'
os.environ['AGENT_LOG_FORMAT'] = 'jsonl'

# Create logger from environment
logger = create_from_environment()
agent = ClaudeChatAgent(streaming_callback=logger)
```

## Automatic Migration Helpers

### Using Migration Utilities

```python
from agent_c.util.session_logger_migration import (
    migrate_agent_initialization,
    migrate_simple_logging,
    quick_migrate_agent
)

# Option 1: Automatic migration from old SessionLogger
old_session_logger = SessionLogger(log_file_path="./logs/session.log")
agent, logger = migrate_agent_initialization(
    ClaudeChatAgent,
    session_logger=old_session_logger  # Automatically converted
)

# Option 2: Quick migration for simple cases
agent, logger = quick_migrate_agent(ClaudeChatAgent, log_dir="./logs")

# Option 3: Migrate just the logging part
logger = migrate_simple_logging("./logs/session.log")
agent = ClaudeChatAgent(streaming_callback=logger)
```

### Migration with Validation

```python
from agent_c.util.session_logger_migration import validate_migration_config

# Check your configuration
config = {
    'log_base_dir': './logs',
    'include_system_prompt': True
}

issues = validate_migration_config(**config)
if issues:
    for issue in issues:
        print(f"⚠️  {issue}")
else:
    print("✅ Configuration is valid")
```

## Advanced Migration Patterns

### Multiple Transport Targets

**NEW CAPABILITY:**
```python
from agent_c.util.event_session_logger_factory import create_multi_transport_logger
from agent_c.util.transports import CallbackTransport, LoggingTransport

# Create multiple transports
callback_transport = CallbackTransport(my_callback)
logging_transport = LoggingTransport()

# Logger that sends to multiple destinations
logger = create_multi_transport_logger(
    log_base_dir="./logs",
    transports=[callback_transport, logging_transport]
)

agent = ClaudeChatAgent(streaming_callback=logger)
```

### Development vs Production

```python
from agent_c.util.event_session_logger_factory import (
    create_development_logger,
    create_production_logger
)

# Development setup
if os.environ.get('ENV') == 'development':
    logger = create_development_logger(
        log_base_dir="./logs",
        enable_debug=True
    )
else:
    # Production setup with retry and monitoring
    logger = create_production_logger(
        log_base_dir="/var/log/agent",
        transport_type=TransportType.QUEUE,
        transport_config={'queue_url': 'redis://localhost:6379'},
        enable_retry=True
    )

agent = ClaudeChatAgent(streaming_callback=logger)
```

## Configuration Migration

### Environment Variables

The new system supports comprehensive environment-based configuration:

```bash
# Basic configuration
export AGENT_LOG_DIR="./logs"
export AGENT_LOG_FORMAT="jsonl"
export AGENT_INCLUDE_SYSTEM_PROMPT="true"

# Advanced configuration
export AGENT_MAX_RETRY_ATTEMPTS="3"
export AGENT_RETRY_DELAY_SECONDS="1.0"
export AGENT_ENABLE_LOCAL_LOGGING="true"
```

### Configuration File Migration

**OLD:** SessionLogger with hardcoded paths
```python
session_logger = SessionLogger(
    log_file_path="./logs/session.log",
    include_system_prompt=True
)
```

**NEW:** EventSessionLogger with flexible configuration
```python
from agent_c.util.event_session_logger_factory import LoggerConfiguration, create_logger_from_config

config = LoggerConfiguration(
    log_base_dir="./logs",
    include_system_prompt=True,
    log_format="jsonl",
    enable_retry=True,
    max_retry_attempts=3
)

logger = create_logger_from_config(config)
```

## Testing Your Migration

### Validation Steps

1. **Check for Deprecation Warnings:**
```python
import warnings
warnings.filterwarnings('error', category=SessionLoggerDeprecationWarning)

# Your code here - will raise exception if deprecated patterns detected
```

2. **Verify Event Flow:**
```python
# Test that events are properly logged
logger = create_session_logger(log_base_dir="./test_logs")
agent = ClaudeChatAgent(streaming_callback=logger)

# Check log files are created and contain events
```

3. **Performance Testing:**
```python
from agent_c.util.event_session_logger_factory import create_testing_logger

# Use testing logger for unit tests
logger = create_testing_logger(transport_type=TransportType.NULL)
agent = ClaudeChatAgent(streaming_callback=logger)
```

### Migration Checklist

- [ ] Replace `SessionLogger` imports with `event_session_logger_factory`
- [ ] Update agent initialization to use `streaming_callback`
- [ ] Remove `session_logger` parameters from agent constructors
- [ ] Remove calls to `agent.initialize_session_logger()`
- [ ] Update log file paths to use directories instead of files
- [ ] Test that events are properly logged
- [ ] Verify callback functions work with new pattern
- [ ] Update environment variable configuration
- [ ] Run tests to ensure no functionality is lost
- [ ] Check for and resolve deprecation warnings

## Common Migration Issues

### Issue 1: Log File vs Directory

**Problem:** Old SessionLogger used file paths, new system uses directories
```python
# OLD - file path
SessionLogger(log_file_path="./logs/session.log")

# NEW - directory path
create_session_logger(log_base_dir="./logs")
```

**Solution:** The new system automatically creates session-specific files within the directory.

### Issue 2: Callback Function Signatures

**Problem:** Callback functions need to be async
```python
# OLD - sync callback
def my_callback(event):
    process_event(event)

# NEW - async callback
async def my_callback(event):
    await process_event(event)
```

### Issue 3: Agent Initialization Order

**Problem:** SessionLogger was passed to agent constructor
```python
# OLD
agent = ClaudeChatAgent(session_logger=logger)

# NEW
logger = create_session_logger()
agent = ClaudeChatAgent(streaming_callback=logger)
```

## Getting Help

### Built-in Migration Help

```python
from agent_c.util.session_logger_migration import print_migration_guide

# Print comprehensive migration guide
print_migration_guide()
```

### Debugging Migration Issues

```python
from agent_c.util.session_logger_migration import MigrationHelper

# Enable detailed deprecation warnings
MigrationHelper.warn_deprecated_usage(
    old_pattern="Your old pattern",
    new_pattern="Recommended new pattern",
    context="Where you found this"
)
```

### Validation and Configuration Help

```python
from agent_c.util.event_session_logger_factory import (
    validate_logger_config,
    print_logger_info
)

# Validate your configuration
config = LoggerConfiguration(log_base_dir="./logs")
if validate_logger_config(config):
    logger = create_logger_from_config(config)
    print_logger_info(logger)
```

## Future-Proofing

The new EventSessionLogger is designed for future transport integration:

```python
# Current transition pattern
logger = create_with_callback(callback=my_callback, log_base_dir="./logs")

# Future transport pattern (when available)
logger = create_with_transport(transport=message_bus, log_base_dir="./logs")
```

This migration prepares your code for:
- Message queue integration (RabbitMQ, Redis, etc.)
- Cloud logging services
- Real-time event streaming
- Distributed logging architectures

## Summary

The migration from SessionLogger to EventSessionLogger provides:

1. **Immediate Benefits:** Better architecture, error isolation, enhanced performance
2. **Future Benefits:** Transport layer integration, cloud-native logging, scalability
3. **Migration Support:** Comprehensive utilities, validation, and documentation
4. **Backward Compatibility:** Smooth transition with deprecation warnings

Start your migration today to take advantage of the improved architecture and prepare for future enhancements!
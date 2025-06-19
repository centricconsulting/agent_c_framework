# EventSessionLogger API Reference

## Overview

The EventSessionLogger is the core component of the event-driven logging system, implementing a gateway pattern that bridges agent events with transport layers. It provides local logging with optional downstream forwarding, error isolation, and support for future transport integration.

## Class: EventSessionLogger

```python
class EventSessionLogger:
    """
    Gateway pattern implementation for event-driven session logging.
    
    Acts as a bridge between agent events and transport layers, providing:
    - Local logging with session-based file organization
    - Optional downstream forwarding (callback or transport)
    - Error isolation between logging and transport concerns
    - Backward compatibility with current SessionLogger behavior
    """
```

### Constructor

```python
def __init__(
    self,
    log_base_dir: Union[str, Path] = None,
    downstream_callback: Optional[Callable[[Any], Awaitable[None]]] = None,
    downstream_transport: Optional[TransportInterface] = None,
    include_system_prompt: bool = True,
    log_format: str = "jsonl",
    file_naming_pattern: str = "%Y%m%d_%H%M%S",
    error_handler: Optional[Callable[[Exception, str], None]] = None,
    max_retry_attempts: int = 3,
    retry_delay_seconds: float = 1.0,
    enable_local_logging: bool = True,
    session_directory_pattern: str = "{session_id}",
    unknown_session_pattern: str = "unknown_{uuid}",
    **kwargs
) -> None
```

#### Parameters

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `log_base_dir` | `Union[str, Path]` | `None` | Base directory for logs. If None, uses `AGENT_LOG_DIR` environment variable or `./logs` |
| `downstream_callback` | `Optional[Callable]` | `None` | Optional async callback for current transition pattern |
| `downstream_transport` | `Optional[TransportInterface]` | `None` | Optional transport interface for future decoupling |
| `include_system_prompt` | `bool` | `True` | Whether to log system prompts (backward compatibility) |
| `log_format` | `str` | `"jsonl"` | Log format: "jsonl", "json", or "custom" |
| `file_naming_pattern` | `str` | `"%Y%m%d_%H%M%S"` | strftime pattern for log file names |
| `error_handler` | `Optional[Callable]` | `None` | Custom error handling callback |
| `max_retry_attempts` | `int` | `3` | Maximum retries for downstream operations |
| `retry_delay_seconds` | `float` | `1.0` | Delay between retry attempts |
| `enable_local_logging` | `bool` | `True` | Whether to perform local file logging |
| `session_directory_pattern` | `str` | `"{session_id}"` | Pattern for session directories |
| `unknown_session_pattern` | `str` | `"unknown_{uuid}"` | Pattern for unknown session directories |

#### Example

```python
# Basic usage
logger = EventSessionLogger(log_base_dir="./logs")

# With callback
async def my_callback(event):
    print(f"Event: {type(event).__name__}")

logger = EventSessionLogger(
    log_base_dir="./logs",
    downstream_callback=my_callback,
    max_retry_attempts=5,
    retry_delay_seconds=2.0
)

# With transport
from agent_c.util.transports import CallbackTransport

transport = CallbackTransport(my_callback)
logger = EventSessionLogger(
    log_base_dir="./logs",
    downstream_transport=transport
)
```

### Methods

#### `__call__(event: Any) -> bool`

Main gateway method that processes events through the logging pipeline.

```python
async def __call__(self, event: Any) -> bool:
    """
    Process event through the gateway.
    
    Always logs locally first, then forwards to downstream if configured.
    Error isolation ensures local logging success doesn't depend on transport.
    
    Args:
        event: Event object (SessionEvent, SemiSessionEvent, or any serializable object)
        
    Returns:
        bool: True if local logging succeeded (transport failures don't affect return value)
        
    Raises:
        EventSessionLoggerError: Only for critical local logging failures
    """
```

**Usage:**
```python
from agent_c.models.events.chat import InteractionEvent

event = InteractionEvent(
    session_id="session_123",
    role="user",
    interaction_id="interaction_1",
    event_type="interaction_start"
)

success = await logger(event)
print(f"Event processed: {success}")
```

#### `get_session_directory(session_id: str) -> Path`

Get the directory path for a specific session.

```python
def get_session_directory(self, session_id: str) -> Path:
    """
    Get the session directory path.
    
    Args:
        session_id: Session identifier
        
    Returns:
        Path: Directory path for the session
    """
```

**Usage:**
```python
session_dir = logger.get_session_directory("session_123")
print(f"Session logs stored in: {session_dir}")
```

#### `get_log_file_path(session_id: str) -> Path`

Get the current log file path for a session.

```python
def get_log_file_path(self, session_id: str) -> Path:
    """
    Get the current log file path for a session.
    
    Args:
        session_id: Session identifier
        
    Returns:
        Path: Current log file path for the session
    """
```

**Usage:**
```python
log_file = logger.get_log_file_path("session_123")
print(f"Current log file: {log_file}")
```

#### `configure_from_env() -> None`

Load configuration from environment variables.

```python
def configure_from_env(self) -> None:
    """
    Load configuration from environment variables.
    
    Supported environment variables:
    - AGENT_LOG_DIR: Base directory for logs
    - AGENT_LOG_FORMAT: Log format (jsonl, json, custom)
    - AGENT_INCLUDE_SYSTEM_PROMPT: Include system prompts (true/false)
    - AGENT_MAX_RETRY_ATTEMPTS: Maximum retry attempts
    - AGENT_RETRY_DELAY_SECONDS: Retry delay in seconds
    - AGENT_ENABLE_LOCAL_LOGGING: Enable local logging (true/false)
    """
```

**Usage:**
```python
import os

os.environ['AGENT_LOG_DIR'] = './custom_logs'
os.environ['AGENT_MAX_RETRY_ATTEMPTS'] = '5'

logger = EventSessionLogger()
logger.configure_from_env()
```

#### `update_downstream_callback(callback: Optional[Callable]) -> None`

Update the downstream callback at runtime.

```python
def update_downstream_callback(self, callback: Optional[Callable]) -> None:
    """
    Update downstream callback for runtime reconfiguration.
    
    Args:
        callback: New callback function or None to remove
    """
```

**Usage:**
```python
async def new_callback(event):
    print(f"New callback: {event}")

logger.update_downstream_callback(new_callback)
```

#### `update_downstream_transport(transport: Optional[TransportInterface]) -> None`

Update the downstream transport at runtime.

```python
def update_downstream_transport(self, transport: Optional[TransportInterface]) -> None:
    """
    Update downstream transport for runtime reconfiguration.
    
    Args:
        transport: New transport interface or None to remove
    """
```

**Usage:**
```python
from agent_c.util.transports import HTTPTransport

new_transport = HTTPTransport("https://new.endpoint.com")
logger.update_downstream_transport(new_transport)
```

#### `close() -> None`

Clean shutdown - close transport connections and flush logs.

```python
def close(self) -> None:
    """
    Clean shutdown - close transport connections and flush logs.
    
    Should be called when shutting down to ensure proper resource cleanup.
    """
```

**Usage:**
```python
# Cleanup when done
await logger.close()

# Or use as async context manager
async with EventSessionLogger(log_base_dir="./logs") as logger:
    # Use logger
    await logger(event)
# Automatically closed
```

### Async Context Manager Support

EventSessionLogger supports async context manager protocol for automatic resource management.

```python
async def __aenter__(self) -> 'EventSessionLogger':
    """Async context manager entry"""
    return self

async def __aexit__(self, exc_type, exc_val, exc_tb) -> None:
    """Async context manager exit with cleanup"""
    await self.close()
```

**Usage:**
```python
async with EventSessionLogger(log_base_dir="./logs") as logger:
    await logger(event1)
    await logger(event2)
    # Automatically cleaned up
```

### Properties

#### `log_base_dir: Path`

Base directory for all log files (read-only).

```python
print(f"Logs stored in: {logger.log_base_dir}")
```

#### `enable_local_logging: bool`

Whether local file logging is enabled (read-only).

```python
if logger.enable_local_logging:
    print("Local logging is enabled")
```

#### `downstream_callback: Optional[Callable]`

Current downstream callback function (read-only).

```python
if logger.downstream_callback:
    print("Downstream callback is configured")
```

#### `downstream_transport: Optional[TransportInterface]`

Current downstream transport interface (read-only).

```python
if logger.downstream_transport:
    print("Downstream transport is configured")
```

#### `include_system_prompt: bool`

Whether system prompts are included in logs (read-only).

```python
if logger.include_system_prompt:
    print("System prompts will be logged")
```

#### `max_retry_attempts: int`

Maximum retry attempts for downstream operations (read-only).

```python
print(f"Max retries: {logger.max_retry_attempts}")
```

#### `retry_delay_seconds: float`

Delay between retry attempts in seconds (read-only).

```python
print(f"Retry delay: {logger.retry_delay_seconds}s")
```

## Error Handling

### Exception Types

```python
from agent_c.util.transport_exceptions import (
    EventSessionLoggerError,
    LocalLoggingError,
    SerializationError,
    TransportError
)
```

#### `EventSessionLoggerError`

Base exception for EventSessionLogger errors.

```python
class EventSessionLoggerError(Exception):
    """Base exception for EventSessionLogger errors"""
    pass
```

#### `LocalLoggingError`

Raised when local file logging fails.

```python
class LocalLoggingError(EventSessionLoggerError):
    """Raised when local logging operations fail"""
    pass
```

#### `SerializationError`

Raised when event serialization fails.

```python
class SerializationError(EventSessionLoggerError):
    """Raised when event serialization fails"""
    pass
```

### Error Isolation

The EventSessionLogger implements error isolation between local logging and transport operations:

```python
try:
    success = await logger(event)
    # success is True if local logging succeeded
    # Transport failures don't affect the return value
except EventSessionLoggerError as e:
    # Only critical local logging failures raise exceptions
    print(f"Critical logging error: {e}")
```

### Custom Error Handling

```python
def custom_error_handler(error: Exception, context: str) -> None:
    """Custom error handler for transport failures"""
    print(f"Error in {context}: {error}")
    # Log to monitoring system, send alerts, etc.

logger = EventSessionLogger(
    log_base_dir="./logs",
    error_handler=custom_error_handler
)
```

## Configuration

### Environment Variables

| Variable | Type | Default | Description |
|----------|------|---------|-------------|
| `AGENT_LOG_DIR` | `str` | `"./logs"` | Base directory for logs |
| `AGENT_LOG_FORMAT` | `str` | `"jsonl"` | Log format (jsonl, json, custom) |
| `AGENT_INCLUDE_SYSTEM_PROMPT` | `bool` | `true` | Include system prompts in logs |
| `AGENT_MAX_RETRY_ATTEMPTS` | `int` | `3` | Maximum retry attempts |
| `AGENT_RETRY_DELAY_SECONDS` | `float` | `1.0` | Retry delay in seconds |
| `AGENT_ENABLE_LOCAL_LOGGING` | `bool` | `true` | Enable local file logging |

### Configuration Loading

```python
# Automatic environment loading
logger = EventSessionLogger()  # Uses environment variables

# Manual configuration
logger = EventSessionLogger(
    log_base_dir="./custom_logs",
    max_retry_attempts=5,
    retry_delay_seconds=2.0
)

# Runtime reconfiguration
logger.configure_from_env()
```

## Usage Patterns

### 1. Basic Gateway Pattern

```python
# Create logger
logger = EventSessionLogger(log_base_dir="./logs")

# Use with agent
agent = ClaudeChatAgent(streaming_callback=logger)

# Events automatically flow through gateway
response = await agent.chat(user_input="Hello")
```

### 2. Transition Pattern (Current Callbacks)

```python
async def existing_callback(event):
    # Your existing event processing logic
    pass

# Bridge to existing callback
logger = EventSessionLogger(
    log_base_dir="./logs",
    downstream_callback=existing_callback
)

agent = ClaudeChatAgent(streaming_callback=logger)
```

### 3. Future Transport Pattern

```python
from agent_c.util.transports import HTTPTransport

# Future transport integration
transport = HTTPTransport("https://events.api.com")
logger = EventSessionLogger(
    log_base_dir="./logs",
    downstream_transport=transport
)

agent = ClaudeChatAgent(streaming_callback=logger)
```

### 4. Development Pattern

```python
# Simple development setup
logger = EventSessionLogger(
    log_base_dir="./dev_logs",
    enable_local_logging=True
    # No downstream - just local logging
)
```

### 5. Production Pattern

```python
# Production setup with monitoring
async def monitoring_callback(event):
    # Send to monitoring system
    pass

logger = EventSessionLogger(
    log_base_dir="/var/log/agent",
    downstream_callback=monitoring_callback,
    max_retry_attempts=5,
    retry_delay_seconds=2.0,
    error_handler=production_error_handler
)
```

## Performance Characteristics

### Throughput

- **Local logging**: >1000 events/second
- **Gateway overhead**: <5% compared to direct file writing
- **Memory usage**: Minimal (streaming processing, no buffering)
- **Concurrent sessions**: Scales linearly with available I/O

### Optimization Tips

1. **Use appropriate log formats**: JSONL is fastest for high volume
2. **Configure retry settings**: Lower retries for better throughput
3. **Use transport batching**: For high-volume downstream delivery
4. **Monitor disk I/O**: Ensure adequate storage performance
5. **Use SSD storage**: For better I/O performance

### Benchmarking

```python
import time
import asyncio

async def benchmark_logger():
    logger = EventSessionLogger(log_base_dir="./benchmark_logs")
    
    num_events = 1000
    start_time = time.time()
    
    for i in range(num_events):
        event = InteractionEvent(
            session_id=f"session_{i % 10}",
            role="user",
            interaction_id=f"interaction_{i}",
            event_type="benchmark"
        )
        await logger(event)
    
    end_time = time.time()
    duration = end_time - start_time
    
    print(f"Processed {num_events} events in {duration:.2f}s")
    print(f"Rate: {num_events/duration:.1f} events/second")

# Run benchmark
asyncio.run(benchmark_logger())
```

## Integration Examples

### With Different Agent Types

```python
# Claude agent
claude_logger = EventSessionLogger(log_base_dir="./claude_logs")
claude_agent = ClaudeChatAgent(streaming_callback=claude_logger)

# GPT agent
gpt_logger = EventSessionLogger(log_base_dir="./gpt_logs")
gpt_agent = GPTChatAgent(streaming_callback=gpt_logger)

# Shared logger for multiple agents
shared_logger = EventSessionLogger(log_base_dir="./shared_logs")
claude_agent = ClaudeChatAgent(streaming_callback=shared_logger)
gpt_agent = GPTChatAgent(streaming_callback=shared_logger)
```

### With Session Managers

```python
from agent_c.chat.session_manager import ChatSessionManager

# Create session manager and logger
session_manager = ChatSessionManager()
logger = EventSessionLogger(log_base_dir="./session_logs")

# Use together
agent = ClaudeChatAgent(
    streaming_callback=logger,
    session_manager=session_manager
)
```

### With Custom Event Types

```python
from agent_c.models.events.base import SessionEvent

class CustomEvent(SessionEvent):
    event_type: str = "custom"
    custom_data: str

# Logger handles any event type
custom_event = CustomEvent(
    session_id="custom_session",
    role="system",
    custom_data="Custom event data"
)

success = await logger(custom_event)
```

## Migration from SessionLogger

### Automatic Migration

```python
from agent_c.util.session_logger_migration import migrate_agent_initialization
from agent_c.util.session_logger import SessionLogger

# Old pattern
old_logger = SessionLogger(log_file_path="./logs/session.log")

# Automatic migration
agent, new_logger = migrate_agent_initialization(
    ClaudeChatAgent,
    session_logger=old_logger  # Automatically converted
)
```

### Manual Migration

```python
# OLD PATTERN
from agent_c.util.session_logger import SessionLogger

session_logger = SessionLogger(log_file_path="./logs/session.log")
agent = ClaudeChatAgent(session_logger=session_logger)

# NEW PATTERN
from agent_c.util.event_logging.event_session_logger import EventSessionLogger

logger = EventSessionLogger(log_base_dir="./logs")
agent = ClaudeChatAgent(streaming_callback=logger)
```

## Best Practices

### 1. Resource Management

```python
# Always use async context manager for automatic cleanup
async with EventSessionLogger(log_base_dir="./logs") as logger:
    # Use logger
    pass

# Or manually close
logger = EventSessionLogger(log_base_dir="./logs")
try:
    # Use logger
    pass
finally:
    await logger.close()
```

### 2. Error Handling

```python
# Handle critical errors
try:
    success = await logger(event)
except EventSessionLoggerError as e:
    # Critical error - local logging failed
    handle_critical_error(e)

# Transport errors are handled internally and don't raise exceptions
```

### 3. Configuration

```python
# Use environment variables for deployment flexibility
import os

os.environ['AGENT_LOG_DIR'] = '/var/log/agent'
os.environ['AGENT_MAX_RETRY_ATTEMPTS'] = '5'

logger = EventSessionLogger()  # Uses environment config
```

### 4. Performance

```python
# For high-volume scenarios
logger = EventSessionLogger(
    log_base_dir="./logs",
    max_retry_attempts=1,        # Reduce retries
    retry_delay_seconds=0.1,     # Faster retries
    log_format="jsonl"           # Fastest format
)
```

### 5. Monitoring

```python
# Add monitoring callback
async def monitor_events(event):
    # Send metrics to monitoring system
    metrics.increment('events.processed')
    metrics.histogram('events.size', len(str(event)))

logger = EventSessionLogger(
    log_base_dir="./logs",
    downstream_callback=monitor_events
)
```
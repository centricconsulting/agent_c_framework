# Redis Streams Integration Guide

## Overview

The Redis Streams integration enables distributed event processing in the Agent C system, allowing events to be shared across multiple application instances while maintaining backward compatibility with the existing callback mechanism.

## Key Features

- **Distributed Event Processing**: Events can be consumed by multiple application instances
- **Event Persistence**: Events are stored durably in Redis with configurable retention
- **Backward Compatibility**: Existing streaming callbacks continue to work unchanged
- **Graceful Fallback**: Automatic fallback to in-memory queues when Redis is unavailable
- **Error Handling**: Comprehensive error handling with circuit breaker patterns

## Architecture Components

### RedisStreamManager

The core component that handles Redis Stream operations:

```python
from agent_c.streaming.redis_stream_manager import RedisStreamManager, RedisConfig

# Create configuration
config = RedisConfig()
config.host = 'localhost'
config.port = 6379
config.enabled = True

# Create manager
redis_manager = RedisStreamManager(config)
await redis_manager.initialize()
```

### StreamKeyManager

Utility for managing Redis Stream key naming conventions:

```python
from agent_c.streaming.stream_key_manager import StreamKeyManager

# Build stream key
key = StreamKeyManager.build_stream_key("user_123", "chat_001")
# Result: "agent_c:stream:user_123:chat_001"

# Parse stream key
session_id, interaction_id = StreamKeyManager.parse_stream_key(key)
```

### EventSerializer

Handles serialization/deserialization of events for Redis Streams:

```python
from agent_c.streaming.event_serializer import EventSerializer, EventContext

# Create context
context = EventContext(
    session_id="user_123",
    interaction_id="chat_001",
    source="BaseAgent"
)

# Serialize event
serialized = EventSerializer.serialize_event(event, context)
```

## BaseAgent Integration

The BaseAgent class has been enhanced to support Redis Streams while maintaining full backward compatibility.

### Basic Usage

```python
from agent_c.agents.base import BaseAgent
from agent_c.streaming.redis_stream_manager import RedisStreamManager

# Create your agent (inherit from BaseAgent)
class MyAgent(BaseAgent):
    # Your agent implementation
    pass

# Create agent instance
agent = MyAgent(model_name="gpt-4")

# Configure Redis Streams (optional)
redis_manager = RedisStreamManager()
await redis_manager.initialize()

agent.set_redis_stream_manager(redis_manager)
agent.set_current_stream_id("user_session_123", "chat_interaction_456")

# Use agent normally - events will be published to both Redis and callbacks
await agent._raise_text_delta("Hello world!")
```

### Event Publishing

All `_raise_*` methods now support Redis Streams:

```python
# All these methods now publish to Redis Streams if configured
await agent._raise_interaction_start()
await agent._raise_text_delta("Processing...")
await agent._raise_tool_call_start(tool_calls)
await agent._raise_completion_start(options)
await agent._raise_system_event("Process complete", severity="info")
await agent._raise_interaction_end()
```

### Explicit Stream ID

You can override the current stream ID for specific events:

```python
# Use explicit stream ID for this event
await agent._raise_text_delta(
    "Special message", 
    stream_id="special_session:special_interaction"
)
```

### Backward Compatibility

Existing code continues to work without changes:

```python
# This works exactly as before
callback = MyStreamingCallback()
agent.streaming_callback = callback

# Events go to both Redis Streams AND the callback
await agent._raise_text_delta("Hello world!")
```

## Configuration

### Environment Variables

Configure Redis Streams using environment variables:

```bash
# Redis Connection
REDIS_HOST=localhost
REDIS_PORT=6379
REDIS_PASSWORD=your_password
REDIS_DB=0
REDIS_SSL=false

# Redis Streams Configuration
REDIS_STREAM_MAX_LEN=1000
REDIS_STREAM_TTL=3600
REDIS_STREAM_READ_TIMEOUT=1000

# Connection Pool Configuration
REDIS_MAX_CONNECTIONS=10
REDIS_SOCKET_TIMEOUT=5
REDIS_SOCKET_CONNECT_TIMEOUT=5

# Feature Flags
ENABLE_REDIS_STREAMS=true
REDIS_STREAMS_FALLBACK_TO_QUEUE=true
```

### Programmatic Configuration

```python
from agent_c.streaming.redis_stream_manager import RedisConfig

config = RedisConfig()
config.host = 'redis.example.com'
config.port = 6379
config.password = 'secure_password'
config.enabled = True
config.stream_max_length = 2000
config.stream_ttl = 7200  # 2 hours

redis_manager = RedisStreamManager(config)
```

## Error Handling and Fallback

### Automatic Fallback

When Redis is unavailable, the system automatically falls back to in-memory queues:

```python
# This continues to work even if Redis is down
await agent._raise_text_delta("Hello world!")
```

### Error Monitoring

Monitor Redis health and fallback status:

```python
# Check Redis health
if redis_manager.is_healthy:
    print("Redis is available")
else:
    print("Using fallback queue")

# Check fallback status
if redis_manager.is_fallback_active:
    print("Currently using fallback mode")
```

### Manual Error Handling

```python
try:
    message_id = await redis_manager.publish_event(
        event_type="custom_event",
        event_data={"key": "value"},
        session_id="user_123",
        interaction_id="chat_001"
    )
    print(f"Published to Redis: {message_id}")
except Exception as e:
    print(f"Redis publish failed: {e}")
    # Event automatically goes to fallback queue
```

## Event Consumption

### Basic Consumption

```python
# Consume events from a stream
async for event_data in redis_manager.consume_events(
    session_id="user_123",
    interaction_id="chat_001",
    block=1000  # Block for 1 second
):
    print(f"Received event: {event_data}")
```

### Processing Events

```python
async def process_stream_events(session_id: str, interaction_id: str):
    async for event_data in redis_manager.consume_events(session_id, interaction_id):
        event_type = event_data.get('type')
        
        if event_type == 'text_delta':
            content = event_data['data']['content']
            print(f"Text delta: {content}")
        
        elif event_type == 'tool_call':
            tool_calls = event_data['data']['tool_calls']
            print(f"Tool call: {tool_calls}")
        
        # Add more event type handling as needed
```

## Stream Lifecycle Management

### Creating Streams

```python
# Create a new stream
stream_key = await redis_manager.create_stream("user_123", "chat_001")
print(f"Created stream: {stream_key}")
```

### Closing Streams

```python
# Close a stream (adds final metadata and extends TTL)
await redis_manager.close_stream("user_123", "chat_001")
```

### Cleanup

```python
# Clean up old streams (automatically called periodically)
await redis_manager.cleanup_old_streams(max_age_seconds=3600)
```

## Best Practices

### 1. Session and Interaction IDs

Use meaningful, unique identifiers:

```python
# Good: descriptive and unique
session_id = f"user_{user_id}_{timestamp}"
interaction_id = f"chat_{uuid.uuid4()}"

# Avoid: generic or non-unique IDs
session_id = "session1"  # Too generic
interaction_id = "chat"  # Not unique
```

### 2. Stream ID Management

Set stream IDs early in your interaction:

```python
# Set at the beginning of each interaction
agent.set_current_stream_id(session_id, interaction_id)

# Create the stream
await redis_manager.create_stream(session_id, interaction_id)

# Use agent normally
await agent._raise_interaction_start()
# ... rest of interaction ...
await agent._raise_interaction_end()

# Clean up at the end
await redis_manager.close_stream(session_id, interaction_id)
```

### 3. Error Handling

Always handle Redis errors gracefully:

```python
try:
    await redis_manager.initialize()
    agent.set_redis_stream_manager(redis_manager)
except Exception as e:
    logger.warning(f"Redis Streams unavailable: {e}")
    # Continue without Redis - callbacks will still work
```

### 4. Resource Cleanup

Properly close Redis connections:

```python
try:
    # Your application logic
    await agent._raise_text_delta("Hello world!")
finally:
    # Clean up
    await redis_manager.close()
```

## Testing

### Unit Testing

Mock Redis for unit tests:

```python
from unittest.mock import Mock, AsyncMock

# Mock Redis manager
mock_redis = Mock()
mock_redis.publish_event = AsyncMock(return_value="msg-123")
mock_redis.is_healthy = True

agent.set_redis_stream_manager(mock_redis)
await agent._raise_text_delta("Test message")

# Verify Redis was called
mock_redis.publish_event.assert_called_once()
```

### Integration Testing

Test with real Redis when available:

```python
@pytest.mark.integration
async def test_with_real_redis():
    try:
        redis_manager = RedisStreamManager()
        await redis_manager.initialize()
        
        if redis_manager.is_healthy:
            # Test with real Redis
            await redis_manager.publish_event(...)
        else:
            pytest.skip("Redis not available")
    except Exception:
        pytest.skip("Redis not available")
```

## Migration Guide

### Existing Applications

To add Redis Streams to existing applications:

1. **Install Redis** (if not already available)

2. **Update your agent initialization**:
   ```python
   # Before
   agent = MyAgent(streaming_callback=my_callback)
   
   # After
   agent = MyAgent(streaming_callback=my_callback)
   
   # Add Redis Streams (optional)
   redis_manager = RedisStreamManager()
   await redis_manager.initialize()
   agent.set_redis_stream_manager(redis_manager)
   ```

3. **Set stream IDs for each interaction**:
   ```python
   # At the start of each chat/interaction
   agent.set_current_stream_id(session_id, interaction_id)
   ```

4. **No other changes required** - existing `_raise_*` calls work unchanged

### Gradual Rollout

1. **Phase 1**: Enable Redis Streams in development
2. **Phase 2**: Enable in staging with monitoring
3. **Phase 3**: Enable in production with fallback active
4. **Phase 4**: Rely on Redis Streams with callback fallback

## Troubleshooting

### Common Issues

1. **Redis connection fails**
   - Check Redis server is running
   - Verify connection parameters
   - Check network connectivity
   - Review firewall settings

2. **Events not appearing in streams**
   - Verify `ENABLE_REDIS_STREAMS=true`
   - Check agent has Redis manager configured
   - Ensure stream ID is set
   - Check Redis health: `redis_manager.is_healthy`

3. **High memory usage in Redis**
   - Reduce `REDIS_STREAM_MAX_LEN`
   - Decrease `REDIS_STREAM_TTL`
   - Enable more aggressive cleanup

4. **Fallback queue filling up**
   - Check Redis connectivity
   - Monitor `is_fallback_active` status
   - Increase fallback queue size if needed

### Debugging

Enable debug logging:

```python
import logging
logging.getLogger('agent_c.streaming').setLevel(logging.DEBUG)
```

Check Redis directly:

```bash
# Connect to Redis
redis-cli

# List all streams
KEYS agent_c:stream:*

# Check stream length
XLEN agent_c:stream:user_123:chat_001

# Read from stream
XREAD STREAMS agent_c:stream:user_123:chat_001 0
```

## Performance Considerations

### Throughput

- Default configuration supports ~1000 events/second per stream
- Use connection pooling for higher throughput
- Consider batching for high-volume scenarios

### Latency

- Typical latency: < 10ms for event publishing
- Network latency to Redis server affects performance
- Use Redis clusters for geographic distribution

### Memory

- Each stream consumes memory in Redis
- Configure appropriate `REDIS_STREAM_MAX_LEN`
- Monitor Redis memory usage
- Set reasonable TTL values

## Conclusion

The Redis Streams integration provides a powerful foundation for distributed event processing while maintaining complete backward compatibility. It enables horizontal scaling, event persistence, and improved reliability without requiring changes to existing agent code.

Start with the fallback mode enabled and gradually transition to Redis-first operation as confidence builds. The system is designed to gracefully handle Redis outages and provide seamless operation in all scenarios.
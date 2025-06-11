# Redis Streams Exclusive Callback Mode

## Overview

Agent C's event system supports two different modes for callback handling:

1. **Hybrid Mode (Default)**: Events are published to Redis Streams AND processed through direct callbacks
2. **Exclusive Redis Mode**: Events are published ONLY to Redis Streams when in Redis mode

The exclusive Redis mode is useful for distributed systems where you want to ensure all events are handled through Redis Streams for consistency, scalability, and fault tolerance.

## Configuration

You can enable exclusive Redis callback mode using the following methods:

### Environment Variables

```bash
ENABLE_REDIS_STREAMS=true
REDIS_EXCLUSIVE_CALLBACK=true
```

### Constructor Parameters

```python
from agent_c.streaming.redis_stream_manager import RedisConfig

redis_config = RedisConfig(
    enabled=True,
    exclusive_callback_via_redis=True
)

# Pass this config to your agent
agent = BaseAgent(
    model_name="claude-3-opus-20240229",
    redis_config=redis_config
)
```

### Configuration File

You can also set this in a YAML or JSON configuration file:

```yaml
redis:
  enabled: true
  exclusive_callback_via_redis: true
  host: localhost
  port: 6379
```

## Behavior

When `exclusive_callback_via_redis` is enabled:

1. In `REDIS` mode, events will ONLY be published to Redis Streams and NOT to direct callbacks
2. In `ASYNC` mode, events will be processed through direct callbacks as usual
3. During failover scenarios, events will fall back to direct callbacks if Redis is unavailable

This ensures a consistent event processing pipeline through Redis while still providing fallback capabilities.

## Implementation Details

The feature works by checking three conditions before invoking direct callbacks:

1. Is there an event handler mode manager?
2. Is the current mode set to `REDIS`?
3. Is `exclusive_callback_via_redis` enabled in the Redis config?

If all three conditions are met, the event will only be published to Redis and the direct callback will be skipped.

## Use Cases

- Distributed systems with multiple consumers of events
- Systems requiring reliable event delivery with persistent storage
- Applications with complex event processing pipelines
- High-throughput scenarios where Redis provides better scalability
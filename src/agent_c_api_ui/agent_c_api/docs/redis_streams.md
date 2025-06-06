# Redis Streams for Event Tracking

## Overview

Agent C uses Redis Streams to track events in a distributed environment. This provides several benefits:

1. **Persistence**: Events are stored persistently in Redis, surviving application restarts
2. **Distributed Event Tracking**: Multiple application instances can track and process events
3. **Time-Ordered Events**: Events are stored in time order with unique IDs
4. **Efficient Storage**: Redis Streams are memory-efficient and support trimming

## Configuration

The following settings in `env_config.py` control Redis Stream behavior:

```python
# Redis Stream Configuration
STREAM_PREFIX: str = "agent_c:stream:"  # Key prefix for stream keys
STREAM_MAX_LENGTH: int = 10000         # Maximum number of events in a stream
STREAM_TRIM_INTERVAL: int = 100        # How often to trim streams (event count)
STREAM_RETENTION_PERIOD: int = 7 * 24 * 60 * 60  # 7 days in seconds
```

## Stream Key Format

Stream keys follow the format: `agent_c:stream:{session_id}`

## Stream Event Format

Each event in a stream is stored with:

1. **Stream ID**: A unique, time-ordered ID in the format `timestamp-uuid`
2. **Event Data**: The serialized event data as field-value pairs

## Stream Management

The RedisSessionManager provides methods for:

- `add_event_to_stream`: Adding events to a session's stream
- `get_events_from_stream`: Retrieving events from a stream
- `trim_stream`: Trimming a stream to a maximum length

Streams are automatically trimmed:

1. After every `STREAM_TRIM_INTERVAL` events
2. When sessions expire (streams for expired sessions are deleted)

## Usage Example

```python
# Add an event to a stream
stream_id = await redis_session_manager.add_event_to_stream(
    session_id="1234",
    event_data={"type": "message", "content": "Hello, world!"}
)

# Get events from a stream
events = await redis_session_manager.get_events_from_stream(
    session_id="1234",
    start="0",       # Start from beginning
    end="+",        # Up to latest event
    count=100       # Maximum events to retrieve
)

# Process events
for event in events:
    print(f"Event ID: {event['id']}")
    print(f"Event Data: {event['data']}")
```

## Performance Considerations

1. **Stream Size**: Adjust `STREAM_MAX_LENGTH` based on expected event volume and memory constraints
2. **Trim Frequency**: Adjust `STREAM_TRIM_INTERVAL` to balance between frequent trimming and memory usage
3. **Retention Period**: Consider your compliance and data retention requirements when setting `STREAM_RETENTION_PERIOD`
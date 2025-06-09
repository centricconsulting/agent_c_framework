# Agent C API Service Layer Design with Redis Streams

## Introduction

This document details the design of the Agent C API service layer with the integration of Redis Streams for event handling. The design aims to replace the current async queue-based event handling system with a more scalable, distributed approach using Redis Streams.

## System Architecture

### Current Architecture

```
+-------------+    +---------------+    +--------------+    +--------+
|  BaseAgent  | -> |  Async Queue  | -> |  AgentBridge | -> | Client |
+-------------+    +---------------+    +--------------+    +--------+
      |                                        |
      v                                        v
+-------------+                        +--------------+
| Event Logger|                        | Redis Session|
+-------------+                        +--------------+
```

### Proposed Architecture with Redis Streams

```
+-------------+    +---------------+    +--------------+    +--------+
|  BaseAgent  | -> | Redis Streams | -> |  AgentBridge | -> | Client |
+-------------+    +---------------+    +--------------+    +--------+
      |                   ^                    |
      v                   |                    v
+-------------+    +-----------------+  +--------------+
| Event Logger| -> | RedisStreamMgr |  | Redis Session|
+-------------+    +-----------------+  +--------------+
```

## Component Designs

### 1. RedisStreamManager

The RedisStreamManager is a new class responsible for all Redis Stream operations:

```python
class RedisStreamManager:
    def __init__(self, config=None):
        # Initialize Redis connection and configuration
        pass
        
    def get_stream_key(self, session_id, interaction_id):
        # Generate the stream key using the naming convention
        return f"agent_c:stream:{session_id}:{interaction_id}"
        
    async def publish_event(self, event_type, event_data, session_id, interaction_id, sequence=None):
        # Publish an event to the Redis Stream
        pass
        
    async def consume_events(self, session_id, interaction_id, last_id='0-0', block=None):
        # Consume events from the Redis Stream as an async generator
        pass
        
    # Other stream management methods
    async def create_stream(self, session_id, interaction_id):
        pass
        
    async def delete_stream(self, session_id, interaction_id=None):
        pass
        
    async def list_streams(self, pattern='agent_c:stream:*'):
        pass
        
    async def cleanup_old_streams(self, max_age_seconds=3600):
        pass
        
    async def close(self):
        pass
```

### 2. BaseAgent Updates

The BaseAgent class needs updates to its event raising methods:

```python
async def _raise_event(self, event, streaming_callback=None, stream_id=None):
    # Updated to support Redis Stream IDs
    # If stream_id is provided, attach it to the event
    pass
    
# All other _raise_* methods need updates to pass stream_id
async def _raise_text_delta(self, content, stream_id=None, **data):
    # Pass stream_id to _raise_event
    pass
```

### 3. AgentBridge Updates

The AgentBridge class needs significant updates:

```python
def __init__(self, ...):
    # Add Redis Stream Manager initialization
    self.use_redis_streams = os.getenv('USE_REDIS_STREAMS', 'false').lower() == 'true'
    self.redis_stream_manager = None
    if self.use_redis_streams:
        self.redis_stream_manager = RedisStreamManager()
    
async def consolidated_streaming_callback(self, event, stream_id=None):
    # Updated to support Redis Streams
    # If stream_id is provided, publish to Redis Stream
    pass
    
async def stream_chat(self, user_message, client_wants_cancel, file_ids=None):
    # Updated to consume from Redis Streams if enabled
    pass
```

## Data Flow

### Event Publication Flow

1. BaseAgent generates an event (e.g., text_delta, tool_call)
2. BaseAgent._raise_event is called with the event and stream_id
3. _raise_event passes the event to the streaming_callback with stream_id
4. consolidated_streaming_callback processes the event
5. If Redis Streams is enabled and stream_id is provided:
   - The appropriate handler processes the event into a JSON payload
   - The payload is published to the Redis Stream using RedisStreamManager
6. Otherwise, the payload is put into the async queue

### Event Consumption Flow

1. stream_chat is called with a user message
2. If Redis Streams is enabled:
   - A unique interaction_id is generated
   - A new Redis Stream is created for the interaction
   - The chat task is started with the interaction_id
   - Events are consumed from the Redis Stream
   - JSON payloads are yielded to the client
3. Otherwise, the async queue is used

## Stream Naming and Message Format

### Stream Naming Convention

```
agent_c:stream:{session_id}:{interaction_id}
```

This convention allows:
- Easy identification of Agent C streams
- Isolation between different sessions
- Separation of interactions within a session

### Message Format

```python
{
    'event_type': str,  # Type of event (text_delta, tool_call, etc.)
    'event_data': str,  # JSON-serialized event data
    'timestamp': str,   # ISO format timestamp
    'session_id': str,  # Session identifier
    'interaction_id': str,  # Interaction identifier
    'sequence': int,    # Sequence number for ordering
}
```

## Configuration

```python
# Feature flag to enable/disable Redis Streams
USE_REDIS_STREAMS = os.getenv('USE_REDIS_STREAMS', 'false').lower() == 'true'

# Redis connection settings
REDIS_HOST = os.getenv('REDIS_HOST', 'localhost')
REDIS_PORT = int(os.getenv('REDIS_PORT', 6379))
REDIS_PASSWORD = os.getenv('REDIS_PASSWORD', None)
REDIS_DB = int(os.getenv('REDIS_DB', 0))
REDIS_SSL = os.getenv('REDIS_SSL', 'false').lower() == 'true'

# Redis connection pool settings
REDIS_MAX_CONNECTIONS = int(os.getenv('REDIS_MAX_CONNECTIONS', 10))
REDIS_SOCKET_TIMEOUT = int(os.getenv('REDIS_SOCKET_TIMEOUT', 5))
REDIS_SOCKET_CONNECT_TIMEOUT = int(os.getenv('REDIS_SOCKET_CONNECT_TIMEOUT', 5))

# Redis Streams specific settings
REDIS_STREAM_MAX_LEN = int(os.getenv('REDIS_STREAM_MAX_LEN', 1000))
REDIS_STREAM_TTL = int(os.getenv('REDIS_STREAM_TTL', 3600))
REDIS_STREAM_READ_TIMEOUT = int(os.getenv('REDIS_STREAM_READ_TIMEOUT', 1000))
```

## Error Handling Strategy

1. **Connection Errors**:
   - Implement exponential backoff retry for connection issues
   - Log connection errors with appropriate severity
   - Provide fallback to in-memory queue if Redis is unavailable

2. **Stream Operations Errors**:
   - Handle stream creation/deletion errors gracefully
   - Implement retry logic for publish operations
   - Log detailed error information for troubleshooting

3. **Consumer Errors**:
   - Handle consumer errors without crashing the application
   - Provide options to skip problematic messages
   - Implement dead-letter pattern for unprocessable messages

## Performance Considerations

1. **Connection Pooling**:
   - Use connection pooling to avoid connection overhead
   - Configure appropriate pool sizes based on expected load

2. **Message Size**:
   - Monitor message sizes to avoid performance issues
   - Consider compression for large messages

3. **Stream Length**:
   - Use MAXLEN to limit stream size and memory usage
   - Implement periodic cleanup of old streams

4. **Read Efficiency**:
   - Use appropriate block times for reads
   - Batch reads where possible

## Migration Strategy

1. **Phased Approach**:
   - Implement Redis Streams alongside async queue system
   - Use feature flag to control which system is active
   - Gradually roll out to production

2. **Testing Strategy**:
   - Unit tests for RedisStreamManager
   - Integration tests for the complete event flow
   - Performance comparison tests
   - Canary testing in production

## Future Enhancements

1. **Consumer Groups**:
   - Implement Redis Stream consumer groups for parallel processing
   - Enable load balancing across multiple workers

2. **Stream Compression**:
   - Add compression for large message payloads

3. **Persistence Policies**:
   - Fine-grained control over stream retention
   - Different TTLs for different event types

4. **Monitoring and Metrics**:
   - Stream size and throughput metrics
   - Error rate tracking
   - Processing latency monitoring
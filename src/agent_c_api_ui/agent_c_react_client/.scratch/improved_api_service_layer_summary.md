# Redis Streams Integration for Agent C Event System

## Executive Summary

We will replace the current async queue-based event handling system in Agent C with Redis Streams to enable distributed event processing, improving scalability and resilience. This migration will maintain backward compatibility while enabling more robust event handling across multiple processes or servers.

## Current System

The current event system uses:
1. `BaseAgent._raise_event` methods to create events
2. `AgentBridge.consolidated_streaming_callback` to process events
3. Async queues to transfer events between components
4. `AgentBridge.stream_chat` to yield events to the client

This approach works well for single-process deployments but has limitations for distributed systems.

## Proposed Solution

Integrate Redis Streams as follows:

1. **Create RedisStreamManager**: A new class to handle all Redis Stream operations
2. **Update BaseAgent**: Modify `_raise_event` to accept stream IDs and publish to Redis
3. **Update AgentBridge**: Adapt `consolidated_streaming_callback` and `stream_chat` to work with Redis Streams
4. **Add Feature Flag**: `USE_REDIS_STREAMS` to control which system is active

## Implementation Plan

### Phase 1: Analysis and Design
- Analyze current event flow and identify needed changes
- Design Redis Stream integration architecture
- Define configuration requirements

### Phase 2: Core Implementation
- Create RedisStreamManager class
- Update BaseAgent._raise_event method
- Update all _raise_* methods in BaseAgent
- Update AgentBridge.consolidated_streaming_callback
- Update AgentBridge.stream_chat method

### Phase 3: Testing and Validation
- Create unit tests for RedisStreamManager
- Create integration tests for event flow
- Create performance tests

### Phase 4: Documentation and Deployment
- Update technical documentation
- Create deployment guide
- Create monitoring and maintenance plan

## Key Technical Details

### Stream Naming Convention
```
agent_c:stream:{session_id}:{interaction_id}
```

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

### Configuration Parameters
```python
# Redis connection settings
REDIS_HOST = os.getenv('REDIS_HOST', 'localhost')
REDIS_PORT = int(os.getenv('REDIS_PORT', 6379))
REDIS_PASSWORD = os.getenv('REDIS_PASSWORD', None)
REDIS_DB = int(os.getenv('REDIS_DB', 0))
REDIS_SSL = os.getenv('REDIS_SSL', 'false').lower() == 'true'

# Redis connection pool settings
REDIS_MAX_CONNECTIONS = int(os.getenv('REDIS_MAX_CONNECTIONS', 10))

# Redis Streams specific settings
REDIS_STREAM_MAX_LEN = int(os.getenv('REDIS_STREAM_MAX_LEN', 1000))
REDIS_STREAM_TTL = int(os.getenv('REDIS_STREAM_TTL', 3600))
```

## Benefits

1. **Scalability**: Handle more concurrent sessions across multiple processes
2. **Resilience**: Survive process restarts without losing in-flight events
3. **Observability**: Better monitoring and debugging of event flow
4. **Distribution**: Enable distributed event processing across servers

## Risks and Mitigations

| Risk | Mitigation |
|------|------------|
| Redis availability issues | Fallback to async queues if Redis is unavailable |
| Performance degradation | Comprehensive performance testing before deployment |
| Data loss during transition | Dual-write approach during initial rollout |
| Increased complexity | Thorough documentation and monitoring |

## Timeline

- **Week 1**: Implementation and unit testing
- **Week 2**: Integration testing and performance optimization
- **Week 3**: Canary deployment and monitoring
- **Week 4**: Full deployment and cleanup

## Conclusion

Migrating to Redis Streams will significantly improve the scalability and resilience of the Agent C event system while maintaining compatibility with existing code. The phased approach with feature flags ensures minimal disruption and easy rollback if issues arise.
# Redis Streams Migration Implementation Strategy

## Overview

This document outlines the implementation strategy for migrating the Agent C event system from using asyncio queues to Redis Streams. This migration will enable more scalable, distributed event processing and improve overall system resilience.

## Migration Approach

We will use a phased approach with feature flags to minimize disruption and allow for easy rollback if issues arise:

### Phase 1: Implementation with Feature Flag

1. **Dual Mode Support**: Implement the Redis Streams functionality alongside the existing asyncio queue system
2. **Feature Flag Control**: Add a configuration flag `USE_REDIS_STREAMS` to control which system is active
3. **Fallback Mechanism**: Ensure the system can gracefully fall back to asyncio queues if Redis is unavailable

### Phase 2: Testing and Validation

1. **Development Testing**: Enable Redis Streams in development environment
2. **Staging Deployment**: Deploy to staging with feature flag enabled
3. **Performance Testing**: Run comprehensive performance tests
4. **Canary Testing**: Enable for a small percentage of production traffic

### Phase 3: Full Deployment

1. **Gradual Rollout**: Increase percentage of production traffic using Redis Streams
2. **Monitoring**: Closely monitor performance and errors
3. **Full Deployment**: Enable Redis Streams for all traffic

### Phase 4: Cleanup

1. **Remove Asyncio Queue Code**: Once Redis Streams is stable, remove the asyncio queue implementation
2. **Optimize**: Further optimize the Redis Streams implementation based on production data

## Key Code Changes

### 1. BaseAgent Class Updates

```python
# In base.py

async def _raise_event(self, event, streaming_callback=None, stream_id=None):
    """Raise a chat event to the event stream.
    
    Events are sent to the streaming_callback if configured. For event logging,
    use EventSessionLogger as your streaming_callback.
    
    Args:
        event: The event to raise
        streaming_callback: Optional callback to handle the event
        stream_id: Optional Redis stream ID for distributed event tracking
    """
    if streaming_callback is None:
        streaming_callback = self.streaming_callback

    # Attach stream_id to event if provided
    if stream_id is not None and hasattr(event, 'set_stream_id'):
        try:
            event.set_stream_id(stream_id)
        except Exception as e:
            self.logger.warning(f"Failed to set stream_id on event: {e}")

    if streaming_callback is not None:
        try:
            await streaming_callback(event, stream_id)
        except Exception as e:
            self.logger.exception(
                f"Streaming callback error for event: {e}. Event Type: {getattr(event, 'type', 'unknown')}"
            )
            # Log internal error as system event
            await self._raise_system_event(
                f"Streaming callback error: {str(e)}",
                severity="error",
                error_type="streaming_callback_error",
                original_event_type=getattr(event, 'type', 'unknown')
            )
```

### 2. RedisStreamManager Class

```python
from redis.asyncio import Redis
import json
from datetime import datetime, timedelta
from typing import Dict, Any, AsyncGenerator, Optional, List, Tuple
import uuid

class RedisStreamManager:
    """Manager for Redis Streams used in Agent C event processing."""
    
    def __init__(self, config=None):
        """Initialize the Redis Stream Manager with configuration."""
        self.config = config or {}
        self.redis = Redis(
            host=self.config.get('REDIS_HOST', 'localhost'),
            port=self.config.get('REDIS_PORT', 6379),
            password=self.config.get('REDIS_PASSWORD', None),
            db=self.config.get('REDIS_DB', 0),
            ssl=self.config.get('REDIS_SSL', False),
            socket_timeout=self.config.get('REDIS_SOCKET_TIMEOUT', 5),
            socket_connect_timeout=self.config.get('REDIS_SOCKET_CONNECT_TIMEOUT', 5),
            max_connections=self.config.get('REDIS_MAX_CONNECTIONS', 10)
        )
        self.stream_max_len = self.config.get('REDIS_STREAM_MAX_LEN', 1000)
        self.stream_ttl = self.config.get('REDIS_STREAM_TTL', 3600)
        self.read_timeout = self.config.get('REDIS_STREAM_READ_TIMEOUT', 1000)
    
    def get_stream_key(self, session_id: str, interaction_id: str) -> str:
        """Get the stream key for a session and interaction."""
        return f"agent_c:stream:{session_id}:{interaction_id}"
    
    async def publish_event(self, event_type: str, event_data: Any, session_id: str, 
                          interaction_id: str, sequence: Optional[int] = None) -> str:
        """Publish an event to a Redis Stream."""
        stream_key = self.get_stream_key(session_id, interaction_id)
        timestamp = datetime.now().isoformat()
        
        # Determine sequence number if not provided
        if sequence is None:
            sequence = await self._get_next_sequence(stream_key)
        
        # Serialize event data if it's not a string
        serialized_data = event_data
        if not isinstance(event_data, str):
            serialized_data = json.dumps(event_data)
        
        # Create fields for the stream entry
        fields = {
            'event_type': event_type,
            'event_data': serialized_data,
            'timestamp': timestamp,
            'session_id': session_id,
            'interaction_id': interaction_id,
            'sequence': str(sequence)
        }
        
        # Flatten fields for xadd
        flat_fields = [item for pair in fields.items() for item in pair]
        
        try:
            # Add to stream with automatic trimming
            message_id = await self.redis.xadd(
                stream_key,
                {'MAXLEN': '~', self.stream_max_len},
                '*',  # Auto-generate ID
                *flat_fields
            )
            
            # Set TTL on the stream
            await self.redis.expire(stream_key, self.stream_ttl)
            
            return message_id
        except Exception as e:
            import logging
            logging.error(f"Error publishing to Redis Stream: {e}")
            raise
    
    async def _get_next_sequence(self, stream_key: str) -> int:
        """Get the next sequence number for a stream."""
        try:
            # Get the last entry in the stream
            last_entries = await self.redis.xrevrange(stream_key, '+', '-', count=1)
            
            if last_entries and len(last_entries) > 0:
                # Extract the sequence number from the last entry
                last_entry = last_entries[0]
                fields = last_entry[1]
                
                if 'sequence' in fields:
                    return int(fields['sequence']) + 1
            
            # If no entries or no sequence, start at 1
            return 1
        except Exception:
            # If anything goes wrong, start at 1
            return 1
    
    async def consume_events(self, session_id: str, interaction_id: str, 
                           last_id: str = '0-0', block: Optional[int] = None) -> AsyncGenerator[Dict[str, Any], None]:
        """Consume events from a Redis Stream as an async generator."""
        stream_key = self.get_stream_key(session_id, interaction_id)
        block_ms = block if block is not None else self.read_timeout
        current_id = last_id
        
        try:
            while True:
                # Read from the stream with blocking
                streams = await self.redis.xread(
                    streams={stream_key: current_id},
                    count=10,  # Get up to 10 messages at a time
                    block=block_ms
                )
                
                # If no data and non-blocking read, we're done
                if not streams:
                    if block_ms == 0:
                        break
                    else:
                        continue  # For blocking reads, continue waiting
                
                # Process the messages
                for stream_name, messages in streams:
                    for message_id, fields in messages:
                        current_id = message_id  # Update ID for next read
                        
                        # Process fields
                        event = {
                            'id': message_id,
                            'event_type': fields.get('event_type', 'unknown'),
                            'timestamp': fields.get('timestamp'),
                            'session_id': fields.get('session_id'),
                            'interaction_id': fields.get('interaction_id'),
                            'sequence': int(fields.get('sequence', '0'))
                        }
                        
                        # Parse event_data if it's JSON
                        event_data = fields.get('event_data')
                        if event_data:
                            try:
                                event['event_data'] = json.loads(event_data)
                            except json.JSONDecodeError:
                                event['event_data'] = event_data
                        
                        yield event
        except asyncio.CancelledError:
            # Allow cancellation to propagate
            raise
        except Exception as e:
            import logging
            logging.error(f"Error consuming from Redis Stream: {e}")
            raise
    
    async def create_stream(self, session_id: str, interaction_id: str) -> str:
        """Create a new stream for a session/interaction."""
        stream_key = self.get_stream_key(session_id, interaction_id)
        
        # Add a marker entry to create the stream
        await self.redis.xadd(
            stream_key,
            {'MAXLEN': '~', self.stream_max_len},
            '*',
            'event_type', 'stream_created',
            'timestamp', datetime.now().isoformat(),
            'session_id', session_id,
            'interaction_id', interaction_id,
            'sequence', '0'
        )
        
        # Set TTL on the stream
        await self.redis.expire(stream_key, self.stream_ttl)
        
        return stream_key
    
    async def delete_stream(self, session_id: str, interaction_id: Optional[str] = None) -> int:
        """Delete a stream or streams for a session/interaction."""
        if interaction_id:
            # Delete specific stream
            stream_key = self.get_stream_key(session_id, interaction_id)
            return await self.redis.delete(stream_key)
        else:
            # Delete all streams for a session
            pattern = f"agent_c:stream:{session_id}:*"
            keys = await self.redis.keys(pattern)
            
            if keys:
                return await self.redis.delete(*keys)
            return 0
    
    async def list_streams(self, pattern: str = 'agent_c:stream:*') -> List[str]:
        """List all streams matching the pattern."""
        return await self.redis.keys(pattern)
    
    async def cleanup_old_streams(self, max_age_seconds: Optional[int] = None) -> int:
        """Clean up streams older than the specified age."""
        ttl = max_age_seconds or self.stream_ttl
        cutoff_time = datetime.now() - timedelta(seconds=ttl)
        cutoff_str = cutoff_time.isoformat()
        
        # Get all streams
        streams = await self.list_streams()
        deleted = 0
        
        for stream_key in streams:
            # Get the first message to check timestamp
            first_messages = await self.redis.xrange(stream_key, '-', '+', count=1)
            
            if first_messages and len(first_messages) > 0:
                fields = first_messages[0][1]
                
                if 'timestamp' in fields:
                    timestamp = fields['timestamp']
                    
                    # If older than cutoff, delete the stream
                    if timestamp < cutoff_str:
                        await self.redis.delete(stream_key)
                        deleted += 1
        
        return deleted
    
    async def close(self):
        """Close the Redis connection."""
        await self.redis.close()
```

### 3. AgentBridge Updates

```python
# In agent_bridge.py

def __init__(self, ...):
    # Add Redis Stream Manager initialization
    self.use_redis_streams = os.getenv('USE_REDIS_STREAMS', 'false').lower() == 'true'
    self.redis_stream_manager = None
    if self.use_redis_streams:
        from agent_c_api.core.redis_stream_manager import RedisStreamManager
        self.redis_stream_manager = RedisStreamManager()


async def consolidated_streaming_callback(self, event, stream_id=None):
    """Process and route various types of session events through appropriate handlers.

    This method serves as the central event processing hub, handling various event
    types including text updates, tool calls, media rendering, and completion status.
    It formats the events into JSON payloads suitable for streaming to the client.

    Args:
        event: The session event to process, containing type and payload information.
        stream_id: Optional Redis Stream ID for distributed event processing.
    """
    # A simple dispatch dictionary that maps event types to handler methods.
    handlers = {
        "text_delta": self._handle_text_delta,
        "tool_call": self._handle_tool_call,
        # ... other handlers ...
    }

    handler = handlers.get(event.type)
    if handler:
        try:
            payload = await handler(event)
            if payload is not None:
                if self.use_redis_streams and self.redis_stream_manager and stream_id:
                    # Get session_id and interaction_id from event
                    session_id = getattr(event, 'session_id', 'unknown')
                    interaction_id = stream_id
                    
                    # Publish to Redis Stream
                    await self.redis_stream_manager.publish_event(
                        event.type, payload, session_id, interaction_id
                    )
                elif hasattr(self, "_stream_queue"):
                    # Legacy queue-based approach
                    await self._stream_queue.put(payload)
                    
                    # If this is the end-of-stream event, push termination markers
                    if event.type == "interaction" and not event.started:
                        await self._stream_queue.put(payload)
                        await self._stream_queue.put(None)

        except Exception as e:
            self.logger.error(f"Error in event handler {handler.__name__} for {event.type}: {str(e)}")
    else:
        self.logger.warning(f"Unhandled event type: {event.type}")


async def stream_chat(self, user_message, client_wants_cancel, file_ids=None):
    """Streams chat responses for a given user message."""
    # Generate a unique interaction ID
    interaction_id = str(uuid.uuid4())
    
    if self.use_redis_streams and self.redis_stream_manager:
        # Redis Streams approach
        try:
            # Create a new stream for this interaction
            session_id = self.chat_session.session_id
            await self.redis_stream_manager.create_stream(session_id, interaction_id)
            
            # Update session and prepare chat parameters
            await self.session_manager.update()
            
            # Process files and prepare chat parameters as before
            # ... [existing file processing code] ...
            
            # Add Redis Stream ID to chat parameters
            chat_params['redis_stream_id'] = interaction_id
            
            # Start the chat task without a queue
            chat_params.pop('streaming_queue', None)  # Remove queue parameter if present
            chat_task = asyncio.create_task(self.agent_runtime.chat(**chat_params))
            
            # Start consuming from the stream
            try:
                async for event in self.redis_stream_manager.consume_events(
                    session_id, interaction_id, block=0
                ):
                    # Extract and yield the payload
                    payload = event.get('event_data')
                    if isinstance(payload, str):
                        yield payload
                    
                    # Check for end of interaction
                    if event.get('event_type') == 'interaction' and 'interaction_end' in str(payload):
                        break
            except asyncio.CancelledError:
                # Handle cancellation
                self.logger.info("Stream consumption cancelled")
            
            # Wait for chat task to complete
            await chat_task
            
            # Cleanup
            await self.session_manager.flush(self.chat_session.session_id)
            
            # Schedule stream deletion after a delay
            asyncio.create_task(self._delayed_stream_cleanup(session_id, interaction_id))
            
        except Exception as e:
            self.logger.exception(f"Error in stream_chat with Redis: {e}")
            yield json.dumps({
                "type": "error",
                "data": f"Error in stream_chat: {str(e)}\n{traceback.format_exc()}"
            }) + "\n"
    else:
        # Legacy queue-based approach
        await self.reset_streaming_state()
        queue = asyncio.Queue()
        self._stream_queue = queue
        
        # ... [existing queue-based implementation] ...


async def _delayed_stream_cleanup(self, session_id, interaction_id, delay_seconds=30):
    """Delete a Redis Stream after a delay to ensure all events are processed."""
    await asyncio.sleep(delay_seconds)
    try:
        await self.redis_stream_manager.delete_stream(session_id, interaction_id)
    except Exception as e:
        self.logger.error(f"Error cleaning up Redis Stream: {e}")
```

## Configuration Updates

Add the following to environment configuration:

```python
# Redis Streams configuration
USE_REDIS_STREAMS = os.getenv('USE_REDIS_STREAMS', 'false').lower() == 'true'
REDIS_HOST = os.getenv('REDIS_HOST', 'localhost')
REDIS_PORT = int(os.getenv('REDIS_PORT', 6379))
REDIS_PASSWORD = os.getenv('REDIS_PASSWORD', None)
REDIS_DB = int(os.getenv('REDIS_DB', 0))
REDIS_SSL = os.getenv('REDIS_SSL', 'false').lower() == 'true'
REDIS_MAX_CONNECTIONS = int(os.getenv('REDIS_MAX_CONNECTIONS', 10))
REDIS_SOCKET_TIMEOUT = int(os.getenv('REDIS_SOCKET_TIMEOUT', 5))
REDIS_SOCKET_CONNECT_TIMEOUT = int(os.getenv('REDIS_SOCKET_CONNECT_TIMEOUT', 5))
REDIS_STREAM_MAX_LEN = int(os.getenv('REDIS_STREAM_MAX_LEN', 1000))
REDIS_STREAM_TTL = int(os.getenv('REDIS_STREAM_TTL', 3600))
REDIS_STREAM_READ_TIMEOUT = int(os.getenv('REDIS_STREAM_READ_TIMEOUT', 1000))
```

## Deployment Considerations

### Redis Infrastructure

1. **Development**: Use Redis Docker container for local development
2. **Testing**: Dedicated Redis instance for testing
3. **Production**: Redis with replication and persistence enabled
   - Consider Redis Sentinel or Redis Cluster for high availability
   - Enable AOF persistence for durability
   - Implement monitoring and alerting

### Performance Tuning

1. **Stream Size**: Adjust `REDIS_STREAM_MAX_LEN` based on memory constraints
2. **Connection Pool**: Tune `REDIS_MAX_CONNECTIONS` based on concurrent sessions
3. **TTL**: Adjust `REDIS_STREAM_TTL` based on session duration and cleanup needs

### Monitoring

1. **Redis Metrics**: Monitor Redis memory usage, connections, and commands
2. **Stream Metrics**: Track stream counts, sizes, and processing times
3. **Error Rates**: Monitor errors in stream operations

## Rollback Plan

If issues arise with the Redis Streams implementation:

1. Set `USE_REDIS_STREAMS=false` to revert to asyncio queues
2. Restart services to apply the change
3. No data migration is needed as streams are ephemeral

## Timeline

1. **Week 1**: Implementation and unit testing
2. **Week 2**: Integration testing and performance optimization
3. **Week 3**: Canary deployment and monitoring
4. **Week 4**: Full deployment and cleanup
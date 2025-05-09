# Session Repository Design

## Overview
The SessionRepository class will handle all Redis operations for session management, providing a clean interface for storing and retrieving session data.

## Redis Data Structures

### Session Data
- Key: `session:{session_id}:data`
- Type: Hash
- Contents: Core session data (model, persona, tools, etc.)
```python
{
    "model_id": "gpt-4",
    "persona_id": "programmer",
    "name": "Code Review Session",
    "temperature": "0.7",
    "reasoning_effort": "5",
    "max_tokens": "2000",
    "tools": "[\"search\", \"code_analysis\"]",
    "custom_prompt": "...",
    "agent_internal_id": "agent-54321"
}
```

### Session Metadata
- Key: `session:{session_id}:meta`
- Type: Hash
- Contents: Timestamps, status, etc.
```python
{
    "created_at": "2025-04-01T14:30:00Z",
    "updated_at": "2025-04-01T16:45:00Z",
    "last_activity": "2025-04-01T16:45:00Z",
    "is_active": "true",
    "status": "running"
}
```

### Active Sessions
- Key: `active_sessions`
- Type: Set
- Contents: Set of active session IDs

### Session Expiry
- Key: `session_expiry:{session_id}`
- Type: String
- Contents: Empty string with TTL
- Purpose: Automatic session cleanup

## Class Structure

```python
class SessionRepository:
    def __init__(self, redis_client: aioredis.Redis):
        self.redis = redis_client
        
    async def create_session(self, session_data: SessionCreate) -> SessionDetail:
        """Create a new session"""
        
    async def get_session(self, session_id: str) -> Optional[SessionDetail]:
        """Get session details"""
        
    async def update_session(self, session_id: str, update_data: SessionUpdate) -> SessionDetail:
        """Update session properties"""
        
    async def delete_session(self, session_id: str) -> bool:
        """Delete a session"""
        
    async def list_sessions(self, limit: int = 10, offset: int = 0) -> SessionListResponse:
        """List active sessions with pagination"""
        
    async def extend_session_ttl(self, session_id: str) -> bool:
        """Extend session TTL"""
        
    async def cleanup_expired_sessions(self) -> int:
        """Clean up expired sessions"""
```

## Implementation Details

### Session Creation
1. Generate UUID for session
2. Store core data in session:{id}:data hash
3. Store metadata in session:{id}:meta hash
4. Add to active_sessions set
5. Set expiry key with TTL

### Session Retrieval
1. Check if session exists
2. Get data from both hashes
3. Combine and return SessionDetail

### Session Update
1. Validate session exists
2. Update relevant hash fields
3. Update metadata (updated_at)
4. Extend TTL

### Session Deletion
1. Remove from active_sessions
2. Delete all session keys
3. Clean up related data

### Session Listing
1. Get IDs from active_sessions
2. Apply pagination
3. Fetch details for each ID
4. Return paginated response

### TTL Management
1. Set initial TTL on creation
2. Extend TTL on activity
3. Cleanup expired sessions

## Integration with Existing Code

### UItoAgentBridgeManager Changes
1. Add SessionRepository as dependency
2. Update session operations to use repository
3. Handle Redis connection management

### API Layer Changes
1. Update SessionService to use repository
2. Add Redis health checks
3. Handle Redis errors

## Error Handling
1. Connection errors
2. Invalid session IDs
3. Concurrent modifications
4. Data consistency issues

## Monitoring & Maintenance
1. Session count metrics
2. TTL monitoring
3. Cleanup job scheduling
4. Error tracking

## Migration Plan
1. Create new repository
2. Add Redis integration
3. Migrate existing sessions
4. Switch to Redis backend
5. Clean up old storage
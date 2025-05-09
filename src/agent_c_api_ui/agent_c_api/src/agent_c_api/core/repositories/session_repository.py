from datetime import datetime
import json
from typing import Dict, List, Any, Optional, Union
from uuid import UUID

from redis import asyncio as aioredis
import structlog

from agent_c_api.api.v2.models.session_models import (
    SessionCreate,
    SessionDetail,
    SessionListResponse,
    SessionSummary,
    SessionUpdate
)

class SessionRepository:
    """Repository for managing sessions in Redis"""
    
    def __init__(self, redis_client: aioredis.Redis):
        """Initialize the session repository
        
        Args:
            redis_client: Redis client instance
        """
        self.redis = redis_client
        self.logger = structlog.get_logger(__name__)
        self.session_ttl = 24 * 60 * 60  # 24 hours default TTL
    
    async def _serialize_value(self, value: Any) -> str:
        """Serialize a value for Redis storage
        
        Args:
            value: Value to serialize
            
        Returns:
            Serialized string value
        """
        if isinstance(value, str):
            return value
        return json.dumps(value)
    
    async def _deserialize_value(self, value: Optional[str], default_type: Any = None) -> Any:
        """Deserialize a value from Redis storage
        
        Args:
            value: Value to deserialize
            default_type: Expected type for the value
            
        Returns:
            Deserialized value
        """
        if value is None:
            return None
            
        # Handle bytes
        if isinstance(value, bytes):
            value = value.decode('utf-8')
            
        # Try JSON deserialization
        try:
            return json.loads(value)
        except json.JSONDecodeError:
            # Return as is if not JSON
            return value
    
    async def _set_session_ttl(self, session_id: str) -> None:
        """Set or refresh session TTL
        
        Args:
            session_id: Session ID
        """
        pipe = self.redis.pipeline()
        
        # Set TTL for all session keys
        for key in [
            f"session:{session_id}:data",
            f"session:{session_id}:meta"
        ]:
            pipe.expire(key, self.session_ttl)
            
        await pipe.execute()
    
    async def create_session(self, session_data: SessionCreate) -> SessionDetail:
        """Create a new session
        
        Args:
            session_data: Session creation parameters
            
        Returns:
            Created session details
            
        Raises:
            Exception: If session creation fails
        """
        try:
            # Generate session ID if not provided
            session_id = str(UUID(session_data.id)) if session_data.id else str(UUID())
            
            # Get current timestamp
            timestamp = datetime.now().isoformat()
            
            # Prepare session data
            core_data = {
                "model_id": session_data.model_id,
                "persona_id": session_data.persona_id or "default",
                "temperature": await self._serialize_value(session_data.temperature),
                "reasoning_effort": await self._serialize_value(session_data.reasoning_effort),
                "budget_tokens": await self._serialize_value(session_data.budget_tokens),
                "max_tokens": await self._serialize_value(session_data.max_tokens),
                "tools": await self._serialize_value(session_data.tools),
                "custom_prompt": session_data.custom_prompt
            }
            
            # Filter out None values
            core_data = {k: v for k, v in core_data.items() if v is not None}
            
            # Prepare metadata
            meta_data = {
                "created_at": timestamp,
                "updated_at": timestamp,
                "last_activity": timestamp,
                "is_active": "true"
            }
            
            # Create Redis pipeline
            pipe = self.redis.pipeline()
            
            # Store session data
            pipe.hset(f"session:{session_id}:data", mapping=core_data)
            pipe.hset(f"session:{session_id}:meta", mapping=meta_data)
            
            # Add to active sessions
            pipe.sadd("active_sessions", session_id)
            
            # Execute pipeline
            await pipe.execute()
            
            # Set TTL
            await self._set_session_ttl(session_id)
            
            # Return session details
            return await self.get_session(session_id)
            
        except Exception as e:
            self.logger.error("create_session_failed", error=str(e))
            raise
    
    async def get_session(self, session_id: str) -> Optional[SessionDetail]:
        """Get session details
        
        Args:
            session_id: Session ID
            
        Returns:
            Session details if found, None otherwise
        """
        try:
            # Check if session exists
            if not await self.redis.exists(f"session:{session_id}:data"):
                return None
                
            # Get session data and metadata
            core_data = await self.redis.hgetall(f"session:{session_id}:data")
            meta_data = await self.redis.hgetall(f"session:{session_id}:meta")
            
            # Deserialize core data
            deserialized_data = {}
            for k, v in core_data.items():
                key = k.decode('utf-8') if isinstance(k, bytes) else k
                deserialized_data[key] = await self._deserialize_value(v)
                
            # Deserialize metadata
            deserialized_meta = {}
            for k, v in meta_data.items():
                key = k.decode('utf-8') if isinstance(k, bytes) else k
                deserialized_meta[key] = await self._deserialize_value(v)
            
            # Refresh TTL on access
            await self._set_session_ttl(session_id)
            
            # Construct session detail
            return SessionDetail(
                id=session_id,
                model_id=deserialized_data.get("model_id"),
                persona_id=deserialized_data.get("persona_id", "default"),
                created_at=datetime.fromisoformat(deserialized_meta.get("created_at")),
                updated_at=datetime.fromisoformat(deserialized_meta.get("updated_at")),
                last_activity=datetime.fromisoformat(deserialized_meta.get("last_activity")),
                is_active=deserialized_meta.get("is_active") == "true",
                agent_internal_id=deserialized_data.get("agent_internal_id"),
                tools=deserialized_data.get("tools", []),
                temperature=deserialized_data.get("temperature"),
                reasoning_effort=deserialized_data.get("reasoning_effort"),
                budget_tokens=deserialized_data.get("budget_tokens"),
                max_tokens=deserialized_data.get("max_tokens"),
                custom_prompt=deserialized_data.get("custom_prompt")
            )
            
        except Exception as e:
            self.logger.error("get_session_failed", session_id=session_id, error=str(e))
            return None
    
    async def update_session(self, session_id: str, update_data: SessionUpdate) -> Optional[SessionDetail]:
        """Update session properties
        
        Args:
            session_id: Session ID
            update_data: Properties to update
            
        Returns:
            Updated session details if successful, None otherwise
        """
        try:
            # Check if session exists
            if not await self.redis.exists(f"session:{session_id}:data"):
                return None
                
            # Get current timestamp
            timestamp = datetime.now().isoformat()
            
            # Prepare update data
            updates = update_data.model_dump(exclude_unset=True, exclude_none=True)
            
            # Separate core data and metadata updates
            core_updates = {}
            for key, value in updates.items():
                if key in ["temperature", "reasoning_effort", "budget_tokens", 
                          "max_tokens", "custom_prompt", "tools"]:
                    core_updates[key] = await self._serialize_value(value)
            
            # Create Redis pipeline
            pipe = self.redis.pipeline()
            
            # Update core data if needed
            if core_updates:
                pipe.hset(f"session:{session_id}:data", mapping=core_updates)
            
            # Update metadata
            pipe.hset(f"session:{session_id}:meta", "updated_at", timestamp)
            pipe.hset(f"session:{session_id}:meta", "last_activity", timestamp)
            
            # Execute pipeline
            await pipe.execute()
            
            # Refresh TTL
            await self._set_session_ttl(session_id)
            
            # Return updated session
            return await self.get_session(session_id)
            
        except Exception as e:
            self.logger.error("update_session_failed", session_id=session_id, error=str(e))
            return None
    
    async def delete_session(self, session_id: str) -> bool:
        """Delete a session
        
        Args:
            session_id: Session ID
            
        Returns:
            True if successful, False otherwise
        """
        try:
            # Create Redis pipeline
            pipe = self.redis.pipeline()
            
            # Remove all session keys
            pipe.delete(f"session:{session_id}:data")
            pipe.delete(f"session:{session_id}:meta")
            pipe.srem("active_sessions", session_id)
            
            # Execute pipeline
            await pipe.execute()
            
            return True
            
        except Exception as e:
            self.logger.error("delete_session_failed", session_id=session_id, error=str(e))
            return False
    
    async def list_sessions(self, limit: int = 10, offset: int = 0) -> SessionListResponse:
        """List active sessions with pagination
        
        Args:
            limit: Maximum number of sessions to return
            offset: Number of sessions to skip
            
        Returns:
            Paginated list of sessions
        """
        try:
            # Get all active session IDs
            session_ids = await self.redis.smembers("active_sessions")
            
            # Convert to strings and sort
            session_ids = sorted([
                id.decode('utf-8') if isinstance(id, bytes) else id
                for id in session_ids
            ])
            
            # Get total count
            total = len(session_ids)
            
            # Apply pagination
            paginated_ids = session_ids[offset:offset + limit]
            
            # Get session summaries
            sessions = []
            for session_id in paginated_ids:
                session = await self.get_session(session_id)
                if session:
                    sessions.append(SessionSummary(
                        id=session.id,
                        model_id=session.model_id,
                        persona_id=session.persona_id,
                        created_at=session.created_at,
                        last_activity=session.last_activity,
                        is_active=session.is_active
                    ))
            
            return SessionListResponse(
                items=sessions,
                total=total,
                limit=limit,
                offset=offset
            )
            
        except Exception as e:
            self.logger.error("list_sessions_failed", error=str(e))
            return SessionListResponse(items=[], total=0, limit=limit, offset=offset)
    
    async def cleanup_expired_sessions(self) -> int:
        """Clean up expired sessions
        
        Returns:
            Number of sessions cleaned up
        """
        try:
            # Get all active session IDs
            session_ids = await self.redis.smembers("active_sessions")
            
            # Convert to strings
            session_ids = [
                id.decode('utf-8') if isinstance(id, bytes) else id
                for id in session_ids
            ]
            
            cleaned = 0
            for session_id in session_ids:
                # Check if session data exists
                if not await self.redis.exists(f"session:{session_id}:data"):
                    # Session expired, clean up
                    await self.delete_session(session_id)
                    cleaned += 1
            
            return cleaned
            
        except Exception as e:
            self.logger.error("cleanup_expired_sessions_failed", error=str(e))
            return 0
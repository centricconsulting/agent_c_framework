from datetime import datetime
import json
import re
from datetime import datetime
from typing import Dict, List, Any, Optional, Union

from redis import asyncio as aioredis
import structlog
from agent_c.util import MnemonicSlugs

from agent_c_api.api.v2.models.session_models import (
    SessionCreate,
    SessionDetail,
    SessionListResponse,
    SessionSummary,
    SessionUpdate
)
from agent_c_api.core.repositories.exceptions import (
    SessionRepositoryError,
    SessionNotFoundError,
    InvalidSessionIdError,
    SessionCreationError,
    RedisConnectionError,
    SessionSerializationError,
    SessionUpdateError,
    SessionValidationError,
    add_session_context,
    add_redis_context
)

class SessionRepository:
    """Repository for managing sessions in Redis
    
    Uses MnemonicSlugs for session ID generation (e.g., 'tiger-castle').
    GUID format session IDs are not supported and will be rejected.
    
    Redis Key Structure:
    - session:{session_id} -> hash (consolidated session data + metadata)
    - sessions:active -> set (active session IDs)
    
    This design provides:
    - Atomic operations on session data
    - Single TTL operation per session
    - Reduced Redis memory overhead
    - Simplified key management
    """
    
    def __init__(self, redis_client: aioredis.Redis):
        """Initialize the session repository
        
        Args:
            redis_client: Redis client instance
        """
        self.redis = redis_client
        self.logger = structlog.get_logger(__name__)
        self.session_ttl = 24 * 60 * 60  # 24 hours default TTL
    
    def _validate_session_id(self, session_id: str) -> None:
        """Validate session ID format - MnemonicSlugs only
        
        Args:
            session_id: Session ID to validate
            
        Raises:
            InvalidSessionIdError: If session ID format is invalid
        """
        if not session_id or not isinstance(session_id, str):
            raise InvalidSessionIdError(
                session_id=str(session_id) if session_id else "<empty>",
                expected_format="non-empty string in MnemonicSlug format",
                context={"validation_error": "empty or non-string value"}
            )
        
        # Check for mnemonic slug format (word-word pattern)
        if not re.match(r'^[a-z]+-[a-z]+$', session_id):
            raise InvalidSessionIdError(
                session_id=session_id,
                context={
                    "validation_error": "format mismatch",
                    "pattern_required": "^[a-z]+-[a-z]+$",
                    "example": "tiger-castle"
                }
            )
    
    # NOTE: _serialize_value and _deserialize_value methods removed!
    # We now use proper Pydantic model serialization:
    # - session_detail.model_dump_json() for serialization
    # - SessionDetail.model_validate_json() for deserialization
    # This eliminates 70+ lines of unnecessary manual serialization code.
    
    async def _set_session_ttl(self, session_id: str) -> None:
        """Set or refresh session TTL
        
        Args:
            session_id: Session ID
        """
        # Set TTL for single session hash
        await self.redis.expire(f"session:{session_id}", self.session_ttl)
    
    async def create_session(self, session_data: SessionCreate) -> SessionDetail:
        """Create a new session
        
        Generates MnemonicSlug format session ID if not provided (e.g., 'tiger-castle').
        GUID format session IDs are rejected.
        
        Args:
            session_data: Session creation parameters
            
        Returns:
            Created session details
            
        Raises:
            ValueError: If session ID format is invalid
            Exception: If session creation fails
        """
        try:
            # Generate session ID if not provided
            session_id = session_data.id or MnemonicSlugs.generate_slug(2)
            
            # Validate session ID format
            self._validate_session_id(session_id)
            
            # Get current timestamp
            timestamp = datetime.now().isoformat()
            
            # Create SessionDetail from SessionCreate data with metadata
            session_detail = SessionDetail(
                id=session_id,
                model_id=session_data.model_id,
                persona_id=session_data.persona_id or "default",
                name=session_data.name or f"Session {session_id}",  # Provide default name
                custom_prompt=session_data.custom_prompt,
                temperature=session_data.temperature,
                reasoning_effort=session_data.reasoning_effort,
                budget_tokens=session_data.budget_tokens,
                max_tokens=session_data.max_tokens,
                tools=session_data.tools or [],
                tool_ids=session_data.tools or [],  # Copy tools to tool_ids for compatibility
                metadata=session_data.metadata or {},
                created_at=datetime.fromisoformat(timestamp),
                updated_at=datetime.fromisoformat(timestamp),
                last_activity=datetime.fromisoformat(timestamp),
                is_active=True
            )
            
            # Store the entire model as JSON (proper Pydantic serialization)
            session_json = session_detail.model_dump_json(exclude_none=True)
            
            # Create Redis pipeline for atomic operations
            pipe = self.redis.pipeline()
            
            # Store session as JSON string
            pipe.set(f"session:{session_id}", session_json)
            
            # Add to active sessions set (renamed for clarity)
            pipe.sadd("sessions:active", session_id)
            
            # Execute pipeline atomically
            await pipe.execute()
            
            # Set TTL for session JSON
            pipe.expire(f"session:{session_id}", self.session_ttl)
            
            # Return session details
            return await self.get_session(session_id)
            
        except InvalidSessionIdError:
            # Re-raise validation errors as-is
            raise
        except SessionSerializationError:
            # Re-raise serialization errors as-is
            raise
        except Exception as e:
            # Wrap other exceptions in SessionCreationError
            context = add_session_context(session_id if 'session_id' in locals() else "unknown", "create")
            context.update({"session_data": session_data.model_dump() if hasattr(session_data, 'model_dump') else str(session_data)})
            
            self.logger.error(
                "create_session_failed", 
                error=str(e),
                session_id=context.get("session_id"),
                context=context
            )
            
            raise SessionCreationError(
                message=str(e),
                session_data=session_data.model_dump() if hasattr(session_data, 'model_dump') else None,
                context=context
            )
    
    async def get_session(self, session_id: str) -> Optional[SessionDetail]:
        """Get session details
        
        Args:
            session_id: Session ID
            
        Returns:
            Session details if found, None otherwise
        """
        try:
            # Validate session ID format
            self._validate_session_id(session_id)
            
            # Check if session exists
            if not await self.redis.exists(f"session:{session_id}"):
                return None
                
            # Get session JSON from Redis
            session_json = await self.redis.get(f"session:{session_id}")
            
            if not session_json:
                return None
                
            # Deserialize using Pydantic (proper model validation)
            session_detail = SessionDetail.model_validate_json(session_json)
            
            # Refresh TTL on access
            await self._set_session_ttl(session_id)
            
            return session_detail
            
        except InvalidSessionIdError:
            # Re-raise validation errors as-is
            raise
        except SessionSerializationError:
            # Re-raise serialization errors as-is
            raise
        except Exception as e:
            # Log Redis or other errors but don't raise (return None for not found)
            context = add_session_context(session_id, "get")
            self.logger.error(
                "get_session_failed", 
                session_id=session_id, 
                error=str(e),
                context=context
            )
            
            # For Redis connection errors, we should raise to indicate service issue
            if "redis" in str(e).lower() or "connection" in str(e).lower():
                raise RedisConnectionError(
                    operation="get_session",
                    redis_error=e,
                    context=context
                )
            
            # For other errors, return None (session not found/accessible)
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
            # Validate session ID format
            self._validate_session_id(session_id)
            
            # Get current session
            current_session = await self.get_session(session_id)
            if not current_session:
                return None
                
            # Get current timestamp
            timestamp = datetime.now()
            
            # Prepare update data (only fields that were actually set)
            updates = update_data.model_dump(exclude_unset=True, exclude_none=True)
            
            # Create updated session by merging current data with updates
            updated_data = current_session.model_dump()
            updated_data.update(updates)
            updated_data["updated_at"] = timestamp
            updated_data["last_activity"] = timestamp
            
            # Create updated SessionDetail and store as JSON
            updated_session = SessionDetail.model_validate(updated_data)
            session_json = updated_session.model_dump_json(exclude_none=True)
            
            # Store updated session JSON
            await self.redis.set(f"session:{session_id}", session_json)
            
            # Refresh TTL
            await self._set_session_ttl(session_id)
            
            # Return the updated session (we already have it)
            return updated_session
            
        except InvalidSessionIdError:
            # Re-raise validation errors as-is
            raise
        except SessionSerializationError:
            # Re-raise serialization errors as-is
            raise
        except Exception as e:
            # Wrap other exceptions in SessionUpdateError
            context = add_session_context(session_id, "update")
            context.update({"update_data": update_data.model_dump() if hasattr(update_data, 'model_dump') else str(update_data)})
            
            self.logger.error(
                "update_session_failed", 
                session_id=session_id, 
                error=str(e),
                context=context
            )
            
            raise SessionUpdateError(
                session_id=session_id,
                message=str(e),
                update_data=update_data.model_dump() if hasattr(update_data, 'model_dump') else None,
                context=context
            )
    
    async def delete_session(self, session_id: str) -> bool:
        """Delete a session
        
        Args:
            session_id: Session ID
            
        Returns:
            True if successful, False otherwise
        """
        try:
            # Validate session ID format
            self._validate_session_id(session_id)
            
            # Create Redis pipeline for atomic deletion
            pipe = self.redis.pipeline()
            
            # Remove session hash and active sessions entry
            pipe.delete(f"session:{session_id}")
            pipe.srem("sessions:active", session_id)
            
            # Execute pipeline atomically
            await pipe.execute()
            
            return True
            
        except InvalidSessionIdError:
            # Re-raise validation errors as-is
            raise
        except Exception as e:
            # Log Redis or other errors
            context = add_session_context(session_id, "delete")
            self.logger.error(
                "delete_session_failed", 
                session_id=session_id, 
                error=str(e),
                context=context
            )
            
            # For Redis connection errors, raise to indicate service issue
            if "redis" in str(e).lower() or "connection" in str(e).lower():
                raise RedisConnectionError(
                    operation="delete_session",
                    redis_error=e,
                    context=context
                )
            
            # For other errors, return False (deletion failed)
            return False
    
    async def list_sessions(
        self, 
        limit: int = 10, 
        offset: Optional[int] = None,
        cursor: Optional[str] = None,
        sort_by: str = "last_activity",
        sort_order: str = "desc"
    ) -> SessionListResponse:
        """List active sessions with efficient pagination
        
        Supports both offset-based (legacy) and cursor-based pagination.
        Cursor-based pagination is more efficient for large datasets.
        
        Args:
            limit: Maximum number of sessions to return
            offset: Number of sessions to skip (offset-based pagination)
            cursor: Cursor for pagination (cursor-based pagination)
            sort_by: Field to sort by (last_activity, created_at, id)
            sort_order: Sort order (asc, desc)
            
        Returns:
            Paginated list of sessions
        """
        try:
            # Use cursor-based pagination if cursor provided or offset not provided
            if cursor is not None or offset is None:
                return await self._list_sessions_cursor_based(limit, cursor, sort_by, sort_order)
            else:
                return await self._list_sessions_offset_based(limit, offset, sort_by, sort_order)
                
        except Exception as e:
            self.logger.error("list_sessions_failed", error=str(e))
            return SessionListResponse(
                items=[], 
                total=0 if offset is not None else None,
                limit=limit, 
                offset=offset,
                cursor=cursor,
                has_more=False
            )
    
    async def _list_sessions_cursor_based(
        self,
        limit: int,
        cursor: Optional[str],
        sort_by: str,
        sort_order: str
    ) -> SessionListResponse:
        """Efficient cursor-based pagination using Redis SSCAN"""
        
        # Parse cursor (Redis SSCAN cursor)
        scan_cursor = int(cursor) if cursor and cursor.isdigit() else 0
        
        # Use SSCAN for efficient pagination
        next_cursor, session_ids = await self.redis.sscan(
            "sessions:active", 
            cursor=scan_cursor, 
            count=limit * 2  # Get more to allow for sorting/filtering
        )
        
        # Convert to strings
        session_ids = [
            id.decode('utf-8') if isinstance(id, bytes) else id
            for id in session_ids
        ]
        
        # Get session data for sorting (batch operation)
        sessions_data = await self._batch_get_sessions_data(session_ids)
        
        # Sort sessions based on criteria
        sorted_sessions = self._sort_sessions_data(sessions_data, sort_by, sort_order)
        
        # Apply limit
        limited_sessions = sorted_sessions[:limit]
        
        # Convert to SessionSummary objects
        session_summaries = [
            SessionSummary(
                id=session_data["id"],
                model_id=session_data["model_id"],
                persona_id=session_data["persona_id"],
                created_at=session_data["created_at"],
                last_activity=session_data["last_activity"],
                is_active=session_data["is_active"]
            )
            for session_data in limited_sessions
        ]
        
        # Determine if there are more pages
        has_more = next_cursor != 0 or len(sorted_sessions) > limit
        next_cursor_str = str(next_cursor) if has_more and next_cursor != 0 else None
        
        return SessionListResponse(
            items=session_summaries,
            total=None,  # Total not available in cursor-based pagination
            limit=limit,
            cursor=cursor,
            next_cursor=next_cursor_str,
            has_more=has_more,
            sort_by=sort_by,
            sort_order=sort_order
        )
    
    async def _list_sessions_offset_based(
        self,
        limit: int,
        offset: int,
        sort_by: str,
        sort_order: str
    ) -> SessionListResponse:
        """Legacy offset-based pagination (less efficient but provides total count)"""
        
        # Get all active session IDs
        session_ids = await self.redis.smembers("sessions:active")
        
        # Convert to strings
        session_ids = [
            id.decode('utf-8') if isinstance(id, bytes) else id
            for id in session_ids
        ]
        
        # Get session data for sorting (batch operation)
        sessions_data = await self._batch_get_sessions_data(session_ids)
        
        # Sort sessions based on criteria
        sorted_sessions = self._sort_sessions_data(sessions_data, sort_by, sort_order)
        
        # Get total count
        total = len(sorted_sessions)
        
        # Apply pagination
        paginated_sessions = sorted_sessions[offset:offset + limit]
        
        # Convert to SessionSummary objects
        session_summaries = [
            SessionSummary(
                id=session_data["id"],
                model_id=session_data["model_id"],
                persona_id=session_data["persona_id"],
                created_at=session_data["created_at"],
                last_activity=session_data["last_activity"],
                is_active=session_data["is_active"]
            )
            for session_data in paginated_sessions
        ]
        
        return SessionListResponse(
            items=session_summaries,
            total=total,
            limit=limit,
            offset=offset,
            sort_by=sort_by,
            sort_order=sort_order,
            has_more=offset + limit < total
        )
    
    async def _batch_get_sessions_data(self, session_ids: List[str]) -> List[Dict[str, Any]]:
        """Efficiently retrieve session data for multiple sessions using pipeline"""
        if not session_ids:
            return []
        
        # Use Redis pipeline for batch operations
        pipe = self.redis.pipeline()
        
        # Queue all session JSON requests
        for session_id in session_ids:
            pipe.get(f"session:{session_id}")
        
        # Execute all requests at once
        results = await pipe.execute()
        
        # Process results
        sessions_data = []
        for i, session_id in enumerate(session_ids):
            session_json = results[i]
            if session_json:  # Session exists
                try:
                    # Deserialize using Pydantic
                    session_detail = SessionDetail.model_validate_json(session_json)
                    sessions_data.append(session_detail.model_dump())
                except Exception as e:
                    # Log error but continue with other sessions
                    self.logger.warning(
                        "session_deserialization_failed",
                        session_id=session_id,
                        error=str(e)
                    )
        
        return sessions_data
    
    def _sort_sessions_data(
        self, 
        sessions_data: List[Dict[str, Any]], 
        sort_by: str, 
        sort_order: str
    ) -> List[Dict[str, Any]]:
        """Sort session data by specified criteria"""
        
        # Define sort key function
        def get_sort_key(session: Dict[str, Any]):
            if sort_by == "last_activity":
                return session.get("last_activity", datetime.min)
            elif sort_by == "created_at":
                return session.get("created_at", datetime.min)
            elif sort_by == "id":
                return session.get("id", "")
            elif sort_by == "model_id":
                return session.get("model_id", "")
            else:
                # Default to last_activity
                return session.get("last_activity", datetime.min)
        
        # Sort the data
        reverse = sort_order.lower() == "desc"
        return sorted(sessions_data, key=get_sort_key, reverse=reverse)
    
    async def cleanup_expired_sessions(self) -> int:
        """Clean up expired sessions using efficient batch operations
        
        This method provides manual cleanup for edge cases and orphaned entries.
        For real-time cleanup, use setup_keyspace_notifications() and 
        listen_for_expirations() for automatic cleanup via Redis events.
        
        Returns:
            Number of sessions cleaned up
        """
        try:
            # Use SSCAN for efficient iteration instead of loading all IDs
            cursor = 0
            cleaned = 0
            batch_size = 100  # Process in batches to avoid blocking
            
            while True:
                # Scan active sessions in batches
                cursor, session_ids = await self.redis.sscan(
                    "sessions:active", 
                    cursor=cursor, 
                    count=batch_size
                )
                
                if not session_ids:
                    break
                    
                # Convert to strings
                session_ids = [
                    id.decode('utf-8') if isinstance(id, bytes) else id
                    for id in session_ids
                ]
                
                # Use pipeline for batch existence checks
                pipe = self.redis.pipeline()
                for session_id in session_ids:
                    pipe.exists(f"session:{session_id}")
                
                # Execute batch existence checks
                existence_results = await pipe.execute()
                
                # Collect expired session IDs
                expired_ids = [
                    session_id for session_id, exists in 
                    zip(session_ids, existence_results) if not exists
                ]
                
                # Remove expired sessions from active set in batch
                if expired_ids:
                    await self.redis.srem("sessions:active", *expired_ids)
                    cleaned += len(expired_ids)
                    self.logger.info(
                        "cleanup_batch_completed", 
                        batch_size=len(expired_ids),
                        total_cleaned=cleaned
                    )
                
                # Break if we've scanned all items
                if cursor == 0:
                    break
            
            if cleaned > 0:
                self.logger.info("cleanup_expired_sessions_completed", total_cleaned=cleaned)
            
            return cleaned
            
        except Exception as e:
            # Log cleanup errors but don't raise (return 0 for failed cleanup)
            context = add_redis_context("sessions:active", "cleanup")
            self.logger.error(
                "cleanup_expired_sessions_failed", 
                error=str(e),
                context=context
            )
            
            # For Redis connection errors, we might want to raise
            if "redis" in str(e).lower() or "connection" in str(e).lower():
                self.logger.warning(
                    "cleanup_failed_redis_connection",
                    error=str(e),
                    message="Cleanup failed due to Redis connection issue"
                )
            
            return 0
    
    async def setup_keyspace_notifications(self) -> bool:
        """Configure Redis for keyspace notifications on session expirations
        
        This enables automatic cleanup via expiration events.
        Requires Redis configuration: notify-keyspace-events Ex
        
        Returns:
            True if configuration successful, False otherwise
        """
        try:
            # Configure Redis to notify on expired events
            await self.redis.config_set("notify-keyspace-events", "Ex")
            self.logger.info("keyspace_notifications_configured")
            return True
        except Exception as e:
            self.logger.error("keyspace_notifications_setup_failed", error=str(e))
            return False
    
    async def listen_for_expirations(self, callback=None):
        """Listen for Redis keyspace expiration events for automatic cleanup
        
        This method should be run as a background task to provide real-time
        cleanup of expired sessions from the active set.
        
        Args:
            callback: Optional callback function for expiration events
        """
        try:
            # Subscribe to expiration events for session keys
            pubsub = self.redis.pubsub()
            await pubsub.psubscribe("__keyevent@*__:expired")
            
            self.logger.info("expiration_listener_started")
            
            async for message in pubsub.listen():
                if message['type'] == 'pmessage':
                    expired_key = message['data'].decode('utf-8')
                    
                    # Check if this is a session key
                    if expired_key.startswith('session:'):
                        session_id = expired_key.replace('session:', '')
                        
                        # Remove from active sessions set
                        removed = await self.redis.srem("sessions:active", session_id)
                        
                        if removed:
                            self.logger.info(
                                "session_auto_cleaned", 
                                session_id=session_id,
                                expired_key=expired_key
                            )
                            
                            # Call optional callback
                            if callback:
                                await callback(session_id)
                                
        except Exception as e:
            self.logger.error("expiration_listener_failed", error=str(e))
        finally:
            try:
                await pubsub.unsubscribe()
                await pubsub.close()
            except:
                pass
    
    async def start_background_cleanup(self):
        """Start background cleanup service
        
        This combines keyspace notifications for real-time cleanup with
        periodic manual cleanup for edge cases.
        """
        import asyncio
        
        try:
            # Setup keyspace notifications
            await self.setup_keyspace_notifications()
            
            # Start expiration listener as background task
            expiration_task = asyncio.create_task(self.listen_for_expirations())
            
            # Start periodic manual cleanup as backup (every 5 minutes)
            async def periodic_cleanup():
                while True:
                    await asyncio.sleep(300)  # 5 minutes
                    cleaned = await self.cleanup_expired_sessions()
                    if cleaned > 0:
                        self.logger.info("periodic_cleanup_completed", cleaned=cleaned)
            
            cleanup_task = asyncio.create_task(periodic_cleanup())
            
            self.logger.info("background_cleanup_service_started")
            
            # Return tasks so they can be managed by the caller
            return expiration_task, cleanup_task
            
        except Exception as e:
            self.logger.error("background_cleanup_start_failed", error=str(e))
            return None, None
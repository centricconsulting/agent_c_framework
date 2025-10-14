"""
Custom exception classes for session repository operations.

This module provides specific exception types for different error scenarios
in the session repository, enabling better error handling, diagnostics, and
recovery mechanisms.
"""

from typing import Optional, Dict, Any


class SessionRepositoryError(Exception):
    """Base exception for session repository operations
    
    All session repository exceptions inherit from this base class,
    allowing for broad exception handling when needed.
    
    Attributes:
        message: Human-readable error message
        context: Additional context information for debugging
        recoverable: Whether this error might be recoverable with retry
    """
    
    def __init__(
        self, 
        message: str, 
        context: Optional[Dict[str, Any]] = None,
        recoverable: bool = False
    ):
        super().__init__(message)
        self.message = message
        self.context = context or {}
        self.recoverable = recoverable
    
    def __str__(self) -> str:
        if self.context:
            context_str = ", ".join(f"{k}={v}" for k, v in self.context.items())
            return f"{self.message} (context: {context_str})"
        return self.message


class SessionNotFoundError(SessionRepositoryError):
    """Session does not exist
    
    Raised when attempting to access a session that doesn't exist in Redis.
    This is typically not recoverable without creating a new session.
    """
    
    def __init__(self, session_id: str, context: Optional[Dict[str, Any]] = None):
        message = f"Session not found: {session_id}"
        super().__init__(
            message=message,
            context={**(context or {}), "session_id": session_id},
            recoverable=False
        )
        self.session_id = session_id


class InvalidSessionIdError(SessionRepositoryError):
    """Session ID format is invalid
    
    Raised when a session ID doesn't conform to the required MnemonicSlug format.
    This indicates a programming error or invalid user input.
    """
    
    def __init__(
        self, 
        session_id: str, 
        expected_format: str = "MnemonicSlug format (e.g., 'tiger-castle')",
        context: Optional[Dict[str, Any]] = None
    ):
        message = f"Invalid session ID format: {session_id}. Expected: {expected_format}"
        super().__init__(
            message=message,
            context={**(context or {}), "session_id": session_id, "expected_format": expected_format},
            recoverable=False
        )
        self.session_id = session_id
        self.expected_format = expected_format


class SessionCreationError(SessionRepositoryError):
    """Failed to create session
    
    Raised when session creation fails due to validation errors,
    Redis operations, or other creation-specific issues.
    """
    
    def __init__(
        self, 
        message: str, 
        session_data: Optional[Dict[str, Any]] = None,
        context: Optional[Dict[str, Any]] = None
    ):
        super().__init__(
            message=f"Session creation failed: {message}",
            context={**(context or {}), "session_data": session_data},
            recoverable=True  # Creation might succeed on retry
        )
        self.session_data = session_data


class RedisConnectionError(SessionRepositoryError):
    """Redis connection or operation failed
    
    Raised when Redis operations fail due to connection issues,
    timeouts, or Redis server errors. Often recoverable with retry.
    """
    
    def __init__(
        self, 
        operation: str, 
        redis_error: Optional[Exception] = None,
        context: Optional[Dict[str, Any]] = None
    ):
        message = f"Redis operation failed: {operation}"
        if redis_error:
            message += f" - {str(redis_error)}"
        
        super().__init__(
            message=message,
            context={**(context or {}), "operation": operation, "redis_error": str(redis_error) if redis_error else None},
            recoverable=True  # Redis errors are often transient
        )
        self.operation = operation
        self.redis_error = redis_error


class SessionSerializationError(SessionRepositoryError):
    """Failed to serialize/deserialize session data
    
    Raised when JSON serialization/deserialization fails or when
    Pydantic model validation fails during data conversion.
    """
    
    def __init__(
        self, 
        operation: str,  # "serialize" or "deserialize"
        data_type: str,
        original_error: Optional[Exception] = None,
        context: Optional[Dict[str, Any]] = None
    ):
        message = f"Failed to {operation} {data_type}"
        if original_error:
            message += f": {str(original_error)}"
        
        super().__init__(
            message=message,
            context={**(context or {}), "operation": operation, "data_type": data_type, "original_error": str(original_error) if original_error else None},
            recoverable=False  # Serialization errors indicate data corruption
        )
        self.operation = operation
        self.data_type = data_type
        self.original_error = original_error


class SessionUpdateError(SessionRepositoryError):
    """Failed to update session
    
    Raised when session update operations fail due to validation errors,
    Redis operations, or session state issues.
    """
    
    def __init__(
        self, 
        session_id: str,
        message: str,
        update_data: Optional[Dict[str, Any]] = None,
        context: Optional[Dict[str, Any]] = None
    ):
        full_message = f"Session update failed for {session_id}: {message}"
        super().__init__(
            message=full_message,
            context={**(context or {}), "session_id": session_id, "update_data": update_data},
            recoverable=True  # Updates might succeed on retry
        )
        self.session_id = session_id
        self.update_data = update_data


class SessionValidationError(SessionRepositoryError):
    """Session data validation failed
    
    Raised when session data doesn't meet business rules or
    validation requirements (e.g., invalid model_id, bad parameters).
    """
    
    def __init__(
        self, 
        field: str,
        value: Any,
        message: str,
        context: Optional[Dict[str, Any]] = None
    ):
        full_message = f"Validation failed for {field}: {message}"
        super().__init__(
            message=full_message,
            context={**(context or {}), "field": field, "value": value},
            recoverable=False  # Validation errors require data correction
        )
        self.field = field
        self.value = value


# Convenience function for error recovery
def is_recoverable_error(error: Exception) -> bool:
    """Check if an error might be recoverable with retry
    
    Args:
        error: Exception to check
        
    Returns:
        True if the error might be recoverable, False otherwise
    """
    if isinstance(error, SessionRepositoryError):
        return error.recoverable
    
    # Consider some standard exceptions as potentially recoverable
    recoverable_types = (
        ConnectionError,
        TimeoutError,
        OSError,  # Network-related errors
    )
    
    return isinstance(error, recoverable_types)


# Error context helpers
def add_session_context(session_id: str, operation: str) -> Dict[str, Any]:
    """Create standard context for session operations
    
    Args:
        session_id: Session ID being operated on
        operation: Operation being performed
        
    Returns:
        Context dictionary for error reporting
    """
    return {
        "session_id": session_id,
        "operation": operation,
        "timestamp": __import__("datetime").datetime.utcnow().isoformat()
    }


def add_redis_context(key: str, operation: str) -> Dict[str, Any]:
    """Create standard context for Redis operations
    
    Args:
        key: Redis key being operated on
        operation: Redis operation being performed
        
    Returns:
        Context dictionary for error reporting
    """
    return {
        "redis_key": key,
        "redis_operation": operation,
        "timestamp": __import__("datetime").datetime.utcnow().isoformat()
    }
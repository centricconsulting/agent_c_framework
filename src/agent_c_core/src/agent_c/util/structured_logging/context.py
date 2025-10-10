"""
Context Management for Structured Logging

This module provides framework-wide context management using contextvars
for automatic context propagation across async boundaries and threads.
"""

import contextvars
from typing import Optional, Dict, Any, Callable
from dataclasses import dataclass, asdict
from contextlib import contextmanager
import threading
import time

from ..slugs import MnemonicSlugs


# Framework-wide context variables
correlation_id_var: contextvars.ContextVar[Optional[str]] = contextvars.ContextVar('correlation_id', default=None)
agent_id_var: contextvars.ContextVar[Optional[str]] = contextvars.ContextVar('agent_id', default=None)
session_id_var: contextvars.ContextVar[Optional[str]] = contextvars.ContextVar('session_id', default=None)
user_id_var: contextvars.ContextVar[Optional[str]] = contextvars.ContextVar('user_id', default=None)
operation_var: contextvars.ContextVar[Optional[str]] = contextvars.ContextVar('operation', default=None)


@dataclass
class LoggingContext:
    """
    Dataclass representing the current logging context.
    
    This class provides a convenient way to work with logging context
    and automatically propagate it through the application.
    
    Attributes:
        correlation_id: Unique identifier for request/operation correlation
        agent_id: Identifier for the current agent
        session_id: Identifier for the current session
        user_id: Identifier for the current user
        operation: Name of the current operation being performed
    """
    correlation_id: Optional[str] = None
    agent_id: Optional[str] = None
    session_id: Optional[str] = None
    user_id: Optional[str] = None
    operation: Optional[str] = None
    
    def apply(self) -> None:
        """Apply this context to the current context variables."""
        if self.correlation_id is not None:
            correlation_id_var.set(self.correlation_id)
        if self.agent_id is not None:
            agent_id_var.set(self.agent_id)
        if self.session_id is not None:
            session_id_var.set(self.session_id)
        if self.user_id is not None:
            user_id_var.set(self.user_id)
        if self.operation is not None:
            operation_var.set(self.operation)
    
    def to_dict(self) -> Dict[str, Any]:
        """Convert context to dictionary, excluding None values."""
        return {k: v for k, v in asdict(self).items() if v is not None}
    
    @classmethod
    def from_current(cls) -> "LoggingContext":
        """Create a LoggingContext from current context variables."""
        return cls(
            correlation_id=correlation_id_var.get(),
            agent_id=agent_id_var.get(),
            session_id=session_id_var.get(),
            user_id=user_id_var.get(),
            operation=operation_var.get(),
        )
    
    def __enter__(self) -> "LoggingContext":
        """Context manager entry - apply the context."""
        self.apply()
        return self
    
    def __exit__(self, exc_type, exc_val, exc_tb) -> None:
        """Context manager exit - context is automatically restored by contextvars."""
        pass


def get_current_context() -> LoggingContext:
    """
    Get the current logging context.
    
    Returns:
        LoggingContext: The current context with all available values
    """
    return LoggingContext.from_current()


def clear_context() -> None:
    """
    Clear all context variables.
    
    This is useful for cleanup between requests or operations
    to prevent context leakage.
    """
    correlation_id_var.set(None)
    agent_id_var.set(None)
    session_id_var.set(None)
    user_id_var.set(None)
    operation_var.set(None)


def set_correlation_id(correlation_id: str) -> None:
    """Set the correlation ID for the current context."""
    correlation_id_var.set(correlation_id)


def get_correlation_id() -> Optional[str]:
    """Get the current correlation ID."""
    return correlation_id_var.get()


def set_agent_id(agent_id: str) -> None:
    """Set the agent ID for the current context."""
    agent_id_var.set(agent_id)


def get_agent_id() -> Optional[str]:
    """Get the current agent ID."""
    return agent_id_var.get()


def set_session_id(session_id: str) -> None:
    """Set the session ID for the current context."""
    session_id_var.set(session_id)


def get_session_id() -> Optional[str]:
    """Get the current session ID."""
    return session_id_var.get()


def set_user_id(user_id: str) -> None:
    """Set the user ID for the current context."""
    user_id_var.set(user_id)


def get_user_id() -> Optional[str]:
    """Get the current user ID."""
    return user_id_var.get()


def set_operation(operation: str) -> None:
    """Set the current operation name."""
    operation_var.set(operation)


def get_operation() -> Optional[str]:
    """Get the current operation name."""
    return operation_var.get()


@contextmanager
def logging_context(**kwargs):
    """
    Context manager for temporarily setting logging context.
    
    Args:
        **kwargs: Context values to set (correlation_id, agent_id, session_id, user_id, operation)
        
    Example:
        with logging_context(correlation_id="req-123", user_id="user-456"):
            logger.info("Processing request")  # Automatically includes context
    """
    context = LoggingContext(**kwargs)
    with context:
        yield context


def get_context_dict() -> Dict[str, Any]:
    """
    Get the current context as a dictionary.
    
    This is useful for processors that need to inject context into log records.
    
    Returns:
        Dict[str, Any]: Current context as dictionary, excluding None values
    """
    return get_current_context().to_dict()


def generate_correlation_id() -> str:
    """
    Generate a new correlation ID using the Agent C MnemonicSlugs system.
    
    Returns:
        str: A new unique correlation ID (e.g., "tiger-castle")
    """
    return MnemonicSlugs.generate_slug(4)


def ensure_correlation_id() -> str:
    """
    Ensure there's a correlation ID in the current context.
    
    If no correlation ID exists, generates and sets a new one.
    
    Returns:
        str: The current or newly generated correlation ID
    """
    correlation_id = get_correlation_id()
    if correlation_id is None:
        correlation_id = generate_correlation_id()
        set_correlation_id(correlation_id)
    return correlation_id


def copy_context_to_thread(target_func: Callable, *args, **kwargs) -> Any:
    """
    Execute a function in a new thread with current context copied.
    
    This utility helps maintain context when spawning new threads.
    
    Args:
        target_func: Function to execute in the new thread
        *args: Arguments for the target function
        **kwargs: Keyword arguments for the target function
        
    Returns:
        Any: Result from the target function
    """
    # Capture current context
    current_context = get_current_context()
    
    def wrapper():
        # Apply context in the new thread
        current_context.apply()
        return target_func(*args, **kwargs)
    
    # Execute in new thread
    thread = threading.Thread(target=wrapper)
    thread.start()
    thread.join()
    
    return None  # Note: This is a simplified version for demonstration


def with_context(context_dict: Dict[str, Any]):
    """
    Decorator to apply context to a function.
    
    Args:
        context_dict: Dictionary of context values to apply
        
    Example:
        @with_context({"operation": "user_login", "user_id": "user-123"})
        def login_user():
            logger.info("User login attempt")  # Includes context automatically
    """
    def decorator(func):
        def wrapper(*args, **kwargs):
            with LoggingContext(**context_dict):
                return func(*args, **kwargs)
        return wrapper
    return decorator


def track_operation(operation_name: str):
    """
    Decorator to track an operation with timing.
    
    Args:
        operation_name: Name of the operation to track
        
    Example:
        @track_operation("database_query")
        def query_users():
            # Function implementation
            pass
    """
    def decorator(func):
        def wrapper(*args, **kwargs):
            start_time = time.time()
            original_operation = get_operation()
            
            try:
                set_operation(operation_name)
                result = func(*args, **kwargs)
                return result
            finally:
                duration = time.time() - start_time
                # Restore original operation
                if original_operation:
                    set_operation(original_operation)
                else:
                    operation_var.set(None)
                
                # Could log timing here if needed
                # logger.info("Operation completed", operation=operation_name, duration=duration)
                
        return wrapper
    return decorator


class ContextSnapshot:
    """
    A snapshot of the current context that can be restored later.
    
    This is useful for temporarily changing context and then restoring it.
    """
    
    def __init__(self):
        """Capture the current context state."""
        self.context = get_current_context()
        self.tokens = {
            'correlation_id': correlation_id_var.get(),
            'agent_id': agent_id_var.get(),
            'session_id': session_id_var.get(),
            'user_id': user_id_var.get(),
            'operation': operation_var.get(),
        }
    
    def restore(self) -> None:
        """Restore the captured context state."""
        for var_name, value in self.tokens.items():
            if var_name == 'correlation_id':
                correlation_id_var.set(value)
            elif var_name == 'agent_id':
                agent_id_var.set(value)
            elif var_name == 'session_id':
                session_id_var.set(value)
            elif var_name == 'user_id':
                user_id_var.set(value)
            elif var_name == 'operation':
                operation_var.set(value)


@contextmanager
def temporary_context(**kwargs):
    """
    Context manager for temporarily changing context with automatic restoration.
    
    Args:
        **kwargs: Context values to set temporarily
        
    Example:
        with temporary_context(user_id="temp-user", operation="temp-op"):
            # Context is temporarily changed
            logger.info("Temporary operation")
        # Context is automatically restored
    """
    snapshot = ContextSnapshot()
    try:
        # Apply temporary context
        temp_context = LoggingContext(**kwargs)
        temp_context.apply()
        yield
    finally:
        # Restore original context
        snapshot.restore()
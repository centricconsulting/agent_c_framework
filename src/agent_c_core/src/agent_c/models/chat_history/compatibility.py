"""
Backward Compatibility Layer for ChatSession Migration.

This module provides compatibility wrappers and utilities to ensure existing
code continues to work seamlessly during the migration from ChatSession to
EnhancedChatSession.
"""

import warnings
from typing import List, Dict, Any, Optional, Union, Type
from functools import wraps

from agent_c.models.chat_history.chat_session import ChatSession
from agent_c.models.chat_history.enhanced_chat_session import EnhancedChatSession
from agent_c.models.chat_history.session_migration import migrate_single_session


class CompatibilityChatSession(EnhancedChatSession):
    """
    Compatibility wrapper that extends EnhancedChatSession to provide
    seamless backward compatibility with legacy ChatSession usage patterns.
    
    This class intercepts legacy usage patterns and provides appropriate
    warnings while maintaining functionality.
    """
    
    def __init__(self, **kwargs):
        """Initialize with automatic migration support."""
        # Handle legacy initialization patterns
        if "messages" in kwargs and isinstance(kwargs["messages"], list):
            legacy_messages = kwargs.pop("messages")
            super().__init__(**kwargs)
            
            # Migrate legacy messages if provided
            if legacy_messages:
                self.migrate_from_legacy(legacy_messages)
        else:
            super().__init__(**kwargs)
    
    @property
    def messages(self) -> List[dict[str, Any]]:
        """
        Backward compatibility property for accessing messages.
        
        Issues a deprecation warning to encourage migration to new methods.
        """
        warnings.warn(
            "Direct access to 'messages' property is deprecated. "
            "Use get_messages_for_llm() or get_all_messages() instead. "
            "Consider migrating to EnhancedChatSession for full functionality.",
            DeprecationWarning,
            stacklevel=2
        )
        return super().messages
    
    @messages.setter
    def messages(self, value: List[dict[str, Any]]) -> None:
        """
        Backward compatibility setter with deprecation warning.
        """
        warnings.warn(
            "Direct assignment to 'messages' property is deprecated. "
            "Use add_message_to_current_interaction() or migrate_from_legacy() instead.",
            DeprecationWarning,
            stacklevel=2
        )
        super(CompatibilityChatSession, self.__class__).messages.fset(self, value)
    
    def append_message(self, message: dict[str, Any]) -> None:
        """
        Legacy method for appending messages.
        
        Args:
            message: Legacy message dictionary
        """
        warnings.warn(
            "append_message() is deprecated. Use add_message_to_current_interaction() instead.",
            DeprecationWarning,
            stacklevel=2
        )
        
        # Convert to enhanced message and add to current interaction
        try:
            if self.current_interaction_id is None:
                self.start_interaction()
            
            enhanced_msg = self._convert_dict_to_enhanced_message(
                message, 
                self.current_interaction_id
            )
            self.add_message_to_current_interaction(enhanced_msg)
        except Exception:
            # Fallback to legacy behavior
            current_messages = self.messages
            current_messages.append(message)
            self.messages = current_messages


def compatibility_wrapper(func):
    """
    Decorator to add compatibility warnings to deprecated methods.
    """
    @wraps(func)
    def wrapper(*args, **kwargs):
        warnings.warn(
            f"Method {func.__name__} is deprecated and may not work correctly "
            f"with EnhancedChatSession. Consider updating your code.",
            DeprecationWarning,
            stacklevel=2
        )
        return func(*args, **kwargs)
    return wrapper


class SessionManagerCompatibility:
    """
    Compatibility utilities for session managers dealing with mixed session types.
    """
    
    @staticmethod
    def ensure_enhanced_session(
        session: Union[ChatSession, EnhancedChatSession]
    ) -> EnhancedChatSession:
        """
        Ensure a session is an EnhancedChatSession, migrating if necessary.
        
        Args:
            session: Either a legacy ChatSession or EnhancedChatSession
            
        Returns:
            EnhancedChatSession instance
        """
        if isinstance(session, EnhancedChatSession):
            return session
        elif isinstance(session, ChatSession):
            # Migrate legacy session
            enhanced_session, result = migrate_single_session(session, validate=False)
            if result.success and enhanced_session:
                return enhanced_session
            else:
                # Migration failed, create compatibility wrapper
                return CompatibilityChatSession(
                    session_id=session.session_id,
                    token_count=session.token_count,
                    context_window_size=session.context_window_size,
                    session_name=session.session_name,
                    created_at=session.created_at,
                    updated_at=session.updated_at,
                    deleted_at=session.deleted_at,
                    user_id=session.user_id,
                    metadata=session.metadata,
                    agent_config=session.agent_config,
                    messages=session.messages or []
                )
        else:
            raise TypeError(f"Expected ChatSession or EnhancedChatSession, got {type(session)}")
    
    @staticmethod
    def get_messages_compatible(
        session: Union[ChatSession, EnhancedChatSession]
    ) -> List[dict[str, Any]]:
        """
        Get messages from any session type in legacy format.
        
        Args:
            session: Session of any supported type
            
        Returns:
            Messages in legacy dict format
        """
        if isinstance(session, EnhancedChatSession):
            return session.get_messages_for_llm()
        elif isinstance(session, ChatSession):
            return session.messages or []
        else:
            raise TypeError(f"Unsupported session type: {type(session)}")
    
    @staticmethod
    def add_message_compatible(
        session: Union[ChatSession, EnhancedChatSession],
        message: dict[str, Any]
    ) -> None:
        """
        Add a message to any session type.
        
        Args:
            session: Session of any supported type
            message: Message dictionary to add
        """
        if isinstance(session, EnhancedChatSession):
            # Ensure there's a current interaction
            if session.current_interaction_id is None:
                session.start_interaction()
            
            # Convert and add message
            enhanced_msg = session._convert_dict_to_enhanced_message(
                message, 
                session.current_interaction_id
            )
            session.add_message_to_current_interaction(enhanced_msg)
        elif isinstance(session, ChatSession):
            if session.messages is None:
                session.messages = []
            session.messages.append(message)
        else:
            raise TypeError(f"Unsupported session type: {type(session)}")


class DeprecationManager:
    """
    Manager for handling deprecation warnings and migration guidance.
    """
    
    _warned_methods = set()
    
    @classmethod
    def warn_once(cls, method_name: str, message: str) -> None:
        """
        Issue a deprecation warning only once per method.
        
        Args:
            method_name: Name of the deprecated method
            message: Deprecation warning message
        """
        if method_name not in cls._warned_methods:
            warnings.warn(message, DeprecationWarning, stacklevel=3)
            cls._warned_methods.add(method_name)
    
    @classmethod
    def reset_warnings(cls) -> None:
        """Reset the warning tracking (useful for testing)."""
        cls._warned_methods.clear()


def create_migration_guide() -> str:
    """
    Generate a comprehensive migration guide for users.
    
    Returns:
        Formatted migration guide text
    """
    return """
ChatSession to EnhancedChatSession Migration Guide
=================================================

The Agent C framework is migrating from ChatSession to EnhancedChatSession
to provide advanced features like interaction tracking, tool optimization,
and automatic work log generation.

## Quick Migration Steps:

1. Replace ChatSession imports:
   OLD: from agent_c.models.chat_history import ChatSession
   NEW: from agent_c.models.chat_history import EnhancedChatSession

2. Update session creation:
   OLD: session = ChatSession(session_id="test", messages=[...])
   NEW: session = EnhancedChatSession(session_id="test")
        session.migrate_from_legacy([...])

3. Update message access:
   OLD: messages = session.messages
   NEW: messages = session.get_messages_for_llm()

4. Update message addition:
   OLD: session.messages.append(message)
   NEW: session.add_message_to_current_interaction(enhanced_message)

## Automatic Migration:

For bulk migration of existing sessions:

```python
from agent_c.models.chat_history import migrate_sessions_with_progress

# Migrate multiple sessions
result = migrate_sessions_with_progress(legacy_sessions, show_progress=True)
print(f"Migrated {result.successful_migrations} sessions")
```

## Backward Compatibility:

The framework provides automatic compatibility for existing code:
- Legacy ChatSession continues to work
- Automatic migration when mixed with EnhancedChatSession
- Deprecation warnings guide you to new APIs

## New Features Available:

- Interaction boundary tracking
- Tool-driven message optimization
- Automatic work log generation
- Advanced message filtering and search
- Observable pattern integration for real-time UI updates

## Need Help?

- Check deprecation warnings for specific guidance
- Use CompatibilityChatSession for gradual migration
- Refer to the enhanced session documentation for new features
"""


# Convenience functions for common compatibility scenarios

def ensure_compatible_session(session: Any) -> EnhancedChatSession:
    """
    Ensure any session object is compatible with new APIs.
    
    Args:
        session: Session object of any type
        
    Returns:
        EnhancedChatSession instance
    """
    return SessionManagerCompatibility.ensure_enhanced_session(session)


def get_legacy_messages(session: Any) -> List[dict[str, Any]]:
    """
    Get messages in legacy format from any session type.
    
    Args:
        session: Session object of any type
        
    Returns:
        Messages in legacy dict format
    """
    return SessionManagerCompatibility.get_messages_compatible(session)


def add_legacy_message(session: Any, message: dict[str, Any]) -> None:
    """
    Add a legacy message to any session type.
    
    Args:
        session: Session object of any type
        message: Message dictionary to add
    """
    SessionManagerCompatibility.add_message_compatible(session, message)


# Migration status checking
def check_session_migration_status(session: Any) -> str:
    """
    Check the migration status of a session.
    
    Args:
        session: Session to check
        
    Returns:
        Migration status string
    """
    if isinstance(session, EnhancedChatSession):
        return getattr(session, '_migration_status', 'enhanced')
    elif isinstance(session, ChatSession):
        return 'legacy'
    else:
        return 'unknown'


def is_session_enhanced(session: Any) -> bool:
    """
    Check if a session is using the enhanced format.
    
    Args:
        session: Session to check
        
    Returns:
        True if session is enhanced, False otherwise
    """
    return isinstance(session, EnhancedChatSession) and \
           getattr(session, '_migration_status', 'enhanced') == 'enhanced'
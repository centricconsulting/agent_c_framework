"""
Stream Key Management for Redis Streams

Provides centralized management of Redis Stream keys with hierarchical naming
convention and validation for the Agent C system.
"""

import re
from typing import Tuple


class StreamKeyManager:
    """Centralized stream key management and validation."""
    
    NAMESPACE_PREFIX = "agent_c:stream:"
    
    @classmethod
    def build_stream_key(cls, session_id: str, interaction_id: str) -> str:
        """
        Build a standardized stream key.
        
        Args:
            session_id: Session identifier
            interaction_id: Interaction identifier
            
        Returns:
            Standardized Redis Stream key
            
        Example:
            >>> StreamKeyManager.build_stream_key("user_123", "chat_001")
            "agent_c:stream:user_123:chat_001"
        """
        # Validate and sanitize inputs
        session_id = cls._sanitize_id(session_id)
        interaction_id = cls._sanitize_id(interaction_id)
        
        return f"{cls.NAMESPACE_PREFIX}{session_id}:{interaction_id}"
    
    @classmethod
    def parse_stream_key(cls, stream_key: str) -> Tuple[str, str]:
        """
        Parse session_id and interaction_id from stream key.
        
        Args:
            stream_key: Redis Stream key to parse
            
        Returns:
            Tuple of (session_id, interaction_id)
            
        Raises:
            ValueError: If stream key format is invalid
            
        Example:
            >>> StreamKeyManager.parse_stream_key("agent_c:stream:user_123:chat_001")
            ("user_123", "chat_001")
        """
        if not stream_key.startswith(cls.NAMESPACE_PREFIX):
            raise ValueError(f"Invalid stream key format: {stream_key}")
        
        suffix = stream_key[len(cls.NAMESPACE_PREFIX):]
        parts = suffix.split(":", 1)
        
        if len(parts) != 2:
            raise ValueError(f"Invalid stream key format: {stream_key}")
        
        return parts[0], parts[1]
    
    @classmethod
    def _sanitize_id(cls, id_value: str) -> str:
        """
        Sanitize ID values for Redis key safety.
        
        Args:
            id_value: ID value to sanitize
            
        Returns:
            Sanitized ID safe for Redis keys
        """
        if not id_value:
            raise ValueError("ID value cannot be empty")
        
        # Remove problematic characters for Redis keys
        # Keep alphanumeric, underscore, and hyphen
        sanitized = re.sub(r'[^a-zA-Z0-9_\-]', '_', id_value)
        
        # Ensure it doesn't start or end with underscore
        sanitized = sanitized.strip('_')
        
        if not sanitized:
            raise ValueError(f"ID value '{id_value}' cannot be sanitized to valid format")
        
        return sanitized
    
    @classmethod
    def is_valid_stream_key(cls, stream_key: str) -> bool:
        """
        Check if a stream key is valid.
        
        Args:
            stream_key: Redis Stream key to validate
            
        Returns:
            True if valid, False otherwise
        """
        try:
            cls.parse_stream_key(stream_key)
            return True
        except ValueError:
            return False
    
    @classmethod
    def get_session_pattern(cls, session_id: str) -> str:
        """
        Get Redis key pattern for all streams in a session.
        
        Args:
            session_id: Session identifier
            
        Returns:
            Redis key pattern for session streams
            
        Example:
            >>> StreamKeyManager.get_session_pattern("user_123")
            "agent_c:stream:user_123:*"
        """
        sanitized_session_id = cls._sanitize_id(session_id)
        return f"{cls.NAMESPACE_PREFIX}{sanitized_session_id}:*"
    
    @classmethod
    def get_all_streams_pattern(cls) -> str:
        """
        Get Redis key pattern for all Agent C streams.
        
        Returns:
            Redis key pattern for all Agent C streams
        """
        return f"{cls.NAMESPACE_PREFIX}*"
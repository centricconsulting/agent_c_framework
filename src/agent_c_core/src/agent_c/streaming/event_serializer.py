"""
Event Serialization for Redis Streams

Handles serialization and deserialization of ChatEvent objects for Redis Streams
storage, ensuring consistent format and proper handling of complex objects.
"""

import json
import uuid
from datetime import datetime
from typing import Dict, Any, Tuple, Optional
from dataclasses import dataclass

from agent_c.models import ChatEvent


@dataclass
class EventContext:
    """Context information for event processing."""
    session_id: str
    interaction_id: str
    user_id: str = "unknown"
    sequence: int = 0
    source: str = "BaseAgent"
    trace_id: str = ""
    correlation_id: str = ""
    retry_count: int = 0
    error_info: Optional[Dict[str, Any]] = None
    
    def __post_init__(self):
        if not self.trace_id:
            self.trace_id = str(uuid.uuid4())
        if not self.correlation_id:
            self.correlation_id = str(uuid.uuid4())


class EventSerializer:
    """Handles serialization/deserialization of events for Redis Streams."""
    
    VERSION = "1.0"
    
    @classmethod
    def serialize_event(cls, event: ChatEvent, context: EventContext) -> Dict[str, str]:
        """
        Serialize a ChatEvent for Redis Stream storage.
        
        Args:
            event: ChatEvent to serialize
            context: Event context information
            
        Returns:
            Dictionary suitable for Redis Stream XADD
            
        Note:
            All values in the returned dict are strings as required by Redis Streams
        """
        
        # Extract core event data
        event_data = {
            "type": event.type,
            "data": event.to_dict() if hasattr(event, 'to_dict') else cls._event_to_dict(event),
            "metadata": getattr(event, 'metadata', {}) or {}
        }
        
        # Add timestamp if event has one
        timestamp = getattr(event, 'timestamp', None)
        if timestamp:
            event_data["timestamp"] = timestamp.isoformat() if hasattr(timestamp, 'isoformat') else str(timestamp)
        
        # Build stream message - all values must be strings for Redis
        message = {
            "event_type": str(event.type),
            "event_id": str(uuid.uuid4()),
            "event_data": json.dumps(event_data, default=cls._json_serializer),
            "event_version": cls.VERSION,
            
            "session_id": str(context.session_id),
            "interaction_id": str(context.interaction_id),
            "user_id": str(context.user_id),
            
            "sequence": str(context.sequence),
            "timestamp": timestamp.isoformat() if timestamp and hasattr(timestamp, 'isoformat') else datetime.now().isoformat(),
            "processing_time": datetime.now().isoformat(),
            
            "source": str(context.source),
            "trace_id": str(context.trace_id),
            "correlation_id": str(context.correlation_id),
        }
        
        # Add optional fields
        if context.retry_count > 0:
            message["retry_count"] = str(context.retry_count)
        
        if context.error_info:
            message["error_info"] = json.dumps(context.error_info)
        
        return message
    
    @classmethod
    def deserialize_event(cls, stream_message: Dict[str, str]) -> Tuple[Dict[str, Any], EventContext]:
        """
        Deserialize a Redis Stream message back to event data and context.
        
        Args:
            stream_message: Redis Stream message data
            
        Returns:
            Tuple of (event_data, context)
            
        Note:
            Returns event data as dict since we may not have all event classes available
        """
        
        # Parse event data
        event_data = json.loads(stream_message["event_data"])
        
        # Reconstruct context
        context = EventContext(
            session_id=stream_message["session_id"],
            interaction_id=stream_message["interaction_id"],
            user_id=stream_message.get("user_id", "unknown"),
            sequence=int(stream_message.get("sequence", "0")),
            source=stream_message.get("source", "unknown"),
            trace_id=stream_message.get("trace_id", ""),
            correlation_id=stream_message.get("correlation_id", ""),
            retry_count=int(stream_message.get("retry_count", "0")),
            error_info=json.loads(stream_message["error_info"]) if stream_message.get("error_info") else None
        )
        
        return event_data, context
    
    @classmethod
    def _event_to_dict(cls, event: ChatEvent) -> Dict[str, Any]:
        """
        Convert a ChatEvent to dictionary format.
        
        Args:
            event: ChatEvent to convert
            
        Returns:
            Dictionary representation of the event
        """
        if hasattr(event, '__dict__'):
            result = {}
            for key, value in event.__dict__.items():
                if not key.startswith('_'):  # Skip private attributes
                    result[key] = value
            return result
        else:
            # Fallback for events that don't have __dict__
            return {"type": getattr(event, 'type', 'unknown')}
    
    @staticmethod
    def _json_serializer(obj):
        """
        Custom JSON serializer for complex objects.
        
        Args:
            obj: Object to serialize
            
        Returns:
            JSON-serializable representation
        """
        if isinstance(obj, datetime):
            return obj.isoformat()
        elif isinstance(obj, uuid.UUID):
            return str(obj)
        elif hasattr(obj, 'to_dict'):
            return obj.to_dict()
        elif hasattr(obj, '__dict__'):
            return obj.__dict__
        else:
            return str(obj)
    
    @classmethod
    def create_stream_metadata_event(cls, event_type: str, session_id: str, interaction_id: str, **metadata) -> Dict[str, str]:
        """
        Create a metadata event for stream lifecycle management.
        
        Args:
            event_type: Type of metadata event (stream_created, stream_closed, etc.)
            session_id: Session identifier
            interaction_id: Interaction identifier
            **metadata: Additional metadata
            
        Returns:
            Redis Stream message for metadata event
        """
        message = {
            "event_type": event_type,
            "event_id": str(uuid.uuid4()),
            "session_id": session_id,
            "interaction_id": interaction_id,
            "timestamp": datetime.now().isoformat(),
            "event_version": cls.VERSION,
            "metadata": "true"
        }
        
        # Add additional metadata
        for key, value in metadata.items():
            message[key] = str(value)
        
        return message
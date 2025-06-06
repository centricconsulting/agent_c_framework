from typing import Optional, Dict, Any
from datetime import datetime

# This file provides extensions to the ChatEvent class from agent_c.models
# to support Redis stream IDs

def extend_chat_event():
    """
    Extends the ChatEvent class from agent_c.models to support Redis stream IDs.
    
    This function is called during application startup to patch the ChatEvent class
    with Redis stream ID support.
    """
    from agent_c.models import ChatEvent
    
    # Check if the class is already extended
    if hasattr(ChatEvent, 'get_stream_id'):
        return
    
    # Add stream_id attribute and methods
    setattr(ChatEvent, '_stream_id', None)
    
    def set_stream_id(self, stream_id: str) -> None:
        """
        Set the Redis stream ID for this event.
        
        Args:
            stream_id: The Redis stream ID
        """
        self._stream_id = stream_id
    
    def get_stream_id(self) -> Optional[str]:
        """
        Get the Redis stream ID for this event.
        
        Returns:
            The Redis stream ID or None if not set
        """
        return getattr(self, '_stream_id', None)
    
    # Original to_dict method backup
    original_to_dict = getattr(ChatEvent, 'to_dict', None)
    
    def extended_to_dict(self) -> Dict[str, Any]:
        """
        Convert the event to a dictionary, including the stream_id if available.
        
        Returns:
            Dictionary representation of the event
        """
        if original_to_dict:
            result = original_to_dict(self)
        else:
            # Basic implementation if original doesn't exist
            result = {
                "type": self.type,
                "session_id": self.session_id,
                "timestamp": datetime.now().isoformat(),
            }
        
        # Include stream_id if available
        stream_id = getattr(self, '_stream_id', None)
        if stream_id is not None:
            result["stream_id"] = stream_id
            
        return result
    
    # Add methods to class
    setattr(ChatEvent, 'set_stream_id', set_stream_id)
    setattr(ChatEvent, 'get_stream_id', get_stream_id)
    
    # Replace to_dict with extended version if it exists
    if original_to_dict:
        setattr(ChatEvent, 'to_dict', extended_to_dict)
from pydantic import Field
from typing import Optional

from agent_c.models.events.base import BaseEvent

class SessionEvent(BaseEvent):
    """
    Session events are generated within a chat session and always include a session_id and role.
    Anything that derives from this base is safe for routing based on the session_id in a multiuser
    environment.
    """
    session_id: str = Field(..., description="The type of the event")
    role: str = Field(..., description="The role that triggered this event event")

class SemiSessionEvent(BaseEvent):
    """
    Semi-Session events are generated within a chat session, but may also be generated by objects
    that do not have access to the session_id and role outside the core event stream and the
    consumer of the event will fill in the session_id and role, then forward the event to the event stream
    """
    session_id: Optional[str] = Field(None, description="The type of the event")
    role: Optional[str] = Field(None, description="The role that triggered this event event")
from pydantic import Field
from typing import Optional

from agent_c.models.events.base import BaseEvent

class SessionEvent(BaseEvent):
    """
    Session events are generated within a chat session and always include a session_id and role.
    Anything that derives from this base is safe for routing based on the session_id in a multiuser
    environment.
    """
    session_id: Optional[str] = Field(None, description="The type of the event")
    role: Optional[str] = Field(None, description="The role that triggered this event event")
    model_name: Optional[str] = Field(None, description="The name of the model that triggered this event")
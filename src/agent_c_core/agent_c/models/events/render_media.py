from typing import Optional

from pydantic import ConfigDict, Field

from agent_c_core.agent_c.models.events.session_event import SessionEvent

class RenderMediaEvent(SessionEvent):
    """
    Set when the agent or tool would like to render media to the user.
    """
    def __init__(self, **data):
        super().__init__(type = "render_media", **data)

    model_config = ConfigDict(populate_by_name=True)
    content_type: str = Field(..., alias="content-type")
    url: Optional[str] = None
    name: Optional[str] = None
    content: Optional[str] = None
    content_bytes: Optional[bytes] = None
    sent_by_class: Optional[str] = None
    sent_by_function: Optional[str] = None
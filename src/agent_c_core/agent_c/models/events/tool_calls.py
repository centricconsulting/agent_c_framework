from pydantic import Field
from typing import Optional, List
from agent_c_core.agent_c.models.events.session_event import SessionEvent

class ToolCallEvent(SessionEvent):
    """
    Sent to notify the UI that tool use has been initiated or completed
    """
    active: bool = Field(..., description="If True, tools will be run immediately after the even goes out.")
    vendor: str = Field(..., description="The completion API vendor.")
    tool_calls: List[dict] = Field(..., description="A list of tool calls to be made. Currently in vendor format")
    tool_results: Optional[List[dict]] = Field(None, description="A list of tool results. Currently in vendor format")

    def __init__(self, **data):
        super().__init__(type = "tool_call", **data)

class ToolCallDeltaEvent(SessionEvent):
    """
    Sent to notify the UI that a chunk of tool_calls content has been received.
    - Clients should handle this event by appending the content to the current message,
    """
    def __init__(self, **data):
        super().__init__(type = "tool_select_delta", **data)

    tool_calls: Optional[list[dict]] = Field(..., description="The current tool calls list.")
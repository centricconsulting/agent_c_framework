import uuid
import datetime
from typing import Optional

from pydantic import Field
from agent_c_core.agent_c.models.base import BaseModel


class ChatSummary(BaseModel):
    """
    Represents a summary of a conversation.

    Attributes
    ----------
    uuid : str
        The unique identifier of the summary.
    created_at : str
        The timestamp of when the summary was created.
    content : str
        The content of the summary.
    recent_message_uuid : str
        The unique identifier of the most recent message in the conversation.
    token_count : int
        The number of tokens in the summary.


    """

    uuid: Optional[str] = Field(default_factory=lambda: str(uuid.uuid4()))
    created_at: Optional[str] = Field(default_factory=lambda: datetime.datetime.now().isoformat())
    content: str = Field("Content is required")
    recent_message_uuid: str = Field("A recent_message_uuid is required")
    token_count: int = Field("A token_count is required")
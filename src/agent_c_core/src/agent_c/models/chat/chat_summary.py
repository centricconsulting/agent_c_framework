import uuid
import datetime
from typing import Optional

from pydantic import Field
from agent_c.models.base import BaseModel
from agent_c.util import MnemonicSlugs


class ChatSummary(BaseModel):
    """
    Represents a summary of a conversation.

    Attributes
    ----------
    id : str
        The unique identifier of the summary.
    created_at : str
        The timestamp of when the summary was created.
    content : str
        The content of the summary.
    recent_message_id : str
        The unique identifier of the most recent message in the conversation.
    token_count : int
        The number of tokens in the summary.
    """
    id: str = Field(default_factory=lambda: MnemonicSlugs.generate_slug(3))
    created_at: str = Field(default_factory=lambda: datetime.datetime.now().isoformat())
    content: str = Field(..., description="The content of the summary")
    recent_message_id: str = Field(..., description="The most recent message ID in the conversation when this was generated.")
    token_count: int = Field(..., description="The number of tokens in the summary")

import uuid
import datetime
from pydantic import Field
from typing import Optional, Any, Dict, List

from agent_c_core.agent_c.models.base import BaseModel
from agent_c_core.agent_c.models.chat_history.chat_summary import ChatSummary
from agent_c_core.agent_c.models.chat_history.memory_message import MemoryMessage

class ChatMemory(BaseModel):
    """
    Represents a memory object with messages, metadata, and other attributes.

    Attributes
    ----------
    messages : Optional[List[Dict[str, Any]]]
        A list of message objects, where each message contains a role and content.
    metadata : Optional[Dict[str, Any]]
        A dictionary containing metadata associated with the memory.
    summary : Optional[Summary]
        A Summary object.
    uuid : Optional[str]
        A unique identifier for the memory.
    created_at : Optional[str]
        The timestamp when the memory was created.
    token_count : Optional[int]
        The token count of the memory.

    Methods
    -------
    to_dict() -> Dict[str, Any]:
        Returns a dictionary representation of the message.
    """

    messages: List[MemoryMessage] = Field(
        default=[], description="A List of Messages or empty List is required"
    )
    metadata: Optional[Dict[str, Any]] = {}
    summary: Optional[ChatSummary] = None
    uuid: Optional[str] = Field(default_factory=lambda: str(uuid.uuid4()))
    created_at: Optional[str] = Field(default_factory=lambda: datetime.datetime.now().isoformat())
    token_count: Optional[int] = None

    @property
    def context(self) -> Optional[str]:
        return None

    @property
    def inference_messages(self) -> List[Dict[str, Any]]:
        return [{"role": d.role, "content": d.content} for d in self.messages]

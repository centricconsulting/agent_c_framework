import uuid
import datetime

from pydantic import Field, field_validator
from typing import Optional, Dict, Any, Union, List

from agent_c.models.base import BaseModel
from agent_c.models.chat.content import ContentTypeUnion, TextContent
from agent_c.util import TokenCounter

class MemoryMessage(BaseModel):
    """
    Represents a message in a conversation. Copied from Zep for compatibility

    Attributes
    ----------
    uuid : str, optional
        The unique identifier of the message.
    created_at : str, optional
        The timestamp of when the message was created.
    role : str
        The role of the sender of the message (e.g., "user", "assistant").
    content : str
        The content of the message.
    token_count : int, optional
        The number of tokens in the message.

    Methods
    -------
    to_dict() -> Dict[str, Any]:
        Returns a dictionary representation of the message.
    """

    role: str
    content: Union[str, List[ContentTypeUnion]]
    uuid: Optional[str] = Field(default_factory=lambda: str(uuid.uuid4()))
    created_at: Optional[str] = Field(default_factory=lambda: datetime.datetime.now().isoformat())
    token_count: Optional[int] = Field(default=None)
    metadata: Optional[Dict[str, Any]] = Field(default=None)

    @field_validator('token_count', mode='before')
    def compute_token_count(cls, value, info):
        """
        Computes the token_count based on the content if token_count is not provided or is None.

        Parameters:
            value (Optional[int]): The current value of token_count.
            info (FieldValidationInfo): Provides access to other fields.

        Returns:
            int: The computed token_count.
        """
        if value is not None:
            return value

        content = info.data.get('content')
        if content is None:
            return 0

        if isinstance(content, str):
            return TokenCounter.count_tokens(content)
        elif isinstance(content, list):
            return sum([item.count_tokens() for item in content])
        else:
            # Default token count if content type is unexpected
            return 0


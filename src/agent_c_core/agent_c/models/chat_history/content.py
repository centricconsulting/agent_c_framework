from typing import Literal, Union, List

from agent_c_core.agent_c.models import BaseModel
from agent_c_core.agent_c.util import TokenCounter

class BaseContent(BaseModel):
    """
    Represents a base content object.

    Attributes
    ----------
    type : str
        The type of content.
    """
    type: str

    def count_tokens(self) -> int:
        return 0

class TextContent(BaseContent):
    """
    Represents a text content object.

    Attributes
    ----------
    text : str
        The text content.
    """
    type: Literal['text']
    text: str

    def count_tokens(self) -> int:
        return TokenCounter.count_tokens(self.text)

class ImageContent(BaseContent):
    """
    Represents an image content object.

    Attributes
    ----------
    url : str
        The URL of the image.
    """
    type: Literal['image']
    url: str

    def count_tokens(self) -> int:
        return TokenCounter.count_tokens(self.url) # This probably isn't right but it's just a placeholder

# Union of content types, used by MemoryMessage and possibly others
ContentTypeUnion = Union[TextContent, ImageContent]

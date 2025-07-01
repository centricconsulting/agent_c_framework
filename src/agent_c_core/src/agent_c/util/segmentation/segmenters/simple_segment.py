import uuid
from typing import Any

from pydantic import BaseModel


class Segment(BaseModel):
    """
    A class representing a segment of text, its associated token count, and a unique identifier.

    Attributes:
        id (str): The unique identifier for the segment.
        content (str): The content of the text segment.
        token_count (int): The number of tokens in the text segment.
    """
    id: str = ""
    content: str
    token_count: int
    source_name: str
    sequence: int

    def __init__(self, **data: Any):
        super().__init__(**data)
        self.id = str(uuid.uuid5(uuid.NAMESPACE_DNS, f"{self.source_name}:{self.sequence}"))


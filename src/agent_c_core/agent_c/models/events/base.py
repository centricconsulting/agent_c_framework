from typing import Any
from pydantic import Field

from agent_c_core.agent_c.util import to_snake_case
from agent_c_core.agent_c.models.base import BaseModel

class BaseEvent(BaseModel):
    """
    A base model for event objects.

    Attributes:
        type (str): The type identifier for the event. Defaults to the snake_case
            version of the class name if not provided.

    Args:
        **data: Arbitrary keyword arguments that will be used to initialize the model.
            If 'type' is not provided in the data, it will be automatically set to
            the snake_case version of the class name, without "event".
    """
    type: str = Field(..., description="The type of the event. Defaults to the snake case class name without event" )

    def __init__(self, **data: Any) -> None:
        if 'type' not in data:
            data['type'] = to_snake_case(self.__class__.__name__.removesuffix('Event'))

        super().__init__(**data)
from pydantic import Field, model_validator

from agent_c.util.string import to_snake_case
from agent_c.models.base import BaseModel

class BaseEvent(BaseModel):
    """
    A base model for event objects.

    Attributes:
        type (str): The type identifier for the event. Defaults to the snake_case
            version of the class name if not provided.
    """
    type: str = Field(None,
                      description="The type of the event. Defaults to the snake case class name without the word event" )

    @model_validator(mode='after')
    def post_init(self):
        if not self.type:
            self.type = to_snake_case(self.__class__.__name__.removesuffix('Event'))

        return self

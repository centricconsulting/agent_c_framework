from typing import Any
from pydantic import Field

from agent_c_core.agent_c.models.base import BaseModel
from agent_c_core.agent_c.util.string import to_snake_case


class BaseInput(BaseModel):
    """
    A base model for input submodels that define the various types of user input.

    Attributes:
        type (str): The type identifier for the input model. Defaults to the snake_case
            version of the class name if not provided.

    Args:
        **data: Arbitrary keyword arguments that will be used to initialize the model.
            If 'type' is not provided in the data, it will be automatically set to
            the snake_case version of the class name minus the word "Input".
    """
    type: str = Field(..., alias="type")
    role: str = Field("user")

    def __init__(self, **data: Any) -> None:
        if 'type' not in data:
            data['type'] = to_snake_case(self.__class__.__name__.removesuffix('Input'))

        super().__init__(**data)

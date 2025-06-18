from typing import Any
from pydantic import Field

from agent_c.models.observable import ObservableModel
from agent_c.util.string import to_snake_case

class BaseContext(ObservableModel):
    type: str = Field(..., description="The type of the context. Defaults to the snake case class name without event")


    def __init__(self, **data: Any) -> None:
        if 'type' not in data:
            data['type'] = to_snake_case(self.__class__.__name__.removesuffix('Context'))

        super().__init__(**data)

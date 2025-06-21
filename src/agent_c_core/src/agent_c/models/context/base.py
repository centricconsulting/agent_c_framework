from typing import Any, Dict
from pydantic import Field

from agent_c.models.observable import ObservableModel
from agent_c.util.string import to_snake_case

class BaseContext(ObservableModel):
    context_type: str = Field(..., description="The type of the context. Defaults to the snake case class name without event")


    def __init__(self, **data: Any) -> None:
        if 'context_type' not in data:
            data['context_type'] = to_snake_case(self.__class__.__name__.removesuffix('Context'))

        super().__init__(**data)

    def __init_subclass__(cls, **kwargs):
        """Automatically register subclasses"""
        super().__init_subclass__(**kwargs)
        from agent_c.util.registries.context_registry import ContextRegistry
        ContextRegistry.register(cls)

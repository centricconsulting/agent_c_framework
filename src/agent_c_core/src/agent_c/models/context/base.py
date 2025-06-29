from pydantic import Field, model_validator

from agent_c.models.async_observable import AsyncObservableModel
from agent_c.util.string import to_snake_case

class BaseContext(AsyncObservableModel):
    context_type: str = Field(None,
                              description="The type of the context. Defaults to the snake case class name without event")

    @model_validator(mode='after')
    def ensure_context_type(self):
        self._init_observable()
        if not self.context_type:
            self.context_type = to_snake_case(self.__class__.__name__.removesuffix('Context'))

        return self

    def __init_subclass__(cls, **kwargs):
        """Automatically register subclasses"""
        super().__init_subclass__(**kwargs)
        from agent_c.util.registries.context_registry import ContextRegistry
        ContextRegistry.register(cls)

from typing import Any, Dict

from agent_c.models.context import BaseContext
from agent_c.util.registries.context_registry import ContextRegistry


class DynamicContext(BaseContext):
    def __init__(self, **data: Any) -> None:
        if 'context_type' not in data:
            raise ValueError("DynamicContext must have 'context_type' field")
        context_type = data.pop('context_type')
        self._dynamic_fields: Dict[str, Any] = {**data}
        super().__init__(context_type=context_type)
        ContextRegistry.register(self.__class__, context_type=context_type)

    def __getattr__(self, name: str) -> Any:
        """Allow obj.key syntax for read-only dictionary access"""
        try:
            return super().__getattr__(name)
        except AttributeError:
            # If parent classes don't have the attribute, try dictionary lookup
            if name in self._dynamic_fields:
                return self._dynamic_fields[name]

            raise AttributeError(f"'{self.__class__.__name__}' object has no attribute '{name}'")


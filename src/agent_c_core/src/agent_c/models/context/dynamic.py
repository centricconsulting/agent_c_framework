from typing import Any, Dict
from agent_c.models.context import BaseContext



class DynamicContext(BaseContext):
    _dynamic_fields: Dict[str, Any]
    def __init__(self, **data: Any) -> None:
        context_type = data.pop('config_type', None)
        if context_type is None:
            raise ValueError("DynamicConfig must have 'config_type'")

        object.__setattr__(self, '_dynamic_fields', data)
        super().__init__(config_type=context_type)

        from agent_c.util.registries.context_registry import ContextRegistry
        ContextRegistry.register(self.__class__, context_type=context_type)

    def __getattr__(self, name: str) -> Any:
        # only called if `name` wasn't found normally
        dynamic = object.__getattribute__(self, '_dynamic_fields')
        if name in dynamic:
            return dynamic[name]
        # fall back to normal behavior:
        raise AttributeError(f"{type(self).__name__!r} object has no attribute {name!r}")


from typing import Any, Dict

from agent_c.models.config.base import BaseConfig
from agent_c.util.registries.config_registry import ConfigRegistry


class DynamicConfig(BaseConfig):
    _dynamic_fields: Dict[str, Any]

    def __init__(self, **data: Any) -> None:
        config_type = data.pop('config_type', None)
        if config_type is None:
            raise ValueError("DynamicConfig must have 'config_type'")

        object.__setattr__(self, '_dynamic_fields', data)
        super().__init__(config_type=config_type)
        ConfigRegistry.register(self.__class__, config_type=config_type)

    def __getattr__(self, name: str) -> Any:
        # only called if `name` wasn't found normally
        dynamic = object.__getattribute__(self, '_dynamic_fields')
        if name in dynamic:
            return dynamic[name]
        # fall back to normal behavior:
        raise AttributeError(f"{type(self).__name__!r} object has no attribute {name!r}")


class DynamicRuntimeConfig(DynamicConfig):
    """
    Dynamic configuration for runtime parameters.
    This allows for flexible addition of runtime-specific fields.
    """
    category: str = "runtime"

    def __init__(self, **data: Any) -> None:
        super().__init__(**data)
        ConfigRegistry.register(self.__class__, config_type=self.config_type)

class DynamicToolsetConfig(DynamicConfig):
    """
    Dynamic configuration for toolsets.
    This allows for flexible addition of toolset-specific fields.
    """
    category: str = "toolsets"

    def __init__(self, **data: Any) -> None:
        super().__init__(**data)
        ConfigRegistry.register(self.__class__, config_type=self.config_type)

class DynamicCoreConfig(DynamicConfig):
    """
    Dynamic configuration for core settings.
    This allows for flexible addition of core-specific fields.
    """
    category: str = "core"

    def __init__(self, **data: Any) -> None:
        super().__init__(**data)
        ConfigRegistry.register(self.__class__, config_type=self.config_type)

class DynamicApiConfig(DynamicConfig):
    """
    Dynamic configuration for API settings.
    This allows for flexible addition of API-specific fields.
    """
    category: str = "api"

    def __init__(self, **data: Any) -> None:
        super().__init__(**data)
        ConfigRegistry.register(self.__class__, config_type=self.config_type)

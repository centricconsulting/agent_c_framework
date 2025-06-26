from typing import Any, Dict, ClassVar

from agent_c.models.config.base import BaseConfig
from agent_c.util.registries.config_registry import ConfigRegistry


class BaseDynamicConfig(BaseConfig):
    _dynamic_fields: Dict[str, Any]
    auto_register: ClassVar[bool] = False

    def __init__(self, **data: Any) -> None:
        config_type = data.pop('config_type', None)
        if config_type is None:
            raise ValueError("BaseDynamicConfig must have 'config_type'")

        object.__setattr__(self, '_dynamic_fields', data)
        super().__init__(config_type=config_type)
        ConfigRegistry.register(self.__class__, config_type=config_type)

    def __getattr__(self, name: str) -> Any:
        dynamic = object.__getattribute__(self, '_dynamic_fields')
        if name in dynamic:
            return dynamic[name]

        raise AttributeError(f"{type(self).__name__!r} object has no attribute {name!r}")


class BaseDynamicRuntimeConfig(BaseDynamicConfig):
    """
    Dynamic configuration for runtime parameters.
    This allows for flexible addition of runtime-specific fields.
    """
    category: str = "runtime"

class BaseDynamicToolsetConfig(BaseDynamicConfig):
    """
    Dynamic configuration for toolsets.
    This allows for flexible addition of toolset-specific fields.
    """
    category: str = "toolsets"

class BaseDynamicCoreConfig(BaseDynamicConfig):
    """
    Dynamic configuration for core settings.
    This allows for flexible addition of core-specific fields.
    """
    category: str = "core"


class BaseDynamicApiConfig(BaseDynamicConfig):
    """
    Dynamic configuration for API settings.
    This allows for flexible addition of API-specific fields.
    """
    category: str = "api"

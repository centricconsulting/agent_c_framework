from typing import Any, Dict

from agent_c.models.config.base import BaseConfig
from agent_c.util.registries.config_registry import ConfigRegistry


class DynamicConfig(BaseConfig):
    def __init__(self, **data: Any) -> None:
        if 'config_type' not in data:
            raise ValueError("DynamicConfig must have 'config_type' field")
        config_type = data.pop('config_type')
        self._dynamic_fields: Dict[str, Any] = {**data}
        super().__init__(config_type=config_type)
        ConfigRegistry.register(self.__class__, config_type=config_type)

    def __getattr__(self, name: str) -> Any:
        """Allow obj.key syntax for read-only dictionary access"""
        try:
            return super().__getattr__(name)
        except AttributeError:
            # If parent classes don't have the attribute, try dictionary lookup
            if name in self._dynamic_fields:
                return self._dynamic_fields[name]

            raise AttributeError(f"'{self.__class__.__name__}' object has no attribute '{name}'")


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

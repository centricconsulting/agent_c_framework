from typing import Any, Dict, ClassVar

from pydantic import Field

from agent_c.models.config.base import BaseConfig


class BaseDynamicConfig(BaseConfig):
    dynamic_fields: Dict[str, Any] = Field(default_factory=dict,
                                           description="Dynamic fields for the configuration",
                                           exclude=True)
    auto_register: ClassVar[bool] = False

    def __init__(self, config_type=None,**data: Any) -> None:
        object.__setattr__(self, 'dynamic_fields', data)
        super().__init__(config_type=config_type, **data)


    def __getattr__(self, name: str) -> Any:
        try:
            return self.dynamic_fields[name]
        except KeyError:
            raise AttributeError(f"{type(self).__name__!r} object has no attribute or key {name!r}")

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

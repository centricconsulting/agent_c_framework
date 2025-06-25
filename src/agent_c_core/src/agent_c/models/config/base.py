from typing import Any
from pydantic import Field

from agent_c.models.observable import ObservableModel
from agent_c.util.string import to_snake_case

CONFIG_RUNTIME = "runtimes"
CONFIG_TOOLSETS = "toolsets"
CONFIG_CORE = "core"
CONFIG_API = "api"
CONFIG_MISC = "misc"

class BaseConfig(ObservableModel):
    config_type: str = Field(None, description="The type of the config. Defaults to the snake case class name without config")
    category: str = Field(CONFIG_MISC, description="The high level category of the config, used for grouping.")

    def __init__(self, **data: Any) -> None:
        if 'config_type' not in data:
            data['config_type'] = to_snake_case(self.__class__.__name__.removesuffix('Config'))

        super().__init__(**data)

    def __init_subclass__(cls, **kwargs):
        """Automatically register subclasses"""
        super().__init_subclass__(**kwargs)
        from agent_c.util.registries.config_registry import ConfigRegistry
        ConfigRegistry.register(cls)

class BaseRuntimeConfig(BaseConfig):
    """
    Base class for runtime configurations.
    """
    category: str = Field(CONFIG_RUNTIME, description="The high level category of the config, used for grouping.")

class BaseToolsetConfig(BaseConfig):
    """
    Base class for toolset configurations.
    """
    category: str = Field(CONFIG_TOOLSETS, description="The high level category of the config, used for grouping.")

class  BaseCoreConfig(BaseConfig):
    """
    Base class for core configurations.
    """
    category: str = Field(CONFIG_CORE, description="The high level category of the config, used for grouping.")

class  BaseApiConfig(BaseConfig):
    """
    Base class for API configurations.
    """
    category: str = Field(CONFIG_API, description="The high level category of the config, used for grouping.")

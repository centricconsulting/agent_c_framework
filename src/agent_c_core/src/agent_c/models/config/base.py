from typing import Any
from pydantic import Field

from agent_c.models.observable import ObservableModel
from agent_c.util.string import to_snake_case

CONFIG_RUNTIME = "runtimes"
CONFIG_TOOLSETS = "toolsets"
CONFIG_CORE = "core"
CONFIG_API = "api"
CONFIG_MISC = "misc"

SKIP_REGISTRY_LIST = []

def do_not_register_config(class_name: str) -> None:
    """
    Add a class name to the skip registry list.
    This is used to prevent certain classes from being automatically registered.
    """
    SKIP_REGISTRY_LIST.append(class_name)



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
        if cls._is_auto_registerable(cls):
            from agent_c.util.registries.config_registry import ConfigRegistry
            ConfigRegistry.register(cls)

    @staticmethod
    def _is_auto_registerable(cls) -> bool:
        auto_register = getattr(cls, 'auto_register', True)
        if auto_register is False:
            return False

        if cls.__name__ in SKIP_REGISTRY_LIST or cls.__name__.startswith("Base"):
            return False

        return True

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

from typing import Optional

from pydantic import Field, field_validator

from agent_c.models.async_observable import AsyncObservableModel
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



class BaseConfig(AsyncObservableModel):
    config_type: str = Field(None, description="The type of the config. Defaults to the snake case class name without config")
    category: str = Field(CONFIG_MISC, description="The high level category of the config, used for grouping.")

    @field_validator('config_type', mode='after')
    @classmethod
    def normalize_config_type(cls, value: Optional[str]) -> str:
        """
        Normalize the name to snake_case for consistency.
        This is done after the model is initialized to ensure the name is always in the correct format.
        """
        if value is None or value == '':
            value = cls.__name__
        return to_snake_case(value).removesuffix('_config')

    def __init_subclass__(cls, **kwargs):
        """Automatically register subclasses"""
        super().__init_subclass__(**kwargs)
        if cls._is_auto_registerable(cls):
            from agent_c.util.registries.config_registry import ConfigRegistry
            ConfigRegistry.register(cls)

    @classmethod
    def is_user_level(cls) -> bool:
        """
        Check if the config is user-level.
        User-level configs are those that are not part of the core system and can be modified by users.
        """
        return cls.__name__.endswith("UserConfig") or getattr(cls, 'user_level', False)

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

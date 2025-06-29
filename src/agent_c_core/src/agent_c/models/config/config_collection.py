from typing import Any
from pydantic import BaseModel

from agent_c.util import to_snake_case
from agent_c.util.observable import ObservableDict
from agent_c.util.registries.config_registry import ConfigRegistry

class ConfigCollection(ObservableDict):
    """
    Custom container that handles config serialization/deserialization ensuring
    that config models not known to core cane be instantiated correctly.
    """
    user_level: bool = False  # Indicates that this collection is user-level

    @classmethod
    def validate(cls, v):
        if isinstance(v, cls):
            for key, value in v.items():
                if isinstance(value, BaseModel):
                    v[key] = value
                elif isinstance(value, dict):
                    v[key] = ConfigRegistry.create_config(value, key, getattr(cls, 'user_level', False))
                else:
                    raise ValueError(f"Config value must be BaseModel instance or dict, got {type(value)}")
        elif hasattr(v, 'items'):
            result = {}
            for key, value in v.items():
                norm_key = cls._normalize_key(key)
                if isinstance(value, BaseModel):
                    result[norm_key] = value
                elif isinstance(value, dict):
                    result[norm_key] = ConfigRegistry.create_config(value, norm_key, getattr(cls, 'user_level', False))
                else:
                    raise ValueError(f"Config value must be BaseModel instance or dict, got {type(value)}")
            return cls(result)
        return v

    @staticmethod
    def _normalize_key(item):
        """Extract the logic for normalizing keys into a separate method."""
        if isinstance(item, type):
            return to_snake_case(item.__name__)
        elif isinstance(item, str):
            return to_snake_case(item)
        elif isinstance(item, object):
            return to_snake_case(item.__class__.__name__)
        else:
            return item

    @classmethod
    def __get_pydantic_core_schema__(cls, source_type, handler):
        from pydantic_core import core_schema

        def validate_config_collection(value: Any) -> 'ConfigCollection':
            return cls.validate(value)

        return core_schema.no_info_plain_validator_function(validate_config_collection)


    def __getitem__(self, item) -> Any:
        # if the item is a Class use the snake case of the class name as the key,
        # if it's an instance of ANY object use the snake case of the class name of that object,
        if isinstance(item, type):
            item = to_snake_case(item.__name__)
        elif isinstance(item, str):
            item = to_snake_case(item)
        elif isinstance(item, object):
            item = to_snake_case(item.__class__.__name__)
        try:
            return super().__getitem__(item.removesuffix('_user'))
        except KeyError:
            value = None
            if ConfigRegistry.is_config_registered(item, getattr(self, 'user_level', False)):
                value = ConfigRegistry.create_config({}, item, getattr(self, 'user_level', False))
            elif ConfigRegistry.is_config_registered(f"{item}_user", getattr(self, 'user_level', False)):
                value = ConfigRegistry.create_config({}, f"{item}_user", getattr(self, 'user_level', False))
            if value is not None:
                self[item] = value
                return value

            raise KeyError(f"Config item '{item}' not found in collection and not registered in ConfigRegistry.")

    def __setitem__(self, key, value):
        norm_key: str = self._normalize_key(key)

        if isinstance(value, dict):
            value['config_type'] = norm_key
            value = ConfigRegistry.create_config(value, norm_key, getattr(self, 'user_level', False))
        elif isinstance(value, BaseModel):
            if hasattr(value, 'config_type'):
                value.config_type = norm_key  # Ensure config_type is set
        else:
            raise ValueError(f"ConfigCollection value must be BaseModel instance or dict, got {type(value)}")

        super().__setitem__(norm_key, value)

class UserConfigCollection(ConfigCollection):
    user_level: bool = True

    def __getitem__(self, item) -> Any:
        norm_key: str = self._normalize_key(item)
        return super().__getitem__(norm_key.removesuffix('_user'))

    def __setitem__(self, key, value):
        norm_key: str = self._normalize_key(key)
        super().__setitem__(norm_key.removesuffix('_user'), value)

    @classmethod
    def __get_pydantic_core_schema__(cls, source_type, handler):
        from pydantic_core import core_schema

        def validate_user_config_collection(value: Any) -> 'UserConfigCollection':
            return cls.validate(value)

        return core_schema.no_info_plain_validator_function(validate_user_config_collection)

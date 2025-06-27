from typing import Optional, Dict, Any, Annotated

from pydantic import BaseModel, BeforeValidator, PlainValidator

from agent_c.util import to_snake_case
from agent_c.util.observable import ObservableDict
from agent_c.util.registries.config_registry import ConfigRegistry

class ConfigCollection(ObservableDict):
    """
    Custom container that handles config serialization/deserialization ensuring
    that config models not known to core cane be instantiated correctly.
    """
    user_level: bool = False  # Indicates that this collection is user-level

    def __init__(self, starting_data: Optional[Dict]=None):
        super().__init__()
        for k, v in (starting_data or {}).items():
            self[k] = v

    def __str__(self):
        return f"ConfigCollection({super().__str__()})"

    def __repr__(self):
        return f"ConfigCollection({super().__repr__()})"

    @classmethod
    def __get_validators__(cls):
        yield cls.validate

    @classmethod
    def validate(cls, v):
        if isinstance(v, dict):
            result = {}
            for key, value in v.items():
                if isinstance(value, BaseModel):
                    result[key] = value
                elif isinstance(value, dict):
                    result[key] = ConfigRegistry.create_config(value, key, getattr(cls, 'user_level', False))
                else:
                    raise ValueError(f"Config value must be BaseModel instance or dict, got {type(value)}")
            return cls(result)
        return v

    def __getitem__(self, item):
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
        # if the key is a Class use the snake case of the class name as the key,
        # if it's an instance of ANY object use the snake case of the class name of that object as the key,
        if isinstance(key, type):
            key = to_snake_case(key.__name__)
        elif isinstance(key, str):
            key = to_snake_case(key)
        elif not isinstance(key, str):
            key = to_snake_case(key.__class__.__name__)

        if isinstance(value, dict):
            value['config_type'] = key
            value = ConfigRegistry.create_config(value, key, getattr(self, 'user_level', False))
        elif isinstance(value, BaseModel):
            if hasattr(value, 'config_type'):
                value.config_type = key  # Ensure config_type is set
        else:
            raise ValueError(f"Config value must be BaseModel instance or dict, got {type(value)}")

        super().__setitem__(key.removesuffix('_user'), value)

class UserConfigCollection(ConfigCollection):
    user_level: bool = True

    def __str__(self):
        return f"UserConfigCollection({super().__str__()})"

    def __repr__(self):
        return f"UserConfigCollection({super().__repr__()})"

def ensure_config_collection(v: Any) -> ConfigCollection:
    """Ensure the value is a UserConfigCollection."""
    if isinstance(v, dict):
        return ConfigCollection(v)
    elif isinstance(v, ConfigCollection):
        return v
    elif v is None:
        return ConfigCollection()
    else:
        raise ValueError(f"Expected dict or ConfigCollection, got {type(v)}")

ConfigCollectionField = Annotated[ConfigCollection, PlainValidator(ensure_config_collection)]

def ensure_user_config_collection(v: Any) -> UserConfigCollection:
    """Ensure the value is a UserConfigCollection."""
    if isinstance(v, dict):
        return UserConfigCollection(v)
    elif isinstance(v, UserConfigCollection):
        return v
    elif v is None:
        return UserConfigCollection()
    else:
        raise ValueError(f"Expected dict or UserConfigCollection, got {type(v)}")

UserConfigCollectionField = Annotated[UserConfigCollection, PlainValidator(ensure_user_config_collection)]

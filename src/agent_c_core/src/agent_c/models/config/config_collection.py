from typing import Optional, Dict

from pydantic import BaseModel

from agent_c.models.config.base import BaseConfig
from agent_c.util import to_snake_case
from agent_c.util.observable import ObservableDict
from agent_c.util.registries.config_registry import ConfigRegistry

class ConfigCollection(ObservableDict):
    """
    Custom container that handles config serialization/deserialization ensuring
    that config models not known to core cane be instantiated correctly.
    """

    def __init__(self, starting_data: Optional[Dict]=None):
        super().__init__()
        for k, v in (starting_data or {}).items():
            self[k] = v

    @classmethod
    def __get_validators__(cls):
        yield cls.validate

    @classmethod
    def validate(cls, v):
        if isinstance(v, dict):
            result = {}
            for key, value in v.items():
                if isinstance(value, BaseModel):
                    # Verify it has config_type field if it's not a BaseConfig
                    if not isinstance(value, BaseConfig):
                        if not hasattr(value, 'config_type'):
                            raise ValueError(f"Non-BaseConfig models must have 'config_type' field. Got {type(value)}")
                    result[key] = value
                elif isinstance(value, dict):
                    result[key] = ConfigRegistry.create(value, key)
                else:
                    raise ValueError(f"Config value must be BaseModel instance or dict, got {type(value)}")
            return cls(result)
        return v

    def __getitem__(self, item):
        # if the item is a Class use the snake case of the class name as the key,
        # if it's an instance of ANY object use the snake case of the class name of that object,
        if isinstance(item, type):
            item = to_snake_case(item.__name__)
        elif isinstance(item, object):
            item = to_snake_case(item.__class__.__name__)
        return super().__getitem__(item)

    def __setitem__(self, key, value):
        # if the key is a Class use the snake case of the class name as the key,
        # if it's an instance of ANY object use the snake case of the class name of that object as the key,
        if isinstance(key, type):
            key = to_snake_case(key.__name__)
        elif not isinstance(key, str):
            key = to_snake_case(key.__class__.__name__)

        if isinstance(value, dict):
            value['config_type'] = key
            value = ConfigRegistry.create(value, key)
        elif isinstance(value, BaseModel):
            # Verify it has config_type field if it's not a BaseConfig
            if not isinstance(value, BaseConfig):
                if not hasattr(value, 'config_type'):
                    raise ValueError(f"Non-BaseConfig models must have 'config_type' field. Got {type(value)}")
            value.config_type = key  # Ensure config_type is set
        else:
            raise ValueError(f"Config value must be BaseModel instance or dict, got {type(value)}")
        super().__setitem__(key, value)


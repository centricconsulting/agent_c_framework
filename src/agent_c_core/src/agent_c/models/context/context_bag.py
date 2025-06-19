from typing import Optional
from pydantic import BaseModel

from agent_c.models.context.base import BaseContext
from agent_c.util.observable import ObservableDict, CallbackType
from agent_c.util.registries.context_registry import ContextRegistry, ContextType

class ContextBag(ObservableDict):
    """
    Custom container that handles context serialization/deserialization ensuring
    that context models not known to core cane be instantiated correctly.
    """

    def __init__(self, *args, **kwargs):
        super().__init__(*args, **kwargs)



    @classmethod
    def __get_validators__(cls):
        yield cls.validate

    @classmethod
    def validate(cls, v):
        if isinstance(v, dict):
            result = {}
            for key, value in v.items():
                if isinstance(value, BaseModel):
                    # Verify it has context_type field if it's not a BaseContext
                    if not isinstance(value, BaseContext):
                        if not hasattr(value, 'context_type'):
                            raise ValueError(f"Non-BaseContext models must have 'context_type' field. Got {type(value)}")
                    result[key] = value
                elif isinstance(value, dict):
                    result[key] = ContextRegistry.create(value)
                else:
                    raise ValueError(f"Context value must be BaseModel instance or dict, got {type(value)}")
            return cls(result)
        return v

    def __setitem__(self, key, value):
        if isinstance(value, dict):
            value = ContextRegistry.create(value)
        elif isinstance(value, BaseModel):
            # Verify it has context_type field if it's not a BaseContext
            if not isinstance(value, BaseContext):
                if not hasattr(value, 'context_type'):
                    raise ValueError(f"Non-BaseContext models must have 'context_type' field. Got {type(value)}")
        else:
            raise ValueError(f"Context value must be BaseModel instance or dict, got {type(value)}")
        super().__setitem__(key, value)


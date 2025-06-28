from typing import Any, Annotated

from pydantic import BaseModel, PlainValidator

from agent_c.models.context.base import BaseContext
from agent_c.util import to_snake_case
from agent_c.util.observable import ObservableDict


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
                if isinstance(value, BaseContext ):
                    result[key] = value
                elif isinstance(value, dict):
                    from agent_c.util.registries.context_registry import ContextRegistry
                    result[key] = ContextRegistry.create(value, key, default_dynamic=True)
                elif isinstance(value, BaseModel):
                    result[key] = value
                else:
                    raise ValueError(f"Context value must be BaseModel instance or dict, got {type(value)}")
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
            return super().__getitem__(item)
        except KeyError:
            from agent_c.util.registries.context_registry import ContextRegistry
            # If the item is not found, try to create it from the registry
            if ContextRegistry.is_context_registered(item):
                value = ContextRegistry.create({}, item)
                self[item] = value
                return value

            if ContextRegistry.is_context_registered(f"{item}_user"):
                value = ContextRegistry.create({}, item)
                self[item] = value
                return value

            raise KeyError(f"Context item '{item}' not found in bag and not registered in ContextRegistry.")

    def __setitem__(self, key, value):
        # if the key is a Class use the snake case of the class name as the key,
        # if it's an instance of ANY object use the snake case of the class name of that object as the key,
        if isinstance(key, type):
            key = to_snake_case(key.__name__)
        elif isinstance(key, str):
            key = to_snake_case(key)
        elif isinstance(key, object):
            key = to_snake_case(key.__class__.__name__)

        if isinstance(value, dict):
            from agent_c.util.registries.context_registry import ContextRegistry
            value = ContextRegistry.create(value, key)
        elif isinstance(value, BaseModel):
            # Verify it has context_type field if it's not a BaseContext
            if not isinstance(value, BaseContext):
                if not hasattr(value, 'context_type'):
                    raise ValueError(f"Non-BaseContext models must have 'context_type' field. Got {type(value)}")
        else:
            raise ValueError(f"Context value must be BaseModel instance or dict, got {type(value)}")
        super().__setitem__(key, value)

def ensure_context_bag(v: Any) -> ContextBag:
    """Ensure the value is a ContextBag."""
    if isinstance(v, ContextBag):
        return v
    elif isinstance(v, dict):
        return ContextBag(v)
    elif v is None:
        return ContextBag()
    else:
        raise ValueError(f"Expected dict or ContextBag, got {type(v)}")

ContextBagField = Annotated[ContextBag, PlainValidator(ensure_context_bag)]

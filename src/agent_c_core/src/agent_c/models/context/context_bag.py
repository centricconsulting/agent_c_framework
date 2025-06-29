from typing import Any

from pydantic import BaseModel

from agent_c.models.context.base import BaseContext
from agent_c.util import to_snake_case
from agent_c.util.observable import ObservableDict


class ContextBag(ObservableDict):
    """
    Custom container that handles context serialization/deserialization ensuring
    that context models not known to core cane be instantiated correctly.
    """

    @classmethod
    def __get_validators__(cls):
        yield cls.validate

    @classmethod
    def validate(cls, v):
        if isinstance(v, dict):
            result = {}
            for key, value in v.items():
                if isinstance(value, BaseContext):
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

    @classmethod
    def __get_pydantic_core_schema__(cls, source_type, handler):
        from pydantic_core import core_schema

        def validate_context_bag(value: Any) -> 'ContextBag':
            if isinstance(value, cls):
                return value
            if isinstance(value, dict):
                return cls(value)
            if hasattr(value, 'items'):
                return cls(value)
            raise ValueError(f"Cannot convert {type(value)} to ContextBag")

        return core_schema.no_info_plain_validator_function(validate_context_bag)

    def _normalize_key(self, item):
        """Extract the logic for normalizing keys into a separate method."""
        if isinstance(item, type):
            return to_snake_case(item.__name__)
        elif isinstance(item, str):
            return to_snake_case(item)
        elif isinstance(item, object):
            return to_snake_case(item.__class__.__name__)
        else:
            return item

    def __getitem__(self, item):
        item_key = self._normalize_key(item)

        try:
            return super().__getitem__(item_key)
        except KeyError:
            from agent_c.util.registries.context_registry import ContextRegistry
            # If the item is not found, try to create it from the registry
            if item_key != "shape" and ContextRegistry.is_context_registered(item_key):
                value = ContextRegistry.create({}, item_key)
                self[item] = value
                return value

            raise KeyError(f"Context item '{item}' not found in bag and not registered in ContextRegistry.")

    def __setitem__(self, key, value):
        key = self._normalize_key(key)

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

    def __getattr__(self, name: str) -> Any:
        """
        Override __getattr__ to ensure dot notation uses the same key normalization
        logic as bracket notation.
        """
        # First, check if we're trying to access a special/private attribute
        if name.startswith('__') and name.endswith('__'):
            raise AttributeError(f"{type(self).__name__!r} object has no attribute {name!r}")

        # For ContextBag, we want to use our custom __getitem__ logic
        # which includes key normalization and registry lookup
        try:
            return self.__getitem__(name)
        except KeyError as e:
            # Convert KeyError to AttributeError for consistency with Python conventions
            raise AttributeError(str(e)) from e

    def __setattr__(self, name: str, value: Any) -> None:
        """
        Override __setattr__ to ensure dot notation uses the same logic as bracket notation.
        """
        # List of attributes that should be handled as real attributes
        # These are from ObservableDict and AsyncObservableMixin
        real_attrs = {'_observers', '_async_observers', '_lock'}

        # If it's a known real attribute or starts with underscore (private),
        # use normal attribute setting
        if name in real_attrs or name.startswith('_'):
            super().__setattr__(name, value)
        # Otherwise, check if this attribute exists on the class or its parents
        elif hasattr(type(self), name):
            # It's a class attribute (method, property, etc.), use normal setting
            super().__setattr__(name, value)
        else:
            # Treat it as a dictionary item using our custom __setitem__
            self.__setitem__(name, value)

    def __delattr__(self, name: str) -> None:
        """
        Handle attribute deletion to maintain consistency.
        """
        # First try to delete as a normal attribute
        try:
            super().__delattr__(name)
        except AttributeError:
            # If that fails, try to delete as a dictionary item
            try:
                # Use the normalized key for deletion
                key = self._normalize_key(name)
                super().__delitem__(key)
            except KeyError:
                raise AttributeError(
                    f"{type(self).__name__!r} object has no attribute {name!r} "
                    f"and no dictionary key {name!r}"
                )

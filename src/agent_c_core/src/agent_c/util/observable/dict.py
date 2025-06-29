from agent_c.util.observable.async_observable_mixin import AsyncObservableMixin
from typing import TypeVar, Iterable, Tuple, Any

K = TypeVar('K')
V = TypeVar('V')


class ObservableDict(AsyncObservableMixin, dict):
    """
    A dict subclass that triggers events:
      - `item_added` with (key, value)
      - `item_removed` with (key, old_value)
      - `item_updated` with (key, old_value, new_value)

    Supports dot notation access to dictionary items as a last resort.
    """
    @classmethod
    def __get_pydantic_core_schema__(cls, source_type, handler):
        from pydantic_core import core_schema

        def validate_observable_dict(value: Any) -> 'ObservableDict':
            if isinstance(value, cls):
                return value
            if isinstance(value, dict):
                return cls(value)
            if hasattr(value, 'items'):
                return cls(value)
            raise ValueError(f"Cannot convert {type(value)} to ObservableDict")

        return core_schema.no_info_plain_validator_function(validate_observable_dict)

    def __setitem__(self, key: K, value: V) -> None:
        if key in self:
            old = self.__getitem__(key)
            super().__setitem__(key, value)
            self.trigger('item_updated', key, old, value)
        else:
            super().__setitem__(key, value)
            self.trigger('item_added', key, value)

    def __delitem__(self, key: K) -> None:
        old = self.__getitem__(key)
        super().__delitem__(key)
        self.trigger('item_removed', key, old)

    def pop(self, key: K, default=None) -> V:
        if key in self:
            value = super().pop(key)
            self.trigger('item_removed', key, value)
            return value
        return super().pop(key, default)

    def update(self, *args, **kwargs) -> None:
        for k, v in dict(*args, **kwargs).items():
            self.__setitem__(k, v)

    def __getattr__(self, name: str) -> Any:
        """
        Called only when attribute is not found through normal lookup.
        This is Python's equivalent of Ruby's method_missing.

        Important: This is already the "last resort" - it's only called after:
        1. Instance __dict__ is checked
        2. Class attributes are checked
        3. Parent class attributes are checked
        4. __getattribute__ (if overridden) fails
        """
        # First, check if we're trying to access a special/private attribute
        # These should raise AttributeError to avoid interfering with Python internals
        if name.startswith('__') and name.endswith('__'):
            raise AttributeError(f"{type(self).__name__!r} object has no attribute {name!r}")

        # Check if the name exists as a key in the dictionary
        try:
            return self.__getitem__(name)
        except KeyError:
            # Provide a clear error message indicating both attribute and key lookup failed
            raise AttributeError(
                f"{type(self).__name__!r} object has no attribute {name!r} "
                f"and no dictionary key {name!r}"
            )

    def __setattr__(self, name: str, value: Any) -> None:
        """
        Handle attribute setting to maintain consistency with __getattr__.
        This ensures that setting via dot notation works properly.
        """
        # List of attributes that should be handled as real attributes, not dict items
        # You may need to extend this based on AsyncObservableMixin's attributes
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
            # Treat it as a dictionary item
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
                self.__delitem__(name)
            except KeyError:
                raise AttributeError(
                    f"{type(self).__name__!r} object has no attribute {name!r} "
                    f"and no dictionary key {name!r}"
                )

    def __dir__(self) -> list:
        """
        Include dictionary keys in dir() output for better introspection.
        """
        # Get normal attributes
        attrs = set(super().__dir__())
        # Add dictionary keys (but only string keys)
        attrs.update(k for k in self.keys() if isinstance(k, str))
        return sorted(attrs)
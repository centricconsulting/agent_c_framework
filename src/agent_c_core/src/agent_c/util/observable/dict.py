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
    """

    def __init__(self, mapping: Iterable[Tuple[K, V]] = (), **kwargs):
        dict.__init__(self, mapping, **kwargs)
        AsyncObservableMixin.__init__(self)

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
        try:
            return self.__getitem__(name)
        except KeyError:
            raise AttributeError(f"{type(self).__name__!r} object has no attribute or key {name!r}")

from agent_c.util.observable.async_observable_mixin import AsyncObservableMixin
from typing import MutableMapping, TypeVar, Iterable, Tuple

K = TypeVar('K')
V = TypeVar('V')

class ObservableDict(AsyncObservableMixin, dict, MutableMapping[K, V]):
    """
    A dict subclass that triggers events:
      - `item_added` with (key, value)
      - `item_removed` with (key, old_value)
      - `item_updated` with (key, old_value, new_value)
    """
    def __init__(self, mapping: Iterable[Tuple[K, V]] = (), **kwargs):
        super().__init__()
        dict.__init__(self, mapping, **kwargs)

    def __setitem__(self, key: K, value: V) -> None:
        if key in self:
            old = self[key]
            super().__setitem__(key, value)
            self.trigger('item_updated', key, old, value)
        else:
            super().__setitem__(key, value)
            self.trigger('item_added', key, value)

    def __delitem__(self, key: K) -> None:
        old = self[key]
        super().__delitem__(key)
        self.trigger('item_removed', key, old)

    def pop(self, key: K, default=None) -> V:
        if key in self:
            old = self[key]
            value = super().pop(key)
            self.trigger('item_removed', key, old)
            return value
        return super().pop(key, default)

    def update(self, *args, **kwargs) -> None:
        for k, v in dict(*args, **kwargs).items():
            self.__setitem__(k, v)

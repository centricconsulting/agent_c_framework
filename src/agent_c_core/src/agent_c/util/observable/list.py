from agent_c.util.observable.async_observable_mixin import AsyncObservableMixin, CallbackType
from typing import TypeVar, Iterable, Optional, Any

T = TypeVar('T')

class ObservableList(AsyncObservableMixin, list):
    """
    A list subclass that triggers events:
      - `item_added` with (item, index)
      - `item_removed` with (item, index)
      - `item_set` with (old, new, index)
    """
    def add_observers(
            self,
            item_added: Optional[CallbackType] = None,
            item_set: Optional[CallbackType] = None,
            item_removed: Optional[CallbackType] = None,
            list_extended: Optional[CallbackType] = None,
    ) -> None:
        """
        Add observers callbacks for a specific field.
        Instead of using the .on() method for each of them in turn
        Supports both sync and async callbacks!

        Args:
            item_added : The callback function to call when an item is added.
            item_set : The callback function to call when an item is set.
            item_removed : The callback function to call when an item is removed.
            list_extended : The callback function to call when the list changes.
        """
        if list_extended:
            self.on("list_extended", list_extended)

        if item_set:
            self.on("item_set", item_set)

        if item_added:
            self.on(f"item_added", item_added)

        if item_removed:
            self.on(f"item_removed", item_removed)


    def append(self, item: T) -> None:
        index = len(self)
        super().append(item)
        self.trigger('item_added', item, index)

    async def aappend(self, item: T) -> None:
        index = len(self)
        super().append(item)
        await self.atrigger('item_added', item, index)

    def insert(self, index: int, item: T) -> None:
        super().insert(index, item)
        self.trigger('item_added', item, index)

    async def ainsert(self, index: int, item: T) -> None:
        super().insert(index, item)
        await self.atrigger('item_added', item, index)

    def __setitem__(self, index: int, value: T) -> None:
        old = self[index]
        super().__setitem__(index, value)
        self.trigger('item_set', old, value, index)

    async def asetitem(self, index: int, value: T) -> None:
        old = self[index]
        super().__setitem__(index, value)
        await self.atrigger('item_set', old, value, index)

    def __delitem__(self, index: int) -> None:
        old = self[index]
        super().__delitem__(index)
        self.trigger('item_removed', old, index)

    async def adelitem(self, index: int) -> None:
        old = self[index]
        super().__delitem__(index)
        await self.atrigger('item_removed', old, index)

    def pop(self, index: int = -1) -> T:
        old = super().pop(index)
        self.trigger('item_removed', old, index)
        return old

    async def apop(self, index: int = -1) -> T:
        old = super().pop(index)
        await self.atrigger('item_removed', old, index)
        return old

    def extend(self, iterable: Iterable[T]) -> None:
        start = len(self)
        super().extend(iterable)
        self.trigger('list_extended', self, start, len(self) - 1)

    @classmethod
    def __get_pydantic_core_schema__(cls, source_type, handler):
        from pydantic_core import core_schema

        def validate_observable_list(value: Any) -> 'ObservableList':
            if isinstance(value, cls):
                return value
            if isinstance(value, list):
                c =  cls(value)
                return c
            if hasattr(value, '__iter__') and not isinstance(value, (str, bytes)):
                return cls(list(value))
            raise ValueError(f"Cannot convert {type(value)} to ObservableList")

        return core_schema.no_info_plain_validator_function(validate_observable_list)

    async def aextend(self, iterable: Iterable[T]) -> None:
        start = len(self)
        super().extend(iterable)
        for i, item in enumerate(iterable, start=start):
            await self.atrigger('item_added', item, i)

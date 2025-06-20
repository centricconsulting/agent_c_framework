from contextlib import contextmanager, asynccontextmanager
from typing import Any, Generator, AsyncGenerator

from agent_c.models.base import BaseModel
from agent_c.util.observable.async_observable_mixin import AsyncObservableMixin

class AsyncObservableModel(BaseModel, AsyncObservableMixin):
    """
    Base class for Pydantic models with observable fields.
    """

    def __init__(self, **data: Any) -> None:
        """
        Initialize the ObservableModel with provided data.

        Args:
            **data: Keyword arguments used to initialize the model fields.
        """
        super().__init__(**data)
        self._batch_active: bool = False

    def __setattr__(self, name: str, value: Any) -> None:
        """
        Async version of setattr that properly awaits async callbacks.

        Usage: await model.asetattr('field_name', new_value)
        """
        old_value = getattr(self, name, None)
        if value == old_value:
            return

        # Use regular setattr to actually set the value
        object.__setattr__(self, name, value)

        # Handle notifications
        if name.startswith("_") or self._batch_active:
            return

        if name != 'model':
            self.trigger(f"{name}_changed", old_value, value)

        self.trigger("model_changed", self)

    async def asetattr(self, name: str, value: Any) -> None:
        """
        Async version of setattr that properly awaits async callbacks.

        Usage: await model.asetattr('field_name', new_value)
        """
        old_value = getattr(self, name, None)
        if value == old_value:
            return

        # Use regular setattr to actually set the value
        object.__setattr__(self, name, value)

        # Handle notifications
        if name.startswith("_") or self._batch_active:
            return

        if name != 'model':
            await self.atrigger(f"{name}_changed", old_value, value)

        await self.atrigger("model_changed", self)

    def begin_batch(self) -> None:
        """
        Begin a batch operation, during which observable events are deferred.
        """
        self._batch_active = True

    def end_batch(self, trigger: bool = True) -> None:
        """
        End a batch operation and optionally trigger a "model_changed" event.

        Args:
            trigger (bool): Whether to trigger a "model_changed" event after ending the batch.
        """
        self._batch_active = False
        if trigger:
            self.trigger("model_changed", self)

    async def aend_batch(self, trigger: bool = True) -> None:
        """
        Async version: End a batch and properly await async callbacks.
        """
        self._batch_active = False
        if trigger:
            await self.atrigger("model_changed", self)

    @contextmanager
    def batch(self, trigger: bool = True) -> Generator[None, None, None]:
        """
        Context manager for batching changes.

        Args:
            trigger (bool): Whether to trigger a "model_changed" event after the batch ends.

        Yields:
            None: Batch context for performing multiple updates.
        """
        try:
            self.begin_batch()
            yield
        finally:
            self.end_batch(trigger)

    @asynccontextmanager
    async def abatch(self, trigger: bool = True) -> AsyncGenerator[None, None]:
        """
        Async context manager for batching changes.

        Args:
            trigger (bool): Whether to trigger a "model_changed" event after the batch ends.

        Yields:
            None: Batch context for performing multiple updates.
        """
        try:
            self.begin_batch()
            yield
        finally:
            await self.aend_batch(trigger)

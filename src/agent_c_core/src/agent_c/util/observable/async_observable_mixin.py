import asyncio
import inspect
from contextlib import asynccontextmanager, contextmanager
from typing import Any, Callable, Union, Generator, AsyncGenerator
from weakref import WeakMethod, ref

from observable import Observable
from agent_c.util.logging_utils import LoggingManager

# Type aliases
SyncCallback = Callable[[Any], None]
AsyncCallback = Callable[[Any], Any]  # coroutine function
CallbackType = Union[SyncCallback, AsyncCallback]

class AsyncObservableMixin:
    """
    Mixin that adds both sync- and async-capable event handling to any class.
    """

    def __init__(self, *args, **kwargs):
        self._batch_active = True
        self._dirty = False
        self._async_callbacks: dict[str, set[ref]] = {}
        self._observable = Observable()
        super().__init__(*args, **kwargs)
        self._batch_active = False

    def __new__(cls, *args, **kwargs):
        instance = super().__new__(cls)
        instance._init_observable()
        return instance

    def _init_observable(self):
        """Initialize the observable instance."""
        self._batch_active = False
        self._dirty = False
        self._async_callbacks: dict[str, set[ref]] = {}
        self._observable = Observable()

    def on(self, event: str, callback: Callable) -> None:
        """Register a sync or async callback for `event`."""
        if inspect.iscoroutinefunction(callback):
            # async handlers go to our own registry
            self._async_callbacks.setdefault(event, set())
            weak_cb = WeakMethod(callback) if inspect.ismethod(callback) else ref(callback)
            self._async_callbacks[event].add(weak_cb)
        else:
            # sync handlers are delegated to Observable
            self._observable.on(event, callback)

    def off(self, event: str, callback: CallbackType) -> None:
        """Unregister a callback for `event`."""
        if inspect.iscoroutinefunction(callback):
            for weak_cb in tuple(self._async_callbacks.get(event, ())):
                if (target := weak_cb()) is callback:
                    self._async_callbacks[event].remove(weak_cb)
        else:
            self._observable.off(event, callback)

    def trigger(self, event: str, *args, **kwargs) -> None:
        """
        Fire `event`. Sync callbacks run immediately; async ones are scheduled
        if there's a running loop, or else a warning is logged.
        """
        if self._batch_active:
            # If we're in a batch, just return without triggering
            return

        handled = self._observable.trigger(event, self, *args, **kwargs)
        # if no sync handlers and no async handlers, shortcut out
        if not handled and not self._async_callbacks.get(event):
            return

        # clean up dead refs
        live_refs = {cb for cb in self._async_callbacks.get(event, ()) if cb()}
        self._async_callbacks[event] = live_refs
        async_cbs = [cb() for cb in live_refs if cb()]

        if not async_cbs:
            return

        try:
            loop = asyncio.get_running_loop()
            for cb in async_cbs:
                loop.create_task(self._safe_async_call(cb, event, self, *args, **kwargs))
        except RuntimeError:
            logger = LoggingManager(self.__class__.__name__).get_logger()
            logger.warning(
                f"Async callbacks for '{event}' but no running event loop; "
                "they won’t be executed."
            )

    async def atrigger(self, event: str, *args, **kwargs) -> None:
        """
        Awaitable trigger: runs sync handlers immediately, then awaits all async
        handlers to completion.
        """
        if self._batch_active:
            # If we're in a batch, just return without triggering
            return

        handled = self._observable.trigger(event, self, *args, **kwargs)
        if not handled and not self._async_callbacks.get(event):
            return

        # clean up dead refs
        live_refs = {cb for cb in self._async_callbacks.get(event, ()) if cb()}
        self._async_callbacks[event] = live_refs

        tasks = [
            self._safe_async_call(cb(), event, self, *args, **kwargs)
            for cb in live_refs
            if cb()
        ]
        if tasks:
            await asyncio.gather(*tasks, return_exceptions=True)

    @staticmethod
    async def _safe_async_call(callback: AsyncCallback, event: str, *args, **kwargs):
        """Helper to catch & log errors inside async handlers."""
        try:
            await callback(*args, **kwargs)
        except Exception as e:
            logger = LoggingManager(__name__).get_logger()
            logger.error(f"Error in async callback for '{event}': {e}", exc_info=True)

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
            self.trigger("batch_change", self)

    async def aend_batch(self, trigger: bool = True) -> None:
        """
        Async version: End a batch and properly await async callbacks.
        """
        self._batch_active = False
        if trigger:
            await self.atrigger("batch_changed", self)

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

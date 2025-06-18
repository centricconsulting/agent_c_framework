import asyncio
import inspect

from typing import Any, Callable, Union
from weakref import WeakMethod, ref
from observable import Observable


from agent_c.util.logging_utils import LoggingManager


# Type aliases
SyncCallback = Callable[[Any], None]
AsyncCallback = Callable[[Any], Any]  # Returns a coroutine
CallbackType = Union[SyncCallback, AsyncCallback]

class AsyncObservableWrapper:
    """
    Wrapper around the observable package to add async support.
    """

    def __init__(self):
        self._observable = Observable()
        self._async_callbacks = {}  # event -> set of async callbacks

    def on(self, event: str, callback: CallbackType) -> None:
        """Add a callback (sync or async) to an event."""
        if inspect.iscoroutinefunction(callback):
            # Store async callbacks separately
            if event not in self._async_callbacks:
                self._async_callbacks[event] = set()

            # Use weak references to prevent memory leaks
            if inspect.ismethod(callback):
                self._async_callbacks[event].add(WeakMethod(callback))
            else:
                self._async_callbacks[event].add(ref(callback))
        else:
            # Regular sync callback goes to the observable package
            self._observable.on(event, callback)

    def off(self, event: str, callback: CallbackType) -> None:
        """Remove a callback from an event."""
        if inspect.iscoroutinefunction(callback):
            if event in self._async_callbacks:
                # Remove the weak reference that matches this callback
                to_remove = None
                for weak_cb in self._async_callbacks[event]:
                    cb = weak_cb()
                    if cb and cb == callback:
                        to_remove = weak_cb
                        break
                if to_remove:
                    self._async_callbacks[event].remove(to_remove)
        else:
            # Let observable package handle sync callbacks
            self._observable.off(event, callback)

    def trigger(self, event: str, *args, **kwargs) -> None:
        """
        Trigger an event. Sync callbacks run immediately, async callbacks
        are scheduled on the event loop if one exists.
        """
        # Trigger sync callbacks through observable

        triggered: bool =  self._observable.trigger(event, *args, **kwargs)
        if not triggered:
            # Check if we have async handlers before raising
            if event not in self._async_callbacks or not self._async_callbacks[event]:
                return

        # Handle async callbacks
        if event in self._async_callbacks:
            # Clean up dead weak references
            self._async_callbacks[event] = {
                weak_cb for weak_cb in self._async_callbacks[event]
                if weak_cb() is not None
            }

            # Get strong references to callbacks
            async_cbs = [weak_cb() for weak_cb in self._async_callbacks[event]]
            async_cbs = [cb for cb in async_cbs if cb is not None]

            if async_cbs:
                try:
                    # Try to get current event loop
                    loop = asyncio.get_running_loop()
                    # Schedule async callbacks
                    for callback in async_cbs:
                        loop.create_task(self._safe_async_call(callback, event, *args, **kwargs))
                except RuntimeError:
                    # No event loop running - log warning
                    logging_manager = LoggingManager(__name__)
                    logger = logging_manager.get_logger()
                    logger.warning(
                        f"Async callbacks registered for event '{event}' but no event loop is running. "
                        "Use atrigger() or run in an async context."
                    )

    async def atrigger(self, event: str, *args, **kwargs) -> None:
        """
        Async version of trigger that properly awaits all callbacks.
        """
        # Run sync callbacks first
        handled: bool = self._observable.trigger(event, *args, **kwargs)
        if not handled:
            if event not in self._async_callbacks or not self._async_callbacks[event]:
                return

        # Run and await async callbacks
        if event in self._async_callbacks:
            # Clean up dead weak references
            self._async_callbacks[event] = {
                weak_cb for weak_cb in self._async_callbacks[event]
                if weak_cb() is not None
            }

            # Get strong references and create tasks
            tasks = []
            for weak_cb in self._async_callbacks[event]:
                callback = weak_cb()
                if callback:
                    tasks.append(self._safe_async_call(callback, event, *args, **kwargs))

            if tasks:
                await asyncio.gather(*tasks, return_exceptions=True)

    async def _safe_async_call(self, callback: AsyncCallback, event: str, *args, **kwargs):
        """Safely call an async callback with error handling."""
        try:
            await callback(*args, **kwargs)
        except Exception as e:
            logging_manager = LoggingManager(__name__)
            logger = logging_manager.get_logger()
            logger.error(f"Error in async callback for event '{event}': {e}", exc_info=True)


**AsyncObservableMixin**

As part of the **agent\_c** framework, `AsyncObservableMixin` provides a lightweight, drop‑in mixin for adding both synchronous and asynchronous event handling to your classes. This document covers:

- Overview and motivations
- Installation & import path
- API reference
- Usage examples
- Batching strategies
- Event‑driven best practices
- Troubleshooting & tips

---

## 1. Overview

`AsyncObservableMixin` augments any class with the ability to:

- Register synchronous callbacks (`on`/`off`) for immediate handling
- Register asynchronous callbacks (coroutines) that can be scheduled (`trigger`) or awaited (`atrigger`)
- Clean up dead references automatically via `weakref`
- Log errors in async handlers without crashing the main loop

### Motivations

- **Separation of concerns**: keep event logic out of business methods
- **Flexibility**: mix into any class via multiple inheritance
- **Async support**: handle long‑running tasks (e.g. I/O notifications) gracefully
- **Low overhead**: minimal dependencies and memory footprint

---

## 2. Installation & Import

Ensure `agent_c` is on your Python path (e.g. installed via `poetry` or `pip`).

```bash
pip install agent-c
```

```python
from agent_c.util.observable.async_observable_mixin import AsyncObservableMixin
```

---

## 3. API Reference

All examples assume `class MyClass(AsyncObservableMixin, BaseClass): ...` and that you call `super().__init__()`.

### Constructor

```python
def __init__(self, *args, **kwargs):
    super().__init__(*args, **kwargs)
    # ... your own init logic
```

- Initializes internal `Observable` and async callback registry.

### on(event: str, callback: Callable)

Register a handler for `event`:

- **Sync**: any regular function — called immediately in `trigger`
- **Async**: coroutine function — scheduled or awaited

```python
obj.on("data_received", sync_handler)
obj.on("data_received", async_handler)
```

### off(event: str, callback: Callable)

Unregister a previously registered handler:

```python
obj.off("data_received", sync_handler)
obj.off("data_received", async_handler)
```

### trigger(event: str, \*args, \*\*kwargs)

Fire all sync callbacks immediately, then schedule async callbacks if an active event loop exists.

```python
obj.trigger("finished", result)
```

- If no loop is running, async callbacks are logged and skipped (to avoid deadlocks).

### atrigger(event: str, \*args, \*\*kwargs) -> Awaitable

Awaitable version that executes sync handlers, then `await`s all async handlers to completion:

```python
await obj.atrigger("finished", result)
```

### Error Handling

Async callbacks run inside a try/except and log exceptions via `LoggingManager`:

```text
ERROR Error in async callback for 'event_name': <exception details>
```

---

## 4. Usage Examples

### Basic example

```python
class Worker(AsyncObservableMixin):
    def __init__(self, id: int):
        super().__init__()
        self.id = id

    def start(self):
        self.trigger("start", self.id)

    async def finish(self):
        await self.atrigger("finish", self.id)
```

```python
async def main():
    w = Worker(42)

    # sync handler
    w.on("start", lambda i: print(f"Worker {i} started"))

    # async handler
    async def notify(i):
        await asyncio.sleep(0.1)
        print(f"Notification: worker {i} done")

    w.on("finish", notify)

    w.start()             # prints immediately
    await w.finish()      # waits 0.1s, prints notification
```

### Removing handlers

```python
w.off("start", sync_handler)
w.off("finish", notify)
```

---

## 5. Batching Strategies

To avoid handling high‑frequency events one at a time, consider batching:

1. **Event buffer**: accumulate payloads in a list or queue, trigger on threshold or timer.
2. **Throttling**: ignore events until a cooldown expires.
3. **Debouncing**: reset a timer on each event; only trigger after a quiet period.

```python
class BatchWorker(AsyncObservableMixin):
    def __init__(self, batch_size=10, interval=1.0):
        super().__init__()
        self._buffer = []
        self._batch_size = batch_size
        self._interval = interval
        self._task = None

    def on_data(self, item):
        self._buffer.append(item)
        if len(self._buffer) >= self._batch_size:
            self._flush()
        elif self._task is None:
            self._task = asyncio.get_running_loop().call_later(
                self._interval, self._flush
            )

    def _flush(self):
        data = self._buffer.copy()
        self._buffer.clear()
        if self._task:
            self._task.cancel()
            self._task = None
        self.trigger("batch", data)
```

---

## 6. Event‑Driven Best Practices

- **Keep events coarse‑grained**: too many tiny events lead to coupling and complexity.
- **Document event contracts**: clarify payload shape and ordering guarantees.
- **Use namespaces**: prefix event names (e.g. `file.uploaded`) to avoid collisions.
- **Clean up**: always `off()` handlers when shutting down or when the listener’s lifecycle ends to prevent memory leaks.
- **Error isolation**: let individual handlers fail without affecting others.  `atrigger` uses `gather(return_exceptions=True)`.
- **Avoid blocking**: sync handlers should be lightweight; delegate heavy work to async handlers.
- **Loop awareness**: call `atrigger` inside coroutines; `trigger` outside if you don’t need to wait.

---

## 7. Troubleshooting & Tips

- **"Async callbacks but no running event loop" warning**: occurs when `trigger` is called outside a coroutine.  Either switch to `atrigger` within an async context or start a loop.
- **Missing callbacks**: ensure your callback signatures match, and unregister with the same reference.
- **Inheritance issues**: if `__init__` of a superclass doesn’t call `super()`, mixin initialization may be skipped—place `AsyncObservableMixin` early in your MRO.

---

> This mixin is battle‑tested in **Agent C**, our production‑ready AI agent framework. Feel free to adapt it or extend it (e.g. add priority queues, persistent event logs, etc.) to suit your needs.


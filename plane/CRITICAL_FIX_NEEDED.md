# 🚨 CRITICAL: PLANE Tools Must Be Async

## Root Cause Found

The PLANE tools use **synchronous HTTP** (`requests` library) but Agent C requires **async operations**.

## What Needs to Change

### 1. Replace `requests` with `httpx`
```bash
pip install httpx
```

### 2. Convert PlaneSession to Async
```python
# OLD (synchronous):
import requests
class PlaneSession:
    def __init__(...):
        self.session = requests.Session()
    
    def get(self, endpoint, **kwargs):
        return self.session.get(endpoint, **kwargs)

# NEW (async):
import httpx
class PlaneSession:
    def __init__(...):
        self.session = httpx.AsyncClient()
    
    async def get(self, endpoint, **kwargs):
        return await self.session.get(endpoint, **kwargs)
```

### 3. Convert PlaneClient to Async
All methods must be `async def` and use `await` for HTTP calls.

### 4. All Toolset Methods Are Already Async
Good news - all tool methods are already `async def`, they just need to `await` the client calls.

## Estimated Fix Time

2-3 hours to properly refactor and test all components.

## Decision Point

**Option A:** Fix async issues now (2-3 hours)
**Option B:** Defer PLANE tools until later
**Option C:** Use synchronous wrapper (hacky, not recommended)

What's your preference?

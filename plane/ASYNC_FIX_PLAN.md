# PLANE Tools - Async Fix Plan

**Based on health tools analysis and error logs**

---

## 🔍 Key Findings from Health Tools

**Pattern from PubMedTools:**
```python
import aiohttp  # <-- Async HTTP library

async def tool_method(self, **kwargs) -> str:
    # Use aiohttp.ClientSession for async HTTP
    async with aiohttp.ClientSession() as session:
        async with session.get(url, params=params) as response:
            data = await response.json()
            return data
```

**Key takeaways:**
1. ✅ Use `aiohttp` not `requests` or `httpx`
2. ✅ All tool methods are `async def`
3. ✅ Use `async with` for session management
4. ✅ Use `await` for all HTTP operations
5. ✅ Use context managers (automatic cleanup)

---

## 🚨 Critical Errors to Fix

### Error 1: Wrong Import Path

```python
# WRONG (line 49 of __init__.py):
from plane import PlaneProjectTools

# CORRECT:
from agent_c_tools.tools.plane.tools.plane_projects import PlaneProjectTools
```

### Error 2: Synchronous HTTP (requests → aiohttp)
```python
# WRONG:
import requests
self.session = requests.Session()
response = self.session.get(url)

# CORRECT:
import aiohttp
async with aiohttp.ClientSession() as session:
    async with session.get(url) as response:
        data = await response.json()
```

### Error 3: Non-async Methods
```python
# WRONG:
def _make_request(self, method, endpoint):
    response = self.session.get(endpoint)
    
# CORRECT:
async def _make_request(self, method, endpoint):
    async with self.session.get(endpoint) as response:
        data = await response.json()
```

---

## 📋 Files That Need Async Conversion

### 1. plane_session.py (CRITICAL)
**Changes needed:**
- Replace `requests` with `aiohttp`
- Convert all methods to `async def`
- Use `async with` for HTTP operations
- Keep cookie management (it's sync and that's OK)

### 2. plane_client.py (CRITICAL)  
**Changes needed:**
- All API methods must be `async def`
- Use `await` when calling session methods
- Error handling stays the same

### 3. All 7 toolsets (ALREADY ASYNC!)
**Good news:** Tool methods are already `async def`!
**Changes needed:** Just add `await` when calling client methods

---

## 🎯 Step-by-Step Fix Process

### Step 1: Fix Import Error in __init__.py ✅ DO NOW

```python
# Line 49 in //project/src/agent_c_tools/src/agent_c_tools/__init__.py
# Change from:
from plane import PlaneProjectTools

# To:
from agent_c_tools.tools.plane.tools.plane_projects import PlaneProjectTools
```

### Step 2: Install aiohttp
```bash
pip install aiohttp
```

### Step 3: Convert PlaneSession to Async
- Replace requests with aiohttp
- Make all HTTP methods async
- Test in isolation

### Step 4: Convert PlaneClient to Async
- Make all methods async def
- Add await for session calls
- Test in isolation

### Step 5: Update All Toolsets
- Add await for client calls
- Test each toolset

### Step 6: Comprehensive Testing
- Import test
- Mock test
- Live API test
- Server integration test

---

## 🔧 Starting Now

Let me fix the import error first, then convert to async properly.

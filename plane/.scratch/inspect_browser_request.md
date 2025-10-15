# How to Inspect Browser Requests to PLANE

We need to see EXACTLY what headers and cookies your browser sends when it successfully accesses workspace resources.

## Steps:

1. **Open PLANE in your browser:** `http://localhost/agent_c/`

2. **Open DevTools:** Press F12

3. **Go to Network tab**

4. **Clear the network log** (🚫 icon)

5. **Navigate to Projects** in PLANE (or refresh the page)

6. **Find a successful API request** in the Network tab:
   - Look for: `GET /api/workspaces/agent_c/projects/`
   - Status should be: `200`

7. **Click on that request**

8. **Copy ALL the Request Headers:**
   - Right-click on the request
   - Select "Copy" → "Copy as cURL" or "Copy request headers"

9. **Paste the headers here**

## What We Need:

Specifically, we need to see:
- All Cookie headers
- Any other authentication headers
- Referer header
- Origin header
- User-Agent
- Any X- headers

## Example of what to look for:

```
GET /api/workspaces/agent_c/projects/ HTTP/1.1
Host: localhost
Cookie: agentc-auth-token=eyJ...; csrftoken=o4t...; other-cookie=value
X-CSRFToken: o4tAHf...
Referer: http://localhost/agent_c/
Origin: http://localhost
User-Agent: Mozilla/5.0...
```

The browser is doing something that makes it work - we need to figure out what!

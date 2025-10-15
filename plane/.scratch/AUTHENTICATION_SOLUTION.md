# PLANE Authentication Solution - CRITICAL INFO

## Date: 2025-10-15

## Problem Discovered
PLANE uses **TWO different authentication methods**:

1. **API Key Authentication** (`X-Api-Key` header)
   - Works for: `/api/v1/users/me/`, `/api/instances/`
   - Does NOT work for: Workspace resources (`/api/workspaces/...`)
   - API Key: `plane_api_623e92aaced748629a60efcb603af928`
   - User: Ethan Booth (ethan.booth@centricconsulting.com)

2. **JWT Token Authentication** (Cookie-based)
   - Works for: ALL workspace resources
   - Required for: Projects, Issues, etc.
   - Cookie name: `agentc-auth-token`
   - Also needs: `csrftoken` cookie
   - User: Admin (changeme@centricconsulting.com)

## Current Cookies (from browser)

```
Cookie: agentc-auth-token=eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJhZG1pbiIsInVzZXIiOnsidXNlcl9pZCI6ImFkbWluIiwidXNlcl9uYW1lIjoiYWRtaW4iLCJlbWFpbCI6ImNoYW5nZW1lQGNlbnRyaWNjb25zdWx0aW5nLmNvbSIsImZpcnN0X25hbWUiOiJBZG1pbiIsImxhc3RfbmFtZSI6IlVzZXIiLCJpc19hY3RpdmUiOnRydWUsInJvbGVzIjpbImFkbWluIl0sImdyb3VwcyI6W10sImNyZWF0ZWRfYXQiOiIyMDI1LTA5LTIzVDA3OjE0OjA3LjU0OTc2MiIsImxhc3RfbG9naW4iOm51bGx9LCJleHAiOjE3NjMwMzY4NjYsImlhdCI6MTc2MDM1ODQ2Nn0.bAzdpE0_siX_aBalWpYdGa-phULkb2WCFESx95BAHRk

Cookie: csrftoken=o4tAHfTcpMvysGKLaIEhw1qUS29yrDP1
```

## JWT Token Decoded

```json
{
  "sub": "admin",
  "user": {
    "user_id": "admin",
    "user_name": "admin",
    "email": "changeme@centricconsulting.com",
    "first_name": "Admin",
    "last_name": "User",
    "is_active": true,
    "roles": ["admin"],
    "groups": [],
    "created_at": "2025-09-23T07:14:07.549762",
    "last_login": null
  },
  "exp": 1763036866,  // Expires: 2025-11-12
  "iat": 1760358466   // Issued: 2024-10-13
}
```

## PLANE Configuration

```yaml
instance_url: http://localhost
workspace_slug: agent_c
workspace_id: TBD

# Authentication (choose one):
auth_method_1:
  type: api_key
  header: X-Api-Key
  value: plane_api_623e92aaced748629a60efcb603af928
  works_for: [/api/v1/users/me/, /api/instances/]
  fails_for: [/api/workspaces/*]

auth_method_2:
  type: jwt_cookie
  cookie_name: agentc-auth-token
  jwt_token: eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
  csrf_cookie: csrftoken
  csrf_value: o4tAHfTcpMvysGKLaIEhw1qUS29yrDP1
  works_for: [ALL ENDPOINTS]
  expires: 2025-11-12
```

## Tool Implementation Strategy

### Option 1: Use JWT Token (RECOMMENDED)
**Pros:**
- Works for ALL workspace resources
- Admin user has full access
- Token valid until 2025-11-12

**Cons:**
- Need to handle token refresh
- Cookie-based auth requires session management

**Implementation:**
```python
import requests

session = requests.Session()
session.cookies.set('agentc-auth-token', jwt_token)
session.cookies.set('csrftoken', csrf_token)
session.headers['X-CSRFToken'] = csrf_token

# Now all workspace requests will work
response = session.get('http://localhost/api/workspaces/agent_c/projects/')
```

### Option 2: Implement Login Flow
**Pros:**
- Can get fresh tokens programmatically
- More sustainable long-term
- Works for any user

**Cons:**
- Need username/password
- More complex implementation

**Implementation:**
```python
# Need to find PLANE's login endpoint
# Check: http://localhost/swagger/ for auth endpoints
```

### Option 3: Hybrid Approach
Use JWT token for initial development, implement proper login flow later.

## Next Steps

1. ✅ Test JWT token with `test_jwt_token.py`
2. ⏳ Verify workspace access works
3. ⏳ Build core PLANE client using JWT auth
4. ⏳ Implement 5 core toolsets
5. ⏳ Add token refresh logic later

## Files Created

- `//plane/.scratch/test_jwt_token.py` - Tests JWT authentication
- `//plane/.scratch/VERIFIED_CONFIG.md` - API configuration
- `//plane/.scratch/AUTHENTICATION_SOLUTION.md` - This file

## Important Notes

- The API key belongs to "Ethan Booth" but doesn't have workspace access
- The browser session uses "Admin" user which has full access
- This suggests PLANE has role-based permissions
- May need to give Ethan's API key workspace permissions, OR
- Use Admin's JWT token for tool development

## Browser Cookie Extraction

When extracting cookies from browser:
1. F12 → Application → Cookies → http://localhost
2. Copy `agentc-auth-token` value
3. Copy `csrftoken` value
4. Use both in requests

## API Documentation Available

- Swagger UI: http://localhost/swagger/
- API Docs: http://localhost/api-docs/
- Check these for proper login/auth endpoints

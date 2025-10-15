#!/usr/bin/env python3
"""
Investigate PLANE/Agent C authentication flow

We need to understand how authentication actually works to implement auto-refresh.
The cookie name 'agentc-auth-token' suggests this might be Agent C's auth, not PLANE's.
"""

import sys
import jwt
import json
from pathlib import Path

tools_path = Path(__file__).parent.parent.parent / "tools" / "src"
sys.path.insert(0, str(tools_path))

from agent_c_tools.tools.plane.auth.cookie_manager import PlaneCookieManager

print("="*70)
print("AUTHENTICATION FLOW INVESTIGATION")
print("="*70)

# Load current cookies
cookie_manager = PlaneCookieManager("agent_c")
cookies = cookie_manager.load_cookies()

if not cookies:
    print("❌ No cookies found. Run setup first.")
    sys.exit(1)

# Analyze the JWT token
print("\n1️⃣  Analyzing JWT Token...")
print("-"*70)

jwt_token = cookies.get('agentc-auth-token')
if jwt_token:
    try:
        # Decode without verification to inspect
        header = jwt.get_unverified_header(jwt_token)
        payload = jwt.decode(jwt_token, options={"verify_signature": False})
        
        print("✅ JWT Token decoded successfully")
        print("\n📋 Header:")
        print(json.dumps(header, indent=2))
        
        print("\n📋 Payload:")
        print(json.dumps(payload, indent=2, default=str))
        
        print("\n🔍 Key Observations:")
        print(f"  Algorithm: {header.get('alg')}")
        print(f"  Token Type: {header.get('typ')}")
        print(f"  Subject: {payload.get('sub')}")
        print(f"  User Email: {payload.get('user', {}).get('email')}")
        print(f"  Issued At: {payload.get('iat')}")
        print(f"  Expires: {payload.get('exp')}")
        
        # Calculate expiration
        import datetime
        exp_timestamp = payload.get('exp')
        if exp_timestamp:
            exp_date = datetime.datetime.fromtimestamp(exp_timestamp)
            now = datetime.datetime.now()
            time_left = exp_date - now
            print(f"  Time until expiration: {time_left}")
            
    except Exception as e:
        print(f"❌ Failed to decode JWT: {e}")
else:
    print("❌ No JWT token found in cookies")

# Check session-id structure
print("\n2️⃣  Analyzing Session ID...")
print("-"*70)

session_id = cookies.get('session-id')
if session_id:
    print(f"✅ Session ID found")
    print(f"   Length: {len(session_id)} characters")
    print(f"   Format: {''.join(['x' if c.isalnum() else c for c in session_id[:40]])}...")
    print(f"\n   This looks like a Django session ID")
else:
    print("❌ No session-id found")

# Look for auth endpoints
print("\n3️⃣  Searching for Auth/Login Endpoints...")
print("-"*70)

from agent_c_tools.tools.plane.client.plane_client import PlaneClient

client = PlaneClient("http://localhost", "agent_c")

# Endpoints to check
auth_endpoints = [
    # Agent C auth
    '/auth/login/',
    '/auth/token/',
    '/auth/refresh/',
    '/api/auth/token/refresh/',
    
    # Django session
    '/accounts/login/',
    '/admin/login/',
    
    # JWT refresh
    '/api/token/refresh/',
    '/api/auth/refresh/',
]

found_endpoints = []

for endpoint in auth_endpoints:
    try:
        response = client.session.get(f"http://localhost{endpoint}")
        if response.status_code in [200, 400, 401, 405]:  # Exists!
            print(f"  ✅ Found: {endpoint} (Status: {response.status_code})")
            found_endpoints.append(endpoint)
        else:
            print(f"  ❌ {endpoint}")
    except:
        print(f"  ❌ {endpoint}")

# Summary
print("\n" + "="*70)
print("FINDINGS & RECOMMENDATIONS")
print("="*70)

print("""
Based on analysis:

1. JWT TOKEN (agentc-auth-token):
   - Issued by Agent C system (not standard PLANE)
   - Contains user info and roles
   - Has expiration timestamp
   - Used for client-side authentication

2. SESSION-ID:
   - Django session ID format
   - Server-side session tracking
   - This is what PLANE actually validates

3. AUTHENTICATION APPROACH:

   Option A: Browser Automation (RECOMMENDED)
   ----------------------------------------
   - Use Playwright to automate login
   - Extract cookies programmatically
   - Reliable and works with any auth system
   - Dependency: playwright library
   
   Implementation:
   ```python
   async def refresh_cookies_browser():
       browser = await playwright.chromium.launch()
       page = await browser.new_page()
       await page.goto('http://localhost/')
       # Cookies auto-set from existing session OR
       # Perform login if needed
       cookies = await page.context.cookies()
       return extract_needed_cookies(cookies)
   ```

   Option B: Check Agent C Auth API
   ---------------------------------
   - If PLANE is using Agent C's auth system
   - There might be an Agent C API to get tokens
   - Check Agent C documentation
   - May have refresh token endpoint

   Option C: Django Session Refresh
   --------------------------------
   - Some Django apps support session refresh
   - Check: /api/auth/refresh/ or /api/session/refresh/
   - Send current session-id, get new one

NEXT STEPS:
1. Try to access Agent C admin/docs at http://localhost/admin/
2. Check if there's Agent C auth documentation
3. Or implement Option A (browser automation)
""")

print("="*70)

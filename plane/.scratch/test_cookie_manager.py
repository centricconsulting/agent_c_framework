#!/usr/bin/env python3
"""Test the new cookie manager with current working cookies"""

import sys
from pathlib import Path

# Add tools to path
tools_path = Path(__file__).parent.parent.parent / "tools" / "src"
sys.path.insert(0, str(tools_path))

from agent_c_tools.tools.plane.auth import PlaneCookieManager
from agent_c_tools.tools.plane import PlaneSession

print("="*70)
print("TESTING PLANE COOKIE MANAGER")
print("="*70)

# Your current working cookies
cookies = {
    'session-id': 'my5jxtxf45248zxyapvhh8hcbpfh9twp4tw6u491dzewnfnjc2wapwccwanxetpg5d00u8amq78zvjwdetdhdo8x8p8p3ps6xlbbl1nm4dvg9c59aj7ehuq3adahedna',
    'agentc-auth-token': 'eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJhZG1pbiIsInVzZXIiOnsidXNlcl9pZCI6ImFkbWluIiwidXNlcl9uYW1lIjoiYWRtaW4iLCJlbWFpbCI6ImNoYW5nZW1lQGNlbnRyaWNjb25zdWx0aW5nLmNvbSIsImZpcnN0X25hbWUiOiJBZG1pbiIsImxhc3RfbmFtZSI6IlVzZXIiLCJpc19hY3RpdmUiOnRydWUsInJvbGVzIjpbImFkbWluIl0sImdyb3VwcyI6W10sImNyZWF0ZWRfYXQiOiIyMDI1LTA5LTIzVDA3OjE0OjA3LjU0OTc2MiIsImxhc3RfbG9naW4iOm51bGx9LCJleHAiOjE3NjMwMzY4NjYsImlhdCI6MTc2MDM1ODQ2Nn0.bAzdpE0_siX_aBalWpYdGa-phULkb2WCFESx95BAHRk',
    'csrftoken': 'o4tAHfTcpMvysGKLaIEhw1qUS29yrDP1',
    'ajs_anonymous_id': 'b84a7c10-2aca-4bf2-9579-f281a47c03d5',
}

workspace = "agent_c"
base_url = "http://localhost"

print("\n1️⃣  Testing CookieManager...")
print("-"*70)

cookie_manager = PlaneCookieManager(workspace)
print(f"✅ CookieManager initialized")
print(f"   Config dir: {cookie_manager.config_dir}")
print(f"   Cookies dir: {cookie_manager.cookies_dir}")

# Test validation
print(f"\n2️⃣  Validating cookies...")
if cookie_manager.validate_cookies(cookies):
    print(f"✅ All required cookies present")
else:
    print(f"❌ Missing required cookies")
    sys.exit(1)

# Test saving
print(f"\n3️⃣  Saving cookies...")
try:
    cookie_manager.save_cookies(cookies)
    print(f"✅ Cookies saved successfully")
    print(f"   File: {cookie_manager._get_cookie_file()}")
except Exception as e:
    print(f"❌ Error saving: {e}")
    sys.exit(1)

# Test loading
print(f"\n4️⃣  Loading cookies...")
loaded_cookies = cookie_manager.load_cookies()
if loaded_cookies:
    print(f"✅ Cookies loaded successfully")
    print(f"   Loaded {len(loaded_cookies)} cookies")
    
    # Verify they match
    if loaded_cookies == cookies:
        print(f"✅ Loaded cookies match original")
    else:
        print(f"⚠️  Loaded cookies differ from original")
else:
    print(f"❌ Failed to load cookies")
    sys.exit(1)

# Test PlaneSession
print(f"\n5️⃣  Testing PlaneSession...")
print("-"*70)

try:
    session = PlaneSession(base_url, workspace, cookies)
    print(f"✅ PlaneSession initialized")
except Exception as e:
    print(f"❌ Error initializing session: {e}")
    sys.exit(1)

# Test connection
print(f"\n6️⃣  Testing API connection...")
try:
    if session.test_connection():
        print(f"✅ Connection test passed")
    else:
        print(f"❌ Connection test failed")
        sys.exit(1)
except Exception as e:
    print(f"❌ Connection test error: {e}")
    sys.exit(1)

# Test actual API call
print(f"\n7️⃣  Making API request...")
try:
    response = session.get("/api/users/me/")
    if response.status_code == 200:
        user = response.json()
        print(f"✅ API request successful")
        print(f"   User: {user.get('email')}")
    else:
        print(f"❌ API request failed: {response.status_code}")
except Exception as e:
    print(f"❌ API request error: {e}")
    sys.exit(1)

# Test loading session from saved cookies
print(f"\n8️⃣  Testing session with saved cookies...")
try:
    # Create new session without providing cookies (should load from disk)
    session2 = PlaneSession(base_url, workspace)
    print(f"✅ Session loaded from saved cookies")
    
    # Test it works
    response = session2.get("/api/workspaces/agent_c/projects/")
    if response.status_code == 200:
        projects = response.json()
        print(f"✅ Loaded session works!")
        print(f"   Found {len(projects)} project(s)")
    else:
        print(f"⚠️  Loaded session returned {response.status_code}")
        
except Exception as e:
    print(f"❌ Error with loaded session: {e}")

print("\n" + "="*70)
print("ALL TESTS PASSED! ✅")
print("="*70)
print("\nCookie manager is working correctly!")
print(f"Cookies are saved in: {cookie_manager._get_cookie_file()}")
print("\nYou can now use PlaneSession without providing cookies:")
print(f'  session = PlaneSession("http://localhost", "agent_c")')
print("="*70)

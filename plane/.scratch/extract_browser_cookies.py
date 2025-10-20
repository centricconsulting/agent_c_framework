#!/usr/bin/env python3
"""
Extract PLANE session cookies from browser for API access

Since PLANE's workspace endpoints require session cookies (not just API keys),
we need to use the session cookies from your browser.
"""

import requests
import json

base_url = "http://localhost"
workspace_slug = "agent_c"

print("="*70)
print("PLANE BROWSER COOKIE AUTHENTICATION")
print("="*70)
print("""
To access workspace resources, PLANE requires session cookies.

STEPS TO GET YOUR COOKIES:

1. Open PLANE in your browser: http://localhost
2. Log in (you're already logged in)
3. Open Browser DevTools (F12 or Right-click → Inspect)
4. Go to: Application → Cookies → http://localhost
5. Find and copy these cookie values:
   - accessToken (or access_token)
   - refreshToken (or refresh_token)  
   - sessionid (if present)
   - csrftoken (if present)

6. Paste them below when prompted
""")
print("="*70)

# Get cookies from user
print("\n📝 Enter your cookies (press Enter to skip if not found):\n")

cookies = {}
cookie_names = [
    'accessToken',
    'access_token', 
    'refreshToken',
    'refresh_token',
    'sessionid',
    'csrftoken',
    'PHPSESSID',
]

for name in cookie_names:
    value = input(f"{name}: ").strip()
    if value:
        cookies[name] = value

if not cookies:
    print("\n❌ No cookies provided. Cannot test workspace access.")
    print("\nPlease extract cookies from your browser and try again.")
    exit(1)

print(f"\n✅ Got {len(cookies)} cookie(s)")
print("\nTesting workspace access with cookies...")
print("-"*70)

# Test with cookies
session = requests.Session()
for name, value in cookies.items():
    session.cookies.set(name, value)

# Try workspace endpoints
endpoints_to_test = [
    (f"/api/workspaces/{workspace_slug}/", "Workspace Info"),
    (f"/api/workspaces/{workspace_slug}/projects/", "Projects List"),
    (f"/api/users/me/workspaces/", "User Workspaces"),
]

results = {}

for endpoint, description in endpoints_to_test:
    url = f"{base_url}{endpoint}"
    print(f"\n🔍 {description}")
    print(f"   Endpoint: {endpoint}")
    
    try:
        response = session.get(url, headers={'Content-Type': 'application/json'})
        status = response.status_code
        
        if status == 200:
            print(f"   ✅ Status: {status} - SUCCESS!")
            try:
                data = response.json()
                if isinstance(data, list):
                    print(f"   Found: {len(data)} items")
                    if len(data) > 0:
                        print(f"   Sample: {list(data[0].keys())[:5]}")
                else:
                    print(f"   Keys: {list(data.keys())[:5]}")
                results[description] = True
            except:
                print(f"   Response: {response.text[:100]}")
                results[description] = True
        else:
            print(f"   ❌ Status: {status}")
            try:
                error = response.json()
                print(f"   Error: {error}")
            except:
                pass
            results[description] = False
    except Exception as e:
        print(f"   ❌ Error: {e}")
        results[description] = False

# Summary
print("\n" + "="*70)
print("TEST RESULTS")
print("="*70)

if all(results.values()):
    print("\n🎉 SUCCESS! All workspace endpoints accessible with cookies!")
    print("\n📝 Save these cookies for API tool development:")
    print("-"*70)
    for name, value in cookies.items():
        print(f"{name}={value}")
    print("-"*70)
    
    print("\n✅ NEXT STEPS:")
    print("1. Use these cookies in the PLANE API client")
    print("2. Implement cookie refresh logic")
    print("3. Build the 5 core toolsets")
    
else:
    print("\n⚠️ Some endpoints still failing.")
    print("\nTry these:")
    print("1. Make sure you're logged into PLANE in your browser")
    print("2. Check DevTools → Application → Cookies")
    print("3. Look for ALL cookies (there may be more than listed)")
    print("4. Clear browser cache and log in again")

print("="*70)

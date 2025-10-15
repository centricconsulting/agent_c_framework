#!/usr/bin/env python3
"""Test PLANE access with JWT token from cookie"""

import requests
import json

base_url = "http://localhost"
workspace_slug = "agent_c"

# The JWT token from your browser cookie
jwt_token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJhZG1pbiIsInVzZXIiOnsidXNlcl9pZCI6ImFkbWluIiwidXNlcl9uYW1lIjoiYWRtaW4iLCJlbWFpbCI6ImNoYW5nZW1lQGNlbnRyaWNjb25zdWx0aW5nLmNvbSIsImZpcnN0X25hbWUiOiJBZG1pbiIsImxhc3RfbmFtZSI6IlVzZXIiLCJpc19hY3RpdmUiOnRydWUsInJvbGVzIjpbImFkbWluIl0sImdyb3VwcyI6W10sImNyZWF0ZWRfYXQiOiIyMDI1LTA5LTIzVDA3OjE0OjA3LjU0OTc2MiIsImxhc3RfbG9naW4iOm51bGx9LCJleHAiOjE3NjMwMzY4NjYsImlhdCI6MTc2MDM1ODQ2Nn0.bAzdpE0_siX_aBalWpYdGa-phULkb2WCFESx95BAHRK"

csrf_token = "o4tAHfTcpMvysGKLaIEhw1qUS29yrDP1"

print("="*70)
print("TESTING PLANE JWT TOKEN AUTHENTICATION")
print("="*70)

# Try different authentication methods with the JWT token
auth_methods = [
    ("Cookie: agentc-auth-token", {}, {'agentc-auth-token': jwt_token}),
    ("Authorization: Bearer", {'Authorization': f'Bearer {jwt_token}'}, {}),
    ("Cookie + CSRF Header", {'X-CSRFToken': csrf_token}, {'agentc-auth-token': jwt_token, 'csrftoken': csrf_token}),
]

endpoints_to_test = [
    f"/api/workspaces/{workspace_slug}/",
    f"/api/workspaces/{workspace_slug}/projects/",
    f"/api/users/me/workspaces/",
]

print("\nTesting authentication methods...")
print("-"*70)

successful_method = None

for auth_name, headers, cookies in auth_methods:
    print(f"\n🔐 Method: {auth_name}")
    
    for endpoint in endpoints_to_test:
        url = f"{base_url}{endpoint}"
        
        try:
            response = requests.get(
                url,
                headers={**headers, 'Content-Type': 'application/json'},
                cookies=cookies,
                timeout=10
            )
            
            status = response.status_code
            symbol = "✅" if status == 200 else "❌"
            print(f"  {symbol} {endpoint} → {status}")
            
            if status == 200 and not successful_method:
                successful_method = (auth_name, headers, cookies)
                try:
                    data = response.json()
                    if isinstance(data, list):
                        print(f"     Found {len(data)} items")
                        if len(data) > 0 and isinstance(data[0], dict):
                            print(f"     Sample keys: {list(data[0].keys())[:5]}")
                    else:
                        print(f"     Keys: {list(data.keys())[:5]}")
                except:
                    pass
                    
        except Exception as e:
            print(f"  ❌ {endpoint} → Error: {e}")

# Summary
print("\n" + "="*70)
print("RESULTS")
print("="*70)

if successful_method:
    auth_name, headers, cookies = successful_method
    print(f"\n🎉 SUCCESS! Found working authentication method:")
    print(f"\n   Method: {auth_name}")
    
    if headers:
        print(f"   Headers: {headers}")
    if cookies:
        print(f"   Cookies: {list(cookies.keys())}")
    
    # Try to get actual workspace and project data
    print(f"\n📊 Fetching workspace data...")
    
    session = requests.Session()
    for name, value in cookies.items():
        session.cookies.set(name, value)
    
    for key, value in headers.items():
        session.headers[key] = value
    
    # Get projects
    response = session.get(f"{base_url}/api/workspaces/{workspace_slug}/projects/")
    if response.status_code == 200:
        projects = response.json()
        print(f"\n✅ Projects ({len(projects)} total):")
        for proj in projects[:5]:
            print(f"   - {proj.get('name')} (ID: {proj.get('id')})")
    
    # Get workspace info  
    response = session.get(f"{base_url}/api/users/me/workspaces/")
    if response.status_code == 200:
        workspaces = response.json()
        print(f"\n✅ Workspaces ({len(workspaces)} total):")
        for ws in workspaces[:5]:
            print(f"   - {ws.get('name')} (Slug: {ws.get('slug')})")
    
    print(f"\n✅ READY TO BUILD TOOLS!")
    print(f"\nConfiguration for tools:")
    print("-"*70)
    print(f"AUTH_METHOD: {auth_name}")
    print(f"JWT_TOKEN: {jwt_token[:50]}...")
    if csrf_token:
        print(f"CSRF_TOKEN: {csrf_token}")
    print("-"*70)
    
else:
    print("\n❌ No working authentication method found")
    print("\nThe JWT token from the cookie may have expired or be invalid.")
    print("\nNext steps:")
    print("1. Log out and log back into PLANE")
    print("2. Extract fresh cookies")
    print("3. Or implement username/password login flow")

print("="*70)

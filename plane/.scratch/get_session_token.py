#!/usr/bin/env python3
"""
Get PLANE session token using API key

PLANE uses session-based authentication for workspace resources.
We need to exchange our API key for session cookies.
"""

import requests

base_url = "http://localhost"
api_key = "plane_api_623e92aaced748629a60efcb603af928"

print("="*70)
print("PLANE SESSION TOKEN ACQUISITION")
print("="*70)

# Strategy: Use the API key with endpoints that work, extract cookies
session = requests.Session()

# First, authenticate with an endpoint that accepts API key
headers = {
    'X-Api-Key': api_key,
    'Content-Type': 'application/json'
}

print("\n1. Getting user info with API key...")
response = session.get(f"{base_url}/api/v1/users/me/", headers=headers)

if response.status_code == 200:
    print("✅ Authenticated!")
    user_data = response.json()
    print(f"   User: {user_data['first_name']} {user_data['last_name']}")
    
    # Check if we got any cookies
    print(f"\n2. Checking for session cookies...")
    if session.cookies:
        print("✅ Got cookies:")
        for cookie in session.cookies:
            print(f"   {cookie.name} = {cookie.value[:20]}...")
    else:
        print("❌ No cookies received")
    
    # Now try workspace endpoint with the session
    print(f"\n3. Trying workspace endpoint with session...")
    response2 = session.get(f"{base_url}/api/workspaces/agent_c/projects/")
    
    if response2.status_code == 200:
        print("✅ SUCCESS! Workspace access working with session!")
        projects = response2.json()
        print(f"   Found {len(projects)} projects")
    else:
        print(f"❌ Still getting {response2.status_code}")
        print(f"   This means API keys don't create sessions automatically")
        
        # Try to find a login endpoint
        print(f"\n4. Looking for authentication endpoint...")
        
        auth_attempts = [
            ('/api/auth/api-token-check/', 'POST', {'token': api_key}),
            ('/api/sign-in/', 'POST', {'api_key': api_key}),
        ]
        
        for endpoint, method, data in auth_attempts:
            url = f"{base_url}{endpoint}"
            print(f"\n   Trying: {method} {endpoint}")
            try:
                if method == 'POST':
                    r = session.post(url, json=data, headers={'Content-Type': 'application/json'})
                else:
                    r = session.get(url, headers=headers)
                    
                print(f"   Status: {r.status_code}")
                if r.status_code == 200:
                    print(f"   ✅ Success! Got response")
                    try:
                        print(f"   Data: {r.json()}")
                    except:
                        print(f"   Text: {r.text[:200]}")
            except Exception as e:
                print(f"   Error: {e}")

print("\n" + "="*70)
print("RECOMMENDATION")
print("="*70)
print("""
PLANE's API key authentication doesn't provide workspace access directly.

OPTIONS:

1. **Use Browser Cookies** (Temporary Solution):
   - Extract cookies from your browser session
   - Use those cookies in API requests
   - Will expire after session timeout

2. **Use Username/Password Login** (Better Solution):
   - Implement proper login flow
   - Get session cookies programmatically  
   - Can refresh automatically

3. **Check PLANE Documentation**:
   - Visit: http://localhost/swagger/
   - Or: http://localhost/api-docs/
   - Look for authentication methods

4. **Contact PLANE Support**:
   - Ask about API key permissions for workspace resources
   - There may be a configuration setting needed
""")
print("="*70)

#!/usr/bin/env python3
"""
Find PLANE's login endpoint by testing common patterns
"""

import requests
import json

base_url = "http://localhost"

print("="*70)
print("SEARCHING FOR PLANE LOGIN ENDPOINT")
print("="*70)

# Common login endpoint patterns
login_endpoints = [
    # Django patterns
    ('/api/sign-in/', 'POST'),
    ('/api/auth/sign-in/', 'POST'),
    ('/api/auth/login/', 'POST'),
    ('/api/login/', 'POST'),
    ('/api/token/', 'POST'),
    ('/api/token/obtain/', 'POST'),
    
    # Session/cookie patterns
    ('/api/session/', 'POST'),
    ('/api/auth/session/', 'POST'),
    
    # User auth patterns
    ('/api/users/login/', 'POST'),
    ('/api/v1/auth/sign-in/', 'POST'),
    
    # Magic link / passwordless
    ('/api/auth/magic-sign-in/', 'POST'),
    ('/api/auth/email-check/', 'POST'),
]

print("\n🔍 Testing common login endpoint patterns...")
print("-"*70)

found_endpoints = []

for endpoint, method in login_endpoints:
    url = f"{base_url}{endpoint}"
    
    try:
        # Try POST with empty body to see response
        response = requests.post(
            url,
            json={},
            headers={'Content-Type': 'application/json'},
            timeout=5
        )
        
        status = response.status_code
        
        # 400 = Bad Request (endpoint exists, needs data)
        # 405 = Method Not Allowed (wrong method)
        # 404 = Not Found
        # 401 = Unauthorized (needs auth)
        
        if status in [400, 401, 422]:  # These mean endpoint exists!
            print(f"✅ FOUND: {endpoint}")
            print(f"   Status: {status}")
            try:
                error_data = response.json()
                print(f"   Response: {json.dumps(error_data, indent=2)[:200]}")
            except:
                print(f"   Response: {response.text[:200]}")
            found_endpoints.append((endpoint, method, status))
            print()
        elif status == 404:
            print(f"❌ {endpoint} - Not Found")
        elif status == 405:
            print(f"⚠️  {endpoint} - Method Not Allowed (try GET?)")
        else:
            print(f"? {endpoint} - {status}")
            
    except requests.exceptions.ConnectionError:
        print(f"❌ {endpoint} - Connection Error")
    except Exception as e:
        print(f"❌ {endpoint} - {e}")

print("\n" + "="*70)
print("CHECKING PLANE DOCUMENTATION")
print("="*70)

# Check if PLANE has API documentation endpoints
doc_endpoints = [
    '/swagger/',
    '/api-docs/',
    '/docs/',
]

print("\nChecking for API documentation...")
for endpoint in doc_endpoints:
    url = f"{base_url}{endpoint}"
    try:
        response = requests.get(url, timeout=5)
        if response.status_code == 200:
            print(f"✅ Found docs at: {url}")
            print(f"   → Open in browser to find auth endpoints")
    except:
        pass

print("\n" + "="*70)
print("SUMMARY")
print("="*70)

if found_endpoints:
    print(f"\n✅ Found {len(found_endpoints)} potential login endpoint(s):")
    for endpoint, method, status in found_endpoints:
        print(f"\n  {method} {endpoint}")
        print(f"  Status: {status}")
        
    print("\n📝 Next steps:")
    print("1. Test these endpoints with actual credentials")
    print("2. Check response for session cookies")
    print("3. Verify cookies work for workspace access")
else:
    print("\n❌ No login endpoints found via common patterns")
    print("\n💡 Alternative approaches:")
    print("1. Check PLANE source code for auth routes")
    print("2. Review http://localhost/swagger/ documentation")
    print("3. Inspect browser network tab during login")
    print("4. Check PLANE GitHub repo documentation")

print("="*70)

#!/usr/bin/env python3
"""Check for PLANE API documentation endpoints"""

import requests

base_url = "http://localhost"
api_key = "plane_api_623e92aaced748629a60efcb603af928"

headers = {
    'X-Api-Key': api_key,
    'Accept': 'application/json'
}

print("="*70)
print("CHECKING FOR PLANE API DOCUMENTATION")
print("="*70)

# Try to find API documentation
doc_endpoints = [
    '/api/docs/',
    '/api/schema/',
    '/api/swagger/',
    '/api/redoc/',
    '/api/',
    '/swagger/',
    '/docs/',
    '/api-docs/',
]

print("\nLooking for API documentation endpoints...\n")

for endpoint in doc_endpoints:
    url = f"{base_url}{endpoint}"
    try:
        response = requests.get(url, headers=headers, timeout=5, allow_redirects=True)
        if response.status_code == 200:
            content_type = response.headers.get('content-type', '')
            print(f"✅ {endpoint}")
            print(f"   Status: {response.status_code}")
            print(f"   Content-Type: {content_type}")
            print(f"   URL: {url}")
            
            if 'json' in content_type:
                print(f"   Preview: {response.text[:200]}...")
            elif 'html' in content_type:
                print(f"   → HTML documentation available")
                print(f"   → Open in browser: {url}")
            print()
        elif response.status_code == 404:
            print(f"❌ {endpoint} - Not Found")
        else:
            print(f"⚠️  {endpoint} - Status: {response.status_code}")
    except Exception as e:
        print(f"❌ {endpoint} - Error: {e}")

print("\n" + "="*70)
print("CHECKING AUTHENTICATION ENDPOINTS")
print("="*70)

# Check for authentication/token endpoints
auth_endpoints = [
    '/api/auth/',
    '/api/token/',
    '/api/v1/auth/',
    '/api/v1/token/',
    '/api/sign-in/',
    '/api/auth/sign-in/',
    '/api/login/',
]

print("\nLooking for authentication endpoints...\n")

for endpoint in auth_endpoints:
    url = f"{base_url}{endpoint}"
    try:
        # Try GET first
        response = requests.get(url, timeout=5)
        if response.status_code in [200, 405]:  # 405 = Method Not Allowed (but exists)
            print(f"✅ {endpoint}")
            print(f"   Status: {response.status_code}")
            if response.status_code == 405:
                print(f"   → Try POST request (GET not allowed)")
            else:
                print(f"   Response: {response.text[:200]}")
            print()
    except Exception as e:
        pass

print("="*70)

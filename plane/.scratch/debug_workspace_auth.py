#!/usr/bin/env python3
"""Debug workspace authentication - try different approaches"""

import requests
import json

base_url = "http://localhost"
api_key = "plane_api_623e92aaced748629a60efcb603af928"
workspace_slug = "agent_c"

print("="*70)
print("PLANE WORKSPACE AUTHENTICATION DEBUG")
print("="*70)
print(f"Base URL: {base_url}")
print(f"Workspace: {workspace_slug}\n")

# Different authentication approaches to try
auth_variations = [
    ("X-Api-Key header", {'X-Api-Key': api_key}),
    ("Authorization Bearer", {'Authorization': f'Bearer {api_key}'}),
    ("x-api-key lowercase", {'x-api-key': api_key}),
    ("API-Key header", {'API-Key': api_key}),
]

# Different endpoint patterns to try
endpoint_patterns = [
    ("Standard", f"/api/workspaces/{workspace_slug}/"),
    ("With v1", f"/api/v1/workspaces/{workspace_slug}/"),
    ("Users endpoint", f"/api/v1/users/me/workspaces/"),
    ("Workspace detail", f"/api/workspaces/{workspace_slug}/me/"),
]

print("Testing different authentication methods and endpoints...")
print("-"*70)

successful_configs = []

for auth_name, auth_headers in auth_variations:
    print(f"\n🔐 Authentication Method: {auth_name}")
    
    for endpoint_name, endpoint in endpoint_patterns:
        url = f"{base_url}{endpoint}"
        headers = {
            **auth_headers,
            'Content-Type': 'application/json',
            'Accept': 'application/json'
        }
        
        try:
            response = requests.get(url, headers=headers, timeout=10)
            status = response.status_code
            
            result = f"   {'✅' if status == 200 else '❌'} {endpoint_name}: {endpoint}"
            print(result)
            
            if status == 200:
                print(f"      Status: {status} - SUCCESS!")
                try:
                    data = response.json()
                    print(f"      Data preview: {str(data)[:150]}...")
                    successful_configs.append({
                        'auth_method': auth_name,
                        'auth_headers': auth_headers,
                        'endpoint': endpoint,
                        'data': data
                    })
                except:
                    pass
            elif status == 401:
                try:
                    error = response.json()
                    print(f"      401 Unauthorized: {error.get('detail', 'No details')}")
                except:
                    pass
            elif status == 404:
                print(f"      404 Not Found")
            else:
                print(f"      Status: {status}")
                
        except Exception as e:
            print(f"   ❌ {endpoint_name}: Error - {e}")

# Try to get workspace info through user endpoint
print("\n" + "="*70)
print("ALTERNATIVE: Getting workspace through user profile")
print("="*70)

user_endpoint = f"{base_url}/api/v1/users/me/"
headers = {
    'X-Api-Key': api_key,
    'Content-Type': 'application/json'
}

try:
    response = requests.get(user_endpoint, headers=headers, timeout=10)
    if response.status_code == 200:
        user_data = response.json()
        print("✅ User data retrieved successfully")
        print(f"\nUser Info:")
        print(f"  Name: {user_data.get('first_name')} {user_data.get('last_name')}")
        print(f"  Email: {user_data.get('email')}")
        print(f"  ID: {user_data.get('id')}")
        
        # Check for workspace info in user data
        if 'workspace' in user_data:
            print(f"\n🏢 Workspace in user data:")
            print(json.dumps(user_data['workspace'], indent=2))
        
        if 'workspaces' in user_data:
            print(f"\n🏢 Workspaces in user data:")
            print(json.dumps(user_data['workspaces'], indent=2))
        
        # Print all keys to see what's available
        print(f"\nAvailable user data keys:")
        for key in user_data.keys():
            print(f"  - {key}")
            
except Exception as e:
    print(f"❌ Error getting user data: {e}")

# Summary
print("\n" + "="*70)
print("SUMMARY")
print("="*70)

if successful_configs:
    print(f"\n✅ Found {len(successful_configs)} working configuration(s):\n")
    for i, config in enumerate(successful_configs, 1):
        print(f"{i}. Auth: {config['auth_method']}")
        print(f"   Endpoint: {config['endpoint']}")
        print()
else:
    print("\n❌ No working workspace endpoint configurations found.")
    print("\nPossible issues:")
    print("1. API key may not have workspace-level permissions")
    print("2. Workspace endpoints may require different authentication")
    print("3. PLANE version may use different endpoint structure")
    print("4. May need to authenticate differently for workspace resources")
    
    print("\n💡 Next steps:")
    print("1. Check PLANE API documentation for workspace access")
    print("2. Verify API key permissions in PLANE settings")
    print("3. Check PLANE logs: docker logs plane-app-api-1 --tail 100")
    print("4. Try accessing workspace via web browser to confirm it exists")

print("="*70)

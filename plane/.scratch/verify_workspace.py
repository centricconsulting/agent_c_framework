#!/usr/bin/env python3
"""Verify workspace access and test key endpoints"""

import requests
import json

base_url = "http://localhost"
api_key = "plane_api_623e92aaced748629a60efcb603af928"
workspace_slug = "agent_c"

headers = {
    'X-Api-Key': api_key,
    'Content-Type': 'application/json'
}

print("="*70)
print("PLANE WORKSPACE VERIFICATION")
print("="*70)
print(f"Base URL: {base_url}")
print(f"Workspace: {workspace_slug}")
print(f"API Key: {'*' * 20}{api_key[-4:]}\n")

# Test endpoints
tests = [
    ("Workspace Info", f"/api/workspaces/{workspace_slug}/"),
    ("Projects List", f"/api/workspaces/{workspace_slug}/projects/"),
    ("Workspace Members", f"/api/workspaces/{workspace_slug}/members/"),
]

print("Testing workspace endpoints...")
print("-"*70)

results = {}

for test_name, endpoint in tests:
    url = f"{base_url}{endpoint}"
    print(f"\n🔍 {test_name}")
    print(f"   Endpoint: {endpoint}")
    
    try:
        response = requests.get(url, headers=headers, timeout=10)
        status = response.status_code
        
        if status == 200:
            data = response.json()
            results[test_name] = {'success': True, 'data': data}
            print(f"   ✅ Status: {status}")
            
            # Show preview of data
            if isinstance(data, dict):
                print(f"   Data keys: {list(data.keys())[:5]}")
                if 'name' in data:
                    print(f"   Name: {data['name']}")
            elif isinstance(data, list):
                print(f"   Count: {len(data)} items")
                if len(data) > 0 and isinstance(data[0], dict):
                    print(f"   First item keys: {list(data[0].keys())[:5]}")
        else:
            results[test_name] = {'success': False, 'status': status}
            print(f"   ❌ Status: {status}")
            try:
                error = response.json()
                print(f"   Error: {error}")
            except:
                print(f"   Response: {response.text[:200]}")
                
    except Exception as e:
        results[test_name] = {'success': False, 'error': str(e)}
        print(f"   ❌ Error: {e}")

# Summary
print("\n" + "="*70)
print("VERIFICATION SUMMARY")
print("="*70)

all_passed = all(r.get('success', False) for r in results.values())

for test_name, result in results.items():
    status = "✅ PASS" if result.get('success') else "❌ FAIL"
    print(f"{status} - {test_name}")

if all_passed:
    print("\n🎉 All tests passed! Workspace is accessible.")
    print("\n📝 Final Configuration:")
    print("-"*70)
    print(f"PLANE_INSTANCE_URL=http://localhost")
    print(f"PLANE_API_KEY=plane_api_623e92aaced748629a60efcb603af928")
    print(f"PLANE_WORKSPACE_SLUG=agent_c")
    print(f"PLANE_AUTH_HEADER=X-Api-Key")
    print("-"*70)
    
    # Show workspace details if available
    if 'Workspace Info' in results and results['Workspace Info'].get('success'):
        ws_data = results['Workspace Info']['data']
        print(f"\n🏢 Workspace Details:")
        print(f"   Name: {ws_data.get('name')}")
        print(f"   Slug: {ws_data.get('slug')}")
        print(f"   ID: {ws_data.get('id')}")
    
    # Show project count if available
    if 'Projects List' in results and results['Projects List'].get('success'):
        projects = results['Projects List']['data']
        print(f"\n📂 Projects: {len(projects)} total")
        if len(projects) > 0:
            print(f"   Sample projects:")
            for proj in projects[:3]:
                print(f"   - {proj.get('name')} (ID: {proj.get('id')})")
    
    print("\n✅ Ready to build PLANE tools!")
else:
    print("\n⚠️ Some endpoints failed. Check errors above.")

print("="*70)

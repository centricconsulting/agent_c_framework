#!/usr/bin/env python3
"""Test PLANE with ALL cookies from browser"""

import requests

base_url = "http://localhost"
workspace_slug = "agent_c"

# ALL cookies from the browser cURL
cookies = {
    'ajs_anonymous_id': 'b84a7c10-2aca-4bf2-9579-f281a47c03d5',
    'agentc-auth-token': 'eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJhZG1pbiIsInVzZXIiOnsidXNlcl9pZCI6ImFkbWluIiwidXNlcl9uYW1lIjoiYWRtaW4iLCJlbWFpbCI6ImNoYW5nZW1lQGNlbnRyaWNjb25zdWx0aW5nLmNvbSIsImZpcnN0X25hbWUiOiJBZG1pbiIsImxhc3RfbmFtZSI6IlVzZXIiLCJpc19hY3RpdmUiOnRydWUsInJvbGVzIjpbImFkbWluIl0sImdyb3VwcyI6W10sImNyZWF0ZWRfYXQiOiIyMDI1LTA5LTIzVDA3OjE0OjA3LjU0OTc2MiIsImxhc3RfbG9naW4iOm51bGx9LCJleHAiOjE3NjMwMzY4NjYsImlhdCI6MTc2MDM1ODQ2Nn0.bAzdpE0_siX_aBalWpYdGa-phULkb2WCFESx95BAHRk',
    'csrftoken': 'o4tAHfTcpMvysGKLaIEhw1qUS29yrDP1',
    'session-id': 'my5jxtxf45248zxyapvhh8hcbpfh9twp4tw6u491dzewnfnjc2wapwccwanxetpg5d00u8amq78zvjwdetdhdo8x8p8p3ps6xlbbl1nm4dvg9c59aj7ehuq3adahedna',
}

print("="*70)
print("TESTING WITH ALL BROWSER COOKIES")
print("="*70)

# Test endpoints
endpoints = [
    (f"/api/workspaces/{workspace_slug}/", "Workspace Info"),
    (f"/api/workspaces/{workspace_slug}/projects/", "Projects"),
    (f"/api/users/me/workspaces/", "User Workspaces"),
    (f"/api/users/me/", "User Profile"),
]

print("\nTesting API endpoints...")
print("-"*70)

session = requests.Session()
for name, value in cookies.items():
    session.cookies.set(name, value)

session.headers.update({
    'Accept': 'application/json',
    'Content-Type': 'application/json',
    'Referer': f'{base_url}/',
})

success_count = 0

for endpoint, description in endpoints:
    url = f"{base_url}{endpoint}"
    print(f"\n🔍 {description}")
    print(f"   {endpoint}")
    
    try:
        response = session.get(url, timeout=10)
        status = response.status_code
        
        if status == 200:
            print(f"   ✅ Status: {status} - SUCCESS!")
            success_count += 1
            
            try:
                data = response.json()
                if isinstance(data, list):
                    print(f"   📊 Found: {len(data)} items")
                    if len(data) > 0 and isinstance(data[0], dict):
                        first_item = data[0]
                        print(f"   Sample item:")
                        for key in list(first_item.keys())[:5]:
                            value = first_item[key]
                            if isinstance(value, str) and len(value) > 50:
                                value = value[:47] + "..."
                            print(f"      {key}: {value}")
                elif isinstance(data, dict):
                    print(f"   📊 Data keys: {list(data.keys())[:8]}")
                    if 'name' in data:
                        print(f"      Name: {data['name']}")
                    if 'email' in data:
                        print(f"      Email: {data['email']}")
            except Exception as e:
                print(f"   Response (non-JSON): {response.text[:100]}")
        else:
            print(f"   ❌ Status: {status}")
            try:
                error = response.json()
                print(f"   Error: {error}")
            except:
                print(f"   Response: {response.text[:100]}")
                
    except Exception as e:
        print(f"   ❌ Exception: {e}")

# Summary
print("\n" + "="*70)
print("RESULTS")
print("="*70)

if success_count == len(endpoints):
    print("\n🎉 SUCCESS! ALL ENDPOINTS WORKING!")
    print("\nConfiguration discovered:")
    print("-"*70)
    print("Required cookies:")
    for name in cookies.keys():
        print(f"  - {name}")
    print("\nThe 'session-id' cookie was the missing piece!")
    print("-"*70)
    
    print("\n✅ READY TO BUILD PLANE TOOLS!")
    print("\nNext steps:")
    print("1. Build core PLANE client with cookie authentication")
    print("2. Implement automatic cookie extraction/refresh")
    print("3. Create the 5 core toolsets")
    
elif success_count > 0:
    print(f"\n⚠️  Partial success: {success_count}/{len(endpoints)} endpoints working")
    print("\nSome endpoints work, but not all. Need to investigate further.")
else:
    print("\n❌ No endpoints working")
    print("\nThe cookies may have expired. Try:")
    print("1. Log out of PLANE")
    print("2. Log back in")
    print("3. Extract fresh cookies")
    print("4. Run this test again")

print("="*70)

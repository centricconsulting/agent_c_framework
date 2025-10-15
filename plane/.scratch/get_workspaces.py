#!/usr/bin/env python3
"""Quick script to get workspace information"""

import requests

base_url = "http://localhost"
api_key = "plane_api_623e92aaced748629a60efcb603af928"

headers = {
    'X-Api-Key': api_key,
    'Content-Type': 'application/json'
}

# Try different workspace endpoints
endpoints = [
    '/api/users/me/workspaces/',
    '/api/v1/users/me/workspaces/',
    '/api/workspaces/',
]

print("Searching for workspaces...\n")

for endpoint in endpoints:
    url = f"{base_url}{endpoint}"
    print(f"Trying: {endpoint}")
    
    try:
        response = requests.get(url, headers=headers, timeout=10)
        if response.status_code == 200:
            data = response.json()
            print(f"✅ Success! Found workspaces:\n")
            
            if isinstance(data, list):
                for ws in data:
                    print(f"  Workspace: {ws.get('name')}")
                    print(f"  Slug:      {ws.get('slug')}")
                    print(f"  ID:        {ws.get('id')}")
                    print()
            else:
                print(f"  {data}")
            break
        else:
            print(f"  ❌ {response.status_code}\n")
    except Exception as e:
        print(f"  ❌ Error: {e}\n")

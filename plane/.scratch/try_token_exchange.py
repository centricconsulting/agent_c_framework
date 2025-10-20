#!/usr/bin/env python3
"""Try to exchange API key for JWT token"""

import requests
import json

base_url = "http://localhost"
api_key = "plane_api_623e92aaced748629a60efcb603af928"

print("="*70)
print("ATTEMPTING TOKEN EXCHANGE")
print("="*70)

# Try to use API key to get a session token
# Different endpoints that might provide token exchange

attempts = [
    {
        'name': 'Direct workspace access with Cookie',
        'method': 'GET',
        'url': f'{base_url}/api/workspaces/agent_c/projects/',
        'headers': {
            'X-Api-Key': api_key,
            'Cookie': f'access_token={api_key}'
        }
    },
    {
        'name': 'Workspace with multiple auth headers',
        'method': 'GET',
        'url': f'{base_url}/api/workspaces/agent_c/projects/',
        'headers': {
            'X-Api-Key': api_key,
            'Authorization': f'Bearer {api_key}',
        }
    },
]

for attempt in attempts:
    print(f"\n🔄 {attempt['name']}")
    print(f"   Method: {attempt['method']}")
    print(f"   URL: {attempt['url']}")
    
    try:
        if attempt['method'] == 'GET':
            response = requests.get(
                attempt['url'],
                headers=attempt['headers'],
                timeout=10
            )
        else:
            response = requests.post(
                attempt['url'],
                headers=attempt['headers'],
                json=attempt.get('data'),
                timeout=10
            )
        
        print(f"   Status: {response.status_code}")
        
        if response.status_code == 200:
            print(f"   ✅ SUCCESS!")
            try:
                data = response.json()
                print(f"   Data: {json.dumps(data, indent=2)[:300]}...")
            except:
                print(f"   Response: {response.text[:200]}")
        else:
            print(f"   ❌ Failed")
            try:
                error = response.json()
                print(f"   Error: {error}")
            except:
                print(f"   Response: {response.text[:200]}")
                
    except Exception as e:
        print(f"   ❌ Exception: {e}")

print("\n" + "="*70)
print("If all attempts failed, we need to:")
print("1. Check PLANE API documentation for workspace authentication")
print("2. Review API key permissions in PLANE settings")
print("3. Consider using a different authentication method")
print("="*70)

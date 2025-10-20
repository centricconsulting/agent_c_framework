#!/usr/bin/env python3
"""Try to access PLANE API documentation with cookies"""

import requests

base_url = "http://localhost"

# Try with your current working cookies
cookies = {
    'session-id': 'my5jxtxf45248zxyapvhh8hcbpfh9twp4tw6u491dzewnfnjc2wapwccwanxetpg5d00u8amq78zvjwdetdhdo8x8p8p3ps6xlbbl1nm4dvg9c59aj7ehuq3adahedna',
    'agentc-auth-token': 'eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJhZG1pbiIsInVzZXIiOnsidXNlcl9pZCI6ImFkbWluIiwidXNlcl9uYW1lIjoiYWRtaW4iLCJlbWFpbCI6ImNoYW5nZW1lQGNlbnRyaWNjb25zdWx0aW5nLmNvbSIsImZpcnN0X25hbWUiOiJBZG1pbiIsImxhc3RfbmFtZSI6IlVzZXIiLCJpc19hY3RpdmUiOnRydWUsInJvbGVzIjpbImFkbWluIl0sImdyb3VwcyI6W10sImNyZWF0ZWRfYXQiOiIyMDI1LTA5LTIzVDA3OjE0OjA3LjU0OTc2MiIsImxhc3RfbG9naW4iOm51bGx9LCJleHAiOjE3NjMwMzY4NjYsImlhdCI6MTc2MDM1ODQ2Nn0.bAzdpE0_siX_aBalWpYdGa-phULkb2WCFESx95BAHRk',
    'csrftoken': 'o4tAHfTcpMvysGKLaIEhw1qUS29yrDP1',
}

print("="*70)
print("ACCESSING PLANE API DOCUMENTATION")
print("="*70)

doc_urls = [
    '/swagger/',
    '/api-docs/',
    '/docs/',
    '/api/schema/',
    '/api/swagger.json',
    '/api/openapi.json',
]

print("\nTrying to access API documentation with authentication...")
print("-"*70)

for doc_url in doc_urls:
    url = f"{base_url}{doc_url}"
    print(f"\n🔍 {doc_url}")
    
    try:
        response = requests.get(url, cookies=cookies, timeout=10)
        status = response.status_code
        
        if status == 200:
            print(f"   ✅ Status: {status}")
            content_type = response.headers.get('content-type', '')
            print(f"   Content-Type: {content_type}")
            
            if 'json' in content_type:
                print(f"   → JSON schema available")
                print(f"   → Saving to file...")
                
                with open(f'.scratch/plane_api_schema.json', 'w') as f:
                    f.write(response.text)
                print(f"   → Saved to: .scratch/plane_api_schema.json")
                
            elif 'html' in content_type:
                print(f"   → HTML documentation")
                print(f"   → Open in browser: {url}")
                
                # Try to extract useful info from HTML
                if 'swagger' in response.text.lower():
                    print(f"   → Contains Swagger UI")
                if 'openapi' in response.text.lower():
                    print(f"   → Uses OpenAPI spec")
        else:
            print(f"   ❌ Status: {status}")
            
    except Exception as e:
        print(f"   ❌ Error: {e}")

print("\n" + "="*70)
print("SUMMARY")
print("="*70)
print("""
If API docs are HTML-based:
1. Open the URL in your browser (while logged in)
2. Look for authentication/login endpoints
3. Copy the endpoint details

If you can't access docs:
1. Capture login request from browser Network tab
2. We'll reverse-engineer from there
""")
print("="*70)

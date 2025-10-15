#!/usr/bin/env python3
"""
PLANE API Discovery Script

This script discovers the correct API endpoints and authentication method
for a PLANE instance by testing various common patterns.
"""

import requests
import json
from typing import Dict, Any, Optional, Tuple


class PlaneApiDiscovery:
    """Discover PLANE API configuration"""
    
    def __init__(self, base_url: str, api_key: str):
        self.base_url = base_url.rstrip('/')
        self.api_key = api_key
        
    def _test_endpoint(self, endpoint: str, auth_header: Dict[str, str]) -> Tuple[bool, Optional[Dict]]:
        """Test an endpoint with specific auth header"""
        url = f"{self.base_url}{endpoint}"
        headers = {
            **auth_header,
            'Content-Type': 'application/json',
            'Accept': 'application/json'
        }
        
        try:
            response = requests.get(url, headers=headers, timeout=10)
            
            # Try to parse as JSON
            try:
                data = response.json()
            except:
                data = None
            
            success = response.status_code in [200, 201]
            
            return success, {
                'status_code': response.status_code,
                'data': data,
                'text': response.text[:200] if not data else None
            }
        except Exception as e:
            return False, {'error': str(e)}
    
    def discover(self) -> Dict[str, Any]:
        """Run discovery process"""
        print("="*70)
        print("PLANE API DISCOVERY")
        print("="*70)
        print(f"Base URL: {self.base_url}")
        print(f"API Key: {'*' * 20}{self.api_key[-4:]}\n")
        
        results = {
            'working_config': None,
            'tests': []
        }
        
        # Different authentication methods to try
        auth_methods = [
            ('X-Api-Key', {'X-Api-Key': self.api_key}),
            ('Authorization Bearer', {'Authorization': f'Bearer {self.api_key}'}),
            ('Authorization Token', {'Authorization': f'Token {self.api_key}'}),
        ]
        
        # Different endpoint patterns to try
        endpoints = [
            '/api/v1/users/me/',
            '/api/users/me/',
            '/api/v1/workspaces/',
            '/api/workspaces/',
            '/api/instances/',
            '/api/v1/instances/',
            '/api/auth/profile/',
            '/api/v1/auth/profile/',
        ]
        
        print("Testing authentication methods and endpoints...")
        print("-"*70)
        
        for auth_name, auth_header in auth_methods:
            print(f"\n🔐 Trying authentication: {auth_name}")
            
            for endpoint in endpoints:
                success, result = self._test_endpoint(endpoint, auth_header)
                
                test_result = {
                    'auth_method': auth_name,
                    'endpoint': endpoint,
                    'success': success,
                    'result': result
                }
                results['tests'].append(test_result)
                
                status = "✅" if success else "❌"
                status_code = result.get('status_code', 'ERROR')
                print(f"  {status} {endpoint} → {status_code}")
                
                if success:
                    print(f"     🎉 FOUND WORKING ENDPOINT!")
                    if result.get('data'):
                        print(f"     Data: {json.dumps(result['data'], indent=6)[:200]}...")
                    
                    if not results['working_config']:
                        results['working_config'] = {
                            'auth_method': auth_name,
                            'auth_header': auth_header,
                            'endpoint': endpoint,
                            'base_api_path': endpoint.rsplit('/', 2)[0] if '/' in endpoint else '/api'
                        }
        
        # Print summary
        print("\n" + "="*70)
        print("DISCOVERY SUMMARY")
        print("="*70)
        
        if results['working_config']:
            config = results['working_config']
            print("\n✅ SUCCESS! Found working API configuration:\n")
            print(f"  Base URL:      {self.base_url}")
            print(f"  Auth Method:   {config['auth_method']}")
            print(f"  API Base Path: {config['base_api_path']}")
            print(f"  Test Endpoint: {config['endpoint']}")
            
            print("\n📝 Configuration to use:")
            print("-"*70)
            print(f"PLANE_INSTANCE_URL={self.base_url}")
            print(f"PLANE_API_KEY={self.api_key}")
            print(f"PLANE_AUTH_HEADER={config['auth_method']}")
            print(f"PLANE_API_BASE={config['base_api_path']}")
            print("-"*70)
            
            # Try to get workspace info with working config
            print("\n🏢 Attempting to fetch workspaces...")
            workspace_endpoints = [
                f"{config['base_api_path']}/workspaces/",
                "/api/v1/workspaces/",
                "/api/workspaces/",
            ]
            
            for ws_endpoint in workspace_endpoints:
                success, result = self._test_endpoint(ws_endpoint, config['auth_header'])
                if success and result.get('data'):
                    print(f"✅ Found workspaces at: {ws_endpoint}")
                    data = result['data']
                    if isinstance(data, list) and len(data) > 0:
                        print(f"\n📊 Workspace Information:")
                        for ws in data:
                            print(f"\n  Workspace: {ws.get('name')}")
                            print(f"  Slug:      {ws.get('slug')}")
                            print(f"  ID:        {ws.get('id')}")
                            print(f"  Owner:     {ws.get('owner', {}).get('email', 'N/A')}")
                    break
            
        else:
            print("\n❌ Could not find working API configuration")
            print("\nPossible issues:")
            print("  1. API key may be invalid or expired")
            print("  2. PLANE instance may use a different API structure")
            print("  3. API may require additional authentication")
            print("  4. PLANE version may be different than expected")
            
            print("\n💡 Suggestions:")
            print("  1. Check PLANE documentation for your version")
            print("  2. Verify API key in PLANE settings")
            print("  3. Check PLANE logs: docker logs plane-app-api-1")
            print("  4. Try accessing API docs: http://localhost/api/docs")
        
        print("\n" + "="*70)
        
        return results


def main():
    print("\nPLANE API Discovery Tool")
    print("="*70)
    print("\nThis will test various API endpoints to find the correct configuration.\n")
    
    base_url = input("PLANE Instance URL (e.g., http://localhost): ").strip()
    api_key = input("PLANE API Key: ").strip()
    
    if not base_url or not api_key:
        print("\n❌ Error: Both URL and API key are required!")
        return
    
    discovery = PlaneApiDiscovery(base_url, api_key)
    results = discovery.discover()
    
    exit(0 if results['working_config'] else 1)


if __name__ == '__main__':
    main()

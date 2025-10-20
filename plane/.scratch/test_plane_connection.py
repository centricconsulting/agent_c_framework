#!/usr/bin/env python3
"""
PLANE API Connection Test Script

This script tests the connection to a local PLANE instance and verifies:
1. PLANE is accessible
2. API key authentication works
3. Can retrieve workspace information
4. API endpoints are responding correctly

Usage:
    python test_plane_connection.py

Requirements:
    pip install requests
"""

import requests
import json
from typing import Dict, Any, Optional


class PlaneConnectionTest:
    """Test connection to PLANE API"""
    
    def __init__(self, base_url: str, api_key: str):
        """
        Initialize connection tester
        
        Args:
            base_url: PLANE instance URL (e.g., http://localhost:3000)
            api_key: Your PLANE API key
        """
        self.base_url = base_url.rstrip('/')
        self.api_key = api_key
        self.headers = {
            'X-Api-Key': api_key,
            'Content-Type': 'application/json'
        }
        
    def _make_request(self, method: str, endpoint: str, **kwargs) -> Optional[Dict[str, Any]]:
        """Make HTTP request to PLANE API"""
        url = f"{self.base_url}{endpoint}"
        
        try:
            response = requests.request(
                method=method,
                url=url,
                headers=self.headers,
                timeout=10,
                **kwargs
            )
            
            print(f"\n{'='*60}")
            print(f"Request: {method} {url}")
            print(f"Status Code: {response.status_code}")
            print(f"{'='*60}")
            
            if response.status_code == 200:
                data = response.json()
                print("✅ Success!")
                print(f"Response preview: {json.dumps(data, indent=2)[:500]}...")
                return data
            else:
                print(f"❌ Error: {response.status_code}")
                print(f"Response: {response.text[:500]}")
                return None
                
        except requests.exceptions.ConnectionError:
            print(f"❌ Connection Error: Cannot connect to {url}")
            print("   Make sure PLANE is running and the URL is correct")
            return None
        except requests.exceptions.Timeout:
            print(f"❌ Timeout: Request to {url} timed out")
            return None
        except Exception as e:
            print(f"❌ Unexpected Error: {str(e)}")
            return None
    
    def test_health(self) -> bool:
        """Test if PLANE API is accessible"""
        print("\n🔍 Testing PLANE API Health...")
        
        # Try common health check endpoints
        endpoints = [
            '/api/',
            '/api/v1/',
            '/health',
            '/'
        ]
        
        for endpoint in endpoints:
            result = self._make_request('GET', endpoint)
            if result is not None:
                return True
        
        return False
    
    def test_authentication(self) -> bool:
        """Test API key authentication"""
        print("\n🔐 Testing API Authentication...")
        
        # Try to access user profile endpoint
        result = self._make_request('GET', '/api/v1/users/me/')
        
        if result:
            print(f"✅ Authenticated as: {result.get('email', 'Unknown')}")
            return True
        
        return False
    
    def test_workspaces(self) -> Optional[list]:
        """Retrieve workspace information"""
        print("\n🏢 Fetching Workspaces...")
        
        result = self._make_request('GET', '/api/v1/workspaces/')
        
        if result:
            if isinstance(result, list) and len(result) > 0:
                print(f"✅ Found {len(result)} workspace(s):")
                for ws in result:
                    print(f"   - Name: {ws.get('name')}")
                    print(f"     Slug: {ws.get('slug')}")
                    print(f"     ID: {ws.get('id')}")
                return result
            else:
                print("⚠️  No workspaces found")
        
        return None
    
    def test_projects(self, workspace_slug: str) -> Optional[list]:
        """Test fetching projects from a workspace"""
        print(f"\n📂 Fetching Projects from workspace '{workspace_slug}'...")
        
        result = self._make_request('GET', f'/api/v1/workspaces/{workspace_slug}/projects/')
        
        if result:
            if isinstance(result, list):
                print(f"✅ Found {len(result)} project(s)")
                for proj in result[:3]:  # Show first 3
                    print(f"   - {proj.get('name')} (ID: {proj.get('id')})")
                return result
            else:
                print("⚠️  Unexpected response format")
        
        return None
    
    def run_all_tests(self) -> Dict[str, bool]:
        """Run all connection tests"""
        print("="*60)
        print("PLANE API CONNECTION TEST")
        print("="*60)
        print(f"Base URL: {self.base_url}")
        print(f"API Key: {'*' * 20}{self.api_key[-4:]}")
        
        results = {
            'health': False,
            'auth': False,
            'workspaces': False,
            'projects': False
        }
        
        # Test 1: Health check
        results['health'] = self.test_health()
        
        if not results['health']:
            print("\n❌ Cannot reach PLANE API. Please check:")
            print("   1. PLANE Docker container is running (docker ps)")
            print("   2. URL is correct (check port mapping)")
            print("   3. No firewall blocking the connection")
            return results
        
        # Test 2: Authentication
        results['auth'] = self.test_authentication()
        
        if not results['auth']:
            print("\n❌ Authentication failed. Please check:")
            print("   1. API key is correct")
            print("   2. API key hasn't expired")
            print("   3. API key has proper permissions")
            return results
        
        # Test 3: Workspaces
        workspaces = self.test_workspaces()
        results['workspaces'] = workspaces is not None
        
        if workspaces:
            # Test 4: Projects (use first workspace)
            workspace_slug = workspaces[0].get('slug')
            if workspace_slug:
                projects = self.test_projects(workspace_slug)
                results['projects'] = projects is not None
        
        # Summary
        print("\n" + "="*60)
        print("TEST SUMMARY")
        print("="*60)
        
        all_passed = all(results.values())
        
        for test, passed in results.items():
            status = "✅ PASS" if passed else "❌ FAIL"
            print(f"{status} - {test.upper()}")
        
        if all_passed:
            print("\n🎉 All tests passed! PLANE API is working correctly.")
            print("\nConfiguration to use:")
            if workspaces:
                ws = workspaces[0]
                print(f"  PLANE_INSTANCE_URL={self.base_url}")
                print(f"  PLANE_API_KEY={self.api_key}")
                print(f"  PLANE_WORKSPACE_SLUG={ws.get('slug')}")
                print(f"  PLANE_WORKSPACE_ID={ws.get('id')}")
        else:
            print("\n⚠️  Some tests failed. Review the errors above.")
        
        print("="*60)
        
        return results


def main():
    """Main function to run connection tests"""
    
    print("\nPLANE API Connection Test")
    print("=" * 60)
    print("\nThis script will test your connection to PLANE.")
    print("Please provide the following information:\n")
    
    # Get user input
    base_url = input("PLANE Instance URL (e.g., http://localhost:3000): ").strip()
    api_key = input("PLANE API Key: ").strip()
    
    if not base_url or not api_key:
        print("\n❌ Error: Both URL and API key are required!")
        return
    
    # Run tests
    tester = PlaneConnectionTest(base_url, api_key)
    results = tester.run_all_tests()
    
    # Exit with appropriate code
    exit(0 if all(results.values()) else 1)


if __name__ == '__main__':
    main()

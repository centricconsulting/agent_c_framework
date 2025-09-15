#!/usr/bin/env python3
"""
Test script to create UiPath asset with the exact payload structure shown by the user
"""

import json
import requests
import os

def test_exact_payload():
    """Test with the exact payload structure provided by the user"""
    
    # Configuration
    org_name = "centrusjldws"
    tenant_name = "DefaultTenant"
    folder_id = 3310023
    client_id = "885ffd53-2db7-480d-baed-563c20293da1"
    client_secret = "giXACS$axxocj5s1$UHc?4vbSSCbP03cvn1Q#nVny(8)!sd@oa^*y4Dq~!l5kHkd"
    scope = "OR.Assets OR.Assets.Read OR.Assets.Write"
    
    # Get token
    print("🔧 Getting authentication token...")
    token_url = f"https://cloud.uipath.com/{org_name}/identity_/connect/token"
    
    token_data = {
        "client_id": client_id,
        "client_secret": client_secret,
        "scope": scope,
        "grant_type": "client_credentials",
    }
    
    token_response = requests.post(token_url, data=token_data)
    if token_response.status_code != 200:
        print(f"❌ Token request failed: {token_response.status_code} - {token_response.text}")
        return
    
    token = token_response.json().get("access_token")
    print("✅ Got authentication token")
    
    # Create Boolean asset with EXACT payload from user
    print("\n🔧 Creating Boolean asset with user's exact payload...")
    
    create_asset_url = f"https://cloud.uipath.com/{org_name}/{tenant_name}/orchestrator_/odata/Assets"
    
    headers = {
        "Authorization": f"Bearer {token}",
        "X-UIPATH-TenantName": tenant_name,
        "X-UIPATH-OrganizationUnitId": str(folder_id),
        "Content-Type": "application/json",
        "Accept": "application/json"
    }
    
    # This is the EXACT payload structure provided by the user
    payload = {
        "Name": "TestAsset-boolean-exact",
        "ValueType": "Bool",
        "ValueScope": "Global",
        "Description": "Created via AgentC",
        "BoolValue": True  # Python boolean, will serialize to JSON true
    }
    
    print(f"URL: {create_asset_url}")
    print(f"Headers: {json.dumps({k: v if k != 'Authorization' else 'Bearer ***' for k, v in headers.items()}, indent=2)}")
    print(f"Payload: {json.dumps(payload, indent=2)}")
    
    response = requests.post(create_asset_url, headers=headers, data=json.dumps(payload))
    
    print(f"\nResponse Status: {response.status_code}")
    print(f"Response Headers: {dict(response.headers)}")
    print(f"Response Body: {response.text}")
    
    if response.status_code == 201:
        print("✅ Boolean asset created successfully with exact payload!")
        result = response.json()
        print(f"Asset ID: {result.get('Id')}")
    else:
        print("❌ Boolean asset creation failed even with exact payload")

if __name__ == "__main__":
    test_exact_payload()
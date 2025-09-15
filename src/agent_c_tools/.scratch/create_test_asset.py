#!/usr/bin/env python3
"""
Direct test of UiPath asset creation with exact payload matching
"""

import json
import requests
import os

def test_direct_boolean_creation():
    """Test Boolean asset creation with exact payload from working code"""
    
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
    
    # Create Boolean asset with EXACT payload from working code
    print("\n🔧 Creating Boolean asset with exact working payload...")
    
    create_asset_url = f"https://cloud.uipath.com/{org_name}/{tenant_name}/orchestrator_/odata/Assets"
    
    headers = {
        "Authorization": f"Bearer {token}",
        "X-UIPATH-TenantName": tenant_name,
        "X-UIPATH-OrganizationUnitId": str(folder_id),
        "Content-Type": "application/json",
        "Accept": "application/json"
    }
    
    # This is the EXACT payload structure from the working createAsset.py
    payload = {
        "Name": "DirectTestBoolean",
        "StringValue": "true",
        "ValueType": "Boolean",
        "ValueScope": "Global",
        "Description": "Direct test boolean asset"
    }
    
    print(f"URL: {create_asset_url}")
    print(f"Headers: {json.dumps({k: v if k != 'Authorization' else 'Bearer ***' for k, v in headers.items()}, indent=2)}")
    print(f"Payload: {json.dumps(payload, indent=2)}")
    
    response = requests.post(create_asset_url, headers=headers, data=json.dumps(payload))
    
    print(f"\nResponse Status: {response.status_code}")
    print(f"Response Body: {response.text}")
    
    if response.status_code == 201:
        print("✅ Boolean asset created successfully with direct approach!")
        result = response.json()
        print(f"Asset ID: {result.get('Id')}")
    else:
        print("❌ Boolean asset creation failed even with direct approach")
        
        # Try with different boolean values
        print("\n🔧 Trying with different boolean formats...")
        
        test_values = ["True", "TRUE", "1", "true"]
        
        for test_val in test_values:
            print(f"\nTrying with StringValue = '{test_val}'...")
            test_payload = payload.copy()
            test_payload["Name"] = f"DirectTestBoolean_{test_val}"
            test_payload["StringValue"] = test_val
            
            test_response = requests.post(create_asset_url, headers=headers, data=json.dumps(test_payload))
            print(f"Status: {test_response.status_code}")
            if test_response.status_code == 201:
                print(f"✅ SUCCESS with '{test_val}'!")
                break
            else:
                print(f"❌ Failed: {test_response.text}")

if __name__ == "__main__":
    test_direct_boolean_creation()
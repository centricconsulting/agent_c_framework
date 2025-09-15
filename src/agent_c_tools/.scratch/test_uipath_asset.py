#!/usr/bin/env python3
"""
Test script to replicate the working UiPath asset creation
"""
import json
import requests
import os

# Configuration (using same values from the working example)
org_name = "centrusjldws"
tenant_name = "DefaultTenant"
folder_id = 3310023
client_id = "885ffd53-2db7-480d-baed-563c20293da1"
client_secret = "giXACS$axxocj5s1$UHc?4vbSSCbP03cvn1Q#nVny(8)!sd@oa^*y4Dq~!l5kHkd"
scope = "OR.Assets OR.Assets.Read OR.Assets.Write"

def get_token():
    """Get OAuth2 token using the working method"""
    url = f"https://cloud.uipath.com/{org_name}/identity_/connect/token"
    
    data = {
        "client_id": client_id,
        "client_secret": client_secret,
        "scope": scope,
        "grant_type": "client_credentials",
    }
    
    try:
        response = requests.post(url, data=data)
        response.raise_for_status()
        token = response.json().get("access_token")
        print("✓ Token retrieved successfully")
        return token
    except requests.RequestException as e:
        print(f"✗ Failed to retrieve token: {e}")
        return None

def create_boolean_asset(token):
    """Create a boolean asset using the exact working method"""
    create_asset_url = f"https://cloud.uipath.com/{org_name}/{tenant_name}/orchestrator_/odata/Assets"
    
    headers = {
        "Authorization": f"Bearer {token}",
        "X-UIPATH-TenantName": tenant_name,
        "X-UIPATH-OrganizationUnitId": str(folder_id),
        "Content-Type": "application/json",
        "Accept": "application/json"
    }
    
    payload = {
        "Name": "test_boolean_from_script",
        "StringValue": "true",
        "ValueType": "Boolean",
        "ValueScope": "Global",
        "Description": "Test boolean asset created by debug script"
    }
    
    print(f"Sending payload: {json.dumps(payload, indent=2)}")
    print(f"Headers: {json.dumps({k: v if k != 'Authorization' else 'Bearer ***' for k, v in headers.items()}, indent=2)}")
    
    response = requests.post(create_asset_url, headers=headers, data=json.dumps(payload))
    
    print(f"Response status: {response.status_code}")
    print(f"Response body: {response.text}")
    
    if response.status_code == 201:
        print("✓ Asset created successfully!")
        return response.json()
    else:
        print(f"✗ Failed to create asset. Status: {response.status_code}")
        return None

if __name__ == "__main__":
    print("Testing UiPath asset creation...")
    
    # Get token
    token = get_token()
    if not token:
        print("Cannot proceed without token")
        exit(1)
    
    # Create asset
    result = create_boolean_asset(token)
    if result:
        print(f"Asset created with ID: {result.get('Id')}")
    else:
        print("Asset creation failed")
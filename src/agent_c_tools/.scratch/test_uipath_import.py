#!/usr/bin/env python3
"""
Test UiPath asset creation using the exact working approach
"""

import sys
import os

# Add the UiPath workspace to path
sys.path.insert(0, '/workspaces/uipath')

from getToken import UiPathAuth
from createAsset import CreateAsset

def test_working_approach():
    """Test using the exact working createAsset.py approach"""
    
    print("🔧 Testing with the working createAsset.py approach...")
    
    # Configuration
    org_name = "centrusjldws"
    tenant_name = "DefaultTenant"
    folder_id = 3310023
    client_id = "885ffd53-2db7-480d-baed-563c20293da1"
    client_secret = "giXACS$axxocj5s1$UHc?4vbSSCbP03cvn1Q#nVny(8)!sd@oa^*y4Dq~!l5kHkd"
    scope = "OR.Assets OR.Assets.Read OR.Assets.Write"
    
    # Get authentication token
    auth = UiPathAuth(
        org_name=org_name,
        client_id=client_id,
        client_secret=client_secret,
        scope=scope,
        tenant_name=tenant_name
    )
    
    token = auth.get_token()
    if not token:
        print("❌ Failed to get authentication token")
        return
    
    print("✅ Got authentication token")
    
    # Test Boolean asset creation
    print("\n🔧 Creating Boolean asset with working approach...")
    
    asset = CreateAsset(
        asset_name="WorkingBooleanTest",
        asset_type="Boolean",
        asset_value="true",
        create_asset_url=f"https://cloud.uipath.com/{org_name}/{tenant_name}/orchestrator_/odata/Assets",
        tenant_name=tenant_name,
        folder_id=folder_id
    )
    
    result = asset.create_asset(token)
    if result:
        print("✅ Boolean asset created successfully using working approach!")
        print(f"Asset ID: {result.get('Id')}")
    else:
        print("❌ Boolean asset creation failed even with working approach")

if __name__ == "__main__":
    test_working_approach()
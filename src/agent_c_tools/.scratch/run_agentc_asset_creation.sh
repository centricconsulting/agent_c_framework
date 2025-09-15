#!/bin/bash

# Set up environment variables
export UIPATH_ORG_NAME="centrusjldws"
export UIPATH_TENANT_NAME="DefaultTenant" 
export UIPATH_FOLDER_ID="3310023"
export UIPATH_CLIENT_ID="885ffd53-2db7-480d-baed-563c20293da1"
export UIPATH_CLIENT_SECRET="giXACS\$axxocj5s1\$UHc?4vbSSCbP03cvn1Q#nVny(8)!sd@oa^*y4Dq~!l5kHkd"

# Run the test script
cd /workspaces/tools/.scratch
python3 create_asset_via_agentc.py
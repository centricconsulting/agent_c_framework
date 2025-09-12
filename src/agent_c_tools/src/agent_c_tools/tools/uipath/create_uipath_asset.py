#!/usr/bin/env python3
"""
Create UiPath asset using the configured credentials from .env file

This script will:
1. Load UiPath configuration from the .env file
2. Test the connection to UiPath Cloud
3. Create the requested asset: TestAsset
"""

import asyncio
import sys
import os
from dotenv import load_dotenv

# Get the directory of this script
script_dir = os.path.dirname(os.path.abspath(__file__))
project_root = os.path.dirname(script_dir)

# Load environment variables from .env file
env_path = os.path.join(project_root, '.env')
load_dotenv(env_path)

print(f"Loading environment from: {env_path}")
print(f"UIPATH_ORG_NAME: {os.getenv('UIPATH_ORG_NAME')}")
print(f"UIPATH_TENANT_NAME: {os.getenv('UIPATH_TENANT_NAME')}")
print(f"UIPATH_FOLDER_ID: {os.getenv('UIPATH_FOLDER_ID')}")
print(f"UIPATH_PAT_TOKEN configured: {'Yes' if os.getenv('UIPATH_PAT_TOKEN') else 'No'}")
print()

# Add the src directory to the path so we can import agent_c_tools
src_path = os.path.join(project_root, 'src')
sys.path.insert(0, src_path)
print(f"Added to Python path: {src_path}")

from agent_c_tools.tools.uipath.tool import UiPathTools

async def create_asset():
    """Create the requested UiPath asset"""
    
    # Initialize the UiPath tools
    uipath_tool = UiPathTools()
    
    # Mock tool context (normally provided by the agent framework)
    tool_context = {
        'session_id': 'asset_creation_session',
        'current_user_username': 'tim_toolman',
        'timestamp': '2024-12-13T00:00:00Z',
        'env_name': 'production',
        'agent_config': {},
        'client_wants_cancel': None,
        'streaming_callback': None,
        'calling_model_name': 'claude-3-5-sonnet'
    }
    
    print("🔧 Testing UiPath connection first...")
    
    # Test connection first
    try:
        connection_result = await uipath_tool.test_connection(tool_context=tool_context)
        print("Connection test result:")
        print(connection_result)
        print()
    except Exception as e:
        print(f"❌ Connection test failed: {str(e)}")
        return
    
    print("🔧 Creating UiPath asset...")
    
    # Create the asset with the user's specifications
    try:
        result = await uipath_tool.create_asset(
            asset_name="TestAsset",
            asset_value="test value", 
            asset_type="Text",
            description="this asset is created by agentC",
            tool_context=tool_context
        )
        
        print("✅ Asset creation result:")
        print(result)
        
    except Exception as e:
        print(f"❌ Asset creation failed: {str(e)}")

if __name__ == "__main__":
    asyncio.run(create_asset())
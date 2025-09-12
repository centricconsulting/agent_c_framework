#!/usr/bin/env python3
"""
Create a UiPath asset with the user's specified parameters
"""

import asyncio
import sys
import os

# Load environment variables from .env file
try:
    from dotenv import load_dotenv
    # Look for .env file in the current directory and parent directories
    env_path = os.path.join(os.path.dirname(__file__), '..', '.env')
    if os.path.exists(env_path):
        load_dotenv(env_path)
        print(f"✅ Loaded .env file from: {env_path}")
    else:
        # Try loading from current directory
        load_dotenv()
        print("✅ Attempted to load .env file from current directory")
except ImportError:
    print("⚠️  python-dotenv not available, trying manual .env loading...")
    # Manual .env loading as fallback
    env_path = os.path.join(os.path.dirname(__file__), '..', '.env')
    if os.path.exists(env_path):
        with open(env_path, 'r') as f:
            for line in f:
                line = line.strip()
                if line and not line.startswith('#') and '=' in line:
                    key, value = line.split('=', 1)
                    os.environ[key.strip()] = value.strip().strip('"').strip("'")
        print(f"✅ Manually loaded .env file from: {env_path}")

# Add the src directory to the path so we can import agent_c_tools
sys.path.insert(0, os.path.join(os.path.dirname(__file__), '..', 'src'))

# Debug: Print some environment variables (without exposing secrets)
print("🔍 Environment check:")
print(f"   UIPATH_ORG_NAME: {'✅ Set' if os.getenv('UIPATH_ORG_NAME') else '❌ Not set'}")
print(f"   UIPATH_TENANT_NAME: {'✅ Set' if os.getenv('UIPATH_TENANT_NAME') else '❌ Not set'}")
print(f"   UIPATH_FOLDER_ID: {'✅ Set' if os.getenv('UIPATH_FOLDER_ID') else '❌ Not set'}")
print(f"   UIPATH_PAT_TOKEN: {'✅ Set' if os.getenv('UIPATH_PAT_TOKEN') else '❌ Not set'}")
print(f"   UIPATH_CLIENT_ID: {'✅ Set' if os.getenv('UIPATH_CLIENT_ID') else '❌ Not set'}")
print(f"   UIPATH_CLIENT_SECRET: {'✅ Set' if os.getenv('UIPATH_CLIENT_SECRET') else '❌ Not set'}")
print()

from agent_c_tools.tools.uipath.tool import UiPathTools

async def create_user_asset():
    """Create the UiPath asset as requested by the user"""
    
    print("🚀 Initializing UiPath tools...")
    
    # Initialize the UiPath tools with required parameters
    uipath_tool = UiPathTools(
        name="UiPathTools",
        tool_cache=None,  # We'll use None for standalone testing
        tool_chest=None   # We'll use None for standalone testing
    )
    
    # Mock tool context (normally provided by the agent framework)
    tool_context = {
        'session_id': 'user_request_session',
        'current_user_username': 'user',
        'timestamp': '2024-12-19T00:00:00Z',
        'env_name': 'production',
        'agent_config': {},
        'client_wants_cancel': None,
        'streaming_callback': None,
        'calling_model_name': 'claude'
    }
    
    print("📋 Creating asset with the following parameters:")
    print("   Asset Name: TestAsset")
    print("   Asset Value: test value")
    print("   Asset Type: Text")
    print("   Description: this asset is created by agentC")
    print()
    
    try:
        # First test the connection
        print("🔍 Testing UiPath connection...")
        connection_result = await uipath_tool.test_connection(tool_context=tool_context)
        print("Connection test result:")
        print(connection_result)
        print()
        
        # Create the asset
        print("🏗️  Creating the asset...")
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
        print(f"❌ Error occurred: {str(e)}")
        import traceback
        traceback.print_exc()

if __name__ == "__main__":
    asyncio.run(create_user_asset())
#!/usr/bin/env python3
"""
Simple test to create UiPath asset using Agent C toolset pattern
"""

import asyncio
import sys
import os
import threading

# Load environment variables from .env file
try:
    from dotenv import load_dotenv
    env_path = os.path.join(os.path.dirname(__file__), '..', '.env')
    if os.path.exists(env_path):
        load_dotenv(env_path)
        print(f"✅ Loaded .env file from: {env_path}")
    else:
        load_dotenv()
        print("✅ Attempted to load .env file from current directory")
except ImportError:
    print("⚠️  python-dotenv not available, trying manual .env loading...")
    env_path = os.path.join(os.path.dirname(__file__), '..', '.env')
    if os.path.exists(env_path):
        with open(env_path, 'r') as f:
            for line in f:
                line = line.strip()
                if line and not line.startswith('#') and '=' in line:
                    key, value = line.split('=', 1)
                    os.environ[key.strip()] = value.strip().strip('"').strip("'")
        print(f"✅ Manually loaded .env file from: {env_path}")

# Add the src directory to the path
sys.path.insert(0, os.path.join(os.path.dirname(__file__), '..', 'src'))

async def main():
    """Create the UiPath asset as requested by Tim"""
    
    print("🚀 Creating UiPath asset via Agent C toolset...")
    
    # Import the UiPath toolset
    from agent_c_tools.tools.uipath.tool import UiPathTools
    
    # Initialize the toolset (same as the working test script)
    uipath_tool = UiPathTools(
        name="UiPathTools",
        tool_cache=None,
        tool_chest=None
    )
    
    # Create tool context
    tool_context = {
        'session_id': 'tim_request_session',
        'current_user_username': 'tim_toolman',
        'timestamp': '2024-12-19T12:00:00Z',
        'env_name': 'production',
        'agent_config': {},
        'client_wants_cancel': threading.Event(),
        'streaming_callback': None,
        'calling_model_name': 'claude'
    }
    
    print("📋 Creating asset with Tim's specified parameters:")
    print("   Asset Name: TestAsset")
    print("   Asset Value: asset value")
    print("   Asset Type: Text")
    print("   Description: from agnetC")
    print()
    
    try:
        # Test connection first
        print("🔍 Testing UiPath connection...")
        connection_result = await uipath_tool.test_connection(tool_context=tool_context)
        print("Connection test result:")
        print(connection_result)
        print()
        
        # Create the asset with Tim's exact specifications
        print("🏗️  Creating the asset...")
        result = await uipath_tool.create_asset(
            asset_name="TestAsset",
            asset_value="asset value",
            asset_type="Text",
            description="from agnetC",
            tool_context=tool_context
        )
        
        print("✅ Asset creation result:")
        print(result)
        
    except Exception as e:
        print(f"❌ Error occurred: {str(e)}")
        import traceback
        traceback.print_exc()

if __name__ == "__main__":
    asyncio.run(main())
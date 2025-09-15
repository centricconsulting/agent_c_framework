#!/usr/bin/env python3
"""
Test script to demonstrate creating different types of UiPath assets.

This script will create examples of all supported asset types:
- Text asset
- Integer asset
- Boolean asset
- Credential asset
"""

import asyncio
import sys
import os
from dotenv import load_dotenv

# Get the directory of this script
script_dir = os.path.dirname(os.path.abspath(__file__))
project_root = os.path.dirname(os.path.dirname(os.path.dirname(script_dir)))

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

async def test_all_asset_types():
    """Test creating all types of UiPath assets"""
    
    # Initialize the UiPath tools
    uipath_tool = UiPathTools()
    
    # Mock tool context (normally provided by the agent framework)
    tool_context = {
        'session_id': 'asset_types_test_session',
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
    
    # Test cases for different asset types
    test_cases = [
        {
            'name': 'Text Asset',
            'params': {
                'asset_name': 'TestTextAsset',
                'asset_value': 'This is a test text value',
                'asset_type': 'Text',
                'description': 'Test text asset created by Agent C'
            }
        },
        {
            'name': 'Integer Asset',
            'params': {
                'asset_name': 'TestIntegerAsset',
                'asset_value': '42',
                'asset_type': 'Integer',
                'description': 'Test integer asset created by Agent C'
            }
        },
        {
            'name': 'Boolean Asset (True)',
            'params': {
                'asset_name': 'TestBooleanAssetTrue',
                'asset_value': True,
                'asset_type': 'Bool',
                'description': 'Test boolean asset (true) created by Agent C'
            }
        },
        {
            'name': 'Boolean Asset (False)',
            'params': {
                'asset_name': 'TestBooleanAssetFalse',
                'asset_value': False,
                'asset_type': 'Bool',
                'description': 'Test boolean asset (false) created by Agent C'
            }
        },
        {
            'name': 'Credential Asset',
            'params': {
                'asset_name': 'TestCredentialAsset',
                'asset_type': 'Credential',
                'username': 'rohan',
                'password': 'rohan123',
                'description': 'Test credential asset created by Agent C'
            }
        }
    ]
    
    print("🔧 Creating test assets of different types...")
    print()
    
    for test_case in test_cases:
        print(f"Creating {test_case['name']}...")
        
        try:
            result = await uipath_tool.create_asset(
                **test_case['params'],
                tool_context=tool_context
            )
            
            print(f"✅ {test_case['name']} creation result:")
            print(result)
            print("-" * 50)
            
        except Exception as e:
            print(f"❌ {test_case['name']} creation failed: {str(e)}")
            print("-" * 50)

if __name__ == "__main__":
    asyncio.run(test_all_asset_types())
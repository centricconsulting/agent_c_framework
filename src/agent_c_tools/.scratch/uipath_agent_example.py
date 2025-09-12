#!/usr/bin/env python3
"""
Example of how to create an Agent C agent that can use UiPath tools
"""

import asyncio
import sys
import os

# Add the src directory to the path
sys.path.insert(0, os.path.join(os.path.dirname(__file__), '..', 'src'))

async def create_uipath_agent():
    """Create and test an agent that can use UiPath tools"""
    
    print("🤖 Creating Agent C agent with UiPath capabilities...")
    
    # This is how you'd typically set up an agent with UiPath tools
    agent_instructions = """
You are a UiPath automation assistant. You have access to UiPath Orchestrator tools that allow you to:

1. Create assets in UiPath Orchestrator
2. Test UiPath connections
3. Get configuration information

Available UiPath Tools:
- UiPathTools-create_asset: Create new assets in Orchestrator
- UiPathTools-test_connection: Test your UiPath connection
- UiPathTools-get_config_info: Get configuration details

When users ask you to create UiPath assets, use the create_asset tool with these parameters:
- asset_name: The name for the asset
- asset_value: The value to store
- asset_type: One of "Text", "Integer", "Boolean", "Credential" (defaults to "Text")
- description: Optional description

Always test the connection first if you're unsure about the UiPath setup.
"""
    
    print("📋 Agent Instructions:")
    print(agent_instructions)
    print()
    
    print("✅ Your UiPath agent is ready!")
    print()
    print("🚀 Next steps:")
    print("1. Start the Agent C server: `agent-c-server`")
    print("2. Create an agent with the above instructions")
    print("3. Test by asking: 'Create a UiPath asset named MyAsset with value hello world'")
    
    return agent_instructions

if __name__ == "__main__":
    asyncio.run(create_uipath_agent())
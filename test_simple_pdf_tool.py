#!/usr/bin/env python3
"""
Simple test to isolate the JSON schema issue
"""

import asyncio
from agent_c.toolsets import Toolset, json_schema

class SimplePDFTool(Toolset):
    def __init__(self, **kwargs):
        super().__init__(name='simple_pdf', **kwargs)
    
    @json_schema(
        description="Simple test method",
        params={
            "test_param": {
                "type": "string",
                "description": "Test parameter",
                "required": True
            }
        }
    )
    async def test_method(self, **kwargs) -> str:
        return "test result"

async def test_simple_tool():
    print("🔍 Testing Simple PDF Tool...")
    
    tool = SimplePDFTool()
    print(f"Tool created: {tool}")
    print(f"Method: {tool.test_method}")
    print(f"Has json_schema: {hasattr(tool.test_method, 'json_schema')}")
    
    if hasattr(tool.test_method, 'json_schema'):
        schema = tool.test_method.json_schema
        print(f"Schema: {schema}")
    else:
        # Check all attributes
        attrs = [attr for attr in dir(tool.test_method) if not attr.startswith('_')]
        print(f"Available attributes: {attrs}")
        
        for attr in attrs:
            value = getattr(tool.test_method, attr)
            if isinstance(value, dict):
                print(f"Dict attribute '{attr}': {value}")

if __name__ == "__main__":
    asyncio.run(test_simple_tool())
#!/usr/bin/env python3
"""
Debug the base64 error handling
"""

import asyncio
import json
from agent_c_demo.pdf_converter.tool import PDFConverterTools

async def debug_base64_error():
    print("🔍 Debugging base64 error handling...")
    
    tool = PDFConverterTools()
    await tool.post_init()
    
    # Test invalid base64
    result = await tool.pdf_to_json(pdf_content="invalid_base64_content")
    result_data = json.loads(result)
    
    print(f"Success: {result_data.get('success')}")
    print(f"Error message: '{result_data.get('error', 'No error message')}'")
    print(f"Full result: {json.dumps(result_data, indent=2)}")
    
    # Check what the test is looking for
    error_msg = result_data.get("error", "")
    has_invalid_base64 = "Invalid base64" in error_msg
    print(f"Contains 'Invalid base64': {has_invalid_base64}")

if __name__ == "__main__":
    asyncio.run(debug_base64_error())
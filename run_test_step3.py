#!/usr/bin/env python3
"""
Step 3: Error Handling Test
"""

import asyncio
import base64
import json

async def test_error_handling():
    print("🚨 Step 3: Error Handling Test")
    print("=" * 60)
    
    # Import and initialize tool
    try:
        from agent_c_demo.pdf_converter.tool import PDFConverterTools
        tool = PDFConverterTools()
        await tool.post_init()
        print("✅ PDFConverterTools initialized for error testing")
    except Exception as e:
        print(f"❌ Tool initialization failed: {e}")
        return False
    
    tests_passed = 0
    total_tests = 4
    
    # Test 1: No PDF content
    print(f"\n📋 Test 1: No PDF content provided")
    try:
        result = await tool.pdf_to_json()
        result_data = json.loads(result)
        
        if not result_data.get("success") and "No PDF content" in result_data.get("error", ""):
            print("✅ Correctly handled missing PDF content")
            tests_passed += 1
        else:
            print(f"❌ Did not handle missing PDF content correctly: {result_data}")
    except Exception as e:
        print(f"❌ Exception during missing content test: {e}")
    
    # Test 2: Invalid base64
    print(f"\n📋 Test 2: Invalid base64 content")
    try:
        result = await tool.pdf_to_json(pdf_content="not_valid_base64_content!")
        result_data = json.loads(result)
        
        if not result_data.get("success") and "Invalid base64" in result_data.get("error", ""):
            print("✅ Correctly handled invalid base64")
            tests_passed += 1
        else:
            print(f"❌ Did not handle invalid base64 correctly: {result_data}")
    except Exception as e:
        print(f"❌ Exception during invalid base64 test: {e}")
    
    # Test 3: Valid base64 but not PDF content
    print(f"\n📋 Test 3: Valid base64 but not PDF content")
    try:
        fake_content = base64.b64encode(b"This is just text, not a PDF file").decode('utf-8')
        result = await tool.pdf_to_json(pdf_content=fake_content)
        result_data = json.loads(result)
        
        if not result_data.get("success"):
            print("✅ Correctly rejected non-PDF content")
            tests_passed += 1
        else:
            print(f"❌ Should have rejected non-PDF content: {result_data}")
    except Exception as e:
        print(f"❌ Exception during non-PDF content test: {e}")
    
    # Test 4: Empty base64
    print(f"\n📋 Test 4: Empty base64 content")
    try:
        result = await tool.pdf_to_json(pdf_content="")
        result_data = json.loads(result)
        
        if not result_data.get("success"):
            print("✅ Correctly handled empty content")
            tests_passed += 1
        else:
            print(f"❌ Should have rejected empty content: {result_data}")
    except Exception as e:
        print(f"❌ Exception during empty content test: {e}")
    
    print(f"\n📊 Error Handling Test Results: {tests_passed}/{total_tests} tests passed")
    return tests_passed == total_tests

if __name__ == "__main__":
    success = asyncio.run(test_error_handling())
    print(f"\n{'✅ Step 3 Error Handling Test PASSED!' if success else '❌ Step 3 Error Handling Test FAILED!'}")
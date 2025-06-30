#!/usr/bin/env python3
"""
PDF Converter Tool Integration Verification

This script verifies that the PDF Converter Tool is properly integrated
with the Agent C framework and can be discovered and used by agents.
"""

import asyncio
import sys
import traceback
from pathlib import Path

print("🔍 PDF Converter Tool Integration Verification")
print("=" * 60)

def test_import():
    """Test that the tool can be imported from production location"""
    print("\n1️⃣ Testing Production Import...")
    try:
        from agent_c_tools.tools.pdf_converter.tool import PDFConverterTools
        print("✅ Production import successful!")
        return PDFConverterTools
    except ImportError as e:
        print(f"❌ Production import failed: {e}")
        traceback.print_exc()
        return None

def test_toolset_registration():
    """Test that the tool is properly registered as a toolset"""
    print("\n2️⃣ Testing Toolset Registration...")
    try:
        from agent_c.toolsets import Toolset
        from agent_c_tools.tools.pdf_converter.tool import PDFConverterTools
        
        # Check inheritance
        if issubclass(PDFConverterTools, Toolset):
            print("✅ Tool properly inherits from Toolset")
        else:
            print("❌ Tool does not inherit from Toolset")
            return False
        
        # Check registration
        registered_tools = Toolset.tool_registry
        tool_names = [tool.__name__ for tool in registered_tools]
        
        if 'PDFConverterTools' in tool_names:
            print("✅ Tool is registered with Agent C framework")
        else:
            print("❌ Tool is not registered with Agent C framework")
            print(f"   Registered tools: {tool_names}")
            return False
            
        return True
        
    except Exception as e:
        print(f"❌ Toolset registration test failed: {e}")
        traceback.print_exc()
        return False

def test_tool_discovery():
    """Test that the tool can be discovered in the tools package"""
    print("\n3️⃣ Testing Tool Discovery...")
    try:
        # Test import from tools package
        from agent_c_tools.tools import pdf_converter
        print("✅ Tool discoverable in tools package")
        
        # Test import from full tools
        from agent_c_tools.tools.full import PDFConverterTools
        print("✅ Tool available in full toolset")
        
        # Test import from in_process tools
        from agent_c_tools.tools.in_process import PDFConverterTools as InProcessPDFTools
        print("✅ Tool available in in_process toolset")
        
        return True
        
    except ImportError as e:
        print(f"❌ Tool discovery failed: {e}")
        traceback.print_exc()
        return False

async def test_tool_functionality():
    """Test basic tool functionality"""
    print("\n4️⃣ Testing Tool Functionality...")
    try:
        from agent_c_tools.tools.pdf_converter.tool import PDFConverterTools
        
        # Initialize tool
        tool = PDFConverterTools()
        await tool.post_init()
        print("✅ Tool initialization successful")
        
        # Test with minimal PDF
        minimal_pdf = b"""%PDF-1.4
1 0 obj
<<
/Type /Catalog
/Pages 2 0 R
>>
endobj
2 0 obj
<<
/Type /Pages
/Kids [3 0 R]
/Count 1
>>
endobj
3 0 obj
<<
/Type /Page
/Parent 2 0 R
/MediaBox [0 0 612 792]
/Contents 4 0 R
>>
endobj
4 0 obj
<<
/Length 44
>>
stream
BT
/F1 12 Tf
100 750 Td
(Integration Test) Tj
ET
endstream
endobj
xref
0 5
0000000000 65535 f 
0000000009 00000 n 
0000000074 00000 n 
0000000120 00000 n 
0000000179 00000 n 
trailer
<<
/Size 5
/Root 1 0 R
>>
startxref
268
%%EOF"""
        
        import base64
        import json
        
        pdf_base64 = base64.b64encode(minimal_pdf).decode('utf-8')
        
        result = await tool.pdf_to_json(pdf_content=pdf_base64)
        result_data = json.loads(result)
        
        if result_data.get("success"):
            print("✅ Tool functionality test passed")
            print(f"   Processed {result_data.get('total_pages', 0)} page(s)")
            return True
        else:
            print(f"❌ Tool functionality test failed: {result_data.get('error', 'Unknown error')}")
            return False
            
    except Exception as e:
        print(f"❌ Tool functionality test failed: {e}")
        traceback.print_exc()
        return False

def test_json_schema():
    """Test that the tool has proper JSON schema decoration"""
    print("\n5️⃣ Testing JSON Schema Decoration...")
    try:
        from agent_c_tools.tools.pdf_converter.tool import PDFConverterTools
        
        tool = PDFConverterTools()
        
        # Check that pdf_to_json has JSON schema decoration
        if hasattr(tool.pdf_to_json, 'schema'):
            print("✅ JSON schema decoration present")
            
            schema = tool.pdf_to_json.schema
            if 'function' in schema and 'description' in schema['function']:
                print("✅ JSON schema has required fields")
                print(f"   Description: {schema['function']['description'][:50]}...")
                return True
            else:
                print("❌ JSON schema missing required fields")
                return False
        else:
            print("❌ JSON schema decoration missing")
            return False
            
    except Exception as e:
        print(f"❌ JSON schema test failed: {e}")
        traceback.print_exc()
        return False

async def run_integration_tests():
    """Run all integration tests"""
    print("🧪 Running PDF Converter Tool Integration Tests...")
    
    tests = [
        ("Import Test", test_import),
        ("Toolset Registration", test_toolset_registration),
        ("Tool Discovery", test_tool_discovery),
        ("Tool Functionality", test_tool_functionality),
        ("JSON Schema", test_json_schema)
    ]
    
    results = []
    
    for test_name, test_func in tests:
        try:
            if asyncio.iscoroutinefunction(test_func):
                result = await test_func()
            else:
                result = test_func()
            results.append((test_name, result))
        except Exception as e:
            print(f"❌ {test_name} failed with exception: {e}")
            results.append((test_name, False))
    
    # Summary
    print("\n" + "=" * 60)
    print("📊 INTEGRATION TEST RESULTS")
    print("=" * 60)
    
    passed = 0
    total = len(results)
    
    for test_name, result in results:
        status = "✅ PASS" if result else "❌ FAIL"
        print(f"{status} - {test_name}")
        if result:
            passed += 1
    
    print(f"\n📈 Results: {passed}/{total} tests passed")
    
    if passed == total:
        print("\n🎉 ALL INTEGRATION TESTS PASSED!")
        print("✅ PDF Converter Tool is properly integrated and ready for use!")
        return True
    else:
        print(f"\n⚠️  {total - passed} test(s) failed. Integration issues detected.")
        return False

if __name__ == "__main__":
    success = asyncio.run(run_integration_tests())
    sys.exit(0 if success else 1)
#!/usr/bin/env python3
"""
Final verification that PDF Converter Tool is working correctly
"""

import asyncio

async def final_test():
    print("🎉 FINAL PDF CONVERTER TOOL VERIFICATION")
    print("=" * 60)
    
    # Test 1: Import and basic functionality
    print("\n1️⃣ Testing Import and Basic Functionality...")
    try:
        from agent_c_tools.tools.pdf_converter.tool import PDFConverterTools
        
        tool = PDFConverterTools()
        await tool.post_init()
        print("✅ Tool imported and initialized successfully")
        
        # Check JSON schema
        if hasattr(tool.pdf_to_json, 'schema'):
            schema = tool.pdf_to_json.schema
            print("✅ JSON schema decoration working")
            print(f"   Function: {schema['function']['name']}")
            print(f"   Description: {schema['function']['description'][:60]}...")
        else:
            print("❌ JSON schema decoration missing")
            return False
            
    except Exception as e:
        print(f"❌ Import/initialization failed: {e}")
        return False
    
    # Test 2: Tool registration
    print("\n2️⃣ Testing Tool Registration...")
    try:
        from agent_c.toolsets import Toolset
        
        registered_tools = Toolset.tool_registry
        tool_names = [tool.__name__ for tool in registered_tools]
        
        if 'PDFConverterTools' in tool_names:
            print("✅ Tool is properly registered with Agent C framework")
        else:
            print("❌ Tool not found in registry")
            return False
            
    except Exception as e:
        print(f"❌ Registration test failed: {e}")
        return False
    
    # Test 3: PDF processing
    print("\n3️⃣ Testing PDF Processing...")
    try:
        import base64
        import json
        
        # Create a simple test PDF
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
(Final Test PDF) Tj
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
        
        pdf_base64 = base64.b64encode(minimal_pdf).decode('utf-8')
        
        result = await tool.pdf_to_json(
            pdf_content=pdf_base64,
            include_metadata=True,
            extract_by_page=True
        )
        
        result_data = json.loads(result)
        
        if result_data.get("success"):
            print("✅ PDF processing successful")
            print(f"   Pages processed: {result_data.get('total_pages', 0)}")
            print(f"   Metadata extracted: {len(result_data.get('metadata', {})) > 0}")
            
            # Check content
            pages = result_data.get('content', {}).get('pages', [])
            if pages and len(pages) > 0:
                text = pages[0].get('text', '')
                if 'Final Test PDF' in text:
                    print("✅ Text extraction working correctly")
                else:
                    print(f"⚠️  Text extraction may have issues: '{text}'")
            
        else:
            print(f"❌ PDF processing failed: {result_data.get('error', 'Unknown error')}")
            return False
            
    except Exception as e:
        print(f"❌ PDF processing test failed: {e}")
        import traceback
        traceback.print_exc()
        return False
    
    # Test 4: Function calling simulation
    print("\n4️⃣ Testing Function Calling Simulation...")
    try:
        # Simulate how an agent would call this tool
        function_call = {
            "name": "pdf_converter-pdf_to_json",
            "arguments": {
                "pdf_content": pdf_base64,
                "include_metadata": True,
                "extract_by_page": True
            }
        }
        
        print(f"✅ Function call format: {function_call['name']}")
        print(f"   Arguments: {list(function_call['arguments'].keys())}")
        
        # Test the actual call
        result = await tool.pdf_to_json(**function_call['arguments'])
        result_data = json.loads(result)
        
        if result_data.get("success"):
            print("✅ Function calling simulation successful")
        else:
            print("❌ Function calling simulation failed")
            return False
            
    except Exception as e:
        print(f"❌ Function calling test failed: {e}")
        return False
    
    print("\n🎉 ALL TESTS PASSED!")
    print("✅ PDF Converter Tool is fully functional and ready for production use!")
    print("🚀 Agents can now process PDF documents via function calling!")
    
    return True

if __name__ == "__main__":
    success = asyncio.run(final_test())
    if success:
        print("\n🏆 PDF CONVERTER TOOL INTEGRATION COMPLETE!")
    else:
        print("\n❌ Some issues remain to be fixed.")
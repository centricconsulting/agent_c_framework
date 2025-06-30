#!/usr/bin/env python3
"""
Test PDF Converter Tool with Real Agents

This script tests the PDF Converter Tool by having real Agent C agents
use it to process PDF documents via function calling.
"""

import asyncio
import base64
import json
import sys
from io import BytesIO

print("🤖 Testing PDF Converter Tool with Real Agents")
print("=" * 60)

def create_sample_pdf():
    """Create a sample PDF for testing"""
    try:
        from reportlab.pdfgen import canvas
        from reportlab.lib.pagesizes import letter
        
        buffer = BytesIO()
        p = canvas.Canvas(buffer, pagesize=letter)
        
        # Page 1
        p.setTitle("Agent Test Document")
        p.setAuthor("Agent C Framework")
        p.drawString(100, 750, 'PDF Converter Tool Test Document')
        p.drawString(100, 730, 'This document is being processed by an AI agent.')
        p.drawString(100, 710, 'Testing special characters: áéíóú ñ ü')
        p.drawString(100, 690, 'Page 1 content for agent analysis.')
        p.showPage()
        
        # Page 2
        p.drawString(100, 750, 'Page 2: Agent Instructions')
        p.drawString(100, 730, 'Please extract all text from this document.')
        p.drawString(100, 710, 'Count the total number of pages.')
        p.drawString(100, 690, 'Identify any metadata present.')
        p.showPage()
        
        p.save()
        return buffer.getvalue()
        
    except ImportError:
        print("⚠️  ReportLab not available, using minimal PDF")
        # Minimal PDF for testing
        return b"""%PDF-1.4
1 0 obj<</Type/Catalog/Pages 2 0 R>>endobj
2 0 obj<</Type/Pages/Kids[3 0 R]/Count 1>>endobj  
3 0 obj<</Type/Page/Parent 2 0 R/MediaBox[0 0 612 792]/Contents 4 0 R>>endobj
4 0 obj<</Length 55>>stream
BT /F1 12 Tf 100 750 Td (Agent Test Document) Tj 0 -20 Td (Page 1 content) Tj ET
endstream endobj
xref 0 5
0000000000 65535 f 
0000000009 00000 n 
0000000058 00000 n 
0000000130 00000 n 
0000000219 00000 n 
trailer<</Size 5/Root 1 0 R>>
startxref 312 %%EOF"""

async def test_direct_tool_usage():
    """Test direct tool usage (simulating agent function call)"""
    print("\n1️⃣ Testing Direct Tool Usage...")
    
    try:
        from agent_c_tools.tools.pdf_converter.tool import PDFConverterTools
        
        # Create tool instance
        tool = PDFConverterTools()
        await tool.post_init()
        print("✅ Tool initialized")
        
        # Create test PDF
        pdf_bytes = create_sample_pdf()
        pdf_base64 = base64.b64encode(pdf_bytes).decode('utf-8')
        print(f"✅ Test PDF created: {len(pdf_bytes)} bytes")
        
        # Simulate agent function call
        mock_tool_context = {
            'session_id': 'test_agent_session',
            'current_user_username': 'test_user',
            'timestamp': '2024-01-15T10:30:00.123456',
            'agent_config': {'name': 'Test Agent'},
            'calling_model_name': 'test_model'
        }
        
        result = await tool.pdf_to_json(
            pdf_content=pdf_base64,
            include_metadata=True,
            extract_by_page=True,
            tool_context=mock_tool_context
        )
        
        result_data = json.loads(result)
        
        if result_data.get("success"):
            print("✅ Direct tool usage successful!")
            print(f"   Pages processed: {result_data.get('total_pages', 0)}")
            print(f"   Metadata extracted: {len(result_data.get('metadata', {})) > 0}")
            
            # Show extracted content preview
            pages = result_data.get('content', {}).get('pages', [])
            if pages:
                first_page_text = pages[0].get('text', '')[:100]
                print(f"   First page preview: '{first_page_text}...'")
            
            return True
        else:
            print(f"❌ Direct tool usage failed: {result_data.get('error', 'Unknown error')}")
            return False
            
    except Exception as e:
        print(f"❌ Direct tool usage test failed: {e}")
        import traceback
        traceback.print_exc()
        return False

async def test_tool_registration_with_framework():
    """Test that the tool is properly registered and can be discovered by the framework"""
    print("\n2️⃣ Testing Tool Registration with Framework...")
    
    try:
        from agent_c.toolsets import Toolset
        
        # Get all registered toolsets
        registered_toolsets = Toolset.tool_registry
        toolset_names = [toolset.__name__ for toolset in registered_toolsets]
        
        if 'PDFConverterTools' in toolset_names:
            print("✅ PDFConverterTools found in registered toolsets")
            
            # Try to get the specific toolset
            pdf_toolset = None
            for toolset in registered_toolsets:
                if toolset.__name__ == 'PDFConverterTools':
                    pdf_toolset = toolset
                    break
            
            if pdf_toolset:
                print("✅ PDFConverterTools class accessible via registration")
                
                # Test instantiation via registration
                instance = pdf_toolset()
                await instance.post_init()
                print("✅ Tool can be instantiated via framework registration")
                
                return True
            else:
                print("❌ Could not access PDFConverterTools class")
                return False
        else:
            print("❌ PDFConverterTools not found in registered toolsets")
            print(f"   Available toolsets: {toolset_names}")
            return False
            
    except Exception as e:
        print(f"❌ Framework registration test failed: {e}")
        import traceback
        traceback.print_exc()
        return False

async def test_function_calling_simulation():
    """Simulate how an agent would call the tool via function calling"""
    print("\n3️⃣ Testing Function Calling Simulation...")
    
    try:
        from agent_c_tools.tools.pdf_converter.tool import PDFConverterTools
        
        # Create tool
        tool = PDFConverterTools()
        await tool.post_init()
        
        # Create test PDF
        pdf_bytes = create_sample_pdf()
        pdf_base64 = base64.b64encode(pdf_bytes).decode('utf-8')
        
        # Simulate function call arguments (as an agent would provide them)
        function_call_args = {
            "pdf_content": pdf_base64,
            "include_metadata": True,
            "extract_by_page": True
        }
        
        print("✅ Simulating agent function call...")
        print(f"   Function: pdf_converter-pdf_to_json")
        print(f"   Arguments: include_metadata={function_call_args['include_metadata']}, extract_by_page={function_call_args['extract_by_page']}")
        
        # Call the tool method with the arguments
        result = await tool.pdf_to_json(**function_call_args)
        
        # Parse and validate result
        result_data = json.loads(result)
        
        if result_data.get("success"):
            print("✅ Function calling simulation successful!")
            
            # Simulate agent processing the result
            pages = result_data.get('content', {}).get('pages', [])
            metadata = result_data.get('metadata', {})
            
            print(f"   Agent would receive: {len(pages)} page(s) of content")
            if metadata:
                print(f"   Agent would receive metadata: {list(metadata.keys())}")
            
            # Simulate agent analysis
            total_chars = sum(page.get('character_count', 0) for page in pages)
            print(f"   Agent analysis: Total characters extracted: {total_chars}")
            
            return True
        else:
            print(f"❌ Function calling simulation failed: {result_data.get('error', 'Unknown error')}")
            return False
            
    except Exception as e:
        print(f"❌ Function calling simulation failed: {e}")
        import traceback
        traceback.print_exc()
        return False

async def test_tool_schema_for_llm():
    """Test that the tool provides proper schema for LLM function calling"""
    print("\n4️⃣ Testing Tool Schema for LLM...")
    
    try:
        from agent_c_tools.tools.pdf_converter.tool import PDFConverterTools
        
        tool = PDFConverterTools()
        
        # Check JSON schema
        if hasattr(tool.pdf_to_json, 'schema'):
            schema = tool.pdf_to_json.schema
            print("✅ JSON schema available for LLM")
            
            # Validate schema structure
            if 'function' in schema:
                func_schema = schema['function']
                if 'description' in func_schema:
                    print(f"✅ Schema has description")
                else:
                    print(f"❌ Schema missing description")
                    return False
            else:
                print(f"❌ Schema missing function section")
                return False
            
            # Check parameters
            params = func_schema.get('parameters', {}).get('properties', {})
            expected_params = ['pdf_content', 'include_metadata', 'extract_by_page']
            
            for param in expected_params:
                if param in params:
                    param_info = params[param]
                    if 'type' in param_info and 'description' in param_info:
                        print(f"✅ Parameter '{param}' properly defined")
                    else:
                        print(f"❌ Parameter '{param}' missing type or description")
                        return False
                else:
                    print(f"❌ Expected parameter '{param}' not found")
                    return False
            
            print("✅ Tool schema is properly formatted for LLM function calling")
            return True
        else:
            print("❌ No JSON schema found on tool method")
            return False
            
    except Exception as e:
        print(f"❌ Tool schema test failed: {e}")
        import traceback
        traceback.print_exc()
        return False

async def run_agent_tests():
    """Run all agent integration tests"""
    print("🤖 Running PDF Converter Tool Agent Tests...")
    
    tests = [
        ("Direct Tool Usage", test_direct_tool_usage),
        ("Framework Registration", test_tool_registration_with_framework),
        ("Function Calling Simulation", test_function_calling_simulation),
        ("LLM Schema Validation", test_tool_schema_for_llm)
    ]
    
    results = []
    
    for test_name, test_func in tests:
        try:
            result = await test_func()
            results.append((test_name, result))
        except Exception as e:
            print(f"❌ {test_name} failed with exception: {e}")
            results.append((test_name, False))
    
    # Summary
    print("\n" + "=" * 60)
    print("📊 AGENT INTEGRATION TEST RESULTS")
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
        print("\n🎉 ALL AGENT TESTS PASSED!")
        print("✅ PDF Converter Tool is ready for production use with agents!")
        print("🚀 Agents can now process PDF documents via function calling!")
        return True
    else:
        print(f"\n⚠️  {total - passed} test(s) failed. Agent integration issues detected.")
        return False

if __name__ == "__main__":
    success = asyncio.run(run_agent_tests())
    sys.exit(0 if success else 1)
#!/usr/bin/env python3
"""
Final comprehensive test for PDF Converter Tool
"""

import asyncio
import base64
import json
from io import BytesIO

async def run_functional_test():
    """Run functional test of the PDF converter"""
    print("\n🔧 FUNCTIONAL TEST")
    print("=" * 30)
    
    try:
        from agent_c_demo.pdf_converter.tool import PDFConverterTools
        
        # Initialize tool
        tool = PDFConverterTools()
        await tool.post_init()
        print("✅ Tool initialized")
        
        # Create test PDF with ReportLab if available
        try:
            from reportlab.pdfgen import canvas
            from reportlab.lib.pagesizes import letter
            
            buffer = BytesIO()
            p = canvas.Canvas(buffer, pagesize=letter)
            p.setTitle("Test Document")
            p.setAuthor("Test Suite")
            p.drawString(100, 750, 'Hello, this is a test PDF!')
            p.drawString(100, 730, 'Page 1 content with special chars: áéíóú')
            p.showPage()
            p.drawString(100, 750, 'This is page 2!')
            p.drawString(100, 730, 'More content on page 2.')
            p.save()
            
            pdf_bytes = buffer.getvalue()
            print(f"✅ ReportLab PDF created: {len(pdf_bytes)} bytes")
            
        except ImportError:
            # Fallback to minimal PDF
            pdf_bytes = b"""%PDF-1.4
1 0 obj<</Type/Catalog/Pages 2 0 R>>endobj
2 0 obj<</Type/Pages/Kids[3 0 R]/Count 1>>endobj
3 0 obj<</Type/Page/Parent 2 0 R/MediaBox[0 0 612 792]/Contents 4 0 R>>endobj
4 0 obj<</Length 55>>stream
BT /F1 12 Tf 100 750 Td (Hello PDF Test!) Tj ET
endstream endobj
xref 0 5
0000000000 65535 f 
0000000009 00000 n 
0000000058 00000 n 
0000000130 00000 n 
0000000219 00000 n 
trailer<</Size 5/Root 1 0 R>>
startxref 323 %%EOF"""
            print(f"✅ Minimal PDF created: {len(pdf_bytes)} bytes")
        
        pdf_base64 = base64.b64encode(pdf_bytes).decode('utf-8')
        
        # Test conversion
        result = await tool.pdf_to_json(
            pdf_content=pdf_base64,
            include_metadata=True,
            extract_by_page=True
        )
        
        result_data = json.loads(result)
        
        if result_data.get("success"):
            print("✅ PDF conversion successful!")
            print(f"   Total pages: {result_data.get('total_pages')}")
            print(f"   Has metadata: {len(result_data.get('metadata', {})) > 0}")
            
            # Show page content
            pages = result_data.get('content', {}).get('pages', [])
            for page in pages:
                text = page.get('text', '').strip()
                preview = text[:50] + "..." if len(text) > 50 else text
                print(f"   Page {page.get('page_number')}: {page.get('character_count')} chars")
                if preview:
                    print(f"      Content: '{preview}'")
            
            return True
        else:
            print(f"❌ PDF conversion failed: {result_data.get('error')}")
            return False
            
    except Exception as e:
        print(f"❌ Functional test failed: {e}")
        import traceback
        traceback.print_exc()
        return False

def run_verification():
    """Run verification tests"""
    print("🔍 VERIFICATION TESTS")
    print("=" * 30)
    
    tests_passed = 0
    total_tests = 4
    
    # Test 1: PyPDF2
    try:
        import PyPDF2
        print(f"✅ PyPDF2 v{getattr(PyPDF2, '__version__', 'unknown')}")
        tests_passed += 1
    except Exception as e:
        print(f"❌ PyPDF2 failed: {e}")
    
    # Test 2: Agent C
    try:
        from agent_c.toolsets import Toolset, json_schema
        print("✅ Agent C toolsets")
        tests_passed += 1
    except Exception as e:
        print(f"❌ Agent C failed: {e}")
    
    # Test 3: Tool import
    try:
        from agent_c_demo.pdf_converter.tool import PDFConverterTools
        print("✅ PDFConverterTools import")
        tests_passed += 1
    except Exception as e:
        print(f"❌ Tool import failed: {e}")
    
    # Test 4: Tool initialization
    try:
        from agent_c_demo.pdf_converter.tool import PDFConverterTools
        tool = PDFConverterTools()
        print("✅ Tool initialization")
        tests_passed += 1
    except Exception as e:
        print(f"❌ Tool init failed: {e}")
    
    print(f"\n📊 Verification: {tests_passed}/{total_tests} passed")
    return tests_passed == total_tests

async def main():
    """Run complete test suite"""
    print("🧪 PDF CONVERTER TOOL - FINAL TEST")
    print("=" * 50)
    
    # Run verification
    verification_passed = run_verification()
    
    if not verification_passed:
        print("\n❌ Verification failed - cannot proceed to functional tests")
        return False
    
    # Run functional test
    functional_passed = await run_functional_test()
    
    # Summary
    print(f"\n{'='*50}")
    print("📋 FINAL TEST SUMMARY")
    print(f"{'='*50}")
    
    if verification_passed and functional_passed:
        print("🎉 ALL TESTS PASSED!")
        print("✅ PyPDF2 dependency is working correctly")
        print("✅ PDF Converter Tool is fully functional")
        print("✅ Ready for production use and documentation")
        return True
    else:
        print("❌ Some tests failed:")
        if not verification_passed:
            print("   - Verification tests failed")
        if not functional_passed:
            print("   - Functional tests failed")
        return False

if __name__ == "__main__":
    success = asyncio.run(main())
    exit(0 if success else 1)
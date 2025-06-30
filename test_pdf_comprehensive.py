#!/usr/bin/env python3
"""
Comprehensive test for PDF Converter Tool
Tests dependency availability and functionality
"""

import asyncio
import base64
import json
import sys
from io import BytesIO

def test_dependencies():
    """Test that all required dependencies are available"""
    print("🔍 Testing Dependencies...")
    
    # Test PyPDF2
    try:
        import PyPDF2
        print(f"✅ PyPDF2 available: version {PyPDF2.__version__}")
    except ImportError as e:
        print(f"❌ PyPDF2 not available: {e}")
        return False
    
    # Test ReportLab (for creating test PDFs)
    try:
        from reportlab.pdfgen import canvas
        from reportlab.lib.pagesizes import letter
        print("✅ ReportLab available for test PDF creation")
    except ImportError as e:
        print(f"❌ ReportLab not available: {e}")
        print("   Note: ReportLab is only needed for testing, not for the tool itself")
        return False
    
    # Test the tool import
    try:
        from agent_c_demo.pdf_converter.tool import PDFConverterTools
        print("✅ PDFConverterTools imported successfully")
    except ImportError as e:
        print(f"❌ PDFConverterTools import failed: {e}")
        return False
    
    return True

def create_test_pdf():
    """Create a test PDF with multiple pages and metadata"""
    print("\n📄 Creating test PDF...")
    
    from reportlab.pdfgen import canvas
    from reportlab.lib.pagesizes import letter
    
    buffer = BytesIO()
    p = canvas.Canvas(buffer, pagesize=letter)
    
    # Set metadata
    p.setTitle("Test PDF Document")
    p.setAuthor("PDF Converter Test Suite")
    p.setSubject("Testing PDF to JSON conversion")
    
    # Page 1
    p.drawString(100, 750, 'Hello, this is a test PDF!')
    p.drawString(100, 730, 'This is page 1 content.')
    p.drawString(100, 710, 'Testing special characters: áéíóú ñ ü')
    p.drawString(100, 690, 'Numbers and symbols: 123 $%& @#!')
    p.showPage()
    
    # Page 2
    p.drawString(100, 750, 'This is page 2!')
    p.drawString(100, 730, 'More content on page 2.')
    p.drawString(100, 710, 'Testing longer text that might wrap around...')
    p.drawString(100, 690, 'Line 4 of page 2')
    p.showPage()
    
    # Page 3 - Empty page
    p.drawString(100, 750, 'Page 3 - minimal content')
    p.showPage()
    
    p.save()
    
    pdf_bytes = buffer.getvalue()
    print(f"✅ Test PDF created: {len(pdf_bytes)} bytes")
    return pdf_bytes

async def test_pdf_conversion():
    """Test the PDF conversion functionality"""
    print("\n🔄 Testing PDF Conversion...")
    
    from agent_c_demo.pdf_converter.tool import PDFConverterTools
    
    # Create test PDF
    pdf_bytes = create_test_pdf()
    pdf_base64 = base64.b64encode(pdf_bytes).decode('utf-8')
    
    # Initialize tool
    tool = PDFConverterTools()
    await tool.post_init()
    
    # Test 1: Page-by-page extraction with metadata
    print("\n📋 Test 1: Page-by-page extraction with metadata")
    result = await tool.pdf_to_json(
        pdf_content=pdf_base64,
        include_metadata=True,
        extract_by_page=True
    )
    
    result_data = json.loads(result)
    
    if result_data["success"]:
        print(f"✅ Success: {result_data['success']}")
        print(f"✅ Total Pages: {result_data['total_pages']}")
        print(f"✅ Metadata extracted: {len(result_data.get('metadata', {})) > 0}")
        
        # Check pages
        pages = result_data['content']['pages']
        for i, page in enumerate(pages):
            text_preview = page['text'][:50] + "..." if len(page['text']) > 50 else page['text']
            print(f"   Page {page['page_number']}: {page['character_count']} chars - '{text_preview}'")
    else:
        print(f"❌ Test 1 failed: {result_data.get('error', 'Unknown error')}")
        return False
    
    # Test 2: Full text extraction without metadata
    print("\n📋 Test 2: Full text extraction without metadata")
    result = await tool.pdf_to_json(
        pdf_content=pdf_base64,
        include_metadata=False,
        extract_by_page=False
    )
    
    result_data = json.loads(result)
    
    if result_data["success"]:
        print(f"✅ Success: {result_data['success']}")
        print(f"✅ Full text length: {result_data['content']['character_count']} characters")
        print(f"✅ No metadata included: {len(result_data.get('metadata', {})) == 0}")
        
        # Show text preview
        full_text = result_data['content']['full_text']
        preview = full_text[:100] + "..." if len(full_text) > 100 else full_text
        print(f"   Text preview: '{preview}'")
    else:
        print(f"❌ Test 2 failed: {result_data.get('error', 'Unknown error')}")
        return False
    
    return True

async def test_error_handling():
    """Test error handling scenarios"""
    print("\n🚨 Testing Error Handling...")
    
    from agent_c_demo.pdf_converter.tool import PDFConverterTools
    
    tool = PDFConverterTools()
    await tool.post_init()
    
    # Test 1: No PDF content
    print("\n📋 Test: No PDF content")
    result = await tool.pdf_to_json()
    result_data = json.loads(result)
    
    if not result_data["success"] and "No PDF content" in result_data.get("error", ""):
        print("✅ Correctly handled missing PDF content")
    else:
        print("❌ Failed to handle missing PDF content properly")
        return False
    
    # Test 2: Invalid base64
    print("\n📋 Test: Invalid base64 content")
    result = await tool.pdf_to_json(pdf_content="invalid_base64_content!@#$%^&*()")
    result_data = json.loads(result)
    
    if not result_data["success"] and ("Invalid base64" in result_data.get("error", "") or "Could not read PDF" in result_data.get("error", "")):
        print("✅ Correctly handled invalid base64")
    else:
        print(f"❌ Failed to handle invalid base64 properly. Error: {result_data.get('error', 'No error')}")
        return False
    
    # Test 3: Invalid PDF content
    print("\n📋 Test: Invalid PDF content")
    invalid_content = base64.b64encode(b"This is not a PDF file").decode('utf-8')
    result = await tool.pdf_to_json(pdf_content=invalid_content)
    result_data = json.loads(result)
    
    if not result_data["success"]:
        print("✅ Correctly handled invalid PDF content")
    else:
        print("❌ Should have failed with invalid PDF content")
        return False
    
    return True

async def main():
    """Run all tests"""
    print("🧪 PDF Converter Tool - Comprehensive Test Suite")
    print("=" * 50)
    
    # Test dependencies
    if not test_dependencies():
        print("\n❌ Dependency tests failed. Cannot continue.")
        sys.exit(1)
    
    # Test functionality
    try:
        if not await test_pdf_conversion():
            print("\n❌ PDF conversion tests failed.")
            sys.exit(1)
        
        if not await test_error_handling():
            print("\n❌ Error handling tests failed.")
            sys.exit(1)
        
        print("\n🎉 All tests passed successfully!")
        print("✅ PyPDF2 dependency is working correctly")
        print("✅ PDF conversion functionality is working")
        print("✅ Error handling is robust")
        
    except Exception as e:
        print(f"\n💥 Test suite failed with exception: {e}")
        import traceback
        traceback.print_exc()
        sys.exit(1)

if __name__ == "__main__":
    asyncio.run(main())
#!/usr/bin/env python3
"""
Step 2: Basic Functionality Test
"""

import asyncio
import base64
import json
from io import BytesIO

async def test_basic_functionality():
    print("🔧 Step 2: Basic Functionality Test")
    print("=" * 60)
    
    # Import and initialize tool
    try:
        from agent_c_demo.pdf_converter.tool import PDFConverterTools
        tool = PDFConverterTools()
        await tool.post_init()
        print("✅ PDFConverterTools initialized successfully")
    except Exception as e:
        print(f"❌ Tool initialization failed: {e}")
        return False
    
    # Create a simple test PDF using ReportLab
    print(f"\n📄 Creating test PDF...")
    try:
        from reportlab.pdfgen import canvas
        from reportlab.lib.pagesizes import letter
        
        buffer = BytesIO()
        p = canvas.Canvas(buffer, pagesize=letter)
        p.setTitle("Test PDF")
        p.setAuthor("Test Suite")
        p.drawString(100, 750, 'Hello from PDF Converter Test!')
        p.drawString(100, 730, 'This is a test document.')
        p.showPage()
        p.drawString(100, 750, 'Page 2 content here.')
        p.save()
        
        pdf_bytes = buffer.getvalue()
        pdf_base64 = base64.b64encode(pdf_bytes).decode('utf-8')
        print(f"✅ Test PDF created: {len(pdf_bytes)} bytes")
        
    except ImportError:
        print("⚠️  ReportLab not available, creating minimal PDF...")
        # Create a minimal PDF manually
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
(Hello World) Tj
ET
endstream
endobj
xref
0 5
0000000000 65535 f 
0000000009 00000 n 
0000000058 00000 n 
0000000115 00000 n 
0000000204 00000 n 
trailer
<<
/Size 5
/Root 1 0 R
>>
startxref
297
%%EOF"""
        pdf_base64 = base64.b64encode(minimal_pdf).decode('utf-8')
        print(f"✅ Minimal test PDF created: {len(minimal_pdf)} bytes")
    
    # Test PDF conversion
    print(f"\n🔄 Testing PDF conversion...")
    try:
        result = await tool.pdf_to_json(
            pdf_content=pdf_base64,
            include_metadata=True,
            extract_by_page=True
        )
        
        result_data = json.loads(result)
        
        if result_data.get("success"):
            print(f"✅ PDF conversion successful!")
            print(f"   Total pages: {result_data.get('total_pages', 'unknown')}")
            print(f"   Metadata included: {len(result_data.get('metadata', {})) > 0}")
            
            pages = result_data.get('content', {}).get('pages', [])
            for page in pages:
                text_preview = page.get('text', '')[:50] + "..." if len(page.get('text', '')) > 50 else page.get('text', '')
                print(f"   Page {page.get('page_number', '?')}: {page.get('character_count', 0)} chars - '{text_preview}'")
            
            return True
        else:
            print(f"❌ PDF conversion failed: {result_data.get('error', 'Unknown error')}")
            return False
            
    except Exception as e:
        print(f"❌ PDF conversion exception: {e}")
        import traceback
        traceback.print_exc()
        return False

if __name__ == "__main__":
    success = asyncio.run(test_basic_functionality())
    print(f"\n{'✅ Step 2 Basic Functionality Test PASSED!' if success else '❌ Step 2 Basic Functionality Test FAILED!'}")
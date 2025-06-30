#!/usr/bin/env python3
"""
Direct test execution for PDF Converter Tool
"""

import sys
import os
import asyncio
import json
import base64
from io import BytesIO

print("🧪 DIRECT PDF CONVERTER TOOL TEST")
print("=" * 50)

# Test 1: Check PyPDF2
print("\n1️⃣ Testing PyPDF2 import...")
try:
    import PyPDF2
    print(f"✅ PyPDF2 version: {getattr(PyPDF2, '__version__', 'unknown')}")
except ImportError as e:
    print(f"❌ PyPDF2 import failed: {e}")
    sys.exit(1)

# Test 2: Check Agent C imports
print("\n2️⃣ Testing Agent C imports...")
try:
    from agent_c.toolsets import Toolset, json_schema
    print("✅ Agent C toolsets imported")
except ImportError as e:
    print(f"❌ Agent C import failed: {e}")
    sys.exit(1)

# Test 3: Import PDF Converter Tool
print("\n3️⃣ Testing PDF Converter Tool import...")
try:
    from agent_c_demo.pdf_converter.tool import PDFConverterTools
    print("✅ PDFConverterTools imported")
except ImportError as e:
    print(f"❌ PDFConverterTools import failed: {e}")
    sys.exit(1)

# Test 4: Initialize tool
print("\n4️⃣ Testing tool initialization...")
try:
    tool = PDFConverterTools()
    print("✅ PDFConverterTools instance created")
except Exception as e:
    print(f"❌ Tool initialization failed: {e}")
    sys.exit(1)

# Test 5: Create minimal test PDF
print("\n5️⃣ Creating test PDF...")
try:
    # Create a minimal valid PDF
    minimal_pdf = b"""%PDF-1.4
1 0 obj<</Type/Catalog/Pages 2 0 R>>endobj
2 0 obj<</Type/Pages/Kids[3 0 R]/Count 1>>endobj  
3 0 obj<</Type/Page/Parent 2 0 R/MediaBox[0 0 612 792]/Contents 4 0 R>>endobj
4 0 obj<</Length 44>>stream
BT /F1 12 Tf 100 750 Td (Test PDF) Tj ET
endstream endobj
xref 0 5
0000000000 65535 f 
0000000009 00000 n 
0000000058 00000 n 
0000000130 00000 n 
0000000219 00000 n 
trailer<</Size 5/Root 1 0 R>>
startxref 312 %%EOF"""
    
    pdf_base64 = base64.b64encode(minimal_pdf).decode('utf-8')
    print(f"✅ Test PDF created: {len(minimal_pdf)} bytes")
except Exception as e:
    print(f"❌ Test PDF creation failed: {e}")
    sys.exit(1)

# Test 6: Async functionality test
print("\n6️⃣ Testing PDF conversion...")

async def test_conversion():
    try:
        await tool.post_init()
        print("✅ Tool post_init completed")
        
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
            
            pages = result_data.get('content', {}).get('pages', [])
            if pages:
                for page in pages[:2]:  # Show first 2 pages
                    text = page.get('text', '')
                    preview = text[:30] + "..." if len(text) > 30 else text
                    print(f"   Page {page.get('page_number')}: '{preview}'")
            
            return True
        else:
            print(f"❌ PDF conversion failed: {result_data.get('error')}")
            return False
            
    except Exception as e:
        print(f"❌ Conversion test failed: {e}")
        import traceback
        traceback.print_exc()
        return False

# Run async test
try:
    success = asyncio.run(test_conversion())
    if success:
        print("\n🎉 ALL TESTS PASSED! PDF Converter Tool is working correctly.")
    else:
        print("\n❌ Some tests failed.")
        sys.exit(1)
except Exception as e:
    print(f"\n💥 Test execution failed: {e}")
    sys.exit(1)

print("\n✅ PDF Converter Tool dependency and functionality verification complete!")
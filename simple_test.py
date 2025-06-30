#!/usr/bin/env python3
"""
Simple test to check if PyPDF2 is working
"""

print("🔍 Testing PyPDF2 availability...")

try:
    import PyPDF2
    print(f"✅ PyPDF2 imported successfully!")
    print(f"   Version: {getattr(PyPDF2, '__version__', 'unknown')}")
    
    # Test basic functionality
    from io import BytesIO
    
    # Try to create a PdfReader (this will test if the library is functional)
    test_data = b"%PDF-1.4\n1 0 obj\n<<\n/Type /Catalog\n/Pages 2 0 R\n>>\nendobj\n2 0 obj\n<<\n/Type /Pages\n/Kids [3 0 R]\n/Count 1\n>>\nendobj\n3 0 obj\n<<\n/Type /Page\n/Parent 2 0 R\n/MediaBox [0 0 612 792]\n>>\nendobj\nxref\n0 4\n0000000000 65535 f \n0000000009 00000 n \n0000000074 00000 n \n0000000120 00000 n \ntrailer\n<<\n/Size 4\n/Root 1 0 R\n>>\nstartxref\n179\n%%EOF"
    
    pdf_stream = BytesIO(test_data)
    reader = PyPDF2.PdfReader(pdf_stream)
    print(f"✅ PyPDF2 basic functionality test passed!")
    print(f"   Test PDF pages: {len(reader.pages)}")
    
except ImportError as e:
    print(f"❌ PyPDF2 not available: {e}")
    print("   You may need to install it with: pip install PyPDF2")
    exit(1)
except Exception as e:
    print(f"⚠️  PyPDF2 imported but basic test failed: {e}")
    print("   This might indicate a version compatibility issue")
    exit(1)

print("\n🔍 Testing PDF Converter Tool import...")

try:
    import sys
    import os
    
    # Add the project path to sys.path if needed
    project_path = os.path.dirname(os.path.abspath(__file__))
    if project_path not in sys.path:
        sys.path.insert(0, project_path)
    
    from agent_c_demo.pdf_converter.tool import PDFConverterTools
    print("✅ PDFConverterTools imported successfully!")
    
    # Test tool initialization
    tool = PDFConverterTools()
    print("✅ PDFConverterTools initialized successfully!")
    
except ImportError as e:
    print(f"❌ Failed to import PDFConverterTools: {e}")
    print("   Check that the tool is in the correct location")
    exit(1)
except Exception as e:
    print(f"❌ Failed to initialize PDFConverterTools: {e}")
    exit(1)

print("\n🎉 All basic tests passed! PyPDF2 dependency is working correctly.")
print("✅ Ready to run full functionality tests.")
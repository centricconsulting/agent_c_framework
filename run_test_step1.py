#!/usr/bin/env python3
"""
Step 1: Environment Check
"""

import subprocess
import sys
import os

print("🔍 Step 1: Environment Check for PDF Converter Tool")
print("=" * 60)

print(f"📍 Current Directory: {os.getcwd()}")
print(f"📍 Python Executable: {sys.executable}")
print(f"📍 Python Version: {sys.version}")

# Check if PyPDF2 is available
print(f"\n📦 Testing PyPDF2 import...")
try:
    import PyPDF2
    print(f"✅ PyPDF2 imported successfully!")
    print(f"   Version: {getattr(PyPDF2, '__version__', 'unknown')}")
    
    # Test basic PyPDF2 functionality
    from io import BytesIO
    print(f"✅ BytesIO imported for PDF stream handling")
    
    # Test if we can create a basic PDF reader
    test_pdf_data = b"%PDF-1.4\n1 0 obj\n<</Type/Catalog/Pages 2 0 R>>\nendobj\n2 0 obj\n<</Type/Pages/Kids[3 0 R]/Count 1>>\nendobj\n3 0 obj\n<</Type/Page/Parent 2 0 R/MediaBox[0 0 612 792]>>\nendobj\nxref\n0 4\n0000000000 65535 f\n0000000009 00000 n\n0000000058 00000 n\n0000000115 00000 n\ntrailer\n<</Size 4/Root 1 0 R>>\nstartxref\n173\n%%EOF"
    
    pdf_stream = BytesIO(test_pdf_data)
    reader = PyPDF2.PdfReader(pdf_stream)
    print(f"✅ PyPDF2 PdfReader created successfully")
    print(f"   Test PDF has {len(reader.pages)} page(s)")
    
except ImportError as e:
    print(f"❌ PyPDF2 not available: {e}")
    print("   Attempting to install...")
    try:
        result = subprocess.run([sys.executable, "-m", "pip", "install", "PyPDF2==3.0.1"], 
                              capture_output=True, text=True)
        if result.returncode == 0:
            print("✅ PyPDF2 installation successful!")
            import PyPDF2
            print(f"✅ PyPDF2 now available: version {getattr(PyPDF2, '__version__', 'unknown')}")
        else:
            print(f"❌ Installation failed: {result.stderr}")
    except Exception as install_error:
        print(f"❌ Installation exception: {install_error}")
        
except Exception as e:
    print(f"⚠️  PyPDF2 available but test failed: {e}")

# Check Agent C imports
print(f"\n🤖 Testing Agent C imports...")
try:
    from agent_c.toolsets import Toolset, json_schema
    print(f"✅ Agent C toolsets imported successfully")
except ImportError as e:
    print(f"❌ Agent C imports failed: {e}")
except Exception as e:
    print(f"⚠️  Agent C import issue: {e}")

# Check PDF Converter Tool import
print(f"\n🔧 Testing PDF Converter Tool import...")
try:
    from agent_c_demo.pdf_converter.tool import PDFConverterTools
    print(f"✅ PDFConverterTools imported successfully")
    
    # Test initialization
    tool = PDFConverterTools()
    print(f"✅ PDFConverterTools instance created")
    
except ImportError as e:
    print(f"❌ PDFConverterTools import failed: {e}")
except Exception as e:
    print(f"⚠️  PDFConverterTools initialization issue: {e}")

print(f"\n✅ Step 1 Environment Check Complete!")
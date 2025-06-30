#!/usr/bin/env python3

import sys
import os

print("Python version:", sys.version)
print("Current working directory:", os.getcwd())
print("Python path:")
for p in sys.path:
    print(f"  {p}")

print("\n" + "="*50)
print("Testing imports...")

# Test 1: PyPDF2
print("\n1. Testing PyPDF2...")
try:
    import PyPDF2
    print(f"✅ PyPDF2 version: {getattr(PyPDF2, '__version__', 'unknown')}")
except Exception as e:
    print(f"❌ PyPDF2 failed: {e}")

# Test 2: Basic imports
print("\n2. Testing basic imports...")
try:
    import json, base64, logging
    from io import BytesIO
    from typing import Dict, Any
    from datetime import datetime
    print("✅ Basic imports successful")
except Exception as e:
    print(f"❌ Basic imports failed: {e}")

# Test 3: Agent C imports
print("\n3. Testing Agent C imports...")
try:
    from agent_c.toolsets import Toolset, json_schema
    print("✅ Agent C toolsets imported")
except Exception as e:
    print(f"❌ Agent C imports failed: {e}")

# Test 4: PDF Converter Tool
print("\n4. Testing PDF Converter Tool...")
try:
    from agent_c_demo.pdf_converter.tool import PDFConverterTools
    print("✅ PDFConverterTools imported")
    
    # Try to create instance
    tool = PDFConverterTools()
    print("✅ PDFConverterTools instance created")
    
except Exception as e:
    print(f"❌ PDF Converter Tool failed: {e}")
    import traceback
    traceback.print_exc()

print("\n" + "="*50)
print("Import test complete!")
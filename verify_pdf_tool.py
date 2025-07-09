#!/usr/bin/env python3
"""
Simple verification script for PDF Converter Tool
"""

def test_step(step_name, test_func):
    """Helper to run a test step and report results"""
    print(f"\n🔍 {step_name}")
    print("-" * 40)
    try:
        result = test_func()
        if result:
            print(f"✅ {step_name} - PASSED")
            return True
        else:
            print(f"❌ {step_name} - FAILED")
            return False
    except Exception as e:
        print(f"💥 {step_name} - EXCEPTION: {e}")
        return False

def test_pypdf2_import():
    """Test PyPDF2 import and basic functionality"""
    try:
        import PyPDF2
        print(f"PyPDF2 version: {getattr(PyPDF2, '__version__', 'unknown')}")
        
        # Test basic functionality
        from io import BytesIO
        test_data = b"%PDF-1.4\n1 0 obj\n<<\n/Type /Catalog\n>>\nendobj\nxref\n0 1\n0000000000 65535 f \ntrailer\n<<\n/Size 1\n>>\nstartxref\n9\n%%EOF"
        pdf_stream = BytesIO(test_data)
        reader = PyPDF2.PdfReader(pdf_stream)
        print(f"Basic PyPDF2 functionality test passed")
        return True
    except Exception as e:
        print(f"PyPDF2 test failed: {e}")
        return False

def test_agent_c_imports():
    """Test Agent C framework imports"""
    try:
        from agent_c.toolsets import Toolset, json_schema
        print("Agent C toolsets imported successfully")
        return True
    except Exception as e:
        print(f"Agent C import failed: {e}")
        return False

def test_tool_import():
    """Test PDF Converter Tool import"""
    try:
        from agent_c_demo.pdf_converter.tool import PDFConverterTools
        print("PDFConverterTools imported successfully")
        return True
    except Exception as e:
        print(f"Tool import failed: {e}")
        return False

def test_tool_initialization():
    """Test tool initialization"""
    try:
        from agent_c_demo.pdf_converter.tool import PDFConverterTools
        tool = PDFConverterTools()
        print("PDFConverterTools instance created successfully")
        return True
    except Exception as e:
        print(f"Tool initialization failed: {e}")
        return False

def main():
    """Run all verification tests"""
    print("🧪 PDF Converter Tool Verification")
    print("=" * 50)
    
    tests = [
        ("PyPDF2 Import & Basic Functionality", test_pypdf2_import),
        ("Agent C Framework Imports", test_agent_c_imports),
        ("PDF Converter Tool Import", test_tool_import),
        ("Tool Initialization", test_tool_initialization),
    ]
    
    passed = 0
    total = len(tests)
    
    for test_name, test_func in tests:
        if test_step(test_name, test_func):
            passed += 1
    
    print(f"\n{'='*50}")
    print(f"📊 VERIFICATION RESULTS: {passed}/{total} tests passed")
    
    if passed == total:
        print("🎉 All verification tests passed!")
        print("✅ PDF Converter Tool is ready for functional testing")
        return True
    else:
        print("❌ Some verification tests failed")
        print("   Check the output above for details")
        return False

if __name__ == "__main__":
    success = main()
    exit(0 if success else 1)
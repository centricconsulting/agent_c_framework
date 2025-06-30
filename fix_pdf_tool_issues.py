#!/usr/bin/env python3
"""
Quick fix script for PDF Converter Tool issues
"""

import subprocess
import sys

def install_missing_deps():
    """Install missing test dependencies"""
    print("📦 Installing missing dependencies...")
    
    deps = ["pytest-asyncio>=0.21.0"]
    
    for dep in deps:
        print(f"Installing {dep}...")
        result = subprocess.run([sys.executable, "-m", "pip", "install", dep], 
                              capture_output=True, text=True)
        if result.returncode == 0:
            print(f"✅ {dep} installed successfully")
        else:
            print(f"❌ Failed to install {dep}: {result.stderr}")

def verify_tool_registration():
    """Verify that the tool is properly registered"""
    print("\n🔍 Verifying tool registration...")
    
    try:
        from agent_c.toolsets import Toolset
        from agent_c_tools.tools.pdf_converter.tool import PDFConverterTools
        
        # Check if tool is in registry
        registered_tools = Toolset.tool_registry
        tool_names = [tool.__name__ for tool in registered_tools]
        
        if 'PDFConverterTools' in tool_names:
            print("✅ PDFConverterTools is registered")
        else:
            print("❌ PDFConverterTools not found in registry")
            print(f"   Available tools: {tool_names}")
        
        # Check JSON schema
        tool = PDFConverterTools()
        if hasattr(tool.pdf_to_json, 'schema'):
            print("✅ JSON schema decoration present")
            schema = tool.pdf_to_json.schema
            print(f"   Schema: {schema['function']['name']} - {schema['function']['description'][:50]}...")
        else:
            print("❌ JSON schema decoration missing")
            # Debug: show what attributes are available
            attrs = [attr for attr in dir(tool.pdf_to_json) if not attr.startswith('_')]
            print(f"   Available attributes: {attrs}")
            
    except Exception as e:
        print(f"❌ Verification failed: {e}")

def main():
    print("🔧 PDF Converter Tool Issue Fix Script")
    print("=" * 50)
    
    install_missing_deps()
    verify_tool_registration()
    
    print("\n✅ Fix script complete!")
    print("Now try running the tests again:")
    print("  python verify_pdf_tool_integration.py")
    print("  python test_pdf_tool_with_agents.py")

if __name__ == "__main__":
    main()
#!/usr/bin/env python3
"""
Install test dependencies for PDF Converter Tool
"""

import subprocess
import sys

def install_test_deps():
    """Install test dependencies"""
    print("📦 Installing test dependencies...")
    
    deps = [
        "pytest>=7.0.0",
        "pytest-asyncio>=0.21.0", 
        "reportlab>=3.6.0"
    ]
    
    for dep in deps:
        print(f"Installing {dep}...")
        result = subprocess.run([sys.executable, "-m", "pip", "install", dep], 
                              capture_output=True, text=True)
        if result.returncode == 0:
            print(f"✅ {dep} installed")
        else:
            print(f"❌ Failed to install {dep}: {result.stderr}")

if __name__ == "__main__":
    install_test_deps()
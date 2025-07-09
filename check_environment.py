#!/usr/bin/env python3

import subprocess
import sys
import os

def run_cmd(cmd):
    """Run a command and return output"""
    try:
        result = subprocess.run(cmd, shell=True, capture_output=True, text=True)
        return result.stdout.strip(), result.stderr.strip(), result.returncode
    except Exception as e:
        return "", str(e), 1

print("🔍 Environment Check for PDF Converter Tool")
print("=" * 50)

print(f"\n📍 Current Directory: {os.getcwd()}")
print(f"📍 Python Executable: {sys.executable}")
print(f"📍 Python Version: {sys.version}")

print(f"\n📦 Checking installed packages...")

# Check if PyPDF2 is installed
stdout, stderr, code = run_cmd("pip list | grep -i pypdf")
if code == 0 and stdout:
    print(f"✅ PyPDF2 packages found:")
    print(f"   {stdout}")
else:
    print("❌ PyPDF2 not found in pip list")
    
    # Try alternative check
    stdout, stderr, code = run_cmd("python -c \"import PyPDF2; print(f'PyPDF2 {PyPDF2.__version__}')\"")
    if code == 0:
        print(f"✅ But PyPDF2 is importable: {stdout}")
    else:
        print(f"❌ PyPDF2 not importable: {stderr}")

# Check pip freeze
print(f"\n📦 All installed packages (pip freeze):")
stdout, stderr, code = run_cmd("pip freeze")
if code == 0:
    packages = stdout.split('\n')
    pdf_packages = [p for p in packages if 'pdf' in p.lower()]
    if pdf_packages:
        print("PDF-related packages:")
        for pkg in pdf_packages:
            print(f"   {pkg}")
    else:
        print("   No PDF-related packages found")
        
    # Show first 10 packages for context
    print("\nFirst 10 packages:")
    for pkg in packages[:10]:
        print(f"   {pkg}")
    print(f"   ... and {len(packages)-10} more")
else:
    print(f"❌ pip freeze failed: {stderr}")

# Check if we're in a virtual environment
print(f"\n🐍 Virtual Environment Check:")
if hasattr(sys, 'real_prefix') or (hasattr(sys, 'base_prefix') and sys.base_prefix != sys.prefix):
    print("✅ Running in virtual environment")
    print(f"   Prefix: {sys.prefix}")
    if hasattr(sys, 'base_prefix'):
        print(f"   Base prefix: {sys.base_prefix}")
else:
    print("❌ Not running in virtual environment")

# Try to install PyPDF2 if missing
print(f"\n🔧 Attempting to install PyPDF2...")
stdout, stderr, code = run_cmd("pip install PyPDF2==3.0.1")
if code == 0:
    print("✅ PyPDF2 installation successful")
else:
    print(f"❌ PyPDF2 installation failed:")
    print(f"   stdout: {stdout}")
    print(f"   stderr: {stderr}")

print("\n" + "=" * 50)
print("Environment check complete!")
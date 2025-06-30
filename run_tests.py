#!/usr/bin/env python3
"""
Test runner for PDF Converter Tool
"""

import subprocess
import sys
import os

def run_command(cmd, description):
    """Run a command and return success status"""
    print(f"\n🚀 {description}")
    print(f"Command: {cmd}")
    print("-" * 40)
    
    try:
        result = subprocess.run(cmd, shell=True, capture_output=True, text=True, cwd=os.getcwd())
        
        if result.stdout:
            print("STDOUT:")
            print(result.stdout)
        
        if result.stderr:
            print("STDERR:")
            print(result.stderr)
        
        if result.returncode == 0:
            print(f"✅ {description} - SUCCESS")
            return True
        else:
            print(f"❌ {description} - FAILED (exit code: {result.returncode})")
            return False
            
    except Exception as e:
        print(f"💥 {description} - EXCEPTION: {e}")
        return False

def main():
    """Run all tests"""
    print("🧪 PDF Converter Tool Test Runner")
    print("=" * 50)
    
    # Change to project directory
    os.chdir("//project".replace("//", ""))
    
    tests = [
        ("python check_pdf_deps.py", "Dependency Check"),
        ("python test_pdf_comprehensive.py", "Comprehensive Functionality Test"),
        ("python test_pdf_tool.py", "Original Test")
    ]
    
    results = []
    for cmd, desc in tests:
        success = run_command(cmd, desc)
        results.append((desc, success))
    
    # Summary
    print("\n" + "=" * 50)
    print("📊 TEST SUMMARY")
    print("=" * 50)
    
    all_passed = True
    for desc, success in results:
        status = "✅ PASS" if success else "❌ FAIL"
        print(f"{status} - {desc}")
        if not success:
            all_passed = False
    
    if all_passed:
        print("\n🎉 All tests passed! PDF Converter Tool is ready to use.")
    else:
        print("\n⚠️  Some tests failed. Check the output above for details.")
    
    return all_passed

if __name__ == "__main__":
    success = main()
    sys.exit(0 if success else 1)
#!/usr/bin/env python3
"""
Test runner for Excel Tool - runs both component and integration tests.
"""
import subprocess
import sys
import os
from pathlib import Path

def run_tests(test_type="all"):
    """Run tests based on type specified."""
    tests_dir = Path(__file__).parent
    os.chdir(tests_dir)
    
    print(f"🧪 Running Excel Tool Tests ({test_type})")
    print("=" * 60)
    
    try:
        if test_type == "component":
            print("🏗️ Running Component Tests (Business Logic)")
            pytest_args = [sys.executable, "-m", "pytest", "-v", "-m", "component"]
        elif test_type == "integration":
            print("🔗 Running Integration Tests (Tool Interface)")
            pytest_args = [sys.executable, "-m", "pytest", "-v", "-m", "integration"]  
        else:  # all
            print("🚀 Running All Tests (Component + Integration)")
            pytest_args = [sys.executable, "-m", "pytest", "-v"]
        
        print(f"Command: {' '.join(pytest_args)}")
        print(f"Working Directory: {tests_dir}")
        print("-" * 60)
        
        result = subprocess.run(pytest_args, capture_output=True, text=True)
        
        print("STDOUT:")
        print(result.stdout)
        if result.stderr:
            print("\\nSTDERR:") 
            print(result.stderr)
        
        print(f"\\n{'=' * 60}")
        if result.returncode == 0:
            print("✅ ALL TESTS PASSED!")
        else:
            print(f"❌ SOME TESTS FAILED (return code: {result.returncode})")
        
        return result.returncode == 0
    except Exception as e:
        print(f"❌ Error running tests: {e}")
        return False

def print_usage():
    """Print usage information."""
    print("Excel Tool Test Runner")
    print("=" * 30)
    print("Usage:")
    print("  python run_tests.py [test_type]")
    print("")
    print("Test Types:")
    print("  component   - Run component tests only (fast)")
    print("  integration - Run integration tests only (slower)")
    print("  all         - Run all tests (default)")
    print("")
    print("Examples:")
    print("  python run_tests.py")
    print("  python run_tests.py component")
    print("  python run_tests.py integration")

def check_requirements():
    """Check if test requirements are installed."""
    print("🔍 Checking test requirements...")
    
    required_packages = ['pytest', 'pytest-asyncio', 'openpyxl']
    missing = []
    
    for package in required_packages:
        try:
            __import__(package.replace('-', '_'))
            print(f"  ✅ {package}")
        except ImportError:
            print(f"  ❌ {package}")
            missing.append(package)
    
    if missing:
        print(f"\\n⚠️ Missing packages: {', '.join(missing)}")
        print("Install with: pip install -r test-requirements.txt")
        return False
    
    print("✅ All requirements satisfied")
    return True

def main():
    """Main test runner function."""
    if len(sys.argv) > 1 and sys.argv[1] in ['--help', '-h', 'help']:
        print_usage()
        return
    
    # Check requirements first
    if not check_requirements():
        print("\\n❌ Please install missing requirements before running tests")
        sys.exit(1)
    
    test_type = sys.argv[1] if len(sys.argv) > 1 else "all"
    
    if test_type not in ["all", "component", "integration"]:
        print(f"❌ Invalid test type: {test_type}")
        print_usage()
        sys.exit(1)
    
    success = run_tests(test_type)
    
    if success:
        print("\\n🎉 Excel Tool testing completed successfully!")
        print("✅ Tool is ready for production use")
    else:
        print("\\n🔧 Some tests failed - please review the output above")
        print("❌ Tool may need attention before production use")
    
    sys.exit(0 if success else 1)

if __name__ == "__main__":
    main()
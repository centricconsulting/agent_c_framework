#!/usr/bin/env python3
"""
Direct test execution for Excel Tool tests.
"""
import sys
import os
from pathlib import Path

# Add the project root to Python path
project_root = Path(__file__).parent.parent.parent.parent.parent
sys.path.insert(0, str(project_root))

# Change to tests directory
tests_dir = Path(__file__).parent
os.chdir(tests_dir)

print("🧪 Executing Excel Tool Tests - Direct Execution")
print("=" * 60)
print(f"Working Directory: {os.getcwd()}")
print(f"Python Path: {sys.path[0]}")
print("-" * 60)

# Import and run the test runner
try:
    from run_tests import run_tests, check_requirements
    
    # Check requirements first
    if not check_requirements():
        print("\n❌ Missing requirements - attempting to continue anyway")
    
    print("\n🚀 Running all tests (component + integration)")
    success = run_tests("all")
    
    if success:
        print("\n🎉 All tests passed!")
    else:
        print("\n❌ Some tests failed")
        
except Exception as e:
    print(f"❌ Error executing tests: {e}")
    import traceback
    traceback.print_exc()
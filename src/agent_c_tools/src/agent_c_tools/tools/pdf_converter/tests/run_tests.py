#!/usr/bin/env python3
"""
Test runner for PDF Converter Tool

This script runs the complete test suite for the PDF Converter Tool.
It can be run standalone or as part of the larger agent_c_tools test suite.

Usage:
    python run_tests.py              # Run all tests
    python run_tests.py --unit       # Run only unit tests
    python run_tests.py --integration # Run only integration tests
    python run_tests.py --fast       # Skip slow tests
"""

import sys
import subprocess
import argparse
from pathlib import Path

def run_tests(test_type=None, fast=False):
    """Run the test suite with optional filtering"""
    
    # Get the directory containing this script
    test_dir = Path(__file__).parent
    
    # Base pytest command
    cmd = ["python", "-m", "pytest", str(test_dir)]
    
    # Add markers based on test type
    if test_type == "unit":
        cmd.extend(["-m", "unit"])
    elif test_type == "integration":
        cmd.extend(["-m", "integration"])
    elif test_type == "error_handling":
        cmd.extend(["-m", "error_handling"])
    
    # Skip slow tests if requested
    if fast:
        cmd.extend(["-m", "not slow"])
    
    # Add verbose output
    cmd.extend(["-v", "--tb=short"])
    
    print(f"🧪 Running PDF Converter Tool tests...")
    print(f"Command: {' '.join(cmd)}")
    print("-" * 50)
    
    # Run the tests
    result = subprocess.run(cmd, cwd=test_dir.parent.parent.parent.parent.parent)
    
    if result.returncode == 0:
        print("\n✅ All tests passed!")
    else:
        print(f"\n❌ Tests failed with exit code: {result.returncode}")
    
    return result.returncode

def main():
    parser = argparse.ArgumentParser(description="Run PDF Converter Tool tests")
    parser.add_argument("--unit", action="store_true", help="Run only unit tests")
    parser.add_argument("--integration", action="store_true", help="Run only integration tests")
    parser.add_argument("--error-handling", action="store_true", help="Run only error handling tests")
    parser.add_argument("--fast", action="store_true", help="Skip slow tests")
    
    args = parser.parse_args()
    
    # Determine test type
    test_type = None
    if args.unit:
        test_type = "unit"
    elif args.integration:
        test_type = "integration"
    elif args.error_handling:
        test_type = "error_handling"
    
    # Run tests
    exit_code = run_tests(test_type=test_type, fast=args.fast)
    sys.exit(exit_code)

if __name__ == "__main__":
    main()
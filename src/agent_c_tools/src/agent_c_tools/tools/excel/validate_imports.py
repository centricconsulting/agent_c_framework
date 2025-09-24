#!/usr/bin/env python3
"""
Import validation script for Excel Tool.
Tests all the imports that were failing in the original test.
"""
import sys
from pathlib import Path

def main():
    print("🔍 Excel Tool Import Validation")
    print("=" * 40)
    
    # Set up path correctly
    current_dir = Path(__file__).parent
    src_dir = current_dir.parent.parent.parent  # Get to src level
    sys.path.insert(0, str(src_dir))
    
    print(f"📁 Source directory: {src_dir}")
    print(f"📁 Current directory: {current_dir}")
    print()
    
    # Test 1: Main Excel tool import
    try:
        print("Testing: from agent_c_tools.tools.excel import ExcelTools")
        from agent_c_tools.tools.excel import ExcelTools
        print("  ✅ SUCCESS: ExcelTools import works!")
    except Exception as e:
        print(f"  ❌ FAILED: {e}")
        return False
    
    # Test 2: Business logic imports (the ones that were failing)
    imports_to_test = [
        ("agent_c_tools.tools.excel.business_logic.workbook_manager", "WorkbookManager"),
        ("agent_c_tools.tools.excel.business_logic.concurrency_manager", "ConcurrencyManager"),
        ("agent_c_tools.tools.excel.business_logic.excel_operations", "ExcelOperations"),
        ("agent_c_tools.tools.excel.models", "WorkbookMetadata"),
        ("agent_c_tools.tools.excel.models", "OperationResult"),
    ]
    
    all_passed = True
    for module_path, class_name in imports_to_test:
        try:
            print(f"Testing: from {module_path} import {class_name}")
            module = __import__(module_path, fromlist=[class_name])
            getattr(module, class_name)
            print(f"  ✅ SUCCESS: {class_name} import works!")
        except Exception as e:
            print(f"  ❌ FAILED: {e}")
            all_passed = False
    
    print()
    if all_passed:
        print("🎉 ALL IMPORTS SUCCESSFUL!")
        print("✅ Your __init__.py fix resolved the import issues!")
        print("🚀 The Excel tool should now work properly!")
    else:
        print("❌ Some imports still failing - additional fixes needed")
    
    return all_passed

if __name__ == "__main__":
    success = main()
    sys.exit(0 if success else 1)
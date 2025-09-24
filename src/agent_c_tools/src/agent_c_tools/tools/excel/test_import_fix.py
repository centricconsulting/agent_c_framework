#!/usr/bin/env python3
"""
Simple test to verify our __init__.py fix worked.
"""
import sys
from pathlib import Path

# Add the src directory to Python path
current_dir = Path(__file__).parent
src_dir = current_dir.parent.parent.parent
sys.path.insert(0, str(src_dir))

print("Testing Excel tool imports after __init__.py fix...")
print(f"Python path includes: {src_dir}")

try:
    # Test the main import that should now work
    from agent_c_tools.tools.excel import ExcelTools
    print("✅ SUCCESS: Main ExcelTools import works!")
    
    # Test the business logic imports that were failing
    from agent_c_tools.tools.excel.business_logic.workbook_manager import WorkbookManager
    print("✅ SUCCESS: WorkbookManager import works!")
    
    from agent_c_tools.tools.excel.business_logic.concurrency_manager import ConcurrencyManager
    print("✅ SUCCESS: ConcurrencyManager import works!")
    
    from agent_c_tools.tools.excel.business_logic.excel_operations import ExcelOperations
    print("✅ SUCCESS: ExcelOperations import works!")
    
    from agent_c_tools.tools.excel.models import WorkbookMetadata, OperationResult
    print("✅ SUCCESS: Models import works!")
    
    print("\n🎉 ALL IMPORTS SUCCESSFUL!")
    print("The __init__.py fix resolved the import issues!")
    print("Excel tool should now work properly!")
    
except ImportError as e:
    print(f"❌ IMPORT FAILED: {e}")
    sys.exit(1)
except Exception as e:
    print(f"❌ UNEXPECTED ERROR: {e}")
    sys.exit(1)

print("\nNow testing basic initialization...")
try:
    # Test basic initialization
    workbook_manager = WorkbookManager()
    concurrency_manager = ConcurrencyManager()
    excel_operations = ExcelOperations()
    
    print("✅ SUCCESS: All business logic classes can be instantiated!")
    print("🚀 Excel tool is ready for testing!")
    
except Exception as e:
    print(f"❌ INITIALIZATION FAILED: {e}")
    sys.exit(1)
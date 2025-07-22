#!/usr/bin/env python3
"""
Test script to verify WebSearchTools registration in Agent C system.
Run this from the project root directory.
"""

import sys
import os

# Add the src directory to the path
sys.path.insert(0, os.path.join(os.path.dirname(__file__), 'src'))

def test_websearch_tools_import():
    """Test that WebSearchTools can be imported successfully."""
    try:
        from agent_c_tools.tools.web_search import WebSearchTools
        print("✅ WebSearchTools imported successfully from web_search module")
        return True
    except ImportError as e:
        print(f"❌ Failed to import WebSearchTools from web_search module: {e}")
        return False

def test_websearch_tools_main_import():
    """Test that WebSearchTools can be imported from main agent_c_tools module."""
    try:
        from agent_c_tools import WebSearchTools
        print("✅ WebSearchTools imported successfully from main agent_c_tools module")
        return True
    except ImportError as e:
        print(f"❌ Failed to import WebSearchTools from main agent_c_tools module: {e}")
        return False

def test_websearch_tools_full_import():
    """Test that WebSearchTools is available in the full tools import."""
    try:
        from agent_c_tools.tools.full import WebSearchTools
        print("✅ WebSearchTools available in full tools import")
        return True
    except ImportError as e:
        print(f"❌ WebSearchTools not available in full tools import: {e}")
        return False

def test_websearch_tools_instantiation():
    """Test that WebSearchTools can be instantiated."""
    try:
        from agent_c_tools.tools.web_search import WebSearchTools
        
        # Try to instantiate the tool
        tool = WebSearchTools()
        print("✅ WebSearchTools instantiated successfully")
        print(f"   Tool name: {tool.name}")
        
        # Check if it has the expected methods
        expected_methods = ['web_search', 'news_search', 'educational_search', 'research_search', 'tech_search', 'flights_search', 'events_search', 'get_engine_info']
        available_methods = [method for method in dir(tool) if not method.startswith('_') and callable(getattr(tool, method))]
        
        missing_methods = [method for method in expected_methods if method not in available_methods]
        if missing_methods:
            print(f"⚠️  Missing expected methods: {missing_methods}")
        else:
            print("✅ All expected methods are available")
        
        return True
    except Exception as e:
        print(f"❌ Failed to instantiate WebSearchTools: {e}")
        return False

def test_websearch_tools_schemas():
    """Test that WebSearchTools provides proper tool schemas."""
    try:
        from agent_c_tools.tools.web_search import WebSearchTools
        
        tool = WebSearchTools()
        schemas = tool.tool_schemas()
        
        if schemas:
            print(f"✅ WebSearchTools provides {len(schemas)} tool schemas")
            for schema in schemas:
                if 'function' in schema and 'name' in schema['function']:
                    print(f"   - {schema['function']['name']}")
        else:
            print("❌ WebSearchTools provides no tool schemas")
            return False
        
        return True
    except Exception as e:
        print(f"❌ Error getting WebSearchTools schemas: {e}")
        return False

def main():
    """Run all tests."""
    print("Testing WebSearchTools registration in Agent C system...")
    print("=" * 60)
    
    tests = [
        ("Web Search Module Import", test_websearch_tools_import),
        ("Main Module Import", test_websearch_tools_main_import),
        ("Full Import", test_websearch_tools_full_import),
        ("Instantiation", test_websearch_tools_instantiation),
        ("Schemas", test_websearch_tools_schemas)
    ]
    
    results = []
    for test_name, test_func in tests:
        print(f"\n{test_name}:")
        result = test_func()
        results.append((test_name, result))
    
    print("\n" + "=" * 60)
    print("Test Results Summary:")
    
    passed = 0
    failed = 0
    
    for test_name, result in results:
        if result is True:
            print(f"✅ {test_name}: PASSED")
            passed += 1
        else:
            print(f"❌ {test_name}: FAILED")
            failed += 1
    
    print(f"\nTotal: {passed} passed, {failed} failed")
    
    if failed == 0:
        print("🎉 All tests passed! WebSearchTools is properly registered.")
        return 0
    else:
        print("💥 Some tests failed. WebSearchTools registration needs attention.")
        return 1

if __name__ == "__main__":
    sys.exit(main())
#!/usr/bin/env python3
"""
Simple test script to verify import paths are working correctly.
"""
import sys
import os

# Add the web search tools directory to Python path
web_search_path = os.path.abspath("../../../src/agent_c_tools/tools/web_search")
if web_search_path not in sys.path:
    sys.path.insert(0, web_search_path)

print(f"Python path: {sys.path}")
print(f"Web search path: {web_search_path}")
print(f"Web search path exists: {os.path.exists(web_search_path)}")

try:
    print("Testing base.models import...")
    from agent_c_tools.tools.web_search.base.models import SearchParameters, SearchType
    print("✅ base.models import successful")
except ImportError as e:
    print(f"❌ base.models import failed: {e}")
try:
    print("Testing engines import...")
    from agent_c_tools.tools.web_search.engines.duckduckgo_engine import DuckDuckGoEngine
    print("✅ engines import successful")
except ImportError as e:
    print(f"❌ engines import failed: {e}")

try:
    print("Testing web_search_tools import...")
    from agent_c_tools.tools.web_search.web_search_tools import WebSearchTools
    print("✅ web_search_tools import successful")
except ImportError as e:
    print(f"❌ web_search_tools import failed: {e}")

print("Import test complete!")
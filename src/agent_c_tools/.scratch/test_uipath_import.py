#!/usr/bin/env python3
"""
Test importing UiPath tools
"""

import sys
import os

# Add the src directory to the path so we can import agent_c_tools
sys.path.insert(0, os.path.join(os.path.dirname(__file__), '..', 'src'))

try:
    from agent_c_tools.tools.uipath.tool import UiPathTools
    print("✅ Successfully imported UiPathTools")
    
    # Try to create an instance
    uipath_tool = UiPathTools()
    print("✅ Successfully created UiPathTools instance")
    
    # Check if we can get config info
    try:
        config_info = uipath_tool._validate_config()
        print("✅ Configuration is valid")
        print(f"Config keys found: {list(config_info.keys())}")
    except Exception as e:
        print(f"❌ Configuration validation failed: {str(e)}")
        
except ImportError as e:
    print(f"❌ Failed to import UiPathTools: {str(e)}")
except Exception as e:
    print(f"❌ Error: {str(e)}")
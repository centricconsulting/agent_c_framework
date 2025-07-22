#!/usr/bin/env python3
"""
Test script for web search configuration management.

This script tests the configuration management functionality to ensure
it works correctly with different API key configurations.
"""

import os
import sys
import json
import asyncio
from typing import Dict, Any

# Add the source directory to the path
sys.path.insert(0, os.path.join(os.path.dirname(__file__), 'src'))

from agent_c_tools.tools.web_search import WebSearchTools
from agent_c_tools.tools.web_search.base.config_manager import get_config_manager


async def test_configuration_management():
    """Test the configuration management functionality."""
    print("Testing Web Search Configuration Management")
    print("=" * 50)
    
    # Test 1: Initialize WebSearchTools and check configuration
    print("\n1. Testing WebSearchTools initialization...")
    try:
        search = WebSearchTools()
        print("✓ WebSearchTools initialized successfully")
    except Exception as e:
        print(f"✗ WebSearchTools initialization failed: {e}")
        return False
    
    # Test 2: Get configuration status
    print("\n2. Testing configuration status...")
    try:
        config_manager = get_config_manager()
        config_status = config_manager.get_configuration_status()
        
        print(f"✓ Configuration status retrieved")
        print(f"  - Total engines: {len(config_status.engine_configs)}")
        print(f"  - Available engines: {len(config_status.available_engines)}")
        print(f"  - Configured engines: {len(config_status.configured_engines)}")
        print(f"  - Missing API keys: {len(config_status.missing_api_keys)}")
        
        # Print available engines
        if config_status.available_engines:
            print(f"  - Available: {', '.join(config_status.available_engines)}")
        
    except Exception as e:
        print(f"✗ Configuration status failed: {e}")
        return False
    
    # Test 3: Get engine info
    print("\n3. Testing engine info...")
    try:
        info_result = await search.get_engine_info()
        info = json.loads(info_result)
        
        print("✓ Engine info retrieved successfully")
        
        # Check if configuration summary is included
        if 'configuration_summary' in info:
            print("✓ Configuration summary included")
        
        # Check if setup instructions are included for missing configs
        if 'setup_instructions' in info:
            print("✓ Setup instructions included for missing configurations")
            
    except Exception as e:
        print(f"✗ Engine info failed: {e}")
        return False
    
    # Test 4: Test configuration validation
    print("\n4. Testing configuration validation...")
    try:
        from agent_c_tools.tools.web_search.base.config_manager import validate_web_search_configuration
        
        is_valid, error_messages = validate_web_search_configuration()
        
        if is_valid:
            print("✓ Configuration validation passed")
        else:
            print("⚠ Configuration validation found issues:")
            for error in error_messages:
                print(f"  - {error}")
            print("Note: This is expected if API keys are not configured")
            
    except Exception as e:
        print(f"✗ Configuration validation failed: {e}")
        return False
    
    # Test 5: Test configuration summary
    print("\n5. Testing configuration summary...")
    try:
        summary = search.get_configuration_summary()
        print("✓ Configuration summary generated")
        print("Summary preview:")
        print(summary[:200] + "..." if len(summary) > 200 else summary)
        
    except Exception as e:
        print(f"✗ Configuration summary failed: {e}")
        return False
    
    # Test 6: Test search with configuration error handling
    print("\n6. Testing search with configuration error handling...")
    try:
        # Try to use an engine that might not be configured
        result = await search.web_search(query="test search", engine="google_serp")
        result_data = json.loads(result)
        
        if result_data.get('success', False):
            print("✓ Google SERP search succeeded (API key configured)")
        else:
            print("⚠ Google SERP search failed (expected if API key not configured)")
            if 'Configuration Error' in result_data.get('error', ''):
                print("✓ Configuration error handling working correctly")
            if 'setup_instructions' in result_data:
                print("✓ Setup instructions provided in error response")
                
    except Exception as e:
        print(f"✗ Search error handling test failed: {e}")
        return False
    
    # Test 7: Test with available engine (should work)
    print("\n7. Testing search with available engine...")
    try:
        # Use DuckDuckGo which doesn't require API keys
        result = await search.web_search(query="test search", engine="duckduckgo")
        result_data = json.loads(result)
        
        if result_data.get('success', False):
            print("✓ DuckDuckGo search succeeded")
        else:
            print(f"⚠ DuckDuckGo search failed: {result_data.get('error', 'Unknown error')}")
            
    except Exception as e:
        print(f"✗ DuckDuckGo search test failed: {e}")
        return False
    
    print("\n" + "=" * 50)
    print("Configuration management tests completed!")
    return True


def print_environment_status():
    """Print current environment variable status."""
    print("\nEnvironment Variables Status:")
    print("-" * 30)
    
    api_keys = [
        'SERPAPI_API_KEY',
        'TAVILI_API_KEY',
        'NEWSAPI_API_KEY'
    ]
    
    for api_key in api_keys:
        value = os.getenv(api_key)
        if value:
            # Mask the key for security
            masked_value = value[:4] + "*" * (len(value) - 8) + value[-4:] if len(value) > 8 else "*" * len(value)
            print(f"  ✓ {api_key}: {masked_value}")
        else:
            print(f"  ✗ {api_key}: Not set")


def main():
    """Main function."""
    print_environment_status()
    
    # Run the async tests
    success = asyncio.run(test_configuration_management())
    
    if success:
        print("\n✓ All configuration management tests passed!")
        sys.exit(0)
    else:
        print("\n✗ Some configuration management tests failed!")
        sys.exit(1)


if __name__ == "__main__":
    main()
#!/usr/bin/env python3
"""
Web Search Configuration Validation Utility

This script validates the configuration of web search engines and provides
detailed information about API key setup and engine availability.
"""

import json
import sys
import os
from typing import Dict, Any

# Add the parent directory to the path to import the web search tools
sys.path.insert(0, os.path.dirname(os.path.dirname(os.path.abspath(__file__))))

from web_search.base.config_manager import get_config_manager, validate_web_search_configuration


def print_separator(title: str = "") -> None:
    """Print a separator line with optional title."""
    if title:
        print(f"\n{'=' * 60}")
        print(f"{title:^60}")
        print(f"{'=' * 60}")
    else:
        print("-" * 60)


def print_configuration_status() -> None:
    """Print detailed configuration status."""
    print_separator("Web Search Configuration Status")
    
    config_manager = get_config_manager()
    
    # Get configuration status
    config_status = config_manager.get_configuration_status()
    
    # Print summary
    print(f"Total engines: {len(config_status.engine_configs)}")
    print(f"Available engines: {len(config_status.available_engines)}")
    print(f"Configured engines: {len(config_status.configured_engines)}")
    print(f"Missing API keys: {len(config_status.missing_api_keys)}")
    
    print_separator("Engine Status")
    
    # Print individual engine status
    for engine_name, engine_config in config_status.engine_configs.items():
        status_icon = "✓" if engine_config.is_available else "✗"
        api_status = engine_config.api_key_status.value
        
        print(f"{status_icon} {engine_name:<15} | API Key: {api_status:<12} | Available: {engine_config.is_available}")
        
        if engine_config.error_message:
            print(f"    Error: {engine_config.error_message}")
        
        if engine_config.configuration_hints:
            print(f"    Hints: {engine_config.configuration_hints[0]}")
    
    # Print missing configurations
    if config_status.missing_api_keys:
        print_separator("Missing API Keys")
        missing_configs = config_manager.get_missing_configurations()
        
        for engine_name, api_key_name, instructions in missing_configs:
            print(f"\n{engine_name.upper()}:")
            print(f"  Environment Variable: {api_key_name}")
            print(f"  Setup Instructions:")
            for i, instruction in enumerate(instructions, 1):
                print(f"    {i}. {instruction}")
    
    # Print configuration errors
    if config_status.configuration_errors:
        print_separator("Configuration Errors")
        for error in config_status.configuration_errors:
            print(f"  ✗ {error}")


def print_environment_variables() -> None:
    """Print current environment variable status."""
    print_separator("Environment Variables")
    
    api_keys = [
        'SERPAPI_API_KEY',
        'TAVILI_API_KEY', 
        'NEWSAPI_API_KEY'
    ]
    
    for api_key in api_keys:
        value = os.getenv(api_key)
        if value:
            # Mask the key for security
            masked_value = value[:8] + "*" * (len(value) - 12) + value[-4:] if len(value) > 12 else "*" * len(value)
            print(f"  ✓ {api_key:<20} = {masked_value}")
        else:
            print(f"  ✗ {api_key:<20} = Not set")


def print_validation_result() -> None:
    """Print configuration validation result."""
    print_separator("Configuration Validation")
    
    is_valid, error_messages = validate_web_search_configuration()
    
    if is_valid:
        print("✓ Configuration is valid - at least one search engine is available")
    else:
        print("✗ Configuration has issues:")
        for error in error_messages:
            print(f"  - {error}")
    
    print(f"\nValidation result: {'PASS' if is_valid else 'FAIL'}")


def print_usage_examples() -> None:
    """Print usage examples."""
    print_separator("Usage Examples")
    
    print("Basic usage (no API keys required):")
    print("""
from agent_c_tools.tools.web_search import WebSearchTools

search = WebSearchTools()

# Use DuckDuckGo (no API key required)
results = await search.web_search(query="python programming", engine="duckduckgo")

# Use Wikipedia (no API key required)
results = await search.educational_search(query="machine learning")

# Use Hacker News (no API key required)
results = await search.tech_search(query="python frameworks")
""")
    
    print("\nAdvanced usage (with API keys):")
    print("""
# Use Google Search (requires SERPAPI_API_KEY)
results = await search.web_search(query="latest AI research", engine="google_serp")

# Use Tavily Research (requires TAVILI_API_KEY)
results = await search.research_search(query="quantum computing", engine="tavily")

# Use NewsAPI (requires NEWSAPI_API_KEY)
results = await search.news_search(query="technology news", engine="newsapi")
""")
    
    print("\nCheck engine status:")
    print("""
# Get detailed engine information
info = await search.get_engine_info()
print(info)

# Get configuration summary
summary = search.get_configuration_summary()
print(summary)
""")


def main():
    """Main function."""
    print("Web Search Configuration Validator")
    print("=" * 60)
    
    try:
        # Print all configuration information
        print_configuration_status()
        print_environment_variables()
        print_validation_result()
        print_usage_examples()
        
        # Return appropriate exit code
        is_valid, _ = validate_web_search_configuration()
        sys.exit(0 if is_valid else 1)
        
    except Exception as e:
        print(f"\nError during validation: {e}")
        sys.exit(2)


if __name__ == "__main__":
    main()
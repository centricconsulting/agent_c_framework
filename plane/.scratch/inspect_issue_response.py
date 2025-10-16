#!/usr/bin/env python3
"""
Inspect PLANE API Issue Response

This script investigates the actual API response from PLANE to find sequence IDs.
Users see issues as "AC-9" in the UI, but we need to find where this is in the API.
"""

import asyncio
import json
import sys
from pathlib import Path

# Add the tools directory to path so we can import PlaneClient
sys.path.insert(0, str(Path(__file__).parent.parent.parent / 'src' / 'agent_c_tools' / 'src'))

from agent_c_tools.tools.plane.client.plane_client import PlaneClient


async def inspect_issue_response():
    """
    Fetch issues and inspect the raw response structure
    """
    print("=" * 80)
    print("PLANE API Issue Response Inspector")
    print("=" * 80)
    print()
    
    # Initialize client (will load cookies from ~/.plane/cookies/agent_c.enc)
    print("Initializing PlaneClient...")
    client = PlaneClient(
        base_url="http://localhost",
        workspace_slug="agent_c"
    )
    print("✓ Client initialized")
    print()
    
    # Project ID to inspect
    project_id = "dad9fe27-de38-4dd6-865f-0455e426339a"
    print(f"Fetching issues from project: {project_id}")
    print()
    
    try:
        # Get issues
        response = await client.list_issues(project_id=project_id)
        
        # Check response structure
        print("-" * 80)
        print("RESPONSE STRUCTURE CHECK")
        print("-" * 80)
        print(f"Response type: {type(response)}")
        print()
        
        # Handle both list and dict responses
        issues = None
        if isinstance(response, dict):
            print("Response is a DICTIONARY")
            print(f"Dictionary keys: {list(response.keys())}")
            print()
            
            if 'results' in response:
                print("✓ Found 'results' key in response")
                issues = response['results']
                print(f"Number of issues in 'results': {len(issues)}")
                
                # Show other keys in the response
                for key, value in response.items():
                    if key != 'results':
                        print(f"  - {key}: {value}")
            else:
                print("Response dict does not have 'results' key")
                print("Full response structure:")
                print(json.dumps(response, indent=2, default=str))
                return
        
        elif isinstance(response, list):
            print("Response is a LIST")
            issues = response
            print(f"Number of issues: {len(issues)}")
        
        else:
            print(f"Unexpected response type: {type(response)}")
            return
        
        print()
        
        if not issues or len(issues) == 0:
            print("No issues found in the response")
            return
        
        # Inspect the first issue in detail
        first_issue = issues[0]
        
        print("-" * 80)
        print("FIRST ISSUE - COMPLETE RAW RESPONSE")
        print("-" * 80)
        print(json.dumps(first_issue, indent=2, default=str))
        print()
        
        print("-" * 80)
        print("SEARCHING FOR SEQUENCE ID FIELDS")
        print("-" * 80)
        
        # List of potential field names to check
        potential_fields = [
            'sequence_id',
            'sequence',
            'number',
            'display_id',
            'identifier',
            'project_identifier',
            'issue_identifier',
            'issue_number',
            'project_id',
            'project'
        ]
        
        found_fields = {}
        for field in potential_fields:
            if field in first_issue:
                found_fields[field] = first_issue[field]
                print(f"✓ FOUND: '{field}' = {first_issue[field]}")
        
        print()
        
        # Check for 'AC' string anywhere in the issue
        print("-" * 80)
        print("SEARCHING FOR 'AC' STRING IN ALL FIELDS")
        print("-" * 80)
        
        def search_for_ac(obj, path=""):
            """Recursively search for 'AC' in all string values"""
            if isinstance(obj, dict):
                for key, value in obj.items():
                    new_path = f"{path}.{key}" if path else key
                    search_for_ac(value, new_path)
            elif isinstance(obj, list):
                for i, item in enumerate(obj):
                    new_path = f"{path}[{i}]"
                    search_for_ac(item, new_path)
            elif isinstance(obj, str):
                if 'AC' in obj.upper():
                    print(f"  Found 'AC' at {path}: {obj}")
        
        search_for_ac(first_issue)
        print()
        
        # Show all top-level keys
        print("-" * 80)
        print("ALL TOP-LEVEL FIELDS IN ISSUE")
        print("-" * 80)
        for key in sorted(first_issue.keys()):
            value = first_issue[key]
            # Truncate long values
            value_str = str(value)
            if len(value_str) > 100:
                value_str = value_str[:100] + "..."
            print(f"  {key}: {type(value).__name__} = {value_str}")
        
        print()
        
        # Summary
        print("=" * 80)
        print("SUMMARY")
        print("=" * 80)
        if found_fields:
            print("Found potential sequence ID fields:")
            for field, value in found_fields.items():
                print(f"  • {field}: {value}")
        else:
            print("No obvious sequence ID fields found")
            print("Review the complete raw response above for clues")
        
        print()
        print("=" * 80)
        
    except Exception as e:
        print(f"ERROR: {e}")
        import traceback
        traceback.print_exc()


if __name__ == "__main__":
    asyncio.run(inspect_issue_response())

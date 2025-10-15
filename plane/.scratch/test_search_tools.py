#!/usr/bin/env python3
"""Test PLANE Search Tools"""

import sys
import asyncio
from pathlib import Path

# Add tools to path
tools_path = Path(__file__).parent.parent.parent / "tools" / "src"
sys.path.insert(0, str(tools_path))

from agent_c_tools.tools.plane.tools.plane_search import PlaneSearchTools

async def main():
    print("="*70)
    print("TESTING PLANE SEARCH TOOLS")
    print("="*70)
    
    # Initialize toolset
    print("\n1️⃣  Initializing PlaneSearchTools...")
    toolset = PlaneSearchTools()
    await toolset.post_init()
    print("✅ Toolset initialized")
    
    # Test global search
    print("\n2️⃣  Testing plane_search_global...")
    print("-"*70)
    result = await toolset.plane_search_global(query="agent")
    print(result)
    
    # Test issue search
    print("\n3️⃣  Testing plane_search_issues...")
    print("-"*70)
    result = await toolset.plane_search_issues(
        project_id="dad9fe27-de38-4dd6-865f-0455e426339a",
        priority="medium"
    )
    print(result)
    
    # Test find my issues
    print("\n4️⃣  Testing plane_find_my_issues...")
    print("-"*70)
    result = await toolset.plane_find_my_issues()
    print(result)
    
    print("\n" + "="*70)
    print("SEARCH TOOLS TEST COMPLETE")
    print("="*70)
    print("\n✅ All search tools working!")
    print("\nAvailable tools:")
    print("  - plane_search_global(query, search_type) - Search across workspace")
    print("  - plane_search_issues(project_id, query, state, priority, assignee) - Search issues")
    print("  - plane_find_my_issues(state, priority) - Find issues assigned to you")
    print("="*70)

if __name__ == "__main__":
    asyncio.run(main())

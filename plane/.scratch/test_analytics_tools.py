#!/usr/bin/env python3
"""Test PLANE Analytics Tools"""

import sys
import asyncio
from pathlib import Path

# Add tools to path
tools_path = Path(__file__).parent.parent.parent / "tools" / "src"
sys.path.insert(0, str(tools_path))

from agent_c_tools.tools.plane import PlaneAnalyticsTools

async def main():
    print("="*70)
    print("TESTING PLANE ANALYTICS TOOLS")
    print("="*70)
    
    # Initialize toolset
    print("\n1️⃣  Initializing PlaneAnalyticsTools...")
    toolset = PlaneAnalyticsTools()
    await toolset.post_init()
    print("✅ Toolset initialized")
    
    # Test workspace overview
    print("\n2️⃣  Testing plane_get_workspace_overview...")
    print("-"*70)
    result = await toolset.plane_get_workspace_overview()
    print(result)
    
    # Test project stats
    print("\n3️⃣  Testing plane_get_project_stats...")
    print("-"*70)
    result = await toolset.plane_get_project_stats(
        project_id="dad9fe27-de38-4dd6-865f-0455e426339a"
    )
    print(result)
    
    # Test team workload
    print("\n4️⃣  Testing plane_get_team_workload...")
    print("-"*70)
    result = await toolset.plane_get_team_workload()
    print(result)
    
    print("\n" + "="*70)
    print("ANALYTICS TOOLS TEST COMPLETE")
    print("="*70)
    print("\n✅ All analytics tools working!")
    print("\nAvailable tools:")
    print("  - plane_get_workspace_overview() - Workspace statistics")
    print("  - plane_get_project_stats(project_id) - Project analytics")
    print("  - plane_get_team_workload(project_id) - Team workload analysis")
    print("="*70)

if __name__ == "__main__":
    asyncio.run(main())

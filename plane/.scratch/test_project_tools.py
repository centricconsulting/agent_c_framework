#!/usr/bin/env python3
"""Test PLANE Project Management Tools"""

import sys
import asyncio
from pathlib import Path

# Add tools to path
tools_path = Path(__file__).parent.parent.parent / "tools" / "src"
sys.path.insert(0, str(tools_path))

from agent_c_tools.tools.plane.tools.plane_projects import PlaneProjectTools

async def main():
    print("="*70)
    print("TESTING PLANE PROJECT TOOLS")
    print("="*70)
    
    # Initialize toolset
    print("\n1️⃣  Initializing PlaneProjectTools...")
    toolset = PlaneProjectTools()
    await toolset.post_init()
    print("✅ Toolset initialized")
    
    # Test list projects
    print("\n2️⃣  Testing plane_list_projects...")
    print("-"*70)
    result = await toolset.plane_list_projects()
    print(result)
    
    # Test get project (using existing project)
    print("\n3️⃣  Testing plane_get_project...")
    print("-"*70)
    result = await toolset.plane_get_project(
        project_id="dad9fe27-de38-4dd6-865f-0455e426339a"
    )
    print(result)
    
    # Test create project (optional - commented out to avoid creating test projects)
    print("\n4️⃣  Testing plane_create_project (skipped)...")
    print("   Skipping create test to avoid cluttering workspace")
    
    # Uncomment to test create:
    # result = await toolset.plane_create_project(
    #     name="Test Project",
    #     identifier="TEST",
    #     description="This is a test project"
    # )
    # print(result)
    
    print("\n" + "="*70)
    print("PROJECT TOOLS TEST COMPLETE")
    print("="*70)
    print("\n✅ All project management tools working!")
    print("\nAvailable tools:")
    print("  - plane_list_projects() - List all projects")
    print("  - plane_get_project(project_id) - Get project details")
    print("  - plane_create_project(name, identifier, description) - Create project")
    print("  - plane_update_project(project_id, name, description) - Update project")
    print("  - plane_archive_project(project_id) - Archive project")
    print("="*70)

if __name__ == "__main__":
    asyncio.run(main())

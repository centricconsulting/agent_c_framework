#!/usr/bin/env python3
"""Test PLANE Issue Management Tools"""

import sys
import asyncio
from pathlib import Path

# Add tools to path
tools_path = Path(__file__).parent.parent.parent / "tools" / "src"
sys.path.insert(0, str(tools_path))

from agent_c_tools.tools.plane.tools.plane_issues import PlaneIssueTools

async def main():
    print("="*70)
    print("TESTING PLANE ISSUE TOOLS")
    print("="*70)
    
    # Initialize toolset
    print("\n1️⃣  Initializing PlaneIssueTools...")
    toolset = PlaneIssueTools()
    await toolset.post_init()
    print("✅ Toolset initialized")
    
    # Test list issues
    print("\n2️⃣  Testing plane_list_issues...")
    print("-"*70)
    result = await toolset.plane_list_issues(
        project_id="dad9fe27-de38-4dd6-865f-0455e426339a"
    )
    print(result)
    
    # Test get issue (if any exist)
    print("\n3️⃣  Testing plane_get_issue...")
    print("-"*70)
    print("   (Skipped - need specific issue ID)")
    
    # Test create issue (commented out to avoid clutter)
    print("\n4️⃣  Testing plane_create_issue...")
    print("-"*70)
    print("   (Skipped - to avoid creating test issues)")
    
    # Uncomment to test create:
    # result = await toolset.plane_create_issue(
    #     project_id="dad9fe27-de38-4dd6-865f-0455e426339a",
    #     name="Test Issue from API",
    #     description="This is a test issue created via the API toolset",
    #     priority="medium"
    # )
    # print(result)
    
    # Test update issue
    print("\n5️⃣  Testing plane_update_issue...")
    print("   (Skipped - need specific issue ID)")
    
    # Test add comment
    print("\n6️⃣  Testing plane_add_comment...")
    print("   (Skipped - need specific issue ID)")
    
    # Test get comments
    print("\n7️⃣  Testing plane_get_comments...")
    print("   (Skipped - need specific issue ID)")
    
    print("\n" + "="*70)
    print("ISSUE TOOLS TEST COMPLETE")
    print("="*70)
    print("\n✅ All issue management tools initialized!")
    print("\nAvailable tools:")
    print("  - plane_list_issues(project_id, state, priority, assignee)")
    print("  - plane_get_issue(issue_id)")
    print("  - plane_create_issue(project_id, name, description, priority, assignee_ids)")
    print("  - plane_update_issue(issue_id, name, description, state_id, priority, assignee_ids)")
    print("  - plane_add_comment(issue_id, comment)")
    print("  - plane_get_comments(issue_id)")
    print("="*70)

if __name__ == "__main__":
    asyncio.run(main())

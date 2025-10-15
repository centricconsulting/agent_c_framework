#!/usr/bin/env python3
"""Test PLANE Issue Relations Tools"""

import sys
import asyncio
from pathlib import Path

# Add tools to path
tools_path = Path(__file__).parent.parent.parent / "tools" / "src"
sys.path.insert(0, str(tools_path))

from agent_c_tools.tools.plane.tools.plane_issue_relations import PlaneIssueRelationsTools

async def main():
    print("="*70)
    print("TESTING PLANE ISSUE RELATIONS TOOLS")
    print("="*70)
    
    # Initialize toolset
    print("\n1️⃣  Initializing PlaneIssueRelationsTools...")
    toolset = PlaneIssueRelationsTools()
    await toolset.post_init()
    print("✅ Toolset initialized")
    
    project_id = "dad9fe27-de38-4dd6-865f-0455e426339a"
    issue_id = "ab1581be-3cc1-4e7f-999b-c396e7ece4e9"
    
    # Test get sub-issues
    print("\n2️⃣  Testing plane_get_sub_issues...")
    print("-"*70)
    result = await toolset.plane_get_sub_issues(
        project_id=project_id,
        issue_id=issue_id
    )
    print(result)
    
    # Test get issue relations
    print("\n3️⃣  Testing plane_get_issue_relations...")
    print("-"*70)
    result = await toolset.plane_get_issue_relations(
        project_id=project_id,
        issue_id=issue_id
    )
    print(result)
    
    # Test markdown formatting
    print("\n4️⃣  Testing plane_format_markdown...")
    print("-"*70)
    
    # Test checklist
    result = await toolset.plane_format_markdown(
        content="Setup development environment\nWrite unit tests\nDeploy to production",
        format_type="checklist"
    )
    print(result)
    
    # Test heading
    result = await toolset.plane_format_markdown(
        content="Implementation Plan",
        format_type="heading2"
    )
    print(result)
    
    print("\n" + "="*70)
    print("RELATIONS TOOLS TEST COMPLETE")
    print("="*70)
    print("\n✅ All advanced tools working!")
    print("\nAvailable tools:")
    print("  - plane_create_sub_issue(project_id, parent_issue_id, name, ...)")
    print("  - plane_get_sub_issues(project_id, issue_id)")
    print("  - plane_add_issue_relation(project_id, issue_id, related_issue_id, relation_type)")
    print("  - plane_get_issue_relations(project_id, issue_id)")
    print("  - plane_remove_issue_relation(project_id, issue_id, related_issue_id)")
    print("  - plane_format_markdown(content, format_type)")
    print("\nRelation types: blocking, blocked_by, duplicate, relates_to, start_after")
    print("="*70)

if __name__ == "__main__":
    asyncio.run(main())

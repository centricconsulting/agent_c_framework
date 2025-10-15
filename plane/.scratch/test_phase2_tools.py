#!/usr/bin/env python3
"""Test Phase 2 Tools - Labels and Bulk Operations"""

import sys
import asyncio
from pathlib import Path

# Add tools to path
tools_path = Path(__file__).parent.parent.parent / "tools" / "src"
sys.path.insert(0, str(tools_path))

from agent_c_tools.tools.plane.tools.plane_labels import PlaneLabelTools
from agent_c_tools.tools.plane.tools.plane_bulk import PlaneBulkTools

async def main():
    print("="*70)
    print("TESTING PHASE 2 TOOLS")
    print("="*70)
    
    # Test Label Tools
    print("\n" + "="*70)
    print("LABEL MANAGEMENT TOOLS")
    print("="*70)
    
    label_tools = PlaneLabelTools()
    await label_tools.post_init()
    print("✅ PlaneLabelTools initialized")
    
    print("\n1️⃣  Testing plane_list_labels...")
    result = await label_tools.plane_list_labels()
    print(result)
    
    print("\n2️⃣  Testing plane_create_label...")
    print("   (Skipped to avoid creating test labels)")
    # result = await label_tools.plane_create_label(
    #     name="Bug",
    #     color="#FF0000",
    #     description="Bug/defect issues"
    # )
    # print(result)
    
    # Test Bulk Tools
    print("\n" + "="*70)
    print("BULK OPERATIONS TOOLS")
    print("="*70)
    
    bulk_tools = PlaneBulkTools()
    await bulk_tools.post_init()
    print("✅ PlaneBulkTools initialized")
    
    print("\n3️⃣  Testing bulk operations...")
    print("   (Skipped - requires multiple issues to test properly)")
    
    print("\n" + "="*70)
    print("PHASE 2 TOOLS TEST COMPLETE")
    print("="*70)
    print("\n✅ All Phase 2 toolsets initialized successfully!")
    
    print("\n📋 NEW TOOLS AVAILABLE:")
    print("\n**PlaneLabelTools (5 tools):**")
    print("  - plane_list_labels(project_id)")
    print("  - plane_create_label(name, color, description)")
    print("  - plane_add_label_to_issue(issue_id, label_id)")
    print("  - plane_remove_label_from_issue(issue_id, label_id)")
    print("  - plane_delete_label(label_id)")
    
    print("\n**PlaneBulkTools (5 tools):**")
    print("  - plane_bulk_update_issues(issue_ids, updates)")
    print("  - plane_bulk_assign_issues(issue_ids, assignee_id)")
    print("  - plane_bulk_change_state(issue_ids, state_id)")
    print("  - plane_bulk_change_priority(issue_ids, priority)")
    print("  - plane_bulk_add_label(issue_ids, label_id)")
    
    print("\n🎉 TOTAL TOOLS NOW: 33 tools across 7 toolsets!")
    print("="*70)

if __name__ == "__main__":
    asyncio.run(main())

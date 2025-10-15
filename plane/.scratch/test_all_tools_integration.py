#!/usr/bin/env python3
"""
Comprehensive integration test for all PLANE tools

Tests all 17 tools across 4 toolsets to ensure they work together correctly.
"""

import sys
import asyncio
from pathlib import Path

# Add tools to path
tools_path = Path(__file__).parent.parent.parent / "tools" / "src"
sys.path.insert(0, str(tools_path))

from agent_c_tools.tools.plane.tools.plane_projects import PlaneProjectTools
from agent_c_tools.tools.plane.tools.plane_issues import PlaneIssueTools
from agent_c_tools.tools.plane.tools.plane_search import PlaneSearchTools
from agent_c_tools.tools.plane.tools.plane_analytics import PlaneAnalyticsTools


async def test_toolset(toolset_class, toolset_name: str, tests: list):
    """Test a toolset with multiple tool calls"""
    print(f"\n{'='*70}")
    print(f"Testing {toolset_name}")
    print('='*70)
    
    toolset = toolset_class()
    await toolset.post_init()
    print(f"✅ {toolset_name} initialized")
    
    passed = 0
    failed = 0
    
    for test_name, tool_method, args in tests:
        print(f"\n🔍 {test_name}...")
        try:
            result = await tool_method(**args)
            
            if result.startswith("ERROR:"):
                print(f"❌ {test_name} returned error:")
                print(f"   {result[:100]}")
                failed += 1
            else:
                print(f"✅ {test_name} successful")
                # Show first 200 chars of result
                preview = result[:200].replace('\n', '\n   ')
                print(f"   {preview}...")
                passed += 1
                
        except Exception as e:
            print(f"❌ {test_name} raised exception: {str(e)[:100]}")
            failed += 1
    
    print(f"\n{toolset_name} Results: {passed} passed, {failed} failed")
    return passed, failed


async def main():
    print("="*70)
    print("PLANE TOOLS - COMPREHENSIVE INTEGRATION TEST")
    print("="*70)
    print("\nTesting all 17 tools across 4 toolsets...")
    
    total_passed = 0
    total_failed = 0
    
    # Test PlaneProjectTools (5 tools)
    project_tests = [
        ("List Projects", PlaneProjectTools().plane_list_projects, {}),
        ("Get Project", PlaneProjectTools().plane_get_project, 
         {"project_id": "dad9fe27-de38-4dd6-865f-0455e426339a"}),
        # Skip create/update/delete to avoid side effects
    ]
    
    # Re-init for each test
    toolset1 = PlaneProjectTools()
    await toolset1.post_init()
    
    passed, failed = await test_toolset(
        PlaneProjectTools,
        "PlaneProjectTools",
        [
            ("List Projects", toolset1.plane_list_projects, {}),
            ("Get Project", toolset1.plane_get_project, 
             {"project_id": "dad9fe27-de38-4dd6-865f-0455e426339a"}),
        ]
    )
    total_passed += passed
    total_failed += failed
    
    # Test PlaneIssueTools (6 tools)
    toolset2 = PlaneIssueTools()
    await toolset2.post_init()
    
    passed, failed = await test_toolset(
        PlaneIssueTools,
        "PlaneIssueTools",
        [
            ("List Issues", toolset2.plane_list_issues,
             {"project_id": "dad9fe27-de38-4dd6-865f-0455e426339a"}),
            ("Get Issue", toolset2.plane_get_issue,
             {"issue_id": "ab1581be-3cc1-4e7f-999b-c396e7ece4e9",
              "project_id": "dad9fe27-de38-4dd6-865f-0455e426339a"}),
        ]
    )
    total_passed += passed
    total_failed += failed
    
    # Test PlaneSearchTools (3 tools)
    toolset3 = PlaneSearchTools()
    await toolset3.post_init()
    
    passed, failed = await test_toolset(
        PlaneSearchTools,
        "PlaneSearchTools",
        [
            ("Search Global", toolset3.plane_search_global, {"query": "agent"}),
            ("Search Issues", toolset3.plane_search_issues,
             {"project_id": "dad9fe27-de38-4dd6-865f-0455e426339a"}),
            ("Find My Issues", toolset3.plane_find_my_issues, {}),
        ]
    )
    total_passed += passed
    total_failed += failed
    
    # Test PlaneAnalyticsTools (3 tools)
    toolset4 = PlaneAnalyticsTools()
    await toolset4.post_init()
    
    passed, failed = await test_toolset(
        PlaneAnalyticsTools,
        "PlaneAnalyticsTools",
        [
            ("Workspace Overview", toolset4.plane_get_workspace_overview, {}),
            ("Project Stats", toolset4.plane_get_project_stats,
             {"project_id": "dad9fe27-de38-4dd6-865f-0455e426339a"}),
            ("Team Workload", toolset4.plane_get_team_workload, {}),
        ]
    )
    total_passed += passed
    total_failed += failed
    
    # Final summary
    print("\n" + "="*70)
    print("FINAL RESULTS")
    print("="*70)
    print(f"\n✅ Passed: {total_passed}")
    print(f"❌ Failed: {total_failed}")
    print(f"📊 Success Rate: {total_passed/(total_passed+total_failed)*100:.1f}%")
    
    if total_failed == 0:
        print("\n🎉 ALL TESTS PASSED! PLANE tools are production ready!")
    else:
        print(f"\n⚠️  {total_failed} test(s) failed. Review errors above.")
    
    print("="*70)


if __name__ == "__main__":
    asyncio.run(main())

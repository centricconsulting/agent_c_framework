#!/usr/bin/env python3
"""
Test that all PLANE modules can be imported without errors.
This must pass before integrating with the server.
"""

import sys
from pathlib import Path

# Add project to path
project_root = Path(__file__).parent.parent / "src" / "agent_c_tools" / "src"
sys.path.insert(0, str(project_root))

print("="*70)
print("PLANE TOOLS - IMPORT VERIFICATION TEST")
print("="*70)
print("\nThis test verifies all PLANE modules can be imported safely.\n")

tests_passed = 0
tests_failed = 0
errors = []

def test_import(module_path, description):
    """Test importing a module"""
    global tests_passed, tests_failed
    
    print(f"Testing: {description}...")
    try:
        exec(f"import {module_path}")
        print(f"  ✅ Success")
        tests_passed += 1
        return True
    except Exception as e:
        print(f"  ❌ FAILED: {str(e)[:100]}")
        tests_failed += 1
        errors.append((description, str(e)))
        return False

# Test imports in dependency order
print("\n1️⃣  Testing Core Infrastructure...")
print("-"*70)
test_import("agent_c_tools.tools.plane.auth.cookie_manager", "CookieManager")
test_import("agent_c_tools.tools.plane.auth.plane_session", "PlaneSession")
test_import("agent_c_tools.tools.plane.client.plane_client", "PlaneClient")

print("\n2️⃣  Testing Toolsets...")
print("-"*70)
test_import("agent_c_tools.tools.plane.tools.plane_projects", "PlaneProjectTools")
test_import("agent_c_tools.tools.plane.tools.plane_issues", "PlaneIssueTools")
test_import("agent_c_tools.tools.plane.tools.plane_search", "PlaneSearchTools")
test_import("agent_c_tools.tools.plane.tools.plane_analytics", "PlaneAnalyticsTools")
test_import("agent_c_tools.tools.plane.tools.plane_issue_relations", "PlaneIssueRelationsTools")
test_import("agent_c_tools.tools.plane.tools.plane_labels", "PlaneLabelTools")
test_import("agent_c_tools.tools.plane.tools.plane_bulk", "PlaneBulkTools")

print("\n3️⃣  Testing Package Imports...")
print("-"*70)
test_import("agent_c_tools.tools.plane", "PLANE Package")

# Summary
print("\n" + "="*70)
print("TEST RESULTS")
print("="*70)
print(f"\n✅ Passed: {tests_passed}")
print(f"❌ Failed: {tests_failed}")
print(f"📊 Success Rate: {tests_passed/(tests_passed+tests_failed)*100:.1f}%")

if tests_failed > 0:
    print("\n❌ ERRORS FOUND:")
    print("-"*70)
    for desc, error in errors:
        print(f"\n{desc}:")
        print(f"  {error}")
    print("\n⚠️  DO NOT INTEGRATE - Fix errors first!")
else:
    print("\n🎉 ALL IMPORTS SUCCESSFUL!")
    print("✅ Safe to integrate with server")

print("="*70)

sys.exit(0 if tests_failed == 0 else 1)

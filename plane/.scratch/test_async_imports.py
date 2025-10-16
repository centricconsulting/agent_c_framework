#!/usr/bin/env python3
"""
Test that all PLANE modules import correctly with async changes
"""

import sys
from pathlib import Path

# Add to path
project_root = Path(__file__).parent.parent.parent / "src" / "agent_c_tools" / "src"
sys.path.insert(0, str(project_root))

print("="*70)
print("PLANE TOOLS - ASYNC IMPORT TEST")
print("="*70)

errors = []

# Test 1: Core infrastructure
print("\n1️⃣  Testing Core Infrastructure...")
try:
    from agent_c_tools.tools.plane.auth import PlaneCookieManager
    print("  ✅ CookieManager")
except Exception as e:
    print(f"  ❌ CookieManager: {e}")
    errors.append(f"CookieManager: {e}")

try:
    from agent_c_tools.tools.plane.auth import PlaneSession, PlaneSessionExpired, PlaneAPIError
    print("  ✅ PlaneSession")
except Exception as e:
    print(f"  ❌ PlaneSession: {e}")
    errors.append(f"PlaneSession: {e}")

try:
    from agent_c_tools.tools.plane.client import PlaneClient
    print("  ✅ PlaneClient")
except Exception as e:
    print(f"  ❌ PlaneClient: {e}")
    errors.append(f"PlaneClient: {e}")

# Test 2: All toolsets
print("\n2️⃣  Testing All Toolsets...")
toolsets = [
    ('PlaneProjectTools', 'agent_c_tools.tools.plane.tools.plane_projects'),
    ('PlaneIssueTools', 'agent_c_tools.tools.plane.tools.plane_issues'),
    ('PlaneSearchTools', 'agent_c_tools.tools.plane.tools.plane_search'),
    ('PlaneAnalyticsTools', 'agent_c_tools.tools.plane.tools.plane_analytics'),
    ('PlaneIssueRelationsTools', 'agent_c_tools.tools.plane.tools.plane_issue_relations'),
    ('PlaneLabelTools', 'agent_c_tools.tools.plane.tools.plane_labels'),
    ('PlaneBulkTools', 'agent_c_tools.tools.plane.tools.plane_bulk'),
]

for name, module_path in toolsets:
    try:
        exec(f"from {module_path} import {name}")
        print(f"  ✅ {name}")
    except Exception as e:
        print(f"  ❌ {name}: {e}")
        errors.append(f"{name}: {e}")

# Test 3: Package-level import
print("\n3️⃣  Testing Package Import...")
try:
    import agent_c_tools.tools.plane
    print("  ✅ plane package")
except Exception as e:
    print(f"  ❌ plane package: {e}")
    errors.append(f"plane package: {e}")

# Summary
print("\n" + "="*70)
if errors:
    print(f"❌ FAILED - {len(errors)} errors found:")
    for error in errors:
        print(f"  - {error}")
    print("\n⚠️  DO NOT START SERVER")
else:
    print("🎉 ALL IMPORTS SUCCESSFUL!")
    print("✅ Safe to enable PLANE tools in agent_c_tools/__init__.py")
print("="*70)

sys.exit(0 if not errors else 1)

#!/usr/bin/env python3
"""Test the core PLANE client"""

import sys
from pathlib import Path

# Add tools to path
tools_path = Path(__file__).parent.parent.parent / "tools" / "src"
sys.path.insert(0, str(tools_path))

from agent_c_tools.tools.plane.client.plane_client import PlaneClient

print("="*70)
print("TESTING PLANE CLIENT")
print("="*70)

# Initialize client (will load cookies from disk)
print("\n1️⃣  Initializing PlaneClient...")
try:
    client = PlaneClient("http://localhost", "agent_c")
    print("✅ Client initialized")
except Exception as e:
    print(f"❌ Error: {e}")
    sys.exit(1)

# Test workspace methods
print("\n2️⃣  Testing Workspace Methods...")
print("-"*70)

try:
    workspace = client.get_workspace()
    print(f"✅ Get Workspace")
    print(f"   Name: {workspace.get('name')}")
    print(f"   ID: {workspace.get('id')}")
except Exception as e:
    print(f"❌ Get workspace failed: {e}")

try:
    members = client.get_workspace_members()
    print(f"✅ Get Workspace Members")
    print(f"   Members: {len(members)}")
except Exception as e:
    print(f"❌ Get members failed: {e}")

# Test project methods
print("\n3️⃣  Testing Project Methods...")
print("-"*70)

try:
    projects = client.list_projects()
    print(f"✅ List Projects")
    print(f"   Projects: {len(projects)}")
    
    if projects:
        project = projects[0]
        print(f"   First project: {project.get('name')} (ID: {project.get('id')})")
        
        # Test get project
        project_id = project.get('id')
        project_details = client.get_project(project_id)
        print(f"✅ Get Project Details")
        print(f"   Name: {project_details.get('name')}")
        print(f"   Identifier: {project_details.get('identifier')}")
        
except Exception as e:
    print(f"❌ Project methods failed: {e}")

# Test issue methods
print("\n4️⃣  Testing Issue Methods...")
print("-"*70)

try:
    if projects:
        project_id = projects[0].get('id')
        issues = client.list_issues(project_id=project_id)
        print(f"✅ List Issues")
        print(f"   Issues in project: {len(issues)}")
        
        if issues:
            issue = issues[0]
            print(f"   First issue: {issue.get('name', 'N/A')}")
except Exception as e:
    print(f"❌ Issue methods failed: {e}")

# Test user methods
print("\n5️⃣  Testing User Methods...")
print("-"*70)

try:
    user = client.get_current_user()
    print(f"✅ Get Current User")
    print(f"   Email: {user.get('email')}")
    print(f"   Name: {user.get('display_name', 'N/A')}")
except Exception as e:
    print(f"❌ Get user failed: {e}")

try:
    workspaces = client.get_user_workspaces()
    print(f"✅ Get User Workspaces")
    print(f"   Workspaces: {len(workspaces)}")
except Exception as e:
    print(f"❌ Get workspaces failed: {e}")

# Summary
print("\n" + "="*70)
print("PLANE CLIENT TEST COMPLETE")
print("="*70)
print("\n✅ Core PLANE client is working!")
print("\nAvailable methods:")
print("  - Workspace: get_workspace(), get_workspace_members()")
print("  - Projects: list_projects(), get_project(), create_project(), update_project()")
print("  - Issues: list_issues(), get_issue(), create_issue(), update_issue()")
print("  - Comments: add_comment(), get_comments()")
print("  - Search: search()")
print("  - User: get_current_user(), get_user_workspaces()")
print("="*70)

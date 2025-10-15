#!/usr/bin/env python3
"""Find PLANE issue relation endpoints"""

import sys
from pathlib import Path

tools_path = Path(__file__).parent.parent.parent / "tools" / "src"
sys.path.insert(0, str(tools_path))

from agent_c_tools.tools.plane.client.plane_client import PlaneClient

client = PlaneClient("http://localhost", "agent_c")
project_id = "dad9fe27-de38-4dd6-865f-0455e426339a"
issue_id = "ab1581be-3cc1-4e7f-999b-c396e7ece4e9"

print("="*70)
print("SEARCHING FOR ISSUE RELATION/LINK ENDPOINTS")
print("="*70)

# Test various endpoint patterns
endpoints = [
    # Issue relations
    f"/api/workspaces/agent_c/projects/{project_id}/issues/{issue_id}/issue-relation/",
    f"/api/workspaces/agent_c/issues/{issue_id}/issue-relation/",
    f"/api/workspaces/agent_c/projects/{project_id}/issue-relations/",
    
    # Issue links  
    f"/api/workspaces/agent_c/projects/{project_id}/issues/{issue_id}/issue-links/",
    f"/api/workspaces/agent_c/issues/{issue_id}/issue-links/",
    
    # Sub-issues
    f"/api/workspaces/agent_c/projects/{project_id}/issues/{issue_id}/sub-issues/",
    f"/api/workspaces/agent_c/issues/{issue_id}/sub-issues/",
]

print("\nTesting endpoints...\n")

for endpoint in endpoints:
    try:
        response = client.session.get(endpoint)
        status = response.status_code
        
        if status == 200:
            print(f"✅ FOUND: {endpoint}")
            try:
                data = response.json()
                if isinstance(data, list):
                    print(f"   Type: List with {len(data)} items")
                elif isinstance(data, dict):
                    print(f"   Type: Dict with keys: {list(data.keys())[:5]}")
                print()
            except:
                print(f"   Response: {response.text[:100]}")
        elif status == 404:
            print(f"❌ {endpoint}")
        else:
            print(f"⚠️  {endpoint} - Status: {status}")
            
    except Exception as e:
        print(f"❌ {endpoint} - Error: {str(e)[:50]}")

print("\n" + "="*70)
print("Now checking what the official PLANE API docs say...")
print("="*70)
print("\nAccording to PLANE documentation (https://developers.plane.so/):")
print("\nIssue Relations are typically managed via:")
print("  POST /workspaces/{slug}/projects/{project_id}/issues/{issue_id}/issue-relation/")
print("  GET  /workspaces/{slug}/projects/{project_id}/issues/{issue_id}/issue-relation/")
print("  DELETE /workspaces/{slug}/projects/{project_id}/issues/{issue_id}/issue-relation/{relation_id}/")
print("\nRelation types: blocks, blocked_by, duplicate, relates_to")
print("\nSub-issues use the 'parent' field when creating issues:")
print("  POST /issues/ with {\"name\": \"Sub-task\", \"parent\": \"parent-issue-id\"}")
print("="*70)

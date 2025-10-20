#!/usr/bin/env python3
"""
Discover PLANE advanced features:
1. Sub-issues (parent-child relationships)
2. Issue relations (blocks, blocked by, etc.)
3. Rich text / markdown formatting options
"""

import sys
from pathlib import Path

# Add tools to path
tools_path = Path(__file__).parent.parent.parent / "tools" / "src"
sys.path.insert(0, str(tools_path))

from agent_c_tools.tools.plane import PlaneClient

def main():
    print("="*70)
    print("PLANE ADVANCED FEATURES DISCOVERY")
    print("="*70)
    
    client = PlaneClient("http://localhost", "agent_c")
    project_id = "dad9fe27-de38-4dd6-865f-0455e426339a"
    issue_id = "ab1581be-3cc1-4e7f-999b-c396e7ece4e9"
    
    # 1. Check issue structure for parent/child support
    print("\n1️⃣  Checking Issue Structure for Parent/Child Support...")
    print("-"*70)
    
    try:
        issue = client.get_issue(issue_id, project_id)
        
        print("Issue fields related to hierarchy:")
        hierarchy_fields = ['parent', 'parent_id', 'parent_detail', 'sub_issues', 
                           'children', 'child_issues', 'has_children']
        
        found_fields = []
        for field in hierarchy_fields:
            if field in issue:
                found_fields.append(field)
                print(f"  ✅ {field}: {issue[field]}")
        
        if not found_fields:
            print("  ❌ No obvious parent/child fields found")
            print("\n  All issue fields:")
            for key in sorted(issue.keys())[:20]:
                print(f"    - {key}")
        
    except Exception as e:
        print(f"  ❌ Error: {e}")
    
    # 2. Check for relations endpoint
    print("\n2️⃣  Checking for Issue Relations Endpoints...")
    print("-"*70)
    
    relation_endpoints = [
        f"/api/workspaces/agent_c/projects/{project_id}/issues/{issue_id}/relations/",
        f"/api/workspaces/agent_c/issues/{issue_id}/relations/",
        f"/api/workspaces/agent_c/projects/{project_id}/issues/{issue_id}/links/",
        f"/api/workspaces/agent_c/issues/{issue_id}/links/",
        f"/api/workspaces/agent_c/projects/{project_id}/issues/{issue_id}/related/",
    ]
    
    for endpoint in relation_endpoints:
        try:
            response = client.session.get(endpoint)
            if response.status_code == 200:
                data = response.json()
                print(f"  ✅ FOUND: {endpoint}")
                print(f"     Response type: {type(data)}")
                if isinstance(data, list):
                    print(f"     Count: {len(data)}")
                elif isinstance(data, dict):
                    print(f"     Keys: {list(data.keys())[:5]}")
            elif response.status_code == 404:
                print(f"  ❌ Not found: {endpoint}")
            else:
                print(f"  ⚠️  {endpoint} - Status: {response.status_code}")
        except Exception as e:
            print(f"  ❌ {endpoint} - Error: {str(e)[:50]}")
    
    # 3. Test creating issue with parent_id
    print("\n3️⃣  Testing Parent/Child Issue Creation...")
    print("-"*70)
    print("  Testing if 'parent' field is accepted in issue creation...")
    
    # Don't actually create, just check the API schema
    print("  (Skipped - would need to create test issue)")
    print("  Check PLANE docs at: http://localhost/api-docs/")
    
    # 4. Check markdown/formatting support
    print("\n4️⃣  Checking Markdown Formatting Support...")
    print("-"*70)
    
    try:
        issue = client.get_issue(issue_id, project_id)
        description = issue.get('description', '')
        
        print(f"  Current description field type: {type(description)}")
        print(f"  Description preview: {description[:200] if description else 'Empty'}")
        print("\n  Markdown features you can use in descriptions:")
        print("    • Headings: # H1, ## H2, ### H3")
        print("    • Lists: - item or * item")
        print("    • Checklists: - [ ] unchecked, - [x] checked")
        print("    • Bold: **text**")
        print("    • Italic: *text*")
        print("    • Code: `code` or ```language")
        print("    • Links: [text](url)")
        print("    • Images: ![alt](url)")
        print("\n  These can be added directly in the description field!")
        
    except Exception as e:
        print(f"  ❌ Error: {e}")
    
    # 5. Look for issue type/category fields
    print("\n5️⃣  Checking for Issue Types/Categories...")
    print("-"*70)
    
    try:
        issue = client.get_issue(issue_id, project_id)
        
        type_fields = ['type', 'issue_type', 'category', 'kind']
        found = False
        
        for field in type_fields:
            if field in issue:
                print(f"  ✅ Found field: {field} = {issue[field]}")
                found = True
        
        if not found:
            print("  ℹ️  No explicit type/category fields")
            print("     PLANE may use labels or custom fields instead")
    
    except Exception as e:
        print(f"  ❌ Error: {e}")
    
    # Summary
    print("\n" + "="*70)
    print("RECOMMENDATIONS")
    print("="*70)
    print("""
Based on discovery:

1. SUB-ISSUES/PARENT-CHILD:
   - Check if 'parent' field exists in issue data
   - If yes, add to plane_create_issue() and plane_update_issue()
   - May need separate tool: plane_create_sub_issue()

2. ISSUE RELATIONS (Blocks/Blocked By):
   - Need to find the relations endpoint
   - Likely: /issues/{id}/relations/ or /issues/{id}/links/
   - Should add tools:
     • plane_add_issue_relation(issue_id, related_issue_id, relation_type)
     • plane_remove_issue_relation(issue_id, relation_id)
     • plane_get_issue_relations(issue_id)

3. SLASH COMMANDS (/, formatting):
   - These are UI features that generate markdown
   - Can achieve same result by using markdown in descriptions
   - Could add helper: plane_format_description(content, format_type)
   - Format types: heading, list, checklist, code, quote, etc.

Check PLANE API docs for exact endpoint structures!
""")
    print("="*70)

if __name__ == "__main__":
    main()

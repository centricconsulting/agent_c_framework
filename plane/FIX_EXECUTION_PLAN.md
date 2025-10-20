# PLANE API Errors - Fix Execution Plan

**Date:** 2025-10-16  
**Status:** Ready to Execute  
**Workspace:** //plane for coordination, //project for implementation

---

## ✅ Root Cause CONFIRMED

**All 4 errors have the SAME root cause:**

PLANE API requires **project_id in the endpoint path** for single-issue operations, but our code makes it optional and falls back to workspace-level endpoints that return 404.

**Working:** `/api/workspaces/agent_c/projects/{project_id}/issues/{issue_id}/`  
**Failing:** `/api/workspaces/agent_c/issues/{issue_id}/` ← Returns 404

---

## 🔧 Fixes Needed

### Fix 1: Make project_id REQUIRED in plane_client.py

**File:** `//project/src/agent_c_tools/src/agent_c_tools/tools/plane/client/plane_client.py`

**Changes:**
```python
# BEFORE:
async def get_issue(self, issue_id: str, project_id: Optional[str] = None):
    if project_id:
        endpoint = f'/workspaces/{ws}/projects/{project_id}/issues/{issue_id}/'
    else:
        endpoint = f'/workspaces/{ws}/issues/{issue_id}/'  # FAILS!

# AFTER:
async def get_issue(self, issue_id: str, project_id: str):  # REQUIRED!
    endpoint = f'/workspaces/{ws}/projects/{project_id}/issues/{issue_id}/'
```

```python
# BEFORE:
async def update_issue(self, issue_id: str, updates: Dict[str, Any]):
    endpoint = f'/workspaces/{ws}/issues/{issue_id}/'  # FAILS!

# AFTER:
async def update_issue(self, issue_id: str, project_id: str, updates: Dict[str, Any]):
    endpoint = f'/workspaces/{ws}/projects/{project_id}/issues/{issue_id}/'
```

```python
# BEFORE:
async def delete_issue(self, issue_id: str):
    endpoint = f'/workspaces/{ws}/issues/{issue_id}/'  # FAILS!

# AFTER:
async def delete_issue(self, issue_id: str, project_id: str):
    endpoint = f'/workspaces/{ws}/projects/{project_id}/issues/{issue_id}/'
```

---

### Fix 2: Update plane_issues.py to pass project_id

**File:** `//project/src/agent_c_tools/src/agent_c_tools/tools/plane/tools/plane_issues.py`

**Changes:**

**plane_get_issue:**
- Add project_id to JSON schema as REQUIRED
- Pass to client: `await self.client.get_issue(issue_id, project_id)`

**plane_update_issue:**
- Add project_id to JSON schema as REQUIRED  
- Pass to client: `await self.client.update_issue(issue_id, project_id, updates)`

---

### Fix 3: Update plane_labels.py

**File:** `//project/src/agent_c_tools/src/agent_c_tools/tools/plane/tools/plane_labels.py`

**plane_add_label_to_issue and plane_remove_label_from_issue:**
- Add project_id parameter to schema
- Pass to get_issue and update_issue calls

---

### Fix 4: Add Sequential ID Lookup

**File:** `//project/src/agent_c_tools/src/agent_c_tools/tools/plane/client/plane_client.py`

**New method:**
```python
async def get_issue_by_identifier(self, project_id: str, identifier: str) -> Dict[str, Any]:
    """
    Get issue by sequential identifier (e.g., 'AC-9')
    
    Lists all issues and finds one matching the sequence_id
    """
    issues = await self.list_issues(project_id)
    
    # Handle dict response with 'results'
    if isinstance(issues, dict) and 'results' in issues:
        issues = issues['results']
    
    for issue in issues:
        # Try different ID formats: 'AC-9', '9', or just the number
        issue_identifier = f"{issue.get('project_detail', {}).get('identifier', '')}-{issue.get('sequence_id', '')}"
        if (issue_identifier == identifier or 
            str(issue.get('sequence_id')) == identifier.split('-')[-1]):
            return issue
    
    raise PlaneAPIError(f"Issue '{identifier}' not found in project")
```

**File:** `//project/src/agent_c_tools/src/agent_c_tools/tools/plane/tool.py`

**New tool:**
```python
@json_schema(
    description="Get PLANE issue by sequential identifier like 'AC-9' instead of UUID",
    params={
        "project_id": {"type": "string", "description": "Project ID", "required": True},
        "identifier": {"type": "string", "description": "Issue identifier (e.g., 'AC-9' or just '9')", "required": True}
    }
)
async def plane_get_issue_by_identifier(self, **kwargs) -> str:
    """Get issue by AC-9 style identifier"""
    # Implementation delegates to helper method
```

---

## 📋 Task Delegation

### Clone Task 1: Fix plane_client.py endpoints
- Update get_issue, update_issue, delete_issue signatures
- Make project_id required
- Remove workspace-level fallback paths
- Add get_issue_by_identifier method

### Clone Task 2: Update plane_issues.py
- Add project_id to get/update tool schemas
- Pass project_id to client methods
- Update error messages

### Clone Task 3: Update plane_labels.py
- Add project_id parameter where needed
- Pass to get_issue/update_issue calls

### Clone Task 4: Add sequential ID tool
- Add new method to PlaneTools in tool.py
- Test with AC-9 format

### Pyper Review:
- Review all changes
- Verify no regressions
- Confirm fixes are correct

---

## 🧪 Testing Plan

After fixes:
1. Test get_issue with UUID (should work)
2. Test update_issue with UUID (should work)
3. Test move between states (should work)
4. Test get by AC-9 identifier (should work)

---

**Estimated Time:** 60 minutes  
**Confidence:** High - root cause is clear and fix is straightforward

Ready to execute?

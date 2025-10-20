# PLANE API Errors - Analysis & Fix Plan

**Source:** //project/rupert/agent_c/plane_errors_for_tim.md  
**Date:** 2025-10-16  
**Status:** Errors identified, fixes planned

---

## 🔍 Root Cause Analysis

All errors stem from **incorrect API endpoint paths**:

### Current (WRONG):
```
GET /api/workspaces/{workspace_slug}/issues/{issue_id}/
PATCH /api/workspaces/{workspace_slug}/issues/{issue_id}/
```

### Should Be (CORRECT):
```
GET /api/workspaces/{workspace_slug}/projects/{project_id}/issues/{issue_id}/
PATCH /api/workspaces/{workspace_slug}/projects/{project_id}/issues/{issue_id}/
```

**PLANE requires project_id in the path for issue operations!**

---

## 🐛 Specific Errors & Fixes

### Error 1 & 2: Get/Update Issue Returns 404

**Problem:** Issue endpoints use workspace-level path  
**Fix:** Always use project-scoped endpoints for issues

**In plane_client.py:**

```python
# CURRENT (WRONG):
async def get_issue(self, issue_id: str, project_id: Optional[str] = None):
    if project_id:
        endpoint = f'/workspaces/{ws}/projects/{project_id}/issues/{issue_id}/'
    else:
        endpoint = f'/workspaces/{ws}/issues/{issue_id}/'  # <-- This fails!

# FIXED:
async def get_issue(self, issue_id: str, project_id: str):  # Make project_id REQUIRED
    endpoint = f'/workspaces/{ws}/projects/{project_id}/issues/{issue_id}/'
```

**Impact:** Affects get_issue() and update_issue()

---

### Error 3: Cannot Move Between States

**Problem:** Same as Error 2 - update_issue uses wrong endpoint  
**Fix:** Include in update_issue fix above

---

### Error 4: UUID vs Sequential ID (AC-9)

**Problem:** Users see "AC-9" but API uses UUIDs  
**Solution:** Add lookup method to translate AC-9 → UUID

**New method needed:**
```python
async def get_issue_by_identifier(self, project_id: str, identifier: str) -> Dict:
    """
    Get issue by sequential identifier (e.g., 'AC-9')
    
    Args:
        project_id: Project ID
        identifier: Sequential ID like 'AC-9'
    
    Returns:
        Issue data with UUID
    """
    # List all issues and find by sequence_id
    issues = await self.list_issues(project_id)
    for issue in issues:
        if issue.get('sequence_id') == identifier or issue.get('project_detail', {}).get('identifier') + '-' + str(issue.get('sequence_id')) == identifier:
            return issue
    raise PlaneAPIError(f"Issue {identifier} not found")
```

---

## 🎯 Fix Implementation Plan

### Phase 1: Fix Issue Endpoints (30 min)

**Files to modify:**
1. `client/plane_client.py`:
   - Make `project_id` REQUIRED in `get_issue()`
   - Make `project_id` REQUIRED in `update_issue()`
   - Use project-scoped endpoints always

2. Update all toolsets to pass `project_id`:
   - `plane_issues.py` - get/update methods need project_id
   - `plane_issue_relations.py` - already has project_id ✅
   - `plane_labels.py` - get_issue calls need project_id

### Phase 2: Add Sequential ID Lookup (20 min)

**Add to plane_client.py:**
- `get_issue_by_identifier(project_id, identifier)` method

**Add to PlaneTools:**
- New tool: `plane_get_issue_by_id(project_id, issue_identifier)`

### Phase 3: Update Tool Schemas (10 min)

**Update JSON schemas** to reflect:
- project_id now required for get_issue
- New tool available for AC-9 style lookups

### Phase 4: Test Everything (30 min)

**Test each fix:**
- Get issue with project_id
- Update issue with project_id  
- Move issue between states
- Look up by AC-9 identifier

---

## 👥 Team Collaboration Plan

### Tim (Me):
- Fix the endpoint paths
- Add sequential ID lookup
- Update schemas

### Clone #1:
- Verify all endpoint paths are correct
- Check PLANE API docs for proper structure

### Clone #2:
- Test the fixes with mock data
- Verify error handling

### Pyper (Code Quality):
- Final review of changes
- Verify no regressions

---

## ⏱️ Timeline

- Phase 1: 30 min
- Phase 2: 20 min  
- Phase 3: 10 min
- Phase 4: 30 min
- **Total: 90 minutes**

---

## ✅ Success Criteria

After fixes:
- ✅ Can get issue details using UUID
- ✅ Can update existing issues
- ✅ Can move issues between states
- ✅ Can look up issues by AC-9 style identifiers
- ✅ All without 404 errors

---

**Ready to proceed with fixes?**

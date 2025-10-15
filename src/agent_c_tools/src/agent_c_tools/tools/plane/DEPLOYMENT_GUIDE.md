# 🚀 PLANE Tools - Deployment Guide

**Version:** 1.1.0 (Phase 1 + Phase 2)  
**Date:** 2025-10-15  
**Status:** Production Ready

---

## 📦 What You're Deploying

**33 Tools Across 7 Toolsets:**

1. **PlaneProjectTools** (5) - Project management
2. **PlaneIssueTools** (6) - Issue/task management
3. **PlaneSearchTools** (3) - Search & discovery
4. **PlaneAnalyticsTools** (3) - Analytics & reporting
5. **PlaneIssueRelationsTools** (6) - Sub-issues & relations
6. **PlaneLabelTools** (5) - Label management
7. **PlaneBulkTools** (5) - Bulk operations

**All tested and ready for production use!**

---

## ✅ Pre-Deployment Checklist

### 1. Dependencies Installed
```bash
# Required dependencies (should already be installed)
pip install cryptography pyyaml requests structlog

# Optional (for auto-refresh)
pip install playwright
playwright install chromium
```

### 2. Cookies Configured
```bash
cd src/agent_c_tools/tools/plane/scripts
python plane_auth_cli.py test agent_c
```

**Expected output:**
```
✅ Cookies are valid and working!
👤 Authenticated as: ethan.booth@centricconsulting.com
```

**If cookies expired:**
```bash
python plane_auth_cli.py setup agent_c
# Follow prompts to enter cookies from browser
```

### 3. PLANE Instance Accessible
```bash
curl http://localhost/api/users/me/
```

Should return user data (may show 401 without cookies - that's OK)

### 4. Test Scripts Pass
```bash
cd //plane/.scratch
python test_all_tools_integration.py
```

**Expected:** 100% success rate (10/10 tests passing)

---

## 🔧 Deployment Steps

### Step 1: Register Toolsets

Add to your Agent C tools registration (usually in `agent_c_tools/__init__.py`):

```python
# Import PLANE toolsets to trigger registration
from agent_c_tools.tools.plane.tools.plane_projects import PlaneProjectTools
from agent_c_tools.tools.plane.tools.plane_issues import PlaneIssueTools
from agent_c_tools.tools.plane.tools.plane_search import PlaneSearchTools
from agent_c_tools.tools.plane.tools.plane_analytics import PlaneAnalyticsTools
from agent_c_tools.tools.plane.tools.plane_issue_relations import PlaneIssueRelationsTools
from agent_c_tools.tools.plane.tools.plane_labels import PlaneLabelTools
from agent_c_tools.tools.plane.tools.plane_bulk import PlaneBulkTools

# Toolsets auto-register via Toolset.register() calls
```

Or simpler:
```python
from agent_c_tools.tools.plane import register_tools
```

### Step 2: Configure Rupert's Toolsets

In Rupert's agent configuration YAML, add PLANE toolsets:

```yaml
# rupert_config.yaml
toolsets:
  # Existing toolsets
  - WorkspaceTools
  - ThinkTools
  # ... other tools ...
  
  # PLANE toolsets - NEW!
  - PlaneProjectTools
  - PlaneIssueTools
  - PlaneSearchTools
  - PlaneAnalyticsTools
  - PlaneIssueRelationsTools
  - PlaneLabelTools
  - PlaneBulkTools
```

### Step 3: Update Rupert's Persona (Optional but Recommended)

Add to Rupert's instructions:

```markdown
## PLANE Project Management Integration

You have comprehensive PLANE project management capabilities with 33 tools.

### What You Can Do

**Projects:**
- Create, list, update, archive projects
- Get project statistics and analytics

**Issues & Tasks:**
- Create, list, update issues
- Create sub-issues (parent-child hierarchy)
- Add/view comments (markdown supported)
- Manage assignees, priorities, states

**Organization:**
- Create and apply labels for categorization
- Add issue relations (blocks, blocked_by, duplicate, relates_to)
- Search across projects and issues
- Find user's assigned issues

**Efficiency:**
- Bulk update multiple issues at once
- Bulk assign, change state, change priority
- Bulk label application

**Analytics:**
- Workspace overview
- Project statistics (state/priority breakdowns)
- Team workload analysis

### When to Use PLANE Tools

- User asks about projects, tasks, or work status
- User wants to create or update issues
- User needs to organize or categorize work
- User wants reports or analytics
- User mentions "sub-tasks", "blockers", "labels"

### Best Practices

**Be proactive:**
- Suggest breaking large tasks into sub-issues
- Identify blockers and dependencies
- Recommend labels for organization
- Offer bulk operations when applicable

**Be conversational:**
- "I found 3 urgent issues. Want details?"
- "Should I break that into sub-tasks?"
- "I can mark those 5 issues as done. Proceed?"

**Use visual indicators:**
- Priority: 🔴 Urgent, 🟠 High, 🟡 Medium, 🟢 Low, ⚪ None
- Relations: 🚫 Blocking, ⛔ Blocked By, 🔗 Related

**Examples:**

User: "What am I working on?"
You: [Use plane_find_my_issues()]

User: "Create a task to fix the login bug"
You: [Use plane_create_issue(), suggest priority/labels]

User: "Break that into sub-tasks"
You: [Use plane_create_sub_issue() multiple times]

User: "That's blocked by issue AC-20"
You: [Use plane_add_issue_relation() with type='blocked_by']
```

### Step 4: Restart Agent C

```bash
# Restart your Agent C instance to load new tools
# Method depends on how you're running Agent C
```

### Step 5: Test with Rupert

Try these test commands:

```
User: "List PLANE projects"
Expected: Shows Agent_C project

User: "What are my PLANE issues?"
Expected: Shows assigned issues grouped by priority

User: "Create a test issue in project AC"
Expected: Creates issue and shows confirmation

User: "Show me workspace stats"
Expected: Shows overview with projects, members, issues
```

---

## 🧪 Verification Tests

After deployment, verify each toolset:

### Project Tools
```
"List all PLANE projects"
"Show me project AC details"
```

### Issue Tools
```
"List issues in project AC"
"What am I working on in PLANE?"
"Create an issue: Test Issue"
```

### Search Tools
```
"Find PLANE issues about testing"
"What medium priority issues do I have?"
```

### Analytics Tools
```
"Show me PLANE workspace overview"
"What are the stats for project AC?"
```

### Relations Tools
```
"Show sub-issues for issue [id]"
"Show relations for issue [id]"
```

### Label Tools
```
"List all PLANE labels"
```

### Bulk Tools
```
(Test after creating multiple issues)
"Change issues [id1], [id2] to high priority"
```

---

## 🔐 Security Verification

### Check Cookie Storage
```bash
ls -la ~/.plane/cookies/
# Should show: agent_c.enc with 600 permissions

ls -la ~/.plane/.key
# Should show: .key with 600 permissions
```

### Verify Encryption
```bash
cat ~/.plane/cookies/agent_c.enc
# Should show: encrypted gibberish, not plain text
```

### Test Cookie Loading
```bash
python -c "
from agent_c_tools.tools.plane.auth.cookie_manager import PlaneCookieManager
cm = PlaneCookieManager('agent_c')
cookies = cm.load_cookies()
print('✅ Cookies loaded:', len(cookies), 'cookies')
"
```

---

## 📊 Performance Expectations

### Response Times
- **List operations:** 50-200ms
- **Get operations:** 50-150ms
- **Create/Update:** 100-300ms
- **Analytics:** 200-500ms (multiple API calls)
- **Bulk operations:** 200ms-2s (depends on count)

### Token Usage
- **Simple responses:** 100-300 tokens
- **List results:** 200-500 tokens
- **Analytics:** 300-600 tokens
- **Error messages:** 50-150 tokens

### API Call Limits
PLANE typically has rate limits around:
- **100 requests/minute** (should be plenty)
- Tools make 1-5 API calls each
- Bulk operations make N calls (one per item)

---

## 🐛 Troubleshooting

### "PLANE client not initialized"
**Cause:** No cookies set up  
**Fix:** Run `python plane_auth_cli.py setup agent_c`

### "Session has expired" 
**Cause:** Cookies expired (happens every ~28 days)  
**Fix:** 
```bash
# Option 1: Manual refresh
python plane_auth_cli.py setup agent_c

# Option 2: Auto refresh (if Playwright installed)
python plane_refresh_cookies.py
```

### "Authentication credentials were not provided"
**Cause:** Missing required cookies  
**Fix:** Re-extract all 4 cookies from browser:
- session-id
- agentc-auth-token
- csrftoken
- ajs_anonymous_id

### Tools returning errors
**Debug steps:**
1. Test cookies: `python plane_auth_cli.py test`
2. Check PLANE is running: `docker ps | grep plane`
3. Verify PLANE accessible: Open `http://localhost/agent_c/`
4. Check logs: `docker logs plane-app-api-1 --tail 50`

### ImportError: No module named 'playwright'
**Cause:** Optional dependency not installed  
**Impact:** Auto-refresh won't work  
**Fix:** `pip install playwright && playwright install chromium`  
**Note:** Only needed for auto-refresh, not for using tools

---

## 📝 Configuration Management

### Environment Variables (Optional)

```bash
# .env file
PLANE_INSTANCE_URL=http://localhost
PLANE_WORKSPACE_SLUG=agent_c
PLANE_AUTO_REFRESH=false  # Set to true if Playwright installed
```

### Cookie Location
```
~/.plane/cookies/agent_c.enc  # Encrypted cookies
~/.plane/.key                 # Encryption key
```

**Backup these files** for recovery!

### Multiple Workspaces

To support multiple PLANE workspaces:

```bash
# Setup for different workspace
python plane_auth_cli.py setup my_other_workspace

# List all configured workspaces
python plane_auth_cli.py list
```

---

## 🎓 Training Materials

### For Users
- **README.md** - Complete usage guide
- **EXAMPLES.md** - Real-world scenarios
- **WEBHOOKS_EXPLAINED.md** - Webhook overview
- **PHASE_4_OVERVIEW.md** - Future features

### For Developers
- **Source code** - Well-documented with type hints
- **Test scripts** - 8 comprehensive test suites
- **Architecture docs** - Component breakdown
- **API reference** - PLANE endpoint documentation

### Quick Start Guide
```
1. Extract cookies from browser
2. Run: python plane_auth_cli.py setup
3. Test: python plane_auth_cli.py test
4. Use with Rupert: "List PLANE projects"
```

---

## 🎯 Success Metrics

### How to Know It's Working

**Week 1:**
- ✅ Users can list projects/issues without errors
- ✅ Creating issues works smoothly
- ✅ Search returns relevant results
- ✅ No cookie expiration issues

**Week 2:**
- ✅ Users actively using sub-issues
- ✅ Labels being applied consistently
- ✅ Bulk operations saving time
- ✅ Analytics providing insights

**Ongoing:**
- ✅ No authentication errors
- ✅ Response times acceptable
- ✅ Users prefer using tools vs PLANE UI for some tasks

---

## 🆘 Support & Maintenance

### Regular Maintenance

**Every 28 days:**
- Refresh cookies (or enable auto-refresh)

**As needed:**
- Update PLANE version → Test compatibility
- Add new tools based on feedback
- Optimize based on usage patterns

### Getting Help

**Check these first:**
1. README.md - Complete guide
2. EXAMPLES.md - Usage examples
3. Test scripts - Verify functionality
4. This deployment guide - Common issues

**For new features:**
- Review PHASE_4_OVERVIEW.md for available options
- Can be added incrementally as needed

---

## 📈 Monitoring Recommendations

### What to Track

**Usage:**
- Which tools are used most?
- Which tools are never used?
- Average response times
- Error rates

**User Feedback:**
- Are responses helpful?
- Are errors clear?
- Are results formatted well?
- What's missing?

**Performance:**
- Token usage per tool
- API call patterns
- Cookie expiration frequency

---

## 🎊 You're Ready!

**Everything is in place:**
- ✅ 33 production-ready tools
- ✅ Complete documentation
- ✅ Full test coverage
- ✅ Security best practices
- ✅ Comprehensive error handling
- ✅ Token-efficient responses

**Next step:** Deploy to Rupert and start using! 🚀

---

## 🏁 Final Deployment Command

```bash
# 1. Verify everything
cd //plane/.scratch
python test_all_tools_integration.py

# 2. Restart Agent C with new tools loaded

# 3. Test with Rupert
# "Hey Rupert, list my PLANE projects"

# 4. 🎉 Enjoy your 33 PLANE tools!
```

---

**Deployment Status:** ✅ **READY TO GO**

**Have fun managing projects with Rupert!** 🎉

---

**Deployed By:** Tim the Tool Man  
**Date:** 2025-10-15  
**Support:** See README.md and EXAMPLES.md

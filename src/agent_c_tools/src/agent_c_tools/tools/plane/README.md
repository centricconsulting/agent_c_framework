# PLANE Tools for Agent C

Comprehensive toolsets for managing PLANE project management system within the Agent C framework.

## 🎯 Features

**5 Core Use Cases Covered:**
1. ✅ Create & manage projects
2. ✅ Create & track issues/tasks
3. ✅ Search across issues and projects
4. ✅ Update issues
5. ✅ Get analytics and reports

**17 Tools Across 4 Toolsets:**
- Project Management (5 tools)
- Issue Management (6 tools)
- Search (3 tools)
- Analytics (3 tools)

**Built for:**
- 🤖 Natural language interaction via Rupert
- 🔒 Secure cookie-based authentication
- ⚡ Token-efficient responses
- 🎨 Beautiful formatting with emojis
- 🛡️ Comprehensive error handling

---

## 🚀 Quick Start

### 1. Install Dependencies

```bash
pip install cryptography pyyaml requests
```

### 2. Set Up Authentication

Run the cookie setup CLI:

```bash
cd src/agent_c_tools/tools/plane/scripts
python plane_auth_cli.py setup
```

Follow the prompts to extract cookies from your browser:
1. Open PLANE at `http://localhost/agent_c/`
2. F12 → Application → Cookies → `http://localhost`
3. Copy these cookie values:
   - `session-id`
   - `agentc-auth-token`
   - `csrftoken`
   - `ajs_anonymous_id` (optional)

Cookies are encrypted and saved to `~/.plane/cookies/agent_c.enc`

### 3. Use the Tools

The toolsets are automatically registered with Agent C. Once configured, Rupert can use commands like:

```
"List all projects in PLANE"
"Create a new issue called 'Update homepage' in project AC"
"What issues are assigned to me?"
"Show me workspace analytics"
```

---

## 📚 Toolset Reference

### PlaneProjectTools

Manage PLANE projects.

#### Tools:

**`plane_list_projects()`**
- Lists all projects in workspace
- Shows: Name, ID, identifier, description, status
- Example: "List all PLANE projects"

**`plane_get_project(project_id)`**
- Get detailed project information
- Returns: Full project details in YAML
- Example: "Get details for project dad9fe27..."

**`plane_create_project(name, identifier, description)`**
- Create a new project
- Validates identifier format (2-5 uppercase letters)
- Example: "Create a project called 'Website Redesign' with identifier WEB"

**`plane_update_project(project_id, name, description)`**
- Update project attributes
- Example: "Update project AC description to 'Core agent components'"

**`plane_archive_project(project_id)`**
- Archive a project
- Example: "Archive project dad9fe27..."

---

### PlaneIssueTools

Manage issues and tasks.

#### Tools:

**`plane_list_issues(project_id, state, priority, assignee)`**
- List issues with optional filters
- Returns: Formatted list with ID, state, priority
- Example: "List all high priority issues in project AC"

**`plane_get_issue(issue_id)`**
- Get detailed issue information
- Returns: Full issue details in YAML
- Example: "Get details for issue ab1581be..."

**`plane_create_issue(project_id, name, description, priority, assignee_ids)`**
- Create a new issue/task
- Supports: Title, description, priority, assignees
- Example: "Create a high priority issue called 'Fix login bug' in project AC"

**`plane_update_issue(issue_id, name, description, state_id, priority, assignee_ids)`**
- Update issue attributes
- Example: "Update issue ab1581be to high priority"

**`plane_add_comment(issue_id, comment)`**
- Add comment to an issue
- Supports markdown formatting
- Example: "Add comment 'Working on this now' to issue ab1581be"

**`plane_get_comments(issue_id)`**
- Get all comments for an issue
- Shows: Author, date, comment text
- Example: "Show comments for issue ab1581be"

---

### PlaneSearchTools

Search and discover content.

#### Tools:

**`plane_search_global(query, search_type)`**
- Search across entire workspace
- search_type: 'all', 'projects', or 'issues'
- Example: "Search for 'authentication' in PLANE"

**`plane_search_issues(project_id, query, state, priority, assignee)`**
- Advanced issue search with filters
- Multiple filter options
- Example: "Find all medium priority issues in project AC"

**`plane_find_my_issues(state, priority)`**
- Find issues assigned to current user
- Grouped by priority with emoji indicators
- Example: "What are my issues?" or "Show me my urgent tasks"

---

### PlaneAnalyticsTools

Get insights and reports.

#### Tools:

**`plane_get_workspace_overview()`**
- Workspace-level statistics
- Shows: Projects, members, total issues
- Example: "Show me workspace overview"

**`plane_get_project_stats(project_id)`**
- Project-level analytics
- Breakdown by state and priority
- Example: "Give me stats for project AC"

**`plane_get_team_workload(project_id)`**
- Team workload analysis
- Shows who's working on what
- Optional project scope
- Example: "Show team workload" or "Who's working on what in project AC?"

---

## 🔧 Configuration

### Environment Variables (Optional)

```bash
PLANE_INSTANCE_URL=http://localhost
PLANE_WORKSPACE_SLUG=agent_c
```

### Cookie Storage

Cookies are stored at: `~/.plane/cookies/{workspace_slug}.enc`

**Managing Cookies:**

```bash
# Setup new cookies
python plane_auth_cli.py setup

# Test cookies
python plane_auth_cli.py test

# Show current cookies (masked)
python plane_auth_cli.py show

# List all workspaces
python plane_auth_cli.py list

# Clear cookies
python plane_auth_cli.py clear
```

### When Cookies Expire

When you see: `ERROR: PLANE session has expired`, refresh your cookies:

1. Open PLANE in browser
2. Extract fresh cookies from DevTools
3. Run: `python plane_auth_cli.py setup`
4. Paste new cookie values

---

## 🏗️ Architecture

### Cookie-Based Authentication

PLANE requires 4 session cookies for workspace access:
- `session-id` - Django session (PRIMARY)
- `agentc-auth-token` - JWT token
- `csrftoken` - CSRF protection
- `ajs_anonymous_id` - Analytics (optional)

**Why not username/password?**
- PLANE instance uses custom/SSO authentication
- No standard login endpoint available
- Cookie-based approach proven and working

### Request Flow

```
Agent/User Request
      ↓
PlaneTools (Toolset)
      ↓
PlaneClient (API wrapper)
      ↓
PlaneSession (HTTP + cookies)
      ↓
CookieManager (Storage)
      ↓
PLANE API
```

---

## 🧪 Testing

All tools have been tested against a live PLANE instance:

- Instance: `http://localhost`
- Workspace: `agent_c`
- Project: `Agent_C` (ID: dad9fe27-de38-4dd6-865f-0455e426339a)
- User: ethan.booth@centricconsulting.com

Test scripts available in `//plane/.scratch/`:
- `test_cookie_manager.py` - Cookie encryption/storage
- `test_plane_client.py` - Core API client
- `test_project_tools.py` - Project management tools
- `test_issue_tools.py` - Issue management tools
- `test_search_tools.py` - Search tools
- `test_analytics_tools.py` - Analytics tools

---

## 📝 Example Usage

### Via Python

```python
from plane import PlaneProjectTools

# Initialize toolset
tools = PlaneProjectTools()
await tools.post_init()

# Use tools
projects = await tools.plane_list_projects()
print(projects)
```

### Via Agent (Rupert)

```
User: "Show me all PLANE projects"
Rupert: [calls plane_list_projects()]

Found 1 project(s):

Agent_C (AC)
  ID: dad9fe27-de38-4dd6-865f-0455e426339a
  Description: No description
  Status: Active
```

```
User: "What am I working on in PLANE?"
Rupert: [calls plane_find_my_issues()]

You have 1 issue(s) assigned:

🟡 MEDIUM (1):
• Agent component ref - phase 1 (ID: ab1581be-3cc1-4e7f-999b-c396e7ece4e9)
```

---

## 🔐 Security

### Cookie Storage
- Encrypted using Fernet (AES-128)
- Stored in `~/.plane/` with 700 permissions
- Key stored separately with 600 permissions
- Per-workspace encryption

### Best Practices
- Never log cookie values
- Rotate cookies regularly
- Use restrictive file permissions
- Clear cookies when done

---

## 🐛 Troubleshooting

### "PLANE client not initialized"
- Cookies not set up
- Run: `python plane_auth_cli.py setup`

### "Session has expired"
- Cookies expired (typically after 24-48 hours)
- Extract fresh cookies from browser
- Run: `python plane_auth_cli.py setup` again

### "Authentication credentials were not provided"
- Missing required cookies
- Verify all 4 cookies are present
- Check cookie values are correct

### Connection errors
- Verify PLANE is running: `docker ps | grep plane`
- Check PLANE is accessible: `http://localhost`
- Verify workspace slug is correct

---

## 📈 Future Enhancements

Potential additions based on user feedback:
- Automated login flow (if auth endpoint discovered)
- Webhook support for real-time updates
- Bulk operations (update multiple issues)
- Label management
- Cycle/sprint management
- Time tracking
- File attachments
- Custom fields
- Advanced reporting

---

## 📄 License

Part of Agent C Tools ecosystem.

---

## 👥 Credits

**Built by:** Tim the Tool Man  
**Date:** 2025-10-15  
**Version:** 1.0.0  

**Tested with:**
- PLANE v1.0.0 (Docker)
- Agent C Framework
- Python 3.12+

---

## 🆘 Support

For issues or questions:
1. Check troubleshooting section above
2. Review test scripts in `//plane/.scratch/`
3. Check PLANE API docs: https://developers.plane.so/
4. Check cookie setup: `python plane_auth_cli.py test`

---

**Status:** ✅ Production Ready  
**Last Updated:** 2025-10-15

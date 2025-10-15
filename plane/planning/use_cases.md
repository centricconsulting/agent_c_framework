# PLANE Integration - Use Cases

## Overview
This document defines the 5 core use cases for PLANE integration with Rupert. These represent the initial feature set, with additional capabilities to be added based on user needs.

**Last Updated:** 2025-10-15
**Status:** Draft - Pending User Approval

---

## Use Case 1: Create & Manage Projects

### Priority: HIGH
### Frequency: Weekly

### Description
Rupert needs to create new projects in PLANE and manage existing project settings. This includes setting up project structure, updating project details, and organizing projects within the workspace.

### Required PLANE Endpoints
- `POST /api/v1/workspaces/{workspace_slug}/projects/` - Create project
- `GET /api/v1/workspaces/{workspace_slug}/projects/` - List projects
- `GET /api/v1/workspaces/{workspace_slug}/projects/{project_id}/` - Get project details
- `PATCH /api/v1/workspaces/{workspace_slug}/projects/{project_id}/` - Update project
- `DELETE /api/v1/workspaces/{workspace_slug}/projects/{project_id}/` - Archive/delete project

### Required Tools
- `plane_create_project`
- `plane_list_projects`
- `plane_get_project`
- `plane_update_project`
- `plane_archive_project`

### Example Interactions
```
User: "Create a new project called 'Website Redesign' with high priority"
Rupert: "I've created the 'Website Redesign' project with high priority. 
         The project ID is WR-001. Would you like me to add any initial 
         tasks or team members?"

User: "Show me all active projects"
Rupert: "You have 3 active projects:
         1. Website Redesign (WR) - High priority, 12 open issues
         2. Mobile App (MA) - Medium priority, 8 open issues  
         3. Documentation (DOC) - Low priority, 3 open issues"
```

### Success Criteria
- Can create projects with all key attributes (name, description, priority, etc.)
- Can list projects with filtering (by status, priority, etc.)
- Can retrieve full project details
- Can update any project attribute
- Can archive completed projects
- Responses are token-efficient and conversational

---

## Use Case 2: Create & Track Issues/Tasks

### Priority: HIGH
### Frequency: Daily

### Description
Rupert needs to create new issues/tasks, track their status, assign them to team members, and manage relationships between issues. This is the primary workflow for day-to-day project management.

### Required PLANE Endpoints
- `POST /api/v1/workspaces/{workspace_slug}/projects/{project_id}/issues/` - Create issue
- `GET /api/v1/workspaces/{workspace_slug}/projects/{project_id}/issues/` - List issues
- `GET /api/v1/workspaces/{workspace_slug}/issues/{issue_id}/` - Get issue details
- `PATCH /api/v1/workspaces/{workspace_slug}/issues/{issue_id}/` - Update issue
- `POST /api/v1/workspaces/{workspace_slug}/issues/{issue_id}/comments/` - Add comment
- `GET /api/v1/workspaces/{workspace_slug}/issues/{issue_id}/comments/` - Get comments

### Required Tools
- `plane_create_issue`
- `plane_list_issues`
- `plane_get_issue`
- `plane_assign_issue`
- `plane_add_comment`
- `plane_get_comments`
- `plane_link_issues`

### Example Interactions
```
User: "Create a task to update the homepage design"
Rupert: "I've created issue WR-101: 'Update homepage design' in the 
         Website Redesign project. Status: To Do. Would you like me 
         to assign it to someone or set a due date?"

User: "What tasks are assigned to me?"
Rupert: "You have 5 tasks assigned:
         
         DUE TODAY:
         - WR-101: Update homepage design (High)
         
         DUE THIS WEEK:
         - MA-45: Fix login bug (High)
         - DOC-12: Review API docs (Medium)
         
         NO DUE DATE:
         - WR-102: Update footer (Low)
         - MA-46: Add dark mode (Medium)"
```

### Success Criteria
- Can create issues with all key attributes
- Can list issues with rich filtering (assignee, status, priority, project, due date)
- Can retrieve full issue details including comments and relationships
- Can assign issues to team members
- Can add comments with proper formatting
- Can link related issues
- Responses are contextual and actionable

---

## Use Case 3: Search Across Issues and Projects

### Priority: HIGH
### Frequency: Daily

### Description
Rupert needs to search across all projects and issues to find relevant information quickly. This includes full-text search, filtered search, and contextual queries.

### Required PLANE Endpoints
- `GET /api/v1/workspaces/{workspace_slug}/search/` - Global search
- `GET /api/v1/workspaces/{workspace_slug}/projects/{project_id}/issues/` - Filtered issue search

### Required Tools
- `plane_search_global`
- `plane_search_issues`
- `plane_find_my_issues`

### Example Interactions
```
User: "Find anything related to authentication"
Rupert: "I found 8 items matching 'authentication':
         
         PROJECTS:
         - User Authentication System (UAS)
         
         ISSUES:
         - MA-45: Fix login bug (In Progress)
         - MA-44: Add OAuth support (To Do)
         - UAS-12: Implement 2FA (In Progress)
         - DOC-15: Document auth flow (To Do)
         
         Would you like details on any of these?"

User: "Show me all high priority bugs"
Rupert: "Found 3 high priority bugs:
         
         1. MA-45: Fix login bug (Assigned to John, In Progress)
            Created 2 days ago, last updated 3 hours ago
         
         2. WR-103: Broken contact form (Unassigned, To Do)
            Created 1 day ago
         
         3. MA-50: Crash on startup (Assigned to Sarah, In Progress)
            Created 5 hours ago
         
         All are actively being worked on. Need me to check status?"
```

### Success Criteria
- Can search across projects and issues simultaneously
- Can filter search by multiple criteria (status, priority, assignee, project, labels)
- Returns relevant, ranked results
- Results are formatted conversationally
- Can find "my" issues quickly
- Search is fast and token-efficient

---

## Use Case 4: Update Issues

### Priority: HIGH
### Frequency: Daily

### Description
Rupert needs to update issue attributes including status, priority, assignee, due dates, descriptions, and custom fields. This should feel natural in conversation.

### Required PLANE Endpoints
- `PATCH /api/v1/workspaces/{workspace_slug}/issues/{issue_id}/` - Update issue
- `POST /api/v1/workspaces/{workspace_slug}/issues/{issue_id}/comments/` - Add comment (for status updates)

### Required Tools
- `plane_update_issue`
- `plane_update_issue_status`
- `plane_update_issue_priority`
- `plane_set_due_date`
- `plane_reassign_issue`

### Example Interactions
```
User: "Mark WR-101 as in progress"
Rupert: "I've updated WR-101: 'Update homepage design' to In Progress.
         It's assigned to you with a due date of Oct 18. Need anything else?"

User: "That task should be high priority and due tomorrow"
Rupert: "Updated WR-101:
         - Priority: Medium → High
         - Due date: Oct 18 → Oct 16 (tomorrow)
         
         This is now your highest priority task due tomorrow. Want me to 
         bump any other tasks?"

User: "Reassign MA-45 to John"
Rupert: "I've reassigned MA-45: 'Fix login bug' from Sarah to John.
         I've added a comment notifying both team members. John now has 
         3 high priority tasks."
```

### Success Criteria
- Can update any issue attribute
- Can update multiple attributes in one conversation turn
- Updates feel natural and conversational
- Provides confirmation with context
- Can infer issue IDs from context when possible
- Automatically adds comments for significant changes
- Notifies affected team members

---

## Use Case 5: Get Analytics and Reports

### Priority: MEDIUM
### Frequency: Weekly

### Description
Rupert needs to provide analytics and reports on project progress, team workload, issue trends, and workspace activity. This helps with planning and status updates.

### Required PLANE Endpoints
- `GET /api/v1/workspaces/{workspace_slug}/analytics/` - Workspace analytics
- `GET /api/v1/workspaces/{workspace_slug}/projects/{project_id}/analytics/` - Project analytics
- `GET /api/v1/workspaces/{workspace_slug}/activity/` - Activity feed

### Required Tools
- `plane_get_workspace_analytics`
- `plane_get_project_analytics`
- `plane_get_team_workload`
- `plane_get_activity_feed`
- `plane_get_issue_trends`

### Example Interactions
```
User: "How's the Website Redesign project going?"
Rupert: "Website Redesign Project Status:
         
         📊 Overall Progress: 68%
         - Completed: 17/25 issues
         - In Progress: 5 issues
         - Blocked: 1 issue (WR-103)
         - To Do: 2 issues
         
         🎯 On Track: Yes (slightly ahead of schedule)
         - Target completion: Oct 31
         - Current pace: Oct 28
         
         ⚠️ Attention Needed:
         - WR-103 has been blocked for 3 days
         
         Need me to dig into any specific area?"

User: "Show me what everyone's working on"
Rupert: "Team Workload Summary:
         
         📈 OVERLOADED (>80% capacity):
         - John: 8 tasks, 3 high priority (Mobile App focus)
         
         ✅ HEALTHY (60-80%):
         - You: 5 tasks, 1 high priority (Website Redesign)
         - Sarah: 6 tasks, 2 high priority (Documentation)
         
         📉 UNDERUTILIZED (<60%):
         - Mike: 2 tasks, 0 high priority
         
         💡 Suggestion: Consider reassigning some of John's tasks to Mike."

User: "What happened in PLANE today?"
Rupert: "Today's Activity (Oct 15):
         
         ✅ Completed: 4 issues
         - MA-42: Add search feature (John)
         - DOC-11: Update README (Sarah)
         - WR-100: Update logo (You)
         - MA-43: Fix dark mode (John)
         
         🆕 Created: 2 new issues
         - WR-105: Mobile responsive fixes (You)
         - MA-51: Performance optimization (John)
         
         💬 Active Discussions: 12 new comments
         - Most activity on MA-45 (login bug)
         
         Great progress today! 🎉"
```

### Success Criteria
- Provides clear, actionable analytics
- Uses visual indicators (emojis, formatting) for quick scanning
- Identifies trends and anomalies
- Provides context and suggestions
- Can generate reports at different scopes (workspace, project, team, individual)
- Analytics are accurate and up-to-date
- Responses are conversational, not just data dumps

---

## Technical Requirements (All Use Cases)

### Authentication
- Support for API key/token authentication
- Secure credential storage
- Token refresh handling (if needed)

### Error Handling
- Clear, helpful error messages
- Graceful degradation when PLANE is unavailable
- Retry logic for transient failures

### Performance
- Response times < 2 seconds average
- Efficient data fetching (only request needed fields)
- Caching strategy for frequently accessed data
- Pagination handling for large result sets

### Token Efficiency
- Tool descriptions optimized for minimal tokens
- Response formatting that's concise but complete
- Avoid redundant data in responses
- Smart summarization for large datasets

### Rate Limiting
- Respect PLANE API rate limits
- Implement backoff strategy
- Queue requests when needed

### Data Consistency
- Properly handle concurrent updates
- Refresh cached data appropriately
- Handle stale data gracefully

---

## Future Enhancements (Not in Initial Scope)

### Medium Priority
- **Cycle/Sprint Management:** Create and manage sprints
- **Label Management:** Create and apply labels/tags
- **Module Management:** Organize work into modules
- **Bulk Operations:** Update multiple issues at once
- **Custom Views:** Save and reuse filtered views
- **Notifications:** Real-time updates via webhooks

### Low Priority
- **Time Tracking:** Log and report time spent
- **File Attachments:** Upload and manage files
- **Advanced Analytics:** Custom reports and dashboards
- **Export Capabilities:** Export data to CSV/PDF
- **Integration Hooks:** Connect with other tools
- **Automation Rules:** Auto-assign, auto-update based on triggers

---

## Configuration Needs

### Required Environment Variables
```bash
PLANE_INSTANCE_URL=http://localhost/agent_c/  # TO BE VERIFIED
PLANE_API_KEY=xxxxx  # TO BE PROVIDED
PLANE_WORKSPACE_SLUG=xxxxx  # TO BE PROVIDED
PLANE_WORKSPACE_ID=xxxxx  # TO BE PROVIDED
```

### Optional Configuration
```yaml
plane:
  timeout: 30  # seconds
  retry_attempts: 3
  rate_limit: 100  # requests per minute
  cache_ttl: 300  # seconds (5 minutes)
  pagination_size: 50
  log_level: INFO
```

---

## Next Steps

1. **User:** Verify PLANE instance URL and provide credentials
2. **User:** Review and approve these use cases
3. **Tim:** Design tool architecture based on approved use cases
4. **Tim:** Build core PLANE client library
5. **Tim:** Implement tools for each use case
6. **Tim:** Create configuration templates
7. **Vera:** Build test suite
8. **Bobb:** Integrate with Rupert

---

**Status:** ⏸️ Awaiting user confirmation of:
- PLANE instance URL (currently: http://localhost/agent_c/)
- Authentication method and credentials
- Use case approval

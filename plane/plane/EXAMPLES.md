# PLANE Tools - Usage Examples

Real-world examples of using PLANE tools with Agent C.

---

## Example 1: Project Setup Workflow

**Scenario:** Starting a new project with initial tasks

```
User: "Create a new PLANE project called 'Website Redesign' with identifier WEB"

Rupert: [calls plane_create_project()]
✅ Project created successfully!

archived_at: null
created_at: '2025-10-15T13:10:00.000000-04:00'
description: ''
id: new-project-id-here
identifier: WEB
name: Website Redesign

User: "Now create 3 tasks in that project: Design mockups, Implement responsive layout, and Testing"

Rupert: [calls plane_create_issue() 3 times]
✅ Created 3 issues successfully:
- WEB-1: Design mockups
- WEB-2: Implement responsive layout
- WEB-3: Testing

All issues created with medium priority.
```

---

## Example 2: Daily Standup

**Scenario:** Quick overview of what's happening

```
User: "What am I working on in PLANE?"

Rupert: [calls plane_find_my_issues()]
You have 1 issue(s) assigned:

🟡 MEDIUM (1):
• Agent component ref - phase 1 (ID: ab1581be-3cc1-4e7f-999b-c396e7ece4e9)

User: "What's the workspace status?"

Rupert: [calls plane_get_workspace_overview()]
Agent_C Workspace Overview

📊 Statistics:
  Projects: 1 total (1 active)
  Members: 1
  Total Issues: 1

📂 Active Projects:
  • Agent_C (AC)
```

---

## Example 3: Task Management

**Scenario:** Creating and updating tasks

```
User: "Create a high priority bug in project AC: Login form validation failing"

Rupert: [calls plane_create_issue()]
✅ Issue created successfully!

**Login form validation failing**
ID: new-issue-id
Project: Agent_C
State: To Do
Priority: high

User: "Assign that to me and add a comment that I'm investigating"

Rupert: [calls plane_update_issue() and plane_add_comment()]
✅ Issue updated - assigned to you
✅ Comment added: "I'm investigating this issue"

User: "Update it to in progress"

Rupert: [calls plane_update_issue()]
✅ Issue status updated to: In Progress
```

---

## Example 4: Search and Discovery

**Scenario:** Finding specific issues

```
User: "Find all urgent issues"

Rupert: [calls plane_search_issues()]
Found 3 issue(s):

• Critical database connection error
  ID: issue-1
  State: In Progress
  Priority: urgent

• Security vulnerability in auth
  ID: issue-2  
  State: To Do
  Priority: urgent

• Production deployment failing
  ID: issue-3
  State: To Do
  Priority: urgent

User: "Show me all issues about authentication"

Rupert: [calls plane_search_global()]
**Issues (2):**

• Security vulnerability in auth
  ID: issue-2
  Project: Agent_C
  State: To Do

• Login form validation failing
  ID: issue-4
  Project: Agent_C
  State: In Progress
```

---

## Example 5: Analytics and Reporting

**Scenario:** Project status check

```
User: "How's the Agent_C project doing?"

Rupert: [calls plane_get_project_stats()]
Agent_C Project Statistics

📊 Overview:
  Total Issues: 12
  Assigned: 8
  Unassigned: 4

📈 By State:
  In Progress: 5
  To Do: 4
  Done: 2
  Blocked: 1

🎯 By Priority:
  🔴 Urgent: 3
  🟠 High: 4
  🟡 Medium: 3
  🟢 Low: 2

User: "Who's working on what?"

Rupert: [calls plane_get_team_workload()]
Team Workload (workspace):

**John Doe** (5 issues)
  🔴 Urgent: 2
  🟠 High: 2
  🟡 Medium: 1

**Jane Smith** (3 issues)
  🔴 Urgent: 1
  🟠 High: 2

**Unassigned** (4 issues)
  🟡 Medium: 2
  🟢 Low: 2
```

---

## Example 6: Issue Lifecycle

**Scenario:** Complete issue workflow

```
User: "Create an issue: Implement dark mode support"

Rupert: [calls plane_create_issue()]
✅ Issue created: AC-15

User: "Assign it to Sarah"

Rupert: [calls plane_update_issue() with Sarah's user_id]
✅ Assigned to Sarah

User: "Set priority to high"

Rupert: [calls plane_update_issue()]
✅ Priority updated to high

User: "Add a comment: Please focus on accessibility in dark mode"

Rupert: [calls plane_add_comment()]
✅ Comment added

User: "Move it to in progress"

Rupert: [calls plane_update_issue() with state_id]
✅ Issue moved to: In Progress

User: "Show me all comments on that issue"

Rupert: [calls plane_get_comments()]
Found 2 comment(s):

**Admin** (2025-10-15T13:00:00)
Please focus on accessibility in dark mode

**Sarah** (2025-10-15T13:15:00)
Starting work on this now. Will ensure WCAG AA compliance.
```

---

## Example 7: Filtering and Prioritization

**Scenario:** Finding specific subsets of work

```
User: "Show me all unassigned high priority issues"

Rupert: [calls plane_search_issues()]
Found 2 issue(s):

• Critical performance issue
  ID: ac-20
  State: To Do
  Priority: high

• API endpoint timeout
  ID: ac-21
  State: To Do
  Priority: high

User: "What medium priority issues does John have?"

Rupert: [calls plane_search_issues() with assignee filter]
Found 3 issue(s):

• Implement caching layer
  ID: ac-18
  State: In Progress
  Priority: medium
  Assignees: John Doe

• Update documentation
  ID: ac-22
  State: To Do
  Priority: medium
  Assignees: John Doe

• Code review for PR #45
  ID: ac-25
  State: To Do
  Priority: medium
  Assignees: John Doe
```

---

## Example 8: Multi-Project Management

**Scenario:** Managing multiple projects

```
User: "List all PLANE projects"

Rupert: [calls plane_list_projects()]
Found 3 project(s):

Agent_C (AC)
  ID: dad9fe27...
  Description: Core agent components
  Status: Active

Website Redesign (WEB)
  ID: 5f3a21b8...
  Description: Company website refresh
  Status: Active

Mobile App (MOB)
  ID: 8c4d92e1...
  Description: iOS and Android apps
  Status: Active

User: "Show me stats for all projects"

Rupert: [calls plane_get_project_stats() for each]
[Shows comprehensive stats for each project...]

User: "What's the overall team workload?"

Rupert: [calls plane_get_team_workload() without project_id]
[Shows workload across all projects...]
```

---

## 🎨 Response Formatting

### Priority Indicators
- 🔴 **Urgent** - Needs immediate attention
- 🟠 **High** - Important, work on soon
- 🟡 **Medium** - Normal priority
- 🟢 **Low** - Nice to have
- ⚪ **None** - No priority set

### Status Indicators
- Common states: To Do, In Progress, Done, Blocked, Cancelled

### Output Formats
- **Lists:** Bullet points with key info
- **Details:** YAML for structured data
- **Analytics:** Grouped by priority/state with counts
- **Search:** Ranked results with context

---

## 🚀 Best Practices

### For Creating Issues
```
✅ Good: "Create a high priority issue: Fix login timeout"
✅ Good: "Add task to AC project: Update API documentation"
❌ Avoid: "Make an issue" (too vague)
```

### For Searching
```
✅ Good: "Find all bugs assigned to me"
✅ Good: "Search for authentication issues"
❌ Avoid: "Search" (needs query)
```

### For Updates
```
✅ Good: "Mark issue AC-15 as done"
✅ Good: "Change priority of AC-20 to urgent"
❌ Avoid: "Update that issue" (needs ID)
```

### For Analytics
```
✅ Good: "How's project AC doing?"
✅ Good: "Show workspace stats"
✅ Good: "Who's overloaded?"
```

---

## 🔮 Advanced Usage

### Combining Tools

The tools can be used together for complex workflows:

```
1. plane_list_projects() → Get all projects
2. For each project:
   - plane_get_project_stats() → Get stats
   - plane_list_issues() → Get issues
3. plane_get_team_workload() → Overall view
4. Provide comprehensive report
```

### Batch Operations

```
User: "Create 5 tasks for the mobile app redesign"

Rupert: [Can call plane_create_issue() multiple times]
```

### Context Awareness

```
User: "Create an issue in the website project"
Rupert: [Uses plane_search_global() or plane_list_projects() to find project ID]
Rupert: [Then calls plane_create_issue() with correct project_id]
```

---

## 📊 Token Efficiency

Tools are designed for minimal token usage:

- **Lists:** Show only essential info (ID, name, state, priority)
- **Long descriptions:** Truncated with "..."
- **Large result sets:** Limited display (top 10-20) with count of remaining
- **Structured data:** YAML format (more compact than JSON)
- **Visual indicators:** Emojis replace text labels
- **Grouping:** Priority-based grouping reduces redundancy

**Example Token Savings:**
```
❌ Verbose: "This issue has a priority of medium and is currently in the state of To Do"
✅ Efficient: "🟡 Medium | To Do"
```

---

## 🎯 Integration with Rupert

### Recommended Persona Updates

Add to Rupert's instructions:

```markdown
## PLANE Integration

You have access to PLANE project management tools for:
- Managing projects and tasks
- Tracking work progress  
- Finding and organizing issues
- Getting analytics and reports

When users ask about tasks, issues, or project status, use PLANE tools to:
1. Check current state
2. Create or update items
3. Provide status reports
4. Help prioritize work

Be conversational and proactive. For example:
- "I see you have 3 urgent issues. Want me to show them?"
- "I've created that task in PLANE. Should I assign it to someone?"
```

### Tool Selection Guide

```
User mentions...           → Use tool...
"create project"          → plane_create_project()
"what projects"           → plane_list_projects()
"create task/issue"       → plane_create_issue()
"my tasks"                → plane_find_my_issues()
"find/search"             → plane_search_issues() or plane_search_global()
"update/change issue"     → plane_update_issue()
"how's project X"         → plane_get_project_stats()
"who's working on"        → plane_get_team_workload()
"workspace status"        → plane_get_workspace_overview()
```

---

**Ready for Production Use!** 🚀

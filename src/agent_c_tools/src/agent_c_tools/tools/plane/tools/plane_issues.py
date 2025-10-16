"""
PLANE Issue Management Toolset

Tools for creating and managing PLANE issues/tasks.
"""

import yaml
from typing import Optional
from agent_c.toolsets import Toolset, json_schema
from ..client.plane_client import PlaneClient, PlaneAPIError
from ..auth.plane_session import PlaneSessionExpired


class PlaneIssueTools(Toolset):
    """Tools for managing PLANE issues and tasks"""
    
    def __init__(self, **kwargs):
        super().__init__(**kwargs, name='plane_issues', use_prefix=True)
        self.client: Optional[PlaneClient] = None
        self.base_url = "http://localhost"
        self.workspace_slug = "agent_c"
    
    async def post_init(self):
        """Initialize PLANE client after toolset creation"""
        try:
            self.client = PlaneClient(self.base_url, self.workspace_slug)
            self.logger.info(f"PLANE issue tools initialized for workspace: {self.workspace_slug}")
        except Exception as e:
            self.logger.error(f"Failed to initialize PLANE client: {str(e)}")
            self.client = None
    
    def _ensure_client(self):
        """Ensure client is initialized"""
        if self.client is None:
            raise PlaneAPIError("PLANE client not initialized. Check authentication.")
    
    def _format_issue(self, issue: dict) -> str:
        """Format issue data for display"""
        result = f"**{issue.get('name', 'Untitled')}**\n"
        result += f"ID: {issue.get('id')}\n"
        result += f"Project: {issue.get('project_detail', {}).get('name', 'N/A')}\n"
        result += f"State: {issue.get('state_detail', {}).get('name', 'N/A')}\n"
        result += f"Priority: {issue.get('priority', 'none')}\n"
        
        if issue.get('assignees'):
            result += f"Assignees: {len(issue.get('assignees', []))}\n"
        
        if issue.get('description'):
            desc = issue.get('description', '')
            if len(desc) > 100:
                desc = desc[:97] + "..."
            result += f"Description: {desc}\n"
        
        return result
    
    def _format_issue_list(self, issues) -> str:
        """Format list of issues for display"""
        # Handle dict response with 'results' key
        if isinstance(issues, dict):
            if 'results' in issues:
                issues = issues['results']
            else:
                return "ERROR: Unexpected response format from API"
        
        if not issues:
            return "No issues found."
        
        # Ensure we have a list
        if not isinstance(issues, list):
            return f"ERROR: Expected list of issues, got {type(issues)}"
        
        result = f"Found {len(issues)} issue(s):\n\n"
        
        # Limit to 20 for token efficiency
        display_issues = issues[:20] if len(issues) > 20 else issues
        
        for issue in display_issues:
            result += f"• **{issue.get('name', 'Untitled')}**\n"
            result += f"  ID: {issue.get('id')}\n"
            result += f"  State: {issue.get('state_detail', {}).get('name', 'N/A')}\n"
            result += f"  Priority: {issue.get('priority', 'none')}\n"
            result += "\n"
        
        if len(issues) > 20:
            result += f"... and {len(issues) - 20} more issues\n"
        
        return result
    
    @json_schema(
        description="List issues in a PLANE project with optional filtering",
        params={
            "project_id": {
                "type": "string",
                "description": "Project ID to list issues from",
                "required": True
            },
            "state": {
                "type": "string",
                "description": "Filter by state (e.g., 'backlog', 'todo', 'in_progress', 'done')"
            },
            "priority": {
                "type": "string",
                "description": "Filter by priority (e.g., 'urgent', 'high', 'medium', 'low', 'none')"
            },
            "assignee": {
                "type": "string",
                "description": "Filter by assignee user ID"
            }
        }
    )
    async def plane_list_issues(self, **kwargs) -> str:
        """
        List issues in a project
        
        Args:
            project_id: Project ID
            state: Optional state filter
            priority: Optional priority filter
            assignee: Optional assignee filter
            
        Returns:
            Formatted list of issues
        """
        self._ensure_client()
        
        project_id = kwargs.get("project_id")
        if not project_id:
            return "ERROR: project_id is required"
        
        # Build filters
        filters = {}
        if "state" in kwargs:
            filters["state"] = kwargs["state"]
        if "priority" in kwargs:
            filters["priority"] = kwargs["priority"]
        if "assignee" in kwargs:
            filters["assignee"] = kwargs["assignee"]
        
        try:
            issues = await self.client.list_issues(project_id=project_id, filters=filters if filters else None)
            return self._format_issue_list(issues)
            
        except PlaneSessionExpired as e:
            return f"ERROR: {str(e)}"
        except PlaneAPIError as e:
            return f"ERROR: Failed to list issues: {str(e)}"
        except Exception as e:
            self.logger.error(f"Unexpected error listing issues: {str(e)}")
            return f"ERROR: Unexpected error: {str(e)}"
    
    @json_schema(
        description="Get detailed information about a specific PLANE issue",
        params={
            "issue_id": {
                "type": "string",
                "description": "The issue ID",
                "required": True
            },
            "project_id": {
                "type": "string",
                "description": "The project ID",
                "required": True
            }
        }
    )
    async def plane_get_issue(self, **kwargs) -> str:
        """
        Get issue details
        
        Args:
            issue_id: Issue ID
            project_id: Optional project ID
            
        Returns:
            Detailed issue information
        """
        self._ensure_client()
        
        issue_id = kwargs.get("issue_id")
        project_id = kwargs.get("project_id")
        
        if not issue_id:
            return "ERROR: issue_id is required"
        if not project_id:
            return "ERROR: project_id is required"
        
        try:
            issue = await self.client.get_issue(issue_id, project_id)
            
            # Format detailed view
            result = yaml.dump({
                'id': issue.get('id'),
                'name': issue.get('name'),
                'description': issue.get('description', 'No description'),
                'state': issue.get('state_detail', {}).get('name'),
                'priority': issue.get('priority'),
                'project': issue.get('project_detail', {}).get('name'),
                'assignees': [a.get('display_name') for a in issue.get('assignees', [])],
                'created_at': issue.get('created_at'),
                'updated_at': issue.get('updated_at'),
            }, allow_unicode=True, default_flow_style=False)
            
            return result
            
        except PlaneSessionExpired as e:
            return f"ERROR: {str(e)}"
        except PlaneAPIError as e:
            return f"ERROR: Failed to get issue: {str(e)}"
        except Exception as e:
            self.logger.error(f"Unexpected error getting issue: {str(e)}")
            return f"ERROR: Unexpected error: {str(e)}"
    
    @json_schema(
        description="Create a new issue/task in a PLANE project",
        params={
            "project_id": {
                "type": "string",
                "description": "Project ID to create issue in",
                "required": True
            },
            "name": {
                "type": "string",
                "description": "Issue title/name",
                "required": True
            },
            "description": {
                "type": "string",
                "description": "Issue description (supports markdown)"
            },
            "priority": {
                "type": "string",
                "description": "Priority: urgent, high, medium, low, or none"
            },
            "assignee_ids": {
                "type": "array",
                "description": "List of user IDs to assign",
                "items": {"type": "string"}
            },
            "parent_id": {
                "type": "string",
                "description": "Parent issue ID to create this as a sub-issue"
            }
        }
    )
    async def plane_create_issue(self, **kwargs) -> str:
        """
        Create a new issue
        
        Args:
            project_id: Project ID
            name: Issue title
            description: Issue description
            priority: Priority level
            assignee_ids: List of assignee user IDs
            
        Returns:
            Created issue details
        """
        self._ensure_client()
        
        project_id = kwargs.get("project_id")
        name = kwargs.get("name")
        
        if not project_id:
            return "ERROR: project_id is required"
        if not name:
            return "ERROR: name is required"
        
        # Build issue data
        issue_data = {
            "name": name,
            "description": kwargs.get("description", ""),
        }
        
        if "priority" in kwargs:
            priority = kwargs["priority"].lower()
            if priority not in ["urgent", "high", "medium", "low", "none"]:
                return "ERROR: priority must be one of: urgent, high, medium, low, none"
            issue_data["priority"] = priority
        
        if "assignee_ids" in kwargs:
            issue_data["assignees"] = kwargs["assignee_ids"]
        
        if "parent_id" in kwargs:
            issue_data["parent"] = kwargs["parent_id"]
        
        try:
            issue = await self.client.create_issue(project_id, issue_data)
            
            result = f"✅ Issue created successfully!\n\n"
            result += self._format_issue(issue)
            return result
            
        except PlaneSessionExpired as e:
            return f"ERROR: {str(e)}"
        except PlaneAPIError as e:
            return f"ERROR: Failed to create issue: {str(e)}"
        except Exception as e:
            self.logger.error(f"Unexpected error creating issue: {str(e)}")
            return f"ERROR: Unexpected error: {str(e)}"
    
    @json_schema(
        description="Update an existing PLANE issue",
        params={
            "issue_id": {
                "type": "string",
                "description": "Issue ID to update",
                "required": True
            },
            "project_id": {
                "type": "string",
                "description": "Project ID",
                "required": True
            },
            "name": {
                "type": "string",
                "description": "New issue title"
            },
            "description": {
                "type": "string",
                "description": "New issue description"
            },
            "state_id": {
                "type": "string",
                "description": "New state ID"
            },
            "priority": {
                "type": "string",
                "description": "New priority: urgent, high, medium, low, or none"
            },
            "assignee_ids": {
                "type": "array",
                "description": "New list of assignee user IDs",
                "items": {"type": "string"}
            }
        }
    )
    async def plane_update_issue(self, **kwargs) -> str:
        """
        Update issue details
        
        Args:
            issue_id: Issue ID
            name: New title
            description: New description
            state_id: New state
            priority: New priority
            assignee_ids: New assignees
            
        Returns:
            Updated issue details
        """
        self._ensure_client()
        
        issue_id = kwargs.get("issue_id")
        project_id = kwargs.get("project_id")
        
        if not issue_id:
            return "ERROR: issue_id is required"
        if not project_id:
            return "ERROR: project_id is required"
        
        # Build updates dict
        updates = {}
        if "name" in kwargs:
            updates["name"] = kwargs["name"]
        if "description" in kwargs:
            updates["description"] = kwargs["description"]
        if "state_id" in kwargs:
            updates["state"] = kwargs["state_id"]
        if "priority" in kwargs:
            priority = kwargs["priority"].lower()
            if priority not in ["urgent", "high", "medium", "low", "none"]:
                return "ERROR: priority must be one of: urgent, high, medium, low, none"
            updates["priority"] = priority
        if "assignee_ids" in kwargs:
            updates["assignees"] = kwargs["assignee_ids"]
        
        if not updates:
            return "ERROR: No updates provided. Specify at least one field to update."
        
        try:
            issue = await self.client.update_issue(issue_id, project_id, updates)
            
            result = f"✅ Issue updated successfully!\n\n"
            result += self._format_issue(issue)
            return result
            
        except PlaneSessionExpired as e:
            return f"ERROR: {str(e)}"
        except PlaneAPIError as e:
            return f"ERROR: Failed to update issue: {str(e)}"
        except Exception as e:
            self.logger.error(f"Unexpected error updating issue: {str(e)}")
            return f"ERROR: Unexpected error: {str(e)}"
    
    @json_schema(
        description="Add a comment to a PLANE issue",
        params={
            "issue_id": {
                "type": "string",
                "description": "Issue ID to comment on",
                "required": True
            },
            "comment": {
                "type": "string",
                "description": "Comment text (supports markdown)",
                "required": True
            }
        }
    )
    async def plane_add_comment(self, **kwargs) -> str:
        """
        Add comment to issue
        
        Args:
            issue_id: Issue ID
            comment: Comment text
            
        Returns:
            Confirmation message
        """
        self._ensure_client()
        
        issue_id = kwargs.get("issue_id")
        comment = kwargs.get("comment")
        
        if not issue_id:
            return "ERROR: issue_id is required"
        if not comment:
            return "ERROR: comment is required"
        
        try:
            comment_data = await self.client.add_comment(issue_id, comment)
            return f"✅ Comment added successfully to issue {issue_id}"
            
        except PlaneSessionExpired as e:
            return f"ERROR: {str(e)}"
        except PlaneAPIError as e:
            return f"ERROR: Failed to add comment: {str(e)}"
        except Exception as e:
            self.logger.error(f"Unexpected error adding comment: {str(e)}")
            return f"ERROR: Unexpected error: {str(e)}"
    
    @json_schema(
        description="Get comments for a PLANE issue",
        params={
            "issue_id": {
                "type": "string",
                "description": "Issue ID to get comments for",
                "required": True
            },
            "project_id": {
                "type": "string",
                "description": "Project ID",
                "required": True
            }
        }
    )
    async def plane_get_comments(self, **kwargs) -> str:
        """
        Get issue comments
        
        Args:
            issue_id: Issue ID
            
        Returns:
            List of comments
        """
        self._ensure_client()
        
        issue_id = kwargs.get("issue_id")
        project_id = kwargs.get("project_id")
        
        if not issue_id:
            return "ERROR: issue_id is required"
        if not project_id:
            return "ERROR: project_id is required"
        
        try:
            comments = await self.client.get_comments(issue_id)
            
            if not comments:
                return "No comments on this issue."
            
            result = f"Found {len(comments)} comment(s):\n\n"
            
            for comment in comments[:10]:  # Limit to 10 for token efficiency
                author = comment.get('actor_detail', {}).get('display_name', 'Unknown')
                created = comment.get('created_at', 'Unknown date')
                text = comment.get('comment', '')
                
                result += f"**{author}** ({created})\n"
                if len(text) > 200:
                    text = text[:197] + "..."
                result += f"{text}\n\n"
            
            if len(comments) > 10:
                result += f"... and {len(comments) - 10} more comments\n"
            
            return result
            
        except PlaneSessionExpired as e:
            return f"ERROR: {str(e)}"
        except PlaneAPIError as e:
            return f"ERROR: Failed to get comments: {str(e)}"
        except Exception as e:
            self.logger.error(f"Unexpected error getting comments: {str(e)}")
            return f"ERROR: Unexpected error: {str(e)}"


# Register the toolset
Toolset.register(PlaneIssueTools)

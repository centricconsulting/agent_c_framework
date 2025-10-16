"""
PLANE Tools - Main Toolset Class

Single coordinating toolset that contains all 33 PLANE tool methods.
"""

import yaml
from typing import Optional
from agent_c.toolsets.tool_set import Toolset
from agent_c.toolsets.json_schema import json_schema

from .auth.plane_session import PlaneAPIError, PlaneSessionExpired
from .tools.plane_projects import PlaneProjectTools
from .tools.plane_issues import PlaneIssueTools
from .tools.plane_search import PlaneSearchTools
from .tools.plane_analytics import PlaneAnalyticsTools
from .tools.plane_issue_relations import PlaneIssueRelationsTools
from .tools.plane_labels import PlaneLabelTools
from .tools.plane_bulk import PlaneBulkTools


class PlaneTools(Toolset):
    """
    PLANE Project Management Tools
    
    Comprehensive toolset for managing PLANE projects, issues, labels, and workflows.
    Provides 33 tools across project management, issue tracking, search, analytics,
    relations, labels, and bulk operations.
    """
    
    def __init__(self, **kwargs):
        super().__init__(**kwargs, name='plane', use_prefix=True)
        
        # Sub-toolsets
        self._projects: Optional[PlaneProjectTools] = None
        self._issues: Optional[PlaneIssueTools] = None
        self._search: Optional[PlaneSearchTools] = None
        self._analytics: Optional[PlaneAnalyticsTools] = None
        self._relations: Optional[PlaneIssueRelationsTools] = None
        self._labels: Optional[PlaneLabelTools] = None
        self._bulk: Optional[PlaneBulkTools] = None
    
    async def post_init(self):
        """Initialize all sub-toolsets"""
        # Initialize specialized toolsets
        self._projects = PlaneProjectTools()
        self._issues = PlaneIssueTools()
        self._search = PlaneSearchTools()
        self._analytics = PlaneAnalyticsTools()
        self._relations = PlaneIssueRelationsTools()
        self._labels = PlaneLabelTools()
        self._bulk = PlaneBulkTools()
        
        # Initialize each
        await self._projects.post_init()
        await self._issues.post_init()
        await self._search.post_init()
        await self._analytics.post_init()
        await self._relations.post_init()
        await self._labels.post_init()
        await self._bulk.post_init()
        
        self.logger.info("PLANE tools initialized - all 7 toolsets ready")
    
    # ============================================================================
    # PROJECT TOOLS - Delegate to PlaneProjectTools
    # ============================================================================
    
    @json_schema(
        description="List all projects in the PLANE workspace",
        params={}
    )
    async def plane_list_projects(self, **kwargs) -> str:
        """List all projects"""
        return await self._projects.plane_list_projects(**kwargs)
    
    @json_schema(
        description="Get detailed information about a specific PLANE project",
        params={
            "project_id": {"type": "string", "description": "The project ID or identifier", "required": True}
        }
    )
    async def plane_get_project(self, **kwargs) -> str:
        """Get project details"""
        return await self._projects.plane_get_project(**kwargs)
    
    @json_schema(
        description="Create a new project in PLANE workspace",
        params={
            "name": {"type": "string", "description": "Project name", "required": True},
            "identifier": {"type": "string", "description": "Project identifier (e.g., 'WEB'). 2-5 uppercase letters.", "required": True},
            "description": {"type": "string", "description": "Project description"}
        }
    )
    async def plane_create_project(self, **kwargs) -> str:
        """Create a new project"""
        return await self._projects.plane_create_project(**kwargs)
    
    @json_schema(
        description="Update an existing PLANE project",
        params={
            "project_id": {"type": "string", "description": "Project ID", "required": True},
            "name": {"type": "string", "description": "New project name"},
            "description": {"type": "string", "description": "New project description"}
        }
    )
    async def plane_update_project(self, **kwargs) -> str:
        """Update project"""
        return await self._projects.plane_update_project(**kwargs)
    
    @json_schema(
        description="Archive/delete a PLANE project",
        params={
            "project_id": {"type": "string", "description": "Project ID to archive", "required": True}
        }
    )
    async def plane_archive_project(self, **kwargs) -> str:
        """Archive a project"""
        return await self._projects.plane_archive_project(**kwargs)
    
    # ============================================================================
    # ISSUE TOOLS - Delegate to PlaneIssueTools
    # ============================================================================
    
    @json_schema(
        description="List issues in a PLANE project with optional filtering",
        params={
            "project_id": {"type": "string", "description": "Project ID", "required": True},
            "state": {"type": "string", "description": "Filter by state"},
            "priority": {"type": "string", "description": "Filter by priority"},
            "assignee": {"type": "string", "description": "Filter by assignee user ID"}
        }
    )
    async def plane_list_issues(self, **kwargs) -> str:
        """List issues"""
        return await self._issues.plane_list_issues(**kwargs)
    
    @json_schema(
        description="Get detailed information about a specific PLANE issue",
        params={
            "issue_id": {"type": "string", "description": "The issue ID", "required": True},
            "project_id": {"type": "string", "description": "Project ID", "required": True}
        }
    )
    async def plane_get_issue(self, **kwargs) -> str:
        """Get issue details"""
        return await self._issues.plane_get_issue(**kwargs)
    
    @json_schema(
        description="Get PLANE issue by sequential identifier (e.g., 'AC-9') instead of UUID",
        params={
            "project_id": {"type": "string", "description": "Project ID", "required": True},
            "identifier": {"type": "string", "description": "Sequential ID like 'AC-9' or just '9'", "required": True}
        }
    )
    async def plane_get_issue_by_identifier(self, **kwargs) -> str:
        """Get issue by AC-9 style identifier"""
        self._ensure_client()
        
        project_id = kwargs.get("project_id")
        identifier = kwargs.get("identifier")
        
        if not project_id:
            return "ERROR: project_id is required"
        if not identifier:
            return "ERROR: identifier is required"
        
        try:
            issue = await self._issues.client.get_issue_by_identifier(project_id, identifier)
            
            # Format like plane_get_issue does
            result = yaml.dump({
                'id': issue.get('id'),
                'sequence_id': issue.get('sequence_id'),
                'identifier': f"{issue.get('project_detail', {}).get('identifier', '')}-{issue.get('sequence_id', '')}",
                'name': issue.get('name'),
                'description': issue.get('description', 'No description'),
                'state': issue.get('state_detail', {}).get('name'),
                'priority': issue.get('priority'),
                'project': issue.get('project_detail', {}).get('name'),
                'assignees': [a.get('display_name') for a in issue.get('assignees', [])],
            }, allow_unicode=True, default_flow_style=False)
            
            return result
            
        except PlaneSessionExpired as e:
            return f"ERROR: {str(e)}"
        except PlaneAPIError as e:
            return f"ERROR: {str(e)}"
        except Exception as e:
            self.logger.error(f"Unexpected error getting issue by identifier: {str(e)}")
            return f"ERROR: Unexpected error: {str(e)}"
    
    def _ensure_client(self):
        """Ensure sub-toolsets are initialized"""
        if self._issues is None:
            raise PlaneAPIError("PLANE toolsets not initialized")
    
    @json_schema(
        description="Create a new issue/task in a PLANE project",
        params={
            "project_id": {"type": "string", "description": "Project ID", "required": True},
            "name": {"type": "string", "description": "Issue title/name", "required": True},
            "description": {"type": "string", "description": "Issue description (supports markdown)"},
            "priority": {"type": "string", "description": "Priority: urgent, high, medium, low, or none"},
            "assignee_ids": {"type": "array", "description": "List of user IDs to assign", "items": {"type": "string"}},
            "parent_id": {"type": "string", "description": "Parent issue ID to create as sub-issue"}
        }
    )
    async def plane_create_issue(self, **kwargs) -> str:
        """Create a new issue"""
        return await self._issues.plane_create_issue(**kwargs)
    
    @json_schema(
        description="Update an existing PLANE issue",
        params={
            "issue_id": {"type": "string", "description": "Issue ID", "required": True},
            "project_id": {"type": "string", "description": "Project ID", "required": True},
            "name": {"type": "string", "description": "New issue title"},
            "description": {"type": "string", "description": "New issue description"},
            "state_id": {"type": "string", "description": "New state ID"},
            "priority": {"type": "string", "description": "New priority"},
            "assignee_ids": {"type": "array", "description": "New assignees", "items": {"type": "string"}}
        }
    )
    async def plane_update_issue(self, **kwargs) -> str:
        """Update issue"""
        return await self._issues.plane_update_issue(**kwargs)
    
    @json_schema(
        description="Add a comment to a PLANE issue",
        params={
            "issue_id": {"type": "string", "description": "Issue ID", "required": True},
            "comment": {"type": "string", "description": "Comment text (supports markdown)", "required": True}
        }
    )
    async def plane_add_comment(self, **kwargs) -> str:
        """Add comment"""
        return await self._issues.plane_add_comment(**kwargs)
    
    @json_schema(
        description="Get comments for a PLANE issue",
        params={
            "issue_id": {"type": "string", "description": "Issue ID", "required": True},
            "project_id": {"type": "string", "description": "Project ID", "required": True}
        }
    )
    async def plane_get_comments(self, **kwargs) -> str:
        """Get comments"""
        return await self._issues.plane_get_comments(**kwargs)
    
    # ============================================================================
    # SEARCH TOOLS - Delegate to PlaneSearchTools
    # ============================================================================
    
    @json_schema(
        description="Search across the entire PLANE workspace",
        params={
            "query": {"type": "string", "description": "Search query", "required": True},
            "search_type": {"type": "string", "description": "Type: 'all', 'projects', or 'issues'"}
        }
    )
    async def plane_search_global(self, **kwargs) -> str:
        """Global search"""
        return await self._search.plane_search_global(**kwargs)
    
    @json_schema(
        description="Search for issues matching criteria",
        params={
            "project_id": {"type": "string", "description": "Project ID (optional)"},
            "query": {"type": "string", "description": "Search query"},
            "state": {"type": "string", "description": "Filter by state"},
            "priority": {"type": "string", "description": "Filter by priority"},
            "assignee": {"type": "string", "description": "Filter by assignee"}
        }
    )
    async def plane_search_issues(self, **kwargs) -> str:
        """Search issues"""
        return await self._search.plane_search_issues(**kwargs)
    
    @json_schema(
        description="Find issues assigned to the current user",
        params={
            "state": {"type": "string", "description": "Filter by state"},
            "priority": {"type": "string", "description": "Filter by priority"}
        }
    )
    async def plane_find_my_issues(self, **kwargs) -> str:
        """Find my issues"""
        return await self._search.plane_find_my_issues(**kwargs)
    
    # ============================================================================
    # ANALYTICS TOOLS - Delegate to PlaneAnalyticsTools
    # ============================================================================
    
    @json_schema(
        description="Get workspace overview and statistics",
        params={}
    )
    async def plane_get_workspace_overview(self, **kwargs) -> str:
        """Workspace overview"""
        return await self._analytics.plane_get_workspace_overview(**kwargs)
    
    @json_schema(
        description="Get project statistics and analytics",
        params={
            "project_id": {"type": "string", "description": "Project ID", "required": True}
        }
    )
    async def plane_get_project_stats(self, **kwargs) -> str:
        """Project statistics"""
        return await self._analytics.plane_get_project_stats(**kwargs)
    
    @json_schema(
        description="Get team workload overview",
        params={
            "project_id": {"type": "string", "description": "Optional project ID to scope analysis"}
        }
    )
    async def plane_get_team_workload(self, **kwargs) -> str:
        """Team workload"""
        return await self._analytics.plane_get_team_workload(**kwargs)
    
    # ============================================================================
    # RELATION TOOLS - Delegate to PlaneIssueRelationsTools
    # ============================================================================
    
    @json_schema(
        description="Create a sub-issue under a parent issue",
        params={
            "project_id": {"type": "string", "description": "Project ID", "required": True},
            "parent_issue_id": {"type": "string", "description": "Parent issue ID", "required": True},
            "name": {"type": "string", "description": "Sub-issue title", "required": True},
            "description": {"type": "string", "description": "Sub-issue description"},
            "priority": {"type": "string", "description": "Priority level"}
        }
    )
    async def plane_create_sub_issue(self, **kwargs) -> str:
        """Create sub-issue"""
        return await self._relations.plane_create_sub_issue(**kwargs)
    
    @json_schema(
        description="Get sub-issues for a parent issue",
        params={
            "project_id": {"type": "string", "description": "Project ID", "required": True},
            "issue_id": {"type": "string", "description": "Parent issue ID", "required": True}
        }
    )
    async def plane_get_sub_issues(self, **kwargs) -> str:
        """Get sub-issues"""
        return await self._relations.plane_get_sub_issues(**kwargs)
    
    @json_schema(
        description="Add a relationship between two issues",
        params={
            "project_id": {"type": "string", "description": "Project ID", "required": True},
            "issue_id": {"type": "string", "description": "Source issue ID", "required": True},
            "related_issue_id": {"type": "string", "description": "Related issue ID", "required": True},
            "relation_type": {"type": "string", "description": "Type: blocking, blocked_by, duplicate, relates_to, start_after", "required": True}
        }
    )
    async def plane_add_issue_relation(self, **kwargs) -> str:
        """Add issue relation"""
        return await self._relations.plane_add_issue_relation(**kwargs)
    
    @json_schema(
        description="Get all relationships for an issue",
        params={
            "project_id": {"type": "string", "description": "Project ID", "required": True},
            "issue_id": {"type": "string", "description": "Issue ID", "required": True}
        }
    )
    async def plane_get_issue_relations(self, **kwargs) -> str:
        """Get issue relations"""
        return await self._relations.plane_get_issue_relations(**kwargs)
    
    @json_schema(
        description="Remove a relationship between two issues",
        params={
            "project_id": {"type": "string", "description": "Project ID", "required": True},
            "issue_id": {"type": "string", "description": "Source issue ID", "required": True},
            "related_issue_id": {"type": "string", "description": "Related issue ID", "required": True}
        }
    )
    async def plane_remove_issue_relation(self, **kwargs) -> str:
        """Remove relation"""
        return await self._relations.plane_remove_issue_relation(**kwargs)
    
    @json_schema(
        description="Format text with markdown",
        params={
            "content": {"type": "string", "description": "Text to format", "required": True},
            "format_type": {"type": "string", "description": "Format: heading1, heading2, heading3, list, checklist, code, quote, bold, italic", "required": True}
        }
    )
    async def plane_format_markdown(self, **kwargs) -> str:
        """Format markdown"""
        return await self._relations.plane_format_markdown(**kwargs)
    
    # ============================================================================
    # LABEL TOOLS - Delegate to PlaneLabelTools
    # ============================================================================
    
    @json_schema(
        description="List all labels in workspace",
        params={
            "project_id": {"type": "string", "description": "Optional project ID"}
        }
    )
    async def plane_list_labels(self, **kwargs) -> str:
        """List labels"""
        return await self._labels.plane_list_labels(**kwargs)
    
    @json_schema(
        description="Create a new label",
        params={
            "name": {"type": "string", "description": "Label name", "required": True},
            "color": {"type": "string", "description": "Hex color (e.g., #FF5733)"},
            "description": {"type": "string", "description": "Label description"}
        }
    )
    async def plane_create_label(self, **kwargs) -> str:
        """Create label"""
        return await self._labels.plane_create_label(**kwargs)
    
    @json_schema(
        description="Add a label to an issue",
        params={
            "issue_id": {"type": "string", "description": "Issue ID", "required": True},
            "project_id": {"type": "string", "description": "Project ID", "required": True},
            "label_id": {"type": "string", "description": "Label ID", "required": True}
        }
    )
    async def plane_add_label_to_issue(self, **kwargs) -> str:
        """Add label to issue"""
        return await self._labels.plane_add_label_to_issue(**kwargs)
    
    @json_schema(
        description="Remove a label from an issue",
        params={
            "issue_id": {"type": "string", "description": "Issue ID", "required": True},
            "project_id": {"type": "string", "description": "Project ID", "required": True},
            "label_id": {"type": "string", "description": "Label ID", "required": True}
        }
    )
    async def plane_remove_label_from_issue(self, **kwargs) -> str:
        """Remove label"""
        return await self._labels.plane_remove_label_from_issue(**kwargs)
    
    @json_schema(
        description="Delete a label from workspace",
        params={
            "label_id": {"type": "string", "description": "Label ID", "required": True}
        }
    )
    async def plane_delete_label(self, **kwargs) -> str:
        """Delete label"""
        return await self._labels.plane_delete_label(**kwargs)
    
    # ============================================================================
    # BULK TOOLS - Delegate to PlaneBulkTools
    # ============================================================================
    
    @json_schema(
        description="Update multiple issues at once",
        params={
            "issue_ids": {"type": "array", "description": "List of issue IDs", "items": {"type": "string"}, "required": True},
            "updates": {"type": "object", "description": "Fields to update", "required": True}
        }
    )
    async def plane_bulk_update_issues(self, **kwargs) -> str:
        """Bulk update"""
        return await self._bulk.plane_bulk_update_issues(**kwargs)
    
    @json_schema(
        description="Assign multiple issues to a user",
        params={
            "issue_ids": {"type": "array", "description": "List of issue IDs", "items": {"type": "string"}, "required": True},
            "assignee_id": {"type": "string", "description": "User ID", "required": True}
        }
    )
    async def plane_bulk_assign_issues(self, **kwargs) -> str:
        """Bulk assign"""
        return await self._bulk.plane_bulk_assign_issues(**kwargs)
    
    @json_schema(
        description="Change state of multiple issues",
        params={
            "issue_ids": {"type": "array", "description": "List of issue IDs", "items": {"type": "string"}, "required": True},
            "state_id": {"type": "string", "description": "State ID", "required": True}
        }
    )
    async def plane_bulk_change_state(self, **kwargs) -> str:
        """Bulk change state"""
        return await self._bulk.plane_bulk_change_state(**kwargs)
    
    @json_schema(
        description="Change priority of multiple issues",
        params={
            "issue_ids": {"type": "array", "description": "List of issue IDs", "items": {"type": "string"}, "required": True},
            "priority": {"type": "string", "description": "Priority", "required": True}
        }
    )
    async def plane_bulk_change_priority(self, **kwargs) -> str:
        """Bulk change priority"""
        return await self._bulk.plane_bulk_change_priority(**kwargs)
    
    @json_schema(
        description="Add same label to multiple issues",
        params={
            "issue_ids": {"type": "array", "description": "List of issue IDs", "items": {"type": "string"}, "required": True},
            "label_id": {"type": "string", "description": "Label ID", "required": True}
        }
    )
    async def plane_bulk_add_label(self, **kwargs) -> str:
        """Bulk add label"""
        return await self._bulk.plane_bulk_add_label(**kwargs)


# Register the main toolset
Toolset.register(PlaneTools)

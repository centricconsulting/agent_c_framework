"""
PLANE Search Toolset

Tools for searching across PLANE projects and issues.
"""

import yaml
from typing import Optional
from agent_c.toolsets import Toolset, json_schema
from ..client.plane_client import PlaneClient, PlaneAPIError
from ..auth.plane_session import PlaneSessionExpired


class PlaneSearchTools(Toolset):
    """Tools for searching PLANE workspace"""
    
    def __init__(self, **kwargs):
        super().__init__(**kwargs, name='plane_search', use_prefix=True)
        self.client: Optional[PlaneClient] = None
        self.base_url = "http://localhost"
        self.workspace_slug = "agent_c"
    
    async def post_init(self):
        """Initialize PLANE client after toolset creation"""
        try:
            self.client = PlaneClient(self.base_url, self.workspace_slug)
            self.logger.info(f"PLANE search tools initialized for workspace: {self.workspace_slug}")
        except Exception as e:
            self.logger.error(f"Failed to initialize PLANE client: {str(e)}")
            self.client = None
    
    def _ensure_client(self):
        """Ensure client is initialized"""
        if self.client is None:
            raise PlaneAPIError("PLANE client not initialized. Check authentication.")
    
    def _format_search_results(self, results: dict) -> str:
        """Format search results for display"""
        if not results:
            return "No results found."
        
        output = ""
        
        # Format projects if present
        if 'projects' in results and results['projects']:
            projects = results['projects']
            output += f"**Projects ({len(projects)}):**\n\n"
            for proj in projects[:5]:  # Limit for token efficiency
                output += f"• **{proj.get('name')}** ({proj.get('identifier')})\n"
                output += f"  ID: {proj.get('id')}\n"
                if proj.get('description'):
                    desc = proj.get('description', '')[:100]
                    output += f"  Description: {desc}...\n"
                output += "\n"
            if len(projects) > 5:
                output += f"  ... and {len(projects) - 5} more projects\n\n"
        
        # Format issues if present
        if 'issues' in results and results['issues']:
            issues = results['issues']
            output += f"**Issues ({len(issues)}):**\n\n"
            for issue in issues[:10]:  # Limit for token efficiency
                output += f"• **{issue.get('name', 'Untitled')}**\n"
                output += f"  ID: {issue.get('id')}\n"
                output += f"  Project: {issue.get('project_detail', {}).get('name', 'N/A')}\n"
                output += f"  State: {issue.get('state_detail', {}).get('name', 'N/A')}\n"
                output += "\n"
            if len(issues) > 10:
                output += f"  ... and {len(issues) - 10} more issues\n\n"
        
        if not output:
            return "No results found."
        
        return output
    
    @json_schema(
        description="Search across the entire PLANE workspace for projects and issues",
        params={
            "query": {
                "type": "string",
                "description": "Search query text",
                "required": True
            },
            "search_type": {
                "type": "string",
                "description": "Type of search: 'all', 'projects', or 'issues'. Default: 'all'"
            }
        }
    )
    async def plane_search_global(self, **kwargs) -> str:
        """
        Search across workspace
        
        Args:
            query: Search query
            search_type: What to search (all, projects, issues)
            
        Returns:
            Formatted search results
        """
        self._ensure_client()
        
        query = kwargs.get("query")
        if not query:
            return "ERROR: query is required"
        
        search_type = kwargs.get("search_type", "all")
        
        try:
            # Build filters based on search type
            filters = {}
            if search_type == "projects":
                filters["entity_type"] = "project"
            elif search_type == "issues":
                filters["entity_type"] = "issue"
            
            results = self.client.search(query, filters if filters else None)
            
            return self._format_search_results(results)
            
        except PlaneSessionExpired as e:
            return f"ERROR: {str(e)}"
        except PlaneAPIError as e:
            return f"ERROR: Search failed: {str(e)}"
        except Exception as e:
            self.logger.error(f"Unexpected error in search: {str(e)}")
            return f"ERROR: Unexpected error: {str(e)}"
    
    @json_schema(
        description="Search for issues matching specific criteria",
        params={
            "project_id": {
                "type": "string",
                "description": "Project ID to search within (optional)"
            },
            "query": {
                "type": "string",
                "description": "Search query text"
            },
            "state": {
                "type": "string",
                "description": "Filter by state (e.g., 'todo', 'in_progress', 'done')"
            },
            "priority": {
                "type": "string",
                "description": "Filter by priority (urgent, high, medium, low, none)"
            },
            "assignee": {
                "type": "string",
                "description": "Filter by assignee user ID"
            }
        }
    )
    async def plane_search_issues(self, **kwargs) -> str:
        """
        Search for issues with filters
        
        Args:
            project_id: Optional project to search within
            query: Search query
            state: State filter
            priority: Priority filter
            assignee: Assignee filter
            
        Returns:
            Matching issues
        """
        self._ensure_client()
        
        project_id = kwargs.get("project_id")
        query = kwargs.get("query")
        
        # Build filters
        filters = {}
        if query:
            filters["search"] = query
        if "state" in kwargs:
            filters["state"] = kwargs["state"]
        if "priority" in kwargs:
            filters["priority"] = kwargs["priority"]
        if "assignee" in kwargs:
            filters["assignee"] = kwargs["assignee"]
        
        try:
            issues = self.client.list_issues(
                project_id=project_id,
                filters=filters if filters else None
            )
            
            # Handle response format
            if isinstance(issues, dict) and 'results' in issues:
                issues = issues['results']
            
            if not issues:
                return "No issues found matching criteria."
            
            # Format results
            result = f"Found {len(issues)} issue(s):\n\n"
            
            display_issues = issues[:15] if len(issues) > 15 else issues
            
            for issue in display_issues:
                result += f"• **{issue.get('name', 'Untitled')}**\n"
                result += f"  ID: {issue.get('id')}\n"
                result += f"  State: {issue.get('state_detail', {}).get('name', 'N/A')}\n"
                result += f"  Priority: {issue.get('priority', 'none')}\n"
                
                if issue.get('assignees'):
                    assignees = [a.get('display_name', 'Unknown') for a in issue.get('assignees', [])]
                    result += f"  Assignees: {', '.join(assignees)}\n"
                
                result += "\n"
            
            if len(issues) > 15:
                result += f"... and {len(issues) - 15} more issues\n"
            
            return result
            
        except PlaneSessionExpired as e:
            return f"ERROR: {str(e)}"
        except PlaneAPIError as e:
            return f"ERROR: Search failed: {str(e)}"
        except Exception as e:
            self.logger.error(f"Unexpected error searching issues: {str(e)}")
            return f"ERROR: Unexpected error: {str(e)}"
    
    @json_schema(
        description="Find issues assigned to the current user",
        params={
            "state": {
                "type": "string",
                "description": "Filter by state (e.g., 'todo', 'in_progress', 'done')"
            },
            "priority": {
                "type": "string",
                "description": "Filter by priority (urgent, high, medium, low, none)"
            }
        }
    )
    async def plane_find_my_issues(self, **kwargs) -> str:
        """
        Find issues assigned to current user
        
        Args:
            state: Optional state filter
            priority: Optional priority filter
            
        Returns:
            User's assigned issues
        """
        self._ensure_client()
        
        try:
            # First get current user ID
            user = self.client.get_current_user()
            user_id = user.get('id')
            
            if not user_id:
                return "ERROR: Could not determine current user ID"
            
            # Build filters
            filters = {"assignee": user_id}
            if "state" in kwargs:
                filters["state"] = kwargs["state"]
            if "priority" in kwargs:
                filters["priority"] = kwargs["priority"]
            
            # Get all issues for user (workspace-level)
            issues = self.client.list_issues(filters=filters)
            
            # Handle response format
            if isinstance(issues, dict) and 'results' in issues:
                issues = issues['results']
            
            if not issues:
                return "No issues assigned to you."
            
            # Format results grouped by priority
            urgent = []
            high = []
            medium = []
            low = []
            none_priority = []
            
            for issue in issues:
                priority = issue.get('priority', 'none')
                if priority == 'urgent':
                    urgent.append(issue)
                elif priority == 'high':
                    high.append(issue)
                elif priority == 'medium':
                    medium.append(issue)
                elif priority == 'low':
                    low.append(issue)
                else:
                    none_priority.append(issue)
            
            result = f"You have {len(issues)} issue(s) assigned:\n\n"
            
            if urgent:
                result += f"**🔴 URGENT ({len(urgent)}):**\n"
                for issue in urgent:
                    result += f"• {issue.get('name', 'Untitled')} (ID: {issue.get('id')})\n"
                result += "\n"
            
            if high:
                result += f"**🟠 HIGH ({len(high)}):**\n"
                for issue in high:
                    result += f"• {issue.get('name', 'Untitled')} (ID: {issue.get('id')})\n"
                result += "\n"
            
            if medium:
                result += f"**🟡 MEDIUM ({len(medium)}):**\n"
                for issue in medium[:5]:  # Limit medium priority display
                    result += f"• {issue.get('name', 'Untitled')} (ID: {issue.get('id')})\n"
                if len(medium) > 5:
                    result += f"  ... and {len(medium) - 5} more\n"
                result += "\n"
            
            if low:
                result += f"**🟢 LOW ({len(low)}):**\n"
                for issue in low[:3]:  # Limit low priority display
                    result += f"• {issue.get('name', 'Untitled')} (ID: {issue.get('id')})\n"
                if len(low) > 3:
                    result += f"  ... and {len(low) - 3} more\n"
                result += "\n"
            
            if none_priority:
                result += f"**⚪ NO PRIORITY ({len(none_priority)}):**\n"
                for issue in none_priority[:3]:
                    result += f"• {issue.get('name', 'Untitled')} (ID: {issue.get('id')})\n"
                if len(none_priority) > 3:
                    result += f"  ... and {len(none_priority) - 3} more\n"
            
            return result
            
        except PlaneSessionExpired as e:
            return f"ERROR: {str(e)}"
        except PlaneAPIError as e:
            return f"ERROR: Failed to find your issues: {str(e)}"
        except Exception as e:
            self.logger.error(f"Unexpected error finding user issues: {str(e)}")
            return f"ERROR: Unexpected error: {str(e)}"


# Register the toolset
Toolset.register(PlaneSearchTools)

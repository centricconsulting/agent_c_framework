"""
PLANE Analytics Toolset

Tools for getting workspace and project analytics, reports, and insights.
"""

import yaml
from typing import Optional
from agent_c.toolsets import Toolset, json_schema
from ..client.plane_client import PlaneClient, PlaneAPIError
from ..auth.plane_session import PlaneSessionExpired


class PlaneAnalyticsTools(Toolset):
    """Tools for PLANE analytics and reporting"""
    
    def __init__(self, **kwargs):
        super().__init__(**kwargs, name='plane_analytics', use_prefix=True)
        self.client: Optional[PlaneClient] = None
        self.base_url = "http://localhost"
        self.workspace_slug = "agent_c"
    
    async def post_init(self):
        """Initialize PLANE client after toolset creation"""
        try:
            self.client = PlaneClient(self.base_url, self.workspace_slug)
            self.logger.info(f"PLANE analytics tools initialized for workspace: {self.workspace_slug}")
        except Exception as e:
            self.logger.error(f"Failed to initialize PLANE client: {str(e)}")
            self.client = None
    
    def _ensure_client(self):
        """Ensure client is initialized"""
        if self.client is None:
            raise PlaneAPIError("PLANE client not initialized. Check authentication.")
    
    @json_schema(
        description="Get workspace overview and statistics",
        params={}
    )
    async def plane_get_workspace_overview(self, **kwargs) -> str:
        """
        Get workspace overview
        
        Returns:
            Workspace statistics and overview
        """
        self._ensure_client()
        
        try:
            # Get workspace info
            workspace = await self.client.get_workspace()
            
            # Get projects
            projects = await self.client.list_projects()
            
            # Get workspace members
            members = await self.client.get_workspace_members()
            
            # Count issues across all projects
            total_issues = 0
            active_projects = 0
            
            for project in projects:
                if not project.get('archived_at'):
                    active_projects += 1
                    try:
                        issues = await self.client.list_issues(project_id=project.get('id'))
                        if isinstance(issues, dict) and 'results' in issues:
                            issues = issues['results']
                        if isinstance(issues, list):
                            total_issues += len(issues)
                    except:
                        pass
            
            result = f"**{workspace.get('name')} Workspace Overview**\n\n"
            result += f"📊 **Statistics:**\n"
            result += f"  Projects: {len(projects)} total ({active_projects} active)\n"
            result += f"  Members: {len(members)}\n"
            result += f"  Total Issues: {total_issues}\n"
            result += f"\n"
            
            result += f"📂 **Active Projects:**\n"
            for project in projects:
                if not project.get('archived_at'):
                    result += f"  • {project.get('name')} ({project.get('identifier')})\n"
            
            return result
            
        except PlaneSessionExpired as e:
            return f"ERROR: {str(e)}"
        except PlaneAPIError as e:
            return f"ERROR: Failed to get workspace overview: {str(e)}"
        except Exception as e:
            self.logger.error(f"Unexpected error getting workspace overview: {str(e)}")
            return f"ERROR: Unexpected error: {str(e)}"
    
    @json_schema(
        description="Get project statistics and analytics",
        params={
            "project_id": {
                "type": "string",
                "description": "Project ID to analyze",
                "required": True
            }
        }
    )
    async def plane_get_project_stats(self, **kwargs) -> str:
        """
        Get project statistics
        
        Args:
            project_id: Project ID
            
        Returns:
            Project statistics and analytics
        """
        self._ensure_client()
        
        project_id = kwargs.get("project_id")
        if not project_id:
            return "ERROR: project_id is required"
        
        try:
            # Get project details
            project = await self.client.get_project(project_id)
            
            # Get all issues for project
            issues = await self.client.list_issues(project_id=project_id)
            
            # Handle response format
            if isinstance(issues, dict) and 'results' in issues:
                issues = issues['results']
            
            if not isinstance(issues, list):
                issues = []
            
            # Analyze issues by state
            state_counts = {}
            priority_counts = {
                'urgent': 0,
                'high': 0,
                'medium': 0,
                'low': 0,
                'none': 0
            }
            assigned_count = 0
            unassigned_count = 0
            
            for issue in issues:
                # Count by state
                state = issue.get('state_detail', {}).get('name', 'Unknown')
                state_counts[state] = state_counts.get(state, 0) + 1
                
                # Count by priority
                priority = issue.get('priority', 'none')
                if priority in priority_counts:
                    priority_counts[priority] += 1
                
                # Count assigned vs unassigned
                if issue.get('assignees'):
                    assigned_count += 1
                else:
                    unassigned_count += 1
            
            # Format output
            result = f"**{project.get('name')} Project Statistics**\n\n"
            result += f"📊 **Overview:**\n"
            result += f"  Total Issues: {len(issues)}\n"
            result += f"  Assigned: {assigned_count}\n"
            result += f"  Unassigned: {unassigned_count}\n"
            result += f"\n"
            
            if state_counts:
                result += f"📈 **By State:**\n"
                for state, count in sorted(state_counts.items(), key=lambda x: x[1], reverse=True):
                    result += f"  {state}: {count}\n"
                result += f"\n"
            
            result += f"🎯 **By Priority:**\n"
            if priority_counts['urgent'] > 0:
                result += f"  🔴 Urgent: {priority_counts['urgent']}\n"
            if priority_counts['high'] > 0:
                result += f"  🟠 High: {priority_counts['high']}\n"
            if priority_counts['medium'] > 0:
                result += f"  🟡 Medium: {priority_counts['medium']}\n"
            if priority_counts['low'] > 0:
                result += f"  🟢 Low: {priority_counts['low']}\n"
            if priority_counts['none'] > 0:
                result += f"  ⚪ None: {priority_counts['none']}\n"
            
            return result
            
        except PlaneSessionExpired as e:
            return f"ERROR: {str(e)}"
        except PlaneAPIError as e:
            return f"ERROR: Failed to get project stats: {str(e)}"
        except Exception as e:
            self.logger.error(f"Unexpected error getting project stats: {str(e)}")
            return f"ERROR: Unexpected error: {str(e)}"
    
    @json_schema(
        description="Get team workload overview showing who's working on what",
        params={
            "project_id": {
                "type": "string",
                "description": "Optional project ID to scope workload analysis"
            }
        }
    )
    async def plane_get_team_workload(self, **kwargs) -> str:
        """
        Get team workload overview
        
        Args:
            project_id: Optional project to scope analysis
            
        Returns:
            Team workload analysis
        """
        self._ensure_client()
        
        project_id = kwargs.get("project_id")
        
        try:
            # Get workspace members
            members = await self.client.get_workspace_members()
            
            # Get issues
            if project_id:
                issues = await self.client.list_issues(project_id=project_id)
            else:
                # Get all workspace issues
                issues = await self.client.list_issues()
            
            # Handle response format
            if isinstance(issues, dict) and 'results' in issues:
                issues = issues['results']
            
            if not isinstance(issues, list):
                issues = []
            
            # Analyze workload by member
            workload = {}
            
            for issue in issues:
                assignees = issue.get('assignees', [])
                
                if not assignees:
                    # Track unassigned
                    if 'Unassigned' not in workload:
                        workload['Unassigned'] = {
                            'count': 0,
                            'urgent': 0,
                            'high': 0,
                            'medium': 0,
                            'low': 0
                        }
                    workload['Unassigned']['count'] += 1
                    priority = issue.get('priority', 'none')
                    if priority in workload['Unassigned']:
                        workload['Unassigned'][priority] += 1
                else:
                    for assignee in assignees:
                        name = assignee.get('display_name', 'Unknown')
                        
                        if name not in workload:
                            workload[name] = {
                                'count': 0,
                                'urgent': 0,
                                'high': 0,
                                'medium': 0,
                                'low': 0
                            }
                        
                        workload[name]['count'] += 1
                        priority = issue.get('priority', 'none')
                        if priority in workload[name]:
                            workload[name][priority] += 1
            
            # Format output
            scope = f"project" if project_id else "workspace"
            result = f"**Team Workload ({scope}):**\n\n"
            
            # Sort by total count
            sorted_workload = sorted(workload.items(), key=lambda x: x[1]['count'], reverse=True)
            
            for name, stats in sorted_workload:
                result += f"**{name}** ({stats['count']} issues)\n"
                
                if stats['urgent'] > 0:
                    result += f"  🔴 Urgent: {stats['urgent']}\n"
                if stats['high'] > 0:
                    result += f"  🟠 High: {stats['high']}\n"
                if stats['medium'] > 0:
                    result += f"  🟡 Medium: {stats['medium']}\n"
                if stats['low'] > 0:
                    result += f"  🟢 Low: {stats['low']}\n"
                
                result += "\n"
            
            return result
            
        except PlaneSessionExpired as e:
            return f"ERROR: {str(e)}"
        except PlaneAPIError as e:
            return f"ERROR: Failed to get team workload: {str(e)}"
        except Exception as e:
            self.logger.error(f"Unexpected error getting team workload: {str(e)}")
            return f"ERROR: Unexpected error: {str(e)}"


# Register the toolset
Toolset.register(PlaneAnalyticsTools)

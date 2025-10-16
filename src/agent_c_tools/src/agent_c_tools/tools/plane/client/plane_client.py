"""
PLANE API Client

Core async client for making requests to PLANE API with standardized error handling,
response parsing, and token-efficient formatting.
"""

import json
import structlog
from typing import Dict, List, Optional, Any, Tuple
from ..auth.plane_session import PlaneSession, PlaneSessionExpired, PlaneAPIError


logger = structlog.get_logger(__name__)


class PlaneClient:
    """
    Core async PLANE API client
    
    Handles all API communication with PLANE, including:
    - Request formatting
    - Response parsing
    - Error handling
    - Data transformation
    """
    
    def __init__(
        self,
        base_url: str = "http://localhost",
        workspace_slug: str = "agent_c",
        cookies: Optional[Dict[str, str]] = None
    ):
        """
        Initialize PLANE client
        
        Args:
            base_url: PLANE instance URL
            workspace_slug: Workspace identifier
            cookies: Optional cookies (will load from disk if not provided)
        """
        self.base_url = base_url
        self.workspace_slug = workspace_slug
        self.session = PlaneSession(base_url, workspace_slug, cookies)
        self.logger = logger.bind(workspace=workspace_slug)
    
    def _build_endpoint(self, path: str) -> str:
        """
        Build full API endpoint URL
        
        Args:
            path: Relative path (can include {workspace_slug} placeholder)
            
        Returns:
            Full endpoint path
        """
        # Replace workspace placeholder
        path = path.replace('{workspace_slug}', self.workspace_slug)
        
        # Ensure path starts with /api
        if not path.startswith('/api'):
            path = f'/api{path}'
        
        return path
    
    async def _handle_response(self, response_data: Tuple[int, str, bytes, Dict[str, str]], operation: str) -> Any:
        """
        Handle API response with standard error handling
        
        Args:
            response_data: Tuple of (status, content_type, body_bytes, headers)
            operation: Description of operation for error messages
            
        Returns:
            Parsed JSON response or raises exception
            
        Raises:
            PlaneAPIError: For API errors
        """
        status, content_type, body, headers = response_data
        
        try:
            # Handle empty responses
            if status == 204 or len(body) == 0:
                return None
            
            # Raise for HTTP errors
            if status >= 400:
                error_data = json.loads(body.decode('utf-8')) if content_type == 'application/json' else {}
                error_msg = error_data.get('detail') or error_data.get('error') or f"HTTP {status}"
                self.logger.error(f"{operation} failed", status_code=status, error=error_msg)
                raise PlaneAPIError(f"{operation} failed: {error_msg}")
            
            return json.loads(body.decode('utf-8'))
            
        except PlaneAPIError:
            raise
        except Exception as e:
            error_msg = f"{operation} failed: {str(e)}"
            self.logger.error(error_msg, status_code=status)
            raise PlaneAPIError(error_msg)
    
    # ==================================================================
    # WORKSPACE METHODS
    # ==================================================================
    
    async def get_workspace(self) -> Dict[str, Any]:
        """Get workspace information"""
        endpoint = self._build_endpoint('/workspaces/{workspace_slug}/')
        response = await self.session.get(endpoint)
        return await self._handle_response(response, "Get workspace")
    
    async def get_workspace_members(self) -> List[Dict[str, Any]]:
        """Get workspace members"""
        endpoint = self._build_endpoint('/workspaces/{workspace_slug}/members/')
        response = await self.session.get(endpoint)
        return await self._handle_response(response, "Get workspace members")
    
    # ==================================================================
    # PROJECT METHODS
    # ==================================================================
    
    async def list_projects(self) -> List[Dict[str, Any]]:
        """List all projects in workspace"""
        endpoint = self._build_endpoint('/workspaces/{workspace_slug}/projects/')
        response = await self.session.get(endpoint)
        return await self._handle_response(response, "List projects")
    
    async def get_project(self, project_id: str) -> Dict[str, Any]:
        """Get project details"""
        endpoint = self._build_endpoint(f'/workspaces/{{workspace_slug}}/projects/{project_id}/')
        response = await self.session.get(endpoint)
        return await self._handle_response(response, f"Get project {project_id}")
    
    async def create_project(self, project_data: Dict[str, Any]) -> Dict[str, Any]:
        """Create a new project"""
        endpoint = self._build_endpoint('/workspaces/{workspace_slug}/projects/')
        response = await self.session.post(endpoint, json=project_data)
        return await self._handle_response(response, "Create project")
    
    async def update_project(self, project_id: str, updates: Dict[str, Any]) -> Dict[str, Any]:
        """Update project"""
        endpoint = self._build_endpoint(f'/workspaces/{{workspace_slug}}/projects/{project_id}/')
        response = await self.session.patch(endpoint, json=updates)
        return await self._handle_response(response, f"Update project {project_id}")
    
    async def delete_project(self, project_id: str) -> None:
        """Delete/archive project"""
        endpoint = self._build_endpoint(f'/workspaces/{{workspace_slug}}/projects/{project_id}/')
        response = await self.session.delete(endpoint)
        await self._handle_response(response, f"Delete project {project_id}")
    
    # ==================================================================
    # ISSUE METHODS
    # ==================================================================
    
    async def list_issues(
        self,
        project_id: Optional[str] = None,
        filters: Optional[Dict[str, Any]] = None
    ) -> List[Dict[str, Any]]:
        """List issues"""
        if project_id:
            endpoint = self._build_endpoint(
                f'/workspaces/{{workspace_slug}}/projects/{project_id}/issues/'
            )
        else:
            endpoint = self._build_endpoint('/workspaces/{workspace_slug}/issues/')
        
        params = filters or {}
        response = await self.session.get(endpoint, params=params)
        return await self._handle_response(response, "List issues")
    
    async def get_issue(self, issue_id: str, project_id: str) -> Dict[str, Any]:
        """Get issue details - requires project_id"""
        endpoint = self._build_endpoint(
            f'/workspaces/{{workspace_slug}}/projects/{project_id}/issues/{issue_id}/'
        )
        response = await self.session.get(endpoint)
        return await self._handle_response(response, f"Get issue {issue_id}")
    
    async def create_issue(
        self,
        project_id: str,
        issue_data: Dict[str, Any]
    ) -> Dict[str, Any]:
        """Create a new issue"""
        endpoint = self._build_endpoint(
            f'/workspaces/{{workspace_slug}}/projects/{project_id}/issues/'
        )
        response = await self.session.post(endpoint, json=issue_data)
        return await self._handle_response(response, "Create issue")
    
    async def update_issue(
        self,
        issue_id: str,
        project_id: str,
        updates: Dict[str, Any]
    ) -> Dict[str, Any]:
        """Update issue - requires project_id"""
        endpoint = self._build_endpoint(
            f'/workspaces/{{workspace_slug}}/projects/{project_id}/issues/{issue_id}/'
        )
        response = await self.session.patch(endpoint, json=updates)
        return await self._handle_response(response, f"Update issue {issue_id}")
    
    async def delete_issue(self, issue_id: str, project_id: str) -> None:
        """Delete issue - requires project_id"""
        endpoint = self._build_endpoint(
            f'/workspaces/{{workspace_slug}}/projects/{project_id}/issues/{issue_id}/'
        )
        response = await self.session.delete(endpoint)
        await self._handle_response(response, f"Delete issue {issue_id}")
    
    async def get_issue_by_identifier(self, project_id: str, identifier: str) -> Dict[str, Any]:
        """
        Get issue by sequential identifier (e.g., 'AC-9' or '9')
        
        Args:
            project_id: Project ID
            identifier: Sequential identifier like 'AC-9' or just the number
            
        Returns:
            Issue data with UUID
            
        Raises:
            PlaneAPIError: If issue not found
        """
        issues = await self.list_issues(project_id)
        
        # Handle dict response with 'results'
        if isinstance(issues, dict) and 'results' in issues:
            issues = issues['results']
        
        # Parse identifier - could be 'AC-9' or just '9'
        if '-' in identifier:
            # Format: 'AC-9'
            parts = identifier.split('-')
            sequence_num = parts[-1]
        else:
            # Just the number: '9'
            sequence_num = identifier
        
        # Search for matching issue
        for issue in issues:
            issue_seq = str(issue.get('sequence_id', ''))
            if issue_seq == sequence_num:
                return issue
        
        raise PlaneAPIError(f"Issue '{identifier}' not found in project")
    
    # ==================================================================
    # COMMENT METHODS
    # ==================================================================
    
    async def add_comment(
        self,
        issue_id: str,
        comment: str
    ) -> Dict[str, Any]:
        """Add comment to issue"""
        endpoint = self._build_endpoint(f'/workspaces/{{workspace_slug}}/issues/{issue_id}/comments/')
        response = await self.session.post(endpoint, json={'comment': comment})
        return await self._handle_response(response, f"Add comment to issue {issue_id}")
    
    async def get_comments(self, issue_id: str) -> List[Dict[str, Any]]:
        """Get issue comments"""
        endpoint = self._build_endpoint(f'/workspaces/{{workspace_slug}}/issues/{issue_id}/comments/')
        response = await self.session.get(endpoint)
        return await self._handle_response(response, f"Get comments for issue {issue_id}")
    
    # ==================================================================
    # SEARCH METHODS
    # ==================================================================
    
    async def search(self, query: str, filters: Optional[Dict[str, Any]] = None) -> Dict[str, Any]:
        """Search across workspace"""
        endpoint = self._build_endpoint('/workspaces/{workspace_slug}/search/')
        params = {'query': query}
        if filters:
            params.update(filters)
        
        response = await self.session.get(endpoint, params=params)
        return await self._handle_response(response, f"Search for '{query}'")
    
    # ==================================================================
    # USER METHODS
    # ==================================================================
    
    async def get_current_user(self) -> Dict[str, Any]:
        """Get current authenticated user"""
        response = await self.session.get('/api/users/me/')
        return await self._handle_response(response, "Get current user")
    
    async def get_user_workspaces(self) -> List[Dict[str, Any]]:
        """Get user's workspaces"""
        response = await self.session.get('/api/users/me/workspaces/')
        return await self._handle_response(response, "Get user workspaces")


# Exceptions are defined in plane_session.py and imported above

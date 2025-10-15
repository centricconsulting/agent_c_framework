"""
PLANE API Client

Core client for making requests to PLANE API with standardized error handling,
response parsing, and token-efficient formatting.
"""

import structlog
from typing import Dict, List, Optional, Any, Union
from ..auth.plane_session import PlaneSession, PlaneSessionExpired


logger = structlog.get_logger(__name__)


class PlaneClient:
    """
    Core PLANE API client
    
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
    
    def _handle_response(self, response, operation: str) -> Any:
        """
        Handle API response with standard error handling
        
        Args:
            response: requests.Response object
            operation: Description of operation for error messages
            
        Returns:
            Parsed JSON response or raises exception
            
        Raises:
            PlaneAPIError: For API errors
        """
        try:
            response.raise_for_status()
            
            # Handle empty responses
            if response.status_code == 204 or not response.content:
                return None
            
            return response.json()
            
        except Exception as e:
            error_msg = f"{operation} failed: {str(e)}"
            
            # Try to get error details from response
            try:
                error_data = response.json()
                if isinstance(error_data, dict):
                    if 'detail' in error_data:
                        error_msg = f"{operation} failed: {error_data['detail']}"
                    elif 'error' in error_data:
                        error_msg = f"{operation} failed: {error_data['error']}"
            except:
                pass
            
            self.logger.error(error_msg, status_code=response.status_code)
            raise PlaneAPIError(error_msg)
    
    # ==================================================================
    # WORKSPACE METHODS
    # ==================================================================
    
    def get_workspace(self) -> Dict[str, Any]:
        """
        Get workspace information
        
        Returns:
            Workspace data
        """
        endpoint = self._build_endpoint('/workspaces/{workspace_slug}/')
        response = self.session.get(endpoint)
        return self._handle_response(response, "Get workspace")
    
    def get_workspace_members(self) -> List[Dict[str, Any]]:
        """
        Get workspace members
        
        Returns:
            List of workspace members
        """
        endpoint = self._build_endpoint('/workspaces/{workspace_slug}/members/')
        response = self.session.get(endpoint)
        return self._handle_response(response, "Get workspace members")
    
    # ==================================================================
    # PROJECT METHODS
    # ==================================================================
    
    def list_projects(self) -> List[Dict[str, Any]]:
        """
        List all projects in workspace
        
        Returns:
            List of projects
        """
        endpoint = self._build_endpoint('/workspaces/{workspace_slug}/projects/')
        response = self.session.get(endpoint)
        return self._handle_response(response, "List projects")
    
    def get_project(self, project_id: str) -> Dict[str, Any]:
        """
        Get project details
        
        Args:
            project_id: Project ID
            
        Returns:
            Project data
        """
        endpoint = self._build_endpoint(f'/workspaces/{{workspace_slug}}/projects/{project_id}/')
        response = self.session.get(endpoint)
        return self._handle_response(response, f"Get project {project_id}")
    
    def create_project(self, project_data: Dict[str, Any]) -> Dict[str, Any]:
        """
        Create a new project
        
        Args:
            project_data: Project attributes (name, description, etc.)
            
        Returns:
            Created project data
        """
        endpoint = self._build_endpoint('/workspaces/{workspace_slug}/projects/')
        response = self.session.post(endpoint, json=project_data)
        return self._handle_response(response, "Create project")
    
    def update_project(self, project_id: str, updates: Dict[str, Any]) -> Dict[str, Any]:
        """
        Update project
        
        Args:
            project_id: Project ID
            updates: Fields to update
            
        Returns:
            Updated project data
        """
        endpoint = self._build_endpoint(f'/workspaces/{{workspace_slug}}/projects/{project_id}/')
        response = self.session.patch(endpoint, json=updates)
        return self._handle_response(response, f"Update project {project_id}")
    
    def delete_project(self, project_id: str) -> None:
        """
        Delete/archive project
        
        Args:
            project_id: Project ID
        """
        endpoint = self._build_endpoint(f'/workspaces/{{workspace_slug}}/projects/{project_id}/')
        response = self.session.delete(endpoint)
        self._handle_response(response, f"Delete project {project_id}")
    
    # ==================================================================
    # ISSUE METHODS
    # ==================================================================
    
    def list_issues(
        self,
        project_id: Optional[str] = None,
        filters: Optional[Dict[str, Any]] = None
    ) -> List[Dict[str, Any]]:
        """
        List issues
        
        Args:
            project_id: Optional project ID to filter by
            filters: Optional query filters
            
        Returns:
            List of issues
        """
        if project_id:
            endpoint = self._build_endpoint(
                f'/workspaces/{{workspace_slug}}/projects/{project_id}/issues/'
            )
        else:
            # Workspace-level issues endpoint
            endpoint = self._build_endpoint('/workspaces/{workspace_slug}/issues/')
        
        params = filters or {}
        response = self.session.get(endpoint, params=params)
        return self._handle_response(response, "List issues")
    
    def get_issue(self, issue_id: str, project_id: Optional[str] = None) -> Dict[str, Any]:
        """
        Get issue details
        
        Args:
            issue_id: Issue ID
            project_id: Optional project ID (required for some PLANE versions)
            
        Returns:
            Issue data
        """
        if project_id:
            # Use project-scoped endpoint
            endpoint = self._build_endpoint(
                f'/workspaces/{{workspace_slug}}/projects/{project_id}/issues/{issue_id}/'
            )
        else:
            # Try workspace-level endpoint first
            endpoint = self._build_endpoint(f'/workspaces/{{workspace_slug}}/issues/{issue_id}/')
        
        response = self.session.get(endpoint)
        return self._handle_response(response, f"Get issue {issue_id}")
    
    def create_issue(
        self,
        project_id: str,
        issue_data: Dict[str, Any]
    ) -> Dict[str, Any]:
        """
        Create a new issue
        
        Args:
            project_id: Project ID
            issue_data: Issue attributes (name, description, etc.)
            
        Returns:
            Created issue data
        """
        endpoint = self._build_endpoint(
            f'/workspaces/{{workspace_slug}}/projects/{project_id}/issues/'
        )
        response = self.session.post(endpoint, json=issue_data)
        return self._handle_response(response, "Create issue")
    
    def update_issue(
        self,
        issue_id: str,
        updates: Dict[str, Any]
    ) -> Dict[str, Any]:
        """
        Update issue
        
        Args:
            issue_id: Issue ID
            updates: Fields to update
            
        Returns:
            Updated issue data
        """
        endpoint = self._build_endpoint(f'/workspaces/{{workspace_slug}}/issues/{issue_id}/')
        response = self.session.patch(endpoint, json=updates)
        return self._handle_response(response, f"Update issue {issue_id}")
    
    def delete_issue(self, issue_id: str) -> None:
        """
        Delete issue
        
        Args:
            issue_id: Issue ID
        """
        endpoint = self._build_endpoint(f'/workspaces/{{workspace_slug}}/issues/{issue_id}/')
        response = self.session.delete(endpoint)
        self._handle_response(response, f"Delete issue {issue_id}")
    
    # ==================================================================
    # COMMENT METHODS
    # ==================================================================
    
    def add_comment(
        self,
        issue_id: str,
        comment: str
    ) -> Dict[str, Any]:
        """
        Add comment to issue
        
        Args:
            issue_id: Issue ID
            comment: Comment text
            
        Returns:
            Created comment data
        """
        endpoint = self._build_endpoint(f'/workspaces/{{workspace_slug}}/issues/{issue_id}/comments/')
        response = self.session.post(endpoint, json={'comment': comment})
        return self._handle_response(response, f"Add comment to issue {issue_id}")
    
    def get_comments(self, issue_id: str) -> List[Dict[str, Any]]:
        """
        Get issue comments
        
        Args:
            issue_id: Issue ID
            
        Returns:
            List of comments
        """
        endpoint = self._build_endpoint(f'/workspaces/{{workspace_slug}}/issues/{issue_id}/comments/')
        response = self.session.get(endpoint)
        return self._handle_response(response, f"Get comments for issue {issue_id}")
    
    # ==================================================================
    # SEARCH METHODS
    # ==================================================================
    
    def search(self, query: str, filters: Optional[Dict[str, Any]] = None) -> Dict[str, Any]:
        """
        Search across workspace
        
        Args:
            query: Search query
            filters: Optional filters
            
        Returns:
            Search results
        """
        endpoint = self._build_endpoint('/workspaces/{workspace_slug}/search/')
        params = {'query': query}
        if filters:
            params.update(filters)
        
        response = self.session.get(endpoint, params=params)
        return self._handle_response(response, f"Search for '{query}'")
    
    # ==================================================================
    # USER METHODS
    # ==================================================================
    
    def get_current_user(self) -> Dict[str, Any]:
        """
        Get current authenticated user
        
        Returns:
            User data
        """
        response = self.session.get('/api/users/me/')
        return self._handle_response(response, "Get current user")
    
    def get_user_workspaces(self) -> List[Dict[str, Any]]:
        """
        Get user's workspaces
        
        Returns:
            List of workspaces
        """
        response = self.session.get('/api/users/me/workspaces/')
        return self._handle_response(response, "Get user workspaces")


class PlaneAPIError(Exception):
    """Exception raised for PLANE API errors"""
    pass

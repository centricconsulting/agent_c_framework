"""
PLANE Project Management Toolset

Tools for creating and managing PLANE projects.
"""

import yaml
from typing import Optional
from agent_c.toolsets import Toolset, json_schema
from ..client.plane_client import PlaneClient, PlaneAPIError
from ..auth.plane_session import PlaneSessionExpired


class PlaneProjectTools(Toolset):
    """Tools for managing PLANE projects"""
    
    def __init__(self, **kwargs):
        super().__init__(**kwargs, name='plane_projects', use_prefix=True)
        self.client: Optional[PlaneClient] = None
        self.base_url = "http://localhost"
        self.workspace_slug = "agent_c"
    
    async def post_init(self):
        """Initialize PLANE client after toolset creation"""
        try:
            self.client = PlaneClient(self.base_url, self.workspace_slug)
            self.logger.info(f"PLANE client initialized for workspace: {self.workspace_slug}")
        except Exception as e:
            self.logger.error(f"Failed to initialize PLANE client: {str(e)}")
            self.client = None
    
    def _ensure_client(self):
        """Ensure client is initialized"""
        if self.client is None:
            raise PlaneAPIError("PLANE client not initialized. Check authentication.")
    
    def _format_project(self, project: dict) -> str:
        """Format project data for display"""
        return yaml.dump({
            'id': project.get('id'),
            'name': project.get('name'),
            'identifier': project.get('identifier'),
            'description': project.get('description', 'No description'),
            'created_at': project.get('created_at'),
            'archived_at': project.get('archived_at'),
        }, allow_unicode=True, default_flow_style=False)
    
    @json_schema(
        description="List all projects in the PLANE workspace",
        params={}
    )
    async def plane_list_projects(self, **kwargs) -> str:
        """
        List all projects in the workspace
        
        Returns:
            YAML formatted list of projects
        """
        self._ensure_client()
        
        try:
            projects = await self.client.list_projects()
            
            if not projects:
                return "No projects found in workspace."
            
            # Format for display
            result = f"Found {len(projects)} project(s):\n\n"
            
            for proj in projects:
                result += f"**{proj.get('name')}** ({proj.get('identifier')})\n"
                result += f"  ID: {proj.get('id')}\n"
                result += f"  Description: {proj.get('description', 'No description')}\n"
                
                if proj.get('archived_at'):
                    result += f"  Status: Archived\n"
                else:
                    result += f"  Status: Active\n"
                
                result += "\n"
            
            return result
            
        except PlaneSessionExpired as e:
            return f"ERROR: {str(e)}"
        except PlaneAPIError as e:
            return f"ERROR: Failed to list projects: {str(e)}"
        except Exception as e:
            self.logger.error(f"Unexpected error listing projects: {str(e)}")
            return f"ERROR: Unexpected error: {str(e)}"
    
    @json_schema(
        description="Get detailed information about a specific PLANE project",
        params={
            "project_id": {
                "type": "string",
                "description": "The project ID or identifier",
                "required": True
            }
        }
    )
    async def plane_get_project(self, **kwargs) -> str:
        """
        Get project details
        
        Args:
            project_id: Project ID or identifier
            
        Returns:
            YAML formatted project details
        """
        self._ensure_client()
        project_id = kwargs.get("project_id")
        
        if not project_id:
            return "ERROR: project_id is required"
        
        try:
            project = await self.client.get_project(project_id)
            return self._format_project(project)
            
        except PlaneSessionExpired as e:
            return f"ERROR: {str(e)}"
        except PlaneAPIError as e:
            return f"ERROR: Failed to get project: {str(e)}"
        except Exception as e:
            self.logger.error(f"Unexpected error getting project: {str(e)}")
            return f"ERROR: Unexpected error: {str(e)}"
    
    @json_schema(
        description="Create a new project in PLANE workspace",
        params={
            "name": {
                "type": "string",
                "description": "Project name",
                "required": True
            },
            "identifier": {
                "type": "string",
                "description": "Project identifier (e.g., 'WEB' for Website project). 2-5 uppercase letters.",
                "required": True
            },
            "description": {
                "type": "string",
                "description": "Project description"
            }
        }
    )
    async def plane_create_project(self, **kwargs) -> str:
        """
        Create a new project
        
        Args:
            name: Project name
            identifier: Project identifier (2-5 uppercase letters)
            description: Optional project description
            
        Returns:
            Created project details
        """
        self._ensure_client()
        
        name = kwargs.get("name")
        identifier = kwargs.get("identifier")
        description = kwargs.get("description", "")
        
        if not name:
            return "ERROR: name is required"
        if not identifier:
            return "ERROR: identifier is required"
        
        # Validate identifier
        if not identifier.isupper():
            return "ERROR: identifier must be uppercase letters (e.g., 'WEB', 'API')"
        if len(identifier) < 2 or len(identifier) > 5:
            return "ERROR: identifier must be 2-5 characters long"
        
        try:
            project_data = {
                "name": name,
                "identifier": identifier,
                "description": description,
            }
            
            project = await self.client.create_project(project_data)
            
            result = f"✅ Project created successfully!\n\n"
            result += self._format_project(project)
            return result
            
        except PlaneSessionExpired as e:
            return f"ERROR: {str(e)}"
        except PlaneAPIError as e:
            return f"ERROR: Failed to create project: {str(e)}"
        except Exception as e:
            self.logger.error(f"Unexpected error creating project: {str(e)}")
            return f"ERROR: Unexpected error: {str(e)}"
    
    @json_schema(
        description="Update an existing PLANE project",
        params={
            "project_id": {
                "type": "string",
                "description": "Project ID",
                "required": True
            },
            "name": {
                "type": "string",
                "description": "New project name"
            },
            "description": {
                "type": "string",
                "description": "New project description"
            }
        }
    )
    async def plane_update_project(self, **kwargs) -> str:
        """
        Update project details
        
        Args:
            project_id: Project ID
            name: Optional new name
            description: Optional new description
            
        Returns:
            Updated project details
        """
        self._ensure_client()
        
        project_id = kwargs.get("project_id")
        if not project_id:
            return "ERROR: project_id is required"
        
        # Build updates dict
        updates = {}
        if "name" in kwargs:
            updates["name"] = kwargs["name"]
        if "description" in kwargs:
            updates["description"] = kwargs["description"]
        
        if not updates:
            return "ERROR: No updates provided. Specify name and/or description."
        
        try:
            project = await self.client.update_project(project_id, updates)
            
            result = f"✅ Project updated successfully!\n\n"
            result += self._format_project(project)
            return result
            
        except PlaneSessionExpired as e:
            return f"ERROR: {str(e)}"
        except PlaneAPIError as e:
            return f"ERROR: Failed to update project: {str(e)}"
        except Exception as e:
            self.logger.error(f"Unexpected error updating project: {str(e)}")
            return f"ERROR: Unexpected error: {str(e)}"
    
    @json_schema(
        description="Archive/delete a PLANE project",
        params={
            "project_id": {
                "type": "string",
                "description": "Project ID to archive",
                "required": True
            }
        }
    )
    async def plane_archive_project(self, **kwargs) -> str:
        """
        Archive a project
        
        Args:
            project_id: Project ID
            
        Returns:
            Confirmation message
        """
        self._ensure_client()
        
        project_id = kwargs.get("project_id")
        if not project_id:
            return "ERROR: project_id is required"
        
        try:
            # Get project name first for confirmation
            project = await self.client.get_project(project_id)
            project_name = project.get('name', 'Unknown')
            
            # Delete the project
            await self.client.delete_project(project_id)
            
            return f"✅ Project '{project_name}' (ID: {project_id}) has been archived."
            
        except PlaneSessionExpired as e:
            return f"ERROR: {str(e)}"
        except PlaneAPIError as e:
            return f"ERROR: Failed to archive project: {str(e)}"
        except Exception as e:
            self.logger.error(f"Unexpected error archiving project: {str(e)}")
            return f"ERROR: Unexpected error: {str(e)}"


# Register the toolset
Toolset.register(PlaneProjectTools)

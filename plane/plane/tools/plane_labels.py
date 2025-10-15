"""
PLANE Label Management Toolset

Tools for creating and managing issue labels for better organization.
"""

import yaml
from typing import Optional
from agent_c.toolsets import Toolset, json_schema
from ..client.plane_client import PlaneClient, PlaneAPIError
from ..auth.plane_session import PlaneSessionExpired


class PlaneLabelTools(Toolset):
    """Tools for managing PLANE labels"""
    
    def __init__(self, **kwargs):
        super().__init__(**kwargs, name='plane_labels', use_prefix=True)
        self.client: Optional[PlaneClient] = None
        self.base_url = "http://localhost"
        self.workspace_slug = "agent_c"
    
    async def post_init(self):
        """Initialize PLANE client after toolset creation"""
        try:
            self.client = PlaneClient(self.base_url, self.workspace_slug)
            self.logger.info(f"PLANE label tools initialized for workspace: {self.workspace_slug}")
        except Exception as e:
            self.logger.error(f"Failed to initialize PLANE client: {str(e)}")
            self.client = None
    
    def _ensure_client(self):
        """Ensure client is initialized"""
        if self.client is None:
            raise PlaneAPIError("PLANE client not initialized. Check authentication.")
    
    @json_schema(
        description="List all labels in the PLANE workspace",
        params={
            "project_id": {
                "type": "string",
                "description": "Optional project ID to get project-specific labels"
            }
        }
    )
    async def plane_list_labels(self, **kwargs) -> str:
        """
        List workspace or project labels
        
        Args:
            project_id: Optional project ID for project labels
            
        Returns:
            Formatted list of labels
        """
        self._ensure_client()
        
        project_id = kwargs.get("project_id")
        
        try:
            if project_id:
                endpoint = f"/api/workspaces/{self.workspace_slug}/projects/{project_id}/issue-labels/"
            else:
                endpoint = f"/api/workspaces/{self.workspace_slug}/labels/"
            
            response = self.client.session.get(endpoint)
            
            if response.status_code != 200:
                return f"ERROR: Failed to list labels (status {response.status_code})"
            
            labels = response.json()
            
            if not labels:
                return "No labels found."
            
            result = f"Found {len(labels)} label(s):\n\n"
            
            for label in labels:
                color = label.get('color', '#000000')
                result += f"• **{label.get('name')}**\n"
                result += f"  ID: {label.get('id')}\n"
                result += f"  Color: {color}\n"
                if label.get('description'):
                    result += f"  Description: {label.get('description')}\n"
                result += "\n"
            
            return result
            
        except PlaneSessionExpired as e:
            return f"ERROR: {str(e)}"
        except Exception as e:
            self.logger.error(f"Unexpected error listing labels: {str(e)}")
            return f"ERROR: Unexpected error: {str(e)}"
    
    @json_schema(
        description="Create a new label in PLANE workspace",
        params={
            "name": {
                "type": "string",
                "description": "Label name",
                "required": True
            },
            "color": {
                "type": "string",
                "description": "Label color (hex code, e.g., #FF5733). Default: random color"
            },
            "description": {
                "type": "string",
                "description": "Label description"
            }
        }
    )
    async def plane_create_label(self, **kwargs) -> str:
        """
        Create a new label
        
        Args:
            name: Label name
            color: Hex color code
            description: Label description
            
        Returns:
            Created label details
        """
        self._ensure_client()
        
        name = kwargs.get("name")
        if not name:
            return "ERROR: name is required"
        
        label_data = {
            "name": name,
            "color": kwargs.get("color", "#808080"),  # Default gray
            "description": kwargs.get("description", ""),
        }
        
        try:
            endpoint = f"/api/workspaces/{self.workspace_slug}/labels/"
            response = self.client.session.post(endpoint, json=label_data)
            
            if response.status_code not in [200, 201]:
                error = response.json() if response.content else {}
                return f"ERROR: Failed to create label: {error}"
            
            label = response.json()
            
            result = f"✅ Label created successfully!\n\n"
            result += f"**{label.get('name')}**\n"
            result += f"ID: {label.get('id')}\n"
            result += f"Color: {label.get('color')}\n"
            
            return result
            
        except PlaneSessionExpired as e:
            return f"ERROR: {str(e)}"
        except Exception as e:
            self.logger.error(f"Unexpected error creating label: {str(e)}")
            return f"ERROR: Unexpected error: {str(e)}"
    
    @json_schema(
        description="Add a label to a PLANE issue",
        params={
            "issue_id": {
                "type": "string",
                "description": "Issue ID",
                "required": True
            },
            "label_id": {
                "type": "string",
                "description": "Label ID to add",
                "required": True
            }
        }
    )
    async def plane_add_label_to_issue(self, **kwargs) -> str:
        """
        Add label to an issue
        
        Args:
            issue_id: Issue ID
            label_id: Label ID
            
        Returns:
            Confirmation message
        """
        self._ensure_client()
        
        issue_id = kwargs.get("issue_id")
        label_id = kwargs.get("label_id")
        
        if not issue_id:
            return "ERROR: issue_id is required"
        if not label_id:
            return "ERROR: label_id is required"
        
        try:
            # Get current issue to get its labels
            issue = self.client.get_issue(issue_id)
            current_labels = issue.get('labels', [])
            
            # Add new label if not already present
            if label_id not in current_labels:
                updated_labels = current_labels + [label_id]
                
                # Update issue with new labels
                self.client.update_issue(issue_id, {"labels": updated_labels})
                return f"✅ Label added to issue {issue_id}"
            else:
                return f"ℹ️  Label already exists on issue {issue_id}"
            
        except PlaneSessionExpired as e:
            return f"ERROR: {str(e)}"
        except Exception as e:
            self.logger.error(f"Unexpected error adding label: {str(e)}")
            return f"ERROR: Unexpected error: {str(e)}"
    
    @json_schema(
        description="Remove a label from a PLANE issue",
        params={
            "issue_id": {
                "type": "string",
                "description": "Issue ID",
                "required": True
            },
            "label_id": {
                "type": "string",
                "description": "Label ID to remove",
                "required": True
            }
        }
    )
    async def plane_remove_label_from_issue(self, **kwargs) -> str:
        """
        Remove label from an issue
        
        Args:
            issue_id: Issue ID
            label_id: Label ID
            
        Returns:
            Confirmation message
        """
        self._ensure_client()
        
        issue_id = kwargs.get("issue_id")
        label_id = kwargs.get("label_id")
        
        if not issue_id:
            return "ERROR: issue_id is required"
        if not label_id:
            return "ERROR: label_id is required"
        
        try:
            # Get current issue labels
            issue = self.client.get_issue(issue_id)
            current_labels = issue.get('labels', [])
            
            # Remove label if present
            if label_id in current_labels:
                updated_labels = [l for l in current_labels if l != label_id]
                
                # Update issue
                self.client.update_issue(issue_id, {"labels": updated_labels})
                return f"✅ Label removed from issue {issue_id}"
            else:
                return f"ℹ️  Label not found on issue {issue_id}"
            
        except PlaneSessionExpired as e:
            return f"ERROR: {str(e)}"
        except Exception as e:
            self.logger.error(f"Unexpected error removing label: {str(e)}")
            return f"ERROR: Unexpected error: {str(e)}"
    
    @json_schema(
        description="Delete a label from the PLANE workspace",
        params={
            "label_id": {
                "type": "string",
                "description": "Label ID to delete",
                "required": True
            }
        }
    )
    async def plane_delete_label(self, **kwargs) -> str:
        """
        Delete a label
        
        Args:
            label_id: Label ID
            
        Returns:
            Confirmation message
        """
        self._ensure_client()
        
        label_id = kwargs.get("label_id")
        if not label_id:
            return "ERROR: label_id is required"
        
        try:
            endpoint = f"/api/workspaces/{self.workspace_slug}/labels/{label_id}/"
            response = self.client.session.delete(endpoint)
            
            if response.status_code in [200, 204]:
                return f"✅ Label {label_id} deleted successfully"
            else:
                return f"ERROR: Failed to delete label (status {response.status_code})"
            
        except PlaneSessionExpired as e:
            return f"ERROR: {str(e)}"
        except Exception as e:
            self.logger.error(f"Unexpected error deleting label: {str(e)}")
            return f"ERROR: Unexpected error: {str(e)}"


# Register the toolset
Toolset.register(PlaneLabelTools)

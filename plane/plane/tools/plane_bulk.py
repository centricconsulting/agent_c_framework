"""
PLANE Bulk Operations Toolset

Tools for performing bulk operations on multiple issues at once.
"""

from typing import Optional, List
from agent_c.toolsets import Toolset, json_schema
from ..client.plane_client import PlaneClient, PlaneAPIError
from ..auth.plane_session import PlaneSessionExpired


class PlaneBulkTools(Toolset):
    """Tools for bulk operations on PLANE issues"""
    
    def __init__(self, **kwargs):
        super().__init__(**kwargs, name='plane_bulk', use_prefix=True)
        self.client: Optional[PlaneClient] = None
        self.base_url = "http://localhost"
        self.workspace_slug = "agent_c"
    
    async def post_init(self):
        """Initialize PLANE client after toolset creation"""
        try:
            self.client = PlaneClient(self.base_url, self.workspace_slug)
            self.logger.info(f"PLANE bulk tools initialized for workspace: {self.workspace_slug}")
        except Exception as e:
            self.logger.error(f"Failed to initialize PLANE client: {str(e)}")
            self.client = None
    
    def _ensure_client(self):
        """Ensure client is initialized"""
        if self.client is None:
            raise PlaneAPIError("PLANE client not initialized. Check authentication.")
    
    @json_schema(
        description="Update multiple PLANE issues at once with the same changes",
        params={
            "issue_ids": {
                "type": "array",
                "description": "List of issue IDs to update",
                "items": {"type": "string"},
                "required": True
            },
            "updates": {
                "type": "object",
                "description": "Fields to update (e.g., {\"priority\": \"high\", \"state_id\": \"...\"} )",
                "required": True
            }
        }
    )
    async def plane_bulk_update_issues(self, **kwargs) -> str:
        """
        Update multiple issues with same changes
        
        Args:
            issue_ids: List of issue IDs
            updates: Dictionary of fields to update
            
        Returns:
            Summary of bulk update operation
        """
        self._ensure_client()
        
        issue_ids = kwargs.get("issue_ids", [])
        updates = kwargs.get("updates", {})
        
        if not issue_ids:
            return "ERROR: issue_ids is required and must be a non-empty list"
        if not updates:
            return "ERROR: updates is required and must be a non-empty dictionary"
        
        if not isinstance(issue_ids, list):
            return "ERROR: issue_ids must be a list"
        if not isinstance(updates, dict):
            return "ERROR: updates must be a dictionary"
        
        try:
            success_count = 0
            failed_count = 0
            errors = []
            
            for issue_id in issue_ids:
                try:
                    self.client.update_issue(issue_id, updates)
                    success_count += 1
                except Exception as e:
                    failed_count += 1
                    errors.append(f"  • {issue_id}: {str(e)[:50]}")
            
            result = f"**Bulk Update Results:**\n\n"
            result += f"✅ Successfully updated: {success_count} issue(s)\n"
            
            if failed_count > 0:
                result += f"❌ Failed: {failed_count} issue(s)\n\n"
                result += "Errors:\n"
                result += "\n".join(errors[:5])  # Limit errors shown
                if len(errors) > 5:
                    result += f"\n  ... and {len(errors) - 5} more errors"
            
            return result
            
        except PlaneSessionExpired as e:
            return f"ERROR: {str(e)}"
        except Exception as e:
            self.logger.error(f"Unexpected error in bulk update: {str(e)}")
            return f"ERROR: Unexpected error: {str(e)}"
    
    @json_schema(
        description="Assign multiple PLANE issues to a user at once",
        params={
            "issue_ids": {
                "type": "array",
                "description": "List of issue IDs to assign",
                "items": {"type": "string"},
                "required": True
            },
            "assignee_id": {
                "type": "string",
                "description": "User ID to assign issues to",
                "required": True
            }
        }
    )
    async def plane_bulk_assign_issues(self, **kwargs) -> str:
        """
        Assign multiple issues to a user
        
        Args:
            issue_ids: List of issue IDs
            assignee_id: User ID to assign to
            
        Returns:
            Summary of bulk assign operation
        """
        self._ensure_client()
        
        issue_ids = kwargs.get("issue_ids", [])
        assignee_id = kwargs.get("assignee_id")
        
        if not issue_ids:
            return "ERROR: issue_ids is required"
        if not assignee_id:
            return "ERROR: assignee_id is required"
        
        updates = {"assignees": [assignee_id]}
        
        return await self.plane_bulk_update_issues(
            issue_ids=issue_ids,
            updates=updates
        )
    
    @json_schema(
        description="Change the state of multiple PLANE issues at once",
        params={
            "issue_ids": {
                "type": "array",
                "description": "List of issue IDs to update",
                "items": {"type": "string"},
                "required": True
            },
            "state_id": {
                "type": "string",
                "description": "State ID to set",
                "required": True
            }
        }
    )
    async def plane_bulk_change_state(self, **kwargs) -> str:
        """
        Change state of multiple issues
        
        Args:
            issue_ids: List of issue IDs
            state_id: New state ID
            
        Returns:
            Summary of bulk state change
        """
        self._ensure_client()
        
        issue_ids = kwargs.get("issue_ids", [])
        state_id = kwargs.get("state_id")
        
        if not issue_ids:
            return "ERROR: issue_ids is required"
        if not state_id:
            return "ERROR: state_id is required"
        
        updates = {"state": state_id}
        
        return await self.plane_bulk_update_issues(
            issue_ids=issue_ids,
            updates=updates
        )
    
    @json_schema(
        description="Change the priority of multiple PLANE issues at once",
        params={
            "issue_ids": {
                "type": "array",
                "description": "List of issue IDs to update",
                "items": {"type": "string"},
                "required": True
            },
            "priority": {
                "type": "string",
                "description": "Priority to set: urgent, high, medium, low, or none",
                "required": True
            }
        }
    )
    async def plane_bulk_change_priority(self, **kwargs) -> str:
        """
        Change priority of multiple issues
        
        Args:
            issue_ids: List of issue IDs
            priority: New priority level
            
        Returns:
            Summary of bulk priority change
        """
        self._ensure_client()
        
        issue_ids = kwargs.get("issue_ids", [])
        priority = kwargs.get("priority", "").lower()
        
        if not issue_ids:
            return "ERROR: issue_ids is required"
        if not priority:
            return "ERROR: priority is required"
        
        # Validate priority
        valid_priorities = ["urgent", "high", "medium", "low", "none"]
        if priority not in valid_priorities:
            return f"ERROR: priority must be one of: {', '.join(valid_priorities)}"
        
        updates = {"priority": priority}
        
        return await self.plane_bulk_update_issues(
            issue_ids=issue_ids,
            updates=updates
        )
    
    @json_schema(
        description="Add the same label to multiple PLANE issues at once",
        params={
            "issue_ids": {
                "type": "array",
                "description": "List of issue IDs",
                "items": {"type": "string"},
                "required": True
            },
            "label_id": {
                "type": "string",
                "description": "Label ID to add to all issues",
                "required": True
            }
        }
    )
    async def plane_bulk_add_label(self, **kwargs) -> str:
        """
        Add same label to multiple issues
        
        Args:
            issue_ids: List of issue IDs
            label_id: Label ID to add
            
        Returns:
            Summary of bulk label operation
        """
        self._ensure_client()
        
        issue_ids = kwargs.get("issue_ids", [])
        label_id = kwargs.get("label_id")
        
        if not issue_ids:
            return "ERROR: issue_ids is required"
        if not label_id:
            return "ERROR: label_id is required"
        
        try:
            success_count = 0
            failed_count = 0
            
            for issue_id in issue_ids:
                try:
                    # Get current labels
                    issue = self.client.get_issue(issue_id)
                    current_labels = issue.get('labels', [])
                    
                    # Add label if not present
                    if label_id not in current_labels:
                        updated_labels = current_labels + [label_id]
                        self.client.update_issue(issue_id, {"labels": updated_labels})
                    
                    success_count += 1
                except:
                    failed_count += 1
            
            result = f"**Bulk Label Add Results:**\n\n"
            result += f"✅ Successfully added label to: {success_count} issue(s)\n"
            
            if failed_count > 0:
                result += f"❌ Failed: {failed_count} issue(s)\n"
            
            return result
            
        except PlaneSessionExpired as e:
            return f"ERROR: {str(e)}"
        except Exception as e:
            self.logger.error(f"Unexpected error in bulk label add: {str(e)}")
            return f"ERROR: Unexpected error: {str(e)}"


# Register the toolset
Toolset.register(PlaneBulkTools)

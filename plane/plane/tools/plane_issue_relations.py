"""
PLANE Issue Relations & Advanced Features Toolset

Tools for managing issue relationships, sub-issues, and advanced formatting.
"""

import yaml
from typing import Optional
from agent_c.toolsets import Toolset, json_schema
from ..client.plane_client import PlaneClient, PlaneAPIError
from ..auth.plane_session import PlaneSessionExpired


class PlaneIssueRelationsTools(Toolset):
    """Tools for managing issue relationships and sub-issues"""
    
    def __init__(self, **kwargs):
        super().__init__(**kwargs, name='plane_issue_relations', use_prefix=True)
        self.client: Optional[PlaneClient] = None
        self.base_url = "http://localhost"
        self.workspace_slug = "agent_c"
    
    async def post_init(self):
        """Initialize PLANE client after toolset creation"""
        try:
            self.client = PlaneClient(self.base_url, self.workspace_slug)
            self.logger.info(f"PLANE issue relations tools initialized for workspace: {self.workspace_slug}")
        except Exception as e:
            self.logger.error(f"Failed to initialize PLANE client: {str(e)}")
            self.client = None
    
    def _ensure_client(self):
        """Ensure client is initialized"""
        if self.client is None:
            raise PlaneAPIError("PLANE client not initialized. Check authentication.")
    
    @json_schema(
        description="Create a sub-issue (child issue) under a parent issue in PLANE",
        params={
            "project_id": {
                "type": "string",
                "description": "Project ID",
                "required": True
            },
            "parent_issue_id": {
                "type": "string",
                "description": "Parent issue ID",
                "required": True
            },
            "name": {
                "type": "string",
                "description": "Sub-issue title",
                "required": True
            },
            "description": {
                "type": "string",
                "description": "Sub-issue description (supports markdown)"
            },
            "priority": {
                "type": "string",
                "description": "Priority: urgent, high, medium, low, or none"
            }
        }
    )
    async def plane_create_sub_issue(self, **kwargs) -> str:
        """
        Create a sub-issue under a parent issue
        
        Args:
            project_id: Project ID
            parent_issue_id: Parent issue ID
            name: Sub-issue title
            description: Sub-issue description
            priority: Priority level
            
        Returns:
            Created sub-issue details
        """
        self._ensure_client()
        
        project_id = kwargs.get("project_id")
        parent_issue_id = kwargs.get("parent_issue_id")
        name = kwargs.get("name")
        
        if not project_id:
            return "ERROR: project_id is required"
        if not parent_issue_id:
            return "ERROR: parent_issue_id is required"
        if not name:
            return "ERROR: name is required"
        
        # Build issue data with parent
        issue_data = {
            "name": name,
            "description": kwargs.get("description", ""),
            "parent": parent_issue_id,  # This makes it a sub-issue!
        }
        
        if "priority" in kwargs:
            priority = kwargs["priority"].lower()
            if priority not in ["urgent", "high", "medium", "low", "none"]:
                return "ERROR: priority must be one of: urgent, high, medium, low, none"
            issue_data["priority"] = priority
        
        try:
            issue = self.client.create_issue(project_id, issue_data)
            
            result = f"✅ Sub-issue created successfully under parent {parent_issue_id}!\n\n"
            result += f"**{issue.get('name')}**\n"
            result += f"ID: {issue.get('id')}\n"
            result += f"Parent: {parent_issue_id}\n"
            result += f"Priority: {issue.get('priority', 'none')}\n"
            
            return result
            
        except PlaneSessionExpired as e:
            return f"ERROR: {str(e)}"
        except PlaneAPIError as e:
            return f"ERROR: Failed to create sub-issue: {str(e)}"
        except Exception as e:
            self.logger.error(f"Unexpected error creating sub-issue: {str(e)}")
            return f"ERROR: Unexpected error: {str(e)}"
    
    @json_schema(
        description="Get sub-issues (child issues) for a parent issue",
        params={
            "project_id": {
                "type": "string",
                "description": "Project ID",
                "required": True
            },
            "issue_id": {
                "type": "string",
                "description": "Parent issue ID",
                "required": True
            }
        }
    )
    async def plane_get_sub_issues(self, **kwargs) -> str:
        """
        Get sub-issues for a parent issue
        
        Args:
            project_id: Project ID
            issue_id: Parent issue ID
            
        Returns:
            List of sub-issues
        """
        self._ensure_client()
        
        project_id = kwargs.get("project_id")
        issue_id = kwargs.get("issue_id")
        
        if not project_id:
            return "ERROR: project_id is required"
        if not issue_id:
            return "ERROR: issue_id is required"
        
        try:
            endpoint = f"/api/workspaces/{self.workspace_slug}/projects/{project_id}/issues/{issue_id}/sub-issues/"
            response = self.client.session.get(endpoint)
            
            if response.status_code != 200:
                return f"ERROR: Failed to get sub-issues (status {response.status_code})"
            
            data = response.json()
            sub_issues = data.get('sub_issues', [])
            
            if not sub_issues:
                return "No sub-issues found for this issue."
            
            result = f"Found {len(sub_issues)} sub-issue(s):\n\n"
            
            for sub in sub_issues:
                result += f"• **{sub.get('name', 'Untitled')}**\n"
                result += f"  ID: {sub.get('id')}\n"
                result += f"  State: {sub.get('state_detail', {}).get('name', 'N/A')}\n"
                result += f"  Priority: {sub.get('priority', 'none')}\n"
                result += "\n"
            
            # Show state distribution
            state_dist = data.get('state_distribution', {})
            if state_dist:
                result += "**State Distribution:**\n"
                for state, count in state_dist.items():
                    result += f"  {state}: {count}\n"
            
            return result
            
        except PlaneSessionExpired as e:
            return f"ERROR: {str(e)}"
        except Exception as e:
            self.logger.error(f"Unexpected error getting sub-issues: {str(e)}")
            return f"ERROR: Unexpected error: {str(e)}"
    
    @json_schema(
        description="Add a relationship between two PLANE issues (blocks, blocked_by, duplicate, relates_to)",
        params={
            "project_id": {
                "type": "string",
                "description": "Project ID",
                "required": True
            },
            "issue_id": {
                "type": "string",
                "description": "Source issue ID",
                "required": True
            },
            "related_issue_id": {
                "type": "string",
                "description": "Related issue ID",
                "required": True
            },
            "relation_type": {
                "type": "string",
                "description": "Relation type: blocking, blocked_by, duplicate, relates_to, or start_after",
                "required": True
            }
        }
    )
    async def plane_add_issue_relation(self, **kwargs) -> str:
        """
        Add a relationship between two issues
        
        Args:
            project_id: Project ID
            issue_id: Source issue ID
            related_issue_id: Related issue ID
            relation_type: Type of relation (blocking, blocked_by, duplicate, relates_to, start_after)
            
        Returns:
            Confirmation message
        """
        self._ensure_client()
        
        project_id = kwargs.get("project_id")
        issue_id = kwargs.get("issue_id")
        related_issue_id = kwargs.get("related_issue_id")
        relation_type = kwargs.get("relation_type")
        
        if not all([project_id, issue_id, related_issue_id, relation_type]):
            return "ERROR: project_id, issue_id, related_issue_id, and relation_type are all required"
        
        # Validate relation type
        valid_types = ["blocking", "blocked_by", "duplicate", "relates_to", "start_after"]
        if relation_type not in valid_types:
            return f"ERROR: relation_type must be one of: {', '.join(valid_types)}"
        
        try:
            endpoint = f"/api/workspaces/{self.workspace_slug}/projects/{project_id}/issues/{issue_id}/issue-relation/"
            
            payload = {
                "related_issue": related_issue_id,
                "relation_type": relation_type
            }
            
            response = self.client.session.post(endpoint, json=payload)
            
            if response.status_code in [200, 201]:
                return f"✅ Relation added: Issue {issue_id} {relation_type} {related_issue_id}"
            else:
                error = response.json() if response.content else {}
                return f"ERROR: Failed to add relation (status {response.status_code}): {error}"
            
        except PlaneSessionExpired as e:
            return f"ERROR: {str(e)}"
        except Exception as e:
            self.logger.error(f"Unexpected error adding relation: {str(e)}")
            return f"ERROR: Unexpected error: {str(e)}"
    
    @json_schema(
        description="Get all relationships for a PLANE issue",
        params={
            "project_id": {
                "type": "string",
                "description": "Project ID",
                "required": True
            },
            "issue_id": {
                "type": "string",
                "description": "Issue ID",
                "required": True
            }
        }
    )
    async def plane_get_issue_relations(self, **kwargs) -> str:
        """
        Get all relationships for an issue
        
        Args:
            project_id: Project ID
            issue_id: Issue ID
            
        Returns:
            Issue relationships
        """
        self._ensure_client()
        
        project_id = kwargs.get("project_id")
        issue_id = kwargs.get("issue_id")
        
        if not project_id:
            return "ERROR: project_id is required"
        if not issue_id:
            return "ERROR: issue_id is required"
        
        try:
            endpoint = f"/api/workspaces/{self.workspace_slug}/projects/{project_id}/issues/{issue_id}/issue-relation/"
            response = self.client.session.get(endpoint)
            
            if response.status_code != 200:
                return f"ERROR: Failed to get relations (status {response.status_code})"
            
            relations = response.json()
            
            result = "**Issue Relations:**\n\n"
            
            has_relations = False
            
            if relations.get('blocking'):
                has_relations = True
                result += f"🚫 **Blocking ({len(relations['blocking'])}):**\n"
                for rel in relations['blocking']:
                    result += f"  • {rel.get('issue_detail', {}).get('name', 'Unknown')} (ID: {rel.get('related_issue')})\n"
                result += "\n"
            
            if relations.get('blocked_by'):
                has_relations = True
                result += f"⛔ **Blocked By ({len(relations['blocked_by'])}):**\n"
                for rel in relations['blocked_by']:
                    result += f"  • {rel.get('issue_detail', {}).get('name', 'Unknown')} (ID: {rel.get('related_issue')})\n"
                result += "\n"
            
            if relations.get('duplicate'):
                has_relations = True
                result += f"👥 **Duplicates ({len(relations['duplicate'])}):**\n"
                for rel in relations['duplicate']:
                    result += f"  • {rel.get('issue_detail', {}).get('name', 'Unknown')} (ID: {rel.get('related_issue')})\n"
                result += "\n"
            
            if relations.get('relates_to'):
                has_relations = True
                result += f"🔗 **Related To ({len(relations['relates_to'])}):**\n"
                for rel in relations['relates_to']:
                    result += f"  • {rel.get('issue_detail', {}).get('name', 'Unknown')} (ID: {rel.get('related_issue')})\n"
                result += "\n"
            
            if relations.get('start_after'):
                has_relations = True
                result += f"⏭️ **Starts After ({len(relations['start_after'])}):**\n"
                for rel in relations['start_after']:
                    result += f"  • {rel.get('issue_detail', {}).get('name', 'Unknown')} (ID: {rel.get('related_issue')})\n"
                result += "\n"
            
            if not has_relations:
                return "No relations found for this issue."
            
            return result
            
        except PlaneSessionExpired as e:
            return f"ERROR: {str(e)}"
        except Exception as e:
            self.logger.error(f"Unexpected error getting relations: {str(e)}")
            return f"ERROR: Unexpected error: {str(e)}"
    
    @json_schema(
        description="Remove a relationship between two PLANE issues",
        params={
            "project_id": {
                "type": "string",
                "description": "Project ID",
                "required": True
            },
            "issue_id": {
                "type": "string",
                "description": "Source issue ID",
                "required": True
            },
            "related_issue_id": {
                "type": "string",
                "description": "Related issue ID to remove",
                "required": True
            }
        }
    )
    async def plane_remove_issue_relation(self, **kwargs) -> str:
        """
        Remove a relationship between issues
        
        Args:
            project_id: Project ID
            issue_id: Source issue ID
            related_issue_id: Related issue ID to remove
            
        Returns:
            Confirmation message
        """
        self._ensure_client()
        
        project_id = kwargs.get("project_id")
        issue_id = kwargs.get("issue_id")
        related_issue_id = kwargs.get("related_issue_id")
        
        if not all([project_id, issue_id, related_issue_id]):
            return "ERROR: project_id, issue_id, and related_issue_id are all required"
        
        try:
            endpoint = f"/api/workspaces/{self.workspace_slug}/projects/{project_id}/issues/{issue_id}/issue-relation/{related_issue_id}/"
            response = self.client.session.delete(endpoint)
            
            if response.status_code in [200, 204]:
                return f"✅ Relation removed between issues {issue_id} and {related_issue_id}"
            else:
                return f"ERROR: Failed to remove relation (status {response.status_code})"
            
        except PlaneSessionExpired as e:
            return f"ERROR: {str(e)}"
        except Exception as e:
            self.logger.error(f"Unexpected error removing relation: {str(e)}")
            return f"ERROR: Unexpected error: {str(e)}"
    
    @json_schema(
        description="Format text with markdown for PLANE issue descriptions using common patterns (headings, lists, checklists, code, etc.)",
        params={
            "content": {
                "type": "string",
                "description": "The text content to format",
                "required": True
            },
            "format_type": {
                "type": "string",
                "description": "Format type: heading1, heading2, heading3, list, checklist, code, quote, bold, italic",
                "required": True
            }
        }
    )
    async def plane_format_markdown(self, **kwargs) -> str:
        """
        Format text with markdown (like PLANE's slash commands)
        
        Args:
            content: Text to format
            format_type: Type of formatting to apply
            
        Returns:
            Markdown-formatted text
        """
        content = kwargs.get("content", "")
        format_type = kwargs.get("format_type", "").lower()
        
        if not content:
            return "ERROR: content is required"
        if not format_type:
            return "ERROR: format_type is required"
        
        # Apply formatting based on type
        if format_type == "heading1" or format_type == "h1":
            result = f"# {content}\n"
            
        elif format_type == "heading2" or format_type == "h2":
            result = f"## {content}\n"
            
        elif format_type == "heading3" or format_type == "h3":
            result = f"### {content}\n"
            
        elif format_type == "list" or format_type == "bullet":
            lines = content.split('\n')
            result = '\n'.join([f"- {line.strip()}" for line in lines if line.strip()])
            result += '\n'
            
        elif format_type == "checklist" or format_type == "todo":
            lines = content.split('\n')
            result = '\n'.join([f"- [ ] {line.strip()}" for line in lines if line.strip()])
            result += '\n'
            
        elif format_type == "code":
            result = f"```\n{content}\n```\n"
            
        elif format_type == "quote":
            lines = content.split('\n')
            result = '\n'.join([f"> {line}" for line in lines])
            result += '\n'
            
        elif format_type == "bold":
            result = f"**{content}**"
            
        elif format_type == "italic":
            result = f"*{content}*"
            
        else:
            return f"ERROR: Unknown format_type '{format_type}'. Valid types: heading1, heading2, heading3, list, checklist, code, quote, bold, italic"
        
        return f"Formatted markdown:\n\n```markdown\n{result}```\n\nYou can use this in issue descriptions!"


# Register the toolset
Toolset.register(PlaneIssueRelationsTools)

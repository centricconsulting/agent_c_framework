from typing import Any, Dict, List, Optional, Union, cast
from datetime import datetime
import json
import yaml

from agent_c.toolsets.tool_set import Toolset
from agent_c.toolsets.json_schema import json_schema
from agent_c_tools.tools.workspace_sequential_thinking.prompt import WorkspaceSequentialThinkingSection
from agent_c_tools.tools.workspace_sequential_thinking.models import (
    ThoughtModel, ThoughtBranchModel, SequentialThinkingModel
)
from agent_c_tools.tools.workspace.tool import WorkspaceTools


class WorkspaceSequentialThinkingTools(Toolset):
    """
    WorkspaceSequentialThinkingTools provides methods for structured thinking through complex problems
    using the metadata of a workspace for persistent storage.
    """

    def __init__(self, **kwargs: Any):
        super().__init__(name='wst', **kwargs)
        self.section = WorkspaceSequentialThinkingSection()
        self.workspace_tool: Optional[WorkspaceTools] = None

    async def post_init(self):
        self.workspace_tool = cast(WorkspaceTools, self.tool_chest.available_tools.get('WorkspaceTools'))
    
    def _parse_thinking_path(self, thinking_path: str) -> tuple[str, str]:
        """Parse a thinking path into workspace name and thinking ID."""
        if not thinking_path.startswith("//"):
            raise ValueError(f"Invalid thinking path format: {thinking_path}. Must start with //")
        
        parts = thinking_path.split("/")
        if len(parts) < 3:
            raise ValueError(f"Invalid thinking path format: {thinking_path}. Format should be //workspace/thinking_id")
        
        workspace_name = parts[2]
        thinking_id = "/".join(parts[3:]) if len(parts) > 3 else "default"
        
        return workspace_name, thinking_id
    
    async def _get_thinking_meta(self, workspace_name: str) -> Dict[str, Any]:
        """Get the sequential thinking metadata dictionary for a workspace."""
        if not self.workspace_tool:
            raise RuntimeError("WorkspaceTools not available")
        
        thinking_meta = await self.workspace_tool.read_meta_value(workspace=workspace_name, key="_sequential_thinking")
        return thinking_meta or {}
    
    async def _save_thinking_meta(self, workspace_name: str, thinking_meta: Dict[str, Any]) -> None:
        """Save the sequential thinking metadata dictionary for a workspace."""
        if not self.workspace_tool:
            raise RuntimeError("WorkspaceTools not available")
        
        await self.workspace_tool.write_meta_value(workspace=workspace_name, key="_sequential_thinking", value=thinking_meta)
    
    async def _get_thinking(self, thinking_path: str) -> Optional[SequentialThinkingModel]:
        """Get a sequential thinking process by its path."""
        workspace_name, thinking_id = self._parse_thinking_path(thinking_path)
        thinking_meta = await self._get_thinking_meta(workspace_name)
        
        if thinking_id not in thinking_meta:
            return None
        
        # Convert the JSON dict back to a SequentialThinkingModel
        try:
            return SequentialThinkingModel.model_validate(thinking_meta[thinking_id])
        except Exception as e:
            self._log.error(f"Error deserializing sequential thinking: {e}")
            return None
    
    async def _save_thinking(self, thinking_path: str, thinking: SequentialThinkingModel) -> None:
        """Save a sequential thinking process to its path."""
        workspace_name, thinking_id = self._parse_thinking_path(thinking_path)
        thinking_meta = await self._get_thinking_meta(workspace_name)
        
        # Update the updated_at timestamp
        thinking.updated_at = datetime.now()
        
        # Convert the SequentialThinkingModel to a dict for storage
        thinking_meta[thinking_id] = thinking.model_dump()
        await self._save_thinking_meta(workspace_name, thinking_meta)
    
    def _format_thought(self, thought: ThoughtModel) -> str:
        """Format a thought for display."""
        context = ""
        
        if thought.is_revision:
            prefix = "🔄 Revision"
            context = f" (revising thought {thought.revises_thought})"
        elif thought.branch_from_thought:
            prefix = "🌿 Branch"
            context = f" (from thought {thought.branch_from_thought}, ID: {thought.branch_id})"
        else:
            prefix = "💭 Thought"
        
        header = f"{prefix} {thought.thought_number}/{thought.total_thoughts}{context}"
        return header + "\n" + thought.thought
    
    @json_schema(
        description="Create a new sequential thinking process in a workspace",
        params={
            "thinking_path": {
                "type": "string",
                "description": "Path to the thinking process in the format //workspace/thinking_id",
                "required": True
            },
            "title": {
                "type": "string",
                "description": "Title of the sequential thinking process",
                "required": True
            },
            "description": {
                "type": "string",
                "description": "Description of the sequential thinking process"
            }
        }
    )
    async def create_thinking(self, **kwargs) -> str:
        """Create a new sequential thinking process in the specified workspace."""
        thinking_path = kwargs.get("thinking_path")
        title = kwargs.get("title")
        description = kwargs.get("description", "")
        
        if not thinking_path:
            return "ERROR: thinking_path parameter is required"
        if not title:
            return "ERROR: title parameter is required"
        
        try:
            workspace_name, thinking_id = self._parse_thinking_path(thinking_path)
            thinking_meta = await self._get_thinking_meta(workspace_name)
            
            if thinking_id in thinking_meta:
                return f"ERROR: Sequential thinking process with ID '{thinking_id}' already exists"
            
            new_thinking = SequentialThinkingModel(title=title, description=description)
            await self._save_thinking(thinking_path, new_thinking)
            
            result = {
                "success": True,
                "thinking": new_thinking.model_dump()
            }
            return yaml.dump(result, allow_unicode=True)
        except Exception as e:
            return f"ERROR: {str(e)}"
    
    @json_schema(
        description="List all sequential thinking processes in a workspace",
        params={
            "workspace": {
                "type": "string",
                "description": "Name of the workspace",
                "required": True
            }
        }
    )
    async def list_thinking(self, **kwargs) -> str:
        """List all sequential thinking processes in the specified workspace."""
        workspace = kwargs.get("workspace")
        
        if not workspace:
            return "ERROR: workspace parameter is required"
        
        try:
            thinking_meta = await self._get_thinking_meta(workspace)
            
            thinking_list = []
            for thinking_id, thinking_data in thinking_meta.items():
                thinking_list.append({
                    "id": thinking_id,
                    "title": thinking_data.get("title", ""),
                    "description": thinking_data.get("description", ""),
                    "created_at": thinking_data.get("created_at", ""),
                    "updated_at": thinking_data.get("updated_at", ""),
                    "thought_count": len(thinking_data.get("thought_history", []))
                })
            
            result = {
                "success": True,
                "thinking_processes": thinking_list
            }
            return yaml.dump(result, allow_unicode=True)
        except Exception as e:
            return f"ERROR: {str(e)}"
    
    @json_schema(
        description="Get details of a sequential thinking process",
        params={
            "thinking_path": {
                "type": "string",
                "description": "Path to the thinking process in the format //workspace/thinking_id",
                "required": True
            }
        }
    )
    async def get_thinking(self, **kwargs) -> str:
        """Get details of a sequential thinking process."""
        thinking_path = kwargs.get("thinking_path")
        
        if not thinking_path:
            return "ERROR: thinking_path parameter is required"
        
        try:
            thinking = await self._get_thinking(thinking_path)
            
            if not thinking:
                return f"ERROR: Sequential thinking process not found at path: {thinking_path}"
            
            result = {
                "success": True,
                "thinking": thinking.model_dump()
            }
            return yaml.dump(result, allow_unicode=True)
        except Exception as e:
            return f"ERROR: {str(e)}"
    
    @json_schema(
        description="Add a thought to a sequential thinking process",
        params={
            "thinking_path": {
                "type": "string",
                "description": "Path to the thinking process in the format //workspace/thinking_id",
                "required": True
            },
            "thought": {
                "type": "string",
                "description": "The content of the thought",
                "required": True
            },
            "thought_number": {
                "type": "integer",
                "description": "The number of this thought in the sequence",
                "required": True
            },
            "total_thoughts": {
                "type": "integer",
                "description": "The estimated total number of thoughts",
                "required": True
            },
            "next_thought_needed": {
                "type": "boolean",
                "description": "Whether another thought is needed",
                "required": True
            },
            "is_revision": {
                "type": "boolean",
                "description": "Whether this thought revises a previous thought"
            },
            "revises_thought": {
                "type": "integer",
                "description": "The thought number being revised (if is_revision is true)"
            },
            "branch_from_thought": {
                "type": "integer",
                "description": "The thought number to branch from (if creating a new branch)"
            },
            "branch_id": {
                "type": "string",
                "description": "The ID of the branch (if part of a branch)"
            },
            "needs_more_thoughts": {
                "type": "boolean",
                "description": "Whether more thoughts are needed despite reaching the estimated total"
            }
        }
    )
    async def add_thought(self, **kwargs) -> str:
        """Add a thought to a sequential thinking process."""
        thinking_path = kwargs.get("thinking_path")
        thought = kwargs.get("thought")
        thought_number = kwargs.get("thought_number")
        total_thoughts = kwargs.get("total_thoughts")
        next_thought_needed = kwargs.get("next_thought_needed")
        is_revision = kwargs.get("is_revision", False)
        revises_thought = kwargs.get("revises_thought")
        branch_from_thought = kwargs.get("branch_from_thought")
        branch_id = kwargs.get("branch_id")
        needs_more_thoughts = kwargs.get("needs_more_thoughts", False)
        
        if not thinking_path:
            return "ERROR: thinking_path parameter is required"
        if not thought:
            return "ERROR: thought parameter is required"
        if thought_number is None:
            return "ERROR: thought_number parameter is required"
        if total_thoughts is None:
            return "ERROR: total_thoughts parameter is required"
        if next_thought_needed is None:
            return "ERROR: next_thought_needed parameter is required"
        
        try:
            thinking = await self._get_thinking(thinking_path)
            
            if not thinking:
                return f"ERROR: Sequential thinking process not found at path: {thinking_path}"
            
            # Validate thought data
            if is_revision and revises_thought is None:
                return "ERROR: revises_thought must be provided when is_revision is true"
            
            if branch_from_thought is not None and branch_id is None:
                return "ERROR: branch_id must be provided when branch_from_thought is specified"
            
            # Create new thought
            new_thought = ThoughtModel(
                thought=thought,
                thought_number=thought_number,
                total_thoughts=total_thoughts,
                next_thought_needed=next_thought_needed,
                is_revision=is_revision,
                revises_thought=revises_thought,
                branch_from_thought=branch_from_thought,
                branch_id=branch_id,
                needs_more_thoughts=needs_more_thoughts
            )
            
            # Handle branch creation if needed
            if branch_from_thought is not None and branch_id is not None:
                # Create branch if it doesn't exist
                if branch_id not in thinking.branches:
                    # Find the parent thought ID
                    parent_thought_id = None
                    for tid, t in thinking.thoughts.items():
                        if t.thought_number == branch_from_thought:
                            parent_thought_id = tid
                            break
                    
                    if parent_thought_id is None:
                        return f"ERROR: Parent thought number {branch_from_thought} not found"
                    
                    # Create the branch
                    branch_name = f"Branch from thought {branch_from_thought}"
                    new_branch = ThoughtBranchModel(
                        id=branch_id,
                        name=branch_name,
                        description=f"Alternative thinking path branching from thought {branch_from_thought}",
                        parent_thought_id=parent_thought_id
                    )
                    thinking.branches[branch_id] = new_branch
                
                # Add the thought to the branch
                thinking.branches[branch_id].thought_ids.append(new_thought.id)
            
            # Add thought to thinking process
            thinking.thoughts[new_thought.id] = new_thought
            thinking.thought_history.append(new_thought.id)
            
            await self._save_thinking(thinking_path, thinking)
            
            # Format the thought for display
            formatted_thought = self._format_thought(new_thought)
            
            result = {
                "success": True,
                "thought": new_thought.model_dump(),
                "formatted_thought": formatted_thought,
                "thinking": {
                    "thought_count": len(thinking.thought_history),
                    "branches": list(thinking.branches.keys()),
                    "next_thought_needed": new_thought.next_thought_needed
                }
            }
            return yaml.dump(result, allow_unicode=True)
        except Exception as e:
            return f"ERROR: {str(e)}"
    
    @json_schema(
        description="Get a specific thought by number",
        params={
            "thinking_path": {
                "type": "string",
                "description": "Path to the thinking process in the format //workspace/thinking_id",
                "required": True
            },
            "thought_number": {
                "type": "integer",
                "description": "The number of the thought to retrieve",
                "required": True
            },
            "branch_id": {
                "type": "string",
                "description": "The ID of the branch (if retrieving a thought from a branch)"
            }
        }
    )
    async def get_thought(self, **kwargs) -> str:
        """Get a specific thought by its number."""
        thinking_path = kwargs.get("thinking_path")
        thought_number = kwargs.get("thought_number")
        branch_id = kwargs.get("branch_id")
        
        if not thinking_path:
            return "ERROR: thinking_path parameter is required"
        if thought_number is None:
            return "ERROR: thought_number parameter is required"
        
        try:
            thinking = await self._get_thinking(thinking_path)
            
            if not thinking:
                return f"ERROR: Sequential thinking process not found at path: {thinking_path}"
            
            # Get thoughts to search in (either main sequence or branch)
            thought_ids = []
            if branch_id:
                if branch_id not in thinking.branches:
                    return f"ERROR: Branch with ID '{branch_id}' not found"
                thought_ids = thinking.branches[branch_id].thought_ids
            else:
                thought_ids = thinking.thought_history
            
            # Find the thought with the matching number
            for thought_id in thought_ids:
                thought = thinking.thoughts.get(thought_id)
                if thought and thought.thought_number == thought_number:
                    result = {
                        "success": True,
                        "thought": thought.model_dump(),
                        "formatted_thought": self._format_thought(thought)
                    }
                    return yaml.dump(result, allow_unicode=True)
            
            return f"ERROR: Thought number {thought_number} not found"
        except Exception as e:
            return f"ERROR: {str(e)}"
    
    @json_schema(
        description="List all thoughts in a sequential thinking process",
        params={
            "thinking_path": {
                "type": "string",
                "description": "Path to the thinking process in the format //workspace/thinking_id",
                "required": True
            },
            "branch_id": {
                "type": "string",
                "description": "The ID of the branch to list thoughts from (if listing a branch)"
            },
            "include_content": {
                "type": "boolean",
                "description": "Whether to include the full thought content"
            }
        }
    )
    async def list_thoughts(self, **kwargs) -> str:
        """List all thoughts in a sequential thinking process."""
        thinking_path = kwargs.get("thinking_path")
        branch_id = kwargs.get("branch_id")
        include_content = kwargs.get("include_content", False)
        
        if not thinking_path:
            return "ERROR: thinking_path parameter is required"
        
        try:
            thinking = await self._get_thinking(thinking_path)
            
            if not thinking:
                return f"ERROR: Sequential thinking process not found at path: {thinking_path}"
            
            # Get thoughts to list (either main sequence or branch)
            thought_ids = []
            if branch_id:
                if branch_id not in thinking.branches:
                    return f"ERROR: Branch with ID '{branch_id}' not found"
                thought_ids = thinking.branches[branch_id].thought_ids
            else:
                thought_ids = thinking.thought_history
            
            # Build the list of thoughts
            thoughts_list = []
            for thought_id in thought_ids:
                thought = thinking.thoughts.get(thought_id)
                if thought:
                    thought_data = {
                        "id": thought.id,
                        "thought_number": thought.thought_number,
                        "total_thoughts": thought.total_thoughts,
                        "next_thought_needed": thought.next_thought_needed,
                        "is_revision": thought.is_revision,
                        "revises_thought": thought.revises_thought,
                        "branch_from_thought": thought.branch_from_thought,
                        "branch_id": thought.branch_id,
                        "created_at": thought.created_at
                    }
                    if include_content:
                        thought_data["thought"] = thought.thought
                        thought_data["formatted_thought"] = self._format_thought(thought)
                    thoughts_list.append(thought_data)
            
            result = {
                "success": True,
                "thoughts": thoughts_list,
                "branch_id": branch_id
            }
            return yaml.dump(result, allow_unicode=True)
        except Exception as e:
            return f"ERROR: {str(e)}"
    
    @json_schema(
        description="List all branches in a sequential thinking process",
        params={
            "thinking_path": {
                "type": "string",
                "description": "Path to the thinking process in the format //workspace/thinking_id",
                "required": True
            }
        }
    )
    async def list_branches(self, **kwargs) -> str:
        """List all branches in a sequential thinking process."""
        thinking_path = kwargs.get("thinking_path")
        
        if not thinking_path:
            return "ERROR: thinking_path parameter is required"
        
        try:
            thinking = await self._get_thinking(thinking_path)
            
            if not thinking:
                return f"ERROR: Sequential thinking process not found at path: {thinking_path}"
            
            branches_list = []
            for branch_id, branch in thinking.branches.items():
                branches_list.append({
                    "id": branch.id,
                    "name": branch.name,
                    "description": branch.description,
                    "parent_thought_id": branch.parent_thought_id,
                    "thought_count": len(branch.thought_ids),
                    "created_at": branch.created_at
                })
            
            result = {
                "success": True,
                "branches": branches_list
            }
            return yaml.dump(result, allow_unicode=True)
        except Exception as e:
            return f"ERROR: {str(e)}"
    
    @json_schema(
        description="Create a new branch in a sequential thinking process",
        params={
            "thinking_path": {
                "type": "string",
                "description": "Path to the thinking process in the format //workspace/thinking_id",
                "required": True
            },
            "parent_thought_number": {
                "type": "integer",
                "description": "The thought number to branch from",
                "required": True
            },
            "name": {
                "type": "string",
                "description": "Name of the branch",
                "required": True
            },
            "description": {
                "type": "string",
                "description": "Description of the branch"
            }
        }
    )
    async def create_branch(self, **kwargs) -> str:
        """Create a new branch in a sequential thinking process."""
        thinking_path = kwargs.get("thinking_path")
        parent_thought_number = kwargs.get("parent_thought_number")
        name = kwargs.get("name")
        description = kwargs.get("description", "")
        
        if not thinking_path:
            return "ERROR: thinking_path parameter is required"
        if parent_thought_number is None:
            return "ERROR: parent_thought_number parameter is required"
        if not name:
            return "ERROR: name parameter is required"
        
        try:
            thinking = await self._get_thinking(thinking_path)
            
            if not thinking:
                return f"ERROR: Sequential thinking process not found at path: {thinking_path}"
            
            # Find the parent thought
            parent_thought_id = None
            for tid, thought in thinking.thoughts.items():
                if thought.thought_number == parent_thought_number:
                    parent_thought_id = tid
                    break
            
            if parent_thought_id is None:
                return f"ERROR: Parent thought number {parent_thought_number} not found"
            
            # Create the branch
            new_branch = ThoughtBranchModel(
                name=name,
                description=description,
                parent_thought_id=parent_thought_id
            )
            
            thinking.branches[new_branch.id] = new_branch
            await self._save_thinking(thinking_path, thinking)
            
            result = {
                "success": True,
                "branch": new_branch.model_dump()
            }
            return yaml.dump(result, allow_unicode=True)
        except Exception as e:
            return f"ERROR: {str(e)}"
    
    @json_schema(
        description="Delete a sequential thinking process",
        params={
            "thinking_path": {
                "type": "string",
                "description": "Path to the thinking process in the format //workspace/thinking_id",
                "required": True
            }
        }
    )
    async def delete_thinking(self, **kwargs) -> str:
        """Delete a sequential thinking process."""
        thinking_path = kwargs.get("thinking_path")
        
        if not thinking_path:
            return "ERROR: thinking_path parameter is required"
        
        try:
            workspace_name, thinking_id = self._parse_thinking_path(thinking_path)
            thinking_meta = await self._get_thinking_meta(workspace_name)
            
            if thinking_id not in thinking_meta:
                return f"ERROR: Sequential thinking process not found at path: {thinking_path}"
            
            # Remove the thinking process
            del thinking_meta[thinking_id]
            await self._save_thinking_meta(workspace_name, thinking_meta)
            
            result = {
                "success": True,
                "message": f"Sequential thinking process at {thinking_path} has been deleted"
            }
            return yaml.dump(result, allow_unicode=True)
        except Exception as e:
            return f"ERROR: {str(e)}"


Toolset.register(WorkspaceSequentialThinkingTools, required_tools=['WorkspaceTools'])
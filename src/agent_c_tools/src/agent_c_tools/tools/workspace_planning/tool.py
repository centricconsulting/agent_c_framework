import yaml

from datetime import datetime
from typing import Any, Dict, List, Optional, cast

from agent_c.toolsets.tool_set import Toolset
from agent_c.util.uncish_path import UNCishPath
from agent_c.toolsets.json_schema import json_schema
from agent_c_tools.tools.workspace.tool import WorkspaceTools
from agent_c_tools.helpers.validate_kwargs import validate_required_fields
from agent_c_tools.tools.workspace_planning.prompt import WorkspacePlanSection
from agent_c_tools.tools.workspace_planning.html_converter import PlanHTMLConverter
from agent_c_tools.helpers.path_helper import ensure_file_extension, os_file_system_path
from agent_c_tools.tools.workspace_planning.models import PlanModel, TaskModel, LessonLearnedModel, PriorityType


class WorkspacePlanningTools(Toolset):
    """
    WorkspacePlanningTools provides methods for creating and tracking plans using the metadata of a workspace.
    """

    def __init__(self, **kwargs: Any):
        super().__init__(name='wsp', **kwargs)
        self.section = WorkspacePlanSection()
        self.workspace_tool: Optional[WorkspaceTools] = None

    async def post_init(self):
        """Post-initialization to set up required tools.
          This method is called by the toolchest after the main init is finished
          to allow for async calls in tools that need them.
        """
        self.workspace_tool = cast(WorkspaceTools, self.tool_chest.available_tools.get('WorkspaceTools'))

    @staticmethod
    def _format_response(success: bool, **additional_data) -> str:
        """Return consistent YAML response format."""
        response = {"success": success}
        response.update(additional_data)
        return yaml.dump(response, default_flow_style=False, sort_keys=False, allow_unicode=True)

    async def _get_plans_meta(self, workspace_name: str) -> Dict[str, Any]:
        """Get the plans metadata dictionary for a workspace."""
        if not self.workspace_tool:
            raise RuntimeError("WorkspaceTools not available")

        error, workspace, key = self.workspace_tool.validate_and_get_workspace_path(f"//{workspace_name}/_kg")

        if error is not None:
            raise ValueError(f"Invalid workspace path: {workspace_name}. Error: {error}")

        plans_meta = await workspace.safe_metadata("_plans")
        return plans_meta or {}

    async def _save_plans_meta(self, workspace_name: str, plans_meta: Dict[str, Any]) -> None:
        """Save the plans metadata dictionary for a workspace."""
        if not self.workspace_tool:
            raise RuntimeError("WorkspaceTools not available")

        error, workspace, key = self.workspace_tool.validate_and_get_workspace_path(f"//{workspace_name}/_kg")
        if error is not None:
            raise ValueError(f"Invalid workspace path: {workspace_name}. Error: {error}")

        val = await workspace.safe_metadata_write("_plans", plans_meta)
        await workspace.save_metadata()
        return val

    async def _get_plan(self, plan_path: str) -> Optional[PlanModel]:
        """Get a plan by its path."""
        try:
            path = UNCishPath(plan_path)
            plan_id = path.path
        except ValueError:
            return None

        plans_meta = await self._get_plans_meta(path.source)

        if plan_id not in plans_meta:
            return None

        try:
            return PlanModel.model_validate(plans_meta[plan_id])
        except Exception as e:
            self.logger.error(f"Error deserializing plan: {e}")
            return None

    async def _save_plan(self, plan_path: str, plan: PlanModel) -> None:
        """Save a plan to its path."""
        path = UNCishPath(plan_path)
        plan_id = path.path

        plans_meta = await self._get_plans_meta(path.source)

        # Update the plan's updated_at timestamp
        plan.updated_at = datetime.now()

        # Convert the PlanModel to a dict for storage
        plans_meta[plan_id] = plan.model_dump()
        await self._save_plans_meta(path.source, plans_meta)

    @json_schema(
        description="Create a new plan in a workspace",
        params={
            "plan_path": {
                "type": "string",
                "description": "Path to the plan in the format //workspace/plan_id",
                "required": True
            },
            "title": {
                "type": "string",
                "description": "Title of the plan",
                "required": True
            },
            "description": {
                "type": "string",
                "description": "Description of the plan"
            }
        }
    )
    async def create_plan(self, **kwargs) -> str:
        """Create a new plan in the specified workspace."""
        plan_path = kwargs.get('plan_path')
        title = kwargs.get('title')
        description = kwargs.get('description', "")

        if not plan_path:
            return "Error: plan_path is required"
        if not title:
            return "Error: title is required"

        try:
            path = UNCishPath(plan_path)
            plan_id = path.path
        except ValueError:
            return f"Error: Invalid plan path format: {plan_path}. Must start with //"

        plans_meta = await self._get_plans_meta(path.source)

        if plan_id in plans_meta:
            return f"Plan with ID '{plan_id}' already exists"

        new_plan = PlanModel(title=title, description=description)
        await self._save_plan(plan_path, new_plan)

        return f"Plan '{title}' created successfully with ID '{plan_id}'"

    @json_schema(
        description="List all plans in a workspace",
        params={
            "workspace": {
                "type": "string",
                "description": "Name of the workspace",
                "required": True
            }
        }
    )
    async def list_plans(self, **kwargs) -> str:
        """List all plans in the specified workspace."""
        workspace = kwargs.get('workspace')

        if not workspace:
            return "Error: workspace is required"

        plans_meta = await self._get_plans_meta(workspace)

        plans_list = []
        for plan_id, plan_data in plans_meta.items():
            plans_list.append({
                "id": plan_id,
                "title": plan_data.get("title", ""),
                "description": plan_data.get("description", ""),
                "created_at": plan_data.get("created_at", ""),
                "updated_at": plan_data.get("updated_at", "")
            })

        return yaml.dump({"plans": plans_list}, default_flow_style=False, sort_keys=False, allow_unicode=True)

    @json_schema(
        description="Get details of a plan",
        params={
            "plan_path": {
                "type": "string",
                "description": "Path to the plan in the format //workspace/plan_id",
                "required": True
            }
        }
    )
    async def get_plan(self, **kwargs) -> str:
        """Get details of a plan."""
        plan_path = kwargs.get('plan_path')

        if not plan_path:
            return "Error: plan_path is required"

        plan = await self._get_plan(plan_path)

        if not plan:
            return f"Plan not found at path: {plan_path}"

        return yaml.dump({"plan": plan.model_dump()}, default_flow_style=False, sort_keys=False, allow_unicode=True)

    @json_schema(
        description="Create a new task in a plan",
        params={
            "plan_path": {
                "type": "string",
                "description": "Path to the plan in the format //workspace/plan_id",
                "required": True
            },
            "title": {
                "type": "string",
                "description": "Title of the task",
                "required": True
            },
            "description": {
                "type": "string",
                "description": "Description of the task"
            },
            "priority": {
                "type": "string",
                "description": "Priority of the task: low, medium, or high",
                "enum": ["low", "medium", "high"]
            },
            "parent_id": {
                "type": "string",
                "description": "ID of the parent task if this is a subtask"
            },
            "context": {
                "type": "string",
                "description": "Additional context for the task"
            },
            "sequence": {
                "type": "integer",
                "description": "Optional sequence number for controlling display order (lower numbers appear first)"
            }
        }
    )
    async def create_task(self, **kwargs) -> str:
        """Create a new task in the specified plan."""
        plan_path = kwargs.get('plan_path')
        title = kwargs.get('title')
        description = kwargs.get('description', "")
        priority = kwargs.get('priority', "medium")
        parent_id = kwargs.get('parent_id')
        context = kwargs.get('context', "")
        sequence = kwargs.get('sequence')

        if not plan_path:
            return "Error: plan_path is required"
        if not title:
            return "Error: title is required"

        plan = await self._get_plan(plan_path)

        if not plan:
            return f"Plan not found at path: {plan_path}"

        # Validate parent task if provided
        if parent_id and parent_id not in plan.tasks:
            return f"Parent task with ID '{parent_id}' not found"

        # Create the new task
        new_task = TaskModel(
            title=title,
            description=description,
            priority=priority,
            parent_id=parent_id,
            context=context,
            sequence=sequence
        )

        # Add to parent's child tasks if applicable
        if parent_id:
            parent_task = plan.tasks[parent_id]
            parent_task.child_tasks.append(new_task.id)

        # Add to plan's tasks
        plan.tasks[new_task.id] = new_task
        await self._save_plan(plan_path, plan)

        return f"Task '{title}' created successfully with ID '{new_task.id}'"

    @json_schema(
        description="Update an existing task",
        params={
            "plan_path": {
                "type": "string",
                "description": "Path to the plan in the format //workspace/plan_id",
                "required": True
            },
            "task_id": {
                "type": "string",
                "description": "ID of the task to update",
                "required": True
            },
            "title": {
                "type": "string",
                "description": "New title for the task"
            },
            "description": {
                "type": "string",
                "description": "New description for the task"
            },
            "priority": {
                "type": "string",
                "description": "New priority for the task: low, medium, or high",
                "enum": ["low", "medium", "high"]
            },
            "completed": {
                "type": "boolean",
                "description": "Mark the task as completed"
            },
            "context": {
                "type": "string",
                "description": "New context for the task"
            },
            "sequence": {
                "type": "integer",
                "description": "New sequence number for controlling display order"
            }
        }
    )
    async def update_task(self, **kwargs) -> str:
        """Update an existing task in the specified plan."""
        plan_path = kwargs.get('plan_path')
        task_id = kwargs.get('task_id')
        title = kwargs.get('title')
        description = kwargs.get('description')
        priority = kwargs.get('priority')
        completed = kwargs.get('completed')
        context = kwargs.get('context')
        sequence = kwargs.get('sequence')

        if not plan_path:
            return "Error: plan_path is required"
        if not task_id:
            return "Error: task_id is required"

        plan = await self._get_plan(plan_path)

        if not plan:
            return f"Plan not found at path: {plan_path}"

        if task_id not in plan.tasks:
            return f"Task with ID '{task_id}' not found in the plan"

        task = plan.tasks[task_id]

        # Update task properties if provided
        if title is not None:
            task.title = title
        if description is not None:
            task.description = description
        if priority is not None:
            task.priority = priority
        if completed is not None:
            task.completed = completed
        if context is not None:
            task.context = context
        if sequence is not None:
            task.sequence = sequence

        # Update the task's updated_at timestamp
        task.updated_at = datetime.now()

        await self._save_plan(plan_path, plan)

        return f"Task '{task_id}' updated successfully"

    @json_schema(
        description="Get details of a task",
        params={
            "plan_path": {
                "type": "string",
                "description": "Path to the plan in the format //workspace/plan_id",
                "required": True
            },
            "task_id": {
                "type": "string",
                "description": "ID of the task to get",
                "required": True
            }
        }
    )
    async def get_task(self, **kwargs) -> str:
        """Get details of a task."""
        plan_path = kwargs.get('plan_path')
        task_id = kwargs.get('task_id')

        if not plan_path:
            return "Error: plan_path is required"
        if not task_id:
            return "Error: task_id is required"

        plan = await self._get_plan(plan_path)

        if not plan:
            return f"Plan not found at path: {plan_path}"

        if task_id not in plan.tasks:
            return f"Task with ID '{task_id}' not found in the plan"

        task = plan.tasks[task_id]

        return yaml.dump({"task": task.model_dump()}, default_flow_style=False, sort_keys=False, allow_unicode=True)

    @json_schema(
        description="List tasks in a plan",
        params={
            "plan_path": {
                "type": "string",
                "description": "Path to the plan in the format //workspace/plan_id",
                "required": True
            },
            "parent_id": {
                "type": "string",
                "description": "ID of the parent task to list subtasks for. If not provided, lists root tasks."
            }
        }
    )
    async def list_tasks(self, **kwargs) -> str:
        """List tasks in a plan, optionally filtered by parent task."""
        plan_path = kwargs.get('plan_path')
        parent_id = kwargs.get('parent_id')

        if not plan_path:
            return "Error: plan_path is required"

        plan = await self._get_plan(plan_path)

        if not plan:
            return f"Plan not found at path: {plan_path}"

        tasks_list = []

        for task_id, task in plan.tasks.items():
            # If parent_id is provided, filter by parent_id
            # If parent_id is None, get root tasks (tasks with no parent)
            if (parent_id and task.parent_id == parent_id) or (parent_id is None and not task.parent_id):
                tasks_list.append(task.model_dump())

        return yaml.dump({"tasks": tasks_list}, default_flow_style=False, sort_keys=False, allow_unicode=True)

    @json_schema(
        description="Add a lesson learned to a plan",
        params={
            "plan_path": {
                "type": "string",
                "description": "Path to the plan in the format //workspace/plan_id",
                "required": True
            },
            "lesson": {
                "type": "string",
                "description": "Lesson learned text",
                "required": True
            },
            "learned_task_id": {
                "type": "string",
                "description": "ID of the task associated with this lesson",
                "required": True
            }
        }
    )
    async def add_lesson_learned(self, **kwargs) -> str:
        """Add a lesson learned to a plan."""
        plan_path = kwargs.get('plan_path')
        lesson = kwargs.get('lesson')
        learned_task_id = kwargs.get('learned_task_id')

        if not plan_path:
            return "Error: plan_path is required"
        if not lesson:
            return "Error: lesson is required"
        if not learned_task_id:
            return "Error: learned_task_id is required"

        plan = await self._get_plan(plan_path)

        if not plan:
            return f"Plan not found at path: {plan_path}"

        if learned_task_id not in plan.tasks:
            return f"Task with ID '{learned_task_id}' not found in the plan"

        new_lesson = LessonLearnedModel(lesson=lesson, learned_task_id=learned_task_id)

        plan.lessons_learned.append(new_lesson)
        await self._save_plan(plan_path, plan)
        return yaml.dump({"lesson": new_lesson.model_dump()}, default_flow_style=False, sort_keys=False,
                         allow_unicode=True)

    @json_schema(
        description="List lessons learned in a plan",
        params={
            "plan_path": {
                "type": "string",
                "description": "Path to the plan in the format //workspace/plan_id",
                "required": True
            },
            "task_id": {
                "type": "string",
                "description": "ID of the task to filter lessons by. If not provided, lists all lessons."
            }
        }
    )
    async def list_lessons_learned(self, **kwargs) -> str:
        """List lessons learned in a plan, optionally filtered by task."""
        plan_path = kwargs.get('plan_path')
        task_id = kwargs.get('task_id')

        if not plan_path:
            return "Error: plan_path is required"

        plan = await self._get_plan(plan_path)

        if not plan:
            return f"Plan not found at path: {plan_path}"

        lessons_list = []

        for lesson in plan.lessons_learned:
            # If task_id is provided, filter by task_id
            if task_id is None or lesson.learned_task_id == task_id:
                lessons_list.append(lesson.model_dump())

        return yaml.dump({"lessons": lessons_list}, default_flow_style=False, sort_keys=False, allow_unicode=True)

    @json_schema(
        description="Delete a task and its subtasks",
        params={
            "plan_path": {
                "type": "string",
                "description": "Path to the plan in the format //workspace/plan_id",
                "required": True
            },
            "task_id": {
                "type": "string",
                "description": "ID of the task to delete",
                "required": True
            }
        }
    )
    async def delete_task(self, **kwargs) -> str:
        """Delete a task and all its subtasks from a plan."""
        plan_path = kwargs.get('plan_path')
        task_id = kwargs.get('task_id')

        if not plan_path:
            return "Error: plan_path is required"
        if not task_id:
            return "Error: task_id is required"

        plan = await self._get_plan(plan_path)

        if not plan:
            return f"Plan not found at path: {plan_path}"

        if task_id not in plan.tasks:
            return f"Task with ID '{task_id}' not found in the plan"

        # Get the parent_id BEFORE deleting the task - otherwise when the task is removed, I can't see its parent.
        task_to_delete = plan.tasks.get(task_id)
        parent_id = task_to_delete.parent_id if task_to_delete else None

        # Delete the task and its subtasks
        deleted_tasks = self._delete_task_recursive(plan, task_id)

        # If the task had a parent, remove it from the parent's child_tasks
        if parent_id and parent_id in plan.tasks:
            parent_task = plan.tasks[parent_id]
            if task_id in parent_task.child_tasks:
                parent_task.child_tasks.remove(task_id)

        await self._save_plan(plan_path, plan)

        return f"{len(deleted_tasks)} task(s) deleted successfully"

    def _delete_task_recursive(self, plan: PlanModel, task_id: str) -> List[str]:
        """Recursively delete a task and all its subtasks."""
        deleted_tasks = [task_id]

        # Get the task
        task = plan.tasks.get(task_id)
        if not task:
            return deleted_tasks

        # Recursively delete all child tasks
        for child_id in list(task.child_tasks):  # Make a copy to avoid modifying while iterating
            deleted_tasks.extend(self._delete_task_recursive(plan, child_id))

        # Remove the task from the plan
        del plan.tasks[task_id]

        return deleted_tasks

    @json_schema(
        description="Export a plan to a markdown or html report file in the .scratch folder or specified location",
        params={
            "plan_path": {
                "type": "string",
                "description": "Path to the plan in the format //workspace/plan_id",
                "required": True
            },
            "output_path": {
                "type": "string",
                "description": "Optional output path. If not provided, saves to //workspace/.scratch/plan_id_report.md",
                "required": False
            },
            "format": {
                "type": "string",
                "description": "Format of the report: 'md' or 'html'. Defaults to 'md'.",
                "enum": ["md", "html"],
                "default": "md"
            }
        }
    )
    async def export_plan_report(self, **kwargs) -> str:
        """Export a plan to a markdown report file."""

        # validate required fields
        success, message = validate_required_fields(kwargs=kwargs, required_fields=['plan_path'])
        if not success:
            return f"Error: {message}"

        # extract parameters
        plan_path = kwargs.get('plan_path')
        output_path = kwargs.get('output_path')
        report_format = kwargs.get('format', 'md').lower()

        try:
            # Parse the plan path to get workspace name and plan ID
            # then load the plan
            try:
                path = UNCishPath(plan_path)
                plan_id = path.path
            except ValueError:
                return f"Error: Invalid plan path format: {plan_path}. Must start with //"

            plan = await self._get_plan(plan_path)

            if not plan:
                return f"Plan not found at path: {plan_path}"

            if not output_path:
                output_path = f"//{path.source}/.scratch/{plan_id}_report.{report_format}"
                output_path = ensure_file_extension(output_path, report_format)

            if report_format == 'md':
                # Generate the markdown report
                report_content = self._generate_plan_markdown(plan, path.source, plan_id)
            elif report_format == 'html':
                yaml_data = {
                    '_plans': {
                        plan_id: plan.model_dump()
                    },
                    'current_plan': plan_id
                }
                yaml_content = yaml.dump(yaml_data, default_flow_style=False, sort_keys=False, allow_unicode=True)
                converter = PlanHTMLConverter()
                report_content = converter.convert_plan_to_html(yaml_content, plan_id)
            else:
                return f"Error: Unsupported format '{report_format}'. Use 'md' or 'html'."

            # write to file
            result = await self.workspace_tool.write(
                path=output_path,
                data=report_content
            )
            # Check if write was successful
            if result.startswith('{"error":'):
                return f"Error writing report: {result}"

            file_system_path = os_file_system_path(self.workspace_tool, output_path)
            await self._raise_render_media(
                sent_by_class=self.__class__.__name__,
                sent_by_function='export_plan_report',
                content_type="text/html",
                content=f"<p>Interactive HTML plan report created: <a href='file://{file_system_path}' target='_blank'>{output_path}</a></p>",
                tool_context=kwargs.get('tool_context', {})
            )

            return f"Plan report exported successfully to: {output_path}"


        except Exception as e:
            return f"Error exporting plan report: {str(e)}"

    def _generate_plan_markdown(self, plan: PlanModel, workspace_name: str, plan_id: str) -> str:
        """Generate markdown content for a plan report."""
        lines = []

        # Header
        lines.append(f"# {plan.title}")
        lines.append("")

        # Overview
        if plan.description:
            lines.append("## Overview")
            lines.append("")
            lines.append(plan.description)
            lines.append("")

        # Plan metadata
        lines.append("## Plan Information")
        lines.append("")
        lines.append(f"- **Workspace:** {workspace_name}")
        lines.append(f"- **Plan ID:** {plan_id}")
        lines.append(f"- **Created:** {plan.created_at}")
        lines.append(f"- **Last Updated:** {plan.updated_at}")

        # Task statistics
        total_tasks = len(plan.tasks)
        completed_tasks = sum(1 for task in plan.tasks.values() if task.completed)
        lines.append(f"- **Total Tasks:** {total_tasks}")
        completion_pct = (completed_tasks / total_tasks * 100) if total_tasks > 0 else 0
        lines.append(f"- **Completed Tasks:** {completed_tasks} ({completion_pct:.1f}%)")
        lines.append("")

        # Tasks section
        if plan.tasks:
            lines.append("## Tasks")
            lines.append("")

            # Get root tasks (tasks without parent)
            root_tasks = [task for task in plan.tasks.values() if not task.parent_id]

            # Sort by sequence (if set), then priority, then creation date
            priority_order = {"high": 0, "medium": 1, "low": 2}
            root_tasks.sort(key=lambda t: (
                t.sequence if t.sequence is not None else float('inf'),  # Tasks with sequence come first
                priority_order.get(t.priority, 1),
                t.created_at
            ))

            for task in root_tasks:
                self._add_task_to_markdown(lines, task, plan.tasks, 0)

        # Lessons learned section
        if plan.lessons_learned:
            lines.append("## Lessons Learned")
            lines.append("")

            for lesson in plan.lessons_learned:
                lines.append(f"### {lesson.created_at}")
                lines.append("")
                lines.append(f"**Task:** {lesson.learned_task_id}")
                lines.append("")
                lines.append(lesson.lesson)
                lines.append("")

        # Footer with generation timestamp
        lines.append("---")
        lines.append(f"*Report generated on {datetime.now().isoformat()}*")

        return "\n".join(lines)

    def _add_task_to_markdown(self, lines: List[str], task: TaskModel, all_tasks: Dict[str, TaskModel],
                              indent_level: int) -> None:
        """Add a task and its subtasks to the markdown lines."""
        indent = "  " * indent_level

        # Task checkbox and title
        checkbox = "[x]" if task.completed else "[ ]"
        priority_indicator = ""
        if task.priority == "high":
            priority_indicator = " 🔴"
        elif task.priority == "low":
            priority_indicator = " 🟡"

        lines.append(f"{indent}- {checkbox} **{task.title}**{priority_indicator}")

        # Task description
        if task.description:
            lines.append(f"{indent}  {task.description}")

        # Task context
        if task.context:
            lines.append(f"{indent}  ")
            lines.append(f"{indent}  *Context:*")
            # Split context into lines and indent each
            context_lines = task.context.split('\n')
            for context_line in context_lines:
                if context_line.strip():
                    lines.append(f"{indent}  {context_line}")

        # Task metadata
        lines.append(f"{indent}  ")
        lines.append(
            f"{indent}  *Created: {task.created_at} | Updated: {task.updated_at} | Priority: {task.priority.title()}*")
        lines.append(f"{indent}  ")

        # Add child tasks
        if task.child_tasks:
            child_tasks = [all_tasks[child_id] for child_id in task.child_tasks if child_id in all_tasks]
            # Sort child tasks by sequence (if set), then priority, then creation date
            priority_order = {"high": 0, "medium": 1, "low": 2}
            child_tasks.sort(key=lambda t: (
                t.sequence if t.sequence is not None else float('inf'),  # Tasks with sequence come first
                priority_order.get(t.priority, 1),
                t.created_at
            ))

            for child_task in child_tasks:
                self._add_task_to_markdown(lines, child_task, all_tasks, indent_level + 1)

        lines.append("")


Toolset.register(WorkspacePlanningTools, required_tools=['WorkspaceTools'])

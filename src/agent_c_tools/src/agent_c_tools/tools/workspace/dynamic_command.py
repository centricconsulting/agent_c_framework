import copy
from typing import Dict, Any, Optional, Callable
from agent_c.toolsets.tool_set import Toolset
from agent_c.toolsets.json_schema import json_schema
from .executors.local_storage.secure_command_executor import CommandExecutionResult
from .executors.local_storage.media_event_helper.media_helper import MediaEventHelper
from ..workspace.tool import WorkspaceTools
from ...helpers.validate_kwargs import validate_required_fields
from .dynamic_command_prompt import DynamicCommandPromptSection

class DynamicCommandTools(Toolset):
    """
    Exposes per-command tools (run_git, run_npm, …) that forward to Workspace.run_command
    so the secure pipeline (policies/validators/env/timeouts/output-capping) remains intact.
    """

    def __init__(self, **kwargs: Any):
        super().__init__(**kwargs, name="commands", use_prefix=False)
        self.workspace_tool: Optional[WorkspaceTools] = None
        self.section = DynamicCommandPromptSection()

        # Current MVP, hardcode your current allowlist
        self.whitelisted_commands: Dict[str, Dict[str, Any]] = {
            "git":    {"description": "Run safe, non-destructive git subcommands and flags."},
            "npm":    {"description": "Run safe, non-destructive npm commands."},
            "node":   {"description": "Run safe, non-destructive node commands."},
            "pytest": {"description": "Run safe, non-destructive pytest invocations."},
            "dotnet": {"description": "Run safe, non-destructive dotnet commands."},
            "which": {"description": "Run safe, non-destructive which commands."},
            "where": {"description": "Run safe, non-destructive where commands."},
            "whoami": {"description": "Run safe, non-destructive whoami commands."},
            "echo": {"description": "Run safe, non-destructive echo commands."},
            # Add more as policies/validators exist
        }

        # (future): read YAML policy to auto-generate this list.

        self._create_dynamic_tools()

    async def post_init(self):
        self.workspace_tool = self.get_dependency('WorkspaceTools')
        if not self.workspace_tool:
            self.valid = False
            self.logger.error("DynamicCommandTools disabled: WorkspaceTools dependency not available")

    def _create_dynamic_tools(self) -> None:
        for base_cmd, info in self.whitelisted_commands.items():
            method_name = f"run_{base_cmd}"
            dynamic_method = self._make_tool_for(base_cmd, info.get("description", ""))

            # Attach json_schema describing this specific tool
            decorated_method = json_schema(
                description=f"{info.get('description','Execute a whitelisted command. Only safe, non-destructive commands are permitted.')} (secured via Workspace).",
                params={
                    "path": {
                        "type": "string",
                        "description": "UNC working directory, e.g., //ALPHA/project or //ALPHA/project/subdir",
                        "required": True,
                    },
                    "args": {
                        "type": "string",
                        "description": f"Arguments to pass after '{base_cmd}'. No pipes/redirection/globbing.",
                        "required": False,
                        "default": "",
                    },
                    "timeout": {
                        "type": "integer",
                        "description": "Requested timeout (seconds). May be lowered by policy/validator.",
                        "required": False,
                    },
                    "max_tokens": {
                        "type": "integer",
                        "description": "Max tokens to return to the LLM (output is smart-trimmed).",
                        "required": False,
                        "default": 1000,
                    },
                },
            )(dynamic_method)

            # (1) attach the callable so it can be invoked
            setattr(self, method_name, decorated_method)

            # (2) also register its schema now so Toolset._tool_schemas() will find it cached
            schema = copy.deepcopy(dynamic_method.schema)
            if self.use_prefix:
                schema['function']['name'] = f"{self.prefix}{schema['function']['name']}"
            self._schemas.append(schema)

            self.logger.info(f"Registered dynamic tool: {method_name}")

    def _make_tool_for(self, base_cmd: str, description: str) -> Callable[..., Any]:
        async def tool_method(**kwargs) -> str:
            """
            Resolve UNC → workspace, build command string, call workspace.run_command,
            then emit the same media + capped YAML that WorkspaceTools.run_command uses.
            """
            unc_path: str = kwargs.get("path", "")
            args: str = (kwargs.get("args") or "").strip()
            timeout: Optional[int] = kwargs.get("timeout", None)
            max_tokens: int = kwargs.get("max_tokens", 1000)
            tool_context = kwargs.get("tool_context", {})  # for UI + token counter

            # 0) Validate required fields exist
            success, message = validate_required_fields(kwargs=kwargs, required_fields=['path'])

            if not success:
                return message

            # 1) Resolve UNC to workspace + relative path using existing workspace helper.
            err, workspace, rel_path = self.workspace_tool.validate_and_get_workspace_path(unc_path)
            if err:
                return f"ERROR: {err}"

            # 2) If no workspace found, OR it's a workspace doesn't support run_command .
            if not workspace or not hasattr(workspace, 'run_command'):
                return f"ERROR: Workspace: {workspace.name if workspace else 'None'} does not support OS level run commands"

            # 3) Get OS filesystem path for working directory (validated/normalized).
            try:
                abs_path = workspace.full_path(rel_path or "", mkdirs=False)
                if not abs_path:
                    return f"Invalid path: {rel_path!r}"
            except Exception as e:
                return f"Failed to resolve path: {e}"

            # 4) Build full command (executor will parse & validate; shell features are disabled there).
            full_command = f"{base_cmd} {args}".strip()

            # 5) Delegate to the secure path (LocalStorageWorkspace → SecureCommandExecutor).
            #    This preserves policies, validators, safe env, timeouts, and output-size limits.
            result: CommandExecutionResult = await workspace.run_command(
                command=full_command,
                working_directory=abs_path,
                timeout=timeout,
            )

            # 6) Optional UI: reuse the WorkspaceTools media helper so stdout shows in the panel.
            try:
                html = await MediaEventHelper.stdout_html(result.to_dict())
                await self._raise_render_media(
                    sent_by_class=workspace.__class__.__name__,
                    sent_by_function=f"run_{base_cmd}",
                    content_type="text/html",
                    content=html,
                    tool_context=tool_context,
                )
            except Exception:
                # If media helper isn’t available here, you can omit UI rendering safely.
                self.logger.debug("Media helper not available, skipping media rendering.")
                pass

            # 7) Return YAML, capped by tokens if we have a counter.
            counter = tool_context.get("agent_runtime").count_tokens if tool_context else None
            self.logger.debug(f"Counter exist: {'yes' if counter else 'no'}")
            return result.to_yaml_capped(max_tokens, counter) if counter else result.to_yaml()

        tool_method.__name__ = f"run_{base_cmd}"
        tool_method.__doc__ = description or f"Execute safe {base_cmd} commands. Only safe, non-destructive commands are permitted."
        return tool_method

Toolset.register(DynamicCommandTools, required_tools=['WorkspaceTools'])



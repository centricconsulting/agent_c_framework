from typing import Any, Optional

from agent_c.prompting.prompt_section import OldPromptSection, property_bag_item


class WorkspaceSection(OldPromptSection):
    tool: Any  # Reference to the workspace tool

    def __init__(self, **data: Any):
        TEMPLATE = ("The workspace tools provide you a way to work with files in one or more workspaces. "
                    "Available workspaces are listed below\n"
                    "#\n## Available Workspace List \n"
                    "${workspace_list}\n"
                    "\n## Workspace Operations Guide\n"
                    "- **UNC Paths**: Always use UNC-style paths in the format `//WORKSPACE/path/to/file` where WORKSPACE is the workspace name\n"
                    "- **Reading**: Use `read` to get file contents; assume paths exist rather than checking first\n"
                    "               Use `read_lines` to read a portion of a file\n"
                    "- **Writing**: Use `write` for creating or replacing files; parent directories are created automatically\n"
                    "- **Appending**: Use `write` with the optional `mode` parameter set to `append`\n"
                    "= **Searching**: Use `grep` to search for patterns in files.  This uses the same syntax as the grep CLI.\n"
                    "- **Navigation**: Use `tree` for broader holistic views. Us `ls` to list a specific directory contents\n"
                    "- **Metadata**: The meta tools allow you to read/write to a dictionary stored in the workspace\n"
                    "  - This supports nested dictionaries\n"
                    "- **File Management**: Use `cp` to copy and `mv` to move files or directories\n"
                    "  - Both source and destination must be in the same workspace\n"    
                    "- Workspace text files are UTF-8 encoded\n"
                    "\n## CRITICAL: Workspace Efficiency Rules:\n"
                    "- Prefer `inspect_code` over reading entire code files in Python, or C# code.\n" 
                    "   - This will give you the signatures and doc strings for code files"
                    "   - Line numbers are included for methods allowing you to target reads and updates more easily"
                    "- You can use the line number from `inspect_code` and the `read_lines` tool to grab the source for a single method or class."
                    "- You can use the strings you get from `read_lines` to call `replace_strings`"
                    "- Small changes to existing files should be made using the `replace_strings` methods. If possible\n"
                    "  - Make ALL changes to the same file at once.\n"
                    "  - Batching saves money and time!.\n"
                    "- If you *must* write and entire file use the `write` tool.\n")
        super().__init__(template=TEMPLATE, required=True, name="Workspaces", render_section_header=True, **data)

    @property_bag_item
    async def workspace_list(self, context,  data: dict[str, Any]) -> str:
        """Generate a compact table-like representation of available workspaces."""
        if not hasattr(self, 'tool') or not self.tool:
            return "No workspaces available"

        workspaces = self.tool.workspace_list(context)
        
        if not workspaces:
            return "No workspaces available"

        allowed_spaces = data.get('allowed_workspaces', [])
        disallowed_spaces = data.get('disallowed_workspaces', [])
        if len(allowed_spaces) > 0:
            workspaces = [space for space in workspaces if space.name in allowed_spaces]

        if len(disallowed_spaces) > 0:
            workspaces = [space for space in workspaces if space.name not in disallowed_spaces]

        force_read_only = data.get('force_read_only', False)

        lines = []
        for space in workspaces:
            rw_status = "RO" if force_read_only else  space.write_status
            lines.append(f"- `{space.name}` ({rw_status}) - {space.description}")
            
        return "\n".join(lines)
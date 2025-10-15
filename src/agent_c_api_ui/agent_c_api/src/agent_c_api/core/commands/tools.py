import copy
from typing import List, TYPE_CHECKING

from agent_c.models.client_tool_info import ClientToolInfo
from agent_c.toolsets import Toolset
from .base_command import Command
from .parsers import FunctionCallParser, JsonStyleParser

if TYPE_CHECKING:
    from agent_c_api.core.realtime_bridge import RealtimeBridge


class EquipToolCommand(Command):
    @property
    def command_strings(self) -> List[str]:
        return ['!equip', '!eq']

    @property
    def help_text(self) -> str:
        return ("Equip a toolset on an agent - Usage: !equip [toolset_name]\n\nCase is not important, and you need only supply as much of the name to be a unique or exact match.\n\n"
                "- `!eq work` would match multiple tools and would result in an error.\n"
                "- `!eq workspace` would equip the WorkspaceTools as it is a complete match.\n"
                "- `!eq workspace_p` would equip the WorkspacePlanningTools toolset as it is enough for a unique match.")

    async def execute(self, context: 'RealtimeBridge', **kwargs):
        to_equip: List[str] = []
        catalog: List[ClientToolInfo] = Toolset.client_tool_registry
        if 'raw_args' in kwargs and kwargs['raw_args']:
            args = kwargs['raw_args'].split(' ')
        else:
            await context.send_system_message("You must specify at least one toolset name to equip.", severity="error")
            return

        for arg in args:
            toolset_name = arg.strip().lower()

            matched_toolsets = [ts.name for ts in catalog if ts.name.lower().startswith(toolset_name)]

            if len(matched_toolsets) == 0:
                await context.send_system_message(f"No toolset found matching '**{toolset_name}**'.", severity="error")
                return

            if len(matched_toolsets) > 1:
                matched_toolsets = [ts for ts in catalog if ts.name.lower() == toolset_name or ts.name.lower().rstrip("tools") == toolset_name]

            if len(matched_toolsets) > 1:
                await context.send_system_message(f"Multiple toolsets match '**{toolset_name}**': {', '.join(matched_toolsets)}. Please be more specific.")
            else:
                to_equip.append(matched_toolsets[0])

        if to_equip:
            tools = copy.deepcopy(context.chat_session.agent_config.tools)
            for toolset_name in to_equip:
                if toolset_name not in tools:
                    tools.append(toolset_name)

            await context.update_tools(tools)

class RemoveToolCommand(Command):
    @property
    def command_strings(self) -> List[str]:
        return ['!remove', '!unequip', '!un']

    @property
    def help_text(self) -> str:
        return ("Remove a toolset on an agent - Usage: !rem [toolset_name]\n\nCase is not important, and you need only supply as much of the name to be a unique or exact match.\n\n"
                "- `!rem work` would match multiple tools and would result in an error.\n"
                "- `!rem workspace` would equip the WorkspaceTools as it is a complete match.\n"
                "- `!rem workspace_p` would equip the WorkspacePlanningTools toolset as it is enough for a unique match.")

    async def execute(self, context: 'RealtimeBridge', **kwargs):
        to_remove: List[str] = []
        agent_tools: List[str] = context.chat_session.agent_config.tools
        if 'raw_args' in kwargs and kwargs['raw_args']:
            args = kwargs['raw_args'].split(' ')
        else:
            await context.send_system_message("You must specify at least one toolset name to remove.", severity="error")
            return

        for arg in args:
            toolset_name = arg.strip().lower()

            matched_toolsets = [ts for ts in agent_tools if ts.lower().startswith(toolset_name)]

            if len(matched_toolsets) == 0:
                await context.send_system_message(f"No toolset found matching '**{toolset_name}**'.", severity="error")
                return

            if len(matched_toolsets) > 1:
                matched_toolsets = [ts for ts in agent_tools if ts.lower() == toolset_name or ts.lower().rstrip("tools") == toolset_name]

            if len(matched_toolsets) > 1:
                await context.send_system_message(f"Multiple toolsets match '**{toolset_name}**': {', '.join(matched_toolsets)}. Please be more specific.")
            else:
                to_remove.append(matched_toolsets[0])

        if to_remove:
            tools = [tn for tn in agent_tools if tn not in to_remove]
            await context.update_tools(tools)

class ToolInfoCommand(Command):
    @property
    def command_strings(self) -> List[str]:
        return ['!tool_info', '!ti']

    @property
    def help_text(self) -> str:
        return ("Get information about available toolsets and their tools - Usage: !tool_info [toolset_name]\n\nCase is not important, and you need only supply as much of the name to be a unique or exact match.\n\n"
                "- `!tool_info` By itself will display the toolsets and their descriptions.\n\n")

    async def execute(self, context: 'RealtimeBridge', **kwargs):

        target = None
        if 'raw_args' in kwargs and kwargs['raw_args']:
            target = kwargs['raw_args'].strip().lower()

        catalogue = Toolset.client_tool_registry

        if target is None:
            lines = ["# Available Toolsets\n"]
            for tool_info in catalogue:
                lines.append(f"## {tool_info.name}\n\n{tool_info.description}\n\n")
            await context.raise_render_media_markdown("\n".join(lines))

        else:
            matched_toolsets: List[ClientToolInfo] = [ts for ts in catalogue if ts.name.lower().startswith(target)]

            if len(matched_toolsets) == 0:
                await context.send_system_message(f"No toolset found matching '**{target}**'.", severity="error")
                return

            if len(matched_toolsets) > 1:
                matched_toolsets = [ts for ts in catalogue if ts.name.lower() == target or ts.name.lower().rstrip("tools") == target]

            if len(matched_toolsets) > 1:
                await context.send_system_message(f"Multiple toolsets match '**{target}**': {', '.join([ts.name for ts in matched_toolsets])}. Please be more specific.")
            else:
                toolset = matched_toolsets[0]
                lines = [f"# Toolset: {toolset.name}\n\n{toolset.description}\n\n## Tools\n"]
                for schema in toolset.schemas:
                    lines.append(f"### {schema['function']['name']}\n\n{schema['function']['description']}\n\n")
                    if schema['function']['parameters']:
                        lines.append("**Parameters:**\n\n")
                        for param_name, param_info in schema['function']['parameters']['properties'].items():
                            req = " (required)" if param_info.get('required', False) else ""
                            param_type = param_info.get('type', 'unknown')
                            param_desc = param_info.get('description', '')
                            lines.append(f"- `{param_name}` ({param_type}{req}): {param_desc}\n")
                        lines.append("\n")

                await context.raise_render_media_markdown("\n".join(lines))


class CallToolCommand(Command):
    def __init__(self):
        super().__init__(FunctionCallParser())

    @property
    def command_strings(self) -> List[str]:
        return ['!call']

    @property
    def help_text(self) -> str:
        return ("Call a tool without an agent - Usage: !call tool_name( arg1: val, arg2: \"val 2\")\n\nCase IS important.\n\n**Example:**\n\n"
                "- `!call bridge_set_session_name(session_name: 'foo')`\n"
                )

    async def execute(self, context: 'RealtimeBridge', function_name: str, params: dict, **kwargs):
        await context.call_tool(function_name, params)
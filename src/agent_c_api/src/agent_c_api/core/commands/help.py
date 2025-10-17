from typing import List, TYPE_CHECKING

from .base_command import Command

if TYPE_CHECKING:
    from agent_c_api.core.realtime_bridge import RealtimeBridge


class HelpCommand(Command):
    @property
    def command_strings(self) -> List[str]:
        return ['!help', '!h', '!?']

    @property
    def help_text(self) -> str:
        return "Show available commands or help for a specific command - Usage: !help [command]"

    async def execute(self, context: 'RealtimeBridge', **kwargs):
        # Get the command handler from context
        handler = context.command_handler

        # Check if they want help on a specific command
        if 'raw_args' in kwargs and kwargs['raw_args']:
            target = kwargs['raw_args'].strip()
            if not target.startswith('!'):
                target = '!' + target

            # Find the command
            if target in handler.command_map:
                cmd = handler.command_map[target]
                aliases = ', '.join(cmd.command_strings)
                message = f"## {cmd.cmd_name} Help\n\n{cmd.help_text}\n\n**Aliases:** {aliases}\n\n"
                await context.send_system_message(message)
            else:
                await context.send_system_message(f"Unknown command: {target}", severity="error")
        else:
            # Show all commands
            lines = ["# Agent C Chat Command Help\n\nChat commands allow you to interact with the Agent C runtime, without dedicated UI or Agent support.\n\n ## Available commands\n"]
            # Group by command instance to avoid duplicates
            seen_commands = set()
            for cmd_str, cmd in handler.command_map.items():
                if id(cmd) not in seen_commands:
                    name = cmd.cmd_name
                    seen_commands.add(id(cmd))
                    aliases = ', '.join(cmd.command_strings)
                    line = f"### {name}\n\n{cmd.help_text}\n\n**Aliases:** {aliases}\n\n"
                    lines.append(line)

            await context.raise_render_media_markdown("\n".join(lines))
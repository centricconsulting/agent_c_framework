from typing import TYPE_CHECKING, List

from agent_c_api.core.commands.agents import ReloadAgentsCommand
from agent_c_api.core.commands.base_command import Command
from agent_c_api.core.commands.help import HelpCommand
from agent_c_api.core.commands.history import ForkCommand, RewindCommand
from agent_c_api.core.commands.tools import EquipToolCommand, RemoveToolCommand, CallToolCommand, ToolInfoCommand

if TYPE_CHECKING:
    from agent_c_api.core.realtime_bridge import RealtimeBridge


class ChatCommandHandler:
    def __init__(self):
        self.commands: List[Command] = []
        self.command_map: dict[str, Command] = {}

        # Register all your commands
        self._register_commands([
            HelpCommand(),
            ForkCommand(),
            RewindCommand(),
            ReloadAgentsCommand(),
            ToolInfoCommand(),
            EquipToolCommand(),
            RemoveToolCommand(),
            CallToolCommand(),
        ])

    def _register_commands(self, commands: List[Command]):
        """Register commands and build the lookup map"""
        for cmd in commands:
            self.commands.append(cmd)
            for cmd_str in cmd.command_strings:
                self.command_map[cmd_str.lower()] = cmd

    async def handle_command(self, user_message, context: 'RealtimeBridge') -> bool:
        user_message = user_message.strip()

        if user_message == "":
            return True

        if not user_message.startswith("!"):
            return False

        if "\n" in user_message:
            return False

        # Split into command and args
        parts = user_message.split(None, 1)
        command_name = parts[0].lower()
        args_string = parts[1] if len(parts) > 1 else ""

        if command_name not in self.command_map:
            return False

        command = self.command_map[command_name]

        try:
            # Parse arguments using command's parser
            parsed_args = command.parser.parse(args_string)
            # Also pass raw args in case command wants them (for !help)
            parsed_args['raw_args'] = args_string
            # Execute with parsed args
            await command.execute(context, **parsed_args)
        except Exception as e:
            await context.send_system_message(f"Error executing {command_name}: {str(e)}", severity="error")

        return True
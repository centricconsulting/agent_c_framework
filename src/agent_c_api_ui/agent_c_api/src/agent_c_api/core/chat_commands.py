from abc import ABC, abstractmethod


class Command(ABC):
    @abstractmethod
    async def execute(self, context):
        pass


class ChangeOutputFormatCommand(Command):
    def __init__(self, format):
        self.format = format

    async def execute(self, context):
        context.agent_output_format = self.format
        context.show_output_mode_hint()


class ExitCommand(Command):
    async def execute(self, context):
        context.exit_event.set()


class SaveSessionCommand(Command):
    async def execute(self, context):
        # Save HTML
        with open(f"{context.zep_cache.session.session_id}.html", 'w', encoding='utf-8') as html_file:
            html_file.write(context.chat_ui.rich_console.export_html(clear=False))
        # Save SVG
        with open(f"{context.zep_cache.session.session_id}.svg", 'w', encoding='utf-8') as svg_file:
            svg_file.write(context.chat_ui.rich_console.export_svg(clear=False, title="Agent C"))


class KeepSessionCommand(Command):
    async def execute(self, context):
        context.zep_cache.is_new_session = False
        context.chat_ui.print_session_info(context.zep_cache.session_id)


class CompactCommand(Command):
    async def execute(self, context):
        context.current_chat_Log = None
        context.chat_ui.system_message("Images, Tool calls and results have been removed from the message log.")

class NewSessionCommand(Command):
    async def execute(self, context):
        await context.new_session()
        context.current_chat_Log = None


class OpenInterpreterCommand(Command):
    def __init__(self, message):
        self.message = message

    async def execute(self, context):
        if context.allow_oi:
            await context.tool_chest.active_tools['opi'].chat(message=self.message, role="user")
        else:
            context.chat_ui.fake_role_message("system", "Open interpreter support is not enabled.  Use --oi to enable.")


class UnrecognizedCommand(Command):
    def __init__(self, message):
        self.message = message

    async def execute(self, context):
        context.chat_ui.fake_role_message("system", f"Unrecognized command. {self.message}")


class CommandHandler:
    def __init__(self):
        self.commands = {
            '!r': ChangeOutputFormatCommand("raw"),
            '!m': ChangeOutputFormatCommand("markdown"),
            '!exit': ExitCommand(),
            '!!!': ExitCommand(),
            '!s': SaveSessionCommand(),
            '!keep': KeepSessionCommand(),
            '!!!!': KeepSessionCommand(),
            '!compact': CompactCommand(),
            '!purge': CompactCommand(),
            '!new': NewSessionCommand()
        }

    async def handle_command(self, user_message, context) -> bool:
        user_message_lower = user_message.lower().strip()

        if user_message_lower == "":
            return True
        elif user_message_lower in self.commands:
            await self.commands[user_message_lower].execute(context)
        elif user_message_lower.startswith("!"):
            #await UnrecognizedCommand(user_message).execute(context)
            return False
        else:
            return False

        return True

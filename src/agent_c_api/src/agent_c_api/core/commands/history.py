from typing import List

from agent_c_api.core.commands.base_command import Command


class RewindCommand(Command):
    @property
    def command_strings(self) -> List[str]:
        return ['!rewind', '!rew']

    @property
    def help_text(self) -> str:
        return "Rewind the chat session by a number of user input messages - Usage: !rewind [steps]\n\n"

    async def execute(self, context, **kwargs):

        if 'raw_args' in kwargs and kwargs['raw_args']:
            count = int(kwargs['raw_args'].strip())
        else:
            count = 1

        await context.rewind_session(count)

class ForkCommand(Command):
    @property
    def command_strings(self) -> List[str]:
        return ['!fork', '!f']

    @property
    def help_text(self) -> str:
        return "Fork the current chat session into a new session - Usage: !fork\n\n"

    async def execute(self, context, **kwargs):
        await context.fork_session()

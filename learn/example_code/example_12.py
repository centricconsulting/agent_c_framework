import asyncio
from dotenv import load_dotenv
load_dotenv(override=True)

from typing import Any, List, Union

from prompt_toolkit import PromptSession
from rich.console import Console
from rich.markdown import Markdown
from rich.rule import Rule

from agent_c_core.agent_c.agents.gpt import GPTChatAgent
from agent_c_core.agent_c.models.chat_event import ChatEvent
from agent_c_core.agent_c.toolsets.tool_chest import ToolChest
from agent_c_tools.tools.weather import WeatherTools  # noqa


class ExampleUI:
    def __init__(self):
        self.rich_console = Console()
        self.prompt_session = PromptSession()
        self.last_role = ""

    def line_separator(self, label, style="bold"):
        self.rich_console.print("\b\b")
        self.rich_console.print(Rule(title=label, style=style, align="right"))

    async def get_user_input(self) -> str:
        self.line_separator("You")
        self.last_role = "You"
        return await self.prompt_session.prompt_async(': ')

    def print_message(self, role: str, content: str):
        self.last_role = role
        self.line_separator(role)
        self.rich_console.print(Markdown(content))

    def print_role_token(self, role: str, token: str):
        if self.last_role != role:
            self.line_separator(role)
            self.last_role = role

        self.rich_console.print(token, end="")


async def main():
    ui = ExampleUI()
    tool_chest = ToolChest()
    await tool_chest.init_tools()

    prompt: str = "You are a helpful assistant helping people learn about, build and test AI agents"

    messages: Union[List[dict[str, Any]], None] = None

    async def chat_callback(event: ChatEvent):
        if event.content is not None:
            ui.print_role_token(event.role, event.content)

    agent = GPTChatAgent(tool_chest=tool_chest, prompt=prompt,
                         model_name="gpt-4-turbo-preview", streaming_callback=chat_callback)

    while True:
        user_message = await ui.get_user_input()
        if user_message == "exit":
            break

        messages = await agent.chat(user_message=user_message, messages=messages)


if __name__ == "__main__":
    asyncio.run(main())

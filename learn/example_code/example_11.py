import asyncio
from dotenv import load_dotenv
load_dotenv(override=True)
from typing import Any, List, Union

from prompt_toolkit import PromptSession
from rich.console import Console
from rich.markdown import Markdown
from rich.rule import Rule

from agent_c_core.agent_c import GPTChatAgent, ToolChest

from agent_c_tools.tools.weather import WeatherTools  # noqa


class ExampleUI:
    def __init__(self):
        self.rich_console = Console()
        self.prompt_session = PromptSession()

    def line_separator(self, label, style="bold"):
        self.rich_console.print()
        self.rich_console.print(Rule(title=label, style=style, align="right"))

    async def get_user_input(self) -> str:
        self.line_separator("You")
        return await self.prompt_session.prompt_async(': ')

    def print_message(self, role: str, content: str):
        self.line_separator(role)
        self.rich_console.print(Markdown(content))


async def main():
    tool_chest = ToolChest()
    await tool_chest.init_tools()

    prompt: str = "You are a helpful assistant helping people learn about, build and test AI agents"
    agent = GPTChatAgent(tool_chest=tool_chest, prompt=prompt, model_name="gpt-4-turbo-preview")

    messages: Union[List[dict[str, Any]], None] = None
    ui = ExampleUI()

    while True:
        user_message = await ui.get_user_input()
        if user_message == "exit":
            break

        messages = await agent.chat(user_message=user_message, messages=messages)

        ui.print_message(messages[-1]['role'], messages[-1]['content'])


if __name__ == "__main__":
    asyncio.run(main())

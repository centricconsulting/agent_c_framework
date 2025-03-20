import os
import asyncio
from dotenv import load_dotenv
load_dotenv(override=True)

import webbrowser
from typing import Any, List, Union

from prompt_toolkit import PromptSession
from prompt_toolkit.history import FileHistory
from rich.console import Console
from rich.markdown import Markdown
from rich.rule import Rule

from agent_c_core.agent_c.agents.gpt import GPTChatAgent
from agent_c_core.agent_c.models.chat_event import ChatEvent
from agent_c_core.agent_c.prompting import PersonaSection, PromptBuilder
from agent_c_core.agent_c.toolsets.tool_chest import ToolChest
from agent_c_tools.tools.workspaces import WorkspaceTools  # noqa
from agent_c_tools.tools.workspaces.local_storage import LocalStorageWorkspace
from agent_c_tools.tools.dall_e import DallETools  # noqa


class ExampleUI:
    def __init__(self):
        self.rich_console = Console(soft_wrap=True)
        self.prompt_session = PromptSession(history=FileHistory(".chat_input"))
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
        self.rich_console.print(Markdown(content), end="")

    def print_role_token(self, role: str, token: str):
        if self.last_role != role:
            self.line_separator(role)
            self.last_role = role
        if 'http' in token:
            s = token
        self.rich_console.print(token, end="")


async def main():
    ui = ExampleUI()
    tool_chest = ToolChest()

    async def chat_callback(event: ChatEvent):
        if event.content is not None:
            ui.print_role_token(event.role, event.content)

        if event.tool_use_active is not None:
            if event.tool_use_active:
                tools = "\n- ".join([tool.name for tool in event.tool_calls])
                ui.print_message("Tool", f"# Agent is using the following toolsets:\n{tools}\n")
            else:
                ui.print_role_token("Tool", "\nAgent has stopped using toolsets\n")

        if event.render_media is not None:
            if event.render_media.url is not None:
                webbrowser.open(event.render_media.url)

    path = os.path.join(os.getcwd(), 'learn/lab_code')
    lab_workspace = LocalStorageWorkspace(name="lab", workspace_path=path)

    await tool_chest.init_tools(workspaces=[lab_workspace],
                                username="example_user",
                                streaming_callback=chat_callback)

    prompt: str = "You are a helpful assistant helping people learn about, build and test AI agents."

    my_sections = [PersonaSection(template=prompt)] + tool_chest.active_tool_sections

    messages: Union[List[dict[str, Any]], None] = None

    agent = GPTChatAgent(tool_chest=tool_chest,
                         streaming_callback=chat_callback)

    while True:
        user_message = await ui.get_user_input()
        if user_message == "exit":
            break

        prompt_metadata = {}
        messages = await agent.chat(user_message=user_message, messages=messages,
                                    prompt_builder=PromptBuilder(sections=my_sections),
                                    prompt_metadata=prompt_metadata)


if __name__ == "__main__":
    asyncio.run(main())

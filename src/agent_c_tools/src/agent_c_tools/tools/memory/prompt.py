from typing import Any
from agent_c.prompting import OldPromptSection, property_bag_item
from agent_c.chat.session_manager import ChatSessionManager


class MemorySection(OldPromptSection):
    session_manager: ChatSessionManager

    def __init__(self, **data: Any):
        TEMPLATE = ("This section displays data you've saved in the session."
                    "toolsets have been provided for you to update some of this information, either by request or when you have a need to store something outside of the chat log.\n"
                    "The metadata records have been converted to YAML below for readability\n"
                    "### Assistant data (ChatSession):\n"
                    "```yaml\n${session_kvps}\n```\n")
        super().__init__(template=TEMPLATE, required=True, name="ChatSession Memory", render_section_header=True, **data)

    @property_bag_item
    async def session_kvps(self):
        return self.session_manager.filtered_session_meta_string('ai_')
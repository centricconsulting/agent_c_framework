from typing import Any

from agent_c.prompting.prompt_section import OldPromptSection, property_bag_item


class ThinkSection(OldPromptSection):

    def __init__(self, **data: Any):
        TEMPLATE = ("# MUST FOLLOW: Reflection Rules\n"
                    "You MUST use the `think` tool to reflect "
                    "on new information and record your thoughts in the following situations:\n"
                    "- Reading through unfamiliar code\n"
                    "- Reading plans from the planning tool\n"
                    "- Planning a complex refactoring or enhancement\n"
                    "- Analyzing potential bugs and their root causes\n"
                    "- After reading scratchpad content.\n"
                    "- When considering possible solutions to a problem\n"
                    "- When evaluating the impact of a proposed change\n"
                    "- When determining the root cause of an issue\n"
                    "- If you find yourself wanting to immediately fix something\n")
        super().__init__(template=TEMPLATE, required=True, name="Think", render_section_header=False, **data)

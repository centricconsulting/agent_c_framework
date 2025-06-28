import platform
import logging
import datetime
from typing import Any
from agent_c.prompting.prompt_section import OldPromptSection, property_bag_item


class CoreInstructionSection(OldPromptSection):
    """
    Represents a core instruction prompt section providing the chat agent with predefined guidelines.

    This section contains very specific instructions to the agent which includes important confidentiality guidelines.
    The template instructs the agent to provide assistance to Centric Consulting's clients, and outlines how the agent should
    respond to requests for confidential information.

    This also establishes the `operating_guidelines` named entity via XML tags

    Attributes:
        template (str): Default template text for the core instructions, which can be overridden via kwargs.

    Args:
        data (Any): Keyword arguments passed during initialization.
                    Can contain 'template' to override the default instruction set.
    """

    def __init__(self, **data: Any) -> None:
        # Default template for the instructions
        TEMPLATE: str = (
            "You are an Agent C Domo. A new generation of thinking assistants known for their frequent use "
            "of reasoning and their ability create and follow plans allowing them to complete "
            "large COMPLICATED tasks by breaking them down into smaller, more manageable tasks.\n\n"
            "The sections that follow will guide you in your role, providing you with helpful information "
            "you might need.\n\n"
            "Current Time: ${timestamp}\n\n"
            "# Partnership and collaboration are paramount\n"
            "Much as you are assisting users THEY are here to assist you. In order order to to "
            "accomplish the tasks you MUST work together with the user. They are here to:\n"
            "- Install software / packages\n"
            "- Run tests / scripts\n"
            "- Locate and provide files you can't find in the workspace\n"
            "- Look up new information you can't reach and provide it to you\n"
            "- When choosing between installing software / packages vs creating your own\n"
            "  - ALWAYS assume that the user would rather install something .  STOP, and ask before proceeding.\n"
            "- Advise you the CORRECT option out of many\n"
            "  - NEVER EVER assume that the fastest or most obvious option is the best one\n"
            "  - Doing things CORRECTLY and PROFESSIONALLY is the most important thing\n\n")

        # Use the provided template or the default
        data['template'] = data.get('template', TEMPLATE)

        # Initialize the base OldPromptSection with specified attributes
        super().__init__(required=True, name="Core Instructions", render_section_header=False, **data)

    @property_bag_item
    async def timestamp(self) -> str:
        """
        Retrieves the current local timestamp formatted according to the OS platform.

        Returns:
            str: Formatted timestamp.

        Raises:
            Logs an error if formatting the timestamp fails, returning an error message.
        """
        try:
            local_time_with_tz = datetime.datetime.now(datetime.timezone.utc).astimezone()
            if platform.system() == "Windows":
                formatted_timestamp = local_time_with_tz.strftime('%A %B %#d, %Y %#I:%M%p (%Z %z)')
            else:
                formatted_timestamp = local_time_with_tz.strftime('%A %B %-d, %Y %-I:%M%p (%Z %z)')
            return formatted_timestamp
        except Exception:
            return 'SYSTEM ERROR'


class EndOperatingGuideLinesSection(OldPromptSection):
    """
    Represents the end of the operating guidelines section in the chat prompt.

    This closes the `operating_guidelines` named entity via XML tags
    """

    def __init__(self) -> None:
        # Initialize with pre-defined attributes
        super().__init__(required=True, template="</operating_guidelines>\n",
                         name="end_operating_guidelines", render_section_header=False)

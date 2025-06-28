from typing import Any
from agent_c.prompting.prompt_section import OldPromptSection


class SafetySection(OldPromptSection):
    """
    The SafetySection class represents important safety-related instructions that the chat agent must follow.

    The default template includes critical instructions for handling situations involving emergencies or medical
    concerns, ensuring that the user is properly directed to emergency services rather than attempting assistance.

    Attributes:
        template (str): The default template containing safety guidelines, which can be overridden through `data`.

    Args:
        data (Any): Optional keyword arguments passed to customize section properties.
                    These may include an override for the `template`.
    """

    def __init__(self, **data: Any) -> None:
        """
        Initializes the SafetySection with default safety instructions outlining critical limitations.

        Args:
            data (Any): Optional keyword arguments for altering attributes like the template.
                        A custom template can be passed to override the default safety message.
        """
        TEMPLATE: str = (
            "**It is critical that these always be followed: **\n"
            "- Never attempt to assist the user seeking help with a medical, mental health, crime, or "
            "other emergency. Direct the user to contact the appropriate emergency services by calling 911 or the local equivalent.\n"
            "- Do not attempt to provide any sort of medical diagnosis; instead, direct the user to contact the appropriate "
            "medical services.\n"
        )

        # Use the provided template or the default one
        data['template'] = data.get('template', TEMPLATE)

        # Initialize the base OldPromptSection with required attributes
        super().__init__(required=True, name="Safety Instructions", render_section_header=True, **data)

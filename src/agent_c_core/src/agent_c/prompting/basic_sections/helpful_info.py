from agent_c.prompting.prompt_section import OldPromptSection


class HelpfulInfoStartSection(OldPromptSection):
    """
    Represents the starting section providing helpful information for the chat agent.

    This section is used to introduce additional contextual information or guidance
    to the agent, displaying a header followed by relevant content from sub-sections.

    Attributes:
        TEMPLATE (str): The default template text for this section, which includes a helpful information heading.
    """

    def __init__(self) -> None:
        """
        Initializes the HelpfulInfoStartSection with a predefined template text.
        The template provides a heading and a brief description for the helpful information section.
        """
        TEMPLATE: str = (
            "\n# Helpful information\n"
            "The following sections are to help orient you and give you a leg up in accomplishing your tasks."
        )

        # Call the base class constructor to set the required properties
        super().__init__(template=TEMPLATE, required=True, name="Helpful information", render_section_header=False)

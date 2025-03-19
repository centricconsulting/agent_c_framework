from agent_c_core.agent_c.prompting.prompt_section import PromptSection


class BeginToolGuideLinesSection(PromptSection):
    """
    Represents the beginning section for tool guidelines in the agent prompt.

    This class generates a section within the prompt that marks the start of tool guidelines using
    a specific template.  Each tool that has a prompt section will be listed between this and the EndToolGuideLinesSection.

    This also establishes the `tool_guidelines` named entity via XML tags.

    The section header is not rendered for this section since it's controlled via the template.


    """

    def __init__(self) -> None:
        """
        Initializes the BeginToolGuideLinesSection with a predefined template that begins a tool guidelines block.
        This sets the template to `<tool_guidelines>\n` and disables section header rendering.
        """
        super().__init__(required=True, template="<tool_guidelines>\n", name="tool_guidelines", render_section_header=False)


class EndToolGuideLinesSection(PromptSection):
    """
    Represents the ending section for tool guidelines in the agent prompt.

    This class generates a section within the prompt that marks the end of tool guidelines using
    a specific template.

    The section header is not rendered for this section since it's controlled via the template.
    """

    def __init__(self) -> None:
        """
        Initializes the EndToolGuideLinesSection with a predefined template that ends a tool guidelines block.
        This sets the template to `</tool_guidelines>\n` and disables section header rendering.
        """
        super().__init__(required=True, template="</tool_guidelines>\n", name="end_tool_guidelines", render_section_header=False)

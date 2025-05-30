import re
import logging
from typing import List, Dict, Any, Set, Optional
from agent_c.prompting.prompt_section import PromptSection
from agent_c.util.logging_utils import LoggingManager


class PromptBuilder:
    """
    A class to build a prompt by rendering sections with provided data.

    Attributes:
        sections (List[PromptSection]): A list of PromptSection objects that define the structure of the prompt.
    """

    def __init__(self, sections: List[PromptSection], tool_sections: List[PromptSection]=None) -> None:
        """
        Initialize the PromptBuilder with a list of sections.

        Args:
            sections (List[PromptSection]): A list of PromptSection objects.
        """
        self.sections: List[PromptSection] = sections
        self.tool_sections: List[PromptSection] = tool_sections or []
        logging_manager = LoggingManager(self.__class__.__name__)
        self.logger = logging_manager.get_logger()

    @staticmethod
    def _get_template_variables(template: str) -> Set[str]:
        """
        Extract the template variables from a string template.

        Args:
            template (str): The string template to extract variables from.

        Returns:
            Set[str]: A set of variable names found in the template.
        """
        return set(re.findall(r'\{(.+?)\}', template))

    async def render(self, data: Dict[str, Any], tool_sections: Optional[List[PromptSection]] = None) -> str:
        """
        Render the prompt sections with the provided data.

        Args:
            data (Dict[str, Any]): A dictionary containing the data to render the sections with.
            tool_sections (Optional[List[PromptSection]]): A list of prompt sections to use in the rendering process
                                                           instead of the active tool sections from the toolchest
        Returns:
            str: The rendered prompt as a string.

        Raises:
            KeyError: If a required key is missing from the data dictionary.
            Exception: If an unexpected error occurs during rendering.
        """
        rendered_sections: List[str] = []
        if tool_sections is None:
            tool_sections = self.tool_sections

        section_lists = [self.sections, tool_sections]
        section_list_titles= ["Core Operating Guidelines", "Additional Tool Operation Guidelines"]

        for index, section_list in enumerate(section_lists):
            if len(section_list) == 0:
                continue

            rendered_sections.append(f"# {section_list_titles[index]}\n\n")

            header_prefix = "#" * (index + 1)

            for section in section_list:
                try:
                    rendered_section: str = await section.render(data)
                    rendered_section += "\n\n"

                    if section.render_section_header:
                        rendered_section = f"{header_prefix} {section.name}\n{rendered_section}"

                    rendered_sections.append(rendered_section)
                except KeyError as e:
                    missing_key = str(e).strip("'")
                    template_vars = self._get_template_variables(section.template)
                    self.logger.error(
                        f"Error rendering section '{section.name}': Missing key '{missing_key}'. "
                        f"Template variables: {template_vars}. Data provided: {data.keys()}"
                    )
                    if section.required:
                        raise
                except Exception as e:
                    self.logger.exception(f"Error rendering section '{section.name}': {e}")
                    if section.required:
                        raise

        result = "\n".join(rendered_sections)
        return result

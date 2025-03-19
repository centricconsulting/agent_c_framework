import re
import logging
from typing import List, Dict, Any, Set
from agent_c_core.agent_c.prompting.prompt_section import PromptSection


class PromptBuilder:
    """
    A class to build a prompt by rendering sections with provided data.

    Attributes:
        sections (List[PromptSection]): A list of PromptSection objects that define the structure of the prompt.
    """

    def __init__(self, sections: List[PromptSection]) -> None:
        """
        Initialize the PromptBuilder with a list of sections.

        Args:
            sections (List[PromptSection]): A list of PromptSection objects.
        """
        self.sections: List[PromptSection] = sections

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

    async def render(self, data: Dict[str, Any]) -> str:
        """
        Render the prompt sections with the provided data.

        Args:
            data (Dict[str, Any]): A dictionary containing the data to render the sections with.

        Returns:
            str: The rendered prompt as a string.

        Raises:
            KeyError: If a required key is missing from the data dictionary.
            Exception: If an unexpected error occurs during rendering.
        """
        rendered_sections: List[str] = []
        for section in self.sections:
            try:
                rendered_section: str = await section.render(data)

                if section.render_section_header:
                    rendered_section = f"\n## {section.name}\n{rendered_section}"

                rendered_sections.append(rendered_section)
            except KeyError as e:
                missing_vars: Set[str] = self._get_template_variables(section.template)
                logging.error(
                    f"Missing required keys for section '{section.name}'. Required: {missing_vars}. Data provided: {data.keys()}"
                )
                if section.required:
                    raise
            except Exception as e:
                logging.exception(f"Error rendering section '{section.name}': {e}")
                if section.required:
                    raise

        result = "\n".join(rendered_sections)
        return result

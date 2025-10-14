import re
import datetime
import platform

from string import Template
from typing import Any, Optional

from agent_c.prompting.prompt_section import PromptSection, property_bag_item
from agent_c.util.logging_utils import LoggingManager


logger = LoggingManager(__name__).get_logger()

class PersonaSection(PromptSection):
    """
    The PersonaSection class defines a chat agent's persona and behavioral traits.
    By default, the persona is described as calm, casual, yet professional, and follows the operating guidelines.

    Attributes:
        template (str): The default template describing the agent's persona.

    Args:
        data (Any): Keyword arguments allowing optional overrides for section properties.
    """

    def __init__(self, **data: Any) -> None:
        """
        Initializes the PersonaSection with a predefined persona template.
        The template promotes a friendly, cheerful, and professional assistant behavior while following the operating guidelines.

        Args:
            data (Any): Optional keyword arguments passed to customize section properties.
                        This includes overriding the template if a different persona behavior is needed.
        """
        TEMPLATE: str = (
            "You are a helpful agent assisting users in a calm, casual yet professional manner. "
            "Be friendly and cheerful and assist users with their requests while staying mindful of the operating guidelines"
        )
        # Use the provided template or the default one
        data['template'] = data.get('template', TEMPLATE)

        # Initialize the base PromptSection with specific attributes
        super().__init__(name="Your Persona", render_section_header=True, **data)



class DynamicPersonaSection(PromptSection):
    """
    The DynamicPersonaSection class allows for dynamic persona customization based on external factors.
    It relies on a variable called 'persona_prompt' to insert the specific persona characteristics.

    Attributes:
        template (str): The default persona template placeholder using $persona_prompt.

    Args:
        data (Any): Keyword arguments allowing optional overrides for section properties.
    """

    def __init__(self, **data: Any) -> None:
        """
        Initializes the DynamicPersonaSection with a dynamic persona prompt.
        The 'persona_prompt' placeholder allows for flexible persona insertion during runtime.

        Args:
            data (Any): Optional keyword arguments passed to customize section properties.
                        This includes overriding the template with a different dynamic persona configuration if needed.
        """
        TEMPLATE: str = "$rendered_persona_prompt"

        # Use the provided template or the default one
        data['template'] = data.get('template', TEMPLATE)

        # Initialize the base PromptSection with specific attributes
        super().__init__(name="Agent Persona, RULES and Task Context", render_section_header=True, **data)


    @staticmethod
    def timestamp() -> str:
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
            return 'Warn the user that there was an error formatting the timestamp.'

    @property_bag_item
    async def rendered_persona_prompt(self, context) -> str:
        base_prompt = context.get("persona_prompt", "")
        try:
            ws_name = context.get('primary_workspace',  self._extract_workspace_name(base_prompt))
            folder_depth = context.get('workspace_folder_depth', 5)
            file_depth = context.get('workspace_file_depth', 3)
            if ws_name:
                ws_tool = context['tool_chest'].available_tools.get('WorkspaceTools')

                tree = await ws_tool.tree(path=f"//{ws_name}/", folder_depth=folder_depth, file_depth=file_depth, tool_context=context)
                context['workspace_tree'] = f"Generated: {self.timestamp()}\n{tree}"

            template: Template = Template(base_prompt, )
            result = template.substitute(context)
        except Exception as e:
            logger.error(f"Error rendering persona prompt: {e}")
            return base_prompt
        return result

    @staticmethod
    def _extract_workspace_name(text) -> Optional[str]:
        """
        Extract the workspace name from a multiline string containing the pattern:
        The `workspace_name` workspace will be used

        Args:
            text (str): The multiline string to search in

        Returns:
            str or None: The extracted workspace name, or None if not found
        """
        pattern = r"The `([^`]+)` workspace will be used"
        match = re.search(pattern, text)
        if match:
            return match.group(1)
        return None
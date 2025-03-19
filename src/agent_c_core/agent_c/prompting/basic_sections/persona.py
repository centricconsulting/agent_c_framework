from typing import Any
from agent_c_core.agent_c.prompting.prompt_section import PromptSection


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
        TEMPLATE: str = "$persona_prompt"

        # Use the provided template or the default one
        data['template'] = data.get('template', TEMPLATE)

        # Initialize the base PromptSection with specific attributes
        super().__init__(name="Your Persona", render_section_header=True, **data)

from agent_c_core.agent_c.models.input.base import BaseInput

class TextInput(BaseInput):
    """Model representing text input with content.

    Attributes:
        content (str): The actual content of the text input
    """
    content: str

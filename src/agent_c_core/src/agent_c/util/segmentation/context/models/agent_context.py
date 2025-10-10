from pydantic import BaseModel

class AgentContextModel(BaseModel):
    """
    Stuffing Response Model Class
    Simple model to hold a basic stuffing response.

    Attributes:
        text: A string containing the formatted text for stuffing into the prompt
        token_count: An int with the number of tokens in the text.
    """

    text: str = ''
    token_count: int = 0
    source_count: int = 0
    segment_count: int = 0
from typing import List


from agent_c.util.segmentation.context.models import AgentContextModel
from agent_c.util.segmentation.weaviate.text_segment import TextSegment


class ContextFormatter:
    """
    Base class for formatters that transforms a collection of TextSegment models into a string.

    The formatted string is used as context information for stuffing into the prompt of an LLM.

    Args:
        segments (List[SegmentModel]): list of segments to process
        **kwargs: keyword arguments
            token_limit (int): the limit for the number of tokens. Default is 0(no limit).
            llm_model (str): the name of the model to use. Default is 'gpt-4'.
    """

    def __init__(self,  **kwargs):
        self.token_limit: int = kwargs.get('token_limit', 5000)
        self.token_counter = kwargs.get('token_counter') # token_counter
        self.source_count = 0
        self.segment_count = 0

    def format_segments(self, segments: List[TextSegment]) -> AgentContextModel:
        """
        Generates a formatted string of the input segments.

        The method is to be implemented in any class inheriting from StuffingFormatter.

        Returns:
            str: the formatted string

        Raises:
            NotImplementedError: If the method is not implemented
        """
        raise NotImplementedError("This method must be overridden in any class that inherits from StuffingFormatter.")

from typing import Dict, List

from agent_c.util.vector_transformers.vector_text_transformer import VectorTextTransformer
from agent_c.util.token_counter import TokenCounter


class ElementSegmenter:
    """
    A base class for classes that segment content based on elements

    ...

    Attributes
    ----------
    chunk_size : int
        the target size of the segment in tokens.
    token_counter : TokenCounter
        a TokenCounter object for counting the tokens in the text

    Methods
    -------
    segment_elements(elements: List, citation: Dict) -> List:
        Segments the given elements (not implemented)
    """

    def __init__(self, **kwargs) -> None:
        """
        Constructs all the necessary attributes for the ElementSegmenter object.

        Parameters
        ----------
            chunk_size : int, optional
                target size in tokens for the segment, defaults to 500
            toke_counter : TokenCounter
                a TokenCounter object for counting the tokens in the text
            vec_transformer : A VectorTextTransformer, optional
                defaults to the normal VectorTextTransformer with default options
        """

        self.chunk_size: int = kwargs['chunk_size']
        self.token_counter: TokenCounter = kwargs.get('token_counter', TokenCounter.counter())
        self.vec_transformer: VectorTextTransformer = kwargs.get('vec_transformer', VectorTextTransformer())

    def segment_elements(self, elements: List, meta: Dict) -> List:
        """Segments the given elements (not implemented)."""
        pass

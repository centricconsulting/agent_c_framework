import os
from pydantic import BaseModel, Field


class VectorTransformOptionsModel(BaseModel):
    """
    A Pydantic model for configuring the transformation of text prior to vectorization.

    Attributes:
    - make_lower (bool): If True, transforms all text to lowercase. Default is True. Configurable via
      the 'VT_MAKE_LOWER' environment variable.
    - remove_punct (bool): If True, removes all punctuation. Default is True. Configurable via
      the 'VT_REMOVE_PUNCT' environment variable.
    - remove_newlines (bool): If True, removes all newlines. Default is True. Configurable via
      the 'VT_REMOVE_NEWLINES' environment variable.
    - remove_unicode (bool): If True, removes all unicode characters. Default is False. Configurable via
      the 'VT_REMOVE_UNICODE' environment variable.
    - remove_markdown (bool): If True, removes all markdown syntax. Default is True. Configurable via
      the 'VT_REMOVE_MARKDOWN' environment variable.
    - strip_stop_words (bool): If True, removes all stop words. Default is True. Configurable via
      the 'VT_STRIP_STOP_WORDS' environment variable.
    - lemmatize (bool): If True, performs word lemmatization. Default is False. Configurable via
      the 'VT_LEMMATIZE' environment variable.
    - language (str): Specifies the language to be used in the process. Default is 'en_core_web_sm'. Configurable via
      the 'VT_LANGUAGE' environment variable.
    """

    make_lower: bool = Field(default_factory=lambda: os.getenv('VT_MAKE_LOWER', 'True').lower() == 'true')
    remove_punct: bool = Field(default_factory=lambda: os.getenv('VT_REMOVE_PUNCT', 'True').lower() == 'true')
    remove_newlines: bool = Field(default_factory=lambda: os.getenv('VT_REMOVE_NEWLINES', 'True').lower() == 'true')
    remove_unicode: bool = Field(default_factory=lambda: os.getenv('VT_REMOVE_UNICODE', 'False').lower() == 'true')
    remove_markdown: bool = Field(default_factory=lambda: os.getenv('VT_REMOVE_MARKDOWN', 'True').lower() == 'true')
    strip_stop_words: bool = Field(default_factory=lambda: os.getenv('VT_STRIP_STOP_WORDS', 'True').lower() == 'true')
    lemmatize: bool = Field(default_factory=lambda: os.getenv('VT_LEMMATIZE', 'False').lower() == 'true')
    language: str = Field(default_factory=lambda: os.getenv('VT_LANGUAGE', 'en_core_web_sm'))

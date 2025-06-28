from typing import TypeVar, Optional

from agent_c.util.logging_utils import LoggingManager
from agent_c.util import SingletonCacheMeta
from agent_c.config import ConfigLoader
from agent_c.models.prompts.base import BasePromptSection


T = TypeVar('T', bound=BasePromptSection)

_singleton_instance = None

class PromptSectionLoader(ConfigLoader, metaclass=SingletonCacheMeta):

    def __init__(self, config_path: Optional[str] = None):
        super().__init__(config_path)
        global _singleton_instance
        if _singleton_instance is None:
            _singleton_instance = self

        logging_manager = LoggingManager(__name__)
        self.logger = logging_manager.get_logger()

    @classmethod
    def mock(cls, mock_instance):
        """
        Mock the AgentConfigLoader instance for testing purposes.

        Args:
            mock_instance: The mock instance to use for testing
        """
        global _singleton_instance
        _singleton_instance = mock_instance

    @classmethod
    def instance(cls) -> 'AgentConfigLoader':
        """
        Get the singleton instance of AgentConfigLoader.

        If it doesn't exist, create a new one with default parameters.
        """
        global _singleton_instance
        if _singleton_instance is None:
            _singleton_instance = cls()
        return _singleton_instance
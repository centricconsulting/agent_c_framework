import os
from typing import Optional

from agent_c.util.logging_utils import LoggingManager


class ConfigLoader:
    def __init__(self, config_path: Optional[str] = None):
        """
        Initialize the loader.

        Args:
            config_path: Optional default path to configuration file
        """
        self.logger = LoggingManager(__name__).get_logger()
        self.config_path = os.environ.get("AGENT_C_CONFIG_PATH", None) if config_path is None else config_path
        if config_path is None:
            self.config_path = self._locate_config_path()

    @staticmethod
    def _locate_config_path():
        current_dir = os.getcwd()
        while True:
            if os.path.exists(os.path.join(current_dir, "agent_c_config")):
                return os.path.join(current_dir, "agent_c_config")
            if current_dir == os.path.dirname(current_dir):
                break
            current_dir = os.path.dirname(current_dir)

        raise FileNotFoundError("Configuration folder not found. Please ensure you are in the correct directory or set AGENT_C_CONFIG_PATH.")

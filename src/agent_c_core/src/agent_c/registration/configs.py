import os

from agent_c.models.config import * # noqa
from agent_c.prompting.config import PromptRenderingConfig # noqa

def get_system_config():
    from agent_c.config.system_config_loader import  SystemConfigurationLoader
    return SystemConfigurationLoader.instance().config

def get_config_registry():
    from agent_c.util.registries.config_registry import ConfigRegistry
    return ConfigRegistry

def locate_config_folder() -> str:
    """
    Locate configuration path by walking up directory tree.

    Returns:
        Path to agent_c_config directory

    Raises:
        FileNotFoundError: If configuration folder cannot be found
    """
    current_dir = os.getcwd()
    while True:
        config_dir = os.path.join(current_dir, "agent_c_config")
        if os.path.exists(config_dir):
            return config_dir

        parent_dir = os.path.dirname(current_dir)
        if current_dir == parent_dir:  # Reached root directory
            break
        current_dir = parent_dir

    raise FileNotFoundError(
        "Configuration folder not found. Please ensure you are in the correct directory or set AGENT_C_CONFIG_PATH."
    )

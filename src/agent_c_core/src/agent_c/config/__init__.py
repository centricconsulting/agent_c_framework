import os

from agent_c.config.model_config_loader import ModelConfigurationLoader


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

__all__ = ["ModelConfigurationLoader", "locate_config_folder"]
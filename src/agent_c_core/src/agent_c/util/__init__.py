import os

from agent_c.util.detect_debugger import debugger_is_active
from agent_c.util.token_counter import TokenCounter
from agent_c.util.dict import filter_dict_by_keys
from agent_c.util.slugs import MnemonicSlugs
from agent_c.util.string import to_snake_case, generate_path_tree
from agent_c.util.singleton_cache import (
    SingletonCacheMeta, 
    CacheManager, 
    ThreadSafeLRUCache, 
    SharedCacheRegistry, 
    shared_cache_registry,
    CacheNames
)


def locate_project_root_folder() -> str:
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
            return current_dir

        parent_dir = os.path.dirname(current_dir)
        if current_dir == parent_dir:  # Reached root directory
            break

        current_dir = parent_dir

    raise FileNotFoundError(
        "root folder not found. Please ensure you are in the correct directory or set AGENT_C_CONFIG_PATH."
    )

__all__ = [
    "debugger_is_active",
    "TokenCounter",
    "filter_dict_by_keys",
    "MnemonicSlugs",
    "to_snake_case",
    "generate_path_tree",
    "SingletonCacheMeta",
    "CacheManager",
    "ThreadSafeLRUCache",
    "SharedCacheRegistry",
    "shared_cache_registry",
    "CacheNames",
    "locate_project_root_folder"
]
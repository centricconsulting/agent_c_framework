import os

from typing import Optional

from agent_c.util.logging_utils import LoggingManager
from agent_c.util import SingletonCacheMeta, shared_cache_registry, CacheNames

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

class ConfigLoader(metaclass=SingletonCacheMeta):
    def __init__(self, config_path: Optional[str] = None):
        """
        Initialize the loader.

        Args:
            config_path: Optional default path to configuration file
        """
        self.logger = LoggingManager(self.__class__.__name__).get_logger()
        
        # Handle config path resolution with caching
        if config_path is not None:
            self.config_path = config_path
        else:
            # Check environment variable first
            env_path = os.environ.get("AGENT_C_CONFIG_PATH", None)
            if env_path is not None:
                self.config_path = env_path
            else:
                # Use cached path resolution
                self.config_path = self._get_cached_config_path()

    @staticmethod
    def _get_cached_config_path() -> str:
        """
        Get config path using cached path resolution.
        
        Returns:
            Resolved configuration path
            
        Raises:
            FileNotFoundError: If configuration folder cannot be found
        """
        # Use current working directory as cache key since path resolution is relative to cwd
        cache_key = f"config_path:{os.getcwd()}"
        path_cache = shared_cache_registry.get_cache(CacheNames.PATH_RESOLUTION, max_size=50, ttl_seconds=300)
        
        return path_cache.get_or_compute(cache_key, locate_config_folder)

    
    @classmethod
    def clear_path_cache(cls) -> int:
        """
        Clear the path resolution cache.
        
        Returns:
            Number of cache entries cleared
        """
        return shared_cache_registry.clear_cache(CacheNames.PATH_RESOLUTION)
    
    @classmethod
    def get_cache_stats(cls) -> dict:
        """
        Get cache statistics for ConfigLoader.
        
        Returns:
            Dictionary with cache statistics including singleton and path resolution stats
        """
        path_cache = shared_cache_registry.get_cache(CacheNames.PATH_RESOLUTION)
        
        return {
            'singleton_stats': super().get_cache_stats() if hasattr(super(), 'get_cache_stats') else {},
            'path_resolution_stats': path_cache.get_stats()
        }
    
    @classmethod
    def invalidate_cache(cls, config_path: Optional[str] = None) -> dict:
        """
        Invalidate ConfigLoader caches.
        
        Args:
            config_path: If provided, invalidate singleton instance for this path
            
        Returns:
            Dictionary with invalidation results
        """
        results = {
            'singleton_invalidated': False,
            'path_cache_cleared': 0
        }
        
        # Invalidate specific singleton instance if path provided
        if config_path is not None:
            results['singleton_invalidated'] = cls.invalidate_instance(config_path=config_path)
        
        # Clear path resolution cache
        results['path_cache_cleared'] = cls.clear_path_cache()
        
        return results

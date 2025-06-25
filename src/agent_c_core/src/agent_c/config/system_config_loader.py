"""
Loader class for model configuration data.

This module provides a loader class to handle loading, parsing, and saving
of model configurations from YAML files. Enhanced with singleton pattern
and shared caching for optimal performance.
"""
import yaml
import threading

from pathlib import Path
from typing import Union, Dict, Any, Optional, List

from agent_c.models import BaseConfig
from agent_c.util import SingletonCacheMeta, shared_cache_registry, CacheNames

from agent_c.config.config_loader import ConfigLoader
from agent_c.models.config.system_config import SystemConfigFile

_singleton_instance = None

class SystemConfigurationLoader(ConfigLoader, metaclass=SingletonCacheMeta):
    """
    Loader for model configuration files.
    
    Handles loading, parsing, validation, and saving of model configuration
    data from YAML files.
    """
    
    def __init__(self, config_path: Optional[str] = None):
        super().__init__(config_path)
        global _singleton_instance
        if _singleton_instance is None:
            _singleton_instance = self

        self.config_file_path = Path(self.config_path).joinpath("config.yaml")
        self.default_config_file_path = Path(self.config_path).joinpath("default_config.yaml")
        self._cached_config: Optional['SystemConfigFile'] = None
        self._file_lock = threading.RLock()  # Thread-safe file operations
        
        # Load configuration using enhanced caching
        self.load_from_yaml()

    @classmethod
    def mock(cls, mock_instance):
        """
        Mock the SystemConfigurationLoader instance for testing purposes.

        Args:
            mock_instance: The mock instance to use for testing
        """
        global _singleton_instance
        _singleton_instance = mock_instance

    @classmethod
    def default_config_model(cls) -> 'SystemConfigFile':
        from agent_c.util.registries.config_registry import ConfigRegistry
        from agent_c.models.config.base import CONFIG_RUNTIME, CONFIG_TOOLSETS, CONFIG_CORE, CONFIG_API, CONFIG_MISC

        return SystemConfigFile(
            runtimes=ConfigRegistry.get_default_models_in_category(CONFIG_RUNTIME),
            core=ConfigRegistry.get_default_models_in_category(CONFIG_CORE),
            tools=ConfigRegistry.get_default_models_in_category(CONFIG_TOOLSETS),
            api=ConfigRegistry.get_default_models_in_category(CONFIG_API),
            misc=ConfigRegistry.get_default_models_in_category(CONFIG_MISC)
        )


    def _ensure_default_config_file_exists(self):
        """
        Ensure the default configuration model exists.

        Returns:
            SystemConfigFile instance with default configuration
        """
        if not self.default_config_file_path.exists():
            # Create default configuration if it doesn't exist
            default_config = self.default_config_model()
            self.save_to_yaml(default_config, yaml_path=self.default_config_file_path)

    @classmethod
    def instance(cls) -> 'SystemConfigurationLoader':
        """
        Get the singleton instance of SystemConfigurationLoader.

        Returns:
            SystemConfigurationLoader instance
        """
        global _singleton_instance
        if _singleton_instance is None:
            _singleton_instance = cls()

        return _singleton_instance

    @property
    def config(self) -> SystemConfigFile:
        """
        Get the cached model configuration if available, otherwise load it.

        Returns:
            SystemConfigurationFile instance with the loaded configuration
        """
        if self._cached_config is None:
            self.load_from_yaml()

        return self._cached_config

  
    def load_from_yaml(self, yaml_path: Optional[Union[str, Path]] = None) -> SystemConfigFile:
        """
        Load model configuration from a YAML file with enhanced caching.
        
        Args:
            yaml_path: Path to the yaml configuration file (uses default if None)
            
        Returns:
            SystemConfigurationFile instance with the loaded configuration
            
        Raises:
            FileNotFoundError: If the YAML file doesn't exist
            yaml.YAMLDecodeError: If the YAML is malformed
            ValidationError: If the YAML doesn't match the expected schema
        """
        path = Path(yaml_path) if yaml_path else self.config_file_path
        
        if not path:
            raise ValueError("No configuration path provided")
        
        if not path.exists():
            from agent_c.util.logging_utils import LoggingManager
            logger = LoggingManager(self.__class__. __name__).get_logger()
            logger.warning(f"Configuration file not found at {path}. Attempting to load default configuration.")
            self._ensure_default_config_file_exists()
            path = self.default_config_file_path
            if not path.exists():
                raise FileNotFoundError(f"Configuration file not found: {path}")
        
        # Use cached loading with file modification time checking
        with self._file_lock:
            config = self._load_yaml_cached(path)
            self._cached_config = config
            return config
    
    def load_from_dict(self, config_data: Dict[str, Any]) -> SystemConfigFile:
        """
        Load model configuration from a dictionary.
        
        Args:
            config_data: Dictionary containing the configuration data
            
        Returns:
            SystemConfigurationFile instance with the loaded configuration
            
        Raises:
            ValidationError: If the data doesn't match the expected schema
        """
        self._cached_config = SystemConfigFile.model_validate(config_data)
        return self._cached_config
    
    def save_to_yaml(
        self, 
        config: SystemConfigFile,
        yaml_path: Optional[Union[str, Path]] = None
    ) -> None:
        """
        Save model configuration to a YAML file.
        
        Args:
            config: SystemConfigurationFile instance to save
            yaml_path: Path where to save the YAML  file (uses default if None)
        """
        path = Path(yaml_path) if yaml_path else self.config_file_path
        
        if not path:
            raise ValueError("No save path provided")
        
        # Ensure parent directory exists
        path.parent.mkdir(parents=True, exist_ok=True)
        
        with open(path, 'w', encoding='utf-8') as f:
            yaml.dump(config.model_dump_yaml(), f, allow_unicode=True, default_flow_style=False, sort_keys=False)

    
    def validate_file(self, yaml_path: Optional[Union[str, Path]] = None) -> bool:
        """
        Validate a model configuration YAML file.
        
        Args:
            yaml_path: Path to the YAML configuration file (uses default if None)
            
        Returns:
            True if the file is valid, False otherwise
        """
        try:
            self.load_from_yaml(yaml_path)
            return True
        except Exception:
            return False
    
    def get_cached_config(self) -> Optional[SystemConfigFile]:
        """
        Get the cached configuration if available.
        
        Returns:
            Cached SystemConfigurationFile or None if not loaded
        """
        return self._cached_config
    
    def reload(self) -> SystemConfigFile:
        """
        Reload configuration from the default path, bypassing cache.
        
        Returns:
            Reloaded SystemConfigurationFile
            
        Raises:
            ValueError: If no default path is set
        """
        if not self.config_file_path:
            raise ValueError("No default configuration path set")
        
        # Clear cache for this file to force reload
        self._invalidate_file_cache(self.config_file_path)
        return self.load_from_yaml()
    
    def _load_yaml_cached(self, path: Path) -> SystemConfigFile:
        """
        Load YAML file with shared caching based on file path and modification time.
        
        Args:
            path: Path to the YAML file
            
        Returns:
            Parsed SystemConfigurationFile instance
        """
        # Get file stats for cache key
        try:
            stat = path.stat()
            file_mtime = stat.st_mtime
            file_size = stat.st_size
        except OSError as e:
            raise FileNotFoundError(f"Configuration file not found: {path}") from e
        
        # Create cache key including file path, mtime, and size
        cache_key = f"model_config:{path.resolve()}:{file_mtime}:{file_size}"
        
        # Get caches
        yaml_cache = shared_cache_registry.get_cache(CacheNames.YAML_PARSING, max_size=50, ttl_seconds=3600)
        model_cache = shared_cache_registry.get_cache(CacheNames.CONFIG_FILES, max_size=500, ttl_seconds=3600)
        
        # Try to get parsed model from cache first
        cached_model = model_cache.get(cache_key)
        if cached_model is not None:
            return cached_model
        
        # Try to get raw YAML data from cache
        def load_and_parse_yaml():
            # Load raw YAML with caching
            yaml_data = yaml_cache.get_or_compute(
                cache_key,
                lambda: self._load_raw_yaml(path)
            )
            
            # Parse and validate YAML data
            return SystemConfigFile.model_validate(yaml_data)
        
        # Get or compute the parsed model
        model = model_cache.get_or_compute(cache_key, load_and_parse_yaml)
        return model
    
    def _load_raw_yaml(self, path: Path) -> Dict[str, Any]:
        """
        Load raw YAML data from file.
        
        Args:
            path: Path to the YAML file
            
        Returns:
            Raw YAML data as dictionary
        """
        with open(path, 'r', encoding='utf-8') as f:
            return yaml.load(f, yaml.FullLoader)
    
    def _invalidate_file_cache(self, path: Path) -> None:
        """
        Invalidate cached data for a specific file.
        
        Args:
            path: Path to the file to invalidate
        """
        # We can't easily invalidate by partial key, so we clear the entire caches
        # This is acceptable since model configs don't change frequently
        shared_cache_registry.clear_cache(CacheNames.YAML_PARSING)
        shared_cache_registry.clear_cache(CacheNames.CONFIG_FILES)
    
    @classmethod
    def clear_model_caches(cls) -> Dict[str, int]:
        """
        Clear all model configuration caches.
        
        Returns:
            Dictionary with cache clear results
        """
        return {
            'yaml_cache_cleared': shared_cache_registry.clear_cache(CacheNames.YAML_PARSING),
            'model_cache_cleared': shared_cache_registry.clear_cache(CacheNames.CONFIG_FILES)
        }
    
    @classmethod
    def get_cache_stats(cls) -> Dict[str, Any]:
        """
        Get comprehensive cache statistics for SystemConfigurationLoader.
        
        Returns:
            Dictionary with cache statistics
        """
        yaml_cache = shared_cache_registry.get_cache(CacheNames.YAML_PARSING)
        model_cache = shared_cache_registry.get_cache(CacheNames.CONFIG_FILES)
        
        return {
            'singleton_stats': super().get_cache_stats() if hasattr(super(), 'get_cache_stats') else {},
            'yaml_parsing_stats': yaml_cache.get_stats(),
            'model_config_stats': model_cache.get_stats()
        }
    
    @classmethod
    def invalidate_cache(cls, config_path: Optional[str] = None) -> Dict[str, Any]:
        """
        Invalidate SystemConfigurationLoader caches.
        
        Args:
            config_path: If provided, invalidate singleton instance for this path
            
        Returns:
            Dictionary with invalidation results
        """
        results = {
            'singleton_invalidated': False,
            'caches_cleared': {}
        }
        
        # Invalidate specific singleton instance if path provided
        if config_path is not None:
            results['singleton_invalidated'] = cls.invalidate_instance(config_path=config_path)
        
        # Clear model configuration caches
        results['caches_cleared'] = cls.clear_model_caches()
        
        return results
    
    def invalidate_file_cache(self, yaml_path: Optional[Union[str, Path]] = None) -> None:
        """
        Invalidate cache for a specific YAML file.
        
        Args:
            yaml_path: Path to the YAML file (uses default if None)
        """
        path = Path(yaml_path) if yaml_path else self.config_file_path
        self._invalidate_file_cache(path)


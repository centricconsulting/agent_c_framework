"""
Loader class for model configuration data.

This module provides a loader class to handle loading, parsing, and saving
of model configurations from JSON files. Enhanced with singleton pattern
and shared caching for optimal performance.
"""
import json
import threading

from pathlib import Path
from typing import Union, Dict, Any, Optional, List
from agent_c.util import SingletonCacheMeta, shared_cache_registry, CacheNames

from agent_c.models.config.model_config.vendors import ModelConfigurationFile
from agent_c.models.config.model_config.models import ModelConfigurationWithVendor
from agent_c.config.config_loader import ConfigLoader


_singleton_instance = None

class ModelConfigurationLoader(ConfigLoader, metaclass=SingletonCacheMeta):
    """
    Loader for model configuration files.
    
    Handles loading, parsing, validation, and saving of model configuration
    data from JSON files.
    """
    
    def __init__(self, config_path: Optional[str] = None):
        super().__init__(config_path)
        global _singleton_instance
        if _singleton_instance is None:
            _singleton_instance = self

        self.config_file_path = Path(self.config_path).joinpath("model_configs.json")
        self._cached_config: Optional[ModelConfigurationFile] = None
        self._file_lock = threading.RLock()  # Thread-safe file operations
        
        # Load configuration using enhanced caching
        self.load_from_json()

    @classmethod
    def instance(cls) -> 'ModelConfigurationLoader':
        """
        Get the singleton instance of ModelConfigurationLoader.

        Returns:
            ModelConfigurationLoader instance
        """
        global _singleton_instance
        if _singleton_instance is None:
            _singleton_instance = cls()

        return _singleton_instance

    @property
    def model_configs(self) -> ModelConfigurationFile:
        """
        Get the cached model configuration if available, otherwise load it.

        Returns:
            ModelConfigurationFile instance with the loaded configuration
        """
        if self._cached_config is None:
            self.load_from_json()

        return self._cached_config

    def flattened_config(self) -> Dict[str, Any]:
        """
        Flatten the cached model configuration into a dictionary.

        Returns:
            Dictionary representation of the cached configuration
        """
        if self._cached_config is None:
            self.load_from_json()

        data = self._cached_config.model_dump(exclude_none=True)
        result: Dict[str, Any] = {}
        for vendor_info in data["vendors"]:
            vendor_name = vendor_info["vendor"]
            for model in vendor_info["models"]:
                model_with_vendor = model.copy()
                model_with_vendor["vendor"] = vendor_name
                result[model["id"]] = model_with_vendor

        return result

    def default_params_for_model(self, model_id: str) -> Dict[str, Any]:
        """
        Get default parameters for a specific model by its ID.

        Args:
            model_id: ID of the model to get default parameters for

        Returns:
            Dictionary of default parameters for the specified model
        """
        model = self.model_id_map.get(model_id)
        if not model:
            raise ValueError(f"Model with ID '{model_id}' not found in configuration")

        return model.default_completion_params

    @property
    def model_id_map(self) -> Dict[str, ModelConfigurationWithVendor]:
        """
        Get a mapping of model IDs to their configurations with vendor information.

        Returns:
            Dictionary mapping model IDs to ModelConfigurationWithVendor instances
        """
        return {model.id: model for model in self.model_list}

    @property
    def model_list(self) -> List[ModelConfigurationWithVendor]:
        models: List[ModelConfigurationWithVendor] = []
        for vendor in self.model_configs.vendors:
            models.extend([ModelConfigurationWithVendor(vendor=vendor.vendor, **model.model_dump()) for model in vendor.models])


        return models

    def load_from_json(self, json_path: Optional[Union[str, Path]] = None) -> ModelConfigurationFile:
        """
        Load model configuration from a JSON file with enhanced caching.
        
        Args:
            json_path: Path to the JSON configuration file (uses default if None)
            
        Returns:
            ModelConfigurationFile instance with the loaded configuration
            
        Raises:
            FileNotFoundError: If the JSON file doesn't exist
            json.JSONDecodeError: If the JSON is malformed
            ValidationError: If the JSON doesn't match the expected schema
        """
        path = Path(json_path) if json_path else self.config_file_path
        
        if not path:
            raise ValueError("No configuration path provided")
        
        if not path.exists():
            raise FileNotFoundError(f"Configuration file not found: {path}")
        
        # Use cached loading with file modification time checking
        with self._file_lock:
            config = self._load_json_cached(path)
            self._cached_config = config
            return config
    
    def load_from_dict(self, config_data: Dict[str, Any]) -> ModelConfigurationFile:
        """
        Load model configuration from a dictionary.
        
        Args:
            config_data: Dictionary containing the configuration data
            
        Returns:
            ModelConfigurationFile instance with the loaded configuration
            
        Raises:
            ValidationError: If the data doesn't match the expected schema
        """
        self._cached_config = ModelConfigurationFile.model_validate(config_data)
        return self._cached_config
    
    def save_to_json(
        self, 
        config: ModelConfigurationFile,
        json_path: Optional[Union[str, Path]] = None,
        indent: int = 2
    ) -> None:
        """
        Save model configuration to a JSON file.
        
        Args:
            config: ModelConfigurationFile instance to save
            json_path: Path where to save the JSON file (uses default if None)
            indent: JSON indentation level (default: 2)
        """
        path = Path(json_path) if json_path else self.config_file_path
        
        if not path:
            raise ValueError("No save path provided")
        
        # Ensure parent directory exists
        path.parent.mkdir(parents=True, exist_ok=True)
        
        with open(path, 'w', encoding='utf-8') as f:
            json.dump(
                config.model_dump(mode='json'),
                f,
                indent=indent,
                ensure_ascii=False
            )
    
    def validate_file(self, json_path: Optional[Union[str, Path]] = None) -> bool:
        """
        Validate a model configuration JSON file.
        
        Args:
            json_path: Path to the JSON configuration file (uses default if None)
            
        Returns:
            True if the file is valid, False otherwise
        """
        try:
            self.load_from_json(json_path)
            return True
        except Exception:
            return False
    
    def get_cached_config(self) -> Optional[ModelConfigurationFile]:
        """
        Get the cached configuration if available.
        
        Returns:
            Cached ModelConfigurationFile or None if not loaded
        """
        return self._cached_config
    
    def reload(self) -> ModelConfigurationFile:
        """
        Reload configuration from the default path, bypassing cache.
        
        Returns:
            Reloaded ModelConfigurationFile
            
        Raises:
            ValueError: If no default path is set
        """
        if not self.config_file_path:
            raise ValueError("No default configuration path set")
        
        # Clear cache for this file to force reload
        self._invalidate_file_cache(self.config_file_path)
        return self.load_from_json()
    
    def _load_json_cached(self, path: Path) -> ModelConfigurationFile:
        """
        Load JSON file with shared caching based on file path and modification time.
        
        Args:
            path: Path to the JSON file
            
        Returns:
            Parsed ModelConfigurationFile instance
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
        json_cache = shared_cache_registry.get_cache(CacheNames.JSON_PARSING, max_size=100, ttl_seconds=3600)
        model_cache = shared_cache_registry.get_cache(CacheNames.MODEL_CONFIGS, max_size=50, ttl_seconds=3600)
        
        # Try to get parsed model from cache first
        cached_model = model_cache.get(cache_key)
        if cached_model is not None:
            return cached_model
        
        # Try to get raw JSON data from cache
        def load_and_parse_json():
            # Load raw JSON with caching
            json_data = json_cache.get_or_compute(
                cache_key,
                lambda: self._load_raw_json(path)
            )
            
            # Parse and validate JSON data
            return ModelConfigurationFile.model_validate(json_data)
        
        # Get or compute the parsed model
        model = model_cache.get_or_compute(cache_key, load_and_parse_json)
        return model
    
    def _load_raw_json(self, path: Path) -> Dict[str, Any]:
        """
        Load raw JSON data from file.
        
        Args:
            path: Path to the JSON file
            
        Returns:
            Raw JSON data as dictionary
        """
        with open(path, 'r', encoding='utf-8') as f:
            return json.load(f)
    
    def _invalidate_file_cache(self, path: Path) -> None:
        """
        Invalidate cached data for a specific file.
        
        Args:
            path: Path to the file to invalidate
        """
        # We can't easily invalidate by partial key, so we clear the entire caches
        # This is acceptable since model configs don't change frequently
        shared_cache_registry.clear_cache(CacheNames.JSON_PARSING)
        shared_cache_registry.clear_cache(CacheNames.MODEL_CONFIGS)
    
    @classmethod
    def clear_model_caches(cls) -> Dict[str, int]:
        """
        Clear all model configuration caches.
        
        Returns:
            Dictionary with cache clear results
        """
        return {
            'json_cache_cleared': shared_cache_registry.clear_cache(CacheNames.JSON_PARSING),
            'model_cache_cleared': shared_cache_registry.clear_cache(CacheNames.MODEL_CONFIGS)
        }
    
    @classmethod
    def get_cache_stats(cls) -> Dict[str, Any]:
        """
        Get comprehensive cache statistics for ModelConfigurationLoader.
        
        Returns:
            Dictionary with cache statistics
        """
        json_cache = shared_cache_registry.get_cache(CacheNames.JSON_PARSING)
        model_cache = shared_cache_registry.get_cache(CacheNames.MODEL_CONFIGS)
        
        return {
            'singleton_stats': super().get_cache_stats() if hasattr(super(), 'get_cache_stats') else {},
            'json_parsing_stats': json_cache.get_stats(),
            'model_config_stats': model_cache.get_stats()
        }
    
    @classmethod
    def invalidate_cache(cls, config_path: Optional[str] = None) -> Dict[str, Any]:
        """
        Invalidate ModelConfigurationLoader caches.
        
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
    
    def invalidate_file_cache(self, json_path: Optional[Union[str, Path]] = None) -> None:
        """
        Invalidate cache for a specific JSON file.
        
        Args:
            json_path: Path to the JSON file (uses default if None)
        """
        path = Path(json_path) if json_path else self.config_file_path
        self._invalidate_file_cache(path)


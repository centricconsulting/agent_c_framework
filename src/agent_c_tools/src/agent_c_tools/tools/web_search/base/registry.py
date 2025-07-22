"""
Engine registry and discovery system.

This module provides the registry system for discovering, registering,
and managing web search engines within the unified search system.
"""

from typing import Dict, List, Optional, Type, Set
import importlib
from pathlib import Path
from agent_c.util.structured_logging import get_logger

from .engine import BaseWebSearchEngine
from .models import WebSearchConfig, SearchType, EngineHealthStatus

logger = get_logger(__name__)


class EngineRegistry:
    """
    Registry for managing web search engines.
    
    The registry handles engine discovery, registration, and lifecycle management.
    It provides a centralized way to access and manage all available search engines.
    """
    
    def __init__(self):
        """Initialize the engine registry."""
        self._engines: Dict[str, BaseWebSearchEngine] = {}
        self._engine_classes: Dict[str, Type[BaseWebSearchEngine]] = {}
        self._configs: Dict[str, WebSearchConfig] = {}
        self._discovery_paths: List[Path] = []
        
    def register_engine_class(
        self, 
        engine_name: str, 
        engine_class: Type[BaseWebSearchEngine],
        config: WebSearchConfig
    ) -> None:
        """
        Register an engine class with the registry.
        
        Args:
            engine_name: Unique name for the engine
            engine_class: Engine class that inherits from BaseWebSearchEngine
            config: Configuration for the engine
        """
        if not issubclass(engine_class, BaseWebSearchEngine):
            raise ValueError(f"Engine class must inherit from BaseWebSearchEngine")
        
        if engine_name in self._engine_classes:
            logger.warning(f"Overriding existing engine registration: {engine_name}")
        
        self._engine_classes[engine_name] = engine_class
        self._configs[engine_name] = config
        
        logger.info(f"Registered engine class: {engine_name}")
    
    def register_engine_instance(
        self, 
        engine_name: str, 
        engine_instance: BaseWebSearchEngine
    ) -> None:
        """
        Register a pre-configured engine instance.
        
        Args:
            engine_name: Unique name for the engine
            engine_instance: Configured engine instance
        """
        if not isinstance(engine_instance, BaseWebSearchEngine):
            raise ValueError(f"Engine must be instance of BaseWebSearchEngine")
        
        if engine_name in self._engines:
            logger.warning(f"Overriding existing engine instance: {engine_name}")
        
        self._engines[engine_name] = engine_instance
        self._configs[engine_name] = engine_instance.config
        
        logger.info(f"Registered engine instance: {engine_name}")
    
    def get_engine(self, engine_name: str) -> Optional[BaseWebSearchEngine]:
        """
        Get an engine instance by name.
        
        If the engine is not already instantiated, it will be created from
        the registered class and configuration.
        
        Args:
            engine_name: Name of the engine to retrieve
            
        Returns:
            Engine instance or None if not found
        """
        # Return existing instance if available
        if engine_name in self._engines:
            return self._engines[engine_name]
        
        # Try to create instance from registered class
        if engine_name in self._engine_classes:
            try:
                engine_class = self._engine_classes[engine_name]
                config = self._configs[engine_name]
                
                engine_instance = engine_class(config)
                self._engines[engine_name] = engine_instance
                
                logger.info(f"Created engine instance: {engine_name}")
                return engine_instance
                
            except Exception as e:
                logger.error(f"Failed to create engine instance {engine_name}: {e}")
                return None
        
        logger.warning(f"Engine not found: {engine_name}")
        return None
    
    def get_available_engines(self) -> List[str]:
        """
        Get list of all registered engine names.
        
        Returns:
            List of engine names
        """
        all_engines = set(self._engines.keys()) | set(self._engine_classes.keys())
        return sorted(list(all_engines))
    
    def get_engines_for_search_type(self, search_type: SearchType) -> List[str]:
        """
        Get engines that support a specific search type.
        
        Args:
            search_type: The search type to filter by
            
        Returns:
            List of engine names that support the search type
        """
        supporting_engines = []
        
        for engine_name in self.get_available_engines():
            config = self._configs.get(engine_name)
            if config and config.capabilities.supports_search_type(search_type):
                supporting_engines.append(engine_name)
        
        return supporting_engines
    
    def get_healthy_engines(self) -> List[str]:
        """
        Get engines that are currently available and healthy.
        
        Returns:
            List of healthy engine names
        """
        healthy_engines = []
        
        for engine_name in self.get_available_engines():
            engine = self.get_engine(engine_name)
            if engine and engine.is_available():
                healthy_engines.append(engine_name)
        
        return healthy_engines
    
    def get_engines_with_api_keys(self) -> List[str]:
        """
        Get engines that have their required API keys configured.
        
        Returns:
            List of engine names with valid API key configuration
        """
        configured_engines = []
        
        for engine_name in self.get_available_engines():
            config = self._configs.get(engine_name)
            if config and config.is_api_key_configured():
                configured_engines.append(engine_name)
        
        return configured_engines
    
    def get_health_status_all(self) -> Dict[str, EngineHealthStatus]:
        """
        Get health status for all registered engines.
        
        Returns:
            Dictionary mapping engine names to their health status
        """
        health_status = {}
        
        for engine_name in self.get_available_engines():
            engine = self.get_engine(engine_name)
            if engine:
                health_status[engine_name] = engine.get_health_status()
            else:
                # Create basic status for engines that failed to initialize
                from datetime import datetime
                health_status[engine_name] = EngineHealthStatus(
                    engine_name=engine_name,
                    is_available=False,
                    last_check=datetime.now(),
                    error_message="Failed to initialize engine",
                    api_key_configured=self._configs[engine_name].is_api_key_configured() if engine_name in self._configs else False
                )
        
        return health_status
    
    def discover_engines(self, discovery_path: Path) -> int:
        """
        Discover and register engines from a directory path.
        
        This method scans the given directory for engine implementations
        and automatically registers them.
        
        Args:
            discovery_path: Path to scan for engine implementations
            
        Returns:
            Number of engines discovered and registered
        """
        if discovery_path in self._discovery_paths:
            logger.info(f"Path already discovered: {discovery_path}")
            return 0
        
        self._discovery_paths.append(discovery_path)
        discovered_count = 0
        
        try:
            # Look for engine directories
            if discovery_path.exists() and discovery_path.is_dir():
                for engine_dir in discovery_path.iterdir():
                    if engine_dir.is_dir() and not engine_dir.name.startswith('_'):
                        try:
                            count = self._discover_engine_in_directory(engine_dir)
                            discovered_count += count
                        except Exception as e:
                            logger.error(f"Failed to discover engine in {engine_dir}: {e}")
            
            logger.info(f"Discovered {discovered_count} engines from {discovery_path}")
            return discovered_count
            
        except Exception as e:
            logger.error(f"Engine discovery failed for {discovery_path}: {e}")
            return 0
    
    def _discover_engine_in_directory(self, engine_dir: Path) -> int:
        """
        Discover engine in a specific directory.
        
        Args:
            engine_dir: Directory containing engine implementation
            
        Returns:
            Number of engines discovered (0 or 1)
        """
        # Look for engine.py or tool.py files
        engine_files = ['engine.py', 'tool.py']
        config_files = ['config.py', 'config.json']
        
        engine_file = None
        for filename in engine_files:
            file_path = engine_dir / filename
            if file_path.exists():
                engine_file = file_path
                break
        
        if not engine_file:
            logger.debug(f"No engine file found in {engine_dir}")
            return 0
        
        try:
            # Import the engine module
            module_name = f"engines.{engine_dir.name}.{engine_file.stem}"
            spec = importlib.util.spec_from_file_location(module_name, engine_file)
            module = importlib.util.module_from_spec(spec)
            spec.loader.exec_module(module)
            
            # Look for engine classes
            engine_classes = []
            for attr_name in dir(module):
                attr = getattr(module, attr_name)
                if (isinstance(attr, type) and 
                    issubclass(attr, BaseWebSearchEngine) and 
                    attr != BaseWebSearchEngine):
                    engine_classes.append(attr)
            
            if not engine_classes:
                logger.debug(f"No engine classes found in {engine_file}")
                return 0
            
            # Register the first engine class found
            engine_class = engine_classes[0]
            engine_name = engine_dir.name
            
            # Try to load configuration
            config = self._load_engine_config(engine_dir, engine_name)
            
            self.register_engine_class(engine_name, engine_class, config)
            return 1
            
        except Exception as e:
            logger.error(f"Failed to load engine from {engine_file}: {e}")
            return 0
    
    def _load_engine_config(self, engine_dir: Path, engine_name: str) -> WebSearchConfig:
        """
        Load configuration for an engine.
        
        Args:
            engine_dir: Directory containing engine files
            engine_name: Name of the engine
            
        Returns:
            WebSearchConfig object
        """
        # Try to load from config.py or config.json
        config_py = engine_dir / 'config.py'
        config_json = engine_dir / 'config.json'
        
        if config_py.exists():
            try:
                spec = importlib.util.spec_from_file_location(f"config.{engine_name}", config_py)
                config_module = importlib.util.module_from_spec(spec)
                spec.loader.exec_module(config_module)
                
                if hasattr(config_module, 'CONFIG'):
                    config_dict = config_module.CONFIG
                    return WebSearchConfig(engine_name=engine_name, **config_dict)
                    
            except Exception as e:
                logger.warning(f"Failed to load config.py for {engine_name}: {e}")
        
        if config_json.exists():
            try:
                import json
                with open(config_json, 'r') as f:
                    config_dict = json.load(f)
                return WebSearchConfig(engine_name=engine_name, **config_dict)
                
            except Exception as e:
                logger.warning(f"Failed to load config.json for {engine_name}: {e}")
        
        # Return default configuration
        logger.info(f"Using default configuration for {engine_name}")
        return WebSearchConfig(engine_name=engine_name)
    
    def unregister_engine(self, engine_name: str) -> bool:
        """
        Unregister an engine from the registry.
        
        Args:
            engine_name: Name of the engine to unregister
            
        Returns:
            True if engine was unregistered, False if not found
        """
        found = False
        
        if engine_name in self._engines:
            del self._engines[engine_name]
            found = True
        
        if engine_name in self._engine_classes:
            del self._engine_classes[engine_name]
            found = True
        
        if engine_name in self._configs:
            del self._configs[engine_name]
            found = True
        
        if found:
            logger.info(f"Unregistered engine: {engine_name}")
        
        return found
    
    def clear_registry(self) -> None:
        """Clear all registered engines and configurations."""
        self._engines.clear()
        self._engine_classes.clear()
        self._configs.clear()
        self._discovery_paths.clear()
        logger.info("Cleared engine registry")
    
    def get_registry_stats(self) -> Dict[str, int]:
        """
        Get statistics about the registry.
        
        Returns:
            Dictionary with registry statistics
        """
        return {
            'total_engines': len(self.get_available_engines()),
            'instantiated_engines': len(self._engines),
            'registered_classes': len(self._engine_classes),
            'healthy_engines': len(self.get_healthy_engines()),
            'configured_engines': len(self.get_engines_with_api_keys()),
            'discovery_paths': len(self._discovery_paths)
        }


# Global registry instance
_global_registry: Optional[EngineRegistry] = None


def get_global_registry() -> EngineRegistry:
    """
    Get the global engine registry instance.
    
    Returns:
        Global EngineRegistry instance
    """
    global _global_registry
    if _global_registry is None:
        _global_registry = EngineRegistry()
    return _global_registry


def reset_global_registry() -> None:
    """Reset the global registry (primarily for testing)."""
    global _global_registry
    _global_registry = None
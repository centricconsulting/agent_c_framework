import os
import glob
import yaml
from pathlib import Path
from typing import Dict, Any, Optional, List, TypeVar

from agent_c.config import ModelConfigurationLoader
from agent_c.config.config_loader import ConfigLoader
from agent_c.util import SingletonCacheMeta, shared_cache_registry, CacheNames, to_snake_case
from agent_c.models.completion.agent_config import (
    AgentConfigurationV1,
    AgentConfigurationV2,
    AgentConfiguration,  # Union type
    CurrentAgentConfiguration,  # Latest version alias
    current_agent_configuration_version # integer for latest version
)
from agent_c.util.logging_utils import LoggingManager

# Type variable for configuration versions
T = TypeVar('T', bound=AgentConfiguration)

_singleton_instance = None

class AgentConfigLoader(ConfigLoader, metaclass=SingletonCacheMeta):
    """
    Loader that handles multiple versions of agent configurations.

    By default, it auto-migrates to the latest version, but this can be controlled.
    """

    def __init__(
            self,
            default_model: str = 'claude-sonnet-4-20250514',
            config_path: Optional[str] = None
    ):
        """
        Initialize the agent config loader.

        Args:
            default_model: Default model ID to use
            config_path: Path to configuration directory
        """
        super().__init__(config_path)
        global _singleton_instance
        if _singleton_instance is None:
            _singleton_instance = self

        logging_manager = LoggingManager(__name__)
        self.logger = logging_manager.get_logger()

        # Get model configs from singleton if not provided
        self.model_config_loader = ModelConfigurationLoader.instance()
        self.agent_config_folder = Path(self.config_path).joinpath("agents")
        self._agent_config_cache: Dict[str, AgentConfiguration] = {}
        self._default_model = default_model
        self._migration_log: Dict[str, Dict[str, Any]] = {}
        self.load_agents()

    @property
    def model_configs(self):
        """
        Get the model configurations.

        Returns:
            Dictionary of model configurations.
        """
        return self.model_config_loader.get_cached_config()

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

    def load_agents(self):
        """Load all agent configurations with caching."""
        # Use cached agent discovery
        cache_key = f"agent_files:{self.agent_config_folder}"
        agent_cache = shared_cache_registry.get_cache(CacheNames.AGENT_CONFIGS, max_size=200, ttl_seconds=1800)
        
        def discover_agent_files():
            return glob.glob(os.path.join(self.agent_config_folder, "**/*.yaml"), recursive=True)
        
        file_paths = agent_cache.get_or_compute(cache_key, discover_agent_files)
        
        for file_path in file_paths:
            self.load_agent_config_file(file_path)

    def _load_agent(self, agent_config_name: str) -> Optional[AgentConfiguration]:
        """Load an agent configuration from disk."""
        agent_config_path = os.path.join(self.agent_config_folder, f"{agent_config_name}.yaml")
        return self.load_agent_config_file(agent_config_path)

    def add_agent_config(self, agent_config: AgentConfiguration) -> None:
        """
        Add a new agent configuration to the loader.

        Args:
            agent_config: The AgentConfiguration object to add.
        """
        self._save_agent_config(agent_config)
        self._agent_config_cache[agent_config.key] = agent_config
        self._invalidate_agent_file_cache()

    def _save_agent_config(self, agent_config: AgentConfiguration) -> None:
        agent_config_path = os.path.join(self.agent_config_folder, f"{agent_config.key}.yaml")

        with open(agent_config_path, 'w', encoding='utf-8') as file:
            yaml_content = agent_config.to_yaml()
            file.write(yaml_content)

    def load_agent_config_file(self, agent_config_path) -> Optional[AgentConfiguration]:
        if os.path.exists(agent_config_path):
            try:
                with open(agent_config_path, 'r', encoding='utf-8') as file:
                    file_contents = file.read()

                data = yaml.safe_load(file_contents)
            except Exception as e:
                self.logger.exception(f"Failed to read agent configuration file {agent_config_path}: {e}", exc_info=True)
                return None
        else:
            raise FileNotFoundError(f"Agent configuration file {agent_config_path} not found.")

        # Handle missing version field (assume v1)
        if 'version' not in data:
            data['version'] = 1

        # Load appropriate version based on version field
        version = data.get('version', 1)
        if version > 2:
            self.logger.warning(f"Unsupported agent configuration version {version} in {agent_config_path}.")
            return None

        try:
            if version == 1:
                config = AgentConfigurationV1(**data)
            else:
                config = AgentConfigurationV2(**data)
        except Exception as e:
            self.logger.exception(f"Failed to load agent configuration from {agent_config_path}: {e}", exc_info=True)
            return None

        # Track original version
        original_version = self._get_version(config)


        config = self._migrate_config(config)

        # Log migration if it occurred
        final_version = self._get_version(config)
        if original_version != final_version:
            self._migration_log[config.name] = {
                'original_version': original_version,
                'final_version': final_version,
                'file_path': agent_config_path
            }

        self._agent_config_cache[config.key] = config
        return config



    def _get_version(self, config: AgentConfiguration) -> int:
        """Get version from a configuration object."""
        if isinstance(config, AgentConfigurationV1):
            return 1
        elif isinstance(config, AgentConfigurationV2):
            return 2
        else:
            # Fallback for future versions
            return getattr(config, 'version', 1)

    def _migrate_config(self, config: AgentConfiguration) -> AgentConfiguration:
        """Migrate configuration to target version or latest."""
        current_version = self._get_version(config)
        target = current_agent_configuration_version

        if current_version >= target:
            return config

        # Migrate v1 to v2
        if isinstance(config, AgentConfigurationV1):
            config = AgentConfigurationV2(version=2,  name=config.name, key=to_snake_case(config.name),
                                          model_id=config.model_id, agent_description=config.agent_description,
                                          tools=config.tools, agent_params=config.agent_params,
                                          prompt_metadata=config.prompt_metadata, persona=config.persona,
                                          uid=config.uid, category=["domo","outdated"],)

        return config

    def _fetch_agent_config(self, agent_name: str) -> AgentConfiguration:
        """Fetch agent configuration, using cache if available."""
        if agent_name in self._agent_config_cache:
            agent_config = self._agent_config_cache[agent_name]
        else:
            try:
                agent_config = self._load_agent(agent_name)
                if agent_config is None:
                    raise Exception(f"Agent {agent_name} configuration could not be loaded.")
                self._agent_config_cache[agent_name] = agent_config
            except FileNotFoundError:
                raise Exception(f"Agent {agent_name} not found.")

        return agent_config

    def duplicate(self, agent_key: str) -> AgentConfiguration:
        """
        Duplicate an existing agent configuration.

        Args:
            agent_key: The key of the agent to duplicate.

        Returns:
            A new AgentConfiguration object with a unique key.
        """
        original_config = self._fetch_agent_config(agent_key)
        if not original_config:
            raise ValueError(f"Agent {agent_key} does not exist.")

        return AgentConfigurationV2(**original_config.model_dump(exclude_none=True, exclude_defaults=True))

    @property
    def catalog(self) -> Dict[str, AgentConfiguration]:
        """Returns a catalog of all agent configurations."""
        if not self._agent_config_cache:
            for agent_name in self.agent_names:
                self._fetch_agent_config(agent_name)

        return self._agent_config_cache

    @property
    def agent_names(self) -> List[str]:
        return list(self._agent_config_cache.keys())

    def get_migration_report(self) -> Dict[str, Any]:
        """Get a report of all migrations performed."""
        return {
            'total_migrated': len(self._migration_log),
            'migrations': self._migration_log,
            'target_version': current_agent_configuration_version
        }

    def save_migrated_configs(self, backup_dir: Optional[str] = None) -> None:
        """
        Save all migrated configurations back to disk.

        Args:
            backup_dir: If provided, save original files here before overwriting
        """
        for agent_name, migration_info in self._migration_log.items():
            original_path = migration_info['file_path']

            # Create backup if requested
            if backup_dir:
                backup_path = Path(backup_dir) / Path(original_path).name
                backup_path.parent.mkdir(parents=True, exist_ok=True)
                with open(original_path, 'r') as f:
                    backup_path.write_text(f.read())

            # Get the migrated config
            config = self._agent_config_cache[agent_name]

            # Save it back to disk
            yaml_content = config.to_yaml()
            with open(original_path, 'w') as f:
                f.write(yaml_content)

    def get_typed_config(self, agent_name: str, version_type: type[T]) -> T:
        """
        Get a configuration with specific version type checking.

        Usage:
            v2_config = loader.get_typed_config("my_agent", AgentConfigurationV2)
        """
        config = self._fetch_agent_config(agent_name)

        if not isinstance(config, version_type):
            raise TypeError(
                f"Agent {agent_name} is version {self._get_version(config)}, "
                f"but {version_type.__name__} was requested"
            )

        return config

    def get_latest_version_configs(self) -> Dict[str, CurrentAgentConfiguration]:
        """Get all configurations as the latest version."""
        result = {}

        for agent_name in self.agent_names:
            config = self._fetch_agent_config(agent_name)
            if isinstance(config, CurrentAgentConfiguration):
                result[agent_name] = config
            else:
                # Force migration to latest
                migrated = self._migrate_config(config)
                if isinstance(migrated, CurrentAgentConfiguration):
                    result[agent_name] = migrated

        return result
    
    def _invalidate_agent_file_cache(self) -> None:
        """
        Invalidate the agent file discovery cache for this loader's directory.
        
        This should be called when agent files are added, removed, or modified
        to ensure the file discovery cache stays current.
        """
        cache_key = f"agent_files:{self.agent_config_folder}"
        agent_cache = shared_cache_registry.get_cache(CacheNames.AGENT_CONFIGS)
        agent_cache.invalidate(cache_key)
    
    @classmethod
    def clear_agent_caches(cls) -> Dict[str, int]:
        """
        Clear all agent configuration caches.
        
        Returns:
            Dictionary with cache clear results
        """
        return {
            'agent_cache_cleared': shared_cache_registry.clear_cache(CacheNames.AGENT_CONFIGS)
        }
    
    @classmethod
    def get_cache_stats(cls) -> Dict[str, Any]:
        """
        Get comprehensive cache statistics for AgentConfigLoader.
        
        Returns:
            Dictionary with cache statistics
        """
        agent_cache = shared_cache_registry.get_cache(CacheNames.AGENT_CONFIGS)
        
        return {
            'singleton_stats': super().get_cache_stats() if hasattr(super(), 'get_cache_stats') else {},
            'agent_config_stats': agent_cache.get_stats()
        }
    
    @classmethod
    def invalidate_cache(cls, config_path: Optional[str] = None, default_model: Optional[str] = None) -> Dict[str, Any]:
        """
        Invalidate AgentConfigLoader caches.
        
        Args:
            config_path: If provided, invalidate singleton instance for this path
            default_model: If provided, invalidate singleton instance for this model
            
        Returns:
            Dictionary with invalidation results
        """
        results = {
            'singleton_invalidated': False,
            'caches_cleared': {}
        }
        
        # Invalidate specific singleton instance if parameters provided
        if config_path is not None or default_model is not None:
            kwargs = {}
            if config_path is not None:
                kwargs['config_path'] = config_path
            if default_model is not None:
                kwargs['default_model'] = default_model
            results['singleton_invalidated'] = cls.invalidate_instance(**kwargs)
        
        # Clear agent configuration caches
        results['caches_cleared'] = cls.clear_agent_caches()
        
        return results


# Convenience functions for specific use cases
def load_all_agents_latest(config_path: str) -> Dict[str, CurrentAgentConfiguration]:
    """Load all agents and ensure they're migrated to the latest version."""
    loader = AgentConfigLoader(config_path=config_path)
    return loader.get_latest_version_configs()


def load_agents_preserve_versions(config_path: str) -> Dict[str, AgentConfiguration]:
    """Load all agents (always migrated to latest in simplified version)."""
    # Note: In simplified version, we always migrate to latest
    # This function is kept for backward compatibility but behaves the same as load_all_agents_latest
    loader = AgentConfigLoader(config_path=config_path)
    return loader.catalog


def migrate_all_agents_in_place(config_path: str, backup_dir: str) -> Dict[str, Any]:
    """
    Load all agents, migrate to latest version, and save back to disk.

    Returns migration report.
    """
    loader = AgentConfigLoader(config_path=config_path)

    # Load all configs (triggers migration)
    _ = loader.catalog

    # Save migrated configs
    loader.save_migrated_configs(backup_dir=backup_dir)

    return loader.get_migration_report()



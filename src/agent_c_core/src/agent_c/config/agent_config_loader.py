import os
import glob
import yaml
from pathlib import Path
from typing import Dict, Any, Optional, List, TypeVar

from agent_c.models import ModelConfigurationFile
from agent_c.config import ModelConfigurationLoader
from agent_c.config.config_loader import ConfigLoader
from agent_c.models.agent_config import (
    AgentConfigurationV1,
    AgentConfigurationV2,
    AgentConfiguration,  # Union type
    CurrentAgentConfiguration  # Latest version alias
)
from agent_c.util import MnemonicSlugs
from agent_c.util.logging_utils import LoggingManager

# Type variable for configuration versions
T = TypeVar('T', bound=AgentConfiguration)


class AgentConfigLoader(ConfigLoader):
    """
    Loader that handles multiple versions of agent configurations.

    By default, it auto-migrates to the latest version, but this can be controlled.
    """

    def __init__(
            self,
            default_model: str = 'claude-sonnet-4-20250514',
            model_configs: ModelConfigurationFile = None,
            config_path: Optional[str] = None,
            auto_migrate: bool = True,
            target_version: Optional[int] = None
    ):
        """
        Initialize the agent config loader.

        Args:
            default_model: Default model ID to use
            model_configs: Model configuration file
            config_path: Path to configuration directory
            auto_migrate: If True, automatically migrate configs to latest version
            target_version: If set, migrate to this specific version instead of latest
        """
        super().__init__(config_path)
        logging_manager = LoggingManager(__name__)
        self.logger = logging_manager.get_logger()

        if model_configs is None:
            model_configs = ModelConfigurationLoader(self.config_path).load_from_json()

        self.model_configs = model_configs
        self.agent_config_folder = Path(self.config_path).joinpath("agents")
        self._agent_config_cache: Dict[str, AgentConfiguration] = {}
        self._default_model = default_model
        self._auto_migrate = auto_migrate
        self._target_version = target_version

        # Track which configs have been migrated for logging/reporting
        self._migration_log: Dict[str, Dict[str, Any]] = {}
        self.load_agents()

    def load_agents(self):
        for file_path in glob.glob(os.path.join(self.agent_config_folder, "**/*.yaml"), recursive=True):
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
        self._agent_config_cache[agent_config.name] = agent_config

    def _save_agent_config(self, agent_config: AgentConfiguration) -> None:

        # Determine file path
        agent_config_path = os.path.join(self.agent_config_folder, f"{agent_config.key}.yaml")

        # Save YAML content
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

        # Add uid if missing
        if 'uid' not in data:
            data['uid'] =  MnemonicSlugs.generate_id_slug(3, file_contents)

        # Transform agent_params to match completion parameter models
        self._transform_agent_params(data)

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
            self.logger.error(f"Failed to load agent configuration from {agent_config_path}: {e}", exc_info=True)
            return None

        # Track original version
        original_version = self._get_version(config)

        # Migrate if requested
        if self._auto_migrate:
            config = self._migrate_config(config, config.name)

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

    def _transform_agent_params(self, data: dict) -> None:
        """Transform agent_params to match completion parameter model expectations."""
        if 'agent_params' not in data or data['agent_params'] is None:
            return
            
        agent_params = data['agent_params']
        model_id = data.get('model_id', '')
        
        # Add model_name from top-level model_id if not present
        if 'model_name' not in agent_params:
            agent_params['model_name'] = model_id
            
        # Determine and add type field if not present
        if 'type' not in agent_params:
            # Infer type based on model_id and available fields
            if 'claude' in model_id.lower():
                if 'budget_tokens' in agent_params or 'max_searches' in agent_params:
                    agent_params['type'] = 'claude_reasoning'
                else:
                    agent_params['type'] = 'claude_non_reasoning'
            elif 'gpt' in model_id.lower() or 'o1' in model_id.lower():
                if 'reasoning_effort' in agent_params:
                    agent_params['type'] = 'g_p_t_reasoning'
                else:
                    agent_params['type'] = 'g_p_t_non_reasoning'
            else:
                # Default fallback - assume Claude non-reasoning
                agent_params['type'] = 'claude_non_reasoning'

    def _get_version(self, config: AgentConfiguration) -> int:
        """Get version from a configuration object."""
        if isinstance(config, AgentConfigurationV1):
            return 1
        elif isinstance(config, AgentConfigurationV2):
            return 2
        else:
            # Fallback for future versions
            return getattr(config, 'version', 1)

    def _migrate_config(self, config: AgentConfiguration, agent_name: str) -> AgentConfiguration:
        """Migrate configuration to target version or latest."""
        current_version = self._get_version(config)
        target = self._target_version or 2  # Default to v2 as latest

        if current_version >= target:
            return config

        # Migrate v1 to v2
        if isinstance(config, AgentConfigurationV1) and target >= 2:
            return AgentConfigurationV2(
                version=2,
                name=config.name,
                model_id=config.model_id,
                agent_description=config.agent_description,
                tools=config.tools,
                agent_params=config.agent_params,
                prompt_metadata=config.prompt_metadata,
                persona=config.persona,
                uid=config.uid,
                category=["outdated"],
            )

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

        return AgentConfigurationV2(**original_config.model_dump(exclude_none=True))

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
            'auto_migrate': self._auto_migrate,
            'target_version': self._target_version
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
                migrated = self._migrate_config(config, agent_name)
                if isinstance(migrated, CurrentAgentConfiguration):
                    result[agent_name] = migrated

        return result


# Convenience functions for specific use cases
def load_all_agents_latest(config_path: str) -> Dict[str, CurrentAgentConfiguration]:
    """Load all agents and ensure they're migrated to the latest version."""
    loader = AgentConfigLoader(config_path=config_path, auto_migrate=True)
    return loader.get_latest_version_configs()


def load_agents_preserve_versions(config_path: str) -> Dict[str, AgentConfiguration]:
    """Load all agents without migrating versions."""
    loader = AgentConfigLoader(config_path=config_path, auto_migrate=False)
    return loader.catalog


def migrate_all_agents_in_place(config_path: str, backup_dir: str) -> Dict[str, Any]:
    """
    Load all agents, migrate to latest version, and save back to disk.

    Returns migration report.
    """
    loader = AgentConfigLoader(config_path=config_path, auto_migrate=True)

    # Load all configs (triggers migration)
    _ = loader.catalog

    # Save migrated configs
    loader.save_migrated_configs(backup_dir=backup_dir)

    return loader.get_migration_report()



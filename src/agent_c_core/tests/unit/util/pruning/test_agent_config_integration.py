"""
Tests for agent configuration integration with pruning.

This module tests that pruning configuration is properly loaded and integrated
into agent configurations.
"""

import pytest
import tempfile
import os
import yaml
import logging
from agent_c.config.agent_config_loader import AgentConfigLoader
from agent_c.models.agent_config import AgentConfigurationV3, CurrentAgentConfiguration
from agent_c.util.pruning.config import PrunerConfig


class TestAgentConfigIntegration:
    """Test suite for agent configuration integration with pruning."""

    def test_agent_config_v3_creation(self):
        """Test creating AgentConfigurationV3 with pruning settings."""
        pruner_config = PrunerConfig(
            recent_message_count=25,
            token_threshold_percent=0.8,
            correction_keywords=["actually", "wrong", "fix"]
        )
        
        config = AgentConfigurationV3(
            name="test_agent",
            model_id="claude-sonnet-4-20250514",
            persona="You are a helpful assistant",
            enable_auto_pruning=True,
            pruner_config=pruner_config
        )
        
        assert config.version == 3
        assert config.enable_auto_pruning is True
        assert config.pruner_config is not None
        assert config.pruner_config.recent_message_count == 25
        assert config.pruner_config.token_threshold_percent == 0.8

    def test_agent_config_v3_defaults(self):
        """Test AgentConfigurationV3 with default pruning settings."""
        config = AgentConfigurationV3(
            name="test_agent",
            model_id="claude-sonnet-4-20250514",
            persona="You are a helpful assistant"
        )
        
        assert config.version == 3
        assert config.enable_auto_pruning is False
        assert config.pruner_config is None

    def test_yaml_config_loading_with_pruning(self):
        """Test loading agent configuration from YAML with pruning settings."""
        # Test the YAML parsing and transformation directly
        import yaml
        from agent_c.config.agent_config_loader import AgentConfigLoader
        
        yaml_content = """
version: 3
name: "test_agent_with_pruning"
model_id: "claude-sonnet-4-20250514"
persona: "You are a helpful assistant with pruning enabled"
agent_description: "Test agent with pruning configuration"
tools: []
category: ["test"]
pruning:
  enable_auto_pruning: true
  recent_message_count: 25
  token_threshold_percent: 0.8
  correction_keywords: ["actually", "wrong", "fix", "correction"]
"""
        
        # Parse YAML and test transformation
        data = yaml.safe_load(yaml_content)
        
        # Create a minimal loader instance to test the transformation methods
        loader = AgentConfigLoader.__new__(AgentConfigLoader)
        loader.logger = logging.getLogger(__name__)
        
        # Test the pruning config transformation
        loader._transform_pruning_config(data)
        
        # Verify the transformation worked
        assert data['enable_auto_pruning'] is True
        assert data['pruner_config'] is not None
        assert data['pruner_config'].recent_message_count == 25
        assert data['pruner_config'].token_threshold_percent == 0.8
        assert "actually" in data['pruner_config'].correction_keywords
        
        # Test that we can create the config object
        config = AgentConfigurationV3(**data)
        assert isinstance(config, AgentConfigurationV3)
        assert config.name == "test_agent_with_pruning"
        assert config.enable_auto_pruning is True
        assert config.pruner_config is not None

    def test_yaml_config_loading_without_pruning(self):
        """Test loading agent configuration from YAML without pruning settings."""
        import yaml
        
        yaml_content = """
version: 3
name: "test_agent_no_pruning"
model_id: "claude-sonnet-4-20250514"
persona: "You are a helpful assistant without pruning"
agent_description: "Test agent without pruning configuration"
tools: []
category: ["test"]
"""
        
        # Parse YAML and test transformation
        data = yaml.safe_load(yaml_content)
        
        # Create a minimal loader instance to test the transformation methods
        loader = AgentConfigLoader.__new__(AgentConfigLoader)
        loader.logger = logging.getLogger(__name__)
        
        # Test the pruning config transformation (should do nothing)
        loader._transform_pruning_config(data)
        
        # Verify no pruning config was added
        assert 'enable_auto_pruning' not in data
        assert 'pruner_config' not in data
        
        # Test that we can create the config object with defaults
        config = AgentConfigurationV3(**data)
        assert isinstance(config, AgentConfigurationV3)
        assert config.name == "test_agent_no_pruning"
        assert config.enable_auto_pruning is False  # Default
        assert config.pruner_config is None  # Default

    def test_yaml_config_partial_pruning(self):
        """Test loading agent configuration with partial pruning settings."""
        import yaml
        
        yaml_content = """
version: 3
name: "test_agent_partial_pruning"
model_id: "claude-sonnet-4-20250514"
persona: "You are a helpful assistant"
pruning:
  enable_auto_pruning: true
  # Only enable auto-pruning, use default pruner config
"""
        
        # Parse YAML and test transformation
        data = yaml.safe_load(yaml_content)
        
        # Create a minimal loader instance to test the transformation methods
        loader = AgentConfigLoader.__new__(AgentConfigLoader)
        loader.logger = logging.getLogger(__name__)
        
        # Test the pruning config transformation
        loader._transform_pruning_config(data)
        
        # Verify only enable_auto_pruning was set
        assert data['enable_auto_pruning'] is True
        assert 'pruner_config' not in data  # No additional config provided
        
        # Test that we can create the config object
        config = AgentConfigurationV3(**data)
        assert isinstance(config, AgentConfigurationV3)
        assert config.enable_auto_pruning is True
        assert config.pruner_config is None  # Default

    def test_migration_v1_to_v3(self):
        """Test migration from V1 to V3 configuration."""
        from agent_c.models.agent_config import AgentConfigurationV1
        
        # Create a V1 config
        v1_config = AgentConfigurationV1(
            name="legacy_agent",
            model_id="claude-sonnet-4-20250514",
            persona="You are a helpful assistant",
            agent_description="Legacy agent configuration",
            tools=["search", "calculator"]
        )
        
        # Create a minimal loader instance to test migration
        loader = AgentConfigLoader.__new__(AgentConfigLoader)
        loader.logger = logging.getLogger(__name__)
        loader._target_version = 3
        
        # Test migration
        migrated_config = loader._migrate_config(v1_config, "legacy_agent")
        
        # Should be migrated to V3
        assert isinstance(migrated_config, AgentConfigurationV3)
        assert migrated_config.version == 3
        assert migrated_config.name == "legacy_agent"
        assert migrated_config.tools == ["search", "calculator"]
        
        # Pruning should be disabled by default
        assert migrated_config.enable_auto_pruning is False
        assert migrated_config.pruner_config is None

    def test_migration_v2_to_v3(self):
        """Test migration from V2 to V3 configuration."""
        from agent_c.models.agent_config import AgentConfigurationV2
        
        # Create a V2 config
        v2_config = AgentConfigurationV2(
            name="v2_agent",
            model_id="claude-sonnet-4-20250514",
            persona="You are a helpful assistant",
            agent_description="V2 agent configuration",
            tools=["search"],
            category=["general", "assistant"]
        )
        
        # Create a minimal loader instance to test migration
        loader = AgentConfigLoader.__new__(AgentConfigLoader)
        loader.logger = logging.getLogger(__name__)
        loader._target_version = 3
        
        # Test migration
        migrated_config = loader._migrate_config(v2_config, "v2_agent")
        
        # Should be migrated to V3
        assert isinstance(migrated_config, AgentConfigurationV3)
        assert migrated_config.version == 3
        assert migrated_config.name == "v2_agent"
        assert migrated_config.category == ["general", "assistant"]
        
        # Pruning should be disabled by default
        assert migrated_config.enable_auto_pruning is False
        assert migrated_config.pruner_config is None

    def test_invalid_pruning_config(self):
        """Test handling of invalid pruning configuration."""
        import yaml
        
        yaml_content = """
version: 3
name: "test_agent_invalid_pruning"
model_id: "claude-sonnet-4-20250514"
persona: "You are a helpful assistant"
pruning:
  enable_auto_pruning: true
  recent_message_count: 5  # Invalid - below minimum
  token_threshold_percent: 1.5  # Invalid - above maximum
"""
        
        # Parse YAML and test transformation
        data = yaml.safe_load(yaml_content)
        
        # Create a minimal loader instance to test the transformation methods
        loader = AgentConfigLoader.__new__(AgentConfigLoader)
        loader.logger = logging.getLogger(__name__)
        
        # Test the pruning config transformation with invalid data
        loader._transform_pruning_config(data)
        
        # Should have enable_auto_pruning but pruner_config should be None due to invalid values
        assert data['enable_auto_pruning'] is True
        assert data['pruner_config'] is None  # Invalid config should be None
        
        # Test that we can create the config object
        config = AgentConfigurationV3(**data)
        assert isinstance(config, AgentConfigurationV3)
        assert config.enable_auto_pruning is True
        assert config.pruner_config is None  # Invalid config should be None

    def test_current_agent_configuration_is_v3(self):
        """Test that CurrentAgentConfiguration points to V3."""
        assert CurrentAgentConfiguration == AgentConfigurationV3

    def test_backward_compatibility(self):
        """Test that agents without pruning config work unchanged."""
        # This simulates existing agent configs that don't have pruning
        config = AgentConfigurationV3(
            name="existing_agent",
            model_id="claude-sonnet-4-20250514",
            persona="You are a helpful assistant"
            # No pruning configuration provided
        )
        
        # Should work with defaults
        assert config.enable_auto_pruning is False
        assert config.pruner_config is None
        
        # Agent should function normally without pruning
        assert config.name == "existing_agent"
        assert config.model_id == "claude-sonnet-4-20250514"
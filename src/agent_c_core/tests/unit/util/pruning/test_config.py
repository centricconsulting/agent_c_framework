"""
Tests for pruning configuration.

This module tests the PrunerConfig class including validation,
default values, and utility methods.
"""

import pytest
from pydantic import ValidationError

from agent_c.util.pruning.config import PrunerConfig


class TestPrunerConfig:
    """Test cases for PrunerConfig class."""
    
    def test_default_configuration(self):
        """Test that default configuration is valid and has expected values."""
        config = PrunerConfig()
        
        assert config.recent_message_count == 20
        assert config.token_threshold_percent == 0.75
        assert config.correction_keywords == ["actually", "correction", "wrong", "mistake"]
        assert config.enable_dry_run is False
        assert config.max_context_tokens is None
        assert config.enable_tool_call_intelligence is False
        assert config.enable_semantic_analysis is False
        assert config.enable_summarization is False
    
    def test_custom_configuration(self):
        """Test creating configuration with custom values."""
        config = PrunerConfig(
            recent_message_count=30,
            token_threshold_percent=0.8,
            correction_keywords=["fix", "error", "oops"],
            enable_dry_run=True,
            max_context_tokens=8000
        )
        
        assert config.recent_message_count == 30
        assert config.token_threshold_percent == 0.8
        assert config.correction_keywords == ["fix", "error", "oops"]
        assert config.enable_dry_run is True
        assert config.max_context_tokens == 8000
    
    def test_recent_message_count_validation(self):
        """Test validation of recent_message_count field."""
        # Valid range
        config = PrunerConfig(recent_message_count=10)
        assert config.recent_message_count == 10
        
        config = PrunerConfig(recent_message_count=50)
        assert config.recent_message_count == 50
        
        # Invalid values
        with pytest.raises(ValidationError):
            PrunerConfig(recent_message_count=5)  # Too low
            
        with pytest.raises(ValidationError):
            PrunerConfig(recent_message_count=100)  # Too high
            
        with pytest.raises(ValidationError):
            PrunerConfig(recent_message_count=0)  # Zero
    
    def test_token_threshold_percent_validation(self):
        """Test validation of token_threshold_percent field."""
        # Valid range
        config = PrunerConfig(token_threshold_percent=0.5)
        assert config.token_threshold_percent == 0.5
        
        config = PrunerConfig(token_threshold_percent=0.95)
        assert config.token_threshold_percent == 0.95
        
        # Invalid values
        with pytest.raises(ValidationError):
            PrunerConfig(token_threshold_percent=0.3)  # Too low
            
        with pytest.raises(ValidationError):
            PrunerConfig(token_threshold_percent=1.1)  # Too high
    
    def test_correction_keywords_validation(self):
        """Test validation of correction_keywords field."""
        # Valid keywords
        config = PrunerConfig(correction_keywords=["fix", "error"])
        assert config.correction_keywords == ["fix", "error"]
        
        # Keywords are converted to lowercase and stripped
        config = PrunerConfig(correction_keywords=["  FIX  ", "ERROR"])
        assert config.correction_keywords == ["fix", "error"]
        
        # Invalid keywords
        with pytest.raises(ValidationError):
            PrunerConfig(correction_keywords=[])  # Empty list
            
        with pytest.raises(ValidationError):
            PrunerConfig(correction_keywords=[""])  # Empty string
            
        with pytest.raises(ValidationError):
            PrunerConfig(correction_keywords=["valid", ""])  # Mixed valid/invalid
    
    def test_max_context_tokens_validation(self):
        """Test validation of max_context_tokens field."""
        # Valid values
        config = PrunerConfig(max_context_tokens=None)
        assert config.max_context_tokens is None
        
        config = PrunerConfig(max_context_tokens=8000)
        assert config.max_context_tokens == 8000
        
        # Invalid values
        with pytest.raises(ValidationError):
            PrunerConfig(max_context_tokens=0)  # Zero
            
        with pytest.raises(ValidationError):
            PrunerConfig(max_context_tokens=-1000)  # Negative
    
    def test_get_effective_token_limit(self):
        """Test calculation of effective token limit."""
        config = PrunerConfig(token_threshold_percent=0.75)
        
        # Using model's context limit
        assert config.get_effective_token_limit(8000) == 6000
        assert config.get_effective_token_limit(32000) == 24000
        
        # Using override limit
        config = PrunerConfig(
            token_threshold_percent=0.8,
            max_context_tokens=10000
        )
        assert config.get_effective_token_limit(8000) == 8000  # Uses override, not model limit
    
    def test_should_preserve_message_content(self):
        """Test message content preservation logic."""
        config = PrunerConfig(correction_keywords=["actually", "wrong", "fix"])
        
        # Messages that should be preserved
        assert config.should_preserve_message_content("Actually, I meant something else")
        assert config.should_preserve_message_content("That's wrong, let me clarify")
        assert config.should_preserve_message_content("Please fix this issue")
        assert config.should_preserve_message_content("ACTUALLY, that's not right")  # Case insensitive
        
        # Messages that should not be preserved
        assert not config.should_preserve_message_content("This is a normal message")
        assert not config.should_preserve_message_content("Everything looks good")
        assert not config.should_preserve_message_content("")  # Empty content
        assert not config.should_preserve_message_content("factually speaking")  # Partial match
    
    def test_configuration_serialization(self):
        """Test that configuration can be serialized and deserialized."""
        original_config = PrunerConfig(
            recent_message_count=25,
            token_threshold_percent=0.8,
            correction_keywords=["fix", "error"],
            enable_dry_run=True
        )
        
        # Serialize to dict
        config_dict = original_config.model_dump()
        
        # Deserialize from dict
        restored_config = PrunerConfig(**config_dict)
        
        assert restored_config.recent_message_count == original_config.recent_message_count
        assert restored_config.token_threshold_percent == original_config.token_threshold_percent
        assert restored_config.correction_keywords == original_config.correction_keywords
        assert restored_config.enable_dry_run == original_config.enable_dry_run
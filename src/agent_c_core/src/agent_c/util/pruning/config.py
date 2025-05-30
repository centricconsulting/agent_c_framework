"""
Configuration models for chat log pruning.

This module defines the configuration structures for the chat log pruner,
including validation rules and default values.
"""

from typing import List, Optional
from pydantic import Field, field_validator

from agent_c.models.base import BaseModel


class PrunerConfig(BaseModel):
    """
    Configuration for the chat log pruner.
    
    This class defines all configurable parameters for the pruning system,
    including preservation rules, token thresholds, and behavioral settings.
    """
    
    # Core pruning parameters
    recent_message_count: int = Field(
        default=20,
        description="Number of recent messages to always preserve",
        ge=10,
        le=50
    )
    
    token_threshold_percent: float = Field(
        default=0.75,
        description="Percentage of context limit that triggers pruning (0.5-0.95)",
        ge=0.5,
        le=0.95
    )
    
    correction_keywords: List[str] = Field(
        default_factory=lambda: ["actually", "correction", "wrong", "mistake"],
        description="Keywords that identify user corrections to preserve"
    )
    
    # Operational settings
    enable_dry_run: bool = Field(
        default=False,
        description="When true, analyze what would be pruned without actually removing messages"
    )
    
    max_context_tokens: Optional[int] = Field(
        default=None,
        description="Override context limit (if None, uses model's default limit)",
        gt=0
    )
    
    # Advanced settings for future extensions
    enable_tool_call_intelligence: bool = Field(
        default=False,
        description="Enable advanced tool call relevance analysis (future feature)"
    )
    
    enable_semantic_analysis: bool = Field(
        default=False,
        description="Enable semantic similarity analysis for smarter pruning (future feature)"
    )
    
    enable_summarization: bool = Field(
        default=False,
        description="Enable conversation summarization instead of removal (future feature)"
    )
    
    @field_validator('correction_keywords')
    @classmethod
    def validate_correction_keywords(cls, v: List[str]) -> List[str]:
        """Validate correction keywords are non-empty strings."""
        if not v:
            raise ValueError("correction_keywords cannot be empty")
        
        for keyword in v:
            if not isinstance(keyword, str) or not keyword.strip():
                raise ValueError("All correction keywords must be non-empty strings")
        
        # Convert to lowercase for case-insensitive matching
        return [keyword.lower().strip() for keyword in v]
    
    @field_validator('recent_message_count')
    @classmethod
    def validate_recent_message_count(cls, v: int) -> int:
        """Validate recent message count is reasonable."""
        if v < 1:
            raise ValueError("recent_message_count must be at least 1")
        return v
    
    def get_effective_token_limit(self, model_context_limit: int) -> int:
        """
        Calculate the effective token limit for pruning decisions.
        
        Args:
            model_context_limit: The model's maximum context window size
            
        Returns:
            The token count that should trigger pruning
        """
        base_limit = self.max_context_tokens or model_context_limit
        return int(base_limit * self.token_threshold_percent)
    
    def should_preserve_message_content(self, content: str) -> bool:
        """
        Check if a message should be preserved based on its content.
        
        Args:
            content: The message content to analyze
            
        Returns:
            True if the message contains correction keywords as complete words
        """
        if not content:
            return False
            
        import re
        content_lower = content.lower()
        
        # Use word boundaries to match complete words only
        for keyword in self.correction_keywords:
            pattern = r'\b' + re.escape(keyword) + r'\b'
            if re.search(pattern, content_lower):
                return True
        return False
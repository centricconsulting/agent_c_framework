"""
Parameter models for AI model configurations.

This module defines the various types of parameters that can be configured
for AI models, including range parameters, enum parameters, and complex
nested parameter structures.
"""

from enum import Enum
from typing import List, Optional, Dict, Any, Union, Literal
from pydantic import BaseModel, Field, ConfigDict, model_validator


class BaseParameter(BaseModel):
    """Base class for all model parameters."""
    required: bool = Field(description="Whether this parameter is required")
    
    model_config = ConfigDict(
        extra="forbid",
        use_enum_values=True
    )


class RangeParameter(BaseParameter):
    """
    Parameter with minimum, maximum, and default values.
    
    Used for parameters like temperature and max_tokens that have
    numeric constraints.
    """
    default: Union[int, float] = Field(description="Default value for the parameter")
    min: Union[int, float] = Field(description="Minimum allowed value")
    max: Union[int, float] = Field(description="Maximum allowed value")
    
    @model_validator(mode='after')
    def validate_default_in_range(self):
        """Ensure default value is within min/max range."""
        if not (self.min <= self.default <= self.max):
            raise ValueError(f"Default value {self.default} must be between {self.min} and {self.max}")
        return self


class EnumParameter(BaseParameter):
    """
    Parameter with a fixed set of allowed values.
    
    Used for parameters like reasoning_effort that have
    predefined options.
    """
    default: str = Field(description="Default value for the parameter")
    values: List[str] = Field(description="List of allowed values")
    
    @model_validator(mode='after')
    def validate_default_in_values(self):
        """Ensure default value is in the allowed values list."""
        if self.default not in self.values:
            raise ValueError(f"Default value '{self.default}' must be one of {self.values}")
        return self


class BudgetTokensParameter(BaseModel):
    """Budget tokens configuration for extended thinking."""
    min: int = Field(description="Minimum budget tokens")
    max: int = Field(description="Maximum budget tokens") 
    default: int = Field(description="Default budget tokens")
    
    @model_validator(mode='after')
    def validate_default_in_range(self):
        """Ensure default value is within min/max range."""
        if not (self.min <= self.default <= self.max):
            raise ValueError(f"Default budget tokens {self.default} must be between {self.min} and {self.max}")
        return self


class ExtendedThinkingParameter(BaseParameter):
    """
    Complex parameter for extended thinking configuration.
    
    Used by Claude models that support extended thinking with
    budget token allocation.
    """
    enabled: bool = Field(description="Whether extended thinking is enabled")
    budget_tokens: BudgetTokensParameter = Field(description="Budget token configuration")


class MaxTokensLimits(BaseModel):
    """Limits configuration for max tokens parameter."""
    min: int = Field(description="Minimum tokens")
    max: Union[int, Dict[str, int]] = Field(description="Maximum tokens (can be conditional)")


class ConditionalMaxTokensParameter(BaseParameter):
    """
    Max tokens parameter with conditional limits.
    
    Used by models where the maximum token limit depends on
    whether other features (like extended thinking) are enabled.
    """
    default: int = Field(description="Default max tokens value")
    limits: MaxTokensLimits = Field(description="Token limits configuration")
    
    @model_validator(mode='after')
    def validate_default_against_limits(self):
        """Ensure default value respects the minimum limit."""
        if self.default < self.limits.min:
            raise ValueError(f"Default max tokens {self.default} must be at least {self.limits.min}")
        return self


# Union type for all parameter types
ModelParameter = Union[
    RangeParameter,
    EnumParameter, 
    ExtendedThinkingParameter,
    ConditionalMaxTokensParameter
]
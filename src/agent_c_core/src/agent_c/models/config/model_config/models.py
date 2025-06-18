"""
Model configuration models for AI models.

This module defines the core model configuration structures including
model types, capabilities, and the main model configuration class.
"""

from enum import Enum
from typing import Dict, Any, Optional
from pydantic import Field, ConfigDict

from agent_c.models.base import BaseModel
from agent_c.models.completion import CompletionParams


class ModelType(str, Enum):
    """Enumeration of supported model types."""
    CHAT = "chat"
    REASONING = "reasoning"


class ModelCapabilities(BaseModel):
    """Capabilities supported by an AI model."""
    supports_tools: bool = Field(True, description="Whether the model supports tool/function calling")
    multi_modal: bool = Field(True, description="Whether the model supports multiple input modalities")



class AllowedInputs(BaseModel):
    """Input modalities allowed by an AI model."""
    image: bool = Field(True, description="Whether the model accepts image inputs")
    video: bool = Field(False, description="Whether the model accepts video inputs")
    audio: bool = Field(False, description="Whether the model accepts audio inputs")
    file: bool = Field(False, description="Whether the model accepts file inputs")

class ModelConfiguration(BaseModel):
    """
    Complete configuration for an AI model.
    
    This represents a single model's configuration including its
    parameters, capabilities, and constraints.
    """
    id: str = Field(description="Unique identifier for the model")
    model_name: Optional[str] = Field(description="Name of the model as used in API calls, if different from ID")
    default_completion_params: CompletionParams = Field(description="Default parameters for model completions")

    model_type: ModelType = Field(description="Type of model (chat, reasoning, etc.)")

    ui_name: str = Field(description="Human-readable name for display in UI")
    description: str = Field(description="Detailed description of the model's capabilities")
    parameters: Dict[str, Any] = Field(
        description="Model-specific parameters and their configurations",
        default_factory=dict
    )
    context_window: int = Field(description="Maximum context window size in tokens")
    capabilities: ModelCapabilities = Field(description="Capabilities supported by the model")
    allowed_inputs: AllowedInputs = Field(description="Input modalities supported by the model")


    def __init__(self, **data: Any) -> None:
        """
        Initialize the model configuration with parameters.

        Args:
            **data: Additional data to initialize the model configuration
        """
        super().__init__(**data)

    
    def get_parameter_default(self, parameter_name: str) -> Any:
        """
        Get the default value for a specific parameter.
        
        Args:
            parameter_name: Name of the parameter
            
        Returns:
            Default value for the parameter, or None if not found
        """
        if parameter_name not in self.parameters:
            return None
            
        param = self.parameters[parameter_name]
        return param.get('default')
    
    def get_parameter_constraints(self, parameter_name: str) -> Dict[str, Any]:
        """
        Get the constraints for a specific parameter.
        
        Args:
            parameter_name: Name of the parameter
            
        Returns:
            Dictionary of constraints (min, max, values, etc.)
        """
        if parameter_name not in self.parameters:
            return {}
            
        param = self.parameters[parameter_name]
        constraints = {}
        
        # Extract common constraint fields
        for field in ['min', 'max', 'values', 'limits']:
            if field in param:
                constraints[field] = param[field]
                
        return constraints
    
    def is_parameter_required(self, parameter_name: str) -> bool:
        """
        Check if a parameter is required.
        
        Args:
            parameter_name: Name of the parameter
            
        Returns:
            True if the parameter is required, False otherwise
        """
        if parameter_name not in self.parameters:
            return False
            
        param = self.parameters[parameter_name]
        return param.get('required', False)
    
    def supports_input_type(self, input_type: str) -> bool:
        """
        Check if the model supports a specific input type.
        
        Args:
            input_type: Type of input ('image', 'video', 'audio', 'file')
            
        Returns:
            True if the input type is supported, False otherwise
        """
        return getattr(self.allowed_inputs, input_type, False)

class ModelConfigurationWithVendor(ModelConfiguration):
    """
    Model configuration that includes vendor information.

    This extends the base model configuration to include the vendor
    providing the model.
    """
    vendor: str = Field(..., description="Vendor providing the model")
    backend: Optional[str] = Field(None, description="Backend service or API for the model")

    def __init__(self, **data: Any) -> None:
        """
        Initialize the model configuration with vendor information.

        Args:
            **data: Additional data to initialize the model configuration
        """
        super().__init__(**data)

        if self.backend is None:
            self.backend = self.vendor


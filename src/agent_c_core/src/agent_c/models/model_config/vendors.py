"""
Vendor configuration models for AI model configurations.

This module defines the vendor-level configuration structures and
the root configuration model that contains all vendors and models.
"""

from typing import List, Optional, Dict
from pydantic import  Field, ConfigDict
from agent_c.models.base import BaseModel
from .models import ModelConfiguration


class VendorConfiguration(BaseModel):
    """
    Configuration for a specific AI model vendor.
    
    Contains all models available from a particular vendor
    like OpenAI, Anthropic, etc.
    """
    vendor: str = Field(description="Name of the vendor (e.g., 'openai', 'claude')")
    models: List[ModelConfiguration] = Field(
        description="List of models available from this vendor",
        default_factory=list
    )
    
    model_config = ConfigDict(
        extra="forbid",
        use_enum_values=True
    )
    
    def get_model_by_id(self, model_id: str) -> Optional[ModelConfiguration]:
        """
        Get a specific model by its ID.
        
        Args:
            model_id: The unique identifier of the model
            
        Returns:
            ModelConfiguration if found, None otherwise
        """
        for model in self.models:
            if model.id == model_id:
                return model
        return None
    
    def get_models_by_type(self, model_type: str) -> List[ModelConfiguration]:
        """
        Get all models of a specific type.
        
        Args:
            model_type: The type of models to retrieve ('chat', 'reasoning', etc.)
            
        Returns:
            List of models matching the specified type
        """
        return [model for model in self.models if model.model_type == model_type]
    
    def get_models_with_capability(self, capability: str) -> List[ModelConfiguration]:
        """
        Get all models that support a specific capability.
        
        Args:
            capability: The capability to check for ('supports_tools', 'multi_modal')
            
        Returns:
            List of models that support the specified capability
        """
        result = []
        for model in self.models:
            if hasattr(model.capabilities, capability):
                if getattr(model.capabilities, capability):
                    result.append(model)
        return result
    
    def get_models_with_input_support(self, input_type: str) -> List[ModelConfiguration]:
        """
        Get all models that support a specific input type.
        
        Args:
            input_type: The input type to check for ('image', 'video', 'audio', 'file')
            
        Returns:
            List of models that support the specified input type
        """
        result = []
        for model in self.models:
            if model.supports_input_type(input_type):
                result.append(model)
        return result


class ModelConfigurationFile(BaseModel):
    """
    Model configuration file containing all vendor configurations.
    
    This is the top-level model that represents the entire
    model configuration structure from the JSON file.
    """
    vendors: List[VendorConfiguration] = Field(
        description="List of vendor configurations",
        default_factory=list
    )
    
    model_config = ConfigDict(
        extra="forbid",
        use_enum_values=True
    )
    
    def get_vendor_by_name(self, vendor_name: str) -> Optional[VendorConfiguration]:
        """
        Get a specific vendor by name.
        
        Args:
            vendor_name: The name of the vendor
            
        Returns:
            VendorConfiguration if found, None otherwise
        """
        for vendor in self.vendors:
            if vendor.vendor == vendor_name:
                return vendor
        return None
    
    def get_model_by_id(self, model_id: str) -> Optional[ModelConfiguration]:
        """
        Get a specific model by ID across all vendors.
        
        Args:
            model_id: The unique identifier of the model
            
        Returns:
            ModelConfiguration if found, None otherwise
        """
        for vendor in self.vendors:
            model = vendor.get_model_by_id(model_id)
            if model:
                return model
        return None
    
    def get_all_models(self) -> List[ModelConfiguration]:
        """
        Get all models from all vendors.
        
        Returns:
            List of all available models
        """
        all_models = []
        for vendor in self.vendors:
            all_models.extend(vendor.models)
        return all_models
    
    def get_models_by_type(self, model_type: str) -> List[ModelConfiguration]:
        """
        Get all models of a specific type across all vendors.
        
        Args:
            model_type: The type of models to retrieve
            
        Returns:
            List of models matching the specified type
        """
        result = []
        for vendor in self.vendors:
            result.extend(vendor.get_models_by_type(model_type))
        return result
    
    def get_models_with_capability(self, capability: str) -> List[ModelConfiguration]:
        """
        Get all models that support a specific capability across all vendors.
        
        Args:
            capability: The capability to check for
            
        Returns:
            List of models that support the specified capability
        """
        result = []
        for vendor in self.vendors:
            result.extend(vendor.get_models_with_capability(capability))
        return result
    
    def get_vendor_names(self) -> List[str]:
        """
        Get a list of all vendor names.
        
        Returns:
            List of vendor names
        """
        return [vendor.vendor for vendor in self.vendors]
    
    def get_model_summary(self) -> Dict[str, int]:
        """
        Get a summary of models by vendor.
        
        Returns:
            Dictionary mapping vendor names to model counts
        """
        return {vendor.vendor: len(vendor.models) for vendor in self.vendors}
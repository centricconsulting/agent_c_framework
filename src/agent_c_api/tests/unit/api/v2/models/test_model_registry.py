"""Tests for the model registry integrity.

These tests verify that the model registry is properly maintained and that
there are no duplicated model classes.
"""

import pytest
from pydantic import BaseModel
from typing import Dict, List, Type
from enum import Enum

from agent_c_api.api.v2.models.registry import (
    MODEL_REGISTRY,
    ENUM_REGISTRY,
    DOMAIN_MODELS,
    get_model_by_name,
    get_enum_by_name,
    get_type_by_name,
    list_models_by_domain,
    verify_no_duplicate_models,
    get_duplicate_model_names
)


@pytest.mark.unit
@pytest.mark.models
class TestModelRegistry:
    """Tests for model registry integrity."""
    
    def test_registry_not_empty(self):
        """Test that the registry contains models."""
        assert len(MODEL_REGISTRY) > 0, "Model registry is empty"
    
    def test_all_models_are_pydantic(self):
        """Test that all registered models are Pydantic models."""
        for model_name, model_class in MODEL_REGISTRY.items():
            assert issubclass(model_class, BaseModel), f"{model_name} is not a Pydantic model"
    
    def test_all_enums_are_enums(self):
        """Test that all registered enums are Enum classes."""
        for enum_name, enum_class in ENUM_REGISTRY.items():
            assert issubclass(enum_class, Enum), f"{enum_name} is not an Enum"
    
    def test_no_duplicate_models(self):
        """Test that there are no duplicate model classes."""
        # Check for duplicate models
        model_names = list(MODEL_REGISTRY.keys())
        assert len(model_names) == len(set(model_names)), "Duplicate model names found"
        
        # Check for duplicate model classes
        model_classes = list(MODEL_REGISTRY.values())
        assert len(model_classes) == len(set(model_classes)), "Duplicate model classes found"
        
        # Use the helper function as well
        assert verify_no_duplicate_models(), "Duplicate model classes found via helper function"
        assert len(get_duplicate_model_names()) == 0, "Unexpected duplicate model names found"
    
    def test_all_domains_have_models(self):
        """Test that all domains have at least one model."""
        for domain, models in DOMAIN_MODELS.items():
            assert len(models) > 0, f"Domain '{domain}' has no models"
    
    def test_get_model_by_name(self):
        """Test getting a model by name."""
        # Test getting an existing model
        model = get_model_by_name("AgentConfig")
        assert model is not None, "Failed to get existing model"
        assert issubclass(model, BaseModel), "Retrieved model is not a Pydantic model"
        
        # Test getting a non-existent model
        model = get_model_by_name("NonExistentModel")
        assert model is None, "Got an unexpected model for a non-existent name"
        
        # Test that enums are not returned
        model = get_model_by_name("ChatEventType")
        assert model is None, "Got an enum when requesting a model"
    
    def test_get_enum_by_name(self):
        """Test getting an enum by name."""
        # Test getting an existing enum
        enum = get_enum_by_name("ChatEventType")
        assert enum is not None, "Failed to get existing enum"
        assert issubclass(enum, Enum), "Retrieved enum is not an Enum"
        
        # Test getting a non-existent enum
        enum = get_enum_by_name("NonExistentEnum")
        assert enum is None, "Got an unexpected enum for a non-existent name"
        
        # Test that models are not returned
        enum = get_enum_by_name("AgentConfig")
        assert enum is None, "Got a model when requesting an enum"
    
    def test_get_type_by_name(self):
        """Test getting any type by name."""
        # Test getting a model
        type_class = get_type_by_name("AgentConfig")
        assert type_class is not None, "Failed to get existing model"
        assert issubclass(type_class, BaseModel), "Retrieved type is not a Pydantic model"
        
        # Test getting an enum
        type_class = get_type_by_name("ChatEventType")
        assert type_class is not None, "Failed to get existing enum"
        assert issubclass(type_class, Enum), "Retrieved type is not an Enum"
        
        # Test getting a non-existent type
        type_class = get_type_by_name("NonExistentType")
        assert type_class is None, "Got an unexpected type for a non-existent name"
    
    def test_list_models_by_domain(self):
        """Test listing models by domain."""
        # Test getting models for an existing domain
        models = list_models_by_domain("session")
        assert len(models) > 0, "Failed to get models for existing domain"
        for model in models:
            assert issubclass(model, BaseModel), "Retrieved model is not a Pydantic model"
        
        # Test getting models for a non-existent domain
        models = list_models_by_domain("non_existent")
        assert len(models) == 0, "Got unexpected models for a non-existent domain"
    
    def test_models_in_registry_match_imports(self):
        """Test that all models in the registry match their imported versions."""
        # Import a few models directly
        from agent_c_api.api.v2.models.session_models import AgentConfig, SessionCreate
        from agent_c_api.api.v2.models.agent_models import ModelInfo
        from agent_c_api.api.v2.models.chat_models import ChatMessage
        
        # Verify they match the registry versions
        assert get_model_by_name("AgentConfig") is AgentConfig, "Registry AgentConfig doesn't match import"
        assert get_model_by_name("SessionCreate") is SessionCreate, "Registry SessionCreate doesn't match import"
        assert get_model_by_name("ModelInfo") is ModelInfo, "Registry ModelInfo doesn't match import"
        assert get_model_by_name("ChatMessage") is ChatMessage, "Registry ChatMessage doesn't match import"
    
    def test_all_registry_models_imported_in_init(self):
        """Test that all models in registry are imported in __init__.py."""
        # Import all models from the top-level models package
        import agent_c_api.api.v2.models as v2_models
        
        # Get all attributes from the models module
        model_attrs = dir(v2_models)
        
        # Check if each model in the registry is in the attributes
        for model_name in MODEL_REGISTRY.keys():
            assert model_name in model_attrs, f"{model_name} not imported in __init__.py"
        
        # Check if each enum in the registry is in the attributes
        for enum_name in ENUM_REGISTRY.keys():
            assert enum_name in model_attrs, f"{enum_name} not imported in __init__.py"
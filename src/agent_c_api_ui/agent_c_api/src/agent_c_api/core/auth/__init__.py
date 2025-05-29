"""Authentication package for Agent C API.

This package provides authentication functionality for the Agent C API,
including abstract interfaces and concrete implementations for various
identity providers.
"""

# Import key classes and functions for convenience
from agent_c_api.core.auth.auth_service import (
    get_auth_service,
    get_current_user,
    configure_auth,
)

# Make the provider registry available
from agent_c_api.core.auth.providers.registry import auth_provider_registry

__all__ = [
    'get_auth_service',
    'get_current_user',
    'configure_auth',
    'auth_provider_registry',
]
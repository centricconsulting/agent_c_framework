"""Authentication providers for Agent C API.

This package contains authentication provider implementations for
various identity providers.
"""

# Import base classes
from agent_c_api.core.auth.providers.base import (
    AuthProvider,
    TokenVerifier,
)

# Import provider implementations
from agent_c_api.core.auth.providers.microsoft import MicrosoftAuthProvider
from agent_c_api.core.auth.providers.google import GoogleAuthProvider

__all__ = [
    'AuthProvider',
    'TokenVerifier',
    'MicrosoftAuthProvider',
    'GoogleAuthProvider',
]
"""Authentication module for Agent C API"""

# Re-export important components for easier imports
from .middleware import get_current_user, optional_user
from .token_verifier import TokenVerifier
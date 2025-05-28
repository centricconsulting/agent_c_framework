"""Authentication configuration for Agent C API"""
import os
from dotenv import load_dotenv

# Load environment variables
load_dotenv(override=True)

# Validate required environment variables
client_id = os.getenv("AZURE_CLIENT_ID")
tenant_id = os.getenv("AZURE_TENANT_ID")

if not client_id or not tenant_id:
    raise ValueError("AZURE_CLIENT_ID and AZURE_TENANT_ID must be set in the .env file")

# Azure AD configuration for Microsoft SSO
AUTH_CONFIG = {
    "client_id": client_id,
    "tenant_id": tenant_id,
    "authority": f"https://login.microsoftonline.com/{tenant_id}",
    "redirect_uri": os.getenv("REDIRECT_URI", "http://localhost:3000"),
    
    # Authorization settings
    "required_scopes": ["openid", "profile", "User.Read"],  # Required scopes for the application
    
    # Authentication options
    "token_validation": {
        "verify_audience": True,  # Verify that the token audience matches the client ID
        "verify_issuer": True,  # Verify that the token issuer is from the expected authority
    }
}

# Protected routes configuration
PROTECTED_ROUTES = {
    # Routes that require authentication
    "require_auth": [
        # Chat and session operations (v1)
        "/api/v1/chat",
        "/api/v1/cancel",
        "/api/v1/initialize",
        "/api/v1/update_settings",
        "/api/v1/update_tools",
        "/api/v1/get_agent_config",
        "/api/v1/get_agent_tools",
        "/api/v1/verify_session",
        "/api/v1/sessions",

        # File operations (v1)
        "/api/v1/upload_file",
        "/api/v1/files",

        # Debug operations (v1)
        "/api/v1/debug_agent_state",
        "/api/v1/chat_session_debug",

        # Session operations (v2)
        "/api/v2/sessions",
        "/api/v2/sessions/*",

        # Config operations that require auth (v2)
        "/api/v2/system",
    ],

    # Routes that are exempt from authentication
    "public": [
        # Authentication endpoints (v1)
        "/api/v1/auth/validate",
        "/api/v1/auth/me",

        # Public information endpoints (v1)
        "/api/v1/models",
        "/api/v1/tools",
        "/api/v1/personas",

        # Public information endpoints (v2)
        "/api/v2/models",
        "/api/v2/models/*",
        "/api/v2/personas",
        "/api/v2/personas/*",
        "/api/v2/tools",
        "/api/v2/tools/*",
    ]
}
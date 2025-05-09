import os
import re

from typing import Dict, Any
from fastapi import FastAPI, APIRouter
from contextlib import asynccontextmanager
from starlette.middleware.cors import CORSMiddleware
from fastapi_cache import FastAPICache
from fastapi_cache.backends.inmemory import InMemoryBackend


from agent_c_api.config.env_config import settings
from agent_c_api.core.util.logging_utils import LoggingManager
from agent_c_api.core.agent_manager import UItoAgentBridgeManager
from agent_c_api.core.util.middleware_logging import APILoggingMiddleware

logging_manager = LoggingManager(__name__)
logger = logging_manager.get_logger()

def get_origins_regex():
    allowed_hosts_str = os.getenv("API_ALLOWED_HOSTS", "localhost,.local")
    patterns = [pattern.strip() for pattern in allowed_hosts_str.split(",")]
    # 1: ^https?:\/\/(localhost|.*\.local)(:\d+)?
    # 2: ^https?:\/\/(localhost|.*\.local)(:\d+)?
    regex_parts = []
    for pattern in patterns:
        if pattern.startswith("."):
            # Domain suffix like .local or .company.com
            suffix = re.escape(pattern)
            regex_parts.append(f".*{suffix}")
        else:
            # Specific host like localhost (with optional port)
            host = re.escape(pattern)
            regex_parts.append(f"{host}")

    return f"^https?://({"|".join(regex_parts)})(:\\d+)?"

def create_application(router: APIRouter, **kwargs) -> FastAPI:
    """
    Create and configure the FastAPI application.

    This function sets up the application metadata, initializes the shared
    AgentManager resource via a lifespan handler, adds any required middleware,
    and includes the given router.
    """

    # Define a lifespan handler for startup and shutdown tasks.
    @asynccontextmanager
    async def lifespan(lifespan_app: FastAPI):
        # Import Redis configuration at runtime to avoid circular imports
        from agent_c_api.config.redis_config import RedisConfig
        
        # Start Redis if configured to manage lifecycle
        if settings.MANAGE_REDIS_LIFECYCLE:
            logger.info("Starting Redis server (managed by application)")
            redis_started = await RedisConfig.start_redis_if_needed()
            if not redis_started:
                logger.error("Failed to start Redis server, application may not function correctly")
                
        # Check if Redis is available, regardless of whether we're managing it
        redis_available = await RedisConfig.ping_redis()
        if not redis_available:
            logger.warning("Redis server is not available, some features may not work properly")
        else:
            logger.info("Successfully connected to Redis server")
        
        # Shared AgentManager instance.
        lifespan_app.state.agent_manager = UItoAgentBridgeManager()
        
        # Initialize FastAPICache with InMemoryBackend
        FastAPICache.init(InMemoryBackend(), prefix="agent_c_api_cache")
        logger.info("FastAPICache initialized with InMemoryBackend")

        yield

        # Shutdown: Stop Redis if we started it
        if settings.MANAGE_REDIS_LIFECYCLE:
            logger.info("Stopping Redis server")
            await RedisConfig.stop_redis_if_needed()


    # Set up comprehensive OpenAPI metadata from settings (or fallback defaults)
    app_version = getattr(settings, "APP_VERSION", "0.2.0")

    openapi_metadata = {
        "title": getattr(settings, "APP_NAME", "Agent C API"),
        "description": getattr(settings, "APP_DESCRIPTION", "RESTful API for interacting with Agent C. The API provides resources for session management, chat interactions, file handling, and history access."),
        "version": app_version,
        "openapi_tags": [
            {
                "name": "config",
                "description": "Configuration resources for models, personas, tools, and system settings"
            },
            {
                "name": "sessions",
                "description": "Session management resources for creating, configuring, and interacting with agent sessions"
            },
            {
                "name": "history",
                "description": "History resources for accessing past interactions and replaying sessions"
            },
            {
                "name": "debug",
                "description": "Debug resources for accessing detailed information about session and agent state"
            }
        ],
        "contact": {
            "name": getattr(settings, "CONTACT_NAME", "Agent C Team"),
            "email": getattr(settings, "CONTACT_EMAIL", "joseph.ours@centricconsulting.com"),
            "url": getattr(settings, "CONTACT_URL", "https://www.centricconsulting.com")
        },
        "license_info": {
            "name": getattr(settings, "LICENSE_NAME", "Business Source License 1.1"),
            "url": getattr(settings, "LICENSE_URL", "https://raw.githubusercontent.com/centricconsulting/agent_c_framework/refs/heads/main/LICENSE")
        },
        "terms_of_service": getattr(settings, "TERMS_URL", "https://www.centricconsulting.com/terms"),
        "docs_url": getattr(settings, "DOCS_URL", "/docs"),
        "redoc_url": getattr(settings, "REDOC_URL", "/redoc"),
        "openapi_url": getattr(settings, "OPENAPI_URL", "/openapi.json")
    }
    

    kwargs.update(openapi_metadata)
    app = FastAPI(lifespan=lifespan, **kwargs)

    origin_regex = get_origins_regex()
    logger.info(f"CORS allowed host regex: {origin_regex}")
    app.add_middleware(
        CORSMiddleware,
        allow_origin_regex=origin_regex,
        allow_credentials=True,
        allow_methods=["*"],
        allow_headers=["*"],
    )


    environment = getattr(settings, "ENV", os.getenv("ENV", "development")).lower()
    is_production = environment == "production"
    logger.info(f"Application running in {environment} environment")

    # Add our custom logging middleware
    # Enable request body logging in development but not in production
    app.add_middleware(
        APILoggingMiddleware,
        log_request_body=not is_production,
        log_response_body=False
    )

    app.include_router(router)

    # Log application creation details
    app_name = getattr(settings, 'APP_NAME', 'Agent C API')
    app_version = getattr(settings, 'APP_VERSION', '2.0.0')
    docs_url = getattr(settings, 'DOCS_URL', '/docs')
    redoc_url = getattr(settings, 'REDOC_URL', '/redoc')
    openapi_url = getattr(settings, 'OPENAPI_URL', '/openapi.json')
    
    logger.info(f"Application created: {app_name} v{app_version}")
    logger.info(f"API documentation available at:\n  - Swagger UI: {docs_url}\n  - ReDoc: {redoc_url}\n  - OpenAPI Schema: {openapi_url}")

    # Add a utility method to the app for accessing the OpenAPI schema
    app.openapi_schema_version = app_version
    
    # Override the default openapi method to add custom components if needed
    original_openapi = app.openapi
    
    def custom_openapi() -> Dict[str, Any]:
        if app.openapi_schema:
            return app.openapi_schema
            
        openapi_schema = original_openapi()
        
        # Add custom security schemes if needed
        # This is where you would add JWT security definitions, OAuth flows, etc.
        
        app.openapi_schema = openapi_schema
        return app.openapi_schema
    
    app.openapi = custom_openapi

    return app
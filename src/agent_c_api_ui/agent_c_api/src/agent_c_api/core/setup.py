import os
import re

from typing import Dict, Any
from fastapi import FastAPI, APIRouter
from contextlib import asynccontextmanager
from starlette.middleware.cors import CORSMiddleware
from fastapi_cache import FastAPICache
from fastapi_cache.backends.inmemory import InMemoryBackend


from agent_c_api.config.env_config import settings
from agent_c.util.structured_logging import get_logger, LoggingContext
from agent_c_api.core.agent_manager import UItoAgentBridgeManager
from agent_c_api.core.util.middleware_logging import APILoggingMiddleware

logger = get_logger(__name__)

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
        from agent_c_api.config.env_config import settings
        
        # Validate Redis connection (no longer managing server lifecycle)
        with LoggingContext(operation="redis_validation"):
            logger.info("Validating Redis connection and configuration")
            redis_status = await RedisConfig.validate_connection()
        
        # Store Redis status in app state for health checks
        lifespan_app.state.redis_status = redis_status
        
        if redis_status["connected"]:
            logger.info("Redis connection successful",
                       host=redis_status['host'],
                       port=redis_status['port'],
                       database=redis_status['db'])
            
            # Log detailed server information
            if redis_status["server_info"]:
                info = redis_status["server_info"]
                logger.info("Redis server details retrieved",
                           redis_version=info.get('redis_version', 'unknown'),
                           redis_mode=info.get('redis_mode', 'unknown'),
                           memory_usage=info.get('used_memory_human', 'unknown'),
                           connected_clients=info.get('connected_clients', 'unknown'),
                           uptime_seconds=info.get('uptime_in_seconds', 'unknown'))
            
            # Log connection pool configuration
            logger.info("Redis connection configuration",
                       host=settings.REDIS_HOST,
                       port=settings.REDIS_PORT,
                       database=settings.REDIS_DB,
                       connection_timeout=getattr(settings, 'REDIS_CONNECT_TIMEOUT', 10),
                       socket_timeout=getattr(settings, 'REDIS_SOCKET_TIMEOUT', 10))
            
            # All Redis-dependent features will be available
            logger.info("Redis-dependent features available",
                       features=[
                           "session_management_and_persistence",
                           "user_data_storage",
                           "chat_history_caching",
                           "real_time_session_state"
                       ])
            
        else:
            logger.warning("Redis connection failed",
                          error=redis_status['error'],
                          attempted_host=redis_status['host'],
                          attempted_port=redis_status['port'],
                          attempted_database=redis_status['db'])
            logger.warning("Redis connection failure impact",
                          affected_features=[
                              "session_persistence_memory_only",
                              "user_data_storage_limited",
                              "chat_history_no_persistence",
                              "real_time_session_state_degraded"
                          ])
            logger.warning("Redis connection resolution instructions",
                          suggested_command=f"redis-server --port {redis_status['port']}",
                          suggestion="check_connection_settings_in_environment_configuration")
        
        # Shared AgentManager instance.
        with LoggingContext(operation="agent_manager_initialization"):
            logger.info("Initializing Agent Manager")
            lifespan_app.state.agent_manager = UItoAgentBridgeManager()
            logger.info("Agent Manager initialized successfully")
        
        # Initialize FastAPICache with InMemoryBackend
        with LoggingContext(operation="cache_initialization"):
            logger.info("Initializing FastAPI Cache")
            FastAPICache.init(InMemoryBackend(), prefix="agent_c_api_cache")
            logger.info("FastAPICache initialized with InMemoryBackend", backend="InMemoryBackend", prefix="agent_c_api_cache")
        
        # Log startup completion
        with LoggingContext(operation="application_startup"):
            logger.info("Application startup completed successfully",
                       redis_connected=redis_status['connected'])

        yield

        # Shutdown: Close Redis client connections
        with LoggingContext(operation="application_shutdown"):
            logger.info("Application shutdown initiated")
            logger.info("Closing Redis connections")
            try:
                await RedisConfig.close_client()
                logger.info("Redis connections closed successfully")
            except Exception as e:
                logger.error("Error during Redis cleanup",
                           error_type=type(e).__name__,
                           error_message=str(e),
                           exc_info=True)
            
            logger.info("Application shutdown completed")


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
    logger.info("CORS configuration initialized", allowed_hosts_regex=origin_regex)
    app.add_middleware(
        CORSMiddleware,
        allow_origin_regex=origin_regex,
        allow_credentials=True,
        allow_methods=["*"],
        allow_headers=["*"],
    )


    environment = getattr(settings, "ENV", os.getenv("ENV", "development")).lower()
    is_production = environment == "production"
    logger.info("Application environment detected", environment=environment, is_production=is_production)

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
    
    logger.info("Application created successfully", 
                app_name=app_name, 
                app_version=app_version)
    logger.info("API documentation endpoints configured",
                swagger_ui_url=docs_url,
                redoc_url=redoc_url,
                openapi_schema_url=openapi_url)

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
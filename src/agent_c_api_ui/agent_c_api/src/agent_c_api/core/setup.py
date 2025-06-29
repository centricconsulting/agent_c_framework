import os
import re

from typing import Dict, Any
from fastapi import FastAPI, APIRouter
from contextlib import asynccontextmanager
from starlette.middleware.cors import CORSMiddleware
from fastapi_cache import FastAPICache
from fastapi_cache.backends.inmemory import InMemoryBackend

from agent_c.chat import get_default_chat_session_manager
from agent_c.config import ModelConfigurationLoader
from agent_c.config.agent_config_loader import AgentConfigLoader
from agent_c.config.saved_chat import SavedChatLoader
from agent_c_api.config.env_config import settings
from agent_c_api.core.util.logging_utils import LoggingManager
from agent_c_api.core.user_session_manager import UserSessionManager
from agent_c_api.core.util.middleware_logging import APILoggingMiddleware
from agent_c.config.system_config_loader import SystemConfigurationLoader
from agent_c.config.prompt_section_loader import PromptSectionLoader

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
        logger.info("🔍 Loading application configuration and initializing resources...")
        SystemConfigurationLoader.instance()

        logger.info("Loading prompt sections...")
        PromptSectionLoader.instance().load_sections()

        # Import Redis configuration at runtime to avoid circular imports
        from agent_c_api.config.redis_config import RedisConfig
        from agent_c_api.config.env_config import settings

        
        # Validate Redis connection (no longer managing server lifecycle)
        logger.info("🔍 Validating Redis connection and configuration...")
        redis_status = await RedisConfig.validate_connection()
        
        # Store Redis status in app state for health checks
        lifespan_app.state.redis_status = redis_status
        
        if redis_status["connected"]:
            logger.info(f"✅ Redis connection successful at {redis_status['host']}:{redis_status['port']} (DB: {redis_status['db']})")
            
            # Log detailed server information
            if redis_status["server_info"]:
                info = redis_status["server_info"]
                logger.info(f"📊 Redis Server Details:")
                logger.info(f"   Version: {info.get('redis_version', 'unknown')}")
                logger.info(f"   Mode: {info.get('redis_mode', 'unknown')}")
                logger.info(f"   Memory Usage: {info.get('used_memory_human', 'unknown')}")
                logger.info(f"   Connected Clients: {info.get('connected_clients', 'unknown')}")
                logger.info(f"   Uptime: {info.get('uptime_in_seconds', 'unknown')} seconds")
            
            # Log connection pool configuration
            logger.info(f"🔧 Redis Connection Config:")
            logger.info(f"   Host: {settings.REDIS_HOST}")
            logger.info(f"   Port: {settings.REDIS_PORT}")
            logger.info(f"   Database: {settings.REDIS_DB}")
            logger.info(f"   Connection Timeout: {getattr(settings, 'REDIS_CONNECT_TIMEOUT', 10)}s")
            logger.info(f"   Socket Timeout: {getattr(settings, 'REDIS_SOCKET_TIMEOUT', 10)}s")
            
            # All Redis-dependent features will be available
            logger.info("🚀 All Redis-dependent features are available:")
            logger.info("   - Session management and persistence")
            logger.info("   - User data storage")
            logger.info("   - Chat history caching")
            logger.info("   - Real-time session state")
            
        else:
            logger.warning(f"⚠️ Redis connection failed: {redis_status['error']}")
            logger.warning(f"🔧 Connection attempted to: {redis_status['host']}:{redis_status['port']} (DB: {redis_status['db']})")
            logger.warning("")
            logger.warning("🚨 IMPACT: The following features will be affected:")
            logger.warning("   - Session persistence (sessions will be memory-only)")
            logger.warning("   - User data storage (limited functionality)")
            logger.warning("   - Chat history (no persistence between restarts)")
            logger.warning("   - Real-time session state (degraded performance)")
            logger.warning("")
            logger.warning("💡 To resolve: Ensure Redis server is running and accessible")
            logger.warning(f"   Command: redis-server --port {redis_status['port']}")
            logger.warning(f"   Or check connection settings in environment configuration")

        # Config loaders
        logger.info("🤖 Loading core configuration files...")
        lifespan_app.state.model_config_loader = ModelConfigurationLoader.instance()
        logger.info("✅ Model configurations successfully loaded")
        lifespan_app.state.agent_config_loader = AgentConfigLoader.instance()
        logger.info("✅ Agent configurations successfully loaded")

        # Repositories
        logger.info("💾 Initializing repositories...")
        lifespan_app.state.chat_session_repository = SavedChatLoader.instance()
        logger.info("✅ Chat session repository successfully loaded")

        # Shared AgentManager instance.
        logger.info("🤖 Initializing Session Managers...")
        lifespan_app.state.chat_session_manager = get_default_chat_session_manager()
        logger.info("✅ Chat session repository successfully loaded")

        lifespan_app.state.user_session_manager = UserSessionManager(lifespan_app.state.chat_session_manager)
        logger.info("✅ User Session Manager initialized successfully")
        
        # Initialize FastAPICache with InMemoryBackend
        logger.info("💾 Initializing FastAPI Cache...")
        FastAPICache.init(InMemoryBackend(), prefix="agent_c_api_cache")
        logger.info("✅ FastAPICache initialized with InMemoryBackend")
        
        # Log startup completion
        logger.info("🎉 Application startup completed successfully")
        logger.info(f"📍 Redis Status: {'Connected' if redis_status['connected'] else 'Disconnected'}")

        yield

        # Shutdown: Close Redis client connections
        logger.info("🔄 Application shutdown initiated...")
        logger.info("🔌 Closing Redis connections...")
        try:
            await RedisConfig.close_client()
            logger.info("✅ Redis connections closed successfully")
        except Exception as e:
            logger.error(f"❌ Error during Redis cleanup: {e}")
        
        logger.info("👋 Application shutdown completed")


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
import time
import uvicorn
import os
from dotenv import load_dotenv
from agent_c.util.structured_logging import get_logger, LoggingContext
from agent_c_api.api import router
from agent_c_api.config.env_config import settings
from agent_c_api.core.setup import create_application
from fastapi import FastAPI

load_dotenv(override=True)

# dictionary to track performance metrics
_timing = {
    "start_time": time.time(),
    "app_creation_start": 0,
    "app_creation_end": 0
}

# Configure structured logging
logger = get_logger(__name__)

# Note: External logger configuration is handled by the core's structured logging infrastructure

# Create and configure the FastAPI application
with LoggingContext(operation="app_initialization", component="main"):
    logger.info("Creating API application",
                host=settings.HOST,
                port=settings.PORT,
                reload_enabled=settings.RELOAD)
    _timing["app_creation_start"] = time.time()

    # Create the application with our router
    app = create_application(router=router, settings=settings)


    _timing["app_creation_end"] = time.time()
    app_creation_duration = _timing["app_creation_end"] - _timing["app_creation_start"]
    
    logger.info("API application created successfully",
                routes_registered=len(app.routes),
                creation_duration_ms=round(app_creation_duration * 1000, 2),
                api_versions=["v1", "v2"],
                directory_structure_routing=True)

def run():
    """Entrypoint for the API"""
    with LoggingContext(operation="server_startup", component="uvicorn"):
        # Get log level from environment or default to INFO
        log_level = os.getenv('LOG_LEVEL', 'INFO').lower()
        
        logger.info("Starting Agent C API server",
                    host=settings.HOST,
                    port=settings.PORT,
                    reload_enabled=settings.RELOAD,
                    log_level=log_level)
        
        startup_time = time.time()
        
        try:
            # If reload is enabled, we must use the import string
            if settings.RELOAD:
                uvicorn.run(
                    "agent_c_api.main:app",
                    host=settings.HOST,
                    port=settings.PORT,
                    reload=settings.RELOAD,
                    log_level=log_level
                )
            else:
                # Otherwise, we can use the app object directly for better debugging
                uvicorn.run(
                    app,
                    host=settings.HOST,
                    port=settings.PORT,
                    log_level=log_level
                )
        except Exception as e:
            logger.error("Server startup failed",
                         error_type=type(e).__name__,
                         error_message=str(e),
                         host=settings.HOST,
                         port=settings.PORT)
            raise
        finally:
            shutdown_time = time.time()
            total_runtime = shutdown_time - startup_time
            logger.info("Server shutdown completed",
                        total_runtime_seconds=round(total_runtime, 2),
                        shutdown_timestamp=shutdown_time)

if __name__ == "__main__":
    run()
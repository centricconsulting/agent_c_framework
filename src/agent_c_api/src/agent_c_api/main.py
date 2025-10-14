import os
import time
import uvicorn
import logging
from dotenv import load_dotenv
from fastapi.logger import logger as fastapi_logger
from agent_c_api.core.util.logging_utils import LoggingManager
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

# Configure logging
LoggingManager.configure_root_logger()
LoggingManager.configure_external_loggers()
LoggingManager.configure_external_loggers({
    "agent_c_api.core.util.middleware_logging": "WARNING"  # Show INFO logs for middleware_logging, debug is too noisy
})

# Configure specific loggers for FastAPI components
logging_manager = LoggingManager("agent_c_api")
logger = logging_manager.get_logger()

# This ensures sub-loggers own logs use our formatting
uvicorn_logger = logging.getLogger("uvicorn")
sub_loggers = [fastapi_logger, uvicorn_logger]

for sub_logger in sub_loggers:
    sub_logger.handlers = []
    for handler in logger.handlers:
        sub_logger.addHandler(handler)
    sub_logger.setLevel(logger.level)
    sub_logger.propagate = False

# Adjust levels for specific uvicorn loggers rather than removing handlers
logging.getLogger("uvicorn.access").setLevel(logging.WARNING)
logging.getLogger("uvicorn.error").setLevel(logging.ERROR)

# Create and configure the FastAPI application
logger.info("Creating API application")
_timing["app_creation_start"] = time.time()

# Create the application with our router
app = create_application(router=router, settings=settings)

_timing["app_creation_end"] = time.time()
logger.info(f"Registered {len(app.routes)} routes")
logger.info(f"API application created in {(_timing['app_creation_end'] - _timing['app_creation_start']):.2f} seconds")
logger.info("API versioning using directory structure with paths '/api/v1' and '/api/v2'")

def run():
    """Entrypoint for the API"""
    logger.info(f"FastAPI Reload Setting is: {settings.RELOAD}. Starting Uvicorn")
    logger.info(f"Agent_C API server running on {settings.HOST}:{settings.PORT}")
    logger.info(f"Working Directory: {os.getcwd()}")
    
    # If reload is enabled, we must use the import string
    if settings.RELOAD:
        uvicorn.run(
            "agent_c_api.main:app",
            host=settings.HOST,
            port=settings.PORT,
            ssl_keyfile="./agent_c_config/localhost_self_signed-key.pem",
            ssl_certfile="./agent_c_config/localhost_self_signed.pem",
            reload=settings.RELOAD,
            log_level=LoggingManager.LOG_LEVEL.lower() if hasattr(LoggingManager, 'LOG_LEVEL') else "info"
        )
    else:
        # Otherwise, we can use the app object directly for better debugging
        if os.environ.get("RUNNING_IN_DOCKER", "false").lower() == "true":
            logger.info("Detected running in Docker, disabling SSL for Uvicorn")
            uvicorn.run(
                app,
                host=settings.HOST,
                port=settings.PORT,
                log_level=LoggingManager.LOG_LEVEL.lower() if hasattr(LoggingManager, 'LOG_LEVEL') else "info"
            )
        else:
            uvicorn.run(
                app,
                host=settings.HOST,
                port=settings.PORT,
                ssl_keyfile="agent_c_config/localhost_self_signed-key.pem",
                ssl_certfile="agent_c_config/localhost_self_signed.pem",
                log_level=LoggingManager.LOG_LEVEL.lower() if hasattr(LoggingManager, 'LOG_LEVEL') else "info"
            )
    logger.info(f"Exiting Run Loop: {time.time()}")

if __name__ == "__main__":
    run()
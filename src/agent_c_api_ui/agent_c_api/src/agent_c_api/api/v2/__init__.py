# src/agent_c_api/api/v2/__init__.py
from fastapi import APIRouter
import logging
# Removed fastapi_versioning import - using directory structure for versioning

# Create the v2 API router with a descriptive tag and version annotation
router = APIRouter(
    tags=["v2"],  # Global tag for all v2 endpoints
)

# Version is now determined by directory structure and router prefixes

# Import all implemented resource routers
from .config import router as config_router
from .sessions import router as sessions_router
from .history import router as history_router
from .debug import router as debug_router

# Include all routers in the main v2 API router
router.include_router(config_router)
router.include_router(sessions_router)
router.include_router(history_router)
router.include_router(debug_router)

# Log the number of routes included for debugging
logger = logging.getLogger(__name__)
logger.info(f"V2 API initialized with {len(router.routes)} routes")
from fastapi import APIRouter
# Removed fastapi_versioning import - using directory structure for versioning

from .sessions import router as sessions_router
from .models import router as models_router
from .tools import router as tools_router
from .agent import router as agent_router
from .chat import router as chat_router
from .files import router as files_router
from .personas import router as personas_router
from .interactions.interactions import router as interactions_router
from .interactions.events import router as events_router

import logging
# Add v1 tag for all routes in this version
router = APIRouter(tags=["v1"])

# Version is now determined by directory structure and router prefixes

# main api routes
router.include_router(sessions_router)
router.include_router(models_router)
router.include_router(tools_router)
router.include_router(agent_router)
router.include_router(chat_router)
router.include_router(files_router)
router.include_router(personas_router)
router.include_router(interactions_router)
router.include_router(events_router)


# logging.debug(f"Including routers: {[r.prefix for r in [sessions_router, models_router, tools_router, agent_router, chat_router, files_router, personas_router]]}")
from fastapi import APIRouter
from .sessions import router as sessions_router
from .agent import router as agent_router
from .chat import router as chat_router
from .files import router as files_router

router = APIRouter()
router.include_router(sessions_router)
router.include_router(agent_router)
router.include_router(chat_router)
router.include_router(files_router)

__all__ = ["router"]
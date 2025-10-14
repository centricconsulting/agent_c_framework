from fastapi import APIRouter
from .rt import router as rt_router

# Create main API router with explicit /api prefix
router = APIRouter(prefix="/api")

router.include_router(rt_router, prefix="/rt")

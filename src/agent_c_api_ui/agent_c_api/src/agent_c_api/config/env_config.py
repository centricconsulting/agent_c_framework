from pathlib import Path
from typing import Optional
from pydantic_settings import BaseSettings

class Settings(BaseSettings):
    """Application settings loaded from environment variables"""
    
    # Redis Configuration
    REDIS_HOST: str = "localhost"
    REDIS_PORT: int = 6379
    REDIS_DB: int = 0
    REDIS_USERNAME: Optional[str] = None
    REDIS_PASSWORD: Optional[str] = None
    REDIS_DATA_DIR: Path = Path("./data/redis")
    REDIS_STARTUP_TIMEOUT: int = 30
    MANAGE_REDIS_LIFECYCLE: bool = True
    
    # Session Configuration
    SESSION_TTL: int = 24 * 60 * 60  # 24 hours
    SESSION_CLEANUP_INTERVAL: int = 60 * 60  # 1 hour
    SESSION_CLEANUP_BATCH_SIZE: int = 100
    
    # Feature Flags
    USE_REDIS_SESSIONS: bool = True
    
    class Config:
        env_file = ".env"
        env_file_encoding = "utf-8"

settings = Settings()
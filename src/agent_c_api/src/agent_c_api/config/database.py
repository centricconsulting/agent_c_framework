"""
Database configuration and setup for Avatar API authentication.

This module provides SQLite database configuration and async session management
for user authentication and profile storage.
"""

import os
import aiosqlite
from pathlib import Path
from typing import AsyncGenerator
from sqlalchemy.ext.asyncio import create_async_engine, AsyncSession, async_sessionmaker
from sqlalchemy.pool import StaticPool

from agent_c_api.models.auth_models import Base


class DatabaseConfig:
    """Configuration class for database setup and management."""
    
    def __init__(self, database_url: str = None):
        """
        Initialize database configuration.
        
        Args:
            database_url: Optional database URL override
        """
        if database_url:
            self.database_url = database_url
        else:
            # Default to SQLite in the data directory
            db_path = Path("agent_c_config/chat_user_auth.db")
            db_path.parent.mkdir(parents=True, exist_ok=True)
            self.database_url = f"sqlite+aiosqlite:///{db_path}"
        
        # Create async engine with appropriate settings for SQLite
        self.engine = create_async_engine(
            self.database_url,
            echo=False,  # Set to True for SQL logging during development
            poolclass=StaticPool,
            connect_args={
                "check_same_thread": False,
                "timeout": 30,
            } if "sqlite" in self.database_url else {}
        )
        
        # Create session factory
        self.async_session_factory = async_sessionmaker(
            self.engine,
            class_=AsyncSession,
            expire_on_commit=False
        )
    
    async def initialize_database(self):
        """
        Initialize the database by creating all tables.
        This should be called during application startup.
        """
        async with self.engine.begin() as conn:
            await conn.run_sync(Base.metadata.create_all)
    
    async def get_session(self) -> AsyncGenerator[AsyncSession, None]:
        """
        Get an async database session.
        
        Yields:
            AsyncSession: Database session for queries
        """
        async with self.async_session_factory() as session:
            try:
                yield session
            except Exception:
                await session.rollback()
                raise
            finally:
                await session.close()
    
    async def close(self):
        """Close the database engine and connections."""
        await self.engine.dispose()


# Global database instance
_db_config: DatabaseConfig = None


def get_database_config() -> DatabaseConfig:
    """
    Get the global database configuration instance.
    
    Returns:
        DatabaseConfig: The database configuration singleton
    """
    global _db_config
    if _db_config is None:
        # Initialize with default settings or from environment
        db_url = os.getenv("AGENT_C_AUTH_DATABASE_URL")
        _db_config = DatabaseConfig(db_url)
    return _db_config


async def get_db_session() -> AsyncGenerator[AsyncSession, None]:
    """
    Dependency function to get database session for FastAPI.
    
    Yields:
        AsyncSession: Database session for request handling
    """
    db_config = get_database_config()
    async for session in db_config.get_session():
        yield session


async def initialize_database():
    """
    Initialize the database tables.
    This should be called during application startup.
    """
    db_config = get_database_config()
    await db_config.initialize_database()


async def close_database():
    """
    Close database connections.
    This should be called during application shutdown.
    """
    global _db_config
    if _db_config:
        await _db_config.close()
        _db_config = None
import os
import sys
import time
import asyncio
import subprocess
import logging
from pathlib import Path
from typing import Optional, Tuple
from redis import asyncio as aioredis
from redis.exceptions import ConnectionError
from .env_config import settings

logger = logging.getLogger(__name__)

class RedisConfig:
    _redis_server_process: Optional[subprocess.Popen] = None
    
    @staticmethod
    async def get_redis_client():
        return aioredis.Redis(
            host=settings.REDIS_HOST,
            port=settings.REDIS_PORT,
            db=settings.REDIS_DB,
            username=settings.REDIS_USERNAME,
            password=settings.REDIS_PASSWORD,
            encoding="utf8",
            decode_responses=True
        )
    
    @classmethod
    async def ping_redis(cls) -> bool:
        """Check if Redis is available"""
        try:
            redis = await cls.get_redis_client()
            return await redis.ping()
        except (ConnectionError, ConnectionRefusedError):
            return False
        except Exception as e:
            logger.error(f"Error pinging Redis: {e}")
            return False
    
    @classmethod
    async def start_redis_if_needed(cls) -> bool:
        """
        Start Redis server if configured to manage lifecycle.
        Returns True if Redis is ready to use (either started or already running).
        """
        # Check if Redis is already running
        if await cls.ping_redis():
            logger.info("Redis is already running")
            return True
            
        if not settings.MANAGE_REDIS_LIFECYCLE:
            logger.warning("Redis is not running and MANAGE_REDIS_LIFECYCLE is disabled")
            return False
        
        # Ensure Redis data directory exists
        data_dir = settings.REDIS_DATA_DIR
        data_dir.mkdir(parents=True, exist_ok=True)
        
        # Start Redis server process
        try:
            # Check if redis-server is available
            if not cls._is_redis_server_available():
                logger.error("redis-server executable not found in PATH")
                return False
                
            port = settings.REDIS_PORT
            logger.info(f"Starting Redis server on port {port}")
            
            # Start Redis using subprocess
            cls._redis_server_process = subprocess.Popen(
                [
                    "redis-server",
                    "--port", str(port),
                    "--dir", str(data_dir),
                    "--dbfilename", "dump.rdb",
                    "--daemonize", "no"  # Run in foreground
                ],
                stdout=subprocess.PIPE,
                stderr=subprocess.PIPE,
                text=True
            )
            
            # Wait for Redis to start
            return await cls._wait_for_redis_ready(settings.REDIS_STARTUP_TIMEOUT)
            
        except Exception as e:
            logger.error(f"Failed to start Redis server: {e}")
            return False
    
    @classmethod
    async def stop_redis_if_needed(cls) -> bool:
        """Stop Redis server if we started it"""
        if cls._redis_server_process is None:
            return True  # Nothing to stop
            
        try:
            logger.info("Stopping Redis server")
            
            # Try graceful shutdown first
            redis = await cls.get_redis_client()
            try:
                await redis.shutdown(save=True)
                await asyncio.sleep(1)  # Give a moment for shutdown
            except Exception as e:
                logger.warning(f"Could not shutdown Redis gracefully: {e}")
            
            # If process is still running, terminate it
            if cls._redis_server_process.poll() is None:
                cls._redis_server_process.terminate()
                try:
                    cls._redis_server_process.wait(timeout=5)
                except subprocess.TimeoutExpired:
                    logger.warning("Redis did not terminate in time, forcing kill")
                    cls._redis_server_process.kill()
            
            cls._redis_server_process = None
            return True
            
        except Exception as e:
            logger.error(f"Error stopping Redis server: {e}")
            return False
    
    @classmethod
    async def _wait_for_redis_ready(cls, timeout: int) -> bool:
        """Wait for Redis to be ready to accept connections"""
        start_time = time.time()
        while time.time() - start_time < timeout:
            # Check if process has exited unexpectedly
            if cls._redis_server_process and cls._redis_server_process.poll() is not None:
                return_code = cls._redis_server_process.poll()
                stderr = cls._redis_server_process.stderr.read() if cls._redis_server_process.stderr else "No error output"
                logger.error(f"Redis server exited unexpectedly with code {return_code}. Error: {stderr}")
                cls._redis_server_process = None
                return False
                
            # Check if Redis is responding
            if await cls.ping_redis():
                logger.info(f"Redis server started successfully after {time.time() - start_time:.2f} seconds")
                return True
                
            await asyncio.sleep(0.5)
            
        logger.error(f"Redis server did not become ready within {timeout} seconds")
        return False
    
    @staticmethod
    def _is_redis_server_available() -> bool:
        """Check if redis-server executable is available in PATH"""
        if sys.platform == "win32":
            cmd = "where redis-server"
        else:
            cmd = "which redis-server"
            
        try:
            result = subprocess.run(cmd, shell=True, capture_output=True, text=True)
            return result.returncode == 0 and bool(result.stdout.strip())
        except Exception:
            return False


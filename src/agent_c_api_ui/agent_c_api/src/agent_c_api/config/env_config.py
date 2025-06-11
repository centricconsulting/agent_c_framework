# config.py
import os
from pathlib import Path
from typing import Optional
from pydantic_settings import BaseSettings, SettingsConfigDict


def get_project_root(marker: str = "pyproject.toml") -> Path:
    """
    Walk up the directory tree until a directory containing the marker file is found.
    This helps us find the project root regardless of the current working directory.
    By default we use the pyproject.toml file as the marker.
    Raises a RuntimeError if not found.
    """
    current_path = Path(__file__).resolve()
    for parent in current_path.parents:
        if (parent / marker).exists():
            return parent
    raise RuntimeError(f"Could not find project root marker ({marker}) in any parent directory.")


class Settings(BaseSettings):
    ## Application metadata
    APP_NAME: str = "Agent C FastAPI Backend"
    APP_DESCRIPTION: str = "A FastAPI backend for Agent C"
    CONTACT_NAME: str = "Joseph Ours"
    CONTACT_EMAIL: str = "joseph.ours@centricconsulting.com"
    LICENSE_NAME: str = "BSL 1.1"
    APP_VERSION: str = "0.1.0"

    # Base directories
    BASE_DIR: Path = get_project_root() / "src"
    CONFIG_DIR: Path = BASE_DIR / "agent_c_api/config"
    PERSONA_DIR: Path = BASE_DIR.parent.parent.parent.parent / "personas"

    # Specific file paths
    MODEL_CONFIG_PATH: Path = CONFIG_DIR / "model_configs.json"

    # FastAPI and CORS settings
    ALLOWED_ORIGINS: list[str] = ["*"]
    HOST: str = os.environ.get("AGENT_C_API_HOST", "0.0.0.0")
    PORT: int = int(os.environ.get("AGENT_C_API_PORT", 8000))
    RELOAD: bool = False

    # Agent settings
    CALLBACK_TIMEOUT: float = 300.0  # Timeout in seconds for stream callbacks

    # Profile API App
    PROFILING_ENABLED: bool = False

    # Redis Configuration
    REDIS_HOST: str = "localhost"
    REDIS_PORT: int = 6379
    REDIS_DB: int = 0
    REDIS_USERNAME: Optional[str] = None
    REDIS_PASSWORD: Optional[str] = None

    # Connection timeouts and pooling
    REDIS_CONNECTION_TIMEOUT: int = 5  # Connection timeout in seconds
    REDIS_SOCKET_TIMEOUT: int = 5      # Socket timeout in seconds
    REDIS_MAX_CONNECTIONS: int = 50    # Maximum connections in pool
    
    # Redis Resilient Mode Configuration
    #rohan batra
    REDIS_OPERATION_MODE: str = "REDIS_ONLY"  # REDIS_ONLY, HYBRID, ASYNC_ONLY
    REDIS_ENABLE_RESILIENT_MODE: bool = True  # Enable/disable resilient mode features
    REDIS_AUTO_MODE_SWITCHING: bool = False  # Enable automatic mode switching based on health
    # end
    REDIS_FALLBACK_MODE: str = "ASYNC_ONLY"  # Default fallback mode when Redis is unavailable
    
    # Health Monitoring Configuration
    #rohan
    REDIS_HEALTH_CHECK_INTERVAL: int = 30  #initially 30 Interval for health checks in seconds
    REDIS_HEALTH_CHECK_TIMEOUT: int = 5  # Timeout for health checks in seconds
    #rohan
    REDIS_FAILURE_THRESHOLD: int = 3  #initialy 3 Number of failures before switching modes
    REDIS_RECOVERY_THRESHOLD: int = 5  # Number of successes before recovering
    
    # Performance Configuration
    REDIS_CIRCUIT_BREAKER_TIMEOUT: int = 60  # Circuit breaker timeout duration in seconds
    REDIS_MAX_RETRIES: int = 3  # Maximum retry attempts for operations
    REDIS_RETRY_BACKOFF: str = "exponential"  # Backoff strategy: exponential or linear
    REDIS_CONNECTION_POOL_SIZE: int = 20  # Redis connection pool size
    
    # Stream Configuration
    REDIS_STREAM_MAX_LEN: int = 10000  # Maximum stream length for trimming
    REDIS_CONSUMER_GROUP_PREFIX: str = "agent_c"  # Prefix for consumer group names
    REDIS_EVENT_TTL: int = 604800  # Event time-to-live in streams (7 days in seconds)
    
    # Mode Switching Configuration
    REDIS_MODE_SWITCH_DELAY: int = 30  # Minimum delay between mode switches in seconds
    REDIS_MAX_TRANSITION_RETRIES: int = 3  # Maximum retries for mode transitions
    REDIS_ENABLE_AUTO_RECOVERY: bool = True  # Enable automatic recovery from failures
    REDIS_PERFORMANCE_MONITORING: bool = True  # Enable performance monitoring
    
    # DEPRECATED SETTINGS - No longer used after Redis refactor
    # Redis should be externally managed (Docker, systemd, cloud service, etc.)
    # These settings are kept for backward compatibility but are ignored
    REDIS_DATA_DIR: Path = Path("./data/redis")  # DEPRECATED: Use external Redis
    REDIS_STARTUP_TIMEOUT: int = 30  # DEPRECATED: No embedded Redis startup
    MANAGE_REDIS_LIFECYCLE: bool = False  # DEPRECATED: Always False - external Redis only

    # Session Configuration
    SESSION_TTL: int = 24 * 60 * 60  # 24 hours
    SESSION_CLEANUP_INTERVAL: int = 60 * 60  # 1 hour
    SESSION_CLEANUP_BATCH_SIZE: int = 100

    # Feature Flags
    USE_REDIS_SESSIONS: bool = True
    # rohan
    USE_REDIS_STREAMS: bool = True

    # Redis Stream Configuration
    STREAM_PREFIX: str = "agent_c:stream:"
    STREAM_MAX_LENGTH: int = 10000  # Maximum number of events in a stream
    STREAM_TRIM_INTERVAL: int = 100  # How often to trim streams (event count)
    STREAM_RETENTION_PERIOD: int = 7 * 24 * 60 * 60  # 7 days in seconds

    # Allows you to override settings via a .env file
    model_config = SettingsConfigDict(
        env_file=str(BASE_DIR.parent.parent.parent.parent / ".env"),  # Get the .env file from the root of the project
        env_file_encoding="utf-8",
        extra="allow"  # This permits extra keys not defined in the model
    )


# Can use getattr(settings, "SECRET_KEY", None) to get the value of SECRET_KEY
# Instantiate the settings
settings = Settings()
# config.py
import os
from pathlib import Path
from pydantic_settings  import BaseSettings, SettingsConfigDict


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
    
    # Redis settings
    REDIS_URL: str = os.environ.get("REDIS_URL", "redis://localhost:6379/0")
    REDIS_STREAM_ENABLED: bool = os.environ.get("REDIS_STREAM_ENABLED", "False").lower() == "true"

    # Profile API App
    PROFILING_ENABLED: bool = False

    # Allows you to override settings via a .env file
    model_config = SettingsConfigDict(
        env_file=str(BASE_DIR.parent.parent.parent.parent / ".env"), # Get the .env file from the root of the project
        env_file_encoding="utf-8",
        extra="allow"  # This permits extra keys not defined in the model
    )
# Can use getattr(settings, "SECRET_KEY", None) to get the value of SECRET_KEY
# Instantiate the settings
settings = Settings()

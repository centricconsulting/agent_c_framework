from pathlib import Path

from pydantic import Field

from agent_c.models.config import BaseApiConfig

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

class AppConfig(BaseApiConfig):
    """
    This class holds the configuration settings for the api "application"
    """
    version: str = Field("0.1.4")
    name: str = Field("Agent C Chat API")
    description: str = Field("Chat API for the Agent C framework.")
    contact_name: str = Field("Joseph Ours")
    contact_email: str = Field("joseph.ours@centricconsulting.com")
    license_name: str = Field("BSL 1.1")
    base_dir: str = Field(default_factory= lambda: str(get_project_root() / "src"))




import os
from typing import Optional, Tuple


def get_github_token() -> Tuple[bool, str]:
    """
    Get the GitHub personal access token from environment variables.
    
    Returns:
        Tuple of (success, token_or_error_message)
    """
    token = os.environ.get("GITHUB_PERSONAL_ACCESS_TOKEN")
    if not token:
        return False, "GitHub personal access token not found. Set GITHUB_PERSONAL_ACCESS_TOKEN environment variable."
    return True, token


def get_github_host() -> Optional[str]:
    """
    Get the GitHub host from environment variables.
    
    Returns:
        GitHub host URL or None if not specified.
    """
    return os.environ.get("GITHUB_HOST")
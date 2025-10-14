from typing import List, Tuple, Optional
import logging

from ..tools.workspace import WorkspaceTools

logger = logging.getLogger(__name__)

def normalize_path(path: str) -> str:
    """Normalize path separators and ensure consistency."""
    return path.replace('\\', '/')

def create_unc_path(workspace: str, relative_path: str) -> str:
    """Create a UNC path from workspace and relative path."""
    if not workspace:
        raise ValueError("Workspace name cannot be empty")

    normalized_path = normalize_path(relative_path).lstrip('/')
    formed_path = f"//{workspace}/{normalized_path}"
    return formed_path

def has_file_extension(filename: str, extensions: List[str]) -> bool:
    """Check if a file is a markdown file."""
    filename_lower = filename.lower()
    suffixes = tuple(ext.lower() for ext in extensions)
    return filename_lower.endswith(suffixes)

def ensure_file_extension(filename: str, extension: str) -> str:
    """Ensure filename has the specified extension."""
    filename = normalize_path(filename)

    # Normalize the extension to ensure it starts with a dot
    if not extension.startswith('.'):
        extension = f'.{extension}'

    # Check if filename already ends with the extension (case-insensitive)
    if not filename.lower().endswith(extension.lower()):
        return f"{filename}{extension}"
    return filename

def os_file_system_path(workspace_tool: WorkspaceTools, unc_path: str) -> str|None:
    try:
        _, workspace_obj, rel_path = workspace_tool._parse_unc_path(unc_path)
        if workspace_obj and hasattr(workspace_obj, 'full_path'):
            return workspace_obj.full_path(rel_path, mkdirs=False)
        return None
    except Exception as e:
        logger.error(f"Error getting file system path: {e}")
        return None

def os_path(workspace_tools: WorkspaceTools,
                        unc_path: str,
                        *,
                        mkdirs: bool = False) -> Tuple[Optional[str], Optional[str]]:
    """
    Resolve //WORKSPACE/relative/path -> absolute OS path safely.

    Returns (error, abs_path). If error is not None, abs_path is None.
    """
    err, workspace, rel_path = workspace_tools.validate_and_get_workspace_path(unc_path)
    if err:
        return err, None
    try:
        abs_path = workspace.full_path(rel_path or "", mkdirs=mkdirs)  # validated + normalized
        if not abs_path:
            return f"Invalid path: {rel_path!r}", None
        return None, abs_path
    except Exception as e:
        return f"Failed to resolve path: {e}", None
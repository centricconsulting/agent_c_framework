import os
from pathlib import Path

from agent_c.util.logging_utils import LoggingManager
from agent_c_tools.tools.workspace.local_storage import LocalStorageWorkspace
from agent_c.util import locate_project_root_folder

class LocalProjectWorkspace(LocalStorageWorkspace):
    """
    A workspace that automatically determines the project path using a fallback strategy:
    1. Uses PROJECT_WORKSPACE_PATH environment variable if available
    2. Uses 'app/workspaces/project' path if it exists
    3. Defaults to current working directory

    The description can be overridden via PROJECT_WORKSPACE_DESCRIPTION environment variable.
    """
    def __init__(self, name="project", description="A workspace holding the `Agent C` source code in Python."):
        self.logger = LoggingManager(self.__name__).get_logger()
        self.logger.info("Initializing LocalProjectWorkspace")
        workspace_path = self._determine_workspace_path()
        super().__init__(
            name=name,
            workspace_path=workspace_path,
            description=description
        )

    def _determine_workspace_path(self) -> str:
        if "PROJECT_WORKSPACE_PATH" in os.environ:
            self.logger.info(f"Found PROJECT_WORKSPACE_PATH environment variable: {os.environ['PROJECT_WORKSPACE_PATH']}")
            return os.environ["PROJECT_WORKSPACE_PATH"]

        app_workspace_path = Path("/app/workspaces/project")
        if app_workspace_path.exists():
            self.logger.info(f"Found /app/workspaces/project directory: {str(app_workspace_path.absolute())}")
            return str(app_workspace_path.absolute())

        self.logger.info(f"Using current working directory as the project workspace: {os.getcwd()}")
        return locate_project_root_folder()

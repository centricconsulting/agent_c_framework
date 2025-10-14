import os
import json
import docker
import logging
import tempfile
from typing import Dict, Any, List, Optional
from datetime import datetime

from .prompt import CodeInterpreterToolsSection
from agent_c.toolsets import Toolset, json_schema


class CodeInterpreterTools(Toolset):
    """
    Allows your agent to write and execute Python code safely in an isolated environment. Your agent can
    analyze data, create visualizations, perform calculations, install libraries, and work with files
    while keeping your system secure through containerized execution.
    """

    def __init__(self, **kwargs):
        """
        Initialize the CodeInterpreterTools.

        Args:
            **kwargs: Passed to parent Toolset.
                Required tools: 'workspace' for managing input/output files
        """
        super().__init__(**kwargs, name='code_interpreter')
        self.workspace_tool = self.tool_chest.available_tools.get('WorkspaceTools')

        self.logger = logging.getLogger(__name__)
        self.section = CodeInterpreterToolsSection()

        # Docker configuration
        self.default_image_tag = "code-interpreter:latest"
        self.dockerfile_path = os.path.join(os.path.dirname(__file__), "docker")

        # File handling configuration for input/output
        self.code_output_dir = os.path.join("code_interpreter", "code_outputs")
        self.input_files_dir = os.path.join("code_interpreter", "code_inputs")
        self.unsafe_mode = False

    def _verify_docker_image(self) -> None:
        """
        Verify the Docker image exists or build it if missing.
        """
        client = docker.from_env()

        try:
            client.images.get(self.default_image_tag)
        except docker.errors.ImageNotFound:
            # Get project workspace to locate Dockerfile
            workspace = self.workspace_tool.find_workspace_by_name('project')
            if workspace is None:
                raise ValueError("Project workspace not found")

            if not os.path.exists(self.dockerfile_path):
                raise FileNotFoundError(f"Dockerfile not found in {self.dockerfile_path}")

            self.logger.info(f"Building Docker image {self.default_image_tag}")
            client.images.build(
                path=self.dockerfile_path,
                tag=self.default_image_tag,
                rm=True,
            )

    def _install_libraries(self, container: docker.models.containers.Container, libraries: List[str]) -> None:
        """
        Install Python libraries in the Docker container.

        Args:
            container: Docker container instance
            libraries: List of library names to install
        """
        for library in libraries:
            result = container.exec_run(f"pip install {library}")
            if result.exit_code != 0:
                self.logger.warning(f"Failed to install library {library}: {result.output.decode('utf-8')}")

    async def _prepare_workspace_dirs(self, workspace_name: str) -> tuple[str, str]:
        """
        Prepare input and output directories in the workspace.

        Args:
            workspace_name: Name of the workspace to use

        Returns:
            Tuple of (input directory path, output directory path)
        """
        workspace = self.workspace_tool.find_workspace_by_name(workspace_name)
        if workspace is None:
            raise ValueError(f"Workspace {workspace_name} not found")

        # Ensure input/output directories exist
        try:
            # Only create .keep files if directories don't exist
            input_path = os.path.join("code_interpreter", self.input_files_dir)
            output_path = os.path.join("code_interpreter", self.code_output_dir)

            # NOTE: If we want to keep input/output folders fairly unique, not 100%.  We can add timestamp to the folder name
            # input_dir = os.path.join(self.input_files_dir, datetime.now().strftime("%Y%m%d_%H%M%S"))
            # output_dir = os.path.join(self.code_output_dir, datetime.now().strftime("%Y%m%d_%H%M%S"))

            # Check if directories exist by trying to list them
            input_exists = 'error' not in await workspace.ls(input_path)
            output_exists = 'error' not in await workspace.ls(output_path)

            # Create .keep files only if directories don't exist
            if not input_exists:
                self.logger.debug(f"Creating input directory: {input_path}")
                await workspace.write(os.path.join(input_path, '.keep'), 'w', '')

            if not output_exists:
                self.logger.debug(f"Creating output directory: {output_path}")
                await workspace.write(os.path.join(output_path, '.keep'), 'w', '')

        except Exception as e:
            self.logger.warning(f"Error while preparing directories: {str(e)}")
            # Continue anyway as the directories might still be usable

        # Create directories if they don't exist
        # input_dir = os.path.join(self.input_files_dir, datetime.now().strftime("%Y%m%d_%H%M%S"))
        # output_dir = os.path.join(self.code_output_dir, datetime.now().strftime("%Y%m%d_%H%M%S"))
        #
        # await workspace.write(os.path.join(input_dir, '.keep'), 'w', '')
        # await workspace.write(os.path.join(output_dir, '.keep'), 'w', '')

        return self.input_files_dir, self.code_output_dir

    async def _copy_input_files(self, file_paths: List[str], workspace_name: str, input_dir: str) -> List[str]:
        """
        Copy input files to the workspace input directory.

        Args:
            file_paths: List of file paths in the workspace to copy
            workspace_name: Name of the workspace
            input_dir: Directory to copy files to

        Returns:
            List of copied file paths relative to input directory
        """
        workspace = self.workspace_tool.find_workspace_by_name(workspace_name)
        if workspace is None:
            raise ValueError(f"Workspace {workspace_name} not found")

        copied_files = []
        for file_path in file_paths:
            try:
                # Construct the destination path
                dest_path = os.path.join(input_dir, os.path.basename(file_path))

                # Use the new copy function
                result = json.loads(await self.workspace_tool.copy(
                    workspace=workspace_name,
                    source_path=file_path,
                    dest_path=dest_path
                ))

                if 'error' in result:
                    self.logger.error(f"Failed to copy file {file_path}: {result['error']}")
                    continue

                copied_files.append(os.path.basename(file_path))
                self.logger.debug(f"Successfully copied {file_path} to {dest_path}")

            except Exception as e:
                self.logger.error(f"Failed to copy file {file_path}: {str(e)}")
                continue

        return copied_files

    def _init_docker_container(self, workspace_path: str, input_dir: str,
                               output_dir: str) -> docker.models.containers.Container:
        container_name = "code-interpreter"
        client = docker.from_env()

        # Remove existing container if present
        try:
            existing = client.containers.get(container_name)
            existing.stop()
            existing.remove()
        except docker.errors.NotFound:
            pass

        # Create workspace directory under code_interpreter
        workspace_dir = os.path.join('code_interpreter', 'workspace')

        # If input_dir and output_dir already include 'code_interpreter', don't add it again
        if not input_dir.startswith('code_interpreter'):
            input_path = os.path.join(workspace_path, 'code_interpreter', input_dir)
        else:
            input_path = os.path.join(workspace_path, input_dir)

        if not output_dir.startswith('code_interpreter'):
            output_path = os.path.join(workspace_path, 'code_interpreter', output_dir)
        else:
            output_path = os.path.join(workspace_path, output_dir)

        # Mount volumes with explicit Windows path handling
        input_path = input_path.replace('\\', '/')
        output_path = output_path.replace('\\', '/')
        workspace_dir = os.path.join(workspace_path, 'code_interpreter', 'workspace').replace('\\', '/')

        # Create directories if they don't exist
        os.makedirs(workspace_dir, exist_ok=True)
        os.makedirs(input_path, exist_ok=True)
        os.makedirs(output_path, exist_ok=True)

        self.logger.debug(f"Workspace path: {workspace_dir}")
        self.logger.debug(f"Input path: {input_path}")
        self.logger.debug(f"Output path: {output_path}")
        self.logger.debug(f"Directories exist: workspace={os.path.exists(workspace_dir)}, "
                          f"input={os.path.exists(input_path)}, "
                          f"output={os.path.exists(output_path)}")

        volumes = {
            input_path: {"bind": "/input", "mode": "ro"},
            output_path: {"bind": "/output", "mode": "rw"},
            workspace_dir: {"bind": "/workspace", "mode": "rw"}
        }

        return client.containers.run(
            self.default_image_tag,
            detach=True,
            tty=True,
            working_dir="/workspace",
            name=container_name,
            volumes=volumes,
        )

    async def run_code_in_docker(
            self,
            code: str,
            libraries: List[str],
            input_files: Optional[List[str]] = None,
            workspace_name: str = 'project',
            delete_source: bool = False
    ) -> Dict[str, Any]:
        """
        Execute Python code inside a Docker container with file support.

        Args:
            code: Python code to execute
            libraries: Required Python libraries
            input_files: Optional list of input files from workspace
            workspace_name: Name of workspace to use
            delete_source: Whether to delete the source code file after execution

        Returns:
            Dictionary containing execution results and file paths
        """
        workspace = self.workspace_tool.find_workspace_by_name(workspace_name)
        if workspace is None:
            raise ValueError(f"Workspace {workspace_name} not found")

        # Prepare directories
        input_dir, output_dir = await self._prepare_workspace_dirs(workspace_name)
        workspace_path = workspace.full_path('.')

        # Copy input files if provided
        input_file_names = []
        if input_files:
            input_file_names = await self._copy_input_files(input_files, workspace_name, input_dir)

        # Prepare container
        self._verify_docker_image()
        container = self._init_docker_container(workspace_path, input_dir, output_dir)
        self._install_libraries(container, libraries)

        # prepare incoming code for proper indentation
        indented_code = '\n'.join('    ' + line for line in code.splitlines())

        # Wrap code to handle input/output paths
        wrapped_code = f"""
import os
import sys

# Set up input/output paths
INPUT_DIR = "/input"
OUTPUT_DIR = "/output"

# Add input files info
input_files = {input_file_names}

# Redirect stdout to capture print statements
from io import StringIO
stdout = StringIO()
sys.stdout = stdout

try:
    # Execute user code
{indented_code}
except Exception as e:
    print(f"Error executing code: {{str(e)}}", file=sys.stderr)

# Restore stdout and get output
sys.stdout = sys.__stdout__
output = stdout.getvalue()
print(output)  # Print to actual stdout for container capture
"""
        # setup workspace to save python code
        workspace_dir = os.path.join(workspace_path, 'code_interpreter', 'workspace')
        file_suffix = datetime.now().strftime("%Y%m%d_%H%M%S")
        script_path = os.path.join(workspace_dir, f'script_{file_suffix}.py')
        try:
            with open(script_path, 'w', encoding='utf-8') as f:
                f.write(wrapped_code)  # Write the raw wrapped_code, no need for escaping
        except Exception as e:
            self.logger.error(f"Failed to write script file: {e}")
            raise

        # Add some debugging
        self.logger.debug(f"Script written to: {script_path} and can be found: {os.path.exists(script_path)}")
        ls_input = container.exec_run("ls -la /input")
        ls_output = container.exec_run("ls -la /output")
        self.logger.debug("Container directory contents:")
        self.logger.debug(f"Input directory (/input):\n{ls_input.output.decode('utf-8')}")
        self.logger.debug(f"Output directory (/output):\n{ls_output.output.decode('utf-8')}")
        self.logger.debug(f"Container ID: {container.id}")
        self.logger.debug(f"Container status: {container.status}")
        workspace_check = container.exec_run("ls -la /workspace")
        self.logger.debug(f"Workspace directory (/workspace):\n{workspace_check.output.decode('utf-8')}")

        # Execute the code from the file
        self.logger.debug("About to execute script in container...")
        try:
            result = container.exec_run(f"python3 /workspace/script_{file_suffix}.py")
            self.logger.debug(f"Execution complete, exit code: {result.exit_code}")
        except Exception as e:
            self.logger.error(f"Exception during exec_run: {str(e)}")
            raise


        # Get list of generated files
        try:
            output_files_result = container.exec_run("ls -1 /output")
            self.logger.debug(f"Output files retrieval exit code: {output_files_result.exit_code}")
            output_files = output_files_result.output.decode('utf-8').splitlines()
            self.logger.debug(f"Output files found: {output_files}")
        except Exception as e:
            self.logger.error(f"Error retrieving output files: {str(e)}")

        # Cleanup
        try:
            self.logger.debug("Stopping container...")
            container.stop()
            self.logger.debug("Container stopped, removing...")
            container.remove()
            self.logger.debug("Container removed")
        except Exception as e:
            self.logger.error(f"Error stopping/removing container: {str(e)}")
        # Clean up the script file
        if delete_source:
            try:
                os.remove(script_path)
            except Exception as e:
                self.logger.warning(f"Failed to remove script file: {e}")

        # Prepare response
        response = {
            "success": result.exit_code == 0,
            "output": result.output.decode('utf-8'),
            "input_files": input_file_names,
            "output_files": output_files,
            "output_dir": output_dir
        }

        if result.exit_code != 0:
            response["error"] = f"Code execution failed with exit code {result.exit_code}"

        return response

    def run_code_unsafe(
            self,
            code: str,
            libraries: List[str],
            input_files: Optional[List[str]] = None,
            workspace_name: str = 'project'
    ) -> Dict[str, Any]:
        """
        Execute code directly on host (unsafe mode).

        Args:
            code: Python code to execute
            libraries: Required Python libraries
            input_files: Optional list of input files
            workspace_name: Name of workspace to use

        Returns:
            Dictionary containing execution results
        """
        # Install libraries
        for library in libraries:
            os.system(f"pip install {library}")

        try:
            # Create temporary directory for outputs
            with tempfile.TemporaryDirectory() as temp_dir:
                # Execute code in isolated namespace
                exec_locals = {
                    'OUTPUT_DIR': temp_dir,
                    'INPUT_FILES': input_files or []
                }
                exec(code, {}, exec_locals)

                # Get generated files
                output_files = os.listdir(temp_dir)

                return {
                    "success": True,
                    "output": str(exec_locals.get("result", "No result variable found.")),
                    "input_files": input_files or [],
                    "output_files": output_files
                }
        except Exception as e:
            return {
                "success": False,
                "error": f"Error executing code: {str(e)}"
            }

    @json_schema(
        description="Execute Python code safely in a Docker container with library and file I/O support",
        params={
            'code': {
                'type': 'string',
                'description': 'Python code to execute. Always include print statements for output.',
                'required': True
            },
            'libraries': {
                'type': 'array',
                'items': {'type': 'string'},
                'description': 'List of Python libraries required by the code',
                'required': True
            },
            'input_files': {
                'type': 'array',
                'items': {'type': 'string'},
                'description': 'List of files from the workspace to make available to the code',
                'required': False
            },
            'workspace_name': {
                'type': 'string',
                'description': 'Name of workspace to use for file operations',
                'required': False,
                'default': 'project'
            }
        }
    )
    async def execute_code(self, **kwargs) -> str:
        """
        Main tool function for executing Python code with file support.

        Args:
            code: Python code to execute
            libraries: List of required libraries
            input_files: Optional list of input files
            workspace_name: Optional workspace name to use

        Returns:
            JSON string with execution results and file information
        """
        try:
            code = kwargs.get('code')
            libraries = kwargs.get('libraries', [])
            input_files = kwargs.get('input_files', [])
            workspace_name = kwargs.get('workspace_name', 'project')

            if not code:
                return json.dumps({
                    "error": "No code provided for execution"
                })

            if self.unsafe_mode:
                result = self.run_code_unsafe(code, libraries, input_files, workspace_name)
            else:
                result = await self.run_code_in_docker(code, libraries, input_files, workspace_name)

            return json.dumps(result)

        except Exception as e:
            self.logger.error(f"Code execution failed: {str(e)}")
            return json.dumps({
                "error": f"Code execution failed: {str(e)}"
            })


# Register the tool
Toolset.register(CodeInterpreterTools, required_tools=['WorkspaceTools'])
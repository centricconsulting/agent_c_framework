import os
import sys
import platform
import logging
import subprocess
import shutil
import tempfile
from typing import Optional, Tuple
from pathlib import Path


class BinaryHelper:
    """
    Helper class for managing the GitHub MCP binary.
    """
    
    # Binary names for different platforms
    BINARY_NAMES = {
        "Windows": "github-mcp-server.exe",
        "Linux": "github-mcp-server",
        "Darwin": "github-mcp-server"
    }
    
    def __init__(self, binary_path: Optional[str] = None):
        """
        Initialize the binary helper.
        
        Args:
            binary_path: Optional path to the GitHub MCP binary. If not provided,
                         the helper will attempt to find or download the binary.
        """
        self.binary_path = binary_path
        self.system = platform.system()
        self.temp_dir: Optional[str] = None
    
    def get_binary_path(self) -> Tuple[bool, str]:
        """
        Get the path to the GitHub MCP binary.
        
        Returns:
            Tuple of (success, path_or_error_message)
        """
        # If binary path was explicitly provided, use it
        if self.binary_path:
            if os.path.isfile(self.binary_path) and os.access(self.binary_path, os.X_OK):
                return True, self.binary_path
            return False, f"Provided binary path is not executable: {self.binary_path}"
        
        # Try to find the binary in PATH
        binary_name = self.BINARY_NAMES.get(self.system)
        if not binary_name:
            return False, f"Unsupported platform: {self.system}"
        
        path_binary = shutil.which(binary_name)
        if path_binary:
            return True, path_binary
        
        # Check for environment variable pointing to the binary
        env_path = os.environ.get("GITHUB_MCP_BINARY")
        if env_path and os.path.isfile(env_path) and os.access(env_path, os.X_OK):
            return True, env_path
        
        return False, "GitHub MCP binary not found. Please install it and ensure it's in your PATH, " \
                     "or set the GITHUB_MCP_BINARY environment variable."
    
    def check_binary_version(self, binary_path: str) -> Tuple[bool, str]:
        """
        Check the version of the GitHub MCP binary.
        
        Args:
            binary_path: Path to the GitHub MCP binary.
            
        Returns:
            Tuple of (success, version_or_error_message)
        """
        try:
            result = subprocess.run(
                [binary_path, "--version"],
                capture_output=True,
                text=True,
                check=True
            )
            return True, result.stdout.strip()
        except subprocess.SubprocessError as e:
            return False, f"Failed to check binary version: {str(e)}"
        except Exception as e:
            return False, f"Unexpected error checking binary version: {str(e)}"

    def cleanup(self):
        """
        Clean up any temporary files or directories created by the helper.
        """
        if self.temp_dir and os.path.exists(self.temp_dir):
            try:
                shutil.rmtree(self.temp_dir)
                self.temp_dir = None
            except Exception as e:
                logging.warning(f"Failed to clean up temporary directory: {str(e)}")
"""Command Line Execution Tool for Agent C.

This module provides a tool for executing shell commands with user approval.
"""

import json
from typing import Dict, Any, Optional
from .command_line_exec import execute_command

class CommandLineExecutionTool:
    """Tool for executing shell commands with user approval."""

    name = "command_exec"
    description = "Execute shell commands with user approval"
    
    schema = {
        "type": "object",
        "properties": {
            "command": {
                "type": "string",
                "description": "The shell command to execute"
            },
            "working_directory": {
                "type": "string",
                "description": "Optional directory to execute the command in"
            },
            "environment_vars": {
                "type": "object",
                "description": "Optional environment variables as key-value pairs"
            },
            "timeout": {
                "type": "integer",
                "description": "Maximum execution time in seconds (default: 60)"
            },
            "stream_output": {
                "type": "boolean",
                "description": "Whether to stream command output in real-time"
            }
        },
        "required": ["command"]
    }
    
    def __call__(self, command: str, working_directory: Optional[str] = None, 
                 environment_vars: Optional[Dict[str, str]] = None, 
                 timeout: Optional[int] = 60,
                 stream_output: bool = False) -> Dict[str, Any]:
        """Execute a shell command with user approval.
        
        Args:
            command: The shell command to execute.
            working_directory: Optional directory to execute the command in.
            environment_vars: Optional environment variables to set.
            timeout: Maximum execution time in seconds (default: 60).
            stream_output: Whether to stream command output in real-time.
            
        Returns:
            Dict containing command output, status, and metadata.
        """
        try:
            result = execute_command(
                command=command,
                working_directory=working_directory,
                environment_vars=environment_vars,
                timeout=timeout,
                stream_output=stream_output
            )
            return result
        except Exception as e:
            return {
                "command": command,
                "approved": False,
                "executed": False,
                "success": False,
                "stdout": "",
                "stderr": "",
                "exit_code": None,
                "error": str(e)
            }
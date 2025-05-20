"""Command Line Execution Tool with Approval Mechanism.

This tool allows agents to execute shell commands with explicit user approval.
The tool presents the command to the user, waits for confirmation, and only
executes approved commands.

Security note: This tool executes commands with the same permissions as the
user running the Agent C framework. Use with caution.
"""

import subprocess
import shlex
import sys
import os
from typing import Dict, Any, Optional, List, Union

def execute_command(
    command: str,
    working_directory: Optional[str] = None,
    environment_vars: Optional[Dict[str, str]] = None,
    timeout: Optional[int] = 60,
    stream_output: bool = False
) -> Dict[str, Any]:
    """
    Execute a shell command with explicit user approval.
    
    Args:
        command: The shell command to execute.
        working_directory: Optional directory to execute the command in.
        environment_vars: Optional environment variables to set.
        timeout: Maximum execution time in seconds (default: 60).
        stream_output: Whether to stream command output in real-time.
        
    Returns:
        Dict containing command output, status, and metadata.
    """
    # Prepare response object
    response = {
        "command": command,
        "approved": False,
        "executed": False,
        "success": False,
        "stdout": "",
        "stderr": "",
        "exit_code": None,
        "error": None
    }
    
    # Present command for approval
    print(f"\n[COMMAND APPROVAL REQUIRED]\nCommand: {command}")
    if working_directory:
        print(f"Working Directory: {working_directory}")
    
    # Get user approval
    approval = input("\nDo you approve execution of this command? (yes/no): ").strip().lower()
    response["approved"] = approval in ["yes", "y"]
    
    if not response["approved"]:
        response["error"] = "Command execution denied by user"
        return response
    
    # Set up environment
    cmd_env = os.environ.copy()
    if environment_vars:
        cmd_env.update(environment_vars)
    
    # Prepare for execution
    try:
        # Use shell=False for security, so we need to parse the command
        args = shlex.split(command)
        
        print("\n[EXECUTING COMMAND]\n")
        response["executed"] = True
        
        process = subprocess.Popen(
            args,
            stdout=subprocess.PIPE,
            stderr=subprocess.PIPE,
            text=True,
            cwd=working_directory,
            env=cmd_env
        )
        
        # Handle streaming output if requested
        if stream_output:
            stdout_chunks = []
            stderr_chunks = []
            
            # Stream output while process is running
            for line in iter(process.stdout.readline, ''):
                if not line:
                    break
                stdout_chunks.append(line)
                print(line, end='')
            
            # Collect any stderr output
            for line in iter(process.stderr.readline, ''):
                if not line:
                    break
                stderr_chunks.append(line)
                print(f"[ERROR] {line}", end='')
            
            # Wait for process to complete
            exit_code = process.wait(timeout=timeout)
            stdout = ''.join(stdout_chunks)
            stderr = ''.join(stderr_chunks)
        else:
            # Simple execution with timeout
            try:
                stdout, stderr = process.communicate(timeout=timeout)
                exit_code = process.returncode
            except subprocess.TimeoutExpired:
                process.kill()
                stdout, stderr = process.communicate()
                response["error"] = f"Command execution timed out after {timeout} seconds"
                exit_code = -1
        
        # Record results
        response["stdout"] = stdout
        response["stderr"] = stderr
        response["exit_code"] = exit_code
        response["success"] = exit_code == 0
        
        # Print output for user visibility
        if not stream_output:
            if stdout:
                print(f"\n[STDOUT]\n{stdout}")
            if stderr:
                print(f"\n[STDERR]\n{stderr}")
            
        print(f"\n[COMMAND COMPLETED]\nExit Code: {exit_code}\n")
        
    except Exception as e:
        response["error"] = str(e)
        print(f"\n[COMMAND EXECUTION FAILED]\nError: {e}\n")
    
    return response
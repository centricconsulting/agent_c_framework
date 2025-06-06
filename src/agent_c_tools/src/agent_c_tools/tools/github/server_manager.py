import os
import sys
import json
import asyncio
import logging
import subprocess
from typing import Dict, List, Optional, Any, Tuple, Union

from agent_c_tools.tools.github.utils.binary_helper import BinaryHelper


class ServerManager:
    """
    Manages the GitHub MCP server subprocess.
    """
    
    def __init__(self, binary_path: Optional[str] = None, token: Optional[str] = None):
        """
        Initialize the server manager.
        
        Args:
            binary_path: Optional path to the GitHub MCP binary.
            token: GitHub personal access token. If not provided, will try to get from environment.
        """
        self.binary_helper = BinaryHelper(binary_path)
        self.token = token or os.environ.get("GITHUB_PERSONAL_ACCESS_TOKEN")
        self.process: Optional[subprocess.Popen] = None
        self.read_task: Optional[asyncio.Task] = None
        self.write_task: Optional[asyncio.Task] = None
        self.request_queue = asyncio.Queue()
        self.response_map: Dict[str, asyncio.Future] = {}
        self.next_id = 1
        self.host = os.environ.get("GITHUB_HOST", "")
    
    async def start(self, toolsets: List[str] = None, read_only: bool = False) -> Tuple[bool, str]:
        """
        Start the GitHub MCP server process.
        
        Args:
            toolsets: List of toolset names to enable.
            read_only: Whether to run in read-only mode.
            
        Returns:
            Tuple of (success, error_message_or_empty_string)
        """
        if self.process:
            return True, ""
        
        if not self.token:
            return False, "GitHub personal access token not provided. Set GITHUB_PERSONAL_ACCESS_TOKEN environment variable."
        
        success, binary_path_or_error = self.binary_helper.get_binary_path()
        if not success:
            return False, binary_path_or_error
        
        binary_path = binary_path_or_error
        
        # Check binary version
        success, version_or_error = self.binary_helper.check_binary_version(binary_path)
        if not success:
            return False, version_or_error
        
        logging.info(f"Using GitHub MCP binary: {binary_path} ({version_or_error})")
        
        # Prepare command and environment
        cmd = [binary_path, "stdio"]
        if toolsets:
            cmd.extend(["--toolsets", ",".join(toolsets)])
        if read_only:
            cmd.append("--read-only")
        if self.host:
            cmd.extend(["--gh-host", self.host])
        
        env = os.environ.copy()
        env["GITHUB_PERSONAL_ACCESS_TOKEN"] = self.token
        
        try:
            # Start the process
            self.process = subprocess.Popen(
                cmd,
                stdin=subprocess.PIPE,
                stdout=subprocess.PIPE,
                stderr=subprocess.PIPE,
                env=env,
                text=False
            )
            
            # Start background tasks for communication
            self.read_task = asyncio.create_task(self._read_responses())
            self.write_task = asyncio.create_task(self._write_requests())
            
            # Give the server a moment to start up
            await asyncio.sleep(0.5)
            
            # Check if process is still running
            if self.process.poll() is not None:
                stderr_output = await self._read_stderr()
                await self.stop()
                return False, f"Server process exited unexpectedly: {stderr_output}"
            
            return True, ""
        except Exception as e:
            await self.stop()
            return False, f"Failed to start server: {str(e)}"
    
    async def stop(self):
        """
        Stop the GitHub MCP server process and clean up resources.
        """
        # Cancel background tasks
        if self.read_task:
            self.read_task.cancel()
            self.read_task = None
        
        if self.write_task:
            self.write_task.cancel()
            self.write_task = None
        
        # Terminate process
        if self.process:
            try:
                self.process.terminate()
                try:
                    self.process.wait(timeout=2)
                except subprocess.TimeoutExpired:
                    self.process.kill()
            except Exception as e:
                logging.warning(f"Error stopping server process: {str(e)}")
            finally:
                self.process = None
        
        # Clean up binary helper resources
        self.binary_helper.cleanup()
        
        # Clear response map and reject any pending requests
        for future in self.response_map.values():
            if not future.done():
                future.set_exception(Exception("Server connection closed"))
        self.response_map.clear()
    
    async def call_method(self, method: str, params: Dict[str, Any] = None) -> Dict[str, Any]:
        """
        Call a method on the GitHub MCP server.
        
        Args:
            method: Method name to call.
            params: Method parameters.
            
        Returns:
            Method response.
            
        Raises:
            Exception: If the server is not running or the method call fails.
        """
        if not self.process or self.process.poll() is not None:
            raise Exception("Server is not running")
        
        request_id = str(self.next_id)
        self.next_id += 1
        
        # Create request message
        request = {
            "jsonrpc": "2.0",
            "id": request_id,
            "method": method,
            "params": params or {}
        }
        
        # Create future for response
        response_future = asyncio.get_running_loop().create_future()
        self.response_map[request_id] = response_future
        
        # Queue request
        await self.request_queue.put(request)
        
        # Wait for response
        try:
            response = await response_future
            if "error" in response:
                raise Exception(f"Method call failed: {response['error']['message']}")
            return response.get("result", {})
        except asyncio.CancelledError:
            del self.response_map[request_id]
            raise
        except Exception as e:
            del self.response_map[request_id]
            raise Exception(f"Error calling method {method}: {str(e)}") from e
    
    async def _read_responses(self):
        """
        Background task to read responses from the server process.
        """
        if not self.process or not self.process.stdout:
            return
        
        while True:
            try:
                # Read a line from stdout
                line = await asyncio.get_event_loop().run_in_executor(
                    None, self.process.stdout.readline
                )
                
                if not line:
                    # EOF reached, process likely terminated
                    logging.warning("Server process closed stdout")
                    break
                
                # Parse response
                try:
                    response = json.loads(line.decode('utf-8'))
                    request_id = str(response.get("id", ""))
                    
                    # Look up corresponding future
                    future = self.response_map.pop(request_id, None)
                    if future and not future.done():
                        future.set_result(response)
                    else:
                        logging.warning(f"Received response for unknown request ID: {request_id}")
                except json.JSONDecodeError as e:
                    logging.error(f"Failed to parse server response: {str(e)}")
                except Exception as e:
                    logging.error(f"Error processing server response: {str(e)}")
            except asyncio.CancelledError:
                break
            except Exception as e:
                logging.error(f"Error reading server response: {str(e)}")
                # Brief pause to avoid tight loop if there's an error
                await asyncio.sleep(0.1)
    
    async def _write_requests(self):
        """
        Background task to write requests to the server process.
        """
        if not self.process or not self.process.stdin:
            return
        
        while True:
            try:
                # Get next request from queue
                request = await self.request_queue.get()
                
                # Serialize and write request
                request_data = json.dumps(request).encode('utf-8') + b'\n'
                await asyncio.get_event_loop().run_in_executor(
                    None,
                    lambda: self.process.stdin.write(request_data) and self.process.stdin.flush()
                )
                
                self.request_queue.task_done()
            except asyncio.CancelledError:
                break
            except Exception as e:
                logging.error(f"Error writing request to server: {str(e)}")
                # Mark task as done to avoid hanging the queue
                self.request_queue.task_done()
                # Brief pause to avoid tight loop if there's an error
                await asyncio.sleep(0.1)
    
    async def _read_stderr(self) -> str:
        """
        Read all available data from the process stderr.
        
        Returns:
            Contents of stderr as a string.
        """
        if not self.process or not self.process.stderr:
            return ""
        
        stderr_data = await asyncio.get_event_loop().run_in_executor(
            None, self.process.stderr.read
        )
        return stderr_data.decode('utf-8', errors='replace')
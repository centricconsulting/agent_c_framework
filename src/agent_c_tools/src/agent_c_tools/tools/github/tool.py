import os
import json
import logging
from typing import Dict, List, Optional, Any, Tuple, Union
from agent_c_tools.tools.github.models import Repository, Issue, PullRequest

from agent_c.toolsets.tool_set import Toolset
from agent_c.toolsets.json_schema import json_schema

from agent_c_tools.tools.github.server_manager import ServerManager
from agent_c_tools.tools.github.client import GithubMCPClient
from agent_c_tools.tools.github.utils.auth import get_github_token
from agent_c_tools.tools.github.prompt import GithubSection


class GithubTools(Toolset):
    """
    GitHub Tools provides access to GitHub operations through the GitHub MCP server.
    
    This toolset allows interacting with GitHub repositories, issues, pull requests,
    and other GitHub features through a consistent interface.
    """
    
    def __init__(self, **kwargs):
        # Set the name for this toolset
        # kwargs['name'] = kwargs.get('name', 'GithubTools')
        super().__init__(**kwargs, name='GithubTools', needed_keys=['GITHUB_MCP_BINARY', 'GITHUB_PERSONAL_ACCESS_TOKEN'])

        if not self.tool_valid:
            logging.error("tool not valid")
            return


        
        # Get binary path from kwargs or environment
        # binary_path = kwargs.get('binary_path') or os.environ.get('GITHUB_MCP_BINARY')
        # # If not found, use the path to your executable
        # if not binary_path:
        #     binary_path = 'PATH_TO_YOUR_GITHUB_MCP_SERVER_EXE'  # Replace with your actual path
        #
        # # Get token from kwargs or environment
        # token = kwargs.get('token') or os.environ.get('GITHUB_PERSONAL_ACCESS_TOKEN')
        # if not token:
        #     self.logger.warning("GitHub token not provided. Set GITHUB_PERSONAL_ACCESS_TOKEN environment variable.")
        
        # Create server manager and client
        self.binary_path: str = kwargs.get('binary_path', os.getenv('GITHUB_MCP_BINARY'))
        logging.info(f"GitHub MCP Binary Path is : {self.binary_path}")
        self.token: str = kwargs.get('token', os.getenv('GITHUB_PERSONAL_ACCESS_TOKEN'))
        logging.info(f"GitHub MCP Token is : {self.token}")
        self.server_manager = ServerManager(binary_path=self.binary_path, token=self.token)
        self.client = GithubMCPClient(self.server_manager)
        
        # Set up prompt section
        self.section = GithubSection()
        
        # Initialize server status
        self.server_initialized = False
        self.server_error = ""
    
    async def post_init(self):
        """
        Initialize the GitHub MCP server after the toolset is fully initialized.
        """
        # Check if the token is available
        token = os.environ.get("GITHUB_PERSONAL_ACCESS_TOKEN")
        if not token:
            self.server_error = "GitHub personal access token not found. Set GITHUB_PERSONAL_ACCESS_TOKEN environment variable."
            logging.warning(f"GitHub MCP server initialization failed: {self.server_error}")
            return
        
        # Try to start the server
        success, error = await self.client.ensure_server_running()
        if not success:
            self.server_error = error
            logging.warning(f"GitHub MCP server initialization failed: {error}")
            return
        
        self.server_initialized = True
        logging.info("GitHub MCP server initialized successfully")
    
    def __del__(self):
        """
        Clean up resources when the toolset is destroyed.
        """
        # Ensure the server is stopped
        if hasattr(self, 'server_manager'):
            import asyncio
            try:
                loop = asyncio.get_event_loop()
                if loop.is_running():
                    loop.create_task(self.server_manager.stop())
                else:
                    loop.run_until_complete(self.server_manager.stop())
            except Exception as e:
                logging.warning(f"Error stopping GitHub MCP server: {str(e)}")
    
    async def _ensure_server(self) -> Tuple[bool, str]:
        """
        Ensure the server is running and handle errors.
        
        Returns:
            Tuple of (success, error_message_or_empty_string)
        """
        if self.server_initialized:
            return True, ""
        
        if self.server_error:
            return False, f"GitHub MCP server is not available: {self.server_error}"
        
        # Try to initialize the server if it wasn't initialized in post_init
        success, error = await self.client.ensure_server_running()
        if success:
            self.server_initialized = True
            return True, ""
        else:
            self.server_error = error
            return False, f"GitHub MCP server initialization failed: {error}"
    
    # Repository Operations
    
    @json_schema(
        description="Search for GitHub repositories matching a query",
        params={
            "query": {
                "type": "string",
                "description": "Search query (e.g. 'language:python stars:>1000')",
                "required": True
            },
            "page": {
                "type": "integer",
                "description": "Page number (1-based)",
                "required": False
            },
            "per_page": {
                "type": "integer",
                "description": "Number of results per page (max 100)",
                "required": False
            }
        }
    )
    async def search_repositories(self, **kwargs) -> str:
        """
        Search for GitHub repositories matching a query.
        
        Args:
            query: Search query (e.g. 'language:python stars:>1000').
            page: Page number (1-based).
            per_page: Number of results per page (max 100).
            
        Returns:
            JSON string containing search results.
        """
        # Get the client_wants_cancel event from tool_context
        tool_context = kwargs.get('tool_context', {})
        client_wants_cancel = tool_context.get('client_wants_cancel')
        
        # Extract other parameters
        query = kwargs.get('query')
        page = kwargs.get('page', 1)
        per_page = kwargs.get('per_page', 10)
        
        if not query:
            return json.dumps({"error": "query parameter is required"})
        
        # Check if cancellation is requested
        if client_wants_cancel and client_wants_cancel.is_set():
            return json.dumps({"error": "Operation cancelled by user"})
        
        # Ensure server is running
        success, error = await self._ensure_server()
        if not success:
            return json.dumps({"error": error})
        
        try:
            # Check again before making the API call
            if client_wants_cancel and client_wants_cancel.is_set():
                return json.dumps({"error": "Operation cancelled by user"})
                
            # Call the client method
            result = await self.client.search_repositories(query, page, per_page)
            return json.dumps(result)
        except Exception as e:
            return json.dumps({"error": f"Error searching repositories: {str(e)}"})
    
    @json_schema(
        description="Get the contents of a file from a GitHub repository",
        params={
            "owner": {
                "type": "string",
                "description": "Repository owner (username or organization)",
                "required": True
            },
            "repo": {
                "type": "string",
                "description": "Repository name",
                "required": True
            },
            "path": {
                "type": "string",
                "description": "File path within the repository",
                "required": True
            },
            "ref": {
                "type": "string",
                "description": "Git reference (branch, tag, commit)",
                "required": False
            }
        }
    )
    async def get_file_contents(self, **kwargs) -> str:
        """
        Get the contents of a file from a GitHub repository.
        
        Args:
            owner: Repository owner (username or organization).
            repo: Repository name.
            path: File path within the repository.
            ref: Git reference (branch, tag, commit).
            
        Returns:
            JSON string containing file contents and metadata.
        """
        # Get the client_wants_cancel event from tool_context
        tool_context = kwargs.get('tool_context', {})
        client_wants_cancel = tool_context.get('client_wants_cancel')
        
        # Extract other parameters
        owner = kwargs.get('owner')
        repo = kwargs.get('repo')
        path = kwargs.get('path')
        ref = kwargs.get('ref')
        
        if not owner or not repo or not path:
            return json.dumps({"error": "owner, repo, and path parameters are required"})
        
        # Check if cancellation is requested
        if client_wants_cancel and client_wants_cancel.is_set():
            return json.dumps({"error": "Operation cancelled by user"})
        
        # Ensure server is running
        success, error = await self._ensure_server()
        if not success:
            return json.dumps({"error": error})
        
        try:
            # Check again before making the API call
            if client_wants_cancel and client_wants_cancel.is_set():
                return json.dumps({"error": "Operation cancelled by user"})
                
            # Call the client method
            result = await self.client.get_file_contents(owner, repo, path, ref)
            return json.dumps(result)
        except Exception as e:
            return json.dumps({"error": f"Error getting file contents: {str(e)}"})
    
    @json_schema(
        description="List commits for a GitHub repository",
        params={
            "owner": {
                "type": "string",
                "description": "Repository owner (username or organization)",
                "required": True
            },
            "repo": {
                "type": "string",
                "description": "Repository name",
                "required": True
            },
            "page": {
                "type": "integer",
                "description": "Page number (1-based)",
                "required": False
            },
            "per_page": {
                "type": "integer",
                "description": "Number of results per page (max 100)",
                "required": False
            }
        }
    )
    async def list_commits(self, **kwargs) -> str:
        """
        List commits for a GitHub repository.
        
        Args:
            owner: Repository owner (username or organization).
            repo: Repository name.
            page: Page number (1-based).
            per_page: Number of results per page (max 100).
            
        Returns:
            JSON string containing list of commits.
        """
        # Get the client_wants_cancel event from tool_context
        tool_context = kwargs.get('tool_context', {})
        client_wants_cancel = tool_context.get('client_wants_cancel')
        
        # Extract other parameters
        owner = kwargs.get('owner')
        repo = kwargs.get('repo')
        page = kwargs.get('page', 1)
        per_page = kwargs.get('per_page', 10)
        
        if not owner or not repo:
            return json.dumps({"error": "owner and repo parameters are required"})
        
        # Check if cancellation is requested
        if client_wants_cancel and client_wants_cancel.is_set():
            return json.dumps({"error": "Operation cancelled by user"})
        
        # Ensure server is running
        success, error = await self._ensure_server()
        if not success:
            return json.dumps({"error": error})
        
        try:
            # Check again before making the API call
            if client_wants_cancel and client_wants_cancel.is_set():
                return json.dumps({"error": "Operation cancelled by user"})
                
            # Call the client method
            result = await self.client.list_commits(owner, repo, page, per_page)
            return json.dumps(result)
        except Exception as e:
            return json.dumps({"error": f"Error listing commits: {str(e)}"})
    
    @json_schema(
        description="Search for code within GitHub repositories",
        params={
            "query": {
                "type": "string",
                "description": "Search query (e.g. 'filename:.py language:python')",
                "required": True
            },
            "page": {
                "type": "integer",
                "description": "Page number (1-based)",
                "required": False
            },
            "per_page": {
                "type": "integer",
                "description": "Number of results per page (max 100)",
                "required": False
            }
        }
    )
    async def search_code(self, **kwargs) -> str:
        """
        Search for code within GitHub repositories.
        
        Args:
            query: Search query (e.g. 'filename:.py language:python').
            page: Page number (1-based).
            per_page: Number of results per page (max 100).
            
        Returns:
            JSON string containing search results.
        """
        # Get the client_wants_cancel event from tool_context
        tool_context = kwargs.get('tool_context', {})
        client_wants_cancel = tool_context.get('client_wants_cancel')
        
        # Extract other parameters
        query = kwargs.get('query')
        page = kwargs.get('page', 1)
        per_page = kwargs.get('per_page', 10)
        
        if not query:
            return json.dumps({"error": "query parameter is required"})
        
        # Check if cancellation is requested
        if client_wants_cancel and client_wants_cancel.is_set():
            return json.dumps({"error": "Operation cancelled by user"})
        
        # Ensure server is running
        success, error = await self._ensure_server()
        if not success:
            return json.dumps({"error": error})
        
        try:
            # Check again before making the API call
            if client_wants_cancel and client_wants_cancel.is_set():
                return json.dumps({"error": "Operation cancelled by user"})
                
            # Call the client method
            result = await self.client.search_code(query, page, per_page)
            return json.dumps(result)
        except Exception as e:
            return json.dumps({"error": f"Error searching code: {str(e)}"})
    
    # User Information
    
    @json_schema(
        description="Get information about the authenticated GitHub user",
        params={}
    )
    async def get_me(self, **kwargs) -> str:
        """
        Get information about the authenticated GitHub user.
        
        Returns:
            JSON string containing user information.
        """
        # Get the client_wants_cancel event from tool_context
        tool_context = kwargs.get('tool_context', {})
        client_wants_cancel = tool_context.get('client_wants_cancel')
        
        # Check if cancellation is requested
        if client_wants_cancel and client_wants_cancel.is_set():
            return json.dumps({"error": "Operation cancelled by user"})
        
        # Ensure server is running
        success, error = await self._ensure_server()
        if not success:
            return json.dumps({"error": error})
        
        try:
            # Check again before making the API call
            if client_wants_cancel and client_wants_cancel.is_set():
                return json.dumps({"error": "Operation cancelled by user"})
                
            # Call the client method
            result = await self.client.get_me()
            logging.info(f"get_me() returned: {result}")
            return json.dumps(result)
        except Exception as e:
            logging.error(f"Detailed error in get_me: {repr(e)}")
            logging.error(f"Error getting user information: {str(e)}")
            return json.dumps({"error": f"Error getting user information: {str(e)}"})
    
    # Additional methods would be implemented here for all GitHub MCP functionalities
    # For brevity, only a subset of methods is implemented in this example


# Register the toolset
Toolset.register(GithubTools)
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
        """
        Initialize the GitHub toolset.
        
        Args:
            **kwargs: Additional arguments to pass to the Toolset constructor.
        """
        super().__init__(**kwargs)
        
        # Get binary path from kwargs or environment
        binary_path = kwargs.get('binary_path') or os.environ.get('GITHUB_MCP_BINARY')
        
        # Get token from kwargs or environment
        token = kwargs.get('token') or os.environ.get('GITHUB_PERSONAL_ACCESS_TOKEN')
        
        # Create server manager and client
        self.server_manager = ServerManager(binary_path=binary_path, token=token)
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
    async def search_repositories(self, query: str, page: int = 1, per_page: int = 10) -> str:
        """
        Search for GitHub repositories matching a query.
        
        Args:
            query: Search query (e.g. 'language:python stars:>1000').
            page: Page number (1-based).
            per_page: Number of results per page (max 100).
            
        Returns:
            JSON string containing search results.
        """
        # Ensure server is running
        success, error = await self._ensure_server()
        if not success:
            return json.dumps({"error": error})
        
        try:
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
    async def get_file_contents(self, owner: str, repo: str, path: str, ref: str = None) -> str:
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
        # Ensure server is running
        success, error = await self._ensure_server()
        if not success:
            return json.dumps({"error": error})
        
        try:
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
    async def list_commits(self, owner: str, repo: str, page: int = 1, per_page: int = 10) -> str:
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
        # Ensure server is running
        success, error = await self._ensure_server()
        if not success:
            return json.dumps({"error": error})
        
        try:
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
    async def search_code(self, query: str, page: int = 1, per_page: int = 10) -> str:
        """
        Search for code within GitHub repositories.
        
        Args:
            query: Search query (e.g. 'filename:.py language:python').
            page: Page number (1-based).
            per_page: Number of results per page (max 100).
            
        Returns:
            JSON string containing search results.
        """
        # Ensure server is running
        success, error = await self._ensure_server()
        if not success:
            return json.dumps({"error": error})
        
        try:
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
    async def get_me(self) -> str:
        """
        Get information about the authenticated GitHub user.
        
        Returns:
            JSON string containing user information.
        """
        # Ensure server is running
        success, error = await self._ensure_server()
        if not success:
            return json.dumps({"error": error})
        
        try:
            # Call the client method
            result = await self.client.get_me()
            return json.dumps(result)
        except Exception as e:
            return json.dumps({"error": f"Error getting user information: {str(e)}"})
    
    # Additional methods would be implemented here for all GitHub MCP functionalities
    # For brevity, only a subset of methods is implemented in this example


# Register the toolset
Toolset.register(GithubTools)
import logging
from typing import Dict, List, Any, Optional, Tuple

from agent_c_tools.tools.github.server_manager import ServerManager


class GithubMCPClient:
    """
    Client for communicating with the GitHub MCP server.
    """
    
    def __init__(self, server_manager: ServerManager):
        """
        Initialize the GitHub MCP client.
        
        Args:
            server_manager: Server manager instance.
        """
        self.server_manager = server_manager
    
    async def ensure_server_running(self, toolsets: List[str] = None, read_only: bool = False) -> Tuple[bool, str]:
        """
        Ensure the server is running with the specified configuration.
        
        Args:
            toolsets: List of toolset names to enable.
            read_only: Whether to run in read-only mode.
            
        Returns:
            Tuple of (success, error_message_or_empty_string)
        """
        return await self.server_manager.start(toolsets, read_only)
    
    async def call(self, method: str, params: Dict[str, Any] = None) -> Dict[str, Any]:
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
        return await self.server_manager.call_method(method, params)
    
    # Repository Operations
    
    async def search_repositories(self, query: str, page: int = 1, per_page: int = 10) -> Dict[str, Any]:
        """
        Search for repositories matching a query.
        
        Args:
            query: Search query.
            page: Page number (1-based).
            per_page: Number of results per page.
            
        Returns:
            Search results.
        """
        params = {
            "query": query,
            "page": page,
            "per_page": per_page
        }
        return await self.call("SearchRepositories", params)
    
    async def get_file_contents(self, owner: str, repo: str, path: str, ref: str = None) -> Dict[str, Any]:
        """
        Get the contents of a file from a repository.
        
        Args:
            owner: Repository owner.
            repo: Repository name.
            path: File path within the repository.
            ref: Git reference (branch, tag, commit).
            
        Returns:
            File contents and metadata.
        """
        params = {
            "owner": owner,
            "repo": repo,
            "path": path
        }
        if ref:
            params["ref"] = ref
        return await self.call("GetFileContents", params)
    
    async def list_commits(self, owner: str, repo: str, page: int = 1, per_page: int = 10) -> Dict[str, Any]:
        """
        List commits for a repository.
        
        Args:
            owner: Repository owner.
            repo: Repository name.
            page: Page number (1-based).
            per_page: Number of results per page.
            
        Returns:
            List of commits.
        """
        params = {
            "owner": owner,
            "repo": repo,
            "page": page,
            "per_page": per_page
        }
        return await self.call("ListCommits", params)
    
    async def search_code(self, query: str, page: int = 1, per_page: int = 10) -> Dict[str, Any]:
        """
        Search for code within repositories.
        
        Args:
            query: Search query.
            page: Page number (1-based).
            per_page: Number of results per page.
            
        Returns:
            Search results.
        """
        params = {
            "query": query,
            "page": page,
            "per_page": per_page
        }
        return await self.call("SearchCode", params)
    
    # Add more client methods as needed to match the GitHub MCP server API
    # For brevity, not all methods are implemented here
    
    # User Information
    
    async def get_me(self) -> Dict[str, Any]:
        """
        Get information about the authenticated user.
        
        Returns:
            User information.
        """
        return await self.call("GetMe", {})
    
    async def search_users(self, query: str, page: int = 1, per_page: int = 10) -> Dict[str, Any]:
        """
        Search for users matching a query.
        
        Args:
            query: Search query.
            page: Page number (1-based).
            per_page: Number of results per page.
            
        Returns:
            Search results.
        """
        params = {
            "query": query,
            "page": page,
            "per_page": per_page
        }
        return await self.call("SearchUsers", params)
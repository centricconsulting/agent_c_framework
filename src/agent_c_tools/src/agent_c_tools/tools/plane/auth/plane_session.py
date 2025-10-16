"""
PLANE Session Manager

Manages HTTP sessions with PLANE API using cookie authentication.
"""

import aiohttp
from typing import Optional, Dict, Any, Tuple
from .cookie_manager import PlaneCookieManager


class PlaneSession:
    """Async HTTP session manager for PLANE API with cookie-based auth"""
    
    def __init__(
        self,
        base_url: str,
        workspace_slug: str,
        cookies: Optional[Dict[str, str]] = None
    ):
        """
        Initialize PLANE session
        
        Args:
            base_url: PLANE instance URL
            workspace_slug: Workspace identifier
            cookies: Optional cookie dict
        """
        self.base_url = base_url.rstrip('/')
        self.workspace_slug = workspace_slug
        self.cookie_manager = PlaneCookieManager(workspace_slug)
        
        # Load or set cookies
        if cookies:
            self.cookies = cookies
        else:
            saved_cookies = self.cookie_manager.load_cookies()
            if saved_cookies:
                self.cookies = saved_cookies
            else:
                raise ValueError(
                    f"No cookies provided and no saved cookies found for workspace '{workspace_slug}'. "
                    "Please provide cookies or run cookie setup."
                )
        
        # Validate cookies
        if not self.cookie_manager.validate_cookies(self.cookies):
            raise ValueError(
                f"Invalid cookies: missing required cookies. "
                f"Required: {', '.join(self.cookie_manager.required_cookies)}"
            )
        
        # Default headers
        self.default_headers = {
            'Accept': 'application/json',
            'Content-Type': 'application/json',
            'Referer': f'{self.base_url}/',
        }
    
    async def test_connection(self) -> bool:
        """Test if current cookies work"""
        try:
            status, _, _, _ = await self.request('GET', '/api/users/me/')
            return status == 200
        except Exception:
            return False
    
    async def request(
        self,
        method: str,
        endpoint: str,
        **kwargs
    ) -> Tuple[int, str, bytes, Dict[str, str]]:
        """
        Make async HTTP request to PLANE API
        
        Args:
            method: HTTP method
            endpoint: API endpoint
            **kwargs: Additional arguments for aiohttp
            
        Returns:
            Tuple of (status, content_type, body_bytes, headers)
            
        Raises:
            PlaneSessionExpired: If cookies expired
        """
        # Build full URL
        if endpoint.startswith('http'):
            url = endpoint
        else:
            url = f"{self.base_url}{endpoint}"
        
        # Merge headers
        headers = {**self.default_headers, **kwargs.pop('headers', {})}
        
        # Set default timeout
        if 'timeout' not in kwargs:
            kwargs['timeout'] = aiohttp.ClientTimeout(total=30)
        
        # Make request with cookies
        async with aiohttp.ClientSession(cookies=self.cookies) as session:
            async with session.request(method, url, headers=headers, **kwargs) as response:
                # Check for auth errors
                if response.status == 401:
                    raise PlaneSessionExpired(
                        "PLANE session has expired. Please refresh your cookies.\n\n"
                        "To refresh cookies:\n"
                        "1. Open PLANE in your browser\n"
                        "2. Press F12 → Application → Cookies → http://localhost\n"
                        "3. Copy these cookie values:\n"
                        "   - session-id\n"
                        "   - agentc-auth-token\n"
                        "   - csrftoken\n"
                        "4. Run: python plane_auth_cli.py setup agent_c"
                    )
                
                # Read response content BEFORE exiting context
                body = await response.read()
                return (response.status, response.content_type or '', body, dict(response.headers))
    
    async def get(self, endpoint: str, **kwargs) -> Tuple[int, str, bytes, Dict[str, str]]:
        """GET request"""
        return await self.request('GET', endpoint, **kwargs)
    
    async def post(self, endpoint: str, **kwargs) -> Tuple[int, str, bytes, Dict[str, str]]:
        """POST request"""
        return await self.request('POST', endpoint, **kwargs)
    
    async def patch(self, endpoint: str, **kwargs) -> Tuple[int, str, bytes, Dict[str, str]]:
        """PATCH request"""
        return await self.request('PATCH', endpoint, **kwargs)
    
    async def delete(self, endpoint: str, **kwargs) -> Tuple[int, str, bytes, Dict[str, str]]:
        """DELETE request"""
        return await self.request('DELETE', endpoint, **kwargs)


class PlaneSessionExpired(Exception):
    """Raised when PLANE session cookies have expired"""
    pass


class PlaneAPIError(Exception):
    """Raised for PLANE API errors"""
    pass

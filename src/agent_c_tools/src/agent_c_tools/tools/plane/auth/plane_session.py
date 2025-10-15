"""
PLANE Session Manager

Manages HTTP sessions with PLANE API using cookie authentication.
Automatically validates cookies and provides helpful error messages when expired.
"""

import requests
from typing import Optional, Dict, Any
from .cookie_manager import PlaneCookieManager
from .cookie_refresh import PlaneCookieRefresh


class PlaneSession:
    """HTTP session manager for PLANE API with cookie-based auth"""
    
    def __init__(
        self,
        base_url: str,
        workspace_slug: str,
        cookies: Optional[Dict[str, str]] = None,
        auto_refresh: bool = False
    ):
        """
        Initialize PLANE session
        
        Args:
            base_url: PLANE instance URL (e.g., http://localhost)
            workspace_slug: Workspace identifier
            cookies: Optional cookie dict. If not provided, will try to load from disk.
            auto_refresh: Enable automatic cookie refresh on expiration (requires Playwright)
        """
        self.base_url = base_url.rstrip('/')
        self.workspace_slug = workspace_slug
        self.cookie_manager = PlaneCookieManager(workspace_slug)
        self.auto_refresh = auto_refresh
        self.cookie_refresh = PlaneCookieRefresh(base_url, workspace_slug) if auto_refresh else None
        
        # Create requests session
        self.session = requests.Session()
        
        # Load or set cookies
        if cookies:
            self._set_cookies(cookies)
        else:
            saved_cookies = self.cookie_manager.load_cookies()
            if saved_cookies:
                self._set_cookies(saved_cookies)
            else:
                raise ValueError(
                    f"No cookies provided and no saved cookies found for workspace '{workspace_slug}'. "
                    "Please provide cookies or run cookie setup."
                )
        
        # Set default headers
        self.session.headers.update({
            'Accept': 'application/json',
            'Content-Type': 'application/json',
            'Referer': f'{self.base_url}/',
        })
    
    def _set_cookies(self, cookies: Dict[str, str]):
        """Set cookies in session"""
        if not self.cookie_manager.validate_cookies(cookies):
            raise ValueError(
                f"Invalid cookies: missing required cookies. "
                f"Required: {', '.join(self.cookie_manager.required_cookies)}"
            )
        
        for name, value in cookies.items():
            self.session.cookies.set(name, value)
    
    def test_connection(self) -> bool:
        """
        Test if current cookies work
        
        Returns:
            True if cookies are valid, False otherwise
        """
        try:
            response = self.session.get(
                f"{self.base_url}/api/users/me/",
                timeout=10
            )
            return response.status_code == 200
        except:
            return False
    
    def request(
        self,
        method: str,
        endpoint: str,
        **kwargs
    ) -> requests.Response:
        """
        Make HTTP request to PLANE API
        
        Args:
            method: HTTP method (GET, POST, PATCH, DELETE)
            endpoint: API endpoint (e.g., '/api/workspaces/agent_c/projects/')
            **kwargs: Additional arguments for requests
            
        Returns:
            Response object
            
        Raises:
            PlaneSessionExpired: If cookies have expired (401 response)
            PlaneAPIError: For other API errors
        """
        # Build full URL
        if endpoint.startswith('http'):
            url = endpoint
        else:
            url = f"{self.base_url}{endpoint}"
        
        # Make request
        response = self.session.request(method, url, **kwargs)
        
        # Check for authentication errors
        if response.status_code == 401:
            # Try auto-refresh if enabled
            if self.auto_refresh and self.cookie_refresh:
                try:
                    # Attempt to refresh cookies
                    new_cookies = await self._attempt_refresh()
                    if new_cookies:
                        # Retry the request with new cookies
                        response = self.session.request(method, url, **kwargs)
                        if response.status_code != 401:
                            return response  # Success!
                except Exception as e:
                    # Refresh failed, raise original error
                    pass
            
            raise PlaneSessionExpired(
                "PLANE session has expired. Please refresh your cookies.\n\n"
                "To refresh cookies:\n"
                "1. Open PLANE in your browser\n"
                "2. Press F12 → Application → Cookies → http://localhost\n"
                "3. Copy these cookie values:\n"
                "   - session-id\n"
                "   - agentc-auth-token\n"
                "   - csrftoken\n"
                "4. Update cookies using the cookie manager"
            )
        
        return response
    
    def get(self, endpoint: str, **kwargs) -> requests.Response:
        """GET request"""
        return self.request('GET', endpoint, **kwargs)
    
    def post(self, endpoint: str, **kwargs) -> requests.Response:
        """POST request"""
        return self.request('POST', endpoint, **kwargs)
    
    def patch(self, endpoint: str, **kwargs) -> requests.Response:
        """PATCH request"""
        return self.request('PATCH', endpoint, **kwargs)
    
    def delete(self, endpoint: str, **kwargs) -> requests.Response:
        """DELETE request"""
        return self.request('DELETE', endpoint, **kwargs)
    
    async def _attempt_refresh(self) -> Optional[Dict[str, str]]:
        """Attempt to refresh cookies using browser automation"""
        if not self.cookie_refresh:
            return None
        
        try:
            # Try to refresh from existing browser session first
            new_cookies = await self.cookie_refresh.refresh_from_existing_session()
            
            if new_cookies:
                # Save new cookies
                self.cookie_manager.save_cookies(new_cookies)
                # Update session
                self._set_cookies(new_cookies)
                return new_cookies
            
            return None
            
        except Exception as e:
            return None


class PlaneSessionExpired(Exception):
    """Raised when PLANE session cookies have expired"""
    pass


class PlaneAPIError(Exception):
    """Raised for PLANE API errors"""
    pass

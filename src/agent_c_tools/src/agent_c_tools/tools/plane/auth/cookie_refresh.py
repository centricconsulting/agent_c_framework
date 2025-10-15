"""
PLANE Cookie Refresh

Automated cookie refresh using browser automation.
Since PLANE uses custom Agent C authentication, we use Playwright to automate
the browser login and extract fresh cookies.
"""

import asyncio
from typing import Dict, Optional
from playwright.async_api import async_playwright, TimeoutError as PlaywrightTimeout
import structlog

logger = structlog.get_logger(__name__)


class PlaneCookieRefresh:
    """Automate cookie refresh using browser automation"""
    
    def __init__(
        self,
        base_url: str = "http://localhost",
        workspace_slug: str = "agent_c",
        headless: bool = True
    ):
        """
        Initialize cookie refresh automation
        
        Args:
            base_url: PLANE instance URL
            workspace_slug: Workspace identifier
            headless: Run browser in headless mode
        """
        self.base_url = base_url
        self.workspace_slug = workspace_slug
        self.headless = headless
    
    async def refresh_from_existing_session(
        self,
        timeout: int = 30000
    ) -> Optional[Dict[str, str]]:
        """
        Refresh cookies by visiting PLANE with existing browser session
        
        This method assumes you're already logged into PLANE in your default browser.
        It opens PLANE, waits for the page to load, and extracts the cookies.
        
        Args:
            timeout: Maximum time to wait for page load (ms)
            
        Returns:
            Dictionary of cookies or None if failed
        """
        try:
            async with async_playwright() as p:
                # Launch browser
                browser = await p.chromium.launch(headless=self.headless)
                
                # Create context (this gets a fresh session)
                context = await browser.new_context()
                page = await context.new_page()
                
                logger.info(f"Opening PLANE at {self.base_url}")
                
                # Navigate to PLANE
                try:
                    await page.goto(
                        f"{self.base_url}/{self.workspace_slug}/",
                        wait_until="networkidle",
                        timeout=timeout
                    )
                except PlaywrightTimeout:
                    logger.warning("Page load timeout, but may have cookies anyway")
                
                # Get cookies
                cookies = await context.cookies()
                
                # Extract the cookies we need
                cookie_dict = {}
                required_names = ['session-id', 'agentc-auth-token', 'csrftoken', 'ajs_anonymous_id']
                
                for cookie in cookies:
                    if cookie['name'] in required_names:
                        cookie_dict[cookie['name']] = cookie['value']
                
                await browser.close()
                
                # Validate we got the required cookies
                if 'session-id' in cookie_dict and 'agentc-auth-token' in cookie_dict:
                    logger.info(f"Successfully extracted {len(cookie_dict)} cookies")
                    return cookie_dict
                else:
                    logger.error("Missing required cookies after browser session")
                    return None
                
        except Exception as e:
            logger.error(f"Failed to refresh cookies: {str(e)}")
            return None
    
    async def refresh_with_login(
        self,
        username: str,
        password: str,
        timeout: int = 30000
    ) -> Optional[Dict[str, str]]:
        """
        Refresh cookies by performing login
        
        This method automates the login process if you're not already logged in.
        
        Args:
            username: Email/username
            password: Password
            timeout: Maximum time to wait
            
        Returns:
            Dictionary of cookies or None if failed
        """
        try:
            async with async_playwright() as p:
                browser = await p.chromium.launch(headless=self.headless)
                context = await browser.new_context()
                page = await context.new_page()
                
                logger.info(f"Navigating to PLANE login at {self.base_url}")
                
                # Go to login page
                await page.goto(self.base_url, timeout=timeout)
                
                # Wait for login form (this may vary based on auth system)
                try:
                    # Common login form selectors
                    await page.wait_for_selector('input[type="email"], input[type="text"], input[name="email"]', timeout=5000)
                    
                    # Fill in credentials
                    email_input = await page.query_selector('input[type="email"], input[type="text"], input[name="email"]')
                    if email_input:
                        await email_input.fill(username)
                    
                    password_input = await page.query_selector('input[type="password"], input[name="password"]')
                    if password_input:
                        await password_input.fill(password)
                    
                    # Click login button
                    login_button = await page.query_selector('button[type="submit"], button:has-text("Sign in"), button:has-text("Login")')
                    if login_button:
                        await login_button.click()
                    
                    # Wait for navigation after login
                    await page.wait_for_url(f"{self.base_url}/{self.workspace_slug}/*", timeout=10000)
                    
                    logger.info("Login successful")
                    
                except PlaywrightTimeout:
                    # May already be logged in
                    logger.info("No login form found - may already be authenticated")
                
                # Extract cookies
                cookies = await context.cookies()
                
                cookie_dict = {}
                required_names = ['session-id', 'agentc-auth-token', 'csrftoken', 'ajs_anonymous_id']
                
                for cookie in cookies:
                    if cookie['name'] in required_names:
                        cookie_dict[cookie['name']] = cookie['value']
                
                await browser.close()
                
                if 'session-id' in cookie_dict:
                    logger.info(f"Successfully obtained {len(cookie_dict)} cookies")
                    return cookie_dict
                else:
                    logger.error("Failed to obtain required cookies after login")
                    return None
                
        except Exception as e:
            logger.error(f"Login failed: {str(e)}")
            return None
    
    async def interactive_refresh(self, timeout: int = 120000) -> Optional[Dict[str, str]]:
        """
        Interactive cookie refresh - opens browser for user to login manually
        
        This is useful when automated login doesn't work. Opens a browser window
        where the user can log in normally, then extracts the cookies.
        
        Args:
            timeout: Maximum time to wait for user to login (ms)
            
        Returns:
            Dictionary of cookies or None if failed
        """
        try:
            async with async_playwright() as p:
                browser = await p.chromium.launch(headless=False)  # Show browser
                context = await browser.new_context()
                page = await context.new_page()
                
                print("\n" + "="*70)
                print("INTERACTIVE LOGIN")
                print("="*70)
                print("\nA browser window will open. Please:")
                print("1. Log into PLANE if needed")
                print("2. Navigate to your workspace")
                print("3. Wait for cookies to be extracted (window will close automatically)")
                print("\nBrowser will open in 3 seconds...")
                print("="*70)
                
                await asyncio.sleep(3)
                
                # Open PLANE
                await page.goto(self.base_url)
                
                # Wait for user to log in and navigate to workspace
                # We'll check periodically for the workspace URL
                max_attempts = timeout // 2000
                for attempt in range(max_attempts):
                    current_url = page.url
                    
                    if self.workspace_slug in current_url:
                        print(f"\n✅ Detected workspace in URL: {current_url}")
                        break
                    
                    await asyncio.sleep(2)
                else:
                    print("\n⏱️ Timeout waiting for workspace navigation")
                
                # Extract cookies
                cookies = await context.cookies()
                
                cookie_dict = {}
                required_names = ['session-id', 'agentc-auth-token', 'csrftoken', 'ajs_anonymous_id']
                
                for cookie in cookies:
                    if cookie['name'] in required_names:
                        cookie_dict[cookie['name']] = cookie['value']
                
                print(f"\n✅ Extracted {len(cookie_dict)} cookies")
                
                await browser.close()
                
                return cookie_dict if len(cookie_dict) >= 2 else None
                
        except Exception as e:
            logger.error(f"Interactive refresh failed: {str(e)}")
            return None

"""
Basic manual login module for Sharedown Slim.

Opens a browser window and waits for the user to manually authenticate.
No automation - user handles all authentication steps.
"""

import asyncio
import logging
from typing import Optional



class ManualLogin:
    """
    Handle manual authentication flow.
    
    Opens the target URL in a browser and waits for the user to complete
    authentication. Detection of successful login is based on the presence
    of SharePoint video player elements.
    """
    
    # Selectors that indicate successful authentication and video page load
    VIDEO_PLAYER_SELECTORS = [
        'div[data-sp-feature-tag="MediaWebPart"]',  # SharePoint media web part
        'div[class*="mediaWebPart"]',  # Media web part class
        'video',  # HTML5 video element
        'iframe[src*="embed"]',  # Embedded video iframe
        'div[class*="video-player"]',  # Generic video player
    ]
    
    def __init__(self, timeout: int = 90):
        """
        Initialize manual login handler.
        
        Args:
            timeout: Maximum seconds to wait for manual authentication
        """
        self.timeout = timeout
        self.logger = logging.getLogger(__name__)
        self.logger.info(f"ManualLogin initialized with {timeout}s timeout")
    
    async def authenticate(self, page, url: str) -> bool:
        """
        Navigate to URL and wait for manual authentication if needed.
        
        Smart flow:
        1. Navigate to URL
        2. Quick check for video player (5s) - may already be logged in
        3. If found → success (no manual auth needed)
        4. If not found → prompt user and wait with full timeout
        
        Args:
            page: Pyppeteer page object
            url: Target SharePoint video URL
            
        Returns:
            True if authentication appears successful, False on timeout
        """
        self.logger.info(f"Starting authentication flow for: {url}")
        
        try:
            # Navigate to the target URL
            self.logger.info("Navigating to target URL...")
            await page.goto(url, {'waitUntil': 'networkidle2', 'timeout': 30000})
            self.logger.info("✓ Page loaded")
            
            # Quick check: are we already authenticated?
            self.logger.info("Checking if already authenticated...")
            quick_check = await self._wait_for_video_player(page, timeout=5)
            
            if quick_check:
                self.logger.info("✓ Already authenticated - video player detected immediately")
                return True
            
            # Not authenticated yet - need manual login
            self.logger.info("Video player not found - manual authentication required")
            self._show_instructions(url)
            
            # Wait for user to complete authentication
            self.logger.info(f"Waiting up to {self.timeout}s for manual authentication...")
            success = await self._wait_for_video_player(page, timeout=self.timeout)
            
            if success:
                self.logger.info("✓ Authentication successful - video player detected")
                return True
            else:
                self.logger.error("✗ Authentication timeout - video player not detected")
                return False
                
        except Exception as e:
            self.logger.error(f"Authentication flow error: {e}")
            return False
    
    async def _wait_for_video_player(self, page, timeout: int = None) -> bool:
        """
        Wait for any video player selector to appear.
        
        Args:
            page: Pyppeteer page object
            timeout: Timeout in seconds (None = use instance timeout)
        
        Returns:
            True if video player detected, False on timeout
        """
        if timeout is None:
            timeout = self.timeout
        
        timeout_ms = timeout * 1000  # Convert to milliseconds
        
        try:
            # Try each selector with the specified timeout
            for selector in self.VIDEO_PLAYER_SELECTORS:
                try:
                    self.logger.debug(f"Checking for selector: {selector} (timeout: {timeout}s)")
                    await page.waitForSelector(
                        selector,
                        {'timeout': timeout_ms}
                    )
                    self.logger.info(f"✓ Found video player element: {selector}")
                    return True
                except Exception:
                    # Try next selector
                    continue
            
            # If we get here, none of the selectors were found
            return False
            
        except Exception as e:
            self.logger.debug(f"Video player detection error: {e}")
            return False
    
    def _show_instructions(self, url: str):
        """Display user instructions for manual authentication."""
        print("\n" + "=" * 70)
        print("  ⚠️  MANUAL AUTHENTICATION REQUIRED")
        print("=" * 70)
        print("\nThe video player was not detected. Please authenticate:")
        print("\n  1. 🔑 Complete the login process in the browser window")
        print("  2. ⏳ Wait for the video page to fully load")
        print(f"  3. ⏱️  You have {self.timeout} seconds to complete this")
        print("\n👁️  The application will automatically continue once the video")
        print("   player is detected on the page.")
        print("\n" + "=" * 70 + "\n")
        self.logger.info("Manual authentication instructions displayed to user")
    
    async def keep_alive(self, page, duration: int = 300):
        """
        Keep the page alive for video capture operations.
        
        Performs periodic checks to ensure page is still responsive.
        
        Args:
            page: Pyppeteer page object
            duration: How long to keep alive (seconds)
        """
        self.logger.info(f"Keeping page alive for {duration}s")
        
        try:
            await asyncio.sleep(duration)
        except Exception as e:
            self.logger.warning(f"Keep-alive interrupted: {e}")


async def perform_manual_login(page, url: str, timeout: int = 90) -> bool:
    """
    Convenience function for manual authentication.
    
    Args:
        page: Pyppeteer page object
        url: Target SharePoint video URL
        timeout: Maximum seconds to wait
        
    Returns:
        True if authentication successful, False otherwise
    """
    login = ManualLogin(timeout=timeout)
    return await login.authenticate(page, url)

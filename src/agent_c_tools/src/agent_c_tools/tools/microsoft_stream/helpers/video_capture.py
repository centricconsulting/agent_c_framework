"""
Video URL capture from SharePoint pages via API interception.

Intercepts SharePoint API responses to extract video manifest URLs,
matching the implementation in //sharedown/new/downloader.py
"""

import asyncio
import json
import logging
from typing import Dict, Optional, List, Any
from urllib.parse import urlparse, parse_qs, urlunparse



class SharePointInterceptor:
    """Intercepts and parses SharePoint API responses to extract video metadata."""
    
    def __init__(self):
        self.known_response_patterns = [
            'RenderListDataAsStream?@a1=',
            'RenderListDataAsStream?@listUrl',
            'SP.List.GetListDataAsStream?listFullUrl'
        ]
        self.captured_responses: List[Any] = []
        self.capture_enabled = False
        self.logger = logging.getLogger(__name__)
    
    async def response_handler(self, response):
        """Handle HTTP responses and capture SharePoint API responses."""
        if not self.capture_enabled:
            return
        
        try:
            request = response.request
            if request.resourceType in ['fetch', 'xhr'] and request.method.lower() in ['post', 'get']:
                url = response.url
                for pattern in self.known_response_patterns:
                    if pattern in url:
                        self.logger.debug(f"Captured SharePoint API response: {url}")
                        self.captured_responses.append(response)
                        break
        except Exception as e:
            self.logger.warning(f"Error handling response: {e}")
    
    def start_capture(self):
        """Start capturing API responses."""
        self.capture_enabled = True
        self.captured_responses.clear()
        self.logger.debug("Started SharePoint API response capture")
    
    def stop_capture(self):
        """Stop capturing API responses."""
        self.capture_enabled = False
        self.logger.debug("Stopped SharePoint API response capture")
    
    async def extract_video_metadata(self, page_url: str) -> Optional[Dict[str, str]]:
        """
        Extract video metadata from captured SharePoint responses.
        
        Returns:
            Dictionary with video URL and metadata, or None if not found
        """
        for response in self.captured_responses:
            try:
                # Check if response still exists before trying to parse
                if hasattr(response, 'json'):
                    data = await response.json()
                    metadata = self._parse_sharepoint_response(data, page_url)
                    if metadata:
                        return metadata
            except Exception as e:
                self.logger.debug(f"Failed to parse SharePoint response: {e}")
                continue
        
        self.logger.warning("No valid video metadata found in captured responses")
        return None
    
    def _parse_sharepoint_response(self, data: Dict, page_url: str) -> Optional[Dict[str, str]]:
        """
        Parse SharePoint API response to extract video information.
        
        Matches original _makeVideoManifestFetchURL implementation.
        """
        try:
            # Look for ListSchema with video metadata
            if 'ListSchema' not in data or 'ListData' not in data:
                self.logger.debug("Response missing ListSchema or ListData")
                return None
            
            list_schema = data['ListSchema']
            list_data = data['ListData']
            
            # Extract required data
            media_base_url = list_schema.get('.mediaBaseUrl', '')
            drive_access_token = list_schema.get('.driveAccessToken', '')
            video_manifest_url_template = list_schema.get('.videoManifestUrl', '')
            caller_stack = list_schema.get('.callerStack', '')
            
            # Extract spItemUrl - try CurrentFolderSpItemUrl first, then Row[0]['.spItemUrl'] as fallback
            sp_item_url = list_data.get('CurrentFolderSpItemUrl', '')
            if not sp_item_url and 'Row' in list_data and len(list_data['Row']) > 0:
                sp_item_url = list_data['Row'][0].get('.spItemUrl', '')
                self.logger.debug(f"Using spItemUrl from Row[0]: {sp_item_url}")
            elif sp_item_url:
                self.logger.debug(f"Using CurrentFolderSpItemUrl: {sp_item_url}")
            else:
                self.logger.warning("Could not find spItemUrl in CurrentFolderSpItemUrl or Row[0]!")
            
            file_type = 'mp4'  # Default to mp4
            
            self.logger.debug(f"Extracted SharePoint data:")
            self.logger.debug(f"  - mediaBaseUrl: {media_base_url[:50] if media_base_url else 'MISSING'}...")
            self.logger.debug(f"  - driveAccessToken: {drive_access_token[:50] if drive_access_token else 'MISSING'}...")
            self.logger.debug(f"  - videoManifestUrl template: {video_manifest_url_template[:100] if video_manifest_url_template else 'MISSING'}...")
            self.logger.debug(f"  - spItemUrl: {sp_item_url}")
            self.logger.debug(f"  - callerStack: {caller_stack[:50] if caller_stack else 'MISSING'}...")
            
            if not video_manifest_url_template:
                self.logger.error("No .videoManifestUrl template found in SharePoint response")
                return None
            
            # Check we have all required data
            if not media_base_url:
                self.logger.error("Missing .mediaBaseUrl")
                return None
            if not drive_access_token:
                self.logger.error("Missing .driveAccessToken")
                return None
            if not sp_item_url:
                self.logger.error("Missing .spItemUrl (docid) - this is required for video manifest URL")
                return None
            
            # Replace placeholders in the template (matching original implementation)
            manifest_url = video_manifest_url_template
            manifest_url = manifest_url.replace('{.mediaBaseUrl}', media_base_url)
            manifest_url = manifest_url.replace('{.fileType}', file_type)
            manifest_url = manifest_url.replace('{.callerStack}', caller_stack)
            manifest_url = manifest_url.replace('{.spItemUrl}', sp_item_url)
            manifest_url = manifest_url.replace('{.driveAccessToken}', drive_access_token)
            
            self.logger.info(f"Template BEFORE replacement: {video_manifest_url_template}")
            self.logger.info(f"After template replacement: {manifest_url}")
            
            # Check for unreplaced placeholders
            placeholders = ['{.mediaBaseUrl}', '{.fileType}', '{.callerStack}', '{.spItemUrl}', '{.driveAccessToken}']
            for placeholder in placeholders:
                if placeholder in manifest_url:
                    self.logger.error(f"UNREPLACED PLACEHOLDER: {placeholder}")
            
            # Parse URL and add DASH parameters
            parsed = urlparse(manifest_url)
            existing_query = parsed.query
            
            # Add DASH-specific parameters
            dash_params = [
                ('action', 'Access'),
                ('part', 'Index'),
                ('format', 'dash'),
                ('useScf', 'True'),
                ('pretranscode', '0'),
                ('transcodeahead', '0')
            ]
            
            # Build query string parts
            query_parts = []
            if existing_query:
                query_parts.append(existing_query)
            
            for key, value in dash_params:
                query_parts.append(f"{key}={value}")
            
            new_query = '&'.join(query_parts)
            
            # Reconstruct URL with new query string
            final_url = urlunparse((
                parsed.scheme,
                parsed.netloc,
                parsed.path,
                parsed.params,
                new_query,
                parsed.fragment
            ))
            
            self.logger.info(f"Constructed final video URL (length={len(final_url)}):")
            self.logger.info(f"  {final_url}")
            self.logger.info(f"URL validation:")
            self.logger.info(f"  - Starts with https://: {final_url.startswith('https://')}")
            self.logger.info(f"  - Contains 'action=Access': {'action=Access' in final_url}")
            self.logger.info(f"  - Has access token: {'access_token' in final_url.lower() or 'driveAccessToken' in final_url}")
            self.logger.info(f"  - No unreplaced placeholders: {'{.' not in final_url}")
            
            return {
                'video_url': final_url,
                'media_base_url': media_base_url,
                'drive_access_token': drive_access_token,
                'sp_item_url': sp_item_url,
                'manifest_url_template': video_manifest_url_template
            }
            
        except Exception as e:
            self.logger.error(f"Error parsing SharePoint response: {e}")
            import traceback
            self.logger.debug(traceback.format_exc())
            return None


logger = logging.getLogger(__name__)

async def setup_interception(page) -> SharePointInterceptor:
    """
    Set up SharePoint API response interception on a page.
    
    Args:
        page: Pyppeteer page object
        
    Returns:
        SharePointInterceptor instance
    """
    interceptor = SharePointInterceptor()
    
    # Set up response handler
    def sync_response_handler(response):
        asyncio.create_task(interceptor.response_handler(response))
    
    page.on('response', sync_response_handler)
    
    logger.debug("SharePoint interception set up on page")
    return interceptor


async def capture_video_url(page, page_url: str, interceptor: SharePointInterceptor) -> Optional[Dict]:
    """
    Capture video URL by reloading page and intercepting SharePoint API.
    
    Args:
        page: Pyppeteer page object (already authenticated)
        page_url: Current page URL
        interceptor: SharePointInterceptor instance
        
    Returns:
        Dictionary with video information or None
    """
    try:
        # Start capturing responses BEFORE reload
        logger.info("Starting response capture for SharePoint API calls")
        interceptor.start_capture()
        
        # Reload page to trigger SharePoint API calls
        logger.info("Reloading page to capture SharePoint API responses...")
        await page.evaluate('() => location.reload(true)')
        
        # Wait for network to settle after reload
        await page.waitForNavigation({'waitUntil': 'networkidle0'})
        logger.info("Page reloaded, network idle")
        
        # Stop capturing and extract metadata
        interceptor.stop_capture()
        logger.info("=" * 70)
        logger.info("SHAREPOINT API RESPONSE CAPTURE RESULTS:")
        logger.info(f"Captured {len(interceptor.captured_responses)} SharePoint API responses")
        
        if len(interceptor.captured_responses) == 0:
            logger.error("⚠️ No SharePoint API responses captured!")
            logger.error("This means the page reload didn't trigger the expected API calls.")
        else:
            logger.info("Captured response URLs:")
            for i, resp in enumerate(interceptor.captured_responses):
                logger.info(f"  [{i}] {resp.url}")
        logger.info("=" * 70)
        
        logger.info("Extracting video metadata from captured responses")
        metadata = await interceptor.extract_video_metadata(page_url)
        
        if not metadata:
            logger.error("Failed to extract video metadata")
            return None
        
        logger.info(f"✓ Extracted video URL: {metadata['video_url'][:100]}... +{len(metadata['video_url'])} chars")
        logger.info(f"✓ Media base URL: {metadata.get('media_base_url'[:100], 'N/A')}...")
        
        return metadata
        
    except Exception as e:
        logger.error(f"Video URL capture failed: {e}")
        return None

import yaml
from datetime import datetime
from pathlib import Path
from typing import Optional
from pyppeteer import launch

from agent_c.toolsets import Toolset, json_schema
from ..workspace.tool import WorkspaceTools

from ...helpers.path_helper import os_file_system_path, ensure_file_extension
from ...helpers.validate_kwargs import validate_required_fields

from .helpers.login import perform_manual_login
from .helpers.config import SlimConfig
from .helpers.video_capture import setup_interception, capture_video_url
from .helpers.ytdlp_command import YtdlpDownloader



class MicrosoftStreamTools(Toolset):
    """
    A toolset for capturing Microsoft Stream videos by intercepting video URLs and downloading them as MP4 files.

    This tool automates the process of:
    1. Launching a browser and handling user authentication to Microsoft Stream
    2. Intercepting SharePoint API calls to capture the direct video URL
    3. Downloading the video using yt-dlp to the specified workspace location

    The tool supports both headless and interactive modes, with interactive mode being the default
    to allow users to manually complete authentication. Videos are saved to workspace paths using
    the UNC path format (//workspace/path/filename.mp4).

    Key Features:
    - Automatic browser-based authentication handling
    - SharePoint API interception for video URL capture
    - High-quality video download using yt-dlp
    - Workspace integration for file management
    - Optional browser persistence for debugging
    """

    def __init__(self, **kwargs):
        super().__init__(**kwargs, name='msft_stream_capture', use_prefix=False)
        self.workspace_tool: Optional[WorkspaceTools] = None
        self.config = SlimConfig()
        self.browser_path = self.config.get_browser_path()
        self.browser_options = self.config.get_browser_launch_options()


    async def post_init(self):
        # Try multiple approaches to get WorkspaceTools dependency
        self.workspace_tool = None

        # First, try to get from available_tools (the correct property)
        self.workspace_tool = self.tool_chest.available_tools.get("WorkspaceTools")

        # If still not found, try to get from toolset instances directly as fallback
        if not self.workspace_tool and hasattr(self.tool_chest, '_ToolChest__toolset_instances'):
            self.workspace_tool = self.tool_chest._ToolChest__toolset_instances.get("WorkspaceTools")
            if self.workspace_tool:
                self.logger.warning("⚠ Found WorkspaceTools in toolset instances - using direct access fallback")

        if self.workspace_tool:
            self.logger.info("✓ MicrosoftStreamTools initialized with WorkspaceTools dependency")
        else:
            self.logger.error("❌ MicrosoftStreamTools failed to get WorkspaceTools dependency")
            self.logger.debug(f"Available tools: {list(self.tool_chest.available_tools.keys())}")

        if self.valid and self.workspace_tool:
            print("MicrosoftStreamTools initialized")
        else:
            print("="*80)
            print(" ❌ MicrosoftStreamTools NOT properly configured!")
            if not self.workspace_tool:
                print(" - WorkspaceTools dependency not found")


    @staticmethod
    def _create_error_response(error_message: str) -> str:
        """Create a standardized error response."""
        return yaml.dump({"success": False, "error": error_message}, default_flow_style=False, sort_keys=False, allow_unicode=True)

    @staticmethod
    def _create_success_response(message: str, **additional_data) -> str:
        """Create a standardized success response."""
        response = {
            "success": True,
            "message": message,
            **additional_data
        }
        return yaml.dump(response, default_flow_style=False, sort_keys=False, allow_unicode=True)

    @json_schema(
        description="Capture a Microsoft Stream video given its URL and save to mp4.",
        params={
            "stream_url": {
                "type": "string",
                "description": "a fully qualified Microsoft Stream video URL",
                "required": True
            },
            "output_path": {
                "type": "string",
                "description": "A full UNC path and filename on where to save the mp4 file. If not provided, defaults to '//project/videos/{timestamp}.mp4'",
                "required": False
            },
            "headless": {
                "type": "boolean",
                "description": "Whether to run the browser in headless mode. Default is False so the user can manually authenticate.",
                "required": False,
                "default": False
            },
            "timeout": {
                "type": "integer",
                "description": "Time in seconds to wait for user to authenticate if headless is False. Default is 90 seconds.",
                "required": False,
                "default": 90
            },
            "keep_open": {
                "type": "boolean",
                "description": "Whether to keep the browser open after capturing the stream. Default is False",
                "required": False,
                "default": False
            },
        }
    )
    async def capture_stream(self, **kwargs) -> str:
        """
        Capture a Microsoft Stream video given its URL.

        Args:
            kwargs: Keyword arguments including:
                stream_url (str): The URL of the Microsoft Stream video to capture.
                output_path (Optional[str]): The workspace path where the captured video will be saved.
                                             If None, a default path will be used.

        Returns:
            str: A yaml string indicating success or failure, and details.
        """
        output_path = kwargs.get('output_path', f"//project/videos/{datetime.now().strftime('%y%m%d_%H%M%S')}.mp4")
        stream_url = kwargs.get('stream_url')
        headless = kwargs.get('headless', False)
        timeout = kwargs.get('timeout', self.config.get_timeout())
        keep_open = kwargs.get('keep_open', False)
        tool_context = kwargs.get('tool_context', {})
        bridge = tool_context.get('bridge')

        success, message = validate_required_fields(kwargs, ['stream_url'])
        if not success:
            return message

        if not self.workspace_tool:
            return "Error. Cannot utilize tool as WorkspaceTools are not available"

        # Find output path
        try:
            # Parse the output_path UNC path using the existing robust workspace parser
            error, workspace_obj, relative_path = self.workspace_tool._parse_unc_path(output_path)

            if error:
                return self._create_error_response(f"Invalid workspace path '{output_path}': {error}")

            if not workspace_obj:
                return self._create_error_response(f"Workspace not found for path '{output_path}'")

            output_full_os_path = os_file_system_path(self.workspace_tool, output_path)

        except Exception as e:
            return self._create_error_response(f"Error processing output path '{output_path}': {e}")

        self.logger.info(f"Capturing stream from {stream_url} to {output_path}")
        ########### Placeholder for actual capture logic
        message = f":::IMPORTANT\n\n- *Browser Launch* A browser window is being launched, you may need to login within 90 seconds to complete authorization to capture stream\n:::"
        await bridge.raise_render_media_markdown(message, "MicrosoftStreamTools")

        # Step 1: launch browser, enable user to manually authenticate if needed
        browser = await launch(**self.browser_options)
        page = await browser.newPage()
        viewport = self.config.get_viewport()
        await page.setViewport(viewport)
        self.logger.info("✓ Browser launched successfully")
        interceptor = await setup_interception(page)
        auth_success = await perform_manual_login(page, stream_url, timeout=timeout)
        if not auth_success:
            self.logger.error("❌ Authentication failed or timed out")
            self.logger.error("   Make sure you complete login and the video player loads")
            message = ":::CAUTION\n\n*Authentication Failed* Authentication failed or timed out. You may want to ask the agent to try again\n:::"
            await bridge.raise_render_media_markdown(message, "MicrosoftStreamTools")
            await browser.close()
            return self._create_error_response(message)
        else:
            self.logger.info("✓ Authentication successful\n")

        # Step 2: Capture video URL via SharePoint API interception
        self.logger.info("=== STEP 2: Video URL Capture ===")
        video_info = await capture_video_url(page, stream_url, interceptor)

        if not video_info:
            self.logger.error("❌ Failed to capture video info.  SharePoint API interception did not capture video manifest")
            await browser.close()
            return self._create_error_response("Failed to capture video info.  SharePoint API interception did not capture video manifest")
        video_url = video_info.get('video_url')
        if video_url:
            self.logger.info(f"✓ Video URL captured successfully")

        if keep_open:
            message = ":::Important\n\n*Browser Left Open* The browser has been left open per your request. You can close it manually when done.\n:::"
        else:
            message = ":::Note\n\n*Browser Closed* The browser has been automatically closed.  The video should be downloading in teh background now.\n:::"
            try:
                await browser.close()
                self.logger.info("✓ Browser closed")
            except Exception as cleanup_error:
                self.logger.warning(f"Browser cleanup warning (ignoring): {cleanup_error}")
        await bridge.raise_render_media_markdown(message, "MicrosoftStreamTools")
        # Step 3: Download video with yt-dlp
        self.logger.info("=== STEP 3: Video Download ===")

        # Prepare output path
        filename = ensure_file_extension(output_full_os_path, '.mp4')
        self.logger.info(f"Downloading to: {filename}")
        client = YtdlpDownloader()
        download_result = await client.download_video(video_url, Path(filename))

        if not download_result or not download_result.get('success'):
            self.logger.error("❌ Video download failed")
            if download_result and 'error' in download_result:
                self.logger.error(f"   Error: {download_result['error']}")
            # Browser already closed, just return error
            return self._create_error_response("Video download failed")

        self.logger.info("✓ Video download completed successfully!")
        self.logger.info(f"  Output file: {download_result['output_file']}")
        self.logger.info(f"  File size: {download_result.get('file_size_mb', 0):.2f} MB")



        # message = f":::SUCCESS\n\n*Stream Captured* The stream has been captured successfully and saved to the specified location.\n- Output Path: {output_path}\n- File Size: {download_result.get('file_size_mb', 0):.2f} MB\n:::"
        # await bridge.raise_render_media_markdown(message, "MicrosoftStreamTools")

        # Simulate successful capture
        return self._create_success_response(
            message="Stream captured successfully",
            filename=output_path,
            file_size_mb=round(download_result.get('file_size_mb', 0), 2)
        )


Toolset.register(MicrosoftStreamTools, required_tools=['WorkspaceTools'])
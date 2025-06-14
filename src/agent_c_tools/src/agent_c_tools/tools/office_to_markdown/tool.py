"""Office to Markdown conversion tool for Agent C."""

import json
import logging
import threading

import yaml
from typing import Optional

from agent_c.models.context.interaction_context import InteractionContext
from agent_c.toolsets.tool_set import Toolset
from agent_c.toolsets.json_schema import json_schema
from ... import WorkspaceTools
from ...helpers.path_helper import create_unc_path, ensure_file_extension, os_file_system_path
from ...helpers.validate_kwargs import validate_required_fields
from .business_logic.office_converter import OfficeToMarkdownConverter

logger = logging.getLogger(__name__)


class OfficeToMarkdownTools(Toolset):
    """
    Toolset for converting office files to markdown using the MarkItDown library.
    
    This tool provides a clean interface for converting various office document formats
    (docx, pptx, xlsx, pdf, html) to markdown format, following clean architecture
    principles with proper separation of concerns.
    """
    
    def __init__(self, **kwargs):
        super().__init__(**kwargs, name="office_to_markdown")
        self.workspace_tool: Optional[WorkspaceTools] = None
        self.converter: Optional[OfficeToMarkdownConverter] = None
        
    async def post_init(self):
        """Initialize dependencies after tool chest is available."""
        self.workspace_tool = self.tool_chest.available_tools.get("WorkspaceTools")
        self.converter = OfficeToMarkdownConverter()
    
    def _format_response(self, success: bool, message: str = None, **additional_data) -> str:
        """Return consistent YAML response format."""
        response = {"success": success}
        if message:
            response["message"] = message
        response.update(additional_data)
        return yaml.dump(response, default_flow_style=False, sort_keys=False, allow_unicode=True)
    
    @json_schema(
        description="Convert office files (docx, pptx, xlsx, pdf, html) to markdown format using MarkItDown library",
        params={
            "input_workspace": {
                "type": "string",
                "description": "Source workspace name containing the office file",
                "required": True
            },
            "input_path": {
                "type": "string",
                "description": "Source file path within workspace (e.g., 'documents/report.docx')",
                "required": True
            },
            "output_workspace": {
                "type": "string",
                "description": "Output workspace name (defaults to input_workspace if not specified)",
                "required": False
            },
            "output_path": {
                "type": "string",
                "description": "Output markdown file path within workspace (e.g., 'converted/report.md')",
                "required": True
            }
        }
    )
    async def convert_office_to_markdown(self, **kwargs) -> str:
        """
        Convert an office file to markdown format.
        
        This method provides a clean interface for office document conversion,
        handling file I/O through workspace tools and delegating the actual
        conversion logic to the business logic layer.
        
        Args:
            **kwargs: Keyword arguments containing:
                - input_workspace: Source workspace name
                - input_path: Source file path within workspace
                - output_workspace: Output workspace (optional, defaults to input_workspace)
                - output_path: Output markdown file path
                
        Returns:
            YAML-formatted response with success status and operation details
        """
        try:
            # Validate required fields
            success, message = validate_required_fields(
                kwargs=kwargs, 
                required_fields=['input_workspace', 'input_path', 'output_path']
            )
            if not success:
                return self._format_response(False, message)
            
            # Extract parameters with workspace defaulting
            input_workspace = kwargs.get('input_workspace')
            output_workspace = kwargs.get('output_workspace', input_workspace)
            input_path = kwargs.get('input_path')
            output_path = kwargs.get('output_path')
            tool_context: InteractionContext =kwargs.get('tool_context')
            client_wants_cancel: threading.Event = tool_context.client_wants_cancel


            # Create UNC path and get OS path for tool
            input_unc_path = create_unc_path(input_workspace, input_path)
            file_system_path = os_file_system_path(self.workspace_tool, input_unc_path)

            # Delegate to business logic for conversion to markdown
            logger.info(f"Converting office file: {input_unc_path}")
            result = self.converter.convert_file_to_markdown(file_system_path)
            
            if not result.success:
                return self._format_response(False, result.error_message, 
                                           input_file=input_unc_path,
                                           file_type=result.file_type)
            
            # Prepare output path with .md extension
            output_path = ensure_file_extension(output_path, 'md')
            output_unc_path = create_unc_path(output_workspace, output_path)
            
            # Write converted markdown content
            write_result = await self.workspace_tool.write(
                    path=output_unc_path,
                    data=result.markdown_content,
                    mode="write")


            # Check for write errors
            write_data = json.loads(write_result)
            if 'error' in write_data:
                return self._format_response(False, f"Failed to write output file: {write_data['error']}",
                                           input_file=input_unc_path)

            # Get OS path for media event
            output_file_system_path = os_file_system_path(self.workspace_tool, output_unc_path)

            # Raise media event for successful conversion
            if output_file_system_path:
                await self._raise_render_media(
                    tool_context,
                    sent_by_class=self.__class__.__name__,
                    sent_by_function='convert_office_to_markdown',
                    content_type="text/html",
                    content=f"<p>Office file converted to markdown: <a href='file://{output_file_system_path}'>{output_path}</a></p>",
                )

            logger.info(f"Successfully converted {input_unc_path} to {output_unc_path}")

            return self._format_response(
                True,
                "Office file converted to markdown successfully",
                input_file=input_unc_path,
                output_file=output_unc_path,
                file_type=result.file_type,
                os_path=output_file_system_path
            )
            
        except Exception as e:
            logger.exception("Error in convert_office_to_markdown")
            return self._format_response(False, f"Unexpected error: {str(e)}")
    
    @json_schema(
        description="Get information about supported file types and converter capabilities",
        params={}
    )
    async def get_conversion_info(self, **kwargs) -> str:
        """
        Get information about the office to markdown converter capabilities.
        
        Returns:
            YAML-formatted response with converter information and supported file types
        """
        try:
            if not self.converter:
                return self._format_response(False, "Converter not initialized")
            
            info = self.converter.get_conversion_info()
            
            return self._format_response(
                True,
                "Converter information retrieved successfully",
                **info
            )
            
        except Exception as e:
            logger.exception("Error in get_conversion_info")
            return self._format_response(False, f"Error retrieving converter info: {str(e)}")


# Register the toolset
Toolset.register(OfficeToMarkdownTools, required_tools=['WorkspaceTools'])
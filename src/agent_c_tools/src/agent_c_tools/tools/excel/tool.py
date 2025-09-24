"""
Excel Tools for Agent C Framework - Thin Interface
Clean, thin interface layer that delegates to business logic components.
Each method is under 25 lines and focuses solely on interface responsibilities.
"""

import json
from typing import Any, Dict, List, Optional, cast

from agent_c.toolsets import Toolset, json_schema
from agent_c_tools.helpers.validate_kwargs import validate_required_fields
from agent_c_tools.tools.workspace.tool import WorkspaceTools

from agent_c_tools.tools.excel.business_logic.workbook_manager import WorkbookManager
from agent_c_tools.tools.excel.business_logic.concurrency_manager import ConcurrencyManager
from agent_c_tools.tools.excel.business_logic.excel_operations import ExcelOperations


class ExcelTools(Toolset):
    """
    Comprehensive Excel file manipulation capabilities for high-volume operations
    and multi-agent workflows. This is a thin interface layer.
    """

    def __init__(self, **kwargs):
        """Initialize the ExcelTools."""
        super().__init__(**kwargs, name='excel')
        self.workspace_tool: Optional[WorkspaceTools] = None
        self.workbook_manager = WorkbookManager()
        self.concurrency_manager = ConcurrencyManager()
        self.excel_operations = ExcelOperations()

    async def post_init(self):
        """Initialize dependencies after tool chest is available."""
        self.workspace_tool = cast(WorkspaceTools, self.tool_chest.available_tools.get('WorkspaceTools'))
        if not self.workspace_tool:
            self.logger.error("WorkspaceTools dependency not available")

    def _validate_workbook_loaded(self) -> Optional[str]:
        """Check if workbook is loaded and return error JSON if not."""
        if not self.workbook_manager.has_workbook():
            return json.dumps({
                'success': False,
                'error': 'No workbook is currently loaded. Create or load a workbook first.'
            })
        return None

    async def _handle_workspace_save(self, unc_path: str, workbook_bytes: bytes, tool_context: Dict) -> Dict[str, Any]:
        """Handle saving workbook bytes to workspace."""
        write_result = await self.workspace_tool.internal_write_bytes(
            path=unc_path, data=workbook_bytes, mode='write'
        )
        result_data = json.loads(write_result)
        if 'error' in result_data:
            return {'success': False, 'error': f"Failed to save workbook: {result_data['error']}"}
        return {'success': True}

    @json_schema(
        description='Create a new Excel workbook in memory',
        params={}
    )
    async def create_workbook(self, **kwargs) -> str:
        """Create a new Excel workbook in memory."""
        try:
            result = self.workbook_manager.create_workbook()
            self.concurrency_manager.sheet_row_counts.clear()
            self.concurrency_manager.row_reservations.clear()
            return json.dumps(result.to_dict())
        except Exception as e:
            self.logger.error(f"Error in create_workbook: {str(e)}")
            return json.dumps({'success': False, 'error': f"Unexpected error: {str(e)}"})

    @json_schema(
        description='Load an Excel workbook from workspace using UNC path',
        params={
            'path': {'type': 'string', 'description': 'UNC-style path to Excel file', 'required': True},
            'read_only': {'type': 'boolean', 'description': 'Open in read-only mode', 'required': False, 'default': False}
        }
    )
    async def load_workbook(self, **kwargs) -> str:
        """Load an Excel workbook from workspace."""
        success, message = validate_required_fields(kwargs, ['path'])
        if not success:
            return json.dumps({'success': False, 'error': message})

        try:
            from agent_c_tools.helpers.path_helper import os_file_system_path
            unc_path = kwargs.get('path')
            read_only = kwargs.get('read_only', False)

            # Validate and get OS path
            error, _, _ = self.workspace_tool.validate_and_get_workspace_path(unc_path)
            if error:
                return json.dumps({'success': False, 'error': error})

            os_path = os_file_system_path(self.workspace_tool, unc_path)
            if not os_path:
                return json.dumps({'success': False, 'error': 'Could not resolve file path'})

            # Load using business logic
            result = self.workbook_manager.load_workbook(os_path, read_only)
            if result.success:
                self._reset_concurrency_tracking(result.sheets)

            response = result.to_dict()
            response['message'] = 'Excel workbook loaded successfully' if result.success else result.error
            return json.dumps(response)

        except Exception as e:
            return json.dumps({'success': False, 'error': f"Error loading workbook: {str(e)}"})

    def _reset_concurrency_tracking(self, sheets):
        """Reset concurrency tracking and update with sheet info."""
        self.concurrency_manager.sheet_row_counts.clear()
        self.concurrency_manager.row_reservations.clear()
        for sheet_info in sheets:
            self.concurrency_manager.update_sheet_row_count(sheet_info.name, sheet_info.max_row)

    @json_schema(
        description='Save the current workbook to workspace using UNC path',
        params={
            'path': {'type': 'string', 'description': 'UNC-style path to save Excel file', 'required': True},
            'create_backup': {'type': 'boolean', 'description': 'Create backup before overwriting', 'required': False, 'default': False}
        }
    )
    async def save_workbook(self, **kwargs) -> str:
        """Save the current workbook to workspace."""
        error_response = self._validate_workbook_loaded()
        if error_response:
            return error_response

        success, message = validate_required_fields(kwargs, ['path'])
        if not success:
            return json.dumps({'success': False, 'error': message})

        try:
            from agent_c_tools.helpers.path_helper import ensure_file_extension
            unc_path = ensure_file_extension(kwargs.get('path'), 'xlsx')
            tool_context = kwargs.get('tool_context', {})

            save_result, workbook_bytes = self.workbook_manager.save_workbook(unc_path)
            if not save_result.success or not workbook_bytes:
                return json.dumps(save_result.to_dict())

            # Save to workspace
            workspace_result = await self._handle_workspace_save(unc_path, workbook_bytes, tool_context)
            if not workspace_result['success']:
                return json.dumps(workspace_result)

            response = save_result.to_dict()
            response['message'] = 'Excel workbook saved successfully'
            return json.dumps(response)

        except Exception as e:
            return json.dumps({'success': False, 'error': f"Error saving workbook: {str(e)}"})

    @json_schema(
        description='List all sheets in the current workbook',
        params={}
    )
    async def list_sheets(self, **kwargs) -> str:
        """List all sheets in the current workbook."""
        error_response = self._validate_workbook_loaded()
        if error_response:
            return error_response

        try:
            result = self.workbook_manager.get_workbook_info()
            if not result.success:
                return json.dumps(result.to_dict())

            # Add tracked row counts
            sheets_data = result.data.get('sheets', [])
            for sheet_data in sheets_data:
                sheet_name = sheet_data['name']
                if sheet_name in self.concurrency_manager.sheet_row_counts:
                    sheet_data['tracked_row_count'] = self.concurrency_manager.sheet_row_counts[sheet_name]

            return json.dumps({
                'success': True,
                'sheets': sheets_data,
                'total_sheets': result.data.get('total_sheets', 0),
                'active_sheet': result.data.get('active_sheet', '')
            })
        except Exception as e:
            return json.dumps({'success': False, 'error': f"Error listing sheets: {str(e)}"})

    @json_schema(
        description='Create a new sheet in the current workbook',
        params={
            'sheet_name': {'type': 'string', 'description': 'Name for the new sheet', 'required': True},
            'index': {'type': 'integer', 'description': 'Position to insert sheet (0-based)', 'required': False}
        }
    )
    async def create_sheet(self, **kwargs) -> str:
        """Create a new sheet in the current workbook."""
        error_response = self._validate_workbook_loaded()
        if error_response:
            return error_response

        success, message = validate_required_fields(kwargs, ['sheet_name'])
        if not success:
            return json.dumps({'success': False, 'error': message})

        try:
            sheet_name = kwargs.get('sheet_name')
            index = kwargs.get('index')

            result = self.workbook_manager.create_sheet(sheet_name, index)
            if result.success:
                self.concurrency_manager.sheet_row_counts[sheet_name] = 0

            response = result.to_dict()
            response['message'] = f'Sheet "{sheet_name}" created successfully' if result.success else result.error
            return json.dumps(response)
        except Exception as e:
            return json.dumps({'success': False, 'error': f"Error creating sheet: {str(e)}"})

    @json_schema(
        description='Reserve rows for concurrent writing by multiple agents',
        params={
            'row_count': {'type': 'integer', 'description': 'Number of rows to reserve', 'required': True},
            'sheet_name': {'type': 'string', 'description': 'Sheet name', 'required': False, 'default': 'Sheet'},
            'agent_id': {'type': 'string', 'description': 'Agent identifier', 'required': False}
        }
    )
    async def reserve_rows(self, **kwargs) -> str:
        """Reserve a range of rows for exclusive writing."""
        error_response = self._validate_workbook_loaded()
        if error_response:
            return error_response

        success, message = validate_required_fields(kwargs, ['row_count'])
        if not success:
            return json.dumps({'success': False, 'error': message})

        try:
            row_count = kwargs.get('row_count')
            sheet_name = kwargs.get('sheet_name', 'Sheet')
            agent_id = kwargs.get('agent_id')

            if row_count <= 0:
                return json.dumps({'success': False, 'error': 'row_count must be greater than 0'})

            workbook = self.workbook_manager.get_workbook()
            self._ensure_sheet_exists(workbook, sheet_name)

            current_max_row = workbook[sheet_name].max_row
            result = await self.concurrency_manager.reserve_rows(
                row_count, sheet_name, current_max_row, agent_id
            )

            return json.dumps(result.to_dict())
        except Exception as e:
            return json.dumps({'success': False, 'error': f'Error reserving rows: {str(e)}'})

    def _ensure_sheet_exists(self, workbook, sheet_name):
        """Ensure a sheet exists in the workbook."""
        if sheet_name not in workbook.sheetnames:
            create_result = self.workbook_manager.create_sheet(sheet_name)
            if not create_result.success:
                raise Exception(f"Could not create sheet: {create_result.error}")
            self.concurrency_manager.sheet_row_counts[sheet_name] = 0

    @json_schema(
        description='Append records to the end of a sheet',
        params={
            'records': {'type': 'array', 'description': 'Array of records to append', 'required': True},
            'sheet_name': {'type': 'string', 'description': 'Sheet name', 'required': False, 'default': 'Sheet'},
            'headers': {'type': 'array', 'description': 'Column headers if sheet is empty', 'required': False},
            'agent_id': {'type': 'string', 'description': 'Agent identifier', 'required': False},
            'chunk_size': {'type': 'integer', 'description': 'Batch size for processing', 'required': False, 'default': 10000}
        }
    )
    async def append_records(self, **kwargs) -> str:
        """Append records to the end of a sheet."""
        error_response = self._validate_workbook_loaded()
        if error_response:
            return error_response

        success, message = validate_required_fields(kwargs, ['records'])
        if not success:
            return json.dumps({'success': False, 'error': message})

        try:
            records = kwargs.get('records')
            if not records:
                return json.dumps({'success': False, 'error': 'No records provided'})

            async with self.concurrency_manager.get_write_lock():
                result = await self._perform_append_operation(kwargs)
                if result.success:
                    self.concurrency_manager.update_sheet_row_count(
                        kwargs.get('sheet_name', 'Sheet'), result.end_row
                    )

                response = result.to_dict()
                response['message'] = (f'Successfully appended {result.records_written} records'
                                     if result.success else result.error)
                return json.dumps(response)
        except Exception as e:
            return json.dumps({'success': False, 'error': f'Error appending records: {str(e)}'})

    async def _perform_append_operation(self, kwargs):
        """Perform the actual append operation with business logic."""
        import uuid
        import threading

        workbook = self.workbook_manager.get_workbook()
        records = kwargs.get('records')
        sheet_name = kwargs.get('sheet_name', 'Sheet')
        headers = kwargs.get('headers')
        agent_id = kwargs.get('agent_id', f'agent_{uuid.uuid4().hex[:8]}')
        chunk_size = kwargs.get('chunk_size', 10000)
        tool_context = kwargs.get('tool_context', {})
        client_wants_cancel = tool_context.get('client_wants_cancel', threading.Event())

        return await self.excel_operations.append_records(
            workbook=workbook,
            records=records,
            sheet_name=sheet_name,
            headers=headers,
            agent_id=agent_id,
            chunk_size=chunk_size,
            client_wants_cancel=client_wants_cancel,
            progress_callback=lambda msg: self._raise_text_delta_event(msg + '\\n', tool_context=tool_context)
        )

    @json_schema(
        description='Write data to a previously reserved row range',
        params={
            'reservation_id': {'type': 'string', 'description': 'Reservation ID from reserve_rows', 'required': True},
            'records': {'type': 'array', 'description': 'Array of records to write', 'required': True}
        }
    )
    async def write_to_reservation(self, **kwargs) -> str:
        """Write data to a previously reserved row range."""
        error_response = self._validate_workbook_loaded()
        if error_response:
            return error_response

        success, message = validate_required_fields(kwargs, ['reservation_id', 'records'])
        if not success:
            return json.dumps({'success': False, 'error': message})

        try:
            reservation_id = kwargs.get('reservation_id')
            records = kwargs.get('records')

            reservation = self.concurrency_manager.get_reservation(reservation_id)
            if not reservation:
                return json.dumps({'success': False, 'error': f'Reservation {reservation_id} not found'})

            if reservation.status.value != 'active':
                return json.dumps({'success': False, 'error': f'Reservation not active: {reservation.status.value}'})

            if len(records) > reservation.row_count:
                return json.dumps({'success': False, 'error': f'Too many records for reservation'})

            workbook = self.workbook_manager.get_workbook()
            result = await self.excel_operations.write_to_reserved_rows(
                workbook, records, reservation.sheet_name, reservation.start_row, reservation.row_count
            )

            if result.success:
                self.concurrency_manager.complete_reservation(reservation_id, len(records))

            response = result.to_dict()
            response.update({
                'reservation_id': reservation_id,
                'agent_id': reservation.agent_id,
                'message': f'Successfully wrote {len(records)} records to reserved rows' if result.success else result.error
            })

            return json.dumps(response)
        except Exception as e:
            return json.dumps({'success': False, 'error': f'Error writing to reservation: {str(e)}'})

    @json_schema(
        description='Read data from a sheet with optional range specification',
        params={
            'sheet_name': {'type': 'string', 'description': 'Sheet name', 'required': False, 'default': 'Sheet'},
            'start_row': {'type': 'integer', 'description': 'Starting row (1-based)', 'required': False},
            'end_row': {'type': 'integer', 'description': 'Ending row (1-based)', 'required': False},
            'start_column': {'type': 'string', 'description': 'Starting column (e.g. "A")', 'required': False},
            'end_column': {'type': 'string', 'description': 'Ending column (e.g. "Z")', 'required': False},
            'include_headers': {'type': 'boolean', 'description': 'Treat first row as headers', 'required': False, 'default': False},
            'max_rows': {'type': 'integer', 'description': 'Maximum rows to read', 'required': False, 'default': 10000}
        }
    )
    async def read_sheet_data(self, **kwargs) -> str:
        """Read data from a sheet with optional range specification."""
        error_response = self._validate_workbook_loaded()
        if error_response:
            return error_response

        try:
            workbook = self.workbook_manager.get_workbook()
            result = self.excel_operations.read_sheet_data(
                workbook=workbook,
                sheet_name=kwargs.get('sheet_name', 'Sheet'),
                start_row=kwargs.get('start_row', 1),
                end_row=kwargs.get('end_row'),
                start_column=kwargs.get('start_column', 'A'),
                end_column=kwargs.get('end_column'),
                include_headers=kwargs.get('include_headers', False),
                max_rows=kwargs.get('max_rows', 10000)
            )

            if not result.success:
                return json.dumps(result.to_dict())

            return self._handle_read_response(result, kwargs.get('sheet_name', 'Sheet'))
        except Exception as e:
            return json.dumps({'success': False, 'error': f'Error reading sheet data: {str(e)}'})

    def _handle_read_response(self, result, sheet_name):
        """Handle read response, potentially caching large results."""
        import time
        import uuid

        response_data = {'data': result.data, 'headers': result.headers}
        response_json = json.dumps(response_data)

        response_dict = result.to_dict()
        response_dict['sheet_name'] = sheet_name

        # Check if response is too big and cache if needed
        if len(response_json) > 35000 * 4:  # Rough token estimate
            cache_key = f'excel_data_{int(time.time() * 1000)}_{uuid.uuid4().hex[:8]}'
            self.tool_cache.set(cache_key, response_data, expire=3600)

            response_dict.update({
                'cached': True,
                'cache_key': cache_key,
                'message': 'Data cached due to size. Use cache key to access full dataset.'
            })

            # Remove data from response
            response_dict.pop('data', None)
            response_dict.pop('headers', None)

        return json.dumps(response_dict)

    @json_schema(
        description='Load cached sheet data using a cache key',
        params={
            'cache_key': {'type': 'string', 'description': 'Cache key from read_sheet_data', 'required': True}
        }
    )
    async def load_cached_data(self, **kwargs) -> str:
        """Load cached sheet data using a cache key."""
        success, message = validate_required_fields(kwargs, ['cache_key'])
        if not success:
            return json.dumps({'success': False, 'error': message})

        try:
            cache_key = kwargs.get('cache_key')
            cached_data = self.tool_cache.get(cache_key)

            if cached_data is None:
                return json.dumps({'success': False, 'error': f'No cached data found for key {cache_key}'})

            return json.dumps({
                'success': True,
                'message': 'Cached data retrieved successfully',
                'cache_key': cache_key,
                **cached_data
            })
        except Exception as e:
            return json.dumps({'success': False, 'error': f'Error loading cached data: {str(e)}'})

    @json_schema(
        description='Get the next available row number in a sheet',
        params={
            'sheet_name': {'type': 'string', 'description': 'Sheet name', 'required': False, 'default': 'Sheet'}
        }
    )
    async def get_next_available_row(self, **kwargs) -> str:
        """Get the next available row number in a sheet."""
        error_response = self._validate_workbook_loaded()
        if error_response:
            return error_response

        try:
            sheet_name = kwargs.get('sheet_name', 'Sheet')
            workbook = self.workbook_manager.get_workbook()

            reserved_rows = sum(
                res.row_count for res in self.concurrency_manager.get_active_reservations(sheet_name).values()
            )

            result = self.excel_operations.get_next_available_row_info(workbook, sheet_name, reserved_rows)
            return json.dumps(result.to_dict())
        except Exception as e:
            return json.dumps({'success': False, 'error': f'Error getting next available row: {str(e)}'})

    @json_schema(
        description='Get operation status and statistics',
        params={
            'operation_id': {'type': 'string', 'description': 'Operation ID', 'required': False}
        }
    )
    async def get_operation_status(self, **kwargs) -> str:
        """Get status and statistics for operations."""
        try:
            workbook_info = {
                'loaded': self.workbook_manager.has_workbook(),
                'path': self.workbook_manager.get_workbook_path(),
                'sheets': len(self.workbook_manager.get_workbook().sheetnames) if self.workbook_manager.has_workbook() else 0,
                'metadata': (self.workbook_manager.get_workbook_metadata().to_dict()
                           if self.workbook_manager.get_workbook_metadata() else {})
            }

            concurrency_status = self.concurrency_manager.get_operation_status()

            return json.dumps({
                'success': True,
                'current_workbook': workbook_info,
                **concurrency_status
            })
        except Exception as e:
            return json.dumps({'success': False, 'error': f'Error getting operation status: {str(e)}'})


# Register the toolset
Toolset.register(ExcelTools, required_tools=['WorkspaceTools'])
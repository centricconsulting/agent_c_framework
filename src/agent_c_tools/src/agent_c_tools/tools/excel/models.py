"""
Data models for Excel Tools.

Contains all data classes and structures used throughout the Excel tool components.
"""

from dataclasses import dataclass, field
from datetime import datetime
from typing import Any, Dict, List, Optional, Union
from enum import Enum


class ReservationStatus(Enum):
    """Status of a row reservation."""
    ACTIVE = "active"
    COMPLETED = "completed"
    EXPIRED = "expired"


@dataclass
class ReservationInfo:
    """Information about a row reservation."""
    reservation_id: str
    agent_id: str
    sheet_name: str
    start_row: int
    end_row: int
    row_count: int
    status: ReservationStatus
    reserved_at: datetime
    completed_at: Optional[datetime] = None
    records_written: Optional[int] = None
    
    def to_dict(self) -> Dict[str, Any]:
        """Convert to dictionary format."""
        return {
            'reservation_id': self.reservation_id,
            'agent_id': self.agent_id,
            'sheet_name': self.sheet_name,
            'start_row': self.start_row,
            'end_row': self.end_row,
            'row_count': self.row_count,
            'status': self.status.value,
            'reserved_at': self.reserved_at.isoformat(),
            'completed_at': self.completed_at.isoformat() if self.completed_at else None,
            'records_written': self.records_written
        }


@dataclass
class WorkbookMetadata:
    """Metadata about a workbook."""
    operation_id: str
    created_at: Optional[datetime] = None
    loaded_at: Optional[datetime] = None
    saved_at: Optional[datetime] = None
    file_path: Optional[str] = None
    read_only: bool = False
    sheets: List[Dict[str, Any]] = field(default_factory=list)
    total_rows: int = 0
    
    def to_dict(self) -> Dict[str, Any]:
        """Convert to dictionary format."""
        return {
            'operation_id': self.operation_id,
            'created_at': self.created_at.isoformat() if self.created_at else None,
            'loaded_at': self.loaded_at.isoformat() if self.loaded_at else None,
            'saved_at': self.saved_at.isoformat() if self.saved_at else None,
            'file_path': self.file_path,
            'read_only': self.read_only,
            'sheets': self.sheets,
            'total_rows': self.total_rows
        }


@dataclass
class SheetInfo:
    """Information about a worksheet."""
    name: str
    max_row: int
    max_column: int
    is_active: bool = False
    tracked_row_count: Optional[int] = None
    
    def to_dict(self) -> Dict[str, Any]:
        """Convert to dictionary format."""
        result = {
            'name': self.name,
            'max_row': self.max_row,
            'max_column': self.max_column,
            'is_active': self.is_active
        }
        if self.tracked_row_count is not None:
            result['tracked_row_count'] = self.tracked_row_count
        return result


@dataclass
class OperationResult:
    """Result of an Excel operation."""
    success: bool
    message: str
    operation_id: Optional[str] = None
    data: Optional[Dict[str, Any]] = None
    error: Optional[str] = None
    
    def to_dict(self) -> Dict[str, Any]:
        """Convert to dictionary format for JSON response."""
        result = {
            'success': self.success,
            'message': self.message
        }
        if self.operation_id:
            result['operation_id'] = self.operation_id
        if self.data:
            result.update(self.data)
        if self.error:
            result['error'] = self.error
        return result


@dataclass
class WriteResult:
    """Result of a write operation."""
    success: bool
    records_written: int
    start_row: int
    end_row: int
    sheet_name: str
    operation_id: Optional[str] = None
    agent_id: Optional[str] = None
    error: Optional[str] = None
    headers_added: bool = False
    
    def to_dict(self) -> Dict[str, Any]:
        """Convert to dictionary format."""
        result = {
            'success': self.success,
            'records_written': self.records_written,
            'start_row': self.start_row,
            'end_row': self.end_row,
            'sheet_name': self.sheet_name,
            'headers_added': self.headers_added
        }
        if self.operation_id:
            result['operation_id'] = self.operation_id
        if self.agent_id:
            result['agent_id'] = self.agent_id
        if self.error:
            result['error'] = self.error
        return result


@dataclass
class ReadResult:
    """Result of a read operation."""
    success: bool
    data: Optional[List[List[str]]] = None
    headers: Optional[List[str]] = None
    rows_read: int = 0
    columns_read: int = 0
    range_info: Optional[str] = None
    cached: bool = False
    cache_key: Optional[str] = None
    error: Optional[str] = None
    
    def to_dict(self) -> Dict[str, Any]:
        """Convert to dictionary format."""
        result = {
            'success': self.success,
            'rows_read': self.rows_read,
            'columns_read': self.columns_read
        }
        if self.range_info:
            result['range'] = self.range_info
        if self.cached:
            result['cached'] = True
            result['cache_key'] = self.cache_key
            result['message'] = 'Data cached due to size. Use cache key to access full dataset.'
        else:
            if self.data is not None:
                result['data'] = self.data
            if self.headers is not None:
                result['headers'] = self.headers
        if self.error:
            result['error'] = self.error
        return result


@dataclass
class SaveResult:
    """Result of a save operation."""
    success: bool
    file_path: str
    file_size_bytes: int
    sheets_count: int
    operation_id: Optional[str] = None
    backup_created: bool = False
    error: Optional[str] = None
    
    def to_dict(self) -> Dict[str, Any]:
        """Convert to dictionary format."""
        result = {
            'success': self.success,
            'file_path': self.file_path,
            'file_size_bytes': self.file_size_bytes,
            'sheets': self.sheets_count,
            'backup_created': self.backup_created
        }
        if self.operation_id:
            result['operation_id'] = self.operation_id
        if self.error:
            result['error'] = self.error
        return result


@dataclass
class LoadResult:
    """Result of a load operation."""
    success: bool
    file_path: str
    sheets: List[SheetInfo]
    total_rows: int
    read_only: bool
    operation_id: Optional[str] = None
    error: Optional[str] = None
    
    def to_dict(self) -> Dict[str, Any]:
        """Convert to dictionary format."""
        result = {
            'success': self.success,
            'file_path': self.file_path,
            'sheets': [sheet.to_dict() for sheet in self.sheets],
            'total_rows': self.total_rows,
            'read_only': self.read_only
        }
        if self.operation_id:
            result['operation_id'] = self.operation_id
        if self.error:
            result['error'] = self.error
        return result
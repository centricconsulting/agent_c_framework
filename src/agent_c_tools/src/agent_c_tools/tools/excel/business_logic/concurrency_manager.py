"""
Concurrency Manager for Excel Tools.
Handles row reservations, locking, and coordination for multi-agent Excel operations.
"""

import asyncio
import time
import uuid
from datetime import datetime
from typing import Dict, Optional

from agent_c_tools.tools.excel.models import ReservationInfo, ReservationStatus, OperationResult


class ConcurrencyManager:
    """
    Manages concurrent access to Excel sheets through row reservations.

    Provides thread-safe mechanisms for multiple agents to write to Excel
    workbooks without conflicts by reserving row ranges.
    """

    def __init__(self):
        """Initialize the concurrency manager."""
        # Row reservations by reservation_id
        self.row_reservations: Dict[str, ReservationInfo] = {}

        # Sheet row counts for tracking
        self.sheet_row_counts: Dict[str, int] = {}

        # Concurrency control locks
        self._reservation_lock = asyncio.Lock()
        self._write_lock = asyncio.Lock()

    def _generate_reservation_id(self) -> str:
        """Generate a unique reservation ID."""
        return f'res_{int(time.time() * 1000)}_{uuid.uuid4().hex[:8]}'

    async def get_next_available_row(self, sheet_name: str, sheet_max_row: int) -> int:
        """
        Get the next available row for writing in a sheet.

        Args:
            sheet_name: Name of the sheet
            sheet_max_row: Current max row from the sheet

        Returns:
            Next available row number (1-based)
        """
        if sheet_max_row == 1:
            # Need to check if row 1 actually has data
            # For now, assume if max_row is 1, we start at row 1
            return 1

        # Use the larger of tracked count or sheet max row
        tracked_count = self.sheet_row_counts.get(sheet_name, 0)
        return max(sheet_max_row, tracked_count) + 1

    async def reserve_rows(
            self,
            row_count: int,
            sheet_name: str,
            current_max_row: int,
            agent_id: Optional[str] = None
    ) -> OperationResult:
        """
        Reserve a range of rows for exclusive writing by an agent.

        Args:
            row_count: Number of rows to reserve
            sheet_name: Name of the sheet to reserve rows in
            current_max_row: Current maximum row in the sheet
            agent_id: Unique identifier for the agent requesting the reservation

        Returns:
            OperationResult with reservation details or error
        """
        try:
            if row_count <= 0:
                return OperationResult(
                    success=False,
                    message='row_count must be greater than 0',
                    error='Invalid row count'
                )

            if not agent_id:
                agent_id = f'agent_{uuid.uuid4().hex[:8]}'

            async with self._reservation_lock:
                # Get next available row
                next_row = await self.get_next_available_row(sheet_name, current_max_row)

                # Account for any existing reservations
                reserved_rows = sum(
                    res.row_count for res in self.row_reservations.values()
                    if res.sheet_name == sheet_name and res.status == ReservationStatus.ACTIVE
                )

                start_row = max(next_row, self.sheet_row_counts.get(sheet_name, 0) + 1) + reserved_rows
                end_row = start_row + row_count - 1

                # Create reservation
                reservation_id = self._generate_reservation_id()

                reservation = ReservationInfo(
                    reservation_id=reservation_id,
                    agent_id=agent_id,
                    sheet_name=sheet_name,
                    start_row=start_row,
                    end_row=end_row,
                    row_count=row_count,
                    status=ReservationStatus.ACTIVE,
                    reserved_at=datetime.now()
                )

                self.row_reservations[reservation_id] = reservation

                return OperationResult(
                    success=True,
                    message=f'Reserved {row_count} rows in sheet "{sheet_name}"',
                    data={
                        'reservation_id': reservation_id,
                        'agent_id': agent_id,
                        'sheet_name': sheet_name,
                        'start_row': start_row,
                        'end_row': end_row,
                        'row_count': row_count
                    }
                )

        except Exception as e:
            return OperationResult(
                success=False,
                message='Error reserving rows',
                error=str(e)
            )

    def get_reservation(self, reservation_id: str) -> Optional[ReservationInfo]:
        """
        Get a reservation by its ID.

        Args:
            reservation_id: ID of the reservation

        Returns:
            ReservationInfo if found, None otherwise
        """
        return self.row_reservations.get(reservation_id)

    def complete_reservation(self, reservation_id: str, records_written: int) -> bool:
        """
        Mark a reservation as completed.

        Args:
            reservation_id: ID of the reservation to complete
            records_written: Number of records that were actually written

        Returns:
            True if reservation was found and marked complete, False otherwise
        """
        reservation = self.row_reservations.get(reservation_id)
        if not reservation:
            return False

        reservation.status = ReservationStatus.COMPLETED
        reservation.completed_at = datetime.now()
        reservation.records_written = records_written

        # Update sheet row count tracking
        end_row = reservation.start_row + records_written - 1
        self.update_sheet_row_count(reservation.sheet_name, end_row)

        return True

    def update_sheet_row_count(self, sheet_name: str, new_count: int):
        """
        Update the tracked row count for a sheet.

        Args:
            sheet_name: Name of the sheet
            new_count: New row count
        """
        self.sheet_row_counts[sheet_name] = max(
            self.sheet_row_counts.get(sheet_name, 0),
            new_count
        )

    def get_write_lock(self) -> asyncio.Lock:
        """Get the write lock for synchronizing write operations."""
        return self._write_lock

    def get_active_reservations(self, sheet_name: Optional[str] = None) -> Dict[str, ReservationInfo]:
        """
        Get all active reservations, optionally filtered by sheet.

        Args:
            sheet_name: Optional sheet name to filter by

        Returns:
            Dictionary of active reservations
        """
        active_reservations = {
            res_id: res for res_id, res in self.row_reservations.items()
            if res.status == ReservationStatus.ACTIVE
        }

        if sheet_name:
            active_reservations = {
                res_id: res for res_id, res in active_reservations.items()
                if res.sheet_name == sheet_name
            }

        return active_reservations

    def get_operation_status(self) -> Dict[str, any]:
        """
        Get status information about concurrency operations.

        Returns:
            Dictionary with operation status information
        """
        return {
            'row_tracking': dict(self.sheet_row_counts),
            'active_reservations': {
                res_id: {
                    'agent_id': res.agent_id,
                    'sheet_name': res.sheet_name,
                    'row_range': f"{res.start_row}-{res.end_row}",
                    'status': res.status.value
                }
                for res_id, res in self.row_reservations.items()
                if res.status == ReservationStatus.ACTIVE
            },
            'total_reservations': len(self.row_reservations)
        }

    def cleanup_expired_reservations(self, max_age_seconds: int = 3600):
        """
        Clean up reservations older than max_age_seconds.

        Args:
            max_age_seconds: Maximum age of reservations to keep (default: 1 hour)
        """
        current_time = datetime.now()
        expired_ids = []

        for res_id, reservation in self.row_reservations.items():
            age_seconds = (current_time - reservation.reserved_at).total_seconds()
            if age_seconds > max_age_seconds:
                expired_ids.append(res_id)

        for res_id in expired_ids:
            if res_id in self.row_reservations:
                self.row_reservations[res_id].status = ReservationStatus.EXPIRED

    def reset_sheet_tracking(self, sheet_name: str):
        """
        Reset tracking for a specific sheet.

        Args:
            sheet_name: Name of the sheet to reset
        """
        if sheet_name in self.sheet_row_counts:
            del self.sheet_row_counts[sheet_name]

        # Cancel active reservations for this sheet
        for reservation in self.row_reservations.values():
            if (reservation.sheet_name == sheet_name and
                    reservation.status == ReservationStatus.ACTIVE):
                reservation.status = ReservationStatus.EXPIRED
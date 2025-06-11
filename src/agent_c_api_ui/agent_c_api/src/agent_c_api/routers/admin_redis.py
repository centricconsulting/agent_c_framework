"""
Redis Admin API Router

Provides comprehensive admin endpoints for Redis Streams management and monitoring.
Includes status monitoring, health checks, mode control, session management,
and administrative operations with proper authentication and authorization.
"""

import asyncio
import logging
from datetime import datetime
from typing import Any, Dict, List, Optional
from uuid import uuid4

from fastapi import APIRouter, Depends, HTTPException, Query, Path, Body, Request, status
from fastapi.responses import StreamingResponse
from sse_starlette.sse import EventSourceResponse

from ..schemas.redis_admin import (
    # Response models
    RedisStatusResponse, RedisHealthResponse, RedisModeResponse,
    RedisMetricsResponse, RedisDiagnosticsResponse, RedisSessionsResponse,
    RedisSessionInfo, RedisEventsResponse, RedisAlertsResponse,
    OperationResult, PermissionInfo, AuthValidationResponse, AuditLogResponse,
    
    # Request models  
    ModeChangeRequest, ConfigUpdateRequest, SessionMigrationRequest,
    
    # Enums
    RedisOperationMode, RedisHealthStatus, ModeState, AlertLevel
)
from ..middleware.redis_admin_auth import (
    require_read_access, require_admin_access, require_super_admin_access,
    rate_limit, audit_log
)
from ..core.util.redis_runtime_controller import RedisRuntimeController
from ..core.util.redis_control_auth import ControllerPermission

logger = logging.getLogger(__name__)

# Initialize router with proper tags and responses
router = APIRouter(
    prefix="/admin/redis",
    tags=["Redis Admin"],
    responses={
        401: {
            "description": "Authentication Required",
            "content": {
                "application/json": {
                    "example": {"detail": "Authentication credentials required"}
                }
            }
        },
        403: {
            "description": "Insufficient Permissions",
            "content": {
                "application/json": {
                    "example": {"detail": "Admin privileges required"}
                }
            }
        },
        429: {
            "description": "Rate Limit Exceeded",
            "content": {
                "application/json": {
                    "example": {"detail": "Rate limit exceeded: 60 requests per 60 seconds"}
                }
            }
        },
        500: {
            "description": "Internal Server Error",
            "content": {
                "application/json": {
                    "example": {"detail": "Internal server error"}
                }
            }
        }
    }
)

# Global runtime controller instance
runtime_controller: Optional[RedisRuntimeController] = None


async def get_runtime_controller() -> RedisRuntimeController:
    """Get or initialize the Redis runtime controller."""
    global runtime_controller
    
    if not runtime_controller:
        try:
            runtime_controller = RedisRuntimeController()
            await runtime_controller.initialize()
            logger.info("Redis runtime controller initialized")
        except Exception as e:
            logger.error(f"Failed to initialize runtime controller: {e}")
            raise HTTPException(
                status_code=status.HTTP_503_SERVICE_UNAVAILABLE,
                detail="Redis runtime controller unavailable"
            )
    
    return runtime_controller


# Status Endpoints (Read-Only)

@router.get("/status", response_model=RedisStatusResponse, summary="Get Redis System Status")
@rate_limit(max_requests=120, window_seconds=60)
async def get_redis_status(
    user_info: Dict[str, Any] = Depends(require_read_access),
    controller: RedisRuntimeController = Depends(get_runtime_controller)
) -> RedisStatusResponse:
    """
    Get overall Redis system status including operation mode, health, and connection info.
    
    This endpoint provides a comprehensive overview of the Redis system state,
    including current operation mode, health status, connection information,
    and basic configuration details.
    
    **Permissions Required:** Read access
    """
    try:
        # Get current status from runtime controller
        current_mode = await controller.get_current_mode()
        health_status = await controller.get_health_status()
        
        # Build connection info
        connection_info = {
            "host": health_status.get("connection", {}).get("host", "localhost"),
            "port": health_status.get("connection", {}).get("port", 6379),
            "connected": health_status.get("connected", False),
            "database": 0,
            "pool_size": health_status.get("connection", {}).get("pool_size"),
            "active_connections": health_status.get("connection", {}).get("active_connections")
        }
        
        # Get mode change history
        mode_history = await controller.get_mode_history(limit=1)
        last_change = mode_history[0] if mode_history else None
        
        return RedisStatusResponse(
            operation_mode=current_mode["mode"],
            mode_state=current_mode["state"],
            health_status=health_status.get("overall_status", "unknown"),
            connection=connection_info,
            auto_switching_enabled=current_mode.get("auto_switching_enabled", False),
            last_mode_change=last_change.get("timestamp") if last_change else None,
            mode_change_reason=last_change.get("reason") if last_change else None,
            uptime_seconds=health_status.get("uptime_seconds", 0.0),
            timestamp=datetime.now()
        )
        
    except Exception as e:
        logger.error(f"Failed to get Redis status: {e}")
        raise HTTPException(
            status_code=status.HTTP_500_INTERNAL_SERVER_ERROR,
            detail=f"Failed to retrieve Redis status: {str(e)}"
        )


@router.get("/health", response_model=RedisHealthResponse, summary="Get Detailed Health Metrics")
@rate_limit(max_requests=120, window_seconds=60)
async def get_redis_health(
    include_history: bool = Query(False, description="Include performance and error history"),
    user_info: Dict[str, Any] = Depends(require_read_access),
    controller: RedisRuntimeController = Depends(get_runtime_controller)
) -> RedisHealthResponse:
    """
    Get detailed Redis health metrics including circuit breaker status,
    failure counts, and optional performance history.
    
    **Permissions Required:** Read access
    """
    try:
        health_data = await controller.get_health_status()
        
        # Build health metrics
        metrics = []
        for metric_name, metric_data in health_data.get("metrics", {}).items():
            if isinstance(metric_data, dict):
                metrics.append({
                    "name": metric_name,
                    "value": metric_data.get("value", 0),
                    "unit": metric_data.get("unit"),
                    "status": metric_data.get("status", "unknown"),
                    "last_updated": metric_data.get("timestamp", datetime.now()),
                    "threshold_warning": metric_data.get("threshold_warning"),
                    "threshold_critical": metric_data.get("threshold_critical")
                })
        
        # Get error and performance history if requested
        error_history = []
        performance_history = []
        
        if include_history:
            try:
                error_log = await controller.get_error_log(limit=10)
                error_history = [
                    {
                        "timestamp": entry.get("timestamp"),
                        "error": entry.get("error"),
                        "operation": entry.get("operation"),
                        "severity": entry.get("severity", "error")
                    }
                    for entry in error_log
                ]
                
                perf_metrics = await controller.get_performance_metrics()
                performance_history = perf_metrics.get("historical_data", [])
                
            except Exception as e:
                logger.warning(f"Failed to get health history: {e}")
        
        return RedisHealthResponse(
            overall_status=health_data.get("overall_status", "unknown"),
            metrics=metrics,
            circuit_breaker_open=health_data.get("circuit_breaker", {}).get("open", False),
            last_health_check=health_data.get("last_check", datetime.now()),
            health_check_interval=health_data.get("check_interval", 30),
            failure_count=health_data.get("failure_count", 0),
            recovery_count=health_data.get("recovery_count", 0),
            failure_threshold=health_data.get("failure_threshold", 3),
            recovery_threshold=health_data.get("recovery_threshold", 5),
            error_history=error_history,
            performance_history=performance_history
        )
        
    except Exception as e:
        logger.error(f"Failed to get Redis health: {e}")
        raise HTTPException(
            status_code=status.HTTP_500_INTERNAL_SERVER_ERROR,
            detail=f"Failed to retrieve Redis health: {str(e)}"
        )


@router.get("/mode", response_model=RedisModeResponse, summary="Get Operation Mode Details")
@rate_limit(max_requests=120, window_seconds=60)
async def get_redis_mode(
    include_history: bool = Query(False, description="Include mode change history"),
    history_limit: int = Query(10, ge=1, le=100, description="Maximum history entries"),
    user_info: Dict[str, Any] = Depends(require_read_access),
    controller: RedisRuntimeController = Depends(get_runtime_controller)
) -> RedisModeResponse:
    """
    Get current operation mode and configuration with optional change history.
    
    **Permissions Required:** Read access
    """
    try:
        current_mode = await controller.get_current_mode()
        available_modes = await controller.get_available_modes()
        
        # Get mode history if requested
        mode_history = []
        if include_history:
            history_data = await controller.get_mode_history(limit=history_limit)
            mode_history = [
                {
                    "mode": entry.get("mode"),
                    "timestamp": entry.get("timestamp"),
                    "reason": entry.get("reason"),
                    "user": entry.get("user", "system"),
                    "duration_seconds": entry.get("duration_seconds")
                }
                for entry in history_data
            ]
        
        # Get configuration details
        configuration = {
            "failure_threshold": current_mode.get("failure_threshold"),
            "recovery_threshold": current_mode.get("recovery_threshold"),
            "health_check_interval": current_mode.get("health_check_interval"),
            "auto_switching_enabled": current_mode.get("auto_switching_enabled"),
            "circuit_breaker_timeout": current_mode.get("circuit_breaker_timeout")
        }
        
        return RedisModeResponse(
            current_mode=current_mode["mode"],
            mode_state=current_mode["state"],
            available_modes=[mode["mode"] for mode in available_modes],
            auto_switching_enabled=current_mode.get("auto_switching_enabled", False),
            mode_history=mode_history,
            configuration=configuration,
            next_scheduled_check=current_mode.get("next_check")
        )
        
    except Exception as e:
        logger.error(f"Failed to get Redis mode: {e}")
        raise HTTPException(
            status_code=status.HTTP_500_INTERNAL_SERVER_ERROR,
            detail=f"Failed to retrieve Redis mode: {str(e)}"
        )


@router.get("/metrics", response_model=RedisMetricsResponse, summary="Get Performance Metrics")
@rate_limit(max_requests=120, window_seconds=60)
async def get_redis_metrics(
    include_history: bool = Query(False, description="Include historical metrics"),
    history_hours: int = Query(1, ge=1, le=24, description="Hours of history to include"),
    user_info: Dict[str, Any] = Depends(require_read_access),
    controller: RedisRuntimeController = Depends(get_runtime_controller)
) -> RedisMetricsResponse:
    """
    Get Redis performance metrics and statistics with optional historical data.
    
    **Permissions Required:** Read access
    """
    try:
        metrics_data = await controller.get_performance_metrics()
        
        current_metrics = {
            "operations_per_second": metrics_data.get("ops_per_second", 0.0),
            "average_response_time": metrics_data.get("avg_response_time", 0.0),
            "peak_response_time": metrics_data.get("peak_response_time", 0.0),
            "memory_usage_mb": metrics_data.get("memory_usage_mb", 0.0),
            "cpu_usage_percent": metrics_data.get("cpu_usage_percent", 0.0),
            "active_connections": metrics_data.get("active_connections", 0),
            "total_operations": metrics_data.get("total_operations", 0),
            "error_rate_percent": metrics_data.get("error_rate_percent", 0.0),
            "cache_hit_rate_percent": metrics_data.get("cache_hit_rate_percent"),
            "collection_period_seconds": metrics_data.get("collection_period", 60)
        }
        
        # Get historical data if requested
        historical_data = []
        if include_history:
            historical_data = metrics_data.get("historical_data", [])
            # Filter by time if needed
            cutoff_time = datetime.now().timestamp() - (history_hours * 3600)
            historical_data = [
                entry for entry in historical_data
                if entry.get("timestamp", 0) >= cutoff_time
            ]
        
        return RedisMetricsResponse(
            current_metrics=current_metrics,
            historical_data=historical_data,
            metric_collection_enabled=metrics_data.get("collection_enabled", True),
            last_updated=datetime.now()
        )
        
    except Exception as e:
        logger.error(f"Failed to get Redis metrics: {e}")
        raise HTTPException(
            status_code=status.HTTP_500_INTERNAL_SERVER_ERROR,
            detail=f"Failed to retrieve Redis metrics: {str(e)}"
        )


@router.get("/diagnostics", response_model=RedisDiagnosticsResponse, summary="Export Diagnostic Data")
@rate_limit(max_requests=30, window_seconds=60)
async def get_redis_diagnostics(
    include_config: bool = Query(True, description="Include configuration details"),
    include_logs: bool = Query(False, description="Include recent log entries"),
    user_info: Dict[str, Any] = Depends(require_read_access),
    controller: RedisRuntimeController = Depends(get_runtime_controller)
) -> RedisDiagnosticsResponse:
    """
    Export comprehensive diagnostic data for troubleshooting and analysis.
    
    **Permissions Required:** Read access
    **Rate Limited:** 30 requests per minute (resource intensive)
    """
    try:
        diagnostic_data = await controller.export_diagnostics()
        
        # Filter sensitive information if user doesn't have admin access
        if user_info.get("role") not in ["ADMIN", "SUPER_ADMIN"]:
            # Remove sensitive configuration details
            if "configuration" in diagnostic_data:
                sensitive_keys = ["password", "secret", "key", "token"]
                config = diagnostic_data["configuration"]
                for key in list(config.keys()):
                    if any(sensitive in key.lower() for sensitive in sensitive_keys):
                        config[key] = "***REDACTED***"
        
        warnings = []
        if not include_config and "configuration" in diagnostic_data:
            del diagnostic_data["configuration"]
            warnings.append("Configuration details excluded")
        
        if not include_logs and "logs" in diagnostic_data:
            del diagnostic_data["logs"]
            warnings.append("Log entries excluded")
        
        return RedisDiagnosticsResponse(
            diagnostic_data=diagnostic_data,
            export_timestamp=datetime.now(),
            export_version="1.0",
            warnings=warnings
        )
        
    except Exception as e:
        logger.error(f"Failed to export diagnostics: {e}")
        raise HTTPException(
            status_code=status.HTTP_500_INTERNAL_SERVER_ERROR,
            detail=f"Failed to export diagnostics: {str(e)}"
        )


# Control Endpoints (Admin-Only)

@router.post("/mode", response_model=OperationResult, summary="Change Operation Mode")
@rate_limit(max_requests=30, window_seconds=60)
@audit_log(operation="mode_change", resource="redis_mode")
async def change_redis_mode(
    request_data: ModeChangeRequest,
    user_info: Dict[str, Any] = Depends(require_admin_access),
    controller: RedisRuntimeController = Depends(get_runtime_controller)
) -> OperationResult:
    """
    Change Redis operation mode with validation and optional force override.
    
    **Permissions Required:** Admin access
    **Rate Limited:** 30 requests per minute
    **Audited:** All mode changes are logged
    """
    try:
        operation_id = str(uuid4())
        
        # Validate mode change request
        current_mode = await controller.get_current_mode()
        if current_mode["mode"] == request_data.target_mode.value:
            return OperationResult(
                success=True,
                message=f"Already in {request_data.target_mode.value} mode",
                operation_id=operation_id,
                details={"no_change_required": True}
            )
        
        # Perform mode change
        result = await controller.set_operation_mode(
            mode=request_data.target_mode.value,
            force=request_data.force,
            timeout_seconds=request_data.timeout_seconds,
            reason=request_data.reason or f"Manual change by {user_info.get('user_id')}",
            user_id=user_info.get("user_id")
        )
        
        if result.get("success", False):
            return OperationResult(
                success=True,
                message=f"Mode changed to {request_data.target_mode.value}",
                operation_id=operation_id,
                details={
                    "old_mode": current_mode["mode"],
                    "new_mode": request_data.target_mode.value,
                    "forced": request_data.force,
                    "transition_time_seconds": result.get("transition_time")
                },
                warnings=result.get("warnings", [])
            )
        else:
            raise HTTPException(
                status_code=status.HTTP_400_BAD_REQUEST,
                detail=result.get("error", "Mode change failed")
            )
        
    except HTTPException:
        raise
    except Exception as e:
        logger.error(f"Failed to change Redis mode: {e}")
        raise HTTPException(
            status_code=status.HTTP_500_INTERNAL_SERVER_ERROR,
            detail=f"Mode change failed: {str(e)}"
        )


@router.post("/health/check", response_model=OperationResult, summary="Trigger Health Check")
@rate_limit(max_requests=60, window_seconds=60)
@audit_log(operation="health_check", resource="redis_health")
async def trigger_health_check(
    force: bool = Query(False, description="Force health check even if recently completed"),
    user_info: Dict[str, Any] = Depends(require_admin_access),
    controller: RedisRuntimeController = Depends(get_runtime_controller)
) -> OperationResult:
    """
    Trigger immediate Redis health check.
    
    **Permissions Required:** Admin access
    **Rate Limited:** 60 requests per minute
    """
    try:
        operation_id = str(uuid4())
        
        result = await controller.trigger_health_check(force=force)
        
        return OperationResult(
            success=True,
            message="Health check completed",
            operation_id=operation_id,
            details={
                "health_status": result.get("status"),
                "response_time_ms": result.get("response_time"),
                "forced": force,
                "check_timestamp": result.get("timestamp")
            }
        )
        
    except Exception as e:
        logger.error(f"Health check failed: {e}")
        raise HTTPException(
            status_code=status.HTTP_500_INTERNAL_SERVER_ERROR,
            detail=f"Health check failed: {str(e)}"
        )


@router.post("/circuit-breaker/reset", response_model=OperationResult, summary="Reset Circuit Breaker")
@rate_limit(max_requests=30, window_seconds=60)
@audit_log(operation="circuit_breaker_reset", resource="redis_circuit_breaker")
async def reset_circuit_breaker(
    user_info: Dict[str, Any] = Depends(require_admin_access),
    controller: RedisRuntimeController = Depends(get_runtime_controller)
) -> OperationResult:
    """
    Reset Redis circuit breaker state.
    
    **Permissions Required:** Admin access
    **Rate Limited:** 30 requests per minute (potentially disruptive)
    """
    try:
        operation_id = str(uuid4())
        
        result = await controller.reset_circuit_breaker()
        
        return OperationResult(
            success=True,
            message="Circuit breaker reset successfully",
            operation_id=operation_id,
            details={
                "previous_state": result.get("previous_state"),
                "reset_timestamp": result.get("timestamp")
            }
        )
        
    except Exception as e:
        logger.error(f"Circuit breaker reset failed: {e}")
        raise HTTPException(
            status_code=status.HTTP_500_INTERNAL_SERVER_ERROR,
            detail=f"Circuit breaker reset failed: {str(e)}"
        )


@router.put("/config", response_model=OperationResult, summary="Update Runtime Configuration")
@rate_limit(max_requests=20, window_seconds=60)
@audit_log(operation="config_update", resource="redis_config")
async def update_redis_config(
    config_request: ConfigUpdateRequest,
    user_info: Dict[str, Any] = Depends(require_admin_access),
    controller: RedisRuntimeController = Depends(get_runtime_controller)
) -> OperationResult:
    """
    Update Redis runtime configuration.
    
    **Permissions Required:** Admin access
    **Rate Limited:** 20 requests per minute (potentially disruptive)
    **Audited:** All configuration changes are logged
    """
    try:
        operation_id = str(uuid4())
        
        # Validate configuration before applying
        if config_request.validate_only:
            # Perform validation only
            validation_result = await controller.validate_configuration(
                config_request.configuration
            )
            
            return OperationResult(
                success=validation_result.get("valid", False),
                message="Configuration validation completed",
                operation_id=operation_id,
                details={
                    "validation_only": True,
                    "valid": validation_result.get("valid"),
                    "errors": validation_result.get("errors", []),
                    "warnings": validation_result.get("warnings", [])
                }
            )
        
        # Apply configuration changes
        result = await controller.update_configuration(
            config_request.configuration,
            user_id=user_info.get("user_id")
        )
        
        if result.get("success", False):
            return OperationResult(
                success=True,
                message="Configuration updated successfully",
                operation_id=operation_id,
                details={
                    "applied_changes": result.get("applied_changes"),
                    "restart_required": result.get("restart_required", False)
                },
                warnings=result.get("warnings", [])
            )
        else:
            raise HTTPException(
                status_code=status.HTTP_400_BAD_REQUEST,
                detail=result.get("error", "Configuration update failed")
            )
        
    except HTTPException:
        raise
    except Exception as e:
        logger.error(f"Configuration update failed: {e}")
        raise HTTPException(
            status_code=status.HTTP_500_INTERNAL_SERVER_ERROR,
            detail=f"Configuration update failed: {str(e)}"
        )


@router.post("/maintenance", response_model=OperationResult, summary="Enter/Exit Maintenance Mode")
@rate_limit(max_requests=10, window_seconds=60)
@audit_log(operation="maintenance_mode", resource="redis_system")
async def toggle_maintenance_mode(
    enable: bool = Query(..., description="Enable or disable maintenance mode"),
    reason: Optional[str] = Query(None, description="Reason for maintenance mode"),
    user_info: Dict[str, Any] = Depends(require_super_admin_access),
    controller: RedisRuntimeController = Depends(get_runtime_controller)
) -> OperationResult:
    """
    Enter or exit Redis maintenance mode.
    
    **Permissions Required:** Super admin access
    **Rate Limited:** 10 requests per minute (highly disruptive)
    """
    try:
        operation_id = str(uuid4())
        
        result = await controller.set_maintenance_mode(
            enabled=enable,
            reason=reason or f"Manual toggle by {user_info.get('user_id')}",
            user_id=user_info.get("user_id")
        )
        
        action = "entered" if enable else "exited"
        return OperationResult(
            success=True,
            message=f"Maintenance mode {action}",
            operation_id=operation_id,
            details={
                "maintenance_enabled": enable,
                "reason": reason,
                "timestamp": result.get("timestamp")
            }
        )
        
    except Exception as e:
        logger.error(f"Maintenance mode toggle failed: {e}")
        raise HTTPException(
            status_code=status.HTTP_500_INTERNAL_SERVER_ERROR,
            detail=f"Maintenance mode operation failed: {str(e)}"
        )


# Session-Specific Endpoints

@router.get("/sessions", response_model=RedisSessionsResponse, summary="List Active Sessions")
@rate_limit(max_requests=120, window_seconds=60)
async def list_redis_sessions(
    mode_filter: Optional[RedisOperationMode] = Query(None, description="Filter by operation mode"),
    limit: int = Query(50, ge=1, le=500, description="Maximum sessions to return"),
    user_info: Dict[str, Any] = Depends(require_read_access),
    controller: RedisRuntimeController = Depends(get_runtime_controller)
) -> RedisSessionsResponse:
    """
    List active Redis sessions with optional filtering by operation mode.
    
    **Permissions Required:** Read access
    """
    try:
        sessions_data = await controller.get_active_sessions(
            mode_filter=mode_filter.value if mode_filter else None,
            limit=limit
        )
        
        sessions = []
        by_mode = {}
        
        for session_data in sessions_data.get("sessions", []):
            session_info = RedisSessionInfo(
                session_id=session_data.get("session_id"),
                operation_mode=session_data.get("operation_mode"),
                created_at=session_data.get("created_at"),
                last_activity=session_data.get("last_activity"),
                message_count=session_data.get("message_count", 0),
                status=session_data.get("status", "unknown"),
                user_id=session_data.get("user_id"),
                metadata=session_data.get("metadata", {})
            )
            sessions.append(session_info)
            
            # Count by mode
            mode = session_info.operation_mode
            by_mode[mode] = by_mode.get(mode, 0) + 1
        
        return RedisSessionsResponse(
            sessions=sessions,
            total_count=sessions_data.get("total_count", len(sessions)),
            active_count=len([s for s in sessions if s.status == "active"]),
            by_mode=by_mode,
            timestamp=datetime.now()
        )
        
    except Exception as e:
        logger.error(f"Failed to list sessions: {e}")
        raise HTTPException(
            status_code=status.HTTP_500_INTERNAL_SERVER_ERROR,
            detail=f"Failed to retrieve sessions: {str(e)}"
        )


@router.get("/sessions/{session_id}", response_model=RedisSessionInfo, summary="Get Session Details")
@rate_limit(max_requests=120, window_seconds=60)
async def get_session_details(
    session_id: str = Path(..., description="Session ID"),
    user_info: Dict[str, Any] = Depends(require_read_access),
    controller: RedisRuntimeController = Depends(get_runtime_controller)
) -> RedisSessionInfo:
    """
    Get detailed information about a specific Redis session.
    
    **Permissions Required:** Read access
    """
    try:
        session_data = await controller.get_session_details(session_id)
        
        if not session_data:
            raise HTTPException(
                status_code=status.HTTP_404_NOT_FOUND,
                detail=f"Session {session_id} not found"
            )
        
        return RedisSessionInfo(
            session_id=session_data.get("session_id"),
            operation_mode=session_data.get("operation_mode"),
            created_at=session_data.get("created_at"),
            last_activity=session_data.get("last_activity"),
            message_count=session_data.get("message_count", 0),
            status=session_data.get("status", "unknown"),
            user_id=session_data.get("user_id"),
            metadata=session_data.get("metadata", {})
        )
        
    except HTTPException:
        raise
    except Exception as e:
        logger.error(f"Failed to get session details: {e}")
        raise HTTPException(
            status_code=status.HTTP_500_INTERNAL_SERVER_ERROR,
            detail=f"Failed to retrieve session details: {str(e)}"
        )


@router.post("/sessions/{session_id}/migrate", response_model=OperationResult, summary="Migrate Session")
@rate_limit(max_requests=30, window_seconds=60)
@audit_log(operation="session_migrate", resource="redis_session")
async def migrate_session(
    session_id: str = Path(..., description="Session ID"),
    migration_request: SessionMigrationRequest = Body(...),
    user_info: Dict[str, Any] = Depends(require_admin_access),
    controller: RedisRuntimeController = Depends(get_runtime_controller)
) -> OperationResult:
    """
    Migrate session between operation modes.
    
    **Permissions Required:** Admin access
    **Rate Limited:** 30 requests per minute
    """
    try:
        operation_id = str(uuid4())
        
        result = await controller.migrate_session(
            session_id=session_id,
            target_mode=migration_request.target_mode.value,
            preserve_state=migration_request.preserve_state,
            timeout_seconds=migration_request.timeout_seconds,
            user_id=user_info.get("user_id")
        )
        
        if result.get("success", False):
            return OperationResult(
                success=True,
                message=f"Session migrated to {migration_request.target_mode.value}",
                operation_id=operation_id,
                details={
                    "session_id": session_id,
                    "target_mode": migration_request.target_mode.value,
                    "state_preserved": migration_request.preserve_state,
                    "migration_time_seconds": result.get("migration_time")
                }
            )
        else:
            raise HTTPException(
                status_code=status.HTTP_400_BAD_REQUEST,
                detail=result.get("error", "Session migration failed")
            )
        
    except HTTPException:
        raise
    except Exception as e:
        logger.error(f"Session migration failed: {e}")
        raise HTTPException(
            status_code=status.HTTP_500_INTERNAL_SERVER_ERROR,
            detail=f"Session migration failed: {str(e)}"
        )


@router.delete("/sessions/{session_id}", response_model=OperationResult, summary="Terminate Session")
@rate_limit(max_requests=60, window_seconds=60)
@audit_log(operation="session_terminate", resource="redis_session")
async def terminate_session(
    session_id: str = Path(..., description="Session ID"),
    graceful: bool = Query(True, description="Perform graceful shutdown"),
    user_info: Dict[str, Any] = Depends(require_admin_access),
    controller: RedisRuntimeController = Depends(get_runtime_controller)
) -> OperationResult:
    """
    Gracefully terminate a Redis session.
    
    **Permissions Required:** Admin access
    **Rate Limited:** 60 requests per minute
    """
    try:
        operation_id = str(uuid4())
        
        result = await controller.terminate_session(
            session_id=session_id,
            graceful=graceful,
            user_id=user_info.get("user_id")
        )
        
        return OperationResult(
            success=True,
            message=f"Session terminated {'gracefully' if graceful else 'forcefully'}",
            operation_id=operation_id,
            details={
                "session_id": session_id,
                "graceful": graceful,
                "termination_time": result.get("timestamp")
            }
        )
        
    except Exception as e:
        logger.error(f"Session termination failed: {e}")
        raise HTTPException(
            status_code=status.HTTP_500_INTERNAL_SERVER_ERROR,
            detail=f"Session termination failed: {str(e)}"
        )


# Monitoring and Events

@router.get("/events", response_model=RedisEventsResponse, summary="Get Recent Events")
@rate_limit(max_requests=120, window_seconds=60)
async def get_redis_events(
    limit: int = Query(50, ge=1, le=500, description="Maximum events to return"),
    level_filter: Optional[AlertLevel] = Query(None, description="Filter by alert level"),
    event_type: Optional[str] = Query(None, description="Filter by event type"),
    since: Optional[datetime] = Query(None, description="Only events after this timestamp"),
    user_info: Dict[str, Any] = Depends(require_read_access),
    controller: RedisRuntimeController = Depends(get_runtime_controller)
) -> RedisEventsResponse:
    """
    Get recent Redis events and mode changes with optional filtering.
    
    **Permissions Required:** Read access
    """
    try:
        events_data = await controller.get_events(
            limit=limit,
            level_filter=level_filter.value if level_filter else None,
            event_type=event_type,
            since=since
        )
        
        events = []
        unresolved_count = 0
        
        for event_data in events_data.get("events", []):
            event = {
                "event_id": event_data.get("event_id"),
                "event_type": event_data.get("event_type"),
                "level": event_data.get("level"),
                "message": event_data.get("message"),
                "timestamp": event_data.get("timestamp"),
                "details": event_data.get("details", {}),
                "resolved": event_data.get("resolved", False),
                "resolved_at": event_data.get("resolved_at")
            }
            events.append(event)
            
            if not event["resolved"]:
                unresolved_count += 1
        
        return RedisEventsResponse(
            events=events,
            total_count=events_data.get("total_count", len(events)),
            unresolved_count=unresolved_count,
            timestamp=datetime.now()
        )
        
    except Exception as e:
        logger.error(f"Failed to get events: {e}")
        raise HTTPException(
            status_code=status.HTTP_500_INTERNAL_SERVER_ERROR,
            detail=f"Failed to retrieve events: {str(e)}"
        )


@router.get("/events/stream", summary="Real-time Event Stream")
async def stream_redis_events(
    request: Request,
    level_filter: Optional[AlertLevel] = Query(None, description="Filter by alert level"),
    event_types: Optional[str] = Query(None, description="Comma-separated event types"),
    user_info: Dict[str, Any] = Depends(require_read_access),
    controller: RedisRuntimeController = Depends(get_runtime_controller)
):
    """
    Server-sent events stream for real-time Redis monitoring.
    
    **Permissions Required:** Read access
    **Note:** Use with EventSource API in browsers
    """
    async def event_generator():
        """Generate server-sent events."""
        try:
            # Parse event types filter
            types_filter = None
            if event_types:
                types_filter = [t.strip() for t in event_types.split(",")]
            
            # Get event stream from controller
            async for event in controller.get_event_stream(
                level_filter=level_filter.value if level_filter else None,
                event_types=types_filter
            ):
                # Check if client disconnected
                if await request.is_disconnected():
                    break
                
                # Format event for SSE
                event_data = {
                    "id": event.get("event_id"),
                    "type": event.get("event_type"),
                    "timestamp": event.get("timestamp").isoformat() if event.get("timestamp") else None,
                    "data": {
                        "level": event.get("level"),
                        "message": event.get("message"),
                        "details": event.get("details", {})
                    }
                }
                
                yield {
                    "id": event.get("event_id"),
                    "event": event.get("event_type"),
                    "data": event_data
                }
                
                # Small delay to prevent overwhelming clients
                await asyncio.sleep(0.1)
                
        except Exception as e:
            logger.error(f"Event stream error: {e}")
            yield {
                "event": "error",
                "data": {"error": str(e)}
            }
    
    return EventSourceResponse(event_generator())


@router.get("/alerts", response_model=RedisAlertsResponse, summary="Get Active Alerts")
@rate_limit(max_requests=120, window_seconds=60)
async def get_redis_alerts(
    level_filter: Optional[AlertLevel] = Query(None, description="Filter by alert level"),
    unacknowledged_only: bool = Query(False, description="Only unacknowledged alerts"),
    user_info: Dict[str, Any] = Depends(require_read_access),
    controller: RedisRuntimeController = Depends(get_runtime_controller)
) -> RedisAlertsResponse:
    """
    Get active Redis alerts and warnings.
    
    **Permissions Required:** Read access
    """
    try:
        alerts_data = await controller.get_alerts(
            level_filter=level_filter.value if level_filter else None,
            unacknowledged_only=unacknowledged_only
        )
        
        alerts = []
        unacknowledged_count = 0
        by_level = {}
        
        for alert_data in alerts_data.get("alerts", []):
            alert = {
                "alert_id": alert_data.get("alert_id"),
                "alert_type": alert_data.get("alert_type"),
                "level": alert_data.get("level"),
                "title": alert_data.get("title"),
                "description": alert_data.get("description"),
                "created_at": alert_data.get("created_at"),
                "acknowledged": alert_data.get("acknowledged", False),
                "acknowledged_by": alert_data.get("acknowledged_by"),
                "acknowledged_at": alert_data.get("acknowledged_at"),
                "auto_resolve": alert_data.get("auto_resolve", False),
                "metadata": alert_data.get("metadata", {})
            }
            alerts.append(alert)
            
            if not alert["acknowledged"]:
                unacknowledged_count += 1
            
            level = alert["level"]
            by_level[level] = by_level.get(level, 0) + 1
        
        return RedisAlertsResponse(
            alerts=alerts,
            total_count=len(alerts),
            unacknowledged_count=unacknowledged_count,
            by_level=by_level,
            timestamp=datetime.now()
        )
        
    except Exception as e:
        logger.error(f"Failed to get alerts: {e}")
        raise HTTPException(
            status_code=status.HTTP_500_INTERNAL_SERVER_ERROR,
            detail=f"Failed to retrieve alerts: {str(e)}"
        )


@router.post("/alerts/{alert_id}/acknowledge", response_model=OperationResult, summary="Acknowledge Alert")
@rate_limit(max_requests=120, window_seconds=60)
@audit_log(operation="alert_acknowledge", resource="redis_alert")
async def acknowledge_alert(
    alert_id: str = Path(..., description="Alert ID"),
    user_info: Dict[str, Any] = Depends(require_admin_access),
    controller: RedisRuntimeController = Depends(get_runtime_controller)
) -> OperationResult:
    """
    Acknowledge a Redis alert.
    
    **Permissions Required:** Admin access
    """
    try:
        operation_id = str(uuid4())
        
        result = await controller.acknowledge_alert(
            alert_id=alert_id,
            user_id=user_info.get("user_id")
        )
        
        if result.get("success", False):
            return OperationResult(
                success=True,
                message="Alert acknowledged",
                operation_id=operation_id,
                details={
                    "alert_id": alert_id,
                    "acknowledged_by": user_info.get("user_id"),
                    "acknowledged_at": result.get("timestamp")
                }
            )
        else:
            raise HTTPException(
                status_code=status.HTTP_404_NOT_FOUND,
                detail=f"Alert {alert_id} not found"
            )
        
    except HTTPException:
        raise
    except Exception as e:
        logger.error(f"Failed to acknowledge alert: {e}")
        raise HTTPException(
            status_code=status.HTTP_500_INTERNAL_SERVER_ERROR,
            detail=f"Failed to acknowledge alert: {str(e)}"
        )


# Security Controls

@router.get("/auth/permissions", response_model=PermissionInfo, summary="Get User Permissions")
@rate_limit(max_requests=120, window_seconds=60)
async def get_user_permissions(
    user_info: Dict[str, Any] = Depends(require_read_access)
) -> PermissionInfo:
    """
    Get current user's permissions and role information.
    
    **Permissions Required:** Read access (returns own permissions)
    """
    return PermissionInfo(
        user_id=user_info.get("user_id", "unknown"),
        role=user_info.get("role", "VIEWER"),
        permissions=user_info.get("permissions", []),
        expires_at=user_info.get("expires_at"),
        last_access=user_info.get("last_access")
    )


@router.post("/auth/validate", response_model=AuthValidationResponse, summary="Validate Auth Token")
@rate_limit(max_requests=240, window_seconds=60)
async def validate_auth_token(
    user_info: Dict[str, Any] = Depends(require_read_access)
) -> AuthValidationResponse:
    """
    Validate current authentication token.
    
    **Permissions Required:** Any valid token
    """
    expires_in = None
    if user_info.get("expires_at"):
        expires_at = user_info["expires_at"]
        if isinstance(expires_at, datetime):
            expires_in = int((expires_at - datetime.now()).total_seconds())
    
    return AuthValidationResponse(
        valid=True,
        user_info=PermissionInfo(
            user_id=user_info.get("user_id", "unknown"),
            role=user_info.get("role", "VIEWER"),
            permissions=user_info.get("permissions", []),
            expires_at=user_info.get("expires_at"),
            last_access=user_info.get("last_access")
        ),
        expires_in_seconds=expires_in
    )


@router.get("/audit", response_model=AuditLogResponse, summary="Get Audit Log")
@rate_limit(max_requests=60, window_seconds=60)
async def get_audit_log(
    limit: int = Query(50, ge=1, le=500, description="Maximum entries to return"),
    user_filter: Optional[str] = Query(None, description="Filter by user ID"),
    operation_filter: Optional[str] = Query(None, description="Filter by operation"),
    since: Optional[datetime] = Query(None, description="Only entries after this timestamp"),
    user_info: Dict[str, Any] = Depends(require_admin_access),
    controller: RedisRuntimeController = Depends(get_runtime_controller)
) -> AuditLogResponse:
    """
    Get Redis admin operation audit log.
    
    **Permissions Required:** Admin access
    **Rate Limited:** 60 requests per minute
    """
    try:
        if not controller.auth_manager:
            raise HTTPException(
                status_code=status.HTTP_503_SERVICE_UNAVAILABLE,
                detail="Audit logging service unavailable"
            )
        
        audit_data = controller.auth_manager.get_audit_log(
            limit=limit,
            user_filter=user_filter,
            operation_filter=operation_filter,
            since=since
        )
        
        entries = []
        for entry in audit_data.get("entries", []):
            entries.append({
                "timestamp": entry.get("timestamp"),
                "user_id": entry.get("user_id"),
                "operation": entry.get("operation"),
                "resource": entry.get("resource"),
                "details": entry.get("details", {}),
                "result": entry.get("result"),
                "ip_address": entry.get("ip_address")
            })
        
        return AuditLogResponse(
            entries=entries,
            total_count=audit_data.get("total_count", len(entries)),
            page=1,
            page_size=limit,
            timestamp=datetime.now()
        )
        
    except HTTPException:
        raise
    except Exception as e:
        logger.error(f"Failed to get audit log: {e}")
        raise HTTPException(
            status_code=status.HTTP_500_INTERNAL_SERVER_ERROR,
            detail=f"Failed to retrieve audit log: {str(e)}"
        )


@router.post("/emergency/lockdown", response_model=OperationResult, summary="Emergency System Lockdown")
@rate_limit(max_requests=5, window_seconds=60)
@audit_log(operation="emergency_lockdown", resource="redis_system")
async def emergency_lockdown(
    reason: Optional[str] = Query(None, description="Reason for lockdown"),
    user_info: Dict[str, Any] = Depends(require_super_admin_access),
    controller: RedisRuntimeController = Depends(get_runtime_controller)
) -> OperationResult:
    """
    Emergency system lockdown - disables all Redis operations.
    
    **Permissions Required:** Super admin access
    **Rate Limited:** 5 requests per minute (emergency use only)
    **WARNING:** This will disable all Redis functionality
    """
    try:
        operation_id = str(uuid4())
        
        result = await controller.emergency_lockdown(
            reason=reason or f"Emergency lockdown by {user_info.get('user_id')}",
            user_id=user_info.get("user_id")
        )
        
        logger.critical(
            f"EMERGENCY LOCKDOWN activated by {user_info.get('user_id')} "
            f"from {user_info.get('ip_address')} - Reason: {reason}"
        )
        
        return OperationResult(
            success=True,
            message="Emergency lockdown activated - all Redis operations disabled",
            operation_id=operation_id,
            details={
                "lockdown_activated": True,
                "reason": reason,
                "activated_by": user_info.get("user_id"),
                "timestamp": result.get("timestamp")
            },
            warnings=[
                "All Redis operations have been disabled",
                "Manual intervention required to restore service"
            ]
        )
        
    except Exception as e:
        logger.error(f"Emergency lockdown failed: {e}")
        raise HTTPException(
            status_code=status.HTTP_500_INTERNAL_SERVER_ERROR,
            detail=f"Emergency lockdown failed: {str(e)}"
        )
"""
Redis Admin API Response Models

Pydantic models for Redis admin API endpoints, providing structured responses
for status, health, metrics, and control operations.
"""

from datetime import datetime
from typing import Any, Dict, List, Optional, Union
from pydantic import BaseModel, Field, ConfigDict
from enum import Enum


class RedisOperationMode(str, Enum):
    """Redis operation modes."""
    REDIS_ONLY = "REDIS_ONLY"
    HYBRID = "HYBRID"
    ASYNC_ONLY = "ASYNC_ONLY"


class RedisHealthStatus(str, Enum):
    """Redis health status values."""
    HEALTHY = "healthy"
    DEGRADED = "degraded"
    UNHEALTHY = "unhealthy"
    ERROR = "error"


class ModeState(str, Enum):
    """Mode transition states."""
    ACTIVE = "active"
    TRANSITIONING = "transitioning"
    DEGRADED = "degraded"
    FAILED = "failed"


class AlertLevel(str, Enum):
    """Alert severity levels."""
    INFO = "info"
    WARNING = "warning"
    ERROR = "error"
    CRITICAL = "critical"


# Status Response Models

class RedisConnectionInfo(BaseModel):
    """Redis connection information."""
    host: str
    port: int
    connected: bool
    database: int = 0
    pool_size: Optional[int] = None
    active_connections: Optional[int] = None

    model_config = ConfigDict(
        json_schema_extra={
            "example": {
                "host": "localhost",
                "port": 6379,
                "connected": True,
                "database": 0,
                "pool_size": 10,
                "active_connections": 3
            }
        }
    )


class RedisStatusResponse(BaseModel):
    """Overall Redis system status."""
    operation_mode: RedisOperationMode
    mode_state: ModeState
    health_status: RedisHealthStatus
    connection: RedisConnectionInfo
    auto_switching_enabled: bool
    last_mode_change: Optional[datetime] = None
    mode_change_reason: Optional[str] = None
    uptime_seconds: float
    version: str = "2.0"
    timestamp: datetime = Field(default_factory=datetime.now)

    model_config = ConfigDict(
        json_schema_extra={
            "example": {
                "operation_mode": "HYBRID",
                "mode_state": "active",
                "health_status": "healthy",
                "connection": {
                    "host": "localhost",
                    "port": 6379,
                    "connected": True,
                    "database": 0,
                    "pool_size": 10,
                    "active_connections": 3
                },
                "auto_switching_enabled": True,
                "last_mode_change": "2024-01-15T10:30:00Z",
                "mode_change_reason": "Health check passed",
                "uptime_seconds": 3600.5,
                "version": "2.0",
                "timestamp": "2024-01-15T10:35:00Z"
            }
        }
    )


class HealthMetric(BaseModel):
    """Individual health metric."""
    name: str
    value: Union[float, int, str, bool]
    unit: Optional[str] = None
    status: RedisHealthStatus
    last_updated: datetime
    threshold_warning: Optional[float] = None
    threshold_critical: Optional[float] = None

    model_config = ConfigDict(
        json_schema_extra={
            "example": {
                "name": "response_time",
                "value": 15.5,
                "unit": "ms",
                "status": "healthy",
                "last_updated": "2024-01-15T10:35:00Z",
                "threshold_warning": 50.0,
                "threshold_critical": 100.0
            }
        }
    )


class RedisHealthResponse(BaseModel):
    """Detailed Redis health metrics and history."""
    overall_status: RedisHealthStatus
    metrics: List[HealthMetric]
    circuit_breaker_open: bool
    last_health_check: datetime
    health_check_interval: int
    failure_count: int
    recovery_count: int
    failure_threshold: int
    recovery_threshold: int
    error_history: List[Dict[str, Any]] = Field(default_factory=list)
    performance_history: List[Dict[str, Any]] = Field(default_factory=list)

    model_config = ConfigDict(
        json_schema_extra={
            "example": {
                "overall_status": "healthy",
                "metrics": [
                    {
                        "name": "response_time",
                        "value": 15.5,
                        "unit": "ms",
                        "status": "healthy",
                        "last_updated": "2024-01-15T10:35:00Z",
                        "threshold_warning": 50.0,
                        "threshold_critical": 100.0
                    }
                ],
                "circuit_breaker_open": False,
                "last_health_check": "2024-01-15T10:35:00Z",
                "health_check_interval": 30,
                "failure_count": 0,
                "recovery_count": 5,
                "failure_threshold": 3,
                "recovery_threshold": 5,
                "error_history": [],
                "performance_history": []
            }
        }
    )


class RedisModeResponse(BaseModel):
    """Current operation mode and configuration."""
    current_mode: RedisOperationMode
    mode_state: ModeState
    available_modes: List[RedisOperationMode]
    auto_switching_enabled: bool
    mode_history: List[Dict[str, Any]] = Field(default_factory=list)
    configuration: Dict[str, Any] = Field(default_factory=dict)
    next_scheduled_check: Optional[datetime] = None

    model_config = ConfigDict(
        json_schema_extra={
            "example": {
                "current_mode": "HYBRID",
                "mode_state": "active",
                "available_modes": ["REDIS_ONLY", "HYBRID", "ASYNC_ONLY"],
                "auto_switching_enabled": True,
                "mode_history": [
                    {
                        "mode": "HYBRID",
                        "timestamp": "2024-01-15T10:30:00Z",
                        "reason": "Manual switch",
                        "user": "admin"
                    }
                ],
                "configuration": {
                    "failure_threshold": 3,
                    "recovery_threshold": 5,
                    "health_check_interval": 30
                },
                "next_scheduled_check": "2024-01-15T10:36:00Z"
            }
        }
    )


class PerformanceMetrics(BaseModel):
    """Redis performance metrics."""
    operations_per_second: float
    average_response_time: float
    peak_response_time: float
    memory_usage_mb: float
    cpu_usage_percent: float
    active_connections: int
    total_operations: int
    error_rate_percent: float
    cache_hit_rate_percent: Optional[float] = None
    collection_period_seconds: int = 60

    model_config = ConfigDict(
        json_schema_extra={
            "example": {
                "operations_per_second": 150.5,
                "average_response_time": 15.2,
                "peak_response_time": 45.8,
                "memory_usage_mb": 256.7,
                "cpu_usage_percent": 12.5,
                "active_connections": 8,
                "total_operations": 54321,
                "error_rate_percent": 0.1,
                "cache_hit_rate_percent": 95.2,
                "collection_period_seconds": 60
            }
        }
    )


class RedisMetricsResponse(BaseModel):
    """Performance metrics and statistics."""
    current_metrics: PerformanceMetrics
    historical_data: List[Dict[str, Any]] = Field(default_factory=list)
    metric_collection_enabled: bool = True
    last_updated: datetime = Field(default_factory=datetime.now)

    model_config = ConfigDict(
        json_schema_extra={
            "example": {
                "current_metrics": {
                    "operations_per_second": 150.5,
                    "average_response_time": 15.2,
                    "peak_response_time": 45.8,
                    "memory_usage_mb": 256.7,
                    "cpu_usage_percent": 12.5,
                    "active_connections": 8,
                    "total_operations": 54321,
                    "error_rate_percent": 0.1,
                    "cache_hit_rate_percent": 95.2,
                    "collection_period_seconds": 60
                },
                "historical_data": [],
                "metric_collection_enabled": True,
                "last_updated": "2024-01-15T10:35:00Z"
            }
        }
    )


class DiagnosticData(BaseModel):
    """Comprehensive diagnostic information."""
    system_info: Dict[str, Any]
    redis_info: Dict[str, Any]
    configuration: Dict[str, Any]
    recent_events: List[Dict[str, Any]]
    performance_summary: Dict[str, Any]
    error_summary: Dict[str, Any]
    health_checks: List[Dict[str, Any]]

    model_config = ConfigDict(
        json_schema_extra={
            "example": {
                "system_info": {
                    "python_version": "3.11.0",
                    "platform": "linux",
                    "memory_mb": 1024
                },
                "redis_info": {
                    "version": "7.0.5",
                    "mode": "standalone",
                    "uptime": 86400
                },
                "configuration": {
                    "operation_mode": "HYBRID",
                    "auto_switching": True
                },
                "recent_events": [],
                "performance_summary": {},
                "error_summary": {},
                "health_checks": []
            }
        }
    )


class RedisDiagnosticsResponse(BaseModel):
    """Comprehensive diagnostic data export."""
    diagnostic_data: DiagnosticData
    export_timestamp: datetime = Field(default_factory=datetime.now)
    export_version: str = "1.0"
    warnings: List[str] = Field(default_factory=list)


# Session Response Models

class RedisSessionInfo(BaseModel):
    """Information about a Redis session."""
    session_id: str
    operation_mode: RedisOperationMode
    created_at: datetime
    last_activity: datetime
    message_count: int
    status: str
    user_id: Optional[str] = None
    metadata: Dict[str, Any] = Field(default_factory=dict)

    model_config = ConfigDict(
        json_schema_extra={
            "example": {
                "session_id": "session-123",
                "operation_mode": "HYBRID",
                "created_at": "2024-01-15T10:00:00Z",
                "last_activity": "2024-01-15T10:30:00Z",
                "message_count": 15,
                "status": "active",
                "user_id": "user-456",
                "metadata": {"project": "api-test"}
            }
        }
    )


class RedisSessionsResponse(BaseModel):
    """List of active Redis sessions."""
    sessions: List[RedisSessionInfo]
    total_count: int
    active_count: int
    by_mode: Dict[str, int] = Field(default_factory=dict)
    timestamp: datetime = Field(default_factory=datetime.now)


# Event and Alert Models

class RedisEvent(BaseModel):
    """Redis system event."""
    event_id: str
    event_type: str
    level: AlertLevel
    message: str
    timestamp: datetime
    details: Dict[str, Any] = Field(default_factory=dict)
    resolved: bool = False
    resolved_at: Optional[datetime] = None

    model_config = ConfigDict(
        json_schema_extra={
            "example": {
                "event_id": "evt-123",
                "event_type": "mode_change",
                "level": "info",
                "message": "Mode changed from REDIS_ONLY to HYBRID",
                "timestamp": "2024-01-15T10:30:00Z",
                "details": {
                    "old_mode": "REDIS_ONLY",
                    "new_mode": "HYBRID",
                    "reason": "Health check passed"
                },
                "resolved": True,
                "resolved_at": "2024-01-15T10:31:00Z"
            }
        }
    )


class RedisEventsResponse(BaseModel):
    """Recent events and mode changes."""
    events: List[RedisEvent]
    total_count: int
    unresolved_count: int
    timestamp: datetime = Field(default_factory=datetime.now)


class RedisAlert(BaseModel):
    """Active Redis alert."""
    alert_id: str
    alert_type: str
    level: AlertLevel
    title: str
    description: str
    created_at: datetime
    acknowledged: bool = False
    acknowledged_by: Optional[str] = None
    acknowledged_at: Optional[datetime] = None
    auto_resolve: bool = False
    metadata: Dict[str, Any] = Field(default_factory=dict)

    model_config = ConfigDict(
        json_schema_extra={
            "example": {
                "alert_id": "alert-123",
                "alert_type": "health_degraded",
                "level": "warning",
                "title": "Redis Health Degraded",
                "description": "Redis response time exceeds warning threshold",
                "created_at": "2024-01-15T10:30:00Z",
                "acknowledged": False,
                "acknowledged_by": None,
                "acknowledged_at": None,
                "auto_resolve": True,
                "metadata": {
                    "response_time": 75.5,
                    "threshold": 50.0
                }
            }
        }
    )


class RedisAlertsResponse(BaseModel):
    """Active alerts and warnings."""
    alerts: List[RedisAlert]
    total_count: int
    unacknowledged_count: int
    by_level: Dict[str, int] = Field(default_factory=dict)
    timestamp: datetime = Field(default_factory=datetime.now)


# Control Request Models

class ModeChangeRequest(BaseModel):
    """Request to change operation mode."""
    target_mode: RedisOperationMode
    force: bool = False
    reason: Optional[str] = None
    timeout_seconds: Optional[int] = Field(None, ge=1, le=300)

    model_config = ConfigDict(
        json_schema_extra={
            "example": {
                "target_mode": "HYBRID",
                "force": False,
                "reason": "Switching for testing",
                "timeout_seconds": 30
            }
        }
    )


class ConfigUpdateRequest(BaseModel):
    """Request to update runtime configuration."""
    configuration: Dict[str, Any]
    validate_only: bool = False
    restart_required: Optional[bool] = None

    model_config = ConfigDict(
        json_schema_extra={
            "example": {
                "configuration": {
                    "health_check_interval": 60,
                    "failure_threshold": 5
                },
                "validate_only": False,
                "restart_required": False
            }
        }
    )


class SessionMigrationRequest(BaseModel):
    """Request to migrate session between modes."""
    target_mode: RedisOperationMode
    preserve_state: bool = True
    timeout_seconds: Optional[int] = Field(None, ge=1, le=300)

    model_config = ConfigDict(
        json_schema_extra={
            "example": {
                "target_mode": "ASYNC_ONLY",
                "preserve_state": True,
                "timeout_seconds": 60
            }
        }
    )


# Success Response Models

class OperationResult(BaseModel):
    """Result of an admin operation."""
    success: bool
    message: str
    operation_id: Optional[str] = None
    timestamp: datetime = Field(default_factory=datetime.now)
    details: Dict[str, Any] = Field(default_factory=dict)
    warnings: List[str] = Field(default_factory=list)

    model_config = ConfigDict(
        json_schema_extra={
            "example": {
                "success": True,
                "message": "Operation completed successfully",
                "operation_id": "op-123",
                "timestamp": "2024-01-15T10:35:00Z",
                "details": {"mode_changed": True},
                "warnings": []
            }
        }
    )


# Security and Auth Models

class PermissionInfo(BaseModel):
    """User permission information."""
    user_id: str
    role: str
    permissions: List[str]
    expires_at: Optional[datetime] = None
    last_access: Optional[datetime] = None

    model_config = ConfigDict(
        json_schema_extra={
            "example": {
                "user_id": "admin",
                "role": "ADMIN",
                "permissions": ["read", "write", "control"],
                "expires_at": "2024-01-16T10:35:00Z",
                "last_access": "2024-01-15T10:30:00Z"
            }
        }
    )


class AuthValidationResponse(BaseModel):
    """Authentication validation result."""
    valid: bool
    user_info: Optional[PermissionInfo] = None
    error: Optional[str] = None
    expires_in_seconds: Optional[int] = None

    model_config = ConfigDict(
        json_schema_extra={
            "example": {
                "valid": True,
                "user_info": {
                    "user_id": "admin",
                    "role": "ADMIN",
                    "permissions": ["read", "write", "control"],
                    "expires_at": "2024-01-16T10:35:00Z",
                    "last_access": "2024-01-15T10:30:00Z"
                },
                "error": None,
                "expires_in_seconds": 3600
            }
        }
    )


class AuditLogEntry(BaseModel):
    """Audit log entry."""
    timestamp: datetime
    user_id: str
    operation: str
    resource: str
    details: Dict[str, Any]
    result: str
    ip_address: Optional[str] = None

    model_config = ConfigDict(
        json_schema_extra={
            "example": {
                "timestamp": "2024-01-15T10:35:00Z",
                "user_id": "admin",
                "operation": "mode_change",
                "resource": "redis_mode",
                "details": {"from": "REDIS_ONLY", "to": "HYBRID"},
                "result": "success",
                "ip_address": "192.168.1.100"
            }
        }
    )


class AuditLogResponse(BaseModel):
    """Audit log response."""
    entries: List[AuditLogEntry]
    total_count: int
    page: int = 1
    page_size: int = 50
    timestamp: datetime = Field(default_factory=datetime.now)
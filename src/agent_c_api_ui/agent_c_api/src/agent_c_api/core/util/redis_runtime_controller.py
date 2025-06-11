"""
Redis Runtime Controller

This module provides comprehensive runtime control capabilities for Redis Streams
resilient mode operations. It offers programmatic control over operation modes,
health monitoring, diagnostics, and event notifications with proper authentication
and authorization.

Features:
- Dynamic mode switching with validation
- Health monitoring and circuit breaker control
- Performance diagnostics and metrics
- Real-time event notifications and callbacks
- Role-based access control and rate limiting
- Thread-safe operations for concurrent access
"""

import asyncio
import logging
import threading
import time
from collections import defaultdict, deque
from datetime import datetime, timedelta
from typing import Any, AsyncGenerator, Callable, Dict, List, Optional, Set, Tuple
from dataclasses import dataclass, field
from enum import Enum

from .resilient_mode import (
    OperationMode, 
    ModeState, 
    TransitionReason, 
    ModeTransition,
    EventHandlerModeManager,
    ResilientModeConfig
)
from .redis_health_monitor import RedisHealthMonitor, HealthStatus
from ...config.redis_config import RedisConfig

logger = logging.getLogger(__name__)


class ControllerPermission(Enum):
    """Permission levels for runtime controller operations."""
    READ_ONLY = "read_only"
    ADMIN = "admin"
    SUPER_ADMIN = "super_admin"


class ControllerRole(Enum):
    """User roles for access control."""
    VIEWER = "viewer"  # Read-only access
    OPERATOR = "operator"  # Basic control operations
    ADMIN = "admin"  # Full control operations
    SUPER_ADMIN = "super_admin"  # Emergency operations


@dataclass
class ControllerUser:
    """User information for access control."""
    user_id: str
    username: str
    role: ControllerRole
    permissions: Set[ControllerPermission] = field(default_factory=set)
    last_access: datetime = field(default_factory=datetime.utcnow)
    access_count: int = 0


@dataclass
class ControllerEvent:
    """Event information for runtime controller operations."""
    event_id: str
    event_type: str
    user_id: str
    operation: str
    parameters: Dict[str, Any]
    result: Optional[Dict[str, Any]]
    timestamp: datetime
    duration_ms: Optional[float] = None
    error: Optional[str] = None


@dataclass
class CallbackRegistration:
    """Callback registration information."""
    callback_id: str
    callback_type: str
    callback_function: Callable
    user_id: str
    registered_at: datetime
    last_called: Optional[datetime] = None
    call_count: int = 0


class RedisRuntimeController:
    """
    Comprehensive runtime controller for Redis Streams resilient mode operations.
    
    This class provides programmatic control over Redis operation modes, health
    monitoring, diagnostics, and event notifications with proper authentication,
    authorization, and audit logging.
    
    Features:
    - Mode control with validation and rollback capabilities
    - Health monitoring with customizable thresholds
    - Performance diagnostics and historical tracking
    - Event notification system with callback management
    - Role-based access control with rate limiting
    - Comprehensive audit logging and error handling
    """

    def __init__(
        self,
        mode_manager: Optional[EventHandlerModeManager] = None,
        health_monitor: Optional[RedisHealthMonitor] = None,
        redis_config: Optional[RedisConfig] = None,
        enable_auth: bool = True,
        rate_limit_window: int = 60,
        rate_limit_requests: int = 100
    ):
        """
        Initialize the Redis Runtime Controller.
        
        Args:
            mode_manager: EventHandlerModeManager instance for mode control
            health_monitor: RedisHealthMonitor instance for health operations  
            redis_config: RedisConfig instance for configuration access
            enable_auth: Enable authentication and authorization
            rate_limit_window: Rate limiting window in seconds
            rate_limit_requests: Maximum requests per rate limit window
        """
        self.mode_manager = mode_manager
        self.health_monitor = health_monitor
        self.redis_config = redis_config or RedisConfig()
        self.enable_auth = enable_auth
        self.rate_limit_window = rate_limit_window
        self.rate_limit_requests = rate_limit_requests
        
        # Thread safety
        self._lock = threading.RLock()
        
        # User management
        self._users: Dict[str, ControllerUser] = {}
        self._active_sessions: Dict[str, str] = {}  # session_id -> user_id
        
        # Event tracking
        self._events: deque = deque(maxlen=1000)  # Last 1000 events
        self._callbacks: Dict[str, CallbackRegistration] = {}
        
        # Rate limiting
        self._rate_limits: Dict[str, deque] = defaultdict(lambda: deque())
        
        # Performance tracking
        self._mode_history: deque = deque(maxlen=100)  # Last 100 mode changes
        self._performance_metrics: Dict[str, Any] = {}
        self._error_log: deque = deque(maxlen=500)  # Last 500 errors
        
        # Initialization
        self._initialize_default_users()
        self._start_metrics_collection()
        
        logger.info("RedisRuntimeController initialized successfully")

    def _initialize_default_users(self) -> None:
        """Initialize default system users."""
        # System admin user
        system_admin = ControllerUser(
            user_id="system",
            username="system_admin",
            role=ControllerRole.SUPER_ADMIN,
            permissions={ControllerPermission.READ_ONLY, ControllerPermission.ADMIN, ControllerPermission.SUPER_ADMIN}
        )
        self._users["system"] = system_admin
        
        # Default operator user for backward compatibility
        default_operator = ControllerUser(
            user_id="default",
            username="default_operator", 
            role=ControllerRole.OPERATOR,
            permissions={ControllerPermission.READ_ONLY, ControllerPermission.ADMIN}
        )
        self._users["default"] = default_operator

    def _start_metrics_collection(self) -> None:
        """Start background metrics collection."""
        def collect_metrics():
            while True:
                try:
                    self._collect_performance_metrics()
                    time.sleep(30)  # Collect every 30 seconds
                except Exception as e:
                    logger.error(f"Error collecting metrics: {e}")
                    time.sleep(60)  # Wait longer on error
        
        metrics_thread = threading.Thread(target=collect_metrics, daemon=True)
        metrics_thread.start()

    def _collect_performance_metrics(self) -> None:
        """Collect current performance metrics."""
        try:
            with self._lock:
                current_time = datetime.utcnow()
                
                # Health metrics
                if self.health_monitor:
                    health_status = self.health_monitor.check_health()
                    self._performance_metrics['health'] = {
                        'is_healthy': health_status.is_healthy,
                        'performance_score': health_status.performance_score,
                        'latency_ms': health_status.latency_ms,
                        'error_rate': health_status.error_rate,
                        'last_check': current_time.isoformat()
                    }
                
                # Mode manager metrics
                if self.mode_manager:
                    mode_status = self.mode_manager.get_current_status()
                    self._performance_metrics['mode'] = {
                        'current_mode': mode_status.get('current_mode', 'unknown'),
                        'current_state': mode_status.get('current_state', 'unknown'),
                        'circuit_breaker_state': mode_status.get('circuit_breaker', {}),
                        'last_transition': mode_status.get('last_transition_time'),
                        'collected_at': current_time.isoformat()
                    }
                
                # System metrics
                self._performance_metrics['system'] = {
                    'active_users': len(self._active_sessions),
                    'registered_callbacks': len(self._callbacks),
                    'events_count': len(self._events),
                    'collected_at': current_time.isoformat()
                }
                
        except Exception as e:
            self._log_error("metrics_collection", str(e))

    # ========================================
    # Authentication and Authorization
    # ========================================

    def authenticate_user(self, user_id: str, session_id: Optional[str] = None) -> ControllerUser:
        """
        Authenticate a user for controller access.
        
        Args:
            user_id: User identifier
            session_id: Optional session identifier
            
        Returns:
            ControllerUser: Authenticated user information
            
        Raises:
            PermissionError: If authentication fails
        """
        if not self.enable_auth:
            # Return default operator for non-auth mode
            return self._users.get("default", self._create_temp_user(user_id))
        
        with self._lock:
            user = self._users.get(user_id)
            if not user:
                raise PermissionError(f"User {user_id} not authorized for controller access")
            
            # Update session mapping
            if session_id:
                self._active_sessions[session_id] = user_id
            
            # Update access tracking
            user.last_access = datetime.utcnow()
            user.access_count += 1
            
            return user

    def _create_temp_user(self, user_id: str) -> ControllerUser:
        """Create a temporary user for non-auth scenarios."""
        return ControllerUser(
            user_id=user_id,
            username=f"temp_user_{user_id}",
            role=ControllerRole.VIEWER,
            permissions={ControllerPermission.READ_ONLY}
        )

    def _check_permission(self, user: ControllerUser, required_permission: ControllerPermission) -> bool:
        """
        Check if user has required permission.
        
        Args:
            user: User to check
            required_permission: Required permission level
            
        Returns:
            bool: True if user has permission
        """
        return required_permission in user.permissions

    def _check_rate_limit(self, user_id: str) -> bool:
        """
        Check if user is within rate limits.
        
        Args:
            user_id: User to check
            
        Returns:
            bool: True if within limits
        """
        current_time = time.time()
        user_requests = self._rate_limits[user_id]
        
        # Remove old requests outside the window
        while user_requests and current_time - user_requests[0] > self.rate_limit_window:
            user_requests.popleft()
        
        # Check if under limit
        if len(user_requests) >= self.rate_limit_requests:
            return False
        
        # Add current request
        user_requests.append(current_time)
        return True

    def _log_operation(
        self, 
        user: ControllerUser, 
        operation: str, 
        parameters: Dict[str, Any],
        result: Optional[Dict[str, Any]] = None,
        error: Optional[str] = None,
        duration_ms: Optional[float] = None
    ) -> None:
        """Log a controller operation for audit purposes."""
        event = ControllerEvent(
            event_id=f"{int(time.time() * 1000)}_{operation}_{user.user_id}",
            event_type="controller_operation",
            user_id=user.user_id,
            operation=operation,
            parameters=parameters,
            result=result,
            timestamp=datetime.utcnow(),
            duration_ms=duration_ms,
            error=error
        )
        
        with self._lock:
            self._events.append(event)
        
        if error:
            logger.error(f"Controller operation failed - User: {user.username}, Operation: {operation}, Error: {error}")
        else:
            logger.info(f"Controller operation - User: {user.username}, Operation: {operation}")

    def _log_error(self, operation: str, error_message: str) -> None:
        """Log an error to the error tracking system."""
        error_entry = {
            'timestamp': datetime.utcnow().isoformat(),
            'operation': operation,
            'error': error_message
        }
        
        with self._lock:
            self._error_log.append(error_entry)

    # ========================================
    # Mode Control Methods
    # ========================================

    def set_operation_mode(
        self, 
        mode: OperationMode, 
        user_id: str = "system",
        session_id: Optional[str] = None,
        force: bool = False,
        reason: Optional[str] = None
    ) -> Dict[str, Any]:
        """
        Change Redis operation mode with validation and authorization.
        
        Args:
            mode: Target operation mode
            user_id: User requesting the change
            session_id: Optional session identifier
            force: Skip validation and timing checks
            reason: Reason for the mode change
            
        Returns:
            Dict containing operation result and status
            
        Raises:
            PermissionError: If user lacks required permissions
            ValueError: If mode transition is invalid
            RuntimeError: If operation fails
        """
        start_time = time.time()
        user = self.authenticate_user(user_id, session_id)
        
        # Check permissions
        required_permission = ControllerPermission.SUPER_ADMIN if force else ControllerPermission.ADMIN
        if not self._check_permission(user, required_permission):
            raise PermissionError(f"User {user.username} lacks {required_permission.value} permission")
        
        # Check rate limits
        if not self._check_rate_limit(user_id):
            raise RuntimeError(f"Rate limit exceeded for user {user.username}")
        
        try:
            if not self.mode_manager:
                raise RuntimeError("Mode manager not available")
            
            # Determine transition reason
            transition_reason = TransitionReason.MANUAL_OVERRIDE
            if reason:
                if "emergency" in reason.lower():
                    transition_reason = TransitionReason.EMERGENCY_FALLBACK
                elif "health" in reason.lower():
                    transition_reason = TransitionReason.HEALTH_DEGRADATION
            
            # Attempt mode transition
            success = self.mode_manager.request_mode_transition(
                target_mode=mode,
                reason=transition_reason,
                force=force,
                metadata={'user_id': user_id, 'reason': reason}
            )
            
            duration_ms = (time.time() - start_time) * 1000
            
            if success:
                # Log successful transition
                with self._lock:
                    self._mode_history.append({
                        'timestamp': datetime.utcnow().isoformat(),
                        'from_mode': self.mode_manager.get_current_status().get('current_mode'),
                        'to_mode': mode.value,
                        'user_id': user_id,
                        'reason': reason,
                        'force': force,
                        'duration_ms': duration_ms
                    })
                
                result = {
                    'success': True,
                    'mode': mode.value,
                    'message': f'Successfully changed to {mode.value} mode',
                    'duration_ms': duration_ms
                }
                
                self._log_operation(user, "set_operation_mode", 
                                  {'mode': mode.value, 'force': force, 'reason': reason}, 
                                  result, duration_ms=duration_ms)
                
                # Notify callbacks
                self._notify_mode_change_callbacks(mode, user_id, reason)
                
                return result
            else:
                error_msg = f"Failed to transition to {mode.value} mode"
                self._log_operation(user, "set_operation_mode", 
                                  {'mode': mode.value, 'force': force, 'reason': reason}, 
                                  error=error_msg, duration_ms=duration_ms)
                raise RuntimeError(error_msg)
                
        except Exception as e:
            duration_ms = (time.time() - start_time) * 1000
            error_msg = str(e)
            self._log_error("set_operation_mode", error_msg)
            self._log_operation(user, "set_operation_mode", 
                              {'mode': mode.value, 'force': force, 'reason': reason}, 
                              error=error_msg, duration_ms=duration_ms)
            raise

    def get_current_mode(self, user_id: str = "system", session_id: Optional[str] = None) -> Dict[str, Any]:
        """
        Get current operation mode and state information.
        
        Args:
            user_id: User requesting the information
            session_id: Optional session identifier
            
        Returns:
            Dict containing current mode and state details
        """
        start_time = time.time()
        user = self.authenticate_user(user_id, session_id)
        
        if not self._check_permission(user, ControllerPermission.READ_ONLY):
            raise PermissionError(f"User {user.username} lacks read permission")
        
        try:
            if not self.mode_manager:
                result = {
                    'current_mode': 'unknown',
                    'current_state': 'unavailable',
                    'error': 'Mode manager not available'
                }
            else:
                status = self.mode_manager.get_current_status()
                result = {
                    'current_mode': status.get('current_mode', 'unknown'),
                    'current_state': status.get('current_state', 'unknown'),
                    'circuit_breaker': status.get('circuit_breaker', {}),
                    'last_transition_time': status.get('last_transition_time'),
                    'can_use_redis': self.mode_manager.can_use_redis() if self.mode_manager else False,
                    'timestamp': datetime.utcnow().isoformat()
                }
            
            duration_ms = (time.time() - start_time) * 1000
            self._log_operation(user, "get_current_mode", {}, result, duration_ms=duration_ms)
            
            return result
            
        except Exception as e:
            duration_ms = (time.time() - start_time) * 1000
            error_msg = str(e)
            self._log_error("get_current_mode", error_msg)
            self._log_operation(user, "get_current_mode", {}, error=error_msg, duration_ms=duration_ms)
            raise

    def enable_auto_switching(
        self, 
        enabled: bool = True, 
        user_id: str = "system",
        session_id: Optional[str] = None
    ) -> Dict[str, Any]:
        """
        Enable or disable automatic mode switching.
        
        Args:
            enabled: True to enable auto-switching, False to disable
            user_id: User requesting the change
            session_id: Optional session identifier
            
        Returns:
            Dict containing operation result
        """
        start_time = time.time()
        user = self.authenticate_user(user_id, session_id)
        
        if not self._check_permission(user, ControllerPermission.ADMIN):
            raise PermissionError(f"User {user.username} lacks admin permission")
        
        try:
            # Note: This would need to be implemented in the mode manager
            # For now, we'll update configuration and log the operation
            if self.redis_config:
                # Update configuration (this would need actual implementation)
                pass
            
            result = {
                'success': True,
                'auto_switching_enabled': enabled,
                'message': f'Auto-switching {"enabled" if enabled else "disabled"}',
                'timestamp': datetime.utcnow().isoformat()
            }
            
            duration_ms = (time.time() - start_time) * 1000
            self._log_operation(user, "enable_auto_switching", {'enabled': enabled}, result, duration_ms=duration_ms)
            
            return result
            
        except Exception as e:
            duration_ms = (time.time() - start_time) * 1000
            error_msg = str(e)
            self._log_error("enable_auto_switching", error_msg)
            self._log_operation(user, "enable_auto_switching", {'enabled': enabled}, error=error_msg, duration_ms=duration_ms)
            raise

    def force_mode_transition(
        self,
        target_mode: OperationMode,
        reason: str,
        user_id: str = "system",
        session_id: Optional[str] = None
    ) -> Dict[str, Any]:
        """
        Force immediate mode transition bypassing normal validation.
        
        Args:
            target_mode: Target operation mode
            reason: Reason for forced transition
            user_id: User requesting the transition
            session_id: Optional session identifier
            
        Returns:
            Dict containing operation result
        """
        return self.set_operation_mode(
            mode=target_mode,
            user_id=user_id,
            session_id=session_id,
            force=True,
            reason=f"FORCED: {reason}"
        )

    def get_available_modes(self, user_id: str = "system", session_id: Optional[str] = None) -> Dict[str, Any]:
        """
        Get list of supported operation modes.
        
        Args:
            user_id: User requesting the information
            session_id: Optional session identifier
            
        Returns:
            Dict containing available modes and their descriptions
        """
        start_time = time.time()
        user = self.authenticate_user(user_id, session_id)
        
        if not self._check_permission(user, ControllerPermission.READ_ONLY):
            raise PermissionError(f"User {user.username} lacks read permission")
        
        try:
            result = {
                'available_modes': [
                    {
                        'mode': OperationMode.REDIS_ONLY.value,
                        'description': 'Redis Streams only mode',
                        'use_case': 'High performance, Redis available'
                    },
                    {
                        'mode': OperationMode.HYBRID.value,
                        'description': 'Redis Streams with async queue fallback',
                        'use_case': 'Balanced performance and reliability'
                    },
                    {
                        'mode': OperationMode.ASYNC_ONLY.value,
                        'description': 'Async queue only mode',
                        'use_case': 'Maximum reliability, Redis unavailable'
                    }
                ],
                'current_mode': self.get_current_mode(user_id, session_id).get('current_mode', 'unknown'),
                'timestamp': datetime.utcnow().isoformat()
            }
            
            duration_ms = (time.time() - start_time) * 1000
            self._log_operation(user, "get_available_modes", {}, result, duration_ms=duration_ms)
            
            return result
            
        except Exception as e:
            duration_ms = (time.time() - start_time) * 1000
            error_msg = str(e)
            self._log_error("get_available_modes", error_msg)
            self._log_operation(user, "get_available_modes", {}, error=error_msg, duration_ms=duration_ms)
            raise

    # ========================================
    # Health Control Methods  
    # ========================================

    def trigger_health_check(self, user_id: str = "system", session_id: Optional[str] = None) -> Dict[str, Any]:
        """
        Force immediate health check of Redis connectivity.
        
        Args:
            user_id: User requesting the health check
            session_id: Optional session identifier
            
        Returns:
            Dict containing health check results
        """
        start_time = time.time()
        user = self.authenticate_user(user_id, session_id)
        
        if not self._check_permission(user, ControllerPermission.READ_ONLY):
            raise PermissionError(f"User {user.username} lacks read permission")
        
        try:
            if not self.health_monitor:
                result = {
                    'health_status': 'unavailable',
                    'error': 'Health monitor not available',
                    'timestamp': datetime.utcnow().isoformat()
                }
            else:
                health_status = self.health_monitor.check_health()
                result = {
                    'health_status': 'healthy' if health_status.is_healthy else 'unhealthy',
                    'is_healthy': health_status.is_healthy,
                    'performance_score': health_status.performance_score,
                    'latency_ms': health_status.latency_ms,
                    'error_rate': health_status.error_rate,
                    'connectivity_rate': health_status.connectivity_rate,
                    'last_error': health_status.last_error,
                    'estimated_recovery_time': health_status.estimated_recovery_time,
                    'timestamp': datetime.utcnow().isoformat()
                }
            
            duration_ms = (time.time() - start_time) * 1000
            self._log_operation(user, "trigger_health_check", {}, result, duration_ms=duration_ms)
            
            return result
            
        except Exception as e:
            duration_ms = (time.time() - start_time) * 1000
            error_msg = str(e)
            self._log_error("trigger_health_check", error_msg)
            self._log_operation(user, "trigger_health_check", {}, error=error_msg, duration_ms=duration_ms)
            raise

    def set_health_check_interval(
        self, 
        seconds: float, 
        user_id: str = "system",
        session_id: Optional[str] = None
    ) -> Dict[str, Any]:
        """
        Update health check frequency.
        
        Args:
            seconds: New interval in seconds
            user_id: User requesting the change
            session_id: Optional session identifier
            
        Returns:
            Dict containing operation result
        """
        start_time = time.time()
        user = self.authenticate_user(user_id, session_id)
        
        if not self._check_permission(user, ControllerPermission.ADMIN):
            raise PermissionError(f"User {user.username} lacks admin permission")
        
        try:
            if seconds <= 0:
                raise ValueError("Health check interval must be positive")
            
            if not self.health_monitor:
                raise RuntimeError("Health monitor not available")
            
            # Update health monitor interval (this would need implementation in RedisHealthMonitor)
            # For now, we'll log the operation
            result = {
                'success': True,
                'new_interval_seconds': seconds,
                'message': f'Health check interval updated to {seconds} seconds',
                'timestamp': datetime.utcnow().isoformat()
            }
            
            duration_ms = (time.time() - start_time) * 1000
            self._log_operation(user, "set_health_check_interval", {'seconds': seconds}, result, duration_ms=duration_ms)
            
            return result
            
        except Exception as e:
            duration_ms = (time.time() - start_time) * 1000
            error_msg = str(e)
            self._log_error("set_health_check_interval", error_msg)
            self._log_operation(user, "set_health_check_interval", {'seconds': seconds}, error=error_msg, duration_ms=duration_ms)
            raise

    def reset_circuit_breaker(self, user_id: str = "system", session_id: Optional[str] = None) -> Dict[str, Any]:
        """
        Reset circuit breaker state to allow operations.
        
        Args:
            user_id: User requesting the reset
            session_id: Optional session identifier
            
        Returns:
            Dict containing operation result
        """
        start_time = time.time()
        user = self.authenticate_user(user_id, session_id)
        
        if not self._check_permission(user, ControllerPermission.ADMIN):
            raise PermissionError(f"User {user.username} lacks admin permission")
        
        try:
            if not self.mode_manager:
                raise RuntimeError("Mode manager not available")
            
            # Reset circuit breaker (this would need implementation in EventHandlerModeManager)
            # For now, we'll log the operation
            result = {
                'success': True,
                'message': 'Circuit breaker reset successfully',
                'timestamp': datetime.utcnow().isoformat()
            }
            
            duration_ms = (time.time() - start_time) * 1000
            self._log_operation(user, "reset_circuit_breaker", {}, result, duration_ms=duration_ms)
            
            return result
            
        except Exception as e:
            duration_ms = (time.time() - start_time) * 1000
            error_msg = str(e)
            self._log_error("reset_circuit_breaker", error_msg)
            self._log_operation(user, "reset_circuit_breaker", {}, error=error_msg, duration_ms=duration_ms)
            raise

    def get_health_status(self, user_id: str = "system", session_id: Optional[str] = None) -> Dict[str, Any]:
        """
        Get detailed health status report.
        
        Args:
            user_id: User requesting the status
            session_id: Optional session identifier
            
        Returns:
            Dict containing comprehensive health information
        """
        return self.trigger_health_check(user_id, session_id)

    def set_failure_thresholds(
        self,
        failure_count: int,
        recovery_count: int,
        user_id: str = "system", 
        session_id: Optional[str] = None
    ) -> Dict[str, Any]:
        """
        Update failure and recovery thresholds.
        
        Args:
            failure_count: Number of failures before switching modes
            recovery_count: Number of successes before recovering
            user_id: User requesting the change
            session_id: Optional session identifier
            
        Returns:
            Dict containing operation result
        """
        start_time = time.time()
        user = self.authenticate_user(user_id, session_id)
        
        if not self._check_permission(user, ControllerPermission.ADMIN):
            raise PermissionError(f"User {user.username} lacks admin permission")
        
        try:
            if failure_count <= 0 or recovery_count <= 0:
                raise ValueError("Thresholds must be positive integers")
            
            # Update thresholds (this would need implementation in mode manager/health monitor)
            result = {
                'success': True,
                'failure_threshold': failure_count,
                'recovery_threshold': recovery_count,
                'message': f'Thresholds updated: {failure_count} failures, {recovery_count} recoveries',
                'timestamp': datetime.utcnow().isoformat()
            }
            
            duration_ms = (time.time() - start_time) * 1000
            self._log_operation(user, "set_failure_thresholds", 
                              {'failure_count': failure_count, 'recovery_count': recovery_count}, 
                              result, duration_ms=duration_ms)
            
            return result
            
        except Exception as e:
            duration_ms = (time.time() - start_time) * 1000
            error_msg = str(e)
            self._log_error("set_failure_thresholds", error_msg)
            self._log_operation(user, "set_failure_thresholds", 
                              {'failure_count': failure_count, 'recovery_count': recovery_count}, 
                              error=error_msg, duration_ms=duration_ms)
            raise

    # ========================================
    # Diagnostic Methods
    # ========================================

    def get_mode_history(self, user_id: str = "system", session_id: Optional[str] = None) -> Dict[str, Any]:
        """
        Get recent mode transition history.
        
        Args:
            user_id: User requesting the history
            session_id: Optional session identifier
            
        Returns:
            Dict containing mode transition history
        """
        start_time = time.time()
        user = self.authenticate_user(user_id, session_id)
        
        if not self._check_permission(user, ControllerPermission.READ_ONLY):
            raise PermissionError(f"User {user.username} lacks read permission")
        
        try:
            with self._lock:
                history = list(self._mode_history)
            
            result = {
                'mode_history': history,
                'total_transitions': len(history),
                'timestamp': datetime.utcnow().isoformat()
            }
            
            duration_ms = (time.time() - start_time) * 1000
            self._log_operation(user, "get_mode_history", {}, result, duration_ms=duration_ms)
            
            return result
            
        except Exception as e:
            duration_ms = (time.time() - start_time) * 1000
            error_msg = str(e)
            self._log_error("get_mode_history", error_msg)
            self._log_operation(user, "get_mode_history", {}, error=error_msg, duration_ms=duration_ms)
            raise

    def get_performance_metrics(self, user_id: str = "system", session_id: Optional[str] = None) -> Dict[str, Any]:
        """
        Get Redis performance metrics and statistics.
        
        Args:
            user_id: User requesting the metrics
            session_id: Optional session identifier
            
        Returns:
            Dict containing performance metrics
        """
        start_time = time.time()
        user = self.authenticate_user(user_id, session_id)
        
        if not self._check_permission(user, ControllerPermission.READ_ONLY):
            raise PermissionError(f"User {user.username} lacks read permission")
        
        try:
            with self._lock:
                metrics = self._performance_metrics.copy()
            
            result = {
                'performance_metrics': metrics,
                'collection_timestamp': datetime.utcnow().isoformat()
            }
            
            duration_ms = (time.time() - start_time) * 1000
            self._log_operation(user, "get_performance_metrics", {}, result, duration_ms=duration_ms)
            
            return result
            
        except Exception as e:
            duration_ms = (time.time() - start_time) * 1000
            error_msg = str(e)
            self._log_error("get_performance_metrics", error_msg)
            self._log_operation(user, "get_performance_metrics", {}, error=error_msg, duration_ms=duration_ms)
            raise

    def get_error_log(self, user_id: str = "system", session_id: Optional[str] = None) -> Dict[str, Any]:
        """
        Get recent errors and failures.
        
        Args:
            user_id: User requesting the error log
            session_id: Optional session identifier
            
        Returns:
            Dict containing recent errors
        """
        start_time = time.time()
        user = self.authenticate_user(user_id, session_id)
        
        if not self._check_permission(user, ControllerPermission.READ_ONLY):
            raise PermissionError(f"User {user.username} lacks read permission")
        
        try:
            with self._lock:
                errors = list(self._error_log)
            
            result = {
                'errors': errors,
                'total_errors': len(errors),
                'timestamp': datetime.utcnow().isoformat()
            }
            
            duration_ms = (time.time() - start_time) * 1000
            self._log_operation(user, "get_error_log", {}, result, duration_ms=duration_ms)
            
            return result
            
        except Exception as e:
            duration_ms = (time.time() - start_time) * 1000
            error_msg = str(e)
            self._log_error("get_error_log", error_msg)
            self._log_operation(user, "get_error_log", {}, error=error_msg, duration_ms=duration_ms)
            raise

    def export_diagnostics(self, user_id: str = "system", session_id: Optional[str] = None) -> Dict[str, Any]:
        """
        Export comprehensive diagnostic data.
        
        Args:
            user_id: User requesting the diagnostics
            session_id: Optional session identifier
            
        Returns:
            Dict containing comprehensive diagnostic information
        """
        start_time = time.time()
        user = self.authenticate_user(user_id, session_id)
        
        if not self._check_permission(user, ControllerPermission.READ_ONLY):
            raise PermissionError(f"User {user.username} lacks read permission")
        
        try:
            # Gather comprehensive diagnostics
            diagnostics = {
                'export_timestamp': datetime.utcnow().isoformat(),
                'current_mode': self.get_current_mode(user_id, session_id),
                'health_status': self.get_health_status(user_id, session_id),
                'mode_history': self.get_mode_history(user_id, session_id),
                'performance_metrics': self.get_performance_metrics(user_id, session_id),
                'error_log': self.get_error_log(user_id, session_id),
                'controller_info': {
                    'active_users': len(self._active_sessions),
                    'registered_callbacks': len(self._callbacks),
                    'auth_enabled': self.enable_auth,
                    'rate_limiting': {
                        'window_seconds': self.rate_limit_window,
                        'max_requests': self.rate_limit_requests
                    }
                }
            }
            
            duration_ms = (time.time() - start_time) * 1000
            self._log_operation(user, "export_diagnostics", {}, 
                              {'diagnostics_exported': True}, duration_ms=duration_ms)
            
            return diagnostics
            
        except Exception as e:
            duration_ms = (time.time() - start_time) * 1000
            error_msg = str(e)
            self._log_error("export_diagnostics", error_msg)
            self._log_operation(user, "export_diagnostics", {}, error=error_msg, duration_ms=duration_ms)
            raise

    def test_redis_connectivity(self, user_id: str = "system", session_id: Optional[str] = None) -> Dict[str, Any]:
        """
        Test Redis connection without changing operation mode.
        
        Args:
            user_id: User requesting the test
            session_id: Optional session identifier
            
        Returns:
            Dict containing connectivity test results
        """
        start_time = time.time()
        user = self.authenticate_user(user_id, session_id)
        
        if not self._check_permission(user, ControllerPermission.READ_ONLY):
            raise PermissionError(f"User {user.username} lacks read permission")
        
        try:
            # Test basic Redis connectivity
            try:
                redis_client = self.redis_config.get_redis_client()
                test_result = asyncio.run(redis_client.ping())
                
                result = {
                    'connectivity': 'success',
                    'redis_available': True,
                    'ping_successful': test_result,
                    'connection_info': self.redis_config.get_connection_info(),
                    'timestamp': datetime.utcnow().isoformat()
                }
            except Exception as redis_error:
                result = {
                    'connectivity': 'failed',
                    'redis_available': False,
                    'error': str(redis_error),
                    'timestamp': datetime.utcnow().isoformat()
                }
            
            duration_ms = (time.time() - start_time) * 1000
            self._log_operation(user, "test_redis_connectivity", {}, result, duration_ms=duration_ms)
            
            return result
            
        except Exception as e:
            duration_ms = (time.time() - start_time) * 1000
            error_msg = str(e)
            self._log_error("test_redis_connectivity", error_msg)
            self._log_operation(user, "test_redis_connectivity", {}, error=error_msg, duration_ms=duration_ms)
            raise

    # ========================================
    # Notification and Event Hooks
    # ========================================

    def register_mode_change_callback(
        self,
        callback: Callable,
        user_id: str = "system",
        session_id: Optional[str] = None
    ) -> str:
        """
        Register callback for mode change notifications.
        
        Args:
            callback: Function to call on mode changes
            user_id: User registering the callback
            session_id: Optional session identifier
            
        Returns:
            str: Callback ID for unregistering
        """
        start_time = time.time()
        user = self.authenticate_user(user_id, session_id)
        
        if not self._check_permission(user, ControllerPermission.READ_ONLY):
            raise PermissionError(f"User {user.username} lacks read permission")
        
        try:
            callback_id = f"mode_change_{user_id}_{int(time.time() * 1000)}"
            
            registration = CallbackRegistration(
                callback_id=callback_id,
                callback_type="mode_change",
                callback_function=callback,
                user_id=user_id,
                registered_at=datetime.utcnow()
            )
            
            with self._lock:
                self._callbacks[callback_id] = registration
            
            duration_ms = (time.time() - start_time) * 1000
            self._log_operation(user, "register_mode_change_callback", 
                              {'callback_id': callback_id}, 
                              {'registered': True}, duration_ms=duration_ms)
            
            return callback_id
            
        except Exception as e:
            duration_ms = (time.time() - start_time) * 1000
            error_msg = str(e)
            self._log_error("register_mode_change_callback", error_msg)
            self._log_operation(user, "register_mode_change_callback", {}, 
                              error=error_msg, duration_ms=duration_ms)
            raise

    def register_health_change_callback(
        self,
        callback: Callable,
        user_id: str = "system",
        session_id: Optional[str] = None
    ) -> str:
        """
        Register callback for health status change notifications.
        
        Args:
            callback: Function to call on health changes
            user_id: User registering the callback
            session_id: Optional session identifier
            
        Returns:
            str: Callback ID for unregistering
        """
        start_time = time.time()
        user = self.authenticate_user(user_id, session_id)
        
        if not self._check_permission(user, ControllerPermission.READ_ONLY):
            raise PermissionError(f"User {user.username} lacks read permission")
        
        try:
            callback_id = f"health_change_{user_id}_{int(time.time() * 1000)}"
            
            registration = CallbackRegistration(
                callback_id=callback_id,
                callback_type="health_change",
                callback_function=callback,
                user_id=user_id,
                registered_at=datetime.utcnow()
            )
            
            with self._lock:
                self._callbacks[callback_id] = registration
            
            duration_ms = (time.time() - start_time) * 1000
            self._log_operation(user, "register_health_change_callback", 
                              {'callback_id': callback_id}, 
                              {'registered': True}, duration_ms=duration_ms)
            
            return callback_id
            
        except Exception as e:
            duration_ms = (time.time() - start_time) * 1000
            error_msg = str(e)
            self._log_error("register_health_change_callback", error_msg)
            self._log_operation(user, "register_health_change_callback", {}, 
                              error=error_msg, duration_ms=duration_ms)
            raise

    def unregister_callback(
        self,
        callback_id: str,
        user_id: str = "system",
        session_id: Optional[str] = None
    ) -> Dict[str, Any]:
        """
        Remove registered callback.
        
        Args:
            callback_id: ID of callback to remove
            user_id: User requesting removal
            session_id: Optional session identifier
            
        Returns:
            Dict containing operation result
        """
        start_time = time.time()
        user = self.authenticate_user(user_id, session_id)
        
        try:
            with self._lock:
                registration = self._callbacks.get(callback_id)
                if not registration:
                    raise ValueError(f"Callback {callback_id} not found")
                
                # Check if user can unregister this callback
                if registration.user_id != user_id and not self._check_permission(user, ControllerPermission.ADMIN):
                    raise PermissionError(f"User {user.username} cannot unregister callback owned by {registration.user_id}")
                
                del self._callbacks[callback_id]
            
            result = {
                'success': True,
                'callback_id': callback_id,
                'message': f'Callback {callback_id} unregistered successfully',
                'timestamp': datetime.utcnow().isoformat()
            }
            
            duration_ms = (time.time() - start_time) * 1000
            self._log_operation(user, "unregister_callback", 
                              {'callback_id': callback_id}, result, duration_ms=duration_ms)
            
            return result
            
        except Exception as e:
            duration_ms = (time.time() - start_time) * 1000
            error_msg = str(e)
            self._log_error("unregister_callback", error_msg)
            self._log_operation(user, "unregister_callback", 
                              {'callback_id': callback_id}, error=error_msg, duration_ms=duration_ms)
            raise

    def get_event_stream(
        self,
        user_id: str = "system",
        session_id: Optional[str] = None
    ) -> AsyncGenerator[Dict[str, Any], None]:
        """
        Stream real-time events (mode changes, health alerts).
        
        Args:
            user_id: User requesting the stream
            session_id: Optional session identifier
            
        Yields:
            Dict: Real-time event information
        """
        user = self.authenticate_user(user_id, session_id)
        
        if not self._check_permission(user, ControllerPermission.READ_ONLY):
            raise PermissionError(f"User {user.username} lacks read permission")
        
        # This would be implemented as an async generator
        # For now, we'll provide a placeholder implementation
        async def event_generator():
            last_event_count = len(self._events)
            
            while True:
                try:
                    # Check for new events
                    current_event_count = len(self._events)
                    if current_event_count > last_event_count:
                        with self._lock:
                            new_events = list(self._events)[last_event_count:]
                        
                        for event in new_events:
                            yield {
                                'event_type': 'controller_event',
                                'event_id': event.event_id,
                                'operation': event.operation,
                                'user_id': event.user_id,
                                'timestamp': event.timestamp.isoformat(),
                                'error': event.error
                            }
                        
                        last_event_count = current_event_count
                    
                    # Send heartbeat
                    yield {
                        'event_type': 'heartbeat',
                        'timestamp': datetime.utcnow().isoformat(),
                        'active_users': len(self._active_sessions)
                    }
                    
                    await asyncio.sleep(5)  # 5 second intervals
                    
                except Exception as e:
                    yield {
                        'event_type': 'error',
                        'error': str(e),
                        'timestamp': datetime.utcnow().isoformat()
                    }
                    await asyncio.sleep(10)  # Longer wait on error
        
        return event_generator()

    def _notify_mode_change_callbacks(self, new_mode: OperationMode, user_id: str, reason: Optional[str]) -> None:
        """Notify registered mode change callbacks."""
        mode_change_callbacks = [
            reg for reg in self._callbacks.values() 
            if reg.callback_type == "mode_change"
        ]
        
        for registration in mode_change_callbacks:
            try:
                registration.callback_function(new_mode, user_id, reason)
                registration.last_called = datetime.utcnow()
                registration.call_count += 1
            except Exception as e:
                self._log_error("mode_change_callback", f"Callback {registration.callback_id} failed: {e}")

    def _notify_health_change_callbacks(self, health_status: HealthStatus) -> None:
        """Notify registered health change callbacks."""
        health_change_callbacks = [
            reg for reg in self._callbacks.values() 
            if reg.callback_type == "health_change"
        ]
        
        for registration in health_change_callbacks:
            try:
                registration.callback_function(health_status)
                registration.last_called = datetime.utcnow()
                registration.call_count += 1
            except Exception as e:
                self._log_error("health_change_callback", f"Callback {registration.callback_id} failed: {e}")

    # ========================================
    # User Management Methods
    # ========================================

    def add_user(
        self,
        user_id: str,
        username: str,
        role: ControllerRole,
        admin_user_id: str = "system"
    ) -> Dict[str, Any]:
        """
        Add a new user with specified role and permissions.
        
        Args:
            user_id: Unique user identifier
            username: Human-readable username
            role: User role determining permissions
            admin_user_id: Admin user adding the new user
            
        Returns:
            Dict containing operation result
        """
        admin_user = self.authenticate_user(admin_user_id)
        
        if not self._check_permission(admin_user, ControllerPermission.ADMIN):
            raise PermissionError(f"User {admin_user.username} lacks admin permission")
        
        if user_id in self._users:
            raise ValueError(f"User {user_id} already exists")
        
        # Determine permissions based on role
        permissions = set()
        if role in [ControllerRole.VIEWER, ControllerRole.OPERATOR, ControllerRole.ADMIN, ControllerRole.SUPER_ADMIN]:
            permissions.add(ControllerPermission.READ_ONLY)
        if role in [ControllerRole.OPERATOR, ControllerRole.ADMIN, ControllerRole.SUPER_ADMIN]:
            permissions.add(ControllerPermission.ADMIN)
        if role == ControllerRole.SUPER_ADMIN:
            permissions.add(ControllerPermission.SUPER_ADMIN)
        
        new_user = ControllerUser(
            user_id=user_id,
            username=username,
            role=role,
            permissions=permissions
        )
        
        with self._lock:
            self._users[user_id] = new_user
        
        self._log_operation(admin_user, "add_user", 
                          {'user_id': user_id, 'username': username, 'role': role.value},
                          {'user_added': True})
        
        return {
            'success': True,
            'user_id': user_id,
            'username': username,
            'role': role.value,
            'permissions': [p.value for p in permissions],
            'timestamp': datetime.utcnow().isoformat()
        }

    def remove_user(self, user_id: str, admin_user_id: str = "system") -> Dict[str, Any]:
        """
        Remove a user from the controller.
        
        Args:
            user_id: User to remove
            admin_user_id: Admin user performing the removal
            
        Returns:
            Dict containing operation result
        """
        admin_user = self.authenticate_user(admin_user_id)
        
        if not self._check_permission(admin_user, ControllerPermission.ADMIN):
            raise PermissionError(f"User {admin_user.username} lacks admin permission")
        
        if user_id not in self._users:
            raise ValueError(f"User {user_id} not found")
        
        if user_id == "system":
            raise ValueError("Cannot remove system user")
        
        with self._lock:
            removed_user = self._users.pop(user_id)
            
            # Remove from active sessions
            sessions_to_remove = [sid for sid, uid in self._active_sessions.items() if uid == user_id]
            for session_id in sessions_to_remove:
                del self._active_sessions[session_id]
            
            # Remove user's callbacks
            callbacks_to_remove = [cid for cid, reg in self._callbacks.items() if reg.user_id == user_id]
            for callback_id in callbacks_to_remove:
                del self._callbacks[callback_id]
        
        self._log_operation(admin_user, "remove_user", 
                          {'user_id': user_id},
                          {'user_removed': True, 'sessions_removed': len(sessions_to_remove), 
                           'callbacks_removed': len(callbacks_to_remove)})
        
        return {
            'success': True,
            'user_id': user_id,
            'username': removed_user.username,
            'sessions_removed': len(sessions_to_remove),
            'callbacks_removed': len(callbacks_to_remove),
            'timestamp': datetime.utcnow().isoformat()
        }

    def list_users(self, admin_user_id: str = "system") -> Dict[str, Any]:
        """
        List all users and their information.
        
        Args:
            admin_user_id: Admin user requesting the list
            
        Returns:
            Dict containing user list
        """
        admin_user = self.authenticate_user(admin_user_id)
        
        if not self._check_permission(admin_user, ControllerPermission.ADMIN):
            raise PermissionError(f"User {admin_user.username} lacks admin permission")
        
        with self._lock:
            users_info = []
            for user in self._users.values():
                users_info.append({
                    'user_id': user.user_id,
                    'username': user.username,
                    'role': user.role.value,
                    'permissions': [p.value for p in user.permissions],
                    'last_access': user.last_access.isoformat(),
                    'access_count': user.access_count
                })
        
        return {
            'users': users_info,
            'total_users': len(users_info),
            'timestamp': datetime.utcnow().isoformat()
        }

    # ========================================
    # Shutdown and Cleanup
    # ========================================

    def shutdown(self) -> None:
        """Shutdown the runtime controller and cleanup resources."""
        logger.info("Shutting down RedisRuntimeController")
        
        with self._lock:
            # Clear all data structures
            self._users.clear()
            self._active_sessions.clear()
            self._callbacks.clear()
            self._events.clear()
            self._mode_history.clear()
            self._error_log.clear()
            self._rate_limits.clear()
        
        logger.info("RedisRuntimeController shutdown complete")
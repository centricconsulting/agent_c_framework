"""
Tests for Redis Runtime Controller

Comprehensive test suite for the RedisRuntimeController class and its
authentication/authorization features. These tests cover all major
functionality including mode control, health monitoring, diagnostics,
event notifications, and security features.
"""

import asyncio
import pytest
import threading
import time
from datetime import datetime, timedelta
from unittest.mock import Mock, MagicMock, patch

from agent_c_api.core.util.redis_runtime_controller import (
    RedisRuntimeController,
    ControllerPermission,
    ControllerRole,
    ControllerUser,
    ControllerEvent,
    CallbackRegistration
)
from agent_c_api.core.util.redis_control_auth import (
    RedisControlAuth,
    AuthenticationMethod,
    SecurityEvent
)
from agent_c_api.core.util.resilient_mode import (
    OperationMode,
    TransitionReason,
    ModeTransition,
    ResilientModeConfig,
    EventHandlerModeManager
)
from agent_c_api.core.util.redis_health_monitor import (
    RedisHealthMonitor,
    HealthStatus,
    HealthMetric
)
from agent_c_api.config.redis_config import RedisConfig


class TestRedisRuntimeController:
    """Test suite for RedisRuntimeController."""

    @pytest.fixture
    def mock_mode_manager(self):
        """Create a mock EventHandlerModeManager."""
        mock_manager = Mock(spec=EventHandlerModeManager)
        mock_manager.get_current_status.return_value = {
            'current_mode': 'REDIS_ONLY',
            'current_state': 'ACTIVE',
            'circuit_breaker': {'state': 'CLOSED', 'failure_count': 0},
            'last_transition_time': datetime.utcnow().isoformat()
        }
        mock_manager.request_mode_transition.return_value = True
        mock_manager.can_use_redis.return_value = True
        return mock_manager

    @pytest.fixture
    def mock_health_monitor(self):
        """Create a mock RedisHealthMonitor."""
        mock_monitor = Mock(spec=RedisHealthMonitor)
        mock_status = HealthStatus(
            is_healthy=True,
            performance_score=0.95,
            latency_ms=10.5,
            error_rate=0.01,
            connectivity_rate=0.99,
            last_error=None,
            estimated_recovery_time=0.0
        )
        mock_monitor.check_health.return_value = mock_status
        return mock_monitor

    @pytest.fixture
    def mock_redis_config(self):
        """Create a mock RedisConfig."""
        mock_config = Mock(spec=RedisConfig)
        mock_config.get_connection_info.return_value = {
            'host': 'localhost',
            'port': 6379,
            'db': 0
        }
        
        # Mock Redis client
        mock_client = Mock()
        mock_client.ping = Mock(return_value=True)
        mock_config.get_redis_client.return_value = mock_client
        
        return mock_config

    @pytest.fixture
    def controller(self, mock_mode_manager, mock_health_monitor, mock_redis_config):
        """Create a RedisRuntimeController instance for testing."""
        controller = RedisRuntimeController(
            mode_manager=mock_mode_manager,
            health_monitor=mock_health_monitor,
            redis_config=mock_redis_config,
            enable_auth=True,
            rate_limit_window=60,
            rate_limit_requests=100
        )
        return controller

    @pytest.fixture
    def auth_disabled_controller(self, mock_mode_manager, mock_health_monitor, mock_redis_config):
        """Create a controller with authentication disabled."""
        controller = RedisRuntimeController(
            mode_manager=mock_mode_manager,
            health_monitor=mock_health_monitor,
            redis_config=mock_redis_config,
            enable_auth=False
        )
        return controller

    # ========================================
    # Initialization Tests
    # ========================================

    def test_controller_initialization(self, controller):
        """Test that controller initializes correctly."""
        assert controller.mode_manager is not None
        assert controller.health_monitor is not None
        assert controller.redis_config is not None
        assert controller.enable_auth is True
        assert len(controller._users) >= 2  # system and default users

    def test_default_users_created(self, controller):
        """Test that default users are created during initialization."""
        assert "system" in controller._users
        assert "default" in controller._users
        
        system_user = controller._users["system"]
        assert system_user.role == ControllerRole.SUPER_ADMIN
        assert ControllerPermission.SUPER_ADMIN in system_user.permissions
        
        default_user = controller._users["default"]
        assert default_user.role == ControllerRole.OPERATOR
        assert ControllerPermission.ADMIN in default_user.permissions

    # ========================================
    # Authentication Tests
    # ========================================

    def test_authenticate_user_success(self, controller):
        """Test successful user authentication."""
        user = controller.authenticate_user("system")
        assert user.user_id == "system"
        assert user.role == ControllerRole.SUPER_ADMIN

    def test_authenticate_user_failure(self, controller):
        """Test failed user authentication."""
        with pytest.raises(PermissionError):
            controller.authenticate_user("nonexistent_user")

    def test_authenticate_user_auth_disabled(self, auth_disabled_controller):
        """Test authentication with auth disabled."""
        user = auth_disabled_controller.authenticate_user("any_user")
        assert user.user_id == "default"

    def test_permission_checking(self, controller):
        """Test permission checking functionality."""
        user = controller._users["system"]
        assert controller._check_permission(user, ControllerPermission.READ_ONLY)
        assert controller._check_permission(user, ControllerPermission.ADMIN)
        assert controller._check_permission(user, ControllerPermission.SUPER_ADMIN)
        
        viewer_user = ControllerUser(
            user_id="viewer",
            username="test_viewer",
            role=ControllerRole.VIEWER,
            permissions={ControllerPermission.READ_ONLY}
        )
        assert controller._check_permission(viewer_user, ControllerPermission.READ_ONLY)
        assert not controller._check_permission(viewer_user, ControllerPermission.ADMIN)

    def test_rate_limiting(self, controller):
        """Test rate limiting functionality."""
        user_id = "test_user"
        
        # Should allow requests initially
        assert controller._check_rate_limit(user_id)
        
        # Simulate many requests
        for _ in range(100):
            controller._check_rate_limit(user_id)
        
        # Should be rate limited now
        assert not controller._check_rate_limit(user_id)

    # ========================================
    # Mode Control Tests
    # ========================================

    def test_set_operation_mode_success(self, controller, mock_mode_manager):
        """Test successful operation mode change."""
        mock_mode_manager.request_mode_transition.return_value = True
        
        result = controller.set_operation_mode(
            mode=OperationMode.HYBRID,
            user_id="system",
            reason="Test mode change"
        )
        
        assert result['success'] is True
        assert result['mode'] == OperationMode.HYBRID.value
        mock_mode_manager.request_mode_transition.assert_called_once()

    def test_set_operation_mode_failure(self, controller, mock_mode_manager):
        """Test failed operation mode change."""
        mock_mode_manager.request_mode_transition.return_value = False
        
        with pytest.raises(RuntimeError):
            controller.set_operation_mode(
                mode=OperationMode.ASYNC_ONLY,
                user_id="system"
            )

    def test_set_operation_mode_permission_denied(self, controller):
        """Test mode change with insufficient permissions."""
        controller.add_user("viewer", "test_viewer", ControllerRole.VIEWER)
        
        with pytest.raises(PermissionError):
            controller.set_operation_mode(
                mode=OperationMode.HYBRID,
                user_id="viewer"
            )

    def test_force_mode_transition(self, controller, mock_mode_manager):
        """Test forced mode transition."""
        mock_mode_manager.request_mode_transition.return_value = True
        
        result = controller.force_mode_transition(
            target_mode=OperationMode.ASYNC_ONLY,
            reason="Emergency fallback",
            user_id="system"
        )
        
        assert result['success'] is True
        # Verify force=True was passed
        call_args = mock_mode_manager.request_mode_transition.call_args
        assert call_args[1]['force'] is True

    def test_get_current_mode(self, controller):
        """Test getting current operation mode."""
        result = controller.get_current_mode("system")
        
        assert 'current_mode' in result
        assert 'current_state' in result
        assert 'circuit_breaker' in result
        assert 'timestamp' in result

    def test_get_available_modes(self, controller):
        """Test getting available operation modes."""
        result = controller.get_available_modes("system")
        
        assert 'available_modes' in result
        assert len(result['available_modes']) == 3
        assert any(mode['mode'] == 'REDIS_ONLY' for mode in result['available_modes'])
        assert any(mode['mode'] == 'HYBRID' for mode in result['available_modes'])
        assert any(mode['mode'] == 'ASYNC_ONLY' for mode in result['available_modes'])

    def test_enable_auto_switching(self, controller):
        """Test enabling/disabling auto-switching."""
        result = controller.enable_auto_switching(enabled=True, user_id="system")
        
        assert result['success'] is True
        assert result['auto_switching_enabled'] is True

    # ========================================
    # Health Control Tests
    # ========================================

    def test_trigger_health_check(self, controller, mock_health_monitor):
        """Test triggering health check."""
        result = controller.trigger_health_check("system")
        
        assert 'health_status' in result
        assert 'is_healthy' in result
        assert 'performance_score' in result
        assert 'latency_ms' in result
        mock_health_monitor.check_health.assert_called_once()

    def test_trigger_health_check_no_monitor(self, controller):
        """Test health check when monitor is unavailable."""
        controller.health_monitor = None
        
        result = controller.trigger_health_check("system")
        
        assert result['health_status'] == 'unavailable'
        assert 'error' in result

    def test_set_health_check_interval(self, controller):
        """Test setting health check interval."""
        result = controller.set_health_check_interval(30.0, "system")
        
        assert result['success'] is True
        assert result['new_interval_seconds'] == 30.0

    def test_set_health_check_interval_invalid(self, controller):
        """Test setting invalid health check interval."""
        with pytest.raises(ValueError):
            controller.set_health_check_interval(-1.0, "system")

    def test_reset_circuit_breaker(self, controller):
        """Test resetting circuit breaker."""
        result = controller.reset_circuit_breaker("system")
        
        assert result['success'] is True
        assert 'message' in result

    def test_set_failure_thresholds(self, controller):
        """Test setting failure thresholds."""
        result = controller.set_failure_thresholds(5, 3, "system")
        
        assert result['success'] is True
        assert result['failure_threshold'] == 5
        assert result['recovery_threshold'] == 3

    def test_set_failure_thresholds_invalid(self, controller):
        """Test setting invalid failure thresholds."""
        with pytest.raises(ValueError):
            controller.set_failure_thresholds(0, 3, "system")
        
        with pytest.raises(ValueError):
            controller.set_failure_thresholds(5, -1, "system")

    # ========================================
    # Diagnostic Tests
    # ========================================

    def test_get_mode_history(self, controller):
        """Test getting mode transition history."""
        # Add some history
        controller._mode_history.append({
            'timestamp': datetime.utcnow().isoformat(),
            'from_mode': 'REDIS_ONLY',
            'to_mode': 'HYBRID',
            'user_id': 'system',
            'reason': 'test'
        })
        
        result = controller.get_mode_history("system")
        
        assert 'mode_history' in result
        assert 'total_transitions' in result
        assert len(result['mode_history']) > 0

    def test_get_performance_metrics(self, controller):
        """Test getting performance metrics."""
        result = controller.get_performance_metrics("system")
        
        assert 'performance_metrics' in result
        assert 'collection_timestamp' in result

    def test_get_error_log(self, controller):
        """Test getting error log."""
        # Add some errors
        controller._log_error("test_operation", "Test error message")
        
        result = controller.get_error_log("system")
        
        assert 'errors' in result
        assert 'total_errors' in result
        assert len(result['errors']) > 0

    def test_export_diagnostics(self, controller):
        """Test exporting comprehensive diagnostics."""
        result = controller.export_diagnostics("system")
        
        assert 'export_timestamp' in result
        assert 'current_mode' in result
        assert 'health_status' in result
        assert 'mode_history' in result
        assert 'performance_metrics' in result
        assert 'error_log' in result
        assert 'controller_info' in result

    @patch('asyncio.run')
    def test_test_redis_connectivity_success(self, mock_asyncio_run, controller, mock_redis_config):
        """Test successful Redis connectivity test."""
        mock_asyncio_run.return_value = True
        
        result = controller.test_redis_connectivity("system")
        
        assert result['connectivity'] == 'success'
        assert result['redis_available'] is True
        assert result['ping_successful'] is True

    @patch('asyncio.run')
    def test_test_redis_connectivity_failure(self, mock_asyncio_run, controller, mock_redis_config):
        """Test failed Redis connectivity test."""
        mock_asyncio_run.side_effect = Exception("Connection failed")
        
        result = controller.test_redis_connectivity("system")
        
        assert result['connectivity'] == 'failed'
        assert result['redis_available'] is False
        assert 'error' in result

    # ========================================
    # Callback Tests
    # ========================================

    def test_register_mode_change_callback(self, controller):
        """Test registering mode change callback."""
        def test_callback(mode, user_id, reason):
            pass
        
        callback_id = controller.register_mode_change_callback(test_callback, "system")
        
        assert callback_id.startswith("mode_change_")
        assert callback_id in controller._callbacks
        assert controller._callbacks[callback_id].callback_type == "mode_change"

    def test_register_health_change_callback(self, controller):
        """Test registering health change callback."""
        def test_callback(health_status):
            pass
        
        callback_id = controller.register_health_change_callback(test_callback, "system")
        
        assert callback_id.startswith("health_change_")
        assert callback_id in controller._callbacks
        assert controller._callbacks[callback_id].callback_type == "health_change"

    def test_unregister_callback(self, controller):
        """Test unregistering callback."""
        def test_callback(mode, user_id, reason):
            pass
        
        callback_id = controller.register_mode_change_callback(test_callback, "system")
        result = controller.unregister_callback(callback_id, "system")
        
        assert result['success'] is True
        assert callback_id not in controller._callbacks

    def test_unregister_callback_not_found(self, controller):
        """Test unregistering non-existent callback."""
        with pytest.raises(ValueError):
            controller.unregister_callback("nonexistent_callback", "system")

    def test_unregister_callback_permission_denied(self, controller):
        """Test unregistering callback without permission."""
        def test_callback(mode, user_id, reason):
            pass
        
        # Add a user with limited permissions
        controller.add_user("limited", "limited_user", ControllerRole.VIEWER)
        
        # Register callback as system user
        callback_id = controller.register_mode_change_callback(test_callback, "system")
        
        # Try to unregister as limited user
        with pytest.raises(PermissionError):
            controller.unregister_callback(callback_id, "limited")

    def test_notify_mode_change_callbacks(self, controller):
        """Test notifying mode change callbacks."""
        callback_called = threading.Event()
        received_args = {}
        
        def test_callback(mode, user_id, reason):
            received_args['mode'] = mode
            received_args['user_id'] = user_id
            received_args['reason'] = reason
            callback_called.set()
        
        controller.register_mode_change_callback(test_callback, "system")
        controller._notify_mode_change_callbacks(OperationMode.HYBRID, "system", "test")
        
        # Wait for callback to be called
        assert callback_called.wait(timeout=1.0)
        assert received_args['mode'] == OperationMode.HYBRID
        assert received_args['user_id'] == "system"
        assert received_args['reason'] == "test"

    # ========================================
    # User Management Tests
    # ========================================

    def test_add_user(self, controller):
        """Test adding a new user."""
        result = controller.add_user("test_user", "Test User", ControllerRole.OPERATOR)
        
        assert result['success'] is True
        assert result['user_id'] == "test_user"
        assert result['role'] == ControllerRole.OPERATOR.value
        assert "test_user" in controller._users

    def test_add_user_already_exists(self, controller):
        """Test adding user that already exists."""
        controller.add_user("test_user", "Test User", ControllerRole.OPERATOR)
        
        with pytest.raises(ValueError):
            controller.add_user("test_user", "Another User", ControllerRole.ADMIN)

    def test_remove_user(self, controller):
        """Test removing a user."""
        controller.add_user("test_user", "Test User", ControllerRole.OPERATOR)
        result = controller.remove_user("test_user")
        
        assert result['success'] is True
        assert result['user_id'] == "test_user"
        assert "test_user" not in controller._users

    def test_remove_system_user(self, controller):
        """Test removing system user (should fail)."""
        with pytest.raises(ValueError):
            controller.remove_user("system")

    def test_list_users(self, controller):
        """Test listing all users."""
        controller.add_user("test_user", "Test User", ControllerRole.OPERATOR)
        
        result = controller.list_users()
        
        assert 'users' in result
        assert 'total_users' in result
        assert len(result['users']) >= 3  # system, default, test_user

    # ========================================
    # Event Stream Tests
    # ========================================

    @pytest.mark.asyncio
    async def test_get_event_stream(self, controller):
        """Test getting event stream."""
        event_stream = controller.get_event_stream("system")
        
        # Get first event (should be heartbeat)
        first_event = await event_stream.__anext__()
        
        assert 'event_type' in first_event
        assert 'timestamp' in first_event

    # ========================================
    # Metrics Collection Tests
    # ========================================

    def test_metrics_collection(self, controller):
        """Test metrics collection functionality."""
        # Force metrics collection
        controller._collect_performance_metrics()
        
        assert 'health' in controller._performance_metrics
        assert 'mode' in controller._performance_metrics
        assert 'system' in controller._performance_metrics

    # ========================================
    # Error Handling Tests
    # ========================================

    def test_operation_with_no_mode_manager(self, controller):
        """Test operations when mode manager is unavailable."""
        controller.mode_manager = None
        
        with pytest.raises(RuntimeError):
            controller.set_operation_mode(OperationMode.HYBRID, "system")

    def test_operation_with_no_health_monitor(self, controller):
        """Test operations when health monitor is unavailable."""
        controller.health_monitor = None
        
        result = controller.trigger_health_check("system")
        assert result['health_status'] == 'unavailable'

    def test_error_logging(self, controller):
        """Test error logging functionality."""
        initial_error_count = len(controller._error_log)
        
        controller._log_error("test_operation", "Test error message")
        
        assert len(controller._error_log) == initial_error_count + 1
        latest_error = controller._error_log[-1]
        assert latest_error['operation'] == "test_operation"
        assert latest_error['error'] == "Test error message"

    # ========================================
    # Shutdown Tests
    # ========================================

    def test_shutdown(self, controller):
        """Test controller shutdown."""
        # Add some data
        controller.add_user("test_user", "Test User", ControllerRole.OPERATOR)
        
        def test_callback(mode, user_id, reason):
            pass
        
        controller.register_mode_change_callback(test_callback, "system")
        
        # Shutdown
        controller.shutdown()
        
        # Verify cleanup
        assert len(controller._users) == 0
        assert len(controller._callbacks) == 0
        assert len(controller._events) == 0


class TestRedisControlAuth:
    """Test suite for RedisControlAuth."""

    @pytest.fixture
    def auth_service(self):
        """Create a RedisControlAuth instance for testing."""
        return RedisControlAuth(
            jwt_secret="test_secret_key_for_testing",
            token_expiry_hours=24,
            api_key_length=32,
            max_login_attempts=3,
            lockout_duration_minutes=15,
            enable_emergency_access=True
        )

    # ========================================
    # JWT Token Tests
    # ========================================

    def test_generate_jwt_token(self, auth_service):
        """Test JWT token generation."""
        token_string, token_obj = auth_service.generate_jwt_token(
            user_id="test_user",
            username="Test User",
            role="operator",
            permissions=["read_only", "admin"]
        )
        
        assert isinstance(token_string, str)
        assert token_obj.user_id == "test_user"
        assert token_obj.username == "Test User"
        assert token_obj.role == "operator"
        assert token_obj.permissions == ["read_only", "admin"]

    def test_validate_jwt_token_success(self, auth_service):
        """Test successful JWT token validation."""
        token_string, _ = auth_service.generate_jwt_token(
            user_id="test_user",
            username="Test User",
            role="operator",
            permissions=["read_only", "admin"]
        )
        
        validated_token = auth_service.validate_jwt_token(token_string)
        assert validated_token.user_id == "test_user"

    def test_validate_jwt_token_expired(self, auth_service):
        """Test JWT token validation with expired token."""
        # Create auth service with very short expiry
        short_auth = RedisControlAuth(
            jwt_secret="test_secret",
            token_expiry_hours=0.001  # Very short expiry
        )
        
        token_string, _ = short_auth.generate_jwt_token(
            user_id="test_user",
            username="Test User",
            role="operator",
            permissions=["read_only"]
        )
        
        # Wait for token to expire
        time.sleep(0.1)
        
        with pytest.raises(PermissionError):
            short_auth.validate_jwt_token(token_string)

    def test_validate_jwt_token_insufficient_permissions(self, auth_service):
        """Test JWT token validation with insufficient permissions."""
        token_string, _ = auth_service.generate_jwt_token(
            user_id="test_user",
            username="Test User",
            role="viewer",
            permissions=["read_only"]
        )
        
        with pytest.raises(PermissionError):
            auth_service.validate_jwt_token(token_string, required_permissions=["admin"])

    def test_revoke_jwt_token(self, auth_service):
        """Test JWT token revocation."""
        token_string, token_obj = auth_service.generate_jwt_token(
            user_id="test_user",
            username="Test User",
            role="operator",
            permissions=["read_only"]
        )
        
        assert auth_service.revoke_jwt_token(token_obj.token_id) is True
        
        with pytest.raises(PermissionError):
            auth_service.validate_jwt_token(token_string)

    # ========================================
    # API Key Tests
    # ========================================

    def test_generate_api_key(self, auth_service):
        """Test API key generation."""
        key_id, api_key_string = auth_service.generate_api_key(
            user_id="service_account",
            description="Test API key",
            permissions={"read_only", "admin"}
        )
        
        assert key_id.startswith("ak_")
        assert len(api_key_string) > 0
        assert key_id in auth_service._api_keys

    def test_validate_api_key_success(self, auth_service):
        """Test successful API key validation."""
        key_id, api_key_string = auth_service.generate_api_key(
            user_id="service_account",
            description="Test API key",
            permissions={"read_only", "admin"}
        )
        
        validated_key = auth_service.validate_api_key(api_key_string)
        assert validated_key.user_id == "service_account"
        assert validated_key.key_id == key_id

    def test_validate_api_key_invalid(self, auth_service):
        """Test API key validation with invalid key."""
        with pytest.raises(PermissionError):
            auth_service.validate_api_key("invalid_api_key")

    def test_validate_api_key_insufficient_permissions(self, auth_service):
        """Test API key validation with insufficient permissions."""
        _, api_key_string = auth_service.generate_api_key(
            user_id="service_account",
            description="Test API key",
            permissions={"read_only"}
        )
        
        with pytest.raises(PermissionError):
            auth_service.validate_api_key(api_key_string, required_permissions=["admin"])

    def test_revoke_api_key(self, auth_service):
        """Test API key revocation."""
        key_id, api_key_string = auth_service.generate_api_key(
            user_id="service_account",
            description="Test API key",
            permissions={"read_only"}
        )
        
        assert auth_service.revoke_api_key(key_id) is True
        
        with pytest.raises(PermissionError):
            auth_service.validate_api_key(api_key_string)

    # ========================================
    # Emergency Access Tests
    # ========================================

    def test_validate_emergency_code(self, auth_service):
        """Test emergency code validation."""
        # Get the generated emergency code
        emergency_code = list(auth_service._emergency_codes.keys())[0]
        
        assert auth_service.validate_emergency_code(emergency_code) is True

    def test_validate_emergency_code_invalid(self, auth_service):
        """Test emergency code validation with invalid code."""
        assert auth_service.validate_emergency_code("invalid_code") is False

    def test_generate_new_emergency_code(self, auth_service):
        """Test generating new emergency code."""
        old_codes = set(auth_service._emergency_codes.keys())
        
        new_code = auth_service.generate_new_emergency_code("admin_user")
        
        assert new_code not in old_codes
        assert new_code in auth_service._emergency_codes

    # ========================================
    # Authentication Request Tests
    # ========================================

    def test_authenticate_request_jwt(self, auth_service):
        """Test request authentication with JWT token."""
        token_string, _ = auth_service.generate_jwt_token(
            user_id="test_user",
            username="Test User",
            role="operator",
            permissions=["read_only", "admin"]
        )
        
        result = auth_service.authenticate_request(
            auth_header=f"Bearer {token_string}"
        )
        
        assert result['authenticated'] is True
        assert result['method'] == AuthenticationMethod.JWT_TOKEN.value
        assert result['user_id'] == "test_user"

    def test_authenticate_request_api_key(self, auth_service):
        """Test request authentication with API key."""
        _, api_key_string = auth_service.generate_api_key(
            user_id="service_account",
            description="Test API key",
            permissions={"read_only", "admin"}
        )
        
        result = auth_service.authenticate_request(api_key=api_key_string)
        
        assert result['authenticated'] is True
        assert result['method'] == AuthenticationMethod.API_KEY.value
        assert result['user_id'] == "service_account"

    def test_authenticate_request_emergency_code(self, auth_service):
        """Test request authentication with emergency code."""
        emergency_code = list(auth_service._emergency_codes.keys())[0]
        
        result = auth_service.authenticate_request(emergency_code=emergency_code)
        
        assert result['authenticated'] is True
        assert result['method'] == AuthenticationMethod.EMERGENCY_CODE.value
        assert result['user_id'] == "emergency"

    def test_authenticate_request_failure(self, auth_service):
        """Test request authentication failure."""
        with pytest.raises(PermissionError):
            auth_service.authenticate_request()

    # ========================================
    # Security Audit Tests
    # ========================================

    def test_security_audit_logging(self, auth_service):
        """Test security audit logging."""
        initial_log_count = len(auth_service._audit_log)
        
        auth_service._log_security_event(
            SecurityEvent.LOGIN_SUCCESS,
            "test_user",
            True,
            operation="test_operation"
        )
        
        assert len(auth_service._audit_log) == initial_log_count + 1
        latest_event = auth_service._audit_log[-1]
        assert latest_event.event_type == SecurityEvent.LOGIN_SUCCESS
        assert latest_event.user_id == "test_user"

    def test_get_security_audit_log(self, auth_service):
        """Test getting security audit log."""
        # Add some events
        auth_service._log_security_event(SecurityEvent.LOGIN_SUCCESS, "user1", True)
        auth_service._log_security_event(SecurityEvent.ACCESS_DENIED, "user2", False)
        
        # Get all events
        all_events = auth_service.get_security_audit_log()
        assert len(all_events) >= 2
        
        # Get filtered events
        user1_events = auth_service.get_security_audit_log(user_id="user1")
        assert all(event['user_id'] == "user1" for event in user1_events)
        
        # Get events by type
        login_events = auth_service.get_security_audit_log(event_type=SecurityEvent.LOGIN_SUCCESS)
        assert all(event['event_type'] == SecurityEvent.LOGIN_SUCCESS.value for event in login_events)

    def test_get_active_sessions(self, auth_service):
        """Test getting active session information."""
        # Generate some tokens and keys
        auth_service.generate_jwt_token("user1", "User 1", "operator", ["read_only"])
        auth_service.generate_api_key("service1", "Service 1", {"read_only"})
        
        sessions = auth_service.get_active_sessions()
        
        assert 'active_tokens' in sessions
        assert 'active_api_keys' in sessions
        assert sessions['active_tokens'] >= 1
        assert sessions['active_api_keys'] >= 1

    def test_cleanup_expired_sessions(self, auth_service):
        """Test cleanup of expired sessions."""
        # Create auth service with short expiry
        short_auth = RedisControlAuth(
            jwt_secret="test_secret",
            token_expiry_hours=0.001
        )
        
        # Generate token that will expire quickly
        short_auth.generate_jwt_token("user1", "User 1", "operator", ["read_only"])
        
        # Wait for expiration
        time.sleep(0.1)
        
        # Cleanup should remove expired token
        cleanup_count = short_auth.cleanup_expired_sessions()
        assert cleanup_count >= 1


if __name__ == "__main__":
    pytest.main([__file__])
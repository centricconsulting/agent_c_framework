"""
Unit tests for RedisHealthMonitor

Comprehensive test suite including:
- Health monitoring functionality
- Circuit breaker state transitions
- Auto-recovery mechanisms
- Event notification system
- Failure scenario simulations
- Thread safety verification
"""

import asyncio
import pytest
import time
import threading
from datetime import datetime, timedelta
from unittest.mock import Mock, AsyncMock, patch, MagicMock
from typing import List, Dict, Any

import redis
from redis.exceptions import ConnectionError, TimeoutError, RedisError

# Import the classes we're testing
from .redis_health_monitor import (
    RedisHealthMonitor,
    CircuitBreaker,
    CircuitBreakerState,
    HealthStatus,
    HealthMetrics,
    HealthEvent
)
from .redis_stream_manager import RedisConfig, OperationMode, FailoverStrategy


class TestHealthMetrics:
    """Test HealthMetrics functionality."""
    
    def test_initialization(self):
        """Test HealthMetrics initialization."""
        metrics = HealthMetrics()
        assert metrics.total_checks == 0
        assert metrics.successful_checks == 0
        assert metrics.failed_checks == 0
        assert metrics.failure_rate == 0.0
        assert metrics.average_latency_ms == 0.0
        assert len(metrics.latency_samples) == 0
    
    def test_add_latency_sample(self):
        """Test latency sample tracking."""
        metrics = HealthMetrics()
        
        # Add some samples
        metrics.add_latency_sample(10.0)
        metrics.add_latency_sample(20.0)
        metrics.add_latency_sample(30.0)
        
        assert len(metrics.latency_samples) == 3
        assert metrics.current_latency_ms == 30.0
        assert metrics.average_latency_ms == 20.0
    
    def test_latency_sample_window(self):
        """Test latency sample window management."""
        metrics = HealthMetrics()
        metrics.max_latency_samples = 3
        
        # Add more samples than the window size
        for i in range(5):
            metrics.add_latency_sample(float(i * 10))
        
        # Should only keep the last 3 samples
        assert len(metrics.latency_samples) == 3
        assert metrics.latency_samples == [20.0, 30.0, 40.0]
        assert metrics.average_latency_ms == 30.0
    
    def test_failure_rate_calculation(self):
        """Test failure rate calculation."""
        metrics = HealthMetrics()
        
        # No checks yet
        assert metrics.calculate_failure_rate() == 0.0
        
        # Simulate some failures
        metrics.total_checks = 10
        metrics.consecutive_failures = 3
        failure_rate = metrics.calculate_failure_rate()
        assert 0.0 <= failure_rate <= 1.0
        
        # Simulate successes
        metrics.consecutive_failures = 0
        metrics.consecutive_successes = 5
        failure_rate = metrics.calculate_failure_rate()
        assert failure_rate < 0.5


class TestCircuitBreaker:
    """Test CircuitBreaker functionality."""
    
    def test_initialization(self):
        """Test CircuitBreaker initialization."""
        cb = CircuitBreaker()
        assert cb.state == CircuitBreakerState.CLOSED
        assert cb.can_execute() == True
    
    def test_closed_state_operations(self):
        """Test operations in CLOSED state."""
        cb = CircuitBreaker(failure_threshold=0.5, min_calls_threshold=3)
        metrics = HealthMetrics()
        
        # Circuit should be closed initially
        assert cb.state == CircuitBreakerState.CLOSED
        assert cb.can_execute() == True
        
        # Record some successes
        for _ in range(5):
            metrics.total_checks += 1
            metrics.successful_checks += 1
            cb.record_success(metrics)
        
        assert cb.state == CircuitBreakerState.CLOSED
    
    def test_circuit_opening(self):
        """Test circuit opening due to failures."""
        cb = CircuitBreaker(failure_threshold=0.5, min_calls_threshold=3)
        metrics = HealthMetrics()
        
        # Simulate failures
        metrics.total_checks = 4
        metrics.failed_checks = 3
        metrics.failure_rate = 0.75  # Above threshold
        
        cb.record_failure(metrics)
        assert cb.state == CircuitBreakerState.OPEN
        assert cb.can_execute() == False
    
    def test_half_open_transition(self):
        """Test transition to HALF_OPEN state."""
        cb = CircuitBreaker(failure_threshold=0.5, recovery_timeout=1, min_calls_threshold=3)
        metrics = HealthMetrics()
        
        # Force circuit to OPEN
        metrics.total_checks = 4
        metrics.failed_checks = 3
        metrics.failure_rate = 0.75
        cb.record_failure(metrics)
        assert cb.state == CircuitBreakerState.OPEN
        
        # Wait for recovery timeout
        time.sleep(1.1)
        
        # Should allow execution and transition to HALF_OPEN
        assert cb.can_execute() == True
        assert cb.state == CircuitBreakerState.HALF_OPEN
    
    def test_recovery_from_half_open(self):
        """Test recovery from HALF_OPEN to CLOSED."""
        cb = CircuitBreaker(half_open_max_calls=2)
        cb._state = CircuitBreakerState.HALF_OPEN
        metrics = HealthMetrics()
        
        # Successful calls in half-open should eventually close circuit
        cb.record_success(metrics)
        assert cb.state == CircuitBreakerState.HALF_OPEN
        
        cb.record_success(metrics)
        assert cb.state == CircuitBreakerState.CLOSED
    
    def test_failure_in_half_open(self):
        """Test failure in HALF_OPEN state reopens circuit."""
        cb = CircuitBreaker()
        cb._state = CircuitBreakerState.HALF_OPEN
        metrics = HealthMetrics()
        
        # Failure in half-open should immediately open circuit
        cb.record_failure(metrics)
        assert cb.state == CircuitBreakerState.OPEN
    
    def test_manual_reset(self):
        """Test manual circuit breaker reset."""
        cb = CircuitBreaker()
        cb._state = CircuitBreakerState.OPEN
        
        cb.reset()
        assert cb.state == CircuitBreakerState.CLOSED
        assert cb.can_execute() == True


class TestRedisHealthMonitor:
    """Test RedisHealthMonitor functionality."""
    
    @pytest.fixture
    def redis_config(self):
        """Create a test Redis configuration."""
        return RedisConfig(
            redis_host='localhost',
            redis_port=6379,
            operation_mode=OperationMode.REDIS_ONLY,
            failover_strategy=FailoverStrategy.AUTO_FAILOVER,
            connection_timeout=5,
            recovery_interval=30
        )
    
    @pytest.fixture
    def health_monitor(self, redis_config):
        """Create a test health monitor."""
        return RedisHealthMonitor(
            redis_config=redis_config,
            health_check_interval=1.0,  # Faster for testing
            unhealthy_threshold=0.6,
            degraded_threshold=0.3,
            latency_threshold_ms=500.0
        )
    
    def test_initialization(self, health_monitor):
        """Test RedisHealthMonitor initialization."""
        assert health_monitor.status == HealthStatus.UNKNOWN
        assert not health_monitor.is_healthy
        assert not health_monitor.is_circuit_open
        assert health_monitor.circuit_breaker.state == CircuitBreakerState.CLOSED
    
    def test_status_properties(self, health_monitor):
        """Test status property methods."""
        # Initial state
        assert health_monitor.status == HealthStatus.UNKNOWN
        assert not health_monitor.is_healthy
        assert not health_monitor.is_circuit_open
        
        # Change status
        health_monitor._current_status = HealthStatus.HEALTHY
        assert health_monitor.is_healthy
        
        # Open circuit
        health_monitor.circuit_breaker._state = CircuitBreakerState.OPEN
        assert health_monitor.is_circuit_open
    
    @pytest.mark.asyncio
    async def test_successful_health_check(self, health_monitor):
        """Test successful health check."""
        # Mock Redis operations
        mock_redis = AsyncMock()
        mock_redis.ping.return_value = True
        mock_redis.set.return_value = True
        mock_redis.get.return_value = "test_value"
        mock_redis.delete.return_value = 1
        mock_redis.xadd.return_value = "1234567890-0"
        mock_redis.xread.return_value = [("test_stream", [("1234567890-0", {"test": "health_check"})])]
        
        health_monitor._async_redis = mock_redis
        
        # Perform health check
        status = await health_monitor.check_health()
        
        # Verify results
        assert status == HealthStatus.HEALTHY
        assert health_monitor.metrics.total_checks == 1
        assert health_monitor.metrics.successful_checks == 1
        assert health_monitor.metrics.failed_checks == 0
        assert health_monitor.metrics.consecutive_successes == 1
        assert health_monitor.metrics.consecutive_failures == 0
    
    @pytest.mark.asyncio
    async def test_failed_health_check(self, health_monitor):
        """Test failed health check."""
        # Mock Redis operations to fail
        mock_redis = AsyncMock()
        mock_redis.ping.side_effect = ConnectionError("Connection refused")
        
        health_monitor._async_redis = mock_redis
        
        # Perform health check
        status = await health_monitor.check_health()
        
        # Verify results
        assert status in [HealthStatus.DEGRADED, HealthStatus.UNHEALTHY]
        assert health_monitor.metrics.total_checks == 1
        assert health_monitor.metrics.successful_checks == 0
        assert health_monitor.metrics.failed_checks == 1
        assert health_monitor.metrics.consecutive_failures == 1
    
    @pytest.mark.asyncio
    async def test_latency_threshold_detection(self, health_monitor):
        """Test latency threshold detection."""
        # Mock slow Redis operations
        mock_redis = AsyncMock()
        
        async def slow_ping():
            await asyncio.sleep(0.6)  # Simulate 600ms latency
            return True
        
        mock_redis.ping.side_effect = slow_ping
        mock_redis.set.return_value = True
        mock_redis.get.return_value = "test_value"
        mock_redis.delete.return_value = 1
        mock_redis.xadd.return_value = "1234567890-0"
        mock_redis.xread.return_value = [("test_stream", [("1234567890-0", {"test": "health_check"})])]
        
        health_monitor._async_redis = mock_redis
        
        # Perform health check
        status = await health_monitor.check_health()
        
        # Should be degraded due to high latency
        assert status == HealthStatus.DEGRADED
        assert health_monitor.metrics.current_latency_ms > health_monitor.latency_threshold_ms
    
    @pytest.mark.asyncio 
    async def test_circuit_breaker_integration(self, health_monitor):
        """Test circuit breaker integration with health monitoring."""
        # Mock failing Redis operations
        mock_redis = AsyncMock()
        mock_redis.ping.side_effect = ConnectionError("Connection refused")
        health_monitor._async_redis = mock_redis
        
        # Perform multiple failed health checks to open circuit
        for _ in range(6):  # Exceed minimum calls threshold
            await health_monitor.check_health()
        
        # Circuit should be open now
        assert health_monitor.is_circuit_open
        assert health_monitor.status == HealthStatus.UNHEALTHY
        
        # Next health check should be skipped due to open circuit
        # (but we need to test the circuit breaker's timeout mechanism)
        health_monitor.circuit_breaker.recovery_timeout = 0.1  # Short timeout for testing
        await asyncio.sleep(0.2)
        
        # Mock successful operations for recovery
        mock_redis.ping.side_effect = None
        mock_redis.ping.return_value = True
        mock_redis.set.return_value = True
        mock_redis.get.return_value = "test_value"
        mock_redis.delete.return_value = 1
        mock_redis.xadd.return_value = "1234567890-0"
        mock_redis.xread.return_value = [("test_stream", [("1234567890-0", {"test": "health_check"})])]
        
        # Circuit should allow testing and potentially recover
        await health_monitor.check_health()
        
        # After enough successful checks, circuit should close
        for _ in range(3):
            await health_monitor.check_health()
        
        assert health_monitor.circuit_breaker.state == CircuitBreakerState.CLOSED
        assert health_monitor.status == HealthStatus.HEALTHY
    
    def test_status_change_callback_registration(self, health_monitor):
        """Test status change callback registration."""
        callback_called = []
        
        def test_callback(event: HealthEvent):
            callback_called.append(event)
        
        # Register callback
        callback_id = health_monitor.register_status_change_callback(test_callback)
        assert isinstance(callback_id, str)
        assert len(health_monitor._status_change_callbacks) == 1
        
        # Unregister callback
        health_monitor.unregister_status_change_callback(test_callback)
        assert len(health_monitor._status_change_callbacks) == 0
    
    @pytest.mark.asyncio
    async def test_status_change_notification(self, health_monitor):
        """Test status change event notification."""
        events_received = []
        
        def test_callback(event: HealthEvent):
            events_received.append(event)
        
        health_monitor.register_status_change_callback(test_callback)
        
        # Mock successful health check
        mock_redis = AsyncMock()
        mock_redis.ping.return_value = True
        mock_redis.set.return_value = True
        mock_redis.get.return_value = "test_value"
        mock_redis.delete.return_value = 1
        mock_redis.xadd.return_value = "1234567890-0"
        mock_redis.xread.return_value = [("test_stream", [("1234567890-0", {"test": "health_check"})])]
        health_monitor._async_redis = mock_redis
        
        # Trigger status change
        await health_monitor.check_health()
        
        # Should have received a status change event
        assert len(events_received) == 1
        event = events_received[0]
        assert event.old_status == HealthStatus.UNKNOWN
        assert event.new_status == HealthStatus.HEALTHY
        assert isinstance(event.timestamp, datetime)
    
    def test_webhook_management(self, health_monitor):
        """Test webhook URL management."""
        url1 = "http://example.com/webhook1"
        url2 = "http://example.com/webhook2"
        
        # Add webhooks
        health_monitor.add_webhook_url(url1)
        health_monitor.add_webhook_url(url2)
        assert len(health_monitor._webhook_urls) == 2
        assert url1 in health_monitor._webhook_urls
        assert url2 in health_monitor._webhook_urls
        
        # Remove webhook
        health_monitor.remove_webhook_url(url1)
        assert len(health_monitor._webhook_urls) == 1
        assert url1 not in health_monitor._webhook_urls
        assert url2 in health_monitor._webhook_urls
    
    def test_manual_circuit_breaker_reset(self, health_monitor):
        """Test manual circuit breaker reset."""
        # Force circuit to open
        health_monitor.circuit_breaker._state = CircuitBreakerState.OPEN
        assert health_monitor.is_circuit_open
        
        # Reset circuit
        health_monitor.reset_circuit_breaker()
        assert not health_monitor.is_circuit_open
        assert health_monitor.circuit_breaker.state == CircuitBreakerState.CLOSED
    
    def test_health_summary(self, health_monitor):
        """Test health summary generation."""
        # Set some test data
        health_monitor._current_status = HealthStatus.HEALTHY
        health_monitor.metrics.total_checks = 10
        health_monitor.metrics.successful_checks = 8
        health_monitor.metrics.failed_checks = 2
        health_monitor.metrics.failure_rate = 0.2
        health_monitor.metrics.average_latency_ms = 150.0
        
        summary = health_monitor.get_health_summary()
        
        # Verify summary structure
        assert 'status' in summary
        assert 'circuit_state' in summary
        assert 'metrics' in summary
        assert 'recovery' in summary
        assert 'configuration' in summary
        
        # Verify summary content
        assert summary['status'] == 'healthy'
        assert summary['metrics']['total_checks'] == 10
        assert summary['metrics']['failure_rate'] == 0.2
        assert summary['metrics']['average_latency_ms'] == 150.0
    
    def test_force_health_check_sync(self, health_monitor):
        """Test synchronous force health check."""
        # Mock sync Redis
        with patch('redis.from_url') as mock_redis_factory:
            mock_redis = Mock()
            mock_redis.ping.return_value = True
            mock_redis_factory.return_value = mock_redis
            
            result = health_monitor.force_health_check()
            assert result == True
            
            # Test failure case
            mock_redis.ping.side_effect = ConnectionError("Connection failed")
            result = health_monitor.force_health_check()
            assert result == False
    
    @pytest.mark.asyncio
    async def test_recovery_mechanism(self, health_monitor):
        """Test automatic recovery mechanism."""
        health_monitor._base_recovery_delay = 0.1  # Fast recovery for testing
        health_monitor._max_recovery_attempts = 2
        
        # Mock initially failing, then succeeding Redis
        mock_redis = AsyncMock()
        call_count = 0
        
        def mock_ping():
            nonlocal call_count
            call_count += 1
            if call_count <= 2:
                raise ConnectionError("Connection failed")
            return True
        
        mock_redis.ping.side_effect = mock_ping
        mock_redis.set.return_value = True
        mock_redis.get.return_value = "test_value"
        mock_redis.delete.return_value = 1
        mock_redis.xadd.return_value = "1234567890-0"
        mock_redis.xread.return_value = [("test_stream", [("1234567890-0", {"test": "health_check"})])]
        health_monitor._async_redis = mock_redis
        
        # Force unhealthy status to trigger recovery
        health_monitor._current_status = HealthStatus.UNHEALTHY
        
        # Trigger recovery
        await health_monitor._trigger_recovery()
        
        # Should have attempted recovery
        assert health_monitor._recovery_attempts >= 1
    
    def test_context_manager(self, health_monitor):
        """Test context manager functionality."""
        with patch.object(health_monitor, 'start_monitoring') as mock_start:
            with patch.object(health_monitor, 'stop_monitoring') as mock_stop:
                with health_monitor:
                    pass
                
                mock_start.assert_called_once()
                mock_stop.assert_called_once()
    
    def test_thread_safety(self, health_monitor):
        """Test thread safety of status updates."""
        results = []
        
        def update_status(status):
            for _ in range(100):
                health_monitor._current_status = status
                results.append(health_monitor.status)
        
        # Start multiple threads updating status
        threads = []
        for status in [HealthStatus.HEALTHY, HealthStatus.DEGRADED, HealthStatus.UNHEALTHY]:
            thread = threading.Thread(target=update_status, args=(status,))
            threads.append(thread)
            thread.start()
        
        # Wait for all threads to complete
        for thread in threads:
            thread.join()
        
        # Should have 300 results (100 per thread)
        assert len(results) == 300
        
        # All results should be valid health statuses
        for result in results:
            assert isinstance(result, HealthStatus)


class TestRedisHealthMonitorFailureScenarios:
    """Test various failure scenarios."""
    
    @pytest.fixture
    def health_monitor(self):
        """Create a health monitor for failure testing."""
        config = RedisConfig(
            redis_host='localhost',
            redis_port=6379,
            operation_mode=OperationMode.REDIS_ONLY,
            connection_timeout=1  # Short timeout for faster tests
        )
        return RedisHealthMonitor(
            redis_config=config,
            health_check_interval=0.5,
            unhealthy_threshold=0.5,
            degraded_threshold=0.2
        )
    
    @pytest.mark.asyncio
    async def test_connection_refused_scenario(self, health_monitor):
        """Test Redis connection refused scenario."""
        mock_redis = AsyncMock()
        mock_redis.ping.side_effect = ConnectionError("Connection refused")
        health_monitor._async_redis = mock_redis
        
        status = await health_monitor.check_health()
        
        assert status in [HealthStatus.DEGRADED, HealthStatus.UNHEALTHY]
        assert health_monitor.metrics.failed_checks == 1
        assert health_monitor.metrics.consecutive_failures == 1
    
    @pytest.mark.asyncio
    async def test_timeout_scenario(self, health_monitor):
        """Test Redis timeout scenario."""
        mock_redis = AsyncMock()
        mock_redis.ping.side_effect = TimeoutError("Operation timed out")
        health_monitor._async_redis = mock_redis
        
        status = await health_monitor.check_health()
        
        assert status in [HealthStatus.DEGRADED, HealthStatus.UNHEALTHY]
        assert health_monitor.metrics.failed_checks == 1
    
    @pytest.mark.asyncio
    async def test_redis_error_scenario(self, health_monitor):
        """Test general Redis error scenario."""
        mock_redis = AsyncMock()
        mock_redis.ping.return_value = True
        mock_redis.set.side_effect = RedisError("Redis internal error")
        health_monitor._async_redis = mock_redis
        
        status = await health_monitor.check_health()
        
        assert status in [HealthStatus.DEGRADED, HealthStatus.UNHEALTHY]
        assert health_monitor.metrics.failed_checks == 1
    
    @pytest.mark.asyncio
    async def test_stream_operation_failure(self, health_monitor):
        """Test Redis stream operation failure."""
        mock_redis = AsyncMock()
        mock_redis.ping.return_value = True
        mock_redis.set.return_value = True
        mock_redis.get.return_value = "test_value"
        mock_redis.delete.return_value = 1
        mock_redis.xadd.side_effect = RedisError("Stream operation failed")
        health_monitor._async_redis = mock_redis
        
        status = await health_monitor.check_health()
        
        assert status in [HealthStatus.DEGRADED, HealthStatus.UNHEALTHY]
        assert health_monitor.metrics.failed_checks == 1
    
    @pytest.mark.asyncio
    async def test_intermittent_failure_scenario(self, health_monitor):
        """Test intermittent failure scenario."""
        mock_redis = AsyncMock()
        call_count = 0
        
        def intermittent_ping():
            nonlocal call_count
            call_count += 1
            if call_count % 3 == 0:  # Fail every 3rd call
                raise ConnectionError("Intermittent failure")
            return True
        
        mock_redis.ping.side_effect = intermittent_ping
        mock_redis.set.return_value = True
        mock_redis.get.return_value = "test_value"
        mock_redis.delete.return_value = 1
        mock_redis.xadd.return_value = "1234567890-0"
        mock_redis.xread.return_value = [("test_stream", [("1234567890-0", {"test": "health_check"})])]
        health_monitor._async_redis = mock_redis
        
        # Perform multiple health checks
        statuses = []
        for _ in range(6):
            status = await health_monitor.check_health()
            statuses.append(status)
        
        # Should have mix of healthy and unhealthy statuses
        assert HealthStatus.HEALTHY in statuses or HealthStatus.DEGRADED in statuses
        assert health_monitor.metrics.total_checks == 6
        assert health_monitor.metrics.failed_checks == 2  # Every 3rd call fails
    
    @pytest.mark.asyncio
    async def test_recovery_after_failure(self, health_monitor):
        """Test recovery after sustained failure."""
        mock_redis = AsyncMock()
        
        # Initial failures
        mock_redis.ping.side_effect = ConnectionError("Connection failed")
        health_monitor._async_redis = mock_redis
        
        # Multiple failed checks
        for _ in range(3):
            await health_monitor.check_health()
        
        assert health_monitor.status == HealthStatus.UNHEALTHY
        assert health_monitor.metrics.consecutive_failures == 3
        
        # Now simulate recovery
        mock_redis.ping.side_effect = None
        mock_redis.ping.return_value = True
        mock_redis.set.return_value = True
        mock_redis.get.return_value = "test_value"
        mock_redis.delete.return_value = 1
        mock_redis.xadd.return_value = "1234567890-0"
        mock_redis.xread.return_value = [("test_stream", [("1234567890-0", {"test": "health_check"})])]
        
        # Reset circuit breaker to allow testing
        health_monitor.reset_circuit_breaker()
        
        # Successful check should start recovery
        status = await health_monitor.check_health()
        assert status == HealthStatus.HEALTHY
        assert health_monitor.metrics.consecutive_successes == 1
        assert health_monitor.metrics.consecutive_failures == 0


if __name__ == "__main__":
    # Run tests with pytest
    pytest.main([__file__, "-v"])
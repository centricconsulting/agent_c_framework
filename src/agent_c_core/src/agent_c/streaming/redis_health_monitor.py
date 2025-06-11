"""
Redis Health Monitor for Agent C

Provides comprehensive health monitoring, circuit breaker patterns, auto-recovery,
and event notification systems for Redis connectivity. Designed to work with the
enhanced RedisConfig class and support resilient Redis operation modes.

This module implements:
- Periodic health checking with configurable intervals
- Circuit breaker pattern with state machine
- Automatic recovery mechanisms
- Health status change events and notifications
- Thread-safe operations with comprehensive error handling
"""

import asyncio
import logging
import random
import time
import threading
from dataclasses import dataclass, field
from datetime import datetime, timedelta
from enum import Enum
from typing import Any, Callable, Dict, List, Optional, Union
from urllib.parse import urlparse
import weakref

import redis
import redis.asyncio as aioredis
from redis.exceptions import ConnectionError, TimeoutError, RedisError

from .redis_stream_manager import RedisConfig


logger = logging.getLogger(__name__)


class CircuitBreakerState(Enum):
    """Circuit breaker states."""
    CLOSED = "closed"       # Normal operation, requests pass through
    OPEN = "open"          # Circuit is open, requests fail immediately
    HALF_OPEN = "half_open" # Testing mode, limited requests allowed


class HealthStatus(Enum):
    """Redis health status levels."""
    HEALTHY = "healthy"
    DEGRADED = "degraded"
    UNHEALTHY = "unhealthy"
    UNKNOWN = "unknown"


@dataclass
class HealthMetrics:
    """Health metrics tracking for Redis connection."""
    total_checks: int = 0
    successful_checks: int = 0
    failed_checks: int = 0
    last_check_time: Optional[datetime] = None
    last_success_time: Optional[datetime] = None
    last_failure_time: Optional[datetime] = None
    average_latency_ms: float = 0.0
    current_latency_ms: float = 0.0
    consecutive_failures: int = 0
    consecutive_successes: int = 0
    failure_rate: float = 0.0
    uptime_percentage: float = 100.0
    
    # Latency tracking
    latency_samples: List[float] = field(default_factory=list)
    max_latency_samples: int = 100
    
    def add_latency_sample(self, latency_ms: float):
        """Add a latency sample and maintain rolling window."""
        self.latency_samples.append(latency_ms)
        if len(self.latency_samples) > self.max_latency_samples:
            self.latency_samples.pop(0)
        
        self.current_latency_ms = latency_ms
        if self.latency_samples:
            self.average_latency_ms = sum(self.latency_samples) / len(self.latency_samples)
    
    def calculate_failure_rate(self, window_size: int = 50) -> float:
        """Calculate failure rate over recent checks."""
        if self.total_checks == 0:
            return 0.0
        
        # Use min of window_size and total_checks for calculation
        recent_checks = min(window_size, self.total_checks)
        if recent_checks == 0:
            return 0.0
        
        # Simple approximation based on consecutive failures
        if self.consecutive_failures > 0:
            return min(1.0, self.consecutive_failures / recent_checks)
        
        return max(0.0, 1.0 - (self.consecutive_successes / recent_checks))


@dataclass
class HealthEvent:
    """Health status change event."""
    timestamp: datetime
    old_status: HealthStatus
    new_status: HealthStatus
    circuit_state: CircuitBreakerState
    metrics: HealthMetrics
    reason: str
    details: Dict[str, Any] = field(default_factory=dict)


class CircuitBreaker:
    """
    Circuit breaker implementation for Redis health monitoring.
    
    Implements the circuit breaker pattern with:
    - CLOSED: Normal operation
    - OPEN: Failing fast, blocking requests
    - HALF_OPEN: Testing recovery with limited requests
    """
    
    def __init__(self, 
                 failure_threshold: float = 0.5,
                 recovery_timeout: int = 60,
                 half_open_max_calls: int = 3,
                 min_calls_threshold: int = 5):
        """
        Initialize circuit breaker.
        
        Args:
            failure_threshold: Failure rate threshold to open circuit (0.0-1.0)
            recovery_timeout: Seconds to wait before attempting recovery
            half_open_max_calls: Max calls allowed in half-open state
            min_calls_threshold: Min calls before considering failure rate
        """
        self.failure_threshold = failure_threshold
        self.recovery_timeout = recovery_timeout
        self.half_open_max_calls = half_open_max_calls
        self.min_calls_threshold = min_calls_threshold
        
        self._state = CircuitBreakerState.CLOSED
        self._last_failure_time: Optional[datetime] = None
        self._half_open_calls = 0
        self._lock = threading.RLock()
        
    @property
    def state(self) -> CircuitBreakerState:
        """Get current circuit breaker state."""
        with self._lock:
            return self._state
    
    def can_execute(self) -> bool:
        """Check if operation can be executed."""
        with self._lock:
            if self._state == CircuitBreakerState.CLOSED:
                return True
            elif self._state == CircuitBreakerState.OPEN:
                # Check if recovery timeout has elapsed
                if (self._last_failure_time and 
                    datetime.utcnow() - self._last_failure_time >= timedelta(seconds=self.recovery_timeout)):
                    self._transition_to_half_open()
                    return True
                return False
            elif self._state == CircuitBreakerState.HALF_OPEN:
                return self._half_open_calls < self.half_open_max_calls
            
            return False
    
    def record_success(self, metrics: HealthMetrics):
        """Record successful operation."""
        with self._lock:
            if self._state == CircuitBreakerState.HALF_OPEN:
                self._half_open_calls += 1
                # If we've had enough successful calls, close the circuit
                if self._half_open_calls >= self.half_open_max_calls:
                    self._transition_to_closed()
            elif self._state == CircuitBreakerState.OPEN:
                # Should not happen, but handle gracefully
                self._transition_to_half_open()
    
    def record_failure(self, metrics: HealthMetrics):
        """Record failed operation."""
        with self._lock:
            self._last_failure_time = datetime.utcnow()
            
            if self._state == CircuitBreakerState.HALF_OPEN:
                # Failure during half-open immediately opens circuit
                self._transition_to_open()
            elif self._state == CircuitBreakerState.CLOSED:
                # Check if we should open the circuit
                if (metrics.total_checks >= self.min_calls_threshold and 
                    metrics.failure_rate >= self.failure_threshold):
                    self._transition_to_open()
    
    def reset(self):
        """Reset circuit breaker to closed state."""
        with self._lock:
            self._state = CircuitBreakerState.CLOSED
            self._half_open_calls = 0
            self._last_failure_time = None
    
    def _transition_to_closed(self):
        """Transition to CLOSED state."""
        logger.info("Circuit breaker transitioning to CLOSED state")
        self._state = CircuitBreakerState.CLOSED
        self._half_open_calls = 0
    
    def _transition_to_open(self):
        """Transition to OPEN state."""
        logger.warning("Circuit breaker transitioning to OPEN state")
        self._state = CircuitBreakerState.OPEN
        self._half_open_calls = 0
    
    def _transition_to_half_open(self):
        """Transition to HALF_OPEN state."""
        logger.info("Circuit breaker transitioning to HALF_OPEN state")
        self._state = CircuitBreakerState.HALF_OPEN
        self._half_open_calls = 0


class RedisHealthMonitor:
    """
    Comprehensive Redis health monitoring and circuit breaker system.
    
    Features:
    - Periodic health checks with configurable intervals
    - Circuit breaker pattern for graceful degradation
    - Automatic recovery attempts with exponential backoff
    - Health status change events and notifications
    - Thread-safe operations with comprehensive error handling
    - Integration with RedisConfig for configuration management
    """
    
    def __init__(self, 
                 redis_config: Optional[RedisConfig] = None,
                 health_check_interval: float = 30.0,
                 unhealthy_threshold: float = 0.6,
                 degraded_threshold: float = 0.3,
                 latency_threshold_ms: float = 1000.0):
        """
        Initialize Redis health monitor.
        
        Args:
            redis_config: Redis configuration object
            health_check_interval: Seconds between health checks
            unhealthy_threshold: Failure rate threshold for unhealthy status
            degraded_threshold: Failure rate threshold for degraded status
            latency_threshold_ms: Latency threshold for degraded status
        """
        self.config = redis_config or RedisConfig()
        self.health_check_interval = health_check_interval
        self.unhealthy_threshold = unhealthy_threshold
        self.degraded_threshold = degraded_threshold
        self.latency_threshold_ms = latency_threshold_ms
        
        # Health tracking
        self.metrics = HealthMetrics()
        self._current_status = HealthStatus.UNKNOWN
        self._lock = threading.RLock()
        
        # Circuit breaker
        self.circuit_breaker = CircuitBreaker(
            failure_threshold=unhealthy_threshold,
            recovery_timeout=self.config.recovery_interval,
            half_open_max_calls=3,
            min_calls_threshold=5
        )
        
        # Background monitoring
        self._monitoring_task: Optional[asyncio.Task] = None
        self._monitoring_active = False
        self._event_loop: Optional[asyncio.AbstractEventLoop] = None
        
        # Recovery tracking
        self._recovery_attempts = 0
        self._last_recovery_attempt: Optional[datetime] = None
        self._max_recovery_attempts = 10
        self._base_recovery_delay = 1.0  # Base delay for exponential backoff
        
        # Event system
        self._status_change_callbacks: List[Callable[[HealthEvent], None]] = []
        self._webhook_urls: List[str] = []
        
        # Redis connections for health checking
        self._sync_redis: Optional[redis.Redis] = None
        self._async_redis: Optional[aioredis.Redis] = None
        
        logger.info(f"RedisHealthMonitor initialized with config: {self.config}")
    
    @property
    def status(self) -> HealthStatus:
        """Get current health status."""
        with self._lock:
            return self._current_status
    
    @property
    def is_healthy(self) -> bool:
        """Check if Redis is currently healthy."""
        return self.status == HealthStatus.HEALTHY
    
    @property
    def is_circuit_open(self) -> bool:
        """Check if circuit breaker is open."""
        return self.circuit_breaker.state == CircuitBreakerState.OPEN
    
    def start_monitoring(self):
        """Start background health monitoring."""
        if self._monitoring_active:
            logger.warning("Health monitoring already active")
            return
        
        self._monitoring_active = True
        
        # Start monitoring in a separate thread to avoid blocking
        def start_monitoring_loop():
            loop = asyncio.new_event_loop()
            asyncio.set_event_loop(loop)
            self._event_loop = loop
            
            try:
                self._monitoring_task = loop.create_task(self._monitoring_loop())
                loop.run_until_complete(self._monitoring_task)
            except asyncio.CancelledError:
                logger.info("Health monitoring stopped")
            except Exception as e:
                logger.error(f"Health monitoring loop error: {e}", exc_info=True)
            finally:
                loop.close()
        
        monitoring_thread = threading.Thread(
            target=start_monitoring_loop,
            name="RedisHealthMonitor",
            daemon=True
        )
        monitoring_thread.start()
        
        logger.info("Started Redis health monitoring")
    
    def stop_monitoring(self):
        """Stop background health monitoring."""
        self._monitoring_active = False
        
        if self._monitoring_task:
            self._monitoring_task.cancel()
        
        if self._event_loop:
            self._event_loop.call_soon_threadsafe(self._event_loop.stop)
        
        # Close Redis connections
        if self._sync_redis:
            try:
                self._sync_redis.close()
            except Exception as e:
                logger.debug(f"Error closing sync Redis connection: {e}")
        
        if self._async_redis:
            try:
                if hasattr(self._async_redis, 'close'):
                    asyncio.create_task(self._async_redis.close())
            except Exception as e:
                logger.debug(f"Error closing async Redis connection: {e}")
        
        logger.info("Stopped Redis health monitoring")
    
    async def _monitoring_loop(self):
        """Main monitoring loop."""
        logger.info("Starting health monitoring loop")
        
        while self._monitoring_active:
            try:
                await self.check_health()
                await asyncio.sleep(self.health_check_interval)
            except asyncio.CancelledError:
                break
            except Exception as e:
                logger.error(f"Error in monitoring loop: {e}", exc_info=True)
                await asyncio.sleep(min(self.health_check_interval, 10.0))
    
    async def check_health(self) -> HealthStatus:
        """
        Perform comprehensive health check.
        
        Returns:
            Current health status
        """
        start_time = time.time()
        check_successful = False
        error_details = {}
        
        try:
            # Check if circuit breaker allows the check
            if not self.circuit_breaker.can_execute():
                logger.debug("Circuit breaker open, skipping health check")
                return self._current_status
            
            # Perform the actual health check
            await self._perform_redis_health_check()
            check_successful = True
            
            # Record latency
            latency_ms = (time.time() - start_time) * 1000
            self.metrics.add_latency_sample(latency_ms)
            
            logger.debug(f"Health check successful, latency: {latency_ms:.2f}ms")
            
        except Exception as e:
            latency_ms = (time.time() - start_time) * 1000
            error_details = {
                'error_type': type(e).__name__,
                'error_message': str(e),
                'latency_ms': latency_ms
            }
            logger.warning(f"Health check failed: {e}")
        
        # Update metrics
        with self._lock:
            self.metrics.total_checks += 1
            self.metrics.last_check_time = datetime.utcnow()
            
            if check_successful:
                self.metrics.successful_checks += 1
                self.metrics.consecutive_successes += 1
                self.metrics.consecutive_failures = 0
                self.metrics.last_success_time = datetime.utcnow()
                self.circuit_breaker.record_success(self.metrics)
                
                # Reset recovery attempts on success
                self._recovery_attempts = 0
                
            else:
                self.metrics.failed_checks += 1
                self.metrics.consecutive_failures += 1
                self.metrics.consecutive_successes = 0
                self.metrics.last_failure_time = datetime.utcnow()
                self.circuit_breaker.record_failure(self.metrics)
        
        # Update failure rate and status
        self.metrics.failure_rate = self.metrics.calculate_failure_rate()
        new_status = self._determine_health_status()
        
        # Check for status change and emit events
        if new_status != self._current_status:
            await self._handle_status_change(new_status, error_details)
        
        return new_status
    
    async def _perform_redis_health_check(self):
        """Perform the actual Redis health check operations."""
        # Create Redis connection if needed
        if not self._async_redis:
            self._async_redis = aioredis.from_url(
                self.config.redis_url,
                decode_responses=True,
                socket_connect_timeout=self.config.connection_timeout,
                socket_timeout=self.config.connection_timeout,
                retry_on_timeout=False
            )
        
        # Test basic connectivity with PING
        pong = await self._async_redis.ping()
        if pong != True:
            raise ConnectionError("PING command failed")
        
        # Test basic read/write operations
        test_key = f"health_check:{int(time.time())}"
        test_value = f"health_check_value_{random.randint(1000, 9999)}"
        
        # SET and GET test
        await self._async_redis.set(test_key, test_value, ex=10)  # 10 second expiry
        retrieved_value = await self._async_redis.get(test_key)
        
        if retrieved_value != test_value:
            raise RedisError(f"GET/SET test failed: expected {test_value}, got {retrieved_value}")
        
        # Clean up test key
        await self._async_redis.delete(test_key)
        
        # Additional checks for stream operations if in Redis mode
        if self.config.operation_mode.value in ['REDIS_ONLY', 'HYBRID']:
            # Test stream operations
            stream_key = f"health_check_stream:{int(time.time())}"
            try:
                # Add a test message to stream
                await self._async_redis.xadd(stream_key, {"test": "health_check"})
                
                # Read from stream
                messages = await self._async_redis.xread({stream_key: '0-0'}, count=1)
                
                if not messages or len(messages) == 0:
                    raise RedisError("Stream read test failed")
                
                # Clean up stream
                await self._async_redis.delete(stream_key)
                
            except Exception as e:
                # Clean up stream on error
                try:
                    await self._async_redis.delete(stream_key)
                except:
                    pass
                raise RedisError(f"Stream operations test failed: {e}")
    
    def _determine_health_status(self) -> HealthStatus:
        """Determine health status based on current metrics."""
        # If circuit breaker is open, status is unhealthy
        if self.circuit_breaker.state == CircuitBreakerState.OPEN:
            return HealthStatus.UNHEALTHY
        
        # Check failure rate thresholds
        if self.metrics.failure_rate >= self.unhealthy_threshold:
            return HealthStatus.UNHEALTHY
        elif self.metrics.failure_rate >= self.degraded_threshold:
            return HealthStatus.DEGRADED
        
        # Check latency thresholds
        if self.metrics.current_latency_ms > self.latency_threshold_ms:
            return HealthStatus.DEGRADED
        
        # Check consecutive failures
        if self.metrics.consecutive_failures >= 3:
            return HealthStatus.UNHEALTHY
        elif self.metrics.consecutive_failures >= 1:
            return HealthStatus.DEGRADED
        
        # If we have successful checks, we're healthy
        if self.metrics.consecutive_successes > 0:
            return HealthStatus.HEALTHY
        
        # Default to unknown if no checks have been performed
        return HealthStatus.UNKNOWN
    
    async def _handle_status_change(self, new_status: HealthStatus, error_details: Dict[str, Any]):
        """Handle health status change."""
        old_status = self._current_status
        
        with self._lock:
            self._current_status = new_status
        
        # Create health event
        event = HealthEvent(
            timestamp=datetime.utcnow(),
            old_status=old_status,
            new_status=new_status,
            circuit_state=self.circuit_breaker.state,
            metrics=self.metrics,
            reason=self._get_status_change_reason(old_status, new_status),
            details=error_details
        )
        
        # Log status change
        logger.info(f"Health status changed: {old_status.value} -> {new_status.value} "
                   f"(circuit: {self.circuit_breaker.state.value}, "
                   f"failure_rate: {self.metrics.failure_rate:.2f})")
        
        # Notify callbacks
        await self._notify_status_change(event)
        
        # Trigger recovery if needed
        if new_status == HealthStatus.UNHEALTHY and old_status != HealthStatus.UNHEALTHY:
            await self._trigger_recovery()
    
    def _get_status_change_reason(self, old_status: HealthStatus, new_status: HealthStatus) -> str:
        """Get human-readable reason for status change."""
        if new_status == HealthStatus.HEALTHY:
            return "Health checks successful, system recovered"
        elif new_status == HealthStatus.DEGRADED:
            if self.metrics.current_latency_ms > self.latency_threshold_ms:
                return f"High latency detected: {self.metrics.current_latency_ms:.2f}ms"
            elif self.metrics.failure_rate >= self.degraded_threshold:
                return f"Elevated failure rate: {self.metrics.failure_rate:.2f}"
            else:
                return "Performance degradation detected"
        elif new_status == HealthStatus.UNHEALTHY:
            if self.circuit_breaker.state == CircuitBreakerState.OPEN:
                return "Circuit breaker opened due to failures"
            elif self.metrics.failure_rate >= self.unhealthy_threshold:
                return f"High failure rate: {self.metrics.failure_rate:.2f}"
            elif self.metrics.consecutive_failures >= 3:
                return f"Multiple consecutive failures: {self.metrics.consecutive_failures}"
            else:
                return "Health checks failing"
        else:
            return "Status determination pending"
    
    async def _notify_status_change(self, event: HealthEvent):
        """Notify registered callbacks and webhooks of status change."""
        # Notify callbacks
        for callback in self._status_change_callbacks:
            try:
                if asyncio.iscoroutinefunction(callback):
                    await callback(event)
                else:
                    callback(event)
            except Exception as e:
                logger.error(f"Error in status change callback: {e}", exc_info=True)
        
        # TODO: Implement webhook notifications
        # This would send HTTP POST requests to registered webhook URLs
        # with the health event data
    
    async def _trigger_recovery(self):
        """Trigger automatic recovery attempts."""
        if self._recovery_attempts >= self._max_recovery_attempts:
            logger.warning(f"Max recovery attempts reached ({self._max_recovery_attempts})")
            return
        
        # Calculate delay with exponential backoff and jitter
        delay = self._base_recovery_delay * (2 ** self._recovery_attempts)
        jitter = random.uniform(0.1, 0.3) * delay
        total_delay = delay + jitter
        
        logger.info(f"Scheduling recovery attempt #{self._recovery_attempts + 1} "
                   f"in {total_delay:.2f} seconds")
        
        # Schedule recovery attempt
        await asyncio.sleep(total_delay)
        
        self._recovery_attempts += 1
        self._last_recovery_attempt = datetime.utcnow()
        
        try:
            # Attempt recovery by forcing a health check
            logger.info(f"Attempting recovery #{self._recovery_attempts}")
            
            # Reset circuit breaker for recovery attempt
            if self.circuit_breaker.state == CircuitBreakerState.OPEN:
                # Force transition to half-open for testing
                self.circuit_breaker._transition_to_half_open()
            
            # Perform health check
            await self.check_health()
            
            if self.status == HealthStatus.HEALTHY:
                logger.info("Recovery successful!")
                self._recovery_attempts = 0
            else:
                logger.warning(f"Recovery attempt #{self._recovery_attempts} failed")
                # Schedule next recovery attempt
                if self._recovery_attempts < self._max_recovery_attempts:
                    await self._trigger_recovery()
        
        except Exception as e:
            logger.error(f"Recovery attempt #{self._recovery_attempts} failed: {e}")
            # Schedule next recovery attempt
            if self._recovery_attempts < self._max_recovery_attempts:
                await self._trigger_recovery()
    
    def register_status_change_callback(self, callback: Callable[[HealthEvent], None]) -> str:
        """
        Register a callback for health status changes.
        
        Args:
            callback: Function to call on status changes
            
        Returns:
            Callback ID for later removal
        """
        callback_id = f"callback_{len(self._status_change_callbacks)}_{int(time.time())}"
        self._status_change_callbacks.append(callback)
        logger.debug(f"Registered status change callback: {callback_id}")
        return callback_id
    
    def unregister_status_change_callback(self, callback: Callable[[HealthEvent], None]):
        """Remove a status change callback."""
        try:
            self._status_change_callbacks.remove(callback)
            logger.debug("Unregistered status change callback")
        except ValueError:
            logger.warning("Callback not found for removal")
    
    def add_webhook_url(self, url: str):
        """Add a webhook URL for health status notifications."""
        self._webhook_urls.append(url)
        logger.info(f"Added webhook URL: {url}")
    
    def remove_webhook_url(self, url: str):
        """Remove a webhook URL."""
        try:
            self._webhook_urls.remove(url)
            logger.info(f"Removed webhook URL: {url}")
        except ValueError:
            logger.warning(f"Webhook URL not found: {url}")
    
    def reset_circuit_breaker(self):
        """Manually reset the circuit breaker to closed state."""
        self.circuit_breaker.reset()
        logger.info("Circuit breaker manually reset")
    
    def get_health_summary(self) -> Dict[str, Any]:
        """Get comprehensive health summary."""
        with self._lock:
            return {
                'status': self._current_status.value,
                'circuit_state': self.circuit_breaker.state.value,
                'metrics': {
                    'total_checks': self.metrics.total_checks,
                    'successful_checks': self.metrics.successful_checks,
                    'failed_checks': self.metrics.failed_checks,
                    'failure_rate': round(self.metrics.failure_rate, 3),
                    'average_latency_ms': round(self.metrics.average_latency_ms, 2),
                    'current_latency_ms': round(self.metrics.current_latency_ms, 2),
                    'consecutive_failures': self.metrics.consecutive_failures,
                    'consecutive_successes': self.metrics.consecutive_successes,
                    'last_check_time': self.metrics.last_check_time.isoformat() if self.metrics.last_check_time else None,
                    'last_success_time': self.metrics.last_success_time.isoformat() if self.metrics.last_success_time else None,
                    'last_failure_time': self.metrics.last_failure_time.isoformat() if self.metrics.last_failure_time else None,
                },
                'recovery': {
                    'attempts': self._recovery_attempts,
                    'last_attempt': self._last_recovery_attempt.isoformat() if self._last_recovery_attempt else None,
                    'max_attempts': self._max_recovery_attempts,
                },
                'configuration': {
                    'health_check_interval': self.health_check_interval,
                    'unhealthy_threshold': self.unhealthy_threshold,
                    'degraded_threshold': self.degraded_threshold,
                    'latency_threshold_ms': self.latency_threshold_ms,
                }
            }
    
    def force_health_check(self) -> bool:
        """
        Force an immediate health check synchronously.
        
        Returns:
            True if health check was successful, False otherwise
        """
        try:
            # Create a sync Redis connection for immediate testing
            if not self._sync_redis:
                self._sync_redis = redis.from_url(
                    self.config.redis_url,
                    decode_responses=True,
                    socket_connect_timeout=self.config.connection_timeout,
                    socket_timeout=self.config.connection_timeout,
                    retry_on_timeout=False
                )
            
            # Perform basic connectivity test
            pong = self._sync_redis.ping()
            return pong == True
            
        except Exception as e:
            logger.warning(f"Force health check failed: {e}")
            return False
    
    def __enter__(self):
        """Context manager entry."""
        self.start_monitoring()
        return self
    
    def __exit__(self, exc_type, exc_val, exc_tb):
        """Context manager exit."""
        self.stop_monitoring()
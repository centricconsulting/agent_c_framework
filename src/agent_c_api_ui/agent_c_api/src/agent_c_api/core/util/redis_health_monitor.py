"""
Redis Health Monitor

This module provides comprehensive health monitoring for Redis connections and operations.
It tracks connectivity, performance metrics, and provides health status reports that
can be used by the EventHandlerModeManager for making mode transition decisions.
"""

import asyncio
import logging
import time
from dataclasses import dataclass, field
from typing import Dict, List, Optional, Any, Callable
from collections import deque
import statistics

from ..util.resilient_mode import TransitionReason

logger = logging.getLogger(__name__)


@dataclass
class HealthMetric:
    """Individual health metric measurement."""
    name: str
    value: float
    unit: str
    timestamp: float
    context: Optional[Dict[str, Any]] = None


@dataclass
class HealthStatus:
    """Comprehensive health status report."""
    is_healthy: bool
    connectivity: bool
    performance_score: float  # 0.0 - 1.0
    error_rate: float
    latency_ms: float
    last_check_time: float
    failures_in_window: int
    recovery_time_estimate: Optional[float] = None
    details: Dict[str, Any] = field(default_factory=dict)


class RedisHealthMonitor:
    """
    Monitors Redis health and provides status reports for resilient mode decisions.
    
    This class continuously monitors Redis connectivity, performance, and error rates.
    It provides health status reports that can be used by the EventHandlerModeManager
    to make informed decisions about mode transitions.
    """
    
    def __init__(self, 
                 redis_client_getter: Callable[[], Any],
                 check_interval: float = 10.0,
                 failure_threshold: int = 3,
                 latency_threshold_ms: float = 100.0,
                 error_rate_threshold: float = 0.05):
        """
        Initialize the Redis health monitor.
        
        Args:
            redis_client_getter: Function that returns the Redis client instance
            check_interval: Seconds between health checks
            failure_threshold: Number of consecutive failures before marking unhealthy
            latency_threshold_ms: Maximum acceptable latency in milliseconds
            error_rate_threshold: Maximum acceptable error rate (0.0-1.0)
        """
        self.redis_client_getter = redis_client_getter
        self.check_interval = check_interval
        self.failure_threshold = failure_threshold
        self.latency_threshold_ms = latency_threshold_ms
        self.error_rate_threshold = error_rate_threshold
        
        # Health tracking
        self.is_running = False
        self.last_health_status: Optional[HealthStatus] = None
        self.consecutive_failures = 0
        self.last_successful_check = time.time()
        
        # Metrics storage (keep last 100 measurements)
        self.latency_history: deque = deque(maxlen=100)
        self.error_history: deque = deque(maxlen=100)
        self.connectivity_history: deque = deque(maxlen=50)
        
        # Callbacks for health status changes
        self.health_callbacks: Dict[str, Callable] = {}
        
        # Background monitoring task
        self.monitor_task: Optional[asyncio.Task] = None
        
        self.logger = logging.getLogger(__name__)
    
    async def start_monitoring(self) -> None:
        """Start background health monitoring."""
        if self.is_running:
            self.logger.warning("Health monitoring is already running")
            return
        
        self.is_running = True
        self.monitor_task = asyncio.create_task(self._monitoring_loop())
        self.logger.info(f"Started Redis health monitoring (interval: {self.check_interval}s)")
    
    async def stop_monitoring(self) -> None:
        """Stop background health monitoring."""
        self.is_running = False
        
        if self.monitor_task:
            self.monitor_task.cancel()
            try:
                await self.monitor_task
            except asyncio.CancelledError:
                pass
            self.monitor_task = None
        
        self.logger.info("Stopped Redis health monitoring")
    
    async def _monitoring_loop(self) -> None:
        """Main monitoring loop."""
        while self.is_running:
            try:
                await self.check_health()
                await asyncio.sleep(self.check_interval)
            except asyncio.CancelledError:
                break
            except Exception as e:
                self.logger.error(f"Error in health monitoring loop: {e}")
                await asyncio.sleep(self.check_interval)
    
    async def check_health(self) -> HealthStatus:
        """
        Perform comprehensive health check.
        
        Returns:
            HealthStatus: Current health status
        """
        check_start_time = time.time()
        
        try:
            # Get Redis client
            redis_client = self.redis_client_getter()
            if not redis_client:
                return self._create_unhealthy_status("Redis client not available")
            
            # Test basic connectivity
            connectivity_result = await self._test_connectivity(redis_client)
            
            # Test performance if connected
            performance_result = None
            if connectivity_result['success']:
                performance_result = await self._test_performance(redis_client)
            
            # Calculate overall health status
            health_status = self._calculate_health_status(
                connectivity_result, 
                performance_result, 
                check_start_time
            )
            
            # Update tracking
            self._update_health_tracking(health_status)
            
            # Store for future reference
            self.last_health_status = health_status
            
            # Notify callbacks if status changed significantly
            await self._notify_health_callbacks(health_status)
            
            return health_status
            
        except Exception as e:
            self.logger.error(f"Health check failed with exception: {e}")
            error_status = self._create_unhealthy_status(f"Health check exception: {e}")
            self.last_health_status = error_status
            await self._notify_health_callbacks(error_status)
            return error_status
    
    async def _test_connectivity(self, redis_client) -> Dict[str, Any]:
        """Test basic Redis connectivity."""
        start_time = time.time()
        
        try:
            # Simple ping test
            await redis_client.ping()
            latency = (time.time() - start_time) * 1000  # Convert to ms
            
            self.connectivity_history.append(True)
            self.latency_history.append(latency)
            
            return {
                'success': True,
                'latency_ms': latency,
                'error': None
            }
            
        except Exception as e:
            self.connectivity_history.append(False)
            self.error_history.append(time.time())
            
            return {
                'success': False,
                'latency_ms': float('inf'),
                'error': str(e)
            }
    
    async def _test_performance(self, redis_client) -> Dict[str, Any]:
        """Test Redis performance with more comprehensive operations."""
        start_time = time.time()
        
        try:
            # Test multiple operations
            operations = []
            
            # Test key operations
            test_key = f"health_check_{int(time.time())}"
            
            # SET operation
            set_start = time.time()
            await redis_client.set(test_key, "health_check_value", ex=60)
            set_latency = (time.time() - set_start) * 1000
            operations.append(('SET', set_latency))
            
            # GET operation
            get_start = time.time()
            value = await redis_client.get(test_key)
            get_latency = (time.time() - get_start) * 1000
            operations.append(('GET', get_latency))
            
            # DELETE operation
            del_start = time.time()
            await redis_client.delete(test_key)
            del_latency = (time.time() - del_start) * 1000
            operations.append(('DEL', del_latency))
            
            # Test stream operations (relevant for our use case)
            stream_key = f"health_stream_{int(time.time())}"
            
            # XADD operation
            xadd_start = time.time()
            await redis_client.xadd(stream_key, {"test": "health_check"})
            xadd_latency = (time.time() - xadd_start) * 1000
            operations.append(('XADD', xadd_latency))
            
            # Cleanup stream
            await redis_client.delete(stream_key)
            
            total_latency = (time.time() - start_time) * 1000
            avg_latency = statistics.mean([op[1] for op in operations])
            
            return {
                'success': True,
                'total_latency_ms': total_latency,
                'avg_operation_latency_ms': avg_latency,
                'operations': operations,
                'error': None
            }
            
        except Exception as e:
            self.error_history.append(time.time())
            
            return {
                'success': False,
                'total_latency_ms': float('inf'),
                'avg_operation_latency_ms': float('inf'),
                'operations': [],
                'error': str(e)
            }
    
    def _calculate_health_status(self, 
                               connectivity_result: Dict[str, Any], 
                               performance_result: Optional[Dict[str, Any]],
                               check_time: float) -> HealthStatus:
        """Calculate overall health status from test results."""
        
        # Basic connectivity
        is_connected = connectivity_result['success']
        
        # Performance metrics
        latency_ms = connectivity_result.get('latency_ms', float('inf'))
        
        if performance_result and performance_result['success']:
            # Use average operation latency if available
            latency_ms = performance_result.get('avg_operation_latency_ms', latency_ms)
        
        # Calculate error rate from recent history
        error_rate = self._calculate_error_rate()
        
        # Calculate performance score (0.0 - 1.0)
        performance_score = self._calculate_performance_score(latency_ms, error_rate)
        
        # Determine overall health
        is_healthy = (
            is_connected and 
            latency_ms < self.latency_threshold_ms and
            error_rate < self.error_rate_threshold and
            self.consecutive_failures < self.failure_threshold
        )
        
        # Estimate recovery time if unhealthy
        recovery_time_estimate = None
        if not is_healthy:
            recovery_time_estimate = self._estimate_recovery_time()
        
        # Gather detailed information
        details = {
            'connectivity_test': connectivity_result,
            'performance_test': performance_result,
            'consecutive_failures': self.consecutive_failures,
            'failure_threshold': self.failure_threshold,
            'latency_threshold_ms': self.latency_threshold_ms,
            'error_rate_threshold': self.error_rate_threshold,
            'recent_connectivity_rate': self._calculate_connectivity_rate(),
            'latency_history_stats': self._get_latency_stats()
        }
        
        return HealthStatus(
            is_healthy=is_healthy,
            connectivity=is_connected,
            performance_score=performance_score,
            error_rate=error_rate,
            latency_ms=latency_ms,
            last_check_time=check_time,
            failures_in_window=self.consecutive_failures,
            recovery_time_estimate=recovery_time_estimate,
            details=details
        )
    
    def _calculate_error_rate(self) -> float:
        """Calculate error rate from recent history."""
        if not self.error_history:
            return 0.0
        
        # Count errors in last 5 minutes
        current_time = time.time()
        recent_errors = [
            error_time for error_time in self.error_history
            if current_time - error_time < 300  # 5 minutes
        ]
        
        # Estimate total operations in 5 minutes
        # Assuming one check per interval
        total_operations = max(1, 300 // self.check_interval)
        
        return min(1.0, len(recent_errors) / total_operations)
    
    def _calculate_performance_score(self, latency_ms: float, error_rate: float) -> float:
        """Calculate performance score from 0.0 (worst) to 1.0 (best)."""
        if latency_ms == float('inf'):
            return 0.0
        
        # Latency score (1.0 if under threshold, decreases as latency increases)
        latency_score = max(0.0, 1.0 - (latency_ms / (self.latency_threshold_ms * 2)))
        
        # Error rate score (1.0 if no errors, decreases as error rate increases)
        error_score = max(0.0, 1.0 - (error_rate / (self.error_rate_threshold * 2)))
        
        # Connectivity score from recent history
        connectivity_score = self._calculate_connectivity_rate()
        
        # Weighted average
        return (latency_score * 0.4 + error_score * 0.3 + connectivity_score * 0.3)
    
    def _calculate_connectivity_rate(self) -> float:
        """Calculate recent connectivity success rate."""
        if not self.connectivity_history:
            return 0.0
        
        successful_connections = sum(1 for connected in self.connectivity_history if connected)
        return successful_connections / len(self.connectivity_history)
    
    def _get_latency_stats(self) -> Dict[str, float]:
        """Get statistical summary of recent latency measurements."""
        if not self.latency_history:
            return {}
        
        latencies = [l for l in self.latency_history if l != float('inf')]
        if not latencies:
            return {}
        
        return {
            'count': len(latencies),
            'mean': statistics.mean(latencies),
            'median': statistics.median(latencies),
            'min': min(latencies),
            'max': max(latencies),
            'stdev': statistics.stdev(latencies) if len(latencies) > 1 else 0.0
        }
    
    def _estimate_recovery_time(self) -> float:
        """Estimate time until Redis might recover (in seconds)."""
        # Simple estimation based on consecutive failures
        base_time = 30.0  # Base recovery time
        failure_multiplier = min(5.0, self.consecutive_failures * 0.5)
        return base_time * failure_multiplier
    
    def _update_health_tracking(self, health_status: HealthStatus) -> None:
        """Update internal health tracking based on current status."""
        if health_status.is_healthy:
            self.consecutive_failures = 0
            self.last_successful_check = time.time()
        else:
            self.consecutive_failures += 1
    
    def _create_unhealthy_status(self, reason: str) -> HealthStatus:
        """Create an unhealthy status with the given reason."""
        self.consecutive_failures += 1
        
        return HealthStatus(
            is_healthy=False,
            connectivity=False,
            performance_score=0.0,
            error_rate=1.0,
            latency_ms=float('inf'),
            last_check_time=time.time(),
            failures_in_window=self.consecutive_failures,
            recovery_time_estimate=self._estimate_recovery_time(),
            details={'error': reason}
        )
    
    async def _notify_health_callbacks(self, health_status: HealthStatus) -> None:
        """Notify registered callbacks of health status changes."""
        for callback_name, callback in self.health_callbacks.items():
            try:
                if asyncio.iscoroutinefunction(callback):
                    await callback(health_status)
                else:
                    callback(health_status)
            except Exception as e:
                self.logger.error(f"Health callback {callback_name} error: {e}")
    
    def register_health_callback(self, name: str, callback: Callable) -> None:
        """Register a callback for health status changes."""
        self.health_callbacks[name] = callback
    
    def unregister_health_callback(self, name: str) -> None:
        """Unregister a health status callback."""
        self.health_callbacks.pop(name, None)
    
    async def is_redis_healthy(self) -> bool:
        """
        Quick health check method for external use.
        
        Returns:
            bool: True if Redis is considered healthy
        """
        if self.last_health_status:
            # Use cached status if recent (within last check interval * 2)
            age = time.time() - self.last_health_status.last_check_time
            if age < (self.check_interval * 2):
                return self.last_health_status.is_healthy
        
        # Perform fresh check
        status = await self.check_health()
        return status.is_healthy
    
    def get_health_summary(self) -> Dict[str, Any]:
        """Get summary of current health status."""
        if not self.last_health_status:
            return {
                'status': 'unknown',
                'message': 'No health check performed yet'
            }
        
        status = self.last_health_status
        
        summary = {
            'status': 'healthy' if status.is_healthy else 'unhealthy',
            'connectivity': status.connectivity,
            'performance_score': status.performance_score,
            'error_rate': status.error_rate,
            'latency_ms': status.latency_ms,
            'consecutive_failures': status.failures_in_window,
            'last_check_time': status.last_check_time,
            'monitoring_active': self.is_running
        }
        
        if not status.is_healthy and status.recovery_time_estimate:
            summary['estimated_recovery_time'] = status.recovery_time_estimate
        
        return summary
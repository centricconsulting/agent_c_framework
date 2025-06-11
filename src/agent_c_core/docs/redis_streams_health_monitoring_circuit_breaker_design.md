# Redis Health Monitoring and Circuit Breaker System Design

## Overview

This document details the design of a robust Redis health monitoring and circuit breaker system for the Redis Streams resilient mode implementation. The system provides comprehensive health monitoring, intelligent circuit breaking, and automatic recovery capabilities.

## Health Monitoring Class Design

### `RedisHealthMonitor` Class

```python
class RedisHealthMonitor:
    """Comprehensive Redis health monitoring system."""
    
    def __init__(self, redis_client, config, callback_handler=None):
        """Initialize the health monitor.
        
        Args:
            redis_client: Redis client instance to monitor
            config: HealthCheckConfig instance with monitoring settings
            callback_handler: Optional callback handler for health status changes
        """
        self.redis_client = redis_client
        self.config = config
        self.callback_handler = callback_handler or DefaultCallbackHandler()
        
        # Health check components
        self.connection_check = ConnectionHealthCheck(redis_client, config)
        self.operation_check = OperationHealthCheck(redis_client, config)
        self.memory_check = MemoryHealthCheck(redis_client, config)
        
        # Health status tracking
        self.current_status = HealthStatus.UNKNOWN
        self.status_history = collections.deque(maxlen=100)
        self.failure_count = 0
        self.success_count = 0
        self.last_check_time = None
        self.error_rate = MovingAverageRateTracker(window_size=10)
        
        # Circuit breaker
        self.circuit_breaker = RedisCircuitBreaker(config)
        
        # Monitoring thread
        self.monitoring_thread = None
        self.monitoring_active = False
        self._lock = threading.RLock()
    
    def start_monitoring(self):
        """Start the health monitoring thread."""
        with self._lock:
            if self.monitoring_active:
                return
            
            self.monitoring_active = True
            self.monitoring_thread = threading.Thread(
                target=self._monitoring_loop,
                daemon=True,
                name="RedisHealthMonitor"
            )
            self.monitoring_thread.start()
            logging.info("Redis health monitoring started")
    
    def stop_monitoring(self):
        """Stop the health monitoring thread."""
        with self._lock:
            if not self.monitoring_active:
                return
            
            self.monitoring_active = False
            if self.monitoring_thread:
                self.monitoring_thread.join(timeout=5.0)
                self.monitoring_thread = None
            logging.info("Redis health monitoring stopped")
    
    def _monitoring_loop(self):
        """Main monitoring loop that runs health checks periodically."""
        while self.monitoring_active:
            try:
                # Run health check
                self.check_health()
                
                # Sleep until next check interval
                time.sleep(self.config.interval)
            except Exception as e:
                logging.error(f"Error in Redis health monitoring loop: {e}")
                time.sleep(1.0)  # Short sleep on error
    
    def check_health(self):
        """Run a complete health check and update status."""
        with self._lock:
            self.last_check_time = time.time()
            
            # Run individual health checks
            connection_status = self.connection_check.check()
            operation_status = self.operation_check.check() if connection_status.healthy else HealthStatus.UNHEALTHY
            memory_status = self.memory_check.check() if connection_status.healthy else HealthStatus.UNKNOWN
            
            # Determine overall status
            if not connection_status.healthy:
                new_status = HealthStatus.UNHEALTHY
                error_details = connection_status.details
            elif not operation_status.healthy:
                new_status = HealthStatus.UNHEALTHY
                error_details = operation_status.details
            elif not memory_status.healthy:
                new_status = HealthStatus.DEGRADED
                error_details = memory_status.details
            else:
                new_status = HealthStatus.HEALTHY
                error_details = None
            
            # Update health tracking
            previous_status = self.current_status
            self.current_status = new_status
            self.status_history.append((time.time(), new_status))
            
            # Update success/failure counts
            if new_status == HealthStatus.HEALTHY:
                self.success_count += 1
                self.failure_count = 0
                self.error_rate.add_sample(0.0)
            else:
                self.failure_count += 1
                self.success_count = 0
                self.error_rate.add_sample(1.0)
            
            # Update circuit breaker
            if new_status == HealthStatus.UNHEALTHY:
                # Open circuit on unhealthy state
                self.circuit_breaker.record_failure(error_details)
            elif new_status == HealthStatus.HEALTHY:
                # Close circuit on healthy state
                self.circuit_breaker.record_success()
            
            # Notify on status change
            if previous_status != new_status:
                self._notify_status_change(previous_status, new_status, error_details)
            
            return new_status
    
    def _notify_status_change(self, previous_status, new_status, error_details):
        """Notify about health status changes."""
        logging.info(f"Redis health status changed: {previous_status} -> {new_status}")
        if error_details:
            logging.info(f"Health check details: {error_details}")
        
        # Notify via callback handler
        self.callback_handler.on_health_status_change(
            previous_status=previous_status,
            new_status=new_status,
            error_details=error_details,
            circuit_state=self.circuit_breaker.state
        )
    
    def is_healthy(self):
        """Returns whether Redis is currently healthy."""
        return self.current_status == HealthStatus.HEALTHY and self.circuit_breaker.is_closed()
    
    def get_status_report(self):
        """Get a comprehensive health status report."""
        with self._lock:
            return {
                "status": self.current_status.name,
                "circuit_state": self.circuit_breaker.state.name,
                "failure_count": self.failure_count,
                "success_count": self.success_count,
                "error_rate": self.error_rate.get_rate(),
                "last_check_time": self.last_check_time,
                "connection_status": self.connection_check.get_status(),
                "operation_status": self.operation_check.get_status(),
                "memory_status": self.memory_check.get_status(),
                "status_history": [(ts, status.name) for ts, status in self.status_history]
            }
```

### Health Check Components

```python
class ConnectionHealthCheck:
    """Redis connection health check."""
    
    def __init__(self, redis_client, config):
        self.redis_client = redis_client
        self.config = config
        self.last_status = HealthStatus.UNKNOWN
        self.last_latency = None
    
    def check(self):
        """Check Redis connection health."""
        try:
            # Measure ping latency
            start_time = time.time()
            result = self.redis_client.ping()
            latency = time.time() - start_time
            self.last_latency = latency
            
            if result:
                self.last_status = HealthStatus.HEALTHY
                return HealthCheckResult(
                    healthy=True,
                    details={
                        "latency": latency,
                        "connection": "successful"
                    }
                )
            else:
                self.last_status = HealthStatus.UNHEALTHY
                return HealthCheckResult(
                    healthy=False,
                    details={
                        "error": "Ping returned false",
                        "latency": latency
                    }
                )
        except Exception as e:
            self.last_status = HealthStatus.UNHEALTHY
            return HealthCheckResult(
                healthy=False,
                details={
                    "error": str(e),
                    "error_type": type(e).__name__
                }
            )
    
    def get_status(self):
        """Get the current status of this health check."""
        return {
            "status": self.last_status.name,
            "latency": self.last_latency
        }


class OperationHealthCheck:
    """Redis stream operations health check."""
    
    def __init__(self, redis_client, config):
        self.redis_client = redis_client
        self.config = config
        self.last_status = HealthStatus.UNKNOWN
        self.last_latency = None
        self.test_stream_name = "_health_check_stream"
    
    def check(self):
        """Check Redis stream operations health."""
        try:
            # Create a unique message ID
            message_id = f"health_check_{time.time()}"
            
            # Test write operation
            start_time = time.time()
            result = self.redis_client.xadd(
                self.test_stream_name,
                {"health_check": message_id},
                maxlen=10,
                approximate=True
            )
            write_latency = time.time() - start_time
            
            # Test read operation
            start_time = time.time()
            messages = self.redis_client.xread(
                {self.test_stream_name: "0-0"},
                count=1,
                block=100
            )
            read_latency = time.time() - start_time
            
            # Calculate total latency
            self.last_latency = write_latency + read_latency
            
            # Verify successful operations
            if result and messages:
                self.last_status = HealthStatus.HEALTHY
                return HealthCheckResult(
                    healthy=True,
                    details={
                        "write_latency": write_latency,
                        "read_latency": read_latency,
                        "total_latency": self.last_latency
                    }
                )
            else:
                self.last_status = HealthStatus.UNHEALTHY
                return HealthCheckResult(
                    healthy=False,
                    details={
                        "error": "Stream operations failed",
                        "write_result": bool(result),
                        "read_result": bool(messages)
                    }
                )
        except Exception as e:
            self.last_status = HealthStatus.UNHEALTHY
            return HealthCheckResult(
                healthy=False,
                details={
                    "error": str(e),
                    "error_type": type(e).__name__
                }
            )
    
    def get_status(self):
        """Get the current status of this health check."""
        return {
            "status": self.last_status.name,
            "latency": self.last_latency
        }


class MemoryHealthCheck:
    """Redis memory usage health check."""
    
    def __init__(self, redis_client, config):
        self.redis_client = redis_client
        self.config = config
        self.last_status = HealthStatus.UNKNOWN
        self.memory_used = None
        self.memory_total = None
        self.memory_usage_ratio = None
    
    def check(self):
        """Check Redis memory health."""
        try:
            # Get memory info
            info = self.redis_client.info("memory")
            
            # Extract key metrics
            self.memory_used = info.get("used_memory", 0)
            self.memory_total = info.get("total_system_memory", 0)
            if self.memory_total and self.memory_total > 0:
                self.memory_usage_ratio = self.memory_used / self.memory_total
            else:
                self.memory_usage_ratio = 0
            
            # Check memory usage threshold (default 90%)
            memory_threshold = float(os.environ.get("REDIS_MEMORY_THRESHOLD", "0.9"))
            if self.memory_usage_ratio > memory_threshold:
                self.last_status = HealthStatus.DEGRADED
                return HealthCheckResult(
                    healthy=False,
                    details={
                        "memory_used": self.memory_used,
                        "memory_total": self.memory_total,
                        "memory_usage_ratio": self.memory_usage_ratio,
                        "threshold": memory_threshold
                    }
                )
            else:
                self.last_status = HealthStatus.HEALTHY
                return HealthCheckResult(
                    healthy=True,
                    details={
                        "memory_used": self.memory_used,
                        "memory_total": self.memory_total,
                        "memory_usage_ratio": self.memory_usage_ratio
                    }
                )
        except Exception as e:
            self.last_status = HealthStatus.UNKNOWN
            return HealthCheckResult(
                healthy=True,  # Don't fail just because we can't check memory
                details={
                    "error": str(e),
                    "error_type": type(e).__name__
                }
            )
    
    def get_status(self):
        """Get the current status of this health check."""
        return {
            "status": self.last_status.name,
            "memory_used": self.memory_used,
            "memory_total": self.memory_total,
            "memory_usage_ratio": self.memory_usage_ratio
        }
```

### Support Classes

```python
class HealthStatus(Enum):
    """Enumeration of health status values."""
    HEALTHY = "healthy"      # All systems operational
    DEGRADED = "degraded"    # Operational but with issues
    UNHEALTHY = "unhealthy"  # Not operational
    UNKNOWN = "unknown"      # Status not determined


class HealthCheckResult:
    """Result of a health check operation."""
    
    def __init__(self, healthy, details=None):
        self.healthy = healthy
        self.details = details or {}


class MovingAverageRateTracker:
    """Tracks a moving average rate over a specified window."""
    
    def __init__(self, window_size=10):
        self.window_size = window_size
        self.samples = collections.deque(maxlen=window_size)
    
    def add_sample(self, value):
        """Add a sample (0.0 for success, 1.0 for failure)."""
        self.samples.append(value)
    
    def get_rate(self):
        """Get the current rate (failures / total)."""
        if not self.samples:
            return 0.0
        return sum(self.samples) / len(self.samples)
```

## Circuit Breaker State Machine

### `RedisCircuitBreaker` Class

```python
class CircuitState(Enum):
    """Enumeration of circuit breaker states."""
    CLOSED = "closed"        # Normal operation, requests pass through
    OPEN = "open"            # Failure mode, requests fail fast
    HALF_OPEN = "half_open"  # Testing recovery, limited requests


class RedisCircuitBreaker:
    """Circuit breaker implementation for Redis operations."""
    
    def __init__(self, config):
        """Initialize the circuit breaker.
        
        Args:
            config: HealthCheckConfig instance with circuit breaker settings
        """
        self.config = config
        self.state = CircuitState.CLOSED
        self.failure_count = 0
        self.last_failure_time = None
        self.last_failure_error = None
        self.failure_threshold = config.circuit_breaker_threshold
        self.reset_timeout = config.circuit_reset_timeout
        self.half_open_max_requests = 3  # Maximum requests in half-open state
        self.half_open_request_count = 0
        self.error_rate = MovingAverageRateTracker(window_size=10)
        self._lock = threading.RLock()
    
    def record_failure(self, error_details=None):
        """Record a failure and potentially open the circuit."""
        with self._lock:
            self.last_failure_time = time.time()
            self.last_failure_error = error_details
            self.error_rate.add_sample(1.0)
            
            if self.state == CircuitState.HALF_OPEN:
                # Any failure in half-open state reopens the circuit
                self._transition_to_open()
                return
            
            self.failure_count += 1
            current_error_rate = self.error_rate.get_rate()
            
            # Check if we should open the circuit
            if self.state == CircuitState.CLOSED and current_error_rate >= self.failure_threshold:
                self._transition_to_open()
    
    def record_success(self):
        """Record a success and potentially close the circuit."""
        with self._lock:
            self.error_rate.add_sample(0.0)
            
            if self.state == CircuitState.HALF_OPEN:
                self.half_open_request_count += 1
                
                # Check if we've seen enough successes to close the circuit
                if self.half_open_request_count >= self.half_open_max_requests:
                    self._transition_to_closed()
            
            # Reset failure count on success
            self.failure_count = 0
    
    def _transition_to_open(self):
        """Transition to the open state."""
        previous_state = self.state
        self.state = CircuitState.OPEN
        
        if previous_state != CircuitState.OPEN:
            logging.warning(f"Circuit breaker opened. Last error: {self.last_failure_error}")
    
    def _transition_to_half_open(self):
        """Transition to the half-open state."""
        previous_state = self.state
        self.state = CircuitState.HALF_OPEN
        self.half_open_request_count = 0
        
        if previous_state != CircuitState.HALF_OPEN:
            logging.info("Circuit breaker half-open, testing recovery")
    
    def _transition_to_closed(self):
        """Transition to the closed state."""
        previous_state = self.state
        self.state = CircuitState.CLOSED
        self.failure_count = 0
        
        if previous_state != CircuitState.CLOSED:
            logging.info("Circuit breaker closed, normal operation resumed")
    
    def is_closed(self):
        """Check if the circuit is closed."""
        with self._lock:
            self._check_timeout()  # Check if we should transition to half-open
            return self.state == CircuitState.CLOSED
    
    def allow_request(self):
        """Check if a request should be allowed through the circuit breaker."""
        with self._lock:
            self._check_timeout()  # Check if we should transition to half-open
            
            if self.state == CircuitState.CLOSED:
                return True
            elif self.state == CircuitState.OPEN:
                return False
            elif self.state == CircuitState.HALF_OPEN:
                # Allow limited requests in half-open state
                return self.half_open_request_count < self.half_open_max_requests
            return False
    
    def _check_timeout(self):
        """Check if timeout has expired and we should attempt recovery."""
        if self.state == CircuitState.OPEN and self.last_failure_time is not None:
            time_since_failure = time.time() - self.last_failure_time
            if time_since_failure >= self.reset_timeout:
                self._transition_to_half_open()
    
    def reset(self):
        """Manually reset the circuit breaker to closed state."""
        with self._lock:
            self._transition_to_closed()
            logging.info("Circuit breaker manually reset")
    
    def get_state(self):
        """Get the current state of the circuit breaker."""
        with self._lock:
            self._check_timeout()  # Update state if needed
            return self.state
    
    def get_metrics(self):
        """Get metrics about the circuit breaker."""
        with self._lock:
            return {
                "state": self.state.name,
                "failure_count": self.failure_count,
                "error_rate": self.error_rate.get_rate(),
                "last_failure_time": self.last_failure_time,
                "time_since_failure": time.time() - self.last_failure_time if self.last_failure_time else None,
                "reset_timeout": self.reset_timeout,
                "half_open_request_count": self.half_open_request_count,
                "half_open_max_requests": self.half_open_max_requests
            }
```

## Recovery Mechanism Flowchart

The recovery mechanism follows this process:

1. **Failure Detection**
   - Health checks detect Redis failure
   - Connection errors during operations
   - Circuit breaker tracks failure rates
   - Memory pressure exceeds thresholds

2. **Fallback Activation**
   - Circuit breaker transitions to OPEN state
   - Redis operations fail fast
   - Automatic switch to async queue mode
   - Components notified of mode change

3. **Recovery Monitoring**
   - Continuous health checks during outage
   - Exponential backoff with jitter for retries
   - Circuit breaker maintains timeout period
   - Metrics recording during recovery attempts

4. **Recovery Validation**
   - Circuit breaker times out and enters HALF_OPEN state
   - Limited Redis operations attempted
   - Full health check suite executed
   - Multiple successful operations required

5. **Gradual Restoration**
   - Progressive increase in Redis operation allowance
   - Circuit breaker allows limited requests
   - Error tracking during recovery phase
   - Any failure returns to OPEN state

6. **Full Recovery**
   - Circuit breaker transitions to CLOSED state
   - Full Redis operations resumed
   - System returns to Redis mode
   - Telemetry records recovery event

## Health Status Reporting Interface

### `HealthStatusReporter` Class

```python
class HealthStatusReporter:
    """Provides standardized health status reporting."""
    
    def __init__(self, health_monitor):
        """Initialize the health status reporter.
        
        Args:
            health_monitor: RedisHealthMonitor instance to report on
        """
        self.health_monitor = health_monitor
    
    def get_simple_status(self):
        """Get a simple health status suitable for health checks."""
        is_healthy = self.health_monitor.is_healthy()
        return {
            "status": "healthy" if is_healthy else "unhealthy",
            "timestamp": time.time()
        }
    
    def get_detailed_status(self):
        """Get a detailed health status report."""
        health_report = self.health_monitor.get_status_report()
        circuit_metrics = self.health_monitor.circuit_breaker.get_metrics()
        
        return {
            "status": health_report["status"],
            "circuit_state": circuit_metrics["state"],
            "operational_mode": self._get_operational_mode(),
            "metrics": {
                "failure_count": health_report["failure_count"],
                "success_count": health_report["success_count"],
                "error_rate": health_report["error_rate"],
                "connection": {
                    "status": health_report["connection_status"]["status"],
                    "latency_ms": health_report["connection_status"]["latency"] * 1000 if health_report["connection_status"]["latency"] else None
                },
                "operations": {
                    "status": health_report["operation_status"]["status"],
                    "latency_ms": health_report["operation_status"]["latency"] * 1000 if health_report["operation_status"]["latency"] else None
                },
                "memory": {
                    "status": health_report["memory_status"]["status"],
                    "usage_ratio": health_report["memory_status"]["memory_usage_ratio"],
                    "used_bytes": health_report["memory_status"]["memory_used"],
                    "total_bytes": health_report["memory_status"]["memory_total"]
                }
            },
            "circuit_breaker": {
                "state": circuit_metrics["state"],
                "error_rate": circuit_metrics["error_rate"],
                "last_failure": circuit_metrics["last_failure_time"],
                "time_since_failure_sec": circuit_metrics["time_since_failure"],
                "reset_timeout_sec": circuit_metrics["reset_timeout"]
            },
            "timestamp": time.time(),
            "last_check_time": health_report["last_check_time"]
        }
    
    def get_prometheus_metrics(self):
        """Get health metrics in Prometheus format."""
        health_report = self.health_monitor.get_status_report()
        circuit_metrics = self.health_monitor.circuit_breaker.get_metrics()
        
        metrics = [
            f"redis_health_status{{status=\"{health_report['status']}\", circuit_state=\"{circuit_metrics['state']}\"}} 1",
            f"redis_health_error_rate {health_report['error_rate']}",
            f"redis_health_failure_count {health_report['failure_count']}",
            f"redis_health_success_count {health_report['success_count']}"
        ]
        
        # Add connection metrics if available
        if health_report["connection_status"]["latency"]:
            metrics.append(f"redis_connection_latency_seconds {health_report['connection_status']['latency']}")
        
        # Add operation metrics if available
        if health_report["operation_status"]["latency"]:
            metrics.append(f"redis_operation_latency_seconds {health_report['operation_status']['latency']}")
        
        # Add memory metrics if available
        if health_report["memory_status"]["memory_usage_ratio"]:
            metrics.append(f"redis_memory_usage_ratio {health_report['memory_status']['memory_usage_ratio']}")
        
        # Add circuit breaker metrics
        metrics.append(f"redis_circuit_breaker_state{{state=\"{circuit_metrics['state']}\"}} 1")
        metrics.append(f"redis_circuit_breaker_error_rate {circuit_metrics['error_rate']}")
        
        return "\n".join(metrics)
    
    def get_recommendations(self):
        """Get actionable recommendations based on current health status."""
        health_report = self.health_monitor.get_status_report()
        circuit_metrics = self.health_monitor.circuit_breaker.get_metrics()
        
        recommendations = []
        
        # Circuit breaker recommendations
        if circuit_metrics["state"] == "OPEN":
            recommendations.append({
                "type": "circuit_breaker",
                "message": "Circuit breaker is open. Redis operations will fail fast.",
                "action": "Wait for automatic recovery or manually reset circuit breaker if Redis is confirmed healthy.",
                "severity": "high"
            })
        
        # Connection recommendations
        if health_report["connection_status"]["status"] == "UNHEALTHY":
            recommendations.append({
                "type": "connection",
                "message": "Redis connection is failing.",
                "action": "Check Redis server status, network connectivity, and credentials.",
                "severity": "high"
            })
        
        # Memory recommendations
        if health_report["memory_status"]["status"] == "DEGRADED":
            memory_ratio = health_report["memory_status"]["memory_usage_ratio"]
            if memory_ratio > 0.95:
                recommendations.append({
                    "type": "memory",
                    "message": f"Redis memory usage critical at {memory_ratio:.1%}.",
                    "action": "Increase Redis memory, reduce data, or implement key eviction policies.",
                    "severity": "critical"
                })
            else:
                recommendations.append({
                    "type": "memory",
                    "message": f"Redis memory usage high at {memory_ratio:.1%}.",
                    "action": "Monitor memory usage and plan for scaling if trend continues.",
                    "severity": "medium"
                })
        
        # Performance recommendations
        if health_report["operation_status"]["status"] == "HEALTHY" and \
           health_report["operation_status"]["latency"] and \
           health_report["operation_status"]["latency"] > 0.1:  # >100ms
            recommendations.append({
                "type": "performance",
                "message": f"Redis operation latency is high ({health_report['operation_status']['latency']*1000:.0f}ms).",
                "action": "Check for network issues, Redis server load, or client connection pool saturation.",
                "severity": "medium"
            })
        
        return recommendations
    
    def _get_operational_mode(self):
        """Determine the current operational mode based on health."""
        is_healthy = self.health_monitor.is_healthy()
        circuit_state = self.health_monitor.circuit_breaker.get_state()
        
        if circuit_state == CircuitState.OPEN:
            return "FALLBACK_MODE"
        elif circuit_state == CircuitState.HALF_OPEN:
            return "RECOVERY_MODE"
        elif is_healthy:
            return "REDIS_MODE"
        else:
            return "DEGRADED_MODE"
```

## Integration Points with EventHandlerService

### `RedisResilienceManager` Class

```python
class RedisResilienceManager:
    """Master orchestration class for Redis resilience."""
    
    def __init__(self, redis_client, config, event_handler_service=None):
        """Initialize the Redis resilience manager.
        
        Args:
            redis_client: Redis client instance to monitor
            config: ResilientRedisConfig instance with configuration
            event_handler_service: Optional EventHandlerService instance for integration
        """
        self.redis_client = redis_client
        self.config = config
        self.event_handler_service = event_handler_service
        
        # Create health monitor with callback to this manager
        self.callback_handler = ResilienceCallbackHandler(self)
        self.health_monitor = RedisHealthMonitor(
            redis_client, 
            config.health_check, 
            self.callback_handler
        )
        
        # Create status reporter
        self.status_reporter = HealthStatusReporter(self.health_monitor)
        
        # Track operation mode
        self.current_mode = self._determine_initial_mode()
        self.mode_change_callbacks = []
    
    def start(self):
        """Start monitoring and resilience services."""
        # Start health monitoring
        self.health_monitor.start_monitoring()
        
        # Notify event handler service of startup
        if self.event_handler_service:
            self.event_handler_service.on_resilience_manager_start(self)
        
        logging.info(f"Redis resilience manager started in {self.current_mode} mode")
    
    def stop(self):
        """Stop monitoring and resilience services."""
        # Stop health monitoring
        self.health_monitor.stop_monitoring()
        
        # Notify event handler service of shutdown
        if self.event_handler_service:
            self.event_handler_service.on_resilience_manager_stop(self)
        
        logging.info("Redis resilience manager stopped")
    
    def _determine_initial_mode(self):
        """Determine the initial operation mode based on configuration and health."""
        # Check initial health
        initial_health = self.health_monitor.check_health()
        is_healthy = initial_health == HealthStatus.HEALTHY
        
        # Determine mode based on config and health
        if not self.config.is_redis_primary():
            return OperationMode.ASYNC_ONLY
        elif not is_healthy and self.config.is_failover_enabled():
            return OperationMode.ASYNC_ONLY
        else:
            return self.config.operation_mode
    
    def on_health_status_change(self, previous_status, new_status, error_details, circuit_state):
        """Handle health status changes."""
        logging.info(f"Redis health changed: {previous_status} -> {new_status} (Circuit: {circuit_state})")
        
        # Determine if mode needs to change
        new_mode = self._get_mode_for_health(new_status, circuit_state)
        if new_mode != self.current_mode:
            self._change_mode(new_mode, {
                "reason": "health_change",
                "previous_health": previous_status.name,
                "new_health": new_status.name,
                "circuit_state": circuit_state.name,
                "error_details": error_details
            })
    
    def _get_mode_for_health(self, health_status, circuit_state):
        """Determine appropriate mode based on health status and circuit state."""
        if not self.config.is_redis_primary():
            return OperationMode.ASYNC_ONLY
        
        is_healthy = health_status == HealthStatus.HEALTHY and circuit_state == CircuitState.CLOSED
        
        if not is_healthy and self.config.is_failover_enabled():
            return OperationMode.ASYNC_ONLY
        else:
            return self.config.operation_mode
    
    def _change_mode(self, new_mode, details):
        """Change operation mode and notify interested parties."""
        old_mode = self.current_mode
        self.current_mode = new_mode
        
        logging.info(f"Redis operation mode changed: {old_mode} -> {new_mode} ({details.get('reason')})")
        
        # Notify event handler service
        if self.event_handler_service:
            self.event_handler_service.on_mode_change(old_mode, new_mode, details)
        
        # Notify registered callbacks
        for callback in self.mode_change_callbacks:
            try:
                callback(old_mode, new_mode, details)
            except Exception as e:
                logging.error(f"Error in mode change callback: {e}")
    
    def execute_with_resilience(self, operation_func, fallback_func=None):
        """Execute an operation with circuit breaker protection.
        
        Args:
            operation_func: Function that performs Redis operation
            fallback_func: Optional function to call if circuit is open
        
        Returns:
            Result of operation_func or fallback_func
        
        Raises:
            Exception if operation fails and no fallback is provided
        """
        # Check if circuit allows request
        if not self.health_monitor.circuit_breaker.allow_request():
            if fallback_func:
                return fallback_func()
            else:
                raise CircuitOpenError("Circuit breaker is open, Redis operations not allowed")
        
        try:
            # Execute operation
            result = operation_func()
            
            # Record success
            self.health_monitor.circuit_breaker.record_success()
            
            return result
        except Exception as e:
            # Record failure
            self.health_monitor.circuit_breaker.record_failure({
                "error": str(e),
                "error_type": type(e).__name__
            })
            
            # Use fallback if provided
            if fallback_func:
                return fallback_func()
            
            # Re-raise exception
            raise
    
    def get_status(self):
        """Get the current status of the resilience system."""
        return {
            "mode": self.current_mode.name,
            "health": self.status_reporter.get_detailed_status(),
            "recommendations": self.status_reporter.get_recommendations()
        }
    
    def register_mode_change_callback(self, callback):
        """Register a callback for mode changes.
        
        Args:
            callback: Function to call with signature (old_mode, new_mode, details)
        """
        self.mode_change_callbacks.append(callback)
    
    def reset_circuit_breaker(self):
        """Manually reset the circuit breaker."""
        self.health_monitor.circuit_breaker.reset()
        logging.info("Circuit breaker manually reset")
```

### `ResilienceCallbackHandler` Class

```python
class ResilienceCallbackHandler:
    """Handles callbacks from health monitoring components."""
    
    def __init__(self, resilience_manager):
        """Initialize the callback handler.
        
        Args:
            resilience_manager: RedisResilienceManager that owns this handler
        """
        self.resilience_manager = resilience_manager
    
    def on_health_status_change(self, previous_status, new_status, error_details, circuit_state):
        """Handle health status change notifications."""
        # Delegate to resilience manager
        self.resilience_manager.on_health_status_change(
            previous_status, new_status, error_details, circuit_state
        )
```

### `EventHandlerServiceIntegration`

The `EventHandlerService` will need to implement these methods for integration:

```python
class EventHandlerService:
    """Interface for integrating with RedisResilienceManager."""
    
    def on_resilience_manager_start(self, resilience_manager):
        """Called when resilience manager starts."""
        pass
    
    def on_resilience_manager_stop(self, resilience_manager):
        """Called when resilience manager stops."""
        pass
    
    def on_mode_change(self, old_mode, new_mode, details):
        """Called when operation mode changes.
        
        Args:
            old_mode: Previous operation mode
            new_mode: New operation mode
            details: Dictionary with change details
        """
        pass
```

## Usage Example

```python
# Initialize Redis client
redis_client = redis.Redis(host='localhost', port=6379)

# Create configuration
config = ResilientRedisConfig()

# Create event handler service (application specific)
event_handler_service = MyEventHandlerService()

# Create resilience manager
resilience_manager = RedisResilienceManager(
    redis_client=redis_client,
    config=config,
    event_handler_service=event_handler_service
)

# Start resilience monitoring
resilience_manager.start()

# Register for mode change notifications
resilience_manager.register_mode_change_callback(lambda old, new, details: print(f"Mode changed: {old} -> {new}"))

# Use resilient execution
def perform_redis_operation():
    return redis_client.xadd('my_stream', {'data': 'value'})

def fallback_operation():
    # Use in-memory queue instead
    return 'fallback_id'

result = resilience_manager.execute_with_resilience(
    operation_func=perform_redis_operation,
    fallback_func=fallback_operation
)

# Get current status
status = resilience_manager.get_status()
print(f"Current mode: {status['mode']}")
print(f"Health: {status['health']['status']}")

# Cleanup
resilience_manager.stop()
```
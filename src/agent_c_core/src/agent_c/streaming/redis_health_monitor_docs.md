# RedisHealthMonitor Documentation

## Overview

The `RedisHealthMonitor` class provides comprehensive health monitoring, circuit breaker patterns, auto-recovery, and event notification systems for Redis connectivity in the Agent C framework. It is designed to work seamlessly with the enhanced `RedisConfig` class and supports resilient Redis operation modes.

## Key Features

### 1. Core Health Monitoring
- **Periodic Health Checks**: Configurable interval-based health checking
- **Connectivity Testing**: Tests Redis PING, GET/SET operations, and stream operations
- **Latency Tracking**: Monitors operation latency with rolling window averaging
- **Error Rate Tracking**: Tracks failure rates with configurable thresholds

### 2. Circuit Breaker Implementation
- **State Machine**: Implements CLOSED → OPEN → HALF_OPEN → CLOSED transitions
- **Automatic Protection**: Prevents cascading failures during Redis outages
- **Configurable Thresholds**: Customizable failure rate and timeout settings
- **Exponential Backoff**: Implements jitter-based exponential backoff for recovery attempts

### 3. Auto-Recovery Logic
- **Background Recovery**: Automatic attempts to restore Redis connectivity
- **Intelligent Backoff**: Exponential backoff with jitter to prevent thundering herd
- **Recovery Tracking**: Detailed logging and metrics for recovery operations
- **Configurable Limits**: Maximum recovery attempts and timeout settings

### 4. Event System
- **Status Change Events**: Notifications for health status transitions
- **Callback Registration**: Support for multiple status change callbacks
- **Webhook Support**: Planned support for external monitoring system integration
- **Structured Logging**: Comprehensive logging with contextual information

## Architecture

### Health Status Levels

```python
class HealthStatus(Enum):
    HEALTHY = "healthy"        # All checks passing, low latency
    DEGRADED = "degraded"      # Some issues but functional
    UNHEALTHY = "unhealthy"    # Significant failures
    UNKNOWN = "unknown"        # Initial state, no checks performed
```

### Circuit Breaker States

```python
class CircuitBreakerState(Enum):
    CLOSED = "closed"          # Normal operation
    OPEN = "open"             # Blocking requests, failing fast
    HALF_OPEN = "half_open"   # Testing recovery with limited requests
```

### Health Metrics

The `HealthMetrics` class tracks comprehensive statistics:
- Total, successful, and failed health checks
- Consecutive failures and successes
- Latency samples with rolling window
- Failure rate calculations
- Uptime percentage tracking

## Configuration

### Basic Configuration

```python
from agent_c.streaming.redis_health_monitor import RedisHealthMonitor
from agent_c.streaming.redis_stream_manager import RedisConfig

# Create Redis configuration
redis_config = RedisConfig(
    redis_host='localhost',
    redis_port=6379,
    operation_mode=OperationMode.REDIS_ONLY,
    failover_strategy=FailoverStrategy.AUTO_FAILOVER
)

# Create health monitor
health_monitor = RedisHealthMonitor(
    redis_config=redis_config,
    health_check_interval=30.0,     # Check every 30 seconds
    unhealthy_threshold=0.6,        # 60% failure rate = unhealthy
    degraded_threshold=0.3,         # 30% failure rate = degraded
    latency_threshold_ms=1000.0     # 1 second latency = degraded
)
```

### Advanced Configuration

```python
# Custom circuit breaker settings
health_monitor = RedisHealthMonitor(
    redis_config=redis_config,
    health_check_interval=15.0,
    unhealthy_threshold=0.5,
    degraded_threshold=0.2,
    latency_threshold_ms=500.0
)

# Configure circuit breaker
health_monitor.circuit_breaker = CircuitBreaker(
    failure_threshold=0.5,          # Open circuit at 50% failure rate
    recovery_timeout=60,            # Wait 60 seconds before testing recovery
    half_open_max_calls=3,          # Allow 3 test calls in half-open state
    min_calls_threshold=5           # Need 5 calls before considering failure rate
)
```

## Usage Examples

### Basic Usage

```python
import asyncio
from agent_c.streaming.redis_health_monitor import RedisHealthMonitor

async def main():
    # Create and start health monitor
    health_monitor = RedisHealthMonitor()
    
    # Start background monitoring
    health_monitor.start_monitoring()
    
    # Check current status
    print(f"Redis status: {health_monitor.status.value}")
    print(f"Circuit breaker: {health_monitor.circuit_breaker.state.value}")
    
    # Perform manual health check
    status = await health_monitor.check_health()
    print(f"Health check result: {status.value}")
    
    # Get comprehensive health summary
    summary = health_monitor.get_health_summary()
    print(f"Health summary: {summary}")
    
    # Stop monitoring
    health_monitor.stop_monitoring()

# Run the example
asyncio.run(main())
```

### Using Context Manager

```python
async def context_manager_example():
    redis_config = RedisConfig()
    
    # Context manager automatically starts/stops monitoring
    with RedisHealthMonitor(redis_config) as health_monitor:
        # Monitor is automatically started
        
        # Wait for some health checks
        await asyncio.sleep(60)
        
        # Check status
        if health_monitor.is_healthy:
            print("Redis is healthy!")
        elif health_monitor.is_circuit_open:
            print("Circuit breaker is open - Redis issues detected")
        
        # Get detailed metrics
        summary = health_monitor.get_health_summary()
        print(f"Failure rate: {summary['metrics']['failure_rate']}")
        print(f"Average latency: {summary['metrics']['average_latency_ms']}ms")
    
    # Monitor is automatically stopped when exiting context

asyncio.run(context_manager_example())
```

### Status Change Callbacks

```python
def on_status_change(event):
    """Handle health status changes."""
    print(f"Status changed: {event.old_status.value} → {event.new_status.value}")
    print(f"Reason: {event.reason}")
    print(f"Circuit state: {event.circuit_state.value}")
    
    if event.new_status == HealthStatus.UNHEALTHY:
        # Alert operations team
        send_alert(f"Redis health degraded: {event.reason}")
    elif event.new_status == HealthStatus.HEALTHY and event.old_status == HealthStatus.UNHEALTHY:
        # Recovery notification
        send_notification("Redis has recovered")

async def callback_example():
    health_monitor = RedisHealthMonitor()
    
    # Register status change callback
    callback_id = health_monitor.register_status_change_callback(on_status_change)
    
    # Start monitoring
    health_monitor.start_monitoring()
    
    # Let it run for a while
    await asyncio.sleep(300)  # 5 minutes
    
    # Unregister callback
    health_monitor.unregister_status_change_callback(on_status_change)
    
    health_monitor.stop_monitoring()

asyncio.run(callback_example())
```

### Manual Circuit Breaker Control

```python
async def circuit_breaker_example():
    health_monitor = RedisHealthMonitor()
    
    # Check circuit breaker state
    if health_monitor.is_circuit_open:
        print("Circuit breaker is open")
        
        # Manually reset circuit breaker
        health_monitor.reset_circuit_breaker()
        print("Circuit breaker reset")
    
    # Force immediate health check
    if health_monitor.force_health_check():
        print("Synchronous health check passed")
    else:
        print("Synchronous health check failed")
    
    # Perform async health check
    status = await health_monitor.check_health()
    print(f"Async health check: {status.value}")

asyncio.run(circuit_breaker_example())
```

## Integration with RedisStreamManager

The `RedisHealthMonitor` is designed to integrate seamlessly with the existing `RedisStreamManager`:

```python
from agent_c.streaming.redis_stream_manager import RedisStreamManager
from agent_c.streaming.redis_health_monitor import RedisHealthMonitor

class EnhancedRedisStreamManager(RedisStreamManager):
    """Extended RedisStreamManager with health monitoring."""
    
    def __init__(self, redis_config=None):
        super().__init__(redis_config)
        
        # Add health monitor
        self.health_monitor = RedisHealthMonitor(
            redis_config=self.config,
            health_check_interval=30.0
        )
        
        # Register for status changes
        self.health_monitor.register_status_change_callback(
            self._on_health_status_change
        )
    
    def initialize(self):
        """Initialize with health monitoring."""
        super().initialize()
        self.health_monitor.start_monitoring()
    
    def close(self):
        """Close with health monitoring cleanup."""
        self.health_monitor.stop_monitoring()
        super().close()
    
    def _on_health_status_change(self, event):
        """Handle health status changes."""
        if event.new_status == HealthStatus.UNHEALTHY:
            # Activate fallback queue
            self.fallback_queue.activate()
            self.logger.warning(f"Redis unhealthy, activating fallback: {event.reason}")
        elif (event.new_status == HealthStatus.HEALTHY and 
              event.old_status == HealthStatus.UNHEALTHY):
            # Deactivate fallback queue
            self.fallback_queue.deactivate()
            self.logger.info("Redis recovered, deactivating fallback")
    
    async def publish_event(self, *args, **kwargs):
        """Publish with health-aware logic."""
        # Check if Redis is healthy before attempting publish
        if self.health_monitor.is_healthy and not self.health_monitor.is_circuit_open:
            try:
                return await super().publish_event(*args, **kwargs)
            except Exception as e:
                # Let health monitor handle the failure
                await self.health_monitor.check_health()
                raise
        else:
            # Use fallback directly
            return self._publish_to_fallback(*args, **kwargs)
```

## Monitoring and Observability

### Health Summary Format

The `get_health_summary()` method returns a comprehensive status object:

```json
{
  "status": "healthy",
  "circuit_state": "closed",
  "metrics": {
    "total_checks": 120,
    "successful_checks": 118,
    "failed_checks": 2,
    "failure_rate": 0.017,
    "average_latency_ms": 45.2,
    "current_latency_ms": 38.1,
    "consecutive_failures": 0,
    "consecutive_successes": 15,
    "last_check_time": "2024-01-15T10:30:45.123456",
    "last_success_time": "2024-01-15T10:30:45.123456",
    "last_failure_time": "2024-01-15T10:25:12.987654"
  },
  "recovery": {
    "attempts": 0,
    "last_attempt": null,
    "max_attempts": 10
  },
  "configuration": {
    "health_check_interval": 30.0,
    "unhealthy_threshold": 0.6,
    "degraded_threshold": 0.3,
    "latency_threshold_ms": 1000.0
  }
}
```

### Logging

The health monitor provides structured logging at various levels:

- **DEBUG**: Individual health check results, latency measurements
- **INFO**: Status changes, recovery attempts, circuit breaker transitions
- **WARNING**: Health degradation, recovery failures
- **ERROR**: Critical failures, configuration issues

Example log output:
```
2024-01-15 10:30:45 INFO RedisHealthMonitor: Health status changed: unknown -> healthy (circuit: closed, failure_rate: 0.00)
2024-01-15 10:35:23 WARNING RedisHealthMonitor: Health check failed: Connection refused
2024-01-15 10:35:23 WARNING RedisHealthMonitor: Circuit breaker transitioning to OPEN state
2024-01-15 10:36:23 INFO RedisHealthMonitor: Circuit breaker transitioning to HALF_OPEN state
2024-01-15 10:36:28 INFO RedisHealthMonitor: Attempting recovery #1 in 1.23 seconds
```

## Performance Considerations

### Resource Usage

- **Memory**: Minimal overhead, approximately 1-2MB for metrics storage
- **CPU**: Low impact, configurable check intervals (recommended: 15-60 seconds)
- **Network**: Lightweight health checks (PING + basic operations)
- **Threads**: Single background thread for monitoring loop

### Configuration Recommendations

| Environment | Check Interval | Thresholds | Recovery Timeout |
|-------------|----------------|------------|------------------|
| Development | 10-15 seconds | Degraded: 0.2, Unhealthy: 0.4 | 30 seconds |
| Testing | 20-30 seconds | Degraded: 0.3, Unhealthy: 0.5 | 60 seconds |
| Production | 30-60 seconds | Degraded: 0.3, Unhealthy: 0.6 | 60-120 seconds |

### Optimization Tips

1. **Adjust check intervals** based on Redis stability and requirements
2. **Tune thresholds** based on expected Redis performance characteristics
3. **Monitor health monitor performance** to ensure it doesn't impact Redis
4. **Use callbacks** efficiently to avoid blocking the monitoring loop
5. **Consider regional differences** in latency thresholds

## Error Handling

The health monitor provides comprehensive error handling:

### Connection Errors
- `ConnectionError`: Redis server unavailable
- `TimeoutError`: Operations taking too long
- `RedisError`: General Redis operational errors

### Recovery Strategies
- **Exponential Backoff**: Prevents overwhelming failed Redis instances
- **Circuit Breaker**: Provides fast failure during outages
- **Graceful Degradation**: Maintains service availability during Redis issues

### Error Categories
- **Transient**: Network blips, temporary overload
- **Persistent**: Configuration issues, Redis server down
- **Performance**: High latency, resource exhaustion

## Best Practices

### 1. Configuration
- Set health check intervals appropriate for your environment
- Configure thresholds based on your Redis performance baseline
- Use shorter intervals in development, longer in production

### 2. Integration
- Register status change callbacks for operational alerts
- Integrate with existing monitoring and alerting systems
- Use health status in application logic for graceful degradation

### 3. Monitoring
- Monitor the health monitor's own performance
- Track recovery success rates and timing
- Alert on prolonged unhealthy states

### 4. Operations
- Implement proper alerting for status changes
- Use manual circuit breaker resets judiciously
- Monitor Redis performance alongside health metrics

## Troubleshooting

### Common Issues

**High False Positive Rate**
- Lower failure rate thresholds
- Increase latency thresholds
- Check network stability between application and Redis

**Slow Recovery**
- Reduce recovery timeout
- Check Redis server recovery time
- Verify network path reliability

**Circuit Breaker Stuck Open**
- Manually reset circuit breaker
- Check Redis server status
- Verify configuration parameters

**Missing Status Change Events**
- Check callback registration
- Verify callback exception handling
- Review monitoring loop errors

### Debug Mode

Enable debug logging for detailed health check information:

```python
import logging

# Enable debug logging for health monitor
logging.getLogger('agent_c.streaming.redis_health_monitor').setLevel(logging.DEBUG)

# Create health monitor with debug-friendly settings
health_monitor = RedisHealthMonitor(
    health_check_interval=5.0,  # Frequent checks for debugging
    unhealthy_threshold=0.1,    # Lower threshold for easier testing
    degraded_threshold=0.05
)
```

## Future Enhancements

### Planned Features
- **Webhook Notifications**: HTTP POST notifications to external systems
- **Metrics Export**: Prometheus/StatsD metrics integration
- **Historical Trending**: Long-term health trend analysis
- **Predictive Analysis**: Machine learning for failure prediction

### Extensibility Points
- **Custom Health Checks**: Plugin system for application-specific checks
- **Notification Channels**: Slack, email, PagerDuty integration
- **Advanced Recovery**: Intelligent recovery strategies based on failure patterns
- **Multi-Redis Support**: Health monitoring for Redis clusters
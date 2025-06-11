# Redis Streams Configuration Model Design

## Overview

This document defines the configuration model for Redis Streams resilient mode, providing a comprehensive approach to configuring Redis Streams operation with focus on reliability, performance, and operational flexibility.

## Configuration Class Design

### `ResilientRedisConfig` Class

```python
class ResilientRedisConfig:
    """Master configuration class for resilient Redis Streams operation."""
    
    def __init__(self, 
                 operation_mode=None,
                 failover_strategy=None,
                 health_check_config=None,
                 performance_config=None,
                 **kwargs):
        # Initialize from environment variables, then override with params
        self.operation_mode = self._get_operation_mode(operation_mode)
        self.failover_strategy = self._get_failover_strategy(failover_strategy)
        self.health_check = HealthCheckConfig(**(health_check_config or {}))
        self.performance = PerformanceConfig(**(performance_config or {}))
        
        # Additional configuration options
        self.debug_mode = kwargs.get('debug_mode', self._get_debug_mode())
        
        # Validate configuration
        self.validate()
    
    def _get_operation_mode(self, mode=None):
        """Get operation mode from param or environment variable."""
        if mode is not None:
            return OperationMode(mode)
        env_mode = os.environ.get('REDIS_OPERATION_MODE', 'hybrid').lower()
        return OperationMode(env_mode)
    
    def _get_failover_strategy(self, strategy=None):
        """Get failover strategy from param or environment variable."""
        if strategy is not None:
            return FailoverStrategy(strategy)
        env_strategy = os.environ.get('REDIS_FAILOVER_MODE', 'auto_failover').lower()
        return FailoverStrategy(env_strategy)
    
    def _get_debug_mode(self):
        """Get debug mode from environment variable."""
        return os.environ.get('REDIS_DEBUG_MODE', 'false').lower() in ('true', '1', 'yes')
    
    def is_redis_primary(self):
        """Returns True if Redis is the primary method for operation."""
        return self.operation_mode in (OperationMode.REDIS_ONLY, OperationMode.HYBRID)
    
    def is_failover_enabled(self):
        """Returns True if automatic failover is enabled."""
        return self.failover_strategy == FailoverStrategy.AUTO_FAILOVER
    
    def get_effective_mode(self, redis_healthy=True):
        """Returns the effective operation mode based on health status."""
        if not redis_healthy and self.is_failover_enabled():
            return OperationMode.ASYNC_ONLY
        return self.operation_mode
    
    def validate(self):
        """Validates the configuration for consistency."""
        # Validate operation mode and failover strategy compatibility
        if self.operation_mode == OperationMode.ASYNC_ONLY and \
           self.failover_strategy != FailoverStrategy.NO_FAILOVER:
            logging.warning("Failover strategy has no effect in ASYNC_ONLY mode. Setting to NO_FAILOVER.")
            self.failover_strategy = FailoverStrategy.NO_FAILOVER
        
        # Validate health check configuration
        self.health_check.validate()
        
        # Validate performance configuration
        self.performance.validate()
        
        # Log configuration summary
        if self.debug_mode:
            self._log_configuration()
    
    def _log_configuration(self):
        """Log the current configuration settings."""
        logging.info(f"Redis Streams Configuration:\n"
                     f"  Operation Mode: {self.operation_mode}\n"
                     f"  Failover Strategy: {self.failover_strategy}\n"
                     f"  Health Check Interval: {self.health_check.interval}s\n"
                     f"  Batch Size: {self.performance.batch_size}")
```

### `OperationMode` Enum

```python
class OperationMode(Enum):
    """Enumeration of Redis Streams operation modes."""
    REDIS_ONLY = 'redis_only'   # Use only Redis Streams for event handling
    HYBRID = 'hybrid'           # Use Redis for publishing, async queue for consumption
    ASYNC_ONLY = 'async_only'   # Use only async queue (legacy mode)
```

### `FailoverStrategy` Enum

```python
class FailoverStrategy(Enum):
    """Enumeration of failover strategies."""
    AUTO_FAILOVER = 'auto_failover'   # Automatically fall back to async queue if Redis is unavailable
    NO_FAILOVER = 'no_failover'       # Fail operations if Redis is unavailable
    MANUAL_FAILOVER = 'manual_failover'  # Only fail over when explicitly requested
```

### `HealthCheckConfig` Class

```python
class HealthCheckConfig:
    """Configuration for Redis health checking."""
    
    def __init__(self, **kwargs):
        # Health check timing
        self.interval = int(kwargs.get('interval', 
                                       os.environ.get('REDIS_HEALTH_CHECK_INTERVAL', '30')))
        self.timeout = float(kwargs.get('timeout', 
                                       os.environ.get('REDIS_CONNECTION_TIMEOUT', '5.0')))
        
        # Failure detection
        self.consecutive_failures_threshold = int(kwargs.get(
            'consecutive_failures_threshold',
            os.environ.get('REDIS_HEALTH_CONSECUTIVE_FAILURES_THRESHOLD', '3')))
        self.error_rate_threshold = float(kwargs.get(
            'error_rate_threshold',
            os.environ.get('REDIS_HEALTH_ERROR_RATE_THRESHOLD', '0.25')))
        
        # Recovery settings
        self.recovery_interval = int(kwargs.get(
            'recovery_interval',
            os.environ.get('REDIS_RECOVERY_INTERVAL', '60')))
        self.recovery_attempts = int(kwargs.get(
            'recovery_attempts',
            os.environ.get('REDIS_MAX_RECOVERY_ATTEMPTS', '0')))  # 0 = unlimited
        
        # Circuit breaker settings
        self.circuit_breaker_threshold = float(kwargs.get(
            'circuit_breaker_threshold',
            os.environ.get('REDIS_CIRCUIT_BREAKER_THRESHOLD', '0.5')))
        self.circuit_reset_timeout = int(kwargs.get(
            'circuit_reset_timeout',
            os.environ.get('REDIS_CIRCUIT_RESET_TIMEOUT', '300')))
    
    def validate(self):
        """Validate health check configuration."""
        # Validate numeric ranges
        if self.interval <= 0:
            raise ValueError("Health check interval must be greater than 0")
        if self.timeout <= 0:
            raise ValueError("Connection timeout must be greater than 0")
        if self.consecutive_failures_threshold <= 0:
            raise ValueError("Consecutive failures threshold must be greater than 0")
        if not 0 <= self.error_rate_threshold <= 1:
            raise ValueError("Error rate threshold must be between 0 and 1")
        if self.recovery_interval <= 0:
            raise ValueError("Recovery interval must be greater than 0")
        if self.recovery_attempts < 0:
            raise ValueError("Recovery attempts must be greater than or equal to 0")
        if not 0 < self.circuit_breaker_threshold <= 1:
            raise ValueError("Circuit breaker threshold must be between 0 and 1")
        if self.circuit_reset_timeout <= 0:
            raise ValueError("Circuit reset timeout must be greater than 0")
```

### `PerformanceConfig` Class

```python
class PerformanceConfig:
    """Configuration for Redis Streams performance settings."""
    
    def __init__(self, **kwargs):
        # Batching configuration
        self.batch_size = int(kwargs.get(
            'batch_size',
            os.environ.get('REDIS_BATCH_SIZE', '50')))
        self.batch_timeout = float(kwargs.get(
            'batch_timeout',
            os.environ.get('REDIS_BATCH_TIMEOUT', '0.1')))
        
        # Buffer configuration
        self.buffer_size = int(kwargs.get(
            'buffer_size',
            os.environ.get('REDIS_EVENT_BUFFER_SIZE', '10000')))
        self.high_watermark = float(kwargs.get(
            'high_watermark',
            os.environ.get('REDIS_BUFFER_HIGH_WATERMARK', '0.9')))
        self.low_watermark = float(kwargs.get(
            'low_watermark',
            os.environ.get('REDIS_BUFFER_LOW_WATERMARK', '0.7')))
        
        # Stream trimming configuration
        self.stream_max_len = int(kwargs.get(
            'stream_max_len',
            os.environ.get('REDIS_STREAM_MAX_LENGTH', '1000')))
        self.stream_trim_interval = int(kwargs.get(
            'stream_trim_interval',
            os.environ.get('REDIS_STREAM_TRIM_INTERVAL', '100')))
        
        # Connection pooling
        self.max_connections = int(kwargs.get(
            'max_connections',
            os.environ.get('REDIS_MAX_CONNECTIONS', '10')))
        
    def validate(self):
        """Validate performance configuration."""
        # Validate numeric ranges
        if self.batch_size <= 0:
            raise ValueError("Batch size must be greater than 0")
        if self.batch_timeout <= 0:
            raise ValueError("Batch timeout must be greater than 0")
        if self.buffer_size <= 0:
            raise ValueError("Buffer size must be greater than 0")
        if not 0 < self.low_watermark < self.high_watermark < 1:
            raise ValueError("Buffer watermarks must be in order: 0 < low < high < 1")
        if self.stream_max_len <= 0:
            raise ValueError("Stream max length must be greater than 0")
        if self.stream_trim_interval <= 0:
            raise ValueError("Stream trim interval must be greater than 0")
        if self.max_connections <= 0:
            raise ValueError("Max connections must be greater than 0")
```

## Environment Variable Mappings

| Environment Variable | Type | Default | Description |
|----------------------|------|---------|-------------|
| **Operation Mode** |
| REDIS_OPERATION_MODE | string | "hybrid" | Primary operation mode (redis_only, hybrid, async_only) |
| REDIS_FAILOVER_MODE | string | "auto_failover" | Failover strategy (auto_failover, no_failover, manual_failover) |
| REDIS_DEBUG_MODE | boolean | "false" | Enable debug mode with verbose logging |
| **Health Check Configuration** |
| REDIS_HEALTH_CHECK_INTERVAL | integer | 30 | Time between health checks (seconds) |
| REDIS_CONNECTION_TIMEOUT | float | 5.0 | Connection timeout (seconds) |
| REDIS_HEALTH_CONSECUTIVE_FAILURES_THRESHOLD | integer | 3 | Number of consecutive failures before triggering failover |
| REDIS_HEALTH_ERROR_RATE_THRESHOLD | float | 0.25 | Error rate that triggers circuit breaker (0-1) |
| REDIS_RECOVERY_INTERVAL | integer | 60 | Time between recovery attempts (seconds) |
| REDIS_MAX_RECOVERY_ATTEMPTS | integer | 0 | Maximum recovery attempts (0 = unlimited) |
| REDIS_CIRCUIT_BREAKER_THRESHOLD | float | 0.5 | Error threshold for circuit breaker (0-1) |
| REDIS_CIRCUIT_RESET_TIMEOUT | integer | 300 | Time before circuit breaker resets (seconds) |
| **Performance Configuration** |
| REDIS_BATCH_SIZE | integer | 50 | Number of events per batch |
| REDIS_BATCH_TIMEOUT | float | 0.1 | Maximum time to wait for batch completion (seconds) |
| REDIS_EVENT_BUFFER_SIZE | integer | 10000 | Maximum buffer size for event queuing |
| REDIS_BUFFER_HIGH_WATERMARK | float | 0.9 | Buffer capacity threshold for backpressure (0-1) |
| REDIS_BUFFER_LOW_WATERMARK | float | 0.7 | Buffer capacity threshold for resuming normal operation (0-1) |
| REDIS_STREAM_MAX_LENGTH | integer | 1000 | Maximum length for Redis Streams |
| REDIS_STREAM_TRIM_INTERVAL | integer | 100 | Number of events between stream trimming operations |
| REDIS_MAX_CONNECTIONS | integer | 10 | Maximum number of Redis connections in the pool |

## Default Values Documentation

### Development Environment Defaults

```
REDIS_OPERATION_MODE=hybrid
REDIS_FAILOVER_MODE=auto_failover
REDIS_DEBUG_MODE=true
REDIS_HEALTH_CHECK_INTERVAL=60
REDIS_BATCH_SIZE=10
REDIS_EVENT_BUFFER_SIZE=1000
REDIS_STREAM_MAX_LENGTH=500
```

Rationale: Optimized for developer experience with verbose logging, less frequent health checks, and smaller batch sizes to make debugging easier. The hybrid mode ensures reliability during development.

### Staging Environment Defaults

```
REDIS_OPERATION_MODE=hybrid
REDIS_FAILOVER_MODE=auto_failover
REDIS_DEBUG_MODE=false
REDIS_HEALTH_CHECK_INTERVAL=30
REDIS_BATCH_SIZE=25
REDIS_EVENT_BUFFER_SIZE=5000
REDIS_STREAM_MAX_LENGTH=1000
```

Rationale: Balanced for testing with realistic parameters. Hybrid mode with auto failover ensures stability during testing while simulating production behavior.

### Production Environment Defaults

```
REDIS_OPERATION_MODE=redis_only
REDIS_FAILOVER_MODE=auto_failover
REDIS_DEBUG_MODE=false
REDIS_HEALTH_CHECK_INTERVAL=15
REDIS_BATCH_SIZE=100
REDIS_EVENT_BUFFER_SIZE=10000
REDIS_STREAM_MAX_LENGTH=2000
```

Rationale: Optimized for performance and reliability in production. Redis-only mode for maximum efficiency with auto failover for reliability. More frequent health checks, larger batch sizes, and buffer capacity for handling production loads.

## Configuration Validation Rules

### Compatibility Validation

1. **Operation Mode and Failover Strategy Compatibility**
   - If `operation_mode` is `ASYNC_ONLY`, then `failover_strategy` must be set to `NO_FAILOVER`
   - Automatic correction: Set `failover_strategy` to `NO_FAILOVER` with warning log

### Range Validation

1. **Numeric Range Checks**
   - All interval settings must be greater than 0
   - All threshold settings must be between 0 and 1
   - Watermark settings must maintain proper ordering: 0 < low_watermark < high_watermark < 1

2. **Resource Limit Validation**
   - `max_connections` should not exceed system limits
   - `buffer_size` should be appropriate for available memory

### Logic Validation

1. **Timing Relationship Checks**
   - `health_check.interval` should be less than `health_check.recovery_interval`
   - `batch_timeout` should be appropriate for `batch_size`

2. **Performance Settings Coherence**
   - `stream_trim_interval` should be less than `stream_max_len`
   - `batch_size` should be appropriate for throughput requirements

### Environment-Specific Recommendations

1. **Production Validation**
   - Warning if `debug_mode` is enabled in production
   - Warning if `health_check.interval` is too high (> 30 seconds)
   - Warning if `stream_max_len` is too small for production traffic

2. **Development Validation**
   - Recommendation for `debug_mode` to be enabled
   - Recommendation for smaller batch sizes and buffer sizes

## Example Usage

```python
# Using environment variables
os.environ['REDIS_OPERATION_MODE'] = 'redis_only'
os.environ['REDIS_HEALTH_CHECK_INTERVAL'] = '15'
config = ResilientRedisConfig()

# Explicit configuration
config = ResilientRedisConfig(
    operation_mode=OperationMode.REDIS_ONLY,
    failover_strategy=FailoverStrategy.AUTO_FAILOVER,
    health_check_config={
        'interval': 15,
        'consecutive_failures_threshold': 5,
        'circuit_breaker_threshold': 0.4
    },
    performance_config={
        'batch_size': 100,
        'buffer_size': 20000,
        'stream_max_len': 5000
    },
    debug_mode=False
)

# Using the configuration
if config.is_redis_primary():
    # Use Redis Streams
    pass

# Get effective mode based on health
effective_mode = config.get_effective_mode(redis_healthy=redis_is_healthy())
if effective_mode == OperationMode.ASYNC_ONLY:
    # Use async queue
    pass
```
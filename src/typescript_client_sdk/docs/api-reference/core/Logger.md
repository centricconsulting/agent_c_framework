# Logger API Reference

The Logger utility provides a structured logging system with environment-aware log levels, ensuring proper logging practices across development, testing, and production environments.

## Overview

The Logger is a static utility class that provides centralized logging functionality with:
- **Environment-aware defaults** - Different log levels for test, development, and production
- **Structured output** - Consistent log formatting with level prefixes
- **Level control** - Runtime configurable log levels
- **Zero-configuration** - Works immediately with sensible defaults

## Configuration

### Log Levels

```typescript
enum LogLevel {
  ERROR = 0,  // Critical errors only
  WARN = 1,   // Warnings and errors
  INFO = 2,   // Informational messages
  DEBUG = 3,  // Debug information
  TRACE = 4   // Detailed trace logs
}
```

### Default Log Levels by Environment

| Environment | Default Level | Description |
|------------|---------------|-------------|
| `test` | `ERROR` | Only critical errors in tests |
| `development` | `DEBUG` | Detailed debugging information |
| `production` | `WARN` | Warnings and errors only |

## API Methods

### `Logger.setLevel(level: LogLevel): void`

Configure the global log level at runtime.

```typescript
import { Logger, LogLevel } from '@agentc/realtime-core';

// Set to debug level
Logger.setLevel(LogLevel.DEBUG);

// Set to error only
Logger.setLevel(LogLevel.ERROR);
```

### `Logger.error(message: string, ...args: any[]): void`

Log error messages (always visible unless completely disabled).

```typescript
Logger.error('Connection failed', error);
Logger.error('Invalid configuration:', config);
```

### `Logger.warn(message: string, ...args: any[]): void`

Log warning messages for potential issues.

```typescript
Logger.warn('Deprecated API usage detected');
Logger.warn('Rate limit approaching:', remaining);
```

### `Logger.info(message: string, ...args: any[]): void`

Log informational messages about normal operations.

```typescript
Logger.info('Server connected successfully');
Logger.info('Processing batch:', batchId, items.length);
```

### `Logger.debug(message: string, ...args: any[]): void`

Log debug information for development troubleshooting.

```typescript
Logger.debug('State transition:', oldState, '->', newState);
Logger.debug('API response:', response);
```

### `Logger.trace(message: string, ...args: any[]): void`

Log detailed trace information (includes stack trace).

```typescript
Logger.trace('Entering function with params:', params);
Logger.trace('Detailed state dump:', state);
```

## Usage Examples

### Basic Logging

```typescript
import { Logger } from '@agentc/realtime-core';

// Simple messages
Logger.info('Application started');
Logger.error('Failed to load configuration');

// With additional context
Logger.debug('User authenticated', { userId, role });
Logger.warn('API rate limit', { remaining: 5, resetAt });
```

### Component Logging Pattern

```typescript
import { Logger } from '@agentc/realtime-core';

export class AudioProcessor {
  constructor() {
    Logger.debug('[AudioProcessor] Initializing');
  }
  
  async initialize(): Promise<void> {
    try {
      Logger.info('[AudioProcessor] Starting initialization');
      // ... initialization code
      Logger.debug('[AudioProcessor] Configuration:', this.config);
    } catch (error) {
      Logger.error('[AudioProcessor] Initialization failed:', error);
      throw error;
    }
  }
  
  private handleError(error: Error): void {
    Logger.error('[AudioProcessor] Processing error:', error.message);
    Logger.debug('[AudioProcessor] Error stack:', error.stack);
  }
}
```

### Conditional Logging

```typescript
import { Logger, LogLevel } from '@agentc/realtime-core';

// Verbose logging for debugging
function debugConnection(ws: WebSocket) {
  if (process.env.DEBUG_WS === 'true') {
    Logger.setLevel(LogLevel.TRACE);
    
    ws.on('message', (data) => {
      Logger.trace('[WebSocket] Message received:', data);
    });
    
    ws.on('error', (error) => {
      Logger.error('[WebSocket] Error:', error);
    });
  }
}
```

### Structured Context Pattern

```typescript
import { Logger } from '@agentc/realtime-core';

class SessionManager {
  private logContext = '[SessionManager]';
  
  async createSession(params: SessionParams): Promise<Session> {
    const requestId = generateId();
    const log = (level: string, msg: string, ...args: any[]) => {
      const message = `${this.logContext}[${requestId}] ${msg}`;
      Logger[level](message, ...args);
    };
    
    log('info', 'Creating session', { userId: params.userId });
    
    try {
      const session = await this.api.create(params);
      log('debug', 'Session created', session);
      return session;
    } catch (error) {
      log('error', 'Session creation failed', error);
      throw error;
    }
  }
}
```

## Best Practices

### 1. Use Consistent Prefixes

```typescript
// Good - consistent component identification
Logger.info('[AudioManager] Initialized');
Logger.error('[AudioManager] Failed to load worklet');

// Bad - inconsistent formatting
Logger.info('audio ready');
Logger.error('ERROR: worklet problem');
```

### 2. Log at Appropriate Levels

```typescript
// Good - appropriate level selection
Logger.error('[API] Authentication failed', { statusCode: 401 });
Logger.warn('[API] Rate limit warning', { remaining: 10 });
Logger.info('[API] Request completed', { duration: 250 });
Logger.debug('[API] Request details', { headers, body });

// Bad - wrong levels
Logger.error('[API] User logged in');  // Should be info
Logger.debug('[API] Critical failure'); // Should be error
```

### 3. Include Relevant Context

```typescript
// Good - includes actionable context
Logger.error('[WebSocket] Connection failed', {
  url: wsUrl,
  attemptNumber,
  lastError: error.message,
  willRetry: attemptNumber < maxRetries
});

// Bad - missing context
Logger.error('Connection failed');
```

### 4. Avoid Logging Sensitive Data

```typescript
// Good - sanitized data
Logger.info('[Auth] User authenticated', {
  userId: user.id,
  role: user.role
});

// Bad - exposes sensitive information
Logger.info('[Auth] Login successful', {
  password: credentials.password,  // Never log passwords
  apiKey: user.apiKey              // Never log secrets
});
```

### 5. Use Logger Exclusively

```typescript
// Good - using Logger
import { Logger } from '@agentc/realtime-core';
Logger.debug('[Component] Debug information');

// Bad - using console directly
console.log('Debug information');  // Violates standards
```

## Testing Guidelines

### Configure Test Logging

```typescript
// In test setup
import { Logger, LogLevel } from '@agentc/realtime-core';

beforeAll(() => {
  // Silence logs in tests unless debugging
  Logger.setLevel(LogLevel.ERROR);
});

// For debugging specific tests
it('should handle connection errors', () => {
  Logger.setLevel(LogLevel.DEBUG); // Temporarily enable debug
  // ... test code
});
```

### Mock Logger in Tests

```typescript
import { vi } from 'vitest';
import { Logger } from '@agentc/realtime-core';

// Spy on logger methods
const errorSpy = vi.spyOn(Logger, 'error');

// Test error logging
expect(errorSpy).toHaveBeenCalledWith(
  '[Component] Error occurred',
  expect.any(Error)
);
```

## Integration Patterns

### With Error Boundaries

```typescript
class ErrorBoundary extends React.Component {
  componentDidCatch(error: Error, errorInfo: ErrorInfo) {
    Logger.error('[ErrorBoundary] React error caught', {
      error: error.message,
      componentStack: errorInfo.componentStack
    });
  }
}
```

### With API Middleware

```typescript
const apiMiddleware = (request: Request, next: () => Promise<Response>) => {
  const startTime = Date.now();
  
  Logger.debug('[API] Request', {
    method: request.method,
    url: request.url
  });
  
  return next().then(
    response => {
      Logger.info('[API] Response', {
        status: response.status,
        duration: Date.now() - startTime
      });
      return response;
    },
    error => {
      Logger.error('[API] Request failed', {
        error: error.message,
        duration: Date.now() - startTime
      });
      throw error;
    }
  );
};
```

### With WebSocket Events

```typescript
class WebSocketManager {
  private setupLogging(ws: WebSocket): void {
    ws.on('open', () => {
      Logger.info('[WebSocket] Connected');
    });
    
    ws.on('message', (data) => {
      Logger.debug('[WebSocket] Message received', {
        size: data.length,
        type: typeof data
      });
    });
    
    ws.on('error', (error) => {
      Logger.error('[WebSocket] Error', error);
    });
    
    ws.on('close', (code, reason) => {
      Logger.info('[WebSocket] Closed', { code, reason });
    });
  }
}
```

## Performance Considerations

- Log level checks happen before message formatting
- Higher log levels (TRACE, DEBUG) may impact performance
- Consider using ERROR level in production for minimal overhead
- Avoid logging in tight loops or high-frequency operations

## Environment Variables

The Logger automatically detects the environment:

```bash
# Development
NODE_ENV=development  # Sets DEBUG level

# Testing  
NODE_ENV=test        # Sets ERROR level

# Production
NODE_ENV=production  # Sets WARN level
```

## Migration Guide

If migrating from `console.*` methods:

```typescript
// Before
console.log('Debug info');
console.error('Error occurred');
console.warn('Warning');

// After
import { Logger } from '@agentc/realtime-core';

Logger.debug('Debug info');
Logger.error('Error occurred');
Logger.warn('Warning');
```

## Related Documentation

- [Testing Standards](../testing_standards_and_architecture.md) - Testing practices for logging
- [RealtimeClient](./RealtimeClient.md) - Client logging integration
- [WebSocketManager](./WebSocketManager.md) - WebSocket event logging
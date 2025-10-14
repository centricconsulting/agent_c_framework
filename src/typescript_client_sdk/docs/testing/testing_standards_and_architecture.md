# Agent C Realtime SDK Testing Standards & Architecture

## Executive Summary

This document establishes MANDATORY testing standards for the Agent C Realtime SDK. All packages MUST follow these standards without exception.

## 1. Testing Architecture

### 1.1 Test Organization Structure

```
packages/
  [package-name]/
    src/
      [feature]/
        index.ts
        [component].ts
        __tests__/
          [component].test.ts
          [component].integration.test.ts
          fixtures/
            [test-data].json
    vitest.config.ts
    package.json
```

**MANDATORY RULES:**

- Unit tests MUST be co-located with source code in `__tests__` directories
- Integration tests use `.integration.test.ts` suffix
- Test fixtures go in `__tests__/fixtures/`
- Each package MUST have its own `vitest.config.ts`
- NO alternative test locations are permitted

### 1.2 Test File Naming Convention

- Unit tests: `[component-name].test.ts(x)`
- Integration tests: `[feature].integration.test.ts(x)`
- Test utilities: `test-utils.ts`
- Mock files: `[component].mock.ts`

## 2. Testing Stack

### 2.1 Required Packages

```json
{
  "devDependencies": {
    // Core Testing
    "vitest": "^1.0.0",
    "@vitest/ui": "^1.0.0",
    "@vitest/coverage-v8": "^1.0.0",

    // React Testing (for react/ui/demo packages)
    "@testing-library/react": "^14.0.0",
    "@testing-library/user-event": "^14.0.0",
    "@testing-library/jest-dom": "^6.0.0",

    // DOM Environment
    "happy-dom": "^12.0.0",

    // Mocking
    "msw": "^2.0.0",

    // Utilities
    "@faker-js/faker": "^8.0.0",
    "superstruct": "^1.0.0"
  }
}
```

### 2.2 Vitest Configuration Template

```typescript
// packages/[package-name]/vitest.config.ts
import { defineConfig } from 'vitest/config';
import { resolve } from 'path';

export default defineConfig({
  test: {
    globals: true,
    environment: 'happy-dom', // or 'node' for core package
    setupFiles: ['./src/test/setup.ts'],
    coverage: {
      provider: 'v8',
      reporter: ['text', 'json', 'html', 'lcov'],
      reportsDirectory: '../../.scratch/coverage/demo',
      exclude: [
        'node_modules/**',
        'dist/**',
        '**/*.config.*',
        '**/*.d.ts',
        '**/__tests__/**',
        '**/__mocks__/**',
        '**/test/setup.ts'
      ],
      thresholds: {
        branches: 80,
        functions: 80,
        lines: 80,
        statements: 80
      }
    },
    testTimeout: 10000,
    hookTimeout: 10000
  },
  resolve: {
    alias: {
      '@': resolve(__dirname, './src'),
      '@test': resolve(__dirname, './src/test')
    }
  }
});
```

## 3. Console Logging Standards

### 3.1 Development Logging

```typescript
// src/utils/logger.ts
export enum LogLevel {
  ERROR = 0,
  WARN = 1,
  INFO = 2,
  DEBUG = 3,
  TRACE = 4
}

export class Logger {
  private static level: LogLevel = 
    process.env.NODE_ENV === 'test' ? LogLevel.ERROR :
    process.env.NODE_ENV === 'development' ? LogLevel.DEBUG :
    LogLevel.WARN;

  static setLevel(level: LogLevel) {
    this.level = level;
  }

  static error(message: string, ...args: any[]) {
    if (this.level >= LogLevel.ERROR) {
      console.error(`[ERROR] ${message}`, ...args);
    }
  }

  static warn(message: string, ...args: any[]) {
    if (this.level >= LogLevel.WARN) {
      console.warn(`[WARN] ${message}`, ...args);
    }
  }

  static info(message: string, ...args: any[]) {
    if (this.level >= LogLevel.INFO) {
      console.info(`[INFO] ${message}`, ...args);
    }
  }

  static debug(message: string, ...args: any[]) {
    if (this.level >= LogLevel.DEBUG) {
      console.debug(`[DEBUG] ${message}`, ...args);
    }
  }

  static trace(message: string, ...args: any[]) {
    if (this.level >= LogLevel.TRACE) {
      console.trace(`[TRACE] ${message}`, ...args);
    }
  }
}
```

### 3.2 Diagnostic Logging Rules

- **Production**: ERROR and WARN only
- **Development**: DEBUG level by default
- **Testing**: ERROR only (unless debugging specific test)
- **NO console.log()**: Use Logger class exclusively
- **Structured logging**: Include context in log messages
- **Performance logging**: Use `console.time()` and `console.timeEnd()` for performance measurements

## 4. Test Writing Standards

### 4.1 Test Structure Template

```typescript
import { describe, it, expect, beforeEach, afterEach, vi } from 'vitest';
import { render, screen, waitFor } from '@testing-library/react';
import userEvent from '@testing-library/user-event';

describe('ComponentName', () => {
  // Setup and teardown
  beforeEach(() => {
    vi.clearAllMocks();
  });

  afterEach(() => {
    vi.restoreAllMocks();
  });

  // Group related tests
  describe('initialization', () => {
    it('should initialize with default props', () => {
      // Arrange
      const defaultProps = { /* ... */ };

      // Act
      render(<Component {...defaultProps} />);

      // Assert
      expect(screen.getByRole('button')).toBeInTheDocument();
    });

    it('should handle missing required props gracefully', () => {
      // Test error boundaries and prop validation
    });
  });

  describe('user interactions', () => {
    it('should handle click events', async () => {
      // Arrange
      const user = userEvent.setup();
      const handleClick = vi.fn();

      // Act
      render(<Component onClick={handleClick} />);
      await user.click(screen.getByRole('button'));

      // Assert
      expect(handleClick).toHaveBeenCalledOnce();
    });
  });

  describe('error handling', () => {
    it('should display error message on API failure', async () => {
      // Test error states
    });
  });

  describe('accessibility', () => {
    it('should have proper ARIA attributes', () => {
      // Test accessibility
    });
  });
});
```

### 4.2 Test Coverage Requirements

- **Minimum coverage**: 80% for all metrics
- **Critical paths**: 100% coverage required
- **Error handling**: All error paths must be tested
- **Edge cases**: Null, undefined, empty arrays, boundary conditions
- **Accessibility**: All interactive components must have a11y tests

### 4.3 Mock Strategy

```typescript
// __mocks__/websocket.mock.ts
export class MockWebSocket {
  readyState = WebSocket.CONNECTING;

  constructor(public url: string) {
    setTimeout(() => {
      this.readyState = WebSocket.OPEN;
      this.onopen?.({} as Event);
    }, 0);
  }

  send = vi.fn();
  close = vi.fn();
  onopen?: (event: Event) => void;
  onclose?: (event: CloseEvent) => void;
  onmessage?: (event: MessageEvent) => void;
  onerror?: (event: Event) => void;
}

// Use in tests
vi.mock('ws', () => ({
  WebSocket: MockWebSocket
}));
```

## 5. Integration Testing Standards

### 5.1 MSW Setup for API Mocking

```typescript
// src/test/mocks/handlers.ts
import { http, HttpResponse } from 'msw';

export const handlers = [
  http.post('/api/auth/session', () => {
    return HttpResponse.json({
      token: 'mock-jwt-token',
      expires_in: 3600
    });
  }),

  http.get('/api/sessions/:id', ({ params }) => {
    return HttpResponse.json({
      id: params.id,
      messages: []
    });
  })
];

// src/test/mocks/server.ts
import { setupServer } from 'msw/node';
import { handlers } from './handlers';

export const server = setupServer(...handlers);
```

### 5.2 Integration Test Template

```typescript
describe('WebSocket Integration', () => {
  let client: RealtimeClient;

  beforeAll(() => server.listen());
  afterEach(() => server.resetHandlers());
  afterAll(() => server.close());

  it('should establish connection and authenticate', async () => {
    // Test full connection flow
    client = new RealtimeClient({ apiKey: 'test-key' });

    await client.connect();

    await waitFor(() => {
      expect(client.isConnected()).toBe(true);
    });
  });
});
```

## 6. Package-Specific Requirements

### 6.1 Core Package (@agentc/realtime-core)

- Focus on unit tests for business logic
- Mock all external dependencies (WebSocket, Audio APIs)
- Test event emitter patterns thoroughly
- Test binary/text protocol handling
- Test reconnection logic with various failure scenarios

### 6.2 React Package (@agentc/realtime-react)

- Test all hooks with renderHook
- Test provider context updates
- Test cleanup and unmounting
- Test StrictMode compatibility
- Test SSR compatibility

### 6.3 UI Package (@agentc/realtime-ui)

- Test all user interactions
- Test accessibility (keyboard nav, ARIA)
- Test responsive behavior
- Test theme customization
- Visual regression tests for critical components

### 6.4 Demo Package

- E2E tests for critical user flows
- Test build process
- Test environment variable handling
- Test error boundaries
- Performance tests for main interactions

## 7. Test Commands

### 7.1 Package.json Scripts

```json
{
  "scripts": {
    "test": "vitest run",
    "test:watch": "vitest watch",
    "test:coverage": "vitest run --coverage",
    "test:ui": "vitest --ui",
    "test:integration": "vitest run --grep '\\.integration\\.'",
    "test:unit": "vitest run --grep -v '\\.integration\\.'"
  }
}
```

### 7.2 CI/CD Requirements

- All tests must pass before merge
- Coverage must meet thresholds
- No console.log statements in code
- Linting must pass
- Type checking must pass

## 8. Test Data Management

### 8.1 Fixtures Organization

```typescript
// __tests__/fixtures/messages.fixture.ts
export const mockTextMessage = {
  id: 'msg_001',
  role: 'assistant',
  content: 'Hello, how can I help you?',
  timestamp: new Date('2024-01-01T10:00:00Z')
};

export const mockStreamingMessage = {
  id: 'msg_002',
  role: 'assistant',
  content: '',
  isStreaming: true,
  timestamp: new Date('2024-01-01T10:00:01Z')
};
```

### 8.2 Test Data Factories

```typescript
// src/test/factories/message.factory.ts
import { faker } from '@faker-js/faker';

export function createMockMessage(overrides = {}) {
  return {
    id: faker.string.uuid(),
    content: faker.lorem.paragraph(),
    role: faker.helpers.arrayElement(['user', 'assistant']),
    timestamp: faker.date.recent(),
    ...overrides
  };
}
```

## 9. Performance Testing

### 9.1 Performance Benchmarks

```typescript
import { bench, describe } from 'vitest';

describe('Performance', () => {
  bench('message processing', () => {
    // Measure message processing speed
    processMessage(largeMessage);
  }, {
    time: 100, // milliseconds to run
    iterations: 1000 // or specific iteration count
  });
});
```

## 10. Debugging Tests

### 10.1 Debug Configuration

```typescript
// When debugging specific test
import { Logger } from '@/utils/logger';

describe.only('Debugging specific issue', () => {
  beforeAll(() => {
    Logger.setLevel(LogLevel.TRACE);
  });

  it('should reveal the issue', () => {
    // Your test with verbose logging
  });
});
```

## ENFORCEMENT

**ZERO TOLERANCE POLICY:**

1. No PR merges without 80% test coverage
2. No alternative test structures allowed
3. All packages must follow these standards EXACTLY
4. No console.log in production code
5. All tests must use proper mocking strategies

**CONSEQUENCES:**

- Non-compliant code will be rejected
- Team members must fix their tests before any other work
- Testing standards violations block all deployments
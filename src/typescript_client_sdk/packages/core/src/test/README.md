# Simple Testing Infrastructure

This directory contains our **SIMPLE** Level 1 testing utilities for the @agentc/realtime-core package.

## Philosophy

- **Tests are a safety net, not a work of art**
- **Simple is better than clever**
- **If you need to debug the test, the test is too complex**
- **95% of mocks should be simple vi.fn() stubs**

## Directory Structure

```
test/
├── shared/           # Reusable test utilities (all under 10 lines)
│   ├── singleton-helpers.ts
│   ├── mock-builders.ts
│   ├── event-helpers.ts
│   └── timer-helpers.ts
├── mocks/           # Simple browser API mocks (Level 1 stubs)
│   ├── websocket.mock.ts
│   ├── audio-context.mock.ts
│   ├── media-stream.mock.ts
│   └── fetch.mock.ts
└── examples/        # Example tests showing proper usage
    └── simple-mock-usage.test.ts
```

## Quick Start

### WebSocket Mocking

```typescript
import { installWebSocketMock } from '../test/mocks';

// One line setup
const MockWS = installWebSocketMock();

// Use it
const ws = new WebSocket('ws://example.com');
ws.send('hello');

// Assert
expect(MockWS.lastInstance().send).toHaveBeenCalledWith('hello');
```

### Fetch Mocking

```typescript
import { installFetchMock } from '../test/mocks';

// Setup with default response
installFetchMock({ user: 'test' });

// Use it
const response = await fetch('/api/user');
const data = await response.json();

// Assert
expect(data).toEqual({ user: 'test' });
```

### Audio Context Mocking

```typescript
import { installAudioContextMock } from '../test/mocks';

// Setup
const MockAudioContext = installAudioContextMock();

// Use it
const context = new AudioContext();
const source = context.createMediaStreamSource();

// Assert
expect(context.createMediaStreamSource).toHaveBeenCalled();
```

### Timer Control

```typescript
import { useFakeTimers } from '../test/shared';

// Setup
const timers = useFakeTimers();

// Control time
setTimeout(callback, 1000);
timers.advance(1000);

// Cleanup
timers.restore();
```

## Key Principles

1. **No Business Logic in Mocks** - Mocks just return values, they don't validate
2. **No Complex State Machines** - If you need state, use separate test cases
3. **Keep Helpers Under 10 Lines** - If it's longer, it belongs in the test
4. **Tests Should Be Obvious** - Anyone should understand the test in 30 seconds
5. **Mock Only What You Need** - Don't create elaborate mocks for simple tests

## Common Patterns

### Testing Async Operations

```typescript
// Simple - just use mockResolvedValue
const mockApi = vi.fn().mockResolvedValue({ data: 'test' });
```

### Testing Event Handlers

```typescript
// Simple - just call the handler directly
mockWebSocket.onmessage = messageHandler;
mockWebSocket.onmessage({ data: 'test' });
expect(messageHandler).toHaveBeenCalled();
```

### Testing Errors

```typescript
// Simple - just return an error
const mockFetch = vi.fn().mockResolvedValue({
  ok: false,
  status: 500
});
```

## What NOT to Do

❌ **Don't build complex mock state machines**
❌ **Don't simulate real browser behavior**
❌ **Don't create fluent APIs for test setup**
❌ **Don't write helpers longer than 10 lines**
❌ **Don't add business logic to mocks**

## Remember

> "Make it work, make it right, make it simple. In that order."

Every minute spent debugging a flaky test is a minute not spent improving the product.
# The Pragmatic Mocking Guide for Agent C Realtime

Note: Coverage output is placed in `.scratch/coverage`

> **Core Philosophy:** Tests should fail for a REASON. Flaky tests provide NEGATIVE value and erode confidence in testing as a whole. A simple test that always works is worth 100 clever tests that sometimes fail.

## 🎯 The Golden Rules

1. **Tests are a safety net, not a work of art**
2. **Simple is better than clever**
3. **Obvious is better than elegant**
4. **If you need to debug the test, the test is too complex**
5. **The more complicated the test, the more time we spend fixing tests than fixing code**
6. **USE PROTOCOL FIXTURES - Don't create test data that already exists**
7. **UNIVERSAL ENVIRONMENT - All packages use happy-dom for consistency**

## 🌍 Universal Test Environment

### All Packages Use happy-dom

**CRITICAL**: ALL packages in the monorepo use happy-dom as the test environment!

| Package | Environment | What's Available | What Needs Mocking |
|---------|-------------|------------------|--------------------|
| `@agentc/realtime-core` | `happy-dom` | DOM APIs, basic browser APIs | WebSocket, fetch, complex browser APIs |
| `@agentc/realtime-react` | `happy-dom` | DOM APIs, basic browser APIs | WebSocket, fetch, complex browser APIs |
| `@agentc/realtime-ui` | `happy-dom` | DOM APIs, basic browser APIs | WebSocket, fetch, complex browser APIs |
| `@agentc/demo` | `happy-dom` | DOM APIs, basic browser APIs | WebSocket, fetch, complex browser APIs |

### happy-dom Environment (All Tests)

**What happy-dom provides out of the box:**
- Document, Window, HTMLElement, and all standard DOM APIs
- Basic Event system (click, input, etc.)
- localStorage, sessionStorage
- Basic URL and Location APIs
- CSS style handling
- Form elements and controls

**What you MUST mock in happy-dom:**
```typescript
// WebSocket - not provided by happy-dom
global.WebSocket = vi.fn(() => ({
  send: vi.fn(),
  close: vi.fn(),
  addEventListener: vi.fn(),
  removeEventListener: vi.fn(),
  readyState: 1 // OPEN
}));

// fetch - needs explicit mock for control
global.fetch = vi.fn().mockResolvedValue({
  ok: true,
  json: async () => ({ data: 'test' })
});

// AudioContext and related APIs
global.AudioContext = vi.fn(() => mockAudioContext);
global.MediaStream = vi.fn();
global.MediaRecorder = vi.fn(() => mockMediaRecorder);
```

### Why happy-dom Everywhere?

**Consistency is key:**
- Same mocking patterns across all packages
- No environment-specific gotchas
- Easier to move code between packages
- Simpler test setup and configuration
- DOM APIs available even in core (though core shouldn't use them)

### Universal Environment Setup

#### Standard Setup for All Packages
```typescript
// packages/*/src/test/setup.ts
import '@testing-library/jest-dom'; // For UI packages
import { vi } from 'vitest';

// Mock what happy-dom doesn't provide - same for ALL packages
global.WebSocket = vi.fn(() => ({
  send: vi.fn(),
  close: vi.fn(),
  addEventListener: vi.fn(),
  removeEventListener: vi.fn(),
  readyState: WebSocket.OPEN
}));

// Mock fetch for controlled testing
global.fetch = vi.fn();

// Mock media APIs
global.MediaStream = vi.fn();
global.MediaRecorder = vi.fn();

// For core package - verify DOM isn't used even though it's available
if (process.env.PACKAGE_NAME === '@agentc/realtime-core') {
  // Optional: Add warnings if core tries to use DOM
  const warnIfUsed = (api: string) => {
    console.warn(`Warning: Core package accessing ${api} - this may indicate incorrect abstraction`);
  };
  // Could wrap document/window methods with warnings if needed
}
```

## 📦 Protocol Fixtures - Your First Stop for Test Data

**CRITICAL**: Before creating ANY test data, check `packages/core/src/test/fixtures/protocol-events.ts`

### What's Available:
```typescript
// Server events - Use for testing event handlers
import { serverEventFixtures } from '@/test/fixtures/protocol-events';
const textDelta = serverEventFixtures.textDelta;
const completion = serverEventFixtures.completionFinished;

// Client events - Use for testing event sending
import { clientEventFixtures } from '@/test/fixtures/protocol-events';
const textInput = clientEventFixtures.textInput;
const ping = clientEventFixtures.ping;

// Messages - Use for testing message handling
import { messageFixtures } from '@/test/fixtures/protocol-events';
const userMessage = messageFixtures.textMessage;
const multimodal = messageFixtures.multimodalMessage;

// Audio data - Use for testing audio processing
import { audioFixtures } from '@/test/fixtures/protocol-events';
const audioChunk = audioFixtures.pcm16Chunk;

// Complete flows - Use for integration tests
import { eventSequences } from '@/test/fixtures/protocol-events';
const textFlow = eventSequences.textInteraction;
const voiceFlow = eventSequences.voiceInteraction;
```

### ❌ DON'T Create What Already Exists:
```typescript
// ❌ BAD: Creating your own test event
const testTextDelta = {
  type: 'text_delta',
  content: 'test',
  // ... reinventing the wheel
};

// ✅ GOOD: Use the protocol fixture
import { serverEventFixtures } from '@/test/fixtures/protocol-events';
const testTextDelta = serverEventFixtures.textDelta;
```

### ✅ DO Extend When Needed:
```typescript
// ✅ GOOD: Extend existing fixture for specific test
import { serverEventFixtures } from '@/test/fixtures/protocol-events';
const customTextDelta = {
  ...serverEventFixtures.textDelta,
  content: 'specific test content',
  session_id: 'custom-session-id'
};
```

## 📊 The Three Levels of Mock Complexity

### Level 1: Simple Stubs (95% of our needs) ✅

**This is what we want 95% of the time:**

```typescript
// GOOD: Simple, obvious, maintainable
describe('AuthManager', () => {
  it('should refresh token when expired', async () => {
    // Simple stub - just returns what we need
    const mockFetch = vi.fn().mockResolvedValue({
      ok: true,
      json: async () => ({ token: 'new-token' })
    });
    global.fetch = mockFetch;

    const manager = new AuthManager();
    await manager.refreshToken();

    expect(mockFetch).toHaveBeenCalledWith('/api/auth/refresh', expect.any(Object));
    expect(manager.getToken()).toBe('new-token');
  });
});
```

**Key characteristics:**
- Uses `vi.fn()` directly
- Returns simple values
- No complex setup
- Test is readable in one glance

### Level 2: Behavior Verification (4% of our needs) ⚠️

**Use sparingly for critical paths:**

```typescript
// ACCEPTABLE: When we need to verify specific behavior sequences
describe('ReconnectionManager', () => {
  it('should implement exponential backoff', async () => {
    const attemptReconnect = vi.fn()
      .mockRejectedValueOnce(new Error('fail 1'))
      .mockRejectedValueOnce(new Error('fail 2'))
      .mockResolvedValueOnce(undefined);

    const manager = new ReconnectionManager(attemptReconnect);
    await manager.reconnect();

    // Verify the sequence matters here
    expect(attemptReconnect).toHaveBeenCalledTimes(3);
    // That's it - don't overcomplicate
  });
});
```

### Level 3: Stateful Mocks (1% of our needs) 🚨

**WARNING: Only use when absolutely necessary:**

```typescript
// RARE: Only for complex state machines that are CORE to the business logic
describe('TurnManager', () => {
  it('should handle turn state transitions', () => {
    let currentTurn = 'user';
    const mockTurnAPI = {
      getCurrentTurn: vi.fn(() => currentTurn),
      requestTurn: vi.fn((type) => {
        if (currentTurn === 'none') {
          currentTurn = type;
          return true;
        }
        return false;
      })
    };

    // Even this should be questioned - can we test without state?
  });
});
```

**🛑 STOP AND THINK:** If you're at Level 3, ask yourself:
- Can I test this with two separate test cases instead?
- Am I testing implementation instead of behavior?
- Will my teammate understand this in 6 months?

## ❌ BAD vs ✅ GOOD Examples

### WebSocket Mocking

```typescript
// ❌ BAD: Over-engineered WebSocket mock
class MockWebSocketBuilder {
  private handlers: Map<string, Function> = new Map();
  private messages: any[] = [];
  
  onMessage(type: string, handler: Function) {
    this.handlers.set(type, handler);
    return this;
  }
  
  willReceive(message: any) {
    this.messages.push(message);
    return this;
  }
  
  build() {
    // 50 more lines of complexity...
  }
}

// ✅ GOOD: Simple WebSocket stub
const createMockWebSocket = () => ({
  send: vi.fn(),
  close: vi.fn(),
  addEventListener: vi.fn(),
  removeEventListener: vi.fn(),
  readyState: WebSocket.OPEN
});

describe('WebSocketManager', () => {
  it('should send messages', () => {
    const mockWs = createMockWebSocket();
    global.WebSocket = vi.fn(() => mockWs);
    
    const manager = new WebSocketManager('ws://test');
    manager.send({ type: 'test' });
    
    expect(mockWs.send).toHaveBeenCalledWith('{"type":"test"}');
  });
});
```

### Fetch Mocking

```typescript
// ❌ BAD: Complex fetch interceptor
class FetchMockBuilder {
  private routes: Map<string, Response> = new Map();
  
  route(path: string) {
    return {
      reply: (status: number, data: any) => {
        // Complex routing logic...
      }
    };
  }
}

// ✅ GOOD: Simple fetch stub
describe('API calls', () => {
  it('should fetch user data', async () => {
    global.fetch = vi.fn().mockResolvedValue({
      ok: true,
      json: async () => ({ id: 1, name: 'Test User' })
    });
    
    const user = await fetchUser(1);
    
    expect(fetch).toHaveBeenCalledWith('/api/users/1');
    expect(user.name).toBe('Test User');
  });
});
```

## 🔧 Common Patterns We Actually Need

### How to Mock a Singleton (SIMPLY)

```typescript
// ✅ GOOD: Direct and obvious
vi.mock('./Logger', () => ({
  Logger: {
    getInstance: () => ({
      debug: vi.fn(),
      error: vi.fn(),
      warn: vi.fn()
    })
  }
}));
```

### How to Mock WebSocket (Universal Pattern)

```typescript
// ✅ GOOD: Same pattern for ALL packages
beforeEach(() => {
  global.WebSocket = vi.fn(() => ({
    send: vi.fn(),
    close: vi.fn(),
    addEventListener: vi.fn(),
    removeEventListener: vi.fn(),
    readyState: 1, // Use numeric value
    OPEN: 1,
    CLOSED: 3,
    CONNECTING: 0,
    CLOSING: 2
  }));
});

// This same mock works in core, react, ui, and demo packages!
```

### How to Mock Fetch (SIMPLY)

```typescript
// ✅ GOOD: One-liner for most cases
global.fetch = vi.fn().mockResolvedValue({
  ok: true,
  json: async () => ({ data: 'test' })
});
```

### How to Mock Timers (SIMPLY)

```typescript
// ✅ GOOD: Use Vitest's built-in timer mocks
beforeEach(() => {
  vi.useFakeTimers();
});

afterEach(() => {
  vi.useRealTimers();
});

it('should debounce calls', () => {
  const fn = vi.fn();
  const debounced = debounce(fn, 100);
  
  debounced();
  debounced();
  debounced();
  
  expect(fn).not.toHaveBeenCalled();
  
  vi.advanceTimersByTime(100);
  expect(fn).toHaveBeenCalledTimes(1);
});
```

### How to Mock AudioContext (Universal Pattern)

```typescript
// ✅ GOOD: Same mock for ALL packages
const mockAudioContext = {
  createMediaStreamSource: vi.fn(() => ({ connect: vi.fn() })),
  createScriptProcessor: vi.fn(() => ({ 
    connect: vi.fn(),
    disconnect: vi.fn(),
    onaudioprocess: null
  })),
  createAnalyser: vi.fn(() => ({
    connect: vi.fn(),
    disconnect: vi.fn(),
    getByteFrequencyData: vi.fn()
  })),
  destination: {},
  sampleRate: 48000,
  state: 'running',
  close: vi.fn().mockResolvedValue(undefined),
  suspend: vi.fn().mockResolvedValue(undefined),
  resume: vi.fn().mockResolvedValue(undefined)
};

// Standard setup for all packages
global.AudioContext = vi.fn(() => mockAudioContext);

// Also mock related APIs
global.MediaStream = vi.fn();
global.MediaRecorder = vi.fn(() => ({
  start: vi.fn(),
  stop: vi.fn(),
  pause: vi.fn(),
  resume: vi.fn(),
  ondataavailable: null,
  onerror: null,
  state: 'inactive'
}));

// Note: Core package should abstract audio through interfaces,
// but uses the same mocks for testing
```

## 🚫 What NOT to Do

### ❌ No Business Logic in Mocks
```typescript
// BAD: Mock shouldn't know business rules
const mockAuth = {
  login: vi.fn((user, pass) => {
    if (user === 'admin' && pass === '123') {  // ❌ Business logic!
      return { token: 'admin-token' };
    }
    throw new Error('Invalid credentials');
  })
};

// GOOD: Test controls the behavior
const mockAuth = {
  login: vi.fn().mockResolvedValue({ token: 'test-token' })
};
```

### ❌ No Complex State Machines
```typescript
// BAD: Mock maintaining complex state
class MockConnectionStateMachine {
  private state = 'disconnected';
  private listeners = new Map();
  
  transition(newState) {
    // 30 lines of state logic... ❌
  }
}

// GOOD: Simple state for specific test
let connectionState = 'connected';
const mockConnection = {
  getState: () => connectionState
};
```

### ❌ No Fluent APIs for Test Setup
```typescript
// BAD: Clever but not obvious
await TestBuilder
  .withUser('test')
  .withAuth()
  .withWebSocket()
  .expectingMessage('hello')
  .run();

// GOOD: Explicit and clear
const user = { id: 1, name: 'test' };
const mockWs = createMockWebSocket();
const result = await sendMessage(mockWs, 'hello');
expect(result).toBe('expected');
```

### ❌ No Monkey-Patching
```typescript
// BAD: Modifying prototypes
WebSocket.prototype.send = vi.fn();  // ❌ Never do this

// GOOD: Replace the constructor
global.WebSocket = vi.fn(() => mockWebSocket);
```

### ❌ No Global State Pollution
```typescript
// BAD: Tests affect each other
let globalMockData = { count: 0 };  // ❌ Shared state

// GOOD: Each test is isolated
beforeEach(() => {
  const mockData = { count: 0 };  // ✅ Fresh for each test
});
```

## 📝 Test Helper Guidelines

### Keep Helpers Under 10 Lines

```typescript
// ✅ GOOD: Simple, focused helper
const createMockUser = (overrides = {}) => ({
  id: 1,
  name: 'Test User',
  email: 'test@example.com',
  ...overrides
});

// ❌ BAD: Helper doing too much
const createCompleteTestEnvironment = () => {
  // 50 lines of setup...
};
```

### If It Needs More, It Belongs in the Specific Test

```typescript
// ✅ GOOD: Complex setup stays in the test
it('should handle complex scenario', () => {
  // This specific test needs complex setup
  const mockWebSocket = createMockWebSocket();
  const messageQueue = [];
  mockWebSocket.send.mockImplementation(msg => {
    messageQueue.push(msg);
    if (messageQueue.length === 3) {
      // Specific behavior for THIS test
    }
  });
  
  // Test implementation...
});
```

### Helpers Should Be Obvious, Not Clever

```typescript
// ❌ BAD: Clever but confusing
const mockify = (obj, ...props) => 
  props.reduce((acc, p) => ({ ...acc, [p]: vi.fn() }), obj);

// ✅ GOOD: Obvious what it does
const createMockLogger = () => ({
  debug: vi.fn(),
  info: vi.fn(),
  warn: vi.fn(),
  error: vi.fn()
});
```

### How to Mock DOM Elements (happy-dom Only)

```typescript
// ✅ GOOD: happy-dom provides real DOM, just use it
describe('UI Component', () => {
  it('should render button', () => {
    // happy-dom provides document, window, etc.
    render(<Button>Click me</Button>);
    
    // DOM queries work naturally
    const button = screen.getByRole('button');
    expect(button).toBeInTheDocument();
  });
  
  it('should handle DOM events', async () => {
    const user = userEvent.setup();
    const handleClick = vi.fn();
    
    render(<Button onClick={handleClick}>Click me</Button>);
    
    // happy-dom handles event propagation
    await user.click(screen.getByRole('button'));
    expect(handleClick).toHaveBeenCalled();
  });
});

// ❌ BAD: Don't mock DOM in happy-dom tests
// happy-dom already provides these!
document.createElement = vi.fn(); // ❌ Don't do this
window.addEventListener = vi.fn(); // ❌ Don't do this
```

### How to Mock Browser APIs Not in happy-dom

```typescript
// ✅ GOOD: Mock APIs that happy-dom doesn't provide

// Intersection Observer
global.IntersectionObserver = vi.fn(() => ({
  observe: vi.fn(),
  unobserve: vi.fn(),
  disconnect: vi.fn()
}));

// Resize Observer
global.ResizeObserver = vi.fn(() => ({
  observe: vi.fn(),
  unobserve: vi.fn(),
  disconnect: vi.fn()
}));

// Clipboard API
Object.assign(navigator, {
  clipboard: {
    writeText: vi.fn().mockResolvedValue(undefined),
    readText: vi.fn().mockResolvedValue('clipboard content')
  }
});

// Notification API
global.Notification = vi.fn(() => ({
  close: vi.fn()
}));
Notification.permission = 'granted';
Notification.requestPermission = vi.fn().mockResolvedValue('granted');
```

## 🎨 Example Test Structure

Here's a complete test file following our patterns:

```typescript
// AuthManager.test.ts (Core Package - happy-dom Environment)
import { describe, it, expect, beforeEach, afterEach, vi } from 'vitest';
import { AuthManager } from './AuthManager';

// Simple test utilities (< 10 lines each)
const createMockFetch = (response = {}) => {
  return vi.fn().mockResolvedValue({
    ok: true,
    json: async () => response
  });
};

describe('AuthManager', () => {
  let authManager: AuthManager;
  
  beforeEach(() => {
    // Clean setup for each test
    authManager = new AuthManager();
    vi.clearAllMocks();
  });
  
  afterEach(() => {
    // Clean up
    vi.restoreAllMocks();
  });
  
  describe('login', () => {
    it('should store token on successful login', async () => {
      // Arrange - Simple mock
      global.fetch = createMockFetch({ 
        token: 'test-token',
        expiresIn: 3600 
      });
      
      // Act
      await authManager.login('user', 'pass');
      
      // Assert - Clear expectations
      expect(fetch).toHaveBeenCalledWith('/api/auth/login', {
        method: 'POST',
        body: JSON.stringify({ username: 'user', password: 'pass' })
      });
      expect(authManager.getToken()).toBe('test-token');
    });
    
    it('should handle login failure', async () => {
      // Simple error case - no complex mock needed
      global.fetch = vi.fn().mockResolvedValue({
        ok: false,
        status: 401
      });
      
      await expect(authManager.login('user', 'wrong'))
        .rejects.toThrow('Authentication failed');
    });
  });
  
  describe('token refresh', () => {
    it('should refresh token before expiry', async () => {
      // Use fake timers - built-in and simple
      vi.useFakeTimers();
      
      global.fetch = createMockFetch({ 
        token: 'refreshed-token',
        expiresIn: 3600 
      });
      
      authManager.setToken('old-token', 1000); // expires in 1 second
      
      // Advance time to just before expiry
      vi.advanceTimersByTime(900);
      
      expect(fetch).toHaveBeenCalledWith('/api/auth/refresh', expect.any(Object));
      
      vi.useRealTimers();
    });
  });
});
```

## 🏗️ Package-Specific Mock Examples

### Core Package Mocks (happy-dom Environment)

```typescript
// packages/core/src/client/__tests__/WebSocketManager.test.ts
import { describe, it, expect, beforeEach, vi } from 'vitest';
import { WebSocketManager } from '../WebSocketManager';

// Mock WebSocket - same as other packages
const mockWebSocket = {
  send: vi.fn(),
  close: vi.fn(),
  addEventListener: vi.fn(),
  removeEventListener: vi.fn(),
  readyState: 1
};

beforeEach(() => {
  global.WebSocket = vi.fn(() => mockWebSocket);
});

describe('WebSocketManager', () => {
  it('should connect to WebSocket', () => {
    const manager = new WebSocketManager('ws://test.com');
    expect(global.WebSocket).toHaveBeenCalledWith('ws://test.com');
  });
});
```

### React Package Mocks (happy-dom Environment)

```typescript
// packages/react/src/hooks/__tests__/useRealtimeClient.test.tsx
import { describe, it, expect, beforeEach } from 'vitest';
import { renderHook, waitFor } from '@testing-library/react';
import { useRealtimeClient } from '../useRealtimeClient';

// WebSocket needs mocking even in happy-dom
beforeEach(() => {
  global.WebSocket = vi.fn(() => ({
    send: vi.fn(),
    close: vi.fn(),
    addEventListener: vi.fn(),
    removeEventListener: vi.fn(),
    readyState: 1
  }));
});

describe('useRealtimeClient', () => {
  it('should initialize client', () => {
    const { result } = renderHook(() => useRealtimeClient());
    expect(result.current.client).toBeDefined();
  });
});
```

### UI Package Mocks (happy-dom Environment)

```typescript
// packages/ui/src/components/__tests__/AudioControls.test.tsx
import { describe, it, expect, beforeEach, vi } from 'vitest';
import { render, screen } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { AudioControls } from '../AudioControls';

// Mock audio APIs not in happy-dom
beforeEach(() => {
  global.MediaStream = vi.fn();
  global.MediaRecorder = vi.fn(() => ({
    start: vi.fn(),
    stop: vi.fn(),
    state: 'inactive'
  }));
});

describe('AudioControls', () => {
  it('should toggle microphone', async () => {
    const user = userEvent.setup();
    render(<AudioControls />);
    
    const micButton = screen.getByRole('button', { name: /microphone/i });
    await user.click(micButton);
    
    // Test UI state changes, not implementation
    expect(micButton).toHaveAttribute('aria-pressed', 'true');
  });
});
```

## 🎯 The Final Word

When writing tests, ask yourself:

1. **Can my teammate understand this test in 30 seconds?**
2. **Will this test break for the RIGHT reasons?**
3. **Am I testing behavior or implementation?**
4. **Is this the simplest way to verify this works?**

If the answer to any of these is "no", simplify your approach.

Remember: **We're building a safety net, not a work of art.** Every minute spent debugging a flaky test is a minute not spent improving the product.

---

*"Make it work, make it right, make it simple. In that order."*

## 📋 Quick Reference Checklist

### Before Writing Any Mock:

- [ ] **Remember environment**: All packages use happy-dom
- [ ] **Check fixtures**: Does this test data already exist in protocol fixtures?
- [ ] **Check complexity**: Can I use a simple `vi.fn()` instead?
- [ ] **Check happy-dom**: Is this API already provided by happy-dom?
- [ ] **Check isolation**: Will this mock affect other tests?
- [ ] **Check clarity**: Will my teammate understand this in 30 seconds?

### Environment Quick Check:

```typescript
// All packages should show happy-dom environment
console.log('Test environment:', process.env.VITEST_ENV || 'not set'); // Should be 'happy-dom'
console.log('Has window?:', typeof window !== 'undefined'); // Should be true
console.log('Has document?:', typeof document !== 'undefined'); // Should be true
```

### Common Gotchas:

1. **WebSocket constants**: Use numeric values (1 for OPEN) in mocks
2. **happy-dom has localStorage**: Don't mock it - it's provided
3. **fetch needs mocking**: Even in happy-dom, mock it for control
4. **AudioContext always needs mocking**: Not provided by happy-dom
5. **Core package discipline**: Even though DOM is available, core shouldn't use it

Remember: All packages use happy-dom, so mocking patterns are consistent everywhere!
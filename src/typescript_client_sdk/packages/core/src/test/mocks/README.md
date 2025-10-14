# Test Mocks

This directory contains simple, reusable mocks for browser APIs used in unit tests.

## Philosophy

These mocks follow the KISS principle:
- **Dead simple** - Just `vi.fn()` stubs, no complex logic
- **Configurable** - Tests control the behavior
- **Isolated** - Each test configures mocks as needed
- **No state** - Mocks don't maintain state between calls

## Available Mocks

### WebSocket
```typescript
import { mockWebSocket, MockWebSocketConstructor, resetWebSocketMock } from '@/test/mocks';

// In your test
beforeEach(() => {
  global.WebSocket = MockWebSocketConstructor;
});

afterEach(() => {
  resetWebSocketMock();
});

it('should send message', () => {
  const ws = new WebSocket('ws://test');
  ws.send('test');
  expect(mockWebSocket.send).toHaveBeenCalledWith('test');
});
```

### AudioContext
```typescript
import { mockAudioContext, MockAudioContextConstructor, resetAudioContextMock } from '@/test/mocks';

beforeEach(() => {
  global.AudioContext = MockAudioContextConstructor;
});

afterEach(() => {
  resetAudioContextMock();
});

it('should create gain node', () => {
  const ctx = new AudioContext();
  const gain = ctx.createGain();
  expect(mockAudioContext.createGain).toHaveBeenCalled();
});
```

### MediaStream
```typescript
import { mockNavigator, mockMediaStream, resetMediaStreamMock } from '@/test/mocks';

beforeEach(() => {
  global.navigator = mockNavigator as unknown as Navigator;
});

afterEach(() => {
  resetMediaStreamMock();
});

it('should get user media', async () => {
  const stream = await navigator.mediaDevices.getUserMedia({ audio: true });
  expect(stream).toBe(mockMediaStream);
});
```

### Browser APIs
```typescript
import { MockAbortControllerConstructor, MockURLConstructor, resetBrowserAPIMocks } from '@/test/mocks';

beforeEach(() => {
  global.AbortController = MockAbortControllerConstructor;
  global.URL = MockURLConstructor as unknown as typeof URL;
});

afterEach(() => {
  resetBrowserAPIMocks();
});
```

## Usage Guidelines

### 1. Configure in Test, Not in Mock

```typescript
// ❌ BAD: Logic in mock
const mockAuth = {
  login: vi.fn((user, pass) => {
    if (user === 'admin') return { token: 'admin' };
    throw new Error('Invalid');
  })
};

// ✅ GOOD: Configure in test
it('should handle login success', () => {
  mockWebSocket.send.mockImplementation(() => {
    // Trigger success response
    mockWebSocket.onmessage({ data: JSON.stringify({ type: 'auth_success' }) });
  });
  // ... test logic
});

it('should handle login failure', () => {
  mockWebSocket.send.mockImplementation(() => {
    // Trigger error response
    mockWebSocket.onerror(new Error('Auth failed'));
  });
  // ... test logic
});
```

### 2. Reset Between Tests

Always reset mocks in `afterEach` to prevent test pollution:

```typescript
afterEach(() => {
  resetAllMocks(); // Resets all mocks at once
  // OR reset individually
  resetWebSocketMock();
  resetAudioContextMock();
});
```

### 3. Use Type Assertions When Needed

```typescript
// When TypeScript complains about mock types
global.navigator = mockNavigator as unknown as Navigator;
global.WebSocket = MockWebSocketConstructor as unknown as typeof WebSocket;
```

### 4. Mock Only What You Need

Don't mock APIs that aren't used in the code under test:

```typescript
// If testing WebSocket manager, only mock WebSocket
beforeEach(() => {
  global.WebSocket = MockWebSocketConstructor;
});

// Don't also mock AudioContext, MediaStream, etc. unless needed
```

## Examples

### Testing Event Handlers

```typescript
it('should handle WebSocket open event', () => {
  const onOpen = vi.fn();
  const manager = new WebSocketManager('ws://test', { onOpen });
  
  // Simulate WebSocket opening
  mockWebSocket.onopen(new Event('open'));
  
  expect(onOpen).toHaveBeenCalled();
});
```

### Testing Async Operations

```typescript
it('should resume audio context', async () => {
  mockAudioContext.resume.mockResolvedValue(undefined);
  
  const audioManager = new AudioManager();
  await audioManager.start();
  
  expect(mockAudioContext.resume).toHaveBeenCalled();
});
```

### Testing Error Scenarios

```typescript
it('should handle getUserMedia rejection', async () => {
  mockMediaDevices.getUserMedia.mockRejectedValue(new Error('Permission denied'));
  
  const audioInput = new AudioInput();
  await expect(audioInput.start()).rejects.toThrow('Permission denied');
});
```

## Remember

- **Keep it simple** - If the mock is getting complex, the test design might be wrong
- **Test behavior, not implementation** - Focus on what the code does, not how
- **One assertion per test** - Makes failures obvious
- **Use fixtures for data** - Import from `@/test/fixtures/protocol-events` for protocol data
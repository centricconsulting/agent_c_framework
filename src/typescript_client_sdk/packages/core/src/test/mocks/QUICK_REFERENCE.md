# Mock Quick Reference

## Setup in Test File

```typescript
import { vi, describe, it, expect, beforeEach, afterEach } from 'vitest';
import {
  mockWebSocket,
  MockWebSocketConstructor,
  mockAudioContext,
  MockAudioContextConstructor,
  mockNavigator,
  resetAllMocks
} from '@/test/mocks';

beforeEach(() => {
  global.WebSocket = MockWebSocketConstructor as unknown as typeof WebSocket;
  global.AudioContext = MockAudioContextConstructor as unknown as typeof AudioContext;
  global.navigator = mockNavigator as unknown as Navigator;
});

afterEach(() => {
  resetAllMocks();
});
```

## Common Patterns

### WebSocket - Send and Receive
```typescript
// Test sends data
ws.send('test');
expect(mockWebSocket.send).toHaveBeenCalledWith('test');

// Simulate receiving data
mockWebSocket.onmessage({ data: 'response' });
```

### WebSocket - Connection States
```typescript
// Change state
mockWebSocket.readyState = WebSocket.CLOSED;

// Simulate events
mockWebSocket.onopen(new Event('open'));
mockWebSocket.onclose({ code: 1000, reason: 'Normal' });
mockWebSocket.onerror(new Error('Connection failed'));
```

### AudioContext - Basic Usage
```typescript
const ctx = new AudioContext();
const gain = ctx.createGain();

// Check calls
expect(mockAudioContext.createGain).toHaveBeenCalled();

// Configure return values
mockAudioContext.createGain.mockReturnValue(customGainNode);
```

### MediaStream - Get User Media
```typescript
// Success case
const stream = await navigator.mediaDevices.getUserMedia({ audio: true });
expect(stream.getTracks()).toHaveLength(1);

// Error case
mockNavigator.mediaDevices.getUserMedia.mockRejectedValue(
  new Error('Permission denied')
);
```

### AudioWorklet - Message Passing
```typescript
const node = new AudioWorkletNode(ctx, 'processor');

// Send to worklet
node.port.postMessage({ type: 'config' });
expect(mockAudioWorkletNode.port.postMessage).toHaveBeenCalledWith({ type: 'config' });

// Receive from worklet
mockAudioWorkletNode.port.onmessage({ data: { type: 'ready' } });
```

### AbortController - Cancellation
```typescript
const controller = new AbortController();
controller.abort('User cancelled');

expect(mockAbortController.signal.aborted).toBe(true);
expect(mockAbortController.signal.reason).toBe('User cancelled');
```

## Configure Mock Behavior

### Return Different Values
```typescript
// Return different values on each call
mockWebSocket.send
  .mockReturnValueOnce('first')
  .mockReturnValueOnce('second');

// Always return same value
mockAudioContext.sampleRate = 48000;
```

### Async Mocks
```typescript
// Resolve
mockAudioContext.resume.mockResolvedValue(undefined);

// Reject
mockNavigator.mediaDevices.getUserMedia.mockRejectedValue(
  new Error('Not allowed')
);
```

### Check Call Arguments
```typescript
// Was called with specific args
expect(mockWebSocket.send).toHaveBeenCalledWith('test');

// Was called N times
expect(mockWebSocket.send).toHaveBeenCalledTimes(3);

// Get call arguments
const [firstCall, secondCall] = mockWebSocket.send.mock.calls;
```

## Reset Between Tests

```typescript
afterEach(() => {
  // Reset all mocks at once
  resetAllMocks();
  
  // Or reset individually
  resetWebSocketMock();
  resetAudioContextMock();
  
  // Clear all vi mocks
  vi.clearAllMocks();
});
```
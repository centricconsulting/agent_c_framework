# MSW (Mock Service Worker) Setup for React Package Testing

This directory contains the Mock Service Worker (MSW) configuration for testing React hooks and components in the `@agentc/realtime-react` package.

## Directory Structure

```
mocks/
├── server.ts                 # MSW server setup and lifecycle functions
├── handlers/                 # Request handlers organized by domain
│   ├── index.ts             # Main handler exports
│   ├── session-handlers.ts  # Session management mocks
│   ├── message-handlers.ts  # Message and streaming mocks
│   ├── audio-handlers.ts    # Audio functionality mocks
│   ├── avatar-handlers.ts   # Avatar state mocks
│   └── connection-handlers.ts # WebSocket-like connection mocks
├── test-helpers.ts          # Utility functions for testing
├── example-hook-test.tsx    # Example test patterns
└── README.md               # This file
```

## Quick Start

### Basic Usage in Tests

```typescript
import { describe, it, expect, beforeEach } from 'vitest';
import { renderHook } from '@testing-library/react';
import { resetMockStores, createTestSession } from '@test/mocks/test-helpers';

describe('Your Hook Test', () => {
  beforeEach(() => {
    // Reset all mock data before each test
    resetMockStores();
  });
  
  it('should fetch data', async () => {
    // Create test data
    const session = createTestSession(5);
    
    // Test your hook
    const { result } = renderHook(() => useYourHook());
    // ... test logic
  });
});
```

## Available Mock Handlers

### Session Management
- `GET /api/sessions` - List all sessions with pagination and search
- `POST /api/sessions` - Create a new session
- `GET /api/sessions/:id` - Get a specific session
- `PATCH /api/sessions/:id` - Update a session
- `DELETE /api/sessions/:id` - Delete a session
- `GET /api/sessions/:id/history` - Get session history
- `POST /api/sessions/:id/activate` - Switch active session

### Message Handling
- `POST /api/sessions/:sessionId/messages` - Send a message
- `GET /api/sessions/:sessionId/stream` - Stream message response (SSE)
- `GET /api/sessions/:sessionId/messages` - Get messages with pagination
- `DELETE /api/sessions/:sessionId/messages/:messageId` - Delete a message
- `PATCH /api/sessions/:sessionId/messages/:messageId` - Edit a message

### Audio Control
- `GET /api/audio/status` - Get audio and turn state
- `POST /api/audio/record/start` - Start recording
- `POST /api/audio/record/stop` - Stop recording
- `POST /api/audio/stream/start` - Start audio streaming
- `POST /api/audio/stream/stop` - Stop audio streaming
- `POST /api/audio/interrupt` - Interrupt current audio
- `PATCH /api/audio/settings` - Update audio settings
- `GET /api/audio/voice-models` - Get available voice models
- `GET /api/audio/devices` - Get audio devices
- `GET /api/audio/turns` - Get turn history

### Avatar Control
- `GET /api/avatar/status` - Get avatar state
- `POST /api/avatar/visibility` - Toggle avatar visibility
- `POST /api/avatar/animate` - Trigger animation
- `POST /api/avatar/expression` - Set expression
- `POST /api/avatar/position` - Update position/rotation
- `GET /api/avatar/animations` - Get available animations
- `POST /api/avatar/sync-audio` - Sync with audio (lip sync)
- `PATCH /api/avatar/settings` - Update avatar settings
- `POST /api/avatar/reset` - Reset to default state
- `GET /api/avatar/events` - Avatar events stream (SSE)

### Connection Management
- `POST /api/connect` - Establish connection
- `POST /api/disconnect` - Close connection
- `GET /api/connection/status` - Get connection status
- `POST /api/connection/ping` - Heartbeat
- `POST /api/connection/subscribe` - Subscribe to events
- `POST /api/connection/unsubscribe` - Unsubscribe from events
- `POST /api/connection/reconnect` - Reconnect
- `GET /api/connection/events` - Event stream (SSE)
- `POST /api/connection/send-event` - Send event to server

## Test Helpers

### Store Management
```typescript
// Reset all mock stores to initial state
resetMockStores();
```

### Connection Helpers
```typescript
// Simulate a connected WebSocket state
const sessionId = mockConnectedState();

// Create a WebSocket event
const event = createWebSocketEvent('turn_start', {
  turn_id: 'turn_1',
  turn_type: 'user'
});
```

### Session Helpers
```typescript
// Create a test session with messages
const session = createTestSession(5); // Creates 5 messages

// Create multiple sessions
const sessions = createTestSessions(10); // Creates 10 sessions
```

### Audio Helpers
```typescript
// Simulate audio recording state
mockAudioRecording();

// Simulate audio streaming state
mockAudioStreaming();
```

### Error Simulation
```typescript
// Mock an API error
mockApiError('/api/sessions/123', {
  message: 'Session not found',
  status: 404
});

// Mock a delayed response
mockApiDelay('/api/sessions', 2000);
```

### Streaming Responses
```typescript
// Mock a streaming response
mockStreamingResponse('/api/stream', [
  { type: 'start', id: '1' },
  { type: 'chunk', content: 'Hello' },
  { type: 'end', id: '1' }
], 100); // 100ms between chunks
```

## Advanced Patterns

### Adding Custom Handlers

You can add custom handlers for specific tests:

```typescript
import { addTestHandler } from '@test/mocks/server';
import { http, HttpResponse } from 'msw';

it('should handle custom endpoint', async () => {
  addTestHandler(
    http.get('/api/custom', () => {
      return HttpResponse.json({ custom: 'data' });
    })
  );
  
  // Your test logic
});
```

### Simulating Event Sequences

```typescript
await simulateEventSequence([
  { type: 'turn_start', data: { turn_type: 'user' }, delay: 100 },
  { type: 'message', data: { content: 'Hello' }, delay: 200 },
  { type: 'turn_end', data: { turn_type: 'user' }, delay: 100 }
]);
```

### Testing Optimistic Updates

```typescript
it('should handle optimistic updates', async () => {
  // Mock a delayed failure
  addTestHandler(
    http.post('/api/sessions/*/messages', async () => {
      await new Promise(r => setTimeout(r, 500));
      return HttpResponse.json({ error: 'Failed' }, { status: 500 });
    })
  );
  
  // Test optimistic update and rollback
});
```

## Mock Data State

The handlers maintain stateful mock data that persists within a test:

- **Sessions**: Stored in `sessionStore` Map
- **Audio State**: Maintained in `audioState` object
- **Turn State**: Tracked in `turnState` object
- **Avatar State**: Stored in `avatarState` object
- **Connection State**: Managed in `connectionState` object

All state is reset between tests via `resetMockStores()` in the `beforeEach` hook.

## SSE (Server-Sent Events) Support

Several endpoints support SSE for real-time updates:

- `/api/sessions/:sessionId/stream` - Message streaming
- `/api/connection/events` - WebSocket-like events
- `/api/avatar/events` - Avatar animation events

## Performance Testing

The mocks are designed to support performance testing:

```typescript
it('should handle large datasets', async () => {
  // Create 1000 sessions
  createTestSessions(1000);
  
  const start = performance.now();
  const response = await fetch('/api/sessions?limit=1000');
  const duration = performance.now() - start;
  
  expect(duration).toBeLessThan(1000);
});
```

## Debugging

To debug MSW requests:

1. Handlers warn on unhandled requests by default
2. Use `console.log` in handlers to inspect requests
3. Check the browser DevTools Network tab (in UI tests)
4. Use the `getMockRequestHistory()` helper (when implemented)

## Integration with Core Package Mocks

When testing React-Core integration, you can import Core package mocks:

```typescript
import { MockWebSocket } from '@agentc/realtime-core/src/test/mocks/websocket.mock';
import { mockAudioContext } from '@agentc/realtime-core/src/test/mocks/audio-context.mock';
```

## Best Practices

1. **Always reset stores**: Call `resetMockStores()` in `beforeEach`
2. **Use test helpers**: Leverage the provided helper functions
3. **Mock realistically**: Use realistic delays and data structures
4. **Test error cases**: Always test both success and failure paths
5. **Clean up custom handlers**: They persist across tests in the same file
6. **Test streaming**: Use SSE endpoints for real-time features
7. **Performance test**: Validate performance with large datasets

## Troubleshooting

### Common Issues

**Issue**: Unhandled request warnings
- **Solution**: Check that the endpoint pattern matches exactly
- MSW uses glob patterns, ensure wildcards are correct

**Issue**: State persisting between tests
- **Solution**: Ensure `resetMockStores()` is called in `beforeEach`

**Issue**: Streaming responses not working
- **Solution**: Use the `mockStreamingResponse()` helper or check SSE headers

**Issue**: Custom handlers not working
- **Solution**: Add them before making requests, they're added to the front of the handler stack

## Examples

See `example-hook-test.tsx` for comprehensive examples of:
- Basic API mocking
- Streaming responses
- WebSocket events
- Custom handlers
- Complex scenarios
- Performance testing
- Component testing with MSW
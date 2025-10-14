# Test Fixtures

This directory contains test fixtures for the Agent C Realtime Client SDK.

## Files

### `protocol-events.json`
JSON fixtures containing real Agent C Realtime API protocol events for testing.

### `protocol-events.ts`
TypeScript fixtures with properly typed protocol events, messages, and helper functions.
- Imports actual types from `src/events/types/`
- Provides typed event fixtures for both server and client events
- Includes helper functions for creating mock WebSocket events

### `audio-test-data.ts`
Audio test utilities and sample data for testing audio functionality.

## Important Notes

⚠️ **These are REAL protocol events** - The Agent C Realtime Client SDK communicates with the Agent C Realtime API. The events in these fixtures match the actual protocol defined in:
- `src/events/types/ServerEvents.ts`
- `src/events/types/ClientEvents.ts`
- `src/events/types/CommonTypes.ts`

⚠️ **Do NOT create fantasy fixtures** - All test data must be based on:
1. The TypeScript type definitions in the SDK
2. Real examples from the reference data in `.scratch/chat_session_ref/`

## Usage

```typescript
import { serverEventFixtures, clientEventFixtures, messageFixtures } from '../fixtures/protocol-events';

// Use in tests
describe('WebSocketManager', () => {
  it('should handle text_delta events', () => {
    const event = serverEventFixtures.textDelta;
    // ... test logic
  });
});
```
# Test Mocks Summary

This document summarizes all the test mocks created for the Agent C Realtime Client SDK. All mocks follow the principles from the [MOCKING_GUIDE.md](.scratch/test_planning/MOCKING_GUIDE.md):

- **Simple**: Just vi.fn() stubs with predictable return values
- **Obvious**: Clear what each mock does
- **No State or Logic**: Mocks are stateless and contain no business logic

## Core Package Mocks
Location: `packages/core/src/__mocks__/`

### Browser API Mocks
- **websocket.mock.ts** - WebSocket constructor and methods
- **audio-api.mock.ts** - AudioContext, AudioWorkletNode, and related audio APIs
- **media-stream.mock.ts** - MediaStream, getUserMedia, and MediaStreamTrack
- **navigator.mock.ts** - navigator.mediaDevices and other navigator APIs
- **fetch.mock.ts** - Fetch API with Response object
- **storage.mock.ts** - localStorage and sessionStorage
- **browser-apis.mock.ts** - Consolidated setup/cleanup for all browser mocks
- **test-helpers.ts** - Utility functions for working with mocks

### Usage
```typescript
import { setupBrowserMocks, cleanupBrowserMocks } from '@/__mocks__/browser-apis.mock';

beforeEach(() => setupBrowserMocks());
afterEach(() => cleanupBrowserMocks());
```

## React Package Mocks
Location: `packages/react/src/__mocks__/`

### Client and Testing Utilities
- **realtime-client.mock.ts** - Mock RealtimeClient with all methods stubbed
- **react-test-utils.tsx** - React testing utilities including renderWithProvider
- **index.ts** - Central exports for all React mocks

### Usage
```typescript
import { renderWithProvider, createMockRealtimeClient } from '@/__mocks__';

const mockClient = createMockRealtimeClient();
const { getByText } = renderWithProvider(<MyComponent />, { client: mockClient });
```

## UI Package Mocks
Location: `packages/ui/src/__mocks__/`

### UI Testing Utilities
- **ui-test-utils.tsx** - UI-specific testing utilities (ResizeObserver, IntersectionObserver, etc.)
- **index.ts** - Central exports for all UI mocks

### Existing Mocks (in test/mocks/)
- **realtime-react.ts** - Comprehensive mocks for all @agentc/realtime-react hooks

### Usage
```typescript
import { setupUIMocks, cleanupUIMocks, updateMockState } from '@/__mocks__';

beforeEach(() => setupUIMocks());
afterEach(() => cleanupUIMocks());

// Update hook mock state
updateMockState('audio', { isRecording: true });
```

## Protocol Fixtures
Location: `packages/core/src/test/fixtures/protocol-events.ts`

**IMPORTANT**: Always use these fixtures for test data instead of creating your own.

### Available Fixtures
- `serverEventFixtures` - All server event types
- `clientEventFixtures` - All client event types  
- `messageFixtures` - Sample messages with multimodal content
- `audioFixtures` - Binary audio data samples
- `eventSequences` - Common event sequences for integration tests

### Usage
```typescript
import { serverEventFixtures, clientEventFixtures } from '@/__mocks__';

const textDelta = serverEventFixtures.textDelta;
const textInput = clientEventFixtures.textInput;
```

## Key Principles

1. **No Business Logic in Mocks** - Mocks just return values, no calculations
2. **Use Protocol Fixtures** - Never create your own event/message test data
3. **Keep It Simple** - If you need complex behavior, put it in the test
4. **Setup/Cleanup Pattern** - Always setup in beforeEach, cleanup in afterEach
5. **Mock at Boundaries** - Mock external dependencies, not internal logic

## Common Testing Patterns

### Basic Test Structure
```typescript
describe('Component/Function', () => {
  beforeEach(() => {
    setupBrowserMocks(); // or setupUIMocks() for UI tests
  });
  
  afterEach(() => {
    cleanupBrowserMocks(); // or cleanupUIMocks()
  });
  
  it('should follow AAA pattern', () => {
    // Arrange
    const mock = createMockWebSocket();
    
    // Act
    const result = doSomething(mock);
    
    // Assert
    expect(result).toBe(expected);
  });
});
```

### Extending Mocks for Specific Tests
```typescript
it('should handle specific scenario', () => {
  const mockWs = createMockWebSocket();
  
  // Add specific behavior for this test only
  mockWs.send.mockImplementation(() => {
    throw new Error('Connection lost');
  });
  
  // Test error handling...
});
```

## Next Steps for Testing

With these mocks in place, you can now:

1. Write unit tests for all core functionality
2. Test React hooks and components in isolation
3. Test UI components without real browser APIs
4. Create integration tests using event sequences
5. Test error handling and edge cases

Remember: These mocks are intentionally simple. Complex test scenarios should be built in the test files themselves, not in the mocks.
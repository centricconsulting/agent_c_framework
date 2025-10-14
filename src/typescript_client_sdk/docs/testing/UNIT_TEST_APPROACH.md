# Unit Test Approach - CORRECTED

Note: Coverage output is placed in `.scratch/coverage`

## Core Principle
**We are writing UNIT tests, not integration tests**

Unit tests verify that individual functions/methods work correctly in isolation. They:
- Test one method at a time
- Mock ALL external dependencies
- Test simple input → output behavior
- Do NOT test workflows, business logic, or component interactions

## What Unit Tests ARE

### Definition
A unit test verifies that a single function/method returns the expected output given specific inputs.

### Examples of CORRECT Unit Tests

```typescript
// Testing a simple getter
it('isConnected() returns true when state is OPEN', () => {
  const manager = new WebSocketManager({ url: 'ws://test' });
  manager['ws'] = { readyState: WebSocket.OPEN } as any;
  expect(manager.isConnected()).toBe(true);
});

// Testing a setter
it('setUrl() updates the URL', () => {
  const manager = new WebSocketManager({ url: 'ws://old' });
  manager.setUrl('ws://new');
  expect(manager.getUrl()).toBe('ws://new');
});

// Testing a method that calls a dependency
it('send() calls ws.send with data', () => {
  const mockSend = vi.fn();
  const manager = new WebSocketManager({ url: 'ws://test' });
  manager['ws'] = { readyState: WebSocket.OPEN, send: mockSend } as any;
  
  manager.send('test data');
  expect(mockSend).toHaveBeenCalledWith('test data');
});

// Testing error conditions
it('send() throws when not connected', () => {
  const manager = new WebSocketManager({ url: 'ws://test' });
  expect(() => manager.send('test')).toThrow('WebSocket is not connected');
});
```

## What Unit Tests ARE NOT

### These are NOT unit tests:
- Testing connection lifecycles
- Testing message flows
- Testing state machines
- Testing business logic algorithms
- Testing workflows
- Testing integration between components

### Examples of INCORRECT "Unit" Tests (These are Integration Tests)

```typescript
// ❌ WRONG - Testing a workflow
it('should handle complete heartbeat cycle', () => {
  // Tests ping-pong workflow - NOT a unit test
});

// ❌ WRONG - Testing business logic
it('should implement exponential backoff', () => {
  // Tests backoff algorithm - NOT a unit test
});

// ❌ WRONG - Testing state transitions
it('should transition from CONNECTING to CONNECTED', () => {
  // Tests state machine - NOT a unit test
});

// ❌ WRONG - Testing component interaction
it('should accumulate text deltas into a message', () => {
  // Tests message building workflow - NOT a unit test
});
```

## Test Structure Template

Every test file should follow this simple structure:

```typescript
describe('ClassName', () => {
  // Constructor tests
  describe('constructor', () => {
    it('stores config', () => {});
    it('initializes properties', () => {});
  });
  
  // Simple getter tests
  describe('getters', () => {
    it('getX() returns X property', () => {});
    it('isY() returns Y boolean', () => {});
  });
  
  // Simple setter tests
  describe('setters', () => {
    it('setX() updates X property', () => {});
    it('throws on invalid input', () => {});
  });
  
  // Method tests (one per method)
  describe('methodName()', () => {
    it('returns expected output for input A', () => {});
    it('returns expected output for input B', () => {});
    it('throws on invalid input', () => {});
    it('calls dependency method', () => {});
  });
});
```

## Mocking Strategy

### Simple Stub Pattern
```typescript
// Mock everything as simple stubs
const mockWebSocket = {
  send: vi.fn(),
  close: vi.fn(),
  readyState: WebSocket.OPEN
};

const mockStorage = {
  getItem: vi.fn(),
  setItem: vi.fn(),
  removeItem: vi.fn()
};

const mockLogger = {
  debug: vi.fn(),
  info: vi.fn(),
  warn: vi.fn(),
  error: vi.fn()
};
```

### NO Complex Mocks
- NO mock builders
- NO fluent APIs
- NO stateful mocks
- NO business logic in mocks

## What We're Testing

For each class, we test:

1. **Constructor** - Does it store config? Initialize properties?
2. **Getters** - Do they return the right property?
3. **Setters** - Do they set the right property?
4. **Pure Functions** - Given input X, do they return Y?
5. **Side Effects** - Do they call the right dependency method?
6. **Error Cases** - Do they throw when they should?

## What We're NOT Testing

- Workflows
- State machines
- Business logic
- Integration between components
- Browser-specific functionality
- Event sequences
- Message accumulation
- Connection lifecycles
- Authentication flows
- Any multi-step process

## File Naming

- `ClassName.test.ts` - Unit tests ONLY
- `ClassName.integration.test.ts` - Integration tests (if needed)

## Coverage Goals

- Test every public method
- Test error conditions
- Test edge cases (null, undefined, empty)
- Don't test private methods directly
- Don't test implementation details

## Remember

**A unit test should:**
- Be simple enough to understand in 10 seconds
- Test ONE thing
- Have an obvious name that describes what it tests
- Not require debugging the test itself
- Run in milliseconds
- Never fail randomly

**If your test:**
- Needs complex setup → It's not a unit test
- Tests multiple methods → It's not a unit test
- Verifies a workflow → It's not a unit test
- Needs fake timers → It's probably not a unit test
- Tests state transitions → It's not a unit test

## The Bottom Line

We're writing a SAFETY NET of simple unit tests that verify individual functions work as expected. Nothing more.

Each test should answer: **"Does this function do what its name says it does?"**

That's it. No workflows. No business logic. No integration. Just simple function verification.
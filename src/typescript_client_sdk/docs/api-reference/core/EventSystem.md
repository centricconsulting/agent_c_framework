# EventSystem API Reference

The Agent C Realtime SDK event system provides type-safe, strongly-typed event handling throughout the SDK. It consists of two core classes: `EventEmitter` for event handling and `EventRegistry` for event type management and validation.

## Overview

The event system is the foundation for all real-time communication in the SDK. It provides:

- **Type-safe event handling** with TypeScript generics
- **Event type validation** and registration
- **Discriminated unions** for event type safety
- **Memory-safe listener management** with warnings
- **Error isolation** in event handlers
- **Support for one-time listeners**

## Import

```typescript
import { 
  EventEmitter, 
  EventListener,
  EventRegistry,
  EventType,
  ClientEventType,
  ServerEventType,
  EventData,
  RealtimeEventMap
} from '@agentc/realtime-core';
```

## EventEmitter Class

The `EventEmitter` class provides a type-safe event emitter with TypeScript generics support.

### Constructor

```typescript
constructor()
```

Creates a new EventEmitter instance.

### Type Parameters

```typescript
class EventEmitter<TEventMap extends Record<string, any> = Record<string, any>>
```

- `TEventMap` - A TypeScript interface mapping event names to their data types

### Core Methods

#### on()

Register an event listener.

```typescript
on<K extends keyof TEventMap>(
  event: K,
  listener: EventListener<TEventMap[K]>
): this
```

**Parameters:**
- `event` - Event type to listen for
- `listener` - Callback function to execute when event is emitted

**Returns:** This instance for chaining

**Example:**
```typescript
client.on('text_delta', (event) => {
  console.log('Received text:', event.content);
});

client.on('error', (event) => {
  console.error('Error occurred:', event.message);
});
```

#### once()

Register a one-time event listener that automatically removes itself after firing.

```typescript
once<K extends keyof TEventMap>(
  event: K,
  listener: EventListener<TEventMap[K]>
): this
```

**Parameters:**
- `event` - Event type to listen for
- `listener` - Callback function to execute once

**Returns:** This instance for chaining

**Example:**
```typescript
client.once('connected', () => {
  console.log('Initial connection established');
});
```

#### off()

Remove an event listener.

```typescript
off<K extends keyof TEventMap>(
  event: K,
  listener: EventListener<TEventMap[K]>
): this
```

**Parameters:**
- `event` - Event type to remove listener from
- `listener` - Specific listener to remove

**Returns:** This instance for chaining

**Example:**
```typescript
const handler = (event: TextDeltaEvent) => {
  console.log(event.content);
};

client.on('text_delta', handler);
// Later...
client.off('text_delta', handler);
```

#### emit()

Emit an event to all registered listeners.

```typescript
emit<K extends keyof TEventMap>(
  event: K,
  data: TEventMap[K]
): boolean
```

**Parameters:**
- `event` - Event type to emit
- `data` - Event data to pass to listeners

**Returns:** `true` if event had listeners, `false` otherwise

**Example:**
```typescript
const hadListeners = client.emit('connected', undefined);
if (!hadListeners) {
  console.log('No listeners registered for connected event');
}
```

### Listener Management Methods

#### removeAllListeners()

Remove all listeners for a specific event or all events.

```typescript
removeAllListeners<K extends keyof TEventMap>(event?: K): this
```

**Parameters:**
- `event` - Optional event type to remove listeners for

**Returns:** This instance for chaining

**Example:**
```typescript
// Remove all listeners for 'error' event
client.removeAllListeners('error');

// Remove all listeners for all events
client.removeAllListeners();
```

#### listenerCount()

Get the number of listeners for a specific event.

```typescript
listenerCount<K extends keyof TEventMap>(event: K): number
```

**Parameters:**
- `event` - Event type to count listeners for

**Returns:** Number of listeners

**Example:**
```typescript
const count = client.listenerCount('text_delta');
console.log(`${count} listeners for text_delta`);
```

#### eventNames()

Get all registered event names.

```typescript
eventNames(): (keyof TEventMap)[]
```

**Returns:** Array of event names

**Example:**
```typescript
const events = client.eventNames();
console.log('Registered events:', events);
```

#### setMaxListeners() / getMaxListeners()

Control the maximum number of listeners per event (memory leak protection).

```typescript
setMaxListeners(n: number): this
getMaxListeners(): number
```

**Parameters:**
- `n` - Maximum number of listeners (0 for unlimited)

**Example:**
```typescript
client.setMaxListeners(20); // Increase limit
const max = client.getMaxListeners(); // Get current limit
```

### Advanced Methods

#### prependListener()

Add a listener to the beginning of the listeners array.

```typescript
prependListener<K extends keyof TEventMap>(
  event: K,
  listener: EventListener<TEventMap[K]>
): this
```

**Example:**
```typescript
// This listener will be called first
client.prependListener('text_delta', (event) => {
  console.log('Priority handler:', event);
});
```

#### prependOnceListener()

Add a one-time listener to the beginning of the listeners array.

```typescript
prependOnceListener<K extends keyof TEventMap>(
  event: K,
  listener: EventListener<TEventMap[K]>
): this
```

#### rawListeners()

Get raw listener functions for an event.

```typescript
rawListeners<K extends keyof TEventMap>(
  event: K
): EventListener<TEventMap[K]>[]
```

**Returns:** Array of listener functions

## EventRegistry Class

The `EventRegistry` class provides event type validation, registration, and utility functions for working with Agent C events.

### Static Methods

#### isClientEventType()

Check if a string is a valid client event type.

```typescript
static isClientEventType(type: string): type is ClientEventType
```

**Parameters:**
- `type` - String to validate

**Returns:** Type predicate for TypeScript type narrowing

**Example:**
```typescript
if (EventRegistry.isClientEventType(eventType)) {
  // TypeScript knows eventType is ClientEventType here
  client.send(eventType, data);
}
```

#### isServerEventType()

Check if a string is a valid server event type.

```typescript
static isServerEventType(type: string): type is ServerEventType
```

#### isValidEventType()

Check if a string is any valid event type.

```typescript
static isValidEventType(type: string): type is EventType
```

**Example:**
```typescript
const eventType = 'text_delta';
if (EventRegistry.isValidEventType(eventType)) {
  console.log('Valid event type');
}
```

#### getClientEventTypes() / getServerEventTypes()

Get lists of all registered event types.

```typescript
static getClientEventTypes(): ClientEventType[]
static getServerEventTypes(): ServerEventType[]
```

**Example:**
```typescript
const clientEvents = EventRegistry.getClientEventTypes();
console.log('Available client events:', clientEvents);
```

#### parseEvent()

Parse an event from JSON string or object.

```typescript
static parseEvent(data: string | any): ClientEvent | ServerEvent | null
```

**Parameters:**
- `data` - JSON string or object to parse

**Returns:** Parsed event or null if invalid

**Example:**
```typescript
const jsonString = '{"type":"text_delta","content":"Hello"}';
const event = EventRegistry.parseEvent(jsonString);
if (event) {
  console.log('Parsed event type:', event.type);
}
```

#### createEvent()

Create a typed event object with type safety.

```typescript
static createEvent<T extends EventType>(
  type: T,
  data: Omit<EventData<T>, 'type'>
): EventData<T>
```

**Parameters:**
- `type` - Event type
- `data` - Event data (without type field)

**Returns:** Complete typed event object

**Example:**
```typescript
const textEvent = EventRegistry.createEvent('text_input', {
  text: 'Hello, Agent!',
  timestamp: new Date().toISOString()
});
```

## Event Type Mappings

The SDK provides comprehensive type mappings for all events:

### RealtimeEventMap

Complete event map including all client, server, and system events.

```typescript
interface RealtimeEventMap extends ClientEventMap, ServerEventMap {
  // Binary and connection events
  'binary_audio': ArrayBuffer;      // Legacy audio event
  'audio:output': ArrayBuffer;      // Binary audio output
  'connected': void;
  'disconnected': { code: number; reason: string };
  'reconnecting': { attempt: number; delay: number };
  'reconnected': void;
  'ping': any;
  'pong': any;
}
```

### Client Event Types

Events sent from client to server:

- `get_agents` - Request list of available agents
- `set_agent` - Select an agent
- `text_input` - Send text message
- `new_chat_session` - Start new chat session
- `resume_chat_session` - Resume existing session
- `set_chat_session_name` - Update session name
- `get_voices` - Request available voices
- `set_agent_voice` - Set agent voice
- `ping` - Keepalive ping
- `client_wants_cancel` - Cancel current operation

### Server Event Types

Events sent from server to client:

- `agent_list` - List of available agents
- `text_delta` - Incremental text response
- `thought_delta` - Agent thinking updates
- `completion` - Response completion
- `user_turn_start` - User's turn to speak
- `user_turn_end` - User's turn ended
- `error` - Error notification
- `tool_call` - Tool execution notification
- `chat_session_changed` - Session state update
- `cancelled` - Operation cancelled

## Usage Patterns

### Creating Typed Event Emitters

Extend EventEmitter with your own event maps:

```typescript
interface MyCustomEvents {
  'custom:start': { id: string; timestamp: Date };
  'custom:progress': { percent: number };
  'custom:complete': { result: any };
}

class CustomManager extends EventEmitter<MyCustomEvents> {
  startProcess(id: string) {
    this.emit('custom:start', { 
      id, 
      timestamp: new Date() 
    });
  }
}

const manager = new CustomManager();
manager.on('custom:progress', (event) => {
  // TypeScript knows event.percent exists
  console.log(`Progress: ${event.percent}%`);
});
```

### Error Handling in Listeners

The EventEmitter automatically catches and logs errors in listeners:

```typescript
client.on('text_delta', (event) => {
  // If this throws, it won't crash the application
  throw new Error('Handler error');
  // Error is logged: "Error in event listener for "text_delta": Error: Handler error"
});

// Other listeners still execute
client.on('text_delta', (event) => {
  console.log('This still runs');
});
```

### Memory Leak Protection

The EventEmitter warns about potential memory leaks:

```typescript
// Default max listeners is 10
for (let i = 0; i < 15; i++) {
  client.on('error', () => {});
}
// Console warning: "Warning: Possible EventEmitter memory leak detected..."

// Increase limit if intentional
client.setMaxListeners(20);
```

### Event Validation

Use EventRegistry for runtime validation:

```typescript
function handleUnknownEvent(eventData: any) {
  const event = EventRegistry.parseEvent(eventData);
  
  if (!event) {
    console.error('Invalid event data');
    return;
  }

  if (EventRegistry.isServerEventType(event.type)) {
    // Handle server event
    processServerEvent(event);
  } else if (EventRegistry.isClientEventType(event.type)) {
    // Handle client event
    processClientEvent(event);
  }
}
```

## Integration with SDK Components

The event system is used throughout the SDK:

### RealtimeClient

The main client extends EventEmitter with the complete event map:

```typescript
export class RealtimeClient extends EventEmitter<RealtimeEventMap> {
  // All events are strongly typed
}
```

### Manager Classes

All manager classes extend EventEmitter with their specific events:

```typescript
// AuthManager events
interface AuthManagerEvents {
  'auth:tokens-refreshed': TokenPair;
  'auth:tokens-expired': void;
  'auth:error': { error: Error };
}

// SessionManager events  
interface SessionManagerEventMap {
  'message': EnhancedMessage;
  'messages:updated': EnhancedMessage[];
  'session:cleared': void;
}

// TurnManager events
interface TurnManagerEventMap {
  'turn:changed': TurnState;
  'turn:user-start': void;
  'turn:user-end': void;
}
```

### React Hooks

React hooks subscribe to events and clean up automatically:

```typescript
useEffect(() => {
  const handleTextDelta = (event: TextDeltaEvent) => {
    // Handle event
  };

  client.on('text_delta', handleTextDelta);
  
  return () => {
    client.off('text_delta', handleTextDelta);
  };
}, [client]);
```

## Best Practices

### 1. Always Remove Listeners

Prevent memory leaks by removing listeners when done:

```typescript
// In React components
useEffect(() => {
  const handler = (event) => { /* ... */ };
  client.on('event', handler);
  return () => client.off('event', handler);
}, []);

// In class components
componentDidMount() {
  this.client.on('event', this.handleEvent);
}

componentWillUnmount() {
  this.client.off('event', this.handleEvent);
}
```

### 2. Use Type-Safe Event Maps

Define event maps for type safety:

```typescript
interface MyEventMap {
  'data': { value: number };
  'error': Error;
}

const emitter = new EventEmitter<MyEventMap>();
// TypeScript enforces correct event data types
```

### 3. Handle Errors Gracefully

Listeners should handle their own errors:

```typescript
client.on('data', (event) => {
  try {
    processData(event);
  } catch (error) {
    logger.error('Failed to process data:', error);
    // Recover or propagate as needed
  }
});
```

### 4. Use Once for Initialization

Use `once()` for one-time setup:

```typescript
client.once('initialized', () => {
  // Run setup code once
  setupUI();
});
```

### 5. Validate Unknown Events

Always validate events from external sources:

```typescript
websocket.onmessage = (message) => {
  const event = EventRegistry.parseEvent(message.data);
  if (event && EventRegistry.isValidEventType(event.type)) {
    client.emit(event.type, event);
  }
};
```

## Error Handling

The event system includes built-in error isolation:

- **Listener errors are caught** and logged, preventing cascade failures
- **Invalid events are rejected** by EventRegistry validation
- **Memory leak warnings** alert to excessive listeners
- **Type safety** prevents runtime type errors

```typescript
// Errors in one listener don't affect others
client.on('error', (e) => {
  throw new Error('Handler 1 error');
});

client.on('error', (e) => {
  // This still executes
  console.log('Handler 2 received error:', e);
});
```

## Performance Considerations

- **Listener arrays are copied** before iteration to prevent modification issues
- **One-time listeners** are removed before execution for efficiency
- **Event validation** is optimized using Sets for O(1) lookups
- **Type predicates** enable TypeScript compiler optimizations

## Migration Guide

If migrating from Node.js EventEmitter:

1. **Add type parameters** for type safety
2. **Use typed event maps** instead of string literals
3. **Replace `addListener`** with `on` (though aliases exist)
4. **Update error handling** to use try-catch in listeners

```typescript
// Before (Node.js EventEmitter)
emitter.on('data', (data) => { /* untyped */ });

// After (SDK EventEmitter)  
emitter.on('data', (data: DataEvent) => { /* typed */ });
```

## See Also

- [RealtimeClient API Reference](./RealtimeClient.md) - Main client using the event system
- [Authentication Guide](../guides/authentication.md) - Auth events and handling
- [Audio System](./AudioSystem.md) - Audio-specific events
- [React Hooks](../react/hooks.md) - React integration with events
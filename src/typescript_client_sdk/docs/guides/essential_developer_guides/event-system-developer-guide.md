# Event System Developer Guide - Agent C Realtime SDK Core Package

> **Purpose**: This guide provides a comprehensive roadmap for developers working with the event system in the `@agentc/realtime-core` package. Whether you're integrating with the core from React/UI packages or debugging event flow, this guide will help you understand where things are and what they do.

## Table of Contents
1. [Quick Start - Essential Concepts](#quick-start---essential-concepts)
2. [Architecture Overview](#architecture-overview)
3. [Event Type System](#event-type-system)
4. [Event Flow & Processing](#event-flow--processing)
5. [Core Components Guide](#core-components-guide)
6. [Integration Patterns](#integration-patterns)
7. [Common Use Cases](#common-use-cases)
8. [Debugging & Troubleshooting](#debugging--troubleshooting)
9. [API Reference Quick Links](#api-reference-quick-links)

---

## Quick Start - Essential Concepts

### What You Need to Know First

The event system is the **central nervous system** of the realtime SDK. Every interaction, message, and state change flows through events. Here's what makes it tick:

1. **Everything is an Event**: User input, agent responses, tool calls, errors - all are events
2. **Type-Safe by Design**: Full TypeScript typing with discriminated unions
3. **Bidirectional Flow**: Client→Server and Server→Client events with clear separation
4. **Session Context**: Events can be global (Control) or session-specific (SessionEvents)

### The 30-Second Tour

```typescript
// The flow you'll see everywhere:
WebSocket → EventStreamProcessor → Specialized Handlers → UI Components
```

**Key Files to Bookmark:**
- Event Types: `packages/core/src/events/types/`
- Event Processing: `packages/core/src/events/EventStreamProcessor.ts`
- Event Infrastructure: `packages/core/src/events/EventEmitter.ts`
- Integration Point: `packages/core/src/client/RealtimeClient.ts`

---

## Architecture Overview

### System Layers

```
┌─────────────────────────────────────────────────┐
│                  UI Layer                       │
│         (React Components & Hooks)              │
└────────────────────┬────────────────────────────┘
                     │ Events & State
┌────────────────────▼────────────────────────────┐
│              Core Package                       │
│  ┌──────────────────────────────────────────┐   │
│  │        RealtimeClient (Orchestrator)     │   │
│  └────────────────┬─────────────────────────┘   │
│                   │                             │
│  ┌────────────────▼──────────────────────┐      │
│  │    EventStreamProcessor (Router)      │      │
│  └────────────────┬──────────────────────┘      │
│                   │                             │
│  ┌────────────────▼──────────────────────┐      │
│  │   Specialized Handlers & Managers     │      │
│  │  (Message, ToolCall, Session, Audio)  │      │
│  └───────────────────────────────────────┘      │
└──────────────────┬──────────────────────────────┘
                   │ WebSocket
┌──────────────────▼──────────────────────────────┐
│            Agent C Realtime API                 │
└─────────────────────────────────────────────────┘
```

### Data Flow Patterns

#### Incoming Events (Server → Client)
1. **WebSocket** receives raw event data
2. **WebSocketManager** validates and types the event
3. **EventStreamProcessor** routes to appropriate handler
4. **Specialized Handlers** process business logic
5. **EventEmitter** broadcasts to subscribers
6. **UI Components** update based on events

#### Outgoing Events (Client → Server)
1. **UI Action** triggers method call
2. **RealtimeClient** method validates input
3. **WebSocketManager** sends typed event
4. **Server** processes and responds with events

---

## Event Type System

### The Type Hierarchy

```typescript
// Base Types - Everything starts here
interface BaseServerEvent {
  type: string;  // Discriminator field
}

interface BaseClientEvent {
  type: string;  // Discriminator field
}

// Session-Aware Events
interface SessionEvent extends BaseServerEvent {
  session_id: string;        // Current session
  role: string;              // Who triggered it
  parent_session_id?: string; // For sub-sessions
  user_session_id?: string;   // Top-level session
}
```

### Event Categories & Their Purpose

#### Control Events (System Operations)
**Location**: Inherit directly from `BaseServerEvent` or `BaseClientEvent`
**Purpose**: Manage system state outside of chat sessions

- **Session Management**: `new_chat_session`, `resume_chat_session`, `chat_session_changed`
- **Configuration**: `set_agent`, `set_avatar`, `set_agent_voice`
- **Discovery**: `get_agents`, `get_avatars`, `get_voices`, `get_tool_catalog`
- **Health**: `ping`/`pong`
- **Turn Control**: `user_turn_start`, `user_turn_end`

#### Session Events (Chat Operations)
**Location**: Inherit from `SessionEvent`
**Purpose**: Handle content within active chat sessions

- **Content Streaming**: `text_delta`, `thought_delta`, `history_delta`
- **Tool Operations**: `tool_select_delta`, `tool_call`
- **Messages**: `system_message`, `user_message`, `anthropic_user_message`
- **Media**: `render_media`
- **Flow Control**: `completion`, `interaction`, `subsession_started`

### Type Definitions Location

```
packages/core/src/events/types/
├── ClientEvents.ts    # Events client sends to server
├── ServerEvents.ts    # Events server sends to client
└── CommonTypes.ts     # Shared types (Message, ToolCall, etc.)
```

### Naming Convention Rules

**CRITICAL**: Event types are **snake_case WITHOUT "event" suffix**

```typescript
// Class Name → Event Type String
TextDeltaEvent      → "text_delta"
ToolCallEvent       → "tool_call"  
SystemMessageEvent  → "system_message"
UserTurnStartEvent  → "user_turn_start"
```

---

## Event Flow & Processing

### The EventStreamProcessor - Your Central Router

**Location**: `packages/core/src/events/EventStreamProcessor.ts`

This is the **brain** of event processing. Every server event flows through here.

#### What It Does:
1. **Routes Events** to appropriate handlers based on type
2. **Manages State** for message building and tool calls
3. **Coordinates** between different subsystems
4. **Normalizes** data for UI consumption

#### Key Methods:

```typescript
class EventStreamProcessor {
  // Main entry point - all events come here
  processEvent(event: ServerEvent): void
  
  // State management
  setUserSessionId(id: string): void
  
  // Special processors for complex flows
  private handleTextDelta(event: TextDeltaEvent): void
  private handleToolCall(event: ToolCallEvent): void
  private handleChatSessionChanged(event: ChatSessionChangedEvent): void
  
  // Resume session handling
  private mapResumedMessagesToEvents(messages: MessageParam[], sessionId: string): void
}
```

### Event Processing Patterns

#### Pattern 1: Streaming Text Assembly
```typescript
// Multiple text_delta events → Single complete message
text_delta → MessageBuilder.appendText() → message-streaming event
text_delta → MessageBuilder.appendText() → message-streaming event
completion → MessageBuilder.finalize() → message-complete event
```

#### Pattern 2: Tool Execution Lifecycle
```typescript
// Tool selection → execution → completion
tool_select_delta → ToolCallManager → tool-notification event
tool_call (active: true) → ToolCallManager → tool-notification event
tool_call (active: false) → ToolCallManager → tool-call-complete event
```

#### Pattern 3: Session Resume
```typescript
// Loading existing session converts messages to events
chat_session_changed → mapResumedMessagesToEvents() → session-messages-loaded event
```

### Supporting Components

#### MessageBuilder
**Location**: `packages/core/src/events/MessageBuilder.ts`
**Purpose**: Assembles streaming deltas into complete messages

```typescript
class MessageBuilder {
  startMessage(role: 'assistant' | 'thought'): void
  appendText(text: string): void
  finalize(metadata?: MessageMetadata): Message
}
```

#### ToolCallManager
**Location**: `packages/core/src/events/ToolCallManager.ts`
**Purpose**: Tracks tool execution lifecycle

```typescript
class ToolCallManager {
  onToolSelect(event: ToolSelectDeltaEvent): ToolNotification
  onToolCallActive(event: ToolCallEvent): ToolNotification | null
  onToolCallComplete(event: ToolCallEvent): void
  getCompletedToolCalls(): CompletedToolCall[]
}
```

---

## Core Components Guide

### RealtimeClient - The Orchestrator

**Location**: `packages/core/src/client/RealtimeClient.ts`

This is your **main entry point** for all SDK operations.

#### Key Responsibilities:
- Manages WebSocket connection
- Coordinates all subsystems
- Provides public API methods
- Handles initialization sequence

#### Important Methods:
```typescript
// Connection
async connect(params?: ConnectionParams): Promise<void>
disconnect(): void

// Chat Operations  
sendTextInput(text: string): Promise<void>
createNewSession(agentConfig?: AgentConfig): Promise<void>
resumeSession(sessionId: string): Promise<void>

// Event Subscription
on(event: string, handler: Function): void
off(event: string, handler: Function): void
```

### WebSocketManager - The Transport Layer

**Location**: `packages/core/src/client/WebSocketManager.ts`

Handles the raw WebSocket connection and protocol.

#### Key Features:
- Automatic reconnection with exponential backoff
- Event validation and typing
- Connection state management
- Binary/text message handling

### EventEmitter - The Broadcasting System

**Location**: `packages/core/src/events/EventEmitter.ts`

Type-safe event emitter used throughout the SDK.

#### Features:
- Full TypeScript generics support
- Memory leak detection
- One-time listeners
- Event namespacing

```typescript
class EventEmitter<TEventMap> {
  on<K extends keyof TEventMap>(event: K, listener: (data: TEventMap[K]) => void): this
  emit<K extends keyof TEventMap>(event: K, data: TEventMap[K]): boolean
}
```

---

## Integration Patterns

### Pattern 1: Subscribing to Events from React

```typescript
// In a React component/hook
useEffect(() => {
  const handleMessage = (event: MessageStreamingEvent) => {
    // Update UI state
    setCurrentMessage(event.message);
  };
  
  realtimeClient.on('message-streaming', handleMessage);
  
  return () => {
    realtimeClient.off('message-streaming', handleMessage);
  };
}, [realtimeClient]);
```

### Pattern 2: Processing Event Sequences

```typescript
// Handle initialization sequence
const initSequence = [
  'chat_user_data',
  'avatar_list',
  'voice_list',
  'agent_list', 
  'tool_catalog',
  'chat_session_changed'
];

let received = new Set();
client.on('*', (event) => {
  if (initSequence.includes(event.type)) {
    received.add(event.type);
    if (received.size === initSequence.length) {
      // Initialization complete
    }
  }
});
```

### Pattern 3: Custom Event Processing

```typescript
// Extend EventStreamProcessor for custom logic
class CustomEventProcessor extends EventStreamProcessor {
  processEvent(event: ServerEvent): void {
    // Custom pre-processing
    if (event.type === 'custom_event') {
      this.handleCustomEvent(event);
      return;
    }
    
    // Delegate to parent
    super.processEvent(event);
  }
}
```

---

## Common Use Cases

### Use Case 1: Display Streaming Assistant Response

**What You Need:**
- Listen to: `message-streaming` events
- Final message: `message-complete` event
- Message structure: `Message` type from CommonTypes

**Where to Hook In:**
```typescript
// React Hook Example
const useStreamingMessage = () => {
  const [message, setMessage] = useState<Message | null>(null);
  
  useEffect(() => {
    client.on('message-streaming', (e) => setMessage(e.message));
    client.on('message-complete', (e) => setMessage(e.message));
  }, []);
  
  return message;
};
```

### Use Case 2: Track Tool Execution

**What You Need:**
- Start notification: `tool-notification` event
- Completion: `tool-call-complete` event
- Removal: `tool-notification-removed` event

**Event Flow:**
1. `tool_select_delta` → UI shows "Selecting tool..."
2. `tool_call` (active: true) → UI shows "Executing tool..."
3. `tool_call` (active: false) → UI shows results

### Use Case 3: Handle Sub-Sessions (Agent Delegation)

**What You Need:**
- Detection: Check `user_session_id` vs `session_id`
- Start: `subsession-started` event
- End: `subsession-ended` event

**Key Logic:**
```typescript
const isSubSession = (event: SessionEvent) => {
  return event.session_id !== event.user_session_id;
};
```

### Use Case 4: Resume Existing Chat

**What You Need:**
- Trigger: `resumeSession(sessionId)` method
- Response: `chat_session_changed` event
- Messages: `session-messages-loaded` event

**Important:** Resumed messages are processed through `mapResumedMessagesToEvents()` to maintain consistency with streamed messages.

---

## Debugging & Troubleshooting

### Enable Debug Logging

```typescript
// In RealtimeClient config
const client = new RealtimeClient({
  debug: true,  // Enables console logging
  // ... other config
});

// Or use the Logger directly
import { Logger } from '@agentc/realtime-core';
Logger.setLevel('debug');
```

### Common Issues & Solutions

#### Issue: Events Not Being Received

**Check These:**
1. WebSocket connection state: `client.getConnectionState()`
2. Event subscription: Ensure handler is registered before events fire
3. Event type: Verify exact event name (snake_case, no "event" suffix)

```typescript
// Debug helper
client.on('*', (event) => {
  console.log('Event received:', event.type, event);
});
```

#### Issue: Messages Appearing Out of Order

**Likely Cause:** Not handling streaming events correctly

**Solution:** Use `message-streaming` for updates, `message-complete` for final

```typescript
// Correct pattern
let streamingMessages = new Map();

client.on('message-streaming', (e) => {
  streamingMessages.set(e.sessionId, e.message);
  updateUI();
});

client.on('message-complete', (e) => {
  streamingMessages.delete(e.sessionId);
  addToHistory(e.message);
});
```

#### Issue: Tool Results Not Displaying

**Check:**
- Tool result processing in EventStreamProcessor
- `tool-call-complete` event has `toolResults` field
- RenderMedia events for rich tool output

#### Issue: Session State Inconsistent

**Verify:**
- SessionManager state: `client.sessionManager.getCurrentSession()`
- Event processing order in EventStreamProcessor
- Sub-session detection logic

### Event Flow Tracing

```typescript
// Trace event flow through the system
class EventTracer {
  static trace(eventType: string) {
    console.group(`Tracing: ${eventType}`);
    
    // 1. WebSocket receive
    console.log('1. WebSocket receives');
    
    // 2. EventStreamProcessor routing
    console.log('2. EventStreamProcessor.processEvent()');
    
    // 3. Handler method
    console.log(`3. handle${this.toPascalCase(eventType)}()`);
    
    // 4. State updates
    console.log('4. Manager/Builder state updates');
    
    // 5. Event emission
    console.log('5. EventEmitter broadcasts');
    
    console.groupEnd();
  }
}
```

### Performance Monitoring

```typescript
// Monitor event processing performance
const perfmon = new Map<string, number>();

client.on('*', (event) => {
  const start = performance.now();
  
  // Let event process
  setTimeout(() => {
    const duration = performance.now() - start;
    const times = perfmon.get(event.type) || [];
    times.push(duration);
    perfmon.set(event.type, times);
    
    // Log slow events
    if (duration > 100) {
      console.warn(`Slow event: ${event.type} took ${duration}ms`);
    }
  }, 0);
});
```

---

## API Reference Quick Links

### Essential Documentation

**Package Documentation:**
- Core Package: `//realtime_client/docs/api-reference/core/index.md`
- Event Types: `//realtime_client/docs/api-reference/core/events/types/`
- EventStreamProcessor: `//realtime_client/docs/api-reference/core/events/EventStreamProcessor.md`

**Implementation Guide:**
- Full API Guide: `//api/docs/realtime_api_implementation_guide.md`
- Event Index: Search for "Event Types" section

### Type Definitions to Keep Open

```typescript
// Most important types you'll reference
import {
  ServerEvent,
  ClientEvent,
  SessionEvent,
  Message,
  ToolCall,
  ChatSession
} from '@agentc/realtime-core';
```

### Event Type Quick Reference

**High-Frequency Events** (you'll see these constantly):
- `text_delta` - Streaming assistant text
- `tool_call` - Tool execution
- `message-streaming` - UI update event
- `completion` - Response complete

**Session Management Events:**
- `chat_session_changed` - Session loaded/switched
- `user_turn_start` - Ready for input
- `user_turn_end` - Input received

**System Events:**
- `error` - Error occurred
- `system_message` - System notification
- `initialized` - Client ready

---

## Summary Cheat Sheet

### File System Map
```
packages/core/src/
├── events/                       # EVENT SYSTEM CORE
│   ├── EventStreamProcessor.ts   # ⭐ Main router
│   ├── EventEmitter.ts          # Broadcasting
│   ├── MessageBuilder.ts        # Message assembly
│   ├── ToolCallManager.ts       # Tool tracking
│   └── types/                   # ⭐ Event definitions
│       ├── ClientEvents.ts     
│       ├── ServerEvents.ts     
│       └── CommonTypes.ts      
├── client/                      # CLIENT INFRASTRUCTURE  
│   ├── RealtimeClient.ts       # ⭐ Main entry point
│   └── WebSocketManager.ts     # Transport layer
└── session/                     # STATE MANAGEMENT
    └── SessionManager.ts        # Session state
```

### Quick Wins for New Developers

1. **Start with EventStreamProcessor** - See how events are routed
2. **Read the event types** - Understand what data flows through
3. **Trace one event end-to-end** - Pick `text_delta` and follow it
4. **Use debug mode** - `debug: true` in client config
5. **Check the tests** - `__tests__` folders have great examples

### Golden Rules

1. **Never modify event field names** - They match the API contract
2. **Events are immutable** - Don't modify events after creation  
3. **Use the type system** - TypeScript will catch most issues
4. **Session context matters** - Check `session_id` fields
5. **Deltas build messages** - Messages are assembled, not sent whole

---

*This guide is a living document. As the event system evolves, update this guide to help your fellow developers navigate the codebase effectively.*
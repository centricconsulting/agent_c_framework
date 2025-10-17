# WebSocket Layer Developer Guide - Agent C Realtime SDK

> **Purpose**: This guide provides comprehensive documentation for the WebSocket layer in the `@agentc/realtime-core` package. As this layer is tightly coupled with event processing, understanding both systems together is crucial for effective development and debugging.

## Table of Contents
1. [Overview - The Transport Foundation](#overview---the-transport-foundation)
2. [Architecture & Components](#architecture--components)
3. [WebSocket Protocol & Message Types](#websocket-protocol--message-types)
4. [Connection Lifecycle Management](#connection-lifecycle-management)
5. [Reconnection Strategy](#reconnection-strategy)
6. [Message Flow & Processing](#message-flow--processing)
7. [Binary Audio Handling](#binary-audio-handling)
8. [Error Handling & Recovery](#error-handling--recovery)
9. [Configuration & Optimization](#configuration--optimization)
10. [Integration with Event System](#integration-with-event-system)
11. [Debugging & Monitoring](#debugging--monitoring)
12. [Common Patterns & Best Practices](#common-patterns--best-practices)

---

## Overview - The Transport Foundation

The WebSocket layer is the **critical transport infrastructure** that enables real-time bidirectional communication between the client and the Agent C Realtime API. It handles:

- **Connection Management**: Establishing, maintaining, and closing WebSocket connections
- **Message Transport**: Sending/receiving both JSON events and binary audio data
- **Reliability**: Automatic reconnection with exponential backoff
- **Health Monitoring**: Ping/pong heartbeat mechanism
- **Protocol Compliance**: Proper WebSocket frame handling and buffering

### Key Design Principles

1. **Separation of Concerns**: Transport layer is isolated from business logic
2. **Reliability First**: Automatic recovery from network failures
3. **Performance Optimized**: Efficient binary audio streaming
4. **Type Safety**: Full TypeScript typing for all messages
5. **Observable State**: Clear connection state tracking

### Quick Reference

```typescript
// The WebSocket flow
RealtimeClient → WebSocketManager → WebSocket API → Agent C Server
                      ↓                    ↑
              ReconnectionManager    Binary/JSON Messages
```

**Critical Files:**
- `packages/core/src/client/WebSocketManager.ts` - Main WebSocket handler
- `packages/core/src/client/ReconnectionManager.ts` - Reconnection logic
- `packages/core/src/client/RealtimeClient.ts` - Integration layer
- `packages/core/src/client/ClientConfig.ts` - Configuration types

---

## Architecture & Components

### Component Hierarchy

```
┌─────────────────────────────────────────────────┐
│                RealtimeClient                   │
│         (High-level API & Orchestration)        │
└──────────────────┬──────────────────────────────┘
                   │ Manages
┌──────────────────▼──────────────────────────────┐
│             WebSocketManager                    │
│      (WebSocket lifecycle & messaging)          │
│                                                  │
│  • Connection establishment                      │
│  • Message send/receive                          │
│  • Binary frame handling                         │
│  • Heartbeat mechanism                           │
└──────────────────┬──────────────────────────────┘
                   │ Uses
┌──────────────────▼──────────────────────────────┐
│           ReconnectionManager                   │
│    (Automatic reconnection strategy)            │
│                                                  │
│  • Exponential backoff                          │
│  • Jitter for distributed systems               │
│  • Configurable retry limits                    │
└──────────────────────────────────────────────────┘
```

### WebSocketManager - Core Transport

**Location**: `packages/core/src/client/WebSocketManager.ts`

The WebSocketManager is responsible for:

1. **WebSocket Lifecycle**: Creating, configuring, and destroying WebSocket instances
2. **Message Handling**: Routing text (JSON) and binary (audio) messages
3. **Connection Health**: Ping/pong heartbeat to detect stale connections
4. **State Management**: Tracking connection readiness

#### Key Methods:

```typescript
class WebSocketManager {
  // Lifecycle
  connect(): void                              // Establish connection
  disconnect(code?, reason?): void             // Close connection
  
  // Messaging
  send(data: string | ArrayBuffer): void       // Send raw data
  sendJSON(data: unknown): void                // Send JSON object
  sendBinary(data: ArrayBuffer): void          // Send binary frame
  
  // State
  isConnected(): boolean                       // Check connection status
  getReadyState(): number                      // Get WebSocket state
  supportsBinary(): boolean                    // Check binary support
  
  // Buffering
  getBufferedAmount(): number                  // Check send buffer
  hasBufferedData(): boolean                   // Quick buffer check
}
```

### ReconnectionManager - Reliability Layer

**Location**: `packages/core/src/client/ReconnectionManager.ts`

Implements sophisticated reconnection strategy:

1. **Exponential Backoff**: Increasing delays between attempts
2. **Jitter**: Randomization to prevent thundering herd
3. **Max Attempts**: Configurable failure threshold
4. **Event-Driven**: Emits events for UI feedback

#### Reconnection Algorithm:

```typescript
// Exponential backoff with jitter
delay = min(initialDelay * (backoffMultiplier ^ attempt), maxDelay)
jitteredDelay = delay ± (delay * jitterFactor)

// Default configuration
{
  initialDelay: 1000ms      // Start with 1 second
  backoffMultiplier: 1.5    // 1.5x increase each time
  maxDelay: 30000ms         // Cap at 30 seconds
  jitterFactor: 0.3         // ±30% randomization
  maxAttempts: 0            // Unlimited (0 = infinite)
}
```

---

## WebSocket Protocol & Message Types

### Message Format Overview

The Agent C Realtime API uses two distinct message types over WebSocket:

1. **Text Frames**: JSON-encoded events
2. **Binary Frames**: Raw PCM16 audio data

### Text Frame Protocol (JSON Events)

All non-audio communication uses JSON text frames:

```typescript
// Client → Server Events
interface ClientEvent {
  type: string;        // Event type (snake_case)
  [key: string]: any;  // Event-specific fields
}

// Server → Client Events  
interface ServerEvent {
  type: string;        // Event type (snake_case)
  [key: string]: any;  // Event-specific fields
}
```

#### Common Client Events:
```typescript
// Text input
{ type: 'text_input', text: string }

// Cancel current response
{ type: 'client_wants_cancel' }

// Session management
{ type: 'new_chat_session', agent_config?: AgentConfig }
{ type: 'resume_chat_session', session_id: string }

// Configuration
{ type: 'set_agent', agent_key: string }
{ type: 'set_avatar', avatar_id: string }
{ type: 'set_agent_voice', voice_id: string }

// Health check
{ type: 'ping' }
{ type: 'pong' }
```

#### Common Server Events:
```typescript
// Content streaming
{ type: 'text_delta', content: string, session_id: string }
{ type: 'tool_call', tool_calls: ToolCall[], active: boolean }
{ type: 'completion', running: boolean, stop_reason?: string }

// Session management  
{ type: 'chat_session_changed', chat_session: ChatSession }
{ type: 'user_turn_start' }
{ type: 'user_turn_end' }

// System events
{ type: 'error', message: string, source: string }
{ type: 'cancelled' }
```

### Binary Frame Protocol (Audio)

Binary frames contain raw audio data without JSON wrapping:

```
[Binary Frame] = Raw PCM16 audio data at 16kHz or 24kHz
                 No headers, no metadata, just audio samples
```

**Important**: Binary frames are NOT wrapped in events - they're raw audio

---

## Connection Lifecycle Management

### Connection States

```typescript
enum ConnectionState {
  DISCONNECTED = 'DISCONNECTED',   // Not connected
  CONNECTING = 'CONNECTING',       // Establishing connection
  CONNECTED = 'CONNECTED',         // Active connection
  RECONNECTING = 'RECONNECTING'    // Lost connection, retrying
}
```

### Connection Flow

#### 1. Initial Connection

```typescript
// Connection establishment sequence
async connect(params?: ConnectionParams): Promise<void> {
  1. Build WebSocket URL with auth token
  2. Create WebSocketManager instance
  3. Attach event handlers
  4. Call wsManager.connect()
  5. Wait for 'open' event or timeout
  6. Server sends 6 initialization events
  7. Mark as initialized when all received
}
```

#### 2. Initialization Sequence

The server sends exactly 6 events upon connection:

```typescript
// Required initialization events (in order)
1. chat_user_data     // User information
2. avatar_list        // Available avatars
3. voice_list         // Available voices  
4. agent_list         // Available agents
5. tool_catalog       // Available tools
6. chat_session_changed // Current/new session

// Client tracks receipt
initializationState: Set<string> = new Set();

// Emit 'initialized' when complete
if (initializationState.size === 6) {
  emit('initialized');
}
```

#### 3. Disconnection Handling

```typescript
disconnect(): void {
  1. Stop reconnection attempts
  2. Stop audio streaming if active
  3. Clear buffers and state
  4. Close WebSocket connection
  5. Clean up event handlers
  6. Set state to DISCONNECTED
}
```

### Connection URL Building

```typescript
// URL construction with authentication
const buildUrl = (apiUrl: string, token: string, sessionId?: string) => {
  const url = new URL(apiUrl);
  
  // Add auth token as query parameter
  url.searchParams.set('token', token);
  
  // Add session ID if resuming
  if (sessionId) {
    url.searchParams.set('session_id', sessionId);
  }
  
  return url.toString();
};
```

---

## Reconnection Strategy

### Automatic Reconnection Flow

```
Connection Lost → Check if auto-reconnect enabled → Start reconnection
      ↓                                                    ↓
  Emit 'disconnected'                            Calculate delay with jitter
                                                          ↓
                                                    Wait for delay
                                                          ↓
                                                   Attempt connection
                                                    ↙            ↘
                                              Success          Failure
                                                ↓                 ↓
                                         Emit 'reconnected'   Increment attempt
                                                              Check max attempts
                                                               ↙         ↘
                                                         Continue    Give up
```

### Configuration Options

```typescript
interface ReconnectionConfig {
  enabled: boolean;           // Enable auto-reconnect (default: true)
  initialDelay: number;       // First retry delay (default: 1000ms)
  maxDelay: number;          // Maximum delay (default: 30000ms)
  backoffMultiplier: number; // Delay multiplier (default: 1.5)
  maxAttempts: number;       // Max retries, 0=unlimited (default: 0)
  jitterFactor: number;      // Randomization 0-1 (default: 0.3)
}
```

### Reconnection Events

```typescript
// Reconnection lifecycle events
client.on('reconnecting', ({ attempt, delay }) => {
  // Update UI to show reconnection attempt
  console.log(`Reconnecting... Attempt ${attempt} in ${delay}ms`);
});

client.on('reconnected', () => {
  // Connection restored
  console.log('Successfully reconnected');
});

client.on('reconnection_failed', ({ attempts, reason }) => {
  // Maximum attempts reached
  console.error(`Failed after ${attempts} attempts: ${reason}`);
});
```

---

## Message Flow & Processing

### Outgoing Messages (Client → Server)

```typescript
// Message sending flow
sendEvent(event) → Validate connection → wsManager.sendJSON(event)
                                              ↓
                                     JSON.stringify(event)
                                              ↓
                                     WebSocket.send(jsonString)
```

#### Sending Text Events:
```typescript
// High-level API
client.sendTextInput('Hello, assistant');

// Internally becomes
wsManager.sendJSON({
  type: 'text_input',
  text: 'Hello, assistant'
});
```

#### Sending Binary Audio:
```typescript
// Direct binary send (NOT wrapped in JSON)
client.sendBinaryFrame(audioBuffer);

// Internally
wsManager.sendBinary(audioBuffer);  // Raw PCM16 data
```

### Incoming Messages (Server → Client)

```typescript
// Message reception flow
WebSocket.onmessage → Detect type → Route to handler
         ↓                ↓                ↓
    MessageEvent    Text or Binary    Process accordingly
```

#### Processing Pipeline:

```typescript
handleMessage(data: string | ArrayBuffer) {
  if (typeof data === 'string') {
    // JSON event
    const event = JSON.parse(data);
    
    // Special handling for certain events
    if (eventStreamProcessor.canHandle(event.type)) {
      eventStreamProcessor.processEvent(event);
    } else {
      // Direct emission for control events
      emit(event.type, event);
    }
  } else {
    // Binary audio frame
    emit('audio:output', data);
  }
}
```

### Critical Message Timing

#### Cancel Events - Special Handling:
```typescript
// Cancel events need immediate transmission
sendCancel() {
  // Check buffer before sending
  if (wsManager.getBufferedAmount() > 0) {
    console.warn('WebSocket buffer not empty - cancel may be delayed');
  }
  
  // Send with high priority
  wsManager.sendJSON({ type: 'client_wants_cancel' });
  
  // Log buffer after send for debugging
  console.debug(`Buffer after cancel: ${wsManager.getBufferedAmount()} bytes`);
}
```

---

## Binary Audio Handling

### Audio Streaming Architecture

```
Microphone → AudioService → AudioAgentCBridge → WebSocketManager → Server
                                ↓                      ↑
                          Chunking (4800 samples)  Binary Frames
```

### PCM16 Audio Format

```typescript
// Audio specifications
{
  format: 'pcm16',          // 16-bit PCM
  sampleRate: 24000,        // 24kHz (or 16kHz)
  channels: 1,              // Mono
  chunkSize: 4800,          // Samples per chunk
  bytesPerSample: 2,        // 16-bit = 2 bytes
  chunkBytes: 9600          // 4800 * 2
}
```

### Sending Audio Chunks

```typescript
// Audio chunking and sending
class AudioAgentCBridge {
  private processAudioChunk(chunk: Float32Array) {
    // Convert Float32 to PCM16
    const pcm16 = this.convertToPCM16(chunk);
    
    // Send as binary frame
    this.client.sendBinaryFrame(pcm16.buffer);
    
    // Track bytes sent
    this.bytesSent += pcm16.byteLength;
  }
  
  private convertToPCM16(float32: Float32Array): Int16Array {
    const int16 = new Int16Array(float32.length);
    for (let i = 0; i < float32.length; i++) {
      // Clamp to [-1, 1] and scale to Int16 range
      const clamped = Math.max(-1, Math.min(1, float32[i]));
      int16[i] = Math.round(clamped * 32767);
    }
    return int16;
  }
}
```

### Receiving Audio

```typescript
// Audio reception handling
client.on('audio:output', (audioBuffer: ArrayBuffer) => {
  // Route to audio output service
  audioOutputService.processAudioChunk(audioBuffer);
});
```

---

## Error Handling & Recovery

### Error Categories

#### 1. Connection Errors
```typescript
// Connection failures
try {
  await client.connect();
} catch (error) {
  if (error.message === 'Connection timeout') {
    // Handle timeout
  } else if (error.message === 'Authentication failed') {
    // Handle auth failure
  }
}
```

#### 2. Message Errors
```typescript
// Malformed messages
client.on('error', (event) => {
  switch(event.source) {
    case 'websocket':
      // WebSocket-level error
      break;
    case 'message_parser':
      // JSON parsing error
      break;
    case 'event_processor':
      // Event handling error
      break;
  }
});
```

#### 3. Network Errors
```typescript
// Network interruptions
client.on('disconnected', ({ code, reason }) => {
  // WebSocket close codes
  switch(code) {
    case 1000: // Normal closure
      break;
    case 1001: // Going away
      break;
    case 1006: // Abnormal closure (network issue)
      // Will trigger auto-reconnect
      break;
    case 4000: // Custom: Ping timeout
      break;
  }
});
```

### Recovery Strategies

#### Automatic Recovery:
- **Connection Loss**: Auto-reconnect with exponential backoff
- **Ping Timeout**: Close connection and reconnect
- **Auth Token Refresh**: Reconnect with new token

#### Manual Recovery:
```typescript
// Manual reconnection
async function manualReconnect() {
  client.disconnect();
  
  // Wait a moment
  await new Promise(r => setTimeout(r, 1000));
  
  // Reconnect with fresh state
  await client.connect();
}
```

---

## Configuration & Optimization

### Performance Tuning

```typescript
const optimizedConfig: RealtimeClientConfig = {
  // Connection settings
  connectionTimeout: 5000,    // Fail fast on connect
  pingInterval: 30000,        // Keep-alive every 30s
  pongTimeout: 10000,         // Detect stale connection
  
  // Reconnection tuning
  reconnection: {
    enabled: true,
    initialDelay: 500,       // Start fast
    maxDelay: 15000,         // Cap at 15s
    backoffMultiplier: 2,    // Double each time
    maxAttempts: 10,         // Give up after 10
    jitterFactor: 0.3        // 30% randomization
  },
  
  // Buffer management
  maxMessageSize: 10 * 1024 * 1024,  // 10MB max
  
  // Binary optimization
  binaryType: 'arraybuffer'   // More efficient than blob
};
```

### Network Optimization

#### Reducing Latency:
```typescript
// Disable Nagle's algorithm (if supported)
// Send small messages immediately
wsManager.send(JSON.stringify({
  type: 'client_wants_cancel'
}));

// Check buffer to ensure immediate transmission
if (wsManager.hasBufferedData()) {
  console.warn('Message queued behind buffered data');
}
```

#### Managing Bandwidth:
```typescript
// Audio streaming optimization
const audioConfig = {
  sampleRate: 16000,      // Lower sample rate for bandwidth
  chunkSize: 2400,        // Smaller chunks, more frequent
  enableVAD: true         // Voice activity detection
};
```

### Memory Management

```typescript
// Cleanup on disconnect
disconnect() {
  // Clear audio buffers
  audioOutputService.clearBuffers();
  
  // Reset message accumulator
  sessionManager.resetAccumulator();
  
  // Clear event processor state
  eventStreamProcessor.reset();
  
  // Null out references
  this.wsManager = null;
}
```

---

## Integration with Event System

### Event Flow Integration

```
WebSocket Layer → Event Processing → Application Layer
       ↓                ↓                   ↓
  Raw Messages    EventStreamProcessor   UI Updates
```

### Message Routing Logic

```typescript
// In RealtimeClient.handleMessage()
private handleMessage(data: string | ArrayBuffer) {
  if (typeof data === 'string') {
    const event = JSON.parse(data);
    
    // Events processed by EventStreamProcessor
    const streamProcessedEvents = [
      'text_delta',        // Message streaming
      'thought_delta',     // Thought streaming  
      'completion',        // Message completion
      'tool_call',         // Tool execution
      'render_media',      // Media rendering
      'chat_session_changed', // Session updates
      // ... more events
    ];
    
    if (streamProcessedEvents.includes(event.type)) {
      // Route through EventStreamProcessor
      this.eventStreamProcessor.processEvent(event);
      // EventStreamProcessor handles emission
    } else {
      // Direct emission for control events
      this.emit(event.type, event);
    }
  } else {
    // Binary audio - direct emission
    this.emit('audio:output', data);
  }
}
```

### Coordination Points

#### 1. Session Initialization:
```typescript
// WebSocket receives chat_session_changed
// → EventStreamProcessor processes
// → SessionManager updates state
// → UI receives session-loaded event
```

#### 2. Message Streaming:
```typescript
// WebSocket receives text_delta events
// → EventStreamProcessor assembles message
// → MessageBuilder accumulates text
// → Emits message-streaming for UI
```

#### 3. Tool Execution:
```typescript
// WebSocket receives tool events
// → EventStreamProcessor routes to ToolCallManager
// → Emits tool-notification events
// → UI shows tool execution status
```

---

## Debugging & Monitoring

### Debug Logging

```typescript
// Enable debug mode
const client = new RealtimeClient({
  apiUrl: 'wss://api.agentc.ai/rt/ws',
  debug: true  // Enables verbose logging
});

// Debug output includes:
// - Connection state changes
// - Event transmission/reception
// - Reconnection attempts
// - Buffer status for critical events
```

### Connection Monitoring

```typescript
// Monitor connection health
class ConnectionMonitor {
  private client: RealtimeClient;
  private stats = {
    connectTime: 0,
    disconnectCount: 0,
    messagesSent: 0,
    messagesReceived: 0,
    bytesReceived: 0
  };
  
  monitor() {
    // Track connection time
    this.client.on('connected', () => {
      this.stats.connectTime = Date.now();
    });
    
    // Track disconnections
    this.client.on('disconnected', () => {
      this.stats.disconnectCount++;
    });
    
    // Track messages
    const originalSend = this.client.sendEvent.bind(this.client);
    this.client.sendEvent = (event) => {
      this.stats.messagesSent++;
      return originalSend(event);
    };
  }
  
  getUptime() {
    return Date.now() - this.stats.connectTime;
  }
}
```

### Buffer Monitoring

```typescript
// Monitor WebSocket send buffer
function monitorBuffer(wsManager: WebSocketManager) {
  setInterval(() => {
    const buffered = wsManager.getBufferedAmount();
    if (buffered > 0) {
      console.warn(`WebSocket buffer: ${buffered} bytes pending`);
      
      // Alert if buffer is growing
      if (buffered > 50000) {
        console.error('WebSocket buffer critically high!');
      }
    }
  }, 1000);
}
```

### Network Diagnostics

```typescript
// Measure round-trip time
async function measureLatency(client: RealtimeClient): Promise<number> {
  const start = Date.now();
  
  return new Promise((resolve) => {
    const handler = () => {
      const latency = Date.now() - start;
      client.off('pong', handler);
      resolve(latency);
    };
    
    client.on('pong', handler);
    client.sendEvent({ type: 'ping' });
  });
}
```

---

## Common Patterns & Best Practices

### Pattern 1: Graceful Degradation

```typescript
// Handle connection issues gracefully
class ResilientClient {
  private client: RealtimeClient;
  private isOnline = true;
  
  async sendMessage(text: string) {
    if (!this.client.isConnected()) {
      // Queue message for later
      this.queueMessage(text);
      this.showOfflineUI();
      return;
    }
    
    try {
      await this.client.sendTextInput(text);
    } catch (error) {
      // Fallback to queuing
      this.queueMessage(text);
    }
  }
}
```

### Pattern 2: Connection State UI

```typescript
// React hook for connection status
function useConnectionStatus(client: RealtimeClient) {
  const [status, setStatus] = useState('disconnected');
  
  useEffect(() => {
    const handlers = {
      connected: () => setStatus('connected'),
      disconnected: () => setStatus('disconnected'),
      reconnecting: ({ attempt }) => 
        setStatus(`reconnecting (attempt ${attempt})`),
      reconnected: () => setStatus('connected')
    };
    
    // Register all handlers
    Object.entries(handlers).forEach(([event, handler]) => {
      client.on(event, handler);
    });
    
    // Cleanup
    return () => {
      Object.entries(handlers).forEach(([event, handler]) => {
        client.off(event, handler);
      });
    };
  }, [client]);
  
  return status;
}
```

### Pattern 3: Binary Audio Streaming

```typescript
// Efficient audio streaming pattern
class AudioStreamer {
  private buffer: ArrayBuffer[] = [];
  private isStreaming = false;
  
  startStreaming() {
    this.isStreaming = true;
    this.flushBuffer();
  }
  
  queueAudio(chunk: ArrayBuffer) {
    if (this.isStreaming && client.isConnected()) {
      // Send immediately
      client.sendBinaryFrame(chunk);
    } else {
      // Buffer for later
      this.buffer.push(chunk);
    }
  }
  
  private async flushBuffer() {
    while (this.buffer.length > 0) {
      if (!client.isConnected()) {
        // Wait for reconnection
        await client.waitForConnection();
      }
      
      const chunk = this.buffer.shift()!;
      client.sendBinaryFrame(chunk);
      
      // Throttle to prevent overwhelming
      await this.delay(10);
    }
  }
}
```

### Pattern 4: Event Deduplication

```typescript
// Prevent duplicate event processing during reconnection
class EventDeduplicator {
  private processedEvents = new Set<string>();
  
  shouldProcess(event: any): boolean {
    // Generate unique ID for event
    const eventId = `${event.type}_${event.timestamp}_${event.session_id}`;
    
    if (this.processedEvents.has(eventId)) {
      return false;  // Already processed
    }
    
    this.processedEvents.add(eventId);
    
    // Clean up old events (keep last 1000)
    if (this.processedEvents.size > 1000) {
      const entries = Array.from(this.processedEvents);
      this.processedEvents = new Set(entries.slice(-1000));
    }
    
    return true;
  }
}
```

### Best Practices Summary

#### DO:
✅ **Monitor buffer state** before sending critical events  
✅ **Implement connection state UI** for user feedback  
✅ **Use exponential backoff** for reconnection  
✅ **Clean up resources** on disconnect  
✅ **Handle all error cases** gracefully  
✅ **Test with network interruptions** during development  

#### DON'T:
❌ **Send large messages** without checking buffer  
❌ **Ignore connection state** when sending  
❌ **Reconnect immediately** without backoff  
❌ **Keep references** to closed WebSockets  
❌ **Process duplicate events** after reconnection  
❌ **Block UI** during reconnection attempts  

---

## Quick Reference Card

### Connection URLs
```typescript
// Production
wss://api.agentc.ai/rt/ws?token=YOUR_TOKEN

// With session resume
wss://api.agentc.ai/rt/ws?token=YOUR_TOKEN&session_id=SESSION_ID
```

### Common WebSocket Close Codes
```
1000 - Normal closure
1001 - Going away (page navigation)
1006 - Abnormal closure (network error)
1011 - Server error
4000 - Custom: Ping timeout
4001 - Custom: Authentication failed
```

### Key Files Map
```
packages/core/src/client/
├── WebSocketManager.ts      # ⭐ WebSocket handling
├── ReconnectionManager.ts   # ⭐ Reconnection logic
├── RealtimeClient.ts       # Integration point
└── ClientConfig.ts         # Configuration types
```

### Essential Methods
```typescript
// Client level
client.connect()           // Establish connection
client.disconnect()        // Close connection
client.sendEvent(event)    // Send JSON event
client.sendBinaryFrame()   // Send audio

// WebSocket level
wsManager.isConnected()   // Check connection
wsManager.getBufferedAmount() // Check buffer
wsManager.sendJSON()       // Send JSON
wsManager.sendBinary()     // Send binary
```

### Debug Commands
```typescript
// Enable all debug logging
localStorage.setItem('debug', 'agentc:*');

// Monitor specific components
localStorage.setItem('debug', 'agentc:websocket,agentc:reconnect');

// Check connection state
console.log(client.getConnectionState());

// Check buffer
console.log(client.wsManager?.getBufferedAmount());
```

---

*This guide documents the WebSocket layer as the critical transport foundation for the Agent C Realtime SDK. The tight coupling with the event system makes understanding both layers essential for effective development.*
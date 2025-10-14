# WebSocketManager API Reference

The `WebSocketManager` class provides low-level WebSocket connection management with support for JSON and binary data transmission, automatic heartbeat/ping-pong, and robust connection lifecycle handling.

## Import

```typescript
import { WebSocketManager } from '@agentc/realtime-core';
```

## Overview

The WebSocketManager is the foundation of the Agent C Realtime SDK's network communication layer. It handles WebSocket connections, message routing, binary audio data transmission, and connection health monitoring through heartbeat mechanisms. This class is primarily used internally by `RealtimeClient` but can be used directly for custom WebSocket implementations.

## Constructor

```typescript
constructor(
  options: WebSocketManagerOptions,
  callbacks?: WebSocketManagerCallbacks
)
```

Creates a new WebSocketManager instance.

### Parameters

- `options` (WebSocketManagerOptions) - Configuration for the WebSocket connection
- `callbacks` (WebSocketManagerCallbacks) - Optional event callback handlers

### Configuration Options

```typescript
interface WebSocketManagerOptions {
  /** WebSocket server URL */
  url: string;
  /** Optional subprotocols for WebSocket handshake */
  protocols?: string[];
  /** Binary data type (default: 'arraybuffer') */
  binaryType?: BinaryType;
  /** Interval for sending ping messages (ms) */
  pingInterval?: number;
  /** Timeout waiting for pong response (ms) */
  pongTimeout?: number;
}
```

### Callback Options

```typescript
interface WebSocketManagerCallbacks {
  /** Called when WebSocket connection opens */
  onOpen?: (event: Event) => void;
  /** Called when WebSocket connection closes */
  onClose?: (event: CloseEvent) => void;
  /** Called when WebSocket error occurs */
  onError?: (event: Event) => void;
  /** Called when message is received (text or binary) */
  onMessage?: (data: string | ArrayBuffer) => void;
}
```

### Default Configuration

```typescript
const defaultOptions = {
  binaryType: 'arraybuffer',  // Required for audio data
  // No defaults for pingInterval/pongTimeout - opt-in feature
};
```

### Example

```typescript
const wsManager = new WebSocketManager(
  {
    url: 'wss://api.example.com/rt/ws',
    protocols: ['realtime-v1'],
    binaryType: 'arraybuffer',
    pingInterval: 30000,    // 30 seconds
    pongTimeout: 5000        // 5 seconds
  },
  {
    onOpen: (event) => console.log('Connected'),
    onClose: (event) => console.log(`Disconnected: ${event.code}`),
    onError: (event) => console.error('WebSocket error'),
    onMessage: (data) => {
      if (typeof data === 'string') {
        console.log('Received JSON:', JSON.parse(data));
      } else {
        console.log('Received binary:', data.byteLength, 'bytes');
      }
    }
  }
);
```

## Connection Management

### connect()

Establishes a WebSocket connection to the server.

```typescript
connect(): void
```

**Behavior:**
- Disconnects any existing connection first
- Creates new WebSocket with configured URL and protocols
- Sets binary type for proper audio handling
- Sets up all event handlers
- Starts heartbeat mechanism if configured

**Throws:** Error if WebSocket creation fails

**Example:**
```typescript
try {
  wsManager.connect();
  console.log('Connection initiated');
} catch (error) {
  console.error('Failed to connect:', error);
}
```

### disconnect()

Closes the WebSocket connection and cleans up resources.

```typescript
disconnect(code?: number, reason?: string): void
```

**Parameters:**
- `code` (number) - WebSocket close code (default: 1000)
- `reason` (string) - Close reason (default: 'Client disconnect')

**Behavior:**
- Stops heartbeat mechanism
- Removes all event handlers
- Closes WebSocket if open
- Sets internal WebSocket reference to null

**Example:**
```typescript
// Normal disconnect
wsManager.disconnect();

// Custom close code and reason
wsManager.disconnect(1001, 'Going away');

// Application shutdown
wsManager.disconnect(1000, 'Application closing');
```

### getReadyState()

Returns the current WebSocket ready state.

```typescript
getReadyState(): number
```

**Returns:** WebSocket ready state constant:
- `WebSocket.CONNECTING` (0) - Connection not yet established
- `WebSocket.OPEN` (1) - Connection is open and ready
- `WebSocket.CLOSING` (2) - Connection is closing
- `WebSocket.CLOSED` (3) - Connection is closed or not connected

**Example:**
```typescript
const state = wsManager.getReadyState();
switch (state) {
  case WebSocket.CONNECTING:
    console.log('Connecting...');
    break;
  case WebSocket.OPEN:
    console.log('Connected and ready');
    break;
  case WebSocket.CLOSING:
    console.log('Connection closing...');
    break;
  case WebSocket.CLOSED:
    console.log('Not connected');
    break;
}
```

### isConnected()

Checks if the WebSocket is currently connected and ready.

```typescript
isConnected(): boolean
```

**Returns:** `true` if WebSocket is in OPEN state, `false` otherwise

**Example:**
```typescript
if (wsManager.isConnected()) {
  wsManager.send('Hello, server!');
} else {
  console.log('Not connected - cannot send message');
}
```

## Data Transmission

### send()

Sends data through the WebSocket connection.

```typescript
send(data: string | ArrayBuffer | ArrayBufferView): void
```

**Parameters:**
- `data` - String for text/JSON, ArrayBuffer or TypedArray for binary

**Throws:** 
- Error if WebSocket is not connected
- Error if WebSocket is not in OPEN state

**Example:**
```typescript
// Send text
wsManager.send('Hello, server!');

// Send JSON (as string)
wsManager.send(JSON.stringify({ type: 'ping' }));

// Send binary data
const audioData = new ArrayBuffer(1024);
wsManager.send(audioData);

// Send typed array
const uint8Data = new Uint8Array([1, 2, 3, 4]);
wsManager.send(uint8Data);
```

### sendJSON()

Sends JSON data through the WebSocket.

```typescript
sendJSON(data: unknown): void
```

**Parameters:**
- `data` - Any JSON-serializable value

**Note:** Special handling for `client_wants_cancel` events with buffered amount logging

**Example:**
```typescript
// Send event object
wsManager.sendJSON({
  type: 'text_input',
  text: 'Hello, how are you?'
});

// Send array
wsManager.sendJSON(['item1', 'item2', 'item3']);

// Send complex nested object
wsManager.sendJSON({
  type: 'set_agent',
  agent_key: 'assistant',
  config: {
    temperature: 0.7,
    max_tokens: 1000
  }
});
```

### sendBinary()

Sends binary data through the WebSocket with validation.

```typescript
sendBinary(data: ArrayBuffer | ArrayBufferView): void
```

**Parameters:**
- `data` - Binary data as ArrayBuffer or TypedArray

**Throws:** Error if WebSocket doesn't support binary data

**Example:**
```typescript
// Send raw PCM16 audio data
const audioChunk = new ArrayBuffer(3200); // 100ms at 16kHz
try {
  wsManager.sendBinary(audioChunk);
} catch (error) {
  console.error('Binary not supported:', error);
}

// Send typed array
const samples = new Int16Array(1600); // Audio samples
wsManager.sendBinary(samples);
```

## Binary Data Support

### supportsBinary()

Checks if the WebSocket connection supports binary data transmission.

```typescript
supportsBinary(): boolean
```

**Returns:** `true` if binary type is 'arraybuffer', `false` otherwise

**Example:**
```typescript
if (wsManager.supportsBinary()) {
  console.log('Binary audio streaming supported');
  startAudioStreaming();
} else {
  console.warn('Binary not supported - audio disabled');
}
```

## Buffer Management

### getBufferedAmount()

Gets the number of bytes queued for transmission but not yet sent.

```typescript
getBufferedAmount(): number
```

**Returns:** Number of bytes in send buffer (0 if not connected)

**Example:**
```typescript
const buffered = wsManager.getBufferedAmount();
if (buffered > 0) {
  console.warn(`${buffered} bytes waiting to be sent`);
  // Maybe wait before sending more data
}
```

### hasBufferedData()

Checks if there is data waiting to be transmitted.

```typescript
hasBufferedData(): boolean
```

**Returns:** `true` if bufferedAmount > 0, `false` otherwise

**Example:**
```typescript
// Check before sending critical messages
if (wsManager.hasBufferedData()) {
  console.warn('Buffer not empty - cancel may be delayed');
}
wsManager.sendJSON({ type: 'client_wants_cancel' });
```

## URL Management

### getUrl()

Gets the current WebSocket URL.

```typescript
getUrl(): string
```

**Returns:** The configured WebSocket URL

**Example:**
```typescript
const url = wsManager.getUrl();
console.log('Connected to:', url);
```

### setUrl()

Updates the WebSocket URL (requires reconnection).

```typescript
setUrl(url: string): void
```

**Parameters:**
- `url` (string) - New WebSocket URL

**Note:** Does not affect existing connection; reconnection required

**Example:**
```typescript
// Update URL for next connection
wsManager.setUrl('wss://new-server.example.com/ws');

// Must reconnect for change to take effect
wsManager.disconnect();
wsManager.connect();
```

## Event Handling

The WebSocketManager processes both text (JSON) and binary messages from the server.

### Message Processing Flow

```typescript
// Internal message handler
private onMessage(event: MessageEvent): void {
  // Reset heartbeat on any message
  this.isAlive = true;
  
  if (typeof event.data === 'string') {
    // JSON message
    this.callbacks.onMessage?.(event.data);
  } else if (event.data instanceof Blob) {
    // Binary blob (browser) - convert to ArrayBuffer
    event.data.arrayBuffer().then(buffer => {
      this.callbacks.onMessage?.(buffer);
    });
  } else if (event.data instanceof ArrayBuffer) {
    // Binary ArrayBuffer
    this.callbacks.onMessage?.(event.data);
  }
}
```

### Event Callback Examples

```typescript
const wsManager = new WebSocketManager(
  { url: 'wss://api.example.com/ws' },
  {
    onOpen: (event) => {
      console.log('✅ WebSocket opened');
      // Connection established, ready to send
      sendInitialHandshake();
    },
    
    onClose: (event) => {
      console.log(`❌ WebSocket closed: ${event.code} - ${event.reason}`);
      // Handle disconnection
      if (event.code !== 1000) {
        attemptReconnection();
      }
    },
    
    onError: (event) => {
      console.error('⚠️ WebSocket error:', event);
      // Log error, maybe attempt recovery
    },
    
    onMessage: (data) => {
      if (typeof data === 'string') {
        // Handle JSON message
        const event = JSON.parse(data);
        handleServerEvent(event);
      } else if (data instanceof ArrayBuffer) {
        // Handle binary audio data
        playAudioChunk(data);
      }
    }
  }
);
```

## Heartbeat Mechanism

The WebSocketManager implements an optional ping-pong heartbeat mechanism to detect stale connections.

### Configuration

```typescript
const wsManager = new WebSocketManager({
  url: 'wss://api.example.com/ws',
  pingInterval: 30000,  // Send ping every 30 seconds
  pongTimeout: 5000      // Wait 5 seconds for pong
});
```

### How It Works

1. **Ping Interval**: Sends `{ type: 'ping' }` message at configured interval
2. **Alive Tracking**: Marks connection as "not alive" when sending ping
3. **Message Reset**: Any received message marks connection as "alive"
4. **Timeout Detection**: If no message received before next ping, disconnects with code 4000
5. **Pong Timeout**: Optional additional timeout for explicit pong response

### Server Implementation

Server should respond to ping messages:

```javascript
// Server-side WebSocket handler
ws.on('message', (data) => {
  const message = JSON.parse(data);
  if (message.type === 'ping') {
    ws.send(JSON.stringify({ type: 'pong' }));
  }
});
```

## Integration with RealtimeClient

The WebSocketManager is the underlying transport layer for RealtimeClient:

```typescript
// Internal usage in RealtimeClient
class RealtimeClient {
  private wsManager: WebSocketManager | null = null;
  
  async connect(): Promise<void> {
    const wsUrl = this.buildWebSocketUrl();
    
    this.wsManager = new WebSocketManager(
      {
        url: wsUrl,
        protocols: this.config.protocols,
        binaryType: 'arraybuffer',  // Always for audio
        pingInterval: this.config.pingInterval,
        pongTimeout: this.config.pongTimeout
      },
      {
        onOpen: () => {
          this.setConnectionState(ConnectionState.CONNECTED);
          this.emit('connected', undefined);
        },
        onClose: (event) => {
          this.handleDisconnection(event.code, event.reason);
        },
        onMessage: (data) => {
          this.handleMessage(data);  // Routes to appropriate handlers
        }
      }
    );
    
    this.wsManager.connect();
  }
  
  // Send methods delegate to WebSocketManager
  sendEvent(event: any): void {
    this.wsManager.sendJSON(event);
  }
  
  sendBinaryFrame(audioData: ArrayBuffer): void {
    this.wsManager.sendBinary(audioData);
  }
}
```

## Complete Example: Audio Streaming Client

```typescript
import { WebSocketManager } from '@agentc/realtime-core';

class AudioStreamingClient {
  private wsManager: WebSocketManager;
  private audioContext: AudioContext;
  private mediaStream: MediaStream | null = null;
  private processor: ScriptProcessorNode | null = null;
  private isStreaming = false;
  
  constructor(url: string, token: string) {
    // Build authenticated URL
    const wsUrl = new URL(url);
    wsUrl.searchParams.set('token', token);
    
    // Initialize WebSocket manager
    this.wsManager = new WebSocketManager(
      {
        url: wsUrl.toString(),
        binaryType: 'arraybuffer',
        pingInterval: 30000,
        pongTimeout: 5000
      },
      {
        onOpen: () => this.handleOpen(),
        onClose: (e) => this.handleClose(e),
        onError: (e) => this.handleError(e),
        onMessage: (data) => this.handleMessage(data)
      }
    );
    
    this.audioContext = new AudioContext();
  }
  
  async connect(): Promise<void> {
    console.log('Connecting to audio streaming service...');
    this.wsManager.connect();
  }
  
  disconnect(): void {
    this.stopStreaming();
    this.wsManager.disconnect();
    console.log('Disconnected from audio streaming service');
  }
  
  private handleOpen(): void {
    console.log('✅ Connected to audio streaming service');
    
    // Send initial configuration
    this.wsManager.sendJSON({
      type: 'configure_audio',
      config: {
        sampleRate: 16000,
        channels: 1,
        format: 'pcm16'
      }
    });
  }
  
  private handleClose(event: CloseEvent): void {
    console.log(`❌ Disconnected: ${event.code} - ${event.reason}`);
    this.stopStreaming();
    
    // Attempt reconnection for non-normal closures
    if (event.code !== 1000) {
      setTimeout(() => this.connect(), 5000);
    }
  }
  
  private handleError(event: Event): void {
    console.error('WebSocket error:', event);
  }
  
  private handleMessage(data: string | ArrayBuffer): void {
    if (typeof data === 'string') {
      // Handle JSON control messages
      const message = JSON.parse(data);
      this.handleControlMessage(message);
    } else {
      // Handle binary audio data from server
      this.handleAudioData(data);
    }
  }
  
  private handleControlMessage(message: any): void {
    console.log('Control message:', message.type);
    
    switch (message.type) {
      case 'audio_configured':
        console.log('Audio configured, ready to stream');
        break;
      case 'transcription':
        console.log('Transcription:', message.text);
        this.onTranscription?.(message.text);
        break;
      case 'error':
        console.error('Server error:', message.message);
        break;
    }
  }
  
  private handleAudioData(data: ArrayBuffer): void {
    // Play received audio through speakers
    this.playAudio(data);
  }
  
  async startStreaming(): Promise<void> {
    if (this.isStreaming) return;
    
    try {
      // Get microphone access
      this.mediaStream = await navigator.mediaDevices.getUserMedia({
        audio: {
          channelCount: 1,
          sampleRate: 16000,
          echoCancellation: true,
          noiseSuppression: true
        }
      });
      
      const source = this.audioContext.createMediaStreamSource(this.mediaStream);
      this.processor = this.audioContext.createScriptProcessor(2048, 1, 1);
      
      this.processor.onaudioprocess = (e) => {
        if (!this.isStreaming) return;
        
        // Convert Float32Array to Int16Array (PCM16)
        const float32 = e.inputBuffer.getChannelData(0);
        const int16 = new Int16Array(float32.length);
        
        for (let i = 0; i < float32.length; i++) {
          const s = Math.max(-1, Math.min(1, float32[i]));
          int16[i] = s < 0 ? s * 0x8000 : s * 0x7FFF;
        }
        
        // Send audio to server
        if (this.wsManager.isConnected() && !this.wsManager.hasBufferedData()) {
          this.wsManager.sendBinary(int16.buffer);
        }
      };
      
      source.connect(this.processor);
      this.processor.connect(this.audioContext.destination);
      
      this.isStreaming = true;
      console.log('🎤 Audio streaming started');
      
      // Notify server
      this.wsManager.sendJSON({ type: 'start_streaming' });
      
    } catch (error) {
      console.error('Failed to start streaming:', error);
      throw error;
    }
  }
  
  stopStreaming(): void {
    if (!this.isStreaming) return;
    
    // Clean up audio resources
    if (this.processor) {
      this.processor.disconnect();
      this.processor = null;
    }
    
    if (this.mediaStream) {
      this.mediaStream.getTracks().forEach(track => track.stop());
      this.mediaStream = null;
    }
    
    this.isStreaming = false;
    console.log('🛑 Audio streaming stopped');
    
    // Notify server if connected
    if (this.wsManager.isConnected()) {
      this.wsManager.sendJSON({ type: 'stop_streaming' });
    }
  }
  
  private playAudio(buffer: ArrayBuffer): void {
    // Convert PCM16 to Float32 for Web Audio API
    const int16 = new Int16Array(buffer);
    const float32 = new Float32Array(int16.length);
    
    for (let i = 0; i < int16.length; i++) {
      float32[i] = int16[i] / (int16[i] < 0 ? 0x8000 : 0x7FFF);
    }
    
    // Create audio buffer and play
    const audioBuffer = this.audioContext.createBuffer(1, float32.length, 16000);
    audioBuffer.copyToChannel(float32, 0);
    
    const source = this.audioContext.createBufferSource();
    source.buffer = audioBuffer;
    source.connect(this.audioContext.destination);
    source.start();
  }
  
  getStatus(): {
    connected: boolean;
    streaming: boolean;
    bufferedBytes: number;
  } {
    return {
      connected: this.wsManager.isConnected(),
      streaming: this.isStreaming,
      bufferedBytes: this.wsManager.getBufferedAmount()
    };
  }
  
  // Event handlers
  onTranscription?: (text: string) => void;
}

// Usage
async function main() {
  const client = new AudioStreamingClient(
    'wss://api.example.com/audio/ws',
    'your-auth-token'
  );
  
  // Set up event handlers
  client.onTranscription = (text) => {
    console.log('You said:', text);
    document.getElementById('transcript')!.textContent = text;
  };
  
  // Connect to service
  await client.connect();
  
  // Start streaming on button click
  document.getElementById('start-btn')!.addEventListener('click', () => {
    client.startStreaming();
  });
  
  document.getElementById('stop-btn')!.addEventListener('click', () => {
    client.stopStreaming();
  });
  
  // Monitor status
  setInterval(() => {
    const status = client.getStatus();
    console.log('Status:', status);
  }, 1000);
  
  // Clean shutdown
  window.addEventListener('beforeunload', () => {
    client.disconnect();
  });
}

main().catch(console.error);
```

## Error Handling

The WebSocketManager can encounter various error conditions:

### Connection Errors

```typescript
try {
  wsManager.connect();
} catch (error) {
  // Failed to create WebSocket
  console.error('Connection failed:', error);
}
```

### Send Errors

```typescript
try {
  wsManager.send('Hello');
} catch (error) {
  if (error.message.includes('not connected')) {
    console.log('WebSocket not connected');
  } else if (error.message.includes('not open')) {
    console.log('WebSocket not ready');
  }
}

try {
  wsManager.sendBinary(audioData);
} catch (error) {
  if (error.message.includes('does not support binary')) {
    console.log('Binary not supported');
  }
}
```

### Event Errors

```typescript
const wsManager = new WebSocketManager(
  { url: 'wss://api.example.com/ws' },
  {
    onError: (event) => {
      // Network errors, protocol errors, etc.
      console.error('WebSocket error event:', event);
      
      // Check connection state
      const state = wsManager.getReadyState();
      if (state === WebSocket.CLOSED) {
        // Attempt reconnection
        scheduleReconnection();
      }
    },
    
    onMessage: (data) => {
      try {
        if (typeof data === 'string') {
          const message = JSON.parse(data);
          handleMessage(message);
        }
      } catch (error) {
        console.error('Failed to parse message:', error);
        // Invalid JSON from server
      }
    }
  }
);
```

## Best Practices

### 1. Always Set Binary Type for Audio

```typescript
// ✅ Correct - explicit arraybuffer for audio
const wsManager = new WebSocketManager({
  url: 'wss://api.example.com/ws',
  binaryType: 'arraybuffer'  // Required for audio streaming
});

// ❌ Wrong - blob type requires conversion
const wsManager = new WebSocketManager({
  url: 'wss://api.example.com/ws',
  binaryType: 'blob'
});
```

### 2. Check Connection State Before Sending

```typescript
// ✅ Correct - verify connection
if (wsManager.isConnected()) {
  wsManager.send(data);
} else {
  console.log('Not connected, queueing message');
  messageQueue.push(data);
}

// ❌ Wrong - sending without checking
wsManager.send(data);  // May throw error
```

### 3. Monitor Buffer for Critical Messages

```typescript
// ✅ Correct - check buffer for time-sensitive messages
function sendCancelRequest() {
  if (wsManager.hasBufferedData()) {
    console.warn('Buffer not empty - cancel may be delayed');
  }
  wsManager.sendJSON({ type: 'client_wants_cancel' });
  console.log('Cancel request sent');
}

// ❌ Wrong - ignoring buffer state
wsManager.sendJSON({ type: 'client_wants_cancel' });
```

### 4. Clean Disconnection

```typescript
// ✅ Correct - clean shutdown
function shutdown() {
  // Stop any ongoing operations
  stopAudioStreaming();
  
  // Disconnect with proper code
  wsManager.disconnect(1000, 'Client shutdown');
  
  // Clean up resources
  cleanup();
}

// ❌ Wrong - abrupt disconnection
wsManager.disconnect(1006, 'Abnormal');  // Use only for errors
```

### 5. Implement Heartbeat for Production

```typescript
// ✅ Correct - heartbeat for connection health
const wsManager = new WebSocketManager({
  url: 'wss://api.example.com/ws',
  pingInterval: 30000,    // 30 seconds
  pongTimeout: 5000       // 5 second timeout
});

// Server should respond to pings
// Connection auto-disconnects if no response

// ❌ Wrong - no heartbeat in production
const wsManager = new WebSocketManager({
  url: 'wss://api.example.com/ws'
  // No ping configuration
});
```

### 6. Handle Binary Data Efficiently

```typescript
// ✅ Correct - efficient binary handling
const audioBuffer = new ArrayBuffer(3200);  // Pre-allocate
// Fill buffer with audio data...
wsManager.sendBinary(audioBuffer);

// ❌ Wrong - inefficient conversion
const audioArray = Array.from(audioSamples);
const json = JSON.stringify({ audio: audioArray });
wsManager.send(json);  // Much larger than binary
```

### 7. Separate Control and Data Channels

```typescript
// ✅ Correct - use appropriate channel
// Control messages as JSON
wsManager.sendJSON({ type: 'configure', settings: {...} });

// Audio data as binary
wsManager.sendBinary(pcm16AudioData);

// ❌ Wrong - mixing concerns
wsManager.sendJSON({ 
  type: 'audio',
  data: Array.from(pcm16AudioData)  // Inefficient
});
```

### 8. Error Recovery Strategy

```typescript
class RobustWebSocketManager {
  private manager: WebSocketManager;
  private reconnectAttempts = 0;
  private maxReconnectAttempts = 5;
  
  constructor(url: string) {
    this.createManager(url);
  }
  
  private createManager(url: string): void {
    this.manager = new WebSocketManager(
      { url, binaryType: 'arraybuffer' },
      {
        onClose: (event) => {
          if (event.code !== 1000) {
            this.handleAbnormalClose(event);
          }
        },
        onError: () => {
          this.handleError();
        }
      }
    );
  }
  
  private handleAbnormalClose(event: CloseEvent): void {
    console.log(`Abnormal close: ${event.code}`);
    
    if (this.reconnectAttempts < this.maxReconnectAttempts) {
      const delay = Math.min(1000 * Math.pow(2, this.reconnectAttempts), 30000);
      setTimeout(() => {
        this.reconnectAttempts++;
        this.connect();
      }, delay);
    } else {
      console.error('Max reconnection attempts reached');
      this.onConnectionFailed?.();
    }
  }
  
  private handleError(): void {
    console.error('WebSocket error occurred');
    // Log to monitoring service
    // Attempt recovery if appropriate
  }
  
  connect(): void {
    try {
      this.manager.connect();
    } catch (error) {
      console.error('Failed to connect:', error);
      this.handleAbnormalClose({ code: 1006 } as CloseEvent);
    }
  }
  
  onConnectionFailed?: () => void;
}
```

## TypeScript Types

```typescript
import {
  WebSocketManager,
  WebSocketManagerOptions,
  WebSocketManagerCallbacks
} from '@agentc/realtime-core';

// Option types
interface WebSocketManagerOptions {
  url: string;
  protocols?: string[];
  binaryType?: BinaryType;
  pingInterval?: number;
  pongTimeout?: number;
}

// Callback types
interface WebSocketManagerCallbacks {
  onOpen?: (event: Event) => void;
  onClose?: (event: CloseEvent) => void;
  onError?: (event: Event) => void;
  onMessage?: (data: string | ArrayBuffer) => void;
}

// Binary type options
type BinaryType = 'blob' | 'arraybuffer';
```

All methods and properties are fully typed for TypeScript applications.

## Integration with ReconnectionManager

WebSocketManager works seamlessly with ReconnectionManager for automatic reconnection:

```typescript
import { WebSocketManager, ReconnectionManager } from '@agentc/realtime-core';

class ReliableWebSocketClient {
  private wsManager: WebSocketManager;
  private reconnectionManager: ReconnectionManager;
  
  constructor(url: string) {
    this.wsManager = new WebSocketManager(
      {
        url,
        binaryType: 'arraybuffer',
        pingInterval: 30000
      },
      {
        onClose: (event) => {
          if (event.code !== 1000) {
            this.handleDisconnection();
          }
        }
      }
    );
    
    this.reconnectionManager = new ReconnectionManager({
      enabled: true,
      initialDelay: 1000,
      maxDelay: 30000,
      backoffMultiplier: 1.5,
      maxAttempts: 10,
      jitterFactor: 0.3
    });
  }
  
  async connect(): Promise<void> {
    this.wsManager.connect();
  }
  
  private async handleDisconnection(): Promise<void> {
    try {
      await this.reconnectionManager.startReconnection(async () => {
        this.wsManager.connect();
        
        // Wait for connection to establish
        await new Promise((resolve, reject) => {
          const timeout = setTimeout(() => reject(new Error('Timeout')), 5000);
          
          const checkConnection = setInterval(() => {
            if (this.wsManager.isConnected()) {
              clearInterval(checkConnection);
              clearTimeout(timeout);
              resolve(undefined);
            }
          }, 100);
        });
      });
      
      console.log('Successfully reconnected');
    } catch (error) {
      console.error('Failed to reconnect:', error);
    }
  }
}
```

## Performance Considerations

### Buffer Management

Monitor bufferedAmount to prevent memory issues:

```typescript
const MAX_BUFFER_SIZE = 65536;  // 64KB

function safeSend(wsManager: WebSocketManager, data: ArrayBuffer): boolean {
  const buffered = wsManager.getBufferedAmount();
  
  if (buffered + data.byteLength > MAX_BUFFER_SIZE) {
    console.warn('Buffer full, dropping audio frame');
    return false;
  }
  
  wsManager.sendBinary(data);
  return true;
}
```

### Audio Streaming Optimization

```typescript
// Optimize audio chunk size for network
const OPTIMAL_CHUNK_SIZE = 3200;  // 100ms at 16kHz PCM16

// Buffer audio samples
let audioBuffer = new Int16Array(OPTIMAL_CHUNK_SIZE / 2);
let bufferIndex = 0;

function processAudioSample(sample: number): void {
  audioBuffer[bufferIndex++] = sample;
  
  if (bufferIndex >= audioBuffer.length) {
    // Send full chunk
    if (wsManager.isConnected()) {
      wsManager.sendBinary(audioBuffer.buffer);
    }
    bufferIndex = 0;
  }
}
```

### Connection State Caching

```typescript
class OptimizedWebSocketWrapper {
  private wsManager: WebSocketManager;
  private cachedState: number = WebSocket.CLOSED;
  private stateListeners: Set<(state: number) => void> = new Set();
  
  constructor(wsManager: WebSocketManager) {
    this.wsManager = wsManager;
    this.pollState();
  }
  
  private pollState(): void {
    setInterval(() => {
      const newState = this.wsManager.getReadyState();
      if (newState !== this.cachedState) {
        this.cachedState = newState;
        this.notifyStateChange(newState);
      }
    }, 100);
  }
  
  private notifyStateChange(state: number): void {
    this.stateListeners.forEach(listener => listener(state));
  }
  
  onStateChange(listener: (state: number) => void): void {
    this.stateListeners.add(listener);
  }
  
  getCachedState(): number {
    return this.cachedState;
  }
  
  isConnected(): boolean {
    return this.cachedState === WebSocket.OPEN;
  }
}
```

This documentation provides comprehensive coverage of the WebSocketManager API, its features, and best practices for production use.
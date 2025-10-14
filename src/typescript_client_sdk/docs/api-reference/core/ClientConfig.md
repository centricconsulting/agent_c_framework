# Client Configuration API Reference

## Overview

The Agent C Realtime SDK provides a comprehensive configuration system that allows developers to customize every aspect of the client's behavior, from connection settings to audio processing. This document covers all configuration interfaces and their options.

## Table of Contents

- [RealtimeClientConfig](#realtimeclientconfig)
- [ConnectionState Enum](#connectionstate-enum)
- [ReconnectionConfig](#reconnectionconfig)
- [AudioConfig](#audioconfig)
- [AuthConfig](#authconfig)
- [VoiceManagerConfig](#voicemanagerconfig)
- [AvatarManagerConfig](#avatarmanagerconfig)
- [SessionManagerConfig](#sessionmanagerconfig)
- [AudioProcessorConfig](#audioprocessorconfig)
- [AudioAgentCBridgeConfig](#audioagentcbridgeconfig)
- [Default Configurations](#default-configurations)
- [Configuration Examples](#configuration-examples)
- [Best Practices](#best-practices)

---

## RealtimeClientConfig

The main configuration interface for initializing a RealtimeClient instance.

```typescript
interface RealtimeClientConfig {
  /** WebSocket API URL (e.g., wss://api.example.com/rt/ws) */
  apiUrl: string;
  
  /** JWT authentication token (optional if using AuthManager) */
  authToken?: string;
  
  /** Optional AuthManager instance for automatic token management */
  authManager?: AuthManager;
  
  /** Optional session ID to resume */
  sessionId?: string;
  
  /** Enable automatic reconnection on disconnect */
  autoReconnect?: boolean;
  
  /** Detailed reconnection configuration */
  reconnection?: Partial<ReconnectionConfig>;
  
  /** WebSocket connection timeout (ms) */
  connectionTimeout?: number;
  
  /** Ping interval to keep connection alive (ms) */
  pingInterval?: number;
  
  /** Pong timeout - close connection if no pong received (ms) */
  pongTimeout?: number;
  
  /** Maximum size for incoming messages (bytes) */
  maxMessageSize?: number;
  
  /** Enable debug logging */
  debug?: boolean;
  
  /** Custom headers for WebSocket connection */
  headers?: Record<string, string>;
  
  /** WebSocket protocols to use */
  protocols?: string[];
  
  /** Binary type for WebSocket ('blob' or 'arraybuffer') */
  binaryType?: 'blob' | 'arraybuffer';
  
  /** Enable turn management (default: true) */
  enableTurnManager?: boolean;
  
  /** Enable audio functionality */
  enableAudio?: boolean;
  
  /** Audio configuration */
  audioConfig?: AudioConfig;
}
```

### Required Fields

- `apiUrl`: The WebSocket endpoint URL for the Agent C Realtime API
- Either `authToken` OR `authManager` must be provided for authentication

### Optional Fields with Defaults

| Field | Default Value | Description |
|-------|--------------|-------------|
| `autoReconnect` | `true` | Automatically attempt to reconnect on disconnect |
| `connectionTimeout` | `10000` (10s) | Maximum time to wait for initial connection |
| `pingInterval` | `30000` (30s) | How often to send ping messages |
| `pongTimeout` | `10000` (10s) | Maximum time to wait for pong response |
| `maxMessageSize` | `10485760` (10MB) | Maximum size of incoming WebSocket messages |
| `debug` | `false` | Enable debug logging to console |
| `binaryType` | `'arraybuffer'` | Format for binary WebSocket data |
| `enableTurnManager` | `true` | Enable server-controlled turn management |
| `enableAudio` | `false` | Enable audio input/output functionality |

---

## ConnectionState Enum

Represents the WebSocket connection lifecycle states.

```typescript
enum ConnectionState {
  /** Not connected to the server */
  DISCONNECTED = 'DISCONNECTED',
  
  /** Attempting to establish connection */
  CONNECTING = 'CONNECTING',
  
  /** Successfully connected to the server */
  CONNECTED = 'CONNECTED',
  
  /** Connection lost, attempting to reconnect */
  RECONNECTING = 'RECONNECTING'
}
```

---

## ReconnectionConfig

Controls automatic reconnection behavior when the connection is lost.

```typescript
interface ReconnectionConfig {
  /** Whether to automatically reconnect on disconnect */
  enabled: boolean;
  
  /** Initial delay before first reconnection attempt (ms) */
  initialDelay: number;
  
  /** Maximum delay between reconnection attempts (ms) */
  maxDelay: number;
  
  /** Multiplier for exponential backoff */
  backoffMultiplier: number;
  
  /** Maximum number of reconnection attempts (0 = unlimited) */
  maxAttempts: number;
  
  /** Jitter factor to randomize delays (0-1) */
  jitterFactor: number;
}
```

### Default ReconnectionConfig

```typescript
const defaultReconnectionConfig = {
  enabled: true,
  initialDelay: 1000,        // 1 second
  maxDelay: 30000,          // 30 seconds
  backoffMultiplier: 1.5,   // 1.5x increase each attempt
  maxAttempts: 0,           // Unlimited attempts
  jitterFactor: 0.3          // 30% jitter
};
```

The reconnection system uses exponential backoff with jitter to avoid thundering herd problems. Each reconnection attempt waits:
```
delay = min(initialDelay * (backoffMultiplier ^ attemptNumber) * (1 + random * jitterFactor), maxDelay)
```

---

## AudioConfig

Configuration for audio input and output functionality.

```typescript
interface AudioConfig {
  /** Enable audio input (microphone) */
  enableInput?: boolean;
  
  /** Enable audio output (speakers) */
  enableOutput?: boolean;
  
  /** Respect turn state for audio input */
  respectTurnState?: boolean;
  
  /** Log audio chunks for debugging */
  logAudioChunks?: boolean;
  
  /** Audio sample rate (default: 24000) */
  sampleRate?: number;
  
  /** Audio chunk size in samples (default: 4800) */
  chunkSize?: number;
  
  /** Initial playback volume (0-1, default: 1.0) */
  initialVolume?: number;
  
  /** Enable automatic gain control */
  autoGainControl?: boolean;
  
  /** Enable echo cancellation */
  echoCancellation?: boolean;
  
  /** Enable noise suppression */
  noiseSuppression?: boolean;
}
```

### Default AudioConfig

```typescript
const defaultAudioConfig = {
  enableInput: true,
  enableOutput: true,
  respectTurnState: true,
  logAudioChunks: false,
  sampleRate: 24000,
  chunkSize: 4800,
  initialVolume: 1.0,
  autoGainControl: true,
  echoCancellation: true,
  noiseSuppression: true
};
```

### Audio Processing Notes

- **Sample Rate**: The SDK uses 24kHz by default, which provides good quality for voice
- **Chunk Size**: 4800 samples at 24kHz = 200ms chunks
- **Turn State**: When `respectTurnState` is true, audio input is only sent when the user has the turn
- **Browser Features**: `autoGainControl`, `echoCancellation`, and `noiseSuppression` use browser MediaStream constraints

---

## AuthConfig

Configuration for the authentication manager.

```typescript
interface AuthConfig {
  /**
   * Base URL for the Agent C API (without /rt paths)
   * @example 'https://api.example.com'
   */
  apiUrl: string;

  /**
   * Optional custom fetch implementation (for testing or custom headers)
   */
  fetch?: typeof fetch;

  /**
   * Token refresh buffer time in milliseconds
   * Tokens will be refreshed this many ms before expiry
   * @default 60000 (1 minute)
   */
  refreshBufferMs?: number;

  /**
   * Whether to automatically refresh tokens before expiry
   * @default true
   */
  autoRefresh?: boolean;

  /**
   * Optional callback when tokens are refreshed
   */
  onTokensRefreshed?: (tokens: TokenPair) => void;

  /**
   * Optional callback when authentication fails
   */
  onAuthError?: (error: Error) => void;

  /**
   * Storage adapter for persisting tokens (optional)
   * If not provided, tokens are only stored in memory
   */
  storage?: TokenStorage;
}
```

### TokenStorage Interface

For persisting authentication tokens across sessions:

```typescript
interface TokenStorage {
  /** Get stored tokens */
  getTokens(): Promise<TokenPair | null>;
  
  /** Store tokens */
  setTokens(tokens: TokenPair): Promise<void>;
  
  /** Clear stored tokens */
  clearTokens(): Promise<void>;
}
```

### Built-in Storage Implementations

#### MemoryTokenStorage
Stores tokens in memory only (default):

```typescript
const storage = new MemoryTokenStorage();
```

#### LocalStorageTokenStorage
Persists tokens to browser localStorage:

```typescript
const storage = new LocalStorageTokenStorage('agentc_auth_tokens');
```

---

## VoiceManagerConfig

Configuration for voice model management.

```typescript
interface VoiceManagerConfig {
  /** Default voice ID to use */
  defaultVoiceId?: string;
  
  /** Enable logging for voice operations */
  enableLogging?: boolean;
}
```

### Special Voice Identifiers

```typescript
const SPECIAL_VOICES = {
  AVATAR: 'avatar',  // Audio handled by HeyGen avatar
  NONE: 'none'       // Text-only mode (no audio)
};
```

---

## AvatarManagerConfig

Configuration for HeyGen avatar integration.

```typescript
interface AvatarManagerConfig {
  /** List of available avatars */
  availableAvatars?: Avatar[];
}
```

The AvatarManager tracks avatar state but does NOT directly control HeyGen. The client application must:
1. Establish a session with HeyGen SDK
2. Wait for HeyGen's STREAM_READY event
3. Call `setAvatarSession()` to notify Agent C

---

## SessionManagerConfig

Configuration for chat session management.

```typescript
interface SessionManagerConfig {
  /** Maximum number of sessions to keep in memory */
  maxSessions?: number;
  
  /** Whether to persist sessions */
  persistSessions?: boolean;
  
  /** Default name for new sessions */
  defaultSessionName?: string;
}
```

---

## AudioProcessorConfig

Low-level configuration for the audio processor.

```typescript
interface AudioProcessorConfig {
  /** Target sample rate in Hz (default: 16000) */
  sampleRate?: number;
  
  /** Number of audio channels (default: 1 for mono) */
  channelCount?: number;
  
  /** Buffer size in samples before sending chunk (default: 2048) */
  bufferSize?: number;
  
  /** Path to the audio worklet file */
  workletPath?: string;
  
  /** Enable debug logging */
  debug?: boolean;
}
```

### Default AudioProcessorConfig

```typescript
const DEFAULT_AUDIO_CONFIG = {
  sampleRate: 24000,
  channelCount: 1,
  bufferSize: 2048,
  workletPath: '/worklets/audio-processor.worklet.js',
  debug: false
};
```

---

## AudioAgentCBridgeConfig

Configuration for bridging audio service with the Agent C client.

```typescript
interface AudioAgentCBridgeConfig {
  /** Whether to respect turn state from TurnManager (default: true) */
  respectTurnState?: boolean;
  
  /** Enable debug logging for audio chunks (default: false) */
  logAudioChunks?: boolean;
  
  /** Enable general debug logging (default: false) */
  debug?: boolean;
}
```

---

## Default Configurations

The SDK provides sensible defaults that work for most use cases:

```typescript
const defaultConfig = {
  // Connection
  autoReconnect: true,
  connectionTimeout: 10000,     // 10 seconds
  pingInterval: 30000,          // 30 seconds
  pongTimeout: 10000,           // 10 seconds
  maxMessageSize: 10485760,     // 10MB
  
  // Features
  debug: false,
  binaryType: 'arraybuffer',
  enableTurnManager: true,
  enableAudio: false,           // Requires user opt-in
  
  // Nested configs
  reconnection: defaultReconnectionConfig,
  audioConfig: defaultAudioConfig
};
```

---

## Configuration Examples

### Basic Configuration

Minimal configuration with authentication token:

```typescript
import { RealtimeClient } from '@agentc/realtime-core';

const client = new RealtimeClient({
  apiUrl: 'wss://api.example.com/rt/ws',
  authToken: 'your-jwt-token'
});
```

### With AuthManager

Using AuthManager for automatic token management:

```typescript
import { RealtimeClient, AuthManager } from '@agentc/realtime-core';

// Create auth manager
const authManager = new AuthManager({
  apiUrl: 'https://api.example.com',
  storage: new LocalStorageTokenStorage(),
  autoRefresh: true,
  refreshBufferMs: 60000
});

// Login
await authManager.login({ username: 'user', password: 'pass' });

// Create client with auth manager
const client = new RealtimeClient({
  apiUrl: 'wss://api.example.com/rt/ws',
  authManager: authManager
});
```

### Full Audio Configuration

Enabling audio with custom settings:

```typescript
const client = new RealtimeClient({
  apiUrl: 'wss://api.example.com/rt/ws',
  authToken: 'your-jwt-token',
  enableAudio: true,
  audioConfig: {
    enableInput: true,
    enableOutput: true,
    respectTurnState: true,
    sampleRate: 24000,
    chunkSize: 4800,
    initialVolume: 0.8,
    autoGainControl: true,
    echoCancellation: true,
    noiseSuppression: true,
    logAudioChunks: false
  }
});
```

### Custom Reconnection Strategy

Configuring reconnection with limited attempts:

```typescript
const client = new RealtimeClient({
  apiUrl: 'wss://api.example.com/rt/ws',
  authToken: 'your-jwt-token',
  autoReconnect: true,
  reconnection: {
    enabled: true,
    initialDelay: 2000,       // Start with 2s delay
    maxDelay: 60000,          // Max 1 minute between attempts
    backoffMultiplier: 2,     // Double the delay each time
    maxAttempts: 10,          // Give up after 10 attempts
    jitterFactor: 0.5         // 50% randomization
  }
});
```

### Debug Configuration

Enabling debug logging for troubleshooting:

```typescript
const client = new RealtimeClient({
  apiUrl: 'wss://api.example.com/rt/ws',
  authToken: 'your-jwt-token',
  debug: true,
  enableAudio: true,
  audioConfig: {
    logAudioChunks: true  // Also log audio chunk details
  }
});
```

### Session Resume Configuration

Resuming a previous session:

```typescript
const client = new RealtimeClient({
  apiUrl: 'wss://api.example.com/rt/ws',
  authToken: 'your-jwt-token',
  sessionId: 'previous-session-id'  // Resume specific session
});
```

### Production Configuration

Recommended settings for production:

```typescript
const client = new RealtimeClient({
  apiUrl: 'wss://api.production.com/rt/ws',
  authManager: authManager,  // Use AuthManager for token management
  
  // Connection settings
  autoReconnect: true,
  connectionTimeout: 15000,
  pingInterval: 30000,
  pongTimeout: 10000,
  
  // Reconnection strategy
  reconnection: {
    enabled: true,
    initialDelay: 1000,
    maxDelay: 30000,
    backoffMultiplier: 1.5,
    maxAttempts: 0,  // Keep trying
    jitterFactor: 0.3
  },
  
  // Features
  enableTurnManager: true,
  enableAudio: true,
  
  // Audio settings
  audioConfig: {
    enableInput: true,
    enableOutput: true,
    respectTurnState: true,
    sampleRate: 24000,
    initialVolume: 1.0,
    autoGainControl: true,
    echoCancellation: true,
    noiseSuppression: true,
    logAudioChunks: false  // Disable in production
  },
  
  // Disable debug in production
  debug: false
});
```

---

## Best Practices

### 1. Authentication Management

**Use AuthManager for production applications:**
- Handles token refresh automatically
- Manages token persistence
- Provides callbacks for auth events

```typescript
const authManager = new AuthManager({
  apiUrl: 'https://api.example.com',
  storage: new LocalStorageTokenStorage(),
  autoRefresh: true,
  onAuthError: (error) => {
    // Handle auth failures
    console.error('Auth failed:', error);
    redirectToLogin();
  }
});
```

### 2. Connection Resilience

**Configure reconnection for network reliability:**
- Use exponential backoff to avoid overwhelming the server
- Add jitter to prevent thundering herd
- Consider limiting attempts for mobile applications

```typescript
reconnection: {
  enabled: true,
  initialDelay: 1000,
  maxDelay: 30000,
  backoffMultiplier: 1.5,
  maxAttempts: 0,  // Unlimited for desktop
  jitterFactor: 0.3
}
```

### 3. Audio Configuration

**Request permissions before enabling audio:**

```typescript
// Check for microphone permission first
try {
  await navigator.mediaDevices.getUserMedia({ audio: true });
  // Permission granted, enable audio
  client.updateConfig({ enableAudio: true });
} catch (error) {
  console.error('Microphone permission denied');
}
```

**Respect turn state for better conversation flow:**

```typescript
audioConfig: {
  respectTurnState: true  // Only send audio when user has turn
}
```

### 4. Debug Logging

**Use debug mode during development only:**

```typescript
const isDevelopment = process.env.NODE_ENV === 'development';

const client = new RealtimeClient({
  apiUrl: config.wsUrl,
  authToken: token,
  debug: isDevelopment,
  audioConfig: {
    logAudioChunks: isDevelopment
  }
});
```

### 5. Session Management

**Handle session lifecycle properly:**

```typescript
// Save session ID for resume
client.on('session-started', ({ sessionId }) => {
  localStorage.setItem('lastSessionId', sessionId);
});

// Resume on reconnect
const lastSessionId = localStorage.getItem('lastSessionId');
if (lastSessionId) {
  client = new RealtimeClient({
    apiUrl: config.wsUrl,
    authToken: token,
    sessionId: lastSessionId
  });
}
```

### 6. Voice Configuration

**Initialize voice manager with defaults:**

```typescript
const voiceManager = new VoiceManager({
  defaultVoiceId: 'openai_tts_nova',
  enableLogging: false  // Disable in production
});

// Update available voices from auth response
authManager.on('authenticated', ({ voices }) => {
  voiceManager.setAvailableVoices(voices);
});
```

### 7. Error Handling

**Always handle configuration errors:**

```typescript
try {
  const client = new RealtimeClient(config);
  await client.connect();
} catch (error) {
  if (error.message.includes('apiUrl is required')) {
    console.error('Missing API URL in configuration');
  } else if (error.message.includes('authToken or authManager')) {
    console.error('Authentication not configured');
  }
  // Handle error appropriately
}
```

### 8. Resource Cleanup

**Always clean up resources:**

```typescript
// On component unmount or app shutdown
client.disconnect();
authManager.dispose();
voiceManager.reset();
```

---

## Configuration Validation

The SDK validates configuration and provides clear error messages:

```typescript
// Missing required field
new RealtimeClient({});
// Error: apiUrl is required in RealtimeClientConfig

// Missing authentication
new RealtimeClient({
  apiUrl: 'wss://api.example.com/rt/ws'
});
// Error: Either authToken or authManager is required in RealtimeClientConfig

// Invalid audio configuration
new RealtimeClient({
  apiUrl: 'wss://api.example.com/rt/ws',
  authToken: 'token',
  audioConfig: {
    sampleRate: -1  // Invalid
  }
});
// Error: Invalid audio sample rate
```

---

## TypeScript Support

All configuration interfaces are fully typed for excellent IDE support:

```typescript
import type { 
  RealtimeClientConfig,
  AudioConfig,
  ReconnectionConfig,
  ConnectionState 
} from '@agentc/realtime-core';

// TypeScript will provide autocomplete and type checking
const config: RealtimeClientConfig = {
  apiUrl: 'wss://api.example.com/rt/ws',
  authToken: 'token',
  // IDE will suggest all available options
};
```

---

## Migration Guide

### From v0.x to v1.0

If migrating from an earlier version:

```typescript
// Old configuration
const client = new RealtimeClient({
  wsUrl: 'wss://api.example.com/ws',  // Changed
  token: 'jwt-token',                  // Changed
  reconnect: true                      // Changed
});

// New configuration
const client = new RealtimeClient({
  apiUrl: 'wss://api.example.com/rt/ws',  // Now apiUrl
  authToken: 'jwt-token',                  // Now authToken
  autoReconnect: true                      // Now autoReconnect
});
```

---

## Troubleshooting

### Common Configuration Issues

**Connection timeouts:**
```typescript
// Increase timeout for slow networks
connectionTimeout: 20000  // 20 seconds
```

**Audio not working:**
```typescript
// Ensure audio is enabled and permissions granted
enableAudio: true,
audioConfig: {
  enableInput: true,
  enableOutput: true
}
```

**Token expiration:**
```typescript
// Use AuthManager with auto-refresh
authManager: new AuthManager({
  autoRefresh: true,
  refreshBufferMs: 120000  // Refresh 2 minutes before expiry
})
```

**Reconnection storms:**
```typescript
// Add backoff and jitter
reconnection: {
  backoffMultiplier: 2,
  jitterFactor: 0.5
}
```

---

## See Also

- [RealtimeClient API Reference](./RealtimeClient.md)
- [AuthManager API Reference](./AuthManager.md)
- [AudioService API Reference](./AudioService.md)
- [Event Types Reference](./EventTypes.md)
- [Migration Guide](../guides/migration.md)
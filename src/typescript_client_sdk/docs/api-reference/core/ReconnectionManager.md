# ReconnectionManager API Reference

The `ReconnectionManager` class manages WebSocket reconnection attempts with exponential backoff strategy, jitter, and configurable retry limits.

## Import

```typescript
import { ReconnectionManager } from '@agentc/realtime-core';
```

## Overview

The ReconnectionManager provides a robust reconnection mechanism for WebSocket connections, implementing exponential backoff with jitter to prevent thundering herd problems. It handles automatic retry attempts and provides fine-grained control over reconnection behavior.

## Constructor

```typescript
constructor(config: ReconnectionConfig)
```

Creates a new ReconnectionManager instance.

### Parameters

- `config` (ReconnectionConfig) - Configuration object for reconnection behavior

### Configuration Options

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

### Default Configuration

```typescript
const defaultReconnectionConfig: ReconnectionConfig = {
  enabled: true,
  initialDelay: 1000,      // 1 second
  maxDelay: 30000,         // 30 seconds
  backoffMultiplier: 1.5,  // 1.5x increase each attempt
  maxAttempts: 0,          // Unlimited attempts
  jitterFactor: 0.3        // 30% jitter
};
```

### Example

```typescript
const reconnectionManager = new ReconnectionManager({
  enabled: true,
  initialDelay: 1000,
  maxDelay: 30000,
  backoffMultiplier: 2,
  maxAttempts: 10,
  jitterFactor: 0.3
});
```

## Core Methods

### startReconnection()

Starts the reconnection process with the provided reconnection function.

```typescript
async startReconnection(
  reconnectFn: () => Promise<void>
): Promise<void>
```

**Parameters:**
- `reconnectFn` (Function) - Async function that attempts to reconnect

**Returns:** Promise that resolves when successfully reconnected

**Throws:** 
- Error if reconnection is disabled
- Error if reconnection already in progress
- Error if maximum attempts reached

**Example:**
```typescript
try {
  await reconnectionManager.startReconnection(async () => {
    await client.connect();
  });
  console.log('Successfully reconnected');
} catch (error) {
  console.error('Reconnection failed:', error.message);
}
```

### stopReconnection()

Stops any ongoing reconnection attempts immediately.

```typescript
stopReconnection(): void
```

**Example:**
```typescript
// Stop reconnection on user request
reconnectionManager.stopReconnection();
console.log('Reconnection stopped');
```

### reset()

Resets the reconnection state to initial values.

```typescript
reset(): void
```

**Note:** This resets attempt counter and delay to initial values

**Example:**
```typescript
// Reset after successful manual reconnection
reconnectionManager.reset();
```

## Status Methods

### isActive()

Checks if reconnection is currently in progress.

```typescript
isActive(): boolean
```

**Returns:** `true` if actively reconnecting, `false` otherwise

**Example:**
```typescript
if (reconnectionManager.isActive()) {
  console.log('Reconnection in progress...');
  showReconnectingSpinner();
}
```

### getCurrentAttempt()

Gets the current reconnection attempt number.

```typescript
getCurrentAttempt(): number
```

**Returns:** Current attempt number (0 if not reconnecting)

**Example:**
```typescript
const attempt = reconnectionManager.getCurrentAttempt();
console.log(`Reconnection attempt ${attempt} of ${config.maxAttempts}`);
```

### shouldRetry()

Determines if another reconnection attempt should be made.

```typescript
shouldRetry(): boolean
```

**Returns:** `true` if should retry, `false` otherwise

**Example:**
```typescript
if (reconnectionManager.shouldRetry()) {
  console.log('Will attempt to reconnect again');
} else {
  console.log('No more reconnection attempts');
}
```

### getNextDelay()

Calculates the delay for the next reconnection attempt.

```typescript
getNextDelay(): number
```

**Returns:** Delay in milliseconds with jitter applied

**Example:**
```typescript
const nextDelay = reconnectionManager.getNextDelay();
console.log(`Next attempt in ${nextDelay}ms`);
```

## Configuration Management

### updateConfig()

Updates the reconnection configuration.

```typescript
updateConfig(config: Partial<ReconnectionConfig>): void
```

**Parameters:**
- `config` (Partial<ReconnectionConfig>) - Configuration properties to update

**Note:** If reconnection is disabled while actively reconnecting, it will stop immediately

**Example:**
```typescript
// Increase max attempts
reconnectionManager.updateConfig({
  maxAttempts: 20
});

// Disable reconnection
reconnectionManager.updateConfig({
  enabled: false
});
```

### getConfig()

Gets a copy of the current configuration.

```typescript
getConfig(): ReconnectionConfig
```

**Returns:** Copy of current configuration

**Example:**
```typescript
const config = reconnectionManager.getConfig();
console.log('Max attempts:', config.maxAttempts);
console.log('Backoff multiplier:', config.backoffMultiplier);
```

## Event Handling

ReconnectionManager extends EventEmitter and emits the following events:

### Events

#### reconnecting

Emitted when a reconnection attempt is about to start.

```typescript
interface ReconnectingEvent {
  attempt: number;  // Current attempt number
  delay: number;    // Delay before this attempt (ms)
}
```

**Example:**
```typescript
reconnectionManager.on('reconnecting', ({ attempt, delay }) => {
  console.log(`Reconnection attempt ${attempt} starting in ${delay}ms`);
  updateUIStatus(`Reconnecting... (Attempt ${attempt})`);
});
```

#### reconnected

Emitted when reconnection succeeds.

```typescript
// Event data: void (no data)
```

**Example:**
```typescript
reconnectionManager.on('reconnected', () => {
  console.log('✅ Successfully reconnected!');
  hideReconnectingUI();
});
```

#### reconnection_failed

Emitted when reconnection fails after all attempts.

```typescript
interface ReconnectionFailedEvent {
  attempts: number;  // Total attempts made
  reason: string;    // Failure reason
}
```

**Example:**
```typescript
reconnectionManager.on('reconnection_failed', ({ attempts, reason }) => {
  console.error(`❌ Reconnection failed after ${attempts} attempts: ${reason}`);
  showReconnectionFailedUI(reason);
});
```

## Complete Example

```typescript
import { ReconnectionManager, RealtimeClient } from '@agentc/realtime-core';

class RobustWebSocketClient {
  private client: RealtimeClient;
  private reconnectionManager: ReconnectionManager;
  
  constructor() {
    // Initialize reconnection manager with custom config
    this.reconnectionManager = new ReconnectionManager({
      enabled: true,
      initialDelay: 1000,      // Start with 1 second
      maxDelay: 60000,         // Cap at 1 minute
      backoffMultiplier: 2,    // Double delay each time
      maxAttempts: 15,         // Try 15 times
      jitterFactor: 0.3        // Add 30% randomization
    });
    
    // Set up event listeners
    this.setupEventListeners();
    
    // Initialize client
    this.client = new RealtimeClient({
      apiUrl: 'wss://localhost:8000/rt/ws',
      autoReconnect: false  // We'll handle it manually
    });
  }
  
  private setupEventListeners(): void {
    // Track reconnection progress
    this.reconnectionManager.on('reconnecting', ({ attempt, delay }) => {
      console.log(`🔄 Reconnection attempt ${attempt} in ${delay}ms`);
      
      // Update UI
      this.updateStatus('reconnecting', {
        attempt,
        delay: Math.round(delay / 1000)
      });
    });
    
    // Handle successful reconnection
    this.reconnectionManager.on('reconnected', () => {
      console.log('✅ Reconnection successful!');
      this.updateStatus('connected');
      
      // Re-subscribe to events, restore state, etc.
      this.restoreConnectionState();
    });
    
    // Handle failed reconnection
    this.reconnectionManager.on('reconnection_failed', ({ attempts, reason }) => {
      console.error(`❌ Failed to reconnect after ${attempts} attempts`);
      console.error(`Reason: ${reason}`);
      
      this.updateStatus('failed', { attempts, reason });
      
      // Offer manual retry option
      this.showManualRetryOption();
    });
    
    // Handle client disconnection
    this.client.on('disconnected', (event) => {
      console.warn('WebSocket disconnected:', event);
      
      // Don't reconnect for clean disconnections
      if (event.code === 1000) {
        console.log('Clean disconnect, not reconnecting');
        return;
      }
      
      // Start automatic reconnection
      this.handleDisconnection();
    });
  }
  
  async connect(): Promise<void> {
    try {
      await this.client.connect();
      console.log('Initial connection successful');
      this.updateStatus('connected');
    } catch (error) {
      console.error('Initial connection failed:', error);
      this.handleDisconnection();
    }
  }
  
  private async handleDisconnection(): Promise<void> {
    console.log('Starting reconnection process...');
    
    try {
      await this.reconnectionManager.startReconnection(async () => {
        // This function will be called for each attempt
        await this.client.connect();
      });
    } catch (error) {
      console.error('All reconnection attempts exhausted:', error);
    }
  }
  
  private updateStatus(status: string, data?: any): void {
    // Update UI status indicator
    const statusElement = document.getElementById('connection-status');
    if (statusElement) {
      switch (status) {
        case 'connected':
          statusElement.className = 'status-connected';
          statusElement.textContent = 'Connected';
          break;
        case 'reconnecting':
          statusElement.className = 'status-reconnecting';
          statusElement.textContent = `Reconnecting (Attempt ${data.attempt}, ${data.delay}s)`;
          break;
        case 'failed':
          statusElement.className = 'status-failed';
          statusElement.textContent = `Connection failed after ${data.attempts} attempts`;
          break;
      }
    }
  }
  
  private restoreConnectionState(): void {
    // Restore any necessary state after reconnection
    console.log('Restoring connection state...');
    
    // Re-join rooms, re-subscribe to channels, etc.
    // This depends on your application's needs
  }
  
  private showManualRetryOption(): void {
    // Show UI for manual retry
    const retryButton = document.getElementById('retry-button');
    if (retryButton) {
      retryButton.style.display = 'block';
      retryButton.onclick = () => this.manualReconnect();
    }
  }
  
  async manualReconnect(): Promise<void> {
    console.log('Manual reconnection requested');
    
    // Reset the manager to start fresh
    this.reconnectionManager.reset();
    
    // Try to reconnect
    await this.handleDisconnection();
  }
  
  disconnect(): void {
    // Stop any ongoing reconnection
    this.reconnectionManager.stopReconnection();
    
    // Disconnect client
    this.client.disconnect();
    
    console.log('Disconnected and reconnection stopped');
  }
  
  // Configuration methods
  
  setMaxReconnectAttempts(attempts: number): void {
    this.reconnectionManager.updateConfig({
      maxAttempts: attempts
    });
  }
  
  disableReconnection(): void {
    this.reconnectionManager.updateConfig({
      enabled: false
    });
  }
  
  enableReconnection(): void {
    this.reconnectionManager.updateConfig({
      enabled: true
    });
  }
  
  getReconnectionStatus(): {
    isActive: boolean;
    currentAttempt: number;
    shouldRetry: boolean;
  } {
    return {
      isActive: this.reconnectionManager.isActive(),
      currentAttempt: this.reconnectionManager.getCurrentAttempt(),
      shouldRetry: this.reconnectionManager.shouldRetry()
    };
  }
}

// Usage
async function main() {
  const client = new RobustWebSocketClient();
  
  // Connect with automatic reconnection
  await client.connect();
  
  // Later, adjust reconnection settings
  client.setMaxReconnectAttempts(20);
  
  // Check status
  const status = client.getReconnectionStatus();
  console.log('Reconnection status:', status);
  
  // Manually trigger reconnection if needed
  if (!status.isActive) {
    await client.manualReconnect();
  }
  
  // Clean shutdown
  process.on('SIGINT', () => {
    client.disconnect();
    process.exit(0);
  });
}

main().catch(console.error);
```

## Exponential Backoff Algorithm

The ReconnectionManager implements exponential backoff with jitter:

```typescript
// Backoff calculation example
// Initial delay: 1000ms, Multiplier: 2, Max: 30000ms

Attempt 1: 1000ms  (±30% jitter → 700-1300ms)
Attempt 2: 2000ms  (±30% jitter → 1400-2600ms)
Attempt 3: 4000ms  (±30% jitter → 2800-5200ms)
Attempt 4: 8000ms  (±30% jitter → 5600-10400ms)
Attempt 5: 16000ms (±30% jitter → 11200-20800ms)
Attempt 6: 30000ms (capped, ±30% jitter → 21000-30000ms)
Attempt 7+: 30000ms (remains at max)
```

### Jitter Calculation

Jitter prevents thundering herd by randomizing delays:

```typescript
// With jitterFactor = 0.3 (30%)
const jitter = currentDelay * jitterFactor;
const minDelay = currentDelay - jitter;
const maxDelay = currentDelay + jitter;
const actualDelay = Math.floor(minDelay + Math.random() * (maxDelay - minDelay));
```

## Error Handling

The ReconnectionManager can throw or emit errors for:

1. **Configuration Errors**
   - Reconnection disabled when attempting to start
   - Reconnection already in progress

2. **Retry Exhaustion**
   - Maximum attempts reached
   - Reconnection cancelled or aborted

3. **Connection Failures**
   - Individual reconnection attempts failing
   - Network timeouts

### Example Error Handling

```typescript
// Handle reconnection errors
try {
  await reconnectionManager.startReconnection(async () => {
    await client.connect();
  });
} catch (error) {
  if (error.message.includes('disabled')) {
    console.log('Reconnection is disabled');
  } else if (error.message.includes('Maximum reconnection attempts')) {
    console.log('All retry attempts exhausted');
    // Offer manual retry or alternative connection method
  } else if (error.message.includes('cancelled')) {
    console.log('Reconnection was cancelled');
  } else {
    console.error('Unexpected error:', error);
  }
}

// Monitor failures via events
reconnectionManager.on('reconnection_failed', ({ attempts, reason }) => {
  // Log to error tracking service
  errorTracker.log({
    event: 'reconnection_failed',
    attempts,
    reason,
    timestamp: Date.now()
  });
  
  // Update UI
  showErrorNotification(`Connection failed after ${attempts} attempts`);
  
  // Potentially try alternative connection method
  if (attempts >= 10) {
    tryAlternativeConnection();
  }
});
```

## Integration with RealtimeClient

The ReconnectionManager is automatically used by RealtimeClient when configured:

```typescript
const client = new RealtimeClient({
  apiUrl: 'wss://localhost:8000/rt/ws',
  autoReconnect: true,  // Enables automatic reconnection
  reconnection: {
    enabled: true,
    initialDelay: 1000,
    maxDelay: 30000,
    backoffMultiplier: 1.5,
    maxAttempts: 0,  // Unlimited
    jitterFactor: 0.3
  }
});

// The client automatically uses ReconnectionManager internally
client.on('reconnecting', ({ attempt, delay }) => {
  console.log(`Client reconnecting: attempt ${attempt}, delay ${delay}ms`);
});

client.on('reconnected', () => {
  console.log('Client reconnected successfully');
});

client.on('disconnected', ({ code, reason }) => {
  if (code !== 1000) {  // Not a clean disconnect
    console.log('Client will attempt to reconnect...');
  }
});
```

## Best Practices

1. **Configure appropriate retry limits:**
```typescript
// For user-facing applications
const config = {
  enabled: true,
  maxAttempts: 10,  // Don't retry forever
  maxDelay: 30000   // Cap at 30 seconds
};

// For backend services
const config = {
  enabled: true,
  maxAttempts: 0,   // Retry indefinitely
  maxDelay: 60000   // Cap at 1 minute
};
```

2. **Use jitter to prevent thundering herd:**
```typescript
// Always use some jitter (0.2 to 0.5 recommended)
const config = {
  jitterFactor: 0.3  // 30% randomization
};
```

3. **Handle user-initiated disconnections:**
```typescript
let userDisconnected = false;

function disconnect() {
  userDisconnected = true;
  reconnectionManager.stopReconnection();
  client.disconnect();
}

client.on('disconnected', () => {
  if (!userDisconnected) {
    // Only reconnect for unexpected disconnections
    startReconnection();
  }
});
```

4. **Provide user feedback during reconnection:**
```typescript
reconnectionManager.on('reconnecting', ({ attempt, delay }) => {
  const seconds = Math.ceil(delay / 1000);
  updateUI({
    status: 'reconnecting',
    message: `Reconnecting in ${seconds} seconds... (Attempt ${attempt})`,
    showSpinner: true
  });
});
```

5. **Implement circuit breaker pattern:**
```typescript
class ConnectionCircuitBreaker {
  private failures = 0;
  private lastFailureTime = 0;
  private readonly threshold = 5;
  private readonly resetTime = 60000; // 1 minute
  
  shouldAttemptConnection(): boolean {
    // Reset failures if enough time has passed
    if (Date.now() - this.lastFailureTime > this.resetTime) {
      this.failures = 0;
    }
    
    return this.failures < this.threshold;
  }
  
  recordFailure(): void {
    this.failures++;
    this.lastFailureTime = Date.now();
  }
  
  recordSuccess(): void {
    this.failures = 0;
  }
}

const breaker = new ConnectionCircuitBreaker();

reconnectionManager.on('reconnection_failed', () => {
  breaker.recordFailure();
});

reconnectionManager.on('reconnected', () => {
  breaker.recordSuccess();
});
```

6. **Log reconnection metrics:**
```typescript
const metrics = {
  reconnectionAttempts: 0,
  successfulReconnections: 0,
  failedReconnections: 0,
  totalReconnectionTime: 0,
  lastReconnectionTime: 0
};

reconnectionManager.on('reconnecting', ({ attempt }) => {
  if (attempt === 1) {
    metrics.lastReconnectionTime = Date.now();
  }
  metrics.reconnectionAttempts++;
});

reconnectionManager.on('reconnected', () => {
  metrics.successfulReconnections++;
  metrics.totalReconnectionTime += Date.now() - metrics.lastReconnectionTime;
  
  console.log('Reconnection metrics:', metrics);
});

reconnectionManager.on('reconnection_failed', () => {
  metrics.failedReconnections++;
});
```

## TypeScript Types

```typescript
import {
  ReconnectionManager,
  ReconnectionConfig,
  ReconnectionEvents
} from '@agentc/realtime-core';

// Event types
interface ReconnectingEvent {
  attempt: number;
  delay: number;
}

interface ReconnectionFailedEvent {
  attempts: number;
  reason: string;
}

// Full event map
interface ReconnectionEvents {
  'reconnecting': ReconnectingEvent;
  'reconnected': void;
  'reconnection_failed': ReconnectionFailedEvent;
}
```

All methods and events are fully typed for TypeScript applications.
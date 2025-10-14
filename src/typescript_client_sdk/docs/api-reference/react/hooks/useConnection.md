# useConnection

## Purpose and Overview

The `useConnection` hook provides a comprehensive interface for managing and monitoring WebSocket connections in your React application. It offers real-time connection state tracking, connection control methods, detailed connection statistics, and automatic reconnection management.

This hook is essential for:
- Monitoring connection health and status
- Managing connection lifecycle (connect, disconnect, reconnect)
- Tracking connection statistics and performance metrics
- Handling connection errors gracefully
- Implementing connection-aware UI components

## Import Statement

```typescript
import { useConnection } from '@agentc/realtime-react';
```

## Full TypeScript Signatures and Types

### Hook Signature

```typescript
function useConnection(): UseConnectionReturn
```

### Return Type

```typescript
interface UseConnectionReturn {
  /** Current connection state */
  connectionState: ConnectionState;
  
  /** Whether currently connected */
  isConnected: boolean;
  
  /** Whether currently connecting */
  isConnecting: boolean;
  
  /** Whether connection is closed */
  isDisconnected: boolean;
  
  /** Whether client is initializing */
  isInitializing: boolean;
  
  /** Connect to the server */
  connect: () => Promise<void>;
  
  /** Disconnect from the server */
  disconnect: () => void;
  
  /** Reconnect to the server */
  reconnect: () => Promise<void>;
  
  /** Connection error if any */
  error: Error | null;
  
  /** Connection statistics */
  stats: ConnectionStats;
  
  /** Whether auto-reconnect is enabled */
  autoReconnectEnabled: boolean;
  
  /** Current reconnect attempt number */
  reconnectAttempt: number;
  
  /** Maximum reconnect attempts */
  maxReconnectAttempts: number;
}
```

### Connection State Enum

```typescript
enum ConnectionState {
  DISCONNECTED = 'DISCONNECTED',
  CONNECTING = 'CONNECTING',
  CONNECTED = 'CONNECTED',
  RECONNECTING = 'RECONNECTING'
}
```

### Connection Statistics Type

```typescript
interface ConnectionStats {
  /** Number of connection attempts */
  connectionAttempts: number;
  
  /** Number of successful connections */
  successfulConnections: number;
  
  /** Number of failed connections */
  failedConnections: number;
  
  /** Last connection time */
  lastConnectedAt: Date | null;
  
  /** Last disconnection time */
  lastDisconnectedAt: Date | null;
  
  /** Current session duration in milliseconds */
  sessionDuration: number;
  
  /** Current latency in milliseconds */
  latency: number;
  
  /** Total messages received */
  messagesReceived: number;
  
  /** Total messages sent */
  messagesSent: number;
  
  /** Total bytes received */
  bytesReceived: number;
  
  /** Total bytes sent */
  bytesSent: number;
}
```

### Helper Types

```typescript
// String literal type for connection state (lowercase version)
type ConnectionStateString = 'connected' | 'disconnected' | 'connecting' | 'reconnecting' | 'error';

// Helper function to convert ConnectionState enum to string literal
function getConnectionStateString(state: ConnectionState): ConnectionStateString
```

## Detailed Return Value Descriptions

### Connection State Properties

- **`connectionState`**: The current state of the WebSocket connection as an enum value. Use this for precise state checking.

- **`isConnected`**: Boolean flag indicating if the connection is fully established and ready for communication.

- **`isConnecting`**: Boolean flag indicating if the connection is currently being established (initial connection attempt).

- **`isDisconnected`**: Boolean flag indicating if the connection is closed or not established.

- **`isInitializing`**: Boolean flag indicating if the client context is still initializing. Inherited from the AgentC provider context.

### Connection Control Methods

- **`connect()`**: Asynchronously establishes a WebSocket connection to the server. Returns a Promise that resolves when connected or rejects with an error.

- **`disconnect()`**: Synchronously closes the WebSocket connection. Safe to call even if already disconnected.

- **`reconnect()`**: Disconnects if connected, waits briefly (100ms), then attempts to reconnect. Useful for manual connection recovery.

### Connection Monitoring

- **`error`**: The most recent connection error, if any. Automatically cleared on successful connection.

- **`stats`**: Comprehensive connection statistics including attempt counts, timing information, and data transfer metrics. Session duration updates every second while connected.

### Reconnection Configuration

- **`autoReconnectEnabled`**: Whether automatic reconnection is enabled in the client configuration.

- **`reconnectAttempt`**: The current reconnection attempt number during reconnection sequences.

- **`maxReconnectAttempts`**: Maximum number of reconnection attempts before giving up.

## Multiple Usage Examples

### Basic Connection Management

```tsx
function ConnectionManager() {
  const { 
    isConnected, 
    isConnecting, 
    connect, 
    disconnect, 
    error 
  } = useConnection();

  return (
    <div>
      <p>Status: {isConnected ? 'Connected' : 'Disconnected'}</p>
      
      {error && (
        <div className="error">
          Connection error: {error.message}
        </div>
      )}
      
      {!isConnected && !isConnecting && (
        <button onClick={connect}>Connect</button>
      )}
      
      {isConnecting && (
        <div>Connecting...</div>
      )}
      
      {isConnected && (
        <button onClick={disconnect}>Disconnect</button>
      )}
    </div>
  );
}
```

### Connection Status Indicator

```tsx
function ConnectionStatusIndicator() {
  const { connectionState, reconnectAttempt, maxReconnectAttempts } = useConnection();
  
  const getStatusColor = () => {
    switch (connectionState) {
      case ConnectionState.CONNECTED:
        return 'green';
      case ConnectionState.CONNECTING:
        return 'yellow';
      case ConnectionState.RECONNECTING:
        return 'orange';
      case ConnectionState.DISCONNECTED:
        return 'red';
      default:
        return 'gray';
    }
  };
  
  const getStatusText = () => {
    if (connectionState === ConnectionState.RECONNECTING) {
      return `Reconnecting (${reconnectAttempt}/${maxReconnectAttempts})`;
    }
    return connectionState;
  };
  
  return (
    <div style={{ color: getStatusColor() }}>
      {getStatusText()}
    </div>
  );
}
```

### Connection Statistics Dashboard

```tsx
function ConnectionStatsDashboard() {
  const { stats, isConnected } = useConnection();
  
  const formatDuration = (ms: number) => {
    const seconds = Math.floor(ms / 1000);
    const minutes = Math.floor(seconds / 60);
    const hours = Math.floor(minutes / 60);
    
    if (hours > 0) {
      return `${hours}h ${minutes % 60}m`;
    } else if (minutes > 0) {
      return `${minutes}m ${seconds % 60}s`;
    } else {
      return `${seconds}s`;
    }
  };
  
  return (
    <div className="stats-dashboard">
      <h3>Connection Statistics</h3>
      
      <div className="stats-grid">
        <div>
          <label>Connection Attempts:</label>
          <span>{stats.connectionAttempts}</span>
        </div>
        
        <div>
          <label>Successful:</label>
          <span>{stats.successfulConnections}</span>
        </div>
        
        <div>
          <label>Failed:</label>
          <span>{stats.failedConnections}</span>
        </div>
        
        {isConnected && (
          <>
            <div>
              <label>Session Duration:</label>
              <span>{formatDuration(stats.sessionDuration)}</span>
            </div>
            
            <div>
              <label>Latency:</label>
              <span>{stats.latency}ms</span>
            </div>
            
            <div>
              <label>Messages (In/Out):</label>
              <span>{stats.messagesReceived} / {stats.messagesSent}</span>
            </div>
          </>
        )}
        
        {stats.lastConnectedAt && (
          <div>
            <label>Last Connected:</label>
            <span>{stats.lastConnectedAt.toLocaleTimeString()}</span>
          </div>
        )}
      </div>
    </div>
  );
}
```

### Auto-Connect on Mount

```tsx
function AutoConnectApp() {
  const { connect, isConnected, isConnecting, error } = useConnection();
  
  useEffect(() => {
    // Auto-connect on mount if not already connected
    if (!isConnected && !isConnecting) {
      connect().catch(err => {
        console.error('Auto-connect failed:', err);
      });
    }
  }, []); // Empty deps - only run on mount
  
  if (error) {
    return <ErrorBoundary error={error} />;
  }
  
  if (!isConnected) {
    return <LoadingScreen message="Establishing connection..." />;
  }
  
  return <MainApplication />;
}
```

### Manual Reconnection with Retry

```tsx
function ReconnectionHandler() {
  const { 
    isConnected, 
    reconnect, 
    error,
    reconnectAttempt,
    maxReconnectAttempts 
  } = useConnection();
  
  const [isRetrying, setIsRetrying] = useState(false);
  
  const handleManualReconnect = async () => {
    setIsRetrying(true);
    
    try {
      await reconnect();
      // Success - connection restored
    } catch (err) {
      console.error('Manual reconnection failed:', err);
      // Could implement custom retry logic here
    } finally {
      setIsRetrying(false);
    }
  };
  
  if (!isConnected && error) {
    return (
      <div className="reconnection-prompt">
        <p>Connection lost: {error.message}</p>
        
        {reconnectAttempt > 0 && (
          <p>Reconnection attempt {reconnectAttempt} of {maxReconnectAttempts}</p>
        )}
        
        <button 
          onClick={handleManualReconnect}
          disabled={isRetrying}
        >
          {isRetrying ? 'Reconnecting...' : 'Reconnect Now'}
        </button>
      </div>
    );
  }
  
  return null;
}
```

## Best Practices

### 1. Connection Lifecycle Management

```tsx
function AppWithConnectionLifecycle() {
  const { connect, disconnect, isConnected } = useConnection();
  
  useEffect(() => {
    // Connect when app becomes visible
    const handleVisibilityChange = () => {
      if (document.visibilityState === 'visible' && !isConnected) {
        connect();
      }
    };
    
    document.addEventListener('visibilitychange', handleVisibilityChange);
    
    // Cleanup: disconnect on unmount
    return () => {
      document.removeEventListener('visibilitychange', handleVisibilityChange);
      if (isConnected) {
        disconnect();
      }
    };
  }, [isConnected, connect, disconnect]);
  
  return <App />;
}
```

### 2. Error Boundary Integration

```tsx
function ConnectionErrorBoundary({ children }: { children: React.ReactNode }) {
  const { error, reconnect } = useConnection();
  
  if (error) {
    return (
      <ErrorFallback 
        error={error} 
        retry={reconnect}
      />
    );
  }
  
  return <>{children}</>;
}
```

### 3. Connection-Aware Data Fetching

```tsx
function DataComponent() {
  const { isConnected, connectionState } = useConnection();
  const [data, setData] = useState(null);
  
  useEffect(() => {
    // Only fetch data when fully connected
    if (isConnected) {
      fetchData().then(setData);
    } else {
      // Clear stale data when disconnected
      setData(null);
    }
  }, [isConnected]);
  
  return (
    <div>
      {!isConnected && (
        <div>Waiting for connection ({connectionState})...</div>
      )}
      {isConnected && data && (
        <DataDisplay data={data} />
      )}
    </div>
  );
}
```

### 4. Optimistic UI Updates

```tsx
function OptimisticChat() {
  const { isConnected, connectionState, error } = useConnection();
  const [messages, setMessages] = useState([]);
  const [pendingMessages, setPendingMessages] = useState([]);
  
  const sendMessage = (text: string) => {
    const message = { id: Date.now(), text, pending: true };
    
    if (isConnected) {
      // Send immediately
      setPendingMessages(prev => [...prev, message]);
      sendToServer(message);
    } else {
      // Queue for later
      setPendingMessages(prev => [...prev, { ...message, queued: true }]);
    }
  };
  
  // Send queued messages when reconnected
  useEffect(() => {
    if (isConnected && pendingMessages.some(m => m.queued)) {
      pendingMessages.filter(m => m.queued).forEach(sendToServer);
    }
  }, [isConnected]);
  
  return (
    <ChatInterface 
      messages={[...messages, ...pendingMessages]}
      canSend={isConnected || connectionState === ConnectionState.RECONNECTING}
      connectionError={error}
    />
  );
}
```

## StrictMode Compatibility Notes

The `useConnection` hook is fully compatible with React StrictMode. Key considerations:

1. **Effect Cleanup**: All event listeners are properly cleaned up in effect return functions to prevent memory leaks during StrictMode's double-mounting.

2. **State Initialization**: Initial state is derived from the client's current state, ensuring consistency across re-renders.

3. **Idempotent Operations**: Connection methods are idempotent - calling `connect()` when already connected is safe and logs a warning.

4. **No Side Effects in Render**: All side effects are contained within useEffect hooks or event callbacks.

Example StrictMode-safe usage:

```tsx
function StrictModeApp() {
  return (
    <StrictMode>
      <AgentCProvider config={config}>
        <ConnectionManager />
      </AgentCProvider>
    </StrictMode>
  );
}
```

## Integration with Other Hooks

### With useInitializationStatus

```tsx
function ConnectionWithInit() {
  const { isConnected, connect } = useConnection();
  const { isInitialized, waitForInitialization } = useInitializationStatus();
  
  const establishFullConnection = async () => {
    if (!isConnected) {
      await connect();
    }
    await waitForInitialization();
    // Now fully ready
  };
  
  return (
    <button onClick={establishFullConnection}>
      Connect & Initialize
    </button>
  );
}
```

### With useChat

```tsx
function ChatWithConnectionStatus() {
  const { isConnected, stats } = useConnection();
  const { messages, sendMessage } = useChat();
  
  return (
    <div>
      <ConnectionBadge 
        connected={isConnected}
        latency={stats.latency}
      />
      <ChatMessages 
        messages={messages}
        disabled={!isConnected}
      />
      <MessageInput 
        onSend={sendMessage}
        disabled={!isConnected}
        placeholder={!isConnected ? "Connecting..." : "Type a message"}
      />
    </div>
  );
}
```

### With useAudio

```tsx
function AudioWithConnection() {
  const { isConnected } = useConnection();
  const { startRecording, isRecording } = useAudio();
  
  const handleRecordClick = () => {
    if (!isConnected) {
      alert('Please wait for connection');
      return;
    }
    
    if (isRecording) {
      stopRecording();
    } else {
      startRecording();
    }
  };
  
  return (
    <button 
      onClick={handleRecordClick}
      disabled={!isConnected}
    >
      {isRecording ? 'Stop' : 'Start'} Recording
    </button>
  );
}
```

## Error Handling Patterns

### Comprehensive Error Recovery

```tsx
function RobustConnection() {
  const { 
    connect, 
    disconnect,
    reconnect,
    error, 
    isConnected,
    reconnectAttempt,
    maxReconnectAttempts 
  } = useConnection();
  
  const [userDisconnected, setUserDisconnected] = useState(false);
  
  const handleConnect = async () => {
    setUserDisconnected(false);
    try {
      await connect();
    } catch (err) {
      // Log for debugging
      console.error('Connection failed:', err);
      
      // Determine error type and response
      if (err.message.includes('unauthorized')) {
        // Auth error - need new token
        await refreshAuthToken();
        await connect(); // Retry with new token
      } else if (err.message.includes('network')) {
        // Network error - wait and retry
        setTimeout(() => reconnect(), 5000);
      } else {
        // Unknown error - surface to user
        throw err;
      }
    }
  };
  
  const handleDisconnect = () => {
    setUserDisconnected(true);
    disconnect();
  };
  
  // Auto-reconnect unless user explicitly disconnected
  useEffect(() => {
    if (!isConnected && !userDisconnected && error) {
      if (reconnectAttempt < maxReconnectAttempts) {
        const delay = Math.min(1000 * Math.pow(2, reconnectAttempt), 30000);
        const timer = setTimeout(() => reconnect(), delay);
        return () => clearTimeout(timer);
      }
    }
  }, [error, reconnectAttempt, userDisconnected]);
  
  return (
    <ConnectionUI 
      onConnect={handleConnect}
      onDisconnect={handleDisconnect}
      error={error}
      isConnected={isConnected}
    />
  );
}
```

### Error Categorization

```tsx
function categorizeError(error: Error): 'auth' | 'network' | 'server' | 'unknown' {
  const message = error.message.toLowerCase();
  
  if (message.includes('unauthorized') || message.includes('forbidden')) {
    return 'auth';
  }
  if (message.includes('network') || message.includes('offline')) {
    return 'network';
  }
  if (message.includes('500') || message.includes('server')) {
    return 'server';
  }
  
  return 'unknown';
}

function SmartErrorHandler() {
  const { error, reconnect } = useConnection();
  
  if (!error) return null;
  
  const errorType = categorizeError(error);
  
  switch (errorType) {
    case 'auth':
      return <AuthRefreshPrompt />;
    case 'network':
      return <NetworkErrorMessage onRetry={reconnect} />;
    case 'server':
      return <ServerErrorMessage supportUrl="/support" />;
    default:
      return <GenericErrorMessage error={error} onRetry={reconnect} />;
  }
}
```

## Performance Considerations

1. **Statistics Polling**: Session duration updates every second while connected. This is optimized to only run when needed.

2. **Event Subscriptions**: The hook subscribes to multiple events but properly cleans them up. No memory leaks.

3. **Memoization**: Connection control methods are memoized with useCallback to prevent unnecessary re-renders in child components.

4. **Conditional Rendering**: Use connection state booleans for efficient conditional rendering instead of checking the enum value.

## Notes on Implementation

- **Message Statistics**: Currently, message and byte transfer statistics remain at 0 as the core library doesn't yet emit the required tracking events (`message:sent`, `message:received`, `latency:update`). These will be functional once the core events are implemented.

- **Configuration Access**: The hook uses default values for `autoReconnectEnabled` and `maxReconnectAttempts` as the client doesn't expose a `getConfig()` method. These match the defaults from the core library.

- **Error Normalization**: Connection errors are normalized to Error instances to ensure consistent error handling across the application.

- **Session Timing**: Session duration tracking starts on connection and resets on disconnection, providing accurate session metrics.
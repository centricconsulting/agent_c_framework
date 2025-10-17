# useRealtimeClient Hook

## Purpose and Overview

The `useRealtimeClient` hook provides direct access to the underlying `RealtimeClient` instance from the React context. This hook is designed for advanced use cases where you need to interact directly with the client's methods and properties that may not be exposed through other specialized hooks.

This hook enforces strict validation and will throw errors if:
- Used outside of an `AgentCProvider` context
- The provider encountered an error during initialization
- The client is still initializing
- The client is not available due to configuration issues

For safer access that returns `null` instead of throwing errors, consider using `useRealtimeClientSafe()`.

## Import Statement

```typescript
import { useRealtimeClient } from '@agentc/realtime-react';
```

## TypeScript Signature

```typescript
function useRealtimeClient(): RealtimeClient
```

### Return Type

Returns a `RealtimeClient` instance from `@agentc/realtime-core`.

### Throws

- `Error` - When used outside of an `AgentCProvider`
- `Error` - When the provider's initialization failed
- `Error` - When the client is still initializing
- `Error` - When the client is not available

## Detailed Return Value Description

The returned `RealtimeClient` instance provides full access to the SDK's core functionality:

### Key Properties and Methods

```typescript
interface RealtimeClient {
  // Connection Management
  connect(): Promise<void>;
  disconnect(): void;
  isConnected(): boolean;
  
  // Event System
  on<T>(event: string, handler: (data: T) => void): void;
  off(event: string, handler: Function): void;
  once<T>(event: string, handler: (data: T) => void): void;
  emit(event: string, data?: any): void;
  
  // Message Sending
  send(type: string, data?: any): void;
  sendText(text: string): void;
  
  // Audio Control
  startRecording(): Promise<void>;
  stopRecording(): Promise<void>;
  setMuted(muted: boolean): void;
  
  // Session Management
  getSessionManager(): SessionManager;
  getTurnManager(): TurnManager;
  getVoiceManager(): VoiceManager;
  getAvatarManager(): AvatarManager;
  getAuthManager(): AuthManager;
  
  // Configuration
  setAuthToken(token: string): void;
  setAuthManager(authManager: AuthManager): void;
  
  // Cleanup
  destroy(): void;
}
```

## Usage Examples

### Basic Usage

```tsx
import { useRealtimeClient } from '@agentc/realtime-react';

function CustomControl() {
  const client = useRealtimeClient();
  
  const handleCustomAction = () => {
    // Direct access to client methods
    client.send('custom:action', {
      timestamp: Date.now(),
      data: 'custom payload'
    });
  };
  
  return (
    <button onClick={handleCustomAction}>
      Send Custom Event
    </button>
  );
}
```

### Advanced Event Handling

```tsx
import { useEffect } from 'react';
import { useRealtimeClient } from '@agentc/realtime-react';

function EventMonitor() {
  const client = useRealtimeClient();
  
  useEffect(() => {
    // Subscribe to custom events
    const handleCustomEvent = (data: any) => {
      console.log('Received custom event:', data);
    };
    
    const handleError = (error: any) => {
      console.error('Client error:', error);
    };
    
    client.on('custom:response', handleCustomEvent);
    client.on('error', handleError);
    
    // Cleanup subscriptions
    return () => {
      client.off('custom:response', handleCustomEvent);
      client.off('error', handleError);
    };
  }, [client]);
  
  return <div>Monitoring events...</div>;
}
```

### Accessing Manager Instances

```tsx
import { useRealtimeClient } from '@agentc/realtime-react';

function ManagerAccess() {
  const client = useRealtimeClient();
  
  const inspectManagers = () => {
    // Access internal managers for advanced operations
    const sessionManager = client.getSessionManager();
    const turnManager = client.getTurnManager();
    const voiceManager = client.getVoiceManager();
    
    console.log('Current session:', sessionManager.getCurrentSession());
    console.log('Turn state:', turnManager.getCurrentTurn());
    console.log('Active voice:', voiceManager.getCurrentVoice());
  };
  
  return (
    <button onClick={inspectManagers}>
      Inspect Manager States
    </button>
  );
}
```

### Conditional Usage with Error Handling

```tsx
import { useRealtimeClient, useInitializationStatus } from '@agentc/realtime-react';

function SafeClientAccess() {
  const { isInitialized } = useInitializationStatus();
  
  // Only access client after initialization
  if (!isInitialized) {
    return <div>Initializing...</div>;
  }
  
  try {
    const client = useRealtimeClient();
    
    return (
      <div>
        <p>Client connected: {client.isConnected() ? 'Yes' : 'No'}</p>
        <button onClick={() => client.send('ping', {})}>
          Send Ping
        </button>
      </div>
    );
  } catch (error) {
    return <div>Error accessing client: {error.message}</div>;
  }
}
```

## Best Practices

### 1. Prefer Specialized Hooks

For most use cases, prefer the specialized hooks over direct client access:

```tsx
// ❌ Avoid direct client access for common operations
function NotRecommended() {
  const client = useRealtimeClient();
  
  const sendMessage = (text: string) => {
    client.sendText(text); // Direct client method
  };
}

// ✅ Use specialized hooks instead
function Recommended() {
  const { sendMessage } = useChat();
  
  // Specialized hook handles edge cases and state management
}
```

### 2. Always Handle Errors

The hook throws errors, so always handle them appropriately:

```tsx
function RobustComponent() {
  try {
    const client = useRealtimeClient();
    // Use client...
  } catch (error) {
    // Handle initialization errors gracefully
    return <ErrorFallback error={error} />;
  }
}
```

### 3. Check Initialization Status First

Verify initialization before attempting to use the client:

```tsx
function InitAwareComponent() {
  const { isInitialized, error } = useInitializationStatus();
  
  if (error) return <ErrorDisplay error={error} />;
  if (!isInitialized) return <LoadingSpinner />;
  
  // Now safe to use useRealtimeClient()
  const client = useRealtimeClient();
  return <ClientFeature client={client} />;
}
```

### 4. Clean Up Event Listeners

Always remove event listeners to prevent memory leaks:

```tsx
function EventSubscriber() {
  const client = useRealtimeClient();
  
  useEffect(() => {
    const handler = (data: any) => {
      // Handle event
    };
    
    client.on('event', handler);
    
    // Critical: Clean up on unmount or dependency change
    return () => {
      client.off('event', handler);
    };
  }, [client]);
}
```

### 5. Use Memoization for Derived Values

```tsx
import { useMemo } from 'react';

function DerivedState() {
  const client = useRealtimeClient();
  
  const clientInfo = useMemo(() => ({
    connected: client.isConnected(),
    hasAuth: !!client.getAuthManager().getToken(),
    sessionId: client.getSessionManager().getCurrentSession()?.id
  }), [client]);
  
  return <pre>{JSON.stringify(clientInfo, null, 2)}</pre>;
}
```

## StrictMode Compatibility Notes

The `useRealtimeClient` hook is fully compatible with React's StrictMode. The underlying `AgentCProvider` uses refs to prevent double initialization during StrictMode's double-mounting behavior:

```tsx
// Safe to use in StrictMode
function App() {
  return (
    <React.StrictMode>
      <AgentCProvider apiUrl="wss://api.example.com">
        <ComponentUsingClient />
      </AgentCProvider>
    </React.StrictMode>
  );
}
```

The provider ensures:
- Only one client instance is created despite double mounting
- Event listeners are properly cleaned up
- No duplicate WebSocket connections are established

## Common Pitfalls to Avoid

### 1. Using Outside Provider Context

```tsx
// ❌ This will throw an error
function WrongUsage() {
  const client = useRealtimeClient(); // Error: Must be within AgentCProvider
  return <div>...</div>;
}

// ✅ Correct usage
function CorrectUsage() {
  return (
    <AgentCProvider apiUrl="...">
      <ComponentThatUsesClient />
    </AgentCProvider>
  );
}
```

### 2. Not Waiting for Initialization

```tsx
// ❌ May throw if client is still initializing
function Impatient() {
  const client = useRealtimeClient();
  
  useEffect(() => {
    client.connect(); // Might fail if not ready
  }, []); // Missing client dependency
}

// ✅ Wait for initialization and include dependencies
function Patient() {
  const { isInitialized } = useInitializationStatus();
  const client = useRealtimeClientSafe(); // Use safe variant
  
  useEffect(() => {
    if (isInitialized && client) {
      client.connect();
    }
  }, [isInitialized, client]);
}
```

### 3. Mutating Client Configuration After Initialization

```tsx
// ❌ Don't modify client config after creation
function BadMutation() {
  const client = useRealtimeClient();
  
  // This won't work as expected
  client.config.apiUrl = 'new-url'; // Config is frozen after initialization
}

// ✅ Use proper methods or recreate provider
function GoodApproach() {
  const client = useRealtimeClient();
  
  // Use proper API methods
  client.setAuthToken('new-token'); // Correct way to update auth
}
```

### 4. Forgetting to Handle Async Operations

```tsx
// ❌ Not handling promises
function FireAndForget() {
  const client = useRealtimeClient();
  
  const handleClick = () => {
    client.connect(); // Returns a promise - should be handled
  };
}

// ✅ Properly handle async operations
function ProperAsync() {
  const client = useRealtimeClient();
  const [isConnecting, setIsConnecting] = useState(false);
  
  const handleClick = async () => {
    try {
      setIsConnecting(true);
      await client.connect();
      console.log('Connected successfully');
    } catch (error) {
      console.error('Connection failed:', error);
    } finally {
      setIsConnecting(false);
    }
  };
}
```

### 5. Creating Circular Dependencies

```tsx
// ❌ Avoid circular effect dependencies
function CircularProblem() {
  const client = useRealtimeClient();
  const [state, setState] = useState();
  
  useEffect(() => {
    client.on('event', (data) => {
      setState(data); // This changes state
      client.send('response', state); // Using stale state
    });
  }, [client, state]); // Circular: state changes trigger effect
}

// ✅ Use refs or callbacks for stable references
function BetterApproach() {
  const client = useRealtimeClient();
  const stateRef = useRef();
  
  useEffect(() => {
    client.on('event', (data) => {
      stateRef.current = data;
      client.send('response', stateRef.current); // Always current
    });
    
    return () => client.off('event');
  }, [client]); // Stable dependency
}
```

## Alternative Hooks

Consider these alternatives based on your use case:

### `useRealtimeClientSafe()`

Returns `null` instead of throwing errors:

```tsx
import { useRealtimeClientSafe } from '@agentc/realtime-react';

function SafeComponent() {
  const client = useRealtimeClientSafe();
  
  if (!client) {
    return <div>Client not available</div>;
  }
  
  // Use client safely
}
```

### `useAgentCContext()`

Access the full context including initialization state:

```tsx
import { useAgentCContext } from '@agentc/realtime-react';

function ContextAware() {
  const { client, isInitializing, error, initialization } = useAgentCContext();
  
  if (isInitializing) return <div>Initializing...</div>;
  if (error) return <div>Error: {error.message}</div>;
  if (!initialization.isInitialized) return <div>Waiting for data...</div>;
  
  // Everything is ready
  return <div>Ready with client: {client ? 'Yes' : 'No'}</div>;
}
```

## Related Hooks

- [`useRealtimeClientSafe()`](./useRealtimeClientSafe.md) - Safe variant that returns null
- [`useAgentCContext()`](./useAgentCContext.md) - Full context access
- [`useConnection()`](./useConnection.md) - Connection management
- [`useChat()`](./useChat.md) - Chat functionality
- [`useAudio()`](./useAudio.md) - Audio controls
- [`useInitializationStatus()`](./useInitializationStatus.md) - Initialization tracking

## See Also

- [AgentCProvider Documentation](../AgentCProvider.md)
- [RealtimeClient API Reference](../../core/RealtimeClient.md)
- [React Integration Guide](../../../guides/react-integration.md)
- [TypeScript Usage](../../../guides/typescript.md)
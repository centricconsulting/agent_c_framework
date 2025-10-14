# useInitializationStatus

## Purpose and Overview

The `useInitializationStatus` hook provides a streamlined interface for tracking the WebSocket initialization sequence in your React application. It monitors the six critical initialization events required for a fully functional Agent C Realtime connection and provides a simple API to determine when your application is ready for user interaction.

This hook is essential for:
- Ensuring all server configuration data is loaded before rendering UI
- Implementing loading screens during the initialization sequence
- Providing error feedback when initialization fails
- Coordinating component rendering with data availability
- Creating initialization-aware data fetching patterns

The initialization sequence tracked includes:
1. **chat_user_data** - User profile and preferences
2. **avatar_list** - Available avatar configurations
3. **voice_list** - Text-to-speech voice options
4. **agent_list** - Available AI agents
5. **tool_catalog** - Available tools and capabilities
6. **chat_session_changed** - Current chat session state

## Import Statement

```typescript
import { useInitializationStatus, ConnectionState } from '@agentc/realtime-react';
```

## Full TypeScript Signatures and Types

### Hook Signature

```typescript
function useInitializationStatus(): UseInitializationStatusReturn
```

### Return Type

```typescript
interface UseInitializationStatusReturn {
  /** Whether the client has completed initialization (all 6 events received) */
  isInitialized: boolean;
  
  /** Whether the client is currently loading/connecting/initializing */
  isLoading: boolean;
  
  /** Error message if initialization failed */
  error: string | null;
  
  /** Current connection state */
  connectionState: ConnectionState;
  
  /** Wait for initialization to complete (returns a promise) */
  waitForInitialization: () => Promise<void>;
}
```

### Connection State Enum (Re-exported)

```typescript
enum ConnectionState {
  DISCONNECTED = 'DISCONNECTED',
  CONNECTING = 'CONNECTING',
  CONNECTED = 'CONNECTED',
  RECONNECTING = 'RECONNECTING'
}
```

## Detailed Return Value Descriptions

### Initialization State Properties

- **`isInitialized`**: Boolean indicating whether all six initialization events have been received from the server. When `true`, all configuration data (users, avatars, voices, agents, tools, session) is available.

- **`isLoading`**: Boolean indicating an active loading state. This is `true` when:
  - The connection is being established (`CONNECTING` state)
  - The connection is established but initialization events are still pending
  - The connection is reconnecting (`RECONNECTING` state)

### Error Handling

- **`error`**: String containing the error message if initialization failed. This is automatically cleared when the connection succeeds. Common error scenarios include:
  - Authentication failures
  - Network timeouts
  - Server errors during initialization
  - WebSocket connection failures

### Connection Monitoring

- **`connectionState`**: The current state of the WebSocket connection. This provides more granular information than the boolean flags and can be used for detailed status displays.

### Asynchronous Waiting

- **`waitForInitialization()`**: Returns a Promise that resolves when initialization is complete. If already initialized, resolves immediately. Throws an error if the client is not available. This is useful for:
  - Async data loading patterns
  - Sequential initialization steps
  - Ensuring data availability before operations

## Multiple Usage Examples

### Basic Loading Screen

```tsx
function App() {
  const { isInitialized, isLoading, error } = useInitializationStatus();
  
  if (error) {
    return (
      <div className="error-screen">
        <h2>Connection Failed</h2>
        <p>{error}</p>
        <button onClick={() => window.location.reload()}>
          Retry
        </button>
      </div>
    );
  }
  
  if (isLoading) {
    return (
      <div className="loading-screen">
        <Spinner />
        <p>Connecting to Agent C...</p>
      </div>
    );
  }
  
  if (!isInitialized) {
    return (
      <div className="initialization-screen">
        <ProgressBar value={50} />
        <p>Loading configuration...</p>
      </div>
    );
  }
  
  return <MainApplication />;
}
```

### Initialization Progress Tracker

```tsx
function InitializationProgress() {
  const { isInitialized, isLoading, connectionState } = useInitializationStatus();
  const [initEvents, setInitEvents] = useState({
    user: false,
    avatars: false,
    voices: false,
    agents: false,
    tools: false,
    session: false
  });
  
  const client = useRealtimeClient();
  
  useEffect(() => {
    if (!client) return;
    
    const handlers = {
      chat_user_data: () => setInitEvents(prev => ({ ...prev, user: true })),
      avatar_list: () => setInitEvents(prev => ({ ...prev, avatars: true })),
      voice_list: () => setInitEvents(prev => ({ ...prev, voices: true })),
      agent_list: () => setInitEvents(prev => ({ ...prev, agents: true })),
      tool_catalog: () => setInitEvents(prev => ({ ...prev, tools: true })),
      chat_session_changed: () => setInitEvents(prev => ({ ...prev, session: true }))
    };
    
    Object.entries(handlers).forEach(([event, handler]) => {
      client.on(event as any, handler);
    });
    
    return () => {
      Object.entries(handlers).forEach(([event, handler]) => {
        client.off(event as any, handler);
      });
    };
  }, [client]);
  
  const progress = Object.values(initEvents).filter(Boolean).length;
  const percentage = (progress / 6) * 100;
  
  return (
    <div className="initialization-progress">
      <h3>Initializing Agent C</h3>
      <div className="progress-bar">
        <div className="progress-fill" style={{ width: `${percentage}%` }} />
      </div>
      <ul className="init-checklist">
        <li className={initEvents.user ? 'complete' : ''}>
          ✓ User Profile
        </li>
        <li className={initEvents.avatars ? 'complete' : ''}>
          ✓ Avatar Configurations
        </li>
        <li className={initEvents.voices ? 'complete' : ''}>
          ✓ Voice Models
        </li>
        <li className={initEvents.agents ? 'complete' : ''}>
          ✓ AI Agents
        </li>
        <li className={initEvents.tools ? 'complete' : ''}>
          ✓ Tool Catalog
        </li>
        <li className={initEvents.session ? 'complete' : ''}>
          ✓ Chat Session
        </li>
      </ul>
      <p className="status">
        Status: {connectionState} ({progress}/6 complete)
      </p>
    </div>
  );
}
```

### Async Data Loading Pattern

```tsx
function DataComponent() {
  const { waitForInitialization, isInitialized } = useInitializationStatus();
  const [agents, setAgents] = useState([]);
  const [loading, setLoading] = useState(true);
  
  useEffect(() => {
    const loadData = async () => {
      try {
        // Wait for initialization before accessing data
        await waitForInitialization();
        
        // Now safe to access initialized data
        const client = useRealtimeClient();
        if (client) {
          const agentsList = client.getAgentsList();
          setAgents(agentsList);
        }
      } catch (error) {
        console.error('Failed to load agents:', error);
      } finally {
        setLoading(false);
      }
    };
    
    loadData();
  }, [waitForInitialization]);
  
  if (loading) {
    return <div>Loading agents...</div>;
  }
  
  return (
    <div className="agents-list">
      {agents.map(agent => (
        <AgentCard key={agent.id} agent={agent} />
      ))}
    </div>
  );
}
```

### Conditional Feature Rendering

```tsx
function FeatureGatedUI() {
  const { isInitialized, error } = useInitializationStatus();
  const client = useRealtimeClient();
  
  // Only render features when data is available
  const renderVoiceSelector = () => {
    if (!isInitialized) return null;
    
    const voices = client?.getVoicesList() || [];
    if (voices.length === 0) return null;
    
    return <VoiceSelector voices={voices} />;
  };
  
  const renderAgentSelector = () => {
    if (!isInitialized) return null;
    
    const agents = client?.getAgentsList() || [];
    if (agents.length === 0) return null;
    
    return <AgentSelector agents={agents} />;
  };
  
  return (
    <div className="app">
      {error && <ErrorBanner message={error} />}
      
      <div className="features">
        {renderVoiceSelector()}
        {renderAgentSelector()}
        
        {!isInitialized && (
          <div className="placeholder">
            Features loading...
          </div>
        )}
      </div>
    </div>
  );
}
```

### Initialization with Timeout

```tsx
function AppWithTimeout() {
  const { isInitialized, isLoading, waitForInitialization } = useInitializationStatus();
  const [timedOut, setTimedOut] = useState(false);
  
  useEffect(() => {
    const timeoutMs = 30000; // 30 seconds
    let timeoutId: NodeJS.Timeout;
    
    const initWithTimeout = async () => {
      try {
        // Race between initialization and timeout
        await Promise.race([
          waitForInitialization(),
          new Promise((_, reject) => {
            timeoutId = setTimeout(() => {
              reject(new Error('Initialization timeout'));
            }, timeoutMs);
          })
        ]);
      } catch (error) {
        if (error.message === 'Initialization timeout') {
          setTimedOut(true);
        }
      }
    };
    
    if (!isInitialized) {
      initWithTimeout();
    }
    
    return () => {
      if (timeoutId) {
        clearTimeout(timeoutId);
      }
    };
  }, [isInitialized, waitForInitialization]);
  
  if (timedOut) {
    return (
      <div className="timeout-screen">
        <h2>Connection Timeout</h2>
        <p>Unable to initialize Agent C. Please check your connection.</p>
        <button onClick={() => window.location.reload()}>
          Retry
        </button>
      </div>
    );
  }
  
  if (isLoading) {
    return <LoadingScreen />;
  }
  
  return <MainApp />;
}
```

## Best Practices

### 1. Early Return Pattern

```tsx
function OptimizedApp() {
  const { isInitialized, isLoading, error } = useInitializationStatus();
  
  // Handle error state first (most specific)
  if (error) {
    return <ErrorDisplay error={error} />;
  }
  
  // Handle loading state
  if (isLoading) {
    return <LoadingDisplay />;
  }
  
  // Handle uninitialized state
  if (!isInitialized) {
    return <InitializingDisplay />;
  }
  
  // Main app only renders when fully ready
  return <MainApplication />;
}
```

### 2. Initialization-Aware Routing

```tsx
function AppRouter() {
  const { isInitialized, waitForInitialization } = useInitializationStatus();
  const navigate = useNavigate();
  
  useEffect(() => {
    const protectRoute = async () => {
      // Wait for initialization before allowing navigation
      await waitForInitialization();
      
      // Now safe to navigate based on user data
      const client = useRealtimeClient();
      const userData = client?.getUserData();
      
      if (!userData?.isAuthenticated) {
        navigate('/login');
      }
    };
    
    protectRoute();
  }, [waitForInitialization, navigate]);
  
  if (!isInitialized) {
    return <SplashScreen />;
  }
  
  return (
    <Routes>
      <Route path="/login" element={<Login />} />
      <Route path="/dashboard" element={<Dashboard />} />
      <Route path="/chat" element={<Chat />} />
    </Routes>
  );
}
```

### 3. Graceful Degradation

```tsx
function GracefulApp() {
  const { isInitialized, error, connectionState } = useInitializationStatus();
  const [offlineMode, setOfflineMode] = useState(false);
  
  useEffect(() => {
    // Switch to offline mode after multiple failures
    if (error && connectionState === ConnectionState.DISCONNECTED) {
      const timer = setTimeout(() => {
        setOfflineMode(true);
      }, 10000); // Wait 10 seconds before offline mode
      
      return () => clearTimeout(timer);
    }
  }, [error, connectionState]);
  
  if (offlineMode) {
    return <OfflineApp />; // Limited functionality
  }
  
  if (!isInitialized) {
    return <InitializationScreen showError={!!error} />;
  }
  
  return <OnlineApp />; // Full functionality
}
```

### 4. Progressive Enhancement

```tsx
function ProgressiveUI() {
  const { isInitialized, isLoading } = useInitializationStatus();
  const [essentialData, setEssentialData] = useState(null);
  
  // Load essential data immediately
  useEffect(() => {
    loadEssentialData().then(setEssentialData);
  }, []);
  
  return (
    <div className="app">
      {/* Always show essential UI */}
      {essentialData && (
        <EssentialFeatures data={essentialData} />
      )}
      
      {/* Show enhanced features when initialized */}
      {isInitialized && (
        <EnhancedFeatures />
      )}
      
      {/* Show loading indicator for enhanced features */}
      {isLoading && !isInitialized && (
        <div className="loading-enhanced">
          Loading advanced features...
        </div>
      )}
    </div>
  );
}
```

## StrictMode Compatibility Notes

The `useInitializationStatus` hook is fully compatible with React StrictMode. Key considerations:

1. **Double Mount Safety**: The hook properly handles StrictMode's double-mounting behavior by:
   - Using proper effect cleanup to unsubscribe event listeners
   - Checking current state before updates
   - Not performing side effects during render

2. **Event Subscription Management**: All event listeners are cleaned up in the effect return function, preventing duplicate subscriptions during StrictMode re-mounting.

3. **State Consistency**: The hook maintains consistent state by always checking the client's actual state rather than relying solely on events.

Example StrictMode usage:

```tsx
function StrictModeApp() {
  return (
    <StrictMode>
      <AgentCProvider config={config}>
        <InitializationAwareApp />
      </AgentCProvider>
    </StrictMode>
  );
}

function InitializationAwareApp() {
  const { isInitialized, error } = useInitializationStatus();
  
  // Hook works correctly even with StrictMode double-mounting
  if (!isInitialized) {
    return <LoadingScreen />;
  }
  
  return <MainApp />;
}
```

## Integration with Other Hooks

### With useConnection

```tsx
function SmartConnection() {
  const { connect, isConnected, connectionState } = useConnection();
  const { isInitialized, waitForInitialization, error } = useInitializationStatus();
  
  const ensureReady = async () => {
    // Connect if needed
    if (!isConnected) {
      await connect();
    }
    
    // Wait for initialization
    await waitForInitialization();
    
    // Now fully ready for operations
    console.log('Connection established and initialized');
  };
  
  return (
    <div>
      <StatusDisplay 
        connectionState={connectionState}
        initialized={isInitialized}
        error={error}
      />
      <button onClick={ensureReady}>
        Ensure Ready
      </button>
    </div>
  );
}
```

### With useChat

```tsx
function ChatWithInit() {
  const { isInitialized } = useInitializationStatus();
  const { messages, sendMessage } = useChat();
  
  // Disable chat until initialization complete
  const canChat = isInitialized;
  
  return (
    <div className="chat-container">
      {!isInitialized && (
        <div className="init-overlay">
          <Spinner />
          <p>Preparing chat...</p>
        </div>
      )}
      
      <ChatMessages messages={messages} />
      
      <ChatInput 
        onSend={sendMessage}
        disabled={!canChat}
        placeholder={!canChat ? "Initializing..." : "Type a message"}
      />
    </div>
  );
}
```

### With useVoiceModel

```tsx
function VoiceSetup() {
  const { isInitialized, waitForInitialization } = useInitializationStatus();
  const { voices, selectedVoice, selectVoice } = useVoiceModel();
  
  useEffect(() => {
    const setupDefaultVoice = async () => {
      // Wait for voices to be loaded
      await waitForInitialization();
      
      // Set default voice if none selected
      if (!selectedVoice && voices.length > 0) {
        selectVoice(voices[0].id);
      }
    };
    
    setupDefaultVoice();
  }, [waitForInitialization, selectedVoice, voices, selectVoice]);
  
  if (!isInitialized) {
    return <div>Loading voice options...</div>;
  }
  
  return (
    <VoiceSelector 
      voices={voices}
      selected={selectedVoice}
      onSelect={selectVoice}
    />
  );
}
```

### With useAvatar

```tsx
function AvatarWithInit() {
  const { isInitialized } = useInitializationStatus();
  const { avatars, selectedAvatar, startAvatarSession } = useAvatar();
  
  const handleAvatarSelect = async (avatarId: string) => {
    if (!isInitialized) {
      alert('Please wait for initialization to complete');
      return;
    }
    
    await startAvatarSession(avatarId);
  };
  
  return (
    <div className="avatar-selector">
      {!isInitialized && (
        <div className="loading-overlay">
          Loading avatars...
        </div>
      )}
      
      {isInitialized && avatars.map(avatar => (
        <AvatarOption
          key={avatar.id}
          avatar={avatar}
          selected={selectedAvatar?.id === avatar.id}
          onSelect={() => handleAvatarSelect(avatar.id)}
          disabled={!isInitialized}
        />
      ))}
    </div>
  );
}
```

## Error Handling Patterns

### Retry Mechanism

```tsx
function RetryableInit() {
  const { isInitialized, error, connectionState } = useInitializationStatus();
  const { reconnect } = useConnection();
  const [retryCount, setRetryCount] = useState(0);
  const maxRetries = 3;
  
  const handleRetry = async () => {
    if (retryCount >= maxRetries) {
      console.error('Max retries reached');
      return;
    }
    
    setRetryCount(prev => prev + 1);
    
    try {
      await reconnect();
      // Reset retry count on success
      setRetryCount(0);
    } catch (err) {
      console.error('Retry failed:', err);
    }
  };
  
  if (error) {
    return (
      <div className="error-container">
        <h3>Initialization Failed</h3>
        <p>{error}</p>
        <p>Connection State: {connectionState}</p>
        
        {retryCount < maxRetries ? (
          <button onClick={handleRetry}>
            Retry ({retryCount}/{maxRetries})
          </button>
        ) : (
          <div>
            <p>Maximum retries exceeded.</p>
            <button onClick={() => window.location.reload()}>
              Reload Page
            </button>
          </div>
        )}
      </div>
    );
  }
  
  if (!isInitialized) {
    return <InitializingScreen />;
  }
  
  return <App />;
}
```

### Error Recovery Strategies

```tsx
function SmartErrorRecovery() {
  const { error, isInitialized, waitForInitialization } = useInitializationStatus();
  const [recoveryStrategy, setRecoveryStrategy] = useState<'retry' | 'refresh' | 'offline'>('retry');
  
  useEffect(() => {
    if (!error) return;
    
    // Analyze error and determine recovery strategy
    const errorLower = error.toLowerCase();
    
    if (errorLower.includes('auth') || errorLower.includes('token')) {
      // Auth errors need token refresh
      setRecoveryStrategy('refresh');
    } else if (errorLower.includes('network') || errorLower.includes('timeout')) {
      // Network errors can retry
      setRecoveryStrategy('retry');
    } else {
      // Unknown errors might need offline mode
      setRecoveryStrategy('offline');
    }
  }, [error]);
  
  const handleRecovery = async () => {
    switch (recoveryStrategy) {
      case 'refresh':
        // Refresh auth token and reload
        await refreshAuthToken();
        window.location.reload();
        break;
        
      case 'retry':
        // Wait and retry initialization
        await new Promise(resolve => setTimeout(resolve, 2000));
        await waitForInitialization();
        break;
        
      case 'offline':
        // Switch to offline mode
        enterOfflineMode();
        break;
    }
  };
  
  if (error) {
    return (
      <ErrorRecoveryUI 
        error={error}
        strategy={recoveryStrategy}
        onRecover={handleRecovery}
      />
    );
  }
  
  if (!isInitialized) {
    return <InitializingDisplay />;
  }
  
  return <MainApp />;
}
```

## Performance Considerations

1. **Event Listener Optimization**: The hook subscribes to multiple events but uses a single effect with proper cleanup to minimize overhead.

2. **State Check Debouncing**: The `checkStatus` function is memoized with useCallback to prevent unnecessary recreations.

3. **Conditional Effect Dependencies**: Effects only run when necessary dependencies change, avoiding unnecessary re-subscriptions.

4. **Promise Caching**: The `waitForInitialization` method leverages the client's internal promise caching to avoid creating multiple pending promises.

## Notes on Implementation

- **Initialization Events**: The hook tracks six specific events that constitute a complete initialization. These events provide all necessary configuration data for the application.

- **Loading State Logic**: The `isLoading` state is intelligent - it's true during connection establishment AND while waiting for initialization events after connection.

- **Error Preservation**: Connection errors are preserved in the hook's state even after disconnection, allowing for proper error display and recovery strategies.

- **State Persistence**: The `isInitialized` state is maintained even during temporary disconnections, reflecting the last known initialization state.

- **Event Granularity**: The hook listens to individual initialization events in addition to the aggregate "initialized" event, allowing for progress tracking and debugging.

- **Client Safety**: The hook uses `useRealtimeClientSafe()` which gracefully handles cases where the client might not be available yet.
# Agent C React Provider

The `AgentCProvider` component provides a React context that manages the lifecycle of the RealtimeClient instance from the core SDK.

## Basic Usage

```tsx
import React from 'react';
import { AgentCProvider } from '@agentc/realtime-react';

function App() {
  return (
    <AgentCProvider 
      apiUrl="wss://api.example.com/rt/ws"
      authToken="your-jwt-token"
      autoConnect={true}
      debug={true}
    >
      <YourAppComponents />
    </AgentCProvider>
  );
}
```

## Using with Environment Variables

The provider can automatically read configuration from environment variables:

```tsx
// .env file
REACT_APP_AGENTC_API_URL=wss://api.example.com/rt/ws

// App.tsx
<AgentCProvider authToken={userToken}>
  <App />
</AgentCProvider>
```

## Using with AuthManager

For automatic token management:

```tsx
import { AuthManager } from '@agentc/realtime-core';

const authManager = new AuthManager({ 
  apiBaseUrl: 'https://api.example.com' 
});

<AgentCProvider 
  authManager={authManager}
  autoConnect={true}
>
  <App />
</AgentCProvider>
```

## Accessing the Client

Use the provided hooks to access the RealtimeClient:

```tsx
import { useRealtimeClient } from '@agentc/realtime-react';

function ChatComponent() {
  const client = useRealtimeClient(); // Throws if not in provider
  
  const sendMessage = (text: string) => {
    client.sendText(text);
  };
  
  return (
    <button onClick={() => sendMessage('Hello!')}>
      Send Message
    </button>
  );
}
```

## Safe Client Access

For components that may render before the client is ready:

```tsx
import { useRealtimeClientSafe } from '@agentc/realtime-react';

function StatusComponent() {
  const client = useRealtimeClientSafe(); // Returns null if not ready
  
  if (!client) {
    return <div>Initializing...</div>;
  }
  
  return <div>Connected: {client.isConnected()}</div>;
}
```

## Checking Initialization State

```tsx
import { useAgentCContext } from '@agentc/realtime-react';

function LoadingComponent() {
  const { client, isInitializing, error } = useAgentCContext();
  
  if (error) {
    return <div>Error: {error.message}</div>;
  }
  
  if (isInitializing) {
    return <div>Loading...</div>;
  }
  
  if (!client) {
    return <div>Client not available</div>;
  }
  
  return <div>Ready!</div>;
}
```

## Provider Props

| Prop | Type | Description | Default |
|------|------|-------------|---------|
| `apiUrl` | `string` | WebSocket API URL | - |
| `authToken` | `string` | JWT authentication token | - |
| `authManager` | `AuthManager` | Auth manager instance for token management | - |
| `config` | `RealtimeClientConfig` | Complete configuration (overrides other props) | - |
| `autoConnect` | `boolean` | Automatically connect on mount | `false` |
| `onInitialized` | `(client) => void` | Callback when client is initialized | - |
| `onError` | `(error) => void` | Callback on initialization error | - |
| `debug` | `boolean` | Enable debug logging | `false` |

## Advanced Configuration

For full control over the client configuration:

```tsx
const config: RealtimeClientConfig = {
  apiUrl: 'wss://api.example.com/rt/ws',
  authToken: 'your-token',
  autoReconnect: true,
  reconnection: {
    enabled: true,
    maxAttempts: 10,
    initialDelay: 1000,
    maxDelay: 30000
  },
  enableAudio: true,
  audioConfig: {
    enableInput: true,
    enableOutput: true,
    respectTurnState: true
  },
  debug: true
};

<AgentCProvider config={config}>
  <App />
</AgentCProvider>
```

## Higher-Order Component

For quick wrapping of components:

```tsx
import { withAgentCProvider } from '@agentc/realtime-react';

const EnhancedApp = withAgentCProvider(App, {
  apiUrl: 'wss://api.example.com/rt/ws',
  authToken: 'your-token',
  autoConnect: true
});

// Use EnhancedApp directly
<EnhancedApp />
```
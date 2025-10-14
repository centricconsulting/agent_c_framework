# AgentCProvider API Reference

The `AgentCProvider` component provides the Agent C Realtime SDK context to React applications, managing client lifecycle and authentication.

## Import

```typescript
import { AgentCProvider } from '@agentc/realtime-react';
```

## Overview

AgentCProvider is a React context provider that creates and manages a RealtimeClient instance, making it available to child components through React hooks. Agent C uses username/password authentication, not API keys.

## Props

```typescript
interface AgentCProviderProps {
  /** Child components that will have access to the RealtimeClient */
  children: React.ReactNode;
  
  /** WebSocket API URL (e.g., wss://localhost:8000/rt/ws) */
  apiUrl?: string;
  
  /** JWT authentication token (if already authenticated) */
  authToken?: string;
  
  /** Optional AuthManager instance for automatic token management */
  authManager?: AuthManager;
  
  /** Complete configuration object (overrides individual props) */
  config?: RealtimeClientConfig;
  
  /** Whether to automatically connect on mount (default: false) */
  autoConnect?: boolean;
  
  /** Callback when client is successfully initialized */
  onInitialized?: (client: RealtimeClient) => void;
  
  /** Callback when initialization fails */
  onError?: (error: Error) => void;
  
  /** Enable debug logging */
  debug?: boolean;
}
```

### Important: Environment Variables in React/Next.js

**Environment variables in React and Next.js are replaced at BUILD TIME, not runtime:**

- **Create React App**: Variables prefixed with `REACT_APP_` are replaced during the build process
- **Next.js**: Variables prefixed with `NEXT_PUBLIC_` are replaced during the build process
- These become hardcoded strings in the compiled JavaScript bundle
- They are NOT read from `process.env` at runtime in the browser

```typescript
// This is replaced at BUILD TIME with the actual value:
const apiUrl = process.env.REACT_APP_AGENTC_API_URL;
// After build, it becomes: const apiUrl = "wss://localhost:8000/rt/ws";

// The value is baked into your bundle - changing the env var after build has no effect
```

## Basic Usage

```tsx
import { AgentCProvider, AuthManager } from '@agentc/realtime-react';

function App() {
  const [authManager, setAuthManager] = useState<AuthManager | null>(null);
  
  // Authenticate before rendering the provider
  useEffect(() => {
    const authenticate = async () => {
      const manager = new AuthManager({
        apiUrl: 'https://localhost:8000'  // REST API base URL
      });
      
      try {
        // Login with username and password
        await manager.login({
          username: 'your-username',
          password: 'your-password'
        });
        
        setAuthManager(manager);
      } catch (error) {
        console.error('Authentication failed:', error);
      }
    };
    
    authenticate();
  }, []);
  
  if (!authManager) {
    return <div>Authenticating...</div>;
  }
  
  return (
    <AgentCProvider 
      apiUrl="wss://localhost:8000/rt/ws"  // WebSocket URL
      authManager={authManager}  // Pass authenticated manager
      autoConnect={true}
      debug={true}
    >
      <YourComponents />
    </AgentCProvider>
  );
}
```

## Props Details

### apiUrl (optional)

The WebSocket URL for the realtime connection. Can be provided directly or via environment variable.

**Development example:**
```tsx
<AgentCProvider 
  apiUrl="wss://localhost:8000/rt/ws"
  authManager={authManager}
>
```

**Production with environment variable (replaced at build time):**
```tsx
// .env file
REACT_APP_AGENTC_WS_URL=wss://api.example.com/rt/ws

// Component
<AgentCProvider 
  apiUrl={process.env.REACT_APP_AGENTC_WS_URL}  // Replaced at build time
  authManager={authManager}
>
```

### authManager (recommended)

An authenticated AuthManager instance that handles token management.

**Example:**
```tsx
const authManager = new AuthManager({
  apiUrl: 'https://localhost:8000',  // REST API base URL
  autoRefresh: true,  // Automatically refresh tokens
  refreshBufferMs: 60000  // Refresh 1 minute before expiry
});

// Login with credentials
await authManager.login({
  username: 'user@example.com',
  password: 'secure-password'
});

<AgentCProvider authManager={authManager}>
```

### authToken (optional)

A JWT token if you've already authenticated elsewhere. Use this if you're managing authentication outside of the SDK.

**Example:**
```tsx
<AgentCProvider 
  apiUrl="wss://localhost:8000/rt/ws"
  authToken={jwtToken}  // Previously obtained JWT
>
```

### children (required)

React components that will have access to the Agent C context.

### onError (optional)

Error handler callback for client errors.

**Example:**
```tsx
<AgentCProvider 
  config={config}
  onError={(error) => {
    console.error('Agent C Error:', error);
    showErrorNotification(error.message);
  }}
>
```

### autoConnect (optional)

Whether to automatically connect when the provider mounts.

**Default:** `false`

**Example:**
```tsx
<AgentCProvider 
  config={config}
  autoConnect={true}  // Connect immediately
>
```

### debug (optional)

Enable debug logging.

**Default:** `false`

**Example:**
```tsx
<AgentCProvider 
  config={config}
  debug={process.env.NODE_ENV === 'development'}
>
```

## Context Value

The provider makes the following context available:

```typescript
interface AgentCContextValue {
  client: RealtimeClient | null;
  isInitialized: boolean;
  error: Error | null;
}
```

## Environment Variables

### Understanding Build-Time Replacement

Environment variables in React/Next.js are **replaced at build time**, not read at runtime:

```bash
# .env file (Create React App)
REACT_APP_AGENTC_WS_URL=wss://localhost:8000/rt/ws
REACT_APP_AGENTC_API_URL=https://localhost:8000
REACT_APP_DEBUG=true

# .env.local file (Next.js)
NEXT_PUBLIC_AGENTC_WS_URL=wss://localhost:8000/rt/ws
NEXT_PUBLIC_AGENTC_API_URL=https://localhost:8000
NEXT_PUBLIC_DEBUG=true
```

```tsx
// During build, this code:
const wsUrl = process.env.REACT_APP_AGENTC_WS_URL;

// Gets compiled to:
const wsUrl = "wss://localhost:8000/rt/ws";

// The actual string value is embedded in your JavaScript bundle
```

### Using Environment Variables Correctly

```tsx
function App() {
  // Environment variables are strings baked in at build time
  const wsUrl = process.env.REACT_APP_AGENTC_WS_URL || 'wss://localhost:8000/rt/ws';
  const apiUrl = process.env.REACT_APP_AGENTC_API_URL || 'https://localhost:8000';
  const debug = process.env.REACT_APP_DEBUG === 'true';
  
  const [authManager, setAuthManager] = useState<AuthManager | null>(null);
  
  useEffect(() => {
    const manager = new AuthManager({ apiUrl });
    // In production, get credentials securely (not from env vars!)
    manager.login({
      username: getUsername(),  // Get from secure source
      password: getPassword()   // Get from secure source
    }).then(() => setAuthManager(manager));
  }, [apiUrl]);
  
  if (!authManager) return <div>Loading...</div>;
  
  return (
    <AgentCProvider 
      apiUrl={wsUrl}
      authManager={authManager}
      debug={debug}
    >
      <YourComponents />
    </AgentCProvider>
  );
}
```

**⚠️ Security Warning:** Never put passwords or sensitive credentials in environment variables that get bundled into client-side code!

## Authentication Methods

### Method 1: AuthManager with Credentials (Recommended)

```tsx
import { AuthManager } from '@agentc/realtime-core';

function App() {
  const [authManager, setAuthManager] = useState<AuthManager | null>(null);
  const [error, setError] = useState<string | null>(null);
  
  const handleLogin = async (username: string, password: string) => {
    const manager = new AuthManager({
      apiUrl: 'https://localhost:8000',  // REST API base URL
      autoRefresh: true,  // Auto-refresh tokens before expiry
      refreshBufferMs: 60000  // Refresh 1 minute before expiry
    });
    
    try {
      // Login returns user info, available agents, voices, avatars, etc.
      const loginResponse = await manager.login({ username, password });
      console.log('Logged in as:', loginResponse.user);
      console.log('Available agents:', loginResponse.agents);
      console.log('WebSocket URL:', loginResponse.ws_url);
      
      setAuthManager(manager);
    } catch (error) {
      setError(error.message);
    }
  };
  
  if (!authManager) {
    return <LoginForm onLogin={handleLogin} error={error} />;
  }
  
  return (
    <AgentCProvider 
      apiUrl="wss://localhost:8000/rt/ws"
      authManager={authManager}
      autoConnect={true}
    >
      <YourApp />
    </AgentCProvider>
  );
}
```

### Method 2: Pre-authenticated Token

```tsx
// If you already have a JWT token from another source
<AgentCProvider 
  apiUrl="wss://localhost:8000/rt/ws"
  authToken={jwtToken}  // Previously obtained JWT
  autoConnect={true}
>
```

### Method 3: External Authentication

```tsx
function App() {
  const [config, setConfig] = useState<any>(null);
  
  useEffect(() => {
    // Authenticate via your own backend
    fetch('/api/agentc-auth', {
      method: 'POST',
      credentials: 'include'  // Include cookies
    })
    .then(res => res.json())
    .then(data => {
      setConfig({
        apiUrl: data.wsUrl,  // From your backend
        authToken: data.token  // JWT from your backend
      });
    });
  }, []);
  
  if (!config) return <div>Authenticating...</div>;
  
  return (
    <AgentCProvider {...config} autoConnect={true}>
      <YourApp />
    </AgentCProvider>
  );
}
```

## Complete Example

```tsx
import React, { useState, useEffect } from 'react';
import { 
  AgentCProvider,
  AuthManager,
  useConnection, 
  useChat 
} from '@agentc/realtime-react';

function App() {
  const [authManager, setAuthManager] = useState<AuthManager | null>(null);
  const [error, setError] = useState<Error | null>(null);
  
  // Build-time environment variable replacement
  const wsUrl = process.env.REACT_APP_AGENTC_WS_URL || 'wss://localhost:8000/rt/ws';
  const apiUrl = process.env.REACT_APP_AGENTC_API_URL || 'https://localhost:8000';
  
  useEffect(() => {
    const authenticate = async () => {
      const manager = new AuthManager({
        apiUrl,  // REST API for authentication
        autoRefresh: true,
        refreshBufferMs: 60000
      });
      
      try {
        // In production, get credentials from a secure source
        // Never hardcode credentials!
        const credentials = await getCredentials();  // Your implementation
        
        const loginResponse = await manager.login(credentials);
        console.log('Authenticated:', loginResponse.user);
        
        setAuthManager(manager);
      } catch (err) {
        setError(err as Error);
      }
    };
    
    authenticate();
  }, [apiUrl]);
  
  if (error) {
    return (
      <div className="error">
        Authentication failed: {error.message}
        <button onClick={() => window.location.reload()}>Retry</button>
      </div>
    );
  }
  
  if (!authManager) {
    return <div>Authenticating...</div>;
  }
  
  return (
    <AgentCProvider 
      apiUrl={wsUrl}
      authManager={authManager}
      autoConnect={false}  // Manual connection
      debug={true}
      onError={(err) => {
        console.error('Agent C Error:', err);
        setError(err);
      }}
    >
      {error && (
        <div className="error-banner">
          Error: {error.message}
          <button onClick={() => setError(null)}>Dismiss</button>
        </div>
      )}
      
      <ChatInterface />
    </AgentCProvider>
  );
}

// Helper function to get credentials securely
async function getCredentials() {
  // In development, you might use env vars (NOT for production!)
  if (process.env.NODE_ENV === 'development') {
    return {
      username: process.env.REACT_APP_DEV_USERNAME || '',
      password: process.env.REACT_APP_DEV_PASSWORD || ''
    };
  }
  
  // In production, get from a login form or secure backend
  return await showLoginDialog();  // Your implementation
}

function ChatInterface() {
  const { connect, disconnect, isConnected, connectionState } = useConnection();
  const { messages, sendMessage } = useChat();
  const [input, setInput] = useState('');
  
  const handleConnect = async () => {
    try {
      await connect();
      console.log('Connected successfully');
    } catch (error) {
      console.error('Connection failed:', error);
    }
  };
  
  const handleSend = () => {
    if (input.trim() && isConnected) {
      sendMessage(input);
      setInput('');
    }
  };
  
  return (
    <div className="chat-interface">
      <div className="connection-status">
        Status: {connectionState}
        <button onClick={isConnected ? disconnect : handleConnect}>
          {isConnected ? 'Disconnect' : 'Connect'}
        </button>
      </div>
      
      <div className="messages">
        {messages.map((msg, i) => (
          <div key={i} className={`message ${msg.role}`}>
            <span className="role">{msg.role}:</span>
            <span className="content">{msg.content}</span>
          </div>
        ))}
      </div>
      
      <div className="input-area">
        <input
          value={input}
          onChange={(e) => setInput(e.target.value)}
          onKeyPress={(e) => e.key === 'Enter' && handleSend()}
          placeholder="Type a message..."
          disabled={!isConnected}
        />
        <button onClick={handleSend} disabled={!isConnected}>
          Send
        </button>
      </div>
    </div>
  );
}

export default App;
```

## Advanced Configuration

### Custom Client Configuration

```tsx
import { RealtimeClient, AuthManager, AgentCContext } from '@agentc/realtime-react';

function CustomProvider({ children }) {
  const [client, setClient] = useState<RealtimeClient | null>(null);
  
  useEffect(() => {
    // Create custom client with advanced config
    const initClient = async () => {
      const authManager = new AuthManager({
        apiUrl: 'https://localhost:8000',  // REST API
        refreshBufferMs: 60000,  // Refresh 1 minute before expiry
        autoRefresh: true,
        storage: new LocalStorageTokenStorage(),  // Persist tokens
        onTokensRefreshed: (tokens) => {
          console.log('Tokens refreshed, expires at:', new Date(tokens.expiresAt));
        },
        onAuthError: (error) => {
          console.error('Auth error:', error);
          // Redirect to login or show error
        }
      });
      
      // Get credentials securely (not from env vars in production!)
      const { username, password } = await getCredentials();
      const loginResponse = await authManager.login({ username, password });
      
      // Use the WebSocket URL from login response if available
      const wsUrl = loginResponse.ws_url || 'wss://localhost:8000/rt/ws';
      
      const realtimeClient = new RealtimeClient({
        apiUrl: wsUrl,
        authManager,  // Pass the authenticated manager
        enableAudio: true,
        audioConfig: {
          enableInput: true,
          enableOutput: true,
          sampleRate: 16000,
          chunkDuration: 100,
          respectTurnState: true
        },
        enableTurnManager: true,
        debug: true,
        connectionTimeout: 10000,
        pingInterval: 30000,
        pongTimeout: 5000
      });
      
      setClient(realtimeClient);
    };
    
    initClient().catch(error => {
      console.error('Failed to initialize client:', error);
    });
    
    return () => {
      client?.destroy();
    };
  }, []);
  
  if (!client) {
    return <div>Initializing...</div>;
  }
  
  return (
    <AgentCContext.Provider value={{ 
      client, 
      isInitializing: false, 
      error: null 
    }}>
      {children}
    </AgentCContext.Provider>
  );
}
```

### Multi-Environment Support

```tsx
const getEnvironmentConfig = () => {
  // Environment variable is replaced at BUILD TIME
  const env = process.env.REACT_APP_ENV || 'development';
  
  const configs = {
    development: {
      wsUrl: 'wss://localhost:8000/rt/ws',
      apiUrl: 'https://localhost:8000',
      debug: true
    },
    staging: {
      wsUrl: 'wss://staging.example.com/rt/ws',
      apiUrl: 'https://staging.example.com',
      debug: true
    },
    production: {
      wsUrl: 'wss://api.example.com/rt/ws',
      apiUrl: 'https://api.example.com',
      debug: false
    }
  };
  
  return configs[env as keyof typeof configs];
};

function App() {
  const config = getEnvironmentConfig();
  const [authManager, setAuthManager] = useState<AuthManager | null>(null);
  
  useEffect(() => {
    const authenticate = async () => {
      const manager = new AuthManager({
        apiUrl: config.apiUrl,
        autoRefresh: true
      });
      
      // Get credentials based on environment
      const credentials = await getCredentialsForEnvironment();
      
      try {
        await manager.login(credentials);
        setAuthManager(manager);
      } catch (error) {
        console.error('Authentication failed:', error);
      }
    };
    
    authenticate();
  }, [config.apiUrl]);
  
  if (!authManager) {
    return <div>Authenticating...</div>;
  }
  
  return (
    <AgentCProvider 
      apiUrl={config.wsUrl}
      authManager={authManager}
      debug={config.debug}
      autoConnect={true}
    >
      <YourApp />
    </AgentCProvider>
  );
}
```

## Error Handling

```tsx
function AppWithErrorBoundary() {
  return (
    <ErrorBoundary>
      <AgentCProvider 
        config={config}
        onError={(error) => {
          // Log to error tracking service
          Sentry.captureException(error);
          
          // Show user-friendly message
          if (error.message.includes('Permission denied')) {
            showNotification('Please allow microphone access');
          } else if (error.message.includes('Network')) {
            showNotification('Connection lost. Retrying...');
          } else {
            showNotification('An error occurred');
          }
        }}
      >
        <App />
      </AgentCProvider>
    </ErrorBoundary>
  );
}

class ErrorBoundary extends React.Component {
  componentDidCatch(error, errorInfo) {
    console.error('React Error:', error, errorInfo);
  }
  
  render() {
    return this.props.children;
  }
}
```

## Best Practices

1. **Understand environment variable behavior:**
```tsx
// ✅ Good: Environment URLs (non-sensitive)
const wsUrl = process.env.REACT_APP_AGENTC_WS_URL || 'wss://localhost:8000/rt/ws';

// ❌ Bad: Never put credentials in client-side env vars
// These get bundled into your JavaScript!
const password = process.env.REACT_APP_PASSWORD;  // NEVER DO THIS!

// ✅ Good: Get credentials securely at runtime
const credentials = await getUserCredentials();  // From login form or secure API
```

2. **Handle errors appropriately:**
```tsx
<AgentCProvider 
  onError={(error) => {
    logError(error);
    showUserNotification(error);
  }}
>
```

3. **Lazy load audio features:**
```tsx
const [audioEnabled, setAudioEnabled] = useState(false);

<AgentCProvider 
  config={{
    ...baseConfig,
    enableAudio: audioEnabled
  }}
>
```

4. **Clean up on unmount:**
```tsx
useEffect(() => {
  return () => {
    // Provider handles cleanup automatically
    // But you can add additional cleanup here
  };
}, []);
```

5. **Use one provider per app:**
```tsx
// ✅ Good - single provider at root with authentication
const App = () => {
  const [authManager, setAuthManager] = useState(null);
  
  // Authenticate once at the app level
  useEffect(() => {
    authenticateUser().then(setAuthManager);
  }, []);
  
  if (!authManager) return <LoginScreen />;
  
  return (
    <AgentCProvider 
      apiUrl="wss://localhost:8000/rt/ws"
      authManager={authManager}
    >
      <Router>
        <AppContent />
      </Router>
    </AgentCProvider>
  );
};

// ❌ Bad - multiple providers
<AgentCProvider authManager={authManager1}>
  <Component1 />
</AgentCProvider>
<AgentCProvider authManager={authManager2}>
  <Component2 />
</AgentCProvider>
```

## TypeScript Usage

```tsx
import { 
  AgentCProvider, 
  AgentCProviderProps,
  AuthManager,
  LoginCredentials
} from '@agentc/realtime-react';

const App: React.FC = () => {
  const [authManager, setAuthManager] = useState<AuthManager | null>(null);
  
  useEffect(() => {
    const initAuth = async () => {
      const manager = new AuthManager({
        apiUrl: 'https://localhost:8000'
      });
      
      const credentials: LoginCredentials = {
        username: 'user@example.com',
        password: 'secure-password'
      };
      
      try {
        await manager.login(credentials);
        setAuthManager(manager);
      } catch (error) {
        console.error('Login failed:', error);
      }
    };
    
    initAuth();
  }, []);
  
  const handleError = (error: Error): void => {
    console.error('AgentC Error:', error);
  };
  
  const handleInitialized = (client: RealtimeClient): void => {
    console.log('Client initialized:', client);
  };
  
  if (!authManager) {
    return <div>Authenticating...</div>;
  }
  
  return (
    <AgentCProvider 
      apiUrl="wss://localhost:8000/rt/ws"
      authManager={authManager}
      onError={handleError}
      onInitialized={handleInitialized}
      autoConnect={true}
      debug={false}
    >
      <YourComponents />
    </AgentCProvider>
  );
};
```

## Troubleshooting

### Provider not connecting

- Check WebSocket URL is correct (ws:// or wss://)
- Verify authentication succeeded (check authManager.isAuthenticated())
- Ensure credentials are valid
- Check network connectivity
- Look for errors in console
- Verify the server is running (for localhost development)

### Context is null in child components

- Ensure components are inside provider
- Check that authentication completed before rendering provider
- Verify authManager is set before passing to provider
- Check for initialization errors in console

### Authentication issues

- Verify username/password are correct
- Check REST API URL (different from WebSocket URL)
- Ensure login endpoint is accessible
- Check for CORS issues in development
- Look for detailed error messages in network tab

### Environment variable issues

- Remember env vars are replaced at BUILD time, not runtime
- After changing .env file, restart the development server
- In production, rebuild the app after changing env vars
- Use REACT_APP_ prefix for Create React App
- Use NEXT_PUBLIC_ prefix for Next.js
- Never put sensitive data in client-side env vars

### Memory leaks

- Provider handles cleanup automatically
- AuthManager cleanup happens in provider unmount
- Ensure custom hooks clean up subscriptions
- Check for circular references

## See Also

- [React Hooks](./hooks.md) - Available hooks for consuming the context
- [RealtimeClient](../core/RealtimeClient.md) - Core client documentation
- [Examples](./examples.md) - Complete React examples
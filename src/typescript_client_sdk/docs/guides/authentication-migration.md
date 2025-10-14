# Authentication Migration Guide

## Breaking Changes in v2.0.0

We've completely restructured how authentication and initialization work to simplify developer experience and improve performance. This guide will help you migrate from the old pattern to the new streamlined approach.

## What Changed

### 1. Login Response Simplified

The login response has been dramatically simplified to only return essential authentication data:

#### Before (Complex Response)
```typescript
// OLD - Login returned everything
const loginResponse = await authManager.login(credentials);
// Response included:
{
  jwt_token: "...",
  heygen_token: "...", 
  websocket_url: "...",
  user: { /* user data */ },
  voices: [ /* voice list */ ],
  agents: [ /* agent list */ ],
  avatars: [ /* avatar list */ ],
  tools: [ /* tool catalog */ ],
  sessions: [ /* chat sessions */ ]
}
```

#### After (Simple Response)
```typescript
// NEW - Login returns only auth tokens
const loginResponse = await authManager.login(credentials);
// Response now only includes:
{
  jwt_token: "...",
  heygen_token: "...",
  websocket_url: "..."
}
```

### 2. Data Delivered Via WebSocket Events

All configuration data now arrives automatically through WebSocket events after connection:

```typescript
// NEW - Data arrives via 6 automatic events
client.on('chat_user_data', (event) => {
  // User profile data
});

client.on('voice_list', (event) => {
  // Available voices
});

client.on('agent_list', (event) => {
  // Available agents
});

client.on('avatar_list', (event) => {
  // Available avatars
});

client.on('tool_catalog', (event) => {
  // Available tools
});

client.on('chat_session_changed', (event) => {
  // Current session
});
```

### 3. New React Hooks

We've added new hooks to simplify accessing this data:

```typescript
// NEW - Unified data access
const { user, voices, agents, avatars, tools } = useAgentCData();

// NEW - Initialization status
const { isInitialized, isConnecting, error } = useInitializationStatus();

// NEW - User data hook
const { user, isLoading } = useUserData();
```

## Migration Steps

### Step 1: Update Login Handling

#### Before
```typescript
// OLD - Complex login with data extraction
const handleLogin = async (credentials) => {
  try {
    const response = await authManager.login(credentials);
    
    // Store everything from login response
    setUser(response.user);
    setVoices(response.voices);
    setAgents(response.agents);
    setAvatars(response.avatars);
    setSessions(response.sessions);
    
    // Then connect
    await client.connect();
  } catch (error) {
    console.error('Login failed:', error);
  }
};
```

#### After
```typescript
// NEW - Simple login, data comes from events
const handleLogin = async (credentials) => {
  try {
    // Login only returns tokens
    await authManager.login(credentials);
    
    // Connect and let events deliver data
    await client.connect();
    
    // Data will arrive via events automatically
  } catch (error) {
    console.error('Login failed:', error);
  }
};
```

### Step 2: Update Data Access

#### Before
```typescript
// OLD - Data from login response
function ChatComponent() {
  const [user, setUser] = useState(null);
  const [voices, setVoices] = useState([]);
  
  useEffect(() => {
    // Data was set during login
    const loginData = getStoredLoginData();
    setUser(loginData.user);
    setVoices(loginData.voices);
  }, []);
  
  if (!user) {
    return <div>Loading...</div>;
  }
  
  return (
    <div>
      <h1>Welcome {user.display_name}</h1>
      <select>
        {voices.map(voice => (
          <option key={voice.voice_id}>{voice.name}</option>
        ))}
      </select>
    </div>
  );
}
```

#### After
```typescript
// NEW - Data from hooks
function ChatComponent() {
  const { user, voices } = useAgentCData();
  const { isInitialized } = useInitializationStatus();
  
  if (!isInitialized) {
    return <div>Initializing...</div>;
  }
  
  return (
    <div>
      <h1>Welcome {user?.display_name}</h1>
      <select>
        {voices.map(voice => (
          <option key={voice.voice_id}>{voice.name}</option>
        ))}
      </select>
    </div>
  );
}
```

### Step 3: Update Component Loading States

#### Before
```typescript
// OLD - Manual loading state management
function AppContent() {
  const [isLoading, setIsLoading] = useState(true);
  const [user, setUser] = useState(null);
  const [error, setError] = useState(null);
  
  useEffect(() => {
    const initializeApp = async () => {
      try {
        const loginData = await authManager.login(credentials);
        setUser(loginData.user);
        await client.connect();
        setIsLoading(false);
      } catch (err) {
        setError(err);
        setIsLoading(false);
      }
    };
    
    initializeApp();
  }, []);
  
  if (isLoading) return <div>Loading...</div>;
  if (error) return <div>Error: {error.message}</div>;
  if (!user) return <div>Not authenticated</div>;
  
  return <ChatInterface user={user} />;
}
```

#### After
```typescript
// NEW - Automatic loading state from hooks
function AppContent() {
  const { isInitialized, isConnecting, error } = useInitializationStatus();
  const { user } = useAgentCData();
  
  if (error) return <div>Error: {error.message}</div>;
  if (isConnecting) return <div>Connecting...</div>;
  if (!isInitialized) return <div>Initializing...</div>;
  if (!user) return <div>Loading user data...</div>;
  
  return <ChatInterface user={user} />;
}
```

### Step 4: Remove Old Data Storage

#### Before
```typescript
// OLD - Manual data storage
class DataStore {
  private user: User | null = null;
  private voices: Voice[] = [];
  private agents: Agent[] = [];
  
  setLoginData(response: LoginResponse) {
    this.user = response.user;
    this.voices = response.voices;
    this.agents = response.agents;
    // Store in localStorage or state management
    localStorage.setItem('user', JSON.stringify(response.user));
    localStorage.setItem('voices', JSON.stringify(response.voices));
  }
  
  getUser(): User | null {
    if (!this.user) {
      const stored = localStorage.getItem('user');
      this.user = stored ? JSON.parse(stored) : null;
    }
    return this.user;
  }
}
```

#### After
```typescript
// NEW - Data managed by SDK
// No manual storage needed! The SDK handles everything
// Just use the hooks:
const { user, voices, agents } = useAgentCData();
```

### Step 5: Update Event Listeners

#### Before
```typescript
// OLD - Limited events, manual data management
client.on('connected', () => {
  console.log('Connected');
  // Had to manually fetch or use stored data
  const loginData = getStoredLoginData();
  updateUI(loginData);
});
```

#### After
```typescript
// NEW - Rich event system with automatic data
client.on('connected', () => {
  console.log('Connected - waiting for initialization events');
});

client.on('chat_user_data', (event) => {
  console.log('User data received:', event.user);
  // No need to manually store - SDK handles it
});

client.on('initialization:complete', () => {
  console.log('All initialization data received');
  // App is ready to use
});
```

## Complete Migration Example

Here's a complete before/after example of a chat application:

### Before (Old Pattern)

```tsx
// app.tsx - OLD VERSION
import React, { useState, useEffect } from 'react';
import { AuthManager, RealtimeClient } from '@agentc/realtime-core';

function App() {
  const [authManager] = useState(() => new AuthManager());
  const [client, setClient] = useState<RealtimeClient | null>(null);
  const [isLoading, setIsLoading] = useState(true);
  const [user, setUser] = useState(null);
  const [voices, setVoices] = useState([]);
  const [agents, setAgents] = useState([]);
  const [error, setError] = useState(null);

  const handleLogin = async (username: string, password: string) => {
    try {
      setIsLoading(true);
      
      // OLD: Login returned everything
      const loginResponse = await authManager.login({ username, password });
      
      // OLD: Extract and store all data from login
      setUser(loginResponse.user);
      setVoices(loginResponse.voices);
      setAgents(loginResponse.agents);
      
      // Store in localStorage for persistence
      localStorage.setItem('user', JSON.stringify(loginResponse.user));
      localStorage.setItem('voices', JSON.stringify(loginResponse.voices));
      localStorage.setItem('jwt_token', loginResponse.jwt_token);
      
      // Create and connect client
      const newClient = new RealtimeClient({
        apiUrl: loginResponse.websocket_url,
        authManager,
        config: {
          agents: loginResponse.agents,
          voices: loginResponse.voices
        }
      });
      
      await newClient.connect();
      setClient(newClient);
      setIsLoading(false);
    } catch (err) {
      setError(err.message);
      setIsLoading(false);
    }
  };
  
  // Try to restore session on mount
  useEffect(() => {
    const storedUser = localStorage.getItem('user');
    const storedVoices = localStorage.getItem('voices');
    const storedToken = localStorage.getItem('jwt_token');
    
    if (storedUser && storedToken) {
      setUser(JSON.parse(storedUser));
      setVoices(JSON.parse(storedVoices));
      // Would need to recreate client with stored data
    }
    setIsLoading(false);
  }, []);
  
  if (isLoading) {
    return <div>Loading...</div>;
  }
  
  if (!user) {
    return (
      <LoginForm onLogin={handleLogin} error={error} />
    );
  }
  
  return (
    <div className="app">
      <header>
        <h1>Welcome {user.display_name}</h1>
        <VoiceSelector voices={voices} />
        <AgentSelector agents={agents} />
      </header>
      <ChatInterface client={client} user={user} />
    </div>
  );
}

// VoiceSelector component
function VoiceSelector({ voices }) {
  const [selectedVoice, setSelectedVoice] = useState(voices[0]?.voice_id);
  
  return (
    <select value={selectedVoice} onChange={(e) => setSelectedVoice(e.target.value)}>
      {voices.map(voice => (
        <option key={voice.voice_id} value={voice.voice_id}>
          {voice.name}
        </option>
      ))}
    </select>
  );
}
```

### After (New Pattern)

```tsx
// app.tsx - NEW VERSION
import React from 'react';
import { AuthManager, RealtimeClient } from '@agentc/realtime-core';
import { 
  AgentCProvider, 
  useAgentCData, 
  useInitializationStatus,
  useConnection 
} from '@agentc/realtime-react';

function App() {
  const [authManager] = useState(() => new AuthManager());
  const [client, setClient] = useState<RealtimeClient | null>(null);

  const handleLogin = async (username: string, password: string) => {
    // NEW: Simple login - only returns tokens
    await authManager.login({ username, password });
    
    // NEW: Create client - data will come from events
    const newClient = new RealtimeClient({
      apiUrl: authManager.getWebSocketUrl(),
      authManager
    });
    
    setClient(newClient);
    await newClient.connect();
    // That's it! Data arrives via events
  };

  if (!client) {
    return <LoginForm onLogin={handleLogin} />;
  }

  return (
    <AgentCProvider client={client}>
      <AppContent />
    </AgentCProvider>
  );
}

function AppContent() {
  // NEW: Use built-in hooks for everything
  const { isInitialized, isConnecting, error } = useInitializationStatus();
  const { user, voices, agents } = useAgentCData();
  
  // NEW: Automatic loading states
  if (error) {
    return <div className="error">Error: {error.message}</div>;
  }
  
  if (isConnecting) {
    return <div className="loading">Connecting to Agent C...</div>;
  }
  
  if (!isInitialized) {
    return <div className="loading">Initializing...</div>;
  }
  
  // NEW: All data available from hooks
  return (
    <div className="app">
      <header>
        <h1>Welcome {user?.display_name}</h1>
        <VoiceSelector />
        <AgentSelector />
      </header>
      <ChatInterface />
    </div>
  );
}

// NEW: Voice selector using hooks
function VoiceSelector() {
  const { voices } = useAgentCData();
  const { voiceModel, setVoiceModel } = useVoiceModel();
  
  return (
    <select value={voiceModel} onChange={(e) => setVoiceModel(e.target.value)}>
      {voices.map(voice => (
        <option key={voice.voice_id} value={voice.voice_id}>
          {voice.name}
        </option>
      ))}
    </select>
  );
}

// NEW: Agent selector using hooks
function AgentSelector() {
  const { agents, currentAgent } = useAgentCData();
  const { selectAgent } = useAgent();
  
  return (
    <select value={currentAgent?.id} onChange={(e) => selectAgent(e.target.value)}>
      {agents.map(agent => (
        <option key={agent.id} value={agent.id}>
          {agent.name}
        </option>
      ))}
    </select>
  );
}
```

## Common Migration Issues

### Issue 1: "user is undefined"

**Problem**: Your code expects user data immediately after login.

**Solution**: Wait for initialization to complete.

```typescript
// ❌ OLD - Assumed user data was immediate
const user = loginResponse.user;
console.log(user.display_name); // Could be undefined

// ✅ NEW - Wait for initialization
const { user } = useAgentCData();
const { isInitialized } = useInitializationStatus();

if (isInitialized && user) {
  console.log(user.display_name); // Safe to access
}
```

### Issue 2: "voices/agents array is empty"

**Problem**: Trying to access voices or agents before events arrive.

**Solution**: Check initialization status first.

```typescript
// ❌ OLD - Might be empty
const firstVoice = voices[0]; // Could crash

// ✅ NEW - Safe access
const { voices } = useAgentCData();
const { isInitialized } = useInitializationStatus();

const firstVoice = isInitialized && voices.length > 0 ? voices[0] : null;
```

### Issue 3: "Cannot read properties of null"

**Problem**: Accessing nested properties before data loads.

**Solution**: Use optional chaining and default values.

```typescript
// ❌ OLD - Unsafe access
<div>{user.profile.avatar_url}</div>

// ✅ NEW - Safe access with optional chaining
<div>{user?.profile?.avatar_url || 'default-avatar.png'}</div>
```

### Issue 4: "State updates after unmount"

**Problem**: Components unmounting before initialization completes.

**Solution**: The new hooks handle cleanup automatically.

```typescript
// ❌ OLD - Manual cleanup needed
useEffect(() => {
  let mounted = true;
  
  authManager.login(credentials).then(response => {
    if (mounted) {
      setUser(response.user);
    }
  });
  
  return () => { mounted = false; };
}, []);

// ✅ NEW - Automatic cleanup in hooks
const { user } = useAgentCData(); // Handles cleanup automatically
```

## API Reference Changes

### Removed from Login Response

The following fields are no longer returned from `authManager.login()`:

- `user` - Now delivered via `chat_user_data` event
- `voices` - Now delivered via `voice_list` event  
- `agents` - Now delivered via `agent_list` event
- `avatars` - Now delivered via `avatar_list` event
- `tools` - Now delivered via `tool_catalog` event
- `sessions` - Now delivered via `chat_session_changed` event

### New Events

The following events are automatically sent after connection:

| Event | Description | Payload |
|-------|-------------|---------|
| `chat_user_data` | User profile information | `{ user: UserProfile }` |
| `voice_list` | Available voice models | `{ voices: Voice[] }` |
| `agent_list` | Available agents | `{ agents: Agent[] }` |
| `avatar_list` | Available avatars | `{ avatars: Avatar[] }` |
| `tool_catalog` | Available tools | `{ tools: Tool[] }` |
| `chat_session_changed` | Current session | `{ session: ChatSession }` |
| `initialization:complete` | All data received | `{}` |

### New Hooks

| Hook | Description | Returns |
|------|-------------|---------|
| `useAgentCData()` | Access all configuration data | `{ user, voices, agents, avatars, tools }` |
| `useInitializationStatus()` | Check initialization state | `{ isInitialized, isConnecting, error }` |
| `useUserData()` | Access user profile | `{ user, isLoading, error }` |

## Performance Improvements

The new architecture provides several performance benefits:

1. **Smaller Initial Payload**: Login response reduced by ~90%
2. **Parallel Data Loading**: All 6 events sent simultaneously  
3. **Progressive Rendering**: UI can update as each event arrives
4. **Reduced Memory Usage**: No duplicate data storage
5. **Automatic Caching**: SDK handles data caching internally

## Best Practices

### 1. Always Check Initialization Status

```typescript
function MyComponent() {
  const { isInitialized } = useInitializationStatus();
  const { user, voices } = useAgentCData();
  
  // Always check initialization before using data
  if (!isInitialized) {
    return <LoadingSpinner />;
  }
  
  // Now safe to use all data
  return <div>{user.display_name}</div>;
}
```

### 2. Use Optional Chaining

```typescript
// Safe property access
const userName = user?.display_name || 'Guest';
const avatarUrl = user?.profile?.avatar_url;
const firstVoice = voices?.[0]?.voice_id;
```

### 3. Handle Progressive Loading

```typescript
function ProgressiveUI() {
  const { user, voices, agents } = useAgentCData();
  
  return (
    <div>
      {/* Show user section when ready */}
      {user ? (
        <UserProfile user={user} />
      ) : (
        <Skeleton height={100} />
      )}
      
      {/* Show voices when ready */}
      {voices.length > 0 ? (
        <VoiceSelector voices={voices} />
      ) : (
        <Skeleton height={40} />
      )}
      
      {/* Show agents when ready */}
      {agents.length > 0 ? (
        <AgentList agents={agents} />
      ) : (
        <Skeleton height={200} />
      )}
    </div>
  );
}
```

### 4. Don't Store SDK Data

```typescript
// ❌ DON'T - Manual storage not needed
localStorage.setItem('user', JSON.stringify(user));
localStorage.setItem('voices', JSON.stringify(voices));

// ✅ DO - Let SDK manage data
const { user, voices } = useAgentCData();
```

## Testing Your Migration

### Unit Tests

```typescript
// Test initialization sequence
it('should receive all initialization events', async () => {
  const client = new RealtimeClient({ authManager });
  
  const events = [];
  client.on('chat_user_data', () => events.push('user'));
  client.on('voice_list', () => events.push('voices'));
  client.on('agent_list', () => events.push('agents'));
  client.on('initialization:complete', () => events.push('complete'));
  
  await client.connect();
  
  // Wait for initialization
  await waitFor(() => {
    expect(events).toContain('complete');
  });
  
  // Verify all events received
  expect(events).toContain('user');
  expect(events).toContain('voices');
  expect(events).toContain('agents');
});
```

### Integration Tests

```typescript
// Test the complete flow
it('should initialize app with new pattern', async () => {
  render(
    <AgentCProvider client={client}>
      <App />
    </AgentCProvider>
  );
  
  // Login
  const loginButton = screen.getByText('Login');
  fireEvent.click(loginButton);
  
  // Wait for initialization
  await waitFor(() => {
    expect(screen.getByText(/Welcome/)).toBeInTheDocument();
  });
  
  // Verify data is available
  expect(screen.getByText(/Select Voice/)).toBeInTheDocument();
  expect(screen.getByText(/Select Agent/)).toBeInTheDocument();
});
```

## Support

If you encounter issues during migration:

1. Check the console for initialization events
2. Verify you're using the latest SDK version
3. Ensure your backend is updated to send events
4. Review the troubleshooting section below

For additional help, contact the Agent C support team.
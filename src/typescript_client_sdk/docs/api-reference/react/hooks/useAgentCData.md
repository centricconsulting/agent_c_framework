# useAgentCData

## Purpose and Overview

The `useAgentCData` hook provides access to all WebSocket initialization data received from the Agent C Realtime API. This hook consolidates data from six initialization events that fire after WebSocket connection is established, providing a unified interface for accessing user information, available agents, voices, avatars, tools, and the current agent configuration.

This hook is essential for components that need to display or interact with the complete configuration state of the Agent C system. It automatically updates when any of the underlying data changes and provides loading states to handle the asynchronous nature of WebSocket initialization.

## Import Statement

```typescript
import { useAgentCData } from '@agentc/realtime-react';
// or with specific types
import { useAgentCData, type AgentCData, type UseAgentCDataReturn } from '@agentc/realtime-react';
```

## Complete TypeScript Types

### Core Interfaces

```typescript
/**
 * All configuration data from WebSocket initialization
 */
export interface AgentCData {
  /** Current user data */
  user: User | null;
  
  /** Available voices for text-to-speech */
  voices: Voice[];
  
  /** Available AI agents */
  agents: Agent[];
  
  /** Current agent configuration from active session */
  currentAgentConfig: AgentConfiguration | null;
  
  /** Available HeyGen avatars */
  avatars: Avatar[];
  
  /** Available tool sets */
  tools: Tool[];
}

/**
 * Return type for the useAgentCData hook
 */
export interface UseAgentCDataReturn {
  /** All configuration data */
  data: AgentCData;
  
  /** Whether initialization is still loading */
  isLoading: boolean;
  
  /** Whether initialization has completed */
  isInitialized: boolean;
  
  /** Error if initialization failed */
  error: string | null;
  
  /** Refresh the data from client */
  refresh: () => void;
}
```

### Data Structure Types

```typescript
// User information from authentication
interface User {
  user_id: string;
  user_name: string;
  email: string | null;
  first_name: string | null;
  last_name: string | null;
  is_active: boolean;
  roles: string[];
  groups: string[];
  created_at: string | null;
  last_login: string | null;
}

// Voice model configuration
interface Voice {
  voice_id: string;
  vendor: string;
  description: string;
  output_format: string;
}

// Agent catalog entry
interface Agent {
  name: string;
  key: string;
  agent_description: string | null;
  category: string[];
  tools: string[];
}

// Current agent configuration (V2)
interface AgentConfiguration {
  version: 2;
  name: string;
  key: string;
  model_id: string;
  agent_description?: string;
  tools: string[];
  blocked_tool_patterns?: string[];
  allowed_tool_patterns?: string[];
  agent_params?: CompletionParams;
  prompt_metadata?: Record<string, any>;
  persona: string;
  uid?: string;
  category: string[];
}

// HeyGen avatar information
interface Avatar {
  avatar_id: string;
  created_at: number;
  default_voice: string;
  is_public: boolean;
  normal_preview: string;
  pose_name: string;
  status: string;
}

// Tool definition
interface Tool {
  name: string;
  description: string;
  schemas: Record<string, ToolSchema>;
}

interface ToolSchema {
  type: 'function';
  function: {
    name: string;
    description: string;
    parameters: {
      type: 'object';
      properties: Record<string, any>;
      required?: string[];
    };
  };
}
```

## Usage Examples

### Basic Usage

```tsx
function AppConfiguration() {
  const { data, isLoading, isInitialized, error } = useAgentCData();
  
  if (error) {
    return <div className="error">Failed to load configuration: {error}</div>;
  }
  
  if (isLoading) {
    return <div className="loading">Loading configuration...</div>;
  }
  
  if (!isInitialized) {
    return <div className="waiting">Waiting for initialization...</div>;
  }
  
  return (
    <div className="config-panel">
      <h2>Welcome {data.user?.user_name || 'Guest'}</h2>
      <div className="stats">
        <p>Available agents: {data.agents.length}</p>
        <p>Available voices: {data.voices.length}</p>
        <p>Available avatars: {data.avatars.length}</p>
        <p>Available tools: {data.tools.length}</p>
        {data.currentAgentConfig && (
          <p>Current agent: {data.currentAgentConfig.name}</p>
        )}
      </div>
    </div>
  );
}
```

### Agent Selector Component

```tsx
function AgentSelector() {
  const { data, isInitialized } = useAgentCData();
  const [selectedAgent, setSelectedAgent] = useState<string>('');
  
  useEffect(() => {
    // Set default selection to current agent if available
    if (data.currentAgentConfig) {
      setSelectedAgent(data.currentAgentConfig.key);
    }
  }, [data.currentAgentConfig]);
  
  if (!isInitialized) {
    return <select disabled><option>Loading agents...</option></select>;
  }
  
  return (
    <select 
      value={selectedAgent} 
      onChange={(e) => setSelectedAgent(e.target.value)}
    >
      <option value="">Select an agent...</option>
      {data.agents.map(agent => (
        <option key={agent.key} value={agent.key}>
          {agent.name} - {agent.agent_description}
        </option>
      ))}
    </select>
  );
}
```

### Voice Configuration Component

```tsx
function VoiceConfiguration() {
  const { data, isInitialized } = useAgentCData();
  
  if (!isInitialized) {
    return <div>Loading voice options...</div>;
  }
  
  // Group voices by vendor
  const voicesByVendor = data.voices.reduce((acc, voice) => {
    if (!acc[voice.vendor]) {
      acc[voice.vendor] = [];
    }
    acc[voice.vendor].push(voice);
    return acc;
  }, {} as Record<string, Voice[]>);
  
  return (
    <div className="voice-config">
      <h3>Available Voices</h3>
      {Object.entries(voicesByVendor).map(([vendor, voices]) => (
        <div key={vendor}>
          <h4>{vendor}</h4>
          <ul>
            {voices.map(voice => (
              <li key={voice.voice_id}>
                {voice.description} ({voice.output_format})
              </li>
            ))}
          </ul>
        </div>
      ))}
    </div>
  );
}
```

### Avatar Gallery Component

```tsx
function AvatarGallery() {
  const { data, isInitialized, refresh } = useAgentCData();
  const [selectedAvatar, setSelectedAvatar] = useState<string | null>(null);
  
  if (!isInitialized) {
    return <div>Loading avatars...</div>;
  }
  
  if (data.avatars.length === 0) {
    return (
      <div>
        <p>No avatars available</p>
        <button onClick={refresh}>Refresh</button>
      </div>
    );
  }
  
  return (
    <div className="avatar-gallery">
      <h3>Available Avatars ({data.avatars.length})</h3>
      <div className="avatar-grid">
        {data.avatars.map(avatar => (
          <div 
            key={avatar.avatar_id}
            className={`avatar-card ${selectedAvatar === avatar.avatar_id ? 'selected' : ''}`}
            onClick={() => setSelectedAvatar(avatar.avatar_id)}
          >
            <img src={avatar.normal_preview} alt={`Avatar ${avatar.pose_name}`} />
            <div className="avatar-info">
              <p className="pose-name">{avatar.pose_name}</p>
              <p className="status">{avatar.status}</p>
              {avatar.is_public && <span className="badge">Public</span>}
            </div>
          </div>
        ))}
      </div>
    </div>
  );
}
```

### Tool Catalog Display

```tsx
function ToolCatalog() {
  const { data, isInitialized } = useAgentCData();
  
  if (!isInitialized) {
    return <div>Loading tool catalog...</div>;
  }
  
  return (
    <div className="tool-catalog">
      <h3>Available Tools</h3>
      {data.tools.map(tool => (
        <details key={tool.name} className="tool-details">
          <summary>
            <strong>{tool.name}</strong> - {tool.description}
          </summary>
          <div className="tool-schemas">
            {Object.entries(tool.schemas).map(([key, schema]) => (
              <div key={key} className="schema">
                <h5>{schema.function.name}</h5>
                <p>{schema.function.description}</p>
                {schema.function.parameters.required && (
                  <p>Required: {schema.function.parameters.required.join(', ')}</p>
                )}
              </div>
            ))}
          </div>
        </details>
      ))}
    </div>
  );
}
```

### User Profile with Role Check

```tsx
function UserProfile() {
  const { data, isInitialized, error } = useAgentCData();
  
  if (error) {
    return <div className="error">Unable to load user profile</div>;
  }
  
  if (!isInitialized || !data.user) {
    return <div>Loading user profile...</div>;
  }
  
  const isAdmin = data.user.roles.includes('admin');
  const hasActiveSession = data.user.is_active;
  
  return (
    <div className="user-profile">
      <h2>{data.user.first_name} {data.user.last_name}</h2>
      <p>Username: {data.user.user_name}</p>
      <p>Email: {data.user.email || 'Not provided'}</p>
      
      <div className="user-status">
        {hasActiveSession ? (
          <span className="status-active">Active</span>
        ) : (
          <span className="status-inactive">Inactive</span>
        )}
        
        {isAdmin && <span className="badge-admin">Admin</span>}
      </div>
      
      <div className="user-meta">
        <p>Member since: {data.user.created_at ? new Date(data.user.created_at).toLocaleDateString() : 'Unknown'}</p>
        <p>Last login: {data.user.last_login ? new Date(data.user.last_login).toLocaleDateString() : 'Never'}</p>
      </div>
      
      {data.user.groups.length > 0 && (
        <div className="user-groups">
          <h4>Groups:</h4>
          <ul>
            {data.user.groups.map(group => (
              <li key={group}>{group}</li>
            ))}
          </ul>
        </div>
      )}
    </div>
  );
}
```

## Initialization Event Dependencies

The `useAgentCData` hook depends on the following six WebSocket initialization events that fire after connection:

1. **`chat_user_data`** - Provides user profile information
2. **`avatar_list`** - Provides available HeyGen avatars
3. **`voice_list`** - Provides available text-to-speech voices
4. **`agent_list`** - Provides available AI agents
5. **`tool_catalog`** - Provides available tool definitions
6. **`chat_session_changed`** - Provides current session with agent configuration

The hook will not report `isInitialized: true` until all six events have been received. This ensures that components can rely on having complete configuration data when the hook reports as initialized.

### Event Flow

```typescript
// Connection established
client.connect() → 'connected' event

// Server sends initialization events (order may vary)
→ 'chat_user_data' event → updates data.user
→ 'avatar_list' event → updates data.avatars
→ 'voice_list' event → updates data.voices
→ 'agent_list' event → updates data.agents
→ 'tool_catalog' event → updates data.tools
→ 'chat_session_changed' event → updates data.currentAgentConfig

// After all events received
→ 'initialized' event → sets isInitialized: true, isLoading: false
```

## Data Freshness and Synchronization

### Automatic Updates

The hook automatically updates its data when:
- Any of the initialization events are received
- The session changes (via SessionManager events)
- Agent configuration changes
- The connection is re-established after disconnection

### Manual Refresh

The `refresh()` function can be called to manually sync data from the client:

```tsx
function RefreshButton() {
  const { refresh, isLoading } = useAgentCData();
  
  return (
    <button onClick={refresh} disabled={isLoading}>
      Refresh Configuration
    </button>
  );
}
```

### Data Persistence

- Data persists during temporary disconnections
- The `isLoading` flag indicates active data fetching
- The `isInitialized` flag only becomes true after all six events are received
- Error states are cleared when connection is re-established

## StrictMode Compatibility

The `useAgentCData` hook is fully compatible with React StrictMode. It properly handles:

- **Double mounting**: Effect cleanup prevents duplicate event subscriptions
- **Double effect execution**: Uses stable callback references with `useCallback`
- **State updates**: All state updates are idempotent and don't cause side effects

Example with StrictMode:

```tsx
// App.tsx
import { StrictMode } from 'react';
import { AgentCProvider } from '@agentc/realtime-react';

function App() {
  return (
    <StrictMode>
      <AgentCProvider config={config}>
        <ConfigurationPanel />
      </AgentCProvider>
    </StrictMode>
  );
}

// ConfigurationPanel.tsx
function ConfigurationPanel() {
  // Safe to use in StrictMode - no duplicate subscriptions
  const { data, isInitialized } = useAgentCData();
  
  // Component logic...
}
```

## Best Practices for Null Checking and Error Handling

### Defensive Programming

Always check for null values and handle loading states:

```tsx
function SafeAgentDisplay() {
  const { data, isInitialized, error } = useAgentCData();
  
  // 1. Check error state first
  if (error) {
    return <ErrorBoundary error={error} />;
  }
  
  // 2. Check initialization state
  if (!isInitialized) {
    return <LoadingSpinner />;
  }
  
  // 3. Defensive null checks for optional data
  const userName = data.user?.user_name ?? 'Guest';
  const agentName = data.currentAgentConfig?.name ?? 'No agent selected';
  const agentTools = data.currentAgentConfig?.tools ?? [];
  
  return (
    <div>
      <h2>Hello, {userName}</h2>
      <p>Current agent: {agentName}</p>
      {agentTools.length > 0 && (
        <p>Agent has {agentTools.length} tools available</p>
      )}
    </div>
  );
}
```

### Type Guards

Use type guards for safer data access:

```tsx
function AgentToolList() {
  const { data, isInitialized } = useAgentCData();
  
  if (!isInitialized || !data.currentAgentConfig) {
    return null;
  }
  
  // Type guard for V2 configuration
  const isV2Config = data.currentAgentConfig.version === 2;
  
  if (isV2Config) {
    // Safe to access V2-specific fields
    const blockedPatterns = data.currentAgentConfig.blocked_tool_patterns ?? [];
    const allowedPatterns = data.currentAgentConfig.allowed_tool_patterns ?? [];
    
    return (
      <div>
        <h3>Tool Configuration</h3>
        {blockedPatterns.length > 0 && (
          <p>Blocked patterns: {blockedPatterns.join(', ')}</p>
        )}
        {allowedPatterns.length > 0 && (
          <p>Allowed patterns: {allowedPatterns.join(', ')}</p>
        )}
      </div>
    );
  }
  
  return null;
}
```

### Error Boundary Integration

Wrap components using the hook with error boundaries:

```tsx
import { ErrorBoundary } from 'react-error-boundary';

function ConfigurationView() {
  return (
    <ErrorBoundary 
      fallback={<div>Configuration failed to load</div>}
      onError={(error) => console.error('Configuration error:', error)}
    >
      <AgentCDataConsumer />
    </ErrorBoundary>
  );
}

function AgentCDataConsumer() {
  const { data, isInitialized, error } = useAgentCData();
  
  if (error) {
    throw new Error(`Failed to load configuration: ${error}`);
  }
  
  // Component logic...
}
```

### Graceful Degradation

Design components to work with partial data:

```tsx
function ResilientAgentSelector() {
  const { data, isInitialized, error, refresh } = useAgentCData();
  
  // Show limited functionality even if not fully initialized
  const agents = data.agents || [];
  const hasAgents = agents.length > 0;
  
  return (
    <div className="agent-selector">
      {error && (
        <div className="error-message">
          {error}
          <button onClick={refresh}>Retry</button>
        </div>
      )}
      
      {!isInitialized && !error && (
        <div className="loading-state">
          Connecting to Agent C service...
        </div>
      )}
      
      {hasAgents ? (
        <select>
          {agents.map(agent => (
            <option key={agent.key} value={agent.key}>
              {agent.name}
            </option>
          ))}
        </select>
      ) : (
        <div className="empty-state">
          No agents available
          {isInitialized && (
            <button onClick={refresh}>Check Again</button>
          )}
        </div>
      )}
    </div>
  );
}
```

## Performance Considerations

- The hook updates efficiently using granular event handlers
- Data is cached in the RealtimeClient instance, not duplicated
- Use the `refresh()` function sparingly as it queries all managers
- Consider memoizing derived values from the data object:

```tsx
function OptimizedAgentList() {
  const { data, isInitialized } = useAgentCData();
  
  // Memoize expensive computations
  const agentsByCategory = useMemo(() => {
    if (!isInitialized) return {};
    
    return data.agents.reduce((acc, agent) => {
      const category = agent.category[0] || 'Uncategorized';
      if (!acc[category]) {
        acc[category] = [];
      }
      acc[category].push(agent);
      return acc;
    }, {} as Record<string, Agent[]>);
  }, [data.agents, isInitialized]);
  
  // Component render...
}
```

## See Also

- [`useUserData`](./useUserData.md) - For accessing just user data
- [`useRealtimeClient`](./useRealtimeClient.md) - For direct client access
- [`useConnection`](./useConnection.md) - For connection state management
- [AgentCProvider](../providers/AgentCProvider.md) - Provider setup and configuration
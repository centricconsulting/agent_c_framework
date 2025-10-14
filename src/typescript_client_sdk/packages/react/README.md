# @agentc/realtime-react

React integration for the Agent C Realtime SDK. Provides hooks, providers, and utilities for building real-time voice and chat interfaces with AI agents.

## Documentation

📚 **[View Full Documentation](../../docs/api-reference/react/)**

The comprehensive documentation includes:
- [Complete Hooks Reference](../../docs/api-reference/react/hooks.md)
- [Provider API](../../docs/api-reference/react/AgentCProvider.md)
- [Working Examples](../../docs/api-reference/react/examples.md)
- Individual hook documentation with detailed examples

## Installation

```bash
npm install @agentc/realtime-react @agentc/realtime-core
```

## Quick Start

```tsx
import { AgentCProvider, useChat, useAudio, useInitializationStatus } from '@agentc/realtime-react';
import { AuthManager, RealtimeClient } from '@agentc/realtime-core';

function App() {
  const [client, setClient] = useState<RealtimeClient | null>(null);
  
  const handleLogin = async (credentials) => {
    const authManager = new AuthManager();
    await authManager.login(credentials);
    
    const newClient = new RealtimeClient({
      apiUrl: authManager.getWebSocketUrl(),
      authManager
    });
    
    setClient(newClient);
    await newClient.connect();
  };
  
  if (!client) {
    return <LoginForm onLogin={handleLogin} />;
  }
  
  return (
    <AgentCProvider client={client}>
      <ChatInterface />
    </AgentCProvider>
  );
}

function ChatInterface() {
  const { isInitialized } = useInitializationStatus();
  const { messages, sendMessage } = useChat();
  const { startRecording, stopRecording, isRecording } = useAudio();
  
  if (!isInitialized) {
    return <div>Initializing...</div>;
  }
  
  return (
    <div>
      {/* Message display */}
      {messages.map(msg => (
        <div key={msg.id}>{msg.role}: {msg.content}</div>
      ))}
      
      {/* Voice input */}
      <button onClick={() => isRecording ? stopRecording() : startRecording()}>
        {isRecording ? '🔴 Stop' : '🎤 Start'}
      </button>
      
      {/* Text input */}
      <input onKeyPress={(e) => {
        if (e.key === 'Enter') {
          sendMessage(e.target.value);
          e.target.value = '';
        }
      }} />
    </div>
  );
}
```

## Available Hooks

The React package provides 14 hooks for different aspects of the realtime experience:

### Core Hooks
- [`useRealtimeClient`](../../docs/api-reference/react/hooks/useRealtimeClient.md) - Direct access to the RealtimeClient instance
- [`useInitializationStatus`](../../docs/api-reference/react/hooks/useInitializationStatus.md) - Track SDK initialization state
- [`useConnection`](../../docs/api-reference/react/hooks/useConnection.md) - Connection state and statistics

### Data Access Hooks  
- [`useAgentCData`](../../docs/api-reference/react/hooks/useAgentCData.md) - Access all configuration data (users, voices, agents, etc.)
- [`useUserData`](../../docs/api-reference/react/hooks/useUserData.md) - User profile information

### Chat & Messaging
- [`useChat`](../../docs/api-reference/react/hooks/useChat.md) - Complete chat functionality (messages, typing indicators)
- [`useChatSessionList`](../../docs/api-reference/react/hooks/useChatSessionList.md) - Manage chat sessions
- [`useChatSessionListOptimized`](../../docs/api-reference/react/hooks/useChatSessionListOptimized.md) - Optimized session management

### Audio & Voice
- [`useAudio`](../../docs/api-reference/react/hooks/useAudio.md) - Audio recording and playback control
- [`useTurnState`](../../docs/api-reference/react/hooks/useTurnState.md) - Conversation turn management
- [`useVoiceModel`](../../docs/api-reference/react/hooks/useVoiceModel.md) - Voice model selection
- [`useOutputMode`](../../docs/api-reference/react/hooks/useOutputMode.md) - Control audio/text output mode

### Advanced Features
- [`useAvatar`](../../docs/api-reference/react/hooks/useAvatar.md) - HeyGen avatar integration
- [`useToolNotifications`](../../docs/api-reference/react/hooks/useToolNotifications.md) - Tool execution notifications

## Core Concepts

### Provider-Based Architecture

All hooks require the `AgentCProvider` at the root of your component tree:

```tsx
import { AgentCProvider } from '@agentc/realtime-react';
import { RealtimeClient } from '@agentc/realtime-core';

<AgentCProvider client={realtimeClient}>
  {/* All child components can use hooks */}
</AgentCProvider>
```

### Event-Driven Data Flow

After connection, the SDK automatically receives initialization events with configuration data:

1. `chat_user_data` - User profile
2. `voice_list` - Available voices  
3. `agent_list` - Available agents
4. `avatar_list` - Available avatars
5. `tool_catalog` - Available tools
6. `chat_session_changed` - Current session

Access this data using the `useAgentCData()` hook.

## Basic Examples

### Authentication Setup

```tsx
import { AuthManager, RealtimeClient } from '@agentc/realtime-core';
import { AgentCProvider } from '@agentc/realtime-react';

async function setupClient(credentials) {
  const authManager = new AuthManager();
  await authManager.login(credentials);
  
  const client = new RealtimeClient({
    apiUrl: authManager.getWebSocketUrl(),
    authManager
  });
  
  await client.connect();
  return client;
}
```

### Using Multiple Hooks

```tsx
function ChatApp() {
  const { isInitialized } = useInitializationStatus();
  const { user, voices } = useAgentCData();
  const { messages, sendMessage } = useChat();
  const { isRecording, startRecording, stopRecording } = useAudio();
  const { voiceModel, setVoiceModel } = useVoiceModel();
  const { turnState } = useTurnState();
  
  if (!isInitialized) {
    return <div>Loading...</div>;
  }
  
  return (
    <div>
      <h1>Welcome, {user?.display_name}</h1>
      <VoiceSelector voices={voices} value={voiceModel} onChange={setVoiceModel} />
      <Messages messages={messages} />
      <Controls 
        isRecording={isRecording}
        canRecord={turnState === 'user_turn'}
        onStart={startRecording}
        onStop={stopRecording}
        onSend={sendMessage}
      />
    </div>
  );
}
```

### Voice-Only Interface

```tsx
function VoiceInterface() {
  const { isInitialized } = useInitializationStatus();
  const { isRecording, startRecording, stopRecording } = useAudio();
  const { turnState, isUserTurn } = useTurnState();
  const { setOutputMode } = useOutputMode();
  
  useEffect(() => {
    // Enable audio output for voice interface
    setOutputMode('audio');
  }, [setOutputMode]);
  
  if (!isInitialized) return <div>Initializing...</div>;
  
  return (
    <button
      onClick={() => isRecording ? stopRecording() : startRecording()}
      disabled={!isUserTurn && !isRecording}
    >
      {isRecording ? '🔴 Stop' : '🎤 Speak'}
    </button>
  );
}
```

## TypeScript Support

All hooks and components are fully typed. Import types directly:

```tsx
import type {
  AgentCData,
  InitializationStatus,
  ChatMessage,
  Voice,
  Agent,
  TurnState,
  OutputMode,
  ToolNotification
} from '@agentc/realtime-react';
```

## Best Practices

### 1. Always Check Initialization

```tsx
function SafeComponent() {
  const { isInitialized } = useInitializationStatus();
  
  if (!isInitialized) {
    return <LoadingSpinner />;
  }
  
  // Safe to use other hooks here
  return <Content />;
}
```

### 2. Handle Connection States

```tsx
function RobustComponent() {
  const { isInitialized, error } = useInitializationStatus();
  const { isConnected } = useConnection();
  
  if (error) return <ErrorBoundary error={error} />;
  if (!isConnected) return <Reconnecting />;
  if (!isInitialized) return <Initializing />;
  
  return <MainContent />;
}
```

### 3. Respect Turn Management

```tsx
function TurnAwareControls() {
  const { isUserTurn } = useTurnState();
  const { startRecording, isRecording } = useAudio();
  
  return (
    <button 
      onClick={startRecording}
      disabled={!isUserTurn || isRecording}
    >
      Start Speaking
    </button>
  );
}
```

## Advanced Topics

For advanced usage, see the full documentation:

- [Session Management](../../docs/api-reference/react/hooks/useChatSessionList.md)
- [Tool Notifications](../../docs/api-reference/react/hooks/useToolNotifications.md) 
- [Avatar Integration](../../docs/api-reference/react/hooks/useAvatar.md)
- [Custom Event Handling](../../docs/api-reference/react/hooks.md#event-handling)
- [Testing React Components](../../docs/testing/testing-react-components.md)

## Migration from v1

Key changes from v1:

1. **Authentication** - Login only returns tokens, no user data
2. **Automatic Events** - Configuration arrives via WebSocket events
3. **New Hooks** - Use `useAgentCData()` for all initialization data
4. **No Storage Required** - SDK manages all state internally

See the [Migration Guide](../../docs/guides/authentication-migration.md) for details.

## Examples

For complete working examples, see:

- [Full Chat Application](../../docs/api-reference/react/examples.md#chat-application)
- [Voice-Only Interface](../../docs/api-reference/react/examples.md#voice-interface)
- [Session Management](../../docs/api-reference/react/examples.md#session-management)
- [Error Handling](../../docs/api-reference/react/examples.md#error-handling)

## API Reference

- **[Complete Hooks Reference](../../docs/api-reference/react/hooks.md)** - All hooks with detailed API
- **[Provider API](../../docs/api-reference/react/AgentCProvider.md)** - AgentCProvider configuration
- **[TypeScript Types](../../docs/api-reference/react/hooks.md#typescript)** - Complete type definitions

## Contributing

See our [Testing Standards](../../docs/testing/testing-standards-and-architecture.md) for guidelines on writing tests for React components and hooks.

## License

MIT
# Agent C Realtime React SDK

The official React SDK for building real-time AI agent applications with the Agent C platform.

## Overview

The `@agentc/realtime-react` package provides a complete React integration layer for the Agent C Realtime Client SDK. It offers:

- **Provider-based architecture** with automatic connection management
- **Comprehensive hooks** for all realtime features
- **StrictMode compatibility** with proper cleanup handling
- **TypeScript-first design** with full type safety
- **Turn management** for coordinated audio conversations
- **Avatar integration** for visual agent interactions
- **Automatic reconnection** with exponential backoff
- **Built-in error handling** and recovery

## Quick Start

### Installation

```bash
npm install @agentc/realtime-react @agentc/realtime-core
# or
pnpm add @agentc/realtime-react @agentc/realtime-core
```

### Basic Setup

```tsx
import { AgentCProvider } from '@agentc/realtime-react';

function App() {
  return (
    <AgentCProvider
      apiKey={process.env.NEXT_PUBLIC_AGENTC_API_KEY}
      agentId={process.env.NEXT_PUBLIC_AGENTC_AGENT_ID}
      config={{
        audioEnabled: true,
        enableReconnection: true,
        maxReconnectAttempts: 5
      }}
    >
      <YourApplication />
    </AgentCProvider>
  );
}
```

### Using Hooks

```tsx
import { useConnection, useChat, useAudio } from '@agentc/realtime-react';

function ChatInterface() {
  const { isConnected, connect, disconnect } = useConnection();
  const { messages, sendMessage } = useChat();
  const { isMuted, toggleMute, isListening } = useAudio();

  // Your UI implementation
}
```

## Documentation Structure

### Core Components

- **[AgentCProvider](./AgentCProvider.md)** - The root provider component that manages the realtime client instance and connection lifecycle

### Hooks Documentation

- **[Hooks Overview](./hooks.md)** - Quick reference for all available hooks
- **[Hooks Patterns Guide](./hooks-overview.md)** - Comprehensive patterns and best practices

#### Individual Hook References

##### Connection & Lifecycle
- **[useRealtimeClient](./hooks/useRealtimeClient.md)** - Direct access to the client instance
- **[useConnection](./hooks/useConnection.md)** - Connection state and management
- **[useInitializationStatus](./hooks/useInitializationStatus.md)** - Track initialization progress

##### Chat & Messaging
- **[useChat](./hooks/useChat.md)** - Message history and text communication
- **[useChatSessionList](./hooks/useChatSessionList.md)** - Manage multiple chat sessions
- **[useChatSessionListOptimized](./hooks/useChatSessionListOptimized.md)** - Performance-optimized session management

##### Audio & Voice
- **[useAudio](./hooks/useAudio.md)** - Audio controls with turn awareness
- **[useTurnState](./hooks/useTurnState.md)** - Turn management for conversations
- **[useVoiceModel](./hooks/useVoiceModel.md)** - Voice selection and configuration
- **[useOutputMode](./hooks/useOutputMode.md)** - Control output modality (text/audio)

##### Avatar & Visual
- **[useAvatar](./hooks/useAvatar.md)** - HeyGen avatar integration

##### Data & Notifications
- **[useAgentCData](./hooks/useAgentCData.md)** - Access agent configuration data
- **[useUserData](./hooks/useUserData.md)** - User profile and preferences
- **[useToolNotifications](./hooks/useToolNotifications.md)** - Real-time tool execution updates

### Examples & Patterns

- **[Examples](./examples.md)** - Complete implementation examples and common use cases

## Key Concepts

### Authentication & Initialization

The SDK supports multiple authentication methods:

```tsx
// API Key authentication
<AgentCProvider apiKey={apiKey} agentId={agentId}>

// JWT authentication with refresh
<AgentCProvider
  getAuthToken={async () => {
    const response = await fetch('/api/auth/token');
    const { accessToken, expiresIn } = await response.json();
    return { accessToken, expiresIn };
  }}
  agentId={agentId}
>
```

### Turn Management

The SDK implements server-driven turn control to prevent talk-over in audio conversations:

```tsx
const { currentTurn, canSpeak, isAgentSpeaking } = useTurnState();

// UI responds to turn state
<button 
  onClick={startSpeaking}
  disabled={!canSpeak || isAgentSpeaking}
>
  Push to Talk
</button>
```

### Connection Lifecycle

The provider handles the complete connection lifecycle:

1. **Initialization** - Creates client instance, validates authentication
2. **Connection** - Establishes WebSocket connection when ready
3. **Reconnection** - Automatic retry with exponential backoff
4. **Cleanup** - Proper disposal on unmount or navigation

## TypeScript Support

The SDK is built with TypeScript and provides comprehensive type definitions:

```tsx
import type { 
  RealtimeClient,
  ConnectionState,
  ChatMessage,
  TurnState,
  VoiceModel
} from '@agentc/realtime-react';

// All hooks return fully typed responses
const { messages }: { messages: ChatMessage[] } = useChat();
```

### Type Exports

- Event types for all realtime events
- Configuration interfaces
- State type definitions
- Error types with discriminated unions
- Utility types for common patterns

## Common Patterns & Best Practices

### 1. Conditional Rendering Based on Connection

```tsx
function App() {
  const { isConnected, isConnecting } = useConnection();

  if (isConnecting) return <LoadingSpinner />;
  if (!isConnected) return <ConnectButton />;
  
  return <ChatInterface />;
}
```

### 2. Error Boundary Integration

```tsx
<ErrorBoundary fallback={<ErrorDisplay />}>
  <AgentCProvider apiKey={apiKey} agentId={agentId}>
    <App />
  </AgentCProvider>
</ErrorBoundary>
```

### 3. Audio Permission Handling

```tsx
function AudioControls() {
  const { error, isListening, toggleMute } = useAudio();
  
  if (error?.type === 'PERMISSION_DENIED') {
    return <RequestMicrophoneAccess />;
  }
  
  // Normal audio controls
}
```

### 4. Optimistic UI Updates

```tsx
function ChatInput() {
  const { sendMessage, messages } = useChat();
  
  const handleSend = async (text: string) => {
    // Optimistically add message to UI
    const optimisticMessage = { text, role: 'user', id: tempId() };
    
    try {
      await sendMessage(text);
    } catch (error) {
      // Revert optimistic update on failure
      rollbackMessage(optimisticMessage.id);
    }
  };
}
```

## Troubleshooting Guide

### Common Issues

#### Connection Issues

**Problem**: Client won't connect
```tsx
// Check authentication
const { error } = useConnection();
if (error?.code === 'AUTH_FAILED') {
  // Refresh token or re-authenticate
}
```

**Problem**: Frequent disconnections
```tsx
// Adjust reconnection settings
<AgentCProvider
  config={{
    enableReconnection: true,
    maxReconnectAttempts: 10,
    reconnectDelay: 1000,
    reconnectDelayMax: 30000
  }}
>
```

#### Audio Issues

**Problem**: Microphone not working
```tsx
const { error } = useAudio();

// Check for permission issues
if (error?.type === 'PERMISSION_DENIED') {
  // Prompt user to grant permissions
}

// Check for device issues
if (error?.type === 'DEVICE_NOT_FOUND') {
  // No microphone available
}
```

**Problem**: Echo or feedback
```tsx
// Enable echo cancellation
<AgentCProvider
  config={{
    audio: {
      echoCancellation: true,
      noiseSuppression: true,
      autoGainControl: true
    }
  }}
>
```

#### React StrictMode Issues

The SDK is fully StrictMode compatible. If you see double connections:

```tsx
// This is handled automatically by the provider
// No action needed - the SDK properly cleans up the extra mount
```

### Debug Mode

Enable detailed logging for troubleshooting:

```tsx
<AgentCProvider
  config={{
    debug: true,
    logLevel: 'debug'
  }}
>
```

## Performance Considerations

### Bundle Size

The React SDK is tree-shakeable. Import only what you need:

```tsx
// ✅ Good - only imports used hooks
import { useChat, useAudio } from '@agentc/realtime-react';

// ❌ Avoid - imports entire package
import * as AgentC from '@agentc/realtime-react';
```

### Render Optimization

Hooks are optimized to minimize re-renders:

```tsx
// Messages are memoized and only update when new messages arrive
const { messages } = useChat();

// Connection state updates are batched
const { stats } = useConnection();
```

### Large Session Management

For applications with many sessions, use the optimized hook:

```tsx
// For 50+ concurrent sessions
import { useChatSessionListOptimized } from '@agentc/realtime-react';
```

## Migration Guide

### From v1.x to v2.x

Key changes:
- Provider now requires explicit `agentId`
- Authentication callback signature changed
- Turn management is now mandatory for audio

See the full [migration guide](../../migration-guide.md) for details.

## Related Documentation

### SDK Documentation
- **[Core SDK Reference](../core/README.md)** - Low-level client documentation
- **[UI Components](../ui/README.md)** - Pre-built UI components
- **[TypeScript Types](../types/README.md)** - Complete type definitions

### API Documentation
- **[Realtime API Guide](../../../../api/docs/realtime_api_implementation_guide.md)** - Server API documentation
- **[Event Reference](../core/events.md)** - Complete event catalog
- **[WebSocket Protocol](../core/websocket.md)** - Protocol specifications

### Guides & Tutorials
- **[Getting Started Guide](../../getting-started.md)** - Step-by-step tutorial
- **[Authentication Guide](../../guides/authentication.md)** - Complete auth patterns
- **[Audio Integration Guide](../../guides/audio-integration.md)** - Audio setup and configuration
- **[Testing Guide](../../testing-guide.md)** - Testing React components with the SDK

### Design & Architecture
- **[Architecture Overview](../../design_docs/architecture.md)** - System design
- **[State Management](../../design_docs/state-management.md)** - State flow and updates
- **[Security Model](../../design_docs/security.md)** - Security considerations

## Support & Resources

- **[GitHub Repository](https://github.com/youragentc/realtime-client-sdk)**
- **[NPM Package](https://www.npmjs.com/package/@agentc/realtime-react)**
- **[Discord Community](https://discord.gg/agentc)**
- **[Stack Overflow Tag](https://stackoverflow.com/questions/tagged/agentc)**

## License

MIT © Agent C, Inc.
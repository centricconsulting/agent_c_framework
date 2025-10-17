# React Hooks API Reference

## Overview

The Agent C Realtime React SDK provides a comprehensive set of hooks for building realtime applications. All hooks must be used within an [`AgentCProvider`](./providers/AgentCProvider.md) context.

For a comprehensive overview and usage patterns, see the [Hooks Overview Guide](./hooks-overview.md).

## Available Hooks

### Core Hooks
- [`useRealtimeClient`](./hooks/useRealtimeClient.md) - Direct access to the RealtimeClient instance
- [`useConnection`](./hooks/useConnection.md) - Connection state management and statistics
- [`useInitializationStatus`](./hooks/useInitializationStatus.md) - SDK initialization tracking

### Audio & Voice Hooks
- [`useAudio`](./hooks/useAudio.md) - Audio recording and playback control
- [`useVoiceModel`](./hooks/useVoiceModel.md) - Voice model selection and management

### Conversation Hooks
- [`useTurnState`](./hooks/useTurnState.md) - Turn management and speaking indicators
- [`useOutputMode`](./hooks/useOutputMode.md) - Output mode control (text/audio/avatar)

### Chat Hooks
- [`useChat`](./hooks/useChat.md) - Message history and text messaging
- [`useChatSessionList`](./hooks/useChatSessionList.md) - Chat session management
- [`useChatSessionListOptimized`](./hooks/useChatSessionListOptimized.md) - Optimized session list for large datasets

### Data Management Hooks
- [`useAgentCData`](./hooks/useAgentCData.md) - Access Agent C configuration data
- [`useUserData`](./hooks/useUserData.md) - User profile and preferences

### Special Features
- [`useAvatar`](./hooks/useAvatar.md) - HeyGen avatar integration
- [`useToolNotifications`](./hooks/useToolNotifications.md) - Tool call tracking and notifications

## Quick Start

### Installation

```bash
npm install @agentc/realtime-react
```

### Basic Setup

```tsx
import { AgentCProvider, useChat, useAudio, useConnection } from '@agentc/realtime-react';
import { RealtimeClient } from '@agentc/realtime-core';

// Create client instance
const client = new RealtimeClient({
  apiKey: 'your-api-key'
});

// Wrap your app with the provider
function App() {
  return (
    <AgentCProvider client={client}>
      <ChatInterface />
    </AgentCProvider>
  );
}

// Use hooks in your components
function ChatInterface() {
  const { messages, sendMessage } = useChat();
  const { isRecording, startRecording, stopRecording } = useAudio();
  const { isConnected } = useConnection();
  
  if (!isConnected) {
    return <div>Connecting...</div>;
  }
  
  return (
    <div>
      {/* Your chat UI */}
    </div>
  );
}
```

## Common Patterns

### Voice-Enabled Chat
Combine chat and audio hooks for voice interactions:
```tsx
const { messages } = useChat();
const { isRecording, startRecording } = useAudio();
const { currentTurn } = useTurnState();
```

### Session Management
Handle multiple conversations:
```tsx
const { sessions, activeSession, createSession } = useChatSessionList();
const { messages } = useChat();
```

### Connection Monitoring
Track connection health:
```tsx
const { isConnected, connectionState, latency } = useConnection();
const { initializationStatus } = useInitializationStatus();
```

## TypeScript Support

All hooks are fully typed. Import types from the package:

```tsx
import type { 
  ConnectionState, 
  Message, 
  TurnState,
  Voice,
  ChatSession 
} from '@agentc/realtime-react';
```

## Best Practices

1. **Check Initialization State**: Always verify the SDK is initialized before using data hooks
2. **Handle Loading States**: Provide feedback while data is loading
3. **Use Error Boundaries**: Wrap components with error boundaries for robust error handling
4. **Optimize Re-renders**: Destructure only the values you need from hooks
5. **Clean Up Effects**: Always clean up event listeners and subscriptions

## Resources

- [Hooks Overview Guide](./hooks-overview.md) - Comprehensive guide with patterns and examples
- [Provider Documentation](./providers/AgentCProvider.md) - Provider setup and configuration
- [Core SDK Documentation](../core/README.md) - Underlying SDK functionality
- [UI Components](../ui/README.md) - Pre-built UI components
- [Demo Application](../../packages/demo/README.md) - Example implementation

## Hook Categories

### By Functionality
- **Connection**: `useConnection`, `useInitializationStatus`
- **Audio**: `useAudio`, `useVoiceModel`
- **Chat**: `useChat`, `useChatSessionList`, `useChatSessionListOptimized`
- **Conversation**: `useTurnState`, `useOutputMode`
- **Data**: `useAgentCData`, `useUserData`
- **Features**: `useAvatar`, `useToolNotifications`
- **Core**: `useRealtimeClient`

### By Complexity
- **Basic**: `useChat`, `useAudio`, `useConnection`
- **Intermediate**: `useVoiceModel`, `useTurnState`, `useUserData`
- **Advanced**: `useRealtimeClient`, `useAvatar`, `useChatSessionListOptimized`

## Migration Guide

If migrating from direct SDK usage:

1. Replace event listeners with hook state
2. Use `useChat` instead of managing messages manually
3. Replace audio code with `useAudio` hook
4. Use `useConnection` instead of WebSocket monitoring

## Support

For issues or questions:
- Check the [Hooks Overview Guide](./hooks-overview.md)
- Review individual hook documentation
- Consult the [API Implementation Guide](../../api/docs/realtime_api_implementation_guide.md)
- See the [Testing Standards](../../testing_standards_and_architecture.md)
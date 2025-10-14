# React Hooks Overview

The Agent C Realtime React SDK provides a comprehensive set of hooks for building realtime applications. This guide helps you understand which hooks to use for different scenarios and how they work together.

## Quick Reference Table

| Hook | Category | Purpose | Common Use Cases |
|------|----------|---------|------------------|
| [`useRealtimeClient`](./hooks/useRealtimeClient.md) | Core | Direct access to RealtimeClient instance | Advanced customization, direct event handling |
| [`useConnection`](./hooks/useConnection.md) | Connection | Connection state and statistics | Connection status UI, network monitoring |
| [`useInitializationStatus`](./hooks/useInitializationStatus.md) | Connection | Initialization state tracking | Loading states, initialization progress |
| [`useAudio`](./hooks/useAudio.md) | Audio | Audio control and state | Push-to-talk, voice activity, audio settings |
| [`useVoiceModel`](./hooks/useVoiceModel.md) | Audio | Voice model selection | Voice picker UI, voice preferences |
| [`useTurnState`](./hooks/useTurnState.md) | Conversation | Turn management state | Speaking indicators, turn control UI |
| [`useOutputMode`](./hooks/useOutputMode.md) | Conversation | Output mode control | Mode selection (text/audio/avatar) |
| [`useChat`](./hooks/useChat.md) | Chat | Message history and sending | Chat interface, message display |
| [`useChatSessionList`](./hooks/useChatSessionList.md) | Chat | Chat session management | Session history, session switching |
| [`useChatSessionListOptimized`](./hooks/useChatSessionListOptimized.md) | Chat | Optimized session list | Large session lists, performance-critical UIs |
| [`useAgentCData`](./hooks/useAgentCData.md) | Data | Agent C data access | Agent information display |
| [`useUserData`](./hooks/useUserData.md) | Data | User data management | User profile, preferences |
| [`useAvatar`](./hooks/useAvatar.md) | Avatar | HeyGen avatar integration | Avatar video display |
| [`useToolNotifications`](./hooks/useToolNotifications.md) | Tools | Tool call notifications | Tool status UI, progress indicators |

## Hook Categories

### Core Hooks
Essential hooks for accessing the underlying SDK functionality.

#### [`useRealtimeClient`](./hooks/useRealtimeClient.md)
- **Purpose**: Provides direct access to the RealtimeClient instance
- **Use when**: You need low-level control or custom event handling
- **Returns**: The RealtimeClient instance or null

### Connection Management
Hooks for managing and monitoring the WebSocket connection.

#### [`useConnection`](./hooks/useConnection.md)
- **Purpose**: Track connection state and statistics
- **Use when**: Building connection status indicators or monitoring network quality
- **Key features**: Connection state, latency, reconnection info

#### [`useInitializationStatus`](./hooks/useInitializationStatus.md)
- **Purpose**: Monitor SDK initialization progress
- **Use when**: Showing loading states during startup
- **Key features**: Detailed initialization steps and error handling

### Audio & Voice
Hooks for managing audio input/output and voice settings.

#### [`useAudio`](./hooks/useAudio.md)
- **Purpose**: Control audio recording and playback
- **Use when**: Implementing push-to-talk, voice activity detection
- **Key features**: Recording control, volume levels, turn awareness

#### [`useVoiceModel`](./hooks/useVoiceModel.md)
- **Purpose**: Select and manage voice models
- **Use when**: Building voice selection UI
- **Key features**: Available voices, current selection, special modes

### Conversation Flow
Hooks for managing the conversation and turn-taking system.

#### [`useTurnState`](./hooks/useTurnState.md)
- **Purpose**: Track who's speaking and turn management
- **Use when**: Showing speaking indicators, managing interruptions
- **Key features**: Current speaker, turn transitions, interruption control

#### [`useOutputMode`](./hooks/useOutputMode.md)
- **Purpose**: Control output format (text/audio/avatar)
- **Use when**: Implementing mode switching UI
- **Key features**: Current mode, available modes, mode transitions

### Chat Interface
Hooks for building chat functionality.

#### [`useChat`](./hooks/useChat.md)
- **Purpose**: Manage message history and sending
- **Use when**: Building the main chat interface
- **Key features**: Message list, send functionality, typing indicators

#### [`useChatSessionList`](./hooks/useChatSessionList.md)
- **Purpose**: Manage multiple chat sessions
- **Use when**: Building session history or switching between conversations
- **Key features**: Session CRUD operations, active session tracking

#### [`useChatSessionListOptimized`](./hooks/useChatSessionListOptimized.md)
- **Purpose**: Optimized version for large session lists
- **Use when**: Dealing with many sessions or performance-critical UIs
- **Key features**: Pagination, lazy loading, reduced re-renders

### Data Management
Hooks for accessing and managing application data.

#### [`useAgentCData`](./hooks/useAgentCData.md)
- **Purpose**: Access Agent C specific data
- **Use when**: Displaying agent information or capabilities
- **Key features**: Agent metadata, configuration access

#### [`useUserData`](./hooks/useUserData.md)
- **Purpose**: Manage user profile and preferences
- **Use when**: Building user settings or profile displays
- **Key features**: User info, preference management

### Special Features
Hooks for advanced features like avatars and tool notifications.

#### [`useAvatar`](./hooks/useAvatar.md)
- **Purpose**: Integrate HeyGen avatar videos
- **Use when**: Using avatar output mode
- **Key features**: Avatar session management, video streaming

#### [`useToolNotifications`](./hooks/useToolNotifications.md)
- **Purpose**: Track tool call execution
- **Use when**: Showing tool usage or progress indicators
- **Key features**: Tool status updates, execution tracking

## Common Usage Patterns

### Basic Chat Interface
```tsx
function ChatInterface() {
  const { messages, sendMessage } = useChat();
  const { isConnected } = useConnection();
  const { isRecording, startRecording, stopRecording } = useAudio();
  
  // Render chat UI with messages and controls
}
```

### Voice-Enabled Chat
```tsx
function VoiceChat() {
  const { messages } = useChat();
  const { currentVoice, setVoice, availableVoices } = useVoiceModel();
  const { isRecording, startRecording, stopRecording } = useAudio();
  const { currentTurn, isAgentSpeaking } = useTurnState();
  
  // Render voice chat with turn indicators
}
```

### Session Management
```tsx
function SessionManager() {
  const { sessions, activeSession, createSession, switchSession } = useChatSessionList();
  const { messages } = useChat();
  
  // Render session list and active chat
}
```

### Avatar Integration
```tsx
function AvatarChat() {
  const { outputMode, setOutputMode } = useOutputMode();
  const { avatarSessionId, avatarStreamUrl } = useAvatar();
  const { messages } = useChat();
  
  // Render chat with avatar video when in avatar mode
}
```

### Connection Monitoring
```tsx
function ConnectionStatus() {
  const { isConnected, connectionState, latency } = useConnection();
  const { initializationStatus, error } = useInitializationStatus();
  
  // Show connection status and handle errors
}
```

## Best Practices

### 1. Use the Right Level of Abstraction
- Start with high-level hooks like `useChat` and `useAudio`
- Only use `useRealtimeClient` when you need custom functionality

### 2. Handle Loading States
```tsx
const { initializationStatus } = useInitializationStatus();
const { isConnected } = useConnection();

if (initializationStatus !== 'initialized' || !isConnected) {
  return <LoadingScreen />;
}
```

### 3. Combine Related Hooks
```tsx
// Good: Combine related hooks for cohesive features
function VoiceControls() {
  const audio = useAudio();
  const voice = useVoiceModel();
  const turn = useTurnState();
  
  // Use together for voice interaction
}
```

### 4. Performance Optimization
- Use `useChatSessionListOptimized` for large session lists
- Memoize expensive computations from hook data
- Avoid unnecessary re-renders by destructuring only needed values

### 5. Error Handling
```tsx
const { error } = useInitializationStatus();
const { connectionError } = useConnection();

if (error || connectionError) {
  return <ErrorBoundary error={error || connectionError} />;
}
```

## Hook Dependencies

Some hooks work best when used together:

- **Chat + Audio**: Combine for voice-enabled chat
- **Connection + Initialization**: Monitor both for robust connection handling  
- **TurnState + Audio**: Coordinate recording with turn management
- **OutputMode + Avatar**: Switch between text, audio, and avatar modes
- **VoiceModel + OutputMode**: Voice selection affects audio output

## Migration Guide

If you're migrating from direct SDK usage to hooks:

1. Replace `RealtimeClient` event listeners with hook state
2. Use `useChat` instead of managing messages manually
3. Replace audio management code with `useAudio` hook
4. Use `useConnection` instead of monitoring WebSocket directly

## TypeScript Support

All hooks are fully typed with TypeScript. Import types from the hooks package:

```tsx
import type { ConnectionState, Message, TurnState } from '@agentc/realtime-react';
```

## See Also

- [Provider Setup Guide](../providers/AgentCProvider.md)
- [Core SDK Documentation](../../core/README.md)
- [UI Components](../../ui/README.md)
- [Demo Application](../../../packages/demo/README.md)
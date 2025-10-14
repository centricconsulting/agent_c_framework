# Agent C Realtime SDK Documentation

> **Breaking Changes in v2.0.0**: Authentication and initialization have been completely restructured. See the [Migration Guide](./guides/authentication-migration.md) for upgrade instructions.

## Overview

The Agent C Realtime SDK enables voice and text conversations with AI agents through WebSocket connections. Built with TypeScript, it provides a production-ready platform for creating interactive AI experiences.

## Key Features

- 🎙️ **Real-time Voice Conversations** - WebRTC-based audio streaming with automatic resampling
- 💬 **Text Chat** - Streaming text responses with typing indicators
- 🔄 **Binary Protocol** - 33% bandwidth savings with efficient binary audio transport
- 🎯 **Turn Management** - Sophisticated conversation flow control
- 🔌 **Event-Driven Architecture** - Comprehensive event system for all interactions
- 🎭 **Avatar Support** - HeyGen avatar integration for visual agents
- ⚛️ **React Integration** - Complete hooks and components for React apps
- 🔐 **Flexible Authentication** - Support for both development and production patterns

## What's New in v2.0.0

### Simplified Authentication
- Login response reduced by 90% - now only returns essential tokens
- All configuration data delivered via WebSocket events
- Automatic initialization sequence on connection

### New Event System  
- 6 automatic initialization events deliver all configuration
- Rich event types for every interaction
- Built-in initialization tracking

### Enhanced React Hooks
- `useAgentCData()` - Unified access to all configuration
- `useInitializationStatus()` - Track initialization state
- `useUserData()` - Direct user profile access

See the [Migration Guide](./guides/authentication-migration.md) for details.

## Quick Links

### Getting Started
- [Quick Start Guide](./getting-started.md) - Set up your first chat in 5 minutes
- [Authentication Guide](./guides/authentication.md) - Production authentication patterns
- [Migration Guide](./guides/authentication-migration.md) - Upgrade from v1.x

### Core Features
- [Audio Streaming](./guides/audio-streaming.md) - Voice conversation setup
- [Turn Management](./guides/turn-management.md) - Conversation flow control
- [Avatar Integration](./guides/avatar-integration.md) - Visual agent setup
- [Voice Models](./guides/voice-models.md) - Available voice options

### API Reference
- [Core SDK](./api-reference/core/index.md) - RealtimeClient and managers
- [React Hooks](./api-reference/react/index.md) - React integration
- [UI Components](./api-reference/ui-components.md) - Pre-built components

### Architecture
- [System Architecture](./architecture.md) - Technical deep dive
- [Audio Architecture](./AUDIO_QUICK_REFERENCE.md) - Audio system details
- [WebSocket Protocol](./api-reference/core/WebSocketManager.md) - Binary protocol

## Installation

```bash
# Core SDK
npm install @agentc/realtime-core

# React integration
npm install @agentc/realtime-react

# UI Components  
npm install @agentc/realtime-ui
```

## Basic Usage

### Simple Connection

```typescript
import { AuthManager, RealtimeClient } from '@agentc/realtime-core';

// Authenticate
const authManager = new AuthManager();
await authManager.login({ username, password });

// Connect
const client = new RealtimeClient({
  apiUrl: authManager.getWebSocketUrl(),
  authManager
});

await client.connect();

// Data arrives automatically via events
client.on('chat_user_data', (event) => {
  console.log('User:', event.user);
});

client.on('initialization:complete', () => {
  console.log('Ready to chat!');
});

// Send messages
await client.sendText('Hello!');
```

### React Integration

```tsx
import { AgentCProvider, useChat, useInitializationStatus } from '@agentc/realtime-react';

function App() {
  const { isInitialized } = useInitializationStatus();
  const { messages, sendMessage } = useChat();
  
  if (!isInitialized) {
    return <div>Initializing...</div>;
  }
  
  return (
    <div>
      {messages.map(msg => (
        <div key={msg.id}>{msg.content}</div>
      ))}
      <input onKeyPress={(e) => {
        if (e.key === 'Enter') sendMessage(e.target.value);
      }} />
    </div>
  );
}
```

## Package Structure

The SDK is organized as a monorepo with four main packages:

```
@agentc/
├── realtime-core      # Core SDK with WebSocket client
├── realtime-react     # React hooks and providers
├── realtime-ui        # Pre-built UI components
└── demo              # Example application
```

## Authentication Patterns

The SDK supports two authentication modes:

### Development Mode
Direct login with ChatUser credentials (development only):

```typescript
const authManager = new AuthManager();
await authManager.login({ username, password });
```

### Production Mode
Token-based authentication through your backend:

```typescript
// Your backend provides tokens
const tokens = await yourBackend.getAgentCTokens();
await authManager.initializeFromPayload(tokens);
```

See the [Authentication Guide](./guides/authentication.md) for complete patterns.

## Event System

The SDK uses a comprehensive event system:

### Initialization Events (Automatic)
- `chat_user_data` - User profile information
- `voice_list` - Available voice models
- `agent_list` - Available agents
- `avatar_list` - Available avatars  
- `tool_catalog` - Available tools
- `chat_session_changed` - Current chat session
- `initialization:complete` - All data received

### Interaction Events
- `text:delta` - Streaming text chunks
- `text:complete` - Complete message
- `audio:output` - Binary audio frames
- `user_turn_start` - User can speak
- `agent_turn_start` - Agent is speaking

## Audio System

The audio system provides high-performance voice conversations:

- **Format**: PCM16 at 24000Hz
- **Processing**: Off-thread via AudioWorklet
- **Transport**: Binary WebSocket frames
- **Features**: VAD, noise suppression, automatic resampling

See [Audio Streaming Guide](./guides/audio-streaming.md) for details.

## Browser Requirements

- Chrome 90+, Firefox 88+, Safari 14.1+, Edge 90+
- HTTPS required (for microphone access)
- WebRTC support required for audio

## TypeScript Support

Full TypeScript support with comprehensive type definitions:

```typescript
import type {
  RealtimeClient,
  ChatMessage,
  Voice,
  Agent,
  UserProfile,
  ConnectionState,
  AudioState,
  TurnState
} from '@agentc/realtime-core';
```

## Testing

The SDK uses Vitest for testing:

```bash
# Run tests
npm test

# Run with coverage
npm run test:coverage

# Watch mode
npm run test:watch
```

## Examples

### Complete Chat Application
See [packages/demo](../packages/demo) for a full example application.

### Code Snippets
- [Simple chat](./examples/simple-chat.ts)
- [Voice conversation](./examples/voice-chat.ts)
- [Avatar integration](./examples/avatar-chat.ts)
- [Custom UI](./examples/custom-ui.tsx)

## Troubleshooting

Common issues and solutions:

- **WebSocket fails to connect**: Check HTTPS and authentication
- **No audio input**: Verify microphone permissions and HTTPS
- **Data is undefined**: Wait for `initialization:complete` event
- **Audio quality issues**: Check network bandwidth

See [Troubleshooting Guide](./guides/audio-troubleshooting.md) for more.

## Migration from v1.x

Version 2.0.0 includes breaking changes to authentication and initialization:

1. Login response simplified to only tokens
2. Configuration data now delivered via events
3. New hooks for accessing data
4. Automatic initialization tracking

See [Migration Guide](./guides/authentication-migration.md) for step-by-step instructions.

## Contributing

We welcome contributions! Please see our [Contributing Guide](../CONTRIBUTING.md) for details.

## License

[MIT License](../LICENSE)

## Support

For questions and support:
- Review the [documentation](./getting-started.md)
- Check [troubleshooting guides](./guides/audio-troubleshooting.md)
- Contact the Agent C support team
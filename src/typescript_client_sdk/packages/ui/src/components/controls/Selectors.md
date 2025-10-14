# OutputSelector and AgentSelector Components

## Overview

The OutputSelector and AgentSelector components provide user interface controls for managing output modes and AI agent selection in the Agent C Realtime SDK.

## OutputSelector

A hierarchical dropdown menu for selecting between text, voice, and avatar output modes.

### Features

- **Text Only Mode** - Disables audio output, text-only responses
- **Voice Mode** - Select from available TTS voices grouped by vendor
- **Avatar Mode** - Choose HeyGen avatars for visual agent representation

### Usage

```tsx
import { OutputSelector } from '@agentc/realtime-ui';

function ChatControls() {
  return <OutputSelector />;
}
```

### Props

| Prop | Type | Default | Description |
|------|------|---------|-------------|
| `className` | `string` | - | Additional CSS classes |
| `disabled` | `boolean` | `false` | Disable the selector |
| `showIcon` | `boolean` | `true` | Show mode icons |

### Behavior

- **Text Only**: Sends `setAgentVoice('none')` to disable audio
- **Voice Selection**: Sends `setAgentVoice(voiceId)` with selected voice
- **Avatar Selection**: Requires HeyGen SDK integration (placeholder in current implementation)

### Visual Design

- Uses dropdown menu with submenus for hierarchical selection
- Icons indicate current mode (Type, Mic, User)
- Shows vendor grouping for voices
- Displays voice descriptions when available

## AgentSelector

A select dropdown for choosing between available AI agents.

### Features

- Displays agent name and description
- Shows equipped tools as badges
- Automatically formats tool names for readability
- Updates selection based on server events

### Usage

```tsx
import { AgentSelector } from '@agentc/realtime-ui';

function ChatHeader() {
  return <AgentSelector />;
}
```

### Props

| Prop | Type | Default | Description |
|------|------|---------|-------------|
| `className` | `string` | - | Additional CSS classes |
| `disabled` | `boolean` | `false` | Disable the selector |
| `showIcon` | `boolean` | `true` | Show agent icon |
| `placeholder` | `string` | `"Select an agent"` | Placeholder text |

### Behavior

- Sends `setAgent(agentKey)` when selection changes
- Listens for `AgentConfigurationChangedEvent` to update display
- Listens for `ChatSessionChangedEvent` to sync with session state
- Initializes with current agent from session if available

### Tool Name Formatting

Tool class names are automatically formatted for display:
- `ThinkTools` → `Think`
- `WorkspaceTools` → `Workspace`
- `WorkspacePlanningTools` → `Workspace Planning`

## Integration Requirements

Both components require:

1. **AgentCProvider Context** - Must be wrapped in AgentCProvider
2. **WebSocket Connection** - Must be connected to receive initialization data
3. **Initialization Complete** - Wait for all 6 initialization events

### Example Integration

```tsx
import { AgentCProvider } from '@agentc/realtime-react';
import { OutputSelector, AgentSelector } from '@agentc/realtime-ui';

function App() {
  const client = new RealtimeClient(config);
  
  return (
    <AgentCProvider client={client}>
      <div className="flex gap-2 p-4">
        <OutputSelector />
        <AgentSelector />
      </div>
    </AgentCProvider>
  );
}
```

## State Management

Both components are self-contained and manage their own state:

- Automatically fetch data using `useAgentCData()` hook
- Handle loading and error states
- Update UI based on server events
- Disable during state transitions

## Avatar Integration (Future)

The avatar selection currently shows available avatars but requires HeyGen SDK integration:

```tsx
// Future implementation would:
// 1. Initialize HeyGen SDK
const heygenSDK = new StreamingAvatarApi(token);

// 2. Create streaming session
const session = await heygenSDK.createStreamingSession({
  avatarId: selectedAvatarId,
  // ... other config
});

// 3. Wait for STREAM_READY event
session.on(StreamingEvents.STREAM_READY, () => {
  // 4. Notify Agent C
  client.setAvatarSession(session.sessionId, selectedAvatarId);
});
```

## Accessibility

Both components include:
- ARIA labels for screen readers
- Keyboard navigation support
- Focus management
- Disabled states
- Loading indicators

## Styling

Components follow CenSuite design system:
- Consistent spacing and typography
- Semantic color tokens
- Responsive design
- Dark mode support (via CSS variables)
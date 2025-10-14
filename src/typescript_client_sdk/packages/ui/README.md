# @agentc/realtime-ui

UI components for the Agent C Realtime SDK, built with shadcn/ui patterns and CenSuite compatibility.

## Documentation

📚 **[View Full Component Documentation →](./docs/api-reference/ui-components/)**

Comprehensive documentation is available for all components including:
- [Audio Components](./docs/api-reference/ui-components/audio-components.md) - Audio controls and visualizers
- [Chat Components](./docs/api-reference/ui-components/chat-components.md) - Message display and chat interface
- [Connection Components](./docs/api-reference/ui-components/connection-components.md) - Connection management UI
- [Avatar Components](./docs/api-reference/ui-components/avatar-components.md) - HeyGen avatar integration
- [Control Components](./docs/api-reference/ui-components/control-components.md) - Voice and output selectors
- [Other Components](./docs/api-reference/ui-components/other-components.md) - Layout, input, and utility components
- [Component Index](./docs/api-reference/ui-components/index.md) - Complete navigation guide

## Installation

```bash
npm install @agentc/realtime-ui @agentc/realtime-react
```

## Setup

This package requires Tailwind CSS to be configured in your application with the appropriate CSS variables for theming.

### 1. Add CSS Variables

Add the following CSS variables to your global CSS file:

```css
@layer base {
  :root {
    --background: 0 0% 100%;
    --foreground: 222.2 84% 4.9%;
    --card: 0 0% 100%;
    --card-foreground: 222.2 84% 4.9%;
    --popover: 0 0% 100%;
    --popover-foreground: 222.2 84% 4.9%;
    --primary: 222.2 47.4% 11.2%;
    --primary-foreground: 210 40% 98%;
    --secondary: 210 40% 96.1%;
    --secondary-foreground: 222.2 47.4% 11.2%;
    --muted: 210 40% 96.1%;
    --muted-foreground: 215.4 16.3% 46.9%;
    --accent: 210 40% 96.1%;
    --accent-foreground: 222.2 47.4% 11.2%;
    --destructive: 0 84.2% 60.2%;
    --destructive-foreground: 210 40% 98%;
    --border: 214.3 31.8% 91.4%;
    --input: 214.3 31.8% 91.4%;
    --ring: 222.2 84% 4.9%;
    --radius: 0.5rem;
  }

  .dark {
    --background: 222.2 84% 4.9%;
    --foreground: 210 40% 98%;
    --card: 222.2 84% 4.9%;
    --card-foreground: 210 40% 98%;
    --popover: 222.2 84% 4.9%;
    --popover-foreground: 210 40% 98%;
    --primary: 210 40% 98%;
    --primary-foreground: 222.2 47.4% 11.2%;
    --secondary: 217.2 32.6% 17.5%;
    --secondary-foreground: 210 40% 98%;
    --muted: 217.2 32.6% 17.5%;
    --muted-foreground: 215 20.2% 65.1%;
    --accent: 217.2 32.6% 17.5%;
    --accent-foreground: 210 40% 98%;
    --destructive: 0 62.8% 30.6%;
    --destructive-foreground: 210 40% 98%;
    --border: 217.2 32.6% 17.5%;
    --input: 217.2 32.6% 17.5%;
    --ring: 212.7 26.8% 83.9%;
  }
}
```

### 2. Configure Tailwind

Ensure your Tailwind config includes the UI package in its content paths:

```js
module.exports = {
  content: [
    // ... your other content paths
    "./node_modules/@agentc/realtime-ui/**/*.{js,ts,jsx,tsx}",
  ],
  // ... rest of your config
}
```

## Quick Start

```tsx
import { ConnectionButton, MicrophoneButton, OutputSelector, ChatMessage } from '@agentc/realtime-ui';
import { AgentCProvider } from '@agentc/realtime-react';

function App() {
  return (
    <AgentCProvider>
      <div className="flex flex-col gap-4 p-4">
        {/* Connection Controls */}
        <div className="flex items-center gap-2">
          <ConnectionButton />
          <MicrophoneButton />
          <OutputSelector />
        </div>
        
        {/* Chat Display */}
        <div className="flex flex-col gap-2">
          <ChatMessage
            message={{
              role: 'assistant',
              content: 'Hello! How can I help you today?',
              timestamp: new Date().toISOString()
            }}
          />
        </div>
      </div>
    </AgentCProvider>
  );
}
```

## Component Overview

### Core Components

- **Connection Components** - Manage WebSocket connection state
  - `ConnectionButton` - Connect/disconnect with status indicator
  - `ConnectionStatus` - Display connection state

- **Audio Components** - Handle audio input/output
  - `MicrophoneButton` - Toggle audio input with visual feedback
  - `AudioLevelIndicator` - Real-time audio level display
  - `VolumeControl` - Output volume adjustment

- **Chat Components** - Display conversation interface
  - `ChatMessage` - Message display with markdown support
  - `ChatMessageList` - Scrollable message container
  - `MessageInput` - Text input with send button

- **Control Components** - Configure agent behavior
  - `OutputSelector` - Choose text/voice/avatar output modes
  - `AgentSelector` - Switch between available agents
  - `VoiceSelector` - Select TTS voice

- **Avatar Components** - HeyGen avatar integration
  - `AvatarDisplay` - Render streaming avatar
  - `AvatarControls` - Avatar configuration

- **Session Components** - Manage chat sessions
  - `SessionList` - Display saved sessions
  - `SessionManager` - Create/delete sessions

### Utility Components

- `TurnIndicator` - Show conversation turn state
- `LoadingIndicator` - Consistent loading states
- `ErrorBoundary` - Graceful error handling
- `ThemeToggle` - Light/dark mode switcher

## Features

✅ **Full TypeScript Support** - Complete type definitions and IntelliSense  
✅ **Accessibility First** - WCAG 2.1 AA compliant with full keyboard navigation  
✅ **Responsive Design** - Mobile-optimized with touch-friendly controls  
✅ **Dark Mode Support** - Automatic theme detection and manual toggle  
✅ **Error Handling** - Built-in error boundaries and recovery  
✅ **Performance Optimized** - Memoization and lazy loading where appropriate  
✅ **Customizable** - Override styles with className prop on all components  

## Testing

```bash
# Run all UI component tests
pnpm test

# Run tests with coverage
pnpm test:coverage

# Run tests in watch mode
pnpm test:watch

# Open test UI
pnpm test:ui
```

## Design System

This package follows:
- **CenSuite Design Principles** - Clarity, Consistency, Efficiency, Scalability, Accessibility
- **shadcn/ui Patterns** - Component composition and styling approach
- **Tailwind CSS** - Utility-first CSS framework

## Development

### Building

```bash
# Build the package
pnpm build

# Build in watch mode
pnpm build:watch
```

### Type Checking

```bash
# Run type checks
pnpm type-check
```

### Linting

```bash
# Run linter
pnpm lint

# Fix linting issues
pnpm lint:fix
```

## License

MIT

## Support

For detailed documentation, examples, and API references, please see the [full documentation](./docs/api-reference/ui-components/).

For issues or questions, please contact the Agent C Realtime team.
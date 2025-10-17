# Agent C Realtime Client SDK Demo App

A reference implementation demonstrating the Agent C Realtime Client SDK capabilities. This Next.js 14 application showcases real-time chat, audio communication, and agent interactions using our TypeScript SDK.

## Documentation

For comprehensive documentation, please refer to:

- **[Complete Demo Documentation](/docs/api-reference/demo/)** - Full setup guide, architecture, features, and deployment
- **[Quick Start Guide](/docs/api-reference/demo/index.md)** - Get up and running quickly
- **[Implementation Patterns](/docs/api-reference/demo/implementation-patterns.md)** - Authentication, providers, chat, and audio patterns
- **[Configuration Guide](/docs/api-reference/demo/configuration-guide.md)** - Environment variables, API endpoints, theming, and build configuration

## Quick Overview

This demo application provides:
- **Real-time Chat Interface** - Full-featured chat with message history and typing indicators
- **Voice Communication** - WebRTC-based audio with push-to-talk and voice activity detection
- **Agent Integration** - Seamless interaction with AI agents via the Realtime API
- **Avatar Support** - HeyGen avatar integration for visual agent representation
- **Responsive Design** - Built with CenSuite design system for optimal user experience

## System Requirements

- Node.js 20.18.0 LTS or higher
- pnpm package manager
- HTTPS enabled (required for microphone access)

## Quick Start

1. **Clone and Install**
   ```bash
   pnpm install
   ```

2. **Configure Environment**
   ```bash
   cp .env.example .env.local
   # Edit .env.local with your API credentials
   ```

3. **Start Development Server**
   ```bash
   pnpm dev
   ```

4. **Build for Production**
   ```bash
   pnpm build
   ```

## Key Features

### Real-time Communication
- WebSocket-based bidirectional communication
- Binary audio streaming with automatic resampling
- Turn-based conversation management
- Automatic reconnection with exponential backoff

### Audio System
- AudioWorklet for high-performance processing
- Automatic sample rate conversion to 16kHz
- Voice activity detection
- Push-to-talk support

### UI Components
- Built with shadcn/ui and CenSuite design system
- Fully accessible (WCAG 2.1 AA compliant)
- Responsive layouts for all screen sizes
- Dark/light theme support

## Project Structure

```
packages/demo/
├── src/
│   ├── app/           # Next.js app router pages
│   ├── components/    # React components
│   │   ├── chat/     # Chat interface components
│   │   └── ui/       # Base UI components
│   └── lib/          # Utilities and helpers
├── public/
│   └── worklets/     # Audio worklet files
└── docs/             # Comprehensive documentation
```

## Environment Variables

Essential configuration (see [Configuration Guide](/docs/api-reference/demo/configuration-guide.md) for full details):

```env
NEXT_PUBLIC_AGENT_ID=your-agent-id
NEXT_PUBLIC_API_URL=https://api.example.com
```

## Development

### Available Scripts

- `pnpm dev` - Start development server
- `pnpm build` - Build for production
- `pnpm start` - Start production server
- `pnpm lint` - Run ESLint
- `pnpm type-check` - Run TypeScript type checking

### Testing the SDK

The demo app serves as a comprehensive test environment for:
- Connection management and error handling
- Audio input/output processing
- Message streaming and chat history
- Voice model selection
- Avatar integration

## Troubleshooting

For common issues and solutions, see:
- [Audio Troubleshooting Guide](../../docs/guides/audio-troubleshooting.md)
- [Demo App Documentation](/docs/api-reference/demo/README.md#troubleshooting)

## Architecture

This demo app is part of the Agent C Realtime Client SDK monorepo:

```
@agentc/realtime-core    # Core SDK functionality
@agentc/realtime-react   # React hooks and providers
@agentc/realtime-ui      # UI components library
@agentc/demo-app         # This demo application
```

## Resources

### SDK Documentation
- [Core SDK API Reference](/docs/api-reference/core/)
- [React Integration Guide](/docs/api-reference/react/)
- [UI Components Library](/docs/api-reference/ui/)

### Design System
- [CenSuite Documentation](https://censuite-ui.vercel.app/)
- [Theming Guide](https://censuite-ui.vercel.app/docs/design/foundation/customization-and-theming)

### Technologies
- [Next.js 14 Documentation](https://nextjs.org/docs)
- [TypeScript](https://www.typescriptlang.org/)
- [Tailwind CSS](https://tailwindcss.com/)
- [shadcn/ui](https://ui.shadcn.com/)

## Support

For questions about the Agent C Realtime Client SDK, please refer to the [comprehensive documentation](/docs/api-reference/demo/) or contact the development team.

## License

Proprietary - See LICENSE file for details
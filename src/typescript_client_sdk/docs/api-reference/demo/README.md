# Agent C Realtime Demo Application

## Overview

The Agent C Realtime Demo Application is a comprehensive reference implementation showcasing the full capabilities of the Agent C Realtime Client SDK. Built with Next.js 14's App Router architecture, this production-ready demonstration app provides developers with a working example of how to integrate real-time agent communication, voice interaction, and avatar capabilities into modern web applications.

This demo serves as both a testing ground for SDK features and a blueprint for building your own Agent C-powered applications. It demonstrates best practices for authentication, real-time WebSocket communication, audio streaming, and UI integration using the CenSuite design system.

## Prerequisites and Requirements

### System Requirements
- **Node.js**: Version 20.18.0 LTS or higher
- **Package Manager**: pnpm (recommended), npm, or yarn
- **Browser**: Modern browser with WebRTC support (Chrome, Firefox, Safari, Edge)
- **HTTPS**: Required for microphone access and secure WebSocket connections

### Development Environment
- **Operating System**: Windows, macOS, or Linux
- **Memory**: Minimum 4GB RAM (8GB recommended)
- **Network**: Stable internet connection for API communication

## Installation and Setup Guide

### 1. Clone and Install Dependencies

```bash
# Navigate to the demo app directory
cd realtime_client/packages/demo

# Install dependencies (automatically deploys audio worklet)
pnpm install

# Verify audio worklet deployment
ls public/worklets/audio-processor.worklet.js
```

### 2. Environment Configuration

Copy the example environment file and configure your settings:

```bash
cp .env.example .env.local
```

Edit `.env.local` with your configuration:

```env
# Local development environment variables
NEXT_PUBLIC_APP_URL=https://localhost:3000

# Server-side only - Agent C API endpoint
AGENT_C_API_URL=https://localhost:8000

# WebSocket endpoint for realtime client
NEXT_PUBLIC_AGENTC_API_URL=wss://localhost:8000
```

### 3. HTTPS Setup

The demo app requires HTTPS for secure communication and microphone access. See [HTTPS_SETUP.md](./HTTPS_SETUP.md) for detailed configuration.

Quick setup:
```bash
# Start the HTTPS development server
pnpm dev

# Or run with HTTP (limited functionality)
pnpm dev:http
```

## Architecture Overview

The demo app is built using Next.js 14's App Router architecture, providing a modern, performant foundation with server-side rendering capabilities.

### Technology Stack
- **Framework**: Next.js 14 with App Router
- **Language**: TypeScript for type safety
- **Styling**: Tailwind CSS with CenSuite design tokens
- **UI Components**: @agentc/realtime-ui (shadcn/ui-based)
- **State Management**: React Context + SDK hooks
- **Real-time Communication**: WebSocket via @agentc/realtime-core
- **Authentication**: JWT tokens with secure cookie storage

### Application Structure
```
src/
├── app/                    # Next.js App Router pages
│   ├── api/               # API routes for authentication
│   ├── chat/              # Main chat interface
│   └── login/             # Authentication page
├── components/            # React components
│   ├── chat/             # Chat-specific components
│   ├── providers/        # Context providers
│   └── layout/           # Layout components
├── lib/                   # Utility libraries
│   └── auth.ts           # Authentication service
├── config/                # Application configuration
├── hooks/                 # Custom React hooks
└── styles/               # Global styles and CSS
```

## Key Features and Functionality

### 1. Real-time Chat Interface
- **Text and Voice Communication**: Seamless switching between text and voice input
- **Message History**: Persistent conversation history with auto-scrolling
- **Turn Management**: Server-controlled turn-taking for natural conversation flow
- **Typing Indicators**: Real-time feedback during agent responses

### 2. Audio System
- **WebRTC Audio Processing**: High-performance audio capture and playback
- **AudioWorklet Integration**: Off-thread audio processing for optimal performance
- **Automatic Resampling**: Converts browser native rate to 16kHz
- **Visual Feedback**: Audio level indicators and recording status

### 3. Avatar Integration
- **HeyGen Avatar Support**: Optional avatar display for agent representation
- **Synchronized Lip-sync**: Avatar animations synchronized with agent speech
- **Multiple Avatar Options**: Choose from available avatar configurations

### 4. Voice Selection
- **Multiple Voice Models**: Support for various TTS voice options
- **Dynamic Voice Switching**: Change voices during conversation
- **Special Modes**: Support for "none" (text-only) and avatar-specific voices

### 5. Authentication System
- **JWT Token Management**: Secure authentication with automatic token refresh
- **Protected Routes**: Automatic redirection for unauthenticated users
- **Session Persistence**: Maintains authentication across page refreshes

### 6. Developer Tools
- **Debug Console**: Built-in logging for development troubleshooting
- **Connection Statistics**: Real-time monitoring of WebSocket connection
- **Error Boundaries**: Graceful error handling with recovery options

## Directory Structure Explanation

### `/src/app` - Next.js App Router
The app directory contains all routes and API endpoints:
- `layout.tsx` - Root layout with theme provider
- `page.tsx` - Landing page (redirects to login or chat)
- `/api/auth/` - Authentication endpoints
- `/chat/` - Main chat interface
- `/login/` - Authentication page

### `/src/components` - UI Components
Organized by feature and purpose:
- `/chat/` - Chat-specific components (ChatPageClient, MessageList, InputBar)
- `/providers/` - React context providers for global state
- `/layout/` - Shared layout components (Header, Footer, Navigation)
- `/content/` - Content display components
- `/input/` - Form and input components

### `/src/lib` - Core Libraries
Utility functions and services:
- `auth.ts` - Authentication service with JWT management
- `utils.ts` - Shared utility functions
- `cn.ts` - Class name utilities for Tailwind

### `/src/config` - Configuration
Application configuration and constants:
- Environment-specific settings
- API endpoints
- Feature flags

### `/public` - Static Assets
- `/worklets/` - Audio worklet files for audio processing
- `favicon.ico` - Application icon

## Authentication Flow

### Login Process
1. **User Navigation**: User accesses `/login` page
2. **Credential Entry**: Username/email and password submission
3. **API Authentication**: POST request to `/api/auth/login`
4. **Token Storage**: JWT stored in secure cookies
5. **Response Handling**: Receives user data, agents, avatars, and voices
6. **Redirect**: Successful login redirects to `/chat`

### Session Management
- **Token Validation**: Automatic validation on each request
- **Expiration Handling**: Token expiration checked with 30-second buffer
- **Refresh Logic**: Automatic token refresh before expiration
- **Logout**: Clears all authentication tokens and redirects to login

### JWT Token Structure
```typescript
{
  sub: string,      // User ID
  exp: number,      // Expiration timestamp
  iat: number,      // Issued at timestamp
  // Additional claims...
}
```

## Main Routes

### `/login` - Authentication Page
- Clean, accessible login form
- Form validation with Zod schemas
- Error messaging for failed attempts
- Automatic redirect for authenticated users

### `/chat` - Main Chat Interface
- Protected route requiring authentication
- Real-time WebSocket connection to Agent C API
- Full chat interface with all SDK features
- Dynamic component loading to avoid SSR issues

### `/api/auth/login` - Login Endpoint
- Proxies authentication to Agent C backend
- Handles token management
- Returns user profile and available resources

### `/api/auth/session` - Session Endpoint
- Validates current authentication
- Returns WebSocket connection details
- Provides user context for client

## Configuration Files

### `next.config.js`
- Transpiles @agentc/realtime-ui package
- Configures webpack aliases for package resolution
- Sets up source maps for development
- Handles module resolution for monorepo structure

### `tailwind.config.js`
- Extends with CenSuite design tokens
- Includes UI package components in content paths
- Configures custom animations
- Sets up color palette and spacing scales

### `tsconfig.json`
- TypeScript compiler configuration
- Path aliases for clean imports (@/*)
- Strict type checking enabled
- Excludes test files from build

### `components.json`
- shadcn/ui configuration
- Component import aliases
- Style preferences (CSS variables, Tailwind)
- Icon library configuration (Lucide)

### `vitest.config.ts`
- Test runner configuration
- Coverage settings
- Test environment setup
- Mock configurations

## Development Workflow

### Starting Development Server

```bash
# Start with HTTPS (recommended)
pnpm dev

# Start with HTTP (limited functionality)
pnpm dev:http
```

### Running Tests

```bash
# Run all tests
pnpm test

# Watch mode for development
pnpm test:watch

# Generate coverage report
pnpm test:coverage

# Interactive UI for test debugging
pnpm test:ui
```

### Code Quality

```bash
# Run ESLint
pnpm lint

# Format code with Prettier
pnpm format

# Check formatting without changes
pnpm format:check
```

### Building for Production

```bash
# Create production build
pnpm build

# Start production server
pnpm start
```

## Production Build and Deployment

### Build Process

1. **Environment Setup**
   ```bash
   # Set production environment variables
   export NEXT_PUBLIC_APP_URL=https://your-domain.com
   export NEXT_PUBLIC_AGENTC_API_URL=wss://api.your-domain.com
   export AGENT_C_API_URL=https://api.your-domain.com
   ```

2. **Build Application**
   ```bash
   # Clean previous builds
   pnpm clean

   # Create optimized production build
   pnpm build
   ```

3. **Verify Build**
   ```bash
   # Test production build locally
   pnpm start
   ```

### Deployment Options

#### Vercel (Recommended)
- Automatic deployment from Git
- Built-in HTTPS and CDN
- Environment variable management
- Zero-configuration deployment

#### AWS/Azure/GCP
- Use Next.js standalone output
- Configure reverse proxy (nginx/Apache)
- Set up SSL certificates
- Configure environment variables

#### Docker Deployment
```dockerfile
FROM node:20-alpine
WORKDIR /app
COPY package*.json ./
RUN npm ci --only=production
COPY . .
RUN npm run build
EXPOSE 3000
CMD ["npm", "start"]
```

### Production Checklist
- [ ] Configure production API endpoints
- [ ] Set up SSL certificates
- [ ] Configure CORS settings
- [ ] Set up monitoring and logging
- [ ] Configure rate limiting
- [ ] Set up backup and recovery
- [ ] Configure CDN for static assets
- [ ] Set up health checks

## Troubleshooting Common Issues

### Audio Issues

#### No Microphone Access
- **Cause**: Browser blocking microphone or no HTTPS
- **Solution**: Ensure HTTPS is enabled and grant microphone permissions

#### Audio Worklet 404 Error
- **Cause**: Missing audio worklet file
- **Solution**: 
  ```bash
  cp ../../packages/core/dist/worklets/audio-processor.worklet.js public/worklets/
  ```

#### Distorted Audio
- **Cause**: Sample rate mismatch
- **Solution**: The system automatically handles resampling; check browser compatibility

### Connection Issues

#### WebSocket Connection Failed
- **Cause**: API server not running or incorrect URL
- **Solution**: Verify API is running and check NEXT_PUBLIC_AGENTC_API_URL

#### Authentication Failures
- **Cause**: Expired token or incorrect credentials
- **Solution**: Clear cookies and re-authenticate

#### CORS Errors
- **Cause**: API server CORS configuration
- **Solution**: Ensure API allows origin https://localhost:3000

### Build Issues

#### Module Resolution Errors
- **Cause**: Monorepo package linking issues
- **Solution**: 
  ```bash
  pnpm install
  pnpm build --filter=@agentc/realtime-core --filter=@agentc/realtime-react
  ```

#### TypeScript Errors
- **Cause**: Type definition mismatches
- **Solution**: Ensure all packages are built and types are generated

### Performance Issues

#### Slow Initial Load
- **Cause**: Large bundle size
- **Solution**: Check bundle analysis with `next build --analyze`

#### Memory Leaks
- **Cause**: Unclean component unmounting
- **Solution**: Ensure proper cleanup in useEffect hooks

## Additional Resources

### SDK Documentation
- [Core SDK API Reference](../core/README.md)
- [React Hooks Documentation](../react/README.md)
- [UI Components Guide](../ui/README.md)
- [Audio System Guide](../../guides/audio-system.md)

### Design System
- [CenSuite Documentation](https://censuite-ui.vercel.app/)
- [Component Patterns](../../design/component-patterns.md)
- [Accessibility Guidelines](../../design/accessibility.md)

### Development Tools
- [Next.js Documentation](https://nextjs.org/docs)
- [Tailwind CSS](https://tailwindcss.com/docs)
- [TypeScript Handbook](https://www.typescriptlang.org/docs/)
- [React Documentation](https://react.dev)

## Support and Contribution

### Getting Help
- Check the [troubleshooting guide](#troubleshooting-common-issues)
- Review [SDK documentation](../README.md)
- Contact the Agent C team

### Contributing
- Follow the established code patterns
- Write tests for new features
- Update documentation
- Submit pull requests for review

### License
This demo application is part of the Agent C Realtime Client SDK and follows the same licensing terms.

---

Built with ❤️ by the Agent C Realtime Team
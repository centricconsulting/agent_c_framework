# Agent C Realtime Demo Application Documentation Index

> Complete documentation index for the Agent C Realtime demo application - your reference implementation for building with the Agent C SDK

## 🚀 Quick Start Guide

### Prerequisites Check
```bash
# Check Node.js version (requires 20.18.0+)
node --version

# Verify pnpm is installed
pnpm --version || npm install -g pnpm
```

### 1-Minute Setup
```bash
# Navigate to demo app
cd realtime_client/packages/demo

# Install dependencies
pnpm install

# Copy environment template
cp .env.example .env.local

# Edit .env.local with your settings
# NEXT_PUBLIC_APP_URL=https://localhost:3000
# NEXT_PUBLIC_AGENTC_API_URL=wss://localhost:8000
# AGENT_C_API_URL=https://localhost:8000

# Start development server with HTTPS
pnpm dev

# Open https://localhost:3000
```

### First Run Checklist
- ✅ HTTPS certificate accepted in browser
- ✅ Microphone permissions granted
- ✅ Agent C API server running
- ✅ Valid authentication credentials ready

## 📚 Documentation Overview

### Core Documentation

#### 📖 [**Main README**](./README.md)
Complete guide covering:
- Overview and architecture
- Installation and setup
- Technology stack details
- Feature descriptions
- Troubleshooting guide
- Production deployment

#### ⚙️ [**Configuration Guide**](./configuration-guide.md)
Comprehensive configuration documentation:
- Environment variables setup
- API endpoint configuration
- HTTPS/SSL configuration
- Theming and styling customization
- Feature toggles and flags
- Build optimization settings
- Testing configuration
- Deployment configurations

#### 🏗️ [**Implementation Patterns**](./implementation-patterns.md)
Best practices and patterns:
- Authentication implementation
- Real-time connection management
- Audio system integration
- State management patterns
- Error handling strategies
- Performance optimizations
- Testing approaches
- Security considerations

## 🗂️ Application Structure

### Directory Layout
```
packages/demo/
├── src/
│   ├── app/                    # Next.js App Router
│   │   ├── api/                # API routes
│   │   │   └── auth/          # Authentication endpoints
│   │   ├── chat/              # Main chat interface
│   │   ├── login/             # Authentication page
│   │   └── layout.tsx         # Root layout
│   ├── components/            # React components
│   │   ├── chat/             # Chat-specific components
│   │   ├── providers/        # Context providers
│   │   └── layout/           # Layout components
│   ├── lib/                   # Utilities
│   │   └── auth.ts           # Authentication service
│   ├── config/                # Configuration
│   ├── hooks/                 # Custom React hooks
│   └── styles/               # Global styles
├── public/
│   └── worklets/             # Audio worklet files
├── .env.example              # Environment template
├── next.config.js            # Next.js configuration
├── tailwind.config.js        # Tailwind configuration
├── tsconfig.json             # TypeScript configuration
└── package.json              # Package dependencies
```

## 🎯 Key Features Reference

### Real-time Communication
- **WebSocket Connection** - Persistent connection to Agent C API
- **Message Streaming** - Real-time text delta accumulation
- **Binary Audio** - PCM audio streaming over WebSocket
- **Turn Management** - Server-controlled conversation flow
- **Reconnection** - Automatic reconnection with exponential backoff

### Audio System
- **AudioWorklet Processing** - Off-thread audio processing
- **Automatic Resampling** - Browser rate to 16kHz conversion
- **Visual Feedback** - Audio level indicators
- **Voice Activity Detection** - Automatic speech detection
- **Echo Cancellation** - Built-in browser echo cancellation

### User Interface
- **Responsive Design** - Mobile-first approach
- **Dark Mode** - System preference detection
- **Accessibility** - WCAG 2.1 AA compliant
- **Loading States** - Skeleton screens and spinners
- **Error Boundaries** - Graceful error recovery

### Authentication
- **JWT Tokens** - Secure token management
- **Auto-refresh** - Token renewal before expiry
- **Protected Routes** - Automatic redirection
- **Session Persistence** - Maintains auth across refreshes

## 🔧 Configuration Quick Reference

### Environment Variables
```env
# Required - Public Variables
NEXT_PUBLIC_APP_URL=https://localhost:3000
NEXT_PUBLIC_AGENTC_API_URL=wss://localhost:8000

# Required - Server-side Only
AGENT_C_API_URL=https://localhost:8000

# Optional - Feature Flags
NEXT_PUBLIC_ENABLE_AVATAR=true
NEXT_PUBLIC_ENABLE_DEBUG=false
NEXT_PUBLIC_MAX_RECONNECT_ATTEMPTS=5
```

### API Endpoints
```typescript
// Authentication
POST   /api/auth/login     // User login
GET    /api/auth/session   // Session validation
POST   /api/auth/logout    // User logout

// Health Check
GET    /api/health         // Application health
```

### WebSocket Events
```typescript
// Client -> Server
{ type: 'session:start' }
{ type: 'input:text', content: string }
{ type: 'voice:start' }
{ type: 'voice:stop' }
Binary: PCM Audio Data

// Server -> Client
{ type: 'session:started' }
{ type: 'response:text:delta', content: string }
{ type: 'response:audio:delta', content: string }
{ type: 'turn:start' }
{ type: 'turn:end' }
Binary: Audio Output Data
```

## 💻 Development Commands

### Essential Commands
```bash
# Development
pnpm dev              # Start HTTPS dev server
pnpm dev:http         # Start HTTP dev server (limited features)

# Testing
pnpm test            # Run all tests
pnpm test:watch      # Watch mode
pnpm test:coverage   # Coverage report
pnpm test:ui         # Interactive UI

# Code Quality
pnpm lint            # Run ESLint
pnpm lint:fix        # Auto-fix issues
pnpm format          # Format with Prettier
pnpm typecheck       # TypeScript checking

# Production
pnpm build           # Production build
pnpm start           # Start production server
pnpm clean           # Clean build artifacts
```

## 🐛 Common Issues & Solutions

### Quick Fixes

#### Audio Not Working
```bash
# Ensure audio worklet is deployed
cp ../../packages/core/dist/worklets/audio-processor.worklet.js public/worklets/

# Check HTTPS is enabled
pnpm dev  # Not dev:http
```

#### Connection Failed
```javascript
// Verify WebSocket URL uses wss:// not ws://
NEXT_PUBLIC_AGENTC_API_URL=wss://localhost:8000  // ✅
NEXT_PUBLIC_AGENTC_API_URL=ws://localhost:8000   // ❌
```

#### Authentication Loop
```bash
# Clear browser cookies
# In DevTools: Application > Storage > Clear site data

# Restart dev server
pnpm dev
```

#### Build Errors
```bash
# Clean and rebuild
pnpm clean
pnpm install
pnpm build
```

## 🚢 Deployment Checklist

### Pre-deployment
- [ ] Environment variables configured
- [ ] SSL certificates obtained
- [ ] API endpoints verified
- [ ] Authentication tested
- [ ] Audio worklet deployed
- [ ] CORS settings configured

### Build & Deploy
- [ ] Production build successful
- [ ] Bundle size optimized
- [ ] Source maps configured
- [ ] Error tracking setup
- [ ] Health checks passing
- [ ] Monitoring configured

### Post-deployment
- [ ] WebSocket connection verified
- [ ] Audio streaming tested
- [ ] Authentication flow working
- [ ] Error boundaries tested
- [ ] Performance metrics acceptable
- [ ] Backup strategy in place

## 📊 Implementation Examples

### Basic Chat Interface
```tsx
// app/chat/page.tsx
import { ChatPageClient } from '@/components/chat/ChatPageClient';

export default function ChatPage() {
  return <ChatPageClient />;
}
```

### Custom Authentication
```typescript
// lib/auth.ts
export async function authenticateUser(credentials: LoginCredentials) {
  const response = await fetch('/api/auth/login', {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(credentials),
  });
  
  if (!response.ok) {
    throw new Error('Authentication failed');
  }
  
  return response.json();
}
```

### WebSocket Integration
```typescript
// Using SDK hooks
import { useConnection, useChat } from '@agentc/realtime-react';

function ChatInterface() {
  const { connect, disconnect, isConnected } = useConnection();
  const { messages, sendMessage } = useChat();
  
  return (
    <div>
      {isConnected ? (
        <MessageList messages={messages} />
      ) : (
        <button onClick={connect}>Connect</button>
      )}
    </div>
  );
}
```

## 🧪 Testing Guide

### Test Structure
```
src/
  components/
    chat/
      __tests__/
        ChatPageClient.test.tsx      # Component tests
        ChatPageClient.integration.test.tsx  # Integration tests
      __mocks__/
        mockMessages.ts              # Test fixtures
```

### Running Tests
```bash
# Unit tests only
pnpm test:unit

# Integration tests
pnpm test:integration

# Specific file
pnpm test ChatPageClient

# With debugging
pnpm test:debug
```

### Writing Tests
```typescript
import { render, screen } from '@testing-library/react';
import { ChatPageClient } from '../ChatPageClient';

describe('ChatPageClient', () => {
  it('renders connection button', () => {
    render(<ChatPageClient />);
    expect(screen.getByRole('button', { name: /connect/i })).toBeInTheDocument();
  });
});
```

## 📚 Related Documentation

### SDK Documentation
- [Core SDK Reference](../core/README.md)
- [React Hooks Reference](../react/README.md)
- [UI Components Reference](../ui-components/index.md)

### API Documentation
- [Realtime API Implementation Guide](../../../api/docs/realtime_api_implementation_guide.md)
- [WebSocket Protocol](../../../api/docs/websocket_protocol.md)

### Design & Architecture
- [Testing Standards](../../testing_standards_and_architecture.md)
- [CenSuite Design System](../../ref/CenSuite_Starter)
- [Audio System Architecture](../../design/audio-system.md)

## 🆘 Getting Help

### Resources
1. **Documentation** - Start with this index
2. **README** - Detailed setup and troubleshooting
3. **Configuration Guide** - Environment and settings
4. **Implementation Patterns** - Best practices

### Support Process
1. Check troubleshooting sections
2. Review error messages in browser console
3. Verify environment configuration
4. Consult team documentation
5. Contact Agent C team if needed

### Debug Tools
- Browser DevTools Network tab for WebSocket
- React Developer Tools for component state
- Application tab for cookie/storage inspection
- Console for error messages and logging

## 🎓 Learning Path

### For New Developers
1. **Start Here** → Quick Start Guide (above)
2. **Understand** → [Main README](./README.md) overview
3. **Configure** → [Configuration Guide](./configuration-guide.md)
4. **Implement** → [Implementation Patterns](./implementation-patterns.md)
5. **Customize** → Modify components in `/src/components`
6. **Test** → Write tests following patterns
7. **Deploy** → Follow deployment checklist

### For Experienced Developers
- Jump to [Implementation Patterns](./implementation-patterns.md)
- Review [WebSocket event reference](#websocket-events)
- Check [API endpoints](#api-endpoints)
- Explore source code in `/src`

## 📝 Version Information

- **Demo App Version**: Check package.json
- **SDK Compatibility**: @agentc/realtime-* workspace:*
- **Node.js Required**: 20.18.0 or higher
- **Next.js Version**: 14.x with App Router

---

*Last updated: Documentation current as of latest version*
*Need help? Check the troubleshooting guide or contact the Agent C Realtime Team*
*Built with ❤️ by the Agent C Realtime Team*
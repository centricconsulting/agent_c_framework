# Agent C Realtime Demo Application - Implementation Patterns

This guide documents the key implementation patterns and best practices demonstrated in the Agent C Realtime demo application. These patterns showcase real-world usage of the SDK and can serve as templates for your own implementations.

## Table of Contents

1. [Authentication Pattern](#authentication-pattern)
2. [Provider Architecture](#provider-architecture)
3. [Chat Implementation Pattern](#chat-implementation-pattern)
4. [Audio System Integration](#audio-system-integration)
5. [State Management Patterns](#state-management-patterns)
6. [Error Handling Patterns](#error-handling-patterns)
7. [Performance Optimizations](#performance-optimizations)
8. [Responsive Design Patterns](#responsive-design-patterns)
9. [Testing Patterns](#testing-patterns)

## Authentication Pattern

The demo implements a robust JWT-based authentication flow that ensures secure access to the realtime API.

### JWT Token Flow

The authentication system follows this sequence:

```typescript
// 1. User submits credentials via login form
// 2. Credentials are sent to Next.js API route (proxy)
// 3. API route forwards to Agent C backend
// 4. JWT tokens are received and stored
// 5. WebSocket connection uses JWT for authentication
```

#### Client-Side Authentication Context

```typescript
// src/contexts/auth-context.tsx
export function AuthProvider({ children }: AuthProviderProps) {
  const [isAuthenticated, setIsAuthenticated] = useState(false);
  const [isLoading, setIsLoading] = useState(true);
  const [uiSessionId, setUiSessionId] = useState<string | undefined>();

  // Login function that stores tokens and session ID
  const login = useCallback(async (credentials: LoginCredentials): Promise<LoginResponse> => {
    const response = await authLogin(credentials);
    
    // Store UI session ID for reconnection
    if (response.ui_session_id) {
      setUiSessionId(response.ui_session_id);
      storeUiSessionId(response.ui_session_id);
    }
    
    setIsAuthenticated(true);
    return response;
  }, []);

  // Periodic authentication check for token expiration
  useEffect(() => {
    const interval = setInterval(() => {
      const authenticated = checkIsAuthenticated();
      if (authenticated !== isAuthenticated) {
        setIsAuthenticated(authenticated);
        if (!authenticated) {
          // Clear session on expiration
          setUiSessionId(undefined);
          clearUiSessionId();
        }
      }
    }, 30000); // Check every 30 seconds
    
    return () => clearInterval(interval);
  }, [isAuthenticated]);
}
```

#### Token Storage and Management

```typescript
// src/lib/auth.ts
const AUTH_CONFIG = {
  tokenCookieName: 'agentc-auth-token',
  tokenCookieOptions: {
    secure: true, // Always use secure cookies
    sameSite: 'strict' as const,
    httpOnly: false, // Need JavaScript access for API calls
    path: '/',
    maxAge: 60 * 60 * 24 * 7, // 7 days
  },
};

// Token validation with expiration checking
export function isAuthenticated(): boolean {
  const token = getToken();
  if (!token) return false;
  
  const payload = parseJWT(token);
  if (!payload || !payload.exp) return false;
  
  // Check if token expires in less than 30 seconds
  const expirationTime = payload.exp * 1000;
  const currentTime = Date.now();
  const buffer = 30 * 1000;
  
  return currentTime < (expirationTime - buffer);
}
```

### Session Management

The demo implements UI session persistence for seamless reconnection:

```typescript
// Store session ID on successful login
const storeUiSessionId = (sessionId: string) => {
  localStorage.setItem('agentc-ui-session-id', sessionId);
};

// Restore session on page reload
const getStoredUiSessionId = (): string | undefined => {
  const stored = localStorage.getItem('agentc-ui-session-id');
  return stored || undefined;
};
```

### Protected Route Handling

The demo uses an `AuthGuard` component to protect routes:

```typescript
// src/components/chat/ChatPageClient.tsx
function AuthGuard({ children }: { children: React.ReactNode }) {
  const router = useRouter();
  const { isAuthenticated, isLoading } = useAuth();

  useEffect(() => {
    // Redirect to login if not authenticated
    if (!isLoading && !isAuthenticated) {
      router.push('/login');
    }
  }, [isLoading, isAuthenticated, router]);

  if (isLoading) {
    return <LoadingState />;
  }

  if (!isAuthenticated) {
    return <AuthRequiredMessage />;
  }

  return <>{children}</>;
}
```

### API Route Proxy Pattern

The demo uses Next.js API routes as a secure proxy layer:

```typescript
// src/app/api/auth/login/route.ts
export async function POST(request: NextRequest) {
  const credentials = await request.json();
  
  // Validate credentials
  if (!credentials.password || (!credentials.username && !credentials.email)) {
    return NextResponse.json({ error: 'Invalid credentials' }, { status: 400 });
  }

  // Forward to Agent C API with proper HTTPS handling
  const loginUrl = `${agentCApiUrl}/api/rt/login`;
  const response = await axios.post(loginUrl, credentials, {
    httpsAgent: isDevelopment ? new https.Agent({ rejectUnauthorized: false }) : undefined,
  });

  // Validate and return response
  const loginResponse = response.data;
  if (!loginResponse.agent_c_token) {
    return NextResponse.json({ error: 'No token received' }, { status: 500 });
  }

  return NextResponse.json(loginResponse, { status: 200 });
}
```

## Provider Architecture

The demo implements a layered provider architecture that ensures proper context isolation and dependency management.

### Provider Hierarchy

```typescript
// src/app/layout.tsx - Root layout with theme provider
export default function RootLayout({ children }: RootLayoutProps) {
  return (
    <html lang="en" suppressHydrationWarning>
      <body className={cn("min-h-screen bg-background antialiased", inter.className)}>
        <ThemeProvider
          attribute="class"
          defaultTheme="dark"
          enableSystem
          disableTransitionOnChange
        >
          {children}
          <Toaster />
        </ThemeProvider>
      </body>
    </html>
  );
}

// src/components/chat/ChatPageClient.tsx - Authentication and SDK providers
export default function ChatPageClient() {
  return (
    <AuthProvider>
      <AuthGuard>
        <ClientProvider
          apiUrl={process.env.NEXT_PUBLIC_AGENTC_API_URL}
          enableAudio={true}
          enableTurnManagement={true}
        >
          <ChatContent />
        </ClientProvider>
      </AuthGuard>
    </AuthProvider>
  );
}
```

### Context Provider Responsibilities

Each provider has a specific responsibility:

1. **ThemeProvider** - Manages dark/light mode across the application
2. **AuthProvider** - Handles authentication state and token management
3. **ClientProvider** - Configures and provides the Realtime SDK client
4. **AgentCProvider** (from SDK) - Manages SDK state and WebSocket connection

### StrictMode Handling

The `ClientProvider` component handles React StrictMode double-mounting gracefully:

```typescript
// src/components/providers/client-provider.tsx
export function ClientProvider({ children, apiUrl, enableAudio = true }: ClientProviderProps) {
  const { isAuthenticated, getAuthToken } = useAuth();
  const [configReady, setConfigReady] = useState(false);

  // Create configuration only when authenticated
  const clientConfig = useMemo<RealtimeClientConfig | null>(() => {
    const token = getAuthToken();
    if (!token) return null;

    return {
      apiUrl: apiUrl || process.env.NEXT_PUBLIC_AGENTC_API_URL,
      authToken: token,
      enableAudio,
      autoReconnect: true,
      reconnection: {
        enabled: true,
        maxAttempts: 5,
        initialDelay: 1000,
        maxDelay: 30000,
        backoffMultiplier: 1.5
      },
      audioConfig: {
        sampleRate: 24000,
        channelCount: 1,
        echoCancellation: true,
        noiseSuppression: true,
        autoGainControl: true
      }
    };
  }, [getAuthToken, apiUrl, enableAudio]);

  // Only render AgentCProvider when config is ready
  if (!clientConfig) {
    return <WaitingForAuth />;
  }

  return (
    <AgentCProvider config={clientConfig} autoConnect={true} debug={true}>
      {children}
    </AgentCProvider>
  );
}
```

## Chat Implementation Pattern

The chat interface demonstrates real-time message handling with a robust UI/UX.

### Real-time Message Handling

The demo uses the SDK's event system for message updates:

```typescript
// Message accumulation happens automatically in the SDK
// The demo consumes messages via the useChat hook:
const { messages, sendMessage, isTyping } = useChat();

// Messages are rendered in the ChatMessagesView component
// with automatic role-based styling (user vs assistant)
```

### Auto-scrolling Behavior

The demo implements smart auto-scrolling that maintains user position:

```typescript
// Auto-scroll to bottom on new messages
// but preserve position if user has scrolled up
const scrollToBottom = () => {
  if (messagesEndRef.current) {
    messagesEndRef.current.scrollIntoView({ behavior: 'smooth' });
  }
};

useEffect(() => {
  // Only auto-scroll if user is near bottom
  const isNearBottom = 
    scrollContainer.scrollHeight - scrollContainer.scrollTop < scrollContainer.clientHeight + 100;
  
  if (isNearBottom) {
    scrollToBottom();
  }
}, [messages]);
```

### Message Rendering Pipeline

Messages flow through this rendering pipeline:

1. **WebSocket Event** → Text delta received
2. **SDK Accumulation** → SessionManager accumulates text
3. **Hook Update** → useChat hook provides messages
4. **Component Render** → ChatMessagesView renders with role-based styling
5. **Auto-scroll** → Smart scrolling maintains UX

### Error Boundaries

While not explicitly implemented in the current demo, the pattern for error boundaries would be:

```typescript
class ChatErrorBoundary extends React.Component {
  state = { hasError: false, error: null };

  static getDerivedStateFromError(error: Error) {
    return { hasError: true, error };
  }

  componentDidCatch(error: Error, errorInfo: React.ErrorInfo) {
    Logger.error('Chat error:', error, errorInfo);
  }

  render() {
    if (this.state.hasError) {
      return <ErrorFallback error={this.state.error} />;
    }
    return this.props.children;
  }
}
```

## Audio System Integration

The demo showcases comprehensive audio system integration patterns.

### Microphone Access

The SDK handles microphone permissions, but the demo provides UI feedback:

```typescript
// Voice visualizer component shows audio level feedback
export const VoiceVisualizerView = () => {
  const { isRecording, audioLevel } = useAudio();

  return (
    <div className="relative">
      <AudioWaveform 
        className={cn(
          "h-16 w-16 mx-auto text-primary transition-all duration-200",
          isRecording && "animate-pulse"
        )}
        style={{
          opacity: isRecording ? 0.5 + (audioLevel * 0.5) : 0.5,
          transform: `scale(${1 + (audioLevel * 0.2)})`
        }}
      />
      {/* Audio level indicator */}
      <div className="w-48 h-2 bg-muted rounded-full overflow-hidden">
        <div 
          className="h-full bg-primary transition-all duration-100"
          style={{ width: `${audioLevel * 100}%` }}
        />
      </div>
    </div>
  );
};
```

### Audio Level Monitoring

The demo polls audio levels for real-time visualization:

```typescript
// The useAudio hook provides audio level with 100ms updates
const { audioLevel, isRecording, isMuted } = useAudio();

// Visual feedback scales with audio level
const visualScale = 1 + (audioLevel * 0.2);
const visualOpacity = 0.5 + (audioLevel * 0.5);
```

### Turn Management

The demo respects server-driven turn management:

```typescript
// Turn state prevents talk-over situations
const { currentTurn, canSpeak } = useTurnState();

// UI disables input when it's not user's turn
<Button 
  disabled={!canSpeak || currentTurn === 'agent'}
  onClick={startRecording}
>
  {currentTurn === 'agent' ? 'Agent is speaking...' : 'Start Recording'}
</Button>
```

## State Management Patterns

The demo demonstrates clear separation between SDK state and local state.

### SDK State vs Local State

```typescript
// SDK State (managed by the SDK, consumed via hooks)
const { messages } = useChat();           // Chat messages
const { isConnected } = useConnection();  // Connection state
const { audioLevel } = useAudio();        // Audio state
const { currentVoice } = useVoiceModel(); // Voice selection

// Local State (managed by components)
const [sessionName, setSessionName] = useState<string>('New Chat');
const [outputMode, setOutputMode] = useState<'chat' | 'avatar' | 'voice'>('chat');
const [sidebarOpen, setSidebarOpen] = useState(false);
```

### Session Persistence

The demo persists session data for reconnection:

```typescript
// Session ID stored for reconnection
localStorage.setItem('agentc-ui-session-id', sessionId);

// Settings persisted across sessions
const saveSettings = (settings: AppSettings) => {
  localStorage.setItem('app-settings', JSON.stringify(settings));
};

// Restore settings on mount
useEffect(() => {
  const stored = localStorage.getItem('app-settings');
  if (stored) {
    setSettings(JSON.parse(stored));
  }
}, []);
```

### Settings Management

User preferences are managed locally:

```typescript
interface AppSettings {
  theme: 'light' | 'dark' | 'system';
  audioEnabled: boolean;
  notifications: boolean;
  autoScroll: boolean;
}

// Settings context provides app-wide preferences
const SettingsContext = createContext<{
  settings: AppSettings;
  updateSettings: (partial: Partial<AppSettings>) => void;
}>({});
```

## Error Handling Patterns

The demo implements comprehensive error handling at multiple levels.

### Connection Errors

```typescript
// Connection error handling in ClientProvider
const [error, setError] = useState<string | null>(null);

// Display user-friendly error messages
if (error) {
  return (
    <Alert variant="destructive">
      <AlertDescription>
        <strong>Connection Error:</strong> {error}
      </AlertDescription>
    </Alert>
  );
}
```

### API Errors

The demo handles API errors gracefully:

```typescript
// Auth library error handling
try {
  const response = await authLogin(credentials);
  return response;
} catch (error) {
  Logger.error('Login failed', error);
  setIsAuthenticated(false);
  
  // Parse error for user display
  const message = error.response?.data?.error || 'Login failed';
  throw new Error(message);
}
```

### User-Friendly Error Display

Errors are displayed with clear, actionable messages:

```typescript
// Generic error component pattern
const ErrorMessage: React.FC<{ error: Error; onRetry?: () => void }> = ({ error, onRetry }) => {
  return (
    <Card className="max-w-md">
      <CardHeader>
        <CardTitle>Something went wrong</CardTitle>
        <CardDescription>{error.message}</CardDescription>
      </CardHeader>
      {onRetry && (
        <CardContent>
          <Button onClick={onRetry}>Try Again</Button>
        </CardContent>
      )}
    </Card>
  );
};
```

## Performance Optimizations

The demo implements several performance optimization patterns.

### Component Memoization

While not extensively used in the current demo, the pattern would be:

```typescript
// Memoize expensive components
const MessageList = React.memo(({ messages }: { messages: Message[] }) => {
  return (
    <div className="space-y-2">
      {messages.map(msg => (
        <MessageItem key={msg.id} message={msg} />
      ))}
    </div>
  );
}, (prevProps, nextProps) => {
  // Custom comparison for optimization
  return prevProps.messages.length === nextProps.messages.length;
});
```

### Virtual Scrolling

For large message lists, virtual scrolling can be implemented:

```typescript
// Pattern for virtual scrolling with react-window
import { VariableSizeList } from 'react-window';

const VirtualMessageList = ({ messages, height, width }) => {
  const getItemSize = (index: number) => {
    // Calculate height based on message content
    return estimateMessageHeight(messages[index]);
  };

  return (
    <VariableSizeList
      height={height}
      width={width}
      itemCount={messages.length}
      itemSize={getItemSize}
    >
      {({ index, style }) => (
        <div style={style}>
          <MessageItem message={messages[index]} />
        </div>
      )}
    </VariableSizeList>
  );
};
```

### Lazy Loading

Components are loaded on demand:

```typescript
// Dynamic imports for code splitting
const AvatarView = dynamic(() => import('./AvatarView'), {
  loading: () => <Skeleton className="h-full w-full" />,
  ssr: false
});

// Conditional rendering based on mode
{outputMode === 'avatar' && <AvatarView />}
```

### Debounced Updates

Input handling uses debouncing for performance:

```typescript
// Debounce pattern for search or filter inputs
const debouncedSearch = useMemo(
  () => debounce((value: string) => {
    performSearch(value);
  }, 300),
  []
);
```

## Responsive Design Patterns

The demo implements responsive design patterns for optimal mobile and desktop experiences.

### Mobile vs Desktop Layouts

```typescript
// useIsMobile hook for responsive behavior
export function useIsMobile() {
  const [isMobile, setIsMobile] = useState<boolean | undefined>(undefined);

  useEffect(() => {
    const mql = window.matchMedia(`(max-width: 767px)`);
    const onChange = () => {
      setIsMobile(window.innerWidth < 768);
    };
    mql.addEventListener("change", onChange);
    setIsMobile(window.innerWidth < 768);
    return () => mql.removeEventListener("change", onChange);
  }, []);

  return !!isMobile;
}

// Conditional rendering based on screen size
const isMobile = useIsMobile();

return (
  <div className={cn(
    "flex",
    isMobile ? "flex-col" : "flex-row"
  )}>
    {!isMobile && <Sidebar />}
    <MainContent />
    {isMobile && <MobileNav />}
  </div>
);
```

### Adaptive Components

Components adapt their layout based on screen size:

```typescript
// Responsive grid layouts
<div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
  {items.map(item => <Card key={item.id} {...item} />)}
</div>

// Responsive text sizes
<h1 className="text-2xl md:text-3xl lg:text-4xl font-bold">
  {title}
</h1>

// Responsive padding/spacing
<div className="p-4 md:p-6 lg:p-8">
  {content}
</div>
```

### Touch Interactions

Mobile-specific interaction patterns:

```typescript
// Touch-friendly button sizes
<Button className="h-12 md:h-10 px-6 md:px-4">
  Tap or Click
</Button>

// Swipe gestures for mobile navigation
const handleTouchStart = (e: TouchEvent) => {
  startX.current = e.touches[0].clientX;
};

const handleTouchEnd = (e: TouchEvent) => {
  const endX = e.changedTouches[0].clientX;
  const diff = startX.current - endX;
  
  if (Math.abs(diff) > 50) {
    if (diff > 0) {
      // Swipe left - next
      navigate('next');
    } else {
      // Swipe right - previous
      navigate('prev');
    }
  }
};
```

## Testing Patterns

The demo includes comprehensive testing utilities and patterns.

### Test Utilities

```typescript
// src/test/utils/demo-test-utils.tsx
export function renderDemo(
  ui: ReactElement,
  options?: {
    route?: string;
    client?: Partial<RealtimeClient>;
  }
): RenderResult & { user: UserEvent; client: Partial<RealtimeClient> } {
  const { route = '/', client = createDemoMockClient() } = options || {};
  const user = userEvent.setup();
  
  const result = render(
    <DemoWrapper initialRoute={route} client={client}>
      {ui}
    </DemoWrapper>
  );

  return { ...result, user, client };
}
```

### Page Object Models

The demo uses page objects for maintainable tests:

```typescript
export const pages = {
  chat: {
    selectors: {
      container: '[data-testid="chat-container"]',
      input: '[data-testid="chat-input"]',
      sendButton: '[data-testid="send-button"]',
      messageList: '[data-testid="message-list"]',
    },
    actions: {
      sendMessage: async (user: UserEvent, message: string) => {
        const input = document.querySelector(pages.chat.selectors.input);
        const sendButton = document.querySelector(pages.chat.selectors.sendButton);
        await user.type(input, message);
        await user.click(sendButton);
      },
    }
  }
};
```

### Mock State Management

Test utilities for state mocking:

```typescript
export const mockState = {
  createInitialState: () => ({
    connection: { isConnected: false, status: 'disconnected' },
    chat: { messages: [], isTyping: false },
    audio: { isEnabled: false, isMuted: false, level: 0 }
  }),

  createConnectedState: () => ({
    ...mockState.createInitialState(),
    connection: { isConnected: true, status: 'connected' },
    session: { id: 'test-session-123', startTime: new Date().toISOString() }
  }),

  createWithMessages: (count: number = 5) => {
    const state = mockState.createConnectedState();
    state.chat.messages = generate.messages(count);
    return state;
  }
};
```

### Test Scenarios

Common test scenarios are pre-defined:

```typescript
export const scenarios = {
  testChatFlow: async (user: UserEvent) => {
    // Connect
    await pages.connection.actions.connect(user);
    
    // Send message
    await pages.chat.actions.sendMessage(user, 'Hello!');
    
    // Wait for response
    await waitFor(() => {
      const messages = document.querySelectorAll(pages.chat.selectors.message);
      expect(messages.length).toBeGreaterThan(0);
    });
  },

  testSettingsPersistence: async (user: UserEvent) => {
    // Change settings
    await pages.settings.actions.toggleTheme(user);
    await pages.settings.actions.saveSettings(user);
    
    // Check persistence
    const settings = storage.get('app-settings');
    expect(settings.theme).toBeDefined();
  }
};
```

## Best Practices Summary

### Authentication & Security
- ✅ Use secure, httpOnly cookies for sensitive tokens when possible
- ✅ Implement token expiration checking with buffer time
- ✅ Provide clear authentication state feedback
- ✅ Use API routes as a proxy layer for backend communication
- ✅ Store session IDs for reconnection capability

### State Management
- ✅ Separate SDK state from local component state
- ✅ Use context providers for cross-cutting concerns
- ✅ Implement proper provider hierarchy
- ✅ Persist user preferences in localStorage
- ✅ Handle StrictMode double-mounting gracefully

### UI/UX Patterns
- ✅ Provide loading states for all async operations
- ✅ Implement smart auto-scrolling for chat
- ✅ Show real-time audio level feedback
- ✅ Respect server-driven turn management
- ✅ Adapt layouts for mobile and desktop

### Error Handling
- ✅ Display user-friendly error messages
- ✅ Provide retry mechanisms where appropriate
- ✅ Log errors for debugging
- ✅ Handle connection failures gracefully
- ✅ Validate data at multiple levels

### Performance
- ✅ Lazy load heavy components
- ✅ Debounce rapid user inputs
- ✅ Use proper React patterns (memo, useMemo, useCallback)
- ✅ Implement virtual scrolling for long lists
- ✅ Optimize WebSocket message handling

### Testing
- ✅ Use page object models for maintainability
- ✅ Create reusable test utilities
- ✅ Mock external dependencies properly
- ✅ Test both happy paths and error cases
- ✅ Implement integration tests for critical flows

## Conclusion

The Agent C Realtime demo application demonstrates production-ready patterns for building real-time communication interfaces. By following these patterns, developers can create robust, performant, and user-friendly applications that leverage the full power of the Agent C Realtime SDK.

For more information about specific SDK features, refer to the [SDK API Reference](../index.md) and the [Realtime API Implementation Guide](../../../api/docs/realtime_api_implementation_guide.md).
# Chat Components API Reference

This document provides comprehensive documentation for all chat components in the `@agentc/realtime-ui` package. These components are built following CenSuite design system guidelines and integrate seamlessly with the `@agentc/realtime-react` hooks.

## Table of Contents

1. [ChatMessagesView](#chatmessagesview)
2. [Message](#message)
3. [MessageList](#messagelist)
4. [MessageContentRenderer](#messagecontentrenderer)
5. [MessageFooter](#messagefooter)
6. [TypingIndicator](#typingindicator)
7. [ScrollAnchor](#scrollanchor)
8. [SubsessionDivider](#subsessiondivider)
9. [ToolNotification](#toolnotification)
10. [Content Renderers](#content-renderers)
11. [Complete Chat Implementation Example](#complete-chat-implementation-example)

---

## ChatMessagesView

Main chat message display container that integrates MessageList with typing indicators and auto-scrolling.

### Props Interface

```typescript
export interface ChatMessagesViewProps {
  className?: string
}
```

### Usage Example

```tsx
import { ChatMessagesView } from '@agentc/realtime-ui'

function ChatInterface() {
  return (
    <div className="h-screen flex flex-col">
      {/* Chat messages take full height */}
      <ChatMessagesView className="flex-1" />
      {/* Input area below */}
      <ChatInput />
    </div>
  )
}
```

### CenSuite Design Compliance
- Uses proper spacing scale (px-4 py-4)
- Implements max-width constraint for optimal reading (max-w-4xl)
- Centered content with mx-auto
- Overflow handling with smooth scrolling

### Accessibility Features
- Proper scroll container management
- Focus management through ScrollAnchor component
- Screen reader compatible structure

---

## Message

Core message component that renders individual chat messages with full support for various content types, tool calls, and interactive features.

### Props Interface

```typescript
export interface MessageData extends SDKMessage {
  id?: string                           // Optional ID for keying
  status?: 'sending' | 'sent' | 'error' // Message status
  error?: string                         // Error message if status is 'error'
  isThought?: boolean                   // Flag for thought messages
  metadata?: {
    // Session-related metadata
    sessionId?: string
    parentSessionId?: string
    userSessionId?: string
    // Token-related metadata for usage tracking
    inputTokens?: number
    outputTokens?: number
  }
  toolCalls?: Array<{
    id: string
    type: 'tool_use'
    name: string
    input: Record<string, unknown>
  }>
  toolResults?: Array<{
    type: 'tool_result'
    tool_use_id: string
    content: string
  }>
}

export interface MessageProps extends React.HTMLAttributes<HTMLDivElement> {
  message: MessageData                  // The message data to display
  showTimestamp?: boolean               // Show timestamp (default: true)
  isStreaming?: boolean                 // Message is currently streaming
  isSubSession?: boolean                // Message is from a sub-session
  avatarComponent?: React.ReactNode     // Custom avatar component
  onEdit?: (id: string, content: string) => void  // Edit callback
  showFooter?: boolean                  // Show footer with metadata (default: true)
}
```

### Usage Examples

#### Basic Message Rendering
```tsx
import { Message } from '@agentc/realtime-ui'

const message = {
  role: 'assistant',
  content: 'Hello! How can I help you today?',
  timestamp: new Date().toISOString()
}

<Message 
  message={message}
  showTimestamp={true}
  showFooter={true}
/>
```

#### Message with Tool Calls
```tsx
const messageWithTools = {
  role: 'assistant',
  content: 'Let me search for that information.',
  toolCalls: [{
    id: 'tool_123',
    type: 'tool_use',
    name: 'search_database',
    input: { query: 'user request' }
  }],
  metadata: {
    inputTokens: 150,
    outputTokens: 230
  }
}

<Message 
  message={messageWithTools}
  showFooter={true}  // Will display tool calls and token counts
/>
```

#### Thought Message (Special Rendering)
```tsx
const thoughtMessage = {
  role: 'assistant',
  content: 'Analyzing the user request to determine the best approach...',
  isThought: true,
  metadata: {
    outputTokens: 45
  }
}

<Message message={thoughtMessage} />
// Renders as collapsible thought bubble with Brain icon
```

### CenSuite Design Compliance
- **Colors**: Semantic color tokens only (bg-muted, bg-card, text-foreground)
- **Spacing**: 4px base unit system (px-4 py-2.5)
- **Typography**: Proper hierarchy (text-[0.9375rem] for body)
- **Visual Hierarchy**: User messages right-aligned, assistant left-aligned
- **Status Indicators**: Consistent color usage (destructive for errors)

### Accessibility Features
- `role="article"` with proper aria-label
- Keyboard navigation for edit mode (Cmd+Enter to save, Esc to cancel)
- Focus management with focus-visible states
- Screen reader announcements for message status

### Message Status States
- **sending**: Shows "Sending..." indicator
- **sent**: Shows check mark
- **error**: Red background with error message
- **streaming**: Shows pulsing cursor animation

---

## MessageList

Intelligent message list component with auto-scrolling behavior and virtual scrolling support.

### Props Interface

```typescript
export interface MessageListProps extends React.HTMLAttributes<HTMLDivElement> {
  maxHeight?: string              // Maximum height (default: '600px', 'none' for no limit)
  showTimestamps?: boolean        // Show timestamps on messages (default: true)
  enableVirtualScroll?: boolean   // Enable virtual scrolling for performance
  emptyStateComponent?: React.ReactNode  // Custom empty state component
}
```

### Usage Examples

#### Basic Message List
```tsx
import { MessageList } from '@agentc/realtime-ui'

function ChatContainer() {
  return (
    <MessageList 
      maxHeight="600px"
      showTimestamps={true}
      className="border rounded-lg"
    />
  )
}
```

#### Full Height Message List
```tsx
// For full viewport height layouts
<MessageList 
  maxHeight="none"
  className="h-full overflow-y-auto"
  enableVirtualScroll={true}  // Recommended for large message counts
/>
```

#### Custom Empty State
```tsx
<MessageList 
  emptyStateComponent={
    <div className="text-center py-12">
      <Bot className="h-12 w-12 mx-auto mb-4 text-muted-foreground" />
      <h3>Ready to assist!</h3>
      <p>Ask me anything to get started.</p>
    </div>
  }
/>
```

### Auto-Scrolling Behavior
The component implements intelligent auto-scrolling:
- Auto-scrolls to bottom when new messages arrive
- Disables auto-scroll when user manually scrolls up
- Re-enables auto-scroll when user scrolls back within 50px of bottom
- Smooth scrolling for better UX
- Instant scroll for initial/bulk loads

### Integration with React Hooks
```tsx
import { useChat } from '@agentc/realtime-react'

// MessageList automatically consumes these from useChat:
// - messages: Array of chat messages
// - isAgentTyping: Shows typing indicator
// - streamingMessage: Currently streaming message
// - isSubSessionMessage: Function to check subsession status
```

### CenSuite Design Compliance
- Consistent spacing between messages (space-y-4)
- Proper loading states with Loader2 component
- Smooth animations (animate-in slide-in-from-bottom-2)
- Semantic HTML structure with proper ARIA attributes

### Accessibility Features
- `role="log"` for message container
- `aria-label="Chat messages"`
- `aria-live="polite"` for dynamic updates
- Scroll sentinel for efficient detection
- Screen reader announcements for typing state

---

## MessageContentRenderer

Main content router component that handles all message content types and routes them to appropriate renderers.

### Props Interface

```typescript
export interface MessageContentRendererProps {
  content: MessageContent  // String, ContentPart[], or null
  role: 'user' | 'assistant' | 'system'
  className?: string
}
```

### Content Types Supported

1. **Text Content**: Markdown with full GFM support
2. **Image Content**: Base64 or URL images
3. **Tool Use**: Tool invocation displays
4. **Tool Results**: Tool execution results
5. **Mixed Content**: Arrays of different content types

### Usage Examples

#### Text Content with Markdown
```tsx
<MessageContentRenderer
  content="# Hello\n\nThis is **bold** and `code`"
  role="assistant"
/>
```

#### Mixed Content Array
```tsx
const mixedContent = [
  { type: 'text', text: 'Here is the image you requested:' },
  { 
    type: 'image', 
    source: { 
      type: 'base64', 
      media_type: 'image/png',
      data: 'base64string...' 
    }
  },
  { 
    type: 'tool_use',
    id: 'tool_123',
    name: 'generate_image',
    input: { prompt: 'A beautiful sunset' }
  }
]

<MessageContentRenderer
  content={mixedContent}
  role="assistant"
/>
```

### Content Renderers

The component routes to specialized renderers:
- `TextContentRenderer`: Full markdown support with code highlighting
- `ImageContentRenderer`: Image display with loading states
- `ToolUseContentRenderer`: Tool invocation visualization
- `ToolResultContentRenderer`: Tool execution results
- `UnknownContentRenderer`: Graceful fallback for unknown types

---

## MessageFooter

Displays message metadata, tool calls, and action buttons.

### Props Interface

```typescript
export interface MessageFooterProps extends React.HTMLAttributes<HTMLDivElement> {
  message: MessageData
  onEdit?: () => void
  onRegenerate?: () => void
  showTimestamp?: boolean
}
```

### Features
- Token count display (input/output/total)
- Expandable tool call details
- Copy message content
- Edit user messages
- Regenerate assistant responses
- Timestamp display

### Usage Example

```tsx
<MessageFooter
  message={message}
  onEdit={() => handleEdit(message.id)}
  onRegenerate={() => handleRegenerate()}
  showTimestamp={true}
/>
```

### Tool Call Display
The footer includes an expandable section for tool calls:
- Tool name and status
- Input arguments (JSON formatted)
- Execution results
- Copy functionality for inputs/outputs

---

## TypingIndicator

Animated typing indicator with multiple variants and sizes.

### Props Interface

```typescript
export interface TypingIndicatorProps extends React.HTMLAttributes<HTMLDivElement> {
  variant?: 'dots' | 'pulse' | 'wave'  // Animation variant (default: 'dots')
  size?: 'sm' | 'md' | 'lg'           // Size of indicator (default: 'md')
  label?: string                       // Screen reader label (default: 'Assistant is typing')
  speed?: number                       // Animation speed in ms (default: 300)
}
```

### Usage Examples

```tsx
// Default dots animation
<TypingIndicator />

// Wave animation with large size
<TypingIndicator variant="wave" size="lg" />

// Custom speed and label
<TypingIndicator 
  variant="pulse" 
  speed={400}
  label="Agent is processing your request"
/>
```

### Animation Variants
1. **dots**: Three bouncing dots (default)
2. **pulse**: Fading pulse effect
3. **wave**: Wave-like scaling animation

### CenSuite Compliance
- Uses muted background colors (bg-muted)
- Proper padding based on size (px-2/3/4)
- Smooth fade-in animation
- Consistent with system loading patterns

---

## ScrollAnchor

Manages auto-scrolling behavior and provides a floating "New messages" button.

### Props Interface

```typescript
export interface ScrollAnchorProps {
  scrollContainerRef: React.RefObject<HTMLElement>
  dependencies?: any[]          // Trigger scroll checks
  threshold?: number            // Pixels from bottom (default: 100)
  showFloatingButton?: boolean  // Show floating button (default: true)
  floatingButtonComponent?: React.ReactNode
  onAutoScrollChange?: (enabled: boolean) => void
}
```

### Usage Example

```tsx
function MessageContainer() {
  const scrollRef = useRef<HTMLDivElement>(null)
  const { messages } = useChat()

  return (
    <div ref={scrollRef} className="overflow-y-auto">
      <MessageList />
      <ScrollAnchor 
        scrollContainerRef={scrollRef}
        dependencies={[messages.length]}
        showFloatingButton={true}
      />
    </div>
  )
}
```

### Features
- Intersection Observer for efficient detection
- Floating "New messages" button
- Message count indicator
- Smooth scroll animations
- Screen reader announcements

---

## SubsessionDivider

Visual indicator for agent-to-agent conversation boundaries.

### Props Interface

```typescript
export interface SubsessionDividerProps extends React.HTMLAttributes<HTMLDivElement> {
  type: 'start' | 'end'      // Type of subsession event
  timestamp?: string         // Optional timestamp
  label?: string            // Custom label
}
```

### Usage Example

```tsx
<SubsessionDivider 
  type="start"
  timestamp={new Date().toISOString()}
  label="Agent consultation started"
/>
```

### Visual Design
- Horizontal line with centered label
- Different colors for start (primary) vs end (muted)
- Icon indicators (MessageSquarePlus/MessageSquareOff)
- Fade-in animation

---

## ToolNotification

Progressive status updates for tool calls with special handling for "think" tools.

### Props Interface

```typescript
export interface ToolNotificationData {
  id: string
  toolName: string
  status: 'preparing' | 'executing' | 'complete'
  timestamp: Date
  arguments?: string
}

export interface ToolNotificationProps extends React.HTMLAttributes<HTMLDivElement> {
  notification: ToolNotificationData
  isThinkTool?: boolean
}
```

### Usage Examples

#### Single Tool Notification
```tsx
const notification = {
  id: 'tool_1',
  toolName: 'search_database',
  status: 'executing',
  timestamp: new Date()
}

<ToolNotification notification={notification} />
```

#### Tool Notification List
```tsx
const notifications = [
  { id: '1', toolName: 'think', status: 'executing', timestamp: new Date() },
  { id: '2', toolName: 'search', status: 'complete', timestamp: new Date() }
]

<ToolNotificationList 
  notifications={notifications}
  maxNotifications={3}
/>
```

### Status States
- **preparing**: Light background, "preparing to use" message
- **executing**: Primary tint, animated spinner
- **complete**: Green tint, check mark icon

### Special "Think" Tool Handling
- Brain icon instead of wrench
- Pulse animation instead of spin
- Custom status messages

---

## Content Renderers

Specialized components for rendering different content types.

### TextContentRenderer

Full markdown support with syntax highlighting and copy functionality.

```typescript
export interface TextContentRendererProps {
  content: string
  role: 'user' | 'assistant' | 'system'
  className?: string
}
```

Features:
- GitHub Flavored Markdown (GFM)
- Code block syntax highlighting
- Copy code button
- Tables, lists, blockquotes
- Link handling (external links open in new tab)

### ImageContentRenderer

Handles both base64 and URL images.

```typescript
export interface ImageContentRendererProps {
  source: {
    type: 'base64' | 'url'
    media_type?: string
    data?: string  // For base64
    url?: string   // For URL
  }
  index: number
}
```

### ToolUseContentRenderer

Displays tool invocations with formatted arguments.

```typescript
export interface ToolUseContentRendererProps {
  id: string
  name: string
  input: Record<string, any>
}
```

### ToolResultContentRenderer

Shows tool execution results with error handling.

```typescript
export interface ToolResultContentRendererProps {
  toolUseId: string
  content: string | ContentPart[]
  isError?: boolean
  role: 'user' | 'assistant' | 'system'
}
```

---

## Complete Chat Implementation Example

Here's a comprehensive example showing how to build a complete chat interface:

```tsx
import React from 'react'
import { 
  AgentCProvider,
  useConnection,
  useChat 
} from '@agentc/realtime-react'
import { 
  ChatMessagesView,
  Button,
  Input,
  Card
} from '@agentc/realtime-ui'

// Main Chat Application
function ChatApplication() {
  return (
    <AgentCProvider 
      apiUrl={process.env.NEXT_PUBLIC_AGENTC_API_URL}
      enableAudio={true}
      enableTurnManagement={true}
    >
      <ChatInterface />
    </AgentCProvider>
  )
}

// Chat Interface Component
function ChatInterface() {
  const { isConnected, connect, disconnect } = useConnection()
  const { sendMessage } = useChat()
  const [input, setInput] = React.useState('')

  const handleSend = () => {
    if (input.trim() && isConnected) {
      sendMessage(input)
      setInput('')
    }
  }

  return (
    <div className="flex flex-col h-screen bg-background">
      {/* Header */}
      <header className="border-b px-4 py-3">
        <div className="flex items-center justify-between max-w-4xl mx-auto">
          <h1 className="text-xl font-semibold">Agent C Chat</h1>
          <Button
            variant={isConnected ? 'outline' : 'default'}
            size="sm"
            onClick={isConnected ? disconnect : connect}
          >
            {isConnected ? 'Disconnect' : 'Connect'}
          </Button>
        </div>
      </header>

      {/* Messages Area */}
      <div className="flex-1 overflow-hidden">
        <ChatMessagesView className="h-full" />
      </div>

      {/* Input Area */}
      <div className="border-t bg-background/95 backdrop-blur supports-[backdrop-filter]:bg-background/60">
        <div className="max-w-4xl mx-auto p-4">
          <div className="flex gap-2">
            <Input
              value={input}
              onChange={(e) => setInput(e.target.value)}
              onKeyDown={(e) => {
                if (e.key === 'Enter' && !e.shiftKey) {
                  e.preventDefault()
                  handleSend()
                }
              }}
              placeholder="Type a message..."
              disabled={!isConnected}
              className="flex-1"
              aria-label="Message input"
            />
            <Button 
              onClick={handleSend}
              disabled={!isConnected || !input.trim()}
              aria-label="Send message"
            >
              Send
            </Button>
          </div>
        </div>
      </div>
    </div>
  )
}

export default ChatApplication
```

### Mobile Optimization Example

```tsx
// Responsive chat layout
function ResponsiveChatInterface() {
  const { messages } = useChat()
  const isMobile = useMediaQuery('(max-width: 768px)')

  return (
    <div className="flex flex-col h-[100dvh]"> {/* Dynamic viewport height */}
      {/* Adjusted spacing for mobile */}
      <ChatMessagesView 
        className={cn(
          "flex-1",
          isMobile ? "px-2" : "px-4"
        )}
      />
      
      {/* Mobile-optimized input */}
      <div className={cn(
        "border-t",
        isMobile ? "p-2" : "p-4"
      )}>
        <ChatInput 
          size={isMobile ? "sm" : "default"}
          autoFocus={!isMobile}
        />
      </div>
    </div>
  )
}
```

### Error Handling Example

```tsx
function RobustChatInterface() {
  const { connectionState, error, reconnect } = useConnection()
  const { messages, sendMessage } = useChat()

  if (connectionState === 'error') {
    return (
      <Card className="max-w-md mx-auto mt-8 p-6">
        <h2 className="text-lg font-semibold mb-2">Connection Error</h2>
        <p className="text-muted-foreground mb-4">{error?.message}</p>
        <Button onClick={reconnect} variant="default">
          Retry Connection
        </Button>
      </Card>
    )
  }

  return <ChatMessagesView />
}
```

### Custom Message Rendering

```tsx
function CustomMessageChat() {
  const { messages } = useChat()

  return (
    <div className="space-y-4 p-4">
      {messages.map((message, index) => (
        <Message
          key={message.id || index}
          message={message}
          avatarComponent={
            message.role === 'user' ? (
              <UserAvatar userId={message.userId} />
            ) : (
              <AgentAvatar agentName={message.agentName} />
            )
          }
          onEdit={(id, content) => {
            // Custom edit handler
            console.log('Edit message:', id, content)
          }}
          showFooter={true}
          showTimestamp={true}
        />
      ))}
    </div>
  )
}
```

## Performance Considerations

### Virtual Scrolling
For chat interfaces with thousands of messages:

```tsx
<MessageList 
  enableVirtualScroll={true}
  maxHeight="100vh"
/>
```

### Lazy Loading
Components use React.lazy and Suspense internally for heavy features:
- Markdown rendering
- Syntax highlighting
- Image loading

### Memoization
Key components use React.memo and useMemo:
- Message content extraction
- Timestamp formatting
- Tool result parsing

## Accessibility Best Practices

1. **Keyboard Navigation**
   - All interactive elements are keyboard accessible
   - Tab order follows visual hierarchy
   - Escape key closes expandable sections

2. **Screen Reader Support**
   - Proper ARIA labels on all components
   - Live regions for dynamic content
   - Status announcements for tool operations

3. **Focus Management**
   - Focus visible states on all interactive elements
   - Focus trapping in modals/dialogs
   - Auto-focus on input after sending message

4. **Color Contrast**
   - All text meets WCAG 2.1 AA standards
   - Status colors have sufficient contrast
   - Alternative indicators beyond color

## Testing Guidelines

### Unit Testing

```tsx
import { render, screen, fireEvent } from '@testing-library/react'
import { Message } from '@agentc/realtime-ui'

test('renders user message correctly', () => {
  const message = {
    role: 'user',
    content: 'Hello world',
    timestamp: new Date().toISOString()
  }

  render(<Message message={message} />)
  
  expect(screen.getByText('Hello world')).toBeInTheDocument()
  expect(screen.getByRole('article')).toHaveAttribute(
    'aria-label', 
    'Message from user'
  )
})

test('handles edit mode', () => {
  const onEdit = vi.fn()
  const message = { role: 'user', content: 'Test', id: '123' }

  const { getByText } = render(
    <Message message={message} onEdit={onEdit} />
  )

  fireEvent.click(getByText('Edit'))
  // Edit UI should appear
})
```

### Integration Testing

```tsx
import { renderWithProviders } from '@/test-utils'
import { ChatMessagesView } from '@agentc/realtime-ui'

test('auto-scrolls on new messages', async () => {
  const { rerender } = renderWithProviders(<ChatMessagesView />)
  
  // Simulate new message arrival
  act(() => {
    // Add message through context
  })
  
  // Verify scroll position
  await waitFor(() => {
    expect(scrollContainer.scrollTop).toBe(scrollContainer.scrollHeight)
  })
})
```

## Migration Guide

If migrating from a custom chat implementation:

1. **Replace message rendering**:
   ```tsx
   // Before
   <div>{message.text}</div>
   
   // After
   <Message message={message} />
   ```

2. **Update message list**:
   ```tsx
   // Before
   {messages.map(msg => <CustomMessage />)}
   
   // After
   <MessageList />
   ```

3. **Add proper providers**:
   ```tsx
   <AgentCProvider>
     <ChatMessagesView />
   </AgentCProvider>
   ```

## Troubleshooting

### Common Issues

1. **Messages not appearing**
   - Ensure AgentCProvider is properly configured
   - Check WebSocket connection status
   - Verify message format matches MessageData interface

2. **Auto-scroll not working**
   - Container must have defined height
   - Check for CSS conflicts with overflow properties
   - Ensure ScrollAnchor is properly positioned

3. **Typing indicator not showing**
   - Verify isAgentTyping from useChat hook
   - Check that no streamingMessage is active
   - Ensure WebSocket events are properly handled

## Related Documentation

- [RealtimeClient SDK Reference](../core/realtime-client.md)
- [React Hooks Reference](../react/hooks.md)
- [WebSocket Events](../core/websocket-events.md)
- [CenSuite Design System](../../design/censuite-guidelines.md)
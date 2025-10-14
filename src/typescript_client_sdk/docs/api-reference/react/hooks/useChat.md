# useChat

## Overview

The `useChat` hook provides a comprehensive interface for managing chat functionality in React applications using the Agent C Realtime SDK. It handles message sending, receiving, streaming, and session management while maintaining proper synchronization with the server's event stream.

## Purpose

This hook serves as the primary interface for:
- Sending and receiving chat messages
- Managing streaming messages from agents
- Tracking typing indicators and agent processing states
- Handling sub-session messages from agent tools
- Managing chat history with configurable limits
- Providing filtered access to messages by role

## Import Statement

```typescript
import { useChat } from '@agentc/realtime-react';
import type { UseChatOptions, UseChatReturn } from '@agentc/realtime-react';
```

## TypeScript Types

### Options Interface

```typescript
interface UseChatOptions {
  /** Maximum number of messages to keep in memory (default: 100) */
  maxMessages?: number;
  
  /** Whether to auto-scroll to new messages */
  autoScroll?: boolean;
}
```

### Return Interface

```typescript
interface UseChatReturn {
  /** Current chat messages with extended metadata */
  messages: ExtendedMessage[];
  
  /** Current session information */
  currentSession: ChatSession | null;
  
  /** Current session ID */
  currentSessionId: string | null;
  
  /** Send a text message */
  sendMessage: (text: string) => Promise<void>;
  
  /** Clear chat history (client-side only) */
  clearMessages: () => void;
  
  /** Whether a message is currently being sent */
  isSending: boolean;
  
  /** Whether the agent is currently typing/processing */
  isAgentTyping: boolean;
  
  /** Current streaming message from agent */
  streamingMessage: ExtendedMessage | null;
  
  /** Error state */
  error: string | null;
  
  /** Get the last message */
  lastMessage: ExtendedMessage | null;
  
  /** Get messages from a specific role */
  getMessagesByRole: (role: 'user' | 'assistant' | 'system') => ExtendedMessage[];
  
  /** Check if a message is from a sub-session */
  isSubSessionMessage: (message: ExtendedMessage) => boolean;
}
```

### Extended Message Type

```typescript
interface ExtendedMessage extends Message {
  isSubSession?: boolean;
  metadata?: {
    sessionId: string;
    parentSessionId?: string;
    userSessionId?: string;
  };
}
```

## Return Value Descriptions

### State Properties

- **`messages`**: Array of all chat messages including both complete and historical messages. Limited by `maxMessages` option.
- **`currentSession`**: Full session object containing session metadata, agent configuration, and settings.
- **`currentSessionId`**: String identifier for the active chat session.
- **`streamingMessage`**: Partial message being streamed from the agent, updated in real-time.
- **`isSending`**: Boolean flag indicating if a user message is currently being sent.
- **`isAgentTyping`**: Boolean flag showing if the agent is processing/about to respond.
- **`error`**: Error message string if an operation fails, null otherwise.
- **`lastMessage`**: Reference to the most recent message in the chat history.

### Methods

- **`sendMessage(text)`**: Asynchronously sends a text message to the agent. Throws errors if not connected or message is empty.
- **`clearMessages()`**: Clears all messages from the local state (does not affect server).
- **`getMessagesByRole(role)`**: Filters and returns messages from a specific participant role.
- **`isSubSessionMessage(message)`**: Checks if a message originated from an agent's sub-session (tool usage).

## Usage Examples

### Basic Chat Implementation

```typescript
import React from 'react';
import { useChat } from '@agentc/realtime-react';

function ChatComponent() {
  const {
    messages,
    sendMessage,
    isSending,
    isAgentTyping,
    streamingMessage,
    error
  } = useChat();
  
  const handleSend = async (text: string) => {
    try {
      await sendMessage(text);
    } catch (err) {
      console.error('Failed to send message:', err);
    }
  };
  
  return (
    <div>
      {/* Message list */}
      <div className="messages">
        {messages.map((msg, index) => (
          <div key={index} className={`message ${msg.role}`}>
            {msg.content}
          </div>
        ))}
        
        {/* Show streaming message */}
        {streamingMessage && (
          <div className="message assistant streaming">
            {streamingMessage.content}
          </div>
        )}
        
        {/* Show typing indicator when no streaming content */}
        {isAgentTyping && !streamingMessage && (
          <div className="typing-indicator">Agent is typing...</div>
        )}
      </div>
      
      {/* Error display */}
      {error && <div className="error">{error}</div>}
      
      {/* Send button state */}
      <button onClick={() => handleSend('Hello')} disabled={isSending}>
        {isSending ? 'Sending...' : 'Send'}
      </button>
    </div>
  );
}
```

### Advanced Message Filtering

```typescript
function FilteredChat() {
  const {
    messages,
    getMessagesByRole,
    isSubSessionMessage,
    currentSessionId
  } = useChat({ maxMessages: 50 });
  
  // Get only user messages
  const userMessages = getMessagesByRole('user');
  
  // Get only assistant messages
  const assistantMessages = getMessagesByRole('assistant');
  
  // Separate main session and sub-session messages
  const mainMessages = messages.filter(msg => !isSubSessionMessage(msg));
  const toolMessages = messages.filter(msg => isSubSessionMessage(msg));
  
  return (
    <div>
      <h3>Session: {currentSessionId}</h3>
      
      <div className="main-chat">
        <h4>Main Conversation ({mainMessages.length} messages)</h4>
        {mainMessages.map((msg, idx) => (
          <div key={idx}>{msg.content}</div>
        ))}
      </div>
      
      <div className="tool-outputs">
        <h4>Tool Outputs ({toolMessages.length} messages)</h4>
        {toolMessages.map((msg, idx) => (
          <div key={idx} className="tool-message">
            <span className="session-id">{msg.metadata?.sessionId}</span>
            <span>{msg.content}</span>
          </div>
        ))}
      </div>
      
      <div className="stats">
        <p>User messages: {userMessages.length}</p>
        <p>Assistant messages: {assistantMessages.length}</p>
      </div>
    </div>
  );
}
```

### Streaming Message Handling

```typescript
function StreamingChat() {
  const {
    messages,
    streamingMessage,
    isAgentTyping,
    lastMessage
  } = useChat();
  
  // Combine messages with streaming for display
  const displayMessages = React.useMemo(() => {
    const allMessages = [...messages];
    
    // Add streaming message if present
    if (streamingMessage) {
      allMessages.push(streamingMessage);
    }
    
    return allMessages;
  }, [messages, streamingMessage]);
  
  return (
    <div>
      {displayMessages.map((msg, index) => (
        <div
          key={msg.message_id || `streaming-${index}`}
          className={`message ${msg === streamingMessage ? 'streaming' : ''}`}
        >
          {msg.role === 'assistant' && msg === streamingMessage ? (
            <>
              {msg.content}
              <span className="cursor">▊</span>
            </>
          ) : (
            msg.content
          )}
        </div>
      ))}
      
      {/* Show typing only when agent is processing but not yet streaming */}
      {isAgentTyping && !streamingMessage && (
        <div className="typing">
          <span>●</span>
          <span>●</span>
          <span>●</span>
        </div>
      )}
      
      {/* Auto-scroll trigger */}
      {lastMessage && (
        <div ref={(el) => el?.scrollIntoView({ behavior: 'smooth' })} />
      )}
    </div>
  );
}
```

## Message Handling Patterns

### Event Flow

1. **User Message Send**:
   - Call `sendMessage(text)`
   - Hook sets `isSending` to true
   - Client sends text via WebSocket
   - Server processes and emits events
   - Hook receives `message-added` event
   - Message appears in `messages` array
   - `isSending` becomes false

2. **Agent Response Stream**:
   - User turn ends (`isAgentTyping` becomes true)
   - Server starts streaming response
   - Hook receives `message-streaming` events
   - `streamingMessage` updates with partial content
   - Hook receives `message-complete` event
   - Message moves to `messages` array
   - `streamingMessage` clears, `isAgentTyping` becomes false

3. **Session Message Loading**:
   - Session change triggers `session-messages-loaded` event
   - Hook replaces entire `messages` array
   - Applies `maxMessages` limit if configured
   - Clears any streaming state

### Sub-Session Message Handling

Sub-sessions are created when agents use tools or make function calls:

```typescript
function ToolMessageHandler() {
  const { messages, isSubSessionMessage } = useChat();
  
  // Group messages by session
  const messagesBySession = messages.reduce((acc, msg) => {
    const sessionId = msg.metadata?.sessionId || 'main';
    if (!acc[sessionId]) acc[sessionId] = [];
    acc[sessionId].push(msg);
    return acc;
  }, {} as Record<string, ExtendedMessage[]>);
  
  // Identify tool usage
  const toolSessions = Object.keys(messagesBySession)
    .filter(id => id !== 'main')
    .map(id => ({
      sessionId: id,
      messages: messagesBySession[id],
      parentId: messagesBySession[id][0]?.metadata?.parentSessionId
    }));
  
  return (
    <div>
      {toolSessions.map(session => (
        <div key={session.sessionId} className="tool-session">
          <h5>Tool Session: {session.sessionId}</h5>
          <p>Parent: {session.parentId}</p>
          {session.messages.map((msg, idx) => (
            <div key={idx}>{msg.content}</div>
          ))}
        </div>
      ))}
    </div>
  );
}
```

## Session Management Strategies

### Session Switching

```typescript
function SessionManager() {
  const { currentSession, currentSessionId, messages, clearMessages } = useChat();
  const { selectSession, sessions } = useChatSessionList();
  
  const handleSessionSwitch = (sessionId: string) => {
    // Messages will be automatically loaded from the new session
    selectSession(sessionId);
    // The hook will handle clearing and loading new messages
  };
  
  React.useEffect(() => {
    // Session changed
    if (currentSessionId) {
      console.log(`Switched to session: ${currentSessionId}`);
      console.log(`Agent: ${currentSession?.agent_config?.name}`);
      console.log(`Messages loaded: ${messages.length}`);
    }
  }, [currentSessionId, currentSession, messages.length]);
  
  return (
    <div>
      <select 
        value={currentSessionId || ''} 
        onChange={(e) => handleSessionSwitch(e.target.value)}
      >
        {sessions.map(session => (
          <option key={session.session_id} value={session.session_id}>
            {session.session_name || 'Untitled Session'}
          </option>
        ))}
      </select>
    </div>
  );
}
```

### Message Persistence

```typescript
function PersistentChat() {
  const { 
    messages, 
    currentSessionId,
    sendMessage,
    error
  } = useChat({ maxMessages: 200 });
  
  // Messages are automatically persisted server-side
  // The hook only maintains a client-side cache
  
  // Save draft to localStorage
  const [draft, setDraft] = React.useState('');
  
  React.useEffect(() => {
    if (currentSessionId) {
      const key = `draft_${currentSessionId}`;
      const saved = localStorage.getItem(key);
      if (saved) setDraft(saved);
    }
  }, [currentSessionId]);
  
  const handleSend = async () => {
    if (!draft.trim()) return;
    
    try {
      await sendMessage(draft);
      setDraft('');
      localStorage.removeItem(`draft_${currentSessionId}`);
    } catch (err) {
      console.error('Send failed:', err);
    }
  };
  
  const handleDraftChange = (text: string) => {
    setDraft(text);
    if (currentSessionId) {
      localStorage.setItem(`draft_${currentSessionId}`, text);
    }
  };
  
  return (
    <div>
      <input 
        value={draft}
        onChange={(e) => handleDraftChange(e.target.value)}
        placeholder="Type a message..."
      />
      <button onClick={handleSend}>Send</button>
      {error && <div className="error">{error}</div>}
    </div>
  );
}
```

## StrictMode Compatibility

The hook is fully compatible with React StrictMode and handles:

1. **Double Mounting**: Event listeners are properly cleaned up and re-attached
2. **Effect Cleanup**: All subscriptions are removed on unmount
3. **State Consistency**: Uses refs for tracking loading states to prevent race conditions
4. **Event Deduplication**: Streaming message IDs are tracked to prevent duplicates

### StrictMode Example

```typescript
// App.tsx
import React from 'react';
import { AgentCProvider } from '@agentc/realtime-react';

function App() {
  return (
    <React.StrictMode>
      <AgentCProvider config={config}>
        <ChatApp />
      </AgentCProvider>
    </React.StrictMode>
  );
}

// ChatApp.tsx
function ChatApp() {
  const chat = useChat();
  
  // Hook handles StrictMode double-mounting automatically
  // No special handling needed in component code
  
  React.useEffect(() => {
    // This effect might run twice in StrictMode
    // But the hook's internal state management prevents issues
    console.log('Chat mounted, messages:', chat.messages.length);
    
    return () => {
      // Cleanup is properly handled
      console.log('Chat unmounting');
    };
  }, [chat.messages.length]);
  
  return <div>{/* Chat UI */}</div>;
}
```

## Best Practices

### 1. Error Handling

Always wrap `sendMessage` in try-catch blocks:

```typescript
const handleSend = async (text: string) => {
  try {
    await sendMessage(text);
    // Success handling
  } catch (error) {
    // Show user-friendly error
    setUserError(error.message);
  }
};
```

### 2. Message Limits

Configure `maxMessages` based on your use case:

```typescript
// For long conversations
const chatLong = useChat({ maxMessages: 500 });

// For performance-critical UIs
const chatQuick = useChat({ maxMessages: 50 });

// For infinite scroll implementations
const chatInfinite = useChat({ maxMessages: 0 }); // No limit
```

### 3. Streaming UI

Differentiate streaming vs complete messages:

```typescript
const MessageComponent = ({ message, isStreaming }) => (
  <div className={`message ${isStreaming ? 'streaming' : 'complete'}`}>
    {message.content}
    {isStreaming && <LoadingDots />}
  </div>
);
```

### 4. Connection State

Always check connection before operations:

```typescript
const SafeChat = () => {
  const { sendMessage } = useChat();
  const { isConnected } = useConnection();
  
  const handleSend = async (text: string) => {
    if (!isConnected) {
      alert('Not connected to server');
      return;
    }
    
    await sendMessage(text);
  };
  
  return <ChatUI onSend={handleSend} disabled={!isConnected} />;
};
```

### 5. Performance Optimization

Memoize computed values:

```typescript
const OptimizedChat = () => {
  const { messages, getMessagesByRole } = useChat();
  
  const userMessageCount = React.useMemo(
    () => getMessagesByRole('user').length,
    [messages] // Only recompute when messages change
  );
  
  const assistantMessages = React.useMemo(
    () => getMessagesByRole('assistant'),
    [messages]
  );
  
  return <div>{/* Optimized rendering */}</div>;
};
```

### 6. Clean State Management

Clear messages appropriately:

```typescript
const SessionSwitcher = () => {
  const { clearMessages } = useChat();
  const { selectSession } = useChatSessionList();
  
  const handleNewSession = () => {
    // Clear messages is usually not needed
    // as session switch handles this automatically
    // Only use for special cases like reset
    clearMessages();
    selectSession(newSessionId);
  };
};
```

## Performance Considerations

1. **Message Limits**: Use `maxMessages` to prevent unbounded growth
2. **Streaming Updates**: Streaming messages update frequently; consider throttling UI updates
3. **Event Subscriptions**: The hook manages subscriptions efficiently with proper cleanup
4. **State Updates**: Multiple state updates are batched when possible
5. **Memory Management**: Old messages are automatically pruned based on `maxMessages`

## Common Pitfalls

1. **Not Handling Errors**: Always catch errors from `sendMessage`
2. **Ignoring Connection State**: Check if connected before sending
3. **Duplicate Streaming Messages**: Use the provided `streamingMessage` instead of manually tracking
4. **Memory Leaks**: Don't store message references outside the hook
5. **Stale Closures**: Use callback refs when accessing messages in event handlers
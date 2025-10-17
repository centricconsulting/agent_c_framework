# useChat Hook - currentSessionId

The `useChat` hook now exposes `currentSessionId` as part of its return value, making it easy to identify and highlight the active session in UI components.

## Updated Return Value

```typescript
interface UseChatReturn {
  // ... existing properties ...
  
  /** Current session ID */
  currentSessionId: string | null;
  
  // ... other properties ...
}
```

## Usage Example

```typescript
import { useChat, useChatSessionList } from '@agentc/realtime-react';

function ChatInterface() {
  const { currentSessionId, messages, sendMessage } = useChat();
  const { sessions, selectSession } = useChatSessionList();
  
  return (
    <div className="chat-container">
      {/* Session List */}
      <div className="session-list">
        {sessions.map(session => (
          <button
            key={session.session_id}
            onClick={() => selectSession(session.session_id)}
            className={
              session.session_id === currentSessionId
                ? 'session-item active'  // Highlight active session
                : 'session-item'
            }
          >
            {session.session_name || 'Untitled Session'}
          </button>
        ))}
      </div>
      
      {/* Chat Messages */}
      <div className="chat-messages">
        {currentSessionId ? (
          <div>
            <h3>Session: {currentSessionId}</h3>
            {messages.map((msg, idx) => (
              <div key={idx} className={`message ${msg.role}`}>
                {msg.content}
              </div>
            ))}
          </div>
        ) : (
          <div>No active session</div>
        )}
      </div>
    </div>
  );
}
```

## Synchronization with useChatSessionList

Both hooks will stay in sync automatically:

```typescript
function SessionAwareComponent() {
  const { currentSessionId: chatSessionId } = useChat();
  const { currentSessionId: listSessionId } = useChatSessionList();
  
  // These will always be the same
  console.log('From useChat:', chatSessionId);
  console.log('From useChatSessionList:', listSessionId);
  
  // Use either one based on which hook you're already using
  const isActive = (sessionId: string) => sessionId === chatSessionId;
}
```

## Reactive Updates

The `currentSessionId` updates automatically when:
- A new session is created
- A session is resumed
- The server sends a `chat_session_changed` event

```typescript
function SessionMonitor() {
  const { currentSessionId } = useChat();
  
  useEffect(() => {
    console.log('Active session changed:', currentSessionId);
  }, [currentSessionId]);
  
  return <div>Current Session: {currentSessionId || 'None'}</div>;
}
```

## Benefits

1. **UI Synchronization**: Easily highlight the active session in lists
2. **Conditional Rendering**: Show/hide UI elements based on session state
3. **Consistency**: Single source of truth for the active session ID
4. **Type Safety**: Fully typed as `string | null`
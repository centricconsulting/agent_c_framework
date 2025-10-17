# SessionManager API Reference

The `SessionManager` class manages chat sessions, maintains conversation history, and handles text accumulation for streaming responses.

## Import

```typescript
import { SessionManager } from '@agentc/realtime-core';
```

## Overview

The SessionManager tracks multiple chat sessions, stores message history, accumulates streaming text responses, and manages session metadata.

## Constructor

```typescript
constructor(config?: SessionManagerConfig)
```

Creates a new SessionManager instance.

### Parameters

- `config` (SessionManagerConfig, optional) - Configuration object

```typescript
interface SessionManagerConfig {
  maxSessions?: number;        // Maximum sessions to keep (default: 50)
  persistSessions?: boolean;   // Persist to storage (default: false)
  storageKey?: string;         // Storage key (default: 'agentc_sessions')
}
```

**Note:** SessionManager is typically created automatically by RealtimeClient

### Example

```typescript
// Setup authentication first
const authManager = new AuthManager({
  apiUrl: 'https://localhost:8000'
});

await authManager.login('username', 'password');

// SessionManager is created automatically
const client = new RealtimeClient({
  apiUrl: 'wss://localhost:8000/rt/ws',
  authManager
});

// Access the SessionManager instance
const sessionManager = client.getSessionManager();
```

## Properties

### currentSession

Gets the current active session.

```typescript
get currentSession(): ChatSession | null
```

**Returns:** Current session object or null

```typescript
interface ChatSession {
  session_id: string;
  session_name?: string;
  agent_key?: string;
  created_at: number;
  updated_at: number;
  messages: Message[];
  metadata?: Record<string, any>;
}

interface Message {
  role: 'user' | 'assistant' | 'system';
  content: string;
  timestamp: number;
  metadata?: Record<string, any>;
}
```

**Example:**
```typescript
const session = sessionManager.currentSession;
if (session) {
  console.log(`Session ID: ${session.session_id}`);
  console.log(`Messages: ${session.messages.length}`);
  console.log(`Created: ${new Date(session.created_at)}`);
}
```

### sessions

Gets all managed sessions.

```typescript
get sessions(): Map<string, ChatSession>
```

**Returns:** Map of session ID to ChatSession

**Example:**
```typescript
const allSessions = sessionManager.sessions;
console.log(`Total sessions: ${allSessions.size}`);

allSessions.forEach((session, id) => {
  console.log(`${id}: ${session.session_name || 'Unnamed'}`);
});
```

### accumulatedText

Gets the currently accumulated text from streaming.

```typescript
get accumulatedText(): string
```

**Returns:** Accumulated text string

**Example:**
```typescript
// During streaming response
const partialText = sessionManager.accumulatedText;
console.log('Partial response:', partialText);
```

## Session Management Methods

### getCurrentSession()

Gets the current active session.

```typescript
getCurrentSession(): ChatSession | null
```

**Returns:** Current session or null

**Example:**
```typescript
const session = sessionManager.getCurrentSession();
if (session) {
  displaySession(session);
}
```

### getCurrentSessionId()

Gets the ID of the current session.

```typescript
getCurrentSessionId(): string | null
```

**Returns:** Session ID or null

**Example:**
```typescript
const sessionId = sessionManager.getCurrentSessionId();
console.log(`Active session: ${sessionId}`);
```

### setCurrentSession()

Sets the current active session.

```typescript
setCurrentSession(session: ChatSession): void
```

**Parameters:**
- `session` (ChatSession) - Session to make active

**Note:** Usually called internally when server sends session changes

**Example:**
```typescript
const newSession: ChatSession = {
  session_id: 'session-123',
  created_at: Date.now(),
  updated_at: Date.now(),
  messages: []
};

sessionManager.setCurrentSession(newSession);
```

### createSession()

Creates a new chat session.

```typescript
createSession(sessionId: string, agentKey?: string): ChatSession
```

**Parameters:**
- `sessionId` (string) - Unique session identifier
- `agentKey` (string, optional) - Agent to use for session

**Returns:** Created ChatSession

**Example:**
```typescript
const session = sessionManager.createSession('session-456', 'support-agent');
console.log('Created session:', session.session_id);
```

### getSession()

Gets a specific session by ID.

```typescript
getSession(sessionId: string): ChatSession | undefined
```

**Parameters:**
- `sessionId` (string) - Session ID to retrieve

**Returns:** ChatSession or undefined

**Example:**
```typescript
const session = sessionManager.getSession('session-123');
if (session) {
  console.log(`Found session with ${session.messages.length} messages`);
}
```

### deleteSession()

Deletes a session.

```typescript
deleteSession(sessionId: string): boolean
```

**Parameters:**
- `sessionId` (string) - Session ID to delete

**Returns:** `true` if deleted, `false` if not found

**Example:**
```typescript
if (sessionManager.deleteSession('old-session')) {
  console.log('Session deleted');
}
```

### updateSessionName()

Updates a session's name.

```typescript
updateSessionName(sessionId: string, name: string): void
```

**Parameters:**
- `sessionId` (string) - Session ID to update
- `name` (string) - New session name

**Example:**
```typescript
sessionManager.updateSessionName('session-123', 'Customer Support - John');
```

### updateSessionMetadata()

Updates a session's metadata.

```typescript
updateSessionMetadata(sessionId: string, metadata: Record<string, any>): void
```

**Parameters:**
- `sessionId` (string) - Session ID to update
- `metadata` (object) - Metadata to merge

**Example:**
```typescript
sessionManager.updateSessionMetadata('session-123', {
  customerId: 'cust-456',
  priority: 'high',
  tags: ['billing', 'urgent']
});
```

### getAllSessions()

Gets all sessions as an array.

```typescript
getAllSessions(): ChatSession[]
```

**Returns:** Array of all sessions

**Example:**
```typescript
const sessions = sessionManager.getAllSessions();
sessions.sort((a, b) => b.updated_at - a.updated_at); // Most recent first

sessions.forEach(session => {
  console.log(`${session.session_name}: ${session.messages.length} messages`);
});
```

## Message Management Methods

### addMessage()

Adds a message to the current session.

```typescript
addMessage(
  role: 'user' | 'assistant' | 'system',
  content: string,
  metadata?: Record<string, any>
): void
```

**Parameters:**
- `role` (string) - Message role
- `content` (string) - Message content
- `metadata` (object, optional) - Additional metadata

**Example:**
```typescript
// Add user message
sessionManager.addMessage('user', 'What is the weather?');

// Add assistant message with metadata
sessionManager.addMessage('assistant', 'The weather is sunny.', {
  temperature: 72,
  location: 'San Francisco'
});
```

### addUserMessage()

Adds a user message to the current session.

```typescript
addUserMessage(content: string, metadata?: Record<string, any>): void
```

**Parameters:**
- `content` (string) - Message content
- `metadata` (object, optional) - Additional metadata

**Example:**
```typescript
sessionManager.addUserMessage('Hello, I need help');
```

### addAssistantMessage()

Adds an assistant message to the current session.

```typescript
addAssistantMessage(content: string, metadata?: Record<string, any>): void
```

**Parameters:**
- `content` (string) - Message content
- `metadata` (object, optional) - Additional metadata

**Example:**
```typescript
sessionManager.addAssistantMessage('Hello! How can I help you today?');
```

### getMessages()

Gets messages from a session.

```typescript
getMessages(sessionId?: string): Message[]
```

**Parameters:**
- `sessionId` (string, optional) - Session ID (defaults to current)

**Returns:** Array of messages

**Example:**
```typescript
// Get current session messages
const messages = sessionManager.getMessages();

// Get specific session messages
const oldMessages = sessionManager.getMessages('session-123');

// Display conversation
messages.forEach(msg => {
  console.log(`${msg.role}: ${msg.content}`);
});
```

### clearMessages()

Clears messages from a session.

```typescript
clearMessages(sessionId?: string): void
```

**Parameters:**
- `sessionId` (string, optional) - Session ID (defaults to current)

**Example:**
```typescript
// Clear current session
sessionManager.clearMessages();

// Clear specific session
sessionManager.clearMessages('session-123');
```

## Text Accumulation Methods

### handleTextDelta()

Handles streaming text chunks.

```typescript
handleTextDelta(content: string): void
```

**Parameters:**
- `content` (string) - Text chunk to accumulate

**Note:** Usually called internally when receiving text_delta events

**Example:**
```typescript
// This is typically called internally
client.on('text_delta', (event) => {
  sessionManager.handleTextDelta(event.content);
});
```

### handleTextDone()

Finalizes accumulated text and adds to message history.

```typescript
handleTextDone(): string
```

**Returns:** The complete accumulated text

**Note:** Usually called internally when response completes

**Example:**
```typescript
// This is typically called internally
client.on('completion', (event) => {
  if (!event.running) {
    const fullText = sessionManager.handleTextDone();
    console.log('Complete response:', fullText);
  }
});
```

### resetAccumulator()

Resets the text accumulator.

```typescript
resetAccumulator(): void
```

**Example:**
```typescript
// Reset when starting new conversation
sessionManager.resetAccumulator();
```

### getAccumulatedText()

Gets the current accumulated text.

```typescript
getAccumulatedText(): string
```

**Returns:** Current accumulated text

**Example:**
```typescript
// Monitor streaming progress
setInterval(() => {
  const partial = sessionManager.getAccumulatedText();
  updateUI(partial);
}, 100);
```

## Session Statistics

### getSessionStats()

Gets statistics for a session.

```typescript
getSessionStats(sessionId?: string): SessionStats
```

**Parameters:**
- `sessionId` (string, optional) - Session ID (defaults to current)

**Returns:** Session statistics

```typescript
interface SessionStats {
  messageCount: number;
  userMessageCount: number;
  assistantMessageCount: number;
  totalCharacters: number;
  averageMessageLength: number;
  sessionDuration: number;
  lastMessageTime: number;
}
```

**Example:**
```typescript
const stats = sessionManager.getSessionStats();
console.log(`Messages: ${stats.messageCount}`);
console.log(`User messages: ${stats.userMessageCount}`);
console.log(`Assistant messages: ${stats.assistantMessageCount}`);
console.log(`Total characters: ${stats.totalCharacters}`);
console.log(`Average length: ${stats.averageMessageLength}`);
console.log(`Duration: ${stats.sessionDuration}ms`);
```

## Persistence Methods

### saveToStorage()

Saves sessions to local storage.

```typescript
saveToStorage(): void
```

**Note:** Only works if `persistSessions` is enabled

**Example:**
```typescript
// Save current sessions
sessionManager.saveToStorage();
```

### loadFromStorage()

Loads sessions from local storage.

```typescript
loadFromStorage(): void
```

**Note:** Only works if `persistSessions` is enabled

**Example:**
```typescript
// Restore previous sessions
sessionManager.loadFromStorage();
```

### exportSessions()

Exports all sessions as JSON.

```typescript
exportSessions(): string
```

**Returns:** JSON string of all sessions

**Example:**
```typescript
const json = sessionManager.exportSessions();
// Save to file or send to server
saveToFile('sessions.json', json);
```

### importSessions()

Imports sessions from JSON.

```typescript
importSessions(json: string): void
```

**Parameters:**
- `json` (string) - JSON string of sessions

**Example:**
```typescript
const json = loadFromFile('sessions.json');
sessionManager.importSessions(json);
```

## Event Handling

SessionManager extends EventEmitter:

### Events

- `session:created` - New session created
- `session:changed` - Active session changed
- `session:updated` - Session updated
- `session:deleted` - Session deleted
- `message:added` - Message added
- `text:accumulated` - Text chunk accumulated
- `text:completed` - Text accumulation completed

### Example Event Handling

```typescript
sessionManager.on('session:created', (session: ChatSession) => {
  console.log(`New session: ${session.session_id}`);
});

sessionManager.on('session:changed', (data: SessionChangeEvent) => {
  console.log(`Session changed from ${data.previous} to ${data.current}`);
});

sessionManager.on('message:added', (message: Message) => {
  console.log(`${message.role}: ${message.content}`);
  updateChatUI(message);
});

sessionManager.on('text:accumulated', (text: string) => {
  // Update UI with streaming text
  updateStreamingUI(text);
});

sessionManager.on('text:completed', (text: string) => {
  // Finalize UI with complete text
  finalizeStreamingUI(text);
});
```

## Complete Example

```typescript
import { RealtimeClient, AuthManager } from '@agentc/realtime-core';

async function sessionManagementExample() {
  // Setup authentication
  const authManager = new AuthManager({
    apiUrl: 'https://localhost:8000'
  });
  
  await authManager.login('username', 'password');
  
  const client = new RealtimeClient({
    apiUrl: 'wss://localhost:8000/rt/ws',
    authManager
  });
  
  const sessionManager = client.getSessionManager();
  
  if (!sessionManager) {
    console.error('SessionManager not initialized');
    return;
  }
  
  // Subscribe to session events
  sessionManager.on('session:created', (session) => {
    console.log(`✨ New session created: ${session.session_id}`);
  });
  
  sessionManager.on('session:changed', (event) => {
    console.log(`🔄 Session changed to: ${event.current}`);
  });
  
  sessionManager.on('message:added', (message) => {
    const icon = message.role === 'user' ? '👤' : '🤖';
    console.log(`${icon} ${message.content}`);
  });
  
  sessionManager.on('text:accumulated', (text) => {
    // Show streaming text in UI
    process.stdout.write(`\r${text}`);
  });
  
  sessionManager.on('text:completed', (text) => {
    console.log('\n✅ Response complete');
  });
  
  // Connect to service
  await client.connect();
  
  // Create a new session
  client.newChatSession();
  
  // Wait for session to be created
  await new Promise(resolve => setTimeout(resolve, 1000));
  
  // Get current session
  const session = sessionManager.getCurrentSession();
  if (session) {
    console.log(`\n📝 Session Details:`);
    console.log(`ID: ${session.session_id}`);
    console.log(`Created: ${new Date(session.created_at).toLocaleString()}`);
    
    // Set session name
    sessionManager.updateSessionName(session.session_id, 'Demo Session');
    
    // Add metadata
    sessionManager.updateSessionMetadata(session.session_id, {
      user: 'demo-user',
      purpose: 'testing',
      tags: ['demo', 'example']
    });
  }
  
  // Send some messages
  const questions = [
    'What is TypeScript?',
    'How does it differ from JavaScript?',
    'What are the main benefits?'
  ];
  
  for (const question of questions) {
    console.log(`\n❓ Asking: ${question}`);
    
    // Add user message
    sessionManager.addUserMessage(question);
    
    // Send to server
    client.sendText(question);
    
    // Wait for response
    await new Promise(resolve => setTimeout(resolve, 3000));
    
    // The response will be accumulated automatically
    // and added as assistant message when complete
  }
  
  // Get session statistics
  const stats = sessionManager.getSessionStats();
  console.log(`\n📊 Session Statistics:`);
  console.log(`Total messages: ${stats.messageCount}`);
  console.log(`User messages: ${stats.userMessageCount}`);
  console.log(`Assistant messages: ${stats.assistantMessageCount}`);
  console.log(`Total characters: ${stats.totalCharacters}`);
  console.log(`Average message length: ${Math.round(stats.averageMessageLength)}`);
  
  // Get conversation history
  const messages = sessionManager.getMessages();
  console.log(`\n💬 Conversation History:`);
  messages.forEach((msg, i) => {
    const icon = msg.role === 'user' ? '👤' : '🤖';
    console.log(`${i + 1}. ${icon} ${msg.content.substring(0, 50)}...`);
  });
  
  // Export sessions
  const exported = sessionManager.exportSessions();
  console.log(`\n📦 Exported ${exported.length} characters of session data`);
  
  // Create another session
  console.log('\n🔄 Creating new session...');
  client.newChatSession();
  
  await new Promise(resolve => setTimeout(resolve, 1000));
  
  // List all sessions
  const allSessions = sessionManager.getAllSessions();
  console.log(`\n📚 All Sessions (${allSessions.length}):`);
  allSessions.forEach(sess => {
    console.log(`  - ${sess.session_id}: ${sess.session_name || 'Unnamed'}`);
    console.log(`    Messages: ${sess.messages.length}`);
    console.log(`    Created: ${new Date(sess.created_at).toLocaleString()}`);
  });
  
  // Switch back to first session
  const firstSession = allSessions[0];
  if (firstSession) {
    console.log(`\n🔙 Resuming session: ${firstSession.session_id}`);
    client.resumeChatSession(firstSession.session_id);
    
    // Continue conversation
    sessionManager.addUserMessage('Can you summarize our conversation?');
    client.sendText('Can you summarize our conversation?');
  }
  
  // Clean up old sessions
  const maxAge = 24 * 60 * 60 * 1000; // 24 hours
  const now = Date.now();
  
  allSessions.forEach(sess => {
    if (now - sess.updated_at > maxAge) {
      console.log(`🗑️ Deleting old session: ${sess.session_id}`);
      sessionManager.deleteSession(sess.session_id);
    }
  });
  
  // Save to storage if persistence is enabled
  if (sessionManager.sessions.size > 0) {
    sessionManager.saveToStorage();
    console.log('\n💾 Sessions saved to storage');
  }
  
  // Destroy when done
  sessionManager.destroy();
}

sessionManagementExample().catch(console.error);
```

## Best Practices

1. **Reset accumulator when switching sessions:**
```typescript
client.on('chat_session_changed', () => {
  sessionManager.resetAccumulator();
});
```

2. **Add user messages before sending:**
```typescript
function sendMessage(text: string) {
  sessionManager.addUserMessage(text);
  client.sendText(text);
}
```

3. **Handle streaming text in UI:**
```typescript
sessionManager.on('text:accumulated', (text) => {
  // Update UI with partial text
  streamingDiv.textContent = text;
});

sessionManager.on('text:completed', (text) => {
  // Move to message history
  addToHistory({ role: 'assistant', content: text });
  streamingDiv.textContent = '';
});
```

4. **Limit session count:**
```typescript
const sessions = sessionManager.getAllSessions();
if (sessions.length > 50) {
  // Delete oldest sessions
  sessions
    .sort((a, b) => a.updated_at - b.updated_at)
    .slice(0, sessions.length - 50)
    .forEach(s => sessionManager.deleteSession(s.session_id));
}
```

5. **Export important sessions:**
```typescript
// Backup important conversations
function backupSession(sessionId: string) {
  const session = sessionManager.getSession(sessionId);
  if (session) {
    const json = JSON.stringify(session);
    localStorage.setItem(`backup_${sessionId}`, json);
  }
}
```

## TypeScript Types

```typescript
import {
  SessionManager,
  SessionManagerConfig,
  ChatSession,
  Message,
  SessionStats,
  SessionChangeEvent
} from '@agentc/realtime-core';
```

All methods and properties are fully typed for TypeScript applications.
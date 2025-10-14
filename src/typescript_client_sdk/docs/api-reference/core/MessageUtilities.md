# Message Utilities API Reference

The message utilities provide essential functions for processing, normalizing, and converting messages between server and client formats in the Agent C Realtime SDK.

## Overview

The message utilities include two main modules:
- **messageUtils** - Message normalization and validation
- **message-converter** - Server/client format conversion

These utilities ensure consistent message handling across the SDK, supporting various content formats including text, structured content blocks, and multi-part messages.

## messageUtils Module

### Functions

#### `normalizeMessageContent(content: any): string`

Normalize message content to ensure it's always a string.

```typescript
import { normalizeMessageContent } from '@agentc/realtime-core/utils';

// String content - returns as-is
const text = normalizeMessageContent('Hello world');
// Returns: 'Hello world'

// Null/undefined - returns empty string
const empty = normalizeMessageContent(null);
// Returns: ''

// Object with text property
const obj = normalizeMessageContent({ text: 'Hello' });
// Returns: 'Hello'

// Array of text segments
const array = normalizeMessageContent([
  { text: 'Part 1' },
  { text: 'Part 2' }
]);
// Returns: 'Part 1Part 2'

// Complex object - stringified
const complex = normalizeMessageContent({ data: [1, 2, 3] });
// Returns: '{\n  "data": [1, 2, 3]\n}'
```

#### `normalizeMessage(message: any): Message`

Normalize a message to ensure all fields are valid.

```typescript
import { normalizeMessage } from '@agentc/realtime-core/utils';

// Partial message
const message = normalizeMessage({
  role: 'user',
  content: 'Hello'
});
// Returns: {
//   role: 'user',
//   content: 'Hello',
//   timestamp: '2024-01-01T00:00:00.000Z',
//   format: 'text'
// }

// Invalid message
const fallback = normalizeMessage(null);
// Returns: {
//   role: 'system',
//   content: '',
//   timestamp: '2024-01-01T00:00:00.000Z',
//   format: 'text'
// }
```

#### `normalizeMessages(messages: any[]): Message[]`

Normalize an array of messages.

```typescript
import { normalizeMessages } from '@agentc/realtime-core/utils';

const messages = normalizeMessages([
  { role: 'user', content: 'Hello' },
  { role: 'assistant', content: { text: 'Hi there' } },
  null  // Invalid message
]);
// Returns array of normalized Message objects
```

#### `isValidMessage(message: any): message is Message`

Type guard to check if a message has valid structure.

```typescript
import { isValidMessage } from '@agentc/realtime-core/utils';

const message = { 
  role: 'user', 
  content: 'Hello',
  timestamp: '2024-01-01T00:00:00.000Z',
  format: 'text'
};

if (isValidMessage(message)) {
  // TypeScript knows message is valid Message type
  console.log(message.content); // string
}
```

## message-converter Module

### Functions

#### `convertMessageParamContent(content: string | ContentBlockParam[]): MessageContent`

Convert server MessageParam content to client MessageContent format.

```typescript
import { convertMessageParamContent } from '@agentc/realtime-core/utils';

// String content
const text = convertMessageParamContent('Hello world');
// Returns: 'Hello world'

// Content blocks
const blocks = convertMessageParamContent([
  { type: 'text', text: 'Hello' },
  { type: 'image', source: { type: 'base64', data: '...' } }
]);
// Returns: [
//   { type: 'text', text: 'Hello' },
//   { type: 'image', source: { type: 'base64', data: '...' } }
// ]

// Filters extra fields
const filtered = convertMessageParamContent([
  { 
    type: 'text', 
    text: 'Hello',
    cache_control: { max_age: 300 },  // Removed
    citations: []                      // Removed
  }
]);
// Returns: [{ type: 'text', text: 'Hello' }]
```

#### `convertMessageParamToMessage(param: MessageParam): Message`

Convert a server MessageParam to client Message format.

```typescript
import { convertMessageParamToMessage } from '@agentc/realtime-core/utils';

const param = {
  role: 'assistant',
  content: [
    { type: 'text', text: 'Hello!' }
  ]
};

const message = convertMessageParamToMessage(param);
// Returns: {
//   role: 'assistant',
//   content: [{ type: 'text', text: 'Hello!' }],
//   timestamp: '2024-01-01T00:00:00.000Z',
//   format: 'text'
// }
```

#### `convertMessageParamsToMessages(params: MessageParam[]): Message[]`

Convert an array of MessageParams to Messages.

```typescript
import { convertMessageParamsToMessages } from '@agentc/realtime-core/utils';

const params = [
  { role: 'user', content: 'Question?' },
  { role: 'assistant', content: [
    { type: 'text', text: 'Answer.' }
  ]}
];

const messages = convertMessageParamsToMessages(params);
// Returns array of converted Message objects
```

#### `ensureMessageFormat(message: any): Message`

Ensure a message has the correct format for UI rendering.

```typescript
import { ensureMessageFormat } from '@agentc/realtime-core/utils';

// Already valid message - returns as-is
const valid = ensureMessageFormat({
  role: 'user',
  content: 'Hello',
  timestamp: '2024-01-01T00:00:00.000Z',
  format: 'text'
});

// Message with content blocks - normalizes
const withBlocks = ensureMessageFormat({
  role: 'assistant',
  content: [
    { type: 'text', text: 'Hello', citations: [] }
  ]
});
// Citations removed, content normalized

// Invalid format - creates error message
const invalid = ensureMessageFormat('not a message');
// Returns: {
//   role: 'system',
//   content: '[Invalid message format]',
//   timestamp: '2024-01-01T00:00:00.000Z',
//   format: 'text'
// }
```

#### `ensureMessagesFormat(messages: any[]): Message[]`

Ensure an array of messages has the correct format.

```typescript
import { ensureMessagesFormat } from '@agentc/realtime-core/utils';

const messages = ensureMessagesFormat([
  { role: 'user', content: 'Hello' },
  { role: 'assistant', content: [
    { type: 'text', text: 'Hi', cache_control: {} }
  ]},
  null  // Invalid
]);
// Returns normalized array with invalid messages converted to error messages
```

## Types

### Message

```typescript
interface Message {
  role: 'user' | 'assistant' | 'system' | 'assistant (thought)';
  content: MessageContent;
  timestamp: string;
  format: 'text' | 'audio';
}
```

### MessageContent

```typescript
type MessageContent = string | ContentPart[] | null;
```

### ContentPart

```typescript
type ContentPart = 
  | { type: 'text'; text: string }
  | { type: 'image'; source: ImageSource }
  | { type: 'tool_use'; id: string; name: string; input: any }
  | { type: 'tool_result'; tool_use_id: string; content: any; is_error?: boolean };
```

### MessageParam (Server Format)

```typescript
interface MessageParam {
  role: 'user' | 'assistant' | 'system';
  content: string | ContentBlockParam[];
}
```

### ContentBlockParam (Server Format)

```typescript
type ContentBlockParam = 
  | { type: 'text'; text: string; cache_control?: any; citations?: any[] }
  | { type: 'image'; source: ImageSource }
  | { type: 'tool_use'; id: string; name: string; input: any }
  | { type: 'tool_result'; tool_use_id: string; content: any }
  | { type: 'thinking'; thinking: string }
  // ... other server-specific types
```

## Usage Examples

### Message Normalization

```typescript
import { normalizeMessage, normalizeMessages } from '@agentc/realtime-core/utils';

// Normalize user input
function processUserMessage(input: any): Message {
  const normalized = normalizeMessage({
    role: 'user',
    content: input,
    timestamp: new Date().toISOString()
  });
  
  return normalized;
}

// Normalize chat history
function loadChatHistory(data: any[]): Message[] {
  return normalizeMessages(data);
}
```

### Server Response Conversion

```typescript
import { convertMessageParamToMessage } from '@agentc/realtime-core/utils';

// Handle server message
websocket.on('message', (data) => {
  const event = JSON.parse(data);
  
  if (event.type === 'message') {
    const message = convertMessageParamToMessage(event.content);
    addToChat(message);
  }
});
```

### Content Block Processing

```typescript
import { convertMessageParamContent } from '@agentc/realtime-core/utils';

function processContentBlocks(blocks: ContentBlockParam[]) {
  const content = convertMessageParamContent(blocks);
  
  if (typeof content === 'string') {
    // Simple text content
    return { type: 'text', value: content };
  }
  
  if (Array.isArray(content)) {
    // Multi-part content
    return content.map(part => {
      switch (part.type) {
        case 'text':
          return { type: 'text', value: part.text };
        case 'image':
          return { type: 'image', source: part.source };
        case 'tool_use':
          return { type: 'tool', name: part.name, input: part.input };
        default:
          return { type: 'unknown', data: part };
      }
    });
  }
  
  return null;
}
```

### Message Validation

```typescript
import { isValidMessage, normalizeMessage } from '@agentc/realtime-core/utils';

function validateAndStore(message: any): boolean {
  // First check if valid
  if (isValidMessage(message)) {
    storage.save(message);
    return true;
  }
  
  // Try to normalize
  const normalized = normalizeMessage(message);
  if (normalized.content !== '') {
    storage.save(normalized);
    return true;
  }
  
  return false;
}
```

### React Component Integration

```typescript
import React from 'react';
import { ensureMessageFormat } from '@agentc/realtime-core/utils';

function ChatMessage({ message: rawMessage }) {
  // Ensure proper format for rendering
  const message = React.useMemo(
    () => ensureMessageFormat(rawMessage),
    [rawMessage]
  );
  
  // Render based on content type
  if (typeof message.content === 'string') {
    return <div className="text-message">{message.content}</div>;
  }
  
  if (Array.isArray(message.content)) {
    return (
      <div className="multi-part-message">
        {message.content.map((part, i) => (
          <MessagePart key={i} part={part} />
        ))}
      </div>
    );
  }
  
  return null;
}
```

### Handling Complex Content

```typescript
import { normalizeMessageContent } from '@agentc/realtime-core/utils';

// Handle various content formats
function extractText(content: any): string {
  const text = normalizeMessageContent(content);
  
  // Further processing if needed
  if (text.startsWith('{') && text.endsWith('}')) {
    // Might be JSON - try to format
    try {
      const parsed = JSON.parse(text);
      return JSON.stringify(parsed, null, 2);
    } catch {
      // Not valid JSON, return as-is
    }
  }
  
  return text;
}
```

## Integration Patterns

### With SessionManager

```typescript
import { SessionManager } from '@agentc/realtime-core';
import { ensureMessagesFormat } from '@agentc/realtime-core/utils';

class ChatSession {
  private sessionManager: SessionManager;
  
  loadHistory() {
    const rawHistory = this.sessionManager.getMessages();
    return ensureMessagesFormat(rawHistory);
  }
  
  addMessage(message: any) {
    const normalized = ensureMessageFormat(message);
    this.sessionManager.addMessage(normalized);
  }
}
```

### With WebSocket Events

```typescript
import { convertMessageParamToMessage } from '@agentc/realtime-core/utils';

class MessageHandler {
  handleServerMessage(event: any) {
    switch (event.type) {
      case 'response.content':
        // Server sends MessageParam format
        const message = convertMessageParamToMessage({
          role: event.role || 'assistant',
          content: event.content
        });
        this.displayMessage(message);
        break;
        
      case 'response.text.delta':
        // Incremental text update
        this.appendText(event.text_delta);
        break;
    }
  }
}
```

## Best Practices

### 1. Always Normalize External Data

```typescript
// Good - normalize data from external sources
const message = normalizeMessage(apiResponse.message);

// Bad - trust external data structure
const message = apiResponse.message;  // May be invalid
```

### 2. Use Type Guards

```typescript
// Good - verify structure before use
if (isValidMessage(data)) {
  processMessage(data);
} else {
  handleInvalidMessage(data);
}
```

### 3. Handle Content Variations

```typescript
// Good - handle all content types
const text = typeof message.content === 'string' 
  ? message.content
  : Array.isArray(message.content)
    ? message.content.map(p => p.text).join('')
    : '';
```

### 4. Preserve Original Format When Possible

```typescript
// Good - only convert when necessary
const formatted = ensureMessageFormat(message);

// Bad - always stringify complex content
const text = JSON.stringify(message.content);
```

### 5. Log Normalization Issues

```typescript
import { Logger } from '@agentc/realtime-core';

function safeNormalize(message: any): Message {
  const normalized = normalizeMessage(message);
  
  if (normalized.content === '') {
    Logger.warn('[MessageUtils] Empty content after normalization', message);
  }
  
  return normalized;
}
```

## Performance Considerations

- Normalization functions are lightweight and synchronous
- Content conversion preserves references when possible
- Array processing is optimized for common cases
- Type guards use early returns for efficiency

## Error Handling

The utilities are designed to be resilient:

```typescript
// Never throws - returns safe defaults
const message = normalizeMessage(null);
// Returns: { role: 'system', content: '', ... }

// Handles circular references
const circular = { content: {} };
circular.content = circular;
const safe = normalizeMessageContent(circular);
// Returns: '[Complex Object]'
```

## Related Documentation

- [SessionManager](./SessionManager.md) - Message history management
- [RealtimeClient](./RealtimeClient.md) - Message event handling
- [Types Documentation](./types/) - Complete type definitions
- [Chat UI Components](./components/Chat.md) - Message rendering
# MessageBuilder API Documentation

## Overview

The `MessageBuilder` class is a critical component in the Agent C Realtime SDK's event streaming architecture that handles the accumulation of streaming text deltas and construction of complete messages. It manages the lifecycle of partial messages during real-time streaming, supporting assistant messages, thoughts, and user messages with proper state management and metadata tracking.

### Purpose

MessageBuilder serves as a stateful accumulator that:
- Collects incremental text deltas from streaming events
- Maintains message state during the streaming process
- Finalizes messages with complete metadata when streaming ends
- Handles different message types (assistant, thought, user)
- Generates unique message IDs and timestamps
- Supports interrupted streams and error handling

### Architecture Context

MessageBuilder is used by the `EventStreamProcessor` as the primary mechanism for building messages from streaming events. It works in conjunction with:
- **EventStreamProcessor**: Orchestrates event handling and delegates text accumulation to MessageBuilder
- **SessionManager**: Receives completed messages and streaming updates
- **ToolCallManager**: Provides tool call metadata for message finalization

## Class Definition

```typescript
export class MessageBuilder {
  constructor()
  
  // Core message building methods
  startMessage(type: MessageType): void
  appendText(delta: string): void
  finalize(metadata?: MessageMetadata): EnhancedMessage
  
  // State inspection methods
  getCurrentMessage(): Partial<EnhancedMessage> | null
  hasCurrentMessage(): boolean
  getCurrentMessageType(): MessageType | null
  getContentLength(): number
  isStreaming(): boolean
  
  // State management
  reset(): void
}
```

## Types and Interfaces

### MessageType
```typescript
export type MessageType = 'assistant' | 'thought' | 'user';
```
Defines the type of message being built. Thoughts are special assistant messages with collapsible UI behavior.

### EnhancedMessage
```typescript
export interface EnhancedMessage extends Message {
  id: string;
  type: 'message' | 'thought' | 'media' | 'notification';
  status: 'streaming' | 'complete' | 'error';
  contentType?: 'text' | 'html' | 'svg' | 'image' | 'unknown';
  metadata?: MessageMetadata;
  toolCalls?: ToolCall[];
  toolResults?: ToolResult[];
  isCollapsed?: boolean; // For thoughts
}
```
Extended message structure with streaming status and UI hints.

### MessageMetadata
```typescript
export interface MessageMetadata {
  inputTokens?: number;
  outputTokens?: number;
  stopReason?: StopReason;
  toolCalls?: ToolCall[];
  toolResults?: ToolResult[];
  sentByClass?: string;
  sentByFunction?: string;
  timestamp?: string;
}
```
Metadata attached to completed messages, including token counts and completion reasons.

## Method Details

### startMessage(type: MessageType): void

Initializes a new message for streaming accumulation. If a message is already in progress, a warning is logged but the new message starts anyway.

**Parameters:**
- `type`: The type of message to build ('assistant', 'thought', or 'user')

**Behavior:**
- Creates a new message with generated ID and timestamp
- Sets initial status to 'streaming'
- Resets content accumulator
- For thoughts, the message type is set to 'thought' for special UI handling

**Example:**
```typescript
const builder = new MessageBuilder();
builder.startMessage('assistant');
// Message is now ready to receive text deltas
```

### appendText(delta: string): void

Appends a text delta to the current message being built. If no message is active, automatically starts an assistant message.

**Parameters:**
- `delta`: The text fragment to append

**Behavior:**
- Concatenates delta to internal content buffer
- Updates the current message's content
- Auto-starts assistant message if none exists

**Example:**
```typescript
builder.appendText('Hello');
builder.appendText(' world');
// Current message content: "Hello world"
```

### finalize(metadata?: MessageMetadata): EnhancedMessage

Completes the current message and returns the finalized version with metadata.

**Parameters:**
- `metadata`: Optional metadata including tokens, tool calls, and stop reason

**Returns:**
- Complete `EnhancedMessage` with status set to 'complete'

**Behavior:**
- Throws error if no message to finalize
- Sets status to 'complete'
- Attaches metadata and tool information
- Thoughts are marked as collapsed by default
- Preserves all accumulated content

**Example:**
```typescript
const message = builder.finalize({
  inputTokens: 150,
  outputTokens: 200,
  stopReason: 'stop',
  toolCalls: [/* tool calls */]
});
// Message is complete with all metadata
```

### getCurrentMessage(): Partial<EnhancedMessage> | null

Returns the current message being built, including accumulated content. Returns null if no message is active.

**Returns:**
- Partial message with current state, or null

**Example:**
```typescript
const current = builder.getCurrentMessage();
if (current) {
  console.log(`Streaming: ${current.content}`);
  console.log(`Status: ${current.status}`); // 'streaming'
}
```

### hasCurrentMessage(): boolean

Checks if there's an active message being built.

**Returns:**
- `true` if a message is in progress, `false` otherwise

**Example:**
```typescript
if (!builder.hasCurrentMessage()) {
  builder.startMessage('assistant');
}
```

### getCurrentMessageType(): MessageType | null

Returns the type of the current message being built.

**Returns:**
- Message type ('assistant', 'thought', 'user') or null

**Example:**
```typescript
const type = builder.getCurrentMessageType();
if (type === 'thought') {
  // Handle thought-specific logic
}
```

### getContentLength(): number

Returns the length of accumulated content.

**Returns:**
- Number of characters in the content buffer

**Example:**
```typescript
const length = builder.getContentLength();
if (length > 10000) {
  // Consider truncation or warning
}
```

### isStreaming(): boolean

Checks if the builder is actively streaming a message.

**Returns:**
- `true` if message exists with 'streaming' status

**Example:**
```typescript
if (builder.isStreaming()) {
  // Show streaming indicator in UI
}
```

### reset(): void

Clears all state and prepares for a new message.

**Behavior:**
- Clears current message
- Resets content buffer
- Resets message type to 'assistant'

**Example:**
```typescript
builder.reset();
// Builder is now ready for a new message
```

## Integration with EventStreamProcessor

The MessageBuilder is tightly integrated with the EventStreamProcessor for handling streaming events:

### Text Delta Handling
```typescript
// In EventStreamProcessor.handleTextDelta()
if (!this.messageBuilder.hasCurrentMessage()) {
  this.messageBuilder.startMessage('assistant');
}
this.messageBuilder.appendText(event.content);

const currentMessage = this.messageBuilder.getCurrentMessage();
if (currentMessage) {
  this.sessionManager.emit('message-streaming', {
    sessionId: event.session_id,
    message: currentMessage
  });
}
```

### Thought Delta Handling
```typescript
// In EventStreamProcessor.handleThoughtDelta()
if (!this.messageBuilder.hasCurrentMessage() || 
    this.messageBuilder.getCurrentMessageType() !== 'thought') {
  // Finalize any in-progress message first
  if (this.messageBuilder.hasCurrentMessage()) {
    const message = this.messageBuilder.finalize();
    this.sessionManager.emit('message-complete', { message });
  }
  this.messageBuilder.startMessage('thought');
}
this.messageBuilder.appendText(event.content);
```

### Completion Handling
```typescript
// In EventStreamProcessor.handleCompletion()
if (!event.running && this.messageBuilder.hasCurrentMessage()) {
  const message = this.messageBuilder.finalize({
    inputTokens: event.input_tokens,
    outputTokens: event.output_tokens,
    stopReason: event.stop_reason,
    toolCalls: toolCalls,
    toolResults: toolResults
  });
  
  this.sessionManager.emit('message-complete', {
    sessionId: event.session_id,
    message
  });
  
  this.messageBuilder.reset();
}
```

## Error Handling

### Interrupted Streams

When a stream is interrupted or cancelled, MessageBuilder supports graceful handling:

```typescript
// Handle cancelled event
if (this.messageBuilder.hasCurrentMessage()) {
  const message = this.messageBuilder.finalize({
    stopReason: 'cancelled'
  });
  // Message is marked as complete with 'cancelled' stop reason
}
```

### Malformed Events

MessageBuilder has defensive programming for malformed events:
- Auto-starts assistant message if text delta arrives without active message
- Logs warnings for unexpected state transitions
- Maintains consistent state even with out-of-order events

## Usage Patterns

### Basic Streaming Flow
```typescript
const builder = new MessageBuilder();

// 1. Start message when first delta arrives
builder.startMessage('assistant');

// 2. Accumulate text deltas
for (const delta of textDeltas) {
  builder.appendText(delta);
  // Emit streaming update
  const current = builder.getCurrentMessage();
  emit('message-streaming', current);
}

// 3. Finalize on completion
const finalMessage = builder.finalize({
  outputTokens: 150,
  stopReason: 'stop'
});
emit('message-complete', finalMessage);

// 4. Reset for next message
builder.reset();
```

### Thought Tracking Pattern
```typescript
// Thoughts require special handling
if (eventType === 'thought_delta') {
  // Ensure we're building a thought message
  if (builder.getCurrentMessageType() !== 'thought') {
    // Finalize any current message first
    if (builder.hasCurrentMessage()) {
      const msg = builder.finalize();
      handleMessage(msg);
    }
    builder.startMessage('thought');
  }
  builder.appendText(thoughtDelta);
}
```

### Multi-Message Conversation
```typescript
class ConversationHandler {
  private builder = new MessageBuilder();
  
  handleUserMessage(content: string) {
    this.builder.startMessage('user');
    this.builder.appendText(content);
    const userMessage = this.builder.finalize();
    this.addToHistory(userMessage);
    this.builder.reset();
  }
  
  handleAssistantStream(deltas: string[]) {
    this.builder.startMessage('assistant');
    for (const delta of deltas) {
      this.builder.appendText(delta);
      this.updateUI(this.builder.getCurrentMessage());
    }
    const assistantMessage = this.builder.finalize();
    this.addToHistory(assistantMessage);
    this.builder.reset();
  }
}
```

## Best Practices

### 1. Always Reset After Finalization
```typescript
const message = builder.finalize(metadata);
processMessage(message);
builder.reset(); // Always reset for clean state
```

### 2. Check State Before Operations
```typescript
// Before appending text
if (!builder.hasCurrentMessage()) {
  builder.startMessage('assistant');
}

// Before finalizing
if (builder.hasCurrentMessage()) {
  const message = builder.finalize();
}
```

### 3. Handle Interruptions Gracefully
```typescript
function handleInterruption() {
  if (builder.isStreaming()) {
    // Finalize with appropriate metadata
    const message = builder.finalize({
      stopReason: 'cancelled'
    });
    // Mark message as interrupted in UI
    message.status = 'error';
  }
  builder.reset();
}
```

### 4. Monitor Content Length
```typescript
// Prevent unbounded growth
if (builder.getContentLength() > MAX_MESSAGE_LENGTH) {
  logger.warn('Message exceeding maximum length');
  // Consider truncation or early finalization
}
```

### 5. Preserve Message Type Context
```typescript
// When switching message types mid-stream
const currentType = builder.getCurrentMessageType();
if (newType !== currentType && builder.hasCurrentMessage()) {
  // Finalize current before starting new type
  const completed = builder.finalize();
  handleCompleted(completed);
  builder.reset();
}
builder.startMessage(newType);
```

### 6. Emit Streaming Updates Efficiently
```typescript
// Throttle streaming updates for performance
let lastEmit = 0;
function handleDelta(delta: string) {
  builder.appendText(delta);
  
  const now = Date.now();
  if (now - lastEmit > 100) { // Throttle to 10Hz
    emit('streaming', builder.getCurrentMessage());
    lastEmit = now;
  }
}
```

## State Management Considerations

### Message ID Generation
MessageBuilder generates unique IDs using timestamp and random components:
```typescript
`msg_${Date.now()}_${Math.random().toString(36).substr(2, 9)}`
```
This ensures uniqueness even with rapid message creation.

### Timestamp Handling
Timestamps are set at message start, not finalization, preserving the actual start time of the stream.

### Status Transitions
- `streaming`: Set on startMessage()
- `complete`: Set on finalize()
- `error`: Can be set by consumer for failed messages

### Memory Management
MessageBuilder maintains minimal state - just the current message and content buffer. After finalization and reset, all references are cleared for garbage collection.

## Testing Considerations

When testing MessageBuilder:

1. **Test State Transitions**: Verify proper handling of start → append → finalize → reset cycle
2. **Test Edge Cases**: Empty messages, very long content, rapid resets
3. **Test Error Scenarios**: Finalize without start, multiple starts without finalize
4. **Test Metadata Preservation**: Ensure all metadata is properly attached
5. **Mock Integration Points**: Mock Logger for testing without console output

Example test structure:
```typescript
describe('MessageBuilder', () => {
  let builder: MessageBuilder;
  
  beforeEach(() => {
    builder = new MessageBuilder();
  });
  
  it('should accumulate text deltas', () => {
    builder.startMessage('assistant');
    builder.appendText('Hello');
    builder.appendText(' world');
    
    const current = builder.getCurrentMessage();
    expect(current?.content).toBe('Hello world');
  });
  
  it('should finalize with metadata', () => {
    builder.startMessage('assistant');
    builder.appendText('Response');
    
    const message = builder.finalize({
      outputTokens: 10,
      stopReason: 'stop'
    });
    
    expect(message.status).toBe('complete');
    expect(message.metadata?.outputTokens).toBe(10);
  });
});
```

## Summary

MessageBuilder is a focused, single-responsibility class that excels at managing the streaming message lifecycle. Its simple API masks the complexity of handling partial messages, state transitions, and metadata accumulation. By maintaining clear separation of concerns and providing defensive programming against edge cases, it ensures reliable message construction even in the face of network interruptions or malformed events.

The class is essential for providing a smooth real-time chat experience, enabling the UI to show progressive message updates while maintaining data integrity and proper message structure for historical display.
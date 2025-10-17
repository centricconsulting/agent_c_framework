# Advanced Message Handlers

## Overview

The Advanced Message Handlers provide sophisticated management of tool calls and rich media content within the Agent C Realtime SDK. These components work together with the MessageBuilder and EventStreamProcessor to enable advanced chat features like function calling, rich content rendering, and streaming notifications.

## Key Components

### ToolCallManager

The `ToolCallManager` tracks the lifecycle of tool usage from selection through completion. It manages tool state, provides real-time notifications for UI updates, and accumulates results for message metadata.

### RichMediaHandler

The `RichMediaHandler` processes and validates rich media content such as HTML, SVG, and images received from tools. It identifies content types, determines sanitization requirements, and provides metadata extraction for proper rendering.

## Architecture

```
ServerEvent
    ↓
EventStreamProcessor
    ├── MessageBuilder (text accumulation)
    ├── ToolCallManager (tool state tracking)
    └── RichMediaHandler (media processing)
         ↓
    SessionManager (event emission)
         ↓
    UI Components (rendering)
```

---

## ToolCallManager

Manages the complete lifecycle of tool calls from initial selection through execution to completion with results.

### Core Functionality

- **Tool State Tracking**: Maintains active and completed tool states
- **Notification Management**: Provides UI-friendly notifications for tool status
- **Result Association**: Links tool results with their corresponding calls
- **Statistics Tracking**: Provides metrics on tool usage

### Lifecycle Flow

1. **Tool Selection** (`tool_select_delta` event)
   - Creates notification with 'preparing' status
   - Stores tool arguments
   - Begins tracking

2. **Tool Execution** (`tool_call` event with `active: true`)
   - Updates notification to 'executing' status
   - Handles tools that skip selection phase

3. **Tool Completion** (`tool_call` event with `active: false`)
   - Marks notification as 'complete'
   - Associates results with tool calls
   - Moves to completed list

### Public API

#### `onToolSelect(event: ToolSelectDeltaEvent): ToolNotification`

Handles tool selection events before execution begins.

```typescript
const notification = toolCallManager.onToolSelect({
  type: 'tool_select_delta',
  session_id: 'session-123',
  tool_calls: [{
    id: 'tool-1',
    type: 'tool_use',
    name: 'search_web',
    input: { query: 'Agent C documentation' }
  }]
});

// Returns:
{
  id: 'tool-1',
  toolName: 'search_web',
  status: 'preparing',
  timestamp: Date,
  arguments: '{"query":"Agent C documentation"}'
}
```

#### `onToolCallActive(event: ToolCallEvent): ToolNotification | null`

Updates tool status to executing during active phase.

```typescript
const notification = toolCallManager.onToolCallActive({
  type: 'tool_call',
  active: true,
  tool_calls: [{
    id: 'tool-1',
    type: 'tool_use',
    name: 'search_web',
    input: { query: 'Agent C documentation' }
  }]
});

// Updates existing notification or creates new one with status: 'executing'
```

#### `onToolCallComplete(event: ToolCallEvent): ToolCallWithResult[]`

Processes tool completion with results.

```typescript
const completed = toolCallManager.onToolCallComplete({
  type: 'tool_call',
  active: false,
  tool_calls: [{
    id: 'tool-1',
    type: 'tool_use',
    name: 'search_web',
    input: { query: 'Agent C documentation' }
  }],
  tool_results: [{
    type: 'tool_result',
    tool_use_id: 'tool-1',
    content: '{"results": [...]}'
  }]
});

// Returns array of completed tool calls with attached results
```

#### `getActiveNotifications(): ToolNotification[]`

Returns all currently active tool notifications.

```typescript
const activeTools = toolCallManager.getActiveNotifications();
// Returns notifications with status 'preparing' or 'executing'
```

#### `getCompletedToolCalls(): ToolCallWithResult[]`

Returns all completed tool calls with their results.

```typescript
const completed = toolCallManager.getCompletedToolCalls();
// Returns tool calls that have finished execution
```

#### `isToolActive(id: string): boolean`

Checks if a specific tool is currently active.

```typescript
const isActive = toolCallManager.isToolActive('tool-1');
```

#### `getStatistics()`

Returns usage statistics for tools.

```typescript
const stats = toolCallManager.getStatistics();
// Returns: { activeCount: 2, completedCount: 5, totalCount: 7 }
```

#### `reset()`

Clears all tool state for new interactions.

```typescript
toolCallManager.reset();
// Clears active tools and completed calls
```

### Special Tool Handling

#### Think Tool

The "think" tool receives special treatment as its content is rendered through thought deltas rather than as a typical tool result:

```typescript
// When think tool is selected
if (toolCall.name === 'think') {
  // Emit special notification
  // Content will stream via thought_delta events
  // Tool call completion is ignored for rendering
}
```

### Data Types

```typescript
interface ToolNotification {
  id: string;
  toolName: string;
  status: 'preparing' | 'executing' | 'complete';
  timestamp: Date;
  arguments?: string;
}

interface ToolCallWithResult extends ToolCall {
  result?: ToolResult;
}
```

---

## RichMediaHandler

Processes and validates rich media content from tools, providing content type detection and metadata extraction for safe rendering.

### Core Functionality

- **Content Type Detection**: Identifies HTML, SVG, images, and unknown content
- **Validation**: Validates content structure based on type
- **Metadata Extraction**: Extracts relevant metadata from content
- **Sanitization Flagging**: Determines which content requires sanitization
- **Size Management**: Checks content size limits

### Content Type Detection

The handler uses content-type headers to determine media type:

- **HTML**: `text/html` or other `text/*` types
- **SVG**: `image/svg+xml` or `text/svg`
- **Images**: `image/png`, `image/jpeg`, `image/gif`, `image/webp`
- **Unknown**: Unrecognized content types

### Public API

#### `processRenderMediaEvent(event: RenderMediaEvent): RichMediaContent`

Processes a render media event into structured content.

```typescript
const media = richMediaHandler.processRenderMediaEvent({
  type: 'render_media',
  content: '<svg>...</svg>',
  content_type: 'image/svg+xml',
  sent_by_class: 'VisualizationTool',
  sent_by_function: 'generate_chart'
});

// Returns:
{
  id: 'media_1234567_abc',
  type: 'svg',
  content: '<svg>...</svg>',
  contentType: 'image/svg+xml',
  metadata: {
    sentByClass: 'VisualizationTool',
    sentByFunction: 'generate_chart',
    timestamp: Date
  },
  needsSanitization: true
}
```

#### `validateContent(content: string, type: RichMediaContent['type'])`

Validates content structure based on type.

```typescript
const validation = richMediaHandler.validateContent(
  '<svg width="100">...</svg>',
  'svg'
);

// Returns:
{
  isValid: true,
  errors: []
}
```

Validation checks:
- **SVG**: Presence of `<svg` or `<?xml` tags
- **HTML**: Presence of HTML tags
- **Images**: Valid data URI format for base64 images

#### `extractContentMetadata(content: string, type: RichMediaContent['type'])`

Extracts type-specific metadata from content.

```typescript
const metadata = richMediaHandler.extractContentMetadata(
  '<svg width="400" height="300" viewBox="0 0 400 300">',
  'svg'
);

// Returns:
{
  width: 400,
  height: 300,
  viewBox: '0 0 400 300',
  contentLength: 45
}
```

Extracted metadata by type:
- **SVG**: width, height, viewBox attributes
- **HTML**: title, presence of scripts, external resources
- **Images**: MIME type from data URIs

#### `createPlaceholder(type: string, reason: string): RichMediaContent`

Creates a placeholder for unsupported media types.

```typescript
const placeholder = richMediaHandler.createPlaceholder(
  'application/pdf',
  'PDF rendering not supported'
);

// Returns HTML content with styled placeholder message
```

#### `isContentSizeAcceptable(content: string, maxSizeKB?: number): boolean`

Checks if content size is within acceptable limits.

```typescript
const isAcceptable = richMediaHandler.isContentSizeAcceptable(
  largeContent,
  1024 // 1MB limit
);

// Logs warning if size exceeds limit
```

### Security Considerations

#### Sanitization Requirements

Content requiring sanitization before rendering:
- **HTML**: May contain scripts, external resources
- **SVG**: Can embed scripts and external references
- **Images**: Generally safe when using data URIs

```typescript
private requiresSanitization(type: RichMediaContent['type']): boolean {
  return type === 'html' || type === 'svg';
}
```

#### Content Validation

The handler performs basic validation but **does not sanitize content**. Sanitization must be performed in the UI layer using appropriate libraries (e.g., DOMPurify).

### Data Types

```typescript
interface RichMediaContent {
  id: string;
  type: 'html' | 'svg' | 'image' | 'unknown';
  content: string;
  contentType: string;
  metadata: {
    sentByClass: string;
    sentByFunction: string;
    timestamp: Date;
  };
  needsSanitization: boolean;
}
```

---

## Integration with EventStreamProcessor

The EventStreamProcessor orchestrates both handlers to process server events and emit appropriate UI events.

### Tool Call Processing Flow

```typescript
// 1. Tool Selection
private handleToolSelect(event: ToolSelectDeltaEvent): void {
  const notification = this.toolCallManager.onToolSelect(event);
  
  // Special handling for think tool
  if (event.tool_calls[0]?.name === 'think') {
    this.sessionManager.emit('tool-notification', {
      ...notification,
      toolName: 'think',
      status: 'preparing'
    });
  } else {
    this.sessionManager.emit('tool-notification', notification);
  }
}

// 2. Tool Execution
private handleToolCall(event: ToolCallEvent): void {
  if (event.active) {
    const notification = this.toolCallManager.onToolCallActive(event);
    if (notification) {
      this.sessionManager.emit('tool-notification', notification);
    }
  } else {
    // 3. Tool Completion
    this.toolCallManager.onToolCallComplete(event);
    
    this.sessionManager.emit('tool-call-complete', {
      toolCalls: event.tool_calls,
      toolResults: event.tool_results
    });
    
    // Remove notifications
    event.tool_calls.forEach(tc => {
      this.sessionManager.emit('tool-notification-removed', tc.id);
    });
  }
}
```

### Rich Media Processing Flow

```typescript
private handleRenderMedia(event: RenderMediaEvent): void {
  const media = this.richMediaHandler.processRenderMediaEvent(event);
  
  this.sessionManager.emit('media-added', {
    sessionId: event.session_id,
    media: {
      id: media.id,
      role: 'assistant',
      type: 'media',
      content: media.content,
      contentType: media.type,
      timestamp: media.metadata.timestamp.toISOString(),
      status: 'complete',
      metadata: {
        ...media.metadata,
        timestamp: media.metadata.timestamp.toISOString()
      }
    }
  });
}
```

### Message Completion with Tool Results

When a message completes, tool results are attached as metadata:

```typescript
private handleCompletion(event: CompletionEvent): void {
  if (!event.running && this.messageBuilder.hasCurrentMessage()) {
    const completedToolCalls = this.toolCallManager.getCompletedToolCalls();
    
    const message = this.messageBuilder.finalize({
      inputTokens: event.input_tokens,
      outputTokens: event.output_tokens,
      stopReason: event.stop_reason,
      toolCalls: completedToolCalls.map(tc => ({
        id: tc.id,
        type: tc.type,
        name: tc.name,
        input: tc.input
      })),
      toolResults: completedToolCalls
        .filter(tc => tc.result)
        .map(tc => tc.result!)
    });
    
    this.sessionManager.emit('message-complete', {
      sessionId: event.session_id,
      message
    });
  }
}
```

---

## Event System Integration

### Emitted Events

Events emitted by these handlers through the SessionManager:

#### Tool Events

- **`tool-notification`**: Tool status updates
  ```typescript
  {
    id: string;
    toolName: string;
    status: 'preparing' | 'executing' | 'complete';
    timestamp: Date;
    arguments?: string;
  }
  ```

- **`tool-notification-removed`**: Tool notification removal
  ```typescript
  toolId: string
  ```

- **`tool-call-complete`**: Tool completion with results
  ```typescript
  {
    toolCalls: ToolCall[];
    toolResults?: ToolResult[];
  }
  ```

#### Media Events

- **`media-added`**: Rich media content added
  ```typescript
  {
    sessionId: string;
    media: {
      id: string;
      role: 'assistant';
      type: 'media';
      content: string;
      contentType: string;
      timestamp: string;
      status: 'complete';
      metadata: object;
    }
  }
  ```

---

## Usage Examples

### Complete Tool Call Flow

```typescript
// Initialize managers
const sessionManager = new SessionManager(config);
const eventProcessor = new EventStreamProcessor(sessionManager);

// Process tool selection event
eventProcessor.processEvent({
  type: 'tool_select_delta',
  session_id: 'session-123',
  tool_calls: [{
    id: 'tool-1',
    type: 'tool_use',
    name: 'calculator',
    input: { expression: '2 + 2' }
  }]
});

// UI receives: tool-notification with status 'preparing'

// Process tool execution
eventProcessor.processEvent({
  type: 'tool_call',
  active: true,
  tool_calls: [{
    id: 'tool-1',
    type: 'tool_use',
    name: 'calculator',
    input: { expression: '2 + 2' }
  }]
});

// UI receives: tool-notification with status 'executing'

// Process tool completion
eventProcessor.processEvent({
  type: 'tool_call',
  active: false,
  tool_calls: [{
    id: 'tool-1',
    type: 'tool_use',
    name: 'calculator',
    input: { expression: '2 + 2' }
  }],
  tool_results: [{
    type: 'tool_result',
    tool_use_id: 'tool-1',
    content: '{"result": 4}'
  }]
});

// UI receives: tool-call-complete and tool-notification-removed
```

### Rich Media Rendering Flow

```typescript
// Process rich media event
eventProcessor.processEvent({
  type: 'render_media',
  session_id: 'session-123',
  content: `
    <svg width="200" height="200">
      <circle cx="100" cy="100" r="50" fill="blue"/>
    </svg>
  `,
  content_type: 'image/svg+xml',
  sent_by_class: 'ChartGenerator',
  sent_by_function: 'create_pie_chart'
});

// UI receives: media-added event with SVG content
// UI must sanitize SVG before rendering (needsSanitization: true)
```

### Think Tool Special Handling

```typescript
// Think tool selection
eventProcessor.processEvent({
  type: 'tool_select_delta',
  tool_calls: [{
    id: 'think-1',
    type: 'tool_use',
    name: 'think',
    input: {}
  }]
});

// UI receives: tool-notification for 'think'

// Thought content streams via thought_delta events
eventProcessor.processEvent({
  type: 'thought_delta',
  content: 'Analyzing the user request...'
});

// Thought deltas remove the notification automatically
// Tool call completion for think is ignored
```

---

## Error Handling

### ToolCallManager Error Scenarios

1. **Missing Tool Calls**
   ```typescript
   // Throws error if event has no tool calls
   if (!event.tool_calls.length) {
     throw new Error('ToolSelectDeltaEvent has no tool calls');
   }
   ```

2. **Orphaned Tool Execution**
   ```typescript
   // Creates new notification if tool wasn't selected first
   if (!existingNotification) {
     return createNewNotification();
   }
   ```

### RichMediaHandler Error Scenarios

1. **Invalid Content Type**
   ```typescript
   // Returns 'unknown' type for unrecognized content
   if (!recognizedType) {
     return 'unknown';
   }
   ```

2. **Content Size Limits**
   ```typescript
   // Logs warning and returns false for oversized content
   if (!isContentSizeAcceptable(content, 1024)) {
     Logger.warn('Content size exceeds limit');
   }
   ```

3. **Validation Failures**
   ```typescript
   const validation = validateContent(content, type);
   if (!validation.isValid) {
     // Handle validation errors
     console.error('Validation errors:', validation.errors);
   }
   ```

---

## Best Practices

### Tool Call Management

1. **Always Reset on New Interactions**
   ```typescript
   // Reset managers when starting new interaction
   toolCallManager.reset();
   messageBuilder.reset();
   ```

2. **Handle Think Tool Specially**
   ```typescript
   // Filter out think tool from normal processing
   if (toolCall.name === 'think') {
     // Handle differently - content comes via thought deltas
   }
   ```

3. **Associate Results with Messages**
   ```typescript
   // Attach tool results to message metadata on completion
   const message = messageBuilder.finalize({
     toolCalls: completedCalls,
     toolResults: results
   });
   ```

### Rich Media Processing

1. **Always Check Sanitization Requirements**
   ```typescript
   if (media.needsSanitization) {
     // Sanitize in UI layer before rendering
     const sanitized = DOMPurify.sanitize(media.content);
   }
   ```

2. **Validate Content Before Processing**
   ```typescript
   const validation = richMediaHandler.validateContent(content, type);
   if (!validation.isValid) {
     // Use placeholder or handle error
     return richMediaHandler.createPlaceholder(type, validation.errors[0]);
   }
   ```

3. **Extract and Use Metadata**
   ```typescript
   const metadata = richMediaHandler.extractContentMetadata(content, type);
   // Use metadata for rendering decisions (dimensions, scripts, etc.)
   ```

4. **Enforce Size Limits**
   ```typescript
   if (!richMediaHandler.isContentSizeAcceptable(content, 1024)) {
     // Reject or truncate oversized content
   }
   ```

---

## Performance Considerations

### Tool Call Manager

- **Memory Management**: Clear completed tools periodically
  ```typescript
  toolCallManager.clearCompleted();
  ```

- **Statistics Caching**: Statistics are computed on-demand
  ```typescript
  // Efficient O(1) operations
  const stats = toolCallManager.getStatistics();
  ```

### Rich Media Handler

- **Content Size Checking**: Check size before processing
  ```typescript
  // Early rejection of oversized content
  if (content.length > maxSize) return;
  ```

- **Metadata Extraction**: Cache extracted metadata
  ```typescript
  // Extract once, use multiple times
  const metadata = extractContentMetadata(content, type);
  ```

- **Validation Optimization**: Perform minimal validation
  ```typescript
  // Quick checks for content type validation
  if (!content.includes('<svg')) return false;
  ```

---

## Testing

### ToolCallManager Testing

```typescript
describe('ToolCallManager', () => {
  it('should track tool lifecycle', () => {
    const manager = new ToolCallManager();
    
    // Selection
    const notification = manager.onToolSelect(selectEvent);
    expect(notification.status).toBe('preparing');
    
    // Execution
    const active = manager.onToolCallActive(activeEvent);
    expect(active.status).toBe('executing');
    
    // Completion
    const completed = manager.onToolCallComplete(completeEvent);
    expect(completed[0].result).toBeDefined();
  });
  
  it('should handle think tool specially', () => {
    const notification = manager.onToolSelect({
      tool_calls: [{ name: 'think', ... }]
    });
    expect(notification.toolName).toBe('think');
  });
});
```

### RichMediaHandler Testing

```typescript
describe('RichMediaHandler', () => {
  it('should detect content types', () => {
    const handler = new RichMediaHandler();
    
    const svg = handler.processRenderMediaEvent({
      content_type: 'image/svg+xml',
      content: '<svg>...</svg>'
    });
    expect(svg.type).toBe('svg');
    expect(svg.needsSanitization).toBe(true);
  });
  
  it('should validate content structure', () => {
    const validation = handler.validateContent('<div>test</div>', 'html');
    expect(validation.isValid).toBe(true);
  });
  
  it('should extract metadata', () => {
    const metadata = handler.extractContentMetadata(
      '<svg width="100" height="200">',
      'svg'
    );
    expect(metadata.width).toBe(100);
    expect(metadata.height).toBe(200);
  });
});
```

---

## Migration Guide

### From Direct Event Handling

If migrating from direct event handling to using these managers:

```typescript
// Before: Manual tracking
let toolStatus = {};
eventEmitter.on('tool_call', (event) => {
  if (event.active) {
    toolStatus[event.tool_calls[0].id] = 'active';
  } else {
    toolStatus[event.tool_calls[0].id] = 'complete';
  }
});

// After: Using ToolCallManager
const toolCallManager = new ToolCallManager();
eventEmitter.on('tool_call', (event) => {
  if (event.active) {
    toolCallManager.onToolCallActive(event);
  } else {
    toolCallManager.onToolCallComplete(event);
  }
});
```

### From Raw Media Handling

```typescript
// Before: Manual media processing
eventEmitter.on('render_media', (event) => {
  const media = {
    content: event.content,
    type: event.content_type.includes('svg') ? 'svg' : 'html'
  };
});

// After: Using RichMediaHandler
const richMediaHandler = new RichMediaHandler();
eventEmitter.on('render_media', (event) => {
  const media = richMediaHandler.processRenderMediaEvent(event);
  // Includes type detection, validation, metadata
});
```

---

## See Also

- [MessageBuilder Documentation](./MessageBuilder.md) - Text accumulation and message construction
- [EventStreamProcessor Documentation](./EventStreamProcessor.md) - Central event coordination
- [SessionManager Documentation](./SessionManager.md) - Session and message management
- [Event Types Reference](./Types.md) - Complete event type definitions
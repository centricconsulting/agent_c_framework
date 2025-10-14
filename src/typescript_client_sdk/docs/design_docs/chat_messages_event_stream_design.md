# Chat Messages and Event Stream Design Document (v2.0)

## Executive Summary

This document outlines the comprehensive architecture for implementing chat message display and real-time event stream processing in the Agent C Realtime SDK. The implementation spans both core SDK components for event processing and UI components for rich message display, supporting streaming text, thoughts, tool calls, rich media, MultiModal content, and message editing capabilities.

## Requirements Overview

### Core Requirements
1. **Initial message population** from ChatSessionChangedEvent
2. **Real-time user message display** from anthropic_user_message events
3. **Parallel accumulation** of thought and text deltas
4. **Support for multiple message types** (assistant, user, thought)
5. **MultiModal content support** (text, images, tool calls)
6. **Sub-session handling** for agent-to-agent conversations
7. **Rich message display** with Markdown, footers, and interactive elements
8. **Message editing capability** with history truncation
9. **Real-time tool usage notifications**
10. **Rich media rendering** (HTML, SVG)
11. **System notifications and error display**
12. **Event filtering** for ignored event types

## Data Models

### Server Data Structures

The server sends data in vendor-specific formats that must be normalized for UI consumption.

#### ChatSession Structure

```typescript
// Actual server ChatSession structure
export interface ChatSession {
  session_id: string;
  created_at?: string;
  updated_at?: string;
  title?: string;
  deleted?: boolean;
  vendor?: 'anthropic' | 'openai';  // Vendor-specific message format
  display_name?: string;             // User display name
  messages: MessageParam[];          // Vendor-specific message array
}
```

#### MessageParam Structure (Vendor-Specific)

The `MessageParam` type varies based on the vendor field:

```typescript
// Anthropic format (when vendor === 'anthropic')
export interface AnthropicMessageParam {
  role: 'user' | 'assistant';
  content: string | ContentBlockParam[];  // Can be string or array
}

// ContentBlockParam for rich content
export type ContentBlockParam = 
  | TextBlockParam 
  | ImageBlockParam 
  | ToolUseBlockParam 
  | ToolResultBlockParam;

export interface TextBlockParam {
  type: 'text';
  text: string;
  cache_control?: CacheControlEphemeral;
}

export interface ImageBlockParam {
  type: 'image';
  source: {
    type: 'base64';
    media_type: string;
    data: string;
  };
}

export interface ToolUseBlockParam {
  type: 'tool_use';
  id: string;
  name: string;
  input: unknown;
  cache_control?: CacheControlEphemeral;
}

export interface ToolResultBlockParam {
  type: 'tool_result';
  tool_use_id: string;
  content?: string | ContentBlockParam[];
  is_error?: boolean;
  cache_control?: CacheControlEphemeral;
}
```

### Server Event Types

```typescript
// User message event (real-time)
export interface AnthropicUserMessageEvent {
  type: 'anthropic_user_message';
  session_id: string;
  role: 'assistant';  // Always 'assistant' for routing
  parent_session_id: string | null;
  user_session_id: string;
  vendor: 'anthropic';
  message: {
    role: 'user';
    content: string | ContentBlockParam[];
  };
}

// Streaming events
export interface TextDeltaEvent {
  type: 'text_delta';
  session_id: string;
  content: string;
  format?: 'markdown';
}

export interface ThoughtDeltaEvent {
  type: 'thought_delta';
  session_id: string;
  content: string;
  format?: 'markdown';
}

// Events to ignore
export interface IgnoredEvents {
  type: 'history' | 'history_delta' | 'complete_thought' | 'system_prompt';
  // These events should be filtered out
}
```

### UI Data Structures

After normalization, the UI works with consistent structures:

```typescript
// Enhanced message for UI consumption
export interface EnhancedMessage {
  id: string;
  role: 'user' | 'assistant';
  content: string;  // Text representation for display
  multiModalContent?: ContentBlockParam[];  // Preserved structured content
  timestamp: string;
  type: 'message' | 'thought' | 'media' | 'notification';
  status: 'streaming' | 'complete' | 'error';
  isSubSession?: boolean;  // True if session_id !== user_session_id
  metadata?: {
    vendor?: 'anthropic' | 'openai';
    displayName?: string;
    sessionId?: string;  // Original session_id for sub-sessions
    parentSessionId?: string;
    inputTokens?: number;
    outputTokens?: number;
    stopReason?: string;
  };
  toolCalls?: ToolCall[];
  isCollapsed?: boolean; // For thoughts
}
```

## Architecture Overview

```
┌─────────────────────────────────────────────────────────┐
│                    Event Stream                         │
│  (TextDelta, ThoughtDelta, ToolCall, RenderMedia, etc.) │
└────────────────────┬────────────────────────────────────┘
                     │
                     ▼
┌─────────────────────────────────────────────────────────┐
│              EventStreamProcessor                       │
│  - Routes events to appropriate handlers                │
│  - Filters ignored events                              │
│  - Detects sub-sessions                               │
│  - Coordinates message building                        │
└────────────────────┬────────────────────────────────────┘
                     │
         ┌───────────┴───────────┬─────────────┐
         ▼                       ▼             ▼
┌──────────────────┐  ┌──────────────────┐  ┌──────────────────┐
│ DualMessageBuilder│  │ ToolCallManager │  │ RichMediaHandler │
│  - Parallel       │  │ - Tracks tools   │  │ - Processes      │
│    accumulation   │  │ - Notifications  │  │   media events   │
└──────────────────┘  └──────────────────┘  └──────────────────┘
         │                       │             │
         └───────────┬───────────┴─────────────┘
                     ▼
┌─────────────────────────────────────────────────────────┐
│              Enhanced SessionManager                    │
│  - Maintains message history                           │
│  - Handles message mutations                           │
│  - Tracks sub-sessions                                │
│  - Emits UI update events                             │
└─────────────────────────────────────────────────────────┘
                     │
                     ▼
┌─────────────────────────────────────────────────────────┐
│                  UI Components                         │
│  - MessageList, Message, ThoughtMessage                │
│  - ImageDisplay, MultiModalContent                     │
│  - ToolCallDisplay, RichMediaDisplay                   │
│  - SubSessionIndicator, SystemNotification             │
└─────────────────────────────────────────────────────────┘
```

## Core SDK Components

### 1. EventStreamProcessor

**Purpose:** Central coordinator for processing incoming events with proper filtering and routing

```typescript
export class EventStreamProcessor {
  private messageBuilder: DualMessageBuilder;
  private toolCallManager: ToolCallManager;
  private richMediaHandler: RichMediaHandler;
  private sessionManager: SessionManager;
  private currentVendor: 'anthropic' | 'openai' = 'anthropic';
  private userSessionId: string;
  
  // Events to explicitly ignore
  private readonly IGNORED_EVENTS = [
    'history',
    'history_delta',
    'complete_thought',
    'system_prompt'
  ];
  
  constructor(sessionManager: SessionManager, userSessionId: string) {
    this.sessionManager = sessionManager;
    this.userSessionId = userSessionId;
    this.messageBuilder = new DualMessageBuilder();
    this.toolCallManager = new ToolCallManager();
    this.richMediaHandler = new RichMediaHandler();
  }
  
  processEvent(event: ServerEvent): void {
    // Filter ignored events
    if (this.IGNORED_EVENTS.includes(event.type)) {
      return; // Silently ignore
    }
    
    // Detect sub-session (agent-to-agent conversation)
    const isSubSession = this.isSubSessionEvent(event);
    
    switch (event.type) {
      case 'chat_session_changed':
        this.handleSessionChanged(event);
        break;
      case 'anthropic_user_message':
        this.handleUserMessage(event);
        break;
      case 'text_delta':
        this.handleTextDelta(event, isSubSession);
        break;
      case 'thought_delta':
        this.handleThoughtDelta(event, isSubSession);
        break;
      case 'completion':
        this.handleCompletion(event, isSubSession);
        break;
      case 'tool_select_delta':
        this.handleToolSelect(event);
        break;
      case 'tool_call':
        this.handleToolCall(event);
        break;
      case 'render_media':
        this.handleRenderMedia(event);
        break;
      case 'interaction':
        this.handleInteraction(event);
        break;
      // ... other events
    }
  }
  
  private isSubSessionEvent(event: any): boolean {
    return event.session_id && 
           event.user_session_id && 
           event.session_id !== event.user_session_id;
  }
  
  private handleSessionChanged(event: ChatSessionChangedEvent): void {
    const session = event.chat_session;
    this.currentVendor = session.vendor || 'anthropic';
    
    // Normalize all messages in the session
    const normalizedMessages = session.messages.map(msg => 
      this.normalizeMessage(msg, session.vendor, session.display_name)
    );
    
    // Update session with normalized messages
    this.sessionManager.setSession({
      ...session,
      messages: normalizedMessages
    });
  }
  
  private handleUserMessage(event: AnthropicUserMessageEvent): void {
    // Handle real-time user message display
    const message = this.normalizeMessage(
      event.message,
      event.vendor,
      null,
      event.session_id,
      event.parent_session_id
    );
    
    // Mark as sub-session if applicable
    if (this.isSubSessionEvent(event)) {
      message.isSubSession = true;
      message.metadata!.sessionId = event.session_id;
      message.metadata!.parentSessionId = event.parent_session_id;
    }
    
    this.sessionManager.addMessage(message);
  }
  
  private normalizeMessage(
    message: MessageParam,
    vendor?: string,
    displayName?: string | null,
    sessionId?: string,
    parentSessionId?: string | null
  ): EnhancedMessage {
    const normalized: EnhancedMessage = {
      id: generateMessageId(),
      role: message.role,
      content: this.extractTextContent(message.content),
      multiModalContent: Array.isArray(message.content) ? message.content : undefined,
      timestamp: new Date().toISOString(),
      type: 'message',
      status: 'complete',
      metadata: {
        vendor: vendor as 'anthropic' | 'openai',
        displayName: displayName || undefined,
        sessionId,
        parentSessionId: parentSessionId || undefined
      }
    };
    
    // Extract tool calls if present
    if (Array.isArray(message.content)) {
      normalized.toolCalls = this.extractToolCalls(message.content);
    }
    
    return normalized;
  }
  
  private extractTextContent(content: string | ContentBlockParam[]): string {
    if (typeof content === 'string') {
      return content;
    }
    
    // Extract text and provide placeholders for other content
    return content.map(block => {
      switch (block.type) {
        case 'text':
          return block.text;
        case 'image':
          return '[Image]'; // Placeholder, actual image in multiModalContent
        case 'tool_use':
          return `[Tool: ${block.name}]`;
        case 'tool_result':
          if (typeof block.content === 'string') {
            return block.content;
          }
          if (Array.isArray(block.content)) {
            return this.extractTextContent(block.content);
          }
          return '[Tool Result]';
        default:
          return '';
      }
    }).join('\n');
  }
  
  private handleTextDelta(event: TextDeltaEvent, isSubSession: boolean): void {
    this.messageBuilder.appendText(event.content, isSubSession);
    const message = this.messageBuilder.getCurrentAssistantMessage();
    if (message) {
      this.sessionManager.updateStreamingMessage(message);
    }
  }
  
  private handleThoughtDelta(event: ThoughtDeltaEvent, isSubSession: boolean): void {
    this.messageBuilder.appendThought(event.content, isSubSession);
    const message = this.messageBuilder.getCurrentThoughtMessage();
    if (message) {
      this.sessionManager.updateStreamingMessage(message);
    }
  }
  
  private handleCompletion(event: CompletionEvent, isSubSession: boolean): void {
    // Filter out completion_opts
    const { completion_options, ...relevantData } = event;
    
    if (!event.running) {
      // Finalize all active messages
      const messages = this.messageBuilder.finalizeAll({
        inputTokens: event.input_tokens,
        outputTokens: event.output_tokens,
        stopReason: event.stop_reason,
        vendor: this.currentVendor
      }, isSubSession);
      
      messages.forEach(msg => {
        this.sessionManager.finalizeMessage(msg);
      });
      
      this.messageBuilder.reset();
    }
  }
  
  private handleInteraction(event: InteractionEvent): void {
    if (event.started) {
      // New interaction starting, prepare for messages
      this.messageBuilder.reset();
    }
  }
  
  private extractToolCalls(content: ContentBlockParam[]): ToolCall[] | undefined {
    const toolCalls = content
      .filter(block => block.type === 'tool_use')
      .map(block => {
        const toolBlock = block as ToolUseBlockParam;
        return {
          id: toolBlock.id,
          function: {
            name: toolBlock.name,
            arguments: JSON.stringify(toolBlock.input)
          }
        };
      });
    
    return toolCalls.length > 0 ? toolCalls : undefined;
  }
}
```

### 2. DualMessageBuilder

**Purpose:** Supports parallel accumulation of thought and text content with proper transition handling

```typescript
export interface MessageMetadata {
  inputTokens?: number;
  outputTokens?: number;
  stopReason?: string;
  toolCalls?: ToolCall[];
  vendor?: 'anthropic' | 'openai';
  displayName?: string;
  sessionId?: string;
  parentSessionId?: string;
}

export class DualMessageBuilder {
  private thoughtMessage: Partial<EnhancedMessage> | null = null;
  private assistantMessage: Partial<EnhancedMessage> | null = null;
  private thoughtContent: string = '';
  private assistantContent: string = '';
  
  appendThought(delta: string, isSubSession: boolean = false): void {
    if (!this.thoughtMessage) {
      this.startThoughtMessage(isSubSession);
    }
    this.thoughtContent += delta;
    if (this.thoughtMessage) {
      this.thoughtMessage.content = this.thoughtContent;
    }
  }
  
  appendText(delta: string, isSubSession: boolean = false): void {
    // If we have an unfinalized thought, keep it active
    // Both can accumulate in parallel
    if (!this.assistantMessage) {
      this.startAssistantMessage(isSubSession);
    }
    this.assistantContent += delta;
    if (this.assistantMessage) {
      this.assistantMessage.content = this.assistantContent;
    }
  }
  
  private startThoughtMessage(isSubSession: boolean): void {
    this.thoughtMessage = {
      id: generateMessageId(),
      role: 'assistant',
      content: '',
      timestamp: new Date().toISOString(),
      type: 'thought',
      status: 'streaming',
      isSubSession,
      isCollapsed: true // Thoughts start collapsed
    };
    this.thoughtContent = '';
  }
  
  private startAssistantMessage(isSubSession: boolean): void {
    this.assistantMessage = {
      id: generateMessageId(),
      role: 'assistant',
      content: '',
      timestamp: new Date().toISOString(),
      type: 'message',
      status: 'streaming',
      isSubSession
    };
    this.assistantContent = '';
  }
  
  getCurrentThoughtMessage(): Partial<EnhancedMessage> | null {
    return this.thoughtMessage ? {
      ...this.thoughtMessage,
      content: this.thoughtContent
    } : null;
  }
  
  getCurrentAssistantMessage(): Partial<EnhancedMessage> | null {
    return this.assistantMessage ? {
      ...this.assistantMessage,
      content: this.assistantContent
    } : null;
  }
  
  finalizeAll(metadata: MessageMetadata, isSubSession: boolean = false): EnhancedMessage[] {
    const messages: EnhancedMessage[] = [];
    
    // Finalize thought if exists
    if (this.thoughtMessage) {
      messages.push({
        ...this.thoughtMessage,
        content: this.thoughtContent,
        status: 'complete',
        metadata: { ...metadata },
        isSubSession
      } as EnhancedMessage);
    }
    
    // Finalize assistant message if exists
    if (this.assistantMessage) {
      messages.push({
        ...this.assistantMessage,
        content: this.assistantContent,
        status: 'complete',
        metadata: { ...metadata },
        isSubSession
      } as EnhancedMessage);
    }
    
    return messages;
  }
  
  reset(): void {
    this.thoughtMessage = null;
    this.assistantMessage = null;
    this.thoughtContent = '';
    this.assistantContent = '';
  }
}
```

### 3. ToolCallManager

**Purpose:** Tracks tool usage state and manages notifications

```typescript
export interface ToolNotification {
  id: string;
  toolName: string;
  status: 'preparing' | 'executing' | 'complete';
  timestamp: Date;
}

export class ToolCallManager {
  private activeTools: Map<string, ToolNotification> = new Map();
  private completedToolCalls: ToolCall[] = [];
  
  onToolSelect(event: ToolSelectDeltaEvent): ToolNotification {
    const toolCall = event.tool_calls[0];
    const notification: ToolNotification = {
      id: toolCall.id,
      toolName: toolCall.function.name,
      status: 'preparing',
      timestamp: new Date()
    };
    
    this.activeTools.set(toolCall.id, notification);
    return notification;
  }
  
  onToolCallActive(event: ToolCallEvent): ToolNotification | null {
    const toolCall = event.tool_calls[0];
    const notification = this.activeTools.get(toolCall.id);
    
    if (notification && event.active) {
      notification.status = 'executing';
      return notification;
    }
    
    return null;
  }
  
  onToolCallComplete(event: ToolCallEvent): ToolCall[] {
    if (!event.active && event.tool_results) {
      event.tool_calls.forEach(tc => {
        this.activeTools.delete(tc.id);
        this.completedToolCalls.push({
          ...tc,
          result: event.tool_results?.find(r => r.tool_use_id === tc.id)
        });
      });
    }
    
    return this.completedToolCalls;
  }
  
  getActiveNotifications(): ToolNotification[] {
    return Array.from(this.activeTools.values())
      .filter(n => n.status !== 'complete');
  }
  
  reset(): void {
    this.activeTools.clear();
    this.completedToolCalls = [];
  }
}
```

### 4. RichMediaHandler

**Purpose:** Processes and validates rich media content

```typescript
export interface RichMediaContent {
  id: string;
  type: 'html' | 'svg' | 'image';
  content: string;
  metadata: {
    sentByClass: string;
    sentByFunction: string;
    timestamp: Date;
  };
}

export class RichMediaHandler {
  processRenderMediaEvent(event: RenderMediaEvent): RichMediaContent {
    const mediaType = this.determineMediaType(event.content_type);
    
    return {
      id: generateMediaId(),
      type: mediaType,
      content: this.sanitizeContent(event.content, mediaType),
      metadata: {
        sentByClass: event.sent_by_class,
        sentByFunction: event.sent_by_function,
        timestamp: new Date()
      }
    };
  }
  
  private determineMediaType(contentType: string): 'html' | 'svg' | 'image' {
    if (contentType.includes('html')) return 'html';
    if (contentType.includes('svg')) return 'svg';
    return 'image';
  }
  
  private sanitizeContent(content: string, type: string): string {
    // Implement content sanitization based on type
    // For HTML: use DOMPurify or similar
    // For SVG: validate SVG structure
    return content;
  }
}
```

### 5. Enhanced SessionManager

**Additions to existing SessionManager:**

```typescript
class SessionManager {
  // ... existing code ...
  
  private streamingMessages: Map<string, Partial<EnhancedMessage>> = new Map();
  private toolNotifications: Map<string, ToolNotification> = new Map();
  private subSessions: Set<string> = new Set();
  
  updateStreamingMessage(message: Partial<EnhancedMessage>): void {
    if (!message.id) return;
    
    this.streamingMessages.set(message.id, message);
    this.emit('message-streaming', {
      sessionId: this.currentSessionId!,
      message
    });
  }
  
  finalizeMessage(message: EnhancedMessage): void {
    const session = this.getCurrentSession();
    if (!session) return;
    
    // Remove from streaming
    this.streamingMessages.delete(message.id);
    
    // Track sub-sessions
    if (message.isSubSession && message.metadata?.sessionId) {
      this.subSessions.add(message.metadata.sessionId);
    }
    
    // Replace or add the finalized message
    const existingIndex = session.messages.findIndex(
      m => m.id === message.id
    );
    
    if (existingIndex >= 0) {
      session.messages[existingIndex] = message;
    } else {
      session.messages.push(message);
    }
    
    this.emit('message-complete', {
      sessionId: session.session_id,
      message
    });
  }
  
  addMessage(message: EnhancedMessage): void {
    const session = this.getCurrentSession();
    if (!session) return;
    
    session.messages.push(message);
    this.emit('message-added', {
      sessionId: session.session_id,
      message
    });
  }
  
  getStreamingMessages(): Partial<EnhancedMessage>[] {
    return Array.from(this.streamingMessages.values());
  }
  
  isSubSession(sessionId: string): boolean {
    return this.subSessions.has(sessionId);
  }
  
  addToolNotification(notification: ToolNotification): void {
    this.toolNotifications.set(notification.id, notification);
    this.emit('tool-notification', notification);
  }
  
  removeToolNotification(id: string): void {
    this.toolNotifications.delete(id);
    this.emit('tool-notification-removed', id);
  }
  
  addRichMedia(media: RichMediaContent): void {
    const session = this.getCurrentSession();
    if (!session) return;
    
    const mediaMessage: EnhancedMessage = {
      id: media.id,
      role: 'assistant',
      type: 'media',
      content: media.content,
      timestamp: media.metadata.timestamp.toISOString(),
      status: 'complete',
      metadata: media.metadata as any
    };
    
    session.messages.push(mediaMessage);
    this.emit('media-added', {
      sessionId: session.session_id,
      media: mediaMessage
    });
  }
}
```

## UI Components Design

### 1. Enhanced Message Component

```tsx
export interface MessageProps {
  message: EnhancedMessage;
  onEdit?: (messageId: string, content: string) => void;
  onCopy?: (content: string) => void;
  showTokenCounts?: boolean;
}

export const Message: React.FC<MessageProps> = ({
  message,
  onEdit,
  onCopy,
  showTokenCounts = true
}) => {
  const [isEditing, setIsEditing] = useState(false);
  const [editContent, setEditContent] = useState(message.content);
  const [showToolCalls, setShowToolCalls] = useState(false);
  
  // Handle different message types
  if (message.type === 'thought') {
    return <ThoughtMessage message={message} />;
  }
  
  if (message.type === 'media') {
    return <RichMediaDisplay message={message} />;
  }
  
  return (
    <div className={`message-container ${message.isSubSession ? 'sub-session' : ''}`}>
      {message.isSubSession && (
        <SubSessionIndicator 
          sessionId={message.metadata?.sessionId} 
          parentSessionId={message.metadata?.parentSessionId}
        />
      )}
      
      <div className="message-content">
        {isEditing ? (
          <MessageEditor
            content={editContent}
            onSave={(content) => {
              onEdit?.(message.id, content);
              setIsEditing(false);
            }}
            onCancel={() => setIsEditing(false)}
          />
        ) : (
          <>
            {/* Render MultiModal content */}
            {message.multiModalContent ? (
              <MultiModalContent 
                content={message.multiModalContent}
                textContent={message.content}
              />
            ) : (
              <MarkdownRenderer content={message.content} />
            )}
          </>
        )}
      </div>
      
      <MessageFooter
        message={message}
        onEdit={() => setIsEditing(true)}
        onCopy={() => onCopy?.(message.content)}
        onToggleToolCalls={() => setShowToolCalls(!showToolCalls)}
        showTokenCounts={showTokenCounts}
      />
      
      {showToolCalls && message.toolCalls && (
        <ToolCallDisplay toolCalls={message.toolCalls} />
      )}
    </div>
  );
};
```

### 2. MultiModalContent Component

```tsx
export const MultiModalContent: React.FC<{
  content: ContentBlockParam[];
  textContent: string;
}> = ({ content, textContent }) => {
  return (
    <div className="multimodal-content">
      {content.map((block, index) => {
        switch (block.type) {
          case 'text':
            return (
              <div key={index} className="text-block">
                <MarkdownRenderer content={block.text} />
              </div>
            );
            
          case 'image':
            return (
              <ImageDisplay 
                key={index}
                source={block.source}
                className="message-image"
              />
            );
            
          case 'tool_use':
            return (
              <div key={index} className="tool-use-block">
                <span className="tool-badge">Tool: {block.name}</span>
              </div>
            );
            
          case 'tool_result':
            return (
              <div key={index} className="tool-result-block">
                {typeof block.content === 'string' ? (
                  <MarkdownRenderer content={block.content} />
                ) : (
                  <MultiModalContent 
                    content={block.content || []} 
                    textContent=""
                  />
                )}
              </div>
            );
            
          default:
            return null;
        }
      })}
    </div>
  );
};
```

### 3. ImageDisplay Component

```tsx
export const ImageDisplay: React.FC<{
  source: {
    type: 'base64';
    media_type: string;
    data: string;
  };
  className?: string;
}> = ({ source, className }) => {
  const [isExpanded, setIsExpanded] = useState(false);
  const imageUrl = `data:${source.media_type};base64,${source.data}`;
  
  return (
    <div className={`image-display ${className || ''}`}>
      <img
        src={imageUrl}
        alt="User uploaded image"
        className={isExpanded ? 'expanded' : 'thumbnail'}
        onClick={() => setIsExpanded(!isExpanded)}
      />
      <div className="image-controls">
        <button 
          onClick={() => setIsExpanded(!isExpanded)}
          aria-label={isExpanded ? 'Collapse' : 'Expand'}
        >
          {isExpanded ? <CollapseIcon /> : <ExpandIcon />}
        </button>
        <button 
          onClick={() => downloadImage(imageUrl, source.media_type)}
          aria-label="Download"
        >
          <DownloadIcon />
        </button>
      </div>
    </div>
  );
};
```

### 4. SubSessionIndicator Component

```tsx
export const SubSessionIndicator: React.FC<{
  sessionId?: string;
  parentSessionId?: string;
}> = ({ sessionId, parentSessionId }) => {
  return (
    <div className="sub-session-indicator">
      <span className="sub-session-badge">
        Agent-to-Agent
      </span>
      <span className="session-info">
        Session: {sessionId?.slice(0, 8)}...
      </span>
    </div>
  );
};
```

### 5. ThoughtMessage Component

```tsx
export const ThoughtMessage: React.FC<{ message: EnhancedMessage }> = ({
  message
}) => {
  const [isCollapsed, setIsCollapsed] = useState(message.isCollapsed ?? true);
  const firstLine = message.content.split('\n')[0];
  
  return (
    <div className={`thought-message ${message.isSubSession ? 'sub-session' : ''}`}>
      {message.isSubSession && (
        <SubSessionIndicator 
          sessionId={message.metadata?.sessionId} 
          parentSessionId={message.metadata?.parentSessionId}
        />
      )}
      
      <button
        className="thought-toggle"
        onClick={() => setIsCollapsed(!isCollapsed)}
      >
        <ChevronIcon className={isCollapsed ? 'rotate-0' : 'rotate-90'} />
        <span className="thought-preview">
          {isCollapsed ? firstLine : 'Hide thought process'}
        </span>
      </button>
      
      {!isCollapsed && (
        <div className="thought-content">
          <MarkdownRenderer content={message.content} />
          <MessageFooter
            message={message}
            onCopy={() => navigator.clipboard.writeText(message.content)}
            showTokenCounts={true}
          />
        </div>
      )}
    </div>
  );
};
```

## Event Flow Sequences

### 1. Session Initialization Flow

```
1. ChatSessionChangedEvent received with vendor and display_name
   → EventStreamProcessor.handleSessionChanged()
   → Store vendor for normalization
   → Normalize all MessageParam objects (preserve MultiModal content)
   → SessionManager.setSession() with normalized data
   → UI receives messages with proper MultiModal content
```

### 2. Real-time User Message Flow

```
1. User sends message with image
2. AnthropicUserMessageEvent received
   → EventStreamProcessor.handleUserMessage()
   → Normalize message (preserve image in multiModalContent)
   → Detect if sub-session
   → SessionManager.addMessage()
   → UI immediately displays user's MultiModal message
```

### 3. Parallel Thought + Text Response Flow

```
1. User sends message → TextInputEvent
2. Server responds with InteractionEvent (started: true)
3. CompletionEvent (running: true)
4. ThoughtDeltaEvent stream begins
   → DualMessageBuilder.appendThought()
   → SessionManager.updateStreamingMessage() for thought
   → UI shows collapsible thought streaming
5. TextDeltaEvent stream begins (parallel with thought)
   → DualMessageBuilder.appendText()
   → SessionManager.updateStreamingMessage() for text
   → UI shows assistant message streaming
6. CompletionEvent (running: false) with token counts
   → DualMessageBuilder.finalizeAll() returns both messages
   → SessionManager.finalizeMessage() for each
   → UI shows both complete messages with footers
7. InteractionEvent (started: false)
```

### 4. Sub-Session Flow

```
1. Event received with session_id !== user_session_id
   → EventStreamProcessor.isSubSessionEvent() returns true
   → Message marked with isSubSession: true
   → Metadata includes original session_id and parent_session_id
   → UI displays with SubSessionIndicator
   → Message only visible during live session (not in resume)
```

### 5. Tool Usage Flow

```
1. ToolSelectDeltaEvent received
   → ToolCallManager.onToolSelect()
   → UI shows "Agent is preparing to use X..."
2. ToolCallEvent (active: true)
   → ToolCallManager.onToolCallActive()
   → UI updates to "Agent is using X..."
3. Tool execution occurs
4. ToolCallEvent (active: false) with results
   → ToolCallManager.onToolCallComplete()
   → UI removes notification
   → Tool calls added to message footer
```

## React Hook Integration

### useMessages Hook

```typescript
export const useMessages = () => {
  const client = useRealtimeClient();
  const [messages, setMessages] = useState<EnhancedMessage[]>([]);
  const [streamingMessages, setStreamingMessages] = useState<Map<string, Partial<EnhancedMessage>>>(new Map());
  const [toolNotifications, setToolNotifications] = useState<ToolNotification[]>([]);
  
  useEffect(() => {
    const sessionManager = client.getSessionManager();
    
    const handleMessageStreaming = ({ message }: any) => {
      setStreamingMessages(prev => {
        const newMap = new Map(prev);
        if (message.id) {
          newMap.set(message.id, message);
        }
        return newMap;
      });
    };
    
    const handleMessageComplete = ({ message }: any) => {
      setMessages(prev => {
        const index = prev.findIndex(m => m.id === message.id);
        if (index >= 0) {
          const newMessages = [...prev];
          newMessages[index] = message;
          return newMessages;
        }
        return [...prev, message];
      });
      
      setStreamingMessages(prev => {
        const newMap = new Map(prev);
        newMap.delete(message.id);
        return newMap;
      });
    };
    
    const handleMessageAdded = ({ message }: any) => {
      setMessages(prev => [...prev, message]);
    };
    
    const handleToolNotification = (notification: ToolNotification) => {
      setToolNotifications(prev => [...prev, notification]);
    };
    
    sessionManager.on('message-streaming', handleMessageStreaming);
    sessionManager.on('message-complete', handleMessageComplete);
    sessionManager.on('message-added', handleMessageAdded);
    sessionManager.on('tool-notification', handleToolNotification);
    
    return () => {
      sessionManager.off('message-streaming', handleMessageStreaming);
      sessionManager.off('message-complete', handleMessageComplete);
      sessionManager.off('message-added', handleMessageAdded);
      sessionManager.off('tool-notification', handleToolNotification);
    };
  }, [client]);
  
  const editMessage = useCallback(async (messageId: string, content: string) => {
    const editManager = new MessageEditManager(
      client.getSessionManager(),
      client
    );
    await editManager.editMessage(messageId, content);
  }, [client]);
  
  // Combine finalized and streaming messages for display
  const allMessages = useMemo(() => {
    const combined = [...messages];
    streamingMessages.forEach(streamingMsg => {
      if (!messages.find(m => m.id === streamingMsg.id)) {
        combined.push(streamingMsg as EnhancedMessage);
      }
    });
    return combined;
  }, [messages, streamingMessages]);
  
  return {
    messages: allMessages,
    streamingMessages: Array.from(streamingMessages.values()),
    toolNotifications,
    editMessage
  };
};
```

## Implementation Phases

### Phase 1: Core SDK Components
1. Implement DualMessageBuilder with parallel accumulation
2. Update EventStreamProcessor with:
   - Event filtering for ignored types
   - anthropic_user_message handler
   - Sub-session detection
3. Enhance SessionManager with streaming support

### Phase 2: Data Structure Updates
1. Add multiModalContent field to EnhancedMessage
2. Update normalization to preserve images
3. Add sub-session tracking

### Phase 3: Basic UI Components
1. Create ImageDisplay component
2. Create MultiModalContent component
3. Add SubSessionIndicator component
4. Update Message component for MultiModal

### Phase 4: Advanced Features
1. Implement parallel thought/text display
2. Add message editing with MultiModal support
3. Create comprehensive test suite
4. Add performance optimizations

## Testing Strategy

### Unit Tests
- DualMessageBuilder parallel accumulation
- Event filtering in EventStreamProcessor
- MultiModal content preservation
- Sub-session detection logic
- Image data handling

### Integration Tests
- Full event stream with thought + text
- MultiModal user message flow
- Sub-session message handling
- Tool usage with parallel messages

### E2E Tests
- Complete conversation with images
- Agent-to-agent conversation display
- Message editing with MultiModal content
- Performance with large images

## Performance Considerations

1. **Image Optimization**: Lazy load images, use thumbnails for initial display
2. **Message Virtualization**: Implement virtual scrolling for long conversations
3. **Streaming Optimization**: Batch streaming updates to avoid excessive re-renders
4. **Memory Management**: Clean up base64 image data when messages scroll out of view
5. **Sub-session Filtering**: Provide UI toggle to show/hide sub-session messages

## Security Considerations

1. **Image Validation**: Validate base64 image data before rendering
2. **Content Security**: Sanitize all HTML/SVG content
3. **Size Limits**: Enforce maximum image size limits
4. **MIME Type Validation**: Verify image MIME types match content

## Migration Notes

### Breaking Changes from v1.0
1. MessageBuilder replaced with DualMessageBuilder
2. New multiModalContent field in EnhancedMessage
3. Additional event handlers required (anthropic_user_message)
4. Sub-session support requires UI updates

### Backward Compatibility
- Text-only messages continue to work unchanged
- Existing tool call handling remains compatible
- SessionManager API mostly unchanged (additions only)

## Conclusion

This redesigned architecture addresses all critical gaps identified in the original design:
- **Parallel accumulation** of thought and text through DualMessageBuilder
- **MultiModal content** preservation and display
- **Event filtering** for ignored event types
- **Sub-session handling** for agent-to-agent conversations
- **Real-time user message** display support

The modular approach maintains separation of concerns while providing the flexibility needed for the complex real-world event patterns observed in the Agent C Realtime system.
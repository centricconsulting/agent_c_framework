# Chat Messages & Event Stream UI Components Design

## Executive Summary

This document outlines the UI component architecture for the Agent C Realtime chat messages and interaction event stream, designed in collaboration between Levi (UI/UX) and Kris (SDK Core). The design follows CenSuite design patterns while integrating with the SDK's event stream processing capabilities.

### Recent Updates (Phase 2 Recovery)

**Problem Discovered:** The initial implementation incorrectly assumed message content would always be simple strings. In reality, the server can send complex content structures:
- Content can be a string OR an array of ContentBlockParam objects
- ContentBlockParam arrays can include text, images, tool use, tool results, and nested content
- Different LLM vendors (Anthropic, OpenAI) use different message formats

**Solution Implemented:** 
- SDK now handles all vendor-specific parsing and normalization
- UI receives simplified ContentPart types that are vendor-agnostic
- New UI components handle each ContentPart type appropriately
- Graceful fallbacks for unknown or future content types

## Architecture Overview

### Component-SDK Integration Model

```
┌─────────────────────────────────────────────────────────┐
│                    UI Layer (Levi)                       │
├─────────────────────────────────────────────────────────┤
│  ChatMessagesView                                        │
│    ├── MessageList                                       │
│    │   ├── Message (user/assistant/thought)             │
│    │   │   └── MessageContentRenderer [NEW]            │
│    │   │       ├── TextContentRenderer                 │
│    │   │       ├── ImageContentRenderer                │
│    │   │       ├── ToolUseContentRenderer              │
│    │   │       └── ToolResultContentRenderer           │
│    │   ├── MessageFooter                                │
│    │   └── MessageActions                               │
│    ├── ToolNotificationContainer                        │
│    ├── RichMediaDisplay                                 │
│    └── SystemNotificationContainer                      │
├─────────────────────────────────────────────────────────┤
│                  Integration Layer                       │
│            useEventStream | useMessageManagement        │
├─────────────────────────────────────────────────────────┤
│                   SDK Layer (Kris)                       │
│    EventStream | ContentNormalizer | SessionManager     │
└─────────────────────────────────────────────────────────┘
```

## Content Type Handling

### SDK-Normalized Content Types

The SDK normalizes complex server message content into simplified types for the UI layer:

```typescript
// Content from SDK can be:
type MessageContent = string | ContentPart[] | null;

// ContentPart types the UI must handle:
type ContentPart = 
  | TextContentPart      // { type: 'text', text: string }
  | ImageContentPart     // { type: 'image', source: ImageSource }
  | ToolUseContentPart   // { type: 'tool_use', id, name, input }
  | ToolResultContentPart // { type: 'tool_result', tool_use_id, content, is_error? }
```

### UI Content Rendering Strategy

The UI layer receives normalized content from the SDK and must render each ContentPart type appropriately:

1. **String Content**: Render directly with Markdown support
2. **ContentPart Arrays**: Iterate and render each part based on type
3. **Null Content**: Handle gracefully (e.g., messages with only metadata)

## Core UI Components

### 1. ChatMessagesView (Container)

Primary container that orchestrates all message-related UI components.

```tsx
interface ChatMessagesViewProps {
  sessionId?: string
  className?: string
}

export const ChatMessagesView: React.FC<ChatMessagesViewProps> = ({
  sessionId,
  className
}) => {
  // SDK Integration
  const {
    messages,
    streamingMessage,
    toolNotifications,
    mediaEvents,
    systemNotifications
  } = useEventStream()
  
  const {
    editMessage,
    deleteMessage,
    resendFromMessage,
    operationStatus
  } = useMessageManagement()
  
  // UI State
  const [selectedMessageId, setSelectedMessageId] = useState<string | null>(null)
  const scrollAreaRef = useRef<HTMLDivElement>(null)
  const { scrollToBottom, isAtBottom } = useScrollAnchor(scrollAreaRef)
  
  return (
    <div className={cn("flex flex-col h-full", className)}>
      {/* Tool Notifications Bar */}
      <ToolNotificationContainer 
        notifications={toolNotifications}
        className="shrink-0"
      />
      
      {/* Messages Scroll Area */}
      <ScrollArea 
        ref={scrollAreaRef}
        className="flex-1 px-4"
      >
        <MessageList>
          {messages.map((message) => (
            <MessageWrapper
              key={message.id}
              message={message}
              isSelected={selectedMessageId === message.id}
              operationStatus={operationStatus.get(message.id)}
              onEdit={editMessage}
              onDelete={deleteMessage}
              onResend={resendFromMessage}
              onSelect={setSelectedMessageId}
            />
          ))}
          
          {/* Streaming Message */}
          {streamingMessage && (
            <Message
              message={streamingMessage}
              isStreaming={true}
            />
          )}
          
          {/* Rich Media Events */}
          {mediaEvents.map((event, index) => (
            <RichMediaDisplay 
              key={`media-${index}`}
              event={event}
            />
          ))}
        </MessageList>
        
        <ScrollAnchor />
      </ScrollArea>
      
      {/* System Notifications */}
      <SystemNotificationContainer
        notifications={systemNotifications}
        className="absolute bottom-4 right-4"
      />
      
      {/* Scroll to Bottom FAB */}
      {!isAtBottom && (
        <Button
          size="icon"
          variant="secondary"
          className="absolute bottom-20 right-4 rounded-full shadow-lg"
          onClick={scrollToBottom}
        >
          <ArrowDown className="h-4 w-4" />
        </Button>
      )}
    </div>
  )
}
```

### 2. Message Component

Handles all message types with proper role-based rendering and complex content support.

```tsx
interface MessageProps {
  message: EnhancedMessage  // content can be string | ContentPart[] | null
  isStreaming?: boolean
  isSelected?: boolean
  operationStatus?: 'pending' | 'success' | 'error'
  onEdit?: (id: string, content: string) => void
  onDelete?: (id: string) => void
  onResend?: (id: string) => void
  onSelect?: (id: string) => void
}

export const Message: React.FC<MessageProps> = ({
  message,
  isStreaming = false,
  isSelected = false,
  operationStatus,
  onEdit,
  onDelete,
  onResend,
  onSelect
}) => {
  const [isHovered, setIsHovered] = useState(false)
  const [isEditing, setIsEditing] = useState(false)
  const [editContent, setEditContent] = useState(message.content)
  
  // Special handling for thought messages
  if (message.isThought) {
    return (
      <ThoughtMessage 
        message={message}
        isStreaming={isStreaming}
      />
    )
  }
  
  const isUser = message.role === 'user'
  const isAssistant = message.role === 'assistant'
  
  return (
    <div
      className={cn(
        "group relative flex gap-3 py-3 px-2 rounded-lg transition-colors",
        isUser && "flex-row-reverse",
        isSelected && "bg-muted/30",
        isHovered && "bg-muted/10",
        operationStatus === 'pending' && "opacity-60"
      )}
      onMouseEnter={() => setIsHovered(true)}
      onMouseLeave={() => setIsHovered(false)}
      onClick={() => onSelect?.(message.id)}
      role="article"
      aria-label={`${message.role} message`}
    >
      {/* Avatar */}
      <MessageAvatar role={message.role} />
      
      {/* Content Container */}
      <div className={cn(
        "flex-1 max-w-[75ch] space-y-2",
        isUser && "flex flex-col items-end"
      )}>
        {/* Message Bubble */}
        <div className={cn(
          "relative rounded-xl transition-all duration-200",
          isUser ? "bg-muted px-4 py-2.5" : "bg-background",
          isStreaming && "after:content-[''] after:inline-block after:w-1.5 after:h-4 after:ml-1 after:bg-current after:animate-pulse after:rounded-full"
        )}>
          {isEditing ? (
            <MessageEditor
              content={editContent}
              onChange={setEditContent}
              onSave={() => {
                onEdit?.(message.id, editContent)
                setIsEditing(false)
              }}
              onCancel={() => {
                setEditContent(getStringContent(message.content))
                setIsEditing(false)
              }}
            />
          ) : (
            <MessageContentRenderer
              content={message.content}
              role={message.role}
              className="text-[0.9375rem] leading-6 tracking-tight"
            />
          )}
          
          {/* User Message Hover Actions */}
          {isUser && !isEditing && isHovered && (
            <MessageHoverActions
              onEdit={() => setIsEditing(true)}
              onDelete={() => onDelete?.(message.id)}
              className="absolute -bottom-1 right-2"
            />
          )}
        </div>
        
        {/* Message Footer */}
        {isAssistant && !isStreaming && (
          <MessageFooter
            message={message}
            onResend={() => onResend?.(message.id)}
          />
        )}
        
        {/* Operation Status Indicator */}
        {operationStatus && (
          <div className="flex items-center gap-1 text-xs text-muted-foreground">
            {operationStatus === 'pending' && (
              <>
                <Loader2 className="h-3 w-3 animate-spin" />
                <span>Updating...</span>
              </>
            )}
            {operationStatus === 'error' && (
              <>
                <AlertCircle className="h-3 w-3 text-destructive" />
                <span className="text-destructive">Failed to update</span>
              </>
            )}
          </div>
        )}
      </div>
    </div>
  )
}
```

### 3. MessageContentRenderer Component

Renders message content based on type (string, ContentPart[], or null).

```tsx
interface MessageContentRendererProps {
  content: MessageContent  // string | ContentPart[] | null
  role: 'user' | 'assistant'
  className?: string
}

export const MessageContentRenderer: React.FC<MessageContentRendererProps> = ({
  content,
  role,
  className
}) => {
  // Handle null content
  if (content === null) {
    return (
      <div className={cn("text-muted-foreground italic", className)}>
        <span>No content available</span>
      </div>
    )
  }
  
  // Handle string content
  if (typeof content === 'string') {
    return (
      <div className={className}>
        <MarkdownRenderer 
          content={content}
          className={cn(
            "prose prose-sm max-w-none",
            role === 'user' && "prose-invert"
          )}
        />
      </div>
    )
  }
  
  // Handle ContentPart array
  return (
    <div className={cn("space-y-3", className)}>
      {content.map((part, index) => (
        <ContentPartRenderer
          key={`content-part-${index}`}
          part={part}
          role={role}
        />
      ))}
    </div>
  )
}

// Sub-component for rendering individual content parts
const ContentPartRenderer: React.FC<{
  part: ContentPart
  role: 'user' | 'assistant'
}> = ({ part, role }) => {
  switch (part.type) {
    case 'text':
      return (
        <TextContentRenderer 
          content={part.text}
          role={role}
        />
      )
    
    case 'image':
      return (
        <ImageContentRenderer
          source={part.source}
        />
      )
    
    case 'tool_use':
      return (
        <ToolUseContentRenderer
          id={part.id}
          name={part.name}
          input={part.input}
        />
      )
    
    case 'tool_result':
      return (
        <ToolResultContentRenderer
          toolUseId={part.tool_use_id}
          content={part.content}
          isError={part.is_error}
        />
      )
    
    default:
      // Graceful handling of unknown content types
      return (
        <UnknownContentRenderer
          type={(part as any).type}
        />
      )
  }
}
```

### 4. Content Type Renderers

Specialized components for each content type:

#### TextContentRenderer

```tsx
const TextContentRenderer: React.FC<{
  content: string
  role: 'user' | 'assistant'
}> = ({ content, role }) => {
  return (
    <MarkdownRenderer 
      content={content}
      className={cn(
        "prose prose-sm max-w-none",
        role === 'user' && "prose-invert"
      )}
    />
  )
}
```

#### ImageContentRenderer

```tsx
const ImageContentRenderer: React.FC<{
  source: ImageSource
}> = ({ source }) => {
  const [loadError, setLoadError] = useState(false)
  const [isExpanded, setIsExpanded] = useState(false)
  
  const imageSrc = useMemo(() => {
    if (source.type === 'url') {
      return source.url
    }
    if (source.type === 'base64' && source.data) {
      const mediaType = source.media_type || 'image/png'
      return `data:${mediaType};base64,${source.data}`
    }
    return null
  }, [source])
  
  if (!imageSrc || loadError) {
    return (
      <div className="flex items-center gap-2 p-4 bg-muted/30 rounded-lg border border-border">
        <ImageOff className="h-5 w-5 text-muted-foreground" />
        <span className="text-sm text-muted-foreground">
          {loadError ? 'Failed to load image' : 'Invalid image source'}
        </span>
      </div>
    )
  }
  
  return (
    <div className="relative group">
      <button
        onClick={() => setIsExpanded(!isExpanded)}
        className={cn(
          "relative overflow-hidden rounded-lg border border-border",
          "transition-all duration-200 cursor-pointer",
          "hover:border-primary/50",
          isExpanded ? "max-w-full" : "max-w-md"
        )}
        aria-label={isExpanded ? "Collapse image" : "Expand image"}
      >
        <img
          src={imageSrc}
          alt="Message attachment"
          className={cn(
            "block w-full h-auto",
            !isExpanded && "max-h-64 object-cover"
          )}
          onError={() => setLoadError(true)}
        />
        <div className="absolute top-2 right-2 opacity-0 group-hover:opacity-100 transition-opacity">
          <div className="flex items-center gap-1 bg-background/90 backdrop-blur-sm rounded-md px-2 py-1">
            {isExpanded ? (
              <>
                <Minimize2 className="h-3 w-3" />
                <span className="text-xs">Collapse</span>
              </>
            ) : (
              <>
                <Maximize2 className="h-3 w-3" />
                <span className="text-xs">Expand</span>
              </>
            )}
          </div>
        </div>
      </button>
      {source.media_type && (
        <div className="mt-1 text-xs text-muted-foreground">
          {source.media_type}
        </div>
      )}
    </div>
  )
}
```

#### ToolUseContentRenderer

```tsx
const ToolUseContentRenderer: React.FC<{
  id: string
  name: string
  input: Record<string, any>
}> = ({ id, name, input }) => {
  const [isExpanded, setIsExpanded] = useState(false)
  const [copied, setCopied] = useState(false)
  
  const handleCopy = () => {
    const content = JSON.stringify({ id, name, input }, null, 2)
    navigator.clipboard.writeText(content)
    setCopied(true)
    setTimeout(() => setCopied(false), 2000)
  }
  
  return (
    <div className="rounded-lg border border-blue-200 bg-blue-50/50 dark:border-blue-800 dark:bg-blue-950/30">
      <button
        onClick={() => setIsExpanded(!isExpanded)}
        className="flex items-center justify-between w-full px-3 py-2 text-left hover:bg-blue-100/50 dark:hover:bg-blue-900/30 transition-colors"
        aria-expanded={isExpanded}
        aria-label={`Tool use: ${name}`}
      >
        <div className="flex items-center gap-2">
          <Wrench className="h-4 w-4 text-blue-600 dark:text-blue-400" />
          <span className="text-sm font-medium text-blue-900 dark:text-blue-100">
            Using {name}
          </span>
        </div>
        <ChevronRight className={cn(
          "h-4 w-4 text-blue-600 dark:text-blue-400 transition-transform",
          isExpanded && "rotate-90"
        )} />
      </button>
      
      <AnimatePresence>
        {isExpanded && (
          <motion.div
            initial={{ height: 0, opacity: 0 }}
            animate={{ height: "auto", opacity: 1 }}
            exit={{ height: 0, opacity: 0 }}
            transition={{ duration: 0.15 }}
            className="overflow-hidden"
          >
            <div className="px-3 pb-3 space-y-2">
              <div className="flex items-center justify-between">
                <span className="text-xs text-muted-foreground">Tool ID: {id}</span>
                <Button
                  variant="ghost"
                  size="sm"
                  className="h-6 px-2 text-xs"
                  onClick={handleCopy}
                >
                  {copied ? (
                    <>
                      <Check className="h-3 w-3 mr-1" />
                      Copied
                    </>
                  ) : (
                    <>
                      <Copy className="h-3 w-3 mr-1" />
                      Copy
                    </>
                  )}
                </Button>
              </div>
              <div className="bg-background/60 rounded p-2 border border-border/50">
                <pre className="text-xs overflow-auto">
                  <code>{JSON.stringify(input, null, 2)}</code>
                </pre>
              </div>
            </div>
          </motion.div>
        )}
      </AnimatePresence>
    </div>
  )
}
```

#### ToolResultContentRenderer

```tsx
const ToolResultContentRenderer: React.FC<{
  toolUseId: string
  content: string | ContentPart[]
  isError?: boolean
}> = ({ toolUseId, content, isError }) => {
  const [isExpanded, setIsExpanded] = useState(!isError) // Auto-expand successful results
  
  const statusColor = isError 
    ? "border-destructive bg-destructive/10" 
    : "border-green-200 bg-green-50/50 dark:border-green-800 dark:bg-green-950/30"
  
  const statusIcon = isError 
    ? <XCircle className="h-4 w-4 text-destructive" />
    : <CheckCircle className="h-4 w-4 text-green-600 dark:text-green-400" />
  
  const statusText = isError ? "Tool Error" : "Tool Result"
  
  return (
    <div className={cn("rounded-lg border", statusColor)}>
      <button
        onClick={() => setIsExpanded(!isExpanded)}
        className={cn(
          "flex items-center justify-between w-full px-3 py-2 text-left transition-colors",
          isError 
            ? "hover:bg-destructive/20" 
            : "hover:bg-green-100/50 dark:hover:bg-green-900/30"
        )}
        aria-expanded={isExpanded}
        aria-label={`${statusText} for tool ${toolUseId}`}
      >
        <div className="flex items-center gap-2">
          {statusIcon}
          <span className={cn(
            "text-sm font-medium",
            isError 
              ? "text-destructive" 
              : "text-green-900 dark:text-green-100"
          )}>
            {statusText}
          </span>
          <span className="text-xs text-muted-foreground">
            (ID: {toolUseId.slice(0, 8)}...)
          </span>
        </div>
        <ChevronRight className={cn(
          "h-4 w-4 transition-transform",
          isError ? "text-destructive" : "text-green-600 dark:text-green-400",
          isExpanded && "rotate-90"
        )} />
      </button>
      
      <AnimatePresence>
        {isExpanded && (
          <motion.div
            initial={{ height: 0, opacity: 0 }}
            animate={{ height: "auto", opacity: 1 }}
            exit={{ height: 0, opacity: 0 }}
            transition={{ duration: 0.15 }}
            className="overflow-hidden"
          >
            <div className="px-3 pb-3">
              <div className="bg-background/60 rounded p-3 border border-border/50">
                {typeof content === 'string' ? (
                  <pre className="text-sm whitespace-pre-wrap">{content}</pre>
                ) : (
                  // Recursively render nested ContentPart arrays
                  <div className="space-y-2">
                    {content.map((part, index) => (
                      <ContentPartRenderer
                        key={`tool-result-${index}`}
                        part={part}
                        role="assistant"
                      />
                    ))}
                  </div>
                )}
              </div>
            </div>
          </motion.div>
        )}
      </AnimatePresence>
    </div>
  )
}
```

#### UnknownContentRenderer

```tsx
const UnknownContentRenderer: React.FC<{
  type: string
}> = ({ type }) => {
  return (
    <Alert variant="default" className="border-yellow-200 bg-yellow-50/50">
      <AlertTriangle className="h-4 w-4 text-yellow-600" />
      <AlertTitle>Unknown Content Type</AlertTitle>
      <AlertDescription>
        Content type "{type}" is not yet supported in this version.
      </AlertDescription>
    </Alert>
  )
}
```

### 5. ThoughtMessage Component

Collapsible thought messages following Claude's pattern.

```tsx
interface ThoughtMessageProps {
  message: EnhancedMessage
  isStreaming?: boolean
}

export const ThoughtMessage: React.FC<ThoughtMessageProps> = ({
  message,
  isStreaming = false
}) => {
  const [isExpanded, setIsExpanded] = useState(false)
  const [copied, setCopied] = useState(false)
  
  // Extract first line for preview
  const firstLine = useMemo(() => {
    const lines = message.content.split('\n')
    const first = lines[0] || ''
    return first.length > 80 ? `${first.slice(0, 77)}...` : first
  }, [message.content])
  
  const handleCopy = useCallback(() => {
    navigator.clipboard.writeText(message.content)
    setCopied(true)
    setTimeout(() => setCopied(false), 2000)
  }, [message.content])
  
  return (
    <div className={cn(
      "rounded-lg border border-border/50 my-2",
      "transition-all duration-200 ease-out",
      "hover:bg-muted/20 hover:border-border/70"
    )}>
      <button
        className={cn(
          "group/thought flex w-full items-center justify-between",
          "gap-4 rounded-lg px-3 py-2 h-[2.625rem]",
          "text-muted-foreground hover:text-foreground",
          "transition-colors duration-200 cursor-pointer",
          "focus-visible:outline-none focus-visible:ring-2",
          "focus-visible:ring-ring focus-visible:ring-offset-2"
        )}
        onClick={() => setIsExpanded(!isExpanded)}
        aria-expanded={isExpanded}
        aria-label={isExpanded ? "Collapse thought" : "Expand thought"}
      >
        <div className="flex items-center gap-2 min-w-0">
          <Brain className="h-4 w-4 shrink-0 text-muted-foreground/70" />
          <span className="text-sm leading-tight truncate">
            {isExpanded ? "Thought process" : firstLine}
          </span>
        </div>
        
        <div className="flex items-center gap-1.5 shrink-0">
          {isStreaming && (
            <Loader2 className="h-3 w-3 animate-spin text-muted-foreground/70" />
          )}
          <ChevronRight className={cn(
            "h-4 w-4 transition-transform duration-200",
            isExpanded && "rotate-90"
          )} />
        </div>
      </button>
      
      {/* Expandable Content */}
      <AnimatePresence>
        {isExpanded && (
          <motion.div
            initial={{ height: 0, opacity: 0 }}
            animate={{ height: "auto", opacity: 1 }}
            exit={{ height: 0, opacity: 0 }}
            transition={{ duration: 0.2, ease: "easeOut" }}
            className="overflow-hidden"
          >
            <div 
              className="px-3 pb-3 text-sm text-muted-foreground"
              style={{
                maskImage: "linear-gradient(transparent 0%, black 1rem, black calc(100% - 1rem), transparent 100%)"
              }}
            >
              <MarkdownRenderer 
                content={message.content}
                className="prose prose-sm prose-muted max-w-none"
              />
              
              {/* Thought Footer */}
              {message.metadata?.outputTokens && (
                <div className="mt-3 pt-2 border-t border-border/30 flex items-center justify-between">
                  <span className="text-xs text-muted-foreground/70">
                    {message.metadata.outputTokens} tokens
                  </span>
                  <Button
                    variant="ghost"
                    size="sm"
                    className="h-6 px-2 text-xs"
                    onClick={handleCopy}
                  >
                    {copied ? (
                      <>
                        <Check className="h-3 w-3 mr-1" />
                        Copied
                      </>
                    ) : (
                      <>
                        <Copy className="h-3 w-3 mr-1" />
                        Copy
                      </>
                    )}
                  </Button>
                </div>
              )}
            </div>
          </motion.div>
        )}
      </AnimatePresence>
    </div>
  )
}
```

### 6. MessageFooter Component

Displays token counts, tool calls, and actions for assistant messages.

```tsx
interface MessageFooterProps {
  message: EnhancedMessage
  onResend?: () => void
}

export const MessageFooter: React.FC<MessageFooterProps> = ({
  message,
  onResend
}) => {
  const [showToolCalls, setShowToolCalls] = useState(false)
  const [copied, setCopied] = useState(false)
  
  const handleCopy = () => {
    navigator.clipboard.writeText(message.content)
    setCopied(true)
    setTimeout(() => setCopied(false), 2000)
  }
  
  const hasToolCalls = message.toolCalls && message.toolCalls.length > 0
  const hasTokenCounts = message.metadata?.inputTokens || message.metadata?.outputTokens
  
  if (!hasTokenCounts && !hasToolCalls) return null
  
  return (
    <div className="space-y-2">
      {/* Primary Footer Row */}
      <div className="flex items-center gap-3 text-xs text-muted-foreground">
        {/* Token Counts */}
        {hasTokenCounts && (
          <div className="flex items-center gap-1">
            <Hash className="h-3 w-3" />
            {message.metadata?.inputTokens && (
              <>
                <span>{message.metadata.inputTokens}</span>
                <ArrowRight className="h-3 w-3" />
              </>
            )}
            <span>{message.metadata?.outputTokens || 0}</span>
          </div>
        )}
        
        {/* Tool Calls Toggle */}
        {hasToolCalls && (
          <Button
            variant="ghost"
            size="sm"
            className="h-6 px-2 gap-1 -ml-2"
            onClick={() => setShowToolCalls(!showToolCalls)}
          >
            <Wrench className="h-3 w-3" />
            <span>{message.toolCalls!.length} tool{message.toolCalls!.length > 1 ? 's' : ''}</span>
            <ChevronDown className={cn(
              "h-3 w-3 transition-transform",
              showToolCalls && "rotate-180"
            )} />
          </Button>
        )}
        
        {/* Copy Button */}
        <Button
          variant="ghost"
          size="sm"
          className="h-6 px-2 gap-1 ml-auto"
          onClick={handleCopy}
        >
          {copied ? (
            <>
              <Check className="h-3 w-3" />
              <span>Copied</span>
            </>
          ) : (
            <>
              <Copy className="h-3 w-3" />
              <span>Copy</span>
            </>
          )}
        </Button>
        
        {/* Resend Button (if applicable) */}
        {onResend && (
          <Button
            variant="ghost"
            size="sm"
            className="h-6 px-2 gap-1"
            onClick={onResend}
          >
            <RefreshCw className="h-3 w-3" />
            <span>Regenerate</span>
          </Button>
        )}
      </div>
      
      {/* Expandable Tool Calls */}
      <AnimatePresence>
        {showToolCalls && hasToolCalls && (
          <motion.div
            initial={{ height: 0, opacity: 0 }}
            animate={{ height: "auto", opacity: 1 }}
            exit={{ height: 0, opacity: 0 }}
            transition={{ duration: 0.15 }}
          >
            <ToolCallDisplay 
              toolCalls={message.toolCalls!}
              className="mt-2"
            />
          </motion.div>
        )}
      </AnimatePresence>
    </div>
  )
}
```

### 7. ToolNotification Component

Real-time tool usage notifications.

```tsx
interface ToolNotificationProps {
  notification: {
    id: string
    toolName: string
    status: 'preparing' | 'executing' | 'complete'
    timestamp: string
  }
}

export const ToolNotification: React.FC<ToolNotificationProps> = ({
  notification
}) => {
  const statusConfig = {
    preparing: {
      text: `Preparing ${notification.toolName}...`,
      icon: Clock,
      className: "text-yellow-600 bg-yellow-50 border-yellow-200"
    },
    executing: {
      text: `Using ${notification.toolName}...`,
      icon: Loader2,
      className: "text-blue-600 bg-blue-50 border-blue-200",
      iconClassName: "animate-spin"
    },
    complete: {
      text: `Completed ${notification.toolName}`,
      icon: Check,
      className: "text-green-600 bg-green-50 border-green-200"
    }
  }
  
  const config = statusConfig[notification.status]
  const Icon = config.icon
  
  return (
    <motion.div
      initial={{ opacity: 0, y: -20 }}
      animate={{ opacity: 1, y: 0 }}
      exit={{ opacity: 0, y: -20 }}
      transition={{ duration: 0.2 }}
      className={cn(
        "flex items-center gap-2 px-3 py-1.5 rounded-md border",
        "text-sm font-medium",
        config.className
      )}
      role="status"
      aria-live="polite"
    >
      <Icon className={cn("h-3.5 w-3.5", config.iconClassName)} />
      <span>{config.text}</span>
    </motion.div>
  )
}

export const ToolNotificationContainer: React.FC<{
  notifications: Map<string, ToolNotification>
  className?: string
}> = ({ notifications, className }) => {
  const sortedNotifications = Array.from(notifications.values())
    .sort((a, b) => new Date(b.timestamp).getTime() - new Date(a.timestamp).getTime())
    .slice(0, 3) // Show max 3 notifications
  
  if (sortedNotifications.length === 0) return null
  
  return (
    <div className={cn(
      "flex flex-col gap-2 p-3 border-b border-border bg-muted/30",
      className
    )}>
      <AnimatePresence mode="popLayout">
        {sortedNotifications.map(notification => (
          <ToolNotification
            key={notification.id}
            notification={notification}
          />
        ))}
      </AnimatePresence>
    </div>
  )
}
```

### 8. RichMediaDisplay Component

Handles HTML and SVG content from RenderMediaEvent.

```tsx
interface RichMediaDisplayProps {
  event: RenderMediaEvent
  className?: string
}

export const RichMediaDisplay: React.FC<RichMediaDisplayProps> = ({
  event,
  className
}) => {
  const [error, setError] = useState<string | null>(null)
  const [isExpanded, setIsExpanded] = useState(false)
  const containerRef = useRef<HTMLDivElement>(null)
  
  // Use SDK's sanitization utilities
  const sanitizedContent = useMemo(() => {
    try {
      if (event.content_type === 'text/html') {
        return MediaSanitizer.sanitizeHTML(event.content)
      }
      if (event.content_type === 'image/svg+xml') {
        return MediaSanitizer.sanitizeSVG(event.content)
      }
      return event.content
    } catch (err) {
      setError('Failed to render media content safely')
      return null
    }
  }, [event.content, event.content_type])
  
  if (error) {
    return (
      <Alert variant="destructive" className={className}>
        <AlertCircle className="h-4 w-4" />
        <AlertDescription>{error}</AlertDescription>
      </Alert>
    )
  }
  
  return (
    <div className={cn("my-4 space-y-2", className)}>
      {/* Media Container */}
      <div className={cn(
        "relative rounded-lg border border-border overflow-hidden",
        "bg-muted/20 transition-all duration-200",
        event.content_type === 'image/svg+xml' && "flex items-center justify-center p-4"
      )}>
        {/* Expand/Collapse for large content */}
        {!isExpanded && (
          <div className="absolute inset-x-0 bottom-0 h-20 bg-gradient-to-t from-background to-transparent" />
        )}
        
        <div
          ref={containerRef}
          className={cn(
            "overflow-auto",
            isExpanded ? "max-h-none" : "max-h-96"
          )}
          dangerouslySetInnerHTML={{ __html: sanitizedContent || '' }}
          aria-label="Rich media content"
        />
        
        {/* Expand Toggle */}
        <Button
          variant="ghost"
          size="sm"
          className="absolute bottom-2 right-2"
          onClick={() => setIsExpanded(!isExpanded)}
        >
          {isExpanded ? (
            <>
              <ChevronUp className="h-3 w-3 mr-1" />
              Collapse
            </>
          ) : (
            <>
              <ChevronDown className="h-3 w-3 mr-1" />
              Expand
            </>
          )}
        </Button>
      </div>
      
      {/* Attribution Footer */}
      <div className="flex items-center gap-2 text-xs text-muted-foreground px-1">
        <Code className="h-3 w-3" />
        <span>
          Generated by {event.sent_by_class}.{event.sent_by_function}
        </span>
      </div>
    </div>
  )
}
```

### 9. SystemNotification Component

System messages and error notifications.

```tsx
interface SystemNotificationProps {
  event: SystemMessageEvent | ErrorEvent
  onDismiss?: () => void
}

export const SystemNotification: React.FC<SystemNotificationProps> = ({
  event,
  onDismiss
}) => {
  const severity = 'severity' in event ? event.severity : 'error'
  
  const config = {
    error: {
      variant: 'destructive' as const,
      icon: XCircle,
      title: 'Error'
    },
    warning: {
      variant: 'default' as const,
      icon: AlertTriangle,
      title: 'Warning'
    },
    info: {
      variant: 'default' as const,
      icon: Info,
      title: 'Info'
    }
  }
  
  const { variant, icon: Icon, title } = config[severity] || config.info
  const content = 'content' in event ? event.content : event.message
  
  return (
    <motion.div
      initial={{ opacity: 0, x: 20 }}
      animate={{ opacity: 1, x: 0 }}
      exit={{ opacity: 0, x: 20 }}
      transition={{ duration: 0.2 }}
    >
      <Alert variant={variant} className="w-80">
        <Icon className="h-4 w-4" />
        <AlertTitle>{title}</AlertTitle>
        <AlertDescription className="pr-6">
          {content}
        </AlertDescription>
        {onDismiss && (
          <button
            className="absolute top-2 right-2 p-1 rounded hover:bg-muted"
            onClick={onDismiss}
            aria-label="Dismiss notification"
          >
            <X className="h-3 w-3" />
          </button>
        )}
      </Alert>
    </motion.div>
  )
}

export const SystemNotificationContainer: React.FC<{
  notifications: SystemMessageEvent[]
  className?: string
}> = ({ notifications, className }) => {
  const [dismissed, setDismissed] = useState<Set<string>>(new Set())
  
  const visibleNotifications = notifications
    .filter(n => !dismissed.has(n.id || ''))
    .slice(-3) // Show max 3 notifications
  
  return (
    <div className={cn(
      "fixed z-50 flex flex-col gap-2",
      className
    )}>
      <AnimatePresence mode="popLayout">
        {visibleNotifications.map((notification, index) => (
          <SystemNotification
            key={notification.id || index}
            event={notification}
            onDismiss={() => {
              if (notification.id) {
                setDismissed(prev => new Set(prev).add(notification.id!))
              }
            }}
          />
        ))}
      </AnimatePresence>
    </div>
  )
}
```

## Visual Examples & Rendering Patterns

### Content Type Visual Hierarchy

#### Text Content
```
┌─────────────────────────────────────────────┐
│ [Avatar] Message text with **markdown**     │
│          support and proper formatting.      │
│                                              │
│          • Bullet points                     │
│          • Code blocks                       │
│          • Links                             │
└─────────────────────────────────────────────┘
```

#### Mixed Content (Text + Tool Use + Tool Result)
```
┌─────────────────────────────────────────────┐
│ [Avatar] Let me search for that information │
│                                              │
│ ┌───────────────────────────────────────┐   │
│ │ 🔧 Using web_search                    │   │
│ │ ▶ {"query": "latest AI news"}         │   │
│ └───────────────────────────────────────┘   │
│                                              │
│ ┌───────────────────────────────────────┐   │
│ │ ✅ Tool Result                         │   │
│ │ ▼ Found 5 relevant articles...         │   │
│ └───────────────────────────────────────┘   │
│                                              │
│ Based on the search results, here are...    │
└─────────────────────────────────────────────┘
```

#### Image Content
```
┌─────────────────────────────────────────────┐
│ [Avatar] Here's the diagram you requested:  │
│                                              │
│ ┌───────────────────────────────────────┐   │
│ │        [Image Preview]                 │   │
│ │     Click to expand ⤢                  │   │
│ └───────────────────────────────────────┘   │
│ image/png                                    │
└─────────────────────────────────────────────┘
```

#### Error Content
```
┌─────────────────────────────────────────────┐
│ [Avatar] I'll help you with that task.      │
│                                              │
│ ┌───────────────────────────────────────┐   │
│ │ 🔧 Using calculator                    │   │
│ │ ▶ {"expression": "1/0"}               │   │
│ └───────────────────────────────────────┘   │
│                                              │
│ ┌───────────────────────────────────────┐   │
│ │ ❌ Tool Error                          │   │
│ │ ▼ Division by zero error               │   │
│ └───────────────────────────────────────┘   │
│                                              │
│ Let me recalculate that differently...      │
└─────────────────────────────────────────────┘
```

### Error Handling Strategies

#### Content Parsing Errors

```tsx
const SafeContentRenderer: React.FC<{ content: any }> = ({ content }) => {
  try {
    // Attempt to parse and render content
    if (isValidContent(content)) {
      return <MessageContentRenderer content={content} />
    }
  } catch (error) {
    console.error('Failed to render content:', error)
  }
  
  // Fallback for unparseable content
  return (
    <Alert variant="destructive">
      <AlertCircle className="h-4 w-4" />
      <AlertTitle>Content Error</AlertTitle>
      <AlertDescription>
        Unable to display message content. 
        Raw content has been logged for debugging.
      </AlertDescription>
    </Alert>
  )
}
```

#### Missing Content Handlers

```tsx
const contentTypeHandlers: Record<ContentPartType, React.FC<any>> = {
  text: TextContentRenderer,
  image: ImageContentRenderer,
  tool_use: ToolUseContentRenderer,
  tool_result: ToolResultContentRenderer,
  // Future content types can be added here
  audio: FutureAudioRenderer,
  video: FutureVideoRenderer
}

const getContentRenderer = (type: ContentPartType) => {
  return contentTypeHandlers[type] || UnknownContentRenderer
}
```

#### Graceful Degradation

```tsx
// Preserve original content for debugging
interface EnhancedMessage {
  content: MessageContent          // Normalized for UI
  originalContent?: any            // Original from server
  contentParseError?: string       // Error details if parsing failed
}

// Show debug info in development
if (process.env.NODE_ENV === 'development' && message.contentParseError) {
  console.warn('Content parse error:', {
    error: message.contentParseError,
    original: message.originalContent
  })
}
```

## Supporting Components

### MessageEditor Component

Inline editing for user messages.

```tsx
interface MessageEditorProps {
  content: string
  onChange: (content: string) => void
  onSave: () => void
  onCancel: () => void
}

export const MessageEditor: React.FC<MessageEditorProps> = ({
  content,
  onChange,
  onSave,
  onCancel
}) => {
  const textareaRef = useRef<HTMLTextAreaElement>(null)
  
  useEffect(() => {
    // Auto-focus and select all on mount
    if (textareaRef.current) {
      textareaRef.current.focus()
      textareaRef.current.select()
    }
  }, [])
  
  const handleKeyDown = (e: React.KeyboardEvent) => {
    if (e.key === 'Enter' && (e.metaKey || e.ctrlKey)) {
      e.preventDefault()
      onSave()
    } else if (e.key === 'Escape') {
      e.preventDefault()
      onCancel()
    }
  }
  
  return (
    <div className="space-y-2">
      <Textarea
        ref={textareaRef}
        value={content}
        onChange={(e) => onChange(e.target.value)}
        onKeyDown={handleKeyDown}
        className="min-h-[80px] resize-none"
        placeholder="Edit your message..."
      />
      <div className="flex items-center gap-2">
        <Button
          size="sm"
          onClick={onSave}
          disabled={!content.trim()}
        >
          Save
        </Button>
        <Button
          size="sm"
          variant="ghost"
          onClick={onCancel}
        >
          Cancel
        </Button>
        <span className="ml-auto text-xs text-muted-foreground">
          {isMac ? '⌘' : 'Ctrl'}+Enter to save, Esc to cancel
        </span>
      </div>
    </div>
  )
}
```

### ToolCallDisplay Component

Displays tool call details in an expandable format.

```tsx
interface ToolCallDisplayProps {
  toolCalls: ToolCall[]
  className?: string
}

export const ToolCallDisplay: React.FC<ToolCallDisplayProps> = ({
  toolCalls,
  className
}) => {
  const [expandedTools, setExpandedTools] = useState<Set<string>>(new Set())
  
  const toggleTool = (toolId: string) => {
    setExpandedTools(prev => {
      const next = new Set(prev)
      if (next.has(toolId)) {
        next.delete(toolId)
      } else {
        next.add(toolId)
      }
      return next
    })
  }
  
  return (
    <div className={cn(
      "rounded-lg border border-border/50 bg-muted/30 p-2 space-y-1",
      className
    )}>
      {toolCalls.map((tool) => {
        const isExpanded = expandedTools.has(tool.id)
        
        return (
          <div key={tool.id} className="rounded bg-background/50">
            <button
              className="flex items-center justify-between w-full px-2 py-1.5 text-left hover:bg-muted/50 transition-colors"
              onClick={() => toggleTool(tool.id)}
            >
              <div className="flex items-center gap-2">
                <Wrench className="h-3 w-3 text-muted-foreground" />
                <span className="text-xs font-medium">
                  {tool.function.name}
                </span>
              </div>
              <ChevronRight className={cn(
                "h-3 w-3 text-muted-foreground transition-transform",
                isExpanded && "rotate-90"
              )} />
            </button>
            
            {isExpanded && (
              <div className="px-2 pb-2 space-y-2">
                {/* Parameters */}
                {tool.function.arguments && (
                  <div className="pl-5">
                    <span className="text-xs text-muted-foreground">Parameters:</span>
                    <pre className="mt-1 p-2 bg-muted/30 rounded text-xs overflow-auto">
                      {JSON.stringify(tool.function.arguments, null, 2)}
                    </pre>
                  </div>
                )}
                
                {/* Results */}
                {tool.results && (
                  <div className="pl-5">
                    <span className="text-xs text-muted-foreground">Result:</span>
                    <pre className="mt-1 p-2 bg-muted/30 rounded text-xs overflow-auto">
                      {JSON.stringify(tool.results, null, 2)}
                    </pre>
                  </div>
                )}
              </div>
            )}
          </div>
        )
      })}
    </div>
  )
}
```

## Content Processing Best Practices

### SDK to UI Data Flow

1. **Server sends** vendor-specific message formats (Anthropic ContentBlockParam[], OpenAI format, etc.)
2. **SDK normalizes** to simplified ContentPart[] or string for UI consumption
3. **UI receives** normalized content and renders appropriately
4. **No UI-side parsing** of vendor-specific formats

### Content Normalization Rules

```typescript
// SDK handles all normalization
class ContentNormalizer {
  static normalize(content: any, vendor: string): MessageContent {
    // String content passes through
    if (typeof content === 'string') {
      return content
    }
    
    // Array content gets normalized to ContentPart[]
    if (Array.isArray(content)) {
      return content.map(block => {
        switch (block.type) {
          case 'text':
            return { type: 'text', text: block.text }
          case 'image':
            return { type: 'image', source: block.source }
          case 'tool_use':
            return {
              type: 'tool_use',
              id: block.id,
              name: block.name,
              input: block.input
            }
          case 'tool_result':
            return {
              type: 'tool_result',
              tool_use_id: block.tool_use_id,
              content: this.normalize(block.content, vendor),
              is_error: block.is_error
            }
          default:
            // Unknown types get logged but don't break
            console.warn(`Unknown content type: ${block.type}`)
            return { type: 'text', text: '[Unsupported content]' }
        }
      })
    }
    
    return null
  }
}
```

### UI Content Rendering Guidelines

1. **Always handle null content** - Some messages may only have metadata
2. **Type-check at runtime** - Use type guards for safety
3. **Provide fallbacks** - Every content type needs a fallback renderer
4. **Preserve accessibility** - All content must be screen-reader accessible
5. **Optimize for performance** - Large content arrays should be virtualized

### Content Type Guards

```typescript
// Type guards for safe rendering
const isTextContent = (part: ContentPart): part is TextContentPart => {
  return part.type === 'text' && typeof part.text === 'string'
}

const isImageContent = (part: ContentPart): part is ImageContentPart => {
  return part.type === 'image' && part.source !== undefined
}

const isToolUseContent = (part: ContentPart): part is ToolUseContentPart => {
  return part.type === 'tool_use' && 
         typeof part.id === 'string' && 
         typeof part.name === 'string'
}

const isToolResultContent = (part: ContentPart): part is ToolResultContentPart => {
  return part.type === 'tool_result' && 
         typeof part.tool_use_id === 'string'
}
```

### Accessibility Requirements

```tsx
// All content renderers must include proper ARIA labels
<div role="article" aria-label="Assistant message with tool use">
  <div role="region" aria-label="Tool invocation">
    {/* Tool use content */}
  </div>
  <div role="region" aria-label="Tool result">
    {/* Tool result content */}
  </div>
</div>

// Images must have alt text
<img 
  src={imageSrc} 
  alt={part.alt || "Image attachment"}
  aria-describedby={`image-desc-${id}`}
/>
<span id={`image-desc-${id}`} className="sr-only">
  {part.media_type || "Image"} in message
</span>

// Tool states must be announced
<div role="status" aria-live="polite">
  Tool {name} is being executed
</div>
```

## State Management Strategy

### Event Processing Flow

```typescript
// Event processing pipeline
const processEventStream = (event: ServerEvent) => {
  switch(event.type) {
    case 'text_delta':
      accumulateText(event)
      break
    case 'thought_delta':
      accumulateThought(event)
      break
    case 'tool_select_delta':
      showToolPreparation(event)
      break
    case 'tool_call':
      updateToolStatus(event)
      break
    case 'completion':
      finalizeMessage(event)
      break
    case 'render_media':
      addMediaContent(event)
      break
    case 'system_message':
    case 'error':
      showNotification(event)
      break
  }
}
```

### Optimistic Updates

```typescript
// Optimistic update pattern for message editing
const handleEditMessage = async (messageId: string, newContent: string) => {
  // 1. Optimistic UI update
  updateMessageOptimistic(messageId, newContent)
  
  // 2. Send to server
  try {
    await editMessage(messageId, newContent)
  } catch (error) {
    // 3. Revert on failure
    revertMessage(messageId)
    showError('Failed to update message')
  }
}
```

## Accessibility Features

1. **Keyboard Navigation**
   - Tab through messages
   - Enter/Space to expand collapsibles
   - Escape to close editors
   - Cmd/Ctrl+Enter to save edits

2. **Screen Reader Support**
   - Proper ARIA labels and roles
   - Live regions for dynamic updates
   - Descriptive button labels
   - Status announcements

3. **Focus Management**
   - Visible focus rings
   - Focus trapping in editors
   - Return focus after actions
   - Skip links for navigation

4. **Color Contrast**
   - WCAG AAA compliance
   - High contrast mode support
   - Semantic color usage

## Performance Optimizations

1. **Message Virtualization**
   ```tsx
   // Use react-window for large message lists
   <FixedSizeList
     height={containerHeight}
     itemCount={messages.length}
     itemSize={estimatedMessageHeight}
   >
     {MessageRow}
   </FixedSizeList>
   ```

2. **Memoization**
   ```tsx
   const MemoizedMessage = React.memo(Message, (prev, next) => {
     return prev.message.id === next.message.id &&
            prev.message.status === next.message.status &&
            prev.isSelected === next.isSelected
   })
   ```

3. **Lazy Loading**
   - Load markdown renderer on demand
   - Defer syntax highlighting
   - Lazy load heavy media content

## Implementation Phases

### Phase 1: Core Components (Immediate)
- [x] ChatMessagesView container
- [x] Basic Message component
- [x] MessageFooter with token counts
- [x] ThoughtMessage (collapsible)
- [x] Integration with useEventStream hook

### Phase 2: Interactive Features (Next Sprint)
- [ ] MessageEditor for user messages
- [ ] ToolNotification real-time updates
- [ ] ToolCallDisplay expandable view
- [ ] Message selection and actions

### Phase 3: Rich Content (Following Sprint)
- [ ] RichMediaDisplay with sanitization
- [ ] SystemNotification container
- [ ] Code syntax highlighting
- [ ] Math equation rendering

### Phase 4: Polish & Optimization
- [ ] Message virtualization
- [ ] Smooth animations
- [ ] Accessibility audit
- [ ] Performance profiling

## Testing Strategy

1. **Unit Tests**
   - Component rendering
   - Event processing logic
   - State management

2. **Integration Tests**
   - SDK hook integration
   - Event stream handling
   - Optimistic updates

3. **Accessibility Tests**
   - Keyboard navigation
   - Screen reader compatibility
   - Focus management

4. **Performance Tests**
   - Large message lists
   - Streaming performance
   - Memory leaks

## Conclusion

This design provides a comprehensive UI architecture for the chat messages and event stream, fully integrated with the SDK's capabilities. The component hierarchy is clean, maintainable, and follows CenSuite design patterns while providing excellent user experience through thoughtful interactions, accessibility, and performance optimizations.

The collaboration between UI and SDK layers ensures proper separation of concerns while maintaining tight integration for real-time updates and state management.
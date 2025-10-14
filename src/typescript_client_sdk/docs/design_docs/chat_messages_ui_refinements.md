# Chat Messages UI Components - CenSuite Design System Refinements

## Overview

This document contains the refined UI component designs from Levi, incorporating proper CenSuite design system patterns, accessibility features, and responsive design considerations. These refinements should be used when implementing the UI components described in the main design document.

## Refined UI Components

### 1. Enhanced Message Component

```tsx
export const Message: React.FC<MessageProps> = ({
  message,
  onEdit,
  onCopy,
  showTokenCounts = true
}) => {
  const [isHovered, setIsHovered] = useState(false);
  const [isEditing, setIsEditing] = useState(false);
  
  // Separate rendering by type for clarity
  if (message.type === 'thought') {
    return <ThoughtMessage message={message} />;
  }
  
  if (message.type === 'media') {
    return <RichMediaDisplay message={message} />;
  }
  
  const isUser = message.role === 'user';
  
  return (
    <div 
      className={cn(
        "group relative flex gap-3 py-4",
        isUser && "flex-row-reverse"
      )}
      onMouseEnter={() => setIsHovered(true)}
      onMouseLeave={() => setIsHovered(false)}
      role="article"
      aria-label={`${message.role} message`}
    >
      {/* Avatar */}
      <div className="flex-shrink-0">
        {isUser ? (
          <div className="h-7 w-7 rounded-full bg-primary text-primary-foreground flex items-center justify-center text-xs font-semibold">
            U
          </div>
        ) : (
          <div className="h-7 w-7 rounded-full bg-primary/10 flex items-center justify-center">
            <Bot className="h-4 w-4 text-primary" />
          </div>
        )}
      </div>
      
      {/* Content Container */}
      <div className={cn(
        "flex-1 max-w-[75ch] space-y-2",
        isUser && "flex flex-col items-end"
      )}>
        {/* Message Bubble */}
        <div className={cn(
          "relative rounded-xl px-4 py-2.5 transition-colors",
          isUser 
            ? "bg-muted inline-block" 
            : "bg-background",
          message.status === 'streaming' && "after:content-[''] after:inline-block after:w-1.5 after:h-4 after:ml-1 after:bg-current after:animate-pulse after:rounded-full"
        )}>
          {isEditing ? (
            <MessageEditor
              initialContent={message.content}
              onSave={(content) => {
                onEdit?.(message.id, content);
                setIsEditing(false);
              }}
              onCancel={() => setIsEditing(false)}
            />
          ) : (
            <div className="text-[0.9375rem] leading-6 tracking-tight">
              <MarkdownRenderer content={message.content} />
            </div>
          )}
          
          {/* User Message Edit Button (Hover State) */}
          {isUser && !isEditing && (
            <div className={cn(
              "absolute -bottom-1 right-2 transition-all duration-200",
              "translate-y-4 translate-x-1",
              isHovered && "translate-x-0.5"
            )}>
              <div className={cn(
                "rounded-lg bg-background/80 backdrop-blur-sm",
                "border border-border p-0.5 shadow-sm",
                "opacity-0 transition-opacity duration-200",
                isHovered && "opacity-100"
              )}>
                <Button
                  variant="ghost"
                  size="sm"
                  className="h-7 px-2 text-xs font-normal"
                  onClick={() => setIsEditing(true)}
                  aria-label="Edit message"
                >
                  Edit
                </Button>
              </div>
            </div>
          )}
        </div>
        
        {/* Message Footer (for assistant messages) */}
        {!isUser && message.status === 'complete' && (
          <MessageFooter
            message={message}
            onCopy={() => onCopy?.(message.content)}
            showTokenCounts={showTokenCounts}
          />
        )}
      </div>
    </div>
  );
};
```

### 2. ThoughtMessage Component

Following Claude's collapsible pattern with CenSuite styling:

```tsx
export const ThoughtMessage: React.FC<{ message: EnhancedMessage }> = ({
  message
}) => {
  const [isExpanded, setIsExpanded] = useState(false);
  const firstLine = message.content.split('\n')[0].slice(0, 80);
  
  return (
    <div className={cn(
      "rounded-lg border border-border/50",
      "my-2 transition-colors duration-200",
      "hover:bg-muted/20"
    )}>
      <button
        className={cn(
          "group/row flex w-full items-center justify-between",
          "gap-4 rounded-lg px-3 py-2 h-[2.625rem]",
          "text-muted-foreground hover:text-foreground",
          "transition-colors duration-200 cursor-pointer"
        )}
        onClick={() => setIsExpanded(!isExpanded)}
        aria-expanded={isExpanded}
        aria-label={isExpanded ? "Collapse thought" : "Expand thought"}
      >
        <div className="flex items-center gap-2 min-w-0">
          <span className="text-sm leading-tight truncate">
            {isExpanded ? "Hide thought process" : firstLine}
          </span>
        </div>
        
        <ChevronRight className={cn(
          "h-4 w-4 shrink-0 transition-transform duration-200",
          isExpanded && "rotate-90"
        )} />
      </button>
      
      {/* Expandable Content */}
      <div
        className={cn(
          "overflow-hidden transition-all duration-200",
          isExpanded ? "max-h-[500px] opacity-100" : "max-h-0 opacity-0"
        )}
        style={{
          maskImage: isExpanded 
            ? "linear-gradient(transparent 0%, black 1rem, black calc(100% - 1rem), transparent 100%)"
            : undefined
        }}
      >
        <div className="px-3 pb-3 text-sm text-muted-foreground">
          <MarkdownRenderer content={message.content} />
          
          {/* Token counts for thoughts */}
          {message.metadata?.outputTokens && (
            <div className="mt-2 flex items-center gap-2 text-xs">
              <span className="text-muted-foreground/70">
                {message.metadata.outputTokens} tokens
              </span>
              <Button
                variant="ghost"
                size="sm"
                className="h-6 px-2"
                onClick={() => navigator.clipboard.writeText(message.content)}
              >
                <Copy className="h-3 w-3" />
              </Button>
            </div>
          )}
        </div>
      </div>
    </div>
  );
};
```

### 3. MessageFooter Component

Refined with proper token display and tool call indicators:

```tsx
export const MessageFooter: React.FC<MessageFooterProps> = ({
  message,
  onCopy,
  showTokenCounts
}) => {
  const [copied, setCopied] = useState(false);
  const [showToolCalls, setShowToolCalls] = useState(false);
  
  const handleCopy = () => {
    onCopy?.();
    setCopied(true);
    setTimeout(() => setCopied(false), 2000);
  };
  
  const hasToolCalls = message.toolCalls && message.toolCalls.length > 0;
  
  return (
    <div className="flex items-center gap-3 text-xs text-muted-foreground">
      {/* Token Counts */}
      {showTokenCounts && message.metadata?.outputTokens && (
        <span className="flex items-center gap-1">
          {message.metadata.inputTokens && (
            <>
              <span>{message.metadata.inputTokens}</span>
              <ArrowRight className="h-3 w-3" />
            </>
          )}
          <span>{message.metadata.outputTokens}</span>
        </span>
      )}
      
      {/* Tool Calls Button */}
      {hasToolCalls && (
        <Button
          variant="ghost"
          size="sm"
          className="h-6 px-2 gap-1"
          onClick={() => setShowToolCalls(!showToolCalls)}
        >
          <Wrench className="h-3 w-3" />
          <span>{message.toolCalls!.length} tool{message.toolCalls!.length > 1 ? 's' : ''}</span>
        </Button>
      )}
      
      {/* Copy Button */}
      <Button
        variant="ghost"
        size="sm"
        className="h-6 px-2 gap-1"
        onClick={handleCopy}
        aria-label="Copy message"
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
      
      {/* Collapsible Tool Calls */}
      {showToolCalls && hasToolCalls && (
        <div className="mt-2 w-full">
          <ToolCallDisplay toolCalls={message.toolCalls!} />
        </div>
      )}
    </div>
  );
};
```

### 4. ToolNotification Component

Real-time feedback with proper animations:

```tsx
export const ToolNotification: React.FC<{ notification: ToolNotification }> = ({ 
  notification 
}) => {
  const statusConfig = {
    preparing: {
      text: `Preparing ${notification.toolName}...`,
      icon: <Clock className="h-3 w-3 animate-pulse" />
    },
    executing: {
      text: `Using ${notification.toolName}...`,
      icon: <Loader2 className="h-3 w-3 animate-spin" />
    },
    complete: {
      text: `Completed ${notification.toolName}`,
      icon: <Check className="h-3 w-3" />
    }
  };
  
  const config = statusConfig[notification.status];
  
  return (
    <div 
      className={cn(
        "flex items-center gap-2 px-3 py-1.5",
        "bg-primary/5 text-primary rounded-md",
        "text-sm animate-in slide-in-from-top-2 duration-200"
      )}
      role="status"
      aria-live="polite"
    >
      {config.icon}
      <span>{config.text}</span>
    </div>
  );
};
```

### 5. RichMediaDisplay Component

With proper sanitization and safety:

```tsx
export const RichMediaDisplay: React.FC<{ message: EnhancedMessage }> = ({ 
  message 
}) => {
  const [error, setError] = useState<string | null>(null);
  const containerRef = useRef<HTMLDivElement>(null);
  
  // Sanitize content using DOMPurify (needs to be imported)
  const sanitizedContent = useMemo(() => {
    try {
      if (message.contentType === 'html') {
        return DOMPurify.sanitize(message.content, {
          ALLOWED_TAGS: ['div', 'span', 'p', 'h1', 'h2', 'h3', 'h4', 'h5', 'h6', 
                         'ul', 'ol', 'li', 'a', 'img', 'table', 'tr', 'td', 'th',
                         'strong', 'em', 'code', 'pre', 'blockquote'],
          ALLOWED_ATTR: ['href', 'src', 'alt', 'class', 'style']
        });
      }
      if (message.contentType === 'svg') {
        return DOMPurify.sanitize(message.content, {
          USE_PROFILES: { svg: true, svgFilters: true }
        });
      }
      return message.content;
    } catch (err) {
      setError('Failed to render media content');
      return '';
    }
  }, [message.content, message.contentType]);
  
  if (error) {
    return (
      <Alert variant="destructive">
        <AlertCircle className="h-4 w-4" />
        <AlertDescription>{error}</AlertDescription>
      </Alert>
    );
  }
  
  return (
    <div className="my-4">
      <div 
        ref={containerRef}
        className={cn(
          "rounded-lg border border-border p-4",
          "bg-muted/20 overflow-auto max-h-96",
          message.contentType === 'svg' && "flex items-center justify-center"
        )}
        dangerouslySetInnerHTML={{ __html: sanitizedContent }}
        aria-label="Rich media content"
      />
      
      {/* Attribution */}
      {message.metadata?.sentByClass && (
        <div className="mt-2 text-xs text-muted-foreground">
          Generated by {message.metadata.sentByClass}.{message.metadata.sentByFunction}
        </div>
      )}
    </div>
  );
};
```

### 6. SystemNotification Component

Consistent with CenSuite Alert patterns:

```tsx
export const SystemNotification: React.FC<{
  event: SystemMessageEvent | ErrorEvent
}> = ({ event }) => {
  const severity = 'severity' in event ? event.severity : 'error';
  
  const variantMap = {
    error: 'destructive',
    warning: 'default',
    info: 'default'
  } as const;
  
  const iconMap = {
    error: <XCircle className="h-4 w-4" />,
    warning: <AlertTriangle className="h-4 w-4" />,
    info: <Info className="h-4 w-4" />
  };
  
  return (
    <Alert 
      variant={variantMap[severity] || 'default'}
      className="my-2"
    >
      {iconMap[severity] || iconMap.info}
      <AlertDescription>
        {'content' in event ? event.content : event.message}
      </AlertDescription>
    </Alert>
  );
};
```

## Key Design System Alignments

### 1. Accessibility Features
- All interactive elements have proper ARIA labels
- Focus states follow the ring pattern: `focus-visible:ring-2 focus-visible:ring-ring`
- Screen reader announcements for dynamic content
- Keyboard navigation support (Tab, Enter, Escape)

### 2. Animation Patterns
- Smooth transitions: `transition-all duration-200`
- Chevron rotation for collapsibles: `rotate-90`
- Slide animations: `animate-in slide-in-from-top-2`
- Loading states: `animate-spin` for spinners, `animate-pulse` for dots

### 3. Responsive Design
- Max width constraints: `max-w-[75ch]` for readability
- Proper truncation with `truncate` class
- Mobile-friendly touch targets (minimum 44px)

### 4. Component Composition
- Reuse existing UI primitives (Button, Alert, etc.)
- Consistent spacing using 4px base scale
- Proper color token usage (no hardcoded colors)

## Dependencies

For full implementation, these libraries are needed:
- `DOMPurify` for HTML/SVG sanitization
- `react-markdown` and `remark-gfm` for markdown rendering
- Icons from `lucide-react` (already in use)

## Implementation Notes

1. Use `cn()` utility for conditional classes
2. Maintain consistent hover states across components
3. Implement proper loading states for async operations
4. Cache sanitized content to avoid re-processing
5. Use React.memo for performance optimization on large message lists
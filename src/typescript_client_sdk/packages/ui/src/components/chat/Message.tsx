'use client'

import * as React from 'react'
import { cn } from '../../lib/utils'
import { 
  User, Bot, Brain, AlertCircle, Clock, 
  ChevronRight, Loader2, Check, Copy
} from 'lucide-react'
import { Button } from '../ui/button'
import { motion, AnimatePresence } from 'framer-motion'
import type { Message as SDKMessage, MessageContent, ContentPart } from '@agentc/realtime-core'
import { MessageContentRenderer } from './MessageContentRenderer'
import { MarkdownRenderer } from './content-renderers/MarkdownRenderer'
import { Logger } from '../../utils/logger'

export interface MessageData extends SDKMessage {
  id?: string  // Optional ID for keying
  status?: 'sending' | 'sent' | 'error'
  error?: string
  isThought?: boolean  // Flag for thought messages
  // Support both SessionEvent metadata and token metadata
  metadata?: {
    // Session-related metadata from ExtendedMessage
    sessionId?: string
    parentSessionId?: string
    userSessionId?: string
    // Token-related metadata for usage tracking
    inputTokens?: number
    outputTokens?: number
  }
  // Tool calls with the corrected structure
  toolCalls?: Array<{
    id: string
    type: 'tool_use'
    name: string  // Direct name, not function.name
    input: Record<string, unknown>  // Arguments as object
  }>
  // Tool results
  toolResults?: Array<{
    type: 'tool_result'
    tool_use_id: string
    content: string
  }>
}

/**
 * Extract text content from MessageContent for display purposes
 */
function extractTextContent(content: MessageContent): string {
  if (content === null) {
    return ''
  }
  
  if (typeof content === 'string') {
    return content
  }
  
  // For array of content parts, extract text parts
  return content
    .filter((part): part is ContentPart => part.type === 'text')
    .map((part: any) => part.text || '')
    .join('\n')
}



export interface MessageProps extends React.HTMLAttributes<HTMLDivElement> {
  /**
   * The message data to display
   */
  message: MessageData
  /**
   * Show timestamp
   */
  showTimestamp?: boolean
  /**
   * Message is currently streaming
   */
  isStreaming?: boolean
  /**
   * Message is from a sub-session (agent-to-agent)
   */
  isSubSession?: boolean
  /**
   * Custom avatar component
   */
  avatarComponent?: React.ReactNode
  /**
   * Callback for message actions
   */
  onEdit?: (id: string, content: string) => void
  /**
   * Show footer with metadata
   */
  showFooter?: boolean
}

// Thought message component
interface ThoughtMessageProps {
  message: MessageData
  isStreaming?: boolean
  showTimestamp?: boolean
  showFooter?: boolean
}

const ThoughtMessage: React.FC<ThoughtMessageProps> = ({
  message,
  isStreaming = false,
  showTimestamp = true,
  showFooter = true
}) => {
  const [isExpanded, setIsExpanded] = React.useState(true) // Default to expanded
  
  // Extract first line for preview - handle markdown better
  const firstLine = React.useMemo(() => {
    const textContent = extractTextContent(message.content)
    // Find first non-empty line that's not a code fence
    const lines = textContent.split('\n').filter(line => line.trim() !== '')
    let first = ''
    for (const line of lines) {
      // Skip code fence markers
      if (!line.trim().startsWith('```')) {
        first = line.trim()
        break
      }
    }
    if (!first && lines.length > 0) {
      first = lines[0].trim()
    }
    return first.length > 80 ? `${first.slice(0, 77)}...` : first
  }, [message.content])
  
  return (
    <div className="group relative flex gap-3 py-2" role="article" aria-label="Thought process">
      {/* Thought glyph - matches avatar positioning */}
      <div className="flex-shrink-0">
        <div className="h-8 w-8 rounded-full bg-muted/50 flex items-center justify-center">
          <Brain className="h-4 w-4 text-muted-foreground" />
        </div>
      </div>
      
      {/* Message content */}
      <div className="flex-1 max-w-full space-y-2 overflow-hidden">
        {/* Thought bubble */}
        <div
          className={cn(
            "relative rounded-xl px-4 py-2.5 transition-all duration-200 overflow-hidden",
            "bg-card-thought border border-border/50",
            "max-w-[85%]", // Match assistant message width
            isStreaming && "after:content-[''] after:inline-block after:w-1.5 after:h-4 after:ml-1 after:bg-current after:animate-pulse after:rounded-full"
          )}
        >
          {/* Collapse/Expand button */}
          <button
            className={cn(
              "flex w-full items-center justify-between gap-2 mb-2",
              "text-muted-foreground hover:text-foreground",
              "transition-colors duration-200 cursor-pointer",
              "focus-visible:outline-none focus-visible:ring-1",
              "focus-visible:ring-ring rounded",
              "-mx-1 px-1 py-1" // Expand clickable area slightly
            )}
            onClick={() => setIsExpanded(!isExpanded)}
            aria-expanded={isExpanded}
            aria-label={isExpanded ? "Collapse thought" : "Expand thought"}
          >
            <span className="text-xs font-medium truncate">
              {isExpanded ? "" : firstLine}
            </span>
            
            <div className="flex items-center gap-1.5 shrink-0">
              {isStreaming && (
                <Loader2 className="h-3 w-3 animate-spin text-muted-foreground/70" />
              )}
              <ChevronRight className={cn(
                "h-3.5 w-3.5 transition-transform duration-200",
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
                <div className="text-[0.9375rem] leading-6 tracking-tight text-muted-foreground">
                  <MarkdownRenderer
                    content={extractTextContent(message.content)}
                    compact={true}
                    className="prose-muted"
                    ariaLabel="Thought process content"
                  />
                </div>
              </motion.div>
            )}
          </AnimatePresence>
        </div>
        
        {/* Message Footer - always shown, placed below bubble like assistant messages */}
        {showFooter && !isStreaming && (
          <MessageFooter 
            message={message}
            showTimestamp={showTimestamp}
            role="assistant (thought)"
          />
        )}
      </div>
    </div>
  )
}

// Import MessageFooter from separate file
import { MessageFooter } from './MessageFooter'

// Debug flag for diagnostic logging
const DEBUG_MESSAGE_RENDERING = false;

const MessageComponent = React.forwardRef<HTMLDivElement, MessageProps>(
  ({ 
    className, 
    message,
    showTimestamp = true,
    isStreaming = false,
    isSubSession = false,
    avatarComponent,
    onEdit,
    showFooter = true,
    ...props 
  }, ref) => {
    // DIAGNOSTIC: Render counter
    const renderCount = React.useRef(0);
    renderCount.current++;
    
    if (DEBUG_MESSAGE_RENDERING) {
      console.log('[Message] 🎨 RENDERING:', {
        renderNumber: renderCount.current,
        messageId: message.id,
        role: message.role,
        toolCallsCount: message.toolCalls?.length || 0,
        toolResultsCount: message.toolResults?.length || 0,
        isStreaming,
        contentType: typeof message.content,
      });
    }
    
    const [isEditing, setIsEditing] = React.useState(false)
    const [editContent, setEditContent] = React.useState(extractTextContent(message.content) || '')
    
    // DIAGNOSTIC: Track when tool data changes
    React.useEffect(() => {
      if (DEBUG_MESSAGE_RENDERING) {
        console.log('[Message] 🔄 Tool data changed:', {
          messageId: message.id,
          toolCallsCount: message.toolCalls?.length || 0,
          toolResultsCount: message.toolResults?.length || 0,
          toolCalls: message.toolCalls,
          toolResults: message.toolResults,
        });
      }
    }, [message.id, message.toolCalls, message.toolResults]);
    
    // Check if message has attachments (images)
    const hasAttachments = React.useMemo(() => {
      return Array.isArray(message.content) && 
        message.content.some(block => 
          typeof block === 'object' && 
          block !== null && 
          'type' in block && 
          block.type === 'image'
        )
    }, [message.content])
    
    // Debug logging
    React.useEffect(() => {
      Logger.debug('[Message] Rendering message:', {
        role: message.role,
        contentType: typeof message.content,
        contentLength: Array.isArray(message.content) ? message.content.length : undefined,
        content: message.content,
        timestamp: message.timestamp,
        isThought: message.isThought,
        hasAttachments
      })
    }, [message, hasAttachments])
    
    // Check for thought messages
    if (message.isThought || message.role === 'assistant (thought)') {
      return (
        <ThoughtMessage 
          message={message}
          isStreaming={isStreaming}
          showTimestamp={showTimestamp}
          showFooter={showFooter}
        />
      )
    }
    
    const isUser = message.role === 'user'
    const isAssistant = message.role === 'assistant'
    const isError = message.status === 'error'
    
    // Avatar component
    const Avatar = () => {
      if (avatarComponent) return <>{avatarComponent}</>
      
      if (isUser) {
        return (
          <div className="h-8 w-8 rounded-full bg-primary flex items-center justify-center">
            <User className="h-4 w-4 text-primary-foreground" />
          </div>
        )
      }
      
      return (
        <div className="h-8 w-8 rounded-full bg-primary/10 flex items-center justify-center">
          <Bot className="h-4 w-4 text-primary" />
        </div>
      )
    }
    

    
    // Render content
    const renderContent = () => {
      if (isUser && isEditing) {
        // Edit mode for user messages - only text editing is supported
        return (
          <div className="whitespace-pre-wrap break-words">
            <textarea
              value={editContent}
              onChange={(e) => setEditContent(e.target.value)}
              onKeyDown={(e) => {
                if (e.key === 'Enter' && (e.metaKey || e.ctrlKey)) {
                  e.preventDefault()
                  if (onEdit && message.id) {
                    onEdit(message.id, editContent)
                    setIsEditing(false)
                  }
                } else if (e.key === 'Escape') {
                  e.preventDefault()
                  setEditContent(extractTextContent(message.content) || '')
                  setIsEditing(false)
                }
              }}
              className="w-full bg-transparent border-none outline-none resize-none min-h-[80px]"
              autoFocus
            />
          </div>
        )
      }
      
      // Use the new MessageContentRenderer for all message content
      return (
        <MessageContentRenderer
          content={message.content}
          role={message.role as 'user' | 'assistant' | 'system'}
        />
      )
    }
    
    return (
      <div
        ref={ref}
        className={cn(
          "group relative flex gap-3 py-2",
          isUser && "flex-row-reverse",
          className
        )}
        role="article"
        aria-label={`Message from ${isUser ? 'user' : 'assistant'}`}
        {...props}
      >
        {/* Avatar */}
        <div className="flex-shrink-0">
          <Avatar />
        </div>
        
        {/* Message content */}
        <div className={cn(
          "flex-1 max-w-full space-y-2 overflow-hidden",
          isUser && "flex flex-col items-end"
        )}>
          {/* Sub-session indicator - removed text label, keeping visual border only */}
          
          {/* Message bubble */}
          <div
            className={cn(
              "relative rounded-xl px-4 py-2.5 transition-all duration-200 overflow-hidden",
              isUser 
                ? "bg-card-user max-w-[85%]" 
                : "bg-card-assistant border border-border/50",
              isError && "bg-destructive/10 border border-destructive",
              isStreaming && "after:content-[''] after:inline-block after:w-1.5 after:h-4 after:ml-1 after:bg-current after:animate-pulse after:rounded-full",
              isSubSession && "border-l-2 border-primary/30",
              hasAttachments && "ring-1 ring-primary/20 ring-inset"
            )}
          >
            {/* Error message */}
            {isError && message.error && (
              <div className="flex items-start gap-2 mb-2 text-destructive">
                <AlertCircle className="h-4 w-4 mt-0.5 flex-shrink-0" />
                <span className="text-sm">{message.error}</span>
              </div>
            )}
            
            {/* Main content */}
            <div className="text-[0.9375rem] leading-6 tracking-tight">
              {renderContent()}
            </div>
            
            {/* Edit controls for user messages */}
            {isEditing && isUser && (
              <div className="flex items-center gap-2 mt-2">
                <Button
                  size="sm"
                  onClick={() => {
                    if (onEdit && message.id) {
                      onEdit(message.id, editContent)
                      setIsEditing(false)
                    }
                  }}
                  disabled={!editContent.trim()}
                >
                  Save
                </Button>
                <Button
                  size="sm"
                  variant="ghost"
                  onClick={() => {
                    setEditContent(extractTextContent(message.content) || '')
                    setIsEditing(false)
                  }}
                >
                  Cancel
                </Button>
                <span className="ml-auto text-xs text-muted-foreground">
                  ⌘+Enter to save, Esc to cancel
                </span>
              </div>
            )}

          </div>
          
          {/* Message Footer for assistant messages */}
          {isAssistant && showFooter && !isStreaming && (
            <MessageFooter 
              message={message}
              showTimestamp={showTimestamp}
              role="assistant"
            />
          )}
          
          {/* Message Footer for user messages */}
          {isUser && showFooter && !isStreaming && !isEditing && (
            <MessageFooter 
              message={message}
              onEdit={() => setIsEditing(true)}
              showTimestamp={showTimestamp}
              role="user"
            />
          )}
          
          {/* Status indicator */}
          {(message.status === 'sending' || message.status === 'sent') && (
            <div className="flex items-center gap-2 px-1">
              {message.status === 'sending' && (
                <span className="text-xs text-muted-foreground">
                  Sending...
                </span>
              )}
              {message.status === 'sent' && !isError && (
                <Check className="h-3 w-3 text-muted-foreground" />
              )}
            </div>
          )}
        </div>
      </div>
    )
  }
)

MessageComponent.displayName = 'Message'

// Memoize the Message component to prevent unnecessary re-renders
const Message = React.memo(MessageComponent, (prevProps, nextProps) => {
  // DIAGNOSTIC: Log memo comparison
  if (DEBUG_MESSAGE_RENDERING) {
    console.log('[Message MEMO] 🔍 Comparing props:', {
      messageId: nextProps.message.id,
      prevToolCallsRef: prevProps.message.toolCalls,
      nextToolCallsRef: nextProps.message.toolCalls,
      prevToolResultsRef: prevProps.message.toolResults,
      nextToolResultsRef: nextProps.message.toolResults,
      prevContentRef: typeof prevProps.message.content,
      nextContentRef: typeof nextProps.message.content,
      prevIsStreaming: prevProps.isStreaming,
      nextIsStreaming: nextProps.isStreaming,
    });
  }
  
  // Custom comparison to prevent re-renders when not needed
  if (prevProps.message.id !== nextProps.message.id) {
    if (DEBUG_MESSAGE_RENDERING) console.log('[Message MEMO] ✅ ID changed - WILL RE-RENDER');
    return false;
  }
  if (prevProps.message.content !== nextProps.message.content) {
    if (DEBUG_MESSAGE_RENDERING) console.log('[Message MEMO] ✅ Content changed - WILL RE-RENDER');
    return false;
  }
  if (prevProps.isStreaming !== nextProps.isStreaming) {
    if (DEBUG_MESSAGE_RENDERING) console.log('[Message MEMO] ✅ Streaming state changed - WILL RE-RENDER');
    return false;
  }
  if (prevProps.isSubSession !== nextProps.isSubSession) {
    if (DEBUG_MESSAGE_RENDERING) console.log('[Message MEMO] ✅ SubSession changed - WILL RE-RENDER');
    return false;
  }
  if (prevProps.showTimestamp !== nextProps.showTimestamp) {
    if (DEBUG_MESSAGE_RENDERING) console.log('[Message MEMO] ✅ ShowTimestamp changed - WILL RE-RENDER');
    return false;
  }
  if (prevProps.showFooter !== nextProps.showFooter) {
    if (DEBUG_MESSAGE_RENDERING) console.log('[Message MEMO] ✅ ShowFooter changed - WILL RE-RENDER');
    return false;
  }
  
  // Compare tool arrays by REFERENCE, not length
  // This detects when arrays are added (undefined → array) or replaced (new reference)
  if (prevProps.message.toolCalls !== nextProps.message.toolCalls) {
    if (DEBUG_MESSAGE_RENDERING) console.log('[Message MEMO] ✅ ToolCalls reference changed - WILL RE-RENDER');
    return false;
  }
  if (prevProps.message.toolResults !== nextProps.message.toolResults) {
    if (DEBUG_MESSAGE_RENDERING) console.log('[Message MEMO] ✅ ToolResults reference changed - WILL RE-RENDER');
    return false;
  }

  if (DEBUG_MESSAGE_RENDERING) console.log('[Message MEMO] ⏭️  Props equal - SKIPPING RE-RENDER');
  // Props are equal, skip re-render
  return true;
});

export { Message }
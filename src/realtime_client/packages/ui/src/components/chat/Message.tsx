'use client'

import * as React from 'react'
import { cn } from '../../lib/utils'
import { 
  User, Bot, Brain, AlertCircle, Clock, 
  ChevronRight, Loader2, Check, Copy
} from 'lucide-react'
import { Button } from '../ui/button'
import ReactMarkdown from 'react-markdown'
import remarkGfm from 'remark-gfm'
import { motion, AnimatePresence } from 'framer-motion'
import type { Message as SDKMessage, MessageContent, ContentPart } from '@agentc/realtime-core'
import { MessageContentRenderer } from './MessageContentRenderer'
import { ToolCallResultList } from './ToolCallResult'
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
}

const ThoughtMessage: React.FC<ThoughtMessageProps> = ({
  message,
  isStreaming = false
}) => {
  const [isExpanded, setIsExpanded] = React.useState(false)
  const [copied, setCopied] = React.useState(false)
  
  // Extract first line for preview
  const firstLine = React.useMemo(() => {
    const textContent = extractTextContent(message.content)
    const lines = textContent.split('\n')
    const first = lines[0] || ''
    return first.length > 80 ? `${first.slice(0, 77)}...` : first
  }, [message.content])
  
  const handleCopy = React.useCallback(() => {
    const textContent = extractTextContent(message.content)
    navigator.clipboard.writeText(textContent)
    setCopied(true)
    setTimeout(() => setCopied(false), 2000)
  }, [message.content])
  
  // Simple markdown components for thought messages
  const thoughtMarkdownComponents = {
    p({ children }: any) {
      return <p className="mb-2 last:mb-0">{children}</p>
    },
    code({ inline, children }: any) {
      if (inline) {
        return <code className="bg-muted px-1 py-0.5 rounded text-xs">{children}</code>
      }
      return (
        <pre className="bg-muted rounded p-2 overflow-x-auto my-2">
          <code className="text-xs">{children}</code>
        </pre>
      )
    }
  }
  
  return (
    <div className={cn(
      "rounded-lg border border-border/50 my-2",
      "bg-muted/30",
      "transition-all duration-200 ease-out",
      "hover:bg-muted/40 hover:border-border/70"
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
              <div className="prose prose-sm prose-muted max-w-none">
                <ReactMarkdown 
                  remarkPlugins={[remarkGfm]}
                  components={thoughtMarkdownComponents}
                >
                  {extractTextContent(message.content)}
                </ReactMarkdown>
              </div>
              
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

// Import MessageFooter from separate file
import { MessageFooter } from './MessageFooter'

const Message = React.forwardRef<HTMLDivElement, MessageProps>(
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
    const [isEditing, setIsEditing] = React.useState(false)
    const [editContent, setEditContent] = React.useState(extractTextContent(message.content) || '')
    
    // Debug logging
    React.useEffect(() => {
      Logger.debug('[Message] Rendering message:', {
        role: message.role,
        contentType: typeof message.content,
        contentLength: Array.isArray(message.content) ? message.content.length : undefined,
        content: message.content,
        timestamp: message.timestamp,
        isThought: message.isThought
      })
    }, [message])
    
    // Check for thought messages
    if (message.isThought || message.role === 'assistant (thought)') {
      return (
        <ThoughtMessage 
          message={message}
          isStreaming={isStreaming}
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
                ? "bg-muted" 
                : "bg-card border border-border/50",
              isError && "bg-destructive/10 border border-destructive",
              isStreaming && "after:content-[''] after:inline-block after:w-1.5 after:h-4 after:ml-1 after:bg-current after:animate-pulse after:rounded-full",
              isSubSession && "border-l-2 border-primary/30"
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
            
            {/* Tool call results - only for assistant messages */}
            {isAssistant && message.toolCalls && message.toolCalls.length > 0 && (
              <div className="mt-3">
                <ToolCallResultList
                  toolCalls={message.toolCalls}
                  toolResults={message.toolResults}
                />
              </div>
            )}
            
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
            />
          )}
          
          {/* Message Footer for user messages */}
          {isUser && showFooter && !isStreaming && !isEditing && (
            <MessageFooter 
              message={message}
              onEdit={() => setIsEditing(true)}
              showTimestamp={showTimestamp}
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

Message.displayName = 'Message'

export { Message }
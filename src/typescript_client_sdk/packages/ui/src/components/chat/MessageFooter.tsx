'use client'

import * as React from 'react'
import { cn } from '../../lib/utils'
import { 
  Copy, Check, Hash, ArrowRight, Wrench, 
  ChevronDown, RefreshCw, Edit2, Clock,
  FileInput, FileOutput, Equal, Code2, ChevronRight
} from 'lucide-react'
import { Button } from '../ui/button'
import { motion, AnimatePresence } from 'framer-motion'
import type { MessageData } from './Message'
import type { MessageContent } from '@agentc/realtime-core'

export interface MessageFooterProps extends React.HTMLAttributes<HTMLDivElement> {
  /**
   * The message data
   */
  message: MessageData
  /**
   * Callback for editing message
   */
  onEdit?: () => void
  /**
   * Callback for regenerating message
   */
  onRegenerate?: () => void
  /**
   * Whether to show timestamp
   */
  showTimestamp?: boolean
  /**
   * Message role for styling
   */
  role?: 'user' | 'assistant' | 'assistant (thought)' | 'system'
}

interface ToolCallDisplayProps {
  toolCalls: NonNullable<MessageData['toolCalls']>
  toolResults?: MessageData['toolResults']
  className?: string
}

const ToolCallDisplay: React.FC<ToolCallDisplayProps> = ({
  toolCalls,
  toolResults = [],
  className
}) => {
  // DIAGNOSTIC: Log when ToolCallDisplay renders
  const renderCount = React.useRef(0);
  renderCount.current++;
  
  if (DEBUG_MESSAGE_FOOTER) {
    console.log('[ToolCallDisplay] 🛠️ RENDERING:', {
      renderNumber: renderCount.current,
      toolCallsCount: toolCalls.length,
      toolResultsCount: toolResults?.length || 0,
      toolNames: toolCalls.map(tc => tc.name),
      toolIds: toolCalls.map(tc => tc.id),
    });
  }
  
  const [expandedTools, setExpandedTools] = React.useState<Set<string>>(new Set())
  const [copiedInputs, setCopiedInputs] = React.useState<Set<string>>(new Set())
  const [copiedResults, setCopiedResults] = React.useState<Set<string>>(new Set())
  
  // Filter out think tool calls
  const visibleToolCalls = React.useMemo(() => {
    return toolCalls.filter(tc => tc.name !== 'think')
  }, [toolCalls])
  
  // Create a map of results by tool_use_id for efficient lookup
  const resultsMap = React.useMemo(() => {
    const map = new Map<string, typeof toolResults[0]>()
    toolResults?.forEach(result => {
      map.set(result.tool_use_id, result)
    })
    return map
  }, [toolResults])
  
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
  
  const handleCopyInput = React.useCallback((toolId: string, input: string) => {
    navigator.clipboard.writeText(input)
    setCopiedInputs(prev => new Set(prev).add(toolId))
    setTimeout(() => {
      setCopiedInputs(prev => {
        const next = new Set(prev)
        next.delete(toolId)
        return next
      })
    }, 2000)
  }, [])
  
  const handleCopyResult = React.useCallback((toolId: string, result: string) => {
    navigator.clipboard.writeText(result)
    setCopiedResults(prev => new Set(prev).add(toolId))
    setTimeout(() => {
      setCopiedResults(prev => {
        const next = new Set(prev)
        next.delete(toolId)
        return next
      })
    }, 2000)
  }, [])
  
  if (visibleToolCalls.length === 0) {
    return null
  }
  
  return (
    <div className={cn(
      "rounded-lg border-2 border-primary/20 bg-gradient-to-br from-card/80 to-muted/30 shadow-sm space-y-2 mt-2",
      className
    )}>
      {visibleToolCalls.map((tool) => {
        const isExpanded = expandedTools.has(tool.id)
        const toolResult = resultsMap.get(tool.id)
        const hasCopiedInput = copiedInputs.has(tool.id)
        const hasCopiedResult = copiedResults.has(tool.id)
        
        // Format the input arguments
        const formattedInput = React.useMemo(() => {
          try {
            return JSON.stringify(tool.input, null, 2)
          } catch {
            return String(tool.input)
          }
        }, [tool.input])
        
        // Format the result content
        const formattedResult = React.useMemo(() => {
          if (!toolResult?.content) return 'No result available'
          
          try {
            // Try to parse as JSON for better formatting
            const parsed = JSON.parse(toolResult.content)
            return JSON.stringify(parsed, null, 2)
          } catch {
            // If not JSON, return as is
            return toolResult.content
          }
        }, [toolResult])
        
        return (
          <div key={tool.id} className="rounded-md border border-border/50 bg-card/50 hover:bg-muted/20 transition-all duration-200">
            {/* Header - Always visible */}
            <button
              className={cn(
                "group/tool flex w-full items-center justify-between",
                "gap-4 rounded-t-md px-4 py-3",
                "text-foreground hover:bg-primary/5",
                "transition-colors duration-200 cursor-pointer",
                "focus-visible:outline-none focus-visible:ring-2",
                "focus-visible:ring-ring focus-visible:ring-offset-2"
              )}
              onClick={() => toggleTool(tool.id)}
              aria-expanded={isExpanded}
              aria-label={`${isExpanded ? 'Collapse' : 'Expand'} ${tool.name} details`}
            >
              <div className="flex items-center gap-3 min-w-0">
                <div className="flex-shrink-0">
                  <div className="h-8 w-8 rounded-md bg-primary/10 flex items-center justify-center">
                    <Wrench className="h-4 w-4 text-primary" />
                  </div>
                </div>
                <div className="flex flex-col items-start min-w-0">
                  <span className="text-sm font-semibold">
                    {tool.name}
                  </span>
                  <span className="text-xs text-muted-foreground">
                    {toolResult ? 'Tool executed successfully' : 'Tool called'}
                  </span>
                </div>
              </div>
              
              <div className="flex items-center gap-2 shrink-0">
                {toolResult && <Check className="h-4 w-4 text-green-600 dark:text-green-400" />}
                <ChevronRight className={cn(
                  "h-4 w-4 transition-transform duration-200 text-muted-foreground",
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
                  <div className="px-4 pb-4 space-y-3">
                    {/* Input Arguments */}
                    <div className="space-y-2">
                      <div className="flex items-center justify-between">
                        <h4 className="text-sm font-medium flex items-center gap-2">
                          <Code2 className="h-3.5 w-3.5 text-muted-foreground" />
                          Input Arguments
                        </h4>
                        <Button
                          variant="ghost"
                          size="sm"
                          className="h-7 px-2 text-xs"
                          onClick={() => handleCopyInput(tool.id, formattedInput)}
                        >
                          {hasCopiedInput ? (
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
                      <pre className={cn(
                        "bg-muted/50 rounded-md p-3 overflow-x-auto",
                        "text-xs font-mono text-muted-foreground",
                        "border border-border/30"
                      )}>
                        <code>{formattedInput}</code>
                      </pre>
                    </div>
                    
                    {/* Result */}
                    {toolResult && (
                      <div className="space-y-2">
                        <div className="flex items-center justify-between">
                          <h4 className="text-sm font-medium">Result</h4>
                          <Button
                            variant="ghost"
                            size="sm"
                            className="h-7 px-2 text-xs"
                            onClick={() => handleCopyResult(tool.id, formattedResult)}
                          >
                            {hasCopiedResult ? (
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
                        <div className={cn(
                          "bg-muted/30 rounded-md p-3 overflow-x-auto",
                          "text-sm text-foreground",
                          "border border-border/30"
                        )}>
                          {formattedResult.length > 500 ? (
                            <details>
                              <summary className="cursor-pointer text-sm font-medium text-primary hover:underline">
                                Show full result ({formattedResult.length} characters)
                              </summary>
                              <pre className="mt-2 text-xs font-mono whitespace-pre-wrap break-words">
                                <code>{formattedResult}</code>
                              </pre>
                            </details>
                          ) : (
                            <pre className="text-xs font-mono whitespace-pre-wrap break-words">
                              <code>{formattedResult}</code>
                            </pre>
                          )}
                        </div>
                      </div>
                    )}
                  </div>
                </motion.div>
              )}
            </AnimatePresence>
          </div>
        )
      })}
    </div>
  )
}

// Debug flag for diagnostic logging
const DEBUG_MESSAGE_FOOTER = false;

export const MessageFooter = React.forwardRef<HTMLDivElement, MessageFooterProps>(
  ({ className, message, onEdit, onRegenerate, showTimestamp = true, role, ...props }, ref) => {
    const [copied, setCopied] = React.useState(false)
    const [showToolCalls, setShowToolCalls] = React.useState(false)
    
    // DIAGNOSTIC: Render counter
    const renderCount = React.useRef(0);
    renderCount.current++;
    
    const handleCopy = React.useCallback(() => {
      // Extract text content for copying
      let textContent = ''
      if (message.content === null) {
        textContent = ''
      } else if (typeof message.content === 'string') {
        textContent = message.content
      } else if (Array.isArray(message.content)) {
        // Extract text from content parts
        textContent = message.content
          .filter((part: any) => part.type === 'text')
          .map((part: any) => part.text || '')
          .join('\n')
      }
      
      navigator.clipboard.writeText(textContent)
      setCopied(true)
      setTimeout(() => setCopied(false), 2000)
    }, [message.content])
    
    const hasToolCalls = message.toolCalls && message.toolCalls.length > 0
    const hasTokenCounts = message.metadata?.inputTokens || message.metadata?.outputTokens
    const isUserMessage = (role || message.role) === 'user'
    const isAssistantMessage = (role || message.role) === 'assistant' || (role || message.role) === 'assistant (thought)'
    const isThoughtMessage = (role || message.role) === 'assistant (thought)'
    
    // DIAGNOSTIC: Log render state
    if (DEBUG_MESSAGE_FOOTER) {
      console.log('[MessageFooter] 👣 RENDERING:', {
        renderNumber: renderCount.current,
        messageId: message.id,
        hasToolCalls,
        showToolCalls,
        toolCallsCount: message.toolCalls?.length || 0,
        toolResultsCount: message.toolResults?.length || 0,
        toolCallsArray: message.toolCalls,
        toolResultsArray: message.toolResults,
      });
    }
    
    // Format timestamp
    const formattedTime = React.useMemo(() => {
      if (!showTimestamp || !message.timestamp) return null
      try {
        const date = new Date(message.timestamp)
        if (isNaN(date.getTime())) return null
        return date.toLocaleTimeString('en-US', { 
          hour: 'numeric', 
          minute: '2-digit',
          hour12: true 
        })
      } catch {
        return null
      }
    }, [message.timestamp, showTimestamp])
    
    // Always show footer for assistant messages (for copy button)
    // For other messages, only show if there's content to display
    if (!isAssistantMessage && !hasTokenCounts && !hasToolCalls && !onRegenerate && !isUserMessage && !formattedTime) {
      return null
    }
    
    return (
      <div 
        ref={ref}
        className={cn(
          "space-y-2",
          // Apply width constraints to match the message bubble
          isUserMessage && "max-w-[85%]",
          isThoughtMessage && "max-w-[85%]",
          // No max-width for regular assistant messages
          className
        )}
        {...props}
      >
        {/* Primary Footer Row */}
        <div className={cn(
          "flex items-center gap-3 text-xs rounded-lg px-3 py-1.5 transition-colors",
          // Subtle background that complements the message bubble
          isUserMessage && "bg-card-user/30 text-muted-foreground",
          isThoughtMessage && "bg-card-thought/20 border border-border/30 text-muted-foreground",
          isAssistantMessage && !isThoughtMessage && "bg-card-assistant/30 border border-border/30 text-muted-foreground"
        )}>
          {/* Token Counts */}
          {hasTokenCounts && (
            <>
              {/* Input Tokens */}
              {message.metadata?.inputTokens !== undefined && (
                <div className="flex items-center gap-1" title="Input tokens">
                  <FileInput className="h-3 w-3" />
                  <span>{message.metadata.inputTokens.toLocaleString()}</span>
                </div>
              )}
              
              {/* Output Tokens */}
              {message.metadata?.outputTokens !== undefined && (
                <div className="flex items-center gap-1" title="Output tokens">
                  <FileOutput className="h-3 w-3" />
                  <span>{message.metadata.outputTokens.toLocaleString()}</span>
                </div>
              )}
              
              {/* Total Tokens */}
              {(message.metadata?.inputTokens !== undefined || message.metadata?.outputTokens !== undefined) && (
                <div className="flex items-center gap-1" title="Total tokens">
                  <Equal className="h-3 w-3 fg-card" />
                  <span>
                    {(
                      (message.metadata?.inputTokens || 0) + 
                      (message.metadata?.outputTokens || 0)
                    ).toLocaleString()}
                  </span>
                </div>
              )}
            </>
          )}
          
          {/* Tool Calls Toggle */}
          {hasToolCalls && (
            <Button
              variant="ghost"
              size="sm"
              className="h-6 px-2 gap-1 -ml-2 text-foreground"
              onClick={() => {
                if (DEBUG_MESSAGE_FOOTER) {
                  console.log('[MessageFooter] 🖘️ TOGGLING tool calls display:', !showToolCalls);
                }
                setShowToolCalls(!showToolCalls);
              }}
              aria-label={`${showToolCalls ? 'Hide' : 'Show'} tool calls`}
            >
              <Wrench className="h-3 w-3" />
              <span>{message.toolCalls!.length} tool{message.toolCalls!.length > 1 ? 's' : ''}</span>
              <ChevronDown className={cn(
                "h-3 w-3 transition-transform",
                showToolCalls && "rotate-180"
              )} />
            </Button>
          )}
          
          {/* Action Buttons and Timestamp - aligned on same line */}
          <div className="flex items-center gap-2 ml-auto">
            {/* Timestamp */}
            {formattedTime && (
              <span className="text-xs text-muted-foreground flex items-center gap-1">
                <Clock className="h-3 w-3" />
                {formattedTime}
              </span>
            )}
            
            {/* Copy Button */}
            <Button
              variant="ghost"
              size="sm"
              className="h-6 px-2 gap-1"
              onClick={handleCopy}
              aria-label={copied ? "Content copied" : "Copy message content"}
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
            
            {/* Edit Button (for user messages on hover) */}
            {isUserMessage && onEdit && (
              <Button
                variant="ghost"
                size="sm"
                className="h-6 px-2 gap-1 opacity-0 group-hover:opacity-100 transition-opacity"
                onClick={onEdit}
                aria-label="Edit message"
              >
                <Edit2 className="h-3 w-3" />
                <span>Edit</span>
              </Button>
            )}
            
            {/* Regenerate Button (for assistant messages) */}
            {!isUserMessage && onRegenerate && (
              <Button
                variant="ghost"
                size="sm"
                className="h-6 px-2 gap-1"
                onClick={onRegenerate}
                aria-label="Regenerate response"
              >
                <RefreshCw className="h-3 w-3" />
                <span>Regenerate</span>
              </Button>
            )}
          </div>
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
              {(() => {
                if (DEBUG_MESSAGE_FOOTER) {
                  console.log('[MessageFooter] 🎁 RENDERING ToolCallDisplay');
                }
                return (
                  <ToolCallDisplay 
                    toolCalls={message.toolCalls!}
                    toolResults={message.toolResults}
                    className="mt-2"
                  />
                );
              })()}
            </motion.div>
          )}
        </AnimatePresence>
      </div>
    )
  }
)

MessageFooter.displayName = 'MessageFooter'
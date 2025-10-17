'use client'

import * as React from 'react'
import { cn } from '../../lib/utils'
import { Wrench, ChevronRight, Check, Copy, Code2 } from 'lucide-react'
import { motion, AnimatePresence } from 'framer-motion'
import { Button } from '../ui/button'

export interface ToolCall {
  id: string
  type: 'tool_use'
  name: string
  input: Record<string, unknown>
}

export interface ToolResult {
  type: 'tool_result'
  tool_use_id: string
  content: string
}

export interface ToolCallResultProps extends React.HTMLAttributes<HTMLDivElement> {
  /**
   * The tool call data
   */
  toolCall: ToolCall
  /**
   * The tool result data
   */
  toolResult?: ToolResult
  /**
   * Whether to show in expanded state by default
   */
  defaultExpanded?: boolean
}

/**
 * ToolCallResult Component
 * Displays completed tool calls with their results
 * Excludes the "think" tool as it's handled separately
 */
const ToolCallResult = React.forwardRef<HTMLDivElement, ToolCallResultProps>(
  ({ className, toolCall, toolResult, defaultExpanded = false, ...props }, ref) => {
    const [isExpanded, setIsExpanded] = React.useState(defaultExpanded)
    const [copiedInput, setCopiedInput] = React.useState(false)
    const [copiedResult, setCopiedResult] = React.useState(false)
    
    // Don't render think tool calls
    if (toolCall.name === 'think') {
      return null
    }
    
    // Format the input arguments
    const formattedInput = React.useMemo(() => {
      try {
        return JSON.stringify(toolCall.input, null, 2)
      } catch {
        return String(toolCall.input)
      }
    }, [toolCall.input])
    
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
    
    const handleCopyInput = React.useCallback(() => {
      navigator.clipboard.writeText(formattedInput)
      setCopiedInput(true)
      setTimeout(() => setCopiedInput(false), 2000)
    }, [formattedInput])
    
    const handleCopyResult = React.useCallback(() => {
      navigator.clipboard.writeText(formattedResult)
      setCopiedResult(true)
      setTimeout(() => setCopiedResult(false), 2000)
    }, [formattedResult])
    
    return (
      <div
        ref={ref}
        className={cn(
          'rounded-lg border border-border/50',
          'bg-card/50',
          'transition-all duration-200',
          'hover:bg-card/70 hover:border-border/70',
          className
        )}
        {...props}
      >
        {/* Header - Always visible */}
        <button
          className={cn(
            'group/tool flex w-full items-center justify-between',
            'gap-4 rounded-t-lg px-4 py-3',
            'text-foreground hover:bg-muted/30',
            'transition-colors duration-200 cursor-pointer',
            'focus-visible:outline-none focus-visible:ring-2',
            'focus-visible:ring-ring focus-visible:ring-offset-2'
          )}
          onClick={() => setIsExpanded(!isExpanded)}
          aria-expanded={isExpanded}
          aria-label={`${isExpanded ? 'Collapse' : 'Expand'} tool call details`}
        >
          <div className="flex items-center gap-3 min-w-0">
            <div className="flex-shrink-0">
              <div className="h-8 w-8 rounded-md bg-primary/10 flex items-center justify-center">
                <Wrench className="h-4 w-4 text-primary" />
              </div>
            </div>
            <div className="flex flex-col items-start min-w-0">
              <span className="text-sm font-semibold">
                {toolCall.name}
              </span>
              <span className="text-xs text-muted-foreground">
                Tool executed successfully
              </span>
            </div>
          </div>
          
          <div className="flex items-center gap-2 shrink-0">
            <Check className="h-4 w-4 text-green-600 dark:text-green-400" />
            <ChevronRight className={cn(
              'h-4 w-4 transition-transform duration-200 text-muted-foreground',
              isExpanded && 'rotate-90'
            )} />
          </div>
        </button>
        
        {/* Expandable Content */}
        <AnimatePresence>
          {isExpanded && (
            <motion.div
              initial={{ height: 0, opacity: 0 }}
              animate={{ height: 'auto', opacity: 1 }}
              exit={{ height: 0, opacity: 0 }}
              transition={{ duration: 0.2, ease: 'easeOut' }}
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
                      onClick={handleCopyInput}
                    >
                      {copiedInput ? (
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
                    'bg-muted/50 rounded-md p-3 overflow-x-auto',
                    'text-xs font-mono text-muted-foreground',
                    'border border-border/30'
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
                        onClick={handleCopyResult}
                      >
                        {copiedResult ? (
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
                      'bg-muted/30 rounded-md p-3 overflow-x-auto',
                      'text-sm text-foreground',
                      'border border-border/30'
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
  }
)

ToolCallResult.displayName = 'ToolCallResult'

/**
 * ToolCallResultList Component
 * Displays multiple tool call results
 */
export interface ToolCallResultListProps extends React.HTMLAttributes<HTMLDivElement> {
  /**
   * Array of tool calls with their results
   */
  toolCalls: ToolCall[]
  /**
   * Map of tool results by tool_use_id
   */
  toolResults?: ToolResult[]
}

const ToolCallResultList = React.forwardRef<HTMLDivElement, ToolCallResultListProps>(
  ({ className, toolCalls, toolResults = [], ...props }, ref) => {
    // Filter out think tool calls
    const visibleToolCalls = React.useMemo(() => {
      return toolCalls.filter(tc => tc.name !== 'think')
    }, [toolCalls])
    
    if (visibleToolCalls.length === 0) {
      return null
    }
    
    // Create a map of results by tool_use_id for efficient lookup
    const resultsMap = React.useMemo(() => {
      const map = new Map<string, ToolResult>()
      toolResults.forEach(result => {
        map.set(result.tool_use_id, result)
      })
      return map
    }, [toolResults])
    
    return (
      <div
        ref={ref}
        className={cn('space-y-2', className)}
        {...props}
      >
        {visibleToolCalls.map((toolCall) => (
          <ToolCallResult
            key={toolCall.id}
            toolCall={toolCall}
            toolResult={resultsMap.get(toolCall.id)}
          />
        ))}
      </div>
    )
  }
)

ToolCallResultList.displayName = 'ToolCallResultList'

export { ToolCallResult, ToolCallResultList }
'use client'

import * as React from 'react'
import { cn } from '../../../lib/utils'
import { Wrench, ChevronRight, Copy, Check, Code } from 'lucide-react'
import { Button } from '../../ui/button'
import { motion, AnimatePresence } from 'framer-motion'

export interface ToolUseContentRendererProps {
  /**
   * Unique identifier for the tool invocation
   */
  id: string
  /**
   * Name of the tool being invoked
   */
  name: string
  /**
   * Input parameters for the tool
   */
  input: Record<string, any>
  /**
   * Additional CSS classes
   */
  className?: string
}

/**
 * Renders tool invocation content with expandable input details
 * Shows tool name and allows viewing/copying of input parameters
 */
export const ToolUseContentRenderer: React.FC<ToolUseContentRendererProps> = ({
  id,
  name,
  input,
  className
}) => {
  const [isExpanded, setIsExpanded] = React.useState(false)
  const [copied, setCopied] = React.useState(false)
  
  // Format the input for display
  const formattedInput = React.useMemo(() => {
    try {
      return JSON.stringify(input, null, 2)
    } catch (error) {
      console.error('Failed to stringify tool input:', error)
      return 'Unable to display input parameters'
    }
  }, [input])
  
  // Handle copy to clipboard
  const handleCopy = React.useCallback(() => {
    const content = JSON.stringify({ id, name, input }, null, 2)
    navigator.clipboard.writeText(content).then(() => {
      setCopied(true)
      setTimeout(() => setCopied(false), 2000)
    }).catch(err => {
      console.error('Failed to copy tool invocation:', err)
    })
  }, [id, name, input])
  
  // Get a preview of the input for collapsed state
  const inputPreview = React.useMemo(() => {
    const keys = Object.keys(input)
    if (keys.length === 0) return 'No parameters'
    if (keys.length === 1 && typeof input[keys[0]] === 'string') {
      const value = input[keys[0]]
      return value.length > 50 ? `${value.substring(0, 47)}...` : value
    }
    return `${keys.length} parameter${keys.length > 1 ? 's' : ''}`
  }, [input])
  
  return (
    <div 
      className={cn(
        "rounded-lg border border-blue-200 bg-blue-50/50",
        "dark:border-blue-800 dark:bg-blue-950/30",
        className
      )}
      role="region"
      aria-label={`Tool invocation: ${name}`}
      aria-expanded={isExpanded}
    >
      {/* Header - Always visible */}
      <button
        onClick={() => setIsExpanded(!isExpanded)}
        className={cn(
          "flex items-center justify-between w-full px-3 py-2 text-left",
          "hover:bg-blue-100/50 dark:hover:bg-blue-900/30 transition-colors",
          "focus-visible:outline-none focus-visible:ring-2",
          "focus-visible:ring-ring focus-visible:ring-inset rounded-t-lg"
        )}
        aria-label={`${isExpanded ? 'Collapse' : 'Expand'} tool invocation details for ${name}`}
      >
        <div className="flex items-center gap-2 min-w-0">
          <Wrench 
            className="h-4 w-4 text-blue-600 dark:text-blue-400 shrink-0" 
            aria-hidden="true"
          />
          <span className="text-sm font-medium text-blue-900 dark:text-blue-100">
            Using {name}
          </span>
          {!isExpanded && (
            <span className="text-xs text-blue-700 dark:text-blue-300 truncate">
              ({inputPreview})
            </span>
          )}
        </div>
        <ChevronRight 
          className={cn(
            "h-4 w-4 text-blue-600 dark:text-blue-400 transition-transform shrink-0",
            isExpanded && "rotate-90"
          )}
          aria-hidden="true"
        />
      </button>
      
      {/* Expandable Content */}
      <AnimatePresence>
        {isExpanded && (
          <motion.div
            initial={{ height: 0, opacity: 0 }}
            animate={{ height: "auto", opacity: 1 }}
            exit={{ height: 0, opacity: 0 }}
            transition={{ duration: 0.15, ease: "easeOut" }}
            className="overflow-hidden"
          >
            <div className="px-3 pb-3 space-y-2">
              {/* Tool ID and Copy Button */}
              <div className="flex items-center justify-between">
                <span 
                  className="text-xs text-muted-foreground font-mono"
                  aria-label="Tool invocation ID"
                >
                  ID: {id.length > 16 ? `${id.slice(0, 8)}...${id.slice(-8)}` : id}
                </span>
                <Button
                  variant="ghost"
                  size="sm"
                  className="h-6 px-2 text-xs"
                  onClick={handleCopy}
                  aria-label="Copy tool invocation details"
                >
                  {copied ? (
                    <>
                      <Check className="h-3 w-3 mr-1" aria-hidden="true" />
                      Copied
                    </>
                  ) : (
                    <>
                      <Copy className="h-3 w-3 mr-1" aria-hidden="true" />
                      Copy
                    </>
                  )}
                </Button>
              </div>
              
              {/* Input Parameters */}
              <div 
                className="bg-background/60 rounded p-2 border border-border/50"
                role="region"
                aria-label="Tool input parameters"
              >
                <div className="flex items-center gap-1 mb-1">
                  <Code className="h-3 w-3 text-muted-foreground" aria-hidden="true" />
                  <span className="text-xs text-muted-foreground font-medium">
                    Input Parameters
                  </span>
                </div>
                <pre 
                  className="text-xs overflow-auto max-h-64 font-mono"
                  aria-label="JSON representation of input parameters"
                >
                  <code>{formattedInput}</code>
                </pre>
              </div>
            </div>
          </motion.div>
        )}
      </AnimatePresence>
    </div>
  )
}

export default ToolUseContentRenderer
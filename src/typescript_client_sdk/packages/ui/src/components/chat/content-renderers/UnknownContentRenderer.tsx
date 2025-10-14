'use client'

import * as React from 'react'
import { cn } from '../../../lib/utils'
import { AlertTriangle, FileQuestion, ChevronRight, Copy, Check } from 'lucide-react'
import { Alert, AlertTitle, AlertDescription } from '../../ui/alert'
import { Button } from '../../ui/button'
import { motion, AnimatePresence } from 'framer-motion'

export interface UnknownContentRendererProps {
  /**
   * The type identifier of the unknown content
   */
  type: string
  /**
   * The raw content data for debugging
   */
  content?: any
  /**
   * Additional CSS classes
   */
  className?: string
}

/**
 * Fallback renderer for unknown or future content types
 * Provides graceful degradation and debugging information
 */
export const UnknownContentRenderer: React.FC<UnknownContentRendererProps> = ({
  type,
  content,
  className
}) => {
  const [showDebugInfo, setShowDebugInfo] = React.useState(false)
  const [copied, setCopied] = React.useState(false)
  
  // Only show debug info in development mode
  // We check for localhost or development indicators
  const isDevelopment = React.useMemo(() => {
    if (typeof window === 'undefined') return false
    const hostname = window.location.hostname
    return hostname === 'localhost' || hostname === '127.0.0.1' || hostname.startsWith('192.168.')
  }, [])
  
  // Format content for debug display
  const debugContent = React.useMemo(() => {
    if (!content) return 'No content data available'
    
    try {
      return JSON.stringify(content, null, 2)
    } catch (error) {
      return `Unable to serialize content: ${error}`
    }
  }, [content])
  
  // Handle copy debug info
  const handleCopyDebug = React.useCallback(() => {
    const debugInfo = {
      type,
      content,
      timestamp: new Date().toISOString(),
      userAgent: navigator.userAgent
    }
    
    navigator.clipboard.writeText(JSON.stringify(debugInfo, null, 2)).then(() => {
      setCopied(true)
      setTimeout(() => setCopied(false), 2000)
    }).catch(err => {
      console.error('Failed to copy debug info:', err)
    })
  }, [type, content])
  
  // Get a user-friendly message based on the type
  const getUserMessage = React.useCallback(() => {
    // Check for common future content types
    if (type === 'audio') {
      return 'Audio content is not yet supported in this version'
    }
    if (type === 'video') {
      return 'Video content is not yet supported in this version'
    }
    if (type === 'file') {
      return 'File attachments are not yet supported in this version'
    }
    if (type === 'invalid') {
      return 'This content could not be processed'
    }
    
    // Generic message for truly unknown types
    return `Content type "${type}" is not yet supported`
  }, [type])
  
  return (
    <Alert 
      variant="default" 
      className={cn(
        "border-yellow-200 bg-yellow-50/50 dark:border-yellow-800 dark:bg-yellow-950/30",
        className
      )}
      role="alert"
      aria-label={`Unknown content type: ${type}`}
    >
      <div className="flex items-start gap-2">
        <FileQuestion 
          className="h-4 w-4 text-yellow-600 dark:text-yellow-400 mt-0.5" 
          aria-hidden="true"
        />
        <div className="flex-1 space-y-1">
          <AlertTitle className="text-yellow-900 dark:text-yellow-100">
            Unsupported Content
          </AlertTitle>
          <AlertDescription className="text-yellow-800 dark:text-yellow-200">
            {getUserMessage()}
          </AlertDescription>
          
          {/* Development mode debug info */}
          {isDevelopment && (
            <div className="mt-2 space-y-2">
              <button
                onClick={() => setShowDebugInfo(!showDebugInfo)}
                className={cn(
                  "flex items-center gap-1 text-xs text-yellow-700 dark:text-yellow-300",
                  "hover:text-yellow-800 dark:hover:text-yellow-200 transition-colors",
                  "focus-visible:outline-none focus-visible:ring-2",
                  "focus-visible:ring-ring focus-visible:ring-offset-2 rounded px-1"
                )}
                aria-expanded={showDebugInfo}
                aria-label={showDebugInfo ? "Hide debug information" : "Show debug information"}
              >
                <ChevronRight 
                  className={cn(
                    "h-3 w-3 transition-transform",
                    showDebugInfo && "rotate-90"
                  )}
                  aria-hidden="true"
                />
                <span>Debug Information</span>
              </button>
              
              <AnimatePresence>
                {showDebugInfo && (
                  <motion.div
                    initial={{ height: 0, opacity: 0 }}
                    animate={{ height: "auto", opacity: 1 }}
                    exit={{ height: 0, opacity: 0 }}
                    transition={{ duration: 0.15 }}
                    className="overflow-hidden"
                  >
                    <div className="space-y-2">
                      <div className="flex items-center justify-between">
                        <span className="text-xs text-muted-foreground">
                          Type: <code className="font-mono">{type}</code>
                        </span>
                        <Button
                          variant="ghost"
                          size="sm"
                          className="h-6 px-2 text-xs"
                          onClick={handleCopyDebug}
                          aria-label="Copy debug information"
                        >
                          {copied ? (
                            <>
                              <Check className="h-3 w-3 mr-1" aria-hidden="true" />
                              Copied
                            </>
                          ) : (
                            <>
                              <Copy className="h-3 w-3 mr-1" aria-hidden="true" />
                              Copy Debug
                            </>
                          )}
                        </Button>
                      </div>
                      <pre 
                        className="text-xs bg-background/60 rounded p-2 border border-border/50 overflow-auto max-h-40 font-mono"
                        aria-label="Raw content data"
                      >
                        <code>{debugContent}</code>
                      </pre>
                      <p className="text-xs text-muted-foreground">
                        This debug information is only visible in development mode.
                      </p>
                    </div>
                  </motion.div>
                )}
              </AnimatePresence>
            </div>
          )}
          
          {/* Production mode message */}
          {!isDevelopment && (
            <p className="mt-2 text-xs text-yellow-700 dark:text-yellow-300">
              Please update to the latest version to view this content type.
            </p>
          )}
        </div>
      </div>
    </Alert>
  )
}

export default UnknownContentRenderer
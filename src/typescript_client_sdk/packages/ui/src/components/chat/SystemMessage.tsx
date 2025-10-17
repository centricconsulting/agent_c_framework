'use client'

import * as React from 'react'
import { cn } from '../../lib/utils'
import { 
  AlertCircle, Info, AlertTriangle, XCircle 
} from 'lucide-react'
import ReactMarkdown from 'react-markdown'
import remarkGfm from 'remark-gfm'

export interface SystemMessageProps extends React.HTMLAttributes<HTMLDivElement> {
  /**
   * The message content to display
   */
  content: string
  /**
   * Message severity level
   */
  severity: 'info' | 'warning' | 'error'
  /**
   * Content format
   */
  format?: 'markdown' | 'text'
  /**
   * Optional timestamp for display
   */
  timestamp?: string
}

/**
 * SystemMessage component - Renders system messages/alerts in the chat flow
 * 
 * Displays system notifications as alert boxes within the message list,
 * with visual indicators for severity levels. This is for in-chat display,
 * not floating toasts.
 */
export const SystemMessage = React.forwardRef<HTMLDivElement, SystemMessageProps>(
  ({ 
    className, 
    content,
    severity = 'info',
    format = 'markdown',
    timestamp,
    ...props 
  }, ref) => {
    
    // Format timestamp if provided
    const formattedTime = React.useMemo(() => {
      if (!timestamp) return null
      try {
        const date = new Date(timestamp)
        if (isNaN(date.getTime())) return null
        return date.toLocaleTimeString('en-US', { 
          hour: 'numeric', 
          minute: '2-digit',
          hour12: true 
        })
      } catch {
        return null
      }
    }, [timestamp])
    
    // Get icon and styling based on severity
    const severityConfig = {
      info: {
        icon: Info,
        bgClass: 'bg-blue-50 dark:bg-blue-950/30',
        borderClass: 'border-blue-200 dark:border-blue-800',
        textClass: 'text-blue-800 dark:text-blue-200',
        iconClass: 'text-blue-600 dark:text-blue-400'
      },
      warning: {
        icon: AlertTriangle,
        bgClass: 'bg-yellow-50 dark:bg-yellow-950/30',
        borderClass: 'border-yellow-200 dark:border-yellow-800',
        textClass: 'text-yellow-800 dark:text-yellow-200',
        iconClass: 'text-yellow-600 dark:text-yellow-400'
      },
      error: {
        icon: XCircle,
        bgClass: 'bg-red-50 dark:bg-red-950/30',
        borderClass: 'border-red-200 dark:border-red-800',
        textClass: 'text-red-800 dark:text-red-200',
        iconClass: 'text-red-600 dark:text-red-400'
      }
    }
    
    const config = severityConfig[severity]
    const Icon = config.icon
    
    // Render content based on format
    const renderContent = () => {
      if (format === 'markdown') {
        return (
          <div className={cn(
            "prose prose-sm max-w-none",
            "dark:prose-invert",
            "[&>*:first-child]:mt-0 [&>*:last-child]:mb-0"
          )}>
            <ReactMarkdown 
              remarkPlugins={[remarkGfm]}
              components={{
                // Override paragraph to avoid extra spacing
                p: ({ children }) => <p className="mb-2 last:mb-0">{children}</p>,
                // Override pre wrapper for code blocks
                pre: ({ children }) => (
                  <pre className="p-2 rounded bg-black/10 dark:bg-white/10 overflow-x-auto mb-2 last:mb-0">
                    {children}
                  </pre>
                ),
                // Override code element styling
                code: ({ inline, children, ...props }: any) => {
                  if (inline) {
                    return (
                      <code 
                        className="px-1 py-0.5 rounded bg-black/10 dark:bg-white/10 text-sm"
                        {...props}
                      >
                        {children}
                      </code>
                    )
                  }
                  // For block code, just return the code element (pre wrapper is handled above)
                  return (
                    <code className="text-sm" {...props}>
                      {children}
                    </code>
                  )
                }
              }}
            >
              {content}
            </ReactMarkdown>
          </div>
        )
      }
      
      // Plain text
      return (
        <div className="whitespace-pre-wrap">
          {content}
        </div>
      )
    }
    
    return (
      <div
        ref={ref}
        className={cn(
          "flex justify-center w-full py-2",
          "animate-in fade-in-50 slide-in-from-bottom-2 duration-200",
          className
        )}
        role="alert"
        aria-live={severity === 'error' ? 'assertive' : 'polite'}
        {...props}
      >
        <div className={cn(
          "max-w-2xl w-full mx-4",
          "rounded-lg border",
          "transition-all duration-200",
          config.bgClass,
          config.borderClass
        )}>
          <div className="flex gap-3 p-4">
            {/* Icon */}
            <div className="flex-shrink-0 mt-0.5">
              <Icon className={cn("h-5 w-5", config.iconClass)} />
            </div>
            
            {/* Content */}
            <div className={cn("flex-1 text-sm", config.textClass)}>
              {renderContent()}
            </div>
            
            {/* Timestamp */}
            {formattedTime && (
              <div className="flex-shrink-0 text-xs opacity-70">
                {formattedTime}
              </div>
            )}
          </div>
        </div>
      </div>
    )
  }
)

SystemMessage.displayName = 'SystemMessage'
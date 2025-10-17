'use client'

import * as React from 'react'
import { cn } from '../../lib/utils'
import { AlertCircle } from 'lucide-react'
import { MarkdownRenderer } from './content-renderers/MarkdownRenderer'

export interface MediaRendererProps extends React.HTMLAttributes<HTMLDivElement> {
  /**
   * The media content to render
   */
  content: string
  /**
   * Media content type (e.g., "text/markdown", "text/html", "image/svg+xml")
   */
  contentType: string
  /**
   * Optional metadata about the source
   */
  metadata?: {
    sentByClass?: string
    sentByFunction?: string
    foreignContent?: boolean
    url?: string
    name?: string
  }
  /**
   * Optional timestamp for display
   */
  timestamp?: string
}

/**
 * MediaRenderer component - Renders RenderMedia events in the chat
 * 
 * Displays media content in a distinct bubble without user/assistant avatars,
 * with optional footer metadata showing the source of the content.
 */
export const MediaRenderer = React.forwardRef<HTMLDivElement, MediaRendererProps>(
  ({ 
    className, 
    content,
    contentType,
    metadata = {},
    timestamp,
    ...props 
  }, ref) => {
    
    // Parse content type to determine rendering approach
    const [mediaType, mediaSubtype] = contentType.split('/') || ['text', 'plain']
    
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
    
    // Render content based on type
    const renderContent = () => {
      // Markdown content
      if (contentType === 'text/markdown' || mediaSubtype === 'markdown') {
        return (
          <MarkdownRenderer
            content={content}
            ariaLabel="Media content markdown"
          />
        )
      }
      
      // HTML content - render in sandboxed iframe for safety
      if (contentType === 'text/html' || mediaSubtype === 'html') {
        return (
          <div className="w-full">
            {metadata.foreignContent && (
              <div className="flex items-center gap-2 p-2 mb-2 text-sm text-yellow-600 dark:text-yellow-400 bg-yellow-50 dark:bg-yellow-900/20 rounded">
                <AlertCircle className="h-4 w-4" />
                <span>External content</span>
              </div>
            )}
            <div 
              className="w-full p-4 bg-background rounded border border-border overflow-auto"
              dangerouslySetInnerHTML={{ __html: content }}
            />
          </div>
        )
      }
      
      // Image content
      if (mediaType === 'image') {
        // For SVG, render inline
        if (mediaSubtype === 'svg+xml') {
          return (
            <div 
              className="w-full max-w-lg"
              dangerouslySetInnerHTML={{ __html: content }}
            />
          )
        }
        
        // For other images, try to render as data URL or regular URL
        const imageSrc = content.startsWith('data:') || content.startsWith('http') 
          ? content 
          : `data:${contentType};base64,${content}`
          
        return (
          <img 
            src={imageSrc} 
            alt={metadata.name || 'Media content'}
            className="max-w-full h-auto rounded"
          />
        )
      }
      
      // Plain text or unknown - render as preformatted text
      return (
        <pre className="whitespace-pre-wrap font-mono text-sm">
          {content}
        </pre>
      )
    }
    
    return (
      <div
        ref={ref}
        className={cn(
          "flex flex-col w-full py-2",
          "animate-in fade-in-50 slide-in-from-bottom-2 duration-200",
          className
        )}
        role="article"
        aria-label="Media content"
        {...props}
      >
        <div className={cn(
          "max-w-[85%] w-full mx-4",
          "rounded-xl overflow-hidden",
          "bg-muted/50 border border-border/50",
          "transition-all duration-200 hover:border-border/70"
        )}>
          {/* Content area */}
          <div className="p-4">
            {renderContent()}
          </div>
          
          {/* Footer with timestamp and metadata */}
          {(timestamp || metadata.sentByClass || metadata.sentByFunction || metadata.url) && (
            <div className="px-4 py-2 border-t border-border/30 bg-background/30">
              <div className="flex items-center gap-4 text-xs text-muted-foreground">
                {formattedTime && (
                  <span className="opacity-70">{formattedTime}</span>
                )}
                {metadata.sentByClass && (
                  <span>
                    <span className="opacity-70">Class:</span> {metadata.sentByClass}
                  </span>
                )}
                {metadata.sentByFunction && (
                  <span>
                    <span className="opacity-70">Function:</span> {metadata.sentByFunction}
                  </span>
                )}
                {metadata.url && (
                  <a 
                    href={metadata.url} 
                    target="_blank" 
                    rel="noopener noreferrer"
                    className="hover:underline"
                  >
                    Source
                  </a>
                )}
              </div>
            </div>
          )}
        </div>
      </div>
    )
  }
)

MediaRenderer.displayName = 'MediaRenderer'
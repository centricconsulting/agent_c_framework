'use client'

import * as React from 'react'
import { useChat, useToolNotifications, useErrors } from '@agentc/realtime-react'
import { 
  isMessageItem, 
  isDividerItem, 
  isMediaItem, 
  isSystemAlertItem,
  type ChatItem,
  type MessageChatItem
} from '@agentc/realtime-react'
import { cn } from '../../lib/utils'
import { Message } from './Message'
import { TypingIndicator } from './TypingIndicator'
import { SubsessionDivider } from './SubsessionDivider'
import { MediaRenderer } from './MediaRenderer'
import { SystemMessage } from './SystemMessage'
import { ToolNotificationList } from './ToolNotification'
import { Loader2, MessageSquare } from 'lucide-react'
import { Logger } from '../../utils/logger'
import { toast } from 'sonner'

export interface MessageListProps extends React.HTMLAttributes<HTMLDivElement> {
  /**
   * Maximum height for the message list
   */
  maxHeight?: string
  /**
   * Show timestamps on messages
   */
  showTimestamps?: boolean
  /**
   * Enable virtual scrolling for performance
   */
  enableVirtualScroll?: boolean
  /**
   * Custom empty state component
   */
  emptyStateComponent?: React.ReactNode
}

/**
 * MessageList component with intelligent auto-scrolling behavior:
 * 
 * - Auto-scrolls to bottom when new messages, streaming content, or typing indicators appear
 * - Disables auto-scroll when user manually scrolls up to review older messages
 * - Re-enables auto-scroll when user scrolls back to within 50px of the bottom
 * - Uses smooth scrolling for better UX
 * 
 * The auto-scroll state is managed internally and responds to user interaction patterns
 * common in chat applications.
 */
const MessageList = React.forwardRef<HTMLDivElement, MessageListProps>(
  ({ 
    className, 
    maxHeight = '600px',
    showTimestamps = true,
    enableVirtualScroll = false,
    emptyStateComponent,
    ...props 
  }, ref) => {
    const { messages, isAgentTyping, streamingMessage, currentSessionId } = useChat()
    const { notifications: toolNotifications } = useToolNotifications({
      autoRemoveCompleted: false, // We'll handle removal when tool completes
      maxNotifications: 5
    })
    const { errors, dismissError } = useErrors()
    
    const scrollContainerRef = React.useRef<HTMLDivElement>(null)
    const scrollSentinelRef = React.useRef<HTMLDivElement>(null)
    const [isLoading, setIsLoading] = React.useState(false)
    const [isAutoScrollEnabled, setIsAutoScrollEnabled] = React.useState(true)
    const [hasCompletedInitialScroll, setHasCompletedInitialScroll] = React.useState(false)
    const [isProgrammaticScroll, setIsProgrammaticScroll] = React.useState(false)
    const programmaticScrollTimeout = React.useRef<NodeJS.Timeout>()
    const previousSessionIdRef = React.useRef(currentSessionId)
    const previousMessageCountRef = React.useRef(0)
    const isInitialMount = React.useRef(true)
    const scrollThreshold = 50 // pixels from bottom to re-enable auto-scroll
    
    // Handle error toasts
    React.useEffect(() => {
      if (errors.length > 0) {
        // Show the latest error as a toast
        const latestError = errors[errors.length - 1]
        if (!latestError.dismissed) {
          toast.error(latestError.message, {
            id: latestError.id,
            description: latestError.source,
            duration: 5000,
            onDismiss: () => dismissError(latestError.id)
          })
        }
      }
    }, [errors, dismissError])
    
    // Debug logging for messages
    React.useEffect(() => {
      Logger.debug('[MessageList] Received messages:', messages.length, 'items')
      if (messages.length > 0) {
        Logger.debug('[MessageList] First 3 items:', messages.slice(0, 3).map(item => ({
          type: item.type,
          id: item.id,
          ...(isMessageItem(item) ? { role: item.role } : {})
        })))
      }
    }, [messages])
    
    // Check if scroll is near bottom
    const isNearBottom = React.useCallback(() => {
      if (!scrollContainerRef.current) return true
      
      const { scrollTop, scrollHeight, clientHeight } = scrollContainerRef.current
      const distanceFromBottom = scrollHeight - scrollTop - clientHeight
      
      return distanceFromBottom <= scrollThreshold
    }, [scrollThreshold])
    
    // Wait for DOM updates to complete
    const waitForDOMUpdate = React.useCallback(() => {
      return new Promise<void>(resolve => {
        // Use double RAF to ensure layout is complete
        requestAnimationFrame(() => {
          requestAnimationFrame(() => {
            resolve()
          })
        })
      })
    }, [])
    
    // Scroll to bottom function
    const scrollToBottom = React.useCallback((smooth = true) => {
      if (!scrollSentinelRef.current) return
      
      // Mark as programmatic
      setIsProgrammaticScroll(true)
      
      // Clear any existing timeout
      if (programmaticScrollTimeout.current) {
        clearTimeout(programmaticScrollTimeout.current)
      }
      
      // Use the sentinel element as the scroll target
      scrollSentinelRef.current.scrollIntoView({ 
        behavior: smooth ? 'smooth' : 'auto',
        block: 'end'
      })
      
      // Reset flag after animation completes
      const duration = smooth ? 600 : 50 // Smooth animations take ~500ms
      programmaticScrollTimeout.current = setTimeout(() => {
        setIsProgrammaticScroll(false)
      }, duration)
    }, [])
    
    // Handle scroll events to detect user scrolling
    const handleScroll = React.useCallback(() => {
      // Ignore if this is our own scroll
      if (isProgrammaticScroll) {
        return
      }
      
      // Ignore scroll events until initial scroll is complete
      if (!hasCompletedInitialScroll) {
        Logger.debug('[MessageList] Ignoring scroll event - initial scroll not complete')
        return
      }
      
      const nearBottom = isNearBottom()
      
      // Re-enable auto-scroll if user scrolls back to bottom
      if (nearBottom && !isAutoScrollEnabled) {
        Logger.debug('[MessageList] Re-enabling auto-scroll - user scrolled to bottom')
        setIsAutoScrollEnabled(true)
      }
      // Disable auto-scroll if user scrolls away from bottom
      else if (!nearBottom && isAutoScrollEnabled) {
        Logger.debug('[MessageList] Disabling auto-scroll - user scrolled up')
        setIsAutoScrollEnabled(false)
      }
    }, [isProgrammaticScroll, isNearBottom, isAutoScrollEnabled, hasCompletedInitialScroll])
    
    // Set up scroll event listener
    React.useEffect(() => {
      const container = scrollContainerRef.current
      if (!container) return
      
      container.addEventListener('scroll', handleScroll, { passive: true })
      
      return () => {
        container.removeEventListener('scroll', handleScroll)
      }
    }, [handleScroll])
    
    // Track session changes and reset state
    React.useEffect(() => {
      if (previousSessionIdRef.current !== currentSessionId && currentSessionId) {
        // Session changed - reset all scroll state
        Logger.debug('[MessageList] Session changed - resetting scroll state')
        setHasCompletedInitialScroll(false)
        setIsAutoScrollEnabled(true)
        setIsProgrammaticScroll(false)
        
        // Clear any pending timeouts
        if (programmaticScrollTimeout.current) {
          clearTimeout(programmaticScrollTimeout.current)
        }
        
        previousSessionIdRef.current = currentSessionId
      }
    }, [currentSessionId])
    
    // Handle initial mount - let auto-scroll effect handle the actual scrolling
    React.useEffect(() => {
      if (isInitialMount.current) {
        if (messages.length === 0) {
          // No messages on mount, mark initial scroll as complete
          Logger.debug('[MessageList] Initial mount with no messages')
          setHasCompletedInitialScroll(true)
        }
        // If there are messages, the auto-scroll effect will handle them
        isInitialMount.current = false
      }
    }, [messages.length])
    
    // Auto-scroll when new content arrives
    React.useEffect(() => {
      // Skip if auto-scroll is disabled
      if (!isAutoScrollEnabled || messages.length === 0) {
        previousMessageCountRef.current = messages.length
        return
      }
      
      const hasNewContent = messages.length !== previousMessageCountRef.current
      const isInitialLoad = previousMessageCountRef.current === 0 && messages.length > 0
      
      if (hasNewContent || streamingMessage || isAgentTyping || toolNotifications.length > 0) {
        // Wait for DOM to update, then scroll
        waitForDOMUpdate().then(() => {
          // Use instant scroll for initial/bulk loads, smooth for incremental
          const useSmooth = !isInitialLoad && messages.length - previousMessageCountRef.current <= 2
          Logger.debug('[MessageList] Auto-scrolling for new content (smooth: ' + useSmooth + ')')
          scrollToBottom(useSmooth)
          
          if (!hasCompletedInitialScroll) {
            setHasCompletedInitialScroll(true)
          }
        })
      }
      
      previousMessageCountRef.current = messages.length
    }, [messages.length, streamingMessage, isAgentTyping, toolNotifications, isAutoScrollEnabled, scrollToBottom, waitForDOMUpdate, hasCompletedInitialScroll])
    
    // Cleanup timeout on unmount
    React.useEffect(() => {
      return () => {
        if (programmaticScrollTimeout.current) {
          clearTimeout(programmaticScrollTimeout.current)
        }
      }
    }, [])
    
    // Combine ref forwarding with internal ref
    React.useImperativeHandle(ref, () => scrollContainerRef.current as HTMLDivElement)
    
    // Virtual scrolling logic (simplified for now - can be enhanced with react-window)
    const visibleItems = React.useMemo(() => {
      Logger.debug('[MessageList] Computing visible items');
      Logger.debug('[MessageList] enableVirtualScroll:', enableVirtualScroll);
      Logger.debug('[MessageList] messages length:', messages.length);
      
      if (!enableVirtualScroll) {
        Logger.debug('[MessageList] Returning all items (no virtual scroll)');
        return messages
      }
      // For now, return all messages - in production, implement windowing
      return messages
    }, [messages, enableVirtualScroll])
    
    // Empty state
    const EmptyState = () => {
      if (emptyStateComponent) return <>{emptyStateComponent}</>
      
      return (
        <div className="flex flex-col items-center justify-center py-12 text-center">
          <MessageSquare className="h-12 w-12 text-muted-foreground mb-4" />
          <h3 className="text-lg font-semibold mb-2">No messages yet</h3>
          <p className="text-sm text-muted-foreground max-w-sm">
            Start a conversation to see messages appear here
          </p>
        </div>
      )
    }
    
    // Loading state
    if (isLoading) {
      return (
        <div 
          className={cn(
            "flex items-center justify-center p-8",
            className
          )}
          {...props}
        >
          <div className="flex items-center gap-2 text-muted-foreground">
            <Loader2 className="h-4 w-4 animate-spin" />
            <span className="text-sm">Loading messages...</span>
          </div>
        </div>
      )
    }
    
    // Render a chat item based on its type
    const renderChatItem = (item: ChatItem, index: number) => {
      // Check for divider items (subsessions)
      if (isDividerItem(item)) {
        return (
          <SubsessionDivider
            key={item.id}
            type={item.dividerType}
            timestamp={item.timestamp}
            label={item.metadata?.subAgentKey 
              ? `Agent: ${item.metadata.subAgentKey}` 
              : undefined}
            className="my-2"
          />
        )
      }
      
      // Check for media items (RenderMedia)
      if (isMediaItem(item)) {
        return (
          <MediaRenderer
            key={item.id}
            content={item.content}
            contentType={item.contentType}
            metadata={item.metadata}
            timestamp={item.timestamp}
            className="animate-in slide-in-from-bottom-2 duration-200"
          />
        )
      }
      
      // Check for system alert items
      if (isSystemAlertItem(item)) {
        return (
          <SystemMessage
            key={item.id}
            content={item.content}
            severity={item.severity}
            format={item.format}
            timestamp={item.timestamp}
            className="animate-in slide-in-from-bottom-2 duration-200"
          />
        )
      }
      
      // Check for message items (regular messages)
      if (isMessageItem(item)) {
        return (
          <Message
            key={item.id}
            message={item}
            showTimestamp={showTimestamps}
            isSubSession={item.isSubSession}
            className="animate-in slide-in-from-bottom-2 duration-200"
          />
        )
      }
      
      // Unknown item type - shouldn't happen but handle gracefully
      Logger.warn('[MessageList] Unknown chat item type:', item)
      return null
    }
    
    return (
      <div
        ref={scrollContainerRef}
        className={cn(
          "relative flex flex-col overflow-y-auto",
          className
        )}
        style={{ maxHeight: maxHeight === 'none' ? undefined : maxHeight }}
        role="log"
        aria-label="Chat messages"
        aria-live="polite"
        {...props}
      >
        {/* Messages container */}
        <div className="space-y-4">
          {visibleItems.length === 0 ? (
            <EmptyState />
          ) : (
            <>
              {/* Render all chat items using unified renderer */}
              {visibleItems.map((item, index) => renderChatItem(item, index))}
              
              {/* Current streaming response */}
              {streamingMessage && (
                <Message
                  message={streamingMessage}
                  showTimestamp={showTimestamps}
                  isStreaming
                  isSubSession={streamingMessage.isSubSession}
                  className="animate-in slide-in-from-bottom-2 duration-200"
                />
              )}
              
              {/* Tool notifications */}
              {toolNotifications.length > 0 && (
                <ToolNotificationList
                  notifications={toolNotifications}
                  maxNotifications={3}
                  className="animate-in slide-in-from-bottom-2 duration-200"
                />
              )}
              
              {/* Typing indicator - only show if no streaming message and no active tools */}
              {isAgentTyping && !streamingMessage && toolNotifications.length === 0 && (
                <div className="flex items-start gap-3 animate-in slide-in-from-bottom-2 duration-200">
                  <div className="h-8 w-8 rounded-full bg-primary/10 flex items-center justify-center">
                    <span className="text-xs font-medium text-primary">AI</span>
                  </div>
                  <div className="flex-1">
                    <TypingIndicator />
                  </div>
                </div>
              )}
            </>
          )}
        </div>
        
        {/* Sentinel element for scroll targeting */}
        <div
          ref={scrollSentinelRef}
          data-testid="scroll-sentinel"
          style={{ height: '1px', visibility: 'hidden' }}
          aria-hidden="true"
        />
      </div>
    )
  }
)

MessageList.displayName = 'MessageList'

export { MessageList }
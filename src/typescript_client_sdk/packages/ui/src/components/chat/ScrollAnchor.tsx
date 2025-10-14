"use client"

import * as React from 'react'
import { cn } from '../../lib/utils'
import { Button } from '../ui/button'
import { ArrowDown } from 'lucide-react'

export interface ScrollAnchorProps {
  /**
   * Reference to the scroll container
   */
  scrollContainerRef: React.RefObject<HTMLElement>
  /**
   * Dependencies that trigger scroll checks
   */
  dependencies?: any[]
  /**
   * Threshold in pixels from bottom to consider "scrolled to bottom"
   */
  threshold?: number
  /**
   * Show floating button when scrolled up
   */
  showFloatingButton?: boolean
  /**
   * Custom floating button component
   */
  floatingButtonComponent?: React.ReactNode
  /**
   * Callback when auto-scroll state changes
   */
  onAutoScrollChange?: (enabled: boolean) => void
}

const ScrollAnchor: React.FC<ScrollAnchorProps> = ({
  scrollContainerRef,
  dependencies = [],
  threshold = 100,
  showFloatingButton = true,
  floatingButtonComponent,
  onAutoScrollChange
}) => {
  const [isAutoScrollEnabled, setIsAutoScrollEnabled] = React.useState(true)
  const [showNewMessagesButton, setShowNewMessagesButton] = React.useState(false)
  const [newMessageCount, setNewMessageCount] = React.useState(0)
  const anchorRef = React.useRef<HTMLDivElement>(null)
  const observerRef = React.useRef<IntersectionObserver | null>(null)
  const previousDependenciesRef = React.useRef(dependencies)
  
  // Check if scrolled to bottom
  const isScrolledToBottom = React.useCallback(() => {
    const container = scrollContainerRef.current
    if (!container) return true
    
    const { scrollTop, scrollHeight, clientHeight } = container
    const distanceFromBottom = scrollHeight - scrollTop - clientHeight
    return distanceFromBottom <= threshold
  }, [scrollContainerRef, threshold])
  
  // Smooth scroll to bottom
  const scrollToBottom = React.useCallback((smooth = true) => {
    const container = scrollContainerRef.current
    if (!container) return
    
    if (smooth && 'scrollBehavior' in document.documentElement.style) {
      container.scrollTo({
        top: container.scrollHeight,
        behavior: 'smooth'
      })
    } else {
      container.scrollTop = container.scrollHeight
    }
    
    setShowNewMessagesButton(false)
    setNewMessageCount(0)
  }, [scrollContainerRef])
  
  // Handle manual scroll
  const handleScroll = React.useCallback(() => {
    const wasAutoScrollEnabled = isAutoScrollEnabled
    const isAtBottom = isScrolledToBottom()
    
    if (isAtBottom) {
      // Re-enable auto-scroll when user scrolls to bottom
      if (!wasAutoScrollEnabled) {
        setIsAutoScrollEnabled(true)
        setShowNewMessagesButton(false)
        setNewMessageCount(0)
        onAutoScrollChange?.(true)
      }
    } else {
      // Disable auto-scroll when user scrolls up
      if (wasAutoScrollEnabled) {
        setIsAutoScrollEnabled(false)
        onAutoScrollChange?.(false)
      }
    }
  }, [isAutoScrollEnabled, isScrolledToBottom, onAutoScrollChange])
  
  // Set up Intersection Observer for efficient detection
  React.useEffect(() => {
    const anchor = anchorRef.current
    if (!anchor) return
    
    observerRef.current = new IntersectionObserver(
      ([entry]) => {
        const isVisible = entry.isIntersecting
        
        if (isVisible) {
          setIsAutoScrollEnabled(true)
          setShowNewMessagesButton(false)
          setNewMessageCount(0)
          onAutoScrollChange?.(true)
        } else {
          // Only disable if we're not already at the bottom
          if (!isScrolledToBottom()) {
            setIsAutoScrollEnabled(false)
            onAutoScrollChange?.(false)
          }
        }
      },
      {
        root: scrollContainerRef.current,
        rootMargin: `0px 0px ${threshold}px 0px`,
        threshold: 0
      }
    )
    
    observerRef.current.observe(anchor)
    
    return () => {
      observerRef.current?.disconnect()
    }
  }, [scrollContainerRef, threshold, isScrolledToBottom, onAutoScrollChange])
  
  // Set up scroll listener
  React.useEffect(() => {
    const container = scrollContainerRef.current
    if (!container) return
    
    container.addEventListener('scroll', handleScroll, { passive: true })
    
    return () => {
      container.removeEventListener('scroll', handleScroll)
    }
  }, [scrollContainerRef, handleScroll])
  
  // Auto-scroll when dependencies change
  React.useEffect(() => {
    // Check if dependencies actually changed
    const hasChanged = dependencies.some((dep, index) => 
      dep !== previousDependenciesRef.current[index]
    )
    
    if (!hasChanged) return
    
    previousDependenciesRef.current = dependencies
    
    if (isAutoScrollEnabled) {
      // Use requestAnimationFrame to ensure DOM updates are complete
      requestAnimationFrame(() => {
        scrollToBottom()
      })
    } else if (showFloatingButton) {
      // Show new messages indicator if not auto-scrolling
      setShowNewMessagesButton(true)
      setNewMessageCount(prev => prev + 1)
    }
  // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [dependencies?.length, isAutoScrollEnabled, showFloatingButton, scrollToBottom, ...dependencies])
  
  // Floating button component
  const FloatingButton = () => {
    if (floatingButtonComponent) return <>{floatingButtonComponent}</>
    
    return (
      <div className="absolute bottom-4 left-1/2 -translate-x-1/2 z-10">
        <Button
          variant="secondary"
          size="sm"
          onClick={() => {
            scrollToBottom()
            setIsAutoScrollEnabled(true)
          }}
          className={cn(
            "rounded-full shadow-lg transition-all duration-200",
            "hover:shadow-xl hover:scale-105",
            showNewMessagesButton 
              ? "opacity-100 translate-y-0" 
              : "opacity-0 translate-y-2 pointer-events-none"
          )}
          aria-label={`Scroll to bottom${newMessageCount > 0 ? `, ${newMessageCount} new messages` : ''}`}
        >
          <ArrowDown className="h-4 w-4 mr-1" />
          <span>
            {newMessageCount > 0 ? (
              <>New messages ({newMessageCount})</>
            ) : (
              <>Back to bottom</>
            )}
          </span>
        </Button>
      </div>
    )
  }
  
  return (
    <>
      {/* Floating button for scrolling to bottom */}
      {showFloatingButton && <FloatingButton />}
      
      {/* Invisible anchor element at the bottom */}
      <div 
        ref={anchorRef}
        className="h-px w-full"
        aria-hidden="true"
      />
      
      {/* Screen reader announcement for new messages */}
      {newMessageCount > 0 && (
        <div className="sr-only" role="status" aria-live="polite">
          {newMessageCount} new {newMessageCount === 1 ? 'message' : 'messages'} available
        </div>
      )}
    </>
  )
}

ScrollAnchor.displayName = 'ScrollAnchor'

export { ScrollAnchor }
"use client"

import * as React from "react"
import { cn } from "../../lib/utils"
import { MessageList } from "./MessageList"
import { ScrollAnchor } from "./ScrollAnchor"

export interface ChatMessagesViewProps {
  className?: string
}

/**
 * ChatMessagesView component - Main chat message display container
 * Integrates MessageList with typing indicators and auto-scrolling
 */
export const ChatMessagesView = React.forwardRef<HTMLDivElement, ChatMessagesViewProps>(
  ({ className }, ref) => {
    const scrollRef = React.useRef<HTMLDivElement>(null)

    return (
      <div
        ref={ref}
        className={cn(
          "h-full overflow-hidden",
          className
        )}
      >
        {/* Scrollable message area - takes full height */}
        <div 
          ref={scrollRef}
          className="h-full overflow-y-auto"
        >
          {/* Message list - remove fixed height, let it grow naturally */}
          <MessageList 
            className="max-w-4xl mx-auto px-4 py-4"
            maxHeight="none"
          />
          
          {/* Scroll anchor for auto-scrolling */}
          <ScrollAnchor scrollContainerRef={scrollRef} />
        </div>
      </div>
    )
  }
)
ChatMessagesView.displayName = "ChatMessagesView"
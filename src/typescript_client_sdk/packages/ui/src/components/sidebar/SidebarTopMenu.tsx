"use client"

import * as React from "react"
import { Plus } from "lucide-react"
import { buttonVariants } from "../ui/button"
import { cn } from "../../lib/utils"
import { useRealtimeClientSafe, useAgentCData } from "@agentc/realtime-react"
import type { NewChatSessionEvent } from "@agentc/realtime-core"

export interface SidebarTopMenuProps {
  onNewChat?: () => void
  isCollapsed?: boolean
  className?: string
  /**
   * The href for the new chat link
   * Defaults to "/chat" but can be overridden for custom routing
   */
  newChatHref?: string
}

/**
 * SidebarTopMenu component - Contains the "New Chat" button/link
 * Provides session management actions at the top of the sidebar
 * 
 * The button is implemented as a link to support:
 * - Middle-click to open new chat in new tab
 * - Right-click context menu "Open in new tab"
 * - Normal click creates new session in current tab
 */
export const SidebarTopMenu = React.forwardRef<HTMLDivElement, SidebarTopMenuProps>(
  ({ onNewChat, isCollapsed, className, newChatHref = "/chat" }, ref) => {
    const client = useRealtimeClientSafe()
    const { data } = useAgentCData()

    const handleNewChat = React.useCallback((e: React.MouseEvent<HTMLAnchorElement>) => {
      // Allow middle-click, Ctrl+click, Cmd+click, or Shift+click to open in new tab
      // These will use the browser's native "open in new tab" behavior
      if (e.button === 1 || e.metaKey || e.ctrlKey || e.shiftKey) {
        // Don't prevent default - let browser handle opening in new tab
        return
      }

      // For normal left-click, prevent navigation and create new session in current tab
      e.preventDefault()
      
      // Send the NewChatSessionEvent to the server
      const event: NewChatSessionEvent = {
        type: 'new_chat_session',
        agent_key: data?.currentAgentConfig?.key
      }
      
      // Send the event using the realtime client
      client?.sendEvent(event)
      
      // Call the onNewChat callback after sending the event if provided
      // This maintains backward compatibility
      if (onNewChat) {
        onNewChat()
      }
    }, [client, data?.currentAgentConfig?.key, onNewChat])

    return (
      <div
        ref={ref}
        className={cn(
          "w-full",
          className
        )}
      >
        <a
          href={newChatHref}
          onClick={handleNewChat}
          className={cn(
            buttonVariants({ variant: "default", size: "default" }),
            "w-full no-underline",
            isCollapsed && "px-0"
          )}
          aria-label="Start a new chat"
          title={isCollapsed ? "New Chat" : undefined}
        >
          <Plus className={cn("h-4 w-4", !isCollapsed && "mr-2")} />
          {!isCollapsed && <span className="font-medium">New Chat</span>}
        </a>
      </div>
    )
  }
)
SidebarTopMenu.displayName = "SidebarTopMenu"
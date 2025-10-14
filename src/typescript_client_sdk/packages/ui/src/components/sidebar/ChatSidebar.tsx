"use client"

import * as React from "react"
import { cn } from "../../lib/utils"
import { SidebarTopMenu } from "./SidebarTopMenu"
import { ChatSessionList } from "../session/ChatSessionList"
import { UserDisplay } from "./UserDisplay"
import { AgentCLogo } from "../ui/agent-c-logo"
import { X, ChevronLeft, ChevronRight } from "lucide-react"
import { Button } from "../ui/button"

export interface ChatSidebarProps {
  isOpen?: boolean
  isCollapsed?: boolean
  onClose?: () => void
  onToggleCollapse?: () => void
  onLogout?: () => void
  className?: string
}

/**
 * ChatSidebar component - Main sidebar container with responsive behavior
 * Desktop: Sticky sidebar always visible (can be collapsed but not hidden)
 * Mobile: Overlay with backdrop (can be fully hidden)
 */
export const ChatSidebar = React.forwardRef<HTMLDivElement, ChatSidebarProps>(
  ({ isOpen = true, isCollapsed = false, onClose, onToggleCollapse, onLogout, className }, ref) => {
    const [isMobile, setIsMobile] = React.useState(false)

    // Detect mobile viewport
    React.useEffect(() => {
      const checkMobile = () => {
        setIsMobile(window.innerWidth < 1024)
      }
      
      checkMobile()
      window.addEventListener("resize", checkMobile)
      return () => window.removeEventListener("resize", checkMobile)
    }, [])

    // Mobile overlay - only render when mobile AND open
    if (isMobile && isOpen) {
      return (
        <>
          {/* Backdrop */}
          <div 
            className="fixed inset-0 z-40 bg-background/80 backdrop-blur-sm lg:hidden"
            onClick={onClose}
            aria-hidden="true"
          />
          
          {/* Mobile Sidebar */}
          <div
            ref={ref}
            className={cn(
              "fixed inset-y-0 left-0 z-50 w-72 bg-background shadow-lg",
              "flex flex-col",
              "animate-in slide-in-from-left duration-200",
              className
            )}
          >
            {/* Mobile Close Button */}
            <div className="flex items-center justify-between p-4">
              <span className="text-base font-semibold">Navigation</span>
              <Button
                variant="ghost"
                size="icon"
                onClick={onClose}
                className="h-9 w-9"
                aria-label="Close sidebar"
              >
                <X className="h-4 w-4" />
              </Button>
            </div>
            
            {/* Sidebar Content with proper flex layout */}
            <div className="flex flex-col flex-1 overflow-hidden">
              <SidebarTopMenu />
              <div className="flex-1 overflow-y-auto">
                <ChatSessionList />
              </div>
              <div className="mt-auto">
                <UserDisplay onLogout={onLogout} />
              </div>
            </div>
          </div>
        </>
      )
    }

    // Desktop sidebar - always visible when not mobile
    // Mobile - hidden when closed
    if (!isMobile) {
      return (
        <div
          ref={ref}
          className={cn(
            "border-r border-border bg-sidebar transition-all duration-300 ease-in-out",
            "sticky top-0 h-screen flex flex-col",
            isCollapsed ? "w-16" : "w-64",
            className
          )}
        >
          {/* Sidebar Header with Title and Collapse Button */}
          <div className="flex items-center justify-between p-4 border-b border-border">
            <div className="flex items-center gap-2 overflow-hidden">
              <span className={cn(
                "text-lg font-semibold transition-opacity duration-200 whitespace-nowrap",
                isCollapsed ? "opacity-0 w-0" : "opacity-100"
              )}>
                Agent
              </span>
              <AgentCLogo 
                className={cn(
                  "h-7 w-auto transition-all duration-200 flex-shrink-0",
                  isCollapsed && "h-8"
                )}
              />
            </div>
            <Button
              variant="ghost"
              size="icon"
              onClick={onToggleCollapse}
              className="h-10 w-10 flex-shrink-0"
              aria-label={isCollapsed ? "Expand sidebar" : "Collapse sidebar"}
            >
              {isCollapsed ? (
                <ChevronRight className="h-4 w-4" />
              ) : (
                <ChevronLeft className="h-4 w-4" />
              )}
            </Button>
          </div>
          
          {/* Sidebar Content with proper flex layout */}
          <div className={cn(
            "flex flex-col flex-1 overflow-hidden",
            isCollapsed && "items-center"
          )}>
            {/* Top menu section */}
            <div className="p-4">
              <SidebarTopMenu isCollapsed={isCollapsed} />
            </div>
            
            {/* Chat sessions list - takes up available space */}
            <div className="flex-1 overflow-y-auto px-4">
              <ChatSessionList isCollapsed={isCollapsed} />
            </div>
            
            {/* User display - pinned to bottom */}
            <div className="border-t border-border">
              <UserDisplay isCollapsed={isCollapsed} onLogout={onLogout} />
            </div>
          </div>
        </div>
      )
    }

    // Mobile closed state - render nothing
    return null
  }
)
ChatSidebar.displayName = "ChatSidebar"
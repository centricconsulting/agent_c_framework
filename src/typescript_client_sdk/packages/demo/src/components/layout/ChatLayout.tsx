"use client"

import * as React from "react"
import { cn } from "@/lib/utils"
import { ChatSidebar, MainContentArea } from "@agentc/realtime-ui"
import type { ContentOutputMode } from "@agentc/realtime-ui"
import { useRouter } from "next/navigation"
import { useAuth } from "@/contexts/auth-context"
import { ChatHeader } from "./ChatHeader"
// Import the SSR-safe wrapper instead of the UI package directly
// This prevents TipTap from breaking the Next.js build
import { InputAreaWrapper } from "../input/InputAreaWrapper"

export interface ChatLayoutProps {
  outputMode?: ContentOutputMode
  sessionName?: string
  className?: string
  children?: React.ReactNode
}

/**
 * ChatLayout component - Main container for the entire chat interface
 * Manages overall layout structure with sidebar, header, content area, and input
 */
export const ChatLayout = React.forwardRef<HTMLDivElement, ChatLayoutProps>(  ({ outputMode = 'chat', sessionName, className, children }, ref) => {
    const router = useRouter()
    const { logout } = useAuth()
    const [sidebarOpen, setSidebarOpen] = React.useState(false)
    const [sidebarCollapsed, setSidebarCollapsed] = React.useState(false)
    const [currentOutputMode, setCurrentOutputMode] = React.useState<ContentOutputMode>(outputMode)

    const handleLogout = React.useCallback(() => {
      logout()
      router.push("/login")
    }, [logout, router])

    // Note: Output mode is controlled by the InputArea component internally
    // The display mode will update based on the SDK's state
    // For now we'll use a default mode

    return (
      <div
        ref={ref}
        className={cn(
          "min-h-screen bg-background text-foreground flex",
          className
        )}
      >
        {/* Sidebar */}
        <ChatSidebar 
          isOpen={sidebarOpen}
          isCollapsed={sidebarCollapsed}
          onClose={() => setSidebarOpen(false)}
          onToggleCollapse={() => setSidebarCollapsed(!sidebarCollapsed)}
          onLogout={handleLogout}
        />

        {/* Main Content Area */}
        <div className="flex-1 flex flex-col h-screen overflow-hidden">
          {/* Header */}
          <ChatHeader 
            onMenuToggle={() => setSidebarOpen(!sidebarOpen)}
            sessionName={sessionName}
          />

          {/* Dynamic Content Area - flex-1 to take remaining space */}
          <div className="flex-1 min-h-0 overflow-hidden">
            <MainContentArea outputMode={currentOutputMode} />
          </div>

          {/* Input Area - flex-shrink-0 to always remain visible */}
          <div className="flex-shrink-0 bg-accent/10 border-t border-border">
            <div className="max-w-4xl mx-auto p-4">
              <InputAreaWrapper 
                className="w-full"
              />
            </div>
          </div>
        </div>
      </div>
    )
  }
)
ChatLayout.displayName = "ChatLayout"
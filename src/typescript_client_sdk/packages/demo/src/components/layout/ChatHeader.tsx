"use client"

import * as React from "react"
import { Menu } from "lucide-react"
import { cn } from "@/lib/utils"
import { 
  Button,
  SessionNameDropdown,
  ConnectionButton, 
  AudioControls,
  ThemeSwitcher
} from "@agentc/realtime-ui"

export interface ChatHeaderProps {
  onMenuToggle?: () => void
  sessionName?: string
  className?: string
}

/**
 * ChatHeader component - Main header for the chat interface
 * Contains menu toggle (mobile), session name, and connection/audio controls
 */
export const ChatHeader = React.forwardRef<HTMLDivElement, ChatHeaderProps>(
  ({ onMenuToggle, sessionName, className }, ref) => {

    return (
      <header
        ref={ref}
        className={cn(
          "sticky top-0 z-40",
          "bg-background/95 backdrop-blur supports-[backdrop-filter]:bg-background/60",
          className
        )}
      >
        <div className="h-14 px-4 flex items-center justify-between gap-2">
          {/* Left side: Menu toggle (mobile) and session name dropdown */}
          <div className="flex items-center gap-2 min-w-0">
            <Button
              variant="ghost"
              size="icon"
              className="lg:hidden shrink-0"
              onClick={onMenuToggle}
              aria-label="Toggle sidebar"
            >
              <Menu className="h-5 w-5" />
            </Button>
            <SessionNameDropdown sessionName={sessionName} />
          </div>

          {/* Center spacer for balanced layout */}
          <div className="flex-1" />

          {/* Right side: Audio controls, theme switcher, and connection status */}
          <div className="flex items-center gap-2 shrink-0">
            <AudioControls size="sm" showLabel={false} />
            <ThemeSwitcher size="icon" />
            <ConnectionButton size="sm" showStatus={true} />
          </div>
        </div>


      </header>
    )
  }
)
ChatHeader.displayName = "ChatHeader"
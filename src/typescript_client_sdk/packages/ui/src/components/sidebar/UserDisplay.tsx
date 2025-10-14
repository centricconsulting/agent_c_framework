"use client"

import * as React from "react"
import { ChevronUp, LogOut, User } from "lucide-react"
import { useUserData } from "@agentc/realtime-react"
import { cn } from "../../lib/utils"
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuTrigger,
} from "../ui/dropdown-menu"

export interface UserDisplayProps {
  isCollapsed?: boolean
  className?: string
  onLogout?: () => void
}

/**
 * UserDisplay component - Shows user info and provides logout functionality
 * 
 * This component gets user data from the WebSocket connection via
 * the useUserData hook from @agentc/realtime-react.
 */
export const UserDisplay = React.forwardRef<HTMLDivElement, UserDisplayProps>(
  ({ isCollapsed, className, onLogout }, ref) => {
    const { user, isLoading: isUserLoading } = useUserData()

    // Get user initials for avatar
    const getInitials = (name?: string) => {
      if (!name) return "U"
      const parts = name.split(" ")
      if (parts.length >= 2) {
        return `${parts[0][0]}${parts[1][0]}`.toUpperCase()
      }
      return name[0].toUpperCase()
    }

    // Display the user_name from WebSocket data
    const getDisplayName = () => {
      if (user?.user_name) {
        return user.user_name;
      }
      // Show loading state while waiting for WebSocket data
      if (isUserLoading) {
        return "Loading...";
      }
      // This should rarely happen - only if WebSocket fails to send user data
      return "Unknown User";
    }

    const displayName = getDisplayName()
    const displayEmail = user?.email || (isUserLoading ? "loading..." : "no-email@example.com")
    const initials = getInitials(displayName)
    
    // When collapsed, show only avatar
    if (isCollapsed) {
      return (
        <div
          ref={ref}
          className={cn(
            "p-4",
            className
          )}
        >
          <DropdownMenu>
            <DropdownMenuTrigger asChild>
              <button
                className={cn(
                  "flex items-center justify-center w-full p-2",
                  "hover:bg-accent rounded-md transition-colors",
                  "focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-ring focus-visible:ring-offset-2"
                )}
                aria-label="User menu"
                title={displayName}
              >
                <div className="h-8 w-8 rounded-full bg-secondary flex items-center justify-center overflow-hidden ring-2 ring-border">
                  <span className="text-sm font-medium text-secondary-foreground">
                    {initials}
                  </span>
                </div>
              </button>
            </DropdownMenuTrigger>
            <DropdownMenuContent 
              className="w-56" 
              align="center" 
              side="right"
              sideOffset={8}
            >
              {onLogout && (
                <DropdownMenuItem 
                  onClick={onLogout}
                  className="cursor-pointer focus:bg-accent focus:text-accent-foreground"
                >
                  <LogOut className="mr-2 h-4 w-4" />
                  <span>Log out</span>
                </DropdownMenuItem>
              )}
            </DropdownMenuContent>
          </DropdownMenu>
        </div>
      )
    }
    
    return (
      <div
        ref={ref}
        className={cn(
          "p-4",
          className
        )}
      >
        <DropdownMenu>
          <DropdownMenuTrigger asChild>
            <button
              className={cn(
                "flex items-center gap-3 w-full p-2",
                "hover:bg-accent rounded-md transition-colors",
                "focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-ring focus-visible:ring-offset-2"
              )}
              aria-label="User menu"
            >
              <div className="flex-shrink-0 h-10 w-10 rounded-full bg-secondary flex items-center justify-center overflow-hidden ring-2 ring-border">
                <span className="text-sm font-medium text-secondary-foreground">
                  {initials}
                </span>
              </div>
              <div className="flex-1 text-left min-w-0">
                <div className="truncate text-sm font-medium leading-tight">
                  {displayName}
                </div>
                <div className="text-xs text-muted-foreground truncate">
                  {displayEmail}
                </div>
              </div>
              <ChevronUp className="h-4 w-4 text-muted-foreground flex-shrink-0" />
            </button>
          </DropdownMenuTrigger>
          <DropdownMenuContent 
            className="w-56" 
            align="start" 
            side="top"
            sideOffset={8}
          >
            {onLogout && (
              <DropdownMenuItem 
                onClick={onLogout}
                className="cursor-pointer focus:bg-accent focus:text-accent-foreground"
              >
                <LogOut className="mr-2 h-4 w-4" />
                <span>Log out</span>
              </DropdownMenuItem>
            )}
          </DropdownMenuContent>
        </DropdownMenu>
      </div>
    )
  }
)
UserDisplay.displayName = "UserDisplay"
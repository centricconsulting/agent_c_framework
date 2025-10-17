"use client"

import * as React from "react"
import { Moon, Sun } from "lucide-react"
import { useTheme } from "next-themes"
import { cn } from "../../lib/utils"
import { Button } from "../ui/button"
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuTrigger,
} from "../ui/dropdown-menu"

export interface ThemeSwitcherProps {
  className?: string
  size?: "default" | "sm" | "lg" | "icon"
  showLabel?: boolean
}

/**
 * ThemeSwitcher component - Toggle between light, dark, and system themes
 * 
 * Features:
 * - Animated icon transitions between sun and moon
 * - Dropdown menu with theme options
 * - Respects system theme preference
 * - Accessible with screen reader support
 */
export const ThemeSwitcher = React.forwardRef<HTMLButtonElement, ThemeSwitcherProps>(
  ({ className, size = "icon", showLabel = false }, ref) => {
    const { theme, setTheme } = useTheme()
    const [mounted, setMounted] = React.useState(false)

    // Prevent hydration mismatch
    React.useEffect(() => {
      setMounted(true)
    }, [])

    // Don't render anything until mounted to avoid hydration issues
    if (!mounted) {
      return (
        <Button
          ref={ref}
          variant="ghost"
          size={size}
          className={cn("relative", className)}
          disabled
          aria-label="Loading theme switcher"
        >
          <div className="h-[1.2rem] w-[1.2rem]" />
          {showLabel && <span className="ml-2">Theme</span>}
        </Button>
      )
    }

    return (
      <DropdownMenu>
        <DropdownMenuTrigger asChild>
          <Button
            ref={ref}
            variant="ghost"
            size={size}
            className={cn(
              "relative",
              // Add hover state that matches other header buttons
              "hover:bg-accent hover:text-accent-foreground",
              className
            )}
            aria-label="Toggle theme"
          >
            {/* Sun icon - visible in light mode */}
            <Sun 
              className={cn(
                "h-[1.2rem] w-[1.2rem] transition-all",
                // Rotation and scale animations for smooth transition
                "rotate-0 scale-100",
                "dark:-rotate-90 dark:scale-0"
              )}
              aria-hidden="true"
            />
            {/* Moon icon - visible in dark mode */}
            <Moon 
              className={cn(
                "absolute h-[1.2rem] w-[1.2rem] transition-all",
                // Start rotated and scaled down
                "rotate-90 scale-0",
                // Animate to normal in dark mode
                "dark:rotate-0 dark:scale-100"
              )}
              aria-hidden="true"
            />
            {/* Screen reader text */}
            <span className="sr-only">
              Toggle theme (current theme: {theme})
            </span>
            {/* Optional visible label */}
            {showLabel && (
              <span className="ml-2">Theme</span>
            )}
          </Button>
        </DropdownMenuTrigger>
        <DropdownMenuContent align="end">
          <DropdownMenuItem 
            onClick={() => setTheme("light")}
            className={cn(
              "cursor-pointer",
              theme === "light" && "bg-accent"
            )}
          >
            <Sun className="mr-2 h-4 w-4" />
            <span>Light</span>
          </DropdownMenuItem>
          <DropdownMenuItem 
            onClick={() => setTheme("dark")}
            className={cn(
              "cursor-pointer",
              theme === "dark" && "bg-accent"
            )}
          >
            <Moon className="mr-2 h-4 w-4" />
            <span>Dark</span>
          </DropdownMenuItem>
          <DropdownMenuItem 
            onClick={() => setTheme("system")}
            className={cn(
              "cursor-pointer",
              theme === "system" && "bg-accent"
            )}
          >
            <Sun className="mr-2 h-4 w-4 dark:hidden" />
            <Moon className="mr-2 h-4 w-4 hidden dark:block" />
            <span>System</span>
          </DropdownMenuItem>
        </DropdownMenuContent>
      </DropdownMenu>
    )
  }
)

ThemeSwitcher.displayName = "ThemeSwitcher"
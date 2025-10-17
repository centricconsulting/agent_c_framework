"use client"

import * as React from "react"
import { Link2, Check } from "lucide-react"
import { cn } from "../../lib/utils"
import { Button } from "../ui/button"
import {
  Tooltip,
  TooltipContent,
  TooltipProvider,
  TooltipTrigger,
} from "../ui/tooltip"
import type { VariantProps } from "class-variance-authority"
import { buttonVariants } from "../ui/button"

export interface CopyLinkButtonProps
  extends Omit<React.ButtonHTMLAttributes<HTMLButtonElement>, 'onClick' | 'size'>,
    Pick<VariantProps<typeof buttonVariants>, 'variant' | 'size'> {
  /**
   * The session ID to include in the URL
   */
  sessionId: string
  /**
   * Optional className for styling
   */
  className?: string
}

/**
 * CopyLinkButton component - Allows users to copy session URLs to clipboard
 * 
 * Features:
 * - Shows Link2 icon by default
 * - Changes to Check icon when copied (with green color)
 * - Shows tooltip with copy status
 * - Automatically reverts after 2 seconds
 * - Handles clipboard API errors gracefully
 * 
 * @example
 * ```tsx
 * <CopyLinkButton 
 *   sessionId="friendly-panda" 
 *   variant="ghost" 
 *   size="icon" 
 * />
 * ```
 */
export const CopyLinkButton = React.forwardRef<HTMLButtonElement, CopyLinkButtonProps>(
  ({ sessionId, variant = 'ghost', size = 'icon', className, ...props }, ref) => {
    const [isCopied, setIsCopied] = React.useState(false)
    const [error, setError] = React.useState<string | null>(null)
    const timeoutRef = React.useRef<ReturnType<typeof setTimeout> | null>(null)

    // Clear timeout on unmount
    React.useEffect(() => {
      return () => {
        if (timeoutRef.current) {
          clearTimeout(timeoutRef.current)
        }
      }
    }, [])

    const handleCopy = React.useCallback(async () => {
      // Prevent multiple rapid clicks
      if (isCopied) return

      try {
        // Construct the full URL
        const url = `${window.location.origin}/chat/${sessionId}`
        
        // Check if clipboard API is available
        if (!navigator.clipboard) {
          // Fallback for older browsers
          const textArea = document.createElement('textarea')
          textArea.value = url
          textArea.style.position = 'fixed'
          textArea.style.left = '-999999px'
          textArea.style.top = '-999999px'
          document.body.appendChild(textArea)
          textArea.focus()
          textArea.select()
          
          try {
            document.execCommand('copy')
            setIsCopied(true)
            setError(null)
          } catch {
            setError('Failed to copy link')
          } finally {
            document.body.removeChild(textArea)
          }
        } else {
          // Modern clipboard API
          await navigator.clipboard.writeText(url)
          setIsCopied(true)
          setError(null)
        }

        // Reset after 2 seconds
        if (timeoutRef.current) {
          clearTimeout(timeoutRef.current)
        }
        timeoutRef.current = setTimeout(() => {
          setIsCopied(false)
          setError(null)
        }, 2000)
      } catch (err) {
        console.error('Failed to copy link:', err)
        setError('Failed to copy link')
        
        // Clear error after 2 seconds
        if (timeoutRef.current) {
          clearTimeout(timeoutRef.current)
        }
        timeoutRef.current = setTimeout(() => {
          setError(null)
        }, 2000)
      }
    }, [sessionId, isCopied])

    // Determine tooltip content
    const tooltipContent = React.useMemo(() => {
      if (error) return error
      if (isCopied) return "Copied!"
      return "Copy link"
    }, [error, isCopied])

    // Determine icon color based on state
    const iconClassName = cn(
      "h-4 w-4 transition-colors",
      isCopied && "text-green-500",
      error && "text-destructive"
    )

    return (
      <TooltipProvider>
        <Tooltip>
          <TooltipTrigger asChild>
            <Button
              ref={ref}
              variant={variant}
              size={size}
              onClick={handleCopy}
              className={cn(
                "transition-all",
                isCopied && "bg-green-500/10 hover:bg-green-500/20",
                className
              )}
              aria-label={isCopied ? "Link copied" : "Copy session link"}
              {...props}
            >
              {isCopied ? (
                <Check className={iconClassName} aria-hidden="true" />
              ) : (
                <Link2 className={iconClassName} aria-hidden="true" />
              )}
              {/* Include text for non-icon sizes */}
              {size !== 'icon' && (
                <span className="ml-2">
                  {isCopied ? 'Copied!' : 'Copy Link'}
                </span>
              )}
            </Button>
          </TooltipTrigger>
          <TooltipContent>
            <p>{tooltipContent}</p>
          </TooltipContent>
        </Tooltip>
      </TooltipProvider>
    )
  }
)

CopyLinkButton.displayName = "CopyLinkButton"
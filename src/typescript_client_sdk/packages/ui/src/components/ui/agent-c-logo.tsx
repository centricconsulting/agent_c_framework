import * as React from "react"
import { cn } from "../../lib/utils"

export interface AgentCLogoProps extends React.SVGProps<SVGSVGElement> {
  className?: string
}

/**
 * Agent C Logo Component
 * Displays the two yellow crescents from the corporate logo
 * Extracted from the full Centric Consulting logo
 */
export const AgentCLogo = React.forwardRef<SVGSVGElement, AgentCLogoProps>(
  ({ className, ...props }, ref) => {
    return (
      <svg
        ref={ref}
        viewBox="0 0 110 101.56"
        fill="none"
        xmlns="http://www.w3.org/2000/svg"
        className={cn("h-6 w-auto", className)}
        aria-label="Agent C Logo"
        role="img"
        {...props}
      >
        {/* First yellow crescent - from official Centric logo */}
        <path
          d="M79.89,0a55.63,55.63,0,0,0,0,101.56A92.58,92.58,0,0,1,79.89,0"
          fill="#fdb924"
        />
        
        {/* Second yellow crescent - from official Centric logo */}
        <path
          d="M32.92,0a55.62,55.62,0,0,0,0,101.56A92.52,92.52,0,0,1,32.92,0"
          fill="#fdb924"
        />
      </svg>
    )
  }
)
AgentCLogo.displayName = "AgentCLogo"

'use client'

import * as React from 'react'
import { cn } from '../../lib/utils'
import { Zap, MessageSquarePlus, MessageSquareOff } from 'lucide-react'

export interface SubsessionDividerProps extends React.HTMLAttributes<HTMLDivElement> {
  /**
   * Type of subsession event
   */
  type: 'start' | 'end'
  /**
   * Optional timestamp for the event
   */
  timestamp?: string
  /**
   * Optional label for the divider
   */
  label?: string
}

/**
 * SubsessionDivider component - Visual indicator for subsession boundaries
 * Following CenSuite design patterns for visual hierarchy and spacing
 */
export const SubsessionDivider = React.forwardRef<HTMLDivElement, SubsessionDividerProps>(
  ({ className, type, timestamp, label, ...props }, ref) => {
    // Format timestamp if provided
    const formattedTime = React.useMemo(() => {
      if (!timestamp) return null
      try {
        const date = new Date(timestamp)
        if (isNaN(date.getTime())) return null
        return date.toLocaleTimeString('en-US', { 
          hour: 'numeric', 
          minute: '2-digit',
          hour12: true 
        })
      } catch {
        return null
      }
    }, [timestamp])
    
    // Icon based on type
    const Icon = type === 'start' ? MessageSquarePlus : MessageSquareOff
    
    // Default label based on type
    const displayLabel = label || (type === 'start' ? 'Subsession started' : 'Subsession ended')
    
    return (
      <div
        ref={ref}
        className={cn(
          "relative flex items-center gap-3 py-4 px-2",
          "animate-in fade-in-50 duration-300",
          className
        )}
        role="separator"
        aria-label={displayLabel}
        {...props}
      >
        {/* Left line */}
        <div className="flex-1 h-px bg-border/50" />
        
        {/* Center content */}
        <div className={cn(
          "flex items-center gap-2 px-3 py-1.5 rounded-full",
          "bg-background border border-border/50",
          "text-xs font-medium text-muted-foreground",
          type === 'start' && "border-primary/30 text-primary/70",
          type === 'end' && "border-muted-foreground/30"
        )}>
          <Icon className="h-3.5 w-3.5" />
          <span>{displayLabel}</span>
          {formattedTime && (
            <>
              <span className="text-muted-foreground/50">•</span>
              <span className="text-muted-foreground/70">{formattedTime}</span>
            </>
          )}
        </div>
        
        {/* Right line */}
        <div className="flex-1 h-px bg-border/50" />
      </div>
    )
  }
)

SubsessionDivider.displayName = 'SubsessionDivider'
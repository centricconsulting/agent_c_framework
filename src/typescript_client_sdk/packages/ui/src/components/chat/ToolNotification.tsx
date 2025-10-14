'use client'

import * as React from 'react'
import { cn } from '../../lib/utils'
import { Wrench, Loader2, Brain, Check } from 'lucide-react'
import { motion, AnimatePresence } from 'framer-motion'

export interface ToolNotificationData {
  id: string
  toolName: string
  status: 'preparing' | 'executing' | 'complete'
  timestamp: Date
  arguments?: string
}

export interface ToolNotificationProps extends React.HTMLAttributes<HTMLDivElement> {
  /**
   * The tool notification data
   */
  notification: ToolNotificationData
  /**
   * Whether this is the "think" tool (special handling)
   */
  isThinkTool?: boolean
}

/**
 * ToolNotification Component
 * Displays progressive status updates for tool calls
 * Special handling for the "think" tool
 */
const ToolNotification = React.forwardRef<HTMLDivElement, ToolNotificationProps>(
  ({ className, notification, isThinkTool, ...props }, ref) => {
    const { toolName, status } = notification
    
    // Determine if this is actually a think tool
    const isThink = isThinkTool || toolName === 'think'
    
    // Determine icon based on tool type and status
    const Icon = React.useMemo(() => {
      if (isThink) {
        return Brain
      }
      
      switch (status) {
        case 'preparing':
        case 'executing':
          return Loader2
        case 'complete':
          return Check
        default:
          return Wrench
      }
    }, [isThink, status])
    
    // Determine status text
    const statusText = React.useMemo(() => {
      // Special handling for "think" tool
      if (isThink) {
        switch (status) {
          case 'preparing':
          case 'executing':
            return 'Agent is thinking...'
          case 'complete':
            return 'Thought process complete'
          default:
            return 'Processing...'
        }
      }
      
      // Regular tool handling
      switch (status) {
        case 'preparing':
          return `Agent is preparing to use ${toolName}`
        case 'executing':
          return `Agent is using ${toolName}`
        case 'complete':
          return `Completed ${toolName}`
        default:
          return `Processing ${toolName}...`
      }
    }, [isThink, toolName, status])
    
    // Determine visual styling based on status
    const statusStyles = React.useMemo(() => {
      switch (status) {
        case 'preparing':
          return 'bg-muted/50 border-border/50 text-muted-foreground'
        case 'executing':
          return 'bg-primary/5 border-primary/20 text-foreground'
        case 'complete':
          return 'bg-green-500/5 border-green-500/20 text-green-700 dark:text-green-400'
        default:
          return 'bg-muted/30 border-border/30 text-muted-foreground'
      }
    }, [status])
    
    // Animation states for icon
    const isAnimating = status === 'preparing' || status === 'executing'
    
    return (
      <AnimatePresence mode="wait">
        <motion.div
          ref={ref}
          initial={{ opacity: 0 }}
          animate={{ opacity: 1 }}
          exit={{ opacity: 0 }}
          transition={{ duration: 0.2, ease: 'easeOut' }}
          className={cn(
            'relative overflow-hidden',
            className
          )}
        >
          <div
            className={cn(
              'flex items-center gap-3 px-4 py-2.5 rounded-lg border',
              'transition-all duration-200',
              statusStyles
            )}
            role="status"
            aria-live="polite"
            aria-label={statusText}
          >
            {/* Icon with animation */}
            <div className="flex-shrink-0">
              <Icon 
                className={cn(
                  'h-4 w-4',
                  isAnimating && !isThink && 'animate-spin',
                  isThink && isAnimating && 'animate-pulse'
                )}
              />
            </div>
            
            {/* Status text */}
            <span className="text-sm font-medium leading-tight">
              {statusText}
            </span>
            
            {/* Optional progress indicator for executing state */}
            {status === 'executing' && (
              <div className="ml-auto flex items-center gap-1">
                <span className="inline-block w-1.5 h-1.5 bg-current rounded-full animate-pulse" />
                <span className="inline-block w-1.5 h-1.5 bg-current rounded-full animate-pulse [animation-delay:150ms]" />
                <span className="inline-block w-1.5 h-1.5 bg-current rounded-full animate-pulse [animation-delay:300ms]" />
              </div>
            )}
          </div>
        </motion.div>
      </AnimatePresence>
    )
  }
)

ToolNotification.displayName = 'ToolNotification'

/**
 * ToolNotificationList Component
 * Manages and displays multiple tool notifications
 */
export interface ToolNotificationListProps extends React.HTMLAttributes<HTMLDivElement> {
  /**
   * Array of tool notifications to display
   */
  notifications: ToolNotificationData[]
  /**
   * Maximum number of notifications to show
   */
  maxNotifications?: number
}

const ToolNotificationList = React.forwardRef<HTMLDivElement, ToolNotificationListProps>(
  ({ className, notifications, maxNotifications = 3, ...props }, ref) => {
    
    // ADD EVE'S LOGGING:
    console.log('[ToolNotificationList] RENDERING with notifications:', notifications);
    console.log('[ToolNotificationList] notifications.length:', notifications.length);
    
    // Only show the most recent notifications
    const visibleNotifications = React.useMemo(() => {
      if (maxNotifications && notifications.length > maxNotifications) {
        return notifications.slice(-maxNotifications)
      }
      return notifications
    }, [notifications, maxNotifications])
    
    if (visibleNotifications.length === 0) {
      return null
    }
    
    return (
      <div
        ref={ref}
        className={cn('space-y-2', className)}
        {...props}
      >
        <AnimatePresence mode="popLayout">
          {visibleNotifications.map((notification) => (
            <ToolNotification
              key={notification.id}
              notification={notification}
              isThinkTool={notification.toolName === 'think'}
            />
          ))}
        </AnimatePresence>
      </div>
    )
  }
)

ToolNotificationList.displayName = 'ToolNotificationList'

export { ToolNotification, ToolNotificationList }
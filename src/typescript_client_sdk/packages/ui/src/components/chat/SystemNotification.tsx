'use client'

import * as React from 'react'
import { cn } from '../../lib/utils'
import { 
  AlertTriangle, Info, XCircle, X, 
  AlertCircle, CheckCircle 
} from 'lucide-react'
import { motion, AnimatePresence } from 'framer-motion'
import { Alert, AlertTitle, AlertDescription } from '../ui/alert'

export type NotificationSeverity = 'info' | 'warning' | 'error' | 'success'

export interface SystemNotificationData {
  id: string
  severity: NotificationSeverity
  title?: string
  content: string
  timestamp?: string | Date
  dismissible?: boolean
}

export interface SystemNotificationProps extends React.HTMLAttributes<HTMLDivElement> {
  /**
   * The notification data
   */
  notification: SystemNotificationData
  /**
   * Callback when notification is dismissed
   */
  onDismiss?: (id: string) => void
  /**
   * Auto-dismiss after duration in milliseconds
   */
  autoDismiss?: number
}

const severityConfig = {
  error: {
    variant: 'destructive' as const,
    icon: XCircle,
    defaultTitle: 'Error',
    className: 'border-destructive/50 text-destructive'
  },
  warning: {
    variant: 'default' as const,
    icon: AlertTriangle,
    defaultTitle: 'Warning',
    className: 'border-yellow-500/50 text-yellow-700 dark:text-yellow-400'
  },
  info: {
    variant: 'default' as const,
    icon: Info,
    defaultTitle: 'Info',
    className: 'border-blue-500/50 text-blue-700 dark:text-blue-400'
  },
  success: {
    variant: 'default' as const,
    icon: CheckCircle,
    defaultTitle: 'Success',
    className: 'border-green-500/50 text-green-700 dark:text-green-400'
  }
}

export const SystemNotification = React.forwardRef<HTMLDivElement, SystemNotificationProps>(
  ({ 
    className, 
    notification, 
    onDismiss, 
    autoDismiss,
    ...props 
  }, ref) => {
    const config = severityConfig[notification.severity] || severityConfig.info
    const Icon = config.icon
    const title = notification.title || config.defaultTitle
    
    React.useEffect(() => {
      if (autoDismiss && autoDismiss > 0) {
        const timer = setTimeout(() => {
          onDismiss?.(notification.id)
        }, autoDismiss)
        
        return () => clearTimeout(timer)
      }
    }, [autoDismiss, notification.id, onDismiss])
    
    return (
      <motion.div
        ref={ref}
        initial={{ opacity: 0, x: 20, scale: 0.95 }}
        animate={{ opacity: 1, x: 0, scale: 1 }}
        exit={{ opacity: 0, x: 20, scale: 0.95 }}
        transition={{ duration: 0.2, ease: 'easeOut' }}
        className={className}
      >
        <Alert 
          variant={config.variant} 
          className={cn(
            "w-80 shadow-lg",
            config.className
          )}
        >
          <Icon className="h-4 w-4" />
          <AlertTitle className="pr-6">{title}</AlertTitle>
          <AlertDescription className="pr-6">
            {notification.content}
          </AlertDescription>
          {(notification.dismissible !== false) && onDismiss && (
            <button
              className={cn(
                "absolute top-2 right-2 p-1 rounded-md",
                "hover:bg-muted transition-colors",
                "focus-visible:outline-none focus-visible:ring-2",
                "focus-visible:ring-ring focus-visible:ring-offset-2"
              )}
              onClick={() => onDismiss(notification.id)}
              aria-label="Dismiss notification"
            >
              <X className="h-3 w-3" />
            </button>
          )}
        </Alert>
      </motion.div>
    )
  }
)

SystemNotification.displayName = 'SystemNotification'

export interface SystemNotificationContainerProps extends React.HTMLAttributes<HTMLDivElement> {
  /**
   * Array of notifications to display
   */
  notifications: SystemNotificationData[]
  /**
   * Maximum number of notifications to show
   */
  maxNotifications?: number
  /**
   * Position of the container
   */
  position?: 'top-left' | 'top-right' | 'bottom-left' | 'bottom-right'
  /**
   * Auto-dismiss duration for all notifications
   */
  autoDismiss?: number
  /**
   * Callback when a notification is dismissed
   */
  onDismiss?: (id: string) => void
}

export const SystemNotificationContainer = React.forwardRef<
  HTMLDivElement, 
  SystemNotificationContainerProps
>(
  ({ 
    className, 
    notifications, 
    maxNotifications = 3,
    position = 'bottom-right',
    autoDismiss = 5000,
    onDismiss,
    ...props 
  }, ref) => {
    const [dismissedIds, setDismissedIds] = React.useState<Set<string>>(new Set())
    
    const handleDismiss = React.useCallback((id: string) => {
      setDismissedIds(prev => new Set(prev).add(id))
      onDismiss?.(id)
    }, [onDismiss])
    
    const visibleNotifications = React.useMemo(() => {
      return notifications
        .filter(n => !dismissedIds.has(n.id))
        .slice(-maxNotifications)
    }, [notifications, dismissedIds, maxNotifications])
    
    const positionClasses = {
      'top-left': 'top-4 left-4',
      'top-right': 'top-4 right-4',
      'bottom-left': 'bottom-4 left-4',
      'bottom-right': 'bottom-4 right-4'
    }
    
    return (
      <div
        ref={ref}
        className={cn(
          "fixed z-50 flex flex-col gap-2",
          positionClasses[position],
          className
        )}
        role="region"
        aria-label="System notifications"
        aria-live="polite"
        aria-atomic="false"
        {...props}
      >
        <AnimatePresence mode="popLayout">
          {visibleNotifications.map(notification => (
            <SystemNotification
              key={notification.id}
              notification={notification}
              onDismiss={handleDismiss}
              autoDismiss={autoDismiss}
            />
          ))}
        </AnimatePresence>
      </div>
    )
  }
)

SystemNotificationContainer.displayName = 'SystemNotificationContainer'
"use client"

import * as React from "react"
import { cva, type VariantProps } from "class-variance-authority"
import { cn } from "../../lib/utils"
import { useConnection } from "@agentc/realtime-react"
import { ConnectionState } from "@agentc/realtime-core"
import { Loader2 } from "lucide-react"

const connectionButtonVariants = cva(
  "inline-flex items-center justify-center rounded-md text-sm font-medium ring-offset-background transition-colors focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-ring focus-visible:ring-offset-2 disabled:pointer-events-none disabled:opacity-50",
  {
    variants: {
      variant: {
        default: "bg-primary text-primary-foreground hover:bg-primary/90",
        outline: "border border-input bg-background hover:bg-accent hover:text-accent-foreground",
        secondary: "bg-secondary text-secondary-foreground hover:bg-secondary/80",
        ghost: "hover:bg-accent hover:text-accent-foreground",
        destructive: "bg-destructive text-destructive-foreground hover:bg-destructive/90",
      },
      size: {
        default: "h-10 px-4 py-2",
        sm: "h-9 rounded-md px-3",
        lg: "h-11 rounded-md px-8",
        icon: "h-10 w-10",
      },
      state: {
        idle: "",
        loading: "opacity-70 cursor-wait",
        error: "border-destructive",
      }
    },
    defaultVariants: {
      variant: "default",
      size: "default",
      state: "idle",
    },
  }
)

export interface ConnectionButtonProps
  extends React.ButtonHTMLAttributes<HTMLButtonElement>,
    VariantProps<typeof connectionButtonVariants> {
  showStatus?: boolean
  statusPosition?: 'left' | 'right'
}

/**
 * ConnectionButton component - Manages WebSocket connection state
 * Provides visual feedback for connection status and handles connect/disconnect
 */
const ConnectionButton = React.forwardRef<HTMLButtonElement, ConnectionButtonProps>(
  ({ className, variant, size, showStatus = true, statusPosition = 'left', ...props }, ref) => {
    const { isConnected, connectionState, connect, disconnect, error } = useConnection()
    
    // Determine variant based on connection state
    const currentVariant = React.useMemo(() => {
      if (error) return 'destructive'
      if (isConnected) return 'secondary'
      return variant || 'outline'
    }, [error, isConnected, variant])
    
    // Handle click
    const handleClick = React.useCallback(async () => {
      try {
        if (isConnected) {
          disconnect()
        } else {
          await connect()
        }
      } catch (error) {
        console.error('Connection error:', error)
      }
    }, [isConnected, connect, disconnect])
    
    return (
      <button
        ref={ref}
        className={cn(
          connectionButtonVariants({ 
            variant: currentVariant, 
            size,
            state: connectionState === ConnectionState.CONNECTING ? 'loading' : 'idle',
            className 
          })
        )}
        onClick={handleClick}
        disabled={connectionState === ConnectionState.CONNECTING}
        aria-label={isConnected ? '' : 'Connect'}
        {...props}
      >
        {showStatus && statusPosition === 'left' && (
          <StatusIndicator state={connectionState} className="mr-2" />
        )}
        {connectionState === ConnectionState.CONNECTING ? (
          <>
            <Loader2 className="h-4 w-4 animate-spin mr-2" />
            <span>Connecting...</span>
          </>
        ) : (
          <span>{isConnected ? '' : 'Connect'}</span>
        )}
        {showStatus && statusPosition === 'right' && (
          <StatusIndicator state={connectionState} className="ml-2" />
        )}
      </button>
    )
  }
)
ConnectionButton.displayName = "ConnectionButton"

/**
 * StatusIndicator - Visual indicator for connection state
 */
const StatusIndicator: React.FC<{ state: ConnectionState; className?: string }> = ({ state, className }) => {
  const statusColors = {
    [ConnectionState.CONNECTED]: 'bg-green-500',
    [ConnectionState.CONNECTING]: 'bg-yellow-500 animate-pulse',
    [ConnectionState.DISCONNECTED]: 'bg-gray-400',
    [ConnectionState.RECONNECTING]: 'bg-yellow-500 animate-pulse'
  }
  
  return (
    <span 
      className={cn(
        "inline-block h-2 w-2 rounded-full",
        statusColors[state] || statusColors[ConnectionState.DISCONNECTED],
        className
      )}
      aria-hidden="true"
    />
  )
}

export { ConnectionButton, connectionButtonVariants }
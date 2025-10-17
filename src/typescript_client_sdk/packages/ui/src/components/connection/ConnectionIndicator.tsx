'use client'

import * as React from 'react'
import { cn } from '../../lib/utils'
import { useConnection, getConnectionStateString } from '@agentc/realtime-react'
import {
  Tooltip,
  TooltipContent,
  TooltipProvider,
  TooltipTrigger,
} from '../ui/tooltip'

export interface ConnectionIndicatorProps extends React.HTMLAttributes<HTMLDivElement> {
  /**
   * Show text label
   */
  showLabel?: boolean
  /**
   * Show tooltip with details
   */
  showTooltip?: boolean
  /**
   * Show connection stats
   */
  showStats?: boolean
  /**
   * Size variant
   */
  size?: 'small' | 'default' | 'large'
}

export const ConnectionIndicator = React.forwardRef<HTMLDivElement, ConnectionIndicatorProps>(
  ({ 
    className,
    showLabel = false,
    showTooltip = false,
    showStats = false,
    size = 'default',
    ...props 
  }, ref) => {
    const { isConnected, connectionState, error, stats } = useConnection()
    
    const connectionStateString = getConnectionStateString(connectionState)
    
    const indicatorSize = {
      small: 'h-2 w-2',
      default: 'h-3 w-3',
      large: 'h-4 w-4'
    }[size]
    
    const indicatorColor = React.useMemo(() => {
      switch (connectionStateString) {
        case 'connected':
          return 'bg-green-500'
        case 'connecting':
        case 'reconnecting':
          return 'bg-yellow-500 animate-pulse'
        default:
          return 'bg-gray-400'
      }
    }, [connectionStateString])
    
    const statusText = React.useMemo(() => {
      switch (connectionStateString) {
        case 'connected':
          return 'Connected'
        case 'connecting':
          return 'Connecting'
        case 'reconnecting':
          return 'Reconnecting'
        default:
          return 'Disconnected'
      }
    }, [connectionStateString])
    
    const indicator = (
      <div
        ref={ref}
        role="status"
        aria-label={`Connection status: ${statusText}`}
        aria-live="polite"
        className={cn(
          'inline-flex items-center gap-2',
          className
        )}
        {...props}
      >
        <span 
          className={cn(
            'rounded-full transition-all duration-200',
            indicatorSize,
            indicatorColor
          )}
        />
        {showLabel && (
          <span className="text-sm text-muted-foreground">
            {statusText}
          </span>
        )}
      </div>
    )
    
    if (!showTooltip) {
      return indicator
    }
    
    return (
      <TooltipProvider>
        <Tooltip>
          <TooltipTrigger asChild>
            {indicator}
          </TooltipTrigger>
          <TooltipContent>
            <div className="space-y-1 text-xs">
              <p className="font-semibold">{statusText}</p>
              {error && (
                <p className="text-destructive">Error: {error.message}</p>
              )}
              {showStats && stats && isConnected && (
                <>
                  <p>Latency: {stats.latency}ms</p>
                  <p>Messages: {stats.messagesReceived} received, {stats.messagesSent} sent</p>
                  {stats.bytesReceived && (
                    <p>Data: {Math.round(stats.bytesReceived / 1024)}KB received</p>
                  )}
                </>
              )}
            </div>
          </TooltipContent>
        </Tooltip>
      </TooltipProvider>
    )
  }
)

ConnectionIndicator.displayName = 'ConnectionIndicator'
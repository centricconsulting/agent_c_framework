'use client'

import React from 'react';
import { useConnection } from '@agentc/realtime-react';
import { Button } from '../ui/button';
import { Card } from '../ui/card';
import { Badge } from '../ui/badge';
import { cn } from '../../lib/utils';
import { RefreshCw, Wifi, WifiOff } from 'lucide-react';

/**
 * Format duration from milliseconds to human-readable string
 */
function formatDuration(ms: number): string {
  const seconds = Math.floor(ms / 1000);
  const minutes = Math.floor(seconds / 60);
  const hours = Math.floor(minutes / 60);
  
  if (hours > 0) {
    const remainingMinutes = minutes % 60;
    return `${hours}h ${remainingMinutes}m`;
  }
  
  if (minutes > 0) {
    const remainingSeconds = seconds % 60;
    return `${minutes}m ${remainingSeconds}s`;
  }
  
  return `${seconds}s`;
}

/**
 * Connection status component props
 */
export interface ConnectionStatusProps {
  className?: string;
  showDetails?: boolean;
  compact?: boolean;
}

/**
 * Connection status component
 * Shows WebSocket connection state with visual indicators
 */
export function ConnectionStatus({ 
  className, 
  showDetails = true,
  compact = false 
}: ConnectionStatusProps) {
  const { 
    isConnected, 
    connectionState, 
    error: connectionError,
    reconnectAttempt,
    stats,
    connect,
    disconnect
  } = useConnection();

  // Determine status color and icon
  const getStatusColor = () => {
    switch (connectionState) {
      case 'CONNECTED':
        return 'bg-green-500';
      case 'CONNECTING':
        return 'bg-yellow-500 animate-pulse';
      case 'RECONNECTING':
        return 'bg-orange-500 animate-pulse';
      case 'DISCONNECTED':
        return connectionError ? 'bg-red-500' : 'bg-gray-500';
      default:
        return 'bg-gray-400';
    }
  };

  const getStatusText = () => {
    switch (connectionState) {
      case 'CONNECTED':
        return 'Connected';
      case 'CONNECTING':
        return 'Connecting...';
      case 'RECONNECTING':
        return `Reconnecting (${reconnectAttempt})...`;
      case 'DISCONNECTED':
        return connectionError ? 'Connection Error' : 'Disconnected';
      default:
        return 'Unknown';
    }
  };

  const getStatusIcon = () => {
    if (isConnected) {
      return <Wifi className="w-4 h-4" />;
    }
    return <WifiOff className="w-4 h-4" />;
  };

  // Compact mode - just a badge
  if (compact) {
    return (
      <Badge
        variant={isConnected ? 'default' : 'secondary'}
        className={cn('gap-1.5', className)}
      >
        <span
          className={cn(
            'w-2 h-2 rounded-full',
            getStatusColor()
          )}
        />
        {getStatusText()}
      </Badge>
    );
  }

  // Full card mode
  return (
    <Card className={cn('p-4', className)}>
      <div className="flex items-center justify-between">
        <div className="flex items-center gap-3">
          {/* Status indicator */}
          <div className="relative">
            <span
              className={cn(
                'absolute inset-0 rounded-full',
                getStatusColor(),
                isConnected && 'animate-ping opacity-75'
              )}
            />
            <span
              className={cn(
                'relative flex w-3 h-3 rounded-full',
                getStatusColor()
              )}
            />
          </div>

          {/* Status text */}
          <div className="flex flex-col">
            <div className="flex items-center gap-2">
              {getStatusIcon()}
              <span className="font-medium">{getStatusText()}</span>
            </div>
            
            {/* Connection error */}
            {connectionError && (
              <p className="text-sm text-destructive mt-1">
                {connectionError.message}
              </p>
            )}

            {/* Session duration */}
            {showDetails && isConnected && stats.sessionDuration > 0 && (
              <p className="text-sm text-muted-foreground mt-1">
                Session: {formatDuration(stats.sessionDuration)}
              </p>
            )}
          </div>
        </div>

        {/* Retry button on error */}
        {(connectionError || connectionState === 'DISCONNECTED') ? (
          <Button
            size="sm"
            variant="outline"
            onClick={() => connect()}
            className="gap-2"
          >
            <RefreshCw className="w-3 h-3" />
            Retry
          </Button>
        ) : connectionState === 'CONNECTED' ? (
          <Button
            size="sm"
            variant="ghost"
            onClick={() => disconnect()}
            className="text-muted-foreground"
          >
            Disconnect
          </Button>
        ) : null}
      </div>

      {/* Connection statistics */}
      {showDetails && stats.connectionAttempts > 0 && (
        <div className="mt-4 pt-4 border-t grid grid-cols-3 gap-4 text-sm">
          <div>
            <p className="text-muted-foreground">Attempts</p>
            <p className="font-medium">{stats.connectionAttempts}</p>
          </div>
          <div>
            <p className="text-muted-foreground">Successful</p>
            <p className="font-medium">{stats.successfulConnections}</p>
          </div>
          <div>
            <p className="text-muted-foreground">Failed</p>
            <p className="font-medium">{stats.failedConnections}</p>
          </div>
        </div>
      )}
    </Card>
  );
}

/**
 * Simple connection indicator for use in headers/navbars
 */
export function ConnectionIndicator({ className }: { className?: string }) {
  const { isConnected, connectionState } = useConnection();

  const getStatusColor = () => {
    switch (connectionState) {
      case 'CONNECTED':
        return 'bg-green-500';
      case 'CONNECTING':
      case 'RECONNECTING':
        return 'bg-yellow-500';
      case 'DISCONNECTED':
        return 'bg-red-500';
      default:
        return 'bg-gray-400';
    }
  };

  return (
    <div className={cn('flex items-center gap-2', className)}>
      <span
        className={cn(
          'w-2 h-2 rounded-full',
          getStatusColor(),
          (connectionState === 'CONNECTING' || connectionState === 'RECONNECTING') && 'animate-pulse'
        )}
      />
      <span className="text-sm text-muted-foreground">
        {isConnected ? 'Online' : 'Offline'}
      </span>
    </div>
  );
}
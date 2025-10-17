/**
 * useConnection - React hook for connection management
 * Provides interface for monitoring and controlling WebSocket connection
 */

import { useEffect, useState, useCallback } from 'react';
import { ConnectionState } from '@agentc/realtime-core';
import { useRealtimeClientSafe, useAgentCContext } from '../providers/AgentCContext';

/**
 * String literal type for connection state (lowercase version)
 */
export type ConnectionStateString = 'connected' | 'disconnected' | 'connecting' | 'reconnecting' | 'error';

/**
 * Helper to convert ConnectionState enum to lowercase string literal
 */
export function getConnectionStateString(state: ConnectionState): ConnectionStateString {
  switch(state) {
    case ConnectionState.CONNECTED:
      return 'connected';
    case ConnectionState.DISCONNECTED:
      return 'disconnected';
    case ConnectionState.CONNECTING:
      return 'connecting';
    case ConnectionState.RECONNECTING:
      return 'reconnecting';
    default:
      return 'disconnected';
  }
}

/**
 * Connection statistics
 */
export interface ConnectionStats {
  /** Number of connection attempts */
  connectionAttempts: number;
  
  /** Number of successful connections */
  successfulConnections: number;
  
  /** Number of failed connections */
  failedConnections: number;
  
  /** Last connection time */
  lastConnectedAt: Date | null;
  
  /** Last disconnection time */
  lastDisconnectedAt: Date | null;
  
  /** Current session duration in milliseconds */
  sessionDuration: number;
  
  /** Current latency in milliseconds */
  latency: number;
  
  /** Total messages received */
  messagesReceived: number;
  
  /** Total messages sent */
  messagesSent: number;
  
  /** Total bytes received */
  bytesReceived: number;
  
  /** Total bytes sent */
  bytesSent: number;
}

/**
 * Return type for the useConnection hook
 */
export interface UseConnectionReturn {
  /** Current connection state */
  connectionState: ConnectionState;
  
  /** Whether currently connected */
  isConnected: boolean;
  
  /** Whether currently connecting */
  isConnecting: boolean;
  
  /** Whether connection is closed */
  isDisconnected: boolean;
  
  /** Whether client is initializing */
  isInitializing: boolean;
  
  /** Connect to the server */
  connect: () => Promise<void>;
  
  /** Disconnect from the server */
  disconnect: () => void;
  
  /** Reconnect to the server */
  reconnect: () => Promise<void>;
  
  /** Connection error if any */
  error: Error | null;
  
  /** Connection statistics */
  stats: ConnectionStats;
  
  /** Whether auto-reconnect is enabled */
  autoReconnectEnabled: boolean;
  
  /** Current reconnect attempt number */
  reconnectAttempt: number;
  
  /** Maximum reconnect attempts */
  maxReconnectAttempts: number;
}

/**
 * React hook for connection management
 * Provides interface for monitoring and controlling WebSocket connection
 */
export function useConnection(): UseConnectionReturn {
  const client = useRealtimeClientSafe();
  const { isInitializing, error: contextError } = useAgentCContext();
  
  // State
  const [connectionState, setConnectionState] = useState<ConnectionState>(ConnectionState.DISCONNECTED);
  const [error, setError] = useState<Error | null>(contextError);
  const [reconnectAttempt, setReconnectAttempt] = useState(0);
  const [stats, setStats] = useState<ConnectionStats>({
    connectionAttempts: 0,
    successfulConnections: 0,
    failedConnections: 0,
    lastConnectedAt: null,
    lastDisconnectedAt: null,
    sessionDuration: 0,
    latency: 0,
    messagesReceived: 0,
    messagesSent: 0,
    bytesReceived: 0,
    bytesSent: 0
  });
  
  // Session duration tracking
  const [sessionStartTime, setSessionStartTime] = useState<number | null>(null);
  
  // Update connection state from client
  const updateConnectionState = useCallback(() => {
    if (!client) {
      setConnectionState(ConnectionState.DISCONNECTED);
      return;
    }
    
    const state = client.getConnectionState();
    setConnectionState(state);
  }, [client]);
  
  // Connect
  const connect = useCallback(async (): Promise<void> => {
    if (!client) {
      throw new Error('Client not available');
    }
    
    if (client.isConnected()) {
      console.warn('Already connected');
      return;
    }
    
    setError(null);
    setStats(prev => ({
      ...prev,
      connectionAttempts: prev.connectionAttempts + 1
    }));
    
    try {
      await client.connect();
      
      setStats(prev => ({
        ...prev,
        successfulConnections: prev.successfulConnections + 1,
        lastConnectedAt: new Date()
      }));
      
      setSessionStartTime(Date.now());
      setReconnectAttempt(0);
    } catch (err) {
      const connectionError = err instanceof Error ? err : new Error('Connection failed');
      setError(connectionError);
      
      setStats(prev => ({
        ...prev,
        failedConnections: prev.failedConnections + 1
      }));
      
      throw connectionError;
    }
  }, [client]);
  
  // Disconnect
  const disconnect = useCallback(() => {
    if (!client) {
      console.warn('Client not available');
      return;
    }
    
    client.disconnect();
    
    setStats(prev => ({
      ...prev,
      lastDisconnectedAt: new Date()
    }));
    
    setSessionStartTime(null);
  }, [client]);
  
  // Reconnect
  const reconnect = useCallback(async (): Promise<void> => {
    if (!client) {
      throw new Error('Client not available');
    }
    
    // Disconnect first if connected
    if (client.isConnected()) {
      client.disconnect();
    }
    
    // Wait a moment before reconnecting
    await new Promise(resolve => setTimeout(resolve, 100));
    
    // Connect again
    await connect();
  }, [client, connect]);
  
  // Subscribe to connection events
  useEffect(() => {
    if (!client) return;
    
    // Initial state update
    updateConnectionState();
    
    // Handle connection events
    const handleConnected = () => {
      setConnectionState(ConnectionState.CONNECTED);
      setError(null);
      setSessionStartTime(Date.now());
      
      setStats(prev => ({
        ...prev,
        lastConnectedAt: new Date()
      }));
    };
    
    const handleDisconnected = ({ code, reason }: { code: number; reason: string }) => {
      setConnectionState(ConnectionState.DISCONNECTED);
      setSessionStartTime(null);
      
      setStats(prev => ({
        ...prev,
        lastDisconnectedAt: new Date()
      }));
      
      // Set error if it was an unexpected disconnection
      if (code !== 1000) { // 1000 is normal closure
        setError(new Error(`Disconnected: ${reason} (code: ${code})`));
      }
    };
    
    const handleReconnecting = ({ attempt }: { attempt: number }) => {
      setConnectionState(ConnectionState.RECONNECTING);
      setReconnectAttempt(attempt);
    };
    
    const handleError = (error: unknown) => {
      const err = error instanceof Error ? error : new Error(
        (error as { message?: string })?.message || 'Unknown error'
      );
      setError(err);
      
      setStats(prev => ({
        ...prev,
        failedConnections: prev.failedConnections + 1
      }));
    };
    
    // TODO: Implement message statistics tracking when core events are available
    // These handlers will track:
    // - messagesSent/bytesSent via 'message:sent' event
    // - messagesReceived/bytesReceived via 'message:received' event
    // - latency via 'latency:update' event (from ping/pong measurements)
    //
    // const handleMessageSent = (data?: { size?: number }) => {
    //   setStats(prev => ({
    //     ...prev,
    //     messagesSent: prev.messagesSent + 1,
    //     bytesSent: prev.bytesSent + (data?.size || 0)
    //   }));
    // };
    //
    // const handleMessageReceived = (data?: { size?: number }) => {
    //   setStats(prev => ({
    //     ...prev,
    //     messagesReceived: prev.messagesReceived + 1,
    //     bytesReceived: prev.bytesReceived + (data?.size || 0)
    //   }));
    // };
    //
    // const handleLatencyUpdate = (latency: number) => {
    //   setStats(prev => ({
    //     ...prev,
    //     latency
    //   }));
    // };
    
    // Subscribe to events
    client.on('connected', handleConnected);
    client.on('disconnected', handleDisconnected);
    client.on('reconnecting', handleReconnecting);
    client.on('error', handleError);
    
    // TODO: Subscribe to message tracking events when implemented in core
    // These events need to be added to RealtimeEventMap in the core package:
    // - 'message:sent': Track outgoing messages and bytes
    // - 'message:received': Track incoming messages and bytes  
    // - 'latency:update': Track connection latency
    // 
    // For now, these stats will remain at 0 until the core events are implemented
    // if (client.on) {
    //   client.on('message:sent', handleMessageSent);
    //   client.on('message:received', handleMessageReceived);
    //   client.on('latency:update', handleLatencyUpdate);
    // }
    
    return () => {
      client.off('connected', handleConnected);
      client.off('disconnected', handleDisconnected);
      client.off('reconnecting', handleReconnecting);
      client.off('error', handleError);
      
      // TODO: Unsubscribe when events are implemented
      // if (client.off) {
      //   client.off('message:sent', handleMessageSent);
      //   client.off('message:received', handleMessageReceived);
      //   client.off('latency:update', handleLatencyUpdate);
      // }
    };
  }, [client, updateConnectionState]);
  
  // Update session duration
  useEffect(() => {
    if (!sessionStartTime) {
      return;
    }
    
    const interval = setInterval(() => {
      setStats(prev => ({
        ...prev,
        sessionDuration: Date.now() - sessionStartTime
      }));
    }, 1000);
    
    return () => clearInterval(interval);
  }, [sessionStartTime]);
  
  // Get configuration values - client doesn't expose getConfig, use defaults
  const autoReconnectEnabled = true; // Default from mergeConfig
  const maxReconnectAttempts = 5; // Default from ReconnectionConfig
  
  // Computed states
  const isConnected = connectionState === ConnectionState.CONNECTED;
  const isConnecting = connectionState === ConnectionState.CONNECTING;
  const isDisconnected = connectionState === ConnectionState.DISCONNECTED;
  
  return {
    connectionState,
    isConnected,
    isConnecting,
    isDisconnected,
    isInitializing,
    connect,
    disconnect,
    reconnect,
    error,
    stats,
    autoReconnectEnabled,
    reconnectAttempt,
    maxReconnectAttempts
  };
}
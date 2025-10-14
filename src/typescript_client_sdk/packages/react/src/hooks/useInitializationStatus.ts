/**
 * useInitializationStatus - React hook for tracking WebSocket initialization status
 * 
 * This hook provides a simple way to track whether the WebSocket connection
 * has completed its initialization sequence (all 6 initialization events received).
 */

import { useEffect, useState, useCallback } from 'react';
import { useRealtimeClientSafe } from '../providers/AgentCContext';
import { ConnectionState } from '@agentc/realtime-core';

/**
 * Return type for the useInitializationStatus hook
 */
export interface UseInitializationStatusReturn {
  /** Whether the client has completed initialization (all 6 events received) */
  isInitialized: boolean;
  
  /** Whether the client is currently loading/connecting/initializing */
  isLoading: boolean;
  
  /** Error message if initialization failed */
  error: string | null;
  
  /** Current connection state */
  connectionState: ConnectionState;
  
  /** Wait for initialization to complete (returns a promise) */
  waitForInitialization: () => Promise<void>;
}

/**
 * React hook for tracking WebSocket initialization status
 * 
 * This hook provides a simple interface for UI components to know when
 * the WebSocket connection is ready with all configuration data loaded.
 * 
 * The initialization sequence includes:
 * 1. chat_user_data - User profile
 * 2. avatar_list - Available avatars
 * 3. voice_list - Available voices
 * 4. agent_list - Available agents
 * 5. tool_catalog - Available tools
 * 6. chat_session_changed - Current session
 * 
 * @example
 * ```tsx
 * function App() {
 *   const { isInitialized, isLoading, error } = useInitializationStatus();
 *   
 *   if (error) {
 *     return <ErrorDisplay message={error} />;
 *   }
 *   
 *   if (isLoading) {
 *     return <LoadingSpinner message="Initializing..." />;
 *   }
 *   
 *   if (!isInitialized) {
 *     return <div>Waiting for server configuration...</div>;
 *   }
 *   
 *   return <MainApplication />;
 * }
 * ```
 * 
 * @example
 * ```tsx
 * // Using with async wait
 * function DataComponent() {
 *   const { waitForInitialization } = useInitializationStatus();
 *   
 *   const loadData = async () => {
 *     await waitForInitialization();
 *     // Now safe to access all configuration data
 *     const client = useRealtimeClient();
 *     const agents = client?.getAgentsList();
 *   };
 * }
 * ```
 */
export function useInitializationStatus(): UseInitializationStatusReturn {
  const client = useRealtimeClientSafe();
  const [isInitialized, setIsInitialized] = useState(false);
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [connectionState, setConnectionState] = useState<ConnectionState>(ConnectionState.DISCONNECTED);
  
  // Check initialization status
  const checkStatus = useCallback(() => {
    if (!client) {
      setIsInitialized(false);
      setIsLoading(false);
      setConnectionState(ConnectionState.DISCONNECTED);
      return;
    }
    
    try {
      // Get current connection state
      const state = client.getConnectionState();
      setConnectionState(state);
      
      // Check if fully initialized
      const initialized = client.isFullyInitialized();
      setIsInitialized(initialized);
      
      // Determine loading state based on connection and initialization
      if (state === ConnectionState.CONNECTING) {
        setIsLoading(true);
      } else if (state === ConnectionState.CONNECTED && !initialized) {
        // Connected but waiting for initialization events
        setIsLoading(true);
      } else {
        setIsLoading(false);
      }
      
      // Clear error on successful status check
      if (state === ConnectionState.CONNECTED) {
        setError(null);
      }
    } catch (err) {
      console.error('Failed to check initialization status:', err);
      setError(err instanceof Error ? err.message : 'Failed to check status');
      setIsLoading(false);
    }
  }, [client]);
  
  // Wait for initialization helper
  const waitForInitialization = useCallback(async () => {
    if (!client) {
      throw new Error('Client not available');
    }
    
    // If already initialized, return immediately
    if (client.isFullyInitialized()) {
      return;
    }
    
    // Otherwise wait for initialization
    await client.waitForInitialization();
  }, [client]);
  
  // Subscribe to events
  useEffect(() => {
    if (!client) {
      setIsLoading(false);
      setIsInitialized(false);
      setConnectionState(ConnectionState.DISCONNECTED);
      return;
    }
    
    // Initial status check
    checkStatus();
    
    // Handle initialization complete
    const handleInitialized = () => {
      setIsInitialized(true);
      setIsLoading(false);
      setError(null);
    };
    
    // Handle connection state changes
    const handleConnected = () => {
      setConnectionState(ConnectionState.CONNECTED);
      // Still loading until initialization completes
      checkStatus();
    };
    
    const handleDisconnected = () => {
      setConnectionState(ConnectionState.DISCONNECTED);
      setIsLoading(false);
      // Don't set isInitialized to false - keep the last known state
    };
    
    const handleReconnecting = () => {
      setConnectionState(ConnectionState.RECONNECTING);
      setIsLoading(true);
    };
    
    const handleError = (event: any) => {
      const errorMessage = event?.error?.message || event?.message || 'Connection error';
      setError(errorMessage);
      setIsLoading(false);
    };
    
    // Subscribe to all relevant events
    client.on('initialized' as any, handleInitialized);
    client.on('connected', handleConnected);
    client.on('disconnected', handleDisconnected);
    client.on('reconnecting', handleReconnecting);
    client.on('error', handleError);
    
    // Also listen to individual initialization events to track progress
    const initEvents = [
      'chat_user_data',
      'avatar_list',
      'voice_list',
      'agent_list',
      'tool_catalog',
      'chat-session-changed'
    ];
    
    const handleInitEvent = () => {
      // Check status on each initialization event
      checkStatus();
    };
    
    initEvents.forEach(event => {
      client.on(event as any, handleInitEvent);
    });
    
    return () => {
      client.off('initialized' as any, handleInitialized);
      client.off('connected', handleConnected);
      client.off('disconnected', handleDisconnected);
      client.off('reconnecting', handleReconnecting);
      client.off('error', handleError);
      
      initEvents.forEach(event => {
        client.off(event as any, handleInitEvent);
      });
    };
  }, [client, checkStatus]);
  
  return {
    isInitialized,
    isLoading,
    error,
    connectionState,
    waitForInitialization
  };
}

// Re-export ConnectionState for convenience
export { ConnectionState };
/**
 * useUserData - React hook for accessing user data from WebSocket
 * 
 * This hook provides access to the user data received from the
 * chat_user_data event after WebSocket connection is established.
 */

import { useEffect, useState, useCallback } from 'react';
import { useRealtimeClientSafe } from '../providers/AgentCContext';
// Import the User type from core package instead of defining our own
import type { User } from '@agentc/realtime-core';

/**
 * Return type for the useUserData hook
 */
export interface UseUserDataReturn {
  /** Current user data from WebSocket */
  user: User | null;
  
  /** Whether user data is loading */
  isLoading: boolean;
  
  /** Error if failed to get user data */
  error: string | null;
  
  /** Refresh user data (typically happens automatically) */
  refresh: () => void;
}

/**
 * React hook for accessing user data from WebSocket
 * 
 * The user data is received from the chat_user_data event
 * after the WebSocket connection is established. This happens
 * automatically after successful authentication.
 * 
 * @example
 * ```tsx
 * function UserProfile() {
 *   const { user, isLoading } = useUserData();
 *   
 *   if (isLoading) return <div>Loading user...</div>;
 *   if (!user) return <div>No user data</div>;
 *   
 *   return (
 *     <div>
 *       <h2>{user.user_name}</h2>
 *       <p>{user.email || 'No email'}</p>
 *     </div>
 *   );
 * }
 * ```
 */
export function useUserData(): UseUserDataReturn {
  const client = useRealtimeClientSafe();
  const [user, setUser] = useState<User | null>(null);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  
  // Update user data from client
  const updateUserData = useCallback(() => {
    if (!client) {
      setUser(null);
      setIsLoading(false);
      setError('Client not available');
      return;
    }
    
    try {
      const userData = client.getUserData();
      setUser(userData);
      setError(null);
      
      // If we're connected but don't have user data yet, we're still loading
      if (client.isConnected() && !userData) {
        setIsLoading(true);
      } else {
        setIsLoading(false);
      }
    } catch (err) {
      console.error('Failed to get user data:', err);
      setError(err instanceof Error ? err.message : 'Failed to get user data');
      setIsLoading(false);
    }
  }, [client]);
  
  // Subscribe to user data updates
  useEffect(() => {
    if (!client) {
      setIsLoading(false);
      return;
    }
    
    // Initial update
    updateUserData();
    
    // Handle chat_user_data event
    const handleUserData = (event: any) => {
      if (event.user) {
        setUser(event.user);
        setIsLoading(false);
        setError(null);
      }
    };
    
    // Handle connection events
    const handleConnect = () => {
      // When connected, user data will come shortly
      setIsLoading(true);
      setError(null);
    };
    
    const handleDisconnect = () => {
      // Keep existing user data on disconnect
      // but mark as not loading
      setIsLoading(false);
    };
    
    const handleError = (event: any) => {
      // Extract error message from event
      const errorMessage = event?.error?.message || event?.message || 'An error occurred';
      setError(errorMessage);
      setIsLoading(false);
    };
    
    // Subscribe to events
    client.on('chat_user_data', handleUserData);
    client.on('connected', handleConnect);
    client.on('disconnected', handleDisconnect);
    client.on('error', handleError);
    
    return () => {
      client.off('chat_user_data', handleUserData);
      client.off('connected', handleConnect);
      client.off('disconnected', handleDisconnect);
      client.off('error', handleError);
    };
  }, [client, updateUserData]);
  
  return {
    user,
    isLoading,
    error,
    refresh: updateUserData
  };
}

// Re-export the User type for convenience
export type { User };
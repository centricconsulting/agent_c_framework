/**
 * useTurnState - React hook for monitoring turn state
 * Provides access to the current turn state from the TurnManager
 */

import { useEffect, useState } from 'react';
import { useRealtimeClientSafe } from '../providers/AgentCContext';

/**
 * Turn state event for history tracking
 */
export interface TurnStateEvent {
  /** Whether user can send input */
  canSendInput: boolean;
  
  /** Timestamp of the event */
  timestamp: number;
  
  /** Event type for clarity */
  type: 'user_turn_start' | 'user_turn_end';
}

/**
 * Options for the useTurnState hook
 */
export interface UseTurnStateOptions {
  /** Whether to track turn state history */
  trackHistory?: boolean;
  
  /** Maximum number of history events to keep */
  maxHistorySize?: number;
}

/**
 * Return type for the useTurnState hook
 */
export interface UseTurnStateReturn {
  /** Whether user can currently send input */
  canSendInput: boolean;
  
  /** Turn state history (if tracking is enabled) */
  turnStateHistory?: TurnStateEvent[];
  
  /** Whether a turn manager is available */
  hasTurnManager: boolean;
  
  /** Clear the turn state history */
  clearHistory?: () => void;
}

/**
 * React hook for monitoring turn state
 * Subscribes to turn manager events and provides current state
 */
export function useTurnState(options: UseTurnStateOptions = {}): UseTurnStateReturn {
  const { 
    trackHistory = false, 
    maxHistorySize = 100 
  } = options;
  const client = useRealtimeClientSafe();
  
  // State
  const [canSendInput, setCanSendInput] = useState(false);
  const [hasTurnManager, setHasTurnManager] = useState(false);
  const [turnStateHistory, setTurnStateHistory] = useState<TurnStateEvent[]>([]);
  
  // Subscribe to turn state changes
  useEffect(() => {
    if (!client) {
      setHasTurnManager(false);
      setCanSendInput(false);
      return;
    }
    
    const turnManager = client.getTurnManager();
    if (!turnManager) {
      // No turn manager means no turn-based control
      // User can always send input
      setHasTurnManager(false);
      setCanSendInput(true);
      return;
    }
    
    setHasTurnManager(true);
    
    // Set initial turn state
    setCanSendInput(turnManager.canSendInput);
    
    // Add initial state to history if tracking
    if (trackHistory) {
      setTurnStateHistory([{
        canSendInput: turnManager.canSendInput,
        timestamp: Date.now(),
        type: turnManager.canSendInput ? 'user_turn_start' : 'user_turn_end'
      }]);
    }
    
    // Subscribe to turn state changes
    const handleTurnStateChange = ({ canSendInput: newCanSendInput }: { canSendInput: boolean }) => {
      setCanSendInput(newCanSendInput);
      
      // Add to history if tracking
      if (trackHistory) {
        const event: TurnStateEvent = {
          canSendInput: newCanSendInput,
          timestamp: Date.now(),
          type: newCanSendInput ? 'user_turn_start' : 'user_turn_end'
        };
        
        setTurnStateHistory((prev: TurnStateEvent[]) => {
          const newHistory = [...prev, event];
          // Limit history size
          if (newHistory.length > maxHistorySize) {
            return newHistory.slice(-maxHistorySize);
          }
          return newHistory;
        });
      }
    };
    
    turnManager.on('turn-state-changed', handleTurnStateChange);
    
    // Cleanup
    return () => {
      turnManager.off('turn-state-changed', handleTurnStateChange);
    };
  }, [client, trackHistory, maxHistorySize]);
  
  // Clear history function
  const clearHistory = trackHistory ? () => {
    setTurnStateHistory([]);
  } : undefined;
  
  return {
    canSendInput,
    hasTurnManager,
    ...(trackHistory && { turnStateHistory, clearHistory })
  };
}
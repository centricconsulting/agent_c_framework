/**
 * Unit tests for useTurnState hook
 * Tests turn state management, history tracking, and event subscriptions
 */

import { describe, it, expect, vi, beforeEach, afterEach } from 'vitest';
import { renderHook, act } from '@testing-library/react';
import { useTurnState } from '../useTurnState';
import type { TurnStateEvent, UseTurnStateOptions, UseTurnStateReturn } from '../useTurnState';
import type { RealtimeClient, TurnManager } from '@agentc/realtime-core';

// Mock the AgentCContext hooks
const mockUseRealtimeClientSafe = vi.fn();

vi.mock('../../providers/AgentCContext', () => ({
  useRealtimeClientSafe: () => mockUseRealtimeClientSafe()
}));

// Store event handlers for manual triggering
const eventHandlers: Map<string, Set<Function>> = new Map();

// Mock TurnManager
const mockTurnManager: Partial<TurnManager> = {
  canSendInput: true,
  on: vi.fn((event: string, handler: Function) => {
    if (!eventHandlers.has(event)) {
      eventHandlers.set(event, new Set());
    }
    eventHandlers.get(event)!.add(handler);
  }),
  off: vi.fn((event: string, handler: Function) => {
    eventHandlers.get(event)?.delete(handler);
  })
};

// Mock Client
const mockClient: Partial<RealtimeClient> = {
  getTurnManager: vi.fn().mockReturnValue(mockTurnManager)
};

// Helper to trigger events within act()
const triggerTurnStateChange = (canSendInput: boolean) => {
  const handlers = eventHandlers.get('turn-state-changed');
  if (handlers) {
    act(() => {
      handlers.forEach(handler => handler({ canSendInput }));
    });
  }
};

// Test data factory
const createTurnStateEvent = (canSendInput: boolean, timestamp: number = Date.now()): TurnStateEvent => ({
  canSendInput,
  timestamp,
  type: canSendInput ? 'user_turn_start' : 'user_turn_end'
});

// Helper to render the hook with options
const renderUseTurnState = (options?: UseTurnStateOptions) => {
  return renderHook(() => useTurnState(options));
};

describe('useTurnState', () => {
  beforeEach(() => {
    vi.clearAllMocks();
    vi.clearAllTimers();
    eventHandlers.clear();
    
    // Use fake timers to control time-based operations
    vi.useFakeTimers();
    
    // Set up default mocks
    mockUseRealtimeClientSafe.mockReturnValue(mockClient);
    (mockClient.getTurnManager as any).mockReturnValue(mockTurnManager);
    (mockTurnManager.canSendInput as any) = true;
  });

  afterEach(() => {
    // Clear all timers to prevent state updates after test
    vi.clearAllTimers();
    vi.useRealTimers();
    vi.restoreAllMocks();
    eventHandlers.clear();
  });

  describe('No TurnManager Scenario', () => {
    it('allows input when no turn manager exists', () => {
      // Arrange
      (mockClient.getTurnManager as any).mockReturnValue(null);

      // Act
      const { result } = renderUseTurnState();

      // Assert
      expect(result.current.hasTurnManager).toBe(false);
      expect(result.current.canSendInput).toBe(true);
      expect(mockTurnManager.on).not.toHaveBeenCalled();
    });

    it('handles null client gracefully', () => {
      // Arrange
      mockUseRealtimeClientSafe.mockReturnValue(null);

      // Act
      const { result } = renderUseTurnState();

      // Assert
      expect(result.current.hasTurnManager).toBe(false);
      expect(result.current.canSendInput).toBe(false);
      expect(mockClient.getTurnManager).not.toHaveBeenCalled();
    });
  });

  describe('TurnManager Initialization', () => {
    it('reads initial turn state from manager when canSendInput is true', () => {
      // Arrange
      (mockTurnManager.canSendInput as any) = true;

      // Act
      const { result } = renderUseTurnState();

      // Assert
      expect(result.current.canSendInput).toBe(true);
      expect(result.current.hasTurnManager).toBe(true);
    });

    it('reads initial turn state from manager when canSendInput is false', () => {
      // Arrange
      (mockTurnManager.canSendInput as any) = false;

      // Act
      const { result } = renderUseTurnState();

      // Assert
      expect(result.current.canSendInput).toBe(false);
      expect(result.current.hasTurnManager).toBe(true);
    });

    it('sets hasTurnManager when manager exists', () => {
      // Arrange
      (mockClient.getTurnManager as any).mockReturnValue(mockTurnManager);

      // Act
      const { result } = renderUseTurnState();

      // Assert
      expect(result.current.hasTurnManager).toBe(true);
      expect(mockClient.getTurnManager).toHaveBeenCalledTimes(1);
    });
  });

  describe('Event Subscription', () => {
    it('subscribes to turn-state-changed event', () => {
      // Act
      renderUseTurnState();

      // Assert
      expect(mockTurnManager.on).toHaveBeenCalledWith('turn-state-changed', expect.any(Function));
      expect(mockTurnManager.on).toHaveBeenCalledTimes(1);
    });

    it('updates state when turn-state-changed event is fired with true', () => {
      // Arrange
      (mockTurnManager.canSendInput as any) = false; // Start with false
      const { result } = renderUseTurnState();
      
      // Initial state
      expect(result.current.canSendInput).toBe(false);

      // Act - Trigger turn state change to true
      triggerTurnStateChange(true);

      // Assert
      expect(result.current.canSendInput).toBe(true);
    });

    it('updates state when turn-state-changed event is fired with false', () => {
      // Arrange
      (mockTurnManager.canSendInput as any) = true; // Start with true
      const { result } = renderUseTurnState();
      
      // Initial state
      expect(result.current.canSendInput).toBe(true);

      // Act - Trigger turn state change to false
      triggerTurnStateChange(false);

      // Assert
      expect(result.current.canSendInput).toBe(false);
    });

    it('unsubscribes on unmount', () => {
      // Arrange
      const { unmount } = renderUseTurnState();
      
      // Verify event handler is registered
      expect(eventHandlers.get('turn-state-changed')?.size).toBeGreaterThan(0);

      // Act
      unmount();

      // Assert
      expect(mockTurnManager.off).toHaveBeenCalledWith('turn-state-changed', expect.any(Function));
      expect(mockTurnManager.off).toHaveBeenCalledTimes(1);
      
      // Verify the same handler that was registered is unregistered
      const registeredHandler = (mockTurnManager.on as any).mock.calls[0][1];
      const unregisteredHandler = (mockTurnManager.off as any).mock.calls[0][1];
      expect(registeredHandler).toBe(unregisteredHandler);
    });
  });

  describe('History Tracking', () => {
    it('does not track history by default', () => {
      // Act
      const { result } = renderUseTurnState();

      // Assert
      expect(result.current.turnStateHistory).toBeUndefined();
      expect(result.current.clearHistory).toBeUndefined();
    });

    it('adds initial state to history when tracking enabled with canSendInput true', () => {
      // Arrange
      (mockTurnManager.canSendInput as any) = true;
      const now = Date.now();
      vi.setSystemTime(now);

      // Act
      const { result } = renderUseTurnState({ trackHistory: true });

      // Assert
      expect(result.current.turnStateHistory).toBeDefined();
      expect(result.current.turnStateHistory).toHaveLength(1);
      expect(result.current.turnStateHistory![0]).toEqual({
        canSendInput: true,
        timestamp: now,
        type: 'user_turn_start'
      });
    });

    it('adds initial state to history when tracking enabled with canSendInput false', () => {
      // Arrange
      (mockTurnManager.canSendInput as any) = false;
      const now = Date.now();
      vi.setSystemTime(now);

      // Act
      const { result } = renderUseTurnState({ trackHistory: true });

      // Assert
      expect(result.current.turnStateHistory).toBeDefined();
      expect(result.current.turnStateHistory).toHaveLength(1);
      expect(result.current.turnStateHistory![0]).toEqual({
        canSendInput: false,
        timestamp: now,
        type: 'user_turn_end'
      });
    });

    it('appends events to history on state changes', () => {
      // Arrange
      const startTime = Date.now();
      vi.setSystemTime(startTime);
      
      (mockTurnManager.canSendInput as any) = true;
      const { result } = renderUseTurnState({ trackHistory: true });
      
      // Initial history should have one event
      expect(result.current.turnStateHistory).toHaveLength(1);

      // Act - Trigger multiple state changes
      vi.setSystemTime(startTime + 1000);
      triggerTurnStateChange(false);
      
      vi.setSystemTime(startTime + 2000);
      triggerTurnStateChange(true);
      
      vi.setSystemTime(startTime + 3000);
      triggerTurnStateChange(false);

      // Assert
      expect(result.current.turnStateHistory).toHaveLength(4);
      
      // Verify events in order
      expect(result.current.turnStateHistory![0]).toEqual({
        canSendInput: true,
        timestamp: startTime,
        type: 'user_turn_start'
      });
      
      expect(result.current.turnStateHistory![1]).toEqual({
        canSendInput: false,
        timestamp: startTime + 1000,
        type: 'user_turn_end'
      });
      
      expect(result.current.turnStateHistory![2]).toEqual({
        canSendInput: true,
        timestamp: startTime + 2000,
        type: 'user_turn_start'
      });
      
      expect(result.current.turnStateHistory![3]).toEqual({
        canSendInput: false,
        timestamp: startTime + 3000,
        type: 'user_turn_end'
      });
    });

    it('limits history to maxHistorySize', () => {
      // Arrange
      const startTime = Date.now();
      vi.setSystemTime(startTime);
      
      (mockTurnManager.canSendInput as any) = true;
      const { result } = renderUseTurnState({ 
        trackHistory: true, 
        maxHistorySize: 3 
      });
      
      // Initial history should have one event
      expect(result.current.turnStateHistory).toHaveLength(1);

      // Act - Trigger 5 state changes (total 6 events including initial)
      for (let i = 1; i <= 5; i++) {
        vi.setSystemTime(startTime + i * 1000);
        triggerTurnStateChange(i % 2 === 0); // Alternate true/false
      }

      // Assert - Should only keep last 3 events
      expect(result.current.turnStateHistory).toHaveLength(3);
      
      // Verify we kept the most recent events (indices 3, 4, 5)
      expect(result.current.turnStateHistory![0].timestamp).toBe(startTime + 3000);
      expect(result.current.turnStateHistory![1].timestamp).toBe(startTime + 4000);
      expect(result.current.turnStateHistory![2].timestamp).toBe(startTime + 5000);
    });

    it('provides clearHistory function when tracking', () => {
      // Arrange
      (mockTurnManager.canSendInput as any) = true;
      const { result } = renderUseTurnState({ trackHistory: true });
      
      // Add some history by triggering state changes
      triggerTurnStateChange(false);
      triggerTurnStateChange(true);
      
      // Verify history has events
      expect(result.current.turnStateHistory).toHaveLength(3);
      expect(result.current.clearHistory).toBeDefined();

      // Act - Clear history
      act(() => {
        result.current.clearHistory!();
      });

      // Assert
      expect(result.current.turnStateHistory).toEqual([]);
    });

    it('does not provide clearHistory when not tracking', () => {
      // Act
      const { result } = renderUseTurnState({ trackHistory: false });

      // Assert
      expect(result.current.clearHistory).toBeUndefined();
    });
  });

  describe('Return Value Shape', () => {
    it('returns correct shape without history', () => {
      // Act
      const { result } = renderUseTurnState();

      // Assert
      expect(result.current).toEqual({
        canSendInput: true,
        hasTurnManager: true
      });
      
      // Explicitly check that history-related properties are not present
      expect('turnStateHistory' in result.current).toBe(false);
      expect('clearHistory' in result.current).toBe(false);
    });

    it('returns correct shape with history', () => {
      // Arrange
      const now = Date.now();
      vi.setSystemTime(now);
      (mockTurnManager.canSendInput as any) = true;

      // Act
      const { result } = renderUseTurnState({ trackHistory: true });

      // Assert
      expect(result.current).toEqual({
        canSendInput: true,
        hasTurnManager: true,
        turnStateHistory: [createTurnStateEvent(true, now)],
        clearHistory: expect.any(Function)
      });
    });
  });

  describe('Options Handling', () => {
    it('uses default options when none provided', () => {
      // Act
      const { result } = renderUseTurnState();

      // Assert - trackHistory defaults to false, so no history properties
      expect(result.current.turnStateHistory).toBeUndefined();
      expect(result.current.clearHistory).toBeUndefined();
    });

    it('respects custom trackHistory option', () => {
      // Act
      const { result } = renderUseTurnState({ trackHistory: true });

      // Assert
      expect(result.current.turnStateHistory).toBeDefined();
      expect(result.current.clearHistory).toBeDefined();
    });

    it('respects custom maxHistorySize option', () => {
      // Arrange
      const startTime = Date.now();
      vi.setSystemTime(startTime);
      
      (mockTurnManager.canSendInput as any) = true;
      const { result } = renderUseTurnState({ 
        trackHistory: true, 
        maxHistorySize: 2 
      });

      // Act - Add 3 events total (1 initial + 2 changes)
      vi.setSystemTime(startTime + 1000);
      triggerTurnStateChange(false);
      
      vi.setSystemTime(startTime + 2000);
      triggerTurnStateChange(true);

      // Assert - Should only keep last 2 events
      expect(result.current.turnStateHistory).toHaveLength(2);
      expect(result.current.turnStateHistory![0].timestamp).toBe(startTime + 1000);
      expect(result.current.turnStateHistory![1].timestamp).toBe(startTime + 2000);
    });

    it('uses default maxHistorySize of 100 when not specified', () => {
      // Arrange
      const startTime = Date.now();
      vi.setSystemTime(startTime);
      
      (mockTurnManager.canSendInput as any) = true;
      const { result } = renderUseTurnState({ trackHistory: true });

      // Act - Add 101 events total (1 initial + 100 changes)
      for (let i = 1; i <= 100; i++) {
        vi.setSystemTime(startTime + i * 1000);
        triggerTurnStateChange(i % 2 === 0);
      }

      // Assert - Should keep exactly 100 events (default maxHistorySize)
      expect(result.current.turnStateHistory).toHaveLength(100);
      
      // Verify we kept the most recent 100 events
      expect(result.current.turnStateHistory![0].timestamp).toBe(startTime + 1000);
      expect(result.current.turnStateHistory![99].timestamp).toBe(startTime + 100000);
    });
  });

  describe('Edge Cases', () => {
    it('handles rapid turn state changes correctly', () => {
      // Arrange
      (mockTurnManager.canSendInput as any) = true;
      const { result } = renderUseTurnState({ trackHistory: true });

      // Act - Trigger rapid state changes
      for (let i = 0; i < 10; i++) {
        triggerTurnStateChange(i % 2 === 0);
      }

      // Assert - All changes should be recorded
      expect(result.current.turnStateHistory).toHaveLength(11); // 1 initial + 10 changes
      
      // Verify final state matches last change
      expect(result.current.canSendInput).toBe(false); // Last change was i=9, 9%2=1, so false
    });

    it('handles re-render without re-subscribing', () => {
      // Arrange
      const { result, rerender } = renderUseTurnState();
      
      // Clear mock to check re-subscription
      vi.clearAllMocks();

      // Act - Force re-render
      rerender();

      // Assert - Should not re-subscribe
      expect(mockTurnManager.on).not.toHaveBeenCalled();
      expect(mockClient.getTurnManager).not.toHaveBeenCalled();
      
      // State should remain the same
      expect(result.current.canSendInput).toBe(true);
      expect(result.current.hasTurnManager).toBe(true);
    });

    it('handles changing from no manager to having manager', () => {
      // Arrange - Start with no manager
      const clientWithoutManager = {
        ...mockClient,
        getTurnManager: vi.fn().mockReturnValue(null)
      };
      mockUseRealtimeClientSafe.mockReturnValue(clientWithoutManager);
      
      const { result, rerender } = renderUseTurnState();
      
      // Verify initial state
      expect(result.current.hasTurnManager).toBe(false);
      expect(result.current.canSendInput).toBe(true);
      
      // Create a new client with manager (triggers useEffect due to new client reference)
      const clientWithManager = {
        ...mockClient,
        getTurnManager: vi.fn().mockReturnValue(mockTurnManager)
      };
      (mockTurnManager.canSendInput as any) = false;
      
      // Act - Change the client reference
      mockUseRealtimeClientSafe.mockReturnValue(clientWithManager);
      rerender();

      // Assert - Should now have manager and read its state
      expect(result.current.hasTurnManager).toBe(true);
      expect(result.current.canSendInput).toBe(false);
      expect(mockTurnManager.on).toHaveBeenCalledWith('turn-state-changed', expect.any(Function));
    });

    it('handles manager being removed', () => {
      // Arrange - Start with manager
      const clientWithManager = {
        ...mockClient,
        getTurnManager: vi.fn().mockReturnValue(mockTurnManager)
      };
      mockUseRealtimeClientSafe.mockReturnValue(clientWithManager);
      
      const { result, rerender } = renderUseTurnState();
      
      // Verify initial state
      expect(result.current.hasTurnManager).toBe(true);
      expect(mockTurnManager.on).toHaveBeenCalled();
      
      // Create a new client without manager (triggers useEffect due to new client reference)
      const clientWithoutManager = {
        ...mockClient,
        getTurnManager: vi.fn().mockReturnValue(null)
      };
      
      // Act - Change the client reference
      mockUseRealtimeClientSafe.mockReturnValue(clientWithoutManager);
      rerender();

      // Assert - Should handle no manager state
      expect(result.current.hasTurnManager).toBe(false);
      expect(result.current.canSendInput).toBe(true); // Default to true when no manager
      expect(mockTurnManager.off).toHaveBeenCalled(); // Should clean up old subscription
    });
  });

  describe('Type Safety', () => {
    it('properly types the return value', () => {
      // This test ensures TypeScript compilation with proper types
      const { result } = renderUseTurnState({ trackHistory: true });
      
      // These should all be properly typed
      const canSendInput: boolean = result.current.canSendInput;
      const hasTurnManager: boolean = result.current.hasTurnManager;
      const history: TurnStateEvent[] | undefined = result.current.turnStateHistory;
      const clear: (() => void) | undefined = result.current.clearHistory;
      
      // Type assertions to satisfy TypeScript
      expect(typeof canSendInput).toBe('boolean');
      expect(typeof hasTurnManager).toBe('boolean');
      expect(Array.isArray(history)).toBe(true);
      expect(typeof clear).toBe('function');
    });

    it('correctly types turn state events', () => {
      // Arrange
      const { result } = renderUseTurnState({ trackHistory: true });
      
      // Act - Trigger a state change
      triggerTurnStateChange(true);
      
      // Assert - Check event structure
      const latestEvent = result.current.turnStateHistory![result.current.turnStateHistory!.length - 1];
      expect(latestEvent).toHaveProperty('canSendInput');
      expect(latestEvent).toHaveProperty('timestamp');
      expect(latestEvent).toHaveProperty('type');
      expect(['user_turn_start', 'user_turn_end']).toContain(latestEvent.type);
    });
  });
});
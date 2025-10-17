/**
 * Unit tests for useConnection hook
 * Part 1: Basic State Management Tests
 * Part 2: Connection Actions Tests
 */

import { describe, it, expect, vi, beforeEach, afterEach } from 'vitest';
import { renderHook, act } from '@testing-library/react';
import { ConnectionState } from '@agentc/realtime-core';
import { useConnection } from '../useConnection';
import type { RealtimeClient } from '@agentc/realtime-core';

// Mock the AgentCContext hooks
const mockUseRealtimeClientSafe = vi.fn();
const mockUseAgentCContext = vi.fn();

vi.mock('../../providers/AgentCContext', () => ({
  useRealtimeClientSafe: () => mockUseRealtimeClientSafe(),
  useAgentCContext: () => mockUseAgentCContext()
}));

// Store event handlers for manual triggering
const eventHandlers: Map<string, Set<Function>> = new Map();

const mockClient: Partial<RealtimeClient> = {
  getConnectionState: vi.fn(),
  isConnected: vi.fn(),
  connect: vi.fn(),
  disconnect: vi.fn(),
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

const mockContext = {
  isInitializing: false,
  error: null
};

// Helper to trigger events within act()
const triggerEvent = (event: string, ...args: any[]) => {
  const handlers = eventHandlers.get(event);
  if (handlers) {
    act(() => {
      handlers.forEach(handler => handler(...args));
    });
  }
};

describe('useConnection', () => {
  beforeEach(() => {
    vi.clearAllMocks();
    vi.clearAllTimers();
    eventHandlers.clear();
    
    // Use fake timers to control time-based operations
    vi.useFakeTimers();
    
    // Set up default mocks
    mockUseRealtimeClientSafe.mockReturnValue(mockClient);
    mockUseAgentCContext.mockReturnValue(mockContext);
    
    // Set default mock implementations
    (mockClient.getConnectionState as any).mockReturnValue(ConnectionState.DISCONNECTED);
    (mockClient.isConnected as any).mockReturnValue(false);
  });

  afterEach(() => {
    // Clear all timers to prevent state updates after test
    vi.clearAllTimers();
    vi.useRealTimers();
    vi.restoreAllMocks();
    eventHandlers.clear();
  });

  describe('Basic State Management', () => {
    it('returns disconnected state when client is null', () => {
      // Arrange
      mockUseRealtimeClientSafe.mockReturnValue(null);

      // Act
      const { result } = renderHook(() => useConnection());

      // Assert
      expect(result.current.connectionState).toBe(ConnectionState.DISCONNECTED);
      expect(result.current.isConnected).toBe(false);
      expect(result.current.isDisconnected).toBe(true);
      expect(result.current.isConnecting).toBe(false);
    });

    it('updates connection state from client', () => {
      // Test DISCONNECTED state
      {
        // Arrange
        (mockClient.getConnectionState as any).mockReturnValue(ConnectionState.DISCONNECTED);

        // Act
        const { result } = renderHook(() => useConnection());

        // Assert
        expect(result.current.connectionState).toBe(ConnectionState.DISCONNECTED);
        expect(mockClient.getConnectionState).toHaveBeenCalled();
      }

      // Clear mocks for next test
      vi.clearAllMocks();

      // Test CONNECTED state
      {
        // Arrange
        (mockClient.getConnectionState as any).mockReturnValue(ConnectionState.CONNECTED);

        // Act
        const { result } = renderHook(() => useConnection());

        // Assert
        expect(result.current.connectionState).toBe(ConnectionState.CONNECTED);
        expect(mockClient.getConnectionState).toHaveBeenCalled();
      }

      // Clear mocks for next test
      vi.clearAllMocks();

      // Test CONNECTING state
      {
        // Arrange
        (mockClient.getConnectionState as any).mockReturnValue(ConnectionState.CONNECTING);

        // Act
        const { result } = renderHook(() => useConnection());

        // Assert
        expect(result.current.connectionState).toBe(ConnectionState.CONNECTING);
        expect(mockClient.getConnectionState).toHaveBeenCalled();
      }

      // Clear mocks for next test
      vi.clearAllMocks();

      // Test RECONNECTING state
      {
        // Arrange
        (mockClient.getConnectionState as any).mockReturnValue(ConnectionState.RECONNECTING);

        // Act
        const { result } = renderHook(() => useConnection());

        // Assert
        expect(result.current.connectionState).toBe(ConnectionState.RECONNECTING);
        expect(mockClient.getConnectionState).toHaveBeenCalled();
      }
    });

    it('provides correct boolean flags', () => {
      // Test DISCONNECTED state flags
      {
        // Arrange
        (mockClient.getConnectionState as any).mockReturnValue(ConnectionState.DISCONNECTED);

        // Act
        const { result } = renderHook(() => useConnection());

        // Assert
        expect(result.current.isConnected).toBe(false);
        expect(result.current.isConnecting).toBe(false);
        expect(result.current.isDisconnected).toBe(true);
      }

      // Clear mocks for next test
      vi.clearAllMocks();

      // Test CONNECTED state flags
      {
        // Arrange
        (mockClient.getConnectionState as any).mockReturnValue(ConnectionState.CONNECTED);

        // Act
        const { result } = renderHook(() => useConnection());

        // Assert
        expect(result.current.isConnected).toBe(true);
        expect(result.current.isConnecting).toBe(false);
        expect(result.current.isDisconnected).toBe(false);
      }

      // Clear mocks for next test
      vi.clearAllMocks();

      // Test CONNECTING state flags
      {
        // Arrange
        (mockClient.getConnectionState as any).mockReturnValue(ConnectionState.CONNECTING);

        // Act
        const { result } = renderHook(() => useConnection());

        // Assert
        expect(result.current.isConnected).toBe(false);
        expect(result.current.isConnecting).toBe(true);
        expect(result.current.isDisconnected).toBe(false);
      }

      // Clear mocks for next test
      vi.clearAllMocks();

      // Test RECONNECTING state flags
      {
        // Arrange
        (mockClient.getConnectionState as any).mockReturnValue(ConnectionState.RECONNECTING);

        // Act
        const { result } = renderHook(() => useConnection());

        // Assert
        expect(result.current.isConnected).toBe(false);
        expect(result.current.isConnecting).toBe(false);
        expect(result.current.isDisconnected).toBe(false);
        
        // RECONNECTING should be tracked in connectionState but not have its own flag
        expect(result.current.connectionState).toBe(ConnectionState.RECONNECTING);
      }
    });
  });

  describe('Connection Actions', () => {
    it('connect() handles successful connection', async () => {
      // Arrange
      const mockConnect = vi.fn().mockResolvedValue(undefined);
      const mockIsConnected = vi.fn().mockReturnValue(false);
      const mockGetConnectionState = vi.fn()
        .mockReturnValueOnce(ConnectionState.DISCONNECTED)
        .mockReturnValueOnce(ConnectionState.CONNECTED);
      
      const clientWithConnect = {
        ...mockClient,
        connect: mockConnect,
        isConnected: mockIsConnected,
        getConnectionState: mockGetConnectionState
      };
      
      mockUseRealtimeClientSafe.mockReturnValue(clientWithConnect);

      // Act
      const { result } = renderHook(() => useConnection());
      
      // Initial state should be disconnected
      expect(result.current.connectionState).toBe(ConnectionState.DISCONNECTED);
      expect(result.current.stats.connectionAttempts).toBe(0);
      expect(result.current.stats.successfulConnections).toBe(0);
      
      // Call connect within act to handle state updates
      await act(async () => {
        await result.current.connect();
      });

      // Assert
      expect(mockConnect).toHaveBeenCalledTimes(1);
      expect(mockIsConnected).toHaveBeenCalled();
      expect(result.current.stats.connectionAttempts).toBe(1);
      expect(result.current.stats.successfulConnections).toBe(1);
    });

    it('connect() handles connection failure', async () => {
      // Arrange
      const connectionError = new Error('Connection failed');
      const mockConnect = vi.fn().mockRejectedValue(connectionError);
      const mockIsConnected = vi.fn().mockReturnValue(false);
      
      const clientWithConnect = {
        ...mockClient,
        connect: mockConnect,
        isConnected: mockIsConnected
      };
      
      mockUseRealtimeClientSafe.mockReturnValue(clientWithConnect);

      // Act
      const { result } = renderHook(() => useConnection());
      
      // Initial state
      expect(result.current.error).toBe(null);
      expect(result.current.stats.failedConnections).toBe(0);
      
      // Call connect and expect it to throw
      let thrownError: Error | undefined;
      await act(async () => {
        try {
          await result.current.connect();
        } catch (err) {
          thrownError = err as Error;
        }
      });

      // Assert
      expect(mockConnect).toHaveBeenCalledTimes(1);
      expect(thrownError).toBeDefined();
      expect(thrownError?.message).toBe('Connection failed');
      expect(result.current.stats.failedConnections).toBe(1);
      expect(result.current.error?.message).toBe('Connection failed');
    });

    it('disconnect() updates state correctly', () => {
      // Arrange
      const mockDisconnect = vi.fn();
      const clientWithDisconnect = {
        ...mockClient,
        disconnect: mockDisconnect
      };
      
      mockUseRealtimeClientSafe.mockReturnValue(clientWithDisconnect);

      // Act
      const { result } = renderHook(() => useConnection());
      
      // Call disconnect within act to handle state updates
      act(() => {
        result.current.disconnect();
      });

      // Assert
      expect(mockDisconnect).toHaveBeenCalledTimes(1);
      expect(result.current.stats.lastDisconnectedAt).toBeInstanceOf(Date);
    });

    it('reconnect() disconnects then connects', async () => {
      // Arrange
      const mockConnect = vi.fn().mockResolvedValue(undefined);
      const mockDisconnect = vi.fn();
      
      // Mock isConnected to return true initially (for disconnect check)
      // then false after disconnect is called (for connect check)
      const mockIsConnected = vi.fn()
        .mockReturnValueOnce(true)  // First call in reconnect() to check if connected
        .mockReturnValueOnce(false); // Second call in connect() to check if already connected
      
      const clientWithReconnect = {
        ...mockClient,
        connect: mockConnect,
        disconnect: mockDisconnect,
        isConnected: mockIsConnected
      };
      
      mockUseRealtimeClientSafe.mockReturnValue(clientWithReconnect);

      // Act
      const { result } = renderHook(() => useConnection());
      
      // Call reconnect within act
      await act(async () => {
        const reconnectPromise = result.current.reconnect();
        
        // Fast-forward through the 100ms delay
        await vi.advanceTimersByTimeAsync(100);
        
        // Wait for reconnect to complete
        await reconnectPromise;
      });

      // Assert
      expect(mockIsConnected).toHaveBeenCalledTimes(2); // Once in reconnect, once in connect
      expect(mockDisconnect).toHaveBeenCalledTimes(1);
      expect(mockConnect).toHaveBeenCalledTimes(1);
      
      // Verify order: disconnect should be called before connect
      const disconnectCallOrder = mockDisconnect.mock.invocationCallOrder[0];
      const connectCallOrder = mockConnect.mock.invocationCallOrder[0];
      expect(disconnectCallOrder).toBeLessThan(connectCallOrder);
    });
  });

  describe('Event Handling', () => {
    it('subscribes to connection events', () => {
      // Arrange & Act
      renderHook(() => useConnection());
      
      // Assert - Verify all event subscriptions are set up
      expect(mockClient.on).toHaveBeenCalledWith('connected', expect.any(Function));
      expect(mockClient.on).toHaveBeenCalledWith('disconnected', expect.any(Function));
      expect(mockClient.on).toHaveBeenCalledWith('reconnecting', expect.any(Function));
      expect(mockClient.on).toHaveBeenCalledWith('error', expect.any(Function));
      
      // Verify exactly 4 event listeners are registered
      expect(mockClient.on).toHaveBeenCalledTimes(4);
    });

    it('handles connected event properly', () => {
      // Arrange
      const { result } = renderHook(() => useConnection());
      
      // Initial state
      expect(result.current.connectionState).toBe(ConnectionState.DISCONNECTED);
      
      // Act - Trigger connected event
      triggerEvent('connected');
      
      // Assert - State updates are synchronous within act()
      expect(result.current.connectionState).toBe(ConnectionState.CONNECTED);
      expect(result.current.error).toBe(null);
      expect(result.current.stats.lastConnectedAt).toBeInstanceOf(Date);
    });

    it('handles disconnected event with normal closure', () => {
      // Arrange
      const { result } = renderHook(() => useConnection());
      
      // Ensure no initial error
      expect(result.current.error).toBe(null);
      
      // Act - Trigger disconnected event with normal closure code (1000)
      triggerEvent('disconnected', { code: 1000, reason: 'Normal closure' });
      
      // Assert - State updates are synchronous within act()
      expect(result.current.connectionState).toBe(ConnectionState.DISCONNECTED);
      expect(result.current.error).toBe(null); // Error should remain null for normal closure
      expect(result.current.stats.lastDisconnectedAt).toBeInstanceOf(Date);
    });

    it('does not overwrite existing error on normal disconnection', () => {
      // Arrange
      const { result } = renderHook(() => useConnection());
      
      // Set initial error
      triggerEvent('error', new Error('Previous error'));
      expect(result.current.error?.message).toBe('Previous error');
      
      // Act - Trigger disconnected event with normal closure code (1000)
      triggerEvent('disconnected', { code: 1000, reason: 'Normal closure' });
      
      // Assert - Previous error should remain (not overwritten for normal closure)
      expect(result.current.connectionState).toBe(ConnectionState.DISCONNECTED);
      expect(result.current.error?.message).toBe('Previous error'); // Error preserved
      expect(result.current.stats.lastDisconnectedAt).toBeInstanceOf(Date);
    });

    it('handles disconnected event with error codes', () => {
      // Arrange
      const { result } = renderHook(() => useConnection());
      
      // Act - Trigger disconnected event with error code (not 1000)
      triggerEvent('disconnected', { code: 1006, reason: 'Connection lost' });
      
      // Assert - State updates are synchronous within act()
      expect(result.current.connectionState).toBe(ConnectionState.DISCONNECTED);
      expect(result.current.error?.message).toContain('Connection lost');
      expect(result.current.error?.message).toContain('code: 1006');
      expect(result.current.stats.lastDisconnectedAt).toBeInstanceOf(Date);
    });

    it('handles reconnecting event properly', () => {
      // Arrange
      const { result } = renderHook(() => useConnection());
      
      // Act - Trigger reconnecting event
      triggerEvent('reconnecting', { attempt: 3 });
      
      // Assert - State updates are synchronous within act()
      expect(result.current.connectionState).toBe(ConnectionState.RECONNECTING);
      expect(result.current.reconnectAttempt).toBe(3);
    });

    it('handles error event properly', () => {
      // Arrange
      const { result } = renderHook(() => useConnection());
      
      // Initial state
      expect(result.current.error).toBe(null);
      expect(result.current.stats.failedConnections).toBe(0);
      
      // Act - Trigger error event
      const testError = new Error('WebSocket error');
      triggerEvent('error', testError);
      
      // Assert - State updates are synchronous within act()
      expect(result.current.error?.message).toBe('WebSocket error');
      expect(result.current.stats.failedConnections).toBe(1);
    });

    it('cleans up event listeners on unmount', () => {
      // Arrange
      const { unmount } = renderHook(() => useConnection());
      
      // Verify event handlers are registered
      expect(eventHandlers.get('connected')?.size).toBeGreaterThan(0);
      expect(eventHandlers.get('disconnected')?.size).toBeGreaterThan(0);
      expect(eventHandlers.get('reconnecting')?.size).toBeGreaterThan(0);
      expect(eventHandlers.get('error')?.size).toBeGreaterThan(0);
      
      // Act - Unmount the hook
      unmount();
      
      // Assert - Event handlers should be cleaned up
      expect(mockClient.off).toHaveBeenCalledWith('connected', expect.any(Function));
      expect(mockClient.off).toHaveBeenCalledWith('disconnected', expect.any(Function));
      expect(mockClient.off).toHaveBeenCalledWith('reconnecting', expect.any(Function));
      expect(mockClient.off).toHaveBeenCalledWith('error', expect.any(Function));
    });
  });

  describe('Session Duration Tracking', () => {
    it('updates session duration when connected', () => {
      // Arrange
      const { result } = renderHook(() => useConnection());
      
      // Simulate connection to start session
      triggerEvent('connected');
      
      // Verify connected state
      expect(result.current.connectionState).toBe(ConnectionState.CONNECTED);
      
      // Act - Advance time by 2 seconds
      act(() => {
        vi.advanceTimersByTime(2000);
      });
      
      // Assert - Session duration should be updated
      expect(result.current.stats.sessionDuration).toBeGreaterThanOrEqual(2000);
    });

    it('stops updating session duration when disconnected', () => {
      // Arrange
      const { result } = renderHook(() => useConnection());
      
      // Connect first
      triggerEvent('connected');
      expect(result.current.connectionState).toBe(ConnectionState.CONNECTED);
      
      // Advance time and verify duration updates
      act(() => {
        vi.advanceTimersByTime(1000);
      });
      
      const durationBeforeDisconnect = result.current.stats.sessionDuration;
      expect(durationBeforeDisconnect).toBeGreaterThanOrEqual(1000);
      
      // Disconnect
      triggerEvent('disconnected', { code: 1000, reason: 'Normal closure' });
      expect(result.current.connectionState).toBe(ConnectionState.DISCONNECTED);
      
      // Record duration immediately after disconnect
      const durationAfterDisconnect = result.current.stats.sessionDuration;
      
      // Advance time after disconnect
      act(() => {
        vi.advanceTimersByTime(2000);
      });
      
      // Assert - Duration should NOT update after disconnect (remains at last value)
      expect(result.current.stats.sessionDuration).toBe(durationAfterDisconnect);
      expect(result.current.stats.sessionDuration).toBe(durationBeforeDisconnect);
    });
  });
});
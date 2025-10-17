import { describe, it, expect, vi, beforeEach, afterEach } from 'vitest';
import { renderHook, act, waitFor } from '@testing-library/react';
import { useInitializationStatus } from '../useInitializationStatus';
import { ConnectionState } from '@agentc/realtime-core';

// Mock the context
const mockUseRealtimeClientSafe = vi.fn();

vi.mock('../../providers/AgentCContext', () => ({
  useRealtimeClientSafe: () => mockUseRealtimeClientSafe()
}));

describe('useInitializationStatus', () => {
  let mockClient: any;
  
  beforeEach(() => {
    vi.clearAllMocks();
    
    // Create mock client
    mockClient = {
      getConnectionState: vi.fn().mockReturnValue(ConnectionState.DISCONNECTED),
      isFullyInitialized: vi.fn().mockReturnValue(false),
      waitForInitialization: vi.fn().mockResolvedValue(undefined),
      on: vi.fn(),
      off: vi.fn()
    };
    
    // Set the mock to return our client
    mockUseRealtimeClientSafe.mockReturnValue(mockClient);
  });
  
  afterEach(() => {
    vi.restoreAllMocks();
  });
  
  // Helper to emit events
  const emitEvent = (eventName: string, data?: any) => {
    const handler = mockClient.on.mock.calls.find(
      (call: any[]) => call[0] === eventName
    )?.[1];
    if (handler) {
      act(() => {
        handler(data);
      });
    }
  };
  
  describe('Initialization State Tests', () => {
    it('returns default state when client is null', () => {
      mockUseRealtimeClientSafe.mockReturnValue(null);
      
      const { result } = renderHook(() => useInitializationStatus());
      
      expect(result.current.isInitialized).toBe(false);
      expect(result.current.isLoading).toBe(false);
      expect(result.current.connectionState).toBe(ConnectionState.DISCONNECTED);
      expect(result.current.error).toBeNull();
    });
    
    it('checks initialization status from client', () => {
      mockClient.isFullyInitialized.mockReturnValue(true);
      mockClient.getConnectionState.mockReturnValue(ConnectionState.CONNECTED);
      
      const { result } = renderHook(() => useInitializationStatus());
      
      expect(result.current.isInitialized).toBe(true);
      expect(mockClient.isFullyInitialized).toHaveBeenCalled();
    });
    
    it('reflects false initialization status', () => {
      mockClient.isFullyInitialized.mockReturnValue(false);
      
      const { result } = renderHook(() => useInitializationStatus());
      
      expect(result.current.isInitialized).toBe(false);
    });
  });
  
  describe('Loading State Logic Tests', () => {
    it('sets loading when connecting', () => {
      mockClient.getConnectionState.mockReturnValue(ConnectionState.CONNECTING);
      
      const { result } = renderHook(() => useInitializationStatus());
      
      expect(result.current.isLoading).toBe(true);
      expect(result.current.connectionState).toBe(ConnectionState.CONNECTING);
    });
    
    it('sets loading when connected but not initialized', () => {
      mockClient.getConnectionState.mockReturnValue(ConnectionState.CONNECTED);
      mockClient.isFullyInitialized.mockReturnValue(false);
      
      const { result } = renderHook(() => useInitializationStatus());
      
      expect(result.current.isLoading).toBe(true);
      expect(result.current.connectionState).toBe(ConnectionState.CONNECTED);
    });
    
    it('clears loading when fully initialized', () => {
      mockClient.getConnectionState.mockReturnValue(ConnectionState.CONNECTED);
      mockClient.isFullyInitialized.mockReturnValue(true);
      
      const { result } = renderHook(() => useInitializationStatus());
      
      expect(result.current.isLoading).toBe(false);
      expect(result.current.isInitialized).toBe(true);
    });
    
    it('clears loading when disconnected', () => {
      mockClient.getConnectionState.mockReturnValue(ConnectionState.DISCONNECTED);
      
      const { result } = renderHook(() => useInitializationStatus());
      
      expect(result.current.isLoading).toBe(false);
      expect(result.current.connectionState).toBe(ConnectionState.DISCONNECTED);
    });
  });
  
  describe('Event Handler Tests', () => {
    it('handles initialized event', () => {
      const { result } = renderHook(() => useInitializationStatus());
      
      // Initial state
      expect(result.current.isInitialized).toBe(false);
      
      // Emit initialized event
      emitEvent('initialized');
      
      expect(result.current.isInitialized).toBe(true);
      expect(result.current.isLoading).toBe(false);
      expect(result.current.error).toBeNull();
    });
    
    it('handles connected event', () => {
      mockClient.getConnectionState.mockReturnValue(ConnectionState.CONNECTED);
      mockClient.isFullyInitialized.mockReturnValue(false);
      
      const { result } = renderHook(() => useInitializationStatus());
      
      // Emit connected event
      emitEvent('connected');
      
      expect(result.current.connectionState).toBe(ConnectionState.CONNECTED);
      expect(result.current.isLoading).toBe(true); // Still loading until initialized
      expect(mockClient.getConnectionState).toHaveBeenCalled();
    });
    
    it('handles disconnected event and preserves initialization state', () => {
      // Start initialized
      mockClient.isFullyInitialized.mockReturnValue(true);
      mockClient.getConnectionState.mockReturnValue(ConnectionState.CONNECTED);
      
      const { result } = renderHook(() => useInitializationStatus());
      
      expect(result.current.isInitialized).toBe(true);
      
      // Emit disconnected event
      emitEvent('disconnected');
      
      expect(result.current.connectionState).toBe(ConnectionState.DISCONNECTED);
      expect(result.current.isLoading).toBe(false);
      expect(result.current.isInitialized).toBe(true); // Should remain true!
    });
    
    it('handles reconnecting event', () => {
      const { result } = renderHook(() => useInitializationStatus());
      
      emitEvent('reconnecting');
      
      expect(result.current.connectionState).toBe(ConnectionState.RECONNECTING);
      expect(result.current.isLoading).toBe(true);
    });
    
    it('handles error event with nested error message', () => {
      const { result } = renderHook(() => useInitializationStatus());
      
      emitEvent('error', { error: { message: 'Nested error message' } });
      
      expect(result.current.error).toBe('Nested error message');
      expect(result.current.isLoading).toBe(false);
    });
    
    it('handles error event with direct message', () => {
      const { result } = renderHook(() => useInitializationStatus());
      
      emitEvent('error', { message: 'Direct error message' });
      
      expect(result.current.error).toBe('Direct error message');
      expect(result.current.isLoading).toBe(false);
    });
    
    it('handles error event with fallback message', () => {
      const { result } = renderHook(() => useInitializationStatus());
      
      emitEvent('error', {});
      
      expect(result.current.error).toBe('Connection error');
      expect(result.current.isLoading).toBe(false);
    });
  });
  
  describe('Initialization Event Tracking Tests', () => {
    it('listens to all 6 initialization events', () => {
      renderHook(() => useInitializationStatus());
      
      const initEvents = [
        'chat_user_data',
        'avatar_list',
        'voice_list',
        'agent_list',
        'tool_catalog',
        'chat-session-changed'
      ];
      
      // Verify each event is registered
      initEvents.forEach(event => {
        expect(mockClient.on).toHaveBeenCalledWith(
          event,
          expect.any(Function)
        );
      });
    });
    
    it('checks status on each init event', () => {
      mockClient.getConnectionState.mockReturnValue(ConnectionState.CONNECTED);
      mockClient.isFullyInitialized
        .mockReturnValueOnce(false) // Initial check
        .mockReturnValueOnce(false) // After first event
        .mockReturnValueOnce(true); // After second event
      
      const { result } = renderHook(() => useInitializationStatus());
      
      expect(result.current.isInitialized).toBe(false);
      
      // Emit init event
      emitEvent('chat_user_data');
      
      // Should check status
      expect(mockClient.getConnectionState).toHaveBeenCalled();
      expect(mockClient.isFullyInitialized).toHaveBeenCalled();
      
      // Emit another init event that triggers initialization
      emitEvent('voice_list');
      
      expect(result.current.isInitialized).toBe(true);
    });
  });
  
  describe('waitForInitialization Tests', () => {
    it('returns immediately if already initialized', async () => {
      mockClient.isFullyInitialized.mockReturnValue(true);
      
      const { result } = renderHook(() => useInitializationStatus());
      
      await act(async () => {
        await result.current.waitForInitialization();
      });
      
      expect(mockClient.isFullyInitialized).toHaveBeenCalled();
      expect(mockClient.waitForInitialization).not.toHaveBeenCalled();
    });
    
    it('waits for initialization if not ready', async () => {
      mockClient.isFullyInitialized.mockReturnValue(false);
      mockClient.waitForInitialization.mockResolvedValue(undefined);
      
      const { result } = renderHook(() => useInitializationStatus());
      
      await act(async () => {
        await result.current.waitForInitialization();
      });
      
      expect(mockClient.isFullyInitialized).toHaveBeenCalled();
      expect(mockClient.waitForInitialization).toHaveBeenCalled();
    });
    
    it('throws error if client not available', async () => {
      mockUseRealtimeClientSafe.mockReturnValue(null);
      
      const { result } = renderHook(() => useInitializationStatus());
      
      await expect(
        result.current.waitForInitialization()
      ).rejects.toThrow('Client not available');
    });
    
    it('propagates client waitForInitialization errors', async () => {
      mockClient.isFullyInitialized.mockReturnValue(false);
      mockClient.waitForInitialization.mockRejectedValue(new Error('Initialization failed'));
      
      const { result } = renderHook(() => useInitializationStatus());
      
      await expect(
        result.current.waitForInitialization()
      ).rejects.toThrow('Initialization failed');
    });
  });
  
  describe('Error Handling Tests', () => {
    it('handles checkStatus errors', () => {
      const consoleErrorSpy = vi.spyOn(console, 'error').mockImplementation(() => {});
      mockClient.getConnectionState.mockImplementation(() => {
        throw new Error('Connection check failed');
      });
      
      const { result } = renderHook(() => useInitializationStatus());
      
      expect(result.current.error).toBe('Connection check failed');
      expect(result.current.isLoading).toBe(false);
      expect(consoleErrorSpy).toHaveBeenCalledWith(
        'Failed to check initialization status:',
        expect.any(Error)
      );
      
      consoleErrorSpy.mockRestore();
    });
    
    it('handles non-Error objects in checkStatus', () => {
      const consoleErrorSpy = vi.spyOn(console, 'error').mockImplementation(() => {});
      mockClient.getConnectionState.mockImplementation(() => {
        throw 'String error';
      });
      
      const { result } = renderHook(() => useInitializationStatus());
      
      expect(result.current.error).toBe('Failed to check status');
      expect(result.current.isLoading).toBe(false);
      
      consoleErrorSpy.mockRestore();
    });
    
    it('clears error on successful connection', () => {
      const consoleErrorSpy = vi.spyOn(console, 'error').mockImplementation(() => {});
      
      // Start with an error
      mockClient.getConnectionState.mockImplementationOnce(() => {
        throw new Error('Initial error');
      });
      
      const { result } = renderHook(() => useInitializationStatus());
      
      expect(result.current.error).toBe('Initial error');
      
      // Fix the error and emit connected
      mockClient.getConnectionState.mockReturnValue(ConnectionState.CONNECTED);
      mockClient.isFullyInitialized.mockReturnValue(true);
      
      emitEvent('connected');
      
      expect(result.current.error).toBeNull();
      
      consoleErrorSpy.mockRestore();
    });
  });
  
  describe('Cleanup Tests', () => {
    it('unsubscribes all event listeners on unmount', () => {
      const { unmount } = renderHook(() => useInitializationStatus());
      
      unmount();
      
      // Verify main events are unsubscribed
      expect(mockClient.off).toHaveBeenCalledWith('initialized', expect.any(Function));
      expect(mockClient.off).toHaveBeenCalledWith('connected', expect.any(Function));
      expect(mockClient.off).toHaveBeenCalledWith('disconnected', expect.any(Function));
      expect(mockClient.off).toHaveBeenCalledWith('reconnecting', expect.any(Function));
      expect(mockClient.off).toHaveBeenCalledWith('error', expect.any(Function));
      
      // Verify init events are unsubscribed
      const initEvents = [
        'chat_user_data',
        'avatar_list',
        'voice_list',
        'agent_list',
        'tool_catalog',
        'chat-session-changed'
      ];
      
      initEvents.forEach(event => {
        expect(mockClient.off).toHaveBeenCalledWith(event, expect.any(Function));
      });
    });
    
    it('handles unmount when client becomes null', () => {
      const { rerender, unmount } = renderHook(() => useInitializationStatus());
      
      // Change client to null
      mockUseRealtimeClientSafe.mockReturnValue(null);
      rerender();
      
      // Should not cause errors on unmount
      expect(() => unmount()).not.toThrow();
    });
  });
  
  describe('Integration Tests', () => {
    it('handles full initialization flow', async () => {
      mockClient.getConnectionState
        .mockReturnValueOnce(ConnectionState.CONNECTING)  // Initial
        .mockReturnValueOnce(ConnectionState.CONNECTED)    // After connected event
        .mockReturnValueOnce(ConnectionState.CONNECTED);   // After init event
      
      mockClient.isFullyInitialized
        .mockReturnValueOnce(false)  // Initial
        .mockReturnValueOnce(false)  // After connected
        .mockReturnValueOnce(true);  // After init events
      
      const { result } = renderHook(() => useInitializationStatus());
      
      // Initially connecting
      expect(result.current.isLoading).toBe(true);
      expect(result.current.isInitialized).toBe(false);
      expect(result.current.connectionState).toBe(ConnectionState.CONNECTING);
      
      // Emit connected
      emitEvent('connected');
      
      await waitFor(() => {
        expect(result.current.connectionState).toBe(ConnectionState.CONNECTED);
        expect(result.current.isLoading).toBe(true); // Still loading
      });
      
      // Emit init event that completes initialization
      emitEvent('chat_user_data');
      
      await waitFor(() => {
        expect(result.current.isInitialized).toBe(true);
        expect(result.current.isLoading).toBe(false);
      });
    });
    
    it('handles reconnection flow', async () => {
      // Start initialized and connected
      mockClient.getConnectionState.mockReturnValue(ConnectionState.CONNECTED);
      mockClient.isFullyInitialized.mockReturnValue(true);
      
      const { result } = renderHook(() => useInitializationStatus());
      
      expect(result.current.isInitialized).toBe(true);
      expect(result.current.connectionState).toBe(ConnectionState.CONNECTED);
      
      // Disconnect
      emitEvent('disconnected');
      
      await waitFor(() => {
        expect(result.current.connectionState).toBe(ConnectionState.DISCONNECTED);
        expect(result.current.isInitialized).toBe(true); // Preserved
        expect(result.current.isLoading).toBe(false);
      });
      
      // Start reconnecting
      emitEvent('reconnecting');
      
      await waitFor(() => {
        expect(result.current.connectionState).toBe(ConnectionState.RECONNECTING);
        expect(result.current.isLoading).toBe(true);
      });
      
      // Reconnect successfully
      mockClient.getConnectionState.mockReturnValue(ConnectionState.CONNECTED);
      emitEvent('connected');
      
      await waitFor(() => {
        expect(result.current.connectionState).toBe(ConnectionState.CONNECTED);
        expect(result.current.isInitialized).toBe(true);
      });
    });
    
    it('updates state in response to multiple init events', () => {
      mockClient.getConnectionState.mockReturnValue(ConnectionState.CONNECTED);
      mockClient.isFullyInitialized
        .mockReturnValueOnce(false) // Initial
        .mockReturnValueOnce(false) // After first event
        .mockReturnValueOnce(false) // After second event
        .mockReturnValueOnce(true); // After third event
      
      const { result } = renderHook(() => useInitializationStatus());
      
      expect(result.current.isInitialized).toBe(false);
      expect(result.current.isLoading).toBe(true);
      
      // Emit series of init events
      emitEvent('chat_user_data');
      expect(mockClient.isFullyInitialized).toHaveBeenCalledTimes(2); // Initial + event
      
      emitEvent('voice_list');
      expect(mockClient.isFullyInitialized).toHaveBeenCalledTimes(3);
      
      emitEvent('agent_list');
      expect(mockClient.isFullyInitialized).toHaveBeenCalledTimes(4);
      
      // Now should be initialized
      expect(result.current.isInitialized).toBe(true);
      expect(result.current.isLoading).toBe(false);
    });
  });
  
  describe('Edge Cases', () => {
    it('handles rapid connection state changes', () => {
      const { result } = renderHook(() => useInitializationStatus());
      
      // Rapid state changes - only events that are actually handled
      emitEvent('connected');
      emitEvent('disconnected');
      emitEvent('reconnecting');
      
      // Mock the state after reconnecting event
      mockClient.getConnectionState.mockReturnValue(ConnectionState.CONNECTED);
      emitEvent('connected');
      
      // Should end up in final state
      expect(result.current.connectionState).toBe(ConnectionState.CONNECTED);
    });
    
    it('handles initialization during error state', () => {
      const { result } = renderHook(() => useInitializationStatus());
      
      // Start with error
      emitEvent('error', { message: 'Connection failed' });
      expect(result.current.error).toBe('Connection failed');
      
      // Then initialize successfully
      emitEvent('initialized');
      
      expect(result.current.isInitialized).toBe(true);
      expect(result.current.error).toBeNull(); // Error cleared
    });
    
    it('maintains stable references for waitForInitialization', () => {
      const { result, rerender } = renderHook(() => useInitializationStatus());
      
      const firstRef = result.current.waitForInitialization;
      
      // Rerender without changes
      rerender();
      
      const secondRef = result.current.waitForInitialization;
      
      // Function reference should be stable
      expect(firstRef).toBe(secondRef);
    });
  });
});
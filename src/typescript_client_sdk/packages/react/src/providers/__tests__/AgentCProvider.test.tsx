/**
 * Unit tests for AgentCProvider component
 * Testing initialization, event handling, token management, and cleanup
 */

import { describe, it, expect, vi, beforeEach, afterEach } from 'vitest';
import { render, waitFor, act } from '@testing-library/react';
import React, { useContext } from 'react';
import { AgentCProvider, AgentCProviderProps, withAgentCProvider } from '../AgentCProvider';
import { AgentCContext } from '../AgentCContext';
import { RealtimeClient } from '@agentc/realtime-core';
import type { 
  RealtimeClientConfig,
  AuthManager,
  ChatUserDataEvent,
  AgentListEvent,
  AvatarListEvent,
  VoiceListEvent,
  ToolCatalogEvent,
  ChatSessionChangedEvent
} from '@agentc/realtime-core';

// Store event handlers for manual triggering
const eventHandlers = new Map<string, Set<Function>>();

// Mock RealtimeClient
const mockClient = {
  on: vi.fn(),
  off: vi.fn(),
  connect: vi.fn(),
  disconnect: vi.fn(),
  destroy: vi.fn(),
  isConnected: vi.fn(),
  setAuthToken: vi.fn(),
  setAuthManager: vi.fn(),
  emit: vi.fn()
};

// Mock RealtimeClient constructor
vi.mock('@agentc/realtime-core', () => {
  return {
    RealtimeClient: vi.fn(() => {
      // Clear and set up event handlers for this instance
      eventHandlers.clear();
      
      // Mock the on method to store handlers
      mockClient.on = vi.fn((event: string, handler: Function) => {
        if (!eventHandlers.has(event)) {
          eventHandlers.set(event, new Set());
        }
        eventHandlers.get(event)!.add(handler);
        return mockClient;
      });
      
      // Mock the off method to remove handlers
      mockClient.off = vi.fn((event: string, handler: Function) => {
        eventHandlers.get(event)?.delete(handler);
        return mockClient;
      });
      
      return mockClient;
    }),
    ConnectionState: {
      DISCONNECTED: 'disconnected',
      CONNECTING: 'connecting',
      CONNECTED: 'connected',
      RECONNECTING: 'reconnecting'
    }
  };
});

// Helper to trigger events
const triggerEvent = (eventName: string, data: any) => {
  const handlers = eventHandlers.get(eventName);
  if (handlers) {
    handlers.forEach(handler => handler(data));
  }
};

// Test component to access context
const TestComponent = () => {
  const context = useContext(AgentCContext);
  return (
    <div data-testid="test-component">
      <div data-testid="client">{context?.client ? 'Client exists' : 'No client'}</div>
      <div data-testid="initializing">{context?.isInitializing ? 'true' : 'false'}</div>
      <div data-testid="error">{context?.error?.message || 'No error'}</div>
      <div data-testid="initialized">{context?.initialization.isInitialized ? 'true' : 'false'}</div>
      <div data-testid="events-count">{context?.initialization.receivedEvents.size || 0}</div>
    </div>
  );
};

describe('AgentCProvider', () => {
  const defaultProps: AgentCProviderProps = {
    children: <TestComponent />,
    apiUrl: 'wss://test.example.com/ws'
  };

  beforeEach(() => {
    vi.clearAllMocks();
    eventHandlers.clear();
    
    // Reset console mocks
    vi.spyOn(console, 'error').mockImplementation(() => {});
    vi.spyOn(console, 'warn').mockImplementation(() => {});
    
    // Set up default mock behaviors
    mockClient.connect.mockResolvedValue(undefined);
    mockClient.isConnected.mockReturnValue(false);
    
    // Clear environment variables
    delete process.env.REACT_APP_AGENTC_API_URL;
    delete process.env.NEXT_PUBLIC_AGENTC_API_URL;
  });

  afterEach(() => {
    vi.restoreAllMocks();
  });

  describe('Component Initialization', () => {
    it('creates RealtimeClient with provided config', () => {
      // Arrange
      const config: RealtimeClientConfig = {
        apiUrl: 'wss://test.example.com/ws',
        authToken: 'test-token',
        debug: true,
        autoReconnect: true,
        enableAudio: false
      };

      // Act
      render(
        <AgentCProvider config={config}>
          <TestComponent />
        </AgentCProvider>
      );

      // Assert
      expect(RealtimeClient).toHaveBeenCalledTimes(1);
      expect(RealtimeClient).toHaveBeenCalledWith(config);
    });

    it('builds config from individual props', () => {
      // Arrange
      const props: AgentCProviderProps = {
        children: <TestComponent />,
        apiUrl: 'wss://api.example.com/ws',
        authToken: 'my-token',
        debug: true
      };

      // Act
      render(<AgentCProvider {...props} />);

      // Assert
      expect(RealtimeClient).toHaveBeenCalledWith({
        apiUrl: 'wss://api.example.com/ws',
        authToken: 'my-token',
        authManager: undefined,
        debug: true,
        autoReconnect: true,
        enableAudio: false
      });
    });

    it('prevents double initialization in StrictMode', () => {
      // This test verifies that the initializationRef guard works
      // In real StrictMode, React may call effects twice, but our ref prevents double init
      // We test this by verifying that only one client is created per mount cycle
      
      // Arrange
      vi.mocked(RealtimeClient).mockClear();
      
      // Act - First render
      const { rerender } = render(<AgentCProvider {...defaultProps} />);
      const firstCallCount = vi.mocked(RealtimeClient).mock.calls.length;
      
      // Rerender with same props (simulates what might happen in StrictMode)
      rerender(<AgentCProvider {...defaultProps} />);
      const secondCallCount = vi.mocked(RealtimeClient).mock.calls.length;

      // Assert - Client should only be created once
      expect(firstCallCount).toBe(1);
      expect(secondCallCount).toBe(1); // Should still be 1, not 2
      expect(RealtimeClient).toHaveBeenCalledTimes(1);
    });

    it('handles missing apiUrl with environment fallback', () => {
      // Arrange - Set environment variable
      process.env.REACT_APP_AGENTC_API_URL = 'wss://env-api.example.com/ws';
      
      // Act
      render(
        <AgentCProvider>
          <TestComponent />
        </AgentCProvider>
      );

      // Assert
      expect(RealtimeClient).toHaveBeenCalledWith(
        expect.objectContaining({
          apiUrl: 'wss://env-api.example.com/ws'
        })
      );
    });

    it('falls back to NEXT_PUBLIC_AGENTC_API_URL when REACT_APP not set', () => {
      // Arrange
      process.env.NEXT_PUBLIC_AGENTC_API_URL = 'wss://next-api.example.com/ws';
      
      // Act
      render(
        <AgentCProvider>
          <TestComponent />
        </AgentCProvider>
      );

      // Assert
      expect(RealtimeClient).toHaveBeenCalledWith(
        expect.objectContaining({
          apiUrl: 'wss://next-api.example.com/ws'
        })
      );
    });

    it('shows error when no apiUrl is available', () => {
      // Arrange
      const onError = vi.fn();
      
      // Act
      const { getByTestId } = render(
        <AgentCProvider onError={onError}>
          <TestComponent />
        </AgentCProvider>
      );

      // Assert
      expect(console.error).toHaveBeenCalledWith(
        expect.stringContaining('apiUrl is required')
      );
      expect(onError).toHaveBeenCalledWith(
        expect.objectContaining({
          message: 'Invalid configuration: Missing required parameters'
        })
      );
      expect(getByTestId('error').textContent).toBe('Invalid configuration: Missing required parameters');
      expect(getByTestId('initializing').textContent).toBe('false');
    });
  });

  describe('Event Listener Tests', () => {
    it('subscribes to all 6 initialization events', () => {
      // Act
      render(<AgentCProvider {...defaultProps} />);

      // Assert
      expect(mockClient.on).toHaveBeenCalledWith('chat_user_data', expect.any(Function));
      expect(mockClient.on).toHaveBeenCalledWith('agent_list', expect.any(Function));
      expect(mockClient.on).toHaveBeenCalledWith('avatar_list', expect.any(Function));
      expect(mockClient.on).toHaveBeenCalledWith('voice_list', expect.any(Function));
      expect(mockClient.on).toHaveBeenCalledWith('tool_catalog', expect.any(Function));
      expect(mockClient.on).toHaveBeenCalledWith('chat_session_changed', expect.any(Function));
      
      // Should also subscribe to debug events if debug is true
      const debugEventCount = mockClient.on.mock.calls.filter(
        call => ['connected', 'disconnected', 'error'].includes(call[0])
      ).length;
      expect(debugEventCount).toBe(0); // No debug events when debug=false
    });

    it('subscribes to debug events when debug=true', () => {
      // Act
      render(<AgentCProvider {...defaultProps} debug={true} />);

      // Assert - Should have 6 init events + 3 debug events
      expect(mockClient.on).toHaveBeenCalledWith('connected', expect.any(Function));
      expect(mockClient.on).toHaveBeenCalledWith('disconnected', expect.any(Function));
      expect(mockClient.on).toHaveBeenCalledWith('error', expect.any(Function));
    });

    it('updates initialization state for each event', () => {
      // Arrange
      const { getByTestId } = render(<AgentCProvider {...defaultProps} />);
      
      // Initially not initialized
      expect(getByTestId('initialized').textContent).toBe('false');
      expect(getByTestId('events-count').textContent).toBe('0');

      // Act & Assert - Trigger each event and verify state updates
      act(() => {
        triggerEvent('chat_user_data', { 
          user: { id: 'user1', name: 'Test User' } 
        } as ChatUserDataEvent);
      });
      expect(getByTestId('events-count').textContent).toBe('1');
      expect(getByTestId('initialized').textContent).toBe('false');

      act(() => {
        triggerEvent('agent_list', { 
          agents: [{ id: 'agent1', name: 'Agent 1' }] 
        } as AgentListEvent);
      });
      expect(getByTestId('events-count').textContent).toBe('2');
      expect(getByTestId('initialized').textContent).toBe('false');

      act(() => {
        triggerEvent('avatar_list', { 
          avatars: [{ id: 'avatar1', name: 'Avatar 1' }] 
        } as AvatarListEvent);
      });
      expect(getByTestId('events-count').textContent).toBe('3');
      expect(getByTestId('initialized').textContent).toBe('false');

      act(() => {
        triggerEvent('voice_list', { 
          voices: [{ id: 'voice1', name: 'Voice 1' }] 
        } as VoiceListEvent);
      });
      expect(getByTestId('events-count').textContent).toBe('4');
      expect(getByTestId('initialized').textContent).toBe('false');

      act(() => {
        triggerEvent('tool_catalog', { 
          tools: [{ id: 'tool1', name: 'Tool 1' }] 
        } as ToolCatalogEvent);
      });
      expect(getByTestId('events-count').textContent).toBe('5');
      expect(getByTestId('initialized').textContent).toBe('false');

      // Last event should mark as initialized
      act(() => {
        triggerEvent('chat_session_changed', { 
          chat_session: { id: 'session1', agent_id: 'agent1' } 
        } as ChatSessionChangedEvent);
      });
      expect(getByTestId('events-count').textContent).toBe('6');
      expect(getByTestId('initialized').textContent).toBe('true');
    });

    it('marks isInitialized true when all 6 events received', () => {
      // Arrange
      const onInitializationComplete = vi.fn();
      const { getByTestId } = render(
        <AgentCProvider {...defaultProps} onInitializationComplete={onInitializationComplete} />
      );

      // Act - Emit all events
      act(() => {
        triggerEvent('chat_user_data', { user: { id: 'user1' } });
        triggerEvent('agent_list', { agents: [] });
        triggerEvent('avatar_list', { avatars: [] });
        triggerEvent('voice_list', { voices: [] });
        triggerEvent('tool_catalog', { tools: [] });
        triggerEvent('chat_session_changed', { chat_session: null });
      });

      // Assert
      expect(getByTestId('initialized').textContent).toBe('true');
      expect(onInitializationComplete).toHaveBeenCalledTimes(1);
      expect(onInitializationComplete).toHaveBeenCalledWith(
        expect.objectContaining({
          isInitialized: true,
          receivedEvents: expect.any(Set)
        })
      );
    });

    it('does not mark initialized if only some events received', () => {
      // Arrange
      const onInitializationComplete = vi.fn();
      const { getByTestId } = render(
        <AgentCProvider {...defaultProps} onInitializationComplete={onInitializationComplete} />
      );

      // Act - Emit only 3 of 6 events
      act(() => {
        triggerEvent('chat_user_data', { user: { id: 'user1' } });
        triggerEvent('agent_list', { agents: [] });
        triggerEvent('voice_list', { voices: [] });
      });

      // Assert
      expect(getByTestId('initialized').textContent).toBe('false');
      expect(getByTestId('events-count').textContent).toBe('3');
      expect(onInitializationComplete).not.toHaveBeenCalled();
    });
  });

  describe('Auto-Connect Tests', () => {
    it('auto-connects when autoConnect prop is true', async () => {
      // Arrange
      mockClient.connect.mockResolvedValue(undefined);
      
      // Act
      render(<AgentCProvider {...defaultProps} autoConnect={true} />);

      // Assert
      await waitFor(() => {
        expect(mockClient.connect).toHaveBeenCalledTimes(1);
      });
    });

    it('does not auto-connect when autoConnect prop is false', () => {
      // Act
      render(<AgentCProvider {...defaultProps} autoConnect={false} />);

      // Assert
      expect(mockClient.connect).not.toHaveBeenCalled();
    });

    it('handles auto-connect failure', async () => {
      // Arrange
      const connectError = new Error('Connection failed');
      mockClient.connect.mockRejectedValue(connectError);
      const onError = vi.fn();
      
      // Act
      const { getByTestId } = render(
        <AgentCProvider {...defaultProps} autoConnect={true} onError={onError} />
      );

      // Assert
      await waitFor(() => {
        expect(mockClient.connect).toHaveBeenCalledTimes(1);
        expect(console.error).toHaveBeenCalledWith(
          'AgentCProvider: Auto-connect failed',
          connectError
        );
        expect(onError).toHaveBeenCalledWith(
          expect.objectContaining({
            message: 'Failed to auto-connect: Connection failed'
          })
        );
        expect(getByTestId('error').textContent).toBe('Failed to auto-connect: Connection failed');
      });
    });
  });

  describe('Token Management Tests', () => {
    it('updates auth token when prop changes', () => {
      // Arrange
      const { rerender } = render(<AgentCProvider {...defaultProps} authToken="token1" />);
      
      // Clear initial calls
      mockClient.setAuthToken.mockClear();
      
      // Act - Update token
      rerender(<AgentCProvider {...defaultProps} authToken="token2" />);

      // Assert
      expect(mockClient.setAuthToken).toHaveBeenCalledTimes(1);
      expect(mockClient.setAuthToken).toHaveBeenCalledWith('token2');
    });

    it('does not update token when authManager is present', () => {
      // Arrange
      const authManager = { getToken: vi.fn() } as unknown as AuthManager;
      const { rerender } = render(
        <AgentCProvider {...defaultProps} authToken="token1" authManager={authManager} />
      );
      
      // Clear initial calls
      mockClient.setAuthToken.mockClear();
      
      // Act - Update token (should be ignored because authManager is present)
      rerender(
        <AgentCProvider {...defaultProps} authToken="token2" authManager={authManager} />
      );

      // Assert
      expect(mockClient.setAuthToken).not.toHaveBeenCalled();
    });

    it('updates auth manager when prop changes', () => {
      // Arrange
      const authManager1 = { getToken: vi.fn() } as unknown as AuthManager;
      const authManager2 = { getToken: vi.fn() } as unknown as AuthManager;
      
      const { rerender } = render(
        <AgentCProvider {...defaultProps} authManager={authManager1} />
      );
      
      // Clear initial calls
      mockClient.setAuthManager.mockClear();
      
      // Act - Update authManager
      rerender(<AgentCProvider {...defaultProps} authManager={authManager2} />);

      // Assert
      expect(mockClient.setAuthManager).toHaveBeenCalledTimes(1);
      expect(mockClient.setAuthManager).toHaveBeenCalledWith(authManager2);
    });
  });

  describe('Cleanup Tests', () => {
    it('disconnects and destroys client on unmount', () => {
      // Arrange
      mockClient.isConnected.mockReturnValue(true);
      
      const { unmount } = render(<AgentCProvider {...defaultProps} />);

      // Act
      unmount();

      // Assert
      expect(mockClient.isConnected).toHaveBeenCalled();
      expect(mockClient.disconnect).toHaveBeenCalledTimes(1);
      expect(mockClient.destroy).toHaveBeenCalledTimes(1);
    });

    it('does not call disconnect if not connected', () => {
      // Arrange
      mockClient.isConnected.mockReturnValue(false);
      
      const { unmount } = render(<AgentCProvider {...defaultProps} />);

      // Act
      unmount();

      // Assert
      expect(mockClient.isConnected).toHaveBeenCalled();
      expect(mockClient.disconnect).not.toHaveBeenCalled();
      expect(mockClient.destroy).toHaveBeenCalledTimes(1);
    });

    it('resets initialization flag on unmount for re-mounting', () => {
      // Arrange
      vi.mocked(RealtimeClient).mockClear();
      
      // First mount
      const { unmount } = render(<AgentCProvider {...defaultProps} />);
      expect(RealtimeClient).toHaveBeenCalledTimes(1);
      
      // Unmount
      unmount();
      
      // Clear mock to count fresh
      vi.mocked(RealtimeClient).mockClear();
      
      // Act - Re-mount should create new client
      render(<AgentCProvider {...defaultProps} />);

      // Assert
      expect(RealtimeClient).toHaveBeenCalledTimes(1);
    });

    it('removes all event listeners with correct handler references to prevent memory leaks', () => {
      // This test verifies the memory leak fix where event listeners weren't being
      // properly removed because the handler references weren't being stored
      
      // Arrange
      const { unmount } = render(<AgentCProvider {...defaultProps} debug={true} />);
      
      // Store the handler references that were registered
      const registeredHandlers = new Map<string, Function>();
      
      // Capture all the on() calls to record handler references
      mockClient.on.mock.calls.forEach(([event, handler]) => {
        registeredHandlers.set(event, handler);
      });
      
      // Should have registered 6 init events + 3 debug events = 9 total
      expect(registeredHandlers.size).toBe(9);
      expect(registeredHandlers.has('chat_user_data')).toBe(true);
      expect(registeredHandlers.has('agent_list')).toBe(true);
      expect(registeredHandlers.has('avatar_list')).toBe(true);
      expect(registeredHandlers.has('voice_list')).toBe(true);
      expect(registeredHandlers.has('tool_catalog')).toBe(true);
      expect(registeredHandlers.has('chat_session_changed')).toBe(true);
      expect(registeredHandlers.has('connected')).toBe(true);
      expect(registeredHandlers.has('disconnected')).toBe(true);
      expect(registeredHandlers.has('error')).toBe(true);
      
      // Clear the mock to count only the off() calls during unmount
      mockClient.off.mockClear();
      
      // Act - Unmount which should trigger cleanup
      unmount();
      
      // Assert - All event listeners should be removed with the EXACT same handler references
      expect(mockClient.off).toHaveBeenCalledTimes(9);
      
      // Verify each listener was removed with the exact same handler reference that was registered
      expect(mockClient.off).toHaveBeenCalledWith('chat_user_data', registeredHandlers.get('chat_user_data'));
      expect(mockClient.off).toHaveBeenCalledWith('agent_list', registeredHandlers.get('agent_list'));
      expect(mockClient.off).toHaveBeenCalledWith('avatar_list', registeredHandlers.get('avatar_list'));
      expect(mockClient.off).toHaveBeenCalledWith('voice_list', registeredHandlers.get('voice_list'));
      expect(mockClient.off).toHaveBeenCalledWith('tool_catalog', registeredHandlers.get('tool_catalog'));
      expect(mockClient.off).toHaveBeenCalledWith('chat_session_changed', registeredHandlers.get('chat_session_changed'));
      expect(mockClient.off).toHaveBeenCalledWith('connected', registeredHandlers.get('connected'));
      expect(mockClient.off).toHaveBeenCalledWith('disconnected', registeredHandlers.get('disconnected'));
      expect(mockClient.off).toHaveBeenCalledWith('error', registeredHandlers.get('error'));
      
      // Verify cleanup happens BEFORE destroy
      const offCallOrder = mockClient.off.mock.invocationCallOrder[0];
      const destroyCallOrder = mockClient.destroy.mock.invocationCallOrder[0];
      expect(offCallOrder).toBeLessThan(destroyCallOrder);
    });

    it('removes only registered event listeners when debug is false', () => {
      // Verify that when debug=false, only the 6 init events are registered and removed
      
      // Arrange
      const { unmount } = render(<AgentCProvider {...defaultProps} debug={false} />);
      
      // Store the handler references that were registered
      const registeredHandlers = new Map<string, Function>();
      
      // Capture all the on() calls to record handler references
      mockClient.on.mock.calls.forEach(([event, handler]) => {
        registeredHandlers.set(event, handler);
      });
      
      // Should have registered only 6 init events (no debug events)
      expect(registeredHandlers.size).toBe(6);
      expect(registeredHandlers.has('connected')).toBe(false);
      expect(registeredHandlers.has('disconnected')).toBe(false);
      expect(registeredHandlers.has('error')).toBe(false);
      
      // Clear the mock to count only the off() calls during unmount
      mockClient.off.mockClear();
      
      // Act - Unmount which should trigger cleanup
      unmount();
      
      // Assert - Only 6 event listeners should be removed
      expect(mockClient.off).toHaveBeenCalledTimes(6);
      
      // Verify each listener was removed with the exact same handler reference
      expect(mockClient.off).toHaveBeenCalledWith('chat_user_data', registeredHandlers.get('chat_user_data'));
      expect(mockClient.off).toHaveBeenCalledWith('agent_list', registeredHandlers.get('agent_list'));
      expect(mockClient.off).toHaveBeenCalledWith('avatar_list', registeredHandlers.get('avatar_list'));
      expect(mockClient.off).toHaveBeenCalledWith('voice_list', registeredHandlers.get('voice_list'));
      expect(mockClient.off).toHaveBeenCalledWith('tool_catalog', registeredHandlers.get('tool_catalog'));
      expect(mockClient.off).toHaveBeenCalledWith('chat_session_changed', registeredHandlers.get('chat_session_changed'));
      
      // Should NOT attempt to remove debug event listeners that were never registered
      expect(mockClient.off).not.toHaveBeenCalledWith('connected', expect.any(Function));
      expect(mockClient.off).not.toHaveBeenCalledWith('disconnected', expect.any(Function));
      expect(mockClient.off).not.toHaveBeenCalledWith('error', expect.any(Function));
    });

    it('prevents event listener accumulation across multiple mount/unmount cycles', () => {
      // This test simulates the scenario that would have caused the memory leak warning:
      // "Warning: Possible EventEmitter memory leak detected. 11 'connected' listeners added"
      // The fix ensures listeners are properly removed and don't accumulate
      
      // Track how many unique listener handlers are created
      const uniqueHandlers = new Set<Function>();
      
      // Perform multiple mount/unmount cycles
      const cycles = 5;
      
      for (let i = 0; i < cycles; i++) {
        // Clear previous mock calls but preserve the mock function
        mockClient.on.mockClear();
        mockClient.off.mockClear();
        
        // Mount
        const { unmount } = render(<AgentCProvider {...defaultProps} debug={true} />);
        
        // Collect the handlers that were registered in this cycle
        const handlersThisCycle = new Map<string, Function>();
        mockClient.on.mock.calls.forEach(([event, handler]) => {
          handlersThisCycle.set(event, handler);
          uniqueHandlers.add(handler);
        });
        
        // Should register exactly 9 listeners (6 init + 3 debug)
        expect(mockClient.on).toHaveBeenCalledTimes(9);
        expect(handlersThisCycle.size).toBe(9);
        
        // Unmount
        unmount();
        
        // Should remove exactly the same 9 listeners with the same handler references
        expect(mockClient.off).toHaveBeenCalledTimes(9);
        
        // Verify each handler was removed with the exact same reference
        handlersThisCycle.forEach((handler, event) => {
          expect(mockClient.off).toHaveBeenCalledWith(event, handler);
        });
      }
      
      // After all cycles, we should have created exactly cycles * 9 unique handlers
      // This proves new handlers are created for each mount (no reuse of stale references)
      expect(uniqueHandlers.size).toBe(cycles * 9);
      
      // If the memory leak bug existed, handlers wouldn't be properly removed
      // and we'd accumulate listeners on the client
    });
  });

  describe('Debug Mode Tests', () => {
    it('logs debug messages when debug=true', () => {
      // Act
      render(<AgentCProvider {...defaultProps} debug={true} />);

      // Assert
      expect(console.warn).toHaveBeenCalledWith(
        'AgentCProvider: Initializing RealtimeClient with config:',
        expect.objectContaining({
          apiUrl: 'wss://test.example.com/ws',
          authToken: undefined
        })
      );
    });

    it('redacts authToken in debug logs', () => {
      // Act
      render(
        <AgentCProvider {...defaultProps} authToken="secret-token" debug={true} />
      );

      // Assert
      expect(console.warn).toHaveBeenCalledWith(
        'AgentCProvider: Initializing RealtimeClient with config:',
        expect.objectContaining({
          apiUrl: 'wss://test.example.com/ws',
          authToken: '[REDACTED]'
        })
      );
    });

    it('logs initialization events when debug=true', () => {
      // Arrange
      render(<AgentCProvider {...defaultProps} debug={true} />);
      
      // Clear initial logs
      vi.mocked(console.warn).mockClear();
      
      // Act
      act(() => {
        triggerEvent('chat_user_data', { user: { id: 'user1' } });
      });

      // Assert
      expect(console.warn).toHaveBeenCalledWith(
        'AgentCProvider: Received chat_user_data event'
      );
    });

    it('logs when all initialization events received in debug mode', () => {
      // Arrange
      render(<AgentCProvider {...defaultProps} debug={true} />);
      
      // Clear initial logs
      vi.mocked(console.warn).mockClear();
      
      // Act - Emit all events
      act(() => {
        triggerEvent('chat_user_data', { user: { id: 'user1' } });
        triggerEvent('agent_list', { agents: [] });
        triggerEvent('avatar_list', { avatars: [] });
        triggerEvent('voice_list', { voices: [] });
        triggerEvent('tool_catalog', { tools: [] });
        triggerEvent('chat_session_changed', { chat_session: null });
      });

      // Assert
      expect(console.warn).toHaveBeenCalledWith(
        'AgentCProvider: All initialization events received'
      );
    });
  });

  describe('Context Value Tests', () => {
    it('provides correct context value', () => {
      // Arrange
      const { getByTestId } = render(<AgentCProvider {...defaultProps} />);

      // Assert
      expect(getByTestId('client').textContent).toBe('Client exists');
      expect(getByTestId('initializing').textContent).toBe('false');
      expect(getByTestId('error').textContent).toBe('No error');
      expect(getByTestId('initialized').textContent).toBe('false');
    });

    it('updates context value when state changes', () => {
      // Arrange
      const { getByTestId } = render(<AgentCProvider {...defaultProps} />);
      
      // Initial state
      expect(getByTestId('initialized').textContent).toBe('false');
      
      // Act - Trigger all initialization events
      act(() => {
        triggerEvent('chat_user_data', { user: { id: 'user1' } });
        triggerEvent('agent_list', { agents: [] });
        triggerEvent('avatar_list', { avatars: [] });
        triggerEvent('voice_list', { voices: [] });
        triggerEvent('tool_catalog', { tools: [] });
        triggerEvent('chat_session_changed', { chat_session: null });
      });

      // Assert - Context should be updated
      expect(getByTestId('initialized').textContent).toBe('true');
      expect(getByTestId('events-count').textContent).toBe('6');
    });
  });

  describe('withAgentCProvider HOC Tests', () => {
    it('wraps component with provider', () => {
      // Arrange
      const TestComponentForHOC = () => {
        const context = useContext(AgentCContext);
        return <div>{context?.client ? 'Has context' : 'No context'}</div>;
      };
      
      const WrappedComponent = withAgentCProvider(TestComponentForHOC, {
        apiUrl: 'wss://test.example.com/ws'
      });

      // Act
      const { getByText } = render(<WrappedComponent />);

      // Assert
      expect(getByText('Has context')).toBeInTheDocument();
    });

    it('passes props through to wrapped component', () => {
      // Arrange
      interface TestProps {
        message: string;
      }
      
      const TestComponentForHOC = ({ message }: TestProps) => {
        const context = useContext(AgentCContext);
        return (
          <div>
            <span>{message}</span>
            <span>{context?.client ? 'Has context' : 'No context'}</span>
          </div>
        );
      };
      
      const WrappedComponent = withAgentCProvider(TestComponentForHOC, {
        apiUrl: 'wss://test.example.com/ws'
      });

      // Act
      const { getByText } = render(<WrappedComponent message="Hello World" />);

      // Assert
      expect(getByText('Hello World')).toBeInTheDocument();
      expect(getByText('Has context')).toBeInTheDocument();
    });
  });

  describe('Error Handling', () => {
    it('handles initialization errors gracefully', () => {
      // Arrange
      const initError = new Error('Init failed');
      vi.mocked(RealtimeClient).mockImplementationOnce(() => {
        throw initError;
      });
      
      const onError = vi.fn();
      
      // Act
      const { getByTestId } = render(
        <AgentCProvider {...defaultProps} onError={onError} />
      );

      // Assert
      expect(console.error).toHaveBeenCalledWith(
        'AgentCProvider: Initialization failed',
        initError
      );
      expect(onError).toHaveBeenCalledWith(initError);
      expect(getByTestId('error').textContent).toBe('Init failed');
      expect(getByTestId('initializing').textContent).toBe('false');
    });

    it('handles non-Error thrown values', () => {
      // Arrange
      vi.mocked(RealtimeClient).mockImplementationOnce(() => {
        throw 'String error';
      });
      
      const onError = vi.fn();
      
      // Act
      const { getByTestId } = render(
        <AgentCProvider {...defaultProps} onError={onError} />
      );

      // Assert
      expect(getByTestId('error').textContent).toBe('Failed to initialize RealtimeClient');
      expect(onError).toHaveBeenCalledWith(
        expect.objectContaining({
          message: 'Failed to initialize RealtimeClient'
        })
      );
    });
  });

  describe('Callback Tests', () => {
    it('calls onInitialized when client is created', () => {
      // Arrange
      const onInitialized = vi.fn();
      
      // Act
      render(<AgentCProvider {...defaultProps} onInitialized={onInitialized} />);

      // Assert
      expect(onInitialized).toHaveBeenCalledTimes(1);
      expect(onInitialized).toHaveBeenCalledWith(mockClient);
    });

    it('does not call onInitialized on error', () => {
      // Arrange
      const onInitialized = vi.fn();
      vi.mocked(RealtimeClient).mockImplementationOnce(() => {
        throw new Error('Init failed');
      });
      
      // Act
      render(<AgentCProvider {...defaultProps} onInitialized={onInitialized} />);

      // Assert
      expect(onInitialized).not.toHaveBeenCalled();
    });
  });
});
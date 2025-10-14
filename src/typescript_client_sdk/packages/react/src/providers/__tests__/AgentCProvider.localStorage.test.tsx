/**
 * Tests for AgentCProvider localStorage integration
 * Verifies that saved agent preferences are loaded and applied
 */

import React from 'react';
import { render, waitFor, act } from '@testing-library/react';
import { describe, it, expect, vi, beforeEach, afterEach } from 'vitest';
import { AgentCProvider } from '../AgentCProvider';
import { RealtimeClient } from '@agentc/realtime-core';
import { AgentStorage } from '../../utils/agentStorage';

// Mock the RealtimeClient
vi.mock('@agentc/realtime-core', () => ({
  RealtimeClient: vi.fn().mockImplementation(() => ({
    on: vi.fn(),
    off: vi.fn(),
    connect: vi.fn().mockResolvedValue(undefined),
    disconnect: vi.fn(),
    destroy: vi.fn(),
    isConnected: vi.fn().mockReturnValue(false),
    setAuthToken: vi.fn(),
    setAuthManager: vi.fn(),
    setPreferredAgentKey: vi.fn(),
  })),
  ConnectionState: {
    DISCONNECTED: 0,
    CONNECTING: 1,
    CONNECTED: 2,
    RECONNECTING: 3
  }
}));

// Mock localStorage
const localStorageMock = (() => {
  let store: Record<string, string> = {};
  
  return {
    getItem: vi.fn((key: string) => store[key] || null),
    setItem: vi.fn((key: string, value: string) => {
      store[key] = value.toString();
    }),
    removeItem: vi.fn((key: string) => {
      delete store[key];
    }),
    clear: vi.fn(() => {
      store = {};
    }),
    get length() {
      return Object.keys(store).length;
    },
    key: vi.fn((index: number) => {
      const keys = Object.keys(store);
      return keys[index] || null;
    })
  };
})();

Object.defineProperty(window, 'localStorage', {
  value: localStorageMock,
  writable: true
});

describe('AgentCProvider - localStorage Integration', () => {
  let mockClient: any;
  
  beforeEach(() => {
    // Clear mocks and localStorage
    vi.clearAllMocks();
    localStorageMock.clear();
    
    // Reset the mock implementation for each test
    mockClient = {
      on: vi.fn(),
      off: vi.fn(),
      connect: vi.fn().mockResolvedValue(undefined),
      disconnect: vi.fn(),
      destroy: vi.fn(),
      isConnected: vi.fn().mockReturnValue(false),
      setAuthToken: vi.fn(),
      setAuthManager: vi.fn(),
      setPreferredAgentKey: vi.fn(),
    };
    
    (RealtimeClient as any).mockImplementation(() => mockClient);
  });
  
  afterEach(() => {
    vi.clearAllMocks();
  });
  
  it('should load saved agent key from localStorage and set it on the client', async () => {
    // Save an agent key to localStorage
    const savedAgentKey = 'test-agent-123';
    AgentStorage.saveAgentKey(savedAgentKey);
    
    // Verify it was saved
    expect(localStorage.getItem('agentc_selected_agent_key')).toBe(savedAgentKey);
    
    // Create the provider
    const { unmount } = render(
      <AgentCProvider
        apiUrl="wss://api.example.com"
        authToken="test-token"
      >
        <div>Test Child</div>
      </AgentCProvider>
    );
    
    // Wait for initialization
    await waitFor(() => {
      // Verify RealtimeClient was created
      expect(RealtimeClient).toHaveBeenCalled();
    });
    
    // Verify setPreferredAgentKey was called with the saved agent key
    expect(mockClient.setPreferredAgentKey).toHaveBeenCalledWith(savedAgentKey);
    expect(mockClient.setPreferredAgentKey).toHaveBeenCalledTimes(1);
    
    // Cleanup
    unmount();
  });
  
  it('should not call setPreferredAgentKey when no saved agent key exists', async () => {
    // Ensure localStorage is empty
    expect(localStorage.getItem('agentc_selected_agent_key')).toBeNull();
    
    // Create the provider
    const { unmount } = render(
      <AgentCProvider
        apiUrl="wss://api.example.com"
        authToken="test-token"
      >
        <div>Test Child</div>
      </AgentCProvider>
    );
    
    // Wait for initialization
    await waitFor(() => {
      // Verify RealtimeClient was created
      expect(RealtimeClient).toHaveBeenCalled();
    });
    
    // Verify setPreferredAgentKey was NOT called
    expect(mockClient.setPreferredAgentKey).not.toHaveBeenCalled();
    
    // Cleanup
    unmount();
  });
  
  it('should handle localStorage access errors gracefully', async () => {
    // Mock localStorage.getItem to throw an error
    const originalGetItem = localStorage.getItem;
    localStorage.getItem = vi.fn().mockImplementation(() => {
      throw new Error('localStorage access denied');
    });
    
    // Spy on console.warn
    const consoleWarnSpy = vi.spyOn(console, 'warn').mockImplementation(() => {});
    
    // Create the provider
    const { unmount } = render(
      <AgentCProvider
        apiUrl="wss://api.example.com"
        authToken="test-token"
      >
        <div>Test Child</div>
      </AgentCProvider>
    );
    
    // Wait for initialization
    await waitFor(() => {
      // Verify RealtimeClient was created
      expect(RealtimeClient).toHaveBeenCalled();
    });
    
    // Verify warning was logged by AgentStorage.getAgentKey()
    expect(consoleWarnSpy).toHaveBeenCalledWith(
      'Failed to retrieve agent key from localStorage:',
      expect.any(Error)
    );
    
    // Verify setPreferredAgentKey was NOT called (error was handled gracefully)
    expect(mockClient.setPreferredAgentKey).not.toHaveBeenCalled();
    
    // Restore original localStorage.getItem
    localStorage.getItem = originalGetItem;
    consoleWarnSpy.mockRestore();
    
    // Cleanup
    unmount();
  });
  
  it('should log debug message when loading saved agent key with debug enabled', async () => {
    // Save an agent key to localStorage
    const savedAgentKey = 'debug-test-agent';
    AgentStorage.saveAgentKey(savedAgentKey);
    
    // Spy on console.warn (used for debug messages)
    const consoleWarnSpy = vi.spyOn(console, 'warn').mockImplementation(() => {});
    
    // Create the provider with debug enabled
    const { unmount } = render(
      <AgentCProvider
        apiUrl="wss://api.example.com"
        authToken="test-token"
        debug={true}
      >
        <div>Test Child</div>
      </AgentCProvider>
    );
    
    // Wait for initialization and setPreferredAgentKey to be called
    await waitFor(() => {
      // Verify RealtimeClient was created
      expect(RealtimeClient).toHaveBeenCalled();
      // Verify setPreferredAgentKey was called
      expect(mockClient.setPreferredAgentKey).toHaveBeenCalledWith(savedAgentKey);
    });
    
    // Verify debug message was logged with the correct format
    expect(consoleWarnSpy).toHaveBeenCalledWith(
      'AgentCProvider: Set preferred agent from localStorage:',
      savedAgentKey
    );
    
    consoleWarnSpy.mockRestore();
    
    // Cleanup
    unmount();
  });
  
  it('should work with autoConnect when agent key is loaded', async () => {
    // Save an agent key to localStorage
    const savedAgentKey = 'auto-connect-agent';
    AgentStorage.saveAgentKey(savedAgentKey);
    
    // Verify it was saved correctly
    expect(localStorage.getItem('agentc_selected_agent_key')).toBe(savedAgentKey);
    
    // Create the provider with autoConnect
    const { unmount } = render(
      <AgentCProvider
        apiUrl="wss://api.example.com"
        authToken="test-token"
        autoConnect={true}
      >
        <div>Test Child</div>
      </AgentCProvider>
    );
    
    // Wait for initialization and connection
    await waitFor(() => {
      // Verify RealtimeClient was created
      expect(RealtimeClient).toHaveBeenCalled();
      
      // Verify setPreferredAgentKey was called with the saved agent key
      expect(mockClient.setPreferredAgentKey).toHaveBeenCalledWith(savedAgentKey);
      
      // Verify connect was called
      expect(mockClient.connect).toHaveBeenCalled();
    }, { timeout: 2000 });
    
    // Verify order of operations: setPreferredAgentKey before connect
    const setAgentCallOrder = mockClient.setPreferredAgentKey.mock.invocationCallOrder[0];
    const connectCallOrder = mockClient.connect.mock.invocationCallOrder[0];
    expect(setAgentCallOrder).toBeLessThan(connectCallOrder);
    
    // Cleanup
    unmount();
  });
  
  it('should handle empty string agent key from localStorage', async () => {
    // Save an empty agent key to localStorage (shouldn't happen but let's be defensive)
    localStorage.setItem('agentc_selected_agent_key', '');
    
    // Create the provider
    const { unmount } = render(
      <AgentCProvider
        apiUrl="wss://api.example.com"
        authToken="test-token"
      >
        <div>Test Child</div>
      </AgentCProvider>
    );
    
    // Wait for initialization
    await waitFor(() => {
      // Verify RealtimeClient was created
      expect(RealtimeClient).toHaveBeenCalled();
    });
    
    // Verify setPreferredAgentKey was NOT called for empty string
    expect(mockClient.setPreferredAgentKey).not.toHaveBeenCalled();
    
    // Cleanup
    unmount();
  });
});
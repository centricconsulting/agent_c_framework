/**
 * Tests for AgentCProvider StrictMode behavior
 * Ensures no duplicate clients are created during React StrictMode's double-invoke
 */

import { describe, it, expect, vi, beforeEach, afterEach } from 'vitest';
import { render, waitFor } from '@testing-library/react';
import { StrictMode } from 'react';
import { AgentCProvider } from '../AgentCProvider';
import { RealtimeClient } from '@agentc/realtime-core';

// Mock RealtimeClient
vi.mock('@agentc/realtime-core', () => {
  const mockClientInstances: any[] = [];
  
  return {
    RealtimeClient: vi.fn().mockImplementation((config) => {
      const mockClient = {
        config,
        connected: false,
        destroyed: false,
        on: vi.fn(),
        off: vi.fn(),
        connect: vi.fn().mockResolvedValue(undefined),
        disconnect: vi.fn(),
        destroy: vi.fn(function(this: any) {
          this.destroyed = true;
        }),
        isConnected: vi.fn(function(this: any) {
          return this.connected;
        }),
        setAuthToken: vi.fn(),
        setAuthManager: vi.fn(),
        setPreferredAgentKey: vi.fn()
      };
      mockClientInstances.push(mockClient);
      return mockClient;
    }),
    // Expose instances for testing
    __getMockClientInstances: () => mockClientInstances,
    __clearMockClientInstances: () => {
      mockClientInstances.length = 0;
    }
  };
});

describe('AgentCProvider - StrictMode Behavior', () => {
  beforeEach(async () => {
    // Clear any previous instances
    const { __clearMockClientInstances } = vi.mocked(
      await import('@agentc/realtime-core')
    );
    __clearMockClientInstances();
  });

  afterEach(() => {
    vi.clearAllMocks();
  });

  it('should create only one client instance in StrictMode', async () => {
    const coreModule = await import('@agentc/realtime-core');
    const { __getMockClientInstances } = vi.mocked(coreModule);

    render(
      <StrictMode>
        <AgentCProvider apiUrl="wss://test.example.com">
          <div>Test Child</div>
        </AgentCProvider>
      </StrictMode>
    );

    await waitFor(() => {
      const instances = __getMockClientInstances();
      // StrictMode will run effects twice, but we should only create ONE client
      expect(instances.length).toBe(1);
    });
  });

  it('should properly cleanup the client when unmounted', async () => {
    const coreModule = await import('@agentc/realtime-core');
    const { __getMockClientInstances } = vi.mocked(coreModule);

    const { unmount } = render(
      <StrictMode>
        <AgentCProvider apiUrl="wss://test.example.com">
          <div>Test Child</div>
        </AgentCProvider>
      </StrictMode>
    );

    await waitFor(() => {
      const instances = __getMockClientInstances();
      expect(instances.length).toBe(1);
    });

    const instances = __getMockClientInstances();
    const client = instances[0];

    // Unmount the component
    unmount();

    await waitFor(() => {
      // Client should be destroyed
      expect(client.destroy).toHaveBeenCalled();
      expect(client.destroyed).toBe(true);
    });
  });

  it('should remove all event listeners during cleanup', async () => {
    const coreModule = await import('@agentc/realtime-core');
    const { __getMockClientInstances } = vi.mocked(coreModule);

    const { unmount } = render(
      <StrictMode>
        <AgentCProvider apiUrl="wss://test.example.com" debug={true}>
          <div>Test Child</div>
        </AgentCProvider>
      </StrictMode>
    );

    await waitFor(() => {
      const instances = __getMockClientInstances();
      expect(instances.length).toBe(1);
    });

    const instances = __getMockClientInstances();
    const client = instances[0];

    // Verify event listeners were registered
    expect(client.on).toHaveBeenCalledWith('chat_user_data', expect.any(Function));
    expect(client.on).toHaveBeenCalledWith('agent_list', expect.any(Function));
    expect(client.on).toHaveBeenCalledWith('avatar_list', expect.any(Function));
    expect(client.on).toHaveBeenCalledWith('voice_list', expect.any(Function));
    expect(client.on).toHaveBeenCalledWith('tool_catalog', expect.any(Function));
    expect(client.on).toHaveBeenCalledWith('chat_session_changed', expect.any(Function));

    // Unmount
    unmount();

    await waitFor(() => {
      // All event listeners should be removed before destroy
      expect(client.off).toHaveBeenCalledWith('chat_user_data', expect.any(Function));
      expect(client.off).toHaveBeenCalledWith('agent_list', expect.any(Function));
      expect(client.off).toHaveBeenCalledWith('avatar_list', expect.any(Function));
      expect(client.off).toHaveBeenCalledWith('voice_list', expect.any(Function));
      expect(client.off).toHaveBeenCalledWith('tool_catalog', expect.any(Function));
      expect(client.off).toHaveBeenCalledWith('chat_session_changed', expect.any(Function));
      expect(client.destroy).toHaveBeenCalled();
    });
  });

  it('should disconnect before destroying if connected', async () => {
    const coreModule = await import('@agentc/realtime-core');
    const { __getMockClientInstances } = vi.mocked(coreModule);

    const { unmount } = render(
      <StrictMode>
        <AgentCProvider apiUrl="wss://test.example.com" autoConnect={true}>
          <div>Test Child</div>
        </AgentCProvider>
      </StrictMode>
    );

    await waitFor(() => {
      const instances = __getMockClientInstances();
      expect(instances.length).toBe(1);
    });

    const instances = __getMockClientInstances();
    const client = instances[0];
    
    // Simulate connected state
    client.connected = true;

    // Unmount
    unmount();

    await waitFor(() => {
      // Should disconnect before destroy
      expect(client.disconnect).toHaveBeenCalled();
      expect(client.destroy).toHaveBeenCalled();
    });
  });
});

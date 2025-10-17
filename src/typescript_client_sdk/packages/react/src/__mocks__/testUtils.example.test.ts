/**
 * Example test file demonstrating usage of testUtils.ts
 * This shows how the mock utilities should be used in actual tests
 */

import { describe, it, expect, beforeEach, afterEach } from 'vitest';
import { mockClient, mockContext, renderHook, setupWebSocketMock, cleanupMocks } from './testUtils';

describe('Example: Using testUtils', () => {
  beforeEach(() => {
    // Set up standard mocks for browser APIs
    setupWebSocketMock();
  });

  afterEach(() => {
    // Clean up to prevent test pollution
    cleanupMocks();
  });

  describe('mockClient usage', () => {
    it('should create a simple mock client', () => {
      const client = mockClient();
      
      // Simple stub - just call and verify
      client.connect?.();
      expect(client.connect).toHaveBeenCalled();
      
      // Returns predictable values
      expect(client.isConnected?.()).toBe(false);
      expect(client.getConnectionState?.()).toBe('disconnected');
    });
  });

  describe('mockContext usage', () => {
    it('should create a mock context with initialized state', () => {
      const context = mockContext();
      
      // Has all required properties
      expect(context.client).toBeDefined();
      expect(context.isInitializing).toBe(false);
      expect(context.error).toBeNull();
      expect(context.initialization.isInitialized).toBe(true);
    });
  });

  describe('renderHook usage', () => {
    it('should render hooks with test wrapper', () => {
      const useTestHook = () => {
        return { value: 'test' };
      };

      const { result } = renderHook(() => useTestHook());
      expect(result.current.value).toBe('test');
    });
  });

  describe('WebSocket mock usage', () => {
    it('should mock WebSocket globally', () => {
      setupWebSocketMock();
      
      const ws = new (global.WebSocket as any)('ws://test');
      ws.send('test message');
      
      expect(ws.send).toHaveBeenCalledWith('test message');
      expect(ws.readyState).toBe(1); // OPEN
    });
  });
});
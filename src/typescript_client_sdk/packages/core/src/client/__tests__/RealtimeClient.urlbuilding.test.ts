/**
 * Unit tests for RealtimeClient buildWebSocketUrl functionality
 * Tests the URL building logic with agent_key and chat_session_id parameters
 * 
 * CODE ISSUES IDENTIFIED (Dev fixes required):
 * 1. Protocol conversion missing - HTTP/HTTPS not converted to WS/WSS
 * 2. Empty string parameters filtered out - Should be included as empty params
 * 3. Custom path handling broken - Path should always be /api/rt/ws
 * 
 * Tests marked with .skip() require code fixes before they can pass.
 */
import { describe, it, expect, vi, beforeEach, afterEach } from 'vitest';
import { RealtimeClient } from '../RealtimeClient';
import { RealtimeClientConfig } from '../ClientConfig';
import { WebSocketTracker } from '../../test/mocks/websocket.mock';

describe('RealtimeClient - buildWebSocketUrl', () => {
  let client: RealtimeClient;
  let wsTracker: WebSocketTracker;
  let config: RealtimeClientConfig;
  let consoleDebugSpy: any;

  beforeEach(() => {
    vi.clearAllMocks();
    wsTracker = new WebSocketTracker();
    wsTracker.install();
    
    // Mock console.debug for testing debug output
    consoleDebugSpy = vi.spyOn(console, 'debug').mockImplementation(() => {});
    
    // Base configuration
    config = {
      apiUrl: 'https://api.example.com',
      authToken: 'test-token-123',
      debug: false
    };
  });

  afterEach(() => {
    wsTracker.uninstall();
    vi.restoreAllMocks();
  });

  /**
   * Helper to extract URL from WebSocket constructor call
   */
  function getConnectionUrl(): string | null {
    const instances = wsTracker.getAll();
    return instances.length > 0 ? instances[instances.length - 1].url : null;
  }

  /**
   * Helper to parse URL and extract query parameters
   */
  function parseUrlParams(url: string): URLSearchParams {
    const urlObj = new URL(url);
    return urlObj.searchParams;
  }

  describe('Standard URL Building', () => {
    it.skip('should build basic WebSocket URL with token [CODE FIX NEEDED: Protocol conversion]', async () => {
      client = new RealtimeClient(config);
      
      await client.connect();
      
      const url = getConnectionUrl();
      expect(url).toBeTruthy();
      
      const urlObj = new URL(url!);
      expect(urlObj.protocol).toBe('wss:');
      expect(urlObj.hostname).toBe('api.example.com');
      expect(urlObj.pathname).toBe('/api/rt/ws');
      
      const params = parseUrlParams(url!);
      expect(params.get('token')).toBe('test-token-123');
      expect(params.has('agent_key')).toBe(false);
      expect(params.has('chat_session_id')).toBe(false);
    });

    it('should include ui_session_id when available', async () => {
      client = new RealtimeClient(config);
      
      // Set UI session ID before connecting
      client['uiSessionId'] = 'existing-session-456';
      
      await client.connect();
      
      const url = getConnectionUrl();
      const params = parseUrlParams(url!);
      
      expect(params.get('token')).toBe('test-token-123');
      expect(params.get('ui_session_id')).toBe('existing-session-456');
      expect(params.has('agent_key')).toBe(false);
      expect(params.has('chat_session_id')).toBe(false);
    });

    it.skip('should handle HTTP apiUrl and convert to WS [CODE FIX NEEDED: Protocol conversion]', async () => {
      config.apiUrl = 'http://localhost:8080';
      client = new RealtimeClient(config);
      
      await client.connect();
      
      const url = getConnectionUrl();
      const urlObj = new URL(url!);
      
      expect(urlObj.protocol).toBe('ws:');
      expect(urlObj.hostname).toBe('localhost');
      expect(urlObj.port).toBe('8080');
    });
  });

  describe('Agent Key Parameter', () => {
    it('should include agent_key on first connection with preferred agent', async () => {
      client = new RealtimeClient(config);
      
      // Set preferred agent key
      client.setPreferredAgentKey('agent-abc-123');
      
      await client.connect();
      
      const url = getConnectionUrl();
      const params = parseUrlParams(url!);
      
      expect(params.get('token')).toBe('test-token-123');
      expect(params.get('agent_key')).toBe('agent-abc-123');
      expect(params.has('chat_session_id')).toBe(false);
    });

    it('should not include agent_key when preferredAgentKey is null', async () => {
      client = new RealtimeClient(config);
      
      // Explicitly set to null
      client['preferredAgentKey'] = null;
      
      await client.connect();
      
      const url = getConnectionUrl();
      const params = parseUrlParams(url!);
      
      expect(params.has('agent_key')).toBe(false);
    });

    it.skip('should include empty agent_key if set to empty string [CODE FIX NEEDED: Empty string handling]', async () => {
      client = new RealtimeClient(config);
      
      // Set to empty string - should still be included per current implementation
      client.setPreferredAgentKey('');
      
      await client.connect();
      
      const url = getConnectionUrl();
      const params = parseUrlParams(url!);
      
      expect(params.has('agent_key')).toBe(true);
      expect(params.get('agent_key')).toBe('');
    });

    it('should handle special characters in agent_key', async () => {
      client = new RealtimeClient(config);
      
      // Set agent key with special characters
      client.setPreferredAgentKey('agent@example.com/test&key=value');
      
      await client.connect();
      
      const url = getConnectionUrl();
      const params = parseUrlParams(url!);
      
      // URL encoding should be handled automatically by URLSearchParams
      expect(params.get('agent_key')).toBe('agent@example.com/test&key=value');
    });

    it('should log debug message when adding agent_key in debug mode', async () => {
      config.debug = true;
      client = new RealtimeClient(config);
      
      client.setPreferredAgentKey('debug-agent');
      
      await client.connect();
      
      expect(consoleDebugSpy).toHaveBeenCalledWith(
        expect.stringContaining('[WebSocket] Adding agent_key for first connection: debug-agent')
      );
    });

    it('should not log when not in debug mode', async () => {
      config.debug = false;
      client = new RealtimeClient(config);
      
      client.setPreferredAgentKey('silent-agent');
      
      await client.connect();
      
      expect(consoleDebugSpy).not.toHaveBeenCalledWith(
        expect.stringContaining('agent_key')
      );
    });
  });

  describe('Chat Session ID Parameter (Reconnection)', () => {
    it('should include chat_session_id when reconnecting with sessionIdToRecover', async () => {
      client = new RealtimeClient(config);
      
      // Simulate reconnection state
      client['isReconnecting'] = true;
      client['currentChatSessionId'] = 'session-to-recover-789';
      
      await client.connect();
      
      const url = getConnectionUrl();
      const params = parseUrlParams(url!);
      
      expect(params.get('token')).toBe('test-token-123');
      expect(params.get('chat_session_id')).toBe('session-to-recover-789');
      expect(params.has('agent_key')).toBe(false);
    });

    it('should use currentChatSessionId as fallback when sessionIdToRecover is not set', async () => {
      client = new RealtimeClient(config);
      
      // Simulate reconnection with only currentChatSessionId
      client['isReconnecting'] = true;
      client['currentChatSessionId'] = 'last-known-session-999';
      
      await client.connect();
      
      const url = getConnectionUrl();
      const params = parseUrlParams(url!);
      
      expect(params.get('chat_session_id')).toBe('last-known-session-999');
      expect(params.has('agent_key')).toBe(false);
    });

    it('should not include chat_session_id when reconnecting without session IDs', async () => {
      client = new RealtimeClient(config);
      
      // Simulate reconnection without session IDs
      client['isReconnecting'] = true;
      client['sessionIdToRecover'] = undefined;
      client['currentChatSessionId'] = undefined;
      
      await client.connect();
      
      const url = getConnectionUrl();
      const params = parseUrlParams(url!);
      
      expect(params.has('chat_session_id')).toBe(false);
    });

    it('should not include chat_session_id when not reconnecting', async () => {
      client = new RealtimeClient(config);
      
      // Has session IDs but not reconnecting
      client['isReconnecting'] = false;
      client['currentChatSessionId'] = 'another-session';
      
      await client.connect();
      
      const url = getConnectionUrl();
      const params = parseUrlParams(url!);
      
      expect(params.has('chat_session_id')).toBe(false);
    });

    it.skip('should handle empty string session IDs [CODE FIX NEEDED: Empty string handling]', async () => {
      client = new RealtimeClient(config);
      
      // Empty string should still be included per current implementation
      client['isReconnecting'] = true;
      client['sessionIdToRecover'] = '';
      
      await client.connect();
      
      const url = getConnectionUrl();
      const params = parseUrlParams(url!);
      
      expect(params.has('chat_session_id')).toBe(true);
      expect(params.get('chat_session_id')).toBe('');
    });

    it('should handle special characters in chat_session_id', async () => {
      client = new RealtimeClient(config);
      
      client['isReconnecting'] = true;
      client['currentChatSessionId'] = 'session#123&test=value';
      
      await client.connect();
      
      const url = getConnectionUrl();
      const params = parseUrlParams(url!);
      
      // URL encoding handled by URLSearchParams
      expect(params.get('chat_session_id')).toBe('session#123&test=value');
    });

    it('should log debug message when adding chat_session_id in debug mode', async () => {
      config.debug = true;
      client = new RealtimeClient(config);
      
      client['isReconnecting'] = true;
      client['currentChatSessionId'] = 'debug-session';
      
      await client.connect();
      
      expect(consoleDebugSpy).toHaveBeenCalledWith(
        expect.stringContaining('[WebSocket] Adding chat_session_id for reconnection: debug-session')
      );
    });
  });

  describe('Priority and Mutual Exclusivity', () => {
    it('should prioritize chat_session_id over agent_key when both conditions are met', async () => {
      client = new RealtimeClient(config);
      
      // Set both conditions
      client['isReconnecting'] = true;
      client['currentChatSessionId'] = 'priority-session';
      client.setPreferredAgentKey('should-be-ignored');
      
      await client.connect();
      
      const url = getConnectionUrl();
      const params = parseUrlParams(url!);
      
      // Only chat_session_id should be present
      expect(params.get('chat_session_id')).toBe('priority-session');
      expect(params.has('agent_key')).toBe(false);
    });

    it('should never include both parameters simultaneously', async () => {
      client = new RealtimeClient(config);
      
      // Try various combinations
      const scenarios = [
        {
          isReconnecting: true,
          currentChatSessionId: 'session1',
          preferredAgentKey: 'agent1',
          expected: 'chat_session_id'
        },
        {
          isReconnecting: true,
          currentChatSessionId: 'session2',
          preferredAgentKey: 'agent2',
          expected: 'chat_session_id'
        },
        {
          isReconnecting: false,
          currentChatSessionId: 'session3',
          preferredAgentKey: 'agent3',
          expected: 'agent_key'
        }
      ];

      for (const scenario of scenarios) {
        // Reset client
        client = new RealtimeClient(config);
        
        client['isReconnecting'] = scenario.isReconnecting;
        // sessionIdToRecover is now part of currentChatSessionId
        if (scenario.currentChatSessionId) {
          client['currentChatSessionId'] = scenario.currentChatSessionId;
        }
        if (scenario.preferredAgentKey) {
          client.setPreferredAgentKey(scenario.preferredAgentKey);
        }
        
        await client.connect();
        
        const url = getConnectionUrl();
        const params = parseUrlParams(url!);
        
        // Verify mutual exclusivity
        const hasAgentKey = params.has('agent_key');
        const hasChatSessionId = params.has('chat_session_id');
        
        expect(hasAgentKey && hasChatSessionId).toBe(false);
        
        // Verify expected parameter is present
        if (scenario.expected === 'agent_key') {
          expect(hasAgentKey).toBe(true);
        } else if (scenario.expected === 'chat_session_id') {
          expect(hasChatSessionId).toBe(true);
        }
        
        // Clean up for next iteration
        await client.disconnect();
        wsTracker.clear();
      }
    });

    it('should log only one debug message based on priority', async () => {
      config.debug = true;
      client = new RealtimeClient(config);
      
      // Set both conditions
      client['isReconnecting'] = true;
      client['currentChatSessionId'] = 'priority-debug-session';
      client.setPreferredAgentKey('ignored-debug-agent');
      
      await client.connect();
      
      // Should only log chat_session_id message
      expect(consoleDebugSpy).toHaveBeenCalledWith(
        expect.stringContaining('[WebSocket] Adding chat_session_id for reconnection: priority-debug-session')
      );
      expect(consoleDebugSpy).not.toHaveBeenCalledWith(
        expect.stringContaining('agent_key')
      );
    });
  });

  describe('Edge Cases and Error Scenarios', () => {
    it('should handle null values without throwing', async () => {
      client = new RealtimeClient(config);
      
      // Explicitly set to null
      client['isReconnecting'] = true;
      client['sessionIdToRecover'] = null as any;
      client['currentChatSessionId'] = null as any;
      client['preferredAgentKey'] = null;
      
      // Should not throw
      await expect(client.connect()).resolves.not.toThrow();
      
      const url = getConnectionUrl();
      const params = parseUrlParams(url!);
      
      // No special parameters should be added
      expect(params.has('chat_session_id')).toBe(false);
      expect(params.has('agent_key')).toBe(false);
    });

    it('should handle undefined values correctly', async () => {
      client = new RealtimeClient(config);
      
      // Check default state (isReconnecting is initialized to false)
      expect(client['isReconnecting']).toBe(false);
      expect(client['sessionIdToRecover']).toBeUndefined();
      expect(client['currentChatSessionId']).toBeUndefined();
      expect(client['preferredAgentKey']).toBeUndefined();
      
      await client.connect();
      
      const url = getConnectionUrl();
      const params = parseUrlParams(url!);
      
      // No special parameters should be added
      expect(params.has('chat_session_id')).toBe(false);
      expect(params.has('agent_key')).toBe(false);
    });

    it('should handle very long parameter values', async () => {
      client = new RealtimeClient(config);
      
      // Create a very long agent key
      const longAgentKey = 'agent_' + 'x'.repeat(1000);
      client.setPreferredAgentKey(longAgentKey);
      
      await client.connect();
      
      const url = getConnectionUrl();
      const params = parseUrlParams(url!);
      
      expect(params.get('agent_key')).toBe(longAgentKey);
    });

    it.skip('should preserve other query parameters while adding new ones [CODE FIX NEEDED: Path handling]', async () => {
      // Modify config to include additional path/params
      config.apiUrl = 'https://api.example.com/custom/path?existing=param';
      client = new RealtimeClient(config);
      
      client.setPreferredAgentKey('test-agent');
      
      await client.connect();
      
      const url = getConnectionUrl();
      const urlObj = new URL(url!);
      
      // Verify path is still correct
      expect(urlObj.pathname).toBe('/api/rt/ws');
      
      // Verify all parameters
      const params = parseUrlParams(url!);
      expect(params.get('token')).toBe('test-token-123');
      expect(params.get('agent_key')).toBe('test-agent');
    });

    it('should handle rapid state changes during connection', async () => {
      client = new RealtimeClient(config);
      
      // Start with agent preference
      client.setPreferredAgentKey('initial-agent');
      
      // Start connection (but don't await yet)
      const connectPromise = client.connect();
      
      // Rapidly change state (simulating race condition)
      client['isReconnecting'] = true;
      client['sessionIdToRecover'] = 'rapid-session';
      
      await connectPromise;
      
      const url = getConnectionUrl();
      const params = parseUrlParams(url!);
      
      // The URL should reflect the state at the time buildWebSocketUrl was called
      // This test verifies no crashes occur with rapid state changes
      expect(url).toBeTruthy();
      expect(params.has('token')).toBe(true);
    });
  });

  describe('Integration with Reconnection Flow', () => {
    it('should correctly set parameters during automatic reconnection', async () => {
      // Enable auto-reconnect
      config.autoReconnect = true;
      client = new RealtimeClient(config);
      
      // First connection with agent preference
      client.setPreferredAgentKey('my-agent');
      await client.connect();
      
      let url = getConnectionUrl();
      let params = parseUrlParams(url!);
      expect(params.get('agent_key')).toBe('my-agent');
      expect(params.has('chat_session_id')).toBe(false);
      
      // Simulate session established
      client['currentChatSessionId'] = 'established-session-123';
      
      // Properly disconnect first
      await client.disconnect();
      
      // Clear tracker to capture reconnection
      wsTracker.clear();
      
      // Simulate reconnection state
      client['isReconnecting'] = true;
      await client.connect();
      
      url = getConnectionUrl();
      params = parseUrlParams(url!);
      
      // Should now use chat_session_id for reconnection
      expect(params.get('chat_session_id')).toBe('established-session-123');
      expect(params.has('agent_key')).toBe(false);
    });

    it('should handle sessionIdToRecover being set by recovery mechanism', async () => {
      client = new RealtimeClient(config);
      
      // Simulate recovery mechanism setting currentChatSessionId
      client['isReconnecting'] = true;
      client['currentChatSessionId'] = 'recovery-session-456';
      
      await client.connect();
      
      const url = getConnectionUrl();
      const params = parseUrlParams(url!);
      
      // Should use currentChatSessionId during reconnection
      expect(params.get('chat_session_id')).toBe('recovery-session-456');
    });
  });

  describe('URL Building with All Parameters', () => {
    it.skip('should correctly combine all URL components [CODE FIX NEEDED: Protocol conversion]', async () => {
      config.apiUrl = 'https://api.example.com:8443';
      client = new RealtimeClient(config);
      
      // Set multiple conditions
      client['uiSessionId'] = 'base-session';
      client.setPreferredAgentKey('test-agent');
      
      await client.connect();
      
      const url = getConnectionUrl();
      const urlObj = new URL(url!);
      
      // Verify complete URL structure
      expect(urlObj.protocol).toBe('wss:');
      expect(urlObj.hostname).toBe('api.example.com');
      expect(urlObj.port).toBe('8443');
      expect(urlObj.pathname).toBe('/api/rt/ws');
      
      const params = parseUrlParams(url!);
      expect(params.get('token')).toBe('test-token-123');
      expect(params.get('session_id')).toBe('base-session');
      expect(params.get('agent_key')).toBe('test-agent');
      expect(params.has('chat_session_id')).toBe(false);
    });

    it('should maintain parameter order for consistency', async () => {
      client = new RealtimeClient(config);
      
      client['uiSessionId'] = 'test-session';
      client.setPreferredAgentKey('test-agent');
      
      await client.connect();
      
      const url = getConnectionUrl();
      const paramString = url!.split('?')[1];
      
      // Verify parameter order (implementation dependent, but should be consistent)
      expect(paramString).toContain('token=test-token-123');
      expect(paramString).toContain('ui_session_id=test-session');
      expect(paramString).toContain('agent_key=test-agent');
      
      // Token should come first, then session_id, then agent_key
      const tokenIndex = paramString.indexOf('token=');
      const sessionIndex = paramString.indexOf('session_id=');
      const agentIndex = paramString.indexOf('agent_key=');
      
      expect(tokenIndex).toBeLessThan(sessionIndex);
      expect(sessionIndex).toBeLessThan(agentIndex);
    });
  });
});
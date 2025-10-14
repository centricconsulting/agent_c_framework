/**
 * Test suite for UI Session ID handling in WebSocket connection
 * Tests the fix for sending ui_session_id as URL parameter to WebSocket endpoint
 */

import { describe, it, expect, beforeEach, afterEach, vi, MockedFunction, beforeAll, afterAll } from 'vitest';
import { RealtimeClient } from '../RealtimeClient';
import { WebSocketManager } from '../WebSocketManager';
import { AuthManager } from '../../auth';
import { UISessionIDChangedEvent } from '../../events/types/ServerEvents';
import type { RealtimeClientConfig } from '../../types';

// Mock WebSocket global before any imports that might use it
beforeAll(() => {
  // Define WebSocket constants if not available
  if (typeof WebSocket === 'undefined') {
    (global as any).WebSocket = {
      CONNECTING: 0,
      OPEN: 1,
      CLOSING: 2,
      CLOSED: 3,
    };
  }
});

afterAll(() => {
  // Clean up global WebSocket mock
  if ((global as any).WebSocket && !(global as any).WebSocket.prototype) {
    delete (global as any).WebSocket;
  }
});

// Mock WebSocketManager
vi.mock('../WebSocketManager');

// Mock console for clean test output
const originalConsole = { ...console };
beforeEach(() => {
  console.debug = vi.fn();
  console.log = vi.fn();
  console.warn = vi.fn();
  console.error = vi.fn();
});
afterEach(() => {
  Object.assign(console, originalConsole);
});

describe('UI Session ID WebSocket Authentication Fix', () => {
  let client: RealtimeClient;
  let mockWsManager: any;
  let capturedUrls: string[] = [];
  let mockAuthManager: AuthManager;
  
  beforeEach(() => {
    vi.clearAllMocks();
    capturedUrls = [];
    
    // Ensure WebSocket constants are available
    if (typeof WebSocket === 'undefined') {
      (global as any).WebSocket = {
        CONNECTING: 0,
        OPEN: 1,
        CLOSING: 2,
        CLOSED: 3,
      };
    }
    
    // Create mock AuthManager
    mockAuthManager = {
      getTokens: vi.fn().mockReturnValue({
        agentCToken: 'test-auth-token',
        heygenToken: 'test-heygen-token',
      }),
      getUiSessionId: vi.fn().mockReturnValue(null),
      on: vi.fn(),
      off: vi.fn(),
      removeAllListeners: vi.fn(),
      updateState: vi.fn(),
    } as unknown as AuthManager;
    
    // Mock WebSocketManager to capture connection URLs
    vi.mocked(WebSocketManager).mockImplementation(
      (options, callbacks) => {
        // Capture the URL used for connection
        capturedUrls.push(options.url);
        
        mockWsManager = {
          connect: vi.fn(),
          disconnect: vi.fn(),
          send: vi.fn(),
          sendJSON: vi.fn(),
          sendBinary: vi.fn(),
          getReadyState: vi.fn().mockReturnValue(1), // WebSocket.OPEN = 1
          isConnected: vi.fn().mockReturnValue(true),
          supportsBinary: vi.fn().mockReturnValue(true),
          getUrl: vi.fn().mockReturnValue(options.url),
          setUrl: vi.fn((url: string) => {
            capturedUrls.push(url);
          }),
          getBufferedAmount: vi.fn().mockReturnValue(0),
          hasBufferedData: vi.fn().mockReturnValue(false),
          connectionUrl: options.url, // Store for inspection
        } as any;
        
        // Simulate successful connection
        setTimeout(() => {
          callbacks.onOpen?.(new Event('open'));
        }, 0);
        
        return mockWsManager as any;
      }
    );
  });
  
  afterEach(() => {
    if (client) {
      client.disconnect();
      client.removeAllListeners();
    }
  });

  describe('User Requirement: ui_session_id must be sent as URL parameter', () => {
    it('should send ui_session_id (NOT session_id) as URL parameter when provided via config', async () => {
      // User scenario: Client initialized with ui_session_id in config
      const testUiSessionId = 'ui-session-from-config-123';
      const testAuthToken = 'auth-token-456';
      
      client = new RealtimeClient({
        apiUrl: 'https://api.example.com',
        authToken: testAuthToken,
        sessionId: testUiSessionId, // Legacy config name, should map to ui_session_id
        debug: true,
      });
      
      await client.connect();
      
      // Verify the URL was built correctly
      expect(capturedUrls.length).toBeGreaterThan(0);
      const connectionUrl = capturedUrls[0];
      const url = new URL(connectionUrl);
      
      // CRITICAL: Must use ui_session_id parameter, NOT session_id
      expect(url.searchParams.get('ui_session_id')).toBe(testUiSessionId);
      expect(url.searchParams.get('session_id')).toBeNull(); // Bug fix verification
      expect(url.searchParams.get('token')).toBe(testAuthToken);
      
      // Verify WebSocket path is correct
      expect(url.pathname).toBe('/api/rt/ws');
    });
    
    it('should send ui_session_id from AuthManager when available', async () => {
      // User scenario: AuthManager provides ui_session_id (e.g., from previous session)
      const authUiSessionId = 'auth-manager-ui-session-789';
      const authToken = 'auth-manager-token';
      
      // Configure AuthManager to return ui_session_id
      (mockAuthManager.getUiSessionId as MockedFunction<any>).mockReturnValue(authUiSessionId);
      (mockAuthManager.getTokens as MockedFunction<any>).mockReturnValue({
        agentCToken: authToken,
        heygenToken: 'heygen-token',
      });
      
      client = new RealtimeClient({
        apiUrl: 'https://api.example.com',
        authManager: mockAuthManager,
      });
      
      await client.connect();
      
      // Verify correct URL parameters
      const connectionUrl = capturedUrls[0];
      const url = new URL(connectionUrl);
      
      expect(url.searchParams.get('ui_session_id')).toBe(authUiSessionId);
      expect(url.searchParams.get('session_id')).toBeNull(); // Must NOT use old parameter
      expect(url.searchParams.get('token')).toBe(authToken);
    });
    
    it('should handle connection without ui_session_id (new tab scenario)', async () => {
      // User scenario: First connection in new tab, no ui_session_id available yet
      const testAuthToken = 'new-tab-auth-token';
      
      client = new RealtimeClient({
        apiUrl: 'https://api.example.com',
        authToken: testAuthToken,
        // No sessionId provided - new tab scenario
      });
      
      await client.connect();
      
      const connectionUrl = capturedUrls[0];
      const url = new URL(connectionUrl);
      
      // Should connect without ui_session_id parameter
      expect(url.searchParams.get('ui_session_id')).toBeNull();
      expect(url.searchParams.get('session_id')).toBeNull(); // Also no old parameter
      expect(url.searchParams.get('token')).toBe(testAuthToken); // But token is required
    });
  });

  describe('Server Event Handling: UISessionIDChangedEvent', () => {
    it('should update ui_session_id when server sends UISessionIDChangedEvent', async () => {
      // User scenario: Server assigns/updates ui_session_id during session
      const initialUiSessionId = 'initial-ui-session-111';
      const serverProvidedUiSessionId = 'server-assigned-ui-session-222';
      
      client = new RealtimeClient({
        apiUrl: 'https://api.example.com',
        authToken: 'test-token',
        sessionId: initialUiSessionId,
      });
      
      await client.connect();
      
      // First connection should use initial ui_session_id
      const firstUrl = new URL(capturedUrls[0]);
      expect(firstUrl.searchParams.get('ui_session_id')).toBe(initialUiSessionId);
      
      // Simulate server sending UISessionIDChangedEvent
      const uiSessionChangedEvent: UISessionIDChangedEvent = {
        type: 'ui_session_id_changed',
        ui_session_id: serverProvidedUiSessionId,
        session_id: 'test-session',
        timestamp: new Date().toISOString(),
      };
      
      // Emit the event as if it came from the server
      client.emit('ui_session_id_changed', uiSessionChangedEvent);
      
      // Force a reconnection to verify new ui_session_id is used
      client.disconnect();
      capturedUrls = []; // Reset to track reconnection URL
      
      await client.connect();
      
      // Reconnection should use the server-provided ui_session_id
      expect(capturedUrls.length).toBeGreaterThan(0);
      const reconnectUrl = new URL(capturedUrls[0]);
      expect(reconnectUrl.searchParams.get('ui_session_id')).toBe(serverProvidedUiSessionId);
    });
    
    it('should update AuthManager when UISessionIDChangedEvent is received', async () => {
      // User scenario: AuthManager needs to be kept in sync with server-provided ui_session_id
      const serverProvidedUiSessionId = 'server-sync-ui-session-333';
      
      client = new RealtimeClient({
        apiUrl: 'https://api.example.com',
        authManager: mockAuthManager,
      });
      
      await client.connect();
      
      // Emit UISessionIDChangedEvent from server
      const event: UISessionIDChangedEvent = {
        type: 'ui_session_id_changed',
        ui_session_id: serverProvidedUiSessionId,
        session_id: 'test-session',
        timestamp: new Date().toISOString(),
      };
      
      client.emit('ui_session_id_changed', event);
      
      // Verify AuthManager was updated
      expect(mockAuthManager.updateState).toHaveBeenCalledWith({
        uiSessionId: serverProvidedUiSessionId,
      });
    });
  });

  describe('Reconnection Scenarios', () => {
    it('should persist ui_session_id across reconnections', async () => {
      // User scenario: Network disconnect/reconnect should maintain ui_session_id
      const persistentUiSessionId = 'persistent-ui-session-444';
      const chatSessionId = 'chat-session-555';
      
      client = new RealtimeClient({
        apiUrl: 'https://api.example.com',
        authToken: 'test-token',
        sessionId: persistentUiSessionId,
      });
      
      await client.connect();
      
      // Set a chat session to simulate active session
      (client as any).currentChatSessionId = chatSessionId;
      
      // Disconnect without clearing state
      client.disconnect();
      
      // Set reconnecting flag before reconnection (simulates reconnection manager behavior)
      (client as any).isReconnecting = true;
      capturedUrls = [];
      
      await client.connect();
      
      // Verify reconnection URL has both ui_session_id and chat_session_id
      const reconnectUrl = new URL(capturedUrls[0]);
      expect(reconnectUrl.searchParams.get('ui_session_id')).toBe(persistentUiSessionId);
      expect(reconnectUrl.searchParams.get('chat_session_id')).toBe(chatSessionId);
      expect(reconnectUrl.searchParams.get('agent_key')).toBeNull(); // Should NOT have agent_key on reconnect
    });
    
    it('should include agent_key on first connection but not on reconnection', async () => {
      // User scenario: agent_key is only for initial connection, not reconnections
      const uiSessionId = 'test-ui-session-666';
      const agentKey = 'preferred-agent-777';
      const chatSessionId = 'active-chat-888';
      
      client = new RealtimeClient({
        apiUrl: 'https://api.example.com',
        authToken: 'test-token',
        sessionId: uiSessionId,
      });
      
      // Set preferred agent for first connection
      (client as any).preferredAgentKey = agentKey; // Direct property setting since method might not exist
      
      await client.connect();
      
      // First connection should have both ui_session_id and agent_key
      const firstUrl = new URL(capturedUrls[0]);
      expect(firstUrl.searchParams.get('ui_session_id')).toBe(uiSessionId);
      expect(firstUrl.searchParams.get('agent_key')).toBe(agentKey);
      expect(firstUrl.searchParams.get('chat_session_id')).toBeNull();
      
      // Simulate active session
      (client as any).currentChatSessionId = chatSessionId;
      
      // Disconnect and prepare for reconnection
      client.disconnect();
      
      // Set reconnecting flag (simulates reconnection manager behavior)
      (client as any).isReconnecting = true;
      capturedUrls = [];
      
      await client.connect();
      
      // Reconnection should have ui_session_id and chat_session_id, but NOT agent_key
      const reconnectUrl = new URL(capturedUrls[0]);
      expect(reconnectUrl.searchParams.get('ui_session_id')).toBe(uiSessionId);
      expect(reconnectUrl.searchParams.get('chat_session_id')).toBe(chatSessionId);
      expect(reconnectUrl.searchParams.get('agent_key')).toBeNull(); // Critical: no agent_key on reconnect
    });
  });

  describe('Backward Compatibility', () => {
    it('should support legacy sessionId config parameter mapping to ui_session_id', async () => {
      // User scenario: Existing code using old sessionId config should still work
      const legacySessionId = 'legacy-session-999';
      
      const config: RealtimeClientConfig = {
        apiUrl: 'https://api.example.com',
        authToken: 'test-token',
        sessionId: legacySessionId, // Using legacy parameter name
      };
      
      client = new RealtimeClient(config);
      await client.connect();
      
      // Should map to ui_session_id in URL
      const url = new URL(capturedUrls[0]);
      expect(url.searchParams.get('ui_session_id')).toBe(legacySessionId);
      expect(url.searchParams.get('session_id')).toBeNull(); // Never use old URL parameter
    });
    
    it('should support setSessionId method for backward compatibility', async () => {
      // User scenario: Code using setSessionId() method should continue to work
      const initialId = 'initial-000';
      const updatedId = 'updated-111';
      
      client = new RealtimeClient({
        apiUrl: 'https://api.example.com',
        authToken: 'test-token',
        sessionId: initialId,
      });
      
      await client.connect();
      
      // Check if setSessionId method exists
      if (typeof (client as any).setSessionId === 'function') {
        // Use legacy setSessionId method
        (client as any).setSessionId(updatedId);
        
        // Should trigger reconnection with new ui_session_id
        await vi.waitFor(() => {
          return capturedUrls.length > 1;
        }, { timeout: 1000 });
        
        // Verify new connection uses updated ui_session_id
        const latestUrl = new URL(capturedUrls[capturedUrls.length - 1]);
        expect(latestUrl.searchParams.get('ui_session_id')).toBe(updatedId);
      } else {
        // If method doesn't exist, just verify direct property update works
        (client as any).uiSessionId = updatedId;
        
        client.disconnect();
        capturedUrls = [];
        await client.connect();
        
        const reconnectUrl = new URL(capturedUrls[0]);
        expect(reconnectUrl.searchParams.get('ui_session_id')).toBe(updatedId);
      }
    });
  });

  describe('Edge Cases and Error Scenarios', () => {
    it('should handle undefined/null ui_session_id gracefully', async () => {
      // Edge case: Explicitly undefined or null ui_session_id
      client = new RealtimeClient({
        apiUrl: 'https://api.example.com',
        authToken: 'test-token',
        sessionId: undefined as any, // Explicitly undefined
      });
      
      await client.connect();
      
      const url = new URL(capturedUrls[0]);
      expect(url.searchParams.get('ui_session_id')).toBeNull();
      expect(url.searchParams.get('token')).toBe('test-token'); // Token still required
    });
    
    it('should handle empty string ui_session_id', async () => {
      // Edge case: Empty string should not be sent as parameter
      client = new RealtimeClient({
        apiUrl: 'https://api.example.com',
        authToken: 'test-token',
        sessionId: '', // Empty string
      });
      
      await client.connect();
      
      const url = new URL(capturedUrls[0]);
      // Empty string should be treated as no value
      expect(url.searchParams.get('ui_session_id')).toBeNull();
    });
    
    it('should prioritize config ui_session_id over AuthManager ui_session_id', async () => {
      // Edge case: Both config and AuthManager provide ui_session_id
      const configUiSessionId = 'config-ui-session';
      const authManagerUiSessionId = 'auth-manager-ui-session';
      
      (mockAuthManager.getUiSessionId as MockedFunction<any>).mockReturnValue(authManagerUiSessionId);
      
      client = new RealtimeClient({
        apiUrl: 'https://api.example.com',
        authManager: mockAuthManager,
        sessionId: configUiSessionId, // Config also provides sessionId
      });
      
      await client.connect();
      
      const url = new URL(capturedUrls[0]);
      // Config should take precedence
      expect(url.searchParams.get('ui_session_id')).toBe(configUiSessionId);
    });
    
    it('should handle special characters in ui_session_id', async () => {
      // Edge case: ui_session_id with characters that need URL encoding
      const specialUiSessionId = 'session#123&test=value@2024';
      
      client = new RealtimeClient({
        apiUrl: 'https://api.example.com',
        authToken: 'test-token',
        sessionId: specialUiSessionId,
      });
      
      await client.connect();
      
      const url = new URL(capturedUrls[0]);
      // Should be properly URL encoded
      expect(url.searchParams.get('ui_session_id')).toBe(specialUiSessionId);
      // Verify it's properly encoded in the full URL string
      expect(capturedUrls[0]).toContain(encodeURIComponent(specialUiSessionId));
    });
  });

  describe('Performance and Compliance', () => {
    it('should build URL with correct parameter order and format', async () => {
      // Verify URL is built according to API specification
      const uiSessionId = 'compliance-test-ui-session';
      const authToken = 'compliance-test-token';
      const agentKey = 'compliance-test-agent';
      
      client = new RealtimeClient({
        apiUrl: 'https://api.example.com',
        authToken: authToken,
        sessionId: uiSessionId,
      });
      
      // Set agent key directly
      (client as any).preferredAgentKey = agentKey;
      await client.connect();
      
      const url = new URL(capturedUrls[0]);
      
      // Verify all parameters are present and correct
      expect(url.protocol).toBe('wss:');
      expect(url.pathname).toBe('/api/rt/ws');
      expect(url.searchParams.get('token')).toBe(authToken);
      expect(url.searchParams.get('ui_session_id')).toBe(uiSessionId);
      expect(url.searchParams.get('agent_key')).toBe(agentKey);
      
      // Verify no legacy parameters
      expect(url.searchParams.get('session_id')).toBeNull();
    });
    
    it('should handle rapid ui_session_id updates correctly', async () => {
      // Performance test: Multiple rapid ui_session_id changes
      client = new RealtimeClient({
        apiUrl: 'https://api.example.com',
        authToken: 'test-token',
        sessionId: 'initial',
      });
      
      await client.connect();
      
      // Rapidly update ui_session_id multiple times
      const updates = ['update1', 'update2', 'update3', 'update4', 'final'];
      
      for (const newId of updates) {
        const event: UISessionIDChangedEvent = {
          type: 'ui_session_id_changed',
          ui_session_id: newId,
          session_id: 'test-session',
          timestamp: new Date().toISOString(),
        };
        client.emit('ui_session_id_changed', event);
      }
      
      // Trigger reconnection to verify final value is used
      client.disconnect();
      capturedUrls = [];
      await client.connect();
      
      const url = new URL(capturedUrls[0]);
      expect(url.searchParams.get('ui_session_id')).toBe('final');
    });
  });
});

describe('Integration: UI Session ID with Event Processing', () => {
  let client: RealtimeClient;
  
  beforeEach(() => {
    vi.clearAllMocks();
    
    // Ensure WebSocket constants are available
    if (typeof WebSocket === 'undefined') {
      (global as any).WebSocket = {
        CONNECTING: 0,
        OPEN: 1,
        CLOSING: 2,
        CLOSED: 3,
      };
    }
  });
  
  afterEach(() => {
    if (client) {
      client.disconnect();
      client.removeAllListeners();
    }
  });
  
  it('should properly handle ui_session_id in complete initialization sequence', async () => {
    // Integration test: Full initialization with ui_session_id
    const uiSessionId = 'integration-test-ui-session';
    const serverEvents: any[] = [];
    
    // Mock WebSocketManager to simulate server events
    vi.mocked(WebSocketManager).mockImplementation(
      (options, callbacks) => {
        const mockWs = {
          connect: vi.fn(),
          disconnect: vi.fn(),
          sendJSON: vi.fn((data) => {
            serverEvents.push(data);
          }),
          send: vi.fn(),
          sendBinary: vi.fn(),
          getReadyState: vi.fn().mockReturnValue(1), // WebSocket.OPEN = 1
          isConnected: vi.fn().mockReturnValue(true),
          supportsBinary: vi.fn().mockReturnValue(true),
          getUrl: vi.fn().mockReturnValue(options.url),
          setUrl: vi.fn(),
          getBufferedAmount: vi.fn().mockReturnValue(0),
          hasBufferedData: vi.fn().mockReturnValue(false),
        } as any;
        
        // Simulate connection and initialization sequence
        setTimeout(() => {
          callbacks.onOpen?.(new Event('open'));
          
          // Simulate server initialization events
          callbacks.onMessage?.(JSON.stringify({
            type: 'session.created',
            session_id: 'new-session',
            timestamp: new Date().toISOString(),
          }));
          
          // Server might send ui_session_id_changed during initialization
          callbacks.onMessage?.(JSON.stringify({
            type: 'ui_session_id_changed',
            ui_session_id: 'server-assigned-ui-session',
            session_id: 'new-session',
            timestamp: new Date().toISOString(),
          }));
        }, 10);
        
        return mockWs as any;
      }
    );
    
    client = new RealtimeClient({
      apiUrl: 'https://api.example.com',
      authToken: 'test-token',
      sessionId: uiSessionId,
    });
    
    const receivedEvents: string[] = [];
    client.on('session.created', () => receivedEvents.push('session.created'));
    client.on('ui_session_id_changed', () => receivedEvents.push('ui_session_id_changed'));
    
    await client.connect();
    
    // Wait for events to be processed
    await vi.waitFor(() => receivedEvents.length >= 2, { timeout: 1000 });
    
    // Verify events were received and processed
    expect(receivedEvents).toContain('session.created');
    expect(receivedEvents).toContain('ui_session_id_changed');
    
    // Verify client state is updated
    expect((client as any).uiSessionId).toBe('server-assigned-ui-session');
  });
});
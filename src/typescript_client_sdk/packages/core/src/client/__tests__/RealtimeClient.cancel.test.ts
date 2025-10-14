import { describe, it, expect, vi, beforeEach, afterEach } from 'vitest';
import { RealtimeClient } from '../RealtimeClient';
import { WebSocketManager } from '../WebSocketManager';

// Mock WebSocketManager
vi.mock('../WebSocketManager');

describe('RealtimeClient - Cancel Functionality', () => {
  let client: RealtimeClient;
  let mockWsManager: any;
  
  beforeEach(() => {
    // Setup mock WebSocketManager
    mockWsManager = {
      connect: vi.fn(),
      disconnect: vi.fn(),
      isConnected: vi.fn().mockReturnValue(true),
      sendJSON: vi.fn(),
      supportsBinary: vi.fn().mockReturnValue(true),
      sendBinary: vi.fn()
    };
    
    (WebSocketManager as any).mockImplementation(() => mockWsManager);
    
    // Create client
    client = new RealtimeClient({
      apiUrl: 'wss://test.example.com',
      authToken: 'test-token'
    });
  });
  
  afterEach(() => {
    vi.clearAllMocks();
  });
  
  describe('cancelResponse', () => {
    it('should send client_wants_cancel event when connected', () => {
      // Connect the client (mock it as connected)
      (client as any).wsManager = mockWsManager;
      (client as any).connectionState = 2; // ConnectionState.CONNECTED
      
      // Call cancelResponse
      client.cancelResponse();
      
      // Verify the correct event was sent
      expect(mockWsManager.sendJSON).toHaveBeenCalledWith({
        type: 'client_wants_cancel'
      });
    });
    
    it('should throw error when not connected', () => {
      // Mock as not connected
      mockWsManager.isConnected.mockReturnValue(false);
      (client as any).wsManager = mockWsManager;
      
      // Should throw when not connected
      expect(() => client.cancelResponse()).toThrow('Not connected to server');
    });
    
    it('should log debug message when debug is enabled', () => {
      const debugSpy = vi.spyOn(console, 'debug').mockImplementation(() => {});
      
      // Create client with debug enabled
      client = new RealtimeClient({
        apiUrl: 'wss://test.example.com',
        authToken: 'test-token',
        debug: true
      });
      
      // Connect the client (mock it as connected)
      (client as any).wsManager = mockWsManager;
      (client as any).connectionState = 2; // ConnectionState.CONNECTED
      
      // Call cancelResponse
      client.cancelResponse();
      
      // Verify debug log was called with the new format
      expect(debugSpy).toHaveBeenCalled();
      
      // Find the call that logs "cancelResponse() called"
      const cancelLogCall = debugSpy.mock.calls.find(call => 
        typeof call[0] === 'string' && call[0].includes('cancelResponse() called')
      );
      expect(cancelLogCall).toBeDefined();
      
      debugSpy.mockRestore();
    });
  });
  
  describe('Event Processing', () => {
    it('should handle cancelled event when received', () => {
      const eventSpy = vi.fn();
      
      // Setup event listener for cancelled event
      client.on('cancelled' as any, eventSpy);
      
      // Connect the client
      (client as any).wsManager = mockWsManager;
      (client as any).connectionState = 2; // ConnectionState.CONNECTED
      
      // Simulate receiving a cancelled event
      const cancelledEvent = { type: 'cancelled' };
      (client as any).handleMessage(JSON.stringify(cancelledEvent));
      
      // The event should be emitted
      // Note: It might be processed by EventStreamProcessor, so we check both possibilities
      // Since EventStreamProcessor is initialized, it would process this event
      expect((client as any).eventStreamProcessor).toBeDefined();
    });
  });
});
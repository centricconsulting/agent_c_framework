/**
 * Unit tests for WebSocketManager
 * SIMPLE TESTS - Individual methods return what they should
 */
import { describe, it, expect, vi, beforeEach, afterEach } from 'vitest';
import { WebSocketManager } from '../WebSocketManager';
import { WebSocketTracker, MockWebSocket } from '../../test/mocks/websocket.mock';
import { clientEventFixtures, serverEventFixtures, audioFixtures } from '../../test/fixtures/protocol-events';

describe('WebSocketManager', () => {
  let wsTracker: WebSocketTracker;
  let mockWs: MockWebSocket;

  beforeEach(() => {
    vi.clearAllMocks();
    wsTracker = new WebSocketTracker();
    wsTracker.install();
  });

  afterEach(() => {
    wsTracker.uninstall();
    vi.restoreAllMocks();
  });

  describe('constructor', () => {
    it('should store config', () => {
      const config = { url: 'ws://test', protocols: ['p1'] };
      const manager = new WebSocketManager(config);
      
      expect(manager['options']).toEqual({
        ...config,
        binaryType: 'arraybuffer'
      });
    });

    it('should store callbacks', () => {
      const callbacks = { onOpen: vi.fn(), onClose: vi.fn() };
      const manager = new WebSocketManager({ url: 'ws://test' }, callbacks);
      
      expect(manager['callbacks']).toEqual(callbacks);
    });

    it('should default binaryType to arraybuffer', () => {
      const manager = new WebSocketManager({ url: 'ws://test' });
      
      expect(manager['options'].binaryType).toBe('arraybuffer');
    });
  });

  describe('getReadyState()', () => {
    it('should return CLOSED when no WebSocket', () => {
      const manager = new WebSocketManager({ url: 'ws://test' });
      
      expect(manager.getReadyState()).toBe(WebSocket.CLOSED);
    });

    it('should return WebSocket readyState when connected', () => {
      const manager = new WebSocketManager({ url: 'ws://test' });
      manager['ws'] = { readyState: WebSocket.OPEN } as any;
      
      expect(manager.getReadyState()).toBe(WebSocket.OPEN);
    });

    it('should return CONNECTING state', () => {
      const manager = new WebSocketManager({ url: 'ws://test' });
      manager['ws'] = { readyState: WebSocket.CONNECTING } as any;
      
      expect(manager.getReadyState()).toBe(WebSocket.CONNECTING);
    });

    it('should return CLOSING state', () => {
      const manager = new WebSocketManager({ url: 'ws://test' });
      manager['ws'] = { readyState: WebSocket.CLOSING } as any;
      
      expect(manager.getReadyState()).toBe(WebSocket.CLOSING);
    });
  });

  describe('isConnected()', () => {
    it('should return false when no WebSocket', () => {
      const manager = new WebSocketManager({ url: 'ws://test' });
      
      expect(manager.isConnected()).toBe(false);
    });

    it('should return false when CONNECTING', () => {
      const manager = new WebSocketManager({ url: 'ws://test' });
      manager['ws'] = { readyState: MockWebSocket.CONNECTING } as any;
      
      expect(manager.isConnected()).toBe(false);
    });

    it('should return false when CLOSING', () => {
      const manager = new WebSocketManager({ url: 'ws://test' });
      manager['ws'] = { readyState: MockWebSocket.CLOSING } as any;
      
      expect(manager.isConnected()).toBe(false);
    });

    it('should return false when CLOSED', () => {
      const manager = new WebSocketManager({ url: 'ws://test' });
      manager['ws'] = { readyState: MockWebSocket.CLOSED } as any;
      
      expect(manager.isConnected()).toBe(false);
    });

    it('should return true when OPEN', () => {
      const manager = new WebSocketManager({ url: 'ws://test' });
      manager['ws'] = { readyState: MockWebSocket.OPEN } as any;
      
      expect(manager.isConnected()).toBe(true);
    });
  });

  describe('send()', () => {
    it('should throw when no WebSocket', () => {
      const manager = new WebSocketManager({ url: 'ws://test' });
      
      expect(() => manager.send('test')).toThrow('WebSocket is not connected');
    });

    it('should throw when WebSocket not OPEN', () => {
      const manager = new WebSocketManager({ url: 'ws://test' });
      manager['ws'] = { readyState: MockWebSocket.CONNECTING, send: vi.fn() } as any;
      
      expect(() => manager.send('test')).toThrow('WebSocket is not open');
    });

    it('should call ws.send with string data', () => {
      const mockSend = vi.fn();
      const manager = new WebSocketManager({ url: 'ws://test' });
      manager['ws'] = { readyState: WebSocket.OPEN, send: mockSend } as any;
      
      manager.send('test data');
      
      expect(mockSend).toHaveBeenCalledWith('test data');
      expect(mockSend).toHaveBeenCalledTimes(1);
    });

    it('should call ws.send with ArrayBuffer', () => {
      const mockSend = vi.fn();
      const manager = new WebSocketManager({ url: 'ws://test' });
      manager['ws'] = { readyState: WebSocket.OPEN, send: mockSend } as any;
      
      const buffer = audioFixtures.pcm16Chunk;
      manager.send(buffer);
      
      expect(mockSend).toHaveBeenCalledWith(buffer);
      expect(mockSend).toHaveBeenCalledTimes(1);
    });

    it('should call ws.send with TypedArray', () => {
      const mockSend = vi.fn();
      const manager = new WebSocketManager({ url: 'ws://test' });
      manager['ws'] = { readyState: WebSocket.OPEN, send: mockSend } as any;
      
      const typedArray = new Uint8Array([1, 2, 3, 4]);
      manager.send(typedArray);
      
      expect(mockSend).toHaveBeenCalledWith(typedArray);
      expect(mockSend).toHaveBeenCalledTimes(1);
    });
  });

  describe('sendBinary()', () => {
    it('should throw when WebSocket does not support binary', () => {
      const manager = new WebSocketManager({ url: 'ws://test' });
      manager['ws'] = { 
        readyState: WebSocket.OPEN, 
        binaryType: 'blob',
        send: vi.fn()
      } as any;
      
      expect(() => manager.sendBinary(new ArrayBuffer(8)))
        .toThrow('WebSocket does not support binary data');
    });

    it('should send binary data when supported', () => {
      const mockSend = vi.fn();
      const manager = new WebSocketManager({ url: 'ws://test' });
      manager['ws'] = { 
        readyState: WebSocket.OPEN, 
        binaryType: 'arraybuffer',
        send: mockSend
      } as any;
      
      const buffer = audioFixtures.pcm16Chunk;
      manager.sendBinary(buffer);
      
      expect(mockSend).toHaveBeenCalledWith(buffer);
    });
  });

  describe('sendJSON()', () => {
    it('should stringify and send object', () => {
      const mockSend = vi.fn();
      const manager = new WebSocketManager({ url: 'ws://test' });
      manager['ws'] = { readyState: WebSocket.OPEN, send: mockSend } as any;
      
      const event = clientEventFixtures.textInput;
      manager.sendJSON(event);
      
      expect(mockSend).toHaveBeenCalledWith(JSON.stringify(event));
      expect(mockSend).toHaveBeenCalledTimes(1);
    });

    it('should handle null value', () => {
      const mockSend = vi.fn();
      const manager = new WebSocketManager({ url: 'ws://test' });
      manager['ws'] = { readyState: WebSocket.OPEN, send: mockSend } as any;
      
      manager.sendJSON(null);
      
      expect(mockSend).toHaveBeenCalledWith('null');
    });

    it('should handle undefined value', () => {
      const mockSend = vi.fn();
      const manager = new WebSocketManager({ url: 'ws://test' });
      manager['ws'] = { readyState: WebSocket.OPEN, send: mockSend } as any;
      
      manager.sendJSON(undefined);
      
      expect(mockSend).toHaveBeenCalledWith(undefined);
    });

    it('should handle complex nested objects', () => {
      const mockSend = vi.fn();
      const manager = new WebSocketManager({ url: 'ws://test' });
      manager['ws'] = { readyState: WebSocket.OPEN, send: mockSend } as any;
      
      const event = serverEventFixtures.agentConfigurationChanged;
      manager.sendJSON(event);
      
      expect(mockSend).toHaveBeenCalledWith(JSON.stringify(event));
    });
  });

  describe('getUrl() / setUrl()', () => {
    it('should return initial URL', () => {
      const manager = new WebSocketManager({ url: 'ws://test' });
      
      expect(manager.getUrl()).toBe('ws://test');
    });

    it('should update URL', () => {
      const manager = new WebSocketManager({ url: 'ws://test' });
      
      manager.setUrl('ws://new-url');
      
      expect(manager.getUrl()).toBe('ws://new-url');
    });

    it('should not affect existing connection when URL changes', () => {
      const manager = new WebSocketManager({ url: 'ws://test' });
      manager['ws'] = { readyState: WebSocket.OPEN } as any;
      
      manager.setUrl('ws://new-url');
      
      expect(manager['ws']).not.toBeNull();
      expect(manager.getUrl()).toBe('ws://new-url');
    });
  });

  describe('supportsBinary()', () => {
    it('should return false when no WebSocket', () => {
      const manager = new WebSocketManager({ url: 'ws://test' });
      
      expect(manager.supportsBinary()).toBe(false);
    });

    it('should return true when binaryType is arraybuffer', () => {
      const manager = new WebSocketManager({ url: 'ws://test' });
      manager['ws'] = { binaryType: 'arraybuffer' } as any;
      
      expect(manager.supportsBinary()).toBe(true);
    });

    it('should return false when binaryType is blob', () => {
      const manager = new WebSocketManager({ url: 'ws://test' });
      manager['ws'] = { binaryType: 'blob' } as any;
      
      expect(manager.supportsBinary()).toBe(false);
    });
  });

  describe('connect()', () => {
    it('should create WebSocket with URL only', () => {
      const manager = new WebSocketManager({ url: 'ws://test' });
      
      manager.connect();
      
      const instances = wsTracker.getAll();
      expect(instances).toHaveLength(1);
      expect(instances[0].url).toBe('ws://test');
    });

    it('should create WebSocket with protocols', () => {
      const manager = new WebSocketManager({ 
        url: 'ws://test',
        protocols: ['p1', 'p2']
      });
      
      manager.connect();
      
      const instances = wsTracker.getAll();
      expect(instances).toHaveLength(1);
      expect(instances[0].url).toBe('ws://test');
      expect(instances[0].protocol).toBe('p1'); // First protocol is selected
    });

    it('should set binaryType to arraybuffer', () => {
      const manager = new WebSocketManager({ url: 'ws://test' });
      
      manager.connect();
      
      mockWs = wsTracker.getLatest()!;
      expect(mockWs.binaryType).toBe('arraybuffer');
    });

    it('should call disconnect if already connected', () => {
      const manager = new WebSocketManager({ url: 'ws://test' });
      manager['ws'] = { close: vi.fn() } as any;
      manager.disconnect = vi.fn();
      
      manager.connect();
      
      expect(manager.disconnect).toHaveBeenCalled();
    });

    it('should store WebSocket instance', () => {
      const manager = new WebSocketManager({ url: 'ws://test' });
      
      manager.connect();
      
      mockWs = wsTracker.getLatest()!;
      expect(manager['ws']).toBe(mockWs);
    });

    it('should set custom binaryType from options', () => {
      const manager = new WebSocketManager({ 
        url: 'ws://test',
        binaryType: 'blob'
      });
      
      manager.connect();
      
      mockWs = wsTracker.getLatest()!;
      expect(mockWs.binaryType).toBe('blob');
    });
  });

  describe('disconnect()', () => {
    it('should do nothing when no WebSocket', () => {
      const manager = new WebSocketManager({ url: 'ws://test' });
      
      expect(() => manager.disconnect()).not.toThrow();
    });

    it('should call ws.close with default params', () => {
      const mockClose = vi.fn();
      const manager = new WebSocketManager({ url: 'ws://test' });
      manager['ws'] = { 
        close: mockClose, 
        readyState: WebSocket.OPEN,
        onopen: vi.fn(),
        onclose: vi.fn(),
        onerror: vi.fn(),
        onmessage: vi.fn()
      } as any;
      
      manager.disconnect();
      
      expect(mockClose).toHaveBeenCalledWith(1000, 'Client disconnect');
    });

    it('should call ws.close with custom code and reason', () => {
      const mockClose = vi.fn();
      const manager = new WebSocketManager({ url: 'ws://test' });
      manager['ws'] = { 
        close: mockClose, 
        readyState: WebSocket.OPEN,
        onopen: vi.fn(),
        onclose: vi.fn(),
        onerror: vi.fn(),
        onmessage: vi.fn()
      } as any;
      
      manager.disconnect(1001, 'Going away');
      
      expect(mockClose).toHaveBeenCalledWith(1001, 'Going away');
    });

    it('should clear heartbeat interval', () => {
      const clearIntervalSpy = vi.spyOn(global, 'clearInterval');
      const manager = new WebSocketManager({ url: 'ws://test' });
      manager['pingInterval'] = 123 as any;
      
      manager.disconnect();
      
      expect(clearIntervalSpy).toHaveBeenCalledWith(123);
      expect(manager['pingInterval']).toBeNull();
    });

    it('should clear pong timeout', () => {
      const clearTimeoutSpy = vi.spyOn(global, 'clearTimeout');
      const manager = new WebSocketManager({ url: 'ws://test' });
      manager['pongTimeout'] = 456 as any;
      
      manager.disconnect();
      
      expect(clearTimeoutSpy).toHaveBeenCalledWith(456);
      expect(manager['pongTimeout']).toBeNull();
    });

    it('should set ws to null', () => {
      const manager = new WebSocketManager({ url: 'ws://test' });
      manager['ws'] = { 
        close: vi.fn(),
        readyState: WebSocket.OPEN,
        onopen: null,
        onclose: null,
        onerror: null,
        onmessage: null
      } as any;
      
      manager.disconnect();
      
      expect(manager['ws']).toBeNull();
    });

    it('should clear all event handlers', () => {
      const manager = new WebSocketManager({ url: 'ws://test' });
      const ws = { 
        close: vi.fn(),
        readyState: WebSocket.OPEN,
        onopen: vi.fn(),
        onclose: vi.fn(),
        onerror: vi.fn(),
        onmessage: vi.fn()
      } as any;
      manager['ws'] = ws;
      
      manager.disconnect();
      
      expect(ws.onopen).toBeNull();
      expect(ws.onclose).toBeNull();
      expect(ws.onerror).toBeNull();
      expect(ws.onmessage).toBeNull();
    });

    it('should not call close when already CLOSED', () => {
      const mockClose = vi.fn();
      const manager = new WebSocketManager({ url: 'ws://test' });
      manager['ws'] = { 
        close: mockClose, 
        readyState: MockWebSocket.CLOSED,
        onopen: null,
        onclose: null,
        onerror: null,
        onmessage: null
      } as any;
      
      manager.disconnect();
      
      // WebSocketManager checks readyState and doesn't call close if not OPEN
      expect(mockClose).not.toHaveBeenCalled();
    });
  });

  describe('event handler setup', () => {
    it('should attach onopen handler', () => {
      const onOpen = vi.fn();
      const manager = new WebSocketManager(
        { url: 'ws://test' },
        { onOpen }
      );
      
      manager.connect();
      mockWs = wsTracker.getLatest()!;
      
      // Simulate open event
      const event = new Event('open');
      mockWs.onopen!(event);
      
      expect(onOpen).toHaveBeenCalledWith(event);
    });

    it('should attach onclose handler', () => {
      const onClose = vi.fn();
      const manager = new WebSocketManager(
        { url: 'ws://test' },
        { onClose }
      );
      
      manager.connect();
      mockWs = wsTracker.getLatest()!;
      
      // Simulate close event - CloseEvent not available in test env, use Event
      const event = { type: 'close', code: 1000, reason: 'Normal' } as any;
      mockWs.onclose!(event);
      
      expect(onClose).toHaveBeenCalledWith(event);
    });

    it('should attach onerror handler', () => {
      const onError = vi.fn();
      const manager = new WebSocketManager(
        { url: 'ws://test' },
        { onError }
      );
      
      manager.connect();
      mockWs = wsTracker.getLatest()!;
      
      // Simulate error event
      const event = new Event('error');
      mockWs.onerror!(event);
      
      expect(onError).toHaveBeenCalledWith(event);
    });

    it('should attach onmessage handler for text', () => {
      const onMessage = vi.fn();
      const manager = new WebSocketManager(
        { url: 'ws://test' },
        { onMessage }
      );
      
      manager.connect();
      mockWs = wsTracker.getLatest()!;
      
      // Simulate text message
      const data = JSON.stringify(serverEventFixtures.textDelta);
      const event = new MessageEvent('message', { data });
      mockWs.onmessage!(event);
      
      expect(onMessage).toHaveBeenCalledWith(data);
    });

    it('should attach onmessage handler for binary', () => {
      const onMessage = vi.fn();
      const manager = new WebSocketManager(
        { url: 'ws://test' },
        { onMessage }
      );
      
      manager.connect();
      mockWs = wsTracker.getLatest()!;
      
      // Simulate binary message
      const data = audioFixtures.audioOutputFrame;
      const event = new MessageEvent('message', { data });
      mockWs.onmessage!(event);
      
      expect(onMessage).toHaveBeenCalledWith(data);
    });
  });

  describe('heartbeat mechanism', () => {
    beforeEach(() => {
      vi.useFakeTimers();
    });

    afterEach(() => {
      vi.useRealTimers();
    });

    it('should not start heartbeat without pingInterval', () => {
      const setIntervalSpy = vi.spyOn(global, 'setInterval');
      const manager = new WebSocketManager({ url: 'ws://test' });
      
      manager.connect();
      mockWs = wsTracker.getLatest()!;
      mockWs.onopen!(new Event('open'));
      
      expect(setIntervalSpy).not.toHaveBeenCalled();
    });

    it('should start heartbeat with pingInterval', () => {
      const setIntervalSpy = vi.spyOn(global, 'setInterval');
      const manager = new WebSocketManager({ 
        url: 'ws://test',
        pingInterval: 30000
      });
      
      manager.connect();
      mockWs = wsTracker.getLatest()!;
      mockWs.onopen!(new Event('open'));
      
      expect(setIntervalSpy).toHaveBeenCalledWith(expect.any(Function), 30000);
    });

    it('should send ping message on interval', () => {
      const manager = new WebSocketManager({ 
        url: 'ws://test',
        pingInterval: 30000
      });
      
      manager.connect();
      mockWs = wsTracker.getLatest()!;
      
      // Make sure WebSocket is open
      mockWs.readyState = MockWebSocket.OPEN;
      mockWs.onopen!(new Event('open'));
      
      // Advance timer to trigger ping
      vi.advanceTimersByTime(30000);
      
      expect(mockWs.send).toHaveBeenCalledWith(JSON.stringify({ type: 'ping' }));
    });

    it('should reset isAlive on any message', () => {
      const manager = new WebSocketManager({ 
        url: 'ws://test',
        pingInterval: 30000
      });
      
      manager.connect();
      mockWs = wsTracker.getLatest()!;
      mockWs.onopen!(new Event('open'));
      
      // Set isAlive to false
      manager['isAlive'] = false;
      
      // Receive a message
      const event = new MessageEvent('message', { 
        data: JSON.stringify(serverEventFixtures.pong)
      });
      mockWs.onmessage!(event);
      
      expect(manager['isAlive']).toBe(true);
    });

    it('should disconnect on ping timeout', () => {
      const manager = new WebSocketManager({ 
        url: 'ws://test',
        pingInterval: 30000
      });
      manager.disconnect = vi.fn();
      
      manager.connect();
      mockWs = wsTracker.getLatest()!;
      mockWs.onopen!(new Event('open'));
      
      // First ping sets isAlive to false
      vi.advanceTimersByTime(30000);
      
      // Second ping checks isAlive and disconnects
      vi.advanceTimersByTime(30000);
      
      expect(manager.disconnect).toHaveBeenCalledWith(4000, 'Ping timeout');
    });

    it('should set pong timeout when configured', () => {
      const manager = new WebSocketManager({ 
        url: 'ws://test',
        pingInterval: 30000,
        pongTimeout: 5000
      });
      
      manager.connect();
      mockWs = wsTracker.getLatest()!;
      
      // Clear existing setTimeout calls from mock auto-open
      vi.clearAllTimers();
      const setTimeoutSpy = vi.spyOn(global, 'setTimeout');
      
      // Make sure WebSocket is open
      mockWs.readyState = MockWebSocket.OPEN;
      mockWs.onopen!(new Event('open'));
      
      // Trigger ping
      vi.advanceTimersByTime(30000);
      
      expect(setTimeoutSpy).toHaveBeenCalledWith(expect.any(Function), 5000);
    });
  });
});
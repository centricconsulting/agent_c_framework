/**
 * Example test file showing how to use the mocks
 * This file demonstrates best practices for using our simple mocks
 */
import { describe, it, expect, beforeEach, afterEach, vi } from 'vitest';
import {
  WebSocketTracker,
  MockWebSocket,
  mockAudioContext,
  MockAudioContextConstructor,
  mockNavigator,
  MockAbortControllerConstructor,
  resetAllMocks,
} from './index';

describe('Mock Usage Examples', () => {
  let wsTracker: WebSocketTracker;
  let mockWs: MockWebSocket;

  beforeEach(() => {
    // Setup WebSocket tracker
    wsTracker = new WebSocketTracker();
    wsTracker.install();
    
    // Setup other global mocks
    global.AudioContext = MockAudioContextConstructor as unknown as typeof AudioContext;
    global.navigator = mockNavigator as unknown as Navigator;
    global.AbortController = MockAbortControllerConstructor as unknown as typeof AbortController;
  });

  afterEach(() => {
    // Cleanup WebSocket tracker
    wsTracker.uninstall();
    
    // Reset all mocks after each test
    resetAllMocks();
    vi.restoreAllMocks();
  });

  describe('WebSocket Mock', () => {
    it('should track WebSocket instantiation', () => {
      const ws = new WebSocket('ws://test.example.com');
      
      const instances = wsTracker.getAll();
      expect(instances).toHaveLength(1);
      expect(instances[0].url).toBe('ws://test.example.com');
      expect(ws).toBe(instances[0]);
    });

    it('should track send calls', () => {
      const ws = new WebSocket('ws://test.example.com');
      mockWs = wsTracker.getLatest()!;
      const message = JSON.stringify({ type: 'test', data: 'hello' });
      
      // Wait for WebSocket to be open
      mockWs.readyState = MockWebSocket.OPEN;
      ws.send(message);
      
      expect(mockWs.send).toHaveBeenCalledWith(message);
      expect(mockWs.send).toHaveBeenCalledTimes(1);
    });

    it('should allow simulating events', () => {
      const ws = new WebSocket('ws://test.example.com');
      mockWs = wsTracker.getLatest()!;
      const onMessage = vi.fn();
      
      // Set up event handler
      ws.onmessage = onMessage;
      
      // Open the WebSocket first
      mockWs.simulateOpen();
      
      // Simulate receiving a message
      mockWs.simulateTextMessage({ type: 'response' });
      
      expect(onMessage).toHaveBeenCalled();
    });

    it('should track connection state', () => {
      const ws = new WebSocket('ws://test.example.com');
      mockWs = wsTracker.getLatest()!;
      
      // Check initial state (CONNECTING)
      expect(ws.readyState).toBe(WebSocket.CONNECTING);
      
      // Simulate opening
      mockWs.simulateOpen();
      expect(ws.readyState).toBe(WebSocket.OPEN);
      
      // Simulate closing
      mockWs.close();
      // Need to wait for async close
      return new Promise<void>((resolve) => {
        setTimeout(() => {
          expect(ws.readyState).toBe(WebSocket.CLOSED);
          resolve();
        }, 10);
      });
    });
  });

  describe('AudioContext Mock', () => {
    it('should track AudioContext creation', () => {
      const ctx = new AudioContext();
      expect(MockAudioContextConstructor).toHaveBeenCalled();
      expect(ctx).toBe(mockAudioContext);
    });

    it('should track node creation', () => {
      const ctx = new AudioContext();
      
      const gain = ctx.createGain();
      const buffer = ctx.createBuffer(2, 44100, 44100);
      const source = ctx.createBufferSource();
      
      expect(mockAudioContext.createGain).toHaveBeenCalled();
      expect(mockAudioContext.createBuffer).toHaveBeenCalledWith(2, 44100, 44100);
      expect(mockAudioContext.createBufferSource).toHaveBeenCalled();
      
      // Nodes can be used
      expect(gain.connect).toBeDefined();
      expect(source.start).toBeDefined();
    });

    it('should handle async operations', async () => {
      const ctx = new AudioContext();
      
      // Configure mock response
      mockAudioContext.resume.mockResolvedValue(undefined);
      
      await ctx.resume();
      
      expect(mockAudioContext.resume).toHaveBeenCalled();
    });
  });

  describe('MediaStream Mock', () => {
    it('should mock getUserMedia', async () => {
      const constraints = { audio: true, video: false };
      
      const stream = await navigator.mediaDevices.getUserMedia(constraints);
      
      expect(mockNavigator.mediaDevices.getUserMedia).toHaveBeenCalledWith(constraints);
      expect(stream.getTracks()).toHaveLength(1);
      expect(stream.getAudioTracks()).toHaveLength(1);
    });

    it('should allow track manipulation', async () => {
      const stream = await mockNavigator.mediaDevices.getUserMedia({ audio: true });
      const tracks = stream.getTracks();
      
      tracks[0].stop();
      
      expect(tracks[0].stop).toHaveBeenCalled();
    });

    it('should handle getUserMedia rejection', async () => {
      // Configure mock to reject
      mockNavigator.mediaDevices.getUserMedia.mockRejectedValue(
        new Error('Permission denied')
      );
      
      await expect(
        navigator.mediaDevices.getUserMedia({ audio: true })
      ).rejects.toThrow('Permission denied');
    });
  });

  describe('AbortController Mock', () => {
    it('should track abort signals', () => {
      const controller = new AbortController();
      
      expect(controller.signal.aborted).toBe(false);
      
      controller.abort('User cancelled');
      
      expect(controller.signal.aborted).toBe(true);
      expect(controller.signal.reason).toBe('User cancelled');
    });
  });

  describe('Combined Usage', () => {
    it('should work with multiple mocks together', async () => {
      // Create audio context
      const ctx = new AudioContext();
      
      // Get user media
      const stream = await navigator.mediaDevices.getUserMedia({ audio: true });
      
      // Create source from stream
      const source = ctx.createMediaStreamSource(stream);
      
      // Create WebSocket
      const ws = new WebSocket('ws://audio.test');
      
      // Verify all mocks were called
      expect(MockAudioContextConstructor).toHaveBeenCalled();
      expect(mockNavigator.mediaDevices.getUserMedia).toHaveBeenCalled();
      expect(mockAudioContext.createMediaStreamSource).toHaveBeenCalledWith(stream);
      
      // Verify WebSocket was created
      const instances = wsTracker.getAll();
      expect(instances).toHaveLength(1);
      expect(instances[0].url).toBe('ws://audio.test');
      
      // All objects are mocked
      expect(source.connect).toBeDefined();
      expect(ws.send).toBeDefined();
    });
  });
});
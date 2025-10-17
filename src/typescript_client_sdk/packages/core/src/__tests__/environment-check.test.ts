import { describe, it, expect, vi, beforeEach } from 'vitest';

describe('Environment Configuration Check', () => {
  describe('Browser API Mocking', () => {
    beforeEach(() => {
      vi.clearAllMocks();
    });

    it('should be able to mock navigator.mediaDevices', () => {
      // This test verifies that browser APIs are available for mocking
      const mockGetUserMedia = vi.fn().mockResolvedValue({
        getTracks: () => [],
        getAudioTracks: () => [],
        getVideoTracks: () => []
      });

      // Mock navigator.mediaDevices
      Object.defineProperty(global.navigator, 'mediaDevices', {
        value: {
          getUserMedia: mockGetUserMedia
        },
        writable: true,
        configurable: true
      });

      expect(navigator.mediaDevices).toBeDefined();
      expect(navigator.mediaDevices.getUserMedia).toBe(mockGetUserMedia);
    });

    it('should be able to mock AudioContext', () => {
      // Mock AudioContext constructor
      const mockAudioContext = {
        createMediaStreamSource: vi.fn(),
        createScriptProcessor: vi.fn(),
        destination: {},
        close: vi.fn()
      };

      global.AudioContext = vi.fn(() => mockAudioContext) as any;

      const context = new AudioContext();
      expect(context).toBeDefined();
      expect(context.createMediaStreamSource).toBeDefined();
    });

    it('should be able to mock WebSocket', () => {
      // Mock WebSocket constructor
      const mockWebSocket = {
        send: vi.fn(),
        close: vi.fn(),
        addEventListener: vi.fn(),
        removeEventListener: vi.fn(),
        readyState: 1 // WebSocket.OPEN
      };

      global.WebSocket = vi.fn(() => mockWebSocket) as any;

      const ws = new WebSocket('ws://test');
      expect(ws).toBeDefined();
      expect(ws.send).toBeDefined();
    });

    it('should have window object available', () => {
      expect(window).toBeDefined();
    });

    it('should have document object available', () => {
      expect(document).toBeDefined();
    });
  });
});
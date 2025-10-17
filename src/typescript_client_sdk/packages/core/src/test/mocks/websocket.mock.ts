/**
 * Enhanced WebSocket Mock for Event Stream Testing
 * Provides realistic WebSocket behavior with event sequencing support
 */
import { vi } from 'vitest';
import { EventEmitter } from 'events';

/**
 * MockWebSocket class that simulates real WebSocket behavior
 * Extends EventEmitter to provide proper event dispatching
 */
export class MockWebSocket extends EventEmitter {
  static CONNECTING = 0;
  static OPEN = 1;
  static CLOSING = 2;
  static CLOSED = 3;

  public readyState: number = MockWebSocket.CONNECTING;
  public binaryType: BinaryType = 'arraybuffer';
  public bufferedAmount: number = 0;
  public extensions: string = '';
  public protocol: string = '';
  public url: string;

  // Mock functions with proper behavior simulation
  public send = vi.fn((data: string | ArrayBuffer | Blob) => {
    if (this.readyState !== MockWebSocket.OPEN) {
      throw new Error('WebSocket is not open');
    }
    
    // Simulate buffering
    if (data instanceof ArrayBuffer) {
      this.bufferedAmount += data.byteLength;
    } else if (typeof data === 'string') {
      this.bufferedAmount += data.length;
    }
    
    // Clear buffer after a tick
    setTimeout(() => {
      this.bufferedAmount = 0;
    }, 0);
  });

  public close = vi.fn((code?: number, reason?: string) => {
    if (this.readyState === MockWebSocket.CLOSED) return;
    
    this.readyState = MockWebSocket.CLOSING;
    
    setTimeout(() => {
      this.readyState = MockWebSocket.CLOSED;
      const event = new CloseEvent('close', {
        code: code || 1000,
        reason: reason || '',
        wasClean: true
      });
      this.dispatchEvent(event);
      this.onclose?.(event);
    }, 0);
  });

  // Event handlers
  public onopen: ((event: Event) => void) | null = null;
  public onclose: ((event: CloseEvent) => void) | null = null;
  public onerror: ((event: Event) => void) | null = null;
  public onmessage: ((event: MessageEvent) => void) | null = null;

  constructor(url: string, protocols?: string | string[]) {
    super();
    this.url = url;
    
    if (protocols) {
      this.protocol = Array.isArray(protocols) ? protocols[0] : protocols;
    }
    
    // Simulate connection establishment
    setTimeout(() => {
      if (this.readyState === MockWebSocket.CONNECTING) {
        this.readyState = MockWebSocket.OPEN;
        const event = new Event('open');
        this.dispatchEvent(event);
        this.onopen?.(event);
      }
    }, 0);
  }

  /**
   * Simulates receiving a JSON message from the server
   * @param data - The data to simulate (will be JSON.stringified if object)
   */
  simulateMessage(data: any) {
    if (this.readyState !== MockWebSocket.OPEN) {
      throw new Error('Cannot simulate message on closed WebSocket');
    }

    const event = new MessageEvent('message', {
      data: typeof data === 'object' ? JSON.stringify(data) : data,
      origin: this.url
    });
    
    this.dispatchEvent(event);
    this.onmessage?.(event);
  }

  /**
   * Alias for simulateMessage for compatibility
   */
  simulateTextMessage(data: any) {
    return this.simulateMessage(data);
  }

  /**
   * Simulates the WebSocket opening
   */
  simulateOpen() {
    this.readyState = MockWebSocket.OPEN;
    const event = new Event('open');
    this.dispatchEvent(event);
    this.onopen?.(event);
  }

  /**
   * Simulates receiving a binary message from the server
   * @param data - ArrayBuffer containing binary data
   */
  simulateBinaryMessage(data: ArrayBuffer) {
    if (this.readyState !== MockWebSocket.OPEN) {
      throw new Error('Cannot simulate message on closed WebSocket');
    }

    const event = new MessageEvent('message', {
      data: data,
      origin: this.url
    });
    
    this.dispatchEvent(event);
    this.onmessage?.(event);
  }

  /**
   * Simulates a WebSocket error
   * @param message - Error message for debugging
   */
  simulateError(message: string = 'WebSocket error') {
    const event = new Event('error');
    (event as any).message = message;
    this.dispatchEvent(event);
    this.onerror?.(event);
  }

  /**
   * Simulates closing the WebSocket connection
   * @param code - Close code (default: 1000 for normal closure)
   * @param reason - Close reason string
   */
  simulateClose(code: number = 1000, reason: string = '') {
    this.readyState = MockWebSocket.CLOSED;
    const event = new CloseEvent('close', { code, reason, wasClean: true });
    this.dispatchEvent(event);
    this.onclose?.(event);
  }

  // EventTarget implementation
  addEventListener(type: string, listener: EventListener): void {
    super.on(type, listener);
  }

  removeEventListener(type: string, listener: EventListener): void {
    super.off(type, listener);
  }

  dispatchEvent(event: Event): boolean {
    super.emit(event.type, event);
    return true;
  }
}

/**
 * Helper function to install WebSocket mock globally
 * Replaces global.WebSocket with MockWebSocket
 */
export function installWebSocketMock() {
  vi.stubGlobal('WebSocket', MockWebSocket);
}

/**
 * WebSocketTracker - Utility class for tracking WebSocket instances in tests
 * Allows for easy cleanup and instance management
 */
export class WebSocketTracker {
  private instances: MockWebSocket[] = [];
  private OriginalWebSocket: any;

  /**
   * Installs the mock WebSocket globally and starts tracking instances
   */
  install() {
    this.OriginalWebSocket = (global as any).WebSocket;
    
    // Use vi.stubGlobal to properly mock the global WebSocket
    const MockConstructor = vi.fn((url: string, protocols?: string | string[]) => {
      const instance = new MockWebSocket(url, protocols);
      this.instances.push(instance);
      return instance;
    }) as any;
    
    // Add the static constants to the constructor
    MockConstructor.CONNECTING = MockWebSocket.CONNECTING;
    MockConstructor.OPEN = MockWebSocket.OPEN;
    MockConstructor.CLOSING = MockWebSocket.CLOSING;
    MockConstructor.CLOSED = MockWebSocket.CLOSED;
    
    // Properly stub the global WebSocket using vitest utilities
    vi.stubGlobal('WebSocket', MockConstructor);
  }

  /**
   * Uninstalls the mock and restores the original WebSocket
   */
  uninstall() {
    // Use vi.unstubAllGlobals to properly restore globals
    vi.unstubAllGlobals();
    this.instances = [];
  }

  /**
   * Gets the most recently created WebSocket instance
   */
  getLatest(): MockWebSocket | undefined {
    return this.instances[this.instances.length - 1];
  }

  /**
   * Gets all tracked WebSocket instances
   */
  getAll(): MockWebSocket[] {
    return this.instances;
  }

  /**
   * Clears all tracked instances
   */
  clear() {
    this.instances = [];
  }
}

// Backward compatibility exports
export const mockWebSocket = new MockWebSocket('ws://test');
export const MockWebSocketConstructor = MockWebSocket;

/**
 * Helper to reset WebSocket mock state
 * @deprecated Use WebSocketTracker for better instance management
 */
export const resetWebSocketMock = () => {
  mockWebSocket.send.mockReset();
  mockWebSocket.close.mockReset();
  mockWebSocket.onopen = null;
  mockWebSocket.onclose = null;
  mockWebSocket.onerror = null;
  mockWebSocket.onmessage = null;
  mockWebSocket.readyState = MockWebSocket.OPEN;
  mockWebSocket.binaryType = 'arraybuffer';
};
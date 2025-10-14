/**
 * Mock WebSocket implementation for testing
 */

import { vi } from 'vitest';
import { EventEmitter } from 'events';

export class MockWebSocket extends EventEmitter {
  static CONNECTING = 0;
  static OPEN = 1;
  static CLOSING = 2;
  static CLOSED = 3;

  CONNECTING = MockWebSocket.CONNECTING;
  OPEN = MockWebSocket.OPEN;
  CLOSING = MockWebSocket.CLOSING;
  CLOSED = MockWebSocket.CLOSED;

  url: string;
  readyState: number = MockWebSocket.CONNECTING;
  binaryType: 'blob' | 'arraybuffer' = 'blob';
  
  // Event handler properties that WebSocketManager uses
  onopen: ((event: Event) => void) | null = null;
  onclose: ((event: CloseEvent) => void) | null = null;
  onerror: ((event: Event) => void) | null = null;
  onmessage: ((event: MessageEvent) => void) | null = null;
  
  send = vi.fn((data: any) => {
    // Simulate successful send
    if (this.readyState !== MockWebSocket.OPEN) {
      throw new Error('WebSocket is not open');
    }
  });
  
  close = vi.fn((code?: number, reason?: string) => {
    if (this.readyState === MockWebSocket.OPEN || this.readyState === MockWebSocket.CONNECTING) {
      this.readyState = MockWebSocket.CLOSED;
      this.isConnected = false;
      const closeEvent = { type: 'close', target: this, code: code || 1000, reason: reason || 'Normal closure' } as CloseEvent;
      if (this.onclose) {
        this.onclose(closeEvent);
      }
      this.emit('close', closeEvent);
    }
  });

  private messageQueue: any[] = [];
  private isConnected = false;

  constructor(url: string, protocols?: string | string[]) {
    super();
    this.url = url;
    
    // Simulate connection delay
    setTimeout(() => {
      if (this.readyState === MockWebSocket.CONNECTING) {
        this.connect();
      }
    }, 0);
  }

  connect() {
    this.readyState = MockWebSocket.OPEN;
    this.isConnected = true;
    const openEvent = { type: 'open', target: this } as Event;
    if (this.onopen) {
      this.onopen(openEvent);
    }
    this.emit('open', openEvent);
    
    // Process any queued messages
    while (this.messageQueue.length > 0) {
      const message = this.messageQueue.shift();
      this.receiveMessage(message);
    }
  }

  disconnect() {
    if (this.readyState === MockWebSocket.OPEN) {
      this.readyState = MockWebSocket.CLOSED;
      this.isConnected = false;
      const closeEvent = { type: 'close', target: this, code: 1000, reason: 'Normal closure' } as CloseEvent;
      if (this.onclose) {
        this.onclose(closeEvent);
      }
      this.emit('close', closeEvent);
    }
  }

  simulateError(error: Error) {
    const errorEvent = { type: 'error', target: this, error } as Event;
    if (this.onerror) {
      this.onerror(errorEvent);
    }
    this.emit('error', errorEvent);
  }

  receiveMessage(data: any) {
    if (this.isConnected) {
      const event = { type: 'message', target: this, data } as MessageEvent;
      if (this.onmessage) {
        this.onmessage(event);
      }
      this.emit('message', event);
    } else {
      this.messageQueue.push(data);
    }
  }

  receiveBinaryMessage(data: ArrayBuffer) {
    if (this.isConnected) {
      const event = { type: 'message', target: this, data } as MessageEvent;
      if (this.onmessage) {
        this.onmessage(event);
      }
      this.emit('message', event);
    } else {
      this.messageQueue.push(data);
    }
  }

  addEventListener(event: string, handler: Function) {
    this.on(event, handler as any);
  }

  removeEventListener(event: string, handler: Function) {
    this.off(event, handler as any);
  }
}

export function createMockWebSocket(url?: string) {
  return new MockWebSocket(url || 'ws://localhost:8080');
}

export function mockWebSocketConstructor() {
  const MockWebSocketClass = vi.fn().mockImplementation((url: string, protocols?: string | string[]) => {
    return createMockWebSocket(url);
  });
  
  // Add static properties
  MockWebSocketClass.CONNECTING = 0;
  MockWebSocketClass.OPEN = 1;
  MockWebSocketClass.CLOSING = 2;
  MockWebSocketClass.CLOSED = 3;
  
  return MockWebSocketClass;
}
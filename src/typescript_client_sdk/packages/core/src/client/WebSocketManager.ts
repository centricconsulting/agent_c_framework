/**
 * WebSocket Manager - Low-level WebSocket handling for the Realtime Client
 */

export interface WebSocketManagerOptions {
  url: string;
  protocols?: string[];
  binaryType?: BinaryType;
  pingInterval?: number;
  pongTimeout?: number;
}

export interface WebSocketManagerCallbacks {
  onOpen?: (event: Event) => void;
  onClose?: (event: CloseEvent) => void;
  onError?: (event: Event) => void;
  onMessage?: (data: string | ArrayBuffer) => void;
}

/**
 * Manages WebSocket connection lifecycle and message handling
 */
export class WebSocketManager {
  private ws: WebSocket | null = null;
  private options: WebSocketManagerOptions;
  private callbacks: WebSocketManagerCallbacks;
  private pingInterval: ReturnType<typeof setInterval> | null = null;
  private pongTimeout: ReturnType<typeof setTimeout> | null = null;
  private isAlive = false;

  constructor(options: WebSocketManagerOptions, callbacks: WebSocketManagerCallbacks = {}) {
    this.options = {
      ...options,
      // Ensure binaryType is set to 'arraybuffer' for proper binary handling
      binaryType: options.binaryType || 'arraybuffer'
    };
    this.callbacks = callbacks;
  }

  /**
   * Connect to the WebSocket server
   */
  connect(): void {
    if (this.ws) {
      this.disconnect();
    }

    try {
      this.ws = new WebSocket(this.options.url, this.options.protocols);
      
      // Always set binaryType to ensure proper binary handling
      this.ws.binaryType = this.options.binaryType || 'arraybuffer';

      this.setupEventHandlers();
    } catch (error) {
      console.error('Failed to create WebSocket:', error);
      throw error;
    }
  }

  /**
   * Disconnect from the WebSocket server
   */
  disconnect(code = 1000, reason = 'Client disconnect'): void {
    this.stopHeartbeat();
    
    if (this.ws) {
      // Remove event handlers to prevent memory leaks
      this.ws.onopen = null;
      this.ws.onclose = null;
      this.ws.onerror = null;
      this.ws.onmessage = null;

      if (this.ws.readyState === WebSocket.OPEN) {
        this.ws.close(code, reason);
      }
      
      this.ws = null;
    }
  }

  /**
   * Send data through the WebSocket
   */
  send(data: string | ArrayBuffer | ArrayBufferView): void {
    if (!this.ws) {
      throw new Error('WebSocket is not connected');
    }

    if (this.ws.readyState !== WebSocket.OPEN) {
      throw new Error('WebSocket is not open');
    }

    // Log bufferedAmount before send for critical events
    const dataStr = typeof data === 'string' ? data : null;
    if (dataStr && dataStr.includes('client_wants_cancel')) {
      console.debug(`[${Date.now()}] WebSocket.send() - bufferedAmount before: ${this.ws.bufferedAmount} bytes`);
    }

    this.ws.send(data);
    
    // Log bufferedAmount after send for critical events
    if (dataStr && dataStr.includes('client_wants_cancel')) {
      console.debug(`[${Date.now()}] WebSocket.send() - bufferedAmount after: ${this.ws.bufferedAmount} bytes`);
      if (this.ws.bufferedAmount > 0) {
        console.warn(`[${Date.now()}] WARNING: WebSocket has ${this.ws.bufferedAmount} bytes buffered - cancel may be delayed!`);
      }
    }
  }

  /**
   * Send binary data through the WebSocket
   * This is a convenience method that ensures the data is sent as binary
   */
  sendBinary(data: ArrayBuffer | ArrayBufferView): void {
    if (!this.supportsBinary()) {
      throw new Error('WebSocket does not support binary data');
    }
    this.send(data);
  }

  /**
   * Send JSON data through the WebSocket
   */
  sendJSON(data: unknown): void {
    const jsonString = JSON.stringify(data);
    
    // Debug logging for cancel events
    if (typeof data === 'object' && data !== null && 'type' in data && data.type === 'client_wants_cancel') {
      console.debug(`[${Date.now()}] WebSocketManager.sendJSON: Sending client_wants_cancel`);
      console.debug(`[${Date.now()}] WebSocket bufferedAmount before send: ${this.ws?.bufferedAmount}`);
    }
    
    this.send(jsonString);
    
    if (typeof data === 'object' && data !== null && 'type' in data && data.type === 'client_wants_cancel') {
      console.debug(`[${Date.now()}] WebSocket bufferedAmount after send: ${this.ws?.bufferedAmount}`);
    }
  }

  /**
   * Get the current WebSocket ready state
   */
  getReadyState(): number {
    return this.ws ? this.ws.readyState : WebSocket.CLOSED;
  }

  /**
   * Check if the WebSocket is connected
   */
  isConnected(): boolean {
    return this.ws !== null && this.ws.readyState === WebSocket.OPEN;
  }

  /**
   * Check if the WebSocket connection supports binary data
   */
  supportsBinary(): boolean {
    return this.ws !== null && this.ws.binaryType === 'arraybuffer';
  }

  /**
   * Setup WebSocket event handlers
   */
  private setupEventHandlers(): void {
    if (!this.ws) return;

    this.ws.onopen = (event) => {
      this.isAlive = true;
      this.startHeartbeat();
      this.callbacks.onOpen?.(event);
    };

    this.ws.onclose = (event) => {
      this.stopHeartbeat();
      this.callbacks.onClose?.(event);
    };

    this.ws.onerror = (event) => {
      this.callbacks.onError?.(event);
    };

    this.ws.onmessage = (event) => {
      // Reset heartbeat on any message
      this.isAlive = true;

      // Handle both text and binary messages
      if (typeof event.data === 'string') {
        // JSON message
        this.callbacks.onMessage?.(event.data);
      } else if (event.data instanceof Blob) {
        // Binary message (Blob in browser)
        event.data.arrayBuffer().then((buffer) => {
          this.callbacks.onMessage?.(buffer);
        }).catch((error) => {
          console.error('Failed to convert Blob to ArrayBuffer:', error);
        });
      } else if (event.data instanceof ArrayBuffer) {
        // Binary message (ArrayBuffer)
        this.callbacks.onMessage?.(event.data);
      } else {
        console.warn('Unknown message type:', typeof event.data);
      }
    };
  }

  /**
   * Start heartbeat ping/pong mechanism
   */
  private startHeartbeat(): void {
    if (!this.options.pingInterval) return;

    this.pingInterval = setInterval(() => {
      if (!this.isAlive) {
        // Connection is dead, close it
        this.disconnect(4000, 'Ping timeout');
        return;
      }

      this.isAlive = false;
      
      // Send ping frame if possible
      if (this.ws && this.ws.readyState === WebSocket.OPEN) {
        try {
          // Send a ping message (could be a specific message type in your protocol)
          this.sendJSON({ type: 'ping' });
          
          // Set timeout for pong response
          if (this.options.pongTimeout) {
            this.pongTimeout = setTimeout(() => {
              if (!this.isAlive) {
                this.disconnect(4000, 'Pong timeout');
              }
            }, this.options.pongTimeout);
          }
        } catch (error) {
          console.error('Failed to send ping:', error);
        }
      }
    }, this.options.pingInterval);
  }

  /**
   * Stop heartbeat mechanism
   */
  private stopHeartbeat(): void {
    if (this.pingInterval) {
      clearInterval(this.pingInterval);
      this.pingInterval = null;
    }
    
    if (this.pongTimeout) {
      clearTimeout(this.pongTimeout);
      this.pongTimeout = null;
    }
  }

  /**
   * Get the WebSocket URL
   */
  getUrl(): string {
    return this.options.url;
  }

  /**
   * Update the WebSocket URL (requires reconnection)
   */
  setUrl(url: string): void {
    this.options.url = url;
  }

  /**
   * Get the current WebSocket bufferedAmount
   * This indicates how many bytes are queued to be sent
   */
  getBufferedAmount(): number {
    return this.ws?.bufferedAmount ?? 0;
  }

  /**
   * Check if the WebSocket has buffered data
   */
  hasBufferedData(): boolean {
    return this.getBufferedAmount() > 0;
  }
}
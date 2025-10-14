/**
 * Realtime Client Test Utilities
 * Helper functions for testing RealtimeClient and related components
 */

import { vi } from 'vitest';
import type { RealtimeClient } from '../../client/RealtimeClient';
import type { RealtimeClientConfig } from '../../client/ClientConfig';
import { MockWebSocket, WebSocketMock } from '../mocks/mock-websocket';
import { MockAudioContext } from '../mocks/audio-mocks';
import { EventEmitter } from '../../events/EventEmitter';
import type { EventTypes } from '../../events/EventRegistry';

/**
 * Default test configuration for RealtimeClient
 */
export const defaultTestConfig: RealtimeClientConfig = {
  apiUrl: 'ws://localhost:8080/test',
  authToken: 'test-token',
  autoReconnect: false,
  reconnection: {
    maxAttempts: 3,
    delay: 100,
    maxDelay: 1000,
    backoffMultiplier: 1.5
  },
  audio: {
    sampleRate: 24000,
    channels: 1,
    echoCancellation: true,
    noiseSuppression: true,
    autoGainControl: true
  },
  session: {
    enableTurnDetection: true,
    turnDetectionThreshold: 0.5,
    silenceTimeout: 1000
  }
};

/**
 * Create a mock RealtimeClient for testing
 */
export function createMockRealtimeClient(
  config: Partial<RealtimeClientConfig> = {}
): {
  client: RealtimeClient;
  mockWebSocket: WebSocketMock;
  mockAudioContext: MockAudioContext;
  eventEmitter: EventEmitter<EventTypes>;
} {
  // Create mocks
  const mockWebSocket = new WebSocketMock('ws://localhost:8080/test');
  const mockAudioContext = new MockAudioContext();
  const eventEmitter = new EventEmitter<EventTypes>();

  // Create mock client
  const client = {
    config: { ...defaultTestConfig, ...config },
    isConnected: vi.fn(() => false),
    connect: vi.fn().mockResolvedValue(undefined),
    disconnect: vi.fn(),
    destroy: vi.fn(),
    sendText: vi.fn(),
    sendBinaryFrame: vi.fn(),
    sendEvent: vi.fn(),
    updateSession: vi.fn(),
    setAudioEnabled: vi.fn(),
    getStatistics: vi.fn(() => ({
      connection: {
        state: 'disconnected',
        latency: 0,
        packetsLost: 0,
        reconnectAttempts: 0
      },
      audio: {
        inputLevel: 0,
        outputLevel: 0,
        isRecording: false,
        isMuted: false
      },
      session: {
        duration: 0,
        messageCount: 0,
        turnCount: 0
      }
    })),
    on: eventEmitter.on.bind(eventEmitter),
    off: eventEmitter.off.bind(eventEmitter),
    once: eventEmitter.once.bind(eventEmitter),
    emit: eventEmitter.emit.bind(eventEmitter),
    removeAllListeners: eventEmitter.removeAllListeners.bind(eventEmitter),
    
    // Internal mocks for testing
    _mockWebSocket: mockWebSocket,
    _mockAudioContext: mockAudioContext,
    _eventEmitter: eventEmitter
  } as any;

  return {
    client,
    mockWebSocket,
    mockAudioContext,
    eventEmitter
  };
}

/**
 * Wait for a WebSocket connection to be established
 */
export async function waitForSocketConnection(
  ws: WebSocket | MockWebSocket,
  timeout: number = 5000
): Promise<void> {
  const startTime = Date.now();
  
  while (Date.now() - startTime < timeout) {
    if (ws.readyState === WebSocket.OPEN || ws.readyState === MockWebSocket.OPEN) {
      return;
    }
    await new Promise(resolve => setTimeout(resolve, 50));
  }
  
  throw new Error('WebSocket connection timeout');
}

/**
 * Simulate a complete turn sequence
 */
export async function simulateTurnSequence(
  client: any,
  options: {
    userMessage?: string;
    assistantResponse?: string;
    includeAudio?: boolean;
    turnDelay?: number;
  } = {}
): Promise<void> {
  const {
    userMessage = 'Hello',
    assistantResponse = 'Hi there!',
    includeAudio = false,
    turnDelay = 100
  } = options;

  const mockWs = client._mockWebSocket;
  const eventEmitter = client._eventEmitter;

  // User turn start
  eventEmitter.emit('turn_start', {
    type: 'turn_start',
    turn_id: 'turn_user_1',
    turn_type: 'user',
    timestamp: new Date().toISOString()
  });

  // User message
  if (userMessage) {
    eventEmitter.emit('text_delta', {
      type: 'text_delta',
      content: userMessage,
      role: 'user',
      timestamp: new Date().toISOString()
    });
  }

  // User audio if included
  if (includeAudio) {
    const audioData = new ArrayBuffer(1024);
    eventEmitter.emit('audio_delta', {
      type: 'audio_delta',
      delta: audioData,
      timestamp: new Date().toISOString()
    });
  }

  // User turn end
  await new Promise(resolve => setTimeout(resolve, turnDelay));
  eventEmitter.emit('turn_end', {
    type: 'turn_end',
    turn_id: 'turn_user_1',
    timestamp: new Date().toISOString()
  });

  // Assistant turn start
  await new Promise(resolve => setTimeout(resolve, turnDelay));
  eventEmitter.emit('turn_start', {
    type: 'turn_start',
    turn_id: 'turn_assistant_1',
    turn_type: 'assistant',
    timestamp: new Date().toISOString()
  });

  // Assistant response
  if (assistantResponse) {
    eventEmitter.emit('text_delta', {
      type: 'text_delta',
      content: assistantResponse,
      role: 'assistant',
      timestamp: new Date().toISOString()
    });
  }

  // Assistant audio if included
  if (includeAudio) {
    const audioData = new ArrayBuffer(2048);
    eventEmitter.emit('audio_delta', {
      type: 'audio_delta',
      delta: audioData,
      timestamp: new Date().toISOString()
    });
  }

  // Assistant turn end
  await new Promise(resolve => setTimeout(resolve, turnDelay));
  eventEmitter.emit('turn_end', {
    type: 'turn_end',
    turn_id: 'turn_assistant_1',
    timestamp: new Date().toISOString()
  });
}

/**
 * Response builder for common test scenarios
 */
export class TestResponseBuilder {
  private responses: any[] = [];

  static create(): TestResponseBuilder {
    return new TestResponseBuilder();
  }

  addTextResponse(content: string, role: 'user' | 'assistant' = 'assistant'): this {
    this.responses.push({
      type: 'text_delta',
      content,
      role,
      timestamp: new Date().toISOString()
    });
    return this;
  }

  addAudioResponse(sizeInBytes: number = 1024): this {
    this.responses.push({
      type: 'audio_delta',
      delta: new ArrayBuffer(sizeInBytes),
      timestamp: new Date().toISOString()
    });
    return this;
  }

  addToolCall(name: string, args: any = {}): this {
    this.responses.push({
      type: 'tool_call',
      tool_call_id: `call_${Math.random().toString(36).substr(2, 9)}`,
      name,
      arguments: args,
      timestamp: new Date().toISOString()
    });
    return this;
  }

  addToolResponse(toolCallId: string, result: any): this {
    this.responses.push({
      type: 'tool_response',
      tool_call_id: toolCallId,
      result,
      timestamp: new Date().toISOString()
    });
    return this;
  }

  addError(code: string, message: string): this {
    this.responses.push({
      type: 'error',
      error: {
        code,
        message,
        details: {}
      },
      timestamp: new Date().toISOString()
    });
    return this;
  }

  addTurnStart(turnType: 'user' | 'assistant' = 'assistant'): this {
    this.responses.push({
      type: 'turn_start',
      turn_id: `turn_${Math.random().toString(36).substr(2, 9)}`,
      turn_type: turnType,
      timestamp: new Date().toISOString()
    });
    return this;
  }

  addTurnEnd(turnId?: string): this {
    this.responses.push({
      type: 'turn_end',
      turn_id: turnId || `turn_${Math.random().toString(36).substr(2, 9)}`,
      timestamp: new Date().toISOString()
    });
    return this;
  }

  build(): any[] {
    return this.responses;
  }

  async simulate(mockWebSocket: MockWebSocket, delay: number = 10): Promise<void> {
    for (const response of this.responses) {
      await new Promise(resolve => setTimeout(resolve, delay));
      
      if (response.type === 'audio_delta') {
        mockWebSocket.simulateBinaryMessage(response.delta);
      } else {
        mockWebSocket.simulateTextMessage(response);
      }
    }
  }
}

/**
 * Mock chat session manager for testing
 */
export function createMockChatSessionManager() {
  return {
    sessionId: 'test-session-id',
    isActive: vi.fn(() => true),
    startSession: vi.fn().mockResolvedValue('test-session-id'),
    endSession: vi.fn().mockResolvedValue(undefined),
    updateSession: vi.fn(),
    getSessionData: vi.fn(() => ({
      id: 'test-session-id',
      startTime: new Date().toISOString(),
      messageCount: 0,
      turnCount: 0
    })),
    on: vi.fn(),
    off: vi.fn(),
    emit: vi.fn()
  };
}

/**
 * Mock turn manager for testing
 */
export function createMockTurnManager() {
  return {
    currentTurn: null,
    isUserTurn: vi.fn(() => false),
    isAssistantTurn: vi.fn(() => false),
    startTurn: vi.fn(),
    endTurn: vi.fn(),
    interruptTurn: vi.fn(),
    getTurnHistory: vi.fn(() => []),
    on: vi.fn(),
    off: vi.fn(),
    emit: vi.fn()
  };
}

/**
 * Mock authentication manager for testing
 */
export function createMockAuthManager() {
  return {
    token: 'test-token',
    isAuthenticated: vi.fn(() => true),
    authenticate: vi.fn().mockResolvedValue('test-token'),
    refreshToken: vi.fn().mockResolvedValue('refreshed-token'),
    logout: vi.fn(),
    on: vi.fn(),
    off: vi.fn(),
    emit: vi.fn()
  };
}

/**
 * Create a test harness for integration testing
 */
export class RealtimeTestHarness {
  client: any;
  mockWebSocket: WebSocketMock;
  mockAudioContext: MockAudioContext;
  eventEmitter: EventEmitter<EventTypes>;
  responseBuilder: TestResponseBuilder;

  constructor(config: Partial<RealtimeClientConfig> = {}) {
    const mocks = createMockRealtimeClient(config);
    this.client = mocks.client;
    this.mockWebSocket = mocks.mockWebSocket;
    this.mockAudioContext = mocks.mockAudioContext;
    this.eventEmitter = mocks.eventEmitter;
    this.responseBuilder = TestResponseBuilder.create();
  }

  async connect(): Promise<void> {
    this.mockWebSocket.simulateOpen();
    this.client.isConnected.mockReturnValue(true);
    this.eventEmitter.emit('connected', {
      type: 'connected',
      session_id: 'test-session',
      timestamp: new Date().toISOString()
    });
  }

  async disconnect(): Promise<void> {
    this.mockWebSocket.simulateClose();
    this.client.isConnected.mockReturnValue(false);
    this.eventEmitter.emit('disconnected', {
      type: 'disconnected',
      reason: 'Test disconnection',
      code: 1000,
      timestamp: new Date().toISOString()
    });
  }

  async simulateMessage(message: string): Promise<void> {
    this.mockWebSocket.simulateTextMessage({
      type: 'text_delta',
      content: message,
      role: 'assistant',
      timestamp: new Date().toISOString()
    });
  }

  async simulateError(code: string, message: string): Promise<void> {
    this.mockWebSocket.simulateTextMessage({
      type: 'error',
      error: {
        code,
        message,
        details: {}
      },
      timestamp: new Date().toISOString()
    });
  }

  async simulateTurn(options: Parameters<typeof simulateTurnSequence>[1]): Promise<void> {
    return simulateTurnSequence(this.client, options);
  }

  cleanup(): void {
    this.client.destroy();
    this.eventEmitter.removeAllListeners();
  }
}
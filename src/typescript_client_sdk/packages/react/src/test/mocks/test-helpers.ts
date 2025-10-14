/**
 * MSW Test Helpers for React Package
 * Utility functions to work with MSW in tests
 */
import { http, HttpResponse } from 'msw';
import { server, addTestHandler } from './server';
import { sessionStore, createMockSession } from './handlers/session-handlers';
import { createMockMessage } from './handlers/message-handlers';
import { audioState, turnState } from './handlers/audio-handlers';
import { avatarState } from './handlers/avatar-handlers';
import { connectionState } from './handlers/connection-handlers';

/**
 * Reset all mock stores to initial state
 */
export function resetMockStores() {
  // Clear session store
  sessionStore.clear();
  
  // Reset audio state
  Object.assign(audioState, {
    isRecording: false,
    isStreaming: false,
    isMuted: false,
    volume: 1.0,
    inputDevice: 'default',
    outputDevice: 'default',
    voiceModel: 'claude-3-sonnet',
    sampleRate: 24000,
    vadEnabled: true,
    vadThreshold: 0.5
  });
  
  // Reset turn state
  Object.assign(turnState, {
    currentTurn: null,
    turnId: null,
    turnStartTime: null,
    isInterrupted: false
  });
  
  // Reset avatar state
  Object.assign(avatarState, {
    isVisible: true,
    currentAnimation: 'idle',
    animationSpeed: 1.0,
    expressionIntensity: 1.0,
    lipSyncEnabled: true,
    headTrackingEnabled: false,
    currentExpression: 'neutral',
    position: { x: 0, y: 0, z: 0 },
    rotation: { x: 0, y: 0, z: 0 }
  });
  
  // Reset connection state
  Object.assign(connectionState, {
    status: 'disconnected',
    sessionId: null,
    lastPing: null,
    reconnectAttempts: 0,
    error: null
  });
}

/**
 * Helper to simulate a connected state
 */
export function mockConnectedState(sessionId?: string) {
  connectionState.status = 'connected';
  connectionState.sessionId = sessionId || `ws_test_${Date.now()}`;
  connectionState.lastPing = new Date().toISOString();
  connectionState.error = null;
  return connectionState.sessionId;
}

/**
 * Helper to create a test session with messages
 */
export function createTestSession(messageCount: number = 5) {
  const session = createMockSession();
  const messages = [];
  
  for (let i = 0; i < messageCount; i++) {
    messages.push(createMockMessage({
      id: `msg_${i}`,
      role: i % 2 === 0 ? 'user' : 'assistant',
      content: `Test message ${i}`,
      timestamp: new Date(Date.now() - (messageCount - i) * 60000).toISOString()
    }));
  }
  
  session.messages = messages;
  sessionStore.set(session.id, session);
  
  return session;
}

/**
 * Helper to create multiple test sessions
 */
export function createTestSessions(count: number) {
  const sessions = [];
  
  for (let i = 0; i < count; i++) {
    const session = createMockSession(`session_${i}`, {
      name: `Session ${i}`,
      createdAt: new Date(Date.now() - i * 3600000).toISOString()
    });
    sessionStore.set(session.id, session);
    sessions.push(session);
  }
  
  return sessions;
}

/**
 * Helper to simulate audio recording
 */
export function mockAudioRecording() {
  audioState.isRecording = true;
  turnState.currentTurn = 'user';
  turnState.turnId = `turn_recording_${Date.now()}`;
  turnState.turnStartTime = new Date().toISOString();
}

/**
 * Helper to simulate audio streaming
 */
export function mockAudioStreaming() {
  audioState.isStreaming = true;
  turnState.currentTurn = 'assistant';
  turnState.turnId = `turn_streaming_${Date.now()}`;
  turnState.turnStartTime = new Date().toISOString();
}

/**
 * Mock a failed API request
 */
export function mockApiError(endpoint: string, error: { message: string; code?: string; status?: number }) {
  addTestHandler(
    http.all(`*${endpoint}`, () => {
      return HttpResponse.json(
        { 
          error: error.message,
          code: error.code || 'ERROR'
        },
        { status: error.status || 500 }
      );
    })
  );
}

/**
 * Mock a delayed API response
 */
export function mockApiDelay(endpoint: string, delay: number) {
  addTestHandler(
    http.all(`*${endpoint}`, async () => {
      await new Promise(resolve => setTimeout(resolve, delay));
      return HttpResponse.json({ success: true });
    })
  );
}

/**
 * Mock a streaming response
 */
export function mockStreamingResponse(endpoint: string, chunks: any[], delayBetweenChunks: number = 50) {
  addTestHandler(
    http.get(`*${endpoint}`, async () => {
      const encoder = new TextEncoder();
      const stream = new ReadableStream({
        async start(controller) {
          for (const chunk of chunks) {
            controller.enqueue(
              encoder.encode(`data: ${JSON.stringify(chunk)}\n\n`)
            );
            await new Promise(resolve => setTimeout(resolve, delayBetweenChunks));
          }
          controller.close();
        }
      });
      
      return new HttpResponse(stream, {
        headers: {
          'Content-Type': 'text/event-stream',
          'Cache-Control': 'no-cache'
        }
      });
    })
  );
}

/**
 * Wait for a condition to be met
 */
export async function waitForCondition(
  condition: () => boolean,
  timeout: number = 5000,
  interval: number = 100
): Promise<void> {
  const startTime = Date.now();
  
  while (!condition()) {
    if (Date.now() - startTime > timeout) {
      throw new Error('Timeout waiting for condition');
    }
    await new Promise(resolve => setTimeout(resolve, interval));
  }
}

/**
 * Create a mock WebSocket event
 */
export function createWebSocketEvent(type: string, data: any) {
  return {
    type,
    data,
    timestamp: new Date().toISOString(),
    sessionId: connectionState.sessionId
  };
}

/**
 * Simulate a series of WebSocket events
 */
export async function simulateEventSequence(events: Array<{ type: string; data: any; delay?: number }>) {
  for (const event of events) {
    if (event.delay) {
      await new Promise(resolve => setTimeout(resolve, event.delay));
    }
    
    // This would be handled by your WebSocket mock in actual tests
    // For now, we just return the formatted event
    const wsEvent = createWebSocketEvent(event.type, event.data);
    
    // You could emit this to a test event emitter if needed
    // eventEmitter.emit(event.type, wsEvent);
  }
}

/**
 * Helper to verify MSW is handling requests
 */
export function getMockRequestHistory() {
  // This is a placeholder - in a real implementation,
  // you might track requests using MSW's life-cycle events
  return {
    requests: [],
    clear: () => {}
  };
}

/**
 * Export commonly used test data
 */
export const testData = {
  validSessionId: 'session_test_123',
  validMessageId: 'msg_test_123',
  validTurnId: 'turn_test_123',
  validUserId: 'user_test_123',
  testApiKey: 'test_api_key_123',
  testWebSocketUrl: 'ws://localhost:3000/ws',
  testApiUrl: 'http://localhost:3000/api'
};

/**
 * Export mock response builders
 */
export const mockResponses = {
  success: (data: any) => HttpResponse.json({ success: true, data }),
  error: (message: string, status: number = 400) => 
    HttpResponse.json({ success: false, error: message }, { status }),
  stream: (chunks: any[]) => {
    const encoder = new TextEncoder();
    const stream = new ReadableStream({
      async start(controller) {
        for (const chunk of chunks) {
          controller.enqueue(encoder.encode(`data: ${JSON.stringify(chunk)}\n\n`));
          await new Promise(r => setTimeout(r, 50));
        }
        controller.close();
      }
    });
    return new HttpResponse(stream, {
      headers: { 'Content-Type': 'text/event-stream' }
    });
  }
};
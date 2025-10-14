/**
 * MSW HTTP Handlers for React Package Tests
 * Mock implementations for API endpoints and WebSocket-like events used by React hooks
 */
import { http, HttpResponse, delay } from 'msw';
import { sessionHandlers } from './session-handlers';
import { messageHandlers } from './message-handlers';
import { audioHandlers } from './audio-handlers';
import { avatarHandlers } from './avatar-handlers';
import { connectionHandlers } from './connection-handlers';

export const handlers = [
  ...sessionHandlers,
  ...messageHandlers,
  ...audioHandlers,
  ...avatarHandlers,
  ...connectionHandlers,
  
  // Health check endpoint
  http.get('*/api/health', () => {
    return HttpResponse.json({
      status: 'healthy',
      version: '1.0.0',
      timestamp: new Date().toISOString()
    });
  }),

  // WebSocket connection endpoint mock
  http.get('*/ws/connect', () => {
    return HttpResponse.json({
      status: 'connected',
      sessionId: `ws_session_${Date.now()}`,
      timestamp: new Date().toISOString()
    });
  }),

  // Default 404 handler for undefined routes
  http.all('*/api/*', () => {
    return HttpResponse.json(
      { error: 'Not found' },
      { status: 404 }
    );
  })
];
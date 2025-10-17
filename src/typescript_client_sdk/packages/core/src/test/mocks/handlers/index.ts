/**
 * MSW HTTP Handlers for Agent C API
 * Mock implementations of all API endpoints used by core package
 */
import { http, HttpResponse, delay } from 'msw';
import { authHandlers } from './auth-handlers';
import { sessionHandlers } from './session-handlers';
import { configHandlers } from './config-handlers';
import { audioHandlers } from './audio-handlers';

export const handlers = [
  ...authHandlers,
  ...sessionHandlers,
  ...configHandlers,
  ...audioHandlers,
  
  // Health check endpoint
  http.get('*/api/health', () => {
    return HttpResponse.json({
      status: 'healthy',
      version: '1.0.0',
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
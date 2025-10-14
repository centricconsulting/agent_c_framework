/**
 * MSW Server Setup for Core Package Tests
 * Provides mock HTTP endpoints for Agent C API
 */
import { setupServer } from 'msw/node';
import { http, HttpResponse, delay } from 'msw';
import { handlers } from './handlers';

// Create and configure the MSW server
export const server = setupServer(...handlers);

// Server lifecycle hooks for tests
export function startMockServer() {
  server.listen({
    onUnhandledRequest: 'warn' // Warn on unhandled requests during tests
  });
}

export function resetMockServer() {
  server.resetHandlers();
}

export function stopMockServer() {
  server.close();
}

// Helper to add runtime handlers for specific tests
export function addTestHandler(handler: any) {
  server.use(handler);
}
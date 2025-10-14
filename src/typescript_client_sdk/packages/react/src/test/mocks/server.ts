/**
 * MSW Server Setup for React Package Tests
 * Provides mock HTTP and WebSocket-like event handlers for React hooks testing
 */
import { setupServer } from 'msw/node';
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

// Helper for simulating WebSocket-like events
export function simulateWebSocketEvent(eventType: string, data: any) {
  // This is a placeholder for event simulation
  // In actual tests, this would be handled by mocked client methods
  return {
    type: eventType,
    data,
    timestamp: new Date().toISOString()
  };
}
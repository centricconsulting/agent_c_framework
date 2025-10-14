/**
 * MSW server setup for tests
 */

import { setupServer } from 'msw/node';
import { handlers } from './msw-handlers';

/**
 * Create MSW server instance for tests
 */
export const server = setupServer(...handlers);

/**
 * Start server before all tests
 */
export function startMockServer() {
  server.listen({
    onUnhandledRequest: 'warn',
  });
}

/**
 * Reset handlers after each test
 */
export function resetMockServer() {
  server.resetHandlers();
}

/**
 * Stop server after all tests
 */
export function stopMockServer() {
  server.close();
}

/**
 * Add custom handlers for specific tests
 */
export function useMockHandler(...customHandlers: any[]) {
  server.use(...customHandlers);
}
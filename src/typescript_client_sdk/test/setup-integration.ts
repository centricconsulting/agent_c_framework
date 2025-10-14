/**
 * Integration test setup with MSW
 */

import { beforeAll, afterEach, afterAll } from 'vitest';
import { startMockServer, resetMockServer, stopMockServer } from './mocks/server';

// Start MSW server before all tests
beforeAll(() => {
  startMockServer();
});

// Reset handlers after each test
afterEach(() => {
  resetMockServer();
});

// Clean up after all tests
afterAll(() => {
  stopMockServer();
});
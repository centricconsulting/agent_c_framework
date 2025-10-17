/**
 * Global test setup for the monorepo
 */

import { beforeAll, afterEach, afterAll, vi } from 'vitest';

// Mock console methods to reduce noise in tests
// Commented out for now as it may cause issues
// beforeAll(() => {
//   // Only mock in test environment
//   if (process.env.NODE_ENV === 'test') {
//     global.console = {
//       ...console,
//       error: vi.fn(),
//       warn: vi.fn(),
//       log: vi.fn(),
//       debug: vi.fn(),
//     };
//   }
// });

// Clean up after each test
afterEach(() => {
  vi.clearAllMocks();
  vi.restoreAllMocks();
});

// Restore console after all tests
afterAll(() => {
  vi.restoreAllMocks();
});

// Add custom matchers if needed
expect.extend({
  toBeWithinRange(received: number, floor: number, ceiling: number) {
    const pass = received >= floor && received <= ceiling;
    if (pass) {
      return {
        message: () => `expected ${received} not to be within range ${floor} - ${ceiling}`,
        pass: true,
      };
    } else {
      return {
        message: () => `expected ${received} to be within range ${floor} - ${ceiling}`,
        pass: false,
      };
    }
  },
});
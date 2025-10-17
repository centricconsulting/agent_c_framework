/**
 * Test Setup for React Package
 * Configure testing environment and global mocks
 */

import '@testing-library/jest-dom';
import { vi } from 'vitest';
import { cleanup } from '@testing-library/react';
import { afterEach, beforeAll, afterAll } from 'vitest';
import { startMockServer, stopMockServer, resetMockServer } from './mocks/server';

// Start MSW server before all tests
beforeAll(() => {
  startMockServer();
});

// Reset handlers and cleanup after each test
afterEach(() => {
  resetMockServer();
  cleanup();
  vi.clearAllMocks();
});

// Stop MSW server after all tests
afterAll(() => {
  stopMockServer();
});

// Mock console methods to reduce test output noise
global.console = {
  ...console,
  error: vi.fn(),
  warn: vi.fn(),
  log: vi.fn(),
  debug: vi.fn(),
};

// Restore console for debugging when needed
(global as any).restoreConsole = () => {
  global.console.error = console.error;
  global.console.warn = console.warn;
  global.console.log = console.log;
  global.console.debug = console.debug;
};

// Mock window.matchMedia
Object.defineProperty(window, 'matchMedia', {
  writable: true,
  value: vi.fn().mockImplementation(query => ({
    matches: false,
    media: query,
    onchange: null,
    addListener: vi.fn(),
    removeListener: vi.fn(),
    addEventListener: vi.fn(),
    removeEventListener: vi.fn(),
    dispatchEvent: vi.fn(),
  })),
});

// Mock IntersectionObserver
global.IntersectionObserver = vi.fn().mockImplementation(() => ({
  observe: vi.fn(),
  unobserve: vi.fn(),
  disconnect: vi.fn(),
}));

// Mock ResizeObserver
global.ResizeObserver = vi.fn().mockImplementation(() => ({
  observe: vi.fn(),
  unobserve: vi.fn(),
  disconnect: vi.fn(),
}));

// Mock requestAnimationFrame for testing animations
global.requestAnimationFrame = vi.fn().mockImplementation((cb) => {
  setTimeout(cb, 0);
  return Math.random();
});

global.cancelAnimationFrame = vi.fn();

// Mock performance.now for performance testing
if (!global.performance) {
  global.performance = {} as any;
}

global.performance.now = vi.fn(() => Date.now());

// Export test utilities
export { vi };


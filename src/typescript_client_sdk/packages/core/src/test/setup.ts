/**
 * Test setup file for Core package
 * Configures the test environment with necessary polyfills and mocks
 */

import { vi } from 'vitest';
import { startMockServer, stopMockServer, resetMockServer } from './mocks/server';

// Start MSW server before all tests
beforeAll(() => {
  startMockServer();
});

// Reset handlers after each test
afterEach(() => {
  resetMockServer();
  vi.clearAllMocks();
});

// Stop server after all tests
afterAll(() => {
  stopMockServer();
});

// Polyfill for CloseEvent (not available in Node)
if (typeof CloseEvent === 'undefined') {
  (global as any).CloseEvent = class CloseEvent extends Event {
    code: number;
    reason: string;
    wasClean: boolean;

    constructor(type: string, init?: { code?: number; reason?: string; wasClean?: boolean }) {
      super(type);
      this.code = init?.code || 1000;
      this.reason = init?.reason || '';
      this.wasClean = init?.wasClean || true;
    }
  };
}

// Polyfill for BlobEvent (not available in Node)
if (typeof BlobEvent === 'undefined') {
  (global as any).BlobEvent = class BlobEvent extends Event {
    data: Blob;
    
    constructor(type: string, init: { data: Blob }) {
      super(type);
      this.data = init.data;
    }
  };
}

// Mock performance.now if not available
if (typeof performance === 'undefined') {
  (global as any).performance = {
    now: () => Date.now()
  };
}

// Setup default test timeout
vi.setConfig({ testTimeout: 10000 });

// Suppress console errors in tests unless debugging
if (!process.env.DEBUG_TESTS) {
  const originalError = console.error;
  console.error = (...args: any[]) => {
    // Filter out expected errors
    const message = args[0]?.toString() || '';
    if (
      message.includes('Failed to parse event') || // Expected in EventRegistry tests
      message.includes('[ERROR]') || // Logger messages
      message.includes('Invalid message') || // Expected validation errors
      message.includes('Invalid session') // Expected validation errors
    ) {
      return;
    }
    originalError.apply(console, args);
  };
}
/**
 * Test Setup for UI Package
 * Configure testing environment for UI components with jsdom
 */

import '@testing-library/jest-dom';
import { vi } from 'vitest';
import { cleanup } from '@testing-library/react';
import { afterEach, beforeAll, afterAll, beforeEach } from 'vitest';
import { server } from './mocks/server';
import * as realtimeReactMocks from './mocks/realtime-react';
import * as reactVirtualMocks from './mocks/react-virtual';

// Mock @agentc/realtime-react globally
vi.mock('@agentc/realtime-react', () => realtimeReactMocks);

// Mock @tanstack/react-virtual globally
vi.mock('@tanstack/react-virtual', () => reactVirtualMocks);

// Start MSW server before all tests
beforeAll(() => {
  server.listen({ onUnhandledRequest: 'warn' });
});

// Setup before each test
beforeEach(() => {
  // Reset all react hooks mocks to default state
  realtimeReactMocks.resetAllMocks();
});

// Cleanup after each test
afterEach(() => {
  cleanup();
  vi.clearAllMocks();
  server.resetHandlers();
});

// Close MSW server after all tests
afterAll(() => {
  server.close();
});

// Standard browser API mocks (still needed with jsdom)

// Mock IntersectionObserver
global.IntersectionObserver = vi.fn().mockImplementation(() => ({
  observe: vi.fn(),
  unobserve: vi.fn(),
  disconnect: vi.fn(),
  takeRecords: vi.fn(() => []),
  root: null,
  rootMargin: '',
  thresholds: []
}));

// Mock ResizeObserver - Required for Radix UI components
class ResizeObserverMock {
  observe = vi.fn();
  unobserve = vi.fn();
  disconnect = vi.fn();
}

global.ResizeObserver = ResizeObserverMock as any;

// Mock matchMedia for responsive design testing
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

// Mock scrollTo for scroll behavior testing
Object.defineProperty(window, 'scrollTo', {
  writable: true,
  value: vi.fn()
});

// Mock requestAnimationFrame for animation testing
global.requestAnimationFrame = vi.fn((cb) => setTimeout(cb, 0));
global.cancelAnimationFrame = vi.fn((id) => clearTimeout(id));

// Mock CSS.supports for feature detection
if (!global.CSS) {
  global.CSS = {} as any;
}
global.CSS.supports = vi.fn().mockReturnValue(true);

// Mock navigator.mediaDevices for audio recording tests
if (!navigator.mediaDevices) {
  Object.defineProperty(navigator, 'mediaDevices', {
    writable: true,
    value: {
      getUserMedia: vi.fn().mockResolvedValue({
        getTracks: () => [],
        getAudioTracks: () => [],
        getVideoTracks: () => [],
        stop: vi.fn(),
      }),
      enumerateDevices: vi.fn().mockResolvedValue([]),
      getSupportedConstraints: vi.fn().mockReturnValue({}),
    },
  });
}

// Mock ClipboardEvent for paste testing
if (typeof ClipboardEvent === 'undefined') {
  class ClipboardEventPolyfill extends Event {
    clipboardData: any;
    
    constructor(type: string, eventInitDict?: any) {
      super(type, eventInitDict);
      this.clipboardData = eventInitDict?.clipboardData || null;
    }
  }
  
  (global as any).ClipboardEvent = ClipboardEventPolyfill;
}

// Mock DataTransfer for clipboard and drag-drop testing
if (typeof DataTransfer === 'undefined') {
  class DataTransferPolyfill {
    items: any[] = [];
    files: File[] = [];
    types: string[] = [];
    
    getData(format: string) {
      return '';
    }
    
    setData(format: string, data: string) {
      // no-op
    }
    
    clearData(format?: string) {
      // no-op
    }
  }
  
  (global as any).DataTransfer = DataTransferPolyfill;
}
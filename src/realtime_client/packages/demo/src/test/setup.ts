/**
 * Test Setup for Demo Package
 * Configure testing environment for the demo application
 */

import '@testing-library/jest-dom';
import { vi } from 'vitest';
import { cleanup } from '@testing-library/react';
import { afterEach, beforeAll, afterAll } from 'vitest';
import { server, serverLifecycle } from './mocks/server';

// Cleanup after each test
afterEach(() => {
  cleanup();
  vi.clearAllMocks();
  localStorage.clear();
  sessionStorage.clear();
  
  // Reset MSW handlers and demo data
  serverLifecycle.reset();
});

// Cleanup after all tests
afterAll(() => {
  // Close MSW server
  serverLifecycle.close();
});

// Setup before all tests
beforeAll(() => {
  // Mock environment variables
  process.env.VITE_API_URL = 'ws://localhost:8080/test';
  process.env.VITE_AUTH_TOKEN = 'test-token';
  
  // Start MSW server for API mocking
  serverLifecycle.start();
});

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

// Mock ResizeObserver
global.ResizeObserver = vi.fn().mockImplementation(() => ({
  observe: vi.fn(),
  unobserve: vi.fn(),
  disconnect: vi.fn()
}));

// Mock matchMedia
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

// Mock scrollTo
Object.defineProperty(window, 'scrollTo', {
  writable: true,
  value: vi.fn()
});

// Mock requestAnimationFrame
global.requestAnimationFrame = vi.fn((cb) => setTimeout(cb, 0));
global.cancelAnimationFrame = vi.fn((id) => clearTimeout(id));

// Mock Web Audio API
class MockAudioContext {
  createMediaStreamSource = vi.fn();
  createScriptProcessor = vi.fn();
  createAnalyser = vi.fn();
  createGain = vi.fn();
  destination = {};
  sampleRate = 44100;
  close = vi.fn();
  resume = vi.fn();
  suspend = vi.fn();
  state = 'running';
}

global.AudioContext = MockAudioContext as any;
global.webkitAudioContext = MockAudioContext as any;

// Mock MediaRecorder
class MockMediaRecorder {
  start = vi.fn();
  stop = vi.fn();
  pause = vi.fn();
  resume = vi.fn();
  requestData = vi.fn();
  ondataavailable = null;
  onerror = null;
  onpause = null;
  onresume = null;
  onstart = null;
  onstop = null;
  state = 'inactive';
  
  static isTypeSupported = vi.fn(() => true);
}

global.MediaRecorder = MockMediaRecorder as any;

// Mock getUserMedia
if (!navigator.mediaDevices) {
  Object.defineProperty(navigator, 'mediaDevices', {
    value: {},
    writable: true
  });
}

navigator.mediaDevices.getUserMedia = vi.fn().mockResolvedValue({
  getTracks: () => [],
  getAudioTracks: () => [],
  getVideoTracks: () => [],
  addTrack: vi.fn(),
  removeTrack: vi.fn(),
  clone: vi.fn(),
  active: true,
  id: 'mock-stream-id'
});

// Mock WebSocket
class MockWebSocket {
  static CONNECTING = 0;
  static OPEN = 1;
  static CLOSING = 2;
  static CLOSED = 3;
  
  readyState = MockWebSocket.CONNECTING;
  url = '';
  onopen = null;
  onclose = null;
  onerror = null;
  onmessage = null;
  
  constructor(url: string) {
    this.url = url;
  }
  
  send = vi.fn();
  close = vi.fn();
  addEventListener = vi.fn();
  removeEventListener = vi.fn();
  dispatchEvent = vi.fn();
}

global.WebSocket = MockWebSocket as any;

// Note: fetch is handled by MSW (Mock Service Worker)
// Do NOT mock global.fetch as it will override MSW handlers

// Mock console methods to reduce test output noise
const originalConsole = {
  log: console.log,
  error: console.error,
  warn: console.warn,
  info: console.info,
  debug: console.debug
};

console.log = vi.fn();
console.error = vi.fn();
console.warn = vi.fn();
console.info = vi.fn();
console.debug = vi.fn();

// Restore console for debugging when needed
(global as any).restoreConsole = () => {
  console.log = originalConsole.log;
  console.error = originalConsole.error;
  console.warn = originalConsole.warn;
  console.info = originalConsole.info;
  console.debug = originalConsole.debug;
};
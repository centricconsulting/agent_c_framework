import { vi } from 'vitest';

/**
 * Creates a simple mock WebSocket
 */
export const createMockWebSocket = () => ({
  send: vi.fn(),
  close: vi.fn(),
  addEventListener: vi.fn(),
  removeEventListener: vi.fn(),
  readyState: WebSocket.OPEN
});

/**
 * Creates a simple mock fetch response
 */
export const createMockFetchResponse = (data: any = {}, ok = true) => ({
  ok,
  json: vi.fn().mockResolvedValue(data),
  text: vi.fn().mockResolvedValue(JSON.stringify(data)),
  status: ok ? 200 : 400
});

/**
 * Creates a simple mock AudioContext
 */
export const createMockAudioContext = () => ({
  createMediaStreamSource: vi.fn(() => ({ connect: vi.fn(), disconnect: vi.fn() })),
  createScriptProcessor: vi.fn(() => ({ connect: vi.fn(), disconnect: vi.fn() })),
  createAnalyser: vi.fn(() => ({ connect: vi.fn(), disconnect: vi.fn() })),
  destination: {},
  close: vi.fn(),
  sampleRate: 48000
});

/**
 * Creates a simple mock MediaStream
 */
export const createMockMediaStream = () => ({
  getTracks: vi.fn(() => []),
  getAudioTracks: vi.fn(() => [{ stop: vi.fn() }]),
  getVideoTracks: vi.fn(() => []),
  addTrack: vi.fn(),
  removeTrack: vi.fn()
});
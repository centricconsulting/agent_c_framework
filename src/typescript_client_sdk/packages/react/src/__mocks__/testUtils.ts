/**
 * Base Mock Utilities for React Package Testing
 * Following MOCKING_GUIDE.md principles:
 * - Keep it simple (Level 1 mocking - simple stubs)
 * - Helpers under 10 lines each
 * - Make it obvious, not clever
 * - All packages use happy-dom environment
 */

import { vi } from 'vitest';
import { renderHook as rtlRenderHook, RenderHookOptions } from '@testing-library/react';
import React from 'react';
import type { RealtimeClient, ConnectionState } from '@agentc/realtime-core';
import type { AgentCContextValue } from '../providers/AgentCContext';

/**
 * Create a mock RealtimeClient instance
 * Simple stub - just returns what we need for testing
 */
export const mockClient = (): Partial<RealtimeClient> => ({
  connect: vi.fn().mockResolvedValue(undefined),
  disconnect: vi.fn(),
  destroy: vi.fn(),
  isConnected: vi.fn(() => false),
  getConnectionState: vi.fn(() => 'disconnected' as ConnectionState),
  sendText: vi.fn(),
  sendBinaryFrame: vi.fn(),
  sendEvent: vi.fn(),
  on: vi.fn(),
  off: vi.fn(),
  emit: vi.fn()
});

/**
 * Create a mock context value for AgentCProvider
 * Simple values for testing React components
 */
export const mockContext = (): AgentCContextValue => ({
  client: mockClient() as RealtimeClient,
  isInitializing: false,
  error: null,
  initialization: {
    isInitialized: true,
    receivedEvents: new Set(),
    user: null,
    agents: [],
    avatars: [],
    voices: [],
    tools: [],
    currentSession: null
  }
});

/**
 * Wrapper for testing hooks with AgentCProvider context
 * Keeps test setup obvious and simple
 */
export const renderHook = <TProps, TResult>(
  hook: (props: TProps) => TResult,
  options?: RenderHookOptions<TProps>
) => {
  const Wrapper = ({ children }: { children: React.ReactNode }) => 
    React.createElement('div', null, children); // Simple wrapper, add provider when needed
  
  return rtlRenderHook(hook, {
    wrapper: Wrapper,
    ...options
  });
};

/**
 * Mock WebSocket for happy-dom environment
 * Standard mock for all React package tests
 */
export const setupWebSocketMock = () => {
  const mockWs = {
    send: vi.fn(),
    close: vi.fn(),
    addEventListener: vi.fn(),
    removeEventListener: vi.fn(),
    readyState: 1 // OPEN
  };
  
  global.WebSocket = Object.assign(
    vi.fn(() => mockWs),
    {
      CONNECTING: 0,
      OPEN: 1,
      CLOSING: 2,
      CLOSED: 3
    }
  ) as any;
};

/**
 * Mock fetch for controlled testing
 * Simple stub that returns success by default
 */
export const setupFetchMock = (response = {}) => {
  global.fetch = vi.fn().mockResolvedValue({
    ok: true,
    json: async () => response
  });
};

/**
 * Mock audio APIs not provided by happy-dom
 * Minimal stubs for testing audio features
 */
export const setupAudioMocks = () => {
  global.MediaStream = vi.fn();
  
  const mockMediaRecorder = {
    start: vi.fn(),
    stop: vi.fn(),
    pause: vi.fn(),
    resume: vi.fn(),
    state: 'inactive' as RecordingState,
    ondataavailable: null,
    onerror: null,
    onstop: null,
    onstart: null,
    onpause: null,
    onresume: null,
    audioBitsPerSecond: 0,
    videoBitsPerSecond: 0,
    stream: {} as MediaStream,
    mimeType: '',
    addEventListener: vi.fn(),
    removeEventListener: vi.fn(),
    dispatchEvent: vi.fn(),
    requestData: vi.fn()
  };
  
  global.MediaRecorder = Object.assign(
    vi.fn(() => mockMediaRecorder),
    { isTypeSupported: vi.fn(() => true) }
  ) as any;
  
  global.AudioContext = vi.fn(() => ({
    createMediaStreamSource: vi.fn(() => ({ connect: vi.fn(), disconnect: vi.fn() })),
    createScriptProcessor: vi.fn(() => ({ 
      connect: vi.fn(),
      disconnect: vi.fn(),
      onaudioprocess: null
    })),
    createAnalyser: vi.fn(() => ({
      connect: vi.fn(),
      disconnect: vi.fn(),
      getByteFrequencyData: vi.fn()
    })),
    createGain: vi.fn(() => ({ connect: vi.fn(), disconnect: vi.fn(), gain: { value: 1 } })),
    createBufferSource: vi.fn(() => ({ connect: vi.fn(), start: vi.fn(), stop: vi.fn() })),
    createOscillator: vi.fn(() => ({ connect: vi.fn(), start: vi.fn(), stop: vi.fn() })),
    decodeAudioData: vi.fn().mockResolvedValue({}),
    destination: {},
    sampleRate: 48000,
    currentTime: 0,
    state: 'running' as AudioContextState,
    baseLatency: 0,
    outputLatency: 0,
    close: vi.fn().mockResolvedValue(undefined),
    suspend: vi.fn().mockResolvedValue(undefined),
    resume: vi.fn().mockResolvedValue(undefined),
    addEventListener: vi.fn(),
    removeEventListener: vi.fn(),
    dispatchEvent: vi.fn(),
    onstatechange: null
  })) as any;
};

/**
 * Clean up all mocks after tests
 * Simple cleanup to prevent test pollution
 */
export const cleanupMocks = () => {
  vi.clearAllMocks();
  vi.restoreAllMocks();
};
/**
 * Base Mock Utilities for UI Package Testing
 * Following MOCKING_GUIDE.md principles:
 * - Keep it simple (Level 1 mocking - simple stubs)
 * - Helpers under 10 lines each
 * - Make it obvious, not clever
 * - All packages use happy-dom environment
 */

import { vi } from 'vitest';
import { render as rtlRender, RenderOptions } from '@testing-library/react';
import React from 'react';
import type { RealtimeClient, ConnectionState } from '@agentc/realtime-core';
import { AgentCProvider } from '@agentc/realtime-react';

/**
 * Create a mock RealtimeClient instance for UI testing
 * Simple stub with common UI-related methods
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
 * Create mock hook returns for common AgentC hooks
 * Simple values for testing UI components
 */
export const mockHooks = {
  useConnection: () => ({
    isConnected: false,
    connectionState: 'disconnected' as const,
    connect: vi.fn(),
    disconnect: vi.fn(),
    statistics: {
      latency: 0,
      messagesReceived: 0,
      messagesSent: 0
    }
  }),
  
  useAudio: () => ({
    isMicrophoneActive: false,
    volume: 0,
    isOutputActive: false,
    startMicrophone: vi.fn(),
    stopMicrophone: vi.fn(),
    canInterrupt: false
  }),
  
  useChat: () => ({
    messages: [],
    sendText: vi.fn(),
    clearMessages: vi.fn(),
    isLoading: false
  }),
  
  useTurnState: () => ({
    currentTurn: 'user' as const,
    canSendAudio: true,
    canSendText: true,
    isProcessing: false
  }),
  
  useVoiceModel: () => ({
    currentVoice: null,
    availableVoices: [],
    setVoice: vi.fn(),
    isLoading: false
  }),
  
  useAvatar: () => ({
    isAvatarActive: false,
    avatarUrl: null,
    startAvatar: vi.fn(),
    stopAvatar: vi.fn(),
    isLoading: false
  })
};

/**
 * Custom render with AgentCProvider wrapper
 * Provides default mock client and context for UI components
 */
export const render = (
  ui: React.ReactElement,
  options?: Omit<RenderOptions, 'wrapper'> & {
    apiUrl?: string;
    authToken?: string;
    autoConnect?: boolean;
    debug?: boolean;
  }
): ReturnType<typeof rtlRender> => {
  const AllTheProviders = ({ children }: { children: React.ReactNode }) => (
    <AgentCProvider
      apiUrl={options?.apiUrl || 'ws://test.com/rt/ws'}
      authToken={options?.authToken || 'test-token'}
      autoConnect={options?.autoConnect ?? false}
      debug={options?.debug ?? false}
    >
      {children}
    </AgentCProvider>
  );

  return rtlRender(ui, { 
    wrapper: AllTheProviders, 
    ...options 
  });
};

/**
 * Mock WebSocket for happy-dom environment
 * Standard mock for all UI package tests
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
 * Minimal stubs for testing audio UI components
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
 * Mock DOM APIs not provided by happy-dom
 * For UI components that use advanced browser features
 */
export const setupBrowserMocks = () => {
  // Intersection Observer
  global.IntersectionObserver = vi.fn(() => ({
    observe: vi.fn(),
    unobserve: vi.fn(),
    disconnect: vi.fn(),
    takeRecords: vi.fn(() => []),
    root: null,
    rootMargin: '',
    thresholds: []
  })) as any;
  
  // Resize Observer
  global.ResizeObserver = vi.fn(() => ({
    observe: vi.fn(),
    unobserve: vi.fn(),
    disconnect: vi.fn()
  })) as any;
  
  // Clipboard API
  Object.assign(navigator, {
    clipboard: {
      writeText: vi.fn().mockResolvedValue(undefined),
      readText: vi.fn().mockResolvedValue('clipboard content')
    }
  });
  
  // Notification API
  const mockNotification = vi.fn(() => ({
    close: vi.fn(),
    addEventListener: vi.fn(),
    removeEventListener: vi.fn(),
    dispatchEvent: vi.fn()
  }));
  
  // Add static properties
  (mockNotification as any).permission = 'granted';
  (mockNotification as any).requestPermission = vi.fn().mockResolvedValue('granted');
  
  global.Notification = mockNotification as any;
};

/**
 * Setup all common mocks for UI testing
 * Call in beforeEach for comprehensive mock setup
 */
export const setupAllMocks = () => {
  setupWebSocketMock();
  setupAudioMocks();
  setupBrowserMocks();
};

/**
 * Clean up all mocks after tests
 * Simple cleanup to prevent test pollution
 */
export const cleanupMocks = () => {
  vi.clearAllMocks();
  vi.restoreAllMocks();
};

// Re-export testing utilities for convenience
export * from '@testing-library/react';
export { userEvent } from '@testing-library/user-event';
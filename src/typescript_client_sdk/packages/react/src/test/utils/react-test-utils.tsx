/**
 * React Testing Utilities
 * Helper components and functions for testing React components
 */

import React, { ReactElement, ReactNode } from 'react';
import { render, RenderOptions, RenderResult } from '@testing-library/react';
import { vi } from 'vitest';
import { AgentCProvider, AgentCProviderProps } from '../../providers/AgentCProvider';
import type { RealtimeClient } from '@agentc/realtime-core';

/**
 * Create a mock SessionManager for testing
 * Provides all methods required by React hooks
 */
export const createMockSessionManager = () => {
  // Store event handlers for manual triggering in tests
  const eventHandlers = new Map<string, Set<Function>>();
  
  return {
    // Event methods
    on: vi.fn((event: string, handler: Function) => {
      if (!eventHandlers.has(event)) {
        eventHandlers.set(event, new Set());
      }
      eventHandlers.get(event)!.add(handler);
      return undefined;
    }),
    off: vi.fn((event: string, handler: Function) => {
      eventHandlers.get(event)?.delete(handler);
      return undefined;
    }),
    emit: vi.fn((event: string, data: any) => {
      const handlers = eventHandlers.get(event);
      if (handlers) {
        handlers.forEach(handler => handler(data));
      }
      return undefined;
    }),
    
    // Session management methods
    getCurrentSession: vi.fn(() => null),
    getCurrentSessionId: vi.fn(() => null),
    setCurrentSession: vi.fn(),
    getSession: vi.fn(() => null),
    getAllSessions: vi.fn(() => new Map()),
    getSessionIds: vi.fn(() => []),
    
    // Message methods
    addUserMessage: vi.fn(),
    
    // Session index methods
    getSessionIndex: vi.fn(() => []),
    getTotalSessionCount: vi.fn(() => 0),
    hasMoreSessions: vi.fn(() => false),
    requestMoreSessions: vi.fn(),
    setSessionIndex: vi.fn(),
    
    // Utility methods
    clearSession: vi.fn(() => true),
    reset: vi.fn(),
    cleanup: vi.fn(),
    destroy: vi.fn(),
    updateSessionMetadata: vi.fn(() => true),
    updateSessionName: vi.fn(() => true),
    getStatistics: vi.fn(() => ({
      totalSessions: 0,
      totalMessages: 0,
      totalTokens: 0,
      currentSessionId: null,
      hasActiveAccumulation: false,
      sessionIndexCount: 0,
      totalSessionsAvailable: 0
    })),
    
    // Expose event handlers for test assertions
    _eventHandlers: eventHandlers,
    
    // Helper to trigger events in tests
    _triggerEvent: (event: string, data: any) => {
      const handlers = eventHandlers.get(event);
      if (handlers) {
        handlers.forEach(handler => handler(data));
      }
    }
  };
};

/**
 * Default mock client for testing
 */
export const createMockClient = (): Partial<RealtimeClient> => {
  const mockSessionManager = createMockSessionManager();
  
  return {
    getConnectionState: vi.fn(() => 'disconnected'),
    isConnected: vi.fn(() => false),
    connect: vi.fn().mockResolvedValue(undefined),
    disconnect: vi.fn(),
    destroy: vi.fn(),
    sendText: vi.fn(),
    sendBinaryFrame: vi.fn(),
    sendEvent: vi.fn(),
    updateSession: vi.fn(),
    setAudioEnabled: vi.fn(),
    // Session manager - CRITICAL for Phase 7 chat features
    getSessionManager: vi.fn(() => mockSessionManager),
    getStatistics: vi.fn(() => ({
      connection: {
        state: 'disconnected',
        latency: 0,
        packetsLost: 0,
        reconnectAttempts: 0
      },
      audio: {
        inputLevel: 0,
        outputLevel: 0,
        isRecording: false,
        isMuted: false
      },
      session: {
        duration: 0,
        messageCount: 0,
        turnCount: 0
      }
    })),
    // Audio methods
    getAudioStatus: vi.fn(() => ({
      isRecording: false,
      isStreaming: false,
      isProcessing: false,
      hasPermission: false,
      currentLevel: 0,
      averageLevel: 0,
      isPlaying: false,
      bufferSize: 0,
      volume: 1,
      isAudioEnabled: false,
      isInputEnabled: false,
      isOutputEnabled: false
    })),
    startAudioRecording: vi.fn().mockResolvedValue(undefined),
    stopAudioRecording: vi.fn(),
    startAudioStreaming: vi.fn(),
    stopAudioStreaming: vi.fn(),
    setAudioVolume: vi.fn(),
    // Turn manager
    getTurnManager: vi.fn(() => ({
      getTurnState: vi.fn(() => ({
        currentTurn: null,
        isUserTurn: false,
        isAssistantTurn: false,
        canSendInput: true,
        pendingTurns: [],
        turnHistory: []
      })),
      startTurn: vi.fn(),
      endTurn: vi.fn(),
      cancelTurn: vi.fn(),
      canSendInput: true,
      on: vi.fn(),
      off: vi.fn(),
      emit: vi.fn()
    })),
    // Voice methods
    getVoiceConfig: vi.fn(() => ({
      currentVoice: null,
      availableVoices: [],
      isAvatarMode: false,
      avatarVoice: null
    })),
    setVoice: vi.fn().mockResolvedValue(undefined),
    enableAvatarMode: vi.fn().mockResolvedValue(undefined),
    disableAvatarMode: vi.fn().mockResolvedValue(undefined),
    setVoiceConfig: vi.fn().mockResolvedValue(undefined),
    // Avatar manager
    getAvatarManager: vi.fn(() => ({
      getAvatarState: vi.fn(() => ({
        isEnabled: false,
        currentAvatar: null,
        session: null,
        isConnected: false,
        isStreaming: false,
        error: null
      })),
      enableAvatar: vi.fn().mockResolvedValue(undefined),
      disableAvatar: vi.fn().mockResolvedValue(undefined),
      startSession: vi.fn().mockResolvedValue({ sessionId: 'session-123' }),
      endSession: vi.fn().mockResolvedValue(undefined),
      switchAvatar: vi.fn().mockResolvedValue(undefined),
      getAvailableAvatars: vi.fn(() => []),
      on: vi.fn(),
      off: vi.fn()
    })),
    // Event methods
    on: vi.fn(),
    off: vi.fn(),
    once: vi.fn(),
    emit: vi.fn(),
    removeAllListeners: vi.fn()
  };
};

/**
 * Test wrapper component with AgentC Provider
 */
interface TestWrapperProps {
  children: ReactNode;
  client?: Partial<RealtimeClient>;
  providerProps?: Partial<AgentCProviderProps>;
}

export function TestWrapper({ 
  children, 
  client = createMockClient(),
  providerProps = {}
}: TestWrapperProps) {
  const defaultProps = {
    apiUrl: 'wss://test.example.com/ws',
    ...providerProps
  };
  
  return (
    <AgentCProvider 
      client={client as RealtimeClient}
      {...defaultProps}
    >
      {children}
    </AgentCProvider>
  );
}

/**
 * Custom render function with providers
 */
interface CustomRenderOptions extends Omit<RenderOptions, 'wrapper'> {
  client?: Partial<RealtimeClient>;
  providerProps?: Partial<AgentCProviderProps>;
}

export function renderWithProvider(
  ui: ReactElement,
  options: CustomRenderOptions = {}
): RenderResult & { client: Partial<RealtimeClient> } {
  const { 
    client = createMockClient(), 
    providerProps = {},
    ...renderOptions 
  } = options;

  const result = render(ui, {
    wrapper: ({ children }) => (
      <TestWrapper client={client} providerProps={providerProps}>
        {children}
      </TestWrapper>
    ),
    ...renderOptions
  });

  return {
    ...result,
    client
  };
}

/**
 * Mock hook utilities
 */
export const mockUseConnection = () => ({
  isConnected: false,
  connectionState: 'disconnected' as const,
  connect: vi.fn(),
  disconnect: vi.fn(),
  error: null,
  statistics: {
    latency: 0,
    packetsLost: 0,
    reconnectAttempts: 0
  }
});

export const mockUseAudio = () => ({
  isRecording: false,
  audioLevel: 0,
  startRecording: vi.fn(),
  stopRecording: vi.fn(),
  mute: vi.fn(),
  unmute: vi.fn(),
  isMuted: false,
  error: null
});

export const mockUseChat = () => ({
  messages: [],
  sendMessage: vi.fn(),
  clearMessages: vi.fn(),
  isTyping: false,
  error: null
});

export const mockUseTurn = () => ({
  currentTurn: null,
  isUserTurn: false,
  isAssistantTurn: false,
  turnHistory: [],
  startTurn: vi.fn(),
  endTurn: vi.fn()
});

export const mockUseSession = () => ({
  sessionId: 'test-session',
  isActive: true,
  startSession: vi.fn(),
  endSession: vi.fn(),
  sessionData: {
    startTime: new Date().toISOString(),
    messageCount: 0,
    turnCount: 0
  }
});

/**
 * Create mock context values
 */
export const createMockContextValue = () => ({
  client: createMockClient() as RealtimeClient,
  connection: mockUseConnection(),
  audio: mockUseAudio(),
  chat: mockUseChat(),
  turn: mockUseTurn(),
  session: mockUseSession()
});

/**
 * Wait for async updates in React components
 */
export async function waitForAsync(timeout: number = 0): Promise<void> {
  return new Promise(resolve => setTimeout(resolve, timeout));
}

/**
 * Trigger a re-render of a component
 */
export function triggerRerender(component: RenderResult): void {
  component.rerender(component.container.firstChild as ReactElement);
}

/**
 * Mock implementation of useAgentC hook
 */
export const createMockUseAgentC = () => {
  const contextValue = createMockContextValue();
  
  return vi.fn(() => contextValue);
};

/**
 * Test IDs for common elements
 */
export const testIds = {
  // Connection
  connectionButton: 'connection-button',
  connectionStatus: 'connection-status',
  connectionIndicator: 'connection-indicator',
  
  // Audio
  audioButton: 'audio-button',
  audioLevel: 'audio-level',
  muteButton: 'mute-button',
  
  // Chat
  chatContainer: 'chat-container',
  chatInput: 'chat-input',
  chatMessage: 'chat-message',
  sendButton: 'send-button',
  
  // Turn
  turnIndicator: 'turn-indicator',
  userTurn: 'user-turn',
  assistantTurn: 'assistant-turn',
  
  // Session
  sessionInfo: 'session-info',
  sessionTimer: 'session-timer'
};

/**
 * Accessibility testing utilities
 */
export const a11y = {
  /**
   * Check if element has accessible name
   */
  hasAccessibleName: (element: HTMLElement): boolean => {
    return !!(
      element.getAttribute('aria-label') ||
      element.getAttribute('aria-labelledby') ||
      element.textContent?.trim()
    );
  },

  /**
   * Check if element has accessible description
   */
  hasAccessibleDescription: (element: HTMLElement): boolean => {
    return !!(
      element.getAttribute('aria-description') ||
      element.getAttribute('aria-describedby')
    );
  },

  /**
   * Check if element is keyboard accessible
   */
  isKeyboardAccessible: (element: HTMLElement): boolean => {
    const tabIndex = element.getAttribute('tabindex');
    const isNaturallyFocusable = ['BUTTON', 'A', 'INPUT', 'SELECT', 'TEXTAREA'].includes(
      element.tagName
    );
    
    return isNaturallyFocusable || (tabIndex !== null && parseInt(tabIndex) >= 0);
  },

  /**
   * Check if element has proper ARIA role
   */
  hasProperRole: (element: HTMLElement): boolean => {
    const role = element.getAttribute('role');
    const tagName = element.tagName;
    
    // Some elements have implicit roles
    const implicitRoles: Record<string, string> = {
      BUTTON: 'button',
      NAV: 'navigation',
      MAIN: 'main',
      ARTICLE: 'article',
      SECTION: 'region'
    };
    
    return role !== null || implicitRoles[tagName] !== undefined;
  }
};

/**
 * Create a mock message for testing
 */
export function createMockMessage(overrides: any = {}) {
  return {
    id: `msg_${Date.now()}`,
    role: 'assistant',
    content: 'Test message',
    timestamp: new Date().toISOString(),
    ...overrides
  };
}

/**
 * Create a mock turn event for testing
 */
export function createMockTurnEvent(type: 'start' | 'end', turnType: 'user' | 'assistant') {
  return {
    type: `turn_${type}`,
    turn_id: `turn_${Date.now()}`,
    turn_type: turnType,
    timestamp: new Date().toISOString()
  };
}

/**
 * Create a mock error for testing
 */
export function createMockError(code: string = 'TEST_ERROR', message: string = 'Test error') {
  return {
    type: 'error',
    error: {
      code,
      message,
      details: {}
    },
    timestamp: new Date().toISOString()
  };
}
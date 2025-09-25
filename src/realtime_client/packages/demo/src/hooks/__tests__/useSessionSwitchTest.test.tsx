/**
 * Tests for useSessionSwitchTest hook
 * 
 * Validates that the session switching test utilities properly detect:
 * - Message leakage between sessions
 * - Message accumulation issues
 * - Loading state problems
 * - Pattern detection functionality
 */

import { describe, it, expect, beforeEach, vi, afterEach } from 'vitest';
import { renderHook, act, waitFor } from '@testing-library/react';
import { useSessionSwitchTest, type TestResult, type MessageTraceEntry } from '../useSessionSwitchTest';
import { useChat, useSession } from '@agentc/realtime-react';
import type { Message } from '@agentc/realtime-core';

// Mock the React SDK hooks
vi.mock('@agentc/realtime-react', () => ({
  useChat: vi.fn(),
  useSession: vi.fn(),
  useConnection: vi.fn(() => ({
    isConnected: true,
    connectionState: 'connected',
    connect: vi.fn(),
    disconnect: vi.fn(),
    error: null
  })),
  useChatSessionList: vi.fn(() => ({
    sessions: [],
    currentSession: null,
    switchSession: vi.fn(),
    createSession: vi.fn(),
    deleteSession: vi.fn(),
    isLoading: false
  }))
}));

describe('useSessionSwitchTest', () => {
  const mockUseChat = vi.mocked(useChat);
  const mockUseSession = vi.mocked(useSession);
  
  // Test data
  const mockMessages: Record<string, Message[]> = {
    session1: [
      {
        id: 'msg1',
        role: 'user',
        content: [{ type: 'text', text: 'Hello from session 1' }],
        timestamp: new Date().toISOString(),
        status: 'complete'
      },
      {
        id: 'msg2',
        role: 'assistant',
        content: [{ type: 'text', text: 'Response in session 1' }],
        timestamp: new Date().toISOString(),
        status: 'complete'
      }
    ],
    session2: [
      {
        id: 'msg3',
        role: 'user',
        content: [{ type: 'text', text: 'Hello from session 2' }],
        timestamp: new Date().toISOString(),
        status: 'complete'
      },
      {
        id: 'msg4',
        role: 'assistant',
        content: [{ type: 'text', text: 'Response in session 2' }],
        timestamp: new Date().toISOString(),
        status: 'complete'
      }
    ]
  };
  
  let currentSessionId = 'session1';
  let currentMessages: Message[] = [];
  let isConnected = true;
  let isLoading = false;
  
  beforeEach(() => {
    vi.clearAllMocks();
    vi.useFakeTimers();
    
    // Reset test state
    currentSessionId = 'session1';
    currentMessages = [...mockMessages.session1];
    isConnected = true;
    isLoading = false;
    
    // Setup mock implementations
    mockUseChat.mockImplementation(() => ({
      messages: currentMessages,
      sendMessage: vi.fn(),
      clearMessages: vi.fn(() => {
        currentMessages = [];
      }),
      isConnected,
      isLoading,
      error: null
    } as any));
    
    mockUseSession.mockImplementation(() => ({
      currentSessionId,
      switchSession: vi.fn((newId: string) => {
        isLoading = true;
        setTimeout(() => {
          currentSessionId = newId;
          currentMessages = mockMessages[newId] || [];
          isLoading = false;
        }, 100);
      }),
      createSession: vi.fn(),
      deleteSession: vi.fn(),
      sessions: [],
      isLoading
    } as any));
  });
  
  afterEach(() => {
    vi.useRealTimers();
  });
  
  describe('Message Leakage Detection', () => {
    it('should detect when messages from one session appear in another', async () => {
      const { result } = renderHook(() => useSessionSwitchTest());
      
      // Start rapid switch test
      act(() => {
        result.current.runScenario('rapid-switching');
      });
      
      expect(result.current.isRunning).toBe(true);
      
      // Simulate message leakage during switch
      mockUseSession.mockImplementationOnce(() => ({
        currentSessionId: 'session2',
        switchSession: vi.fn(),
        createSession: vi.fn(),
        deleteSession: vi.fn(),
        sessions: [],
        isLoading: false
      } as any));
      
      // Messages from session1 leak into session2
      currentMessages = [
        ...mockMessages.session1,  // Leaked messages
        ...mockMessages.session2
      ];
      
      // Wait for test to complete
      await act(async () => {
        vi.advanceTimersByTime(1500);
      });
      
      await waitFor(() => {
        expect(result.current.isRunning).toBe(false);
      });
      
      // Check test results
      const rapidResult = result.current.results.find(r => r.scenario === 'rapid-switching');
      expect(rapidResult).toBeDefined();
      expect(rapidResult?.passed).toBe(false);
      expect(rapidResult?.details.issues).toContain(
        expect.stringMatching(/message.*leak|unexpected.*message/i)
      );
    });
    
    it('should pass when messages are properly isolated between sessions', async () => {
      const { result } = renderHook(() => useSessionSwitchTest());
      
      // Setup proper session switching
      let switchCount = 0;
      mockUseSession.mockImplementation(() => ({
        currentSessionId,
        switchSession: vi.fn((newId: string) => {
          switchCount++;
          currentSessionId = newId;
          currentMessages = mockMessages[newId] || [];
        }),
        createSession: vi.fn(),
        deleteSession: vi.fn(),
        sessions: [],
        isLoading: false
      } as any));
      
      // Run rapid switch test
      await act(async () => {
        await result.current.runScenario('rapid-switching');
      });
      
      // Advance timers to complete test
      await act(async () => {
        vi.advanceTimersByTime(1500);
      });
      
      await waitFor(() => {
        expect(result.current.isRunning).toBe(false);
      });
      
      // Should pass with proper isolation
      const rapidResult = result.current.results.find(r => r.scenario === 'rapid-switching');
      expect(rapidResult?.passed).toBe(true);
      expect(rapidResult?.details.issues).toHaveLength(0);
    });
  });
  
  describe('Message Accumulation Detection', () => {
    it('should detect when messages accumulate across sessions', async () => {
      const { result } = renderHook(() => useSessionSwitchTest());
      
      // Setup accumulation scenario
      let messageAccumulator: Message[] = [];
      
      mockUseChat.mockImplementation(() => ({
        messages: messageAccumulator,
        sendMessage: vi.fn(),
        clearMessages: vi.fn(),
        isConnected: true,
        isLoading: false,
        error: null
      } as any));
      
      mockUseSession.mockImplementation(() => ({
        currentSessionId,
        switchSession: vi.fn((newId: string) => {
          currentSessionId = newId;
          // Messages accumulate instead of clearing
          messageAccumulator = [
            ...messageAccumulator,
            ...(mockMessages[newId] || [])
          ];
        }),
        createSession: vi.fn(),
        deleteSession: vi.fn(),
        sessions: [],
        isLoading: false
      } as any));
      
      // Run populated session test
      act(() => {
        result.current.runTest('populated');
      });
      
      // Advance timers
      await act(async () => {
        vi.advanceTimersByTime(2000);
      });
      
      await waitFor(() => {
        expect(result.current.isRunning).toBe(false);
      });
      
      // Should detect accumulation
      const populatedResult = result.current.results.find(r => r.scenario === 'populated-session');
      expect(populatedResult?.passed).toBe(false);
      expect(populatedResult?.details.issues).toContain(
        expect.stringMatching(/accumula|not.*clear/i)
      );
    });
  });
  
  describe('Empty Session Testing', () => {
    it('should verify empty sessions have no messages', async () => {
      const { result } = renderHook(() => useSessionSwitchTest());
      
      // Setup empty session scenario
      mockUseSession.mockImplementation(() => ({
        currentSessionId: 'empty-session',
        switchSession: vi.fn((newId: string) => {
          currentSessionId = newId;
          currentMessages = []; // Empty session
        }),
        createSession: vi.fn(),
        deleteSession: vi.fn(),
        sessions: [],
        isLoading: false
      } as any));
      
      mockUseChat.mockImplementation(() => ({
        messages: [],
        sendMessage: vi.fn(),
        clearMessages: vi.fn(),
        isConnected: true,
        isLoading: false,
        error: null
      } as any));
      
      // Run empty session test
      await act(async () => {
        await result.current.runScenario('empty-session');
      });
      
      await act(async () => {
        vi.advanceTimersByTime(1000);
      });
      
      await waitFor(() => {
        expect(result.current.isRunning).toBe(false);
      });
      
      // Should pass for properly empty session
      const emptyResult = result.current.results.find(r => r.scenario === 'empty-session');
      expect(emptyResult?.passed).toBe(true);
      expect(result.current.messageCount).toBe(0);
    });
    
    it('should fail if empty session contains messages', async () => {
      const { result } = renderHook(() => useSessionSwitchTest());
      
      // Setup scenario where "empty" session has messages
      mockUseChat.mockImplementation(() => ({
        messages: mockMessages.session1, // Not empty!
        sendMessage: vi.fn(),
        clearMessages: vi.fn(),
        isConnected: true,
        isLoading: false,
        error: null
      } as any));
      
      // Run empty session test
      await act(async () => {
        await result.current.runScenario('empty-session');
      });
      
      await act(async () => {
        vi.advanceTimersByTime(1000);
      });
      
      await waitFor(() => {
        expect(result.current.isRunning).toBe(false);
      });
      
      // Should fail
      const emptyResult = result.current.results.find(r => r.scenario === 'empty-session');
      expect(emptyResult?.passed).toBe(false);
      expect(emptyResult?.details.issues).toContain(
        expect.stringMatching(/expected.*empty|should.*no messages/i)
      );
    });
  });
  
  describe('Loading State Testing', () => {
    it('should detect messages appearing during loading state', async () => {
      const { result } = renderHook(() => useSessionSwitchTest());
      
      // Setup loading state scenario
      mockUseSession.mockImplementation(() => ({
        currentSessionId,
        switchSession: vi.fn(),
        createSession: vi.fn(),
        deleteSession: vi.fn(),
        sessions: [],
        isLoading: true // Always loading
      } as any));
      
      // Messages appear during loading
      mockUseChat.mockImplementation(() => ({
        messages: mockMessages.session1,
        sendMessage: vi.fn(),
        clearMessages: vi.fn(),
        isConnected: true,
        isLoading: true,
        error: null
      } as any));
      
      // Run loading state test
      await act(async () => {
        await result.current.runScenario('loading-state');
      });
      
      await act(async () => {
        vi.advanceTimersByTime(1500);
      });
      
      await waitFor(() => {
        expect(result.current.isRunning).toBe(false);
      });
      
      // Should detect issue
      const loadingResult = result.current.results.find(r => r.scenario === 'loading-state');
      expect(loadingResult?.passed).toBe(false);
      expect(loadingResult?.details.issues).toContain(
        expect.stringMatching(/loading.*messages|messages.*during.*loading/i)
      );
    });
    
    it('should pass when no messages appear during loading', async () => {
      const { result } = renderHook(() => useSessionSwitchTest());
      
      // Setup proper loading state
      let loadingPhase = true;
      
      mockUseSession.mockImplementation(() => ({
        currentSessionId,
        switchSession: vi.fn(() => {
          loadingPhase = true;
          setTimeout(() => {
            loadingPhase = false;
          }, 200);
        }),
        createSession: vi.fn(),
        deleteSession: vi.fn(),
        sessions: [],
        isLoading: loadingPhase
      } as any));
      
      mockUseChat.mockImplementation(() => ({
        messages: loadingPhase ? [] : currentMessages, // No messages during loading
        sendMessage: vi.fn(),
        clearMessages: vi.fn(),
        isConnected: true,
        isLoading: loadingPhase,
        error: null
      } as any));
      
      // Run loading state test
      await act(async () => {
        await result.current.runScenario('loading-state');
      });
      
      await act(async () => {
        vi.advanceTimersByTime(1500);
      });
      
      await waitFor(() => {
        expect(result.current.isRunning).toBe(false);
      });
      
      // Should pass
      const loadingResult = result.current.results.find(r => r.scenario === 'loading-state');
      expect(loadingResult?.passed).toBe(true);
      expect(loadingResult?.details.issues).toHaveLength(0);
    });
  });
  
  describe('Message Trace Recording', () => {
    it('should record detailed message traces during tests', async () => {
      const { result } = renderHook(() => useSessionSwitchTest());
      
      // Run a test
      act(() => {
        result.current.runScenario('rapid-switching');
      });
      
      await act(async () => {
        vi.advanceTimersByTime(1500);
      });
      
      await waitFor(() => {
        expect(result.current.isRunning).toBe(false);
      });
      
      // Should have message traces
      expect(result.current.messageTrace).toBeDefined();
      expect(result.current.messageTrace.length).toBeGreaterThan(0);
      
      // Verify trace structure
      const trace = result.current.messageTrace[0];
      expect(trace).toHaveProperty('timestamp');
      expect(trace).toHaveProperty('sessionId');
      expect(trace).toHaveProperty('messageCount');
      expect(trace).toHaveProperty('event');
    });
  });
  
  describe('Multiple Test Execution', () => {
    it('should clear previous results when running new test', () => {
      const { result } = renderHook(() => useSessionSwitchTest());
      
      // Run first test
      act(() => {
        result.current.runScenario('empty-session');
      });
      
      // Immediately run another test
      act(() => {
        result.current.runScenario('rapid-switching');
      });
      
      // Should only be running rapid test
      expect(result.current.currentScenario).toBe('rapid-switching');
      
      // Previous test result should be cleared from active state
      expect(result.current.isRunning).toBe(true);
    });
    
    it('should handle running all tests sequentially', async () => {
      const { result } = renderHook(() => useSessionSwitchTest());
      
      // Run all tests
      await act(async () => {
        await result.current.runAllScenarios();
      });
      
      expect(result.current.isRunning).toBe(true);
      
      // Advance through all tests
      await act(async () => {
        vi.advanceTimersByTime(10000); // Enough time for all tests
      });
      
      await waitFor(() => {
        expect(result.current.isRunning).toBe(false);
      });
      
      // Should have results for all test types
      expect(result.current.results.find(r => r.scenario === 'rapid-switching')).toBeDefined();
      expect(result.current.results.find(r => r.scenario === 'empty-session')).toBeDefined();
      expect(result.current.results.find(r => r.scenario === 'populated-session')).toBeDefined();
      expect(result.current.results.find(r => r.scenario === 'loading-state')).toBeDefined();
    });
  });
  
  describe('Clear Functionality', () => {
    it('should clear results and traces', async () => {
      const { result } = renderHook(() => useSessionSwitchTest());
      
      // Run a test to generate results
      await act(async () => {
        await result.current.runScenario('empty-session');
      });
      
      await act(async () => {
        vi.advanceTimersByTime(1000);
      });
      
      await waitFor(() => {
        expect(result.current.isRunning).toBe(false);
      });
      
      // Verify results exist
      expect(result.current.results.find(r => r.scenario === 'empty-session')).toBeDefined();
      expect(result.current.messageTrace.length).toBeGreaterThan(0);
      
      // Clear results
      act(() => {
        result.current.clearResults();
      });
      
      // Should be cleared
      expect(result.current.results).toEqual([]);
      expect(result.current.messageTrace).toEqual([]);
    });
  });
});
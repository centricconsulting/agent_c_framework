/**
 * Tests for useTestSession hook
 */

import React from 'react';
import { describe, it, expect, vi, beforeEach, afterEach } from 'vitest';
import { renderHook, waitFor } from '@testing-library/react';
import { act } from 'react';
import { useTestSession } from '../use-test-session';
import { TestSession } from '../../types/test-session';
import { testSessionManager } from '../../services/test-session-manager';

// Mock dependencies
vi.mock('@agentc/realtime-react', () => ({
  useChat: vi.fn(() => ({
    clearMessages: vi.fn()
  })),
  useChatSessionList: vi.fn(() => ({
    selectSession: vi.fn()
  })),
  useConnection: vi.fn(() => ({
    isConnected: true
  }))
}));

// Mock the test session manager
vi.mock('../../services/test-session-manager', () => {
  const TestSessionManagerMock = {
    instance: null,
    getInstance: function() {
      if (!this.instance) {
        this.instance = {
          isEnabled: vi.fn(() => true),
          getConfig: vi.fn(() => ({
            enabled: true,
            scenarios: [],
            showTestControls: true,
            autoLoadOnMount: false,
            currentScenarioId: null
          })),
          getScenarios: vi.fn(() => [
            { id: 'test-1', name: 'Test 1', filePath: '/test1.json' },
            { id: 'test-2', name: 'Test 2', filePath: '/test2.json' }
          ]),
          getCurrentScenario: vi.fn(() => null),
          setTestMode: vi.fn(),
          setShowTestControls: vi.fn(),
          loadScenario: vi.fn(),
          resumeSession: vi.fn(),
          clearCurrentSession: vi.fn(),
          clearCaches: vi.fn(),
          setResumeCallback: vi.fn()
        };
      }
      return this.instance;
    }
  };

  return {
    testSessionManager: TestSessionManagerMock.getInstance(),
    TestSessionManager: TestSessionManagerMock
  };
});

describe('useTestSession hook', () => {
  let mockSelectSession: ReturnType<typeof vi.fn>;
  let mockClearMessages: ReturnType<typeof vi.fn>;
  let mockUseConnection: ReturnType<typeof vi.fn>;
  
  beforeEach(async () => {
    vi.clearAllMocks();
    
    // Reset mocks
    mockSelectSession = vi.fn();
    mockClearMessages = vi.fn();
    mockUseConnection = vi.fn(() => ({ isConnected: true }));
    
    // Update mocked modules
    const realtimeReact = await import('@agentc/realtime-react');
    (realtimeReact.useChat as any).mockReturnValue({
      clearMessages: mockClearMessages
    });
    (realtimeReact.useChatSessionList as any).mockReturnValue({
      selectSession: mockSelectSession
    });
    (realtimeReact.useConnection as any).mockImplementation(mockUseConnection);
  });

  afterEach(() => {
    vi.restoreAllMocks();
  });

  describe('Initial State', () => {
    it('should initialize with correct default state', () => {
      const { result } = renderHook(() => useTestSession());
      
      expect(result.current.testModeEnabled).toBe(true);
      expect(result.current.testConfig).toEqual({
        enabled: true,
        scenarios: [],
        showTestControls: true,
        autoLoadOnMount: false,
        currentScenarioId: null
      });
      expect(result.current.scenarios).toHaveLength(2);
      expect(result.current.currentScenario).toBeNull();
      expect(result.current.isLoading).toBe(false);
      expect(result.current.error).toBeNull();
      expect(result.current.showTestControls).toBe(true);
    });

    it('should register resume callback on mount', () => {
      renderHook(() => useTestSession());
      
      expect(testSessionManager.setResumeCallback).toHaveBeenCalledTimes(1);
      expect(testSessionManager.setResumeCallback).toHaveBeenCalledWith(expect.any(Function));
    });

    it('should only register resume callback once on multiple renders', () => {
      const { rerender } = renderHook(() => useTestSession());
      
      rerender();
      rerender();
      
      expect(testSessionManager.setResumeCallback).toHaveBeenCalledTimes(1);
    });
  });

  describe('Enable/Disable Test Mode', () => {
    it('should enable test mode correctly', async () => {
      const { result } = renderHook(() => useTestSession());
      
      await act(async () => {
        result.current.enableTestMode(true);
      });
      
      expect(testSessionManager.setTestMode).toHaveBeenCalledWith(true);
      expect(result.current.testModeEnabled).toBe(true);
    });

    it('should disable test mode and clear scenario', async () => {
      const { result } = renderHook(() => useTestSession());
      
      await act(async () => {
        result.current.enableTestMode(false);
      });
      
      expect(testSessionManager.setTestMode).toHaveBeenCalledWith(false);
      expect(testSessionManager.clearCurrentSession).toHaveBeenCalled();
      expect(result.current.testModeEnabled).toBe(false); // Should be false after disabling
      expect(result.current.currentScenario).toBeNull();
    });
  });

  describe('Load Scenario', () => {
    it('should load scenario successfully', async () => {
      const mockSession: TestSession = {
        version: 1,
        session_id: 'test-session-123',
        token_count: 100,
        context_window_size: 1000,
        session_name: 'Test Session',
        created_at: '2024-01-01T00:00:00Z',
        updated_at: '2024-01-01T00:00:00Z',
        deleted_at: null,
        user_id: 'test-user',
        metadata: {},
        messages: []
      };
      
      (testSessionManager.loadScenario as any).mockResolvedValue(mockSession);
      (testSessionManager.resumeSession as any).mockResolvedValue(undefined);
      (testSessionManager.getCurrentScenario as any).mockReturnValue({
        id: 'test-1',
        name: 'Test 1',
        filePath: '/test1.json'
      });
      
      const { result } = renderHook(() => useTestSession());
      
      await act(async () => {
        await result.current.loadScenario('test-1');
      });
      
      expect(testSessionManager.loadScenario).toHaveBeenCalledWith('test-1');
      expect(testSessionManager.resumeSession).toHaveBeenCalledWith(mockSession);
      expect(result.current.currentScenario).toEqual({
        id: 'test-1',
        name: 'Test 1',
        filePath: '/test1.json'
      });
      expect(result.current.error).toBeNull();
    });

    it('should handle load scenario errors', async () => {
      const errorMsg = 'Failed to load test data';
      (testSessionManager.loadScenario as any).mockRejectedValue(new Error(errorMsg));
      
      const { result } = renderHook(() => useTestSession());
      
      await act(async () => {
        await result.current.loadScenario('test-1');
      });
      
      expect(result.current.error).toContain(errorMsg);
      expect(result.current.isLoading).toBe(false);
    });

    it('should not load scenario when not connected', async () => {
      mockUseConnection.mockReturnValue({ isConnected: false });
      
      const { result } = renderHook(() => useTestSession());
      
      await act(async () => {
        await result.current.loadScenario('test-1');
      });
      
      expect(testSessionManager.loadScenario).not.toHaveBeenCalled();
      expect(result.current.error).toContain('Not connected to server');
    });

    it('should set loading state during scenario load', async () => {
      let resolveLoad: () => void;
      const loadPromise = new Promise<TestSession>((resolve) => {
        resolveLoad = () => resolve({
          version: 1,
          session_id: 'test',
          token_count: 0,
          context_window_size: 0,
          session_name: null,
          created_at: '',
          updated_at: '',
          deleted_at: null,
          user_id: 'test',
          metadata: {},
          messages: []
        });
      });
      
      (testSessionManager.loadScenario as any).mockReturnValue(loadPromise);
      
      const { result } = renderHook(() => useTestSession());
      
      // Start loading
      let loadPromiseResult: Promise<void>;
      act(() => {
        loadPromiseResult = result.current.loadScenario('test-1');
      });
      
      // Should be loading
      expect(result.current.isLoading).toBe(true);
      
      // Complete loading
      await act(async () => {
        resolveLoad!();
        await loadPromiseResult!;
      });
      
      // Should not be loading anymore
      expect(result.current.isLoading).toBe(false);
    });
  });

  describe('Clear Session', () => {
    it('should clear session correctly', async () => {
      const { result } = renderHook(() => useTestSession());
      
      await act(async () => {
        result.current.clearSession();
      });
      
      expect(mockClearMessages).toHaveBeenCalled();
      expect(testSessionManager.clearCurrentSession).toHaveBeenCalled();
      expect(result.current.currentScenario).toBeNull();
    });
  });

  describe('Reload Current Scenario', () => {
    it('should reload current scenario', async () => {
      const mockSession: TestSession = {
        version: 1,
        session_id: 'test-reload',
        token_count: 0,
        context_window_size: 0,
        session_name: null,
        created_at: '',
        updated_at: '',
        deleted_at: null,
        user_id: 'test',
        metadata: {},
        messages: []
      };
      
      (testSessionManager.loadScenario as any).mockResolvedValue(mockSession);
      (testSessionManager.getCurrentScenario as any).mockReturnValue({
        id: 'test-1',
        name: 'Test 1',
        filePath: '/test1.json'
      });
      
      const { result } = renderHook(() => useTestSession());
      
      // Set current scenario first
      await act(async () => {
        await result.current.loadScenario('test-1');
      });
      
      vi.clearAllMocks();
      
      // Now reload
      await act(async () => {
        await result.current.reloadCurrentScenario();
      });
      
      expect(testSessionManager.clearCaches).toHaveBeenCalled();
      expect(testSessionManager.loadScenario).toHaveBeenCalledWith('test-1');
    });

    it('should handle reload when no scenario loaded', async () => {
      const { result } = renderHook(() => useTestSession());
      
      await act(async () => {
        await result.current.reloadCurrentScenario();
      });
      
      expect(result.current.error).toContain('No scenario currently loaded');
      expect(testSessionManager.loadScenario).not.toHaveBeenCalled();
    });
  });

  describe('Show Test Controls', () => {
    it('should update show test controls', async () => {
      const { result } = renderHook(() => useTestSession());
      
      await act(async () => {
        result.current.setShowTestControls(false);
      });
      
      expect(testSessionManager.setShowTestControls).toHaveBeenCalledWith(false);
      
      await act(async () => {
        result.current.setShowTestControls(true);
      });
      
      expect(testSessionManager.setShowTestControls).toHaveBeenCalledWith(true);
    });
  });

  describe('Resume Callback', () => {
    it('should execute resume callback with selectSession', async () => {
      renderHook(() => useTestSession());
      
      // Get the callback that was registered
      const resumeCallback = (testSessionManager.setResumeCallback as any).mock.calls[0][0];
      
      const testSession: TestSession = {
        version: 1,
        session_id: 'resume-test-123',
        token_count: 0,
        context_window_size: 0,
        session_name: null,
        created_at: '',
        updated_at: '',
        deleted_at: null,
        user_id: 'test',
        metadata: {},
        messages: []
      };
      
      // Call the resume callback
      await act(async () => {
        await resumeCallback(testSession);
      });
      
      expect(mockSelectSession).toHaveBeenCalledWith('resume-test-123');
    });

    it('should handle errors in resume callback', async () => {
      renderHook(() => useTestSession());
      
      const resumeCallback = (testSessionManager.setResumeCallback as any).mock.calls[0][0];
      
      mockSelectSession.mockRejectedValue(new Error('Session not found'));
      
      const testSession: TestSession = {
        version: 1,
        session_id: 'error-test',
        token_count: 0,
        context_window_size: 0,
        session_name: null,
        created_at: '',
        updated_at: '',
        deleted_at: null,
        user_id: 'test',
        metadata: {},
        messages: []
      };
      
      // Should throw the error
      await expect(resumeCallback(testSession)).rejects.toThrow('Session not found');
    });
  });

  describe('Auto Load on Mount', () => {
    it('should auto-load scenario on mount when configured', async () => {
      const mockSession: TestSession = {
        version: 1,
        session_id: 'auto-load',
        token_count: 0,
        context_window_size: 0,
        session_name: null,
        created_at: '',
        updated_at: '',
        deleted_at: null,
        user_id: 'test',
        metadata: {},
        messages: []
      };
      
      (testSessionManager.loadScenario as any).mockResolvedValue(mockSession);
      (testSessionManager.getConfig as any).mockReturnValue({
        enabled: true,
        scenarios: [],
        showTestControls: true,
        autoLoadOnMount: true,
        currentScenarioId: 'auto-scenario'
      });
      
      const { result } = renderHook(() => useTestSession());
      
      await waitFor(() => {
        expect(testSessionManager.loadScenario).toHaveBeenCalledWith('auto-scenario');
      });
    });

    it('should not auto-load when not configured', () => {
      (testSessionManager.getConfig as any).mockReturnValue({
        enabled: true,
        scenarios: [],
        showTestControls: true,
        autoLoadOnMount: false,
        currentScenarioId: 'test-scenario'
      });
      
      renderHook(() => useTestSession());
      
      expect(testSessionManager.loadScenario).not.toHaveBeenCalled();
    });

    it('should not auto-load when already has current scenario', () => {
      (testSessionManager.getConfig as any).mockReturnValue({
        enabled: true,
        scenarios: [],
        showTestControls: true,
        autoLoadOnMount: true,
        currentScenarioId: 'test-scenario'
      });
      (testSessionManager.getCurrentScenario as any).mockReturnValue({
        id: 'existing',
        name: 'Existing Scenario',
        filePath: '/existing.json'
      });
      
      renderHook(() => useTestSession());
      
      expect(testSessionManager.loadScenario).not.toHaveBeenCalled();
    });
  });

  describe('State Synchronization', () => {
    it('should sync with manager state periodically', async () => {
      vi.useFakeTimers();
      
      const { result } = renderHook(() => useTestSession());
      
      // Change the manager's state
      (testSessionManager.isEnabled as any).mockReturnValue(false);
      (testSessionManager.getConfig as any).mockReturnValue({
        enabled: false,
        scenarios: [],
        showTestControls: false,
        autoLoadOnMount: false,
        currentScenarioId: null
      });
      
      // Advance time to trigger sync
      await act(async () => {
        vi.advanceTimersByTime(1000);
      });
      
      // State should be updated
      expect(testSessionManager.getConfig).toHaveBeenCalled();
      
      vi.useRealTimers();
    });
  });
});
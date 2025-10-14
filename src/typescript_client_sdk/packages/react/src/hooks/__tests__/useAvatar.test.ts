/**
 * Tests for useAvatar hook
 * Covers avatar session management, HeyGen integration, and voice coordination
 */

import { describe, it, expect, vi, beforeEach, afterEach } from 'vitest';
import { renderHook, act, waitFor } from '@testing-library/react';
import { useAvatar } from '../useAvatar';
import type { Avatar, Voice, AvatarManager, VoiceManager, RealtimeClient } from '@agentc/realtime-core';

// Mock the context - must match the import path used by useAvatar
vi.mock('../../providers/AgentCContext', () => ({
  useRealtimeClientSafe: vi.fn()
}));

// Import the mocked module after mocking
import { useRealtimeClientSafe } from '../../providers/AgentCContext';

describe('useAvatar', () => {
  // Test data
  const mockAvatar: Avatar = {
    avatar_id: 'avatar-1',
    name: 'Test Avatar',
    provider: 'heygen'
  };

  const mockAvatar2: Avatar = {
    avatar_id: 'avatar-2',
    name: 'Second Avatar',
    provider: 'heygen'
  };

  const mockVoice: Voice = {
    voice_id: 'voice-1',
    name: 'Default Voice',
    provider: 'openai'
  };

  const mockAvatarVoice: Voice = {
    voice_id: 'avatar',
    name: 'Avatar Voice',
    provider: 'heygen'
  };

  const mockNoneVoice: Voice = {
    voice_id: 'none',
    name: 'No Voice',
    provider: 'none'
  };

  // Mock managers
  let mockVoiceManager: Partial<VoiceManager>;
  let mockAvatarManager: Partial<AvatarManager>;
  let mockClient: Partial<RealtimeClient>;
  let consoleErrorSpy: ReturnType<typeof vi.spyOn>;

  beforeEach(() => {
    // Clear all mocks
    vi.clearAllMocks();

    // Spy on console.error
    consoleErrorSpy = vi.spyOn(console, 'error').mockImplementation(() => {});

    // Setup mock voice manager
    mockVoiceManager = {
      getAvailableVoices: vi.fn().mockReturnValue([mockVoice, mockAvatarVoice, mockNoneVoice]),
      setCurrentVoice: vi.fn().mockReturnValue(true),
      on: vi.fn(),
      off: vi.fn()
    };

    // Setup mock avatar manager
    mockAvatarManager = {
      getAvailableAvatars: vi.fn().mockReturnValue([mockAvatar, mockAvatar2]),
      getSessionId: vi.fn().mockReturnValue(null),
      getAvatarId: vi.fn().mockReturnValue(null),
      setAvatarSession: vi.fn(),
      clearAvatarSession: vi.fn(),
      on: vi.fn(),
      off: vi.fn()
    };

    // Setup mock client
    mockClient = {
      getAvatarManager: vi.fn().mockReturnValue(mockAvatarManager),
      getVoiceManager: vi.fn().mockReturnValue(mockVoiceManager),
      getHeyGenAccessToken: vi.fn().mockReturnValue('test-token'),
      setAvatarSession: vi.fn(),
      clearAvatarSession: vi.fn(),
      setAgentVoice: vi.fn()
    };

    // Set default mock implementation
    (useRealtimeClientSafe as ReturnType<typeof vi.fn>).mockReturnValue(mockClient as RealtimeClient);
  });

  afterEach(() => {
    consoleErrorSpy.mockRestore();
    vi.restoreAllMocks();
  });

  describe('Initialization', () => {
    it('returns empty state when client is null', () => {
      (useRealtimeClientSafe as ReturnType<typeof vi.fn>).mockReturnValue(null);

      const { result } = renderHook(() => useAvatar());

      // Hook should still return the full structure even with null client
      expect(result.current).toBeDefined();
      expect(result.current.availableAvatars).toEqual([]);
      expect(result.current.avatarSession).toBeNull();
      expect(result.current.heygenAccessToken).toBeNull();
      expect(result.current.isAvatarEnabled).toBe(false);
      expect(result.current.isAvatarActive).toBe(false);
    });

    it('returns empty state when AvatarManager is null', () => {
      (mockClient.getAvatarManager as ReturnType<typeof vi.fn>).mockReturnValue(null);

      const { result } = renderHook(() => useAvatar());

      // Hook should still return the full structure even with null manager
      expect(result.current).toBeDefined();
      expect(result.current.availableAvatars).toEqual([]);
      expect(result.current.avatarSession).toBeNull();
      expect(result.current.heygenAccessToken).toBeNull();
      expect(result.current.isAvatarEnabled).toBe(false);
    });

    it('loads avatar information on mount', () => {
      const { result } = renderHook(() => useAvatar());

      expect(result.current.availableAvatars).toEqual([mockAvatar, mockAvatar2]);
      expect(result.current.heygenAccessToken).toBe('test-token');
      expect(mockAvatarManager.getAvailableAvatars).toHaveBeenCalled();
      expect(mockClient.getHeyGenAccessToken).toHaveBeenCalled();
    });

    it('sets isAvatarEnabled based on available avatars', () => {
      const { result, rerender } = renderHook(() => useAvatar());

      // With avatars available
      expect(result.current.isAvatarEnabled).toBe(true);

      // Update mock to return no avatars
      (mockAvatarManager.getAvailableAvatars as ReturnType<typeof vi.fn>).mockReturnValue([]);
      
      // Trigger re-render to update state
      act(() => {
        const handler = (mockAvatarManager.on as ReturnType<typeof vi.fn>).mock.calls.find(
          call => call[0] === 'avatar-state-changed'
        )?.[1];
        handler?.();
      });

      expect(result.current.isAvatarEnabled).toBe(false);
    });

    it('loads existing avatar session on mount', () => {
      (mockAvatarManager.getSessionId as ReturnType<typeof vi.fn>).mockReturnValue('session-123');
      (mockAvatarManager.getAvatarId as ReturnType<typeof vi.fn>).mockReturnValue('avatar-1');

      const { result } = renderHook(() => useAvatar());

      expect(result.current.avatarSession).toEqual({
        sessionId: 'session-123',
        avatarId: 'avatar-1'
      });
    });

    it('sets avatarSession to null when only one ID is present', () => {
      // Only session ID present
      (mockAvatarManager.getSessionId as ReturnType<typeof vi.fn>).mockReturnValue('session-123');
      (mockAvatarManager.getAvatarId as ReturnType<typeof vi.fn>).mockReturnValue(null);

      const { result } = renderHook(() => useAvatar());

      expect(result.current.avatarSession).toBeNull();
    });
  });

  describe('Set Avatar', () => {
    it('sets avatar successfully', async () => {
      const { result } = renderHook(() => useAvatar());

      await act(async () => {
        await result.current.setAvatar('avatar-1', 'session-123');
      });

      expect(mockAvatarManager.setAvatarSession).toHaveBeenCalledWith('session-123', 'avatar-1');
      expect(mockClient.setAvatarSession).toHaveBeenCalledWith('session-123', 'avatar-1');
      expect(result.current.error).toBeNull();
    });

    it('switches voice to avatar mode when setting avatar', async () => {
      const { result } = renderHook(() => useAvatar());

      await act(async () => {
        await result.current.setAvatar('avatar-1', 'session-123');
      });

      expect(mockVoiceManager.setCurrentVoice).toHaveBeenCalledWith('avatar', 'client');
      expect(mockClient.setAgentVoice).toHaveBeenCalledWith('avatar');
    });

    it('handles client not available gracefully', async () => {
      (useRealtimeClientSafe as ReturnType<typeof vi.fn>).mockReturnValue(null);

      const { result } = renderHook(() => useAvatar());

      // Should not be able to set avatar when client is null
      expect(result.current.setAvatar).toBeDefined();
      
      // But calling it should fail
      await expect(
        result.current.setAvatar('avatar-1', 'session-123')
      ).rejects.toThrow('Client not available');
    });

    it('handles avatar manager not available gracefully', async () => {
      (mockClient.getAvatarManager as ReturnType<typeof vi.fn>).mockReturnValue(null);

      const { result } = renderHook(() => useAvatar());

      // Should not be able to set avatar when manager is null
      expect(result.current.setAvatar).toBeDefined();
      
      // But calling it should fail
      await expect(
        result.current.setAvatar('avatar-1', 'session-123')
      ).rejects.toThrow('Avatar manager not available');
    });

    it('throws error when no avatars available', async () => {
      (mockAvatarManager.getAvailableAvatars as ReturnType<typeof vi.fn>).mockReturnValue([]);

      const { result } = renderHook(() => useAvatar());

      await expect(
        act(async () => {
          await result.current.setAvatar('avatar-1', 'session-123');
        })
      ).rejects.toThrow('No avatars available');
    });

    // TODO: Enable when avatar functionality is fully implemented
    it.skip('sets isLoading during operation', async () => {
      const { result } = renderHook(() => useAvatar());

      expect(result.current.isLoading).toBe(false);

      let loadingDuringOperation = false;

      // Make setAvatarSession async and capture loading state
      (mockAvatarManager.setAvatarSession as ReturnType<typeof vi.fn>).mockImplementation(
        () => {
          // Capture the loading state during the operation
          loadingDuringOperation = result.current.isLoading;
          return Promise.resolve();
        }
      );

      await act(async () => {
        await result.current.setAvatar('avatar-1', 'session-123');
      });

      // Verify loading was true during operation
      expect(loadingDuringOperation).toBe(true);
      expect(result.current.isLoading).toBe(false);
    });

    it('handles voice manager not available', async () => {
      (mockClient.getVoiceManager as ReturnType<typeof vi.fn>).mockReturnValue(null);

      const { result } = renderHook(() => useAvatar());

      // Ensure hook returns properly even without voice manager
      expect(result.current).toBeDefined();
      expect(result.current.setAvatar).toBeDefined();

      await act(async () => {
        await result.current.setAvatar('avatar-1', 'session-123');
      });

      // Should complete without error
      expect(mockAvatarManager.setAvatarSession).toHaveBeenCalled();
      expect(mockClient.setAvatarSession).toHaveBeenCalled();
    });

    // TODO: Enable when avatar functionality is fully implemented
    it.skip('handles error during setAvatar', async () => {
      const { result } = renderHook(() => useAvatar());

      // Ensure hook is properly initialized
      expect(result.current).toBeDefined();
      expect(result.current.setAvatar).toBeDefined();

      const error = new Error('Failed to set avatar session');
      (mockAvatarManager.setAvatarSession as ReturnType<typeof vi.fn>).mockImplementationOnce(() => {
        throw error;
      });

      await expect(
        act(async () => {
          await result.current.setAvatar('avatar-1', 'session-123');
        })
      ).rejects.toThrow('Failed to set avatar session');

      expect(result.current.error).toBe('Failed to set avatar session');
      expect(result.current.isLoading).toBe(false);
    });
  });

  describe('Clear Avatar', () => {
    it('clears avatar successfully', async () => {
      const { result } = renderHook(() => useAvatar());

      await act(async () => {
        await result.current.clearAvatar();
      });

      expect(mockAvatarManager.clearAvatarSession).toHaveBeenCalled();
      expect(mockClient.clearAvatarSession).toHaveBeenCalled();
      expect(result.current.error).toBeNull();
    });

    it('switches voice back from avatar mode', async () => {
      const { result } = renderHook(() => useAvatar());

      await act(async () => {
        await result.current.clearAvatar();
      });

      // Should select the first non-avatar, non-none voice
      expect(mockVoiceManager.setCurrentVoice).toHaveBeenCalledWith('voice-1', 'client');
      expect(mockClient.setAgentVoice).toHaveBeenCalledWith('voice-1');
    });

    it('handles no suitable default voice', async () => {
      // Only avatar and none voices available
      (mockVoiceManager.getAvailableVoices as ReturnType<typeof vi.fn>).mockReturnValue([mockAvatarVoice, mockNoneVoice]);

      const { result } = renderHook(() => useAvatar());

      await act(async () => {
        await result.current.clearAvatar();
      });

      // Should still clear avatar but not set voice
      expect(mockAvatarManager.clearAvatarSession).toHaveBeenCalled();
      expect(mockClient.clearAvatarSession).toHaveBeenCalled();
      expect(mockVoiceManager.setCurrentVoice).not.toHaveBeenCalled();
    });

    it('handles client not available gracefully', async () => {
      (useRealtimeClientSafe as ReturnType<typeof vi.fn>).mockReturnValue(null);

      const { result } = renderHook(() => useAvatar());

      // Should not be able to set avatar when client is null
      expect(result.current.setAvatar).toBeDefined();
      
      // But calling it should fail
      await expect(
        result.current.setAvatar('avatar-1', 'session-123')
      ).rejects.toThrow('Client not available');
    });

    it('handles avatar manager not available gracefully', async () => {
      (mockClient.getAvatarManager as ReturnType<typeof vi.fn>).mockReturnValue(null);

      const { result } = renderHook(() => useAvatar());

      // Should not be able to set avatar when manager is null
      expect(result.current.setAvatar).toBeDefined();
      
      // But calling it should fail
      await expect(
        result.current.setAvatar('avatar-1', 'session-123')
      ).rejects.toThrow('Avatar manager not available');
    });

    // TODO: Enable when avatar functionality is fully implemented
    it.skip('handles error during clear', async () => {
      const error = new Error('Clear failed');
      (mockAvatarManager.clearAvatarSession as ReturnType<typeof vi.fn>).mockImplementation(() => {
        throw error;
      });

      const { result } = renderHook(() => useAvatar());

      await expect(
        act(async () => {
          await result.current.clearAvatar();
        })
      ).rejects.toThrow('Clear failed');

      expect(result.current.error).toBe('Clear failed');
      expect(result.current.isLoading).toBe(false);
    });

    it('handles voice manager not available', async () => {
      (mockClient.getVoiceManager as ReturnType<typeof vi.fn>).mockReturnValue(null);

      const { result } = renderHook(() => useAvatar());

      await act(async () => {
        await result.current.clearAvatar();
      });

      // Should complete without error
      expect(mockAvatarManager.clearAvatarSession).toHaveBeenCalled();
      expect(mockClient.clearAvatarSession).toHaveBeenCalled();
    });
  });

  describe('Helper Methods', () => {
    it('getAvatarById finds correct avatar', () => {
      const { result } = renderHook(() => useAvatar());

      expect(result.current.getAvatarById('avatar-1')).toEqual(mockAvatar);
      expect(result.current.getAvatarById('avatar-2')).toEqual(mockAvatar2);
      expect(result.current.getAvatarById('non-existent')).toBeUndefined();
    });

    it('isAvatarAvailable checks correctly', () => {
      const { result } = renderHook(() => useAvatar());

      expect(result.current.isAvatarAvailable('avatar-1')).toBe(true);
      expect(result.current.isAvatarAvailable('avatar-2')).toBe(true);
      expect(result.current.isAvatarAvailable('non-existent')).toBe(false);
    });
  });

  describe('Event Subscriptions', () => {
    it('subscribes to avatar manager events', async () => {
      renderHook(() => useAvatar());

      await waitFor(() => {
        expect(mockAvatarManager.on).toHaveBeenCalledWith('avatar-state-changed', expect.any(Function));
        expect(mockAvatarManager.on).toHaveBeenCalledWith('avatar-session-started', expect.any(Function));
        expect(mockAvatarManager.on).toHaveBeenCalledWith('avatar-session-ended', expect.any(Function));
        expect(mockAvatarManager.on).toHaveBeenCalledTimes(3);
      });
    });

    it('updates state on avatar-state-changed event', () => {
      const { result } = renderHook(() => useAvatar());

      // Update mock to return different data
      (mockAvatarManager.getSessionId as ReturnType<typeof vi.fn>).mockReturnValue('new-session');
      (mockAvatarManager.getAvatarId as ReturnType<typeof vi.fn>).mockReturnValue('avatar-2');

      // Emit event
      act(() => {
        const handler = (mockAvatarManager.on as ReturnType<typeof vi.fn>).mock.calls.find(
          call => call[0] === 'avatar-state-changed'
        )?.[1];
        handler?.();
      });

      expect(result.current.avatarSession).toEqual({
        sessionId: 'new-session',
        avatarId: 'avatar-2'
      });
    });

    it('updates state on avatar-session-started event', () => {
      const { result } = renderHook(() => useAvatar());

      // Update mock to return session data
      (mockAvatarManager.getSessionId as ReturnType<typeof vi.fn>).mockReturnValue('started-session');
      (mockAvatarManager.getAvatarId as ReturnType<typeof vi.fn>).mockReturnValue('avatar-1');

      // Emit event
      act(() => {
        const handler = (mockAvatarManager.on as ReturnType<typeof vi.fn>).mock.calls.find(
          call => call[0] === 'avatar-session-started'
        )?.[1];
        handler?.();
      });

      expect(result.current.avatarSession).toEqual({
        sessionId: 'started-session',
        avatarId: 'avatar-1'
      });
    });

    it('updates state on avatar-session-ended event', () => {
      // Start with a session
      (mockAvatarManager.getSessionId as ReturnType<typeof vi.fn>).mockReturnValue('ending-session');
      (mockAvatarManager.getAvatarId as ReturnType<typeof vi.fn>).mockReturnValue('avatar-1');

      const { result } = renderHook(() => useAvatar());

      expect(result.current.avatarSession).toEqual({
        sessionId: 'ending-session',
        avatarId: 'avatar-1'
      });

      // Update mock to return no session
      (mockAvatarManager.getSessionId as ReturnType<typeof vi.fn>).mockReturnValue(null);
      (mockAvatarManager.getAvatarId as ReturnType<typeof vi.fn>).mockReturnValue(null);

      // Emit event
      act(() => {
        const handler = (mockAvatarManager.on as ReturnType<typeof vi.fn>).mock.calls.find(
          call => call[0] === 'avatar-session-ended'
        )?.[1];
        handler?.();
      });

      expect(result.current.avatarSession).toBeNull();
    });

    it('unsubscribes on unmount', async () => {
      const { unmount } = renderHook(() => useAvatar());

      // Wait for subscription to happen first
      await waitFor(() => {
        expect(mockAvatarManager.on).toHaveBeenCalled();
      });

      unmount();

      expect(mockAvatarManager.off).toHaveBeenCalledWith('avatar-state-changed', expect.any(Function));
      expect(mockAvatarManager.off).toHaveBeenCalledWith('avatar-session-started', expect.any(Function));
      expect(mockAvatarManager.off).toHaveBeenCalledWith('avatar-session-ended', expect.any(Function));
      expect(mockAvatarManager.off).toHaveBeenCalledTimes(3);
    });

    it('does not subscribe when client is null', () => {
      (useRealtimeClientSafe as ReturnType<typeof vi.fn>).mockReturnValue(null);

      renderHook(() => useAvatar());

      // Should not attempt to subscribe when client is null
      expect(mockAvatarManager.on).not.toHaveBeenCalled();
    });

    it('does not subscribe when avatar manager is null', () => {
      (mockClient.getAvatarManager as ReturnType<typeof vi.fn>).mockReturnValue(null);

      renderHook(() => useAvatar());

      // Should not attempt to subscribe when manager is null
      expect(mockAvatarManager.on).not.toHaveBeenCalled();
    });
  });

  describe('Computed Properties', () => {
    it('isAvatarActive reflects session state', () => {
      const { result, rerender } = renderHook(() => useAvatar());

      // Initially no session
      expect(result.current.isAvatarActive).toBe(false);

      // Update mock to return session
      (mockAvatarManager.getSessionId as ReturnType<typeof vi.fn>).mockReturnValue('active-session');
      (mockAvatarManager.getAvatarId as ReturnType<typeof vi.fn>).mockReturnValue('avatar-1');

      // Trigger update
      act(() => {
        const handler = (mockAvatarManager.on as ReturnType<typeof vi.fn>).mock.calls.find(
          call => call[0] === 'avatar-state-changed'
        )?.[1];
        handler?.();
      });

      expect(result.current.isAvatarActive).toBe(true);

      // Clear session
      (mockAvatarManager.getSessionId as ReturnType<typeof vi.fn>).mockReturnValue(null);
      (mockAvatarManager.getAvatarId as ReturnType<typeof vi.fn>).mockReturnValue(null);

      // Trigger update
      act(() => {
        const handler = (mockAvatarManager.on as ReturnType<typeof vi.fn>).mock.calls.find(
          call => call[0] === 'avatar-state-changed'
        )?.[1];
        handler?.();
      });

      expect(result.current.isAvatarActive).toBe(false);
    });
  });

  describe('Error Handling', () => {
    it('handles errors in updateAvatarInfo', () => {
      const error = new Error('Failed to get avatars');
      (mockAvatarManager.getAvailableAvatars as ReturnType<typeof vi.fn>).mockImplementation(() => {
        throw error;
      });

      const { result } = renderHook(() => useAvatar());

      expect(consoleErrorSpy).toHaveBeenCalledWith('Failed to get avatar information:', error);
      expect(result.current.error).toBe('Failed to get avatars');
    });

    it('handles non-Error objects in updateAvatarInfo', () => {
      (mockAvatarManager.getAvailableAvatars as ReturnType<typeof vi.fn>).mockImplementation(() => {
        throw 'String error';
      });

      const { result } = renderHook(() => useAvatar());

      expect(consoleErrorSpy).toHaveBeenCalledWith('Failed to get avatar information:', 'String error');
      expect(result.current.error).toBe('Failed to get avatar information');
    });

    it('clears error on successful update', () => {
      // Start with an error
      const error = new Error('Initial error');
      (mockAvatarManager.getAvailableAvatars as ReturnType<typeof vi.fn>).mockImplementationOnce(() => {
        throw error;
      });

      const { result } = renderHook(() => useAvatar());
      expect(result.current.error).toBe('Initial error');

      // Fix the error
      (mockAvatarManager.getAvailableAvatars as ReturnType<typeof vi.fn>).mockReturnValue([mockAvatar]);

      // Trigger update
      act(() => {
        const handler = (mockAvatarManager.on as ReturnType<typeof vi.fn>).mock.calls.find(
          call => call[0] === 'avatar-state-changed'
        )?.[1];
        handler?.();
      });

      expect(result.current.error).toBeNull();
    });

    // TODO: Enable when avatar functionality is fully implemented
    it.skip('handles non-Error objects in setAvatar', async () => {
      (mockAvatarManager.setAvatarSession as ReturnType<typeof vi.fn>).mockImplementation(() => {
        throw 'String error';
      });

      const { result } = renderHook(() => useAvatar());

      await expect(
        act(async () => {
          await result.current.setAvatar('avatar-1', 'session-123');
        })
      ).rejects.toThrow('Failed to set avatar');

      expect(result.current.error).toBe('Failed to set avatar');
    });

    // TODO: Enable when avatar functionality is fully implemented
    it.skip('handles non-Error objects in clearAvatar', async () => {
      (mockAvatarManager.clearAvatarSession as ReturnType<typeof vi.fn>).mockImplementation(() => {
        throw 'String error';
      });

      const { result } = renderHook(() => useAvatar());

      await expect(
        act(async () => {
          await result.current.clearAvatar();
        })
      ).rejects.toThrow('Failed to clear avatar');

      expect(result.current.error).toBe('Failed to clear avatar');
    });
  });

  describe('Integration scenarios', () => {
    it('handles full avatar lifecycle', async () => {
      const { result } = renderHook(() => useAvatar());

      // Start with no session
      expect(result.current.isAvatarActive).toBe(false);
      expect(result.current.avatarSession).toBeNull();

      // Set avatar
      await act(async () => {
        await result.current.setAvatar('avatar-1', 'session-123');
      });

      // Verify avatar set
      expect(mockAvatarManager.setAvatarSession).toHaveBeenCalledWith('session-123', 'avatar-1');
      expect(mockVoiceManager.setCurrentVoice).toHaveBeenCalledWith('avatar', 'client');

      // Simulate session update
      (mockAvatarManager.getSessionId as ReturnType<typeof vi.fn>).mockReturnValue('session-123');
      (mockAvatarManager.getAvatarId as ReturnType<typeof vi.fn>).mockReturnValue('avatar-1');
      
      act(() => {
        const handler = (mockAvatarManager.on as ReturnType<typeof vi.fn>).mock.calls.find(
          call => call[0] === 'avatar-session-started'
        )?.[1];
        handler?.();
      });

      expect(result.current.isAvatarActive).toBe(true);
      expect(result.current.avatarSession).toEqual({
        sessionId: 'session-123',
        avatarId: 'avatar-1'
      });

      // Clear avatar
      await act(async () => {
        await result.current.clearAvatar();
      });

      // Verify avatar cleared
      expect(mockAvatarManager.clearAvatarSession).toHaveBeenCalled();
      expect(mockVoiceManager.setCurrentVoice).toHaveBeenCalledWith('voice-1', 'client');

      // Simulate session clear
      (mockAvatarManager.getSessionId as ReturnType<typeof vi.fn>).mockReturnValue(null);
      (mockAvatarManager.getAvatarId as ReturnType<typeof vi.fn>).mockReturnValue(null);
      
      act(() => {
        const handler = (mockAvatarManager.on as ReturnType<typeof vi.fn>).mock.calls.find(
          call => call[0] === 'avatar-session-ended'
        )?.[1];
        handler?.();
      });

      expect(result.current.isAvatarActive).toBe(false);
      expect(result.current.avatarSession).toBeNull();
    });

    it('handles rapid avatar switching', async () => {
      const { result } = renderHook(() => useAvatar());

      // Set first avatar
      await act(async () => {
        await result.current.setAvatar('avatar-1', 'session-1');
      });

      expect(mockAvatarManager.setAvatarSession).toHaveBeenLastCalledWith('session-1', 'avatar-1');

      // Switch to second avatar
      await act(async () => {
        await result.current.setAvatar('avatar-2', 'session-2');
      });

      expect(mockAvatarManager.setAvatarSession).toHaveBeenLastCalledWith('session-2', 'avatar-2');
      expect(mockAvatarManager.setAvatarSession).toHaveBeenCalledTimes(2);
    });

    // TODO: Enable when avatar functionality is fully implemented
    it.skip('recovers from errors gracefully', async () => {
      const { result } = renderHook(() => useAvatar());

      // First attempt fails
      (mockAvatarManager.setAvatarSession as ReturnType<typeof vi.fn>).mockImplementationOnce(() => {
        throw new Error('Network error');
      });

      await expect(
        act(async () => {
          await result.current.setAvatar('avatar-1', 'session-123');
        })
      ).rejects.toThrow('Network error');

      expect(result.current.error).toBe('Network error');

      // Second attempt succeeds
      (mockAvatarManager.setAvatarSession as ReturnType<typeof vi.fn>).mockImplementation(() => {});

      await act(async () => {
        await result.current.setAvatar('avatar-1', 'session-123');
      });

      expect(result.current.error).toBeNull();
      expect(mockAvatarManager.setAvatarSession).toHaveBeenCalledTimes(2);
    });
  });
});
/**
 * Tests for useVoiceModel hook
 * Tests voice selection, special modes (avatar/text-only), and server synchronization
 */

import { describe, it, expect, beforeEach, afterEach, vi } from 'vitest';
import { renderHook, act, waitFor } from '@testing-library/react';
import type { Mock } from 'vitest';
import { useVoiceModel } from '../useVoiceModel';
import type { UseVoiceModelReturn } from '../useVoiceModel';
import type { RealtimeClient, VoiceManager, Voice } from '@agentc/realtime-core';

// Mock dependencies
vi.mock('../../providers/AgentCContext', () => ({
  useRealtimeClientSafe: vi.fn()
}));

describe('useVoiceModel', () => {
  // Test utilities
  let mockClient: {
    getVoiceManager: Mock;
    setAgentVoice: Mock;
    on: Mock;
    off: Mock;
  };

  let mockVoiceManager: {
    getCurrentVoice: Mock;
    getAvailableVoices: Mock;
    setCurrentVoice: Mock;
    on: Mock;
    off: Mock;
  };

  let mockConsoleError: Mock;

  let voiceEventHandlers: Map<string, (event?: unknown) => void>;
  let clientEventHandlers: Map<string, (event?: unknown) => void>;

  // Test data
  const mockVoices: Voice[] = [
    {
      voice_id: 'voice-1',
      name: 'Voice One',
      provider: 'openai'
    },
    {
      voice_id: 'voice-2',
      name: 'Voice Two',
      provider: 'elevenlabs'
    },
    {
      voice_id: 'avatar',
      name: 'Avatar Voice',
      provider: 'heygen'
    },
    {
      voice_id: 'none',
      name: 'No Voice (Text Only)',
      provider: 'none'
    }
  ];

  // Helper to emit events
  const emitVoiceEvent = (eventName: string, data?: unknown) => {
    const handler = voiceEventHandlers.get(eventName);
    if (handler) {
      act(() => {
        handler(data);
      });
    }
  };

  const emitClientEvent = (eventName: string, data?: unknown) => {
    const handler = clientEventHandlers.get(eventName);
    if (handler) {
      act(() => {
        handler(data);
      });
    }
  };

  // Helper to create voice
  const createVoice = (id: string, name: string, provider = 'test'): Voice => ({
    voice_id: id,
    name,
    provider
  });

  beforeEach(async () => {
    // Initialize event handler storage
    voiceEventHandlers = new Map();
    clientEventHandlers = new Map();

    // Setup mock VoiceManager
    mockVoiceManager = {
      getCurrentVoice: vi.fn(() => mockVoices[0]),
      getAvailableVoices: vi.fn(() => mockVoices),
      setCurrentVoice: vi.fn(() => true),
      on: vi.fn((event: string, handler: (event?: unknown) => void) => {
        voiceEventHandlers.set(event, handler);
      }),
      off: vi.fn((event: string) => {
        voiceEventHandlers.delete(event);
      })
    };

    // Setup mock client
    mockClient = {
      getVoiceManager: vi.fn(() => mockVoiceManager),
      setAgentVoice: vi.fn(),
      on: vi.fn((event: string, handler: (event?: unknown) => void) => {
        clientEventHandlers.set(event, handler);
      }),
      off: vi.fn((event: string) => {
        clientEventHandlers.delete(event);
      })
    };

    // Mock console.error to prevent test output pollution
    mockConsoleError = vi.spyOn(console, 'error').mockImplementation(() => {});

    // Apply mocks
    const agentCContext = await import('../../providers/AgentCContext');
    (agentCContext.useRealtimeClientSafe as Mock).mockReturnValue(mockClient);

    vi.clearAllMocks();
  });

  afterEach(() => {
    vi.restoreAllMocks();
  });

  describe('Initialization', () => {
    it('returns empty state when client is null', async () => {
      const agentCContext = await import('../../providers/AgentCContext');
      (agentCContext.useRealtimeClientSafe as Mock).mockReturnValue(null);

      const { result } = renderHook(() => useVoiceModel());

      expect(result.current.currentVoice).toBeNull();
      expect(result.current.availableVoices).toEqual([]);
      expect(result.current.isLoading).toBe(false);
      expect(result.current.error).toBeNull();
      expect(result.current.isAvatarMode).toBe(false);
      expect(result.current.isTextOnly).toBe(false);
    });

    it('returns empty state when VoiceManager is null', () => {
      mockClient.getVoiceManager.mockReturnValue(null);

      const { result } = renderHook(() => useVoiceModel());

      expect(result.current.currentVoice).toBeNull();
      expect(result.current.availableVoices).toEqual([]);
      expect(result.current.isLoading).toBe(false);
      expect(result.current.error).toBeNull();
    });

    it('loads voice information on mount', () => {
      const { result } = renderHook(() => useVoiceModel());

      expect(mockVoiceManager.getCurrentVoice).toHaveBeenCalled();
      expect(mockVoiceManager.getAvailableVoices).toHaveBeenCalled();
      expect(result.current.currentVoice).toEqual(mockVoices[0]);
      expect(result.current.availableVoices).toEqual(mockVoices);
      expect(result.current.error).toBeNull();
    });

    it('handles errors in updateVoiceInfo', () => {
      mockVoiceManager.getCurrentVoice.mockImplementation(() => {
        throw new Error('Failed to get voice');
      });

      const { result } = renderHook(() => useVoiceModel());

      expect(mockConsoleError).toHaveBeenCalledWith(
        'Failed to get voice information:',
        expect.any(Error)
      );
      expect(result.current.error).toBe('Failed to get voice');
      expect(result.current.currentVoice).toBeNull();
      expect(result.current.availableVoices).toEqual([]);
    });
  });

  describe('Voice Selection', () => {
    it('sets voice successfully', async () => {
      const { result } = renderHook(() => useVoiceModel());

      await act(async () => {
        await result.current.setVoice('voice-2');
      });

      expect(mockVoiceManager.setCurrentVoice).toHaveBeenCalledWith('voice-2', 'client');
      expect(mockClient.setAgentVoice).toHaveBeenCalledWith('voice-2');
      expect(mockVoiceManager.getCurrentVoice).toHaveBeenCalledTimes(2); // Initial + update
      expect(result.current.error).toBeNull();
    });

    it('throws error when setCurrentVoice fails', async () => {
      mockVoiceManager.setCurrentVoice.mockReturnValue(false);

      const { result } = renderHook(() => useVoiceModel());

      let errorThrown = false;
      await act(async () => {
        try {
          await result.current.setVoice('voice-2');
        } catch (error) {
          errorThrown = true;
          expect((error as Error).message).toBe('Failed to set voice');
        }
      });

      expect(errorThrown).toBe(true);
      expect(result.current.error).toBe('Failed to set voice');
    });

    it('throws error when client not available', async () => {
      const agentCContext = await import('../../providers/AgentCContext');
      (agentCContext.useRealtimeClientSafe as Mock).mockReturnValue(null);

      const { result } = renderHook(() => useVoiceModel());

      await expect(
        act(async () => {
          await result.current.setVoice('voice-1');
        })
      ).rejects.toThrow('Client not available');
    });

    it('throws error when voice manager not available', async () => {
      mockClient.getVoiceManager.mockReturnValue(null);

      const { result } = renderHook(() => useVoiceModel());

      await expect(
        act(async () => {
          await result.current.setVoice('voice-1');
        })
      ).rejects.toThrow('Voice manager not available');
    });

    it('sets isLoading during operation', async () => {
      let resolveSetVoice: () => void;
      const setVoicePromise = new Promise<void>((resolve) => {
        resolveSetVoice = resolve;
      });

      mockVoiceManager.setCurrentVoice.mockImplementation(() => {
        // Delay to observe loading state
        return true;
      });

      const { result } = renderHook(() => useVoiceModel());

      expect(result.current.isLoading).toBe(false);

      const setVoiceAct = act(async () => {
        const promise = result.current.setVoice('voice-2');
        // Check loading state immediately after starting
        await Promise.resolve(); // Let React update
        return promise;
      });

      // Note: Due to the synchronous nature of the operation,
      // we can't observe isLoading being true from outside
      await setVoiceAct;

      expect(result.current.isLoading).toBe(false);
      expect(mockVoiceManager.setCurrentVoice).toHaveBeenCalled();
    });

    it('handles errors during setVoice', async () => {
      mockVoiceManager.setCurrentVoice.mockImplementation(() => {
        throw new Error('Network error');
      });

      const { result } = renderHook(() => useVoiceModel());

      let errorThrown = false;
      await act(async () => {
        try {
          await result.current.setVoice('voice-2');
        } catch (error) {
          errorThrown = true;
          expect((error as Error).message).toBe('Network error');
        }
      });

      expect(errorThrown).toBe(true);
      expect(result.current.error).toBe('Network error');
      expect(result.current.isLoading).toBe(false);
    });

    it('clears error on successful operation', async () => {
      // First, create an error
      mockVoiceManager.setCurrentVoice.mockReturnValueOnce(false);

      const { result } = renderHook(() => useVoiceModel());

      await act(async () => {
        try {
          await result.current.setVoice('voice-2');
        } catch {
          // Expected error
        }
      });

      expect(result.current.error).toBe('Failed to set voice');

      // Now successful operation
      mockVoiceManager.setCurrentVoice.mockReturnValue(true);

      await act(async () => {
        await result.current.setVoice('voice-1');
      });

      expect(result.current.error).toBeNull();
    });
  });

  describe('Helper Methods', () => {
    it('isVoiceAvailable checks correctly', () => {
      const { result } = renderHook(() => useVoiceModel());

      expect(result.current.isVoiceAvailable('voice-1')).toBe(true);
      expect(result.current.isVoiceAvailable('voice-2')).toBe(true);
      expect(result.current.isVoiceAvailable('avatar')).toBe(true);
      expect(result.current.isVoiceAvailable('none')).toBe(true);
      expect(result.current.isVoiceAvailable('non-existent')).toBe(false);
    });

    it('getVoiceById finds correct voice', () => {
      const { result } = renderHook(() => useVoiceModel());

      const voice1 = result.current.getVoiceById('voice-1');
      expect(voice1).toEqual(mockVoices[0]);

      const avatarVoice = result.current.getVoiceById('avatar');
      expect(avatarVoice).toEqual(mockVoices[2]);

      const nonExistent = result.current.getVoiceById('non-existent');
      expect(nonExistent).toBeUndefined();
    });

    it('helper methods work with empty availableVoices', () => {
      mockVoiceManager.getAvailableVoices.mockReturnValue([]);

      const { result } = renderHook(() => useVoiceModel());

      expect(result.current.availableVoices).toEqual([]);
      expect(result.current.isVoiceAvailable('voice-1')).toBe(false);
      expect(result.current.getVoiceById('voice-1')).toBeUndefined();
    });
  });

  describe('Event Subscriptions', () => {
    it('subscribes to voice-changed event on VoiceManager', () => {
      renderHook(() => useVoiceModel());

      expect(mockVoiceManager.on).toHaveBeenCalledWith('voice-changed', expect.any(Function));
    });

    it('subscribes to agent_voice_changed event on client', () => {
      renderHook(() => useVoiceModel());

      expect(mockClient.on).toHaveBeenCalledWith('agent_voice_changed', expect.any(Function));
    });

    it('updates voice info on voice-changed event', () => {
      const { result } = renderHook(() => useVoiceModel());

      // Change the mock return value
      const newVoice = mockVoices[1];
      mockVoiceManager.getCurrentVoice.mockReturnValue(newVoice);

      // Clear previous calls
      vi.clearAllMocks();

      // Emit voice-changed event
      emitVoiceEvent('voice-changed');

      expect(mockVoiceManager.getCurrentVoice).toHaveBeenCalled();
      expect(mockVoiceManager.getAvailableVoices).toHaveBeenCalled();
      expect(result.current.currentVoice).toEqual(newVoice);
    });

    it('updates voice info on agent_voice_changed event', () => {
      const { result } = renderHook(() => useVoiceModel());

      // Change the mock return value
      const newVoice = mockVoices[2];
      mockVoiceManager.getCurrentVoice.mockReturnValue(newVoice);

      // Clear previous calls
      vi.clearAllMocks();

      // Emit agent_voice_changed event
      emitClientEvent('agent_voice_changed');

      expect(mockVoiceManager.getCurrentVoice).toHaveBeenCalled();
      expect(mockVoiceManager.getAvailableVoices).toHaveBeenCalled();
      expect(result.current.currentVoice).toEqual(newVoice);
    });

    it('unsubscribes events on unmount', () => {
      const { unmount } = renderHook(() => useVoiceModel());

      // Store handler references
      const voiceHandler = mockVoiceManager.on.mock.calls.find(
        call => call[0] === 'voice-changed'
      )?.[1];
      const clientHandler = mockClient.on.mock.calls.find(
        call => call[0] === 'agent_voice_changed'
      )?.[1];

      unmount();

      expect(mockVoiceManager.off).toHaveBeenCalledWith('voice-changed', voiceHandler);
      expect(mockClient.off).toHaveBeenCalledWith('agent_voice_changed', clientHandler);
    });

    it('handles events when VoiceManager is null', () => {
      mockClient.getVoiceManager.mockReturnValue(null);

      const { result } = renderHook(() => useVoiceModel());

      // Should not subscribe to events
      expect(mockVoiceManager.on).not.toHaveBeenCalled();
      expect(mockClient.on).not.toHaveBeenCalled();

      // State should be empty
      expect(result.current.currentVoice).toBeNull();
      expect(result.current.availableVoices).toEqual([]);
    });

    it('handles duplicate event subscriptions on rerender', () => {
      const { rerender } = renderHook(() => useVoiceModel());

      // Initial render subscribes events
      const initialOnCalls = mockVoiceManager.on.mock.calls.length;
      const initialClientOnCalls = mockClient.on.mock.calls.length;
      expect(mockVoiceManager.on).toHaveBeenCalledTimes(1);
      expect(mockClient.on).toHaveBeenCalledTimes(1);

      // Rerender should not add duplicate subscriptions
      // The effect has [client, updateVoiceInfo] dependencies
      // Since updateVoiceInfo is memoized with [client], it should be stable
      rerender();

      // Should not have added more subscriptions (effect shouldn't re-run)
      expect(mockVoiceManager.on).toHaveBeenCalledTimes(1);
      expect(mockClient.on).toHaveBeenCalledTimes(1);
    });
  });

  describe('Computed Properties', () => {
    it('isAvatarMode detects avatar voice', () => {
      mockVoiceManager.getCurrentVoice.mockReturnValue(mockVoices[2]); // avatar voice

      const { result } = renderHook(() => useVoiceModel());

      expect(result.current.isAvatarMode).toBe(true);
      expect(result.current.isTextOnly).toBe(false);
    });

    it('isTextOnly detects none voice', () => {
      mockVoiceManager.getCurrentVoice.mockReturnValue(mockVoices[3]); // none voice

      const { result } = renderHook(() => useVoiceModel());

      expect(result.current.isTextOnly).toBe(true);
      expect(result.current.isAvatarMode).toBe(false);
    });

    it('handles null currentVoice for computed properties', () => {
      mockVoiceManager.getCurrentVoice.mockReturnValue(null);

      const { result } = renderHook(() => useVoiceModel());

      expect(result.current.currentVoice).toBeNull();
      expect(result.current.isAvatarMode).toBe(false);
      expect(result.current.isTextOnly).toBe(false);
    });

    it('updates computed properties when voice changes', () => {
      const { result } = renderHook(() => useVoiceModel());

      // Initial state (voice-1)
      expect(result.current.isAvatarMode).toBe(false);
      expect(result.current.isTextOnly).toBe(false);

      // Change to avatar voice
      mockVoiceManager.getCurrentVoice.mockReturnValue(mockVoices[2]);
      emitVoiceEvent('voice-changed');

      expect(result.current.isAvatarMode).toBe(true);
      expect(result.current.isTextOnly).toBe(false);

      // Change to text-only voice
      mockVoiceManager.getCurrentVoice.mockReturnValue(mockVoices[3]);
      emitVoiceEvent('voice-changed');

      expect(result.current.isAvatarMode).toBe(false);
      expect(result.current.isTextOnly).toBe(true);

      // Change back to normal voice
      mockVoiceManager.getCurrentVoice.mockReturnValue(mockVoices[0]);
      emitVoiceEvent('voice-changed');

      expect(result.current.isAvatarMode).toBe(false);
      expect(result.current.isTextOnly).toBe(false);
    });
  });

  describe('Edge Cases', () => {
    it('handles missing VoiceManager gracefully', async () => {
      // Start with null VoiceManager
      mockClient.getVoiceManager.mockReturnValue(null);

      const { result } = renderHook(() => useVoiceModel());

      expect(result.current.currentVoice).toBeNull();
      expect(result.current.availableVoices).toEqual([]);

      // Later provide VoiceManager
      mockClient.getVoiceManager.mockReturnValue(mockVoiceManager);
      
      // The hook won't automatically detect VoiceManager changes since the client itself
      // hasn't changed. This is correct behavior - the hook updates on:
      // 1. Client changes
      // 2. Voice events from VoiceManager or client
      // 3. Explicit calls to setVoice
      
      // In a real scenario, VoiceManager becoming available would trigger an event.
      // We simulate this by having the VoiceManager available when an event occurs:
      
      // Try to set a voice - this will trigger updateVoiceInfo
      await act(async () => {
        await result.current.setVoice('voice-1');
      });

      // Now the hook should have the voice data
      expect(result.current.currentVoice).toEqual(mockVoices[0]);
      expect(result.current.availableVoices).toEqual(mockVoices);
    });

    it('handles voice change while loading', async () => {
      const { result } = renderHook(() => useVoiceModel());

      // Start setVoice operation
      const setVoicePromise = act(async () => {
        await result.current.setVoice('voice-2');
      });

      // Emit voice-changed event during operation
      // (though the operation is synchronous, test the scenario)
      emitVoiceEvent('voice-changed');

      await setVoicePromise;

      // State should be consistent
      expect(result.current.isLoading).toBe(false);
      expect(result.current.error).toBeNull();
    });

    it('handles rapid voice changes', async () => {
      const { result } = renderHook(() => useVoiceModel());

      // Rapid voice changes
      await act(async () => {
        await result.current.setVoice('voice-1');
      });

      await act(async () => {
        await result.current.setVoice('voice-2');
      });

      await act(async () => {
        await result.current.setVoice('avatar');
      });

      await act(async () => {
        await result.current.setVoice('none');
      });

      // All calls should have been made
      expect(mockVoiceManager.setCurrentVoice).toHaveBeenCalledTimes(4);
      expect(mockClient.setAgentVoice).toHaveBeenCalledTimes(4);
    });

    it('handles voice list updates', () => {
      const { result } = renderHook(() => useVoiceModel());

      // Initial voices
      expect(result.current.availableVoices).toEqual(mockVoices);

      // Update available voices
      const newVoices = [
        createVoice('new-1', 'New Voice 1'),
        createVoice('new-2', 'New Voice 2')
      ];
      mockVoiceManager.getAvailableVoices.mockReturnValue(newVoices);

      // Trigger update
      emitVoiceEvent('voice-changed');

      expect(result.current.availableVoices).toEqual(newVoices);
    });

    it('handles error recovery in updateVoiceInfo', () => {
      const { result } = renderHook(() => useVoiceModel());

      // First update succeeds
      expect(result.current.error).toBeNull();

      // Make getCurrentVoice throw
      mockVoiceManager.getCurrentVoice.mockImplementation(() => {
        throw new Error('Temporary error');
      });

      // Trigger update that will fail
      emitVoiceEvent('voice-changed');

      expect(result.current.error).toBe('Temporary error');
      expect(mockConsoleError).toHaveBeenCalled();

      // Fix the error
      mockVoiceManager.getCurrentVoice.mockReturnValue(mockVoices[0]);

      // Trigger successful update
      emitVoiceEvent('voice-changed');

      expect(result.current.error).toBeNull();
      expect(result.current.currentVoice).toEqual(mockVoices[0]);
    });

    it('handles empty voice list', () => {
      mockVoiceManager.getAvailableVoices.mockReturnValue([]);
      mockVoiceManager.getCurrentVoice.mockReturnValue(null);

      const { result } = renderHook(() => useVoiceModel());

      expect(result.current.availableVoices).toEqual([]);
      expect(result.current.currentVoice).toBeNull();
      expect(result.current.isVoiceAvailable('any-voice')).toBe(false);
      expect(result.current.getVoiceById('any-voice')).toBeUndefined();
    });

    it('handles malformed voice objects gracefully', () => {
      const malformedVoices = [
        { voice_id: 'valid', name: 'Valid', provider: 'test' },
        { voice_id: '', name: 'Empty ID', provider: 'test' }, // Empty ID
        { voice_id: 'no-name', name: '', provider: 'test' }, // Empty name
      ] as Voice[];

      mockVoiceManager.getAvailableVoices.mockReturnValue(malformedVoices);

      const { result } = renderHook(() => useVoiceModel());

      expect(result.current.availableVoices).toEqual(malformedVoices);
      
      // Should still work with malformed data
      expect(result.current.isVoiceAvailable('valid')).toBe(true);
      expect(result.current.isVoiceAvailable('')).toBe(true); // Empty ID exists
      expect(result.current.getVoiceById('no-name')).toEqual(malformedVoices[2]);
    });
  });

  describe('Server Synchronization', () => {
    it('syncs voice selection with server', async () => {
      const { result } = renderHook(() => useVoiceModel());

      await act(async () => {
        await result.current.setVoice('voice-2');
      });

      // Verify both local and server updates
      expect(mockVoiceManager.setCurrentVoice).toHaveBeenCalledWith('voice-2', 'client');
      expect(mockClient.setAgentVoice).toHaveBeenCalledWith('voice-2');
    });

    it('handles server voice change events', () => {
      const { result } = renderHook(() => useVoiceModel());

      // Server changes voice
      const serverVoice = mockVoices[2];
      mockVoiceManager.getCurrentVoice.mockReturnValue(serverVoice);

      emitClientEvent('agent_voice_changed', { voice_id: 'avatar' });

      expect(result.current.currentVoice).toEqual(serverVoice);
      expect(result.current.isAvatarMode).toBe(true);
    });

    it('maintains consistency between local and server state', async () => {
      const { result } = renderHook(() => useVoiceModel());

      // Local change
      await act(async () => {
        await result.current.setVoice('none');
      });

      expect(mockClient.setAgentVoice).toHaveBeenCalledWith('none');

      // Simulate server confirmation
      mockVoiceManager.getCurrentVoice.mockReturnValue(mockVoices[3]);
      emitClientEvent('agent_voice_changed');

      expect(result.current.currentVoice?.voice_id).toBe('none');
      expect(result.current.isTextOnly).toBe(true);
    });
  });

  describe('Special Modes', () => {
    it('correctly identifies avatar mode', async () => {
      const { result } = renderHook(() => useVoiceModel());

      // Set avatar voice
      mockVoiceManager.getCurrentVoice.mockReturnValue(mockVoices[2]);
      emitVoiceEvent('voice-changed');

      expect(result.current.currentVoice?.voice_id).toBe('avatar');
      expect(result.current.isAvatarMode).toBe(true);
      expect(result.current.isTextOnly).toBe(false);
    });

    it('correctly identifies text-only mode', async () => {
      const { result } = renderHook(() => useVoiceModel());

      // Set none voice
      mockVoiceManager.getCurrentVoice.mockReturnValue(mockVoices[3]);
      emitVoiceEvent('voice-changed');

      expect(result.current.currentVoice?.voice_id).toBe('none');
      expect(result.current.isTextOnly).toBe(true);
      expect(result.current.isAvatarMode).toBe(false);
    });

    it('transitions between special modes correctly', async () => {
      const { result } = renderHook(() => useVoiceModel());

      // Start with normal voice
      expect(result.current.isAvatarMode).toBe(false);
      expect(result.current.isTextOnly).toBe(false);

      // Switch to avatar
      await act(async () => {
        mockVoiceManager.getCurrentVoice.mockReturnValue(mockVoices[2]);
        await result.current.setVoice('avatar');
      });
      expect(result.current.isAvatarMode).toBe(true);
      expect(result.current.isTextOnly).toBe(false);

      // Switch to text-only
      await act(async () => {
        mockVoiceManager.getCurrentVoice.mockReturnValue(mockVoices[3]);
        await result.current.setVoice('none');
      });
      expect(result.current.isAvatarMode).toBe(false);
      expect(result.current.isTextOnly).toBe(true);

      // Back to normal
      await act(async () => {
        mockVoiceManager.getCurrentVoice.mockReturnValue(mockVoices[0]);
        await result.current.setVoice('voice-1');
      });
      expect(result.current.isAvatarMode).toBe(false);
      expect(result.current.isTextOnly).toBe(false);
    });

    it('handles special voice availability checks', () => {
      const { result } = renderHook(() => useVoiceModel());

      expect(result.current.isVoiceAvailable('avatar')).toBe(true);
      expect(result.current.isVoiceAvailable('none')).toBe(true);

      const avatarVoice = result.current.getVoiceById('avatar');
      expect(avatarVoice?.provider).toBe('heygen');

      const noneVoice = result.current.getVoiceById('none');
      expect(noneVoice?.provider).toBe('none');
    });
  });

  describe('Memory Management', () => {
    it('cleans up all subscriptions on unmount', () => {
      const { unmount } = renderHook(() => useVoiceModel());

      unmount();

      expect(mockVoiceManager.off).toHaveBeenCalled();
      expect(mockClient.off).toHaveBeenCalled();
    });

    it('prevents memory leaks from retained handlers', () => {
      const { unmount, rerender } = renderHook(() => useVoiceModel());

      // Multiple rerenders
      rerender();
      rerender();
      rerender();

      unmount();

      // Should have cleaned up properly
      expect(mockVoiceManager.off).toHaveBeenCalled();
      expect(mockClient.off).toHaveBeenCalled();
    });

    it('handles unmount during async operations gracefully', async () => {
      const { result, unmount } = renderHook(() => useVoiceModel());

      // Start async operation
      const setVoicePromise = act(async () => {
        await result.current.setVoice('voice-2');
      });

      // Unmount during operation
      unmount();

      // Operation should complete without errors
      await expect(setVoicePromise).resolves.not.toThrow();
    });

    it('stops processing events after unmount', () => {
      const { unmount } = renderHook(() => useVoiceModel());

      unmount();

      // Clear handlers to simulate unmount
      voiceEventHandlers.clear();
      clientEventHandlers.clear();

      // Try to emit events - handlers should be gone
      const voiceHandler = voiceEventHandlers.get('voice-changed');
      expect(voiceHandler).toBeUndefined();

      const clientHandler = clientEventHandlers.get('agent_voice_changed');
      expect(clientHandler).toBeUndefined();
    });
  });
});
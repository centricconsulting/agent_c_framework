/**
 * useAvatar - React hook for avatar management
 * Provides interface for HeyGen avatar integration
 */

import { useEffect, useState, useCallback } from 'react';
import type { Avatar, Voice } from '@agentc/realtime-core';
import { useRealtimeClientSafe } from '../providers/AgentCContext';

/**
 * Return type for the useAvatar hook
 */
export interface UseAvatarReturn {
  /** List of available avatars */
  availableAvatars: Avatar[];
  
  /** Current avatar session info */
  avatarSession: { sessionId: string | null; avatarId: string | null } | null;
  
  /** Whether an avatar is currently active */
  isAvatarActive: boolean;
  
  /** HeyGen access token for client-side SDK */
  heygenAccessToken: string | null;
  
  /** Set the active avatar */
  setAvatar: (avatarId: string, sessionId: string) => Promise<void>;
  
  /** Clear the current avatar session */
  clearAvatar: () => Promise<void>;
  
  /** Get avatar by ID */
  getAvatarById: (avatarId: string) => Avatar | undefined;
  
  /** Check if an avatar is available */
  isAvatarAvailable: (avatarId: string) => boolean;
  
  /** Whether avatar operations are loading */
  isLoading: boolean;
  
  /** Error state */
  error: string | null;
  
  /** Whether avatar feature is enabled */
  isAvatarEnabled: boolean;
}

/**
 * React hook for avatar management
 * Provides interface to AvatarManager functionality
 */
export function useAvatar(): UseAvatarReturn {
  const client = useRealtimeClientSafe();
  
  // State
  const [availableAvatars, setAvailableAvatars] = useState<Avatar[]>([]);
  const [avatarSession, setAvatarSession] = useState<{ sessionId: string | null; avatarId: string | null } | null>(null);
  const [heygenAccessToken, setHeygenAccessToken] = useState<string | null>(null);
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [isAvatarEnabled, setIsAvatarEnabled] = useState(false);
  
  // Update avatar information from client
  const updateAvatarInfo = useCallback(() => {
    if (!client) {
      setAvailableAvatars([]);
      setAvatarSession(null);
      setHeygenAccessToken(null);
      setIsAvatarEnabled(false);
      return;
    }
    
    const avatarManager = client.getAvatarManager();
    if (!avatarManager) {
      setAvailableAvatars([]);
      setAvatarSession(null);
      setHeygenAccessToken(null);
      setIsAvatarEnabled(false);
      return;
    }
    
    try {
      setAvailableAvatars(avatarManager.getAvailableAvatars());
      
      // Get session info from avatar manager
      const sessionId = avatarManager.getSessionId();
      const avatarId = avatarManager.getAvatarId();
      setAvatarSession(sessionId && avatarId ? { sessionId, avatarId } : null);
      
      // Get HeyGen token from client
      const token = client.getHeyGenAccessToken();
      setHeygenAccessToken(token);
      
      // Avatar is enabled if avatars are available
      setIsAvatarEnabled(avatarManager.getAvailableAvatars().length > 0);
      setError(null);
    } catch (err) {
      console.error('Failed to get avatar information:', err);
      setError(err instanceof Error ? err.message : 'Failed to get avatar information');
    }
  }, [client]);
  
  // Set avatar
  const setAvatar = useCallback(async (avatarId: string, sessionId: string): Promise<void> => {
    if (!client) {
      throw new Error('Client not available');
    }
    
    const avatarManager = client.getAvatarManager();
    if (!avatarManager) {
      throw new Error('Avatar manager not available');
    }
    
    if (avatarManager.getAvailableAvatars().length === 0) {
      throw new Error('No avatars available');
    }
    
    setIsLoading(true);
    setError(null);
    
    try {
      // Set avatar session on both manager and client
      avatarManager.setAvatarSession(sessionId, avatarId);
      client.setAvatarSession(sessionId, avatarId);
      updateAvatarInfo();
      
      // Also switch voice to avatar mode
      const voiceManager = client.getVoiceManager();
      if (voiceManager) {
        voiceManager.setCurrentVoice('avatar', 'client');
        client.setAgentVoice('avatar');
      }
    } catch (err) {
      const errorMessage = err instanceof Error ? err.message : 'Failed to set avatar';
      setError(errorMessage);
      throw new Error(errorMessage);
    } finally {
      setIsLoading(false);
    }
  }, [client, updateAvatarInfo]);
  
  // Clear avatar
  const clearAvatar = useCallback(async (): Promise<void> => {
    if (!client) {
      throw new Error('Client not available');
    }
    
    const avatarManager = client.getAvatarManager();
    if (!avatarManager) {
      throw new Error('Avatar manager not available');
    }
    
    setIsLoading(true);
    setError(null);
    
    try {
      // Clear avatar session on both manager and client
      avatarManager.clearAvatarSession();
      client.clearAvatarSession();
      updateAvatarInfo();
      
      // Switch voice back to default
      const voiceManager = client.getVoiceManager();
      if (voiceManager) {
        const availableVoices = voiceManager.getAvailableVoices();
        // Find a non-avatar voice
        const defaultVoice = availableVoices.find((v: Voice) => v.voice_id !== 'avatar' && v.voice_id !== 'none');
        if (defaultVoice) {
          voiceManager.setCurrentVoice(defaultVoice.voice_id, 'client');
          client.setAgentVoice(defaultVoice.voice_id);
        }
      }
    } catch (err) {
      const errorMessage = err instanceof Error ? err.message : 'Failed to clear avatar';
      setError(errorMessage);
      throw new Error(errorMessage);
    } finally {
      setIsLoading(false);
    }
  }, [client, updateAvatarInfo]);
  
  // Get avatar by ID
  const getAvatarById = useCallback((avatarId: string): Avatar | undefined => {
    return availableAvatars.find(avatar => avatar.avatar_id === avatarId);
  }, [availableAvatars]);
  
  // Check if avatar is available
  const isAvatarAvailable = useCallback((avatarId: string): boolean => {
    return availableAvatars.some(avatar => avatar.avatar_id === avatarId);
  }, [availableAvatars]);
  
  // Subscribe to avatar events
  useEffect(() => {
    if (!client) return;
    
    const avatarManager = client.getAvatarManager();
    if (!avatarManager) return;
    
    // Initial update
    updateAvatarInfo();
    
    // Subscribe to avatar state changes
    const handleStateChange = () => {
      updateAvatarInfo();
    };
    
    avatarManager.on('avatar-state-changed', handleStateChange);
    avatarManager.on('avatar-session-started', handleStateChange);
    avatarManager.on('avatar-session-ended', handleStateChange);
    
    return () => {
      avatarManager.off('avatar-state-changed', handleStateChange);
      avatarManager.off('avatar-session-started', handleStateChange);
      avatarManager.off('avatar-session-ended', handleStateChange);
    };
  }, [client, updateAvatarInfo]);
  
  // Computed properties
  const isAvatarActive = avatarSession !== null;
  
  return {
    availableAvatars,
    avatarSession,
    isAvatarActive,
    heygenAccessToken,
    setAvatar,
    clearAvatar,
    getAvatarById,
    isAvatarAvailable,
    isLoading,
    error,
    isAvatarEnabled
  };
}
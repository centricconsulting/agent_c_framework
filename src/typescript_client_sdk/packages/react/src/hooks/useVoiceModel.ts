/**
 * useVoiceModel - React hook for voice model management
 * Provides access to available voices and current voice settings
 */

import { useEffect, useState, useCallback } from 'react';
import type { Voice } from '@agentc/realtime-core';
import { useRealtimeClientSafe } from '../providers/AgentCContext';

/**
 * Return type for the useVoiceModel hook
 */
export interface UseVoiceModelReturn {
  /** Currently selected voice */
  currentVoice: Voice | null;
  
  /** List of available voices */
  availableVoices: Voice[];
  
  /** Whether avatar mode is active */
  isAvatarMode: boolean;
  
  /** Whether text-only mode is active (voice id = 'none') */
  isTextOnly: boolean;
  
  /** Set the active voice */
  setVoice: (voiceId: string) => Promise<void>;
  
  /** Check if a voice is available */
  isVoiceAvailable: (voiceId: string) => boolean;
  
  /** Get voice by ID */
  getVoiceById: (voiceId: string) => Voice | undefined;
  
  /** Whether voice selection is loading */
  isLoading: boolean;
  
  /** Error message if voice selection failed */
  error: string | null;
}

/**
 * React hook for voice model management
 * Provides interface to VoiceManager functionality
 */
export function useVoiceModel(): UseVoiceModelReturn {
  const client = useRealtimeClientSafe();
  
  // State
  const [currentVoice, setCurrentVoice] = useState<Voice | null>(null);
  const [availableVoices, setAvailableVoices] = useState<Voice[]>([]);
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  
  // Update voice information from client
  const updateVoiceInfo = useCallback(() => {
    if (!client) {
      setCurrentVoice(null);
      setAvailableVoices([]);
      return;
    }
    
    const voiceManager = client.getVoiceManager();
    if (!voiceManager) {
      setCurrentVoice(null);
      setAvailableVoices([]);
      return;
    }
    
    try {
      setCurrentVoice(voiceManager.getCurrentVoice());
      setAvailableVoices(voiceManager.getAvailableVoices());
      setError(null);
    } catch (err) {
      console.error('Failed to get voice information:', err);
      setError(err instanceof Error ? err.message : 'Failed to get voice information');
    }
  }, [client]);
  
  // Set voice
  const setVoice = useCallback(async (voiceId: string): Promise<void> => {
    if (!client) {
      throw new Error('Client not available');
    }
    
    const voiceManager = client.getVoiceManager();
    if (!voiceManager) {
      throw new Error('Voice manager not available');
    }
    
    setIsLoading(true);
    setError(null);
    
    try {
      // Voice manager doesn't have async setVoice, use setCurrentVoice
      const success = voiceManager.setCurrentVoice(voiceId, 'client');
      if (!success) {
        throw new Error('Failed to set voice');
      }
      
      // Also send to server
      client.setAgentVoice(voiceId);
      updateVoiceInfo();
    } catch (err) {
      const errorMessage = err instanceof Error ? err.message : 'Failed to set voice';
      setError(errorMessage);
      throw new Error(errorMessage);
    } finally {
      setIsLoading(false);
    }
  }, [client, updateVoiceInfo]);
  
  // Check if voice is available
  const isVoiceAvailable = useCallback((voiceId: string): boolean => {
    return availableVoices.some(voice => voice.voice_id === voiceId);
  }, [availableVoices]);
  
  // Get voice by ID
  const getVoiceById = useCallback((voiceId: string): Voice | undefined => {
    return availableVoices.find(voice => voice.voice_id === voiceId);
  }, [availableVoices]);
  
  // Subscribe to voice changes
  useEffect(() => {
    if (!client) return;
    
    const voiceManager = client.getVoiceManager();
    if (!voiceManager) return;
    
    // Initial update
    updateVoiceInfo();
    
    // Subscribe to voice changes
    const handleVoiceChange = () => {
      updateVoiceInfo();
    };
    
    voiceManager.on('voice-changed', handleVoiceChange);
    
    // Also listen for agent_voice_changed events from server
    const handleServerVoiceChange = () => {
      updateVoiceInfo();
    };
    
    client.on('agent_voice_changed', handleServerVoiceChange);
    
    return () => {
      voiceManager.off('voice-changed', handleVoiceChange);
      client.off('agent_voice_changed', handleServerVoiceChange);
    };
  }, [client, updateVoiceInfo]);
  
  // Computed properties
  const isAvatarMode = currentVoice?.voice_id === 'avatar';
  const isTextOnly = currentVoice?.voice_id === 'none';
  
  return {
    currentVoice,
    availableVoices,
    isAvatarMode,
    isTextOnly,
    setVoice,
    isVoiceAvailable,
    getVoiceById,
    isLoading,
    error
  };
}
/**
 * useOutputMode - React hook for output mode management
 * Coordinates between UI output modes (text/voice/avatar) and SDK voice model selection
 */

import { useState, useCallback, useEffect } from 'react';
import { useVoiceModel } from './useVoiceModel';

export type OutputMode = 'text' | 'voice' | 'avatar';

/**
 * Voice configurations for different output modes
 */
const VOICE_MODE_CONFIG = {
  text: 'none',        // Text-only mode uses 'none' voice
  voice: 'alloy',      // Default voice for voice mode
  avatar: 'avatar'     // Avatar mode uses special 'avatar' voice
} as const;

export interface UseOutputModeOptions {
  /**
   * Initial output mode
   */
  initialMode?: OutputMode;
  
  /**
   * Default voice ID for voice mode
   */
  defaultVoiceId?: string;
  
  /**
   * Callback when mode changes
   */
  onModeChange?: (mode: OutputMode) => void;
}

export interface UseOutputModeReturn {
  /**
   * Current output mode
   */
  outputMode: OutputMode;
  
  /**
   * Set the output mode
   */
  setOutputMode: (mode: OutputMode) => void;
  
  /**
   * Whether currently in text-only mode
   */
  isTextMode: boolean;
  
  /**
   * Whether currently in voice mode
   */
  isVoiceMode: boolean;
  
  /**
   * Whether currently in avatar mode
   */
  isAvatarMode: boolean;
  
  /**
   * Switch to text mode
   */
  switchToText: () => void;
  
  /**
   * Switch to voice mode
   */
  switchToVoice: () => void;
  
  /**
   * Switch to avatar mode
   */
  switchToAvatar: () => void;
  
  /**
   * Whether the mode is currently switching
   */
  isSwitching: boolean;
  
  /**
   * Error message if mode switch failed
   */
  error: string | null;
}

/**
 * React hook to manage output mode state and SDK voice switching
 * Coordinates between UI output mode and SDK voice model selection
 * 
 * @example
 * ```tsx
 * const {
 *   outputMode,
 *   setOutputMode,
 *   isTextMode,
 *   isVoiceMode,
 *   isAvatarMode
 * } = useOutputMode({ initialMode: 'voice' });
 * 
 * // Switch to text mode
 * setOutputMode('text');
 * 
 * // Or use convenience methods
 * switchToVoice();
 * ```
 */
export function useOutputMode(options: UseOutputModeOptions = {}): UseOutputModeReturn {
  const {
    initialMode = 'text',
    defaultVoiceId = 'alloy',
    onModeChange
  } = options;
  
  const { 
    setVoice, 
    currentVoice, 
    isVoiceAvailable, 
    availableVoices,
    isLoading: isVoiceLoading 
  } = useVoiceModel();
  
  // State
  const [outputMode, setOutputModeState] = useState<OutputMode>(initialMode);
  const [isSwitching, setIsSwitching] = useState(false);
  const [error, setError] = useState<string | null>(null);
  
  /**
   * Get the appropriate voice ID for a given mode
   */
  const getVoiceIdForMode = useCallback((mode: OutputMode): string => {
    switch (mode) {
      case 'text':
        return VOICE_MODE_CONFIG.text;
      case 'avatar':
        return VOICE_MODE_CONFIG.avatar;
      case 'voice':
        // Use default voice or first available voice
        if (isVoiceAvailable(defaultVoiceId)) {
          return defaultVoiceId;
        }
        // Find first available voice that's not 'none' or 'avatar'
        const availableVoice = availableVoices.find(
          v => v.voice_id !== 'none' && v.voice_id !== 'avatar'
        );
        return availableVoice?.voice_id || VOICE_MODE_CONFIG.voice;
    }
  }, [defaultVoiceId, isVoiceAvailable, availableVoices]);
  
  /**
   * Set the output mode and update SDK voice accordingly
   */
  const setOutputMode = useCallback(async (newMode: OutputMode) => {
    if (newMode === outputMode) return;
    
    setIsSwitching(true);
    setError(null);
    
    try {
      // Get the appropriate voice ID for the new mode
      const voiceId = getVoiceIdForMode(newMode);
      
      // Check if voice is available
      if (!isVoiceAvailable(voiceId)) {
        // Special handling for 'none' and 'avatar' which might not be in the list
        if (voiceId !== 'none' && voiceId !== 'avatar') {
          throw new Error(`Voice "${voiceId}" is not available for ${newMode} mode`);
        }
      }
      
      // Update SDK voice
      await setVoice(voiceId);
      
      // Update local state
      setOutputModeState(newMode);
      
      // Call callback if provided
      onModeChange?.(newMode);
    } catch (err) {
      const errorMessage = err instanceof Error ? err.message : 'Failed to switch output mode';
      setError(errorMessage);
      console.error(`Failed to switch to ${newMode} mode:`, err);
      
      // Don't update mode if voice switch failed
    } finally {
      setIsSwitching(false);
    }
  }, [outputMode, getVoiceIdForMode, isVoiceAvailable, setVoice, onModeChange]);
  
  /**
   * Convenience methods for switching modes
   */
  const switchToText = useCallback(() => {
    setOutputMode('text');
  }, [setOutputMode]);
  
  const switchToVoice = useCallback(() => {
    setOutputMode('voice');
  }, [setOutputMode]);
  
  const switchToAvatar = useCallback(() => {
    setOutputMode('avatar');
  }, [setOutputMode]);
  
  /**
   * Sync with SDK voice changes
   * If voice changes externally, update our mode accordingly
   */
  useEffect(() => {
    if (!currentVoice || isSwitching) return;
    
    // Determine mode based on current voice
    let detectedMode: OutputMode | null = null;
    
    if (currentVoice.voice_id === 'none') {
      detectedMode = 'text';
    } else if (currentVoice.voice_id === 'avatar') {
      detectedMode = 'avatar';
    } else {
      // Any other voice means voice mode
      detectedMode = 'voice';
    }
    
    // Update mode if it doesn't match
    if (detectedMode && detectedMode !== outputMode) {
      setOutputModeState(detectedMode);
      onModeChange?.(detectedMode);
    }
  }, [currentVoice, outputMode, isSwitching, onModeChange]);
  
  /**
   * Initialize voice on mount based on initial mode
   */
  useEffect(() => {
    // Only run once on mount
    const voiceId = getVoiceIdForMode(initialMode);
    
    // Set initial voice if available
    if (isVoiceAvailable(voiceId) || voiceId === 'none' || voiceId === 'avatar') {
      setVoice(voiceId).catch(err => {
        console.error('Failed to set initial voice:', err);
        setError(err instanceof Error ? err.message : 'Failed to set initial voice');
      });
    }
  }, []); // eslint-disable-line react-hooks/exhaustive-deps
  
  // Computed properties
  const isTextMode = outputMode === 'text';
  const isVoiceMode = outputMode === 'voice';
  const isAvatarModeActive = outputMode === 'avatar';
  
  return {
    outputMode,
    setOutputMode,
    isTextMode,
    isVoiceMode,
    isAvatarMode: isAvatarModeActive,
    switchToText,
    switchToVoice,
    switchToAvatar,
    isSwitching: isSwitching || isVoiceLoading,
    error
  };
}
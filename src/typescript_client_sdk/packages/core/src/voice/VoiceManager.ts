/**
 * VoiceManager - Centralized voice model management for Agent C SDK
 * 
 * This class manages available voices, tracks the current voice selection,
 * and provides voice-related functionality for the realtime client.
 * It supports special voice modes like avatar and text-only, as well as
 * various TTS voices with different formats.
 */

import { EventEmitter } from '../events/EventEmitter';
import { Voice } from '../events/types/CommonTypes';

/**
 * Voice manager event types
 */
export interface VoiceManagerEvents {
  'voice-changed': {
    previousVoice: Voice | null;
    currentVoice: Voice | null;
    source: 'client' | 'server';
  };
  'voices-updated': {
    voices: Voice[];
    previousCount: number;
    currentCount: number;
  };
}

/**
 * Voice manager configuration options
 */
export interface VoiceManagerConfig {
  defaultVoiceId?: string;
  enableLogging?: boolean;
}

/**
 * Special voice identifiers
 */
export const SPECIAL_VOICES = {
  AVATAR: 'avatar',
  NONE: 'none'
} as const;

/**
 * VoiceManager class for managing voice models in the Agent C SDK
 * 
 * Features:
 * - Tracks available voices from login response
 * - Manages current voice selection
 * - Handles voice changes from server events
 * - Supports special voice modes (avatar, text-only)
 * - Emits events for voice changes
 * - Provides voice format information
 */
export class VoiceManager extends EventEmitter<VoiceManagerEvents> {
  private availableVoices: Map<string, Voice>;
  private currentVoice: Voice | null;
  private defaultVoiceId: string;
  private enableLogging: boolean;

  /**
   * Creates a new VoiceManager instance
   * @param config - Optional configuration for the voice manager
   */
  constructor(config: VoiceManagerConfig = {}) {
    super();
    
    this.availableVoices = new Map();
    this.currentVoice = null;
    this.defaultVoiceId = config.defaultVoiceId || SPECIAL_VOICES.NONE;
    this.enableLogging = config.enableLogging ?? true;
    
    this.log('VoiceManager initialized', { defaultVoiceId: this.defaultVoiceId });
  }

  /**
   * Set the available voices from login response
   * @param voices - Array of available voices
   * @emits voices-updated when the available voices list is updated
   */
  setAvailableVoices(voices: Voice[]): void {
    const previousCount = this.availableVoices.size;
    
    // Clear existing voices
    this.availableVoices.clear();
    
    // Add new voices to the map
    for (const voice of voices) {
      if (!voice.voice_id) {
        this.warn('Voice missing voice_id, skipping', voice);
        continue;
      }
      this.availableVoices.set(voice.voice_id, voice);
    }

    // Add special voices if not already present
    this.ensureSpecialVoices();
    
    const currentCount = this.availableVoices.size;
    
    this.log('Available voices updated', {
      previousCount,
      currentCount,
      voiceIds: Array.from(this.availableVoices.keys())
    });

    // If no current voice is set, try to set the default
    if (!this.currentVoice && this.availableVoices.size > 0) {
      this.setDefaultVoice();
    }

    // Emit voices updated event
    this.emit('voices-updated', {
      voices: Array.from(this.availableVoices.values()),
      previousCount,
      currentCount
    });
  }

  /**
   * Set the current voice by ID
   * @param voiceId - ID of the voice to set as current
   * @param source - Source of the voice change (client or server)
   * @returns True if voice was successfully set, false otherwise
   * @emits voice-changed when the current voice changes
   */
  setCurrentVoice(voiceId: string, source: 'client' | 'server' = 'client'): boolean {
    // Handle null or undefined voiceId
    if (!voiceId) {
      this.error('Cannot set voice: voiceId is null or undefined');
      return false;
    }

    // Check if voice exists in available voices
    const voice = this.availableVoices.get(voiceId);
    if (!voice) {
      // For special voices that might not be in the list, create them
      if (voiceId === SPECIAL_VOICES.AVATAR || voiceId === SPECIAL_VOICES.NONE) {
        this.ensureSpecialVoices();
        const specialVoice = this.availableVoices.get(voiceId);
        if (specialVoice) {
          return this.updateCurrentVoice(specialVoice, source);
        }
      }
      
      this.error(`Voice with ID "${voiceId}" not found in available voices`);
      return false;
    }

    return this.updateCurrentVoice(voice, source);
  }

  /**
   * Get the current voice
   * @returns Current voice or null if not set
   */
  getCurrentVoice(): Voice | null {
    return this.currentVoice;
  }

  /**
   * Get all available voices
   * @returns Array of available voices
   */
  getAvailableVoices(): Voice[] {
    return Array.from(this.availableVoices.values());
  }

  /**
   * Get a specific voice by ID
   * @param voiceId - ID of the voice to retrieve
   * @returns Voice object or undefined if not found
   */
  getVoiceById(voiceId: string): Voice | undefined {
    return this.availableVoices.get(voiceId);
  }

  /**
   * Check if the current voice is in avatar mode
   * @returns True if current voice is avatar mode
   */
  isAvatarVoice(): boolean {
    return this.currentVoice?.voice_id === SPECIAL_VOICES.AVATAR;
  }

  /**
   * Check if the current voice is text-only (no audio)
   * @returns True if current voice is text-only mode
   */
  isTextOnlyVoice(): boolean {
    return this.currentVoice?.voice_id === SPECIAL_VOICES.NONE;
  }

  /**
   * Get the audio format for the current voice
   * @returns Audio format string or null if no voice/text-only/avatar
   */
  getAudioFormat(): string | null {
    if (!this.currentVoice) {
      return null;
    }

    // Special voices don't have audio formats
    if (this.isAvatarVoice() || this.isTextOnlyVoice()) {
      return null;
    }

    return this.currentVoice.output_format || null;
  }

  /**
   * Handle a voice change event from the server
   * @param voiceId - ID of the new voice from server
   */
  handleServerVoiceChange(voiceId: string): void {
    this.log('Handling server voice change', { voiceId });
    this.setCurrentVoice(voiceId, 'server');
  }

  /**
   * Reset the voice manager to initial state
   */
  reset(): void {
    const previousVoice = this.currentVoice;
    
    this.currentVoice = null;
    this.availableVoices.clear();
    this.ensureSpecialVoices();
    
    if (previousVoice) {
      this.emit('voice-changed', {
        previousVoice,
        currentVoice: null,
        source: 'client'
      });
    }
    
    this.log('VoiceManager reset');
  }

  /**
   * Get voice capabilities for the current voice
   * @returns Object describing voice capabilities
   */
  getVoiceCapabilities(): {
    hasAudio: boolean;
    hasAvatar: boolean;
    format: string | null;
    vendor: string | null;
  } {
    const isAvatar = this.isAvatarVoice();
    const isTextOnly = this.isTextOnlyVoice();
    
    return {
      hasAudio: !isTextOnly && !isAvatar,
      hasAvatar: isAvatar,
      format: this.getAudioFormat(),
      vendor: this.currentVoice?.vendor || null
    };
  }

  /**
   * Check if a voice ID is available
   * @param voiceId - Voice ID to check
   * @returns True if voice is available
   */
  isVoiceAvailable(voiceId: string): boolean {
    return this.availableVoices.has(voiceId);
  }

  /**
   * Get voices filtered by vendor
   * @param vendor - Vendor name to filter by
   * @returns Array of voices from the specified vendor
   */
  getVoicesByVendor(vendor: string): Voice[] {
    return Array.from(this.availableVoices.values())
      .filter(voice => voice.vendor === vendor);
  }

  /**
   * Get voices filtered by output format
   * @param format - Output format to filter by
   * @returns Array of voices with the specified format
   */
  getVoicesByFormat(format: string): Voice[] {
    return Array.from(this.availableVoices.values())
      .filter(voice => voice.output_format === format);
  }

  // Private methods

  /**
   * Update the current voice and emit change event
   * @param voice - New voice to set as current
   * @param source - Source of the change
   * @returns True if voice was changed
   */
  private updateCurrentVoice(voice: Voice, source: 'client' | 'server'): boolean {
    const previousVoice = this.currentVoice;
    
    // Check if voice is actually changing
    if (previousVoice?.voice_id === voice.voice_id) {
      this.log('Voice already set to', { voiceId: voice.voice_id });
      return true;
    }

    this.currentVoice = voice;
    
    this.log('Current voice changed', {
      from: previousVoice?.voice_id || 'none',
      to: voice.voice_id,
      source
    });

    // Emit voice changed event
    this.emit('voice-changed', {
      previousVoice,
      currentVoice: voice,
      source
    });

    return true;
  }

  /**
   * Set the default voice based on configuration
   */
  private setDefaultVoice(): void {
    // Try to set the configured default
    if (this.defaultVoiceId && this.availableVoices.has(this.defaultVoiceId)) {
      this.setCurrentVoice(this.defaultVoiceId, 'client');
      return;
    }

    // Fallback to 'none' if available
    if (this.availableVoices.has(SPECIAL_VOICES.NONE)) {
      this.setCurrentVoice(SPECIAL_VOICES.NONE, 'client');
      return;
    }

    // Set first available voice as fallback
    const firstVoice = this.availableVoices.values().next().value;
    if (firstVoice) {
      this.setCurrentVoice(firstVoice.voice_id, 'client');
    }
  }

  /**
   * Ensure special voices are available in the voices map
   */
  private ensureSpecialVoices(): void {
    // Add 'none' voice if not present
    if (!this.availableVoices.has(SPECIAL_VOICES.NONE)) {
      this.availableVoices.set(SPECIAL_VOICES.NONE, {
        voice_id: SPECIAL_VOICES.NONE,
        vendor: 'system',
        description: 'Text-only mode (no audio output)',
        output_format: 'none'
      });
    }

    // Add 'avatar' voice if not present
    if (!this.availableVoices.has(SPECIAL_VOICES.AVATAR)) {
      this.availableVoices.set(SPECIAL_VOICES.AVATAR, {
        voice_id: SPECIAL_VOICES.AVATAR,
        vendor: 'heygen',
        description: 'Avatar mode (audio handled by HeyGen)',
        output_format: 'avatar'
      });
    }
  }

  /**
   * Log a message if logging is enabled
   * @param message - Message to log
   * @param data - Optional data to log
   */
  private log(message: string, data?: unknown): void {
    if (this.enableLogging) {
      console.warn(`[VoiceManager] ${message}`, data || '');
    }
  }

  /**
   * Log a warning if logging is enabled
   * @param message - Warning message
   * @param data - Optional data to log
   */
  private warn(message: string, data?: unknown): void {
    if (this.enableLogging) {
      console.warn(`[VoiceManager] ${message}`, data || '');
    }
  }

  /**
   * Log an error
   * @param message - Error message
   * @param data - Optional data to log
   */
  private error(message: string, data?: unknown): void {
    console.error(`[VoiceManager] ${message}`, data || '');
  }
}

/**
 * Export a singleton instance for convenience
 * Applications can either use this singleton or create their own instances
 */
export const voiceManager = new VoiceManager();
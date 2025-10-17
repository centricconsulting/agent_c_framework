/**
 * Voice utility functions for the Agent C SDK
 * 
 * These are standalone utility functions for working with voice models
 * that can be used independently of the VoiceManager instance.
 */

import { Voice } from '../events/types/CommonTypes';
import { VoiceModel } from '../audio/types';
import { SPECIAL_VOICES } from './VoiceManager';

/**
 * Audio format types supported by voices
 */
export enum AudioFormat {
  PCM16 = 'pcm16',
  PCM8 = 'pcm8',
  OPUS = 'opus',
  MP3 = 'mp3',
  NONE = 'none',
  AVATAR = 'avatar'
}

/**
 * Voice vendor types
 */
export enum VoiceVendor {
  OPENAI = 'openai',
  HEYGEN = 'heygen',
  ELEVENLABS = 'elevenlabs',
  SYSTEM = 'system'
}

/**
 * Check if a voice ID represents avatar mode
 * @param voiceId - Voice ID to check
 * @returns True if voice is avatar mode
 */
export function isAvatarVoiceId(voiceId: string): boolean {
  return voiceId === SPECIAL_VOICES.AVATAR;
}

/**
 * Check if a voice ID represents text-only mode
 * @param voiceId - Voice ID to check
 * @returns True if voice is text-only mode
 */
export function isTextOnlyVoiceId(voiceId: string): boolean {
  return voiceId === SPECIAL_VOICES.NONE;
}

/**
 * Check if a voice ID is a special voice (avatar or text-only)
 * @param voiceId - Voice ID to check
 * @returns True if voice is special
 */
export function isSpecialVoiceId(voiceId: string): boolean {
  return isAvatarVoiceId(voiceId) || isTextOnlyVoiceId(voiceId);
}

/**
 * Check if a voice object represents avatar mode
 * @param voice - Voice object to check
 * @returns True if voice is avatar mode
 */
export function isAvatarVoice(voice: Voice | null): boolean {
  return voice?.voice_id === SPECIAL_VOICES.AVATAR;
}

/**
 * Check if a voice object represents text-only mode
 * @param voice - Voice object to check
 * @returns True if voice is text-only mode
 */
export function isTextOnlyVoice(voice: Voice | null): boolean {
  return voice?.voice_id === SPECIAL_VOICES.NONE;
}

/**
 * Check if a voice object is a special voice
 * @param voice - Voice object to check
 * @returns True if voice is special
 */
export function isSpecialVoice(voice: Voice | null): boolean {
  return isAvatarVoice(voice) || isTextOnlyVoice(voice);
}

/**
 * Check if a voice has audio output
 * @param voice - Voice object to check
 * @returns True if voice has audio output
 */
export function hasAudioOutput(voice: Voice | null): boolean {
  if (!voice) return false;
  return !isAvatarVoice(voice) && !isTextOnlyVoice(voice);
}

/**
 * Get the audio format for a voice
 * @param voice - Voice object
 * @returns Audio format or null for special voices
 */
export function getVoiceAudioFormat(voice: Voice | null): string | null {
  if (!voice) return null;
  if (isSpecialVoice(voice)) return null;
  return voice.output_format || null;
}

/**
 * Convert a Voice object to VoiceModel format
 * @param voice - Voice object to convert
 * @returns VoiceModel object
 */
export function voiceToVoiceModel(voice: Voice): VoiceModel {
  return {
    voice_id: voice.voice_id,
    format: voice.output_format || 'pcm16',
    sampleRate: 24000, // Default sample rate for Agent C voices
    vendor: voice.vendor || 'unknown',
    description: voice.description || ''
  };
}

/**
 * Get a human-readable voice description
 * @param voice - Voice object
 * @returns Human-readable description
 */
export function getVoiceDescription(voice: Voice | null): string {
  if (!voice) return 'No voice selected';
  
  if (isAvatarVoice(voice)) {
    return 'Avatar mode (HeyGen)';
  }
  
  if (isTextOnlyVoice(voice)) {
    return 'Text-only mode';
  }
  
  // For regular voices, use description or construct from vendor/id
  if (voice.description) {
    return voice.description;
  }
  
  return `${voice.vendor || 'Unknown'} - ${voice.voice_id}`;
}

/**
 * Parse audio format enum from string
 * @param format - Format string
 * @returns AudioFormat enum value or null
 */
export function parseAudioFormat(format: string): AudioFormat | null {
  const normalizedFormat = format.toLowerCase();
  
  switch (normalizedFormat) {
    case 'pcm16':
      return AudioFormat.PCM16;
    case 'pcm8':
      return AudioFormat.PCM8;
    case 'opus':
      return AudioFormat.OPUS;
    case 'mp3':
      return AudioFormat.MP3;
    case 'none':
      return AudioFormat.NONE;
    case 'avatar':
      return AudioFormat.AVATAR;
    default:
      return null;
  }
}

/**
 * Get sample rate for audio format
 * @param format - Audio format
 * @returns Sample rate in Hz or null if not applicable
 */
export function getSampleRateForFormat(format: string): number | null {
  const audioFormat = parseAudioFormat(format);
  
  switch (audioFormat) {
    case AudioFormat.PCM16:
      return 24000; // Agent C default for PCM16
    case AudioFormat.PCM8:
      return 8000; // Typical for PCM8
    case AudioFormat.OPUS:
      return 48000; // Opus default
    case AudioFormat.MP3:
      return 44100; // MP3 standard
    case AudioFormat.NONE:
    case AudioFormat.AVATAR:
      return null; // No audio
    default:
      return 24000; // Default fallback
  }
}

/**
 * Check if a voice is from a specific vendor
 * @param voice - Voice object
 * @param vendor - Vendor to check
 * @returns True if voice is from the vendor
 */
export function isVoiceFromVendor(voice: Voice | null, vendor: string): boolean {
  if (!voice) return false;
  return voice.vendor?.toLowerCase() === vendor.toLowerCase();
}

/**
 * Filter voices by vendor
 * @param voices - Array of voices
 * @param vendor - Vendor to filter by
 * @returns Filtered array of voices
 */
export function filterVoicesByVendor(voices: Voice[], vendor: string): Voice[] {
  return voices.filter(voice => isVoiceFromVendor(voice, vendor));
}

/**
 * Filter voices by output format
 * @param voices - Array of voices
 * @param format - Format to filter by
 * @returns Filtered array of voices
 */
export function filterVoicesByFormat(voices: Voice[], format: string): Voice[] {
  return voices.filter(voice => voice.output_format === format);
}

/**
 * Filter out special voices
 * @param voices - Array of voices
 * @returns Array without special voices
 */
export function filterRegularVoices(voices: Voice[]): Voice[] {
  return voices.filter(voice => !isSpecialVoice(voice));
}

/**
 * Sort voices alphabetically by description or ID
 * @param voices - Array of voices
 * @returns Sorted array of voices
 */
export function sortVoicesAlphabetically(voices: Voice[]): Voice[] {
  return [...voices].sort((a, b) => {
    const aName = a.description || a.voice_id;
    const bName = b.description || b.voice_id;
    return aName.localeCompare(bName);
  });
}

/**
 * Group voices by vendor
 * @param voices - Array of voices
 * @returns Map of vendor to voices
 */
export function groupVoicesByVendor(voices: Voice[]): Map<string, Voice[]> {
  const grouped = new Map<string, Voice[]>();
  
  for (const voice of voices) {
    const vendor = voice.vendor || 'unknown';
    if (!grouped.has(vendor)) {
      grouped.set(vendor, []);
    }
    grouped.get(vendor)!.push(voice);
  }
  
  return grouped;
}

/**
 * Find the default voice from a list
 * @param voices - Array of voices
 * @param preferredVendor - Optional preferred vendor
 * @returns Default voice or null
 */
export function findDefaultVoice(voices: Voice[], preferredVendor?: string): Voice | null {
  // First, try to find text-only mode as safe default
  const textOnly = voices.find(v => isTextOnlyVoice(v));
  if (textOnly) return textOnly;
  
  // If preferred vendor specified, try to find a voice from that vendor
  if (preferredVendor) {
    const vendorVoice = voices.find(v => 
      isVoiceFromVendor(v, preferredVendor) && !isSpecialVoice(v)
    );
    if (vendorVoice) return vendorVoice;
  }
  
  // Find first regular voice
  const regularVoice = voices.find(v => !isSpecialVoice(v));
  if (regularVoice) return regularVoice;
  
  // Return first voice if any
  return voices[0] || null;
}

/**
 * Validate voice configuration
 * @param voice - Voice to validate
 * @returns Object with validation result and errors
 */
export function validateVoice(voice: unknown): { valid: boolean; errors: string[] } {
  const errors: string[] = [];
  
  if (!voice || typeof voice !== 'object') {
    return { valid: false, errors: ['Voice must be an object'] };
  }
  
  // Type assertion for voice properties
  const voiceObj = voice as {
    voice_id?: unknown;
    vendor?: unknown;
    output_format?: unknown;
    description?: unknown;
  };
  
  if (!voiceObj.voice_id || typeof voiceObj.voice_id !== 'string') {
    errors.push('Voice must have a valid voice_id string');
  }
  
  if (voiceObj.vendor !== undefined && typeof voiceObj.vendor !== 'string') {
    errors.push('Voice vendor must be a string if provided');
  }
  
  if (voiceObj.output_format !== undefined && typeof voiceObj.output_format !== 'string') {
    errors.push('Voice output_format must be a string if provided');
  }
  
  if (voiceObj.description !== undefined && typeof voiceObj.description !== 'string') {
    errors.push('Voice description must be a string if provided');
  }
  
  return {
    valid: errors.length === 0,
    errors
  };
}
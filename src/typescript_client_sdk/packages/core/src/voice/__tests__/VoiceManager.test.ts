/**
 * Unit tests for VoiceManager
 * Tests simple getters, setters, and voice state management
 */

import { describe, it, expect, beforeEach, afterEach, vi } from 'vitest';
import { VoiceManager, SPECIAL_VOICES } from '../VoiceManager';
import { serverEventFixtures } from '../../test/fixtures/protocol-events';
import type { Voice } from '../../events/types/CommonTypes';

describe('VoiceManager', () => {
  let voiceManager: VoiceManager;

  beforeEach(() => {
    vi.clearAllMocks();
    voiceManager = new VoiceManager({ enableLogging: false });
  });

  afterEach(() => {
    vi.restoreAllMocks();
  });

  describe('constructor', () => {
    it('should initialize with null current voice', () => {
      expect(voiceManager.getCurrentVoice()).toBeNull();
    });

    it('should initialize with empty available voices', () => {
      // VoiceManager starts with empty voices until setAvailableVoices is called
      const voices = voiceManager.getAvailableVoices();
      expect(voices).toHaveLength(0);
    });
  });

  describe('setAvailableVoices', () => {
    it('should update available voices list', () => {
      const voices = serverEventFixtures.voiceList.voices;
      
      voiceManager.setAvailableVoices(voices);
      
      const available = voiceManager.getAvailableVoices();
      // Should have the 2 voices from fixture plus 2 special voices
      expect(available).toHaveLength(4);
      expect(available.some(v => v.voice_id === 'nova')).toBe(true);
      expect(available.some(v => v.voice_id === 'alloy')).toBe(true);
    });

    it('should add special voices when setting available voices', () => {
      const voices = serverEventFixtures.voiceList.voices;
      
      voiceManager.setAvailableVoices(voices);
      
      const available = voiceManager.getAvailableVoices();
      const voiceIds = available.map(v => v.voice_id);
      expect(voiceIds).toContain(SPECIAL_VOICES.NONE);
      expect(voiceIds).toContain(SPECIAL_VOICES.AVATAR);
    });

    it('should emit voices-updated event', () => {
      const listener = vi.fn();
      voiceManager.on('voices-updated', listener);
      
      const voices = serverEventFixtures.voiceList.voices;
      voiceManager.setAvailableVoices(voices);
      
      expect(listener).toHaveBeenCalledWith({
        voices: expect.any(Array),
        previousCount: 0, // Initially empty
        currentCount: 4   // 2 from fixture + 2 special
      });
    });
  });

  describe('setCurrentVoice', () => {
    beforeEach(() => {
      // Set up available voices
      voiceManager.setAvailableVoices(serverEventFixtures.voiceList.voices);
    });

    it('should update current voice when valid voice ID is provided', () => {
      const result = voiceManager.setCurrentVoice('nova');
      
      expect(result).toBe(true);
      expect(voiceManager.getCurrentVoice()?.voice_id).toBe('nova');
    });

    it('should return false for invalid voice ID', () => {
      const result = voiceManager.setCurrentVoice('invalid-voice');
      
      expect(result).toBe(false);
      expect(voiceManager.getCurrentVoice()?.voice_id).not.toBe('invalid-voice');
    });

    it('should emit voice-changed event when voice changes', () => {
      // First set the current voice to 'none' (which happens after setAvailableVoices)
      voiceManager.setCurrentVoice('none');
      
      const listener = vi.fn();
      voiceManager.on('voice-changed', listener);
      
      voiceManager.setCurrentVoice('nova');
      
      expect(listener).toHaveBeenCalledWith({
        previousVoice: expect.objectContaining({ voice_id: 'none' }),
        currentVoice: expect.objectContaining({ voice_id: 'nova' }),
        source: 'client'
      });
    });

    it('should handle special voice IDs', () => {
      const result = voiceManager.setCurrentVoice(SPECIAL_VOICES.AVATAR);
      
      expect(result).toBe(true);
      expect(voiceManager.getCurrentVoice()?.voice_id).toBe(SPECIAL_VOICES.AVATAR);
    });
  });

  describe('getCurrentVoice', () => {
    it('should return null when no voice is set', () => {
      expect(voiceManager.getCurrentVoice()).toBeNull();
    });

    it('should return the current voice object', () => {
      voiceManager.setAvailableVoices(serverEventFixtures.voiceList.voices);
      voiceManager.setCurrentVoice('nova');
      
      const current = voiceManager.getCurrentVoice();
      expect(current).toMatchObject({
        voice_id: 'nova',
        vendor: 'openai'
      });
    });
  });

  describe('getAvailableVoices', () => {
    it('should return array of all available voices', () => {
      voiceManager.setAvailableVoices(serverEventFixtures.voiceList.voices);
      
      const voices = voiceManager.getAvailableVoices();
      expect(Array.isArray(voices)).toBe(true);
      expect(voices).toHaveLength(4); // 2 from fixture + 2 special
    });
  });

  describe('getVoiceById', () => {
    beforeEach(() => {
      voiceManager.setAvailableVoices(serverEventFixtures.voiceList.voices);
    });

    it('should return voice object for valid ID', () => {
      const voice = voiceManager.getVoiceById('nova');
      
      expect(voice).toBeDefined();
      expect(voice?.voice_id).toBe('nova');
      expect(voice?.vendor).toBe('openai');
    });

    it('should return undefined for invalid ID', () => {
      const voice = voiceManager.getVoiceById('invalid-id');
      
      expect(voice).toBeUndefined();
    });

    it('should return special voices by ID', () => {
      const avatarVoice = voiceManager.getVoiceById(SPECIAL_VOICES.AVATAR);
      const noneVoice = voiceManager.getVoiceById(SPECIAL_VOICES.NONE);
      
      expect(avatarVoice).toBeDefined();
      expect(avatarVoice?.voice_id).toBe(SPECIAL_VOICES.AVATAR);
      expect(noneVoice).toBeDefined();
      expect(noneVoice?.voice_id).toBe(SPECIAL_VOICES.NONE);
    });
  });

  describe('isAvatarVoice', () => {
    it('should return false when no voice is set', () => {
      expect(voiceManager.isAvatarVoice()).toBe(false);
    });

    it('should return true when avatar voice is set', () => {
      voiceManager.setCurrentVoice(SPECIAL_VOICES.AVATAR);
      expect(voiceManager.isAvatarVoice()).toBe(true);
    });

    it('should return false for non-avatar voices', () => {
      voiceManager.setAvailableVoices(serverEventFixtures.voiceList.voices);
      voiceManager.setCurrentVoice('nova');
      expect(voiceManager.isAvatarVoice()).toBe(false);
    });
  });

  describe('isTextOnlyVoice', () => {
    it('should return false when no voice is set', () => {
      expect(voiceManager.isTextOnlyVoice()).toBe(false);
    });

    it('should return true when none voice is set', () => {
      voiceManager.setCurrentVoice(SPECIAL_VOICES.NONE);
      expect(voiceManager.isTextOnlyVoice()).toBe(true);
    });

    it('should return false for audio voices', () => {
      voiceManager.setAvailableVoices(serverEventFixtures.voiceList.voices);
      voiceManager.setCurrentVoice('nova');
      expect(voiceManager.isTextOnlyVoice()).toBe(false);
    });
  });

  describe('isVoiceAvailable', () => {
    beforeEach(() => {
      voiceManager.setAvailableVoices(serverEventFixtures.voiceList.voices);
    });

    it('should return true for available voice', () => {
      expect(voiceManager.isVoiceAvailable('nova')).toBe(true);
      expect(voiceManager.isVoiceAvailable('alloy')).toBe(true);
    });

    it('should return false for unavailable voice', () => {
      expect(voiceManager.isVoiceAvailable('unknown-voice')).toBe(false);
    });

    it('should return true for special voices', () => {
      expect(voiceManager.isVoiceAvailable(SPECIAL_VOICES.AVATAR)).toBe(true);
      expect(voiceManager.isVoiceAvailable(SPECIAL_VOICES.NONE)).toBe(true);
    });
  });

  describe('getVoicesByVendor', () => {
    beforeEach(() => {
      voiceManager.setAvailableVoices(serverEventFixtures.voiceList.voices);
    });

    it('should return voices filtered by vendor', () => {
      const openaiVoices = voiceManager.getVoicesByVendor('openai');
      
      expect(openaiVoices).toHaveLength(2);
      expect(openaiVoices.every(v => v.vendor === 'openai')).toBe(true);
    });

    it('should return empty array for unknown vendor', () => {
      const voices = voiceManager.getVoicesByVendor('unknown-vendor');
      
      expect(voices).toHaveLength(0);
    });
  });

  describe('getVoicesByFormat', () => {
    beforeEach(() => {
      voiceManager.setAvailableVoices(serverEventFixtures.voiceList.voices);
    });

    it('should return voices filtered by output format', () => {
      const pcmVoices = voiceManager.getVoicesByFormat('pcm16');
      
      expect(pcmVoices).toHaveLength(2);
      expect(pcmVoices.every(v => v.output_format === 'pcm16')).toBe(true);
    });

    it('should return empty array for unknown format', () => {
      const voices = voiceManager.getVoicesByFormat('unknown-format');
      
      expect(voices).toHaveLength(0);
    });
  });

  describe('reset', () => {
    it('should clear current voice and available voices', () => {
      voiceManager.setAvailableVoices(serverEventFixtures.voiceList.voices);
      voiceManager.setCurrentVoice('nova');
      
      voiceManager.reset();
      
      expect(voiceManager.getCurrentVoice()).toBeNull();
      // Should only have special voices after reset
      expect(voiceManager.getAvailableVoices()).toHaveLength(2);
    });

    it('should emit voice-changed event when voice was set', () => {
      voiceManager.setAvailableVoices(serverEventFixtures.voiceList.voices);
      voiceManager.setCurrentVoice('nova');
      
      const listener = vi.fn();
      voiceManager.on('voice-changed', listener);
      
      voiceManager.reset();
      
      expect(listener).toHaveBeenCalledWith({
        previousVoice: expect.objectContaining({ voice_id: 'nova' }),
        currentVoice: null,
        source: 'client'
      });
    });
  });
});
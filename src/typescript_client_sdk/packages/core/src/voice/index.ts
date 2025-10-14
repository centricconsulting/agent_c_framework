/**
 * Voice module exports for the Agent C SDK
 */

export {
  VoiceManager,
  VoiceManagerEvents,
  VoiceManagerConfig,
  SPECIAL_VOICES,
  voiceManager
} from './VoiceManager';

export {
  // Enums
  AudioFormat,
  VoiceVendor,
  
  // Voice ID checks
  isAvatarVoiceId,
  isTextOnlyVoiceId,
  isSpecialVoiceId,
  
  // Voice object checks
  isAvatarVoice,
  isTextOnlyVoice,
  isSpecialVoice,
  hasAudioOutput,
  
  // Voice utilities
  getVoiceAudioFormat,
  voiceToVoiceModel,
  getVoiceDescription,
  
  // Format utilities
  parseAudioFormat,
  getSampleRateForFormat,
  
  // Vendor utilities
  isVoiceFromVendor,
  filterVoicesByVendor,
  
  // Filtering and sorting
  filterVoicesByFormat,
  filterRegularVoices,
  sortVoicesAlphabetically,
  groupVoicesByVendor,
  
  // Voice selection
  findDefaultVoice,
  
  // Validation
  validateVoice
} from './utils';
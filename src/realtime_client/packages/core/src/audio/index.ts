/**
 * Audio module exports for the Agent C Realtime Client SDK
 */

// Export the main audio classes
export { AudioProcessor } from './AudioProcessor';
export { AudioService } from './AudioService';

// Export all types and interfaces
export {
  // Core interfaces
  AudioChunkData,
  AudioProcessorConfig,
  AudioProcessorStatus,
  
  // Service types
  AudioServiceState,
  AudioServiceStatus,
  AudioServiceEvents,
  
  // Worklet message types
  WorkletMessage,
  AudioChunkMessage,
  WorkletStatusMessage,
  WorkletControlMessage,
  
  // Event types
  AudioProcessorEvents,
  
  // Error types
  AudioProcessorErrorCode,
  AudioProcessorError,
  
  // Type guards
  isAudioChunkMessage,
  isWorkletStatusMessage,
  
  // Constants
  DEFAULT_AUDIO_CONFIG
} from './types';
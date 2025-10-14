/**
 * Audio module exports for the Agent C Realtime Client SDK
 */

// Export the main audio classes
export { AudioProcessor } from './AudioProcessor';
export { AudioService } from './AudioService';
export { AudioAgentCBridge } from './AudioAgentCBridge';
export { AudioOutputService } from './AudioOutputService';

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
  
  // Bridge types
  AudioAgentCBridgeConfig,
  AudioAgentCBridgeStatus,
  
  // Output types
  AudioOutputStatus,
  AudioStatus,
  VoiceModel,
  
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
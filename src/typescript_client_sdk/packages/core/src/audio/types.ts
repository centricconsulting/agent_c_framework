/**
 * Core audio types and interfaces for the Agent C Realtime Client SDK
 */

/**
 * Audio chunk data structure for transmission
 */
export interface AudioChunkData {
  /** PCM16 audio data as ArrayBuffer */
  content: ArrayBuffer;
  
  /** MIME type of the audio data */
  content_type: string;
  
  /** RMS audio level (0.0 to 1.0) for visualization */
  audio_level: number;
  
  /** Sequential frame counter */
  frame_count: number;
  
  /** Timestamp when audio was captured (milliseconds) */
  timestamp: number;
  
  /** Sample rate in Hz (typically 16000) */
  sample_rate: number;
  
  /** Number of samples in this chunk */
  sample_count: number;
}

/**
 * Configuration for the audio processor
 */
export interface AudioProcessorConfig {
  /** Target sample rate in Hz (default: 16000) */
  sampleRate?: number;
  
  /** Number of audio channels (default: 1 for mono) */
  channelCount?: number;
  
  /** Buffer size in samples before sending chunk (default: 2048) */
  bufferSize?: number;
  
  /** Path to the audio worklet file */
  workletPath?: string;
  
  /** Enable debug logging */
  debug?: boolean;
}

/**
 * Status of the audio processor
 */
export interface AudioProcessorStatus {
  /** Current state of the processor */
  state: 'idle' | 'loading' | 'ready' | 'processing' | 'error';
  
  /** Whether the processor is actively processing audio */
  isProcessing: boolean;
  
  /** Whether the audio worklet is loaded and ready */
  isReady: boolean;
  
  /** Current audio level (0.0 to 1.0) */
  audioLevel: number;
  
  /** Total number of chunks processed */
  chunksProcessed: number;
  
  /** Error message if in error state */
  error?: string;
  
  /** Audio context sample rate */
  contextSampleRate?: number;
  
  /** Configured output sample rate */
  outputSampleRate: number;
}

/**
 * Audio worklet message types
 */
export interface WorkletMessage {
  type: string;
  [key: string]: unknown;
}

/**
 * Audio chunk message from worklet
 */
export interface AudioChunkMessage extends WorkletMessage {
  type: 'audio_chunk';
  audioBuffer: ArrayBuffer;
  audioLevel: number;
  sampleCount: number;
  timestamp: number;
  sampleRate: number;
}

/**
 * Status message from worklet
 */
export interface WorkletStatusMessage extends WorkletMessage {
  type: 'ready' | 'started' | 'stopped' | 'error';
  message?: string;
}

/**
 * Control message to worklet
 */
export interface WorkletControlMessage extends WorkletMessage {
  type: 'start' | 'stop' | 'configure';
  bufferSize?: number;
}

/**
 * Audio processor event types
 */
export interface AudioProcessorEvents {
  /** Emitted when a new audio chunk is ready */
  audioChunk: (chunk: AudioChunkData) => void;
  
  /** Emitted when the processor status changes */
  statusChange: (status: AudioProcessorStatus) => void;
  
  /** Emitted when an error occurs */
  error: (error: Error) => void;
  
  /** Emitted when audio level changes */
  levelChange: (level: number) => void;
}

/**
 * Audio processor error types
 */
export enum AudioProcessorErrorCode {
  /** Failed to create or access AudioContext */
  AUDIO_CONTEXT_ERROR = 'AUDIO_CONTEXT_ERROR',
  
  /** Failed to load audio worklet */
  WORKLET_LOAD_ERROR = 'WORKLET_LOAD_ERROR',
  
  /** Failed to access microphone */
  MICROPHONE_ACCESS_ERROR = 'MICROPHONE_ACCESS_ERROR',
  
  /** Audio processing error */
  PROCESSING_ERROR = 'PROCESSING_ERROR',
  
  /** Invalid configuration */
  INVALID_CONFIG = 'INVALID_CONFIG',
  
  /** Not supported by browser */
  NOT_SUPPORTED = 'NOT_SUPPORTED'
}

/**
 * Custom error class for audio processor errors
 */
export class AudioProcessorError extends Error {
  constructor(
    message: string,
    public code: AudioProcessorErrorCode,
    public details?: unknown
  ) {
    super(message);
    this.name = 'AudioProcessorError';
  }
}

/**
 * Type guard to check if a message is an audio chunk message
 */
export function isAudioChunkMessage(message: WorkletMessage): message is AudioChunkMessage {
  return message.type === 'audio_chunk' && 'audioBuffer' in message;
}

/**
 * Type guard to check if a message is a status message
 */
export function isWorkletStatusMessage(message: WorkletMessage): message is WorkletStatusMessage {
  return ['ready', 'started', 'stopped', 'error'].includes(message.type);
}

/**
 * Default audio processor configuration
 */
export const DEFAULT_AUDIO_CONFIG: Required<AudioProcessorConfig> = {
  sampleRate: 24000,
  channelCount: 1,
  bufferSize: 2048,
  workletPath: '/worklets/audio-processor.worklet.js',
  debug: false
};

/**
 * AudioService state types
 */
export type AudioServiceState = 
  | 'idle' 
  | 'initializing' 
  | 'ready' 
  | 'recording' 
  | 'failed' 
  | 'permission-denied';

/**
 * AudioService status interface
 */
export interface AudioServiceStatus {
  /** Current state of the service */
  state: AudioServiceState;
  
  /** Whether the service is currently recording */
  isRecording: boolean;
  
  /** Current audio level (0.0 to 1.0) */
  audioLevel: number;
  
  /** Total number of frames processed */
  frameCount: number;
  
  /** Error message if in error state */
  error?: string;
  
  /** Device information */
  deviceId?: string;
  
  /** Sample rate in Hz */
  sampleRate: number;
  
  /** Number of audio channels */
  channelCount: number;
}

/**
 * AudioService event types
 */
export interface AudioServiceEvents {
  /** Emitted when a new audio chunk is ready */
  audioChunk: (chunk: AudioChunkData) => void;
  
  /** Emitted when the service status changes */
  statusChange: (status: AudioServiceStatus) => void;
  
  /** Emitted when an error occurs */
  error: (error: Error) => void;
  
  /** Emitted when audio level changes */
  levelChange: (level: number) => void;
}

/**
 * Configuration for the AudioAgentCBridge
 */
export interface AudioAgentCBridgeConfig {
  /** Whether to respect turn state from TurnManager (default: true) */
  respectTurnState?: boolean;
  
  /** Enable debug logging for audio chunks (default: false) */
  logAudioChunks?: boolean;
  
  /** Enable general debug logging (default: false) */
  debug?: boolean;
}

/**
 * Status of the AudioAgentCBridge
 */
export interface AudioAgentCBridgeStatus {
  /** Whether the bridge is currently streaming audio to the client */
  isStreaming: boolean;
  
  /** Whether a RealtimeClient is connected */
  isConnected: boolean;
  
  /** Whether the user currently has the turn to send input */
  userHasTurn: boolean;
  
  /** Total number of chunks streamed to the server */
  chunksStreamed: number;
  
  /** Total number of chunks suppressed due to turn state */
  chunksSuppressed: number;
  
  /** Whether turn state is being respected */
  respectingTurnState: boolean;
  
  /** Current client connection state */
  clientState?: 'connected' | 'disconnected' | 'connecting';
}

/**
 * Voice model configuration for audio output
 */
export interface VoiceModel {
  /** Voice identifier (e.g., 'openai_tts_nova', 'avatar', 'none') */
  voice_id: string;
  
  /** Audio format (e.g., 'pcm16') */
  format: string;
  
  /** Vendor of the voice model */
  vendor?: string;
  
  /** Description of the voice */
  description?: string;
  
  /** Sample rate in Hz (typically 16000 for PCM16) */
  sampleRate?: number;
}

/**
 * Status of the AudioOutputService
 */
export interface AudioOutputStatus {
  /** Whether audio is currently playing */
  isPlaying: boolean;
  
  /** Whether the service is enabled for playback */
  isEnabled: boolean;
  
  /** Total number of audio chunks received */
  chunksReceived: number;
  
  /** Total number of audio chunks played */
  chunksPlayed: number;
  
  /** Total number of audio chunks skipped (avatar/none mode) */
  chunksSkipped: number;
  
  /** Current length of the playback queue */
  queueLength: number;
  
  /** Current volume level (0.0 to 1.0) */
  volume: number;
  
  /** Current voice model */
  voiceModel: VoiceModel | null;
  
  /** Whether playback is being skipped due to voice mode */
  skipPlayback: boolean;
}

/**
 * Combined audio status for the entire audio system
 */
export interface AudioStatus {
  // Input status
  /** Whether the service is currently recording */
  isRecording: boolean;
  
  /** Whether the service is currently streaming to server */
  isStreaming: boolean;
  
  /** Whether audio is being processed */
  isProcessing: boolean;
  
  /** Whether microphone permission has been granted */
  hasPermission: boolean;
  
  /** Current audio input level (0.0 to 1.0) */
  currentLevel: number;
  
  /** Average audio input level (0.0 to 1.0) */
  averageLevel: number;
  
  // Output status
  /** Whether audio is currently playing */
  isPlaying: boolean;
  
  /** Current size of the audio output buffer */
  bufferSize: number;
  
  /** Current volume level (0.0 to 1.0) */
  volume: number;
  
  // System status
  /** Whether the audio system is enabled */
  isAudioEnabled: boolean;
  
  /** Whether audio input is enabled */
  isInputEnabled: boolean;
  
  /** Whether audio output is enabled */
  isOutputEnabled: boolean;
}
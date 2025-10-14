/**
 * Configuration types and defaults for the Realtime Client
 */

import type { AuthManager } from '../auth';

/**
 * Connection state enum representing the WebSocket connection lifecycle
 */
export enum ConnectionState {
  /** Not connected to the server */
  DISCONNECTED = 'DISCONNECTED',
  /** Attempting to establish connection */
  CONNECTING = 'CONNECTING',
  /** Successfully connected to the server */
  CONNECTED = 'CONNECTED',
  /** Connection lost, attempting to reconnect */
  RECONNECTING = 'RECONNECTING'
}

/**
 * Configuration for reconnection behavior
 */
export interface ReconnectionConfig {
  /** Whether to automatically reconnect on disconnect */
  enabled: boolean;
  /** Initial delay before first reconnection attempt (ms) */
  initialDelay: number;
  /** Maximum delay between reconnection attempts (ms) */
  maxDelay: number;
  /** Multiplier for exponential backoff */
  backoffMultiplier: number;
  /** Maximum number of reconnection attempts (0 = unlimited) */
  maxAttempts: number;
  /** Jitter factor to randomize delays (0-1) */
  jitterFactor: number;
}

/**
 * Configuration for audio functionality
 */
export interface AudioConfig {
  /** Enable audio input (microphone) */
  enableInput?: boolean;
  /** Enable audio output (speakers) */
  enableOutput?: boolean;
  /** Respect turn state for audio input */
  respectTurnState?: boolean;
  /** Log audio chunks for debugging */
  logAudioChunks?: boolean;
  /** Audio sample rate (default: 24000) */
  sampleRate?: number;
  /** Audio chunk size in samples (default: 4800) */
  chunkSize?: number;
  /** Initial playback volume (0-1, default: 1.0) */
  initialVolume?: number;
  /** Enable automatic gain control */
  autoGainControl?: boolean;
  /** Enable echo cancellation */
  echoCancellation?: boolean;
  /** Enable noise suppression */
  noiseSuppression?: boolean;
}

/**
 * Configuration for the Realtime Client
 */
export interface RealtimeClientConfig {
  /** WebSocket API URL (e.g., wss://api.example.com/rt/ws) */
  apiUrl: string;
  
  /** JWT authentication token (optional if using AuthManager) */
  authToken?: string;
  
  /** Optional AuthManager instance for automatic token management */
  authManager?: AuthManager;
  
  /** UI Session ID for WebSocket reconnection (identifies client instance) */
  uiSessionId?: string;
  
  /** Enable automatic reconnection on disconnect */
  autoReconnect?: boolean;
  
  /** Detailed reconnection configuration */
  reconnection?: Partial<ReconnectionConfig>;
  
  /** WebSocket connection timeout (ms) */
  connectionTimeout?: number;
  
  /** Ping interval to keep connection alive (ms) */
  pingInterval?: number;
  
  /** Pong timeout - close connection if no pong received (ms) */
  pongTimeout?: number;
  
  /** Maximum size for incoming messages (bytes) */
  maxMessageSize?: number;
  
  /** Enable debug logging */
  debug?: boolean;
  
  /** Custom headers for WebSocket connection */
  headers?: Record<string, string>;
  
  /** WebSocket protocols to use */
  protocols?: string[];
  
  /** Binary type for WebSocket ('blob' or 'arraybuffer') */
  binaryType?: 'blob' | 'arraybuffer';
  
  /** Enable turn management (default: true) */
  enableTurnManager?: boolean;
  
  /** Enable audio functionality */
  enableAudio?: boolean;
  
  /** Audio configuration */
  audioConfig?: AudioConfig;
  
  /** Maximum file upload size in bytes (default: 10MB) */
  maxUploadSize?: number;
  
  /** Allowed file MIME types for upload (default: undefined = allow all) */
  allowedMimeTypes?: string[];
  
  /** Maximum number of files per message (default: 10) */
  maxFilesPerMessage?: number;
}

/**
 * Default reconnection configuration
 */
export const defaultReconnectionConfig: ReconnectionConfig = {
  enabled: true,
  initialDelay: 1000,      // 1 second
  maxDelay: 30000,         // 30 seconds
  backoffMultiplier: 1.5,  // 1.5x increase each attempt
  maxAttempts: 0,          // Unlimited attempts
  jitterFactor: 0.3        // 30% jitter
};

/**
 * Default audio configuration
 */
export const defaultAudioConfig: AudioConfig = {
  enableInput: true,
  enableOutput: true,
  respectTurnState: true,
  logAudioChunks: false,
  sampleRate: 24000,
  chunkSize: 4800,
  initialVolume: 1.0,
  autoGainControl: true,
  echoCancellation: true,
  noiseSuppression: true
};

/**
 * Default client configuration
 */
export const defaultConfig: Partial<RealtimeClientConfig> = {
  autoReconnect: true,
  reconnection: defaultReconnectionConfig,
  connectionTimeout: 10000,  // 10 seconds
  pingInterval: 30000,       // 30 seconds
  pongTimeout: 10000,        // 10 seconds
  maxMessageSize: 10 * 1024 * 1024, // 10MB
  debug: false,
  binaryType: 'arraybuffer',
  enableTurnManager: true,    // Enable turn management by default
  enableAudio: false,         // Audio disabled by default (requires user opt-in)
  audioConfig: defaultAudioConfig,
  maxUploadSize: 10 * 1024 * 1024, // 10MB
  allowedMimeTypes: undefined, // Allow all
  maxFilesPerMessage: 10
};

/**
 * Merge user config with defaults
 */
export function mergeConfig(
  userConfig: RealtimeClientConfig
): Required<Omit<RealtimeClientConfig, 'uiSessionId' | 'headers' | 'protocols' | 'authToken' | 'authManager'>> & 
  Pick<RealtimeClientConfig, 'uiSessionId' | 'headers' | 'protocols' | 'authToken' | 'authManager'> {
  
  const config = {
    ...defaultConfig,
    ...userConfig
  } as RealtimeClientConfig;

  // Merge reconnection config - ensure it's always defined
  config.reconnection = {
    ...defaultReconnectionConfig,
    ...(userConfig.reconnection || {})
  };
  
  // Merge audio config if audio is enabled
  if (config.enableAudio) {
    config.audioConfig = {
      ...defaultAudioConfig,
      ...(userConfig.audioConfig || {})
    };
  }

  // Ensure required fields are present
  if (!config.apiUrl) {
    throw new Error('apiUrl is required in RealtimeClientConfig');
  }
  
  // Note: Auth validation moved to connect() method to allow tests to create
  // clients and test auth validation behavior

  return config as Required<Omit<RealtimeClientConfig, 'uiSessionId' | 'headers' | 'protocols' | 'authToken' | 'authManager'>> & 
    Pick<RealtimeClientConfig, 'uiSessionId' | 'headers' | 'protocols' | 'authToken' | 'authManager'>;
}
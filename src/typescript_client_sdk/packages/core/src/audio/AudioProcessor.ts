/**
 * AudioProcessor - Manages audio worklet for PCM16 conversion
 * 
 * This class provides a clean API for managing audio processing in a separate thread
 * using Web Audio API's AudioWorklet. It handles microphone access, PCM16 conversion,
 * and chunk generation for transmission to the Agent C Realtime API.
 */

import {
  AudioProcessorConfig,
  AudioProcessorStatus,
  AudioChunkData,
  AudioProcessorError,
  AudioProcessorErrorCode,
  AudioChunkMessage,
  WorkletStatusMessage,
  WorkletMessage,
  DEFAULT_AUDIO_CONFIG,
  isAudioChunkMessage,
  isWorkletStatusMessage
} from './types';

export class AudioProcessor {
  private config: Required<AudioProcessorConfig>;
  private audioContext: AudioContext | null = null;
  private sourceNode: MediaStreamAudioSourceNode | null = null;
  private workletNode: AudioWorkletNode | null = null;
  private mediaStream: MediaStream | null = null;
  
  private status: AudioProcessorStatus = {
    state: 'idle',
    isProcessing: false,
    isReady: false,
    audioLevel: 0,
    chunksProcessed: 0,
    outputSampleRate: DEFAULT_AUDIO_CONFIG.sampleRate
  };
  
  private chunkCounter = 0;
  private listeners = new Map<string, Set<Function>>();
  
  constructor(config?: AudioProcessorConfig) {
    this.config = { ...DEFAULT_AUDIO_CONFIG, ...config };
    
    if (this.config.debug) {
      // console.log('[AudioProcessor] Initialized with config:', this.config);
    }
  }
  
  /**
   * Initialize the audio processor and load the worklet
   */
  async initialize(): Promise<void> {
    try {
      if (this.status.state === 'ready') {
        return; // Already initialized
      }
      
      this.updateStatus({ state: 'loading' });
      
      // Check browser support
      if (!('AudioContext' in window || 'webkitAudioContext' in window)) {
        throw new AudioProcessorError(
          'Web Audio API is not supported in this browser',
          AudioProcessorErrorCode.NOT_SUPPORTED
        );
      }
      
      // Request microphone access FIRST to trigger permission prompt
      await this.requestMicrophoneAccess();
      
      // Create audio context after we have microphone permission
      const AudioContextClass = window.AudioContext || (window as Window & { webkitAudioContext?: typeof AudioContext }).webkitAudioContext;
      if (!AudioContextClass) {
        throw new Error('AudioContext not supported');
      }
      // Create AudioContext without forcing sample rate to allow native rate
      // The AudioWorklet will handle resampling to target rate
      this.audioContext = new AudioContextClass();
      
      // Check AudioWorklet support AFTER creating the AudioContext
      if (!this.audioContext.audioWorklet) {
        throw new AudioProcessorError(
          'AudioWorklet is not supported in this browser',
          AudioProcessorErrorCode.NOT_SUPPORTED
        );
      }
      
      // Load the audio worklet module
      await this.loadWorklet();
      
      // Set up audio nodes
      await this.setupAudioNodes();
      
      this.updateStatus({ 
        state: 'ready',
        isReady: true,
        contextSampleRate: this.audioContext?.sampleRate
      });
      
      if (this.config.debug) {
        console.warn('[AudioProcessor] Audio system initialized:', {
          audioContextSampleRate: this.audioContext?.sampleRate,
          targetOutputRate: this.config.sampleRate,
          resamplingNeeded: this.audioContext?.sampleRate !== this.config.sampleRate
        });
      }
      
      if (this.config.debug) {
        console.warn('[AudioProcessor] Initialization complete');
      }
    } catch (error) {
      this.handleError(error as Error);
      throw error;
    }
  }
  
  /**
   * Load the audio worklet module
   */
  private async loadWorklet(): Promise<void> {
    if (!this.audioContext) {
      throw new AudioProcessorError(
        'Audio context not initialized',
        AudioProcessorErrorCode.AUDIO_CONTEXT_ERROR
      );
    }
    
    try {
      // Check if audioWorklet is available
      if (!this.audioContext.audioWorklet) {
        throw new AudioProcessorError(
          'AudioWorklet API is not available',
          AudioProcessorErrorCode.NOT_SUPPORTED
        );
      }
      
      if (this.config.debug) {
        console.warn('[AudioProcessor] Loading worklet from:', this.config.workletPath);
        console.warn('[AudioProcessor] AudioContext state:', this.audioContext.state);
        console.warn('[AudioProcessor] AudioContext sample rate:', this.audioContext.sampleRate);
      }
      
      await this.audioContext.audioWorklet.addModule(this.config.workletPath);
      
      if (this.config.debug) {
        console.warn('[AudioProcessor] Worklet loaded successfully from:', this.config.workletPath);
      }
    } catch (error) {
      const errorDetails = error instanceof Error ? error.message : String(error);
      const detailedMessage = `Failed to load audio worklet from ${this.config.workletPath}. Error: ${errorDetails}. ` +
        `Please ensure the worklet file exists and is accessible. AudioContext state: ${this.audioContext?.state}, ` +
        `Sample rate: ${this.audioContext?.sampleRate}`;
      
      console.error('[AudioProcessor] Worklet load error:', error);
      console.error('[AudioProcessor] Detailed error:', detailedMessage);
      
      throw new AudioProcessorError(
        detailedMessage,
        AudioProcessorErrorCode.WORKLET_LOAD_ERROR,
        error
      );
    }
  }
  
  /**
   * Request microphone access
   */
  private async requestMicrophoneAccess(): Promise<void> {
    try {
      // Request microphone without forcing sample rate
      // Browsers often ignore the sampleRate constraint anyway
      // The AudioWorklet will handle resampling from native rate to target rate
      this.mediaStream = await navigator.mediaDevices.getUserMedia({
        audio: {
          channelCount: this.config.channelCount,
          // Don't specify sampleRate - let browser use native rate
          echoCancellation: true,
          noiseSuppression: true,
          autoGainControl: true
        }
      });
      
      if (this.config.debug) {
        const tracks = this.mediaStream.getAudioTracks();
        const settings = tracks[0]?.getSettings();
        console.warn('[AudioProcessor] Microphone access granted:', {
          tracks: tracks.length,
          settings: settings,
          requestedSampleRate: this.config.sampleRate,
          actualSampleRate: settings?.sampleRate || 'unknown'
        });
      }
    } catch (error) {
      throw new AudioProcessorError(
        'Failed to access microphone',
        AudioProcessorErrorCode.MICROPHONE_ACCESS_ERROR,
        error
      );
    }
  }
  
  /**
   * Set up audio nodes and connections
   */
  private async setupAudioNodes(): Promise<void> {
    if (!this.audioContext || !this.mediaStream) {
      throw new AudioProcessorError(
        'Audio context or media stream not available',
        AudioProcessorErrorCode.AUDIO_CONTEXT_ERROR
      );
    }
    
    // Create source node from microphone
    this.sourceNode = this.audioContext.createMediaStreamSource(this.mediaStream);
    
    // Create worklet node
    this.workletNode = new AudioWorkletNode(this.audioContext, 'pcm16-processor', {
      numberOfInputs: 1,
      numberOfOutputs: 1,
      channelCount: this.config.channelCount
    });
    
    // Set up message handling from worklet
    this.workletNode.port.onmessage = (event) => {
      this.handleWorkletMessage(event.data);
    };
    
    // Configure the worklet with both native and target sample rates
    this.workletNode.port.postMessage({
      type: 'configure',
      bufferSize: this.config.bufferSize,
      nativeSampleRate: this.audioContext.sampleRate,  // Browser's native rate (usually 48000)
      targetSampleRate: this.config.sampleRate          // Our target rate (16000)
    });
    
    // Connect the audio graph: microphone -> worklet -> destination (optional monitoring)
    this.sourceNode.connect(this.workletNode);
    // Note: Not connecting to destination to avoid feedback
    
    if (this.config.debug) {
      // console.log('[AudioProcessor] Audio nodes connected');
    }
  }
  
  /**
   * Handle messages from the worklet
   */
  private handleWorkletMessage(message: WorkletMessage): void {
    if (isAudioChunkMessage(message)) {
      this.handleAudioChunk(message);
    } else if (isWorkletStatusMessage(message)) {
      this.handleWorkletStatus(message);
    }
  }
  
  /**
   * Handle audio chunk from worklet
   */
  private handleAudioChunk(message: AudioChunkMessage): void {
    const chunk: AudioChunkData = {
      content: message.audioBuffer,
      content_type: 'audio/L16',
      audio_level: message.audioLevel,
      frame_count: ++this.chunkCounter,
      timestamp: Date.now(),
      sample_rate: message.sampleRate,
      sample_count: message.sampleCount
    };
    
    this.updateStatus({
      audioLevel: message.audioLevel,
      chunksProcessed: this.chunkCounter
    });
    
    // Emit audio chunk event
    this.emit('audioChunk', chunk);
    
    // Emit level change event
    this.emit('levelChange', message.audioLevel);
  }
  
  /**
   * Handle status message from worklet
   */
  private handleWorkletStatus(message: WorkletStatusMessage): void {
    if (this.config.debug) {
      // console.log('[AudioProcessor] Worklet status:', message.type, message.message);
    }
    
    switch (message.type) {
      case 'ready':
        // Worklet is ready
        break;
      case 'started':
        this.updateStatus({ isProcessing: true });
        break;
      case 'stopped':
        this.updateStatus({ isProcessing: false });
        break;
      case 'error':
        this.handleError(new Error(message.message || 'Worklet error'));
        break;
    }
  }
  
  /**
   * Start processing audio
   */
  async startProcessing(): Promise<void> {
    if (this.status.state !== 'ready') {
      await this.initialize();
    }
    
    if (this.status.isProcessing) {
      return; // Already processing
    }
    
    if (!this.workletNode) {
      throw new AudioProcessorError(
        'Worklet node not initialized',
        AudioProcessorErrorCode.PROCESSING_ERROR
      );
    }
    
    // Resume audio context if suspended
    if (this.audioContext?.state === 'suspended') {
      await this.audioContext.resume();
    }
    
    // Start the worklet processing
    this.workletNode.port.postMessage({ type: 'start' });
    
    this.updateStatus({ 
      state: 'processing',
      isProcessing: true 
    });
    
    if (this.config.debug) {
      console.warn('[AudioProcessor] Started processing');
    }
  }
  
  /**
   * Stop processing audio
   */
  stopProcessing(): void {
    if (!this.status.isProcessing) {
      return; // Not processing
    }
    
    if (this.workletNode) {
      this.workletNode.port.postMessage({ type: 'stop' });
    }
    
    this.updateStatus({ 
      state: 'ready',
      isProcessing: false,
      audioLevel: 0
    });
    
    if (this.config.debug) {
      console.warn('[AudioProcessor] Stopped processing');
    }
  }
  
  /**
   * Clean up and release resources
   */
  async cleanup(): Promise<void> {
    this.stopProcessing();
    
    // Disconnect audio nodes
    if (this.sourceNode) {
      this.sourceNode.disconnect();
      this.sourceNode = null;
    }
    
    if (this.workletNode) {
      this.workletNode.disconnect();
      this.workletNode.port.close();
      this.workletNode = null;
    }
    
    // Stop media stream tracks
    if (this.mediaStream) {
      this.mediaStream.getTracks().forEach(track => track.stop());
      this.mediaStream = null;
    }
    
    // Close audio context
    if (this.audioContext && this.audioContext.state !== 'closed') {
      await this.audioContext.close();
      this.audioContext = null;
    }
    
    // Reset status
    this.status = {
      state: 'idle',
      isProcessing: false,
      isReady: false,
      audioLevel: 0,
      chunksProcessed: 0,
      outputSampleRate: this.config.sampleRate
    };
    
    // Clear listeners
    this.listeners.clear();
    
    if (this.config.debug) {
      // console.log('[AudioProcessor] Cleanup complete');
    }
  }
  
  /**
   * Get current status
   */
  getStatus(): AudioProcessorStatus {
    return { ...this.status };
  }
  
  /**
   * Update status and emit change event
   */
  private updateStatus(partial: Partial<AudioProcessorStatus>): void {
    this.status = { ...this.status, ...partial };
    this.emit('statusChange', this.status);
  }
  
  /**
   * Handle errors
   */
  private handleError(error: Error): void {
    const audioError = error instanceof AudioProcessorError
      ? error
      : new AudioProcessorError(
          error.message,
          AudioProcessorErrorCode.PROCESSING_ERROR,
          error
        );
    
    this.updateStatus({ 
      state: 'error',
      error: audioError.message
    });
    
    this.emit('error', audioError);
    
    if (this.config.debug) {
      console.error('[AudioProcessor] Error:', audioError);
    }
  }
  
  /**
   * Event emitter implementation
   */
  on(event: string, listener: Function): () => void {
    if (!this.listeners.has(event)) {
      this.listeners.set(event, new Set());
    }
    
    this.listeners.get(event)!.add(listener);
    
    // Return unsubscribe function
    return () => {
      this.listeners.get(event)?.delete(listener);
    };
  }
  
  private emit(event: string, ...args: unknown[]): void {
    const listeners = this.listeners.get(event);
    if (listeners) {
      listeners.forEach(listener => {
        try {
          listener(...args);
        } catch (error) {
          console.error(`[AudioProcessor] Error in event listener for ${event}:`, error);
        }
      });
    }
  }
}
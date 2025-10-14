/**
 * Singleton service for TTS audio playback from server
 * Handles binary audio chunks, voice model awareness, and smooth playback
 */

import { AudioOutputStatus, VoiceModel } from './types';

/**
 * AudioOutputService singleton for playing TTS audio from the server
 * Handles PCM16 audio format at 16kHz sample rate
 */
export class AudioOutputService {
  private static instance: AudioOutputService | null = null;
  
  // Audio context and playback state
  private audioContext: AudioContext | null = null;
  private isInitialized: boolean = false;
  private gainNode: GainNode | null = null;
  
  // Playback queue and state
  private audioQueue: ArrayBuffer[] = [];
  private isPlaying: boolean = false;
  private currentSource: AudioBufferSourceNode | null = null;
  private nextPlayTime: number = 0;
  
  // Configuration
  private enabled: boolean = true;
  private volume: number = 1.0;
  private voiceModel: VoiceModel | null = null;
  
  // Statistics
  private chunksReceived: number = 0;
  private chunksPlayed: number = 0;
  private chunksSkipped: number = 0;
  
  // Status change listeners
  private statusListeners: Array<(status: AudioOutputStatus) => void> = [];
  
  // Constants
  private readonly SAMPLE_RATE = 16000; // 16kHz for PCM16
  private readonly CHUNK_OVERLAP_MS = 10; // Small overlap to prevent gaps
  
  /**
   * Private constructor to enforce singleton pattern
   */
  private constructor() {
    // Initialize on first use
  }
  
  /**
   * Get the singleton instance of AudioOutputService
   */
  public static getInstance(): AudioOutputService {
    if (!AudioOutputService.instance) {
      AudioOutputService.instance = new AudioOutputService();
    }
    return AudioOutputService.instance;
  }
  
  /**
   * Initialize the audio context and components
   */
  private async initialize(): Promise<void> {
    if (this.isInitialized) {
      return;
    }
    
    try {
      // Create audio context
      const AudioContextClass = window.AudioContext || (window as Window & { webkitAudioContext?: typeof AudioContext }).webkitAudioContext;
      if (!AudioContextClass) {
        throw new Error('AudioContext not supported');
      }
      this.audioContext = new AudioContextClass({
        sampleRate: this.SAMPLE_RATE
      });
      
      // Create gain node for volume control
      this.gainNode = this.audioContext.createGain();
      this.gainNode.gain.value = this.volume;
      this.gainNode.connect(this.audioContext.destination);
      
      // Reset playback time
      this.nextPlayTime = 0;
      
      this.isInitialized = true;
      console.warn('[AudioOutputService] Initialized with sample rate:', this.SAMPLE_RATE);
    } catch (error) {
      console.error('[AudioOutputService] Failed to initialize:', error);
      throw error;
    }
  }
  
  /**
   * Queue and play an audio chunk from the server
   */
  public async playAudioChunk(audioData: ArrayBuffer): Promise<void> {
    this.chunksReceived++;
    
    // Check if we should skip playback based on voice model
    if (this.shouldSkipPlayback()) {
      this.chunksSkipped++;
      this.notifyStatusChange();
      return;
    }
    
    // Initialize if needed
    if (!this.isInitialized) {
      await this.initialize();
    }
    
    // Add to queue
    this.audioQueue.push(audioData);
    this.notifyStatusChange();
    
    // Start processing queue if not already playing
    if (!this.isPlaying) {
      this.processQueue();
    }
  }
  
  /**
   * Process the audio queue and play chunks
   */
  private async processQueue(): Promise<void> {
    if (!this.audioContext || !this.gainNode) {
      console.warn('[AudioOutputService] Not initialized, cannot process queue');
      return;
    }
    
    this.isPlaying = true;
    this.notifyStatusChange();
    
    while (this.audioQueue.length > 0 && this.enabled) {
      const audioData = this.audioQueue.shift();
      if (!audioData) continue;
      
      try {
        // Convert PCM16 to AudioBuffer
        const audioBuffer = await this.pcm16ToAudioBuffer(audioData);
        
        // Create source node
        const source = this.audioContext.createBufferSource();
        source.buffer = audioBuffer;
        source.connect(this.gainNode);
        
        // Calculate play time
        const currentTime = this.audioContext.currentTime;
        const playTime = Math.max(currentTime, this.nextPlayTime);
        
        // Update next play time with small overlap to prevent gaps
        const duration = audioBuffer.duration;
        const overlapSeconds = this.CHUNK_OVERLAP_MS / 1000;
        this.nextPlayTime = playTime + duration - overlapSeconds;
        
        // Store current source for stopping
        this.currentSource = source;
        
        // Start playback
        source.start(playTime);
        
        // Track completion
        source.onended = () => {
          this.chunksPlayed++;
          if (this.currentSource === source) {
            this.currentSource = null;
          }
          this.notifyStatusChange();
        };
        
      } catch (error) {
        console.error('[AudioOutputService] Error playing chunk:', error);
      }
    }
    
    // Wait for current playback to finish
    if (this.currentSource) {
      await new Promise<void>((resolve) => {
        if (this.currentSource) {
          const source = this.currentSource;
          const onEnded = source.onended;
          source.onended = () => {
            if (onEnded && typeof onEnded === 'function') {
              onEnded.call(source, new Event('ended'));
            }
            resolve();
          };
        } else {
          resolve();
        }
      });
    }
    
    this.isPlaying = false;
    this.notifyStatusChange();
  }
  
  /**
   * Convert PCM16 ArrayBuffer to AudioBuffer
   */
  private async pcm16ToAudioBuffer(pcm16Data: ArrayBuffer): Promise<AudioBuffer> {
    if (!this.audioContext) {
      throw new Error('Audio context not initialized');
    }
    
    // Create Int16Array view of the PCM16 data
    const int16Array = new Int16Array(pcm16Data);
    const length = int16Array.length;
    
    // Create AudioBuffer
    const audioBuffer = this.audioContext.createBuffer(
      1, // mono
      length,
      this.SAMPLE_RATE
    );
    
    // Get the channel data and convert Int16 to Float32
    const channelData = audioBuffer.getChannelData(0);
    for (let i = 0; i < length; i++) {
      // Convert from Int16 range (-32768 to 32767) to Float32 range (-1 to 1)
      // @ts-expect-error - getChannelData always returns Float32Array
      channelData[i] = int16Array[i] / 32768.0;
    }
    
    return audioBuffer;
  }
  
  /**
   * Check if playback should be skipped based on voice model
   */
  private shouldSkipPlayback(): boolean {
    if (!this.enabled) {
      return true;
    }
    
    if (!this.voiceModel) {
      return false; // Play by default if no voice model set
    }
    
    // Skip playback for avatar mode (HeyGen handles audio)
    if (this.voiceModel.voice_id === 'avatar') {
      return true;
    }
    
    // Skip playback for text-only mode
    if (this.voiceModel.voice_id === 'none') {
      return true;
    }
    
    return false;
  }
  
  /**
   * Stop current playback and clear the queue
   */
  public stopPlayback(): void {
    // Stop current source
    if (this.currentSource) {
      try {
        this.currentSource.stop();
        this.currentSource.disconnect();
        this.currentSource = null;
      } catch (_error) {
        // Ignore errors when stopping
      }
    }
    
    // Clear queue
    this.audioQueue = [];
    
    // Reset play time
    this.nextPlayTime = 0;
    
    this.isPlaying = false;
    this.notifyStatusChange();
    
    console.warn('[AudioOutputService] Playback stopped');
  }
  
  /**
   * Clear queued audio without stopping current playback
   */
  public clearBuffers(): void {
    this.audioQueue = [];
    this.notifyStatusChange();
    // console.log('[AudioOutputService] Buffers cleared');
  }
  
  /**
   * Set the playback volume (0.0 to 1.0)
   */
  public setVolume(volume: number): void {
    this.volume = Math.max(0, Math.min(1, volume));
    
    if (this.gainNode) {
      this.gainNode.gain.value = this.volume;
    }
    
    this.notifyStatusChange();
    // console.log('[AudioOutputService] Volume set to:', this.volume);
  }
  
  /**
   * Set the current voice model
   */
  public setVoiceModel(voiceModel: VoiceModel | null): void {
    const previousModel = this.voiceModel;
    this.voiceModel = voiceModel;
    
    // If switching to/from avatar or none mode, clear the queue
    if (voiceModel && (voiceModel.voice_id === 'avatar' || voiceModel.voice_id === 'none')) {
      this.clearBuffers();
      this.stopPlayback();
    } else if (previousModel && (previousModel.voice_id === 'avatar' || previousModel.voice_id === 'none')) {
      // Switching from avatar/none to normal voice, reset
      this.nextPlayTime = 0;
    }
    
    this.notifyStatusChange();
    console.warn('[AudioOutputService] Voice model set to:', voiceModel?.voice_id || 'null');
  }
  
  /**
   * Enable or disable playback
   */
  public setEnabled(enabled: boolean): void {
    this.enabled = enabled;
    
    if (!enabled) {
      this.stopPlayback();
    }
    
    this.notifyStatusChange();
    // console.log('[AudioOutputService] Enabled:', enabled);
  }
  
  /**
   * Get the current status
   */
  public getStatus(): AudioOutputStatus {
    return {
      isPlaying: this.isPlaying,
      isEnabled: this.enabled,
      chunksReceived: this.chunksReceived,
      chunksPlayed: this.chunksPlayed,
      chunksSkipped: this.chunksSkipped,
      queueLength: this.audioQueue.length,
      volume: this.volume,
      voiceModel: this.voiceModel,
      skipPlayback: this.shouldSkipPlayback()
    };
  }
  
  /**
   * Subscribe to status changes
   */
  public onStatusChange(callback: (status: AudioOutputStatus) => void): () => void {
    this.statusListeners.push(callback);
    
    // Return unsubscribe function
    return () => {
      const index = this.statusListeners.indexOf(callback);
      if (index !== -1) {
        this.statusListeners.splice(index, 1);
      }
    };
  }
  
  /**
   * Notify all listeners of status change
   */
  private notifyStatusChange(): void {
    const status = this.getStatus();
    this.statusListeners.forEach(listener => {
      try {
        listener(status);
      } catch (error) {
        console.error('[AudioOutputService] Error in status listener:', error);
      }
    });
  }
  
  /**
   * Reset all statistics
   */
  public resetStats(): void {
    this.chunksReceived = 0;
    this.chunksPlayed = 0;
    this.chunksSkipped = 0;
    this.notifyStatusChange();
  }
  
  /**
   * Cleanup resources
   */
  public async cleanup(): Promise<void> {
    this.stopPlayback();
    
    if (this.gainNode) {
      this.gainNode.disconnect();
      this.gainNode = null;
    }
    
    if (this.audioContext) {
      await this.audioContext.close();
      this.audioContext = null;
    }
    
    this.isInitialized = false;
    this.statusListeners = [];
    
    // console.log('[AudioOutputService] Cleaned up');
  }
}
/**
 * AudioService - Singleton service for microphone access and recording
 * 
 * This service provides a high-level API for managing audio recording,
 * wrapping the AudioProcessor with state management and permission handling.
 * It follows the singleton pattern to ensure only one instance manages
 * the audio resources.
 */

import { AudioProcessor } from './AudioProcessor';
import {
  AudioServiceStatus,
  AudioServiceState,
  AudioChunkData,
  AudioProcessorConfig,
  AudioProcessorError,
  AudioProcessorErrorCode,
  AudioProcessorStatus
} from './types';

export class AudioService {
  private static instance: AudioService | null = null;
  
  private processor: AudioProcessor;
  private status: AudioServiceStatus;
  private frameCounter = 0;
  private listeners = new Map<string, Set<Function>>();
  private processorUnsubscribers: Array<() => void> = [];
  private permissionGranted = false;
  private debug: boolean;
  
  /**
   * Private constructor - use getInstance() instead
   */
  private constructor(config?: AudioProcessorConfig) {
    this.debug = config?.debug || false;
    
    // Initialize processor with config
    this.processor = new AudioProcessor(config);
    
    // Initialize status
    this.status = {
      state: 'idle',
      isRecording: false,
      audioLevel: 0,
      frameCount: 0,
      sampleRate: config?.sampleRate || 16000,
      channelCount: config?.channelCount || 1
    };
    
    // Set up processor event listeners
    this.setupProcessorListeners();
    
    if (this.debug) {
      // Singleton instance initialized
    }
  }
  
  /**
   * Get singleton instance of AudioService
   */
  static getInstance(config?: AudioProcessorConfig): AudioService {
    if (!AudioService.instance) {
      AudioService.instance = new AudioService(config);
    }
    return AudioService.instance;
  }
  
  /**
   * Reset the singleton instance (mainly for testing)
   */
  static resetInstance(): void {
    if (AudioService.instance) {
      AudioService.instance.cleanup();
      AudioService.instance = null;
    }
  }
  
  /**
   * Set up event listeners for the AudioProcessor
   */
  private setupProcessorListeners(): void {
    // Listen to audio chunks from processor
    const unsubChunk = this.processor.on('audioChunk', (chunk: AudioChunkData) => {
      // Update frame count
      this.frameCounter++;
      
      // Forward the chunk with updated frame count
      const serviceChunk = {
        ...chunk,
        frame_count: this.frameCounter
      };
      
      this.emit('audioChunk', serviceChunk);
    });
    this.processorUnsubscribers.push(unsubChunk);
    
    // Listen to processor status changes
    const unsubStatus = this.processor.on('statusChange', (processorStatus: AudioProcessorStatus) => {
      this.handleProcessorStatusChange(processorStatus);
    });
    this.processorUnsubscribers.push(unsubStatus);
    
    // Listen to audio level changes
    const unsubLevel = this.processor.on('levelChange', (level: number) => {
      this.updateStatus({ audioLevel: level });
      this.emit('levelChange', level);
    });
    this.processorUnsubscribers.push(unsubLevel);
    
    // Listen to processor errors
    const unsubError = this.processor.on('error', (error: Error) => {
      this.handleProcessorError(error);
    });
    this.processorUnsubscribers.push(unsubError);
  }
  
  /**
   * Handle processor status changes
   */
  private handleProcessorStatusChange(processorStatus: AudioProcessorStatus): void {
    // Map processor state to service state
    let serviceState: AudioServiceState = this.status.state;
    
    switch (processorStatus.state) {
      case 'idle':
        serviceState = 'idle';
        break;
      case 'loading':
        serviceState = 'initializing';
        break;
      case 'ready':
        serviceState = this.status.isRecording ? 'recording' : 'ready';
        break;
      case 'processing':
        serviceState = 'recording';
        break;
      case 'error':
        // Determine if it's a permission error
        if (processorStatus.error?.includes('microphone') || 
            processorStatus.error?.includes('permission')) {
          serviceState = 'permission-denied';
        } else {
          serviceState = 'failed';
        }
        break;
    }
    
    this.updateStatus({
      state: serviceState,
      audioLevel: processorStatus.audioLevel,
      error: processorStatus.error
    });
  }
  
  /**
   * Handle processor errors
   */
  private handleProcessorError(error: Error): void {
    if (error instanceof AudioProcessorError) {
      if (error.code === AudioProcessorErrorCode.MICROPHONE_ACCESS_ERROR) {
        this.updateStatus({
          state: 'permission-denied',
          error: error.message
        });
        this.permissionGranted = false;
      } else {
        this.updateStatus({
          state: 'failed',
          error: error.message
        });
      }
    } else {
      this.updateStatus({
        state: 'failed',
        error: error.message
      });
    }
    
    this.emit('error', error);
    
    if (this.debug) {
      console.error('[AudioService] Error:', error);
    }
  }
  
  /**
   * Request microphone permission
   */
  async requestPermission(): Promise<boolean> {
    try {
      if (this.permissionGranted) {
        return true;
      }
      
      if (this.debug) {
        // Requesting microphone permission
      }
      
      // Try to get user media to trigger permission prompt
      const stream = await navigator.mediaDevices.getUserMedia({
        audio: {
          channelCount: this.status.channelCount,
          sampleRate: this.status.sampleRate,
          echoCancellation: true,
          noiseSuppression: true,
          autoGainControl: true
        }
      });
      
      // Permission granted, stop the stream immediately
      stream.getTracks().forEach(track => track.stop());
      
      this.permissionGranted = true;
      
      if (this.debug) {
        // Microphone permission granted
      }
      
      return true;
    } catch (error) {
      this.permissionGranted = false;
      
      const errorMessage = error instanceof Error ? error.message : 'Permission denied';
      
      this.updateStatus({
        state: 'permission-denied',
        error: errorMessage
      });
      
      if (this.debug) {
        console.error('[AudioService] Permission denied:', error);
      }
      
      return false;
    }
  }
  
  /**
   * Check if microphone permission has been granted
   */
  hasPermission(): boolean {
    return this.permissionGranted;
  }
  
  /**
   * Start recording audio
   */
  async startRecording(): Promise<void> {
    try {
      if (this.status.isRecording) {
        if (this.debug) {
          // Already recording
        }
        return;
      }
      
      if (this.debug) {
        // Starting recording
      }
      
      this.updateStatus({ state: 'initializing' });
      
      // Initialize processor if needed
      if (this.processor.getStatus().state !== 'ready') {
        await this.processor.initialize();
        this.permissionGranted = true; // If initialization succeeded, we have permission
      }
      
      // Start processing
      await this.processor.startProcessing();
      
      this.updateStatus({
        state: 'recording',
        isRecording: true,
        frameCount: this.frameCounter
      });
      
      if (this.debug) {
        // Recording started
      }
    } catch (error) {
      this.handleProcessorError(error as Error);
      throw error;
    }
  }
  
  /**
   * Stop recording audio
   */
  stopRecording(): void {
    if (!this.status.isRecording) {
      if (this.debug) {
        // Not currently recording
      }
      return;
    }
    
    if (this.debug) {
      // Stopping recording
    }
    
    // Stop processor
    this.processor.stopProcessing();
    
    this.updateStatus({
      state: 'ready',
      isRecording: false,
      audioLevel: 0
    });
    
    if (this.debug) {
      // Recording stopped
    }
  }
  
  /**
   * Get current status
   */
  getStatus(): AudioServiceStatus {
    return { ...this.status };
  }
  
  /**
   * Subscribe to status changes
   * @returns Unsubscribe function
   */
  onStatusChange(callback: (status: AudioServiceStatus) => void): () => void {
    return this.on('statusChange', callback);
  }
  
  /**
   * Subscribe to audio chunks
   * @returns Unsubscribe function
   */
  onAudioChunk(callback: (chunk: AudioChunkData) => void): () => void {
    return this.on('audioChunk', callback);
  }
  
  /**
   * Update status and emit change event
   */
  private updateStatus(partial: Partial<AudioServiceStatus>): void {
    const oldState = this.status.state;
    this.status = { ...this.status, ...partial };
    
    // Always emit status change
    this.emit('statusChange', this.status);
    
    // Log state transitions if debug enabled
    if (this.debug && oldState !== this.status.state) {
      // State transition: ${oldState} -> ${this.status.state}
    }
  }
  
  /**
   * Generic event subscription
   * @returns Unsubscribe function
   */
  private on(event: string, listener: Function): () => void {
    if (!this.listeners.has(event)) {
      this.listeners.set(event, new Set());
    }
    
    this.listeners.get(event)!.add(listener);
    
    // Return unsubscribe function
    return () => {
      this.listeners.get(event)?.delete(listener);
    };
  }
  
  /**
   * Emit an event to all listeners
   */
  private emit(event: string, ...args: unknown[]): void {
    const listeners = this.listeners.get(event);
    if (listeners) {
      listeners.forEach(listener => {
        try {
          listener(...args);
        } catch (error) {
          console.error(`[AudioService] Error in event listener for ${event}:`, error);
        }
      });
    }
  }
  
  /**
   * Clean up and release resources
   */
  private async cleanup(): Promise<void> {
    if (this.debug) {
      // Cleaning up
    }
    
    // Stop recording if active
    this.stopRecording();
    
    // Unsubscribe from processor events
    this.processorUnsubscribers.forEach(unsub => unsub());
    this.processorUnsubscribers = [];
    
    // Clean up processor
    await this.processor.cleanup();
    
    // Clear listeners
    this.listeners.clear();
    
    // Reset state
    this.status = {
      state: 'idle',
      isRecording: false,
      audioLevel: 0,
      frameCount: 0,
      sampleRate: this.status.sampleRate,
      channelCount: this.status.channelCount
    };
    
    this.frameCounter = 0;
    this.permissionGranted = false;
    
    if (this.debug) {
      // Cleanup complete
    }
  }
}
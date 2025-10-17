/**
 * AudioAgentCBridge - Singleton bridge between AudioService and RealtimeClient
 * 
 * This bridge connects the AudioService (which captures audio) to the RealtimeClient
 * (which sends it to the server), while respecting turn management to prevent
 * sending audio when the agent is speaking.
 * 
 * The bridge doesn't create audio or manage the WebSocket - it just connects them
 * with turn awareness for clean separation of concerns.
 */

import { AudioService } from './AudioService';
import { RealtimeClient } from '../client/RealtimeClient';
import { TurnManager } from '../session/TurnManager';
import {
  AudioAgentCBridgeConfig,
  AudioAgentCBridgeStatus,
  AudioChunkData
} from './types';

export class AudioAgentCBridge {
  private static instance: AudioAgentCBridge | null = null;
  
  private config: Required<AudioAgentCBridgeConfig>;
  private client: RealtimeClient | null = null;
  private audioService: AudioService;
  private turnManager: TurnManager | null = null;
  
  // State tracking
  private isStreaming = false;
  private chunksStreamed = 0;
  private chunksSuppressed = 0;
  
  // Subscription management
  private audioChunkUnsubscribe: (() => void) | null = null;
  private turnStateUnsubscribe: (() => void) | null = null;
  private listeners = new Map<string, Set<Function>>();
  
  // Current state cache
  private currentUserHasTurn = false;
  
  /**
   * Private constructor - use getInstance() instead
   */
  private constructor(config?: AudioAgentCBridgeConfig) {
    // Apply default config
    this.config = {
      respectTurnState: config?.respectTurnState ?? true,
      logAudioChunks: config?.logAudioChunks ?? false,
      debug: config?.debug ?? false
    };
    
    // Get AudioService singleton
    this.audioService = AudioService.getInstance();
    
    if (this.config.debug) {
      // console.log('[AudioAgentCBridge] Initialized singleton instance', this.config);
    }
  }
  
  /**
   * Get singleton instance of AudioAgentCBridge
   */
  static getInstance(config?: AudioAgentCBridgeConfig): AudioAgentCBridge {
    if (!AudioAgentCBridge.instance) {
      AudioAgentCBridge.instance = new AudioAgentCBridge(config);
    }
    return AudioAgentCBridge.instance;
  }
  
  /**
   * Reset the singleton instance (mainly for testing)
   */
  static resetInstance(): void {
    if (AudioAgentCBridge.instance) {
      AudioAgentCBridge.instance.cleanup();
      AudioAgentCBridge.instance = null;
    }
  }
  
  /**
   * Set or clear the RealtimeClient reference
   * @param client - RealtimeClient instance or null to disconnect
   */
  setClient(client: RealtimeClient | null): void {
    if (this.config.debug) {
      console.warn('[AudioAgentCBridge] Setting client:', client ? 'connected' : 'disconnected');
    }
    
    // Clean up existing client connections
    if (this.turnManager && this.turnStateUnsubscribe) {
      this.turnStateUnsubscribe();
      this.turnStateUnsubscribe = null;
    }
    
    this.client = client;
    this.turnManager = null;
    this.currentUserHasTurn = false;
    
    if (client) {
      // Get the turn manager from the client
      this.turnManager = client.getTurnManager();
      
      if (this.turnManager) {
        // Create turn state change handler
        const turnStateHandler = ({ canSendInput }: { canSendInput: boolean }) => {
          this.currentUserHasTurn = canSendInput;
          
          if (this.config.debug) {
            // console.log('[AudioAgentCBridge] Turn state changed:', canSendInput);
          }
          
          // Emit status change when turn state changes
          this.emitStatusChange();
        };
        
        // Subscribe to turn state changes
        this.turnManager.on('turn-state-changed', turnStateHandler);
        
        // Create unsubscribe function
        this.turnStateUnsubscribe = () => {
          this.turnManager?.off('turn-state-changed', turnStateHandler);
        };
        
        // Get initial turn state
        this.currentUserHasTurn = this.turnManager.canSendInput;
      }
    }
    
    // Emit status change
    this.emitStatusChange();
  }
  
  /**
   * Start streaming audio chunks to the connected client
   */
  async startStreaming(): Promise<void> {
    if (this.isStreaming) {
      if (this.config.debug) {
        // console.log('[AudioAgentCBridge] Already streaming');
      }
      return;
    }
    
    if (!this.client) {
      throw new Error('No client connected. Call setClient() first.');
    }
    
    if (this.config.debug) {
      console.warn('[AudioAgentCBridge] Starting audio streaming');
    }
    
    // Subscribe to audio chunks from AudioService
    this.audioChunkUnsubscribe = this.audioService.onAudioChunk(
      (chunk: AudioChunkData) => this.handleAudioChunk(chunk)
    );
    
    this.isStreaming = true;
    
    // Emit status change
    this.emitStatusChange();
    
    if (this.config.debug) {
      // console.log('[AudioAgentCBridge] Audio streaming started');
    }
  }
  
  /**
   * Stop streaming audio chunks
   */
  stopStreaming(): void {
    if (!this.isStreaming) {
      if (this.config.debug) {
        // console.log('[AudioAgentCBridge] Not currently streaming');
      }
      return;
    }
    
    if (this.config.debug) {
      console.warn('[AudioAgentCBridge] Stopping audio streaming');
    }
    
    // Unsubscribe from audio chunks
    if (this.audioChunkUnsubscribe) {
      this.audioChunkUnsubscribe();
      this.audioChunkUnsubscribe = null;
    }
    
    this.isStreaming = false;
    
    // Emit status change
    this.emitStatusChange();
    
    if (this.config.debug) {
      console.warn('[AudioAgentCBridge] Audio streaming stopped');
      // console.log(`[AudioAgentCBridge] Stats - Streamed: ${this.chunksStreamed}, Suppressed: ${this.chunksSuppressed}`);
    }
  }
  
  /**
   * Handle an audio chunk from AudioService
   */
  private handleAudioChunk(chunk: AudioChunkData): void {
    // Check if we should send this chunk
    const shouldSend = this.shouldSendChunk();
    
    if (shouldSend) {
      this.sendChunkToClient(chunk);
      this.chunksStreamed++;
      
      if (this.config.logAudioChunks) {
        // console.log('[AudioAgentCBridge] Sent audio chunk:', {
        //   frameCount: chunk.frame_count,
        //   bytes: chunk.content.byteLength,
        //   level: chunk.audio_level.toFixed(3)
        // });
      }
    } else {
      this.chunksSuppressed++;
      
      if (this.config.logAudioChunks) {
        // console.log('[AudioAgentCBridge] Suppressed audio chunk (no turn):', {
        //   frameCount: chunk.frame_count,
        //   bytes: chunk.content.byteLength
        // });
      }
    }
  }
  
  /**
   * Determine if we should send the current chunk
   */
  private shouldSendChunk(): boolean {
    // Check if client is connected
    if (!this.client || !this.client.isConnected()) {
      return false;
    }
    
    // If not respecting turn state, always send
    if (!this.config.respectTurnState) {
      return true;
    }
    
    // Check turn state
    return this.currentUserHasTurn;
  }
  
  /**
   * Send an audio chunk to the client
   */
  private sendChunkToClient(chunk: AudioChunkData): void {
    if (!this.client) {
      return;
    }
    
    try {
      // Send the raw PCM16 audio data as a binary frame
      this.client.sendBinaryFrame(chunk.content);
    } catch (error) {
      if (this.config.debug) {
        console.error('[AudioAgentCBridge] Failed to send audio chunk:', error);
      }
      
      // Client might have disconnected, stop streaming
      if (!this.client.isConnected()) {
        this.stopStreaming();
      }
    }
  }
  
  /**
   * Get current bridge status
   */
  getStatus(): AudioAgentCBridgeStatus {
    const isConnected = this.client?.isConnected() ?? false;
    
    return {
      isStreaming: this.isStreaming,
      isConnected,
      userHasTurn: this.currentUserHasTurn,
      chunksStreamed: this.chunksStreamed,
      chunksSuppressed: this.chunksSuppressed,
      respectingTurnState: this.config.respectTurnState,
      clientState: isConnected ? 'connected' : 
                   (this.client ? 'disconnected' : 'disconnected')
    };
  }
  
  /**
   * Subscribe to status changes
   * @returns Unsubscribe function
   */
  onStatusChange(callback: (status: AudioAgentCBridgeStatus) => void): () => void {
    return this.on('statusChange', callback);
  }
  
  /**
   * Reset statistics
   */
  resetStats(): void {
    this.chunksStreamed = 0;
    this.chunksSuppressed = 0;
    
    if (this.config.debug) {
      // console.log('[AudioAgentCBridge] Stats reset');
    }
    
    this.emitStatusChange();
  }
  
  /**
   * Update configuration
   */
  updateConfig(config: Partial<AudioAgentCBridgeConfig>): void {
    this.config = {
      ...this.config,
      ...config
    };
    
    if (this.config.debug) {
      // console.log('[AudioAgentCBridge] Configuration updated:', this.config);
    }
    
    this.emitStatusChange();
  }
  
  /**
   * Emit status change event
   */
  private emitStatusChange(): void {
    const status = this.getStatus();
    this.emit('statusChange', status);
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
          console.error(`[AudioAgentCBridge] Error in event listener for ${event}:`, error);
        }
      });
    }
  }
  
  /**
   * Clean up and release resources
   */
  private cleanup(): void {
    if (this.config.debug) {
      // console.log('[AudioAgentCBridge] Cleaning up');
    }
    
    // Stop streaming
    this.stopStreaming();
    
    // Clear client reference
    this.setClient(null);
    
    // Clear all listeners
    this.listeners.clear();
    
    // Reset state
    this.chunksStreamed = 0;
    this.chunksSuppressed = 0;
    this.currentUserHasTurn = false;
    
    if (this.config.debug) {
      // console.log('[AudioAgentCBridge] Cleanup complete');
    }
  }
}
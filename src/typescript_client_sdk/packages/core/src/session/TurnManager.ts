/**
 * Turn Manager - Simple turn state tracking for Agent C Realtime SDK
 * Tracks whether the user can send input based on server events
 */

import { EventEmitter } from '../events/EventEmitter';
import { RealtimeClient } from '../client/RealtimeClient';

/**
 * Event map for TurnManager events
 */
export interface TurnManagerEventMap {
  'turn-state-changed': { canSendInput: boolean };
}

/**
 * Simple turn management for tracking user input availability
 * Listens to server events and maintains a boolean state
 */
export class TurnManager extends EventEmitter<TurnManagerEventMap> {
  private client: RealtimeClient;
  private _canSendInput: boolean = false;
  private userTurnStartHandler: () => void;
  private userTurnEndHandler: () => void;

  /**
   * Create a new TurnManager instance
   * @param client - RealtimeClient instance to listen to events from
   */
  constructor(client: RealtimeClient) {
    super();
    this.client = client;
    
    // Create bound event handlers
    this.userTurnStartHandler = () => this.updateTurnState(true);
    this.userTurnEndHandler = () => this.updateTurnState(false);
    
    // Subscribe to turn events
    this.setupEventListeners();
  }

  /**
   * Get the current turn state
   * @returns true if user can send input, false otherwise
   */
  get canSendInput(): boolean {
    return this._canSendInput;
  }

  /**
   * Set up event listeners for turn management
   */
  private setupEventListeners(): void {
    // User can send input when their turn starts
    this.client.on('user_turn_start', this.userTurnStartHandler);

    // User cannot send input when their turn ends
    this.client.on('user_turn_end', this.userTurnEndHandler);
  }

  /**
   * Update the turn state and emit change event
   * @param canSendInput - New state for input availability
   */
  private updateTurnState(canSendInput: boolean): void {
    if (this._canSendInput !== canSendInput) {
      this._canSendInput = canSendInput;
      this.emit('turn-state-changed', { canSendInput });
    }
  }

  /**
   * Clean up event listeners and resources
   */
  cleanup(): void {
    // Remove listeners from the client
    this.client.off('user_turn_start', this.userTurnStartHandler);
    this.client.off('user_turn_end', this.userTurnEndHandler);
    
    // Remove all listeners from this emitter
    this.removeAllListeners();
  }

  /**
   * Destroy the turn manager
   * Alias for cleanup for consistency
   */
  destroy(): void {
    this.cleanup();
  }
}
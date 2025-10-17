/**
 * Manages WebSocket reconnection with exponential backoff strategy
 */

import { EventEmitter } from '../events/EventEmitter';
import { ReconnectionConfig } from './ClientConfig';

/**
 * Events emitted by the ReconnectionManager
 */
export interface ReconnectionEvents {
  'reconnecting': { attempt: number; delay: number };
  'reconnected': void;
  'reconnection_failed': { attempts: number; reason: string };
}

/**
 * Manages reconnection attempts with exponential backoff
 */
export class ReconnectionManager extends EventEmitter<ReconnectionEvents> {
  private config: ReconnectionConfig;
  private currentAttempt: number = 0;
  private currentDelay: number;
  private reconnectTimer?: ReturnType<typeof setTimeout>;
  private isReconnecting: boolean = false;
  private abortController?: AbortController;

  constructor(config: ReconnectionConfig) {
    super();
    this.config = config;
    this.currentDelay = config.initialDelay;
  }

  /**
   * Start reconnection process
   * @param reconnectFn - Function to call for reconnection attempt
   * @returns Promise that resolves when reconnected or rejects if max attempts reached
   */
  async startReconnection(
    reconnectFn: () => Promise<void>
  ): Promise<void> {
    if (!this.config.enabled) {
      throw new Error('Reconnection is disabled');
    }

    if (this.isReconnecting) {
      throw new Error('Reconnection already in progress');
    }

    this.isReconnecting = true;
    this.currentAttempt = 0;
    this.currentDelay = this.config.initialDelay;
    this.abortController = new AbortController();

    return this.attemptReconnection(reconnectFn);
  }

  /**
   * Stop any ongoing reconnection attempts
   */
  stopReconnection(): void {
    this.isReconnecting = false;
    this.abortController?.abort();
    
    if (this.reconnectTimer) {
      clearTimeout(this.reconnectTimer);
      this.reconnectTimer = undefined;
    }

    this.reset();
  }

  /**
   * Reset reconnection state
   */
  reset(): void {
    this.currentAttempt = 0;
    this.currentDelay = this.config.initialDelay;
    this.isReconnecting = false;
  }

  /**
   * Check if currently reconnecting
   */
  isActive(): boolean {
    return this.isReconnecting;
  }

  /**
   * Get current attempt number
   */
  getCurrentAttempt(): number {
    return this.currentAttempt;
  }

  /**
   * Attempt reconnection with backoff
   */
  private async attemptReconnection(
    reconnectFn: () => Promise<void>
  ): Promise<void> {
    while (this.isReconnecting) {
      // Check if max attempts reached
      if (this.config.maxAttempts > 0 && this.currentAttempt >= this.config.maxAttempts) {
        const error = `Maximum reconnection attempts (${this.config.maxAttempts}) reached`;
        this.emit('reconnection_failed', {
          attempts: this.currentAttempt,
          reason: error
        });
        this.stopReconnection();
        throw new Error(error);
      }

      this.currentAttempt++;
      
      // Calculate delay with jitter
      const jitteredDelay = this.calculateJitteredDelay();
      
      // Emit reconnecting event
      this.emit('reconnecting', {
        attempt: this.currentAttempt,
        delay: jitteredDelay
      });

      // Wait before attempting
      await this.delay(jitteredDelay);

      // Check if reconnection was cancelled during delay
      if (!this.isReconnecting || this.abortController?.signal.aborted) {
        throw new Error('Reconnection cancelled');
      }

      try {
        // Attempt to reconnect
        await reconnectFn();
        
        // Success - emit event and reset
        this.emit('reconnected', undefined);
        this.reset();
        return;
      } catch (error) {
        // Failed - calculate next delay
        this.currentDelay = Math.min(
          this.currentDelay * this.config.backoffMultiplier,
          this.config.maxDelay
        );

        // Log the error (could check a debug flag instead)
        console.warn(`Reconnection attempt ${this.currentAttempt} failed:`, error);

        // Continue to next attempt
      }
    }

    throw new Error('Reconnection stopped');
  }

  /**
   * Calculate delay with jitter
   */
  private calculateJitteredDelay(): number {
    const jitter = this.currentDelay * this.config.jitterFactor;
    const minDelay = this.currentDelay - jitter;
    const maxDelay = this.currentDelay + jitter;
    return Math.floor(minDelay + Math.random() * (maxDelay - minDelay));
  }

  /**
   * Delay helper that can be cancelled
   */
  private delay(ms: number): Promise<void> {
    return new Promise((resolve, reject) => {
      const abortHandler = () => {
        clearTimeout(this.reconnectTimer);
        reject(new Error('Delay cancelled'));
      };

      this.abortController?.signal.addEventListener('abort', abortHandler, { once: true });

      this.reconnectTimer = setTimeout(() => {
        this.abortController?.signal.removeEventListener('abort', abortHandler);
        resolve();
      }, ms);
    });
  }

  /**
   * Update configuration
   */
  updateConfig(config: Partial<ReconnectionConfig>): void {
    this.config = { ...this.config, ...config };
    
    // If disabled and currently reconnecting, stop
    if (!this.config.enabled && this.isReconnecting) {
      this.stopReconnection();
    }
  }

  /**
   * Get current configuration
   */
  getConfig(): ReconnectionConfig {
    return { ...this.config };
  }

  /**
   * Check if another retry should be attempted
   * @returns true if reconnection should continue, false otherwise
   */
  shouldRetry(): boolean {
    if (!this.config.enabled || !this.isReconnecting) {
      return false;
    }

    if (this.config.maxAttempts === 0) {
      return true; // Unlimited attempts
    }

    return this.currentAttempt < this.config.maxAttempts;
  }

  /**
   * Get the next reconnection delay
   * @returns Delay in milliseconds for the next reconnection attempt
   */
  getNextDelay(): number {
    // Calculate base delay for next attempt
    const nextDelay = Math.min(
      this.currentDelay * this.config.backoffMultiplier,
      this.config.maxDelay
    );

    // Apply jitter
    const jitter = nextDelay * this.config.jitterFactor;
    const minDelay = nextDelay - jitter;
    const maxDelay = nextDelay + jitter;
    
    return Math.floor(minDelay + Math.random() * (maxDelay - minDelay));
  }
}
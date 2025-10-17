/**
 * Type-safe event emitter for the Agent C Realtime SDK
 * Provides strongly-typed event handling with TypeScript generics
 */

import { Logger } from '../utils/logger';

/**
 * Event listener function type
 */
export type EventListener<T = any> = (event: T) => void;

/**
 * Event listener with metadata for management
 */
interface ListenerEntry<T = any> {
  listener: EventListener<T>;
  once: boolean;
}

/**
 * Type-safe event emitter class
 * @template TEventMap - Map of event types to event data types
 */
export class EventEmitter<TEventMap extends Record<string, any> = Record<string, any>> {
  private listeners: Map<keyof TEventMap, ListenerEntry<any>[]>;
  private maxListeners: number;
  private readonly HIGH_LISTENER_THRESHOLD = 10; // Warn when listener count exceeds this

  constructor() {
    this.listeners = new Map();
    this.maxListeners = 10;
  }

  /**
   * Register an event listener
   * @param event - Event type to listen for
   * @param listener - Callback function to execute when event is emitted
   * @returns This instance for chaining
   */
  on<K extends keyof TEventMap>(
    event: K,
    listener: EventListener<TEventMap[K]>
  ): this {
    return this.addListener(event, listener, false);
  }

  /**
   * Register a one-time event listener
   * @param event - Event type to listen for
   * @param listener - Callback function to execute once when event is emitted
   * @returns This instance for chaining
   */
  once<K extends keyof TEventMap>(
    event: K,
    listener: EventListener<TEventMap[K]>
  ): this {
    return this.addListener(event, listener, true);
  }

  /**
   * Remove an event listener
   * @param event - Event type to remove listener from
   * @param listener - Specific listener to remove
   * @returns This instance for chaining
   */
  off<K extends keyof TEventMap>(
    event: K,
    listener: EventListener<TEventMap[K]>
  ): this {
    const eventListeners = this.listeners.get(event);
    if (!eventListeners) return this;

    const index = eventListeners.findIndex(entry => entry.listener === listener);
    if (index !== -1) {
      eventListeners.splice(index, 1);
      if (eventListeners.length === 0) {
        this.listeners.delete(event);
      }
    }

    return this;
  }

  /**
   * Emit an event to all registered listeners
   * @param event - Event type to emit
   * @param data - Event data to pass to listeners
   * @returns True if event had listeners, false otherwise
   */
  emit<K extends keyof TEventMap>(
    event: K,
    data: TEventMap[K]
  ): boolean {
    const eventListeners = this.listeners.get(event);
    if (!eventListeners || eventListeners.length === 0) {
      return false;
    }

    // Create a copy to avoid modification during iteration
    const listenersToCall = [...eventListeners];

    // Remove one-time listeners before calling
    this.listeners.set(
      event,
      eventListeners.filter(entry => !entry.once)
    );

    // Call all listeners
    for (const entry of listenersToCall) {
      try {
        entry.listener(data);
      } catch (error) {
        console.error(`Error in event listener for "${String(event)}":`, error);
      }
    }

    return true;
  }

  /**
   * Remove all listeners for a specific event or all events
   * @param event - Optional event type to remove listeners for
   * @returns This instance for chaining
   */
  removeAllListeners<K extends keyof TEventMap>(event?: K): this {
    if (event) {
      this.listeners.delete(event);
    } else {
      this.listeners.clear();
    }
    return this;
  }

  /**
   * Get the number of listeners for a specific event
   * @param event - Event type to count listeners for
   * @returns Number of listeners
   */
  listenerCount<K extends keyof TEventMap>(event: K): number {
    const eventListeners = this.listeners.get(event);
    return eventListeners ? eventListeners.length : 0;
  }

  /**
   * Get all registered event names
   * @returns Array of event names
   */
  eventNames(): (keyof TEventMap)[] {
    return Array.from(this.listeners.keys());
  }

  /**
   * Set the maximum number of listeners per event
   * @param n - Maximum number of listeners (0 for unlimited)
   * @returns This instance for chaining
   */
  setMaxListeners(n: number): this {
    this.maxListeners = n;
    return this;
  }

  /**
   * Get the maximum number of listeners per event
   * @returns Maximum number of listeners
   */
  getMaxListeners(): number {
    return this.maxListeners;
  }

  /**
   * Get raw listeners for an event
   * @param event - Event type to get listeners for
   * @returns Array of listener functions
   */
  rawListeners<K extends keyof TEventMap>(
    event: K
  ): EventListener<TEventMap[K]>[] {
    const eventListeners = this.listeners.get(event);
    return eventListeners ? eventListeners.map(entry => entry.listener) : [];
  }

  /**
   * Alias for on()
   */
  addListener<K extends keyof TEventMap>(
    event: K,
    listener: EventListener<TEventMap[K]>,
    once: boolean = false
  ): this {
    let eventListeners = this.listeners.get(event);
    if (!eventListeners) {
      eventListeners = [];
      this.listeners.set(event, eventListeners);
    }

    // Check for duplicate listener registration (reference equality)
    const isDuplicate = eventListeners.some(entry => entry.listener === listener);
    if (isDuplicate) {
      Logger.warn('[EventEmitter] Duplicate listener registration detected', {
        event: String(event),
        currentCount: eventListeners.length,
        message: 'Attempted to register the same listener function twice - ignoring duplicate'
      });
      return this;
    }

    eventListeners.push({ listener, once });
    
    // Monitor listener count
    if (eventListeners.length > this.HIGH_LISTENER_THRESHOLD) {
      Logger.warn('[EventEmitter] High listener count detected', {
        event: String(event),
        count: eventListeners.length,
        threshold: this.HIGH_LISTENER_THRESHOLD,
        message: 'Possible listener leak - check for missing cleanup in useEffect'
      });
    }
    
    return this;
  }

  /**
   * Alias for off()
   */
  removeListener<K extends keyof TEventMap>(
    event: K,
    listener: EventListener<TEventMap[K]>
  ): this {
    return this.off(event, listener);
  }

  /**
   * Prepend a listener to the beginning of the listeners array
   * @param event - Event type to listen for
   * @param listener - Callback function to execute when event is emitted
   * @returns This instance for chaining
   */
  prependListener<K extends keyof TEventMap>(
    event: K,
    listener: EventListener<TEventMap[K]>
  ): this {
    let eventListeners = this.listeners.get(event);
    if (!eventListeners) {
      eventListeners = [];
      this.listeners.set(event, eventListeners);
    }

    // Check for duplicate listener registration (reference equality)
    const isDuplicate = eventListeners.some(entry => entry.listener === listener);
    if (isDuplicate) {
      Logger.warn('[EventEmitter] Duplicate listener registration detected', {
        event: String(event),
        currentCount: eventListeners.length,
        message: 'Attempted to register the same listener function twice - ignoring duplicate'
      });
      return this;
    }

    eventListeners.unshift({ listener, once: false });
    return this;
  }

  /**
   * Prepend a one-time listener to the beginning of the listeners array
   * @param event - Event type to listen for
   * @param listener - Callback function to execute once when event is emitted
   * @returns This instance for chaining
   */
  prependOnceListener<K extends keyof TEventMap>(
    event: K,
    listener: EventListener<TEventMap[K]>
  ): this {
    let eventListeners = this.listeners.get(event);
    if (!eventListeners) {
      eventListeners = [];
      this.listeners.set(event, eventListeners);
    }

    // Check for duplicate listener registration (reference equality)
    const isDuplicate = eventListeners.some(entry => entry.listener === listener);
    if (isDuplicate) {
      Logger.warn('[EventEmitter] Duplicate listener registration detected', {
        event: String(event),
        currentCount: eventListeners.length,
        message: 'Attempted to register the same listener function twice - ignoring duplicate'
      });
      return this;
    }

    eventListeners.unshift({ listener, once: true });
    return this;
  }

  /**
   * Get listener counts for all events (for debugging).
   * 
   * @returns Map of event names to listener counts
   */
  getListenerCounts(): Map<string, number> {
    const counts = new Map<string, number>();
    
    for (const [event, listeners] of this.listeners.entries()) {
      counts.set(String(event), listeners.length);
    }
    
    return counts;
  }

  /**
   * Log all listener counts (for debugging).
   */
  logListenerCounts(): void {
    Logger.debug('[EventEmitter] Current listener counts:');
    
    for (const [event, listeners] of this.listeners.entries()) {
      Logger.debug(`  ${String(event)}: ${listeners.length} listeners`);
    }
  }
}
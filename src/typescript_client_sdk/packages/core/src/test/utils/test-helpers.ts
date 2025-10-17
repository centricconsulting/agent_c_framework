/**
 * Test Helper Utilities
 * Common utilities for testing async operations and creating mock data
 */

import { vi } from 'vitest';
import type { EventRegistry, EventTypes } from '../../events/EventRegistry';

/**
 * Sleep for a specified number of milliseconds
 */
export function sleep(ms: number): Promise<void> {
  return new Promise(resolve => setTimeout(resolve, ms));
}

/**
 * Wait for a condition to be true
 */
export async function waitFor(
  condition: () => boolean | Promise<boolean>,
  options: {
    timeout?: number;
    interval?: number;
    message?: string;
  } = {}
): Promise<void> {
  const { timeout = 5000, interval = 50, message = 'Condition not met' } = options;
  const startTime = Date.now();

  while (Date.now() - startTime < timeout) {
    const result = await condition();
    if (result) {
      return;
    }
    await sleep(interval);
  }

  throw new Error(`Timeout: ${message}`);
}

/**
 * Wait for a specific number of event loop cycles
 */
export async function nextTick(cycles: number = 1): Promise<void> {
  for (let i = 0; i < cycles; i++) {
    await new Promise(resolve => setImmediate ? setImmediate(resolve) : setTimeout(resolve, 0));
  }
}

/**
 * Create a mock Agent C event
 */
export function createMockAgentCEvent<T extends keyof EventTypes>(
  type: T,
  data: Partial<EventTypes[T]> = {}
): EventTypes[T] {
  const timestamp = new Date().toISOString();
  
  const baseEvent = {
    type,
    timestamp,
    event_id: `evt_${Math.random().toString(36).substr(2, 9)}`,
  };

  // Handle specific event types
  switch (type) {
    case 'text_delta':
      return {
        ...baseEvent,
        content: '',
        role: 'assistant',
        ...data
      } as EventTypes[T];
    
    case 'text_done':
      return {
        ...baseEvent,
        content: '',
        role: 'assistant',
        ...data
      } as EventTypes[T];
    
    case 'audio_delta':
      return {
        ...baseEvent,
        delta: new ArrayBuffer(0),
        ...data
      } as EventTypes[T];
    
    case 'audio_done':
      return {
        ...baseEvent,
        ...data
      } as EventTypes[T];
    
    case 'turn_start':
      return {
        ...baseEvent,
        turn_id: `turn_${Math.random().toString(36).substr(2, 9)}`,
        type: 'assistant',
        ...data
      } as EventTypes[T];
    
    case 'turn_end':
      return {
        ...baseEvent,
        turn_id: `turn_${Math.random().toString(36).substr(2, 9)}`,
        ...data
      } as EventTypes[T];
    
    case 'error':
      return {
        ...baseEvent,
        error: {
          code: 'UNKNOWN_ERROR',
          message: 'An error occurred',
          details: {}
        },
        ...data
      } as EventTypes[T];
    
    case 'connected':
      return {
        ...baseEvent,
        session_id: `sess_${Math.random().toString(36).substr(2, 9)}`,
        ...data
      } as EventTypes[T];
    
    case 'disconnected':
      return {
        ...baseEvent,
        reason: 'Normal disconnection',
        code: 1000,
        ...data
      } as EventTypes[T];
    
    case 'tool_call':
      return {
        ...baseEvent,
        tool_call_id: `call_${Math.random().toString(36).substr(2, 9)}`,
        name: 'test_tool',
        arguments: {},
        ...data
      } as EventTypes[T];
    
    case 'tool_response':
      return {
        ...baseEvent,
        tool_call_id: `call_${Math.random().toString(36).substr(2, 9)}`,
        result: {},
        ...data
      } as EventTypes[T];
    
    default:
      return {
        ...baseEvent,
        ...data
      } as EventTypes[T];
  }
}

/**
 * Create a mock audio buffer
 */
export function createMockAudioBuffer(
  duration: number = 1000,
  sampleRate: number = 24000
): ArrayBuffer {
  const samples = Math.floor((duration / 1000) * sampleRate);
  const buffer = new ArrayBuffer(samples * 2); // 16-bit audio
  const view = new Int16Array(buffer);
  
  // Generate simple sine wave
  for (let i = 0; i < samples; i++) {
    view[i] = Math.sin(2 * Math.PI * 440 * i / sampleRate) * 32767;
  }
  
  return buffer;
}

/**
 * Create a deferred promise for testing async operations
 */
export function createDeferred<T = void>(): {
  promise: Promise<T>;
  resolve: (value: T) => void;
  reject: (error: Error) => void;
} {
  let resolve: (value: T) => void;
  let reject: (error: Error) => void;
  
  const promise = new Promise<T>((res, rej) => {
    resolve = res;
    reject = rej;
  });
  
  return {
    promise,
    resolve: resolve!,
    reject: reject!
  };
}

/**
 * Mock timer controls for testing time-dependent operations
 */
export class MockTimers {
  private originalSetTimeout: typeof setTimeout;
  private originalSetInterval: typeof setInterval;
  private originalClearTimeout: typeof clearTimeout;
  private originalClearInterval: typeof clearInterval;
  private originalDate: typeof Date;
  
  private timers: Map<number, { callback: Function; delay: number; type: 'timeout' | 'interval' }> = new Map();
  private currentTime: number = Date.now();
  private nextId: number = 1;

  constructor() {
    this.originalSetTimeout = global.setTimeout;
    this.originalSetInterval = global.setInterval;
    this.originalClearTimeout = global.clearTimeout;
    this.originalClearInterval = global.clearInterval;
    this.originalDate = global.Date;
  }

  install(): void {
    global.setTimeout = ((callback: Function, delay: number) => {
      const id = this.nextId++;
      this.timers.set(id, { callback, delay, type: 'timeout' });
      return id;
    }) as any;

    global.setInterval = ((callback: Function, delay: number) => {
      const id = this.nextId++;
      this.timers.set(id, { callback, delay, type: 'interval' });
      return id;
    }) as any;

    global.clearTimeout = ((id: number) => {
      this.timers.delete(id);
    }) as any;

    global.clearInterval = ((id: number) => {
      this.timers.delete(id);
    }) as any;

    // Mock Date
    const mockDate = class extends this.originalDate {
      constructor(...args: any[]) {
        if (args.length === 0) {
          super(this.currentTime);
        } else {
          super(...args);
        }
      }
      
      static now = () => this.currentTime;
    };
    
    global.Date = mockDate as any;
  }

  uninstall(): void {
    global.setTimeout = this.originalSetTimeout;
    global.setInterval = this.originalSetInterval;
    global.clearTimeout = this.originalClearTimeout;
    global.clearInterval = this.originalClearInterval;
    global.Date = this.originalDate;
    this.timers.clear();
  }

  advance(ms: number): void {
    const targetTime = this.currentTime + ms;
    
    while (this.currentTime < targetTime) {
      // Find the next timer to execute
      let nextTimer: { id: number; executeAt: number } | null = null;
      
      for (const [id, timer] of this.timers) {
        const executeAt = this.currentTime + timer.delay;
        if (executeAt <= targetTime) {
          if (!nextTimer || executeAt < nextTimer.executeAt) {
            nextTimer = { id, executeAt };
          }
        }
      }
      
      if (nextTimer) {
        this.currentTime = nextTimer.executeAt;
        const timer = this.timers.get(nextTimer.id)!;
        
        if (timer.type === 'timeout') {
          this.timers.delete(nextTimer.id);
        }
        
        timer.callback();
      } else {
        this.currentTime = targetTime;
      }
    }
  }

  runAll(): void {
    const maxIterations = 1000;
    let iterations = 0;
    
    while (this.timers.size > 0 && iterations < maxIterations) {
      iterations++;
      const timer = this.timers.values().next().value;
      
      if (timer.type === 'timeout') {
        this.timers.delete(this.timers.keys().next().value);
      }
      
      timer.callback();
    }
    
    if (iterations >= maxIterations) {
      throw new Error('Maximum timer iterations reached - possible infinite loop');
    }
  }
}

/**
 * Spy on console methods for testing
 */
export function spyOnConsole(): {
  log: jest.SpyInstance;
  error: jest.SpyInstance;
  warn: jest.SpyInstance;
  info: jest.SpyInstance;
  debug: jest.SpyInstance;
  restore: () => void;
} {
  const spies = {
    log: vi.spyOn(console, 'log').mockImplementation(() => {}),
    error: vi.spyOn(console, 'error').mockImplementation(() => {}),
    warn: vi.spyOn(console, 'warn').mockImplementation(() => {}),
    info: vi.spyOn(console, 'info').mockImplementation(() => {}),
    debug: vi.spyOn(console, 'debug').mockImplementation(() => {})
  };

  return {
    ...spies,
    restore: () => {
      Object.values(spies).forEach(spy => spy.mockRestore());
    }
  };
}

/**
 * Create a test event emitter for mocking
 */
export function createMockEventEmitter(): {
  emit: jest.Mock;
  on: jest.Mock;
  off: jest.Mock;
  once: jest.Mock;
  listeners: Map<string, Set<Function>>;
} {
  const listeners = new Map<string, Set<Function>>();

  const emit = vi.fn((event: string, ...args: any[]) => {
    const eventListeners = listeners.get(event);
    if (eventListeners) {
      eventListeners.forEach(listener => listener(...args));
    }
  });

  const on = vi.fn((event: string, listener: Function) => {
    if (!listeners.has(event)) {
      listeners.set(event, new Set());
    }
    listeners.get(event)!.add(listener);
  });

  const off = vi.fn((event: string, listener: Function) => {
    const eventListeners = listeners.get(event);
    if (eventListeners) {
      eventListeners.delete(listener);
    }
  });

  const once = vi.fn((event: string, listener: Function) => {
    const wrappedListener = (...args: any[]) => {
      listener(...args);
      off(event, wrappedListener);
    };
    on(event, wrappedListener);
  });

  return { emit, on, off, once, listeners };
}
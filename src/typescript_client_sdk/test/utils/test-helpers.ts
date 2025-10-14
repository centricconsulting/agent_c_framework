/**
 * Common test helper utilities
 */

import { vi } from 'vitest';

/**
 * Wait for a condition to be true
 */
export async function waitFor(
  condition: () => boolean | Promise<boolean>,
  timeout = 5000,
  interval = 100
): Promise<void> {
  const startTime = Date.now();
  
  while (Date.now() - startTime < timeout) {
    if (await condition()) {
      return;
    }
    await sleep(interval);
  }
  
  throw new Error(`Timeout waiting for condition after ${timeout}ms`);
}

/**
 * Sleep for a specified duration
 */
export function sleep(ms: number): Promise<void> {
  return new Promise(resolve => setTimeout(resolve, ms));
}

/**
 * Create a deferred promise
 */
export function createDeferred<T>() {
  let resolve: (value: T) => void;
  let reject: (error: any) => void;
  
  const promise = new Promise<T>((res, rej) => {
    resolve = res;
    reject = rej;
  });
  
  return {
    promise,
    resolve: resolve!,
    reject: reject!,
  };
}

/**
 * Flush all pending promises
 */
export async function flushPromises(): Promise<void> {
  await new Promise(resolve => setImmediate(resolve));
}

/**
 * Create a mock event
 */
export function createMockEvent(type: string, data?: any): Event {
  const event = new Event(type);
  if (data) {
    Object.assign(event, data);
  }
  return event;
}

/**
 * Create a mock message event
 */
export function createMockMessageEvent(data: any): MessageEvent {
  return new MessageEvent('message', { data });
}

/**
 * Create a mock error event
 */
export function createMockErrorEvent(error: Error): ErrorEvent {
  return new ErrorEvent('error', { error, message: error.message });
}

/**
 * Create a mock close event
 */
export function createMockCloseEvent(code = 1000, reason = 'Normal closure'): CloseEvent {
  return new CloseEvent('close', { code, reason });
}

/**
 * Mock timer functions
 */
export function useFakeTimers() {
  vi.useFakeTimers();
  
  return {
    advance: (ms: number) => vi.advanceTimersByTime(ms),
    runAll: () => vi.runAllTimers(),
    runPending: () => vi.runOnlyPendingTimers(),
    restore: () => vi.useRealTimers(),
  };
}

/**
 * Create a mock JWT token
 */
export function createMockJWT(payload: any = {}, expiresIn = 3600): string {
  const header = { alg: 'HS256', typ: 'JWT' };
  const defaultPayload = {
    sub: 'test-user',
    iat: Math.floor(Date.now() / 1000),
    exp: Math.floor(Date.now() / 1000) + expiresIn,
    ...payload,
  };
  
  const encodedHeader = btoa(JSON.stringify(header));
  const encodedPayload = btoa(JSON.stringify(defaultPayload));
  const signature = 'mock-signature';
  
  return `${encodedHeader}.${encodedPayload}.${signature}`;
}

/**
 * Decode a JWT token (mock implementation)
 */
export function decodeJWT(token: string): any {
  const parts = token.split('.');
  if (parts.length !== 3) {
    throw new Error('Invalid JWT token');
  }
  
  try {
    return JSON.parse(atob(parts[1]));
  } catch (error) {
    throw new Error('Failed to decode JWT token');
  }
}

/**
 * Create a spy that tracks calls but passes through to original
 */
export function createSpy<T extends (...args: any[]) => any>(fn: T): T & { calls: any[][] } {
  const calls: any[][] = [];
  
  const spy = vi.fn((...args: any[]) => {
    calls.push(args);
    return fn(...args);
  }) as T & { calls: any[][] };
  
  spy.calls = calls;
  
  return spy;
}

/**
 * Assert that a promise rejects with a specific error
 */
export async function expectReject(promise: Promise<any>, errorMessage?: string | RegExp) {
  try {
    await promise;
    throw new Error('Expected promise to reject');
  } catch (error: any) {
    if (errorMessage) {
      if (typeof errorMessage === 'string') {
        expect(error.message).toBe(errorMessage);
      } else {
        expect(error.message).toMatch(errorMessage);
      }
    }
    return error;
  }
}

/**
 * Create mock event data for Agent C events
 */
export function createMockAgentCEvent(type: string, data: any = {}) {
  return {
    type,
    timestamp: new Date().toISOString(),
    ...data,
  };
}

/**
 * Create mock audio data
 */
export function createMockAudioData(samples = 1024): ArrayBuffer {
  const buffer = new ArrayBuffer(samples * 2); // 16-bit audio
  const view = new Int16Array(buffer);
  
  // Generate a simple sine wave
  for (let i = 0; i < samples; i++) {
    view[i] = Math.sin((i / samples) * Math.PI * 2) * 32767;
  }
  
  return buffer;
}
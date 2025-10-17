import { vi } from 'vitest';

/**
 * Sets up fake timers for testing
 */
export const useFakeTimers = () => {
  vi.useFakeTimers();
  return {
    advance: (ms: number) => vi.advanceTimersByTime(ms),
    runAll: () => vi.runAllTimers(),
    runPending: () => vi.runOnlyPendingTimers(),
    restore: () => vi.useRealTimers()
  };
};

/**
 * Advances timers and awaits promises
 */
export const advanceTimersAndFlush = async (ms: number) => {
  vi.advanceTimersByTime(ms);
  await Promise.resolve(); // Flush microtasks
};

/**
 * Runs a test with fake timers and auto-cleanup
 */
export const withFakeTimers = async (testFn: () => void | Promise<void>) => {
  vi.useFakeTimers();
  try {
    await testFn();
  } finally {
    vi.useRealTimers();
  }
};

/**
 * Creates a mock timer/interval ID
 */
export const createMockTimer = () => {
  let id = 1;
  return {
    setTimeout: vi.fn(() => id++),
    clearTimeout: vi.fn(),
    setInterval: vi.fn(() => id++),
    clearInterval: vi.fn()
  };
};
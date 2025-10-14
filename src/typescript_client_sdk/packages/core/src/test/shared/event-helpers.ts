import { vi } from 'vitest';
import type { Mock } from 'vitest';

/**
 * Asserts an event was emitted with specific type
 */
export const expectEventEmitted = (emitFn: Mock, eventType: string) => {
  expect(emitFn).toHaveBeenCalledWith(
    eventType,
    expect.any(Object)
  );
};

/**
 * Asserts an event was emitted with specific type and partial data
 */
export const expectEventWithData = (emitFn: Mock, eventType: string, data: any) => {
  expect(emitFn).toHaveBeenCalledWith(
    eventType,
    expect.objectContaining(data)
  );
};

/**
 * Creates a simple mock event emitter
 */
export const createMockEventEmitter = () => ({
  emit: vi.fn(),
  on: vi.fn(),
  off: vi.fn(),
  once: vi.fn(),
  removeAllListeners: vi.fn()
});

/**
 * Gets all events of a specific type from mock calls
 */
export const getEmittedEvents = (emitFn: Mock, eventType: string) => 
  emitFn.mock.calls
    .filter(([type]) => type === eventType)
    .map(([, data]) => data);

/**
 * Asserts exact number of events were emitted
 */
export const expectEventCount = (emitFn: Mock, eventType: string, count: number) => {
  const events = getEmittedEvents(emitFn, eventType);
  expect(events).toHaveLength(count);
};
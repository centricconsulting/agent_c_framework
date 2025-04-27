/**
 * Safe serialization utilities - Performance optimized version
 *
 * This module has been completely disabled for performance.
 * All functions are no-ops that return minimal values to prevent errors.
 */

// Complete no-op implementation
export function toSafeString(value) {
  if (value === null) return 'null';
  if (value === undefined) return 'undefined';
  return typeof value === 'object' ? '[Object]' : String(value);
}

// Complete no-op implementation
export function safeStringify() {
  return '[Object]';
}

// Complete no-op implementation
export function safeInspect(obj) {
  return obj;
}

// Return a no-op logger
export function createSafeLogger() {
  return {
    debug: () => {},
    log: () => {},
    info: () => {},
    warn: () => {},
    error: () => {}
  };
}

export default {
  safeStringify,
  safeInspect,
  createSafeLogger
};